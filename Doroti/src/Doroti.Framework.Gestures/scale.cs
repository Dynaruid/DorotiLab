// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/scale.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Gestures;

public static partial class ScaleLibrary
{
    public static double kDefaultMouseScrollToScaleFactor = 200;
}

public static partial class ScaleLibrary
{
    public static Offset kDefaultTrackpadScrollToScaleFactor = new global::Doroti.Ui.Offset(0, (-1L / kDefaultMouseScrollToScaleFactor));
}

internal enum _ScaleState__scale
{
    ready,
    possible,
    accepted,
    started
}

internal class _PointerPanZoomData__scale
{
    public virtual ScaleGestureRecognizer parent { get; private set; } = default!;
    internal virtual Offset _position { get; private set; } = default!;
    internal virtual Offset _pan { get; private set; } = default!;
    internal virtual double _scale { get; private set; } = default!;
    internal virtual double _rotation { get; private set; } = default!;

    private _PointerPanZoomData__scale(ScaleGestureRecognizer parent, Offset position, Offset pan, double scale, double rotation)
    {
        this.parent = parent;
        this._position = position;
        this._pan = pan;
        this._scale = scale;
        this._rotation = rotation;
    }

    internal _PointerPanZoomData__scale(ScaleGestureRecognizer parent, PointerPanZoomStartEvent @event)
        : this(parent, @event.position, Offset.zero, 1, 0) { }

    internal static _PointerPanZoomData__scale CreateFromUpdateEvent(ScaleGestureRecognizer parent, PointerPanZoomUpdateEvent @event)
    {
        return new _PointerPanZoomData__scale(parent, @event.position, @event.pan, @event.scale, @event.rotation);
    }

    public virtual global::Doroti.Ui.Offset focalPoint
    {
        get
        {
            if (((ScaleGestureRecognizer)this.parent).trackpadScrollCausesScale)
            {
                return this._position;
            }
            return (this._position + this._pan);
        }
    }
    public virtual double scale
    {
        get
        {
            if (((ScaleGestureRecognizer)this.parent).trackpadScrollCausesScale)
            {
                return (this._scale * Dart_mathLibrary.exp((((this._pan.dx * ((ScaleGestureRecognizer)this.parent).trackpadScrollToScaleFactor.dx)) + ((this._pan.dy * ((ScaleGestureRecognizer)this.parent).trackpadScrollToScaleFactor.dy)))));
            }
            return this._scale;
        }
    }
    public virtual double rotation => this._rotation;
    public override string ToString() => $"_PointerPanZoomData(parent: {this.parent}, _position: {this._position}, _pan: {this._pan}, _scale: {this._scale}, _rotation: {this._rotation})";
}

public class ScaleStartDetails : Diagnosticable
{
    public virtual Offset focalPoint { get; private set; } = default!;
    public virtual Offset localFocalPoint { get; private set; } = default!;
    public virtual long pointerCount { get; private set; } = default!;
    public virtual Duration? sourceTimeStamp { get; private set; }
    public virtual PointerDeviceKind? kind { get; private set; }

    public ScaleStartDetails(Offset focalPoint = default, Offset? localFocalPoint = null, long pointerCount = 0, Duration? sourceTimeStamp = null, PointerDeviceKind? kind = null)
    {
        this.focalPoint = focalPoint;
        this.pointerCount = pointerCount;
        this.sourceTimeStamp = sourceTimeStamp;
        this.kind = kind;
        this.localFocalPoint = (localFocalPoint ?? focalPoint);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("focalPoint", this.focalPoint));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localFocalPoint", this.localFocalPoint));
        properties.add(new IntProperty("pointerCount", this.pointerCount));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", this.sourceTimeStamp));
    }

}

