using backend.Models;
using backend.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Controllers.Api
{
    /// <author>Mário Silva - 202000500</author>
    /// <summary>
    /// Controller responsible for managing user accounts and authentication.
    /// Handles user registration, login, password management, and email operations.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for:
    /// - User registration and email confirmation
    /// - User authentication (including Google OAuth)
    /// - Password management (change, reset, recovery)
    /// </remarks>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly UserLogService _userLogService;
        private readonly EmailSenderService _emailService;

        /// <summary>
        /// Initializes a new instance of the AccountController.
        /// </summary>
        /// <param name="userManager">The ASP.NET Core Identity user manager.</param>
        /// <param name="userLogService">Service for logging user activities.</param>
        /// <param name="configuration">Application configuration instance.</param>
        /// <param name="signInManager">The ASP.NET Core Identity sign-in manager.</param>
        /// <param name="emailService">Service for sending emails.</param>
        public AccountController(UserManager<User> userManager, UserLogService userLogService, IConfiguration configuration, SignInManager<User> signInManager, EmailSenderService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _userLogService = userLogService;
            _emailService = emailService;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="model">The login credentials containing email and password.</param>
        /// <returns>
        /// Returns an IActionResult containing:
        /// - 200 OK with JWT token if authentication is successful
        /// - 400 Bad Request if credentials are invalid
        /// </returns>
        /// <remarks>
        /// The generated JWT token contains user claims including:
        /// - Name
        /// - Email
        /// - User ID
        /// - Roles
        /// </remarks>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                await _userLogService.LogAsync(null, $"Invalid data: {model.Email}, {model.Password}");
                return BadRequest(new { message = "Invalid data.", errors = ModelState });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await _userLogService.LogAsync(null, $"Failed login attempt for email: {model.Email}");
                return BadRequest("Invalid login credentials.");
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                await _userLogService.LogAsync(null, $"Failed login attempt for email: {model.Email}");
                return BadRequest("Invalid login credentials.");
            }

            // Gerar os claims do utilizador
            var claims = new List<Claim>
            {
                new("Name", user.Name),
                new("Email", user.Email),
                new("UserId", user.Id)
            };
            // Obtém as roles do Identity
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Gerar o token JWT
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1), // Definir o tempo de expiração
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Retornar o token ao cliente
            return Ok(new
            {
                token = tokenString,
                roles = roles,
                expiration = token.ValidTo,
                message = "Login successful"
            });
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="model">Registration details including email, password, and client URL.</param>
        /// <returns>
        /// Returns an IActionResult containing:
        /// - 200 OK if registration is successful
        /// - 400 Bad Request if registration data is invalid
        /// - 409 Conflict if email is already registered
        /// </returns>
        /// <remarks>
        /// After successful registration:
        /// - User is assigned the "User" role
        /// - Confirmation email is sent to user's email address
        /// </remarks>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                await _userLogService.LogAsync(null, $"Invalid data: {model.Email}, {model.Password}");
                return BadRequest(new { message = "Invalid data.", errors = ModelState });
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                await _userLogService.LogAsync(null, $"This email is already registered: {model.Email}");
                return Conflict(new { message = "This email is already registered." });
            }

            var user = new User()
            {
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                Name = model.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"{model.ClientUrl}/confirm-email?email={model.Email}&token={Uri.EscapeDataString(token)}";

                var subject = "Please confirm your email address";
                var body = $@"
                    <p>Olá {user.Name},</p>
                    <p>Por favor, confirme seu endereço de e-mail clicando no link abaixo:</p>
                    <p><a href='{confirmationLink}'>Confirmar E-mail</a></p>
                    <p>Se você não solicitou isso, ignore este e-mail.</p>
                    <p>Obrigado!</p>";

                await _emailService.SendEmailAsync(user.Email, subject, body);

                await _userLogService.LogAsync(user.Id, $"Account created successfully. Please confirm your email: {model.Email}");
                return Ok(new { message = "Account created successfully. Please confirm your email." });
            }


            await _userLogService.LogAsync(null, $"Registration failed: {model.Email}");
            return BadRequest(new { message = "Registration failed.", errors = result.Errors });
        }

        /// <summary>
        /// Handles Google OAuth authentication.
        /// </summary>
        /// <param name="request">Contains the Google ID token.</param>
        /// <returns>
        /// Returns an IActionResult containing:
        /// - 200 OK with JWT token if authentication is successful
        /// - 400 Bad Request if Google token is invalid
        /// </returns>
        /// <remarks>
        /// - Creates new user account if email doesn't exist
        /// - Automatically confirms email for Google users
        /// - Assigns "User" role to new accounts
        /// </remarks>
        [HttpPost("google-login")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            // Validar o token do Google
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = ["452109114218-ld8o3eiqgar6jg6h42r6q3fvqsevfiv4.apps.googleusercontent.com"]
            };
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

                // Buscar ou criar o utilizador no banco de dados
                var user = await _userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    // Criar novo utilizador
                    user = new User
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        EmailConfirmed = true,
                        Name = payload.Name,
                        Picture = payload.Picture,
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        await _userLogService.LogAsync(null, $"Google login failed for {payload.Email}");
                        return BadRequest(result.Errors);
                    }

                    await _userManager.AddToRoleAsync(user, "User");
                }

                // Gerar os claims do utilizador
                var claims = new List<Claim>
                {
                    new("Name", user.Name),
                    new("Email", user.Email),
                    new("UserId", user.Id)
                };
                // Obtém as roles do Identity
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


                // Gerar o token JWT
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1), // Definir o tempo de expiração
                    signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                await _userLogService.LogAsync(user.Id, $"Google login successful for {user.Email}");

                return Ok(new
                {
                    token = tokenString,
                    expiration = token.ValidTo,
                    message = "Login via Google realizado com sucesso!"
                });
            }
            catch (InvalidJwtException ex)
            {
                await _userLogService.LogAsync(null, $"Google login failed: {ex.Message}");
                return BadRequest(new { Message = "Token do Google inválido", Error = ex.Message });
            }
        }

        /// <summary>
        /// Initiates the password recovery process.
        /// </summary>
        /// <param name="model">Contains email and client URL for password reset.</param>
        /// <returns>
        /// Returns an IActionResult containing:
        /// - 200 OK if recovery email is sent successfully
        /// - 400 Bad Request if email is not found
        /// </returns>
        /// <remarks>
        /// Sends an email with a password reset link to the user's email address.
        /// </remarks>
        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] ForgotPasswordModel model)
        {
            Console.WriteLine("RecoverPassword", model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await _userLogService.LogAsync(null, $"Recover password attempt for non-existent email: {model.Email}");
                return BadRequest(new { message = "Email não encontrado." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"{model.ClientUrl}/reset-password?email={model.Email}&token={Uri.EscapeDataString(token)}";

            // Enviar email com o link de redefinição de senha
            var subject = "Redefinição de Senha";
            var body = $@"
                        <p>Olá {user.Name},</p>
                        <p>Recebemos um pedido para redefinir sua senha. Se foi você, clique no link abaixo:</p>
                        <p><a href='{resetLink}'>Redefinir Senha</a></p>
                        <p>Se você não solicitou isso, ignore este e-mail.</p>
                        <p>Obrigado!</p>";

            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
                return BadRequest("Invalid email request.");

            await _emailService.SendEmailAsync(user.Email, subject, body);
            await _userLogService.LogAsync(user.Id, $"Password recovery email sent to {user.Email}");

            return Ok(new { message = "Instruções para redefinição de senha enviadas para o e-mail." });
        }

        /// <summary>
        /// Resets user's password using a reset token.
        /// </summary>
        /// <param name="model">Contains email, reset token, and new password.</param>
        /// <returns>
        /// Returns an IActionResult containing:
        /// - 200 OK if password is reset successfully
        /// - 400 Bad Request if token is invalid or user not found
        /// </returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                await _userLogService.LogAsync(null, $"Reset password attempt for non-existent email: {model.Email}");
                return BadRequest(new { message = "Utilizador não encontrado." });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
            {
                await _userLogService.LogAsync(user.Id, $"Password reset failed for {user.Email}");
                return BadRequest(new { message = "Erro ao redefinir a senha.", errors = result.Errors });
            }

            await _userLogService.LogAsync(user.Id, $"Password reset successful for {user.Email}");
            return Ok(new { message = "Senha redefinida com sucesso." });
        }

        /// <summary>
        /// Changes the password of an authenticated user.
        /// </summary>
        /// <param name="model">Contains current password and new password.</param>
        /// <returns>
        /// Returns an IActionResult containing:
        /// - 200 OK if password is changed successfully
        /// - 400 Bad Request if current password is incorrect
        /// - 401 Unauthorized if user is not authenticated
        /// </returns>
        /// <remarks>
        /// Requires JWT authentication.
        /// </remarks>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

            var userId = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Token inválido ou usuário não autenticado." });

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Unauthorized(new { message = "Utilizador não encontrado." });


            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!isPasswordCorrect)
                return BadRequest(new { message = "Senha atual incorreta." });


            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            Console.WriteLine(result);
            if (!result.Succeeded)
            {
                await _userLogService.LogAsync(user.Id, $"Password change failed for {user.Email}");
                return BadRequest(new { message = "Erro ao alterar senha.", errors = result.Errors });
            }
            await _signInManager.RefreshSignInAsync(user);
            await _userLogService.LogAsync(user.Id, $"Password changed successfully for {user.Email}");

            return Ok(new { message = "Senha alterada com sucesso!" });
        }

        /// <summary>
        /// Confirms a user's email address.
        /// </summary>
        /// <param name="email">The email address to confirm.</param>
        /// <param name="token">The confirmation token.</param>
        /// <returns>
        /// Returns an IActionResult containing:
        /// - 200 OK if email is confirmed successfully
        /// - 400 Bad Request if token is invalid or user not found
        /// </returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                await _userLogService.LogAsync(user.Id, $"Email confirmed for {user.Email}");
                return Ok(new { message = "Email confirmado com sucesso!" });
            }

            await _userLogService.LogAsync(user.Id, $"Error confirming email for {user.Email}");
            return BadRequest(new { message = "Error confirming email." });
        }

        /// <summary>
        /// Model class for user login credentials.
        /// </summary>
        public class LoginModel
        {
            /// <summary>
            /// Gets or sets the user's email address.
            /// </summary>
            public required string Email { get; set; }

            /// <summary>
            /// Gets or sets the user's password.
            /// </summary>
            public required string Password { get; set; }
        }

        /// <summary>
        /// Model class for user registration.
        /// </summary>
        public class RegisterModel
        {
            /// <summary>
            /// Gets or sets the email address for registration.
            /// </summary>
            public required string Email { get; set; }

            /// <summary>
            /// Gets or sets the password for the new account.
            /// </summary>
            public required string Password { get; set; }

            /// <summary>
            /// Gets or sets the client URL for email confirmation.
            /// </summary>
            public required string ClientUrl { get; set; }
        }

        /// <summary>
        /// Model class for Google login requests.
        /// </summary>
        public class GoogleLoginRequest
        {
            /// <summary>
            /// Gets or sets the Google ID token for authentication.
            /// </summary>
            public required string IdToken { get; set; }
        }

        /// <summary>
        /// Model class for password recovery requests.
        /// </summary>
        public class ForgotPasswordModel
        {
            /// <summary>
            /// Gets or sets the email address for password recovery.
            /// </summary>
            public required string Email { get; set; }

            /// <summary>
            /// Gets or sets the client URL for password reset.
            /// </summary>
            public required string ClientUrl { get; set; }
        }

        /// <summary>
        /// Model class for password reset operations.
        /// </summary>
        public class ResetPasswordModel
        {
            /// <summary>
            /// Gets or sets the email address of the account.
            /// </summary>
            public required string Email { get; set; }

            /// <summary>
            /// Gets or sets the password reset token.
            /// </summary>
            public required string Token { get; set; }

            /// <summary>
            /// Gets or sets the new password.
            /// </summary>
            public required string NewPassword { get; set; }
        }

        /// <summary>
        /// Model class for password change operations.
        /// </summary>
        public class ChangePasswordModel
        {
            /// <summary>
            /// Gets or sets the current password.
            /// </summary>
            public required string CurrentPassword { get; set; }

            /// <summary>
            /// Gets or sets the new password.
            /// </summary>
            public required string NewPassword { get; set; }
        }

        /// <summary>
        /// Model class for email operations.
        /// </summary>
        public class EmailModel
        {
            /// <summary>
            /// Gets or sets the recipient's email address.
            /// </summary>
            public required string To { get; set; }

            /// <summary>
            /// Gets or sets the email subject.
            /// </summary>
            public required string Subject { get; set; }

            /// <summary>
            /// Gets or sets the email body content.
            /// </summary>
            public required string Body { get; set; }
        }
    }
}
