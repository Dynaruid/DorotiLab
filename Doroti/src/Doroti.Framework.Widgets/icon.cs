// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/icon.dart
using Doroti.Runtime;
using Doroti.Ui;

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

    public Icon(
        IconData? icon,
        Key? key = null,
        double? size = null,
        double? fill = null,
        double? weight = null,
        double? grade = null,
        double? opticalSize = null,
        Color? color = null,
        List<Shadow>? shadows = null,
        string? semanticLabel = null,
        TextDirection? textDirection = null,
        bool? applyTextScaling = null,
        BlendMode? blendMode = null,
        FontWeight? fontWeight = null
    )
        : base(key: key)
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
        System.Diagnostics.Debug.Assert(
            (fill is null) || ((0.0 <= DartRuntimePrimitives.RequireValue(fill)) && (fill <= 1.0))
        );
        System.Diagnostics.Debug.Assert(
            (weight is null) || 0.0 < DartRuntimePrimitives.RequireValue(weight)
        );
        System.Diagnostics.Debug.Assert(
            (opticalSize is null) || 0.0 < DartRuntimePrimitives.RequireValue(opticalSize)
        );
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            (textDirection is not null) || DebugLibrary.debugCheckHasDirectionality(context)
        );
        TextDirection textDirectionLocal = textDirection ?? Directionality.of(context);
        IconThemeData iconTheme = IconTheme.of(context);
        bool applyTextScalingLocal = (applyTextScaling ?? iconTheme.applyTextScaling) ?? false;
        double tentativeIconSize = (size ?? iconTheme.size) ?? Text_painterLibrary.kDefaultFontSize;
        double iconSize = DartRuntimePrimitives.RequireValue(applyTextScalingLocal)
            ? MediaQuery.textScalerOf(context).scale(tentativeIconSize)
            : tentativeIconSize;
        double? iconFill = fill ?? iconTheme.fill;
        double? iconWeight = weight ?? iconTheme.weight;
        double? iconGrade = grade ?? iconTheme.grade;
        double? iconOpticalSize = opticalSize ?? iconTheme.opticalSize;
        List<Shadow>? iconShadows = shadows ?? iconTheme.shadows;
        IconData? iconLocal = icon;
        if (iconLocal is null)
        {
            return new Semantics(
                label: semanticLabel,
                child: new SizedBox(width: iconSize, height: iconSize)
            );
        }
        double iconOpacity = iconTheme.opacity ?? 1.0;
        Color? iconColor = DartRuntimePrimitives.RequireReference(color ?? iconTheme.color);
        Paint? foregroundLocal = default!;
        if (iconOpacity != 1.0)
        {
            iconColor = iconColor.withOpacity(iconColor.opacity * iconOpacity);
        }
        if (blendMode is not null)
        {
            foregroundLocal = (
                (Func<Paint>)(
                    () =>
                    {
                        var __cascade = new Paint();
                        __cascade.blendMode = DartRuntimePrimitives.RequireValue(blendMode);
                        __cascade.color = iconColor;
                        return __cascade;
                    }
                )
            )();
            iconColor = null;
        }
        var fontStyle = new TextStyle(
            fontVariations: new List<FontVariation>(),
            inherit: false,
            color: iconColor,
            fontSize: iconSize,
            fontFamily: iconLocal.fontFamily,
            fontWeight: fontWeight,
            package: iconLocal.fontPackage,
            fontFamilyFallback: iconLocal.fontFamilyFallback,
            shadows: iconShadows,
            height: 1.0,
            leadingDistribution: TextLeadingDistribution.even,
            foreground: foregroundLocal
        );
        Widget iconWidget = new RichText(
            overflow: TextOverflow.visible,
            textDirection: DartRuntimePrimitives.RequireValue(textDirectionLocal),
            text: new TextSpan(
                text: char.ConvertFromUtf32(checked((int)iconLocal.codePoint)),
                style: fontStyle
            )
        );
        if (iconLocal.matchTextDirection)
        {
            switch (DartRuntimePrimitives.RequireValue(textDirectionLocal))
            {
                case TextDirection.rtl:
                {
                    iconWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                        new Transform(
                            transform: (
                                (Func<Matrix4>)(
                                    () =>
                                    {
                                        var __cascade = Matrix4.identity();
                                        __cascade.scaleByDouble(-1.0, 1.0, 1.0, 1);
                                        return __cascade;
                                    }
                                )
                            )(),
                            alignment: Alignment.center,
                            transformHitTests: false,
                            child: iconWidget
                        )
                    );
                    break;
                }
                case TextDirection.ltr:
                {
                    break;
                }
            }
        }
        return new Semantics(
            label: semanticLabel,
            child: new ExcludeSemantics(
                child: new SizedBox(
                    width: iconSize,
                    height: iconSize,
                    child: new Center(child: iconWidget)
                )
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new IconDataProperty("icon", icon, ifNull: "<empty>", showName: false));
        properties.add(new DoubleProperty("size", size, defaultValue: null));
        properties.add(new DoubleProperty("fill", fill, defaultValue: null));
        properties.add(new DoubleProperty("weight", weight, defaultValue: null));
        properties.add(new DoubleProperty("grade", grade, defaultValue: null));
        properties.add(new DoubleProperty("opticalSize", opticalSize, defaultValue: null));
        properties.add(new ColorProperty("color", color, defaultValue: null));
        properties.add(new IterableProperty<Shadow>("shadows", shadows, defaultValue: null));
        properties.add(new StringProperty("semanticLabel", semanticLabel, defaultValue: null));
        properties.add(
            new EnumProperty<TextDirection>("textDirection", textDirection, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<bool>("applyTextScaling", applyTextScaling, defaultValue: null)
        );
    }
}
