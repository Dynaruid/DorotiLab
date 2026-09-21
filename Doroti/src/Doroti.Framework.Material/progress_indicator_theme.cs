// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/progress_indicator_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class ProgressIndicatorThemeData : Diagnosticable
{
    public virtual Color? color { get; private set; }
    public virtual Color? linearTrackColor { get; private set; }
    public virtual double? linearMinHeight { get; private set; }
    public virtual Color? circularTrackColor { get; private set; }
    public virtual Color? refreshBackgroundColor { get; private set; }
    public virtual BorderRadiusGeometry? borderRadius { get; private set; }
    public virtual Color? stopIndicatorColor { get; private set; }
    public virtual double? stopIndicatorRadius { get; private set; }
    public virtual double? strokeWidth { get; private set; }
    public virtual double? strokeAlign { get; private set; }
    public virtual StrokeCap? strokeCap { get; private set; }
    public virtual BoxConstraints? constraints { get; private set; }
    public virtual double? trackGap { get; private set; }
    public virtual EdgeInsetsGeometry? circularTrackPadding { get; private set; }
    public virtual bool? year2023 { get; private set; }
    public virtual AnimationController? controller { get; private set; }

    public ProgressIndicatorThemeData(
        Color? color = null,
        Color? linearTrackColor = null,
        double? linearMinHeight = null,
        Color? circularTrackColor = null,
        Color? refreshBackgroundColor = null,
        BorderRadiusGeometry? borderRadius = null,
        Color? stopIndicatorColor = null,
        double? stopIndicatorRadius = null,
        double? strokeWidth = null,
        double? strokeAlign = null,
        StrokeCap? strokeCap = null,
        BoxConstraints? constraints = null,
        double? trackGap = null,
        EdgeInsetsGeometry? circularTrackPadding = null,
        bool? year2023 = null,
        AnimationController? controller = null
    )
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

    public virtual ProgressIndicatorThemeData copyWith(
        Color? color = null,
        Color? linearTrackColor = null,
        double? linearMinHeight = null,
        Color? circularTrackColor = null,
        Color? refreshBackgroundColor = null,
        BorderRadiusGeometry? borderRadius = null,
        Color? stopIndicatorColor = null,
        double? stopIndicatorRadius = null,
        double? strokeWidth = null,
        double? strokeAlign = null,
        StrokeCap? strokeCap = null,
        BoxConstraints? constraints = null,
        double? trackGap = null,
        EdgeInsetsGeometry? circularTrackPadding = null,
        bool? year2023 = null,
        AnimationController? controller = null
    )
    {
        return new ProgressIndicatorThemeData(
            color: color ?? this.color,
            linearTrackColor: linearTrackColor ?? this.linearTrackColor,
            linearMinHeight: linearMinHeight ?? this.linearMinHeight,
            circularTrackColor: circularTrackColor ?? this.circularTrackColor,
            refreshBackgroundColor: refreshBackgroundColor ?? this.refreshBackgroundColor,
            borderRadius: borderRadius ?? this.borderRadius,
            stopIndicatorColor: stopIndicatorColor ?? this.stopIndicatorColor,
            stopIndicatorRadius: stopIndicatorRadius ?? this.stopIndicatorRadius,
            strokeWidth: strokeWidth ?? this.strokeWidth,
            strokeAlign: strokeAlign ?? this.strokeAlign,
            strokeCap: strokeCap ?? this.strokeCap,
            constraints: constraints ?? this.constraints,
            trackGap: trackGap ?? this.trackGap,
            circularTrackPadding: circularTrackPadding ?? this.circularTrackPadding,
            year2023: year2023 ?? this.year2023,
            controller: controller ?? this.controller
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ProgressIndicatorThemeData? lerp(
        ProgressIndicatorThemeData? a,
        ProgressIndicatorThemeData? b,
        double t
    )
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ProgressIndicatorThemeData(
            color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t),
            linearTrackColor: Dart_uiLibrary.Color.lerp(
                a?.linearTrackColor,
                b?.linearTrackColor,
                t
            ),
            linearMinHeight: Dart_uiLibrary.lerpDouble(a?.linearMinHeight, b?.linearMinHeight, t),
            circularTrackColor: Dart_uiLibrary.Color.lerp(
                a?.circularTrackColor,
                b?.circularTrackColor,
                t
            ),
            refreshBackgroundColor: Dart_uiLibrary.Color.lerp(
                a?.refreshBackgroundColor,
                b?.refreshBackgroundColor,
                t
            ),
            borderRadius: BorderRadiusGeometry.lerp(a?.borderRadius, b?.borderRadius, t),
            stopIndicatorColor: Dart_uiLibrary.Color.lerp(
                a?.stopIndicatorColor,
                b?.stopIndicatorColor,
                t
            ),
            stopIndicatorRadius: Dart_uiLibrary.lerpDouble(
                a?.stopIndicatorRadius,
                b?.stopIndicatorRadius,
                t
            ),
            strokeWidth: Dart_uiLibrary.lerpDouble(a?.strokeWidth, b?.strokeWidth, t),
            strokeAlign: Dart_uiLibrary.lerpDouble(a?.strokeAlign, b?.strokeAlign, t),
            strokeCap: (t < 0.5) ? a?.strokeCap : b?.strokeCap,
            constraints: BoxConstraints.lerp(a?.constraints, b?.constraints, t),
            trackGap: Dart_uiLibrary.lerpDouble(a?.trackGap, b?.trackGap, t),
            circularTrackPadding: EdgeInsetsGeometry.lerp(
                a?.circularTrackPadding,
                b?.circularTrackPadding,
                t
            ),
            year2023: (t < 0.5) ? a?.year2023 : b?.year2023,
            controller: (t < 0.5) ? a?.controller : b?.controller
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                color,
                linearTrackColor,
                linearMinHeight,
                circularTrackColor,
                refreshBackgroundColor,
                borderRadius,
                stopIndicatorColor,
                stopIndicatorRadius,
                strokeAlign,
                strokeWidth,
                strokeCap,
                constraints,
                trackGap,
                circularTrackPadding,
                year2023,
                controller
            )
        );

    public override bool Equals(object? other)
    {
        var __other = other as ProgressIndicatorThemeData;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ProgressIndicatorThemeData)
            && Equals(__other.color, color)
            && Equals(__other.linearTrackColor, linearTrackColor)
            && (__other.linearMinHeight == linearMinHeight)
            && Equals(__other.circularTrackColor, circularTrackColor)
            && Equals(__other.refreshBackgroundColor, refreshBackgroundColor)
            && Equals(__other.borderRadius, borderRadius)
            && Equals(__other.stopIndicatorColor, stopIndicatorColor)
            && (__other.stopIndicatorRadius == stopIndicatorRadius)
            && (__other.strokeAlign == strokeAlign)
            && (__other.strokeWidth == strokeWidth)
            && Equals(__other.strokeCap, strokeCap)
            && Equals(__other.constraints, constraints)
            && (__other.trackGap == trackGap)
            && Equals(__other.circularTrackPadding, circularTrackPadding)
            && (__other.year2023 == year2023)
            && Equals(__other.controller, controller);
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new ColorProperty("linearTrackColor", linearTrackColor, defaultValue: null));
        properties.add(new DoubleProperty("linearMinHeight", linearMinHeight, defaultValue: null));
        properties.add(
            new ColorProperty("circularTrackColor", circularTrackColor, defaultValue: null)
        );
        properties.add(
            new ColorProperty("refreshBackgroundColor", refreshBackgroundColor, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<BorderRadiusGeometry>(
                "borderRadius",
                borderRadius,
                defaultValue: null
            )
        );
        properties.add(
            new ColorProperty("stopIndicatorColor", stopIndicatorColor, defaultValue: null)
        );
        properties.add(
            new DoubleProperty("stopIndicatorRadius", stopIndicatorRadius, defaultValue: null)
        );
        properties.add(new DoubleProperty("strokeWidth", strokeWidth, defaultValue: null));
        properties.add(new DoubleProperty("strokeAlign", strokeAlign, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<StrokeCap>("strokeCap", strokeCap, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<BoxConstraints>("constraints", constraints, defaultValue: null)
        );
        properties.add(new DoubleProperty("trackGap", trackGap, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<EdgeInsetsGeometry>(
                "circularTrackPadding",
                circularTrackPadding,
                defaultValue: null
            )
        );
        properties.add(new DiagnosticsProperty<bool>("year2023", year2023, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<AnimationController>(
                "controller",
                controller,
                defaultValue: null
            )
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class ProgressIndicatorTheme : InheritedTheme
{
    public virtual ProgressIndicatorThemeData data { get; private set; } = default!;

    public ProgressIndicatorTheme(
        Key? key = null,
        ProgressIndicatorThemeData data = default!,
        Widget child = default!
    )
        : base(key: key, child: child)
    {
        this.data = data;
    }

    public static ProgressIndicatorThemeData of(BuildContext context)
    {
        ProgressIndicatorTheme? progressIndicatorThemeLocal =
            context.dependOnInheritedWidgetOfExactType<ProgressIndicatorTheme>();
        return progressIndicatorThemeLocal?.data ?? Theme.of(context).progressIndicatorTheme;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget wrap(BuildContext context, Widget child)
    {
        return new ProgressIndicatorTheme(data: data, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        DartRuntimePrimitives.ConvertValue<bool>(
            !Equals(data, ((ProgressIndicatorTheme)oldWidget).data)
        );
}
