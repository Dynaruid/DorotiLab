using Doroti.Ui;
using Doroti.Hosting;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
namespace Doroti.Host.Maui;
internal sealed class MauiFeedback(IMauiSkiaSurface surface) : IPlatformFeedbackHostCapability
{
    public async ValueTask<PlatformFeedbackResult> PlaySoundAsync(PlatformSystemSound sound, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(sound)) throw new ArgumentOutOfRangeException(nameof(sound));
#if ANDROID
        if (sound == PlatformSystemSound.Click)
            return await MainThread.InvokeOnMainThreadAsync(() => {
                cancellationToken.ThrowIfCancellationRequested();
                var view = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window?.DecorView;
                if (view is null) return Unsupported("The originating Android activity is unavailable.");
                view.PlaySoundEffect(Android.Views.SoundEffects.Click);
                return new PlatformFeedbackResult(PlatformFeedbackStatus.Scheduled);
            });
#endif
        await Task.CompletedTask;
        return Unsupported("This platform does not implement the requested system sound.");
    }
    public async ValueTask<PlatformFeedbackResult> HapticAsync(PlatformHaptic haptic, CancellationToken cancellationToken = default)
    {
        _ = surface;
        cancellationToken.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(haptic)) throw new ArgumentOutOfRangeException(nameof(haptic));
#if ANDROID
        return await MainThread.InvokeOnMainThreadAsync(() => {
            cancellationToken.ThrowIfCancellationRequested();
            var view = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.Window?.DecorView;
            if (view is null) return Unsupported("The originating Android activity is unavailable.");
            Android.Views.FeedbackConstants? feedback = haptic switch {
                PlatformHaptic.Vibrate => Android.Views.FeedbackConstants.LongPress,
                PlatformHaptic.HeavyImpact => Android.Views.FeedbackConstants.LongPress,
                PlatformHaptic.LightImpact => Android.Views.FeedbackConstants.VirtualKey,
                PlatformHaptic.MediumImpact => Android.Views.FeedbackConstants.KeyboardTap,
                PlatformHaptic.SelectionClick => Android.Views.FeedbackConstants.ClockTick,
                PlatformHaptic.SuccessNotification when OperatingSystem.IsAndroidVersionAtLeast(30) => Android.Views.FeedbackConstants.Confirm,
                PlatformHaptic.WarningNotification when OperatingSystem.IsAndroidVersionAtLeast(30) => Android.Views.FeedbackConstants.KeyboardTap,
                PlatformHaptic.ErrorNotification when OperatingSystem.IsAndroidVersionAtLeast(30) => Android.Views.FeedbackConstants.Reject, _ => null };
            if (feedback is null) return Unsupported("The Android API level does not support this feedback.");
            return view.PerformHapticFeedback(feedback.Value)
                ? new PlatformFeedbackResult(PlatformFeedbackStatus.Scheduled)
                : Unsupported("Android rejected feedback because of device support or user settings.");
        });
#elif IOS || MACCATALYST
        return await MainThread.InvokeOnMainThreadAsync(async () => {
            cancellationToken.ThrowIfCancellationRequested();
            if (surface.Element.Handler?.PlatformView is not UIKit.UIView)
                return Unsupported("The originating UIKit view is unavailable.");
            if (haptic == PlatformHaptic.Vibrate && !Vibration.Default.IsSupported)
                return Unsupported("This device does not support vibration.");
            await MauiHapticFeedback.PerformAsync((HapticFeedbackKind)(int)haptic, surface, cancellationToken);
            return new PlatformFeedbackResult(PlatformFeedbackStatus.Scheduled);
        });
#else
        await Task.CompletedTask;
        return Unsupported("This desktop platform has no device haptic feedback.");
#endif
    }
    private static PlatformFeedbackResult Unsupported(string reason) => new(PlatformFeedbackStatus.Unsupported, reason);
}
