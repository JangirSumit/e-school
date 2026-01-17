namespace SchoolManagement.API.DTOs;

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
