// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/chip.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new RawChip(avatar: this.avatar, label: this.label, labelStyle: this.labelStyle, labelPadding: this.labelPadding, deleteIcon: this.deleteIcon, onDeleted: () => this.onDeleted(), deleteIconColor: this.deleteIconColor, deleteButtonTooltipMessage: this.deleteButtonTooltipMessage, tapEnabled: false, side: this.side, shape: this.shape, clipBehavior: this.clipBehavior, focusNode: this.focusNode, autofocus: this.autofocus, color: this.color, backgroundColor: this.backgroundColor, padding: this.padding, visualDensity: this.visualDensity, materialTapTargetSize: this.materialTapTargetSize, elevation: this.elevation, shadowColor: this.shadowColor, surfaceTintColor: this.surfaceTintColor, iconTheme: this.iconTheme, avatarBoxConstraints: this.avatarBoxConstraints, deleteIconBoxConstraints: this.deleteIconBoxConstraints, chipAnimationStyle: this.chipAnimationStyle, mouseCursor: this.mouseCursor));
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
    public virtual global::Doroti.Framework.Widgets.Widget? deleteIcon { get; private set; }
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
        this.deleteIcon = (deleteIcon ?? ChipLibrary._kDefaultDeleteIcon);
        System.Diagnostics.Debug.Assert(((pressElevation is null) || (pressElevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
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

    public virtual bool hasDeleteButton => DartRuntimePrimitives.ConvertValue<bool>((((RawChip)this.widget).onDeleted is not null));
    public virtual bool hasAvatar => DartRuntimePrimitives.ConvertValue<bool>((((RawChip)this.widget).avatar is not null));
    public virtual bool canTap
    {
        get
        {
            return ((((RawChip)this.widget).isEnabled && ((RawChip)this.widget).tapEnabled) && (((((RawChip)this.widget).onPressed is not null) || (((RawChip)this.widget).onSelected is not null))));
            return default!;
        }
    }
    public virtual bool isTapping => DartRuntimePrimitives.ConvertValue<bool>((this.canTap && this._isTapping));
    public override void initState()
    {
        DartRuntimePrimitives.Assert(() => ((((RawChip)this.widget).onSelected is null) || (((RawChip)this.widget).onPressed is null)));
        base.initState();
        DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Widgets.WidgetStatesController>)(() =>
{
    var __cascade = this.statesController;
    __cascade.update(global::Doroti.Framework.Widgets.WidgetState.disabled, !((RawChip)this.widget).isEnabled);
    __cascade.update(global::Doroti.Framework.Widgets.WidgetState.selected, ((RawChip)this.widget).selected);
    __cascade.addListener(((global::System.Action)(() =>
    {
        setState(((global::System.Action)(() =>
        {
        })));
    })));
    return __cascade;
}))());
        selectController = new global::Doroti.Framework.Animation.AnimationController(duration: (((RawChip)this.widget).chipAnimationStyle?.selectAnimation?.duration ?? ChipLibrary._kSelectDuration), reverseDuration: ((RawChip)this.widget).chipAnimationStyle?.selectAnimation?.reverseDuration, value: (((RawChip)this.widget).selected ? 1.0 : 0.0), vsync: this);
        selectionFade = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this.selectController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        avatarDrawerController = new global::Doroti.Framework.Animation.AnimationController(duration: (((RawChip)this.widget).chipAnimationStyle?.avatarDrawerAnimation?.duration ?? ChipLibrary._kDrawerDuration), reverseDuration: ((RawChip)this.widget).chipAnimationStyle?.avatarDrawerAnimation?.reverseDuration, value: ((this.hasAvatar || ((RawChip)this.widget).selected) ? 1.0 : 0.0), vsync: this);
        deleteDrawerController = new global::Doroti.Framework.Animation.AnimationController(duration: (((RawChip)this.widget).chipAnimationStyle?.deleteDrawerAnimation?.duration ?? ChipLibrary._kDrawerDuration), reverseDuration: ((RawChip)this.widget).chipAnimationStyle?.deleteDrawerAnimation?.reverseDuration, value: (this.hasDeleteButton ? 1.0 : 0.0), vsync: this);
        enableController = new global::Doroti.Framework.Animation.AnimationController(duration: (((RawChip)this.widget).chipAnimationStyle?.enableAnimation?.duration ?? ChipLibrary._kDisableDuration), reverseDuration: ((RawChip)this.widget).chipAnimationStyle?.enableAnimation?.reverseDuration, value: (((RawChip)this.widget).isEnabled ? 1.0 : 0.0), vsync: this);
        double checkmarkPercentage = (ChipLibrary._kCheckmarkDuration.inMilliseconds / ChipLibrary._kSelectDuration.inMilliseconds);
        double checkmarkReversePercentage = (ChipLibrary._kCheckmarkReverseDuration.inMilliseconds / ChipLibrary._kSelectDuration.inMilliseconds);
        double avatarDrawerReversePercentage = (ChipLibrary._kReverseDrawerDuration.inMilliseconds / ChipLibrary._kSelectDuration.inMilliseconds);
        checkmarkAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this.selectController, curve: new global::Doroti.Framework.Animation.Interval((1.0 - checkmarkPercentage), 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn), reverseCurve: new global::Doroti.Framework.Animation.Interval((1.0 - checkmarkReversePercentage), 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn));
        deleteDrawerAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this.deleteDrawerController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
        avatarDrawerAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this.avatarDrawerController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, reverseCurve: new global::Doroti.Framework.Animation.Interval((1.0 - avatarDrawerReversePercentage), 1.0, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn));
        enableAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this.enableController, curve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn);
    }

    public override void dispose()
    {
        this.selectController.dispose();
        this.avatarDrawerController.dispose();
        this.deleteDrawerController.dispose();
        this.enableController.dispose();
        this.checkmarkAnimation.dispose();
        this.avatarDrawerAnimation.dispose();
        this.deleteDrawerAnimation.dispose();
        this.enableAnimation.dispose();
        this.selectionFade.dispose();
        this.statesController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual void _handleTapDown(global::Doroti.Framework.Gestures.TapDownDetails details)
    {
        if (!this.canTap)
        {
            return;
        }
        this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.pressed, true);
        setState(((global::System.Action)(() =>
        {
            _isTapping = true;
        })));
    }

    internal virtual void _handleTapCancel()
    {
        if (!this.canTap)
        {
            return;
        }
        this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.pressed, false);
        setState(((global::System.Action)(() =>
        {
            _isTapping = false;
        })));
    }

    internal virtual void _handleTap()
    {
        if (!this.canTap)
        {
            return;
        }
        this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.pressed, false);
        setState(((global::System.Action)(() =>
        {
            _isTapping = false;
        })));
        ((RawChip)this.widget).onSelected?.Invoke(!((RawChip)this.widget).selected);
        ((RawChip)this.widget).onPressed?.Invoke();
    }

    internal virtual global::Doroti.Framework.Painting.OutlinedBorder _getShape(ThemeData theme, ChipThemeData chipTheme, ChipThemeData chipDefaults)
    {
        global::Doroti.Framework.Painting.BorderSide? resolvedSide = ((WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.BorderSide?>(((RawChip)this.widget).side, this.statesController.value) ?? (global::Doroti.Framework.Painting.BorderSide)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.BorderSide?>(chipTheme.side, this.statesController.value)));
        global::Doroti.Framework.Painting.OutlinedBorder resolvedShape = (((((WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.OutlinedBorder?>(((RawChip)this.widget).shape, this.statesController.value) ?? (global::Doroti.Framework.Painting.OutlinedBorder)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.OutlinedBorder?>(chipTheme.shape, this.statesController.value))) ?? (global::Doroti.Framework.Painting.OutlinedBorder)WidgetStateProperty.resolveAs<global::Doroti.Framework.Painting.OutlinedBorder?>(chipDefaults.shape, this.statesController.value))) ?? new global::Doroti.Framework.Painting.StadiumBorder());
        if ((resolvedSide is not null))
        {
            return ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)resolvedShape.copyWith(side: resolvedSide));
        }
        return ((!object.Equals(((global::Doroti.Framework.Painting.OutlinedBorder)resolvedShape).side, global::Doroti.Framework.Painting.BorderSide.none)) ? resolvedShape : resolvedShape.copyWith(side: chipDefaults.side));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color? resolveColor(global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? selectedColor = null, Color? backgroundColor = null, Color? disabledColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? defaultColor = null)
    {
        return ((global::Doroti.Ui.Color?)(object?)((new _IndividualOverrides__chip(color: color, selectedColor: selectedColor, backgroundColor: backgroundColor, disabledColor: disabledColor).resolve(this.statesController.value) ?? (Color)defaultColor?.resolve(this.statesController.value))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color? _getBackgroundColor(ThemeData theme, ChipThemeData chipTheme, ChipThemeData chipDefaults)
    {
        if (theme.useMaterial3)
        {
            global::Doroti.Ui.Color? disabledColorLocal = ((global::Doroti.Ui.Color?)(object?)resolveColor(color: (((RawChip)this.widget).color ?? chipTheme.color), disabledColor: (((RawChip)this.widget).disabledColor ?? chipTheme.disabledColor), defaultColor: chipDefaults.color));
            global::Doroti.Ui.Color? backgroundColorLocal = ((global::Doroti.Ui.Color?)(object?)resolveColor(color: (((RawChip)this.widget).color ?? chipTheme.color), backgroundColor: (((RawChip)this.widget).backgroundColor ?? chipTheme.backgroundColor), defaultColor: chipDefaults.color));
            global::Doroti.Ui.Color? selectedColorLocal = ((global::Doroti.Ui.Color?)(object?)resolveColor(color: (((RawChip)this.widget).color ?? chipTheme.color), selectedColor: (((RawChip)this.widget).selectedColor ?? chipTheme.selectedColor), defaultColor: chipDefaults.color));
            var backgroundTween = new global::Doroti.Framework.Animation.ColorTween(begin: disabledColorLocal, end: backgroundColorLocal);
            var selectTween = new global::Doroti.Framework.Animation.ColorTween(begin: backgroundTween.evaluate(this.enableController), end: selectedColorLocal);
            return ((global::Doroti.Ui.Color?)(object?)selectTween.evaluate(this.selectionFade));
        }
        else
        {
            var backgroundTweenLocal = new global::Doroti.Framework.Animation.ColorTween(begin: ((((RawChip)this.widget).disabledColor ?? chipTheme.disabledColor) ?? theme.disabledColor), end: (((((RawChip)this.widget).backgroundColor ?? chipTheme.backgroundColor) ?? theme.chipTheme.backgroundColor) ?? chipDefaults.backgroundColor));
            var selectTweenLocal = new global::Doroti.Framework.Animation.ColorTween(begin: backgroundTweenLocal.evaluate(this.enableController), end: (((((RawChip)this.widget).selectedColor ?? chipTheme.selectedColor) ?? theme.chipTheme.selectedColor) ?? chipDefaults.selectedColor));
            return ((global::Doroti.Ui.Color?)(object?)selectTweenLocal.evaluate(this.selectionFade));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didUpdateWidget(RawChip oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((((RawChip)oldWidget).isEnabled != ((RawChip)this.widget).isEnabled))
        {
            setState(((global::System.Action)(() =>
            {
                this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.disabled, !((RawChip)this.widget).isEnabled);
                if (((RawChip)this.widget).isEnabled)
                {
                    this.enableController.forward();
                }
                else
                {
                    this.enableController.reverse();
                }
            })));
        }
        if (((!object.Equals(((RawChip)oldWidget).avatar, ((RawChip)this.widget).avatar)) || (((RawChip)oldWidget).selected != ((RawChip)this.widget).selected)))
        {
            setState(((global::System.Action)(() =>
            {
                if ((this.hasAvatar || ((RawChip)this.widget).selected))
                {
                    this.avatarDrawerController.forward();
                }
                else
                {
                    this.avatarDrawerController.reverse();
                }
            })));
        }
        if ((((RawChip)oldWidget).selected != ((RawChip)this.widget).selected))
        {
            setState(((global::System.Action)(() =>
            {
                this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.selected, ((RawChip)this.widget).selected);
                if (((RawChip)this.widget).selected)
                {
                    this.selectController.forward();
                }
                else
                {
                    this.selectController.reverse();
                }
            })));
        }
        if ((!object.Equals((global::System.Action?)((RawChip)oldWidget).onDeleted, (global::System.Action?)((RawChip)this.widget).onDeleted)))
        {
            setState(((global::System.Action)(() =>
            {
                if (this.hasDeleteButton)
                {
                    this.deleteDrawerController.forward();
                }
                else
                {
                    this.deleteDrawerController.reverse();
                }
            })));
        }
    }

    internal virtual global::Doroti.Framework.Widgets.Widget? _wrapWithTooltip(string? tooltip = null, bool enabled = true, global::Doroti.Framework.Widgets.Widget? child = null)
    {
        if ((((child is null) || !enabled) || (tooltip is null)))
        {
            return child;
        }
        return ((global::Doroti.Framework.Widgets.Widget?)(object?)new Tooltip(message: tooltip, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget? _buildDeleteIcon(global::Doroti.Framework.Widgets.BuildContext context, ThemeData theme, ChipThemeData chipTheme, ChipThemeData chipDefaults)
    {
        if (!this.hasDeleteButton)
        {
            return null;
        }
        global::Doroti.Framework.Widgets.IconThemeData iconThemeLocal = (((((RawChip)this.widget).iconTheme ?? chipTheme.iconTheme) ?? theme.chipTheme.iconTheme) ?? new _ChipDefaultsM3__chip(context, ((RawChip)this.widget).isEnabled).iconTheme!);
        global::Doroti.Ui.Color? effectiveDeleteIconColor = ((global::Doroti.Ui.Color?)(object?)WidgetStateProperty.resolveAs((((((((RawChip)this.widget).deleteIconColor ?? chipTheme.deleteIconColor) ?? theme.chipTheme.deleteIconColor) ?? ((RawChip)this.widget).iconTheme?.color) ?? chipTheme.iconTheme?.color) ?? chipDefaults.deleteIconColor), this.statesController.value));
        double effectiveIconSize = (((((RawChip)this.widget).iconTheme?.size ?? chipTheme.iconTheme?.size) ?? theme.chipTheme.iconTheme?.size) ?? DartRuntimePrimitives.RequireValue(new _ChipDefaultsM3__chip(context, ((RawChip)this.widget).isEnabled).iconTheme!.size));
        MaterialTapTargetSize effectiveMaterialTapTargetSize = (((RawChip)this.widget).materialTapTargetSize ?? theme.materialTapTargetSize);
        global::Doroti.Ui.Size semanticSizeLocal = ((global::Doroti.Ui.Size)(object?)(effectiveMaterialTapTargetSize switch { var __constant45576 when (object.Equals(__constant45576, MaterialTapTargetSize.padded)) => new global::Doroti.Ui.Size(ConstantsLibrary.kMinInteractiveDimension), var __constant45659 when (object.Equals(__constant45659, MaterialTapTargetSize.shrinkWrap)) => new global::Doroti.Ui.Size((ConstantsLibrary.kMinInteractiveDimension - 8.0)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
        VisualDensity effectiveVisualDensity = (((RawChip)this.widget).visualDensity ?? theme.visualDensity);
        return ((global::Doroti.Framework.Widgets.Widget?)(object?)new _EnsureMinSemanticsSize__chip(semanticSize: (semanticSizeLocal + effectiveVisualDensity.baseSizeAdjustment), child: _wrapWithTooltip(tooltip: (((RawChip)this.widget).deleteButtonTooltipMessage ?? MaterialLocalizations.of(context).deleteButtonTooltip), enabled: (((RawChip)this.widget).isEnabled && (((RawChip)this.widget).onDeleted is not null)), child: new InkWell(radius: (((ChipLibrary._kChipHeight + ((((RawChip)this.widget).padding?.vertical ?? 0.0)))) * 0.45), splashFactory: new _UnconstrainedInkSplashFactory__chip(Theme.of(context).splashFactory), customBorder: new global::Doroti.Framework.Painting.CircleBorder(), onTap: ((global::System.Action)(((RawChip)this.widget).isEnabled ? ((RawChip)this.widget).onDeleted : null)), child: new global::Doroti.Framework.Widgets.IconTheme(data: iconThemeLocal.copyWith(color: effectiveDeleteIconColor, size: effectiveIconSize), child: ((RawChip)this.widget).deleteIcon)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterial(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasMediaQuery(context));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
        DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasMaterialLocalizations(context));
        ThemeData themeLocal = Theme.of(context);
        ChipThemeData chipTheme = ChipTheme.of(context);
        global::Doroti.Ui.Brightness brightnessLocal = (chipTheme.brightness ?? themeLocal.brightness);
        ChipThemeData chipDefaults = (((RawChip)this.widget).defaultProperties ?? ((themeLocal.useMaterial3 ? new _ChipDefaultsM3__chip(context, ((RawChip)this.widget).isEnabled) : new ChipThemeData(brightness: brightnessLocal, secondarySelectedColor: ((object.Equals(brightnessLocal, Brightness.dark)) ? Colors.tealAccent[200L]! : themeLocal.primaryColor), labelStyle: themeLocal.textTheme.bodyLarge!))));
        global::Doroti.Ui.TextDirection? textDirection = Directionality.maybeOf(context);
        global::Doroti.Framework.Painting.OutlinedBorder resolvedShape = ((global::Doroti.Framework.Painting.OutlinedBorder)(object?)_getShape(themeLocal, chipTheme, chipDefaults));
        double elevationLocal = (((((RawChip)this.widget).elevation ?? chipTheme.elevation) ?? chipDefaults.elevation) ?? 0);
        double pressElevationLocal = (((((RawChip)this.widget).pressElevation ?? chipTheme.pressElevation) ?? chipDefaults.pressElevation) ?? 0);
        global::Doroti.Ui.Color? shadowColorLocal = ((global::Doroti.Ui.Color?)(object?)((((RawChip)this.widget).shadowColor ?? chipTheme.shadowColor) ?? chipDefaults.shadowColor));
        global::Doroti.Ui.Color? surfaceTintColorLocal = ((global::Doroti.Ui.Color?)(object?)((((RawChip)this.widget).surfaceTintColor ?? chipTheme.surfaceTintColor) ?? chipDefaults.surfaceTintColor));
        global::Doroti.Ui.Color? selectedShadowColorLocal = ((global::Doroti.Ui.Color?)(object?)((((RawChip)this.widget).selectedShadowColor ?? chipTheme.selectedShadowColor) ?? chipDefaults.selectedShadowColor));
        global::Doroti.Ui.Color? checkmarkColorLocal = ((global::Doroti.Ui.Color?)(object?)((((RawChip)this.widget).checkmarkColor ?? chipTheme.checkmarkColor) ?? chipDefaults.checkmarkColor));
        bool showCheckmarkLocal = ((((RawChip)this.widget).showCheckmark ?? chipTheme.showCheckmark) ?? DartRuntimePrimitives.RequireValue(chipDefaults.showCheckmark));
        global::Doroti.Framework.Painting.EdgeInsetsGeometry paddingLocal = ((((RawChip)this.widget).padding ?? chipTheme.padding) ?? chipDefaults.padding!);
        global::Doroti.Framework.Painting.TextStyle labelStyleLocal = (chipTheme.labelStyle ?? chipDefaults.labelStyle!);
        global::Doroti.Framework.Widgets.IconThemeData? iconThemeLocal = ((((RawChip)this.widget).iconTheme ?? chipTheme.iconTheme) ?? chipDefaults.iconTheme);
        global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraintsLocal = (((RawChip)this.widget).avatarBoxConstraints ?? chipTheme.avatarBoxConstraints);
        global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraintsLocal = (((RawChip)this.widget).deleteIconBoxConstraints ?? chipTheme.deleteIconBoxConstraints);
        global::Doroti.Framework.Painting.TextStyle effectiveLabelStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)labelStyleLocal.merge(((RawChip)this.widget).labelStyle));
        global::Doroti.Ui.Color? resolvedLabelColor = ((global::Doroti.Ui.Color?)(object?)WidgetStateProperty.resolveAs<global::Doroti.Ui.Color?>(((global::Doroti.Framework.Painting.TextStyle)effectiveLabelStyle).color, this.statesController.value));
        global::Doroti.Framework.Painting.TextStyle resolvedLabelStyle = ((global::Doroti.Framework.Painting.TextStyle)(object?)effectiveLabelStyle.copyWith(color: resolvedLabelColor));
        global::Doroti.Framework.Widgets.Widget? avatarLocal = (((iconThemeLocal is not null) && this.hasAvatar) ? IconTheme.merge(data: chipDefaults.iconTheme!.merge(iconThemeLocal), child: ((RawChip)this.widget).avatar!) : ((RawChip)this.widget).avatar);
        double defaultFontSize = (((global::Doroti.Framework.Painting.TextStyle)effectiveLabelStyle).fontSize ?? 14.0);
        double effectiveTextScale = (MediaQuery.textScalerOf(context).scale(defaultFontSize) / 14.0);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry defaultLabelPadding = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)EdgeInsets.lerp(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4.0), Dart_uiLibrary.clampDouble((effectiveTextScale - 1.0), 0.0, 1.0))!);
        global::Doroti.Framework.Painting.EdgeInsetsGeometry labelPaddingLocal = (((((RawChip)this.widget).labelPadding ?? chipTheme.labelPadding) ?? chipDefaults.labelPadding) ?? defaultLabelPadding);
        global::Doroti.Framework.Widgets.Widget result = ((global::Doroti.Framework.Widgets.Widget)(object?)new Material(elevation: (this.isTapping ? pressElevationLocal : elevationLocal), shadowColor: (((RawChip)this.widget).selected ? selectedShadowColorLocal : shadowColorLocal), surfaceTintColor: surfaceTintColorLocal, animationDuration: pressedAnimationDuration, shape: resolvedShape, clipBehavior: ((RawChip)this.widget).clipBehavior, child: new InkWell(onFocusChange: ((value) =>
        {
            this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.focused, value);
        }), focusNode: ((RawChip)this.widget).focusNode, autofocus: ((RawChip)this.widget).autofocus, canRequestFocus: ((RawChip)this.widget).isEnabled, onTap: ((global::System.Action)(this.canTap ? this._handleTap : null)), onTapDown: ((global::System.Action<global::Doroti.Framework.Gestures.TapDownDetails>)(this.canTap ? this._handleTapDown : null)), onTapCancel: ((global::System.Action)(this.canTap ? this._handleTapCancel : null)), onHover: ((global::System.Action<bool>)(this.canTap ? ((value) =>
        {
            this.statesController.update(global::Doroti.Framework.Widgets.WidgetState.hovered, value);
        }) : null)), mouseCursor: ((RawChip)this.widget).mouseCursor, hoverColor: ((((((RawChip)this.widget).color ?? chipTheme.color)) is null) ? null : Colors.transparent), customBorder: resolvedShape, child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: global::Doroti.Framework.Foundation.Listenable.CreateMerge(new List<global::Doroti.Framework.Foundation.Listenable> { this.selectController, this.enableController }.Cast<global::Doroti.Framework.Foundation.Listenable?>()), builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new Ink(decoration: new global::Doroti.Framework.Painting.ShapeDecoration(shape: resolvedShape, color: _getBackgroundColor(themeLocal, chipTheme, chipDefaults)), child: child));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), child: _wrapWithTooltip(tooltip: ((RawChip)this.widget).tooltip, enabled: ((((RawChip)this.widget).onPressed is not null) || (((RawChip)this.widget).onSelected is not null)), child: new _ChipRenderWidget__chip(theme: new _ChipRenderTheme__chip(label: new global::Doroti.Framework.Widgets.DefaultTextStyle(overflow: global::Doroti.Framework.Painting.TextOverflow.fade, textAlign: global::Doroti.Ui.TextAlign.start, maxLines: 1L, softWrap: false, style: resolvedLabelStyle, child: ((RawChip)this.widget).label), avatar: new global::Doroti.Framework.Widgets.AnimatedSwitcher(duration: ChipLibrary._kDrawerDuration, switchInCurve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, child: avatarLocal), deleteIcon: new global::Doroti.Framework.Widgets.AnimatedSwitcher(duration: ChipLibrary._kDrawerDuration, switchInCurve: global::Doroti.Framework.Animation.Curves.fastOutSlowIn, child: _buildDeleteIcon(context, themeLocal, chipTheme, chipDefaults)), brightness: brightnessLocal, padding: paddingLocal.resolve(textDirection), visualDensity: (((RawChip)this.widget).visualDensity ?? themeLocal.visualDensity), labelPadding: labelPaddingLocal.resolve(textDirection), showAvatar: this.hasAvatar, showCheckmark: showCheckmarkLocal, checkmarkColor: checkmarkColorLocal, canTapBody: this.canTap), value: ((RawChip)this.widget).selected, checkmarkAnimation: this.checkmarkAnimation, enableAnimation: this.enableAnimation, avatarDrawerAnimation: this.avatarDrawerAnimation, deleteDrawerAnimation: this.deleteDrawerAnimation, isEnabled: ((RawChip)this.widget).isEnabled, avatarBorder: ((RawChip)this.widget).avatarBorder, avatarBoxConstraints: avatarBoxConstraintsLocal, deleteIconBoxConstraints: deleteIconBoxConstraintsLocal))))));
        global::Doroti.Framework.Rendering.BoxConstraints constraintsLocal = default!;
        global::Doroti.Ui.Offset densityAdjustment = ((global::Doroti.Ui.Offset)(object?)((((RawChip)this.widget).visualDensity ?? themeLocal.visualDensity)).baseSizeAdjustment);
        switch ((((RawChip)this.widget).materialTapTargetSize ?? themeLocal.materialTapTargetSize))
        {
            case var __constant54393 when (object.Equals(__constant54393, MaterialTapTargetSize.padded)):
                {
                    constraintsLocal = new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: (ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dx), minHeight: (ConstantsLibrary.kMinInteractiveDimension + densityAdjustment.dy));
                    break;
                }
            case var __constant54622 when (object.Equals(__constant54622, MaterialTapTargetSize.shrinkWrap)):
                {
                    constraintsLocal = new global::Doroti.Framework.Rendering.BoxConstraints();
                    break;
                }
        }
        result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new _ChipRedirectingHitDetectionWidget__chip(constraints: constraintsLocal, child: new global::Doroti.Framework.Widgets.Center(widthFactor: 1.0, heightFactor: 1.0, child: result)));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Semantics(button: ((RawChip)this.widget).tapEnabled, container: true, selected: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? null : ((RawChip)this.widget).selected), @checked: (global::Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? ((RawChip)this.widget).selected : null), enabled: (((RawChip)this.widget).tapEnabled ? this.canTap : null), child: result));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(this._updateTickers);
        newNotifier.addListener(this._updateTickers);
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
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
        if ((this.color is not null))
        {
            return ((Color?)(object?)this.color!.resolve(states));
        }
        if ((states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected) && states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled)))
        {
            return this.selectedColor;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.disabled))
        {
            return this.disabledColor;
        }
        if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.selected))
        {
            return this.selectedColor;
        }
        return this.backgroundColor;
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderChipRedirectingHitDetection__chip(this.constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderChipRedirectingHitDetection__chip)(object)renderObject;
        __renderObject.additionalConstraints = this.constraints;
    }

}

