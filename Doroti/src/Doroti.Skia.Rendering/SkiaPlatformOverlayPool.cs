using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>Bounded render-owner surface pool. Leases must survive GPU and compositor retirement.</summary>
public sealed class SkiaPlatformOverlayPool : IDisposable
{
    private readonly int _thread = Environment.CurrentManagedThreadId;
    private readonly int _maximumSurfaces;
    private readonly int _maximumIdle;
    private readonly long _maximumBytes;
    private readonly List<Slot> _idle = [];
    private long _generation;
    private bool _disposed;
    private sealed record Slot(SKSurface Surface, int Width, int Height, long Generation, long Bytes);

    public SkiaPlatformOverlayPool(int maximumSurfaces = 17, long maximumBytes = 128L * 1024 * 1024, int maximumIdle = 4)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumSurfaces, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumBytes, 1);
        ArgumentOutOfRangeException.ThrowIfNegative(maximumIdle);
        _maximumSurfaces = maximumSurfaces; _maximumBytes = maximumBytes; _maximumIdle = Math.Min(maximumIdle, maximumSurfaces);
    }
    public int LiveSurfaces { get; private set; }
    public int IdleSurfaces => _idle.Count;
    /// <summary>Reserved allocation bytes provided by the backend, including its alignment/format overhead.</summary>
    public long ReservedBytes { get; private set; }
    public long Reuses { get; private set; }

    public Lease Rent(int width, int height, long allocationBytes, Func<SKSurface> create)
    {
        VerifyThread(); ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(create);
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1); ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        if (allocationBytes < checked((long)width * height * 4) || allocationBytes > _maximumBytes)
            throw new InvalidOperationException("Overlay allocation exceeds the byte budget or under-reports its pixel storage.");
        for (var index = 0; index < _idle.Count; index++)
        {
            var candidate = _idle[index];
            if (candidate.Width != width || candidate.Height != height || candidate.Bytes != allocationBytes) continue;
            _idle.RemoveAt(index); Reuses++;
            return new(this, candidate.Surface, candidate.Width, candidate.Height, candidate.Generation, candidate.Bytes);
        }
        while (_idle.Count != 0 && (LiveSurfaces >= _maximumSurfaces || ReservedBytes > _maximumBytes - allocationBytes))
        {
            var slot = _idle[0]; _idle.RemoveAt(0); Destroy(slot);
        }
        if (LiveSurfaces >= _maximumSurfaces || ReservedBytes > _maximumBytes - allocationBytes)
            throw new InvalidOperationException("Overlay pool capacity is still held by unretired frames.");
        var surface = create() ?? throw new InvalidOperationException("Overlay surface allocation failed.");
        LiveSurfaces++; ReservedBytes += allocationBytes;
        return new(this, surface, width, height, _generation, allocationBytes);
    }
    public void Invalidate(long contextGeneration)
    {
        VerifyThread(); ObjectDisposedException.ThrowIf(_disposed, this);
        if (contextGeneration <= _generation) throw new InvalidOperationException("Overlay context generation must advance.");
        _generation = contextGeneration;
        foreach (var slot in _idle) Destroy(slot);
        _idle.Clear();
        // Active leases remain owned by their frames and are destroyed when returned.
    }
    public sealed class Lease : IDisposable
    {
        private SkiaPlatformOverlayPool? _owner;
        private readonly int _width, _height;
        private readonly long _generation, _bytes;
        internal Lease(SkiaPlatformOverlayPool owner, SKSurface surface, int width, int height, long generation, long bytes)
        { _owner = owner; Surface = surface; _width = width; _height = height; _generation = generation; _bytes = bytes; }
        public SKSurface Surface { get; }
        public void Dispose()
        {
            if (_owner is not { } owner) return;
            owner.VerifyThread(); _owner = null;
            var slot = new Slot(Surface, _width, _height, _generation, _bytes);
            if (owner._disposed || _generation != owner._generation || owner._idle.Count >= owner._maximumIdle) owner.Destroy(slot);
            else owner._idle.Add(slot);
        }
    }
    public void Dispose()
    {
        VerifyThread(); if (_disposed) return; _disposed = true;
        foreach (var slot in _idle) Destroy(slot);
        _idle.Clear();
    }
    private void Destroy(Slot slot) { slot.Surface.Dispose(); LiveSurfaces--; ReservedBytes -= slot.Bytes; }
    private void VerifyThread()
    {
        if (Environment.CurrentManagedThreadId != _thread) throw new InvalidOperationException("Overlay pool operations belong to the render owner thread.");
    }
}
