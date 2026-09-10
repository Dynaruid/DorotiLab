using Doroti.Hosting;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;

#if ANDROID
// MAUI's haptic implementation checks this permission even for View feedback.
[assembly: Android.App.UsesPermission(Android.Manifest.Permission.Vibrate)]
#endif

namespace Doroti.Host.Maui;

internal static class MauiHapticFeedback
{
    internal static async ValueTask PerformAsync(HapticFeedbackKind kind, IMauiSkiaSurface surface, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            Perform(kind, surface);
        });
    }

    private static void Perform(HapticFeedbackKind kind, IMauiSkiaSurface surface)
    {
        _ = surface;
#if ANDROID
        // Match Flutter PlatformPlugin.java. Do not use timed vibrator pulses:
        // View feedback preserves the user's system haptic setting.
        if (kind is HapticFeedbackKind.standard or HapticFeedbackKind.heavyImpact)
        {
            if (HapticFeedback.Default.IsSupported)
                HapticFeedback.Default.Perform(kind == HapticFeedbackKind.standard
                    ? HapticFeedbackType.LongPress : HapticFeedbackType.Click);
            return;
        }

        Android.Views.FeedbackConstants? feedback = kind switch
        {
            HapticFeedbackKind.lightImpact => Android.Views.FeedbackConstants.VirtualKey,
            HapticFeedbackKind.mediumImpact => Android.Views.FeedbackConstants.KeyboardTap,
            HapticFeedbackKind.selectionClick => Android.Views.FeedbackConstants.ClockTick,
            HapticFeedbackKind.successNotification when OperatingSystem.IsAndroidVersionAtLeast(30) => Android.Views.FeedbackConstants.Confirm,
            HapticFeedbackKind.warningNotification when OperatingSystem.IsAndroidVersionAtLeast(30) => Android.Views.FeedbackConstants.KeyboardTap,
            HapticFeedbackKind.errorNotification when OperatingSystem.IsAndroidVersionAtLeast(30) => Android.Views.FeedbackConstants.Reject,
            _ => null,
        };
        if (feedback is { } value)
            Platform.CurrentActivity?.Window?.DecorView?.PerformHapticFeedback(value);
#elif IOS || MACCATALYST
        // MAUI's default iOS vibration uses the same system sound as Flutter.
        if (kind == HapticFeedbackKind.standard)
        {
            if (Vibration.Default.IsSupported) Vibration.Default.Vibrate();
            return;
        }
        // MAUI exposes only Click/LongPress; retain Flutter's distinct UIKit types.
        if (kind == HapticFeedbackKind.selectionClick)
        {
            using var selection = new UIKit.UISelectionFeedbackGenerator();
            selection.SelectionChanged();
        }
        else if (kind is HapticFeedbackKind.successNotification or HapticFeedbackKind.warningNotification or HapticFeedbackKind.errorNotification)
        {
            using var notification = new UIKit.UINotificationFeedbackGenerator();
            notification.NotificationOccurred(kind switch
            {
                HapticFeedbackKind.successNotification => UIKit.UINotificationFeedbackType.Success,
                HapticFeedbackKind.warningNotification => UIKit.UINotificationFeedbackType.Warning,
                _ => UIKit.UINotificationFeedbackType.Error,
            });
        }
        else
        {
            var style = kind switch
            {
                HapticFeedbackKind.lightImpact => UIKit.UIImpactFeedbackStyle.Light,
                HapticFeedbackKind.mediumImpact => UIKit.UIImpactFeedbackStyle.Medium,
                _ => UIKit.UIImpactFeedbackStyle.Heavy,
            };
            // Modern UIKit requires the real originating view, not an unattached placeholder.
            if (surface.Element.Handler?.PlatformView is not UIKit.UIView nativeView) return;
            using var impact = OperatingSystem.IsIOSVersionAtLeast(17, 5) || OperatingSystem.IsMacCatalystVersionAtLeast(17, 5)
                ? UIKit.UIImpactFeedbackGenerator.GetFeedbackGenerator(style, nativeView)
                : new UIKit.UIImpactFeedbackGenerator(style);
            impact.ImpactOccurred();
        }
#else
        // Flutter desktop hosts do not provide device haptic feedback.
        _ = kind;
#endif
    }
}
