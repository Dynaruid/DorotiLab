// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/eager.dart
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

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

