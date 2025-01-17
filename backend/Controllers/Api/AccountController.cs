using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using backend.Services;
using System.Security.Claims;

namespace backend.Controllers.Api
{
    //Mário
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserLogService _userLogService;

        public AccountController(SignInManager<User> signInManager, UserLogService userLogService)
        {
            _signInManager = signInManager;
            _userLogService = userLogService;
        }

        [HttpGet("isAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok(User.Identity.IsAuthenticated);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                await _userLogService.LogAsync(null, $"Invalid data: {model.Email}");
                return BadRequest(new { message = "Invalid data.", errors = ModelState });
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

            if (result.Succeeded)
            {
                var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
                await _userLogService.LogAsync(user?.Id, "User logged in successfully.");

                return Ok(new { message = "Login successful" });
            }

            await _userLogService.LogAsync(null, $"Failed login attempt for email: {model.Email}");
            return Unauthorized("Invalid login credentials.");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User ID not found.");

            await _userLogService.LogAsync(userId, "User logged out.");
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                await _userLogService.LogAsync(null, $"Invalid data: {model.Email}");
                return BadRequest(new { message = "Invalid data.", errors = ModelState });
            }

            var existingUser = await _signInManager.UserManager.FindByEmailAsync(model.Email);
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

            var result = await _signInManager.UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userLogService.LogAsync(user.Id, $"Account created successfully. Please confirm your email: {model.Email}");
                return Ok(new { message = "Account created successfully. Please confirm your email." });
            }

            await _userLogService.LogAsync(null, $"Registration failed: {model.Email}");
            return BadRequest(new { message = "Registration failed.", errors = result.Errors });
        }

    }

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

}
