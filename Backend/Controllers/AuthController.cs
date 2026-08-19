using Microsoft.AspNetCore.Mvc;
 
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
 
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
 
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var result = await _authService.Register(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed");
                return BadRequest(new { message = ex.Message });
            }
        }
 
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var result = await _authService.Login(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return Unauthorized(new { message = ex.Message });
            }
        }
    }