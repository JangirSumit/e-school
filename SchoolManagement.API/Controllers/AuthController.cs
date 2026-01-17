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
        User? user = null;
        
        // Check if login is schoolcode@username format
        if (request.Email.Contains('@') && !request.Email.Contains('.'))
        {
            var parts = request.Email.Split('@');
            if (parts.Length == 2)
            {
                var schoolCode = parts[0].ToUpper();
                var username = parts[1];
                
                var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.SchoolCode == schoolCode);
                if (tenant != null)
                {
                    user = await _context.Users.FirstOrDefaultAsync(u => 
                        u.TenantId == tenant.Id && u.Username == username);
                }
            }
        }
        else
        {
            // Admin login with email
            user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == UserRole.Admin);
        }
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        var token = GenerateJwtToken(user);
        return Ok(new LoginResponse(token, user.Id, user.TenantId, user.Role.ToString(), user.FullName));
    }

    [HttpPost("signup")]
    public async Task<ActionResult<SignupResponse>> Signup(SignupRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new SignupResponse(false, "Email already exists"));

        // Generate school code from school name (first 3-4 letters, uppercase)
        var schoolCode = GenerateSchoolCode(request.SchoolName);
        
        // Ensure uniqueness
        var existingCode = await _context.Tenants.FirstOrDefaultAsync(t => t.SchoolCode == schoolCode);
        if (existingCode != null)
        {
            schoolCode = schoolCode + new Random().Next(10, 99);
        }

        var tenant = new Tenant
        {
            SchoolCode = schoolCode,
            SchoolName = request.SchoolName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true,
            SubscriptionStart = DateTime.UtcNow,
            SubscriptionEnd = DateTime.UtcNow.AddYears(1)
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync();

        var user = new User
        {
            TenantId = tenant.Id,
            Email = request.Email,
            FullName = request.AdminName,
            Phone = request.Phone,
            Role = UserRole.Admin,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new SignupResponse(true, $"School registered successfully. School Code: {schoolCode}"));
    }

    private string GenerateSchoolCode(string schoolName)
    {
        var words = schoolName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (words.Length >= 2)
        {
            return (words[0].Substring(0, Math.Min(2, words[0].Length)) + 
                   words[1].Substring(0, Math.Min(2, words[1].Length))).ToUpper();
        }
        return schoolName.Substring(0, Math.Min(4, schoolName.Length)).ToUpper();
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
}
