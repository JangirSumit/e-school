using SchoolManagement.Data;
using SchoolManagement.Helpers;
using SchoolManagement.Models;

namespace SchoolManagement.Services;

public class AuthService : IAuthService
{
    private readonly IDataStore _dataStore;

    public AuthService(IDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<(bool Success, string Message, User User)> LoginAsync(string email, string password)
    {
        var user = await _dataStore.GetUserByEmailAsync(email);
        if (user == null || user.PasswordHash != HashPassword(password))
            return (false, "Invalid credentials", null);

        SessionManager.CurrentUserId = user.Id;
        SessionManager.CurrentTenantId = user.TenantId;
        SessionManager.CurrentUserRole = user.Role.ToString();
        SessionManager.CurrentUserName = user.FullName;

        return (true, "Login successful", user);
    }

    public async Task<(bool Success, string Message)> SignupSchoolAsync(Tenant tenant, User adminUser, string password)
    {
        var existing = await _dataStore.GetUserByEmailAsync(adminUser.Email);
        if (existing != null)
            return (false, "Email already exists");

        tenant.IsActive = true;
        tenant.SubscriptionStart = DateTime.UtcNow;
        tenant.SubscriptionEnd = DateTime.UtcNow.AddYears(1);

        await _dataStore.AddTenantAsync(tenant);

        adminUser.TenantId = tenant.Id;
        adminUser.Role = UserRole.Admin;
        adminUser.PasswordHash = HashPassword(password);

        await _dataStore.AddUserAsync(adminUser);

        return (true, "School registered successfully");
    }

    public Task LogoutAsync()
    {
        SessionManager.ClearSession();
        return Task.CompletedTask;
    }

    private string HashPassword(string password) => 
        Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
}
