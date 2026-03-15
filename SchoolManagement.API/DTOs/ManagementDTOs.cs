namespace SchoolManagement.API.DTOs;

public record SummaryMetric(string Label, string Value, string Hint);

public record ActionItem(string Title, string Description);

public record TenantSummaryResponse(
    string Id,
    string SchoolName,
    string SchoolCode,
    string ContactPersonName,
    string Email,
    string Phone,
    string Address,
    bool IsActive,
    string SubscriptionPlan,
    string BillingCycle,
    decimal BillingAmount,
    DateTime SubscriptionStart,
    DateTime SubscriptionEnd,
    DateTime? LastBillingDate,
    DateTime? NextBillingDate,
    int LicenseSeats,
    string LicensedModules,
    string BillingNotes,
    DateTime CreatedAt,
    int StudentCount,
    int FacultyCount,
    int ClassCount
);

public record ClassResponse(
    string Id,
    string Name,
    string Section,
    int Capacity,
    int StudentCount,
    string ClassTeacherName
);

public record FacultyResponse(
    string Id,
    string FullName,
    string Username,
    string Department,
    string Qualification,
    string EmployeeId,
    DateTime JoiningDate
);

public record ParentChildResponse(
    string StudentId,
    string StudentName,
    string RollNumber,
    string ClassName,
    string Section,
    decimal AttendancePercentage,
    decimal AverageMarks
);

public record DashboardResponse(
    string Role,
    string UserName,
    string SchoolName,
    string SchoolCode,
    List<SummaryMetric> Metrics,
    List<ActionItem> Highlights,
    List<TenantSummaryResponse> Schools,
    List<ClassResponse> Classes,
    List<FacultyResponse> FacultyMembers,
    List<StudentResponse> Students,
    List<ParentChildResponse> Children
);

public record CreateSchoolRequest(
    string SchoolName,
    string Email,
    string Phone,
    string Address,
    string ContactPersonName,
    string AdminName,
    string AdminUsername,
    string AdminPassword
);

public record UpdateSchoolStatusRequest(bool IsActive);

public record UpdateSchoolManagementRequest(
    string ContactPersonName,
    string Email,
    string Phone,
    string Address,
    bool IsActive,
    string SubscriptionPlan,
    string BillingCycle,
    decimal BillingAmount,
    DateTime SubscriptionStart,
    DateTime SubscriptionEnd,
    DateTime? LastBillingDate,
    DateTime? NextBillingDate,
    int LicenseSeats,
    string LicensedModules,
    string BillingNotes
);

public record CreateClassRequest(
    string Name,
    string Section,
    int Capacity,
    string? ClassTeacherUserId
);

public record CreateFacultyRequest(
    string FullName,
    string Username,
    string Email,
    string Phone,
    string EmployeeId,
    string Department,
    string Qualification,
    DateTime JoiningDate,
    string Password
);
