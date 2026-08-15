// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/painting/text_scaler.dart
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

public abstract class TextScaler
{
    public static TextScaler noScaling = new _LinearTextScaler__text_scaler(1.0);

    protected TextScaler()
    {
    }

    public static TextScaler CreateLinear(double textScaleFactor)
        => new _LinearTextScaler__text_scaler(textScaleFactor);

    public abstract double scale(double fontSize);
    public abstract double textScaleFactor { get; }
    public virtual TextScaler clamp(double minScaleFactor = 0, double maxScaleFactor = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => (maxScaleFactor >= minScaleFactor));
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxScaleFactor));
        DartRuntimePrimitives.Assert(() => double.IsFinite(minScaleFactor));
        DartRuntimePrimitives.Assert(() => (minScaleFactor >= 0L));
        if (((minScaleFactor == 0L) && (maxScaleFactor == double.PositiveInfinity)))
        {
            return this;
        }
        return ((minScaleFactor == maxScaleFactor) ? TextScaler.CreateLinear(minScaleFactor) : new _ClampedTextScaler__text_scaler(this, minScaleFactor, maxScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _LinearTextScaler__text_scaler : TextScaler
{
    private double __field_textScaleFactor = default!;
    public override double textScaleFactor { get => __field_textScaleFactor; }

    internal _LinearTextScaler__text_scaler(double textScaleFactor)
    {
        this.__field_textScaleFactor = textScaleFactor;
        System.Diagnostics.Debug.Assert((textScaleFactor >= 0L));
    }

    public override double scale(double fontSize)
    {
        DartRuntimePrimitives.Assert(() => (fontSize >= 0L));
        DartRuntimePrimitives.Assert(() => double.IsFinite(fontSize));
        return (fontSize * this.textScaleFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override TextScaler clamp(double minScaleFactor = 0, double maxScaleFactor = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => (maxScaleFactor >= minScaleFactor));
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxScaleFactor));
        DartRuntimePrimitives.Assert(() => double.IsFinite(minScaleFactor));
        DartRuntimePrimitives.Assert(() => (minScaleFactor >= 0L));
        double newScaleFactor__3878 = Dart_uiLibrary.clampDouble(this.textScaleFactor, minScaleFactor, maxScaleFactor);
        return ((newScaleFactor__3878 == this.textScaleFactor) ? this : new _LinearTextScaler__text_scaler(newScaleFactor__3878));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _LinearTextScaler__text_scaler;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return ((__other is _LinearTextScaler__text_scaler) && (((_LinearTextScaler__text_scaler)((_LinearTextScaler__text_scaler)__other)).textScaleFactor == this.textScaleFactor));
    }

    public override int GetHashCode() => this.textScaleFactor.GetHashCode();
    public override string ToString() => ((this.textScaleFactor == 1.0) ? "no scaling" : $"linear ({this.textScaleFactor}x)");
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
        System.Diagnostics.Debug.Assert((maxScale > minScale));
    }

    public override double textScaleFactor => Dart_uiLibrary.clampDouble(((TextScaler)this.scaler).textScaleFactor, this.minScale, this.maxScale);
    public override double scale(double fontSize)
    {
        DartRuntimePrimitives.Assert(() => (fontSize >= 0L));
        DartRuntimePrimitives.Assert(() => double.IsFinite(fontSize));
        return Dart_uiLibrary.clampDouble(this.scaler.scale(fontSize), (this.minScale * fontSize), (this.maxScale * fontSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override TextScaler clamp(double minScaleFactor = 0, double maxScaleFactor = double.PositiveInfinity)
    {
        DartRuntimePrimitives.Assert(() => (maxScaleFactor >= minScaleFactor));
        DartRuntimePrimitives.Assert(() => !double.IsNaN(maxScaleFactor));
        DartRuntimePrimitives.Assert(() => double.IsFinite(minScaleFactor));
        DartRuntimePrimitives.Assert(() => (minScaleFactor >= 0L));
        double newMinScale__5223 = Math.Max(this.minScale, minScaleFactor);
        double newMaxScale__5285 = Math.Min(this.maxScale, maxScaleFactor);
        if ((newMaxScale__5285 <= newMinScale__5223))
        {
            return TextScaler.CreateLinear(Dart_uiLibrary.clampDouble(this.minScale, minScaleFactor, maxScaleFactor));
        }
        return new _ClampedTextScaler__text_scaler(this.scaler, newMinScale__5223, newMaxScale__5285);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool Equals(object? other)
    {
        var __other = other as _ClampedTextScaler__text_scaler;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        return ((((__other is _ClampedTextScaler__text_scaler) && (this.minScale == ((_ClampedTextScaler__text_scaler)((_ClampedTextScaler__text_scaler)__other)).minScale)) && (this.maxScale == ((_ClampedTextScaler__text_scaler)((_ClampedTextScaler__text_scaler)__other)).maxScale)) && (object.Equals(this.scaler, ((_ClampedTextScaler__text_scaler)((_ClampedTextScaler__text_scaler)__other)).scaler)));
    }

    public override int GetHashCode() => FoundationRuntimePorts.ObjectHash(this.scaler, this.minScale, this.maxScale);
    public override string ToString() => $"{this.scaler} clamped [{this.minScale}, {this.maxScale}]";
}

