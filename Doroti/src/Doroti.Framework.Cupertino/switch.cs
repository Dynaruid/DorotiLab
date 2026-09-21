// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/switch.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public static partial class SwitchLibrary
{
    internal static double _kDisabledOpacity = 0.5;
}

public static partial class SwitchLibrary
{
    internal static double _kThumbRadius = 14.0;
}

public static partial class SwitchLibrary
{
    internal static double _kTrackHeight = 31.0;
}

public static partial class SwitchLibrary
{
    internal static double _kTrackWidth = 51.0;
}

public static partial class SwitchLibrary
{
    internal static Size _kSwitchSize = new Size(59.0, 39.0);
}

public static partial class SwitchLibrary
{
    internal static double _kThumbExtensionFactor = 7.0;
}

public static partial class SwitchLibrary
{
    internal static List<BoxShadow> _kSwitchBoxShadows = new List<BoxShadow>
    {
        new BoxShadow(color: new Color(637534208L), offset: new Offset(0, 3), blurRadius: 8.0),
        new BoxShadow(color: new Color(251658240L), offset: new Offset(0, 3), blurRadius: 1.0),
    };
}

public static partial class SwitchLibrary
{
    internal static double _kDragCommitThreshold = 0.7;
}

public static partial class SwitchLibrary
{
    internal static double _kDragReverseThreshold = 0.2;
}

public static partial class SwitchLibrary
{
    internal static double _kOnLabelWidth = 1.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOnLabelHeight = 10.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOnLabelPaddingHorizontal = 11.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOffLabelWidth = 1.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOffLabelPaddingHorizontal = 12.0;
}

public static partial class SwitchLibrary
{
    internal static double _kOffLabelRadius = 5.0;
}

public static partial class SwitchLibrary
{
    internal static CupertinoDynamicColor _kOffLabelColor = new CupertinoDynamicColor(
        debugLabel: "offSwitchLabel",
        color: Color.fromARGB(255L, 179L, 179L, 179L),
        darkColor: Color.fromARGB(255L, 179L, 179L, 179L),
        highContrastColor: Color.fromARGB(255L, 255L, 255L, 255L),
        darkHighContrastColor: Color.fromARGB(255L, 255L, 255L, 255L)
    );
}

public class CupertinoSwitch : StatefulWidget
{
    public virtual bool value { get; private set; } = default!;
    public virtual Action<bool>? onChanged { get; private set; }
    public virtual Color? activeTrackColor { get; private set; }
    public virtual Color? inactiveTrackColor { get; private set; }
    public virtual Color? thumbColor { get; private set; }
    public virtual Color? inactiveThumbColor { get; private set; }
    public virtual Color? focusColor { get; private set; }
    public virtual Color? onLabelColor { get; private set; }
    public virtual Color? offLabelColor { get; private set; }
    public virtual IImageProvider? activeThumbImage { get; private set; } = default!;
    public virtual Action<object, System.Diagnostics.StackTrace?>? onActiveThumbImageError
    {
        get;
        private set;
    }
    public virtual IImageProvider? inactiveThumbImage { get; private set; } = default!;
    public virtual Action<object, System.Diagnostics.StackTrace?>? onInactiveThumbImageError
    {
        get;
        private set;
    }
    public virtual WidgetStateProperty<Color?>? trackOutlineColor { get; private set; }
    public virtual WidgetStateProperty<double?>? trackOutlineWidth { get; private set; }
    public virtual WidgetStateProperty<Icon?>? thumbIcon { get; private set; }
    public virtual WidgetStateProperty<MouseCursor>? mouseCursor { get; private set; }
    public virtual FocusNode? focusNode { get; private set; }
    public virtual Action<bool>? onFocusChange { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual bool? applyTheme { get; private set; }
    public virtual Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;

    public CupertinoSwitch(
        Key? key = null,
        bool value = default!,
        Action<bool>? onChanged = default!,
        Color? activeColor = null,
        Color? trackColor = null,
        Color? activeTrackColor = null,
        Color? inactiveTrackColor = null,
        Color? thumbColor = null,
        Color? inactiveThumbColor = null,
        bool? applyTheme = null,
        Color? focusColor = null,
        Color? onLabelColor = null,
        Color? offLabelColor = null,
        IImageProvider? activeThumbImage = null,
        Action<object, System.Diagnostics.StackTrace?>? onActiveThumbImageError = null,
        IImageProvider? inactiveThumbImage = null,
        Action<object, System.Diagnostics.StackTrace?>? onInactiveThumbImageError = null,
        WidgetStateProperty<Color?>? trackOutlineColor = null,
        WidgetStateProperty<double?>? trackOutlineWidth = null,
        WidgetStateProperty<Icon?>? thumbIcon = null,
        WidgetStateProperty<MouseCursor>? mouseCursor = null,
        FocusNode? focusNode = null,
        Action<bool>? onFocusChange = null,
        bool autofocus = false,
        Gestures.DragStartBehavior dragStartBehavior = Gestures.DragStartBehavior.start
    )
        : base(key: key)
    {
        this.value = value;
        this.onChanged = onChanged;
        this.thumbColor = thumbColor;
        this.inactiveThumbColor = inactiveThumbColor;
        this.applyTheme = applyTheme;
        this.focusColor = focusColor;
        this.onLabelColor = onLabelColor;
        this.offLabelColor = offLabelColor;
        this.activeThumbImage = activeThumbImage;
        this.onActiveThumbImageError = onActiveThumbImageError;
        this.inactiveThumbImage = inactiveThumbImage;
        this.onInactiveThumbImageError = onInactiveThumbImageError;
        this.trackOutlineColor = trackOutlineColor;
        this.trackOutlineWidth = trackOutlineWidth;
        this.thumbIcon = thumbIcon;
        this.mouseCursor = mouseCursor;
        this.focusNode = focusNode;
        this.onFocusChange = onFocusChange;
        this.autofocus = autofocus;
        this.dragStartBehavior = dragStartBehavior;
        this.activeTrackColor = activeTrackColor ?? activeColor;
        this.inactiveTrackColor = inactiveTrackColor ?? trackColor;
        System.Diagnostics.Debug.Assert(
            (activeThumbImage is not null) || (onActiveThumbImageError is null)
        );
        System.Diagnostics.Debug.Assert(
            (inactiveThumbImage is not null) || (onInactiveThumbImageError is null)
        );
        System.Diagnostics.Debug.Assert((activeTrackColor is null) || (activeColor is null));
        System.Diagnostics.Debug.Assert((inactiveTrackColor is null) || (trackColor is null));
    }

    public virtual Color? activeColor =>
        DartRuntimePrimitives.ConvertValue<Color>(activeTrackColor);
    public virtual Color? trackColor =>
        DartRuntimePrimitives.ConvertValue<Color>(inactiveTrackColor);

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSwitchState__switch());

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new FlagProperty("value", value: value, ifTrue: "on", ifFalse: "off", showName: true)
        );
        properties.add(
            new ObjectFlagProperty<Action<bool>>("onChanged", onChanged, ifNull: "disabled")
        );
    }
}

