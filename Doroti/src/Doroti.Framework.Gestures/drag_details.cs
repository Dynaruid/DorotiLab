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
        __field_globalPosition = globalPosition;
        __field_localPosition = localPosition ?? globalPosition;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
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
        __field_globalPosition = globalPosition;
        this.sourceTimeStamp = sourceTimeStamp;
        this.kind = kind;
        __field_localPosition = localPosition ?? globalPosition;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", sourceTimeStamp));
        properties.add(new EnumProperty<PointerDeviceKind>("kind", kind));
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
        __field_globalPosition = globalPosition;
        this.sourceTimeStamp = sourceTimeStamp;
        this.delta = delta;
        this.primaryDelta = primaryDelta;
        this.kind = kind;
        __field_localPosition = localPosition ?? globalPosition;
        System.Diagnostics.Debug.Assert((primaryDelta is null) || (DartRuntimePrimitives.RequireValue(primaryDelta) == delta.dx) && (delta.dy == 0.0) || (DartRuntimePrimitives.RequireValue(primaryDelta) == delta.dy) && (delta.dx == 0.0));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", sourceTimeStamp));
        properties.add(new DiagnosticsProperty<Offset>("delta", delta));
        properties.add(new DoubleProperty("primaryDelta", primaryDelta));
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
        __field_globalPosition = globalPosition;
        this.velocity = __velocity;
        this.primaryVelocity = primaryVelocity;
        __field_localPosition = localPosition ?? globalPosition;
        System.Diagnostics.Debug.Assert((primaryVelocity is null) || (DartRuntimePrimitives.RequireValue(primaryVelocity) == __velocity.pixelsPerSecond.dx) && (__velocity.pixelsPerSecond.dy == 0L) || (DartRuntimePrimitives.RequireValue(primaryVelocity) == __velocity.pixelsPerSecond.dy) && (__velocity.pixelsPerSecond.dx == 0L));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Offset>("globalPosition", globalPosition));
        properties.add(new DiagnosticsProperty<Offset>("localPosition", localPosition));
        properties.add(new DiagnosticsProperty<Velocity>("velocity", velocity));
        properties.add(new DoubleProperty("primaryVelocity", primaryVelocity));
    }

}

