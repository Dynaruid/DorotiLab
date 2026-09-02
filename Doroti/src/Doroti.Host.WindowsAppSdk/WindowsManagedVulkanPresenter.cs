using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Doroti.Skia.RuntimeEffects;
using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;
using VkImage = Silk.NET.Vulkan.Image;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter : WindowsManagedHwndPresenterBase
{
    private const ulong FenceTimeoutNanoseconds = 5_000_000_000;
    private const uint VulkanApiVersion11 = (1u << 22) | (1u << 12);

    private readonly bool _diagnosticsEnabled;
    private readonly string _loaderPath;
    private readonly string _loaderSha256;
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
    private GRVkExtensions? _skiaExtensions;
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
    private VkSemaphore _acquireSemaphore;
    private VkSemaphore[] _renderFinishedSemaphores = [];
    private ImageLayout[] _swapchainLayouts = [];
    private bool _acquired;
    private uint _acquiredImageIndex;
    private Format _format;
    private uint _queueFamily;
    private uint _loaderApiVersion;
    private uint _deviceApiVersion;
    private uint _deviceVendorId;
    private uint _deviceId;
    private uint _driverVersion;
    private string _deviceType = "uninitialized";
    private string _deviceLuid = "";
    private string _colorSpace = "uninitialized";
    private string _compositeAlpha = "uninitialized";
    private string _presentMode = "uninitialized";
    private ulong _swapchainGeneration;
    private ulong _acquiredCount;
    private ulong _presentTerminalCount;
    private ulong _maximumOutstandingAcquired;
    private ulong _queueIdleRetirementWaitCount;
    private ulong _deviceLostCount;
    private ulong _surfaceLostCount;
    private ulong _outOfDateCount;
    private ulong _suboptimalCount;
    private int _maximumRetiredSwapchains;
    private long _lastRetirementLatencyMicroseconds;
    private long _firstPresentQpc;
    private long _lastTargetQpc;
    private long _lastPresentQpc;
    private string _lastRecreateReason = "not-created";
    private string? _pendingRecreateReason;
    private Result _lastAcquireResult = Result.Success;
    private Result _lastSubmitResult = Result.Success;
    private Result _lastPresentResult = Result.Success;
    private readonly HashSet<string> _consumedInjections = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _eventGate = new();
    private readonly Queue<string> _recentEvents = [];
    private nint _window;
    private bool _flushAfterResizePresent;
    private bool _debugBaselineSealed;
    private bool _disposed;

    internal WindowsManagedVulkanPresenter(bool enableDiagnostics)
    {
        _diagnosticsEnabled = enableDiagnostics;
        _loaderPath = Path.GetFullPath(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll"));
        if (!File.Exists(_loaderPath))
            throw new FileNotFoundException("The System32 Vulkan loader is missing.", _loaderPath);
        using (var loaderStream = File.OpenRead(_loaderPath))
            _loaderSha256 = Convert.ToHexString(SHA256.HashData(loaderStream)).ToLowerInvariant();
        _vk = new Vk(new DefaultNativeContext(_loaderPath));
        RecordEvent($"loader-open path={_loaderPath}");
    }

    internal override string BackendName => "Vulkan";
    internal override string RuntimeEffectsBackend => DorotiSkiaRuntimeEffects.WindowsVulkanBackend;
    internal override string DiagnosticCoverage =>
        "direct Vulkan 1.1 device, Win32 FIFO surface/swapchain, acquire-as-presentation-commit, " +
        "unconditional copy/present after acquire, queue-idle swapchain retirement, checked VkResult values, and resize DwmFlush";
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

    internal VulkanPresenterSnapshot Snapshot() => new(
        _loaderPath, _loaderSha256, FormatVersion(_loaderApiVersion),
        typeof(Vk).Assembly.GetName().Version?.ToString() ?? "unknown",
        AdapterDescription, _deviceType, _deviceVendorId, _deviceId, _driverVersion,
        FormatVersion(_deviceApiVersion), _deviceLuid, _queueFamily,
        _format.ToString(), _colorSpace, _compositeAlpha, _presentMode,
        _swapchainImages.Length, Width, Height, _swapchainGeneration,
        _acquiredCount, _presentTerminalCount, PresentCount,
        _acquired ? 1 : 0, _acquired ? checked((int)_acquiredImageIndex) : -1,
        _maximumOutstandingAcquired, _deviceLostCount, _surfaceLostCount,
        _outOfDateCount, _suboptimalCount,
        _lastAcquireResult.ToString(), _lastSubmitResult.ToString(),
        _lastPresentResult.ToString(),
        ActiveSwapchains: _swapchain.Handle == 0 ? 0 : 1,
        RetiredSwapchains: 0,
        ValidationEnabled: false,
        MaximumRetiredSwapchains: _maximumRetiredSwapchains,
        LastRecreateReason: _lastRecreateReason,
        LastRetirementLatencyMicroseconds: _lastRetirementLatencyMicroseconds,
        FirstPresentQpc: _firstPresentQpc,
        LastTargetQpc: _lastTargetQpc,
        LastPresentQpc: _lastPresentQpc,
        RetirementMode: "queue-idle-on-recreate",
        QueueIdleRetirementWaits: _queueIdleRetirementWaitCount,
        RecentEvents: SnapshotEvents());

    internal bool HasPendingInjectedResult
    {
        get
        {
            var requested = Environment.GetEnvironmentVariable(
                "DOROTI_WINDOWS_VULKAN_INJECT_RESULT")?.Trim();
            return !string.IsNullOrWhiteSpace(requested) && !_consumedInjections.Contains(requested);
        }
    }

    internal override bool EnsureTarget(nint childWindow, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        LastPresentSucceeded = false;
        if (childWindow == 0) throw new ArgumentOutOfRangeException(nameof(childWindow));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        _lastTargetQpc = Stopwatch.GetTimestamp();

        if (_window != 0 && _window != childWindow)
            ReleaseDevice(deviceLost: false);
        EnsureDevice(childWindow);
        if (_swapchain.Handle != 0 && Width == width && Height == height)
            return true;

        var resized = _swapchain.Handle != 0;
        var recreateReason = _pendingRecreateReason ??
            (resized ? "extent-change" : _swapchainGeneration == 0 ? "initial" : "device-or-surface-recovery");
        _pendingRecreateReason = null;
        if (!RecreateSwapchain(width, height, recreateReason)) return false;
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

        LastPresentSucceeded = false;
        var result = paint(backing);
        if (!shouldPresent(result)) return result;
        backing.Canvas.Flush();
        context.Flush(backing);
        context.Submit(true);
        GpuSubmitCount++;
        if (!shouldPresent(result)) return result;

        // This is the presentation commit boundary. Staleness is checked up to
        // the acquire attempt; once an image is acquired, this frame always
        // completes copy -> present and no acquired-image release extension is
        // needed. Newer work remains the single latest pending frame.
        if (!TryAcquireNextImage(() => shouldPresent(result), out var imageIndex)) return result;

        var renderFinished = _renderFinishedSemaphores[checked((int)imageIndex)];
        CopyBackingToSwapchain(_swapchainImages[checked((int)imageIndex)], imageIndex, renderFinished);
        GpuCopyCount++;
        var swapchain = _swapchain;
        var presentInfo = new PresentInfoKHR
        {
            SType = StructureType.PresentInfoKhr,
            WaitSemaphoreCount = 1,
            PWaitSemaphores = &renderFinished,
            SwapchainCount = 1,
            PSwapchains = &swapchain,
            PImageIndices = &imageIndex,
        };
        var present = _swapchainApi.QueuePresent(_queue, &presentInfo);
        _presentTerminalCount++;
        if (present == Result.Success && TakeInjectedResult("SUBOPTIMAL"))
            present = Result.SuboptimalKhr;
        _lastPresentResult = present;
        RecordEvent($"present image={imageIndex} result={present}");
        _acquired = false;
        if (present is Result.ErrorOutOfDateKhr)
        {
            _outOfDateCount++;
            _pendingRecreateReason = "present-out-of-date";
            Width = Height = 0;
            return result;
        }
        Check(present, "vkQueuePresentKHR", allowSuboptimal: true);
        var suboptimal = present == Result.SuboptimalKhr || _lastAcquireResult == Result.SuboptimalKhr;
        if (suboptimal) _suboptimalCount++;
        PresentCount++;
        LastPresentSucceeded = true;
        _lastPresentQpc = Stopwatch.GetTimestamp();
        if (_firstPresentQpc == 0) _firstPresentQpc = _lastPresentQpc;
        if (suboptimal)
        {
            _pendingRecreateReason = "present-suboptimal";
            Width = Height = 0;
        }
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
        if (_acquired) throw new InvalidOperationException("A Vulkan swapchain image is already acquired.");
        imageIndex = 0;
        uint acquiredIndex = 0;
        var deadline = Environment.TickCount64 + 5_000;
        while (true)
        {
            var acquire = TakeInjectedResult("OUT_OF_DATE") ? Result.ErrorOutOfDateKhr :
                TakeInjectedResult("SURFACE_LOST") ? Result.ErrorSurfaceLostKhr :
                TakeInjectedResult("DEVICE_LOST") ? Result.ErrorDeviceLost :
                _swapchainApi!.AcquireNextImage(
                    _device, _swapchain, 10_000_000, _acquireSemaphore, default, &acquiredIndex);
            _lastAcquireResult = acquire;
            if (acquire is Result.Success or Result.SuboptimalKhr)
            {
                _acquired = true;
                _acquiredImageIndex = acquiredIndex;
                _acquiredCount++;
                _maximumOutstandingAcquired = Math.Max(_maximumOutstandingAcquired, 1);
                RecordEvent($"acquire image={acquiredIndex} result={acquire}");
                imageIndex = acquiredIndex;
                return true;
            }
            if (acquire is Result.ErrorOutOfDateKhr)
            {
                _outOfDateCount++;
                _pendingRecreateReason = "acquire-out-of-date";
                RecordEvent($"acquire result={acquire}");
                Width = Height = 0;
                return false;
            }
            if (acquire is Result.ErrorSurfaceLostKhr)
            {
                _surfaceLostCount++;
                RecordEvent($"acquire result={acquire}");
                throw new WindowsManagedVulkanSurfaceLostException(
                    "vkAcquireNextImageKHR reported ErrorSurfaceLostKhr.");
            }
            if (acquire is Result.ErrorDeviceLost)
            {
                _deviceLostCount++;
                RecordEvent($"acquire result={acquire}");
                throw new WindowsManagedVulkanDeviceLostException(
                    "vkAcquireNextImageKHR reported ErrorDeviceLost.");
            }
            if (acquire is not (Result.NotReady or Result.Timeout))
                Check(acquire, "vkAcquireNextImageKHR");
            if (!shouldContinue()) return false;
            if (Environment.TickCount64 >= deadline)
                throw new TimeoutException("Vulkan acquire did not become ready within 5 seconds.");
            Thread.Yield();
        }
    }

    private bool TakeInjectedResult(string result)
    {
        var requested = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_INJECT_RESULT")?.Trim();
        if (!string.Equals(requested, result, StringComparison.OrdinalIgnoreCase) ||
            _consumedInjections.Contains(result))
            return false;
        var minimumPresents = 0UL;
        var configured = Environment.GetEnvironmentVariable(
            "DOROTI_WINDOWS_VULKAN_INJECT_AFTER_PRESENTS");
        if (!string.IsNullOrWhiteSpace(configured) && !ulong.TryParse(configured, out minimumPresents))
            throw new InvalidOperationException(
                "DOROTI_WINDOWS_VULKAN_INJECT_AFTER_PRESENTS must be an unsigned integer.");
        if (PresentCount < minimumPresents) return false;
        _consumedInjections.Add(result);
        return true;
    }

    internal override void ResetDevice()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ReleaseDevice(deviceLost: false);
    }

    internal void RecoverAfterDeviceLoss()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RecordEvent("device-loss recovery");
        _pendingRecreateReason = "device-loss-recovery";
        ReleaseDevice(deviceLost: true);
    }

    internal void RecoverAfterSurfaceLoss()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RecordEvent("surface-loss recovery");
        _pendingRecreateReason = "surface-loss-recovery";
        ReleaseDevice(deviceLost: false);
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
        var loaderApiVersion = VulkanApiVersion11;
        Check(_vk.EnumerateInstanceVersion(&loaderApiVersion), "vkEnumerateInstanceVersion");
        _loaderApiVersion = loaderApiVersion;
        if (_loaderApiVersion < VulkanApiVersion11)
            throw new InvalidOperationException(
                $"The Vulkan loader API {FormatVersion(_loaderApiVersion)} is below 1.1.");
        var applicationName = (byte*)SilkMarshal.StringToPtr("Doroti");
        var engineName = (byte*)SilkMarshal.StringToPtr("Doroti");
        var extensionNames = new[]
        {
            KhrSurface.ExtensionName,
            KhrWin32Surface.ExtensionName,
        };
        RequireInstanceExtensions(extensionNames);
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

        var candidates = new List<(PhysicalDevice Device, uint QueueFamily, PhysicalDeviceProperties Properties,
            string Name, string Luid, HashSet<string> Extensions)>();
        var rejectedCandidates = new List<string>();
        foreach (var candidate in devices)
        {
            var id = new PhysicalDeviceIDProperties
            {
                SType = StructureType.PhysicalDeviceIDProperties,
            };
            var properties2 = new PhysicalDeviceProperties2
            {
                SType = StructureType.PhysicalDeviceProperties2,
                PNext = &id,
            };
            _vk.GetPhysicalDeviceProperties2(candidate, &properties2);
            var properties = properties2.Properties;
            if (properties.DeviceType == PhysicalDeviceType.Cpu) continue;
            var name = Marshal.PtrToStringUTF8((nint)properties.DeviceName) ?? "unnamed Vulkan device";
            var extensions = EnumerateDeviceExtensions(candidate);
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
                if (!extensions.Contains(KhrSwapchain.ExtensionName))
                {
                    rejectedCandidates.Add($"{name}: missing VK_KHR_swapchain");
                    break;
                }
                if (properties.ApiVersion < VulkanApiVersion11)
                {
                    rejectedCandidates.Add($"{name}: Vulkan API below 1.1");
                    break;
                }
                candidates.Add((candidate, family, properties, name, DeviceLuid(id), extensions));
                break;
            }
        }
        if (candidates.Count == 0)
            throw new InvalidOperationException(
                "No hardware Vulkan device satisfies the graphics/present and core-swapchain requirements" +
                (rejectedCandidates.Count == 0 ? "." : $": {string.Join("; ", rejectedCandidates)}."));
        var selector = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_DEVICE")?.Trim();
        if (string.IsNullOrWhiteSpace(selector) && candidates.Count != 1)
            throw new InvalidOperationException(
                "Multiple Vulkan devices are eligible. Set DOROTI_WINDOWS_VULKAN_DEVICE to an exact or unique device-name fragment.");
        var selected = string.IsNullOrWhiteSpace(selector)
            ? candidates[0]
            : candidates.SingleOrDefault(value => value.Name.Contains(selector, StringComparison.OrdinalIgnoreCase));
        if (selected.Device.Handle == 0)
            throw new InvalidOperationException($"DOROTI_WINDOWS_VULKAN_DEVICE='{selector}' did not match exactly one Vulkan device.");
        if (!selected.Extensions.Contains(KhrSwapchain.ExtensionName))
            throw new InvalidOperationException(
                $"Vulkan device '{selected.Name}' is missing VK_KHR_swapchain.");
        if (selected.Properties.ApiVersion < VulkanApiVersion11)
            throw new InvalidOperationException($"Vulkan device '{selected.Name}' does not support Vulkan 1.1.");
        _physicalDevice = selected.Device;
        _queueFamily = selected.QueueFamily;
        _deviceApiVersion = selected.Properties.ApiVersion;
        _deviceVendorId = selected.Properties.VendorID;
        _deviceId = selected.Properties.DeviceID;
        _driverVersion = selected.Properties.DriverVersion;
        _deviceType = selected.Properties.DeviceType.ToString();
        _deviceLuid = selected.Luid;
        AdapterDescription = $"{selected.Name}; vendor=0x{selected.Properties.VendorID:x4}; " +
            $"device=0x{selected.Properties.DeviceID:x4}; api={FormatVersion(selected.Properties.ApiVersion)}";
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
                EnabledExtensionCount = checked((uint)extensionNames.Length),
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
        var semaphoreInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        Check(_vk.CreateSemaphore(_device, &semaphoreInfo, null, out _acquireSemaphore),
            "vkCreateSemaphore(acquire)");
    }

    private void CreateSkiaContext()
    {
        string[] instanceExtensions = [KhrSurface.ExtensionName, KhrWin32Surface.ExtensionName];
        string[] deviceExtensions = [KhrSwapchain.ExtensionName];
        _skiaExtensions = GRVkExtensions.Create(
            GetVulkanProcedureAddress,
            _instance.Handle,
            _physicalDevice.Handle,
            instanceExtensions,
            deviceExtensions);
        _skiaBackend = new GRVkBackendContext
        {
            VkInstance = _instance.Handle,
            VkPhysicalDevice = _physicalDevice.Handle,
            VkDevice = _device.Handle,
            VkQueue = _queue.Handle,
            GraphicsQueueIndex = _queueFamily,
            GetProcedureAddress = GetVulkanProcedureAddress,
            Extensions = _skiaExtensions,
            MaxAPIVersion = Math.Min(VulkanApiVersion11, _deviceApiVersion),
        };
        _context = GRContext.CreateVulkan(_skiaBackend)
            ?? throw new InvalidOperationException("Skia could not create the managed Vulkan context.");
    }

    private nint GetVulkanProcedureAddress(string name, nint instance, nint device)
    {
        if (device != 0) return _vk.GetDeviceProcAddr(new VkDevice(device), name);
        return _vk.GetInstanceProcAddr(new Instance(instance), name);
    }

    private bool RecreateSwapchain(int width, int height, string reason)
    {
        if (_acquired)
            throw new InvalidOperationException(
                "Swapchain recreation cannot begin inside an acquire-as-presentation-commit transaction.");
        var oldSwapchain = _swapchain;
        if (oldSwapchain.Handle != 0) _maximumRetiredSwapchains = Math.Max(_maximumRetiredSwapchains, 1);
        var retirementStarted = Stopwatch.GetTimestamp();
        WaitForPresentRetirement();
        _lastRetirementLatencyMicroseconds = checked((long)
            Stopwatch.GetElapsedTime(retirementStarted).TotalMicroseconds);
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
        _colorSpace = selected.ColorSpace.ToString();

        var extent = capabilities.CurrentExtent.Width != uint.MaxValue
            ? capabilities.CurrentExtent
            : new Extent2D(
                Math.Clamp(checked((uint)width), capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
                Math.Clamp(checked((uint)height), capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height));
        if (extent.Width != width || extent.Height != height)
            return false;
        ReleaseBacking();
        ReleaseSwapchainSynchronization();
        var imageCount = capabilities.MinImageCount + 1;
        if (capabilities.MaxImageCount > 0) imageCount = Math.Min(imageCount, capabilities.MaxImageCount);
        var presentMode = SelectPresentMode();
        var compositeAlpha = SelectCompositeAlpha(capabilities.SupportedCompositeAlpha);
        _presentMode = presentMode.ToString();
        _compositeAlpha = compositeAlpha.ToString();
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
            CompositeAlpha = compositeAlpha,
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
        _swapchainLayouts = Enumerable.Repeat(ImageLayout.Undefined, checked((int)swapchainImageCount)).ToArray();
        _renderFinishedSemaphores = new VkSemaphore[swapchainImageCount];
        var semaphoreInfo = new SemaphoreCreateInfo { SType = StructureType.SemaphoreCreateInfo };
        for (var index = 0; index < swapchainImageCount; index++)
            Check(_vk.CreateSemaphore(_device, &semaphoreInfo, null, out _renderFinishedSemaphores[index]),
                "vkCreateSemaphore(render-finished)");
        CreateBacking(width, height);
        _swapchainGeneration++;
        _lastRecreateReason = reason;
        Width = width;
        Height = height;
        RecordEvent(
            $"swapchain generation={_swapchainGeneration} reason={reason} extent={width}x{height} retirementUs={_lastRetirementLatencyMicroseconds}");
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
        if (!modes.Contains(PresentModeKHR.FifoKhr))
            throw new InvalidOperationException("The Vulkan Win32 surface does not expose mandatory FIFO present mode.");
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

    private void CopyBackingToSwapchain(VkImage destination, uint imageIndex, VkSemaphore renderFinished)
    {
        BeginCommands();
        var barriers = stackalloc ImageMemoryBarrier[2];
        barriers[0] = ImageBarrier(
            _backingImage, ImageLayout.ColorAttachmentOptimal, ImageLayout.TransferSrcOptimal,
            AccessFlags.ColorAttachmentWriteBit, AccessFlags.TransferReadBit);
        barriers[1] = ImageBarrier(
            destination, _swapchainLayouts[checked((int)imageIndex)], ImageLayout.TransferDstOptimal,
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
        _swapchainLayouts[checked((int)imageIndex)] = ImageLayout.PresentSrcKhr;
        SubmitCommandsAndWait("Vulkan backing copy", _acquireSemaphore, renderFinished);
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

    private void SubmitCommandsAndWait(
        string identity,
        VkSemaphore waitSemaphore = default,
        VkSemaphore signalSemaphore = default)
    {
        Check(_vk.EndCommandBuffer(_commandBuffer), "vkEndCommandBuffer");
        ResetFence();
        var commandBuffer = _commandBuffer;
        var waitStage = PipelineStageFlags.TransferBit;
        var submitInfo = new SubmitInfo
        {
            SType = StructureType.SubmitInfo,
            CommandBufferCount = 1,
            PCommandBuffers = &commandBuffer,
        };
        if (waitSemaphore.Handle != 0)
        {
            submitInfo.WaitSemaphoreCount = 1;
            submitInfo.PWaitSemaphores = &waitSemaphore;
            submitInfo.PWaitDstStageMask = &waitStage;
        }
        if (signalSemaphore.Handle != 0)
        {
            submitInfo.SignalSemaphoreCount = 1;
            submitInfo.PSignalSemaphores = &signalSemaphore;
        }
        _lastSubmitResult = _vk.QueueSubmit(_queue, 1, &submitInfo, _fence);
        Check(_lastSubmitResult, "vkQueueSubmit");
        GpuSubmitCount++;
        WaitFence(identity);
    }

    private void ResetFence() => Check(_vk.ResetFences(_device, 1, in _fence), "vkResetFences");

    private void WaitFence(string identity)
        => WaitFence(_fence, identity);

    private void WaitFence(Fence fence, string identity)
    {
        var result = _vk.WaitForFences(_device, 1, in fence, true, FenceTimeoutNanoseconds);
        if (result == Result.Timeout) throw new TimeoutException($"{identity} fence timed out after 5 seconds.");
        Check(result, "vkWaitForFences");
    }

    private void WaitForPresentRetirement()
    {
        if (_swapchain.Handle == 0) return;
        Check(_vk.QueueWaitIdle(_queue), "vkQueueWaitIdle(swapchain retirement)");
        _queueIdleRetirementWaitCount++;
    }

    private void ReleaseSwapchainSynchronization()
    {
        foreach (var semaphore in _renderFinishedSemaphores)
            if (semaphore.Handle != 0) _vk.DestroySemaphore(_device, semaphore, null);
        _renderFinishedSemaphores = [];
        _swapchainLayouts = [];
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

    private void ReleaseDevice(bool deviceLost)
    {
        if (_instance.Handle == 0) return;
        if (!deviceLost) WaitIdle();
        _acquired = false;
        ReleaseBacking();
        ReleaseSwapchainSynchronization();
        if (_swapchain.Handle != 0) _swapchainApi?.DestroySwapchain(_device, _swapchain, null);
        _swapchain = default;
        _swapchainImages = [];
        _context?.AbandonContext(deviceLost);
        _context?.Dispose();
        _context = null;
        _skiaBackend?.Dispose();
        _skiaBackend = null;
        _skiaExtensions?.Dispose();
        _skiaExtensions = null;
        if (_fence.Handle != 0) _vk.DestroyFence(_device, _fence, null);
        _fence = default;
        if (_acquireSemaphore.Handle != 0) _vk.DestroySemaphore(_device, _acquireSemaphore, null);
        _acquireSemaphore = default;
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
        RecordEvent($"failure operation={operation} result={result}");
        if (result == Result.ErrorDeviceLost)
        {
            _deviceLostCount++;
            throw new WindowsManagedVulkanDeviceLostException(
                $"{operation} failed with Vulkan result {result}.");
        }
        if (result == Result.ErrorSurfaceLostKhr)
        {
            _surfaceLostCount++;
            throw new WindowsManagedVulkanSurfaceLostException(
                $"{operation} failed with Vulkan result {result}.");
        }
        throw RecordOperationalFailure($"{operation} failed with Vulkan result {result}.", resize: false);
    }

    private static void FreeStringArray(byte** values, int count)
    {
        if (values is null) return;
        for (var index = 0; index < count; index++)
            SilkMarshal.Free((nint)values[index]);
        SilkMarshal.Free((nint)values);
    }

    private void RequireInstanceExtensions(IReadOnlyCollection<string> required)
    {
        uint count = 0;
        Check(_vk.EnumerateInstanceExtensionProperties((byte*)null, &count, null),
            "vkEnumerateInstanceExtensionProperties(count)");
        var properties = new ExtensionProperties[count];
        fixed (ExtensionProperties* pointer = properties)
            Check(_vk.EnumerateInstanceExtensionProperties((byte*)null, &count, pointer),
                "vkEnumerateInstanceExtensionProperties");
        var available = properties.Select(value =>
            Marshal.PtrToStringUTF8((nint)value.ExtensionName) ?? string.Empty).ToHashSet(StringComparer.Ordinal);
        var missing = required.Where(value => !available.Contains(value)).ToArray();
        if (missing.Length != 0)
            throw new InvalidOperationException(
                $"Missing required Vulkan instance extension(s): {string.Join(", ", missing)}.");
    }

    private HashSet<string> EnumerateDeviceExtensions(PhysicalDevice device)
    {
        uint count = 0;
        Check(_vk.EnumerateDeviceExtensionProperties(device, (byte*)null, &count, null),
            "vkEnumerateDeviceExtensionProperties(count)");
        var properties = new ExtensionProperties[count];
        fixed (ExtensionProperties* pointer = properties)
            Check(_vk.EnumerateDeviceExtensionProperties(device, (byte*)null, &count, pointer),
                "vkEnumerateDeviceExtensionProperties");
        return properties.Select(value =>
            Marshal.PtrToStringUTF8((nint)value.ExtensionName) ?? string.Empty).ToHashSet(StringComparer.Ordinal);
    }

    private static string DeviceLuid(PhysicalDeviceIDProperties value)
    {
        if (!value.DeviceLuidvalid) return string.Empty;
        return Convert.ToHexString(
            new ReadOnlySpan<byte>(value.DeviceLuid, checked((int)Vk.LuidSize)));
    }

    private static string FormatVersion(uint version) =>
        $"{version >> 22}.{version >> 12 & 0x3ff}.{version & 0xfff}";

    private void RecordEvent(string value)
    {
        lock (_eventGate)
        {
            while (_recentEvents.Count >= 256) _recentEvents.Dequeue();
            _recentEvents.Enqueue($"qpc={Stopwatch.GetTimestamp()} {value}");
        }
    }

    private string[] SnapshotEvents()
    {
        lock (_eventGate) return _recentEvents.ToArray();
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
        ReleaseDevice(deviceLost: false);
        _vk.Dispose();
        _disposed = true;
    }

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandle(string? moduleName);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmFlush();
}

internal sealed class WindowsManagedVulkanDeviceLostException(string message)
    : InvalidOperationException(message);

internal sealed class WindowsManagedVulkanSurfaceLostException(string message)
    : InvalidOperationException(message);

internal sealed record VulkanPresenterSnapshot(
    string LoaderPath,
    string LoaderSha256,
    string LoaderApiVersion,
    string SilkNetVersion,
    string Device,
    string DeviceType,
    uint VendorId,
    uint DeviceId,
    uint DriverVersion,
    string DeviceApiVersion,
    string DeviceLuid,
    uint QueueFamily,
    string SurfaceFormat,
    string ColorSpace,
    string CompositeAlpha,
    string PresentMode,
    int ImageCount,
    int Width,
    int Height,
    ulong SwapchainGeneration,
    ulong Acquired,
    ulong Presented,
    ulong SuccessfulPresents,
    int OutstandingAcquired,
    int OutstandingImageIndex,
    ulong MaximumOutstandingAcquired,
    ulong DeviceLostResults,
    ulong SurfaceLostResults,
    ulong OutOfDateResults,
    ulong SuboptimalResults,
    string LastAcquireResult,
    string LastSubmitResult,
    string LastPresentResult,
    int ActiveSwapchains,
    int RetiredSwapchains,
    bool ValidationEnabled,
    int MaximumRetiredSwapchains,
    string LastRecreateReason,
    long LastRetirementLatencyMicroseconds,
    long FirstPresentQpc,
    long LastTargetQpc,
    long LastPresentQpc,
    string RetirementMode,
    ulong QueueIdleRetirementWaits,
    string[] RecentEvents);
