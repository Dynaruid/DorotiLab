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
        if (Intent?.GetStringExtra("doroti_testbed_mode") is { } mode)
            Environment.SetEnvironmentVariable("DOROTI_TESTBED_MODE", mode);
        if (Intent?.GetStringExtra("doroti_platform_view_composition") is { } composition)
            Environment.SetEnvironmentVariable("DOROTI_PLATFORM_VIEW_COMPOSITION", composition);
        if (Intent?.GetStringExtra("doroti_platform_view_raster_mode") is { } rasterMode)
            Environment.SetEnvironmentVariable("DOROTI_ANDROID_PLATFORM_RASTER_MODE", rasterMode);
        if (Intent?.GetStringExtra("doroti_platform_view_backdrop") is { } backdrop)
            Environment.SetEnvironmentVariable("DOROTI_PLATFORM_VIEW_BACKDROP", backdrop);
        base.OnCreate(savedInstanceState);
    }
}
