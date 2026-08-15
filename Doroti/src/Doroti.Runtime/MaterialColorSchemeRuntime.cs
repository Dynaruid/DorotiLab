using System.Collections.Concurrent;
using System.Diagnostics;
using MaterialColorUtilities.ColorAppearance;
using MaterialColorUtilities.Utils;

namespace Doroti.Runtime;

/// <summary>
/// Managed Material Color Utilities bridge used by generated Flutter Material code.
/// Palette definitions and role tone curves follow material_color_utilities 0.13.0,
/// the version pinned by the Flutter source snapshot.
/// </summary>
public static class MaterialColorSchemeRuntime
{
    private static readonly ConcurrentDictionary<SchemeKey, Scheme> SchemeCache = new();
    private static readonly ConcurrentDictionary<ColorKey, long> ColorCache = new();

    public static long GetArgb(long seedArgb, bool isDark, string variant, double contrastLevel, string role)
    {
        if (contrastLevel is < -1 or > 1 || !double.IsFinite(contrastLevel))
        {
            throw new ArgumentOutOfRangeException(nameof(contrastLevel), "Material contrast must be between -1 and 1.");
        }

        var schemeKey = new SchemeKey(unchecked((uint)seedArgb), isDark, variant, contrastLevel);
        var colorKey = new ColorKey(schemeKey, role);
        return ColorCache.GetOrAdd(colorKey, static key =>
        {
            var scheme = SchemeCache.GetOrAdd(key.Scheme, static item =>
                BuildScheme(Hct.FromInt(item.SeedArgb), item.IsDark, item.Variant, item.ContrastLevel));
            var tone = ResolveTone(scheme, key.Role);
            var palette = PaletteForRole(scheme, key.Role);
            return unchecked((long)Hct.From(palette.Hue, palette.Chroma, tone).ToInt());
        });
    }

    private static Scheme BuildScheme(Hct source, bool dark, string variant, double contrast)
    {
        var hue = source.Hue;
        var chroma = source.Chroma;
        var normalized = variant switch
        {
            "tonalSpot" or "fidelity" or "content" or "monochrome" or "neutral" or
            "vibrant" or "expressive" or "rainbow" or "fruitSalad" => variant,
            _ => throw new NotSupportedException($"Unsupported Material dynamic scheme variant '{variant}'."),
        };

        var palettes = normalized switch
        {
            "tonalSpot" => new Palettes(P(hue, 36), P(hue, 16), P(hue + 60, 24), P(hue, 6), P(hue, 8)),
            "monochrome" => new Palettes(P(hue, 0), P(hue, 0), P(hue, 0), P(hue, 0), P(hue, 0)),
            "neutral" => new Palettes(P(hue, 12), P(hue, 8), P(hue, 16), P(hue, 2), P(hue, 2)),
            "vibrant" => new Palettes(P(hue, 200), P(RotatedHue(hue, VibrantHues, VibrantSecondaryRotations), 24), P(RotatedHue(hue, VibrantHues, VibrantTertiaryRotations), 32), P(hue, 10), P(hue, 12)),
            "expressive" => new Palettes(P(hue + 240, 40), P(RotatedHue(hue, ExpressiveHues, ExpressiveSecondaryRotations), 24), P(RotatedHue(hue, ExpressiveHues, ExpressiveTertiaryRotations), 32), P(hue + 15, 8), P(hue + 15, 12)),
            "rainbow" => new Palettes(P(hue, 48), P(hue, 16), P(hue + 60, 24), P(hue, 0), P(hue, 0)),
            "fruitSalad" => new Palettes(P(hue - 50, 48), P(hue - 50, 36), P(hue, 36), P(hue, 10), P(hue, 16)),
            "content" => new Palettes(P(hue, chroma), P(hue, Math.Max(chroma - 32, chroma * 0.5)), PaletteFromHct(FixIfDisliked(Analogous(source))), P(hue, chroma / 8), P(hue, (chroma / 8) + 4)),
            "fidelity" => new Palettes(P(hue, chroma), P(hue, Math.Max(chroma - 32, chroma * 0.5)), PaletteFromHct(FixIfDisliked(Complement(source))), P(hue, chroma / 8), P(hue, (chroma / 8) + 4)),
            _ => throw new UnreachableException(),
        };
        return new(source, normalized, dark, contrast, palettes);
    }

