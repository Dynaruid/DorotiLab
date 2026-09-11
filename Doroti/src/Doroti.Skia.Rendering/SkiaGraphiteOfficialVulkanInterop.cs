using SkiaSharp;

namespace Doroti.Skia.Rendering;

/// <summary>
/// Public SkiaSharp Vulkan entry point. The host supplies observed, successfully
/// submitted image state and owns all Vulkan barriers, semaphore submissions and
/// completion fences. The selected official native asset must already be loaded.
/// No enabled-feature descriptor is inferred or fabricated for Skia.
/// </summary>
public sealed record SkiaGraphiteOfficialVulkanOptions(
    nint Instance, nint PhysicalDevice, nint Device, nint Queue,
    uint QueueFamily, uint MaxApiVersion,
    Func<string, nint, nint, nint> GetProcedure,
    Func<nint, (int Layout, uint QueueFamily)> GetSubmittedImageState,
    Action CheckHostState);

internal sealed class SkiaGraphiteOfficialVulkanInterop : SkiaGraphiteVulkanBinding
{
    private readonly SkiaGraphiteOfficialVulkanOptions _options;
    private readonly Dictionary<nint, nint> _targets = [];
    private Exception? _dispatchFailure;
    private bool _disposed, _hostLost;
    internal override SKGraphiteContext Context { get; }

    internal SkiaGraphiteOfficialVulkanInterop(SkiaGraphiteOfficialVulkanOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(options.GetProcedure);
        ArgumentNullException.ThrowIfNull(options.GetSubmittedImageState);
        ArgumentNullException.ThrowIfNull(options.CheckHostState);
        _options = options;
        if (!SKGraphiteContext.IsBackendAvailable(SKGraphiteBackend.Vulkan))
            throw new PlatformNotSupportedException("Selected Skia asset has no Graphite Vulkan backend.");
        options.CheckHostState();
        var unavailableProcedures = new List<string>();
        using var backend = new SKGraphiteVkBackendContext
        {
            VkInstance = options.Instance, VkPhysicalDevice = options.PhysicalDevice, VkDevice = options.Device,
            VkQueue = options.Queue, GraphicsQueueIndex = options.QueueFamily, MaxApiVersion = options.MaxApiVersion,
            GetProcedureAddress = (name, instance, device) =>
            {
                try
                {
                    var pointer = options.GetProcedure(name, instance, device);
                    if (pointer == 0 && unavailableProcedures.Count < 32) unavailableProcedures.Add(name);
                    return pointer;
                }
                catch (Exception e) { _dispatchFailure = e; return 0; }
            }
        };
        var context = SKGraphiteContext.CreateVulkan(backend, new SKGraphiteContextOptions { GpuBudgetInBytes = SkiaGraphiteSession.ContextBudgetBytes });
        if (context is null)
        {
            options.CheckHostState();
            throw new InvalidOperationException("Official Graphite Vulkan context creation failed. Unavailable procedures: " +
                string.Join(", ", unavailableProcedures), _dispatchFailure);
        }
        Context = context;
        try { CheckHostState(); }
        catch { Context.Dispose(); throw; }
    }
    internal override void CheckHostState()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_dispatchFailure != null) throw new InvalidOperationException("Vulkan procedure callback failed.", _dispatchFailure);
        if (_hostLost) throw new InvalidOperationException("Host observed Vulkan device loss; generation is permanently faulted.");
        _options.CheckHostState();
    }
    internal override bool Insert(nint context, nint recording, ReadOnlySpan<ulong> waits, ReadOnlySpan<ulong> signals)
    {
        CheckHostState();
        if (context != Context.Handle) throw new InvalidOperationException("Context identity mismatch.");
        if (!waits.IsEmpty || !signals.IsEmpty)
            throw new NotSupportedException("Official Graphite frames require host-owned same-queue semaphore submissions.");
        var result = Context.InsertRecording(new SKGraphiteInsertRecordingInfo { Recording = recording });
        CheckHostState();
        return result == SKGraphiteInsertStatus.Success;
    }
    internal override void TrackTarget(nint texture, nint image, int layout, uint family)
    {
        CheckHostState();
        var state = _options.GetSubmittedImageState(image);
        if (state.QueueFamily != _options.QueueFamily || state.QueueFamily != family || state.Layout != layout)
            throw new InvalidOperationException("Target wrap state must match its observed initial layout and Graphite queue family.");
        _targets.Add(texture, image);
    }
    internal override void UntrackTarget(nint texture) => _targets.Remove(texture);
    internal override bool GetState(nint texture, out int layout, out uint family)
    {
        CheckHostState();
        (layout, family) = _options.GetSubmittedImageState(_targets[texture]);
        return true;
    }
    internal override bool SetState(nint texture, int layout, uint family)
    {
        // The public API cannot mutate Skia's private state. The host must actually
        // restore the GPU image to its observed pre-copy layout. Verify only.
        GetState(texture, out var actualLayout, out var actualFamily);
        return actualLayout == layout && actualFamily == family;
    }
    internal override bool ReportDeviceLost(nint context)
    {
        if (context != Context.Handle) return false;
        _hostLost = true; // Do not claim that public Context.IsDeviceLost changed.
        return true;
    }
    public override void Dispose()
    {
        if (_disposed) return;
        if (_targets.Count != 0) throw new InvalidOperationException("Return persistent targets before disposing their official context.");
        Context.Dispose(); // Owner must have established actual GPU completion/loss.
        _disposed = true;
    }
}
