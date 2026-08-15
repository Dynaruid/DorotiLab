// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/snack_bar_theme.dart
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

public enum SnackBarBehavior
{
    @fixed,
    floating
}

public class SnackBarThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? backgroundColor { get; private set; }
    public virtual Color? actionTextColor { get; private set; }
    public virtual Color? disabledActionTextColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual SnackBarBehavior? behavior { get; private set; }
    public virtual double? width { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding { get; private set; }
    public virtual bool? showCloseIcon { get; private set; }
    public virtual Color? closeIconColor { get; private set; }
    public virtual double? actionOverflowThreshold { get; private set; }
    public virtual Color? actionBackgroundColor { get; private set; }
    public virtual Color? disabledActionBackgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.DismissDirection? dismissDirection { get; private set; }

    public SnackBarThemeData(Color? backgroundColor = null, Color? actionTextColor = null, Color? disabledActionTextColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, SnackBarBehavior? behavior = null, double? width = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, bool? showCloseIcon = null, Color? closeIconColor = null, double? actionOverflowThreshold = null, Color? actionBackgroundColor = null, Color? disabledActionBackgroundColor = null, global::Doroti.Generated.Framework.Widgets.DismissDirection? dismissDirection = null)
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
        System.Diagnostics.Debug.Assert(((elevation is null) || (elevation >= 0.0)));
        System.Diagnostics.Debug.Assert(((width is null) || DartRuntimePrimitives.Identical(behavior, SnackBarBehavior.floating)));
        System.Diagnostics.Debug.Assert(((actionOverflowThreshold is null) || (((actionOverflowThreshold >= 0L) && (actionOverflowThreshold <= 1L)))));
        System.Diagnostics.Debug.Assert(((actionBackgroundColor is not global::Doroti.Generated.Framework.Widgets.WidgetStateColor) || (disabledActionBackgroundColor is null)));
    }

    public virtual SnackBarThemeData copyWith(Color? backgroundColor = null, Color? actionTextColor = null, Color? disabledActionTextColor = null, global::Doroti.Generated.Framework.Painting.TextStyle? contentTextStyle = null, double? elevation = null, global::Doroti.Generated.Framework.Painting.ShapeBorder? shape = null, SnackBarBehavior? behavior = null, double? width = null, global::Doroti.Generated.Framework.Painting.EdgeInsets? insetPadding = null, bool? showCloseIcon = null, Color? closeIconColor = null, double? actionOverflowThreshold = null, Color? actionBackgroundColor = null, Color? disabledActionBackgroundColor = null, global::Doroti.Generated.Framework.Widgets.DismissDirection? dismissDirection = null)
    {
        return new SnackBarThemeData(backgroundColor: (backgroundColor ?? this.backgroundColor), actionTextColor: (actionTextColor ?? this.actionTextColor), disabledActionTextColor: (disabledActionTextColor ?? this.disabledActionTextColor), contentTextStyle: (contentTextStyle ?? this.contentTextStyle), elevation: (elevation ?? this.elevation), shape: (shape ?? this.shape), behavior: (behavior ?? this.behavior), width: (width ?? this.width), insetPadding: (insetPadding ?? this.insetPadding), showCloseIcon: (showCloseIcon ?? this.showCloseIcon), closeIconColor: (closeIconColor ?? this.closeIconColor), actionOverflowThreshold: (actionOverflowThreshold ?? this.actionOverflowThreshold), actionBackgroundColor: (actionBackgroundColor ?? this.actionBackgroundColor), disabledActionBackgroundColor: (disabledActionBackgroundColor ?? this.disabledActionBackgroundColor), dismissDirection: (dismissDirection ?? this.dismissDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static SnackBarThemeData lerp(SnackBarThemeData? a, SnackBarThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new SnackBarThemeData(backgroundColor: Dart_uiLibrary.Color.lerp(a?.backgroundColor, b?.backgroundColor, t), actionTextColor: Dart_uiLibrary.Color.lerp(a?.actionTextColor, b?.actionTextColor, t), disabledActionTextColor: Dart_uiLibrary.Color.lerp(a?.disabledActionTextColor, b?.disabledActionTextColor, t), contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t), elevation: Dart_uiLibrary.lerpDouble(a?.elevation, b?.elevation, t), shape: ShapeBorder.lerp(a?.shape, b?.shape, t), behavior: ((t < 0.5) ? a?.behavior : b?.behavior), width: Dart_uiLibrary.lerpDouble(a?.width, b?.width, t), insetPadding: EdgeInsets.lerp(a?.insetPadding, b?.insetPadding, t), closeIconColor: Dart_uiLibrary.Color.lerp(a?.closeIconColor, b?.closeIconColor, t), actionOverflowThreshold: Dart_uiLibrary.lerpDouble(a?.actionOverflowThreshold, b?.actionOverflowThreshold, t), actionBackgroundColor: Dart_uiLibrary.Color.lerp(a?.actionBackgroundColor, b?.actionBackgroundColor, t), disabledActionBackgroundColor: Dart_uiLibrary.Color.lerp(a?.disabledActionBackgroundColor, b?.disabledActionBackgroundColor, t), dismissDirection: ((t < 0.5) ? a?.dismissDirection : b?.dismissDirection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.backgroundColor, this.actionTextColor, this.disabledActionTextColor, this.contentTextStyle, this.elevation, this.shape, this.behavior, this.width, this.insetPadding, this.showCloseIcon, this.closeIconColor, this.actionOverflowThreshold, this.actionBackgroundColor, this.disabledActionBackgroundColor, this.dismissDirection));
    public override bool Equals(object? other)
    {
        var __other = other as SnackBarThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((__other is SnackBarThemeData) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).backgroundColor, this.backgroundColor))) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).actionTextColor, this.actionTextColor))) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).disabledActionTextColor, this.disabledActionTextColor))) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).contentTextStyle, this.contentTextStyle))) && (((SnackBarThemeData)((SnackBarThemeData)__other)).elevation == this.elevation)) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).shape, this.shape))) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).behavior, this.behavior))) && (((SnackBarThemeData)((SnackBarThemeData)__other)).width == this.width)) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).insetPadding, this.insetPadding))) && (((SnackBarThemeData)((SnackBarThemeData)__other)).showCloseIcon == this.showCloseIcon)) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).closeIconColor, this.closeIconColor))) && (((SnackBarThemeData)((SnackBarThemeData)__other)).actionOverflowThreshold == this.actionOverflowThreshold)) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).actionBackgroundColor, this.actionBackgroundColor))) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).disabledActionBackgroundColor, this.disabledActionBackgroundColor))) && (object.Equals(((SnackBarThemeData)((SnackBarThemeData)__other)).dismissDirection, this.dismissDirection)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("backgroundColor", this.backgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("actionTextColor", this.actionTextColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("disabledActionTextColor", this.disabledActionTextColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("contentTextStyle", this.contentTextStyle, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("elevation", this.elevation, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.ShapeBorder>("shape", this.shape, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<SnackBarBehavior>("behavior", this.behavior, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("width", this.width, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsets>("insetPadding", this.insetPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("showCloseIcon", this.showCloseIcon, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("closeIconColor", this.closeIconColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("actionOverflowThreshold", this.actionOverflowThreshold, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("actionBackgroundColor", this.actionBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("disabledActionBackgroundColor", this.disabledActionBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Widgets.DismissDirection>("dismissDirection", this.dismissDirection, defaultValue: null));
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

public class SnackBarTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual SnackBarThemeData data { get; private set; } = default!;

    public SnackBarTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, SnackBarThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static SnackBarThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        SnackBarTheme? snackBarTheme__14012 = ((SnackBarTheme?)(object?)context.dependOnInheritedWidgetOfExactType<SnackBarTheme>());
        return (snackBarTheme__14012?.data ?? Theme.of(context).snackBarTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new SnackBarTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((SnackBarTheme)oldWidget).data)));
}
