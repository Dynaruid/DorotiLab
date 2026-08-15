// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/icon_theme_data.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class IconThemeData : global::Doroti.Generated.Framework.Foundation.Diagnosticable
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

    public IconThemeData(double? size = null, double? fill = null, double? weight = null, double? grade = null, double? opticalSize = null, Color? color = null, double? opacity = null, List<Shadow>? shadows = null, bool? applyTextScaling = null)
    {
        this.size = size;
        this.fill = fill;
        this.weight = weight;
        this.grade = grade;
        this.opticalSize = opticalSize;
        this.color = color;
        this.shadows = shadows;
        this.applyTextScaling = applyTextScaling;
        this._opacity = opacity;
        System.Diagnostics.Debug.Assert(((fill is null) || (((0.0 <= DartRuntimePrimitives.RequireValue(fill)) && (fill <= 1.0)))));
        System.Diagnostics.Debug.Assert(((weight is null) || ((0.0 < DartRuntimePrimitives.RequireValue(weight)))));
        System.Diagnostics.Debug.Assert(((opticalSize is null) || ((0.0 < DartRuntimePrimitives.RequireValue(opticalSize)))));
    }

    public static IconThemeData CreateFallback()
    {
        var __instance = new IconThemeData(default!, default!, default!, default!, default!, default!, default!, default!, default!);
        __instance.size = 24.0;
        __instance.fill = 0.0;
        __instance.weight = 400.0;
        __instance.grade = 0.0;
        __instance.opticalSize = 48.0;
        __instance.color = new global::Doroti.Ui.Color(4278190080L);
        __instance._opacity = 1.0;
        __instance.shadows = null;
        __instance.applyTextScaling = false;
        return __instance;
    }

    public virtual IconThemeData copyWith(double? size = null, double? fill = null, double? weight = null, double? grade = null, double? opticalSize = null, Color? color = null, double? opacity = null, List<Shadow>? shadows = null, bool? applyTextScaling = null)
    {
        return new IconThemeData(size: (size ?? this.size), fill: (fill ?? this.fill), weight: (weight ?? this.weight), grade: (grade ?? this.grade), opticalSize: (opticalSize ?? this.opticalSize), color: (color ?? this.color), opacity: (opacity ?? this.opacity), shadows: (shadows ?? this.shadows), applyTextScaling: (applyTextScaling ?? this.applyTextScaling));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IconThemeData merge(IconThemeData? other)
    {
        if ((other is null))
        {
            return this;
        }
        return ((IconThemeData)(object?)copyWith(size: ((IconThemeData)other).size, fill: ((IconThemeData)other).fill, weight: ((IconThemeData)other).weight, grade: ((IconThemeData)other).grade, opticalSize: ((IconThemeData)other).opticalSize, color: ((IconThemeData)other).color, opacity: ((IconThemeData)other).opacity, shadows: ((IconThemeData)other).shadows, applyTextScaling: ((IconThemeData)other).applyTextScaling));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual IconThemeData resolve(BuildContext context) => this;
    public virtual bool isConcrete => DartRuntimePrimitives.ConvertValue<bool>(((((((((this.size is not null) && (this.fill is not null)) && (this.weight is not null)) && (this.grade is not null)) && (this.opticalSize is not null)) && (this.color is not null)) && (this.opacity is not null)) && (this.applyTextScaling is not null)));
    public virtual double? opacity => ((this._opacity is null) ? null : Dart_uiLibrary.clampDouble(this._opacity, 0.0, 1.0));
    public static IconThemeData lerp(IconThemeData? a, IconThemeData? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new IconThemeData(size: Dart_uiLibrary.lerpDouble(a?.size, b?.size, t), fill: Dart_uiLibrary.lerpDouble(a?.fill, b?.fill, t), weight: Dart_uiLibrary.lerpDouble(a?.weight, b?.weight, t), grade: Dart_uiLibrary.lerpDouble(a?.grade, b?.grade, t), opticalSize: Dart_uiLibrary.lerpDouble(a?.opticalSize, b?.opticalSize, t), color: Dart_uiLibrary.Color.lerp(a?.color, b?.color, t), opacity: Dart_uiLibrary.lerpDouble(a?.opacity, b?.opacity, t), shadows: Dart_uiLibrary.Shadow.lerpList(a?.shadows, b?.shadows, t), applyTextScaling: ((t < 0.5) ? a?.applyTextScaling : b?.applyTextScaling));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as IconThemeData;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((__other is IconThemeData) && (((IconThemeData)((IconThemeData)__other)).size == this.size)) && (((IconThemeData)((IconThemeData)__other)).fill == this.fill)) && (((IconThemeData)((IconThemeData)__other)).weight == this.weight)) && (((IconThemeData)((IconThemeData)__other)).grade == this.grade)) && (((IconThemeData)((IconThemeData)__other)).opticalSize == this.opticalSize)) && (object.Equals(((IconThemeData)((IconThemeData)__other)).color, this.color))) && (((IconThemeData)((IconThemeData)__other)).opacity == this.opacity)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((IconThemeData)((IconThemeData)__other)).shadows, this.shadows)) && (((IconThemeData)((IconThemeData)__other)).applyTextScaling == this.applyTextScaling));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.size, this.fill, this.weight, this.grade, this.opticalSize, this.color, this.opacity, ((this.shadows is null) ? null : FoundationRuntimePorts.ObjectHashAll(this.shadows!)), this.applyTextScaling));
    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("size", this.size, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("fill", this.fill, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("weight", this.weight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("grade", this.grade, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("opticalSize", this.opticalSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("opacity", this.opacity, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IterableProperty<global::Doroti.Ui.Shadow>("shadows", this.shadows.Cast<global::Doroti.Ui.Shadow>(), defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("applyTextScaling", this.applyTextScaling, defaultValue: null));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString__105654 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                fullString__105654 = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toDiagnosticsNode().toStringDeep(minLevel: minLevel);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
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

