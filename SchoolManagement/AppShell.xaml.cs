using SchoolManagement.Views;

namespace SchoolManagement;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		Routing.RegisterRoute("SignupPage", typeof(SignupPage));
		Routing.RegisterRoute("StudentsPage", typeof(StudentsPage));
	}
}