internal class _CupertinoSwitchState__switch
    : State<CupertinoSwitch>,
        TickerProviderStateMixin<CupertinoSwitch>,
        ToggleableStateMixin<CupertinoSwitch>
{
    internal virtual _SwitchPainter__switch _painter { get; private set; } =
        new _SwitchPainter__switch();
    internal virtual Offset _dragStartPosition { get; set; } = Offset.zero;
    internal virtual double _dragDelta { get; set; } = 0;
    internal virtual bool? _dragValue { get; set; } = default;
    internal virtual bool _needsPositionAnimation { get; set; } = false;
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

    public override void initState()
    {
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
        positionController.duration = Duration.Create(milliseconds: 200L);
        reactionController.duration = Duration.Create(milliseconds: 300L);
        DartRuntimePrimitives.Ignore(
            (
                (Func<CurvedAnimation>)(
                    () =>
                    {
                        var __cascade = position;
                        __cascade.curve = Curves.ease;
                        __cascade.reverseCurve = Curves.ease.flipped;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override void didUpdateWidget(CupertinoSwitch oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.value != widget.value)
        {
            animateToValue();
        }
    }

    public override void dispose()
    {
        _painter.dispose();
        _positionController.dispose();
        _position.dispose();
        _reactionController.dispose();
        _reaction.dispose();
        _reactionHoverFadeController.dispose();
        _reactionHoverFade.dispose();
        _reactionFocusFadeController.dispose();
        _reactionFocusFade.dispose();
        base.dispose();
    }

    public virtual Action<bool?>? onChanged =>
        (widget.onChanged is not null) ? _handleChanged : null;
    public virtual bool tristate => false;
    public virtual bool? value => widget.value;
    internal virtual WidgetStateProperty<Color?> _widgetThumbColor
    {
        get
        {
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        return widget.thumbColor;
                    }
                    return widget.inactiveThumbColor;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
        }
    }
    internal virtual WidgetStateProperty<Color?> _widgetTrackColor
    {
        get
        {
            return WidgetStateProperty.resolveWith(
                (states) =>
                {
                    if (states.Contains(WidgetState.selected))
                    {
                        return widget.activeTrackColor;
                    }
                    return widget.inactiveTrackColor;
                    throw new InvalidOperationException(
                        "Callback completed without returning a value."
                    );
                }
            );
        }
    }
    internal virtual WidgetStateProperty<MouseCursor> _defaultMouseCursor =>
        WidgetStateProperty.resolveWith(
            (states) =>
            {
                if (states.Contains(WidgetState.disabled))
                {
                    return MouseCursor.defer;
                }
                return Foundation.ConstantsLibrary.kIsWeb
                    ? SystemMouseCursors.click
                    : MouseCursor.defer;
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );

    internal virtual Color? _resolveTrackColor(Color? trackColor, HashSet<WidgetState> states)
    {
        if (trackColor is WidgetStateColor)
        {
            WidgetStateColor trackColor__as19180 = (WidgetStateColor)trackColor;
            return WidgetStateProperty.resolveAs<Color?>(trackColor__as19180, states);
        }
        return trackColor;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Color? _resolveThumbColor(Color? thumbColor, HashSet<WidgetState> states)
    {
        if (thumbColor is WidgetStateColor)
        {
            WidgetStateColor thumbColor__as19402 = (WidgetStateColor)thumbColor;
            return WidgetStateProperty.resolveAs<Color?>(thumbColor__as19402, states);
        }
        return thumbColor;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual double _trackInnerLength
    {
        get
        {
            double trackInnerStart = SwitchLibrary._kTrackHeight / 2.0;
            double trackInnerEnd = SwitchLibrary._kTrackWidth - trackInnerStart;
            double trackInnerLength = trackInnerEnd - trackInnerStart;
            return trackInnerLength;
        }
    }

    internal virtual void _handleOnTapDown(Gestures.TapDownDetails details)
    {
        if (isInteractive)
        {
            _dragStartPosition = details.globalPosition;
        }
    }

    internal virtual void _handleDragStart(Gestures.DragStartDetails details)
    {
        if (isInteractive)
        {
            reactionController.forward();
            if (!Equals(_dragStartPosition, Offset.zero))
            {
                double delta =
                    (details.globalPosition - _dragStartPosition).dx / SwitchLibrary._kTrackWidth;
                _dragDelta = Directionality.of(context) switch
                {
                    TextDirection.rtl => -delta,
                    TextDirection.ltr => delta,
                    _ => throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
                };
            }
            _dragValue = value;
        }
    }

    internal virtual void _handleDragUpdate(Gestures.DragUpdateDetails details)
    {
        if (isInteractive)
        {
            double delta =
                (
                    details.primaryDelta
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) / SwitchLibrary._kTrackWidth;
            _dragDelta += Directionality.of(context) switch
            {
                TextDirection.rtl => -delta,
                TextDirection.ltr => delta,
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
            var valueChangedWhileDragging = widget.value != _dragValue;
            double threshold = valueChangedWhileDragging
                ? SwitchLibrary._kDragReverseThreshold
                : SwitchLibrary._kDragCommitThreshold;
            double effectiveThreshold = widget.value ? -threshold : threshold;
            bool newDragValue = _dragDelta >= effectiveThreshold;
            if (_dragValue != newDragValue)
            {
                _emitVibration();
                if (newDragValue)
                {
                    positionController.forward();
                }
                else
                {
                    positionController.reverse();
                }
                _dragValue = newDragValue;
            }
        }
    }

    internal virtual void _handleDragEnd(Gestures.DragEndDetails details)
    {
        if (_dragValue != widget.value)
        {
            widget.onChanged?.Invoke(!widget.value);
            setState(() =>
            {
                _needsPositionAnimation = true;
            });
        }
        _dragStartPosition = Offset.zero;
        _dragDelta = 0;
        _dragValue = null;
        reactionController.reverse();
    }

    internal virtual void _handleChanged(bool? value)
    {
        DartRuntimePrimitives.Assert(() => value is not null);
        DartRuntimePrimitives.Assert(() => widget.onChanged is not null);
        widget.onChanged?.Invoke(
            (value ?? throw new global::System.NullReferenceException("A required value was null."))
        );
        _emitVibration();
    }

    internal virtual void _emitVibration()
    {
        switch (PlatformLibrary.defaultTargetPlatform)
        {
            case TargetPlatform.iOS:
            {
                DartRuntimePrimitives.Ignore(HapticFeedback.lightImpact());
                break;
            }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                break;
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        if (_needsPositionAnimation)
        {
            _needsPositionAnimation = false;
            animateToValue();
        }
        CupertinoThemeData theme = CupertinoTheme.of(context);
        Color activeColorLocal = CupertinoDynamicColor.resolve(
            (
                widget.activeTrackColor
                ?? (
                    ((widget.applyTheme ?? theme.applyThemeToAll) == true)
                        ? theme.primaryColor
                        : null
                )
            ) ?? CupertinoColors.systemGreen,
            context
        );
        (Color, Color)? onOffLabelColorsLocal = MediaQuery.onOffSwitchLabelsOf(context)
            ? (
                CupertinoDynamicColor.resolve(
                    widget.onLabelColor ?? CupertinoColors.white,
                    context
                ),
                CupertinoDynamicColor.resolve(
                    widget.offLabelColor ?? SwitchLibrary._kOffLabelColor,
                    context
                )
            )
            : null;
        HashSet<WidgetState> activeStates = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __cascade = states;
                    __cascade.Add(WidgetState.selected);
                    return __cascade;
                }
            )
        )();
        HashSet<WidgetState> inactiveStates = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __cascade = states;
                    __cascade.Remove(WidgetState.selected);
                    return __cascade;
                }
            )
        )();
        Color effectiveActiveThumbColor =
            (
                _resolveThumbColor(widget.thumbColor, activeStates)
                ?? _widgetThumbColor.resolve(activeStates)
            ) ?? CupertinoColors.white;
        Color effectiveInactiveThumbColor =
            (
                _resolveThumbColor(widget.inactiveThumbColor, inactiveStates)
                ?? _widgetThumbColor.resolve(inactiveStates)
            ) ?? effectiveActiveThumbColor;
        Color effectiveActiveTrackColor =
            _widgetTrackColor.resolve(activeStates) ?? activeColorLocal;
        Color? effectiveActiveTrackOutlineColor = widget.trackOutlineColor?.resolve(activeStates);
        double? effectiveActiveTrackOutlineWidth = widget.trackOutlineWidth?.resolve(activeStates);
        Color effectiveInactiveTrackColor =
            _resolveTrackColor(widget.trackColor, inactiveStates)
            ?? CupertinoDynamicColor.resolve(CupertinoColors.secondarySystemFill, context);
        Color? effectiveInactiveTrackOutlineColor = widget.trackOutlineColor?.resolve(
            inactiveStates
        );
        double? effectiveInactiveTrackOutlineWidth = widget.trackOutlineWidth?.resolve(
            inactiveStates
        );
        Icon? effectiveActiveIcon = widget.thumbIcon?.resolve(activeStates);
        Icon? effectiveInactiveIcon = widget.thumbIcon?.resolve(inactiveStates);
        Color effectiveActiveIconColor = effectiveActiveIcon?.color ?? CupertinoColors.black;
        Color effectiveInactiveIconColor = effectiveInactiveIcon?.color ?? CupertinoColors.black;
        var activePressedStates = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __cascade = activeStates;
                    __cascade.Add(WidgetState.pressed);
                    return __cascade;
                }
            )
        )();
        Color effectiveActivePressedThumbColor =
            (
                _resolveThumbColor(widget.thumbColor, activePressedStates)
                ?? _widgetThumbColor.resolve(activePressedStates)
            ) ?? CupertinoColors.white;
        var inactivePressedStates = (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __cascade = inactiveStates;
                    __cascade.Add(WidgetState.pressed);
                    return __cascade;
                }
            )
        )();
        Color effectiveInactivePressedThumbColor =
            (
                _resolveThumbColor(widget.thumbColor, inactivePressedStates)
                ?? _widgetThumbColor.resolve(inactivePressedStates)
            ) ?? CupertinoColors.white;
        WidgetStateProperty<MouseCursor> effectiveMouseCursor =
            widget.mouseCursor ?? _defaultMouseCursor;
        return new Widgets.Semantics(
            toggled: widget.value,
            child: new GestureDetector(
                excludeFromSemantics: true,
                onTapDown: _handleOnTapDown,
                onHorizontalDragStart: _handleDragStart,
                onHorizontalDragUpdate: _handleDragUpdate,
                onHorizontalDragEnd: _handleDragEnd,
                dragStartBehavior: widget.dragStartBehavior,
                child: new Opacity(
                    opacity: (onChanged is null) ? SwitchLibrary._kDisabledOpacity : 1,
                    child: buildToggleable(
                        mouseCursor: effectiveMouseCursor,
                        focusNode: widget.focusNode,
                        onFocusChange: widget.onFocusChange,
                        autofocus: widget.autofocus,
                        size: SwitchLibrary._kSwitchSize,
                        painter: (
                            (Func<_SwitchPainter__switch>)(
                                () =>
                                {
                                    var __cascade = _painter;
                                    __cascade.position = position;
                                    __cascade.reaction = reaction;
                                    __cascade.reactionFocusFade = reactionFocusFade;
                                    __cascade.reactionHoverFade = reactionHoverFade;
                                    __cascade.focusColor = CupertinoDynamicColor.resolve(
                                        widget.focusColor
                                            ?? HSLColor
                                                .CreateFromColor(
                                                    activeColorLocal.withOpacity(
                                                        ConstantsLibrary.kCupertinoFocusColorOpacity
                                                    )
                                                )
                                                .withLightness(
                                                    ConstantsLibrary.kCupertinoFocusColorBrightness
                                                )
                                                .withSaturation(
                                                    ConstantsLibrary.kCupertinoFocusColorSaturation
                                                )
                                                .toColor(),
                                        context
                                    );
                                    __cascade.downPosition = downPosition;
                                    __cascade.isFocused = states.Contains(WidgetState.focused);
                                    __cascade.isHovered = states.Contains(WidgetState.hovered);
                                    __cascade.activeColor = effectiveActiveThumbColor;
                                    __cascade.inactiveColor = effectiveInactiveThumbColor;
                                    __cascade.activePressedColor = effectiveActivePressedThumbColor;
                                    __cascade.onOffLabelColors = onOffLabelColorsLocal;
                                    __cascade.inactivePressedColor =
                                        effectiveInactivePressedThumbColor;
                                    __cascade.activeThumbImage = widget.activeThumbImage;
                                    __cascade.onActiveThumbImageError =
                                        widget.onActiveThumbImageError;
                                    __cascade.inactiveThumbImage = widget.inactiveThumbImage;
                                    __cascade.onInactiveThumbImageError =
                                        widget.onInactiveThumbImageError;
                                    __cascade.activeTrackColor = effectiveActiveTrackColor;
                                    __cascade.activeTrackOutlineColor =
                                        effectiveActiveTrackOutlineColor;
                                    __cascade.activeTrackOutlineWidth =
                                        effectiveActiveTrackOutlineWidth;
                                    __cascade.inactiveTrackColor = effectiveInactiveTrackColor;
                                    __cascade.inactiveTrackOutlineColor =
                                        effectiveInactiveTrackOutlineColor;
                                    __cascade.inactiveTrackOutlineWidth =
                                        effectiveInactiveTrackOutlineWidth;
                                    __cascade.configuration =
                                        ImageLibrary.createLocalImageConfiguration(context);
                                    __cascade.isInteractive = isInteractive;
                                    __cascade.trackInnerLength = _trackInnerLength;
                                    __cascade.textDirection = Directionality.of(context);
                                    __cascade.activeIconColor = effectiveActiveIconColor;
                                    __cascade.inactiveIconColor = effectiveInactiveIconColor;
                                    __cascade.activeIcon = effectiveActiveIcon;
                                    __cascade.inactiveIcon = effectiveInactiveIcon;
                                    __cascade.iconTheme = IconTheme.of(context);
                                    __cascade.surfaceColor = theme.scaffoldBackgroundColor;
                                    __cascade.positionController = positionController;
                                    return __cascade;
                                }
                            )
                        )()
                    )
                )
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
    public virtual bool isInteractive =>
        DartRuntimePrimitives.ConvertValue<bool>(onChanged is not null);

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

    public virtual void _handleTapDown(Gestures.TapDownDetails details)
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
        context.findRenderObject()!.sendSemanticsEvent(new Semantics.TapSemanticEvent());
    }

    public virtual void _handleTapEnd(Gestures.TapUpDetails? __unused0 = null)
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

    public virtual HashSet<WidgetState> states =>
        (
            (Func<HashSet<WidgetState>>)(
                () =>
                {
                    var __collection10795 = new HashSet<WidgetState>();
                    if (!isInteractive)
                    {
                        __collection10795.Add(WidgetState.disabled);
                    }
                    if (_hovering)
                    {
                        __collection10795.Add(WidgetState.hovered);
                    }
                    if (_focused)
                    {
                        __collection10795.Add(WidgetState.focused);
                    }
                    if (value ?? true)
                    {
                        __collection10795.Add(WidgetState.selected);
                    }
                    return __collection10795;
                }
            )
        )();

    public virtual Widget buildToggleable(
        FocusNode? focusNode = null,
        Action<bool>? onFocusChange = null,
        bool autofocus = false,
        WidgetStateProperty<MouseCursor>? mouseCursor = null,
        Size size = default!,
        object? painter = default!
    )
    {
        return buildToggleableWithChild(
            focusNode: focusNode,
            onFocusChange: onFocusChange,
            autofocus: autofocus,
            mouseCursor: mouseCursor,
            child: new CustomPaint(size: size, painter: painter)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
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
                child: new Widgets.Semantics(enabled: isInteractive, child: child)
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _SwitchPainter__switch : ToggleablePainter
{
    internal virtual AnimationController? _positionController { get; set; } = default;
    internal virtual CurvedAnimation? _colorAnimation { get; set; } = default;
    internal virtual Icon? _activeIcon { get; set; } = default;
    internal virtual Icon? _inactiveIcon { get; set; } = default;
    internal virtual IconThemeData? _iconTheme { get; set; } = default;
    internal virtual Color? _activeIconColor { get; set; } = default;
    internal virtual Color? _inactiveIconColor { get; set; } = default;
    internal virtual Color? _activePressedColor { get; set; } = default;
    internal virtual Color? _inactivePressedColor { get; set; } = default;
    internal virtual IImageProvider? _activeThumbImage { get; set; } = default!;
    internal virtual Action<
        object,
        System.Diagnostics.StackTrace?
    >? _onActiveThumbImageError { get; set; } = default;
    internal virtual IImageProvider? _inactiveThumbImage { get; set; } = default!;
    internal virtual Action<
        object,
        System.Diagnostics.StackTrace?
    >? _onInactiveThumbImageError { get; set; } = default;
    internal virtual Color? _activeTrackColor { get; set; } = default;
    internal virtual Color? _activeTrackOutlineColor { get; set; } = default;
    internal virtual Color? _inactiveTrackOutlineColor { get; set; } = default;
    internal virtual double? _activeTrackOutlineWidth { get; set; } = default;
    internal virtual double? _inactiveTrackOutlineWidth { get; set; } = default;
    internal virtual Color? _inactiveTrackColor { get; set; } = default;
    internal virtual ImageConfiguration? _configuration { get; set; } = default;
    internal virtual TextDirection? _textDirection { get; set; } = default;
    internal virtual Color? _surfaceColor { get; set; } = default;
    internal virtual bool? _isInteractive { get; set; } = default;
    internal virtual double? _trackInnerLength { get; set; } = default;
    internal virtual (Color, Color)? _onOffLabelColors { get; set; } = default;
    internal virtual TextPainter _textPainter { get; private set; } = new TextPainter();
    internal virtual Color? _cachedThumbColor { get; set; } = default;
    internal virtual IImageProvider? _cachedThumbImage { get; set; } = default!;
    internal virtual Action<
        object,
        System.Diagnostics.StackTrace?
    >? _cachedThumbErrorListener { get; set; } = default;
    internal virtual BoxPainter? _cachedThumbPainter { get; set; } = default;
    internal virtual bool _isPainting { get; set; } = false;
    internal virtual bool _stopPressAnimation { get; set; } = false;
    internal virtual double? _pressedThumbExtension { get; set; } = default;

    public virtual AnimationController positionController
    {
        get => _positionController!;
        set
        {
            var __value = value;
            if (Equals(__value, _positionController))
            {
                return;
            }
            _positionController = __value;
            _colorAnimation?.dispose();
            _colorAnimation = new CurvedAnimation(
                parent: positionController,
                curve: Curves.easeOut,
                reverseCurve: Curves.easeIn
            );
            notifyListeners();
        }
    }
    public virtual Icon? activeIcon
    {
        get => _activeIcon;
        set
        {
            var __value = value;
            if (Equals(__value, _activeIcon))
            {
                return;
            }
            _activeIcon = __value;
            notifyListeners();
        }
    }
    public virtual Icon? inactiveIcon
    {
        get => _inactiveIcon;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveIcon))
            {
                return;
            }
            _inactiveIcon = __value;
            notifyListeners();
        }
    }
    public virtual IconThemeData? iconTheme
    {
        get => _iconTheme;
        set
        {
            var __value = value;
            if (Equals(__value, _iconTheme))
            {
                return;
            }
            _iconTheme = __value;
            notifyListeners();
        }
    }
    public virtual Color activeIconColor
    {
        get => _activeIconColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _activeIconColor))
            {
                return;
            }
            _activeIconColor = __value;
            notifyListeners();
        }
    }
    public virtual Color inactiveIconColor
    {
        get => _inactiveIconColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveIconColor))
            {
                return;
            }
            _inactiveIconColor = __value;
            notifyListeners();
        }
    }
    public virtual Color activePressedColor
    {
        get => _activePressedColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _activePressedColor))
            {
                return;
            }
            _activePressedColor = __value;
            notifyListeners();
        }
    }
    public virtual Color inactivePressedColor
    {
        get => _inactivePressedColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _inactivePressedColor))
            {
                return;
            }
            _inactivePressedColor = __value;
            notifyListeners();
        }
    }
    public virtual IImageProvider? activeThumbImage
    {
        get => _activeThumbImage;
        set
        {
            var __value = value;
            if (Equals(__value, _activeThumbImage))
            {
                return;
            }
            _activeThumbImage = __value;
            notifyListeners();
        }
    }
    public virtual Action<object, System.Diagnostics.StackTrace?>? onActiveThumbImageError
    {
        get => _onActiveThumbImageError;
        set
        {
            var __value = value;
            if (Equals(__value, _onActiveThumbImageError))
            {
                return;
            }
            _onActiveThumbImageError = __value;
            notifyListeners();
        }
    }
    public virtual IImageProvider? inactiveThumbImage
    {
        get => _inactiveThumbImage;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveThumbImage))
            {
                return;
            }
            _inactiveThumbImage = __value;
            notifyListeners();
        }
    }
    public virtual Action<object, System.Diagnostics.StackTrace?>? onInactiveThumbImageError
    {
        get => _onInactiveThumbImageError;
        set
        {
            var __value = value;
            if (Equals(__value, _onInactiveThumbImageError))
            {
                return;
            }
            _onInactiveThumbImageError = __value;
            notifyListeners();
        }
    }
    public virtual Color activeTrackColor
    {
        get => _activeTrackColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _activeTrackColor))
            {
                return;
            }
            _activeTrackColor = __value;
            notifyListeners();
        }
    }
    public virtual Color? activeTrackOutlineColor
    {
        get => _activeTrackOutlineColor;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(__value, _activeTrackOutlineColor))
            {
                return;
            }
            _activeTrackOutlineColor = __value;
            notifyListeners();
        }
    }
    public virtual Color? inactiveTrackOutlineColor
    {
        get => _inactiveTrackOutlineColor;
        set
        {
            var __value = value is null ? null : value;
            if (Equals(__value, _inactiveTrackOutlineColor))
            {
                return;
            }
            _inactiveTrackOutlineColor = __value;
            notifyListeners();
        }
    }
    public virtual double? activeTrackOutlineWidth
    {
        get => _activeTrackOutlineWidth;
        set
        {
            var __value = value;
            if (__value == _activeTrackOutlineWidth)
            {
                return;
            }
            _activeTrackOutlineWidth = __value;
            notifyListeners();
        }
    }
    public virtual double? inactiveTrackOutlineWidth
    {
        get => _inactiveTrackOutlineWidth;
        set
        {
            var __value = value;
            if (__value == _inactiveTrackOutlineWidth)
            {
                return;
            }
            _inactiveTrackOutlineWidth = __value;
            notifyListeners();
        }
    }
    public virtual Color inactiveTrackColor
    {
        get => _inactiveTrackColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _inactiveTrackColor))
            {
                return;
            }
            _inactiveTrackColor = __value;
            notifyListeners();
        }
    }
    public virtual ImageConfiguration configuration
    {
        get => _configuration!;
        set
        {
            var __value = value;
            if (Equals(__value, _configuration))
            {
                return;
            }
            _configuration = __value;
            notifyListeners();
        }
    }
    public virtual TextDirection textDirection
    {
        get =>
            (
                _textDirection
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        set
        {
            var __value = value;
            if (Equals(_textDirection, (__value)))
            {
                return;
            }
            _textDirection = (__value);
            notifyListeners();
        }
    }
    public virtual Color surfaceColor
    {
        get => _surfaceColor!;
        set
        {
            var __value = value;
            if (Equals(__value, _surfaceColor))
            {
                return;
            }
            _surfaceColor = __value;
            notifyListeners();
        }
    }
    public virtual bool isInteractive
    {
        get =>
            (
                _isInteractive
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        set
        {
            var __value = value;
            if ((__value) == _isInteractive)
            {
                return;
            }
            _isInteractive = (__value);
            notifyListeners();
        }
    }
    public virtual double trackInnerLength
    {
        get =>
            (
                _trackInnerLength
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        set
        {
            var __value = value;
            if ((__value) == _trackInnerLength)
            {
                return;
            }
            _trackInnerLength = (__value);
            notifyListeners();
        }
    }
    public virtual (Color, Color)? onOffLabelColors
    {
        get => _onOffLabelColors;
        set
        {
            var __value = value;
            if (Equals(__value, _onOffLabelColors))
            {
                return;
            }
            _onOffLabelColors = __value;
            notifyListeners();
        }
    }

    internal virtual ShapeDecoration _createDefaultThumbDecoration(
        Color color,
        IImageProvider? image,
        Action<object, System.Diagnostics.StackTrace?>? errorListener
    )
    {
        return new ShapeDecoration(
            color: color,
            image: (image is null)
                ? null
                : new DecorationImage(image: image, onError: errorListener),
            shape: new StadiumBorder()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _handleDecorationChanged()
    {
        if (!_isPainting)
        {
            notifyListeners();
        }
    }

    public override void paint(Canvas canvas, Size size)
    {
        double currentValue = position.value;
        double visualPosition = textDirection switch
        {
            TextDirection.rtl => 1.0 - currentValue,
            TextDirection.ltr => currentValue,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        if (Equals(reaction.status, AnimationStatus.reverse) && !_stopPressAnimation)
        {
            _stopPressAnimation = true;
        }
        else
        {
            _stopPressAnimation = false;
        }
        _pressedThumbExtension = reaction.value * SwitchLibrary._kThumbExtensionFactor;
        var thumbSize = new Size(
            (SwitchLibrary._kThumbRadius * 2L)
                + (
                    _pressedThumbExtension
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
            SwitchLibrary._kThumbRadius * 2L
        );
        double colorValue = _colorAnimation!.value;
        Color trackColor = Dart_uiLibrary.Color.lerp(
            inactiveTrackColor,
            activeTrackColor,
            position.value
        )!;
        Color? trackOutlineColor =
            ((inactiveTrackOutlineColor is null) || (activeTrackOutlineColor is null))
                ? null
                : Dart_uiLibrary.Color.lerp(
                    inactiveTrackOutlineColor,
                    activeTrackOutlineColor,
                    colorValue
                );
        double? trackOutlineWidth = Dart_uiLibrary.lerpDouble(
            inactiveTrackOutlineWidth,
            activeTrackOutlineWidth,
            colorValue
        );
        Color lerpedThumbColor = default!;
        if (!reaction.isDismissed)
        {
            lerpedThumbColor = Dart_uiLibrary.Color.lerp(
                inactivePressedColor,
                activePressedColor,
                colorValue
            )!;
        }
        else
        {
            if (Equals(positionController.status, AnimationStatus.forward))
            {
                lerpedThumbColor = Dart_uiLibrary.Color.lerp(
                    inactivePressedColor,
                    activeColor,
                    colorValue
                )!;
            }
            else
            {
                if (Equals(positionController.status, AnimationStatus.reverse))
                {
                    lerpedThumbColor = Dart_uiLibrary.Color.lerp(
                        inactiveColor,
                        activePressedColor,
                        colorValue
                    )!;
                }
                else
                {
                    lerpedThumbColor = Dart_uiLibrary.Color.lerp(
                        inactiveColor,
                        activeColor,
                        colorValue
                    )!;
                }
            }
        }
        Color thumbColor = Dart_uiLibrary.Color.alphaBlend(lerpedThumbColor, surfaceColor);
        Icon? thumbIcon = (currentValue < 0.5) ? inactiveIcon : activeIcon;
        IImageProvider? thumbImage = (currentValue < 0.5) ? inactiveThumbImage : activeThumbImage;
        Action<object, System.Diagnostics.StackTrace?>? thumbErrorListener =
            (currentValue < 0.5) ? onInactiveThumbImageError : onActiveThumbImageError;
        var paintLocal = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = trackColor;
                    return __cascade;
                }
            )
        )();
        Offset trackPaintOffset = _computeTrackPaintOffset(size);
        Offset thumbPaintOffset = _computeThumbPaintOffset(
            trackPaintOffset,
            thumbSize,
            visualPosition
        );
        var trackRect = Rect.fromLTWH(
            trackPaintOffset.dx,
            trackPaintOffset.dy,
            SwitchLibrary._kTrackWidth,
            SwitchLibrary._kTrackHeight
        );
        _paintTrackWith(
            canvas,
            paintLocal,
            trackPaintOffset,
            trackOutlineColor,
            trackOutlineWidth,
            trackRect
        );
        double currentReactionValue = reaction.value;
        if (_onOffLabelColors is not null)
        {
            var (onLabelColor, offLabelColor) = (
                onOffLabelColors
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            double leftLabelOpacity = visualPosition * (1.0 - currentReactionValue);
            double rightLabelOpacity = (1.0 - visualPosition) * (1.0 - currentReactionValue);
            var (onLabelOpacity, offLabelOpacity) = textDirection switch
            {
                TextDirection.ltr => (leftLabelOpacity, rightLabelOpacity),
                TextDirection.rtl => (rightLabelOpacity, leftLabelOpacity),
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
            var (onLabelOffset, offLabelOffset) = textDirection switch
            {
                TextDirection.ltr => (
                    trackRect.centerLeft.translate(SwitchLibrary._kOnLabelPaddingHorizontal, 0),
                    trackRect.centerRight.translate(-SwitchLibrary._kOffLabelPaddingHorizontal, 0)
                ),
                TextDirection.rtl => (
                    trackRect.centerRight.translate(-SwitchLibrary._kOnLabelPaddingHorizontal, 0),
                    trackRect.centerLeft.translate(SwitchLibrary._kOffLabelPaddingHorizontal, 0)
                ),
                _ => throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
            };
            var onLabelRect = Rect.fromCenter(
                center: onLabelOffset,
                width: SwitchLibrary._kOnLabelWidth,
                height: SwitchLibrary._kOnLabelHeight
            );
            var onLabelPaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = onLabelColor.withOpacity(onLabelOpacity);
                        __cascade.style = PaintingStyle.fill;
                        return __cascade;
                    }
                )
            )();
            canvas.drawRect(onLabelRect, onLabelPaint);
            var offLabelPaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.color = offLabelColor.withOpacity(offLabelOpacity);
                        __cascade.style = PaintingStyle.stroke;
                        __cascade.strokeWidth = SwitchLibrary._kOffLabelWidth;
                        return __cascade;
                    }
                )
            )();
            canvas.drawCircle(offLabelOffset, SwitchLibrary._kOffLabelRadius, offLabelPaint);
        }
        _paintThumbWith(
            thumbPaintOffset,
            canvas,
            colorValue,
            thumbColor,
            thumbImage,
            thumbErrorListener,
            thumbIcon,
            thumbSize
        );
    }

    internal static Offset _computeTrackPaintOffset(Size canvasSize)
    {
        double horizontalOffset = (canvasSize.width - SwitchLibrary._kTrackWidth) / 2.0;
        double verticalOffset = (canvasSize.height - SwitchLibrary._kTrackHeight) / 2.0;
        return new Offset(horizontalOffset, verticalOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Offset _computeThumbPaintOffset(
        Offset trackPaintOffset,
        Size thumbSize,
        double visualPosition
    )
    {
        double trackRadius = SwitchLibrary._kTrackHeight / 2L;
        double additionalThumbRadius = (thumbSize.height / 2L) - trackRadius;
        double horizontalProgress =
            visualPosition
            * (
                trackInnerLength
                - (
                    _pressedThumbExtension
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                )
            );
        double thumbHorizontalOffset =
            trackPaintOffset.dx
            + trackRadius
            + (
                (
                    _pressedThumbExtension
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ) / 2L
            )
            - (thumbSize.width / 2L)
            + horizontalProgress;
        double thumbVerticalOffset = trackPaintOffset.dy - additionalThumbRadius;
        return new Offset(thumbHorizontalOffset, thumbVerticalOffset);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual void _paintTrackWith(
        Canvas canvas,
        Paint paint,
        Offset trackPaintOffset,
        Color? trackOutlineColor,
        double? trackOutlineWidth,
        Rect trackRect
    )
    {
        double trackRadius = SwitchLibrary._kTrackHeight / 2L;
        var trackRRect = RRect.fromRectAndRadius(trackRect, Radius.circular(trackRadius));
        canvas.drawRRect(trackRRect, paint);
        if (trackOutlineColor is not null)
        {
            var outlineTrackRect = Rect.fromLTWH(
                trackPaintOffset.dx + 1L,
                trackPaintOffset.dy + 1L,
                SwitchLibrary._kTrackWidth - 2L,
                SwitchLibrary._kTrackHeight - 2L
            );
            var outlineTrackRRect = RRect.fromRectAndRadius(
                outlineTrackRect,
                Radius.circular(trackRadius)
            );
            var outlinePaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.style = PaintingStyle.stroke;
                        __cascade.strokeWidth = trackOutlineWidth ?? 2.0;
                        __cascade.color = trackOutlineColor;
                        return __cascade;
                    }
                )
            )();
            canvas.drawRRect(outlineTrackRRect, outlinePaint);
        }
        if (isFocused)
        {
            RRect focusedOutline = trackRRect.inflate(1.75);
            var focusedPaint = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.style = PaintingStyle.stroke;
                        __cascade.color = focusColor;
                        __cascade.strokeWidth = 3.5;
                        return __cascade;
                    }
                )
            )();
            canvas.drawRRect(focusedOutline, focusedPaint);
        }
        canvas.clipRRect(trackRRect);
    }

    internal virtual void _paintThumbWith(
        Offset thumbPaintOffset,
        Canvas canvas,
        double currentValue,
        Color thumbColor,
        IImageProvider? thumbImage,
        Action<object, System.Diagnostics.StackTrace?>? thumbErrorListener,
        Icon? thumbIcon,
        Size thumbSize
    )
    {
        try
        {
            _isPainting = true;
            if (
                (_cachedThumbPainter is null)
                || (!Equals(thumbColor, _cachedThumbColor))
                || (!Equals(thumbImage, _cachedThumbImage))
                || (!Equals(thumbErrorListener, _cachedThumbErrorListener))
            )
            {
                _cachedThumbColor = thumbColor;
                _cachedThumbImage = thumbImage;
                _cachedThumbErrorListener = thumbErrorListener;
                _cachedThumbPainter?.dispose();
                _cachedThumbPainter = _createDefaultThumbDecoration(
                        thumbColor,
                        thumbImage,
                        thumbErrorListener
                    )
                    .createBoxPainter(() => _handleDecorationChanged());
            }
            BoxPainter thumbPainter = _cachedThumbPainter!;
            _paintCupertinoThumbShadowAndBorder(canvas, thumbPaintOffset, thumbSize);
            thumbPainter.paint(canvas, thumbPaintOffset, configuration.copyWith(size: thumbSize));
            if ((thumbIcon is not null) && (thumbIcon.icon is not null))
            {
                Color iconColor = Dart_uiLibrary.Color.lerp(
                    inactiveIconColor,
                    activeIconColor,
                    currentValue
                )!;
                double iconSize = thumbIcon.size ?? 16.0;
                IconData iconData = thumbIcon.icon!;
                double? iconWeight = thumbIcon.weight ?? iconTheme?.weight;
                double? iconFill = thumbIcon.fill ?? iconTheme?.fill;
                double? iconGrade = thumbIcon.grade ?? iconTheme?.grade;
                double? iconOpticalSize = thumbIcon.opticalSize ?? iconTheme?.opticalSize;
                List<Shadow>? iconShadows = (thumbIcon.shadows ?? iconTheme?.shadows)?.ToList();
                var textSpan = new TextSpan(
                    text: char.ConvertFromUtf32(checked((int)iconData.codePoint)),
                    style: new TextStyle(
                        fontVariations: (
                            (Func<List<FontVariation>>)(
                                () =>
                                {
                                    var __collection45150 = new List<FontVariation>();
                                    if (iconFill is not null)
                                    {
                                        __collection45150.Add(
                                            new FontVariation(
                                                "FILL",
                                                (
                                                    iconFill
                                                    ?? throw new global::System.NullReferenceException(
                                                        "A required value was null."
                                                    )
                                                )
                                            )
                                        );
                                    }
                                    if (iconWeight is not null)
                                    {
                                        __collection45150.Add(
                                            new FontVariation(
                                                "wght",
                                                (
                                                    iconWeight
                                                    ?? throw new global::System.NullReferenceException(
                                                        "A required value was null."
                                                    )
                                                )
                                            )
                                        );
                                    }
                                    if (iconGrade is not null)
                                    {
                                        __collection45150.Add(
                                            new FontVariation(
                                                "GRAD",
                                                (
                                                    iconGrade
                                                    ?? throw new global::System.NullReferenceException(
                                                        "A required value was null."
                                                    )
                                                )
                                            )
                                        );
                                    }
                                    if (iconOpticalSize is not null)
                                    {
                                        __collection45150.Add(
                                            new FontVariation(
                                                "opsz",
                                                (
                                                    iconOpticalSize
                                                    ?? throw new global::System.NullReferenceException(
                                                        "A required value was null."
                                                    )
                                                )
                                            )
                                        );
                                    }
                                    return __collection45150;
                                }
                            )
                        )(),
                        color: iconColor,
                        fontSize: iconSize,
                        inherit: false,
                        fontFamily: iconData.fontFamily,
                        package: iconData.fontPackage,
                        shadows: iconShadows
                    )
                );
                DartRuntimePrimitives.Ignore(
                    (
                        (Func<TextPainter>)(
                            () =>
                            {
                                var __cascade = _textPainter;
                                __cascade.textDirection = textDirection;
                                __cascade.text = textSpan;
                                return __cascade;
                            }
                        )
                    )()
                );
                _textPainter.layout();
                double additionalHorizontalOffset = (thumbSize.width - iconSize) / 2L;
                double additionalVerticalOffset = (thumbSize.height - iconSize) / 2L;
                Offset offset =
                    thumbPaintOffset
                    + new Offset(additionalHorizontalOffset, additionalVerticalOffset);
                _textPainter.paint(canvas, offset);
            }
        }
        finally
        {
            _isPainting = false;
        }
    }

    internal virtual void _paintCupertinoThumbShadowAndBorder(
        Canvas canvas,
        Offset thumbPaintOffset,
        Size thumbSize
    )
    {
        var thumbBounds = RRect.fromLTRBR(
            thumbPaintOffset.dx,
            thumbPaintOffset.dy,
            thumbPaintOffset.dx + thumbSize.width,
            thumbPaintOffset.dy + thumbSize.height,
            Radius.circular(thumbSize.height / 2.0)
        );
        foreach (BoxShadow shadow in SwitchLibrary._kSwitchBoxShadows)
        {
            canvas.drawRRect(thumbBounds.shift(shadow.offset), shadow.toPaint());
        }
        canvas.drawRRect(
            thumbBounds.inflate(0.5),
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
    }

    public override void dispose()
    {
        _textPainter.dispose();
        _cachedThumbPainter?.dispose();
        _cachedThumbPainter = null;
        _cachedThumbColor = null;
        _cachedThumbImage = null;
        _cachedThumbErrorListener = null;
        _colorAnimation?.dispose();
        base.dispose();
    }
}
