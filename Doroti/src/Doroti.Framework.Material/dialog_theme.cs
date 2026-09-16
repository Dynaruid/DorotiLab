// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/dialog_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class DialogTheme : global::Doroti.Framework.Widgets.InheritedTheme, global::Doroti.Framework.Foundation.Diagnosticable
{
    internal virtual DialogThemeData? _data { get; private set; }
    internal virtual Color? _backgroundColor { get; private set; }
    internal virtual double? _elevation { get; private set; }
    internal virtual Color? _shadowColor { get; private set; }
    internal virtual Color? _surfaceTintColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.ShapeBorder? _shape { get; private set; }
    internal virtual global::Doroti.Framework.Painting.AlignmentGeometry? _alignment { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _titleTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.TextStyle? _contentTextStyle { get; private set; }
    internal virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? _actionsPadding { get; private set; }
    internal virtual Color? _iconColor { get; private set; }
    internal virtual Color? _barrierColor { get; private set; }
    internal virtual global::Doroti.Framework.Painting.EdgeInsets? _insetPadding { get; private set; }
    internal virtual Clip? _clipBehavior { get; private set; }

    public DialogTheme(global::Doroti.Framework.Foundation.Key? key = null, Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, Color? barrierColor = null, global::Doroti.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, DialogThemeData? data = null, global::Doroti.Framework.Widgets.Widget? child = null) : base(key: key, child: child ?? new global::Doroti.Framework.Widgets.SizedBox())
    {
        _data = data;
        _backgroundColor = backgroundColor;
        _elevation = elevation;
        _shadowColor = shadowColor;
        _surfaceTintColor = surfaceTintColor;
        _shape = shape;
        _alignment = alignment;
        _iconColor = iconColor;
        _titleTextStyle = titleTextStyle;
        _contentTextStyle = contentTextStyle;
        _actionsPadding = actionsPadding;
        _barrierColor = barrierColor;
        _insetPadding = insetPadding;
        _clipBehavior = clipBehavior;
        System.Diagnostics.Debug.Assert((data is null) || ((((((((((((((object?)backgroundColor ?? elevation) ?? shadowColor) ?? surfaceTintColor) ?? shape) ?? alignment) ?? iconColor) ?? titleTextStyle) ?? contentTextStyle) ?? actionsPadding) ?? barrierColor) ?? insetPadding) ?? clipBehavior) is null));
    }

    public virtual global::Doroti.Ui.Color? backgroundColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.backgroundColor : _backgroundColor);
    public virtual double? elevation => (_data is not null) ? _data.elevation : _elevation;
    public virtual global::Doroti.Ui.Color? shadowColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.shadowColor : _shadowColor);
    public virtual global::Doroti.Ui.Color? surfaceTintColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.surfaceTintColor : _surfaceTintColor);
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape => (_data is not null) ? _data.shape : _shape;
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment => (_data is not null) ? _data.alignment : _alignment;
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle => (_data is not null) ? _data.titleTextStyle : _titleTextStyle;
    public virtual global::Doroti.Framework.Painting.TextStyle? contentTextStyle => (_data is not null) ? _data.contentTextStyle : _contentTextStyle;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding => (_data is not null) ? _data.actionsPadding : _actionsPadding;
    public virtual global::Doroti.Ui.Color? iconColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.iconColor : _iconColor);
    public virtual global::Doroti.Ui.Color? barrierColor => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((_data is not null) ? _data.barrierColor : _barrierColor);
    public virtual global::Doroti.Framework.Painting.EdgeInsets? insetPadding => (_data is not null) ? _data.insetPadding : _insetPadding;
    public virtual global::Doroti.Ui.Clip? clipBehavior => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Clip>((_data is not null) ? _data.clipBehavior : _clipBehavior);
    public virtual DialogThemeData data
    {
        get
        {
            return _data ?? new DialogThemeData(backgroundColor: _backgroundColor, elevation: _elevation, shadowColor: _shadowColor, surfaceTintColor: _surfaceTintColor, shape: _shape, alignment: _alignment, iconColor: _iconColor, titleTextStyle: _titleTextStyle, contentTextStyle: _contentTextStyle, actionsPadding: _actionsPadding, barrierColor: _barrierColor, insetPadding: _insetPadding, clipBehavior: _clipBehavior);
        }
    }
    public static DialogThemeData of(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DialogTheme? dialogThemeLocal = context.dependOnInheritedWidgetOfExactType<DialogTheme>();
        return dialogThemeLocal?.data ?? Theme.of(context).dialogTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget wrap(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Widgets.Widget child)
    {
        return new DialogTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((DialogTheme)oldWidget).data));
    public virtual DialogTheme copyWith(Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, Color? barrierColor = null, global::Doroti.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null)
    {
        return new DialogTheme(backgroundColor: backgroundColor ?? this.backgroundColor, elevation: elevation ?? this.elevation, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, shape: shape ?? this.shape, alignment: alignment ?? this.alignment, iconColor: iconColor ?? this.iconColor, titleTextStyle: titleTextStyle ?? this.titleTextStyle, contentTextStyle: contentTextStyle ?? this.contentTextStyle, actionsPadding: actionsPadding ?? this.actionsPadding, barrierColor: barrierColor ?? this.barrierColor, insetPadding: insetPadding ?? this.insetPadding, clipBehavior: clipBehavior ?? this.clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DialogTheme lerp(DialogTheme? a, DialogTheme? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new DialogTheme(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t), contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t), actionsPadding: EdgeInsetsGeometry.lerp(a?.actionsPadding, b?.actionsPadding, t), barrierColor: Dart_uiLibrary.Color.lerp(a?.barrierColor, b?.barrierColor, t), insetPadding: EdgeInsets.lerp(a?.insetPadding, b?.insetPadding, t), clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("iconColor", iconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("titleTextStyle", titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("contentTextStyle", contentTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("actionsPadding", actionsPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("barrierColor", barrierColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsets>("insetPadding", insetPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", clipBehavior, defaultValue: null));
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

public class DialogThemeData : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual Color? shadowColor { get; private set; }
    public virtual Color? surfaceTintColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual global::Doroti.Framework.Painting.AlignmentGeometry? alignment { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? titleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding { get; private set; }
    public virtual Color? iconColor { get; private set; }
    public virtual Color? barrierColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual Clip? clipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Rendering.BoxConstraints? constraints { get; private set; }

    public DialogThemeData(Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, Color? barrierColor = null, global::Doroti.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null)
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

    public virtual DialogThemeData copyWith(Color? backgroundColor = null, double? elevation = null, Color? shadowColor = null, Color? surfaceTintColor = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, global::Doroti.Framework.Painting.AlignmentGeometry? alignment = null, Color? iconColor = null, global::Doroti.Framework.Painting.TextStyle? titleTextStyle = null, global::Doroti.Framework.Painting.TextStyle? contentTextStyle = null, global::Doroti.Framework.Painting.EdgeInsetsGeometry? actionsPadding = null, Color? barrierColor = null, global::Doroti.Framework.Painting.EdgeInsets? insetPadding = null, Clip? clipBehavior = null, global::Doroti.Framework.Rendering.BoxConstraints? constraints = null)
    {
        return new DialogThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, elevation: elevation ?? this.elevation, shadowColor: shadowColor ?? this.shadowColor, surfaceTintColor: surfaceTintColor ?? this.surfaceTintColor, shape: shape ?? this.shape, alignment: alignment ?? this.alignment, iconColor: iconColor ?? this.iconColor, titleTextStyle: titleTextStyle ?? this.titleTextStyle, contentTextStyle: contentTextStyle ?? this.contentTextStyle, actionsPadding: actionsPadding ?? this.actionsPadding, barrierColor: barrierColor ?? this.barrierColor, insetPadding: insetPadding ?? this.insetPadding, clipBehavior: clipBehavior ?? this.clipBehavior, constraints: constraints ?? this.constraints);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static DialogThemeData lerp(DialogThemeData? a, DialogThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new DialogThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shadowColor: Dart_uiLibrary.Color.lerp(a?.shadowColor, b?.shadowColor, t), surfaceTintColor: Dart_uiLibrary.Color.lerp(a?.surfaceTintColor, b?.surfaceTintColor, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), alignment: AlignmentGeometry.lerp(a?.alignment, b?.alignment, t), iconColor: Dart_uiLibrary.Color.lerp(a?.iconColor, b?.iconColor, t), titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t), contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t), actionsPadding: EdgeInsetsGeometry.lerp(a?.actionsPadding, b?.actionsPadding, t), barrierColor: Dart_uiLibrary.Color.lerp(a?.barrierColor, b?.barrierColor, t), insetPadding: EdgeInsets.lerp(a?.insetPadding, b?.insetPadding, t), clipBehavior: (t < 0.5) ? a?.clipBehavior : b?.clipBehavior, constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHashAll(new List<object?> { backgroundColor, elevation, shadowColor, surfaceTintColor, shape, alignment, iconColor, titleTextStyle, contentTextStyle, actionsPadding, barrierColor, insetPadding, clipBehavior, constraints }));
    public override bool Equals(object? other)
    {
        var __other = other as DialogThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is DialogThemeData) && Equals(__other.backgroundColor, backgroundColor) && (__other.elevation == elevation) && Equals(__other.shadowColor, shadowColor) && Equals(__other.surfaceTintColor, surfaceTintColor) && Equals(__other.shape, shape) && Equals(__other.alignment, alignment) && Equals(__other.iconColor, iconColor) && Equals(__other.titleTextStyle, titleTextStyle) && Equals(__other.contentTextStyle, contentTextStyle) && Equals(__other.actionsPadding, actionsPadding) && Equals(__other.barrierColor, barrierColor) && Equals(__other.insetPadding, insetPadding) && Equals(__other.clipBehavior, clipBehavior) && Equals(__other.constraints, constraints);
    }

    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadowColor", shadowColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTintColor", surfaceTintColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.AlignmentGeometry>("alignment", alignment, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("iconColor", iconColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("titleTextStyle", titleTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.TextStyle>("contentTextStyle", contentTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsetsGeometry>("actionsPadding", actionsPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("barrierColor", barrierColor, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Painting.EdgeInsets>("insetPadding", insetPadding, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Clip>("clipBehavior", clipBehavior, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Framework.Rendering.BoxConstraints>("constraints", constraints, defaultValue: null));
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
