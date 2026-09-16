// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/snack_bar_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public enum SnackBarBehavior
{
    @fixed,
    floating
}

public class SnackBarThemeData : Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? actionTextColor { get; private set; }
    public virtual Color? disabledActionTextColor { get; private set; }
    public virtual TextStyle? contentTextStyle { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual ShapeBorder? shape { get; private set; }
    public virtual SnackBarBehavior? behavior { get; private set; }
    public virtual double? width { get; private set; }
    public virtual EdgeInsets? insetPadding { get; private set; }
    public virtual bool? showCloseIcon { get; private set; }
    public virtual Color? closeIconColor { get; private set; }
    public virtual double? actionOverflowThreshold { get; private set; }
    public virtual Color? actionBackgroundColor { get; private set; }
    public virtual Color? disabledActionBackgroundColor { get; private set; }
    public virtual DismissDirection? dismissDirection { get; private set; }

    public SnackBarThemeData(Color? backgroundColor = null, Color? actionTextColor = null, Color? disabledActionTextColor = null, TextStyle? contentTextStyle = null, double? elevation = null, ShapeBorder? shape = null, SnackBarBehavior? behavior = null, double? width = null, EdgeInsets? insetPadding = null, bool? showCloseIcon = null, Color? closeIconColor = null, double? actionOverflowThreshold = null, Color? actionBackgroundColor = null, Color? disabledActionBackgroundColor = null, DismissDirection? dismissDirection = null)
    {
        this.backgroundColor = backgroundColor;
        this.actionTextColor = actionTextColor;
        this.disabledActionTextColor = disabledActionTextColor;
        this.contentTextStyle = contentTextStyle;
        this.elevation = elevation;
        this.shape = shape;
        this.behavior = behavior;
        this.width = width;
        this.insetPadding = insetPadding;
        this.showCloseIcon = showCloseIcon;
        this.closeIconColor = closeIconColor;
        this.actionOverflowThreshold = actionOverflowThreshold;
        this.actionBackgroundColor = actionBackgroundColor;
        this.disabledActionBackgroundColor = disabledActionBackgroundColor;
        this.dismissDirection = dismissDirection;
        System.Diagnostics.Debug.Assert((elevation is null) || (elevation >= 0.0));
        System.Diagnostics.Debug.Assert((width is null) || DartRuntimePrimitives.Identical(behavior, SnackBarBehavior.floating));
        System.Diagnostics.Debug.Assert((actionOverflowThreshold is null) || (actionOverflowThreshold >= 0L) && (actionOverflowThreshold <= 1L));
        System.Diagnostics.Debug.Assert((actionBackgroundColor is not WidgetStateColor) || (disabledActionBackgroundColor is null));
    }

    public virtual SnackBarThemeData copyWith(Color? backgroundColor = null, Color? actionTextColor = null, Color? disabledActionTextColor = null, TextStyle? contentTextStyle = null, double? elevation = null, ShapeBorder? shape = null, SnackBarBehavior? behavior = null, double? width = null, EdgeInsets? insetPadding = null, bool? showCloseIcon = null, Color? closeIconColor = null, double? actionOverflowThreshold = null, Color? actionBackgroundColor = null, Color? disabledActionBackgroundColor = null, DismissDirection? dismissDirection = null)
    {
        return new SnackBarThemeData(backgroundColor: backgroundColor ?? this.backgroundColor, actionTextColor: actionTextColor ?? this.actionTextColor, disabledActionTextColor: disabledActionTextColor ?? this.disabledActionTextColor, contentTextStyle: contentTextStyle ?? this.contentTextStyle, elevation: elevation ?? this.elevation, shape: shape ?? this.shape, behavior: behavior ?? this.behavior, width: width ?? this.width, insetPadding: insetPadding ?? this.insetPadding, showCloseIcon: showCloseIcon ?? this.showCloseIcon, closeIconColor: closeIconColor ?? this.closeIconColor, actionOverflowThreshold: actionOverflowThreshold ?? this.actionOverflowThreshold, actionBackgroundColor: actionBackgroundColor ?? this.actionBackgroundColor, disabledActionBackgroundColor: disabledActionBackgroundColor ?? this.disabledActionBackgroundColor, dismissDirection: dismissDirection ?? this.dismissDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SnackBarThemeData lerp(SnackBarThemeData? a, SnackBarThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new SnackBarThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), actionTextColor: Dart_uiLibrary.Color.lerp(a?.actionTextColor, b?.actionTextColor, t), disabledActionTextColor: Dart_uiLibrary.Color.lerp(a?.disabledActionTextColor, b?.disabledActionTextColor, t), contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), behavior: (t < 0.5) ? a?.behavior : b?.behavior, width: Dart_uiLibrary.lerpDouble(a?.width, b?.width, t), insetPadding: EdgeInsets.lerp(a?.insetPadding, b?.insetPadding, t), closeIconColor: Dart_uiLibrary.Color.lerp(a?.closeIconColor, b?.closeIconColor, t), actionOverflowThreshold: Dart_uiLibrary.lerpDouble(a?.actionOverflowThreshold, b?.actionOverflowThreshold, t), actionBackgroundColor: Dart_uiLibrary.Color.lerp(a?.actionBackgroundColor, b?.actionBackgroundColor, t), disabledActionBackgroundColor: Dart_uiLibrary.Color.lerp(a?.disabledActionBackgroundColor, b?.disabledActionBackgroundColor, t), dismissDirection: (t < 0.5) ? a?.dismissDirection : b?.dismissDirection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(backgroundColor, actionTextColor, disabledActionTextColor, contentTextStyle, elevation, shape, behavior, width, insetPadding, showCloseIcon, closeIconColor, actionOverflowThreshold, actionBackgroundColor, disabledActionBackgroundColor, dismissDirection));
    public override bool Equals(object? other)
    {
        var __other = other as SnackBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is SnackBarThemeData) && Equals(__other.backgroundColor, backgroundColor) && Equals(__other.actionTextColor, actionTextColor) && Equals(__other.disabledActionTextColor, disabledActionTextColor) && Equals(__other.contentTextStyle, contentTextStyle) && (__other.elevation == elevation) && Equals(__other.shape, shape) && Equals(__other.behavior, behavior) && (__other.width == width) && Equals(__other.insetPadding, insetPadding) && (__other.showCloseIcon == showCloseIcon) && Equals(__other.closeIconColor, closeIconColor) && (__other.actionOverflowThreshold == actionOverflowThreshold) && Equals(__other.actionBackgroundColor, actionBackgroundColor) && Equals(__other.disabledActionBackgroundColor, disabledActionBackgroundColor) && Equals(__other.dismissDirection, dismissDirection);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("backgroundColor", backgroundColor, defaultValue: null));
        properties.add(new ColorProperty("actionTextColor", actionTextColor, defaultValue: null));
        properties.add(new ColorProperty("disabledActionTextColor", disabledActionTextColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<TextStyle>("contentTextStyle", contentTextStyle, defaultValue: null));
        properties.add(new DoubleProperty("elevation", elevation, defaultValue: null));
        properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape, defaultValue: null));
        properties.add(new DiagnosticsProperty<SnackBarBehavior>("behavior", behavior, defaultValue: null));
        properties.add(new DoubleProperty("width", width, defaultValue: null));
        properties.add(new DiagnosticsProperty<EdgeInsets>("insetPadding", insetPadding, defaultValue: null));
        properties.add(new DiagnosticsProperty<bool>("showCloseIcon", showCloseIcon, defaultValue: null));
        properties.add(new ColorProperty("closeIconColor", closeIconColor, defaultValue: null));
        properties.add(new DoubleProperty("actionOverflowThreshold", actionOverflowThreshold, defaultValue: null));
        properties.add(new ColorProperty("actionBackgroundColor", actionBackgroundColor, defaultValue: null));
        properties.add(new ColorProperty("disabledActionBackgroundColor", disabledActionBackgroundColor, defaultValue: null));
        properties.add(new DiagnosticsProperty<DismissDirection>("dismissDirection", dismissDirection, defaultValue: null));
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

public class SnackBarTheme : InheritedTheme
{
    public virtual SnackBarThemeData data { get; private set; } = default!;

    public SnackBarTheme(Key? key = null, SnackBarThemeData data = default!, Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SnackBarThemeData of(BuildContext context)
    {
        SnackBarTheme? snackBarThemeLocal = context.dependOnInheritedWidgetOfExactType<SnackBarTheme>();
        return snackBarThemeLocal?.data ?? Theme.of(context).snackBarTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new SnackBarTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>(!Equals(data, ((SnackBarTheme)oldWidget).data));
}
