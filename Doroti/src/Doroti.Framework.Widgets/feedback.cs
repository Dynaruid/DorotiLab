// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/feedback.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public abstract class Feedback
{
    public static async Future forTap(BuildContext context)
    {
        (context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Framework.Semantics.TapSemanticEvent());
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
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
    }

    public static global::System.Action? wrapForTap(global::System.Action? callback, BuildContext context)
    {
        if ((callback is null))
        {
            return ((global::System.Action?)null);
        }
        return ((global::System.Action)(() =>
        {
            DartRuntimePrimitives.Ignore(forTap(context));
            callback();
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future forLongPress(BuildContext context)
    {
        (context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Framework.Semantics.LongPressSemanticsEvent());
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
                {
                    return ((Future)HapticFeedback.vibrate());
                }
            case TargetPlatform.iOS:
                {
                    return ((Future)DartAsyncRuntime.wait(new List<Future> { SystemSound.play(SystemSoundType.click), HapticFeedback.heavyImpact() }));
                }
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    return Future.value();
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::System.Action? wrapForLongPress(global::System.Action? callback, BuildContext context)
    {
        if ((callback is null))
        {
            return ((global::System.Action?)null);
        }
        return ((global::System.Action)(() =>
        {
            DartRuntimePrimitives.Ignore(forLongPress(context));
            callback();
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

