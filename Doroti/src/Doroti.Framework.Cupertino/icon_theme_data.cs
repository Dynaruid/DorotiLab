// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/icon_theme_data.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Cupertino;

public class CupertinoIconThemeData : global::Doroti.Framework.Widgets.IconThemeData, global::Doroti.Framework.Foundation.Diagnosticable
{

    public CupertinoIconThemeData(double? size = null, double? fill = null, double? weight = null, double? grade = null, double? opticalSize = null, Color? color = null, double? opacity = null, List<Shadow>? shadows = null, bool? applyTextScaling = null) : base(size: size, fill: fill, weight: weight, grade: grade, opticalSize: opticalSize, color: color, opacity: opacity, shadows: shadows, applyTextScaling: applyTextScaling)
    {
    }

    public override global::Doroti.Framework.Widgets.IconThemeData resolve(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? resolvedColor__871 = ((global::Doroti.Ui.Color?)(object?)CupertinoDynamicColor.maybeResolve(this.color, context));
        return ((global::Doroti.Framework.Widgets.IconThemeData)(object?)((object.Equals(resolvedColor__871, this.color)) ? this : copyWith(color: resolvedColor__871)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override CupertinoIconThemeData copyWith(double? size = null, double? fill = null, double? weight = null, double? grade = null, double? opticalSize = null, Color? color = null, double? opacity = null, List<Shadow>? shadows = null, bool? applyTextScaling = null)
    {
        return new CupertinoIconThemeData(size: (size ?? this.size), fill: (fill ?? this.fill), weight: (weight ?? this.weight), grade: (grade ?? this.grade), opticalSize: (opticalSize ?? this.opticalSize), color: (color ?? this.color), opacity: (opacity ?? this.opacity), shadows: (shadows ?? this.shadows), applyTextScaling: (applyTextScaling ?? this.applyTextScaling));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(ColorsLibrary.createCupertinoColorProperty("color", this.color, defaultValue: null));
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
