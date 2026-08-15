// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/progress_indicator_theme.dart
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

public class ProgressIndicatorThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual Color? linearTrackColor { get; private set; }
    public virtual double? linearMinHeight { get; private set; }
    public virtual Color? circularTrackColor { get; private set; }
    public virtual Color? refreshBackgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? borderRadius { get; private set; }
    public virtual Color? stopIndicatorColor { get; private set; }
    public virtual double? stopIndicatorRadius { get; private set; }
    public virtual double? strokeWidth { get; private set; }
    public virtual double? strokeAlign { get; private set; }
    public virtual StrokeCap? strokeCap { get; private set; }
    public virtual global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints { get; private set; }
    public virtual double? trackGap { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? circularTrackPadding { get; private set; }
    public virtual bool? year2023 { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController? controller { get; private set; }

    public ProgressIndicatorThemeData(Color? color = null, Color? linearTrackColor = null, double? linearMinHeight = null, Color? circularTrackColor = null, Color? refreshBackgroundColor = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? borderRadius = null, Color? stopIndicatorColor = null, double? stopIndicatorRadius = null, double? strokeWidth = null, double? strokeAlign = null, StrokeCap? strokeCap = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, double? trackGap = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? circularTrackPadding = null, bool? year2023 = null, global::Doroti.Generated.Framework.Animation.AnimationController? controller = null)
    {
        this.color = color;
        this.linearTrackColor = linearTrackColor;
        this.linearMinHeight = linearMinHeight;
        this.circularTrackColor = circularTrackColor;
        this.refreshBackgroundColor = refreshBackgroundColor;
        this.borderRadius = borderRadius;
        this.stopIndicatorColor = stopIndicatorColor;
        this.stopIndicatorRadius = stopIndicatorRadius;
        this.strokeWidth = strokeWidth;
        this.strokeAlign = strokeAlign;
        this.strokeCap = strokeCap;
        this.constraints = constraints;
        this.trackGap = trackGap;
        this.circularTrackPadding = circularTrackPadding;
        this.year2023 = year2023;
        this.controller = controller;
    }

    public virtual ProgressIndicatorThemeData copyWith(Color? color = null, Color? linearTrackColor = null, double? linearMinHeight = null, Color? circularTrackColor = null, Color? refreshBackgroundColor = null, global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry? borderRadius = null, Color? stopIndicatorColor = null, double? stopIndicatorRadius = null, double? strokeWidth = null, double? strokeAlign = null, StrokeCap? strokeCap = null, global::Doroti.Generated.Framework.Rendering.BoxConstraints? constraints = null, double? trackGap = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry? circularTrackPadding = null, bool? year2023 = null, global::Doroti.Generated.Framework.Animation.AnimationController? controller = null)
    {
        return new ProgressIndicatorThemeData(color: (color ?? this.color), linearTrackColor: (linearTrackColor ?? this.linearTrackColor), linearMinHeight: (linearMinHeight ?? this.linearMinHeight), circularTrackColor: (circularTrackColor ?? this.circularTrackColor), refreshBackgroundColor: (refreshBackgroundColor ?? this.refreshBackgroundColor), borderRadius: (borderRadius ?? this.borderRadius), stopIndicatorColor: (stopIndicatorColor ?? this.stopIndicatorColor), stopIndicatorRadius: (stopIndicatorRadius ?? this.stopIndicatorRadius), strokeWidth: (strokeWidth ?? this.strokeWidth), strokeAlign: (strokeAlign ?? this.strokeAlign), strokeCap: (strokeCap ?? this.strokeCap), constraints: (constraints ?? this.constraints), trackGap: (trackGap ?? this.trackGap), circularTrackPadding: (circularTrackPadding ?? this.circularTrackPadding), year2023: (year2023 ?? this.year2023), controller: (controller ?? this.controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ProgressIndicatorThemeData? lerp(ProgressIndicatorThemeData? a, ProgressIndicatorThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ProgressIndicatorThemeData(color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), linearTrackColor: Dart_uiLibrary.Color.lerp(a?.linearTrackColor, b?.linearTrackColor, t), linearMinHeight: Dart_uiLibrary.lerpDouble(a?.linearMinHeight, b?.linearMinHeight, t), circularTrackColor: Dart_uiLibrary.Color.lerp(a?.circularTrackColor, b?.circularTrackColor, t), refreshBackgroundColor: Dart_uiLibrary.Color.lerp(a?.refreshBackgroundColor, b?.refreshBackgroundColor, t), borderRadius: BorderRadiusGeometry.lerp(a?.borderRadius, b?.borderRadius, t), stopIndicatorColor: Dart_uiLibrary.Color.lerp(a?.stopIndicatorColor, b?.stopIndicatorColor, t), stopIndicatorRadius: Dart_uiLibrary.lerpDouble(a?.stopIndicatorRadius, b?.stopIndicatorRadius, t), strokeWidth: Dart_uiLibrary.lerpDouble(a?.strokeWidth, b?.strokeWidth, t), strokeAlign: Dart_uiLibrary.lerpDouble(a?.strokeAlign, b?.strokeAlign, t), strokeCap: ((t < 0.5) ? a?.strokeCap : b?.strokeCap), constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t), trackGap: Dart_uiLibrary.lerpDouble(a?.trackGap, b?.trackGap, t), circularTrackPadding: EdgeInsetsGeometry.lerp(a?.circularTrackPadding, b?.circularTrackPadding, t), year2023: ((t < 0.5) ? a?.year2023 : b?.year2023), controller: ((t < 0.5) ? a?.controller : b?.controller));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.color, this.linearTrackColor, this.linearMinHeight, this.circularTrackColor, this.refreshBackgroundColor, this.borderRadius, this.stopIndicatorColor, this.stopIndicatorRadius, this.strokeAlign, this.strokeWidth, this.strokeCap, this.constraints, this.trackGap, this.circularTrackPadding, this.year2023, this.controller));
    public override bool Equals(object? other)
    {
        var __other = other as ProgressIndicatorThemeData;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((((__other is ProgressIndicatorThemeData) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).color, this.color))) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).linearTrackColor, this.linearTrackColor))) && (((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).linearMinHeight == this.linearMinHeight)) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).circularTrackColor, this.circularTrackColor))) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).refreshBackgroundColor, this.refreshBackgroundColor))) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).borderRadius, this.borderRadius))) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).stopIndicatorColor, this.stopIndicatorColor))) && (((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).stopIndicatorRadius == this.stopIndicatorRadius)) && (((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).strokeAlign == this.strokeAlign)) && (((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).strokeWidth == this.strokeWidth)) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).strokeCap, this.strokeCap))) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).constraints, this.constraints))) && (((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).trackGap == this.trackGap)) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).circularTrackPadding, this.circularTrackPadding))) && (((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).year2023 == this.year2023)) && (object.Equals(((ProgressIndicatorThemeData)((ProgressIndicatorThemeData)__other)).controller, this.controller)));
    }

    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("linearTrackColor", this.linearTrackColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("linearMinHeight", this.linearMinHeight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("circularTrackColor", this.circularTrackColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("refreshBackgroundColor", this.refreshBackgroundColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.BorderRadiusGeometry>("borderRadius", this.borderRadius, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("stopIndicatorColor", this.stopIndicatorColor, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("stopIndicatorRadius", this.stopIndicatorRadius, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("strokeWidth", this.strokeWidth, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("strokeAlign", this.strokeAlign, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.StrokeCap>("strokeCap", this.strokeCap, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.BoxConstraints>("constraints", this.constraints, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("trackGap", this.trackGap, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.EdgeInsetsGeometry>("circularTrackPadding", this.circularTrackPadding, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("year2023", this.year2023, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Animation.AnimationController>("controller", this.controller, defaultValue: null));
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

public class ProgressIndicatorTheme : global::Doroti.Generated.Framework.Widgets.InheritedTheme
{
    public virtual ProgressIndicatorThemeData data { get; private set; } = default!;

    public ProgressIndicatorTheme(global::Doroti.Generated.Framework.Foundation.Key? key = null, ProgressIndicatorThemeData data = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!) : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ProgressIndicatorThemeData of(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ProgressIndicatorTheme? progressIndicatorTheme__13497 = ((ProgressIndicatorTheme?)(object?)context.dependOnInheritedWidgetOfExactType<ProgressIndicatorTheme>());
        return (progressIndicatorTheme__13497?.data ?? Theme.of(context).progressIndicatorTheme);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget wrap(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new ProgressIndicatorTheme(data: this.data, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(global::Doroti.Generated.Framework.Widgets.InheritedWidget oldWidget) => DartRuntimePrimitives.ConvertValue<bool>((!object.Equals(this.data, ((ProgressIndicatorTheme)oldWidget).data)));
}
