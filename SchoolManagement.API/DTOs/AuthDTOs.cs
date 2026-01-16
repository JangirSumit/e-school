namespace SchoolManagement.API.DTOs;

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
