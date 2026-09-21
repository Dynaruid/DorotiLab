// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/sliding_segmented_control.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class Sliding_segmented_controlLibrary
{
    internal static EdgeInsetsGeometry _kHorizontalItemPadding = EdgeInsets.CreateSymmetric(
        vertical: 2,
        horizontal: 3
    );
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Radius _kCornerRadius = Radius.circular(9);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Radius _kThumbRadius = Radius.circular(7);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static EdgeInsets _kThumbInsets = EdgeInsets.CreateSymmetric(horizontal: 1);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kMinSegmentedControlHeight = 28.0;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Color _kSeparatorColor = new Color(1301188243L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static CupertinoDynamicColor _kThumbColor = new CupertinoDynamicColor(
        color: new Color(4294967295L),
        darkColor: new Color(4284703590L)
    );
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static EdgeInsets _kSeparatorInset = EdgeInsets.CreateSymmetric(vertical: 5);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kSeparatorWidth = 1;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Radius _kSeparatorRadius = Radius.circular(_kSeparatorWidth / 2L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kMinThumbScale = 0.95;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kMaxThumbScaleForMomentary = 1.05;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kSegmentMinPadding = 10;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kTouchYDistanceThreshold = 50.0 * 50.0;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kContentPressedMinOpacity = 0.2;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static double _kFontSize = 13.0;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static FontWeight _kFontWeight = FontWeight.w500;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static FontWeight _kHighlightedFontWeight = FontWeight.w600;
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Color _kDisabledContentColor = Color.fromARGB(115L, 122L, 122L, 122L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Physics.SpringSimulation _kThumbSpringAnimationSimulation =
        new Physics.SpringSimulation(
            new Physics.SpringDescription(mass: 1, stiffness: 503.551, damping: 44.8799),
            0,
            1,
            0
        );
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Duration _kSpringAnimationDuration = Duration.Create(milliseconds: 412L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Duration _kOpacityAnimationDuration = Duration.Create(milliseconds: 470L);
}

public static partial class Sliding_segmented_controlLibrary
{
    internal static Duration _kHighlightAnimationDuration = Duration.Create(milliseconds: 200L);
}

public class _Segment__sliding_segmented_control<T> : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual bool pressed { get; private set; } = default!;
    public virtual bool highlighted { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;
    public virtual _SegmentLocation__sliding_segmented_control segmentLocation
    {
        get;
        private set;
    } = default!;
    public virtual bool isMomentary { get; private set; } = default!;
    public virtual bool isDragging { get; private set; } = default!;

    internal _Segment__sliding_segmented_control(
        ValueKey<T> key,
        Widget child,
        bool pressed,
        bool highlighted,
        bool isDragging,
        bool enabled,
        _SegmentLocation__sliding_segmented_control segmentLocation,
        bool isMomentary
    )
        : base(key: key)
    {
        this.child = child;
        this.pressed = pressed;
        this.highlighted = highlighted;
        this.isDragging = isDragging;
        this.enabled = enabled;
        this.segmentLocation = segmentLocation;
        this.isMomentary = isMomentary;
    }

    public virtual bool shouldFadeoutContent =>
        DartRuntimePrimitives.ConvertValue<bool>(
            pressed && !highlighted && enabled && !isMomentary
        );
    public virtual bool shouldScaleContent =>
        DartRuntimePrimitives.ConvertValue<bool>(
            pressed && enabled && ((highlighted && isDragging) || isMomentary)
        );
    public virtual bool shouldHighlightContent =>
        DartRuntimePrimitives.ConvertValue<bool>(highlighted && !isMomentary);

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _SegmentState__sliding_segmented_control<T>()
        );
}

public class _SegmentState__sliding_segmented_control<T>
    : State<_Segment__sliding_segmented_control<T>>,
        TickerProviderStateMixin<_Segment__sliding_segmented_control<T>>
{
    public virtual AnimationController highlightPressScaleController { get; private set; } =
        default!;
    public virtual Animation<double> highlightPressScaleAnimation { get; set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        highlightPressScaleController = new AnimationController(
            duration: Sliding_segmented_controlLibrary._kOpacityAnimationDuration,
            value: widget.shouldScaleContent ? 1 : 0,
            vsync: this
        );
        highlightPressScaleAnimation = highlightPressScaleController.drive(
            new Tween<double>(begin: 1.0, end: Sliding_segmented_controlLibrary._kMinThumbScale)
        );
    }

    public override void didUpdateWidget(_Segment__sliding_segmented_control<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => Equals(oldWidget.key, widget.key));
        if (oldWidget.shouldScaleContent != widget.shouldScaleContent)
        {
            Animatable<double> scaleAnimation =
                (widget.isMomentary && widget.shouldScaleContent)
                    ? new global::Doroti.Framework.Animation.TweenSequence<double>(
                        new List<global::Doroti.Framework.Animation.TweenSequenceItem<double>>
                        {
                            new global::Doroti.Framework.Animation.TweenSequenceItem<double>(
                                tween: new global::Doroti.Framework.Animation.Tween<double>(
                                    begin: highlightPressScaleAnimation.value,
                                    end: Sliding_segmented_controlLibrary._kMaxThumbScaleForMomentary
                                ),
                                weight: 50
                            ),
                            new global::Doroti.Framework.Animation.TweenSequenceItem<double>(
                                tween: new global::Doroti.Framework.Animation.Tween<double>(
                                    begin: Sliding_segmented_controlLibrary._kMaxThumbScaleForMomentary,
                                    end: 1.0
                                ),
                                weight: 50
                            ),
                        }
                    )
                    : new global::Doroti.Framework.Animation.Tween<double>(
                        begin: highlightPressScaleAnimation.value,
                        end: widget.shouldScaleContent
                            ? Sliding_segmented_controlLibrary._kMinThumbScale
                            : 1.0
                    );
            highlightPressScaleAnimation = highlightPressScaleController.drive(scaleAnimation);
            highlightPressScaleController.animateWith(
                Sliding_segmented_controlLibrary._kThumbSpringAnimationSimulation
            );
        }
    }

    public override void dispose()
    {
        highlightPressScaleController.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        Alignment scaleAlignment = widget.segmentLocation switch
        {
            _SegmentLocation__sliding_segmented_control.leftmost => Alignment.centerLeft,
            _SegmentLocation__sliding_segmented_control.rightmost => Alignment.centerRight,
            _SegmentLocation__sliding_segmented_control.inbetween => Alignment.center,
            _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
        };
        return new MetaData(
            behavior: HitTestBehavior.opaque,
            child: new IndexedStack(
                alignment: Alignment.center,
                children: new List<Widget>
                {
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        new AnimatedOpacity(
                            opacity: widget.shouldFadeoutContent
                                ? Sliding_segmented_controlLibrary._kContentPressedMinOpacity
                                : 1,
                            duration: Sliding_segmented_controlLibrary._kOpacityAnimationDuration,
                            curve: Curves.ease,
                            child: new AnimatedDefaultTextStyle(
                                style: DefaultTextStyle
                                    .of(context)
                                    .style.merge(
                                        new TextStyle(
                                            fontWeight: widget.shouldHighlightContent
                                                ? Sliding_segmented_controlLibrary._kHighlightedFontWeight
                                                : Sliding_segmented_controlLibrary._kFontWeight,
                                            fontSize: Sliding_segmented_controlLibrary._kFontSize,
                                            color: widget.enabled
                                                ? null
                                                : Sliding_segmented_controlLibrary._kDisabledContentColor
                                        )
                                    ),
                                duration: Sliding_segmented_controlLibrary._kHighlightAnimationDuration,
                                curve: Curves.ease,
                                child: new ScaleTransition(
                                    alignment: scaleAlignment,
                                    scale: highlightPressScaleAnimation,
                                    child: widget.child
                                )
                            )
                        )
                    ),
                    DartRuntimePrimitives.ConvertValue<Widget>(
                        DefaultTextStyle.merge(
                            style: new TextStyle(
                                fontWeight: Sliding_segmented_controlLibrary._kHighlightedFontWeight,
                                fontSize: Sliding_segmented_controlLibrary._kFontSize
                            ),
                            child: widget.child
                        )
                    ),
                }
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
}

public class _SegmentSeparator__sliding_segmented_control : StatefulWidget
{
    public virtual bool highlighted { get; private set; } = default!;

    internal _SegmentSeparator__sliding_segmented_control(ValueKey<long> key, bool highlighted)
        : base(key: key)
    {
        this.highlighted = highlighted;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _SegmentSeparatorState__sliding_segmented_control()
        );
}

public class _SegmentSeparatorState__sliding_segmented_control
    : State<_SegmentSeparator__sliding_segmented_control>,
        TickerProviderStateMixin<_SegmentSeparator__sliding_segmented_control>
{
    public virtual AnimationController separatorOpacityController { get; private set; } = default!;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        separatorOpacityController = new AnimationController(
            duration: Sliding_segmented_controlLibrary._kSpringAnimationDuration,
            value: widget.highlighted ? 0 : 1,
            vsync: this
        );
    }

    public override void didUpdateWidget(_SegmentSeparator__sliding_segmented_control oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        DartRuntimePrimitives.Assert(() => Equals(oldWidget.key, widget.key));
        if (oldWidget.highlighted != widget.highlighted)
        {
            separatorOpacityController.animateTo(
                widget.highlighted ? 0 : 1,
                duration: Sliding_segmented_controlLibrary._kSpringAnimationDuration,
                curve: Curves.ease
            );
        }
    }

    public override void dispose()
    {
        separatorOpacityController.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new AnimatedBuilder(
            animation: separatorOpacityController,
            child: new SizedBox(width: Sliding_segmented_controlLibrary._kSeparatorWidth),
            builder: (context, child) =>
            {
                return new Padding(
                    padding: Sliding_segmented_controlLibrary._kSeparatorInset,
                    child: new DecoratedBox(
                        decoration: new BoxDecoration(
                            color: Sliding_segmented_controlLibrary._kSeparatorColor.withOpacity(
                                Sliding_segmented_controlLibrary._kSeparatorColor.opacity
                                    * separatorOpacityController.value
                            ),
                            borderRadius: BorderRadius.CreateAll(
                                Sliding_segmented_controlLibrary._kSeparatorRadius
                            )
                        ),
                        child: child
                    )
                );
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
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
}

public class CupertinoSlidingSegmentedControl<T> : StatefulWidget
    where T : notnull
{
    public virtual DartMap<T, Widget> children { get; private set; } = default!;
    public virtual HashSet<T> disabledChildren { get; private set; } = default!;
    public virtual T? groupValue { get; private set; }
    public virtual Action<T?> onValueChanged { get; private set; } = default!;
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual bool proportionalWidth { get; private set; } = default!;
    public virtual Color thumbColor { get; private set; } = default!;
    public virtual EdgeInsetsGeometry padding { get; private set; } = default!;
    public virtual bool isMomentary { get; private set; } = default!;

    public CupertinoSlidingSegmentedControl(
        Key? key = null,
        DartMap<T, Widget> children = default!,
        Action<T?> onValueChanged = default!,
        HashSet<T> disabledChildren = default!,
        T? groupValue = default,
        Color thumbColor = default!,
        EdgeInsetsGeometry padding = default!,
        Color backgroundColor = default!,
        bool proportionalWidth = false,
        bool isMomentary = false
    )
        : base(key: key)
    {
        HashSet<T> __disabledChildren = disabledChildren ?? new HashSet<T>();
        Color __thumbColor = thumbColor ?? Sliding_segmented_controlLibrary._kThumbColor;
        EdgeInsetsGeometry __padding =
            padding ?? Sliding_segmented_controlLibrary._kHorizontalItemPadding;
        Color __backgroundColor = backgroundColor ?? CupertinoColors.tertiarySystemFill;
        this.children = children;
        this.onValueChanged = onValueChanged;
        this.disabledChildren = __disabledChildren;
        this.groupValue = groupValue;
        this.thumbColor = __thumbColor;
        this.padding = __padding;
        this.backgroundColor = __backgroundColor;
        this.proportionalWidth = proportionalWidth;
        this.isMomentary = isMomentary;
        System.Diagnostics.Debug.Assert(checked(children.Count) >= 2L);
        System.Diagnostics.Debug.Assert((groupValue is null) || children.Keys.contains(groupValue));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _SegmentedControlState__sliding_segmented_control<T>()
        );
}

public class _SlidingSegmentButton__sliding_segmented_control<T> : StatefulWidget
{
    public virtual T value { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;
    public virtual bool enabled { get; private set; } = default!;

    internal _SlidingSegmentButton__sliding_segmented_control(
        Key? key = null,
        T value = default!,
        Widget child = default!,
        bool enabled = default!
    )
        : base(key: key)
    {
        this.value = value;
        this.child = child;
        this.enabled = enabled;
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(
            new _SlidingSegmentButtonState__sliding_segmented_control<T>()
        );
}

internal class _SlidingSegmentButtonState__sliding_segmented_control<T>
    : State<_SlidingSegmentButton__sliding_segmented_control<T>>,
        RadioClient<T>
{
    internal virtual FocusNode _focusNode { get; private set; } = default!;
    public virtual RadioGroupRegistry<T>? _registry { get; set; } = default;

    public override void initState()
    {
        base.initState();
        _focusNode = new FocusNode(
            debugLabel: $"CupertinoSlidingSegmentedControl<{typeof(T)}>[{widget.value}]"
        );
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        registry = widget.enabled ? RadioGroup.maybeOf<T>(context) : null;
    }

    public override void didUpdateWidget(
        _SlidingSegmentButton__sliding_segmented_control<T> oldWidget
    )
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.enabled != widget.enabled)
        {
            registry = widget.enabled ? RadioGroup.maybeOf<T>(context) : null;
        }
    }

    public override void dispose()
    {
        registry = null;
        _focusNode.dispose();
        base.dispose();
    }

    public virtual bool enabled => widget.enabled;
    public virtual T radioValue => widget.value;
    public virtual FocusNode focusNode => _focusNode;
    public virtual bool tristate => false;

    public virtual void requestFocus()
    {
        if (widget.enabled)
        {
            _focusNode.requestFocus();
        }
    }

    public override Widget build(BuildContext context)
    {
        return new Focus(
            focusNode: _focusNode,
            canRequestFocus: widget.enabled,
            onKeyEvent: (node, @event) => KeyEventResult.ignored,
            child: widget.child
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

public class _SegmentedControlState__sliding_segmented_control<T>
    : State<CupertinoSlidingSegmentedControl<T>>,
        TickerProviderStateMixin<CupertinoSlidingSegmentedControl<T>>
    where T : notnull
{
    private bool __late_thumbController_initialized;
    private AnimationController __late_thumbController = default!;
    public virtual AnimationController thumbController
    {
        get
        {
            if (!__late_thumbController_initialized)
            {
                __late_thumbController = new AnimationController(
                    duration: Sliding_segmented_controlLibrary._kSpringAnimationDuration,
                    value: 0,
                    vsync: this
                );
                __late_thumbController_initialized = true;
            }
            return __late_thumbController;
        }
    }
    public virtual Animatable<Rect?>? thumbAnimatable { get; set; } = default;
    private bool __late_thumbScaleController_initialized;
    private AnimationController __late_thumbScaleController = default!;
    public virtual AnimationController thumbScaleController
    {
        get
        {
            if (!__late_thumbScaleController_initialized)
            {
                __late_thumbScaleController = new AnimationController(
                    duration: Sliding_segmented_controlLibrary._kSpringAnimationDuration,
                    value: 0,
                    vsync: this
                );
                __late_thumbScaleController_initialized = true;
            }
            return __late_thumbScaleController;
        }
    }
    private bool __late_thumbScaleAnimation_initialized;
    private Animation<double> __late_thumbScaleAnimation = default!;
    public virtual Animation<double> thumbScaleAnimation
    {
        get
        {
            if (!__late_thumbScaleAnimation_initialized)
            {
                __late_thumbScaleAnimation = thumbScaleController.drive(
                    new Tween<double>(
                        begin: 1,
                        end: Sliding_segmented_controlLibrary._kMinThumbScale
                    )
                );
                __late_thumbScaleAnimation_initialized = true;
            }
            return __late_thumbScaleAnimation;
        }
        set
        {
            __late_thumbScaleAnimation = value;
            __late_thumbScaleAnimation_initialized = true;
        }
    }
    public virtual Gestures.TapGestureRecognizer tap { get; private set; } =
        new Gestures.TapGestureRecognizer();
    public virtual Gestures.HorizontalDragGestureRecognizer drag { get; private set; } =
        new Gestures.HorizontalDragGestureRecognizer();
    public virtual Gestures.LongPressGestureRecognizer longPress { get; private set; } =
        new Gestures.LongPressGestureRecognizer();
    public virtual GlobalKey<IState> segmentedControlRenderWidgetKey { get; private set; } =
        GlobalKey<IState>.Create();
    internal virtual DartMap<
        T,
        GlobalKey<_SlidingSegmentButtonState__sliding_segmented_control<T>>
    > _segmentKeys { get; private set; } =
        new DartMap<T, GlobalKey<_SlidingSegmentButtonState__sliding_segmented_control<T>>>();
    internal virtual bool? _startedOnSelectedSegment { get; set; } = default;
    internal virtual bool _startedOnDisabledSegment { get; set; } = false;
    public virtual T? highlighted { get; set; } = default;
    public virtual T? pressed { get; set; } = default;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        var teamLocal = new Gestures.GestureArenaTeam();
        longPress.team = teamLocal;
        drag.team = teamLocal;
        teamLocal.captain = DartRuntimePrimitives.ConvertValue<Gestures.GestureArenaMember>(drag);
        DartRuntimePrimitives.Ignore(
            (
                (Func<Gestures.HorizontalDragGestureRecognizer>)(
                    () =>
                    {
                        var __cascade = drag;
                        __cascade.onDown = onDown;
                        __cascade.onUpdate = onUpdate;
                        __cascade.onEnd = onEnd;
                        __cascade.onCancel = onCancel;
                        return __cascade;
                    }
                )
            )()
        );
        tap.onTapUp = onTapUp;
        longPress.onLongPress = () => { };
        highlighted = widget.groupValue;
    }

    public override void didUpdateWidget(CupertinoSlidingSegmentedControl<T> oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!isThumbDragging && !EqualityComparer<T>.Default.Equals(highlighted, widget.groupValue))
        {
            thumbController.animateWith(
                Sliding_segmented_controlLibrary._kThumbSpringAnimationSimulation
            );
            thumbAnimatable = null;
            highlighted = widget.groupValue;
        }
    }

    public override void dispose()
    {
        thumbScaleController.dispose();
        thumbController.dispose();
        drag.dispose();
        tap.dispose();
        longPress.dispose();
        DartRuntimePrimitives.Assert(() =>
        {
            if (_tickers is not null)
            {
                foreach (Scheduler.Ticker ticker in _tickers!)
                {
                    if (ticker.isActive)
                    {
                        throw DartRuntimePrimitives.AsException(
                            new FlutterError(
                                new List<DiagnosticsNode>
                                {
                                    new ErrorSummary($"{this} was disposed with an active Ticker."),
                                    new ErrorDescription(
                                        $"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time "
                                            + "dispose() was called on the mixin, that Ticker was still active. All Tickers must "
                                            + "be disposed before calling super.dispose()."
                                    ),
                                    new ErrorHint(
                                        "Tickers used by AnimationControllers "
                                            + "should be disposed by calling dispose() on the AnimationController itself. "
                                            + "Otherwise, the ticker will leak."
                                    ),
                                    ticker.describeForError("The offending ticker was"),
                                }
                            )
                        );
                    }
                }
            }
            return true;
        });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    public virtual bool isThumbDragging =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (_startedOnSelectedSegment ?? false) && !_startedOnDisabledSegment
        );

    public virtual T segmentForXPosition(double dx)
    {
        BuildContext currentContextLocal = segmentedControlRenderWidgetKey.currentContext!;
        var renderBox = (
            (_RenderSegmentedControl__sliding_segmented_control<T>?)
                currentContextLocal.findRenderObject()!
        )!;
        long numOfChildren = checked(widget.children.Count);
        DartRuntimePrimitives.Assert(() => renderBox.hasSize);
        DartRuntimePrimitives.Assert(() => numOfChildren >= 2L);
        long segmentIndex = renderBox.getClosestSegmentIndex(dx);
        switch (Directionality.of(context))
        {
            case TextDirection.ltr:
            {
                break;
            }
            case TextDirection.rtl:
            {
                segmentIndex = numOfChildren - 1L - segmentIndex;
                break;
            }
        }
        return widget.children.Keys.elementAt(segmentIndex);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _hasDraggedTooFar(Gestures.DragUpdateDetails details)
    {
        var renderBox = ((RenderBox?)context.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => renderBox.hasSize);
        Size sizeLocal = renderBox.size;
        Offset offCenter =
            details.localPosition - new Offset(sizeLocal.width / 2L, sizeLocal.height / 2L);
        var l2 = (double)(
            Dart_mathLibrary.pow(Math.Max(0.0, offCenter.dx.abs() - (sizeLocal.width / 2L)), 2L)
            + Dart_mathLibrary.pow(Math.Max(0.0, offCenter.dy.abs() - (sizeLocal.height / 2L)), 2L)
        );
        return l2 > Sliding_segmented_controlLibrary._kTouchYDistanceThreshold;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _playThumbScaleAnimation(bool isExpanding)
    {
        thumbScaleAnimation = thumbScaleController.drive(
            new Tween<double>(
                begin: thumbScaleAnimation.value,
                end: isExpanding ? 1 : Sliding_segmented_controlLibrary._kMinThumbScale
            )
        );
        thumbScaleController.animateWith(
            Sliding_segmented_controlLibrary._kThumbSpringAnimationSimulation
        );
    }

    public virtual void onHighlightChangedByGesture(T newValue)
    {
        if (EqualityComparer<T>.Default.Equals(highlighted, newValue))
        {
            return;
        }
        setState(() =>
        {
            highlighted = newValue;
        });
        thumbController.animateWith(
            Sliding_segmented_controlLibrary._kThumbSpringAnimationSimulation
        );
        thumbAnimatable = null;
    }

    public virtual void onPressedChangedByGesture(T? newValue)
    {
        if (!EqualityComparer<T>.Default.Equals(pressed, newValue))
        {
            setState(() =>
            {
                pressed = newValue;
            });
        }
    }

    public virtual void onTapUp(Gestures.TapUpDetails details)
    {
        if (isThumbDragging)
        {
            return;
        }
        T segment = segmentForXPosition(details.localPosition.dx);
        onPressedChangedByGesture(default);
        if (!widget.disabledChildren.Contains(segment))
        {
            DartRuntimePrimitives
                .NullAware(
                    _segmentKeys.GetValueOrDefault(segment),
                    __target => __target.currentState
                )
                ?.requestFocus();
            if (!EqualityComparer<T>.Default.Equals(segment, widget.groupValue))
            {
                widget.onValueChanged(segment);
            }
        }
    }

    public virtual void onDown(Gestures.DragDownDetails details)
    {
        T touchDownSegment = segmentForXPosition(details.localPosition.dx);
        _startedOnSelectedSegment = EqualityComparer<T>.Default.Equals(
            touchDownSegment,
            highlighted
        );
        _startedOnDisabledSegment = widget.disabledChildren.Contains(touchDownSegment);
        if (widget.disabledChildren.Contains(touchDownSegment))
        {
            return;
        }
        onPressedChangedByGesture(touchDownSegment);
        if (isThumbDragging)
        {
            _playThumbScaleAnimation(isExpanding: false);
        }
    }

    public virtual void onUpdate(Gestures.DragUpdateDetails details)
    {
        if (_startedOnDisabledSegment)
        {
            return;
        }
        T touchDownSegment = segmentForXPosition(details.localPosition.dx);
        if (widget.disabledChildren.Contains(touchDownSegment))
        {
            return;
        }
        if (isThumbDragging)
        {
            onPressedChangedByGesture(touchDownSegment);
            onHighlightChangedByGesture(touchDownSegment);
        }
        else
        {
            T? segment = _hasDraggedTooFar(details)
                ? default(T)
                : segmentForXPosition(details.localPosition.dx);
            onPressedChangedByGesture(segment);
        }
    }

    public virtual void onEnd(Gestures.DragEndDetails details)
    {
        T? pressedLocal = pressed;
        if (isThumbDragging)
        {
            _playThumbScaleAnimation(isExpanding: true);
            if (!EqualityComparer<T>.Default.Equals(highlighted, widget.groupValue))
            {
                DartRuntimePrimitives
                    .NullAware(
                        _segmentKeys.GetValueOrDefault(highlighted),
                        __target => __target.currentState
                    )
                    ?.requestFocus();
                widget.onValueChanged(highlighted);
            }
        }
        else
        {
            if (pressedLocal is not null)
            {
                onHighlightChangedByGesture(pressedLocal);
                DartRuntimePrimitives.Assert(() =>
                    EqualityComparer<T>.Default.Equals(pressedLocal, highlighted)
                );
                if (!EqualityComparer<T>.Default.Equals(highlighted, widget.groupValue))
                {
                    DartRuntimePrimitives
                        .NullAware(
                            _segmentKeys.GetValueOrDefault(highlighted),
                            __target => __target.currentState
                        )
                        ?.requestFocus();
                    widget.onValueChanged(highlighted);
                }
            }
        }
        onPressedChangedByGesture(default);
        _startedOnSelectedSegment = null;
    }

    public virtual void onCancel()
    {
        if (isThumbDragging)
        {
            _playThumbScaleAnimation(isExpanding: true);
        }
        onPressedChangedByGesture(default);
        _startedOnSelectedSegment = null;
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => checked(widget.children.Count) >= 2L);
        var childrenLocal = new List<Widget>();
        var isPreviousSegmentHighlighted = false;
        var index = 0L;
        long? highlightedIndexLocal = default!;
        foreach (MapEntry<T, Widget> entry in widget.children.entries)
        {
            var isHighlighted = EqualityComparer<T>.Default.Equals(highlighted, entry.key);
            if (isHighlighted)
            {
                highlightedIndexLocal = index;
            }
            if (index != 0L)
            {
                childrenLocal.Add(
                    new _SegmentSeparator__sliding_segmented_control(
                        key: new ValueKey<long>(index),
                        highlighted: isPreviousSegmentHighlighted || isHighlighted
                    )
                );
            }
            TextDirection textDirection = Directionality.of(context);
            _SegmentLocation__sliding_segmented_control segmentLocationLocal = textDirection switch
            {
                TextDirection.ltr when index == 0L =>
                    _SegmentLocation__sliding_segmented_control.leftmost,
                TextDirection.ltr when index == (checked(widget.children.Count) - 1L) =>
                    _SegmentLocation__sliding_segmented_control.rightmost,
                TextDirection.rtl when index == (checked(widget.children.Count) - 1L) =>
                    _SegmentLocation__sliding_segmented_control.leftmost,
                TextDirection.rtl when index == 0L =>
                    _SegmentLocation__sliding_segmented_control.rightmost,
                TextDirection.ltr => _SegmentLocation__sliding_segmented_control.inbetween,
                TextDirection.rtl => _SegmentLocation__sliding_segmented_control.inbetween,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            GlobalKey<_SlidingSegmentButtonState__sliding_segmented_control<T>> segmentKey =
                _segmentKeys.putIfAbsent(
                    entry.key,
                    () =>
                        GlobalKey<_SlidingSegmentButtonState__sliding_segmented_control<T>>.Create()
                );
            childrenLocal.Add(
                new _SlidingSegmentButton__sliding_segmented_control<T>(
                    key: segmentKey,
                    value: entry.key,
                    enabled: !widget.disabledChildren.Contains(entry.key),
                    child: new Widgets.Semantics(
                        button: true,
                        onTap: () =>
                        {
                            if (widget.disabledChildren.Contains(entry.key))
                            {
                                return;
                            }
                            DartRuntimePrimitives
                                .NullAware(
                                    _segmentKeys.GetValueOrDefault(entry.key),
                                    __target => __target.currentState
                                )
                                ?.requestFocus();
                            widget.onValueChanged(entry.key);
                        },
                        inMutuallyExclusiveGroup: true,
                        selected: EqualityComparer<T>.Default.Equals(widget.groupValue, entry.key),
                        child: new MouseRegion(
                            cursor: Foundation.ConstantsLibrary.kIsWeb
                                ? SystemMouseCursors.click
                                : MouseCursor.defer,
                            child: new _Segment__sliding_segmented_control<T>(
                                key: new ValueKey<T>(entry.key),
                                highlighted: isHighlighted,
                                pressed: EqualityComparer<T>.Default.Equals(pressed, entry.key),
                                isDragging: isThumbDragging,
                                enabled: !widget.disabledChildren.Contains(entry.key),
                                segmentLocation: segmentLocationLocal,
                                isMomentary: widget.isMomentary,
                                child: entry.value
                            )
                        )
                    )
                )
            );
            index += 1L;
            isPreviousSegmentHighlighted = isHighlighted;
        }
        DartRuntimePrimitives.Assert(() =>
            (highlightedIndexLocal is null) == (highlighted is null)
        );
        switch (Directionality.of(context))
        {
            case TextDirection.ltr:
            {
                break;
            }
            case TextDirection.rtl:
            {
                childrenLocal = Enumerable.Reverse(childrenLocal).ToList();
                if (highlightedIndexLocal is not null)
                {
                    long highlightedIndex__30239__value33090 = DartRuntimePrimitives.RequireValue(
                        highlightedIndexLocal
                    );
                    highlightedIndexLocal =
                        index
                        - 1L
                        - DartRuntimePrimitives.RequireValue(highlightedIndex__30239__value33090);
                }
                break;
            }
        }
        return new Actions(
            actions: new DartMap<Type, dynamic>
            {
                [typeof(VoidCallbackIntent)] = new VoidCallbackAction(),
            },
            child: new RadioGroup<T>(
                groupValue: widget.groupValue,
                onChanged: (value) =>
                {
                    if ((value is not null) && !widget.disabledChildren.Contains(value))
                    {
                        widget.onValueChanged(value);
                    }
                },
                child: new UnconstrainedBox(
                    constrainedAxis: Axis.horizontal,
                    child: new Container(
                        clipBehavior: Clip.antiAlias,
                        padding: widget.padding.resolve(Directionality.of(context)),
                        decoration: new ShapeDecoration(
                            shape: new RoundedSuperellipseBorder(
                                borderRadius: BorderRadius.CreateAll(
                                    Radius.circular(DialogLibrary._kCornerRadius)
                                )
                            ),
                            color: CupertinoDynamicColor.resolve(widget.backgroundColor, context)
                        ),
                        child: new AnimatedBuilder(
                            animation: thumbScaleAnimation,
                            builder: (context, child) =>
                            {
                                return new _SegmentedControlRenderWidget__sliding_segmented_control<T>(
                                    key: segmentedControlRenderWidgetKey,
                                    highlightedIndex: widget.isMomentary
                                        ? null
                                        : highlightedIndexLocal,
                                    thumbColor: CupertinoDynamicColor.resolve(
                                        widget.thumbColor,
                                        context
                                    ),
                                    thumbScale: thumbScaleAnimation.value,
                                    proportionalWidth: widget.proportionalWidth,
                                    state: this,
                                    children: childrenLocal
                                );
                                throw new InvalidOperationException(
                                    "Dart closure completed without a value."
                                );
                            }
                        )
                    )
                )
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
}

internal class _SegmentedControlRenderWidget__sliding_segmented_control<T>
    : MultiChildRenderObjectWidget
    where T : notnull
{
    public virtual long? highlightedIndex { get; private set; }
    public virtual Color thumbColor { get; private set; } = default!;
    public virtual double thumbScale { get; private set; } = default!;
    public virtual bool proportionalWidth { get; private set; } = default!;
    public virtual _SegmentedControlState__sliding_segmented_control<T> state
    {
        get;
        private set;
    } = default!;

    internal _SegmentedControlRenderWidget__sliding_segmented_control(
        Key? key = null,
        List<Widget> children = default!,
        long? highlightedIndex = default!,
        Color thumbColor = default!,
        double thumbScale = default!,
        bool proportionalWidth = default!,
        _SegmentedControlState__sliding_segmented_control<T> state = default!
    )
        : base(key: key, children: children ?? new List<Widget>())
    {
        this.highlightedIndex = highlightedIndex;
        this.thumbColor = thumbColor;
        this.thumbScale = thumbScale;
        this.proportionalWidth = proportionalWidth;
        this.state = state;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderSegmentedControl__sliding_segmented_control<T>(
            highlightedIndex: highlightedIndex,
            thumbColor: thumbColor,
            thumbScale: thumbScale,
            proportionalWidth: proportionalWidth,
            state: state
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderSegmentedControl__sliding_segmented_control<T>)renderObject;
        DartRuntimePrimitives.Assert(() => Equals(__renderObject.state, state));
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderSegmentedControl__sliding_segmented_control<T>>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.thumbColor = thumbColor;
                        __cascade.thumbScale = thumbScale;
                        __cascade.highlightedIndex = highlightedIndex;
                        __cascade.proportionalWidth = proportionalWidth;
                        return __cascade;
                    }
                )
            )()
        );
    }
}

internal class _SegmentedControlContainerBoxParentData__sliding_segmented_control
    : ContainerBoxParentData<RenderBox> { }

public enum _SegmentLocation__sliding_segmented_control
{
    leftmost,
    rightmost,
    inbetween,
}

public class _RenderSegmentedControl__sliding_segmented_control<T>
    : RenderBox,
        ContainerRenderObjectMixin<RenderBox, ContainerBoxParentData<RenderBox>>,
        RenderBoxContainerDefaultsMixin<RenderBox, ContainerBoxParentData<RenderBox>>
    where T : notnull
{
    public virtual _SegmentedControlState__sliding_segmented_control<T> state
    {
        get;
        private set;
    } = default!;
    public virtual Rect? currentThumbRect { get; set; } = default;
    internal virtual double _thumbScale { get; set; } = default!;
    internal virtual long? _highlightedIndex { get; set; } = default;
    internal virtual Color _thumbColor { get; set; } = default!;
    internal virtual bool _proportionalWidth { get; set; } = default!;
    public virtual Paint separatorPaint { get; private set; } = new Paint();
    public virtual long _childCount { get; set; } = 0L;
    public virtual RenderBox? _firstChild { get; set; } = default;
    public virtual RenderBox? _lastChild { get; set; } = default;

    internal _RenderSegmentedControl__sliding_segmented_control(
        long? highlightedIndex,
        Color thumbColor,
        double thumbScale,
        bool proportionalWidth,
        _SegmentedControlState__sliding_segmented_control<T> state
    )
    {
        this.state = state;
        _highlightedIndex = highlightedIndex;
        _thumbColor = thumbColor;
        _thumbScale = thumbScale;
        _proportionalWidth = proportionalWidth;
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.attach(owner);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
        state.thumbController.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        state.thumbController.removeListener(markNeedsPaint);
        base.detach();
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            child.detach();
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual double thumbScale
    {
        get => _thumbScale;
        set
        {
            var __value = value;
            if (_thumbScale == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _thumbScale = DartRuntimePrimitives.RequireValue(__value);
            if (state.highlighted is not null)
            {
                markNeedsPaint();
            }
        }
    }
    public virtual long? highlightedIndex
    {
        get => _highlightedIndex;
        set
        {
            var __value = value;
            if (_highlightedIndex == __value)
            {
                return;
            }
            _highlightedIndex = __value;
            markNeedsPaint();
        }
    }
    public virtual Color thumbColor
    {
        get => _thumbColor;
        set
        {
            var __value = value;
            if (Equals(_thumbColor, __value))
            {
                return;
            }
            _thumbColor = __value;
            markNeedsPaint();
        }
    }
    public virtual bool proportionalWidth
    {
        get => _proportionalWidth;
        set
        {
            var __value = value;
            if (_proportionalWidth == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _proportionalWidth = DartRuntimePrimitives.RequireValue(__value);
            markNeedsLayout();
        }
    }

    public override void handleEvent(
        Gestures.PointerEvent @event,
        Gestures.HitTestEntry<Gestures.HitTestTarget> entry
    )
    {
        DartRuntimePrimitives.Assert(() => debugHandleEvent(@event, entry));
        if ((@event is Gestures.PointerDownEvent) && !state.isThumbDragging)
        {
            Gestures.PointerDownEvent @event__as40275 = (Gestures.PointerDownEvent)@event;
            state.tap.addPointer(@event__as40275);
            state.longPress.addPointer(@event__as40275);
            state.drag.addPointer(@event__as40275);
        }
    }

    public virtual double separatorWidth =>
        DartRuntimePrimitives.ConvertValue<double>(
            Sliding_segmented_controlLibrary._kSeparatorInset.horizontal
                + Sliding_segmented_controlLibrary._kSeparatorWidth
        );
    public virtual double totalSeparatorWidth =>
        DartRuntimePrimitives.ConvertValue<double>(separatorWidth * checked(childCount / 2L));

    public virtual long getClosestSegmentIndex(double dx)
    {
        var index = 0L;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = (
                (_SegmentedControlContainerBoxParentData__sliding_segmented_control?)
                    child.parentData!
            )!;
            double clampX = Dart_uiLibrary.clampDouble(
                dx,
                childParentData.offset.dx,
                child.size.width + childParentData.offset.dx
            );
            if (dx <= clampX)
            {
                break;
            }
            index++;
            child = nonSeparatorChildAfter(child);
        }
        long segmentCount = checked(childCount / 2L) + 1L;
        return Math.Min(index, segmentCount - 1L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? nonSeparatorChildAfter(RenderBox child)
    {
        RenderBox? nextChild = childAfter(child);
        return (nextChild is null) ? null : childAfter(nextChild);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        long childCountLocal = checked(childCount / 2L) + 1L;
        RenderBox? child = firstChild;
        double maxMinChildWidth = 0;
        while (child is not null)
        {
            double childWidth = child.getMinIntrinsicWidth(height);
            maxMinChildWidth = Math.Max(maxMinChildWidth, childWidth);
            child = nonSeparatorChildAfter(child);
        }
        return (
                (maxMinChildWidth + (2L * Sliding_segmented_controlLibrary._kSegmentMinPadding))
                * childCountLocal
            ) + totalSeparatorWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        long childCountLocal = checked(childCount / 2L) + 1L;
        RenderBox? child = firstChild;
        double maxMaxChildWidth = 0;
        while (child is not null)
        {
            double childWidth = child.getMaxIntrinsicWidth(height);
            maxMaxChildWidth = Math.Max(maxMaxChildWidth, childWidth);
            child = nonSeparatorChildAfter(child);
        }
        return (
                (maxMaxChildWidth + (2L * Sliding_segmented_controlLibrary._kSegmentMinPadding))
                * childCountLocal
            ) + totalSeparatorWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        RenderBox? child = firstChild;
        double maxMinChildHeight = Segmented_controlLibrary._kMinSegmentedControlHeight;
        while (child is not null)
        {
            double childHeight = child.getMinIntrinsicHeight(width);
            maxMinChildHeight = Math.Max(maxMinChildHeight, childHeight);
            child = nonSeparatorChildAfter(child);
        }
        return maxMinChildHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        RenderBox? child = firstChild;
        double maxMaxChildHeight = Segmented_controlLibrary._kMinSegmentedControlHeight;
        while (child is not null)
        {
            double childHeight = child.getMaxIntrinsicHeight(width);
            maxMaxChildHeight = Math.Max(maxMaxChildHeight, childHeight);
            child = nonSeparatorChildAfter(child);
        }
        return maxMaxChildHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return defaultComputeDistanceToHighestActualBaseline(baseline);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void setupParentData(RenderObject child)
    {
        var __child = (RenderBox)child;
        if (
            __child.parentData
            is not _SegmentedControlContainerBoxParentData__sliding_segmented_control
        )
        {
            __child.parentData =
                new _SegmentedControlContainerBoxParentData__sliding_segmented_control();
        }
    }

    internal virtual double _getMaxChildWidth(BoxConstraints constraints)
    {
        long childCountLocal = checked(childCount / 2L) + 1L;
        double childWidth = (constraints.minWidth - totalSeparatorWidth) / childCountLocal;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            childWidth = Math.Max(
                childWidth,
                child.getMaxIntrinsicWidth(double.PositiveInfinity)
                    + (2L * Sliding_segmented_controlLibrary._kSegmentMinPadding)
            );
            child = nonSeparatorChildAfter(child);
        }
        return Math.Min(childWidth, (constraints.maxWidth - totalSeparatorWidth) / childCountLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getMaxChildHeight(BoxConstraints constraints, double childWidth)
    {
        double maxHeight = Segmented_controlLibrary._kMinSegmentedControlHeight;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            double boxHeight = child.getMaxIntrinsicHeight(childWidth);
            maxHeight = Math.Max(maxHeight, boxHeight);
            child = nonSeparatorChildAfter(child);
        }
        return maxHeight;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual List<double> _getChildWidths(BoxConstraints constraints)
    {
        if (!proportionalWidth)
        {
            double maxChildWidth = _getMaxChildWidth(constraints);
            long segmentCount = checked(childCount / 2L) + 1L;
            return new List<double>(Enumerable.Repeat(maxChildWidth, checked((int)segmentCount)));
        }
        var segmentWidths = new List<double>();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            double childWidth =
                child.getMaxIntrinsicWidth(double.PositiveInfinity)
                + (2L * Sliding_segmented_controlLibrary._kSegmentMinPadding);
            child = nonSeparatorChildAfter(child);
            segmentWidths.Add(childWidth);
        }
        double totalWidth = segmentWidths.sum();
        double allowedMaxWidth = constraints.maxWidth - totalSeparatorWidth;
        double allowedMinWidth = constraints.minWidth - totalSeparatorWidth;
        double scale =
            Dart_uiLibrary.clampDouble(totalWidth, allowedMinWidth, allowedMaxWidth) / totalWidth;
        if (scale != 1L)
        {
            for (var i = 0L; i < checked(segmentWidths.Count); i++)
            {
                segmentWidths[(int)i] = segmentWidths[(int)i] * scale;
            }
        }
        return segmentWidths;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeOverallSize(BoxConstraints constraints)
    {
        double maxChildHeight = _getMaxChildHeight(constraints, constraints.maxWidth);
        return constraints.constrain(
            new Size(_getChildWidths(constraints).sum() + totalSeparatorWidth, maxChildHeight)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        List<double> segmentWidths = _getChildWidths(constraints);
        double childHeight = _getMaxChildHeight(constraints, constraints.maxWidth);
        var index = 0L;
        BaselineOffset baselineOffset = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childConstraints = BoxConstraints.CreateTight(
                new Size(segmentWidths[(int)index], childHeight)
            );
            baselineOffset = baselineOffset.minOf(
                new BaselineOffset(child.getDryBaseline(childConstraints, baseline))
            );
            child = nonSeparatorChildAfter(child);
            index++;
        }
        return baselineOffset.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeOverallSize(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        BoxConstraints constraintsLocal = constraints;
        List<double> segmentWidths = _getChildWidths(constraintsLocal);
        double childHeight = _getMaxChildHeight(constraintsLocal, double.PositiveInfinity);
        var separatorConstraints = new BoxConstraints(
            minHeight: childHeight,
            maxHeight: childHeight
        );
        RenderBox? child = firstChild;
        var index = 0L;
        double start = 0;
        while (child is not null)
        {
            var childConstraints = BoxConstraints.CreateTight(
                new Size(segmentWidths[(int)checked(index / 2L)], childHeight)
            );
            child.layout(
                ((checked(index) & 1L) == 0L) ? childConstraints : separatorConstraints,
                parentUsesSize: true
            );
            var childParentData = (
                (_SegmentedControlContainerBoxParentData__sliding_segmented_control?)
                    child.parentData!
            )!;
            var childOffset = new Offset(start, 0);
            childParentData.offset = childOffset;
            start += child.size.width;
            DartRuntimePrimitives.Assert(
                () =>
                    ((checked(index) & 1L) == 0L)
                    || (
                        child.size.width
                        == (
                            Sliding_segmented_controlLibrary._kSeparatorWidth
                            + Sliding_segmented_controlLibrary._kSeparatorInset.horizontal
                        )
                    ),
                () =>
                    (object?)
                        $"{child.size.width} != {Sliding_segmented_controlLibrary._kSeparatorWidth + Sliding_segmented_controlLibrary._kSeparatorInset.horizontal}"
            );
            child = childAfter(child);
            index += 1L;
        }
        size = _computeOverallSize(constraintsLocal);
    }

    public virtual Rect? moveThumbRectInBound(Rect? thumbRect, List<RenderBox> children)
    {
        DartRuntimePrimitives.Assert(() => hasSize);
        DartRuntimePrimitives.Assert(() => checked(children.Count) >= 2L);
        if (thumbRect is null)
        {
            return null;
        }
        Offset firstChildOffset = (
            (_SegmentedControlContainerBoxParentData__sliding_segmented_control?)
                children.First().parentData!
        )!.offset;
        double leftMost = firstChildOffset.dx;
        double rightMost =
            (
                (_SegmentedControlContainerBoxParentData__sliding_segmented_control?)
                    children.Last().parentData!
            )!
                .offset
                .dx + children.Last().size.width;
        DartRuntimePrimitives.Assert(() => rightMost > leftMost);
        return Rect.fromLTRB(
            Math.Max(
                DartRuntimePrimitives.RequireValue(thumbRect).left,
                leftMost - Sliding_segmented_controlLibrary._kThumbInsets.left
            ),
            firstChildOffset.dy - Sliding_segmented_controlLibrary._kThumbInsets.top,
            Math.Min(
                DartRuntimePrimitives.RequireValue(thumbRect).right,
                rightMost + Sliding_segmented_controlLibrary._kThumbInsets.right
            ),
            firstChildOffset.dy
                + children.First().size.height
                + Sliding_segmented_controlLibrary._kThumbInsets.bottom
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        List<RenderBox> children = getChildrenAsList();
        for (var index = 1L; index < childCount; index += 2L)
        {
            _paintSeparator(context, offset, children[(int)index]);
        }
        long? highlightedChildIndex = highlightedIndex;
        if (highlightedChildIndex is not null)
        {
            long highlightedChildIndex__50131__value50234 = DartRuntimePrimitives.RequireValue(
                highlightedChildIndex
            );
            RenderBox selectedChild = children[
                (int)(
                    DartRuntimePrimitives.RequireValue(highlightedChildIndex__50131__value50234)
                    * 2L
                )
            ];
            var childParentData = (
                (_SegmentedControlContainerBoxParentData__sliding_segmented_control?)
                    selectedChild.parentData!
            )!;
            Rect newThumbRect = Sliding_segmented_controlLibrary._kThumbInsets.inflateRect(
                childParentData.offset & selectedChild.size
            );
            if (state.thumbController.isAnimating)
            {
                Animatable<Rect?>? thumbTween = state.thumbAnimatable;
                if (thumbTween is null)
                {
                    Rect startingRect =
                        moveThumbRectInBound(currentThumbRect, children) ?? newThumbRect;
                    state.thumbAnimatable = new RectTween(begin: startingRect, end: newThumbRect);
                }
                else
                {
                    if (!Equals(newThumbRect, thumbTween.transform(1)))
                    {
                        Rect startingRectLocal =
                            moveThumbRectInBound(currentThumbRect, children) ?? newThumbRect;
                        state.thumbAnimatable = new RectTween(
                            begin: startingRectLocal,
                            end: newThumbRect
                        ).chain(
                            new CurveTween(
                                curve: new Interval(
                                    DartRuntimePrimitives.RequireValue(state.thumbController.value),
                                    1
                                )
                            )
                        );
                    }
                }
            }
            else
            {
                state.thumbAnimatable = null;
            }
            Rect unscaledThumbRect =
                state.thumbAnimatable?.evaluate(state.thumbController) ?? newThumbRect;
            currentThumbRect = unscaledThumbRect;
            _SegmentLocation__sliding_segmented_control childLocation = default!;
            if (DartRuntimePrimitives.RequireValue(highlightedChildIndex__50131__value50234) == 0L)
            {
                childLocation = _SegmentLocation__sliding_segmented_control.leftmost;
            }
            else
            {
                if (
                    DartRuntimePrimitives.RequireValue(highlightedChildIndex__50131__value50234)
                    == checked(checked(children.Count) / 2L)
                )
                {
                    childLocation = _SegmentLocation__sliding_segmented_control.rightmost;
                }
                else
                {
                    childLocation = _SegmentLocation__sliding_segmented_control.inbetween;
                }
            }
            double delta = childLocation switch
            {
                _SegmentLocation__sliding_segmented_control.leftmost => unscaledThumbRect.width
                    - (unscaledThumbRect.width * thumbScale),
                _SegmentLocation__sliding_segmented_control.rightmost => (
                    unscaledThumbRect.width * thumbScale
                ) - unscaledThumbRect.width,
                _SegmentLocation__sliding_segmented_control.inbetween => 0,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
            var thumbRect = Rect.fromCenter(
                center: unscaledThumbRect.center - new Offset(delta / 2L, 0),
                width: unscaledThumbRect.width * thumbScale,
                height: unscaledThumbRect.height * thumbScale
            );
            _paintThumb(
                context,
                offset,
                DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(thumbRect))
            );
        }
        else
        {
            currentThumbRect = null;
        }
        for (var indexLocal = 0L; indexLocal < checked(children.Count); indexLocal += 2L)
        {
            _paintChild(context, offset, children[(int)indexLocal]);
        }
    }

    internal virtual void _paintSeparator(PaintingContext context, Offset offset, RenderBox child)
    {
        var childParentData = (
            (_SegmentedControlContainerBoxParentData__sliding_segmented_control?)child.parentData!
        )!;
        context.paintChild(child, offset + childParentData.offset);
    }

    internal virtual void _paintChild(PaintingContext context, Offset offset, RenderBox child)
    {
        var childParentData = (
            (_SegmentedControlContainerBoxParentData__sliding_segmented_control?)child.parentData!
        )!;
        context.paintChild(child, childParentData.offset + offset);
    }

    internal virtual void _paintThumb(PaintingContext context, Offset offset, Rect thumbRect)
    {
        var thumbShadow = new List<BoxShadow>
        {
            new BoxShadow(color: new Color(520093696L), offset: new Offset(0, 3), blurRadius: 8),
            new BoxShadow(color: new Color(167772160L), offset: new Offset(0, 3), blurRadius: 1),
        };
        var thumbShape = RSuperellipse.fromRectAndRadius(
            DartRuntimePrimitives.RequireValue(thumbRect).shift(offset),
            Sliding_segmented_controlLibrary._kThumbRadius
        );
        foreach (var shadow in thumbShadow)
        {
            context.canvas.drawRSuperellipse(thumbShape.shift(shadow.offset), shadow.toPaint());
        }
        context.canvas.drawRSuperellipse(
            thumbShape.inflate(0.5),
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = new Color(167772160L);
                        return __cascade;
                    }
                )
            )()
        );
        context.canvas.drawRSuperellipse(
            thumbShape,
            (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = thumbColor;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = (
                (_SegmentedControlContainerBoxParentData__sliding_segmented_control?)
                    child.parentData!
            )!;
            if ((childParentData.offset & child.size).contains(position))
            {
                return result.addWithPaintOffset(
                    offset: childParentData.offset,
                    position: position,
                    hitTest: (result, localOffset) =>
                    {
                        DartRuntimePrimitives.Assert(() =>
                            Equals(localOffset, position - childParentData.offset)
                        );
                        return child!.hitTest(result, position: localOffset);
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                );
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimatePreviousSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        while (childParentData.previousSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.previousSibling, child));
            child = childParentData.previousSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _debugUltimateNextSiblingOf(RenderBox child, RenderBox? equals = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        while (childParentData.nextSibling is not null)
        {
            DartRuntimePrimitives.Assert(() => !Equals(childParentData.nextSibling, child));
            child = childParentData.nextSibling!;
            childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        }
        return Equals(child, equals);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual long childCount => _childCount;

    public virtual bool debugValidateChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            if (child is not RenderBox)
            {
                throw DartRuntimePrimitives.AsException(
                    new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                $"A {GetType()} expected a child of type {typeof(RenderBox)} but received a "
                                    + $"child of type {DartRuntimePrimitives.RuntimeType(child)}."
                            ),
                            new ErrorDescription(
                                "RenderObjects expect specific types of children because they "
                                    + "coordinate with their children during layout and paint. For "
                                    + "example, a RenderSliver cannot be the child of a RenderBox because "
                                    + "a RenderSliver does not understand the RenderBox layout protocol."
                            ),
                            new ErrorSpacer(),
                            new DiagnosticsProperty<object?>(
                                $"The {GetType()} that expected a {typeof(RenderBox)} child was created by",
                                debugCreator,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                            new ErrorSpacer(),
                            new DiagnosticsProperty<object?>(
                                $"The {DartRuntimePrimitives.RuntimeType(child)} that did not match the expected child type "
                                    + "was created by",
                                child.debugCreator,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        }
                    )
                );
            }
            return true;
        });
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _insertIntoChildList(RenderBox child, RenderBox? after = null)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() => childParentData.nextSibling is null);
        DartRuntimePrimitives.Assert(() => childParentData.previousSibling is null);
        _childCount += 1L;
        DartRuntimePrimitives.Assert(() => _childCount > 0L);
        if (after is null)
        {
            childParentData.nextSibling = _firstChild;
            if (_firstChild is not null)
            {
                var firstChildParentData = (
                    (ContainerBoxParentData<RenderBox>?)_firstChild!.parentData!
                )!;
                firstChildParentData.previousSibling = child;
            }
            _firstChild = child;
            _lastChild ??= child;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => _firstChild is not null);
            DartRuntimePrimitives.Assert(() => _lastChild is not null);
            DartRuntimePrimitives.Assert(() =>
                _debugUltimatePreviousSiblingOf(after, equals: _firstChild)
            );
            DartRuntimePrimitives.Assert(() =>
                _debugUltimateNextSiblingOf(after, equals: _lastChild)
            );
            var afterParentData = ((ContainerBoxParentData<RenderBox>?)after.parentData!)!;
            if (afterParentData.nextSibling is null)
            {
                DartRuntimePrimitives.Assert(() => Equals(after, _lastChild));
                childParentData.previousSibling = after;
                afterParentData.nextSibling = child;
                _lastChild = child;
            }
            else
            {
                childParentData.nextSibling = afterParentData.nextSibling;
                childParentData.previousSibling = after;
                var childPreviousSiblingParentData = (
                    (ContainerBoxParentData<RenderBox>?)childParentData.previousSibling!.parentData!
                )!;
                var childNextSiblingParentData = (
                    (ContainerBoxParentData<RenderBox>?)childParentData.nextSibling!.parentData!
                )!;
                childPreviousSiblingParentData.nextSibling = child;
                childNextSiblingParentData.previousSibling = child;
                DartRuntimePrimitives.Assert(() => Equals(afterParentData.nextSibling, child));
            }
        }
    }

    public virtual void insert(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(
            () => !Equals(child, this),
            () => (object?)"A RenderObject cannot be inserted into itself."
        );
        DartRuntimePrimitives.Assert(
            () => !Equals(after, this),
            () =>
                (object?)
                    "A RenderObject cannot simultaneously be both the parent and the sibling of another RenderObject."
        );
        DartRuntimePrimitives.Assert(
            () => !Equals(child, after),
            () => (object?)"A RenderObject cannot be inserted after itself."
        );
        DartRuntimePrimitives.Assert(() => !Equals(child, _firstChild));
        DartRuntimePrimitives.Assert(() => !Equals(child, _lastChild));
        adoptChild(child);
        DartRuntimePrimitives.Assert(
            () => child.parentData is ContainerBoxParentData<RenderBox>,
            () =>
                (object?)
                    $"A child of {GetType()} has parentData of type {DartRuntimePrimitives.RuntimeType(child.parentData)}, "
                + $"which does not conform to {typeof(ContainerBoxParentData<RenderBox>)}. Class using ContainerRenderObjectMixin "
                + $"should override setupParentData() to set parentData to type {typeof(ContainerBoxParentData<RenderBox>)}."
        );
        _insertIntoChildList(child, after: after);
    }

    public virtual void add(RenderBox child)
    {
        insert(child, after: _lastChild);
    }

    public virtual void addAll(List<RenderBox>? children)
    {
        children?.forEach((__arg0) => ((Action<RenderBox>)add)(__arg0));
    }

    public virtual void _removeFromChildList(RenderBox child)
    {
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        DartRuntimePrimitives.Assert(() =>
            _debugUltimatePreviousSiblingOf(child, equals: _firstChild)
        );
        DartRuntimePrimitives.Assert(() => _debugUltimateNextSiblingOf(child, equals: _lastChild));
        DartRuntimePrimitives.Assert(() => _childCount >= 0L);
        if (childParentData.previousSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_firstChild, child));
            _firstChild = childParentData.nextSibling;
        }
        else
        {
            var childPreviousSiblingParentData = (
                (ContainerBoxParentData<RenderBox>?)childParentData.previousSibling!.parentData!
            )!;
            childPreviousSiblingParentData.nextSibling = childParentData.nextSibling;
        }
        if (childParentData.nextSibling is null)
        {
            DartRuntimePrimitives.Assert(() => Equals(_lastChild, child));
            _lastChild = childParentData.previousSibling;
        }
        else
        {
            var childNextSiblingParentData = (
                (ContainerBoxParentData<RenderBox>?)childParentData.nextSibling!.parentData!
            )!;
            childNextSiblingParentData.previousSibling = childParentData.previousSibling;
        }
        childParentData.previousSibling = null;
        childParentData.nextSibling = null;
        _childCount -= 1L;
    }

    public virtual void remove(RenderBox child)
    {
        _removeFromChildList(child);
        dropChild(child);
    }

    public virtual void removeAll()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            RenderBox? next = childParentData.nextSibling;
            childParentData.previousSibling = null;
            childParentData.nextSibling = null;
            dropChild(child);
            child = next;
        }
        _firstChild = null;
        _lastChild = null;
        _childCount = 0L;
    }

    public virtual void move(RenderBox child, RenderBox? after = null)
    {
        DartRuntimePrimitives.Assert(() => !Equals(child, this));
        DartRuntimePrimitives.Assert(() => !Equals(after, this));
        DartRuntimePrimitives.Assert(() => !Equals(child, after));
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        if (Equals(childParentData.previousSibling, after))
        {
            return;
        }
        _removeFromChildList(child);
        _insertIntoChildList(child, after: after);
        markNeedsLayout();
    }

    public override void redepthChildren()
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            redepthChild(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        RenderBox? child = _firstChild;
        while (child is not null)
        {
            visitor(child);
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            child = childParentData.nextSibling;
        }
    }

    public virtual RenderBox? firstChild => _firstChild;
    public virtual RenderBox? lastChild => _lastChild;

    public virtual RenderBox? childBefore(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        return childParentData.previousSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderBox? childAfter(RenderBox child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
        return childParentData.nextSibling;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var children = new List<DiagnosticsNode>();
        if (firstChild is not null)
        {
            RenderBox child = firstChild!;
            var count = 1L;
            while (true)
            {
                children.Add(
                    ((Diagnosticable)child).toDiagnosticsNode(name: $"child__183606 {count}")
                );
                if (Equals(child, lastChild))
                {
                    break;
                }
                count += 1L;
                var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
                child = childParentData.nextSibling!;
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToFirstActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            double? result = child.getDistanceToActualBaseline(baseline);
            if (result is not null)
            {
                double result__138852__value138916 = DartRuntimePrimitives.RequireValue(result);
                return DartRuntimePrimitives.RequireValue(result__138852__value138916)
                    + childParentData.offset.dy;
            }
            child = childParentData.nextSibling;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? defaultComputeDistanceToHighestActualBaseline(TextBaseline baseline)
    {
        DartRuntimePrimitives.Assert(() => !debugNeedsLayout);
        BaselineOffset minBaseline = BaselineOffset.noBaseline;
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            BaselineOffset candidate = new BaselineOffset(
                child.getDistanceToActualBaseline(baseline)
            ).op_Add(childParentData.offset.dy);
            minBaseline = minBaseline.minOf(candidate);
            child = childParentData.nextSibling;
        }
        return minBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool defaultHitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? child = lastChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            bool isHit = result.addWithPaintOffset(
                offset: childParentData.offset,
                position: position,
                hitTest: (result, transformed) =>
                {
                    DartRuntimePrimitives.Assert(() =>
                        Equals(transformed, position - childParentData.offset)
                    );
                    return child!.hitTest(result, position: transformed);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            if (isHit)
            {
                return true;
            }
            child = childParentData.previousSibling;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void defaultPaint(PaintingContext context, Offset offset)
    {
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            context.paintChild(child, childParentData.offset + offset);
            child = childParentData.nextSibling;
        }
    }

    public virtual List<RenderBox> getChildrenAsList()
    {
        var result = new List<RenderBox>();
        RenderBox? child = firstChild;
        while (child is not null)
        {
            var childParentData = ((ContainerBoxParentData<RenderBox>?)child.parentData!)!;
            result.Add(child!);
            child = childParentData.nextSibling;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