    private static double ResolveTone(Scheme scheme, string role)
    {
        if (role is "shadow" or "scrim") return 0;
        if (role is "surfaceTint") return scheme.IsDark ? 80 : 40;

        if (role is "primary" or "primaryContainer") return ResolvePair(scheme, role, "primaryContainer", "primary");
        if (role is "secondary" or "secondaryContainer") return ResolvePair(scheme, role, "secondaryContainer", "secondary");
        if (role is "tertiary" or "tertiaryContainer") return ResolvePair(scheme, role, "tertiaryContainer", "tertiary");

        var initial = InitialTone(scheme, role);
        var backgroundRole = BackgroundRole(role, scheme.IsDark);
        if (backgroundRole is null) return initial;

        var backgroundTone = ResolveTone(scheme, backgroundRole);
        var ratio = ContrastFor(role, scheme.Contrast);
        var answer = RatioOfTones(backgroundTone, initial) >= ratio && scheme.Contrast >= 0
            ? initial
            : ForegroundTone(backgroundTone, ratio);
        if (IsBackgroundRole(role) && answer is >= 50 and < 60)
        {
            answer = RatioOfTones(49, backgroundTone) >= ratio ? 49 : 60;
        }
        return answer;
    }

    private static double ResolvePair(Scheme scheme, string requestedRole, string nearerRole, string fartherRole)
    {
        var backgroundTone = InitialTone(scheme, scheme.IsDark ? "surfaceBright" : "surfaceDim");
        var nearerInitial = InitialTone(scheme, nearerRole);
        var fartherInitial = InitialTone(scheme, fartherRole);
        var nearerRatio = ContrastFor(nearerRole, scheme.Contrast);
        var fartherRatio = ContrastFor(fartherRole, scheme.Contrast);
        var decreasing = scheme.Contrast < 0;
        var nearer = !decreasing && RatioOfTones(backgroundTone, nearerInitial) >= nearerRatio
            ? nearerInitial : ForegroundTone(backgroundTone, nearerRatio);
        var farther = !decreasing && RatioOfTones(backgroundTone, fartherInitial) >= fartherRatio
            ? fartherInitial : ForegroundTone(backgroundTone, fartherRatio);
        var direction = scheme.IsDark ? 1d : -1d;
        if ((farther - nearer) * direction < 10)
        {
            farther = Math.Clamp(nearer + (10 * direction), 0, 100);
            if ((farther - nearer) * direction < 10)
            {
                nearer = Math.Clamp(farther - (10 * direction), 0, 100);
            }
        }
        if (nearer is >= 50 and < 60)
        {
            nearer = direction > 0 ? 60 : 49;
            farther = direction > 0 ? Math.Max(farther, nearer + 10) : Math.Min(farther, nearer - 10);
        }
        else if (farther is >= 50 and < 60)
        {
            farther = direction > 0 ? 60 : 49;
        }
        return requestedRole == nearerRole ? nearer : farther;
    }

