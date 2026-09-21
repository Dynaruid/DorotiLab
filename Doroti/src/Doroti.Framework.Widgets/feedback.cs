// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/feedback.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class Feedback
{
    public static async Future forTap(BuildContext context)
    {
        context.findRenderObject()!.sendSemanticsEvent(new TapSemanticEvent());
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                await SystemSound.play(SystemSoundType.click);
                return;
            }
            case TargetPlatform.iOS:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                await Future.value();
                return;
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
    }

    public static Action? wrapForTap(Action? callback, BuildContext context)
    {
        if (callback is null)
        {
            return null;
        }
        return () =>
        {
            DartRuntimePrimitives.Ignore(forTap(context));
            callback();
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Future forLongPress(BuildContext context)
    {
        context.findRenderObject()!.sendSemanticsEvent(new LongPressSemanticsEvent());
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            {
                return HapticFeedback.vibrate();
            }
            case TargetPlatform.iOS:
            {
                return DartAsyncRuntime.wait(
                    new List<Future>
                    {
                        SystemSound.play(SystemSoundType.click),
                        HapticFeedback.heavyImpact(),
                    }
                );
            }
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                return Future.value();
            }
            default:
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static Action? wrapForLongPress(Action? callback, BuildContext context)
    {
        if (callback is null)
        {
            return null;
        }
        return () =>
        {
            DartRuntimePrimitives.Ignore(forLongPress(context));
            callback();
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
