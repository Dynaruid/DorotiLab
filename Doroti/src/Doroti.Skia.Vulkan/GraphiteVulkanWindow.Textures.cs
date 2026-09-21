using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using SkiaSharp;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Skia.Vulkan;

public sealed unsafe partial class GraphiteVulkanWindow
{
    private readonly Dictionary<nint, HardwareTexture> _textureInputs = [];
    private readonly List<HardwareTexture> _failedTextureInputs = [];
    public long ImportedTextureFrames { get; private set; }
    public long RetiredTextureFrames { get; private set; }

    /// <summary>The caller supplies an acquired, producer-complete RGBA AHB and a retained source lease.
    /// The release callback is invoked only after the GPU relinquishes foreign ownership.</summary>
    public void DrawHardwareBuffer(
        SKCanvas canvas,
        nint buffer,
        int width,
        int height,
        SKRect destination,
        SKSamplingOptions sampling,
        Action release
    )
    {
        CheckOwner();
        if (!SupportsHardwareBuffer || _platformRecording is null)
        {
            release();
            throw new PlatformNotSupportedException(
                "An active Android Vulkan frame with AHB import is required."
            );
        }
        if (!_textureInputs.TryGetValue(buffer, out var texture))
        {
            texture = new HardwareTexture(this, buffer, width, height, release);
            _textureInputs.Add(buffer, texture);
            ImportedTextureFrames++;
        }
        else
            release();
        canvas.DrawImage(texture.Image, destination, sampling);
    }

    private void ReleaseTextureInputs(bool completed)
    {
        if (_textureInputs.Count == 0 && _failedTextureInputs.Count == 0)
            return;
        if (!completed)
        {
            var idle = _vk.DeviceWaitIdle(_device);
            if (idle == Result.ErrorDeviceLost)
                _session!.NotifyVulkanDeviceLost();
            else
                CheckDevice(idle, "external texture failure/cancellation drain");
        }
        foreach (var texture in _textureInputs.Values)
        {
            if (!_session!.IsDeviceLost)
                texture.EnsureReleased();
            texture.Dispose();
            RetiredTextureFrames++;
        }
        _textureInputs.Clear();
        foreach (var texture in _failedTextureInputs)
        {
            if (!_session!.IsDeviceLost)
                texture.EnsureReleased();
            texture.Dispose();
        }
        _failedTextureInputs.Clear();
    }

    private sealed class HardwareTexture : IDisposable
    {
        private const uint Foreign = uint.MaxValue - 2;
        private readonly GraphiteVulkanWindow _owner;
        private readonly Action _release;
        private VkImage _image;
        private DeviceMemory _memory;
        private CommandBuffer _acquire;
        private SkiaGraphiteSession.VulkanImage? _wrapped;
        private bool _acquired;
        private bool _disposed;
        internal bool ReleaseSubmitted;
        internal SKImage Image => _wrapped!.Image;

