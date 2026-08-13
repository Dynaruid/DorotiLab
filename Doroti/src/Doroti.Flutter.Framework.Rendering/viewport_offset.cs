// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/viewport_offset.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Rendering;

public enum ScrollDirection
{
    idle,
    forward,
    reverse
}

public static partial class Viewport_offsetLibrary
{
    public static ScrollDirection flipScrollDirection(ScrollDirection direction)
    {
        return (direction switch { ScrollDirection.idle => ScrollDirection.idle, ScrollDirection.forward => ScrollDirection.reverse, ScrollDirection.reverse => ScrollDirection.forward, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public abstract class ViewportOffset : ChangeNotifier
{
    protected ViewportOffset()
    {
    }

    public static ViewportOffset CreateFixed(double value)
        => new _FixedViewportOffset__viewport_offset(value);

    public static ViewportOffset CreateZero()
        => _FixedViewportOffset__viewport_offset.CreateZero();

    public abstract double pixels { get; }
    public abstract bool hasPixels { get; }
    public abstract bool applyViewportDimension(double viewportDimension);
    public abstract bool applyContentDimensions(double minScrollExtent, double maxScrollExtent);
    public abstract void correctBy(double correction);
    public abstract void jumpTo(double pixels);
    public abstract Future animateTo(double to, Duration duration, Curve curve);
    public virtual Future moveTo(double to, Duration? duration = null, Curve? curve = null, bool? clamp = null)
    {
        if (((duration is null) || (object.Equals(DartRuntimePrimitives.RequireValue(duration), Duration.zero))))
        {
            jumpTo(to);
            return Future.value();
        }
        else
        {
            return animateTo(to, duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)), curve: (curve ?? Curves.ease));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public abstract ScrollDirection userScrollDirection { get; }
    public abstract bool allowImplicitScrolling { get; }
    public override string ToString()
    {
        var description__11654 = new List<string>();
        debugFillDescription(description__11654);
        return $"{(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}({string.Join(", ", description__11654)})";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillDescription(List<string> description)
    {
        if (this.hasPixels)
        {
            description.Add($"offset: {this.pixels.toStringAsFixed(1L)}");
        }
    }

}

internal class _FixedViewportOffset__viewport_offset : ViewportOffset
{
    internal virtual double _pixels { get; set; } = default!;

    internal _FixedViewportOffset__viewport_offset(double _pixels)
    {
        this._pixels = _pixels;
    }

    internal static _FixedViewportOffset__viewport_offset CreateZero()
    {
        var __instance = new _FixedViewportOffset__viewport_offset(default!);
        __instance._pixels = 0.0;
        return __instance;
    }

    public override double pixels => this._pixels;
    public override bool hasPixels => true;
    public override bool applyViewportDimension(double viewportDimension) => true;
    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent) => true;
    public override void correctBy(double correction)
    {
        _pixels += correction;
    }

    public override void jumpTo(double pixels)
    {
    }

    public async override Future animateTo(double to, Duration duration, Curve curve)
    {
    }

    public override ScrollDirection userScrollDirection => ScrollDirection.idle;
    public override bool allowImplicitScrolling => false;
}

