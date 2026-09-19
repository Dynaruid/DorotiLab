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
        if (Intent?.GetStringExtra("doroti_webview_evidence") == "1")
            Environment.SetEnvironmentVariable("DOROTI_ANDROID_WEBVIEW_EVIDENCE", System.IO.Path.Combine(EvidenceDirectory(), "webview-evidence.txt"));
        if (Intent?.GetStringExtra("doroti_effect_calibration") == "1")
            Environment.SetEnvironmentVariable("DOROTI_ANDROID_EFFECT_CALIBRATION", System.IO.Path.Combine(EvidenceDirectory(), "effect-calibration"));
        if (Intent?.GetStringExtra("doroti_webview_count") is { } count)
            Environment.SetEnvironmentVariable("DOROTI_WEBVIEW_COUNT", count);
        if (Intent?.GetStringExtra("doroti_webview_workload") is { } workload)
            Environment.SetEnvironmentVariable("DOROTI_WEBVIEW_WORKLOAD", workload);
        base.OnCreate(savedInstanceState);
    }
    private string EvidenceDirectory() => GetExternalFilesDir(null)?.AbsolutePath ?? FilesDir?.AbsolutePath
        ?? throw new InvalidOperationException("Android application storage unavailable.");
}
