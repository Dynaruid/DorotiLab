// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/system_sound.dart
using Doroti.Runtime;

namespace Doroti.Framework.Services;

public enum SystemSoundType
{
    click,
    tick,
    alert
}

public abstract class SystemSound
{
    public static async Future play(SystemSoundType type)
    {
        await SystemChannels.platform.invokeMethod<object?>("SystemSound.play", $"SystemSoundType.{type}");
    }

}
