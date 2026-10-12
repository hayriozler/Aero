using Aero.Scanner.Mobile.Localization;
using Aero.Scanner.Mobile.Pages;

namespace Aero.Scanner.Mobile.ViewModels;

public sealed class MainMenuViewModel : BaseViewModel
{
    private static readonly Color[] _palette =
    [
        Color.FromArgb("#1565C0"), // blue
        Color.FromArgb("#2E7D32"), // green
        Color.FromArgb("#E65100"), // orange
        Color.FromArgb("#6A1B9A"), // purple
        Color.FromArgb("#00695C"), // teal
    ];

    private string _deviceName = string.Empty;
    public string DeviceName
    {
        get => _deviceName;
        set => SetField(ref _deviceName, value);
    }

    private bool _isExitOverlayVisible;
    public bool IsExitOverlayVisible
    {
        get => _isExitOverlayVisible;
        set => SetField(ref _isExitOverlayVisible, value);
    }

    public IReadOnlyList<MenuItemViewModel> MenuItems { get; }

    public MainMenuViewModel()
    {
        MenuItems = BuildMenuItems();
        Strings.LanguageChanged += Refresh;
    }

    private void Refresh() => OnPropertyChanged(nameof(MenuItems));

    private List<MenuItemViewModel> BuildMenuItems()
    {
        var definitions = new[]
        {
            (Strings.MenuArrivalFlights,    nameof(ArrivalFlightsPage)),
            (Strings.MenuDepartureFlights,  nameof(DepartureFlightsPage)),
            (Strings.MenuVehicleList,       nameof(VehicleListPage)),
            (Strings.MenuAddVehicle,        nameof(AddVehiclePage)),
            (Strings.MenuStatus,            nameof(StatusPage)),
            (Strings.MenuBagActivation,     nameof(BagActivationQueryPage)),
            (Strings.MenuDownloadBaggage,   nameof(DownloadBaggagePage)),
            (Strings.MenuDepartureStatus,   nameof(DepartureStatusPage)),
            (Strings.MenuForceLoad,         nameof(ForceLoadPage)),
            (Strings.MenuPassengerQuery,    nameof(PassengerQueryPage)),
            (Strings.MenuBaggageToUnload,   nameof(BaggageToUnloadPage)),
            (Strings.MenuDeviceQuery,       nameof(DeviceQueryPage)),
            (Strings.MenuConnectionBaggage, nameof(ConnectionBaggagePage)),
            (Strings.MenuFlightList,        nameof(FlightListPage)),
            (Strings.MenuArrFlightList,     nameof(ArrFlightListPage)),
        };

        var items = new List<MenuItemViewModel>(definitions.Length);
        for (var i = 0; i < definitions.Length; i++)
        {
            var (title, route) = definitions[i];
            var color = _palette[i % _palette.Length];
            items.Add(new MenuItemViewModel
            {
                Title = title,
                Route = route,
                Background = color,
                NavigateCommand = new Command<string>(static async r =>
                    await Shell.Current.GoToAsync(r))
            });
        }
        return items;
    }
}
