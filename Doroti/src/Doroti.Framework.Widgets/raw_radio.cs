// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/raw_radio.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget RadioBuilder(BuildContext context, IToggleableState state);

public class RawRadio<T> : StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual WidgetStateProperty<MouseCursor> mouseCursor { get; private set; } = default!;
    public virtual bool toggleable { get; private set; } = default!;
    public virtual FocusNode focusNode { get; private set; } = default!;
    public virtual bool autofocus { get; private set; } = default!;
    public virtual Func<BuildContext, IToggleableState, Widget> builder { get; private set; } =
        default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual RadioGroupRegistry<T>? groupRegistry { get; private set; }

    public RawRadio(
        Key? key = null,
        T value = default!,
        WidgetStateProperty<MouseCursor> mouseCursor = default!,
        bool toggleable = default!,
        FocusNode focusNode = default!,
        bool autofocus = default!,
        RadioGroupRegistry<T>? groupRegistry = default!,
        bool enabled = default!,
        Func<BuildContext, IToggleableState, Widget> builder = default!
    )
        : base(key: key)
    {
        this.value = value;
        this.mouseCursor = mouseCursor;
        this.toggleable = toggleable;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.groupRegistry = groupRegistry;
        this.enabled = enabled;
        this.builder = builder;
        System.Diagnostics.Debug.Assert(!enabled || (groupRegistry is not null));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RawRadioState__raw_radio<T>());
}

