using Microsoft.Extensions.Logging;
using Autoservice156MAUI.Services;
using Autoservice156MAUI.Services.Interfaces;
using Autoservice156MAUI.ViewModels.Auth;
using Autoservice156MAUI.ViewModels.Client;
using Autoservice156MAUI.ViewModels.Vehicle;
using Autoservice156MAUI.ViewModels.Service;
using Autoservice156MAUI.Views.Auth;
using Autoservice156MAUI.Views.Client;
using Autoservice156MAUI.Views.Vehicle;
using Autoservice156MAUI.Views.Service;
using CommunityToolkit.Maui;

namespace Autoservice156MAUI;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>().ConfigureFonts(fonts =>
        {
            fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        }).UseMauiCommunityToolkit();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        // Регистрация сервисов
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IClientService, ClientService>();
        builder.Services.AddSingleton<IVehicleService, VehicleService>();
        builder.Services.AddSingleton<IServiceService, ServiceService>();
        // Регистрация ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ClientListViewModel>();
        builder.Services.AddTransient<ClientDetailsViewModel>();
        builder.Services.AddTransient<ClientEditViewModel>();
        builder.Services.AddTransient<VehicleListViewModel>();
        builder.Services.AddTransient<VehicleDetailsViewModel>();
        builder.Services.AddTransient<VehicleEditViewModel>();
        builder.Services.AddTransient<ServiceListViewModel>();
        builder.Services.AddTransient<ServiceDetailsViewModel>();
        // Регистрация Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ClientListPage>();
        builder.Services.AddTransient<ClientDetailsPage>();
        builder.Services.AddTransient<ClientEditPage>();
        builder.Services.AddTransient<VehicleListPage>();
        builder.Services.AddTransient<VehicleDetailsPage>();
        builder.Services.AddTransient<VehicleEditPage>();
        builder.Services.AddTransient<ServiceListPage>();
        return builder.Build();
    }
}