using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HolidaysController : ControllerBase
{
    private readonly SchoolDbContext _context;

    public HolidaysController(SchoolDbContext context) => _context = context;

    private string TenantId => User.FindFirst("TenantId")?.Value ?? string.Empty;

    [HttpGet]
    public async Task<ActionResult<List<Holiday>>> GetAll() =>
        await _context.Holidays.Where(h => h.TenantId == TenantId).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Holiday>> Create(Holiday holiday)
    {
        holiday.TenantId = TenantId;
        _context.Holidays.Add(holiday);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = holiday.Id }, holiday);
    }
}
