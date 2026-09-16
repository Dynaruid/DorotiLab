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
    internal static Color _kSelectScrimColor = new global::Doroti.Ui.Color(1612257561L);
}

public static partial class ChipLibrary
{
    internal static global::Doroti.Framework.Widgets.Icon _kDefaultDeleteIcon = new global::Doroti.Framework.Widgets.Icon(Icons.cancel);
}

public interface ChipAttributes
{
    public global::Doroti.Framework.Widgets.Widget label { get; }
    public global::Doroti.Framework.Widgets.Widget? avatar { get; }
    public global::Doroti.Framework.Painting.TextStyle? labelStyle { get; }
    public global::Doroti.Framework.Painting.BorderSide? side { get; }
    public global::Doroti.Framework.Painting.OutlinedBorder? shape { get; }
    public global::Doroti.Ui.Clip clipBehavior { get; }
    public global::Doroti.Framework.Widgets.FocusNode? focusNode { get; }
    public bool autofocus { get; }
    public global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? color { get; }
    public global::Doroti.Ui.Color? backgroundColor { get; }
    public global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; }
    public VisualDensity? visualDensity { get; }
    public global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; }
    public MaterialTapTargetSize? materialTapTargetSize { get; }
    public double? elevation { get; }
    public global::Doroti.Ui.Color? shadowColor { get; }
    public global::Doroti.Ui.Color? surfaceTintColor { get; }
    public global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; }
    public global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints { get; }
    public ChipAnimationStyle? chipAnimationStyle { get; }
    public global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; }
}

public interface DeletableChipAttributes
{
    public global::Doroti.Framework.Widgets.Widget? deleteIcon { get; }
    public global::System.Action? onDeleted { get; }
    public global::Doroti.Ui.Color? deleteIconColor { get; }
    public string? deleteButtonTooltipMessage { get; }
    public global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints { get; }
}

public interface CheckmarkableChipAttributes
{
    public bool? showCheckmark { get; }
    public global::Doroti.Ui.Color? checkmarkColor { get; }
}

public interface SelectableChipAttributes
{
    public bool selected { get; }
    public global::System.Action<bool>? onSelected { get; }
    public double? pressElevation { get; }
    public global::Doroti.Ui.Color? selectedColor { get; }
    public global::Doroti.Ui.Color? selectedShadowColor { get; }
    public string? tooltip { get; }
    public global::Doroti.Framework.Painting.ShapeBorder avatarBorder { get; }
}

public interface DisabledChipAttributes
{
    public bool isEnabled { get; }
    public global::Doroti.Ui.Color? disabledColor { get; }
}

public interface TappableChipAttributes
{
    public global::System.Action? onPressed { get; }
    public double? pressElevation { get; }
    public string? tooltip { get; }
}

