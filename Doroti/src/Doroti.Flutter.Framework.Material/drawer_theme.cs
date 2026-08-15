// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/drawer_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class DrawerThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? scrimColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? endShape { get; private set; }
    public virtual double? width { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }

    public DrawerThemeData(Color? backgroundColor = null, Color? scrimColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? endShape = null, double? width = null, Clip? clipBehavior = null)
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

    public virtual DrawerThemeData copyWith(Color? backgroundColor = null, Color? scrimColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? endShape = null, double? width = null, Clip? clipBehavior = null)
    {
        return new DrawerThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), scrimColor: (scrimColor ?? this.scrimColor), elevation: (elevation ?? this.elevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shape: (shape ?? this.shape), endShape: (endShape ?? this.endShape), width: (width ?? this.width), clipBehavior: (clipBehavior ?? this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DrawerThemeData? lerp(DrawerThemeData? a, DrawerThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new DrawerThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), scrimColor: Dart_uiLibrary.Color.lerp(a?.scrimColor, b?.scrimColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), endShape: ShapeBorder.lerp(a?.endShape, b?.endShape, t), width: Dart_uiLibrary.lerpDouble(a?.width, b?.width, t), clipBehavior: ((t < 0.5) ? a?.clipBehavior : b?.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.scrimColor, this.elevation, this.shadowColor, this.surfaceTintColor, this.shape, this.endShape, this.width, this.clipBehavior));
    public override bool Equals(object? other)
    {
        var __other = other as DrawerThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((__other is DrawerThemeData) && (object.Equals(((DrawerThemeData)((DrawerThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((DrawerThemeData)((DrawerThemeData)__other)).scrimColor, this.scrimColor))) && (((DrawerThemeData)((DrawerThemeData)__other)).elevation == this.elevation)) && (object.Equals(((DrawerThemeData)((DrawerThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((DrawerThemeData)((DrawerThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((DrawerThemeData)((DrawerThemeData)__other)).shape, this.shape))) && (object.Equals(((DrawerThemeData)((DrawerThemeData)__other)).endShape, this.endShape))) && (((DrawerThemeData)((DrawerThemeData)__other)).width == this.width)) && (object.Equals(((DrawerThemeData)((DrawerThemeData)__other)).clipBehavior, this.clipBehavior)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("scrimColor", this.scrimColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("endShape", this.endShape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Flutter.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
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

public class DrawerTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual DrawerThemeData data { get; private set; } = default!;

    public DrawerTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, DrawerThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static DrawerThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DrawerTheme? drawerTheme__6803 = ((DrawerTheme?)(object?)context.dependOnInheritedWidgetOfExactType<DrawerTheme>());
        return (drawerTheme__6803?.data ?? Theme.of(context).drawerTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new DrawerTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((DrawerTheme)oldWidget).data)));
}
