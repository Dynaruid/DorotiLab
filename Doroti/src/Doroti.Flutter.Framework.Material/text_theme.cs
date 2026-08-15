// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/text_theme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public class TextTheme : global::Doroti.Generated.Framework.Foundation.Diagnosticable
{
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? displayLarge { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? displayMedium { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? displaySmall { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? headlineLarge { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? headlineMedium { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? headlineSmall { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleLarge { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleMedium { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? titleSmall { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? bodyLarge { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? bodyMedium { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? bodySmall { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? labelLarge { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? labelMedium { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? labelSmall { get; private set; }

    public TextTheme(global::Doroti.Generated.Framework.Painting.TextStyle? displayLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? displayMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? displaySmall = null, global::Doroti.Generated.Framework.Painting.TextStyle? headlineLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? headlineMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? headlineSmall = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleSmall = null, global::Doroti.Generated.Framework.Painting.TextStyle? bodyLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? bodyMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? bodySmall = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelSmall = null)
    {
        this.displayLarge = displayLarge;
        this.displayMedium = displayMedium;
        this.displaySmall = displaySmall;
        this.headlineLarge = headlineLarge;
        this.headlineMedium = headlineMedium;
        this.headlineSmall = headlineSmall;
        this.titleLarge = titleLarge;
        this.titleMedium = titleMedium;
        this.titleSmall = titleSmall;
        this.bodyLarge = bodyLarge;
        this.bodyMedium = bodyMedium;
        this.bodySmall = bodySmall;
        this.labelLarge = labelLarge;
        this.labelMedium = labelMedium;
        this.labelSmall = labelSmall;
    }

    public virtual TextTheme copyWith(global::Doroti.Generated.Framework.Painting.TextStyle? displayLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? displayMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? displaySmall = null, global::Doroti.Generated.Framework.Painting.TextStyle? headlineLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? headlineMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? headlineSmall = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? titleSmall = null, global::Doroti.Generated.Framework.Painting.TextStyle? bodyLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? bodyMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? bodySmall = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelLarge = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelMedium = null, global::Doroti.Generated.Framework.Painting.TextStyle? labelSmall = null)
    {
        return new TextTheme(displayLarge: (displayLarge ?? this.displayLarge), displayMedium: (displayMedium ?? this.displayMedium), displaySmall: (displaySmall ?? this.displaySmall), headlineLarge: (headlineLarge ?? this.headlineLarge), headlineMedium: (headlineMedium ?? this.headlineMedium), headlineSmall: (headlineSmall ?? this.headlineSmall), titleLarge: (titleLarge ?? this.titleLarge), titleMedium: (titleMedium ?? this.titleMedium), titleSmall: (titleSmall ?? this.titleSmall), bodyLarge: (bodyLarge ?? this.bodyLarge), bodyMedium: (bodyMedium ?? this.bodyMedium), bodySmall: (bodySmall ?? this.bodySmall), labelLarge: (labelLarge ?? this.labelLarge), labelMedium: (labelMedium ?? this.labelMedium), labelSmall: (labelSmall ?? this.labelSmall));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextTheme merge(TextTheme? other)
    {
        if ((other is null))
        {
            return this;
        }
        return ((TextTheme)(object?)copyWith(displayLarge: (this.displayLarge?.merge(((TextTheme)other).displayLarge) ?? ((TextTheme)other).displayLarge), displayMedium: (this.displayMedium?.merge(((TextTheme)other).displayMedium) ?? ((TextTheme)other).displayMedium), displaySmall: (this.displaySmall?.merge(((TextTheme)other).displaySmall) ?? ((TextTheme)other).displaySmall), headlineLarge: (this.headlineLarge?.merge(((TextTheme)other).headlineLarge) ?? ((TextTheme)other).headlineLarge), headlineMedium: (this.headlineMedium?.merge(((TextTheme)other).headlineMedium) ?? ((TextTheme)other).headlineMedium), headlineSmall: (this.headlineSmall?.merge(((TextTheme)other).headlineSmall) ?? ((TextTheme)other).headlineSmall), titleLarge: (this.titleLarge?.merge(((TextTheme)other).titleLarge) ?? ((TextTheme)other).titleLarge), titleMedium: (this.titleMedium?.merge(((TextTheme)other).titleMedium) ?? ((TextTheme)other).titleMedium), titleSmall: (this.titleSmall?.merge(((TextTheme)other).titleSmall) ?? ((TextTheme)other).titleSmall), bodyLarge: (this.bodyLarge?.merge(((TextTheme)other).bodyLarge) ?? ((TextTheme)other).bodyLarge), bodyMedium: (this.bodyMedium?.merge(((TextTheme)other).bodyMedium) ?? ((TextTheme)other).bodyMedium), bodySmall: (this.bodySmall?.merge(((TextTheme)other).bodySmall) ?? ((TextTheme)other).bodySmall), labelLarge: (this.labelLarge?.merge(((TextTheme)other).labelLarge) ?? ((TextTheme)other).labelLarge), labelMedium: (this.labelMedium?.merge(((TextTheme)other).labelMedium) ?? ((TextTheme)other).labelMedium), labelSmall: (this.labelSmall?.merge(((TextTheme)other).labelSmall) ?? ((TextTheme)other).labelSmall)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextTheme apply(string? fontFamily = null, List<string>? fontFamilyFallback = null, string? package = null, double fontSizeFactor = 1.0, double fontSizeDelta = 0.0, double letterSpacingFactor = 1.0, double letterSpacingDelta = 0.0, double wordSpacingFactor = 1.0, double wordSpacingDelta = 0.0, double heightFactor = 1.0, double heightDelta = 0.0, Color? displayColor = null, Color? bodyColor = null, TextDecoration? decoration = null, Color? decorationColor = null, TextDecorationStyle? decorationStyle = null)
    {
        return new TextTheme(displayLarge: this.displayLarge?.apply(color: displayColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), displayMedium: this.displayMedium?.apply(color: displayColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), displaySmall: this.displaySmall?.apply(color: displayColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), headlineLarge: this.headlineLarge?.apply(color: displayColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), headlineMedium: this.headlineMedium?.apply(color: displayColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), headlineSmall: this.headlineSmall?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), titleLarge: this.titleLarge?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), titleMedium: this.titleMedium?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), titleSmall: this.titleSmall?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), bodyLarge: this.bodyLarge?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), bodyMedium: this.bodyMedium?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), bodySmall: this.bodySmall?.apply(color: displayColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), labelLarge: this.labelLarge?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), labelMedium: this.labelMedium?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package), labelSmall: this.labelSmall?.apply(color: bodyColor, decoration: decoration, decorationColor: decorationColor, decorationStyle: decorationStyle, fontFamily: fontFamily, fontFamilyFallback: fontFamilyFallback, fontSizeFactor: fontSizeFactor, fontSizeDelta: fontSizeDelta, letterSpacingDelta: letterSpacingDelta, letterSpacingFactor: letterSpacingFactor, wordSpacingDelta: wordSpacingDelta, wordSpacingFactor: wordSpacingFactor, heightFactor: heightFactor, heightDelta: heightDelta, package: package));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextTheme lerp(TextTheme? a, TextTheme? b, double t)
    {
        if ((DartRuntimePrimitives.Identical(a, b) && (a is not null)))
        {
            return a;
        }
        return new TextTheme(displayLarge: TextStyle.lerp(a?.displayLarge, b?.displayLarge, t), displayMedium: TextStyle.lerp(a?.displayMedium, b?.displayMedium, t), displaySmall: TextStyle.lerp(a?.displaySmall, b?.displaySmall, t), headlineLarge: TextStyle.lerp(a?.headlineLarge, b?.headlineLarge, t), headlineMedium: TextStyle.lerp(a?.headlineMedium, b?.headlineMedium, t), headlineSmall: TextStyle.lerp(a?.headlineSmall, b?.headlineSmall, t), titleLarge: TextStyle.lerp(a?.titleLarge, b?.titleLarge, t), titleMedium: TextStyle.lerp(a?.titleMedium, b?.titleMedium, t), titleSmall: TextStyle.lerp(a?.titleSmall, b?.titleSmall, t), bodyLarge: TextStyle.lerp(a?.bodyLarge, b?.bodyLarge, t), bodyMedium: TextStyle.lerp(a?.bodyMedium, b?.bodyMedium, t), bodySmall: TextStyle.lerp(a?.bodySmall, b?.bodySmall, t), labelLarge: TextStyle.lerp(a?.labelLarge, b?.labelLarge, t), labelMedium: TextStyle.lerp(a?.labelMedium, b?.labelMedium, t), labelSmall: TextStyle.lerp(a?.labelSmall, b?.labelSmall, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextTheme of(global::Doroti.Generated.Framework.Widgets.BuildContext context) => Theme.of(context).textTheme;
    public static TextTheme primaryOf(global::Doroti.Generated.Framework.Widgets.BuildContext context) => Theme.of(context).primaryTextTheme;
    public override bool Equals(object? other)
    {
        var __other = other as TextTheme;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((((((((((((((((__other is TextTheme) && (object.Equals(this.displayLarge, ((TextTheme)((TextTheme)__other)).displayLarge))) && (object.Equals(this.displayMedium, ((TextTheme)((TextTheme)__other)).displayMedium))) && (object.Equals(this.displaySmall, ((TextTheme)((TextTheme)__other)).displaySmall))) && (object.Equals(this.headlineLarge, ((TextTheme)((TextTheme)__other)).headlineLarge))) && (object.Equals(this.headlineMedium, ((TextTheme)((TextTheme)__other)).headlineMedium))) && (object.Equals(this.headlineSmall, ((TextTheme)((TextTheme)__other)).headlineSmall))) && (object.Equals(this.titleLarge, ((TextTheme)((TextTheme)__other)).titleLarge))) && (object.Equals(this.titleMedium, ((TextTheme)((TextTheme)__other)).titleMedium))) && (object.Equals(this.titleSmall, ((TextTheme)((TextTheme)__other)).titleSmall))) && (object.Equals(this.bodyLarge, ((TextTheme)((TextTheme)__other)).bodyLarge))) && (object.Equals(this.bodyMedium, ((TextTheme)((TextTheme)__other)).bodyMedium))) && (object.Equals(this.bodySmall, ((TextTheme)((TextTheme)__other)).bodySmall))) && (object.Equals(this.labelLarge, ((TextTheme)((TextTheme)__other)).labelLarge))) && (object.Equals(this.labelMedium, ((TextTheme)((TextTheme)__other)).labelMedium))) && (object.Equals(this.labelSmall, ((TextTheme)((TextTheme)__other)).labelSmall)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.displayLarge, this.displayMedium, this.displaySmall, this.headlineLarge, this.headlineMedium, this.headlineSmall, this.titleLarge, this.titleMedium, this.titleSmall, this.bodyLarge, this.bodyMedium, this.bodySmall, this.labelLarge, this.labelMedium, this.labelSmall));
    public virtual void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        TextTheme defaultTheme__29959 = Typography.CreateMaterial2018(platform: global::Doroti.Generated.Framework.Foundation.PlatformLibrary.defaultTargetPlatform).black;
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("displayLarge", this.displayLarge, defaultValue: ((TextTheme)defaultTheme__29959).displayLarge));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("displayMedium", this.displayMedium, defaultValue: ((TextTheme)defaultTheme__29959).displayMedium));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("displaySmall", this.displaySmall, defaultValue: ((TextTheme)defaultTheme__29959).displaySmall));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("headlineLarge", this.headlineLarge, defaultValue: ((TextTheme)defaultTheme__29959).headlineLarge));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("headlineMedium", this.headlineMedium, defaultValue: ((TextTheme)defaultTheme__29959).headlineMedium));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("headlineSmall", this.headlineSmall, defaultValue: ((TextTheme)defaultTheme__29959).headlineSmall));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("titleLarge", this.titleLarge, defaultValue: ((TextTheme)defaultTheme__29959).titleLarge));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("titleMedium", this.titleMedium, defaultValue: ((TextTheme)defaultTheme__29959).titleMedium));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("titleSmall", this.titleSmall, defaultValue: ((TextTheme)defaultTheme__29959).titleSmall));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("bodyLarge", this.bodyLarge, defaultValue: ((TextTheme)defaultTheme__29959).bodyLarge));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("bodyMedium", this.bodyMedium, defaultValue: ((TextTheme)defaultTheme__29959).bodyMedium));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("bodySmall", this.bodySmall, defaultValue: ((TextTheme)defaultTheme__29959).bodySmall));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("labelLarge", this.labelLarge, defaultValue: ((TextTheme)defaultTheme__29959).labelLarge));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("labelMedium", this.labelMedium, defaultValue: ((TextTheme)defaultTheme__29959).labelMedium));
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Painting.TextStyle>("labelSmall", this.labelSmall, defaultValue: ((TextTheme)defaultTheme__29959).labelSmall));
    }

    public virtual string toStringShort() => global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
