using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using SkiaSharp;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Skia.Vulkan;

/// <summary>One immutable publication: Graphite renders private R, then the owner
/// copies R to imported P. Dispose only after the producer fence (or cancellation).
/// The native consumer retains P independently; P is never overwritten or pooled.</summary>
public sealed unsafe class VulkanSharedRaster : IDisposable
{
    private readonly Vk _vk;
    private readonly Device _device;
    private readonly VulkanObserver _observer;
    private readonly uint _family;
    private readonly int _owner = Environment.CurrentManagedThreadId;
    private readonly uint _externalFamily;
    private VkImage _render, _output;
    private DeviceMemory _renderMemory, _outputMemory;
    private SkiaGraphiteSession.VulkanTarget? _target;
    private bool _disposed, _copied;
    private readonly Action? _releaseSource;
    public SKCanvas Canvas => _target!.Canvas;
    public int Width { get; }
    public int Height { get; }

    internal VulkanSharedRaster(Vk vk, Instance instance, PhysicalDevice physical, Device device,
        uint family, VulkanObserver observer, SkiaGraphiteSession session,
        nint sharedHandle, int width, int height, bool android, Action? releaseSource = null)
    {
        _vk = vk; _device = device; _observer = observer; _family = family;
        _externalFamily = android ? uint.MaxValue - 2 : uint.MaxValue - 1;
        Width = width; Height = height;
        _releaseSource = releaseSource;
        try
        {
            if (sharedHandle == 0 || width <= 0 || height <= 0 || width > 16384 || height > 16384 ||
                (long)width * height * 8 > 256L * 1024 * 1024)
                throw new ArgumentOutOfRangeException(nameof(width));
            var format = android ? Format.R8G8B8A8Unorm : Format.B8G8R8A8Unorm;
            var info = new ImageCreateInfo { SType = StructureType.ImageCreateInfo,
                ImageType = ImageType.Type2D, Format = format, Extent = new((uint)width, (uint)height, 1),
                MipLevels = 1, ArrayLayers = 1, Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal,
                SharingMode = SharingMode.Exclusive, Usage = ImageUsageFlags.ColorAttachmentBit |
                    ImageUsageFlags.InputAttachmentBit | ImageUsageFlags.SampledBit |
                    ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit };
            Check(vk.CreateImage(device, &info, null, out _render), "private raster image");
            vk.GetImageMemoryRequirements(device, _render, out var requirements);
            Allocate(physical, requirements.Size, requirements.MemoryTypeBits, null, out _renderMemory);
            Check(vk.BindImageMemory(device, _render, _renderMemory, 0), "private raster bind");
            observer.RegisterHostTarget(_render.Handle, info);
            _target = session.CreateVulkanTarget(width, height, new SKGraphiteVkTextureInfo {
                SampleCount = 1, Format = (int)format, ImageTiling = (int)info.Tiling,
                ImageUsageFlags = (uint)info.Usage, AspectMask = (uint)ImageAspectFlags.ColorBit },
                (int)ImageLayout.Undefined, family, (nint)_render.Handle,
                android ? SKColorType.Rgba8888 : SKColorType.Bgra8888);
            var external = new ExternalMemoryImageCreateInfo { SType = StructureType.ExternalMemoryImageCreateInfo,
                HandleTypes = android ? ExternalMemoryHandleTypeFlags.AndroidHardwareBufferBitAndroid : ExternalMemoryHandleTypeFlags.D3D11TextureKmtBit };
            info.PNext = &external;
            info.Usage = ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit | ImageUsageFlags.ColorAttachmentBit;
            Check(vk.CreateImage(device, &info, null, out _output), "shared raster image");
            var dedicated = new MemoryDedicatedAllocateInfo { SType = StructureType.MemoryDedicatedAllocateInfo, Image = _output };
            if (android)
            {
                // AHB requirements cannot be queried on an unbound image. The
                // buffer supplies the allocation size and compatible memory bits.
                var get = (delegate* unmanaged[Cdecl]<nint, nint, AndroidHardwareBufferPropertiesANDROID*, Result>)
                    (nint)vk.GetDeviceProcAddr(device, "vkGetAndroidHardwareBufferPropertiesANDROID");
                if (get == null) throw new PlatformNotSupportedException("Vulkan AHardwareBuffer import is unavailable.");
                var properties = new AndroidHardwareBufferPropertiesANDROID { SType = StructureType.AndroidHardwareBufferPropertiesAndroid };
                Check(get(device.Handle, sharedHandle, &properties), "hardware buffer properties");
                var import = new ImportAndroidHardwareBufferInfoANDROID { SType = StructureType.ImportAndroidHardwareBufferInfoAndroid,
                    PNext = &dedicated, Buffer = (nint*)sharedHandle };
                Allocate(physical, properties.AllocationSize, properties.MemoryTypeBits, &import, out _outputMemory);
            }
            else
            {
                vk.GetImageMemoryRequirements(device, _output, out requirements);
                if (!vk.TryGetDeviceExtension<KhrExternalMemoryWin32>(instance, device, out var api))
                    throw new PlatformNotSupportedException("Vulkan Win32 memory import is unavailable.");
                using (api)
                {
                    var properties = new MemoryWin32HandlePropertiesKHR { SType = StructureType.MemoryWin32HandlePropertiesKhr };
                    Check(api.GetMemoryWin32HandleProperties(device, external.HandleTypes, sharedHandle, &properties), "shared texture properties");
                    var import = new ImportMemoryWin32HandleInfoKHR { SType = StructureType.ImportMemoryWin32HandleInfoKhr,
                        PNext = &dedicated, HandleType = external.HandleTypes, Handle = sharedHandle };
                    Allocate(physical, requirements.Size, requirements.MemoryTypeBits & properties.MemoryTypeBits, &import, out _outputMemory);
                }
            }
            Check(vk.BindImageMemory(device, _output, _outputMemory, 0), "shared raster bind");
            // External ownership is tracked by this immutable publication, not Graphite.
            Canvas.Clear(SKColors.Transparent);
        }
        catch { Dispose(); throw; }
    }
    private void Allocate(PhysicalDevice physical, ulong size, uint bits, void* chain, out DeviceMemory memory)
    {
        if (size > 256UL * 1024 * 1024) throw new NotSupportedException("Shared raster allocation exceeds 256 MiB.");
        _vk.GetPhysicalDeviceMemoryProperties(physical, out var properties);
        uint type = uint.MaxValue;
        for (uint i = 0; i < properties.MemoryTypeCount; i++)
            if ((bits & (1u << (int)i)) != 0 && (properties.MemoryTypes[(int)i].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) != 0)
            { type = i; break; }
        if (type == uint.MaxValue) throw new PlatformNotSupportedException("No compatible shared raster memory.");
        var allocation = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo, PNext = chain,
            AllocationSize = size, MemoryTypeIndex = type };
        Check(_vk.AllocateMemory(_device, &allocation, null, out memory), "shared raster allocation");
    }
    /// <summary>After Graphite Submit, in the same queue's fenced copy submission.</summary>
    public void RecordCopy(CommandBuffer command)
    {
        if (_disposed || _copied || Environment.CurrentManagedThreadId != _owner)
            throw new InvalidOperationException("Shared raster must be published exactly once on its GPU owner.");
        var state = _target!.GetState();
        var barriers = stackalloc ImageMemoryBarrier[2];
        barriers[0] = Transition(_render, (ImageLayout)state.Layout, ImageLayout.TransferSrcOptimal,
            uint.MaxValue, uint.MaxValue, AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit, AccessFlags.TransferReadBit);
        barriers[1] = Transition(_output, ImageLayout.Undefined, ImageLayout.TransferDstOptimal,
            _externalFamily, _family, 0, AccessFlags.TransferWriteBit);
        var barrier = _observer.Call<VulkanObserver.CmdPipelineBarrierDelegate>("vkCmdPipelineBarrier");
        barrier(command, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.AllCommandsBit, 0, 0, null, 0, null, 2, barriers);
        var copy = new ImageCopy { SrcSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1),
            DstSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1), Extent = new((uint)Width, (uint)Height, 1) };
        _vk.CmdCopyImage(command, _render, ImageLayout.TransferSrcOptimal, _output, ImageLayout.TransferDstOptimal, 1, &copy);
        barriers[0] = Transition(_render, ImageLayout.TransferSrcOptimal, (ImageLayout)state.Layout,
            uint.MaxValue, uint.MaxValue, AccessFlags.TransferReadBit, AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit);
        barriers[1] = Transition(_output, ImageLayout.TransferDstOptimal, ImageLayout.General,
            _family, _externalFamily, AccessFlags.TransferWriteBit, 0);
        barrier(command, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.AllCommandsBit, 0, 0, null, 0, null, 2, barriers);
        _copied = true;
    }
    private static ImageMemoryBarrier Transition(VkImage image, ImageLayout from, ImageLayout to,
        uint source, uint destination, AccessFlags read, AccessFlags write) => new() {
        SType = StructureType.ImageMemoryBarrier, Image = image, OldLayout = from, NewLayout = to,
        SrcQueueFamilyIndex = source, DstQueueFamilyIndex = destination, SrcAccessMask = read, DstAccessMask = write,
        SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1) };
    private static void Check(Result result, string operation)
    { if (result != Result.Success) throw new InvalidOperationException($"{operation}: {result}"); }
    public void Dispose()
    {
        if (_disposed) return;
        // VulkanTarget enforces the session's current owner, including the
        // explicitly drained terminal handoff to a host shutdown thread.
        _target?.Dispose();
        if (_render.Handle != 0) { _observer.ForgetHostTarget(_render.Handle); _vk.DestroyImage(_device, _render, null); }
        if (_output.Handle != 0) _vk.DestroyImage(_device, _output, null);
        if (_renderMemory.Handle != 0) _vk.FreeMemory(_device, _renderMemory, null);
        if (_outputMemory.Handle != 0) _vk.FreeMemory(_device, _outputMemory, null);
        _releaseSource?.Invoke();
        _disposed = true;
    }
}
