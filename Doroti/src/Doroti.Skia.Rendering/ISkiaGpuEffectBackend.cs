using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public interface ISkiaGpuEffectBackend
{
    void Draw(SKCanvas destination, int width, int height, Action<SKCanvas> capture,
        GpuEffectProgram program, GpuEffectParameters parameters, float logicalWidth = 0, float logicalHeight = 0);
}

/// <summary>Render-owner scope for hosts whose GPU context is not a SkiaGraphiteSession.</summary>
public sealed class SkiaGpuEffectScope : IDisposable
{
    [ThreadStatic] private static ISkiaGpuEffectBackend? _current;
    private readonly ISkiaGpuEffectBackend? _previous;
    private readonly int _thread = Environment.CurrentManagedThreadId;
    public static ISkiaGpuEffectBackend? Current => _current;
    public SkiaGpuEffectScope(ISkiaGpuEffectBackend backend)
    {
        _previous = _current;
        _current = backend;
    }
    public void Dispose()
    {
        if (_thread != Environment.CurrentManagedThreadId) throw new InvalidOperationException("GPU scope crossed its owner thread.");
        _current = _previous;
    }
}

public sealed partial class SkiaGraphiteSession
{
    public ISkiaGpuEffectBackend? GpuEffects { get; set; }
}
