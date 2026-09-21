using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Skia.Vulkan;

public sealed record WindowsSharedTextureBuffer(
    int Width,
    int Height,
    NativeTextureFormat Format,
    nint SharedHandle,
    long AdapterLuid
) : NativeTextureBuffer(Width, Height, Format)
{
    public override NativeTexturePlatform Platform => NativeTexturePlatform.Windows;
}

public sealed record LinuxDmaBufTextureBuffer(
    int Width,
    int Height,
    NativeTextureFormat Format,
    int FileDescriptor,
    ulong AllocationSize,
    ulong Offset,
    ulong RowPitch,
    ulong Modifier
) : NativeTextureBuffer(Width, Height, Format)
{
    public override NativeTexturePlatform Platform => NativeTexturePlatform.Linux;
}

/// <summary>Immutable, producer-complete Windows D3D11 KMT or Linux single-plane DMA-BUF input.
/// Imports live in the recording's resource set and return ownership only after GPU retirement.</summary>
internal sealed unsafe class VulkanNativeTextureImporter(
    SkiaGraphiteSession session,
    Vk vk,
    PhysicalDevice physical,
    Device device,
    Queue queue,
    uint family,
    VulkanObserver observer,
    NativeTexturePlatform platform
) : ISkiaNativeTextureImporter
{
    internal static readonly string[] LinuxExtensions =
    [
        "VK_KHR_external_memory_fd",
        "VK_EXT_external_memory_dma_buf",
        "VK_EXT_image_drm_format_modifier",
        "VK_EXT_queue_family_foreign",
    ];

    internal static bool SupportsLinux(Vk api, PhysicalDevice gpu)
    {
        uint count = 0;
        if (
            api.EnumerateDeviceExtensionProperties(gpu, (byte*)null, &count, null) != Result.Success
        )
            return false;
        var properties = new ExtensionProperties[count];
        fixed (ExtensionProperties* data = properties)
            if (
                api.EnumerateDeviceExtensionProperties(gpu, (byte*)null, &count, data)
                != Result.Success
            )
                return false;
        var names = new HashSet<string>();
        foreach (var value in properties)
        {
            var copy = value;
            names.Add(Marshal.PtrToStringUTF8((nint)copy.ExtensionName)!);
        }
        return LinuxExtensions.All(names.Contains);
    }

    private readonly SkiaGraphiteSession _session = session;
    private readonly Vk _vk = vk;
    private readonly PhysicalDevice _physical = physical;
    private readonly Device _device = device;
    private readonly Queue _queue = queue;
    private readonly uint _family = family;
    private readonly VulkanObserver _observer = observer;
    private readonly HashSet<ImportedImage> _live = [];
    private bool _disposed;
    public long Imported { get; private set; }
    public long Retired { get; private set; }

    public SkiaNativeTextureImage Import(NativeTextureFrame frame)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (frame.Buffer.Platform != platform)
            throw new PlatformNotSupportedException("Native GPU buffer platform mismatch.");
        var result = new ImportedImage(this, frame.Retain());
        _live.Add(result);
        try
        {
            result.Initialize();
            Imported++;
            return result;
        }
        catch
        {
            // Failed initialization remains rooted if drain/release itself cannot be proven.
            result.Dispose();
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        foreach (var image in _live.ToArray())
            image.Dispose();
        _disposed = true;
        Console.WriteLine(
            $"doroti.texture.importer platform={platform} imported={Imported} retired={Retired} live={_live.Count}"
        );
    }

    private sealed class ImportedImage(VulkanNativeTextureImporter owner, NativeTextureFrame source)
        : SkiaNativeTextureImage
    {
        private VkImage _image;
        private DeviceMemory _memory;
        private CommandPool _pool;
        private CommandBuffer _command;
        private Fence _fence;
        private SkiaGraphiteSession.VulkanImage? _wrapped;
        private bool _acquireSubmitted,
            _releaseSubmitted,
            _released,
            _disposed;
        private uint _externalFamily;
        public override SKImage Image => _wrapped!.Image;

        internal void Initialize()
        {
            var descriptor = source.Buffer;
            var format =
                descriptor.Format == NativeTextureFormat.Bgra8888
                    ? Format.B8G8R8A8Unorm
                    : Format.R8G8B8A8Unorm;
            _externalFamily =
                descriptor.Platform == NativeTexturePlatform.Windows
                    ? uint.MaxValue - 1
                    : uint.MaxValue - 2;
            var handleType =
                descriptor.Platform == NativeTexturePlatform.Windows
                    ? ExternalMemoryHandleTypeFlags.D3D11TextureKmtBit
                    : ExternalMemoryHandleTypeFlags.DmaBufBitExt;
            var external = new ExternalMemoryImageCreateInfo
            {
                SType = StructureType.ExternalMemoryImageCreateInfo,
                HandleTypes = handleType,
            };
            var plane = new SubresourceLayout();
            var modifier = new ImageDrmFormatModifierExplicitCreateInfoEXT
            {
                SType = StructureType.ImageDrmFormatModifierExplicitCreateInfoExt,
            };
            if (descriptor is LinuxDmaBufTextureBuffer dma)
            {
                if (
                    dma.FileDescriptor < 0
                    || dma.RowPitch < (ulong)dma.Width * 4
                    || dma.AllocationSize == 0
                )
                    throw new ArgumentException("Invalid DMA-BUF layout.");
                plane.Offset = dma.Offset;
                plane.RowPitch = dma.RowPitch;
                modifier.DrmFormatModifier = dma.Modifier;
                modifier.DrmFormatModifierPlaneCount = 1;
                modifier.PPlaneLayouts = &plane;
                external.PNext = &modifier;
            }
            if (descriptor is WindowsSharedTextureBuffer windows)
            {
                var identity = new PhysicalDeviceIDProperties
                {
                    SType = StructureType.PhysicalDeviceIDProperties,
                };
                var properties = new PhysicalDeviceProperties2
                {
                    SType = StructureType.PhysicalDeviceProperties2,
                    PNext = &identity,
                };
                owner._vk.GetPhysicalDeviceProperties2(owner._physical, &properties);
                if (
                    !identity.DeviceLuidvalid
                    || windows.AdapterLuid != *(long*)identity.DeviceLuid
                    || windows.SharedHandle == 0
                )
                    throw new PlatformNotSupportedException(
                        "Camera texture and renderer must use the same GPU adapter LUID."
                    );
            }
            var info = new ImageCreateInfo
            {
                SType = StructureType.ImageCreateInfo,
                PNext = &external,
                ImageType = ImageType.Type2D,
                Format = format,
                Extent = new((uint)descriptor.Width, (uint)descriptor.Height, 1),
                MipLevels = 1,
                ArrayLayers = 1,
                Samples = SampleCountFlags.Count1Bit,
                Tiling =
                    descriptor.Platform == NativeTexturePlatform.Windows
                        ? ImageTiling.Optimal
                        : ImageTiling.DrmFormatModifierExt,
                Usage = ImageUsageFlags.SampledBit,
                SharingMode = SharingMode.Exclusive,
            };
            var queryExternal = new PhysicalDeviceExternalImageFormatInfo
            {
                SType = StructureType.PhysicalDeviceExternalImageFormatInfo,
                HandleType = handleType,
            };
            var queryModifier = new PhysicalDeviceImageDrmFormatModifierInfoEXT
            {
                SType = StructureType.PhysicalDeviceImageDrmFormatModifierInfoExt,
                DrmFormatModifier = modifier.DrmFormatModifier,
                SharingMode = SharingMode.Exclusive,
            };
            if (descriptor is LinuxDmaBufTextureBuffer)
                queryExternal.PNext = &queryModifier;
            var query = new PhysicalDeviceImageFormatInfo2
            {
                SType = StructureType.PhysicalDeviceImageFormatInfo2,
                PNext = &queryExternal,
                Format = format,
                Type = ImageType.Type2D,
                Tiling = info.Tiling,
                Usage = info.Usage,
            };
            var externalProperties = new ExternalImageFormatProperties
            {
                SType = StructureType.ExternalImageFormatProperties,
            };
            var imageProperties = new ImageFormatProperties2
            {
                SType = StructureType.ImageFormatProperties2,
                PNext = &externalProperties,
            };
            Check(
                owner._vk.GetPhysicalDeviceImageFormatProperties2(
                    owner._physical,
                    &query,
                    &imageProperties
                ),
                "native texture format support"
            );
            if (
                (
                    externalProperties.ExternalMemoryProperties.ExternalMemoryFeatures
                    & ExternalMemoryFeatureFlags.ImportableBit
                ) == 0
            )
                throw new PlatformNotSupportedException(
                    "GPU cannot import the requested native format/modifier."
                );
            Check(
                owner._vk.CreateImage(owner._device, &info, null, out _image),
                "native texture image"
            );
            owner._vk.GetImageMemoryRequirements(owner._device, _image, out var requirements);
            var bits = requirements.MemoryTypeBits;
            var bytes = requirements.Size;
            var winImport = new ImportMemoryWin32HandleInfoKHR
            {
                SType = StructureType.ImportMemoryWin32HandleInfoKhr,
                HandleType = handleType,
            };
            var fdImport = new ImportMemoryFdInfoKHR
            {
                SType = StructureType.ImportMemoryFDInfoKhr,
                HandleType = handleType,
                Fd = -1,
            };
            var dedicated = new MemoryDedicatedAllocateInfo
            {
                SType = StructureType.MemoryDedicatedAllocateInfo,
                Image = _image,
            };
            void* import;
            if (descriptor is WindowsSharedTextureBuffer win)
            {
                var get = (delegate* unmanaged<
                    nint,
                    ExternalMemoryHandleTypeFlags,
                    nint,
                    MemoryWin32HandlePropertiesKHR*,
                    Result>)
                    (nint)
                        owner._vk.GetDeviceProcAddr(
                            owner._device,
                            "vkGetMemoryWin32HandlePropertiesKHR"
                        );
                if (get == null)
                    throw new PlatformNotSupportedException(
                        "Win32 external-memory import is unavailable."
                    );
                var properties = new MemoryWin32HandlePropertiesKHR
                {
                    SType = StructureType.MemoryWin32HandlePropertiesKhr,
                };
                Check(
                    get(owner._device.Handle, handleType, win.SharedHandle, &properties),
                    "native texture memory properties"
                );
                bits &= properties.MemoryTypeBits;
                winImport.Handle = win.SharedHandle;
                winImport.PNext = &dedicated;
                import = &winImport;
            }
            else if (descriptor is LinuxDmaBufTextureBuffer dmaMemory)
            {
                var get = (delegate* unmanaged<
                    nint,
                    ExternalMemoryHandleTypeFlags,
                    int,
                    MemoryFdPropertiesKHR*,
                    Result>)
                    (nint)owner._vk.GetDeviceProcAddr(owner._device, "vkGetMemoryFdPropertiesKHR");
                if (get == null)
                    throw new PlatformNotSupportedException(
                        "DMA-BUF external-memory import is unavailable."
                    );
                var properties = new MemoryFdPropertiesKHR
                {
                    SType = StructureType.MemoryFDPropertiesKhr,
                };
                Check(
                    get(owner._device.Handle, handleType, dmaMemory.FileDescriptor, &properties),
                    "DMA-BUF memory properties"
                );
                bits &= properties.MemoryTypeBits;
                bytes = dmaMemory.AllocationSize;
                if (bytes < requirements.Size)
                    throw new ArgumentException(
                        "DMA-BUF allocation is smaller than Vulkan requirements."
                    );
                fdImport.PNext = &dedicated;
                import = &fdImport;
            }
            else
                throw new PlatformNotSupportedException(
                    "Unsupported Vulkan native texture descriptor."
                );
            if (bytes > 256UL * 1024 * 1024)
                throw new NotSupportedException("Native import exceeds 256 MiB.");
            owner._vk.GetPhysicalDeviceMemoryProperties(owner._physical, out var memoryProperties);
            uint memoryType = uint.MaxValue;
            for (uint i = 0; i < memoryProperties.MemoryTypeCount; i++)
                if ((bits & (1u << (int)i)) != 0)
                {
                    memoryType = i;
                    break;
                }
            if (memoryType == uint.MaxValue)
                throw new PlatformNotSupportedException("No compatible native buffer memory.");
            var allocate = new MemoryAllocateInfo
            {
                SType = StructureType.MemoryAllocateInfo,
                PNext = import,
                AllocationSize = bytes,
                MemoryTypeIndex = memoryType,
            };
            if (descriptor is LinuxDmaBufTextureBuffer fd)
            {
                fdImport.Fd = Dup(fd.FileDescriptor);
                if (fdImport.Fd < 0)
                    throw new IOException("Could not duplicate DMA-BUF fd.");
            }
            var allocated = owner._vk.AllocateMemory(owner._device, &allocate, null, out _memory);
            if (allocated != Result.Success && fdImport.Fd >= 0)
                Close(fdImport.Fd);
            Check(allocated, "native texture allocation"); // successful import consumes the duplicated fd
            Check(
                owner._vk.BindImageMemory(owner._device, _image, _memory, 0),
                "native texture bind"
            );
            info.InitialLayout = ImageLayout.General;
            owner._observer.RegisterHostTarget(_image.Handle, info, _externalFamily);
            var pool = new CommandPoolCreateInfo
            {
                SType = StructureType.CommandPoolCreateInfo,
                QueueFamilyIndex = owner._family,
                Flags = CommandPoolCreateFlags.ResetCommandBufferBit,
            };
            Check(
                owner._vk.CreateCommandPool(owner._device, &pool, null, out _pool),
                "native texture pool"
            );
            var command = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = _pool,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = 1,
            };
            Check(
                owner._vk.AllocateCommandBuffers(owner._device, &command, out _command),
                "native texture command"
            );
            owner._observer.Journal.Allocate(_command.Handle, _pool.Handle);
            var fence = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(
                owner._vk.CreateFence(owner._device, &fence, null, out _fence),
                "native texture fence"
            );
            Transition(true);
            _wrapped = owner._session.WrapVulkanImage(
                descriptor.Width,
                descriptor.Height,
                new SKGraphiteVkTextureInfo
                {
                    SampleCount = 1,
                    Format = (int)format,
                    ImageTiling = (int)info.Tiling,
                    ImageUsageFlags = (uint)info.Usage,
                    AspectMask = (uint)ImageAspectFlags.ColorBit,
                },
                (int)ImageLayout.ShaderReadOnlyOptimal,
                owner._family,
                (nint)_image.Handle,
                descriptor.Format == NativeTextureFormat.Bgra8888
                    ? SKColorType.Bgra8888
                    : SKColorType.Rgba8888
            );
        }

        private void Transition(bool acquire)
        {
            if (_acquireSubmitted)
                Check(
                    owner._vk.WaitForFences(owner._device, 1, in _fence, true, 5_000_000_000),
                    "native acquire retirement"
                );
            Check(owner._vk.ResetCommandBuffer(_command, 0), "native transition reset");
            var begin = new CommandBufferBeginInfo
            {
                SType = StructureType.CommandBufferBeginInfo,
                Flags = CommandBufferUsageFlags.OneTimeSubmitBit,
            };
            Check(owner._vk.BeginCommandBuffer(_command, &begin), "native transition begin");
            owner._observer.Journal.Begin(_command.Handle);
            var state = owner._observer.State(_image.Handle);
            var barrier = new ImageMemoryBarrier
            {
                SType = StructureType.ImageMemoryBarrier,
                Image = _image,
                OldLayout = state.Layout,
                NewLayout = acquire ? ImageLayout.ShaderReadOnlyOptimal : ImageLayout.General,
                SrcQueueFamilyIndex = acquire ? _externalFamily : owner._family,
                DstQueueFamilyIndex = acquire ? owner._family : _externalFamily,
                SrcAccessMask = acquire ? 0 : AccessFlags.ShaderReadBit,
                DstAccessMask = acquire ? AccessFlags.ShaderReadBit : 0,
                SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1),
            };
            owner._observer.Call<VulkanObserver.CmdPipelineBarrierDelegate>("vkCmdPipelineBarrier")(
                _command,
                PipelineStageFlags.AllCommandsBit,
                PipelineStageFlags.AllCommandsBit,
                0,
                0,
                null,
                0,
                null,
                1,
                &barrier
            );
            Check(owner._vk.EndCommandBuffer(_command), "native transition end");
            owner._observer.Journal.End(_command.Handle);
            Check(
                owner._vk.ResetFences(owner._device, 1, in _fence),
                "native transition fence reset"
            );
            var command = _command;
            var submit = new SubmitInfo
            {
                SType = StructureType.SubmitInfo,
                CommandBufferCount = 1,
                PCommandBuffers = &command,
            };
            var result = owner._vk.QueueSubmit(owner._queue, 1, &submit, _fence);
            if (result == Result.Success)
            {
                _acquireSubmitted = true;
                if (!acquire)
                    _releaseSubmitted = true;
            }
            owner._observer.Journal.Submit([command.Handle], result);
            Check(result, "native transition submit");
            Check(
                owner._vk.WaitForFences(owner._device, 1, in _fence, true, 5_000_000_000),
                "native transition completion"
            );
            if (!acquire)
                _released = true;
        }

        private void Check(Result result, string operation)
        {
            if (result == Result.ErrorDeviceLost)
                owner._session.NotifyVulkanDeviceLost();
            if (result != Result.Success)
                throw new InvalidOperationException($"{operation}: {result}");
        }

        public override void Dispose()
        {
            if (_disposed)
                return;
            if (_acquireSubmitted && !_released && !owner._session.IsDeviceLost)
            {
                if (_releaseSubmitted)
                {
                    Check(
                        owner._vk.WaitForFences(owner._device, 1, in _fence, true, 5_000_000_000),
                        "native release retirement"
                    );
                    _released = true;
                }
                else
                    Transition(false);
            }
            _wrapped?.Dispose();
            if (_fence.Handle != 0)
                owner._vk.DestroyFence(owner._device, _fence, null);
            if (_pool.Handle != 0)
            {
                owner._observer.Journal.FreePool(_pool.Handle);
                owner._vk.DestroyCommandPool(owner._device, _pool, null);
            }
            if (_image.Handle != 0)
            {
                owner._observer.ForgetHostTarget(_image.Handle);
                owner._vk.DestroyImage(owner._device, _image, null);
            }
            if (_memory.Handle != 0)
                owner._vk.FreeMemory(owner._device, _memory, null);
            _disposed = true;
            owner._live.Remove(this);
            owner.Retired++;
            source.Dispose();
        }
    }

    [DllImport("libc", EntryPoint = "dup")]
    private static extern int Dup(int fd);

    [DllImport("libc", EntryPoint = "close")]
    private static extern int Close(int fd);
}
