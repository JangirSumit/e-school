namespace SchoolManagement.API.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string? Username { get; set; } // For staff/students: unique per tenant
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Tenant Tenant { get; set; } = null!;
}

public enum UserRole
{
    Admin,
    Faculty,
    Student
}
