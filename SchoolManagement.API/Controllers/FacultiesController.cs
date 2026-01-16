using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
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
    public async Task<ActionResult<List<Faculty>>> GetAll() =>
        await _context.Faculties.Where(f => f.TenantId == TenantId).ToListAsync();

    [HttpPost]
    public async Task<ActionResult<Faculty>> Create(Faculty faculty)
    {
        faculty.TenantId = TenantId;
        _context.Faculties.Add(faculty);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = faculty.Id }, faculty);
    }
}
