using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MarksController : ControllerBase
{
    private readonly SchoolDbContext _context;

    public MarksController(SchoolDbContext context) => _context = context;

    private string TenantId => User.FindFirst("TenantId")?.Value ?? string.Empty;

    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<Mark>>> GetByStudent(string studentId) =>
        await _context.Marks.Where(m => m.TenantId == TenantId && m.StudentId == studentId).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Mark>> Create(Mark mark)
    {
        mark.TenantId = TenantId;
        _context.Marks.Add(mark);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByStudent), new { studentId = mark.StudentId }, mark);
    }
}
