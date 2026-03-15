using System.Collections.ObjectModel;
using System.Windows.Input;
using SchoolManagement.DTOs;
using SchoolManagement.Services;

namespace SchoolManagement.ViewModels;

public class StudentsViewModel : BaseViewModel
{
    private readonly IApiService _apiService;
    public ObservableCollection<StudentResponse> Students { get; } = new();
    public ICommand AddStudentCommand { get; }
    public ICommand RefreshCommand { get; }

    public StudentsViewModel(IApiService apiService)
    {
        _apiService = apiService;
        AddStudentCommand = new Command(async () => await AddStudentAsync());
        RefreshCommand = new Command(async () => await LoadStudentsAsync());
        _ = LoadStudentsAsync();
    }

    [Obsolete]
    private async Task LoadStudentsAsync()
    {
        try
        {
            IsBusy = true;
            var students = await _apiService.GetStudentsAsync();
            Students.Clear();
            foreach (var student in students)
                Students.Add(student);
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load students: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [Obsolete]
    private async Task AddStudentAsync()
    {
        var fullName = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Full Name:");
        if (string.IsNullOrWhiteSpace(fullName)) return;

        var email = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Email:", keyboard: Keyboard.Email);
        email ??= string.Empty;

        var username = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Username:");
        if (string.IsNullOrWhiteSpace(username)) return;

        var phone = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Phone:", keyboard: Keyboard.Telephone);
        if (string.IsNullOrWhiteSpace(phone)) return;

        var rollNumber = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Roll Number:");
        if (string.IsNullOrWhiteSpace(rollNumber)) return;

        var className = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Class:");
        if (string.IsNullOrWhiteSpace(className)) return;

        var section = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Section:");
        if (string.IsNullOrWhiteSpace(section)) return;

        var parentName = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Parent Name:");
        if (string.IsNullOrWhiteSpace(parentName)) return;

        var parentUsername = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Parent Username:");
        if (string.IsNullOrWhiteSpace(parentUsername)) return;

        var parentPhone = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Parent Phone:", keyboard: Keyboard.Telephone);
        if (string.IsNullOrWhiteSpace(parentPhone)) return;

        var parentPassword = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Parent Password:");
        if (string.IsNullOrWhiteSpace(parentPassword)) return;

        var password = await Application.Current.MainPage.DisplayPromptAsync("Add Student", "Password:");
        if (string.IsNullOrWhiteSpace(password)) return;

        try
        {
            IsBusy = true;
            var request = new CreateStudentRequest(
                username, fullName, email, phone, rollNumber, className, section,
                DateTime.Now.AddYears(-10), parentName, parentUsername, string.Empty, parentPhone, parentPassword, password
            );

            var student = await _apiService.CreateStudentAsync(request);
            if (student != null)
            {
                Students.Add(student);
                await Application.Current.MainPage.DisplayAlert("Success", "Student added successfully", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to add student: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
