using Aero.Scanner.Mobile.Pages;

namespace Aero.Scanner.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ArrivalFlightsPage),    typeof(ArrivalFlightsPage));
        Routing.RegisterRoute(nameof(DepartureFlightsPage),  typeof(DepartureFlightsPage));
        Routing.RegisterRoute(nameof(VehicleListPage),       typeof(VehicleListPage));
        Routing.RegisterRoute(nameof(AddVehiclePage),        typeof(AddVehiclePage));
        Routing.RegisterRoute(nameof(StatusPage),            typeof(StatusPage));
        Routing.RegisterRoute(nameof(BagActivationQueryPage),typeof(BagActivationQueryPage));
        Routing.RegisterRoute(nameof(DownloadBaggagePage),   typeof(DownloadBaggagePage));
        Routing.RegisterRoute(nameof(DepartureStatusPage),   typeof(DepartureStatusPage));
        Routing.RegisterRoute(nameof(ForceLoadPage),         typeof(ForceLoadPage));
        Routing.RegisterRoute(nameof(PassengerQueryPage),    typeof(PassengerQueryPage));
        Routing.RegisterRoute(nameof(BaggageToUnloadPage),   typeof(BaggageToUnloadPage));
        Routing.RegisterRoute(nameof(DeviceQueryPage),       typeof(DeviceQueryPage));
        Routing.RegisterRoute(nameof(ConnectionBaggagePage), typeof(ConnectionBaggagePage));
        Routing.RegisterRoute(nameof(FlightListPage),        typeof(FlightListPage));
        Routing.RegisterRoute(nameof(ArrFlightListPage),     typeof(ArrFlightListPage));
    }
}
