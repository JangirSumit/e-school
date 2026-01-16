using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SchoolDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(SchoolDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || user.PasswordHash != HashPassword(request.Password))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = GenerateJwtToken(user);
        return Ok(new LoginResponse(token, user.Id, user.TenantId, user.Role.ToString(), user.FullName));
    }

    [HttpPost("signup")]
    public async Task<ActionResult<SignupResponse>> Signup(SignupRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new SignupResponse(false, "Email already exists"));

        var tenant = new Tenant
        {
            SchoolName = request.SchoolName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true,
            SubscriptionStart = DateTime.UtcNow,
            SubscriptionEnd = DateTime.UtcNow.AddYears(1)
        };

        var user = new User
        {
            TenantId = tenant.Id,
            Email = request.Email,
            FullName = request.AdminName,
            Phone = request.Phone,
            Role = UserRole.Admin,
            PasswordHash = HashPassword(request.Password)
        };

        _context.Tenants.Add(tenant);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new SignupResponse(true, "School registered successfully"));
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "YourSecretKeyHere123456789012345678901234567890"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("TenantId", user.TenantId),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "SchoolManagementAPI",
            audience: _config["Jwt:Audience"] ?? "SchoolManagementApp",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
}
