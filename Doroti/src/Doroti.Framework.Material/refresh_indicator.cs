// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/refresh_indicator.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class Refresh_indicatorLibrary
{
    internal static double _kDragContainerExtentPercentage = 0.25;
}

public static partial class Refresh_indicatorLibrary
{
    internal static double _kDragSizeFactorLimit = 1.5;
}

public static partial class Refresh_indicatorLibrary
{
    internal static Duration _kIndicatorSnapDuration = Duration.Create(milliseconds: 150L);
}

public static partial class Refresh_indicatorLibrary
{
    internal static Duration _kIndicatorScaleDuration = Duration.Create(milliseconds: 200L);
}

public delegate Future RefreshCallback();

public enum RefreshIndicatorStatus
{
    drag,
    armed,
    snap,
    refresh,
    done,
    canceled
}

public enum RefreshIndicatorTriggerMode
{
    anywhere,
    onEdge
}

internal enum _IndicatorType__refresh_indicator
{
    material,
    adaptive,
    noSpinner
}

public class RefreshIndicator : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual double displacement { get; private set; } = default!;
    public virtual double edgeOffset { get; private set; } = default!;
    public virtual global::System.Func<Future> onRefresh { get; private set; } = default!;
    public virtual global::System.Action<RefreshIndicatorStatus?>? onStatusChange { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> notificationPredicate { get; private set; } = default!;
    public virtual string? semanticsLabel { get; private set; }
    public virtual string? semanticsValue { get; private set; }
    public virtual double strokeWidth { get; private set; } = default!;
    internal virtual _IndicatorType__refresh_indicator _indicatorType { get; private set; } = default!;
    public virtual RefreshIndicatorTriggerMode triggerMode { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;

    public RefreshIndicator(global::Doroti.Framework.Foundation.Key? key = null, double displacement = 40.0, double edgeOffset = 0.0, global::System.Func<Future> onRefresh = default!, Color? color = null, Color? backgroundColor = null, global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> notificationPredicate = default!, string? semanticsLabel = null, string? semanticsValue = null, double? strokeWidth = null, RefreshIndicatorTriggerMode triggerMode = RefreshIndicatorTriggerMode.onEdge, double elevation = 2.0, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        double __strokeWidth = strokeWidth ?? RefreshProgressIndicator.defaultStrokeWidth;
        this.displacement = displacement;
        this.edgeOffset = edgeOffset;
        this.onRefresh = onRefresh;
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.notificationPredicate = __notificationPredicate;
        this.semanticsLabel = semanticsLabel;
        this.semanticsValue = semanticsValue;
        this.strokeWidth = __strokeWidth;
        this.triggerMode = triggerMode;
        this.elevation = elevation;
        this.child = child;
        _indicatorType = _IndicatorType__refresh_indicator.material;
        onStatusChange = null;
        System.Diagnostics.Debug.Assert(elevation >= 0.0);
    }

    public static RefreshIndicator CreateAdaptive(global::Doroti.Framework.Foundation.Key? key = null, double displacement = 40.0, double edgeOffset = 0.0, global::System.Func<Future> onRefresh = default!, Color? color = null, Color? backgroundColor = null, global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> notificationPredicate = default!, string? semanticsLabel = null, string? semanticsValue = null, double? strokeWidth = null, RefreshIndicatorTriggerMode triggerMode = RefreshIndicatorTriggerMode.onEdge, double elevation = 2.0, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        var __instance = new RefreshIndicator(key: key, displacement: displacement, edgeOffset: edgeOffset, onRefresh: onRefresh, color: color, backgroundColor: backgroundColor, notificationPredicate: notificationPredicate, semanticsLabel: semanticsLabel, semanticsValue: semanticsValue, strokeWidth: strokeWidth, triggerMode: triggerMode, elevation: elevation, child: child);
        global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        double __strokeWidth = strokeWidth ?? RefreshProgressIndicator.defaultStrokeWidth;
        __instance.displacement = displacement;
        __instance.edgeOffset = edgeOffset;
        __instance.onRefresh = onRefresh;
        __instance.color = color;
        __instance.backgroundColor = backgroundColor;
        __instance.notificationPredicate = __notificationPredicate;
        __instance.semanticsLabel = semanticsLabel;
        __instance.semanticsValue = semanticsValue;
        __instance.strokeWidth = __strokeWidth;
        __instance.triggerMode = triggerMode;
        __instance.elevation = elevation;
        __instance.child = child;
        __instance._indicatorType = _IndicatorType__refresh_indicator.adaptive;
        __instance.onStatusChange = null;
        return __instance;
    }

    public static RefreshIndicator CreateNoSpinner(global::Doroti.Framework.Foundation.Key? key = null, global::System.Func<Future> onRefresh = default!, global::System.Action<RefreshIndicatorStatus?>? onStatusChange = null, global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> notificationPredicate = default!, string? semanticsLabel = null, string? semanticsValue = null, RefreshIndicatorTriggerMode triggerMode = RefreshIndicatorTriggerMode.onEdge, double elevation = 2.0, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        var __instance = new RefreshIndicator(key: key, onRefresh: onRefresh, notificationPredicate: notificationPredicate, semanticsLabel: semanticsLabel, semanticsValue: semanticsValue, strokeWidth: RefreshProgressIndicator.defaultStrokeWidth, triggerMode: triggerMode, elevation: elevation, child: child);
        global::System.Func<global::Doroti.Framework.Widgets.ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        __instance.onRefresh = onRefresh;
        __instance.onStatusChange = onStatusChange;
        __instance.notificationPredicate = __notificationPredicate;
        __instance.semanticsLabel = semanticsLabel;
        __instance.semanticsValue = semanticsValue;
        __instance.triggerMode = triggerMode;
        __instance.elevation = elevation;
        __instance.child = child;
        __instance._indicatorType = _IndicatorType__refresh_indicator.noSpinner;
        __instance.displacement = 0.0;
        __instance.edgeOffset = 0.0;
        __instance.color = null;
        __instance.backgroundColor = null;
        __instance.strokeWidth = 0.0;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new RefreshIndicatorState());
}

public class RefreshIndicatorState : global::Doroti.Framework.Widgets.State<RefreshIndicator>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<RefreshIndicator>
{
    internal virtual global::Doroti.Framework.Animation.AnimationController _positionController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController _scaleController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _positionFactor { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _scaleFactor { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<double> _value { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animation<Color?> _valueColor { get; set; } = default!;
    internal virtual RefreshIndicatorStatus? _status { get; set; } = default;
    internal virtual Future _pendingRefreshFuture { get; set; } = default!;
    internal virtual bool? _isIndicatorAtTop { get; set; } = default;
    internal virtual double? _dragOffset { get; set; } = default;
    private bool __late__effectiveValueColor_initialized;
    private Color __late__effectiveValueColor = default!;
    internal virtual Color _effectiveValueColor
    {
        get
        {
            if (!__late__effectiveValueColor_initialized)
            {
                __late__effectiveValueColor = widget.color ?? Theme.of(context).colorScheme.primary;
                __late__effectiveValueColor_initialized = true;
            }
            return __late__effectiveValueColor;
        }
        set { __late__effectiveValueColor = value; __late__effectiveValueColor_initialized = true; }
    }
    internal static global::Doroti.Framework.Animation.Animatable<double> _threeQuarterTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.75);
    internal static global::Doroti.Framework.Animation.Animatable<double> _kDragSizeFactorLimitTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: Refresh_indicatorLibrary._kDragSizeFactorLimit);
    internal static global::Doroti.Framework.Animation.Animatable<double> _oneToZeroTween = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0);
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _positionController = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
        _positionFactor = _positionController.drive(_kDragSizeFactorLimitTween);
        _value = _positionController.drive(_threeQuarterTween);
        _scaleController = new global::Doroti.Framework.Animation.AnimationController(vsync: this);
        _scaleFactor = _scaleController.drive(_oneToZeroTween);
    }

    public override void didChangeDependencies()
    {
        _setupColorTween();
        base.didChangeDependencies();
    }

    public override void didUpdateWidget(RefreshIndicator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(oldWidget.color, widget.color))
        {
            _setupColorTween();
        }
    }

    public override void dispose()
    {
        _positionController.dispose();
        _scaleController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _setupColorTween()
    {
        _effectiveValueColor = widget.color ?? Theme.of(context).colorScheme.primary;
        global::Doroti.Ui.Color colorLocal = _effectiveValueColor;
        if (colorLocal.alpha == 0L)
        {
            _valueColor = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<Color?>>(new global::Doroti.Framework.Animation.AlwaysStoppedAnimation<global::Doroti.Ui.Color>(colorLocal));
        }
        else
        {
            _valueColor = _positionController.drive(new global::Doroti.Framework.Animation.ColorTween(begin: colorLocal.withAlpha(0L), end: colorLocal.withAlpha(colorLocal.alpha)).chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, 1.0 / Refresh_indicatorLibrary._kDragSizeFactorLimit))));
        }
    }

    internal virtual bool _shouldStart(global::Doroti.Framework.Widgets.ScrollNotification notification)
    {
        return ((notification is global::Doroti.Framework.Widgets.ScrollStartNotification) && (((global::Doroti.Framework.Widgets.ScrollStartNotification)notification).dragDetails is not null) || (notification is global::Doroti.Framework.Widgets.ScrollUpdateNotification) && (((global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification).dragDetails is not null) && Equals(widget.triggerMode, RefreshIndicatorTriggerMode.anywhere)) && (Equals(notification.metrics.axisDirection, AxisDirection.up) && (notification.metrics.extentAfter == 0.0) || Equals(notification.metrics.axisDirection, AxisDirection.down) && (notification.metrics.extentBefore == 0.0)) && (_status is null) && _start(notification.metrics.axisDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _handleScrollNotification(global::Doroti.Framework.Widgets.ScrollNotification notification)
    {
        if (!widget.notificationPredicate(notification))
        {
            return false;
        }
        if (_shouldStart(notification))
        {
            setState(() =>
            {
                _status = RefreshIndicatorStatus.drag;
                widget.onStatusChange?.Invoke(_status);
            });
            return false;
        }
        bool? indicatorAtTopNow = notification.metrics.axisDirection switch { AxisDirection.down => true, AxisDirection.up => true, AxisDirection.left => DartRuntimePrimitives.ConvertValue<bool>(null), AxisDirection.right => DartRuntimePrimitives.ConvertValue<bool>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        if (indicatorAtTopNow != _isIndicatorAtTop)
        {
            if (Equals(_status, RefreshIndicatorStatus.drag) || Equals(_status, RefreshIndicatorStatus.armed))
            {
                DartRuntimePrimitives.Ignore(_dismiss(RefreshIndicatorStatus.canceled));
            }
        }
        else
        {
            if (notification is global::Doroti.Framework.Widgets.ScrollUpdateNotification)
            {
                global::Doroti.Framework.Widgets.ScrollUpdateNotification notification__as16986 = (global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification;
                if (Equals(_status, RefreshIndicatorStatus.drag) || Equals(_status, RefreshIndicatorStatus.armed))
                {
                    if (Equals(notification__as16986.metrics.axisDirection, AxisDirection.down))
                    {
                        _dragOffset = DartRuntimePrimitives.RequireValue(_dragOffset) - DartRuntimePrimitives.RequireValue(notification__as16986.scrollDelta);
                    }
                    else
                    {
                        if (Equals(notification__as16986.metrics.axisDirection, AxisDirection.up))
                        {
                            _dragOffset = DartRuntimePrimitives.RequireValue(_dragOffset) + DartRuntimePrimitives.RequireValue(notification__as16986.scrollDelta);
                        }
                    }
                    _checkDragOffset(notification__as16986.metrics.viewportDimension);
                }
                if (Equals(_status, RefreshIndicatorStatus.armed) && (notification__as16986.dragDetails is null))
                {
                    _show();
                }
            }
            else
            {
                if (notification is global::Doroti.Framework.Widgets.OverscrollNotification)
                {
                    global::Doroti.Framework.Widgets.OverscrollNotification notification__as17855 = (global::Doroti.Framework.Widgets.OverscrollNotification)notification;
                    if (Equals(_status, RefreshIndicatorStatus.drag) || Equals(_status, RefreshIndicatorStatus.armed))
                    {
                        if (Equals(notification__as17855.metrics.axisDirection, AxisDirection.down))
                        {
                            _dragOffset = DartRuntimePrimitives.RequireValue(_dragOffset) - notification__as17855.overscroll;
                        }
                        else
                        {
                            if (Equals(notification__as17855.metrics.axisDirection, AxisDirection.up))
                            {
                                _dragOffset = DartRuntimePrimitives.RequireValue(_dragOffset) + notification__as17855.overscroll;
                            }
                        }
                        _checkDragOffset(notification__as17855.metrics.viewportDimension);
                    }
                }
                else
                {
                    if (notification is global::Doroti.Framework.Widgets.ScrollEndNotification)
                    {
                        global::Doroti.Framework.Widgets.ScrollEndNotification notification__as18368 = (global::Doroti.Framework.Widgets.ScrollEndNotification)notification;
                        switch (_status)
                        {
                            case RefreshIndicatorStatus.armed:
                                {
                                    if (_positionController.value < 1.0)
                                    {
                                        DartRuntimePrimitives.Ignore(_dismiss(RefreshIndicatorStatus.canceled));
                                    }
                                    else
                                    {
                                        _show();
                                    }
                                    break;
                                }
                            case RefreshIndicatorStatus.drag:
                                {
                                    DartRuntimePrimitives.Ignore(_dismiss(RefreshIndicatorStatus.canceled));
                                    break;
                                }
                            case RefreshIndicatorStatus.canceled:
                            case RefreshIndicatorStatus.done:
                            case RefreshIndicatorStatus.refresh:
                            case RefreshIndicatorStatus.snap:
                            case null:
                                {
                                    break;
                                }
                        }
                    }
                }
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _handleIndicatorNotification(global::Doroti.Framework.Widgets.OverscrollIndicatorNotification notification)
    {
        if ((notification.depth != 0L) || !notification.leading)
        {
            return false;
        }
        if (Equals(_status, RefreshIndicatorStatus.drag))
        {
            notification.disallowIndicator();
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _start(global::Doroti.Framework.Painting.AxisDirection direction)
    {
        DartRuntimePrimitives.Assert(() => _status is null);
        DartRuntimePrimitives.Assert(() => _isIndicatorAtTop is null);
        DartRuntimePrimitives.Assert(() => _dragOffset is null);
        switch (direction)
        {
            case AxisDirection.down:
            case AxisDirection.up:
                {
                    _isIndicatorAtTop = true;
                    break;
                }
            case AxisDirection.left:
            case AxisDirection.right:
                {
                    _isIndicatorAtTop = null;
                    return false;
                }
        }
        _dragOffset = 0.0;
        _scaleController.value = 0.0;
        _positionController.value = 0.0;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _checkDragOffset(double containerExtent)
    {
        DartRuntimePrimitives.Assert(() => Equals(_status, RefreshIndicatorStatus.drag) || Equals(_status, RefreshIndicatorStatus.armed));
        double newValue = DartRuntimePrimitives.RequireValue(_dragOffset) / (containerExtent * Refresh_indicatorLibrary._kDragContainerExtentPercentage);
        if (Equals(_status, RefreshIndicatorStatus.armed))
        {
            newValue = Math.Max(newValue, 1.0 / Refresh_indicatorLibrary._kDragSizeFactorLimit);
        }
        _positionController.value = Dart_uiLibrary.clampDouble(newValue, 0.0, 1.0);
        if (Equals(_status, RefreshIndicatorStatus.drag) && (_valueColor.value!.alpha == _effectiveValueColor.alpha))
        {
            _status = RefreshIndicatorStatus.armed;
            widget.onStatusChange?.Invoke(_status);
        }
    }

    internal async virtual Future _dismiss(RefreshIndicatorStatus newMode)
    {
        await Future.value();
        DartRuntimePrimitives.Assert(() => Equals(newMode, RefreshIndicatorStatus.canceled) || Equals(newMode, RefreshIndicatorStatus.done));
        setState(() =>
        {
            _status = newMode;
            widget.onStatusChange?.Invoke(_status);
        });
        switch (DartRuntimePrimitives.RequireValue(_status))
        {
            case RefreshIndicatorStatus.done:
                {
                    await _scaleController.animateTo(1.0, duration: Refresh_indicatorLibrary._kIndicatorScaleDuration);
                    break;
                }
            case RefreshIndicatorStatus.canceled:
                {
                    await _positionController.animateTo(0.0, duration: Refresh_indicatorLibrary._kIndicatorScaleDuration);
                    break;
                }
            case RefreshIndicatorStatus.armed:
            case RefreshIndicatorStatus.drag:
            case RefreshIndicatorStatus.refresh:
            case RefreshIndicatorStatus.snap:
                {
                    DartRuntimePrimitives.Assert(() => false);
                    break;
                }
        }
        if (mounted && Equals(_status, newMode))
        {
            _dragOffset = null;
            _isIndicatorAtTop = null;
            setState(() =>
            {
                _status = null;
            });
        }
    }

    internal virtual void _show()
    {
        DartRuntimePrimitives.Assert(() => !Equals(_status, RefreshIndicatorStatus.refresh));
        DartRuntimePrimitives.Assert(() => !Equals(_status, RefreshIndicatorStatus.snap));
        var completer = new Completer<object?>();
        DartRuntimePrimitives.Ignore(_pendingRefreshFuture = completer.future);
        _status = RefreshIndicatorStatus.snap;
        widget.onStatusChange?.Invoke(_status);
        DartRuntimePrimitives.Ignore(_positionController.animateTo(1.0 / Refresh_indicatorLibrary._kDragSizeFactorLimit, duration: Refresh_indicatorLibrary._kIndicatorSnapDuration).then((value) =>
        {
            if (mounted && Equals(_status, RefreshIndicatorStatus.snap))
            {
                setState(() =>
                {
                    _status = RefreshIndicatorStatus.refresh;
                });
                Future refreshResult = widget.onRefresh();
                DartRuntimePrimitives.Ignore(refreshResult.whenComplete(() =>
                {
                    if (mounted && Equals(_status, RefreshIndicatorStatus.refresh))
                    {
                        completer.complete();
                        DartRuntimePrimitives.Ignore(_dismiss(RefreshIndicatorStatus.done));
                    }
                }));
            }
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
    }

    public virtual Future show(bool atTop = true)
    {
        if ((!Equals(_status, RefreshIndicatorStatus.refresh)) && (!Equals(_status, RefreshIndicatorStatus.snap)))
        {
            if (_status is null)
            {
                _start(atTop ? AxisDirection.down : AxisDirection.up);
            }
            _show();
        }
        return _pendingRefreshFuture;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Framework.Widgets.Widget childLocal = new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.ScrollNotification>(onNotification: _handleScrollNotification, child: new global::Doroti.Framework.Widgets.NotificationListener<global::Doroti.Framework.Widgets.OverscrollIndicatorNotification>(onNotification: _handleIndicatorNotification, child: widget.child));
        DartRuntimePrimitives.Assert(() =>
            {
                if (_status is null)
                {
                    DartRuntimePrimitives.Assert(() => _dragOffset is null);
                    DartRuntimePrimitives.Assert(() => _isIndicatorAtTop is null);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => _dragOffset is not null);
                    DartRuntimePrimitives.Assert(() => _isIndicatorAtTop is not null);
                }
                return true;
            });
        bool showIndeterminateIndicator = Equals(_status, RefreshIndicatorStatus.refresh) || Equals(_status, RefreshIndicatorStatus.done);
        return new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() =>
        {
            var __collection24667 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection24667.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(childLocal)); if (_status is not null)
            {
                __collection24667.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(top: DartRuntimePrimitives.RequireValue(_isIndicatorAtTop) ? widget.edgeOffset : null, bottom: !DartRuntimePrimitives.RequireValue(_isIndicatorAtTop) ? widget.edgeOffset : null, left: 0.0, right: 0.0, child: new global::Doroti.Framework.Widgets.SizeTransition(alignment: new global::Doroti.Framework.Painting.AlignmentDirectional(-1.0, DartRuntimePrimitives.RequireValue(_isIndicatorAtTop) ? 1.0 : -1.0), sizeFactor: _positionFactor, child: new global::Doroti.Framework.Widgets.Padding(padding: DartRuntimePrimitives.RequireValue(_isIndicatorAtTop) ? EdgeInsets.CreateOnly(top: widget.displacement) : EdgeInsets.CreateOnly(bottom: widget.displacement), child: new global::Doroti.Framework.Widgets.Align(alignment: DartRuntimePrimitives.RequireValue(_isIndicatorAtTop) ? Alignment.topCenter : Alignment.bottomCenter, child: new global::Doroti.Framework.Widgets.ScaleTransition(scale: _scaleFactor, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _positionController, builder: (context, child) =>
                {
                    global::Doroti.Framework.Widgets.Widget materialIndicator = new RefreshProgressIndicator(semanticsLabel: widget.semanticsLabel ?? MaterialLocalizations.of(context).refreshIndicatorSemanticLabel, semanticsValue: widget.semanticsValue, value: showIndeterminateIndicator ? null : _value.value, valueColor: _valueColor, backgroundColor: widget.backgroundColor, strokeWidth: widget.strokeWidth, elevation: widget.elevation);
                    global::Doroti.Framework.Widgets.Widget cupertinoIndicator = new CupertinoActivityIndicator(color: widget.color);
                    switch (widget._indicatorType)
                    {
                        case _IndicatorType__refresh_indicator.material:
                            {
                                return materialIndicator;
                            }
                        case _IndicatorType__refresh_indicator.adaptive:
                            {
                                ThemeData theme = Theme.of(context);
                                switch (theme.platform)
                                {
                                    case TargetPlatform.android:
                                    case TargetPlatform.fuchsia:
                                    case TargetPlatform.linux:
                                    case TargetPlatform.windows:
                                        {
                                            return materialIndicator;
                                        }
                                    case TargetPlatform.iOS:
                                    case TargetPlatform.macOS:
                                        {
                                            return cupertinoIndicator;
                                        }
                                    default:
                                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                                }
                            }
                        case _IndicatorType__refresh_indicator.noSpinner:
                            {
                                return new global::Doroti.Framework.Widgets.Container();
                            }
                    }
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }))))))));
            }
            return __collection24667;
        }))());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}
