// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/chip.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class ChipLibrary
{
    internal static double _kChipHeight = 32.0;
}

public static partial class ChipLibrary
{
    internal static long _kCheckmarkAlpha = 222L;
}

public static partial class ChipLibrary
{
    internal static long _kDisabledAlpha = 97L;
}

public static partial class ChipLibrary
{
    internal static double _kCheckmarkStrokeWidth = 2.0;
}

public static partial class ChipLibrary
{
    internal static Duration _kSelectDuration = Duration.Create(milliseconds: 195L);
}

public static partial class ChipLibrary
{
    internal static Duration _kCheckmarkDuration = Duration.Create(milliseconds: 150L);
}

public static partial class ChipLibrary
{
    internal static Duration _kCheckmarkReverseDuration = Duration.Create(milliseconds: 50L);
}

public static partial class ChipLibrary
{
    internal static Duration _kDrawerDuration = Duration.Create(milliseconds: 150L);
}

public static partial class ChipLibrary
{
    internal static Duration _kReverseDrawerDuration = Duration.Create(milliseconds: 100L);
}

public static partial class ChipLibrary
{
    internal static Duration _kDisableDuration = Duration.Create(milliseconds: 75L);
}

public static partial class ChipLibrary
{
    internal static Color _kSelectScrimColor = new Color(1612257561L);
}

public static partial class ChipLibrary
{
    internal static Icon _kDefaultDeleteIcon = new Icon(Icons.cancel);
}

public interface ChipAttributes
{
    public Widget label { get; }
    public Widget? avatar { get; }
    public TextStyle? labelStyle { get; }
    public BorderSide? side { get; }
    public OutlinedBorder? shape { get; }
    public Clip clipBehavior { get; }
    public FocusNode? focusNode { get; }
    public bool autofocus { get; }
    public WidgetStateProperty<Color?>? color { get; }
    public Color? backgroundColor { get; }
    public EdgeInsetsGeometry? padding { get; }
    public VisualDensity? visualDensity { get; }
    public EdgeInsetsGeometry? labelPadding { get; }
    public MaterialTapTargetSize? materialTapTargetSize { get; }
    public double? elevation { get; }
    public Color? shadowColor { get; }
    public Color? surfaceTintColor { get; }
    public IconThemeData? iconTheme { get; }
    public BoxConstraints? avatarBoxConstraints { get; }
    public ChipAnimationStyle? chipAnimationStyle { get; }
    public MouseCursor? mouseCursor { get; }
}

public interface DeletableChipAttributes
{
    public Widget? deleteIcon { get; }
    public Action? onDeleted { get; }
    public Color? deleteIconColor { get; }
    public string? deleteButtonTooltipMessage { get; }
    public BoxConstraints? deleteIconBoxConstraints { get; }
}

public interface CheckmarkableChipAttributes
{
    public bool? showCheckmark { get; }
    public Color? checkmarkColor { get; }
}

public interface SelectableChipAttributes
{
    public bool selected { get; }
    public Action<bool>? onSelected { get; }
    public double? pressElevation { get; }
    public Color? selectedColor { get; }
    public Color? selectedShadowColor { get; }
    public string? tooltip { get; }
    public ShapeBorder avatarBorder { get; }
}

public interface DisabledChipAttributes
{
    public bool isEnabled { get; }
    public Color? disabledColor { get; }
}

public interface TappableChipAttributes
{
    public Action? onPressed { get; }
    public double? pressElevation { get; }
    public string? tooltip { get; }
}

public class ChipAnimationStyle
{
    public virtual AnimationStyle? enableAnimation { get; private set; }
    public virtual AnimationStyle? selectAnimation { get; private set; }
    public virtual AnimationStyle? avatarDrawerAnimation { get; private set; }
    public virtual AnimationStyle? deleteDrawerAnimation { get; private set; }

    public ChipAnimationStyle(
        AnimationStyle? enableAnimation = null,
        AnimationStyle? selectAnimation = null,
        AnimationStyle? avatarDrawerAnimation = null,
        AnimationStyle? deleteDrawerAnimation = null
    )
    {
        this.enableAnimation = enableAnimation;
        this.selectAnimation = selectAnimation;
        this.avatarDrawerAnimation = avatarDrawerAnimation;
        this.deleteDrawerAnimation = deleteDrawerAnimation;
    }
}

