using Android.App;
using Android.Content.PM;
using Microsoft.Maui;

namespace DorotiTestbedApp.Android;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    Exported = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize
        | ConfigChanges.Orientation
        | ConfigChanges.UiMode
        | ConfigChanges.ScreenLayout
        | ConfigChanges.SmallestScreenSize
        | ConfigChanges.Density
)]
[IntentFilter(new[] { global::Android.Content.Intent.ActionView },
    Categories = new[] { global::Android.Content.Intent.CategoryDefault, global::Android.Content.Intent.CategoryBrowsable },
    DataScheme = "doroti-testbed")]
public sealed class MainActivity : Doroti.Host.Maui.DorotiMauiActivity
{
    protected override void OnCreate(global::Android.OS.Bundle? savedInstanceState)
    {
        if (Intent?.GetStringExtra("doroti_sample") is { } sample)
            Environment.SetEnvironmentVariable("DOROTI_SAMPLE", sample);
        if (Intent?.GetStringExtra("doroti_sample_webview_profile") is { } sampleProfile)
        {
            Environment.SetEnvironmentVariable("DOROTI_SAMPLE_WEBVIEW_PROFILE", sampleProfile);
        }

        base.OnCreate(savedInstanceState);
    }
}