        internal HardwareTexture(
            GraphiteVulkanWindow owner,
            nint buffer,
            int width,
            int height,
            Action release
        )
        {
            _owner = owner;
            _release = release;
            try
            {
                if (buffer == 0 || width <= 0 || height <= 0 || width > 8192 || height > 8192)
                    throw new ArgumentOutOfRangeException(nameof(width));
                DescribeHardwareBuffer(buffer, out var description);
                if (
                    description.Width != width
                    || description.Height != height
                    || description.Layers != 1
                    || description.Format != 1
                    || (description.Usage & (1UL << 8)) == 0
                )
                    throw new ArgumentException(
                        "AHardwareBuffer extent, format, layers or GPU sampling usage does not match the texture.",
                        nameof(buffer)
                    );
                var get = (delegate* unmanaged[Cdecl]<
                    nint,
                    nint,
                    AndroidHardwareBufferPropertiesANDROID*,
                    Result>)
                    (nint)
                        owner._vk.GetDeviceProcAddr(
                            owner._device,
                            "vkGetAndroidHardwareBufferPropertiesANDROID"
                        );
                if (get == null)
                    throw new PlatformNotSupportedException("AHB import procedure unavailable.");
                var format = new AndroidHardwareBufferFormatPropertiesANDROID
                {
                    SType = StructureType.AndroidHardwareBufferFormatPropertiesAndroid,
                };
                var properties = new AndroidHardwareBufferPropertiesANDROID
                {
                    SType = StructureType.AndroidHardwareBufferPropertiesAndroid,
                    PNext = &format,
                };
                Check(get(owner._device.Handle, buffer, &properties), "texture AHB properties");
                if (
                    format.Format != Format.R8G8B8A8Unorm
                    || properties.AllocationSize > 256UL * 1024 * 1024
                )
                    throw new PlatformNotSupportedException(
                        "Texture import requires a bounded RGBA8888 hardware buffer; convert external YUV on the GPU first."
                    );
                var external = new ExternalMemoryImageCreateInfo
                {
                    SType = StructureType.ExternalMemoryImageCreateInfo,
                    HandleTypes = ExternalMemoryHandleTypeFlags.AndroidHardwareBufferBitAndroid,
                };
                var info = new ImageCreateInfo
                {
                    SType = StructureType.ImageCreateInfo,
                    PNext = &external,
                    ImageType = ImageType.Type2D,
                    Format = format.Format,
                    Extent = new((uint)width, (uint)height, 1),
                    MipLevels = 1,
                    ArrayLayers = 1,
                    Samples = SampleCountFlags.Count1Bit,
                    Tiling = ImageTiling.Optimal,
                    Usage = ImageUsageFlags.SampledBit,
                    SharingMode = SharingMode.Exclusive,
                };
                Check(
                    owner._vk.CreateImage(owner._device, &info, null, out _image),
                    "texture import image"
                );
                var dedicated = new MemoryDedicatedAllocateInfo
                {
                    SType = StructureType.MemoryDedicatedAllocateInfo,
                    Image = _image,
                };
                var import = new ImportAndroidHardwareBufferInfoANDROID
                {
                    SType = StructureType.ImportAndroidHardwareBufferInfoAndroid,
                    PNext = &dedicated,
                    Buffer = (nint*)buffer,
                };
                owner._vk.GetPhysicalDeviceMemoryProperties(owner._physical, out var memory);
                var memoryType = uint.MaxValue;
                for (uint i = 0; i < memory.MemoryTypeCount; i++)
                    if ((properties.MemoryTypeBits & (1u << (int)i)) != 0)
                    {
                        memoryType = i;
                        break;
                    }
                if (memoryType == uint.MaxValue)
                    throw new PlatformNotSupportedException("No compatible AHB memory type.");
                var allocate = new MemoryAllocateInfo
                {
                    SType = StructureType.MemoryAllocateInfo,
                    PNext = &import,
                    AllocationSize = properties.AllocationSize,
                    MemoryTypeIndex = memoryType,
                };
                Check(
                    owner._vk.AllocateMemory(owner._device, &allocate, null, out _memory),
                    "texture import allocation"
                );
                Check(
                    owner._vk.BindImageMemory(owner._device, _image, _memory, 0),
                    "texture import bind"
                );
                // Imported producer contents arrive in GENERAL with foreign ownership.
                info.InitialLayout = ImageLayout.General;
                owner._stockObserver!.RegisterHostTarget(_image.Handle, info, Foreign);
                var commands = new CommandBufferAllocateInfo
                {
                    SType = StructureType.CommandBufferAllocateInfo,
                    CommandPool = owner._pool,
                    Level = CommandBufferLevel.Primary,
                    CommandBufferCount = 1,
                };
                Check(
                    owner._vk.AllocateCommandBuffers(owner._device, &commands, out _acquire),
                    "texture acquire commands"
                );
                owner._stockObserver.Journal.Allocate(_acquire.Handle, owner._pool.Handle);
                SubmitTransition(acquire: true);
                _acquired = true;
                _wrapped = owner._session!.WrapVulkanImage(
                    width,
                    height,
                    new SKGraphiteVkTextureInfo
                    {
                        SampleCount = 1,
                        Format = (int)format.Format,
                        ImageTiling = (int)ImageTiling.Optimal,
                        ImageUsageFlags = (uint)ImageUsageFlags.SampledBit,
                        AspectMask = (uint)ImageAspectFlags.ColorBit,
                    },
                    (int)ImageLayout.ShaderReadOnlyOptimal,
                    owner._family,
                    (nint)_image.Handle
                );
            }
            catch
            {
                // Never return a producer allocation while even its acquire barrier is in flight.
                try
                {
                    if (_acquired)
                    {
                        CheckDeviceIdle();
                        try
                        {
                            EnsureReleased();
                        }
                        catch when (owner._session!.IsDeviceLost) { }
                    }
                    Dispose();
                }
                catch
                {
                    // Keep failed construction rooted until the window establishes safe retirement.
                    owner._failedTextureInputs.Add(this);
                    throw;
                }
                throw;
            }
        }

