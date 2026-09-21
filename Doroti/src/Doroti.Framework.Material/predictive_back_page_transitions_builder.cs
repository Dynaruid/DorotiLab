// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/predictive_back_page_transitions_builder.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class PredictiveBackPageTransitionsBuilder : PageTransitionsBuilder
{
    public virtual Color? fallbackColor { get; private set; }

    public PredictiveBackPageTransitionsBuilder(Color? fallbackColor = null)
    {
        this.fallbackColor = fallbackColor;
    }

    public override Duration transitionDuration =>
        Duration.Create(milliseconds: FadeForwardsPageTransitionsBuilder.kTransitionMilliseconds);

    public override Widget buildTransitions<T>(
        PageRoute<T> route,
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        return new _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(
            route: route,
            builder: (context, phase, startBackEvent, currentBackEvent) =>
            {
                if (route.popGestureInProgress)
                {
                    return new _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder(
                        isDelegatedTransition: true,
                        animation: animation,
                        phase: phase,
                        secondaryAnimation: secondaryAnimation,
                        startBackEvent: startBackEvent,
                        currentBackEvent: currentBackEvent,
                        child: child
                    );
                }
                return new FadeForwardsPageTransitionsBuilder(
                    backgroundColor: fallbackColor
                ).buildTransitions(route, context, animation, secondaryAnimation, child);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class PredictiveBackFullscreenPageTransitionsBuilder : PageTransitionsBuilder
{
    public virtual Color? fallbackColor { get; private set; }

    public PredictiveBackFullscreenPageTransitionsBuilder(Color? fallbackColor = null)
    {
        this.fallbackColor = fallbackColor;
    }

    public override Widget buildTransitions<T>(
        PageRoute<T> route,
        BuildContext context,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Widget child
    )
    {
        return new _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(
            route: route,
            builder: (context, phase, startBackEvent, currentBackEvent) =>
            {
                if (route.popGestureInProgress)
                {
                    return new _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder(
                        animation: animation,
                        secondaryAnimation: secondaryAnimation,
                        getIsCurrent: () => route.isCurrent,
                        phase: phase,
                        child: child
                    );
                }
                return new ZoomPageTransitionsBuilder(
                    backgroundColor: fallbackColor
                ).buildTransitions(route, context, animation, secondaryAnimation, child);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal delegate Widget _PredictiveBackGestureDetectorWidgetBuilder__predictive_back_page_transitions_builder(
    BuildContext context,
    _PredictiveBackPhase__predictive_back_page_transitions_builder phase,
    PredictiveBackEvent? startBackEvent,
    PredictiveBackEvent? currentBackEvent
);

public enum _PredictiveBackPhase__predictive_back_page_transitions_builder
{
    idle,
    start,
    update,
    commit,
    cancel,
}

internal class _PredictiveBackGestureDetector__predictive_back_page_transitions_builder
    : StatefulWidget
{
    public virtual Func<
        BuildContext,
        _PredictiveBackPhase__predictive_back_page_transitions_builder,
        PredictiveBackEvent?,
        PredictiveBackEvent?,
        Widget
    > builder { get; private set; } = default!;
    public virtual IModalRoute route { get; private set; } = default!;

    internal _PredictiveBackGestureDetector__predictive_back_page_transitions_builder(
        IModalRoute route,
        Func<
            BuildContext,
            _PredictiveBackPhase__predictive_back_page_transitions_builder,
            PredictiveBackEvent?,
            PredictiveBackEvent?,
            Widget
        > builder
    )
    {
        this.route = route;
        this.builder = builder;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _PredictiveBackGestureDetectorState__predictive_back_page_transitions_builder()
        );
}

internal class _PredictiveBackGestureDetectorState__predictive_back_page_transitions_builder
    : State<_PredictiveBackGestureDetector__predictive_back_page_transitions_builder>,
        WidgetsBindingObserver
{
    internal virtual _PredictiveBackPhase__predictive_back_page_transitions_builder _phase { get; set; } =
        _PredictiveBackPhase__predictive_back_page_transitions_builder.idle;
    internal virtual PredictiveBackEvent? _startBackEvent { get; set; } = default;
    internal virtual PredictiveBackEvent? _currentBackEvent { get; set; } = default;

    internal virtual bool _isEnabled
    {
        get { return widget.route.isCurrent && widget.route.popGestureEnabled; }
    }
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase
    {
        get => _phase;
        set
        {
            var phase = value;
            if ((!Equals(_phase, phase)) && mounted)
            {
                setState(() =>
                {
                    _ = _phase = phase;
                });
            }
        }
    }
    public virtual PredictiveBackEvent? startBackEvent
    {
        get => _startBackEvent;
        set
        {
            var startBackEvent = value;
            if ((!Equals(_startBackEvent, startBackEvent)) && mounted)
            {
                setState(() =>
                {
                    _ = _startBackEvent = startBackEvent;
                });
            }
        }
    }
    public virtual PredictiveBackEvent? currentBackEvent
    {
        get => _currentBackEvent;
        set
        {
            var currentBackEvent = value;
            if ((!Equals(_currentBackEvent, currentBackEvent)) && mounted)
            {
                setState(() =>
                {
                    _ = _currentBackEvent = currentBackEvent;
                });
            }
        }
    }

    public virtual bool handleStartBackGesture(PredictiveBackEvent backEvent)
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void handleUpdateBackGestureProgress(PredictiveBackEvent backEvent)
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.update;
        widget.route.handleUpdateBackGestureProgress(progress: 1L - backEvent.progress);
        currentBackEvent = backEvent;
    }

    public virtual void handleCancelBackGesture()
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.cancel;
        widget.route.handleCancelBackGesture();
        startBackEvent = DartRuntimePrimitives.ConvertValue<PredictiveBackEvent>(
            currentBackEvent = null
        );
    }

    public virtual void handleCommitBackGesture()
    {
        phase = _PredictiveBackPhase__predictive_back_page_transitions_builder.commit;
        widget.route.handleCommitBackGesture();
        startBackEvent = DartRuntimePrimitives.ConvertValue<PredictiveBackEvent>(
            currentBackEvent = null
        );
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

    public override Widget build(BuildContext context)
    {
        _PredictiveBackPhase__predictive_back_page_transitions_builder effectivePhase = widget
            .route
            .popGestureInProgress
            ? phase
            : _PredictiveBackPhase__predictive_back_page_transitions_builder.idle;
        return widget.builder(context, effectivePhase, startBackEvent, currentBackEvent);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder
    : StatefulWidget
{
    public virtual bool isDelegatedTransition { get; private set; } = default!;
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase
    {
        get;
        private set;
    } = default!;
    public virtual PredictiveBackEvent? startBackEvent { get; private set; }
    public virtual PredictiveBackEvent? currentBackEvent { get; private set; }
    public virtual Widget child { get; private set; } = default!;

    internal _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder(
        bool isDelegatedTransition,
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        _PredictiveBackPhase__predictive_back_page_transitions_builder phase,
        PredictiveBackEvent? startBackEvent,
        PredictiveBackEvent? currentBackEvent,
        Widget child
    )
    {
        this.isDelegatedTransition = isDelegatedTransition;
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.phase = phase;
        this.startBackEvent = startBackEvent;
        this.currentBackEvent = currentBackEvent;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder()
        );
}

internal class _PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder
    : State<_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder>,
        SingleTickerProviderStateMixin<_PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder>
{
    internal const double _kMinScale = 0.9;
    internal const double _kDivisionFactor = 20.0;
    internal const double _kMargin = 8.0;
    internal const double _kYPositionFactor = 0.1;
    internal const long _kCommitMilliseconds = 400L;
    internal static Curve _kCurve = Curves.easeInOutCubicEmphasized;
    internal static Interval _kCommitInterval = new Interval(
        0.0,
        _kCommitMilliseconds / FadeForwardsPageTransitionsBuilder.kTransitionMilliseconds,
        curve: _kCurve
    );
    internal const double _kDeviceBorderRadius = 32.0;
    internal virtual Tween<double> _borderRadiusTween { get; private set; } =
        new Tween<double>(begin: 0.0, end: _kDeviceBorderRadius);
    internal virtual Tween<double> _opacityTween { get; private set; } =
        new Tween<double>(begin: 1.0, end: 0.0);
    internal virtual Tween<double> _scaleTween { get; private set; } =
        new Tween<double>(begin: 1.0, end: _kMinScale);
    internal virtual ProxyAnimation _commitAnimation { get; private set; } = new ProxyAnimation();
    internal virtual ProxyAnimation _bounceAnimation { get; private set; } = new ProxyAnimation();
    internal virtual double _lastBounceAnimationValue { get; set; } = 0.0;
    internal virtual ProxyAnimation _animation { get; private set; } = new ProxyAnimation();
    internal virtual CurvedAnimation? _curvedAnimation { get; set; } = default;
    internal virtual CurvedAnimation? _curvedAnimationReversed { get; set; } = default;
    internal virtual Animation<Offset> _positionAnimation { get; set; } = default!;
    internal virtual Offset _lastDrag { get; set; } = Offset.zero;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual double _getYShiftPosition(double screenHeight)
    {
        double startTouchY = widget.startBackEvent?.touchOffset?.dy ?? 0;
        double currentTouchY = widget.currentBackEvent?.touchOffset?.dy ?? 0;
        double yShiftMax = (screenHeight / _kDivisionFactor) - _kMargin;
        double rawYShift = currentTouchY - startTouchY;
        double easedYShift =
            Curves.easeOut.transform(
                Dart_uiLibrary.clampDouble(rawYShift.abs() / screenHeight, 0.0, 1.0)
            )
            * Math.Sign(rawYShift)
            * yShiftMax;
        return Dart_uiLibrary.clampDouble(easedYShift, -yShiftMax, yShiftMax);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _updateAnimations(Size screenSize)
    {
        _animation.parent = widget.phase switch
        {
            _PredictiveBackPhase__predictive_back_page_transitions_builder.commit =>
                DartRuntimePrimitives.ConvertValue<Animation<double>>(_curvedAnimationReversed),
            _ => widget.animation,
        };
        _bounceAnimation.parent = widget.phase switch
        {
            _PredictiveBackPhase__predictive_back_page_transitions_builder.commit =>
                new Tween<double>(begin: 0.0, end: _lastBounceAnimationValue).animate(
                    _curvedAnimation!
                ),
            _ => DartRuntimePrimitives.ConvertValue<Animation<double>>(
                new ReverseAnimation(widget.animation)
            ),
        };
        _commitAnimation.parent = widget.phase switch
        {
            _PredictiveBackPhase__predictive_back_page_transitions_builder.commit =>
                DartRuntimePrimitives.ConvertValue<Animation<double>>(_animation),
            _ => AnimationsLibrary.kAlwaysDismissedAnimation,
        };
        double xShift = (screenSize.width / _kDivisionFactor) - _kMargin;
        _positionAnimation = _animation.drive(
            widget.phase switch
            {
                _PredictiveBackPhase__predictive_back_page_transitions_builder.commit =>
                    new Tween<Offset>(
                        begin: _lastDrag,
                        end: new Offset(screenSize.height * _kYPositionFactor, 0.0)
                    ),
                _ => new Tween<Offset>(
                    begin: widget.currentBackEvent?.swipeEdge switch
                    {
                        SwipeEdge.left => new Offset(xShift, _getYShiftPosition(screenSize.height)),
                        SwipeEdge.right => new Offset(
                            -xShift,
                            _getYShiftPosition(screenSize.height)
                        ),
                        null => new Offset(xShift, _getYShiftPosition(screenSize.height)),
                        _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                            throw new InvalidOperationException(
                                "Switch expression did not handle the supplied value."
                            ),
                    },
                    end: Offset.zero
                ),
            }
        );
    }

    internal virtual void _updateCurvedAnimations()
    {
        _curvedAnimation?.dispose();
        _curvedAnimationReversed?.dispose();
        _curvedAnimation = new CurvedAnimation(parent: widget.animation, curve: _kCommitInterval);
        _curvedAnimationReversed = new CurvedAnimation(
            parent: new ReverseAnimation(widget.animation),
            curve: _kCommitInterval
        );
    }

    public override void initState()
    {
        base.initState();
    }

    public override void didUpdateWidget(
        _PredictiveBackSharedElementPageTransition__predictive_back_page_transitions_builder oldWidget
    )
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.animation, oldWidget.animation))
        {
            _updateCurvedAnimations();
        }
        if (
            (!Equals(widget.phase, oldWidget.phase))
            && Equals(
                widget.phase,
                _PredictiveBackPhase__predictive_back_page_transitions_builder.commit
            )
        )
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
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary($"{this} was disposed with an active Ticker."),
                        new ErrorDescription(
                            $"{GetType()} created a Ticker via its SingleTickerProviderStateMixin, but at the time "
                                + "dispose() was called on the mixin, that Ticker was still active. The Ticker must "
                                + "be disposed before calling super.dispose()."
                        ),
                        new ErrorHint(
                            "Tickers used by AnimationControllers "
                                + "should be disposed by calling dispose() on the AnimationController itself. "
                                + "Otherwise, the ticker will leak."
                        ),
                        _ticker!.describeForError("The offending ticker was"),
                    }
                )
            );
        });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new AnimatedBuilder(
            animation: widget.animation,
            builder: (context, child) =>
            {
                _lastBounceAnimationValue = _bounceAnimation.value;
                return Transform.CreateScale(
                    scale: _scaleTween.evaluate(_bounceAnimation),
                    child: Transform.CreateTranslate(
                        offset: widget.phase switch
                        {
                            _PredictiveBackPhase__predictive_back_page_transitions_builder.commit =>
                                _positionAnimation.value,
                            _ => _lastDrag = new Offset(
                                _positionAnimation.value.dx,
                                _getYShiftPosition(MediaQuery.heightOf(context))
                            ),
                        },
                        child: new Opacity(
                            opacity: _opacityTween.evaluate(_commitAnimation),
                            child: new ClipRRect(
                                borderRadius: MediaQuery.displayCornerRadiiOf(context)
                                    ?? BorderRadius.CreateCircular(
                                        _borderRadiusTween.evaluate(_bounceAnimation)
                                    ),
                                child: child
                            )
                        )
                    )
                );
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: widget.child
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (_ticker is null)
            {
                return true;
            }
            throw DartRuntimePrimitives.AsException(
                new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            $"{GetType()} is a SingleTickerProviderStateMixin but multiple tickers were created."
                        ),
                        new ErrorDescription(
                            "A SingleTickerProviderStateMixin can only be used as a TickerProvider once."
                        ),
                        new ErrorHint(
                            "If a State is used for multiple AnimationController objects, or if it is passed to other "
                                + "objects and those objects might use it more than one time in total, then instead of "
                                + "mixing in a SingleTickerProviderStateMixin, use a regular TickerProviderStateMixin."
                        ),
                    }
                )
            );
        });
        _ticker = new Scheduler.Ticker(
            onTick,
            debugLabel: Foundation.ConstantsLibrary.kDebugMode
                ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                : null
        );
        _updateTickerModeNotifier();
        _updateTicker();
        return _ticker!;
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTicker);
        newNotifier.addListener(_updateTicker);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        string? tickerDescription = (_ticker?.isActive, _ticker?.muted) switch
        {
            (true, true) => "active but muted",
            (true, _) => "active",
            (false, true) => "inactive and muted",
            (false, _) => "inactive",
            (null, _) => DartRuntimePrimitives.ConvertValue<string>(null),
        };
        properties.add(
            new DiagnosticsProperty<Scheduler.Ticker>(
                "ticker",
                _ticker,
                description: tickerDescription,
                showSeparator: false,
                defaultValue: default
            )
        );
    }
}

