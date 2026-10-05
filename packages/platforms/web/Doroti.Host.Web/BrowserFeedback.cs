using Doroti.Ui;
namespace Doroti.Host.Web;
[System.Runtime.Versioning.SupportedOSPlatform("browser")]
internal sealed class BrowserFeedback : IPlatformFeedbackHostCapability
{
    public ValueTask<PlatformFeedbackResult> PlaySoundAsync(PlatformSystemSound sound, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(sound)) throw new ArgumentOutOfRangeException(nameof(sound));
        return ValueTask.FromResult(new PlatformFeedbackResult(PlatformFeedbackStatus.Unsupported, "The browser provider has no system sound API."));
    }
    public async ValueTask<PlatformFeedbackResult> HapticAsync(PlatformHaptic haptic, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(haptic)) throw new ArgumentOutOfRangeException(nameof(haptic));
        var duration = haptic switch {
            PlatformHaptic.LightImpact or PlatformHaptic.SelectionClick => 10,
            PlatformHaptic.MediumImpact or PlatformHaptic.SuccessNotification or PlatformHaptic.WarningNotification => 20,
            PlatformHaptic.HeavyImpact or PlatformHaptic.ErrorNotification => 30, _ => 50 };
        var accepted = await BrowserInterop.VibrateAsync(duration);
        cancellationToken.ThrowIfCancellationRequested();
        return new(accepted ? PlatformFeedbackStatus.Scheduled : PlatformFeedbackStatus.Unsupported,
            accepted ? null : "The browser rejected vibration or does not expose the API.");
    }
}
