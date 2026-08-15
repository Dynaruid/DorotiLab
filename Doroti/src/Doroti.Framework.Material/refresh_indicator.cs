// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/refresh_indicator.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

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

public class RefreshIndicator : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;
    public virtual double displacement { get; private set; } = default!;
    public virtual double edgeOffset { get; private set; } = default!;
    public virtual global::System.Func<Future> onRefresh { get; private set; } = default!;
    public virtual global::System.Action<RefreshIndicatorStatus?>? onStatusChange { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool> notificationPredicate { get; private set; } = default!;
    public virtual string? semanticsLabel { get; private set; }
    public virtual string? semanticsValue { get; private set; }
    public virtual double strokeWidth { get; private set; } = default!;
    internal virtual _IndicatorType__refresh_indicator _indicatorType { get; private set; } = default!;
    public virtual RefreshIndicatorTriggerMode triggerMode { get; private set; } = default!;
    public virtual double elevation { get; private set; } = default!;

    public RefreshIndicator(global::Doroti.Generated.Framework.Foundation.Key? key = null, double displacement = 40.0, double edgeOffset = 0.0, global::System.Func<Future> onRefresh = default!, Color? color = null, Color? backgroundColor = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool> notificationPredicate = default!, string? semanticsLabel = null, string? semanticsValue = null, double? strokeWidth = null, RefreshIndicatorTriggerMode triggerMode = RefreshIndicatorTriggerMode.onEdge, double elevation = 2.0, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key)
    {
        global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
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
        this._indicatorType = _IndicatorType__refresh_indicator.material;
        this.onStatusChange = null;
        System.Diagnostics.Debug.Assert((elevation >= 0.0));
    }

    public static RefreshIndicator CreateAdaptive(global::Doroti.Generated.Framework.Foundation.Key? key = null, double displacement = 40.0, double edgeOffset = 0.0, global::System.Func<Future> onRefresh = default!, Color? color = null, Color? backgroundColor = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool> notificationPredicate = default!, string? semanticsLabel = null, string? semanticsValue = null, double? strokeWidth = null, RefreshIndicatorTriggerMode triggerMode = RefreshIndicatorTriggerMode.onEdge, double elevation = 2.0, global::Doroti.Generated.Framework.Widgets.Widget child = default!)
    {
        var __instance = new RefreshIndicator(key: key, displacement: displacement, edgeOffset: edgeOffset, onRefresh: onRefresh, color: color, backgroundColor: backgroundColor, notificationPredicate: notificationPredicate, semanticsLabel: semanticsLabel, semanticsValue: semanticsValue, strokeWidth: strokeWidth, triggerMode: triggerMode, elevation: elevation, child: child);
        global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
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

    public static RefreshIndicator CreateNoSpinner(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Func<Future> onRefresh = default!, global::System.Action<RefreshIndicatorStatus?>? onStatusChange = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool> notificationPredicate = default!, string? semanticsLabel = null, string? semanticsValue = null, RefreshIndicatorTriggerMode triggerMode = RefreshIndicatorTriggerMode.onEdge, double elevation = 2.0, global::Doroti.Generated.Framework.Widgets.Widget child = default!)
    {
        var __instance = new RefreshIndicator(key: key, onRefresh: onRefresh, notificationPredicate: notificationPredicate, semanticsLabel: semanticsLabel, semanticsValue: semanticsValue, strokeWidth: RefreshProgressIndicator.defaultStrokeWidth, triggerMode: triggerMode, elevation: elevation, child: child);
        global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
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

public class RefreshIndicatorState : global::Doroti.Generated.Framework.Widgets.State<RefreshIndicator>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<RefreshIndicator>
{
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _positionController { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _scaleController { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _positionFactor { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _scaleFactor { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<double> _value { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.Animation<Color?> _valueColor { get; set; } = default!;
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
                __late__effectiveValueColor = (((RefreshIndicator)this.widget).color ?? Theme.of(this.context).colorScheme.primary);
                __late__effectiveValueColor_initialized = true;
            }
            return __late__effectiveValueColor;
        }
        set { __late__effectiveValueColor = value; __late__effectiveValueColor_initialized = true; }
    }
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _threeQuarterTween = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 0.75));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _kDragSizeFactorLimitTween = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: Refresh_indicatorLibrary._kDragSizeFactorLimit));
    internal static global::Doroti.Generated.Framework.Animation.Animatable<double> _oneToZeroTween = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0));
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _positionController = new global::Doroti.Generated.Framework.Animation.AnimationController(vsync: this);
        _positionFactor = this._positionController.drive(_kDragSizeFactorLimitTween);
        _value = this._positionController.drive(_threeQuarterTween);
        _scaleController = new global::Doroti.Generated.Framework.Animation.AnimationController(vsync: this);
        _scaleFactor = this._scaleController.drive(_oneToZeroTween);
    }

    public override void didChangeDependencies()
    {
        _setupColorTween();
        base.didChangeDependencies();
    }

    public override void didUpdateWidget(RefreshIndicator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((RefreshIndicator)oldWidget).color, ((RefreshIndicator)this.widget).color)))
        {
            _setupColorTween();
        }
    }

    public override void dispose()
    {
        this._positionController.dispose();
        this._scaleController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _setupColorTween()
    {
        _effectiveValueColor = (((RefreshIndicator)this.widget).color ?? Theme.of(this.context).colorScheme.primary);
        global::Doroti.Ui.Color color__14807 = ((global::Doroti.Ui.Color)(object?)this._effectiveValueColor);
        if ((color__14807.alpha == 0L))
        {
            _valueColor = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Animation.Animation<Color?>>(new global::Doroti.Generated.Framework.Animation.AlwaysStoppedAnimation<global::Doroti.Ui.Color>(color__14807));
        }
        else
        {
            _valueColor = this._positionController.drive(new global::Doroti.Generated.Framework.Animation.ColorTween(begin: color__14807.withAlpha(0L), end: color__14807.withAlpha(color__14807.alpha)).chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, (1.0 / Refresh_indicatorLibrary._kDragSizeFactorLimit)))));
        }
    }

    internal virtual bool _shouldStart(global::Doroti.Generated.Framework.Widgets.ScrollNotification notification)
    {
        return ((((((((notification is global::Doroti.Generated.Framework.Widgets.ScrollStartNotification) && (((global::Doroti.Generated.Framework.Widgets.ScrollStartNotification)((global::Doroti.Generated.Framework.Widgets.ScrollStartNotification)notification)).dragDetails is not null))) || ((((notification is global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification) && (((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification)).dragDetails is not null)) && (object.Equals(((RefreshIndicator)this.widget).triggerMode, RefreshIndicatorTriggerMode.anywhere)))))) && (((((object.Equals(((global::Doroti.Generated.Framework.Widgets.ScrollNotification)notification).metrics.axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.up)) && (((global::Doroti.Generated.Framework.Widgets.ScrollNotification)notification).metrics.extentAfter == 0.0))) || (((object.Equals(((global::Doroti.Generated.Framework.Widgets.ScrollNotification)notification).metrics.axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.down)) && (((global::Doroti.Generated.Framework.Widgets.ScrollNotification)notification).metrics.extentBefore == 0.0)))))) && (this._status is null)) && _start(((global::Doroti.Generated.Framework.Widgets.ScrollNotification)notification).metrics.axisDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _handleScrollNotification(global::Doroti.Generated.Framework.Widgets.ScrollNotification notification)
    {
        if (!this.widget.notificationPredicate(notification))
        {
            return false;
        }
        if (_shouldStart(notification))
        {
            setState(((global::System.Action)(() => {
_status = RefreshIndicatorStatus.drag;
((RefreshIndicator)this.widget).onStatusChange?.Invoke(this._status);
})));
            return false;
        }
        bool? indicatorAtTopNow__16583 = (((global::Doroti.Generated.Framework.Widgets.ScrollNotification)notification).metrics.axisDirection switch { global::Doroti.Generated.Framework.Painting.AxisDirection.down => true, global::Doroti.Generated.Framework.Painting.AxisDirection.up => true, global::Doroti.Generated.Framework.Painting.AxisDirection.left => DartRuntimePrimitives.ConvertValue<bool>(null), global::Doroti.Generated.Framework.Painting.AxisDirection.right => DartRuntimePrimitives.ConvertValue<bool>(null), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        if ((indicatorAtTopNow__16583 != this._isIndicatorAtTop))
        {
            if (((object.Equals(this._status, RefreshIndicatorStatus.drag)) || (object.Equals(this._status, RefreshIndicatorStatus.armed))))
            {
                DartRuntimePrimitives.Ignore(_dismiss(RefreshIndicatorStatus.canceled));
            }
        }
        else
        {
            if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification))
            {
                global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification notification__as16986 = (global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification;
                if (((object.Equals(this._status, RefreshIndicatorStatus.drag)) || (object.Equals(this._status, RefreshIndicatorStatus.armed))))
                {
                    if ((object.Equals(((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification__as16986).metrics.axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.down)))
                    {
                        _dragOffset = (DartRuntimePrimitives.RequireValue(this._dragOffset) - DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification__as16986)).scrollDelta));
                    }
                    else
                    {
                        if ((object.Equals(((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification__as16986).metrics.axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.up)))
                        {
                            _dragOffset = (DartRuntimePrimitives.RequireValue(this._dragOffset) + DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification__as16986)).scrollDelta));
                        }
                    }
                    _checkDragOffset(((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification__as16986).metrics.viewportDimension);
                }
                if (((object.Equals(this._status, RefreshIndicatorStatus.armed)) && (((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification__as16986)).dragDetails is null)))
                {
                    _show();
                }
            }
            else
            {
                if ((notification is global::Doroti.Generated.Framework.Widgets.OverscrollNotification))
                {
                    global::Doroti.Generated.Framework.Widgets.OverscrollNotification notification__as17855 = (global::Doroti.Generated.Framework.Widgets.OverscrollNotification)notification;
                    if (((object.Equals(this._status, RefreshIndicatorStatus.drag)) || (object.Equals(this._status, RefreshIndicatorStatus.armed))))
                    {
                        if ((object.Equals(((global::Doroti.Generated.Framework.Widgets.OverscrollNotification)notification__as17855).metrics.axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.down)))
                        {
                            _dragOffset = (DartRuntimePrimitives.RequireValue(this._dragOffset) - ((global::Doroti.Generated.Framework.Widgets.OverscrollNotification)((global::Doroti.Generated.Framework.Widgets.OverscrollNotification)notification__as17855)).overscroll);
                        }
                        else
                        {
                            if ((object.Equals(((global::Doroti.Generated.Framework.Widgets.OverscrollNotification)notification__as17855).metrics.axisDirection, global::Doroti.Generated.Framework.Painting.AxisDirection.up)))
                            {
                                _dragOffset = (DartRuntimePrimitives.RequireValue(this._dragOffset) + ((global::Doroti.Generated.Framework.Widgets.OverscrollNotification)((global::Doroti.Generated.Framework.Widgets.OverscrollNotification)notification__as17855)).overscroll);
                            }
                        }
                        _checkDragOffset(((global::Doroti.Generated.Framework.Widgets.OverscrollNotification)notification__as17855).metrics.viewportDimension);
                    }
                }
                else
                {
                    if ((notification is global::Doroti.Generated.Framework.Widgets.ScrollEndNotification))
                    {
                        global::Doroti.Generated.Framework.Widgets.ScrollEndNotification notification__as18368 = (global::Doroti.Generated.Framework.Widgets.ScrollEndNotification)notification;
                        switch (this._status)
                        {
                            case RefreshIndicatorStatus.armed:
                                {
                                    if ((((global::Doroti.Generated.Framework.Animation.AnimationController)this._positionController).value < 1.0))
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

    internal virtual bool _handleIndicatorNotification(global::Doroti.Generated.Framework.Widgets.OverscrollIndicatorNotification notification)
    {
        if (((notification.depth != 0L) || !((global::Doroti.Generated.Framework.Widgets.OverscrollIndicatorNotification)notification).leading))
        {
            return false;
        }
        if ((object.Equals(this._status, RefreshIndicatorStatus.drag)))
        {
            notification.disallowIndicator();
            return true;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _start(global::Doroti.Generated.Framework.Painting.AxisDirection direction)
    {
        DartRuntimePrimitives.Assert(() => (this._status is null));
        DartRuntimePrimitives.Assert(() => (this._isIndicatorAtTop is null));
        DartRuntimePrimitives.Assert(() => (this._dragOffset is null));
        switch (direction)
        {
            case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
                {
                    _isIndicatorAtTop = true;
                    break;
                }
            case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
            case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
                {
                    _isIndicatorAtTop = null;
                    return false;
                }
        }
        _dragOffset = 0.0;
        this._scaleController.value = 0.0;
        this._positionController.value = 0.0;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _checkDragOffset(double containerExtent)
    {
        DartRuntimePrimitives.Assert(() => ((object.Equals(this._status, RefreshIndicatorStatus.drag)) || (object.Equals(this._status, RefreshIndicatorStatus.armed))));
        double newValue__20020 = (DartRuntimePrimitives.RequireValue(this._dragOffset) / ((containerExtent * Refresh_indicatorLibrary._kDragContainerExtentPercentage)));
        if ((object.Equals(this._status, RefreshIndicatorStatus.armed)))
        {
            newValue__20020 = Math.Max(newValue__20020, (1.0 / Refresh_indicatorLibrary._kDragSizeFactorLimit));
        }
        this._positionController.value = Dart_uiLibrary.clampDouble(newValue__20020, 0.0, 1.0);
        if (((object.Equals(this._status, RefreshIndicatorStatus.drag)) && (((global::Doroti.Generated.Framework.Animation.Animation<Color?>)this._valueColor).value!.alpha == this._effectiveValueColor.alpha)))
        {
            _status = RefreshIndicatorStatus.armed;
            ((RefreshIndicator)this.widget).onStatusChange?.Invoke(this._status);
        }
    }

    internal async virtual Future _dismiss(RefreshIndicatorStatus newMode)
    {
        await Future.value();
        DartRuntimePrimitives.Assert(() => ((object.Equals(newMode, RefreshIndicatorStatus.canceled)) || (object.Equals(newMode, RefreshIndicatorStatus.done))));
        setState(((global::System.Action)(() => {
_status = newMode;
((RefreshIndicator)this.widget).onStatusChange?.Invoke(this._status);
})));
        switch (DartRuntimePrimitives.RequireValue(this._status))
        {
            case RefreshIndicatorStatus.done:
                {
                    this._scaleController.animateTo(1.0, duration: Refresh_indicatorLibrary._kIndicatorScaleDuration);
                    break;
                }
            case RefreshIndicatorStatus.canceled:
                {
                    this._positionController.animateTo(0.0, duration: Refresh_indicatorLibrary._kIndicatorScaleDuration);
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
        if ((this.mounted && (object.Equals(this._status, newMode))))
        {
            _dragOffset = null;
            _isIndicatorAtTop = null;
            setState(((global::System.Action)(() => {
_status = null;
})));
        }
    }

    internal virtual void _show()
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._status, RefreshIndicatorStatus.refresh)));
        DartRuntimePrimitives.Assert(() => (!object.Equals(this._status, RefreshIndicatorStatus.snap)));
        var completer__21801 = new Completer<object?>();
        DartRuntimePrimitives.Ignore(_pendingRefreshFuture = completer__21801.future);
        _status = RefreshIndicatorStatus.snap;
        ((RefreshIndicator)this.widget).onStatusChange?.Invoke(this._status);
        DartRuntimePrimitives.Ignore(this._positionController.animateTo((1.0 / Refresh_indicatorLibrary._kDragSizeFactorLimit), duration: Refresh_indicatorLibrary._kIndicatorSnapDuration).then(((global::System.Func<object?, object>)((value) => {
if ((this.mounted && (object.Equals(this._status, RefreshIndicatorStatus.snap))))
{
    setState(((global::System.Action)(() => {
_status = RefreshIndicatorStatus.refresh;
})));
    Future refreshResult__22362 = this.widget.onRefresh();
    DartRuntimePrimitives.Ignore(refreshResult__22362.whenComplete((() => {
if ((this.mounted && (object.Equals(this._status, RefreshIndicatorStatus.refresh))))
{
    completer__21801.complete();
    DartRuntimePrimitives.Ignore(_dismiss(RefreshIndicatorStatus.done));
}
})));
}
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
    }

    public virtual Future show(bool atTop = true)
    {
        if (((!object.Equals(this._status, RefreshIndicatorStatus.refresh)) && (!object.Equals(this._status, RefreshIndicatorStatus.snap))))
        {
            if ((this._status is null))
            {
                _start((atTop ? global::Doroti.Generated.Framework.Painting.AxisDirection.down : global::Doroti.Generated.Framework.Painting.AxisDirection.up));
            }
            _show();
        }
        return this._pendingRefreshFuture;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        global::Doroti.Generated.Framework.Widgets.Widget child__23971 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.ScrollNotification>(onNotification: (global::System.Func<global::Doroti.Generated.Framework.Widgets.ScrollNotification, bool>)this._handleScrollNotification, child: new global::Doroti.Generated.Framework.Widgets.NotificationListener<global::Doroti.Generated.Framework.Widgets.OverscrollIndicatorNotification>(onNotification: (global::System.Func<global::Doroti.Generated.Framework.Widgets.OverscrollIndicatorNotification, bool>)this._handleIndicatorNotification, child: ((RefreshIndicator)this.widget).child)));
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._status is null))
                {
                    DartRuntimePrimitives.Assert(() => (this._dragOffset is null));
                    DartRuntimePrimitives.Assert(() => (this._isIndicatorAtTop is null));
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => (this._dragOffset is not null));
                    DartRuntimePrimitives.Assert(() => (this._isIndicatorAtTop is not null));
                }
                return true;
            });
        bool showIndeterminateIndicator__24510 = ((object.Equals(this._status, RefreshIndicatorStatus.refresh)) || (object.Equals(this._status, RefreshIndicatorStatus.done)));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection24667 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection24667.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(child__23971)); if ((this._status is not null)) { __collection24667.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(top: (DartRuntimePrimitives.RequireValue(this._isIndicatorAtTop) ? ((RefreshIndicator)this.widget).edgeOffset : null), bottom: (!DartRuntimePrimitives.RequireValue(this._isIndicatorAtTop) ? ((RefreshIndicator)this.widget).edgeOffset : null), left: 0.0, right: 0.0, child: new global::Doroti.Generated.Framework.Widgets.SizeTransition(alignment: new global::Doroti.Generated.Framework.Painting.AlignmentDirectional(-1.0, (DartRuntimePrimitives.RequireValue(this._isIndicatorAtTop) ? 1.0 : -1.0)), sizeFactor: this._positionFactor, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: (DartRuntimePrimitives.RequireValue(this._isIndicatorAtTop) ? global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(top: ((RefreshIndicator)this.widget).displacement) : global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: ((RefreshIndicator)this.widget).displacement)), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: (DartRuntimePrimitives.RequireValue(this._isIndicatorAtTop) ? global::Doroti.Generated.Framework.Painting.Alignment.topCenter : global::Doroti.Generated.Framework.Painting.Alignment.bottomCenter), child: new global::Doroti.Generated.Framework.Widgets.ScaleTransition(scale: this._scaleFactor, child: new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this._positionController, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
global::Doroti.Generated.Framework.Widgets.Widget materialIndicator__25739 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new RefreshProgressIndicator(semanticsLabel: (((RefreshIndicator)this.widget).semanticsLabel ?? MaterialLocalizations.of(context).refreshIndicatorSemanticLabel), semanticsValue: ((RefreshIndicator)this.widget).semanticsValue, value: (showIndeterminateIndicator__24510 ? null : ((global::Doroti.Generated.Framework.Animation.Animation<double>)this._value).value), valueColor: this._valueColor, backgroundColor: ((RefreshIndicator)this.widget).backgroundColor, strokeWidth: ((RefreshIndicator)this.widget).strokeWidth, elevation: ((RefreshIndicator)this.widget).elevation));
global::Doroti.Generated.Framework.Widgets.Widget cupertinoIndicator__26422 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoActivityIndicator(color: ((RefreshIndicator)this.widget).color));
switch (((RefreshIndicator)this.widget)._indicatorType)
{
    case _IndicatorType__refresh_indicator.material:
        {
            return materialIndicator__25739;
        }
    case _IndicatorType__refresh_indicator.adaptive:
        {
            ThemeData theme__26814 = Theme.of(context);
            switch (theme__26814.platform)
            {
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                    {
                        return materialIndicator__25739;
                    }
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                    {
                        return cupertinoIndicator__26422;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            break;
        }
    case _IndicatorType__refresh_indicator.noSpinner:
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Container());
        }
}
throw new InvalidOperationException("Dart closure completed without a value.");
})))))))))); } return __collection24667; }))()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}
