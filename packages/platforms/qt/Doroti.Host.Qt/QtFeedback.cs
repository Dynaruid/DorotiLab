using Doroti.Ui;
namespace Doroti.Host.Qt;
internal sealed class QtFeedback : IPlatformFeedbackHostCapability
{
    public ValueTask<PlatformFeedbackResult> PlaySoundAsync(PlatformSystemSound sound, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(sound)) throw new ArgumentOutOfRangeException(nameof(sound));
        return ValueTask.FromResult(new PlatformFeedbackResult(PlatformFeedbackStatus.Unsupported, "Qt system sounds are not implemented by this provider."));
    }
    public ValueTask<PlatformFeedbackResult> HapticAsync(PlatformHaptic haptic, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(haptic)) throw new ArgumentOutOfRangeException(nameof(haptic));
        return ValueTask.FromResult(new PlatformFeedbackResult(PlatformFeedbackStatus.Unsupported, "Qt desktop has no device haptic feedback."));
    }
}
