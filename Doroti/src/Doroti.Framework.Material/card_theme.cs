// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/card_theme.dart
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

    public CardTheme(global::Doroti.Framework.Foundation.Key? key = null, Clip? clipBehavior = null, Color? color = null, Color? surfaceTintColor = null, Color? shadowColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, CardThemeData? data = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key, child: (child ?? new global::Doroti.Framework.Widgets.SizedBox()))
    {
        this._data = data;
        this._clipBehavior = clipBehavior;
        this._color = color;
        this._surfaceTintColor = surfaceTintColor;
        this._shadowColor = shadowColor;
        this._elevation = elevation;
        this._margin = margin;
        this._shape = shape;
        System.Diagnostics.Debug.Assert(((data is null) || (((((object?)(((object?)(((object?)(((object?)(((object?)(((object?)clipBehavior ?? (object?)color)) ?? (object?)surfaceTintColor)) ?? (object?)shadowColor)) ?? (object?)elevation)) ?? (object?)margin)) ?? (object?)shape))) is null)));
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public virtual global::Doroti.Ui.Clip? clipBehavior => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Clip>(((this._data is not null) ? ((CardThemeData)this._data).clipBehavior : this._clipBehavior));
    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((CardThemeData)this._data).color : this._color));
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((CardThemeData)this._data).surfaceTintColor : this._surfaceTintColor));
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((CardThemeData)this._data).shadowColor : this._shadowColor));
    public virtual double? elevation => ((this._data is not null) ? ((CardThemeData)this._data).elevation : this._elevation);
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin => ((this._data is not null) ? ((CardThemeData)this._data).margin : this._margin);
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape => ((this._data is not null) ? ((CardThemeData)this._data).shape : this._shape);
    public virtual CardThemeData data
    {
        get
        {
            return (this._data ?? new CardThemeData(clipBehavior: this._clipBehavior, color: this._color, surfaceTintColor: this._surfaceTintColor, shadowColor: this._shadowColor, elevation: this._elevation, margin: this._margin, shape: this._shape));
            return default!;
        }
    }
    public virtual CardTheme copyWith(Clip? clipBehavior = null, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null)
    {
        return new CardTheme(clipBehavior: (clipBehavior ?? this.clipBehavior), color: (color ?? this.color), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), elevation: (elevation ?? this.elevation), margin: (margin ?? this.margin), shape: (shape ?? this.shape));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CardThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CardTheme? cardTheme__5957 = ((CardTheme?)(object?)context.dependOnInheritedWidgetOfExactType<CardTheme>());
        return (cardTheme__5957?.data ?? Theme.of(context).cardTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((CardTheme)oldWidget).data)));
    public static CardTheme lerp(CardTheme? a, CardTheme? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new CardTheme(clipBehavior: ((t < 0.5) ? a?.clipBehavior : b?.clipBehavior), color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), margin: EdgeInsetsGeometry.lerp(a?.margin, b?.margin, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double>("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("margin", this.margin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
    }

    public override string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
    }

    public virtual CardThemeData copyWith(Clip? clipBehavior = null, Color? color = null, Color? shadowColor = null, Color? surfaceTintColor = null, double? elevation = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? margin = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null)
    {
        return new CardThemeData(clipBehavior: (clipBehavior ?? this.clipBehavior), color: (color ?? this.color), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), elevation: (elevation ?? this.elevation), margin: (margin ?? this.margin), shape: (shape ?? this.shape));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static CardThemeData lerp(CardThemeData? a, CardThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new CardThemeData(clipBehavior: ((t < 0.5) ? a?.clipBehavior : b?.clipBehavior), color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), margin: EdgeInsetsGeometry.lerp(a?.margin, b?.margin, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.clipBehavior, this.color, this.shadowColor, this.surfaceTintColor, this.elevation, this.margin, this.shape));
    public override bool Equals(object? other)
    {
        var __other = other as CardThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((__other is CardThemeData) && (object.Equals(((CardThemeData)((CardThemeData)__other)).clipBehavior, this.clipBehavior))) && (object.Equals(((CardThemeData)((CardThemeData)__other)).color, this.color))) && (object.Equals(((CardThemeData)((CardThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((CardThemeData)((CardThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (((CardThemeData)((CardThemeData)__other)).elevation == this.elevation)) && (object.Equals(((CardThemeData)((CardThemeData)__other)).margin, this.margin))) && (object.Equals(((CardThemeData)((CardThemeData)__other)).shape, this.shape)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<double>("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("margin", this.margin, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
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
