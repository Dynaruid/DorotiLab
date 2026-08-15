// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_style.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Painting;

public static partial class Text_styleLibrary
{
    internal static string _kDefaultDebugLabel = "unknown";
}

public static partial class Text_styleLibrary
{
    internal static string _kColorForegroundWarning = "Cannot provide both a color and a foreground\n" + "The color argument is just a shorthand for \"foreground: Paint()..color = color\".";
}

public static partial class Text_styleLibrary
{
    internal static string _kColorBackgroundWarning = "Cannot provide both a backgroundColor and a background\n" + "The backgroundColor argument is just a shorthand for \"background: Paint()..color = color\".";
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

    public TextStyle(bool inherit = true, Color? color = null, Color? backgroundColor = null, double? fontSize = null, FontWeight? fontWeight = null, FontStyle? fontStyle = null, double? letterSpacing = null, double? wordSpacing = null, TextBaseline? textBaseline = null, double? height = null, TextLeadingDistribution? leadingDistribution = null, Locale? locale = null, Paint? foreground = null, Paint? background = null, List<Shadow>? shadows = null, List<FontFeature>? fontFeatures = null, List<FontVariation>? fontVariations = null, TextDecoration? decoration = null, Color? decorationColor = null, TextDecorationStyle? decorationStyle = null, double? decorationThickness = null, string? debugLabel = null, string? fontFamily = null, List<string>? fontFamilyFallback = null, string? package = null, TextOverflow? overflow = null)
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
        this.fontFamily = ((package is null) ? fontFamily : $"packages/{package}/{fontFamily}");
        this._fontFamilyFallback = fontFamilyFallback;
        this._package = package;
        System.Diagnostics.Debug.Assert(((color is null) || (foreground is null)));
        System.Diagnostics.Debug.Assert(((backgroundColor is null) || (background is null)));
        System.Diagnostics.Debug.Assert(((height is null) || (DartRuntimePrimitives.RequireValue(height) == DartRuntimePrimitives.RequireValue(height))));
    }

    public virtual List<string>? fontFamilyFallback => ((this._package is null) ? this._fontFamilyFallback : this._fontFamilyFallback?.map<string, string>(((str) => $"packages/{this._package}/{str}")).ToList());
    internal virtual string? _fontFamily
    {
        get
        {
            if ((this._package is not null))
            {
                var fontFamilyPrefix__35550 = $"packages/{this._package}/";
                DartRuntimePrimitives.Assert(() => (this.fontFamily?.startsWith(fontFamilyPrefix__35550) ?? true));
                return this.fontFamily?.substring(fontFamilyPrefix__35550.Length);
            }
            return this.fontFamily;
            return default!;
        }
    }
    public virtual TextStyle copyWith(bool? inherit = null, Color? color = null, Color? backgroundColor = null, double? fontSize = null, FontWeight? fontWeight = null, FontStyle? fontStyle = null, double? letterSpacing = null, double? wordSpacing = null, TextBaseline? textBaseline = null, double? height = null, TextLeadingDistribution? leadingDistribution = null, Locale? locale = null, Paint? foreground = null, Paint? background = null, List<Shadow>? shadows = null, List<FontFeature>? fontFeatures = null, List<FontVariation>? fontVariations = null, TextDecoration? decoration = null, Color? decorationColor = null, TextDecorationStyle? decorationStyle = null, double? decorationThickness = null, string? debugLabel = null, string? fontFamily = null, List<string>? fontFamilyFallback = null, string? package = null, TextOverflow? overflow = null)
    {
        DartRuntimePrimitives.Assert(() => ((color is null) || (foreground is null)));
        DartRuntimePrimitives.Assert(() => ((backgroundColor is null) || (background is null)));
        string? newDebugLabel__37129 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((debugLabel is not null))
                {
                    newDebugLabel__37129 = debugLabel;
                }
                else
                {
                    if ((this.debugLabel is not null))
                    {
                        newDebugLabel__37129 = $"({this.debugLabel}).copyWith";
                    }
                }
                return true;
            });
        return new TextStyle(inherit: (inherit ?? this.inherit), color: (((this.foreground is null) && (foreground is null)) ? (color ?? this.color) : null), backgroundColor: (((this.background is null) && (background is null)) ? (backgroundColor ?? this.backgroundColor) : null), fontSize: (fontSize ?? this.fontSize), fontWeight: (fontWeight ?? this.fontWeight), fontStyle: (fontStyle ?? this.fontStyle), letterSpacing: (letterSpacing ?? this.letterSpacing), wordSpacing: (wordSpacing ?? this.wordSpacing), textBaseline: (textBaseline ?? this.textBaseline), height: (height ?? this.height), leadingDistribution: (leadingDistribution ?? this.leadingDistribution), locale: (locale ?? this.locale), foreground: (foreground ?? this.foreground), background: (background ?? this.background), shadows: (shadows ?? this.shadows), fontFeatures: (fontFeatures ?? this.fontFeatures), fontVariations: (fontVariations ?? this.fontVariations), decoration: (decoration ?? this.decoration), decorationColor: (decorationColor ?? this.decorationColor), decorationStyle: (decorationStyle ?? this.decorationStyle), decorationThickness: (decorationThickness ?? this.decorationThickness), debugLabel: newDebugLabel__37129, fontFamily: (fontFamily ?? this._fontFamily), fontFamilyFallback: (fontFamilyFallback ?? this._fontFamilyFallback), package: (package ?? this._package), overflow: (overflow ?? this.overflow));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextStyle apply(Color? color = null, Color? backgroundColor = null, TextDecoration? decoration = null, Color? decorationColor = null, TextDecorationStyle? decorationStyle = null, double decorationThicknessFactor = 1.0, double decorationThicknessDelta = 0.0, string? fontFamily = null, List<string>? fontFamilyFallback = null, double fontSizeFactor = 1.0, double fontSizeDelta = 0.0, long fontWeightDelta = 0, FontStyle? fontStyle = null, double letterSpacingFactor = 1.0, double letterSpacingDelta = 0.0, double wordSpacingFactor = 1.0, double wordSpacingDelta = 0.0, double heightFactor = 1.0, double heightDelta = 0.0, TextBaseline? textBaseline = null, TextLeadingDistribution? leadingDistribution = null, Locale? locale = null, List<Shadow>? shadows = null, List<FontFeature>? fontFeatures = null, List<FontVariation>? fontVariations = null, string? package = null, TextOverflow? overflow = null)
    {
        DartRuntimePrimitives.Assert(() => ((this.fontSize is not null) || (((fontSizeFactor == 1.0) && (fontSizeDelta == 0.0)))));
        DartRuntimePrimitives.Assert(() => ((this.fontWeight is not null) || (fontWeightDelta == 0.0)));
        DartRuntimePrimitives.Assert(() => ((this.letterSpacing is not null) || (((letterSpacingFactor == 1.0) && (letterSpacingDelta == 0.0)))));
        DartRuntimePrimitives.Assert(() => ((this.wordSpacing is not null) || (((wordSpacingFactor == 1.0) && (wordSpacingDelta == 0.0)))));
        DartRuntimePrimitives.Assert(() => ((this.decorationThickness is not null) || (((decorationThicknessFactor == 1.0) && (decorationThicknessDelta == 0.0)))));
        string? modifiedDebugLabel__41676 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this.debugLabel is not null))
                {
                    modifiedDebugLabel__41676 = $"({this.debugLabel}).apply";
                }
                return true;
            });
        return new TextStyle(inherit: this.inherit, color: ((this.foreground is null) ? (color ?? this.color) : null), backgroundColor: ((this.background is null) ? (backgroundColor ?? this.backgroundColor) : null), fontFamily: (fontFamily ?? this._fontFamily), fontFamilyFallback: (fontFamilyFallback ?? this._fontFamilyFallback), fontSize: ((this.fontSize is null) ? null : ((DartRuntimePrimitives.RequireValue(this.fontSize) * fontSizeFactor) + fontSizeDelta)), fontWeight: ((this.fontWeight is null) ? null : global::Doroti.Ui.FontWeight.values[(int)(((FoundationRuntimePorts.EnumIndex(this.fontWeight!) + fontWeightDelta)).clamp(0L, (checked((long)(global::Doroti.Ui.FontWeight.values.Count)) - 1L)))]), fontStyle: (fontStyle ?? this.fontStyle), letterSpacing: ((this.letterSpacing is null) ? null : ((DartRuntimePrimitives.RequireValue(this.letterSpacing) * letterSpacingFactor) + letterSpacingDelta)), wordSpacing: ((this.wordSpacing is null) ? null : ((DartRuntimePrimitives.RequireValue(this.wordSpacing) * wordSpacingFactor) + wordSpacingDelta)), textBaseline: (textBaseline ?? this.textBaseline), height: ((((this.height is null) || (this.height == Dart_uiLibrary.kTextHeightNone))) ? this.height : ((DartRuntimePrimitives.RequireValue(this.height) * heightFactor) + heightDelta)), leadingDistribution: (leadingDistribution ?? this.leadingDistribution), locale: (locale ?? this.locale), foreground: this.foreground, background: this.background, shadows: (shadows ?? this.shadows), fontFeatures: (fontFeatures ?? this.fontFeatures), fontVariations: (fontVariations ?? this.fontVariations), decoration: (decoration ?? this.decoration), decorationColor: (decorationColor ?? this.decorationColor), decorationStyle: (decorationStyle ?? this.decorationStyle), decorationThickness: ((this.decorationThickness is null) ? null : ((DartRuntimePrimitives.RequireValue(this.decorationThickness) * decorationThicknessFactor) + decorationThicknessDelta)), overflow: (overflow ?? this.overflow), package: (package ?? this._package), debugLabel: modifiedDebugLabel__41676);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextStyle merge(TextStyle? other)
    {
        if ((other is null))
        {
            return this;
        }
        if (!((TextStyle)other).inherit)
        {
            return other;
        }
        string? mergedDebugLabel__44976 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                if (((((TextStyle)other).debugLabel is not null) || (this.debugLabel is not null)))
                {
                    mergedDebugLabel__44976 = $"({(this.debugLabel ?? Text_styleLibrary._kDefaultDebugLabel)}).merge({(((TextStyle)other).debugLabel ?? Text_styleLibrary._kDefaultDebugLabel)})";
                }
                return true;
            });
        return copyWith(color: ((TextStyle)other).color, backgroundColor: ((TextStyle)other).backgroundColor, fontSize: ((TextStyle)other).fontSize, fontWeight: ((TextStyle)other).fontWeight, fontStyle: ((TextStyle)other).fontStyle, letterSpacing: ((TextStyle)other).letterSpacing, wordSpacing: ((TextStyle)other).wordSpacing, textBaseline: ((TextStyle)other).textBaseline, height: ((TextStyle)other).height, leadingDistribution: ((TextStyle)other).leadingDistribution, locale: ((TextStyle)other).locale, foreground: ((TextStyle)other).foreground, background: ((TextStyle)other).background, shadows: ((TextStyle)other).shadows, fontFeatures: ((TextStyle)other).fontFeatures, fontVariations: ((TextStyle)other).fontVariations, decoration: ((TextStyle)other).decoration, decorationColor: ((TextStyle)other).decorationColor, decorationStyle: ((TextStyle)other).decorationStyle, decorationThickness: ((TextStyle)other).decorationThickness, debugLabel: mergedDebugLabel__44976, fontFamily: ((TextStyle)other)._fontFamily, fontFamilyFallback: ((TextStyle)other)._fontFamilyFallback, package: ((TextStyle)other)._package, overflow: ((TextStyle)other).overflow);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextStyle? lerp(TextStyle? a, TextStyle? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        string? lerpDebugLabel__47582 = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                lerpDebugLabel__47582 = $"lerp({(a?.debugLabel ?? Text_styleLibrary._kDefaultDebugLabel)} ⎯{t.toStringAsFixed(1L)}→ {(b?.debugLabel ?? Text_styleLibrary._kDefaultDebugLabel)})";
                return true;
            });
        if ((a is null))
        {
            return new TextStyle(inherit: b!.inherit, color: Dart_uiLibrary.Color.lerp(null, ((TextStyle)b).color, t), backgroundColor: Dart_uiLibrary.Color.lerp(null, ((TextStyle)b).backgroundColor, t), fontSize: ((t < 0.5) ? null : ((TextStyle)b).fontSize), fontWeight: Dart_uiLibrary.FontWeight.lerp(null, ((TextStyle)b).fontWeight, t), fontStyle: ((t < 0.5) ? null : ((TextStyle)b).fontStyle), letterSpacing: ((t < 0.5) ? null : ((TextStyle)b).letterSpacing), wordSpacing: ((t < 0.5) ? null : ((TextStyle)b).wordSpacing), textBaseline: ((t < 0.5) ? null : ((TextStyle)b).textBaseline), height: ((t < 0.5) ? null : ((TextStyle)b).height), leadingDistribution: ((t < 0.5) ? null : ((TextStyle)b).leadingDistribution), locale: ((t < 0.5) ? null : ((TextStyle)b).locale), foreground: ((t < 0.5) ? null : ((TextStyle)b).foreground), background: ((t < 0.5) ? null : ((TextStyle)b).background), shadows: ((t < 0.5) ? null : ((TextStyle)b).shadows), fontFeatures: ((t < 0.5) ? null : ((TextStyle)b).fontFeatures), fontVariations: Text_styleLibrary.lerpFontVariations(null, ((TextStyle)b).fontVariations, t), decoration: ((t < 0.5) ? null : ((TextStyle)b).decoration), decorationColor: Dart_uiLibrary.Color.lerp(null, ((TextStyle)b).decorationColor, t), decorationStyle: ((t < 0.5) ? null : ((TextStyle)b).decorationStyle), decorationThickness: ((t < 0.5) ? null : ((TextStyle)b).decorationThickness), debugLabel: lerpDebugLabel__47582, fontFamily: ((t < 0.5) ? null : ((TextStyle)b)._fontFamily), fontFamilyFallback: ((t < 0.5) ? null : ((TextStyle)b)._fontFamilyFallback), package: ((t < 0.5) ? null : ((TextStyle)b)._package), overflow: ((t < 0.5) ? null : ((TextStyle)b).overflow));
        }
        if ((b is null))
        {
            return new TextStyle(inherit: ((TextStyle)a).inherit, color: Dart_uiLibrary.Color.lerp(((TextStyle)a).color, null, t), backgroundColor: Dart_uiLibrary.Color.lerp(null, ((TextStyle)a).backgroundColor, t), fontSize: ((t < 0.5) ? ((TextStyle)a).fontSize : null), fontWeight: Dart_uiLibrary.FontWeight.lerp(((TextStyle)a).fontWeight, null, t), fontStyle: ((t < 0.5) ? ((TextStyle)a).fontStyle : null), letterSpacing: ((t < 0.5) ? ((TextStyle)a).letterSpacing : null), wordSpacing: ((t < 0.5) ? ((TextStyle)a).wordSpacing : null), textBaseline: ((t < 0.5) ? ((TextStyle)a).textBaseline : null), height: ((t < 0.5) ? ((TextStyle)a).height : null), leadingDistribution: ((t < 0.5) ? ((TextStyle)a).leadingDistribution : null), locale: ((t < 0.5) ? ((TextStyle)a).locale : null), foreground: ((t < 0.5) ? ((TextStyle)a).foreground : null), background: ((t < 0.5) ? ((TextStyle)a).background : null), shadows: ((t < 0.5) ? ((TextStyle)a).shadows : null), fontFeatures: ((t < 0.5) ? ((TextStyle)a).fontFeatures : null), fontVariations: Text_styleLibrary.lerpFontVariations(((TextStyle)a).fontVariations, null, t), decoration: ((t < 0.5) ? ((TextStyle)a).decoration : null), decorationColor: Dart_uiLibrary.Color.lerp(((TextStyle)a).decorationColor, null, t), decorationStyle: ((t < 0.5) ? ((TextStyle)a).decorationStyle : null), decorationThickness: ((t < 0.5) ? ((TextStyle)a).decorationThickness : null), debugLabel: lerpDebugLabel__47582, fontFamily: ((t < 0.5) ? ((TextStyle)a)._fontFamily : null), fontFamilyFallback: ((t < 0.5) ? ((TextStyle)a)._fontFamilyFallback : null), package: ((t < 0.5) ? ((TextStyle)a)._package : null), overflow: ((t < 0.5) ? ((TextStyle)a).overflow : null));
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if ((((TextStyle)a).inherit == ((TextStyle)b).inherit))
                {
                    return true;
                }
                var nullFields__50773 = new List<string>();
                if ((checked((long)(nullFields__50773.Count)) == 0))
                {
                    return true;
                }
                throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Failed to interpolate TextStyles with different inherit values."), new ErrorSpacer(), new ErrorDescription("The TextStyles being interpolated were:"), ((Diagnosticable)a).toDiagnosticsNode(name: "from", style: DiagnosticsTreeStyle.singleLine), ((Diagnosticable)b).toDiagnosticsNode(name: "to", style: DiagnosticsTreeStyle.singleLine), new ErrorDescription("The following fields are unspecified in both TextStyles:\n" + $"{string.Join(", ", nullFields__50773.map<string, string>(((name) => $"\"{name}\"")))}.\n" + "When \"inherit\" changes during the transition, these fields may " + "observe abrupt value changes as a result, causing \"jump\"s in the " + "transition."), new ErrorSpacer(), new ErrorHint("In general, TextStyle.lerp only works well when both TextStyles have " + "the same \"inherit\" value, and specify the same fields."), new ErrorHint("If the TextStyles were directly created by you, consider bringing " + "them to parity to ensure a smooth transition."), new ErrorSpacer(), new ErrorHint("If one of the TextStyles being lerped is significantly more elaborate " + "than the other, and has \"inherited\" set to false, it is often because " + "it is merged with another TextStyle before being lerped. Comparing " + "the \"debugLabel\"s of the two TextStyles may help identify if that was " + "the case."), new ErrorHint("For example, you may see this error message when trying to lerp " + "between \"ThemeData()\" and \"Theme.of(context)\". This is because " + "TextStyles from \"Theme.of(context)\" are merged with TextStyles from " + "another theme and thus are more elaborate than the TextStyles from " + "\"ThemeData()\" (which is reflected in their \"debugLabel\"s -- " + "TextStyles from \"Theme.of(context)\" should have labels in the form of " + "\"(<A TextStyle>).merge(<Another TextStyle>)\"). It is recommended to " + "only lerp ThemeData with matching TextStyles.") });
            });
        return new TextStyle(inherit: ((t < 0.5) ? ((TextStyle)a).inherit : ((TextStyle)b).inherit), color: (((((TextStyle)a).foreground is null) && (((TextStyle)b).foreground is null)) ? Dart_uiLibrary.Color.lerp(((TextStyle)a).color, ((TextStyle)b).color, t) : null), backgroundColor: (((((TextStyle)a).background is null) && (((TextStyle)b).background is null)) ? Dart_uiLibrary.Color.lerp(((TextStyle)a).backgroundColor, ((TextStyle)b).backgroundColor, t) : null), fontSize: Dart_uiLibrary.lerpDouble((((TextStyle)a).fontSize ?? ((TextStyle)b).fontSize), (((TextStyle)b).fontSize ?? ((TextStyle)a).fontSize), t), fontWeight: Dart_uiLibrary.FontWeight.lerp(((TextStyle)a).fontWeight, ((TextStyle)b).fontWeight, t), fontStyle: ((t < 0.5) ? ((TextStyle)a).fontStyle : ((TextStyle)b).fontStyle), letterSpacing: Dart_uiLibrary.lerpDouble((((TextStyle)a).letterSpacing ?? ((TextStyle)b).letterSpacing), (((TextStyle)b).letterSpacing ?? ((TextStyle)a).letterSpacing), t), wordSpacing: Dart_uiLibrary.lerpDouble((((TextStyle)a).wordSpacing ?? ((TextStyle)b).wordSpacing), (((TextStyle)b).wordSpacing ?? ((TextStyle)a).wordSpacing), t), textBaseline: ((t < 0.5) ? ((TextStyle)a).textBaseline : ((TextStyle)b).textBaseline), height: Dart_uiLibrary.lerpDouble((((TextStyle)a).height ?? ((TextStyle)b).height), (((TextStyle)b).height ?? ((TextStyle)a).height), t), leadingDistribution: ((t < 0.5) ? ((TextStyle)a).leadingDistribution : ((TextStyle)b).leadingDistribution), locale: ((t < 0.5) ? ((TextStyle)a).locale : ((TextStyle)b).locale), foreground: ((((((TextStyle)a).foreground is not null) || (((TextStyle)b).foreground is not null))) ? ((t < 0.5) ? (((TextStyle)a).foreground ?? (((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((TextStyle)a).color!;
    return __cascade;
}))())) : (((TextStyle)b).foreground ?? (((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((TextStyle)b).color!;
    return __cascade;
}))()))) : null), background: ((((((TextStyle)a).background is not null) || (((TextStyle)b).background is not null))) ? ((t < 0.5) ? (((TextStyle)a).background ?? (((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((TextStyle)a).backgroundColor!;
    return __cascade;
}))())) : (((TextStyle)b).background ?? (((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = ((TextStyle)b).backgroundColor!;
    return __cascade;
}))()))) : null), shadows: Dart_uiLibrary.Shadow.lerpList(((TextStyle)a).shadows, ((TextStyle)b).shadows, t), fontFeatures: ((t < 0.5) ? ((TextStyle)a).fontFeatures : ((TextStyle)b).fontFeatures), fontVariations: Text_styleLibrary.lerpFontVariations(((TextStyle)a).fontVariations, ((TextStyle)b).fontVariations, t), decoration: ((t < 0.5) ? ((TextStyle)a).decoration : ((TextStyle)b).decoration), decorationColor: Dart_uiLibrary.Color.lerp(((TextStyle)a).decorationColor, ((TextStyle)b).decorationColor, t), decorationStyle: ((t < 0.5) ? ((TextStyle)a).decorationStyle : ((TextStyle)b).decorationStyle), decorationThickness: Dart_uiLibrary.lerpDouble((((TextStyle)a).decorationThickness ?? ((TextStyle)b).decorationThickness), (((TextStyle)b).decorationThickness ?? ((TextStyle)a).decorationThickness), t), debugLabel: lerpDebugLabel__47582, fontFamily: ((t < 0.5) ? ((TextStyle)a)._fontFamily : ((TextStyle)b)._fontFamily), fontFamilyFallback: ((t < 0.5) ? ((TextStyle)a)._fontFamilyFallback : ((TextStyle)b)._fontFamilyFallback), package: ((t < 0.5) ? ((TextStyle)a)._package : ((TextStyle)b)._package), overflow: ((t < 0.5) ? ((TextStyle)a).overflow : ((TextStyle)b).overflow));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.TextStyle getTextStyle(double textScaleFactor = 1.0, TextScaler textScaler = default!)
    {
        textScaler ??= TextScaler.noScaling;
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.Identical(textScaler, TextScaler.noScaling) || (textScaleFactor == 1.0)));
        double? fontSize__56839 = (this.fontSize switch { null => null, double size__56914 when (object.Equals(textScaler, TextScaler.noScaling)) => (size__56914 * textScaleFactor), double size__57005 => textScaler.scale(size__57005) });
        return new global::Doroti.Ui.TextStyle(color: this.color, decoration: this.decoration, decorationColor: this.decorationColor, decorationStyle: this.decorationStyle, decorationThickness: this.decorationThickness, fontWeight: this.fontWeight, fontStyle: this.fontStyle, textBaseline: this.textBaseline, leadingDistribution: this.leadingDistribution, fontFamily: this.fontFamily, fontFamilyFallback: this.fontFamilyFallback, fontSize: fontSize__56839, letterSpacing: this.letterSpacing, wordSpacing: this.wordSpacing, height: this.height, locale: this.locale, foreground: this.foreground, background: ((this.background, this.backgroundColor) switch
        {
            (global::Doroti.Ui.Paint paint__57711, _) => paint__57711,
            (_, global::Doroti.Ui.Color color__57755) => ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.color = color__57755;
    return __cascade;
}))(),
            _ => null
        }), shadows: this.shadows, fontFeatures: this.fontFeatures, fontVariations: this.fontVariations);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.ParagraphStyle getParagraphStyle(TextAlign? textAlign = null, TextDirection? textDirection = null, TextScaler textScaler = default!, string? ellipsis = null, long? maxLines = null, TextHeightBehavior? textHeightBehavior = null, Locale? locale = null, string? fontFamily = null, double? fontSize = null, FontWeight? fontWeight = null, FontStyle? fontStyle = null, double? height = null, StrutStyle? strutStyle = null)
    {
        textScaler ??= TextScaler.noScaling;
        DartRuntimePrimitives.Assert(() => ((maxLines is null) || (DartRuntimePrimitives.RequireValue(maxLines) > 0L)));
        DartRuntimePrimitives.Assert(() => ((height is null) || !double.IsNaN(DartRuntimePrimitives.RequireValue(height))));
        global::Doroti.Ui.TextLeadingDistribution? leadingDistribution__58864 = this.leadingDistribution;
        global::Doroti.Ui.TextHeightBehavior? effectiveTextHeightBehavior__58942 = (textHeightBehavior ?? (((leadingDistribution__58864 is null) ? null : new global::Doroti.Ui.TextHeightBehavior(leadingDistribution: DartRuntimePrimitives.RequireValue(leadingDistribution__58864)))));
        return new global::Doroti.Ui.ParagraphStyle(textAlign: textAlign, textDirection: textDirection, fontWeight: (fontWeight ?? this.fontWeight), fontStyle: (fontStyle ?? this.fontStyle), fontFamily: (fontFamily ?? this.fontFamily), fontSize: textScaler.scale(((fontSize ?? this.fontSize) ?? global::Doroti.Generated.Framework.Painting.Text_painterLibrary.kDefaultFontSize)), height: (height ?? this.height), textHeightBehavior: effectiveTextHeightBehavior__58942, strutStyle: ((strutStyle is null) ? null : new global::Doroti.Ui.StrutStyle(fontFamily: ((StrutStyle)strutStyle).fontFamily, fontFamilyFallback: ((StrutStyle)strutStyle).fontFamilyFallback, fontSize: (((StrutStyle)strutStyle).fontSize switch { null => null, double unscaled__59989 => textScaler.scale(unscaled__59989) }), height: ((StrutStyle)strutStyle).height, leading: ((StrutStyle)strutStyle).leading, leadingDistribution: ((StrutStyle)strutStyle).leadingDistribution, fontWeight: ((StrutStyle)strutStyle).fontWeight, fontStyle: ((StrutStyle)strutStyle).fontStyle, forceStrutHeight: ((StrutStyle)strutStyle).forceStrutHeight)), maxLines: maxLines, ellipsis: ellipsis, locale: locale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual RenderComparison compareTo(TextStyle other)
    {
        if (DartRuntimePrimitives.Identical(this, other))
        {
            return RenderComparison.identical;
        }
        if (((((((((((((((((((this.inherit != ((TextStyle)other).inherit) || (this.fontFamily != ((TextStyle)other).fontFamily)) || (this.fontSize != ((TextStyle)other).fontSize)) || (!object.Equals(this.fontWeight, ((TextStyle)other).fontWeight))) || (!object.Equals(this.fontStyle, ((TextStyle)other).fontStyle))) || (this.letterSpacing != ((TextStyle)other).letterSpacing)) || (this.wordSpacing != ((TextStyle)other).wordSpacing)) || (!object.Equals(this.textBaseline, ((TextStyle)other).textBaseline))) || (this.height != ((TextStyle)other).height)) || (!object.Equals(this.leadingDistribution, ((TextStyle)other).leadingDistribution))) || (!object.Equals(this.locale, ((TextStyle)other).locale))) || (!object.Equals(this.foreground, ((TextStyle)other).foreground))) || (!object.Equals(this.background, ((TextStyle)other).background))) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(this.shadows, ((TextStyle)other).shadows)) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(this.fontFeatures, ((TextStyle)other).fontFeatures)) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(this.fontVariations, ((TextStyle)other).fontVariations)) || !global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(this.fontFamilyFallback, ((TextStyle)other).fontFamilyFallback)) || (!object.Equals(this.overflow, ((TextStyle)other).overflow))))
        {
            return RenderComparison.layout;
        }
        if (((((((!object.Equals(this.color, ((TextStyle)other).color)) || (!object.Equals(this.backgroundColor, ((TextStyle)other).backgroundColor))) || (!object.Equals(this.decoration, ((TextStyle)other).decoration))) || (!object.Equals(this.decorationColor, ((TextStyle)other).decorationColor))) || (!object.Equals(this.decorationStyle, ((TextStyle)other).decorationStyle))) || (this.decorationThickness != ((TextStyle)other).decorationThickness)))
        {
            return RenderComparison.paint;
        }
        return RenderComparison.identical;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as TextStyle;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((((((((((((__other is TextStyle) && (((TextStyle)((TextStyle)__other)).inherit == this.inherit)) && (object.Equals(((TextStyle)((TextStyle)__other)).color, this.color))) && (object.Equals(((TextStyle)((TextStyle)__other)).backgroundColor, this.backgroundColor))) && (((TextStyle)((TextStyle)__other)).fontSize == this.fontSize)) && (object.Equals(((TextStyle)((TextStyle)__other)).fontWeight, this.fontWeight))) && (object.Equals(((TextStyle)((TextStyle)__other)).fontStyle, this.fontStyle))) && (((TextStyle)((TextStyle)__other)).letterSpacing == this.letterSpacing)) && (((TextStyle)((TextStyle)__other)).wordSpacing == this.wordSpacing)) && (object.Equals(((TextStyle)((TextStyle)__other)).textBaseline, this.textBaseline))) && (((TextStyle)((TextStyle)__other)).height == this.height)) && (object.Equals(((TextStyle)((TextStyle)__other)).leadingDistribution, this.leadingDistribution))) && (object.Equals(((TextStyle)((TextStyle)__other)).locale, this.locale))) && (object.Equals(((TextStyle)((TextStyle)__other)).foreground, this.foreground))) && (object.Equals(((TextStyle)((TextStyle)__other)).background, this.background))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((TextStyle)((TextStyle)__other)).shadows, this.shadows)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((TextStyle)((TextStyle)__other)).fontFeatures, this.fontFeatures)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((TextStyle)((TextStyle)__other)).fontVariations, this.fontVariations)) && (object.Equals(((TextStyle)((TextStyle)__other)).decoration, this.decoration))) && (object.Equals(((TextStyle)((TextStyle)__other)).decorationColor, this.decorationColor))) && (object.Equals(((TextStyle)((TextStyle)__other)).decorationStyle, this.decorationStyle))) && (((TextStyle)((TextStyle)__other)).decorationThickness == this.decorationThickness)) && (((TextStyle)((TextStyle)__other)).fontFamily == this.fontFamily)) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals(((TextStyle)((TextStyle)__other)).fontFamilyFallback, this.fontFamilyFallback)) && (((TextStyle)((TextStyle)__other))._package == this._package)) && (object.Equals(((TextStyle)((TextStyle)__other)).overflow, this.overflow)));
    }

    public override int GetHashCode()
    {
        List<string>? fontFamilyFallback__63474 = this.fontFamilyFallback;
        long fontHash__63534 = FoundationRuntimePorts.ObjectHash(this.decorationStyle, this.decorationThickness, this.fontFamily, ((fontFamilyFallback__63474 is null) ? null : FoundationRuntimePorts.ObjectHashAll(fontFamilyFallback__63474)), this._package, this.overflow);
        List<global::Doroti.Ui.Shadow>? shadows__63768 = this.shadows;
        List<global::Doroti.Ui.FontFeature>? fontFeatures__63821 = this.fontFeatures;
        List<global::Doroti.Ui.FontVariation>? fontVariations__63886 = this.fontVariations;
        return FoundationRuntimePorts.ObjectHash(this.inherit, this.color, this.backgroundColor, this.fontSize, this.fontWeight, this.fontStyle, this.letterSpacing, this.wordSpacing, this.textBaseline, this.height, this.leadingDistribution, this.locale, this.foreground, this.background, ((shadows__63768 is null) ? null : FoundationRuntimePorts.ObjectHashAll(shadows__63768)), ((fontFeatures__63821 is null) ? null : FoundationRuntimePorts.ObjectHashAll(fontFeatures__63821)), ((fontVariations__63886 is null) ? null : FoundationRuntimePorts.ObjectHashAll(fontVariations__63886)), this.decoration, this.decorationColor, fontHash__63534);
        return default!;
    }
    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "TextStyle");
    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties, string prefix = "")
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        if ((this.debugLabel is not null))
        {
            properties.add(new MessageProperty($"{prefix}debugLabel", this.debugLabel!));
        }
        var styles__64887 = new List<DiagnosticsNode> { new ColorProperty($"{prefix}color", this.color, defaultValue: null), new ColorProperty($"{prefix}backgroundColor", this.backgroundColor, defaultValue: null), new StringProperty($"{prefix}family", this.fontFamily, defaultValue: null, quoted: false), new IterableProperty<string>($"{prefix}familyFallback", this.fontFamilyFallback, defaultValue: null), new DoubleProperty($"{prefix}size", this.fontSize, defaultValue: null) };
        string? weightDescription__65342 = default!;
        if ((this.fontWeight is not null))
        {
            weightDescription__65342 = $"{(FoundationRuntimePorts.EnumIndex(this.fontWeight!) + 1L)}00";
        }
        styles__64887.Add(new DiagnosticsProperty<global::Doroti.Ui.FontWeight>($"{prefix}weight", this.fontWeight, description: weightDescription__65342, defaultValue: null));
        styles__64887.Add(new EnumProperty<global::Doroti.Ui.FontStyle>($"{prefix}style", this.fontStyle, defaultValue: null));
        styles__64887.Add(new DoubleProperty($"{prefix}letterSpacing", this.letterSpacing, defaultValue: null));
        styles__64887.Add(new DoubleProperty($"{prefix}wordSpacing", this.wordSpacing, defaultValue: null));
        styles__64887.Add(new EnumProperty<global::Doroti.Ui.TextBaseline>($"{prefix}baseline", this.textBaseline, defaultValue: null));
        styles__64887.Add(new DoubleProperty($"{prefix}height", this.height, unit: "x", defaultValue: null));
        styles__64887.Add(new EnumProperty<global::Doroti.Ui.TextLeadingDistribution>($"{prefix}leadingDistribution", this.leadingDistribution, defaultValue: null));
        styles__64887.Add(new DiagnosticsProperty<global::Doroti.Ui.Locale>($"{prefix}locale", this.locale, defaultValue: null));
        styles__64887.Add(new DiagnosticsProperty<global::Doroti.Ui.Paint>($"{prefix}foreground", this.foreground, defaultValue: null));
        styles__64887.Add(new DiagnosticsProperty<global::Doroti.Ui.Paint>($"{prefix}background", this.background, defaultValue: null));
        if (((((this.decoration is not null) || (this.decorationColor is not null)) || (this.decorationStyle is not null)) || (this.decorationThickness is not null)))
        {
            var decorationDescription__66918 = new List<string>();
            if ((this.decorationStyle is not null))
            {
                TextDecorationStyle decorationStyle__value66964 = DartRuntimePrimitives.RequireValue(decorationStyle);
                decorationDescription__66918.Add(DartRuntimePrimitives.RequireValue(this.decorationStyle).ToString());
            }
            styles__64887.Add(new ColorProperty($"{prefix}decorationColor", this.decorationColor, defaultValue: null, level: DiagnosticLevel.fine));
            if ((this.decorationColor is not null))
            {
                decorationDescription__66918.Add($"{this.decorationColor}");
            }
            styles__64887.Add(new DiagnosticsProperty<global::Doroti.Ui.TextDecoration>($"{prefix}decoration", this.decoration, defaultValue: null, level: DiagnosticLevel.hidden));
            if ((this.decoration is not null))
            {
                decorationDescription__66918.Add($"{this.decoration}");
            }
            DartRuntimePrimitives.Assert(() => (checked((long)(decorationDescription__66918.Count)) != 0));
            styles__64887.Add(new MessageProperty($"{prefix}decoration", string.Join(" ", decorationDescription__66918)));
            styles__64887.Add(new DoubleProperty($"{prefix}decorationThickness", this.decorationThickness, unit: "x", defaultValue: null));
        }
        bool styleSpecified__68317 = styles__64887.any(((n) => !n.isFiltered(DiagnosticLevel.info)));
        properties.add(new DiagnosticsProperty<bool>($"{prefix}inherit", this.inherit, level: (((!styleSpecified__68317 && this.inherit)) ? DiagnosticLevel.fine : DiagnosticLevel.info)));
        styles__64887.forEach(properties.add);
        if (!styleSpecified__68317)
        {
            properties.add(new FlagProperty("inherit", value: this.inherit, ifTrue: $"{prefix}<all styles inherited>", ifFalse: $"{prefix}<no style specified>"));
        }
        styles__64887.Add(new EnumProperty<TextOverflow>($"{prefix}overflow", this.overflow, defaultValue: null));
    }

}

