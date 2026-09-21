// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/icon_theme_data.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class IconThemeData : Diagnosticable
{
    public virtual double? size { get; private set; }
    public virtual double? fill { get; private set; }
    public virtual double? weight { get; private set; }
    public virtual double? grade { get; private set; }
    public virtual double? opticalSize { get; private set; }
    public virtual Color? color { get; private set; }
    internal virtual double? _opacity { get; private set; }
    public virtual List<Shadow>? shadows { get; private set; }
    public virtual bool? applyTextScaling { get; private set; }

    public IconThemeData(
        double? size = null,
        double? fill = null,
        double? weight = null,
        double? grade = null,
        double? opticalSize = null,
        Color? color = null,
        double? opacity = null,
        List<Shadow>? shadows = null,
        bool? applyTextScaling = null
    )
    {
        this.size = size;
        this.fill = fill;
        this.weight = weight;
        this.grade = grade;
        this.opticalSize = opticalSize;
        this.color = color;
        this.shadows = shadows;
        this.applyTextScaling = applyTextScaling;
        _opacity = opacity;
        System.Diagnostics.Debug.Assert(
            (fill is null)
                || (
                    (
                        0.0
                        <= (
                            fill
                            ?? throw new global::System.NullReferenceException(
                                "A required value was null."
                            )
                        )
                    ) && (fill <= 1.0)
                )
        );
        System.Diagnostics.Debug.Assert(
            (weight is null)
                || 0.0
                    < (
                        weight
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
        );
        System.Diagnostics.Debug.Assert(
            (opticalSize is null)
                || 0.0
                    < (
                        opticalSize
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
        );
    }

    public static IconThemeData CreateFallback()
    {
        var __instance = new IconThemeData(
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!,
            default!
        );
        __instance.size = 24.0;
        __instance.fill = 0.0;
        __instance.weight = 400.0;
        __instance.grade = 0.0;
        __instance.opticalSize = 48.0;
        __instance.color = new Color(4278190080L);
        __instance._opacity = 1.0;
        __instance.shadows = null;
        __instance.applyTextScaling = false;
        return __instance;
    }

    public virtual IconThemeData copyWith(
        double? size = null,
        double? fill = null,
        double? weight = null,
        double? grade = null,
        double? opticalSize = null,
        Color? color = null,
        double? opacity = null,
        List<Shadow>? shadows = null,
        bool? applyTextScaling = null
    )
    {
        return new IconThemeData(
            size: size ?? this.size,
            fill: fill ?? this.fill,
            weight: weight ?? this.weight,
            grade: grade ?? this.grade,
            opticalSize: opticalSize ?? this.opticalSize,
            color: color ?? this.color,
            opacity: opacity ?? this.opacity,
            shadows: shadows ?? this.shadows,
            applyTextScaling: applyTextScaling ?? this.applyTextScaling
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IconThemeData merge(IconThemeData? other)
    {
        if (other is null)
        {
            return this;
        }
        return copyWith(
            size: other.size,
            fill: other.fill,
            weight: other.weight,
            grade: other.grade,
            opticalSize: other.opticalSize,
            color: other.color,
            opacity: other.opacity,
            shadows: other.shadows,
            applyTextScaling: other.applyTextScaling
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual IconThemeData resolve(BuildContext context) => this;

    public virtual bool isConcrete =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (size is not null)
                && (fill is not null)
                && (weight is not null)
                && (grade is not null)
                && (opticalSize is not null)
                && (color is not null)
                && (opacity is not null)
                && (applyTextScaling is not null)
        );
    public virtual double? opacity =>
        (_opacity is null) ? null : Dart_uiLibrary.clampDouble(_opacity, 0.0, 1.0);

    public static IconThemeData lerp(IconThemeData? a, IconThemeData? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new IconThemeData(
            size: Dart_uiLibrary.lerpDouble(a?.size, b?.size, t),
            fill: Dart_uiLibrary.lerpDouble(a?.fill, b?.fill, t),
            weight: Dart_uiLibrary.lerpDouble(a?.weight, b?.weight, t),
            grade: Dart_uiLibrary.lerpDouble(a?.grade, b?.grade, t),
            opticalSize: Dart_uiLibrary.lerpDouble(a?.opticalSize, b?.opticalSize, t),
            color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t),
            opacity: Dart_uiLibrary.lerpDouble(a?.opacity, b?.opacity, t),
            shadows: Dart_uiLibrary.Shadow.lerpList(a?.shadows, b?.shadows, t),
            applyTextScaling: (t < 0.5) ? a?.applyTextScaling : b?.applyTextScaling
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as IconThemeData;
        if (__other is null)
        {
            return false;
        }

        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is IconThemeData)
            && (__other.size == size)
            && (__other.fill == fill)
            && (__other.weight == weight)
            && (__other.grade == grade)
            && (__other.opticalSize == opticalSize)
            && Equals(__other.color, color)
            && (__other.opacity == opacity)
            && CollectionsLibrary.listEquals(__other.shadows, shadows)
            && (__other.applyTextScaling == applyTextScaling);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                size,
                fill,
                weight,
                grade,
                opticalSize,
                color,
                opacity,
                (shadows is null) ? null : FoundationRuntimePorts.ObjectHashAll(shadows!),
                applyTextScaling
            )
        );

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        properties.add(new DoubleProperty("size", size, defaultValue: null));
        properties.add(new DoubleProperty("fill", fill, defaultValue: null));
        properties.add(new DoubleProperty("weight", weight, defaultValue: null));
        properties.add(new DoubleProperty("grade", grade, defaultValue: null));
        properties.add(new DoubleProperty("opticalSize", opticalSize, defaultValue: null));
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new DoubleProperty("opacity", opacity, defaultValue: null));
        properties.add(new IterableProperty<Shadow>("shadows", shadows, defaultValue: null));
        properties.add(
            new DiagnosticsProperty<bool>("applyTextScaling", applyTextScaling, defaultValue: null)
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
            throw new InvalidOperationException("Callback completed without returning a value.");
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
