// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/overscroll_indicator.dart
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

public class GlowingOverscrollIndicator : StatefulWidget
{
    public virtual bool showLeading { get; private set; } = default!;
    public virtual bool showTrailing { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual Color color { get; private set; } = default!;
    public virtual global::System.Func<ScrollNotification, bool> notificationPredicate { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public GlowingOverscrollIndicator(global::Doroti.Framework.Foundation.Key? key = null, bool showLeading = true, bool showTrailing = true, global::Doroti.Framework.Painting.AxisDirection axisDirection = default!, Color color = default!, global::System.Func<ScrollNotification, bool> notificationPredicate = default!, Widget? child = null) : base(key: key)
    {
        global::System.Func<ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        this.showLeading = showLeading;
        this.showTrailing = showTrailing;
        this.axisDirection = axisDirection;
        this.color = color;
        this.notificationPredicate = __notificationPredicate;
        this.child = child;
    }

    public virtual global::Doroti.Framework.Painting.Axis axis => global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this.axisDirection);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _GlowingOverscrollIndicatorState__overscroll_indicator());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
        string showDescription__6404 = ((this.showLeading, this.showTrailing) switch { (true, true) => "both sides", (true, false) => "leading side only", (false, true) => "trailing side only", (false, false) => "neither side (!)" });
        properties.add(new global::Doroti.Framework.Foundation.MessageProperty("show", showDescription__6404));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, showName: false));
    }

}

