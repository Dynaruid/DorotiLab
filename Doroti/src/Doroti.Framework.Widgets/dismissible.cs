// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/dismissible.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public static partial class DismissibleLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kResizeTimeCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.4, 1.0, curve: global::Doroti.Framework.Animation.Curves.ease));
}

public static partial class DismissibleLibrary
{
    internal static double _kMinFlingVelocity = 700.0;
}

public static partial class DismissibleLibrary
{
    internal static double _kMinFlingVelocityDelta = 400.0;
}

public static partial class DismissibleLibrary
{
    internal static double _kFlingVelocityScale = (1.0 / 300.0);
}

public static partial class DismissibleLibrary
{
    internal static double _kDismissThreshold = 0.4;
}

public delegate void DismissDirectionCallback(DismissDirection direction);

public delegate Future<bool?> ConfirmDismissCallback(DismissDirection direction);

public delegate void DismissUpdateCallback(DismissUpdateDetails details);

public enum DismissDirection
{
    vertical,
    horizontal,
    endToStart,
    startToEnd,
    up,
    down,
    none
}

public class Dismissible : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual Widget? background { get; private set; }
    public virtual Widget? secondaryBackground { get; private set; }
    public virtual global::System.Func<DismissDirection, Future<bool?>>? confirmDismiss { get; private set; }
    public virtual global::System.Action? onResize { get; private set; }
    public virtual global::System.Action<DismissDirection>? onDismissed { get; private set; }
    public virtual DismissDirection direction { get; private set; } = default!;
    public virtual Duration? resizeDuration { get; private set; }
    public virtual DartMap<DismissDirection, double> dismissThresholds { get; private set; } = default!;
    public virtual Duration movementDuration { get; private set; } = default!;
    public virtual double crossAxisEndOffset { get; private set; } = default!;
    public virtual global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.HitTestBehavior behavior { get; private set; } = default!;
    public virtual global::System.Action<DismissUpdateDetails>? onUpdate { get; private set; }

    public Dismissible(global::Doroti.Framework.Foundation.Key key, Widget child, Widget? background = null, Widget? secondaryBackground = null, global::System.Func<DismissDirection, Future<bool?>>? confirmDismiss = null, global::System.Action? onResize = null, global::System.Action<DismissUpdateDetails>? onUpdate = null, global::System.Action<DismissDirection>? onDismissed = null, DismissDirection direction = DismissDirection.horizontal, Duration? resizeDuration = null, DartMap<DismissDirection, double> dismissThresholds = default!, Duration? movementDuration = null, double crossAxisEndOffset = 0.0, global::Doroti.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Framework.Gestures.DragStartBehavior.start, global::Doroti.Framework.Rendering.HitTestBehavior behavior = global::Doroti.Framework.Rendering.HitTestBehavior.opaque) : base(key: key)
    {
        Duration? __resizeDuration = resizeDuration ?? Duration.Create(milliseconds: 300);
        DartMap<DismissDirection, double> __dismissThresholds = dismissThresholds ?? new DartMap<DismissDirection, double>();
        Duration __movementDuration = movementDuration ?? Duration.Create(milliseconds: 200);
        this.child = child;
        this.background = background;
        this.secondaryBackground = secondaryBackground;
        this.confirmDismiss = confirmDismiss;
        this.onResize = onResize;
        this.onUpdate = onUpdate;
        this.onDismissed = onDismissed;
        this.direction = direction;
        this.resizeDuration = __resizeDuration;
        this.dismissThresholds = __dismissThresholds;
        this.movementDuration = __movementDuration;
        this.crossAxisEndOffset = crossAxisEndOffset;
        this.dragStartBehavior = dragStartBehavior;
        this.behavior = behavior;
        System.Diagnostics.Debug.Assert(((secondaryBackground is null) || (background is not null)));
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _DismissibleState__dismissible());
}

public class DismissUpdateDetails
{
    public virtual DismissDirection direction { get; private set; } = default!;
    public virtual bool reached { get; private set; } = default!;
    public virtual bool previousReached { get; private set; } = default!;
    public virtual double progress { get; private set; } = default!;

