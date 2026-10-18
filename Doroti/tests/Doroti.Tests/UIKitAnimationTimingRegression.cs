using Doroti.Host.Maui;

internal static class UIKitAnimationTimingRegression
{
    internal static void Run()
    {
        static void Near(double actual, double expected, double tolerance, string reason)
        {
            if (!double.IsFinite(actual) || Math.Abs(actual - expected) > tolerance)
                throw new Exception($"{reason}: expected {expected}, got {actual}.");
        }

        // Independent reference values for Core Animation's standard Beziers.
        Near(UIKitAnimationTiming.Cubic(.25, .42, 0, 1, 1), .09346465, 1e-7, "Ease in");
        Near(UIKitAnimationTiming.Cubic(.25, 0, 0, .58, 1), .37813813, 1e-7, "Ease out");
        Near(UIKitAnimationTiming.Cubic(.25, .42, 0, .58, 1), .12916193, 1e-7, "Ease in/out");
        Near(UIKitAnimationTiming.Cubic(.25, 0, 0, 1, 1), .25, 1e-7, "Linear");
        Near(UIKitAnimationTiming.Cubic(-1, .42, 0, .58, 1), 0, 0, "Before animation");
        Near(UIKitAnimationTiming.Cubic(2, .42, 0, .58, 1), 1, 0, "After animation");

        // Critical damping has the independent closed form 1-(1+10t)e^(-10t).
        Near(UIKitAnimationTiming.Spring(.1, 1, 100, 20, 0), 1 - 2 / Math.E, 1e-12,
            "Critical spring");
        // Measured UIKit curve 7 reports these parameters, but presents the
        // critically damped response (about .84 at .18s), not overdamped .27.
        var nativeFrequency = Math.Sqrt(1000.0 / 3);
        Near(UIKitAnimationTiming.Spring(.18, 3, 1000, 500, 0),
            1 - (1 + nativeFrequency * .18) * Math.Exp(-nativeFrequency * .18), 1e-12,
            "UIKit keyboard damping clamp");
        // An undamped spring must retain overshoot, rather than clamp its progress.
        Near(UIKitAnimationTiming.Spring(Math.PI / 10, 1, 100, 0, 0), 2, 1e-12,
            "Spring overshoot");
        foreach (var damping in new[] { 10.0, 20.0, 30.0 })
        {
            foreach (var velocity in new[] { -2.0, 0.0, 3.0 })
            {
                double Position(double time) => UIKitAnimationTiming.Spring(time, 1, 100, damping, velocity);
                const double step = 1e-5;
                Near(Position(0), 0, 0, "Spring origin");
                Near((Position(step) - Position(0)) / step, velocity, .001,
                    "Native spring velocity direction");
                // Match the native effective damping in the oscillator equation.
                var effectiveDamping = Math.Min(damping, 20);
                foreach (var time in new[] { .05, .15, .3 })
                {
                    var position = Position(time);
                    var speed = (Position(time + step) - Position(time - step)) / (2 * step);
                    var acceleration = (Position(time + step) - 2 * position + Position(time - step))
                        / (step * step);
                    Near(acceleration + effectiveDamping * speed + 100 * (position - 1), 0, .0001,
                        $"Spring equation with damping {damping}");
                }
                Near(Position(10), 1, 1e-10, "Spring settles");
            }
        }
        var current = UIKitAnimationTiming.Spring(.1, 1, 100, 20, 0);
        var nextDisplay = UIKitAnimationTiming.Spring(.1 + 1.0 / 60, 1, 100, 20, 0);
        if (nextDisplay <= current)
            throw new Exception("The upcoming display must advance the keyboard dismissal inset.");
        Console.WriteLine("UIKit animation timing: PASS");
    }
}
