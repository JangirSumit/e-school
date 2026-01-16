using System.Collections.ObjectModel;
using System.Windows.Input;
using SchoolManagement.Data;
using SchoolManagement.Helpers;
using SchoolManagement.Models;

namespace SchoolManagement.ViewModels;

public class StudentsViewModel : BaseViewModel
{
    private readonly IDataStore _dataStore;
    public ObservableCollection<Student> Students { get; } = new();
    public ICommand AddStudentCommand { get; }

    public StudentsViewModel(IDataStore dataStore)
    {
        _dataStore = dataStore;
        AddStudentCommand = new Command(async () => await AddStudentAsync());
        LoadStudents();
    }

    private async void LoadStudents()
    {
        var students = await _dataStore.GetStudentsAsync(SessionManager.CurrentTenantId);
        Students.Clear();
        foreach (var student in students)
            Students.Add(student);
    }

    private async Task AddStudentAsync()
    {
        await Application.Current.MainPage.DisplayAlert("Info", "Add student feature coming soon", "OK");
    }
}