        private void CheckDeviceIdle()
        {
            var result = _owner._vk.DeviceWaitIdle(_owner._device);
            if (result == Result.ErrorDeviceLost)
                _owner._session!.NotifyVulkanDeviceLost();
            else
                _owner.CheckDevice(result, "texture retirement drain");
        }

        internal void RecordRelease(CommandBuffer command) => Transition(command, acquire: false);

        private void Transition(CommandBuffer command, bool acquire)
        {
            var state = _owner._stockObserver!.State(_image.Handle);
            var barrier = new ImageMemoryBarrier
            {
                SType = StructureType.ImageMemoryBarrier,
                Image = _image,
                OldLayout = state.Layout,
                NewLayout = acquire ? ImageLayout.ShaderReadOnlyOptimal : ImageLayout.General,
                SrcQueueFamilyIndex = acquire ? Foreign : _owner._family,
                DstQueueFamilyIndex = acquire ? _owner._family : Foreign,
                SrcAccessMask = acquire ? 0 : AccessFlags.ShaderReadBit,
                DstAccessMask = acquire ? AccessFlags.ShaderReadBit : 0,
                SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1),
            };
            _owner._stockObserver.Call<VulkanObserver.CmdPipelineBarrierDelegate>(
                "vkCmdPipelineBarrier"
            )(
                command,
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
        }

        private void SubmitTransition(bool acquire)
        {
            var vk = _owner._vk;
            Check(vk.ResetCommandBuffer(_acquire, 0), "texture transition reset");
            var begin = new CommandBufferBeginInfo
            {
                SType = StructureType.CommandBufferBeginInfo,
                Flags = CommandBufferUsageFlags.OneTimeSubmitBit,
            };
            Check(vk.BeginCommandBuffer(_acquire, &begin), "texture transition begin");
            _owner._stockObserver!.Journal.Begin(_acquire.Handle);
            Transition(_acquire, acquire);
            Check(vk.EndCommandBuffer(_acquire), "texture transition end");
            _owner._stockObserver.Journal.End(_acquire.Handle);
            var command = _acquire;
            var submit = new SubmitInfo
            {
                SType = StructureType.SubmitInfo,
                CommandBufferCount = 1,
                PCommandBuffers = &command,
            };
            var result = vk.QueueSubmit(_owner._queue, 1, &submit, default);
            if (result == Result.Success && acquire)
                _acquired = true;
            _owner._stockObserver.Journal.Submit([command.Handle], result);
            _owner.CheckDevice(result, "texture transition submit");
        }

        internal void EnsureReleased()
        {
            if (!_acquired || ReleaseSubmitted || _owner._session!.IsDeviceLost)
                return;
            SubmitTransition(acquire: false);
            CheckDeviceIdle();
            ReleaseSubmitted = true;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            _wrapped?.Dispose();
            if (_acquire.Handle != 0)
            {
                var command = _acquire;
                _owner._vk.FreeCommandBuffers(_owner._device, _owner._pool, 1, &command);
                _owner._stockObserver!.Journal.Free(command.Handle);
            }
            if (_image.Handle != 0)
            {
                _owner._stockObserver!.ForgetHostTarget(_image.Handle);
                _owner._vk.DestroyImage(_owner._device, _image, null);
            }
            if (_memory.Handle != 0)
                _owner._vk.FreeMemory(_owner._device, _memory, null);
            _release();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HardwareBufferDescription
    {
        public uint Width,
            Height,
            Layers,
            Format;
        public ulong Usage;
        public uint Stride,
            Reserved;
        public ulong Reserved2;
    }

    [DllImport("android", EntryPoint = "AHardwareBuffer_describe")]
    private static extern void DescribeHardwareBuffer(
        nint buffer,
        out HardwareBufferDescription description
    );
}
