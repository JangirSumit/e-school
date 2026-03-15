using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FacultiesController : ControllerBase
{
    private readonly SchoolDbContext _context;

    public FacultiesController(SchoolDbContext context) => _context = context;

    private string TenantId => User.FindFirst("TenantId")?.Value ?? string.Empty;

    [HttpGet]
    public async Task<ActionResult<List<FacultyResponse>>> GetAll()
    {
        var faculty = await _context.Faculties
            .Include(f => f.User)
            .Where(f => f.TenantId == TenantId)
            .Select(f => new FacultyResponse(
                f.Id,
                f.User.FullName,
                f.User.Username ?? string.Empty,
                f.Department,
                f.Qualification,
                f.EmployeeId,
                f.JoiningDate
            ))
            .ToListAsync();

        return Ok(faculty);
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.SchoolAdmin))]
    public async Task<ActionResult<FacultyResponse>> Create(CreateFacultyRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.TenantId == TenantId && u.Username == request.Username))
            return BadRequest(new { message = "Username already exists in this school" });

        var user = new User
        {
            TenantId = TenantId,
            Username = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            Phone = request.Phone,
            Role = UserRole.Faculty,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        var faculty = new Faculty
        {
            TenantId = TenantId,
            UserId = user.Id,
            Username = request.Username,
            EmployeeId = request.EmployeeId,
            Department = request.Department,
            Qualification = request.Qualification,
            JoiningDate = request.JoiningDate
        };

        _context.Users.Add(user);
        _context.Faculties.Add(faculty);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = faculty.Id }, new FacultyResponse(
            faculty.Id,
            user.FullName,
            user.Username ?? string.Empty,
            faculty.Department,
            faculty.Qualification,
            faculty.EmployeeId,
            faculty.JoiningDate
        ));
    }
}
