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
    private string _password;

    public string SchoolName { get => _schoolName; set => SetProperty(ref _schoolName, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
    public string Address { get => _address; set => SetProperty(ref _address, value); }
    public string AdminName { get => _adminName; set => SetProperty(ref _adminName, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }

    public ICommand SignupCommand { get; }

    public SignupViewModel(IAuthService authService)
    {
        _authService = authService;
        SignupCommand = new Command(async () => await SignupAsync());
    }

    [Obsolete]
    private async Task SignupAsync()
    {
        if (string.IsNullOrWhiteSpace(SchoolName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please fill all required fields", "OK");
            return;
        }

        IsBusy = true;
        var tenant = new Tenant { SchoolName = SchoolName, Email = Email, Phone = Phone, Address = Address };
        var admin = new User { Email = Email, FullName = AdminName, Phone = Phone };
        var result = await _authService.SignupSchoolAsync(tenant, admin, Password);
        IsBusy = false;

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
}
