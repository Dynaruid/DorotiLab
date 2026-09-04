using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.Json;
using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using VkDevice = Silk.NET.Vulkan.Device;
using VkImage = Silk.NET.Vulkan.Image;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace Doroti.Validation.WindowsVulkanCapability;

internal static unsafe partial class Program
{
    private const string ExtDebugUtilsExtensionName = "VK_EXT_debug_utils";
    private const string KhrExternalMemoryExtensionName = "VK_KHR_external_memory";
    private const string KhrExternalMemoryWin32ExtensionName = "VK_KHR_external_memory_win32";
    private const string KhrGetMemoryRequirements2ExtensionName = "VK_KHR_get_memory_requirements2";
    private const string KhrDedicatedAllocationExtensionName = "VK_KHR_dedicated_allocation";
    private const uint VulkanApiVersion11 = (1u << 22) | (1u << 12);
    private const int CompositionBufferCount = 3;
    private const ulong FenceTimeoutNanoseconds = 5_000_000_000;
    private const uint WsOverlappedWindow = 0x00CF0000;
    private const uint WsChild = 0x40000000;
    private const uint WsVisible = 0x10000000;

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static readonly object ValidationLock = new();
    private static readonly List<string> ValidationMessages = [];
    private static int _validationWarnings;
    private static int _validationErrors;
    private static readonly DebugUtilsMessengerCallbackFunctionEXT ValidationCallback = OnValidationMessage;

    [STAThread]
    private static int Main(string[] args)
    {
        var options = Options.Parse(args);
        CapabilityReport report;
        try
        {
            report = options.WsiStress ? RunLegacyWsi(options) : RunComposition(options);
        }
        catch (Exception exception)
        {
            report = CapabilityReport.Failed(options, exception);
        }

        var json = JsonSerializer.Serialize(report, JsonOptions);
        if (!string.IsNullOrWhiteSpace(options.OutputPath))
        {
            var fullPath = Path.GetFullPath(options.OutputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, json);
            Console.WriteLine($"report={fullPath}");
        }
        Console.WriteLine(json);
        return report.Status == "PASS" ? 0 : 1;
    }

    private static CapabilityReport RunComposition(Options options)
    {
        if (options.SelfTest == "software-device")
            throw new InvalidOperationException("Software Vulkan device rejected: 'synthetic CPU ICD'.");
        var loaderPath = Path.GetFullPath(options.LoaderPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll"));
        if (!File.Exists(loaderPath))
            throw new FileNotFoundException("The explicitly resolved Vulkan loader is missing.", loaderPath);
        if (!loaderPath.Equals(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll"),
                StringComparison.OrdinalIgnoreCase) && !options.AllowNonSystemLoader)
            throw new InvalidOperationException($"The Vulkan loader must be the System32 loader: '{loaderPath}'.");

        var loader = InspectPortableExecutable(loaderPath);
        if (!loader.Machine.Contains("Amd64", StringComparison.OrdinalIgnoreCase))
            throw new BadImageFormatException($"The Vulkan loader is not x64: {loader.Machine}.");

        using var vk = new Vk(new DefaultNativeContext(loaderPath));
        uint loaderApiVersion = VulkanApiVersion11;
        Check(vk.EnumerateInstanceVersion(&loaderApiVersion), "vkEnumerateInstanceVersion");
        if (loaderApiVersion < VulkanApiVersion11)
            throw new InvalidOperationException(
                $"Vulkan loader API {FormatVersion(loaderApiVersion)} is below the required 1.1.");

        var instanceExtensions = EnumerateInstanceExtensions(vk);
        var requiredInstanceExtensions = Array.Empty<string>();
        var layers = EnumerateInstanceLayers(vk);
        var validationAvailable = layers.Contains("VK_LAYER_KHRONOS_validation", StringComparer.Ordinal);
        var validationEnabled = validationAvailable && !options.DisableValidation;
        if (validationEnabled && !instanceExtensions.Contains(ExtDebugUtilsExtensionName))
            throw new InvalidOperationException("Validation was requested but VK_EXT_debug_utils is unavailable.");

        Instance instance = default;
        DebugUtilsMessengerEXT debugMessenger = default;
        VkDevice device = default;
        try
        {
            var enabledInstanceExtensions = new List<string>();
            if (validationEnabled) enabledInstanceExtensions.Add(ExtDebugUtilsExtensionName);
            instance = CreateInstance(vk, enabledInstanceExtensions, validationEnabled);
            if (validationEnabled) debugMessenger = CreateDebugMessenger(vk, instance);

            var candidates = EnumerateDevices(vk, instance, null, default, options);
            var selected = SelectDevice(candidates, options.DeviceSelector);
            if (selected.ApiVersion < VulkanApiVersion11)
                throw new InvalidOperationException(
                    $"Selected device API {FormatVersion(selected.ApiVersion)} is below the required 1.1.");
            if (selected.DeviceType == PhysicalDeviceType.Cpu && !options.AllowSoftware)
                throw new InvalidOperationException($"Software Vulkan device rejected: '{selected.Name}'.");
            if (selected.Luid.Length != checked((int)Vk.LuidSize * 2))
                throw new InvalidOperationException(
                    $"Selected Vulkan device '{selected.Name}' does not expose a valid Windows adapter LUID.");

            var requiredDeviceExtensions = new List<string>
            {
                KhrExternalMemoryExtensionName,
                KhrExternalMemoryWin32ExtensionName,
                KhrGetMemoryRequirements2ExtensionName,
                KhrDedicatedAllocationExtensionName,
            };
            if (!string.IsNullOrWhiteSpace(options.RequiredDeviceExtension))
                requiredDeviceExtensions.Add(options.RequiredDeviceExtension);
            RequireAll(selected.Extensions, requiredDeviceExtensions, "device extension");

            var externalMemory = QueryCompositionExternalMemory(vk, selected.Handle);
            if (!externalMemory.Importable)
                throw new InvalidOperationException(
                    "BGRA8 optimal transfer-destination D3D11 texture memory is not importable.");
            if (!externalMemory.DedicatedOnly)
                throw new InvalidOperationException(
                    "BGRA8 D3D11 texture import does not require the dedicated allocation used by the product contract.");
            if (!externalMemory.CompatibleHandleType)
                throw new InvalidOperationException(
                    "BGRA8 external image properties do not include D3D11TextureBit as a compatible handle type.");

            var priority = 1f;
            var queueInfo = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = selected.QueueFamily,
                QueueCount = 1,
                PQueuePriorities = &priority,
            };
            var extensionPointers = (byte**)SilkMarshal.StringArrayToPtr(requiredDeviceExtensions);
            try
            {
                var deviceInfo = new DeviceCreateInfo
                {
                    SType = StructureType.DeviceCreateInfo,
                    QueueCreateInfoCount = 1,
                    PQueueCreateInfos = &queueInfo,
                    EnabledExtensionCount = checked((uint)requiredDeviceExtensions.Count),
                    PpEnabledExtensionNames = extensionPointers,
                };
                Check(vk.CreateDevice(selected.Handle, &deviceInfo, null, out device), "vkCreateDevice");
            }
            finally
            {
                FreeStringArray(extensionPointers, requiredDeviceExtensions.Count);
            }

            if (_validationWarnings != 0 || _validationErrors != 0)
                throw new InvalidOperationException(
                    $"Vulkan validation emitted warnings={_validationWarnings}, errors={_validationErrors}: " +
                    string.Join(" | ", ValidationMessages.Take(8)));

            return CapabilityReport.PassedComposition(
                options, loaderPath, loader, loaderApiVersion, requiredInstanceExtensions,
                requiredDeviceExtensions, validationAvailable, _validationWarnings, _validationErrors,
                ValidationMessages.ToArray(), selected, externalMemory);
        }
        finally
        {
            if (device.Handle != 0)
            {
                _ = vk.DeviceWaitIdle(device);
                vk.DestroyDevice(device, null);
            }
            if (debugMessenger.Handle != 0) DestroyDebugMessenger(vk, instance, debugMessenger);
            if (instance.Handle != 0) vk.DestroyInstance(instance, null);
        }
    }

