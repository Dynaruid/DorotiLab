// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/carousel_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class CarouselViewThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape { get; private set; }
    public virtual Clip? itemClipBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }

    public CarouselViewThemeData(double? elevation = null, Color? backgroundColor = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? padding = null, Clip? itemClipBehavior = null)
    {
        this.elevation = elevation;
        this.backgroundColor = backgroundColor;
        this.overlayColor = overlayColor;
        this.shape = shape;
        this.padding = padding;
        this.itemClipBehavior = itemClipBehavior;
    }

    public virtual CarouselViewThemeData copyWith(Color? backgroundColor = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.OutlinedBorder? shape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? padding = null, Clip? itemClipBehavior = null)
    {
        return new CarouselViewThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), elevation: (elevation ?? this.elevation), shape: (shape ?? this.shape), overlayColor: (overlayColor ?? this.overlayColor), padding: (padding ?? this.padding), itemClipBehavior: (itemClipBehavior ?? this.itemClipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CarouselViewThemeData lerp(CarouselViewThemeData? a, CarouselViewThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new CarouselViewThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shape: ((global::Doroti.Generated.Framework.Painting.OutlinedBorder?)(object?)ShapeBorder.lerp(a?.shape, b?.shape, t))!, overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), padding: EdgeInsets.lerp(a?.padding, b?.padding, t), itemClipBehavior: ((t < 0.5) ? a?.itemClipBehavior : b?.itemClipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.elevation, this.shape, this.overlayColor, this.padding, this.itemClipBehavior));
    public override bool Equals(object? other)
    {
        var __other = other as CarouselViewThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((__other is CarouselViewThemeData) && (object.Equals(((CarouselViewThemeData)((CarouselViewThemeData)__other)).backgroundColor, this.backgroundColor))) && (((CarouselViewThemeData)((CarouselViewThemeData)__other)).elevation == this.elevation)) && (object.Equals(((CarouselViewThemeData)((CarouselViewThemeData)__other)).shape, this.shape))) && (object.Equals(((CarouselViewThemeData)((CarouselViewThemeData)__other)).overlayColor, this.overlayColor))) && (object.Equals(((CarouselViewThemeData)((CarouselViewThemeData)__other)).padding, this.padding))) && (object.Equals(((CarouselViewThemeData)((CarouselViewThemeData)__other)).itemClipBehavior, this.itemClipBehavior)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.OutlinedBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("overlayColor", this.overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsets>("padding", this.padding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.Clip>("itemClipBehavior", this.itemClipBehavior, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public class CarouselViewTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual CarouselViewThemeData data { get; private set; } = default!;

    public CarouselViewTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, CarouselViewThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static CarouselViewThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CarouselViewTheme? inheritedTheme__6808 = ((CarouselViewTheme?)(object?)context.dependOnInheritedWidgetOfExactType<CarouselViewTheme>());
        return (inheritedTheme__6808?.data ?? Theme.of(context).carouselViewTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CarouselViewTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((CarouselViewTheme)oldWidget).data)));
}
