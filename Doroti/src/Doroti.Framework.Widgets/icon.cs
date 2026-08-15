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

namespace Doroti.Generated.Framework.Widgets;

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

    public Icon(IconData? icon, global::Doroti.Generated.Framework.Foundation.Key? key = null, double? size = null, double? fill = null, double? weight = null, double? grade = null, double? opticalSize = null, Color? color = null, List<Shadow>? shadows = null, string? semanticLabel = null, TextDirection? textDirection = null, bool? applyTextScaling = null, BlendMode? blendMode = null, FontWeight? fontWeight = null) : base(key: key)
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
        DartRuntimePrimitives.Assert(() => ((((TextDirection?)((dynamic)this).textDirection) is not null) || global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context)));
        global::Doroti.Ui.TextDirection textDirection__9567 = ((((TextDirection?)((dynamic)this).textDirection) ?? (TextDirection)Directionality.of(context)));
        IconThemeData iconTheme__9658 = ((IconThemeData)(object?)IconTheme.of(context));
        bool applyTextScaling__9709 = ((this.applyTextScaling ?? ((IconThemeData)iconTheme__9658).applyTextScaling) ?? false);
        double tentativeIconSize__9808 = ((this.size ?? ((IconThemeData)iconTheme__9658).size) ?? global::Doroti.Generated.Framework.Painting.Text_painterLibrary.kDefaultFontSize);
        double iconSize__9890 = (DartRuntimePrimitives.RequireValue(applyTextScaling__9709) ? MediaQuery.textScalerOf(context).scale(tentativeIconSize__9808) : tentativeIconSize__9808);
        double? iconFill__10034 = (this.fill ?? ((IconThemeData)iconTheme__9658).fill);
        double? iconWeight__10088 = (this.weight ?? ((IconThemeData)iconTheme__9658).weight);
        double? iconGrade__10148 = (this.grade ?? ((IconThemeData)iconTheme__9658).grade);
        double? iconOpticalSize__10205 = (this.opticalSize ?? ((IconThemeData)iconTheme__9658).opticalSize);
        List<global::Doroti.Ui.Shadow>? iconShadows__10286 = (this.shadows ?? ((IconThemeData)iconTheme__9658).shadows);
        IconData? icon__10351 = this.icon;
        if ((icon__10351 is null))
        {
            return ((Widget)(object?)new Semantics(label: this.semanticLabel, child: new SizedBox(width: iconSize__9890, height: iconSize__9890)));
        }
        double iconOpacity__10540 = (((IconThemeData)iconTheme__9658).opacity ?? 1.0);
        global::Doroti.Ui.Color? iconColor__10591 = ((global::Doroti.Ui.Color?)(object?)(this.color ?? ((IconThemeData)iconTheme__9658).color!));
        global::Doroti.Ui.Paint? foreground__10641 = default!;
        if ((iconOpacity__10540 != 1.0))
        {
            iconColor__10591 = iconColor__10591.withOpacity((iconColor__10591.opacity * iconOpacity__10540));
        }
        if ((this.blendMode is not null))
        {
            foreground__10641 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Ui.Paint();
            __cascade.blendMode = DartRuntimePrimitives.RequireValue(this.blendMode);
            __cascade.color = iconColor__10591;
            return __cascade;        }))();
            iconColor__10591 = DartRuntimePrimitives.ConvertValue<Color>(null);
        }
        var fontStyle__10977 = new global::Doroti.Generated.Framework.Painting.TextStyle(fontVariations: new List<global::Doroti.Ui.FontVariation>(), inherit: false, color: iconColor__10591, fontSize: iconSize__9890, fontFamily: ((IconData)icon__10351).fontFamily, fontWeight: this.fontWeight, package: ((IconData)icon__10351).fontPackage, fontFamilyFallback: ((IconData)icon__10351).fontFamilyFallback, shadows: iconShadows__10286, height: 1.0, leadingDistribution: TextLeadingDistribution.even, foreground: foreground__10641);
        Widget iconWidget__11796 = ((Widget)(object?)new RichText(overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.visible, textDirection: DartRuntimePrimitives.RequireValue(textDirection__9567), text: new global::Doroti.Generated.Framework.Painting.TextSpan(text: char.ConvertFromUtf32(checked((int)((IconData)icon__10351).codePoint)), style: fontStyle__10977)));
        if (((IconData)icon__10351).matchTextDirection)
        {
            switch (DartRuntimePrimitives.RequireValue(textDirection__9567))
            {
                case TextDirection.rtl:
                    {
                        iconWidget__11796 = DartRuntimePrimitives.ConvertValue<Widget>(new Transform(transform: ((Func<Matrix4>)(() =>
{            var __cascade = Matrix4.identity();
            __cascade.scaleByDouble(-1.0, 1.0, 1.0, 1);
            return __cascade;        }))(), alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, transformHitTests: false, child: iconWidget__11796));
                        break;
                    }
                case TextDirection.ltr:
                    {
                        break;
                    }
            }
        }
        return ((Widget)(object?)new Semantics(label: this.semanticLabel, child: new ExcludeSemantics(child: new SizedBox(width: iconSize__9890, height: iconSize__9890, child: new Center(child: iconWidget__11796)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IconDataProperty("icon", this.icon, ifNull: "<empty>", showName: false));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("size", this.size, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("fill", this.fill, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("weight", this.weight, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("grade", this.grade, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DoubleProperty("opticalSize", this.opticalSize, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Painting.ColorProperty("color", this.color, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.IterableProperty<global::Doroti.Ui.Shadow>("shadows", this.shadows.Cast<global::Doroti.Ui.Shadow>(), defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.StringProperty("semanticLabel", this.semanticLabel, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Ui.TextDirection>("textDirection", this.textDirection, defaultValue: null));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<bool>("applyTextScaling", this.applyTextScaling, defaultValue: null));
    }

}