    private static double InitialTone(Scheme scheme, string role)
    {
        var dark = scheme.IsDark;
        var monochrome = scheme.Variant == "monochrome";
        return role switch
        {
            "background" => dark ? 6 : 98,
            "onBackground" => dark ? 90 : 10,
            "surface" => dark ? 6 : 98,
            "surfaceDim" => dark ? 6 : Curve(scheme.Contrast, 87, 87, 80, 75),
            "surfaceBright" => dark ? Curve(scheme.Contrast, 24, 24, 29, 34) : 98,
            "surfaceContainerLowest" => dark ? Curve(scheme.Contrast, 4, 4, 2, 0) : 100,
            "surfaceContainerLow" => dark ? Curve(scheme.Contrast, 10, 10, 11, 12) : Curve(scheme.Contrast, 96, 96, 96, 95),
            "surfaceContainer" => dark ? Curve(scheme.Contrast, 12, 12, 16, 20) : Curve(scheme.Contrast, 94, 94, 92, 90),
            "surfaceContainerHigh" => dark ? Curve(scheme.Contrast, 17, 17, 21, 25) : Curve(scheme.Contrast, 92, 92, 88, 85),
            "surfaceContainerHighest" => dark ? Curve(scheme.Contrast, 22, 22, 26, 30) : Curve(scheme.Contrast, 90, 90, 84, 80),
            "onSurface" => dark ? 90 : 10,
            "surfaceVariant" => dark ? 30 : 90,
            "onSurfaceVariant" => dark ? 80 : 30,
            "inverseSurface" => dark ? 90 : 20,
            "inverseOnSurface" => dark ? 20 : 95,
            "outline" => dark ? 60 : 50,
            "outlineVariant" => dark ? 30 : 80,
            "primary" => monochrome ? (dark ? 100 : 0) : (dark ? 80 : 40),
            "onPrimary" => monochrome ? (dark ? 10 : 90) : (dark ? 20 : 100),
            "primaryContainer" => monochrome ? (dark ? 85 : 25) : (scheme.Variant is "fidelity" or "content" ? scheme.Source.Tone : dark ? 30 : 90),
            "onPrimaryContainer" => monochrome ? (dark ? 0 : 100) : IsFidelity(scheme)
                ? ForegroundTone(InitialTone(scheme, "primaryContainer"), 4.5)
                : (dark ? 90 : 30),
            "inversePrimary" => dark ? 40 : 80,
            "secondary" => dark ? 80 : 40,
            "onSecondary" => monochrome ? (dark ? 10 : 100) : (dark ? 20 : 100),
            "secondaryContainer" => monochrome ? (dark ? 30 : 85) : IsFidelity(scheme)
                ? FindDesiredChromaByTone(scheme.Palettes.Secondary, dark ? 30 : 90, !dark)
                : (dark ? 30 : 90),
            "onSecondaryContainer" => monochrome ? (dark ? 90 : 10) : IsFidelity(scheme)
                ? ForegroundTone(InitialTone(scheme, "secondaryContainer"), 4.5)
                : (dark ? 90 : 30),
            "tertiary" => monochrome ? (dark ? 90 : 25) : (dark ? 80 : 40),
            "onTertiary" => monochrome ? (dark ? 10 : 90) : (dark ? 20 : 100),
            "tertiaryContainer" => monochrome ? (dark ? 60 : 49) : IsFidelity(scheme)
                ? FixIfDisliked(Hct.From(scheme.Palettes.Tertiary.Hue, scheme.Palettes.Tertiary.Chroma, scheme.Source.Tone)).Tone
                : (dark ? 30 : 90),
            "onTertiaryContainer" => monochrome ? (dark ? 0 : 100) : IsFidelity(scheme)
                ? ForegroundTone(InitialTone(scheme, "tertiaryContainer"), 4.5)
                : (dark ? 90 : 30),
            "error" => dark ? 80 : 40,
            "onError" => dark ? 20 : 100,
            "errorContainer" => dark ? 30 : 90,
            "onErrorContainer" => dark ? 90 : 30,
            "primaryFixed" or "secondaryFixed" or "tertiaryFixed" => 90,
            "primaryFixedDim" or "secondaryFixedDim" or "tertiaryFixedDim" => 80,
            "onPrimaryFixed" or "onSecondaryFixed" or "onTertiaryFixed" => 10,
            "onPrimaryFixedVariant" or "onSecondaryFixedVariant" or "onTertiaryFixedVariant" => 30,
            _ => throw new NotSupportedException($"Unsupported Material color role '{role}'."),
        };
    }

