// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/predictive_back_page_transitions_builder.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class PredictiveBackPageTransitionsBuilder : global::Doroti.Framework.Widgets.PageTransitionsBuilder
{
    public virtual Color? fallbackColor { get; private set; }

    public PredictiveBackPageTransitionsBuilder(Color? fallbackColor = null)
    {
        this.fallbackColor = fallbackColor;
    }

    public override Duration transitionDuration => Duration.Create(milliseconds: FadeForwardsPageTransitionsBuilder.kTransitionMilliseconds);
    public override global::Doroti.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Framework.Widgets.PageRoute<T> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return new _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(route: route, builder: (context, phase, startBackEvent, currentBackEvent) =>
        {
            if (route.popGestureInProgress)
            {
                return new _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder(isDelegatedTransition: true, animation: animation, phase: phase, secondaryAnimation: secondaryAnimation, startBackEvent: startBackEvent, currentBackEvent: currentBackEvent, child: child);
            }
            return new FadeForwardsPageTransitionsBuilder(backgroundColor: fallbackColor).buildTransitions(route, context, animation, secondaryAnimation, child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PredictiveBackFullscreenPageTransitionsBuilder : global::Doroti.Framework.Widgets.PageTransitionsBuilder
{
    public virtual Color? fallbackColor { get; private set; }

    public PredictiveBackFullscreenPageTransitionsBuilder(Color? fallbackColor = null)
    {
        this.fallbackColor = fallbackColor;
    }

    public override global::Doroti.Framework.Widgets.Widget buildTransitions<T>(global::Doroti.Framework.Widgets.PageRoute<T> route, global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::Doroti.Framework.Widgets.Widget child)
    {
        return new _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(route: route, builder: (context, phase, startBackEvent, currentBackEvent) =>
        {
            if (route.popGestureInProgress)
            {
                return new _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder(animation: animation, secondaryAnimation: secondaryAnimation, getIsCurrent: () => route.isCurrent, phase: phase, child: child);
            }
            return new ZoomPageTransitionsBuilder(backgroundColor: fallbackColor).buildTransitions(route, context, animation, secondaryAnimation, child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal delegate global::Doroti.Framework.Widgets.Widget _PredictiveBackGestureDetectorWidgetBuilder__predictive_back_page_transitions_builder(global::Doroti.Framework.Widgets.BuildContext context, _PredictiveBackPhase__predictive_back_page_transitions_builder phase, global::Doroti.Framework.Services.PredictiveBackEvent? startBackEvent, global::Doroti.Framework.Services.PredictiveBackEvent? currentBackEvent);

public enum _PredictiveBackPhase__predictive_back_page_transitions_builder
{
    idle,
    start,
    update,
    commit,
    cancel
}

internal class _PredictiveBackGestureDetector__predictive_back_page_transitions_builder : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, _PredictiveBackPhase__predictive_back_page_transitions_builder, global::Doroti.Framework.Services.PredictiveBackEvent?, global::Doroti.Framework.Services.PredictiveBackEvent?, global::Doroti.Framework.Widgets.Widget> builder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.IModalRoute route { get; private set; } = default!;

    internal _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(global::Doroti.Framework.Widgets.IModalRoute route, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, _PredictiveBackPhase__predictive_back_page_transitions_builder, global::Doroti.Framework.Services.PredictiveBackEvent?, global::Doroti.Framework.Services.PredictiveBackEvent?, global::Doroti.Framework.Widgets.Widget> builder)
    {
        this.route = route;
        this.builder = builder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PredictiveBackGestureDetectorState__predictive_back_page_transitions_builder());
}

internal class _PredictiveBackGestureDetectorState__predictive_back_page_transitions_builder : global::Doroti.Framework.Widgets.State<_PredictiveBackGestureDetector__predictive_back_page_transitions_builder>, global::Doroti.Framework.Widgets.WidgetsBindingObserver
{
    internal virtual _PredictiveBackPhase__predictive_back_page_transitions_builder _phase { get; set; } = _PredictiveBackPhase__predictive_back_page_transitions_builder.idle;
    internal virtual global::Doroti.Framework.Services.PredictiveBackEvent? _startBackEvent { get; set; } = default;
    internal virtual global::Doroti.Framework.Services.PredictiveBackEvent? _currentBackEvent { get; set; } = default;

    internal virtual bool _isEnabled
    {
        get
        {
            return widget.route.isCurrent && widget.route.popGestureEnabled;
        }
    }
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase
    {
        get => _phase;
        set
        {
            var phase = value;
            if ((!Equals(_phase, phase)) && mounted)
            {
                setState(() => { _ = _phase = phase; });
            }
        }
    }
    public virtual global::Doroti.Framework.Services.PredictiveBackEvent? startBackEvent
    {
        get => _startBackEvent;
        set
        {
            var startBackEvent = value;
            if ((!Equals(_startBackEvent, startBackEvent)) && mounted)
            {
                setState(() => { _ = _startBackEvent = startBackEvent; });
            }
        }
    }
    public virtual global::Doroti.Framework.Services.PredictiveBackEvent? currentBackEvent
    {
        get => _currentBackEvent;
        set
        {
            var currentBackEvent = value;
            if ((!Equals(_currentBackEvent, currentBackEvent)) && mounted)
            {
                setState(() => { _ = _currentBackEvent = currentBackEvent; });
            }
        }
    }
    public virtual bool handleStartBackGesture(global::Doroti.Framework.Services.PredictiveBackEvent backEvent)
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.start;
        bool gestureInProgress = !backEvent.isButtonEvent && _isEnabled;
        if (!gestureInProgress)
        {
            return false;
        }
        widget.route.handleStartBackGesture(progress: 1L - backEvent.progress);
        startBackEvent = currentBackEvent = backEvent;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void handleUpdateBackGestureProgress(global::Doroti.Framework.Services.PredictiveBackEvent backEvent)
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.update;
        widget.route.handleUpdateBackGestureProgress(progress: 1L - backEvent.progress);
        currentBackEvent = backEvent;
    }

    public virtual void handleCancelBackGesture()
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.cancel;
        widget.route.handleCancelBackGesture();
        startBackEvent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.PredictiveBackEvent>(currentBackEvent = null);
    }

    public virtual void handleCommitBackGesture()
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.commit;
        widget.route.handleCommitBackGesture();
        startBackEvent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Services.PredictiveBackEvent>(currentBackEvent = null);
    }

    public override void initState()
    {
        base.initState();
        WidgetsBinding.instance.addObserver(this);
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        _PredictiveBackPhase__predictive_back_page_transitions_builder effectivePhase = widget.route.popGestureInProgress ? phase : _PredictiveBackPhase__predictive_back_page_transitions_builder.idle;
        return widget.builder(context, effectivePhase, startBackEvent, currentBackEvent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual bool isDelegatedTransition { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase { get; private set; } = default!;
    public virtual global::Doroti.Framework.Services.PredictiveBackEvent? startBackEvent { get; private set; }
    public virtual global::Doroti.Framework.Services.PredictiveBackEvent? currentBackEvent { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder(bool isDelegatedTransition, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, _PredictiveBackPhase__predictive_back_page_transitions_builder phase, global::Doroti.Framework.Services.PredictiveBackEvent? startBackEvent, global::Doroti.Framework.Services.PredictiveBackEvent? currentBackEvent, global::Doroti.Framework.Widgets.Widget child)
    {
        this.isDelegatedTransition = isDelegatedTransition;
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.phase = phase;
        this.startBackEvent = startBackEvent;
        this.currentBackEvent = currentBackEvent;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder());
}

internal class _PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder : global::Doroti.Framework.Widgets.State<_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder>, global::Doroti.Framework.Widgets.SingleTickerProviderStateMixin<_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder>
{
    internal const double _kMinScale = 0.9;
    internal const double _kDivisionFactor = 20.0;
    internal const double _kMargin = 8.0;
    internal const double _kYPositionFactor = 0.1;
    internal const long _kCommitMilliseconds = 400L;
    internal static global::Doroti.Framework.Animation.Curve _kCurve = Curves.easeInOutCubicEmphasized;
    internal static global::Doroti.Framework.Animation.Interval _kCommitInterval = new global::Doroti.Framework.Animation.Interval(0.0, _kCommitMilliseconds / FadeForwardsPageTransitionsBuilder.kTransitionMilliseconds, curve: _kCurve);
    internal const double _kDeviceBorderRadius = 32.0;
    internal virtual global::Doroti.Framework.Animation.Tween<double> _borderRadiusTween { get; private set; } = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: _kDeviceBorderRadius);
    internal virtual global::Doroti.Framework.Animation.Tween<double> _opacityTween { get; private set; } = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0);
    internal virtual global::Doroti.Framework.Animation.Tween<double> _scaleTween { get; private set; } = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: _kMinScale);
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _commitAnimation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation();
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _bounceAnimation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation();
    internal virtual double _lastBounceAnimationValue { get; set; } = 0.0;
    internal virtual global::Doroti.Framework.Animation.ProxyAnimation _animation { get; private set; } = new global::Doroti.Framework.Animation.ProxyAnimation();
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation? _curvedAnimationReversed { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.Animation<Offset> _positionAnimation { get; set; } = default!;
    internal virtual Offset _lastDrag { get; set; } = Offset.zero;
    public virtual global::Doroti.Framework.Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual double _getYShiftPosition(double screenHeight)
    {
        double startTouchY = widget.startBackEvent?.touchOffset?.dy ?? 0;
        double currentTouchY = widget.currentBackEvent?.touchOffset?.dy ?? 0;
        double yShiftMax = screenHeight / _kDivisionFactor - _kMargin;
        double rawYShift = currentTouchY - startTouchY;
        double easedYShift = Curves.easeOut.transform(Dart_uiLibrary.clampDouble(rawYShift.abs() / screenHeight, 0.0, 1.0)) * Math.Sign(rawYShift) * yShiftMax;
        return Dart_uiLibrary.clampDouble(easedYShift, -yShiftMax, yShiftMax);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateAnimations(Size screenSize)
    {
        _animation.parent = widget.phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(_curvedAnimationReversed), _ => widget.animation };
        _bounceAnimation.parent = widget.phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: _lastBounceAnimationValue).animate(_curvedAnimation!), _ => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(new global::Doroti.Framework.Animation.ReverseAnimation(widget.animation)) };
        _commitAnimation.parent = widget.phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animation<double>>(_animation), _ => AnimationsLibrary.kAlwaysDismissedAnimation };
        double xShift = screenSize.width / _kDivisionFactor - _kMargin;
        _positionAnimation = _animation.drive(widget.phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: _lastDrag, end: new global::Doroti.Ui.Offset(screenSize.height * _kYPositionFactor, 0.0)), _ => new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: widget.currentBackEvent?.swipeEdge switch { SwipeEdge.left => new global::Doroti.Ui.Offset(xShift, _getYShiftPosition(screenSize.height)), SwipeEdge.right => new global::Doroti.Ui.Offset(-xShift, _getYShiftPosition(screenSize.height)), null => new global::Doroti.Ui.Offset(xShift, _getYShiftPosition(screenSize.height)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }, end: Offset.zero) });
    }

    internal virtual void _updateCurvedAnimations()
    {
        _curvedAnimation?.dispose();
        _curvedAnimationReversed?.dispose();
        _curvedAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: widget.animation, curve: _kCommitInterval);
        _curvedAnimationReversed = new global::Doroti.Framework.Animation.CurvedAnimation(parent: new global::Doroti.Framework.Animation.ReverseAnimation(widget.animation), curve: _kCommitInterval);
    }

    public override void initState()
    {
        base.initState();
    }

    public override void didUpdateWidget(_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.animation, oldWidget.animation))
        {
            _updateCurvedAnimations();
        }
        if ((!Equals(widget.phase, oldWidget.phase)) && Equals(widget.phase, _PredictiveBackPhase__predictive_back_page_transitions_builder.commit))
        {
            _updateAnimations(MediaQuery.sizeOf(context));
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _updateCurvedAnimations();
        _updateAnimations(MediaQuery.sizeOf(context));
    }

    public override void dispose()
    {
        _curvedAnimation!.dispose();
        _curvedAnimationReversed!.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((_ticker is null) || !_ticker!.isActive)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. The Ticker must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), _ticker!.describeForError("The offending ticker was") }));
            });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: widget.animation, builder: (context, child) =>
        {
            _lastBounceAnimationValue = _bounceAnimation.value;
            return Transform.CreateScale(scale: _scaleTween.evaluate(_bounceAnimation), child: Transform.CreateTranslate(offset: widget.phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => _positionAnimation.value, _ => _lastDrag = new global::Doroti.Ui.Offset(_positionAnimation.value.dx, _getYShiftPosition(MediaQuery.heightOf(context))) }, child: new global::Doroti.Framework.Widgets.Opacity(opacity: _opacityTween.evaluate(_commitAnimation), child: new global::Doroti.Framework.Widgets.ClipRRect(borderRadius: MediaQuery.displayCornerRadiiOf(context) ?? BorderRadius.CreateCircular(_borderRadiusTween.evaluate(_bounceAnimation)), child: child))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: widget.child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (_ticker is null)
                {
                    return true;
                }
                throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."), new global::Doroti.Framework.Foundation.ErrorDescription("A SingleTickerProviderStateMixin can only be used as a TickerProvider once."), new global::Doroti.Framework.Foundation.ErrorHint("If a State is used for multiple AnimationController objects, or if it is passed to other " + "objects and those objects might use it more than one time in total, then instead of " + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin.") }));
            });
        _ticker = new global::Doroti.Framework.Scheduler.Ticker(onTick, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTicker();
    }

    public virtual void _updateTicker()
    {
        TickerModeData values = _tickerModeNotifier!.value;
        if (_ticker is not null)
        {
            _ticker!.muted = !values.enabled;
            _ticker!.forceFrames = values.forceFrames;
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch { (true, true) => "active but muted", (true, _) => "active", (false, true) => "inactive and muted", (false, _) => "inactive", (null, _) => DartRuntimePrimitives.ConvertValue<string>(null) };
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Scheduler.Ticker>("ticker", _ticker, description: tickerDescription, showSeparator: false, defaultValue: default));
    }

}

