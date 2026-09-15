// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/services/haptic_feedback.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Services;

public abstract class HapticFeedback
{
    public static async Future vibrate()
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate");
    }

    public static async Future lightImpact()
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate", "HapticFeedbackType.lightImpact");
    }

    public static async Future mediumImpact()
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate", "HapticFeedbackType.mediumImpact");
    }

    public static async Future heavyImpact()
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate", "HapticFeedbackType.heavyImpact");
    }

    public static async Future selectionClick()
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate", "HapticFeedbackType.selectionClick");
    }

    public static async Future successNotification()
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate", "HapticFeedbackType.successNotification");
    }

    public static async Future warningNotification()
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate", "HapticFeedbackType.warningNotification");
    }

    public static async Future errorNotification()
    {
        await SystemChannels.platform.invokeMethod<object?>("HapticFeedback.vibrate", "HapticFeedbackType.errorNotification");
    }

}

