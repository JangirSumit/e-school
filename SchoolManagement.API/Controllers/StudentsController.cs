using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
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
    public async Task<ActionResult<List<Student>>> GetAll() =>
        await _context.Students.Where(s => s.TenantId == TenantId).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetById(string id)
    {
        var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id && s.TenantId == TenantId);
        return student == null ? NotFound() : Ok(student);
    }

    [HttpPost]
    public async Task<ActionResult<Student>> Create(Student student)
    {
        student.TenantId = TenantId;
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, Student student)
    {
        if (id != student.Id) return BadRequest();
        student.TenantId = TenantId;
        _context.Entry(student).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
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
