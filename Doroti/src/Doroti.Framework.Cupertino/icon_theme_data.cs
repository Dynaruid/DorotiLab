// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/icon_theme_data.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoIconThemeData : global::Doroti.Framework.Widgets.IconThemeData, global::Doroti.Framework.Foundation.Diagnosticable
{

    public CupertinoIconThemeData(double? size = null, double? fill = null, double? weight = null, double? grade = null, double? opticalSize = null, Color? color = null, double? opacity = null, List<Shadow>? shadows = null, bool? applyTextScaling = null) : base(size: size, fill: fill, weight: weight, grade: grade, opticalSize: opticalSize, color: color, opacity: opacity, shadows: shadows, applyTextScaling: applyTextScaling)
    {
    }

    public override global::Doroti.Framework.Widgets.IconThemeData resolve(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color? resolvedColor = CupertinoDynamicColor.maybeResolve(color, context);
        return Equals(resolvedColor, color) ? this : copyWith(color: resolvedColor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override CupertinoIconThemeData copyWith(double? size = null, double? fill = null, double? weight = null, double? grade = null, double? opticalSize = null, Color? color = null, double? opacity = null, List<Shadow>? shadows = null, bool? applyTextScaling = null)
    {
        return new CupertinoIconThemeData(size: size ?? this.size, fill: fill ?? this.fill, weight: weight ?? this.weight, grade: grade ?? this.grade, opticalSize: opticalSize ?? this.opticalSize, color: color ?? this.color, opacity: opacity ?? this.opacity, shadows: shadows ?? this.shadows, applyTextScaling: applyTextScaling ?? this.applyTextScaling);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        properties.add(ColorsLibrary.createCupertinoColorProperty("color", color, defaultValue: null));
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