    private static Palette PaletteForRole(Scheme scheme, string role) => role switch
    {
        "primary" or "onPrimary" or "primaryContainer" or "onPrimaryContainer" or "inversePrimary" or
        "primaryFixed" or "primaryFixedDim" or "onPrimaryFixed" or "onPrimaryFixedVariant" or "surfaceTint" => scheme.Palettes.Primary,
        "secondary" or "onSecondary" or "secondaryContainer" or "onSecondaryContainer" or
        "secondaryFixed" or "secondaryFixedDim" or "onSecondaryFixed" or "onSecondaryFixedVariant" => scheme.Palettes.Secondary,
        "tertiary" or "onTertiary" or "tertiaryContainer" or "onTertiaryContainer" or
        "tertiaryFixed" or "tertiaryFixedDim" or "onTertiaryFixed" or "onTertiaryFixedVariant" => scheme.Palettes.Tertiary,
        "error" or "onError" or "errorContainer" or "onErrorContainer" => new(25, 84),
        "surfaceVariant" or "onSurfaceVariant" or "outline" or "outlineVariant" => scheme.Palettes.NeutralVariant,
        _ => scheme.Palettes.Neutral,
    };

    private static string? BackgroundRole(string role, bool dark) => role switch
    {
        "onBackground" => "background",
        "onSurface" or "onSurfaceVariant" or "outline" or "outlineVariant" or
        "primary" or "primaryContainer" or "secondary" or "secondaryContainer" or
        "tertiary" or "tertiaryContainer" or "error" or "errorContainer" => dark ? "surfaceBright" : "surfaceDim",
        "onPrimary" => "primary",
        "onPrimaryContainer" => "primaryContainer",
        "onSecondary" => "secondary",
        "onSecondaryContainer" => "secondaryContainer",
        "onTertiary" => "tertiary",
        "onTertiaryContainer" => "tertiaryContainer",
        "onError" => "error",
        "onErrorContainer" => "errorContainer",
        "inverseOnSurface" or "inversePrimary" => "inverseSurface",
        _ => null,
    };

    private static bool IsBackgroundRole(string role) => role is "primary" or "primaryContainer" or "secondary" or
        "secondaryContainer" or "tertiary" or "tertiaryContainer" or "error" or "errorContainer";

    private static double ContrastFor(string role, double contrast) => role switch
    {
        "primary" or "secondary" or "tertiary" or "error" or "inversePrimary" => Curve(contrast, 3, 4.5, 7, 7),
        "primaryContainer" or "secondaryContainer" or "tertiaryContainer" or "errorContainer" or "outlineVariant" => Curve(contrast, 1, 1, 3, 4.5),
        "onPrimary" or "onSecondary" or "onTertiary" or "onError" or "onSurface" or "inverseOnSurface" => Curve(contrast, 4.5, 7, 11, 21),
        "onPrimaryContainer" or "onSecondaryContainer" or "onTertiaryContainer" or "onErrorContainer" or "onSurfaceVariant" => Curve(contrast, 3, 4.5, 7, 11),
        "outline" => Curve(contrast, 1.5, 3, 4.5, 7),
        "onBackground" => Curve(contrast, 3, 3, 4.5, 7),
        _ => 1,
    };

    private static double Curve(double contrast, double low, double normal, double medium, double high) => contrast switch
    {
        <= -1 => low,
        < 0 => Lerp(low, normal, contrast + 1),
        < 0.5 => Lerp(normal, medium, contrast / 0.5),
        < 1 => Lerp(medium, high, (contrast - 0.5) / 0.5),
        _ => high,
    };

    private static double ForegroundTone(double backgroundTone, double ratio)
    {
        var lighter = LighterUnsafe(backgroundTone, ratio);
        var darker = DarkerUnsafe(backgroundTone, ratio);
        var lighterRatio = RatioOfTones(lighter, backgroundTone);
        var darkerRatio = RatioOfTones(darker, backgroundTone);
        var preferLighter = Math.Round(backgroundTone) < 60;
        if (preferLighter)
        {
            var negligible = Math.Abs(lighterRatio - darkerRatio) < 0.1 && lighterRatio < ratio && darkerRatio < ratio;
            return lighterRatio >= ratio || lighterRatio >= darkerRatio || negligible ? lighter : darker;
        }
        return darkerRatio >= ratio || darkerRatio >= lighterRatio ? darker : lighter;
    }

    private static double RatioOfTones(double first, double second)
    {
        var y1 = YFromLstar(Math.Clamp(first, 0, 100));
        var y2 = YFromLstar(Math.Clamp(second, 0, 100));
        return (Math.Max(y1, y2) + 5) / (Math.Min(y1, y2) + 5);
    }

