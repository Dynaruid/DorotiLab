// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/button_style_button.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum IconAlignment
{
    start,
    end,
}

public abstract class ButtonStyleButton : StatefulWidget
{
    public virtual Action? onPressed { get; private set; }
    public virtual Action? onLongPress { get; private set; }
    public virtual Action<bool>? onHover { get; private set; }
    public virtual Action<bool>? onFocusChange { get; private set; }
    public virtual ButtonStyle? style { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual WidgetStatesController? statesController { get; private set; }
    public virtual bool? isSemanticButton { get; private set; }
    public virtual IconAlignment? iconAlignment { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual Widget? child { get; private set; }

    protected ButtonStyleButton(
        Key? key = null,
        Action? onPressed = default!,
        Action? onLongPress = default!,
        Action<bool>? onHover = default!,
        Action<bool>? onFocusChange = default!,
        ButtonStyle? style = default!,
        FocusNode? focusNode = default!,
        bool autofocus = default!,
        Clip? clipBehavior = default!,
        WidgetStatesController? statesController = null,
        bool? isSemanticButton = true,
        IconAlignment? iconAlignment = null,
        string? tooltip = null,
        Widget? child = default!
    )
        : base(key: key)
    {
        this.onPressed = onPressed;
        this.onLongPress = onLongPress;
        this.onHover = onHover;
        this.onFocusChange = onFocusChange;
        this.style = style;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.clipBehavior = clipBehavior;
        this.statesController = statesController;
        this.isSemanticButton = isSemanticButton;
        this.iconAlignment = iconAlignment;
        this.tooltip = tooltip;
        this.child = child;
    }

    public virtual ButtonStyle defaultStyleOf(BuildContext context) => default!;

    public virtual ButtonStyle? themeStyleOf(BuildContext context) => default;

    public virtual bool enabled =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (onPressed is not null) || (onLongPress is not null)
        );

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _ButtonStyleState__button_style_button());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("enabled", value: enabled, ifFalse: "disabled"));
        properties.add(new DiagnosticsProperty<ButtonStyle>("style", style, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<FocusNode>("focusNode", focusNode, defaultValue: null)
        );
    }

    public static WidgetStateProperty<T>? allOrNull<T>(T? value) =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<T>>(
            (value is null) ? null : new WidgetStatePropertyAll<T>(value)
        );

    public static WidgetStateProperty<Color?>? defaultColor(Color? enabled, Color? disabled)
    {
        if ((enabled ?? disabled) is null)
        {
            return null;
        }
        return (WidgetStateProperty<Color?>?)
            WidgetStateProperty<Color?>.CreateFromMap(
                new DartMap<WidgetStatesConstraint, Color?>
                {
                    [WidgetState.disabled.asConstraint()] = disabled,
                    [WidgetStateMembers.any] = enabled,
                }.cast<WidgetStatesConstraint, Color?>()
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static EdgeInsetsGeometry scaledPadding(
        EdgeInsetsGeometry geometry1x,
        EdgeInsetsGeometry geometry2x,
        EdgeInsetsGeometry geometry3x,
        double fontSizeMultiplier
    )
    {
        return fontSizeMultiplier switch
        {
            <= 1L => geometry1x,
            < 2L => EdgeInsetsGeometry.lerp(geometry1x, geometry2x, fontSizeMultiplier - 1L)!,
            < 3L => EdgeInsetsGeometry.lerp(geometry2x, geometry3x, fontSizeMultiplier - 2L)!,
            _ => geometry3x,
        };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ButtonStyleState__button_style_button
    : State<ButtonStyleButton>,
        TickerProviderStateMixin<ButtonStyleButton>
{
    public virtual AnimationController? controller { get; set; } = default;
    public virtual double? elevation { get; set; } = default;
    public virtual Color? backgroundColor { get; set; } = default;
    public virtual WidgetStatesController? internalStatesController { get; set; } = default;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual void handleStatesControllerChange()
    {
        setState(() => { });
    }

    public virtual WidgetStatesController statesController =>
        DartRuntimePrimitives.ConvertValue<WidgetStatesController>(
            widget.statesController ?? internalStatesController!
        );

    public virtual void initStatesController()
    {
        if (widget.statesController is null)
        {
            internalStatesController = new WidgetStatesController();
        }
        statesController.update(WidgetState.disabled, !widget.enabled);
        statesController.addListener(handleStatesControllerChange);
    }

    public override void initState()
    {
        base.initState();
        initStatesController();
    }

    public override void didUpdateWidget(ButtonStyleButton oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.statesController, oldWidget.statesController))
        {
            oldWidget.statesController?.removeListener(handleStatesControllerChange);
            if (widget.statesController is not null)
            {
                internalStatesController?.dispose();
                internalStatesController = null;
            }
            initStatesController();
        }
        if (widget.enabled != oldWidget.enabled)
        {
            statesController.update(WidgetState.disabled, !widget.enabled);
            if (!widget.enabled)
            {
                statesController.update(WidgetState.pressed, false);
            }
        }
    }

    public override void dispose()
    {
        statesController.removeListener(handleStatesControllerChange);
        internalStatesController?.dispose();
        controller?.dispose();
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
        ThemeData theme = Theme.of(context);
        IconThemeData iconThemeLocal = IconTheme.of(context);
        ButtonStyle? widgetStyle = widget.style;
        ButtonStyle? themeStyle = widget.themeStyleOf(context);
        ButtonStyle defaultStyle = widget.defaultStyleOf(context);
        P? effectiveValue<P>(Func<ButtonStyle?, P?> getProperty)
        {
            P? widgetValue = getProperty(widgetStyle);
            P? themeValue = getProperty(themeStyle);
            P? defaultValue = getProperty(defaultStyle);
            return (widgetValue ?? themeValue) ?? defaultValue;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        P? resolve<P>(Func<ButtonStyle?, WidgetStateProperty<P>?> getProperty)
        {
            return effectiveValue(
                (style) =>
                {
                    return getProperty(style) is { } property
                        ? property.resolve(statesController.value)
                        : default;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }
            );
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        Color? effectiveIconColor()
        {
            return widgetStyle?.iconColor?.resolve(statesController.value)
                ?? themeStyle?.iconColor?.resolve(statesController.value)
                ?? widgetStyle?.foregroundColor?.resolve(statesController.value)
                ?? themeStyle?.foregroundColor?.resolve(statesController.value)
                ?? defaultStyle.iconColor?.resolve(statesController.value)
                ?? defaultStyle.foregroundColor?.resolve(statesController.value);
        }
        double? resolvedElevation = resolve((style) => style?.elevation) ?? 0.0;
        TextStyle? resolvedTextStyle = resolve((style) => style?.textStyle);
        Color? resolvedBackgroundColor = resolve((style) => style?.backgroundColor);
        Color? resolvedForegroundColor = resolve((style) => style?.foregroundColor);
        Color? resolvedShadowColor = resolve((style) => style?.shadowColor);
        Color? resolvedSurfaceTintColor = resolve((style) => style?.surfaceTintColor);
        EdgeInsetsGeometry? resolvedPadding =
            resolve((style) => style?.padding)
            ?? EdgeInsets.CreateSymmetric(horizontal: 16, vertical: 8);
        Size? resolvedMinimumSize = resolve((style) => style?.minimumSize) ?? new Size(64, 40);
        Size? resolvedFixedSize = resolve((style) => style?.fixedSize);
        Size? resolvedMaximumSize =
            resolve((style) => style?.maximumSize)
            ?? new Size(double.PositiveInfinity, double.PositiveInfinity);
        Color? resolvedIconColor = effectiveIconColor();
        double? resolvedIconSize = resolve((style) => style?.iconSize);
        BorderSide? resolvedSide = resolve((style) => style?.side);
        OutlinedBorder? resolvedShape =
            resolve((style) => style?.shape) ?? new RoundedRectangleBorder();
        WidgetStateMouseCursor mouseCursorLocal = new _MouseCursor__button_style_button(
            (states) => effectiveValue((style) => style?.mouseCursor?.resolve(states))
        );
        WidgetStateProperty<Color?> overlayColorLocal = WidgetStateProperty.resolveWith(
            (states) => effectiveValue((style) => style?.overlayColor?.resolve(states))
        );
        VisualDensity? resolvedVisualDensity =
            effectiveValue((style) => style?.visualDensity) ?? theme.visualDensity;
        MaterialTapTargetSize? resolvedTapTargetSize =
            effectiveValue((style) => style?.tapTargetSize) ?? MaterialTapTargetSize.padded;
        Duration? resolvedAnimationDuration =
            effectiveValue((style) => style?.animationDuration)
            ?? Duration.Create(milliseconds: 200);
        bool resolvedEnableFeedback = effectiveValue((style) => style?.enableFeedback) ?? true;
        AlignmentGeometry? resolvedAlignment =
            effectiveValue((style) => style?.alignment) ?? Alignment.center;
        Offset densityAdjustment = resolvedVisualDensity!.baseSizeAdjustment;
        InteractiveInkFeatureFactory? resolvedSplashFactory = effectiveValue(
            (style) => style?.splashFactory
        );
        Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? resolvedBackgroundBuilder =
            effectiveValue((style) => style?.backgroundBuilder);
        Func<BuildContext, HashSet<WidgetState>, Widget?, Widget>? resolvedForegroundBuilder =
            effectiveValue((style) => style?.foregroundBuilder);
        Clip effectiveClipBehavior =
            widget.clipBehavior
            ?? (
                ((resolvedBackgroundBuilder ?? resolvedForegroundBuilder) is not null)
                    ? Clip.antiAlias
                    : Clip.none
            );
        BoxConstraints effectiveConstraintsLocal = resolvedVisualDensity.effectiveConstraints(
            new BoxConstraints(
                minWidth: (
                    resolvedMinimumSize
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).width,
                minHeight: (
                    resolvedMinimumSize
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).height,
                maxWidth: (
                    resolvedMaximumSize
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).width,
                maxHeight: (
                    resolvedMaximumSize
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ).height
            )
        );
        if (resolvedFixedSize is not null)
        {
            Size resolvedFixedSize__16402__value18999 = (
                resolvedFixedSize
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            );
            Size sizeLocal = effectiveConstraintsLocal.constrain(
                (
                    (
                        resolvedFixedSize__16402__value18999
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                )
            );
            if (double.IsFinite(sizeLocal.width))
            {
                effectiveConstraintsLocal = effectiveConstraintsLocal.copyWith(
                    minWidth: sizeLocal.width,
                    maxWidth: sizeLocal.width
                );
            }
            if (double.IsFinite(sizeLocal.height))
            {
                effectiveConstraintsLocal = effectiveConstraintsLocal.copyWith(
                    minHeight: sizeLocal.height,
                    maxHeight: sizeLocal.height
                );
            }
        }
        double dyLocal = densityAdjustment.dy;
        double dxLocal = Math.Max(0, densityAdjustment.dx);
        EdgeInsetsGeometry paddingLocal = resolvedPadding!
            .add(new EdgeInsets(dxLocal, dyLocal, dxLocal, dyLocal))
            .clamp(EdgeInsets.zero, EdgeInsetsGeometry.infinity);
        if (
            (
                (
                    resolvedAnimationDuration
                    ?? throw new global::System.NullReferenceException(
                        "Dart null assertion failed."
                    )
                ) > Duration.zero
            )
            && (elevation is not null)
            && (backgroundColor is not null)
            && (elevation != resolvedElevation)
            && (backgroundColor!.value != resolvedBackgroundColor!.value)
            && (backgroundColor!.opacity == 1L)
            && (resolvedBackgroundColor.opacity < 1L)
            && (resolvedElevation == 0L)
        )
        {
            if (
                !Equals(
                    controller?.duration,
                    (
                        resolvedAnimationDuration
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                )
            )
            {
                controller?.dispose();
                controller = (
                    (Func<AnimationController>)(
                        () =>
                        {
                            var __cascade = new AnimationController(
                                duration: (
                                    resolvedAnimationDuration
                                    ?? throw new global::System.NullReferenceException(
                                        "Dart null assertion failed."
                                    )
                                ),
                                vsync: this
                            );
                            __cascade.addStatusListener(
                                (status) =>
                                {
                                    if (Equals(status, AnimationStatus.completed))
                                    {
                                        setState(() => { });
                                    }
                                }
                            );
                            return __cascade;
                        }
                    )
                )();
            }
            resolvedBackgroundColor = backgroundColor;
            controller!.value = 0;
            controller!.forward();
        }
        elevation = resolvedElevation;
        backgroundColor = resolvedBackgroundColor;
        Widget result = new Padding(
            padding: paddingLocal,
            child: new Align(
                alignment: resolvedAlignment!,
                widthFactor: 1.0,
                heightFactor: 1.0,
                child: (resolvedForegroundBuilder is not null)
                    ? resolvedForegroundBuilder(context, statesController.value, widget.child)
                    : widget.child
            )
        );
        if (resolvedBackgroundBuilder is not null)
        {
            result = resolvedBackgroundBuilder(context, statesController.value, result);
        }
        result = DartRuntimePrimitives.ConvertValue<Widget>(
            new AnimatedTheme(
                duration: (
                    (
                        resolvedAnimationDuration
                        ?? throw new global::System.NullReferenceException(
                            "Dart null assertion failed."
                        )
                    )
                ),
                data: theme.copyWith(
                    iconTheme: iconThemeLocal.merge(
                        new IconThemeData(color: resolvedIconColor, size: resolvedIconSize)
                    )
                ),
                child: new InkWell(
                    onTap: widget.onPressed,
                    onLongPress: widget.onLongPress,
                    onHover: widget.onHover,
                    mouseCursor: mouseCursorLocal,
                    enableFeedback: resolvedEnableFeedback,
                    focusNode: widget.focusNode,
                    canRequestFocus: widget.enabled,
                    onFocusChange: widget.onFocusChange,
                    autofocus: widget.autofocus,
                    splashFactory: resolvedSplashFactory,
                    overlayColor: overlayColorLocal,
                    highlightColor: Colors.transparent,
                    customBorder: resolvedShape!.copyWith(side: resolvedSide),
                    statesController: statesController,
                    child: result
                )
            )
        );
        if (widget.tooltip is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(
                new Tooltip(message: widget.tooltip, child: result)
            );
        }
        Size minSizeLocal = default!;
        switch (
            (
                resolvedTapTargetSize
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            )
        )
        {
            case MaterialTapTargetSize.padded:
            {
                minSizeLocal = new Size(
                    Widgets.ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dx,
                    Widgets.ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy
                );
                DartRuntimePrimitives.Assert(() => minSizeLocal.width >= 0.0);
                DartRuntimePrimitives.Assert(() => minSizeLocal.height >= 0.0);
                break;
            }
            case MaterialTapTargetSize.shrinkWrap:
            {
                minSizeLocal = Size.zero;
                break;
            }
        }
        return new Widgets.Semantics(
            container: true,
            button: widget.isSemanticButton,
            enabled: widget.enabled,
            child: new _InputPadding__button_style_button(
                minSize: minSizeLocal,
                child: new ConstrainedBox(
                    constraints: effectiveConstraintsLocal,
                    child: new Material(
                        elevation: (
                            resolvedElevation
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ),
                        textStyle: resolvedTextStyle?.copyWith(color: resolvedForegroundColor),
                        shape: resolvedShape.copyWith(side: resolvedSide),
                        color: resolvedBackgroundColor,
                        shadowColor: resolvedShadowColor,
                        surfaceTintColor: resolvedSurfaceTintColor,
                        type: (resolvedBackgroundColor is null)
                            ? MaterialType.transparency
                            : MaterialType.button,
                        animationDuration: (
                            resolvedAnimationDuration
                            ?? throw new global::System.NullReferenceException(
                                "Dart null assertion failed."
                            )
                        ),
                        clipBehavior: effectiveClipBehavior,
                        borderOnForeground: false,
                        child: result
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

internal class _MouseCursor__button_style_button : WidgetStateMouseCursor
{
    public virtual Func<HashSet<WidgetState>, MouseCursor?> resolveCallback { get; private set; } =
        default!;

    internal _MouseCursor__button_style_button(
        Func<HashSet<WidgetState>, MouseCursor?> resolveCallback
    )
    {
        this.resolveCallback = resolveCallback;
    }

    public override MouseCursor resolve(HashSet<WidgetState> states) =>
        DartRuntimePrimitives.ConvertValue<MouseCursor>(resolveCallback(states)!);

    public override string debugDescription => "ButtonStyleButton_MouseCursor";
}

internal class _InputPadding__button_style_button : SingleChildRenderObjectWidget
{
    public virtual Size minSize { get; private set; } = default!;

    internal _InputPadding__button_style_button(Widget? child = null, Size minSize = default!)
        : base(child: child)
    {
        this.minSize = minSize;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderInputPadding__button_style_button(minSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderInputPadding__button_style_button)renderObject;
        __renderObject.minSize = minSize;
    }
}

public class _RenderInputPadding__button_style_button : RenderShiftedBox
{
    internal virtual Size _minSize { get; set; } = default!;

    internal _RenderInputPadding__button_style_button(Size _minSize, RenderBox? child = null)
        : base(child)
    {
        this._minSize = _minSize;
    }

    public virtual Size minSize
    {
        get => _minSize;
        set
        {
            var __value = value;
            if (Equals(_minSize, __value))
            {
                return;
            }
            _minSize = __value;
            markNeedsLayout();
        }
    }

    public override double computeMinIntrinsicWidth(double height)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMinIntrinsicWidth(height), minSize.width);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMinIntrinsicHeight(width), minSize.height);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMaxIntrinsicWidth(height), minSize.width);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width)
    {
        if (child is not null)
        {
            return Math.Max(child!.getMaxIntrinsicHeight(width), minSize.height);
        }
        return 0.0;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Size _computeSize(
        BoxConstraints constraints,
        Func<RenderBox, BoxConstraints, Size> layoutChild
    )
    {
        if (child is not null)
        {
            Size childSize = layoutChild(child!, constraints);
            double widthLocal = Math.Max(childSize.width, minSize.width);
            double heightLocal = Math.Max(childSize.height, minSize.height);
            return constraints.constrain(new Size(widthLocal, heightLocal));
        }
        return Size.zero;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSize(
            constraints: constraints,
            layoutChild: ChildLayoutHelper.dryLayoutChild
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        double? result = childLocal.getDryBaseline(constraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(constraints);
        return (
                result
                ?? throw new global::System.NullReferenceException("Dart null assertion failed.")
            ) + Alignment.center.alongOffset(getDryLayout(constraints) - childSize).dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        size = _computeSize(constraints: constraints, layoutChild: ChildLayoutHelper.layoutChild);
        if (child is not null)
        {
            var childParentData = ((BoxParentData?)child!.parentData!)!;
            childParentData.offset = Alignment.center.alongOffset(size - child!.size);
        }
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (base.hitTest(result, position: position))
        {
            return true;
        }
        Offset centerLocal = child!.size.center(Offset.zero);
        return result.addWithRawTransform(
            transform: MatrixUtils.forceToPoint(centerLocal),
            position: centerLocal,
            hitTest: (result, position) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(position, centerLocal));
                return child!.hitTest(result, position: centerLocal);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
