// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/dialog_theme.dart
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

public class DialogTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme, global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    internal virtual DialogThemeData? _data { get; private set; }
    internal virtual Color? _backgroundColor { get; private set; }
    internal virtual double? _elevation { get; private set; }
    internal virtual Color? _shadowColor { get; private set; }
    internal virtual Color? _surfaceTintColor { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? _shape { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? _alignment { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle? _titleTextStyle { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.TextStyle? _contentTextStyle { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? _actionsPadding { get; private set; }
    internal virtual Color? _iconColor { get; private set; }
    internal virtual Color? _barrierColor { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? _insetPadding { get; private set; }
    internal virtual Clip? _clipBehavior { get; private set; }

    public DialogTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, Color? iconColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, Color? barrierColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, DialogThemeData? data = null, global::Doroti.Generated.Framework.Widgets.Widget? child = null) : base(key: key, child: (child ?? new global::Doroti.Generated.Framework.Widgets.SizedBox()))
    {
        this._data = data;
        this._backgroundColor = backgroundColor;
        this._elevation = elevation;
        this._shadowColor = shadowColor;
        this._surfaceTintColor = surfaceTintColor;
        this._shape = shape;
        this._alignment = alignment;
        this._iconColor = iconColor;
        this._titleTextStyle = titleTextStyle;
        this._contentTextStyle = contentTextStyle;
        this._actionsPadding = actionsPadding;
        this._barrierColor = barrierColor;
        this._insetPadding = insetPadding;
        this._clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert(((data is null) || (((((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)(((object?)backgroundColor ?? (object?)elevation)) ?? (object?)shadowColor)) ?? (object?)surfaceTintColor)) ?? (object?)shape)) ?? (object?)alignment)) ?? (object?)iconColor)) ?? (object?)titleTextStyle)) ?? (object?)contentTextStyle)) ?? (object?)actionsPadding)) ?? (object?)barrierColor)) ?? (object?)insetPadding)) ?? (object?)clipBehavior))) is null)));
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((DialogThemeData)this._data).backgroundColor : this._backgroundColor));
    public virtual double? elevation => ((this._data is not null) ? ((DialogThemeData)this._data).elevation : this._elevation);
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((DialogThemeData)this._data).shadowColor : this._shadowColor));
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((DialogThemeData)this._data).surfaceTintColor : this._surfaceTintColor));
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape => ((this._data is not null) ? ((DialogThemeData)this._data).shape : this._shape);
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment => ((this._data is not null) ? ((DialogThemeData)this._data).alignment : this._alignment);
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle => ((this._data is not null) ? ((DialogThemeData)this._data).titleTextStyle : this._titleTextStyle);
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle => ((this._data is not null) ? ((DialogThemeData)this._data).contentTextStyle : this._contentTextStyle);
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding => ((this._data is not null) ? ((DialogThemeData)this._data).actionsPadding : this._actionsPadding);
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((DialogThemeData)this._data).iconColor : this._iconColor));
    public virtual global::Doroti.Ui.Color? barrierColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._data is not null) ? ((DialogThemeData)this._data).barrierColor : this._barrierColor));
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding => ((this._data is not null) ? ((DialogThemeData)this._data).insetPadding : this._insetPadding);
    public virtual global::Doroti.Ui.Clip? clipBehavior => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Clip>(((this._data is not null) ? ((DialogThemeData)this._data).clipBehavior : this._clipBehavior));
    public virtual DialogThemeData data
    {
        get
        {
            return (this._data ?? new DialogThemeData(backgroundColor: this._backgroundColor, elevation: this._elevation, shadowColor: this._shadowColor, surfaceTintColor: this._surfaceTintColor, shape: this._shape, alignment: this._alignment, iconColor: this._iconColor, titleTextStyle: this._titleTextStyle, contentTextStyle: this._contentTextStyle, actionsPadding: this._actionsPadding, barrierColor: this._barrierColor, insetPadding: this._insetPadding, clipBehavior: this._clipBehavior));
            return default!;
        }
    }
    public static DialogThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DialogTheme? dialogTheme__8240 = ((DialogTheme?)(object?)context.dependOnInheritedWidgetOfExactType<DialogTheme>());
        return (dialogTheme__8240?.data ?? Theme.of(context).dialogTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new DialogTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((DialogTheme)oldWidget).data)));
    public virtual DialogTheme copyWith(Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, Color? iconColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, Color? barrierColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null)
    {
        return new DialogTheme(backgroundColor: (backgroundColor ?? this.backgroundColor), elevation: (elevation ?? this.elevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shape: (shape ?? this.shape), alignment: (alignment ?? this.alignment), iconColor: (iconColor ?? this.iconColor), titleTextStyle: (titleTextStyle ?? this.titleTextStyle), contentTextStyle: (contentTextStyle ?? this.contentTextStyle), actionsPadding: (actionsPadding ?? this.actionsPadding), barrierColor: (barrierColor ?? this.barrierColor), insetPadding: (insetPadding ?? this.insetPadding), clipBehavior: (clipBehavior ?? this.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DialogTheme lerp(DialogTheme? a, DialogTheme? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new DialogTheme(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t), contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t), actionsPadding: EdgeInsetsGeometry.lerp(a?.actionsPadding, b?.actionsPadding, t), barrierColor: Dart_uiLibrary.Color.lerp(a?.barrierColor, b?.barrierColor, t), insetPadding: EdgeInsets.lerp(a?.insetPadding, b?.insetPadding, t), clipBehavior: ((t < 0.5) ? a?.clipBehavior : b?.clipBehavior));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("iconColor", this.iconColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("titleTextStyle", this.titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("contentTextStyle", this.contentTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("actionsPadding", this.actionsPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("barrierColor", this.barrierColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsets>("insetPadding", this.insetPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
    }

    public override string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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

public class DialogThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? barrierColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    public DialogThemeData(Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, Color? iconColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, Color? barrierColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null)
    {
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shadowColor = shadowColor;
        this.surfaceTintColor = surfaceTintColor;
        this.shape = shape;
        this.alignment = alignment;
        this.iconColor = iconColor;
        this.titleTextStyle = titleTextStyle;
        this.contentTextStyle = contentTextStyle;
        this.actionsPadding = actionsPadding;
        this.barrierColor = barrierColor;
        this.insetPadding = insetPadding;
        this.clipBehavior = clipBehavior;
        this.constraints = constraints;
    }

    public virtual DialogThemeData copyWith(Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Generated.Framework.Painting.AlignmentGeometry? alignment = null, Color? iconColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, Color? barrierColor = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null)
    {
        return new DialogThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), elevation: (elevation ?? this.elevation), shadowColor: (shadowColor ?? this.shadowColor), surfaceTintColor: (surfaceTintColor ?? this.surfaceTintColor), shape: (shape ?? this.shape), alignment: (alignment ?? this.alignment), iconColor: (iconColor ?? this.iconColor), titleTextStyle: (titleTextStyle ?? this.titleTextStyle), contentTextStyle: (contentTextStyle ?? this.contentTextStyle), actionsPadding: (actionsPadding ?? this.actionsPadding), barrierColor: (barrierColor ?? this.barrierColor), insetPadding: (insetPadding ?? this.insetPadding), clipBehavior: (clipBehavior ?? this.clipBehavior), constraints: (constraints ?? this.constraints));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DialogThemeData lerp(DialogThemeData? a, DialogThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new DialogThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t), contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t), actionsPadding: EdgeInsetsGeometry.lerp(a?.actionsPadding, b?.actionsPadding, t), barrierColor: Dart_uiLibrary.Color.lerp(a?.barrierColor, b?.barrierColor, t), insetPadding: EdgeInsets.lerp(a?.insetPadding, b?.insetPadding, t), clipBehavior: ((t < 0.5) ? a?.clipBehavior : b?.clipBehavior), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(new List<object?> { this.backgroundColor, this.elevation, this.shadowColor, this.surfaceTintColor, this.shape, this.alignment, this.iconColor, this.titleTextStyle, this.contentTextStyle, this.actionsPadding, this.barrierColor, this.insetPadding, this.clipBehavior, this.constraints }));
    public override bool Equals(object? other)
    {
        var __other = other as DialogThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((__other is DialogThemeData) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).backgroundColor, this.backgroundColor))) && (((DialogThemeData)((DialogThemeData)__other)).elevation == this.elevation)) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).shadowColor, this.shadowColor))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).surfaceTintColor, this.surfaceTintColor))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).shape, this.shape))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).alignment, this.alignment))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).iconColor, this.iconColor))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).titleTextStyle, this.titleTextStyle))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).contentTextStyle, this.contentTextStyle))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).actionsPadding, this.actionsPadding))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).barrierColor, this.barrierColor))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).insetPadding, this.insetPadding))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).clipBehavior, this.clipBehavior))) && (object.Equals(((DialogThemeData)((DialogThemeData)__other)).constraints, this.constraints)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("shadowColor", this.shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("surfaceTintColor", this.surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.AlignmentGeometry>("alignment", this.alignment, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("iconColor", this.iconColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("titleTextStyle", this.titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("contentTextStyle", this.contentTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("actionsPadding", this.actionsPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("barrierColor", this.barrierColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsets>("insetPadding", this.insetPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", this.clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null));
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
