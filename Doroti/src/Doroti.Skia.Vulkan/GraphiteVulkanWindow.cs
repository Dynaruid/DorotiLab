using System.Runtime.InteropServices;
using Doroti.Skia.Rendering;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using SkiaSharp;
using VkImage = Silk.NET.Vulkan.Image;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace Doroti.Skia.Vulkan;

/// <summary>
/// Single-threaded Vulkan WSI owner. Native hosts supply their VkSurfaceKHR and
/// retain it until disposal. Graphite draws a persistent image; only GPU copies
/// reach the swapchain. A successful Present means queue acceptance, not scan-out.
/// </summary>
public sealed unsafe partial class GraphiteVulkanWindow : IDisposable
{
    private const uint Api12 = (1u << 22) | (2u << 12);
    private const ulong Timeout = 5_000_000_000;
    private readonly Vk _vk;
    private readonly Instance _instance;
    private readonly SurfaceKHR _surface;
    private readonly bool _ownsInstance;
    private readonly bool _ownsSurface;
    private readonly bool _pipelinedWindowFrames;
    private int _owner = Environment.CurrentManagedThreadId;
    private KhrSurface _surfaces = null!;
    private KhrSwapchain _swapchains = null!;
    private PhysicalDevice _physical;
    private Device _device;
    private Queue _queue;
    private uint _family;
    private SwapchainKHR _swapchain;
    private VkImage[] _images = [];
    private bool[] _initialized = [];
    private VkSemaphore[] _presentReady = [];
    private Fence _fence;
    private CommandPool _pool;
    private CommandBuffer _command;
    private VkImage _backing;
    private DeviceMemory _memory;
    private SkiaGraphiteSession? _session;
    private SkiaGraphiteSession.VulkanTarget? _target;
    private bool _disposed;
    private bool _recreate = true;
    private Format _format;
    private VulkanObserver? _stockObserver;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public long Generation { get; private set; }
    public string DeviceName { get; private set; } = "";
    public bool IsSoftwareDevice { get; private set; }
    public object ContextIdentity => _session ?? throw new InvalidOperationException("No Vulkan session.");
    public bool IsDeviceLost => _session?.IsDeviceLost == true;
    public event Action? ResourcesReleasing;

    public static GraphiteVulkanWindow CreateAndroid(nint nativeWindow)
    {
        if (!GraphiteNativeLibrary.IsOfficialSelected)
            throw new InvalidOperationException("Configure the official APK asset before creating an Android surface.");
        var vk = Vk.GetApi();
        string[] extensions = ["VK_KHR_surface", "VK_KHR_android_surface"];
        var instance = CreateInstance(vk, extensions, Api12);
        ulong surface = 0;
        try
        {
            // Keep this portable assembly free of Android framework references.
            var create = (delegate* unmanaged[Cdecl]<nint, AndroidSurfaceInfo*, void*, ulong*, Result>)
                (nint)vk.GetInstanceProcAddr(instance, "vkCreateAndroidSurfaceKHR");
            if (create == null) throw new PlatformNotSupportedException("Vulkan Android surface extension is unavailable.");
            var info = new AndroidSurfaceInfo { Type = 1000008000, Window = nativeWindow };
            Check(create(instance.Handle, &info, null, &surface), "vkCreateAndroidSurfaceKHR");
            return new(vk, instance, new(surface), extensions, true, true, pipelinedWindowFrames: true);
        }
        catch
        {
            if (surface != 0 && vk.TryGetInstanceExtension<KhrSurface>(instance, out var surfaces))
            {
                using (surfaces) surfaces.DestroySurface(instance, new(surface), null);
            }
            vk.DestroyInstance(instance, null); vk.Dispose(); throw;
        }
    }

