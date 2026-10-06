// Adapted from flutter_custom_carousel, Copyright (c) 2024, gskinner.com, inc.
using Doroti.Framework.Physics;
using Doroti.Framework.Widgets;

namespace Doroti.CustomCarousel;

public sealed class CustomCarouselScrollPhysics : BouncingScrollPhysics
{
    public bool sticky { get; }
    public double stiffness { get; }

    public CustomCarouselScrollPhysics(ScrollPhysics? parent = null, bool sticky = false, double stiffness = 1)
        : base(parent: parent)
    {
        if (!double.IsFinite(stiffness) || stiffness <= 0) throw new ArgumentOutOfRangeException(nameof(stiffness));
        this.sticky = sticky;
        this.stiffness = stiffness;
    }

    public override CustomCarouselScrollPhysics applyTo(ScrollPhysics? ancestor) =>
        new(buildParent(ancestor), sticky, stiffness);

    public override Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        if (position is not CustomCarouselScrollPosition metrics)
            throw new ArgumentException("CustomCarouselScrollPhysics requires CustomCarouselScrollController.", nameof(position));
        var pixels = metrics.pixels;
        if ((velocity <= 0 && pixels <= metrics.minScrollExtent) ||
            (velocity >= 0 && pixels >= metrics.maxScrollExtent))
            return base.createBallisticSimulation(metrics, velocity);
        var friction = base.createBallisticSimulation(metrics, velocity);
        // Doroti friction includes a deceleration term: evaluating 0 * infinity
        // produces NaN. Sample its finite stopping point instead (at most 30 steps).
        var natural = pixels;
        if (friction is not null)
        {
            for (var second = 1; second <= 30; second++)
            {
                var candidate = friction.x(second);
                if (!double.IsFinite(candidate)) break;
                natural = candidate;
                if (friction.isDone(second)) break;
            }
            if (natural == metrics.minScrollExtent || natural == metrics.maxScrollExtent) return friction;
        }
        var target = metrics.getItemFromOffset(natural) * metrics.itemExtent;
        var delta = (target - pixels) / metrics.itemExtent;
        if (sticky && Math.Abs(delta) > 1)
            target = metrics.getItemFromOffset(pixels + metrics.itemExtent * Math.Sign(delta)) * metrics.itemExtent;
        var tolerance = new Tolerance(velocity: 200 / metrics.devicePixelRatio,
            time: 0.01, distance: 1 / metrics.devicePixelRatio);
        if (Math.Abs(velocity) < tolerance.velocity && Math.Abs(target - pixels) < tolerance.distance) return null;
        return new SpringSimulation(SpringDescription.CreateWithDampingRatio(mass: 1,
            stiffness: Math.Min(150, 1000 / Math.Sqrt(Math.Max(0.001, Math.Abs(pixels - target))) + 15) * stiffness),
            pixels, target, velocity, snapToEnd: true, tolerance: tolerance);
    }
}