public static partial class Text_styleLibrary
{
    public static List<FontVariation>? lerpFontVariations(List<FontVariation>? a, List<FontVariation>? b, double t)
    {
        if ((t == 0.0))
        {
            return a;
        }
        if ((t == 1.0))
        {
            return b;
        }
        if (((((a is null) || (checked((long)(a.Count)) == 0)) || (b is null)) || (checked((long)(b.Count)) == 0)))
        {
            return ((t < 0.5) ? a : b);
        }
        DartRuntimePrimitives.Assert(() => ((checked((long)(a.Count)) != 0) && (checked((long)(b.Count)) != 0)));
        var result__71534 = new List<global::Doroti.Ui.FontVariation>();
        var index__71661 = 0L;
        long minLength__71684 = ((checked((long)(a.Count)) < checked((long)(b.Count))) ? checked((long)(a.Count)) : checked((long)(b.Count)));
        for (; (index__71661 < minLength__71684); index__71661 += 1L)
        {
            if ((a[(int)(index__71661)].axis != b[(int)(index__71661)].axis))
            {
                break;
            }
            result__71534.Add(Dart_uiLibrary.FontVariation.lerp(a[(int)(index__71661)], b[(int)(index__71661)], t)!);
        }
        long maxLength__71956 = ((checked((long)(a.Count)) > checked((long)(b.Count))) ? checked((long)(a.Count)) : checked((long)(b.Count)));
        if ((index__71661 < maxLength__71956))
        {
            HashSet<string> axes__72158 = new HashSet<string>();
            DartMap<string, global::Doroti.Ui.FontVariation> aVariations__72221 = new DartMap<string, global::Doroti.Ui.FontVariation>().cast<string, global::Doroti.Ui.FontVariation>();
            for (var indexA__72282 = index__71661; (indexA__72282 < checked((long)(a.Count))); indexA__72282 += 1L)
            {
                aVariations__72221[a[(int)(indexA__72282)].axis] = a[(int)(indexA__72282)];
                axes__72158.Add(a[(int)(indexA__72282)].axis);
            }
            DartMap<string, global::Doroti.Ui.FontVariation> bVariations__72454 = new DartMap<string, global::Doroti.Ui.FontVariation>().cast<string, global::Doroti.Ui.FontVariation>();
            for (var indexB__72515 = index__71661; (indexB__72515 < checked((long)(b.Count))); indexB__72515 += 1L)
            {
                bVariations__72454[b[(int)(indexB__72515)].axis] = b[(int)(indexB__72515)];
                axes__72158.Add(b[(int)(indexB__72515)].axis);
            }
            foreach (var axis__72665 in axes__72158)
            {
                global::Doroti.Ui.FontVariation? variation__72708 = Dart_uiLibrary.FontVariation.lerp(aVariations__72221.GetValueOrDefault(axis__72665), bVariations__72454.GetValueOrDefault(axis__72665), t);
                if ((variation__72708 is not null))
                {
                    result__71534.Add(variation__72708);
                }
            }
        }
        return result__71534;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
