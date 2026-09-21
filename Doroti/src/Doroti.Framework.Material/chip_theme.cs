// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/chip_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ChipTheme : InheritedTheme
{
    public virtual ChipThemeData data { get; private set; } = default!;

    public ChipTheme(Key? key = null, ChipThemeData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ChipThemeData of(BuildContext context)
    {
        ChipTheme? inheritedTheme = context.dependOnInheritedWidgetOfExactType<ChipTheme>();
        return inheritedTheme?.data ?? Theme.of(context).chipTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new ChipTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((ChipTheme)oldWidget).data));
}

public class ChipThemeData : Diagnosticable
{
    public virtual WidgetStateProperty<Color?>? color { get; private set; }
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
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }
    public virtual EdgeInsetsGeometry? padding { get; private set; }
    public virtual BorderSide? side { get; private set; }
    public virtual OutlinedBorder? shape { get; private set; }
    public virtual TextStyle? labelStyle { get; private set; }
    public virtual TextStyle? secondaryLabelStyle { get; private set; }
    public virtual Brightness? brightness { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual double? pressElevation { get; private set; }
    public virtual IconThemeData? iconTheme { get; private set; }
    public virtual BoxConstraints? avatarBoxConstraints { get; private set; }
    public virtual BoxConstraints? deleteIconBoxConstraints { get; private set; }

    public ChipThemeData(
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        Color? deleteIconColor = null,
        Color? disabledColor = null,
        Color? selectedColor = null,
        Color? secondarySelectedColor = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        Color? selectedShadowColor = null,
        bool? showCheckmark = null,
        Color? checkmarkColor = null,
        EdgeInsetsGeometry? labelPadding = null,
        EdgeInsetsGeometry? padding = null,
        BorderSide? side = null,
        OutlinedBorder? shape = null,
        TextStyle? labelStyle = null,
        TextStyle? secondaryLabelStyle = null,
        Brightness? brightness = null,
        double? elevation = null,
        double? pressElevation = null,
        IconThemeData? iconTheme = null,
        BoxConstraints? avatarBoxConstraints = null,
        BoxConstraints? deleteIconBoxConstraints = null
    )
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

    public static ChipThemeData CreateFromDefaults(
        Brightness? brightness = null,
        Color? primaryColor = null,
        Color secondaryColor = default!,
        TextStyle labelStyle = default!
    )
    {
        DartRuntimePrimitives.Assert(
            () => (primaryColor is not null) || (brightness is not null),
            () => (object?)"One of primaryColor or brightness must be specified"
        );
        DartRuntimePrimitives.Assert(
            () => (primaryColor is null) || (brightness is null),
            () => (object?)"Only one of primaryColor or brightness may be specified"
        );
        if (primaryColor is not null)
        {
            brightness = ThemeData.estimateBrightnessForColor(primaryColor);
        }
        var backgroundAlpha = 31L;
        var deleteIconAlpha = 222L;
        var disabledAlpha = 12L;
        var selectAlpha = 61L;
        var textLabelAlpha = 222L;
        EdgeInsetsGeometry paddingLocal = EdgeInsets.CreateAll(4.0);
        primaryColor =
            primaryColor ?? (Equals(brightness, Brightness.light) ? Colors.black : Colors.white);
        Color backgroundColorLocal = primaryColor.withAlpha(backgroundAlpha);
        Color deleteIconColorLocal = primaryColor.withAlpha(deleteIconAlpha);
        Color disabledColorLocal = primaryColor.withAlpha(disabledAlpha);
        Color selectedColorLocal = primaryColor.withAlpha(selectAlpha);
        Color secondarySelectedColorLocal = secondaryColor.withAlpha(selectAlpha);
        TextStyle secondaryLabelStyleLocal = labelStyle.copyWith(
            color: secondaryColor.withAlpha(textLabelAlpha)
        );
        labelStyle = labelStyle.copyWith(color: primaryColor.withAlpha(textLabelAlpha));
        return new ChipThemeData(
            backgroundColor: backgroundColorLocal,
            deleteIconColor: deleteIconColorLocal,
            disabledColor: disabledColorLocal,
            selectedColor: selectedColorLocal,
            secondarySelectedColor: secondarySelectedColorLocal,
            shadowColor: Colors.black,
            selectedShadowColor: Colors.black,
            showCheckmark: true,
            padding: paddingLocal,
            labelStyle: labelStyle,
            secondaryLabelStyle: secondaryLabelStyleLocal,
            brightness: brightness,
            elevation: 0.0,
            pressElevation: 8.0,
            iconTheme: new IconThemeData(size: 18.0)
        );
    }

    public virtual ChipThemeData copyWith(
        WidgetStateProperty<Color?>? color = null,
        Color? backgroundColor = null,
        Color? deleteIconColor = null,
        Color? disabledColor = null,
        Color? selectedColor = null,
        Color? secondarySelectedColor = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        Color? selectedShadowColor = null,
        bool? showCheckmark = null,
        Color? checkmarkColor = null,
        EdgeInsetsGeometry? labelPadding = null,
        EdgeInsetsGeometry? padding = null,
        BorderSide? side = null,
        OutlinedBorder? shape = null,
        TextStyle? labelStyle = null,
        TextStyle? secondaryLabelStyle = null,
        Brightness? brightness = null,
        double? elevation = null,
        double? pressElevation = null,
        IconThemeData? iconTheme = null,
        BoxConstraints? avatarBoxConstraints = null,
        BoxConstraints? deleteIconBoxConstraints = null
    )
    {
        return new ChipThemeData(
            color: color ?? this.color,
            backgroundColor: backgroundColor ?? this.backgroundColor,
            deleteIconColor: deleteIconColor ?? this.deleteIconColor,
            disabledColor: disabledColor ?? this.disabledColor,
            selectedColor: selectedColor ?? this.selectedColor,
            secondarySelectedColor: secondarySelectedColor ?? this.secondarySelectedColor,
            shadowColor: shadowColor ?? this.shadowColor,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            selectedShadowColor: selectedShadowColor ?? this.selectedShadowColor,
            showCheckmark: showCheckmark ?? this.showCheckmark,
            checkmarkColor: checkmarkColor ?? this.checkmarkColor,
            labelPadding: labelPadding ?? this.labelPadding,
            padding: padding ?? this.padding,
            side: side ?? this.side,
            shape: shape ?? this.shape,
            labelStyle: labelStyle ?? this.labelStyle,
            secondaryLabelStyle: secondaryLabelStyle ?? this.secondaryLabelStyle,
            brightness: brightness ?? this.brightness,
            elevation: elevation ?? this.elevation,
            pressElevation: pressElevation ?? this.pressElevation,
            iconTheme: iconTheme ?? this.iconTheme,
            avatarBoxConstraints: avatarBoxConstraints ?? this.avatarBoxConstraints,
            deleteIconBoxConstraints: deleteIconBoxConstraints ?? this.deleteIconBoxConstraints
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static ChipThemeData? lerp(ChipThemeData? a, ChipThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ChipThemeData(
            color: WidgetStateProperty.lerp(a?.color, b?.color, t, Color.lerp),
            backgroundColor: DorotiUiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t),
            deleteIconColor: DorotiUiLibrary.Color.lerp(a?.deleteIconColor, b?.deleteIconColor, t),
            disabledColor: DorotiUiLibrary.Color.lerp(a?.disabledColor, b?.disabledColor, t),
            selectedColor: DorotiUiLibrary.Color.lerp(a?.selectedColor, b?.selectedColor, t),
            secondarySelectedColor: DorotiUiLibrary.Color.lerp(
                a?.secondarySelectedColor,
                b?.secondarySelectedColor,
                t
            ),
            shadowColor: DorotiUiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            surfaceTintColor: DorotiUiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            selectedShadowColor: DorotiUiLibrary.Color.lerp(
                a?.selectedShadowColor,
                b?.selectedShadowColor,
                t
            ),
            showCheckmark: (t < 0.5) ? (a?.showCheckmark ?? true) : (b?.showCheckmark ?? true),
            checkmarkColor: DorotiUiLibrary.Color.lerp(a?.checkmarkColor, b?.checkmarkColor, t),
            labelPadding: EdgeInsetsGeometry.lerp(a?.labelPadding, b?.labelPadding, t),
            padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t),
            side: _lerpSides(a?.side, b?.side, t),
            shape: OutlinedBorder.lerp(a?.shape, b?.shape, t),
            labelStyle: TextStyle.lerp(a?.labelStyle, b?.labelStyle, t),
            secondaryLabelStyle: TextStyle.lerp(a?.secondaryLabelStyle, b?.secondaryLabelStyle, t),
            brightness: (t < 0.5)
                ? (a?.brightness ?? Brightness.light)
                : (b?.brightness ?? Brightness.light),
            elevation: DorotiUiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            pressElevation: DorotiUiLibrary.lerpDouble(a?.pressElevation, b?.pressElevation, t),
            iconTheme: ((a?.iconTheme is not null) || (b?.iconTheme is not null))
                ? IconThemeData.lerp(a?.iconTheme, b?.iconTheme, t)
                : null,
            avatarBoxConstraints: BoxConstraints.lerp(
                a?.avatarBoxConstraints,
                b?.avatarBoxConstraints,
                t
            ),
            deleteIconBoxConstraints: BoxConstraints.lerp(
                a?.deleteIconBoxConstraints,
                b?.deleteIconBoxConstraints,
                t
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal static BorderSide? _lerpSides(BorderSide? a, BorderSide? b, double t)
    {
        if ((a is null) && (b is null))
        {
            return null;
        }
        if (a is WidgetStateBorderSide)
        {
            a = ((WidgetStateBorderSide)a).resolve(new HashSet<WidgetState>());
        }
        if (b is WidgetStateBorderSide)
        {
            b = ((WidgetStateBorderSide)b).resolve(new HashSet<WidgetState>());
        }
        a ??= new BorderSide(width: 0, color: b!.color.withAlpha(0L));
        b ??= new BorderSide(width: 0, color: a.color.withAlpha(0L));
        return (BorderSide?)BorderSide.lerp(a, b, t);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHashAll(
                new List<object?>
                {
                    color,
                    backgroundColor,
                    deleteIconColor,
                    disabledColor,
                    selectedColor,
                    secondarySelectedColor,
                    shadowColor,
                    surfaceTintColor,
                    selectedShadowColor,
                    showCheckmark,
                    checkmarkColor,
                    labelPadding,
                    padding,
                    side,
                    shape,
                    labelStyle,
                    secondaryLabelStyle,
                    brightness,
                    elevation,
                    pressElevation,
                    iconTheme,
                    avatarBoxConstraints,
                    deleteIconBoxConstraints,
                }
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as ChipThemeData;
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
        return (__other is ChipThemeData)
            && Equals(__other.color, color)
            && Equals(__other.backgroundColor, backgroundColor)
            && Equals(__other.deleteIconColor, deleteIconColor)
            && Equals(__other.disabledColor, disabledColor)
            && Equals(__other.selectedColor, selectedColor)
            && Equals(__other.secondarySelectedColor, secondarySelectedColor)
            && Equals(__other.shadowColor, shadowColor)
            && Equals(__other.surfaceTintColor, surfaceTintColor)
            && Equals(__other.selectedShadowColor, selectedShadowColor)
            && (__other.showCheckmark == showCheckmark)
            && Equals(__other.checkmarkColor, checkmarkColor)
            && Equals(__other.labelPadding, labelPadding)
            && Equals(__other.padding, padding)
            && Equals(__other.side, side)
            && Equals(__other.shape, shape)
            && Equals(__other.labelStyle, labelStyle)
            && Equals(__other.secondaryLabelStyle, secondaryLabelStyle)
            && Equals(__other.brightness, brightness)
            && (__other.elevation == elevation)
            && (__other.pressElevation == pressElevation)
            && Equals(__other.iconTheme, iconTheme)
            && Equals(__other.avatarBoxConstraints, avatarBoxConstraints)
            && Equals(__other.deleteIconBoxConstraints, deleteIconBoxConstraints);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>("color", color, defaultValue: null)
        );
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new ColorProperty("deleteIconColor", deleteIconColor, defaultValue: null));
        properties.add(new ColorProperty("disabledColor", disabledColor, defaultValue: null));
        properties.add(new ColorProperty("selectedColor", selectedColor, defaultValue: null));
        properties.add(
            new ColorProperty("secondarySelectedColor", secondarySelectedColor, defaultValue: null)
        );
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(
            new ColorProperty("selectedShadowColor", selectedShadowColor, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<bool>("showCheckmark", showCheckmark, defaultValue: null)
        );
        properties.add(new ColorProperty("checkMarkColor", checkmarkColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>(
                "labelPadding",
                labelPadding,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>("padding", padding, defaultValue: null)
        );
        properties.add(new DiagnosticsProperty<BorderSide>("side", side, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<TextStyle>("labelStyle", labelStyle, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "secondaryLabelStyle",
                secondaryLabelStyle,
                defaultValue: null
            )
        );
        properties.add(new EnumProperty<Brightness>("brightness", brightness, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new DoubleProperty("pressElevation", pressElevation, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<IconThemeData>("iconTheme", iconTheme, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<BoxConstraints>(
                "avatarBoxConstraints",
                avatarBoxConstraints,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<BoxConstraints>(
                "deleteIconBoxConstraints",
                deleteIconBoxConstraints,
                defaultValue: null
            )
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
