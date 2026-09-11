using Doroti.Hosting;
using Microsoft.Maui.ApplicationModel;

namespace Doroti.Host.Maui;

internal static class MauiSystemSound
{
    internal static async ValueTask PlayAsync(SystemSoundKind kind, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
#if ANDROID
            // Flutter PlatformPlugin.java supports click only. View playback respects
            // the user's touch-sound setting; never replace this with a vibration.
            if (kind == SystemSoundKind.click)
                Platform.CurrentActivity?.Window?.DecorView?.PlaySoundEffect(Android.Views.SoundEffects.Click);
#else
            // Preserve the existing no-op on other MAUI hosts.
            _ = kind;
#endif
        });
    }
}
