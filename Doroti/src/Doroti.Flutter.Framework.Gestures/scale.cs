// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/gestures/scale.dart
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

public static partial class ScaleLibrary
{
    public static double kDefaultMouseScrollToScaleFactor = 200;
}

public static partial class ScaleLibrary
{
    public static Offset kDefaultTrackpadScrollToScaleFactor = new global::Doroti.Flutter.Ui.Offset(0, (-1L / ScaleLibrary.kDefaultMouseScrollToScaleFactor));
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

    internal _PointerPanZoomData__scale(ScaleGestureRecognizer parent, PointerPanZoomStartEvent @event)
    {
        this.parent = parent;
        this._position = @event.position;
        this._pan = Offset.zero;
        this._scale = 1;
        this._rotation = 0;
    }

    internal static _PointerPanZoomData__scale CreateFromUpdateEvent(ScaleGestureRecognizer parent, PointerPanZoomUpdateEvent @event)
    {
        var __instance = new _PointerPanZoomData__scale(default!, default!);
        __instance.parent = parent;
        __instance._position = @event.position;
        __instance._pan = ((PointerPanZoomUpdateEvent)@event).pan;
        __instance._scale = ((PointerPanZoomUpdateEvent)@event).scale;
        __instance._rotation = ((PointerPanZoomUpdateEvent)@event).rotation;
        return __instance;
    }