internal class _GlowingOverscrollIndicatorState__overscroll_indicator : State<GlowingOverscrollIndicator>, TickerProviderStateMixin<GlowingOverscrollIndicator>
{
    internal virtual _GlowController__overscroll_indicator? _leadingController { get; set; } = default;
    internal virtual _GlowController__overscroll_indicator? _trailingController { get; set; } = default;
    internal virtual global::Doroti.Framework.Foundation.Listenable? _leadingAndTrailingListener { get; set; } = default;
    internal virtual Type? _lastNotificationType { get; set; } = default;
    internal virtual DartMap<bool, bool> _accepted { get; private set; } = new DartMap<bool, bool> { [false] = true, [true] = true };
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _leadingController = new _GlowController__overscroll_indicator(vsync: this, color: ((GlowingOverscrollIndicator)this.widget).color, axis: ((GlowingOverscrollIndicator)this.widget).axis);
        _trailingController = new _GlowController__overscroll_indicator(vsync: this, color: ((GlowingOverscrollIndicator)this.widget).color, axis: ((GlowingOverscrollIndicator)this.widget).axis);
        _leadingAndTrailingListener = global::Doroti.Framework.Foundation.Listenable.CreateMerge(new List<global::Doroti.Framework.Foundation.Listenable> { this._leadingController!, this._trailingController! }.Cast<global::Doroti.Framework.Foundation.Listenable?>());
    }

    public override void didUpdateWidget(GlowingOverscrollIndicator oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (((!object.Equals(((GlowingOverscrollIndicator)oldWidget).color, ((GlowingOverscrollIndicator)this.widget).color)) || (!object.Equals(((GlowingOverscrollIndicator)oldWidget).axis, ((GlowingOverscrollIndicator)this.widget).axis))))
        {
            this._leadingController!.color = ((GlowingOverscrollIndicator)this.widget).color;
            this._leadingController!.axis = ((GlowingOverscrollIndicator)this.widget).axis;
            this._trailingController!.color = ((GlowingOverscrollIndicator)this.widget).color;
            this._trailingController!.axis = ((GlowingOverscrollIndicator)this.widget).axis;
        }
    }

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if (!this.widget.notificationPredicate(notification))
        {
            return false;
        }
        if ((!object.Equals(((ScrollNotification)notification).metrics.axis, ((GlowingOverscrollIndicator)this.widget).axis)))
        {
            return false;
        }
        this._leadingController!._paintOffsetScrollPixels = -Math.Min((((ScrollNotification)notification).metrics.pixels - ((ScrollNotification)notification).metrics.minScrollExtent), this._leadingController!._paintOffset);
        this._trailingController!._paintOffsetScrollPixels = -Math.Min((((ScrollNotification)notification).metrics.maxScrollExtent - ((ScrollNotification)notification).metrics.pixels), this._trailingController!._paintOffset);
        if ((notification is OverscrollNotification))
        {
            OverscrollNotification notification__as9386 = (OverscrollNotification)notification;
            _GlowController__overscroll_indicator? controller__9451 = default!;
            if ((((OverscrollNotification)((OverscrollNotification)notification__as9386)).overscroll < 0.0))
            {
                controller__9451 = this._leadingController;
            }
            else
            {
                if ((((OverscrollNotification)((OverscrollNotification)notification__as9386)).overscroll > 0.0))
                {
                    controller__9451 = this._trailingController;
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => false);
                }
            }
            var isLeading__9697 = (object.Equals(controller__9451, this._leadingController));
            if (!object.Equals(this._lastNotificationType, typeof(OverscrollNotification)))
            {
                var confirmationNotification__9819 = new OverscrollIndicatorNotification(leading: isLeading__9697);
                confirmationNotification__9819.dispatch(this.context);
                this._accepted[isLeading__9697] = ((OverscrollIndicatorNotification)confirmationNotification__9819).accepted;
                if (DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<bool>(this._accepted, isLeading__9697)))
                {
                    controller__9451!._paintOffset = ((OverscrollIndicatorNotification)confirmationNotification__9819).paintOffset;
                }
            }
            DartRuntimePrimitives.Assert(() => (controller__9451 is not null));
            if (DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<bool>(this._accepted, isLeading__9697)))
            {
                if ((((OverscrollNotification)((OverscrollNotification)notification__as9386)).velocity != 0.0))
                {
                    DartRuntimePrimitives.Assert(() => (((OverscrollNotification)((OverscrollNotification)notification__as9386)).dragDetails is null));
                    controller__9451!.absorbImpact(((OverscrollNotification)((OverscrollNotification)notification__as9386)).velocity.abs());
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => (((OverscrollNotification)((OverscrollNotification)notification__as9386)).overscroll != 0.0));
                    if ((((OverscrollNotification)((OverscrollNotification)notification__as9386)).dragDetails is not null))
                    {
                        var renderer__10512 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((OverscrollNotification)notification__as9386).context!.findRenderObject()!)!;
                        DartRuntimePrimitives.Assert(() => ((global::Doroti.Framework.Rendering.RenderBox)renderer__10512).hasSize);
                        global::Doroti.Ui.Size size__10640 = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.RenderBox)renderer__10512).size);
                        global::Doroti.Ui.Offset position__10687 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)renderer__10512).globalToLocal(((OverscrollNotification)((OverscrollNotification)notification__as9386)).dragDetails!.globalPosition)));
                        switch (((OverscrollNotification)notification__as9386).metrics.axis)
                        {
                            case global::Doroti.Framework.Painting.Axis.horizontal:
                                {
                                    controller__9451!.pull(((OverscrollNotification)((OverscrollNotification)notification__as9386)).overscroll.abs(), size__10640.width, Dart_uiLibrary.clampDouble(position__10687.dy, 0.0, size__10640.height), size__10640.height);
                                    break;
                                }
                            case global::Doroti.Framework.Painting.Axis.vertical:
                                {
                                    controller__9451!.pull(((OverscrollNotification)((OverscrollNotification)notification__as9386)).overscroll.abs(), size__10640.height, Dart_uiLibrary.clampDouble(position__10687.dx, 0.0, size__10640.width), size__10640.width);
                                    break;
                                }
                        }
                    }
                }
            }
        }
        else
        {
            if (((((notification is ScrollEndNotification) && (((ScrollEndNotification)((ScrollEndNotification)notification)).dragDetails is not null))) || (((notification is ScrollUpdateNotification) && (((ScrollUpdateNotification)((ScrollUpdateNotification)notification)).dragDetails is not null)))))
            {
                this._leadingController!.scrollEnd();
                this._trailingController!.scrollEnd();
            }
        }
        _lastNotificationType = DartRuntimePrimitives.RuntimeType(notification);
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._leadingController!.dispose();
        this._trailingController!.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
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

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new NotificationListener<ScrollNotification>(onNotification: (global::System.Func<ScrollNotification, bool>)this._handleScrollNotification, child: new RepaintBoundary(child: new CustomPaint(foregroundPainter: new _GlowingOverscrollIndicatorPainter__overscroll_indicator(leadingController: (((GlowingOverscrollIndicator)this.widget).showLeading ? this._leadingController : null), trailingController: (((GlowingOverscrollIndicator)this.widget).showTrailing ? this._trailingController : null), axisDirection: ((GlowingOverscrollIndicator)this.widget).axisDirection, repaint: this._leadingAndTrailingListener), child: new RepaintBoundary(child: ((GlowingOverscrollIndicator)this.widget).child)))));
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values__17506).enabled;
    __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal enum _GlowState__overscroll_indicator
{
    idle,
    absorb,
    pull,
    recede
}

