using System.Diagnostics;
using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Skia.Vulkan;

public sealed unsafe partial class GraphiteVulkanWindow
{
    private readonly ExternalSlot[] _externalSlots = [new(), new()];
    private int _externalSlot;
    private bool _externalSlotsEnabled;
    private long _externalSubmittedAt;
    private sealed class ExternalSlot
    {
        internal VkImage Backing;
        internal DeviceMemory Memory;
        internal VkImage IntermediateTarget;
        internal DeviceMemory IntermediateTargetMemory;
        internal SkiaGraphiteSession.VulkanTarget? Target;
        internal SkiaGraphiteSession.Frame? ExternalFrame;
        internal bool ExternalSubmitted;
        internal bool ExternalInitialized;
        internal CommandBuffer Command;
        internal Fence Fence;
        internal int Width;
        internal int Height;
        internal long SubmittedAt;
    }

    private void SaveExternalSlot()
    {
        var slot = _externalSlots[_externalSlot];
        slot.Backing = _backing;
        slot.Memory = _memory;
        slot.IntermediateTarget = _intermediateTarget;
        slot.IntermediateTargetMemory = _intermediateTargetMemory;
        slot.Target = _target;
        slot.ExternalFrame = _externalFrame;
        slot.ExternalSubmitted = _externalSubmitted;
        slot.ExternalInitialized = _externalInitialized;
        slot.Command = _command;
        slot.Fence = _fence;
        slot.Width = Width;
        slot.Height = Height;
        slot.SubmittedAt = _externalSubmittedAt;
    }

    /// <summary>Two isolated R/P targets on the existing device, queue and recorder.
    /// The caller must establish the previous D3D consumer completion before reusing a slot.</summary>
    public void SelectD3D12Slot(int index) => SelectExternalSlot(index, allocate: true);

    private void SelectExternalSlot(int index, bool allocate)
    {
        CheckOwner();
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _externalSlots.Length);
        _externalSlotsEnabled = true;
        if (index == _externalSlot) return;
        SaveExternalSlot();
        _externalSlot = index;
        var slot = _externalSlots[index];
        _backing = slot.Backing;
        _memory = slot.Memory;
        _intermediateTarget = slot.IntermediateTarget;
        _intermediateTargetMemory = slot.IntermediateTargetMemory;
        _target = slot.Target;
        _externalFrame = slot.ExternalFrame;
        _externalSubmitted = slot.ExternalSubmitted;
        _externalInitialized = slot.ExternalInitialized;
        _command = slot.Command;
        _fence = slot.Fence;
        Width = slot.Width;
        Height = slot.Height;
        _externalSubmittedAt = slot.SubmittedAt;
        if (allocate && _command.Handle == 0)
        {
            var info = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo, CommandPool = _pool,
                Level = CommandBufferLevel.Primary, CommandBufferCount = 1,
            };
            Check(_vk.AllocateCommandBuffers(_device, &info, out _command), "external slot command");
            _stockObserver?.Journal.Allocate(_command.Handle, _pool.Handle);
            var fence = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(_vk.CreateFence(_device, &fence, null, out _fence), "external slot fence");
        }
    }

    public bool PollD3D12Slot()
    {
        CheckOwner();
        if (_externalFrame is null) return true;
        if (!_externalSubmitted) return false;
        var result = _vk.GetFenceStatus(_device, _fence);
        if (result == Result.NotReady)
        {
            if (Stopwatch.GetElapsedTime(_externalSubmittedAt) > TimeSpan.FromSeconds(5))
                throw new TimeoutException("External Vulkan copy retirement exceeded five seconds; resources remain held.");
            return false;
        }
        CheckDevice(result, "external slot copy completion");
        _externalFrame.CompleteGpuWork();
        _externalFrame = null;
        _externalSubmitted = false;
        var state = _target!.GetState();
        _target.SetStateAfterGpuCompletion(state.Layout, state.QueueFamily);
        _externalInitialized = true;
        return true;
    }

    private void ReleaseExternalSlotsAfterDrain()
    {
        if (!_externalSlotsEnabled) return;
        var current = _externalSlot;
        for (var index = 0; index < _externalSlots.Length; index++)
        {
            SelectExternalSlot(index, allocate: false);
            ReleaseD3D12Frame();
            ReleaseImages();
            if (index != current && _fence.Handle != 0)
            {
                _vk.DestroyFence(_device, _fence, null);
                _fence = default;
            }
        }
        SelectExternalSlot(current, allocate: false);
        _externalSlotsEnabled = false;
    }
}