public class ScaleUpdateDetails : Diagnosticable
{
    public virtual Offset focalPointDelta { get; private set; } = default!;
    public virtual Offset focalPoint { get; private set; } = default!;
    public virtual Offset localFocalPoint { get; private set; } = default!;
    public virtual double scale { get; private set; } = default!;
    public virtual double horizontalScale { get; private set; } = default!;
    public virtual double verticalScale { get; private set; } = default!;
    public virtual double rotation { get; private set; } = default!;
    public virtual long pointerCount { get; private set; } = default!;
    public virtual Duration? sourceTimeStamp { get; private set; }

    public ScaleUpdateDetails(Offset focalPoint = default, Offset? localFocalPoint = null, double scale = 1.0, double horizontalScale = 1.0, double verticalScale = 1.0, double rotation = 0.0, long pointerCount = 0, Offset focalPointDelta = default, Duration? sourceTimeStamp = null)
    {
        this.focalPoint = focalPoint;
        this.scale = scale;
        this.horizontalScale = horizontalScale;
        this.verticalScale = verticalScale;
        this.rotation = rotation;
        this.pointerCount = pointerCount;
        this.focalPointDelta = focalPointDelta;
        this.sourceTimeStamp = sourceTimeStamp;
        this.localFocalPoint = (localFocalPoint ?? focalPoint);
        System.Diagnostics.Debug.Assert((scale >= 0.0));
        System.Diagnostics.Debug.Assert((horizontalScale >= 0.0));
        System.Diagnostics.Debug.Assert((verticalScale >= 0.0));
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("focalPointDelta", this.focalPointDelta));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("focalPoint", this.focalPoint));
        properties.add(new DiagnosticsProperty<global::Doroti.Ui.Offset>("localFocalPoint", this.localFocalPoint));
        properties.add(new DoubleProperty("scale", this.scale));
        properties.add(new DoubleProperty("horizontalScale", this.horizontalScale));
        properties.add(new DoubleProperty("verticalScale", this.verticalScale));
        properties.add(new DoubleProperty("rotation", this.rotation));
        properties.add(new IntProperty("pointerCount", this.pointerCount));
        properties.add(new DiagnosticsProperty<Duration?>("sourceTimeStamp", this.sourceTimeStamp));
    }

}

public class ScaleEndDetails : Diagnosticable
{
    public virtual Velocity velocity { get; private set; } = default!;
    public virtual double scaleVelocity { get; private set; } = default!;
    public virtual long pointerCount { get; private set; } = default!;

    public ScaleEndDetails(Velocity velocity = default!, double scaleVelocity = 0, long pointerCount = 0)
    {
        Velocity __velocity = velocity ?? Velocity.zero;
        this.velocity = __velocity;
        this.scaleVelocity = scaleVelocity;
        this.pointerCount = pointerCount;
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<Velocity>("velocity", this.velocity));
        properties.add(new DoubleProperty("scaleVelocity", this.scaleVelocity));
        properties.add(new IntProperty("pointerCount", this.pointerCount));
    }

}

public delegate void GestureScaleStartCallback(ScaleStartDetails details);

public delegate void GestureScaleUpdateCallback(ScaleUpdateDetails details);

public delegate void GestureScaleEndCallback(ScaleEndDetails details);

