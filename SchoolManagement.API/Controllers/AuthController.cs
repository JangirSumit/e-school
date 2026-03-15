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
        Tenant? tenant = null;
        
        // Check if login is schoolcode@username format
        if (request.Email.Contains('@') && !request.Email.Contains('.'))
        {
            var parts = request.Email.Split('@');
            if (parts.Length == 2)
            {
                var schoolCode = parts[0].ToUpper();
                var username = parts[1];
                
                tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.SchoolCode == schoolCode);
                if (tenant != null)
                {
                    user = await _context.Users.FirstOrDefaultAsync(u => 
                        u.TenantId == tenant.Id && u.Username == username);
                }
            }
        }
        else
        {
            user = await _context.Users
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == UserRole.Owner);

            tenant = user?.Tenant;
        }
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials" });

        if (user.TenantId != null)
        {
            tenant ??= await _context.Tenants.FirstOrDefaultAsync(t => t.Id == user.TenantId);
            if (tenant is null || !tenant.IsActive)
                return Unauthorized(new { message = "This school is inactive. Please contact the app owner." });
        }

        var token = GenerateJwtToken(user);
        Response.Cookies.Append("eschool_auth", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        return Ok(new LoginResponse(
            token,
            user.Id,
            user.TenantId ?? string.Empty,
            user.Role.ToString(),
            user.FullName,
            user.Role == UserRole.Owner ? "Platform Owner" : tenant?.SchoolName ?? string.Empty,
            user.Role == UserRole.Owner ? string.Empty : tenant?.SchoolCode ?? string.Empty));
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("eschool_auth", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax
        });

        return NoContent();
    }

    [HttpPost("signup")]
    public ActionResult<SignupResponse> Signup(SignupRequest request)
    {
        return StatusCode(StatusCodes.Status403Forbidden,
            new SignupResponse(false, "School onboarding is handled only by the app owner after pricing and billing discussion. Please contact support."));
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "YourSecretKeyHere123456789012345678901234567890"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("TenantId", user.TenantId ?? string.Empty),
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
