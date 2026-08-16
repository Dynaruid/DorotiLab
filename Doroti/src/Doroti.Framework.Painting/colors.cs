// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/colors.dart
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

namespace Doroti.Framework.Painting;

public static partial class ColorsLibrary
{
    internal static double _getHue(double red, double green, double blue, double max, double delta)
    {
        double hue__432 = default!;
        if ((max == 0.0))
        {
            hue__432 = 0.0;
        }
        else
        {
            if ((max == red))
            {
                hue__432 = (60.0 * ((((((green - blue)) / delta)) % 6L)));
            }
            else
            {
                if ((max == green))
                {
                    hue__432 = (60.0 * ((((((blue - red)) / delta)) + 2L)));
                }
                else
                {
                    if ((max == blue))
                    {
                        hue__432 = (60.0 * ((((((red - green)) / delta)) + 4L)));
                    }
                }
            }
        }
        hue__432 = (double.IsNaN(hue__432) ? 0.0 : hue__432);
        return hue__432;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class ColorsLibrary
{
    internal static Color _colorFromHue(double alpha, double hue, double chroma, double secondary, double match)
    {
        var (red__912, green__924, blue__938) = (hue switch { < 60.0 => (((double, double, double))((chroma, secondary, 0.0))), < 120.0 => (((double, double, double))((secondary, chroma, 0.0))), < 180.0 => (((double, double, double))((0.0, chroma, secondary))), < 240.0 => (((double, double, double))((0.0, secondary, chroma))), < 300.0 => (((double, double, double))((secondary, 0.0, chroma))), _ => (((double, double, double))((chroma, 0.0, secondary))) });
        return global::Doroti.Ui.Color.fromARGB(((alpha * 255L)).round(), ((((red__912 + match)) * 255L)).round(), ((((green__924 + match)) * 255L)).round(), ((((blue__938 + match)) * 255L)).round());
        throw new InvalidOperationException("Dart control flow completed without a value.");
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
        System.Diagnostics.Debug.Assert((alpha >= 0.0));
        System.Diagnostics.Debug.Assert((alpha <= 1.0));
        System.Diagnostics.Debug.Assert((hue >= 0.0));
        System.Diagnostics.Debug.Assert((hue <= 360.0));
        System.Diagnostics.Debug.Assert((saturation >= 0.0));
        System.Diagnostics.Debug.Assert((saturation <= 1.0));
        System.Diagnostics.Debug.Assert((value >= 0.0));
        System.Diagnostics.Debug.Assert((value <= 1.0));
    }

    public static HSVColor CreateFromColor(Color color)
    {
        double red__3114 = (color.red / 255L);
        double green__3155 = (color.green / 255L);
        double blue__3200 = (color.blue / 255L);
        double max__3244 = Math.Max(red__3114, Math.Max(green__3155, blue__3200));
        double min__3305 = Math.Min(red__3114, Math.Min(green__3155, blue__3200));
        double delta__3366 = (max__3244 - min__3305);
        double alpha__3403 = (color.alpha / 255L);
        double hue__3448 = ColorsLibrary._getHue(red__3114, green__3155, blue__3200, max__3244, delta__3366);
        double saturation__3510 = ((max__3244 == 0.0) ? 0.0 : (delta__3366 / max__3244));
        return new HSVColor(alpha__3403, hue__3448, saturation__3510, max__3244);
    }

    public virtual HSVColor withAlpha(double alpha)
    {
        return new HSVColor(alpha, this.hue, this.saturation, this.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HSVColor withHue(double hue)
    {
        return new HSVColor(this.alpha, hue, this.saturation, this.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HSVColor withSaturation(double saturation)
    {
        return new HSVColor(this.alpha, this.hue, saturation, this.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HSVColor withValue(double value)
    {
        return new HSVColor(this.alpha, this.hue, this.saturation, value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color toColor()
    {
        double chroma__5594 = (this.saturation * this.value);
        double secondary__5640 = (chroma__5594 * ((1.0 - ((((((this.hue / 60.0)) % 2.0)) - 1.0)).abs())));
        double match__5722 = (this.value - chroma__5594);
        return ColorsLibrary._colorFromHue(this.alpha, this.hue, chroma__5594, secondary__5640, match__5722);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HSVColor _scaleAlpha(double factor)
    {
        return withAlpha((this.alpha * factor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HSVColor? lerp(HSVColor? a, HSVColor? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return b!._scaleAlpha(t);
        }
        if ((b is null))
        {
            return a._scaleAlpha((1.0 - t));
        }
        return new HSVColor(Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((HSVColor)a).alpha, ((HSVColor)b).alpha, t)), 0.0, 1.0), (DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((HSVColor)a).hue, ((HSVColor)b).hue, t)) % 360.0), Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((HSVColor)a).saturation, ((HSVColor)b).saturation, t)), 0.0, 1.0), Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((HSVColor)a).value, ((HSVColor)b).value, t)), 0.0, 1.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as HSVColor;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (((((__other is HSVColor) && (((HSVColor)((HSVColor)__other)).alpha == this.alpha)) && (((HSVColor)((HSVColor)__other)).hue == this.hue)) && (((HSVColor)((HSVColor)__other)).saturation == this.saturation)) && (((HSVColor)((HSVColor)__other)).value == this.value));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.alpha, this.hue, this.saturation, this.value);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "HSVColor"))}({this.alpha}, {this.hue}, {this.saturation}, {this.value})";
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
        System.Diagnostics.Debug.Assert((alpha >= 0.0));
        System.Diagnostics.Debug.Assert((alpha <= 1.0));
        System.Diagnostics.Debug.Assert((hue >= 0.0));
        System.Diagnostics.Debug.Assert((hue <= 360.0));
        System.Diagnostics.Debug.Assert((saturation >= 0.0));
        System.Diagnostics.Debug.Assert((saturation <= 1.0));
        System.Diagnostics.Debug.Assert((lightness >= 0.0));
        System.Diagnostics.Debug.Assert((lightness <= 1.0));
    }

