using SchoolManagement.ViewModels;

namespace SchoolManagement.Views;

public partial class SignupPage : ContentPage
{
    public SignupPage(SignupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
