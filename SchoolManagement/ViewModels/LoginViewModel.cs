using System.Windows.Input;
using SchoolManagement.Services;

namespace SchoolManagement.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IApiService _apiService;
    private string _email;
    private string _password;
    private string _appName = "eSchool";
    private string _tagline = "School management made simple.";
    private string _supportEmail = "support@eschool.app";
    private string _supportPhone = "";
    private string _supportWhatsApp = "";
    private string _helpMessage = "";
    private string _loginHint = "Owner uses email. School users log in with SCHOOLCODE@username.";

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

    public string AppName { get => _appName; set => SetProperty(ref _appName, value); }
    public string Tagline { get => _tagline; set => SetProperty(ref _tagline, value); }
    public string SupportEmail { get => _supportEmail; set => SetProperty(ref _supportEmail, value); }
    public string SupportPhone { get => _supportPhone; set => SetProperty(ref _supportPhone, value); }
    public string SupportWhatsApp { get => _supportWhatsApp; set => SetProperty(ref _supportWhatsApp, value); }
    public string HelpMessage { get => _helpMessage; set => SetProperty(ref _helpMessage, value); }
    public string LoginHint { get => _loginHint; set => SetProperty(ref _loginHint, value); }

    public ICommand LoginCommand { get; }
    public ICommand SignupCommand { get; }

    public LoginViewModel(IAuthService authService, IApiService apiService)
    {
        _authService = authService;
        _apiService = apiService;
        LoginCommand = new Command(async () => await LoginAsync(), () => !IsBusy);
        SignupCommand = new Command(async () => await NavigateToSignupAsync());
        _ = LoadAppInfoAsync();
    }

    private async Task LoadAppInfoAsync()
    {
        try
        {
            var info = await _apiService.GetPublicAppInfoAsync();
            if (info == null)
                return;

            AppName = info.AppName;
            Tagline = info.Tagline;
            SupportEmail = info.SupportEmail;
            SupportPhone = info.SupportPhone;
            SupportWhatsApp = info.SupportWhatsApp;
            HelpMessage = info.HelpMessage;
            LoginHint = info.LoginHint;
        }
        catch
        {
            // Keep sensible defaults if API info is unavailable.
        }
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
