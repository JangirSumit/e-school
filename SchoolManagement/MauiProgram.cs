using Microsoft.Extensions.Logging;
using SchoolManagement.Data;
using SchoolManagement.Services;
using SchoolManagement.ViewModels;
using SchoolManagement.Views;
using UraniumUI;

namespace SchoolManagement;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseUraniumUI()
			.UseUraniumUIMaterial()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddMaterialIconFonts();
			});

		builder.Services.AddSingleton<IApiService, ApiService>();
		builder.Services.AddSingleton<IAuthService, AuthService>();
		
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<SignupPage>();
		builder.Services.AddTransient<SignupViewModel>();
		builder.Services.AddTransient<DashboardPage>();
		builder.Services.AddTransient<DashboardViewModel>();
		builder.Services.AddTransient<StudentsPage>();
		builder.Services.AddTransient<StudentsViewModel>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
