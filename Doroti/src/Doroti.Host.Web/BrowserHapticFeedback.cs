using Doroti.Hosting;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
internal static class BrowserHapticFeedback
{
    internal static async ValueTask PerformAsync(HapticFeedbackKind kind, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Flutter web PlatformDispatcher._getHapticFeedbackDuration, in milliseconds.
        var duration = kind switch
        {
            HapticFeedbackKind.lightImpact or HapticFeedbackKind.selectionClick => 10,
            HapticFeedbackKind.mediumImpact or HapticFeedbackKind.successNotification or HapticFeedbackKind.warningNotification => 20,
            HapticFeedbackKind.heavyImpact or HapticFeedbackKind.errorNotification => 30,
            _ => 50,
        };
        await BrowserInterop.VibrateAsync(duration);
    }
}
