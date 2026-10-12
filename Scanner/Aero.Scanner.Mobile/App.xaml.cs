using Aero.Scanner.Mobile.Config;
using Aero.Scanner.Mobile.Localization;

namespace Aero.Scanner.Mobile;

public partial class App : Application
{
    private readonly AppConfig _config;

    public App(AppConfig config)
    {
        _config = config;
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var errors = _config.Validate();
        if (errors.Count > 0)
        {
            var message = string.Join("\n", errors);
            var page = new ContentPage
            {
                BackgroundColor = Color.FromArgb("#B71C1C"),
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Padding = 40,
                    Spacing = 20,
                    Children =
                    {
                        new Label
                        {
                            Text = Strings.ConfigErrorTitle,
                            TextColor = Colors.White,
                            FontSize = 28,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalTextAlignment = TextAlignment.Center
                        },
                        new Label
                        {
                            Text = message,
                            TextColor = Colors.White,
                            FontSize = 18,
                            HorizontalTextAlignment = TextAlignment.Center
                        }
                    }
                }
            };
            return new Window(page);
        }

        return new Window(new AppShell());
    }
}
