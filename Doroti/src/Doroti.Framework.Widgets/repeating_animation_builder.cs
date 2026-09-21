// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/repeating_animation_builder.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public enum RepeatMode
{
    restart,
    reverse,
}

public class RepeatingAnimationBuilder<T> : StatefulWidget
{
    public virtual Animatable<T> animatable { get; private set; } = default!;
    public virtual Duration duration { get; private set; } = default!;
    public virtual Curve curve { get; private set; } = default!;
    public virtual Func<BuildContext, T, Widget?, Widget> builder { get; private set; } = default!;
    public virtual Widget? child { get; private set; }
    public virtual RepeatMode repeatMode { get; private set; } = default!;
    public virtual bool paused { get; private set; } = default!;

    public RepeatingAnimationBuilder(
        Key? key = null,
        Animatable<T> animatable = default!,
        Duration duration = default!,
        Curve curve = default!,
        RepeatMode repeatMode = RepeatMode.restart,
        bool paused = false,
        Func<BuildContext, T, Widget?, Widget> builder = default!,
        Widget? child = null
    )
        : base(key: key)
    {
        Curve __curve = curve ?? Curves.linear;
        this.animatable = animatable;
        this.duration = duration;
        this.curve = __curve;
        this.repeatMode = repeatMode;
        this.paused = paused;
        this.builder = builder;
        this.child = child;
    }

    public override IState createState()
    {
        return new _RepeatingAnimationBuilderState__repeating_animation_builder<T>();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _RepeatingAnimationBuilderState__repeating_animation_builder<T>
    : State<RepeatingAnimationBuilder<T>>,
        SingleTickerProviderStateMixin<RepeatingAnimationBuilder<T>>
{
    internal virtual AnimationController _controller { get; private set; } = default!;
    internal virtual CurvedAnimation _curvedAnimation { get; private set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _controller = new AnimationController(duration: widget.duration, vsync: this);
        _curvedAnimation = new CurvedAnimation(parent: _controller, curve: widget.curve);
        if (!widget.paused)
        {
            _controller.repeat(reverse: Equals(widget.repeatMode, RepeatMode.reverse));
        }
    }

    public override void didUpdateWidget(RepeatingAnimationBuilder<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.duration, oldWidget.duration))
        {
            _controller.duration = widget.duration;
        }
        if (!Equals(widget.curve, oldWidget.curve))
        {
            _curvedAnimation.curve = widget.curve;
        }
        if (widget.paused)
        {
            if (!oldWidget.paused || _controller.isAnimating)
            {
                _controller.stop(canceled: false);
            }
            return;
        }
        bool shouldRestart =
            oldWidget.paused
            || (!Equals(widget.repeatMode, oldWidget.repeatMode))
            || (!Equals(widget.duration, oldWidget.duration))
            || !_controller.isAnimating;
        if (shouldRestart)
        {
            _controller.repeat(reverse: Equals(widget.repeatMode, RepeatMode.reverse));
        }
    }

    public override void dispose()
    {
        _curvedAnimation.dispose();
        _controller.dispose();
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
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        _tickerModeNotifier?.removeListener(_updateTicker);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new AnimatedBuilder(
            animation: _curvedAnimation,
            builder: (context, child) =>
            {
                T valueLocal = widget.animatable.transform(_curvedAnimation.value);
                return widget.builder(context, valueLocal, child);
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
            throw new InvalidOperationException("Callback completed without returning a value.");
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
