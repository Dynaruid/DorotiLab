#if MACOS
using AppKit;
using Foundation;
using Doroti.Ui;

namespace Doroti.Host.Maui;

internal sealed partial class MauiViewEnvironment
{
    partial void AttachNative()
    {
        if (_element.Handler?.PlatformView is not NSView native) return;
        foreach (var name in new[] { "NSWindowDidResizeNotification", "NSWindowDidChangeScreenNotification",
            "NSWindowDidChangeBackingPropertiesNotification", "NSWindowDidEnterFullScreenNotification",
            "NSWindowDidExitFullScreenNotification", "NSApplicationDidBecomeActiveNotification",
            "NSCurrentLocaleDidChangeNotification" })
        {
            var token = NSNotificationCenter.DefaultCenter.AddObserver(new NSString(name), _ => Refresh());
            _detach.Add(() => { NSNotificationCenter.DefaultCenter.RemoveObserver(token); token.Dispose(); });
        }
        var voiceOver = NSWorkspace.SharedWorkspace.AddObserver("voiceOverEnabled", NSKeyValueObservingOptions.New, _ => Refresh());
        _detach.Add(voiceOver.Dispose);
        var workspace = NSWorkspace.SharedWorkspace.NotificationCenter;
        var accessibility = workspace.AddObserver(new NSString("NSWorkspaceAccessibilityDisplayOptionsDidChangeNotification"), _ => Refresh());
        _detach.Add(() => { workspace.RemoveObserver(accessibility); accessibility.Dispose(); });
    }
    partial void CaptureNative()
    {
        if (_element.Handler?.PlatformView is not NSView native || native.Window is not { } window) return;
        NativePhysicalSize = new(Math.Round(native.Bounds.Width * window.BackingScaleFactor), Math.Round(native.Bounds.Height * window.BackingScaleFactor));
        var safe = native.SafeAreaInsets;
        // NSEdgeInsets uses named top/bottom independent of NSView's flipped drawing coordinates.
        Padding = ViewOcclusion.Scale(new(safe.Left, safe.Top, safe.Right, safe.Bottom), window.BackingScaleFactor);
        Insets = ViewPadding.zero;
        var workspace = NSWorkspace.SharedWorkspace;
        Accessibility = new(workspace.VoiceOverEnabled, workspace.AccessibilityDisplayShouldInvertColors, false, false,
            workspace.AccessibilityDisplayShouldIncreaseContrast, false, false,
            workspace.AccessibilityDisplayShouldReduceMotion);
        Locales = NSLocale.PreferredLanguages.Select(ParseLocale).ToArray();
        Use24Hour = NSDateFormatter.GetDateFormatFromTemplate("j", 0, NSLocale.CurrentLocale)?.Contains('a') == false;
    }
}
#endif
