using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthControllerTests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock;
        private Mock<ILogger<AuthController>> _loggerMock;
        private AuthController _controller;

        [SetUp]
        public void SetUp()
        {
            _authServiceMock = new Mock<IAuthService>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_authServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task Register_ValidDto_ReturnsOk()
        {
            var dto = new RegisterDto
            {
                Username = "testuser",
                Password = "Test@123",
                Email = "test@example.com"
            };

            var authResult = new AuthResult
            {
                Success = true,
                Token = "mock-jwt-token",
                Message = "Registration successful"
            };

            _authServiceMock.Setup(s => s.Register(dto)).ReturnsAsync(authResult);

            var result = await _controller.Register(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Register_ExceptionThrown_ReturnsBadRequest()
        {
            var dto = new RegisterDto
            {
                Username = "testuser",
                Password = "Test@123",
                Email = "test@example.com"
            };

            _authServiceMock.Setup(s => s.Register(dto)).ThrowsAsync(new System.Exception("Registration failed"));

            var result = await _controller.Register(dto);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Login_ValidDto_ReturnsOk()
        {
            var dto = new LoginDto
            {
                Username = "testuser",
                Password = "Test@123"
            };

            var authResult = new AuthResult
            {
                Success = true,
                Token = "mock-jwt-token",
                Message = "Login successful"
            };

            _authServiceMock.Setup(s => s.Login(dto)).ReturnsAsync(authResult);

            var result = await _controller.Login(dto);

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task Login_ExceptionThrown_ReturnsUnauthorized()
        {
            var dto = new LoginDto
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            _authServiceMock.Setup(s => s.Login(dto)).ThrowsAsync(new System.Exception("Login failed"));

            var result = await _controller.Login(dto);

            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }
    }

    internal class AuthResult : ResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }

    internal class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    internal class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    internal class ResponseDto
    {
        // Base response properties if any
    }

    internal interface IAuthService
    {
        Task<AuthResult> Register(RegisterDto dto);
        Task<AuthResult> Login(LoginDto dto);
    }

    internal class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var result = await _authService.Register(dto);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.Login(dto);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return Unauthorized(ex.Message);
            }
        }
    }
}
