// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/haptic_feedback.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Services;

public abstract class HapticFeedback
{
    public static Future vibrate() => InvokeAsync(PlatformHaptic.Vibrate);
    public static Future lightImpact() => InvokeAsync(PlatformHaptic.LightImpact);
    public static Future mediumImpact() => InvokeAsync(PlatformHaptic.MediumImpact);
    public static Future heavyImpact() => InvokeAsync(PlatformHaptic.HeavyImpact);
    public static Future selectionClick() => InvokeAsync(PlatformHaptic.SelectionClick);
    public static Future successNotification() => InvokeAsync(PlatformHaptic.SuccessNotification);
    public static Future warningNotification() => InvokeAsync(PlatformHaptic.WarningNotification);
    public static Future errorNotification() => InvokeAsync(PlatformHaptic.ErrorNotification);
    private static async Future InvokeAsync(PlatformHaptic haptic)
    {
        var invocation = DorotiUiInvocation.Managed("HapticFeedback." + haptic);
        var view = PlatformDispatcher.instance.RequireInvocationView(invocation);
        var result = await view.InvokeCapabilityAsync<IPlatformFeedbackHostCapability, PlatformFeedbackResult>(DorotiCapabilityIds.PlatformFeedback, invocation,
            (host, token) => host.HapticAsync(haptic, token));
        result.RequireScheduled();
    }
}
