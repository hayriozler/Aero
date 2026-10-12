using System.Windows.Input;

namespace Aero.Scanner.Mobile.ViewModels;

public sealed class MenuItemViewModel
{
    public required string Title { get; init; }
    public required string Route { get; init; }
    public required Color Background { get; init; }
    public required ICommand NavigateCommand { get; init; }
}
