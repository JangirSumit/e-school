namespace SchoolManagement.API.Models;

public class SchoolClass
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? ClassTeacherUserId { get; set; }
    public int Capacity { get; set; }

    public Tenant Tenant { get; set; } = null!;
    public User? ClassTeacherUser { get; set; }
    public ICollection<Student> Students { get; set; } = new List<Student>();
}
