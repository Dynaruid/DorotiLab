using System.Diagnostics;
using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using Image = Silk.NET.Vulkan.Image;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter
{
    internal NativeFrameAdmission FrameAdmission { get; set; } = new(false, false, 1, true, "unprepared");
    private readonly PipelineBank _spareBank = new();
    private long _activeCopySubmittedAt;
    private int _pipelineConsumerSlot = -1;
    internal int MaximumPipelineFrames { get; private set; }
    internal int PendingPipelineFrames => (_copySubmissionPending || _pipelineConsumerSlot >= 0 ? 1 : 0)
        + (_spareBank.CopySubmissionPending || _spareBank.ConsumerSlot >= 0 ? 1 : 0);
    internal bool SupportsFramePipeline => _useGraphite;
    private ulong OtherPipelineBytes => _spareBank.BackingAllocationSize + _spareBank.RetainedFrameAllocationSize;

    private sealed class PipelineBank
    {
        internal Image BackingImage;
        internal DeviceMemory BackingMemory;
        internal ulong BackingAllocationSize;
        internal Image RetainedFrameImage;
        internal DeviceMemory RetainedFrameMemory;
        internal ulong RetainedFrameAllocationSize;
        internal ImageLayout RetainedFrameLayout;
        internal bool RetainedFrameInitialized;
        internal Format BackingFormat;
        internal int BackingCapacityWidth;
        internal int BackingCapacityHeight;
        internal CommandBuffer CommandBuffer;
        internal Fence Fence;
        internal bool CopySubmissionPending;
        internal SkiaGraphiteSession.VulkanTarget? GraphiteTarget;
        internal SkiaGraphiteSession.Frame? GraphiteFrame;
        internal bool GraphiteSubmissionAttempted;
        internal ImageLayout GraphiteCopyRestoreLayout;
        internal long SubmittedAt;
        internal int ConsumerSlot = -1;
    }

    private void SwapPipelineBank()
    {
        (_pipelineConsumerSlot, _spareBank.ConsumerSlot) = (_spareBank.ConsumerSlot, _pipelineConsumerSlot);
        (_backingImage, _spareBank.BackingImage) = (_spareBank.BackingImage, _backingImage);
        (_backingMemory, _spareBank.BackingMemory) = (_spareBank.BackingMemory, _backingMemory);
        (_backingAllocationSize, _spareBank.BackingAllocationSize) = (_spareBank.BackingAllocationSize, _backingAllocationSize);
        (_retainedFrameImage, _spareBank.RetainedFrameImage) = (_spareBank.RetainedFrameImage, _retainedFrameImage);
        (_retainedFrameMemory, _spareBank.RetainedFrameMemory) = (_spareBank.RetainedFrameMemory, _retainedFrameMemory);
        (_retainedFrameAllocationSize, _spareBank.RetainedFrameAllocationSize) = (_spareBank.RetainedFrameAllocationSize, _retainedFrameAllocationSize);
        (_retainedFrameLayout, _spareBank.RetainedFrameLayout) = (_spareBank.RetainedFrameLayout, _retainedFrameLayout);
        (_retainedFrameInitialized, _spareBank.RetainedFrameInitialized) = (_spareBank.RetainedFrameInitialized, _retainedFrameInitialized);
        (_backingFormat, _spareBank.BackingFormat) = (_spareBank.BackingFormat, _backingFormat);
        (_backingCapacityWidth, _spareBank.BackingCapacityWidth) = (_spareBank.BackingCapacityWidth, _backingCapacityWidth);
        (_backingCapacityHeight, _spareBank.BackingCapacityHeight) = (_spareBank.BackingCapacityHeight, _backingCapacityHeight);
        (_commandBuffer, _spareBank.CommandBuffer) = (_spareBank.CommandBuffer, _commandBuffer);
        (_fence, _spareBank.Fence) = (_spareBank.Fence, _fence);
        (_copySubmissionPending, _spareBank.CopySubmissionPending) = (_spareBank.CopySubmissionPending, _copySubmissionPending);
        (_graphiteTarget, _spareBank.GraphiteTarget) = (_spareBank.GraphiteTarget, _graphiteTarget);
        (_graphiteFrame, _spareBank.GraphiteFrame) = (_spareBank.GraphiteFrame, _graphiteFrame);
        (_graphiteSubmissionAttempted, _spareBank.GraphiteSubmissionAttempted) = (_spareBank.GraphiteSubmissionAttempted, _graphiteSubmissionAttempted);
        (_graphiteCopyRestoreLayout, _spareBank.GraphiteCopyRestoreLayout) = (_spareBank.GraphiteCopyRestoreLayout, _graphiteCopyRestoreLayout);
        (_activeCopySubmittedAt, _spareBank.SubmittedAt) = (_spareBank.SubmittedAt, _activeCopySubmittedAt);
    }

    private bool PipelineFenceReady(Fence fence, long started)
    {
        var result = _vk.GetFenceStatus(_device, fence);
        if (result == Result.NotReady)
        {
            if (Stopwatch.GetElapsedTime(started) >= TimeSpan.FromSeconds(5))
                throw new TimeoutException("Windows Vulkan copy retirement exceeded five seconds; resources remain held.");
            return false;
        }
        Check(result, "Vulkan pipeline copy completion");
        return true;
    }

    private void RetireSpareFrame()
    {
        if (_spareBank.GraphiteFrame is { } frame)
        {
            frame.CompleteGpuWork();
            _spareBank.GraphiteFrame = null;
            _spareBank.GraphiteSubmissionAttempted = false;
            if (_graphite?.IsDeviceLost != true)
                _spareBank.GraphiteTarget!.SetStateAfterGpuCompletion((int)_spareBank.GraphiteCopyRestoreLayout, _queueFamily);
        }
        _spareBank.CopySubmissionPending = false;
    }

    internal void PollPipelineFrames()
    {
        if (_device.Handle == 0) return;
        if (_copySubmissionPending && PipelineFenceReady(_fence, _activeCopySubmittedAt))
        {
            ReturnGraphiteFrameAfterGpuCompletion();
            _graphiteTarget?.SetStateAfterGpuCompletion((int)_graphiteCopyRestoreLayout, _queueFamily);
            _copySubmissionPending = false;
        }
        if (_spareBank.CopySubmissionPending && PipelineFenceReady(_spareBank.Fence, _spareBank.SubmittedAt))
            RetireSpareFrame();
        bool ConsumerComplete(int slot)
        {
            Marshal.ThrowExceptionForHR(IsCompositionBufferAvailable(_presentationContext, (uint)slot, out var available));
            return available != 0;
        }
        if (!_copySubmissionPending && _pipelineConsumerSlot >= 0 && ConsumerComplete(_pipelineConsumerSlot))
            _pipelineConsumerSlot = -1;
        if (!_spareBank.CopySubmissionPending && _spareBank.ConsumerSlot >= 0 && ConsumerComplete(_spareBank.ConsumerSlot))
            _spareBank.ConsumerSlot = -1;
    }

    private bool TrySelectPipelineBank()
    {
        PollPipelineFrames();
        if (_copySubmissionPending || _pipelineConsumerSlot >= 0)
        {
            if (!FrameAdmission.FreshOnly || _spareBank.CopySubmissionPending || _spareBank.ConsumerSlot >= 0) return false;
            if (_sharedRasters.Count != 0) throw new InvalidOperationException("Native raster cannot change pipeline banks.");
            SwapPipelineBank();
        }
        if (_commandBuffer.Handle == 0)
        {
            var allocation = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo, CommandPool = _commandPool,
                Level = CommandBufferLevel.Primary, CommandBufferCount = 1,
            };
            Check(_vk.AllocateCommandBuffers(_device, &allocation, out _commandBuffer), "pipeline copy command");
            var fence = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(_vk.CreateFence(_device, &fence, null, out _fence), "pipeline copy fence");
        }
        return true;
    }

    // Called only after a successful GPU drain or confirmed device loss, while
    // Graphite and its observer still exist and the caller owns their thread.
    private void ReleaseSparePipelineBankAfterDrain()
    {
        RetireSpareFrame();
        SwapPipelineBank();
        ReleaseBackingSurface();
        ReleaseBackingStorage();
        if (_fence.Handle != 0) _vk.DestroyFence(_device, _fence, null);
        _fence = default;
        if (_commandBuffer.Handle != 0)
        {
            // Both command buffers are freed with their shared command pool.
        }
        _commandBuffer = default;
        SwapPipelineBank();
    }
}
