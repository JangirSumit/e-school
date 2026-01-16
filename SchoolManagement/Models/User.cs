namespace SchoolManagement.Models;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum UserRole
{
    Admin,
    Faculty,
    Student
}
