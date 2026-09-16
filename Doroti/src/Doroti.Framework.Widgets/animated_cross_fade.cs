// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/animated_cross_fade.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public enum CrossFadeState
{
    showFirst,
    showSecond
}

public delegate Widget AnimatedCrossFadeBuilder(Widget topChild, Key topChildKey, Widget bottomChild, Key bottomChildKey);

public class AnimatedCrossFade : StatefulWidget
{
    public virtual Widget firstChild { get; private set; } = default!;
    public virtual Widget secondChild { get; private set; } = default!;
    public virtual CrossFadeState crossFadeState { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual Duration? reverseDuration { get; private set; }
    public virtual Curve firstCurve { get; private set; } = default!;
    public virtual Curve secondCurve { get; private set; } = default!;
    public virtual Curve sizeCurve { get; private set; } = default!;
    public virtual AlignmentGeometry alignment { get; private set; } = default!;
    public virtual Func<Widget, Key, Widget, Key, Widget> layoutBuilder { get; private set; } = default!;
    public virtual bool excludeBottomFocus { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual Action? onEnd { get; private set; }

    public AnimatedCrossFade(Key? key = null, Widget firstChild = default!, Widget secondChild = default!, Curve firstCurve = default!, Curve secondCurve = default!, Curve sizeCurve = default!, AlignmentGeometry alignment = default!, CrossFadeState crossFadeState = default!, Duration duration = default!, Duration? reverseDuration = null, Func<Widget, Key, Widget, Key, Widget> layoutBuilder = default!, bool excludeBottomFocus = true, Clip clipBehavior = Clip.hardEdge, Action? onEnd = null) : base(key: key)
    {
        Curve __firstCurve = firstCurve ?? Curves.linear;
        Curve __secondCurve = secondCurve ?? Curves.linear;
        Curve __sizeCurve = sizeCurve ?? Curves.linear;
        AlignmentGeometry __alignment = alignment ?? Alignment.topCenter;
        Func<Widget, Key, Widget, Key, Widget> __layoutBuilder = layoutBuilder ?? defaultLayoutBuilder;
        this.firstChild = firstChild;
        this.secondChild = secondChild;
        this.firstCurve = __firstCurve;
        this.secondCurve = __secondCurve;
        this.sizeCurve = __sizeCurve;
        this.alignment = __alignment;
        this.crossFadeState = crossFadeState;
        this.duration = duration;
        this.reverseDuration = reverseDuration;
        this.layoutBuilder = __layoutBuilder;
        this.excludeBottomFocus = excludeBottomFocus;
        this.clipBehavior = clipBehavior;
        this.onEnd = onEnd;
    }

    public static Widget defaultLayoutBuilder(Widget topChild, Key topChildKey, Widget bottomChild, Key bottomChildKey)
    {
        return new Stack(clipBehavior: Clip.none, children: new List<Widget> { new Positioned(key: bottomChildKey, left: 0.0, top: 0.0, right: 0.0, child: bottomChild), new Positioned(key: topChildKey, child: topChild) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _AnimatedCrossFadeState__animated_cross_fade());
    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new EnumProperty<CrossFadeState>("crossFadeState", crossFadeState));
        properties.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", alignment, defaultValue: Alignment.topCenter));
        properties.add(new IntProperty("duration", duration.inMilliseconds, unit: "ms"));
        properties.add(new IntProperty("reverseDuration", reverseDuration?.inMilliseconds, unit: "ms", defaultValue: null));
    }

}

internal class _AnimatedCrossFadeState__animated_cross_fade : State<AnimatedCrossFade>, TickerProviderStateMixin<AnimatedCrossFade>
{
    internal virtual AnimationController _controller { get; set; } = default!;
    internal virtual Animation<double> _firstAnimation { get; set; } = default!;
    internal virtual Animation<double> _secondAnimation { get; set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(duration: widget.duration, reverseDuration: widget.reverseDuration, vsync: this);
        if (Equals(widget.crossFadeState, CrossFadeState.showSecond))
        {
            _controller.value = 1.0;
        }
        _firstAnimation = _initAnimation(widget.firstCurve, true);
        _secondAnimation = _initAnimation(widget.secondCurve, false);
        _controller.addStatusListener((status) =>
        {
            setState(() =>
            {
            });
            if (Equals(status, AnimationStatus.completed) || Equals(status, AnimationStatus.dismissed))
            {
                widget.onEnd?.Invoke();
            }
        });
    }

    internal virtual Animation<double> _initAnimation(Curve curve, bool inverted)
    {
        Animation<double> result = _controller.drive(new CurveTween(curve: curve));
        if (inverted)
        {
            result = result.drive(new Tween<double>(begin: 1.0, end: 0.0));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void dispose()
    {
        _controller.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override void didUpdateWidget(AnimatedCrossFade oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.duration, oldWidget.duration))
        {
            _controller.duration = widget.duration;
        }
        if (!Equals(widget.reverseDuration, oldWidget.reverseDuration))
        {
            _controller.reverseDuration = widget.reverseDuration;
        }
        if (!Equals(widget.firstCurve, oldWidget.firstCurve))
        {
            _firstAnimation = _initAnimation(widget.firstCurve, true);
        }
        if (!Equals(widget.secondCurve, oldWidget.secondCurve))
        {
            _secondAnimation = _initAnimation(widget.secondCurve, false);
        }
        if (!Equals(widget.crossFadeState, oldWidget.crossFadeState))
        {
            switch (widget.crossFadeState)
            {
                case CrossFadeState.showFirst:
                    {
                        _controller.reverse();
                        break;
                    }
                case CrossFadeState.showSecond:
                    {
                        _controller.forward();
                        break;
                    }
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        Key kFirstChildKey = new ValueKey<CrossFadeState>(CrossFadeState.showFirst);
        Key kSecondChildKey = new ValueKey<CrossFadeState>(CrossFadeState.showSecond);
        Key topKey = default!;
        Widget topChild = default!;
        Animation<double> topAnimation = default!;
        Key bottomKey = default!;
        Widget bottomChild = default!;
        Animation<double> bottomAnimation = default!;
        if (_controller.isForwardOrCompleted)
        {
            topKey = kSecondChildKey;
            topChild = widget.secondChild;
            topAnimation = _secondAnimation;
            bottomKey = kFirstChildKey;
            bottomChild = widget.firstChild;
            bottomAnimation = _firstAnimation;
        }
        else
        {
            topKey = kFirstChildKey;
            topChild = widget.firstChild;
            topAnimation = _firstAnimation;
            bottomKey = kSecondChildKey;
            bottomChild = widget.secondChild;
            bottomAnimation = _secondAnimation;
        }
        bottomChild = DartRuntimePrimitives.ConvertValue<Widget>(new TickerMode(key: bottomKey, enabled: _controller.isAnimating, child: new IgnorePointer(child: new ExcludeSemantics(child: new ExcludeFocus(excluding: widget.excludeBottomFocus, child: new FadeTransition(opacity: bottomAnimation, child: bottomChild))))));
        topChild = DartRuntimePrimitives.ConvertValue<Widget>(new TickerMode(key: topKey, enabled: true, child: new IgnorePointer(ignoring: false, child: new ExcludeSemantics(excluding: false, child: new ExcludeFocus(excluding: false, child: new FadeTransition(opacity: topAnimation, child: topChild))))));
        return new ClipRect(clipBehavior: widget.clipBehavior, child: new AnimatedSize(alignment: widget.alignment, duration: widget.duration, reverseDuration: widget.reverseDuration, curve: widget.sizeCurve, clipBehavior: widget.clipBehavior, child: widget.layoutBuilder(topChild, topKey, bottomChild, bottomKey)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
        description.add(new EnumProperty<CrossFadeState>("crossFadeState", widget.crossFadeState));
        description.add(new DiagnosticsProperty<AnimationController>("controller", _controller, showName: false));
        description.add(new DiagnosticsProperty<AlignmentGeometry>("alignment", widget.alignment, defaultValue: Alignment.topCenter));
        description.add(new EnumProperty<Clip>("clipBehavior", widget.clipBehavior, defaultValue: Clip.hardEdge));
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
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

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
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
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

}

