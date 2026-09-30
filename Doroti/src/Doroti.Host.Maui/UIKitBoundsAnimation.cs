#if IOS && !MACCATALYST
using CoreAnimation;
using CoreGraphics;
using Foundation;

namespace Doroti.Host.Maui;

/// <summary>
/// Evaluate UIKit's actual bounds animation for the upcoming display, rather
/// than laying out the presentation tree's already displayed (older) size.
/// Unsupported animations deliberately fall back to presentation sampling.
/// </summary>
internal static class UIKitBoundsAnimation
{
    internal static CGSize? Sample(CALayer layer, double displayTime)
    {
        using var value = layer.AnimationForKey("bounds.size");
        if (
            value is not CABasicAnimation animation
            || animation is CASpringAnimation
            || animation.Duration <= 0
            || animation.Speed <= 0
            || animation.AutoReverses
            || animation.RepeatCount != 0
            || animation.RepeatDuration != 0
            || animation.From is not NSValue fromValue
        )
        {
            return null;
        }

        var model = layer.Bounds.Size;
        var from = fromValue.CGSizeValue;
        var to =
            animation.To is NSValue toValue ? toValue.CGSizeValue
            : animation.Additive ? CGSize.Empty
            : model;
        var x1 = 0.0;
        var y1 = 0.0;
        var x2 = 1.0;
        var y2 = 1.0;
        if (animation.TimingFunction is { } curve)
        {
            var first = curve.GetControlPoint(1);
            var second = curve.GetControlPoint(2);
            (x1, y1, x2, y2) = (first.X, first.Y, second.X, second.Y);
        }
        double elapsed;
        if (animation.BeginTime > 0)
        {
            var local = layer.ConvertTimeFromLayer(displayTime, null);
            elapsed = (local - animation.BeginTime) * animation.Speed + animation.TimeOffset;
        }
        else
        {
            // UIKit leaves beginTime at zero for its implicit additive bounds
            // animation. Recover its phase from the live presentation value and
            // invert the *native* timing curve, then advance to targetTimestamp.
            if (layer.PresentationLayer is not { } presentation)
                return null;
            var current = presentation.Bounds.Size;
            if (animation.Additive)
                current = new CGSize(current.Width - model.Width, current.Height - model.Height);
            var dx = (double)(to.Width - from.Width);
            var dy = (double)(to.Height - from.Height);
            var distance = dx * dx + dy * dy;
            if (distance <= 0 || y1 < 0 || y1 > 1 || y2 < 0 || y2 > 1)
                return null;
            var position = Math.Clamp(
                ((current.Width - from.Width) * dx + (current.Height - from.Height) * dy)
                    / distance,
                0,
                1
            );
            var phase = EvaluateTiming(position, y1, x1, y2, x2);
            var remaining =
                layer.ConvertTimeFromLayer(displayTime, null)
                - layer.ConvertTimeFromLayer(CAAnimation.CurrentMediaTime(), null);
            elapsed = phase * animation.Duration + Math.Max(0, remaining) * animation.Speed;
        }
        var progress = EvaluateTiming(
            Math.Clamp(elapsed / animation.Duration, 0, 1),
            x1,
            y1,
            x2,
            y2
        );
        var width = from.Width + (to.Width - from.Width) * progress;
        var height = from.Height + (to.Height - from.Height) * progress;
        if (animation.Additive)
        {
            width += model.Width;
            height += model.Height;
        }
        return width > 0 && height > 0 && double.IsFinite(width) && double.IsFinite(height)
            ? new CGSize(width, height)
            : null;
    }

    internal static double EvaluateTiming(double time, double x1, double y1, double x2, double y2)
    {
        if (time <= 0)
            return 0;
        if (time >= 1)
            return 1;
        // The timing function is y(x), not y(t). Solve x(t) before sampling y.
        var low = 0.0;
        var high = 1.0;
        for (var i = 0; i < 24; i++)
        {
            var middle = (low + high) / 2;
            if (Bezier(middle, x1, x2) < time)
                low = middle;
            else
                high = middle;
        }
        return Bezier((low + high) / 2, y1, y2);
    }

    private static double Bezier(double t, double first, double second) =>
        3 * (1 - t) * (1 - t) * t * first + 3 * (1 - t) * t * t * second + t * t * t;
}
#endif
