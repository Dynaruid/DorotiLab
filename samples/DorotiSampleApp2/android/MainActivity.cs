using Android.App;
using Android.Content.PM;
using Microsoft.Maui;

namespace DorotiSampleApp2.Android;

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
public sealed class MainActivity : Doroti.Host.Maui.DorotiMauiActivity
{
    protected override void OnNewIntent(global::Android.Content.Intent? intent)
    {
        base.OnNewIntent(intent);
        if (intent?.GetStringExtra("doroti_recreate_probe") == "1") Recreate();
    }
    protected override void OnCreate(global::Android.OS.Bundle? savedInstanceState)
    {
        // Opt-in automation uses Android's actual launch transport, before the
        // framework constructs the page. Only the existing benchmark selectors
        // are accepted; normal launches retain the interactive page defaults.
        foreach (var key in new[] { "DOROTI_VARIABLE_BLUR_BENCHMARK", "DOROTI_VARIABLE_BLUR_BENCHMARK_SIGMA", "DOROTI_VARIABLE_BLUR_BENCHMARK_STATIC" })
            if (Intent?.GetStringExtra(key) is { } value) Environment.SetEnvironmentVariable(key, value);
        base.OnCreate(savedInstanceState);
    }
}
