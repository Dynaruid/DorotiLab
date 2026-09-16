// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/card_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class CardTheme : global::Doroti.Framework.Widgets.InheritedWidget, global::Doroti.Framework.Foundation.Diagnosticable
{
    internal virtual CardThemeData? _data { get; private set; }
    internal virtual Clip? _clipBehavior { get; private set; }
    internal virtual Color? _color { get; private set; }
    internal virtual Color? _surfaceTintColor { get; private set; }
    internal virtual Color? _shadowColor { get; private set; }
    internal virtual double? _elevation { get; private set; }
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _margin { get; private set; }
    internal virtual global::Doroti.Framework.Painting.ShapeBorder? _shape { get; private set; }

    public CardTheme(global::Doroti.Framework.Foundation.Key? key = null, Clip? clipBehavior = null, Color? color = null, Color? surfaceTintColor = null, Color? shadowColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, CardThemeData? data = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key, child: child ?? new global::Doroti.Framework.Widgets.SizedBox())
    {
        _data = data;
        _clipBehavior = clipBehavior;
        _color = color;
        _surfaceTintColor = surfaceTintColor;
        _shadowColor = shadowColor;
        _elevation = elevation;
        _margin = margin;
        _shape = shape;
        System.Diagnostics.Debug.Assert((data is null) || ((((((((object?)clipBehavior ?? color) ?? surfaceTintColor) ?? shadowColor) ?? elevation) ?? margin) ?? shape) is null));
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public virtual global::Doroti.Ui.Clip? clipBehavior => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Clip>((_data is not null) ? _data.clipBehavior : _clipBehavior);
    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.color : _color);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.surfaceTintColor : _surfaceTintColor);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.shadowColor : _shadowColor);
    public virtual double? elevation => (_data is not null) ? _data.elevation : _elevation;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin => (_data is not null) ? _data.margin : _margin;
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape => (_data is not null) ? _data.shape : _shape;
    public virtual CardThemeData data
    {
        get
        {
            return _data ?? new CardThemeData(clipBehavior: _clipBehavior, color: _color, surfaceTintColor: _surfaceTintColor, shadowColor: _shadowColor, elevation: _elevation, margin: _margin, shape: _shape);
        }
    }
    public virtual CardTheme copyWith(Clip? clipBehavior = null, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null)
    {
        return new CardTheme(clipBehavior: clipBehavior ?? this.clipBehavior, color: color ?? this.color, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, elevation: elevation ?? this.elevation, margin: margin ?? this.margin, shape: shape ?? this.shape);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CardThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CardTheme? cardThemeLocal = context.dependOnInheritedWidgetOfExactType<CardTheme>();
        return cardThemeLocal?.data ?? Theme.of(context).cardTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((CardTheme)oldWidget).data));
    public static CardTheme lerp(CardTheme? a, CardTheme? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new CardTheme(clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior, color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), margin: EdgeInsetsGeometry.lerp(a?.margin, b?.margin, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double>("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("margin", margin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
    }

    public override string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
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

    public override DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CardThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Clip? clipBehavior { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }

    public CardThemeData(Clip? clipBehavior = null, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null)
    {
        this.clipBehavior = clipBehavior;
        this.color = color;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.elevation = elevation;
        this.margin = margin;
        this.shape = shape;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
    }

    public virtual CardThemeData copyWith(Clip? clipBehavior = null, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null)
    {
        return new CardThemeData(clipBehavior: clipBehavior ?? this.clipBehavior, color: color ?? this.color, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, elevation: elevation ?? this.elevation, margin: margin ?? this.margin, shape: shape ?? this.shape);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CardThemeData lerp(CardThemeData? a, CardThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new CardThemeData(clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior, color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), margin: EdgeInsetsGeometry.lerp(a?.margin, b?.margin, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(clipBehavior, color, shadowColor, surfaceTintColor, elevation, margin, shape));
    public override bool Equals(object? other)
    {
        var __other = other as CardThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is CardThemeData) && Equals(__other.clipBehavior, clipBehavior) && Equals(__other.color, color) && Equals(__other.shadowColor, shadowColor) && Equals(__other.surfaceTintColor, surfaceTintColor) && (__other.elevation == elevation) && Equals(__other.margin, margin) && Equals(__other.shape, shape);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double>("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("margin", margin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
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