    public static GraphiteVulkanWindow FromQtOfficial(nint instance, ulong surface, string[] enabledInstanceExtensions, uint instanceApiVersion)
    {
        if (instanceApiVersion < Api12) throw new PlatformNotSupportedException("Qt must create and report a real Vulkan 1.2 instance.");
        GraphiteNativeLibrary.ConfigureOfficial(GraphiteNativeLibrary.PackagedOfficialAsset());
        var vk = Vk.GetApi();
        try { return new(vk, new(instance), new(surface), enabledInstanceExtensions, false, false, pipelinedWindowFrames: true); }
        catch { vk.Dispose(); throw; }
    }

    private GraphiteVulkanWindow(Vk vk, Instance instance, SurfaceKHR surface, string[] extensions,
        bool ownsInstance, bool ownsSurface, long? adapterLuid = null, bool pipelinedWindowFrames = false)
    {
        _vk = vk; _instance = instance; _surface = surface;
        _ownsInstance = ownsInstance; _ownsSurface = ownsSurface;
        _pipelinedWindowFrames = pipelinedWindowFrames;
        try
        {
            if (surface.Handle != 0 && !_vk.TryGetInstanceExtension(_instance, out _surfaces))
                throw new PlatformNotSupportedException("VK_KHR_surface is required.");
            uint count = 0;
            Check(_vk.EnumeratePhysicalDevices(instance, &count, null), "physical device count");
            var devices = new PhysicalDevice[count];
            fixed (PhysicalDevice* values = devices)
                Check(_vk.EnumeratePhysicalDevices(instance, &count, values), "physical devices");
            foreach (var candidate in devices)
            {
                _vk.GetPhysicalDeviceProperties(candidate, out var properties);
                if (properties.ApiVersion < Api12) continue;
                if (adapterLuid is { } requiredLuid)
                {
                    var identity = new PhysicalDeviceIDProperties { SType = StructureType.PhysicalDeviceIDProperties };
                    var properties2 = new PhysicalDeviceProperties2 { SType = StructureType.PhysicalDeviceProperties2, PNext = &identity };
                    _vk.GetPhysicalDeviceProperties2(candidate, &properties2);
                    if (!identity.DeviceLuidvalid || *(long*)identity.DeviceLuid != requiredLuid) continue;
                }
                uint families = 0;
                _vk.GetPhysicalDeviceQueueFamilyProperties(candidate, &families, null);
                var queues = new QueueFamilyProperties[families];
                fixed (QueueFamilyProperties* values = queues)
                    _vk.GetPhysicalDeviceQueueFamilyProperties(candidate, &families, values);
                for (uint i = 0; i < families; i++)
                {
                    if ((queues[i].QueueFlags & QueueFlags.GraphicsBit) == 0) continue;
                    if (surface.Handle != 0)
                    {
                        Check(_surfaces.GetPhysicalDeviceSurfaceSupport(candidate, i, surface, out var present), "present support");
                        if (!present) continue;
                    }
                    _physical = candidate; _family = i;
                    DeviceName = Marshal.PtrToStringUTF8((nint)properties.DeviceName) ?? "Vulkan GPU";
                    IsSoftwareDevice = properties.DeviceType == PhysicalDeviceType.Cpu;
                    if (OperatingSystem.IsLinux())
                        Console.WriteLine($"DorotiGraphite Vulkan device={DeviceName} type={properties.DeviceType}");
                    break;
                }
                if (_physical.Handle != 0) break;
            }
            if (_physical.Handle == 0) throw new PlatformNotSupportedException(
                "A Vulkan 1.2 device with a compatible graphics/present queue is required.");
            float priority = 1;
            var queueInfo = new DeviceQueueCreateInfo { SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = _family, QueueCount = 1, PQueuePriorities = &priority };
            var extension = surface.Handle != 0 ? "VK_KHR_swapchain" : "VK_KHR_external_memory_win32";
            uint extensionCount = 0;
            Check(_vk.EnumerateDeviceExtensionProperties(_physical, (byte*)null, &extensionCount, null), "device extension count");
            var availableExtensions = new ExtensionProperties[extensionCount];
            fixed (ExtensionProperties* available = availableExtensions)
                Check(_vk.EnumerateDeviceExtensionProperties(_physical, (byte*)null, &extensionCount, available), "device extensions");
            var extensionNames = new List<string> { extension };
            foreach (var entry in availableExtensions)
            {
                var copy = entry;
                var name = Marshal.PtrToStringUTF8((nint)copy.ExtensionName);
                if (name == "VK_KHR_driver_properties" || name == "VK_KHR_create_renderpass2")
                    extensionNames.Add(name);
            }
            var enabledExtensions = extensionNames.Select(Marshal.StringToCoTaskMemUTF8).ToArray();
            try
            {
                fixed (nint* names = enabledExtensions)
                {
                var info = new DeviceCreateInfo { SType = StructureType.DeviceCreateInfo,
                    QueueCreateInfoCount = 1, PQueueCreateInfos = &queueInfo,
                    EnabledExtensionCount = (uint)enabledExtensions.Length, PpEnabledExtensionNames = (byte**)names };
                Check(_vk.CreateDevice(_physical, &info, null, out _device), "vkCreateDevice");
                }
            }
            finally { foreach (var value in enabledExtensions) Marshal.FreeCoTaskMem(value); }
            _vk.GetDeviceQueue(_device, _family, 0, out _queue);
            if (surface.Handle != 0 && !_vk.TryGetDeviceExtension(_instance, _device, out _swapchains))
                throw new PlatformNotSupportedException("VK_KHR_swapchain is required.");
            _stockObserver = new VulkanObserver(_vk, _instance, _device, _queue, _family,
                extensionNames.Contains("VK_KHR_create_renderpass2"));
            _session = SkiaGraphiteSession.CreateOfficialVulkan(new(_instance.Handle, _physical.Handle,
                _device.Handle, _queue.Handle, _family, Api12, _stockObserver.Resolve,
                image => { var state = _stockObserver.State((ulong)image); return ((int)state.Layout, state.Family); }, _stockObserver.Check), 1,
                _pipelinedWindowFrames ? WindowFrameLimit : 1);
            var poolInfo = new CommandPoolCreateInfo { SType = StructureType.CommandPoolCreateInfo,
                QueueFamilyIndex = _family, Flags = CommandPoolCreateFlags.ResetCommandBufferBit };
            Check(_vk.CreateCommandPool(_device, &poolInfo, null, out _pool), "command pool");
            if (surface.Handle != 0) CreateWindowFrameSlots();
            else
            {
                var allocation = new CommandBufferAllocateInfo { SType = StructureType.CommandBufferAllocateInfo,
                    CommandPool = _pool, Level = CommandBufferLevel.Primary, CommandBufferCount = 1 };
                Check(_vk.AllocateCommandBuffers(_device, &allocation, out _command), "command buffer");
                _stockObserver?.Journal.Allocate(_command.Handle, _pool.Handle);
                var fence = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
                Check(_vk.CreateFence(_device, &fence, null, out _fence), "fence");
            }
        }
        catch
        {
            // Caller retains instance on construction failure.
            ReleaseDevice();
            throw;
        }
    }