internal class _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder
    : StatefulWidget
{
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual Animation<double> secondaryAnimation { get; private set; } = default!;
    public virtual _PredictiveBackPhase__predictive_back_page_transitions_builder phase
    {
        get;
        private set;
    } = default!;
    public virtual Func<bool> getIsCurrent { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder(
        Animation<double> animation,
        Animation<double> secondaryAnimation,
        Func<bool> getIsCurrent,
        _PredictiveBackPhase__predictive_back_page_transitions_builder phase,
        Widget child
    )
    {
        this.animation = animation;
        this.secondaryAnimation = secondaryAnimation;
        this.getIsCurrent = getIsCurrent;
        this.phase = phase;
        this.child = child;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _PredictiveBackFullscreenPageTransitionState__predictive_back_page_transitions_builder()
        );
}

internal class _PredictiveBackFullscreenPageTransitionState__predictive_back_page_transitions_builder
    : State<_PredictiveBackFullscreenPageTransition__predictive_back_page_transitions_builder>
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
    internal virtual Animatable<double> _primaryOpacityTween { get; private set; } =
        new Tween<double>(begin: _kOpacityStartTransition, end: _kOpacityFullyOpened);
    internal virtual Animatable<double> _primaryScaleTween { get; private set; } =
        new TweenSequence<double>(
            new List<TweenSequenceItem<double>>
            {
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: _kScaleStart, end: _kScaleStart),
                    weight: _kWeightPreCommit
                ),
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: _kScaleCommit, end: _kScaleStart),
                    weight: _kWeightPostCommit
                ),
            }
        );
    internal virtual ConstantTween<double> _secondaryScaleTweenCurrent { get; private set; } =
        new ConstantTween<double>(_kScaleStart);
    internal virtual TweenSequence<double> _secondaryTweenScale { get; private set; } =
        new TweenSequence<double>(
            new List<TweenSequenceItem<double>>
            {
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: _kScaleCommit, end: _kScaleStart),
                    weight: _kWeightPreCommit
                ),
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(begin: _kScaleStart, end: _kScaleStart),
                    weight: _kWeightPostCommit
                ),
            }
        );
    internal virtual ConstantTween<double> _secondaryOpacityTweenCurrent { get; private set; } =
        new ConstantTween<double>(_kOpacityFullyOpened);
    internal virtual TweenSequence<double> _secondaryOpacityTween { get; private set; } =
        new TweenSequence<double>(
            new List<TweenSequenceItem<double>>
            {
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(
                        begin: _kOpacityFullyOpened,
                        end: _kOpacityStartTransition
                    ),
                    weight: _kWeightPreCommit
                ),
                new TweenSequenceItem<double>(
                    tween: new Tween<double>(
                        begin: _kOpacityFullyOpened,
                        end: _kOpacityFullyOpened
                    ),
                    weight: _kWeightPostCommit
                ),
            }
        );
    internal virtual Animatable<Offset> _primaryPositionTween { get; set; } = default!;
    internal virtual Animatable<Offset> _secondaryPositionTween { get; set; } = default!;
    internal virtual Animatable<Offset> _secondaryCurrentPositionTween { get; set; } = default!;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        double screenWidth = MediaQuery.widthOf(context);
        double xShift = (screenWidth / _kScreenWidthDivisionFactor) - _kXShiftAdjustment;
        _primaryPositionTween = DartRuntimePrimitives.ConvertValue<Animatable<Offset>>(
            new TweenSequence<Offset>(
                new List<TweenSequenceItem<Offset>>
                {
                    new TweenSequenceItem<Offset>(
                        tween: new Tween<Offset>(begin: Offset.zero, end: Offset.zero),
                        weight: _kWeightPreCommit
                    ),
                    new TweenSequenceItem<Offset>(
                        tween: new Tween<Offset>(begin: new Offset(xShift, 0.0), end: Offset.zero),
                        weight: _kWeightPostCommit
                    ),
                }
                    .Cast<TweenSequenceItem<Offset>>()
                    .ToList()
            )
        );
        _secondaryCurrentPositionTween = DartRuntimePrimitives.ConvertValue<Animatable<Offset>>(
            new ConstantTween<Offset>(Offset.zero)
        );
        _secondaryPositionTween = DartRuntimePrimitives.ConvertValue<Animatable<Offset>>(
            new Tween<Offset>(begin: new Offset(xShift, 0.0), end: Offset.zero)
        );
    }

    internal virtual Widget _secondaryAnimatedBuilder(BuildContext context, Widget? child)
    {
        bool isCurrent = widget.getIsCurrent();
        return Transform.CreateTranslate(
            offset: isCurrent
                ? _secondaryCurrentPositionTween.evaluate(widget.secondaryAnimation)
                : _secondaryPositionTween.evaluate(widget.secondaryAnimation),
            child: Transform.CreateScale(
                scale: isCurrent
                    ? _secondaryScaleTweenCurrent.evaluate(widget.secondaryAnimation)
                    : _secondaryTweenScale.evaluate(widget.secondaryAnimation),
                child: new Opacity(
                    opacity: isCurrent
                        ? _secondaryOpacityTweenCurrent.evaluate(widget.secondaryAnimation)
                        : _secondaryOpacityTween.evaluate(widget.secondaryAnimation),
                    child: child
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget _primaryAnimatedBuilder(BuildContext context, Widget? child)
    {
        return Transform.CreateTranslate(
            offset: _primaryPositionTween.evaluate(widget.animation),
            child: Transform.CreateScale(
                scale: _primaryScaleTween.evaluate(widget.animation),
                child: new Opacity(
                    opacity: _primaryOpacityTween.evaluate(widget.animation),
                    child: new AnimatedOpacity(
                        opacity: widget.phase switch
                        {
                            _PredictiveBackPhase__predictive_back_page_transitions_builder.commit =>
                                0.0,
                            _ => (widget.animation.value < _kCommitAt) ? 0.0 : 1.0,
                        },
                        duration: _kCommitDuration,
                        child: child
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        return new AnimatedBuilder(
            animation: widget.secondaryAnimation,
            builder: _secondaryAnimatedBuilder,
            child: new AnimatedBuilder(
                animation: widget.animation,
                builder: _primaryAnimatedBuilder,
                child: new ClipRRect(
                    borderRadius: MediaQuery.displayCornerRadiiOf(context)
                        ?? BorderRadius.CreateAll(
                            Radius.circular(
                                _PredictiveBackSharedElementPageTransitionState__predictive_back_page_transitions_builder._kDeviceBorderRadius
                            )
                        ),
                    child: widget.child
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
