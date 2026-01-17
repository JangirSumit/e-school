namespace SchoolManagement.DTOs;

public record LoginRequest(string Email, string Password);
public record LoginResponse(string Token, string UserId, string TenantId, string Role, string FullName);

public record SignupRequest(
    string SchoolName,
    string Email,
    string Phone,
    string Address,
    string AdminName,
    string Password
);

public record SignupResponse(bool Success, string Message);

public record CreateStudentRequest(
    string FullName,
    string Email,
    string Phone,
    string RollNumber,
    string Class,
    string Section,
    DateTime DateOfBirth,
    string ParentName,
    string ParentPhone,
    string Password
);

public record StudentResponse(
    string Id,
    string RollNumber,
    string FullName,
    string Email,
    string Class,
    string Section,
    DateTime DateOfBirth,
    string ParentName,
    string ParentPhone
);
