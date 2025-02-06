using backend.Controllers.Api;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using static backend.Controllers.Api.AccountController;
using Microsoft.AspNetCore.Http;
using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backendtest
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ICareServerContext> _mockCareServerContext; // Mock for ICareServerContext
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor; // Mock for IHttpContextAccessor
        private readonly Mock<UserLogService> _mockUserLogService; // Mock for UserLogService
        private readonly Mock<EmailSenderService> _mockEmailService; // Assuming EmailSenderService implements IEmailSenderService
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var options = new DbContextOptionsBuilder<ICareServerContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            // Setup UserManager mock
            var userStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStore.Object,
                null, null, null, null, null, null, null, null);

            // Setup SignInManager mock
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(_mockUserManager.Object,
                contextAccessor.Object, claimsFactory.Object, null, null, null, null);

            _mockConfiguration = new Mock<IConfiguration>();
            _mockCareServerContext = new Mock<ICareServerContext>(options); // Mock for ICareServerContext
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>(); // Mock for IHttpContextAccessor
            _mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(new DefaultHttpContext());
            _mockUserLogService = new Mock<UserLogService>(_mockCareServerContext.Object, _mockHttpContextAccessor.Object); // Mocking UserLogService
            _mockEmailService = new Mock<EmailSenderService>(); // Mocking EmailSenderService

            // Setup Configuration mock for JWT
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("your-test-secret-key-that-is-long-enough-for-testing");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("test-audience");

            _controller = new AccountController(
                _mockUserManager.Object,
                _mockUserLogService.Object,
                _mockConfiguration.Object,
                _mockSignInManager.Object,
                _mockEmailService.Object
            );
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = "testUserId",
                Email = loginModel.Email,
                Name = "Test User"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, loginModel.Password))
                .ReturnsAsync(true);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value as dynamic;
            Assert.NotNull(value.token);
            Assert.Equal("Login successful", value.message);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginModel.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Register_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "newuser@example.com",
                Password = "Password123!",
                ClientUrl = "http://localhost:3000"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerModel.Email))
                .ReturnsAsync((User)null);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), registerModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "User"))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync("confirmation-token");

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Account created successfully", (okResult.Value as dynamic).message);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsConflict()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "existing@example.com",
                Password = "Password123!",
                ClientUrl = "http://localhost:3000"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerModel.Email))
                .ReturnsAsync(new User());

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            Assert.IsType<ConflictObjectResult>(result);
        }

        [Fact]
        public async Task ChangePassword_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var userId = "testUserId";
            var changePasswordModel = new ChangePasswordModel
            {
                CurrentPassword = "CurrentPassword123!",
                NewPassword = "NewPassword123!"
            };

            var user = new User { Id = userId, Email = "test@example.com" };

            var claims = new List<Claim> { new Claim("UserId", userId) };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.CheckPasswordAsync(user, changePasswordModel.CurrentPassword))
                .ReturnsAsync(true);
            _mockUserManager.Setup(x => x.ChangePasswordAsync(user, 
                changePasswordModel.CurrentPassword, changePasswordModel.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ChangePassword(changePasswordModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Senha alterada com sucesso", (okResult.Value as dynamic).message);
        }

        [Fact]
        public async Task RecoverPassword_WithValidEmail_ReturnsOkResult()
        {
            // Arrange
            var model = new ForgotPasswordModel
            {
                Email = "test@example.com",
                ClientUrl = "http://localhost:3000"
            };

            var user = new User
            {
                Id = "testUserId",
                Email = model.Email,
                Name = "Test User"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset-token");

            // Act
            var result = await _controller.RecoverPassword(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Instruções para redefinição de senha", (okResult.Value as dynamic).message);
        }

        [Fact]
        public async Task ConfirmEmail_WithValidToken_ReturnsOkResult()
        {
            // Arrange
            var email = "test@example.com";
            var token = "valid-token";
            var user = new User { Email = email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ConfirmEmail(email, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Email confirmed successfully", (okResult.Value as dynamic).message);
        }

        [Fact]
        public async Task ResetPassword_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var model = new ResetPasswordModel
            {
                Email = "test@example.com",
                Token = "valid-token",
                NewPassword = "NewPassword123!"
            };

            var user = new User { Email = model.Email };

            _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ResetPasswordAsync(user, model.Token, model.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Senha redefinida com sucesso", (okResult.Value as dynamic).message);
        }

        [Fact]
        public async Task GoogleLogin_WithValidToken_ReturnsOkResult()
        {
            // Arrange
            var request = new GoogleLoginRequest
            {
                IdToken = "valid-google-token"
            };

            var user = new User
            {
                Id = "testUserId",
                Email = "test@gmail.com",
                Name = "Test User"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { "User" });

            // Note: This test assumes the Google token validation will succeed
            // In a real test, you might want to mock the GoogleJsonWebSignature.ValidateAsync method

            // Act
            var result = await _controller.GoogleLogin(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull((okResult.Value as dynamic).token);
        }
    }
}