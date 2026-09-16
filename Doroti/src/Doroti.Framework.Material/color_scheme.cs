// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/color_scheme.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

internal sealed class QuantizerResult
{
    internal DartMap<long, long> colorToCount { get; } = new();
}

internal sealed class QuantizerCelebi
{
    internal async Future<QuantizerResult> quantize(dynamic pixels, long maxColors, bool returnInputPixelToClusterPixel = false)
    {
        var input = ((System.Collections.IEnumerable)(object)pixels).Cast<object>().Select(Convert.ToInt64).ToArray();
        var limit = checked((int)maxColors);
        // Quantization is CPU work and must not block the UI continuation which
        // receives an image while a lazily mounted section is scrolling into view.
        return await Task.Run(() =>
        {
            var result = new QuantizerResult();
            foreach (var entry in MaterialImageColorRuntime.Quantize(input, limit))
                result.colorToCount[entry.Key] = entry.Value;
            return result;
        });
    }
}

internal static class Score
{
    internal static IEnumerable<long> score(DartMap<long, long> colors, long desired = 1) =>
        MaterialImageColorRuntime.Score(colors.ToDictionary(entry => entry.Key, entry => entry.Value), checked((int)desired));
}

internal sealed class Hct
{
    internal long argb { get; }
    private Hct(long argb) => this.argb = argb;
    internal static Hct fromInt(long argb) => new(argb);
}

internal class DynamicScheme
{
    internal long seedArgb { get; }
    internal bool isDark { get; }
    internal double contrastLevel { get; }
    internal string variant { get; }
    internal DynamicScheme(Hct sourceColorHct, bool isDark, double contrastLevel, string variant)
    {
        seedArgb = sourceColorHct.argb;
        this.isDark = isDark;
        this.contrastLevel = contrastLevel;
        this.variant = variant;
    }
}

internal sealed class SchemeTonalSpot(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "tonalSpot");
internal sealed class SchemeFidelity(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "fidelity");
internal sealed class SchemeContent(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "content");
internal sealed class SchemeMonochrome(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "monochrome");
internal sealed class SchemeNeutral(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "neutral");
internal sealed class SchemeVibrant(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "vibrant");
internal sealed class SchemeExpressive(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "expressive");
internal sealed class SchemeRainbow(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "rainbow");
internal sealed class SchemeFruitSalad(Hct sourceColorHct, bool isDark, double contrastLevel) : DynamicScheme(sourceColorHct, isDark, contrastLevel, "fruitSalad");

internal sealed class _MaterialDynamicColor
{
    private readonly string _role;
    internal _MaterialDynamicColor(string role) => _role = role;

    internal long getArgb(DynamicScheme scheme) => MaterialColorSchemeRuntime.GetArgb(
        scheme.seedArgb, scheme.isDark, scheme.variant, scheme.contrastLevel, _role);
}

internal static class MaterialDynamicColors
{
    internal static readonly _MaterialDynamicColor primary = new("primary"), onPrimary = new("onPrimary"), primaryContainer = new("primaryContainer"), onPrimaryContainer = new("onPrimaryContainer"), primaryFixed = new("primaryFixed"), primaryFixedDim = new("primaryFixedDim"), onPrimaryFixed = new("onPrimaryFixed"), onPrimaryFixedVariant = new("onPrimaryFixedVariant"), secondary = new("secondary"), onSecondary = new("onSecondary"), secondaryContainer = new("secondaryContainer"), onSecondaryContainer = new("onSecondaryContainer"), secondaryFixed = new("secondaryFixed"), secondaryFixedDim = new("secondaryFixedDim"), onSecondaryFixed = new("onSecondaryFixed"), onSecondaryFixedVariant = new("onSecondaryFixedVariant"), tertiary = new("tertiary"), onTertiary = new("onTertiary"), tertiaryContainer = new("tertiaryContainer"), onTertiaryContainer = new("onTertiaryContainer"), tertiaryFixed = new("tertiaryFixed"), tertiaryFixedDim = new("tertiaryFixedDim"), onTertiaryFixed = new("onTertiaryFixed"), onTertiaryFixedVariant = new("onTertiaryFixedVariant"), error = new("error"), onError = new("onError"), errorContainer = new("errorContainer"), onErrorContainer = new("onErrorContainer"), outline = new("outline"), outlineVariant = new("outlineVariant"), surface = new("surface"), surfaceDim = new("surfaceDim"), surfaceBright = new("surfaceBright"), surfaceContainerLowest = new("surfaceContainerLowest"), surfaceContainerLow = new("surfaceContainerLow"), surfaceContainer = new("surfaceContainer"), surfaceContainerHigh = new("surfaceContainerHigh"), surfaceContainerHighest = new("surfaceContainerHighest"), onSurface = new("onSurface"), onSurfaceVariant = new("onSurfaceVariant"), inverseSurface = new("inverseSurface"), inverseOnSurface = new("inverseOnSurface"), inversePrimary = new("inversePrimary"), shadow = new("shadow"), scrim = new("scrim"), background = new("background"), onBackground = new("onBackground"), surfaceVariant = new("surfaceVariant");
}

public enum DynamicSchemeVariant
{
    tonalSpot,
    fidelity,
    monochrome,
    neutral,
    vibrant,
    expressive,
    content,
    rainbow,
    fruitSalad
}

public class ColorScheme : global::Doroti.Framework.Foundation.Diagnosticable
{
    public virtual Brightness brightness { get; private set; } = default!;
    public virtual Color primary { get; private set; } = default!;
    public virtual Color onPrimary { get; private set; } = default!;
    internal virtual Color? _primaryContainer { get; private set; }
    internal virtual Color? _onPrimaryContainer { get; private set; }
    internal virtual Color? _primaryFixed { get; private set; }
    internal virtual Color? _primaryFixedDim { get; private set; }
    internal virtual Color? _onPrimaryFixed { get; private set; }
    internal virtual Color? _onPrimaryFixedVariant { get; private set; }
    public virtual Color secondary { get; private set; } = default!;
    public virtual Color onSecondary { get; private set; } = default!;
    internal virtual Color? _secondaryContainer { get; private set; }
    internal virtual Color? _onSecondaryContainer { get; private set; }
    internal virtual Color? _secondaryFixed { get; private set; }
    internal virtual Color? _secondaryFixedDim { get; private set; }
    internal virtual Color? _onSecondaryFixed { get; private set; }
    internal virtual Color? _onSecondaryFixedVariant { get; private set; }
    internal virtual Color? _tertiary { get; private set; }
    internal virtual Color? _onTertiary { get; private set; }
    internal virtual Color? _tertiaryContainer { get; private set; }
    internal virtual Color? _onTertiaryContainer { get; private set; }
    internal virtual Color? _tertiaryFixed { get; private set; }
    internal virtual Color? _tertiaryFixedDim { get; private set; }
    internal virtual Color? _onTertiaryFixed { get; private set; }
    internal virtual Color? _onTertiaryFixedVariant { get; private set; }
    public virtual Color error { get; private set; } = default!;
    public virtual Color onError { get; private set; } = default!;
    internal virtual Color? _errorContainer { get; private set; }
    internal virtual Color? _onErrorContainer { get; private set; }
    public virtual Color surface { get; private set; } = default!;
    public virtual Color onSurface { get; private set; } = default!;
    internal virtual Color? _surfaceVariant { get; private set; }
    internal virtual Color? _surfaceDim { get; private set; }
    internal virtual Color? _surfaceBright { get; private set; }
    internal virtual Color? _surfaceContainerLowest { get; private set; }
    internal virtual Color? _surfaceContainerLow { get; private set; }
    internal virtual Color? _surfaceContainer { get; private set; }
    internal virtual Color? _surfaceContainerHigh { get; private set; }
    internal virtual Color? _surfaceContainerHighest { get; private set; }
    internal virtual Color? _onSurfaceVariant { get; private set; }
    internal virtual Color? _outline { get; private set; }
    internal virtual Color? _outlineVariant { get; private set; }
    internal virtual Color? _shadow { get; private set; }
    internal virtual Color? _scrim { get; private set; }
    internal virtual Color? _inverseSurface { get; private set; }
    internal virtual Color? _onInverseSurface { get; private set; }
    internal virtual Color? _inversePrimary { get; private set; }
    internal virtual Color? _surfaceTint { get; private set; }
    internal virtual Color? _background { get; private set; }
    internal virtual Color? _onBackground { get; private set; }

