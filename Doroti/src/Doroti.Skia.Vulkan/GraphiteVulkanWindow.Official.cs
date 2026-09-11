using Silk.NET.Vulkan;
using SkiaSharp;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Skia.Vulkan;

public sealed unsafe partial class GraphiteVulkanWindow
{
    private VkImage _officialIntermediate;
    private DeviceMemory _officialIntermediateMemory;

    private void CreateOfficialIntermediate(int width, int height, ImageUsageFlags usage)
    {
        var info = new ImageCreateInfo { SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D,
            Format = Format.R8G8B8A8Unorm, Extent = new((uint)width, (uint)height, 1), MipLevels = 1, ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal, Usage = usage, SharingMode = SharingMode.Exclusive };
        Check(_vk.CreateImage(_device, &info, null, out _officialIntermediate), "official private image");
        _vk.GetImageMemoryRequirements(_device, _officialIntermediate, out var requirements);
        _vk.GetPhysicalDeviceMemoryProperties(_physical, out var properties);
        uint type = uint.MaxValue;
        for (uint i = 0; i < properties.MemoryTypeCount; i++)
            if ((requirements.MemoryTypeBits & (1u << (int)i)) != 0 && (properties.MemoryTypes[(int)i].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) != 0) { type = i; break; }
        if (type == uint.MaxValue || requirements.Size > 512UL * 1024 * 1024) throw new NotSupportedException("Private Graphite image exceeds memory support/budget.");
        var allocate = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, AllocationSize = requirements.Size, MemoryTypeIndex = type };
        Check(_vk.AllocateMemory(_device, &allocate, null, out _officialIntermediateMemory), "official private allocation");
        Check(_vk.BindImageMemory(_device, _officialIntermediate, _officialIntermediateMemory, 0), "official private bind");
        _stockObserver!.RegisterHostTarget(_officialIntermediate.Handle, info);
        _target = _session!.CreateVulkanTarget(width, height, new SKGraphiteVkTextureInfo {
            SampleCount = 1, Format = (int)info.Format, ImageTiling = (int)info.Tiling,
            ImageUsageFlags = (uint)usage, AspectMask = (uint)ImageAspectFlags.ColorBit },
            (int)ImageLayout.Undefined, _family, (nint)_officialIntermediate.Handle, SKColorType.Rgba8888);
    }
    private void ReleaseOfficialIntermediate()
    {
        if (_officialIntermediate.Handle != 0)
        {
            _stockObserver?.ForgetHostTarget(_officialIntermediate.Handle);
            _vk.DestroyImage(_device, _officialIntermediate, null); _officialIntermediate = default;
        }
        if (_officialIntermediateMemory.Handle != 0)
        { _vk.FreeMemory(_device, _officialIntermediateMemory, null); _officialIntermediateMemory = default; }
    }
    private void CopyOfficialIntermediateToD3D12()
    {
        var observer = _stockObserver!;
        var state = _target!.GetState();
        Check(observer.Call<VulkanObserver.ResetCommandBufferDelegate>("vkResetCommandBuffer")(_command, 0), "official copy reset");
        var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo, Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
        Check(observer.Call<VulkanObserver.BeginCommandBufferDelegate>("vkBeginCommandBuffer")(_command, &begin), "official copy begin");
        var barrier = observer.Call<VulkanObserver.CmdPipelineBarrierDelegate>("vkCmdPipelineBarrier");
        static ImageMemoryBarrier Transition(VkImage image, ImageLayout from, ImageLayout to, uint sourceFamily, uint destinationFamily, AccessFlags sourceAccess, AccessFlags destinationAccess) =>
            new() { SType = StructureType.ImageMemoryBarrier, Image = image, OldLayout = from, NewLayout = to,
                SrcAccessMask = sourceAccess, DstAccessMask = destinationAccess, SrcQueueFamilyIndex = sourceFamily, DstQueueFamilyIndex = destinationFamily,
                SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1) };
        ImageMemoryBarrier* transitions = stackalloc ImageMemoryBarrier[2];
        transitions[0] = Transition(_officialIntermediate, (ImageLayout)state.Layout, ImageLayout.TransferSrcOptimal, uint.MaxValue, uint.MaxValue,
            AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit, AccessFlags.TransferReadBit);
        transitions[1] = Transition(_backing, _externalInitialized ? ImageLayout.General : ImageLayout.Undefined, ImageLayout.TransferDstOptimal, ExternalFamily, _family,
            0, AccessFlags.TransferWriteBit);
        barrier(_command, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.AllCommandsBit, 0, 0, null, 0, null, 2, transitions);
        var copy = new ImageCopy { SrcSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1), DstSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1),
            Extent = new((uint)Width, (uint)Height, 1) };
        _vk.CmdCopyImage(_command, _officialIntermediate, ImageLayout.TransferSrcOptimal, _backing, ImageLayout.TransferDstOptimal, 1, &copy);
        transitions[0] = Transition(_officialIntermediate, ImageLayout.TransferSrcOptimal, (ImageLayout)state.Layout, uint.MaxValue, uint.MaxValue,
            AccessFlags.TransferReadBit, AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit);
        transitions[1] = Transition(_backing, ImageLayout.TransferDstOptimal, ImageLayout.General, _family, ExternalFamily, AccessFlags.TransferWriteBit, 0);
        barrier(_command, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.AllCommandsBit, 0, 0, null, 0, null, 2, transitions);
        Check(observer.Call<VulkanObserver.EndCommandBufferDelegate>("vkEndCommandBuffer")(_command), "official copy end"); observer.Check();
        Check(_vk.ResetFences(_device, 1, in _fence), "official copy reset fence");
        var command = _command;
        var submit = new SubmitInfo { SType = StructureType.SubmitInfo, CommandBufferCount = 1, PCommandBuffers = &command };
        Check(observer.Call<VulkanObserver.QueueSubmitDelegate>("vkQueueSubmit")(_queue, 1, &submit, _fence), "official D3D12 copy submit");
        Check(_vk.WaitForFences(_device, 1, in _fence, true, Timeout), "official D3D12 copy fence");
        observer.Check();
        _externalFrame!.CompleteGpuWork(); _externalFrame = null; _externalSubmitted = false;
        _target.SetStateAfterGpuCompletion(state.Layout, state.QueueFamily); _externalInitialized = true;
    }
}
