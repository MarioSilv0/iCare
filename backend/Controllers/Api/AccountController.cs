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
using System.Web;

namespace backend.Controllers.Api
{
    //Mário with 'PeopleAngular(identity)'
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly UserLogService _userLogService;
        private readonly EmailSenderService _emailService;

        public AccountController(UserManager<User> userManager, UserLogService userLogService, IConfiguration configuration, SignInManager<User> signInManager, EmailSenderService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _userLogService = userLogService;
            _emailService = emailService;
        }

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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] LoginModel model)
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

            var user = new User
            {
                Email = model.Email,
                UserName = model.Email.Split('@')[1],
                Name = model.Email.Split('@')[1]
            };
            Console.WriteLine("User:", user);

            await _userManager.AddToRoleAsync(user, "User");

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userLogService.LogAsync(user.Id, $"Account created successfully. Please confirm your email: {model.Email}");
                return Ok(new { message = "Account created successfully. Please confirm your email." });
            }

            await _userLogService.LogAsync(null, $"Registration failed: {model.Email}");
            return BadRequest(new { message = "Registration failed.", errors = result.Errors });
        }

        public class LoginModel
        {
            public required string Email { get; set; }

            public required string Password { get; set; }

        }

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
                    if (!result.Succeeded) return BadRequest(result.Errors);

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

                // Retornar o token ao cliente
                return Ok(new
                {
                    token = tokenString,
                    expiration = token.ValidTo,
                    message = "Login via Google realizado com sucesso!"
                });
            }
            catch (InvalidJwtException ex)
            {
                return BadRequest(new { Message = "Token do Google inválido", Error = ex.Message });
            }
        }

        public class GoogleLoginRequest
        {
            public required string IdToken { get; set; }
        }

        [HttpPost("recover-password")]
        public async Task<IActionResult> RecoverPassword([FromBody] ForgotPasswordModel model)
        {
            Console.WriteLine("RecoverPassword", model);
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { message = "Email não encontrado." });

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

            return Ok(new { message = "Instruções para redefinição de senha enviadas para o e-mail." });
        }

        public class ForgotPasswordModel
        {
            public required string Email { get; set; }
            public required string ClientUrl { get; set; }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest(new { message = "Utilizador não encontrado." });

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(new { message = "Erro ao redefinir a senha.", errors = result.Errors });

            return Ok(new { message = "Senha redefinida com sucesso." });
        }
        public class ResetPasswordModel
        {
            public required string Email { get; set; }
            public required string Token { get; set; }
            public required string NewPassword { get; set; }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            Console.WriteLine("Claims do Token: ");
            userClaims.ForEach(c => Console.WriteLine($"{c.Type}: {c.Value}"));

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
                return BadRequest(new { message = "Erro ao alterar senha.", errors = result.Errors });

            await _signInManager.RefreshSignInAsync(user);
            return Ok(new { message = "Senha alterada com sucesso!" });
        }
        public class ChangePasswordModel
        {
            public required string CurrentPassword { get; set; }
            public required string NewPassword { get; set; }
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] EmailModel model)
        {
            if (string.IsNullOrEmpty(model.To) || string.IsNullOrEmpty(model.Subject) || string.IsNullOrEmpty(model.Body))
                return BadRequest("Invalid email request.");

            await _emailService.SendEmailAsync(model.To, model.Subject, model.Body);
            return Ok("Email sent successfully.");
        }
        public class EmailModel
        {
            public required string To { get; set; }
            public required string Subject { get; set; }
            public required string Body { get; set; }
        }


    }
}
