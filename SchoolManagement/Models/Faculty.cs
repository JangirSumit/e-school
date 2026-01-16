namespace SchoolManagement.Models;

public class Faculty
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; }
    public string UserId { get; set; }
    public string EmployeeId { get; set; }
    public string Department { get; set; }
    public string Qualification { get; set; }
    public DateTime JoiningDate { get; set; }
}
