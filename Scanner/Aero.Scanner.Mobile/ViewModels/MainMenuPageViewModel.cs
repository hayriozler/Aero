using Aero.Scanner.Mobile.Config;
using Aero.Scanner.Mobile.Localization;

namespace Aero.Scanner.Mobile.ViewModels;

public sealed class MainMenuPageViewModel : BaseViewModel
{
    private readonly AppConfig _config;

    public IReadOnlyList<MenuItemViewModel> MenuItems => _mainVm.MenuItems;

    private bool _isExitOverlayVisible;
    public bool IsExitOverlayVisible
    {
        get => _isExitOverlayVisible;
        set => SetField(ref _isExitOverlayVisible, value);
    }

    private string _exitPassword = string.Empty;
    public string ExitPassword
    {
        get => _exitPassword;
        set => SetField(ref _exitPassword, value);
    }

    private string _exitPasswordError = string.Empty;
    public string ExitPasswordError
    {
        get => _exitPasswordError;
        set
        {
            SetField(ref _exitPasswordError, value);
            OnPropertyChanged(nameof(HasExitPasswordError));
        }
    }

    public bool HasExitPasswordError => !string.IsNullOrEmpty(_exitPasswordError);

    // Translatable labels
    public string AppTitle          => Strings.AppTitle;
    public string ExitLabel         => Strings.ExitButton;
    public string ExitPasswordTitle => Strings.ExitPasswordTitle;
    public string ExitPasswordHint  => Strings.ExitPasswordHint;
    public string ConfirmLabel      => Strings.ExitConfirm;
    public string CancelLabel       => Strings.ExitCancel;
    public string DeviceName        => _config.HhtName;
    public string StatusText        => $"Gateway: {_config.GatewayUrl}";

    public Command ShowExitOverlayCommand => new(() =>
    {
        ExitPassword = string.Empty;
        ExitPasswordError = string.Empty;
        IsExitOverlayVisible = true;
    });

    public Command CancelExitCommand => new(() =>
    {
        ExitPassword = string.Empty;
        ExitPasswordError = string.Empty;
        IsExitOverlayVisible = false;
    });

    public Command ConfirmExitCommand => new(() =>
    {
        if (ExitPassword == _config.ExitPassword)
        {
            Application.Current!.Quit();
            return;
        }
        ExitPasswordError = Strings.ExitPasswordWrong;
        ExitPassword = string.Empty;
    });

    private readonly MainMenuViewModel _mainVm;

    public MainMenuPageViewModel(MainMenuViewModel mainVm, AppConfig config)
    {
        _mainVm = mainVm;
        _config = config;
    }

    public void RefreshLabels()
    {
        OnPropertyChanged(nameof(AppTitle));
        OnPropertyChanged(nameof(ExitLabel));
        OnPropertyChanged(nameof(ExitPasswordTitle));
        OnPropertyChanged(nameof(ExitPasswordHint));
        OnPropertyChanged(nameof(ConfirmLabel));
        OnPropertyChanged(nameof(CancelLabel));
        OnPropertyChanged(nameof(MenuItems));
    }
}