    public static HSLColor CreateFromColor(Color color)
    {
        double red__9552 = (color.red / 255L);
        double green__9593 = (color.green / 255L);
        double blue__9638 = (color.blue / 255L);
        double max__9682 = Math.Max(red__9552, Math.Max(green__9593, blue__9638));
        double min__9743 = Math.Min(red__9552, Math.Min(green__9593, blue__9638));
        double delta__9804 = (max__9682 - min__9743);
        double alpha__9841 = (color.alpha / 255L);
        double hue__9886 = ColorsLibrary._getHue(red__9552, green__9593, blue__9638, max__9682, delta__9804);
        double lightness__9948 = (((max__9682 + min__9743)) / 2.0);
        double saturation__10064 = ((min__9743 == max__9682) ? 0.0 : Dart_uiLibrary.clampDouble((delta__9804 / ((1.0 - (((2.0 * lightness__9948) - 1.0)).abs()))), 0.0, 1.0));
        return new HSLColor(alpha__9841, hue__9886, saturation__10064, lightness__9948);
    }

    public virtual HSLColor withAlpha(double alpha)
    {
        return new HSLColor(alpha, this.hue, this.saturation, this.lightness);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HSLColor withHue(double hue)
    {
        return new HSLColor(this.alpha, hue, this.saturation, this.lightness);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HSLColor withSaturation(double saturation)
    {
        return new HSLColor(this.alpha, this.hue, saturation, this.lightness);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual HSLColor withLightness(double lightness)
    {
        return new HSLColor(this.alpha, this.hue, this.saturation, lightness);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Color toColor()
    {
        double chroma__12385 = (((1.0 - (((2.0 * this.lightness) - 1.0)).abs())) * this.saturation);
        double secondary__12463 = (chroma__12385 * ((1.0 - ((((((this.hue / 60.0)) % 2.0)) - 1.0)).abs())));
        double match__12545 = (this.lightness - (chroma__12385 / 2.0));
        return ColorsLibrary._colorFromHue(this.alpha, this.hue, chroma__12385, secondary__12463, match__12545);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HSLColor _scaleAlpha(double factor)
    {
        return withAlpha((this.alpha * factor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static HSLColor? lerp(HSLColor? a, HSLColor? b, double t)
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        if ((a is null))
        {
            return b!._scaleAlpha(t);
        }
        if ((b is null))
        {
            return a._scaleAlpha((1.0 - t));
        }
        return new HSLColor(Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((HSLColor)a).alpha, ((HSLColor)b).alpha, t)), 0.0, 1.0), (DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((HSLColor)a).hue, ((HSLColor)b).hue, t)) % 360.0), Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((HSLColor)a).saturation, ((HSLColor)b).saturation, t)), 0.0, 1.0), Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(Dart_uiLibrary.lerpDouble(((HSLColor)a).lightness, ((HSLColor)b).lightness, t)), 0.0, 1.0));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as HSLColor;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (((((__other is HSLColor) && (((HSLColor)((HSLColor)__other)).alpha == this.alpha)) && (((HSLColor)((HSLColor)__other)).hue == this.hue)) && (((HSLColor)((HSLColor)__other)).saturation == this.saturation)) && (((HSLColor)((HSLColor)__other)).lightness == this.lightness));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.alpha, this.hue, this.saturation, this.lightness);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "HSLColor"))}({this.alpha}, {this.hue}, {this.saturation}, {this.lightness})";
}