    public DismissUpdateDetails(DismissDirection direction = DismissDirection.horizontal, bool reached = false, bool previousReached = false, double progress = 0.0)
    {
        this.direction = direction;
        this.reached = reached;
        this.previousReached = previousReached;
        this.progress = progress;
    }

}

internal class _DismissibleClipper__dismissible : global::Doroti.Framework.Rendering.CustomClipper<Rect>
{
    public virtual global::Doroti.Framework.Painting.Axis axis { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<Offset> moveAnimation { get; private set; } = default!;

    internal _DismissibleClipper__dismissible(global::Doroti.Framework.Painting.Axis axis, global::Doroti.Framework.Animation.Animation<Offset> moveAnimation) : base(reclip: moveAnimation)
    {
        this.axis = axis;
        this.moveAnimation = moveAnimation;
    }

    public override Rect getClip(Size size)
    {
        switch (this.axis)
        {
            case global::Doroti.Framework.Painting.Axis.horizontal:
                {
                    double offset = (((global::Doroti.Framework.Animation.Animation<Offset>)this.moveAnimation).value.dx * size.width);
                    if ((offset < 0L))
                    {
                        return global::Doroti.Ui.Rect.fromLTRB((size.width + offset), 0.0, size.width, size.height);
                    }
                    return global::Doroti.Ui.Rect.fromLTRB(0.0, 0.0, offset, size.height);
                }
            case global::Doroti.Framework.Painting.Axis.vertical:
                {
                    double offsetLocal = (((global::Doroti.Framework.Animation.Animation<Offset>)this.moveAnimation).value.dy * size.height);
                    if ((offsetLocal < 0L))
                    {
                        return global::Doroti.Ui.Rect.fromLTRB(0.0, (size.height + offsetLocal), size.width, size.height);
                    }
                    return global::Doroti.Ui.Rect.fromLTRB(0.0, 0.0, size.width, offsetLocal);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Rect getApproximateClipRect(Size size) => getClip(size);
    public override bool shouldReclip(global::Doroti.Framework.Rendering.CustomClipper<Rect> oldClipper)
    {
        var __oldClipper = (_DismissibleClipper__dismissible)(object)oldClipper;
        return ((!object.Equals(((_DismissibleClipper__dismissible)__oldClipper).axis, this.axis)) || (!object.Equals(((_DismissibleClipper__dismissible)__oldClipper).moveAnimation.value, ((global::Doroti.Framework.Animation.Animation<Offset>)this.moveAnimation).value)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal enum _FlingGestureKind__dismissible
{
    none,
    forward,
    reverse
}

internal class _DismissibleState__dismissible : State<Dismissible>, TickerProviderStateMixin<Dismissible>, AutomaticKeepAliveClientMixin<Dismissible>
{
    private bool __late__moveController_initialized;
    private global::Doroti.Framework.Animation.AnimationController __late__moveController = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _moveController
    {
        get
        {
            if (!__late__moveController_initialized)
            {
                __late__moveController = new global::Doroti.Framework.Animation.AnimationController(duration: ((Dismissible)this.widget).movementDuration, vsync: this);
                __late__moveController_initialized = true;
            }
            return __late__moveController;
        }
    }
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _moveAnimation { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController? _resizeController { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<double>? _resizeAnimation { get; set; } = default;
    internal virtual double _dragExtent { get; set; } = 0.0;
    internal virtual bool _confirming { get; set; } = false;
    internal virtual bool _dragUnderway { get; set; } = false;
    internal virtual Size? _sizePriorToCollapse { get; set; } = default;
    internal virtual bool _dismissThresholdReached { get; set; } = false;
    internal virtual GlobalKey<IState> _contentKey { get; private set; } = GlobalKey<IState>.Create();
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual KeepAliveHandle? _keepAliveHandle { get; set; } = default;

    public override void initState()
    {
        base.initState();
        if (this.wantKeepAlive)
        {
            _ensureKeepAlive();
        }
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = this._moveController;
    __cascade.addStatusListener((global::Doroti.Framework.Animation.AnimationStatus __status) => { _ = this._handleDismissStatusChanged(__status); });
    __cascade.addListener(() => this._handleDismissUpdateValueChanged());
    return __cascade;
}))());
        _updateMoveAnimation();
    }

    public virtual bool wantKeepAlive => DartRuntimePrimitives.ConvertValue<bool>((((global::Doroti.Framework.Animation.AnimationController)this._moveController).isAnimating || ((this._resizeController?.isAnimating ?? false))));
    public override void dispose()
    {
        this._moveController.dispose();
        this._resizeController?.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual bool _directionIsXAxis
    {
        get
        {
            return (((object.Equals(((Dismissible)this.widget).direction, DismissDirection.horizontal)) || (object.Equals(((Dismissible)this.widget).direction, DismissDirection.endToStart))) || (object.Equals(((Dismissible)this.widget).direction, DismissDirection.startToEnd)));
            return default!;
        }
    }
    internal virtual DismissDirection _extentToDirection(double extent)
    {
        if ((extent == 0.0))
        {
            return DismissDirection.none;
        }
        if (this._directionIsXAxis)
        {
            return (Directionality.of(this.context) switch { TextDirection.rtl when ((extent < 0L)) => DismissDirection.startToEnd, TextDirection.ltr when ((extent > 0L)) => DismissDirection.startToEnd, TextDirection.rtl => DismissDirection.endToStart, TextDirection.ltr => DismissDirection.endToStart, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        }
        return ((extent > 0L) ? DismissDirection.down : DismissDirection.up);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual DismissDirection _dismissDirection => _extentToDirection(this._dragExtent);
    internal virtual double _dismissThreshold => DartRuntimePrimitives.ConvertValue<double>((DartCollectionRuntime.NullableMapValue<double>(((Dismissible)this.widget).dismissThresholds, this._dismissDirection) ?? DismissibleLibrary._kDismissThreshold));
    internal virtual double _overallDragAxisExtent
    {
        get
        {
            global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)DartRuntimePrimitives.RequireValue(((BuildContext)this.context).size));
            return (this._directionIsXAxis ? sizeLocal.width : sizeLocal.height);
            return default!;
        }
    }
    internal virtual void _handleDragStart(global::Doroti.Framework.Gestures.DragStartDetails details)
    {
        if (this._confirming)
        {
            return;
        }
        _dragUnderway = true;
        if (((global::Doroti.Framework.Animation.AnimationController)this._moveController).isAnimating)
        {
            _dragExtent = ((((global::Doroti.Framework.Animation.AnimationController)this._moveController).value * this._overallDragAxisExtent) * Math.Sign(this._dragExtent));
            this._moveController.stop();
        }
        else
        {
            _dragExtent = 0.0;
            this._moveController.value = 0.0;
        }
        setState(((global::System.Action)(() =>
        {
            _updateMoveAnimation();
        })));
    }

    internal virtual void _handleDragUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails details)
    {
        if ((!this._dragUnderway || ((global::Doroti.Framework.Animation.AnimationController)this._moveController).isAnimating))
        {
            return;
        }
        double delta = DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Gestures.DragUpdateDetails)details).primaryDelta);
        double oldDragExtent = this._dragExtent;
        switch (((Dismissible)this.widget).direction)
        {
            case DismissDirection.horizontal:
            case DismissDirection.vertical:
                {
                    _dragExtent += delta;
                    break;
                }
            case DismissDirection.up:
                {
                    if (((this._dragExtent + delta) < 0L))
                    {
                        _dragExtent += delta;
                    }
                    break;
                }
            case DismissDirection.down:
                {
                    if (((this._dragExtent + delta) > 0L))
                    {
                        _dragExtent += delta;
                    }
                    break;
                }
            case DismissDirection.endToStart:
                {
                    switch (Directionality.of(this.context))
                    {
                        case TextDirection.rtl:
                            {
                                if (((this._dragExtent + delta) > 0L))
                                {
                                    _dragExtent += delta;
                                }
                                break;
                            }
                        case TextDirection.ltr:
                            {
                                if (((this._dragExtent + delta) < 0L))
                                {
                                    _dragExtent += delta;
                                }
                                break;
                            }
                    }
                    break;
                }
            case DismissDirection.startToEnd:
                {
                    switch (Directionality.of(this.context))
                    {
                        case TextDirection.rtl:
                            {
                                if (((this._dragExtent + delta) < 0L))
                                {
                                    _dragExtent += delta;
                                }
                                break;
                            }
                        case TextDirection.ltr:
                            {
                                if (((this._dragExtent + delta) > 0L))
                                {
                                    _dragExtent += delta;
                                }
                                break;
                            }
                    }
                    break;
                }
            case DismissDirection.none:
                {
                    _dragExtent = 0;
                    break;
                }
        }
        if ((Math.Sign(oldDragExtent) != Math.Sign(this._dragExtent)))
        {
            setState(((global::System.Action)(() =>
            {
                _updateMoveAnimation();
            })));
        }
        if (!((global::Doroti.Framework.Animation.AnimationController)this._moveController).isAnimating)
        {
            this._moveController.value = (this._dragExtent.abs() / this._overallDragAxisExtent);
        }
    }

    internal virtual void _handleDismissUpdateValueChanged()
    {
        if ((((Dismissible)this.widget).onUpdate is not null))
        {
            bool oldDismissThresholdReached = this._dismissThresholdReached;
            _dismissThresholdReached = (((global::Doroti.Framework.Animation.AnimationController)this._moveController).value > this._dismissThreshold);
            var details = new DismissUpdateDetails(direction: this._dismissDirection, reached: this._dismissThresholdReached, previousReached: oldDismissThresholdReached, progress: ((global::Doroti.Framework.Animation.AnimationController)this._moveController).value);
            ((Dismissible)this.widget).onUpdate!(details);
        }
    }

    internal virtual void _updateMoveAnimation()
    {
        double endLocal = Math.Sign(this._dragExtent);
        _moveAnimation = this._moveController.drive(new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: (this._directionIsXAxis ? new global::Doroti.Ui.Offset(endLocal, ((Dismissible)this.widget).crossAxisEndOffset) : new global::Doroti.Ui.Offset(((Dismissible)this.widget).crossAxisEndOffset, endLocal))));
    }

    internal virtual _FlingGestureKind__dismissible _describeFlingGesture(global::Doroti.Framework.Gestures.Velocity velocity)
    {
        if ((this._dragExtent == 0.0))
        {
            return _FlingGestureKind__dismissible.none;
        }
        double vx = ((global::Doroti.Framework.Gestures.Velocity)velocity).pixelsPerSecond.dx;
        double vy = ((global::Doroti.Framework.Gestures.Velocity)velocity).pixelsPerSecond.dy;
        DismissDirection flingDirection = default!;
        if (this._directionIsXAxis)
        {
            if ((((vx.abs() - vy.abs()) < DismissibleLibrary._kMinFlingVelocityDelta) || (vx.abs() < DismissibleLibrary._kMinFlingVelocity)))
            {
                return _FlingGestureKind__dismissible.none;
            }
            DartRuntimePrimitives.Assert(() => (vx != 0.0));
            flingDirection = _extentToDirection(vx);
        }
        else
        {
            if ((((vy.abs() - vx.abs()) < DismissibleLibrary._kMinFlingVelocityDelta) || (vy.abs() < DismissibleLibrary._kMinFlingVelocity)))
            {
                return _FlingGestureKind__dismissible.none;
            }
            DartRuntimePrimitives.Assert(() => (vy != 0.0));
            flingDirection = _extentToDirection(vy);
        }
        if ((object.Equals(flingDirection, this._dismissDirection)))
        {
            return _FlingGestureKind__dismissible.forward;
        }
        return _FlingGestureKind__dismissible.reverse;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _handleDragEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        if ((!this._dragUnderway || ((global::Doroti.Framework.Animation.AnimationController)this._moveController).isAnimating))
        {
            return;
        }
        _dragUnderway = false;
        if (this._moveController.isCompleted)
        {
            DartRuntimePrimitives.Ignore(_handleMoveCompleted());
            return;
        }
        double flingVelocity = (this._directionIsXAxis ? ((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dx : ((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity.pixelsPerSecond.dy);
        switch (_describeFlingGesture(((global::Doroti.Framework.Gestures.DragEndDetails)details).velocity))
        {
            case _FlingGestureKind__dismissible.forward:
                {
                    DartRuntimePrimitives.Assert(() => (this._dragExtent != 0.0));
                    DartRuntimePrimitives.Assert(() => !this._moveController.isDismissed);
                    if ((this._dismissThreshold >= 1.0))
                    {
                        this._moveController.reverse();
                        break;
                    }
                    _dragExtent = Math.Sign(flingVelocity);
                    this._moveController.fling(velocity: (flingVelocity.abs() * DismissibleLibrary._kFlingVelocityScale));
                    break;
                }
            case _FlingGestureKind__dismissible.reverse:
                {
                    DartRuntimePrimitives.Assert(() => (this._dragExtent != 0.0));
                    DartRuntimePrimitives.Assert(() => !this._moveController.isDismissed);
                    _dragExtent = Math.Sign(flingVelocity);
                    this._moveController.fling(velocity: (-flingVelocity.abs() * DismissibleLibrary._kFlingVelocityScale));
                    break;
                }
            case _FlingGestureKind__dismissible.none:
                {
                    if (!this._moveController.isDismissed)
                    {
                        if ((((global::Doroti.Framework.Animation.AnimationController)this._moveController).value > this._dismissThreshold))
                        {
                            this._moveController.forward();
                        }
                        else
                        {
                            this._moveController.reverse();
                        }
                    }
                    break;
                }
        }
    }

    internal async virtual Future _handleDismissStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if ((global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status) && !this._dragUnderway))
        {
            await _handleMoveCompleted();
        }
        if (this.mounted)
        {
            updateKeepAlive();
        }
    }

    internal async virtual Future _handleMoveCompleted()
    {
        if ((this._dismissThreshold >= 1.0))
        {
            this._moveController.reverse();
            return;
        }
        bool result = await _confirmStartResizeAnimation();
        if (this.mounted)
        {
            if (result)
            {
                _startResizeAnimation();
            }
            else
            {
                this._moveController.reverse();
            }
        }
    }

    internal async virtual Future<bool> _confirmStartResizeAnimation()
    {
        if ((((Dismissible)this.widget).confirmDismiss is not null))
        {
            _confirming = true;
            DismissDirection direction = this._dismissDirection;
            try
            {
                return (await ((Dismissible)this.widget).confirmDismiss!(direction) ?? false);
            }
            finally
            {
                _confirming = false;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _startResizeAnimation()
    {
        DartRuntimePrimitives.Assert(() => this._moveController.isCompleted);
        DartRuntimePrimitives.Assert(() => (this._resizeController is null));
        DartRuntimePrimitives.Assert(() => (this._sizePriorToCollapse is null));
        if ((((Dismissible)this.widget).resizeDuration is null))
        {
            if ((((Dismissible)this.widget).onDismissed is not null))
            {
                DismissDirection direction = this._dismissDirection;
                ((Dismissible)this.widget).onDismissed!(direction);
            }
        }
        else
        {
            _resizeController = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = new global::Doroti.Framework.Animation.AnimationController(duration: ((Dismissible)this.widget).resizeDuration, vsync: this);
    __cascade.addListener(() => this._handleResizeProgressChanged());
    __cascade.addStatusListener(((AnimationStatusListener)((status) => updateKeepAlive())));
    return __cascade;
}))();
            this._resizeController!.forward();
            setState(((global::System.Action)(() =>
            {
                _sizePriorToCollapse = ((BuildContext)this.context).size;
                _resizeAnimation = this._resizeController!.drive(new global::Doroti.Framework.Animation.CurveTween(curve: DismissibleLibrary._kResizeTimeCurve)).drive(new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0));
            })));
        }
    }

    internal virtual void _handleResizeProgressChanged()
    {
        if (this._resizeController!.isCompleted)
        {
            ((Dismissible)this.widget).onDismissed?.Invoke(this._dismissDirection);
        }
        else
        {
            ((Dismissible)this.widget).onResize?.Invoke();
        }
    }

    public override Widget build(BuildContext context)
    {
        if ((this.wantKeepAlive && (this._keepAliveHandle is null)))
        {
            _ensureKeepAlive();
        }
        DartRuntimePrimitives.Assert(() => (!this._directionIsXAxis || global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context)));
        Widget? backgroundLocal = ((Dismissible)this.widget).background;
        if ((((Dismissible)this.widget).secondaryBackground is not null))
        {
            DismissDirection directionLocal = this._dismissDirection;
            if (((object.Equals(directionLocal, DismissDirection.endToStart)) || (object.Equals(directionLocal, DismissDirection.up))))
            {
                backgroundLocal = ((Dismissible)this.widget).secondaryBackground;
            }
        }
        if ((this._resizeAnimation is not null))
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((!object.Equals(this._resizeAnimation!.status, global::Doroti.Framework.Animation.AnimationStatus.forward)))
                    {
                        DartRuntimePrimitives.Assert(() => this._resizeAnimation!.isCompleted);
                        throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("A dismissed Dismissible widget is still part of the tree."), new global::Doroti.Framework.Foundation.ErrorHint("Make sure to implement the onDismissed handler and to immediately remove the Dismissible " + "widget from the application once that handler has fired.") }));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return ((Widget)(object?)new SizeTransition(sizeFactor: this._resizeAnimation!, axis: (this._directionIsXAxis ? global::Doroti.Framework.Painting.Axis.vertical : global::Doroti.Framework.Painting.Axis.horizontal), child: new SizedBox(width: DartRuntimePrimitives.RequireValue(this._sizePriorToCollapse).width, height: DartRuntimePrimitives.RequireValue(this._sizePriorToCollapse).height, child: backgroundLocal)));
        }
        Widget content = ((Widget)(object?)new SlideTransition(position: this._moveAnimation, child: new KeyedSubtree(key: this._contentKey, child: ((Dismissible)this.widget).child)));
        if ((backgroundLocal is not null))
        {
            content = DartRuntimePrimitives.ConvertValue<Widget>(new Stack(children: new List<Widget> { content }));
        }
        if ((object.Equals(((Dismissible)this.widget).direction, DismissDirection.none)))
        {
            return content;
        }
        return ((Widget)(object?)new GestureDetector(onHorizontalDragStart: ((global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)(this._directionIsXAxis ? this._handleDragStart : null)), onHorizontalDragUpdate: ((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)(this._directionIsXAxis ? this._handleDragUpdate : null)), onHorizontalDragEnd: ((global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)(this._directionIsXAxis ? this._handleDragEnd : null)), onVerticalDragStart: ((global::System.Action<global::Doroti.Framework.Gestures.DragStartDetails>)(this._directionIsXAxis ? null : this._handleDragStart)), onVerticalDragUpdate: ((global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)(this._directionIsXAxis ? null : this._handleDragUpdate)), onVerticalDragEnd: ((global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)(this._directionIsXAxis ? null : this._handleDragEnd)), behavior: ((Dismissible)this.widget).behavior, dragStartBehavior: ((Dismissible)this.widget).dragStartBehavior, child: content));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

    public virtual void _ensureKeepAlive()
    {
        DartRuntimePrimitives.Assert(() => (this._keepAliveHandle is null));
        this._keepAliveHandle = new KeepAliveHandle();
        new KeepAliveNotification(this._keepAliveHandle!).dispatch(this.context);
    }

    public virtual void _releaseKeepAlive()
    {
        this._keepAliveHandle!.dispose();
        this._keepAliveHandle = null;
    }

    public virtual void updateKeepAlive()
    {
        if (this.wantKeepAlive)
        {
            if ((this._keepAliveHandle is null))
            {
                _ensureKeepAlive();
            }
        }
        else
        {
            if ((this._keepAliveHandle is not null))
            {
                _releaseKeepAlive();
            }
        }
    }

    public override void deactivate()
    {
        if ((this._keepAliveHandle is not null))
        {
            _releaseKeepAlive();
        }
        base.deactivate();
    }

}