    public bool Render(int width, int height, Action<SKSurface, int, int> paint, Func<bool>? shouldPresent = null,
        Action? beforePresent = null)
    {
        CheckOwner();
        if (width <= 0 || height <= 0) return false;
        if (_recreate || Width != width || Height != height) Resize(width, height);
        var startTime = FrameTimestamp();
        PollGpuWork();
        var slot = _windowFrames[_nextWindowFrame];
        if (slot.Frame is not null) { BusyWindowFrames++; return false; }
        uint index = 0;
        var acquired = _swapchains.AcquireNextImage(_device, _swapchain,
            _pipelinedWindowFrames ? 0 : Timeout, slot.Acquired, default, &index);
        if (acquired is Result.NotReady or Result.Timeout) { UnavailableWindowImages++; return false; }
        if (acquired == Result.ErrorOutOfDateKhr) { _recreate = true; return false; }
        if (acquired != Result.SuboptimalKhr) CheckDevice(acquired, "acquire image");
        slot.AcquireWaitPending = true;
        var acquiredTime = FrameTimestamp();
        SkiaGraphiteSession.Frame? frame = null;
        var submitted = false;
        try
        {
            frame = _session!.BeginVulkanFrame(slot.Target!);
            frame.Surface.Canvas.Clear(SKColors.Transparent);
            paint(frame.Surface, Width, Height);
            var paintedTime = FrameTimestamp();
            if (shouldPresent?.Invoke() == false)
            {
                frame.CancelRecording(); frame = null;
                // Retire the acquired generation without showing a superseded frame.
                _recreate = true;
                return false;
            }
            submitted = true;
            frame.Submit();
            var submittedTime = FrameTimestamp();
            var state = slot.Target!.GetState();
            var command = slot.Command;
            CheckDevice(_vk.ResetCommandBuffer(command, 0), "reset commands");
            var begin = new CommandBufferBeginInfo { SType = StructureType.CommandBufferBeginInfo,
                Flags = CommandBufferUsageFlags.OneTimeSubmitBit };
            CheckDevice(_vk.BeginCommandBuffer(command, &begin), "begin commands");
            _stockObserver?.Journal.Begin(command.Handle);
            Barrier(command, slot.Backing, (ImageLayout)state.Layout, ImageLayout.TransferSrcOptimal,
                AccessFlags.MemoryWriteBit, AccessFlags.TransferReadBit);
            Barrier(command, _images[index], _initialized[index] ? ImageLayout.PresentSrcKhr : ImageLayout.Undefined,
                ImageLayout.TransferDstOptimal, 0, AccessFlags.TransferWriteBit);
            var copy = new ImageCopy { SrcSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1),
                DstSubresource = new(ImageAspectFlags.ColorBit, 0, 0, 1), Extent = new((uint)Width, (uint)Height, 1) };
            _vk.CmdCopyImage(command, slot.Backing, ImageLayout.TransferSrcOptimal, _images[index],
                ImageLayout.TransferDstOptimal, 1, &copy);
            Barrier(command, _images[index], ImageLayout.TransferDstOptimal, ImageLayout.PresentSrcKhr,
                AccessFlags.TransferWriteBit, 0);
            var restoreLayout = _stockObserver is null ? ImageLayout.ColorAttachmentOptimal : (ImageLayout)state.Layout;
            Barrier(command, slot.Backing, ImageLayout.TransferSrcOptimal, restoreLayout,
                AccessFlags.TransferReadBit, AccessFlags.MemoryReadBit | AccessFlags.MemoryWriteBit);
            CheckDevice(_vk.EndCommandBuffer(command), "end commands");
            _stockObserver?.Journal.End(command.Handle);
            CheckDevice(_vk.ResetFences(_device, 1, in slot.Fence), "reset submit fence");
            // The GPU waits for acquisition before writing or re-signalling
            // this image's presentation semaphore; the UI thread never waits.
            var ready = _presentReady[index];
            var acquireReady = slot.Acquired;
            // Include the layout transitions, whose barriers use AllCommands,
            // as well as the copy itself in the acquire wait's execution scope.
            var waitStage = PipelineStageFlags.AllCommandsBit;
            var submit = new SubmitInfo { SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 1, PWaitSemaphores = &acquireReady, PWaitDstStageMask = &waitStage,
                CommandBufferCount = 1, PCommandBuffers = &command,
                SignalSemaphoreCount = 1, PSignalSemaphores = &ready };
            var copyResult = _vk.QueueSubmit(_queue, 1, &submit, slot.Fence);
            _stockObserver?.Journal.Submit([command.Handle], copyResult);
            CheckDevice(copyResult, "copy submit");
            _stockObserver?.Check();
            slot.RestoredLayout = restoreLayout;
            var copiedTime = FrameTimestamp();
            slot.AcquireWaitPending = false;
            slot.Frame = frame;
            slot.SubmittedAt = System.Diagnostics.Stopwatch.GetTimestamp();
            frame = null;
            SubmittedWindowFrames++;
            MaximumWindowFramesInFlight = Math.Max(MaximumWindowFramesInFlight, WindowFramesInFlight);
            if (!_pipelinedWindowFrames)
            {
                // Qt has no idle completion callback yet; preserve its
                // synchronous retirement contract until its host adopts one.
                CheckDevice(_vk.WaitForFences(_device, 1, in slot.Fence, true, Timeout), "copy fence");
                CompleteWindowFrame(slot);
            }
            _initialized[index] = true;
            var completedTime = FrameTimestamp();
            var swapchain = _swapchain;
            var present = new PresentInfoKHR { SType = StructureType.PresentInfoKhr,
                WaitSemaphoreCount = 1, PWaitSemaphores = &ready,
                SwapchainCount = 1, PSwapchains = &swapchain, PImageIndices = &index };
            beforePresent?.Invoke();
            var result = _swapchains.QueuePresent(_queue, &present);
            // Android reports Suboptimal when compositor rotation differs from
            // our identity pre-transform. The image remains usable; recreating
            // with the same transform would repeat on every landscape frame.
            // SurfaceView size changes and OutOfDate still recreate normally.
            if (result is Result.ErrorOutOfDateKhr or Result.SuboptimalKhr)
                _recreate = result == Result.ErrorOutOfDateKhr || !OperatingSystem.IsAndroid();
            else CheckDevice(result, "vkQueuePresentKHR");
            _nextWindowFrame = (_nextWindowFrame + 1) % _windowFrames.Count;
            var presentedTime = FrameTimestamp();
            LastFrameTiming = new(Milliseconds(startTime, acquiredTime),
                Milliseconds(acquiredTime, paintedTime), Milliseconds(paintedTime, submittedTime),
                Milliseconds(submittedTime, copiedTime), Milliseconds(copiedTime, completedTime),
                Milliseconds(completedTime, presentedTime), Milliseconds(startTime, presentedTime));
            return result != Result.ErrorOutOfDateKhr;
        }
        catch
        {
            // A failed paint owns an acquired image too. Retire that swapchain
            // generation before retrying instead of exhausting its image pool.
            _recreate = true;
            throw;
        }
        finally
        {
            if (frame is not null)
            {
                if (!submitted) frame.CancelRecording();
                else
                {
                    var idle = _vk.DeviceWaitIdle(_device);
                    if (idle == Result.ErrorDeviceLost) _session!.NotifyVulkanDeviceLost();
                    else Check(idle, "failed frame drain");
                    frame.CompleteGpuWork();
                }
            }
        }
    }

    private void Resize(int width, int height)
    {
        DrainWindowFrames();
        // Keep the native window associated with its current swapchain until
        // WSI creates the replacement. Destroying it first can expose an empty
        // surface to the compositor during an interactive resize.
        ReleaseImages(releaseSwapchain: false);
        Check(_surfaces.GetPhysicalDeviceSurfaceCapabilities(_physical, _surface, out var caps), "surface capabilities");
        // Android paints in SurfaceView coordinates, without pre-rotating the
        // Graphite image. Identity lets the compositor apply device orientation.
        // Claiming CurrentTransform here cancels that rotation even though the
        // pixels were never rotated (and stretches a landscape buffer sideways).
        var preTransform = OperatingSystem.IsAndroid() ? SurfaceTransformFlagsKHR.IdentityBitKhr : caps.CurrentTransform;
        if ((caps.SupportedTransforms & preTransform) == 0)
            throw new PlatformNotSupportedException("The surface does not support the renderer's orientation transform.");
        uint count = 0;
        Check(_surfaces.GetPhysicalDeviceSurfaceFormats(_physical, _surface, &count, null), "format count");
        var formats = new SurfaceFormatKHR[count];
        fixed (SurfaceFormatKHR* values = formats)
            Check(_surfaces.GetPhysicalDeviceSurfaceFormats(_physical, _surface, &count, values), "formats");
        var selected = formats.FirstOrDefault(value => value.Format == Format.B8G8R8A8Unorm && value.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr);
        if (selected.Format == Format.Undefined)
            selected = formats.FirstOrDefault(value => value.Format == Format.R8G8B8A8Unorm && value.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr);
        if (selected.Format == Format.Undefined || (caps.SupportedUsageFlags & ImageUsageFlags.TransferDstBit) == 0)
            throw new PlatformNotSupportedException("Vulkan output requires an RGBA/BGRA UNORM transfer-destination surface.");
        _format = selected.Format;
        // Android's currentExtent can lag SurfaceChanged during rotation.
        // Use the view's pixel size (within supported bounds), as its image is
        // unrotated. Desktop WSI retains its fixed-currentExtent contract.
        var extent = OperatingSystem.IsAndroid() || caps.CurrentExtent.Width == uint.MaxValue
            ? new Extent2D(Math.Clamp((uint)width, caps.MinImageExtent.Width, caps.MaxImageExtent.Width),
                Math.Clamp((uint)height, caps.MinImageExtent.Height, caps.MaxImageExtent.Height)) : caps.CurrentExtent;
        var images = Math.Max(2u, caps.MinImageCount);
        if (caps.MaxImageCount != 0) images = Math.Min(images, caps.MaxImageCount);
        var alpha = new[] { CompositeAlphaFlagsKHR.PreMultipliedBitKhr, CompositeAlphaFlagsKHR.OpaqueBitKhr,
            CompositeAlphaFlagsKHR.InheritBitKhr, CompositeAlphaFlagsKHR.PostMultipliedBitKhr }
            .First(value => (caps.SupportedCompositeAlpha & value) != 0);
        var info = new SwapchainCreateInfoKHR { SType = StructureType.SwapchainCreateInfoKhr,
            Surface = _surface, MinImageCount = images, ImageFormat = _format, ImageColorSpace = selected.ColorSpace,
            ImageExtent = extent, ImageArrayLayers = 1, ImageUsage = ImageUsageFlags.TransferDstBit,
            ImageSharingMode = SharingMode.Exclusive, PreTransform = preTransform,
            CompositeAlpha = alpha, PresentMode = PresentModeKHR.FifoKhr, Clipped = true,
            OldSwapchain = _swapchain };
        var oldSwapchain = _swapchain;
        try
        {
            // A failed replacement also retires OldSwapchain; never acquire
            // from it again, and retain neither handle outside owner cleanup.
            Check(_swapchains.CreateSwapchain(_device, &info, null, out var replacement), "create swapchain");
            _swapchain = replacement;
        }
        catch
        {
            _swapchain = default;
            throw;
        }
        finally
        {
            if (oldSwapchain.Handle != 0) _swapchains.DestroySwapchain(_device, oldSwapchain, null);
        }
        Check(_swapchains.GetSwapchainImages(_device, _swapchain, &count, null), "image count");
        _images = new VkImage[count]; _initialized = new bool[count];
        fixed (VkImage* values = _images)
            Check(_swapchains.GetSwapchainImages(_device, _swapchain, &count, values), "swapchain images");
        _presentReady = new VkSemaphore[count];
        var semaphore = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        for (var i = 0; i < _presentReady.Length; i++)
            Check(_vk.CreateSemaphore(_device, &semaphore, null, out _presentReady[i]), "present semaphore");
        Width = (int)extent.Width; Height = (int)extent.Height;
        foreach (var slot in _windowFrames) CreateWindowBacking(slot, extent);
        _nextWindowFrame = 0;
        Generation++; _recreate = false;
        if (OperatingSystem.IsAndroid())
            Console.WriteLine($"DorotiGraphite swapchain generation={Generation} requested={width}x{height} extent={Width}x{Height} currentTransform={caps.CurrentTransform} preTransform={preTransform}");
    }

    private void CreateWindowBacking(WindowFrameSlot slot, Extent2D extent)
    {
        const ImageUsageFlags usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.InputAttachmentBit |
            ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit;
        var imageInfo = new ImageCreateInfo { SType = StructureType.ImageCreateInfo, ImageType = ImageType.Type2D,
            Format = _format, Extent = new(extent.Width, extent.Height, 1), MipLevels = 1, ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit, Tiling = ImageTiling.Optimal, Usage = usage,
            SharingMode = SharingMode.Exclusive, InitialLayout = ImageLayout.Undefined };
        Check(_vk.CreateImage(_device, &imageInfo, null, out slot.Backing), "backing image");
        _vk.GetImageMemoryRequirements(_device, slot.Backing, out var requirements);
        if (requirements.Size > 512UL * 1024 * 1024 / (ulong)_windowFrames.Count) throw new InvalidOperationException("Vulkan surfaces exceed the total 512 MiB backing budget.");
        _vk.GetPhysicalDeviceMemoryProperties(_physical, out var memory);
        uint memoryType = uint.MaxValue;
        for (uint i = 0; i < memory.MemoryTypeCount; i++)
            if ((requirements.MemoryTypeBits & (1u << (int)i)) != 0 &&
                (memory.MemoryTypes[(int)i].PropertyFlags & MemoryPropertyFlags.DeviceLocalBit) != 0) { memoryType = i; break; }
        if (memoryType == uint.MaxValue) throw new PlatformNotSupportedException("No device-local Vulkan image memory.");
        var allocation = new MemoryAllocateInfo { SType = StructureType.MemoryAllocateInfo,
            AllocationSize = requirements.Size, MemoryTypeIndex = memoryType };
        Check(_vk.AllocateMemory(_device, &allocation, null, out slot.Memory), "backing memory");
        Check(_vk.BindImageMemory(_device, slot.Backing, slot.Memory, 0), "bind backing memory");
        _stockObserver?.RegisterHostTarget(slot.Backing.Handle, imageInfo);
        slot.Target = _session!.CreateVulkanTarget(Width, Height, new SKGraphiteVkTextureInfo {
            SampleCount = 1, Format = (int)_format, ImageTiling = (int)ImageTiling.Optimal,
            ImageUsageFlags = (uint)usage, SharingMode = (int)SharingMode.Exclusive, AspectMask = (uint)ImageAspectFlags.ColorBit },
            (int)ImageLayout.Undefined, _family, (nint)slot.Backing.Handle,
            _format == Format.B8G8R8A8Unorm ? SKColorType.Bgra8888 : SKColorType.Rgba8888);
    }

    private void Barrier(CommandBuffer command, VkImage image, ImageLayout oldLayout, ImageLayout newLayout, AccessFlags source, AccessFlags destination)
    {
        var barrier = new ImageMemoryBarrier { SType = StructureType.ImageMemoryBarrier,
            SrcAccessMask = source, DstAccessMask = destination, OldLayout = oldLayout, NewLayout = newLayout,
            SrcQueueFamilyIndex = uint.MaxValue, DstQueueFamilyIndex = uint.MaxValue, Image = image,
            SubresourceRange = new(ImageAspectFlags.ColorBit, 0, 1, 0, 1) };
        _vk.CmdPipelineBarrier(command, PipelineStageFlags.AllCommandsBit, PipelineStageFlags.AllCommandsBit,
            0, 0, null, 0, null, 1, &barrier);
        _stockObserver?.Journal.Barrier(command.Handle, image.Handle, oldLayout, newLayout, uint.MaxValue, uint.MaxValue);
    }

    private nint GetProcedure(string name, nint instance, nint device) => device != 0
        ? _vk.GetDeviceProcAddr(new(device), name) : _vk.GetInstanceProcAddr(new(instance), name);

    private static Instance CreateInstance(Vk vk, string[] names, uint apiVersion = Api12)
    {
        var strings = names.Select(Marshal.StringToCoTaskMemUTF8).ToArray();
        try
        {
            var application = new ApplicationInfo { SType = StructureType.ApplicationInfo, ApiVersion = apiVersion };
            fixed (nint* extensions = strings)
            {
                var info = new InstanceCreateInfo { SType = StructureType.InstanceCreateInfo,
                    PApplicationInfo = &application, EnabledExtensionCount = (uint)strings.Length,
                    PpEnabledExtensionNames = (byte**)extensions };
                Check(vk.CreateInstance(&info, null, out var instance), "vkCreateInstance");
                return instance;
            }
        }
        finally { foreach (var value in strings) Marshal.FreeCoTaskMem(value); }
    }

    private void ReleaseImages(bool releaseSwapchain = true)
    {
        ResourcesReleasing?.Invoke();
        foreach (var slot in _windowFrames)
        {
            slot.Target?.Dispose(); slot.Target = null;
            if (slot.Backing.Handle != 0) _stockObserver?.ForgetHostTarget(slot.Backing.Handle);
            if (slot.Backing.Handle != 0) _vk.DestroyImage(_device, slot.Backing, null);
            if (slot.Memory.Handle != 0) _vk.FreeMemory(_device, slot.Memory, null);
            slot.Backing = default; slot.Memory = default;
        }
        _target?.Dispose(); _target = null;
        ReleaseOfficialIntermediate();
        if (_backing.Handle != 0) _vk.DestroyImage(_device, _backing, null);
        if (_memory.Handle != 0) _vk.FreeMemory(_device, _memory, null);
        if (releaseSwapchain && _swapchain.Handle != 0)
        {
            _swapchains.DestroySwapchain(_device, _swapchain, null);
            _swapchain = default;
        }
        foreach (var semaphore in _presentReady)
            if (semaphore.Handle != 0) _vk.DestroySemaphore(_device, semaphore, null);
        _presentReady = [];
        _backing = default; _memory = default;
        _images = []; _initialized = [];
    }

    private void ReleaseDevice()
    {
        if (_device.Handle == 0) return;
        ReleaseD3D12Frame();
        DrainWindowFrames();
        ReleaseImages();
        _session?.Dispose(); _session = null;
        foreach (var slot in _windowFrames)
        {
            if (slot.Fence.Handle != 0) _vk.DestroyFence(_device, slot.Fence, null);
            if (slot.Acquired.Handle != 0) _vk.DestroySemaphore(_device, slot.Acquired, null);
        }
        _windowFrames.Clear();
        if (_fence.Handle != 0) _vk.DestroyFence(_device, _fence, null);
        if (_pool.Handle != 0) _vk.DestroyCommandPool(_device, _pool, null);
        if (_stockObserver != null)
        {
            _stockObserver.Journal.FreePool(_pool.Handle);
            _stockObserver.Check(); _stockObserver.Dispose(); _stockObserver = null;
        }
        _vk.DestroyDevice(_device, null); _device = default;
    }

    public void Dispose()
    {
        if (_disposed) return;
        CheckOwner(); ReleaseDevice();
        if (_ownsSurface) _surfaces.DestroySurface(_instance, _surface, null);
        if (_ownsInstance) _vk.DestroyInstance(_instance, null);
        _swapchains?.Dispose(); _surfaces?.Dispose(); _vk.Dispose(); _disposed = true;
    }

    /// <summary>Terminal-only transfer after the host has successfully joined the render thread.</summary>
    public void TakeShutdownOwnershipAfterThreadJoined()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var result = _vk.DeviceWaitIdle(_device);
        if (result != Result.ErrorDeviceLost) Check(result, "terminal owner transfer drain");
        _stockObserver?.TakeShutdownOwnershipAfterGpuDrain();
        _session!.TakeVulkanShutdownOwnershipAfterGpuDrain();
        _owner = Environment.CurrentManagedThreadId;
        if (result == Result.ErrorDeviceLost) _session.NotifyVulkanDeviceLost();
    }

    private void CheckOwner()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_owner != Environment.CurrentManagedThreadId) throw new InvalidOperationException("Vulkan WSI accessed outside its owner thread.");
    }
    private static void Check(Result result, string operation)
    {
        if (result != Result.Success) throw new InvalidOperationException($"{operation}: {result}");
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct AndroidSurfaceInfo { public uint Type; public nint Next; public uint Flags; public nint Window; }
}
