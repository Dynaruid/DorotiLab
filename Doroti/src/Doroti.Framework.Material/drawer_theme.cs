// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/drawer_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class DrawerThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? scrimColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual ShapeBorder? endShape { get; private set; }
    public virtual double? width { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }

    public DrawerThemeData(
        Color? backgroundColor = null,
        Color? scrimColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        ShapeBorder? endShape = null,
        double? width = null,
        Clip? clipBehavior = null
    )
    {
        this.backgroundColor = backgroundColor;
        this.scrimColor = scrimColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shape = shape;
        this.endShape = endShape;
        this.width = width;
        this.clipBehavior = clipBehavior;
    }

    public virtual DrawerThemeData copyWith(
        Color? backgroundColor = null,
        Color? scrimColor = null,
        double? elevation = null,
        Color? shadowColor = null,
        Color? surfaceTintColor = null,
        ShapeBorder? shape = null,
        ShapeBorder? endShape = null,
        double? width = null,
        Clip? clipBehavior = null
    )
    {
        return new DrawerThemeData(
            backgroundColor: backgroundColor ?? this.backgroundColor,
            scrimColor: scrimColor ?? this.scrimColor,
            elevation: elevation ?? this.elevation,
            shadowColor: shadowColor ?? this.shadowColor,
            surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor,
            shape: shape ?? this.shape,
            endShape: endShape ?? this.endShape,
            width: width ?? this.width,
            clipBehavior: clipBehavior ?? this.clipBehavior
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static DrawerThemeData? lerp(DrawerThemeData? a, DrawerThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new DrawerThemeData(
            backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t),
            scrimColor: Dart_uiLibrary.Color.lerp(a?.scrimColor, b?.scrimColor, t),
            elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t),
            shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t),
            surfaceTintColor: Dart_uiLibrary.Color.lerp(
                a?.surfaceTintColor,
                b?.surfaceTintColor,
                t
            ),
            shape: ShapeBorder.lerp(a?.shape, b?.shape, t),
            endShape: ShapeBorder.lerp(a?.endShape, b?.endShape, t),
            width: Dart_uiLibrary.lerpDouble(a?.width, b?.width, t),
            clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                backgroundColor,
                scrimColor,
                elevation,
                shadowColor,
                surfaceTintColor,
                shape,
                endShape,
                width,
                clipBehavior
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as DrawerThemeData;
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
        return (__other is DrawerThemeData)
            && Equals(__other.backgroundColor, backgroundColor)
            && Equals(__other.scrimColor, scrimColor)
            && (__other.elevation == elevation)
            && Equals(__other.shadowColor, shadowColor)
            && Equals(__other.surfaceTintColor, surfaceTintColor)
            && Equals(__other.shape, shape)
            && Equals(__other.endShape, endShape)
            && (__other.width == width)
            && Equals(__other.clipBehavior, clipBehavior);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new ColorProperty("scrimColor", scrimColor, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<ShapeBorder>("endShape", endShape, defaultValue: null)
        );
        properties.add(new DoubleProperty("width", width, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<Clip>("clipBehavior", clipBehavior, defaultValue: null)
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

public class DrawerTheme : InheritedTheme
{
    public virtual DrawerThemeData data { get; private set; } = default!;

    public DrawerTheme(Key? key = null, DrawerThemeData data = default!, Widget child = default!)
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DrawerThemeData of(BuildContext context)
    {
        DrawerTheme? drawerThemeLocal = context.dependOnInheritedWidgetOfExactType<DrawerTheme>();
        return drawerThemeLocal?.data ?? Theme.of(context).drawerTheme;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new DrawerTheme(data: data, child: child);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((DrawerTheme)oldWidget).data));
}