public static partial class ScaleLibrary
{
    internal static bool _isFlingGesture(Velocity velocity)
    {
        double speedSquared = ((Velocity)velocity).pixelsPerSecond.distanceSquared;
        return (speedSquared > (ConstantsLibrary.kMinFlingVelocity * ConstantsLibrary.kMinFlingVelocity));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _LineBetweenPointers__scale
{
    public virtual Offset pointerStartLocation { get; private set; } = default!;
    public virtual long pointerStartId { get; private set; } = default!;
    public virtual Offset pointerEndLocation { get; private set; } = default!;
    public virtual long pointerEndId { get; private set; } = default!;

    internal _LineBetweenPointers__scale(Offset pointerStartLocation = default, long pointerStartId = 0, Offset pointerEndLocation = default, long pointerEndId = 1)
    {
        this.pointerStartLocation = pointerStartLocation;
        this.pointerStartId = pointerStartId;
        this.pointerEndLocation = pointerEndLocation;
        this.pointerEndId = pointerEndId;
        System.Diagnostics.Debug.Assert((pointerStartId != pointerEndId));
    }

}

public class ScaleGestureRecognizer : OneSequenceGestureRecognizer
{
    public virtual DragStartBehavior dragStartBehavior { get; set; } = default!;
    public virtual Action<ScaleStartDetails>? onStart { get; set; } = default;
    public virtual Action<ScaleUpdateDetails>? onUpdate { get; set; } = default;
    public virtual Action<ScaleEndDetails>? onEnd { get; set; } = default;
    internal virtual _ScaleState__scale _state { get; set; } = _ScaleState__scale.ready;
    internal virtual Matrix4? _lastTransform { get; set; } = default;
    public virtual bool trackpadScrollCausesScale { get; set; } = default!;
    public virtual Offset trackpadScrollToScaleFactor { get; set; } = default!;
    internal virtual Offset _initialFocalPoint { get; set; } = default!;
    internal virtual Offset? _currentFocalPoint { get; set; } = default;
    internal virtual double _initialSpan { get; set; } = default!;
    internal virtual double _currentSpan { get; set; } = default!;
    internal virtual double _initialHorizontalSpan { get; set; } = default!;
    internal virtual double _currentHorizontalSpan { get; set; } = default!;
    internal virtual double _initialVerticalSpan { get; set; } = default!;
    internal virtual double _currentVerticalSpan { get; set; } = default!;
    internal virtual Offset _localFocalPoint { get; set; } = default!;
    internal virtual _LineBetweenPointers__scale? _initialLine { get; set; } = default;
    internal virtual _LineBetweenPointers__scale? _currentLine { get; set; } = default;
    internal virtual DartMap<long, Offset> _pointerLocations { get; private set; } = new DartMap<long, Offset>();
    internal virtual List<long> _pointerQueue { get; private set; } = new List<long>();
    internal virtual DartMap<long, VelocityTracker> _velocityTrackers { get; private set; } = new DartMap<long, VelocityTracker>();
    internal virtual VelocityTracker? _scaleVelocityTracker { get; set; } = default;
    internal virtual Offset _delta { get; set; } = default!;
    internal virtual DartMap<long, _PointerPanZoomData__scale> _pointerPanZooms { get; private set; } = new DartMap<long, _PointerPanZoomData__scale>();
    internal virtual double _initialPanZoomScaleFactor { get; set; } = 1;
    internal virtual double _initialPanZoomRotationFactor { get; set; } = 0;
    internal virtual Duration? _initialEventTimestamp { get; set; } = default;

    public ScaleGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!, DragStartBehavior dragStartBehavior = DragStartBehavior.down, bool trackpadScrollCausesScale = false, Offset? trackpadScrollToScaleFactor = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? _defaultButtonAcceptBehavior)
    {
        Offset __trackpadScrollToScaleFactor = trackpadScrollToScaleFactor ?? ScaleLibrary.kDefaultTrackpadScrollToScaleFactor;
        this.dragStartBehavior = dragStartBehavior;
        this.trackpadScrollCausesScale = trackpadScrollCausesScale;
        this.trackpadScrollToScaleFactor = __trackpadScrollToScaleFactor;
    }

    public virtual long pointerCount
    {
        get
        {
            return (((2L * checked((long)(this._pointerPanZooms.Count)))) + checked((long)(this._pointerQueue.Count)));
        }
    }
    internal virtual double _pointerScaleFactor => ((this._initialSpan > 0.0) ? (this._currentSpan / this._initialSpan) : 1.0);
    internal virtual double _pointerHorizontalScaleFactor => ((this._initialHorizontalSpan > 0.0) ? (this._currentHorizontalSpan / this._initialHorizontalSpan) : 1.0);
    internal virtual double _pointerVerticalScaleFactor => ((this._initialVerticalSpan > 0.0) ? (this._currentVerticalSpan / this._initialVerticalSpan) : 1.0);
    internal virtual double _scaleFactor
    {
        get
        {
            double scaleLocal = this._pointerScaleFactor;
            foreach (_PointerPanZoomData__scale p in this._pointerPanZooms.Values)
            {
                scaleLocal *= (((_PointerPanZoomData__scale)p).scale / this._initialPanZoomScaleFactor);
            }
            return scaleLocal;
        }
    }
    internal virtual double _horizontalScaleFactor
    {
        get
        {
            double scaleLocal = this._pointerHorizontalScaleFactor;
            foreach (_PointerPanZoomData__scale p in this._pointerPanZooms.Values)
            {
                scaleLocal *= (((_PointerPanZoomData__scale)p).scale / this._initialPanZoomScaleFactor);
            }
            return scaleLocal;
        }
    }
    internal virtual double _verticalScaleFactor
    {
        get
        {
            double scaleLocal = this._pointerVerticalScaleFactor;
            foreach (_PointerPanZoomData__scale p in this._pointerPanZooms.Values)
            {
                scaleLocal *= (((_PointerPanZoomData__scale)p).scale / this._initialPanZoomScaleFactor);
            }
            return scaleLocal;
        }
    }
    internal virtual double _computeRotationFactor()
    {
        var factor = 0.0;
        if (((this._initialLine is not null) && (this._currentLine is not null)))
        {
            double fx = this._initialLine!.pointerStartLocation.dx;
            double fy = this._initialLine!.pointerStartLocation.dy;
            double sx = this._initialLine!.pointerEndLocation.dx;
            double sy = this._initialLine!.pointerEndLocation.dy;
            double nfx = this._currentLine!.pointerStartLocation.dx;
            double nfy = this._currentLine!.pointerStartLocation.dy;
            double nsx = this._currentLine!.pointerEndLocation.dx;
            double nsy = this._currentLine!.pointerEndLocation.dy;
            double angle1 = Dart_mathLibrary.atan2((fy - sy), (fx - sx));
            double angle2 = Dart_mathLibrary.atan2((nfy - nsy), (nfx - nsx));
            factor = (angle2 - angle1);
        }
        foreach (_PointerPanZoomData__scale p in this._pointerPanZooms.Values)
        {
            factor += ((_PointerPanZoomData__scale)p).rotation;
        }
        factor -= this._initialPanZoomRotationFactor;
        return factor;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        this._velocityTrackers[@event.pointer] = new VelocityTracker(@event.kind);
        _initialEventTimestamp = @event.timeStamp;
        if ((Equals(this._state, _ScaleState__scale.ready)))
        {
            _state = _ScaleState__scale.possible;
            _initialSpan = 0.0;
            _currentSpan = 0.0;
            _initialHorizontalSpan = 0.0;
            _currentHorizontalSpan = 0.0;
            _initialVerticalSpan = 0.0;
            _currentVerticalSpan = 0.0;
        }
    }

    public override bool isPointerPanZoomAllowed(PointerPanZoomStartEvent @event) => true;
    public override void addAllowedPointerPanZoom(PointerPanZoomStartEvent @event)
    {
        base.addAllowedPointerPanZoom(@event);
        startTrackingPointer(@event.pointer, @event.transform);
        this._velocityTrackers[@event.pointer] = new VelocityTracker(@event.kind);
        _initialEventTimestamp = @event.timeStamp;
        if ((Equals(this._state, _ScaleState__scale.ready)))
        {
            _state = _ScaleState__scale.possible;
            _initialPanZoomScaleFactor = 1.0;
            _initialPanZoomRotationFactor = 0.0;
        }
    }

    public override void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (!Equals(this._state, _ScaleState__scale.ready)));
        var didChangeConfiguration = false;
        var shouldStartIfAccepted = false;
        if ((@event is PointerMoveEvent))
        {
            VelocityTracker tracker = this._velocityTrackers.GetValueOrDefault(((PointerMoveEvent)@event).pointer)!;
            if (!((PointerMoveEvent)@event).synthesized)
            {
                tracker.addPosition(((PointerMoveEvent)@event).timeStamp, ((PointerMoveEvent)@event).position);
            }
            this._pointerLocations[@event.pointer] = ((PointerMoveEvent)@event).position;
            shouldStartIfAccepted = true;
            _lastTransform = ((PointerMoveEvent)@event).transform;
        }
        else
        {
            if ((@event is PointerDownEvent))
            {
                this._pointerLocations[@event.pointer] = ((PointerDownEvent)@event).position;
                this._pointerQueue.Add(((PointerDownEvent)@event).pointer);
                didChangeConfiguration = true;
                shouldStartIfAccepted = true;
                _lastTransform = ((PointerDownEvent)@event).transform;
            }
            else
            {
                if (((@event is PointerUpEvent) || (@event is PointerCancelEvent)))
                {
                    this._pointerLocations.remove(((PointerEvent)@event).pointer);
                    this._pointerQueue.Remove(((PointerEvent)@event).pointer);
                    didChangeConfiguration = true;
                    _lastTransform = ((PointerEvent)@event).transform;
                }
                else
                {
                    if ((@event is PointerPanZoomStartEvent))
                    {
                        DartRuntimePrimitives.Assert(() => (!this._pointerPanZooms.ContainsKey(((PointerPanZoomStartEvent)@event).pointer)));
                        this._pointerPanZooms[@event.pointer] = new _PointerPanZoomData__scale(this, ((PointerPanZoomStartEvent)@event));
                        didChangeConfiguration = true;
                        shouldStartIfAccepted = true;
                        _lastTransform = ((PointerPanZoomStartEvent)@event).transform;
                    }
                    else
                    {
                        if ((@event is PointerPanZoomUpdateEvent))
                        {
                            DartRuntimePrimitives.Assert(() => (this._pointerPanZooms.ContainsKey(((PointerPanZoomUpdateEvent)@event).pointer)));
                            if ((!((PointerPanZoomUpdateEvent)@event).synthesized && !this.trackpadScrollCausesScale))
                            {
                                this._velocityTrackers.GetValueOrDefault(((PointerPanZoomUpdateEvent)@event).pointer)!.addPosition(((PointerPanZoomUpdateEvent)@event).timeStamp, ((PointerPanZoomUpdateEvent)((PointerPanZoomUpdateEvent)@event)).pan);
                            }
                            this._pointerPanZooms[@event.pointer] = _PointerPanZoomData__scale.CreateFromUpdateEvent(this, ((PointerPanZoomUpdateEvent)@event));
                            _lastTransform = ((PointerPanZoomUpdateEvent)@event).transform;
                            shouldStartIfAccepted = true;
                        }
                        else
                        {
                            if ((@event is PointerPanZoomEndEvent))
                            {
                                PointerPanZoomEndEvent @event__as21439 = (PointerPanZoomEndEvent)@event;
                                DartRuntimePrimitives.Assert(() => (this._pointerPanZooms.ContainsKey(((PointerPanZoomEndEvent)@event__as21439).pointer)));
                                this._pointerPanZooms.remove(((PointerPanZoomEndEvent)@event__as21439).pointer);
                                didChangeConfiguration = true;
                            }
                        }
                    }
                }
            }
        }
        _updateLines();
        _update();
        if ((!didChangeConfiguration || _reconfigure(((PointerEvent)@event).pointer)))
        {
            _advanceStateMachine(shouldStartIfAccepted, @event);
        }
        stopTrackingIfPointerNoLongerDown(@event);
    }

    internal virtual void _update()
    {
        global::Doroti.Ui.Offset? previousFocalPoint = this._currentFocalPoint;
        global::Doroti.Ui.Offset focalPointLocal = Offset.zero;
        foreach (long pointer in this._pointerLocations.Keys)
        {
            focalPointLocal += DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointer));
        }
        foreach (_PointerPanZoomData__scale p in this._pointerPanZooms.Values)
        {
            focalPointLocal += ((_PointerPanZoomData__scale)p).focalPoint;
        }
        _currentFocalPoint = (focalPointLocal / Math.Max(1L, (checked((long)(this._pointerLocations.Count)) + checked((long)(this._pointerPanZooms.Count)))).toDouble());
        if ((previousFocalPoint is null))
        {
            _localFocalPoint = PointerEvent.transformPosition(this._lastTransform, DartRuntimePrimitives.RequireValue(this._currentFocalPoint));
            _delta = Offset.zero;
        }
        else
        {
            global::Doroti.Ui.Offset localPreviousFocalPoint = this._localFocalPoint;
            _localFocalPoint = PointerEvent.transformPosition(this._lastTransform, DartRuntimePrimitives.RequireValue(this._currentFocalPoint));
            _delta = (this._localFocalPoint - localPreviousFocalPoint);
        }
        long count = this._pointerLocations.Keys.Count();
        global::Doroti.Ui.Offset pointerFocalPoint = Offset.zero;
        foreach (long pointerLocal in this._pointerLocations.Keys)
        {
            pointerFocalPoint += DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointerLocal));
        }
        if ((count > 0L))
        {
            pointerFocalPoint = (pointerFocalPoint / count.toDouble());
        }
        var totalDeviation = 0.0;
        var totalHorizontalDeviation = 0.0;
        var totalVerticalDeviation = 0.0;
        foreach (long pointerAlternate in this._pointerLocations.Keys)
        {
            totalDeviation += ((pointerFocalPoint - DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointerAlternate)))).distance;
            totalHorizontalDeviation += ((pointerFocalPoint.dx - DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointerAlternate)).dx)).abs();
            totalVerticalDeviation += ((pointerFocalPoint.dy - DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointerAlternate)).dy)).abs();
        }
        _currentSpan = ((count > 0L) ? (totalDeviation / count) : 0.0);
        _currentHorizontalSpan = ((count > 0L) ? (totalHorizontalDeviation / count) : 0.0);
        _currentVerticalSpan = ((count > 0L) ? (totalVerticalDeviation / count) : 0.0);
    }

    internal virtual void _updateLines()
    {
        long count = this._pointerLocations.Keys.Count();
        DartRuntimePrimitives.Assert(() => (checked((long)(this._pointerQueue.Count)) >= count));
        if ((count < 2L))
        {
            _initialLine = this._currentLine;
        }
        else
        {
            if ((((this._initialLine is not null) && (this._initialLine!.pointerStartId == this._pointerQueue[(int)(0L)])) && (this._initialLine!.pointerEndId == this._pointerQueue[(int)(1L)])))
            {
                _currentLine = new _LineBetweenPointers__scale(pointerStartId: this._pointerQueue[(int)(0L)], pointerStartLocation: DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(this._pointerQueue[(int)(0L)])), pointerEndId: this._pointerQueue[(int)(1L)], pointerEndLocation: DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(this._pointerQueue[(int)(1L)])));
            }
            else
            {
                _initialLine = new _LineBetweenPointers__scale(pointerStartId: this._pointerQueue[(int)(0L)], pointerStartLocation: DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(this._pointerQueue[(int)(0L)])), pointerEndId: this._pointerQueue[(int)(1L)], pointerEndLocation: DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(this._pointerQueue[(int)(1L)])));
                _currentLine = this._initialLine;
            }
        }
    }

    internal virtual bool _reconfigure(long pointer)
    {
        _initialFocalPoint = DartRuntimePrimitives.RequireValue(this._currentFocalPoint);
        _initialSpan = this._currentSpan;
        _initialLine = this._currentLine;
        _initialHorizontalSpan = this._currentHorizontalSpan;
        _initialVerticalSpan = this._currentVerticalSpan;
        if ((checked((long)(this._pointerPanZooms.Count)) == 0))
        {
            _initialPanZoomScaleFactor = 1.0;
            _initialPanZoomRotationFactor = 0.0;
        }
        else
        {
            _initialPanZoomScaleFactor = (this._scaleFactor / this._pointerScaleFactor);
            _initialPanZoomRotationFactor = this._pointerPanZooms.Values.map<_PointerPanZoomData__scale, double>(((x) => ((_PointerPanZoomData__scale)x).rotation)).reduce(((a, b) => (a + b)));
        }
        if ((Equals(this._state, _ScaleState__scale.started)))
        {
            if ((this.onEnd is not null))
            {
                VelocityTracker tracker = this._velocityTrackers.GetValueOrDefault(pointer)!;
                Velocity velocityLocal = tracker.getVelocity();
                if (ScaleLibrary._isFlingGesture(velocityLocal))
                {
                    global::Doroti.Ui.Offset pixelsPerSecondLocal = ((Velocity)velocityLocal).pixelsPerSecond;
                    if ((pixelsPerSecondLocal.distanceSquared > (ConstantsLibrary.kMaxFlingVelocity * ConstantsLibrary.kMaxFlingVelocity)))
                    {
                        velocityLocal = new Velocity(pixelsPerSecond: (((pixelsPerSecondLocal / pixelsPerSecondLocal.distance)) * ConstantsLibrary.kMaxFlingVelocity));
                    }
                    invokeCallback<object?>("onEnd", () => { ((Action)((() => this.onEnd!(new ScaleEndDetails(velocity: velocityLocal, scaleVelocity: (this._scaleVelocityTracker?.getVelocity().pixelsPerSecond.dx ?? -1), pointerCount: this.pointerCount)))))(); return null; });
                }
                else
                {
                    invokeCallback<object?>("onEnd", () => { ((Action)((() => this.onEnd!(new ScaleEndDetails(scaleVelocity: (this._scaleVelocityTracker?.getVelocity().pixelsPerSecond.dx ?? -1), pointerCount: this.pointerCount)))))(); return null; });
                }
            }
            _state = _ScaleState__scale.accepted;
            _scaleVelocityTracker = new VelocityTracker(PointerDeviceKind.touch);
            return false;
        }
        _scaleVelocityTracker = new VelocityTracker(PointerDeviceKind.touch);
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _advanceStateMachine(bool shouldStartIfAccepted, PointerEvent @event)
    {
        if ((Equals(this._state, _ScaleState__scale.ready)))
        {
            _state = _ScaleState__scale.possible;
        }
        if ((Equals(this._state, _ScaleState__scale.possible)))
        {
            double spanDelta = ((this._currentSpan - this._initialSpan)).abs();
            double focalPointDeltaLocal = ((DartRuntimePrimitives.RequireValue(this._currentFocalPoint) - this._initialFocalPoint)).distance;
            if ((((spanDelta > EventsLibrary.computeScaleSlop(((PointerEvent)@event).kind)) || (focalPointDeltaLocal > EventsLibrary.computePanSlop(((PointerEvent)@event).kind, gestureSettings))) || (Math.Max((this._scaleFactor / this._pointerScaleFactor), (this._pointerScaleFactor / this._scaleFactor)) > 1.05)))
            {
                resolve(GestureDisposition.accepted);
            }
        }
        else
        {
            if ((FoundationRuntimePorts.EnumIndex(this._state) >= FoundationRuntimePorts.EnumIndex(_ScaleState__scale.accepted)))
            {
                resolve(GestureDisposition.accepted);
            }
        }
        if (((Equals(this._state, _ScaleState__scale.accepted)) && shouldStartIfAccepted))
        {
            _initialEventTimestamp = ((PointerEvent)@event).timeStamp;
            _state = _ScaleState__scale.started;
            _dispatchOnStartCallbackIfNeeded();
        }
        if ((Equals(this._state, _ScaleState__scale.started)))
        {
            this._scaleVelocityTracker?.addPosition(((PointerEvent)@event).timeStamp, new global::Doroti.Ui.Offset(this._scaleFactor, 0));
            if ((this.onUpdate is not null))
            {
                invokeCallback<object?>("onUpdate", () =>
                {
                    ((Action)((() =>
                    {
                        this.onUpdate!(new ScaleUpdateDetails(scale: this._scaleFactor, horizontalScale: this._horizontalScaleFactor, verticalScale: this._verticalScaleFactor, focalPoint: DartRuntimePrimitives.RequireValue(this._currentFocalPoint), localFocalPoint: this._localFocalPoint, rotation: _computeRotationFactor(), pointerCount: this.pointerCount, focalPointDelta: this._delta, sourceTimeStamp: ((PointerEvent)@event).timeStamp));
                    })))(); return null;
                });
            }
        }
    }

    internal virtual void _dispatchOnStartCallbackIfNeeded()
    {
        DartRuntimePrimitives.Assert(() => (Equals(this._state, _ScaleState__scale.started)));
        if ((this.onStart is not null))
        {
            invokeCallback<object?>("onStart", () =>
            {
                ((Action)((() =>
                {
                    this.onStart!(new ScaleStartDetails(focalPoint: DartRuntimePrimitives.RequireValue(this._currentFocalPoint), localFocalPoint: this._localFocalPoint, pointerCount: this.pointerCount, sourceTimeStamp: this._initialEventTimestamp, kind: ((checked((long)(this._pointerQueue.Count)) != 0) ? getKindForPointer(this._pointerQueue.First()) : ((checked((long)(this._pointerPanZooms.Count)) != 0) ? getKindForPointer(this._pointerPanZooms.Keys.First()) : null))));
                })))(); return null;
            });
        }
        _initialEventTimestamp = null;
    }

    public override void acceptGesture(long pointer)
    {
        if ((Equals(this._state, _ScaleState__scale.possible)))
        {
            _state = _ScaleState__scale.started;
            _dispatchOnStartCallbackIfNeeded();
            if ((Equals(this.dragStartBehavior, DragStartBehavior.start)))
            {
                _initialFocalPoint = DartRuntimePrimitives.RequireValue(this._currentFocalPoint);
                _initialSpan = this._currentSpan;
                _initialLine = this._currentLine;
                _initialHorizontalSpan = this._currentHorizontalSpan;
                _initialVerticalSpan = this._currentVerticalSpan;
                if ((checked((long)(this._pointerPanZooms.Count)) == 0))
                {
                    _initialPanZoomScaleFactor = 1.0;
                    _initialPanZoomRotationFactor = 0.0;
                }
                else
                {
                    _initialPanZoomScaleFactor = (this._scaleFactor / this._pointerScaleFactor);
                    _initialPanZoomRotationFactor = this._pointerPanZooms.Values.map<_PointerPanZoomData__scale, double>(((x) => ((_PointerPanZoomData__scale)x).rotation)).reduce(((a, b) => (a + b)));
                }
            }
        }
    }

    public override void rejectGesture(long pointer)
    {
        this._pointerPanZooms.remove(pointer);
        this._pointerLocations.remove(pointer);
        this._pointerQueue.Remove(pointer);
        stopTrackingPointer(pointer);
    }

    public override void didStopTrackingLastPointer(long pointer)
    {
        switch (this._state)
        {
            case _ScaleState__scale.possible:
                {
                    resolve(GestureDisposition.rejected);
                    break;
                }
            case _ScaleState__scale.ready:
                {
                    DartRuntimePrimitives.Assert(() => false);
                    break;
                }
            case _ScaleState__scale.accepted:
                {
                    break;
                }
            case _ScaleState__scale.started:
                {
                    DartRuntimePrimitives.Assert(() => false);
                    break;
                }
        }
        _state = _ScaleState__scale.ready;
    }

    public override void dispose()
    {
        this._velocityTrackers.Clear();
        base.dispose();
    }

    public override string debugDescription => "scale";
}
