using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using SkiaSharp;

namespace Doroti.Skia.Vulkan;

public sealed unsafe partial class GraphiteVulkanWindow
{
    private SkiaGraphiteSession.Frame? _externalFrame;
    private bool _externalSubmitted;
    private bool _externalInitialized;
    private const uint ExternalFamily = uint.MaxValue - 1;

    /// <summary>A Vulkan owner on the exact D3D12 adapter; no presentation surface is created.</summary>
    public static GraphiteVulkanWindow CreateD3D12(long adapterLuid) =>
        CreateD3D12(adapterLuid, GraphiteNativeLibrary.GetPackagedAsset());

    /// <summary>Official asset selection. R remains private; only P is imported from D3D12.</summary>
    public static GraphiteVulkanWindow CreateD3D12(long adapterLuid, GraphiteNativeAsset asset)
    {
        if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException();
        GraphiteNativeLibrary.Configure(asset);
        var vk = Vk.GetApi();
        var instance = CreateInstance(vk, [], Api12);
        try { return new(vk, instance, default, [], true, false, adapterLuid); }
        catch { vk.DestroyInstance(instance, null); vk.Dispose(); throw; }
    }

    /// <summary>Caller has drained D3D copies and keeps the resource in COMMON until the next FlushD3D12Frame.</summary>
    public void ImportD3D12Resource(nint sharedHandle, int width, int height)
    {
        CheckOwner();
        ReleaseD3D12Frame();
        Check(_vk.DeviceWaitIdle(_device), "shared image resize drain");
        ReleaseImages();
        const ImageUsageFlags usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.InputAttachmentBit |
            ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit;
        var external = new ExternalMemoryImageCreateInfo { SType = StructureType.ExternalMemoryImageCreateInfo,
            HandleTypes = ExternalMemoryHandleTypeFlags.D3D12ResourceBit };
        var image = new ImageCreateInfo { SType = StructureType.ImageCreateInfo, PNext = &external,
            ImageType = ImageType.Type2D, Format = Format.R8G8B8A8Unorm, Extent = new((uint)width, (uint)height, 1),
            MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal,
            Usage = usage, SharingMode = SharingMode.Exclusive, InitialLayout = ImageLayout.Undefined };
        Check(_vk.CreateImage(_device, &image, null, out _backing), "D3D12 import image");
        _vk.GetImageMemoryRequirements(_device, _backing, out var requirements);
        if (!_vk.TryGetDeviceExtension<KhrExternalMemoryWin32>(_instance, _device, out var api))
            throw new PlatformNotSupportedException("Vulkan D3D12 memory import is unavailable.");
        using (api)
        {
            var properties = new MemoryWin32HandlePropertiesKHR { SType = StructureType.MemoryWin32HandlePropertiesKhr };
            Check(api.GetMemoryWin32HandleProperties(_device, ExternalMemoryHandleTypeFlags.D3D12ResourceBit, sharedHandle, &properties), "D3D12 memory properties");
            var bits = properties.MemoryTypeBits & requirements.MemoryTypeBits;
            _vk.GetPhysicalDeviceMemoryProperties(_physical, out var memory);
            uint type = uint.MaxValue;
            for (uint i = 0; i < memory.MemoryTypeCount; i++)
                if ((bits & (1u << (int)i)) != 0 && (memory.MemoryTypes[(int)i].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) != 0) { type = i; break; }
            if (type == uint.MaxValue || requirements.Size > 512UL * 1024 * 1024)
                throw new PlatformNotSupportedException("D3D12 import has no compatible memory or exceeds the image budget.");
            var dedicated = new MemoryDedicatedAllocateInfo { SType = StructureType.MemoryDedicatedAllocateInfo, Image = _backing };
            var import = new ImportMemoryWin32HandleInfoKHR { SType = StructureType.ImportMemoryWin32HandleInfoKhr,
                PNext = &dedicated, HandleType = ExternalMemoryHandleTypeFlags.D3D12ResourceBit, Handle = sharedHandle };
            var allocation = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, PNext = &import,
                AllocationSize = requirements.Size, MemoryTypeIndex = type };
            Check(_vk.AllocateMemory(_device, &allocation, null, out _memory), "D3D12 import memory");
        }
        Check(_vk.BindImageMemory(_device, _backing, _memory, 0), "D3D12 image bind");
        Width = width; Height = height; Generation++; _externalInitialized = false;
        CreateIntermediateTarget(width, height, usage);
    }

    public SKSurface BeginD3D12Frame()
    {
        CheckOwner();
        ReleaseD3D12Frame();
        _externalFrame = _session!.BeginVulkanFrame(_target!);
        return _externalFrame.Surface;
    }

    public void FlushD3D12Frame()
    {
        CheckOwner();
        if (_externalFrame is null) throw new InvalidOperationException("No recording frame to flush.");
        _externalSubmitted = true;
        _externalFrame.Submit();
        CopyIntermediateTargetToD3D12();
    }

    public void ReleaseD3D12Frame()
    {
        if (_externalFrame is null) return;
        if (_externalSubmitted)
        {
            var result = _vk.DeviceWaitIdle(_device);
            if (result == Result.ErrorDeviceLost) _session!.NotifyVulkanDeviceLost();
            else Check(result, "D3D12 frame drain");
            _externalFrame.CompleteGpuWork();
        }
        else
        {
            _externalFrame.CancelRecording();
        }
        _externalFrame = null; _externalSubmitted = false;
    }

}