public class _RawRadioState__raw_radio<T>
    : State<RawRadio<T>>,
        TickerProviderStateMixin<RawRadio<T>>,
        ToggleableStateMixin<RawRadio<T>>,
        RadioClient<T>
{
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;
    public virtual AnimationController _positionController { get; set; } = default!;
    public virtual CurvedAnimation _position { get; set; } = default!;
    public virtual AnimationController _reactionController { get; set; } = default!;
    public virtual CurvedAnimation _reaction { get; set; } = default!;
    public virtual CurvedAnimation _reactionHoverFade { get; set; } = default!;
    public virtual AnimationController _reactionHoverFadeController { get; set; } = default!;
    public virtual CurvedAnimation _reactionFocusFade { get; set; } = default!;
    public virtual AnimationController _reactionFocusFadeController { get; set; } = default!;
    public virtual Duration _reactionAnimationDuration { get; set; } =
        Duration.Create(milliseconds: 100L);
    private bool __late__actionMap_initialized;
    private DartMap<Type, dynamic> __late__actionMap = default!;
    public virtual DartMap<Type, dynamic> _actionMap
    {
        get
        {
            if (!__late__actionMap_initialized)
            {
                __late__actionMap = new DartMap<Type, dynamic>
                {
                    [typeof(ActivateIntent)] = new CallbackAction<ActivateIntent>(
                        onInvoke: (__arg0) =>
                        {
                            ((Action<Intent?>)_handleTap)(__arg0);
                            return default!;
                        }
                    ),
                };
                __late__actionMap_initialized = true;
            }
            return __late__actionMap;
        }
    }
    public virtual Offset? _downPosition { get; set; } = default;
    public virtual bool _focused { get; set; } = false;
    public virtual bool _hovering { get; set; } = false;
    public virtual RadioGroupRegistry<T>? _registry { get; set; } = default;

    public virtual FocusNode focusNode => widget.focusNode;
    public virtual bool enabled => isInteractive;
    public virtual T radioValue => widget.value;

    public override void initState()
    {
        registry = widget.groupRegistry;
        base.initState();
        _positionController = new AnimationController(
            duration: ToggleableLibrary._kToggleDuration,
            value: (value == false) ? 0.0 : 1.0,
            vsync: this
        );
        _position = new CurvedAnimation(
            parent: _positionController,
            curve: Curves.easeIn,
            reverseCurve: Curves.easeOut
        );
        _reactionController = new AnimationController(
            duration: _reactionAnimationDuration,
            vsync: this
        );
        _reaction = new CurvedAnimation(parent: _reactionController, curve: Curves.fastOutSlowIn);
        _reactionHoverFadeController = new AnimationController(
            duration: ToggleableLibrary._kReactionFadeDuration,
            value: (_hovering || _focused) ? 1.0 : 0.0,
            vsync: this
        );
        _reactionHoverFade = new CurvedAnimation(
            parent: _reactionHoverFadeController,
            curve: Curves.fastOutSlowIn
        );
        _reactionFocusFadeController = new AnimationController(
            duration: ToggleableLibrary._kReactionFadeDuration,
            value: (_hovering || _focused) ? 1.0 : 0.0,
            vsync: this
        );
        _reactionFocusFade = new CurvedAnimation(
            parent: _reactionFocusFadeController,
            curve: Curves.fastOutSlowIn
        );
    }

    internal virtual void _handleChanged(bool? selected)
    {
        DartRuntimePrimitives.Assert(() => registry is not null);
        if (!(selected ?? true))
        {
            return;
        }
        if (selected ?? false)
        {
            registry!.onChanged(widget.value);
        }
        else
        {
            registry!.onChanged(default);
        }
    }

    public override void didUpdateWidget(RawRadio<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        registry = widget.groupRegistry;
        animateToValue();
    }

    public override void dispose()
    {
        _positionController.dispose();
        _position.dispose();
        _reactionController.dispose();
        _reaction.dispose();
        _reactionHoverFadeController.dispose();
        _reactionHoverFade.dispose();
        _reactionFocusFadeController.dispose();
        _reactionFocusFade.dispose();
        base.dispose();
        registry = null;
    }

    public virtual Action<bool?>? onChanged => (registry is not null) ? _handleChanged : null;
    public virtual bool tristate => widget.toggleable;
    public virtual bool? value =>
        DartRuntimePrimitives.ConvertValue<bool>(
            EqualityComparer<T>.Default.Equals(
                widget.value,
                DartRuntimePrimitives.NullAware(registry, __target => __target.groupValue)
            )
        );
    public virtual bool isInteractive => widget.enabled;

    public override Widget build(BuildContext context)
    {
        bool? accessibilitySelected = default!;
        string? semanticsHint = default!;
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.windows:
            {
                accessibilitySelected = null;
                semanticsHint = null;
                break;
            }
            case TargetPlatform.iOS:
            case TargetPlatform.macOS:
            {
                accessibilitySelected = value;
                if (!(value ?? false))
                {
                    WidgetsLocalizations localizations = WidgetsLocalizations.of(context);
                    semanticsHint = localizations.radioButtonUnselectedLabel;
                }
                break;
            }
        }
        return new Semantics(
            inMutuallyExclusiveGroup: true,
            @checked: value,
            selected: accessibilitySelected,
            hint: semanticsHint,
            child: buildToggleableWithChild(
                focusNode: focusNode,
                autofocus: widget.autofocus,
                mouseCursor: widget.mouseCursor,
                child: widget.builder(context, this)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        var result = (
            (Func<_WidgetTicker__ticker_provider>)(
                () =>
                {
                    var __cascade = new _WidgetTicker__ticker_provider(
                        onTick,
                        this,
                        debugLabel: Foundation.ConstantsLibrary.kDebugMode
                            ? $"created by {DiagnosticsLibrary.describeIdentity(this)}"
                            : null
                    );
                    __cascade.muted = !values.enabled;
                    __cascade.forceFrames = values.forceFrames;
                    return __cascade;
                }
            )
        )();
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

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<HashSet<Scheduler.Ticker>>(
                "tickers",
                _tickers,
                description: (_tickers is not null)
                    ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}"
                    : null,
                defaultValue: default
            )
        );
    }

    public virtual AnimationController positionController => _positionController;
    public virtual CurvedAnimation position => _position;
    public virtual AnimationController reactionController => _reactionController;
    public virtual CurvedAnimation reaction => _reaction;
    public virtual CurvedAnimation reactionHoverFade => _reactionHoverFade;
    public virtual CurvedAnimation reactionFocusFade => _reactionFocusFade;
    public virtual Duration? reactionAnimationDuration => _reactionAnimationDuration;

    public virtual void animateToValue()
    {
        if (tristate)
        {
            if (value is null)
            {
                _positionController.value = 0.0;
            }
            if (value ?? true)
            {
                _positionController.forward();
            }
            else
            {
                _positionController.reverse();
            }
        }
        else
        {
            if (value ?? false)
            {
                _positionController.forward();
            }
            else
            {
                _positionController.reverse();
            }
        }
    }

    public virtual Offset? downPosition => _downPosition;

    public virtual void _handleTapDown(TapDownDetails details)
    {
        if (isInteractive)
        {
            setState(() =>
            {
                _downPosition = details.localPosition;
            });
            _reactionController.forward();
        }
    }

    public virtual void _handleTap(Intent? __unused0 = null)
    {
        if (!isInteractive)
        {
            return;
        }
        switch (value)
        {
            case false:
            {
                onChanged!(true);
                break;
            }
            case true:
            {
                onChanged!(tristate ? null : false);
                break;
            }
            case null:
            {
                onChanged!(false);
                break;
            }
        }
        context.findRenderObject()!.sendSemanticsEvent(new TapSemanticEvent());
    }

    public virtual void _handleTapEnd(TapUpDetails? __unused0 = null)
    {
        if (_downPosition is not null)
        {
            setState(() =>
            {
                _downPosition = null;
            });
        }
        _reactionController.reverse();
    }

    public virtual void _handleFocusHighlightChanged(bool focused)
    {
        if (focused != _focused)
        {
            setState(() =>
            {
                _focused = focused;
            });
            if (focused)
            {
                _reactionFocusFadeController.forward();
            }
            else
            {
                _reactionFocusFadeController.reverse();
            }
        }
    }

    public virtual void _handleHoverChanged(bool hovering)
    {
        if (hovering != _hovering)
        {
            setState(() =>
            {
                _hovering = hovering;
            });
            if (hovering)
            {
                _reactionHoverFadeController.forward();
            }
            else
            {
                _reactionHoverFadeController.reverse();
            }
        }
    }

    public virtual HashSet<WidgetState> states => new HashSet<WidgetState>();

    public virtual Widget buildToggleable(
        FocusNode? focusNode = null,
        Action<bool>? onFocusChange = null,
        bool autofocus = false,
        WidgetStateProperty<MouseCursor>? mouseCursor = null,
        Size size = default!,
        CustomPainter painter = default!
    )
    {
        return buildToggleableWithChild(
            focusNode: focusNode,
            onFocusChange: onFocusChange,
            autofocus: autofocus,
            mouseCursor: mouseCursor,
            child: new CustomPaint(size: size, painter: painter)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Widget buildToggleable(
        FocusNode? focusNode = null,
        Action<bool>? onFocusChange = null,
        bool autofocus = false,
        WidgetStateProperty<MouseCursor>? mouseCursor = null,
        Size size = default!,
        ToggleablePainter painter = default!
    )
    {
        return buildToggleableWithChild(
            focusNode: focusNode,
            onFocusChange: onFocusChange,
            autofocus: autofocus,
            mouseCursor: mouseCursor,
            child: new SizedBox(width: size.width, height: size.height)
        );
    }

    public virtual Widget buildToggleableWithChild(
        FocusNode? focusNode = null,
        Action<bool>? onFocusChange = null,
        bool autofocus = false,
        WidgetStateProperty<MouseCursor>? mouseCursor = null,
        Widget child = default!
    )
    {
        return new FocusableActionDetector(
            actions: _actionMap,
            focusNode: focusNode,
            autofocus: autofocus,
            onFocusChange: onFocusChange,
            enabled: isInteractive,
            onShowFocusHighlight: _handleFocusHighlightChanged,
            onShowHoverHighlight: _handleHoverChanged,
            mouseCursor: mouseCursor?.resolve(states) ?? SystemMouseCursors.basic,
            child: new GestureDetector(
                excludeFromSemantics: !isInteractive,
                onTapDown: isInteractive ? _handleTapDown : null,
                onTap: isInteractive ? () => _handleTap(null) : null,
                onTapUp: isInteractive ? _handleTapEnd : null,
                onTapCancel: isInteractive ? () => _handleTapEnd(null) : null,
                child: new Semantics(enabled: isInteractive, child: child)
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RadioGroupRegistry<T>? registry
    {
        get => _registry;
        set
        {
            var newRegistry = value;
            if (!Equals(_registry, newRegistry))
            {
                _registry?.unregisterClient(this);
            }
            _registry = newRegistry;
            _registry?.registerClient(this);
        }
    }
}
