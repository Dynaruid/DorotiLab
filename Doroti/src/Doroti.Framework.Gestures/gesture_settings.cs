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
        return new DeviceGestureSettings(touchSlop: ((physicalTouchSlopLocal is null) ? null : (DartRuntimePrimitives.RequireValue(physicalTouchSlopLocal) / view.devicePixelRatio)));
    }

    public virtual double? panSlop => ((this.touchSlop is not null) ? ((DartRuntimePrimitives.RequireValue(this.touchSlop) * 2L)) : null);
    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.touchSlop, 23L);
    public override bool Equals(object? other)
    {
        var __other = other as DeviceGestureSettings;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is DeviceGestureSettings) && (((DeviceGestureSettings)((DeviceGestureSettings)__other)).touchSlop == this.touchSlop));
    }

    public override string ToString() => $"DeviceGestureSettings(touchSlop: {this.touchSlop})";
}

