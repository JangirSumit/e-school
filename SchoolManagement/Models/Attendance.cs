namespace SchoolManagement.Models;

public class Attendance
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; }
    public string StudentId { get; set; }
    public DateTime Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Remarks { get; set; }
}

public enum AttendanceStatus
{
    Present,
    Absent,
    Late,
    Excused
}
