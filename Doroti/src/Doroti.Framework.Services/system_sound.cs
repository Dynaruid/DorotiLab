// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/system_sound.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Services;

public enum SystemSoundType
{
    click,
    tick,
    alert,
}

public abstract class SystemSound
{
    public static async Future play(SystemSoundType type)
    {
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        var invocation = DorotiUiInvocation.Managed("SystemSound.play");
        var view = PlatformDispatcher.instance.RequireInvocationView(invocation);
        var result = await view.InvokeCapabilityAsync<IPlatformFeedbackHostCapability, PlatformFeedbackResult>(DorotiCapabilityIds.PlatformFeedback, invocation,
            (host, token) => host.PlaySoundAsync((PlatformSystemSound)(int)type, token));
        result.RequireScheduled();
    }
}
