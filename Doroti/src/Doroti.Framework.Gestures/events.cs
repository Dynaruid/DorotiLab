// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/events.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Gestures;

public static partial class EventsLibrary
{
    public static long kPrimaryButton = 1L;
}
public static partial class EventsLibrary
{
    public static long kSecondaryButton = 2L;
}

public static partial class EventsLibrary
{
    public static long kPrimaryMouseButton = EventsLibrary.kPrimaryButton;
}

public static partial class EventsLibrary
{
    public static long kSecondaryMouseButton = EventsLibrary.kSecondaryButton;
}

public static partial class EventsLibrary
{
    public static long kStylusContact = EventsLibrary.kPrimaryButton;
}

public static partial class EventsLibrary
{
    public static long kPrimaryStylusButton = EventsLibrary.kSecondaryButton;
}

public static partial class EventsLibrary
{
    public static long kTertiaryButton = 4L;
}

public static partial class EventsLibrary
{
    public static long kMiddleMouseButton = EventsLibrary.kTertiaryButton;
}

public static partial class EventsLibrary
{
    public static long kSecondaryStylusButton = EventsLibrary.kTertiaryButton;
}

public static partial class EventsLibrary
{
    public static long kBackMouseButton = 8L;
}

public static partial class EventsLibrary
{
    public static long kForwardMouseButton = 16L;
}

public static partial class EventsLibrary
{
    public static long kTouchContact = EventsLibrary.kPrimaryButton;
}

public static partial class EventsLibrary
{
    public static long nthMouseButton(long number) => (((EventsLibrary.kPrimaryMouseButton << (int)(((number - 1L))))) & global::Doroti.Framework.Foundation.BitfieldLibrary.kMaxUnsignedSMI);
}

public static partial class EventsLibrary
{
    public static long nthStylusButton(long number) => (((EventsLibrary.kPrimaryStylusButton << (int)(((number - 1L))))) & global::Doroti.Framework.Foundation.BitfieldLibrary.kMaxUnsignedSMI);
}

public static partial class EventsLibrary
{
    public static long smallestButton(long buttons) => (buttons & (-buttons));
}

public static partial class EventsLibrary
{
    public static bool isSingleButton(long buttons) => ((buttons != 0L) && ((EventsLibrary.smallestButton(buttons) == buttons)));
}

public abstract class PointerEvent : global::Doroti.Runtime.IPointerEvent, Diagnosticable
{
    public virtual long viewId { get; private set; } = default!;
    public virtual long embedderId { get; private set; } = default!;
    public virtual Duration timeStamp { get; private set; } = default!;
    public virtual long pointer { get; private set; } = default!;
    public virtual PointerDeviceKind kind { get; private set; } = default!;
    public virtual long device { get; private set; } = default!;
    public virtual Offset position { get; private set; } = default!;
    public virtual Offset delta { get; private set; } = default!;
    public virtual long buttons { get; private set; } = default!;
    public virtual bool down { get; private set; } = default!;
    public virtual bool obscured { get; private set; } = default!;
    public virtual double pressure { get; private set; } = default!;
    public virtual double pressureMin { get; private set; } = default!;
    public virtual double pressureMax { get; private set; } = default!;
    public virtual double distance { get; private set; } = default!;
    public virtual double distanceMax { get; private set; } = default!;
    public virtual double size { get; private set; } = default!;
    public virtual double radiusMajor { get; private set; } = default!;
    public virtual double radiusMinor { get; private set; } = default!;
    public virtual double radiusMin { get; private set; } = default!;
    public virtual double radiusMax { get; private set; } = default!;
    public virtual double orientation { get; private set; } = default!;
    public virtual double tilt { get; private set; } = default!;
    public virtual long platformData { get; private set; } = default!;
    public virtual bool synthesized { get; private set; } = default!;
    public virtual Matrix4? transform { get; private set; }
    public virtual PointerEvent? original { get; private set; }
    public PointerEvent() { }


    protected PointerEvent(long viewId = 0, long embedderId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.touch, long device = 0, Offset position = default, Offset delta = default, long buttons = 0, bool down = false, bool obscured = false, double pressure = 1.0, double pressureMin = 1.0, double pressureMax = 1.0, double distance = 0.0, double distanceMax = 0.0, double size = 0.0, double radiusMajor = 0.0, double radiusMinor = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, long platformData = 0, bool synthesized = false, Matrix4? transform = null, PointerEvent? original = null)
    {
        this.viewId = viewId;
        this.embedderId = embedderId;
        this.timeStamp = timeStamp;
        this.pointer = pointer;
        this.kind = kind;
        this.device = device;
        this.position = position;
        this.delta = delta;
        this.buttons = buttons;
        this.down = down;
        this.obscured = obscured;
        this.pressure = pressure;
        this.pressureMin = pressureMin;
        this.pressureMax = pressureMax;
        this.distance = distance;
        this.distanceMax = distanceMax;
        this.size = size;
        this.radiusMajor = radiusMajor;
        this.radiusMinor = radiusMinor;
        this.radiusMin = radiusMin;
        this.radiusMax = radiusMax;
        this.orientation = orientation;
        this.tilt = tilt;
        this.platformData = platformData;
        this.synthesized = synthesized;
        this.transform = transform;
        this.original = original;
    }

