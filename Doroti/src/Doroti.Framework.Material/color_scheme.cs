// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/color_scheme.dart
using System;
using System.Collections.Generic;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Material;
internal sealed class QuantizerResult
{
    internal DartMap<long, long> colorToCount { get; } = new();
}

internal sealed class QuantizerCelebi
{
    internal Future<QuantizerResult> quantize(dynamic pixels, long maxColors, bool returnInputPixelToClusterPixel = false)
    {
        var result = new QuantizerResult();
        foreach (var pixel in pixels) { var value = Convert.ToInt64(pixel); result.colorToCount[value] = result.colorToCount.GetValueOrDefault(value) + 1; }
        if (result.colorToCount.Count == 0) result.colorToCount[0xff6750a4] = 1;
        return Future<QuantizerResult>.value(result);
    }
}

internal static class Score
{
    internal static IEnumerable<long> score(DartMap<long, long> colors, long desired = 1) =>
        colors.OrderByDescending(entry => entry.Value).Take(checked((int)desired)).Select(entry => entry.Key);
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
        this._primaryContainer = primaryContainer;
        this._onPrimaryContainer = onPrimaryContainer;
        this._primaryFixed = primaryFixed;
        this._primaryFixedDim = primaryFixedDim;
        this._onPrimaryFixed = onPrimaryFixed;
        this._onPrimaryFixedVariant = onPrimaryFixedVariant;
        this._secondaryContainer = secondaryContainer;
        this._onSecondaryContainer = onSecondaryContainer;
        this._secondaryFixed = secondaryFixed;
        this._secondaryFixedDim = secondaryFixedDim;
        this._onSecondaryFixed = onSecondaryFixed;
        this._onSecondaryFixedVariant = onSecondaryFixedVariant;
        this._tertiary = tertiary;
        this._onTertiary = onTertiary;
        this._tertiaryContainer = tertiaryContainer;
        this._onTertiaryContainer = onTertiaryContainer;
        this._tertiaryFixed = tertiaryFixed;
        this._tertiaryFixedDim = tertiaryFixedDim;
        this._onTertiaryFixed = onTertiaryFixed;
        this._onTertiaryFixedVariant = onTertiaryFixedVariant;
        this._errorContainer = errorContainer;
        this._onErrorContainer = onErrorContainer;
        this._surfaceDim = surfaceDim;
        this._surfaceBright = surfaceBright;
        this._surfaceContainerLowest = surfaceContainerLowest;
        this._surfaceContainerLow = surfaceContainerLow;
        this._surfaceContainer = surfaceContainer;
        this._surfaceContainerHigh = surfaceContainerHigh;
        this._surfaceContainerHighest = surfaceContainerHighest;
        this._onSurfaceVariant = onSurfaceVariant;
        this._outline = outline;
        this._outlineVariant = outlineVariant;
        this._shadow = shadow;
        this._scrim = scrim;
        this._inverseSurface = inverseSurface;
        this._onInverseSurface = onInverseSurface;
        this._inversePrimary = inversePrimary;
        this._surfaceTint = surfaceTint;
        this._background = background;
        this._onBackground = onBackground;
        this._surfaceVariant = surfaceVariant;
    }

    public static ColorScheme CreateFromSeed(Color seedColor, Brightness brightness = Brightness.light, DynamicSchemeVariant dynamicSchemeVariant = DynamicSchemeVariant.tonalSpot, double contrastLevel = 0.0, Color? primary = null, Color? onPrimary = null, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color? secondary = null, Color? onSecondary = null, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color? error = null, Color? onError = null, Color? errorContainer = null, Color? onErrorContainer = null, Color? outline = null, Color? outlineVariant = null, Color? surface = null, Color? onSurface = null, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? shadow = null, Color? scrim = null, Color? surfaceTint = null, Color? background = null, Color? onBackground = null, Color? surfaceVariant = null)
    {
        DynamicScheme scheme__14456 = ((DynamicScheme)(object?)ColorScheme._buildDynamicScheme(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(brightness)), seedColor, dynamicSchemeVariant, contrastLevel));
        return new ColorScheme(primary: (primary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primary.getArgb(scheme__14456))), onPrimary: (onPrimary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimary.getArgb(scheme__14456))), primaryContainer: (primaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryContainer.getArgb(scheme__14456))), onPrimaryContainer: (onPrimaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryContainer.getArgb(scheme__14456))), primaryFixed: (primaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryFixed.getArgb(scheme__14456))), primaryFixedDim: (primaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryFixedDim.getArgb(scheme__14456))), onPrimaryFixed: (onPrimaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryFixed.getArgb(scheme__14456))), onPrimaryFixedVariant: (onPrimaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryFixedVariant.getArgb(scheme__14456))), secondary: (secondary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondary.getArgb(scheme__14456))), onSecondary: (onSecondary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondary.getArgb(scheme__14456))), secondaryContainer: (secondaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryContainer.getArgb(scheme__14456))), onSecondaryContainer: (onSecondaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryContainer.getArgb(scheme__14456))), secondaryFixed: (secondaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryFixed.getArgb(scheme__14456))), secondaryFixedDim: (secondaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryFixedDim.getArgb(scheme__14456))), onSecondaryFixed: (onSecondaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryFixed.getArgb(scheme__14456))), onSecondaryFixedVariant: (onSecondaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryFixedVariant.getArgb(scheme__14456))), tertiary: (tertiary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiary.getArgb(scheme__14456))), onTertiary: (onTertiary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiary.getArgb(scheme__14456))), tertiaryContainer: (tertiaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryContainer.getArgb(scheme__14456))), onTertiaryContainer: (onTertiaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryContainer.getArgb(scheme__14456))), tertiaryFixed: (tertiaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryFixed.getArgb(scheme__14456))), tertiaryFixedDim: (tertiaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryFixedDim.getArgb(scheme__14456))), onTertiaryFixed: (onTertiaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryFixed.getArgb(scheme__14456))), onTertiaryFixedVariant: (onTertiaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryFixedVariant.getArgb(scheme__14456))), error: (error ?? new global::Doroti.Ui.Color(MaterialDynamicColors.error.getArgb(scheme__14456))), onError: (onError ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onError.getArgb(scheme__14456))), errorContainer: (errorContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.errorContainer.getArgb(scheme__14456))), onErrorContainer: (onErrorContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onErrorContainer.getArgb(scheme__14456))), outline: (outline ?? new global::Doroti.Ui.Color(MaterialDynamicColors.outline.getArgb(scheme__14456))), outlineVariant: (outlineVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.outlineVariant.getArgb(scheme__14456))), surface: (surface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surface.getArgb(scheme__14456))), surfaceDim: (surfaceDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceDim.getArgb(scheme__14456))), surfaceBright: (surfaceBright ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceBright.getArgb(scheme__14456))), surfaceContainerLowest: (surfaceContainerLowest ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerLowest.getArgb(scheme__14456))), surfaceContainerLow: (surfaceContainerLow ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerLow.getArgb(scheme__14456))), surfaceContainer: (surfaceContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainer.getArgb(scheme__14456))), surfaceContainerHigh: (surfaceContainerHigh ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerHigh.getArgb(scheme__14456))), surfaceContainerHighest: (surfaceContainerHighest ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerHighest.getArgb(scheme__14456))), onSurface: (onSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSurface.getArgb(scheme__14456))), onSurfaceVariant: (onSurfaceVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSurfaceVariant.getArgb(scheme__14456))), inverseSurface: (inverseSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inverseSurface.getArgb(scheme__14456))), onInverseSurface: (onInverseSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inverseOnSurface.getArgb(scheme__14456))), inversePrimary: (inversePrimary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inversePrimary.getArgb(scheme__14456))), shadow: (shadow ?? new global::Doroti.Ui.Color(MaterialDynamicColors.shadow.getArgb(scheme__14456))), scrim: (scrim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.scrim.getArgb(scheme__14456))), surfaceTint: (surfaceTint ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primary.getArgb(scheme__14456))), brightness: DartRuntimePrimitives.RequireValue(brightness), background: (background ?? new global::Doroti.Ui.Color(MaterialDynamicColors.background.getArgb(scheme__14456))), onBackground: (onBackground ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onBackground.getArgb(scheme__14456))), surfaceVariant: (surfaceVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceVariant.getArgb(scheme__14456))));
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
        var isDark__40190 = (object.Equals(DartRuntimePrimitives.RequireValue(brightness), Brightness.dark));
        var primaryIsDark__40240 = (object.Equals(ColorScheme._brightnessFor(__primarySwatch), Brightness.dark));
        global::Doroti.Ui.Color secondary__40322 = ((global::Doroti.Ui.Color)(object?)(accentColor ?? ((isDark__40190 ? Colors.tealAccent[200L]! : __primarySwatch))));
        var secondaryIsDark__40411 = (object.Equals(ColorScheme._brightnessFor(secondary__40322), Brightness.dark));
        return new ColorScheme(primary: __primarySwatch, secondary: secondary__40322, surface: (cardColor ?? ((isDark__40190 ? Colors.grey[800L]! : Colors.white))), error: (errorColor ?? Colors.red[700L]!), onPrimary: (primaryIsDark__40240 ? Colors.white : Colors.black), onSecondary: (secondaryIsDark__40411 ? Colors.white : Colors.black), onSurface: (isDark__40190 ? Colors.white : Colors.black), onError: (isDark__40190 ? Colors.black : Colors.white), brightness: DartRuntimePrimitives.RequireValue(brightness), background: (backgroundColor ?? ((isDark__40190 ? Colors.grey[700L]! : __primarySwatch[200L]!))), onBackground: (primaryIsDark__40240 ? Colors.white : Colors.black));
    }

    internal static global::Doroti.Ui.Brightness _brightnessFor(Color color) => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Brightness>(ThemeData.estimateBrightnessForColor(color));
    public virtual global::Doroti.Ui.Color primaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._primaryContainer ?? this.primary));
    public virtual global::Doroti.Ui.Color onPrimaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onPrimaryContainer ?? this.onPrimary));
    public virtual global::Doroti.Ui.Color primaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._primaryFixed ?? this.primary));
    public virtual global::Doroti.Ui.Color primaryFixedDim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._primaryFixedDim ?? this.primary));
    public virtual global::Doroti.Ui.Color onPrimaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onPrimaryFixed ?? this.onPrimary));
    public virtual global::Doroti.Ui.Color onPrimaryFixedVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onPrimaryFixedVariant ?? this.onPrimary));
    public virtual global::Doroti.Ui.Color secondaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._secondaryContainer ?? this.secondary));
    public virtual global::Doroti.Ui.Color onSecondaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onSecondaryContainer ?? this.onSecondary));
    public virtual global::Doroti.Ui.Color secondaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._secondaryFixed ?? this.secondary));
    public virtual global::Doroti.Ui.Color secondaryFixedDim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._secondaryFixedDim ?? this.secondary));
    public virtual global::Doroti.Ui.Color onSecondaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onSecondaryFixed ?? this.onSecondary));
    public virtual global::Doroti.Ui.Color onSecondaryFixedVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onSecondaryFixedVariant ?? this.onSecondary));
    public virtual global::Doroti.Ui.Color tertiary => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._tertiary ?? this.secondary));
    public virtual global::Doroti.Ui.Color onTertiary => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onTertiary ?? this.onSecondary));
    public virtual global::Doroti.Ui.Color tertiaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._tertiaryContainer ?? (Color)this.tertiary)));
    public virtual global::Doroti.Ui.Color onTertiaryContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._onTertiaryContainer ?? (Color)this.onTertiary)));
    public virtual global::Doroti.Ui.Color tertiaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._tertiaryFixed ?? (Color)this.tertiary)));
    public virtual global::Doroti.Ui.Color tertiaryFixedDim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._tertiaryFixedDim ?? (Color)this.tertiary)));
    public virtual global::Doroti.Ui.Color onTertiaryFixed => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._onTertiaryFixed ?? (Color)this.onTertiary)));
    public virtual global::Doroti.Ui.Color onTertiaryFixedVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._onTertiaryFixedVariant ?? (Color)this.onTertiary)));
    public virtual global::Doroti.Ui.Color errorContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._errorContainer ?? this.error));
    public virtual global::Doroti.Ui.Color onErrorContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onErrorContainer ?? this.onError));
    public virtual global::Doroti.Ui.Color surfaceVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceVariant ?? this.surface));
    public virtual global::Doroti.Ui.Color surfaceDim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceDim ?? this.surface));
    public virtual global::Doroti.Ui.Color surfaceBright => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceBright ?? this.surface));
    public virtual global::Doroti.Ui.Color surfaceContainerLowest => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceContainerLowest ?? this.surface));
    public virtual global::Doroti.Ui.Color surfaceContainerLow => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceContainerLow ?? this.surface));
    public virtual global::Doroti.Ui.Color surfaceContainer => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceContainer ?? this.surface));
    public virtual global::Doroti.Ui.Color surfaceContainerHigh => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceContainerHigh ?? this.surface));
    public virtual global::Doroti.Ui.Color surfaceContainerHighest => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceContainerHighest ?? this.surface));
    public virtual global::Doroti.Ui.Color onSurfaceVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onSurfaceVariant ?? this.onSurface));
    public virtual global::Doroti.Ui.Color outline => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._outline ?? (Color)this.onBackground)));
    public virtual global::Doroti.Ui.Color outlineVariant => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>(((this._outlineVariant ?? (Color)this.onBackground)));
    public virtual global::Doroti.Ui.Color shadow => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._shadow ?? new global::Doroti.Ui.Color(4278190080L)));
    public virtual global::Doroti.Ui.Color scrim => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._scrim ?? new global::Doroti.Ui.Color(4278190080L)));
    public virtual global::Doroti.Ui.Color inverseSurface => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._inverseSurface ?? this.onSurface));
    public virtual global::Doroti.Ui.Color onInverseSurface => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onInverseSurface ?? this.surface));
    public virtual global::Doroti.Ui.Color inversePrimary => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._inversePrimary ?? this.onPrimary));
    public virtual global::Doroti.Ui.Color surfaceTint => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._surfaceTint ?? this.primary));
    public virtual global::Doroti.Ui.Color background => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._background ?? this.surface));
    public virtual global::Doroti.Ui.Color onBackground => DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Color>((this._onBackground ?? this.onSurface));
    public virtual ColorScheme copyWith(Brightness? brightness = null, Color? primary = null, Color? onPrimary = null, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color? secondary = null, Color? onSecondary = null, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color? error = null, Color? onError = null, Color? errorContainer = null, Color? onErrorContainer = null, Color? surface = null, Color? onSurface = null, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? outline = null, Color? outlineVariant = null, Color? shadow = null, Color? scrim = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? surfaceTint = null, Color? background = null, Color? onBackground = null, Color? surfaceVariant = null)
    {
        return new ColorScheme(brightness: (brightness ?? this.brightness), primary: (primary ?? this.primary), onPrimary: (onPrimary ?? this.onPrimary), primaryContainer: (primaryContainer ?? this.primaryContainer), onPrimaryContainer: (onPrimaryContainer ?? this.onPrimaryContainer), primaryFixed: (primaryFixed ?? this.primaryFixed), primaryFixedDim: (primaryFixedDim ?? this.primaryFixedDim), onPrimaryFixed: (onPrimaryFixed ?? this.onPrimaryFixed), onPrimaryFixedVariant: (onPrimaryFixedVariant ?? this.onPrimaryFixedVariant), secondary: (secondary ?? this.secondary), onSecondary: (onSecondary ?? this.onSecondary), secondaryContainer: (secondaryContainer ?? this.secondaryContainer), onSecondaryContainer: (onSecondaryContainer ?? this.onSecondaryContainer), secondaryFixed: (secondaryFixed ?? this.secondaryFixed), secondaryFixedDim: (secondaryFixedDim ?? this.secondaryFixedDim), onSecondaryFixed: (onSecondaryFixed ?? this.onSecondaryFixed), onSecondaryFixedVariant: (onSecondaryFixedVariant ?? this.onSecondaryFixedVariant), tertiary: (tertiary ?? this.tertiary), onTertiary: (onTertiary ?? this.onTertiary), tertiaryContainer: (tertiaryContainer ?? this.tertiaryContainer), onTertiaryContainer: (onTertiaryContainer ?? this.onTertiaryContainer), tertiaryFixed: (tertiaryFixed ?? this.tertiaryFixed), tertiaryFixedDim: (tertiaryFixedDim ?? this.tertiaryFixedDim), onTertiaryFixed: (onTertiaryFixed ?? this.onTertiaryFixed), onTertiaryFixedVariant: (onTertiaryFixedVariant ?? this.onTertiaryFixedVariant), error: (error ?? this.error), onError: (onError ?? this.onError), errorContainer: (errorContainer ?? this.errorContainer), onErrorContainer: (onErrorContainer ?? this.onErrorContainer), surface: (surface ?? this.surface), onSurface: (onSurface ?? this.onSurface), surfaceDim: (surfaceDim ?? this.surfaceDim), surfaceBright: (surfaceBright ?? this.surfaceBright), surfaceContainerLowest: (surfaceContainerLowest ?? this.surfaceContainerLowest), surfaceContainerLow: (surfaceContainerLow ?? this.surfaceContainerLow), surfaceContainer: (surfaceContainer ?? this.surfaceContainer), surfaceContainerHigh: (surfaceContainerHigh ?? this.surfaceContainerHigh), surfaceContainerHighest: (surfaceContainerHighest ?? this.surfaceContainerHighest), onSurfaceVariant: (onSurfaceVariant ?? this.onSurfaceVariant), outline: (outline ?? this.outline), outlineVariant: (outlineVariant ?? this.outlineVariant), shadow: (shadow ?? this.shadow), scrim: (scrim ?? this.scrim), inverseSurface: (inverseSurface ?? this.inverseSurface), onInverseSurface: (onInverseSurface ?? this.onInverseSurface), inversePrimary: (inversePrimary ?? this.inversePrimary), surfaceTint: (surfaceTint ?? this.surfaceTint), background: (background ?? this.background), onBackground: (onBackground ?? this.onBackground), surfaceVariant: (surfaceVariant ?? this.surfaceVariant));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ColorScheme lerp(ColorScheme a, ColorScheme b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        return new ColorScheme(brightness: ((t < 0.5) ? ((ColorScheme)a).brightness : ((ColorScheme)b).brightness), primary: Dart_uiLibrary.Color.lerp(((ColorScheme)a).primary, ((ColorScheme)b).primary, t)!, onPrimary: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onPrimary, ((ColorScheme)b).onPrimary, t)!, primaryContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).primaryContainer, ((ColorScheme)b).primaryContainer, t), onPrimaryContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onPrimaryContainer, ((ColorScheme)b).onPrimaryContainer, t), primaryFixed: Dart_uiLibrary.Color.lerp(((ColorScheme)a).primaryFixed, ((ColorScheme)b).primaryFixed, t), primaryFixedDim: Dart_uiLibrary.Color.lerp(((ColorScheme)a).primaryFixedDim, ((ColorScheme)b).primaryFixedDim, t), onPrimaryFixed: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onPrimaryFixed, ((ColorScheme)b).onPrimaryFixed, t), onPrimaryFixedVariant: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onPrimaryFixedVariant, ((ColorScheme)b).onPrimaryFixedVariant, t), secondary: Dart_uiLibrary.Color.lerp(((ColorScheme)a).secondary, ((ColorScheme)b).secondary, t)!, onSecondary: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onSecondary, ((ColorScheme)b).onSecondary, t)!, secondaryContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).secondaryContainer, ((ColorScheme)b).secondaryContainer, t), onSecondaryContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onSecondaryContainer, ((ColorScheme)b).onSecondaryContainer, t), secondaryFixed: Dart_uiLibrary.Color.lerp(((ColorScheme)a).secondaryFixed, ((ColorScheme)b).secondaryFixed, t), secondaryFixedDim: Dart_uiLibrary.Color.lerp(((ColorScheme)a).secondaryFixedDim, ((ColorScheme)b).secondaryFixedDim, t), onSecondaryFixed: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onSecondaryFixed, ((ColorScheme)b).onSecondaryFixed, t), onSecondaryFixedVariant: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onSecondaryFixedVariant, ((ColorScheme)b).onSecondaryFixedVariant, t), tertiary: Dart_uiLibrary.Color.lerp(((ColorScheme)a).tertiary, ((ColorScheme)b).tertiary, t), onTertiary: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onTertiary, ((ColorScheme)b).onTertiary, t), tertiaryContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).tertiaryContainer, ((ColorScheme)b).tertiaryContainer, t), onTertiaryContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onTertiaryContainer, ((ColorScheme)b).onTertiaryContainer, t), tertiaryFixed: Dart_uiLibrary.Color.lerp(((ColorScheme)a).tertiaryFixed, ((ColorScheme)b).tertiaryFixed, t), tertiaryFixedDim: Dart_uiLibrary.Color.lerp(((ColorScheme)a).tertiaryFixedDim, ((ColorScheme)b).tertiaryFixedDim, t), onTertiaryFixed: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onTertiaryFixed, ((ColorScheme)b).onTertiaryFixed, t), onTertiaryFixedVariant: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onTertiaryFixedVariant, ((ColorScheme)b).onTertiaryFixedVariant, t), error: Dart_uiLibrary.Color.lerp(((ColorScheme)a).error, ((ColorScheme)b).error, t)!, onError: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onError, ((ColorScheme)b).onError, t)!, errorContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).errorContainer, ((ColorScheme)b).errorContainer, t), onErrorContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onErrorContainer, ((ColorScheme)b).onErrorContainer, t), surface: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surface, ((ColorScheme)b).surface, t)!, onSurface: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onSurface, ((ColorScheme)b).onSurface, t)!, surfaceDim: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceDim, ((ColorScheme)b).surfaceDim, t), surfaceBright: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceBright, ((ColorScheme)b).surfaceBright, t), surfaceContainerLowest: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceContainerLowest, ((ColorScheme)b).surfaceContainerLowest, t), surfaceContainerLow: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceContainerLow, ((ColorScheme)b).surfaceContainerLow, t), surfaceContainer: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceContainer, ((ColorScheme)b).surfaceContainer, t), surfaceContainerHigh: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceContainerHigh, ((ColorScheme)b).surfaceContainerHigh, t), surfaceContainerHighest: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceContainerHighest, ((ColorScheme)b).surfaceContainerHighest, t), onSurfaceVariant: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onSurfaceVariant, ((ColorScheme)b).onSurfaceVariant, t), outline: Dart_uiLibrary.Color.lerp(((ColorScheme)a).outline, ((ColorScheme)b).outline, t), outlineVariant: Dart_uiLibrary.Color.lerp(((ColorScheme)a).outlineVariant, ((ColorScheme)b).outlineVariant, t), shadow: Dart_uiLibrary.Color.lerp(((ColorScheme)a).shadow, ((ColorScheme)b).shadow, t), scrim: Dart_uiLibrary.Color.lerp(((ColorScheme)a).scrim, ((ColorScheme)b).scrim, t), inverseSurface: Dart_uiLibrary.Color.lerp(((ColorScheme)a).inverseSurface, ((ColorScheme)b).inverseSurface, t), onInverseSurface: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onInverseSurface, ((ColorScheme)b).onInverseSurface, t), inversePrimary: Dart_uiLibrary.Color.lerp(((ColorScheme)a).inversePrimary, ((ColorScheme)b).inversePrimary, t), surfaceTint: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceTint, ((ColorScheme)b).surfaceTint, t), background: Dart_uiLibrary.Color.lerp(((ColorScheme)a).background, ((ColorScheme)b).background, t), onBackground: Dart_uiLibrary.Color.lerp(((ColorScheme)a).onBackground, ((ColorScheme)b).onBackground, t), surfaceVariant: Dart_uiLibrary.Color.lerp(((ColorScheme)a).surfaceVariant, ((ColorScheme)b).surfaceVariant, t));
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
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((((((((((((((((((((((((((((((((((((((((((((((((__other is ColorScheme) && (object.Equals(((ColorScheme)((ColorScheme)__other)).brightness, this.brightness))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).primary, this.primary))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onPrimary, this.onPrimary))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).primaryContainer, this.primaryContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onPrimaryContainer, this.onPrimaryContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).primaryFixed, this.primaryFixed))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).primaryFixedDim, this.primaryFixedDim))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onPrimaryFixed, this.onPrimaryFixed))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onPrimaryFixedVariant, this.onPrimaryFixedVariant))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).secondary, this.secondary))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onSecondary, this.onSecondary))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).secondaryContainer, this.secondaryContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onSecondaryContainer, this.onSecondaryContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).secondaryFixed, this.secondaryFixed))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).secondaryFixedDim, this.secondaryFixedDim))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onSecondaryFixed, this.onSecondaryFixed))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onSecondaryFixedVariant, this.onSecondaryFixedVariant))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).tertiary, this.tertiary))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onTertiary, this.onTertiary))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).tertiaryContainer, this.tertiaryContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onTertiaryContainer, this.onTertiaryContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).tertiaryFixed, this.tertiaryFixed))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).tertiaryFixedDim, this.tertiaryFixedDim))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onTertiaryFixed, this.onTertiaryFixed))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onTertiaryFixedVariant, this.onTertiaryFixedVariant))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).error, this.error))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onError, this.onError))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).errorContainer, this.errorContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onErrorContainer, this.onErrorContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surface, this.surface))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onSurface, this.onSurface))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceDim, this.surfaceDim))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceBright, this.surfaceBright))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceContainerLowest, this.surfaceContainerLowest))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceContainerLow, this.surfaceContainerLow))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceContainer, this.surfaceContainer))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceContainerHigh, this.surfaceContainerHigh))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceContainerHighest, this.surfaceContainerHighest))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onSurfaceVariant, this.onSurfaceVariant))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).outline, this.outline))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).outlineVariant, this.outlineVariant))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).shadow, this.shadow))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).scrim, this.scrim))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).inverseSurface, this.inverseSurface))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onInverseSurface, this.onInverseSurface))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).inversePrimary, this.inversePrimary))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceTint, this.surfaceTint))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).background, this.background))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).onBackground, this.onBackground))) && (object.Equals(((ColorScheme)((ColorScheme)__other)).surfaceVariant, this.surfaceVariant)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.brightness, this.primary, this.onPrimary, this.primaryContainer, this.onPrimaryContainer, this.secondary, this.onSecondary, this.secondaryContainer, this.onSecondaryContainer, this.tertiary, this.onTertiary, this.tertiaryContainer, this.onTertiaryContainer, this.error, this.onError, this.errorContainer, this.onErrorContainer, FoundationRuntimePorts.ObjectHash(this.surface, this.onSurface, this.surfaceDim, this.surfaceBright, this.surfaceContainerLowest, this.surfaceContainerLow, this.surfaceContainer, this.surfaceContainerHigh, this.surfaceContainerHighest, this.onSurfaceVariant, this.outline, this.outlineVariant, this.shadow, this.scrim, this.inverseSurface, this.onInverseSurface, this.inversePrimary, this.surfaceTint, FoundationRuntimePorts.ObjectHash(this.primaryFixed, this.primaryFixedDim, this.onPrimaryFixed, this.onPrimaryFixedVariant, this.secondaryFixed, this.secondaryFixedDim, this.onSecondaryFixed, this.onSecondaryFixedVariant, this.tertiaryFixed, this.tertiaryFixedDim, this.onTertiaryFixed, this.onTertiaryFixedVariant, this.background, this.onBackground, this.surfaceVariant))));
    public virtual void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        var defaultScheme__66720 = ColorScheme.CreateLight();
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<global::Doroti.Ui.Brightness>("brightness", this.brightness, defaultValue: ((ColorScheme)defaultScheme__66720).brightness));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primary", this.primary, defaultValue: ((ColorScheme)defaultScheme__66720).primary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onPrimary", this.onPrimary, defaultValue: ((ColorScheme)defaultScheme__66720).onPrimary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryContainer", this.primaryContainer, defaultValue: ((ColorScheme)defaultScheme__66720).primaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onPrimaryContainer", this.onPrimaryContainer, defaultValue: ((ColorScheme)defaultScheme__66720).onPrimaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryFixed", this.primaryFixed, defaultValue: ((ColorScheme)defaultScheme__66720).primaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("primaryFixedDim", this.primaryFixedDim, defaultValue: ((ColorScheme)defaultScheme__66720).primaryFixedDim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onPrimaryFixed", this.onPrimaryFixed, defaultValue: ((ColorScheme)defaultScheme__66720).onPrimaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onPrimaryFixedVariant", this.onPrimaryFixedVariant, defaultValue: ((ColorScheme)defaultScheme__66720).onPrimaryFixedVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondary", this.secondary, defaultValue: ((ColorScheme)defaultScheme__66720).secondary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSecondary", this.onSecondary, defaultValue: ((ColorScheme)defaultScheme__66720).onSecondary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryContainer", this.secondaryContainer, defaultValue: ((ColorScheme)defaultScheme__66720).secondaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSecondaryContainer", this.onSecondaryContainer, defaultValue: ((ColorScheme)defaultScheme__66720).onSecondaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryFixed", this.secondaryFixed, defaultValue: ((ColorScheme)defaultScheme__66720).secondaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("secondaryFixedDim", this.secondaryFixedDim, defaultValue: ((ColorScheme)defaultScheme__66720).secondaryFixedDim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSecondaryFixed", this.onSecondaryFixed, defaultValue: ((ColorScheme)defaultScheme__66720).onSecondaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSecondaryFixedVariant", this.onSecondaryFixedVariant, defaultValue: ((ColorScheme)defaultScheme__66720).onSecondaryFixedVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tertiary", this.tertiary, defaultValue: ((ColorScheme)defaultScheme__66720).tertiary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onTertiary", this.onTertiary, defaultValue: ((ColorScheme)defaultScheme__66720).onTertiary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tertiaryContainer", this.tertiaryContainer, defaultValue: ((ColorScheme)defaultScheme__66720).tertiaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onTertiaryContainer", this.onTertiaryContainer, defaultValue: ((ColorScheme)defaultScheme__66720).onTertiaryContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tertiaryFixed", this.tertiaryFixed, defaultValue: ((ColorScheme)defaultScheme__66720).tertiaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("tertiaryFixedDim", this.tertiaryFixedDim, defaultValue: ((ColorScheme)defaultScheme__66720).tertiaryFixedDim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onTertiaryFixed", this.onTertiaryFixed, defaultValue: ((ColorScheme)defaultScheme__66720).onTertiaryFixed));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onTertiaryFixedVariant", this.onTertiaryFixedVariant, defaultValue: ((ColorScheme)defaultScheme__66720).onTertiaryFixedVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("error", this.error, defaultValue: ((ColorScheme)defaultScheme__66720).error));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onError", this.onError, defaultValue: ((ColorScheme)defaultScheme__66720).onError));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("errorContainer", this.errorContainer, defaultValue: ((ColorScheme)defaultScheme__66720).errorContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onErrorContainer", this.onErrorContainer, defaultValue: ((ColorScheme)defaultScheme__66720).onErrorContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surface", this.surface, defaultValue: ((ColorScheme)defaultScheme__66720).surface));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSurface", this.onSurface, defaultValue: ((ColorScheme)defaultScheme__66720).onSurface));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceDim", this.surfaceDim, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceDim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceBright", this.surfaceBright, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceBright));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainerLowest", this.surfaceContainerLowest, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceContainerLowest));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainerLow", this.surfaceContainerLow, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceContainerLow));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainer", this.surfaceContainer, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceContainer));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainerHigh", this.surfaceContainerHigh, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceContainerHigh));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceContainerHighest", this.surfaceContainerHighest, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceContainerHighest));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onSurfaceVariant", this.onSurfaceVariant, defaultValue: ((ColorScheme)defaultScheme__66720).onSurfaceVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("outline", this.outline, defaultValue: ((ColorScheme)defaultScheme__66720).outline));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("outlineVariant", this.outlineVariant, defaultValue: ((ColorScheme)defaultScheme__66720).outlineVariant));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("shadow", this.shadow, defaultValue: ((ColorScheme)defaultScheme__66720).shadow));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("scrim", this.scrim, defaultValue: ((ColorScheme)defaultScheme__66720).scrim));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inverseSurface", this.inverseSurface, defaultValue: ((ColorScheme)defaultScheme__66720).inverseSurface));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onInverseSurface", this.onInverseSurface, defaultValue: ((ColorScheme)defaultScheme__66720).onInverseSurface));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("inversePrimary", this.inversePrimary, defaultValue: ((ColorScheme)defaultScheme__66720).inversePrimary));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceTint", this.surfaceTint, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceTint));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("background", this.background, defaultValue: ((ColorScheme)defaultScheme__66720).background));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("onBackground", this.onBackground, defaultValue: ((ColorScheme)defaultScheme__66720).onBackground));
        properties.add(new global::Doroti.Framework.Painting.ColorProperty("surfaceVariant", this.surfaceVariant, defaultValue: ((ColorScheme)defaultScheme__66720).surfaceVariant));
    }

    public static async Future<ColorScheme> fromImageProvider(dynamic provider, Brightness brightness = Brightness.light, DynamicSchemeVariant dynamicSchemeVariant = DynamicSchemeVariant.tonalSpot, double contrastLevel = 0.0, Color? primary = null, Color? onPrimary = null, Color? primaryContainer = null, Color? onPrimaryContainer = null, Color? primaryFixed = null, Color? primaryFixedDim = null, Color? onPrimaryFixed = null, Color? onPrimaryFixedVariant = null, Color? secondary = null, Color? onSecondary = null, Color? secondaryContainer = null, Color? onSecondaryContainer = null, Color? secondaryFixed = null, Color? secondaryFixedDim = null, Color? onSecondaryFixed = null, Color? onSecondaryFixedVariant = null, Color? tertiary = null, Color? onTertiary = null, Color? tertiaryContainer = null, Color? onTertiaryContainer = null, Color? tertiaryFixed = null, Color? tertiaryFixedDim = null, Color? onTertiaryFixed = null, Color? onTertiaryFixedVariant = null, Color? error = null, Color? onError = null, Color? errorContainer = null, Color? onErrorContainer = null, Color? outline = null, Color? outlineVariant = null, Color? surface = null, Color? onSurface = null, Color? surfaceDim = null, Color? surfaceBright = null, Color? surfaceContainerLowest = null, Color? surfaceContainerLow = null, Color? surfaceContainer = null, Color? surfaceContainerHigh = null, Color? surfaceContainerHighest = null, Color? onSurfaceVariant = null, Color? inverseSurface = null, Color? onInverseSurface = null, Color? inversePrimary = null, Color? shadow = null, Color? scrim = null, Color? surfaceTint = null, Color? background = null, Color? onBackground = null, Color? surfaceVariant = null)
    {
        QuantizerResult quantizerResult__77412 = await ColorScheme._extractColorsFromImageProvider(provider);
        DartMap<long, long> colorToCount__77503 = quantizerResult__77412.colorToCount.map<long, long, long, long>(((key, value) => new MapEntry<long, long>(ColorScheme._getArgbFromAbgr(key), value)));
        List<long> scoredResults__77710 = Score.score(colorToCount__77503, desired: 1L).ToList();
        var baseColor__77775 = new global::Doroti.Ui.Color(scoredResults__77710.First());
        DynamicScheme scheme__77840 = ((DynamicScheme)(object?)ColorScheme._buildDynamicScheme(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(brightness)), baseColor__77775, dynamicSchemeVariant, contrastLevel));
        return new ColorScheme(primary: (primary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primary.getArgb(scheme__77840))), onPrimary: (onPrimary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimary.getArgb(scheme__77840))), primaryContainer: (primaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryContainer.getArgb(scheme__77840))), onPrimaryContainer: (onPrimaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryContainer.getArgb(scheme__77840))), primaryFixed: (primaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryFixed.getArgb(scheme__77840))), primaryFixedDim: (primaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primaryFixedDim.getArgb(scheme__77840))), onPrimaryFixed: (onPrimaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryFixed.getArgb(scheme__77840))), onPrimaryFixedVariant: (onPrimaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onPrimaryFixedVariant.getArgb(scheme__77840))), secondary: (secondary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondary.getArgb(scheme__77840))), onSecondary: (onSecondary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondary.getArgb(scheme__77840))), secondaryContainer: (secondaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryContainer.getArgb(scheme__77840))), onSecondaryContainer: (onSecondaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryContainer.getArgb(scheme__77840))), secondaryFixed: (secondaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryFixed.getArgb(scheme__77840))), secondaryFixedDim: (secondaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.secondaryFixedDim.getArgb(scheme__77840))), onSecondaryFixed: (onSecondaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryFixed.getArgb(scheme__77840))), onSecondaryFixedVariant: (onSecondaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSecondaryFixedVariant.getArgb(scheme__77840))), tertiary: (tertiary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiary.getArgb(scheme__77840))), onTertiary: (onTertiary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiary.getArgb(scheme__77840))), tertiaryContainer: (tertiaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryContainer.getArgb(scheme__77840))), onTertiaryContainer: (onTertiaryContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryContainer.getArgb(scheme__77840))), tertiaryFixed: (tertiaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryFixed.getArgb(scheme__77840))), tertiaryFixedDim: (tertiaryFixedDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.tertiaryFixedDim.getArgb(scheme__77840))), onTertiaryFixed: (onTertiaryFixed ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryFixed.getArgb(scheme__77840))), onTertiaryFixedVariant: (onTertiaryFixedVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onTertiaryFixedVariant.getArgb(scheme__77840))), error: (error ?? new global::Doroti.Ui.Color(MaterialDynamicColors.error.getArgb(scheme__77840))), onError: (onError ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onError.getArgb(scheme__77840))), errorContainer: (errorContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.errorContainer.getArgb(scheme__77840))), onErrorContainer: (onErrorContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onErrorContainer.getArgb(scheme__77840))), outline: (outline ?? new global::Doroti.Ui.Color(MaterialDynamicColors.outline.getArgb(scheme__77840))), outlineVariant: (outlineVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.outlineVariant.getArgb(scheme__77840))), surface: (surface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surface.getArgb(scheme__77840))), surfaceDim: (surfaceDim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceDim.getArgb(scheme__77840))), surfaceBright: (surfaceBright ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceBright.getArgb(scheme__77840))), surfaceContainerLowest: (surfaceContainerLowest ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerLowest.getArgb(scheme__77840))), surfaceContainerLow: (surfaceContainerLow ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerLow.getArgb(scheme__77840))), surfaceContainer: (surfaceContainer ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainer.getArgb(scheme__77840))), surfaceContainerHigh: (surfaceContainerHigh ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerHigh.getArgb(scheme__77840))), surfaceContainerHighest: (surfaceContainerHighest ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceContainerHighest.getArgb(scheme__77840))), onSurface: (onSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSurface.getArgb(scheme__77840))), onSurfaceVariant: (onSurfaceVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onSurfaceVariant.getArgb(scheme__77840))), inverseSurface: (inverseSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inverseSurface.getArgb(scheme__77840))), onInverseSurface: (onInverseSurface ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inverseOnSurface.getArgb(scheme__77840))), inversePrimary: (inversePrimary ?? new global::Doroti.Ui.Color(MaterialDynamicColors.inversePrimary.getArgb(scheme__77840))), shadow: (shadow ?? new global::Doroti.Ui.Color(MaterialDynamicColors.shadow.getArgb(scheme__77840))), scrim: (scrim ?? new global::Doroti.Ui.Color(MaterialDynamicColors.scrim.getArgb(scheme__77840))), surfaceTint: (surfaceTint ?? new global::Doroti.Ui.Color(MaterialDynamicColors.primary.getArgb(scheme__77840))), brightness: DartRuntimePrimitives.RequireValue(brightness), background: (background ?? new global::Doroti.Ui.Color(MaterialDynamicColors.background.getArgb(scheme__77840))), onBackground: (onBackground ?? new global::Doroti.Ui.Color(MaterialDynamicColors.onBackground.getArgb(scheme__77840))), surfaceVariant: (surfaceVariant ?? new global::Doroti.Ui.Color(MaterialDynamicColors.surfaceVariant.getArgb(scheme__77840))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static async Future<QuantizerResult> _extractColorsFromImageProvider(dynamic imageProvider)
    {
        global::Doroti.Ui.Image scaledImage__83578 = ((global::Doroti.Ui.Image)(object?)await ColorScheme._imageProviderToScaled(imageProvider));
        ByteData? imageBytes__83657 = await scaledImage__83578.toByteData();
        QuantizerResult quantizerResult__83729 = await new QuantizerCelebi().quantize(imageBytes__83657!.buffer.asUint32List(), 128L, returnInputPixelToClusterPixel: true);
        return quantizerResult__83729;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static async Future<global::Doroti.Ui.Image> _imageProviderToScaled(dynamic imageProvider)
    {
        var maxDimension__84088 = 112.0;
        global::Doroti.Framework.Painting.ImageStream stream__84132 = ((global::Doroti.Framework.Painting.ImageStream)(object?)((global::Doroti.Framework.Painting.ImageStream)((dynamic)imageProvider).resolve(new global::Doroti.Framework.Painting.ImageConfiguration(size: new global::Doroti.Ui.Size(maxDimension__84088, maxDimension__84088)))));
        var imageCompleter__84253 = new Completer<global::Doroti.Ui.Image>();
        global::Doroti.Framework.Painting.ImageStreamListener listener__84322 = default!;
        global::Doroti.Ui.Image scaledImage__84350 = default!;
        Timer? loadFailureTimeout__84374 = default!;
        listener__84322 = new global::Doroti.Framework.Painting.ImageStreamListener(((global::System.Action<global::Doroti.Framework.Painting.ImageInfo, bool>)(async (info, sync) => {
loadFailureTimeout__84374?.cancel();
stream__84132.removeListener(listener__84322);
global::Doroti.Ui.Image image__84575 = ((global::Doroti.Ui.Image)(object?)((global::Doroti.Framework.Painting.ImageInfo)info).image);
long width__84613 = image__84575.width;
long height__84652 = image__84575.height;
double paintWidth__84690 = width__84613.toDouble();
double paintHeight__84736 = height__84652.toDouble();
DartRuntimePrimitives.Assert(() => ((width__84613 > 0L) && (height__84652 > 0L)));
bool rescale__84830 = ((width__84613 > maxDimension__84088) || (height__84652 > maxDimension__84088));
if (rescale__84830)
{
    paintWidth__84690 = (((width__84613 > height__84652)) ? maxDimension__84088 : (((maxDimension__84088 / height__84652)) * width__84613));
    paintHeight__84736 = (((height__84652 > width__84613)) ? maxDimension__84088 : (((maxDimension__84088 / width__84613)) * height__84652));
}
var pictureRecorder__85115 = new global::Doroti.Ui.PictureRecorder();
var canvas__85169 = new global::Doroti.Ui.Canvas(pictureRecorder__85115);
global::Doroti.Framework.Painting.Decoration_imageLibrary.paintImage(canvas: canvas__85169, rect: global::Doroti.Ui.Rect.fromLTRB(0, 0, paintWidth__84690, paintHeight__84736), image: image__84575, filterQuality: FilterQuality.none);
global::Doroti.Ui.Picture picture__85417 = ((global::Doroti.Ui.Picture)(object?)pictureRecorder__85115.endRecording());
scaledImage__84350 = await picture__85417.toImage(paintWidth__84690.toInt(), paintHeight__84736.toInt());
imageCompleter__84253.complete(((global::Doroti.Framework.Painting.ImageInfo)info).image);
})), onError: ((global::System.Action<object, global::System.Diagnostics.StackTrace?>)((exception, stackTrace) => {
loadFailureTimeout__84374?.cancel();
stream__84132.removeListener(listener__84322);
imageCompleter__84253.completeError(new Exception($"Failed to render image: {exception}"), stackTrace);
})));
        loadFailureTimeout__84374 = new Timer(Duration.Create(seconds: 5L), (() => {
stream__84132.removeListener(listener__84322);
imageCompleter__84253.completeError(new TimeoutException("Timeout occurred trying to load image"));
}));
        stream__84132.addListener(listener__84322);
        await imageCompleter__84253.future;
        return ((global::Doroti.Ui.Image)(object?)scaledImage__84350);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static long _getArgbFromAbgr(long abgr)
    {
        var exceptRMask__86262 = 4278255615L;
        long onlyRMask__86302 = ~exceptRMask__86262;
        var exceptBMask__86338 = 4294967040L;
        long onlyBMask__86378 = ~exceptBMask__86338;
        long r__86418 = (((abgr & onlyRMask__86302)) >> (int)(16L));
        long b__86462 = (abgr & onlyBMask__86378);
        return (((((abgr & exceptRMask__86262) & exceptBMask__86338)) | ((b__86462 << (int)(16L)))) | r__86418);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static DynamicScheme _buildDynamicScheme(Brightness brightness, Color seedColor, DynamicSchemeVariant schemeVariant, double contrastLevel)
    {
        DartRuntimePrimitives.Assert(() => ((contrastLevel >= -1.0) && (contrastLevel <= 1.0)), () => (object?)"contrastLevel must be between -1.0 and 1.0 inclusive.");
        var isDark__86861 = (object.Equals(DartRuntimePrimitives.RequireValue(brightness), Brightness.dark));
        Hct sourceColor__86915 = Hct.fromInt(seedColor.value);
        return (schemeVariant switch { DynamicSchemeVariant.tonalSpot => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeTonalSpot(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), DynamicSchemeVariant.fidelity => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeFidelity(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), DynamicSchemeVariant.content => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeContent(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), DynamicSchemeVariant.monochrome => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeMonochrome(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), DynamicSchemeVariant.neutral => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeNeutral(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), DynamicSchemeVariant.vibrant => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeVibrant(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), DynamicSchemeVariant.expressive => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeExpressive(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), DynamicSchemeVariant.rainbow => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeRainbow(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), DynamicSchemeVariant.fruitSalad => DartRuntimePrimitives.ConvertValue<DynamicScheme>(new SchemeFruitSalad(sourceColorHct: sourceColor__86915, isDark: isDark__86861, contrastLevel: contrastLevel)), _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static ColorScheme of(global::Doroti.Framework.Widgets.BuildContext context) => Theme.of(context).colorScheme;
    public virtual string toStringShort() => global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
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
