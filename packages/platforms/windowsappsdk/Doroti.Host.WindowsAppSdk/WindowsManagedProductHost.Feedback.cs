using System.Runtime.InteropServices;
using Doroti.Ui;
namespace Doroti.Host.WindowsAppSdk;
internal sealed unsafe partial class WindowsManagedProductHost
{
    public ValueTask<PlatformFeedbackResult> PlaySoundAsync(PlatformSystemSound sound, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(sound)) throw new ArgumentOutOfRangeException(nameof(sound));
        if (_disposed) throw new ObjectDisposedException(nameof(WindowsManagedProductHost));
        return ValueTask.FromResult(MessageBeep(sound == PlatformSystemSound.Alert ? 0x30u : 0u)
            ? new PlatformFeedbackResult(PlatformFeedbackStatus.Scheduled)
            : new PlatformFeedbackResult(PlatformFeedbackStatus.Failed, "Windows MessageBeep failed."));
    }
    public ValueTask<PlatformFeedbackResult> HapticAsync(PlatformHaptic haptic, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!Enum.IsDefined(haptic)) throw new ArgumentOutOfRangeException(nameof(haptic));
        return ValueTask.FromResult(new PlatformFeedbackResult(PlatformFeedbackStatus.Unsupported, "This desktop provider does not expose a physical haptic actuator."));
    }
    [DllImport("user32.dll")] private static extern bool MessageBeep(uint type);
}
