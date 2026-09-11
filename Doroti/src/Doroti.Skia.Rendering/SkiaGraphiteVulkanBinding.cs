using SkiaSharp;

namespace Doroti.Skia.Rendering;

// Vulkan host state/completion contract implemented using the public SkiaSharp API.
internal abstract class SkiaGraphiteVulkanBinding : IDisposable
{
    internal abstract SKGraphiteContext Context { get; }
    internal abstract bool Insert(nint context, nint recording, ReadOnlySpan<ulong> waits, ReadOnlySpan<ulong> signals);
    internal abstract bool ReportDeviceLost(nint context);
    internal abstract bool SetState(nint texture, int layout, uint family);
    internal abstract bool GetState(nint texture, out int layout, out uint family);
    internal virtual void TrackTarget(nint texture, nint image, int layout, uint family) { }
    internal virtual void UntrackTarget(nint texture) { }
    internal virtual void CheckHostState() { }
    public abstract void Dispose();
}
