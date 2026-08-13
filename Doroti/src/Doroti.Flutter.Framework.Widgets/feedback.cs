// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/feedback.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public abstract class Feedback
{
    public static async Future forTap(BuildContext context)
    {
        ((dynamic)context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Generated.Framework.Semantics.TapSemanticEvent());
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    await SystemSound.play(global::Doroti.Generated.Framework.Services.SystemSoundType.click);
                    return;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
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
            return ((global::System.Action)(object)null);
        }
        return ((global::System.Action)(() => {
DartRuntimePrimitives.Ignore(Feedback.forTap(context));
callback();
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Future forLongPress(BuildContext context)
    {
        ((dynamic)context.findRenderObject()!).sendSemanticsEvent(new global::Doroti.Generated.Framework.Semantics.LongPressSemanticsEvent());
        switch (global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform)
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    return ((Future)(object?)HapticFeedback.vibrate());
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                {
                    return ((Future)(object?)global::Doroti.Flutter.Runtime.DartAsyncRuntime.wait(new List<Future> { SystemSound.play(global::Doroti.Generated.Framework.Services.SystemSoundType.click), HapticFeedback.heavyImpact() }));
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
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
            return ((global::System.Action)(object)null);
        }
        return ((global::System.Action)(() => {
DartRuntimePrimitives.Ignore(Feedback.forLongPress(context));
callback();
}));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