public class _RenderChipRedirectingHitDetection__chip : global::Doroti.Framework.Rendering.RenderConstrainedBox
{
    internal _RenderChipRedirectingHitDetection__chip(global::Doroti.Framework.Rendering.BoxConstraints additionalConstraints) : base(additionalConstraints: additionalConstraints)
    {
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (!this.size.contains(position))
        {
            return false;
        }
        var offset = new global::Doroti.Ui.Offset(position.dx, (this.size.height / 2L));
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(offset), position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(position, offset)));
            return this.child!.hitTest(result, position: offset);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
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

    public override IEnumerable<_ChipSlot__chip> slots => DartRuntimePrimitives.ConvertValue<IEnumerable<_ChipSlot__chip>>(System.Enum.GetValues<_ChipSlot__chip>().ToList());
    public override global::Doroti.Framework.Widgets.Widget? childForSlot(_ChipSlot__chip slot)
    {
        return (slot switch { _ChipSlot__chip.label => ((_ChipRenderTheme__chip)this.theme).label, _ChipSlot__chip.avatar => ((_ChipRenderTheme__chip)this.theme).avatar, _ChipSlot__chip.deleteIcon => ((_ChipRenderTheme__chip)this.theme).deleteIcon, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.SlottedContainerRenderObjectMixin<_ChipSlot__chip, global::Doroti.Framework.Rendering.RenderBox> renderObject)
    {
        var __renderObject = (_RenderChip__chip)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderChip__chip>)(() =>
{
    var __cascade = __renderObject;
    __cascade.theme = this.theme;
    __cascade.textDirection = Directionality.of(context);
    __cascade.value = this.value;
    __cascade.isEnabled = this.isEnabled;
    __cascade.checkmarkAnimation = this.checkmarkAnimation;
    __cascade.avatarDrawerAnimation = this.avatarDrawerAnimation;
    __cascade.deleteDrawerAnimation = this.deleteDrawerAnimation;
    __cascade.enableAnimation = this.enableAnimation;
    __cascade.avatarBorder = this.avatarBorder;
    __cascade.avatarBoxConstraints = this.avatarBoxConstraints;
    __cascade.deleteIconBoxConstraints = this.deleteIconBoxConstraints;
    return __cascade;
}))());
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderChip__chip(theme: this.theme, textDirection: Directionality.of(context), value: this.value, isEnabled: this.isEnabled, checkmarkAnimation: this.checkmarkAnimation, avatarDrawerAnimation: this.avatarDrawerAnimation, deleteDrawerAnimation: this.deleteDrawerAnimation, enableAnimation: this.enableAnimation, avatarBorder: this.avatarBorder, avatarBoxConstraints: this.avatarBoxConstraints, deleteIconBoxConstraints: this.deleteIconBoxConstraints));
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((__other is _ChipRenderTheme__chip) && (object.Equals(((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).avatar, this.avatar))) && (object.Equals(((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).label, this.label))) && (object.Equals(((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).deleteIcon, this.deleteIcon))) && (object.Equals(((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).brightness, this.brightness))) && (object.Equals(((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).padding, this.padding))) && (object.Equals(((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).labelPadding, this.labelPadding))) && (((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).showAvatar == this.showAvatar)) && (((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).showCheckmark == this.showCheckmark)) && (object.Equals(((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).checkmarkColor, this.checkmarkColor))) && (((_ChipRenderTheme__chip)((_ChipRenderTheme__chip)__other)).canTapBody == this.canTapBody));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.avatar, this.label, this.deleteIcon, this.brightness, this.padding, this.labelPadding, this.showAvatar, this.showCheckmark, this.checkmarkColor, this.canTapBody));
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
        this._theme = theme;
        this._textDirection = textDirection;
        this._avatarBoxConstraints = avatarBoxConstraints;
        this._deleteIconBoxConstraints = deleteIconBoxConstraints;
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox avatar => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(childForSlot(_ChipSlot__chip.avatar)!);
    public virtual global::Doroti.Framework.Rendering.RenderBox deleteIcon => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(childForSlot(_ChipSlot__chip.deleteIcon)!);
    public virtual global::Doroti.Framework.Rendering.RenderBox label => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderBox>(childForSlot(_ChipSlot__chip.label)!);
    public virtual _ChipRenderTheme__chip theme
    {
        get => this._theme;
        set
        {
            var __value = value;
            if ((object.Equals(this._theme, __value)))
            {
                return;
            }
            _theme = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Ui.TextDirection textDirection
    {
        get => this._textDirection;
        set
        {
            var __value = value;
            if ((object.Equals(this._textDirection, DartRuntimePrimitives.RequireValue(__value))))
            {
                return;
            }
            _textDirection = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(__value));
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints
    {
        get => this._avatarBoxConstraints;
        set
        {
            var __value = value;
            if ((object.Equals(this._avatarBoxConstraints, __value)))
            {
                return;
            }
            _avatarBoxConstraints = __value;
            markNeedsLayout();
        }
    }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints
    {
        get => this._deleteIconBoxConstraints;
        set
        {
            var __value = value;
            if ((object.Equals(this._deleteIconBoxConstraints, __value)))
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
            global::Doroti.Framework.Rendering.RenderBox? avatarLocal = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childForSlot(_ChipSlot__chip.avatar));
            global::Doroti.Framework.Rendering.RenderBox? labelLocal = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childForSlot(_ChipSlot__chip.label));
            global::Doroti.Framework.Rendering.RenderBox? deleteIconLocal = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childForSlot(_ChipSlot__chip.deleteIcon));
            return ((IEnumerable<global::Doroti.Framework.Rendering.RenderBox>)(object?)((Func<List<global::Doroti.Framework.Rendering.RenderBox>>)(() => { var __collection64442 = new List<global::Doroti.Framework.Rendering.RenderBox>(); var __collectionElement64454 = avatarLocal; if (__collectionElement64454 is { } __nonNullCollectionElement64454) { __collection64442.Add(__nonNullCollectionElement64454); } var __collectionElement64463 = labelLocal; if (__collectionElement64463 is { } __nonNullCollectionElement64463) { __collection64442.Add(__nonNullCollectionElement64463); } var __collectionElement64471 = deleteIconLocal; if (__collectionElement64471 is { } __nonNullCollectionElement64471) { __collection64442.Add(__nonNullCollectionElement64471); } return __collection64442; }))());
            return default!;
        }
    }
    public virtual bool isDrawingCheckmark => DartRuntimePrimitives.ConvertValue<bool>((((_ChipRenderTheme__chip)this.theme).showCheckmark && !((global::Doroti.Framework.Animation.Animation<double>)this.checkmarkAnimation).isDismissed));
    public virtual bool deleteIconShowing => !((global::Doroti.Framework.Animation.Animation<double>)this.deleteDrawerAnimation).isDismissed;
    internal static global::Doroti.Ui.Rect _boxRect(global::Doroti.Framework.Rendering.RenderBox box) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Rect>((_RenderChip__chip._boxParentData(box).offset & ((global::Doroti.Framework.Rendering.RenderBox)box).size));
    internal static global::Doroti.Framework.Rendering.BoxParentData _boxParentData(global::Doroti.Framework.Rendering.RenderBox box) => ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)box.parentData!)!;
    public override double computeMinIntrinsicWidth(double height)
    {
        double overallPadding = (((_ChipRenderTheme__chip)this.theme).padding.horizontal + ((_ChipRenderTheme__chip)this.theme).labelPadding.horizontal);
        return (((overallPadding + this.avatar.getMinIntrinsicWidth(height)) + this.label.getMinIntrinsicWidth(height)) + this.deleteIcon.getMinIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicWidth(double height)
    {
        double overallPadding = (((_ChipRenderTheme__chip)this.theme).padding.horizontal + ((_ChipRenderTheme__chip)this.theme).labelPadding.horizontal);
        return (((overallPadding + this.avatar.getMaxIntrinsicWidth(height)) + this.label.getMaxIntrinsicWidth(height)) + this.deleteIcon.getMaxIntrinsicWidth(height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMinIntrinsicHeight(double width)
    {
        return Math.Max(ChipLibrary._kChipHeight, ((((_ChipRenderTheme__chip)this.theme).padding.vertical + ((_ChipRenderTheme__chip)this.theme).labelPadding.vertical) + this.label.getMinIntrinsicHeight(width)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxIntrinsicHeight(double width) => getMinIntrinsicHeight(width);
    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        return ((new global::Doroti.Framework.Rendering.BaselineOffset(this.label.getDistanceToActualBaseline(baseline)).op_Add(_RenderChip__chip._boxParentData(this.label).offset.dy))).offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _labelConstraintsFrom(global::Doroti.Framework.Rendering.BoxConstraints contentConstraints, double iconWidth, double contentSize, Size rawLabelSize)
    {
        double freeSpace = (((((global::Doroti.Framework.Rendering.BoxConstraints)contentConstraints).maxWidth - iconWidth) - ((_ChipRenderTheme__chip)this.theme).labelPadding.horizontal) - ((_ChipRenderTheme__chip)this.theme).padding.horizontal);
        double maxLabelWidth = Math.Max(0.0, freeSpace);
        return new global::Doroti.Framework.Rendering.BoxConstraints(minHeight: rawLabelSize.height, maxHeight: contentSize, maxWidth: (double.IsFinite(maxLabelWidth) ? maxLabelWidth : rawLabelSize.width));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _layoutAvatar(double contentSize, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild = default!)
    {
        global::Doroti.Framework.Rendering.BoxConstraints avatarConstraints = (this.avatarBoxConstraints ?? global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: contentSize, height: contentSize));
        global::Doroti.Ui.Size avatarBoxSize = ((global::Doroti.Ui.Size)(object?)layoutChild(this.avatar, avatarConstraints));
        if ((!((_ChipRenderTheme__chip)this.theme).showCheckmark && !((_ChipRenderTheme__chip)this.theme).showAvatar))
        {
            return new global::Doroti.Ui.Size(0.0, contentSize);
        }
        double avatarFullWidth = (((_ChipRenderTheme__chip)this.theme).showAvatar ? avatarBoxSize.width : contentSize);
        return new global::Doroti.Ui.Size((avatarFullWidth * ((global::Doroti.Framework.Animation.Animation<double>)this.avatarDrawerAnimation).value), avatarBoxSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Size _layoutDeleteIcon(double contentSize, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild = default!)
    {
        global::Doroti.Framework.Rendering.BoxConstraints deleteIconConstraints = (this.deleteIconBoxConstraints ?? global::Doroti.Framework.Rendering.BoxConstraints.CreateTightFor(width: contentSize, height: contentSize));
        global::Doroti.Ui.Size boxSize = ((global::Doroti.Ui.Size)(object?)layoutChild(this.deleteIcon, deleteIconConstraints));
        if (!this.deleteIconShowing)
        {
            return new global::Doroti.Ui.Size(0.0, contentSize);
        }
        return new global::Doroti.Ui.Size((((global::Doroti.Framework.Animation.Animation<double>)this.deleteDrawerAnimation).value * boxSize.width), boxSize.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool hitTest(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        if (!this.size.contains(position))
        {
            return false;
        }
        bool hitIsOnDeleteIcon = ChipLibrary._hitIsOnDeleteIcon(padding: ((_ChipRenderTheme__chip)this.theme).padding, labelPadding: ((_ChipRenderTheme__chip)this.theme).labelPadding, tapPosition: position, chipSize: this.size, deleteButtonSize: ((global::Doroti.Framework.Rendering.RenderBox)this.deleteIcon).size, textDirection: this.textDirection);
        global::Doroti.Framework.Rendering.RenderBox hitTestChild = (hitIsOnDeleteIcon ? this.deleteIcon : this.label);
        global::Doroti.Ui.Offset centerLocal = ((global::Doroti.Ui.Offset)(object?)((global::Doroti.Framework.Rendering.RenderBox)hitTestChild).size.center(Offset.zero));
        return result.addWithRawTransform(transform: MatrixUtils.forceToPoint(centerLocal), position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, position) =>
        {
            DartRuntimePrimitives.Assert(() => (object.Equals(position, centerLocal)));
            return hitTestChild.hitTest(result, position: centerLocal);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return _computeSizes(constraints, (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild).size;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        _ChipSizes__chip sizes = ((_ChipSizes__chip)(object?)_computeSizes(constraints, (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.dryLayoutChild));
        global::Doroti.Framework.Rendering.BaselineOffset labelBaseline = (((new global::Doroti.Framework.Rendering.BaselineOffset(this.label.getDryBaseline(((_ChipSizes__chip)sizes).labelConstraints, baseline)).op_Add(((((((_ChipSizes__chip)sizes).content - ((_ChipSizes__chip)sizes).label.height) + ((_ChipSizes__chip)sizes).densityAdjustment.dy)) / 2L))).op_Add(((_ChipRenderTheme__chip)this.theme).padding.top)).op_Add(((_ChipRenderTheme__chip)this.theme).labelPadding.top));
        return labelBaseline.offset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual _ChipSizes__chip _computeSizes(global::Doroti.Framework.Rendering.BoxConstraints constraints, global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size> layoutChild)
    {
        global::Doroti.Framework.Rendering.BoxConstraints contentConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.loosen());
        global::Doroti.Ui.Size rawLabelSize = ((global::Doroti.Ui.Size)(object?)this.label.getDryLayout(contentConstraints));
        double contentSize = Math.Max(((ChipLibrary._kChipHeight - ((_ChipRenderTheme__chip)this.theme).padding.vertical) + ((_ChipRenderTheme__chip)this.theme).labelPadding.vertical), (rawLabelSize.height + ((_ChipRenderTheme__chip)this.theme).labelPadding.vertical));
        DartRuntimePrimitives.Assert(() => (contentSize >= rawLabelSize.height));
        global::Doroti.Ui.Size avatarSize = ((global::Doroti.Ui.Size)(object?)_layoutAvatar(contentSize, (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)layoutChild));
        global::Doroti.Ui.Size deleteIconSize = ((global::Doroti.Ui.Size)(object?)_layoutDeleteIcon(contentSize, (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)layoutChild));
        global::Doroti.Framework.Rendering.BoxConstraints labelConstraintsLocal = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)_labelConstraintsFrom(contentConstraints, (avatarSize.width + deleteIconSize.width), contentSize, rawLabelSize));
        global::Doroti.Ui.Size labelSize = ((global::Doroti.Ui.Size)(object?)((_ChipRenderTheme__chip)this.theme).labelPadding.inflateSize(layoutChild(this.label, labelConstraintsLocal)));
        var densityAdjustmentLocal = new global::Doroti.Ui.Offset(0.0, (((_ChipRenderTheme__chip)this.theme).visualDensity.baseSizeAdjustment.dy / 2.0));
        global::Doroti.Ui.Size overallSize = ((global::Doroti.Ui.Size)(object?)(new global::Doroti.Ui.Size(((avatarSize.width + labelSize.width) + deleteIconSize.width), contentSize) + densityAdjustmentLocal));
        var paddedSize = new global::Doroti.Ui.Size((overallSize.width + ((_ChipRenderTheme__chip)this.theme).padding.horizontal), (overallSize.height + ((_ChipRenderTheme__chip)this.theme).padding.vertical));
        return new _ChipSizes__chip(size: constraints.constrain(paddedSize), overall: overallSize, content: contentSize, densityAdjustment: densityAdjustmentLocal, avatar: avatarSize, labelConstraints: labelConstraintsLocal, label: labelSize, deleteIcon: deleteIconSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        _ChipSizes__chip sizes = ((_ChipSizes__chip)(object?)_computeSizes(this.constraints, (global::System.Func<global::Doroti.Framework.Rendering.RenderBox, global::Doroti.Framework.Rendering.BoxConstraints, Size>)global::Doroti.Framework.Rendering.ChildLayoutHelper.layoutChild));
        var leftLocal = 0.0;
        double rightLocal = ((_ChipSizes__chip)sizes).overall.width;
        Offset centerLayout(Size boxSize, double x)
        {
            DartRuntimePrimitives.Assert(() => (((_ChipSizes__chip)sizes).content >= boxSize.height));
            switch (this.textDirection)
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
            return new global::Doroti.Ui.Offset(x, ((((((_ChipSizes__chip)sizes).content - boxSize.height) + ((_ChipSizes__chip)sizes).densityAdjustment.dy)) / 2.0));
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        global::Doroti.Ui.Offset avatarOffset = ((global::Doroti.Ui.Offset)(object?)Offset.zero);
        global::Doroti.Ui.Offset labelOffset = ((global::Doroti.Ui.Offset)(object?)Offset.zero);
        global::Doroti.Ui.Offset deleteIconOffset = ((global::Doroti.Ui.Offset)(object?)Offset.zero);
        switch (this.textDirection)
        {
            case TextDirection.rtl:
                {
                    var start = rightLocal;
                    if ((((_ChipRenderTheme__chip)this.theme).showCheckmark || ((_ChipRenderTheme__chip)this.theme).showAvatar))
                    {
                        avatarOffset = centerLayout(((_ChipSizes__chip)sizes).avatar, start);
                        start -= ((_ChipSizes__chip)sizes).avatar.width;
                    }
                    labelOffset = centerLayout(((_ChipSizes__chip)sizes).label, start);
                    start -= ((_ChipSizes__chip)sizes).label.width;
                    if (this.deleteIconShowing)
                    {
                        _deleteButtonRect = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, (((_ChipSizes__chip)sizes).deleteIcon.width + ((_ChipRenderTheme__chip)this.theme).padding.right), (((_ChipSizes__chip)sizes).overall.height + ((_ChipRenderTheme__chip)this.theme).padding.vertical));
                        deleteIconOffset = centerLayout(((_ChipSizes__chip)sizes).deleteIcon, start);
                    }
                    else
                    {
                        _deleteButtonRect = Rect.zero;
                    }
                    start -= ((_ChipSizes__chip)sizes).deleteIcon.width;
                    if (((_ChipRenderTheme__chip)this.theme).canTapBody)
                    {
                        _pressRect = global::Doroti.Ui.Rect.fromLTWH(this._deleteButtonRect.width, 0.0, ((((_ChipSizes__chip)sizes).overall.width - this._deleteButtonRect.width) + ((_ChipRenderTheme__chip)this.theme).padding.horizontal), (((_ChipSizes__chip)sizes).overall.height + ((_ChipRenderTheme__chip)this.theme).padding.vertical));
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
                    if ((((_ChipRenderTheme__chip)this.theme).showCheckmark || ((_ChipRenderTheme__chip)this.theme).showAvatar))
                    {
                        avatarOffset = centerLayout(((_ChipSizes__chip)sizes).avatar, ((startLocal - ((global::Doroti.Framework.Rendering.RenderBox)this.avatar).size.width) + ((_ChipSizes__chip)sizes).avatar.width));
                        startLocal += ((_ChipSizes__chip)sizes).avatar.width;
                    }
                    labelOffset = centerLayout(((_ChipSizes__chip)sizes).label, startLocal);
                    startLocal += ((_ChipSizes__chip)sizes).label.width;
                    if (((_ChipRenderTheme__chip)this.theme).canTapBody)
                    {
                        _pressRect = global::Doroti.Ui.Rect.fromLTWH(0.0, 0.0, (this.deleteIconShowing ? (startLocal + ((_ChipRenderTheme__chip)this.theme).padding.left) : (((_ChipSizes__chip)sizes).overall.width + ((_ChipRenderTheme__chip)this.theme).padding.horizontal)), (((_ChipSizes__chip)sizes).overall.height + ((_ChipRenderTheme__chip)this.theme).padding.vertical));
                    }
                    else
                    {
                        _pressRect = Rect.zero;
                    }
                    startLocal -= (((global::Doroti.Framework.Rendering.RenderBox)this.deleteIcon).size.width - ((_ChipSizes__chip)sizes).deleteIcon.width);
                    if (this.deleteIconShowing)
                    {
                        deleteIconOffset = centerLayout(((_ChipSizes__chip)sizes).deleteIcon, startLocal);
                        _deleteButtonRect = global::Doroti.Ui.Rect.fromLTWH((startLocal + ((_ChipRenderTheme__chip)this.theme).padding.left), 0.0, (((_ChipSizes__chip)sizes).deleteIcon.width + ((_ChipRenderTheme__chip)this.theme).padding.right), (((_ChipSizes__chip)sizes).overall.height + ((_ChipRenderTheme__chip)this.theme).padding.vertical));
                    }
                    else
                    {
                        _deleteButtonRect = Rect.zero;
                    }
                    break;
                }
        }
        labelOffset = (labelOffset + new global::Doroti.Ui.Offset(0.0, (((((((_ChipSizes__chip)sizes).label.height - ((_ChipRenderTheme__chip)this.theme).labelPadding.vertical)) - ((global::Doroti.Framework.Rendering.RenderBox)this.label).size.height)) / 2.0)));
        _RenderChip__chip._boxParentData(this.avatar).offset = (((_ChipRenderTheme__chip)this.theme).padding.topLeft + avatarOffset);
        _RenderChip__chip._boxParentData(this.label).offset = ((((_ChipRenderTheme__chip)this.theme).padding.topLeft + labelOffset) + ((_ChipRenderTheme__chip)this.theme).labelPadding.topLeft);
        _RenderChip__chip._boxParentData(this.deleteIcon).offset = (((_ChipRenderTheme__chip)this.theme).padding.topLeft + deleteIconOffset);
        var paddedSize = new global::Doroti.Ui.Size((((_ChipSizes__chip)sizes).overall.width + ((_ChipRenderTheme__chip)this.theme).padding.horizontal), (((_ChipSizes__chip)sizes).overall.height + ((_ChipRenderTheme__chip)this.theme).padding.vertical));
        size = this.constraints.constrain(paddedSize);
        DartRuntimePrimitives.Assert(() => (this.size.height == this.constraints.constrainHeight(paddedSize.height)), () => (object?)$"Constrained height {this.size.height} doesn't match expected height " + $"{this.constraints.constrainWidth(paddedSize.height)}");
        DartRuntimePrimitives.Assert(() => (this.size.width == this.constraints.constrainWidth(paddedSize.width)), () => (object?)$"Constrained width {this.size.width} doesn't match expected width " + $"{this.constraints.constrainWidth(paddedSize.width)}");
    }

    internal virtual global::Doroti.Ui.Color _disabledColor
    {
        get
        {
            if (((global::Doroti.Framework.Animation.Animation<double>)this.enableAnimation).isCompleted)
            {
                return Colors.white;
            }
            global::Doroti.Ui.Color color = ((global::Doroti.Ui.Color)(object?)(((_ChipRenderTheme__chip)this.theme).brightness switch { Brightness.light => Colors.white, Brightness.dark => Colors.black, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") }));
            return new global::Doroti.Framework.Animation.ColorTween(begin: color.withAlpha(ChipLibrary._kDisabledAlpha), end: color).evaluate(this.enableAnimation)!;
            return default!;
        }
    }
    internal virtual void _paintCheck(Canvas canvas, Offset origin, double size)
    {
        global::Doroti.Ui.Color? paintColor = ((global::Doroti.Ui.Color?)(object?)(((_ChipRenderTheme__chip)this.theme).checkmarkColor ?? ((((_ChipRenderTheme__chip)this.theme).brightness, ((_ChipRenderTheme__chip)this.theme).showAvatar) switch { (Brightness.light, true) => Colors.white, (Brightness.light, false) => Colors.black.withAlpha(ChipLibrary._kCheckmarkAlpha), (Brightness.dark, true) => Colors.black, (Brightness.dark, false) => Colors.white.withAlpha(ChipLibrary._kCheckmarkAlpha) })));
        var fadeTween = new global::Doroti.Framework.Animation.ColorTween(begin: Colors.transparent, end: paintColor);
        paintColor = ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.checkmarkAnimation).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)) ? fadeTween.evaluate(this.checkmarkAnimation) : paintColor);
        var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = paintColor!;
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = ((ChipLibrary._kCheckmarkStrokeWidth * ((global::Doroti.Framework.Rendering.RenderBox)this.avatar).size.height) / 24.0);
    return __cascade;
}))();
        double t = ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.checkmarkAnimation).status, global::Doroti.Framework.Animation.AnimationStatus.reverse)) ? 1.0 : ((global::Doroti.Framework.Animation.Animation<double>)this.checkmarkAnimation).value);
        if ((t == 0.0))
        {
            return;
        }
        DartRuntimePrimitives.Assert(() => ((t > 0.0) && (t <= 1.0)));
        var path = new global::Doroti.Ui.Path();
        var start = new global::Doroti.Ui.Offset((size * 0.15), (size * 0.45));
        var mid = new global::Doroti.Ui.Offset((size * 0.4), (size * 0.7));
        var endLocal = new global::Doroti.Ui.Offset((size * 0.85), (size * 0.25));
        if ((t < 0.5))
        {
            double strokeT = (t * 2.0);
            global::Doroti.Ui.Offset drawMid = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(start, mid, strokeT)));
            path.moveTo((origin.dx + start.dx), (origin.dy + start.dy));
            path.lineTo((origin.dx + drawMid.dx), (origin.dy + drawMid.dy));
        }
        else
        {
            double strokeTLocal = (((t - 0.5)) * 2.0);
            global::Doroti.Ui.Offset drawEnd = ((global::Doroti.Ui.Offset)(object?)DartRuntimePrimitives.RequireValue(Dart_uiLibrary.Offset.lerp(mid, endLocal, strokeTLocal)));
            path.moveTo((origin.dx + start.dx), (origin.dy + start.dy));
            path.lineTo((origin.dx + mid.dx), (origin.dy + mid.dy));
            path.lineTo((origin.dx + drawEnd.dx), (origin.dy + drawEnd.dy));
        }
        canvas.drawPath(path, paint);
    }

    internal virtual void _paintSelectionOverlay(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        if (this.isDrawingCheckmark)
        {
            if (((_ChipRenderTheme__chip)this.theme).showAvatar)
            {
                global::Doroti.Ui.Rect avatarRect = ((global::Doroti.Ui.Rect)(object?)_RenderChip__chip._boxRect(this.avatar).shift(offset));
                var darkenPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = selectionScrimTween.evaluate(this.checkmarkAnimation)!;
    __cascade.blendMode = BlendMode.srcATop;
    return __cascade;
}))();
                if (this.avatarBorder!.preferPaintInterior)
                {
                    this.avatarBorder!.paintInterior(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, avatarRect, darkenPaint);
                }
                else
                {
                    global::Doroti.Ui.Path path = ((global::Doroti.Ui.Path)(object?)this.avatarBorder!.getOuterPath(avatarRect));
                    ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawPath(path, darkenPaint);
                }
            }
            double checkSize = (((global::Doroti.Framework.Rendering.RenderBox)this.avatar).size.height * 0.75);
            global::Doroti.Ui.Offset checkOffset = ((global::Doroti.Ui.Offset)(object?)(_RenderChip__chip._boxParentData(this.avatar).offset + new global::Doroti.Ui.Offset((((global::Doroti.Framework.Rendering.RenderBox)this.avatar).size.height * 0.125), (((global::Doroti.Framework.Rendering.RenderBox)this.avatar).size.height * 0.125))));
            _paintCheck(((global::Doroti.Framework.Rendering.PaintingContext)context).canvas, (offset + checkOffset), checkSize);
        }
    }

    internal virtual void _paintAvatar(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        void paintWithOverlay(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
        {
            context.paintChild(this.avatar, (_RenderChip__chip._boxParentData(this.avatar).offset + offset));
            _paintSelectionOverlay(context, offset);
        }
        if ((!((_ChipRenderTheme__chip)this.theme).showAvatar && ((global::Doroti.Framework.Animation.Animation<double>)this.avatarDrawerAnimation).isDismissed))
        {
            this._avatarOpacityLayerHandler.layer = null;
            return;
        }
        global::Doroti.Ui.Color disabledColor = ((global::Doroti.Ui.Color)(object?)this._disabledColor);
        long disabledColorAlpha = disabledColor.alpha;
        if (this.needsCompositing)
        {
            this._avatarOpacityLayerHandler.layer = context.pushOpacity(offset, disabledColorAlpha, (global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)paintWithOverlay, oldLayer: ((global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer>)this._avatarOpacityLayerHandler).layer);
        }
        else
        {
            this._avatarOpacityLayerHandler.layer = null;
            if ((disabledColorAlpha != 255L))
            {
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.saveLayer(_RenderChip__chip._boxRect(this.avatar).shift(offset).inflate(20.0), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = disabledColor;
    return __cascade;
}))());
            }
            paintWithOverlay(context, offset);
            if ((disabledColorAlpha != 255L))
            {
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
            }
        }
    }

    internal virtual void _paintChild(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset, global::Doroti.Framework.Rendering.RenderBox? child, bool isDeleteIcon)
    {
        if ((child is null))
        {
            this._labelOpacityLayerHandler.layer = null;
            this._deleteIconOpacityLayerHandler.layer = null;
            return;
        }
        long disabledColorAlpha = this._disabledColor.alpha;
        if (!((global::Doroti.Framework.Animation.Animation<double>)this.enableAnimation).isCompleted)
        {
            if (this.needsCompositing)
            {
                this._labelOpacityLayerHandler.layer = context.pushOpacity(offset, disabledColorAlpha, ((global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((context, offset) =>
                {
                    context.paintChild(child, (_RenderChip__chip._boxParentData(child).offset + offset));
                })), oldLayer: ((global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer>)this._labelOpacityLayerHandler).layer);
                if (isDeleteIcon)
                {
                    this._deleteIconOpacityLayerHandler.layer = context.pushOpacity(offset, disabledColorAlpha, ((global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((context, offset) =>
                    {
                        context.paintChild(child, (_RenderChip__chip._boxParentData(child).offset + offset));
                    })), oldLayer: ((global::Doroti.Framework.Rendering.LayerHandle<global::Doroti.Framework.Rendering.OpacityLayer>)this._deleteIconOpacityLayerHandler).layer);
                }
            }
            else
            {
                this._labelOpacityLayerHandler.layer = null;
                this._deleteIconOpacityLayerHandler.layer = null;
                global::Doroti.Ui.Rect childRect = ((global::Doroti.Ui.Rect)(object?)_RenderChip__chip._boxRect(child).shift(offset));
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.saveLayer(childRect.inflate(20.0), ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = this._disabledColor;
    return __cascade;
}))());
                context.paintChild(child, (_RenderChip__chip._boxParentData(child).offset + offset));
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.restore();
            }
        }
        else
        {
            context.paintChild(child, (_RenderChip__chip._boxParentData(child).offset + offset));
        }
    }

    public override void attach(global::Doroti.Framework.Rendering.PipelineOwner owner)
    {
        base.attach(owner);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            child.attach(owner);
        }
        this.checkmarkAnimation.addListener(this.markNeedsPaint);
        this.avatarDrawerAnimation.addListener(this.markNeedsLayout);
        this.deleteDrawerAnimation.addListener(this.markNeedsLayout);
        this.enableAnimation.addListener(this.markNeedsPaint);
    }

    public override void detach()
    {
        this.checkmarkAnimation.removeListener(this.markNeedsPaint);
        this.avatarDrawerAnimation.removeListener(this.markNeedsLayout);
        this.deleteDrawerAnimation.removeListener(this.markNeedsLayout);
        this.enableAnimation.removeListener(this.markNeedsPaint);
        base.detach();
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            child.detach();
        }
    }

    public override void dispose()
    {
        this._labelOpacityLayerHandler.layer = null;
        this._deleteIconOpacityLayerHandler.layer = null;
        this._avatarOpacityLayerHandler.layer = null;
        base.dispose();
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        _paintAvatar(context, offset);
        if (this.deleteIconShowing)
        {
            _paintChild(context, offset, this.deleteIcon, isDeleteIcon: true);
        }
        _paintChild(context, offset, this.label, isDeleteIcon: false);
    }

    public override void debugPaint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => (!_debugShowTapTargetOutlines || ((global::System.Func<bool>)(() =>
        {
            var outlinePaint = ((Func<Paint>)(() =>
            {
                var __cascade = new global::Doroti.Ui.Paint();
                __cascade.color = new global::Doroti.Ui.Color(4286578688L);
                __cascade.strokeWidth = 1.0;
                __cascade.style = PaintingStyle.stroke;
                return __cascade;
            }))();
            if (this.deleteIconShowing)
            {
                ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(this._deleteButtonRect.shift(offset), outlinePaint);
            }
            ((global::Doroti.Framework.Rendering.PaintingContext)context).canvas.drawRect(this._pressRect.shift(offset), ((Func<Paint>)(() =>
            {
                var __cascade = outlinePaint;
                __cascade.color = new global::Doroti.Ui.Color(4278222848L);
                return __cascade;
            }))());
            return true;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))()));
    }

    public override bool hitTestSelf(Offset position) => DartRuntimePrimitives.ConvertValue<bool>((this._deleteButtonRect.contains(position) || this._pressRect.contains(position)));
    public virtual global::Doroti.Framework.Rendering.RenderBox? childForSlot(_ChipSlot__chip slot) => this._slotToChild.GetValueOrDefault(slot);
    public virtual string debugNameForSlot(_ChipSlot__chip slot)
    {
        if (true)
        {
            return slot.ToString();
        }
        return slot.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void redepthChildren()
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)this.redepthChild)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override void visitChildren(global::System.Action<global::Doroti.Framework.Rendering.RenderObject> visitor)
    {
        this.children.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)visitor)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
    }

    public override List<global::Doroti.Framework.Foundation.DiagnosticsNode> debugDescribeChildren()
    {
        var value = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        var childToSlot = new DartMap<global::Doroti.Framework.Rendering.RenderBox, _ChipSlot__chip>(this._slotToChild.Values, this._slotToChild.Keys);
        foreach (global::Doroti.Framework.Rendering.RenderBox child in this.children)
        {
            _addDiagnostics(child, value, debugNameForSlot(((_ChipSlot__chip)DartRuntimePrimitives.RequireValue(DartCollectionRuntime.NullableMapValue<_ChipSlot__chip>(childToSlot, child)))));
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
        global::Doroti.Framework.Rendering.RenderBox? oldChild = this._slotToChild.GetValueOrDefault(slot);
        if ((oldChild is not null))
        {
            dropChild(oldChild);
            this._slotToChild.remove(slot);
        }
        if ((child is not null))
        {
            this._slotToChild[slot] = child;
            adoptChild(child);
        }
    }

    public virtual void _moveChild(global::Doroti.Framework.Rendering.RenderBox child, _ChipSlot__chip slot, _ChipSlot__chip oldSlot)
    {
        DartRuntimePrimitives.Assert(() => (!object.Equals(slot, oldSlot)));
        global::Doroti.Framework.Rendering.RenderBox? oldChild = this._slotToChild.GetValueOrDefault(oldSlot);
        if ((object.Equals(oldChild, child)))
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
        return ((InteractiveInkFeature)(object?)this.parentFactory.create(controller: controller, referenceBox: referenceBox, position: position, color: color, rectCallback: rectCallback, borderRadius: borderRadius, customBorder: customBorder, radius: radius, onRemoved: onRemoved, textDirection: textDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class ChipLibrary
{
    internal static bool _hitIsOnDeleteIcon(global::Doroti.Framework.Painting.EdgeInsetsGeometry padding, global::Doroti.Framework.Painting.EdgeInsetsGeometry labelPadding, Offset tapPosition, Size chipSize, Size deleteButtonSize, TextDirection textDirection)
    {
        global::Doroti.Framework.Painting.EdgeInsets resolvedPadding = ((global::Doroti.Framework.Painting.EdgeInsets)(object?)padding.resolve(textDirection));
        global::Doroti.Ui.Size deflatedSize = ((global::Doroti.Ui.Size)(object?)resolvedPadding.deflateSize(chipSize));
        global::Doroti.Ui.Offset adjustedPosition = ((global::Doroti.Ui.Offset)(object?)(tapPosition - new global::Doroti.Ui.Offset(((global::Doroti.Framework.Painting.EdgeInsets)resolvedPadding).left, ((global::Doroti.Framework.Painting.EdgeInsets)resolvedPadding).top)));
        double accessibleDeleteButtonWidth = Math.Min((deflatedSize.width * 0.499), Math.Min((labelPadding.resolve(textDirection).right + deleteButtonSize.width), (24.0 + (deleteButtonSize.width / 2.0))));
        return (textDirection switch { TextDirection.ltr => (adjustedPosition.dx >= (deflatedSize.width - accessibleDeleteButtonWidth)), TextDirection.rtl => (adjustedPosition.dx <= accessibleDeleteButtonWidth), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderEnsureMinSemanticsSize__chip(this.semanticSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderEnsureMinSemanticsSize__chip)(object)renderObject;
        __renderObject.semanticSize = this.semanticSize;
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
        get => this._semanticSize;
        set
        {
            var __value = value;
            if ((object.Equals(this._semanticSize, __value)))
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
            return global::Doroti.Ui.Rect.fromCenter(center: ((Offset)((dynamic)this.paintBounds).center), width: Math.Max(this._semanticSize.width, this.size.width), height: Math.Max(this._semanticSize.height, this.size.height));
            return default!;
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
                __late__colors = Theme.of(this.context).colorScheme;
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
                __late__textTheme = Theme.of(this.context).textTheme;
                __late__textTheme_initialized = true;
            }
            return __late__textTheme;
        }
    }

    internal _ChipDefaultsM3__chip(global::Doroti.Framework.Widgets.BuildContext context, bool isEnabled) : base(elevation: 0.0, shape: new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(8.0))), showCheckmark: true)
    {
        this.context = context;
        this.isEnabled = isEnabled;
    }

    public override global::Doroti.Framework.Painting.TextStyle? labelStyle => this._textTheme.labelLarge?.copyWith(color: (this.isEnabled ? this._colors.onSurfaceVariant : this._colors.onSurface));
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>(null);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(Colors.transparent);
    public virtual global::Doroti.Ui.Color? checkmarkColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(null);
    public virtual global::Doroti.Ui.Color? deleteIconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this.isEnabled ? this._colors.onSurfaceVariant : this._colors.onSurface));
    public override global::Doroti.Framework.Painting.BorderSide? side => (this.isEnabled ? new global::Doroti.Framework.Painting.BorderSide(color: this._colors.outlineVariant) : new global::Doroti.Framework.Painting.BorderSide(color: this._colors.onSurface.withOpacity(0.12)));
    public override global::Doroti.Framework.Widgets.IconThemeData? iconTheme => new global::Doroti.Framework.Widgets.IconThemeData(color: (this.isEnabled ? this._colors.primary : this._colors.onSurface), size: 18.0);
    public override global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding => DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Painting.EdgeInsetsGeometry>(global::Doroti.Framework.Painting.EdgeInsets.CreateAll(8.0));
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding
    {
        get
        {
            double fontSizeLocal = (this.labelStyle?.fontSize ?? 14.0);
            double fontSizeRatio = (MediaQuery.textScalerOf(this.context).scale(fontSizeLocal) / 14.0);
            return ((global::Doroti.Framework.Painting.EdgeInsetsGeometry?)(object?)EdgeInsets.lerp(global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 8.0), global::Doroti.Framework.Painting.EdgeInsets.CreateSymmetric(horizontal: 4.0), Dart_uiLibrary.clampDouble((fontSizeRatio - 1.0), 0.0, 1.0))!);
            return default!;
        }
    }
}
