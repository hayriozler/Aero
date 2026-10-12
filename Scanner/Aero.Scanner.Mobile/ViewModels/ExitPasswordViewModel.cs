using Aero.Scanner.Mobile.Config;
using Aero.Scanner.Mobile.Localization;

namespace Aero.Scanner.Mobile.ViewModels;

public sealed class ExitPasswordViewModel(AppConfig config, Action hideOverlay) : BaseViewModel
{
    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => SetField(ref _password, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetField(ref _errorMessage, value);
    }

    public Command ConfirmCommand => new(OnConfirm);
    public Command CancelCommand => new(OnCancel);

    private void OnConfirm()
    {
        if (Password == config.ExitPassword)
        {
            Application.Current!.Quit();
            return;
        }
        ErrorMessage = Strings.ExitPasswordWrong;
        Password = string.Empty;
    }

    private void OnCancel()
    {
        ErrorMessage = string.Empty;
        Password = string.Empty;
        hideOverlay();
    }
}
