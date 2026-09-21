// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/colors.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public static partial class ColorsLibrary
{
    internal static double _getHue(double red, double green, double blue, double max, double delta)
    {
        double hue = default!;
        if (max == 0.0)
        {
            hue = 0.0;
        }
        else
        {
            if (max == red)
            {
                hue = 60.0 * ((green - blue) / delta % 6L);
            }
            else
            {
                if (max == green)
                {
                    hue = 60.0 * (((blue - red) / delta) + 2L);
                }
                else
                {
                    if (max == blue)
                    {
                        hue = 60.0 * (((red - green) / delta) + 4L);
                    }
                }
            }
        }
        hue = double.IsNaN(hue) ? 0.0 : hue;
        return hue;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public static partial class ColorsLibrary
{
    internal static Color _colorFromHue(
        double alpha,
        double hue,
        double chroma,
        double secondary,
        double match
    )
    {
        var (red, green, blue) = hue switch
        {
            < 60.0 => (chroma, secondary, 0.0),
            < 120.0 => (secondary, chroma, 0.0),
            < 180.0 => (0.0, chroma, secondary),
            < 240.0 => (0.0, secondary, chroma),
            < 300.0 => (secondary, 0.0, chroma),
            _ => (chroma, 0.0, secondary),
        };
        return Color.fromARGB(
            (alpha * 255L).round(),
            ((red + match) * 255L).round(),
            ((green + match) * 255L).round(),
            ((blue + match) * 255L).round()
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class HSVColor
{
    public virtual double alpha { get; private set; } = default!;
    public virtual double hue { get; private set; } = default!;
    public virtual double saturation { get; private set; } = default!;
    public virtual double value { get; private set; } = default!;

    public HSVColor(double alpha, double hue, double saturation, double value)
    {
        this.alpha = alpha;
        this.hue = hue;
        this.saturation = saturation;
        this.value = value;
        System.Diagnostics.Debug.Assert(alpha >= 0.0);
        System.Diagnostics.Debug.Assert(alpha <= 1.0);
        System.Diagnostics.Debug.Assert(hue >= 0.0);
        System.Diagnostics.Debug.Assert(hue <= 360.0);
        System.Diagnostics.Debug.Assert(saturation >= 0.0);
        System.Diagnostics.Debug.Assert(saturation <= 1.0);
        System.Diagnostics.Debug.Assert(value >= 0.0);
        System.Diagnostics.Debug.Assert(value <= 1.0);
    }

    public static HSVColor CreateFromColor(Color color)
    {
        double redLocal = color.red / 255L;
        double greenLocal = color.green / 255L;
        double blueLocal = color.blue / 255L;
        double max = Math.Max(redLocal, Math.Max(greenLocal, blueLocal));
        double min = Math.Min(redLocal, Math.Min(greenLocal, blueLocal));
        double delta = max - min;
        double alphaLocal = color.alpha / 255L;
        double hue = ColorsLibrary._getHue(redLocal, greenLocal, blueLocal, max, delta);
        double saturation = (max == 0.0) ? 0.0 : (delta / max);
        return new HSVColor(alphaLocal, hue, saturation, max);
    }

    public virtual HSVColor withAlpha(double alpha)
    {
        return new HSVColor(alpha, hue, saturation, value);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual HSVColor withHue(double hue)
    {
        return new HSVColor(alpha, hue, saturation, value);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual HSVColor withSaturation(double saturation)
    {
        return new HSVColor(alpha, hue, saturation, value);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual HSVColor withValue(double value)
    {
        return new HSVColor(alpha, hue, saturation, value);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Color toColor()
    {
        double chroma = saturation * value;
        double secondary = chroma * (1.0 - ((hue / 60.0 % 2.0) - 1.0).abs());
        double match = value - chroma;
        return ColorsLibrary._colorFromHue(alpha, hue, chroma, secondary, match);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual HSVColor _scaleAlpha(double factor)
    {
        return withAlpha(alpha * factor);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static HSVColor? lerp(HSVColor? a, HSVColor? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!._scaleAlpha(t);
        }
        if (b is null)
        {
            return a._scaleAlpha(1.0 - t);
        }
        return new HSVColor(
            Dart_uiLibrary.clampDouble(
                (
                    Dart_uiLibrary.lerpDouble(a.alpha, b.alpha, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                0.0,
                1.0
            ),
            (
                Dart_uiLibrary.lerpDouble(a.hue, b.hue, t)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) % 360.0,
            Dart_uiLibrary.clampDouble(
                (
                    Dart_uiLibrary.lerpDouble(a.saturation, b.saturation, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                0.0,
                1.0
            ),
            Dart_uiLibrary.clampDouble(
                (
                    Dart_uiLibrary.lerpDouble(a.value, b.value, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                0.0,
                1.0
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as HSVColor;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is HSVColor)
            && (__other.alpha == alpha)
            && (__other.hue == hue)
            && (__other.saturation == saturation)
            && (__other.value == value);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(alpha, hue, saturation, value);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "HSVColor")}({alpha}, {hue}, {saturation}, {value})";
}

public class HSLColor
{
    public virtual double alpha { get; private set; } = default!;
    public virtual double hue { get; private set; } = default!;
    public virtual double saturation { get; private set; } = default!;
    public virtual double lightness { get; private set; } = default!;

    public HSLColor(double alpha, double hue, double saturation, double lightness)
    {
        this.alpha = alpha;
        this.hue = hue;
        this.saturation = saturation;
        this.lightness = lightness;
        System.Diagnostics.Debug.Assert(alpha >= 0.0);
        System.Diagnostics.Debug.Assert(alpha <= 1.0);
        System.Diagnostics.Debug.Assert(hue >= 0.0);
        System.Diagnostics.Debug.Assert(hue <= 360.0);
        System.Diagnostics.Debug.Assert(saturation >= 0.0);
        System.Diagnostics.Debug.Assert(saturation <= 1.0);
        System.Diagnostics.Debug.Assert(lightness >= 0.0);
        System.Diagnostics.Debug.Assert(lightness <= 1.0);
    }

    public static HSLColor CreateFromColor(Color color)
    {
        double redLocal = color.red / 255L;
        double greenLocal = color.green / 255L;
        double blueLocal = color.blue / 255L;
        double max = Math.Max(redLocal, Math.Max(greenLocal, blueLocal));
        double min = Math.Min(redLocal, Math.Min(greenLocal, blueLocal));
        double delta = max - min;
        double alphaLocal = color.alpha / 255L;
        double hue = ColorsLibrary._getHue(redLocal, greenLocal, blueLocal, max, delta);
        double lightness = (max + min) / 2.0;
        double saturation =
            (min == max)
                ? 0.0
                : Dart_uiLibrary.clampDouble(
                    delta / (1.0 - ((2.0 * lightness) - 1.0).abs()),
                    0.0,
                    1.0
                );
        return new HSLColor(alphaLocal, hue, saturation, lightness);
    }

    public virtual HSLColor withAlpha(double alpha)
    {
        return new HSLColor(alpha, hue, saturation, lightness);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual HSLColor withHue(double hue)
    {
        return new HSLColor(alpha, hue, saturation, lightness);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual HSLColor withSaturation(double saturation)
    {
        return new HSLColor(alpha, hue, saturation, lightness);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual HSLColor withLightness(double lightness)
    {
        return new HSLColor(alpha, hue, saturation, lightness);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual Color toColor()
    {
        double chroma = (1.0 - ((2.0 * lightness) - 1.0).abs()) * saturation;
        double secondary = chroma * (1.0 - ((hue / 60.0 % 2.0) - 1.0).abs());
        double match = lightness - (chroma / 2.0);
        return ColorsLibrary._colorFromHue(alpha, hue, chroma, secondary, match);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    internal virtual HSLColor _scaleAlpha(double factor)
    {
        return withAlpha(alpha * factor);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public static HSLColor? lerp(HSLColor? a, HSLColor? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if (a is null)
        {
            return b!._scaleAlpha(t);
        }
        if (b is null)
        {
            return a._scaleAlpha(1.0 - t);
        }
        return new HSLColor(
            Dart_uiLibrary.clampDouble(
                (
                    Dart_uiLibrary.lerpDouble(a.alpha, b.alpha, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                0.0,
                1.0
            ),
            (
                Dart_uiLibrary.lerpDouble(a.hue, b.hue, t)
                ?? throw new global::System.NullReferenceException("A required value was null.")
            ) % 360.0,
            Dart_uiLibrary.clampDouble(
                (
                    Dart_uiLibrary.lerpDouble(a.saturation, b.saturation, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                0.0,
                1.0
            ),
            Dart_uiLibrary.clampDouble(
                (
                    Dart_uiLibrary.lerpDouble(a.lightness, b.lightness, t)
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                0.0,
                1.0
            )
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as HSLColor;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is HSLColor)
            && (__other.alpha == alpha)
            && (__other.hue == hue)
            && (__other.saturation == saturation)
            && (__other.lightness == lightness);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(alpha, hue, saturation, lightness);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "HSLColor")}({alpha}, {hue}, {saturation}, {lightness})";
}

public class ColorSwatch<T> : Color
    where T : notnull
{
    internal virtual DartMap<T, Color> _swatch { get; private set; } = default!;

    public ColorSwatch(long primary, DartMap<T, Color> _swatch)
        : base(primary)
    {
        this._swatch = _swatch;
    }

    public Color? this[T key]
    {
        get { return _swatch.GetValueOrDefault(key); }
    }

    public virtual IEnumerable<T> keys => _swatch.Keys;

    public override bool Equals(object? other)
    {
        var __other = other as ColorSwatch<T>;
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
        return Equals(__other)
            && (__other is ColorSwatch<T>)
            && CollectionsLibrary.mapEquals(__other._swatch, _swatch);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(GetType(), value, _swatch);

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "ColorSwatch")}(primary value: {base.ToString()})";

    public static ColorSwatch<TKey>? lerp<TKey>(
        ColorSwatch<TKey>? a,
        ColorSwatch<TKey>? b,
        double t
    )
        where TKey : notnull
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        DartMap<TKey, Color> swatch = default!;
        if (b is null)
        {
            swatch = a!._swatch.map(
                (key, color) =>
                    new MapEntry<TKey, Color>(key, Dart_uiLibrary.Color.lerp(color, null, t)!)
            );
        }
        else
        {
            if (a is null)
            {
                swatch = b._swatch.map(
                    (key, color) =>
                        new MapEntry<TKey, Color>(key, Dart_uiLibrary.Color.lerp(null, color, t)!)
                );
            }
            else
            {
                swatch = a._swatch.map(
                    (key, color) =>
                        new MapEntry<TKey, Color>(key, Dart_uiLibrary.Color.lerp(color, b[key], t)!)
                );
            }
        }
        return new ColorSwatch<TKey>(Dart_uiLibrary.Color.lerp(a, b, t)!.value, swatch);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public class ColorProperty : DiagnosticsProperty<Color>
{
    public ColorProperty(
        string name,
        Color? value,
        bool showName = true,
        object? defaultValue = default!,
        DiagnosticsTreeStyle style = DiagnosticsTreeStyle.singleLine,
        DiagnosticLevel level = DiagnosticLevel.info
    )
        : base(
            name,
            value,
            showName: showName,
            defaultValue: defaultValue ?? DiagnosticsLibrary.kNoDefaultValue,
            style: style,
            level: level
        ) { }

    public override DartMap<string, object?> toJsonMap(
        DiagnosticsSerializationDelegate? @delegate = null
    )
    {
        DartMap<string, object?> json = base.toJsonMap(@delegate);
        if (value is not null)
        {
            json["valueProperties"] = new DartMap<string, object>
            {
                ["red"] = value!.red,
                ["green"] = value!.green,
                ["blue"] = value!.blue,
                ["alpha"] = value!.alpha,
            };
        }
        return json;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}