public class ColorSwatch<T> : Color where T : notnull
{
    internal virtual DartMap<T, Color> _swatch { get; private set; } = default!;

    public ColorSwatch(long primary, DartMap<T, Color> _swatch) : base(primary)
    {
        this._swatch = _swatch;
    }

    public global::Doroti.Ui.Color? this[T key]
    {
        get
        {
            return this._swatch.GetValueOrDefault(key);
        }
    }

    public virtual IEnumerable<T> keys => this._swatch.Keys;
    public override bool Equals(object? other)
    {
        var __other = other as ColorSwatch<T>;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((base.Equals(__other) && (__other is ColorSwatch<T>)) && global::Doroti.Framework.Foundation.CollectionsLibrary.mapEquals<T, global::Doroti.Ui.Color>(((ColorSwatch<T>)((ColorSwatch<T>)__other))._swatch, this._swatch));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.GetType(), value, this._swatch);
    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "ColorSwatch"))}(primary value: {base.ToString()})";
    public static ColorSwatch<T>? lerp<T>(ColorSwatch<T>? a, ColorSwatch<T>? b, double t) where T : notnull
    {
        if (DartRuntimePrimitives.Identical(a, b))
        {
            return a;
        }
        DartMap<T, global::Doroti.Ui.Color> swatch__18030 = default!;
        if ((b is null))
        {
            swatch__18030 = a!._swatch.map<T, Color, T, Color>(((key, color) => new MapEntry<T, global::Doroti.Ui.Color>(key, Dart_uiLibrary.Color.lerp(color, null, t)!)));
        }
        else
        {
            if ((a is null))
            {
                swatch__18030 = ((ColorSwatch<T>)b)._swatch.map<T, Color, T, Color>(((key, color) => new MapEntry<T, global::Doroti.Ui.Color>(key, Dart_uiLibrary.Color.lerp(null, color, t)!)));
            }
            else
            {
                swatch__18030 = ((ColorSwatch<T>)a)._swatch.map<T, Color, T, Color>(((key, color) => new MapEntry<T, global::Doroti.Ui.Color>(key, Dart_uiLibrary.Color.lerp(color, b[key], t)!)));
            }
        }
        return new ColorSwatch<T>(Dart_uiLibrary.Color.lerp(a, b, t)!.value, swatch__18030);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class ColorProperty : DiagnosticsProperty<Color>
{
    public ColorProperty(string name, Color? value, bool showName = true, object? defaultValue = default!, DiagnosticsTreeStyle style = DiagnosticsTreeStyle.singleLine, DiagnosticLevel level = DiagnosticLevel.info) : base(name, value, showName: showName, defaultValue: defaultValue ?? global::Doroti.Framework.Foundation.DiagnosticsLibrary.kNoDefaultValue, style: style, level: level)
    {
    }

    public virtual DartMap<string, object?> toJsonMap(DiagnosticsSerializationDelegate @delegate)
    {
        DartMap<string, object?> json__19011 = base.toJsonMap(@delegate);
        if ((value is not null))
        {
            json__19011["valueProperties"] = new DartMap<string, object> { ["red"] = value!.red, ["green"] = value!.green, ["blue"] = value!.blue, ["alpha"] = value!.alpha };
        }
        return json__19011;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

