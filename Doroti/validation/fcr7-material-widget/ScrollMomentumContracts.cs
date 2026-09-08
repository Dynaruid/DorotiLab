using Doroti.Host.Maui;

internal static class ScrollMomentumContracts
{
    internal static void Verify()
    {
        var momentum = new MauiScrollMomentum();
        if (momentum.Start(0, 0, TimeSpan.Zero) || momentum.Start(double.NaN, 100, TimeSpan.Zero))
            throw new Exception("Stationary/invalid velocity must not start inertia.");
        double Distance(int rate)
        {
            momentum.Start(0, -1200, TimeSpan.Zero);
            var distance = 0d;
            var previous = double.PositiveInfinity;
            var frame = 0;
            while (momentum.IsActive && frame < rate * 5)
            {
                var delta = momentum.Advance(TimeSpan.FromSeconds((double)++frame / rate));
                if (delta.X != 0 || delta.Y >= 0 || Math.Abs(delta.Y) > previous)
                    throw new Exception("Inertia must preserve direction and decay monotonically.");
                previous = Math.Abs(delta.Y);
                distance -= delta.Y;
            }
            if (momentum.IsActive || distance < 190 || distance > 200)
                throw new Exception("Inertia failed to travel and settle.");
            return distance;
        }
        if (Math.Abs(Distance(60) - Distance(120)) > .5)
            throw new Exception("Inertial distance depends on refresh rate.");
        momentum.Start(500, 500, TimeSpan.Zero);
        momentum.Stop();
        if (momentum.Advance(TimeSpan.FromMilliseconds(16)) != default)
            throw new Exception("Interrupted inertia emitted a delta.");
        momentum.Start(500, 500, TimeSpan.Zero);
        if (momentum.Advance(TimeSpan.FromSeconds(1)) != default || momentum.IsActive)
            throw new Exception("Resumed display link must not jump after suspension.");
        momentum.Start(-600, 0, TimeSpan.Zero);
        momentum.Start(600, 0, TimeSpan.Zero);
        if (momentum.Advance(TimeSpan.FromMilliseconds(16)).X <= 0)
            throw new Exception("New gesture did not replace previous velocity.");
        Console.WriteLine("Catalyst scroll inertia: decay, refresh rates, direction, interruption and suspension PASS");
    }
}
