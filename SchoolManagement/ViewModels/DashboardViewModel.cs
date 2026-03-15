using System.Collections.ObjectModel;
using System.Windows.Input;
using SchoolManagement.DTOs;
using SchoolManagement.Helpers;
using SchoolManagement.Models;
using SchoolManagement.Services;

namespace SchoolManagement.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IApiService _apiService;

    private string _userName = string.Empty;
    private string _role = string.Empty;
    private string _schoolHeader = string.Empty;
    private string _summaryText = string.Empty;

    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string Role
    {
        get => _role;
        set
        {
            if (SetProperty(ref _role, value))
            {
                OnPropertyChanged(nameof(IsOwner));
                OnPropertyChanged(nameof(IsSchoolAdmin));
                OnPropertyChanged(nameof(IsFaculty));
                OnPropertyChanged(nameof(IsParent));
                OnPropertyChanged(nameof(IsStudent));
            }
        }
    }

    public string SchoolHeader
    {
        get => _schoolHeader;
        set => SetProperty(ref _schoolHeader, value);
    }

    public string SummaryText
    {
        get => _summaryText;
        set => SetProperty(ref _summaryText, value);
    }

    public bool IsOwner => Role == "Owner";
    public bool IsSchoolAdmin => Role == "SchoolAdmin";
    public bool IsFaculty => Role == "Faculty";
    public bool IsParent => Role == "Parent";
    public bool IsStudent => Role == "Student";

    public ObservableCollection<SummaryMetric> Metrics { get; } = new();
    public ObservableCollection<ActionItem> Highlights { get; } = new();
    public ObservableCollection<TenantSummaryResponse> Schools { get; } = new();
    public ObservableCollection<ClassResponse> Classes { get; } = new();
    public ObservableCollection<FacultyResponse> FacultyMembers { get; } = new();
    public ObservableCollection<StudentResponse> Students { get; } = new();
    public ObservableCollection<ParentChildResponse> Children { get; } = new();
    public ObservableCollection<Holiday> Holidays { get; } = new();

    public ICommand RefreshCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand AddSchoolCommand { get; }
    public ICommand ToggleSchoolStatusCommand { get; }
    public ICommand AddClassCommand { get; }
    public ICommand AddFacultyCommand { get; }
    public ICommand AddStudentCommand { get; }
    public ICommand AddHolidayCommand { get; }

    public DashboardViewModel(IAuthService authService, IApiService apiService)
    {
        _authService = authService;
        _apiService = apiService;

        UserName = SessionManager.CurrentUserName ?? string.Empty;
        Role = SessionManager.CurrentUserRole ?? string.Empty;
        SchoolHeader = BuildSchoolHeader(SessionManager.CurrentSchoolName, SessionManager.CurrentSchoolCode);

        RefreshCommand = new Command(async () => await LoadDashboardAsync());
        LogoutCommand = new Command(async () => await LogoutAsync());
        AddSchoolCommand = new Command(async () => await AddSchoolAsync());
        ToggleSchoolStatusCommand = new Command(async () => await ToggleSchoolStatusAsync());
        AddClassCommand = new Command(async () => await AddClassAsync());
        AddFacultyCommand = new Command(async () => await AddFacultyAsync());
        AddStudentCommand = new Command(async () => await AddStudentAsync());
        AddHolidayCommand = new Command(async () => await AddHolidayAsync());
    }

    public async Task InitializeAsync()
    {
        if (IsBusy)
            return;

        await LoadDashboardAsync();
    }

    private async Task LoadDashboardAsync()
    {
        try
        {
            IsBusy = true;
            var dashboard = await _apiService.GetDashboardAsync();
            if (dashboard == null)
                return;

            UserName = dashboard.UserName;
            Role = dashboard.Role;
            SchoolHeader = BuildSchoolHeader(dashboard.SchoolName, dashboard.SchoolCode);
            SummaryText = BuildSummaryText(dashboard);

            ReplaceCollection(Metrics, dashboard.Metrics);
            ReplaceCollection(Highlights, dashboard.Highlights);
            ReplaceCollection(Schools, dashboard.Schools);
            ReplaceCollection(Classes, dashboard.Classes);
            ReplaceCollection(FacultyMembers, dashboard.FacultyMembers);
            ReplaceCollection(Students, dashboard.Students);
            ReplaceCollection(Children, dashboard.Children);
            await LoadHolidaysAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Unable to load dashboard: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddSchoolAsync()
    {
        var schoolName = await PromptAsync("Onboard School", "School name");
        if (string.IsNullOrWhiteSpace(schoolName)) return;

        var contactPerson = await PromptAsync("Onboard School", "Contact person");
        if (string.IsNullOrWhiteSpace(contactPerson)) return;

        var email = await PromptAsync("Onboard School", "School email", Keyboard.Email);
        if (string.IsNullOrWhiteSpace(email)) return;

        var phone = await PromptAsync("Onboard School", "Phone", Keyboard.Telephone);
        if (string.IsNullOrWhiteSpace(phone)) return;

        var address = await PromptAsync("Onboard School", "Address");
        if (string.IsNullOrWhiteSpace(address)) return;

        var adminName = await PromptAsync("Onboard School", "Admin full name");
        if (string.IsNullOrWhiteSpace(adminName)) return;

        var adminUsername = await PromptAsync("Onboard School", "Admin username");
        if (string.IsNullOrWhiteSpace(adminUsername)) return;

        var adminPassword = await PromptAsync("Onboard School", "Admin password");
        if (string.IsNullOrWhiteSpace(adminPassword)) return;

        try
        {
            IsBusy = true;
            await _apiService.CreateSchoolAsync(new CreateSchoolRequest(
                schoolName,
                email,
                phone,
                address,
                contactPerson,
                adminName,
                adminUsername,
                adminPassword));

            await LoadDashboardAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Unable to onboard school: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ToggleSchoolStatusAsync()
    {
        var schoolCode = await PromptAsync("Manage School", "School code to enable/disable");
        if (string.IsNullOrWhiteSpace(schoolCode)) return;

        var school = Schools.FirstOrDefault(s => string.Equals(s.SchoolCode, schoolCode.Trim(), StringComparison.OrdinalIgnoreCase));
        if (school == null)
        {
            await Application.Current!.MainPage!.DisplayAlert("Not found", "No school found with that school code.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            await _apiService.UpdateSchoolStatusAsync(school.Id, !school.IsActive);
            await LoadDashboardAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Unable to update school status: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddClassAsync()
    {
        var className = await PromptAsync("Create Class", "Class name");
        if (string.IsNullOrWhiteSpace(className)) return;

        var section = await PromptAsync("Create Class", "Section");
        if (string.IsNullOrWhiteSpace(section)) return;

        var capacityText = await PromptAsync("Create Class", "Capacity", Keyboard.Numeric);
        if (!int.TryParse(capacityText, out var capacity))
            capacity = 30;

        try
        {
            IsBusy = true;
            await _apiService.CreateClassAsync(new CreateClassRequest(className, section, capacity, null));
            await LoadDashboardAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Unable to create class: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddFacultyAsync()
    {
        var fullName = await PromptAsync("Add Faculty", "Full name");
        if (string.IsNullOrWhiteSpace(fullName)) return;

        var username = await PromptAsync("Add Faculty", "Username");
        if (string.IsNullOrWhiteSpace(username)) return;

        var department = await PromptAsync("Add Faculty", "Department");
        if (string.IsNullOrWhiteSpace(department)) return;

        var qualification = await PromptAsync("Add Faculty", "Qualification");
        if (string.IsNullOrWhiteSpace(qualification)) return;

        var employeeId = await PromptAsync("Add Faculty", "Employee ID");
        if (string.IsNullOrWhiteSpace(employeeId)) return;

        var email = await PromptAsync("Add Faculty", "Email", Keyboard.Email);
        if (string.IsNullOrWhiteSpace(email)) return;

        var phone = await PromptAsync("Add Faculty", "Phone", Keyboard.Telephone);
        if (string.IsNullOrWhiteSpace(phone)) return;

        var password = await PromptAsync("Add Faculty", "Password");
        if (string.IsNullOrWhiteSpace(password)) return;

        try
        {
            IsBusy = true;
            await _apiService.CreateFacultyAsync(new CreateFacultyRequest(
                fullName,
                username,
                email,
                phone,
                employeeId,
                department,
                qualification,
                DateTime.UtcNow.Date,
                password));
            await LoadDashboardAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Unable to add faculty: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddStudentAsync()
    {
        var fullName = await PromptAsync("Admit Student", "Student full name");
        if (string.IsNullOrWhiteSpace(fullName)) return;

        var username = await PromptAsync("Admit Student", "Student username");
        if (string.IsNullOrWhiteSpace(username)) return;

        var rollNumber = await PromptAsync("Admit Student", "Roll number");
        if (string.IsNullOrWhiteSpace(rollNumber)) return;

        var className = await PromptAsync("Admit Student", "Class");
        if (string.IsNullOrWhiteSpace(className)) return;

        var section = await PromptAsync("Admit Student", "Section");
        if (string.IsNullOrWhiteSpace(section)) return;

        var parentName = await PromptAsync("Admit Student", "Parent name");
        if (string.IsNullOrWhiteSpace(parentName)) return;

        var parentUsername = await PromptAsync("Admit Student", "Parent username");
        if (string.IsNullOrWhiteSpace(parentUsername)) return;

        var parentPhone = await PromptAsync("Admit Student", "Parent phone", Keyboard.Telephone);
        if (string.IsNullOrWhiteSpace(parentPhone)) return;

        var studentPassword = await PromptAsync("Admit Student", "Student password");
        if (string.IsNullOrWhiteSpace(studentPassword)) return;

        var parentPassword = await PromptAsync("Admit Student", "Parent password");
        if (string.IsNullOrWhiteSpace(parentPassword)) return;

        try
        {
            IsBusy = true;
            await _apiService.CreateStudentAsync(new CreateStudentRequest(
                username,
                fullName,
                string.Empty,
                parentPhone,
                rollNumber,
                className,
                section,
                DateTime.UtcNow.Date.AddYears(-10),
                parentName,
                parentUsername,
                string.Empty,
                parentPhone,
                parentPassword,
                studentPassword));
            await LoadDashboardAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Unable to admit student: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task AddHolidayAsync()
    {
        var name = await PromptAsync("Add Holiday", "Holiday name");
        if (string.IsNullOrWhiteSpace(name)) return;

        var dateText = await PromptAsync("Add Holiday", "Date (YYYY-MM-DD)");
        if (!DateTime.TryParse(dateText, out var date))
            date = DateTime.UtcNow.Date;

        var description = await PromptAsync("Add Holiday", "Description");
        description ??= string.Empty;

        try
        {
            IsBusy = true;
            await _apiService.CreateHolidayAsync(new Holiday
            {
                Name = name,
                Date = date,
                Description = description
            });
            await LoadHolidaysAsync();
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("Error", $"Unable to add holiday: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadHolidaysAsync()
    {
        if (!(IsSchoolAdmin || IsParent || Role == "Student"))
            return;

        var holidays = await _apiService.GetHolidaysAsync();
        ReplaceCollection(Holidays, holidays.OrderBy(h => h.Date));
    }

    private async Task LogoutAsync()
    {
        await _authService.LogoutAsync();
        await Shell.Current.GoToAsync("//LoginPage");
    }

    private static void ReplaceCollection<T>(ObservableCollection<T> target, IEnumerable<T> items)
    {
        target.Clear();
        foreach (var item in items)
            target.Add(item);
    }

    private static string BuildSchoolHeader(string? schoolName, string? schoolCode)
    {
        if (string.IsNullOrWhiteSpace(schoolName))
            return "Multi-tenant eSchool platform";

        return string.IsNullOrWhiteSpace(schoolCode) ? schoolName : $"{schoolName} ({schoolCode})";
    }

    private static string BuildSummaryText(DashboardResponse dashboard)
    {
        if (dashboard.Role == "Owner")
            return "Manage onboarding, school access, and the whole app from one place.";

        if (dashboard.Role == "SchoolAdmin")
            return "Run daily operations including classes, faculty, students, and parent access.";

        if (dashboard.Role == "Faculty")
            return "Track your school sections, student directory, and classroom progress.";

        if (dashboard.Role == "Parent")
            return "See your children, attendance, and marks in one parent view.";

        return "School dashboard";
    }

    private static Task<string?> PromptAsync(string title, string message, Keyboard? keyboard = null) =>
        Application.Current!.MainPage!.DisplayPromptAsync(title, message, keyboard: keyboard);
}
