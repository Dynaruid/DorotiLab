using Android.App;
using Android.Content.PM;
using Microsoft.Maui;

namespace DorotiTestbedApp.Android;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    Exported = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public sealed class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(global::Android.OS.Bundle? savedInstanceState)
    {
        if (Intent?.GetStringExtra("doroti_testbed_mode") == "media-query")
            Environment.SetEnvironmentVariable("DOROTI_TESTBED_MODE", "media-query");
        base.OnCreate(savedInstanceState);
    }
}
