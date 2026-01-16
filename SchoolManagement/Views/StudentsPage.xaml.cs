using SchoolManagement.ViewModels;

namespace SchoolManagement.Views;

public partial class StudentsPage : ContentPage
{
    public StudentsPage(StudentsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
