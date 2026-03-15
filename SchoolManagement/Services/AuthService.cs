using SchoolManagement.DTOs;
using SchoolManagement.Helpers;

namespace SchoolManagement.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _apiService;

    public AuthService(IApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<(bool Success, string Message, Models.User User)> LoginAsync(string email, string password)
    {
        try
        {
            var response = await _apiService.LoginAsync(new LoginRequest(email, password));
            if (response == null)
                return (false, "Invalid credentials", null!);

            SessionManager.CurrentUserId = response.UserId;
            SessionManager.CurrentTenantId = response.TenantId;
            SessionManager.CurrentUserRole = response.Role;
            SessionManager.CurrentUserName = response.FullName;
            SessionManager.CurrentSchoolName = response.SchoolName;
            SessionManager.CurrentSchoolCode = response.SchoolCode;
            SessionManager.AuthToken = response.Token;

            _apiService.SetAuthToken(response.Token);

            return (true, "Login successful", new Models.User
            {
                Id = response.UserId,
                TenantId = response.TenantId,
                FullName = response.FullName,
                Email = email
            });
        }
        catch (Exception ex)
        {
            return (false, $"Login failed: {ex.Message}", null!);
        }
    }

    public async Task<(bool Success, string Message)> SignupSchoolAsync(Models.Tenant tenant, Models.User adminUser, string password)
    {
        try
        {
            var request = new SignupRequest(
                tenant.SchoolName,
                tenant.Email,
                tenant.Phone,
                tenant.Address,
                adminUser.FullName,
                string.IsNullOrWhiteSpace(adminUser.Username) ? "admin" : adminUser.Username,
                password
            );

            var response = await _apiService.SignupAsync(request);
            return response == null 
                ? (false, "Signup failed") 
                : (response.Success, response.Message);
        }
        catch (Exception ex)
        {
            return (false, $"Signup failed: {ex.Message}");
        }
    }

    public Task LogoutAsync()
    {
        SessionManager.ClearSession();
        return Task.CompletedTask;
    }
}
