namespace Doroti.Ui;
public enum PlatformSystemSound { Click, Tick, Alert }
public enum PlatformHaptic { Vibrate, LightImpact, MediumImpact, HeavyImpact, SelectionClick, SuccessNotification, WarningNotification, ErrorNotification }
public enum PlatformFeedbackStatus { Scheduled, Unsupported, Failed }
public sealed record PlatformFeedbackResult(PlatformFeedbackStatus Status, string? Reason = null)
{
    public void RequireScheduled()
    {
        if (Status == PlatformFeedbackStatus.Unsupported) throw new NotSupportedException(Reason ?? "Platform feedback is unsupported.");
        if (Status != PlatformFeedbackStatus.Scheduled) throw new InvalidOperationException(Reason ?? "Platform feedback failed.");
    }
}
public interface IPlatformFeedbackHostCapability
{
    ValueTask<PlatformFeedbackResult> PlaySoundAsync(PlatformSystemSound sound, CancellationToken cancellationToken = default);
    ValueTask<PlatformFeedbackResult> HapticAsync(PlatformHaptic haptic, CancellationToken cancellationToken = default);
}