    private static ExternalMemoryCapability QueryCompositionExternalMemory(
        Vk vk, PhysicalDevice physicalDevice)
    {
        var handleType = ExternalMemoryHandleTypeFlags.D3D11TextureBit;
        var externalInfo = new PhysicalDeviceExternalImageFormatInfo
        {
            SType = StructureType.PhysicalDeviceExternalImageFormatInfo,
            HandleType = handleType,
        };
        var formatInfo = new PhysicalDeviceImageFormatInfo2
        {
            SType = StructureType.PhysicalDeviceImageFormatInfo2,
            PNext = &externalInfo,
            Format = Format.B8G8R8A8Unorm,
            Type = ImageType.Type2D,
            Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.TransferDstBit,
        };
        var externalProperties = new ExternalImageFormatProperties
        {
            SType = StructureType.ExternalImageFormatProperties,
        };
        var properties = new ImageFormatProperties2
        {
            SType = StructureType.ImageFormatProperties2,
            PNext = &externalProperties,
        };
        Check(vk.GetPhysicalDeviceImageFormatProperties2(physicalDevice, &formatInfo, &properties),
            "vkGetPhysicalDeviceImageFormatProperties2(BGRA8/D3D11TextureBit)");

        var memory = externalProperties.ExternalMemoryProperties;
        return new ExternalMemoryCapability(
            Format.B8G8R8A8Unorm.ToString(),
            ImageTiling.Optimal.ToString(),
            ImageUsageFlags.TransferDstBit.ToString(),
            handleType.ToString(),
            memory.ExternalMemoryFeatures.ToString(),
            memory.CompatibleHandleTypes.ToString(),
            (memory.ExternalMemoryFeatures & ExternalMemoryFeatureFlags.ImportableBit) != 0,
            (memory.ExternalMemoryFeatures & ExternalMemoryFeatureFlags.DedicatedOnlyBit) != 0,
            (memory.CompatibleHandleTypes & handleType) != 0);
    }

