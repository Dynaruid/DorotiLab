// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/drag_details.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public class DragDownDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }

    public DragDownDetails(Offset globalPosition = default, Offset? localPosition = null)
    {
        this.__field_globalPosition = globalPosition;
        this.__field_localPosition = (localPosition ?? globalPosition);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
    }

}

public delegate void GestureDragDownCallback(DragDownDetails details);

public class DragStartDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual Duration? sourceTimeStamp { get; private set; }
    public virtual PointerDeviceKind? kind { get; private set; }

    public DragStartDetails(Offset globalPosition = default, Offset? localPosition = null, Duration? sourceTimeStamp = null, PointerDeviceKind? kind = null)
    {
        this.__field_globalPosition = globalPosition;
        this.sourceTimeStamp = sourceTimeStamp;
        this.kind = kind;
        this.__field_localPosition = (localPosition ?? globalPosition);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", this.sourceTimeStamp));
        properties.add(new EnumProperty<global::Doroti.Ui.PointerDeviceKind>("kind", this.kind));
    }

}

public delegate void GestureDragStartCallback(DragStartDetails details);

public class DragUpdateDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual Duration? sourceTimeStamp { get; private set; }
    public virtual Offset delta { get; private set; } = default!;
    public virtual double? primaryDelta { get; private set; }
    public virtual PointerDeviceKind? kind { get; private set; }

    public DragUpdateDetails(Offset globalPosition, Offset? localPosition = null, Duration? sourceTimeStamp = null, Offset delta = default, double? primaryDelta = null, PointerDeviceKind? kind = null)
    {
        this.__field_globalPosition = globalPosition;
        this.sourceTimeStamp = sourceTimeStamp;
        this.delta = delta;
        this.primaryDelta = primaryDelta;
        this.kind = kind;
        this.__field_localPosition = (localPosition ?? globalPosition);
        System.Diagnostics.Debug.Assert((((primaryDelta is null) || (((DartRuntimePrimitives.RequireValue(primaryDelta) == delta.dx) && (delta.dy == 0.0)))) || (((DartRuntimePrimitives.RequireValue(primaryDelta) == delta.dy) && (delta.dx == 0.0)))));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", this.sourceTimeStamp));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("delta", this.delta));
        properties.add(new DoubleProperty("primaryDelta", this.primaryDelta));
    }

}

public delegate void GestureDragUpdateCallback(DragUpdateDetails details);

public class DragEndDetails : PositionedGestureDetails, Diagnosticable
{
    private Offset __field_globalPosition = default!;
    public override Offset globalPosition { get => __field_globalPosition; }
    private Offset __field_localPosition = default!;
    public override Offset localPosition { get => __field_localPosition; }
    public virtual Velocity velocity { get; private set; } = default!;
    public virtual double? primaryVelocity { get; private set; }

    public DragEndDetails(Offset globalPosition = default, Offset? localPosition = null, Velocity velocity = default!, double? primaryVelocity = null)
    {
        Velocity __velocity = velocity ?? Velocity.zero;
        this.__field_globalPosition = globalPosition;
        this.velocity = __velocity;
        this.primaryVelocity = primaryVelocity;
        this.__field_localPosition = (localPosition ?? globalPosition);
        System.Diagnostics.Debug.Assert((((primaryVelocity is null) || (((DartRuntimePrimitives.RequireValue(primaryVelocity) == ((Velocity)__velocity).pixelsPerSecond.dx) && (((Velocity)__velocity).pixelsPerSecond.dy == 0L)))) || (((DartRuntimePrimitives.RequireValue(primaryVelocity) == ((Velocity)__velocity).pixelsPerSecond.dy) && (((Velocity)__velocity).pixelsPerSecond.dx == 0L)))));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("globalPosition", this.globalPosition));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localPosition", this.localPosition));
        properties.add(new DiagnosticsProperty<Velocity>("velocity", this.velocity));
        properties.add(new DoubleProperty("primaryVelocity", this.primaryVelocity));
    }

}

