using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
 
        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _logger = logger;
        }
 
        public async Task<ResponseDto> Register(RegisterDto dto)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                    throw new Exception("User already exists");
 
                if(dto.Role.ToUpper()=="ADMIN"){
                    throw new Exception("You are not allowed to register as Admin");
                }
 
                var user = new ApplicationUser
                {
                    FullName = dto.FullName,
                    Email = dto.Email,
                    UserName = dto.Email,
                    PhoneNumber = dto.Phone
                };
 
                var result = await _userManager.CreateAsync(user, dto.Password);
 
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    _logger.LogError("User registration failed: {Errors}", errors);
                    throw new Exception("User creation failed: " + errors);
                }
 
                await _userManager.AddToRoleAsync(user, dto.Role.ToUpper());
 
                return new ResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = dto.Role.ToUpper()
                };
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                throw;
            }
        }
 
        public async Task<ResponseDto> Login(LoginDto dto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null || !user.IsActive)
                    throw new Exception("Invalid credentials or inactive user");
                    
                var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
                if (!result.Succeeded)
                    throw new Exception("Invalid email or password");
 
                var roles = await _userManager.GetRolesAsync(user);
 
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
 
                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }
 
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
 
                var token = new JwtSecurityToken(
                    issuer: _config["JWT:ValidIssuer"],
                    audience: _config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddDays(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
 
                return new ResponseDto{
                    Id=user.Id,
                    FullName=user.FullName,
                    Email=user.Email,
                    Role=roles.FirstOrDefault(),
                    Token=new JwtSecurityTokenHandler().WriteToken(token)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}", dto.Email);
                throw;
            }
        }
    }