namespace Doroti.Host.Maui;

internal static class UIKitAnimationTiming
{
    internal static double Cubic(double time, double x1, double y1, double x2, double y2)
    {
        if (time <= 0) return 0;
        if (time >= 1) return 1;
        // Core Animation specifies y(x), so solve x before evaluating y.
        var low = 0.0;
        var high = 1.0;
        for (var i = 0; i < 24; i++)
        {
            var middle = (low + high) / 2;
            if (Bezier(middle, x1, x2) < time) low = middle;
            else high = middle;
        }
        return Bezier((low + high) / 2, y1, y2);
    }

    internal static double Spring(
        double time, double mass, double stiffness, double damping, double velocity
    )
    {
        if (time <= 0) return 0;
        var frequencySquared = stiffness / mass;
        // UIKit's keyboard spring can report damping above the critical value
        // (e.g. mass=3, stiffness=1000, damping=500). Core Animation presents
        // that as critical damping, rather than the slower overdamped solution.
        var naturalFrequency = Math.Sqrt(frequencySquared);
        var decay = Math.Min(damping / (2 * mass), naturalFrequency);
        var discriminant = decay * decay - frequencySquared;
        if (Math.Abs(discriminant) <= 1e-10 * frequencySquared)
            return 1 - Math.Exp(-decay * time) * (1 + (decay - velocity) * time);
        var frequency = Math.Sqrt(-discriminant);
        return 1 - Math.Exp(-decay * time)
            * (Math.Cos(frequency * time)
                + (decay - velocity) / frequency * Math.Sin(frequency * time));
    }

    private static double Bezier(double t, double first, double second) =>
        3 * (1 - t) * (1 - t) * t * first + 3 * (1 - t) * t * t * second + t * t * t;
}
