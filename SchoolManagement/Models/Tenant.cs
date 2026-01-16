namespace SchoolManagement.Models;

public class Tenant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SchoolName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public DateTime SubscriptionStart { get; set; }
    public DateTime SubscriptionEnd { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
