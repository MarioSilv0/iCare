using backend.Controllers.Api;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace backendtest.Controllers.Api
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly AccountController _controller;
        private readonly ICareServerContext _careServerContext;

        public AccountControllerTests()
        {
            // Cria um banco de dados in-memory (usando um nome único para isolar os testes)
            var options = new DbContextOptionsBuilder<ICareServerContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _careServerContext = new ICareServerContext(options);

            // Configura o UserManager com um mock
            var userStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                userStore.Object,
                null, null, null, null, null, null, null, null);

            // Configura o SignInManager com um mock
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null);

            // Configura o IConfiguration (para JWT e outras dependências, se necessário)
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["Jwt:Key"])
                .Returns("your-test-secret-key-that-is-long-enough-for-testing");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("test-audience");

            // Configura o IHttpContextAccessor
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockHttpContextAccessor.Setup(h => h.HttpContext)
                .Returns(new DefaultHttpContext());

            // Instancia os serviços reais (UserLogService e EmailSenderService)
            var userLogService = new UserLogService(_careServerContext, _mockHttpContextAccessor.Object);

            // Configura o EmailSettings e encapsula-o em IOptions<EmailSettings>
            var emailSettings = new EmailSettings
            {
                SmtpServer = "smtp.gmail.com",
                Port = 587,
                SenderEmail = "marioextr@gmail.com",
                SenderPassword = "iqxn etsa qgxp efsk",
                EnableSsl = true
            };
            var emailSenderService = new EmailSenderService(Options.Create(emailSettings));

            // Instancia o controller, injetando as dependências
            _controller = new AccountController(
                _mockUserManager.Object,
                userLogService,
                _mockConfiguration.Object,
                _mockSignInManager.Object,
                emailSenderService
            );
        }

        /// <summary>
        /// Obtém o valor de uma propriedade de um objeto via reflection.
        /// Retorna null caso a propriedade não exista.
        /// </summary>
        private static string GetPropertyValue(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            return prop?.GetValue(obj, null)?.ToString();
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginModel = new AccountController.LoginModel
            {
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new User
            {
                Id = "testUserId",
                Email = loginModel.Email,
                Name = "TestUser"
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
            var resultValue = okResult.Value;

            // Acessa a propriedade "token" via reflection
            var token = GetPropertyValue(resultValue, "token");
            Assert.False(string.IsNullOrEmpty(token));

            // Acessa a propriedade "message" via reflection
            var message = GetPropertyValue(resultValue, "message");
            Assert.Equal("Login successful", message);
        }

        [Fact]
        public async Task Login_WithInvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");
            var loginModel = new AccountController.LoginModel
            {
                Email = "",
                Password = "Password123!"
            };

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var message = GetPropertyValue(badRequestResult.Value, "message");
            Assert.Equal("Invalid data.", message);
        }


        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var loginModel = new AccountController.LoginModel
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
            var registerModel = new AccountController.RegisterModel
            {
                Email = "mario@gmail.com",
                Password = "Password123!",
                ClientUrl = "http://localhost:3000"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(registerModel.Email))
                .ReturnsAsync((User)null);
            // O controller chama AddToRoleAsync ANTES de CreateAsync
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "User"))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), registerModel.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync("confirmation-token");

            // Act
            var result = await _controller.Register(registerModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = GetPropertyValue(okResult.Value, "message");
            Assert.Contains("Account created successfully", message);
        }

        [Fact]
        public async Task Register_WithExistingEmail_ReturnsConflict()
        {
            // Arrange
            var registerModel = new AccountController.RegisterModel
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
            var changePasswordModel = new AccountController.ChangePasswordModel
            {
                CurrentPassword = "CurrentPassword123!",
                NewPassword = "NewPassword123!"
            };

            var user = new User { Id = userId, Email = "test@example.com", Name = "TestUser" };

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
            _mockUserManager.Setup(x => x.ChangePasswordAsync(user, changePasswordModel.CurrentPassword, changePasswordModel.NewPassword))
                .ReturnsAsync(IdentityResult.Success);
            _mockSignInManager.Setup(x => x.RefreshSignInAsync(user))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangePassword(changePasswordModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = GetPropertyValue(okResult.Value, "message");
            Assert.Contains("Senha alterada com sucesso", message);
        }


        [Fact]
        public async Task RecoverPassword_WithValidEmail_ReturnsOkResult()
        {
            // Arrange
            var model = new AccountController.ForgotPasswordModel
            {
                Email = "mario@gmail.com",
                ClientUrl = "http://localhost:3000"
            };

            var user = new User
            {
                Id = "testUserId",
                Email = model.Email,
                Name = "TestUser"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset-token");

            // Act
            var result = await _controller.RecoverPassword(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = GetPropertyValue(okResult.Value, "message");
            Assert.Contains("Instruções para redefinição de senha", message);
        }

        [Fact]
        public async Task ResetPassword_WithValidModel_ReturnsOkResult()
        {
            // Arrange
            var model = new AccountController.ResetPasswordModel
            {
                Email = "test@example.com",
                Token = "valid-token",
                NewPassword = "NewPassword123!"
            };

            var user = new User { Id = "testUserId", Email = model.Email, Name = "TestUser" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ResetPasswordAsync(user, model.Token, model.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ResetPassword(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = GetPropertyValue(okResult.Value, "message");
            Assert.Contains("Senha redefinida com sucesso", message);
        }

        [Fact]
        public async Task ConfirmEmail_WithValidToken_ReturnsOkResult()
        {
            // Arrange
            var email = "test@example.com";
            var token = "valid-token";
            var user = new User { Id = "testUserId", Email = email, Name = "TestUser" };

            _mockUserManager.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ConfirmEmailAsync(user, token))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ConfirmEmail(email, token);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = GetPropertyValue(okResult.Value, "message");
            Assert.Contains("Email confirmado com sucesso", message);
        }

        [Fact]
        public async Task GoogleLogin_WithInvalidToken_ReturnsBadRequest()
        {
            // Arrange
            var request = new AccountController.GoogleLoginRequest
            {
                IdToken = "invalid-google-token"
            };

            // Act
            var result = await _controller.GoogleLogin(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            // Neste caso, a propriedade retornada é "Message" com M maiúsculo
            var message = GetPropertyValue(badRequestResult.Value, "Message");
            Assert.Equal("Token do Google inválido", message);
        }

    }
}
