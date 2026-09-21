// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_style.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public static partial class Text_styleLibrary
{
    internal static string _kDefaultDebugLabel = "unknown";
}

public static partial class Text_styleLibrary
{
    internal static string _kColorForegroundWarning =
        "Cannot provide both a color and a foreground\n"
        + "The color argument is just a shorthand for \"foreground: Paint()..color = color\".";
}

public static partial class Text_styleLibrary
{
    internal static string _kColorBackgroundWarning =
        "Cannot provide both a backgroundColor and a background\n"
        + "The backgroundColor argument is just a shorthand for \"background: Paint()..color = color\".";
}

public static partial class Text_styleLibrary
{
    internal static string _kTextStyleHeightNaNWarning = "TextStyle.height must not be NaN.";
}

public class TextStyle : Diagnosticable
{
    public virtual bool inherit { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual string? fontFamily { get; private set; }
    internal virtual List<string>? _fontFamilyFallback { get; private set; }
    internal virtual string? _package { get; private set; }
    public virtual double? fontSize { get; private set; }
    public virtual FontWeight? fontWeight { get; private set; }
    public virtual FontStyle? fontStyle { get; private set; }
    public virtual double? letterSpacing { get; private set; }
    public virtual double? wordSpacing { get; private set; }
    public virtual TextBaseline? textBaseline { get; private set; }
    public virtual double? height { get; private set; }
    public virtual TextLeadingDistribution? leadingDistribution { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual Paint? foreground { get; private set; }
    public virtual Paint? background { get; private set; }
    public virtual TextDecoration? decoration { get; private set; }
    public virtual Color? decorationColor { get; private set; }
    public virtual TextDecorationStyle? decorationStyle { get; private set; }
    public virtual double? decorationThickness { get; private set; }
    public virtual string? debugLabel { get; private set; }
    public virtual List<Shadow>? shadows { get; private set; }
    public virtual List<FontFeature>? fontFeatures { get; private set; }
    public virtual List<FontVariation>? fontVariations { get; private set; }
    public virtual TextOverflow? overflow { get; private set; }

    public TextStyle(
        bool inherit = true,
        Color? color = null,
        Color? backgroundColor = null,
        double? fontSize = null,
        FontWeight? fontWeight = null,
        FontStyle? fontStyle = null,
        double? letterSpacing = null,
        double? wordSpacing = null,
        TextBaseline? textBaseline = null,
        double? height = null,
        TextLeadingDistribution? leadingDistribution = null,
        Locale? locale = null,
        Paint? foreground = null,
        Paint? background = null,
        List<Shadow>? shadows = null,
        List<FontFeature>? fontFeatures = null,
        List<FontVariation>? fontVariations = null,
        TextDecoration? decoration = null,
        Color? decorationColor = null,
        TextDecorationStyle? decorationStyle = null,
        double? decorationThickness = null,
        string? debugLabel = null,
        string? fontFamily = null,
        List<string>? fontFamilyFallback = null,
        string? package = null,
        TextOverflow? overflow = null
    )
    {
        this.inherit = inherit;
        this.color = color;
        this.backgroundColor = backgroundColor;
        this.fontSize = fontSize;
        this.fontWeight = fontWeight;
        this.fontStyle = fontStyle;
        this.letterSpacing = letterSpacing;
        this.wordSpacing = wordSpacing;
        this.textBaseline = textBaseline;
        this.height = height;
        this.leadingDistribution = leadingDistribution;
        this.locale = locale;
        this.foreground = foreground;
        this.background = background;
        this.shadows = shadows;
        this.fontFeatures = fontFeatures;
        this.fontVariations = fontVariations;
        this.decoration = decoration;
        this.decorationColor = decorationColor;
        this.decorationStyle = decorationStyle;
        this.decorationThickness = decorationThickness;
        this.debugLabel = debugLabel;
        this.overflow = overflow;
        this.fontFamily = (package is null) ? fontFamily : $"packages/{package}/{fontFamily}";
        _fontFamilyFallback = fontFamilyFallback;
        _package = package;
        System.Diagnostics.Debug.Assert((color is null) || (foreground is null));
        System.Diagnostics.Debug.Assert((backgroundColor is null) || (background is null));
        System.Diagnostics.Debug.Assert(
            (height is null)
                || (
                    DartRuntimePrimitives.RequireValue(height)
                    == DartRuntimePrimitives.RequireValue(height)
                )
        );
    }

    public virtual List<string>? fontFamilyFallback =>
        (_package is null)
            ? _fontFamilyFallback
            : _fontFamilyFallback?.map((str) => $"packages/{_package}/{str}").ToList();
    internal virtual string? _fontFamily
    {
        get
        {
            if (_package is not null)
            {
                var fontFamilyPrefix = $"packages/{_package}/";
                DartRuntimePrimitives.Assert(() =>
                    fontFamily?.startsWith(fontFamilyPrefix) ?? true
                );
                return fontFamily?.substring(fontFamilyPrefix.Length);
            }
            return fontFamily;
        }
    }

    public virtual TextStyle copyWith(
        bool? inherit = null,
        Color? color = null,
        Color? backgroundColor = null,
        double? fontSize = null,
        FontWeight? fontWeight = null,
        FontStyle? fontStyle = null,
        double? letterSpacing = null,
        double? wordSpacing = null,
        TextBaseline? textBaseline = null,
        double? height = null,
        TextLeadingDistribution? leadingDistribution = null,
        Locale? locale = null,
        Paint? foreground = null,
        Paint? background = null,
        List<Shadow>? shadows = null,
        List<FontFeature>? fontFeatures = null,
        List<FontVariation>? fontVariations = null,
        TextDecoration? decoration = null,
        Color? decorationColor = null,
        TextDecorationStyle? decorationStyle = null,
        double? decorationThickness = null,
        string? debugLabel = null,
        string? fontFamily = null,
        List<string>? fontFamilyFallback = null,
        string? package = null,
        TextOverflow? overflow = null
    )
    {
        DartRuntimePrimitives.Assert(() => (color is null) || (foreground is null));
        DartRuntimePrimitives.Assert(() => (backgroundColor is null) || (background is null));
        string? newDebugLabel = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            if (debugLabel is not null)
            {
                newDebugLabel = debugLabel;
            }
            else
            {
                if (this.debugLabel is not null)
                {
                    newDebugLabel = $"({this.debugLabel}).copyWith";
                }
            }
            return true;
        });
        return new TextStyle(
            inherit: inherit ?? this.inherit,
            color: ((this.foreground is null) && (foreground is null))
                ? (color ?? this.color)
                : null,
            backgroundColor: ((this.background is null) && (background is null))
                ? (backgroundColor ?? this.backgroundColor)
                : null,
            fontSize: fontSize ?? this.fontSize,
            fontWeight: fontWeight ?? this.fontWeight,
            fontStyle: fontStyle ?? this.fontStyle,
            letterSpacing: letterSpacing ?? this.letterSpacing,
            wordSpacing: wordSpacing ?? this.wordSpacing,
            textBaseline: textBaseline ?? this.textBaseline,
            height: height ?? this.height,
            leadingDistribution: leadingDistribution ?? this.leadingDistribution,
            locale: locale ?? this.locale,
            foreground: foreground ?? this.foreground,
            background: background ?? this.background,
            shadows: shadows ?? this.shadows,
            fontFeatures: fontFeatures ?? this.fontFeatures,
            fontVariations: fontVariations ?? this.fontVariations,
            decoration: decoration ?? this.decoration,
            decorationColor: decorationColor ?? this.decorationColor,
            decorationStyle: decorationStyle ?? this.decorationStyle,
            decorationThickness: decorationThickness ?? this.decorationThickness,
            debugLabel: newDebugLabel,
            fontFamily: fontFamily ?? _fontFamily,
            fontFamilyFallback: fontFamilyFallback ?? _fontFamilyFallback,
            package: package ?? _package,
            overflow: overflow ?? this.overflow
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextStyle apply(
        Color? color = null,
        Color? backgroundColor = null,
        TextDecoration? decoration = null,
        Color? decorationColor = null,
        TextDecorationStyle? decorationStyle = null,
        double decorationThicknessFactor = 1.0,
        double decorationThicknessDelta = 0.0,
        string? fontFamily = null,
        List<string>? fontFamilyFallback = null,
        double fontSizeFactor = 1.0,
        double fontSizeDelta = 0.0,
        long fontWeightDelta = 0,
        FontStyle? fontStyle = null,
        double letterSpacingFactor = 1.0,
        double letterSpacingDelta = 0.0,
        double wordSpacingFactor = 1.0,
        double wordSpacingDelta = 0.0,
        double heightFactor = 1.0,
        double heightDelta = 0.0,
        TextBaseline? textBaseline = null,
        TextLeadingDistribution? leadingDistribution = null,
        Locale? locale = null,
        List<Shadow>? shadows = null,
        List<FontFeature>? fontFeatures = null,
        List<FontVariation>? fontVariations = null,
        string? package = null,
        TextOverflow? overflow = null
    )
    {
        DartRuntimePrimitives.Assert(() =>
            (fontSize is not null) || ((fontSizeFactor == 1.0) && (fontSizeDelta == 0.0))
        );
        DartRuntimePrimitives.Assert(() => (fontWeight is not null) || (fontWeightDelta == 0.0));
        DartRuntimePrimitives.Assert(() =>
            (letterSpacing is not null)
            || ((letterSpacingFactor == 1.0) && (letterSpacingDelta == 0.0))
        );
        DartRuntimePrimitives.Assert(() =>
            (wordSpacing is not null) || ((wordSpacingFactor == 1.0) && (wordSpacingDelta == 0.0))
        );
        DartRuntimePrimitives.Assert(() =>
            (decorationThickness is not null)
            || ((decorationThicknessFactor == 1.0) && (decorationThicknessDelta == 0.0))
        );
        string? modifiedDebugLabel = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            if (debugLabel is not null)
            {
                modifiedDebugLabel = $"({debugLabel}).apply";
            }
            return true;
        });
        return new TextStyle(
            inherit: inherit,
            color: (foreground is null) ? (color ?? this.color) : null,
            backgroundColor: (background is null)
                ? (backgroundColor ?? this.backgroundColor)
                : null,
            fontFamily: fontFamily ?? _fontFamily,
            fontFamilyFallback: fontFamilyFallback ?? _fontFamilyFallback,
            fontSize: (fontSize is null)
                ? null
                : ((DartRuntimePrimitives.RequireValue(fontSize) * fontSizeFactor) + fontSizeDelta),
            fontWeight: (fontWeight is null)
                ? null
                : global::Doroti.Ui.FontWeight.values[
                    (int)
                        (FoundationRuntimePorts.EnumIndex(fontWeight!) + fontWeightDelta).clamp(
                            0L,
                            checked(global::Doroti.Ui.FontWeight.values.Count) - 1L
                        )
                ],
            fontStyle: fontStyle ?? this.fontStyle,
            letterSpacing: (letterSpacing is null)
                ? null
                : (
                    (DartRuntimePrimitives.RequireValue(letterSpacing) * letterSpacingFactor)
                    + letterSpacingDelta
                ),
            wordSpacing: (wordSpacing is null)
                ? null
                : (
                    (DartRuntimePrimitives.RequireValue(wordSpacing) * wordSpacingFactor)
                    + wordSpacingDelta
                ),
            textBaseline: textBaseline ?? this.textBaseline,
            height: ((height is null) || (height == Dart_uiLibrary.kTextHeightNone))
                ? height
                : ((DartRuntimePrimitives.RequireValue(height) * heightFactor) + heightDelta),
            leadingDistribution: leadingDistribution ?? this.leadingDistribution,
            locale: locale ?? this.locale,
            foreground: foreground,
            background: background,
            shadows: shadows ?? this.shadows,
            fontFeatures: fontFeatures ?? this.fontFeatures,
            fontVariations: fontVariations ?? this.fontVariations,
            decoration: decoration ?? this.decoration,
            decorationColor: decorationColor ?? this.decorationColor,
            decorationStyle: decorationStyle ?? this.decorationStyle,
            decorationThickness: (decorationThickness is null)
                ? null
                : (
                    (
                        DartRuntimePrimitives.RequireValue(decorationThickness)
                        * decorationThicknessFactor
                    ) + decorationThicknessDelta
                ),
            overflow: overflow ?? this.overflow,
            package: package ?? _package,
            debugLabel: modifiedDebugLabel
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextStyle merge(TextStyle? other)
    {
        if (other is null)
        {
            return this;
        }
        if (!other.inherit)
        {
            return other;
        }
        string? mergedDebugLabel = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            if ((other.debugLabel is not null) || (debugLabel is not null))
            {
                mergedDebugLabel =
                    $"({debugLabel ?? Text_styleLibrary._kDefaultDebugLabel}).merge({other.debugLabel ?? Text_styleLibrary._kDefaultDebugLabel})";
            }
            return true;
        });
        return copyWith(
            color: other.color,
            backgroundColor: other.backgroundColor,
            fontSize: other.fontSize,
            fontWeight: other.fontWeight,
            fontStyle: other.fontStyle,
            letterSpacing: other.letterSpacing,
            wordSpacing: other.wordSpacing,
            textBaseline: other.textBaseline,
            height: other.height,
            leadingDistribution: other.leadingDistribution,
            locale: other.locale,
            foreground: other.foreground,
            background: other.background,
            shadows: other.shadows,
            fontFeatures: other.fontFeatures,
            fontVariations: other.fontVariations,
            decoration: other.decoration,
            decorationColor: other.decorationColor,
            decorationStyle: other.decorationStyle,
            decorationThickness: other.decorationThickness,
            debugLabel: mergedDebugLabel,
            fontFamily: other._fontFamily,
            fontFamilyFallback: other._fontFamilyFallback,
            package: other._package,
            overflow: other.overflow
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextStyle? lerp(TextStyle? a, TextStyle? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        string? lerpDebugLabel = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            lerpDebugLabel =
                $"lerp({a?.debugLabel ?? Text_styleLibrary._kDefaultDebugLabel} ⎯{t.toStringAsFixed(1L)}→ {b?.debugLabel ?? Text_styleLibrary._kDefaultDebugLabel})";
            return true;
        });
        if (a is null)
        {
            return new TextStyle(
                inherit: b!.inherit,
                color: Dart_uiLibrary.Color.lerp(null, b.color, t),
                backgroundColor: Dart_uiLibrary.Color.lerp(null, b.backgroundColor, t),
                fontSize: (t < 0.5) ? null : b.fontSize,
                fontWeight: Dart_uiLibrary.FontWeight.lerp(null, b.fontWeight, t),
                fontStyle: (t < 0.5) ? null : b.fontStyle,
                letterSpacing: (t < 0.5) ? null : b.letterSpacing,
                wordSpacing: (t < 0.5) ? null : b.wordSpacing,
                textBaseline: (t < 0.5) ? null : b.textBaseline,
                height: (t < 0.5) ? null : b.height,
                leadingDistribution: (t < 0.5) ? null : b.leadingDistribution,
                locale: (t < 0.5) ? null : b.locale,
                foreground: (t < 0.5) ? null : b.foreground,
                background: (t < 0.5) ? null : b.background,
                shadows: (t < 0.5) ? null : b.shadows,
                fontFeatures: (t < 0.5) ? null : b.fontFeatures,
                fontVariations: Text_styleLibrary.lerpFontVariations(null, b.fontVariations, t),
                decoration: (t < 0.5) ? null : b.decoration,
                decorationColor: Dart_uiLibrary.Color.lerp(null, b.decorationColor, t),
                decorationStyle: (t < 0.5) ? null : b.decorationStyle,
                decorationThickness: (t < 0.5) ? null : b.decorationThickness,
                debugLabel: lerpDebugLabel,
                fontFamily: (t < 0.5) ? null : b._fontFamily,
                fontFamilyFallback: (t < 0.5) ? null : b._fontFamilyFallback,
                package: (t < 0.5) ? null : b._package,
                overflow: (t < 0.5) ? null : b.overflow
            );
        }
        if (b is null)
        {
            return new TextStyle(
                inherit: a.inherit,
                color: Dart_uiLibrary.Color.lerp(a.color, null, t),
                backgroundColor: Dart_uiLibrary.Color.lerp(null, a.backgroundColor, t),
                fontSize: (t < 0.5) ? a.fontSize : null,
                fontWeight: Dart_uiLibrary.FontWeight.lerp(a.fontWeight, null, t),
                fontStyle: (t < 0.5) ? a.fontStyle : null,
                letterSpacing: (t < 0.5) ? a.letterSpacing : null,
                wordSpacing: (t < 0.5) ? a.wordSpacing : null,
                textBaseline: (t < 0.5) ? a.textBaseline : null,
                height: (t < 0.5) ? a.height : null,
                leadingDistribution: (t < 0.5) ? a.leadingDistribution : null,
                locale: (t < 0.5) ? a.locale : null,
                foreground: (t < 0.5) ? a.foreground : null,
                background: (t < 0.5) ? a.background : null,
                shadows: (t < 0.5) ? a.shadows : null,
                fontFeatures: (t < 0.5) ? a.fontFeatures : null,
                fontVariations: Text_styleLibrary.lerpFontVariations(a.fontVariations, null, t),
                decoration: (t < 0.5) ? a.decoration : null,
                decorationColor: Dart_uiLibrary.Color.lerp(a.decorationColor, null, t),
                decorationStyle: (t < 0.5) ? a.decorationStyle : null,
                decorationThickness: (t < 0.5) ? a.decorationThickness : null,
                debugLabel: lerpDebugLabel,
                fontFamily: (t < 0.5) ? a._fontFamily : null,
                fontFamilyFallback: (t < 0.5) ? a._fontFamilyFallback : null,
                package: (t < 0.5) ? a._package : null,
                overflow: (t < 0.5) ? a.overflow : null
            );
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (a.inherit == b.inherit)
            {
                return true;
            }
            var nullFields = new List<string>();
            if (checked((long)nullFields.Count) == 0)
            {
                return true;
            }
            throw new FlutterError(
                new List<DiagnosticsNode>
                {
                    new ErrorSummary(
                        "Failed to interpolate TextStyles with different inherit values."
                    ),
                    new ErrorSpacer(),
                    new ErrorDescription("The TextStyles being interpolated were:"),
                    ((Diagnosticable)a).toDiagnosticsNode(
                        name: "from",
                        style: DiagnosticsTreeStyle.singleLine
                    ),
                    ((Diagnosticable)b).toDiagnosticsNode(
                        name: "to",
                        style: DiagnosticsTreeStyle.singleLine
                    ),
                    new ErrorDescription(
                        "The following fields are unspecified in both TextStyles:\n"
                            + $"{string.Join(", ", nullFields.map((name) => $"\"{name}\""))}.\n"
                            + "When \"inherit\" changes during the transition, these fields may "
                            + "observe abrupt value changes as a result, causing \"jump\"s in the "
                            + "transition."
                    ),
                    new ErrorSpacer(),
                    new ErrorHint(
                        "In general, TextStyle.lerp only works well when both TextStyles have "
                            + "the same \"inherit\" value, and specify the same fields."
                    ),
                    new ErrorHint(
                        "If the TextStyles were directly created by you, consider bringing "
                            + "them to parity to ensure a smooth transition."
                    ),
                    new ErrorSpacer(),
                    new ErrorHint(
                        "If one of the TextStyles being lerped is significantly more elaborate "
                            + "than the other, and has \"inherited\" set to false, it is often because "
                            + "it is merged with another TextStyle before being lerped. Comparing "
                            + "the \"debugLabel\"s of the two TextStyles may help identify if that was "
                            + "the case."
                    ),
                    new ErrorHint(
                        "For example, you may see this error message when trying to lerp "
                            + "between \"ThemeData()\" and \"Theme.of(context)\". This is because "
                            + "TextStyles from \"Theme.of(context)\" are merged with TextStyles from "
                            + "another theme and thus are more elaborate than the TextStyles from "
                            + "\"ThemeData()\" (which is reflected in their \"debugLabel\"s -- "
                            + "TextStyles from \"Theme.of(context)\" should have labels in the form of "
                            + "\"(<A TextStyle>).merge(<Another TextStyle>)\"). It is recommended to "
                            + "only lerp ThemeData with matching TextStyles."
                    ),
                }
            );
        });
        return new TextStyle(
            inherit: (t < 0.5) ? a.inherit : b.inherit,
            color: ((a.foreground is null) && (b.foreground is null))
                ? Dart_uiLibrary.Color.lerp(a.color, b.color, t)
                : null,
            backgroundColor: ((a.background is null) && (b.background is null))
                ? Dart_uiLibrary.Color.lerp(a.backgroundColor, b.backgroundColor, t)
                : null,
            fontSize: Dart_uiLibrary.lerpDouble(
                a.fontSize ?? b.fontSize,
                b.fontSize ?? a.fontSize,
                t
            ),
            fontWeight: Dart_uiLibrary.FontWeight.lerp(a.fontWeight, b.fontWeight, t),
            fontStyle: (t < 0.5) ? a.fontStyle : b.fontStyle,
            letterSpacing: Dart_uiLibrary.lerpDouble(
                a.letterSpacing ?? b.letterSpacing,
                b.letterSpacing ?? a.letterSpacing,
                t
            ),
            wordSpacing: Dart_uiLibrary.lerpDouble(
                a.wordSpacing ?? b.wordSpacing,
                b.wordSpacing ?? a.wordSpacing,
                t
            ),
            textBaseline: (t < 0.5) ? a.textBaseline : b.textBaseline,
            height: Dart_uiLibrary.lerpDouble(a.height ?? b.height, b.height ?? a.height, t),
            leadingDistribution: (t < 0.5) ? a.leadingDistribution : b.leadingDistribution,
            locale: (t < 0.5) ? a.locale : b.locale,
            foreground: ((a.foreground is not null) || (b.foreground is not null))
                ? (
                    (t < 0.5)
                        ? (
                            a.foreground
                            ?? (
                                (Func<Paint>)(
                                    () =>
                                    {
                                        var __cascade = new Paint();
                                        __cascade.color = a.color!;
                                        return __cascade;
                                    }
                                )
                            )()
                        )
                        : (
                            b.foreground
                            ?? (
                                (Func<Paint>)(
                                    () =>
                                    {
                                        var __cascade = new Paint();
                                        __cascade.color = b.color!;
                                        return __cascade;
                                    }
                                )
                            )()
                        )
                )
                : null,
            background: ((a.background is not null) || (b.background is not null))
                ? (
                    (t < 0.5)
                        ? (
                            a.background
                            ?? (
                                (Func<Paint>)(
                                    () =>
                                    {
                                        var __cascade = new Paint();
                                        __cascade.color = a.backgroundColor!;
                                        return __cascade;
                                    }
                                )
                            )()
                        )
                        : (
                            b.background
                            ?? (
                                (Func<Paint>)(
                                    () =>
                                    {
                                        var __cascade = new Paint();
                                        __cascade.color = b.backgroundColor!;
                                        return __cascade;
                                    }
                                )
                            )()
                        )
                )
                : null,
            shadows: Dart_uiLibrary.Shadow.lerpList(a.shadows, b.shadows, t),
            fontFeatures: (t < 0.5) ? a.fontFeatures : b.fontFeatures,
            fontVariations: Text_styleLibrary.lerpFontVariations(
                a.fontVariations,
                b.fontVariations,
                t
            ),
            decoration: (t < 0.5) ? a.decoration : b.decoration,
            decorationColor: Dart_uiLibrary.Color.lerp(a.decorationColor, b.decorationColor, t),
            decorationStyle: (t < 0.5) ? a.decorationStyle : b.decorationStyle,
            decorationThickness: Dart_uiLibrary.lerpDouble(
                a.decorationThickness ?? b.decorationThickness,
                b.decorationThickness ?? a.decorationThickness,
                t
            ),
            debugLabel: lerpDebugLabel,
            fontFamily: (t < 0.5) ? a._fontFamily : b._fontFamily,
            fontFamilyFallback: (t < 0.5) ? a._fontFamilyFallback : b._fontFamilyFallback,
            package: (t < 0.5) ? a._package : b._package,
            overflow: (t < 0.5) ? a.overflow : b.overflow
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Ui.TextStyle getTextStyle(
        double textScaleFactor = 1.0,
        TextScaler textScaler = default!
    )
    {
        textScaler ??= TextScaler.noScaling;
        DartRuntimePrimitives.Assert(() =>
            DartRuntimePrimitives.Identical(textScaler, TextScaler.noScaling)
            || (textScaleFactor == 1.0)
        );
        double? fontSizeLocal = fontSize switch
        {
            null => null,
            double size when Equals(textScaler, TextScaler.noScaling) => size * textScaleFactor,
            double sizeLocal => textScaler.scale(sizeLocal),
        };
        return new Ui.TextStyle(
            color: color,
            decoration: decoration,
            decorationColor: decorationColor,
            decorationStyle: decorationStyle,
            decorationThickness: decorationThickness,
            fontWeight: fontWeight,
            fontStyle: fontStyle,
            textBaseline: textBaseline,
            leadingDistribution: leadingDistribution,
            fontFamily: fontFamily,
            fontFamilyFallback: fontFamilyFallback,
            fontSize: fontSizeLocal,
            letterSpacing: letterSpacing,
            wordSpacing: wordSpacing,
            height: height,
            locale: locale,
            foreground: foreground,
            background: (background, backgroundColor) switch
            {
                (Paint paint, _) => paint,
                (_, Color colorLocal) => (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.color = colorLocal;
                            return __cascade;
                        }
                    )
                )(),
                _ => null,
            },
            shadows: shadows,
            fontFeatures: fontFeatures,
            fontVariations: fontVariations
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ParagraphStyle getParagraphStyle(
        TextAlign? textAlign = null,
        TextDirection? textDirection = null,
        TextScaler textScaler = default!,
        string? ellipsis = null,
        long? maxLines = null,
        TextHeightBehavior? textHeightBehavior = null,
        Locale? locale = null,
        string? fontFamily = null,
        double? fontSize = null,
        FontWeight? fontWeight = null,
        FontStyle? fontStyle = null,
        double? height = null,
        StrutStyle? strutStyle = null
    )
    {
        textScaler ??= TextScaler.noScaling;
        DartRuntimePrimitives.Assert(() =>
            (maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)
        );
        DartRuntimePrimitives.Assert(() =>
            (height is null) || !double.IsNaN(DartRuntimePrimitives.RequireValue(height))
        );
        TextLeadingDistribution? leadingDistributionLocal = leadingDistribution;
        TextHeightBehavior? effectiveTextHeightBehavior =
            textHeightBehavior
            ?? (
                (leadingDistributionLocal is null)
                    ? null
                    : new TextHeightBehavior(
                        leadingDistribution: DartRuntimePrimitives.RequireValue(
                            leadingDistributionLocal
                        )
                    )
            );
        return new ParagraphStyle(
            textAlign: textAlign,
            textDirection: textDirection,
            fontWeight: fontWeight ?? this.fontWeight,
            fontStyle: fontStyle ?? this.fontStyle,
            fontFamily: fontFamily ?? this.fontFamily,
            fontSize: textScaler.scale(
                (fontSize ?? this.fontSize) ?? Text_painterLibrary.kDefaultFontSize
            ),
            height: height ?? this.height,
            textHeightBehavior: effectiveTextHeightBehavior,
            strutStyle: (strutStyle is null)
                ? null
                : new Ui.StrutStyle(
                    fontFamily: strutStyle.fontFamily,
                    fontFamilyFallback: strutStyle.fontFamilyFallback,
                    fontSize: strutStyle.fontSize switch
                    {
                        null => null,
                        double unscaled => textScaler.scale(unscaled),
                    },
                    height: strutStyle.height,
                    leading: strutStyle.leading,
                    leadingDistribution: strutStyle.leadingDistribution,
                    fontWeight: strutStyle.fontWeight,
                    fontStyle: strutStyle.fontStyle,
                    forceStrutHeight: strutStyle.forceStrutHeight
                ),
            maxLines: maxLines,
            ellipsis: ellipsis,
            locale: locale
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderComparison compareTo(TextStyle other)
    {
        if (DartRuntimePrimitives.Identical(this, other))
        {
            return RenderComparison.identical;
        }
        if (
            (inherit != other.inherit)
            || (fontFamily != other.fontFamily)
            || (fontSize != other.fontSize)
            || (!Equals(fontWeight, other.fontWeight))
            || (!Equals(fontStyle, other.fontStyle))
            || (letterSpacing != other.letterSpacing)
            || (wordSpacing != other.wordSpacing)
            || (!Equals(textBaseline, other.textBaseline))
            || (height != other.height)
            || (!Equals(leadingDistribution, other.leadingDistribution))
            || (!Equals(locale, other.locale))
            || (!Equals(foreground, other.foreground))
            || (!Equals(background, other.background))
            || !CollectionsLibrary.listEquals(shadows, other.shadows)
            || !CollectionsLibrary.listEquals(fontFeatures, other.fontFeatures)
            || !CollectionsLibrary.listEquals(fontVariations, other.fontVariations)
            || !CollectionsLibrary.listEquals(fontFamilyFallback, other.fontFamilyFallback)
            || (!Equals(overflow, other.overflow))
        )
        {
            return RenderComparison.layout;
        }
        if (
            (!Equals(color, other.color))
            || (!Equals(backgroundColor, other.backgroundColor))
            || (!Equals(decoration, other.decoration))
            || (!Equals(decorationColor, other.decorationColor))
            || (!Equals(decorationStyle, other.decorationStyle))
            || (decorationThickness != other.decorationThickness)
        )
        {
            return RenderComparison.paint;
        }
        return RenderComparison.identical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as TextStyle;
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
        return (__other is TextStyle)
            && (__other.inherit == inherit)
            && Equals(__other.color, color)
            && Equals(__other.backgroundColor, backgroundColor)
            && (__other.fontSize == fontSize)
            && Equals(__other.fontWeight, fontWeight)
            && Equals(__other.fontStyle, fontStyle)
            && (__other.letterSpacing == letterSpacing)
            && (__other.wordSpacing == wordSpacing)
            && Equals(__other.textBaseline, textBaseline)
            && (__other.height == height)
            && Equals(__other.leadingDistribution, leadingDistribution)
            && Equals(__other.locale, locale)
            && Equals(__other.foreground, foreground)
            && Equals(__other.background, background)
            && CollectionsLibrary.listEquals(__other.shadows, shadows)
            && CollectionsLibrary.listEquals(__other.fontFeatures, fontFeatures)
            && CollectionsLibrary.listEquals(__other.fontVariations, fontVariations)
            && Equals(__other.decoration, decoration)
            && Equals(__other.decorationColor, decorationColor)
            && Equals(__other.decorationStyle, decorationStyle)
            && (__other.decorationThickness == decorationThickness)
            && (__other.fontFamily == fontFamily)
            && CollectionsLibrary.listEquals(__other.fontFamilyFallback, fontFamilyFallback)
            && (__other._package == _package)
            && Equals(__other.overflow, overflow);
    }

    public override int GetHashCode()
    {
        List<string>? fontFamilyFallbackLocal = fontFamilyFallback;
        long fontHash = FoundationRuntimePorts.ObjectHash(
            decorationStyle,
            decorationThickness,
            fontFamily,
            (fontFamilyFallbackLocal is null)
                ? null
                : FoundationRuntimePorts.ObjectHashAll(fontFamilyFallbackLocal),
            _package,
            overflow
        );
        List<Shadow>? shadowsLocal = shadows;
        List<FontFeature>? fontFeaturesLocal = fontFeatures;
        List<FontVariation>? fontVariationsLocal = fontVariations;
        return FoundationRuntimePorts.ObjectHash(
            inherit,
            color,
            backgroundColor,
            fontSize,
            fontWeight,
            fontStyle,
            letterSpacing,
            wordSpacing,
            textBaseline,
            height,
            leadingDistribution,
            locale,
            foreground,
            background,
            (shadowsLocal is null) ? null : FoundationRuntimePorts.ObjectHashAll(shadowsLocal),
            (fontFeaturesLocal is null)
                ? null
                : FoundationRuntimePorts.ObjectHashAll(fontFeaturesLocal),
            (fontVariationsLocal is null)
                ? null
                : FoundationRuntimePorts.ObjectHashAll(fontVariationsLocal),
            decoration,
            decorationColor,
            fontHash
        );
    }

    public virtual string toStringShort() =>
        objectRuntimeTypeFunctions.objectRuntimeType(this, "TextStyle");

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
            new ColorProperty($"{prefix}color", color, defaultValue: null),
            new ColorProperty($"{prefix}backgroundColor", backgroundColor, defaultValue: null),
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
            weightDescription = $"{FoundationRuntimePorts.EnumIndex(fontWeight!) + 1L}00";
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
        styles.Add(new DoubleProperty($"{prefix}letterSpacing", letterSpacing, defaultValue: null));
        styles.Add(new DoubleProperty($"{prefix}wordSpacing", wordSpacing, defaultValue: null));
        styles.Add(
            new EnumProperty<TextBaseline>($"{prefix}baseline", textBaseline, defaultValue: null)
        );
        styles.Add(new DoubleProperty($"{prefix}height", height, unit: "x", defaultValue: null));
        styles.Add(
            new EnumProperty<TextLeadingDistribution>(
                $"{prefix}leadingDistribution",
                leadingDistribution,
                defaultValue: null
            )
        );
        styles.Add(new DiagnosticsProperty<Locale>($"{prefix}locale", locale, defaultValue: null));
        styles.Add(
            new DiagnosticsProperty<Paint>($"{prefix}foreground", foreground, defaultValue: null)
        );
        styles.Add(
            new DiagnosticsProperty<Paint>($"{prefix}background", background, defaultValue: null)
        );
        if (
            (decoration is not null)
            || (decorationColor is not null)
            || (decorationStyle is not null)
            || (decorationThickness is not null)
        )
        {
            var decorationDescription = new List<string>();
            if (decorationStyle is not null)
            {
                TextDecorationStyle decorationStyle__value66964 =
                    DartRuntimePrimitives.RequireValue(decorationStyle);
                decorationDescription.Add(
                    DartRuntimePrimitives.RequireValue(decorationStyle).ToString()
                );
            }
            styles.Add(
                new ColorProperty(
                    $"{prefix}decorationColor",
                    decorationColor,
                    defaultValue: null,
                    level: DiagnosticLevel.fine
                )
            );
            if (decorationColor is not null)
            {
                decorationDescription.Add($"{decorationColor}");
            }
            styles.Add(
                new DiagnosticsProperty<TextDecoration>(
                    $"{prefix}decoration",
                    decoration,
                    defaultValue: null,
                    level: DiagnosticLevel.hidden
                )
            );
            if (decoration is not null)
            {
                decorationDescription.Add($"{decoration}");
            }
            DartRuntimePrimitives.Assert(() => checked((long)decorationDescription.Count) != 0);
            styles.Add(
                new MessageProperty($"{prefix}decoration", string.Join(" ", decorationDescription))
            );
            styles.Add(
                new DoubleProperty(
                    $"{prefix}decorationThickness",
                    decorationThickness,
                    unit: "x",
                    defaultValue: null
                )
            );
        }
        bool styleSpecified = styles.any((n) => !n.isFiltered(DiagnosticLevel.info));
        properties.add(
            new DiagnosticsProperty<bool>(
                $"{prefix}inherit",
                inherit,
                level: (!styleSpecified && inherit) ? DiagnosticLevel.fine : DiagnosticLevel.info
            )
        );
        styles.forEach(properties.add);
        if (!styleSpecified)
        {
            properties.add(
                new FlagProperty(
                    "inherit",
                    value: inherit,
                    ifTrue: $"{prefix}<all styles inherited>",
                    ifFalse: $"{prefix}<no style specified>"
                )
            );
        }
        styles.Add(
            new EnumProperty<TextOverflow>($"{prefix}overflow", overflow, defaultValue: null)
        );
    }
}

public static partial class Text_styleLibrary
{
    public static List<FontVariation>? lerpFontVariations(
        List<FontVariation>? a,
        List<FontVariation>? b,
        double t
    )
    {
        if (t == 0.0)
        {
            return a;
        }
        if (t == 1.0)
        {
            return b;
        }
        if (
            (a is null)
            || (checked((long)a.Count) == 0)
            || (b is null)
            || (checked((long)b.Count) == 0)
        )
        {
            return (t < 0.5) ? a : b;
        }
        DartRuntimePrimitives.Assert(() =>
            (checked((long)a.Count) != 0) && (checked((long)b.Count) != 0)
        );
        var result = new List<FontVariation>();
        var index = 0L;
        long minLength =
            (checked(a.Count) < checked((long)b.Count)) ? checked(a.Count) : checked((long)b.Count);
        for (; index < minLength; index += 1L)
        {
            if (a[(int)index].axis != b[(int)index].axis)
            {
                break;
            }
            result.Add(Dart_uiLibrary.FontVariation.lerp(a[(int)index], b[(int)index], t)!);
        }
        long maxLength =
            (checked(a.Count) > checked((long)b.Count)) ? checked(a.Count) : checked((long)b.Count);
        if (index < maxLength)
        {
            HashSet<string> axes = new HashSet<string>();
            DartMap<string, FontVariation> aVariations = new DartMap<string, FontVariation>().cast<
                string,
                FontVariation
            >();
            for (var indexA = index; indexA < checked(a.Count); indexA += 1L)
            {
                aVariations[a[(int)indexA].axis] = a[(int)indexA];
                axes.Add(a[(int)indexA].axis);
            }
            DartMap<string, FontVariation> bVariations = new DartMap<string, FontVariation>().cast<
                string,
                FontVariation
            >();
            for (var indexB = index; indexB < checked(b.Count); indexB += 1L)
            {
                bVariations[b[(int)indexB].axis] = b[(int)indexB];
                axes.Add(b[(int)indexB].axis);
            }
            foreach (var axisLocal in axes)
            {
                FontVariation? variation = Dart_uiLibrary.FontVariation.lerp(
                    aVariations.GetValueOrDefault(axisLocal),
                    bVariations.GetValueOrDefault(axisLocal),
                    t
                );
                if (variation is not null)
                {
                    result.Add(variation);
                }
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
