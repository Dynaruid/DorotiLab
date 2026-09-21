// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class NavigationBarThemeData : Diagnosticable
{
    public virtual double? height { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual ShapeBorder? indicatorShape { get; private set; }
    public virtual WidgetStateProperty<TextStyle?>? labelTextStyle { get; private set; }
    public virtual WidgetStateProperty<IconThemeData?>? iconTheme { get; private set; }
    public virtual NavigationDestinationLabelBehavior? labelBehavior { get; private set; }
    public virtual WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual EdgeInsetsGeometry? labelPadding { get; private set; }

    public NavigationBarThemeData(
        double? height = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        Color? indicatorColor = null,
        ShapeBorder? indicatorShape = null,
        WidgetStateProperty<TextStyle?>? labelTextStyle = null,
        WidgetStateProperty<IconThemeData?>? iconTheme = null,
        NavigationDestinationLabelBehavior? labelBehavior = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        EdgeInsetsGeometry? labelPadding = null
    )
    {
        this.height = height;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.labelTextStyle = labelTextStyle;
        this.iconTheme = iconTheme;
        this.labelBehavior = labelBehavior;
        this.overlayColor = overlayColor;
        this.labelPadding = labelPadding;
    }

    public virtual NavigationBarThemeData copyWith(
        double? height = null,
        Color? backgroundColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        Color? indicatorColor = null,
        ShapeBorder? indicatorShape = null,
        WidgetStateProperty<TextStyle?>? labelTextStyle = null,
        WidgetStateProperty<IconThemeData?>? iconTheme = null,
        NavigationDestinationLabelBehavior? labelBehavior = null,
        WidgetStateProperty<Color?>? overlayColor = null,
        EdgeInsetsGeometry? labelPadding = null
    )
    {
        return new NavigationBarThemeData(
            height: height ?? this.height,
            backgroundColor: backgroundColor ?? this.backgroundColor,
            elevation: elevation ?? this.elevation,
            shadowColor: shadowColor ?? this.shadowColor,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            indicatorColor: indicatorColor ?? this.indicatorColor,
            indicatorShape: indicatorShape ?? this.indicatorShape,
            labelTextStyle: labelTextStyle ?? this.labelTextStyle,
            iconTheme: iconTheme ?? this.iconTheme,
            labelBehavior: labelBehavior ?? this.labelBehavior,
            overlayColor: overlayColor ?? this.overlayColor,
            labelPadding: labelPadding ?? this.labelPadding
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static NavigationBarThemeData? lerp(
        NavigationBarThemeData? a,
        NavigationBarThemeData? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new NavigationBarThemeData(
            height: DorotiUiLibrary.lerpDouble(a?.height, b?.height, t),
            backgroundColor: DorotiUiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t),
            elevation: DorotiUiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            shadowColor: DorotiUiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            surfaceTintColor: DorotiUiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            indicatorColor: DorotiUiLibrary.Color.lerp(a?.indicatorColor, b?.indicatorColor, t),
            indicatorShape: ShapeBorder.lerp(a?.indicatorShape, b?.indicatorShape, t),
            labelTextStyle: WidgetStateProperty.lerp(
                a?.labelTextStyle,
                b?.labelTextStyle,
                t,
                TextStyle.lerp
            ),
            iconTheme: WidgetStateProperty.lerp(a?.iconTheme, b?.iconTheme, t, IconThemeData.lerp),
            labelBehavior: (t < 0.5) ? a?.labelBehavior : b?.labelBehavior,
            overlayColor: WidgetStateProperty.lerp(a?.overlayColor, b?.overlayColor, t, Color.lerp),
            labelPadding: EdgeInsetsGeometry.lerp(a?.labelPadding, b?.labelPadding, t)
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                height,
                backgroundColor,
                elevation,
                shadowColor,
                surfaceTintColor,
                indicatorColor,
                indicatorShape,
                labelTextStyle,
                iconTheme,
                labelBehavior,
                overlayColor,
                labelPadding
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as NavigationBarThemeData;
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
        return (__other is NavigationBarThemeData)
            && (__other.height == height)
            && Equals(__other.backgroundColor, backgroundColor)
            && (__other.elevation == elevation)
            && Equals(__other.shadowColor, shadowColor)
            && Equals(__other.surfaceTintColor, surfaceTintColor)
            && Equals(__other.indicatorColor, indicatorColor)
            && Equals(__other.indicatorShape, indicatorShape)
            && Equals(__other.labelTextStyle, labelTextStyle)
            && Equals(__other.iconTheme, iconTheme)
            && Equals(__other.labelBehavior, labelBehavior)
            && Equals(__other.overlayColor, overlayColor)
            && Equals(__other.labelPadding, labelPadding);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DoubleProperty("height", height, defaultValue: null));
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new ColorProperty("indicatorColor", indicatorColor, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<ShapeBorder>(
                "indicatorShape",
                indicatorShape,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<TextStyle?>>(
                "labelTextStyle",
                labelTextStyle,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<IconThemeData?>>(
                "iconTheme",
                iconTheme,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<NavigationDestinationLabelBehavior>(
                "labelBehavior",
                labelBehavior,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<WidgetStateProperty<Color?>>(
                "overlayColor",
                overlayColor,
                defaultValue: null
            )
        );
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>(
                "labelPadding",
                labelPadding,
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

public class NavigationBarTheme : InheritedTheme
{
    public virtual NavigationBarThemeData data { get; private set; } = default!;

    public NavigationBarTheme(
        Key? key = null,
        NavigationBarThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static NavigationBarThemeData of(BuildContext context)
    {
        NavigationBarTheme? navigationBarThemeLocal =
            context.dependOnInheritedWidgetOfExactType<NavigationBarTheme>();
        return navigationBarThemeLocal?.data ?? Theme.of(context).navigationBarTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new NavigationBarTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(data, ((NavigationBarTheme)oldWidget).data)
        );
}
