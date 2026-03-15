namespace SchoolManagement.API.Models;

public class Student
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? ParentUserId { get; set; }
    public string? ClassId { get; set; }
    public string RollNumber { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public string ParentPhone { get; set; } = string.Empty;
    
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
    public User? ParentUser { get; set; }
    public SchoolClass? SchoolClass { get; set; }
}
