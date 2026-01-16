namespace SchoolManagement.API.Models;

public class Attendance
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Remarks { get; set; } = string.Empty;
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    Excused
}
