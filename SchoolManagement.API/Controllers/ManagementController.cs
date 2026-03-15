using System.Security.Claims;
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
public class ManagementController : ControllerBase
{
    private readonly SchoolDbContext _context;

    public ManagementController(SchoolDbContext context)
    {
        _context = context;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    private string CurrentTenantId => User.FindFirst("TenantId")?.Value ?? string.Empty;
    private string CurrentRole => User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardResponse>> GetDashboard()
    {
        var user = await _context.Users.Include(u => u.Tenant).FirstAsync(u => u.Id == CurrentUserId);

        if (user.Role == UserRole.Owner)
        {
            var schools = await BuildTenantSummariesAsync();
            var metrics = new List<SummaryMetric>
            {
                new("Schools", schools.Count.ToString(), "Active and inactive tenants"),
                new("Active", schools.Count(s => s.IsActive).ToString(), "Currently enabled schools"),
                new("Students", await _context.Students.CountAsync() + string.Empty, "Across all onboarded schools"),
                new("Faculty", await _context.Faculties.CountAsync() + string.Empty, "Teaching and operations teams")
            };

            return Ok(new DashboardResponse(
                user.Role.ToString(),
                user.FullName,
                "Platform Owner",
                string.Empty,
                metrics,
                new List<ActionItem>
                {
                    new("Onboard schools", "Create a tenant, admin account, and school code from one place."),
                    new("Control access", "Pause a school instantly by toggling active status.")
                },
                schools,
                new(),
                new(),
                new(),
                new()));
        }

        var schoolName = user.Tenant?.SchoolName ?? string.Empty;
        var schoolCode = user.Tenant?.SchoolCode ?? string.Empty;

        if (user.Role == UserRole.SchoolAdmin)
        {
            var classes = await BuildClassesAsync(CurrentTenantId);
            var faculty = await BuildFacultyAsync(CurrentTenantId);
            var students = await BuildStudentsAsync(CurrentTenantId);

            return Ok(new DashboardResponse(
                user.Role.ToString(),
                user.FullName,
                schoolName,
                schoolCode,
                new List<SummaryMetric>
                {
                    new("Classes", classes.Count.ToString(), "Grades and sections running this term"),
                    new("Students", students.Count.ToString(), "Children enrolled in this school"),
                    new("Faculty", faculty.Count.ToString(), "Teaching and staff accounts"),
                    new("Parents", await _context.Users.CountAsync(u => u.TenantId == CurrentTenantId && u.Role == UserRole.Parent) + string.Empty, "Parent logins linked to children")
                },
                new List<ActionItem>
                {
                    new("Create classes", "Set up grades, sections, capacity, and class teachers."),
                    new("Admit students", "Add children, parent accounts, and student logins together."),
                    new("Add faculty", "Create faculty credentials for teachers and staff.")
                },
                new(),
                classes,
                faculty,
                students,
                new()));
        }

        if (user.Role == UserRole.Faculty)
        {
            var classes = await BuildClassesAsync(CurrentTenantId, CurrentUserId);
            var students = await BuildStudentsAsync(CurrentTenantId);

            return Ok(new DashboardResponse(
                user.Role.ToString(),
                user.FullName,
                schoolName,
                schoolCode,
                new List<SummaryMetric>
                {
                    new("Assigned Classes", classes.Count.ToString(), "Sections where you are class teacher"),
                    new("Students", students.Count.ToString(), "Visible student directory"),
                    new("Attendance Entries", await _context.Attendances.CountAsync(a => a.TenantId == CurrentTenantId) + string.Empty, "School-wide attendance captured so far")
                },
                new List<ActionItem>
                {
                    new("Track classroom", "Review classes, students, marks, and attendance trends."),
                    new("Coordinate with admin", "Use the directory to stay aligned on sections and students.")
                },
                new(),
                classes,
                new(),
                students,
                new()));
        }

        if (user.Role == UserRole.Parent)
        {
            var children = await BuildParentChildrenAsync(CurrentTenantId, CurrentUserId);

            return Ok(new DashboardResponse(
                user.Role.ToString(),
                user.FullName,
                schoolName,
                schoolCode,
                new List<SummaryMetric>
                {
                    new("Children", children.Count.ToString(), "Linked student profiles"),
                    new("Avg Attendance", FormatAverage(children.Select(c => c.AttendancePercentage)), "Across linked children"),
                    new("Avg Marks", FormatAverage(children.Select(c => c.AverageMarks)), "Across recorded exams")
                },
                new List<ActionItem>
                {
                    new("Stay informed", "View your children's class, attendance, and marks summary."),
                    new("Single parent login", "One parent account can be linked to multiple children.")
                },
                new(),
                new(),
                new(),
                new(),
                children));
        }

        var studentRecords = await BuildStudentsAsync(CurrentTenantId);
        return Ok(new DashboardResponse(
            user.Role.ToString(),
            user.FullName,
            schoolName,
            schoolCode,
            new(),
            new(),
            new(),
            new(),
            new(),
            studentRecords.Where(s => s.Id == CurrentUserId).ToList(),
            new()));
    }

    [Authorize(Roles = nameof(UserRole.Owner))]
    [HttpGet("schools")]
    public async Task<ActionResult<List<TenantSummaryResponse>>> GetSchools()
    {
        return Ok(await BuildTenantSummariesAsync());
    }

    [Authorize(Roles = nameof(UserRole.Owner))]
    [HttpPost("schools")]
    public async Task<ActionResult<TenantSummaryResponse>> CreateSchool(CreateSchoolRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest(new { message = "Email already exists." });

        if (await _context.Users.AnyAsync(u => u.TenantId != null && u.Username == request.AdminUsername))
            return BadRequest(new { message = "Admin username already exists in another school." });

        var schoolCode = await GenerateUniqueSchoolCodeAsync(request.SchoolName);
        var tenant = new Tenant
        {
            SchoolCode = schoolCode,
            SchoolName = request.SchoolName,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            ContactPersonName = request.ContactPersonName,
            SubscriptionPlan = "Standard",
            BillingCycle = "Yearly",
            BillingAmount = 25000,
            LicenseSeats = 100,
            LicensedModules = "Admissions,Academics,Attendance,Parents,Reports",
            IsActive = true,
            SubscriptionStart = DateTime.UtcNow,
            SubscriptionEnd = DateTime.UtcNow.AddYears(1),
            NextBillingDate = DateTime.UtcNow.AddYears(1)
        };

        var admin = new User
        {
            Tenant = tenant,
            Username = request.AdminUsername.Trim().ToLowerInvariant(),
            Email = request.Email,
            FullName = request.AdminName,
            Phone = request.Phone,
            Role = UserRole.SchoolAdmin,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.AdminPassword)
        };

        _context.Tenants.Add(tenant);
        _context.Users.Add(admin);
        await _context.SaveChangesAsync();

        return Ok(new TenantSummaryResponse(
            tenant.Id,
            tenant.SchoolName,
            tenant.SchoolCode,
            tenant.ContactPersonName,
            tenant.Email,
            tenant.Phone,
            tenant.Address,
            tenant.IsActive,
            tenant.SubscriptionPlan,
            tenant.BillingCycle,
            tenant.BillingAmount,
            tenant.SubscriptionStart,
            tenant.SubscriptionEnd,
            tenant.LastBillingDate,
            tenant.NextBillingDate,
            tenant.LicenseSeats,
            tenant.LicensedModules,
            tenant.BillingNotes,
            tenant.CreatedAt,
            0,
            0,
            0));
    }

