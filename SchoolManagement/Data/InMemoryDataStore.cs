using SchoolManagement.Models;

namespace SchoolManagement.Data;

public class InMemoryDataStore : IDataStore
{
    private readonly List<Tenant> _tenants = new();
    private readonly List<User> _users = new();
    private readonly List<Student> _students = new();
    private readonly List<Faculty> _faculties = new();
    private readonly List<Attendance> _attendances = new();
    private readonly List<Mark> _marks = new();
    private readonly List<Holiday> _holidays = new();

    public Task<User> GetUserByEmailAsync(string email) =>
        Task.FromResult(_users.FirstOrDefault(u => u.Email == email));

    public Task AddTenantAsync(Tenant tenant)
    {
        _tenants.Add(tenant);
        return Task.CompletedTask;
    }

    public Task AddUserAsync(User user)
    {
        _users.Add(user);
        return Task.CompletedTask;
    }

    public Task<List<Student>> GetStudentsAsync(string tenantId) =>
        Task.FromResult(_students.Where(s => s.TenantId == tenantId).ToList());

    public Task<List<Faculty>> GetFacultiesAsync(string tenantId) =>
        Task.FromResult(_faculties.Where(f => f.TenantId == tenantId).ToList());

    public Task<List<Attendance>> GetAttendanceAsync(string tenantId, DateTime date) =>
        Task.FromResult(_attendances.Where(a => a.TenantId == tenantId && a.Date.Date == date.Date).ToList());

    public Task<List<Mark>> GetMarksAsync(string tenantId, string studentId) =>
        Task.FromResult(_marks.Where(m => m.TenantId == tenantId && m.StudentId == studentId).ToList());

    public Task<List<Holiday>> GetHolidaysAsync(string tenantId) =>
        Task.FromResult(_holidays.Where(h => h.TenantId == tenantId).ToList());

    public Task AddStudentAsync(Student student)
    {
        _students.Add(student);
        return Task.CompletedTask;
    }

    public Task AddFacultyAsync(Faculty faculty)
    {
        _faculties.Add(faculty);
        return Task.CompletedTask;
    }

    public Task AddAttendanceAsync(Attendance attendance)
    {
        _attendances.Add(attendance);
        return Task.CompletedTask;
    }

    public Task AddMarkAsync(Mark mark)
    {
        _marks.Add(mark);
        return Task.CompletedTask;
    }

    public Task AddHolidayAsync(Holiday holiday)
    {
        _holidays.Add(holiday);
        return Task.CompletedTask;
    }
}