public class _GlowController__overscroll_indicator : global::Doroti.Framework.Foundation.ChangeNotifier
{
    internal virtual _GlowState__overscroll_indicator _state { get; set; } = _GlowState__overscroll_indicator.idle;
    internal virtual global::Doroti.Framework.Animation.AnimationController _glowController { get; private set; } = default!;
    internal virtual Timer? _pullRecedeTimer { get; set; } = default;
    internal virtual double _paintOffset { get; set; } = 0.0;
    internal virtual double _paintOffsetScrollPixels { get; set; } = 0.0;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _decelerator { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Tween<double> _glowOpacityTween { get; private set; } = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.0);
    internal virtual global::Doroti.Framework.Animation.Animation<double> _glowOpacity { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Tween<double> _glowSizeTween { get; private set; } = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 0.0);
    internal virtual global::Doroti.Framework.Animation.Animation<double> _glowSize { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Scheduler.Ticker _displacementTicker { get; private set; } = default!;
    internal virtual Duration? _displacementTickerLastElapsed { get; set; } = default;
    internal virtual double _displacementTarget { get; set; } = 0.5;
    internal virtual double _displacement { get; set; } = 0.5;
    internal virtual double _pullDistance { get; set; } = 0.0;
    internal virtual Color _color { get; set; } = default!;
    internal virtual global::Doroti.Framework.Painting.Axis _axis { get; set; } = default!;
    internal static Duration _recedeTime = Duration.Create(milliseconds: 600L);
    internal static Duration _pullTime = Duration.Create(milliseconds: 167L);
    internal static Duration _pullHoldTime = Duration.Create(milliseconds: 167L);
    internal static Duration _pullDecayTime = Duration.Create(milliseconds: 2000L);
    internal static Duration _crossAxisHalfTime = Duration.Create(microseconds: ((Duration.microsecondsPerSecond / 60.0)).round());
    internal const double _maxOpacity = 0.5;
    internal const double _pullOpacityGlowFactor = 0.8;
    internal const double _velocityGlowFactor = 0.00006;
    internal const double _sqrt3 = 1.73205080757;
    internal static double _widthToHeightFactor = (((3.0 / 4.0)) * ((2.0 - _sqrt3)));
    internal const double _minVelocity = 100.0;
    internal const double _maxVelocity = 10000.0;

    internal _GlowController__overscroll_indicator(global::Doroti.Framework.Scheduler.TickerProvider vsync, Color color, global::Doroti.Framework.Painting.Axis axis)
    {
        this._color = color;
        this._axis = axis;
    }

    public virtual global::Doroti.Ui.Color color
    {
        get => this._color;
        set
        {
            var __value = (Color)(object)value;
            if ((object.Equals(this.color, __value)))
            {
                return;
            }
            _color = __value;
            notifyListeners();
        }
    }
    public virtual global::Doroti.Framework.Painting.Axis axis
    {
        get => this._axis;
        set
        {
            var __value = value;
            if ((object.Equals(this.axis, __value)))
            {
                return;
            }
            _axis = __value;
            notifyListeners();
        }
    }
    public virtual void dispose()
    {
        this._glowController.dispose();
        this._decelerator.dispose();
        this._displacementTicker.dispose();
        this._pullRecedeTimer?.cancel();
        base.dispose();
    }

    public virtual void absorbImpact(double velocity)
    {
        DartRuntimePrimitives.Assert(() => (velocity >= 0.0));
        this._pullRecedeTimer?.cancel();
        _pullRecedeTimer = null;
        velocity = Dart_uiLibrary.clampDouble(velocity, _minVelocity, _maxVelocity);
        this._glowOpacityTween.begin = ((object.Equals(this._state, _GlowState__overscroll_indicator.idle)) ? 0.3 : ((global::Doroti.Framework.Animation.Animation<double>)this._glowOpacity).value);
        this._glowOpacityTween.end = Dart_uiLibrary.clampDouble((velocity * _velocityGlowFactor), DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Tween<double>)this._glowOpacityTween).begin), _maxOpacity);
        this._glowSizeTween.begin = ((global::Doroti.Framework.Animation.Animation<double>)this._glowSize).value;
        this._glowSizeTween.end = Math.Min((0.025 + ((7.5e-7 * velocity) * velocity)), 1.0);
        this._glowController.duration = Duration.Create(milliseconds: ((0.15 + (velocity * 0.02))).round());
        this._glowController.forward(from: 0.0);
        _displacement = 0.5;
        _state = _GlowState__overscroll_indicator.absorb;
    }

    public virtual void pull(double overscroll, double extent, double crossAxisOffset, double crossExtent)
    {
        this._pullRecedeTimer?.cancel();
        _pullDistance += (overscroll / 200.0);
        this._glowOpacityTween.begin = ((global::Doroti.Framework.Animation.Animation<double>)this._glowOpacity).value;
        this._glowOpacityTween.end = Math.Min((((global::Doroti.Framework.Animation.Animation<double>)this._glowOpacity).value + ((overscroll / extent) * _pullOpacityGlowFactor)), _maxOpacity);
        double height__17649 = Math.Min(extent, (crossExtent * _widthToHeightFactor));
        this._glowSizeTween.begin = ((global::Doroti.Framework.Animation.Animation<double>)this._glowSize).value;
        this._glowSizeTween.end = Math.Max((1.0 - (1.0 / ((0.7 * global::Doroti.Runtime.Dart_mathLibrary.sqrt((this._pullDistance * height__17649)))))), ((global::Doroti.Framework.Animation.Animation<double>)this._glowSize).value);
        _displacementTarget = (crossAxisOffset / crossExtent);
        if ((this._displacementTarget != this._displacement))
        {
            if (!((global::Doroti.Framework.Scheduler.Ticker)this._displacementTicker).isTicking)
            {
                DartRuntimePrimitives.Assert(() => (this._displacementTickerLastElapsed is null));
                this._displacementTicker.start();
            }
        }
        else
        {
            this._displacementTicker.stop();
            _displacementTickerLastElapsed = null;
        }
        this._glowController.duration = _pullTime;
        if ((!object.Equals(this._state, _GlowState__overscroll_indicator.pull)))
        {
            this._glowController.forward(from: 0.0);
            _state = _GlowState__overscroll_indicator.pull;
        }
        else
        {
            if (!((global::Doroti.Framework.Animation.AnimationController)this._glowController).isAnimating)
            {
                DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Animation.AnimationController)this._glowController).value == 1.0));
                notifyListeners();
            }
        }
        _pullRecedeTimer = new Timer(_pullHoldTime, (() => { _recede(_pullDecayTime); }));
    }

    public virtual void scrollEnd()
    {
        if ((object.Equals(this._state, _GlowState__overscroll_indicator.pull)))
        {
            _recede(_recedeTime);
        }
    }

    internal virtual void _changePhase(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        if (!global::Doroti.Framework.Animation.AnimationStatusMembers.isCompleted(status))
        {
            return;
        }
        switch (this._state)
        {
            case _GlowState__overscroll_indicator.absorb:
                {
                    _recede(_recedeTime);
                    break;
                }
            case _GlowState__overscroll_indicator.recede:
                {
                    _state = _GlowState__overscroll_indicator.idle;
                    _pullDistance = 0.0;
                    break;
                }
            case _GlowState__overscroll_indicator.pull:
            case _GlowState__overscroll_indicator.idle:
                {
                    break;
                }
        }
    }

    internal virtual void _recede(Duration duration)
    {
        if (((object.Equals(this._state, _GlowState__overscroll_indicator.recede)) || (object.Equals(this._state, _GlowState__overscroll_indicator.idle))))
        {
            return;
        }
        this._pullRecedeTimer?.cancel();
        _pullRecedeTimer = null;
        this._glowOpacityTween.begin = ((global::Doroti.Framework.Animation.Animation<double>)this._glowOpacity).value;
        this._glowOpacityTween.end = 0.0;
        this._glowSizeTween.begin = ((global::Doroti.Framework.Animation.Animation<double>)this._glowSize).value;
        this._glowSizeTween.end = 0.0;
        this._glowController.duration = duration;
        this._glowController.forward(from: 0.0);
        _state = _GlowState__overscroll_indicator.recede;
    }

    internal virtual void _tickDisplacement(Duration elapsed)
    {
        if ((this._displacementTickerLastElapsed is not null))
        {
            double t__19631 = ((elapsed.inMicroseconds - DartRuntimePrimitives.RequireValue(this._displacementTickerLastElapsed).inMicroseconds)).toDouble();
            _displacement = (this._displacementTarget - (((this._displacementTarget - this._displacement)) * global::Doroti.Runtime.Dart_mathLibrary.pow(2.0, (-t__19631 / _crossAxisHalfTime.inMicroseconds))));
            notifyListeners();
        }
        if (global::Doroti.Framework.Physics.UtilsLibrary.nearEqual(this._displacementTarget, this._displacement, global::Doroti.Framework.Physics.Tolerance.defaultTolerance.distance))
        {
            this._displacementTicker.stop();
            _displacementTickerLastElapsed = null;
        }
        else
        {
            _displacementTickerLastElapsed = elapsed;
        }
    }

    public virtual void paint(Canvas canvas, Size size)
    {
        if ((((global::Doroti.Framework.Animation.Animation<double>)this._glowOpacity).value == 0.0))
        {
            return;
        }
        double baseGlowScale__20296 = ((size.width > size.height) ? (size.height / size.width) : 1.0);
        double radius__20388 = ((size.width * 3.0) / 2.0);
        double height__20438 = Math.Min(size.height, (size.width * _widthToHeightFactor));
        double scaleY__20522 = (((global::Doroti.Framework.Animation.Animation<double>)this._glowSize).value * baseGlowScale__20296);
        var rect__20574 = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, size.width, height__20438);
        var center__20636 = new global::Doroti.Ui.Offset((((size.width / 2.0)) * ((0.5 + this._displacement))), (height__20438 - radius__20388));
        var paint__20724 = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this.color.withOpacity(((global::Doroti.Framework.Animation.Animation<double>)this._glowOpacity).value);
    return __cascade;
}))();
        canvas.save();
        canvas.translate(0.0, (this._paintOffset + this._paintOffsetScrollPixels));
        canvas.scale(1.0, scaleY__20522);
        canvas.clipRect(rect__20574);
        canvas.drawCircle(center__20636, radius__20388, paint__20724);
        canvas.restore();
    }

    public override string ToString()
    {
        return $"_GlowController(color: {this.color}, axis: {this.axis.ToString()})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _GlowingOverscrollIndicatorPainter__overscroll_indicator : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual _GlowController__overscroll_indicator? leadingController { get; private set; }
    public virtual _GlowController__overscroll_indicator? trailingController { get; private set; }
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public static double piOver2 = (Dart_mathLibrary.pi / 2.0);

    internal _GlowingOverscrollIndicatorPainter__overscroll_indicator(_GlowController__overscroll_indicator? leadingController = null, _GlowController__overscroll_indicator? trailingController = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = default!, global::Doroti.Framework.Foundation.Listenable? repaint = null) : base(repaint: repaint)
    {
        this.leadingController = leadingController;
        this.trailingController = trailingController;
        this.axisDirection = axisDirection;
    }

    internal virtual void _paintSide(Canvas canvas, Size size, _GlowController__overscroll_indicator? controller, global::Doroti.Framework.Painting.AxisDirection axisDirection, global::Doroti.Framework.Rendering.GrowthDirection growthDirection)
    {
        if ((controller is null))
        {
            return;
        }
        switch (global::Doroti.Framework.Rendering.SliverLibrary.applyGrowthDirectionToAxisDirection(axisDirection, growthDirection))
        {
            case global::Doroti.Framework.Painting.AxisDirection.up:
                {
                    controller.paint(canvas, size);
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.down:
                {
                    canvas.save();
                    canvas.translate(0.0, size.height);
                    canvas.scale(1.0, -1.0);
                    controller.paint(canvas, size);
                    canvas.restore();
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.left:
                {
                    canvas.save();
                    canvas.rotate(piOver2);
                    canvas.scale(1.0, -1.0);
                    controller.paint(canvas, new global::Doroti.Ui.Size(size.height, size.width));
                    canvas.restore();
                    break;
                }
            case global::Doroti.Framework.Painting.AxisDirection.right:
                {
                    canvas.save();
                    canvas.translate(size.width, 0.0);
                    canvas.rotate(piOver2);
                    controller.paint(canvas, new global::Doroti.Ui.Size(size.height, size.width));
                    canvas.restore();
                    break;
                }
        }
    }

    public override void paint(Canvas canvas, Size size)
    {
        _paintSide(canvas, size, this.leadingController, this.axisDirection, global::Doroti.Framework.Rendering.GrowthDirection.reverse);
        _paintSide(canvas, size, this.trailingController, this.axisDirection, global::Doroti.Framework.Rendering.GrowthDirection.forward);
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_GlowingOverscrollIndicatorPainter__overscroll_indicator)(object)oldDelegate;
        return ((!object.Equals(((_GlowingOverscrollIndicatorPainter__overscroll_indicator)__oldDelegate).leadingController, this.leadingController)) || (!object.Equals(((_GlowingOverscrollIndicatorPainter__overscroll_indicator)__oldDelegate).trailingController, this.trailingController)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString()
    {
        return $"_GlowingOverscrollIndicatorPainter({this.leadingController}, {this.trailingController})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class StretchingOverscrollIndicator : StatefulWidget
{
    public virtual global::Doroti.Framework.Painting.AxisDirection axisDirection { get; private set; } = default!;
    public virtual global::System.Func<ScrollNotification, bool> notificationPredicate { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Widget? child { get; private set; }

    public StretchingOverscrollIndicator(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.AxisDirection axisDirection = default!, global::System.Func<ScrollNotification, bool> notificationPredicate = default!, Clip clipBehavior = Clip.hardEdge, Widget? child = null) : base(key: key)
    {
        global::System.Func<ScrollNotification, bool> __notificationPredicate = notificationPredicate ?? Scroll_notificationLibrary.defaultScrollNotificationPredicate;
        this.axisDirection = axisDirection;
        this.notificationPredicate = __notificationPredicate;
        this.clipBehavior = clipBehavior;
        this.child = child;
    }

    public virtual global::Doroti.Framework.Painting.Axis axis => global::Doroti.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(this.axisDirection);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _StretchingOverscrollIndicatorState__overscroll_indicator());
    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Framework.Painting.AxisDirection>("axisDirection", this.axisDirection));
    }

}

internal class _StretchingOverscrollIndicatorState__overscroll_indicator : State<StretchingOverscrollIndicator>, TickerProviderStateMixin<StretchingOverscrollIndicator>
{
    private bool __late__stretchController_initialized;
    private _StretchController__overscroll_indicator __late__stretchController = default!;
    internal virtual _StretchController__overscroll_indicator _stretchController
    {
        get
        {
            if (!__late__stretchController_initialized)
            {
                __late__stretchController = new _StretchController__overscroll_indicator(vsync: this);
                __late__stretchController_initialized = true;
            }
            return __late__stretchController;
        }
    }
    internal virtual ScrollNotification? _lastNotification { get; set; } = default;
    internal virtual OverscrollNotification? _lastOverscrollNotification { get; set; } = default;
    internal virtual double _totalOverscroll { get; set; } = 0.0;
    internal virtual bool _accepted { get; set; } = true;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual bool _handleScrollNotification(ScrollNotification notification)
    {
        if (!this.widget.notificationPredicate(notification))
        {
            return false;
        }
        if ((!object.Equals(((ScrollNotification)notification).metrics.axis, ((StretchingOverscrollIndicator)this.widget).axis)))
        {
            return false;
        }
        if ((notification is ScrollStartNotification))
        {
            ScrollStartNotification notification__as27070 = (ScrollStartNotification)notification;
            _accepted = true;
            _totalOverscroll = 0.0;
        }
        else
        {
            if ((notification is OverscrollNotification))
            {
                OverscrollNotification notification__as27182 = (OverscrollNotification)notification;
                _lastOverscrollNotification = ((OverscrollNotification)notification__as27182);
                if (!object.Equals(DartRuntimePrimitives.RuntimeType(this._lastNotification), typeof(OverscrollNotification)))
                {
                    var confirmationNotification__27358 = new OverscrollIndicatorNotification(leading: (((OverscrollNotification)((OverscrollNotification)notification__as27182)).overscroll < 0.0));
                    confirmationNotification__27358.dispatch(this.context);
                    _accepted = ((OverscrollIndicatorNotification)confirmationNotification__27358).accepted;
                }
                if (this._accepted)
                {
                    _totalOverscroll += ((OverscrollNotification)((OverscrollNotification)notification__as27182)).overscroll;
                    if ((((OverscrollNotification)((OverscrollNotification)notification__as27182)).velocity != 0.0))
                    {
                        DartRuntimePrimitives.Assert(() => (((OverscrollNotification)((OverscrollNotification)notification__as27182)).dragDetails is null));
                        this._stretchController.absorbImpact(((OverscrollNotification)((OverscrollNotification)notification__as27182)).velocity);
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => (((OverscrollNotification)((OverscrollNotification)notification__as27182)).overscroll != 0.0));
                        if ((((OverscrollNotification)((OverscrollNotification)notification__as27182)).dragDetails is not null))
                        {
                            double viewportDimension__28309 = ((OverscrollNotification)notification__as27182).metrics.viewportDimension;
                            double distanceForPull__28394 = (this._totalOverscroll / viewportDimension__28309);
                            double clampedOverscroll__28475 = Dart_uiLibrary.clampDouble(distanceForPull__28394, -1.0, 1.0);
                            this._stretchController.pull(clampedOverscroll__28475);
                        }
                    }
                }
            }
            else
            {
                if ((notification is ScrollEndNotification))
                {
                    ScrollEndNotification notification__as28637 = (ScrollEndNotification)notification;
                    double velocity__28691 = (((StretchingOverscrollIndicator)this.widget).axis switch { global::Doroti.Framework.Painting.Axis.vertical => (((ScrollEndNotification)((ScrollEndNotification)notification__as28637)).dragDetails?.velocity.pixelsPerSecond.dy ?? 0.0), global::Doroti.Framework.Painting.Axis.horizontal => (((ScrollEndNotification)((ScrollEndNotification)notification__as28637)).dragDetails?.velocity.pixelsPerSecond.dx ?? 0.0), _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
                    if (((object.Equals(((ScrollEndNotification)notification__as28637).metrics.axisDirection, global::Doroti.Framework.Painting.AxisDirection.left)) || (object.Equals(((ScrollEndNotification)notification__as28637).metrics.axisDirection, global::Doroti.Framework.Painting.AxisDirection.up))))
                    {
                        velocity__28691 = -velocity__28691;
                    }
                    _totalOverscroll = 0.0;
                    if (this._accepted)
                    {
                        this._stretchController.scrollEnd(velocity__28691);
                    }
                }
                else
                {
                    if ((notification is ScrollUpdateNotification))
                    {
                        ScrollUpdateNotification notification__as29426 = (ScrollUpdateNotification)notification;
                        _totalOverscroll = 0.0;
                        this._stretchController.scrollEnd(0.0);
                    }
                }
            }
        }
        _lastNotification = notification;
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        this._stretchController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
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

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new NotificationListener<ScrollNotification>(onNotification: (global::System.Func<ScrollNotification, bool>)this._handleScrollNotification, child: new AnimatedBuilder(animation: this._stretchController, builder: ((global::System.Func<BuildContext, Widget?, Widget>)((context, child) =>
        {
            double stretch__30001 = ((_StretchController__overscroll_indicator)this._stretchController).overscroll;
            double mainAxisSize__30065 = default!;
            switch (((StretchingOverscrollIndicator)this.widget).axis)
            {
                case global::Doroti.Framework.Painting.Axis.horizontal:
                    {
                        mainAxisSize__30065 = MediaQuery.widthOf(context);
                        break;
                    }
                case global::Doroti.Framework.Painting.Axis.vertical:
                    {
                        mainAxisSize__30065 = MediaQuery.heightOf(context);
                        break;
                    }
            }
            double viewportDimension__30332 = (this._lastOverscrollNotification?.metrics.viewportDimension ?? mainAxisSize__30065);
            double overscroll__30456 = -stretch__30001;
            if (((object.Equals(((StretchingOverscrollIndicator)this.widget).axisDirection, global::Doroti.Framework.Painting.AxisDirection.up)) || (object.Equals(((StretchingOverscrollIndicator)this.widget).axisDirection, global::Doroti.Framework.Painting.AxisDirection.left))))
            {
                overscroll__30456 = -overscroll__30456;
            }
            Widget transform__30734 = ((Widget)(object?)new StretchEffect(stretchStrength: overscroll__30456, axis: ((StretchingOverscrollIndicator)this.widget).axis, child: (((StretchingOverscrollIndicator)this.widget).child ?? SizedBox.CreateShrink())));
            return ((Widget)(object?)new ClipRect(clipBehavior: (((stretch__30001 != 0.0) && (viewportDimension__30332 != mainAxisSize__30065)) ? ((StretchingOverscrollIndicator)this.widget).clipBehavior : Clip.none), child: transform__30734));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
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
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values__17506).enabled;
    __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result__17553);
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _StretchController__overscroll_indicator : global::Doroti.Framework.Foundation.Listenable
{
    public virtual global::Doroti.Framework.Scheduler.TickerProvider vsync { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Animation.AnimationController? _controller { get; set; } = default;
    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<double> _overscrollNotifier { get; private set; } = new global::Doroti.Framework.Foundation.ValueNotifier<double>(0.0);
    internal virtual double _interruptedOverscroll { get; set; } = 0.0;
    internal static double _exponentialScalar = (global::Doroti.Runtime.Dart_mathLibrary.e / 0.33);
    internal const double _stretchIntensity = 0.016;
    public static double minOverscroll = -1.0;
    public const double maxOverscroll = 1.0;
    internal static double _flingVelocityFriction = (1L / 6000L);
    internal static double _absorbImpactVelocityFriction = (1L / 3000L);
    internal const double _maxFlingVelocity = 0.5;
    internal const double _maxAbsorbImpactVelocity = 1.25;
    public const double kNaturalFrequency = 24.657;
    public const double kDampingRatio = 0.98;
    public const double kTimeCorrectionFactor = 0.8;
    public static double kStiffness = (kNaturalFrequency * kNaturalFrequency);
    internal static global::Doroti.Framework.Physics.SpringDescription _kStretchSpringDescription = global::Doroti.Framework.Physics.SpringDescription.CreateWithDampingRatio(mass: 1, stiffness: ((kStiffness * kTimeCorrectionFactor) * kTimeCorrectionFactor), ratio: kDampingRatio);

    internal _StretchController__overscroll_indicator(global::Doroti.Framework.Scheduler.TickerProvider vsync)
    {
        this.vsync = vsync;
    }

    public virtual double overscroll
    {
        get => ((global::Doroti.Framework.Foundation.ValueNotifier<double>)this._overscrollNotifier).value;
        set
        {
            var newValue = value;
            this._overscrollNotifier.value = Dart_uiLibrary.clampDouble(newValue, minOverscroll, maxOverscroll);
        }
    }
    public virtual void addListener(global::System.Action listener)
    {
        this._overscrollNotifier.addListener(() => listener());
    }

    public virtual void removeListener(global::System.Action listener)
    {
        this._overscrollNotifier.removeListener(() => listener());
    }

    internal virtual global::Doroti.Framework.Physics.SpringSimulation _createStretchSimulation(double velocity)
    {
        return new global::Doroti.Framework.Physics.SpringSimulation(_kStretchSpringDescription, this.overscroll, 0.0, (velocity * kTimeCorrectionFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void absorbImpact(double velocity)
    {
        if ((velocity == 0.0))
        {
            return;
        }
        double scaledVelocity__36614 = Dart_uiLibrary.clampDouble((velocity * _absorbImpactVelocityFriction), -_maxAbsorbImpactVelocity, _maxAbsorbImpactVelocity);
        animate(_createStretchSimulation(scaledVelocity__36614));
    }

    public virtual void scrollEnd(double velocity)
    {
        if (((velocity == 0.0) && (this.overscroll == 0.0)))
        {
            return;
        }
        double scaledVelocity__37024 = Dart_uiLibrary.clampDouble(-((velocity * _flingVelocityFriction)), -_maxFlingVelocity, _maxFlingVelocity);
        if ((this._controller is null))
        {
            animate(_createStretchSimulation(scaledVelocity__37024));
        }
    }

    public virtual void animate(global::Doroti.Framework.Physics.Simulation simulation)
    {
        var controller__37583 = ((Func<global::Doroti.Framework.Animation.AnimationController>)(() =>
{
    var __cascade = global::Doroti.Framework.Animation.AnimationController.CreateUnbounded(vsync: this.vsync);
    __cascade.addListener(((global::System.Action)(() =>
    {
        double newOverscroll__37686 = (this._controller?.value ?? 0.0);
        overscroll = newOverscroll__37686;
    })));
    return __cascade;
}))();
        DartRuntimePrimitives.Ignore(controller__37583.animateWith(simulation).whenComplete((() =>
        {
            if ((object.Equals(this._controller, controller__37583)))
            {
                overscroll = 0.0;
                _interruptedOverscroll = 0.0;
                controller__37583.dispose();
                _controller = null;
            }
            return default!;
        })));
        this._controller?.dispose();
        _controller = controller__37583;
    }

    public virtual void pull(double normalizedOverscroll)
    {
        if ((this._controller is not null))
        {
            _interruptedOverscroll = this._controller!.value;
            this._controller!.dispose();
            _controller = null;
        }
        var pullDistance__38799 = normalizedOverscroll;
        double absDistance__38853 = pullDistance__38799.abs();
        double linearIntensity__38904 = (_stretchIntensity * absDistance__38853);
        double exponentialIntensity__38972 = (_stretchIntensity * ((1L - global::Doroti.Runtime.Dart_mathLibrary.exp((-absDistance__38853 * _exponentialScalar)))));
        double directionSign__39142 = Math.Sign(pullDistance__38799);
        double newOverscroll__39194 = (directionSign__39142 * ((linearIntensity__38904 + exponentialIntensity__38972)));
        overscroll = (newOverscroll__39194 + this._interruptedOverscroll);
    }

    public virtual void dispose()
    {
        this._controller?.dispose();
        _controller = null;
        this._overscrollNotifier.dispose();
    }

    public override string ToString() => "_StretchController()";
}

public class OverscrollIndicatorNotification : Notification, ViewportNotificationMixin
{
    public virtual bool leading { get; private set; } = default!;
    public virtual double paintOffset { get; set; } = 0.0;
    public virtual bool accepted { get; set; } = true;
    public virtual long _depth { get; set; } = 0L;

    public OverscrollIndicatorNotification(bool leading)
    {
        this.leading = leading;
    }

    public virtual void disallowIndicator()
    {
        accepted = false;
    }

    public override void debugFillDescription(List<string> description)
    {
        base.debugFillDescription(description);
        description.Add($"depth: {this.depth} ({((this.depth == 0L) ? "local" : "remote")})");
        description.Add($"side: {(this.leading ? "leading edge" : "trailing edge")}");
    }

    public virtual long depth => this._depth;
}

