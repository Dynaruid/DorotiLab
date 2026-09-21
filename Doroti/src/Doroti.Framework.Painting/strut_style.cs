// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/strut_style.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public class StrutStyle : Diagnosticable
{
    public static StrutStyle disabled = new StrutStyle(height: 0.0, leading: 0.0);
    public virtual string? fontFamily { get; private set; }
    internal virtual List<string>? _fontFamilyFallback { get; private set; }
    internal virtual string? _package { get; private set; }
    public virtual double? fontSize { get; private set; }
    public virtual double? height { get; private set; }
    public virtual TextLeadingDistribution? leadingDistribution { get; private set; }
    public virtual FontWeight? fontWeight { get; private set; }
    public virtual FontStyle? fontStyle { get; private set; }
    public virtual double? leading { get; private set; }
    public virtual bool? forceStrutHeight { get; private set; }
    public virtual string? debugLabel { get; private set; }

    public StrutStyle(
        string? fontFamily = null,
        List<string>? fontFamilyFallback = null,
        double? fontSize = null,
        double? height = null,
        TextLeadingDistribution? leadingDistribution = null,
        double? leading = null,
        FontWeight? fontWeight = null,
        FontStyle? fontStyle = null,
        bool? forceStrutHeight = null,
        string? debugLabel = null,
        string? package = null
    )
    {
        this.fontSize = fontSize;
        this.height = height;
        this.leadingDistribution = leadingDistribution;
        this.leading = leading;
        this.fontWeight = fontWeight;
        this.fontStyle = fontStyle;
        this.forceStrutHeight = forceStrutHeight;
        this.debugLabel = debugLabel;
        this.fontFamily = (package is null) ? fontFamily : $"packages/{package}/{fontFamily}";
        _fontFamilyFallback = fontFamilyFallback;
        _package = package;
        System.Diagnostics.Debug.Assert(
            (fontSize is null) || (DartRuntimePrimitives.RequireValue(fontSize) > 0L)
        );
        System.Diagnostics.Debug.Assert((leading is null) || (leading >= 0L));
        System.Diagnostics.Debug.Assert(
            (package is null) || (fontFamily is not null) || (fontFamilyFallback is not null)
        );
    }

    public static StrutStyle CreateFromTextStyle(
        TextStyle textStyle,
        string? fontFamily = null,
        List<string>? fontFamilyFallback = null,
        double? fontSize = null,
        double? height = null,
        TextLeadingDistribution? leadingDistribution = null,
        double? leading = null,
        FontWeight? fontWeight = null,
        FontStyle? fontStyle = null,
        bool? forceStrutHeight = null,
        string? debugLabel = null,
        string? package = null
    )
    {
        return new StrutStyle(
            fontFamily: (fontFamily is not null)
                ? ((package is null) ? fontFamily : $"packages/{package}/{fontFamily}")
                : textStyle.fontFamily,
            fontFamilyFallback: fontFamilyFallback ?? textStyle.fontFamilyFallback,
            height: height ?? textStyle.height,
            leadingDistribution: leadingDistribution ?? textStyle.leadingDistribution,
            fontSize: fontSize ?? textStyle.fontSize,
            leading: leading,
            fontWeight: fontWeight ?? textStyle.fontWeight,
            fontStyle: fontStyle ?? textStyle.fontStyle,
            forceStrutHeight: forceStrutHeight,
            debugLabel: debugLabel ?? textStyle.debugLabel,
            package: package
        );
    }

    public virtual List<string>? fontFamilyFallback
    {
        get
        {
            if ((_package is not null) && (_fontFamilyFallback is not null))
            {
                return _fontFamilyFallback
                    .map((family) => $"packages/{_package}/{family}")
                    .ToList();
            }
            return _fontFamilyFallback;
        }
    }

    public virtual RenderComparison compareTo(StrutStyle other)
    {
        if (DartRuntimePrimitives.Identical(this, other))
        {
            return RenderComparison.identical;
        }
        if (
            (fontFamily != other.fontFamily)
            || (fontSize != other.fontSize)
            || (!Equals(fontWeight, other.fontWeight))
            || (!Equals(fontStyle, other.fontStyle))
            || (height != other.height)
            || (leading != other.leading)
            || (forceStrutHeight != other.forceStrutHeight)
            || (!CollectionsLibrary.listEquals(fontFamilyFallback, other.fontFamilyFallback))
            || ((height is not null) && (!Equals(leadingDistribution, other.leadingDistribution)))
        )
        {
            return RenderComparison.layout;
        }
        return RenderComparison.identical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual StrutStyle inheritFromTextStyle(TextStyle? other)
    {
        if (other is null)
        {
            return this;
        }
        double? effectiveHeight = height ?? other.height;
        return new StrutStyle(
            fontFamily: fontFamily ?? other.fontFamily,
            fontFamilyFallback: fontFamilyFallback ?? other.fontFamilyFallback,
            fontSize: fontSize ?? other.fontSize,
            height: effectiveHeight,
            leading: leading,
            fontWeight: fontWeight ?? other.fontWeight,
            fontStyle: fontStyle ?? other.fontStyle,
            forceStrutHeight: forceStrutHeight,
            debugLabel: debugLabel ?? other.debugLabel,
            leadingDistribution: (effectiveHeight is not null)
                ? (leadingDistribution ?? other.leadingDistribution)
                : null
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual StrutStyle merge(StrutStyle? other)
    {
        if (other is null)
        {
            return this;
        }
        return new StrutStyle(
            fontFamily: other.fontFamily ?? fontFamily,
            fontFamilyFallback: other.fontFamilyFallback ?? fontFamilyFallback,
            fontSize: other.fontSize ?? fontSize,
            height: other.height ?? height,
            leadingDistribution: other.leadingDistribution ?? leadingDistribution,
            leading: other.leading ?? leading,
            fontWeight: other.fontWeight ?? fontWeight,
            fontStyle: other.fontStyle ?? fontStyle,
            forceStrutHeight: other.forceStrutHeight ?? forceStrutHeight,
            debugLabel: other.debugLabel ?? debugLabel,
            package: other._package ?? _package
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as StrutStyle;
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
        return (__other is StrutStyle)
            && (__other.fontFamily == fontFamily)
            && (__other.fontSize == fontSize)
            && Equals(__other.fontWeight, fontWeight)
            && Equals(__other.fontStyle, fontStyle)
            && (__other.height == height)
            && (__other.leading == leading)
            && (__other.forceStrutHeight == forceStrutHeight)
            && ((height is null) || Equals(leadingDistribution, __other.leadingDistribution))
            && CollectionsLibrary.listEquals(__other.fontFamilyFallback, fontFamilyFallback);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(
            fontFamily,
            fontSize,
            fontWeight,
            fontStyle,
            height,
            leading,
            forceStrutHeight
        );

    public virtual string toStringShort() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "StrutStyle");

    public virtual void debugFillProperties(
        DiagnosticPropertiesBuilder properties,
        string prefix = ""
    )
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if (debugLabel is not null)
        {
            properties.add(new MessageProperty($"{prefix}debugLabel", debugLabel!));
        }
        var styles = new List<DiagnosticsNode>
        {
            new StringProperty($"{prefix}family", fontFamily, defaultValue: null, quoted: false),
            new IterableProperty<string>(
                $"{prefix}familyFallback",
                fontFamilyFallback,
                defaultValue: null
            ),
            new DoubleProperty($"{prefix}size", fontSize, defaultValue: null),
        };
        string? weightDescription = default!;
        if (fontWeight is not null)
        {
            weightDescription = $"w{FoundationRuntimePorts.EnumIndex(fontWeight!) + 1L}00";
        }
        styles.Add(
            new DiagnosticsProperty<FontWeight>(
                $"{prefix}weight",
                fontWeight,
                description: weightDescription,
                defaultValue: null
            )
        );
        styles.Add(new EnumProperty<FontStyle>($"{prefix}style", fontStyle, defaultValue: null));
        styles.Add(new DoubleProperty($"{prefix}height", height, unit: "x", defaultValue: null));
        styles.Add(
            new FlagProperty(
                $"{prefix}forceStrutHeight",
                value: forceStrutHeight,
                ifTrue: $"{prefix}<strut height forced>",
                ifFalse: $"{prefix}<strut height normal>"
            )
        );
        if (height is not null)
        {
            double height__value26382 = DartRuntimePrimitives.RequireValue(height);
            styles.Add(
                new EnumProperty<TextLeadingDistribution>(
                    $"{prefix}leadingDistribution",
                    leadingDistribution,
                    defaultValue: null
                )
            );
        }
        bool styleSpecified = styles.any((n) => !n.isFiltered(DiagnosticLevel.info));
        styles.forEach(properties.add);
        if (!styleSpecified)
        {
            properties.add(
                new FlagProperty(
                    "forceStrutHeight",
                    value: forceStrutHeight,
                    ifTrue: $"{prefix}<strut height forced>",
                    ifFalse: $"{prefix}<strut height normal>"
                )
            );
        }
    }
}
