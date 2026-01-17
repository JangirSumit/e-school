namespace SchoolManagement.API.DTOs;

public record CreateStudentRequest(
    string Username, // Required: unique username for login
    string FullName,
    string? Email, // Optional for students
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
    string Username,
    string? Email,
    string Class,
    string Section,
    DateTime DateOfBirth,
    string ParentName,
    string ParentPhone
);
