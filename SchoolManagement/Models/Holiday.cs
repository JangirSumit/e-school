namespace SchoolManagement.Models;

public class Holiday
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
}
