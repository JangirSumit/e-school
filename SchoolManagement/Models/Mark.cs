namespace SchoolManagement.Models;

public class Mark
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; }
    public string StudentId { get; set; }
    public string Subject { get; set; }
    public string ExamType { get; set; }
    public decimal ObtainedMarks { get; set; }
    public decimal TotalMarks { get; set; }
    public DateTime ExamDate { get; set; }
}
