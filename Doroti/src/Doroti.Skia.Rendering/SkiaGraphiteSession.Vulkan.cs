using Doroti.Skia.RuntimeEffects;
using SkiaSharp;

namespace Doroti.Skia.Rendering;

public sealed partial class SkiaGraphiteSession
{
    private SkiaGraphiteVulkanInterop? _vulkanOwner;
    private readonly HashSet<VulkanTarget> _vulkanTargets = [];

    /// <summary>
    /// Terminal handoff only: the host has joined the render owner and established
    /// device idleness or VK_ERROR_DEVICE_LOST. Admission stays permanently closed.
    /// This must never be called while that owner or any GPU submission can race.
    /// </summary>
    public void TakeVulkanShutdownOwnershipAfterGpuDrain()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_vulkanOwner is null) throw new InvalidOperationException("Shutdown handoff requires a Vulkan session.");
        _stopping = true;
        _ownerThread = Environment.CurrentManagedThreadId;
    }

    /// <summary>Only after the host observes VK_ERROR_DEVICE_LOST, never for a timeout.</summary>
    public void NotifyVulkanDeviceLost()
    {
        CheckOwner();
        if (_vulkanOwner is null) throw new InvalidOperationException("This is not a Vulkan session.");
        _stopping = _faulted = true;
        if (!_vulkanOwner.ReportDeviceLost(_context.Handle))
            throw new InvalidOperationException("Could not forward external Vulkan device loss to Graphite.");
    }

    public static SkiaGraphiteSession CreateVulkan(SkiaGraphiteVulkanOptions options, long generation, int maxFrames = 3)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(generation, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxFrames, 1);
        var owner = new SkiaGraphiteVulkanInterop(options);
        try { return new(owner.Context, generation, maxFrames) { _vulkanOwner = owner }; }
        catch { owner.Dispose(); throw; }
    }

    /// <summary>Wrap once per host image generation, retaining the real image until target disposal.</summary>
    public VulkanTarget CreateVulkanTarget(int width, int height, SKGraphiteVkTextureInfo info,
        int layout, uint queueFamily, nint image, SKColorType colorType)
    {
        CheckOwner();
        if (_vulkanOwner is null || _stopping || _faulted)
            throw new InvalidOperationException("A live Vulkan session is required.");
        ArgumentOutOfRangeException.ThrowIfLessThan(width, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(height, 1);
        if (image == 0) throw new ArgumentException("A live Vulkan image is required.", nameof(image));
        var backend = SKGraphiteBackendTexture.CreateVulkan(width, height, info, layout, queueFamily, image)
            ?? throw new InvalidOperationException("Graphite Vulkan texture wrapping failed.");
        try
        {
            var surface = SKSurface.Create(_recorder, backend, colorType)
                ?? throw new InvalidOperationException("Graphite Vulkan surface creation failed; verify format and INPUT_ATTACHMENT usage.");
            var target = new VulkanTarget(this, backend, SkiaGpuSurfaces.Register(surface, _recorder));
            _vulkanTargets.Add(target);
            return target;
        }
        catch { backend.Dispose(); throw; }
    }

    public Frame BeginVulkanFrame(VulkanTarget target)
    {
        CheckOwner();
        ArgumentNullException.ThrowIfNull(target);
        if (!_vulkanTargets.Contains(target) || target.ActiveFrame is not null || !CanBeginFrame)
            throw new InvalidOperationException("Vulkan target belongs to another generation, is busy, or the session cannot admit a frame.");
        if (_context.IsDeviceLost)
        {
            _faulted = true;
            throw new InvalidOperationException("Graphite Vulkan device lost.");
        }
        _context.CheckAsyncWorkCompletion();
        var frame = new Frame(this, target.Backend, target.Surface, target);
        target.ActiveFrame = frame;
        _recordingFrame = frame;
        _frames.Add(frame);
        return frame;
    }

    public sealed class VulkanTarget : IDisposable
    {
        private readonly SkiaGraphiteSession _session;
        internal SKGraphiteBackendTexture Backend { get; }
        internal SKSurface Surface { get; }
        internal Frame? ActiveFrame { get; set; }
        private bool _disposed;

        internal VulkanTarget(SkiaGraphiteSession session, SKGraphiteBackendTexture backend, SKSurface surface)
        { _session = session; Backend = backend; Surface = surface; }

        public long Generation => _session.Generation;

        /// <summary>Scheduled state for external barriers; this is not a GPU completion receipt.</summary>
        public (int Layout, uint QueueFamily) GetState()
        {
            _session.CheckOwner();
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (!_session._vulkanOwner!.GetState(Backend.Handle, out var layout, out var family))
                throw new InvalidOperationException("Cannot query Graphite Vulkan texture state.");
            return (layout, family);
        }

        /// <summary>Publish a completed external barrier after returning the active frame.</summary>
        public void SetStateAfterGpuCompletion(int layout, uint queueFamily)
        {
            _session.CheckOwner();
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (ActiveFrame is not null)
                throw new InvalidOperationException("Complete GPU work before updating the persistent target state.");
            if (!_session._vulkanOwner!.SetState(Backend.Handle, layout, queueFamily))
                throw new InvalidOperationException("Cannot update Graphite Vulkan texture state.");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _session.CheckOwner();
            if (ActiveFrame is not null) throw new InvalidOperationException("Return the Vulkan frame before disposing its target.");
            Surface.Dispose();
            Backend.Dispose();
            _session._vulkanTargets.Remove(this);
            _disposed = true;
        }
    }
}