    public virtual global::Doroti.Flutter.Ui.Offset focalPoint
    {
        get
        {
            if (((ScaleGestureRecognizer)this.parent).trackpadScrollCausesScale)
            {
                return this._position;
            }
            return (this._position + this._pan);
            return default!;
        }
    }
    public virtual double scale
    {
        get
        {
            if (((ScaleGestureRecognizer)this.parent).trackpadScrollCausesScale)
            {
                return (this._scale * global::Doroti.Flutter.Runtime.Dart_mathLibrary.exp((((this._pan.dx * ((ScaleGestureRecognizer)this.parent).trackpadScrollToScaleFactor.dx)) + ((this._pan.dy * ((ScaleGestureRecognizer)this.parent).trackpadScrollToScaleFactor.dy)))));
            }
            return this._scale;
            return default!;
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
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("focalPoint", this.focalPoint));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("localFocalPoint", this.localFocalPoint));
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
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("focalPointDelta", this.focalPointDelta));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("focalPoint", this.focalPoint));
        properties.add(new DiagnosticsProperty<global::Doroti.Flutter.Ui.Offset>("localFocalPoint", this.localFocalPoint));
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
        double speedSquared__10727 = ((Velocity)velocity).pixelsPerSecond.distanceSquared;
        return (speedSquared__10727 > (global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity * global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMinFlingVelocity));
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

    public ScaleGestureRecognizer(object? debugOwner = null, HashSet<PointerDeviceKind>? supportedDevices = null, Func<long, bool> allowedButtonsFilter = default!, DragStartBehavior dragStartBehavior = DragStartBehavior.down, bool trackpadScrollCausesScale = false, Offset? trackpadScrollToScaleFactor = null) : base(debugOwner: debugOwner, supportedDevices: supportedDevices, allowedButtonsFilter: allowedButtonsFilter ?? GestureRecognizer._defaultButtonAcceptBehavior)
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
            return default!;
        }
    }
    internal virtual double _pointerScaleFactor => ((this._initialSpan > 0.0) ? (this._currentSpan / this._initialSpan) : 1.0);
    internal virtual double _pointerHorizontalScaleFactor => ((this._initialHorizontalSpan > 0.0) ? (this._currentHorizontalSpan / this._initialHorizontalSpan) : 1.0);
    internal virtual double _pointerVerticalScaleFactor => ((this._initialVerticalSpan > 0.0) ? (this._currentVerticalSpan / this._initialVerticalSpan) : 1.0);
    internal virtual double _scaleFactor
    {
        get
        {
            double scale__17025 = this._pointerScaleFactor;
            foreach (_PointerPanZoomData__scale p__17089 in this._pointerPanZooms.Values)
            {
                scale__17025 *= (((_PointerPanZoomData__scale)p__17089).scale / this._initialPanZoomScaleFactor);
            }
            return scale__17025;
            return default!;
        }
    }
    internal virtual double _horizontalScaleFactor
    {
        get
        {
            double scale__17252 = this._pointerHorizontalScaleFactor;
            foreach (_PointerPanZoomData__scale p__17326 in this._pointerPanZooms.Values)
            {
                scale__17252 *= (((_PointerPanZoomData__scale)p__17326).scale / this._initialPanZoomScaleFactor);
            }
            return scale__17252;
            return default!;
        }
    }
    internal virtual double _verticalScaleFactor
    {
        get
        {
            double scale__17487 = this._pointerVerticalScaleFactor;
            foreach (_PointerPanZoomData__scale p__17559 in this._pointerPanZooms.Values)
            {
                scale__17487 *= (((_PointerPanZoomData__scale)p__17559).scale / this._initialPanZoomScaleFactor);
            }
            return scale__17487;
            return default!;
        }
    }
    internal virtual double _computeRotationFactor()
    {
        var factor__17717 = 0.0;
        if (((this._initialLine is not null) && (this._currentLine is not null)))
        {
            double fx__17806 = this._initialLine!.pointerStartLocation.dx;
            double fy__17869 = this._initialLine!.pointerStartLocation.dy;
            double sx__17932 = this._initialLine!.pointerEndLocation.dx;
            double sy__17993 = this._initialLine!.pointerEndLocation.dy;
            double nfx__18055 = this._currentLine!.pointerStartLocation.dx;
            double nfy__18119 = this._currentLine!.pointerStartLocation.dy;
            double nsx__18183 = this._currentLine!.pointerEndLocation.dx;
            double nsy__18245 = this._currentLine!.pointerEndLocation.dy;
            double angle1__18308 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.atan2((fy__17869 - sy__17993), (fx__17806 - sx__17932));
            double angle2__18366 = global::Doroti.Flutter.Runtime.Dart_mathLibrary.atan2((nfy__18119 - nsy__18245), (nfx__18055 - nsx__18183));
            factor__17717 = (angle2__18366 - angle1__18308);
        }
        foreach (_PointerPanZoomData__scale p__18483 in this._pointerPanZooms.Values)
        {
            factor__17717 += ((_PointerPanZoomData__scale)p__18483).rotation;
        }
        factor__17717 -= this._initialPanZoomRotationFactor;
        return factor__17717;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void addAllowedPointer(PointerDownEvent @event)
    {
        base.addAllowedPointer(@event);
        this._velocityTrackers[@event.pointer] = new VelocityTracker(@event.kind);
        _initialEventTimestamp = @event.timeStamp;
        if ((object.Equals(this._state, _ScaleState__scale.ready)))
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
        if ((object.Equals(this._state, _ScaleState__scale.ready)))
        {
            _state = _ScaleState__scale.possible;
            _initialPanZoomScaleFactor = 1.0;
            _initialPanZoomRotationFactor = 0.0;
        }
    }

    public override void handleEvent(PointerEvent @event)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._state, _ScaleState__scale.ready)));
        var didChangeConfiguration__19778 = false;
        var shouldStartIfAccepted__19818 = false;
        if ((@event is PointerMoveEvent))
        {
            VelocityTracker tracker__19914 = this._velocityTrackers.GetValueOrDefault(((PointerMoveEvent)@event).pointer)!;
            if (!((PointerMoveEvent)@event).synthesized)
            {
                tracker__19914.addPosition(((PointerMoveEvent)@event).timeStamp, ((PointerMoveEvent)@event).position);
            }
            this._pointerLocations[@event.pointer] = ((PointerMoveEvent)@event).position;
            shouldStartIfAccepted__19818 = true;
            _lastTransform = ((PointerMoveEvent)@event).transform;
        }
        else
        {
            if ((@event is PointerDownEvent))
            {
                this._pointerLocations[@event.pointer] = ((PointerDownEvent)@event).position;
                this._pointerQueue.Add(((PointerDownEvent)@event).pointer);
                didChangeConfiguration__19778 = true;
                shouldStartIfAccepted__19818 = true;
                _lastTransform = ((PointerDownEvent)@event).transform;
            }
            else
            {
                if (((@event is PointerUpEvent) || (@event is PointerCancelEvent)))
                {
                    this._pointerLocations.remove(((PointerEvent)@event).pointer);
                    this._pointerQueue.Remove(((PointerEvent)@event).pointer);
                    didChangeConfiguration__19778 = true;
                    _lastTransform = ((PointerEvent)@event).transform;
                }
                else
                {
                    if ((@event is PointerPanZoomStartEvent))
                    {
                        DartRuntimePrimitives.Assert(() => (!this._pointerPanZooms.ContainsKey(((PointerPanZoomStartEvent)@event).pointer)));
                        this._pointerPanZooms[@event.pointer] = new _PointerPanZoomData__scale(this, ((PointerPanZoomStartEvent)@event));
                        didChangeConfiguration__19778 = true;
                        shouldStartIfAccepted__19818 = true;
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
                            shouldStartIfAccepted__19818 = true;
                        }
                        else
                        {
                            if ((@event is PointerPanZoomEndEvent))
                            {
                                PointerPanZoomEndEvent @event__as21439 = (PointerPanZoomEndEvent)@event;
                                DartRuntimePrimitives.Assert(() => (this._pointerPanZooms.ContainsKey(((PointerPanZoomEndEvent)@event__as21439).pointer)));
                                this._pointerPanZooms.remove(((PointerPanZoomEndEvent)@event__as21439).pointer);
                                didChangeConfiguration__19778 = true;
                            }
                        }
                    }
                }
            }
        }
        _updateLines();
        _update();
        if ((!didChangeConfiguration__19778 || _reconfigure(((PointerEvent)@event).pointer)))
        {
            _advanceStateMachine(shouldStartIfAccepted__19818, @event);
        }
        stopTrackingIfPointerNoLongerDown(@event);
    }

    internal virtual void _update()
    {
        global::Doroti.Flutter.Ui.Offset? previousFocalPoint__21873 = this._currentFocalPoint;
        global::Doroti.Flutter.Ui.Offset focalPoint__21957 = Offset.zero;
        foreach (long pointer__22002 in this._pointerLocations.Keys)
        {
            focalPoint__21957 += DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointer__22002));
        }
        foreach (_PointerPanZoomData__scale p__22129 in this._pointerPanZooms.Values)
        {
            focalPoint__21957 += ((_PointerPanZoomData__scale)p__22129).focalPoint;
        }
        _currentFocalPoint = (focalPoint__21957 / Math.Max(1L, (checked((long)(this._pointerLocations.Count)) + checked((long)(this._pointerPanZooms.Count)))).toDouble());
        if ((previousFocalPoint__21873 is null))
        {
            _localFocalPoint = PointerEvent.transformPosition(this._lastTransform, DartRuntimePrimitives.RequireValue(this._currentFocalPoint));
            _delta = Offset.zero;
        }
        else
        {
            global::Doroti.Flutter.Ui.Offset localPreviousFocalPoint__22516 = this._localFocalPoint;
            _localFocalPoint = PointerEvent.transformPosition(this._lastTransform, DartRuntimePrimitives.RequireValue(this._currentFocalPoint));
            _delta = (this._localFocalPoint - localPreviousFocalPoint__22516);
        }
        long count__22734 = this._pointerLocations.Keys.Count();
        global::Doroti.Flutter.Ui.Offset pointerFocalPoint__22785 = Offset.zero;
        foreach (long pointer__22837 in this._pointerLocations.Keys)
        {
            pointerFocalPoint__22785 += DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointer__22837));
        }
        if ((count__22734 > 0L))
        {
            pointerFocalPoint__22785 = (pointerFocalPoint__22785 / count__22734.toDouble());
        }
        var totalDeviation__23236 = 0.0;
        var totalHorizontalDeviation__23266 = 0.0;
        var totalVerticalDeviation__23306 = 0.0;
        foreach (long pointer__23355 in this._pointerLocations.Keys)
        {
            totalDeviation__23236 += ((pointerFocalPoint__22785 - DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointer__23355)))).distance;
            totalHorizontalDeviation__23266 += ((pointerFocalPoint__22785.dx - DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointer__23355)).dx)).abs();
            totalVerticalDeviation__23306 += ((pointerFocalPoint__22785.dy - DartRuntimePrimitives.RequireValue(this._pointerLocations.GetValueOrDefault(pointer__23355)).dy)).abs();
        }
        _currentSpan = ((count__22734 > 0L) ? (totalDeviation__23236 / count__22734) : 0.0);
        _currentHorizontalSpan = ((count__22734 > 0L) ? (totalHorizontalDeviation__23266 / count__22734) : 0.0);
        _currentVerticalSpan = ((count__22734 > 0L) ? (totalVerticalDeviation__23306 / count__22734) : 0.0);
    }

    internal virtual void _updateLines()
    {
        long count__24047 = this._pointerLocations.Keys.Count();
        DartRuntimePrimitives.Assert(() => (checked((long)(this._pointerQueue.Count)) >= count__24047));
        if ((count__24047 < 2L))
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
        if ((object.Equals(this._state, _ScaleState__scale.started)))
        {
            if ((this.onEnd is not null))
            {
                VelocityTracker tracker__25857 = this._velocityTrackers.GetValueOrDefault(pointer)!;
                Velocity velocity__25914 = tracker__25857.getVelocity();
                if (ScaleLibrary._isFlingGesture(velocity__25914))
                {
                    global::Doroti.Flutter.Ui.Offset pixelsPerSecond__26012 = ((Velocity)velocity__25914).pixelsPerSecond;
                    if ((pixelsPerSecond__26012.distanceSquared > (global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity * global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity)))
                    {
                        velocity__25914 = new Velocity(pixelsPerSecond: (((pixelsPerSecond__26012 / pixelsPerSecond__26012.distance)) * global::Doroti.Generated.Framework.Gestures.ConstantsLibrary.kMaxFlingVelocity));
                    }
                    invokeCallback<object?>("onEnd", () => { ((Action)((() => this.onEnd!(new ScaleEndDetails(velocity: velocity__25914, scaleVelocity: (this._scaleVelocityTracker?.getVelocity().pixelsPerSecond.dx ?? -1), pointerCount: this.pointerCount)))))(); return null; });
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
        if ((object.Equals(this._state, _ScaleState__scale.ready)))
        {
            _state = _ScaleState__scale.possible;
        }
        if ((object.Equals(this._state, _ScaleState__scale.possible)))
        {
            double spanDelta__27518 = ((this._currentSpan - this._initialSpan)).abs();
            double focalPointDelta__27586 = ((DartRuntimePrimitives.RequireValue(this._currentFocalPoint) - this._initialFocalPoint)).distance;
            if ((((spanDelta__27518 > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computeScaleSlop(((PointerEvent)@event).kind)) || (focalPointDelta__27586 > global::Doroti.Generated.Framework.Gestures.EventsLibrary.computePanSlop(((PointerEvent)@event).kind, gestureSettings))) || (Math.Max((this._scaleFactor / this._pointerScaleFactor), (this._pointerScaleFactor / this._scaleFactor)) > 1.05)))
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
        if (((object.Equals(this._state, _ScaleState__scale.accepted)) && shouldStartIfAccepted))
        {
            _initialEventTimestamp = ((PointerEvent)@event).timeStamp;
            _state = _ScaleState__scale.started;
            _dispatchOnStartCallbackIfNeeded();
        }
        if ((object.Equals(this._state, _ScaleState__scale.started)))
        {
            this._scaleVelocityTracker?.addPosition(((PointerEvent)@event).timeStamp, new global::Doroti.Flutter.Ui.Offset(this._scaleFactor, 0));
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
        DartRuntimePrimitives.Assert(() => (object.Equals(this._state, _ScaleState__scale.started)));
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
        if ((object.Equals(this._state, _ScaleState__scale.possible)))
        {
            _state = _ScaleState__scale.started;
            _dispatchOnStartCallbackIfNeeded();
            if ((object.Equals(this.dragStartBehavior, DragStartBehavior.start)))
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

