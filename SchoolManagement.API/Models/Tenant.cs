namespace SchoolManagement.API.Models;

public class Tenant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SchoolCode { get; set; } = string.Empty; // Unique 3-6 char code
    public string SchoolName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime SubscriptionStart { get; set; }
    public DateTime SubscriptionEnd { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Faculty> Faculties { get; set; } = new List<Faculty>();
}
