// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/eager.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Gestures;

public class EagerGestureRecognizer : OneSequenceGestureRecognizer
{
    public EagerGestureRecognizer(HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!) : base(supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
    {
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        resolve(GestureDisposition.accepted);
        stopTrackingPointer(@event.pointer);
    }

    public override string debugDescription => "eager";
    public override void didStopTrackingLastPointer(long pointer)
    {
    }

    public override void handleEvent(PointerEvent @event)
    {
    }

}

