// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/icon.dart
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

namespace Doroti.Framework.Widgets;

public class Icon : StatelessWidget
{
    public virtual IconData? icon { get; private set; }
    public virtual double? size { get; private set; }
    public virtual double? fill { get; private set; }
    public virtual double? weight { get; private set; }
    public virtual double? grade { get; private set; }
    public virtual double? opticalSize { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual List<Shadow>? shadows { get; private set; }
    public virtual string? semanticLabel { get; private set; }
    public virtual TextDirection? textDirection { get; private set; }
    public virtual bool? applyTextScaling { get; private set; }
    public virtual BlendMode? blendMode { get; private set; }
    public virtual FontWeight? fontWeight { get; private set; }

    public Icon(IconData? icon, global::Doroti.Framework.Foundation.Key? key = null, double? size = null, double? fill = null, double? weight = null, double? grade = null, double? opticalSize = null, Color? color = null, List<Shadow>? shadows = null, string? semanticLabel = null, TextDirection? textDirection = null, bool? applyTextScaling = null, BlendMode? blendMode = null, FontWeight? fontWeight = null) : base(key: key)
    {
        this.icon = icon;
        this.size = size;
        this.fill = fill;
        this.weight = weight;
        this.grade = grade;
        this.opticalSize = opticalSize;
        this.color = color;
        this.shadows = shadows;
        this.semanticLabel = semanticLabel;
        this.textDirection = textDirection;
        this.applyTextScaling = applyTextScaling;
        this.blendMode = blendMode;
        this.fontWeight = fontWeight;
        System.Diagnostics.Debug.Assert(((fill is null) || (((0.0 <= DartRuntimePrimitives.RequireValue(fill)) && (fill <= 1.0)))));
        System.Diagnostics.Debug.Assert(((weight is null) || ((0.0 < DartRuntimePrimitives.RequireValue(weight)))));
        System.Diagnostics.Debug.Assert(((opticalSize is null) || ((0.0 < DartRuntimePrimitives.RequireValue(opticalSize)))));
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ((((TextDirection?)((dynamic)this).textDirection) is not null) || global::Doroti.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context)));
        global::Doroti.Ui.TextDirection textDirectionLocal = ((((TextDirection?)((dynamic)this).textDirection) ?? (TextDirection)Directionality.of(context)));
        IconThemeData iconTheme = ((IconThemeData)(object?)IconTheme.of(context));
        bool applyTextScalingLocal = ((this.applyTextScaling ?? ((IconThemeData)iconTheme).applyTextScaling) ?? false);
        double tentativeIconSize = ((this.size ?? ((IconThemeData)iconTheme).size) ?? global::Doroti.Framework.Painting.Text_painterLibrary.kDefaultFontSize);
        double iconSize = (DartRuntimePrimitives.RequireValue(applyTextScalingLocal) ? MediaQuery.textScalerOf(context).scale(tentativeIconSize) : tentativeIconSize);
        double? iconFill = (this.fill ?? ((IconThemeData)iconTheme).fill);
        double? iconWeight = (this.weight ?? ((IconThemeData)iconTheme).weight);
        double? iconGrade = (this.grade ?? ((IconThemeData)iconTheme).grade);
        double? iconOpticalSize = (this.opticalSize ?? ((IconThemeData)iconTheme).opticalSize);
        List<global::Doroti.Ui.Shadow>? iconShadows = (this.shadows ?? ((IconThemeData)iconTheme).shadows);
        IconData? iconLocal = this.icon;
        if ((iconLocal is null))
        {
            return ((Widget)(object?)new Semantics(label: this.semanticLabel, child: new SizedBox(width: iconSize, height: iconSize)));
        }
        double iconOpacity = (((IconThemeData)iconTheme).opacity ?? 1.0);
        global::Doroti.Ui.Color? iconColor = ((global::Doroti.Ui.Color?)(object?)(this.color ?? ((IconThemeData)iconTheme).color!));
        global::Doroti.Ui.Paint? foregroundLocal = default!;
        if ((iconOpacity != 1.0))
        {
            iconColor = iconColor.withOpacity((iconColor.opacity * iconOpacity));
        }
        if ((this.blendMode is not null))
        {
            foregroundLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.blendMode = DartRuntimePrimitives.RequireValue(this.blendMode);
    __cascade.color = iconColor;
    return __cascade;
}))();
            iconColor = DartRuntimePrimitives.ConvertValue<Color>(null);
        }
        var fontStyle = new global::Doroti.Framework.Painting.TextStyle(fontVariations: new List<global::Doroti.Ui.FontVariation>(), inherit: false, color: iconColor, fontSize: iconSize, fontFamily: ((IconData)iconLocal).fontFamily, fontWeight: this.fontWeight, package: ((IconData)iconLocal).fontPackage, fontFamilyFallback: ((IconData)iconLocal).fontFamilyFallback, shadows: iconShadows, height: 1.0, leadingDistribution: TextLeadingDistribution.even, foreground: foregroundLocal);
        Widget iconWidget = ((Widget)(object?)new RichText(overflow: global::Doroti.Framework.Painting.TextOverflow.visible, textDirection: DartRuntimePrimitives.RequireValue(textDirectionLocal), text: new global::Doroti.Framework.Painting.TextSpan(text: char.ConvertFromUtf32(checked((int)((IconData)iconLocal).codePoint)), style: fontStyle)));
        if (((IconData)iconLocal).matchTextDirection)
        {
            switch (DartRuntimePrimitives.RequireValue(textDirectionLocal))
            {
                case TextDirection.rtl:
                    {
                        iconWidget = DartRuntimePrimitives.ConvertValue<Widget>(new Transform(transform: ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.scaleByDouble(-1.0, 1.0, 1.0, 1);
    return __cascade;
}))(), alignment: global::Doroti.Framework.Painting.Alignment.center, transformHitTests: false, child: iconWidget));
                        break;
                    }
                case TextDirection.ltr:
                    {
                        break;
                    }
            }
        }
        return ((Widget)(object?)new Semantics(label: this.semanticLabel, child: new ExcludeSemantics(child: new SizedBox(width: iconSize, height: iconSize, child: new Center(child: iconWidget)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IconDataProperty("icon", this.icon, ifNull: "<empty>", showName: false));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("size", this.size, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("fill", this.fill, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("weight", this.weight, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("grade", this.grade, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DoubleProperty("opticalSize", this.opticalSize, defaultValue: null));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.IterableProperty<global::Doroti.Ui.Shadow>("shadows", this.shadows.Cast<global::Doroti.Ui.Shadow>(), defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.StringProperty("semanticLabel", this.semanticLabel, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<bool>("applyTextScaling", this.applyTextScaling, defaultValue: null));
    }

}
