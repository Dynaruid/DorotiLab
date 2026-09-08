using System.Runtime.CompilerServices;
using SkiaSharp;

namespace Doroti.Skia.RuntimeEffects;

/// <summary>Explicit recorder ownership for Graphite surfaces and compatible GPU captures.</summary>
public static class SkiaGpuSurfaces
{
    private sealed record Owner(SKGraphiteRecorder Recorder, int Thread);
    private static readonly ConditionalWeakTable<SKSurface, Owner> Owners = new();

    public static SKSurface Register(SKSurface surface, SKGraphiteRecorder recorder)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ArgumentNullException.ThrowIfNull(recorder);
        Owners.Add(surface, new(recorder, Environment.CurrentManagedThreadId));
        return surface;
    }

    public static SKGraphiteRecorder? RecorderFor(SKCanvas canvas)
    {
        if (canvas.Surface is not { } surface || !Owners.TryGetValue(surface, out var owner)) return null;
        if (owner.Thread != Environment.CurrentManagedThreadId || owner.Recorder.Handle == IntPtr.Zero)
            throw new InvalidOperationException("Graphite surface accessed outside its live recorder owner.");
        return owner.Recorder;
    }

    public static bool IsGpu(SKCanvas canvas) => canvas.Context is not null || RecorderFor(canvas) is not null;

    public static SKSurface CreateCompatible(SKCanvas canvas, SKImageInfo info, SKSurfaceProperties? properties)
    {
        if (RecorderFor(canvas) is { } recorder)
            return Register(SKSurface.Create(recorder, info, properties)
                ?? throw new InvalidOperationException("Graphite GPU surface allocation failed."), recorder);
        return SKSurface.Create(canvas.Context
            ?? throw new InvalidOperationException("A GPU capture requires an owned GPU canvas."), true, info, properties)
            ?? throw new InvalidOperationException("Ganesh GPU surface allocation failed.");
    }
}
