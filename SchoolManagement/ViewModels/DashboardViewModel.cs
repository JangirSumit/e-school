using System.Windows.Input;
using SchoolManagement.Helpers;
using SchoolManagement.Services;

namespace SchoolManagement.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private string _userName;

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public ICommand LogoutCommand { get; }
    public ICommand NavigateToStudentsCommand { get; }
    public ICommand NavigateToFacultiesCommand { get; }
    public ICommand NavigateToAttendanceCommand { get; }
    public ICommand NavigateToMarksCommand { get; }
    public ICommand NavigateToHolidaysCommand { get; }

    public DashboardViewModel(IAuthService authService)
    {
        _authService = authService;
        UserName = SessionManager.CurrentUserName;
        
        LogoutCommand = new Command(async () => await LogoutAsync());
        NavigateToStudentsCommand = new Command(async () => await Shell.Current.GoToAsync("StudentsPage"));
        NavigateToFacultiesCommand = new Command(async () => await Shell.Current.GoToAsync("FacultiesPage"));
        NavigateToAttendanceCommand = new Command(async () => await Shell.Current.GoToAsync("AttendancePage"));
        NavigateToMarksCommand = new Command(async () => await Shell.Current.GoToAsync("MarksPage"));
        NavigateToHolidaysCommand = new Command(async () => await Shell.Current.GoToAsync("HolidaysPage"));
    }

    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }
}
