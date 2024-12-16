using Microsoft.AspNetCore.Mvc;
using SharedService;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase {
    private readonly SharedDbContext _dbContext;
    public AuthController(SharedDbContext dbContext) {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    [HttpPost("Register")]
    public IActionResult Register([FromBody] RegisterRequest request) {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required.");

        if (_dbContext.Users.Any(u => u.Username == request.Username))
            return Conflict("Username already exists.");

        // Hash the password (simplified for demo purposes)
        var hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password));

        var user = new User {
            Username = request.Username,
            Password = hashedPassword,    
            Role = "User"
        };

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return Ok(new { Message = "User registered successfully." });
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginRequest request) {
        var user = _dbContext.Users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null) {
            return Unauthorized("Invalid username or password.");
        }

        var hashedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password));
        if (user.Password != hashedPassword) {
            return Unauthorized("Invalid username or password.");
        }

        // Generate JWT token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("LcWVcTb9JdFaZ43oRDk1GZR19gApAyi3cIH8wDWsF/M="); // REMOVE DEFAULT SECRET KEY IN PRODUCTION
        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new System.Security.Claims.ClaimsIdentity(new [] {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            // Change it to conform: Issuer and Audience should be configurable
            Issuer = "https://localhost:5001",
            Audience = "ShortTermStayAPI",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });
    }

    [HttpPost("ValidateToken")]
    public IActionResult ValidateToken([FromBody] TokenValidationRequest request) {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("LcWVcTb9JdFaZ43oRDk1GZR19gApAyi3cIH8wDWsF/M="); // REMOVE DEFAULT SECRET KEY IN PRODUCTION
        try {
            tokenHandler.ValidateToken(request.Token, new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                // Change it to conform: Issuer and Audience should be configurable
                ValidIssuer = "https://localhost:5001",
                ValidAudience = "ShortTermStayAPI",
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);
        } catch {
            return Unauthorized(new { IsValid = false });
        }

        return Ok(new { IsValid = true });
    }
}

public class RegisterRequest {
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class TokenValidationRequest
    {
        public required string Token { get; set; }
    }