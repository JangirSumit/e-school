namespace SchoolManagement.API.Models;

public class Mark
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string StudentId { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public decimal ObtainedMarks { get; set; }
    public decimal TotalMarks { get; set; }
    public DateTime ExamDate { get; set; }
}
