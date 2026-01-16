using SchoolManagement.Models;

namespace SchoolManagement.Data;

public interface IDataStore
{
    Task<User> GetUserByEmailAsync(string email);
    Task AddTenantAsync(Tenant tenant);
    Task AddUserAsync(User user);
    Task<List<Student>> GetStudentsAsync(string tenantId);
    Task<List<Faculty>> GetFacultiesAsync(string tenantId);
    Task<List<Attendance>> GetAttendanceAsync(string tenantId, DateTime date);
    Task<List<Mark>> GetMarksAsync(string tenantId, string studentId);
    Task<List<Holiday>> GetHolidaysAsync(string tenantId);
    Task AddStudentAsync(Student student);
    Task AddFacultyAsync(Faculty faculty);
    Task AddAttendanceAsync(Attendance attendance);
    Task AddMarkAsync(Mark mark);
    Task AddHolidayAsync(Holiday holiday);
}
