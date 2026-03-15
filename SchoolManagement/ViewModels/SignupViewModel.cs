using System.Windows.Input;
using SchoolManagement.Models;
using SchoolManagement.Services;

namespace SchoolManagement.ViewModels;

public class SignupViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _schoolName;
    private string _email;
    private string _phone;
    private string _address;
    private string _adminName;
    private string _adminUsername;
    private string _password;

    public string SchoolName { get => _schoolName; set => SetProperty(ref _schoolName, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
    public string Address { get => _address; set => SetProperty(ref _address, value); }
    public string AdminName { get => _adminName; set => SetProperty(ref _adminName, value); }
    public string AdminUsername { get => _adminUsername; set => SetProperty(ref _adminUsername, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }

    public ICommand SignupCommand { get; }

    public SignupViewModel(IAuthService authService)
    {
        _authService = authService;
        SignupCommand = new Command(async () => await SignupAsync(), () => !IsBusy);
    }

    [Obsolete]
    private async Task SignupAsync()
    {
        if (string.IsNullOrWhiteSpace(SchoolName))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "School name is required", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Email is required", "OK");
            return;
        }

        if (!IsValidEmail(Email))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please enter a valid email address", "OK");
            return;
        }

        if (!string.IsNullOrWhiteSpace(Phone) && Phone.Length < 10)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Phone number must be at least 10 digits", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Password is required", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(AdminUsername))
        {
            AdminUsername = "admin";
        }

        if (Password.Length < 6)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Password must be at least 6 characters", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var tenant = new Tenant { SchoolName = SchoolName, Email = Email, Phone = Phone, Address = Address };
            var admin = new User { Username = AdminUsername, Email = Email, FullName = AdminName, Phone = Phone };
            var result = await _authService.SignupSchoolAsync(tenant, admin, Password);

            if (result.Success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", result.Message, "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", result.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Sign up failed: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