internal class _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase { get; private set; } = default!;
    public virtual global::System.Func<bool> getIsCurrent { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder(global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Animation.Animation<double> secondaryAnimation, global::System.Func<bool> getIsCurrent, _PredictiveBackPhase__predictive_back_page_transitions_builder phase, global::Doroti.Framework.Widgets.Widget child)
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.getIsCurrent = getIsCurrent;
        this.phase = phase;
        this.child = child;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PredictiveBackFullscreenPageTransitionState__predictive_back_page_transitions_builder());
}

internal class _PredictiveBackFullscreenPageTransitionState__predictive_back_page_transitions_builder : global::Doroti.Framework.Widgets.State<_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder>
{
    internal const double _kScaleStart = 1.0;
    internal const double _kScaleCommit = 0.95;
    internal const double _kOpacityFullyOpened = 1.0;
    internal const double _kOpacityStartTransition = 0.95;
    internal const double _kCommitAt = 0.65;
    internal static double _kWeightPreCommit => _kCommitAt;
    internal static double _kWeightPostCommit = 1L - _kWeightPreCommit;
    internal const double _kScreenWidthDivisionFactor = 20.0;
    internal const double _kXShiftAdjustment = 8.0;
    internal static Duration _kCommitDuration = Duration.Create(milliseconds: 100L);
    internal virtual global::Doroti.Framework.Animation.Animatable<double> _primaryOpacityTween { get; private set; } = new global::Doroti.Framework.Animation.Tween<double>(begin: _kOpacityStartTransition, end: _kOpacityFullyOpened);
    internal virtual global::Doroti.Framework.Animation.Animatable<double> _primaryScaleTween { get; private set; } = new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: _kScaleStart, end: _kScaleStart), weight: _kWeightPreCommit), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: _kScaleCommit, end: _kScaleStart), weight: _kWeightPostCommit) });
    internal virtual global::Doroti.Framework.Animation.ConstantTween<double> _secondaryScaleTweenCurrent { get; private set; } = new global::Doroti.Framework.Animation.ConstantTween<double>(_kScaleStart);
    internal virtual global::Doroti.Framework.Animation.TweenSequence<double> _secondaryTweenScale { get; private set; } = new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: _kScaleCommit, end: _kScaleStart), weight: _kWeightPreCommit), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: _kScaleStart, end: _kScaleStart), weight: _kWeightPostCommit) });
    internal virtual global::Doroti.Framework.Animation.ConstantTween<double> _secondaryOpacityTweenCurrent { get; private set; } = new global::Doroti.Framework.Animation.ConstantTween<double>(_kOpacityFullyOpened);
    internal virtual global::Doroti.Framework.Animation.TweenSequence<double> _secondaryOpacityTween { get; private set; } = new global::Doroti.Framework.Animation.TweenSequence<double>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>> { new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: _kOpacityFullyOpened, end: _kOpacityStartTransition), weight: _kWeightPreCommit), new global::Doroti.Framework.Animation.TweenSequenceItem<double>(tween: new global::Doroti.Framework.Animation.Tween<double>(begin: _kOpacityFullyOpened, end: _kOpacityFullyOpened), weight: _kWeightPostCommit) });
    internal virtual global::Doroti.Framework.Animation.Animatable<Offset> _primaryPositionTween { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animatable<Offset> _secondaryPositionTween { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.Animatable<Offset> _secondaryCurrentPositionTween { get; set; } = default!;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        double screenWidth = MediaQuery.widthOf(context);
        double xShift = screenWidth / _kScreenWidthDivisionFactor - _kXShiftAdjustment;
        _primaryPositionTween = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animatable<Offset>>(new global::Doroti.Framework.Animation.TweenSequence<global::Doroti.Ui.Offset>(new List<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Offset>> { new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Offset>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: Offset.zero, end: Offset.zero), weight: _kWeightPreCommit), new global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Offset>(tween: new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(xShift, 0.0), end: Offset.zero), weight: _kWeightPostCommit) }.Cast<global::Doroti.Framework.Animation.TweenSequenceItem<global::Doroti.Ui.Offset>>().ToList()));
        _secondaryCurrentPositionTween = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animatable<Offset>>(new global::Doroti.Framework.Animation.ConstantTween<global::Doroti.Ui.Offset>(Offset.zero));
        _secondaryPositionTween = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Animation.Animatable<Offset>>(new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(xShift, 0.0), end: Offset.zero));
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _secondaryAnimatedBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget? child)
    {
        bool isCurrent = widget.getIsCurrent();
        return Transform.CreateTranslate(offset: isCurrent ? _secondaryCurrentPositionTween.evaluate(widget.secondaryAnimation) : _secondaryPositionTween.evaluate(widget.secondaryAnimation), child: Transform.CreateScale(scale: isCurrent ? _secondaryScaleTweenCurrent.evaluate(widget.secondaryAnimation) : _secondaryTweenScale.evaluate(widget.secondaryAnimation), child: new global::Doroti.Framework.Widgets.Opacity(opacity: isCurrent ? _secondaryOpacityTweenCurrent.evaluate(widget.secondaryAnimation) : _secondaryOpacityTween.evaluate(widget.secondaryAnimation), child: child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _primaryAnimatedBuilder(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget? child)
    {
        return Transform.CreateTranslate(offset: _primaryPositionTween.evaluate(widget.animation), child: Transform.CreateScale(scale: _primaryScaleTween.evaluate(widget.animation), child: new global::Doroti.Framework.Widgets.Opacity(opacity: _primaryOpacityTween.evaluate(widget.animation), child: new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: widget.phase switch { _PredictiveBackPhase__predictive_back_page_transitions_builder.commit => 0.0, _ => (widget.animation.value < _kCommitAt) ? 0.0 : 1.0 }, duration: _kCommitDuration, child: child))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: widget.secondaryAnimation, builder: _secondaryAnimatedBuilder, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: widget.animation, builder: _primaryAnimatedBuilder, child: new global::Doroti.Framework.Widgets.ClipRRect(borderRadius: MediaQuery.displayCornerRadiiOf(context) ?? BorderRadius.CreateAll(Radius.circular(_PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder._kDeviceBorderRadius)), child: widget.child)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
