// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/viewport_offset.dart
using Doroti.Runtime;

namespace Doroti.Framework.Rendering;

public enum ScrollDirection
{
    idle,
    forward,
    reverse,
}

public static partial class Viewport_offsetLibrary
{
    public static ScrollDirection flipScrollDirection(ScrollDirection direction)
    {
        return direction switch
        {
            ScrollDirection.idle => ScrollDirection.idle,
            ScrollDirection.forward => ScrollDirection.reverse,
            ScrollDirection.reverse => ScrollDirection.forward,
            _ => throw new InvalidOperationException(
                "Switch expression did not handle the supplied value."
            ),
        };
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }
}

public abstract class ViewportOffset : ChangeNotifier
{
    protected ViewportOffset() { }

    public static ViewportOffset CreateFixed(double value) =>
        new _FixedViewportOffset__viewport_offset(value);

    public static ViewportOffset CreateZero() => _FixedViewportOffset__viewport_offset.CreateZero();

    public abstract double pixels { get; }
    public abstract bool hasPixels { get; }
    public abstract bool applyViewportDimension(double viewportDimension);
    public abstract bool applyContentDimensions(double minScrollExtent, double maxScrollExtent);
    public abstract void correctBy(double correction);
    public abstract void jumpTo(double pixels);
    public abstract Future animateTo(double to, Duration duration, Curve curve);

    public virtual Future moveTo(
        double to,
        Duration? duration = null,
        Curve? curve = null,
        bool? clamp = null
    )
    {
        if (
            (duration is null)
            || Equals(
                (
                    duration
                    ?? throw new global::System.NullReferenceException("A required value was null.")
                ),
                Duration.zero
            )
        )
        {
            jumpTo(to);
            return Future.value();
        }
        else
        {
            return animateTo(
                to,
                duration: (
                    (
                        duration
                        ?? throw new global::System.NullReferenceException(
                            "A required value was null."
                        )
                    )
                ),
                curve: curve ?? Curves.ease
            );
        }
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public abstract ScrollDirection userScrollDirection { get; }
    public abstract bool allowImplicitScrolling { get; }

    public override string ToString()
    {
        var description = new List<string>();
        debugFillDescription(description);
        return $"{DiagnosticsLibrary.describeIdentity(this)}({string.Join(", ", description)})";
        throw new InvalidOperationException("Control flow completed without returning a value.");
    }

    public virtual void debugFillDescription(List<string> description)
    {
        if (hasPixels)
        {
            description.Add($"offset: {pixels.toStringAsFixed(1L)}");
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

    internal static new _FixedViewportOffset__viewport_offset CreateZero()
    {
        var __instance = new _FixedViewportOffset__viewport_offset(default!);
        __instance._pixels = 0.0;
        return __instance;
    }

    public override double pixels => _pixels;
    public override bool hasPixels => true;

    public override bool applyViewportDimension(double viewportDimension) => true;

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent) =>
        true;

    public override void correctBy(double correction)
    {
        _pixels += correction;
    }

    public override void jumpTo(double pixels) { }

    public override async Future animateTo(double to, Duration duration, Curve curve) { }

    public override ScrollDirection userScrollDirection => ScrollDirection.idle;
    public override bool allowImplicitScrolling => false;
}