    private static double LighterUnsafe(double tone, double ratio)
    {
        var lightY = ratio * (YFromLstar(tone) + 5) - 5;
        var result = LstarFromY(lightY) + 0.4;
        return result is < 0 or > 100 ? 100 : result;
    }

    private static double DarkerUnsafe(double tone, double ratio)
    {
        var darkY = ((YFromLstar(tone) + 5) / ratio) - 5;
        var result = LstarFromY(darkY) - 0.4;
        return result is < 0 or > 100 ? 0 : result;
    }

    private static double YFromLstar(double lstar) => lstar > 8 ? 100 * Math.Pow((lstar + 16) / 116, 3) : 100 * lstar / 903.2962962962963;
    private static double LstarFromY(double y)
    {
        var normalized = y / 100;
        var f = normalized > 216d / 24389d ? Math.Pow(normalized, 1d / 3d) : ((24389d / 27d) * normalized + 16) / 116;
        return 116 * f - 16;
    }

    private static Palette P(double hue, double chroma) => new(Sanitize(hue), chroma);
    private static double Sanitize(double degrees) => ((degrees % 360) + 360) % 360;
    private static double Lerp(double start, double stop, double amount) => start + ((stop - start) * amount);

    private static double RotatedHue(double sourceHue, double[] hues, double[] rotations)
    {
        for (var index = 0; index < hues.Length - 1; index++)
        {
            if (sourceHue >= hues[index] && sourceHue < hues[index + 1]) return Sanitize(sourceHue + rotations[index]);
        }
        return sourceHue;
    }

    private static bool IsFidelity(Scheme scheme) => scheme.Variant is "fidelity" or "content";

    private static double FindDesiredChromaByTone(Palette palette, double tone, bool decreasing)
    {
        var answer = tone;
        var closest = Hct.From(palette.Hue, palette.Chroma, tone);
        if (closest.Chroma >= palette.Chroma) return answer;
        var peak = closest.Chroma;
        while (closest.Chroma < palette.Chroma)
        {
            answer += decreasing ? -1 : 1;
            var candidate = Hct.From(palette.Hue, palette.Chroma, answer);
            if (peak > candidate.Chroma) break;
            if (Math.Abs(candidate.Chroma - palette.Chroma) < 0.4) break;
            if (Math.Abs(candidate.Chroma - palette.Chroma) < Math.Abs(closest.Chroma - palette.Chroma)) closest = candidate;
            peak = Math.Max(peak, candidate.Chroma);
        }
        return answer;
    }

    private static Palette PaletteFromHct(Hct hct) => P(hct.Hue, hct.Chroma);

    private static Hct FixIfDisliked(Hct hct) =>
        Math.Round(hct.Hue) is >= 90 and <= 111 && Math.Round(hct.Chroma) > 16 && Math.Round(hct.Tone) < 65
            ? Hct.From(hct.Hue, hct.Chroma, 70)
            : hct;

    private static Hct Analogous(Hct input)
    {
        var hues = HuesAtSourceTone(input);
        var temperatures = hues.Select(RawTemperature).ToArray();
        var startHue = checked((int)Math.Round(input.Hue)) % 360;
        var lastTemperature = RelativeTemperature(temperatures[startHue], temperatures);
        var colors = new List<Hct> { hues[startHue] };
        var absoluteDelta = 0d;
        for (var index = 0; index < 360; index++)
        {
            var hue = (startHue + index) % 360;
            var temperature = RelativeTemperature(temperatures[hue], temperatures);
            absoluteDelta += Math.Abs(temperature - lastTemperature);
            lastTemperature = temperature;
        }
        var hueAddend = 1;
        var step = absoluteDelta / 6;
        var totalDelta = 0d;
        lastTemperature = RelativeTemperature(temperatures[startHue], temperatures);
        while (colors.Count < 6)
        {
            var hue = (startHue + hueAddend) % 360;
            var temperature = RelativeTemperature(temperatures[hue], temperatures);
            totalDelta += Math.Abs(temperature - lastTemperature);
            var satisfied = totalDelta >= colors.Count * step;
            var indexAddend = 1;
            while (satisfied && colors.Count < 6)
            {
                colors.Add(hues[hue]);
                satisfied = totalDelta >= (colors.Count + indexAddend) * step;
                indexAddend++;
            }
            lastTemperature = temperature;
            hueAddend++;
            if (hueAddend <= 360) continue;
            while (colors.Count < 6) colors.Add(hues[hue]);
        }
        return colors[1];
    }