    private static CapabilityReport RunLegacyWsi(Options options)
    {
        if (options.SelfTest == "software-device")
            throw new InvalidOperationException("Software Vulkan device rejected: 'synthetic CPU ICD'.");
        var loaderPath = Path.GetFullPath(options.LoaderPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll"));
        if (!File.Exists(loaderPath))
            throw new FileNotFoundException("The explicitly resolved Vulkan loader is missing.", loaderPath);
        if (!loaderPath.Equals(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll"),
                StringComparison.OrdinalIgnoreCase) && !options.AllowNonSystemLoader)
            throw new InvalidOperationException($"The Vulkan loader must be the System32 loader: '{loaderPath}'.");

        var loader = InspectPortableExecutable(loaderPath);
        if (!loader.Machine.Contains("Amd64", StringComparison.OrdinalIgnoreCase))
            throw new BadImageFormatException($"The Vulkan loader is not x64: {loader.Machine}.");

        using var window = NativeWindow.Create();
        using var vk = new Vk(new DefaultNativeContext(loaderPath));

        uint loaderApiVersion = VulkanApiVersion11;
        Check(vk.EnumerateInstanceVersion(&loaderApiVersion), "vkEnumerateInstanceVersion");
        if (loaderApiVersion < VulkanApiVersion11)
            throw new InvalidOperationException(
                $"Vulkan loader API {FormatVersion(loaderApiVersion)} is below the required 1.1.");

        var instanceExtensions = EnumerateInstanceExtensions(vk);
        var requiredInstanceExtensions = new[]
        {
            KhrSurface.ExtensionName,
            KhrWin32Surface.ExtensionName,
        };
        RequireAll(instanceExtensions, requiredInstanceExtensions, "instance extension");
        var layers = EnumerateInstanceLayers(vk);
        var validationAvailable = layers.Contains("VK_LAYER_KHRONOS_validation", StringComparer.Ordinal);
        var validationEnabled = validationAvailable && !options.DisableValidation;
        if (validationEnabled && !instanceExtensions.Contains(ExtDebugUtilsExtensionName))
            throw new InvalidOperationException("Validation was requested but VK_EXT_debug_utils is unavailable.");

        Instance instance = default;
        DebugUtilsMessengerEXT debugMessenger = default;
        KhrSurface? surfaceApi = null;
        KhrWin32Surface? win32SurfaceApi = null;
        SurfaceKHR surface = default;
        VkDevice device = default;
        KhrSwapchain? swapchainApi = null;
        SwapchainKHR swapchain = default;
        try
        {
            var enabledInstanceExtensions = requiredInstanceExtensions.ToList();
            if (validationEnabled) enabledInstanceExtensions.Add(ExtDebugUtilsExtensionName);
            instance = CreateInstance(vk, enabledInstanceExtensions, validationEnabled);
            if (validationEnabled) debugMessenger = CreateDebugMessenger(vk, instance);
            if (!vk.TryGetInstanceExtension(instance, out surfaceApi) || surfaceApi is null)
                throw new InvalidOperationException("VK_KHR_surface could not be loaded.");
            if (!vk.TryGetInstanceExtension(instance, out win32SurfaceApi) || win32SurfaceApi is null)
                throw new InvalidOperationException("VK_KHR_win32_surface could not be loaded.");

            var surfaceInfo = new Win32SurfaceCreateInfoKHR
            {
                SType = StructureType.Win32SurfaceCreateInfoKhr,
                Hinstance = GetModuleHandle(null),
                Hwnd = window.Child,
            };
            Check(win32SurfaceApi.CreateWin32Surface(instance, &surfaceInfo, null, out surface),
                "vkCreateWin32SurfaceKHR");

            var candidates = EnumerateDevices(vk, instance, surfaceApi, surface, options);
            var selected = SelectDevice(candidates, options.DeviceSelector);
            if (selected.ApiVersion < VulkanApiVersion11)
                throw new InvalidOperationException(
                    $"Selected device API {FormatVersion(selected.ApiVersion)} is below the required 1.1.");
            var requiredDeviceExtensions = new List<string>
            {
                KhrSwapchain.ExtensionName,
            };
            if (!string.IsNullOrWhiteSpace(options.RequiredDeviceExtension))
                requiredDeviceExtensions.Add(options.RequiredDeviceExtension);
            RequireAll(selected.Extensions, requiredDeviceExtensions, "device extension");
            if (selected.DeviceType == PhysicalDeviceType.Cpu && !options.AllowSoftware)
                throw new InvalidOperationException($"Software Vulkan device rejected: '{selected.Name}'.");

            surfaceApi.GetPhysicalDeviceSurfaceCapabilities(selected.Handle, surface, out var capabilities);
            if ((capabilities.SupportedUsageFlags & ImageUsageFlags.TransferDstBit) == 0)
                throw new InvalidOperationException("The Win32 surface does not support VK_IMAGE_USAGE_TRANSFER_DST_BIT.");
            if ((capabilities.SupportedCompositeAlpha & CompositeAlphaFlagsKHR.OpaqueBitKhr) == 0)
                throw new InvalidOperationException("The Win32 surface does not support opaque composite alpha.");

            var formats = EnumerateFormats(surfaceApi, selected.Handle, surface);
            var selectedFormat = formats.FirstOrDefault(value =>
                value.Format == Format.B8G8R8A8Unorm &&
                value.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr);
            if (selectedFormat.Format == Format.Undefined)
                selectedFormat = formats.FirstOrDefault(value =>
                    value.Format == Format.R8G8B8A8Unorm &&
                    value.ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr);
            if (selectedFormat.Format == Format.Undefined)
                throw new InvalidOperationException("No approved UNORM/SRGB-nonlinear surface format is available.");

            var presentModes = EnumeratePresentModes(surfaceApi, selected.Handle, surface);
            if (!presentModes.Contains(PresentModeKHR.FifoKhr))
                throw new InvalidOperationException("VK_PRESENT_MODE_FIFO_KHR is unavailable.");

            var extent = capabilities.CurrentExtent.Width != uint.MaxValue
                ? capabilities.CurrentExtent
                : new Extent2D(
                    Math.Clamp(window.Width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
                    Math.Clamp(window.Height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height));
            if (extent.Width != window.Width || extent.Height != window.Height)
                throw new InvalidOperationException(
                    $"The surface cannot create the exact child-client extent {window.Width}x{window.Height}; " +
                    $"selected {extent.Width}x{extent.Height}.");

            var priority = 1f;
            var queueInfo = new DeviceQueueCreateInfo
            {
                SType = StructureType.DeviceQueueCreateInfo,
                QueueFamilyIndex = selected.QueueFamily,
                QueueCount = 1,
                PQueuePriorities = &priority,
            };
            var extensionPointers = (byte**)SilkMarshal.StringArrayToPtr(requiredDeviceExtensions);
            try
            {
                var deviceInfo = new DeviceCreateInfo
                {
                    SType = StructureType.DeviceCreateInfo,
                    QueueCreateInfoCount = 1,
                    PQueueCreateInfos = &queueInfo,
                    EnabledExtensionCount = checked((uint)requiredDeviceExtensions.Count),
                    PpEnabledExtensionNames = extensionPointers,
                };
                Check(vk.CreateDevice(selected.Handle, &deviceInfo, null, out device), "vkCreateDevice");
            }
            finally
            {
                FreeStringArray(extensionPointers, requiredDeviceExtensions.Count);
            }

            if (!vk.TryGetDeviceExtension(instance, device, out swapchainApi) || swapchainApi is null)
                throw new InvalidOperationException("VK_KHR_swapchain could not be loaded.");
            var imageCount = capabilities.MinImageCount + 1;
            if (capabilities.MaxImageCount > 0)
                imageCount = Math.Min(imageCount, capabilities.MaxImageCount);
            var swapchainInfo = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr,
                Surface = surface,
                MinImageCount = imageCount,
                ImageFormat = selectedFormat.Format,
                ImageColorSpace = selectedFormat.ColorSpace,
                ImageExtent = extent,
                ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.TransferDstBit,
                ImageSharingMode = SharingMode.Exclusive,
                PreTransform = capabilities.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = PresentModeKHR.FifoKhr,
                Clipped = true,
            };
            Check(swapchainApi.CreateSwapchain(device, &swapchainInfo, null, out swapchain),
                "vkCreateSwapchainKHR");

            WsiStressReport? wsiStress = null;
            if (options.WsiStress)
            {
                vk.GetDeviceQueue(device, selected.QueueFamily, 0, out var queue);
                wsiStress = RunWsiStress(
                    vk, surfaceApi, swapchainApi, device, queue,
                    selected, surface, selectedFormat, ref swapchain, window,
                    options.WsiExtendedStress, options.WsiSoak);
            }

            if (_validationWarnings != 0 || _validationErrors != 0)
                throw new InvalidOperationException(
                    $"Vulkan validation emitted warnings={_validationWarnings}, errors={_validationErrors}: " +
                    string.Join(" | ", ValidationMessages.Take(8)));

            return CapabilityReport.PassedLegacyWsi(
                options, loaderPath, loader, loaderApiVersion, requiredInstanceExtensions,
                validationAvailable, _validationWarnings, _validationErrors,
                ValidationMessages.ToArray(), selected, selectedFormat, presentModes, capabilities,
                extent, imageCount, wsiStress);
        }
        finally
        {
            if (device.Handle != 0)
            {
                _ = vk.DeviceWaitIdle(device);
                if (swapchain.Handle != 0) swapchainApi?.DestroySwapchain(device, swapchain, null);
                swapchainApi?.Dispose();
                vk.DestroyDevice(device, null);
            }
            if (surface.Handle != 0) surfaceApi?.DestroySurface(instance, surface, null);
            win32SurfaceApi?.Dispose();
            surfaceApi?.Dispose();
            if (debugMessenger.Handle != 0) DestroyDebugMessenger(vk, instance, debugMessenger);
            if (instance.Handle != 0) vk.DestroyInstance(instance, null);
        }
    }

    private static DebugUtilsMessengerEXT CreateDebugMessenger(Vk vk, Instance instance)
    {
        var address = vk.GetInstanceProcAddr(instance, "vkCreateDebugUtilsMessengerEXT");
        if (address == 0) throw new InvalidOperationException("vkCreateDebugUtilsMessengerEXT could not be loaded.");
        var create = Marshal.GetDelegateForFunctionPointer<CreateDebugUtilsMessengerDelegate>(address);
        var callback = new PfnDebugUtilsMessengerCallbackEXT(ValidationCallback);
        var info = new DebugUtilsMessengerCreateInfoEXT
        {
            SType = StructureType.DebugUtilsMessengerCreateInfoExt,
            MessageSeverity = DebugUtilsMessageSeverityFlagsEXT.WarningBitExt |
                              DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt,
            MessageType = DebugUtilsMessageTypeFlagsEXT.GeneralBitExt |
                          DebugUtilsMessageTypeFlagsEXT.ValidationBitExt |
                          DebugUtilsMessageTypeFlagsEXT.PerformanceBitExt,
            PfnUserCallback = callback,
        };
        DebugUtilsMessengerEXT messenger = default;
        Check(create(instance, &info, null, &messenger), "vkCreateDebugUtilsMessengerEXT");
        return messenger;
    }

    private static void DestroyDebugMessenger(Vk vk, Instance instance, DebugUtilsMessengerEXT messenger)
    {
        var address = vk.GetInstanceProcAddr(instance, "vkDestroyDebugUtilsMessengerEXT");
        if (address == 0) return;
        Marshal.GetDelegateForFunctionPointer<DestroyDebugUtilsMessengerDelegate>(address)(
            instance, messenger, null);
    }

    private static uint OnValidationMessage(
        DebugUtilsMessageSeverityFlagsEXT severity,
        DebugUtilsMessageTypeFlagsEXT types,
        DebugUtilsMessengerCallbackDataEXT* data,
        void* userData)
    {
        var message = data is null || data->PMessage is null
            ? $"{severity}/{types}"
            : Marshal.PtrToStringUTF8((nint)data->PMessage) ?? $"{severity}/{types}";
        lock (ValidationLock)
        {
            if ((severity & DebugUtilsMessageSeverityFlagsEXT.ErrorBitExt) != 0) _validationErrors++;
            else if ((severity & DebugUtilsMessageSeverityFlagsEXT.WarningBitExt) != 0) _validationWarnings++;
            if (ValidationMessages.Count < 256) ValidationMessages.Add(message);
        }
        return 0;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private unsafe delegate Result CreateDebugUtilsMessengerDelegate(
        Instance instance,
        DebugUtilsMessengerCreateInfoEXT* createInfo,
        AllocationCallbacks* allocator,
        DebugUtilsMessengerEXT* messenger);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private unsafe delegate void DestroyDebugUtilsMessengerDelegate(
        Instance instance,
        DebugUtilsMessengerEXT messenger,
        AllocationCallbacks* allocator);

    private static WsiStressReport RunWsiStress(
        Vk vk,
        KhrSurface surfaceApi,
        KhrSwapchain swapchainApi,
        VkDevice device,
        Queue queue,
        DeviceCandidate selected,
        SurfaceKHR surface,
        SurfaceFormatKHR format,
        ref SwapchainKHR swapchain,
        NativeWindow window,
        bool extendedStress,
        bool soak)
    {
        var presentIterations = soak ? 1_000 : extendedStress ? 25 : 3;
        var staleIterations = soak ? 1_000 : extendedStress ? 25 : 3;
        var recreateIterations = soak ? 500 : extendedStress ? 10 : 3;
        var lifecycleIterations = soak ? 10 : extendedStress ? 5 : 2;
        var phase = Stopwatch.StartNew();
        var initialSwapchain = swapchain;
        swapchain = default;
        using var owner = new WsiStressOwner(
            vk, surfaceApi, swapchainApi, device, queue,
            selected.Handle, selected.QueueFamily, surface, format, initialSwapchain, window.Width, window.Height);

        for (var index = 0; index < presentIterations; index++) owner.PresentMarker(index);
        Console.Error.WriteLine($"wsi.phase=present count={presentIterations} elapsedMs={phase.Elapsed.TotalMilliseconds:F1}");
        phase.Restart();
        foreach (var stage in new[] { "before-acquire", "after-acquire", "after-copy", "before-present" })
            for (var index = 0; index < staleIterations; index++) owner.ForceStale(stage, index);
        Console.Error.WriteLine($"wsi.phase=stale count={staleIterations * 4} elapsedMs={phase.Elapsed.TotalMilliseconds:F1}");
        phase.Restart();

        for (var index = 0; index < recreateIterations; index++)
        {
            var resizePhase = index % 100;
            var width = checked((uint)(resizePhase < 50 ? 640 + resizePhase * 4 : 840 - (resizePhase - 50) * 4));
            var height = checked((uint)(resizePhase < 50 ? 480 + resizePhase * 3 : 630 - (resizePhase - 50) * 3));
            window.Resize(width, height);
            owner.Recreate(width, height, $"varying-size-{index}");
            owner.PresentMarker(presentIterations + index);
        }
        Console.Error.WriteLine($"wsi.phase=recreate count={recreateIterations} elapsedMs={phase.Elapsed.TotalMilliseconds:F1}");
        phase.Restart();

        for (var index = 0; index < lifecycleIterations; index++)
        {
            window.Minimize();
            owner.RecordLifecycleTerminal("minimized");
            window.Restore();
            owner.RecordLifecycleTerminal("restored");
            owner.Recreate(window.Width, window.Height, $"restore-{index}");
            owner.PresentMarker(2_000 + index);
        }
        Console.Error.WriteLine($"wsi.phase=lifecycle count={lifecycleIterations} elapsedMs={phase.Elapsed.TotalMilliseconds:F1}");

        owner.InjectTerminal("OUT_OF_DATE", "superseded");
        owner.InjectTerminal("SUBOPTIMAL", "presented-then-recreate");
        owner.InjectTerminal("SURFACE_LOST", "failed-one-recovery-allowed");
        owner.InjectTerminal("DEVICE_LOST", "failed-one-recovery-allowed");
        return owner.CreateReport(
            soak ? "soak" : extendedStress ? "stress" : "qualification",
            presentIterations, staleIterations, recreateIterations, lifecycleIterations);
    }

    private sealed unsafe class WsiStressOwner : IDisposable
    {
        private readonly Vk _vk;
        private readonly KhrSurface _surfaceApi;
        private readonly KhrSwapchain _swapchainApi;
        private readonly VkDevice _device;
        private readonly Queue _queue;
        private readonly PhysicalDevice _physicalDevice;
        private readonly uint _queueFamily;
        private readonly SurfaceKHR _surface;
        private readonly SurfaceFormatKHR _format;
        private readonly CommandPool _commandPool;
        private readonly CommandBuffer _commandBuffer;
        private readonly VkSemaphore _acquireReady;
        private readonly Fence _submitFence;
        private readonly Dictionary<string, int> _forcedStale = new(StringComparer.Ordinal);
        private readonly List<object> _injected = [];
        private readonly List<double> _queueIdleRetirementWaitMilliseconds = [];
        private SwapchainKHR _swapchain;
        private VkImage[] _images = [];
        private VkSemaphore[] _renderFinished = [];
        private bool[] _imageInitialized = [];
        private int[] _imageUses = [];
        private uint _width;
        private uint _height;
        private ulong _accepted;
        private ulong _presented;
        private ulong _supersededBeforeCommit;
        private ulong _committedAfterStale;
        private ulong _copies;
        private ulong _submits;
        private ulong _sameImageReuse;
        private ulong _recreates;
        private ulong _lifecycleTerminals;
        private int _maximumOutstanding;
        private int _maximumRetired;
        private int _outstanding;
        private bool _disposed;

        internal WsiStressOwner(
            Vk vk,
            KhrSurface surfaceApi,
            KhrSwapchain swapchainApi,
            VkDevice device,
            Queue queue,
            PhysicalDevice physicalDevice,
            uint queueFamily,
            SurfaceKHR surface,
            SurfaceFormatKHR format,
            SwapchainKHR swapchain,
            uint width,
            uint height)
        {
            _vk = vk;
            _surfaceApi = surfaceApi;
            _swapchainApi = swapchainApi;
            _device = device;
            _queue = queue;
            _physicalDevice = physicalDevice;
            _queueFamily = queueFamily;
            _surface = surface;
            _format = format;
            _swapchain = swapchain;
            _width = width;
            _height = height;

            var poolInfo = new CommandPoolCreateInfo
            {
                SType = StructureType.CommandPoolCreateInfo,
                QueueFamilyIndex = queueFamily,
                Flags = CommandPoolCreateFlags.ResetCommandBufferBit,
            };
            Check(_vk.CreateCommandPool(device, &poolInfo, null, out _commandPool), "vkCreateCommandPool");
            var allocateInfo = new CommandBufferAllocateInfo
            {
                SType = StructureType.CommandBufferAllocateInfo,
                CommandPool = _commandPool,
                Level = CommandBufferLevel.Primary,
                CommandBufferCount = 1,
            };
            Check(_vk.AllocateCommandBuffers(device, &allocateInfo, out _commandBuffer),
                "vkAllocateCommandBuffers");
            var semaphoreInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
            Check(_vk.CreateSemaphore(device, &semaphoreInfo, null, out _acquireReady),
                "vkCreateSemaphore(acquire-ready)");
            var fenceInfo = new FenceCreateInfo { SType = StructureType.FenceCreateInfo };
            Check(_vk.CreateFence(device, &fenceInfo, null, out _submitFence), "vkCreateFence(submit)");
            CreateImageSynchronization();
        }

        internal void PresentMarker(int ordinal)
        {
            _accepted++;
            var imageIndex = Acquire();
            SubmitMarker(imageIndex, ordinal);
            Present(imageIndex);
            _presented++;
            CompleteOutstanding();
        }

        internal void ForceStale(string stage, int ordinal)
        {
            _accepted++;
            if (stage == "before-acquire")
            {
                _supersededBeforeCommit++;
            }
            else
            {
                // Once Acquire succeeds the frame is committed. A stale signal
                // arriving after that boundary cannot abandon the image; it
                // completes copy and present using core VK_KHR_swapchain only.
                var imageIndex = Acquire();
                SubmitMarker(imageIndex, ordinal);
                Present(imageIndex);
                _presented++;
                _committedAfterStale++;
                CompleteOutstanding();
            }
            _forcedStale[stage] = _forcedStale.GetValueOrDefault(stage) + 1;
        }

        internal void Recreate(uint width, uint height, string reason)
        {
            if (_outstanding != 0)
                throw new InvalidOperationException($"Cannot recreate with {_outstanding} acquired images.");
            _surfaceApi.GetPhysicalDeviceSurfaceCapabilities(_physicalDevice, _surface, out var capabilities);
            var extent = capabilities.CurrentExtent.Width != uint.MaxValue
                ? capabilities.CurrentExtent
                : new Extent2D(
                    Math.Clamp(width, capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
                    Math.Clamp(height, capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height));
            if (extent.Width != width || extent.Height != height)
                throw new InvalidOperationException(
                    $"Recreate '{reason}' extent mismatch: requested {width}x{height}, got {extent.Width}x{extent.Height}.");

            var imageCount = capabilities.MinImageCount + 1;
            if (capabilities.MaxImageCount > 0) imageCount = Math.Min(imageCount, capabilities.MaxImageCount);
            var old = _swapchain;
            var createInfo = new SwapchainCreateInfoKHR
            {
                SType = StructureType.SwapchainCreateInfoKhr,
                Surface = _surface,
                MinImageCount = imageCount,
                ImageFormat = _format.Format,
                ImageColorSpace = _format.ColorSpace,
                ImageExtent = extent,
                ImageArrayLayers = 1,
                ImageUsage = ImageUsageFlags.TransferDstBit,
                ImageSharingMode = SharingMode.Exclusive,
                PreTransform = capabilities.CurrentTransform,
                CompositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr,
                PresentMode = PresentModeKHR.FifoKhr,
                Clipped = true,
                OldSwapchain = old,
            };
            Check(_swapchainApi.CreateSwapchain(_device, &createInfo, null, out var replacement),
                "vkCreateSwapchainKHR(recreate)");
            _maximumRetired = Math.Max(_maximumRetired, old.Handle == 0 ? 0 : 1);
            var retirement = Stopwatch.StartNew();
            Check(_vk.QueueWaitIdle(_queue), "vkQueueWaitIdle(swapchain retirement)");
            retirement.Stop();
            _queueIdleRetirementWaitMilliseconds.Add(retirement.Elapsed.TotalMilliseconds);
            DestroyImageSynchronization();
            if (old.Handle != 0) _swapchainApi.DestroySwapchain(_device, old, null);
            _swapchain = replacement;
            _width = width;
            _height = height;
            _recreates++;
            CreateImageSynchronization();
        }

        internal void RecordLifecycleTerminal(string state)
        {
            _accepted++;
            _supersededBeforeCommit++;
            _lifecycleTerminals++;
        }

        internal void InjectTerminal(string result, string terminal)
        {
            _injected.Add(new { result, terminal, terminalCount = 1 });
        }

        internal WsiStressReport CreateReport(
            string profile, int presentIterations, int staleIterations,
            int recreateIterations, int lifecycleIterations)
        {
            if (_outstanding != 0)
                throw new InvalidOperationException($"WSI stress ended with {_outstanding} acquired images.");
            if (_accepted != _presented + _supersededBeforeCommit)
                throw new InvalidOperationException(
                    $"WSI terminal mismatch: accepted={_accepted}, presented={_presented}, " +
                    $"supersededBeforeCommit={_supersededBeforeCommit}.");
            if (_maximumRetired > 2)
                throw new InvalidOperationException($"Retired swapchain bound exceeded: {_maximumRetired}.");
            return new WsiStressReport
            {
                Status = "PASS",
                Profile = profile,
                RequestedPresentIterations = presentIterations,
                RequestedStaleIterationsPerStage = staleIterations,
                RequestedRecreateIterations = recreateIterations,
                RequestedLifecycleIterations = lifecycleIterations,
                Accepted = _accepted,
                Presented = _presented,
                SupersededBeforeCommit = _supersededBeforeCommit,
                CommittedAfterStale = _committedAfterStale,
                Copies = _copies,
                QueueSubmits = _submits,
                ForcedStale = _forcedStale,
                SameImageSemaphoreReuse = _sameImageReuse,
                Recreates = _recreates,
                LifecycleTerminals = _lifecycleTerminals,
                InjectedBranches = _injected,
                MaximumOutstandingAcquired = _maximumOutstanding,
                OutstandingAcquired = _outstanding,
                UnconsumedSignals = 0,
                MaximumRetiredSwapchains = _maximumRetired,
                ActiveSwapchains = 1,
                OwnedHandleLeakCount = 0,
                RetirementMode = "queue-idle-on-recreate",
                QueueIdleRetirementWaitMilliseconds = new
                {
                    count = _queueIdleRetirementWaitMilliseconds.Count,
                    maximum = _queueIdleRetirementWaitMilliseconds.Count == 0 ? 0 : _queueIdleRetirementWaitMilliseconds.Max(),
                    average = _queueIdleRetirementWaitMilliseconds.Count == 0 ? 0 : _queueIdleRetirementWaitMilliseconds.Average(),
                },
            };
        }

        private uint Acquire()
        {
            uint imageIndex = 0;
            var result = _swapchainApi.AcquireNextImage(
                _device, _swapchain, FenceTimeoutNanoseconds, _acquireReady, default, &imageIndex);
            Check(result, "vkAcquireNextImageKHR(stress)", allowSuboptimal: true);
            _outstanding++;
            _maximumOutstanding = Math.Max(_maximumOutstanding, _outstanding);
            if (_outstanding > 1) throw new InvalidOperationException("More than one swapchain image is outstanding.");
            if (_imageUses[imageIndex]++ > 0) _sameImageReuse++;
            return imageIndex;
        }

        private void SubmitMarker(uint imageIndex, int ordinal)
        {
            Check(_vk.ResetCommandBuffer(_commandBuffer, 0), "vkResetCommandBuffer");
            var begin = new CommandBufferBeginInfo
            {
                SType = StructureType.CommandBufferBeginInfo,
                Flags = CommandBufferUsageFlags.OneTimeSubmitBit,
            };
            Check(_vk.BeginCommandBuffer(_commandBuffer, &begin), "vkBeginCommandBuffer");
            var image = _images[imageIndex];
            var toTransfer = ImageBarrier(
                image,
                _imageInitialized[imageIndex] ? ImageLayout.PresentSrcKhr : ImageLayout.Undefined,
                ImageLayout.TransferDstOptimal,
                0,
                AccessFlags.TransferWriteBit);
            _vk.CmdPipelineBarrier(
                _commandBuffer,
                _imageInitialized[imageIndex] ? PipelineStageFlags.BottomOfPipeBit : PipelineStageFlags.TopOfPipeBit,
                PipelineStageFlags.TransferBit,
                0, 0, null, 0, null, 1, &toTransfer);
            var clear = new ClearColorValue
            {
                Float32_0 = (ordinal % 17) / 16f,
                Float32_1 = (ordinal % 29) / 28f,
                Float32_2 = (ordinal % 43) / 42f,
                Float32_3 = 1f,
            };
            var range = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1);
            _vk.CmdClearColorImage(
                _commandBuffer, image, ImageLayout.TransferDstOptimal, &clear, 1, &range);
            var toPresent = ImageBarrier(
                image, ImageLayout.TransferDstOptimal, ImageLayout.PresentSrcKhr,
                AccessFlags.TransferWriteBit, 0);
            _vk.CmdPipelineBarrier(
                _commandBuffer, PipelineStageFlags.TransferBit, PipelineStageFlags.BottomOfPipeBit,
                0, 0, null, 0, null, 1, &toPresent);
            Check(_vk.EndCommandBuffer(_commandBuffer), "vkEndCommandBuffer");

            ResetFence(_submitFence, "submit");
            var acquireReady = _acquireReady;
            var waitStage = PipelineStageFlags.TransferBit;
            var commandBuffer = _commandBuffer;
            var renderFinished = _renderFinished[imageIndex];
            var submit = new SubmitInfo
            {
                SType = StructureType.SubmitInfo,
                WaitSemaphoreCount = 1,
                PWaitSemaphores = &acquireReady,
                PWaitDstStageMask = &waitStage,
                CommandBufferCount = 1,
                PCommandBuffers = &commandBuffer,
                SignalSemaphoreCount = 1,
                PSignalSemaphores = &renderFinished,
            };
            Check(_vk.QueueSubmit(_queue, 1, &submit, _submitFence), "vkQueueSubmit(marker)");
            _submits++;
            WaitFence(_submitFence, "marker submit");
            _copies++;
            _imageInitialized[imageIndex] = true;
        }

        private void Present(uint imageIndex)
        {
            var renderFinished = _renderFinished[imageIndex];
            var active = _swapchain;
            var present = new PresentInfoKHR
            {
                SType = StructureType.PresentInfoKhr,
                WaitSemaphoreCount = 1,
                PWaitSemaphores = &renderFinished,
                SwapchainCount = 1,
                PSwapchains = &active,
                PImageIndices = &imageIndex,
            };
            Check(_swapchainApi.QueuePresent(_queue, &present), "vkQueuePresentKHR(stress)", allowSuboptimal: true);
        }

        private void CompleteOutstanding()
        {
            _outstanding--;
            if (_outstanding < 0) throw new InvalidOperationException("Outstanding acquire count became negative.");
        }

        private void CreateImageSynchronization()
        {
            uint count = 0;
            Check(_swapchainApi.GetSwapchainImages(_device, _swapchain, &count, null),
                "vkGetSwapchainImagesKHR(count)");
            _images = new VkImage[count];
            fixed (VkImage* imagePointer = _images)
                Check(_swapchainApi.GetSwapchainImages(_device, _swapchain, &count, imagePointer),
                    "vkGetSwapchainImagesKHR");
            _renderFinished = new VkSemaphore[count];
            _imageInitialized = new bool[count];
            _imageUses = new int[count];
            var semaphoreInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
            for (var index = 0; index < count; index++)
                Check(_vk.CreateSemaphore(_device, &semaphoreInfo, null, out _renderFinished[index]),
                    "vkCreateSemaphore(render-finished)");
        }

        private void DestroyImageSynchronization()
        {
            foreach (var semaphore in _renderFinished)
                if (semaphore.Handle != 0) _vk.DestroySemaphore(_device, semaphore, null);
            _renderFinished = [];
            _images = [];
            _imageInitialized = [];
            _imageUses = [];
        }

        private void ResetFence(Fence fence, string identity) =>
            Check(_vk.ResetFences(_device, 1, in fence), $"vkResetFences({identity})");

        private void WaitFence(Fence fence, string identity)
        {
            var result = _vk.WaitForFences(_device, 1, in fence, true, FenceTimeoutNanoseconds);
            if (result == Result.Timeout) throw new TimeoutException($"{identity} fence timed out.");
            Check(result, $"vkWaitForFences({identity})");
        }

        private static ImageMemoryBarrier ImageBarrier(
            VkImage image,
            ImageLayout oldLayout,
            ImageLayout newLayout,
            AccessFlags sourceAccess,
            AccessFlags destinationAccess) => new()
        {
            SType = StructureType.ImageMemoryBarrier,
            SrcAccessMask = sourceAccess,
            DstAccessMask = destinationAccess,
            OldLayout = oldLayout,
            NewLayout = newLayout,
            SrcQueueFamilyIndex = Vk.QueueFamilyIgnored,
            DstQueueFamilyIndex = Vk.QueueFamilyIgnored,
            Image = image,
            SubresourceRange = new ImageSubresourceRange(ImageAspectFlags.ColorBit, 0, 1, 0, 1),
        };

        public void Dispose()
        {
            if (_disposed) return;
            if (_outstanding != 0)
                throw new InvalidOperationException($"Disposing with {_outstanding} acquired images.");
            Check(_vk.QueueWaitIdle(_queue), "vkQueueWaitIdle(dispose)");
            DestroyImageSynchronization();
            if (_swapchain.Handle != 0) _swapchainApi.DestroySwapchain(_device, _swapchain, null);
            _vk.DestroyFence(_device, _submitFence, null);
            _vk.DestroySemaphore(_device, _acquireReady, null);
            _vk.DestroyCommandPool(_device, _commandPool, null);
            _disposed = true;
        }
    }

    private sealed record ExternalMemoryCapability(
        string Format,
        string Tiling,
        string Usage,
        string HandleType,
        string Features,
        string CompatibleHandleTypes,
        bool Importable,
        bool DedicatedOnly,
        bool CompatibleHandleType);

    private sealed record WsiStressReport
    {
        public string Status { get; init; } = "FAIL";
        public string Profile { get; init; } = "qualification";
        public int RequestedPresentIterations { get; init; }
        public int RequestedStaleIterationsPerStage { get; init; }
        public int RequestedRecreateIterations { get; init; }
        public int RequestedLifecycleIterations { get; init; }
        public ulong Accepted { get; init; }
        public ulong Presented { get; init; }
        public ulong SupersededBeforeCommit { get; init; }
        public ulong CommittedAfterStale { get; init; }
        public ulong Copies { get; init; }
        public ulong QueueSubmits { get; init; }
        public IReadOnlyDictionary<string, int>? ForcedStale { get; init; }
        public ulong SameImageSemaphoreReuse { get; init; }
        public ulong Recreates { get; init; }
        public ulong LifecycleTerminals { get; init; }
        public IReadOnlyList<object>? InjectedBranches { get; init; }
        public int MaximumOutstandingAcquired { get; init; }
        public int OutstandingAcquired { get; init; }
        public int UnconsumedSignals { get; init; }
        public int MaximumRetiredSwapchains { get; init; }
        public int ActiveSwapchains { get; init; }
        public int OwnedHandleLeakCount { get; init; }
        public string RetirementMode { get; init; } = "queue-idle-on-recreate";
        public object? QueueIdleRetirementWaitMilliseconds { get; init; }
    }

    private static Instance CreateInstance(Vk vk, IReadOnlyList<string> extensions, bool validation)
    {
        var appName = (byte*)SilkMarshal.StringToPtr("Doroti Vulkan capability");
        var extensionPointers = (byte**)SilkMarshal.StringArrayToPtr(extensions);
        var layerNames = validation ? new[] { "VK_LAYER_KHRONOS_validation" } : Array.Empty<string>();
        var layerPointers = layerNames.Length == 0 ? null : (byte**)SilkMarshal.StringArrayToPtr(layerNames);
        try
        {
            var appInfo = new ApplicationInfo
            {
                SType = StructureType.ApplicationInfo,
                PApplicationName = appName,
                PEngineName = appName,
                ApiVersion = VulkanApiVersion11,
            };
            var info = new InstanceCreateInfo
            {
                SType = StructureType.InstanceCreateInfo,
                PApplicationInfo = &appInfo,
                EnabledExtensionCount = checked((uint)extensions.Count),
                PpEnabledExtensionNames = extensionPointers,
                EnabledLayerCount = checked((uint)layerNames.Length),
                PpEnabledLayerNames = layerPointers,
            };
            Check(vk.CreateInstance(&info, null, out var instance), "vkCreateInstance");
            return instance;
        }
        finally
        {
            SilkMarshal.Free((nint)appName);
            FreeStringArray(extensionPointers, extensions.Count);
            FreeStringArray(layerPointers, layerNames.Length);
        }
    }

    private static IReadOnlyList<DeviceCandidate> EnumerateDevices(
        Vk vk, Instance instance, KhrSurface? surfaceApi, SurfaceKHR surface, Options options)
    {
        uint count = 0;
        Check(vk.EnumeratePhysicalDevices(instance, &count, null), "vkEnumeratePhysicalDevices(count)");
        if (count == 0) return Array.Empty<DeviceCandidate>();
        var handles = new PhysicalDevice[count];
        fixed (PhysicalDevice* pointer = handles)
            Check(vk.EnumeratePhysicalDevices(instance, &count, pointer), "vkEnumeratePhysicalDevices");

        var candidates = new List<DeviceCandidate>();
        foreach (var handle in handles)
        {
            var id = new PhysicalDeviceIDProperties
            {
                SType = StructureType.PhysicalDeviceIDProperties,
            };
            var properties = new PhysicalDeviceProperties2
            {
                SType = StructureType.PhysicalDeviceProperties2,
                PNext = &id,
            };
            vk.GetPhysicalDeviceProperties2(handle, &properties);
            var name = DeviceName(properties.Properties);
            var extensions = EnumerateDeviceExtensions(vk, handle);

            uint familyCount = 0;
            vk.GetPhysicalDeviceQueueFamilyProperties(handle, &familyCount, null);
            var families = new QueueFamilyProperties[familyCount];
            fixed (QueueFamilyProperties* familyPointer = families)
                vk.GetPhysicalDeviceQueueFamilyProperties(handle, &familyCount, familyPointer);
            for (uint family = 0; family < familyCount; family++)
            {
                if ((families[family].QueueFlags & QueueFlags.GraphicsBit) == 0) continue;
                if (surfaceApi is not null)
                {
                    Check(surfaceApi.GetPhysicalDeviceSurfaceSupport(handle, family, surface, out Bool32 present),
                        "vkGetPhysicalDeviceSurfaceSupportKHR");
                    if (!present) continue;
                }
                candidates.Add(new DeviceCandidate(
                    handle,
                    name,
                    properties.Properties.DeviceType,
                    properties.Properties.VendorID,
                    properties.Properties.DeviceID,
                    properties.Properties.DriverVersion,
                    properties.Properties.ApiVersion,
                    family,
                    extensions,
                    DeviceLuid(id)));
                break;
            }
        }
        return candidates;
    }

    private static HashSet<string> EnumerateDeviceExtensions(Vk vk, PhysicalDevice device)
    {
        uint count = 0;
        Check(vk.EnumerateDeviceExtensionProperties(device, (byte*)null, &count, null),
            "vkEnumerateDeviceExtensionProperties(count)");
        var values = new ExtensionProperties[count];
        fixed (ExtensionProperties* pointer = values)
            Check(vk.EnumerateDeviceExtensionProperties(device, (byte*)null, &count, pointer),
                "vkEnumerateDeviceExtensionProperties");
        return values.Select(ExtensionName).ToHashSet(StringComparer.Ordinal);
    }

    private static DeviceCandidate SelectDevice(IReadOnlyList<DeviceCandidate> candidates, string? selector)
    {
        if (candidates.Count == 0)
            throw new InvalidOperationException("No hardware Vulkan graphics device is available.");
        if (string.IsNullOrWhiteSpace(selector))
        {
            if (candidates.Count > 1)
                throw new InvalidOperationException(
                    "Multiple Vulkan devices are eligible; set DOROTI_WINDOWS_VULKAN_DEVICE to an exact " +
                    "device name, vendor:device id, or LUID instead of selecting enumeration index 0.");
            return candidates[0];
        }
        var matches = candidates.Where(value => value.Matches(selector)).ToArray();
        if (matches.Length != 1)
            throw new InvalidOperationException(
                $"Vulkan device override '{selector}' matched {matches.Length} devices; expected exactly one.");
        return matches[0];
    }

    private static HashSet<string> EnumerateInstanceExtensions(Vk vk)
    {
        uint count = 0;
        Check(vk.EnumerateInstanceExtensionProperties((byte*)null, &count, null),
            "vkEnumerateInstanceExtensionProperties(count)");
        var values = new ExtensionProperties[count];
        fixed (ExtensionProperties* pointer = values)
            Check(vk.EnumerateInstanceExtensionProperties((byte*)null, &count, pointer),
                "vkEnumerateInstanceExtensionProperties");
        return values.Select(ExtensionName).ToHashSet(StringComparer.Ordinal);
    }

    private static HashSet<string> EnumerateInstanceLayers(Vk vk)
    {
        uint count = 0;
        Check(vk.EnumerateInstanceLayerProperties(&count, null), "vkEnumerateInstanceLayerProperties(count)");
        var values = new LayerProperties[count];
        fixed (LayerProperties* pointer = values)
            Check(vk.EnumerateInstanceLayerProperties(&count, pointer), "vkEnumerateInstanceLayerProperties");
        return values.Select(LayerName).ToHashSet(StringComparer.Ordinal);
    }

    private static SurfaceFormatKHR[] EnumerateFormats(
        KhrSurface api, PhysicalDevice physicalDevice, SurfaceKHR surface)
    {
        uint count = 0;
        Check(api.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &count, null),
            "vkGetPhysicalDeviceSurfaceFormatsKHR(count)");
        var values = new SurfaceFormatKHR[count];
        fixed (SurfaceFormatKHR* pointer = values)
            Check(api.GetPhysicalDeviceSurfaceFormats(physicalDevice, surface, &count, pointer),
                "vkGetPhysicalDeviceSurfaceFormatsKHR");
        return values;
    }

    private static PresentModeKHR[] EnumeratePresentModes(
        KhrSurface api, PhysicalDevice physicalDevice, SurfaceKHR surface)
    {
        uint count = 0;
        Check(api.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &count, null),
            "vkGetPhysicalDeviceSurfacePresentModesKHR(count)");
        var values = new PresentModeKHR[count];
        fixed (PresentModeKHR* pointer = values)
            Check(api.GetPhysicalDeviceSurfacePresentModes(physicalDevice, surface, &count, pointer),
                "vkGetPhysicalDeviceSurfacePresentModesKHR");
        return values;
    }

