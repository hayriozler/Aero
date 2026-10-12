using Aero.Scanner.Mobile.Localization;

namespace Aero.Scanner.Mobile.Pages;

public partial class StubPage : ContentPage
{
    public new string Title { get; }
    public string NotImplementedText => Strings.PageNotImplemented;
    public Command BackCommand => new(async () => await Shell.Current.GoToAsync(".."));

    protected StubPage(string title)
    {
        Title = title;
        InitializeComponent();
        BindingContext = this;
    }
}
