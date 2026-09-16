// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/carousel_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class CarouselViewThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Clip? itemClipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }

    public CarouselViewThemeData(double? elevation = null, Color? backgroundColor = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Clip? itemClipBehavior = null)
    {
        this.elevation = elevation;
        this.backgroundColor = backgroundColor;
        this.overlayColor = overlayColor;
        this.shape = shape;
        this.padding = padding;
        this.itemClipBehavior = itemClipBehavior;
    }

    public virtual CarouselViewThemeData copyWith(Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Clip? itemClipBehavior = null)
    {
        return new CarouselViewThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, elevation: elevation ?? this.elevation, shape: shape ?? this.shape, overlayColor: overlayColor ?? this.overlayColor, padding: padding ?? this.padding, itemClipBehavior: itemClipBehavior ?? this.itemClipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CarouselViewThemeData lerp(CarouselViewThemeData? a, CarouselViewThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new CarouselViewThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shape: ((global::Doroti.Framework.Painting.OutlinedBorder?)ShapeBorder.lerp(a?.shape, b?.shape, t))!, overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, Color.lerp), padding: EdgeInsets.lerp(a?.padding, b?.padding, t), itemClipBehavior: (t < 0.5) ? a?.itemClipBehavior : b?.itemClipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(backgroundColor, elevation, shape, overlayColor, padding, itemClipBehavior));
    public override bool Equals(object? other)
    {
        var __other = other as CarouselViewThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is CarouselViewThemeData) && Equals(__other.backgroundColor, backgroundColor) && (__other.elevation == elevation) && Equals(__other.shape, shape) && Equals(__other.overlayColor, overlayColor) && Equals(__other.padding, padding) && Equals(__other.itemClipBehavior, itemClipBehavior);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.OutlinedBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("overlayColor", overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsets>("padding", padding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("itemClipBehavior", itemClipBehavior, defaultValue: null));
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CarouselViewTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual CarouselViewThemeData data { get; private set; } = default!;

    public CarouselViewTheme(global::Doroti.Framework.Foundation.Key? key = null, CarouselViewThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static CarouselViewThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CarouselViewTheme? inheritedTheme = context.dependOnInheritedWidgetOfExactType<CarouselViewTheme>();
        return inheritedTheme?.data ?? Theme.of(context).carouselViewTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new CarouselViewTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((CarouselViewTheme)oldWidget).data));
}
