// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/text_theme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class TextTheme : Diagnosticable
{
    public virtual TextStyle? displayLarge { get; private set; }
    public virtual TextStyle? displayMedium { get; private set; }
    public virtual TextStyle? displaySmall { get; private set; }
    public virtual TextStyle? headlineLarge { get; private set; }
    public virtual TextStyle? headlineMedium { get; private set; }
    public virtual TextStyle? headlineSmall { get; private set; }
    public virtual TextStyle? titleLarge { get; private set; }
    public virtual TextStyle? titleMedium { get; private set; }
    public virtual TextStyle? titleSmall { get; private set; }
    public virtual TextStyle? bodyLarge { get; private set; }
    public virtual TextStyle? bodyMedium { get; private set; }
    public virtual TextStyle? bodySmall { get; private set; }
    public virtual TextStyle? labelLarge { get; private set; }
    public virtual TextStyle? labelMedium { get; private set; }
    public virtual TextStyle? labelSmall { get; private set; }

    public TextTheme(
        TextStyle? displayLarge = null,
        TextStyle? displayMedium = null,
        TextStyle? displaySmall = null,
        TextStyle? headlineLarge = null,
        TextStyle? headlineMedium = null,
        TextStyle? headlineSmall = null,
        TextStyle? titleLarge = null,
        TextStyle? titleMedium = null,
        TextStyle? titleSmall = null,
        TextStyle? bodyLarge = null,
        TextStyle? bodyMedium = null,
        TextStyle? bodySmall = null,
        TextStyle? labelLarge = null,
        TextStyle? labelMedium = null,
        TextStyle? labelSmall = null
    )
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

    public virtual TextTheme copyWith(
        TextStyle? displayLarge = null,
        TextStyle? displayMedium = null,
        TextStyle? displaySmall = null,
        TextStyle? headlineLarge = null,
        TextStyle? headlineMedium = null,
        TextStyle? headlineSmall = null,
        TextStyle? titleLarge = null,
        TextStyle? titleMedium = null,
        TextStyle? titleSmall = null,
        TextStyle? bodyLarge = null,
        TextStyle? bodyMedium = null,
        TextStyle? bodySmall = null,
        TextStyle? labelLarge = null,
        TextStyle? labelMedium = null,
        TextStyle? labelSmall = null
    )
    {
        return new TextTheme(
            displayLarge: displayLarge ?? this.displayLarge,
            displayMedium: displayMedium ?? this.displayMedium,
            displaySmall: displaySmall ?? this.displaySmall,
            headlineLarge: headlineLarge ?? this.headlineLarge,
            headlineMedium: headlineMedium ?? this.headlineMedium,
            headlineSmall: headlineSmall ?? this.headlineSmall,
            titleLarge: titleLarge ?? this.titleLarge,
            titleMedium: titleMedium ?? this.titleMedium,
            titleSmall: titleSmall ?? this.titleSmall,
            bodyLarge: bodyLarge ?? this.bodyLarge,
            bodyMedium: bodyMedium ?? this.bodyMedium,
            bodySmall: bodySmall ?? this.bodySmall,
            labelLarge: labelLarge ?? this.labelLarge,
            labelMedium: labelMedium ?? this.labelMedium,
            labelSmall: labelSmall ?? this.labelSmall
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextTheme merge(TextTheme? other)
    {
        if (other is null)
        {
            return this;
        }
        return copyWith(
            displayLarge: displayLarge?.merge(other.displayLarge) ?? other.displayLarge,
            displayMedium: displayMedium?.merge(other.displayMedium) ?? other.displayMedium,
            displaySmall: displaySmall?.merge(other.displaySmall) ?? other.displaySmall,
            headlineLarge: headlineLarge?.merge(other.headlineLarge) ?? other.headlineLarge,
            headlineMedium: headlineMedium?.merge(other.headlineMedium) ?? other.headlineMedium,
            headlineSmall: headlineSmall?.merge(other.headlineSmall) ?? other.headlineSmall,
            titleLarge: titleLarge?.merge(other.titleLarge) ?? other.titleLarge,
            titleMedium: titleMedium?.merge(other.titleMedium) ?? other.titleMedium,
            titleSmall: titleSmall?.merge(other.titleSmall) ?? other.titleSmall,
            bodyLarge: bodyLarge?.merge(other.bodyLarge) ?? other.bodyLarge,
            bodyMedium: bodyMedium?.merge(other.bodyMedium) ?? other.bodyMedium,
            bodySmall: bodySmall?.merge(other.bodySmall) ?? other.bodySmall,
            labelLarge: labelLarge?.merge(other.labelLarge) ?? other.labelLarge,
            labelMedium: labelMedium?.merge(other.labelMedium) ?? other.labelMedium,
            labelSmall: labelSmall?.merge(other.labelSmall) ?? other.labelSmall
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TextTheme apply(
        string? fontFamily = null,
        List<string>? fontFamilyFallback = null,
        string? package = null,
        double fontSizeFactor = 1.0,
        double fontSizeDelta = 0.0,
        double letterSpacingFactor = 1.0,
        double letterSpacingDelta = 0.0,
        double wordSpacingFactor = 1.0,
        double wordSpacingDelta = 0.0,
        double heightFactor = 1.0,
        double heightDelta = 0.0,
        Color? displayColor = null,
        Color? bodyColor = null,
        TextDecoration? decoration = null,
        Color? decorationColor = null,
        TextDecorationStyle? decorationStyle = null
    )
    {
        return new TextTheme(
            displayLarge: displayLarge?.apply(
                color: displayColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            displayMedium: displayMedium?.apply(
                color: displayColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            displaySmall: displaySmall?.apply(
                color: displayColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            headlineLarge: headlineLarge?.apply(
                color: displayColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            headlineMedium: headlineMedium?.apply(
                color: displayColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            headlineSmall: headlineSmall?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            titleLarge: titleLarge?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            titleMedium: titleMedium?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            titleSmall: titleSmall?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            bodyLarge: bodyLarge?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            bodyMedium: bodyMedium?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            bodySmall: bodySmall?.apply(
                color: displayColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            labelLarge: labelLarge?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            labelMedium: labelMedium?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            ),
            labelSmall: labelSmall?.apply(
                color: bodyColor,
                decoration: decoration,
                decorationColor: decorationColor,
                decorationStyle: decorationStyle,
                fontFamily: fontFamily,
                fontFamilyFallback: fontFamilyFallback,
                fontSizeFactor: fontSizeFactor,
                fontSizeDelta: fontSizeDelta,
                letterSpacingDelta: letterSpacingDelta,
                letterSpacingFactor: letterSpacingFactor,
                wordSpacingDelta: wordSpacingDelta,
                wordSpacingFactor: wordSpacingFactor,
                heightFactor: heightFactor,
                heightDelta: heightDelta,
                package: package
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextTheme lerp(TextTheme? a, TextTheme? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b) && (a is not null))
        {
            return a;
        }
        return new TextTheme(
            displayLarge: TextStyle.lerp(a?.displayLarge, b?.displayLarge, t),
            displayMedium: TextStyle.lerp(a?.displayMedium, b?.displayMedium, t),
            displaySmall: TextStyle.lerp(a?.displaySmall, b?.displaySmall, t),
            headlineLarge: TextStyle.lerp(a?.headlineLarge, b?.headlineLarge, t),
            headlineMedium: TextStyle.lerp(a?.headlineMedium, b?.headlineMedium, t),
            headlineSmall: TextStyle.lerp(a?.headlineSmall, b?.headlineSmall, t),
            titleLarge: TextStyle.lerp(a?.titleLarge, b?.titleLarge, t),
            titleMedium: TextStyle.lerp(a?.titleMedium, b?.titleMedium, t),
            titleSmall: TextStyle.lerp(a?.titleSmall, b?.titleSmall, t),
            bodyLarge: TextStyle.lerp(a?.bodyLarge, b?.bodyLarge, t),
            bodyMedium: TextStyle.lerp(a?.bodyMedium, b?.bodyMedium, t),
            bodySmall: TextStyle.lerp(a?.bodySmall, b?.bodySmall, t),
            labelLarge: TextStyle.lerp(a?.labelLarge, b?.labelLarge, t),
            labelMedium: TextStyle.lerp(a?.labelMedium, b?.labelMedium, t),
            labelSmall: TextStyle.lerp(a?.labelSmall, b?.labelSmall, t)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static TextTheme of(BuildContext context) => Theme.of(context).textTheme;

    public static TextTheme primaryOf(BuildContext context) => Theme.of(context).primaryTextTheme;

    public override bool Equals(object? other)
    {
        var __other = other as TextTheme;
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
        return (__other is TextTheme)
            && Equals(displayLarge, __other.displayLarge)
            && Equals(displayMedium, __other.displayMedium)
            && Equals(displaySmall, __other.displaySmall)
            && Equals(headlineLarge, __other.headlineLarge)
            && Equals(headlineMedium, __other.headlineMedium)
            && Equals(headlineSmall, __other.headlineSmall)
            && Equals(titleLarge, __other.titleLarge)
            && Equals(titleMedium, __other.titleMedium)
            && Equals(titleSmall, __other.titleSmall)
            && Equals(bodyLarge, __other.bodyLarge)
            && Equals(bodyMedium, __other.bodyMedium)
            && Equals(bodySmall, __other.bodySmall)
            && Equals(labelLarge, __other.labelLarge)
            && Equals(labelMedium, __other.labelMedium)
            && Equals(labelSmall, __other.labelSmall);
    }

    public override int GetHashCode() =>
        DartRuntimePrimitives.ConvertValue<int>(
            FoundationRuntimePorts.ObjectHash(
                displayLarge,
                displayMedium,
                displaySmall,
                headlineLarge,
                headlineMedium,
                headlineSmall,
                titleLarge,
                titleMedium,
                titleSmall,
                bodyLarge,
                bodyMedium,
                bodySmall,
                labelLarge,
                labelMedium,
                labelSmall
            )
        );

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        TextTheme defaultTheme = Typography
            .CreateMaterial2021(platform: PlatformLibrary.defaultTargetPlatform)
            .black;
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "displayLarge",
                displayLarge,
                defaultValue: defaultTheme.displayLarge
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "displayMedium",
                displayMedium,
                defaultValue: defaultTheme.displayMedium
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "displaySmall",
                displaySmall,
                defaultValue: defaultTheme.displaySmall
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "headlineLarge",
                headlineLarge,
                defaultValue: defaultTheme.headlineLarge
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "headlineMedium",
                headlineMedium,
                defaultValue: defaultTheme.headlineMedium
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "headlineSmall",
                headlineSmall,
                defaultValue: defaultTheme.headlineSmall
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "titleLarge",
                titleLarge,
                defaultValue: defaultTheme.titleLarge
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "titleMedium",
                titleMedium,
                defaultValue: defaultTheme.titleMedium
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "titleSmall",
                titleSmall,
                defaultValue: defaultTheme.titleSmall
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "bodyLarge",
                bodyLarge,
                defaultValue: defaultTheme.bodyLarge
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "bodyMedium",
                bodyMedium,
                defaultValue: defaultTheme.bodyMedium
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "bodySmall",
                bodySmall,
                defaultValue: defaultTheme.bodySmall
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "labelLarge",
                labelLarge,
                defaultValue: defaultTheme.labelLarge
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "labelMedium",
                labelMedium,
                defaultValue: defaultTheme.labelMedium
            )
        );
        properties.add(
            new DiagnosticsProperty<TextStyle>(
                "labelSmall",
                labelSmall,
                defaultValue: defaultTheme.labelSmall
            )
        );
    }

    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
    {
        string? fullString = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                .toDiagnosticsNode()
                .toStringDeep(minLevel: minLevel);
            return true;
        });
        return fullString ?? toStringShort();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode toDiagnosticsNode(
        string? name = null,
        DiagnosticsTreeStyle? style = null
    )
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
