using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly SchoolDbContext _context;

    public AttendanceController(SchoolDbContext context) => _context = context;

    private string TenantId => User.FindFirst("TenantId")?.Value ?? string.Empty;

    [HttpGet]
    public async Task<ActionResult<List<Attendance>>> GetByDate([FromQuery] DateTime date) =>
        await _context.Attendances.Where(a => a.TenantId == TenantId && a.Date.Date == date.Date).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Attendance>> Create(Attendance attendance)
    {
        attendance.TenantId = TenantId;
        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByDate), new { date = attendance.Date }, attendance);
    }
}