    [Authorize(Roles = nameof(UserRole.Owner))]
    [HttpPut("schools/{id}/status")]
    public async Task<IActionResult> UpdateSchoolStatus(string id, UpdateSchoolStatusRequest request)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id);
        if (tenant == null)
            return NotFound();

        tenant.IsActive = request.IsActive;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = nameof(UserRole.Owner))]
    [HttpPut("schools/{id}")]
    public async Task<ActionResult<TenantSummaryResponse>> UpdateSchool(string id, UpdateSchoolManagementRequest request)
    {
        var tenant = await _context.Tenants
            .Include(t => t.Students)
            .Include(t => t.Faculties)
            .Include(t => t.Classes)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (tenant == null)
            return NotFound();

        tenant.ContactPersonName = request.ContactPersonName.Trim();
        tenant.Email = request.Email.Trim();
        tenant.Phone = request.Phone.Trim();
        tenant.Address = request.Address.Trim();
        tenant.IsActive = request.IsActive;
        tenant.SubscriptionPlan = request.SubscriptionPlan.Trim();
        tenant.BillingCycle = request.BillingCycle.Trim();
        tenant.BillingAmount = request.BillingAmount;
        tenant.SubscriptionStart = request.SubscriptionStart;
        tenant.SubscriptionEnd = request.SubscriptionEnd;
        tenant.LastBillingDate = request.LastBillingDate;
        tenant.NextBillingDate = request.NextBillingDate;
        tenant.LicenseSeats = request.LicenseSeats;
        tenant.LicensedModules = request.LicensedModules.Trim();
        tenant.BillingNotes = request.BillingNotes.Trim();

        await _context.SaveChangesAsync();

        return Ok(MapTenantSummary(tenant));
    }

    [Authorize(Roles = $"{nameof(UserRole.SchoolAdmin)},{nameof(UserRole.Faculty)}")]
    [HttpGet("classes")]
    public async Task<ActionResult<List<ClassResponse>>> GetClasses()
    {
        var teacherFilter = CurrentRole == nameof(UserRole.Faculty) ? CurrentUserId : null;
        return Ok(await BuildClassesAsync(CurrentTenantId, teacherFilter));
    }

    [Authorize(Roles = nameof(UserRole.SchoolAdmin))]
    [HttpPost("classes")]
    public async Task<ActionResult<ClassResponse>> CreateClass(CreateClassRequest request)
    {
        if (await _context.Classes.AnyAsync(c => c.TenantId == CurrentTenantId && c.Name == request.Name && c.Section == request.Section))
            return BadRequest(new { message = "This class and section already exists." });

        var schoolClass = new SchoolClass
        {
            TenantId = CurrentTenantId,
            Name = request.Name,
            Section = request.Section,
            Capacity = request.Capacity,
            ClassTeacherUserId = request.ClassTeacherUserId
        };

        _context.Classes.Add(schoolClass);
        await _context.SaveChangesAsync();

        var teacherName = string.Empty;
        if (!string.IsNullOrWhiteSpace(schoolClass.ClassTeacherUserId))
        {
            teacherName = await _context.Users
                .Where(u => u.Id == schoolClass.ClassTeacherUserId)
                .Select(u => u.FullName)
                .FirstOrDefaultAsync() ?? string.Empty;
        }

        return Ok(new ClassResponse(schoolClass.Id, schoolClass.Name, schoolClass.Section, schoolClass.Capacity, 0, teacherName));
    }

    [Authorize(Roles = $"{nameof(UserRole.SchoolAdmin)},{nameof(UserRole.Faculty)}")]
    [HttpGet("faculty")]
    public async Task<ActionResult<List<FacultyResponse>>> GetFaculty()
    {
        return Ok(await BuildFacultyAsync(CurrentTenantId));
    }

    private async Task<List<TenantSummaryResponse>> BuildTenantSummariesAsync()
    {
        var tenants = await _context.Tenants
            .OrderBy(t => t.SchoolName)
            .ToListAsync();

        return tenants.Select(MapTenantSummary).ToList();
    }

    private static TenantSummaryResponse MapTenantSummary(Tenant t) =>
        new(
            t.Id,
            t.SchoolName,
            t.SchoolCode,
            t.ContactPersonName,
            t.Email,
            t.Phone,
            t.Address,
            t.IsActive,
            t.SubscriptionPlan,
            t.BillingCycle,
            t.BillingAmount,
            t.SubscriptionStart,
            t.SubscriptionEnd,
            t.LastBillingDate,
            t.NextBillingDate,
            t.LicenseSeats,
            t.LicensedModules,
            t.BillingNotes,
            t.CreatedAt,
            t.Students.Count,
            t.Faculties.Count,
            t.Classes.Count
        );

    private async Task<List<ClassResponse>> BuildClassesAsync(string tenantId, string? classTeacherUserId = null)
    {
        var query = _context.Classes
            .Include(c => c.ClassTeacherUser)
            .Include(c => c.Students)
            .Where(c => c.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(classTeacherUserId))
            query = query.Where(c => c.ClassTeacherUserId == classTeacherUserId);

        return await query
            .OrderBy(c => c.Name)
            .ThenBy(c => c.Section)
            .Select(c => new ClassResponse(
                c.Id,
                c.Name,
                c.Section,
                c.Capacity,
                c.Students.Count,
                c.ClassTeacherUser != null ? c.ClassTeacherUser.FullName : string.Empty
            ))
            .ToListAsync();
    }

    private async Task<List<FacultyResponse>> BuildFacultyAsync(string tenantId)
    {
        return await _context.Faculties
            .Include(f => f.User)
            .Where(f => f.TenantId == tenantId)
            .OrderBy(f => f.User.FullName)
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
    }

    private async Task<List<StudentResponse>> BuildStudentsAsync(string tenantId)
    {
        return await _context.Students
            .Include(s => s.User)
            .Where(s => s.TenantId == tenantId)
            .OrderBy(s => s.Class)
            .ThenBy(s => s.Section)
            .ThenBy(s => s.RollNumber)
            .Select(s => new StudentResponse(
                s.Id,
                s.RollNumber,
                s.User.FullName,
                s.User.Username ?? string.Empty,
                s.User.Email,
                s.Class,
                s.Section,
                s.DateOfBirth,
                s.ParentName,
                s.ParentPhone
            ))
            .ToListAsync();
    }

    private async Task<List<ParentChildResponse>> BuildParentChildrenAsync(string tenantId, string parentUserId)
    {
        var students = await _context.Students
            .Include(s => s.User)
            .Where(s => s.TenantId == tenantId && s.ParentUserId == parentUserId)
            .ToListAsync();

        var studentIds = students.Select(s => s.Id).ToList();
        var attendances = await _context.Attendances.Where(a => a.TenantId == tenantId && studentIds.Contains(a.StudentId)).ToListAsync();
        var marks = await _context.Marks.Where(m => m.TenantId == tenantId && studentIds.Contains(m.StudentId)).ToListAsync();

        return students.Select(student =>
        {
            var attendanceEntries = attendances.Where(a => a.StudentId == student.Id).ToList();
            var markEntries = marks.Where(m => m.StudentId == student.Id).ToList();
            var attendancePercentage = attendanceEntries.Count == 0
                ? 0
                : Math.Round((decimal)attendanceEntries.Count(a => a.Status == AttendanceStatus.Present) * 100 / attendanceEntries.Count, 1);
            var averageMarks = markEntries.Count == 0
                ? 0
                : Math.Round(markEntries.Average(m => m.TotalMarks == 0 ? 0 : m.ObtainedMarks * 100 / m.TotalMarks), 1);

            return new ParentChildResponse(
                student.Id,
                student.User.FullName,
                student.RollNumber,
                student.Class,
                student.Section,
                attendancePercentage,
                averageMarks);
        }).ToList();
    }

    private async Task<string> GenerateUniqueSchoolCodeAsync(string schoolName)
    {
        var parts = schoolName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var baseCode = parts.Length >= 2
            ? (parts[0][..Math.Min(2, parts[0].Length)] + parts[1][..Math.Min(2, parts[1].Length)]).ToUpperInvariant()
            : schoolName[..Math.Min(4, schoolName.Length)].ToUpperInvariant();

        var code = baseCode;
        var suffix = 1;

        while (await _context.Tenants.AnyAsync(t => t.SchoolCode == code))
        {
            code = $"{baseCode}{suffix:00}";
            suffix++;
        }

        return code;
    }

    private static string FormatAverage(IEnumerable<decimal> values)
    {
        var list = values.ToList();
        return list.Count == 0 ? "0%" : $"{Math.Round(list.Average(), 1)}%";
    }
}
