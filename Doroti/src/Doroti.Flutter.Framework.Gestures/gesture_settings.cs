// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/gesture_settings.dart
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

public class DeviceGestureSettings
{
    public virtual double? touchSlop { get; private set; }

    public DeviceGestureSettings(double? touchSlop = null)
    {
        this.touchSlop = touchSlop;
    }

    public static DeviceGestureSettings CreateFromView(FlutterView view)
    {
        double? physicalTouchSlop__936 = view.gestureSettings.physicalTouchSlop;
        return new DeviceGestureSettings(touchSlop: ((physicalTouchSlop__936 is null) ? null : (DartRuntimePrimitives.RequireValue(physicalTouchSlop__936) / view.devicePixelRatio)));
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

