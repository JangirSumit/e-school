using System.Windows.Input;
using SchoolManagement.Services;

namespace SchoolManagement.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _email;
    private string _password;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }
    public ICommand SignupCommand { get; }

    public LoginViewModel(IAuthService authService)
    {
        _authService = authService;
        LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
        SignupCommand = new Command(async () => await NavigateToSignupAsync());
    }

    [Obsolete]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please enter email and password", "OK");
            return;
        }

        IsBusy = true;
        try
        {
            var result = await _authService.LoginAsync(Email, Password);

            if (result.Success)
            {
                await Shell.Current.GoToAsync("//DashboardPage");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", result.Message, "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task NavigateToSignupAsync()
    {
        await Shell.Current.GoToAsync("SignupPage");
    }
}
