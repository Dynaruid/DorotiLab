// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_scaler.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Painting;

public abstract class TextScaler
{
    public static TextScaler noScaling = new _LinearTextScaler__text_scaler(1.0);

    protected TextScaler() { }

    public static TextScaler CreateLinear(double textScaleFactor) =>
        new _LinearTextScaler__text_scaler(textScaleFactor);

    public abstract double scale(double fontSize);
    public abstract double textScaleFactor { get; }

    public virtual TextScaler clamp(
        double minScaleFactor = 0,
        double maxScaleFactor = double.PositiveInfinity
    )
    {
        DartRuntimePrimitives.Assert(() => maxScaleFactor >= minScaleFactor);
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxScaleFactor));
        DartRuntimePrimitives.Assert(() => double.IsFinite(minScaleFactor));
        DartRuntimePrimitives.Assert(() => minScaleFactor >= 0L);
        if ((minScaleFactor == 0L) && (maxScaleFactor == double.PositiveInfinity))
        {
            return this;
        }
        return (minScaleFactor == maxScaleFactor)
            ? CreateLinear(minScaleFactor)
            : new _ClampedTextScaler__text_scaler(this, minScaleFactor, maxScaleFactor);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

internal class _LinearTextScaler__text_scaler : TextScaler
{
    private double __field_textScaleFactor = default!;
    public override double textScaleFactor
    {
        get => __field_textScaleFactor;
    }

    internal _LinearTextScaler__text_scaler(double textScaleFactor)
    {
        __field_textScaleFactor = textScaleFactor;
        System.Diagnostics.Debug.Assert(textScaleFactor >= 0L);
    }

    public override double scale(double fontSize)
    {
        DartRuntimePrimitives.Assert(() => fontSize >= 0L);
        DartRuntimePrimitives.Assert(() => double.IsFinite(fontSize));
        return fontSize * textScaleFactor;
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override TextScaler clamp(
        double minScaleFactor = 0,
        double maxScaleFactor = double.PositiveInfinity
    )
    {
        DartRuntimePrimitives.Assert(() => maxScaleFactor >= minScaleFactor);
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxScaleFactor));
        DartRuntimePrimitives.Assert(() => double.IsFinite(minScaleFactor));
        DartRuntimePrimitives.Assert(() => minScaleFactor >= 0L);
        double newScaleFactor = DorotiUiLibrary.clampDouble(
            textScaleFactor,
            minScaleFactor,
            maxScaleFactor
        );
        return (newScaleFactor == textScaleFactor)
            ? this
            : new _LinearTextScaler__text_scaler(newScaleFactor);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _LinearTextScaler__text_scaler;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is _LinearTextScaler__text_scaler)
            && (__other.textScaleFactor == textScaleFactor);
    }

    public override int GetHashCode() => textScaleFactor.GetHashCode();

    public override string ToString() =>
        (textScaleFactor == 1.0) ? "no scaling" : $"linear ({textScaleFactor}x)";
}

internal class _ClampedTextScaler__text_scaler : TextScaler
{
    public virtual TextScaler scaler { get; private set; } = default!;
    public virtual double minScale { get; private set; } = default!;
    public virtual double maxScale { get; private set; } = default!;

    internal _ClampedTextScaler__text_scaler(TextScaler scaler, double minScale, double maxScale)
    {
        this.scaler = scaler;
        this.minScale = minScale;
        this.maxScale = maxScale;
        System.Diagnostics.Debug.Assert(maxScale > minScale);
    }

    public override double textScaleFactor =>
        DorotiUiLibrary.clampDouble(scaler.textScaleFactor, minScale, maxScale);

    public override double scale(double fontSize)
    {
        DartRuntimePrimitives.Assert(() => fontSize >= 0L);
        DartRuntimePrimitives.Assert(() => double.IsFinite(fontSize));
        return DorotiUiLibrary.clampDouble(
            scaler.scale(fontSize),
            minScale * fontSize,
            maxScale * fontSize
        );
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override TextScaler clamp(
        double minScaleFactor = 0,
        double maxScaleFactor = double.PositiveInfinity
    )
    {
        DartRuntimePrimitives.Assert(() => maxScaleFactor >= minScaleFactor);
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxScaleFactor));
        DartRuntimePrimitives.Assert(() => double.IsFinite(minScaleFactor));
        DartRuntimePrimitives.Assert(() => minScaleFactor >= 0L);
        double newMinScale = Math.Max(minScale, minScaleFactor);
        double newMaxScale = Math.Min(maxScale, maxScaleFactor);
        if (newMaxScale <= newMinScale)
        {
            return CreateLinear(
                DorotiUiLibrary.clampDouble(minScale, minScaleFactor, maxScaleFactor)
            );
        }
        return new _ClampedTextScaler__text_scaler(scaler, newMinScale, newMaxScale);
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ClampedTextScaler__text_scaler;
        if (__other is null)
        {
            return false;
        }

        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return (__other is _ClampedTextScaler__text_scaler)
            && (minScale == __other.minScale)
            && (maxScale == __other.maxScale)
            && Equals(scaler, __other.scaler);
    }

    public override int GetHashCode() =>
        FoundationRuntimePorts.ObjectHash(scaler, minScale, maxScale);

    public override string ToString() => $"{scaler} clamped [{minScale}, {maxScale}]";
}
