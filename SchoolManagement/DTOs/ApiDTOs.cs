namespace SchoolManagement.DTOs;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string UserId, string TenantId, string Role, string FullName, string SchoolName, string SchoolCode);
public record PublicAppInfoResponse(
    string AppName,
    string Tagline,
    string SupportEmail,
    string SupportPhone,
    string SupportWhatsApp,
    string HelpMessage,
    string LoginHint
);

public record SignupRequest(
    string SchoolName,
    string Email,
    string Phone,
    string Address,
    string AdminName,
    string AdminUsername,
    string Password
);

public record SignupResponse(bool Success, string Message);

public record CreateStudentRequest(
    string Username,
    string FullName,
    string Email,
    string Phone,
    string RollNumber,
    string Class,
    string Section,
    DateTime DateOfBirth,
    string ParentName,
    string ParentUsername,
    string ParentEmail,
    string ParentPhone,
    string ParentPassword,
    string Password
);

public record StudentResponse(
    string Id,
    string RollNumber,
    string FullName,
    string Username,
    string Email,
    string Class,
    string Section,
    DateTime DateOfBirth,
    string ParentName,
    string ParentPhone
);

public record SummaryMetric(string Label, string Value, string Hint);
public record ActionItem(string Title, string Description);

public record TenantSummaryResponse(
    string Id,
    string SchoolName,
    string SchoolCode,
    string ContactPersonName,
    string Email,
    string Phone,
    bool IsActive,
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
