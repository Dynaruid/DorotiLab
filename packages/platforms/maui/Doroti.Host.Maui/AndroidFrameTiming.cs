namespace Doroti.Host.Maui;

internal static class AndroidFrameTiming
{
    // Choreographer's frameTimeNanos is in System.nanoTime's clock domain.
    // Anchoring it to callback arrival shifts animation time into the future
    // by the first callback's queue delay. Retain that age, including after a
    // delayed first callback or resume, just as the input clock conversion does.
    internal static TimeSpan Timestamp(TimeSpan now, long nativeNowNanos, long frameTimeNanos)
    {
        if (frameTimeNanos <= 0 || frameTimeNanos > nativeNowNanos) return now;
        var age = TimeSpan.FromTicks((nativeNowNanos - frameTimeNanos) / 100);
        return age >= now ? TimeSpan.Zero : now - age;
    }
}
