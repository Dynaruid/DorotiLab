// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/navigation_bar_theme.dart
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

public class NavigationBarThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual double? height { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData?>? iconTheme { get; private set; }
    public virtual NavigationDestinationLabelBehavior? labelBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding { get; private set; }

    public NavigationBarThemeData(double? height = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? indicatorColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? indicatorShape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData?>? iconTheme = null, NavigationDestinationLabelBehavior? labelBehavior = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null)
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

    public virtual NavigationBarThemeData copyWith(double? height = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? indicatorColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? indicatorShape = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>? labelTextStyle = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData?>? iconTheme = null, NavigationDestinationLabelBehavior? labelBehavior = null, global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? labelPadding = null)
    {
        return new NavigationBarThemeData(height: (height ?? this.height), backgroundColor: (backgroundColor ?? this.backgroundColor), elevation: (elevation ?? this.elevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), indicatorColor: (indicatorColor ?? this.indicatorColor), indicatorShape: (indicatorShape ?? this.indicatorShape), labelTextStyle: (labelTextStyle ?? this.labelTextStyle), iconTheme: (iconTheme ?? this.iconTheme), labelBehavior: (labelBehavior ?? this.labelBehavior), overlayColor: (overlayColor ?? this.overlayColor), labelPadding: (labelPadding ?? this.labelPadding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static NavigationBarThemeData? lerp(NavigationBarThemeData? a, NavigationBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new NavigationBarThemeData(height: Dart_uiLibrary.lerpDouble(a?.height, b?.height, t), backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), indicatorColor: Dart_uiLibrary.Color.lerp(a?.indicatorColor, b?.indicatorColor, t), indicatorShape: ShapeBorder.lerp(a?.indicatorShape, b?.indicatorShape, t), labelTextStyle: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Painting.TextStyle?>(a?.labelTextStyle, b?.labelTextStyle, t, (global::System.Func<global::Doroti.Generated.Framework.Painting.TextStyle?, global::Doroti.Generated.Framework.Painting.TextStyle?, double, global::Doroti.Generated.Framework.Painting.TextStyle?>)global::Doroti.Generated.Framework.Painting.TextStyle.lerp), iconTheme: WidgetStateProperty.lerp<global::Doroti.Generated.Framework.Widgets.IconThemeData?>(a?.iconTheme, b?.iconTheme, t, (global::System.Func<global::Doroti.Generated.Framework.Widgets.IconThemeData?, global::Doroti.Generated.Framework.Widgets.IconThemeData?, double, global::Doroti.Generated.Framework.Widgets.IconThemeData>)global::Doroti.Generated.Framework.Widgets.IconThemeData.lerp), labelBehavior: ((t < 0.5) ? a?.labelBehavior : b?.labelBehavior), overlayColor: WidgetStateProperty.lerp<global::Doroti.Ui.Color?>(a?.overlayColor, b?.overlayColor, t, (global::System.Func<Color?, Color?, double, Color?>)Color.lerp), labelPadding: EdgeInsetsGeometry.lerp(a?.labelPadding, b?.labelPadding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.height, this.backgroundColor, this.elevation, this.shadowColor, this.surfaceTintColor, this.indicatorColor, this.indicatorShape, this.labelTextStyle, this.iconTheme, this.labelBehavior, this.overlayColor, this.labelPadding));
    public override bool Equals(object? other)
    {
        var __other = other as NavigationBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((__other is NavigationBarThemeData) && (((NavigationBarThemeData)((NavigationBarThemeData)__other)).height == this.height)) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).backgroundColor, this.backgroundColor))) && (((NavigationBarThemeData)((NavigationBarThemeData)__other)).elevation == this.elevation)) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).indicatorColor, this.indicatorColor))) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).indicatorShape, this.indicatorShape))) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).labelTextStyle, this.labelTextStyle))) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).iconTheme, this.iconTheme))) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).labelBehavior, this.labelBehavior))) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).overlayColor, this.overlayColor))) && (object.Equals(((NavigationBarThemeData)((NavigationBarThemeData)__other)).labelPadding, this.labelPadding)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("indicatorColor", this.indicatorColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("indicatorShape", this.indicatorShape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Painting.TextStyle?>>("labelTextStyle", this.labelTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Generated.Framework.Widgets.IconThemeData?>>("iconTheme", this.iconTheme, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<NavigationDestinationLabelBehavior>("labelBehavior", this.labelBehavior, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>>("overlayColor", this.overlayColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("labelPadding", this.labelPadding, defaultValue: null));
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

public class NavigationBarTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual NavigationBarThemeData data { get; private set; } = default!;

    public NavigationBarTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, NavigationBarThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static NavigationBarThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        NavigationBarTheme? navigationBarTheme__10042 = ((NavigationBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<NavigationBarTheme>());
        return (navigationBarTheme__10042?.data ?? Theme.of(context).navigationBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new NavigationBarTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((NavigationBarTheme)oldWidget).data)));
}
