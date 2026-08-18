// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/chip_theme.dart
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace Doroti.Framework.Material;

public class ChipTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual ChipThemeData data { get; private set; } = default!;

    public ChipTheme(global::Doroti.Framework.Foundation.Key? key = null, ChipThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ChipThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ChipTheme? inheritedTheme__3187 = ((ChipTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ChipTheme>());
        return (inheritedTheme__3187?.data ?? Theme.of(context).chipTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new ChipTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ChipTheme)oldWidget).data)));
}

public class ChipThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? deleteIconColor { get; private set; }
    public virtual Color? disabledColor { get; private set; }
    public virtual Color? selectedColor { get; private set; }
    public virtual Color? secondarySelectedColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? selectedShadowColor { get; private set; }
    public virtual bool? showCheckmark { get; private set; }
    public virtual Color? checkmarkColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.BorderSide? side { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? labelStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? secondaryLabelStyle { get; private set; }
    public virtual Brightness? brightness { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IconThemeData? iconTheme { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints { get; private set; }

    public ChipThemeData(global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, Color? deleteIconColor = null, Color? disabledColor = null, Color? selectedColor = null, Color? secondarySelectedColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? selectedShadowColor = null, bool? showCheckmark = null, Color? checkmarkColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? secondaryLabelStyle = null, Brightness? brightness = null, double? elevation = null, double? pressElevation = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null)
    {
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.deleteIconColor = deleteIconColor;
        this.disabledColor = disabledColor;
        this.selectedColor = selectedColor;
        this.secondarySelectedColor = secondarySelectedColor;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.selectedShadowColor = selectedShadowColor;
        this.showCheckmark = showCheckmark;
        this.checkmarkColor = checkmarkColor;
        this.labelPadding = labelPadding;
        this.padding = padding;
        this.side = side;
        this.shape = shape;
        this.labelStyle = labelStyle;
        this.secondaryLabelStyle = secondaryLabelStyle;
        this.brightness = brightness;
        this.elevation = elevation;
        this.pressElevation = pressElevation;
        this.iconTheme = iconTheme;
        this.avatarBoxConstraints = avatarBoxConstraints;
        this.deleteIconBoxConstraints = deleteIconBoxConstraints;
    }

    public static ChipThemeData CreateFromDefaults(Brightness? brightness = null, Color? primaryColor = null, Color secondaryColor = default!, global::Doroti.Framework.Painting.TextStyle labelStyle = default!)
    {
        DartRuntimePrimitives.Assert(() => ((primaryColor is not null) || (brightness is not null)), () => (object?)"One of primaryColor or brightness must be specified");
        DartRuntimePrimitives.Assert(() => ((primaryColor is null) || (brightness is null)), () => (object?)"Only one of primaryColor or brightness may be specified");
        if ((primaryColor is not null))
        {
            brightness = ThemeData.estimateBrightnessForColor(primaryColor);
        }
        var backgroundAlpha__8667 = 31L;
        var deleteIconAlpha__8708 = 222L;
        var disabledAlpha__8749 = 12L;
        var selectAlpha__8799 = 61L;
        var textLabelAlpha__8848 = 222L;
        global::Doroti.Framework.Painting.EdgeInsetsGeometry padding__8907 = ((global::Doroti.Framework.Painting.EdgeInsetsGeometry)(object?)global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0));
        primaryColor = (primaryColor ?? (((object.Equals(brightness, Brightness.light)) ? Colors.black : Colors.white)));
        global::Doroti.Ui.Color backgroundColor__9054 = ((global::Doroti.Ui.Color)(object?)primaryColor.withAlpha(backgroundAlpha__8667));
        global::Doroti.Ui.Color deleteIconColor__9129 = ((global::Doroti.Ui.Color)(object?)primaryColor.withAlpha(deleteIconAlpha__8708));
        global::Doroti.Ui.Color disabledColor__9204 = ((global::Doroti.Ui.Color)(object?)primaryColor.withAlpha(disabledAlpha__8749));
        global::Doroti.Ui.Color selectedColor__9275 = ((global::Doroti.Ui.Color)(object?)primaryColor.withAlpha(selectAlpha__8799));
        global::Doroti.Ui.Color secondarySelectedColor__9344 = ((global::Doroti.Ui.Color)(object?)secondaryColor.withAlpha(selectAlpha__8799));
        global::Doroti.Framework.Painting.TextStyle secondaryLabelStyle__9428 = ((global::Doroti.Framework.Painting.TextStyle)(object?)labelStyle.copyWith(color: secondaryColor.withAlpha(textLabelAlpha__8848)));
        labelStyle = labelStyle.copyWith(color: primaryColor.withAlpha(textLabelAlpha__8848));
        return new ChipThemeData(backgroundColor: backgroundColor__9054, deleteIconColor: deleteIconColor__9129, disabledColor: disabledColor__9204, selectedColor: selectedColor__9275, secondarySelectedColor: secondarySelectedColor__9344, shadowColor: Colors.black, selectedShadowColor: Colors.black, showCheckmark: true, padding: padding__8907, labelStyle: labelStyle, secondaryLabelStyle: secondaryLabelStyle__9428, brightness: brightness, elevation: 0.0, pressElevation: 8.0, iconTheme: new global::Doroti.Framework.Widgets.IconThemeData(size: 18.0));
    }

    public virtual ChipThemeData copyWith(global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? color = null, Color? backgroundColor = null, Color? deleteIconColor = null, Color? disabledColor = null, Color? selectedColor = null, Color? secondarySelectedColor = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? selectedShadowColor = null, bool? showCheckmark = null, Color? checkmarkColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? labelPadding = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, global::Doroti.Framework.Painting.BorderSide? side = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Painting.TextStyle? labelStyle = null, global::Doroti.Framework.Painting.TextStyle? secondaryLabelStyle = null, Brightness? brightness = null, double? elevation = null, double? pressElevation = null, global::Doroti.Framework.Widgets.IconThemeData? iconTheme = null, global::Doroti.Framework.Rendering.BoxConstraints? avatarBoxConstraints = null, global::Doroti.Framework.Rendering.BoxConstraints? deleteIconBoxConstraints = null)
    {
        return new ChipThemeData(color: (color ?? this.color), backgroundColor: (backgroundColor ?? this.backgroundColor), deleteIconColor: (deleteIconColor ?? this.deleteIconColor), disabledColor: (disabledColor ?? this.disabledColor), selectedColor: (selectedColor ?? this.selectedColor), secondarySelectedColor: (secondarySelectedColor ?? this.secondarySelectedColor), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), selectedShadowColor: (selectedShadowColor ?? this.selectedShadowColor), showCheckmark: (showCheckmark ?? this.showCheckmark), checkmarkColor: (checkmarkColor ?? this.checkmarkColor), labelPadding: (labelPadding ?? this.labelPadding), padding: (padding ?? this.padding), side: (side ?? this.side), shape: (shape ?? this.shape), labelStyle: (labelStyle ?? this.labelStyle), secondaryLabelStyle: (secondaryLabelStyle ?? this.secondaryLabelStyle), brightness: (brightness ?? this.brightness), elevation: (elevation ?? this.elevation), pressElevation: (pressElevation ?? this.pressElevation), iconTheme: (iconTheme ?? this.iconTheme), avatarBoxConstraints: (avatarBoxConstraints ?? this.avatarBoxConstraints), deleteIconBoxConstraints: (deleteIconBoxConstraints ?? this.deleteIconBoxConstraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ChipThemeData? lerp(ChipThemeData? a, ChipThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ChipThemeData(color: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.color, b?.color, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), deleteIconColor: Dart_uiLibrary.Color.lerp(a?.deleteIconColor, b?.deleteIconColor, t), disabledColor: Dart_uiLibrary.Color.lerp(a?.disabledColor, b?.disabledColor, t), selectedColor: Dart_uiLibrary.Color.lerp(a?.selectedColor, b?.selectedColor, t), secondarySelectedColor: Dart_uiLibrary.Color.lerp(a?.secondarySelectedColor, b?.secondarySelectedColor, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), selectedShadowColor: Dart_uiLibrary.Color.lerp(a?.selectedShadowColor, b?.selectedShadowColor, t), showCheckmark: ((t < 0.5) ? (a?.showCheckmark ?? true) : (b?.showCheckmark ?? true)), checkmarkColor: Dart_uiLibrary.Color.lerp(a?.checkmarkColor, b?.checkmarkColor, t), labelPadding: EdgeInsetsGeometry.lerp(a?.labelPadding, b?.labelPadding, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t), side: ChipThemeData._lerpSides(a?.side, b?.side, t), shape: OutlinedBorder.lerp(a?.shape, b?.shape, t), labelStyle: TextStyle.lerp(a?.labelStyle, b?.labelStyle, t), secondaryLabelStyle: TextStyle.lerp(a?.secondaryLabelStyle, b?.secondaryLabelStyle, t), brightness: ((t < 0.5) ? (a?.brightness ?? Brightness.light) : (b?.brightness ?? Brightness.light)), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), pressElevation: Dart_uiLibrary.lerpDouble(a?.pressElevation, b?.pressElevation, t), iconTheme: (((a?.iconTheme is not null) || (b?.iconTheme is not null)) ? IconThemeData.lerp(a?.iconTheme, b?.iconTheme, t) : null), avatarBoxConstraints: BoxConstraints.lerp(a?.avatarBoxConstraints, b?.avatarBoxConstraints, t), deleteIconBoxConstraints: BoxConstraints.lerp(a?.deleteIconBoxConstraints, b?.deleteIconBoxConstraints, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Painting.BorderSide? _lerpSides(global::Doroti.Framework.Painting.BorderSide? a, global::Doroti.Framework.Painting.BorderSide? b, double t)
    {
        if (((a is null) && (b is null)))
        {
            return null;
        }
        if ((a is global::Doroti.Framework.Widgets.WidgetStateBorderSide))
        {
            a = ((global::Doroti.Framework.Widgets.WidgetStateBorderSide)a).resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>());
        }
        if ((b is global::Doroti.Framework.Widgets.WidgetStateBorderSide))
        {
            b = ((global::Doroti.Framework.Widgets.WidgetStateBorderSide)b).resolve(new HashSet<global::Doroti.Framework.Widgets.WidgetState>());
        }
        a ??= new global::Doroti.Framework.Painting.BorderSide(width: 0, color: b!.color.withAlpha(0L));
        b ??= new global::Doroti.Framework.Painting.BorderSide(width: 0, color: ((global::Doroti.Framework.Painting.BorderSide)a).color.withAlpha(0L));
        return ((global::Doroti.Framework.Painting.BorderSide?)(object?)BorderSide.lerp(a, b, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(new List<object?> { this.color, this.backgroundColor, this.deleteIconColor, this.disabledColor, this.selectedColor, this.secondarySelectedColor, this.shadowColor, this.surfaceTintColor, this.selectedShadowColor, this.showCheckmark, this.checkmarkColor, this.labelPadding, this.padding, this.side, this.shape, this.labelStyle, this.secondaryLabelStyle, this.brightness, this.elevation, this.pressElevation, this.iconTheme, this.avatarBoxConstraints, this.deleteIconBoxConstraints }));
    public override bool Equals(object? other)
    {
        var __other = other as ChipThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((((((((__other is ChipThemeData) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).color, this.color))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).deleteIconColor, this.deleteIconColor))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).disabledColor, this.disabledColor))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).selectedColor, this.selectedColor))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).secondarySelectedColor, this.secondarySelectedColor))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).selectedShadowColor, this.selectedShadowColor))) && (((ChipThemeData)((ChipThemeData)__other)).showCheckmark == this.showCheckmark)) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).checkmarkColor, this.checkmarkColor))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).labelPadding, this.labelPadding))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).padding, this.padding))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).side, this.side))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).shape, this.shape))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).labelStyle, this.labelStyle))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).secondaryLabelStyle, this.secondaryLabelStyle))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).brightness, this.brightness))) && (((ChipThemeData)((ChipThemeData)__other)).elevation == this.elevation)) && (((ChipThemeData)((ChipThemeData)__other)).pressElevation == this.pressElevation)) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).iconTheme, this.iconTheme))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).avatarBoxConstraints, this.avatarBoxConstraints))) && (object.Equals(((ChipThemeData)((ChipThemeData)__other)).deleteIconBoxConstraints, this.deleteIconBoxConstraints)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("deleteIconColor", this.deleteIconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("disabledColor", this.disabledColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedColor", this.selectedColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondarySelectedColor", this.secondarySelectedColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("selectedShadowColor", this.selectedShadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("showCheckmark", this.showCheckmark, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("checkMarkColor", this.checkmarkColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("labelPadding", this.labelPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.BorderSide>("side", this.side, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("labelStyle", this.labelStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("secondaryLabelStyle", this.secondaryLabelStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.Brightness>("brightness", this.brightness, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("pressElevation", this.pressElevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.IconThemeData>("iconTheme", this.iconTheme, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("avatarBoxConstraints", this.avatarBoxConstraints, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("deleteIconBoxConstraints", this.deleteIconBoxConstraints, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString__105654 ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
