using System.Diagnostics;
using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using VkImage = Silk.NET.Vulkan.Image;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace Doroti.Skia.Vulkan;

public sealed unsafe partial class GraphiteVulkanWindow
{
    private const int WindowFrameLimit = 2;
    private readonly List<WindowFrameSlot> _windowFrames = [];
    private int _nextWindowFrame;
    public long SubmittedWindowFrames { get; private set; }
    public long CompletedWindowFrames { get; private set; }
    public long BusyWindowFrames { get; private set; }
    public long UnavailableWindowImages { get; private set; }
    public int MaximumWindowFramesInFlight { get; private set; }
    public int WindowFramesInFlight => _windowFrames.Count(slot => slot.Frame is not null);

    private sealed class WindowFrameSlot
    {
        internal CommandBuffer Command;
        internal Fence Fence;
        internal VkSemaphore Acquired;
        internal VkImage Backing;
        internal DeviceMemory Memory;
        internal SkiaGraphiteSession.VulkanTarget? Target;
        internal SkiaGraphiteSession.Frame? Frame;
        internal bool AcquireWaitPending;
        internal long SubmittedAt;
        internal ImageLayout RestoredLayout = ImageLayout.ColorAttachmentOptimal;
    }

    private void CreateWindowFrameSlots()
    {
        for (var i = 0; i < (_pipelinedWindowFrames ? WindowFrameLimit : 1); i++)
        {
            var slot = new WindowFrameSlot();
            _windowFrames.Add(slot); // Retain partial construction for cleanup.
            var allocation = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = _pool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
            Check(_vk.AllocateCommandBuffers(_device, &allocation, out slot.Command), "frame command buffer");
            _stockObserver?.Journal.Allocate(slot.Command.Handle, _pool.Handle);
            var fence = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(_vk.CreateFence(_device, &fence, null, out slot.Fence), "frame fence");
            var semaphore = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
            Check(_vk.CreateSemaphore(_device, &semaphore, null, out slot.Acquired), "acquire semaphore");
        }
    }

    /// <summary>Owner-thread, nonblocking retirement. Also call after the last presentation while idle.</summary>
    public bool PollGpuWork()
    {
        CheckOwner();
        var complete = true;
        foreach (var slot in _windowFrames)
        {
            if (slot.Frame is null) continue;
            var result = _vk.GetFenceStatus(_device, slot.Fence);
            if (result == Result.NotReady)
            {
                if (Stopwatch.GetElapsedTime(slot.SubmittedAt) >= TimeSpan.FromSeconds(5))
                    throw new TimeoutException("Vulkan frame completion exceeded five seconds.");
                complete = false;
                continue;
            }
            CheckDevice(result, "poll frame fence");
            CompleteWindowFrame(slot);
        }
        return complete;
    }

    private void CompleteWindowFrame(WindowFrameSlot slot)
    {
        if (slot.Frame is null) return;
        slot.Frame.CompleteGpuWork();
        slot.Frame = null;
        if (_session?.IsDeviceLost != true)
            slot.Target!.SetStateAfterGpuCompletion((int)slot.RestoredLayout, _family);
        CompletedWindowFrames++;
    }

    private void DrainWindowFrames()
    {
        var idle = _vk.DeviceWaitIdle(_device);
        if (idle == Result.ErrorDeviceLost) _session?.NotifyVulkanDeviceLost();
        else Check(idle, "window frame drain");
        foreach (var slot in _windowFrames)
        {
            CompleteWindowFrame(slot);
            if (!slot.AcquireWaitPending) continue;
            if (idle != Result.ErrorDeviceLost)
            {
                // A cancelled/failed paint may have acquired an image without
                // submitting its copy. Consume that signal before destroying
                // or reusing the semaphore, including an asynchronous acquire.
                var acquired = slot.Acquired;
                var stage = PipelineStageFlags.AllCommandsBit;
                var submit = new SubmitInfo { SType = StructureType.SubmitInfo,
                    WaitSemaphoreCount = 1, PWaitSemaphores = &acquired, PWaitDstStageMask = &stage };
                CheckDevice(_vk.ResetFences(_device, 1, in slot.Fence), "reset abandoned acquire fence");
                CheckDevice(_vk.QueueSubmit(_queue, 1, &submit, slot.Fence), "consume abandoned acquire");
                CheckDevice(_vk.WaitForFences(_device, 1, in slot.Fence, true, Timeout), "abandoned acquire fence");
            }
            slot.AcquireWaitPending = false;
        }
    }

    private void CheckDevice(Result result, string operation)
    {
        if (result == Result.ErrorDeviceLost) _session?.NotifyVulkanDeviceLost();
        Check(result, operation);
    }
}