    public virtual global::Doroti.Ui.Offset localPosition => this.position;
    public virtual global::Doroti.Ui.Offset localDelta => this.delta;
    public virtual double distanceMin => 0.0;
    public abstract PointerEvent transformed(Matrix4? transform);
    public abstract PointerEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
    public static global::Doroti.Ui.Offset transformPosition(Matrix4? transform, Offset position)
    {
        if ((transform is null))
        {
            return DartRuntimePrimitives.RequireValue(position);
        }
        var position3 = new Vector3(DartRuntimePrimitives.RequireValue(position).dx, DartRuntimePrimitives.RequireValue(position).dy, 0.0);
        Vector3 transformed3 = transform.perspectiveTransform(position3);
        return new global::Doroti.Ui.Offset(transformed3.x, transformed3.y);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Ui.Offset transformDeltaViaPositions(Offset untransformedEndPosition, Offset? transformedEndPosition = null, Offset untransformedDelta = default!, Matrix4? transform = default!)
    {
        if ((transform is null))
        {
            return untransformedDelta;
        }
        transformedEndPosition ??= transformPosition(transform, untransformedEndPosition);
        global::Doroti.Ui.Offset transformedStartPosition = transformPosition(transform, (untransformedEndPosition - untransformedDelta));
        return (DartRuntimePrimitives.RequireValue(transformedEndPosition) - transformedStartPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static Matrix4 removePerspectiveTransform(Matrix4 transform)
    {
        var vector = new global::System.Numerics.Vector4(checked((float)0), checked((float)0), checked((float)1), checked((float)0));
        return ((Func<Matrix4>)(() =>
{
    var __cascade = transform.clone();
    __cascade.setColumn(2L, vector);
    __cascade.setRow(2L, vector);
    return __cascade;
}))();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null) =>
        ((Diagnosticable)this).toDiagnosticsNode(name, style);
    public virtual string toStringShort() => ((Diagnosticable)this).toStringShort();
}

public interface _PointerEventDescription__events
{
    public void debugFillProperties(DiagnosticPropertiesBuilder properties);
    public string toStringFull();
}

internal abstract class _AbstractPointerEvent__events : PointerEvent
{
}

internal abstract class _TransformedPointerEvent__events : _AbstractPointerEvent__events, Diagnosticable, _PointerEventDescription__events
{
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    public abstract override PointerEvent? original { get; }
    public abstract override Matrix4? transform { get; }
    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public interface _CopyPointerAddedEvent__events
{
    public PointerAddedEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerAddedEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerAddedEvent__events
{
    public PointerAddedEvent() { }


    public PointerAddedEvent(long viewId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.touch, long device = 0, Offset position = default, bool obscured = false, double pressureMin = 1.0, double pressureMax = 1.0, double distance = 0.0, double distanceMax = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, pointer: pointer, kind: kind, device: device, position: position, obscured: obscured, pressureMin: pressureMin, pressureMax: pressureMax, distance: distance, distanceMax: distanceMax, radiusMin: radiusMin, radiusMax: radiusMax, orientation: orientation, tilt: tilt, embedderId: embedderId, pressure: 0.0)
    {
    }

    public override PointerAddedEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerAddedEvent__events((((PointerAddedEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerAddedEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerAddedEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerAddedEvent__events : PointerAddedEvent, _CopyPointerAddedEvent__events
{
    private PointerAddedEvent __field_original = default!;
    public override PointerAddedEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerAddedEvent__events(PointerAddedEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerAddedEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerAddedEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerAddedEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerRemovedEvent__events
{
    public PointerRemovedEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerRemovedEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerRemovedEvent__events
{
    public PointerRemovedEvent() { }


    public PointerRemovedEvent(long viewId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.touch, long device = 0, Offset position = default, bool obscured = false, double pressureMin = 1.0, double pressureMax = 1.0, double distanceMax = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, PointerRemovedEvent? original = null, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, pointer: pointer, kind: kind, device: device, position: position, obscured: obscured, pressureMin: pressureMin, pressureMax: pressureMax, distanceMax: distanceMax, radiusMin: radiusMin, radiusMax: radiusMax, original: original, embedderId: embedderId, pressure: 0.0)
    {
    }

    public override PointerRemovedEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerRemovedEvent__events((((PointerRemovedEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerRemovedEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerRemovedEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distanceMax: (distanceMax ?? this.distanceMax), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerRemovedEvent__events : PointerRemovedEvent, _CopyPointerRemovedEvent__events
{
    private PointerRemovedEvent __field_original = default!;
    public override PointerRemovedEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerRemovedEvent__events(PointerRemovedEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerRemovedEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerRemovedEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerRemovedEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distanceMax: (distanceMax ?? this.distanceMax), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerHoverEvent__events
{
    public PointerHoverEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerHoverEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerHoverEvent__events
{
    public PointerHoverEvent() { }


    public PointerHoverEvent(long viewId = 0, Duration timeStamp = default, PointerDeviceKind kind = PointerDeviceKind.touch, long pointer = 0, long device = 0, Offset position = default, Offset delta = default, long buttons = 0, bool obscured = false, double pressureMin = 1.0, double pressureMax = 1.0, double distance = 0.0, double distanceMax = 0.0, double size = 0.0, double radiusMajor = 0.0, double radiusMinor = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, bool synthesized = false, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, kind: kind, pointer: pointer, device: device, position: position, delta: delta, buttons: buttons, obscured: obscured, pressureMin: pressureMin, pressureMax: pressureMax, distance: distance, distanceMax: distanceMax, size: size, radiusMajor: radiusMajor, radiusMinor: radiusMinor, radiusMin: radiusMin, radiusMax: radiusMax, orientation: orientation, tilt: tilt, synthesized: synthesized, embedderId: embedderId, down: false, pressure: 0.0)
    {
    }

    public override PointerHoverEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerHoverEvent__events((((PointerHoverEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerHoverEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerHoverEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), delta: (delta ?? this.delta), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), synthesized: (synthesized ?? this.synthesized), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerHoverEvent__events : PointerHoverEvent, _CopyPointerHoverEvent__events
{
    private PointerHoverEvent __field_original = default!;
    public override PointerHoverEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerHoverEvent__events(PointerHoverEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerHoverEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerHoverEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerHoverEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), delta: (delta ?? this.delta), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), synthesized: (synthesized ?? this.synthesized), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerEnterEvent__events
{
    public PointerEnterEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerEnterEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerEnterEvent__events
{
    public PointerEnterEvent() { }


    public PointerEnterEvent(long viewId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.touch, long device = 0, Offset position = default, Offset delta = default, long buttons = 0, bool obscured = false, double pressureMin = 1.0, double pressureMax = 1.0, double distance = 0.0, double distanceMax = 0.0, double size = 0.0, double radiusMajor = 0.0, double radiusMinor = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, bool down = false, bool synthesized = false, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, pointer: pointer, kind: kind, device: device, position: position, delta: delta, buttons: buttons, obscured: obscured, pressureMin: pressureMin, pressureMax: pressureMax, distance: distance, distanceMax: distanceMax, size: size, radiusMajor: radiusMajor, radiusMinor: radiusMinor, radiusMin: radiusMin, radiusMax: radiusMax, orientation: orientation, tilt: tilt, down: down, synthesized: synthesized, embedderId: embedderId, pressure: 0.0)
    {
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad));
    }

    public static PointerEnterEvent CreateFromMouseEvent(PointerEvent @event) => new PointerEnterEvent(viewId: ((PointerEvent)@event).viewId, timeStamp: ((PointerEvent)@event).timeStamp, pointer: ((PointerEvent)@event).pointer, kind: ((PointerEvent)@event).kind, device: ((PointerEvent)@event).device, position: ((PointerEvent)@event).position, delta: ((PointerEvent)@event).delta, buttons: ((PointerEvent)@event).buttons, obscured: ((PointerEvent)@event).obscured, pressureMin: ((PointerEvent)@event).pressureMin, pressureMax: ((PointerEvent)@event).pressureMax, distance: ((PointerEvent)@event).distance, distanceMax: ((PointerEvent)@event).distanceMax, size: ((PointerEvent)@event).size, radiusMajor: ((PointerEvent)@event).radiusMajor, radiusMinor: ((PointerEvent)@event).radiusMinor, radiusMin: ((PointerEvent)@event).radiusMin, radiusMax: ((PointerEvent)@event).radiusMax, orientation: ((PointerEvent)@event).orientation, tilt: ((PointerEvent)@event).tilt, down: ((PointerEvent)@event).down, synthesized: ((PointerEvent)@event).synthesized).transformed(((PointerEvent)@event).transform);

    public override PointerEnterEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerEnterEvent__events((((PointerEnterEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerEnterEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerEnterEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), delta: (delta ?? this.delta), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), synthesized: (synthesized ?? this.synthesized), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static implicit operator global::Doroti.Ui.PointerEnterEvent(PointerEnterEvent value) => new()
    {
        pointer = value.pointer,
        embedderId = value.embedderId,
        platformData = value.platformData,
        timeStamp = value.timeStamp,
        position = value.position,
        kind = value.kind,
        orientation = value.orientation,
        pressure = value.pressure,
        size = value.size,
        radiusMajor = value.radiusMajor,
        radiusMinor = value.radiusMinor,
    };
}

internal class _TransformedPointerEnterEvent__events : PointerEnterEvent, _CopyPointerEnterEvent__events
{
    private PointerEnterEvent __field_original = default!;
    public override PointerEnterEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerEnterEvent__events(PointerEnterEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerEnterEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerEnterEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerEnterEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), delta: (delta ?? this.delta), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), synthesized: (synthesized ?? this.synthesized), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerExitEvent__events
{
    public PointerExitEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerExitEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerExitEvent__events
{
    public PointerExitEvent() { }


    public PointerExitEvent(long viewId = 0, Duration timeStamp = default, PointerDeviceKind kind = PointerDeviceKind.touch, long pointer = 0, long device = 0, Offset position = default, Offset delta = default, long buttons = 0, bool obscured = false, double pressureMin = 1.0, double pressureMax = 1.0, double distance = 0.0, double distanceMax = 0.0, double size = 0.0, double radiusMajor = 0.0, double radiusMinor = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, bool down = false, bool synthesized = false, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, kind: kind, pointer: pointer, device: device, position: position, delta: delta, buttons: buttons, obscured: obscured, pressureMin: pressureMin, pressureMax: pressureMax, distance: distance, distanceMax: distanceMax, size: size, radiusMajor: radiusMajor, radiusMinor: radiusMinor, radiusMin: radiusMin, radiusMax: radiusMax, orientation: orientation, tilt: tilt, down: down, synthesized: synthesized, embedderId: embedderId, pressure: 0.0)
    {
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad));
    }

    public static PointerExitEvent CreateFromMouseEvent(PointerEvent @event) => new PointerExitEvent(viewId: ((PointerEvent)@event).viewId, timeStamp: ((PointerEvent)@event).timeStamp, pointer: ((PointerEvent)@event).pointer, kind: ((PointerEvent)@event).kind, device: ((PointerEvent)@event).device, position: ((PointerEvent)@event).position, delta: ((PointerEvent)@event).delta, buttons: ((PointerEvent)@event).buttons, obscured: ((PointerEvent)@event).obscured, pressureMin: ((PointerEvent)@event).pressureMin, pressureMax: ((PointerEvent)@event).pressureMax, distance: ((PointerEvent)@event).distance, distanceMax: ((PointerEvent)@event).distanceMax, size: ((PointerEvent)@event).size, radiusMajor: ((PointerEvent)@event).radiusMajor, radiusMinor: ((PointerEvent)@event).radiusMinor, radiusMin: ((PointerEvent)@event).radiusMin, radiusMax: ((PointerEvent)@event).radiusMax, orientation: ((PointerEvent)@event).orientation, tilt: ((PointerEvent)@event).tilt, down: ((PointerEvent)@event).down, synthesized: ((PointerEvent)@event).synthesized).transformed(((PointerEvent)@event).transform);

    public override PointerExitEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerExitEvent__events((((PointerExitEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerExitEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerExitEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), delta: (delta ?? this.delta), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), synthesized: (synthesized ?? this.synthesized), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static implicit operator global::Doroti.Ui.PointerExitEvent(PointerExitEvent value) => new()
    {
        pointer = value.pointer,
        embedderId = value.embedderId,
        platformData = value.platformData,
        timeStamp = value.timeStamp,
        position = value.position,
        kind = value.kind,
        orientation = value.orientation,
        pressure = value.pressure,
        size = value.size,
        radiusMajor = value.radiusMajor,
        radiusMinor = value.radiusMinor,
    };
}

internal class _TransformedPointerExitEvent__events : PointerExitEvent, _CopyPointerExitEvent__events
{
    private PointerExitEvent __field_original = default!;
    public override PointerExitEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerExitEvent__events(PointerExitEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerExitEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerExitEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerExitEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), delta: (delta ?? this.delta), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), synthesized: (synthesized ?? this.synthesized), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerDownEvent__events
{
    public PointerDownEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerDownEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerDownEvent__events
{
    public PointerDownEvent() { }


    public PointerDownEvent(long viewId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.touch, long device = 0, Offset position = default, long? buttons = null, bool obscured = false, double pressure = 1.0, double pressureMin = 1.0, double pressureMax = 1.0, double distanceMax = 0.0, double size = 0.0, double radiusMajor = 0.0, double radiusMinor = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, pointer: pointer, kind: kind, device: device, position: position, buttons: buttons ?? EventsLibrary.kPrimaryButton, obscured: obscured, pressure: pressure, pressureMin: pressureMin, pressureMax: pressureMax, distanceMax: distanceMax, size: size, radiusMajor: radiusMajor, radiusMinor: radiusMinor, radiusMin: radiusMin, radiusMax: radiusMax, orientation: orientation, tilt: tilt, embedderId: embedderId, down: true, distance: 0.0)
    {
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad));
    }

    public override PointerDownEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerDownEvent__events((((PointerDownEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", DartRuntimePrimitives.RequireValue(buttons), defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerDownEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerDownEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), pointer: (pointer ?? this.pointer), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressure: (pressure ?? this.pressure), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerDownEvent__events : PointerDownEvent, _CopyPointerDownEvent__events
{
    private PointerDownEvent __field_original = default!;
    public override PointerDownEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerDownEvent__events(PointerDownEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerDownEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerDownEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerDownEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), pointer: (pointer ?? this.pointer), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressure: (pressure ?? this.pressure), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerMoveEvent__events
{
    public PointerMoveEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerMoveEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerMoveEvent__events
{
    public PointerMoveEvent() { }


    public PointerMoveEvent(long viewId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.touch, long device = 0, Offset position = default, Offset delta = default, long? buttons = null, bool obscured = false, double pressure = 1.0, double pressureMin = 1.0, double pressureMax = 1.0, double distanceMax = 0.0, double size = 0.0, double radiusMajor = 0.0, double radiusMinor = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, long platformData = 0, bool synthesized = false, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, pointer: pointer, kind: kind, device: device, position: position, delta: delta, buttons: buttons ?? EventsLibrary.kPrimaryButton, obscured: obscured, pressure: pressure, pressureMin: pressureMin, pressureMax: pressureMax, distanceMax: distanceMax, size: size, radiusMajor: radiusMajor, radiusMinor: radiusMinor, radiusMin: radiusMin, radiusMax: radiusMax, orientation: orientation, tilt: tilt, platformData: platformData, synthesized: synthesized, embedderId: embedderId, down: true, distance: 0.0)
    {
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad));
    }

    public override PointerMoveEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerMoveEvent__events((((PointerMoveEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", DartRuntimePrimitives.RequireValue(buttons), defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerMoveEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerMoveEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), pointer: (pointer ?? this.pointer), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), delta: (delta ?? this.delta), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressure: (pressure ?? this.pressure), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), synthesized: (synthesized ?? this.synthesized), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerMoveEvent__events : PointerMoveEvent, _CopyPointerMoveEvent__events
{
    private PointerMoveEvent __field_original = default!;
    public override PointerMoveEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerMoveEvent__events(PointerMoveEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerMoveEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerMoveEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerMoveEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), pointer: (pointer ?? this.pointer), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), delta: (delta ?? this.delta), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressure: (pressure ?? this.pressure), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), synthesized: (synthesized ?? this.synthesized), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerUpEvent__events
{
    public PointerUpEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerUpEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerUpEvent__events
{
    public PointerUpEvent() { }


    public PointerUpEvent(long viewId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.touch, long device = 0, Offset position = default, long buttons = 0, bool obscured = false, double pressure = 0.0, double pressureMin = 1.0, double pressureMax = 1.0, double distance = 0.0, double distanceMax = 0.0, double size = 0.0, double radiusMajor = 0.0, double radiusMinor = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, pointer: pointer, kind: kind, device: device, position: position, buttons: buttons, obscured: obscured, pressure: pressure, pressureMin: pressureMin, pressureMax: pressureMax, distance: distance, distanceMax: distanceMax, size: size, radiusMajor: radiusMajor, radiusMinor: radiusMinor, radiusMin: radiusMin, radiusMax: radiusMax, orientation: orientation, tilt: tilt, embedderId: embedderId, down: false)
    {
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad));
    }

    public override PointerUpEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerUpEvent__events((((PointerUpEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerUpEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerUpEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), pointer: (pointer ?? this.pointer), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressure: (pressure ?? this.pressure), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerUpEvent__events : PointerUpEvent, _CopyPointerUpEvent__events
{
    private PointerUpEvent __field_original = default!;
    public override PointerUpEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerUpEvent__events(PointerUpEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerUpEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerUpEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerUpEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), pointer: (pointer ?? this.pointer), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressure: (pressure ?? this.pressure), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public abstract class PointerSignalEvent : PointerEvent, _RespondablePointerEvent__events
{

    protected PointerSignalEvent(long viewId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.mouse, long device = 0, Offset position = default, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, pointer: pointer, kind: kind, device: device, position: position, embedderId: embedderId)
    {
    }

    public virtual void respond(bool allowPlatformDefault)
    {
    }

}

public delegate void RespondPointerEventCallback(bool allowPlatformDefault);

public interface _RespondablePointerEvent__events
{
    public void respond(bool allowPlatformDefault);
}

public interface _CopyPointerScrollEvent__events
{
    public global::Doroti.Ui.Offset scrollDelta { get; }
    public PointerScrollEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerScrollEvent : PointerSignalEvent, _PointerEventDescription__events, _CopyPointerScrollEvent__events
{
    public virtual Offset scrollDelta { get; private set; } = default!;
    internal virtual Action<bool>? _onRespond { get; private set; }
    public PointerScrollEvent() { }


    public PointerScrollEvent(long viewId = 0, Duration timeStamp = default, PointerDeviceKind kind = PointerDeviceKind.mouse, long device = 0, Offset position = default, Offset scrollDelta = default, long embedderId = 0, Action<bool>? onRespond = null) : base(viewId: viewId, timeStamp: timeStamp, kind: kind, device: device, position: position, embedderId: embedderId)
    {
        this.scrollDelta = scrollDelta;
        this._onRespond = onRespond;
    }

    public override PointerScrollEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerScrollEvent__events((((PointerScrollEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("scrollDelta", this.scrollDelta));
    }

    public override void respond(bool allowPlatformDefault)
    {
        this._onRespond?.Invoke(allowPlatformDefault);
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerScrollEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerScrollEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), scrollDelta: this.scrollDelta, embedderId: (embedderId ?? this.embedderId), onRespond: (onRespond ?? (((PointerScrollEvent?)(object?)this)!).respond)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerScrollEvent__events : PointerScrollEvent, _CopyPointerScrollEvent__events
{
    private PointerScrollEvent __field_original = default!;
    public override PointerScrollEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerScrollEvent__events(PointerScrollEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override Offset scrollDelta => ((PointerScrollEvent)this.original).scrollDelta;
    public override PointerScrollEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("scrollDelta", this.scrollDelta));
    }

    internal override Action<bool>? _onRespond => ((PointerScrollEvent)this.original)._onRespond;
    public override void respond(bool allowPlatformDefault)
    {
        this.original.respond(allowPlatformDefault: allowPlatformDefault);
    }

    public override PointerScrollEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerScrollEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), scrollDelta: this.scrollDelta, embedderId: (embedderId ?? this.embedderId), onRespond: (onRespond ?? (((PointerScrollEvent?)(object?)this)!).respond)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerScrollInertiaCancelEvent__events
{
    public PointerScrollInertiaCancelEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerScrollInertiaCancelEvent : PointerSignalEvent, _PointerEventDescription__events, _CopyPointerScrollInertiaCancelEvent__events
{
    public PointerScrollInertiaCancelEvent() { }


    public PointerScrollInertiaCancelEvent(long viewId = 0, Duration timeStamp = default, PointerDeviceKind kind = PointerDeviceKind.mouse, long device = 0, Offset position = default, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, kind: kind, device: device, position: position, embedderId: embedderId)
    {
    }

    public override PointerScrollInertiaCancelEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerScrollInertiaCancelEvent__events((((PointerScrollInertiaCancelEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerScrollInertiaCancelEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerScrollInertiaCancelEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerScrollInertiaCancelEvent__events : PointerScrollInertiaCancelEvent, _CopyPointerScrollInertiaCancelEvent__events, _RespondablePointerEvent__events
{
    private PointerScrollInertiaCancelEvent __field_original = default!;
    public override PointerScrollInertiaCancelEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerScrollInertiaCancelEvent__events(PointerScrollInertiaCancelEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerScrollInertiaCancelEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerScrollInertiaCancelEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerScrollInertiaCancelEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void respond(bool allowPlatformDefault)
    {
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerScaleEvent__events
{
    public double scale { get; }
    public PointerScaleEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerScaleEvent : PointerSignalEvent, _PointerEventDescription__events, _CopyPointerScaleEvent__events
{
    public virtual double scale { get; private set; } = default!;
    public PointerScaleEvent() { }


    public PointerScaleEvent(long viewId = 0, Duration timeStamp = default, PointerDeviceKind kind = PointerDeviceKind.mouse, long device = 0, Offset position = default, long embedderId = 0, double scale = 1.0) : base(viewId: viewId, timeStamp: timeStamp, kind: kind, device: device, position: position, embedderId: embedderId)
    {
        this.scale = scale;
    }

    public override PointerScaleEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerScaleEvent__events((((PointerScaleEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerScaleEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerScaleEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId), scale: (scale ?? this.scale)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerScaleEvent__events : PointerScaleEvent, _CopyPointerScaleEvent__events, _RespondablePointerEvent__events
{
    private PointerScaleEvent __field_original = default!;
    public override PointerScaleEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerScaleEvent__events(PointerScaleEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override double scale => ((PointerScaleEvent)this.original).scale;
    public override PointerScaleEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerScaleEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerScaleEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId), scale: (scale ?? this.scale)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void respond(bool allowPlatformDefault)
    {
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerPanZoomStartEvent__events
{
    public PointerPanZoomStartEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerPanZoomStartEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerPanZoomStartEvent__events
{
    public PointerPanZoomStartEvent() { }


    public PointerPanZoomStartEvent(long viewId = 0, Duration timeStamp = default, long device = 0, long pointer = 0, Offset position = default, long embedderId = 0, bool synthesized = false) : base(viewId: viewId, timeStamp: timeStamp, device: device, pointer: pointer, position: position, embedderId: embedderId, synthesized: synthesized, kind: PointerDeviceKind.trackpad)
    {
    }

    public override PointerPanZoomStartEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerPanZoomStartEvent__events((((PointerPanZoomStartEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerPanZoomStartEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        DartRuntimePrimitives.Assert(() => ((kind is null) || DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad)));
        return new PointerPanZoomStartEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerPanZoomStartEvent__events : PointerPanZoomStartEvent, _CopyPointerPanZoomStartEvent__events
{
    private PointerPanZoomStartEvent __field_original = default!;
    public override PointerPanZoomStartEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerPanZoomStartEvent__events(PointerPanZoomStartEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerPanZoomStartEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerPanZoomStartEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        DartRuntimePrimitives.Assert(() => ((kind is null) || DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad)));
        return new PointerPanZoomStartEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerPanZoomUpdateEvent__events
{
    public global::Doroti.Ui.Offset pan { get; }
    public global::Doroti.Ui.Offset localPan { get; }
    public global::Doroti.Ui.Offset panDelta { get; }
    public global::Doroti.Ui.Offset localPanDelta { get; }
    public double scale { get; }
    public double rotation { get; }
    public PointerPanZoomUpdateEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerPanZoomUpdateEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerPanZoomUpdateEvent__events
{
    public virtual Offset pan { get; private set; } = default!;
    public virtual Offset panDelta { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual double rotation { get; private set; } = default!;
    public PointerPanZoomUpdateEvent() { }


    public PointerPanZoomUpdateEvent(long viewId = 0, Duration timeStamp = default, long device = 0, long pointer = 0, Offset position = default, long embedderId = 0, Offset pan = default, Offset panDelta = default, double scale = 1.0, double rotation = 0.0, bool synthesized = false) : base(viewId: viewId, timeStamp: timeStamp, device: device, pointer: pointer, position: position, embedderId: embedderId, synthesized: synthesized, kind: PointerDeviceKind.trackpad)
    {
        this.pan = pan;
        this.panDelta = panDelta;
        this.scale = scale;
        this.rotation = rotation;
    }

    public virtual Offset localPan => this.pan;
    public virtual Offset localPanDelta => this.panDelta;
    public override PointerPanZoomUpdateEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerPanZoomUpdateEvent__events((((PointerPanZoomUpdateEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerPanZoomUpdateEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        DartRuntimePrimitives.Assert(() => ((kind is null) || DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad)));
        return new PointerPanZoomUpdateEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId), pan: (pan ?? this.pan), panDelta: (panDelta ?? this.panDelta), scale: (scale ?? this.scale), rotation: (rotation ?? this.rotation)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerPanZoomUpdateEvent__events : PointerPanZoomUpdateEvent, _CopyPointerPanZoomUpdateEvent__events
{
    private bool __late_localPan_initialized;
    private Offset __late_localPan = default!;
    public override Offset localPan
    {
        get
        {
            if (!__late_localPan_initialized)
            {
                __late_localPan = PointerEvent.transformPosition(this.transform, this.pan);
                __late_localPan_initialized = true;
            }
            return __late_localPan;
        }
    }
    private bool __late_localPanDelta_initialized;
    private Offset __late_localPanDelta = default!;
    public override Offset localPanDelta
    {
        get
        {
            if (!__late_localPanDelta_initialized)
            {
                __late_localPanDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.panDelta, untransformedEndPosition: this.pan, transformedEndPosition: this.localPan);
                __late_localPanDelta_initialized = true;
            }
            return __late_localPanDelta;
        }
    }
    private PointerPanZoomUpdateEvent __field_original = default!;
    public override PointerPanZoomUpdateEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerPanZoomUpdateEvent__events(PointerPanZoomUpdateEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override Offset pan => ((PointerPanZoomUpdateEvent)this.original).pan;
    public override Offset panDelta => ((PointerPanZoomUpdateEvent)this.original).panDelta;
    public override double scale => ((PointerPanZoomUpdateEvent)this.original).scale;
    public override double rotation => ((PointerPanZoomUpdateEvent)this.original).rotation;
    public override PointerPanZoomUpdateEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerPanZoomUpdateEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        DartRuntimePrimitives.Assert(() => ((kind is null) || DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad)));
        return new PointerPanZoomUpdateEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId), pan: (pan ?? this.pan), panDelta: (panDelta ?? this.panDelta), scale: (scale ?? this.scale), rotation: (rotation ?? this.rotation)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerPanZoomEndEvent__events
{
    public PointerPanZoomEndEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerPanZoomEndEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerPanZoomEndEvent__events
{
    public PointerPanZoomEndEvent() { }


    public PointerPanZoomEndEvent(long viewId = 0, Duration timeStamp = default, long device = 0, long pointer = 0, Offset position = default, long embedderId = 0, bool synthesized = false) : base(viewId: viewId, timeStamp: timeStamp, device: device, pointer: pointer, position: position, embedderId: embedderId, synthesized: synthesized, kind: PointerDeviceKind.trackpad)
    {
    }

    public override PointerPanZoomEndEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerPanZoomEndEvent__events((((PointerPanZoomEndEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerPanZoomEndEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        DartRuntimePrimitives.Assert(() => ((kind is null) || DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad)));
        return new PointerPanZoomEndEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _TransformedPointerPanZoomEndEvent__events : PointerPanZoomEndEvent, _CopyPointerPanZoomEndEvent__events
{
    private PointerPanZoomEndEvent __field_original = default!;
    public override PointerPanZoomEndEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerPanZoomEndEvent__events(PointerPanZoomEndEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerPanZoomEndEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerPanZoomEndEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        DartRuntimePrimitives.Assert(() => ((kind is null) || DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad)));
        return new PointerPanZoomEndEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), device: (device ?? this.device), position: (position ?? this.position), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}

public interface _CopyPointerCancelEvent__events
{
    public PointerCancelEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null);
}

public class PointerCancelEvent : PointerEvent, _PointerEventDescription__events, _CopyPointerCancelEvent__events
{
    public PointerCancelEvent() { }


    public PointerCancelEvent(long viewId = 0, Duration timeStamp = default, long pointer = 0, PointerDeviceKind kind = PointerDeviceKind.touch, long device = 0, Offset position = default, long buttons = 0, bool obscured = false, double pressureMin = 1.0, double pressureMax = 1.0, double distance = 0.0, double distanceMax = 0.0, double size = 0.0, double radiusMajor = 0.0, double radiusMinor = 0.0, double radiusMin = 0.0, double radiusMax = 0.0, double orientation = 0.0, double tilt = 0.0, long embedderId = 0) : base(viewId: viewId, timeStamp: timeStamp, pointer: pointer, kind: kind, device: device, position: position, buttons: buttons, obscured: obscured, pressureMin: pressureMin, pressureMax: pressureMax, distance: distance, distanceMax: distanceMax, size: size, radiusMajor: radiusMajor, radiusMinor: radiusMinor, radiusMin: radiusMin, radiusMax: radiusMax, orientation: orientation, tilt: tilt, embedderId: embedderId, down: false, pressure: 0.0)
    {
        System.Diagnostics.Debug.Assert(!DartRuntimePrimitives.Identical(kind, PointerDeviceKind.trackpad));
    }

    public override PointerCancelEvent transformed(Matrix4? transform)
    {
        if (((transform is null) || (object.Equals(transform, this.transform))))
        {
            return this;
        }
        return new _TransformedPointerCancelEvent__events((((PointerCancelEvent?)(object?)original)! ?? this), transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("position", position));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", localPosition, defaultValue: position, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", delta, defaultValue: Offset.zero, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localDelta", localDelta, defaultValue: delta, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<Duration>("timeStamp", timeStamp, defaultValue: Duration.zero, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("pointer", pointer, level: DiagnosticLevel.debug));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", kind, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("device", device, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("buttons", buttons, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<bool>("down", down, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressure", pressure, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMin", pressureMin, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("pressureMax", pressureMax, defaultValue: 1.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distance", distance, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMin", distanceMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("distanceMax", distanceMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("size", size, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMajor", radiusMajor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMinor", radiusMinor, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMin", radiusMin, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("radiusMax", radiusMax, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("orientation", orientation, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new DoubleProperty("tilt", tilt, defaultValue: 0.0, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("platformData", platformData, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("obscured", value: obscured, ifTrue: "obscured", level: DiagnosticLevel.debug));
        properties.add(new FlagProperty("synthesized", value: synthesized, ifTrue: "synthesized", level: DiagnosticLevel.debug));
        properties.add(new IntProperty("embedderId", embedderId, defaultValue: 0L, level: DiagnosticLevel.debug));
        properties.add(new IntProperty("viewId", viewId, defaultValue: 0L, level: DiagnosticLevel.debug));
    }

    public virtual string toStringFull()
    {
        return toDiagnosticsNode().toStringDeep(minLevel: DiagnosticLevel.fine);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PointerCancelEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerCancelEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), pointer: (pointer ?? this.pointer), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class EventsLibrary
{
    public static double computeHitSlop(PointerDeviceKind kind, DeviceGestureSettings? settings)
    {
        switch (kind)
        {
            case PointerDeviceKind.mouse:
                {
                    return global::Doroti.Framework.Gestures.ConstantsLibrary.kPrecisePointerHitSlop;
                }
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            case PointerDeviceKind.unknown:
            case PointerDeviceKind.touch:
            case PointerDeviceKind.trackpad:
                {
                    return (settings?.touchSlop ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kTouchSlop);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class EventsLibrary
{
    public static double computePanSlop(PointerDeviceKind kind, DeviceGestureSettings? settings)
    {
        switch (kind)
        {
            case PointerDeviceKind.mouse:
                {
                    return global::Doroti.Framework.Gestures.ConstantsLibrary.kPrecisePointerPanSlop;
                }
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            case PointerDeviceKind.unknown:
            case PointerDeviceKind.touch:
            case PointerDeviceKind.trackpad:
                {
                    return (settings?.panSlop ?? global::Doroti.Framework.Gestures.ConstantsLibrary.kPanSlop);
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class EventsLibrary
{
    public static double computeScaleSlop(PointerDeviceKind kind)
    {
        switch (kind)
        {
            case PointerDeviceKind.mouse:
                {
                    return global::Doroti.Framework.Gestures.ConstantsLibrary.kPrecisePointerScaleSlop;
                }
            case PointerDeviceKind.stylus:
            case PointerDeviceKind.invertedStylus:
            case PointerDeviceKind.unknown:
            case PointerDeviceKind.touch:
            case PointerDeviceKind.trackpad:
                {
                    return global::Doroti.Framework.Gestures.ConstantsLibrary.kScaleSlop;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _TransformedPointerCancelEvent__events : PointerCancelEvent, _CopyPointerCancelEvent__events
{
    private PointerCancelEvent __field_original = default!;
    public override PointerCancelEvent original { get => __field_original; }
    private Matrix4 __field_transform = default!;
    public override Matrix4 transform { get => __field_transform; }
    private bool __late_localPosition_initialized;
    private Offset __late_localPosition = default!;
    public override Offset localPosition
    {
        get
        {
            if (!__late_localPosition_initialized)
            {
                __late_localPosition = PointerEvent.transformPosition(this.transform, this.position);
                __late_localPosition_initialized = true;
            }
            return __late_localPosition;
        }
    }
    private bool __late_localDelta_initialized;
    private Offset __late_localDelta = default!;
    public override Offset localDelta
    {
        get
        {
            if (!__late_localDelta_initialized)
            {
                __late_localDelta = PointerEvent.transformDeltaViaPositions(transform: this.transform, untransformedDelta: this.delta, untransformedEndPosition: this.position, transformedEndPosition: this.localPosition);
                __late_localDelta_initialized = true;
            }
            return __late_localDelta;
        }
    }

    internal _TransformedPointerCancelEvent__events(PointerCancelEvent original, Matrix4 transform)
    {
        this.__field_original = original;
        this.__field_transform = transform;
    }

    public override PointerCancelEvent transformed(Matrix4? transform) => this.original.transformed(transform);
    public override PointerCancelEvent copyWith(long? viewId = null, Duration? timeStamp = null, long? pointer = null, PointerDeviceKind? kind = null, long? device = null, Offset? position = null, Offset? delta = null, long? buttons = null, bool? obscured = null, double? pressure = null, double? pressureMin = null, double? pressureMax = null, double? distance = null, double? distanceMax = null, double? size = null, double? radiusMajor = null, double? radiusMinor = null, double? radiusMin = null, double? radiusMax = null, double? orientation = null, double? tilt = null, bool? synthesized = null, long? embedderId = null, Offset? pan = null, Offset? localPan = null, Offset? panDelta = null, Offset? localPanDelta = null, double? scale = null, double? rotation = null, Action<bool>? onRespond = null, Offset? localPosition = null)
    {
        return new PointerCancelEvent(viewId: (viewId ?? this.viewId), timeStamp: (timeStamp ?? this.timeStamp), pointer: (pointer ?? this.pointer), kind: (kind ?? this.kind), device: (device ?? this.device), position: (position ?? this.position), buttons: (buttons ?? this.buttons), obscured: (obscured ?? this.obscured), pressureMin: (pressureMin ?? this.pressureMin), pressureMax: (pressureMax ?? this.pressureMax), distance: (distance ?? this.distance), distanceMax: (distanceMax ?? this.distanceMax), size: (size ?? this.size), radiusMajor: (radiusMajor ?? this.radiusMajor), radiusMinor: (radiusMinor ?? this.radiusMinor), radiusMin: (radiusMin ?? this.radiusMin), radiusMax: (radiusMax ?? this.radiusMax), orientation: (orientation ?? this.orientation), tilt: (tilt ?? this.tilt), embedderId: (embedderId ?? this.embedderId)).transformed(transform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long embedderId => ((PointerEvent)this.original).embedderId;
    public override Duration timeStamp => ((PointerEvent)this.original).timeStamp;
    public override long pointer => ((PointerEvent)this.original).pointer;
    public override PointerDeviceKind kind => ((PointerEvent)this.original).kind;
    public override long device => ((PointerEvent)this.original).device;
    public override Offset position => ((PointerEvent)this.original).position;
    public override Offset delta => ((PointerEvent)this.original).delta;
    public override long buttons => ((PointerEvent)this.original).buttons;
    public override bool down => ((PointerEvent)this.original).down;
    public override bool obscured => ((PointerEvent)this.original).obscured;
    public override double pressure => ((PointerEvent)this.original).pressure;
    public override double pressureMin => ((PointerEvent)this.original).pressureMin;
    public override double pressureMax => ((PointerEvent)this.original).pressureMax;
    public override double distance => ((PointerEvent)this.original).distance;
    public override double distanceMin => 0.0;
    public override double distanceMax => ((PointerEvent)this.original).distanceMax;
    public override double size => ((PointerEvent)this.original).size;
    public override double radiusMajor => ((PointerEvent)this.original).radiusMajor;
    public override double radiusMinor => ((PointerEvent)this.original).radiusMinor;
    public override double radiusMin => ((PointerEvent)this.original).radiusMin;
    public override double radiusMax => ((PointerEvent)this.original).radiusMax;
    public override double orientation => ((PointerEvent)this.original).orientation;
    public override double tilt => ((PointerEvent)this.original).tilt;
    public override long platformData => ((PointerEvent)this.original).platformData;
    public override bool synthesized => ((PointerEvent)this.original).synthesized;
    public override long viewId => ((PointerEvent)this.original).viewId;
}
