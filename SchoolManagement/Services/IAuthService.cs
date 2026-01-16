using SchoolManagement.Models;

namespace SchoolManagement.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, User User)> LoginAsync(string email, string password);
    Task<(bool Success, string Message)> SignupSchoolAsync(Tenant tenant, User adminUser, string password);
    Task LogoutAsync();
}
