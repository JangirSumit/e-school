namespace SchoolManagement.Models;

public class Student
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; }
    public string UserId { get; set; }
    public string RollNumber { get; set; }
    public string Class { get; set; }
    public string Section { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string ParentName { get; set; }
    public string ParentPhone { get; set; }
}
