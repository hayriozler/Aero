using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace Aero.Scanner.Mobile.Platforms.Android;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ScreenOrientation = ScreenOrientation.Landscape,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                           ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
                           ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetFullScreen();
    }

    public override void OnWindowFocusChanged(bool hasFocus)
    {
        base.OnWindowFocusChanged(hasFocus);
        if (hasFocus) SetFullScreen();
    }

    private void SetFullScreen()
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            Window?.InsetsController?.Hide(
                global::Android.Views.WindowInsets.Type.StatusBars() |
                global::Android.Views.WindowInsets.Type.NavigationBars());
        }
        else
        {
            Window!.DecorView.SystemUiFlags =
                SystemUiFlags.Fullscreen |
                SystemUiFlags.HideNavigation |
                SystemUiFlags.ImmersiveSticky |
                SystemUiFlags.LayoutFullscreen |
                SystemUiFlags.LayoutHideNavigation |
                SystemUiFlags.LayoutStable;
        }
    }
}
