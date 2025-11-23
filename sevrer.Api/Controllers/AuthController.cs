using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using sevrer.Api.Models;     
using sevrer.Api.DTOs;       

namespace server.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _config;

    public AuthController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration config)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already registered.");

        var hashedPassword = _passwordHasher.HashPassword(null, dto.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email
        });
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null)
            return Unauthorized("Invalid email or password.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Unauthorized("Invalid email or password.");

        var token = GenerateJwtToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email
        });
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryMinutes"]!)),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}