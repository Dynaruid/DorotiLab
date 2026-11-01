using Silk.NET.Vulkan;

namespace Doroti.Skia.Vulkan;

public sealed unsafe partial class GraphiteVulkanWindow
{
    private bool _androidSurfaceDetaching;
    private bool _androidSurfaceReattached;
    private bool _androidSurfaceDeviceLost;
    private Task? _androidSurfaceDrain;

    public bool HasPresentationSurface => _surface.Handle != 0;

    /// <summary>
    /// Close Android WSI admission and drain submitted GPU work off the UI thread.
    /// The host removes every draw/poll callback first and retains ANativeWindow
    /// until CompleteAndroidSurfaceDetach has run back on this same owner.
    /// The device, recorder and bounded caches remain owned by the live view.
    /// </summary>
    public Task BeginAndroidSurfaceDetachAsync()
    {
        CheckOwner();
        if (!OperatingSystem.IsAndroid() || !_ownsSurface || !_ownsInstance
            || _terminalShutdown || _androidSurfaceDetaching || _surface.Handle == 0)
            throw new InvalidOperationException("No attached Android surface is available for detachment.");
        _androidSurfaceDetaching = true;
        _androidSurfaceDeviceLost = false;
        return _androidSurfaceDrain = Task.Run(DrainAndroidSurfaceNativeWork);
    }

    private void DrainAndroidSurfaceNativeWork()
    {
        if (!CheckAndroidSurfaceDrain(_vk.DeviceWaitIdle(_device), "Android surface detach drain")) return;
        // A cancelled recording can leave a successfully acquired WSI image's
        // semaphore unconsumed. Retire that native wait on the worker too. No
        // Graphite callback, recorder or observer ownership moves to this thread.
        foreach (var slot in _windowFrames)
        {
            if (!slot.AcquireWaitPending) continue;
            var acquired = slot.Acquired;
            var stage = PipelineStageFlags.AllCommandsBit;
            var submit = new SubmitInfo
            {
                SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 1,
                PWaitSemaphores = &acquired,
                PWaitDstStageMask = &stage,
            };
            if (!CheckAndroidSurfaceDrain(_vk.ResetFences(_device, 1, in slot.Fence), "reset detached acquire fence")) return;
            if (!CheckAndroidSurfaceDrain(_vk.QueueSubmit(_queue, 1, &submit, slot.Fence), "consume detached acquire")) return;
            if (!CheckAndroidSurfaceDrain(_vk.WaitForFences(_device, 1, in slot.Fence, true, Timeout), "detached acquire fence")) return;
            slot.AcquireWaitPending = false;
        }
    }

    private bool CheckAndroidSurfaceDrain(Result result, string operation)
    {
        if (result == Result.ErrorDeviceLost)
        {
            // Publish only the native result here. Graphite's owner acknowledges
            // it after joining this task, then the host performs terminal cleanup.
            _androidSurfaceDeviceLost = true;
            return false;
        }
        Check(result, operation);
        return true;
    }

    /// <summary>Owner only, after the exact drain task completes successfully.</summary>
    public bool CompleteAndroidSurfaceDetach()
    {
        CheckOwner();
        if (!_androidSurfaceDetaching || _terminalShutdown || _androidSurfaceDrain?.IsCompletedSuccessfully != true)
            throw new InvalidOperationException("Android surface detach drain has not completed successfully.");
        // The worker established device idleness. Retire native recordings on
        // their original owner; never transfer a reusable Graphite recorder.
        if (_androidSurfaceDeviceLost) _session!.NotifyVulkanDeviceLost();
        foreach (var slot in _windowFrames)
        {
            CompleteWindowFrame(slot);
            if (_androidSurfaceDeviceLost) slot.AcquireWaitPending = false;
        }
        ReleaseTextureInputs(completed: true);
        ReleaseImages(invalidateHostResources: false);
        _surfaces.DestroySurface(_instance, _surface, null);
        _surface = default;
        _androidSurfaceDetaching = false;
        _androidSurfaceDrain = null;
        _recreate = true;
        return !_androidSurfaceDeviceLost;
    }

    /// <summary>Bind a replacement SurfaceView to this view's existing device/context.</summary>
    public void AttachAndroidSurface(nint nativeWindow)
    {
        CheckOwner();
        if (!OperatingSystem.IsAndroid() || !_ownsSurface || !_ownsInstance
            || _terminalShutdown || _androidSurfaceDetaching || _surface.Handle != 0 || nativeWindow == 0)
            throw new InvalidOperationException("Android surface cannot be attached in the current state.");
        var create = (delegate* unmanaged[Cdecl]<nint, AndroidSurfaceInfo*, void*, ulong*, Result>)
            (nint)_vk.GetInstanceProcAddr(_instance, "vkCreateAndroidSurfaceKHR");
        if (create == null) throw new PlatformNotSupportedException("Vulkan Android surface extension is unavailable.");
        var info = new AndroidSurfaceInfo { Type = 1000008000, Window = nativeWindow };
        ulong handle = 0;
        Check(create(_instance.Handle, &info, null, &handle), "reattach Android surface");
        var surface = new SurfaceKHR(handle);
        try
        {
            Check(_surfaces.GetPhysicalDeviceSurfaceSupport(_physical, _family, surface, out var supported),
                "replacement Android present support");
            if (!supported) throw new PlatformNotSupportedException("Replacement Android surface requires a different GPU queue.");
        }
        catch
        {
            _surfaces.DestroySurface(_instance, surface, null);
            throw;
        }
        _surface = surface;
        _androidSurfaceReattached = true;
        _recreate = true;
    }

    public void PurgeUnusedGpuResources()
    {
        CheckOwner();
        if (_terminalShutdown || _androidSurfaceDetaching)
            throw new InvalidOperationException("Vulkan context is draining.");
        _session!.PurgeUnusedGpuResources();
    }
}