public class Chip : StatelessWidget, ChipAttributes, DeletableChipAttributes
{
    public virtual Widget? avatar { get; private set; }
    public virtual Widget label { get; private set; } = default!;
    public virtual TextStyle? labelStyle { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual BorderSide? side { get; private set; }
    public virtual OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual Widget? deleteIcon { get; private set; }
    public virtual Action? onDeleted { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual string? deleteButtonTooltipMessage { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual BoxConstraints? deleteIconBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }

    public Chip(
        Key? key = null,
        Widget? avatar = null,
        Widget label = default!,
        TextStyle? labelStyle = null,
        EdgeInsetsGeometry? labelPadding = null,
        Widget? deleteIcon = null,
        Action? onDeleted = null,
        Color? deleteIconColor = null,
        string? deleteButtonTooltipMessage = null,
        BorderSide? side = null,
        OutlinedBorder? shape = null,
        Clip clipBehavior = Clip.none,
        FocusNode? focusNode = null,
        bool autofocus = false,
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        EdgeInsetsGeometry? padding = null,
        VisualDensity? visualDensity = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        IconThemeData? iconTheme = null,
        BoxConstraints? avatarBoxConstraints = null,
        BoxConstraints? deleteIconBoxConstraints = null,
        ChipAnimationStyle? chipAnimationStyle = null,
        MouseCursor? mouseCursor = null
    )
        : base(key: key)
    {
        this.avatar = avatar;
        this.label = label;
        this.labelStyle = labelStyle;
        this.labelPadding = labelPadding;
        this.deleteIcon = deleteIcon;
        this.onDeleted = onDeleted;
        this.deleteIconColor = deleteIconColor;
        this.deleteButtonTooltipMessage = deleteButtonTooltipMessage;
        this.side = side;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.padding = padding;
        this.visualDensity = visualDensity;
        this.materialTapTargetSize = materialTapTargetSize;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.iconTheme = iconTheme;
        this.avatarBoxConstraints = avatarBoxConstraints;
        this.deleteIconBoxConstraints = deleteIconBoxConstraints;
        this.chipAnimationStyle = chipAnimationStyle;
        this.mouseCursor = mouseCursor;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        return new RawChip(
            avatar: avatar,
            label: label,
            labelStyle: labelStyle,
            labelPadding: labelPadding,
            deleteIcon: deleteIcon,
            onDeleted: onDeleted,
            deleteIconColor: deleteIconColor,
            deleteButtonTooltipMessage: deleteButtonTooltipMessage,
            tapEnabled: false,
            side: side,
            shape: shape,
            clipBehavior: clipBehavior,
            focusNode: focusNode,
            autofocus: autofocus,
            color: color,
            backgroundColor: backgroundColor,
            padding: padding,
            visualDensity: visualDensity,
            materialTapTargetSize: materialTapTargetSize,
            elevation: elevation,
            shadowColor: shadowColor,
            surfaceTintColor: surfaceTintColor,
            iconTheme: iconTheme,
            avatarBoxConstraints: avatarBoxConstraints,
            deleteIconBoxConstraints: deleteIconBoxConstraints,
            chipAnimationStyle: chipAnimationStyle,
            mouseCursor: mouseCursor
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class RawChip
    : StatefulWidget,
        ChipAttributes,
        DeletableChipAttributes,
        SelectableChipAttributes,
        CheckmarkableChipAttributes,
        DisabledChipAttributes,
        TappableChipAttributes
{
    public virtual ChipThemeData? defaultProperties { get; private set; }
    public virtual Widget? avatar { get; private set; }
    public virtual Widget label { get; private set; } = default!;
    public virtual TextStyle? labelStyle { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual Widget deleteIcon { get; private set; }
    public virtual Action? onDeleted { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual string? deleteButtonTooltipMessage { get; private set; }
    public virtual Action<bool>? onSelected { get; private set; }
    public virtual Action? onPressed { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual bool isEnabled { get; private set; } = default!;
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual BorderSide? side { get; private set; }
    public virtual OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual Color? selectedShadowColor { get; private set; }
    public virtual bool? showCheckmark { get; private set; }
    public virtual Color? checkmarkColor { get; private set; }
    public virtual ShapeBorder avatarBorder { get; private set; } = default!;
    public virtual BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual BoxConstraints? deleteIconBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual MouseCursor? mouseCursor { get; private set; }
    public virtual bool tapEnabled { get; private set; } = default!;

    public RawChip(
        Key? key = null,
        ChipThemeData? defaultProperties = null,
        Widget? avatar = null,
        Widget label = default!,
        TextStyle? labelStyle = null,
        EdgeInsetsGeometry? padding = null,
        VisualDensity? visualDensity = null,
        EdgeInsetsGeometry? labelPadding = null,
        Widget? deleteIcon = null,
        Action? onDeleted = null,
        Color? deleteIconColor = null,
        string? deleteButtonTooltipMessage = null,
        Action? onPressed = null,
        Action<bool>? onSelected = null,
        double? pressElevation = null,
        bool tapEnabled = true,
        bool selected = false,
        bool isEnabled = true,
        Color? disabledColor = null,
        Color? selectedColor = null,
        string? tooltip = null,
        BorderSide? side = null,
        OutlinedBorder? shape = null,
        Clip clipBehavior = Clip.none,
        FocusNode? focusNode = null,
        bool autofocus = false,
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        MaterialTapTargetSize? materialTapTargetSize = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        IconThemeData? iconTheme = null,
        Color? selectedShadowColor = null,
        bool? showCheckmark = null,
        Color? checkmarkColor = null,
        ShapeBorder avatarBorder = default!,
        BoxConstraints? avatarBoxConstraints = null,
        BoxConstraints? deleteIconBoxConstraints = null,
        ChipAnimationStyle? chipAnimationStyle = null,
        MouseCursor? mouseCursor = null
    )
        : base(key: key)
    {
        ShapeBorder __avatarBorder = avatarBorder ?? new CircleBorder();
        this.defaultProperties = defaultProperties;
        this.avatar = avatar;
        this.label = label;
        this.labelStyle = labelStyle;
        this.padding = padding;
        this.visualDensity = visualDensity;
        this.labelPadding = labelPadding;
        this.onDeleted = onDeleted;
        this.deleteIconColor = deleteIconColor;
        this.deleteButtonTooltipMessage = deleteButtonTooltipMessage;
        this.onPressed = onPressed;
        this.onSelected = onSelected;
        this.pressElevation = pressElevation;
        this.tapEnabled = tapEnabled;
        this.selected = selected;
        this.isEnabled = isEnabled;
        this.disabledColor = disabledColor;
        this.selectedColor = selectedColor;
        this.tooltip = tooltip;
        this.side = side;
        this.shape = shape;
        this.clipBehavior = clipBehavior;
        this.focusNode = focusNode;
        this.autofocus = autofocus;
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.materialTapTargetSize = materialTapTargetSize;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.iconTheme = iconTheme;
        this.selectedShadowColor = selectedShadowColor;
        this.showCheckmark = showCheckmark;
        this.checkmarkColor = checkmarkColor;
        this.avatarBorder = __avatarBorder;
        this.avatarBoxConstraints = avatarBoxConstraints;
        this.deleteIconBoxConstraints = deleteIconBoxConstraints;
        this.chipAnimationStyle = chipAnimationStyle;
        this.mouseCursor = mouseCursor;
        this.deleteIcon = deleteIcon ?? ChipLibrary._kDefaultDeleteIcon;
        System.Diagnostics.Debug.Assert((pressElevation is null) || (pressElevation >= 0.0));
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _RawChipState__chip());
}

internal class _RawChipState__chip : State<RawChip>, TickerProviderStateMixin<RawChip>
{
    public static Duration pressedAnimationDuration = Duration.Create(milliseconds: 75L);
    public virtual AnimationController selectController { get; set; } = default!;
    public virtual AnimationController avatarDrawerController { get; set; } = default!;
    public virtual AnimationController deleteDrawerController { get; set; } = default!;
    public virtual AnimationController enableController { get; set; } = default!;
    public virtual CurvedAnimation checkmarkAnimation { get; set; } = default!;
    public virtual CurvedAnimation avatarDrawerAnimation { get; set; } = default!;
    public virtual CurvedAnimation deleteDrawerAnimation { get; set; } = default!;
    public virtual CurvedAnimation enableAnimation { get; set; } = default!;
    public virtual CurvedAnimation selectionFade { get; set; } = default!;
    public virtual WidgetStatesController statesController { get; private set; } =
        new WidgetStatesController();
    internal virtual bool _isTapping { get; set; } = false;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual bool hasDeleteButton =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.onDeleted is not null);
    public virtual bool hasAvatar =>
        DartRuntimePrimitives.ConvertValue<bool>(widget.avatar is not null);
    public virtual bool canTap
    {
        get
        {
            return widget.isEnabled
                && widget.tapEnabled
                && ((widget.onPressed is not null) || (widget.onSelected is not null));
        }
    }
    public virtual bool isTapping => DartRuntimePrimitives.ConvertValue<bool>(canTap && _isTapping);

    public override void initState()
    {
        DartRuntimePrimitives.Assert(() =>
            (widget.onSelected is null) || (widget.onPressed is null)
        );
        base.initState();
        DartRuntimePrimitives.Ignore(
            (
                (Func<WidgetStatesController>)(
                    () =>
                    {
                        var __cascade = statesController;
                        __cascade.update(WidgetState.disabled, !widget.isEnabled);
                        __cascade.update(WidgetState.selected, widget.selected);
                        __cascade.addListener(() =>
                        {
                            setState(() => { });
                        });
                        return __cascade;
                    }
                )
            )()
        );
        selectController = new AnimationController(
            duration: widget.chipAnimationStyle?.selectAnimation?.duration
                ?? ChipLibrary._kSelectDuration,
            reverseDuration: widget.chipAnimationStyle?.selectAnimation?.reverseDuration,
            value: widget.selected ? 1.0 : 0.0,
            vsync: this
        );
        selectionFade = new CurvedAnimation(parent: selectController, curve: Curves.fastOutSlowIn);
        avatarDrawerController = new AnimationController(
            duration: widget.chipAnimationStyle?.avatarDrawerAnimation?.duration
                ?? ChipLibrary._kDrawerDuration,
            reverseDuration: widget.chipAnimationStyle?.avatarDrawerAnimation?.reverseDuration,
            value: (hasAvatar || widget.selected) ? 1.0 : 0.0,
            vsync: this
        );
        deleteDrawerController = new AnimationController(
            duration: widget.chipAnimationStyle?.deleteDrawerAnimation?.duration
                ?? ChipLibrary._kDrawerDuration,
            reverseDuration: widget.chipAnimationStyle?.deleteDrawerAnimation?.reverseDuration,
            value: hasDeleteButton ? 1.0 : 0.0,
            vsync: this
        );
        enableController = new AnimationController(
            duration: widget.chipAnimationStyle?.enableAnimation?.duration
                ?? ChipLibrary._kDisableDuration,
            reverseDuration: widget.chipAnimationStyle?.enableAnimation?.reverseDuration,
            value: widget.isEnabled ? 1.0 : 0.0,
            vsync: this
        );
        double checkmarkPercentage =
            ChipLibrary._kCheckmarkDuration.inMilliseconds
            / ChipLibrary._kSelectDuration.inMilliseconds;
        double checkmarkReversePercentage =
            ChipLibrary._kCheckmarkReverseDuration.inMilliseconds
            / ChipLibrary._kSelectDuration.inMilliseconds;
        double avatarDrawerReversePercentage =
            ChipLibrary._kReverseDrawerDuration.inMilliseconds
            / ChipLibrary._kSelectDuration.inMilliseconds;
        checkmarkAnimation = new CurvedAnimation(
            parent: selectController,
            curve: new Interval(1.0 - checkmarkPercentage, 1.0, curve: Curves.fastOutSlowIn),
            reverseCurve: new Interval(
                1.0 - checkmarkReversePercentage,
                1.0,
                curve: Curves.fastOutSlowIn
            )
        );
        deleteDrawerAnimation = new CurvedAnimation(
            parent: deleteDrawerController,
            curve: Curves.fastOutSlowIn
        );
        avatarDrawerAnimation = new CurvedAnimation(
            parent: avatarDrawerController,
            curve: Curves.fastOutSlowIn,
            reverseCurve: new Interval(
                1.0 - avatarDrawerReversePercentage,
                1.0,
                curve: Curves.fastOutSlowIn
            )
        );
        enableAnimation = new CurvedAnimation(
            parent: enableController,
            curve: Curves.fastOutSlowIn
        );
    }

    public override void dispose()
    {
        selectController.dispose();
        avatarDrawerController.dispose();
        deleteDrawerController.dispose();
        enableController.dispose();
        checkmarkAnimation.dispose();
        avatarDrawerAnimation.dispose();
        deleteDrawerAnimation.dispose();
        enableAnimation.dispose();
        selectionFade.dispose();
        statesController.dispose();
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

    internal virtual void _handleTapDown(Gestures.TapDownDetails details)
    {
        if (!canTap)
        {
            return;
        }
        statesController.update(WidgetState.pressed, true);
        setState(() =>
        {
            _isTapping = true;
        });
    }

    internal virtual void _handleTapCancel()
    {
        if (!canTap)
        {
            return;
        }
        statesController.update(WidgetState.pressed, false);
        setState(() =>
        {
            _isTapping = false;
        });
    }

    internal virtual void _handleTap()
    {
        if (!canTap)
        {
            return;
        }
        statesController.update(WidgetState.pressed, false);
        setState(() =>
        {
            _isTapping = false;
        });
        widget.onSelected?.Invoke(!widget.selected);
        widget.onPressed?.Invoke();
    }

    internal virtual OutlinedBorder _getShape(
        ThemeData theme,
        ChipThemeData chipTheme,
        ChipThemeData chipDefaults
    )
    {
        BorderSide? resolvedSide =
            WidgetStateProperty.resolveAs(widget.side, statesController.value)
            ?? WidgetStateProperty.resolveAs(chipTheme.side, statesController.value);
        OutlinedBorder resolvedShape =
            (
                (
                    WidgetStateProperty.resolveAs(widget.shape, statesController.value)
                    ?? WidgetStateProperty.resolveAs(chipTheme.shape, statesController.value)
                ) ?? WidgetStateProperty.resolveAs(chipDefaults.shape, statesController.value)
            ) ?? new StadiumBorder();
        if (resolvedSide is not null)
        {
            return resolvedShape.copyWith(side: resolvedSide);
        }
        return (!Equals(resolvedShape.side, BorderSide.none))
            ? resolvedShape
            : resolvedShape.copyWith(side: chipDefaults.side);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Color? resolveColor(
        WidgetStateProperty<Color?>? color = null,
        Color? selectedColor = null,
        Color? backgroundColor = null,
        Color? disabledColor = null,
        WidgetStateProperty<Color?>? defaultColor = null
    )
    {
        return new _IndividualOverrides__chip(
                color: color,
                selectedColor: selectedColor,
                backgroundColor: backgroundColor,
                disabledColor: disabledColor
            ).resolve(statesController.value) ?? (defaultColor?.resolve(statesController.value));
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Color? _getBackgroundColor(
        ThemeData theme,
        ChipThemeData chipTheme,
        ChipThemeData chipDefaults
    )
    {
        {
            Color? disabledColorLocal = resolveColor(
                color: widget.color ?? chipTheme.color,
                disabledColor: widget.disabledColor ?? chipTheme.disabledColor,
                defaultColor: chipDefaults.color
            );
            Color? backgroundColorLocal = resolveColor(
                color: widget.color ?? chipTheme.color,
                backgroundColor: widget.backgroundColor ?? chipTheme.backgroundColor,
                defaultColor: chipDefaults.color
            );
            Color? selectedColorLocal = resolveColor(
                color: widget.color ?? chipTheme.color,
                selectedColor: widget.selectedColor ?? chipTheme.selectedColor,
                defaultColor: chipDefaults.color
            );
            var backgroundTween = new ColorTween(
                begin: disabledColorLocal,
                end: backgroundColorLocal
            );
            var selectTween = new ColorTween(
                begin: backgroundTween.evaluate(enableController),
                end: selectedColorLocal
            );
            return selectTween.evaluate(selectionFade);
        }
    }

    public override void didUpdateWidget(RawChip oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.isEnabled != widget.isEnabled)
        {
            setState(() =>
            {
                statesController.update(WidgetState.disabled, !widget.isEnabled);
                if (widget.isEnabled)
                {
                    enableController.forward();
                }
                else
                {
                    enableController.reverse();
                }
            });
        }
        if ((!Equals(oldWidget.avatar, widget.avatar)) || (oldWidget.selected != widget.selected))
        {
            setState(() =>
            {
                if (hasAvatar || widget.selected)
                {
                    avatarDrawerController.forward();
                }
                else
                {
                    avatarDrawerController.reverse();
                }
            });
        }
        if (oldWidget.selected != widget.selected)
        {
            setState(() =>
            {
                statesController.update(WidgetState.selected, widget.selected);
                if (widget.selected)
                {
                    selectController.forward();
                }
                else
                {
                    selectController.reverse();
                }
            });
        }
        if (!Equals(oldWidget.onDeleted, widget.onDeleted))
        {
            setState(() =>
            {
                if (hasDeleteButton)
                {
                    deleteDrawerController.forward();
                }
                else
                {
                    deleteDrawerController.reverse();
                }
            });
        }
    }

    internal virtual Widget? _wrapWithTooltip(
        string? tooltip = null,
        bool enabled = true,
        Widget? child = null
    )
    {
        if ((child is null) || !enabled || (tooltip is null))
        {
            return child;
        }
        return (Widget?)new Tooltip(message: tooltip, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Widget? _buildDeleteIcon(
        BuildContext context,
        ThemeData theme,
        ChipThemeData chipTheme,
        ChipThemeData chipDefaults
    )
    {
        if (!hasDeleteButton)
        {
            return null;
        }
        IconThemeData iconThemeLocal =
            ((widget.iconTheme ?? chipTheme.iconTheme) ?? theme.chipTheme.iconTheme)
            ?? new _ChipDefaultsM3__chip(context, widget.isEnabled).iconTheme!;
        Color? effectiveDeleteIconColor = WidgetStateProperty.resolveAs(
            (
                (
                    (
                        (widget.deleteIconColor ?? chipTheme.deleteIconColor)
                        ?? theme.chipTheme.deleteIconColor
                    ) ?? widget.iconTheme?.color
                ) ?? chipTheme.iconTheme?.color
            ) ?? chipDefaults.deleteIconColor,
            statesController.value
        );
        double effectiveIconSize =
            (
                (widget.iconTheme?.size ?? chipTheme.iconTheme?.size)
                ?? theme.chipTheme.iconTheme?.size
            )
            ?? (
                new _ChipDefaultsM3__chip(context, widget.isEnabled).iconTheme!.size
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        MaterialTapTargetSize effectiveMaterialTapTargetSize =
            widget.materialTapTargetSize ?? theme.materialTapTargetSize;
        Size semanticSizeLocal = effectiveMaterialTapTargetSize switch
        {
            var __constant45576 when Equals(__constant45576, MaterialTapTargetSize.padded) =>
                new Size(ConstantsLibrary.kMinInteractiveDimension),
            var __constant45659 when Equals(__constant45659, MaterialTapTargetSize.shrinkWrap) =>
                new Size(ConstantsLibrary.kMinInteractiveDimension - 8.0),
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        VisualDensity effectiveVisualDensity = widget.visualDensity ?? theme.visualDensity;
        return (Widget?)
            new _EnsureMinSemanticsSize__chip(
                semanticSize: semanticSizeLocal + effectiveVisualDensity.baseSizeAdjustment,
                child: _wrapWithTooltip(
                    tooltip: widget.deleteButtonTooltipMessage
                        ?? MaterialLocalizations.of(context).deleteButtonTooltip,
                    enabled: widget.isEnabled && (widget.onDeleted is not null),
                    child: new InkWell(
                        radius: (ChipLibrary._kChipHeight + (widget.padding?.vertical ?? 0.0))
                            * 0.45,
                        splashFactory: new _UnconstrainedInkSplashFactory__chip(
                            Theme.of(context).splashFactory
                        ),
                        customBorder: new CircleBorder(),
                        onTap: widget.isEnabled ? widget.onDeleted : null,
                        child: new IconTheme(
                            data: iconThemeLocal.copyWith(
                                color: effectiveDeleteIconColor,
                                size: effectiveIconSize
                            ),
                            child: widget.deleteIcon
                        )
                    )
                )
            );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() =>
            Widgets.DebugLibrary.debugCheckHasDirectionality(context)
        );
        DartRuntimePrimitives.Assert(() =>
            DebugLibrary.debugCheckHasMaterialLocalizations(context)
        );
        ThemeData themeLocal = Theme.of(context);
        ChipThemeData chipTheme = ChipTheme.of(context);
        Brightness brightnessLocal = chipTheme.brightness ?? themeLocal.brightness;
        ChipThemeData chipDefaults =
            widget.defaultProperties ?? new _ChipDefaultsM3__chip(context, widget.isEnabled);
        TextDirection? textDirection = Directionality.maybeOf(context);
        OutlinedBorder resolvedShape = _getShape(themeLocal, chipTheme, chipDefaults);
        double elevationLocal =
            ((widget.elevation ?? chipTheme.elevation) ?? chipDefaults.elevation) ?? 0;
        double pressElevationLocal =
            ((widget.pressElevation ?? chipTheme.pressElevation) ?? chipDefaults.pressElevation)
            ?? 0;
        Color? shadowColorLocal =
            (widget.shadowColor ?? chipTheme.shadowColor) ?? chipDefaults.shadowColor;
        Color? surfaceTintColorLocal =
            (widget.surfaceTintColor ?? chipTheme.surfaceTintColor)
            ?? chipDefaults.surfaceTintColor;
        Color? selectedShadowColorLocal =
            (widget.selectedShadowColor ?? chipTheme.selectedShadowColor)
            ?? chipDefaults.selectedShadowColor;
        Color? checkmarkColorLocal =
            (widget.checkmarkColor ?? chipTheme.checkmarkColor) ?? chipDefaults.checkmarkColor;
        bool showCheckmarkLocal =
            (widget.showCheckmark ?? chipTheme.showCheckmark)
            ?? (
                chipDefaults.showCheckmark
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
        EdgeInsetsGeometry paddingLocal =
            (widget.padding ?? chipTheme.padding) ?? chipDefaults.padding!;
        TextStyle labelStyleLocal = chipTheme.labelStyle ?? chipDefaults.labelStyle!;
        IconThemeData? iconThemeLocal =
            (widget.iconTheme ?? chipTheme.iconTheme) ?? chipDefaults.iconTheme;
        BoxConstraints? avatarBoxConstraintsLocal =
            widget.avatarBoxConstraints ?? chipTheme.avatarBoxConstraints;
        BoxConstraints? deleteIconBoxConstraintsLocal =
            widget.deleteIconBoxConstraints ?? chipTheme.deleteIconBoxConstraints;
        TextStyle effectiveLabelStyle = labelStyleLocal.merge(widget.labelStyle);
        Color? resolvedLabelColor = WidgetStateProperty.resolveAs(
            effectiveLabelStyle.color,
            statesController.value
        );
        TextStyle resolvedLabelStyle = effectiveLabelStyle.copyWith(color: resolvedLabelColor);
        Widget? avatarLocal =
            ((iconThemeLocal is not null) && hasAvatar)
                ? IconTheme.merge(
                    data: chipDefaults.iconTheme!.merge(iconThemeLocal),
                    child: widget.avatar!
                )
                : widget.avatar;
        double defaultFontSize = effectiveLabelStyle.fontSize ?? 14.0;
        double effectiveTextScale = MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0;
        EdgeInsetsGeometry defaultLabelPadding = EdgeInsets.lerp(
            EdgeInsets.CreateSymmetric(horizontal: 8.0),
            EdgeInsets.CreateSymmetric(horizontal: 4.0),
            Dart_uiLibrary.clampDouble(effectiveTextScale - 1.0, 0.0, 1.0)
        )!;
        EdgeInsetsGeometry labelPaddingLocal =
            ((widget.labelPadding ?? chipTheme.labelPadding) ?? chipDefaults.labelPadding)
            ?? defaultLabelPadding;
        Widget result = new Material(
            elevation: isTapping ? pressElevationLocal : elevationLocal,
            shadowColor: widget.selected ? selectedShadowColorLocal : shadowColorLocal,
            surfaceTintColor: surfaceTintColorLocal,
            animationDuration: pressedAnimationDuration,
            shape: resolvedShape,
            clipBehavior: widget.clipBehavior,
            child: new InkWell(
                onFocusChange: (value) =>
                {
                    statesController.update(WidgetState.focused, value);
                },
                focusNode: widget.focusNode,
                autofocus: widget.autofocus,
                canRequestFocus: widget.isEnabled,
                onTap: canTap ? _handleTap : null,
                onTapDown: canTap ? _handleTapDown : null,
                onTapCancel: canTap ? _handleTapCancel : null,
                onHover: canTap
                    ? (
                        (value) =>
                        {
                            statesController.update(WidgetState.hovered, value);
                        }
                    )
                    : null,
                mouseCursor: widget.mouseCursor,
                hoverColor: ((widget.color ?? chipTheme.color) is null) ? null : Colors.transparent,
                customBorder: resolvedShape,
                child: new AnimatedBuilder(
                    animation: Listenable.CreateMerge(
                        new List<Listenable>
                        {
                            selectController,
                            enableController,
                        }.Cast<Listenable?>()
                    ),
                    builder: (context, child) =>
                    {
                        return new Ink(
                            decoration: new ShapeDecoration(
                                shape: resolvedShape,
                                color: _getBackgroundColor(themeLocal, chipTheme, chipDefaults)
                            ),
                            child: child
                        );
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    },
                    child: _wrapWithTooltip(
                        tooltip: widget.tooltip,
                        enabled: (widget.onPressed is not null) || (widget.onSelected is not null),
                        child: new _ChipRenderWidget__chip(
                            theme: new _ChipRenderTheme__chip(
                                label: new DefaultTextStyle(
                                    overflow: TextOverflow.fade,
                                    textAlign: TextAlign.start,
                                    maxLines: 1L,
                                    softWrap: false,
                                    style: resolvedLabelStyle,
                                    child: widget.label
                                ),
                                avatar: new AnimatedSwitcher(
                                    duration: ChipLibrary._kDrawerDuration,
                                    switchInCurve: Curves.fastOutSlowIn,
                                    child: avatarLocal
                                ),
                                deleteIcon: new AnimatedSwitcher(
                                    duration: ChipLibrary._kDrawerDuration,
                                    switchInCurve: Curves.fastOutSlowIn,
                                    child: _buildDeleteIcon(
                                        context,
                                        themeLocal,
                                        chipTheme,
                                        chipDefaults
                                    )
                                ),
                                brightness: brightnessLocal,
                                padding: paddingLocal.resolve(textDirection),
                                visualDensity: widget.visualDensity ?? themeLocal.visualDensity,
                                labelPadding: labelPaddingLocal.resolve(textDirection),
                                showAvatar: hasAvatar,
                                showCheckmark: showCheckmarkLocal,
                                checkmarkColor: checkmarkColorLocal,
                                canTapBody: canTap
                            ),
                            value: widget.selected,
                            checkmarkAnimation: checkmarkAnimation,
                            enableAnimation: enableAnimation,
                            avatarDrawerAnimation: avatarDrawerAnimation,
                            deleteDrawerAnimation: deleteDrawerAnimation,
                            isEnabled: widget.isEnabled,
                            avatarBorder: widget.avatarBorder,
                            avatarBoxConstraints: avatarBoxConstraintsLocal,
                            deleteIconBoxConstraints: deleteIconBoxConstraintsLocal
                        )
                    )
                )
            )
        );
        BoxConstraints constraintsLocal = default!;
        Offset densityAdjustment = (
            widget.visualDensity ?? themeLocal.visualDensity
        ).baseSizeAdjustment;
        switch (widget.materialTapTargetSize ?? themeLocal.materialTapTargetSize)
        {
            case var __constant54393 when Equals(__constant54393, MaterialTapTargetSize.padded):
            {
                constraintsLocal = new BoxConstraints(
                    minWidth: ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dx,
                    minHeight: ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy
                );
                break;
            }
            case var __constant54622 when Equals(__constant54622, MaterialTapTargetSize.shrinkWrap):
            {
                constraintsLocal = new BoxConstraints();
                break;
            }
        }
        result = DartRuntimePrimitives.ConvertValue<Widget>(
            new _ChipRedirectingHitDetectionWidget__chip(
                constraints: constraintsLocal,
                child: new Center(widthFactor: 1.0, heightFactor: 1.0, child: result)
            )
        );
        return new Widgets.Semantics(
            button: widget.tapEnabled,
            container: true,
            selected: Foundation.ConstantsLibrary.kIsWeb ? null : widget.selected,
            @checked: Foundation.ConstantsLibrary.kIsWeb ? widget.selected : null,
            enabled: widget.tapEnabled ? canTap : null,
            child: result
        );
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
}

internal class _IndividualOverrides__chip : WidgetStateProperty<Color?>
{
    public virtual WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }

    internal _IndividualOverrides__chip(
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        Color? selectedColor = null,
        Color? disabledColor = null
    )
    {
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.selectedColor = selectedColor;
        this.disabledColor = disabledColor;
    }

    public virtual Color? resolve(HashSet<WidgetState> states)
    {
        if (color is not null)
        {
            return color!.resolve(states);
        }
        if (states.Contains(WidgetState.selected) && states.Contains(WidgetState.disabled))
        {
            return selectedColor;
        }
        if (states.Contains(WidgetState.disabled))
        {
            return disabledColor;
        }
        if (states.Contains(WidgetState.selected))
        {
            return selectedColor;
        }
        return backgroundColor;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _ChipRedirectingHitDetectionWidget__chip : SingleChildRenderObjectWidget
{
    public virtual BoxConstraints constraints { get; private set; } = default!;

    internal _ChipRedirectingHitDetectionWidget__chip(
        Widget? child = null,
        BoxConstraints constraints = default!
    )
        : base(child: child)
    {
        this.constraints = constraints;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderChipRedirectingHitDetection__chip(constraints);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderChipRedirectingHitDetection__chip)renderObject;
        __renderObject.additionalConstraints = constraints;
    }
}

public class _RenderChipRedirectingHitDetection__chip : RenderConstrainedBox
{
    internal _RenderChipRedirectingHitDetection__chip(BoxConstraints additionalConstraints)
        : base(additionalConstraints: additionalConstraints) { }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (!size.contains(position))
        {
            return false;
        }
        var offset = new Offset(position.dx, size.height / 2L);
        return result.addWithRawTransform(
            transform: MatrixUtils.forceToPoint(offset),
            position: position,
            hitTest: (result, position) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(position, offset));
                return child!.hitTest(result, position: offset);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _ChipRenderWidget__chip
    : SlottedMultiChildRenderObjectWidget<_ChipSlot__chip, RenderBox>
{
    public virtual _ChipRenderTheme__chip theme { get; private set; } = default!;
    public virtual bool? value { get; private set; }
    public virtual bool? isEnabled { get; private set; }
    public virtual Animation<double> checkmarkAnimation { get; private set; } = default!;
    public virtual Animation<double> avatarDrawerAnimation { get; private set; } = default!;
    public virtual Animation<double> deleteDrawerAnimation { get; private set; } = default!;
    public virtual Animation<double> enableAnimation { get; private set; } = default!;
    public virtual ShapeBorder? avatarBorder { get; private set; }
    public virtual BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual BoxConstraints? deleteIconBoxConstraints { get; private set; }

    internal _ChipRenderWidget__chip(
        _ChipRenderTheme__chip theme,
        bool? value = null,
        bool? isEnabled = null,
        Animation<double> checkmarkAnimation = default!,
        Animation<double> avatarDrawerAnimation = default!,
        Animation<double> deleteDrawerAnimation = default!,
        Animation<double> enableAnimation = default!,
        ShapeBorder? avatarBorder = null,
        BoxConstraints? avatarBoxConstraints = null,
        BoxConstraints? deleteIconBoxConstraints = null
    )
    {
        this.theme = theme;
        this.value = value;
        this.isEnabled = isEnabled;
        this.checkmarkAnimation = checkmarkAnimation;
        this.avatarDrawerAnimation = avatarDrawerAnimation;
        this.deleteDrawerAnimation = deleteDrawerAnimation;
        this.enableAnimation = enableAnimation;
        this.avatarBorder = avatarBorder;
        this.avatarBoxConstraints = avatarBoxConstraints;
        this.deleteIconBoxConstraints = deleteIconBoxConstraints;
    }

    public override IEnumerable<_ChipSlot__chip> slots =>
        DartRuntimePrimitives.ConvertValue<IEnumerable<_ChipSlot__chip>>(
            Enum.GetValues<_ChipSlot__chip>().ToList()
        );

    public override Widget? childForSlot(_ChipSlot__chip slot)
    {
        return slot switch
        {
            _ChipSlot__chip.label => theme.label,
            _ChipSlot__chip.avatar => theme.avatar,
            _ChipSlot__chip.deleteIcon => theme.deleteIcon,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderChip__chip)renderObject;
        DartRuntimePrimitives.Ignore(
            (
                (Func<_RenderChip__chip>)(
                    () =>
                    {
                        var __cascade = __renderObject;
                        __cascade.theme = theme;
                        __cascade.textDirection = Directionality.of(context);
                        __cascade.value = value;
                        __cascade.isEnabled = isEnabled;
                        __cascade.checkmarkAnimation = checkmarkAnimation;
                        __cascade.avatarDrawerAnimation = avatarDrawerAnimation;
                        __cascade.deleteDrawerAnimation = deleteDrawerAnimation;
                        __cascade.enableAnimation = enableAnimation;
                        __cascade.avatarBorder = avatarBorder;
                        __cascade.avatarBoxConstraints = avatarBoxConstraints;
                        __cascade.deleteIconBoxConstraints = deleteIconBoxConstraints;
                        return __cascade;
                    }
                )
            )()
        );
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderChip__chip(
            theme: theme,
            textDirection: Directionality.of(context),
            value: value,
            isEnabled: isEnabled,
            checkmarkAnimation: checkmarkAnimation,
            avatarDrawerAnimation: avatarDrawerAnimation,
            deleteDrawerAnimation: deleteDrawerAnimation,
            enableAnimation: enableAnimation,
            avatarBorder: avatarBorder,
            avatarBoxConstraints: avatarBoxConstraints,
            deleteIconBoxConstraints: deleteIconBoxConstraints
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public enum _ChipSlot__chip
{
    label,
    avatar,
    deleteIcon,
}

public class _ChipRenderTheme__chip
{
    public virtual Widget avatar { get; private set; } = default!;
    public virtual Widget label { get; private set; } = default!;
    public virtual Widget deleteIcon { get; private set; } = default!;
    public virtual Brightness brightness { get; private set; } = default!;
    public virtual EdgeInsets padding { get; private set; } = default!;
    public virtual VisualDensity visualDensity { get; private set; } = default!;
    public virtual EdgeInsets labelPadding { get; private set; } = default!;
    public virtual bool showAvatar { get; private set; } = default!;
    public virtual bool showCheckmark { get; private set; } = default!;
    public virtual Color? checkmarkColor { get; private set; }
    public virtual bool canTapBody { get; private set; } = default!;

    internal _ChipRenderTheme__chip(
        Widget avatar,
        Widget label,
        Widget deleteIcon,
        Brightness brightness,
        EdgeInsets padding,
        VisualDensity visualDensity,
        EdgeInsets labelPadding,
        bool showAvatar,
        bool showCheckmark,
        Color? checkmarkColor,
        bool canTapBody
    )
    {
        this.avatar = avatar;
        this.label = label;
        this.deleteIcon = deleteIcon;
        this.brightness = brightness;
        this.padding = padding;
        this.visualDensity = visualDensity;
        this.labelPadding = labelPadding;
        this.showAvatar = showAvatar;
        this.showCheckmark = showCheckmark;
        this.checkmarkColor = checkmarkColor;
        this.canTapBody = canTapBody;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ChipRenderTheme__chip;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _ChipRenderTheme__chip)
            && Equals(__other.avatar, avatar)
            && Equals(__other.label, label)
            && Equals(__other.deleteIcon, deleteIcon)
            && Equals(__other.brightness, brightness)
            && Equals(__other.padding, padding)
            && Equals(__other.labelPadding, labelPadding)
            && (__other.showAvatar == showAvatar)
            && (__other.showCheckmark == showCheckmark)
            && Equals(__other.checkmarkColor, checkmarkColor)
            && (__other.canTapBody == canTapBody);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                avatar,
                label,
                deleteIcon,
                brightness,
                padding,
                labelPadding,
                showAvatar,
                showCheckmark,
                checkmarkColor,
                canTapBody
            )
        );
}

public class _RenderChip__chip
    : RenderBox,
        SlottedContainerRenderObjectMixin<_ChipSlot__chip, RenderBox>
{
    public virtual bool? value { get; set; } = default;
    public virtual bool? isEnabled { get; set; } = default;
    internal virtual Rect _deleteButtonRect { get; set; } = default!;
    internal virtual Rect _pressRect { get; set; } = default!;
    public virtual Animation<double> checkmarkAnimation { get; set; } = default!;
    public virtual Animation<double> avatarDrawerAnimation { get; set; } = default!;
    public virtual Animation<double> deleteDrawerAnimation { get; set; } = default!;
    public virtual Animation<double> enableAnimation { get; set; } = default!;
    public virtual ShapeBorder? avatarBorder { get; set; } = default;
    internal virtual _ChipRenderTheme__chip _theme { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual BoxConstraints? _avatarBoxConstraints { get; set; } = default;
    internal virtual BoxConstraints? _deleteIconBoxConstraints { get; set; } = default;
    public static ColorTween selectionScrimTween = new ColorTween(
        begin: Colors.transparent,
        end: ChipLibrary._kSelectScrimColor
    );
    internal virtual LayerHandle<OpacityLayer> _avatarOpacityLayerHandler { get; private set; } =
        new LayerHandle<OpacityLayer>();
    internal virtual LayerHandle<OpacityLayer> _labelOpacityLayerHandler { get; private set; } =
        new LayerHandle<OpacityLayer>();
    internal virtual LayerHandle<OpacityLayer> _deleteIconOpacityLayerHandler
    {
        get;
        private set;
    } = new LayerHandle<OpacityLayer>();
    internal const bool _debugShowTapTargetOutlines = false;
    public virtual DartMap<_ChipSlot__chip, RenderBox> _slotToChild { get; set; } =
        new DartMap<_ChipSlot__chip, RenderBox>();

    internal _RenderChip__chip(
        _ChipRenderTheme__chip theme,
        TextDirection textDirection,
        bool? value = null,
        bool? isEnabled = null,
        Animation<double> checkmarkAnimation = default!,
        Animation<double> avatarDrawerAnimation = default!,
        Animation<double> deleteDrawerAnimation = default!,
        Animation<double> enableAnimation = default!,
        ShapeBorder? avatarBorder = null,
        BoxConstraints? avatarBoxConstraints = null,
        BoxConstraints? deleteIconBoxConstraints = null
    )
    {
        this.value = value;
        this.isEnabled = isEnabled;
        this.checkmarkAnimation = checkmarkAnimation;
        this.avatarDrawerAnimation = avatarDrawerAnimation;
        this.deleteDrawerAnimation = deleteDrawerAnimation;
        this.enableAnimation = enableAnimation;
        this.avatarBorder = avatarBorder;
        _theme = theme;
        _textDirection = textDirection;
        _avatarBoxConstraints = avatarBoxConstraints;
        _deleteIconBoxConstraints = deleteIconBoxConstraints;
    }

    public virtual RenderBox avatar =>
        DartRuntimePrimitives.ConvertValue<RenderBox>(childForSlot(_ChipSlot__chip.avatar)!);
    public virtual RenderBox deleteIcon =>
        DartRuntimePrimitives.ConvertValue<RenderBox>(childForSlot(_ChipSlot__chip.deleteIcon)!);
    public virtual RenderBox label =>
        DartRuntimePrimitives.ConvertValue<RenderBox>(childForSlot(_ChipSlot__chip.label)!);
    public virtual _ChipRenderTheme__chip theme
    {
        get => _theme;
        set
        {
            var __value = value;
            if (Equals(_theme, __value))
            {
                return;
            }
            _theme = __value;
            markNeedsLayout();
        }
    }
    public virtual TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, (__value)))
            {
                return;
            }
            _textDirection = ((__value));
            markNeedsLayout();
        }
    }
    public virtual BoxConstraints? avatarBoxConstraints
    {
        get => _avatarBoxConstraints;
        set
        {
            var __value = value;
            if (Equals(_avatarBoxConstraints, __value))
            {
                return;
            }
            _avatarBoxConstraints = __value;
            markNeedsLayout();
        }
    }
    public virtual BoxConstraints? deleteIconBoxConstraints
    {
        get => _deleteIconBoxConstraints;
        set
        {
            var __value = value;
            if (Equals(_deleteIconBoxConstraints, __value))
            {
                return;
            }
            _deleteIconBoxConstraints = __value;
            markNeedsLayout();
        }
    }
    public virtual IEnumerable<RenderBox> children
    {
        get
        {
            RenderBox? avatarLocal = childForSlot(_ChipSlot__chip.avatar);
            RenderBox? labelLocal = childForSlot(_ChipSlot__chip.label);
            RenderBox? deleteIconLocal = childForSlot(_ChipSlot__chip.deleteIcon);
            return (
                (Func<List<RenderBox>>)(
                    () =>
                    {
                        var __collection64442 = new List<RenderBox>();
                        var __collectionElement64454 = avatarLocal;
                        if (__collectionElement64454 is { } __nonNullCollectionElement64454)
                        {
                            __collection64442.Add(__nonNullCollectionElement64454);
                        }
                        var __collectionElement64463 = labelLocal;
                        if (__collectionElement64463 is { } __nonNullCollectionElement64463)
                        {
                            __collection64442.Add(__nonNullCollectionElement64463);
                        }
                        var __collectionElement64471 = deleteIconLocal;
                        if (__collectionElement64471 is { } __nonNullCollectionElement64471)
                        {
                            __collection64442.Add(__nonNullCollectionElement64471);
                        }
                        return __collection64442;
                    }
                )
            )();
        }
    }
    public virtual bool isDrawingCheckmark =>
        DartRuntimePrimitives.ConvertValue<bool>(
            theme.showCheckmark && !checkmarkAnimation.isDismissed
        );
    public virtual bool deleteIconShowing => !deleteDrawerAnimation.isDismissed;

    internal static Rect _boxRect(RenderBox box) =>
        DartRuntimePrimitives.ConvertValue<Rect>(_boxParentData(box).offset & box.size);

    internal static BoxParentData _boxParentData(RenderBox box) =>
        ((BoxParentData?)box.parentData!)!;

    public override double computeMinIntrinsicWidth(double height)
    {
        double overallPadding = theme.padding.horizontal + theme.labelPadding.horizontal;
        return overallPadding
            + avatar.getMinIntrinsicWidth(height)
            + label.getMinIntrinsicWidth(height)
            + deleteIcon.getMinIntrinsicWidth(height);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double overallPadding = theme.padding.horizontal + theme.labelPadding.horizontal;
        return overallPadding
            + avatar.getMaxIntrinsicWidth(height)
            + label.getMaxIntrinsicWidth(height)
            + deleteIcon.getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return Math.Max(
            ChipLibrary._kChipHeight,
            theme.padding.vertical
                + theme.labelPadding.vertical
                + label.getMinIntrinsicHeight(width)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double computeMaxIntrinsicHeight(double width) => getMinIntrinsicHeight(width);

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return new BaselineOffset(label.getDistanceToActualBaseline(baseline))
            .op_Add(_boxParentData(label).offset.dy)
            .offset;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual BoxConstraints _labelConstraintsFrom(
        BoxConstraints contentConstraints,
        double iconWidth,
        double contentSize,
        Size rawLabelSize
    )
    {
        double freeSpace =
            contentConstraints.maxWidth
            - iconWidth
            - theme.labelPadding.horizontal
            - theme.padding.horizontal;
        double maxLabelWidth = Math.Max(0.0, freeSpace);
        return new BoxConstraints(
            minHeight: rawLabelSize.height,
            maxHeight: contentSize,
            maxWidth: double.IsFinite(maxLabelWidth) ? maxLabelWidth : rawLabelSize.width
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Size _layoutAvatar(
        double contentSize,
        Func<RenderBox, BoxConstraints, Size> layoutChild = default!
    )
    {
        BoxConstraints avatarConstraints =
            avatarBoxConstraints
            ?? BoxConstraints.CreateTightFor(width: contentSize, height: contentSize);
        Size avatarBoxSize = layoutChild(avatar, avatarConstraints);
        if (!theme.showCheckmark && !theme.showAvatar)
        {
            return new Size(0.0, contentSize);
        }
        double avatarFullWidth = theme.showAvatar ? avatarBoxSize.width : contentSize;
        return new Size(avatarFullWidth * avatarDrawerAnimation.value, avatarBoxSize.height);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual Size _layoutDeleteIcon(
        double contentSize,
        Func<RenderBox, BoxConstraints, Size> layoutChild = default!
    )
    {
        BoxConstraints deleteIconConstraints =
            deleteIconBoxConstraints
            ?? BoxConstraints.CreateTightFor(width: contentSize, height: contentSize);
        Size boxSize = layoutChild(deleteIcon, deleteIconConstraints);
        if (!deleteIconShowing)
        {
            return new Size(0.0, contentSize);
        }
        return new Size(deleteDrawerAnimation.value * boxSize.width, boxSize.height);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool hitTest(BoxHitTestResult result, Offset position)
    {
        if (!size.contains(position))
        {
            return false;
        }
        bool hitIsOnDeleteIcon = ChipLibrary._hitIsOnDeleteIcon(
            padding: theme.padding,
            labelPadding: theme.labelPadding,
            tapPosition: position,
            chipSize: size,
            deleteButtonSize: deleteIcon.size,
            textDirection: textDirection
        );
        RenderBox hitTestChild = hitIsOnDeleteIcon ? deleteIcon : label;
        Offset centerLocal = hitTestChild.size.center(Offset.zero);
        return result.addWithRawTransform(
            transform: MatrixUtils.forceToPoint(centerLocal),
            position: position,
            hitTest: (result, position) =>
            {
                DartRuntimePrimitives.Assert(() => Equals(position, centerLocal));
                return hitTestChild.hitTest(result, position: centerLocal);
                throw new InvalidOperationException(
                    "Callback completed without returning a value."
                );
            }
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return _computeSizes(constraints, ChildLayoutHelper.dryLayoutChild).size;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        _ChipSizes__chip sizes = _computeSizes(constraints, ChildLayoutHelper.dryLayoutChild);
        BaselineOffset labelBaseline = new BaselineOffset(
            label.getDryBaseline(sizes.labelConstraints, baseline)
        )
            .op_Add((sizes.content - sizes.label.height + sizes.densityAdjustment.dy) / 2L)
            .op_Add(theme.padding.top)
            .op_Add(theme.labelPadding.top);
        return labelBaseline.offset;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual _ChipSizes__chip _computeSizes(
        BoxConstraints constraints,
        Func<RenderBox, BoxConstraints, Size> layoutChild
    )
    {
        BoxConstraints contentConstraints = constraints.loosen();
        Size rawLabelSize = label.getDryLayout(contentConstraints);
        double contentSize = Math.Max(
            ChipLibrary._kChipHeight - theme.padding.vertical + theme.labelPadding.vertical,
            rawLabelSize.height + theme.labelPadding.vertical
        );
        DartRuntimePrimitives.Assert(() => contentSize >= rawLabelSize.height);
        Size avatarSize = _layoutAvatar(contentSize, layoutChild);
        Size deleteIconSize = _layoutDeleteIcon(contentSize, layoutChild);
        BoxConstraints labelConstraintsLocal = _labelConstraintsFrom(
            contentConstraints,
            avatarSize.width + deleteIconSize.width,
            contentSize,
            rawLabelSize
        );
        Size labelSize = theme.labelPadding.inflateSize(layoutChild(label, labelConstraintsLocal));
        var densityAdjustmentLocal = new Offset(
            0.0,
            theme.visualDensity.baseSizeAdjustment.dy / 2.0
        );
        Size overallSize =
            new Size(avatarSize.width + labelSize.width + deleteIconSize.width, contentSize)
            + densityAdjustmentLocal;
        var paddedSize = new Size(
            overallSize.width + theme.padding.horizontal,
            overallSize.height + theme.padding.vertical
        );
        return new _ChipSizes__chip(
            size: constraints.constrain(paddedSize),
            overall: overallSize,
            content: contentSize,
            densityAdjustment: densityAdjustmentLocal,
            avatar: avatarSize,
            labelConstraints: labelConstraintsLocal,
            label: labelSize,
            deleteIcon: deleteIconSize
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void performLayout()
    {
        _ChipSizes__chip sizes = _computeSizes(constraints, ChildLayoutHelper.layoutChild);
        var leftLocal = 0.0;
        double rightLocal = sizes.overall.width;
        Offset centerLayout(Size boxSize, double x)
        {
            DartRuntimePrimitives.Assert(() => sizes.content >= boxSize.height);
            switch (textDirection)
            {
                case TextDirection.rtl:
                {
                    x -= boxSize.width;
                    break;
                }
                case TextDirection.ltr:
                {
                    break;
                }
            }
            return new Offset(
                x,
                (sizes.content - boxSize.height + sizes.densityAdjustment.dy) / 2.0
            );
            throw new InvalidOperationException(
                "Control flow completed without returning a value."
            );
        }
        Offset avatarOffset = Offset.zero;
        Offset labelOffset = Offset.zero;
        Offset deleteIconOffset = Offset.zero;
        switch (textDirection)
        {
            case TextDirection.rtl:
            {
                var start = rightLocal;
                if (theme.showCheckmark || theme.showAvatar)
                {
                    avatarOffset = centerLayout(sizes.avatar, start);
                    start -= sizes.avatar.width;
                }
                labelOffset = centerLayout(sizes.label, start);
                start -= sizes.label.width;
                if (deleteIconShowing)
                {
                    _deleteButtonRect = Rect.fromLTWH(
                        0.0,
                        0.0,
                        sizes.deleteIcon.width + theme.padding.right,
                        sizes.overall.height + theme.padding.vertical
                    );
                    deleteIconOffset = centerLayout(sizes.deleteIcon, start);
                }
                else
                {
                    _deleteButtonRect = Rect.zero;
                }
                start -= sizes.deleteIcon.width;
                if (theme.canTapBody)
                {
                    _pressRect = Rect.fromLTWH(
                        _deleteButtonRect.width,
                        0.0,
                        sizes.overall.width - _deleteButtonRect.width + theme.padding.horizontal,
                        sizes.overall.height + theme.padding.vertical
                    );
                }
                else
                {
                    _pressRect = Rect.zero;
                }
                break;
            }
            case TextDirection.ltr:
            {
                var startLocal = leftLocal;
                if (theme.showCheckmark || theme.showAvatar)
                {
                    avatarOffset = centerLayout(
                        sizes.avatar,
                        startLocal - avatar.size.width + sizes.avatar.width
                    );
                    startLocal += sizes.avatar.width;
                }
                labelOffset = centerLayout(sizes.label, startLocal);
                startLocal += sizes.label.width;
                if (theme.canTapBody)
                {
                    _pressRect = Rect.fromLTWH(
                        0.0,
                        0.0,
                        deleteIconShowing
                            ? (startLocal + theme.padding.left)
                            : (sizes.overall.width + theme.padding.horizontal),
                        sizes.overall.height + theme.padding.vertical
                    );
                }
                else
                {
                    _pressRect = Rect.zero;
                }
                startLocal -= deleteIcon.size.width - sizes.deleteIcon.width;
                if (deleteIconShowing)
                {
                    deleteIconOffset = centerLayout(sizes.deleteIcon, startLocal);
                    _deleteButtonRect = Rect.fromLTWH(
                        startLocal + theme.padding.left,
                        0.0,
                        sizes.deleteIcon.width + theme.padding.right,
                        sizes.overall.height + theme.padding.vertical
                    );
                }
                else
                {
                    _deleteButtonRect = Rect.zero;
                }
                break;
            }
        }
        labelOffset =
            labelOffset
            + new Offset(
                0.0,
                (sizes.label.height - theme.labelPadding.vertical - label.size.height) / 2.0
            );
        _boxParentData(avatar).offset = theme.padding.topLeft + avatarOffset;
        _boxParentData(label).offset =
            theme.padding.topLeft + labelOffset + theme.labelPadding.topLeft;
        _boxParentData(deleteIcon).offset = theme.padding.topLeft + deleteIconOffset;
        var paddedSize = new Size(
            sizes.overall.width + theme.padding.horizontal,
            sizes.overall.height + theme.padding.vertical
        );
        size = constraints.constrain(paddedSize);
        DartRuntimePrimitives.Assert(
            () => size.height == constraints.constrainHeight(paddedSize.height),
            () =>
                (object?)$"Constrained height {size.height} doesn't match expected height "
                + $"{constraints.constrainWidth(paddedSize.height)}"
        );
        DartRuntimePrimitives.Assert(
            () => size.width == constraints.constrainWidth(paddedSize.width),
            () =>
                (object?)$"Constrained width {size.width} doesn't match expected width "
                + $"{constraints.constrainWidth(paddedSize.width)}"
        );
    }

    internal virtual Color _disabledColor
    {
        get
        {
            if (enableAnimation.isCompleted)
            {
                return Colors.white;
            }
            Color color = theme.brightness switch
            {
                Brightness.light => Colors.white,
                Brightness.dark => Colors.black,
                _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                    throw new InvalidOperationException(
                        "Switch expression did not handle the supplied value."
                    ),
            };
            return new ColorTween(
                begin: color.withAlpha(ChipLibrary._kDisabledAlpha),
                end: color
            ).evaluate(enableAnimation)!;
        }
    }

    internal virtual void _paintCheck(Canvas canvas, Offset origin, double size)
    {
        Color? paintColor = (Color?)(
            theme.checkmarkColor
            ?? (
                (theme.brightness, theme.showAvatar) switch
                {
                    (Brightness.light, true) => Colors.white,
                    (Brightness.light, false) => Colors.black.withAlpha(
                        ChipLibrary._kCheckmarkAlpha
                    ),
                    (Brightness.dark, true) => Colors.black,
                    (Brightness.dark, false) => Colors.white.withAlpha(
                        ChipLibrary._kCheckmarkAlpha
                    ),
                }
            )
        );
        var fadeTween = new ColorTween(begin: Colors.transparent, end: paintColor);
        paintColor = Equals(checkmarkAnimation.status, AnimationStatus.reverse)
            ? fadeTween.evaluate(checkmarkAnimation)
            : paintColor;
        var paint = (
            (Func<Paint>)(
                () =>
                {
                    var __cascade = new Paint();
                    __cascade.color = paintColor!;
                    __cascade.style = PaintingStyle.stroke;
                    __cascade.strokeWidth =
                        ChipLibrary._kCheckmarkStrokeWidth * avatar.size.height / 24.0;
                    return __cascade;
                }
            )
        )();
        double t = Equals(checkmarkAnimation.status, AnimationStatus.reverse)
            ? 1.0
            : checkmarkAnimation.value;
        if (t == 0.0)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (t > 0.0) && (t <= 1.0));
        var path = new Path();
        var start = new Offset(size * 0.15, size * 0.45);
        var mid = new Offset(size * 0.4, size * 0.7);
        var endLocal = new Offset(size * 0.85, size * 0.25);
        if (t < 0.5)
        {
            double strokeT = t * 2.0;
            Offset drawMid = (
                Dart_uiLibrary.Offset.lerp(start, mid, strokeT)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            path.moveTo(origin.dx + start.dx, origin.dy + start.dy);
            path.lineTo(origin.dx + drawMid.dx, origin.dy + drawMid.dy);
        }
        else
        {
            double strokeTLocal = (t - 0.5) * 2.0;
            Offset drawEnd = (
                Dart_uiLibrary.Offset.lerp(mid, endLocal, strokeTLocal)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            );
            path.moveTo(origin.dx + start.dx, origin.dy + start.dy);
            path.lineTo(origin.dx + mid.dx, origin.dy + mid.dy);
            path.lineTo(origin.dx + drawEnd.dx, origin.dy + drawEnd.dy);
        }
        canvas.drawPath(path, paint);
    }

    internal virtual void _paintSelectionOverlay(PaintingContext context, Offset offset)
    {
        if (isDrawingCheckmark)
        {
            if (theme.showAvatar)
            {
                Rect avatarRect = _boxRect(avatar).shift(offset);
                var darkenPaint = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = selectionScrimTween.evaluate(checkmarkAnimation)!;
                            __cascade.blendMode = BlendMode.srcATop;
                            return __cascade;
                        }
                    )
                )();
                if (avatarBorder!.preferPaintInterior)
                {
                    avatarBorder!.paintInterior(context.canvas, avatarRect, darkenPaint);
                }
                else
                {
                    Path path = avatarBorder!.getOuterPath(avatarRect);
                    context.canvas.drawPath(path, darkenPaint);
                }
            }
            double checkSize = avatar.size.height * 0.75;
            Offset checkOffset =
                _boxParentData(avatar).offset
                + new Offset(avatar.size.height * 0.125, avatar.size.height * 0.125);
            _paintCheck(context.canvas, offset + checkOffset, checkSize);
        }
    }

    internal virtual void _paintAvatar(PaintingContext context, Offset offset)
    {
        void paintWithOverlay(PaintingContext context, Offset offset)
        {
            context.paintChild(avatar, _boxParentData(avatar).offset + offset);
            _paintSelectionOverlay(context, offset);
        }
        if (!theme.showAvatar && avatarDrawerAnimation.isDismissed)
        {
            _avatarOpacityLayerHandler.layer = null;
            return;
        }
        Color disabledColor = _disabledColor;
        long disabledColorAlpha = disabledColor.alpha;
        if (needsCompositing)
        {
            _avatarOpacityLayerHandler.layer = context.pushOpacity(
                offset,
                disabledColorAlpha,
                paintWithOverlay,
                oldLayer: _avatarOpacityLayerHandler.layer
            );
        }
        else
        {
            _avatarOpacityLayerHandler.layer = null;
            if (disabledColorAlpha != 255L)
            {
                context.canvas.saveLayer(
                    _boxRect(avatar).shift(offset).inflate(20.0),
                    (
                        (Func<Paint>)(
                            () =>
                            {
                                var __cascade = new Paint();
                                __cascade.color = disabledColor;
                                return __cascade;
                            }
                        )
                    )()
                );
            }
            paintWithOverlay(context, offset);
            if (disabledColorAlpha != 255L)
            {
                context.canvas.restore();
            }
        }
    }

    internal virtual void _paintChild(
        PaintingContext context,
        Offset offset,
        RenderBox? child,
        bool isDeleteIcon
    )
    {
        if (child is null)
        {
            _labelOpacityLayerHandler.layer = null;
            _deleteIconOpacityLayerHandler.layer = null;
            return;
        }
        long disabledColorAlpha = _disabledColor.alpha;
        if (!enableAnimation.isCompleted)
        {
            if (needsCompositing)
            {
                _labelOpacityLayerHandler.layer = context.pushOpacity(
                    offset,
                    disabledColorAlpha,
                    (context, offset) =>
                    {
                        context.paintChild(child, _boxParentData(child).offset + offset);
                    },
                    oldLayer: _labelOpacityLayerHandler.layer
                );
                if (isDeleteIcon)
                {
                    _deleteIconOpacityLayerHandler.layer = context.pushOpacity(
                        offset,
                        disabledColorAlpha,
                        (context, offset) =>
                        {
                            context.paintChild(child, _boxParentData(child).offset + offset);
                        },
                        oldLayer: _deleteIconOpacityLayerHandler.layer
                    );
                }
            }
            else
            {
                _labelOpacityLayerHandler.layer = null;
                _deleteIconOpacityLayerHandler.layer = null;
                Rect childRect = _boxRect(child).shift(offset);
                context.canvas.saveLayer(
                    childRect.inflate(20.0),
                    (
                        (Func<Paint>)(
                            () =>
                            {
                                var __cascade = new Paint();
                                __cascade.color = _disabledColor;
                                return __cascade;
                            }
                        )
                    )()
                );
                context.paintChild(child, _boxParentData(child).offset + offset);
                context.canvas.restore();
            }
        }
        else
        {
            context.paintChild(child, _boxParentData(child).offset + offset);
        }
    }

    public override void attach(PipelineOwner owner)
    {
        base.attach(owner);
        foreach (RenderBox child in children)
        {
            child.attach(owner);
        }
        checkmarkAnimation.addListener(markNeedsPaint);
        avatarDrawerAnimation.addListener(markNeedsLayout);
        deleteDrawerAnimation.addListener(markNeedsLayout);
        enableAnimation.addListener(markNeedsPaint);
    }

    public override void detach()
    {
        checkmarkAnimation.removeListener(markNeedsPaint);
        avatarDrawerAnimation.removeListener(markNeedsLayout);
        deleteDrawerAnimation.removeListener(markNeedsLayout);
        enableAnimation.removeListener(markNeedsPaint);
        base.detach();
        foreach (RenderBox child in children)
        {
            child.detach();
        }
    }

    public override void dispose()
    {
        _labelOpacityLayerHandler.layer = null;
        _deleteIconOpacityLayerHandler.layer = null;
        _avatarOpacityLayerHandler.layer = null;
        base.dispose();
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        _paintAvatar(context, offset);
        if (deleteIconShowing)
        {
            _paintChild(context, offset, deleteIcon, isDeleteIcon: true);
        }
        _paintChild(context, offset, label, isDeleteIcon: false);
    }

    public override void debugPaint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            !_debugShowTapTargetOutlines
            || (
                (Func<bool>)(
                    () =>
                    {
                        var outlinePaint = (
                            (Func<Paint>)(
                                () =>
                                {
                                    var __cascade = new Paint();
                                    __cascade.color = new Color(4286578688L);
                                    __cascade.strokeWidth = 1.0;
                                    __cascade.style = PaintingStyle.stroke;
                                    return __cascade;
                                }
                            )
                        )();
                        if (deleteIconShowing)
                        {
                            context.canvas.drawRect(_deleteButtonRect.shift(offset), outlinePaint);
                        }
                        context.canvas.drawRect(
                            _pressRect.shift(offset),
                            (
                                (Func<Paint>)(
                                    () =>
                                    {
                                        var __cascade = outlinePaint;
                                        __cascade.color = new Color(4278222848L);
                                        return __cascade;
                                    }
                                )
                            )()
                        );
                        return true;
                        throw new InvalidOperationException(
                            "Callback completed without returning a value."
                        );
                    }
                )
            )()
        );
    }

    public override bool hitTestSelf(Offset position) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            _deleteButtonRect.contains(position) || _pressRect.contains(position)
        );

    public virtual RenderBox? childForSlot(_ChipSlot__chip slot) =>
        _slotToChild.GetValueOrDefault(slot);

    public virtual string debugNameForSlot(_ChipSlot__chip slot)
    {
        {
            return slot.ToString();
        }
    }

    public override void redepthChildren()
    {
        children.forEach(
            (__arg0) =>
                ((Action<RenderObject>)redepthChild)(
                    DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0)
                )
        );
    }

    public override void visitChildren(Action<RenderObject> visitor)
    {
        children.forEach(
            (__arg0) => visitor(DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0))
        );
    }

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<DiagnosticsNode>();
        var childToSlot = new DartMap<RenderBox, _ChipSlot__chip>(
            _slotToChild.Values,
            _slotToChild.Keys
        );
        foreach (RenderBox child in children)
        {
            _addDiagnostics(
                child,
                value,
                debugNameForSlot(
                    (
                        DartCollectionRuntime.NullableMapValue<_ChipSlot__chip>(childToSlot, child)
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                )
            );
        }
        return value;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void _addDiagnostics(RenderBox child, List<DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(RenderBox? child, _ChipSlot__chip slot)
    {
        RenderBox? oldChild = _slotToChild.GetValueOrDefault(slot);
        if (oldChild is not null)
        {
            dropChild(oldChild);
            _slotToChild.remove(slot);
        }
        if (child is not null)
        {
            _slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(RenderBox child, _ChipSlot__chip slot, _ChipSlot__chip oldSlot)
    {
        DartRuntimePrimitives.Assert(() => !Equals(slot, oldSlot));
        RenderBox? oldChild = _slotToChild.GetValueOrDefault(oldSlot);
        if (Equals(oldChild, child))
        {
            _setChild(null, oldSlot);
        }
        _setChild(child, slot);
    }
}

internal class _ChipSizes__chip
{
    public virtual Size size { get; private set; } = default!;
    public virtual Size overall { get; private set; } = default!;
    public virtual double content { get; private set; } = default!;
    public virtual Size avatar { get; private set; } = default!;
    public virtual BoxConstraints labelConstraints { get; private set; } = default!;
    public virtual Size label { get; private set; } = default!;
    public virtual Size deleteIcon { get; private set; } = default!;
    public virtual Offset densityAdjustment { get; private set; } = default!;

    internal _ChipSizes__chip(
        Size size,
        Size overall,
        double content,
        Size avatar,
        BoxConstraints labelConstraints,
        Size label,
        Size deleteIcon,
        Offset densityAdjustment
    )
    {
        this.size = size;
        this.overall = overall;
        this.content = content;
        this.avatar = avatar;
        this.labelConstraints = labelConstraints;
        this.label = label;
        this.deleteIcon = deleteIcon;
        this.densityAdjustment = densityAdjustment;
    }
}

internal class _UnconstrainedInkSplashFactory__chip : InteractiveInkFeatureFactory
{
    public virtual InteractiveInkFeatureFactory parentFactory { get; private set; } = default!;

    internal _UnconstrainedInkSplashFactory__chip(InteractiveInkFeatureFactory parentFactory)
    {
        this.parentFactory = parentFactory;
    }

    public virtual InteractiveInkFeature create(
        MaterialInkController controller,
        RenderBox referenceBox,
        Offset position,
        Color color,
        TextDirection textDirection,
        bool containedInkWell = false,
        Func<Rect>? rectCallback = null,
        BorderRadius? borderRadius = null,
        ShapeBorder? customBorder = null,
        double? radius = null,
        Action? onRemoved = null
    )
    {
        return parentFactory.create(
            controller: controller,
            referenceBox: referenceBox,
            position: position,
            color: color,
            rectCallback: rectCallback,
            borderRadius: borderRadius,
            customBorder: customBorder,
            radius: radius,
            onRemoved: onRemoved,
            textDirection: textDirection
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class ChipLibrary
{
    internal static bool _hitIsOnDeleteIcon(
        EdgeInsetsGeometry padding,
        EdgeInsetsGeometry labelPadding,
        Offset tapPosition,
        Size chipSize,
        Size deleteButtonSize,
        TextDirection textDirection
    )
    {
        EdgeInsets resolvedPadding = padding.resolve(textDirection);
        Size deflatedSize = resolvedPadding.deflateSize(chipSize);
        Offset adjustedPosition =
            tapPosition - new Offset(resolvedPadding.left, resolvedPadding.top);
        double accessibleDeleteButtonWidth = Math.Min(
            deflatedSize.width * 0.499,
            Math.Min(
                labelPadding.resolve(textDirection).right + deleteButtonSize.width,
                24.0 + (deleteButtonSize.width / 2.0)
            )
        );
        return textDirection switch
        {
            TextDirection.ltr => adjustedPosition.dx
                >= (deflatedSize.width - accessibleDeleteButtonWidth),
            TextDirection.rtl => adjustedPosition.dx <= accessibleDeleteButtonWidth,
            _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard =>
                throw new InvalidOperationException(
                    "Switch expression did not handle the supplied value."
                ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _EnsureMinSemanticsSize__chip : SingleChildRenderObjectWidget
{
    public virtual Size semanticSize { get; private set; } = default!;

    internal _EnsureMinSemanticsSize__chip(Widget? child = null, Size semanticSize = default!)
        : base(child: child)
    {
        this.semanticSize = semanticSize;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderEnsureMinSemanticsSize__chip(semanticSize);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderEnsureMinSemanticsSize__chip)renderObject;
        __renderObject.semanticSize = semanticSize;
    }
}

public class _RenderEnsureMinSemanticsSize__chip : RenderProxyBox
{
    internal virtual Size _semanticSize { get; set; } = default!;

    internal _RenderEnsureMinSemanticsSize__chip(Size _semanticSize, RenderBox? child = null)
        : base(child)
    {
        this._semanticSize = _semanticSize;
    }

    public virtual Size semanticSize
    {
        get => _semanticSize;
        set
        {
            var __value = value;
            if (Equals(_semanticSize, __value))
            {
                return;
            }
            _semanticSize = __value;
            markNeedsSemanticsUpdate();
        }
    }

    public override void describeSemanticsConfiguration(SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.isButton = true;
    }

    public override Rect semanticBounds
    {
        get
        {
            return Rect.fromCenter(
                center: paintBounds.center,
                width: Math.Max(_semanticSize.width, size.width),
                height: Math.Max(_semanticSize.height, size.height)
            );
        }
    }
}

internal class _ChipDefaultsM3__chip : ChipThemeData
{
    public virtual BuildContext context { get; private set; } = default!;
    public virtual bool isEnabled { get; private set; } = default!;
    private bool __late__colors_initialized;
    private ColorScheme __late__colors = default!;
    internal virtual ColorScheme _colors
    {
        get
        {
            if (!__late__colors_initialized)
            {
                __late__colors = Theme.of(context).colorScheme;
                __late__colors_initialized = true;
            }
            return __late__colors;
        }
    }
    private bool __late__textTheme_initialized;
    private TextTheme __late__textTheme = default!;
    internal virtual TextTheme _textTheme
    {
        get
        {
            if (!__late__textTheme_initialized)
            {
                __late__textTheme = Theme.of(context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _ChipDefaultsM3__chip(BuildContext context, bool isEnabled)
        : base(
            elevation: 0.0,
            shape: new RoundedRectangleBorder(
                borderRadius: BorderRadius.CreateAll(Radius.circular(8.0))
            ),
            showCheckmark: true
        )
    {
        this.context = context;
        this.isEnabled = isEnabled;
    }

    public override TextStyle? labelStyle =>
        _textTheme.labelLarge?.copyWith(
            color: isEnabled ? _colors.onSurfaceVariant : _colors.onSurface
        );
    public override WidgetStateProperty<Color?>? color =>
        DartRuntimePrimitives.ConvertValue<WidgetStateProperty<Color?>>(null);
    public override Color? shadowColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? surfaceTintColor =>
        DartRuntimePrimitives.ConvertValue<Color>(Colors.transparent);
    public override Color? checkmarkColor => DartRuntimePrimitives.ConvertValue<Color>(null);
    public override Color? deleteIconColor =>
        DartRuntimePrimitives.ConvertValue<Color>(
            isEnabled ? _colors.onSurfaceVariant : _colors.onSurface
        );
    public override BorderSide? side =>
        isEnabled
            ? new BorderSide(color: _colors.outlineVariant)
            : new BorderSide(color: _colors.onSurface.withOpacity(0.12));
    public override IconThemeData? iconTheme =>
        new IconThemeData(color: isEnabled ? _colors.primary : _colors.onSurface, size: 18.0);
    public override EdgeInsetsGeometry? padding =>
        DartRuntimePrimitives.ConvertValue<EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0));
    public override EdgeInsetsGeometry? labelPadding
    {
        get
        {
            double fontSizeLocal = labelStyle?.fontSize ?? 14.0;
            double fontSizeRatio = MediaQuery.textScalerOf(context).scale(fontSizeLocal) / 14.0;
            return EdgeInsets.lerp(
                EdgeInsets.CreateSymmetric(horizontal: 8.0),
                EdgeInsets.CreateSymmetric(horizontal: 4.0),
                Dart_uiLibrary.clampDouble(fontSizeRatio - 1.0, 0.0, 1.0)
            )!;
        }
    }
}
