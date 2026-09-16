// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/gesture_settings.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public class DeviceGestureSettings
{
    public virtual double? touchSlop { get; private set; }

    public DeviceGestureSettings(double? touchSlop = null)
    {
        this.touchSlop = touchSlop;
    }

    public static DeviceGestureSettings CreateFromView(DorotiView view)
    {
        double? physicalTouchSlopLocal = view.gestureSettings.physicalTouchSlop;
        return new DeviceGestureSettings(touchSlop: (physicalTouchSlopLocal is null) ? null : (DartRuntimePrimitives.RequireValue(physicalTouchSlopLocal) / view.devicePixelRatio));
    }

    public virtual double? panSlop => (touchSlop is not null) ? (DartRuntimePrimitives.RequireValue(touchSlop) * 2L) : null;
    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(touchSlop, 23L);
    public override bool Equals(object? other)
    {
        var __other = other as DeviceGestureSettings;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is DeviceGestureSettings) && (__other.touchSlop == touchSlop);
    }

    public override string ToString() => $"DeviceGestureSettings(touchSlop: {touchSlop})";
}