public class ChipAnimationStyle
{
    public virtual global::Doroti.Framework.Animation.AnimationStyle? enableAnimation { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationStyle? selectAnimation { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationStyle? avatarDrawerAnimation { get; private set; }
    public virtual global::Doroti.Framework.Animation.AnimationStyle? deleteDrawerAnimation { get; private set; }

    public ChipAnimationStyle(global::Doroti.Framework.Animation.AnimationStyle? enableAnimation = null, global::Doroti.Framework.Animation.AnimationStyle? selectAnimation = null, global::Doroti.Framework.Animation.AnimationStyle? avatarDrawerAnimation = null, global::Doroti.Framework.Animation.AnimationStyle? deleteDrawerAnimation = null)
    {
        this.enableAnimation = enableAnimation;
        this.selectAnimation = selectAnimation;
        this.avatarDrawerAnimation = avatarDrawerAnimation;
        this.deleteDrawerAnimation = deleteDrawerAnimation;
    }

}

public class Chip : global::Doroti.Framework.Widgets.StatelessWidget, ChipAttributes, DeletableChipAttributes
{
    public virtual global::Doroti.Framework.Widgets.Widget? avatar { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? deleteIcon { get; private set; }
    public virtual global::System.Action? onDeleted { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual string? deleteButtonTooltipMessage { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }

    public Chip(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? avatar = null, global::Doroti.Framework.Widgets.Widget label = default!, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Widgets.Widget? deleteIcon = null, global::System.Action? onDeleted = null, Color? deleteIconColor = null, string? deleteButtonTooltipMessage = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, MaterialTapTargetSize? materialTapTargetSize = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null, ChipAnimationStyle? chipAnimationStyle = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        return new RawChip(avatar: avatar, label: label, labelStyle: labelStyle, labelPadding: labelPadding, deleteIcon: deleteIcon, onDeleted: onDeleted, deleteIconColor: deleteIconColor, deleteButtonTooltipMessage: deleteButtonTooltipMessage, tapEnabled: false, side: side, shape: shape, clipBehavior: clipBehavior, focusNode: focusNode, autofocus: autofocus, color: color, backgroundColor: backgroundColor, padding: padding, visualDensity: visualDensity, materialTapTargetSize: materialTapTargetSize, elevation: elevation, shadowColor: shadowColor, surfaceTintColor: surfaceTintColor, iconTheme: iconTheme, avatarBoxConstraints: avatarBoxConstraints, deleteIconBoxConstraints: deleteIconBoxConstraints, chipAnimationStyle: chipAnimationStyle, mouseCursor: mouseCursor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class RawChip : global::Doroti.Framework.Widgets.StatefulWidget, ChipAttributes, DeletableChipAttributes, SelectableChipAttributes, CheckmarkableChipAttributes, DisabledChipAttributes, TappableChipAttributes
{
    public virtual ChipThemeData? defaultProperties { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? avatar { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget deleteIcon { get; private set; }
    public virtual global::System.Action? onDeleted { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual string? deleteButtonTooltipMessage { get; private set; }
    public virtual global::System.Action<bool>? onSelected { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual bool selected { get; private set; } = default!;
    public virtual bool isEnabled { get; private set; } = default!;
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual string? tooltip { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.FocusNode? focusNode { get; private set; }
    public virtual bool autofocus { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual VisualDensity? visualDensity { get; private set; }
    public virtual MaterialTapTargetSize? materialTapTargetSize { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual Color? selectedShadowColor { get; private set; }
    public virtual bool? showCheckmark { get; private set; }
    public virtual Color? checkmarkColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder avatarBorder { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints { get; private set; }
    public virtual ChipAnimationStyle? chipAnimationStyle { get; private set; }
    public virtual global::Doroti.Framework.Services.MouseCursor? mouseCursor { get; private set; }
    public virtual bool tapEnabled { get; private set; } = default!;

    public RawChip(global::Doroti.Framework.Foundation.Key? key = null, ChipThemeData? defaultProperties = null, global::Doroti.Framework.Widgets.Widget? avatar = null, global::Doroti.Framework.Widgets.Widget label = default!, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, VisualDensity? visualDensity = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Widgets.Widget? deleteIcon = null, global::System.Action? onDeleted = null, Color? deleteIconColor = null, string? deleteButtonTooltipMessage = null, global::System.Action? onPressed = null, global::System.Action<bool>? onSelected = null, double? pressElevation = null, bool tapEnabled = true, bool selected = false, bool isEnabled = true, Color? disabledColor = null, Color? selectedColor = null, string? tooltip = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, Clip clipBehavior = Clip.none, global::Doroti.Framework.Widgets.FocusNode? focusNode = null, bool autofocus = false, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, MaterialTapTargetSize? materialTapTargetSize = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, Color? selectedShadowColor = null, bool? showCheckmark = null, Color? checkmarkColor = null, global::Doroti.Framework.Painting.ShapeBorder avatarBorder = default!, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null, ChipAnimationStyle? chipAnimationStyle = null, global::Doroti.Framework.Services.MouseCursor? mouseCursor = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.ShapeBorder __avatarBorder = avatarBorder ?? new global::Doroti.Framework.Painting.CircleBorder();
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _RawChipState__chip());
}

internal class _RawChipState__chip : global::Doroti.Framework.Widgets.State<RawChip>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<RawChip>
{
    public static Duration pressedAnimationDuration = Duration.Create(milliseconds: 75L);
    public virtual global::Doroti.Framework.Animation.AnimationController selectController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController avatarDrawerController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController deleteDrawerController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController enableController { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation checkmarkAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation avatarDrawerAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation deleteDrawerAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation enableAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.CurvedAnimation selectionFade { get; set; } = default!;
    public virtual global::Doroti.Framework.Widgets.WidgetStatesController statesController { get; private set; } = new global::Doroti.Framework.Widgets.WidgetStatesController();
    internal virtual bool _isTapping { get; set; } = false;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public virtual bool hasDeleteButton => DartRuntimePrimitives.ConvertValue<bool>(widget.onDeleted is not null);
    public virtual bool hasAvatar => DartRuntimePrimitives.ConvertValue<bool>(widget.avatar is not null);
    public virtual bool canTap
    {
        get
        {
            return widget.isEnabled && widget.tapEnabled && ((widget.onPressed is not null) || (widget.onSelected is not null));
        }
    }
    public virtual bool isTapping => DartRuntimePrimitives.ConvertValue<bool>(canTap && _isTapping);
    public override void initState()
    {
        DartRuntimePrimitives.Assert(() => (widget.onSelected is null) || (widget.onPressed is null));
        base.initState();
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Widgets.WidgetStatesController>)(() =>
{
    var __cascade = statesController;
    __cascade.update(WidgetState.disabled, !widget.isEnabled);
    __cascade.update(WidgetState.selected, widget.selected);
    __cascade.addListener(() =>
    {
        setState(() =>
        {
        });
    });
    return __cascade;
}))());
        selectController = new global::Doroti.Framework.Animation.AnimationController(duration: widget.chipAnimationStyle?.selectAnimation?.duration ?? ChipLibrary._kSelectDuration, reverseDuration: widget.chipAnimationStyle?.selectAnimation?.reverseDuration, value: widget.selected ? 1.0 : 0.0, vsync: this);
        selectionFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: selectController, curve: Curves.fastOutSlowIn);
        avatarDrawerController = new global::Doroti.Framework.Animation.AnimationController(duration: widget.chipAnimationStyle?.avatarDrawerAnimation?.duration ?? ChipLibrary._kDrawerDuration, reverseDuration: widget.chipAnimationStyle?.avatarDrawerAnimation?.reverseDuration, value: (hasAvatar || widget.selected) ? 1.0 : 0.0, vsync: this);
        deleteDrawerController = new global::Doroti.Framework.Animation.AnimationController(duration: widget.chipAnimationStyle?.deleteDrawerAnimation?.duration ?? ChipLibrary._kDrawerDuration, reverseDuration: widget.chipAnimationStyle?.deleteDrawerAnimation?.reverseDuration, value: hasDeleteButton ? 1.0 : 0.0, vsync: this);
        enableController = new global::Doroti.Framework.Animation.AnimationController(duration: widget.chipAnimationStyle?.enableAnimation?.duration ?? ChipLibrary._kDisableDuration, reverseDuration: widget.chipAnimationStyle?.enableAnimation?.reverseDuration, value: widget.isEnabled ? 1.0 : 0.0, vsync: this);
        double checkmarkPercentage = ChipLibrary._kCheckmarkDuration.inMilliseconds / ChipLibrary._kSelectDuration.inMilliseconds;
        double checkmarkReversePercentage = ChipLibrary._kCheckmarkReverseDuration.inMilliseconds / ChipLibrary._kSelectDuration.inMilliseconds;
        double avatarDrawerReversePercentage = ChipLibrary._kReverseDrawerDuration.inMilliseconds / ChipLibrary._kSelectDuration.inMilliseconds;
        checkmarkAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: selectController, curve: new global::Doroti.Framework.Animation.Interval(1.0 - checkmarkPercentage, 1.0, curve: Curves.fastOutSlowIn), reverseCurve: new global::Doroti.Framework.Animation.Interval(1.0 - checkmarkReversePercentage, 1.0, curve: Curves.fastOutSlowIn));
        deleteDrawerAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: deleteDrawerController, curve: Curves.fastOutSlowIn);
        avatarDrawerAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: avatarDrawerController, curve: Curves.fastOutSlowIn, reverseCurve: new global::Doroti.Framework.Animation.Interval(1.0 - avatarDrawerReversePercentage, 1.0, curve: Curves.fastOutSlowIn));
        enableAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: enableController, curve: Curves.fastOutSlowIn);
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
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
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

    internal virtual global::Doroti.Framework.Painting.OutlinedBorder _getShape(ThemeData theme, ChipThemeData chipTheme, ChipThemeData chipDefaults)
    {
        global::Doroti.Framework.Painting.BorderSide? resolvedSide = WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.BorderSide?>(widget.side, statesController.value) ?? WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.BorderSide?>(chipTheme.side, statesController.value);
        global::Doroti.Framework.Painting.OutlinedBorder resolvedShape = ((WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.OutlinedBorder?>(widget.shape, statesController.value) ?? WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.OutlinedBorder?>(chipTheme.shape, statesController.value)) ?? WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.OutlinedBorder?>(chipDefaults.shape, statesController.value)) ?? new global::Doroti.Framework.Painting.StadiumBorder();
        if (resolvedSide is not null)
        {
            return resolvedShape.copyWith(side: resolvedSide);
        }
        return (!Equals(resolvedShape.side, BorderSide.none)) ? resolvedShape : resolvedShape.copyWith(side: chipDefaults.side);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color? resolveColor(global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? selectedColor = null, Color? backgroundColor = null, Color? disabledColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? defaultColor = null)
    {
        return new _IndividualOverrides__chip(color: color, selectedColor: selectedColor, backgroundColor: backgroundColor, disabledColor: disabledColor).resolve(statesController.value) ?? (defaultColor?.resolve(statesController.value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color? _getBackgroundColor(ThemeData theme, ChipThemeData chipTheme, ChipThemeData chipDefaults)
    {
        {
            global::Doroti.Ui.Color? disabledColorLocal = resolveColor(color: widget.color ?? chipTheme.color, disabledColor: widget.disabledColor ?? chipTheme.disabledColor, defaultColor: chipDefaults.color);
            global::Doroti.Ui.Color? backgroundColorLocal = resolveColor(color: widget.color ?? chipTheme.color, backgroundColor: widget.backgroundColor ?? chipTheme.backgroundColor, defaultColor: chipDefaults.color);
            global::Doroti.Ui.Color? selectedColorLocal = resolveColor(color: widget.color ?? chipTheme.color, selectedColor: widget.selectedColor ?? chipTheme.selectedColor, defaultColor: chipDefaults.color);
            var backgroundTween = new global::Doroti.Framework.Animation.ColorTween(begin: disabledColorLocal, end: backgroundColorLocal);
            var selectTween = new global::Doroti.Framework.Animation.ColorTween(begin: backgroundTween.evaluate(enableController), end: selectedColorLocal);
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

    internal virtual global::Doroti.Framework.Widgets.Widget? _wrapWithTooltip(string? tooltip = null, bool enabled = true, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        if ((child is null) || !enabled || (tooltip is null))
        {
            return child;
        }
        return (global::Doroti.Framework.Widgets.Widget?)new Tooltip(message: tooltip, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget? _buildDeleteIcon(global::Doroti.Framework.Widgets.BuildContext context, ThemeData theme, ChipThemeData chipTheme, ChipThemeData chipDefaults)
    {
        if (!hasDeleteButton)
        {
            return null;
        }
        global::Doroti.Framework.Widgets.IconThemeData iconThemeLocal = ((widget.iconTheme ?? chipTheme.iconTheme) ?? theme.chipTheme.iconTheme) ?? new _ChipDefaultsM3__chip(context, widget.isEnabled).iconTheme!;
        global::Doroti.Ui.Color? effectiveDeleteIconColor = WidgetStateProperty.resolveAs(((((widget.deleteIconColor ?? chipTheme.deleteIconColor) ?? theme.chipTheme.deleteIconColor) ?? widget.iconTheme?.color) ?? chipTheme.iconTheme?.color) ?? chipDefaults.deleteIconColor, statesController.value);
        double effectiveIconSize = ((widget.iconTheme?.size ?? chipTheme.iconTheme?.size) ?? theme.chipTheme.iconTheme?.size) ?? DartRuntimePrimitives.RequireValue(new _ChipDefaultsM3__chip(context, widget.isEnabled).iconTheme!.size);
        MaterialTapTargetSize effectiveMaterialTapTargetSize = widget.materialTapTargetSize ?? theme.materialTapTargetSize;
        global::Doroti.Ui.Size semanticSizeLocal = effectiveMaterialTapTargetSize switch { var __constant45576 when Equals(__constant45576, MaterialTapTargetSize.padded) => new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimension), var __constant45659 when Equals(__constant45659, MaterialTapTargetSize.shrinkWrap) => new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimension - 8.0), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        VisualDensity effectiveVisualDensity = widget.visualDensity ?? theme.visualDensity;
        return (global::Doroti.Framework.Widgets.Widget?)new _EnsureMinSemanticsSize__chip(semanticSize: semanticSizeLocal + effectiveVisualDensity.baseSizeAdjustment, child: _wrapWithTooltip(tooltip: widget.deleteButtonTooltipMessage ?? MaterialLocalizations.of(context).deleteButtonTooltip, enabled: widget.isEnabled && (widget.onDeleted is not null), child: new InkWell(radius: (ChipLibrary._kChipHeight + (widget.padding?.vertical ?? 0.0)) * 0.45, splashFactory: new _UnconstrainedInkSplashFactory__chip(Theme.of(context).splashFactory), customBorder: new global::Doroti.Framework.Painting.CircleBorder(), onTap: widget.isEnabled ? widget.onDeleted : null, child: new global::Doroti.Framework.Widgets.IconTheme(data: iconThemeLocal.copyWith(color: effectiveDeleteIconColor, size: effectiveIconSize), child: widget.deleteIcon))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData themeLocal = Theme.of(context);
        ChipThemeData chipTheme = ChipTheme.of(context);
        global::Doroti.Ui.Brightness brightnessLocal = chipTheme.brightness ?? themeLocal.brightness;
        ChipThemeData chipDefaults = widget.defaultProperties ?? new _ChipDefaultsM3__chip(context, widget.isEnabled);
        global::Doroti.Ui.TextDirection? textDirection = Directionality.maybeOf(context);
        global::Doroti.Framework.Painting.OutlinedBorder resolvedShape = _getShape(themeLocal, chipTheme, chipDefaults);
        double elevationLocal = ((widget.elevation ?? chipTheme.elevation) ?? chipDefaults.elevation) ?? 0;
        double pressElevationLocal = ((widget.pressElevation ?? chipTheme.pressElevation) ?? chipDefaults.pressElevation) ?? 0;
        global::Doroti.Ui.Color? shadowColorLocal = (widget.shadowColor ?? chipTheme.shadowColor) ?? chipDefaults.shadowColor;
        global::Doroti.Ui.Color? surfaceTintColorLocal = (widget.surfaceTintColor ?? chipTheme.surfaceTintColor) ?? chipDefaults.surfaceTintColor;
        global::Doroti.Ui.Color? selectedShadowColorLocal = (widget.selectedShadowColor ?? chipTheme.selectedShadowColor) ?? chipDefaults.selectedShadowColor;
        global::Doroti.Ui.Color? checkmarkColorLocal = (widget.checkmarkColor ?? chipTheme.checkmarkColor) ?? chipDefaults.checkmarkColor;
        bool showCheckmarkLocal = (widget.showCheckmark ?? chipTheme.showCheckmark) ?? DartRuntimePrimitives.RequireValue(chipDefaults.showCheckmark);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = (widget.padding ?? chipTheme.padding) ?? chipDefaults.padding!;
        global::Doroti.Framework.Painting.TextStyle labelStyleLocal = chipTheme.labelStyle ?? chipDefaults.labelStyle!;
        global::Doroti.Framework.Widgets.IconThemeData? iconThemeLocal = (widget.iconTheme ?? chipTheme.iconTheme) ?? chipDefaults.iconTheme;
        global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraintsLocal = widget.avatarBoxConstraints ?? chipTheme.avatarBoxConstraints;
        global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraintsLocal = widget.deleteIconBoxConstraints ?? chipTheme.deleteIconBoxConstraints;
        global::Doroti.Framework.Painting.TextStyle effectiveLabelStyle = labelStyleLocal.merge(widget.labelStyle);
        global::Doroti.Ui.Color? resolvedLabelColor = WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(effectiveLabelStyle.color, statesController.value);
        global::Doroti.Framework.Painting.TextStyle resolvedLabelStyle = effectiveLabelStyle.copyWith(color: resolvedLabelColor);
        global::Doroti.Framework.Widgets.Widget? avatarLocal = ((iconThemeLocal is not null) && hasAvatar) ? IconTheme.merge(data: chipDefaults.iconTheme!.merge(iconThemeLocal), child: widget.avatar!) : widget.avatar;
        double defaultFontSize = effectiveLabelStyle.fontSize ?? 14.0;
        double effectiveTextScale = MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry defaultLabelPadding = EdgeInsets.lerp(EdgeInsets.CreateSymmetric(horizontal: 8.0), EdgeInsets.CreateSymmetric(horizontal: 4.0), Dart_uiLibrary.clampDouble(effectiveTextScale - 1.0, 0.0, 1.0))!;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry labelPaddingLocal = ((widget.labelPadding ?? chipTheme.labelPadding) ?? chipDefaults.labelPadding) ?? defaultLabelPadding;
        global::Doroti.Framework.Widgets.Widget result = new Material(elevation: isTapping ? pressElevationLocal : elevationLocal, shadowColor: widget.selected ? selectedShadowColorLocal : shadowColorLocal, surfaceTintColor: surfaceTintColorLocal, animationDuration: pressedAnimationDuration, shape: resolvedShape, clipBehavior: widget.clipBehavior, child: new InkWell(onFocusChange: (value) =>
        {
            statesController.update(WidgetState.focused, value);
        }, focusNode: widget.focusNode, autofocus: widget.autofocus, canRequestFocus: widget.isEnabled, onTap: canTap ? _handleTap : null, onTapDown: canTap ? _handleTapDown : null, onTapCancel: canTap ? _handleTapCancel : null, onHover: canTap ? ((value) =>
        {
            statesController.update(WidgetState.hovered, value);
        }) : null, mouseCursor: widget.mouseCursor, hoverColor: ((widget.color ?? chipTheme.color) is null) ? null : Colors.transparent, customBorder: resolvedShape, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: Listenable.CreateMerge(new List<global::Doroti.Framework.Foundation.Listenable> { selectController, enableController }.Cast<global::Doroti.Framework.Foundation.Listenable?>()), builder: (context, child) =>
        {
            return new Ink(decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: resolvedShape, color: _getBackgroundColor(themeLocal, chipTheme, chipDefaults)), child: child);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: _wrapWithTooltip(tooltip: widget.tooltip, enabled: (widget.onPressed is not null) || (widget.onSelected is not null), child: new _ChipRenderWidget__chip(theme: new _ChipRenderTheme__chip(label: new global::Doroti.Framework.Widgets.DefaultTextStyle(overflow: TextOverflow.fade, textAlign: TextAlign.start, maxLines: 1L, softWrap: false, style: resolvedLabelStyle, child: widget.label), avatar: new global::Doroti.Framework.Widgets.AnimatedSwitcher(duration: ChipLibrary._kDrawerDuration, switchInCurve: Curves.fastOutSlowIn, child: avatarLocal), deleteIcon: new global::Doroti.Framework.Widgets.AnimatedSwitcher(duration: ChipLibrary._kDrawerDuration, switchInCurve: Curves.fastOutSlowIn, child: _buildDeleteIcon(context, themeLocal, chipTheme, chipDefaults)), brightness: brightnessLocal, padding: paddingLocal.resolve(textDirection), visualDensity: widget.visualDensity ?? themeLocal.visualDensity, labelPadding: labelPaddingLocal.resolve(textDirection), showAvatar: hasAvatar, showCheckmark: showCheckmarkLocal, checkmarkColor: checkmarkColorLocal, canTapBody: canTap), value: widget.selected, checkmarkAnimation: checkmarkAnimation, enableAnimation: enableAnimation, avatarDrawerAnimation: avatarDrawerAnimation, deleteDrawerAnimation: deleteDrawerAnimation, isEnabled: widget.isEnabled, avatarBorder: widget.avatarBorder, avatarBoxConstraints: avatarBoxConstraintsLocal, deleteIconBoxConstraints: deleteIconBoxConstraintsLocal)))));
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = default!;
        global::Doroti.Ui.Offset densityAdjustment = (widget.visualDensity ?? themeLocal.visualDensity).baseSizeAdjustment;
        switch (widget.materialTapTargetSize ?? themeLocal.materialTapTargetSize)
        {
            case var __constant54393 when Equals(__constant54393, MaterialTapTargetSize.padded):
                {
                    constraintsLocal = new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dx, minHeight: ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy);
                    break;
                }
            case var __constant54622 when Equals(__constant54622, MaterialTapTargetSize.shrinkWrap):
                {
                    constraintsLocal = new global::Doroti.Framework.Rendering.BoxConstraints();
                    break;
                }
        }
        result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _ChipRedirectingHitDetectionWidget__chip(constraints: constraintsLocal, child: new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: result)));
        return new global::Doroti.Framework.Widgets.Semantics(button: widget.tapEnabled, container: true, selected: Foundation.ConstantsLibrary.kIsWeb ? null : widget.selected, @checked: Foundation.ConstantsLibrary.kIsWeb ? widget.selected : null, enabled: widget.tapEnabled ? canTap : null, child: result);
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
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

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _IndividualOverrides__chip : global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>
{
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }

    internal _IndividualOverrides__chip(global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, Color? selectedColor = null, Color? disabledColor = null)
    {
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.selectedColor = selectedColor;
        this.disabledColor = disabledColor;
    }

    public virtual Color? resolve(HashSet<global::Doroti.Framework.Widgets.WidgetState> states)
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
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ChipRedirectingHitDetectionWidget__chip : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual global::Doroti.Framework.Rendering.BoxConstraints constraints { get; private set; } = default!;

    internal _ChipRedirectingHitDetectionWidget__chip(global::Doroti.Framework.Widgets.Widget? child = null, global::Doroti.Framework.Rendering.BoxConstraints constraints = default!) : base(child: child)
    {
        this.constraints = constraints;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderChipRedirectingHitDetection__chip(constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderChipRedirectingHitDetection__chip)renderObject;
        __renderObject.additionalConstraints = constraints;
    }

}

public class _RenderChipRedirectingHitDetection__chip : global::Doroti.Framework.Rendering.RenderConstrainedBox
{
    internal _RenderChipRedirectingHitDetection__chip(global::Doroti.Framework.Rendering.BoxConstraints additionalConstraints) : base(additionalConstraints: additionalConstraints)
    {
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (!size.contains(position))
        {
            return false;
        }
        var offset = new global::Doroti.Ui.Offset(position.dx, size.height / 2L);
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(offset), position: position, hitTest: (result, position) =>
        {
            DartRuntimePrimitives.Assert(() => Equals(position, offset));
            return child!.hitTest(result, position: offset);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ChipRenderWidget__chip : global::Doroti.Framework.Widgets.SlottedMultiChildRenderObjectWidget<_ChipSlot__chip, global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual _ChipRenderTheme__chip theme { get; private set; } = default!;
    public virtual bool? value { get; private set; }
    public virtual bool? isEnabled { get; private set; }
    public virtual global::Doroti.Framework.Animation.Animation<double> checkmarkAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> avatarDrawerAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> deleteDrawerAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> enableAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? avatarBorder { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints { get; private set; }

    internal _ChipRenderWidget__chip(_ChipRenderTheme__chip theme, bool? value = null, bool? isEnabled = null, global::Doroti.Framework.Animation.Animation<double> checkmarkAnimation = default!, global::Doroti.Framework.Animation.Animation<double> avatarDrawerAnimation = default!, global::Doroti.Framework.Animation.Animation<double> deleteDrawerAnimation = default!, global::Doroti.Framework.Animation.Animation<double> enableAnimation = default!, global::Doroti.Framework.Painting.ShapeBorder? avatarBorder = null, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null)
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

    public override IEnumerable<_ChipSlot__chip> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_ChipSlot__chip>>(Enum.GetValues<_ChipSlot__chip>().ToList());
    public override global::Doroti.Framework.Widgets.Widget? childForSlot(_ChipSlot__chip slot)
    {
        return slot switch { _ChipSlot__chip.label => theme.label, _ChipSlot__chip.avatar => theme.avatar, _ChipSlot__chip.deleteIcon => theme.deleteIcon, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderChip__chip)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderChip__chip>)(() =>
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
}))());
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderChip__chip(theme: theme, textDirection: Directionality.of(context), value: value, isEnabled: isEnabled, checkmarkAnimation: checkmarkAnimation, avatarDrawerAnimation: avatarDrawerAnimation, deleteDrawerAnimation: deleteDrawerAnimation, enableAnimation: enableAnimation, avatarBorder: avatarBorder, avatarBoxConstraints: avatarBoxConstraints, deleteIconBoxConstraints: deleteIconBoxConstraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public enum _ChipSlot__chip
{
    label,
    avatar,
    deleteIcon
}

public class _ChipRenderTheme__chip
{
    public virtual global::Doroti.Framework.Widgets.Widget avatar { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget label { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget deleteIcon { get; private set; } = default!;
    public virtual Brightness brightness { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets padding { get; private set; } = default!;
    public virtual VisualDensity visualDensity { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsets labelPadding { get; private set; } = default!;
    public virtual bool showAvatar { get; private set; } = default!;
    public virtual bool showCheckmark { get; private set; } = default!;
    public virtual Color? checkmarkColor { get; private set; }
    public virtual bool canTapBody { get; private set; } = default!;

    internal _ChipRenderTheme__chip(global::Doroti.Framework.Widgets.Widget avatar, global::Doroti.Framework.Widgets.Widget label, global::Doroti.Framework.Widgets.Widget deleteIcon, Brightness brightness, global::Doroti.Framework.Painting.EdgeInsets padding, VisualDensity visualDensity, global::Doroti.Framework.Painting.EdgeInsets labelPadding, bool showAvatar, bool showCheckmark, Color? checkmarkColor, bool canTapBody)
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
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _ChipRenderTheme__chip) && Equals(__other.avatar, avatar) && Equals(__other.label, label) && Equals(__other.deleteIcon, deleteIcon) && Equals(__other.brightness, brightness) && Equals(__other.padding, padding) && Equals(__other.labelPadding, labelPadding) && (__other.showAvatar == showAvatar) && (__other.showCheckmark == showCheckmark) && Equals(__other.checkmarkColor, checkmarkColor) && (__other.canTapBody == canTapBody);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(avatar, label, deleteIcon, brightness, padding, labelPadding, showAvatar, showCheckmark, checkmarkColor, canTapBody));
}

public class _RenderChip__chip : global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Widgets.SlottedContainerRenderObjectMixin<_ChipSlot__chip, global::Doroti.Framework.Rendering.RenderBox>
{
    public virtual bool? value { get; set; } = default;
    public virtual bool? isEnabled { get; set; } = default;
    internal virtual Rect _deleteButtonRect { get; set; } = default!;
    internal virtual Rect _pressRect { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> checkmarkAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> avatarDrawerAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> deleteDrawerAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> enableAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? avatarBorder { get; set; } = default;
    internal virtual _ChipRenderTheme__chip _theme { get; set; } = default!;
    internal virtual TextDirection _textDirection { get; set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints? _avatarBoxConstraints { get; set; } = default;
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints? _deleteIconBoxConstraints { get; set; } = default;
    public static global::Doroti.Framework.Animation.ColorTween selectionScrimTween = new global::Doroti.Framework.Animation.ColorTween(begin: Colors.transparent, end: ChipLibrary._kSelectScrimColor);
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer> _avatarOpacityLayerHandler { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer>();
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer> _labelOpacityLayerHandler { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer>();
    internal virtual global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer> _deleteIconOpacityLayerHandler { get; private set; } = new global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer>();
    internal const bool _debugShowTapTargetOutlines = false;
    public virtual DartMap<_ChipSlot__chip, global::Doroti.Framework.Rendering.RenderBox> _slotToChild { get; set; } = new DartMap<_ChipSlot__chip, global::Doroti.Framework.Rendering.RenderBox>();

    internal _RenderChip__chip(_ChipRenderTheme__chip theme, TextDirection textDirection, bool? value = null, bool? isEnabled = null, global::Doroti.Framework.Animation.Animation<double> checkmarkAnimation = default!, global::Doroti.Framework.Animation.Animation<double> avatarDrawerAnimation = default!, global::Doroti.Framework.Animation.Animation<double> deleteDrawerAnimation = default!, global::Doroti.Framework.Animation.Animation<double> enableAnimation = default!, global::Doroti.Framework.Painting.ShapeBorder? avatarBorder = null, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null)
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

    public virtual global::Doroti.Framework.Rendering.RenderBox avatar => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(childForSlot(_ChipSlot__chip.avatar)!);
    public virtual global::Doroti.Framework.Rendering.RenderBox deleteIcon => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(childForSlot(_ChipSlot__chip.deleteIcon)!);
    public virtual global::Doroti.Framework.Rendering.RenderBox label => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(childForSlot(_ChipSlot__chip.label)!);
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
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => _textDirection;
        set
        {
            var __value = value;
            if (Equals(_textDirection, DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(__value));
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints
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
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints
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
    public virtual IEnumerable<global::Doroti.Framework.Rendering.RenderBox> children
    {
        get
        {
            global::Doroti.Framework.Rendering.RenderBox? avatarLocal = childForSlot(_ChipSlot__chip.avatar);
            global::Doroti.Framework.Rendering.RenderBox? labelLocal = childForSlot(_ChipSlot__chip.label);
            global::Doroti.Framework.Rendering.RenderBox? deleteIconLocal = childForSlot(_ChipSlot__chip.deleteIcon);
            return ((Func<List<global::Doroti.Framework.Rendering.RenderBox>>)(() => { var __collection64442 = new List<global::Doroti.Framework.Rendering.RenderBox>(); var __collectionElement64454 = avatarLocal; if (__collectionElement64454 is { } __nonNullCollectionElement64454) { __collection64442.Add(__nonNullCollectionElement64454); } var __collectionElement64463 = labelLocal; if (__collectionElement64463 is { } __nonNullCollectionElement64463) { __collection64442.Add(__nonNullCollectionElement64463); } var __collectionElement64471 = deleteIconLocal; if (__collectionElement64471 is { } __nonNullCollectionElement64471) { __collection64442.Add(__nonNullCollectionElement64471); } return __collection64442; }))();
        }
    }
    public virtual bool isDrawingCheckmark => DartRuntimePrimitives.ConvertValue<bool>(theme.showCheckmark && !checkmarkAnimation.isDismissed);
    public virtual bool deleteIconShowing => !deleteDrawerAnimation.isDismissed;
    internal static global::Doroti.Ui.Rect _boxRect(global::Doroti.Framework.Rendering.RenderBox box) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Rect>(_boxParentData(box).offset & box.size);
    internal static global::Doroti.Framework.Rendering.BoxParentData _boxParentData(global::Doroti.Framework.Rendering.RenderBox box) => ((global::Doroti.Framework.Rendering.BoxParentData?)box.parentData!)!;
    public override double computeMinIntrinsicWidth(double height)
    {
        double overallPadding = theme.padding.horizontal + theme.labelPadding.horizontal;
        return overallPadding + avatar.getMinIntrinsicWidth(height) + label.getMinIntrinsicWidth(height) + deleteIcon.getMinIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double overallPadding = theme.padding.horizontal + theme.labelPadding.horizontal;
        return overallPadding + avatar.getMaxIntrinsicWidth(height) + label.getMaxIntrinsicWidth(height) + deleteIcon.getMaxIntrinsicWidth(height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return Math.Max(ChipLibrary._kChipHeight, theme.padding.vertical + theme.labelPadding.vertical + label.getMinIntrinsicHeight(width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width) => getMinIntrinsicHeight(width);
    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return new global::Doroti.Framework.Rendering.BaselineOffset(label.getDistanceToActualBaseline(baseline)).op_Add(_boxParentData(label).offset.dy).offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _labelConstraintsFrom(global::Doroti.Framework.Rendering.BoxConstraints contentConstraints, double iconWidth, double contentSize, Size rawLabelSize)
    {
        double freeSpace = contentConstraints.maxWidth - iconWidth - theme.labelPadding.horizontal - theme.padding.horizontal;
        double maxLabelWidth = Math.Max(0.0, freeSpace);
        return new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: rawLabelSize.height, maxHeight: contentSize, maxWidth: double.IsFinite(maxLabelWidth) ? maxLabelWidth : rawLabelSize.width);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _layoutAvatar(double contentSize, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild = default!)
    {
        global::Doroti.Framework.Rendering.BoxConstraints avatarConstraints = avatarBoxConstraints ?? BoxConstraints.CreateTightFor(width: contentSize, height: contentSize);
        global::Doroti.Ui.Size avatarBoxSize = layoutChild(avatar, avatarConstraints);
        if (!theme.showCheckmark && !theme.showAvatar)
        {
            return new global::Doroti.Ui.Size(0.0, contentSize);
        }
        double avatarFullWidth = theme.showAvatar ? avatarBoxSize.width : contentSize;
        return new global::Doroti.Ui.Size(avatarFullWidth * avatarDrawerAnimation.value, avatarBoxSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _layoutDeleteIcon(double contentSize, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild = default!)
    {
        global::Doroti.Framework.Rendering.BoxConstraints deleteIconConstraints = deleteIconBoxConstraints ?? BoxConstraints.CreateTightFor(width: contentSize, height: contentSize);
        global::Doroti.Ui.Size boxSize = layoutChild(deleteIcon, deleteIconConstraints);
        if (!deleteIconShowing)
        {
            return new global::Doroti.Ui.Size(0.0, contentSize);
        }
        return new global::Doroti.Ui.Size(deleteDrawerAnimation.value * boxSize.width, boxSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (!size.contains(position))
        {
            return false;
        }
        bool hitIsOnDeleteIcon = ChipLibrary._hitIsOnDeleteIcon(padding: theme.padding, labelPadding: theme.labelPadding, tapPosition: position, chipSize: size, deleteButtonSize: deleteIcon.size, textDirection: textDirection);
        global::Doroti.Framework.Rendering.RenderBox hitTestChild = hitIsOnDeleteIcon ? deleteIcon : label;
        global::Doroti.Ui.Offset centerLocal = hitTestChild.size.center(Offset.zero);
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(centerLocal), position: position, hitTest: (result, position) =>
        {
            DartRuntimePrimitives.Assert(() => Equals(position, centerLocal));
            return hitTestChild.hitTest(result, position: centerLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSizes(constraints, ChildLayoutHelper.dryLayoutChild).size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        _ChipSizes__chip sizes = _computeSizes(constraints, ChildLayoutHelper.dryLayoutChild);
        global::Doroti.Framework.Rendering.BaselineOffset labelBaseline = new global::Doroti.Framework.Rendering.BaselineOffset(label.getDryBaseline(sizes.labelConstraints, baseline)).op_Add((sizes.content - sizes.label.height + sizes.densityAdjustment.dy) / 2L).op_Add(theme.padding.top).op_Add(theme.labelPadding.top);
        return labelBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _ChipSizes__chip _computeSizes(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        global::Doroti.Framework.Rendering.BoxConstraints contentConstraints = constraints.loosen();
        global::Doroti.Ui.Size rawLabelSize = label.getDryLayout(contentConstraints);
        double contentSize = Math.Max(ChipLibrary._kChipHeight - theme.padding.vertical + theme.labelPadding.vertical, rawLabelSize.height + theme.labelPadding.vertical);
        DartRuntimePrimitives.Assert(() => contentSize >= rawLabelSize.height);
        global::Doroti.Ui.Size avatarSize = _layoutAvatar(contentSize, layoutChild);
        global::Doroti.Ui.Size deleteIconSize = _layoutDeleteIcon(contentSize, layoutChild);
        global::Doroti.Framework.Rendering.BoxConstraints labelConstraintsLocal = _labelConstraintsFrom(contentConstraints, avatarSize.width + deleteIconSize.width, contentSize, rawLabelSize);
        global::Doroti.Ui.Size labelSize = theme.labelPadding.inflateSize(layoutChild(label, labelConstraintsLocal));
        var densityAdjustmentLocal = new global::Doroti.Ui.Offset(0.0, theme.visualDensity.baseSizeAdjustment.dy / 2.0);
        global::Doroti.Ui.Size overallSize = new global::Doroti.Ui.Size(avatarSize.width + labelSize.width + deleteIconSize.width, contentSize) + densityAdjustmentLocal;
        var paddedSize = new global::Doroti.Ui.Size(overallSize.width + theme.padding.horizontal, overallSize.height + theme.padding.vertical);
        return new _ChipSizes__chip(size: constraints.constrain(paddedSize), overall: overallSize, content: contentSize, densityAdjustment: densityAdjustmentLocal, avatar: avatarSize, labelConstraints: labelConstraintsLocal, label: labelSize, deleteIcon: deleteIconSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
            return new global::Doroti.Ui.Offset(x, (sizes.content - boxSize.height + sizes.densityAdjustment.dy) / 2.0);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Offset avatarOffset = Offset.zero;
        global::Doroti.Ui.Offset labelOffset = Offset.zero;
        global::Doroti.Ui.Offset deleteIconOffset = Offset.zero;
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
                        _deleteButtonRect = Rect.fromLTWH(0.0, 0.0, sizes.deleteIcon.width + theme.padding.right, sizes.overall.height + theme.padding.vertical);
                        deleteIconOffset = centerLayout(sizes.deleteIcon, start);
                    }
                    else
                    {
                        _deleteButtonRect = Rect.zero;
                    }
                    start -= sizes.deleteIcon.width;
                    if (theme.canTapBody)
                    {
                        _pressRect = Rect.fromLTWH(_deleteButtonRect.width, 0.0, sizes.overall.width - _deleteButtonRect.width + theme.padding.horizontal, sizes.overall.height + theme.padding.vertical);
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
                        avatarOffset = centerLayout(sizes.avatar, startLocal - avatar.size.width + sizes.avatar.width);
                        startLocal += sizes.avatar.width;
                    }
                    labelOffset = centerLayout(sizes.label, startLocal);
                    startLocal += sizes.label.width;
                    if (theme.canTapBody)
                    {
                        _pressRect = Rect.fromLTWH(0.0, 0.0, deleteIconShowing ? (startLocal + theme.padding.left) : (sizes.overall.width + theme.padding.horizontal), sizes.overall.height + theme.padding.vertical);
                    }
                    else
                    {
                        _pressRect = Rect.zero;
                    }
                    startLocal -= deleteIcon.size.width - sizes.deleteIcon.width;
                    if (deleteIconShowing)
                    {
                        deleteIconOffset = centerLayout(sizes.deleteIcon, startLocal);
                        _deleteButtonRect = Rect.fromLTWH(startLocal + theme.padding.left, 0.0, sizes.deleteIcon.width + theme.padding.right, sizes.overall.height + theme.padding.vertical);
                    }
                    else
                    {
                        _deleteButtonRect = Rect.zero;
                    }
                    break;
                }
        }
        labelOffset = labelOffset + new global::Doroti.Ui.Offset(0.0, (sizes.label.height - theme.labelPadding.vertical - label.size.height) / 2.0);
        _boxParentData(avatar).offset = theme.padding.topLeft + avatarOffset;
        _boxParentData(label).offset = theme.padding.topLeft + labelOffset + theme.labelPadding.topLeft;
        _boxParentData(deleteIcon).offset = theme.padding.topLeft + deleteIconOffset;
        var paddedSize = new global::Doroti.Ui.Size(sizes.overall.width + theme.padding.horizontal, sizes.overall.height + theme.padding.vertical);
        size = constraints.constrain(paddedSize);
        DartRuntimePrimitives.Assert(() => size.height == constraints.constrainHeight(paddedSize.height), () => (object?)$"Constrained height {size.height} doesn't match expected height " + $"{constraints.constrainWidth(paddedSize.height)}");
        DartRuntimePrimitives.Assert(() => size.width == constraints.constrainWidth(paddedSize.width), () => (object?)$"Constrained width {size.width} doesn't match expected width " + $"{constraints.constrainWidth(paddedSize.width)}");
    }

    internal virtual global::Doroti.Ui.Color _disabledColor
    {
        get
        {
            if (enableAnimation.isCompleted)
            {
                return Colors.white;
            }
            global::Doroti.Ui.Color color = theme.brightness switch { Brightness.light => Colors.white, Brightness.dark => Colors.black, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            return new global::Doroti.Framework.Animation.ColorTween(begin: color.withAlpha(ChipLibrary._kDisabledAlpha), end: color).evaluate(enableAnimation)!;
        }
    }
    internal virtual void _paintCheck(Canvas canvas, Offset origin, double size)
    {
        global::Doroti.Ui.Color? paintColor = (global::Doroti.Ui.Color?)(theme.checkmarkColor ?? ((theme.brightness, theme.showAvatar) switch { (Brightness.light, true) => Colors.white, (Brightness.light, false) => Colors.black.withAlpha(ChipLibrary._kCheckmarkAlpha), (Brightness.dark, true) => Colors.black, (Brightness.dark, false) => Colors.white.withAlpha(ChipLibrary._kCheckmarkAlpha) }));
        var fadeTween = new global::Doroti.Framework.Animation.ColorTween(begin: Colors.transparent, end: paintColor);
        paintColor = Equals(checkmarkAnimation.status, AnimationStatus.reverse) ? fadeTween.evaluate(checkmarkAnimation) : paintColor;
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = paintColor!;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = ChipLibrary._kCheckmarkStrokeWidth * avatar.size.height / 24.0;
    return __cascade;
}))();
        double t = Equals(checkmarkAnimation.status, AnimationStatus.reverse) ? 1.0 : checkmarkAnimation.value;
        if (t == 0.0)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => (t > 0.0) && (t <= 1.0));
        var path = new global::Doroti.Ui.Path();
        var start = new global::Doroti.Ui.Offset(size * 0.15, size * 0.45);
        var mid = new global::Doroti.Ui.Offset(size * 0.4, size * 0.7);
        var endLocal = new global::Doroti.Ui.Offset(size * 0.85, size * 0.25);
        if (t < 0.5)
        {
            double strokeT = t * 2.0;
            global::Doroti.Ui.Offset drawMid = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(start, mid, strokeT));
            path.moveTo(origin.dx + start.dx, origin.dy + start.dy);
            path.lineTo(origin.dx + drawMid.dx, origin.dy + drawMid.dy);
        }
        else
        {
            double strokeTLocal = (t - 0.5) * 2.0;
            global::Doroti.Ui.Offset drawEnd = DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(mid, endLocal, strokeTLocal));
            path.moveTo(origin.dx + start.dx, origin.dy + start.dy);
            path.lineTo(origin.dx + mid.dx, origin.dy + mid.dy);
            path.lineTo(origin.dx + drawEnd.dx, origin.dy + drawEnd.dy);
        }
        canvas.drawPath(path, paint);
    }

    internal virtual void _paintSelectionOverlay(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (isDrawingCheckmark)
        {
            if (theme.showAvatar)
            {
                global::Doroti.Ui.Rect avatarRect = _boxRect(avatar).shift(offset);
                var darkenPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = selectionScrimTween.evaluate(checkmarkAnimation)!;
    __cascade.blendMode = BlendMode.srcATop;
    return __cascade;
}))();
                if (avatarBorder!.preferPaintInterior)
                {
                    avatarBorder!.paintInterior(context.canvas, avatarRect, darkenPaint);
                }
                else
                {
                    global::Doroti.Ui.Path path = avatarBorder!.getOuterPath(avatarRect);
                    context.canvas.drawPath(path, darkenPaint);
                }
            }
            double checkSize = avatar.size.height * 0.75;
            global::Doroti.Ui.Offset checkOffset = _boxParentData(avatar).offset + new global::Doroti.Ui.Offset(avatar.size.height * 0.125, avatar.size.height * 0.125);
            _paintCheck(context.canvas, offset + checkOffset, checkSize);
        }
    }

    internal virtual void _paintAvatar(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        void paintWithOverlay(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
        {
            context.paintChild(avatar, _boxParentData(avatar).offset + offset);
            _paintSelectionOverlay(context, offset);
        }
        if (!theme.showAvatar && avatarDrawerAnimation.isDismissed)
        {
            _avatarOpacityLayerHandler.layer = null;
            return;
        }
        global::Doroti.Ui.Color disabledColor = _disabledColor;
        long disabledColorAlpha = disabledColor.alpha;
        if (needsCompositing)
        {
            _avatarOpacityLayerHandler.layer = context.pushOpacity(offset, disabledColorAlpha, paintWithOverlay, oldLayer: _avatarOpacityLayerHandler.layer);
        }
        else
        {
            _avatarOpacityLayerHandler.layer = null;
            if (disabledColorAlpha != 255L)
            {
                context.canvas.saveLayer(_boxRect(avatar).shift(offset).inflate(20.0), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = disabledColor;
    return __cascade;
}))());
            }
            paintWithOverlay(context, offset);
            if (disabledColorAlpha != 255L)
            {
                context.canvas.restore();
            }
        }
    }

    internal virtual void _paintChild(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox? child, bool isDeleteIcon)
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
                _labelOpacityLayerHandler.layer = context.pushOpacity(offset, disabledColorAlpha, (context, offset) =>
                {
                    context.paintChild(child, _boxParentData(child).offset + offset);
                }, oldLayer: _labelOpacityLayerHandler.layer);
                if (isDeleteIcon)
                {
                    _deleteIconOpacityLayerHandler.layer = context.pushOpacity(offset, disabledColorAlpha, (context, offset) =>
                    {
                        context.paintChild(child, _boxParentData(child).offset + offset);
                    }, oldLayer: _deleteIconOpacityLayerHandler.layer);
                }
            }
            else
            {
                _labelOpacityLayerHandler.layer = null;
                _deleteIconOpacityLayerHandler.layer = null;
                global::Doroti.Ui.Rect childRect = _boxRect(child).shift(offset);
                context.canvas.saveLayer(childRect.inflate(20.0), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = _disabledColor;
    return __cascade;
}))());
                context.paintChild(child, _boxParentData(child).offset + offset);
                context.canvas.restore();
            }
        }
        else
        {
            context.paintChild(child, _boxParentData(child).offset + offset);
        }
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
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
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
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

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        _paintAvatar(context, offset);
        if (deleteIconShowing)
        {
            _paintChild(context, offset, deleteIcon, isDeleteIcon: true);
        }
        _paintChild(context, offset, label, isDeleteIcon: false);
    }

    public override void debugPaint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => !_debugShowTapTargetOutlines || ((global::System.Func<bool>)(() =>
        {
            var outlinePaint = ((Func<Paint>)(() =>
            {
                var __cascade = new global::Doroti.Ui.Paint();
                __cascade.color = new global::Doroti.Ui.Color(4286578688L);
                __cascade.strokeWidth = 1.0;
                __cascade.style = PaintingStyle.stroke;
                return __cascade;
            }))();
            if (deleteIconShowing)
            {
                context.canvas.drawRect(_deleteButtonRect.shift(offset), outlinePaint);
            }
            context.canvas.drawRect(_pressRect.shift(offset), ((Func<Paint>)(() =>
            {
                var __cascade = outlinePaint;
                __cascade.color = new global::Doroti.Ui.Color(4278222848L);
                return __cascade;
            }))());
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))());
    }

    public override bool hitTestSelf(Offset position) => DartRuntimePrimitives.ConvertValue<bool>(_deleteButtonRect.contains(position) || _pressRect.contains(position));
    public virtual global::Doroti.Framework.Rendering.RenderBox? childForSlot(_ChipSlot__chip slot) => _slotToChild.GetValueOrDefault(slot);
    public virtual string debugNameForSlot(_ChipSlot__chip slot)
    {
        {
            return slot.ToString();
        }
    }

    public override void redepthChildren()
    {
        children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        children.forEach((__arg0) => visitor(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var childToSlot = new DartMap<global::Doroti.Framework.Rendering.RenderBox, _ChipSlot__chip>(_slotToChild.Values, _slotToChild.Keys);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in children)
        {
            _addDiagnostics(child, value, debugNameForSlot(DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_ChipSlot__chip>(childToSlot, child))));
        }
        return value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _addDiagnostics(global::Doroti.Framework.Rendering.RenderBox child, List<global::Doroti.Framework.Foundation.DiagnosticsNode> value, string name)
    {
        value.Add(((Diagnosticable)child).toDiagnosticsNode(name: name));
    }

    public virtual void _setChild(global::Doroti.Framework.Rendering.RenderBox? child, _ChipSlot__chip slot)
    {
        global::Doroti.Framework.Rendering.RenderBox? oldChild = _slotToChild.GetValueOrDefault(slot);
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

    public virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, _ChipSlot__chip slot, _ChipSlot__chip oldSlot)
    {
        DartRuntimePrimitives.Assert(() => !Equals(slot, oldSlot));
        global::Doroti.Framework.Rendering.RenderBox? oldChild = _slotToChild.GetValueOrDefault(oldSlot);
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
    public virtual global::Doroti.Framework.Rendering.BoxConstraints labelConstraints { get; private set; } = default!;
    public virtual Size label { get; private set; } = default!;
    public virtual Size deleteIcon { get; private set; } = default!;
    public virtual Offset densityAdjustment { get; private set; } = default!;

    internal _ChipSizes__chip(Size size, Size overall, double content, Size avatar, global::Doroti.Framework.Rendering.BoxConstraints labelConstraints, Size label, Size deleteIcon, Offset densityAdjustment)
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

    public virtual InteractiveInkFeature create(MaterialInkController controller, global::Doroti.Framework.Rendering.RenderBox referenceBox, Offset position, Color color, TextDirection textDirection, bool containedInkWell = false, global::System.Func<Rect>? rectCallback = null, global::Doroti.Framework.Painting.BorderRadius? borderRadius = null, global::Doroti.Framework.Painting.ShapeBorder? customBorder = null, double? radius = null, global::System.Action? onRemoved = null)
    {
        return parentFactory.create(controller: controller, referenceBox: referenceBox, position: position, color: color, rectCallback: rectCallback, borderRadius: borderRadius, customBorder: customBorder, radius: radius, onRemoved: onRemoved, textDirection: textDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class ChipLibrary
{
    internal static bool _hitIsOnDeleteIcon(global::Doroti.Framework.Painting.EdgeInsetsGeometry padding, global::Doroti.Framework.Painting.EdgeInsetsGeometry labelPadding, Offset tapPosition, Size chipSize, Size deleteButtonSize, TextDirection textDirection)
    {
        global::Doroti.Framework.Painting.EdgeInsets resolvedPadding = padding.resolve(textDirection);
        global::Doroti.Ui.Size deflatedSize = resolvedPadding.deflateSize(chipSize);
        global::Doroti.Ui.Offset adjustedPosition = tapPosition - new global::Doroti.Ui.Offset(resolvedPadding.left, resolvedPadding.top);
        double accessibleDeleteButtonWidth = Math.Min(deflatedSize.width * 0.499, Math.Min(labelPadding.resolve(textDirection).right + deleteButtonSize.width, 24.0 + (deleteButtonSize.width / 2.0)));
        return textDirection switch { TextDirection.ltr => adjustedPosition.dx >= (deflatedSize.width - accessibleDeleteButtonWidth), TextDirection.rtl => adjustedPosition.dx <= accessibleDeleteButtonWidth, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _EnsureMinSemanticsSize__chip : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual Size semanticSize { get; private set; } = default!;

    internal _EnsureMinSemanticsSize__chip(global::Doroti.Framework.Widgets.Widget? child = null, Size semanticSize = default!) : base(child: child)
    {
        this.semanticSize = semanticSize;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderEnsureMinSemanticsSize__chip(semanticSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderEnsureMinSemanticsSize__chip)renderObject;
        __renderObject.semanticSize = semanticSize;
    }

}

public class _RenderEnsureMinSemanticsSize__chip : global::Doroti.Framework.Rendering.RenderProxyBox
{
    internal virtual Size _semanticSize { get; set; } = default!;

    internal _RenderEnsureMinSemanticsSize__chip(Size _semanticSize, global::Doroti.Framework.Rendering.RenderBox? child = null) : base(child)
    {
        this._semanticSize = _semanticSize;
    }

    public virtual global::Doroti.Ui.Size semanticSize
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
    public override void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
        base.describeSemanticsConfiguration(config);
        config.isSemanticBoundary = true;
        config.isButton = true;
    }

    public override Rect semanticBounds
    {
        get
        {
            return Rect.fromCenter(center: paintBounds.center, width: Math.Max(_semanticSize.width, size.width), height: Math.Max(_semanticSize.height, size.height));
        }
    }
}

internal class _ChipDefaultsM3__chip : ChipThemeData
{
    public virtual global::Doroti.Framework.Widgets.BuildContext context { get; private set; } = default!;
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

    internal _ChipDefaultsM3__chip(global::Doroti.Framework.Widgets.BuildContext context, bool isEnabled) : base(elevation: 0.0, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(8.0))), showCheckmark: true)
    {
        this.context = context;
        this.isEnabled = isEnabled;
    }

    public override global::Doroti.Framework.Painting.TextStyle? labelStyle => _textTheme.labelLarge?.copyWith(color: isEnabled ? _colors.onSurfaceVariant : _colors.onSurface);
    public override global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(null);
    public override global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public override global::Doroti.Ui.Color? checkmarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(null);
    public override global::Doroti.Ui.Color? deleteIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(isEnabled ? _colors.onSurfaceVariant : _colors.onSurface);
    public override global::Doroti.Framework.Painting.BorderSide? side => isEnabled ? new global::Doroti.Framework.Painting.BorderSide(color: _colors.outlineVariant) : new global::Doroti.Framework.Painting.BorderSide(color: _colors.onSurface.withOpacity(0.12));
    public override global::Doroti.Framework.Widgets.IconThemeData? iconTheme => new global::Doroti.Framework.Widgets.IconThemeData(color: isEnabled ? _colors.primary : _colors.onSurface, size: 18.0);
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(EdgeInsets.CreateAll(8.0));
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding
    {
        get
        {
            double fontSizeLocal = labelStyle?.fontSize ?? 14.0;
            double fontSizeRatio = MediaQuery.textScalerOf(context).scale(fontSizeLocal) / 14.0;
            return EdgeInsets.lerp(EdgeInsets.CreateSymmetric(horizontal: 8.0), EdgeInsets.CreateSymmetric(horizontal: 4.0), Dart_uiLibrary.clampDouble(fontSizeRatio - 1.0, 0.0, 1.0))!;
        }
    }
}
