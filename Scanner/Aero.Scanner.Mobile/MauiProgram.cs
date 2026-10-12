using Aero.Scanner.Mobile.Config;
using Aero.Scanner.Mobile.Localization;
using Aero.Scanner.Mobile.Pages;
using Aero.Scanner.Mobile.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace Aero.Scanner.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        var config = AppConfig.Load();
        Strings.SetLanguage(config.Language);

        builder.Services.AddSingleton(config);
        builder.Services.AddSingleton<MainMenuViewModel>();
        builder.Services.AddTransient<ExitPasswordViewModel>();

        // Pages
        builder.Services.AddTransient<MainMenuPage>();
        builder.Services.AddTransient<ArrivalFlightsPage>();
        builder.Services.AddTransient<DepartureFlightsPage>();
        builder.Services.AddTransient<VehicleListPage>();
        builder.Services.AddTransient<AddVehiclePage>();
        builder.Services.AddTransient<StatusPage>();
        builder.Services.AddTransient<BagActivationQueryPage>();
        builder.Services.AddTransient<DownloadBaggagePage>();
        builder.Services.AddTransient<DepartureStatusPage>();
        builder.Services.AddTransient<ForceLoadPage>();
        builder.Services.AddTransient<PassengerQueryPage>();
        builder.Services.AddTransient<BaggageToUnloadPage>();
        builder.Services.AddTransient<DeviceQueryPage>();
        builder.Services.AddTransient<ConnectionBaggagePage>();
        builder.Services.AddTransient<FlightListPage>();
        builder.Services.AddTransient<ArrFlightListPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