    private static Hct Complement(Hct input)
    {
        var hues = HuesAtSourceTone(input);
        var temperatures = hues.Select(RawTemperature).ToArray();
        var coldestIndex = Array.IndexOf(temperatures, temperatures.Min());
        var warmestIndex = Array.IndexOf(temperatures, temperatures.Max());
        var coldestHue = hues[coldestIndex].Hue;
        var warmestHue = hues[warmestIndex].Hue;
        var startInputBetween = IsBetween(input.Hue, coldestHue, warmestHue);
        var start = startInputBetween ? warmestHue : coldestHue;
        var end = startInputBetween ? coldestHue : warmestHue;
        var desired = 1 - RelativeTemperature(RawTemperature(input), temperatures);
        var bestError = double.PositiveInfinity;
        var answer = hues[checked((int)Math.Round(input.Hue)) % 360];
        for (var addend = 0; addend <= 360; addend++)
        {
            var hue = Sanitize(start + addend);
            if (!IsBetween(hue, start, end)) continue;
            var candidate = hues[checked((int)Math.Round(hue)) % 360];
            var error = Math.Abs(desired - RelativeTemperature(RawTemperature(candidate), temperatures));
            if (error >= bestError) continue;
            bestError = error;
            answer = candidate;
        }
        return answer;
    }

    private static Hct[] HuesAtSourceTone(Hct input) => Enumerable.Range(0, 360)
        .Select(hue => Hct.From(hue, input.Chroma, input.Tone)).ToArray();

    private static double RelativeTemperature(double temperature, IReadOnlyList<double> temperatures)
    {
        var coldest = temperatures.Min();
        var range = temperatures.Max() - coldest;
        return range == 0 ? 0.5 : (temperature - coldest) / range;
    }

    private static double RawTemperature(Hct color)
    {
        var lab = ColorUtils.LabFromArgb(color.ToInt());
        var hue = Sanitize(Math.Atan2(lab[2], lab[1]) * 180 / Math.PI);
        var chroma = Math.Sqrt((lab[1] * lab[1]) + (lab[2] * lab[2]));
        return -0.5 + (0.02 * Math.Pow(chroma, 1.07) * Math.Cos(Sanitize(hue - 50) * Math.PI / 180));
    }

    private static bool IsBetween(double angle, double start, double end) =>
        start < end ? angle >= start && angle <= end : angle >= start || angle <= end;

    private static readonly double[] VibrantHues = [0, 41, 61, 101, 131, 181, 251, 301, 360];
    private static readonly double[] VibrantSecondaryRotations = [18, 15, 10, 12, 15, 18, 15, 12, 12];
    private static readonly double[] VibrantTertiaryRotations = [35, 30, 20, 25, 30, 35, 30, 25, 25];
    private static readonly double[] ExpressiveHues = [0, 21, 51, 121, 151, 191, 271, 321, 360];
    private static readonly double[] ExpressiveSecondaryRotations = [45, 95, 45, 20, 45, 90, 45, 45, 45];
    private static readonly double[] ExpressiveTertiaryRotations = [120, 120, 20, 45, 20, 15, 20, 120, 120];

    private sealed record Scheme(Hct Source, string Variant, bool IsDark, double Contrast, Palettes Palettes);
    private sealed record Palettes(Palette Primary, Palette Secondary, Palette Tertiary, Palette Neutral, Palette NeutralVariant);
    private readonly record struct SchemeKey(uint SeedArgb, bool IsDark, string Variant, double ContrastLevel);
    private readonly record struct ColorKey(SchemeKey Scheme, string Role);
    private readonly record struct Palette(double Hue, double Chroma);
}
