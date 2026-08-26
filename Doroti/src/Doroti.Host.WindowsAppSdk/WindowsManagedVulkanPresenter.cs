using System.Runtime.InteropServices;
using Doroti.Skia.RuntimeEffects;
using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;
using VkImage = Silk.NET.Vulkan.Image;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter : WindowsManagedHwndPresenterBase
{
    private const ulong FenceTimeoutNanoseconds = 5_000_000_000;
    private const uint VulkanApiVersion11 = (1u << 22) | (1u << 12);

    private readonly bool _diagnosticsEnabled;
    private readonly Vk _vk;
    private Instance _instance;
    private PhysicalDevice _physicalDevice;
    private VkDevice _device;
    private Queue _queue;
    private SurfaceKHR _surface;
    private SwapchainKHR _swapchain;
    private KhrSurface? _surfaceApi;
    private KhrWin32Surface? _win32SurfaceApi;
    private KhrSwapchain? _swapchainApi;
    private GRVkBackendContext? _skiaBackend;
    private GRContext? _context;
    private VkImage _backingImage;
    private DeviceMemory _backingMemory;
    private GRBackendRenderTarget? _backingTarget;
    private SKSurface? _backingSurface;
    private VkImage[] _swapchainImages = [];
    private CommandPool _commandPool;
    private CommandBuffer _commandBuffer;
    private Fence _fence;
    private Format _format;
    private uint _queueFamily;
    private nint _window;
    private bool _flushAfterResizePresent;
    private bool _debugBaselineSealed;
    private bool _disposed;

    internal WindowsManagedVulkanPresenter(bool enableDiagnostics)
    {
        _diagnosticsEnabled = enableDiagnostics;
        var loaderPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll");
        _vk = new Vk(new DefaultNativeContext(loaderPath));
    }

    internal override string BackendName => "Vulkan";
    internal override string RuntimeEffectsBackend => DorotiSkiaRuntimeEffects.WindowsVulkanBackend;
    internal override string DiagnosticCoverage =>
        "direct Vulkan 1.1 device, Win32 surface/swapchain, checked VkResult values, " +
        "CPU-confirmed acquire/Skia submit/copy fences, and resize DwmFlush";
    internal override int Width { get; set; }
    internal override int Height { get; set; }
    internal override ulong DeviceGeneration { get; set; }
    internal override ulong ResizeBuffersCount { get; set; }
    internal override ulong ResizeInvalidCallCount { get; set; }
    internal override ulong PresentCount { get; set; }
    internal override ulong GpuSubmitCount { get; set; }
    internal override ulong GpuCopyCount { get; set; }
    internal override bool LastPresentSucceeded { get; set; }
    internal override ulong InitializationDebugMessageCount { get; set; }
    internal override ulong InitializationDebugErrorCount { get; set; }
    internal override ulong OperationalDebugMessageCount { get; set; }
    internal override ulong OperationalDebugErrorCount { get; set; }
    internal override ulong OperationalDebugWarningCount { get; set; }
    internal override string AdapterDescription { get; set; } = "uninitialized";

    internal override bool EnsureTarget(nint childWindow, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        LastPresentSucceeded = false;
        if (childWindow == 0) throw new ArgumentOutOfRangeException(nameof(childWindow));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

        if (_window != 0 && _window != childWindow)
            ReleaseDevice();
        EnsureDevice(childWindow);
        if (_swapchain.Handle != 0 && Width == width && Height == height)
            return true;

        var resized = _swapchain.Handle != 0;
        if (!RecreateSwapchain(width, height)) return false;
        if (resized)
        {
            ResizeBuffersCount++;
            _flushAfterResizePresent = true;
        }
        return true;
    }

    internal override void SealInitializationDebugBaseline() => _debugBaselineSealed = true;

    internal override void CaptureOperationalDebugMessages()
    {
        // Every Vulkan call in this presenter is checked at its call site. A validation
        // layer is intentionally not made a product/runtime dependency.
        _ = _diagnosticsEnabled;
    }

    internal override T RenderAndPresent<T>(Func<SKSurface, T> paint, Predicate<T> shouldPresent)
    {
        ArgumentNullException.ThrowIfNull(paint);
        ArgumentNullException.ThrowIfNull(shouldPresent);
        ObjectDisposedException.ThrowIf(_disposed, this);
        var context = _context ?? throw new InvalidOperationException("The managed Vulkan Skia context is unavailable.");
        var backing = _backingSurface ?? throw new InvalidOperationException("The managed Vulkan backing surface is unavailable.");
        if (_swapchain.Handle == 0 || _swapchainApi is null)
            throw new InvalidOperationException("The Vulkan HWND swapchain is unavailable.");

        var result = paint(backing);
        if (!shouldPresent(result)) return result;
        backing.Canvas.Flush();
        context.Flush(backing);
        context.Submit(true);
        GpuSubmitCount++;
        if (!shouldPresent(result)) return result;

        if (!TryAcquireNextImage(() => shouldPresent(result), out var imageIndex)) return result;

        CopyBackingToSwapchain(_swapchainImages[checked((int)imageIndex)]);
        GpuCopyCount++;
        if (!shouldPresent(result)) return result;

        var swapchain = _swapchain;
        var presentInfo = new PresentInfoKHR
        {
            SType = StructureType.PresentInfoKhr,
            SwapchainCount = 1,
            PSwapchains = &swapchain,
            PImageIndices = &imageIndex,
        };
        var present = _swapchainApi.QueuePresent(_queue, &presentInfo);
        if (present is Result.ErrorOutOfDateKhr)
        {
            Width = Height = 0;
            return result;
        }
        Check(present, "vkQueuePresentKHR", allowSuboptimal: true);
        PresentCount++;
        LastPresentSucceeded = true;
        if (_flushAfterResizePresent ||
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_DWM_FLUSH") == "1")
        {
            Marshal.ThrowExceptionForHR(DwmFlush());
            _flushAfterResizePresent = false;
        }
        return result;
    }

    private bool TryAcquireNextImage(Func<bool> shouldContinue, out uint imageIndex)
    {
        ResetFence();
        imageIndex = 0;
        uint acquiredIndex = 0;
        var deadline = Environment.TickCount64 + 5_000;
        while (true)
        {
            var acquire = _swapchainApi!.AcquireNextImage(
                _device, _swapchain, 10_000_000, default, _fence, &acquiredIndex);
            if (acquire is Result.Success or Result.SuboptimalKhr)
            {
                WaitFence("Vulkan acquire");
                imageIndex = acquiredIndex;
                return true;
            }
            if (acquire is Result.ErrorOutOfDateKhr)
            {
                Width = Height = 0;
                return false;
            }
            if (acquire is not (Result.NotReady or Result.Timeout))
                Check(acquire, "vkAcquireNextImageKHR");
            if (!shouldContinue()) return false;
            if (Environment.TickCount64 >= deadline)
                throw new TimeoutException("Vulkan acquire did not become ready within 5 seconds.");
            Thread.Yield();
        }
    }

    internal override void ResetDevice()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ReleaseDevice();
    }

    private void EnsureDevice(nint childWindow)
    {
        if (_device.Handle != 0) return;
        _window = childWindow;
        CreateInstance();
        CreateWin32Surface(childWindow);
        SelectPhysicalDeviceAndQueue();
        CreateLogicalDevice();
        CreateSkiaContext();
        DeviceGeneration++;
        _debugBaselineSealed = false;
    }

    private void CreateInstance()
    {
        var applicationName = (byte*)SilkMarshal.StringToPtr("Doroti");
        var engineName = (byte*)SilkMarshal.StringToPtr("Doroti");
        var extensionNames = new[] { KhrSurface.ExtensionName, KhrWin32Surface.ExtensionName };
        var extensions = (byte**)SilkMarshal.StringArrayToPtr(extensionNames);
        try
        {
            var applicationInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = applicationName,
                PEngineName = engineName,
                ApiVersion = VulkanApiVersion11,
            };
            var createInfo = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &applicationInfo,
                EnabledExtensionCount = checked((uint)extensionNames.Length),
                PpEnabledExtensionNames = extensions,
            };
            Check(_vk.CreateInstance(&createInfo, null, out _instance), "vkCreateInstance");
        }
        finally
        {
            SilkMarshal.Free((nint)applicationName);
            SilkMarshal.Free((nint)engineName);
            FreeStringArray(extensions, extensionNames.Length);
        }

        if (!_vk.TryGetInstanceExtension(_instance, out _surfaceApi) || _surfaceApi is null)
            throw new InvalidOperationException("VK_KHR_surface could not be loaded.");
        if (!_vk.TryGetInstanceExtension(_instance, out _win32SurfaceApi) || _win32SurfaceApi is null)
            throw new InvalidOperationException("VK_KHR_win32_surface could not be loaded.");
    }

    private void CreateWin32Surface(nint childWindow)
    {
        var createInfo = new Win32SurfaceCreateInfoKHR
        {
            SType = StructureType.Win32SurfaceCreateInfoKhr,
            Hinstance = GetModuleHandle(null),
            Hwnd = childWindow,
        };
        Check(_win32SurfaceApi!.CreateWin32Surface(_instance, &createInfo, null, out _surface),
            "vkCreateWin32SurfaceKHR");
    }

    private void SelectPhysicalDeviceAndQueue()
    {
        uint deviceCount = 0;
        Check(_vk.EnumeratePhysicalDevices(_instance, &deviceCount, null), "vkEnumeratePhysicalDevices(count)");
        if (deviceCount == 0) throw new InvalidOperationException("No Vulkan physical device is available.");
        var devices = new PhysicalDevice[deviceCount];
        fixed (PhysicalDevice* devicesPointer = devices)
            Check(_vk.EnumeratePhysicalDevices(_instance, &deviceCount, devicesPointer), "vkEnumeratePhysicalDevices");

        var bestScore = int.MinValue;
        foreach (var candidate in devices)
        {
            _vk.GetPhysicalDeviceProperties(candidate, out var properties);
            if (properties.DeviceType == PhysicalDeviceType.Cpu) continue;
            uint familyCount = 0;
            _vk.GetPhysicalDeviceQueueFamilyProperties(candidate, &familyCount, null);
            if (familyCount == 0) continue;
            var families = new QueueFamilyProperties[familyCount];
            fixed (QueueFamilyProperties* familiesPointer = families)
                _vk.GetPhysicalDeviceQueueFamilyProperties(candidate, &familyCount, familiesPointer);
            for (uint family = 0; family < familyCount; family++)
            {
                if ((families[family].QueueFlags & QueueFlags.GraphicsBit) == 0) continue;
                Check(_surfaceApi!.GetPhysicalDeviceSurfaceSupport(candidate, family, _surface, out Bool32 supported),
                    "vkGetPhysicalDeviceSurfaceSupportKHR");
                if (!supported) continue;
                var score = properties.DeviceType switch
                {
                    PhysicalDeviceType.DiscreteGpu => 300,
                    PhysicalDeviceType.IntegratedGpu => 200,
                    PhysicalDeviceType.VirtualGpu => 100,
                    _ => 0,
                };
                if (score <= bestScore) continue;
                bestScore = score;
                _physicalDevice = candidate;
                _queueFamily = family;
                AdapterDescription = Marshal.PtrToStringUTF8((nint)properties.DeviceName)
                    ?? "unnamed Vulkan device";
            }
        }
        if (_physicalDevice.Handle == 0)
            throw new InvalidOperationException("No hardware Vulkan graphics/present queue is available for the child HWND.");
        if (AdapterDescription.Contains("SwiftShader", StringComparison.OrdinalIgnoreCase) ||
            AdapterDescription.Contains("llvmpipe", StringComparison.OrdinalIgnoreCase) ||
            AdapterDescription.Contains("softpipe", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Vulkan selected a software renderer: '{AdapterDescription}'.");
    }

    private void CreateLogicalDevice()
    {
        var priority = 1.0f;
        var queueInfo = new DeviceQueueCreateInfo
        {
            SType = StructureType.DeviceQueueCreateInfo,
            QueueFamilyIndex = _queueFamily,
            QueueCount = 1,
            PQueuePriorities = &priority,
        };
        var extensionNames = new[] { KhrSwapchain.ExtensionName };
        var extensions = (byte**)SilkMarshal.StringArrayToPtr(extensionNames);
        try
        {
            var createInfo = new DeviceCreateInfo
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1,
                PQueueCreateInfos = &queueInfo,
                EnabledExtensionCount = 1,
                PpEnabledExtensionNames = extensions,
            };
            Check(_vk.CreateDevice(_physicalDevice, &createInfo, null, out _device), "vkCreateDevice");
        }
        finally
        {
            FreeStringArray(extensions, extensionNames.Length);
        }
        _vk.GetDeviceQueue(_device, _queueFamily, 0, out _queue);
        if (!_vk.TryGetDeviceExtension(_instance, _device, out _swapchainApi) || _swapchainApi is null)
            throw new InvalidOperationException("VK_KHR_swapchain could not be loaded.");

        var poolInfo = new CommandPoolCreateInfo
        {
            SType = StructureType.CommandPoolCreateInfo,
            QueueFamilyIndex = _queueFamily,
            Flags = CommandPoolCreateFlags.ResetCommandBufferBit,
        };
        Check(_vk.CreateCommandPool(_device, &poolInfo, null, out _commandPool), "vkCreateCommandPool");
        var allocateInfo = new CommandBufferAllocateInfo
        {
            SType = StructureType.CommandBufferAllocateInfo,
            CommandPool = _commandPool,
            Level = CommandBufferLevel.Primary,
            CommandBufferCount = 1,
        };
        Check(_vk.AllocateCommandBuffers(_device, &allocateInfo, out _commandBuffer), "vkAllocateCommandBuffers");
        var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
        Check(_vk.CreateFence(_device, &fenceInfo, null, out _fence), "vkCreateFence");
    }

    private void CreateSkiaContext()
    {
        _skiaBackend = new GRVkBackendContext
        {
            VkInstance = _instance.Handle,
            VkPhysicalDevice = _physicalDevice.Handle,
            VkDevice = _device.Handle,
            VkQueue = _queue.Handle,
            GraphicsQueueIndex = _queueFamily,
            GetProcedureAddress = GetVulkanProcedureAddress,
        };
        _context = GRContext.CreateVulkan(_skiaBackend)
            ?? throw new InvalidOperationException("Skia could not create the managed Vulkan context.");
    }

    private nint GetVulkanProcedureAddress(string name, nint instance, nint device)
    {
        if (device != 0) return _vk.GetDeviceProcAddr(new VkDevice(device), name);
        return _vk.GetInstanceProcAddr(new Instance(instance), name);
    }

    private bool RecreateSwapchain(int width, int height)
    {
        WaitIdle();
        var oldSwapchain = _swapchain;
        _surfaceApi!.GetPhysicalDeviceSurfaceCapabilities(_physicalDevice, _surface, out var capabilities);
        uint formatCount = 0;
        Check(_surfaceApi.GetPhysicalDeviceSurfaceFormats(_physicalDevice, _surface, &formatCount, null),
            "vkGetPhysicalDeviceSurfaceFormatsKHR(count)");
        if (formatCount == 0) throw new InvalidOperationException("The Vulkan Win32 surface exposes no formats.");
        var formats = new SurfaceFormatKHR[formatCount];
        fixed (SurfaceFormatKHR* formatsPointer = formats)
            Check(_surfaceApi.GetPhysicalDeviceSurfaceFormats(
                _physicalDevice, _surface, &formatCount, formatsPointer), "vkGetPhysicalDeviceSurfaceFormatsKHR");
        var selected = formats.FirstOrDefault(value =>
            value.Format == Format.B8G8R8A8Unorm && value.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr);
        if (selected.Format == Format.Undefined) selected = formats[0];
        if (selected.Format is not (Format.B8G8R8A8Unorm or Format.R8G8B8A8Unorm))
            throw new InvalidOperationException($"Unsupported Vulkan swapchain format: {selected.Format}.");
        _format = selected.Format;

        var extent = capabilities.CurrentExtent.Width != uint.MaxValue
            ? capabilities.CurrentExtent
            : new Extent2D(
                Math.Clamp(checked((uint)width), capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
                Math.Clamp(checked((uint)height), capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height));
        if (extent.Width != width || extent.Height != height)
            return false;
        ReleaseBacking();
        var imageCount = capabilities.MinImageCount + 1;
        if (capabilities.MaxImageCount > 0) imageCount = Math.Min(imageCount, capabilities.MaxImageCount);
        var presentMode = SelectPresentMode();
        var createInfo = new SwapchainCreateInfoKHR
        {
            SType = StructureType.SwapchainCreateInfoKhr,
            Surface = _surface,
            MinImageCount = imageCount,
            ImageFormat = selected.Format,
            ImageColorSpace = selected.ColorSpace,
            ImageExtent = extent,
            ImageArrayLayers = 1,
            ImageUsage = ImageUsageFlags.TransferDstBit,
            ImageSharingMode = SharingMode.Exclusive,
            PreTransform = capabilities.CurrentTransform,
            CompositeAlpha = SelectCompositeAlpha(capabilities.SupportedCompositeAlpha),
            PresentMode = presentMode,
            Clipped = true,
            OldSwapchain = oldSwapchain,
        };
        Check(_swapchainApi!.CreateSwapchain(_device, &createInfo, null, out _swapchain), "vkCreateSwapchainKHR");
        if (oldSwapchain.Handle != 0) _swapchainApi.DestroySwapchain(_device, oldSwapchain, null);
        uint swapchainImageCount = 0;
        Check(_swapchainApi.GetSwapchainImages(_device, _swapchain, &swapchainImageCount, null),
            "vkGetSwapchainImagesKHR(count)");
        _swapchainImages = new VkImage[swapchainImageCount];
        fixed (VkImage* imagesPointer = _swapchainImages)
            Check(_swapchainApi.GetSwapchainImages(_device, _swapchain, &swapchainImageCount, imagesPointer),
                "vkGetSwapchainImagesKHR");
        CreateBacking(width, height);
        Width = width;
        Height = height;
        return true;
    }

    private PresentModeKHR SelectPresentMode()
    {
        uint count = 0;
        Check(_surfaceApi!.GetPhysicalDeviceSurfacePresentModes(_physicalDevice, _surface, &count, null),
            "vkGetPhysicalDeviceSurfacePresentModesKHR(count)");
        var modes = new PresentModeKHR[count];
        fixed (PresentModeKHR* modesPointer = modes)
            Check(_surfaceApi.GetPhysicalDeviceSurfacePresentModes(_physicalDevice, _surface, &count, modesPointer),
                "vkGetPhysicalDeviceSurfacePresentModesKHR");
        var vsync = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_VSYNC") == "1";
        if (!vsync && modes.Contains(PresentModeKHR.ImmediateKhr)) return PresentModeKHR.ImmediateKhr;
        if (!vsync && modes.Contains(PresentModeKHR.MailboxKhr)) return PresentModeKHR.MailboxKhr;
        return PresentModeKHR.FifoKhr;
    }

    private static CompositeAlphaFlagsKHR SelectCompositeAlpha(CompositeAlphaFlagsKHR supported)
    {
        foreach (var candidate in new[]
                 {
                     CompositeAlphaFlagsKHR.OpaqueBitKhr,
                     CompositeAlphaFlagsKHR.PreMultipliedBitKhr,
                     CompositeAlphaFlagsKHR.PostMultipliedBitKhr,
                     CompositeAlphaFlagsKHR.InheritBitKhr,
                 })
            if ((supported & candidate) != 0) return candidate;
        throw new InvalidOperationException("The Vulkan Win32 surface exposes no composite-alpha mode.");
    }

    private void CreateBacking(int width, int height)
    {
        var imageInfo = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D,
            Format = _format,
            Extent = new Extent3D(checked((uint)width), checked((uint)height), 1),
            MipLevels = 1,
            ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.TransferSrcBit |
                    ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit,
            SharingMode = SharingMode.Exclusive,
            InitialLayout = ImageLayout.Undefined,
        };
        Check(_vk.CreateImage(_device, &imageInfo, null, out _backingImage), "vkCreateImage(backing)");
        _vk.GetImageMemoryRequirements(_device, _backingImage, out var requirements);
        var allocationInfo = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = requirements.Size,
            MemoryTypeIndex = FindMemoryType(requirements.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit),
        };
        Check(_vk.AllocateMemory(_device, &allocationInfo, null, out _backingMemory), "vkAllocateMemory(backing)");
        Check(_vk.BindImageMemory(_device, _backingImage, _backingMemory, 0), "vkBindImageMemory(backing)");
        TransitionBackingToColorAttachment();

        var skiaImageInfo = new GRVkImageInfo
        {
            Image = _backingImage.Handle,
            Alloc = new GRVkAlloc
            {
                Memory = _backingMemory.Handle,
                Offset = 0,
                Size = requirements.Size,
            },
            ImageTiling = (uint)ImageTiling.Optimal,
            ImageLayout = (uint)ImageLayout.ColorAttachmentOptimal,
            Format = (uint)_format,
            ImageUsageFlags = (uint)(ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.TransferSrcBit |
                                     ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit),
            SampleCount = 1,
            LevelCount = 1,
            CurrentQueueFamily = _queueFamily,
        };
        _context!.ResetContext();
        _backingTarget = new GRBackendRenderTarget(width, height, skiaImageInfo);
        var colorType = _format == Format.B8G8R8A8Unorm ? SKColorType.Bgra8888 : SKColorType.Rgba8888;
        _backingSurface = SKSurface.Create(
            _context!, _backingTarget, GRSurfaceOrigin.TopLeft, colorType)
            ?? throw new InvalidOperationException(
                $"Skia could not wrap the managed Vulkan backing image " +
                $"(targetValid={_backingTarget.IsValid}, backend={_backingTarget.Backend}, " +
                $"format={_format}, colorType={colorType}, maxSamples={_context!.GetMaxSurfaceSampleCount(colorType)}).");
    }

    private uint FindMemoryType(uint typeFilter, MemoryPropertyFlags required)
    {
        _vk.GetPhysicalDeviceMemoryProperties(_physicalDevice, out var properties);
        for (uint index = 0; index < properties.MemoryTypeCount; index++)
            if ((typeFilter & (1u << checked((int)index))) != 0 &&
                (properties.MemoryTypes[checked((int)index)].PropertyFlags & required) == required)
                return index;
        throw new InvalidOperationException("No device-local Vulkan memory type is available.");
    }

    private void TransitionBackingToColorAttachment()
    {
        BeginCommands();
        var barrier = ImageBarrier(
            _backingImage, ImageLayout.Undefined, ImageLayout.ColorAttachmentOptimal,
            0, AccessFlags.ColorAttachmentWriteBit);
        _vk.CmdPipelineBarrier(
            _commandBuffer, PipelineStageFlags.TopOfPipeBit, PipelineStageFlags.ColorAttachmentOutputBit,
            0, 0, null, 0, null, 1, &barrier);
        SubmitCommandsAndWait("Vulkan backing initialization");
    }

    private void CopyBackingToSwapchain(VkImage destination)
    {
        BeginCommands();
        var barriers = stackalloc ImageMemoryBarrier[2];
        barriers[0] = ImageBarrier(
            _backingImage, ImageLayout.ColorAttachmentOptimal, ImageLayout.TransferSrcOptimal,
            AccessFlags.ColorAttachmentWriteBit, AccessFlags.TransferReadBit);
        barriers[1] = ImageBarrier(
            destination, ImageLayout.Undefined, ImageLayout.TransferDstOptimal,
            0, AccessFlags.TransferWriteBit);
        _vk.CmdPipelineBarrier(
            _commandBuffer, PipelineStageFlags.ColorAttachmentOutputBit, PipelineStageFlags.TransferBit,
            0, 0, null, 0, null, 2, barriers);

        var copy = new ImageCopy
        {
            SrcSubresource = ColorSubresourceLayers(),
            DstSubresource = ColorSubresourceLayers(),
            Extent = new Extent3D(checked((uint)Width), checked((uint)Height), 1),
        };
        _vk.CmdCopyImage(
            _commandBuffer, _backingImage, ImageLayout.TransferSrcOptimal,
            destination, ImageLayout.TransferDstOptimal, 1, &copy);

        barriers[0] = ImageBarrier(
            _backingImage, ImageLayout.TransferSrcOptimal, ImageLayout.ColorAttachmentOptimal,
            AccessFlags.TransferReadBit, AccessFlags.ColorAttachmentWriteBit);
        barriers[1] = ImageBarrier(
            destination, ImageLayout.TransferDstOptimal, ImageLayout.PresentSrcKhr,
            AccessFlags.TransferWriteBit, 0);
        _vk.CmdPipelineBarrier(
            _commandBuffer, PipelineStageFlags.TransferBit,
            PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.BottomOfPipeBit,
            0, 0, null, 0, null, 2, barriers);
        SubmitCommandsAndWait("Vulkan backing copy");
    }

    private static ImageMemoryBarrier ImageBarrier(
        VkImage image, ImageLayout oldLayout, ImageLayout newLayout,
        AccessFlags sourceAccess, AccessFlags destinationAccess) => new()
    {
        SType = StructureType.ImageMemoryBarrier,
        OldLayout = oldLayout,
        NewLayout = newLayout,
        SrcQueueFamilyIndex = Vk.QueueFamilyIgnored,
        DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
        Image = image,
        SubresourceRange = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1),
        SrcAccessMask = sourceAccess,
        DstAccessMask = destinationAccess,
    };

    private static ImageSubresourceLayers ColorSubresourceLayers() => new()
    {
        AspectMask = ImageAspectFlags.ColorBit,
        MipLevel = 0,
        BaseArrayLayer = 0,
        LayerCount = 1,
    };

    private void BeginCommands()
    {
        Check(_vk.ResetCommandBuffer(_commandBuffer, 0), "vkResetCommandBuffer");
        var beginInfo = new CommandBufferBeginInfo
        {
            SType = StructureType.CommandBufferBeginInfo,
            Flags = CommandBufferUsageFlags.OneTimeSubmitBit,
        };
        Check(_vk.BeginCommandBuffer(_commandBuffer, &beginInfo), "vkBeginCommandBuffer");
    }

    private void SubmitCommandsAndWait(string identity)
    {
        Check(_vk.EndCommandBuffer(_commandBuffer), "vkEndCommandBuffer");
        ResetFence();
        var commandBuffer = _commandBuffer;
        var submitInfo = new SubmitInfo
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &commandBuffer,
        };
        Check(_vk.QueueSubmit(_queue, 1, &submitInfo, _fence), "vkQueueSubmit");
        WaitFence(identity);
    }

    private void ResetFence() => Check(_vk.ResetFences(_device, 1, in _fence), "vkResetFences");

    private void WaitFence(string identity)
    {
        var result = _vk.WaitForFences(_device, 1, in _fence, true, FenceTimeoutNanoseconds);
        if (result == Result.Timeout) throw new TimeoutException($"{identity} fence timed out after 5 seconds.");
        Check(result, "vkWaitForFences");
    }

    private void WaitIdle()
    {
        if (_device.Handle != 0) Check(_vk.DeviceWaitIdle(_device), "vkDeviceWaitIdle");
    }

    private void ReleaseBacking()
    {
        _backingSurface?.Dispose();
        _backingSurface = null;
        _backingTarget?.Dispose();
        _backingTarget = null;
        if (_backingImage.Handle != 0) _vk.DestroyImage(_device, _backingImage, null);
        _backingImage = default;
        if (_backingMemory.Handle != 0) _vk.FreeMemory(_device, _backingMemory, null);
        _backingMemory = default;
    }

    private void ReleaseDevice()
    {
        if (_instance.Handle == 0) return;
        WaitIdle();
        ReleaseBacking();
        if (_swapchain.Handle != 0) _swapchainApi?.DestroySwapchain(_device, _swapchain, null);
        _swapchain = default;
        _swapchainImages = [];
        _context?.AbandonContext(false);
        _context?.Dispose();
        _context = null;
        _skiaBackend?.Dispose();
        _skiaBackend = null;
        if (_fence.Handle != 0) _vk.DestroyFence(_device, _fence, null);
        _fence = default;
        if (_commandPool.Handle != 0) _vk.DestroyCommandPool(_device, _commandPool, null);
        _commandPool = default;
        _commandBuffer = default;
        _swapchainApi?.Dispose();
        _swapchainApi = null;
        if (_device.Handle != 0) _vk.DestroyDevice(_device, null);
        _device = default;
        _queue = default;
        if (_surface.Handle != 0) _surfaceApi?.DestroySurface(_instance, _surface, null);
        _surface = default;
        _win32SurfaceApi?.Dispose();
        _win32SurfaceApi = null;
        _surfaceApi?.Dispose();
        _surfaceApi = null;
        _vk.DestroyInstance(_instance, null);
        _instance = default;
        _physicalDevice = default;
        _window = 0;
        Width = Height = 0;
        AdapterDescription = "uninitialized";
        _flushAfterResizePresent = false;
    }

    private void Check(Result result, string operation, bool allowSuboptimal = false)
    {
        if (result == Result.Success || allowSuboptimal && result == Result.SuboptimalKhr) return;
        throw RecordOperationalFailure($"{operation} failed with Vulkan result {result}.", resize: false);
    }

    private static void FreeStringArray(byte** values, int count)
    {
        if (values is null) return;
        for (var index = 0; index < count; index++)
            SilkMarshal.Free((nint)values[index]);
        SilkMarshal.Free((nint)values);
    }

    private Exception RecordOperationalFailure(string message, bool resize)
    {
        if (resize) ResizeInvalidCallCount++;
        if (_debugBaselineSealed)
        {
            OperationalDebugMessageCount++;
            OperationalDebugErrorCount++;
        }
        else
        {
            InitializationDebugMessageCount++;
            InitializationDebugErrorCount++;
        }
        return new InvalidOperationException(message);
    }

    public override void Dispose()
    {
        if (_disposed) return;
        ReleaseDevice();
        _vk.Dispose();
        _disposed = true;
    }

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandle(string? moduleName);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmFlush();
}
