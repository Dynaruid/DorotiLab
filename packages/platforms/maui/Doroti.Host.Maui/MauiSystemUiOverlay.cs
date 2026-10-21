#if ANDROID || IOS
using Doroti.Hosting;
using Doroti.Ui;
using Microsoft.Maui.ApplicationModel;

namespace Doroti.Host.Maui;

/// <summary>Keeps the system bar style with the surface across native handler/window replacement.</summary>
internal sealed class MauiSystemUiOverlay : IDisposable
{
    private readonly Microsoft.Maui.Controls.View _element;
    private SystemUiOverlaySettings _settings = new();
    private bool _hasSettings;
    private bool _disposed;

    internal MauiSystemUiOverlay(Microsoft.Maui.Controls.View element)
    {
        _element = element;
        _element.Loaded += Reapply;
        _element.HandlerChanged += Reapply;
    }

    internal async ValueTask ApplyAsync(SystemUiOverlaySettings settings, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (_disposed) return;
            _settings = _settings.Merge(settings);
            _hasSettings = true;
            Apply();
        });
    }

    private void Reapply(object? sender, EventArgs args)
    {
        if (!_disposed && _hasSettings) Apply();
    }

    private void Apply()
    {
#if ANDROID
        // Resolve the surface's own current window; CurrentActivity can belong to another view.
        Android.Content.Context? context = _element.Window?.Handler?.PlatformView as Android.App.Activity
            ?? (_element.Handler?.PlatformView as Android.Views.View)?.Context;
        while (context is Android.Content.ContextWrapper wrapper && context is not Android.App.Activity)
            context = wrapper.BaseContext;
        if (context is not Android.App.Activity { IsDestroyed: false, Window: { } window }) return;
        var decor = window.DecorView;
        using var controller = new AndroidX.Core.View.WindowInsetsControllerCompat(window, decor);
        if (!OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            window.AddFlags(Android.Views.WindowManagerFlags.DrawsSystemBarBackgrounds);
            window.ClearFlags(Android.Views.WindowManagerFlags.TranslucentStatus
                | Android.Views.WindowManagerFlags.TranslucentNavigation);
        }
        if (_settings.StatusBarIconBrightness is { } statusIcons)
            controller.AppearanceLightStatusBars = statusIcons == Brightness.dark;
        if (OperatingSystem.IsAndroidVersionAtLeast(26)
            && _settings.SystemNavigationBarIconBrightness is { } navigationIcons)
            controller.AppearanceLightNavigationBars = navigationIcons == Brightness.dark;

        // Android 15+ owns edge-to-edge bar backgrounds. Paint behind them in the widget tree.
        if (!OperatingSystem.IsAndroidVersionAtLeast(35))
        {
            if (_settings.StatusBarColor is { } statusColor)
                window.SetStatusBarColor(new Android.Graphics.Color(unchecked((int)statusColor)));
            if (OperatingSystem.IsAndroidVersionAtLeast(26) && _settings.SystemNavigationBarColor is { } navigationColor)
                window.SetNavigationBarColor(new Android.Graphics.Color(unchecked((int)navigationColor)));
            if (OperatingSystem.IsAndroidVersionAtLeast(28) && _settings.SystemNavigationBarDividerColor is { } divider)
                window.NavigationBarDividerColor = new Android.Graphics.Color(unchecked((int)divider));
        }
        if (OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(35) && _settings.SystemStatusBarContrastEnforced is { } statusContrast)
                window.StatusBarContrastEnforced = statusContrast;
            if (_settings.SystemNavigationBarContrastEnforced is { } navigationContrast)
                window.NavigationBarContrastEnforced = navigationContrast;
        }
#elif IOS
        // iOS interprets statusBarBrightness as the background brightness, the inverse of icon brightness.
        if (_settings.StatusBarBrightness is not { } brightness) return;
        for (Microsoft.Maui.Controls.Element? parent = _element; parent is not null; parent = parent.Parent)
        {
            if (parent is DorotiSystemUiPage page)
            {
                page.StatusBarBrightness = brightness;
                (page.Handler as Microsoft.Maui.IPlatformViewHandler)?.ViewController?.SetNeedsStatusBarAppearanceUpdate();
                break;
            }
        }
#endif
    }

    public void Dispose()
    {
        _disposed = true;
        _element.Loaded -= Reapply;
        _element.HandlerChanged -= Reapply;
    }
}
#endif
