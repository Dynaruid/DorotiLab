// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/expansible.dart
using Doroti.Runtime;

namespace Doroti.Framework.Widgets;

public delegate Widget ExpansibleComponentBuilder(
    BuildContext context,
    Animation<double> animation
);

public delegate Widget ExpansibleBuilder(
    BuildContext context,
    Widget header,
    Widget body,
    Animation<double> animation
);

public class ExpansibleController : ChangeNotifier
{
    internal virtual bool _isExpanded { get; set; } = false;

    public ExpansibleController() { }

    internal virtual void _setExpansionState(bool newValue)
    {
        if (newValue != _isExpanded)
        {
            _isExpanded = newValue;
            notifyListeners();
        }
    }

    public virtual bool isExpanded => _isExpanded;

    public virtual void expand()
    {
        _setExpansionState(true);
    }

    public virtual void collapse()
    {
        _setExpansionState(false);
    }

    public virtual void toggle() =>
        (
            (Action)(
                () =>
                {
                    if (isExpanded)
                    {
                        collapse();
                    }
                    else
                    {
                        expand();
                    }
                }
            )
        )();

    public static ExpansibleController of(BuildContext context)
    {
        _ExpansibleState__expansible? result =
            context.findAncestorStateOfType<_ExpansibleState__expansible>();
        DartRuntimePrimitives.Assert(() =>
        {
            if (result is null)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                "ExpansibleController.of() called with a context that does not contain a Expansible."
                            ),
                            new ErrorDescription(
                                "No Expansible ancestor could be found starting from the context that was passed to ExpansibleController.of(). "
                                    + "This usually happens when the context provided is from the same StatefulWidget as that "
                                    + "whose build function actually creates the Expansible widget being sought."
                            ),
                            new ErrorHint(
                                "There are several ways to avoid this problem. The simplest is to use a Builder to get a "
                                    + "context that is \"under\" the Expansible. "
                            ),
                            new ErrorHint(
                                "A more efficient solution is to split your build function into several widgets. This "
                                    + "introduces a new context from which you can obtain the Expansible. In this solution, "
                                    + "you would have an outer widget that creates the Expansible populated by instances of "
                                    + "your new inner widgets, and then in these inner widgets you would use ExpansibleController.of().\n"
                                    + "An other solution is assign a GlobalKey to the Expansible, "
                                    + "then use the key.currentState property to obtain the Expansible rather than "
                                    + "using the ExpansibleController.of() function."
                            ),
                            context.describeElement("The context used was"),
                        }
                    )
                );
            }
            return true;
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return result!.widget.controller;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ExpansibleController? maybeOf(BuildContext context)
    {
        return context.findAncestorStateOfType<_ExpansibleState__expansible>()?.widget.controller;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class Expansible : StatefulWidget
{
    public virtual ExpansibleController controller { get; private set; } = default!;
    public virtual Func<BuildContext, Animation<double>, Widget> headerBuilder
    {
        get;
        private set;
    } = default!;
    public virtual Func<BuildContext, Animation<double>, Widget> bodyBuilder { get; private set; } =
        default!;
    public virtual AnimationStyle? animationStyle { get; private set; }
    public virtual Duration duration { get; private set; } = default!;
    public virtual Curve curve { get; private set; } = default!;
    public virtual Curve? reverseCurve { get; private set; }
    public virtual bool maintainState { get; private set; } = default!;
    public virtual Func<BuildContext, Widget, Widget, Animation<double>, Widget> expansibleBuilder
    {
        get;
        private set;
    } = default!;

    public Expansible(
        Key? key = null,
        Func<BuildContext, Animation<double>, Widget> headerBuilder = default!,
        Func<BuildContext, Animation<double>, Widget> bodyBuilder = default!,
        ExpansibleController controller = default!,
        Func<BuildContext, Widget, Widget, Animation<double>, Widget> expansibleBuilder = default!,
        AnimationStyle? animationStyle = null,
        Duration? duration = null,
        Curve curve = default!,
        Curve? reverseCurve = null,
        bool maintainState = true
    )
        : base(key: key)
    {
        Func<BuildContext, Widget, Widget, Animation<double>, Widget> __expansibleBuilder =
            expansibleBuilder ?? _defaultExpansibleBuilder;
        Duration __duration = duration ?? Duration.Create(milliseconds: 200);
        Curve __curve = curve ?? Curves.ease;
        this.headerBuilder = headerBuilder;
        this.bodyBuilder = bodyBuilder;
        this.controller = controller;
        this.expansibleBuilder = __expansibleBuilder;
        this.animationStyle = animationStyle;
        this.duration = __duration;
        this.curve = __curve;
        this.reverseCurve = reverseCurve;
        this.maintainState = maintainState;
    }

    internal static Widget _defaultExpansibleBuilder(
        BuildContext context,
        Widget header,
        Widget body,
        Animation<double> animation
    )
    {
        return new Column(
            mainAxisSize: MainAxisSize.min,
            children: new List<Widget> { header, body }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ExpansibleState__expansible());
}

internal class _ExpansibleState__expansible
    : State<Expansible>,
        SingleTickerProviderStateMixin<Expansible>
{
    internal virtual AnimationController _animationController { get; set; } = default!;
    internal virtual CurvedAnimation _heightFactor { get; set; } = default!;
    public virtual Scheduler.Ticker? _ticker { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    internal virtual Duration _duration
    {
        get { return widget.animationStyle?.duration ?? widget.duration; }
    }
    internal virtual Curve _curve
    {
        get { return widget.animationStyle?.curve ?? widget.curve; }
    }
    internal virtual Curve? _reverseCurve
    {
        get { return widget.animationStyle?.reverseCurve ?? widget.reverseCurve; }
    }

    public override void initState()
    {
        base.initState();
        _animationController = new AnimationController(duration: _duration, vsync: this);
        bool initiallyExpanded =
            ((bool?)PageStorage.maybeOf(context)?.readState(context))
            ?? widget.controller.isExpanded;
        if (initiallyExpanded)
        {
            _animationController.value = 1.0;
            widget.controller.expand();
        }
        else
        {
            widget.controller.collapse();
        }
        var heightFactorTween = new Tween<double>(begin: 0.0, end: 1.0);
        _heightFactor = new CurvedAnimation(
            parent: _animationController.drive(heightFactorTween),
            curve: _curve,
            reverseCurve: _reverseCurve
        );
        widget.controller.addListener(_toggleExpansion);
    }

    public override void didUpdateWidget(Expansible oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        Duration oldDuration = oldWidget.animationStyle?.duration ?? oldWidget.duration;
        Curve oldCurve = oldWidget.animationStyle?.curve ?? oldWidget.curve;
        Curve? oldReverseCurve = oldWidget.animationStyle?.reverseCurve ?? oldWidget.reverseCurve;
        if (!Equals(_curve, oldCurve))
        {
            _heightFactor.curve = _curve;
        }
        if (!Equals(_reverseCurve, oldReverseCurve))
        {
            _heightFactor.reverseCurve = _reverseCurve;
        }
        if (!Equals(_duration, oldDuration))
        {
            _animationController.duration = _duration;
        }
        if (!Equals(widget.controller, oldWidget.controller))
        {
            oldWidget.controller.removeListener(_toggleExpansion);
            widget.controller.addListener(_toggleExpansion);
            if (oldWidget.controller.isExpanded != widget.controller.isExpanded)
            {
                _toggleExpansion();
            }
        }
    }

    public override void dispose()
    {
        widget.controller.removeListener(_toggleExpansion);
        _animationController.dispose();
        _heightFactor.dispose();
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

    internal virtual void _toggleExpansion()
    {
        setState(() =>
        {
            if (widget.controller.isExpanded)
            {
                _animationController.forward();
            }
            else
            {
                DartRuntimePrimitives.Ignore(
                    _animationController
                        .reverse()
                        .then(
                            (value) =>
                            {
                                if (!mounted)
                                {
                                    return default!;
                                }
                                setState(() => { });
                                throw new InvalidOperationException(
                                    "Callback completed without returning a value."
                                );
                            }
                        )
                );
            }
            PageStorage.maybeOf(context)?.writeState(context, widget.controller.isExpanded);
        });
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            !_animationController.isDismissed || !widget.controller.isExpanded
        );
        bool closed = !widget.controller.isExpanded && _animationController.isDismissed;
        bool shouldRemoveBody = closed && !widget.maintainState;
        Widget result = new Offstage(
            offstage: closed,
            child: new TickerMode(
                enabled: !closed,
                child: widget.bodyBuilder(context, _animationController)
            )
        );
        return new AnimatedBuilder(
            animation: _animationController.view,
            builder: (context, child) =>
            {
                Widget header = widget.headerBuilder(context, _animationController);
                Widget body = new ClipRect(
                    child: new Align(heightFactor: _heightFactor.value, child: child)
                );
                return widget.expansibleBuilder(context, header, body, _animationController);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            },
            child: shouldRemoveBody ? null : result
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