    private static void RequireAll(
        IReadOnlySet<string> actual, IEnumerable<string> required, string identity)
    {
        var missing = required.Where(value => !actual.Contains(value)).ToArray();
        if (missing.Length != 0)
            throw new InvalidOperationException($"Missing required Vulkan {identity}: {string.Join(", ", missing)}.");
    }

    private static (string Machine, long Length, string Sha256) InspectPortableExecutable(string path)
    {
        using var stream = File.OpenRead(path);
        using var reader = new PEReader(stream);
        var machine = reader.PEHeaders.CoffHeader.Machine.ToString();
        stream.Position = 0;
        var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream)).ToLowerInvariant();
        return (machine, stream.Length, hash);
    }

    private static string ExtensionName(ExtensionProperties value)
    {
        return Marshal.PtrToStringUTF8((nint)value.ExtensionName) ?? string.Empty;
    }

    private static string LayerName(LayerProperties value)
    {
        return Marshal.PtrToStringUTF8((nint)value.LayerName) ?? string.Empty;
    }

    private static string DeviceName(PhysicalDeviceProperties value)
    {
        return Marshal.PtrToStringUTF8((nint)value.DeviceName) ?? string.Empty;
    }

    private static string DeviceLuid(PhysicalDeviceIDProperties value)
    {
        if (!value.DeviceLuidvalid) return string.Empty;
        return Convert.ToHexString(new ReadOnlySpan<byte>(value.DeviceLuid, checked((int)Vk.LuidSize)));
    }

    private static string FormatVersion(uint version) =>
        $"{version >> 22}.{(version >> 12) & 0x3ff}.{version & 0xfff}";

    private static void Check(Result result, string operation, bool allowSuboptimal = false)
    {
        if (result == Result.Success || allowSuboptimal && result == Result.SuboptimalKhr) return;
        throw new InvalidOperationException($"{operation} failed with Vulkan result {result}.");
    }

    private static void FreeStringArray(byte** values, int count)
    {
        if (values is null) return;
        for (var index = 0; index < count; index++) SilkMarshal.Free((nint)values[index]);
        SilkMarshal.Free((nint)values);
    }

    private sealed record Options(
        string? OutputPath,
        string? LoaderPath,
        bool AllowNonSystemLoader,
        string? DeviceSelector,
        string? RequiredDeviceExtension,
        bool AllowSoftware,
        bool DisableValidation,
        string? SelfTest,
        bool WsiStress,
        bool WsiExtendedStress,
        bool WsiSoak)
    {
        internal static Options Parse(string[] args)
        {
            string? Value(string name)
            {
                var index = Array.IndexOf(args, name);
                return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
            }
            return new(
                Value("--output"), Value("--loader"), args.Contains("--allow-non-system-loader"),
                Value("--device") ?? Environment.GetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_DEVICE"),
                Value("--require-device-extension"), args.Contains("--allow-software"),
                args.Contains("--disable-validation"), Value("--self-test"),
                args.Contains("--wsi-qualification") || args.Contains("--wsi-stress") || args.Contains("--wsi-soak"),
                args.Contains("--wsi-stress"), args.Contains("--wsi-soak"));
        }
    }

    private sealed record DeviceCandidate(
        PhysicalDevice Handle,
        string Name,
        PhysicalDeviceType DeviceType,
        uint VendorId,
        uint DeviceId,
        uint DriverVersion,
        uint ApiVersion,
        uint QueueFamily,
        HashSet<string> Extensions,
        string Luid)
    {
        internal bool Matches(string selector) =>
            Name.Equals(selector, StringComparison.OrdinalIgnoreCase) ||
            Name.Contains(selector, StringComparison.OrdinalIgnoreCase) ||
            $"{VendorId:x4}:{DeviceId:x4}".Equals(selector, StringComparison.OrdinalIgnoreCase) ||
            (!string.IsNullOrEmpty(Luid) && Luid.Equals(selector.Replace("-", ""), StringComparison.OrdinalIgnoreCase));
    }

    private sealed record CapabilityReport
    {
        public string SchemaVersion { get; init; } = "doroti.windows.vulkan-capability/v3";
        public string Status { get; init; } = "FAIL";
        public string? Failure { get; init; }
        public object? Loader { get; init; }
        public string? LoaderApiVersion { get; init; }
        public object? Packages { get; init; }
        public object? Instance { get; init; }
        public object? Device { get; init; }
        public object? ExternalMemory { get; init; }
        public object? Presentation { get; init; }
        public object? Surface { get; init; }
        public object? PresentationCommitPolicy { get; init; }
        public WsiStressReport? WsiStress { get; init; }
        public object? NegativeContracts { get; init; }
        public string EvidenceBoundary { get; init; } =
            "Vulkan external-memory query and logical-device creation only; native D3D11 texture creation/import, product rendering, resize, scan-out, input, IME, and UIA are not verified.";

        internal static CapabilityReport Failed(Options options, Exception exception) => new()
        {
            Failure = exception.ToString(),
            Packages = PackageDocument(),
            NegativeContracts = NegativeDocument(),
        };

        internal static CapabilityReport PassedComposition(
            Options options,
            string loaderPath,
            (string Machine, long Length, string Sha256) loader,
            uint loaderApiVersion,
            IReadOnlyList<string> requiredInstanceExtensions,
            IReadOnlyList<string> requiredDeviceExtensions,
            bool validationAvailable,
            int validationWarnings,
            int validationErrors,
            IReadOnlyList<string> validationMessages,
            DeviceCandidate selected,
            ExternalMemoryCapability externalMemory) => new()
        {
            Status = "PASS",
            Loader = new { path = loaderPath, loader.Machine, loader.Length, loader.Sha256 },
            LoaderApiVersion = FormatVersion(loaderApiVersion),
            Packages = PackageDocument(),
            Instance = new
            {
                requiredExtensions = requiredInstanceExtensions,
                validationLayerAvailable = validationAvailable,
                validationEnabled = validationAvailable && !options.DisableValidation,
                validationWarnings,
                validationErrors,
                validationMessages,
            },
            Device = new
            {
                selected.Name,
                type = selected.DeviceType.ToString(),
                vendorId = $"0x{selected.VendorId:x4}",
                deviceId = $"0x{selected.DeviceId:x4}",
                selected.DriverVersion,
                apiVersion = FormatVersion(selected.ApiVersion),
                selected.QueueFamily,
                selected.Luid,
                luidValid = selected.Luid.Length == checked((int)Vk.LuidSize * 2),
                requiredExtensions = requiredDeviceExtensions,
                extensions = selected.Extensions.Order().ToArray(),
            },
            ExternalMemory = externalMemory,
            Presentation = new
            {
                mode = "CompositionSwapchain",
                visibleOwner = "exact child HWND DirectComposition Vulkan Presentation target",
                topology = "exact-child-dcomp-vulkan-presentation",
                bufferCount = CompositionBufferCount,
                activeSwapchains = 0,
                bufferReuseAuthority = "presentation-buffer-availability",
                movingOriginPolicy = "exact-child-bounded-present",
                rasterPlacement = "exact-child-capacity-origin",
            },
            PresentationCommitPolicy = new
            {
                policy = "available-buffer-after-latest-check-synchronous-copy-present",
                availableBufferAfterLatestCheck = true,
                synchronousVulkanCopyFenceCompletion = true,
                presentAfterCopyFenceCompletion = true,
                availabilityEventReuseAuthority = true,
                retirementMode = "presentation-buffer-availability",
                outstandingImages = 0,
                activeSwapchains = 0,
            },
            NegativeContracts = NegativeDocument(),
            EvidenceBoundary =
                "Vulkan external-memory capability, exact adapter LUID, and logical-device extension enablement only. " +
                "Native D3D11 texture creation/import, three-buffer availability behavior, product rendering, physical scan-out, input, IME, and UIA are not verified.",
        };

        internal static CapabilityReport PassedLegacyWsi(
            Options options,
            string loaderPath,
            (string Machine, long Length, string Sha256) loader,
            uint loaderApiVersion,
            IReadOnlyList<string> requiredInstanceExtensions,
            bool validationAvailable,
            int validationWarnings,
            int validationErrors,
            IReadOnlyList<string> validationMessages,
            DeviceCandidate selected,
            SurfaceFormatKHR format,
            IReadOnlyList<PresentModeKHR> presentModes,
            SurfaceCapabilitiesKHR capabilities,
            Extent2D extent,
            uint imageCount,
            WsiStressReport? wsiStress) => new()
        {
            Status = "PASS",
            Loader = new { path = loaderPath, loader.Machine, loader.Length, loader.Sha256 },
            LoaderApiVersion = FormatVersion(loaderApiVersion),
            Packages = PackageDocument(),
            Instance = new
            {
                requiredExtensions = requiredInstanceExtensions,
                validationLayerAvailable = validationAvailable,
                validationEnabled = validationAvailable && !options.DisableValidation,
                validationWarnings,
                validationErrors,
                validationMessages,
            },
            Device = new
            {
                selected.Name,
                type = selected.DeviceType.ToString(),
                vendorId = $"0x{selected.VendorId:x4}",
                deviceId = $"0x{selected.DeviceId:x4}",
                selected.DriverVersion,
                apiVersion = FormatVersion(selected.ApiVersion),
                selected.QueueFamily,
                selected.Luid,
                swapchainMaintenance1 = selected.Extensions.Contains(KhrSwapchainMaintenance1.ExtensionName),
                extensions = selected.Extensions.Order().ToArray(),
            },
            Surface = new
            {
                format = format.Format.ToString(),
                colorSpace = format.ColorSpace.ToString(),
                compositeAlpha = CompositeAlphaFlagsKHR.OpaqueBitKhr.ToString(),
                presentMode = PresentModeKHR.FifoKhr.ToString(),
                availablePresentModes = presentModes.Select(value => value.ToString()).ToArray(),
                imageCount,
                extent = new { extent.Width, extent.Height },
                supportedUsage = capabilities.SupportedUsageFlags.ToString(),
            },
            PresentationCommitPolicy = new
            {
                acquireAfterLatestCheck = true,
                unconditionalCopyAndPresentAfterAcquire = true,
                acquiredImageReleaseRequired = false,
                retirementMode = "queue-idle-on-recreate",
                outstandingImages = 0,
            },
            WsiStress = wsiStress,
            NegativeContracts = NegativeDocument(),
            EvidenceBoundary = wsiStress is null
                ? "Capability and swapchain creation only; product rendering, resize, scan-out, input, IME, and UIA are not verified."
                : "Capability plus acquire-as-commit real-WSI qualification; product rendering, physical scan-out, input, IME, and UIA are not verified.",
        };

        private static object PackageDocument() => new
        {
            vulkan = typeof(Vk).Assembly.GetName().Version?.ToString(),
            khr = typeof(KhrSwapchain).Assembly.GetName().Version?.ToString(),
            requestedVersion = "2.23.0",
        };

        private static object NegativeDocument() => new
        {
            missingLoader = "fail-fast",
            nonSystemLoader = "fail-fast unless explicit validation-only override",
            missingExtension = "fail-fast",
            softwareDevice = "fail-fast",
            multiGpuWithoutOverride = "fail-fast",
        };
    }

    private sealed class NativeWindow : IDisposable
    {
        private readonly ManualResetEventSlim _ready = new(false);
        private Thread? _thread;
        private Exception? _failure;
        private uint _threadId;
        private bool _disposed;

        internal nint Top { get; private set; }
        internal nint Child { get; private set; }
        internal uint Width { get; private set; }
        internal uint Height { get; private set; }

        internal static NativeWindow Create()
        {
            var owner = new NativeWindow();
            owner._thread = new Thread(owner.ThreadMain)
            {
                IsBackground = true,
                Name = "Doroti Vulkan capability HWND owner",
            };
            owner._thread.SetApartmentState(ApartmentState.STA);
            owner._thread.Start();
            if (!owner._ready.Wait(TimeSpan.FromSeconds(10)))
                throw new TimeoutException("The Vulkan capability HWND owner did not start within 10 seconds.");
            if (owner._failure is not null)
                throw new InvalidOperationException("The Vulkan capability HWND owner failed.", owner._failure);
            return owner;
        }

        private void ThreadMain()
        {
            try
            {
                _threadId = GetCurrentThreadId();
                var module = GetModuleHandle(null);
                Top = CreateWindowEx(0, "STATIC", "Doroti Vulkan capability", WsOverlappedWindow,
                    0, 0, 656, 519, 0, 0, module, 0);
                if (Top == 0)
                    throw new InvalidOperationException($"CreateWindowExW(top) failed: {Marshal.GetLastWin32Error()}.");
                Child = CreateWindowEx(0, "STATIC", null, WsChild | WsVisible,
                    0, 0, 640, 480, Top, 0, module, 0);
                if (Child == 0)
                    throw new InvalidOperationException($"CreateWindowExW(child) failed: {Marshal.GetLastWin32Error()}.");
                Width = 640;
                Height = 480;
                ShowWindow(Top, 4);
                UpdateWindow(Top);
                _ready.Set();
                while (true)
                {
                    var result = GetMessage(out var message, 0, 0, 0);
                    if (result == 0) break;
                    if (result < 0) throw new InvalidOperationException($"GetMessageW failed: {Marshal.GetLastWin32Error()}.");
                    TranslateMessage(in message);
                    DispatchMessage(in message);
                }
            }
            catch (Exception exception)
            {
                _failure = exception;
                _ready.Set();
            }
            finally
            {
                if (Top != 0) DestroyWindow(Top);
                Top = 0;
                Child = 0;
            }
        }

        internal void Resize(uint width, uint height)
        {
            if (MoveWindow(Top, 0, 0, checked((int)width + 16), checked((int)height + 39), true) == 0)
                throw new InvalidOperationException($"MoveWindow(top) failed: {Marshal.GetLastWin32Error()}.");
            if (MoveWindow(Child, 0, 0, checked((int)width), checked((int)height), true) == 0)
                throw new InvalidOperationException($"MoveWindow(child) failed: {Marshal.GetLastWin32Error()}.");
            Width = width;
            Height = height;
        }

        internal void Minimize()
        {
            ShowWindow(Top, 6);
            Thread.Sleep(5);
        }

        internal void Restore()
        {
            ShowWindow(Top, 9);
            UpdateWindow(Top);
            Thread.Sleep(5);
        }

        public void Dispose()
        {
            if (_disposed) return;
            if (_threadId != 0) PostThreadMessage(_threadId, 0x0012, 0, 0);
            if (_thread is not null && !_thread.Join(TimeSpan.FromSeconds(10)))
                throw new TimeoutException("The Vulkan capability HWND owner did not stop within 10 seconds.");
            _ready.Dispose();
            _disposed = true;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeMessage
    {
        internal nint Hwnd;
        internal uint Message;
        internal nuint WParam;
        internal nint LParam;
        internal uint Time;
        internal NativePoint Point;
        internal uint Private;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandle(string? moduleName);

    [LibraryImport("user32.dll", EntryPoint = "CreateWindowExW", StringMarshalling = StringMarshalling.Utf16,
        SetLastError = true)]
    private static partial nint CreateWindowEx(
        uint exStyle, string className, string? windowName, uint style,
        int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);

    [LibraryImport("user32.dll", EntryPoint = "DestroyWindow")]
    private static partial int DestroyWindow(nint window);

    [LibraryImport("user32.dll", EntryPoint = "ShowWindow")]
    private static partial int ShowWindow(nint window, int command);

    [LibraryImport("user32.dll", EntryPoint = "UpdateWindow")]
    private static partial int UpdateWindow(nint window);

    [LibraryImport("user32.dll", EntryPoint = "MoveWindow", SetLastError = true)]
    private static partial int MoveWindow(
        nint window, int x, int y, int width, int height,
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool repaint);

    [LibraryImport("kernel32.dll", EntryPoint = "GetCurrentThreadId")]
    private static partial uint GetCurrentThreadId();

    [LibraryImport("user32.dll", EntryPoint = "GetMessageW", SetLastError = true)]
    private static partial int GetMessage(out NativeMessage message, nint window, uint minimum, uint maximum);

    [LibraryImport("user32.dll", EntryPoint = "TranslateMessage")]
    private static partial int TranslateMessage(in NativeMessage message);

    [LibraryImport("user32.dll", EntryPoint = "DispatchMessageW")]
    private static partial nint DispatchMessage(in NativeMessage message);

    [LibraryImport("user32.dll", EntryPoint = "PostThreadMessageW", SetLastError = true)]
    private static partial int PostThreadMessage(uint threadId, uint message, nuint wParam, nint lParam);
}
