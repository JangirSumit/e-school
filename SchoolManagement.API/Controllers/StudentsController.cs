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
public class StudentsController : ControllerBase
{
    private readonly SchoolDbContext _context;

    public StudentsController(SchoolDbContext context) => _context = context;

    private string TenantId => User.FindFirst("TenantId")?.Value ?? string.Empty;

    [HttpGet]
    public async Task<ActionResult<List<StudentResponse>>> GetAll()
    {
        var students = await _context.Students
            .Include(s => s.User)
            .Where(s => s.TenantId == TenantId)
            .Select(s => new StudentResponse(
                s.Id,
                s.RollNumber,
                s.User.FullName,
                s.User.Username ?? "",
                s.User.Email,
                s.Class,
                s.Section,
                s.DateOfBirth,
                s.ParentName,
                s.ParentPhone
            ))
            .ToListAsync();
        return Ok(students);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentResponse>> GetById(string id)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == TenantId);
        
        if (student == null) return NotFound();
        
        return Ok(new StudentResponse(
            student.Id,
            student.RollNumber,
            student.User.FullName,
            student.User.Username ?? "",
            student.User.Email,
            student.Class,
            student.Section,
            student.DateOfBirth,
            student.ParentName,
            student.ParentPhone
        ));
    }

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.SchoolAdmin))]
    public async Task<ActionResult<StudentResponse>> Create(CreateStudentRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.TenantId == TenantId && u.Username == request.ParentUsername))
        {
            var existingParent = await _context.Users.FirstAsync(u => u.TenantId == TenantId && u.Username == request.ParentUsername);
            if (existingParent.Role != UserRole.Parent)
                return BadRequest(new { message = "Parent username is already used by another role." });
        }

        if (await _context.Users.AnyAsync(u => u.TenantId == TenantId && u.Username == request.Username))
            return BadRequest(new { message = "Username already exists in this school" });

        var user = new User
        {
            TenantId = TenantId,
            Username = request.Username,
            Email = request.Email ?? "",
            FullName = request.FullName,
            Phone = request.Phone,
            Role = UserRole.Student,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        User? parentUser = await _context.Users
            .FirstOrDefaultAsync(u => u.TenantId == TenantId && u.Username == request.ParentUsername && u.Role == UserRole.Parent);

        if (parentUser == null)
        {
            parentUser = new User
            {
                TenantId = TenantId,
                Username = request.ParentUsername,
                Email = request.ParentEmail,
                FullName = request.ParentName,
                Phone = request.ParentPhone,
                Role = UserRole.Parent,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.ParentPassword)
            };

            _context.Users.Add(parentUser);
        }

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var schoolClass = await _context.Classes
            .FirstOrDefaultAsync(c => c.TenantId == TenantId && c.Name == request.Class && c.Section == request.Section);

        var student = new Student
        {
            TenantId = TenantId,
            UserId = user.Id,
            ParentUserId = parentUser.Id,
            ClassId = schoolClass?.Id,
            RollNumber = request.RollNumber,
            Class = request.Class,
            Section = request.Section,
            DateOfBirth = request.DateOfBirth,
            ParentName = request.ParentName,
            ParentPhone = request.ParentPhone
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = student.Id }, new StudentResponse(
            student.Id,
            student.RollNumber,
            user.FullName,
            user.Username,
            user.Email,
            student.Class,
            student.Section,
            student.DateOfBirth,
            student.ParentName,
            student.ParentPhone
        ));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = nameof(UserRole.SchoolAdmin))]
    public async Task<IActionResult> Delete(string id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id && s.TenantId == TenantId);
        if (student == null) return NotFound();
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
