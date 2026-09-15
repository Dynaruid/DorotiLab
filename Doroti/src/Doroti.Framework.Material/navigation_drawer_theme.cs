// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/navigation_drawer_theme.dart
#pragma warning disable CS8600, CS8603
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class NavigationDrawerThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual double? tileHeight { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? indicatorColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? indicatorShape { get; private set; }
    public virtual Size? indicatorSize { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>? iconTheme { get; private set; }

    public NavigationDrawerThemeData(double? tileHeight = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, Size? indicatorSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>? iconTheme = null)
    {
        this.tileHeight = tileHeight;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.indicatorColor = indicatorColor;
        this.indicatorShape = indicatorShape;
        this.indicatorSize = indicatorSize;
        this.labelTextStyle = labelTextStyle;
        this.iconTheme = iconTheme;
    }

    public virtual NavigationDrawerThemeData copyWith(double? tileHeight = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, Color? indicatorColor = null, global::Doroti.Framework.Painting.ShapeBorder? indicatorShape = null, Size? indicatorSize = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>? labelTextStyle = null, global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>? iconTheme = null)
    {
        return new NavigationDrawerThemeData(tileHeight: (tileHeight ?? this.tileHeight), backgroundColor: (backgroundColor ?? this.backgroundColor), elevation: (elevation ?? this.elevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), indicatorColor: (indicatorColor ?? this.indicatorColor), indicatorShape: (indicatorShape ?? this.indicatorShape), indicatorSize: (indicatorSize ?? this.indicatorSize), labelTextStyle: (labelTextStyle ?? this.labelTextStyle), iconTheme: (iconTheme ?? this.iconTheme));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static NavigationDrawerThemeData? lerp(NavigationDrawerThemeData? a, NavigationDrawerThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new NavigationDrawerThemeData(tileHeight: Dart_uiLibrary.lerpDouble(a?.tileHeight, b?.tileHeight, t), backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), indicatorColor: Dart_uiLibrary.Color.lerp(a?.indicatorColor, b?.indicatorColor, t), indicatorShape: ShapeBorder.lerp(a?.indicatorShape, b?.indicatorShape, t), indicatorSize: Dart_uiLibrary.Size.lerp(a?.indicatorSize, a?.indicatorSize, t), labelTextStyle: WidgetStateProperty.lerp<global::Doroti.Framework.Painting.TextStyle?>(a?.labelTextStyle, b?.labelTextStyle, t, (global::System.Func<global::Doroti.Framework.Painting.TextStyle?, global::Doroti.Framework.Painting.TextStyle?, double, global::Doroti.Framework.Painting.TextStyle?>)global::Doroti.Framework.Painting.TextStyle.lerp), iconTheme: WidgetStateProperty.lerp<global::Doroti.Framework.Widgets.IconThemeData?>(a?.iconTheme, b?.iconTheme, t, (global::System.Func<global::Doroti.Framework.Widgets.IconThemeData?, global::Doroti.Framework.Widgets.IconThemeData?, double, global::Doroti.Framework.Widgets.IconThemeData>)global::Doroti.Framework.Widgets.IconThemeData.lerp));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.tileHeight, this.backgroundColor, this.elevation, this.shadowColor, this.surfaceTintColor, this.indicatorColor, this.indicatorShape, this.indicatorSize, this.labelTextStyle, this.iconTheme));
    public override bool Equals(object? other)
    {
        var __other = other as NavigationDrawerThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((__other is NavigationDrawerThemeData) && (((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).tileHeight == this.tileHeight)) && (object.Equals(((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).backgroundColor, this.backgroundColor))) && (((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).elevation == this.elevation)) && (object.Equals(((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).indicatorColor, this.indicatorColor))) && (object.Equals(((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).indicatorShape, this.indicatorShape))) && (object.Equals(((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).indicatorSize, this.indicatorSize))) && (object.Equals(((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).labelTextStyle, this.labelTextStyle))) && (object.Equals(((NavigationDrawerThemeData)((NavigationDrawerThemeData)__other)).iconTheme, this.iconTheme)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("tileHeight", this.tileHeight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("indicatorColor", this.indicatorColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("indicatorShape", this.indicatorShape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Size>("indicatorSize", this.indicatorSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Painting.TextStyle?>>("labelTextStyle", this.labelTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Framework.Widgets.IconThemeData?>>("iconTheme", this.iconTheme, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(global::Doroti.Framework.Foundation.DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
            });
        return ((fullString ?? (string)toStringShort()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return ((DiagnosticsNode)(object?)new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class NavigationDrawerTheme : global::Doroti.Framework.Widgets.InheritedTheme
{
    public virtual NavigationDrawerThemeData data { get; private set; } = default!;

    public NavigationDrawerTheme(global::Doroti.Framework.Foundation.Key? key = null, NavigationDrawerThemeData data = default!, global::Doroti.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static NavigationDrawerThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        NavigationDrawerTheme? navigationDrawerThemeLocal = ((NavigationDrawerTheme?)(object?)context.dependOnInheritedWidgetOfExactType<NavigationDrawerTheme>());
        return (navigationDrawerThemeLocal?.data ?? Theme.of(context).navigationDrawerTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new NavigationDrawerTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((NavigationDrawerTheme)oldWidget).data)));
}