    public ColorScheme(Brightness brightness, Color primary, Color onPrimary, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color secondary = default!, Color onSecondary = default!, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color error = default!, Color onError = default!, Color? errorContainer = null, Color? onErrorContainer = null, Color surface = default!, Color onSurface = default!, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? outline = null, Color? outlineVariant = null, Color? shadow = null, Color? scrim = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? surfaceTint = null, Color? background = null, Color? onBackground = null, Color? surfaceVariant = null)
    {
        this.brightness = brightness;
        this.primary = primary;
        this.onPrimary = onPrimary;
        this.secondary = secondary;
        this.onSecondary = onSecondary;
        this.error = error;
        this.onError = onError;
        this.surface = surface;
        this.onSurface = onSurface;
        _primaryContainer = primaryContainer;
        _onPrimaryContainer = onPrimaryContainer;
        _primaryFixed = primaryFixed;
        _primaryFixedDim = primaryFixedDim;
        _onPrimaryFixed = onPrimaryFixed;
        _onPrimaryFixedVariant = onPrimaryFixedVariant;
        _secondaryContainer = secondaryContainer;
        _onSecondaryContainer = onSecondaryContainer;
        _secondaryFixed = secondaryFixed;
        _secondaryFixedDim = secondaryFixedDim;
        _onSecondaryFixed = onSecondaryFixed;
        _onSecondaryFixedVariant = onSecondaryFixedVariant;
        _tertiary = tertiary;
        _onTertiary = onTertiary;
        _tertiaryContainer = tertiaryContainer;
        _onTertiaryContainer = onTertiaryContainer;
        _tertiaryFixed = tertiaryFixed;
        _tertiaryFixedDim = tertiaryFixedDim;
        _onTertiaryFixed = onTertiaryFixed;
        _onTertiaryFixedVariant = onTertiaryFixedVariant;
        _errorContainer = errorContainer;
        _onErrorContainer = onErrorContainer;
        _surfaceDim = surfaceDim;
        _surfaceBright = surfaceBright;
        _surfaceContainerLowest = surfaceContainerLowest;
        _surfaceContainerLow = surfaceContainerLow;
        _surfaceContainer = surfaceContainer;
        _surfaceContainerHigh = surfaceContainerHigh;
        _surfaceContainerHighest = surfaceContainerHighest;
        _onSurfaceVariant = onSurfaceVariant;
        _outline = outline;
        _outlineVariant = outlineVariant;
        _shadow = shadow;
        _scrim = scrim;
        _inverseSurface = inverseSurface;
        _onInverseSurface = onInverseSurface;
        _inversePrimary = inversePrimary;
        _surfaceTint = surfaceTint;
        _background = background;
        _onBackground = onBackground;
        _surfaceVariant = surfaceVariant;
    }

