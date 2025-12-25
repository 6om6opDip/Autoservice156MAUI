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
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Autoservice156MAUI;
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();

        // Загружаем конфигурацию из appsettings.json
        var assembly = typeof(App).Assembly;
        using var stream = assembly.GetManifestResourceStream("Autoservice156MAUI.appsettings.json");

        if (stream != null)
        {
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);
        }
        else
        {
            Console.WriteLine("⚠️ appsettings.json не найден как EmbeddedResource");
        }

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Регистрация сервисов
        builder.Services.AddSingleton<ApiService>();
        builder.Services.AddSingleton<IApiService>(sp => sp.GetRequiredService<ApiService>());
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<IAuthService>(sp => sp.GetRequiredService<AuthService>());
        builder.Services.AddSingleton<VehicleService>();
        builder.Services.AddSingleton<IVehicleService>(sp => sp.GetRequiredService<VehicleService>());
        builder.Services.AddSingleton<ClientService>();
        builder.Services.AddSingleton<IClientService>(sp => sp.GetRequiredService<ClientService>());
        builder.Services.AddSingleton<ServiceService>();
        builder.Services.AddSingleton<IServiceService>(sp => sp.GetRequiredService<ServiceService>());
        builder.Services.AddSingleton<IServiceService, ServiceService>();
        builder.Services.AddSingleton<ServiceListViewModel>();
        builder.Services.AddSingleton<ServiceListPage>();
        builder.Services.AddSingleton<ServiceEditPage>();

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