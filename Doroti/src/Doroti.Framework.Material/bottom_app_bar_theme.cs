// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/bottom_app_bar_theme.dart
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

public class BottomAppBarTheme : global::Doroti.Framework.Widgets.InheritedTheme, global::Doroti.Framework.Foundation.Diagnosticable
{
    internal virtual BottomAppBarThemeData? _data { get; private set; }
    internal virtual Color? _color { get; private set; }
    internal virtual double? _elevation { get; private set; }
    internal virtual global::Doroti.Framework.Painting.NotchedShape? _shape { get; private set; }
    internal virtual double? _height { get; private set; }
    internal virtual Color? _surfaceTintColor { get; private set; }
    internal virtual Color? _shadowColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _padding { get; private set; }

    public BottomAppBarTheme(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, double? elevation = null, global::Doroti.Framework.Painting.NotchedShape? shape = null, double? height = null, Color? surfaceTintColor = null, Color? shadowColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null, BottomAppBarThemeData? data = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key, child: (child ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink()))
    {
        this._color = color;
        this._elevation = elevation;
        this._shape = shape;
        this._height = height;
        this._surfaceTintColor = surfaceTintColor;
        this._shadowColor = shadowColor;
        this._padding = padding;
        this._data = data;
        System.Diagnostics.Debug.Assert(((data is null) || (((((object?)(((object?)(((object?)(((object?)(((object?)(((object?)color ?? (object?)elevation)) ?? (object?)shape)) ?? (object?)height)) ?? (object?)surfaceTintColor)) ?? (object?)shadowColor)) ?? (object?)padding))) is null)));
    }

    public virtual global::Doroti.Ui.Color? color => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((BottomAppBarThemeData)this._data).color : this._color));
    public virtual double? elevation => ((this._data is not null) ? ((BottomAppBarThemeData)this._data).elevation : this._elevation);
    public virtual global::Doroti.Framework.Painting.NotchedShape? shape => ((this._data is not null) ? ((BottomAppBarThemeData)this._data).shape : this._shape);
    public virtual double? height => ((this._data is not null) ? ((BottomAppBarThemeData)this._data).height : this._height);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((BottomAppBarThemeData)this._data).surfaceTintColor : this._surfaceTintColor));
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((BottomAppBarThemeData)this._data).shadowColor : this._shadowColor));
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding => ((this._data is not null) ? ((BottomAppBarThemeData)this._data).padding : this._padding);
    public virtual BottomAppBarThemeData data => DartRuntimePrimitives.ConvertValue<BottomAppBarThemeData>((this._data ?? new BottomAppBarThemeData(color: this._color, elevation: this._elevation, shape: this._shape, height: this._height, surfaceTintColor: this._surfaceTintColor, shadowColor: this._shadowColor, padding: this._padding)));
    public virtual BottomAppBarTheme copyWith(Color? color = null, double? elevation = null, global::Doroti.Framework.Painting.NotchedShape? shape = null, double? height = null, Color? surfaceTintColor = null, Color? shadowColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null)
    {
        return new BottomAppBarTheme(color: (color ?? this.color), elevation: (elevation ?? this.elevation), shape: (shape ?? this.shape), height: (height ?? this.height), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shadowColor: (shadowColor ?? this.shadowColor), padding: (padding ?? this.padding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomAppBarThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        BottomAppBarTheme? bottomAppBarTheme__5895 = ((BottomAppBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<BottomAppBarTheme>());
        return (bottomAppBarTheme__5895?.data ?? Theme.of(context).bottomAppBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomAppBarTheme lerp(BottomAppBarTheme? a, BottomAppBarTheme? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new BottomAppBarTheme(color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shape: ((t < 0.5) ? a?.shape : b?.shape), height: Dart_uiLibrary.lerpDouble(a?.height, b?.height, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((BottomAppBarTheme)oldWidget).data)));
    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new BottomAppBarTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
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

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
    }

}

public class BottomAppBarThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.NotchedShape? shape { get; private set; }
    public virtual double? height { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding { get; private set; }

    public BottomAppBarThemeData(Color? color = null, double? elevation = null, global::Doroti.Framework.Painting.NotchedShape? shape = null, double? height = null, Color? surfaceTintColor = null, Color? shadowColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null)
    {
        this.color = color;
        this.elevation = elevation;
        this.shape = shape;
        this.height = height;
        this.surfaceTintColor = surfaceTintColor;
        this.shadowColor = shadowColor;
        this.padding = padding;
    }

    public virtual BottomAppBarThemeData copyWith(Color? color = null, double? elevation = null, global::Doroti.Framework.Painting.NotchedShape? shape = null, double? height = null, Color? surfaceTintColor = null, Color? shadowColor = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? padding = null)
    {
        return new BottomAppBarThemeData(color: (color ?? this.color), elevation: (elevation ?? this.elevation), shape: (shape ?? this.shape), height: (height ?? this.height), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shadowColor: (shadowColor ?? this.shadowColor), padding: (padding ?? this.padding));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static BottomAppBarThemeData lerp(BottomAppBarThemeData? a, BottomAppBarThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new BottomAppBarThemeData(color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shape: ((t < 0.5) ? a?.shape : b?.shape), height: Dart_uiLibrary.lerpDouble(a?.height, b?.height, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), padding: EdgeInsetsGeometry.lerp(a?.padding, b?.padding, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.color, this.elevation, this.shape, this.height, this.surfaceTintColor, this.shadowColor, this.padding));
    public override bool Equals(object? other)
    {
        var __other = other as BottomAppBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((__other is BottomAppBarThemeData) && (object.Equals(((BottomAppBarThemeData)((BottomAppBarThemeData)__other)).color, this.color))) && (((BottomAppBarThemeData)((BottomAppBarThemeData)__other)).elevation == this.elevation)) && (object.Equals(((BottomAppBarThemeData)((BottomAppBarThemeData)__other)).shape, this.shape))) && (((BottomAppBarThemeData)((BottomAppBarThemeData)__other)).height == this.height)) && (object.Equals(((BottomAppBarThemeData)((BottomAppBarThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((BottomAppBarThemeData)((BottomAppBarThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((BottomAppBarThemeData)((BottomAppBarThemeData)__other)).padding, this.padding)));
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.NotchedShape?>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("height", this.height, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry?>("padding", this.padding, defaultValue: null));
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