    public static ColorScheme CreateFromSeed(Color seedColor, Brightness brightness = Brightness.light, DynamicSchemeVariant dynamicSchemeVariant = DynamicSchemeVariant.tonalSpot, double contrastLevel = 0.0, Color? primary = null, Color? onPrimary = null, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color? secondary = null, Color? onSecondary = null, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color? error = null, Color? onError = null, Color? errorContainer = null, Color? onErrorContainer = null, Color? outline = null, Color? outlineVariant = null, Color? surface = null, Color? onSurface = null, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? shadow = null, Color? scrim = null, Color? surfaceTint = null, Color? background = null, Color? onBackground = null, Color? surfaceVariant = null)
    {
        DynamicScheme scheme = _buildDynamicScheme(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(brightness)), seedColor, dynamicSchemeVariant, contrastLevel);
        return new ColorScheme(primary: primary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primary.getArgb(scheme)), onPrimary: onPrimary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimary.getArgb(scheme)), primaryContainer: primaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryContainer.getArgb(scheme)), onPrimaryContainer: onPrimaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryContainer.getArgb(scheme)), primaryFixed: primaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryFixed.getArgb(scheme)), primaryFixedDim: primaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryFixedDim.getArgb(scheme)), onPrimaryFixed: onPrimaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryFixed.getArgb(scheme)), onPrimaryFixedVariant: onPrimaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryFixedVariant.getArgb(scheme)), secondary: secondary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondary.getArgb(scheme)), onSecondary: onSecondary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondary.getArgb(scheme)), secondaryContainer: secondaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryContainer.getArgb(scheme)), onSecondaryContainer: onSecondaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryContainer.getArgb(scheme)), secondaryFixed: secondaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryFixed.getArgb(scheme)), secondaryFixedDim: secondaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryFixedDim.getArgb(scheme)), onSecondaryFixed: onSecondaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryFixed.getArgb(scheme)), onSecondaryFixedVariant: onSecondaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryFixedVariant.getArgb(scheme)), tertiary: tertiary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiary.getArgb(scheme)), onTertiary: onTertiary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiary.getArgb(scheme)), tertiaryContainer: tertiaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryContainer.getArgb(scheme)), onTertiaryContainer: onTertiaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryContainer.getArgb(scheme)), tertiaryFixed: tertiaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryFixed.getArgb(scheme)), tertiaryFixedDim: tertiaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryFixedDim.getArgb(scheme)), onTertiaryFixed: onTertiaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryFixed.getArgb(scheme)), onTertiaryFixedVariant: onTertiaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryFixedVariant.getArgb(scheme)), error: error ?? new global::Doroti.Ui.Color(MaterialDynamicColors.error.getArgb(scheme)), onError: onError ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onError.getArgb(scheme)), errorContainer: errorContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.errorContainer.getArgb(scheme)), onErrorContainer: onErrorContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onErrorContainer.getArgb(scheme)), outline: outline ?? new global::Doroti.Ui.Color(MaterialDynamicColors.outline.getArgb(scheme)), outlineVariant: outlineVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.outlineVariant.getArgb(scheme)), surface: surface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surface.getArgb(scheme)), surfaceDim: surfaceDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceDim.getArgb(scheme)), surfaceBright: surfaceBright ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceBright.getArgb(scheme)), surfaceContainerLowest: surfaceContainerLowest ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerLowest.getArgb(scheme)), surfaceContainerLow: surfaceContainerLow ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerLow.getArgb(scheme)), surfaceContainer: surfaceContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainer.getArgb(scheme)), surfaceContainerHigh: surfaceContainerHigh ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerHigh.getArgb(scheme)), surfaceContainerHighest: surfaceContainerHighest ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerHighest.getArgb(scheme)), onSurface: onSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSurface.getArgb(scheme)), onSurfaceVariant: onSurfaceVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSurfaceVariant.getArgb(scheme)), inverseSurface: inverseSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inverseSurface.getArgb(scheme)), onInverseSurface: onInverseSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inverseOnSurface.getArgb(scheme)), inversePrimary: inversePrimary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inversePrimary.getArgb(scheme)), shadow: shadow ?? new global::Doroti.Ui.Color(MaterialDynamicColors.shadow.getArgb(scheme)), scrim: scrim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.scrim.getArgb(scheme)), surfaceTint: surfaceTint ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primary.getArgb(scheme)), brightness: DartRuntimePrimitives.RequireValue(brightness), background: background ?? new global::Doroti.Ui.Color(MaterialDynamicColors.background.getArgb(scheme)), onBackground: onBackground ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onBackground.getArgb(scheme)), surfaceVariant: surfaceVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceVariant.getArgb(scheme)));
    }

    public static ColorScheme CreateLight(Brightness brightness = Brightness.light, Color primary = default!, Color onPrimary = default!, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color secondary = default!, Color onSecondary = default!, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color error = default!, Color onError = default!, Color? errorContainer = null, Color? onErrorContainer = null, Color surface = default!, Color onSurface = default!, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? outline = null, Color? outlineVariant = null, Color? shadow = null, Color? scrim = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? surfaceTint = null, Color? background = default!, Color? onBackground = default!, Color? surfaceVariant = null)
    {
        var __instance = new ColorScheme(brightness: brightness, primary: primary, onPrimary: onPrimary, primaryContainer: primaryContainer, onPrimaryContainer: onPrimaryContainer, primaryFixed: primaryFixed, primaryFixedDim: primaryFixedDim, onPrimaryFixed: onPrimaryFixed, onPrimaryFixedVariant: onPrimaryFixedVariant, secondary: secondary, onSecondary: onSecondary, secondaryContainer: secondaryContainer, onSecondaryContainer: onSecondaryContainer, secondaryFixed: secondaryFixed, secondaryFixedDim: secondaryFixedDim, onSecondaryFixed: onSecondaryFixed, onSecondaryFixedVariant: onSecondaryFixedVariant, tertiary: tertiary, onTertiary: onTertiary, tertiaryContainer: tertiaryContainer, onTertiaryContainer: onTertiaryContainer, tertiaryFixed: tertiaryFixed, tertiaryFixedDim: tertiaryFixedDim, onTertiaryFixed: onTertiaryFixed, onTertiaryFixedVariant: onTertiaryFixedVariant, error: error, onError: onError, errorContainer: errorContainer, onErrorContainer: onErrorContainer, surface: surface, onSurface: onSurface, surfaceDim: surfaceDim, surfaceBright: surfaceBright, surfaceContainerLowest: surfaceContainerLowest, surfaceContainerLow: surfaceContainerLow, surfaceContainer: surfaceContainer, surfaceContainerHigh: surfaceContainerHigh, surfaceContainerHighest: surfaceContainerHighest, onSurfaceVariant: onSurfaceVariant, outline: outline, outlineVariant: outlineVariant, shadow: shadow, scrim: scrim, inverseSurface: inverseSurface, onInverseSurface: onInverseSurface, inversePrimary: inversePrimary, surfaceTint: surfaceTint, background: background, onBackground: onBackground, surfaceVariant: surfaceVariant);
        Color __primary = primary ?? new Color(0xff6200ee);
        Color __onPrimary = onPrimary ?? Colors.white;
        Color __secondary = secondary ?? new Color(0xff03dac6);
        Color __onSecondary = onSecondary ?? Colors.black;
        Color __error = error ?? new Color(0xffb00020);
        Color __onError = onError ?? Colors.white;
        Color __surface = surface ?? Colors.white;
        Color __onSurface = onSurface ?? Colors.black;
        Color? __background = background ?? Colors.white;
        Color? __onBackground = onBackground ?? Colors.black;
        __instance.brightness = brightness;
        __instance.primary = __primary;
        __instance.onPrimary = __onPrimary;
        __instance.secondary = __secondary;
        __instance.onSecondary = __onSecondary;
        __instance.error = __error;
        __instance.onError = __onError;
        __instance.surface = __surface;
        __instance.onSurface = __onSurface;
        __instance._primaryContainer = primaryContainer;
        __instance._onPrimaryContainer = onPrimaryContainer;
        __instance._primaryFixed = primaryFixed;
        __instance._primaryFixedDim = primaryFixedDim;
        __instance._onPrimaryFixed = onPrimaryFixed;
        __instance._onPrimaryFixedVariant = onPrimaryFixedVariant;
        __instance._secondaryContainer = secondaryContainer;
        __instance._onSecondaryContainer = onSecondaryContainer;
        __instance._secondaryFixed = secondaryFixed;
        __instance._secondaryFixedDim = secondaryFixedDim;
        __instance._onSecondaryFixed = onSecondaryFixed;
        __instance._onSecondaryFixedVariant = onSecondaryFixedVariant;
        __instance._tertiary = tertiary;
        __instance._onTertiary = onTertiary;
        __instance._tertiaryContainer = tertiaryContainer;
        __instance._onTertiaryContainer = onTertiaryContainer;
        __instance._tertiaryFixed = tertiaryFixed;
        __instance._tertiaryFixedDim = tertiaryFixedDim;
        __instance._onTertiaryFixed = onTertiaryFixed;
        __instance._onTertiaryFixedVariant = onTertiaryFixedVariant;
        __instance._errorContainer = errorContainer;
        __instance._onErrorContainer = onErrorContainer;
        __instance._surfaceDim = surfaceDim;
        __instance._surfaceBright = surfaceBright;
        __instance._surfaceContainerLowest = surfaceContainerLowest;
        __instance._surfaceContainerLow = surfaceContainerLow;
        __instance._surfaceContainer = surfaceContainer;
        __instance._surfaceContainerHigh = surfaceContainerHigh;
        __instance._surfaceContainerHighest = surfaceContainerHighest;
        __instance._onSurfaceVariant = onSurfaceVariant;
        __instance._outline = outline;
        __instance._outlineVariant = outlineVariant;
        __instance._shadow = shadow;
        __instance._scrim = scrim;
        __instance._inverseSurface = inverseSurface;
        __instance._onInverseSurface = onInverseSurface;
        __instance._inversePrimary = inversePrimary;
        __instance._surfaceTint = surfaceTint;
        __instance._background = __background;
        __instance._onBackground = __onBackground;
        __instance._surfaceVariant = surfaceVariant;
        return __instance;
    }

    public static ColorScheme CreateDark(Brightness brightness = Brightness.dark, Color primary = default!, Color onPrimary = default!, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color secondary = default!, Color onSecondary = default!, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color error = default!, Color onError = default!, Color? errorContainer = null, Color? onErrorContainer = null, Color surface = default!, Color onSurface = default!, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? outline = null, Color? outlineVariant = null, Color? shadow = null, Color? scrim = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? surfaceTint = null, Color? background = default!, Color? onBackground = default!, Color? surfaceVariant = null)
    {
        var __instance = new ColorScheme(brightness: brightness, primary: primary, onPrimary: onPrimary, primaryContainer: primaryContainer, onPrimaryContainer: onPrimaryContainer, primaryFixed: primaryFixed, primaryFixedDim: primaryFixedDim, onPrimaryFixed: onPrimaryFixed, onPrimaryFixedVariant: onPrimaryFixedVariant, secondary: secondary, onSecondary: onSecondary, secondaryContainer: secondaryContainer, onSecondaryContainer: onSecondaryContainer, secondaryFixed: secondaryFixed, secondaryFixedDim: secondaryFixedDim, onSecondaryFixed: onSecondaryFixed, onSecondaryFixedVariant: onSecondaryFixedVariant, tertiary: tertiary, onTertiary: onTertiary, tertiaryContainer: tertiaryContainer, onTertiaryContainer: onTertiaryContainer, tertiaryFixed: tertiaryFixed, tertiaryFixedDim: tertiaryFixedDim, onTertiaryFixed: onTertiaryFixed, onTertiaryFixedVariant: onTertiaryFixedVariant, error: error, onError: onError, errorContainer: errorContainer, onErrorContainer: onErrorContainer, surface: surface, onSurface: onSurface, surfaceDim: surfaceDim, surfaceBright: surfaceBright, surfaceContainerLowest: surfaceContainerLowest, surfaceContainerLow: surfaceContainerLow, surfaceContainer: surfaceContainer, surfaceContainerHigh: surfaceContainerHigh, surfaceContainerHighest: surfaceContainerHighest, onSurfaceVariant: onSurfaceVariant, outline: outline, outlineVariant: outlineVariant, shadow: shadow, scrim: scrim, inverseSurface: inverseSurface, onInverseSurface: onInverseSurface, inversePrimary: inversePrimary, surfaceTint: surfaceTint, background: background, onBackground: onBackground, surfaceVariant: surfaceVariant);
        Color __primary = primary ?? new Color(0xffbb86fc);
        Color __onPrimary = onPrimary ?? Colors.black;
        Color __secondary = secondary ?? new Color(0xff03dac6);
        Color __onSecondary = onSecondary ?? Colors.black;
        Color __error = error ?? new Color(0xffcf6679);
        Color __onError = onError ?? Colors.black;
        Color __surface = surface ?? new Color(0xff121212);
        Color __onSurface = onSurface ?? Colors.white;
        Color? __background = background ?? new Color(0xff121212);
        Color? __onBackground = onBackground ?? Colors.white;
        __instance.brightness = brightness;
        __instance.primary = __primary;
        __instance.onPrimary = __onPrimary;
        __instance.secondary = __secondary;
        __instance.onSecondary = __onSecondary;
        __instance.error = __error;
        __instance.onError = __onError;
        __instance.surface = __surface;
        __instance.onSurface = __onSurface;
        __instance._primaryContainer = primaryContainer;
        __instance._onPrimaryContainer = onPrimaryContainer;
        __instance._primaryFixed = primaryFixed;
        __instance._primaryFixedDim = primaryFixedDim;
        __instance._onPrimaryFixed = onPrimaryFixed;
        __instance._onPrimaryFixedVariant = onPrimaryFixedVariant;
        __instance._secondaryContainer = secondaryContainer;
        __instance._onSecondaryContainer = onSecondaryContainer;
        __instance._secondaryFixed = secondaryFixed;
        __instance._secondaryFixedDim = secondaryFixedDim;
        __instance._onSecondaryFixed = onSecondaryFixed;
        __instance._onSecondaryFixedVariant = onSecondaryFixedVariant;
        __instance._tertiary = tertiary;
        __instance._onTertiary = onTertiary;
        __instance._tertiaryContainer = tertiaryContainer;
        __instance._onTertiaryContainer = onTertiaryContainer;
        __instance._tertiaryFixed = tertiaryFixed;
        __instance._tertiaryFixedDim = tertiaryFixedDim;
        __instance._onTertiaryFixed = onTertiaryFixed;
        __instance._onTertiaryFixedVariant = onTertiaryFixedVariant;
        __instance._errorContainer = errorContainer;
        __instance._onErrorContainer = onErrorContainer;
        __instance._surfaceDim = surfaceDim;
        __instance._surfaceBright = surfaceBright;
        __instance._surfaceContainerLowest = surfaceContainerLowest;
        __instance._surfaceContainerLow = surfaceContainerLow;
        __instance._surfaceContainer = surfaceContainer;
        __instance._surfaceContainerHigh = surfaceContainerHigh;
        __instance._surfaceContainerHighest = surfaceContainerHighest;
        __instance._onSurfaceVariant = onSurfaceVariant;
        __instance._outline = outline;
        __instance._outlineVariant = outlineVariant;
        __instance._shadow = shadow;
        __instance._scrim = scrim;
        __instance._inverseSurface = inverseSurface;
        __instance._onInverseSurface = onInverseSurface;
        __instance._inversePrimary = inversePrimary;
        __instance._surfaceTint = surfaceTint;
        __instance._background = __background;
        __instance._onBackground = __onBackground;
        __instance._surfaceVariant = surfaceVariant;
        return __instance;
    }

    public static ColorScheme CreateHighContrastLight(Brightness brightness = Brightness.light, Color primary = default!, Color onPrimary = default!, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color secondary = default!, Color onSecondary = default!, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color error = default!, Color onError = default!, Color? errorContainer = null, Color? onErrorContainer = null, Color surface = default!, Color onSurface = default!, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? outline = null, Color? outlineVariant = null, Color? shadow = null, Color? scrim = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? surfaceTint = null, Color? background = default!, Color? onBackground = default!, Color? surfaceVariant = null)
    {
        var __instance = new ColorScheme(brightness: brightness, primary: primary, onPrimary: onPrimary, primaryContainer: primaryContainer, onPrimaryContainer: onPrimaryContainer, primaryFixed: primaryFixed, primaryFixedDim: primaryFixedDim, onPrimaryFixed: onPrimaryFixed, onPrimaryFixedVariant: onPrimaryFixedVariant, secondary: secondary, onSecondary: onSecondary, secondaryContainer: secondaryContainer, onSecondaryContainer: onSecondaryContainer, secondaryFixed: secondaryFixed, secondaryFixedDim: secondaryFixedDim, onSecondaryFixed: onSecondaryFixed, onSecondaryFixedVariant: onSecondaryFixedVariant, tertiary: tertiary, onTertiary: onTertiary, tertiaryContainer: tertiaryContainer, onTertiaryContainer: onTertiaryContainer, tertiaryFixed: tertiaryFixed, tertiaryFixedDim: tertiaryFixedDim, onTertiaryFixed: onTertiaryFixed, onTertiaryFixedVariant: onTertiaryFixedVariant, error: error, onError: onError, errorContainer: errorContainer, onErrorContainer: onErrorContainer, surface: surface, onSurface: onSurface, surfaceDim: surfaceDim, surfaceBright: surfaceBright, surfaceContainerLowest: surfaceContainerLowest, surfaceContainerLow: surfaceContainerLow, surfaceContainer: surfaceContainer, surfaceContainerHigh: surfaceContainerHigh, surfaceContainerHighest: surfaceContainerHighest, onSurfaceVariant: onSurfaceVariant, outline: outline, outlineVariant: outlineVariant, shadow: shadow, scrim: scrim, inverseSurface: inverseSurface, onInverseSurface: onInverseSurface, inversePrimary: inversePrimary, surfaceTint: surfaceTint, background: background, onBackground: onBackground, surfaceVariant: surfaceVariant);
        Color __primary = primary ?? new Color(0xff0000ba);
        Color __onPrimary = onPrimary ?? Colors.white;
        Color __secondary = secondary ?? new Color(0xff66fff9);
        Color __onSecondary = onSecondary ?? Colors.black;
        Color __error = error ?? new Color(0xff790000);
        Color __onError = onError ?? Colors.white;
        Color __surface = surface ?? Colors.white;
        Color __onSurface = onSurface ?? Colors.black;
        Color? __background = background ?? Colors.white;
        Color? __onBackground = onBackground ?? Colors.black;
        __instance.brightness = brightness;
        __instance.primary = __primary;
        __instance.onPrimary = __onPrimary;
        __instance.secondary = __secondary;
        __instance.onSecondary = __onSecondary;
        __instance.error = __error;
        __instance.onError = __onError;
        __instance.surface = __surface;
        __instance.onSurface = __onSurface;
        __instance._primaryContainer = primaryContainer;
        __instance._onPrimaryContainer = onPrimaryContainer;
        __instance._primaryFixed = primaryFixed;
        __instance._primaryFixedDim = primaryFixedDim;
        __instance._onPrimaryFixed = onPrimaryFixed;
        __instance._onPrimaryFixedVariant = onPrimaryFixedVariant;
        __instance._secondaryContainer = secondaryContainer;
        __instance._onSecondaryContainer = onSecondaryContainer;
        __instance._secondaryFixed = secondaryFixed;
        __instance._secondaryFixedDim = secondaryFixedDim;
        __instance._onSecondaryFixed = onSecondaryFixed;
        __instance._onSecondaryFixedVariant = onSecondaryFixedVariant;
        __instance._tertiary = tertiary;
        __instance._onTertiary = onTertiary;
        __instance._tertiaryContainer = tertiaryContainer;
        __instance._onTertiaryContainer = onTertiaryContainer;
        __instance._tertiaryFixed = tertiaryFixed;
        __instance._tertiaryFixedDim = tertiaryFixedDim;
        __instance._onTertiaryFixed = onTertiaryFixed;
        __instance._onTertiaryFixedVariant = onTertiaryFixedVariant;
        __instance._errorContainer = errorContainer;
        __instance._onErrorContainer = onErrorContainer;
        __instance._surfaceDim = surfaceDim;
        __instance._surfaceBright = surfaceBright;
        __instance._surfaceContainerLowest = surfaceContainerLowest;
        __instance._surfaceContainerLow = surfaceContainerLow;
        __instance._surfaceContainer = surfaceContainer;
        __instance._surfaceContainerHigh = surfaceContainerHigh;
        __instance._surfaceContainerHighest = surfaceContainerHighest;
        __instance._onSurfaceVariant = onSurfaceVariant;
        __instance._outline = outline;
        __instance._outlineVariant = outlineVariant;
        __instance._shadow = shadow;
        __instance._scrim = scrim;
        __instance._inverseSurface = inverseSurface;
        __instance._onInverseSurface = onInverseSurface;
        __instance._inversePrimary = inversePrimary;
        __instance._surfaceTint = surfaceTint;
        __instance._background = __background;
        __instance._onBackground = __onBackground;
        __instance._surfaceVariant = surfaceVariant;
        return __instance;
    }

    public static ColorScheme CreateHighContrastDark(Brightness brightness = Brightness.dark, Color primary = default!, Color onPrimary = default!, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color secondary = default!, Color onSecondary = default!, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color error = default!, Color onError = default!, Color? errorContainer = null, Color? onErrorContainer = null, Color surface = default!, Color onSurface = default!, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? outline = null, Color? outlineVariant = null, Color? shadow = null, Color? scrim = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? surfaceTint = null, Color? background = default!, Color? onBackground = default!, Color? surfaceVariant = null)
    {
        var __instance = new ColorScheme(brightness: brightness, primary: primary, onPrimary: onPrimary, primaryContainer: primaryContainer, onPrimaryContainer: onPrimaryContainer, primaryFixed: primaryFixed, primaryFixedDim: primaryFixedDim, onPrimaryFixed: onPrimaryFixed, onPrimaryFixedVariant: onPrimaryFixedVariant, secondary: secondary, onSecondary: onSecondary, secondaryContainer: secondaryContainer, onSecondaryContainer: onSecondaryContainer, secondaryFixed: secondaryFixed, secondaryFixedDim: secondaryFixedDim, onSecondaryFixed: onSecondaryFixed, onSecondaryFixedVariant: onSecondaryFixedVariant, tertiary: tertiary, onTertiary: onTertiary, tertiaryContainer: tertiaryContainer, onTertiaryContainer: onTertiaryContainer, tertiaryFixed: tertiaryFixed, tertiaryFixedDim: tertiaryFixedDim, onTertiaryFixed: onTertiaryFixed, onTertiaryFixedVariant: onTertiaryFixedVariant, error: error, onError: onError, errorContainer: errorContainer, onErrorContainer: onErrorContainer, surface: surface, onSurface: onSurface, surfaceDim: surfaceDim, surfaceBright: surfaceBright, surfaceContainerLowest: surfaceContainerLowest, surfaceContainerLow: surfaceContainerLow, surfaceContainer: surfaceContainer, surfaceContainerHigh: surfaceContainerHigh, surfaceContainerHighest: surfaceContainerHighest, onSurfaceVariant: onSurfaceVariant, outline: outline, outlineVariant: outlineVariant, shadow: shadow, scrim: scrim, inverseSurface: inverseSurface, onInverseSurface: onInverseSurface, inversePrimary: inversePrimary, surfaceTint: surfaceTint, background: background, onBackground: onBackground, surfaceVariant: surfaceVariant);
        Color __primary = primary ?? new Color(0xffefb7ff);
        Color __onPrimary = onPrimary ?? Colors.black;
        Color __secondary = secondary ?? new Color(0xff66fff9);
        Color __onSecondary = onSecondary ?? Colors.black;
        Color __error = error ?? new Color(0xff9b374d);
        Color __onError = onError ?? Colors.black;
        Color __surface = surface ?? new Color(0xff121212);
        Color __onSurface = onSurface ?? Colors.white;
        Color? __background = background ?? new Color(0xff121212);
        Color? __onBackground = onBackground ?? Colors.white;
        __instance.brightness = brightness;
        __instance.primary = __primary;
        __instance.onPrimary = __onPrimary;
        __instance.secondary = __secondary;
        __instance.onSecondary = __onSecondary;
        __instance.error = __error;
        __instance.onError = __onError;
        __instance.surface = __surface;
        __instance.onSurface = __onSurface;
        __instance._primaryContainer = primaryContainer;
        __instance._onPrimaryContainer = onPrimaryContainer;
        __instance._primaryFixed = primaryFixed;
        __instance._primaryFixedDim = primaryFixedDim;
        __instance._onPrimaryFixed = onPrimaryFixed;
        __instance._onPrimaryFixedVariant = onPrimaryFixedVariant;
        __instance._secondaryContainer = secondaryContainer;
        __instance._onSecondaryContainer = onSecondaryContainer;
        __instance._secondaryFixed = secondaryFixed;
        __instance._secondaryFixedDim = secondaryFixedDim;
        __instance._onSecondaryFixed = onSecondaryFixed;
        __instance._onSecondaryFixedVariant = onSecondaryFixedVariant;
        __instance._tertiary = tertiary;
        __instance._onTertiary = onTertiary;
        __instance._tertiaryContainer = tertiaryContainer;
        __instance._onTertiaryContainer = onTertiaryContainer;
        __instance._tertiaryFixed = tertiaryFixed;
        __instance._tertiaryFixedDim = tertiaryFixedDim;
        __instance._onTertiaryFixed = onTertiaryFixed;
        __instance._onTertiaryFixedVariant = onTertiaryFixedVariant;
        __instance._errorContainer = errorContainer;
        __instance._onErrorContainer = onErrorContainer;
        __instance._surfaceDim = surfaceDim;
        __instance._surfaceBright = surfaceBright;
        __instance._surfaceContainerLowest = surfaceContainerLowest;
        __instance._surfaceContainerLow = surfaceContainerLow;
        __instance._surfaceContainer = surfaceContainer;
        __instance._surfaceContainerHigh = surfaceContainerHigh;
        __instance._surfaceContainerHighest = surfaceContainerHighest;
        __instance._onSurfaceVariant = onSurfaceVariant;
        __instance._outline = outline;
        __instance._outlineVariant = outlineVariant;
        __instance._shadow = shadow;
        __instance._scrim = scrim;
        __instance._inverseSurface = inverseSurface;
        __instance._onInverseSurface = onInverseSurface;
        __instance._inversePrimary = inversePrimary;
        __instance._surfaceTint = surfaceTint;
        __instance._background = __background;
        __instance._onBackground = __onBackground;
        __instance._surfaceVariant = surfaceVariant;
        return __instance;
    }

    public static ColorScheme CreateFromSwatch(MaterialColor primarySwatch = default!, Color? accentColor = null, Color? cardColor = null, Color? backgroundColor = null, Color? errorColor = null, Brightness brightness = Brightness.light)
    {
        MaterialColor __primarySwatch = primarySwatch ?? Colors.blue;
        var isDark = Equals(DartRuntimePrimitives.RequireValue(brightness), Brightness.dark);
        var primaryIsDark = Equals(_brightnessFor(__primarySwatch), Brightness.dark);
        global::Doroti.Ui.Color secondaryLocal = accentColor ?? (isDark ? Colors.tealAccent[200L]! : __primarySwatch);
        var secondaryIsDark = Equals(_brightnessFor(secondaryLocal), Brightness.dark);
        return new ColorScheme(primary: __primarySwatch, secondary: secondaryLocal, surface: cardColor ?? (isDark ? Colors.grey[800L]! : Colors.white), error: errorColor ?? Colors.red[700L]!, onPrimary: primaryIsDark ? Colors.white : Colors.black, onSecondary: secondaryIsDark ? Colors.white : Colors.black, onSurface: isDark ? Colors.white : Colors.black, onError: isDark ? Colors.black : Colors.white, brightness: DartRuntimePrimitives.RequireValue(brightness), background: backgroundColor ?? (isDark ? Colors.grey[700L]! : __primarySwatch[200L]!), onBackground: primaryIsDark ? Colors.white : Colors.black);
    }

    internal static global::Doroti.Ui.Brightness _brightnessFor(Color color) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Brightness>(ThemeData.estimateBrightnessForColor(color));
    public virtual global::Doroti.Ui.Color primaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_primaryContainer ?? primary);
    public virtual global::Doroti.Ui.Color onPrimaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onPrimaryContainer ?? onPrimary);
    public virtual global::Doroti.Ui.Color primaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_primaryFixed ?? primary);
    public virtual global::Doroti.Ui.Color primaryFixedDim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_primaryFixedDim ?? primary);
    public virtual global::Doroti.Ui.Color onPrimaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onPrimaryFixed ?? onPrimary);
    public virtual global::Doroti.Ui.Color onPrimaryFixedVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onPrimaryFixedVariant ?? onPrimary);
    public virtual global::Doroti.Ui.Color secondaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_secondaryContainer ?? secondary);
    public virtual global::Doroti.Ui.Color onSecondaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onSecondaryContainer ?? onSecondary);
    public virtual global::Doroti.Ui.Color secondaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_secondaryFixed ?? secondary);
    public virtual global::Doroti.Ui.Color secondaryFixedDim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_secondaryFixedDim ?? secondary);
    public virtual global::Doroti.Ui.Color onSecondaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onSecondaryFixed ?? onSecondary);
    public virtual global::Doroti.Ui.Color onSecondaryFixedVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onSecondaryFixedVariant ?? onSecondary);
    public virtual global::Doroti.Ui.Color tertiary => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_tertiary ?? secondary);
    public virtual global::Doroti.Ui.Color onTertiary => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onTertiary ?? onSecondary);
    public virtual global::Doroti.Ui.Color tertiaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_tertiaryContainer ?? tertiary);
    public virtual global::Doroti.Ui.Color onTertiaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onTertiaryContainer ?? onTertiary);
    public virtual global::Doroti.Ui.Color tertiaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_tertiaryFixed ?? tertiary);
    public virtual global::Doroti.Ui.Color tertiaryFixedDim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_tertiaryFixedDim ?? tertiary);
    public virtual global::Doroti.Ui.Color onTertiaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onTertiaryFixed ?? onTertiary);
    public virtual global::Doroti.Ui.Color onTertiaryFixedVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onTertiaryFixedVariant ?? onTertiary);
    public virtual global::Doroti.Ui.Color errorContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_errorContainer ?? error);
    public virtual global::Doroti.Ui.Color onErrorContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onErrorContainer ?? onError);
    public virtual global::Doroti.Ui.Color surfaceVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceVariant ?? surface);
    public virtual global::Doroti.Ui.Color surfaceDim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceDim ?? surface);
    public virtual global::Doroti.Ui.Color surfaceBright => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceBright ?? surface);
    public virtual global::Doroti.Ui.Color surfaceContainerLowest => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceContainerLowest ?? surface);
    public virtual global::Doroti.Ui.Color surfaceContainerLow => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceContainerLow ?? surface);
    public virtual global::Doroti.Ui.Color surfaceContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceContainer ?? surface);
    public virtual global::Doroti.Ui.Color surfaceContainerHigh => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceContainerHigh ?? surface);
    public virtual global::Doroti.Ui.Color surfaceContainerHighest => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceContainerHighest ?? surface);
    public virtual global::Doroti.Ui.Color onSurfaceVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onSurfaceVariant ?? onSurface);
    public virtual global::Doroti.Ui.Color outline => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_outline ?? onBackground);
    public virtual global::Doroti.Ui.Color outlineVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_outlineVariant ?? onBackground);
    public virtual global::Doroti.Ui.Color shadow => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_shadow ?? new global::Doroti.Ui.Color(4278190080L));
    public virtual global::Doroti.Ui.Color scrim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_scrim ?? new global::Doroti.Ui.Color(4278190080L));
    public virtual global::Doroti.Ui.Color inverseSurface => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_inverseSurface ?? onSurface);
    public virtual global::Doroti.Ui.Color onInverseSurface => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onInverseSurface ?? surface);
    public virtual global::Doroti.Ui.Color inversePrimary => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_inversePrimary ?? onPrimary);
    public virtual global::Doroti.Ui.Color surfaceTint => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_surfaceTint ?? primary);
    public virtual global::Doroti.Ui.Color background => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_background ?? surface);
    public virtual global::Doroti.Ui.Color onBackground => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(_onBackground ?? onSurface);
    public virtual ColorScheme copyWith(Brightness? brightness = null, Color? primary = null, Color? onPrimary = null, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color? secondary = null, Color? onSecondary = null, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color? error = null, Color? onError = null, Color? errorContainer = null, Color? onErrorContainer = null, Color? surface = null, Color? onSurface = null, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? outline = null, Color? outlineVariant = null, Color? shadow = null, Color? scrim = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? surfaceTint = null, Color? background = null, Color? onBackground = null, Color? surfaceVariant = null)
    {
        return new ColorScheme(brightness: brightness ?? this.brightness, primary: primary ?? this.primary, onPrimary: onPrimary ?? this.onPrimary, primaryContainer: primaryContainer ?? this.primaryContainer, onPrimaryContainer: onPrimaryContainer ?? this.onPrimaryContainer, primaryFixed: primaryFixed ?? this.primaryFixed, primaryFixedDim: primaryFixedDim ?? this.primaryFixedDim, onPrimaryFixed: onPrimaryFixed ?? this.onPrimaryFixed, onPrimaryFixedVariant: onPrimaryFixedVariant ?? this.onPrimaryFixedVariant, secondary: secondary ?? this.secondary, onSecondary: onSecondary ?? this.onSecondary, secondaryContainer: secondaryContainer ?? this.secondaryContainer, onSecondaryContainer: onSecondaryContainer ?? this.onSecondaryContainer, secondaryFixed: secondaryFixed ?? this.secondaryFixed, secondaryFixedDim: secondaryFixedDim ?? this.secondaryFixedDim, onSecondaryFixed: onSecondaryFixed ?? this.onSecondaryFixed, onSecondaryFixedVariant: onSecondaryFixedVariant ?? this.onSecondaryFixedVariant, tertiary: tertiary ?? this.tertiary, onTertiary: onTertiary ?? this.onTertiary, tertiaryContainer: tertiaryContainer ?? this.tertiaryContainer, onTertiaryContainer: onTertiaryContainer ?? this.onTertiaryContainer, tertiaryFixed: tertiaryFixed ?? this.tertiaryFixed, tertiaryFixedDim: tertiaryFixedDim ?? this.tertiaryFixedDim, onTertiaryFixed: onTertiaryFixed ?? this.onTertiaryFixed, onTertiaryFixedVariant: onTertiaryFixedVariant ?? this.onTertiaryFixedVariant, error: error ?? this.error, onError: onError ?? this.onError, errorContainer: errorContainer ?? this.errorContainer, onErrorContainer: onErrorContainer ?? this.onErrorContainer, surface: surface ?? this.surface, onSurface: onSurface ?? this.onSurface, surfaceDim: surfaceDim ?? this.surfaceDim, surfaceBright: surfaceBright ?? this.surfaceBright, surfaceContainerLowest: surfaceContainerLowest ?? this.surfaceContainerLowest, surfaceContainerLow: surfaceContainerLow ?? this.surfaceContainerLow, surfaceContainer: surfaceContainer ?? this.surfaceContainer, surfaceContainerHigh: surfaceContainerHigh ?? this.surfaceContainerHigh, surfaceContainerHighest: surfaceContainerHighest ?? this.surfaceContainerHighest, onSurfaceVariant: onSurfaceVariant ?? this.onSurfaceVariant, outline: outline ?? this.outline, outlineVariant: outlineVariant ?? this.outlineVariant, shadow: shadow ?? this.shadow, scrim: scrim ?? this.scrim, inverseSurface: inverseSurface ?? this.inverseSurface, onInverseSurface: onInverseSurface ?? this.onInverseSurface, inversePrimary: inversePrimary ?? this.inversePrimary, surfaceTint: surfaceTint ?? this.surfaceTint, background: background ?? this.background, onBackground: onBackground ?? this.onBackground, surfaceVariant: surfaceVariant ?? this.surfaceVariant);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ColorScheme lerp(ColorScheme a, ColorScheme b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ColorScheme(brightness: (t < 0.5) ? a.brightness : b.brightness, primary: Dart_uiLibrary.Color.lerp(a.primary, b.primary, t)!, onPrimary: Dart_uiLibrary.Color.lerp(a.onPrimary, b.onPrimary, t)!, primaryContainer: Dart_uiLibrary.Color.lerp(a.primaryContainer, b.primaryContainer, t), onPrimaryContainer: Dart_uiLibrary.Color.lerp(a.onPrimaryContainer, b.onPrimaryContainer, t), primaryFixed: Dart_uiLibrary.Color.lerp(a.primaryFixed, b.primaryFixed, t), primaryFixedDim: Dart_uiLibrary.Color.lerp(a.primaryFixedDim, b.primaryFixedDim, t), onPrimaryFixed: Dart_uiLibrary.Color.lerp(a.onPrimaryFixed, b.onPrimaryFixed, t), onPrimaryFixedVariant: Dart_uiLibrary.Color.lerp(a.onPrimaryFixedVariant, b.onPrimaryFixedVariant, t), secondary: Dart_uiLibrary.Color.lerp(a.secondary, b.secondary, t)!, onSecondary: Dart_uiLibrary.Color.lerp(a.onSecondary, b.onSecondary, t)!, secondaryContainer: Dart_uiLibrary.Color.lerp(a.secondaryContainer, b.secondaryContainer, t), onSecondaryContainer: Dart_uiLibrary.Color.lerp(a.onSecondaryContainer, b.onSecondaryContainer, t), secondaryFixed: Dart_uiLibrary.Color.lerp(a.secondaryFixed, b.secondaryFixed, t), secondaryFixedDim: Dart_uiLibrary.Color.lerp(a.secondaryFixedDim, b.secondaryFixedDim, t), onSecondaryFixed: Dart_uiLibrary.Color.lerp(a.onSecondaryFixed, b.onSecondaryFixed, t), onSecondaryFixedVariant: Dart_uiLibrary.Color.lerp(a.onSecondaryFixedVariant, b.onSecondaryFixedVariant, t), tertiary: Dart_uiLibrary.Color.lerp(a.tertiary, b.tertiary, t), onTertiary: Dart_uiLibrary.Color.lerp(a.onTertiary, b.onTertiary, t), tertiaryContainer: Dart_uiLibrary.Color.lerp(a.tertiaryContainer, b.tertiaryContainer, t), onTertiaryContainer: Dart_uiLibrary.Color.lerp(a.onTertiaryContainer, b.onTertiaryContainer, t), tertiaryFixed: Dart_uiLibrary.Color.lerp(a.tertiaryFixed, b.tertiaryFixed, t), tertiaryFixedDim: Dart_uiLibrary.Color.lerp(a.tertiaryFixedDim, b.tertiaryFixedDim, t), onTertiaryFixed: Dart_uiLibrary.Color.lerp(a.onTertiaryFixed, b.onTertiaryFixed, t), onTertiaryFixedVariant: Dart_uiLibrary.Color.lerp(a.onTertiaryFixedVariant, b.onTertiaryFixedVariant, t), error: Dart_uiLibrary.Color.lerp(a.error, b.error, t)!, onError: Dart_uiLibrary.Color.lerp(a.onError, b.onError, t)!, errorContainer: Dart_uiLibrary.Color.lerp(a.errorContainer, b.errorContainer, t), onErrorContainer: Dart_uiLibrary.Color.lerp(a.onErrorContainer, b.onErrorContainer, t), surface: Dart_uiLibrary.Color.lerp(a.surface, b.surface, t)!, onSurface: Dart_uiLibrary.Color.lerp(a.onSurface, b.onSurface, t)!, surfaceDim: Dart_uiLibrary.Color.lerp(a.surfaceDim, b.surfaceDim, t), surfaceBright: Dart_uiLibrary.Color.lerp(a.surfaceBright, b.surfaceBright, t), surfaceContainerLowest: Dart_uiLibrary.Color.lerp(a.surfaceContainerLowest, b.surfaceContainerLowest, t), surfaceContainerLow: Dart_uiLibrary.Color.lerp(a.surfaceContainerLow, b.surfaceContainerLow, t), surfaceContainer: Dart_uiLibrary.Color.lerp(a.surfaceContainer, b.surfaceContainer, t), surfaceContainerHigh: Dart_uiLibrary.Color.lerp(a.surfaceContainerHigh, b.surfaceContainerHigh, t), surfaceContainerHighest: Dart_uiLibrary.Color.lerp(a.surfaceContainerHighest, b.surfaceContainerHighest, t), onSurfaceVariant: Dart_uiLibrary.Color.lerp(a.onSurfaceVariant, b.onSurfaceVariant, t), outline: Dart_uiLibrary.Color.lerp(a.outline, b.outline, t), outlineVariant: Dart_uiLibrary.Color.lerp(a.outlineVariant, b.outlineVariant, t), shadow: Dart_uiLibrary.Color.lerp(a.shadow, b.shadow, t), scrim: Dart_uiLibrary.Color.lerp(a.scrim, b.scrim, t), inverseSurface: Dart_uiLibrary.Color.lerp(a.inverseSurface, b.inverseSurface, t), onInverseSurface: Dart_uiLibrary.Color.lerp(a.onInverseSurface, b.onInverseSurface, t), inversePrimary: Dart_uiLibrary.Color.lerp(a.inversePrimary, b.inversePrimary, t), surfaceTint: Dart_uiLibrary.Color.lerp(a.surfaceTint, b.surfaceTint, t), background: Dart_uiLibrary.Color.lerp(a.background, b.background, t), onBackground: Dart_uiLibrary.Color.lerp(a.onBackground, b.onBackground, t), surfaceVariant: Dart_uiLibrary.Color.lerp(a.surfaceVariant, b.surfaceVariant, t));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as ColorScheme;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is ColorScheme) && Equals(__other.brightness, brightness) && Equals(__other.primary, primary) && Equals(__other.onPrimary, onPrimary) && Equals(__other.primaryContainer, primaryContainer) && Equals(__other.onPrimaryContainer, onPrimaryContainer) && Equals(__other.primaryFixed, primaryFixed) && Equals(__other.primaryFixedDim, primaryFixedDim) && Equals(__other.onPrimaryFixed, onPrimaryFixed) && Equals(__other.onPrimaryFixedVariant, onPrimaryFixedVariant) && Equals(__other.secondary, secondary) && Equals(__other.onSecondary, onSecondary) && Equals(__other.secondaryContainer, secondaryContainer) && Equals(__other.onSecondaryContainer, onSecondaryContainer) && Equals(__other.secondaryFixed, secondaryFixed) && Equals(__other.secondaryFixedDim, secondaryFixedDim) && Equals(__other.onSecondaryFixed, onSecondaryFixed) && Equals(__other.onSecondaryFixedVariant, onSecondaryFixedVariant) && Equals(__other.tertiary, tertiary) && Equals(__other.onTertiary, onTertiary) && Equals(__other.tertiaryContainer, tertiaryContainer) && Equals(__other.onTertiaryContainer, onTertiaryContainer) && Equals(__other.tertiaryFixed, tertiaryFixed) && Equals(__other.tertiaryFixedDim, tertiaryFixedDim) && Equals(__other.onTertiaryFixed, onTertiaryFixed) && Equals(__other.onTertiaryFixedVariant, onTertiaryFixedVariant) && Equals(__other.error, error) && Equals(__other.onError, onError) && Equals(__other.errorContainer, errorContainer) && Equals(__other.onErrorContainer, onErrorContainer) && Equals(__other.surface, surface) && Equals(__other.onSurface, onSurface) && Equals(__other.surfaceDim, surfaceDim) && Equals(__other.surfaceBright, surfaceBright) && Equals(__other.surfaceContainerLowest, surfaceContainerLowest) && Equals(__other.surfaceContainerLow, surfaceContainerLow) && Equals(__other.surfaceContainer, surfaceContainer) && Equals(__other.surfaceContainerHigh, surfaceContainerHigh) && Equals(__other.surfaceContainerHighest, surfaceContainerHighest) && Equals(__other.onSurfaceVariant, onSurfaceVariant) && Equals(__other.outline, outline) && Equals(__other.outlineVariant, outlineVariant) && Equals(__other.shadow, shadow) && Equals(__other.scrim, scrim) && Equals(__other.inverseSurface, inverseSurface) && Equals(__other.onInverseSurface, onInverseSurface) && Equals(__other.inversePrimary, inversePrimary) && Equals(__other.surfaceTint, surfaceTint) && Equals(__other.background, background) && Equals(__other.onBackground, onBackground) && Equals(__other.surfaceVariant, surfaceVariant);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(brightness, primary, onPrimary, primaryContainer, onPrimaryContainer, secondary, onSecondary, secondaryContainer, onSecondaryContainer, tertiary, onTertiary, tertiaryContainer, onTertiaryContainer, error, onError, errorContainer, onErrorContainer, FoundationRuntimePorts.ObjectHash(surface, onSurface, surfaceDim, surfaceBright, surfaceContainerLowest, surfaceContainerLow, surfaceContainer, surfaceContainerHigh, surfaceContainerHighest, onSurfaceVariant, outline, outlineVariant, shadow, scrim, inverseSurface, onInverseSurface, inversePrimary, surfaceTint, FoundationRuntimePorts.ObjectHash(primaryFixed, primaryFixedDim, onPrimaryFixed, onPrimaryFixedVariant, secondaryFixed, secondaryFixedDim, onSecondaryFixed, onSecondaryFixedVariant, tertiaryFixed, tertiaryFixedDim, onTertiaryFixed, onTertiaryFixedVariant, background, onBackground, surfaceVariant))));
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultScheme = CreateLight();
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Brightness>("brightness", brightness, defaultValue: defaultScheme.brightness));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primary", primary, defaultValue: defaultScheme.primary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onPrimary", onPrimary, defaultValue: defaultScheme.onPrimary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryContainer", primaryContainer, defaultValue: defaultScheme.primaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onPrimaryContainer", onPrimaryContainer, defaultValue: defaultScheme.onPrimaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryFixed", primaryFixed, defaultValue: defaultScheme.primaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryFixedDim", primaryFixedDim, defaultValue: defaultScheme.primaryFixedDim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onPrimaryFixed", onPrimaryFixed, defaultValue: defaultScheme.onPrimaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onPrimaryFixedVariant", onPrimaryFixedVariant, defaultValue: defaultScheme.onPrimaryFixedVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondary", secondary, defaultValue: defaultScheme.secondary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSecondary", onSecondary, defaultValue: defaultScheme.onSecondary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryContainer", secondaryContainer, defaultValue: defaultScheme.secondaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSecondaryContainer", onSecondaryContainer, defaultValue: defaultScheme.onSecondaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryFixed", secondaryFixed, defaultValue: defaultScheme.secondaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryFixedDim", secondaryFixedDim, defaultValue: defaultScheme.secondaryFixedDim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSecondaryFixed", onSecondaryFixed, defaultValue: defaultScheme.onSecondaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSecondaryFixedVariant", onSecondaryFixedVariant, defaultValue: defaultScheme.onSecondaryFixedVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tertiary", tertiary, defaultValue: defaultScheme.tertiary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onTertiary", onTertiary, defaultValue: defaultScheme.onTertiary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tertiaryContainer", tertiaryContainer, defaultValue: defaultScheme.tertiaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onTertiaryContainer", onTertiaryContainer, defaultValue: defaultScheme.onTertiaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tertiaryFixed", tertiaryFixed, defaultValue: defaultScheme.tertiaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tertiaryFixedDim", tertiaryFixedDim, defaultValue: defaultScheme.tertiaryFixedDim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onTertiaryFixed", onTertiaryFixed, defaultValue: defaultScheme.onTertiaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onTertiaryFixedVariant", onTertiaryFixedVariant, defaultValue: defaultScheme.onTertiaryFixedVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("error", error, defaultValue: defaultScheme.error));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onError", onError, defaultValue: defaultScheme.onError));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("errorContainer", errorContainer, defaultValue: defaultScheme.errorContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onErrorContainer", onErrorContainer, defaultValue: defaultScheme.onErrorContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surface", surface, defaultValue: defaultScheme.surface));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSurface", onSurface, defaultValue: defaultScheme.onSurface));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceDim", surfaceDim, defaultValue: defaultScheme.surfaceDim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceBright", surfaceBright, defaultValue: defaultScheme.surfaceBright));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainerLowest", surfaceContainerLowest, defaultValue: defaultScheme.surfaceContainerLowest));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainerLow", surfaceContainerLow, defaultValue: defaultScheme.surfaceContainerLow));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainer", surfaceContainer, defaultValue: defaultScheme.surfaceContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainerHigh", surfaceContainerHigh, defaultValue: defaultScheme.surfaceContainerHigh));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainerHighest", surfaceContainerHighest, defaultValue: defaultScheme.surfaceContainerHighest));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSurfaceVariant", onSurfaceVariant, defaultValue: defaultScheme.onSurfaceVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("outline", outline, defaultValue: defaultScheme.outline));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("outlineVariant", outlineVariant, defaultValue: defaultScheme.outlineVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadow", shadow, defaultValue: defaultScheme.shadow));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("scrim", scrim, defaultValue: defaultScheme.scrim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inverseSurface", inverseSurface, defaultValue: defaultScheme.inverseSurface));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onInverseSurface", onInverseSurface, defaultValue: defaultScheme.onInverseSurface));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inversePrimary", inversePrimary, defaultValue: defaultScheme.inversePrimary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTint", surfaceTint, defaultValue: defaultScheme.surfaceTint));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("background", background, defaultValue: defaultScheme.background));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onBackground", onBackground, defaultValue: defaultScheme.onBackground));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceVariant", surfaceVariant, defaultValue: defaultScheme.surfaceVariant));
    }

    public static async Future<ColorScheme> fromImageProvider(global::Doroti.Framework.Painting.IImageProvider provider, Brightness brightness = Brightness.light, DynamicSchemeVariant dynamicSchemeVariant = DynamicSchemeVariant.tonalSpot, double contrastLevel = 0.0, Color? primary = null, Color? onPrimary = null, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color? secondary = null, Color? onSecondary = null, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color? error = null, Color? onError = null, Color? errorContainer = null, Color? onErrorContainer = null, Color? outline = null, Color? outlineVariant = null, Color? surface = null, Color? onSurface = null, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? shadow = null, Color? scrim = null, Color? surfaceTint = null, Color? background = null, Color? onBackground = null, Color? surfaceVariant = null)
    {
        QuantizerResult quantizerResult = await _extractColorsFromImageProvider(provider);
        DartMap<long, long> colorToCountLocal = quantizerResult.colorToCount;
        List<long> scoredResults = Score.score(colorToCountLocal, desired: 1L).ToList();
        var baseColor = new global::Doroti.Ui.Color(scoredResults.First());
        DynamicScheme scheme = _buildDynamicScheme(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(brightness)), baseColor, dynamicSchemeVariant, contrastLevel);
        return new ColorScheme(primary: primary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primary.getArgb(scheme)), onPrimary: onPrimary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimary.getArgb(scheme)), primaryContainer: primaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryContainer.getArgb(scheme)), onPrimaryContainer: onPrimaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryContainer.getArgb(scheme)), primaryFixed: primaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryFixed.getArgb(scheme)), primaryFixedDim: primaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryFixedDim.getArgb(scheme)), onPrimaryFixed: onPrimaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryFixed.getArgb(scheme)), onPrimaryFixedVariant: onPrimaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryFixedVariant.getArgb(scheme)), secondary: secondary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondary.getArgb(scheme)), onSecondary: onSecondary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondary.getArgb(scheme)), secondaryContainer: secondaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryContainer.getArgb(scheme)), onSecondaryContainer: onSecondaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryContainer.getArgb(scheme)), secondaryFixed: secondaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryFixed.getArgb(scheme)), secondaryFixedDim: secondaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryFixedDim.getArgb(scheme)), onSecondaryFixed: onSecondaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryFixed.getArgb(scheme)), onSecondaryFixedVariant: onSecondaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryFixedVariant.getArgb(scheme)), tertiary: tertiary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiary.getArgb(scheme)), onTertiary: onTertiary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiary.getArgb(scheme)), tertiaryContainer: tertiaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryContainer.getArgb(scheme)), onTertiaryContainer: onTertiaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryContainer.getArgb(scheme)), tertiaryFixed: tertiaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryFixed.getArgb(scheme)), tertiaryFixedDim: tertiaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryFixedDim.getArgb(scheme)), onTertiaryFixed: onTertiaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryFixed.getArgb(scheme)), onTertiaryFixedVariant: onTertiaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryFixedVariant.getArgb(scheme)), error: error ?? new global::Doroti.Ui.Color(MaterialDynamicColors.error.getArgb(scheme)), onError: onError ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onError.getArgb(scheme)), errorContainer: errorContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.errorContainer.getArgb(scheme)), onErrorContainer: onErrorContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onErrorContainer.getArgb(scheme)), outline: outline ?? new global::Doroti.Ui.Color(MaterialDynamicColors.outline.getArgb(scheme)), outlineVariant: outlineVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.outlineVariant.getArgb(scheme)), surface: surface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surface.getArgb(scheme)), surfaceDim: surfaceDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceDim.getArgb(scheme)), surfaceBright: surfaceBright ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceBright.getArgb(scheme)), surfaceContainerLowest: surfaceContainerLowest ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerLowest.getArgb(scheme)), surfaceContainerLow: surfaceContainerLow ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerLow.getArgb(scheme)), surfaceContainer: surfaceContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainer.getArgb(scheme)), surfaceContainerHigh: surfaceContainerHigh ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerHigh.getArgb(scheme)), surfaceContainerHighest: surfaceContainerHighest ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerHighest.getArgb(scheme)), onSurface: onSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSurface.getArgb(scheme)), onSurfaceVariant: onSurfaceVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSurfaceVariant.getArgb(scheme)), inverseSurface: inverseSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inverseSurface.getArgb(scheme)), onInverseSurface: onInverseSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inverseOnSurface.getArgb(scheme)), inversePrimary: inversePrimary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inversePrimary.getArgb(scheme)), shadow: shadow ?? new global::Doroti.Ui.Color(MaterialDynamicColors.shadow.getArgb(scheme)), scrim: scrim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.scrim.getArgb(scheme)), surfaceTint: surfaceTint ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primary.getArgb(scheme)), brightness: DartRuntimePrimitives.RequireValue(brightness), background: background ?? new global::Doroti.Ui.Color(MaterialDynamicColors.background.getArgb(scheme)), onBackground: onBackground ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onBackground.getArgb(scheme)), surfaceVariant: surfaceVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceVariant.getArgb(scheme)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static async Future<QuantizerResult> _extractColorsFromImageProvider(global::Doroti.Framework.Painting.IImageProvider imageProvider)
    {
        using global::Doroti.Ui.Image scaledImage = await _imageProviderToScaled(imageProvider);
        var bytes = await scaledImage.toByteData(ImageByteFormat.rawRgba)
            ?? throw new InvalidOperationException("Image readback returned no RGBA bytes.");
        return await new QuantizerCelebi().quantize(MaterialImageColorRuntime.ArgbFromRgba(bytes.asMemory().Span), 128L);
    }

    internal static async Future<global::Doroti.Ui.Image> _imageProviderToScaled(global::Doroti.Framework.Painting.IImageProvider imageProvider)
    {
        var stream = imageProvider.resolve(
            new global::Doroti.Framework.Painting.ImageConfiguration(size: new Size(112, 112)));
        var completion = new TaskCompletionSource<global::Doroti.Ui.Image>(TaskCreationOptions.RunContinuationsAsynchronously);
        var admitted = 0;
        var removed = 0;
        global::Doroti.Framework.Painting.ImageStreamListener listener = null!;
        void RemoveListener()
        {
            if (Interlocked.Exchange(ref removed, 1) == 0) stream.removeListener(listener);
        }
        listener = new global::Doroti.Framework.Painting.ImageStreamListener(async (info, synchronous) =>
        {
            using var image = info.image;
            if (Interlocked.Exchange(ref admitted, 1) != 0) return;
            try
            {
                RemoveListener();
                if (image.width <= 0 || image.height <= 0) throw new InvalidDataException("Image has invalid dimensions.");
                var scale = Math.Min(1.0, 112.0 / Math.Max(image.width, image.height));
                var paintWidth = image.width * scale;
                var paintHeight = image.height * scale;
                var recorder = new PictureRecorder();
                var canvas = new Canvas(recorder);
                Decoration_imageLibrary.paintImage(canvas: canvas,
                    rect: Rect.fromLTRB(0, 0, paintWidth, paintHeight), image: image, filterQuality: FilterQuality.none);
                using var picture = recorder.endRecording();
                var scaled = await picture.toImage(Math.Max(1, (int)paintWidth), Math.Max(1, (int)paintHeight));
                if (!completion.TrySetResult(scaled)) scaled.Dispose();
            }
            catch (Exception exception) { completion.TrySetException(exception); }
        }, onError: (exception, stack) => completion.TrySetException(
            exception as Exception ?? new InvalidOperationException($"Failed to load image: {exception}")));
        try
        {
            stream.addListener(listener);
            return await completion.Task.WaitAsync(TimeSpan.FromSeconds(30), DartAsyncRuntime.timeProvider);
        }
        catch (Exception exception)
        {
            // Late raster completions must release their output instead of leaking after timeout.
            completion.TrySetException(exception);
            throw;
        }
        finally { RemoveListener(); }
    }

    internal static long _getArgbFromAbgr(long abgr)
    {
        var exceptRMask = 4278255615L;
        long onlyRMask = ~exceptRMask;
        var exceptBMask = 4294967040L;
        long onlyBMask = ~exceptBMask;
        long r = (abgr & onlyRMask) >> (int)16L;
        long b = abgr & onlyBMask;
        return abgr & exceptRMask & exceptBMask | b << (int)16L | r;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static DynamicScheme _buildDynamicScheme(Brightness brightness, Color seedColor, DynamicSchemeVariant schemeVariant, double contrastLevel)
    {
        DartRuntimePrimitives.Assert(() => (contrastLevel >= -1.0) && (contrastLevel <= 1.0), () => (object?)"contrastLevel must be between -1.0 and 1.0 inclusive.");
        var isDarkLocal = Equals(DartRuntimePrimitives.RequireValue(brightness), Brightness.dark);
        Hct sourceColor = Hct.fromInt(seedColor.value);
        return schemeVariant switch { DynamicSchemeVariant.tonalSpot => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeTonalSpot(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), DynamicSchemeVariant.fidelity => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeFidelity(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), DynamicSchemeVariant.content => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeContent(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), DynamicSchemeVariant.monochrome => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeMonochrome(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), DynamicSchemeVariant.neutral => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeNeutral(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), DynamicSchemeVariant.vibrant => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeVibrant(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), DynamicSchemeVariant.expressive => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeExpressive(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), DynamicSchemeVariant.rainbow => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeRainbow(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), DynamicSchemeVariant.fruitSalad => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeFruitSalad(sourceColorHct: sourceColor, isDark: isDarkLocal, contrastLevel: contrastLevel)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ColorScheme of(global::Doroti.Framework.Widgets.BuildContext context) => Theme.of(context).colorScheme;
    public virtual string toStringShort() => DiagnosticsLibrary.describeIdentity(this);
    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info)
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

    public virtual DiagnosticsNode toDiagnosticsNode(string? name = null, DiagnosticsTreeStyle? style = null)
    {
        return new DiagnosticableNode<Diagnosticable>(name: name, value: this, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
