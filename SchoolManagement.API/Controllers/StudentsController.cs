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
            student.User.Email,
            student.Class,
            student.Section,
            student.DateOfBirth,
            student.ParentName,
            student.ParentPhone
        ));
    }

    [HttpPost]
    public async Task<ActionResult<StudentResponse>> Create(CreateStudentRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "Email already exists" });

        var user = new User
        {
            TenantId = TenantId,
            Email = request.Email,
            FullName = request.FullName,
            Phone = request.Phone,
            Role = UserRole.Student,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var student = new Student
        {
            TenantId = TenantId,
            UserId = user.Id,
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
            user.Email,
            student.Class,
            student.Section,
            student.DateOfBirth,
            student.ParentName,
            student.ParentPhone
        ));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id && s.TenantId == TenantId);
        if (student == null) return NotFound();
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
