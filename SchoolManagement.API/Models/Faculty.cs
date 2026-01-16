namespace SchoolManagement.API.Models;

public class Faculty
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string EmployeeId { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
}
