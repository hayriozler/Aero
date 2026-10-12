using Aero.Scanner.Mobile.Config;
using Aero.Scanner.Mobile.Localization;
using Aero.Scanner.Mobile.ViewModels;

namespace Aero.Scanner.Mobile.Pages;

public partial class MainMenuPage : ContentPage
{
    private readonly MainMenuPageViewModel _vm;

    public MainMenuPage(MainMenuViewModel menuVm, AppConfig config)
    {
        InitializeComponent();
        _vm = new MainMenuPageViewModel(menuVm, config);
        BindingContext = _vm;
    }

    protected override bool OnBackButtonPressed()
    {
        _vm.ShowExitOverlayCommand.Execute(null);
        return true;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Strings.LanguageChanged += RefreshBindings;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Strings.LanguageChanged -= RefreshBindings;
    }

    private void RefreshBindings() => _vm.RefreshLabels();
}
