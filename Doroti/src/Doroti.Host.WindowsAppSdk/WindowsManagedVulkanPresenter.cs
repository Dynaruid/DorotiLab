using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Doroti.Skia.RuntimeEffects;
using Microsoft.UI.Composition.SystemBackdrops;
using Silk.NET.Core;
using Silk.NET.Core.Contexts;
using Silk.NET.Core.Native;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;
using SkiaSharp;
using VkDevice = Silk.NET.Vulkan.Device;
using VkImage = Silk.NET.Vulkan.Image;
using VkSemaphore = Silk.NET.Vulkan.Semaphore;
using SystemComposition = Windows.UI.Composition;
using SystemCompositionDesktop = Windows.UI.Composition.Desktop;
using Brightness = Doroti.Ui.Brightness;
using WindowBackdropOptions = Doroti.Ui.WindowBackdropOptions;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedVulkanPresenter :
    WindowsManagedHwndPresenterBase,
    IWindowsAcrylicPresenter
{
    private const ulong FenceTimeoutNanoseconds = 5_000_000_000;
    private const ulong MaximumRetainedStorageAllocationBytes = 512UL * 1024 * 1024;
    private const uint VulkanApiVersion11 = (1u << 22) | (1u << 12);
    private const uint BufferAvailabilityWaitMilliseconds = 17;
    private const uint CompositionFrameWaitMilliseconds = 50;
    private const uint WaitObject0 = 0;
    private const uint WaitFailed = uint.MaxValue;
    private const int BufferCount = 3;
    private const int CapacityQuantum = 256;
    private const int DwmwaUseHostBackdropBrush = 17;
    private const int DwmwaSystemBackdropType = 38;
    private const int DwmSystemBackdropAuto = 0;
    private const int DwmSystemBackdropTransientWindow = 3;
    private const uint WmszLeft = 1;
    private const uint WmszTop = 3;
    private const uint WmszTopLeft = 4;
    private const uint WmszTopRight = 5;
    private const uint WmszBottomLeft = 7;
    private const ImageUsageFlags PresentationImageUsage = ImageUsageFlags.TransferDstBit;

    private readonly bool _diagnosticsEnabled;
    private readonly WindowsAcrylicOptionsState? _acrylicOptions;
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
    private KhrExternalMemoryWin32? _externalMemoryApi;
    private GRVkExtensions? _skiaExtensions;
    private GRSilkNetBackendContext? _skiaBackend;
    private GRContext? _context;
    private VkImage _backingImage;
    private DeviceMemory _backingMemory;
    private ulong _backingAllocationSize;
    private VkImage _retainedFrameImage;
    private DeviceMemory _retainedFrameMemory;
    private ulong _retainedFrameAllocationSize;
    private ImageLayout _retainedFrameLayout = ImageLayout.Undefined;
    private bool _retainedFrameInitialized;
    private Format _backingFormat;
    private int _backingCapacityWidth;
    private int _backingCapacityHeight;
    private int _surfaceWidth;
    private int _surfaceHeight;
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
    private bool _copySubmissionPending;
    private readonly PresentationSlot[] _presentationSlots =
        [new(), new(), new()];
    private nint _presentationContext;
    private nint _compositionSurfaceHandle;
    private int _selectedSlot = -1;
    private long _selectedViewportRevision;
    private ulong _presentTag;
    private readonly object _viewportGate = new();
    private SystemCompositionWorker? _composition;
    private VulkanAcrylicScene? _acrylicScene;
    private long _viewportRevision;
    private long _displayWaitViewportRevision;
    private ulong _resizeClockWaitCount;
    private ulong _resizeClockSignalCount;
    private ulong _resizeClockFailureCount;
    private uint _lastResizeClockStatus;
    private long _maximumResizeClockWaitMicroseconds;
    private int _viewportWidth;
    private int _viewportHeight;
    private double _viewportScale = 1;
    private nint _topLevelWindow;
    private bool _backdropTargetAdded;
    private bool _contentIslandConnected;
    private bool _desktopWindowTargetConnected;
    private bool _compositionSurfaceConnected;
    private bool _hostBackdropBrushEnabled;
    private bool _dwmSystemBackdropEnabled;
    private VulkanCompositionProbe _compositionProbe;
    private bool _presentationPoisoned;
    private bool _presentationRetiring;
    private bool _presentationDrainCommitted;
    private bool _compositionReleased;
    private Format _format;
    private uint _queueFamily;
    private uint _loaderApiVersion;
    private uint _deviceApiVersion;
    private uint _deviceVendorId;
    private uint _deviceId;
    private uint _adapterLuidLow;
    private int _adapterLuidHigh;
    private uint _driverVersion;
    private uint _maximumImageDimension2D;
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
    private ulong _backingAllocationCount;
    private ulong _backingReuseCount;
    private ulong _retainedSurfaceReuseCount;
    private ulong _surfaceRecreateCount;
    private ulong _deferredCopySubmissionCount;
    private ulong _copyFenceWaitCount;
    private ulong _compositionFrameWaitCount;
    private ulong _compositionFrameObservedCount;
    private ulong _compositionFrameWaitTimeoutCount;
    private ulong _fixedOriginPreGeometryAdmissionCount;
    private ulong _movingOriginPreGeometryAdmissionCount;
    private ulong _movingOriginPreGeometryDisplayWaitCount;
    private ulong _maximumRegisteredPresentationSlots;
    private ulong _unavailablePresentationSkipCount;
    private long _lastCompositionFrameWaitMicroseconds;
    private long _maximumCompositionFrameWaitMicroseconds;
    private long _lastCopyFenceWaitMicroseconds;
    private long _maximumCopyFenceWaitMicroseconds;
    private ulong _deviceLostCount;
    private ulong _surfaceLostCount;
    private ulong _outOfDateCount;
    private ulong _suboptimalCount;
    private int _maximumRetiredSwapchains;
    private long _lastRetirementLatencyMicroseconds;
    private long _lastRecreateLatencyMicroseconds;
    private long _maximumRecreateLatencyMicroseconds;
    private long _lastSwapchainCreateLatencyMicroseconds;
    private long _maximumSwapchainCreateLatencyMicroseconds;
    private long _lastBackingWrapLatencyMicroseconds;
    private long _maximumBackingWrapLatencyMicroseconds;
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
    private bool _debugBaselineSealed;
    private bool _contextAbandoned;
    private bool _rendererReleasePreflightReportedDeviceLoss;
    private bool _disposed;

    internal WindowsManagedVulkanPresenter(
        bool enableDiagnostics,
        WindowBackdropOptions? acrylicOptions = null,
        Brightness systemBrightness = Brightness.light)
    {
        _diagnosticsEnabled = enableDiagnostics;
        if (acrylicOptions is not null)
            _acrylicOptions = new WindowsAcrylicOptionsState(
                acrylicOptions, systemBrightness);
        _loaderPath = Path.GetFullPath(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System), "vulkan-1.dll"));
        if (!File.Exists(_loaderPath))
            throw new FileNotFoundException("The System32 Vulkan loader is missing.", _loaderPath);
        using (var loaderStream = File.OpenRead(_loaderPath))
            _loaderSha256 = Convert.ToHexString(SHA256.HashData(loaderStream)).ToLowerInvariant();
        _vk = new Vk(new DefaultNativeContext(_loaderPath));
        RecordEvent($"loader-open path={_loaderPath}");
    }

    internal override string BackendName => "Vulkan/Composition-Swapchain";
    internal override string RuntimeEffectsBackend => DorotiSkiaRuntimeEffects.WindowsVulkanBackend;
    internal override ulong NativeRequiredFeatures =>
        WindowsNativeV1.PostPresentDwmFlushFeature |
        WindowsNativeV1.RetainedOversizedChildSurfaceFeature |
        WindowsNativeV1.CompositionPresentationFeature |
        WindowsNativeV1.PreparedGeometryReceiptFeature |
        (_acrylicOptions is null
            ? 0
            : WindowsNativeV1.ExperimentalAcrylicFeature |
              WindowsNativeV1.VulkanAcrylicFeature);
    internal override bool UsesCompositionTopology => true;
    internal override string VisibleOwner => _acrylicOptions is null
        ? "top-level HWND DirectComposition Vulkan Presentation target"
        : "top-level HWND DirectComposition Vulkan Presentation target over a top-level Desktop Acrylic window target";
    internal override string TopologySlug => _acrylicOptions is null
        ? "top-level-dcomp-vulkan-presentation-synchronous"
        : "top-level-dcomp-vulkan-presentation-synchronous-acrylic";
    internal override bool InvalidatesRendererSurfaceResourcesOnResize => false;
    internal override string DiagnosticCoverage =>
        "Vulkan 1.1 retained offscreen backing, exact-LUID D3D11 Presentation buffers, dedicated D3D11_TEXTURE imports, " +
        "external queue-family ownership transfers, CPU copy-fence completion before native Present, three-slot availability retirement, " +
        "exact proposed-size Skia raster with non-visible moving-origin preparation, bounded pre-geometry compositor-clock alignment and immediate WM_WINDOWPOSCHANGED commit; fixed-origin submission retains its pre-geometry DWM wait, a native topmost DirectComposition target on the top-level HWND, " +
        "identity full-capacity Presentation coverage clipped by the single top-level client geometry, " +
        (_acrylicOptions is null
            ? "opaque alpha, "
            : "premultiplied content over a host-backdrop-enabled DesktopAcrylicController window target with a DWM transient-backdrop resize underlay, ") +
        "checked VkResult/HRESULT values, and bounded actual-size fallback";
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
        BufferCount, _surfaceWidth, _surfaceHeight,
        _retainedSurfaceReuseCount, 0,
        _backingCapacityWidth, _backingCapacityHeight,
        _backingAllocationSize, _retainedFrameAllocationSize, _retainedFrameInitialized,
        _backingAllocationCount, _backingReuseCount,
        _deferredCopySubmissionCount, _copyFenceWaitCount,
        _lastCopyFenceWaitMicroseconds, _maximumCopyFenceWaitMicroseconds,
        Width, Height, _swapchainGeneration,
        _acquiredCount, _presentTerminalCount, PresentCount,
        _acquired ? 1 : 0, _acquired ? checked((int)_acquiredImageIndex) : -1,
        _copySubmissionPending ? 1 : 0,
        _maximumOutstandingAcquired, _deviceLostCount, _surfaceLostCount,
        _outOfDateCount, _suboptimalCount,
        _lastAcquireResult.ToString(), _lastSubmitResult.ToString(),
        _lastPresentResult.ToString(),
        ActiveSwapchains: 0,
        RetiredSwapchains: 0,
        ValidationEnabled: false,
        MaximumRetiredSwapchains: 0,
        LastRecreateReason: _lastRecreateReason,
        LastRetirementLatencyMicroseconds: _lastRetirementLatencyMicroseconds,
        LastRecreateLatencyMicroseconds: _lastRecreateLatencyMicroseconds,
        MaximumRecreateLatencyMicroseconds: _maximumRecreateLatencyMicroseconds,
        LastSwapchainCreateLatencyMicroseconds: _lastSwapchainCreateLatencyMicroseconds,
        MaximumSwapchainCreateLatencyMicroseconds: _maximumSwapchainCreateLatencyMicroseconds,
        LastBackingWrapLatencyMicroseconds: _lastBackingWrapLatencyMicroseconds,
        MaximumBackingWrapLatencyMicroseconds: _maximumBackingWrapLatencyMicroseconds,
        FirstPresentQpc: _firstPresentQpc,
        LastTargetQpc: _lastTargetQpc,
        LastPresentQpc: _lastPresentQpc,
        RetirementMode: "presentation-buffer-availability",
        QueueIdleRetirementWaits: 0,
        CompositionFrameWaits: _compositionFrameWaitCount,
        CompositionFrameObserved: _compositionFrameObservedCount,
        CompositionFrameWaitTimeouts: _compositionFrameWaitTimeoutCount,
        FixedOriginPreGeometryAdmissions: _fixedOriginPreGeometryAdmissionCount,
        MovingOriginPreGeometryAdmissions: _movingOriginPreGeometryAdmissionCount,
        MovingOriginPreGeometryDisplayWaits: _movingOriginPreGeometryDisplayWaitCount,
        MovingOriginPrepared: PreparedDiagnostics.Prepared,
        MovingOriginWindowPosCommitAttempt: MovingOriginWindowPosCommitAttempt,
        MovingOriginWindowPosCommitted: PreparedDiagnostics.Committed,
        MovingOriginWindowPosMismatch: MovingOriginWindowPosCommitMismatch,
        MovingOriginWindowPosCancelled: PreparedDiagnostics.Cancelled,
        MovingOriginWindowPosFailed: MovingOriginWindowPosCommitFailed,
        MovingOriginReserved: PreparedDiagnostics.Reserved,
        ClockWait: 0, ClockWaitObserved: 0, ClockWaitTimeout: 0,
        PostGeometryFallback: _postGeometryFallback,
        CandidatePolicy: "moving-origin-clock-geometry-prepared-commit-receipt",
        LastCompositionFrameWaitMicroseconds: _lastCompositionFrameWaitMicroseconds,
        MaximumCompositionFrameWaitMicroseconds: _maximumCompositionFrameWaitMicroseconds,
        ResizeClockWaits: _resizeClockWaitCount,
        ResizeClockSignals: _resizeClockSignalCount,
        ResizeClockFailures: _resizeClockFailureCount,
        LastResizeClockStatus: _lastResizeClockStatus,
        MaximumResizeClockWaitMicroseconds: _maximumResizeClockWaitMicroseconds,
        RecentEvents: SnapshotEvents());

    bool IWindowsAcrylicPresenter.AcrylicEnabled => _acrylicOptions is not null;

    void IWindowsAcrylicPresenter.ApplySystemBrightness(Brightness brightness) =>
        ApplySystemBrightness(brightness);

    internal void ApplySystemBrightness(Brightness brightness)
    {
        _acrylicOptions?.ApplySystemBrightness(brightness);
    }

    ValueTask<ReadOnlyMemory<byte>?> IWindowsAcrylicPresenter.HandleRuntimeMessageAsync(
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken)
    {
        var options = _acrylicOptions ?? throw new InvalidOperationException(
            "The Vulkan presenter was not configured for Acrylic.");
        return options.HandleRuntimeMessageAsync(
            data, cancellationToken, CreateAcrylicSnapshot);
    }

    AcrylicPresenterSnapshot IWindowsAcrylicPresenter.Snapshot() =>
        CreateAcrylicSnapshot();

    private AcrylicPresenterSnapshot CreateAcrylicSnapshot()
    {
        var state = _acrylicOptions ?? throw new InvalidOperationException(
            "The Vulkan presenter was not configured for Acrylic.");
        var options = state.Options;
        var backdropState = _composition is not null && _acrylicScene is not null
            ? _composition.Invoke(() => _acrylicScene.State)
            : null;
        return new AcrylicPresenterSnapshot(
            options.mode.ToString(), options.mode.ToString(), null,
            _compositionProbe.PresentationSupported != 0,
            _compositionProbe.IndependentFlipSupported != 0,
            $"{_adapterLuidHigh}:{_adapterLuidLow}",
            _deviceVendorId, _deviceId,
            options.acrylicKind.ToString(), state.EffectiveTheme,
            options.tintColor?.value, options.tintOpacity, options.luminosityOpacity,
            WindowsManagedAcrylicCompositionPresenter.LogicalEdgeBudget,
            WindowsManagedAcrylicCompositionPresenter.PhysicalEdgeBudget,
            _maximumRegisteredPresentationSlots,
            _backingReuseCount,
            _unavailablePresentationSkipCount,
            state.AcceptedOptionRevisions,
            state.AppliedOptionRevisions,
            state.SupersededOptionRevisions,
            state.FailedOptionRevisions,
            _backdropTargetAdded,
            _contentIslandConnected,
            _desktopWindowTargetConnected,
            _hostBackdropBrushEnabled,
            BackdropTransport: "DesktopAcrylicController",
            SystemBackdropType: _dwmSystemBackdropEnabled ? "TransientWindowUnderlay" : null,
            RedirectionBitmapAlphaEnabled: false,
            BackdropState: backdropState);
    }

    internal bool HasPendingInjectedResult
    {
        get
        {
            var requested = Environment.GetEnvironmentVariable(
                "DOROTI_WINDOWS_VULKAN_INJECT_RESULT")?.Trim();
            return !string.IsNullOrWhiteSpace(requested) && !_consumedInjections.Contains(requested);
        }
    }

    internal override void AttachWindow(nint topLevelWindow)
    {
        AttachTopLevelWindow(topLevelWindow);
    }

    internal void AttachTopLevelWindow(nint topLevelWindow)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (topLevelWindow == 0) throw new ArgumentOutOfRangeException(nameof(topLevelWindow));
        if (_topLevelWindow != 0)
            throw new InvalidOperationException("The Vulkan Composition presenter already owns a window.");

        _compositionReleased = false;
        _topLevelWindow = topLevelWindow;
        try
        {
            if (_acrylicOptions is not null)
            {
                InitializeAcrylicTarget(topLevelWindow);
                RecordEvent("Desktop Acrylic window target connected; synchronous top-level Vulkan target pending first surface");
            }
            else
            {
                RecordEvent("top-level HWND attached; synchronous Vulkan target pending first surface");
            }
        }
        catch
        {
            ReleaseAcrylicTarget();
            _topLevelWindow = 0;
            throw;
        }
    }

    private void InitializeAcrylicTarget(nint topLevelWindow)
    {
        if (_composition is not null || _acrylicScene is not null)
            throw new InvalidOperationException("The Vulkan Acrylic window-target topology is already initialized.");
        var acrylic = _acrylicOptions ?? throw new InvalidOperationException(
            "The Vulkan presenter was not configured for Acrylic.");
        if (!DesktopAcrylicController.IsSupported())
            throw new PlatformNotSupportedException(
                "Desktop Acrylic is not supported by this Windows session.");

        SetHostBackdropBrush(topLevelWindow, enabled: true, throwOnFailure: true);
        SetDwmSystemBackdrop(topLevelWindow, enabled: true, throwOnFailure: true);

        _composition = new SystemCompositionWorker();
        var composition = _composition;
        _acrylicScene = composition.Invoke(() => new VulkanAcrylicScene(
            composition.Compositor, topLevelWindow,
            acrylic.Options, acrylic.SystemBrightness));
        _backdropTargetAdded = _acrylicScene.BackdropTargetAdded;
        _desktopWindowTargetConnected = _acrylicScene.DesktopWindowTargetConnected;
        if (!_backdropTargetAdded)
            throw new InvalidOperationException(
                "DesktopAcrylicController.SetTarget did not attach to the top-level window target.");
        if (!_desktopWindowTargetConnected)
            throw new InvalidOperationException(
                "The Vulkan Desktop Acrylic window target did not connect.");
        var scene = _acrylicScene;
        acrylic.Attach((options, brightness) =>
            composition.Invoke(() => scene.ApplyAcrylic(options, brightness)));
    }

    internal override void ResizeViewport(
        int width, int height, double scale, uint sizingEdge, bool preGeometry)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (!double.IsFinite(scale) || scale <= 0) throw new ArgumentOutOfRangeException(nameof(scale));
        if (_topLevelWindow == 0)
            throw new InvalidOperationException("The Vulkan top-level window is not attached.");

        lock (_viewportGate)
        {
            var movingOrigin = SizingEdgeMovesWindowOrigin(sizingEdge);
            if (!preGeometry && _viewportRevision != 0) _postGeometryFallback++;
            _preparedMoving.Cancel();
            _movingPrepareRequest = null;
            _phaseAlignedFrame = null;
            _viewportRevision++;
            _viewportWidth = width;
            _viewportHeight = height;
            _viewportScale = scale;
            // Both classes submit their exact proposed-size Presentation frame
            // before USER32 changes geometry. Fixed-origin growth additionally
            // waits until DWM observes it. Moving-origin submission deliberately
            // does not wait: forcing that frame to scan out at the old screen
            // origin creates the double movement reported during left/top drag.
            if (preGeometry && sizingEdge != 0)
            {
                if (movingOrigin)
                    _movingOriginPreGeometryAdmissionCount++;
                else
                    _fixedOriginPreGeometryAdmissionCount++;
            }
            var waitForDisplayBoundary = !preGeometry || !movingOrigin;
            if (preGeometry && movingOrigin && waitForDisplayBoundary)
                _movingOriginPreGeometryDisplayWaitCount++;
            _displayWaitViewportRevision = waitForDisplayBoundary
                ? _viewportRevision
                : 0;
        }
    }

    private static bool SizingEdgeMovesWindowOrigin(uint sizingEdge) =>
        sizingEdge is WmszLeft or WmszTop or WmszTopLeft or
            WmszTopRight or WmszBottomLeft;

    internal override bool EnsureTarget(nint childWindow, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        LastPresentSucceeded = false;
        if (childWindow == 0) throw new ArgumentOutOfRangeException(nameof(childWindow));
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        _lastTargetQpc = Stopwatch.GetTimestamp();

        if (_topLevelWindow == 0)
            throw new InvalidOperationException("The Vulkan top-level topology is not attached.");
        if (_presentationPoisoned)
            throw new InvalidOperationException(
                "The Vulkan Composition manager is poisoned after an indeterminate Present failure.");
        if (_window != 0 && _window != childWindow)
            throw new InvalidOperationException("The Vulkan bootstrap child HWND changed unexpectedly.");
        _window = childWindow;
        EnsureDevice(childWindow);

        var capacityWidth = width;
        var capacityHeight = height;
        // The retained backing can grow when its top-level parent moves to a
        // larger monitor without changing the logical viewport. Always fold
        // its current capacity into the backing requirement; checking only on
        // first allocation leaves the guard stranded at the creation monitor.
        if (GetClientRect(childWindow, out var retainedClient))
        {
            capacityWidth = Math.Max(capacityWidth, retainedClient.Right - retainedClient.Left);
            capacityHeight = Math.Max(capacityHeight, retainedClient.Bottom - retainedClient.Top);
        }
        capacityWidth = RoundCapacity(capacityWidth);
        capacityHeight = RoundCapacity(capacityHeight);
        var resized = Width != 0 && (Width != width || Height != height);
        EnsureBackingCapacity(capacityWidth, capacityHeight);
        EnsureAcrylicTarget();
        if (_presentationSlots.All(static slot => !slot.Registered))
        {
            for (var index = 0; index < BufferCount; index++)
                ReplacePresentationSlot(index, _backingCapacityWidth, _backingCapacityHeight);
        }

        _selectedSlot = SelectAvailablePresentationSlot(
            _backingCapacityWidth, _backingCapacityHeight);
        if (_selectedSlot < 0 && WaitForAnyPresentationSlot())
            _selectedSlot = SelectAvailablePresentationSlot(
                _backingCapacityWidth, _backingCapacityHeight);
        if (_selectedSlot < 0)
        {
            _lastAcquireResult = Result.NotReady;
            _unavailablePresentationSkipCount++;
            return false;
        }
        var slot = _presentationSlots[_selectedSlot];
        if (!slot.Registered || slot.CapacityWidth != _backingCapacityWidth ||
            slot.CapacityHeight != _backingCapacityHeight)
            ReplacePresentationSlot(
                _selectedSlot, _backingCapacityWidth, _backingCapacityHeight);

        lock (_viewportGate)
        {
            if (_presentationRetiring || _viewportRevision == 0 ||
                _viewportWidth != width || _viewportHeight != height)
            {
                _selectedSlot = -1;
                return false;
            }
            Width = width;
            Height = height;
            _selectedViewportRevision = _viewportRevision;
        }
        if (resized) ResizeBuffersCount++;
        _acquired = true;
        _acquiredImageIndex = checked((uint)_selectedSlot);
        _maximumOutstandingAcquired = Math.Max(_maximumOutstandingAcquired, 1);
        _lastAcquireResult = Result.Success;
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
        if (_selectedSlot is < 0 or >= BufferCount || _presentationContext == 0)
            throw new InvalidOperationException("No Vulkan Composition buffer is admitted.");

        LastPresentSucceeded = false;
        MovingFrameKey? prepareRequest;
        lock (_viewportGate) prepareRequest = _movingPrepareRequest;
        try
        {
            // The raster target and client clip share one top-level HWND geometry.
            // Paint replacements at the same origin; an edge-based
            // offset would make the entire scene jump at raster cadence.
            var result = paint(backing);
            backing.Canvas.Flush();
            context.Flush(backing);
            context.Submit(false);
            GpuSubmitCount++;
            if (!shouldPresent(result)) return result;

            if (TakeInjectedResult("OUT_OF_DATE"))
            {
                _outOfDateCount++;
                _lastAcquireResult = Result.ErrorOutOfDateKhr;
                Width = Height = 0;
                return result;
            }
            if (TakeInjectedResult("SURFACE_LOST"))
            {
                _surfaceLostCount++;
                _lastAcquireResult = Result.ErrorSurfaceLostKhr;
                throw new WindowsManagedVulkanSurfaceLostException(
                    "Synthetic Composition surface loss was requested before copy.");
            }
            if (TakeInjectedResult("DEVICE_LOST"))
            {
                _deviceLostCount++;
                _lastSubmitResult = Result.ErrorDeviceLost;
                throw new WindowsManagedVulkanDeviceLostException(
                    "Synthetic Vulkan device loss was requested before copy.");
            }

            var slotIndex = _selectedSlot;
            var slot = _presentationSlots[slotIndex];
            lock (_viewportGate)
            {
                if (!shouldPresent(result) ||
                    _viewportRevision != _selectedViewportRevision ||
                    _viewportWidth != Width || _viewportHeight != Height)
                    return result;
                // Keep the viewport authority stable while the retained guard
                // is updated and committed. Otherwise a superseded raster could
                // replace pixels that belong to the last displayed geometry.
                CopyBackingToPresentation(slot);
                GpuCopyCount++;
                if (prepareRequest is { } prepareKey)
                {
                    if (_movingPrepareRequest != prepareRequest) return result;
                    _preparedMoving.Reserve(new PreparedMovingFrame(
                        prepareKey, slotIndex, _viewportRevision));
                    TracePreparedCopyComplete();
                    LastPrepareSucceeded = true;
                    return result;
                }
                PresentSlotLocked(
                    slotIndex, _viewportRevision,
                    _displayWaitViewportRevision == _viewportRevision
                        ? "pregeometry-display-gated-or-exact-fallback"
                        : "pregeometry-moving-origin-submit");
            }
            if (LastPresentSucceeded &&
                Environment.GetEnvironmentVariable("DOROTI_WINDOWS_DWM_FLUSH") == "1")
                Marshal.ThrowExceptionForHR(DwmFlush());
            return result;
        }
        finally
        {
            _selectedSlot = -1;
            _selectedViewportRevision = 0;
            _acquired = false;
        }
    }

    private void PresentSlotLocked(int slotIndex, long viewportRevision, string admission, bool waitForResizeReceipt = false)
    {
        var slot = _presentationSlots[slotIndex];
        var waitForCompositionFrame = waitForResizeReceipt || _displayWaitViewportRevision == viewportRevision;
        var waitStarted = waitForCompositionFrame ? Stopwatch.GetTimestamp() : 0;
        // Keep the full retained buffer at identity. The top-level client is
        // the only raster clip; no transform moves the scene independently
        // from HWND geometry.
        var present = PresentCropped(
            _presentationContext, checked((uint)slotIndex),
            0, 0,
            checked((uint)slot.CapacityWidth),
            checked((uint)slot.CapacityHeight), ++_presentTag,
            waitForResizeReceipt ? 2u : waitForCompositionFrame ? 1u : 0u,
            CompositionFrameWaitMilliseconds,
            out var compositionFrameObserved,
            out var presentId, out var retiringFenceValue);
        if (present < 0)
        {
            slot.Poisoned = true;
            _presentationPoisoned = true;
            _lastPresentResult = Result.ErrorUnknown;
            RecordEvent($"composition present failed hresult=0x{unchecked((uint)present):x8}");
            Marshal.ThrowExceptionForHR(present);
        }
        var compositionFrameWasObserved = false;
        var compositionFrameReady = !waitForCompositionFrame;
        if (waitForCompositionFrame)
        {
            _compositionFrameWaitCount++;
            _lastCompositionFrameWaitMicroseconds = checked((long)
                Stopwatch.GetElapsedTime(waitStarted).TotalMicroseconds);
            _maximumCompositionFrameWaitMicroseconds = Math.Max(
                _maximumCompositionFrameWaitMicroseconds,
                _lastCompositionFrameWaitMicroseconds);
            compositionFrameWasObserved = compositionFrameObserved != 0;
            compositionFrameReady = compositionFrameWasObserved;
            if (compositionFrameReady)
            {
                _compositionFrameObservedCount++;
                _displayWaitViewportRevision = 0;
            }
            else
            {
                _compositionFrameWaitTimeoutCount++;
            }
        }
        var suboptimal = TakeInjectedResult("SUBOPTIMAL");
        _lastPresentResult = suboptimal ? Result.SuboptimalKhr : Result.Success;
        if (suboptimal) _suboptimalCount++;
        _acquiredCount++;
        _presentTerminalCount++;
        _surfaceWidth = slot.CapacityWidth;
        _surfaceHeight = slot.CapacityHeight;
        PresentCount++;
        if (compositionFrameReady)
        {
            LastPresentSucceeded = true;
            _lastPresentQpc = Stopwatch.GetTimestamp();
            if (_firstPresentQpc == 0) _firstPresentQpc = _lastPresentQpc;
        }
        RecordEvent(
            $"composition present slot={slotIndex} id={presentId} retiring={retiringFenceValue} " +
            $"source=0,0,{slot.CapacityWidth}x{slot.CapacityHeight} " +
            $"viewport={Width}x{Height}@0,0 admission={admission} " +
            $"displayWait={(waitForCompositionFrame ? 1 : 0)} " +
            $"displayObserved={(compositionFrameWasObserved ? 1 : 0)}");
    }

    private void EnsureBackingCapacity(int requestedWidth, int requestedHeight)
    {
        if (_backingImage.Handle != 0 &&
            _backingCapacityWidth >= requestedWidth &&
            _backingCapacityHeight >= requestedHeight)
        {
            _backingReuseCount++;
            _retainedSurfaceReuseCount++;
            return;
        }

        if (_backingImage.Handle != 0) WaitIdle();
        var started = Stopwatch.GetTimestamp();
        CreateBacking(requestedWidth, requestedHeight);
        _lastBackingWrapLatencyMicroseconds = checked((long)
            Stopwatch.GetElapsedTime(started).TotalMicroseconds);
        _maximumBackingWrapLatencyMicroseconds = Math.Max(
            _maximumBackingWrapLatencyMicroseconds, _lastBackingWrapLatencyMicroseconds);
        _surfaceWidth = _backingCapacityWidth;
        _surfaceHeight = _backingCapacityHeight;
        _swapchainGeneration++;
        _lastRecreateReason = _swapchainGeneration == 1 ? "composition-initial" : "composition-capacity-grow";
        RecordEvent(
            $"composition backing generation={_swapchainGeneration} " +
            $"capacity={_backingCapacityWidth}x{_backingCapacityHeight}");
    }

    private int SelectAvailablePresentationSlot(int requiredWidth, int requiredHeight)
    {
        for (var index = 0; index < BufferCount; index++)
        {
            lock (_viewportGate)
                if (_preparedMoving.IsReserved(index)) continue;
            var slot = _presentationSlots[index];
            if (slot.Poisoned) continue;
            if (!slot.Registered) return index;
            if (slot.CapacityWidth != requiredWidth || slot.CapacityHeight != requiredHeight)
                continue;
            var result = IsCompositionBufferAvailable(
                _presentationContext, checked((uint)index), out var available);
            if (result < 0) Marshal.ThrowExceptionForHR(result);
            if (available != 0) return index;
        }
        for (var index = 0; index < BufferCount; index++)
        {
            var slot = _presentationSlots[index];
            if (slot.Poisoned) continue;
            if (!slot.Registered) return index;
            if (slot.CapacityWidth == requiredWidth && slot.CapacityHeight == requiredHeight)
                continue;
            var result = IsCompositionBufferAvailable(
                _presentationContext, checked((uint)index), out var available);
            if (result < 0) Marshal.ThrowExceptionForHR(result);
            if (available != 0) return index;
        }
        return -1;
    }

    private bool WaitForAnyPresentationSlot()
    {
        nint* handles = stackalloc nint[BufferCount];
        uint count = 0;
        for (var index = 0; index < _presentationSlots.Length; index++)
        {
            lock (_viewportGate)
                if (_preparedMoving.IsReserved(index)) continue;
            var slot = _presentationSlots[index];
            if (slot.Poisoned || !slot.Registered || slot.AvailableEvent == 0) continue;
            handles[count++] = unchecked((nint)slot.AvailableEvent);
        }
        if (count == 0) return false;
        var result = WaitForMultipleObjects(
            count, handles, false, BufferAvailabilityWaitMilliseconds);
        if (result == WaitFailed)
            throw new Win32Exception(
                Marshal.GetLastWin32Error(),
                "Waiting for a Vulkan Composition buffer failed.");
        return result >= WaitObject0 && result < WaitObject0 + count;
    }

    private void ReplacePresentationSlot(int index, int width, int height)
    {
        lock (_viewportGate)
            if (_preparedMoving.IsReserved(index))
                throw new InvalidOperationException("A prepared Presentation slot cannot be replaced.");
        var slot = _presentationSlots[index];
        if (slot.Poisoned)
            throw new InvalidOperationException("A failed Presentation slot cannot be replaced in-place.");
        ReleasePresentationSlotVulkan(slot);
        var snapshot = new VulkanCompositionBuffer
        {
            AbiVersion = 1,
            StructSize = checked((uint)sizeof(VulkanCompositionBuffer)),
        };
        var result = ReplaceCompositionBuffer(
            _presentationContext, checked((uint)index),
            checked((uint)width), checked((uint)height),
            out var sharedHandle, out var availableEvent, ref snapshot);
        if (result < 0) Marshal.ThrowExceptionForHR(result);
        if (sharedHandle == 0 || availableEvent == 0 || snapshot.InitiallyAvailable == 0)
            throw new InvalidOperationException(
                "The native Vulkan Composition buffer is incomplete or unavailable.");
        slot.Registered = true;
        slot.AvailableEvent = availableEvent;
        slot.CapacityWidth = width;
        slot.CapacityHeight = height;
        _maximumRegisteredPresentationSlots = Math.Max(
            _maximumRegisteredPresentationSlots,
            checked((ulong)_presentationSlots.Count(static item => item.Registered)));
        try
        {
            ImportPresentationTexture(slot, sharedHandle);
        }
        finally
        {
            if (!CloseHandle(unchecked((nint)sharedHandle)))
                RecordEvent($"CloseHandle(shared texture) failed win32={Marshal.GetLastWin32Error()}");
        }
    }

    private void ImportPresentationTexture(PresentationSlot slot, ulong sharedHandle)
    {
        var external = new ExternalMemoryImageCreateInfo
        {
            SType = StructureType.ExternalMemoryImageCreateInfo,
            HandleTypes = ExternalMemoryHandleTypeFlags.D3D11TextureBit,
        };
        var imageInfo = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo,
            PNext = &external,
            ImageType = ImageType.Type2D,
            Format = Format.B8G8R8A8Unorm,
            Extent = new Extent3D(
                checked((uint)slot.CapacityWidth), checked((uint)slot.CapacityHeight), 1),
            MipLevels = 1,
            ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal,
            Usage = PresentationImageUsage,
            SharingMode = SharingMode.Exclusive,
            InitialLayout = ImageLayout.Undefined,
        };
        Check(_vk.CreateImage(_device, &imageInfo, null, out slot.Image),
            "vkCreateImage(D3D11 import)");
        try
        {
            _vk.GetImageMemoryRequirements(_device, slot.Image, out var requirements);
            var handleProperties = new MemoryWin32HandlePropertiesKHR
            {
                SType = StructureType.MemoryWin32HandlePropertiesKhr,
            };
            Check(_externalMemoryApi!.GetMemoryWin32HandleProperties(
                _device, ExternalMemoryHandleTypeFlags.D3D11TextureBit,
                unchecked((nint)sharedHandle), &handleProperties),
                "vkGetMemoryWin32HandlePropertiesKHR(D3D11_TEXTURE)");
            var memoryTypeBits = requirements.MemoryTypeBits & handleProperties.MemoryTypeBits;
            if (memoryTypeBits == 0)
                throw new PlatformNotSupportedException(
                    "The D3D11 texture exposes no Vulkan-compatible memory type.");
            var dedicated = new MemoryDedicatedAllocateInfo
            {
                SType = StructureType.MemoryDedicatedAllocateInfo,
                Image = slot.Image,
            };
            var import = new ImportMemoryWin32HandleInfoKHR
            {
                SType = StructureType.ImportMemoryWin32HandleInfoKhr,
                PNext = &dedicated,
                HandleType = ExternalMemoryHandleTypeFlags.D3D11TextureBit,
                Handle = unchecked((nint)sharedHandle),
            };
            var allocation = new MemoryAllocateInfo
            {
                SType = StructureType.MemoryAllocateInfo,
                PNext = &import,
                AllocationSize = requirements.Size,
                MemoryTypeIndex = FindCompatibleMemoryType(memoryTypeBits),
            };
            Check(_vk.AllocateMemory(_device, &allocation, null, out slot.Memory),
                "vkAllocateMemory(D3D11 import)");
            Check(_vk.BindImageMemory(_device, slot.Image, slot.Memory, 0),
                "vkBindImageMemory(D3D11 import)");
            slot.Layout = ImageLayout.Undefined;
        }
        catch
        {
            if (slot.Memory.Handle != 0) _vk.FreeMemory(_device, slot.Memory, null);
            slot.Memory = default;
            if (slot.Image.Handle != 0) _vk.DestroyImage(_device, slot.Image, null);
            slot.Image = default;
            throw;
        }
    }

    private uint FindCompatibleMemoryType(uint typeFilter)
    {
        _vk.GetPhysicalDeviceMemoryProperties(_physicalDevice, out var properties);
        for (uint index = 0; index < properties.MemoryTypeCount; index++)
            if ((typeFilter & (1u << checked((int)index))) != 0 &&
                (properties.MemoryTypes[checked((int)index)].PropertyFlags &
                 MemoryPropertyFlags.DeviceLocalBit) != 0)
                return index;
        for (uint index = 0; index < properties.MemoryTypeCount; index++)
            if ((typeFilter & (1u << checked((int)index))) != 0) return index;
        throw new PlatformNotSupportedException(
            "No Vulkan memory type is compatible with the imported D3D11 texture.");
    }

    private void CopyBackingToPresentation(PresentationSlot slot)
    {
        if (_retainedFrameImage.Handle == 0)
            throw new InvalidOperationException("The Vulkan retained-frame image is unavailable.");
        if (Width <= 0 || Height <= 0 ||
            Width > _backingCapacityWidth || Height > _backingCapacityHeight)
            throw new InvalidOperationException(
                $"The Vulkan viewport {Width}x{Height} exceeds retained capacity " +
                $"{_backingCapacityWidth}x{_backingCapacityHeight}.");

        BeginCommands();
        var acquireBarriers = stackalloc ImageMemoryBarrier[3];
        acquireBarriers[0] = ImageBarrier(
            _backingImage, ImageLayout.ColorAttachmentOptimal, ImageLayout.TransferSrcOptimal,
            AccessFlags.ColorAttachmentWriteBit, AccessFlags.TransferReadBit);
        acquireBarriers[1] = ImageBarrier(
            _retainedFrameImage, _retainedFrameLayout, ImageLayout.TransferDstOptimal,
            _retainedFrameLayout == ImageLayout.Undefined
                ? 0
                : AccessFlags.TransferReadBit,
            AccessFlags.TransferWriteBit);
        acquireBarriers[2] = ExternalImageBarrier(
            slot.Image, slot.Layout, ImageLayout.TransferDstOptimal,
            Vk.QueueFamilyExternal, _queueFamily,
            0, AccessFlags.TransferWriteBit);
        _vk.CmdPipelineBarrier(
            _commandBuffer,
            PipelineStageFlags.TopOfPipeBit | PipelineStageFlags.ColorAttachmentOutputBit |
            PipelineStageFlags.TransferBit,
            PipelineStageFlags.TransferBit, 0,
            0, null, 0, null, 3, acquireBarriers);

        // Refresh the complete retained capacity. The renderer has cleared it
        // to the app background and painted this frame's exact viewport at the
        // top-level origin, so an old layout cannot remain in any overscan
        // pixels exposed by an asynchronously moving HWND.
        var retainedCopyFromBacking = new ImageCopy
        {
            SrcSubresource = ColorSubresourceLayers(),
            DstSubresource = ColorSubresourceLayers(),
            Extent = new Extent3D(
                checked((uint)_backingCapacityWidth),
                checked((uint)_backingCapacityHeight), 1),
        };
        _vk.CmdCopyImage(
            _commandBuffer, _backingImage, ImageLayout.TransferSrcOptimal,
            _retainedFrameImage, ImageLayout.TransferDstOptimal, 1,
            &retainedCopyFromBacking);

        var retainedReady = ImageBarrier(
            _retainedFrameImage, ImageLayout.TransferDstOptimal, ImageLayout.TransferSrcOptimal,
            AccessFlags.TransferWriteBit, AccessFlags.TransferReadBit);
        _vk.CmdPipelineBarrier(
            _commandBuffer, PipelineStageFlags.TransferBit, PipelineStageFlags.TransferBit,
            0, 0, null, 0, null, 1, &retainedReady);

        var retainedCopy = new ImageCopy
        {
            SrcSubresource = ColorSubresourceLayers(),
            DstSubresource = ColorSubresourceLayers(),
            Extent = new Extent3D(
                checked((uint)_backingCapacityWidth),
                checked((uint)_backingCapacityHeight), 1),
        };
        _vk.CmdCopyImage(
            _commandBuffer, _retainedFrameImage, ImageLayout.TransferSrcOptimal,
            slot.Image, ImageLayout.TransferDstOptimal, 1, &retainedCopy);

        var releaseBarriers = stackalloc ImageMemoryBarrier[2];
        releaseBarriers[0] = ImageBarrier(
            _backingImage, ImageLayout.TransferSrcOptimal, ImageLayout.ColorAttachmentOptimal,
            AccessFlags.TransferReadBit, AccessFlags.ColorAttachmentWriteBit);
        releaseBarriers[1] = ExternalImageBarrier(
            slot.Image, ImageLayout.TransferDstOptimal, ImageLayout.General,
            _queueFamily, Vk.QueueFamilyExternal,
            AccessFlags.TransferWriteBit, 0);
        _vk.CmdPipelineBarrier(
            _commandBuffer, PipelineStageFlags.TransferBit,
            PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.BottomOfPipeBit,
            0, 0, null, 0, null, 2, releaseBarriers);

        var started = Stopwatch.GetTimestamp();
        SubmitCommands("Vulkan Composition copy", waitForCompletion: true);
        _lastCopyFenceWaitMicroseconds = checked((long)
            Stopwatch.GetElapsedTime(started).TotalMicroseconds);
        _maximumCopyFenceWaitMicroseconds = Math.Max(
            _maximumCopyFenceWaitMicroseconds, _lastCopyFenceWaitMicroseconds);
        _copyFenceWaitCount++;
        _retainedFrameLayout = ImageLayout.TransferSrcOptimal;
        _retainedFrameInitialized = true;
        slot.Layout = ImageLayout.General;
    }

    private static ImageMemoryBarrier ExternalImageBarrier(
        VkImage image, ImageLayout oldLayout, ImageLayout newLayout,
        uint sourceQueueFamily, uint destinationQueueFamily,
        AccessFlags sourceAccess, AccessFlags destinationAccess) => new()
    {
        SType = StructureType.ImageMemoryBarrier,
        OldLayout = oldLayout,
        NewLayout = newLayout,
        SrcQueueFamilyIndex = sourceQueueFamily,
        DstQueueFamilyIndex = destinationQueueFamily,
        Image = image,
        SubresourceRange = new ImageSubresourceRange(
            ImageAspectFlags.ColorBit, 0, 1, 0, 1),
        SrcAccessMask = sourceAccess,
        DstAccessMask = destinationAccess,
    };

    private static int RoundCapacity(int value) =>
        checked(((value + CapacityQuantum - 1) / CapacityQuantum) * CapacityQuantum);

    private bool TryAcquireNextImage(Func<bool> shouldContinue, out uint imageIndex)
    {
        if (_acquired) throw new InvalidOperationException("A Vulkan swapchain image is already acquired.");
        imageIndex = 0;
        uint acquiredIndex = 0;
        var deadline = Environment.TickCount64 + 5_000;
        while (true)
        {
            if (!shouldContinue()) return false;
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

    internal override void ReleaseCompositionResources()
    {
        try
        {
            UnbindPresentationSurfaceForRetirement();
            WaitForPresentationRetirement();
        }
        finally
        {
            ReleaseCompositionTopology();
        }
    }

    private void ReleaseCompositionTopology()
    {
        if (_compositionReleased) return;
        _compositionReleased = true;
        _acrylicOptions?.Detach();
        ReleaseAcrylicTarget();
        RecordEvent(_acrylicOptions is null
            ? "synchronous top-level Vulkan topology released"
            : "synchronous top-level Vulkan plus Desktop Acrylic window-target topology released");
        _compositionSurfaceConnected = false;
        _topLevelWindow = 0;
        _backdropTargetAdded = false;
        _contentIslandConnected = false;
        _acrylicOptions?.Dispose();
    }

    private void ClearCompositionSurfaceBinding()
    {
        if (!_compositionSurfaceConnected) return;
        _compositionSurfaceConnected = false;
    }

    private void ReleaseAcrylicTarget()
    {
        if (_acrylicScene is not null && _composition is not null)
            _composition.Invoke(_acrylicScene.Dispose);
        _acrylicScene = null;
        _composition?.Dispose();
        _composition = null;
        if (_dwmSystemBackdropEnabled && _topLevelWindow != 0)
            SetDwmSystemBackdrop(_topLevelWindow, enabled: false, throwOnFailure: false);
        if (_hostBackdropBrushEnabled && _topLevelWindow != 0)
            SetHostBackdropBrush(_topLevelWindow, enabled: false, throwOnFailure: false);
    }

    internal override bool PrepareForRendererGpuResourceRelease()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        CancelPreparedMovingFrame();
        if (_instance.Handle == 0) return false;
        _rendererReleasePreflightReportedDeviceLoss = false;
        try
        {
            WaitIdle();
            _copySubmissionPending = false;
            return false;
        }
        catch (WindowsManagedVulkanDeviceLostException)
        {
            _rendererReleasePreflightReportedDeviceLoss = true;
            AbandonContextForDeviceLossCore();
            TryRecordEvent("device-loss context abandoned before renderer invalidation");
            TryRecordEvent("device loss observed during renderer-release preflight");
            return true;
        }
        catch (InvalidOperationException)
        {
            // A non-device-lost failure from vkDeviceWaitIdle still means that
            // GPU idleness was not established. Treat the backend as unsafe so
            // Skia wrappers are abandoned before renderer cache destruction and
            // the original failure is preserved. Because idleness was not
            // established, callers must not destroy native child objects.
            AbandonContextForDeviceLossCore();
            TryRecordEvent("renderer-release preflight failed; quarantining unsafe Vulkan context");
            throw;
        }
    }

    internal override bool TryAbandonGpuContextAfterRendererReleasePreflightFailure()
    {
        AbandonContextForDeviceLossCore();
        TryRecordEvent("renderer-release preflight threw; abandoning Vulkan context before renderer cleanup");
        return _context is null || _contextAbandoned;
    }

    internal override void ResetDeviceAfterRendererGpuResourceRelease(bool deviceLost)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (deviceLost)
            RecordRendererReleaseTeardown();
        ReleaseDevice(deviceLost, waitForIdle: false);
    }

    internal void RecoverAfterDeviceLoss()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RecordEvent("device-loss renderer resources invalidated; starting native teardown");
        _pendingRecreateReason = "device-loss-recovery";
        ReleaseDevice(deviceLost: true);
    }

    internal void AbandonContextForDeviceLoss()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        AbandonContextForDeviceLossCore();
        TryRecordEvent("device-loss context abandoned before renderer invalidation");
    }

    internal void RecoverAfterSurfaceLoss(bool deviceLost)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        RecordEvent(deviceLost
            ? "surface-loss recovery observed device loss during preflight"
            : "surface-loss recovery");
        if (deviceLost)
            RecordRendererReleaseTeardown();
        _pendingRecreateReason = deviceLost ? "device-loss-during-surface-recovery" : "surface-loss-recovery";
        ReleaseDevice(deviceLost, waitForIdle: false);
    }

    private void EnsureDevice(nint childWindow)
    {
        if (_device.Handle != 0) return;
        _window = childWindow;
        CreateInstance();
        SelectPhysicalDeviceAndQueue();
        RequireExternalImageImportSupport();
        CreateLogicalDevice();
        CreateSkiaContext();
        _compositionProbe = new VulkanCompositionProbe
        {
            AbiVersion = 1,
            StructSize = checked((uint)sizeof(VulkanCompositionProbe)),
        };
        var create = CreateComposition(
            _adapterLuidLow, _adapterLuidHigh, out _presentationContext,
            out var compositionSurfaceHandle, ref _compositionProbe);
        if (create < 0) Marshal.ThrowExceptionForHR(create);
        _compositionSurfaceHandle = unchecked((nint)compositionSurfaceHandle);
        if (create != 0 || _presentationContext == 0 || _compositionSurfaceHandle == 0 ||
            _compositionProbe.AdapterLuidMatched == 0 ||
            _compositionProbe.PresentationSupported == 0)
            throw new PlatformNotSupportedException(
                "The exact-LUID D3D11 device does not support Composition Swapchain presentation.");
        if (_compositionProbe.ActualAdapterLuidLow != unchecked((int)_adapterLuidLow) ||
            _compositionProbe.ActualAdapterLuidHigh != _adapterLuidHigh)
            throw new InvalidOperationException("The D3D11 and Vulkan adapter LUIDs differ.");
        if (_acrylicOptions is not null)
        {
            var alpha = SetCompositionPremultipliedAlpha(_presentationContext, 1);
            if (alpha < 0) Marshal.ThrowExceptionForHR(alpha);
        }
        ConnectCompositionSurface();
        RecordEvent(_acrylicOptions is null
            ? "Vulkan Presentation surface connected to synchronous top-level DirectComposition"
            : "premultiplied synchronous top-level Vulkan surface connected over Desktop Acrylic window target");
        _format = Format.B8G8R8A8Unorm;
        _colorSpace = "RGB_FULL_G22_NONE_P709";
        _compositeAlpha = _acrylicOptions is null ? "Ignore" : "Premultiplied";
        _presentMode = "CompositionSwapchain";
        lock (_viewportGate)
        {
            _presentationRetiring = false;
            _presentationDrainCommitted = false;
        }
        DeviceGeneration++;
        _debugBaselineSealed = false;
    }

    private void EnsureAcrylicTarget()
    {
        if (_acrylicOptions is null) return;
        if (_composition is null || _acrylicScene is null)
            throw new InvalidOperationException("The Vulkan Acrylic window target is unavailable.");
    }

    private void ConnectCompositionSurface()
    {
        if (_compositionSurfaceHandle == 0)
            throw new InvalidOperationException("The Vulkan Presentation surface handle is unavailable.");
        if (_topLevelWindow == 0)
            throw new InvalidOperationException("The Vulkan top-level HWND is unavailable.");
        var attach = AttachCompositionWindow(
            _presentationContext, unchecked((ulong)_topLevelWindow));
        if (attach < 0) Marshal.ThrowExceptionForHR(attach);
        _compositionSurfaceConnected = true;
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
                EnabledExtensionCount = 0,
                PpEnabledExtensionNames = null,
            };
            Check(_vk.CreateInstance(&createInfo, null, out _instance), "vkCreateInstance");
        }
        finally
        {
            SilkMarshal.Free((nint)applicationName);
            SilkMarshal.Free((nint)engineName);
        }
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
                if (!id.DeviceLuidvalid)
                {
                    rejectedCandidates.Add($"{name}: no valid Windows adapter LUID");
                    break;
                }
                var requiredExtensions = new[]
                {
                    "VK_KHR_external_memory",
                    KhrExternalMemoryWin32.ExtensionName,
                    "VK_KHR_get_memory_requirements2",
                    "VK_KHR_dedicated_allocation",
                };
                var missing = requiredExtensions.Where(value => !extensions.Contains(value)).ToArray();
                if (missing.Length != 0)
                {
                    rejectedCandidates.Add($"{name}: missing {string.Join(", ", missing)}");
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
                "No hardware Vulkan device satisfies the graphics/LUID/external-memory requirements" +
                (rejectedCandidates.Count == 0 ? "." : $": {string.Join("; ", rejectedCandidates)}."));
        var selector = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_VULKAN_DEVICE")?.Trim();
        var preference = WindowsGpuSelection.RequestedPreference;
        var selected = candidates[0];
        if (!string.IsNullOrWhiteSpace(selector))
        {
            var matches = candidates.Where(value =>
                value.Name.Equals(selector, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (matches.Length == 0)
                matches = candidates.Where(value =>
                    value.Name.Contains(selector, StringComparison.OrdinalIgnoreCase)).ToArray();
            if (matches.Length != 1)
                throw new InvalidOperationException(
                    $"DOROTI_WINDOWS_VULKAN_DEVICE='{selector}' did not match exactly one Vulkan device " +
                    $"(matched {matches.Length}). Available devices: {string.Join(", ", candidates.Select(value => value.Name))}.");
            selected = matches[0];
        }
        else
        {
            var eligibleLuids = candidates.Select(value => WindowsGpuSelection.ParseVulkanLuid(value.Luid)).ToArray();
            var luid = WindowsGpuSelection.SelectAdapter(preference, eligibleLuids);
            selected = candidates[Array.IndexOf(eligibleLuids, luid)];
        }
        RecordEvent($"gpu-selection preference={preference} override={selector ?? "none"} device={selected.Name} luid={selected.Luid}");
        if (selected.Properties.ApiVersion < VulkanApiVersion11)
            throw new InvalidOperationException($"Vulkan device '{selected.Name}' does not support Vulkan 1.1.");
        _physicalDevice = selected.Device;
        _queueFamily = selected.QueueFamily;
        _deviceApiVersion = selected.Properties.ApiVersion;
        _deviceVendorId = selected.Properties.VendorID;
        _deviceId = selected.Properties.DeviceID;
        _driverVersion = selected.Properties.DriverVersion;
        _maximumImageDimension2D = selected.Properties.Limits.MaxImageDimension2D;
        _deviceType = selected.Properties.DeviceType.ToString();
        _deviceLuid = selected.Luid;
        var selectedId = new PhysicalDeviceIDProperties
        {
            SType = StructureType.PhysicalDeviceIDProperties,
        };
        var selectedProperties = new PhysicalDeviceProperties2
        {
            SType = StructureType.PhysicalDeviceProperties2,
            PNext = &selectedId,
        };
        _vk.GetPhysicalDeviceProperties2(_physicalDevice, &selectedProperties);
        if (!selectedId.DeviceLuidvalid)
            throw new PlatformNotSupportedException(
                $"Vulkan device '{selected.Name}' does not expose a valid Windows adapter LUID.");
        _adapterLuidLow =
            (uint)selectedId.DeviceLuid[0] |
            (uint)selectedId.DeviceLuid[1] << 8 |
            (uint)selectedId.DeviceLuid[2] << 16 |
            (uint)selectedId.DeviceLuid[3] << 24;
        _adapterLuidHigh =
            selectedId.DeviceLuid[4] |
            selectedId.DeviceLuid[5] << 8 |
            selectedId.DeviceLuid[6] << 16 |
            selectedId.DeviceLuid[7] << 24;
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
        var extensionNames = new[]
        {
            "VK_KHR_external_memory",
            KhrExternalMemoryWin32.ExtensionName,
            "VK_KHR_get_memory_requirements2",
            "VK_KHR_dedicated_allocation",
        };
        var availableExtensions = EnumerateDeviceExtensions(_physicalDevice);
        var missingExtensions = extensionNames.Where(value => !availableExtensions.Contains(value)).ToArray();
        if (missingExtensions.Length != 0)
            throw new PlatformNotSupportedException(
                $"Vulkan device is missing required Composition import extension(s): {string.Join(", ", missingExtensions)}.");
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
        if (!_vk.TryGetDeviceExtension(_instance, _device, out _externalMemoryApi) ||
            _externalMemoryApi is null)
            throw new InvalidOperationException("VK_KHR_external_memory_win32 could not be loaded.");
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

    private void RequireExternalImageImportSupport()
    {
        var externalInfo = new PhysicalDeviceExternalImageFormatInfo
        {
            SType = StructureType.PhysicalDeviceExternalImageFormatInfo,
            HandleType = ExternalMemoryHandleTypeFlags.D3D11TextureBit,
        };
        var imageInfo = new PhysicalDeviceImageFormatInfo2
        {
            SType = StructureType.PhysicalDeviceImageFormatInfo2,
            PNext = &externalInfo,
            Format = Format.B8G8R8A8Unorm,
            Type = ImageType.Type2D,
            Tiling = ImageTiling.Optimal,
            Usage = PresentationImageUsage,
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
        Check(_vk.GetPhysicalDeviceImageFormatProperties2(
            _physicalDevice, &imageInfo, &properties),
            "vkGetPhysicalDeviceImageFormatProperties2(D3D11_TEXTURE)");
        var memory = externalProperties.ExternalMemoryProperties;
        if ((memory.ExternalMemoryFeatures & ExternalMemoryFeatureFlags.ImportableBit) == 0 ||
            (memory.ExternalMemoryFeatures & ExternalMemoryFeatureFlags.DedicatedOnlyBit) == 0 ||
            (memory.CompatibleHandleTypes & ExternalMemoryHandleTypeFlags.D3D11TextureBit) == 0)
            throw new PlatformNotSupportedException(
                "BGRA8 D3D11_TEXTURE import is not dedicated-only, importable, and compatible.");
    }

    private void CreateSkiaContext()
    {
        string[] instanceExtensions = [];
        string[] deviceExtensions =
        [
            "VK_KHR_external_memory",
            KhrExternalMemoryWin32.ExtensionName,
            "VK_KHR_get_memory_requirements2",
            "VK_KHR_dedicated_allocation",
        ];
        _skiaExtensions = new GRVkExtensions();
        _skiaExtensions.Initialize(
            GetVulkanProcedureAddress,
            _instance,
            _physicalDevice,
            instanceExtensions,
            deviceExtensions);
        _skiaBackend = new GRSilkNetBackendContext
        {
            VkInstance = _instance,
            VkPhysicalDevice = _physicalDevice,
            VkDevice = _device,
            VkQueue = _queue,
            GraphicsQueueIndex = _queueFamily,
            GetProcedureAddress = GetVulkanProcedureAddress,
            Extensions = _skiaExtensions,
            MaxAPIVersion = Math.Min(VulkanApiVersion11, _deviceApiVersion),
        };
        _context = GRContext.CreateVulkan(_skiaBackend)
            ?? throw new InvalidOperationException("Skia could not create the managed Vulkan context.");
        _contextAbandoned = false;
    }

    private nint GetVulkanProcedureAddress(
        string name, Instance instance, VkDevice device)
    {
        if (device.Handle != 0) return _vk.GetDeviceProcAddr(device, name);
        return _vk.GetInstanceProcAddr(instance, name);
    }

    private bool RecreateSwapchain(int width, int height, string reason)
    {
        var recreateStarted = Stopwatch.GetTimestamp();
        if (_acquired)
            throw new InvalidOperationException(
                "Swapchain recreation cannot begin inside an acquire-as-presentation-commit transaction.");
        var oldSwapchain = _swapchain;
        if (oldSwapchain.Handle != 0) _maximumRetiredSwapchains = Math.Max(_maximumRetiredSwapchains, 1);
        var retirementStarted = Stopwatch.GetTimestamp();
        WaitForPresentRetirement();
        _lastRetirementLatencyMicroseconds = checked((long)
            Stopwatch.GetElapsedTime(retirementStarted).TotalMicroseconds);
        Check(_surfaceApi!.GetPhysicalDeviceSurfaceCapabilities(
            _physicalDevice, _surface, out var capabilities),
            "vkGetPhysicalDeviceSurfaceCapabilitiesKHR");
        if ((capabilities.SupportedUsageFlags & ImageUsageFlags.TransferDstBit) == 0)
            throw new PlatformNotSupportedException(
                "The Vulkan Win32 surface does not support transfer-destination swapchain images.");
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
        var extent = capabilities.CurrentExtent.Width != uint.MaxValue
            ? capabilities.CurrentExtent
            : new Extent2D(
                Math.Clamp(checked((uint)width), capabilities.MinImageExtent.Width, capabilities.MaxImageExtent.Width),
                Math.Clamp(checked((uint)height), capabilities.MinImageExtent.Height, capabilities.MaxImageExtent.Height));
        if (extent.Width < width || extent.Height < height)
            return false;
        // The presenter owns a single acquired/copy slot. Requesting an extra
        // FIFO image only multiplies driver allocation during every Win32
        // extent change without enabling another frame in flight.
        var imageCount = capabilities.MinImageCount;
        var presentMode = SelectPresentMode();
        var compositeAlpha = SelectCompositeAlpha(capabilities.SupportedCompositeAlpha);
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
        var createStarted = Stopwatch.GetTimestamp();
        var createResult = _swapchainApi!.CreateSwapchain(
            _device, &createInfo, null, out var newSwapchain);
        _lastSwapchainCreateLatencyMicroseconds = checked((long)
            Stopwatch.GetElapsedTime(createStarted).TotalMicroseconds);
        _maximumSwapchainCreateLatencyMicroseconds = Math.Max(
            _maximumSwapchainCreateLatencyMicroseconds, _lastSwapchainCreateLatencyMicroseconds);
        if (createResult != Result.Success)
        {
            // oldSwapchain may be retired by a failed replacement attempt.
            // Never retry or present it as though it were still current.
            if (oldSwapchain.Handle != 0)
            {
                ReleaseSwapchainSynchronization();
                _swapchainApi.DestroySwapchain(_device, oldSwapchain, null);
                _swapchain = default;
                _swapchainImages = [];
                Width = Height = 0;
            }
            Check(createResult, "vkCreateSwapchainKHR");
        }

        _format = selected.Format;
        _colorSpace = selected.ColorSpace.ToString();
        _presentMode = presentMode.ToString();
        _compositeAlpha = compositeAlpha.ToString();
        _swapchain = newSwapchain;
        if (oldSwapchain.Handle != 0) _swapchainApi.DestroySwapchain(_device, oldSwapchain, null);
        ReleaseSwapchainSynchronization();
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
        _surfaceWidth = checked((int)extent.Width);
        _surfaceHeight = checked((int)extent.Height);
        var backingStarted = Stopwatch.GetTimestamp();
        CreateBacking(_surfaceWidth, _surfaceHeight);
        _lastBackingWrapLatencyMicroseconds = checked((long)
            Stopwatch.GetElapsedTime(backingStarted).TotalMicroseconds);
        _maximumBackingWrapLatencyMicroseconds = Math.Max(
            _maximumBackingWrapLatencyMicroseconds, _lastBackingWrapLatencyMicroseconds);
        _swapchainGeneration++;
        _surfaceRecreateCount++;
        _lastRecreateReason = reason;
        Width = width;
        Height = height;
        _lastRecreateLatencyMicroseconds = checked((long)
            Stopwatch.GetElapsedTime(recreateStarted).TotalMicroseconds);
        _maximumRecreateLatencyMicroseconds = Math.Max(
            _maximumRecreateLatencyMicroseconds, _lastRecreateLatencyMicroseconds);
        RecordEvent(
            $"swapchain generation={_swapchainGeneration} reason={reason} " +
            $"viewport={width}x{height} surface={_surfaceWidth}x{_surfaceHeight} " +
            $"backingCapacity={_backingCapacityWidth}x{_backingCapacityHeight} " +
            $"recreateUs={_lastRecreateLatencyMicroseconds} createUs={_lastSwapchainCreateLatencyMicroseconds} " +
            $"backingUs={_lastBackingWrapLatencyMicroseconds} retirementUs={_lastRetirementLatencyMicroseconds}");
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
        var reuseStorage = _backingImage.Handle != 0 && _backingFormat == _format &&
            _backingCapacityWidth >= width && _backingCapacityHeight >= height;
        ReleaseBackingSurface();
        if (!reuseStorage)
        {
            var capacityWidth = GrowBackingDimension(_backingCapacityWidth, width);
            var capacityHeight = GrowBackingDimension(_backingCapacityHeight, height);
            ReleaseBackingStorage();
            _backingCapacityWidth = capacityWidth;
            _backingCapacityHeight = capacityHeight;
            _backingFormat = _format;
            if (!TryAllocateBackingStorage())
            {
                if (_backingCapacityWidth == width && _backingCapacityHeight == height)
                    throw new InvalidOperationException(
                        $"Vulkan exact backing allocation for {width}x{height} could not be allocated " +
                        $"within the {MaximumRetainedStorageAllocationBytes}-byte retained-storage bound.");
                RecordEvent(
                    $"backing capacity fallback requested={width}x{height} " +
                    $"candidate={capacityWidth}x{capacityHeight} " +
                    $"boundBytes={MaximumRetainedStorageAllocationBytes}");
                _backingCapacityWidth = width;
                _backingCapacityHeight = height;
                if (!TryAllocateBackingStorage())
                    throw new InvalidOperationException(
                        $"Vulkan exact backing allocation for {width}x{height} could not be allocated " +
                        $"within the {MaximumRetainedStorageAllocationBytes}-byte retained-storage bound.");
            }
            _backingAllocationCount++;
        }
        else
        {
            _backingReuseCount++;
        }

        WrapBackingSurface(_backingCapacityWidth, _backingCapacityHeight);
    }

    private bool TryAllocateBackingStorage()
    {
        var imageInfo = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D,
            Format = _format,
            Extent = new Extent3D(
                checked((uint)_backingCapacityWidth), checked((uint)_backingCapacityHeight), 1),
            MipLevels = 1,
            ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.ColorAttachmentBit | ImageUsageFlags.TransferSrcBit |
                    ImageUsageFlags.TransferDstBit | ImageUsageFlags.SampledBit,
            SharingMode = SharingMode.Exclusive,
            InitialLayout = ImageLayout.Undefined,
        };
        var createResult = _vk.CreateImage(_device, &imageInfo, null, out _backingImage);
        if (createResult is Result.ErrorOutOfDeviceMemory or Result.ErrorOutOfHostMemory)
        {
            _backingImage = default;
            _backingAllocationSize = 0;
            return false;
        }
        Check(createResult, "vkCreateImage(backing)");
        _vk.GetImageMemoryRequirements(_device, _backingImage, out var requirements);
        if (requirements.Size > MaximumRetainedStorageAllocationBytes)
        {
            // No command references this candidate yet, so it can be rejected
            // safely before allocation/submission. The caller retries with the
            // exact requested extent instead of combining independent width and
            // height high-water marks into an unnecessarily huge image.
            _vk.DestroyImage(_device, _backingImage, null);
            _backingImage = default;
            _backingAllocationSize = 0;
            return false;
        }
        _backingAllocationSize = requirements.Size;
        var allocationInfo = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = requirements.Size,
            MemoryTypeIndex = FindMemoryType(requirements.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit),
        };
        var allocationResult = _vk.AllocateMemory(_device, &allocationInfo, null, out _backingMemory);
        if (allocationResult is Result.ErrorOutOfDeviceMemory or Result.ErrorOutOfHostMemory)
        {
            _vk.DestroyImage(_device, _backingImage, null);
            _backingImage = default;
            _backingMemory = default;
            _backingAllocationSize = 0;
            return false;
        }
        Check(allocationResult, "vkAllocateMemory(backing)");
        var bindResult = _vk.BindImageMemory(_device, _backingImage, _backingMemory, 0);
        if (bindResult is Result.ErrorOutOfDeviceMemory or Result.ErrorOutOfHostMemory)
        {
            _vk.FreeMemory(_device, _backingMemory, null);
            _backingMemory = default;
            _vk.DestroyImage(_device, _backingImage, null);
            _backingImage = default;
            _backingAllocationSize = 0;
            return false;
        }
        Check(bindResult, "vkBindImageMemory(backing)");
        try
        {
            if (!TryAllocateRetainedFrameStorage())
            {
                ReleaseBackingAllocationHandles();
                return false;
            }
        }
        catch
        {
            ReleaseBackingAllocationHandles();
            throw;
        }
        TransitionBackingToColorAttachment();
        return true;
    }

    private bool TryAllocateRetainedFrameStorage()
    {
        var imageInfo = new ImageCreateInfo
        {
            SType = StructureType.ImageCreateInfo,
            ImageType = ImageType.Type2D,
            Format = _format,
            Extent = new Extent3D(
                checked((uint)_backingCapacityWidth), checked((uint)_backingCapacityHeight), 1),
            MipLevels = 1,
            ArrayLayers = 1,
            Samples = SampleCountFlags.Count1Bit,
            Tiling = ImageTiling.Optimal,
            Usage = ImageUsageFlags.TransferSrcBit | ImageUsageFlags.TransferDstBit,
            SharingMode = SharingMode.Exclusive,
            InitialLayout = ImageLayout.Undefined,
        };
        var createResult = _vk.CreateImage(
            _device, &imageInfo, null, out _retainedFrameImage);
        if (createResult is Result.ErrorOutOfDeviceMemory or Result.ErrorOutOfHostMemory)
        {
            _retainedFrameImage = default;
            return false;
        }
        Check(createResult, "vkCreateImage(retained frame)");

        _vk.GetImageMemoryRequirements(
            _device, _retainedFrameImage, out var requirements);
        if (_backingAllocationSize > MaximumRetainedStorageAllocationBytes ||
            requirements.Size > MaximumRetainedStorageAllocationBytes - _backingAllocationSize)
        {
            _vk.DestroyImage(_device, _retainedFrameImage, null);
            _retainedFrameImage = default;
            return false;
        }

        _retainedFrameAllocationSize = requirements.Size;
        var allocationInfo = new MemoryAllocateInfo
        {
            SType = StructureType.MemoryAllocateInfo,
            AllocationSize = requirements.Size,
            MemoryTypeIndex = FindMemoryType(
                requirements.MemoryTypeBits, MemoryPropertyFlags.DeviceLocalBit),
        };
        var allocationResult = _vk.AllocateMemory(
            _device, &allocationInfo, null, out _retainedFrameMemory);
        if (allocationResult is Result.ErrorOutOfDeviceMemory or Result.ErrorOutOfHostMemory)
        {
            _vk.DestroyImage(_device, _retainedFrameImage, null);
            _retainedFrameImage = default;
            _retainedFrameMemory = default;
            _retainedFrameAllocationSize = 0;
            return false;
        }
        Check(allocationResult, "vkAllocateMemory(retained frame)");

        var bindResult = _vk.BindImageMemory(
            _device, _retainedFrameImage, _retainedFrameMemory, 0);
        if (bindResult is Result.ErrorOutOfDeviceMemory or Result.ErrorOutOfHostMemory)
        {
            _vk.FreeMemory(_device, _retainedFrameMemory, null);
            _retainedFrameMemory = default;
            _vk.DestroyImage(_device, _retainedFrameImage, null);
            _retainedFrameImage = default;
            _retainedFrameAllocationSize = 0;
            return false;
        }
        Check(bindResult, "vkBindImageMemory(retained frame)");
        _retainedFrameLayout = ImageLayout.Undefined;
        _retainedFrameInitialized = false;
        RecordEvent(
            $"retained frame allocated capacity={_backingCapacityWidth}x{_backingCapacityHeight} " +
            $"bytes={_retainedFrameAllocationSize} totalBytes=" +
            $"{_backingAllocationSize + _retainedFrameAllocationSize}");
        return true;
    }

    private void WrapBackingSurface(int width, int height)
    {
        var skiaImageInfo = new GRVkImageInfo
        {
            Image = _backingImage.Handle,
            Alloc = new GRVkAlloc
            {
                Memory = _backingMemory.Handle,
                Offset = 0,
                Size = _backingAllocationSize,
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

    private int GrowBackingDimension(int current, int required)
    {
        if (_maximumImageDimension2D != 0 && required > _maximumImageDimension2D)
            throw new InvalidOperationException(
                $"Vulkan backing dimension {required} exceeds the device limit {_maximumImageDimension2D}.");
        if (current >= required) return current;
        var desired = current == 0
            ? (long)required
            : Math.Max((long)required, current + Math.Max(256L, current / 4L));
        var aligned = checked((desired + 255L) & ~255L);
        if (_maximumImageDimension2D != 0 && aligned > _maximumImageDimension2D)
            aligned = required;
        return checked((int)aligned);
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
        SubmitCommands("Vulkan backing initialization", waitForCompletion: true);
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
            _commandBuffer,
            PipelineStageFlags.ColorAttachmentOutputBit | PipelineStageFlags.TransferBit,
            PipelineStageFlags.TransferBit,
            0, 0, null, 0, null, 2, barriers);

        var copy = new ImageCopy
        {
            SrcSubresource = ColorSubresourceLayers(),
            DstSubresource = ColorSubresourceLayers(),
            Extent = new Extent3D(checked((uint)_surfaceWidth), checked((uint)_surfaceHeight), 1),
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
        SubmitCommands(
            "Vulkan backing copy", _acquireSemaphore, renderFinished,
            waitForCompletion: false);
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

    private void SubmitCommands(
        string identity,
        VkSemaphore waitSemaphore = default,
        VkSemaphore signalSemaphore = default,
        bool waitForCompletion = true)
    {
        if (_copySubmissionPending)
            throw new InvalidOperationException(
                "The Vulkan copy command buffer cannot be reused before its fence completes.");
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
        if (waitForCompletion)
        {
            WaitFence(identity);
        }
        else
        {
            _copySubmissionPending = true;
            _deferredCopySubmissionCount++;
        }
    }

    private void WaitForPendingCopySubmission()
    {
        if (!_copySubmissionPending) return;
        var started = Stopwatch.GetTimestamp();
        WaitFence("previous Vulkan backing copy");
        _lastCopyFenceWaitMicroseconds = checked((long)
            Stopwatch.GetElapsedTime(started).TotalMicroseconds);
        _maximumCopyFenceWaitMicroseconds = Math.Max(
            _maximumCopyFenceWaitMicroseconds, _lastCopyFenceWaitMicroseconds);
        _copyFenceWaitCount++;
        _copySubmissionPending = false;
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
        _copySubmissionPending = false;
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
        if (_device.Handle == 0) return;
        var result = TakeInjectedResult("DEVICE_LOST_ON_WAIT_IDLE")
            ? Result.ErrorDeviceLost
            : _vk.DeviceWaitIdle(_device);
        Check(result, "vkDeviceWaitIdle");
    }

    private void ReleaseBackingSurface()
    {
        _backingSurface?.Dispose();
        _backingSurface = null;
        _backingTarget?.Dispose();
        _backingTarget = null;
    }

    private void ReleaseBackingStorage()
    {
        ReleaseBackingAllocationHandles();
        _backingFormat = default;
        _backingCapacityWidth = 0;
        _backingCapacityHeight = 0;
    }

    private void ReleaseBackingAllocationHandles()
    {
        if (_retainedFrameImage.Handle != 0)
            _vk.DestroyImage(_device, _retainedFrameImage, null);
        _retainedFrameImage = default;
        if (_retainedFrameMemory.Handle != 0)
            _vk.FreeMemory(_device, _retainedFrameMemory, null);
        _retainedFrameMemory = default;
        _retainedFrameAllocationSize = 0;
        _retainedFrameLayout = ImageLayout.Undefined;
        _retainedFrameInitialized = false;

        if (_backingImage.Handle != 0) _vk.DestroyImage(_device, _backingImage, null);
        _backingImage = default;
        if (_backingMemory.Handle != 0) _vk.FreeMemory(_device, _backingMemory, null);
        _backingMemory = default;
        _backingAllocationSize = 0;
    }

    private void ReleasePresentationSlotVulkan(PresentationSlot slot)
    {
        if (slot.Image.Handle != 0) _vk.DestroyImage(_device, slot.Image, null);
        slot.Image = default;
        if (slot.Memory.Handle != 0) _vk.FreeMemory(_device, slot.Memory, null);
        slot.Memory = default;
        slot.Layout = ImageLayout.Undefined;
    }

    private void WaitForPresentationRetirement()
    {
        if (_presentationContext == 0) return;
        var deadline = Environment.TickCount64 + 5_000;
        nint* handles = stackalloc nint[BufferCount];
        while (true)
        {
            var allAvailable = true;
            uint count = 0;
            for (var index = 0; index < BufferCount; index++)
            {
                var slot = _presentationSlots[index];
                if (!slot.Registered) continue;
                var result = IsCompositionBufferAvailable(
                    _presentationContext, checked((uint)index), out var available);
                if (result < 0) Marshal.ThrowExceptionForHR(result);
                if (available != 0) continue;
                allAvailable = false;
                if (slot.AvailableEvent != 0)
                    handles[count++] = unchecked((nint)slot.AvailableEvent);
            }
            if (allAvailable) return;
            if (Environment.TickCount64 >= deadline)
                throw new TimeoutException(
                    "Presentation buffers did not retire within 5 seconds.");
            if (count == 0)
            {
                Thread.Yield();
                continue;
            }
            var wait = WaitForMultipleObjects(
                count, handles, false, BufferAvailabilityWaitMilliseconds);
            if (wait == WaitFailed)
                throw new Win32Exception(
                    Marshal.GetLastWin32Error(),
                    "Waiting for Vulkan Composition buffer retirement failed.");
        }
    }

    private void UnbindPresentationSurfaceForRetirement()
    {
        CancelPreparedMovingFrame();
        nint context;
        ulong tag;
        lock (_viewportGate)
        {
            _presentationRetiring = true;
            context = _presentationContext;
            if (context == 0 || _presentationDrainCommitted ||
                !_presentationSlots.Any(static slot => slot.Registered))
                return;
            _presentationDrainCommitted = true;
            tag = ++_presentTag;
        }
        int result;
        ulong presentId;
        try
        {
            result = UnbindCompositionBuffer(context, tag, out presentId);
            if (result < 0) Marshal.ThrowExceptionForHR(result);
        }
        catch
        {
            lock (_viewportGate) _presentationDrainCommitted = false;
            throw;
        }
        RecordEvent($"composition retirement-buffer present id={presentId}");
    }

    private void ReleaseDevice(bool deviceLost, bool waitForIdle = true)
    {
        CancelPreparedMovingFrame();
        if (_instance.Handle == 0) return;
        if (!deviceLost && waitForIdle)
        {
            try
            {
                WaitIdle();
            }
            catch (WindowsManagedVulkanDeviceLostException)
            {
                // vkDeviceWaitIdle can be the first call to report loss. From
                // this point Skia must be abandoned without releasing backend
                // resources through the dead device, but native handles still
                // need deterministic teardown.
                deviceLost = true;
                RecordEvent("device loss observed during release; abandoning Skia context");
            }
        }
        UnbindPresentationSurfaceForRetirement();
        WaitForPresentationRetirement();
        if (deviceLost)
        {
            // A lost backend must be abandoned before any Skia-owned wrapper
            // is disposed so its destructors make no Vulkan calls.
            AbandonContextForDeviceLossCore();
        }
        if (deviceLost || _contextAbandoned)
        {
            ReleaseBackingSurface();
        }
        else
        {
            _copySubmissionPending = false;
            ReleaseBackingSurface();
            // The device is still valid, so let Skia release its backend
            // resources before Vulkan device destruction.
            _context?.AbandonContext(true);
        }
        _acquired = false;
        _copySubmissionPending = false;
        _context?.Dispose();
        _context = null;
        _contextAbandoned = false;
        _skiaBackend?.Dispose();
        _skiaBackend = null;
        _skiaExtensions?.Dispose();
        _skiaExtensions = null;
        ReleaseBackingStorage();
        foreach (var slot in _presentationSlots) ReleasePresentationSlotVulkan(slot);
        ClearCompositionSurfaceBinding();
        if (_presentationContext != 0) DestroyComposition(_presentationContext);
        lock (_viewportGate)
        {
            _presentationContext = 0;
            _compositionSurfaceHandle = 0;
            _presentationDrainCommitted = false;
        }
        foreach (var slot in _presentationSlots) slot.ResetNativeState();
        _presentationPoisoned = false;
        _selectedSlot = -1;
        ReleaseSwapchainSynchronization();
        if (_swapchain.Handle != 0) _swapchainApi?.DestroySwapchain(_device, _swapchain, null);
        _swapchain = default;
        _swapchainImages = [];
        if (_fence.Handle != 0) _vk.DestroyFence(_device, _fence, null);
        _fence = default;
        if (_acquireSemaphore.Handle != 0) _vk.DestroySemaphore(_device, _acquireSemaphore, null);
        _acquireSemaphore = default;
        if (_commandPool.Handle != 0) _vk.DestroyCommandPool(_device, _commandPool, null);
        _commandPool = default;
        _commandBuffer = default;
        _externalMemoryApi?.Dispose();
        _externalMemoryApi = null;
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
        _surfaceWidth = _surfaceHeight = 0;
        AdapterDescription = "uninitialized";
    }

    private void AbandonContextForDeviceLossCore()
    {
        if (_context is null || _contextAbandoned) return;
        _context.AbandonContext(false);
        _contextAbandoned = true;
        TryRecordEvent("Vulkan context abandoned before renderer invalidation");
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

    private void TryRecordEvent(string value)
    {
        try
        {
            RecordEvent(value);
        }
        catch
        {
            // Diagnostics must never prevent abandon/teardown safety work.
        }
    }

    private void RecordRendererReleaseTeardown()
    {
        TryRecordEvent(_rendererReleasePreflightReportedDeviceLoss
            ? "device-loss renderer resources invalidated; starting native teardown"
            : "unsafe-backend renderer resources invalidated; starting native teardown");
        _rendererReleasePreflightReportedDeviceLoss = false;
    }

    private string[] SnapshotEvents()
    {
        lock (_eventGate) return _recentEvents.ToArray();
    }

    private void SetHostBackdropBrush(
        nint window, bool enabled, bool throwOnFailure)
    {
        var requested = enabled ? 1 : 0;
        try
        {
            var set = DwmSetWindowAttribute(
                window, DwmwaUseHostBackdropBrush, ref requested, sizeof(int));
            if (set < 0) Marshal.ThrowExceptionForHR(set);
            // DWMWA_USE_HOSTBACKDROPBRUSH is accepted by DwmSetWindowAttribute
            // on supported systems but DwmGetWindowAttribute returns
            // E_INVALIDARG on current Windows builds. Treat the checked set
            // call as the contract and verify the controller/island separately.
            _hostBackdropBrushEnabled = enabled;
            RecordEvent($"host backdrop brush enabled={(_hostBackdropBrushEnabled ? 1 : 0)}");
        }
        catch when (!throwOnFailure)
        {
            _hostBackdropBrushEnabled = false;
        }
    }

    private void SetDwmSystemBackdrop(
        nint window, bool enabled, bool throwOnFailure)
    {
        var requested = enabled
            ? DwmSystemBackdropTransientWindow
            : DwmSystemBackdropAuto;
        try
        {
            var set = DwmSetWindowAttribute(
                window, DwmwaSystemBackdropType, ref requested, sizeof(int));
            if (set < 0) Marshal.ThrowExceptionForHR(set);
            _dwmSystemBackdropEnabled = enabled;
            RecordEvent($"DWM transient backdrop underlay enabled={(_dwmSystemBackdropEnabled ? 1 : 0)}");
        }
        catch when (!throwOnFailure)
        {
            _dwmSystemBackdropEnabled = false;
        }
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
        ReleaseCompositionTopology();
        ReleaseDevice(deviceLost: false);
        _vk.Dispose();
        _disposed = true;
    }

    internal override void DisposeAfterRendererGpuResourceRelease(bool deviceLost)
    {
        if (_disposed) return;
        if (deviceLost)
            RecordRendererReleaseTeardown();
        ReleaseCompositionTopology();
        ReleaseDevice(deviceLost, waitForIdle: false);
        _vk.Dispose();
        _disposed = true;
    }

    internal override void DisposeAfterRendererGpuResourceReleaseFailure()
    {
        if (_disposed) return;

        ReleaseCompositionTopology();

        // vkDeviceWaitIdle failed without VK_ERROR_DEVICE_LOST, so neither
        // execution completion nor presentation-resource retirement is known.
        // Dispose only Skia's already-abandoned managed wrappers. Vulkan/Silk
        // objects and the loader stay quarantined for process reclamation;
        // destroying them here could violate in-use object lifetime rules.
        var failures = new List<Exception>();
        void Cleanup(Action action)
        {
            try
            {
                action();
            }
            catch (Exception failure)
            {
                failures.Add(failure);
            }
        }

        if (_backingSurface is { } surface) Cleanup(surface.Dispose);
        _backingSurface = null;
        if (_backingTarget is { } target) Cleanup(target.Dispose);
        _backingTarget = null;
        if (_context is { } context) Cleanup(context.Dispose);
        _context = null;
        if (_skiaBackend is { } backend) Cleanup(backend.Dispose);
        _skiaBackend = null;
        if (_skiaExtensions is { } extensions) Cleanup(extensions.Dispose);
        _skiaExtensions = null;
        _disposed = true;
        TryRecordEvent("unsafe Vulkan native objects quarantined after failed idle preflight");

        if (failures.Count == 1)
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(failures[0]).Throw();
        if (failures.Count > 1)
            throw new AggregateException("Unsafe Vulkan managed-wrapper cleanup failed.", failures);
    }

    private sealed class PresentationSlot
    {
        internal bool Registered;
        internal bool Poisoned;
        internal VkImage Image;
        internal DeviceMemory Memory;
        internal ImageLayout Layout;
        internal ulong AvailableEvent;
        internal int CapacityWidth;
        internal int CapacityHeight;

        internal void ResetNativeState()
        {
            Registered = false;
            Poisoned = false;
            AvailableEvent = 0;
            CapacityWidth = CapacityHeight = 0;
            Layout = ImageLayout.Undefined;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct VulkanCompositionProbe
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal int DxgiFactoryHresult;
        internal int AdapterEnumerationHresult;
        internal int D3D11DeviceHresult;
        internal int PresentationFactoryHresult;
        internal int PresentationManagerHresult;
        internal int SurfaceHandleHresult;
        internal int PresentationSurfaceHresult;
        internal int RetiringFenceHresult;
        internal int RequestedAdapterLuidLow;
        internal int RequestedAdapterLuidHigh;
        internal int ActualAdapterLuidLow;
        internal int ActualAdapterLuidHigh;
        internal uint AdapterVendorId;
        internal uint AdapterDeviceId;
        internal uint AdapterFlags;
        internal uint DeviceCreationFlags;
        internal uint DeviceFeatureLevel;
        internal uint AdapterLuidMatched;
        internal uint PresentationSupported;
        internal uint IndependentFlipSupported;
        internal ulong RetiringFenceCompletedValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct VulkanCompositionBuffer
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal int TextureHresult;
        internal int DxgiResourceHresult;
        internal int SharedHandleHresult;
        internal int AddBufferHresult;
        internal int AvailableEventHresult;
        internal uint Width;
        internal uint Height;
        internal uint Format;
        internal uint BindFlags;
        internal uint MiscFlags;
        internal uint InitiallyAvailable;
    }

    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_create_v1")]
    private static partial int CreateComposition(
        uint adapterLuidLow, int adapterLuidHigh,
        out nint context, out ulong compositionSurfaceHandle,
        ref VulkanCompositionProbe snapshot);

    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_set_premultiplied_alpha_v1")]
    private static partial int SetCompositionPremultipliedAlpha(
        nint context, uint enabled);

    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_attach_window_v1")]
    private static partial int AttachCompositionWindow(
        nint context, ulong targetWindow);

    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_destroy_v1")]
    private static partial void DestroyComposition(nint context);

    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_replace_buffer_v1")]
    private static partial int ReplaceCompositionBuffer(
        nint context, uint slotIndex, uint width, uint height,
        out ulong sharedTextureHandle, out ulong availableEvent,
        ref VulkanCompositionBuffer snapshot);

    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_is_available_v1")]
    private static partial int IsCompositionBufferAvailable(
        nint context, uint slotIndex, out uint available);

    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_present_cropped_v1")]
    private static partial int PresentCropped(
        nint context, uint slotIndex,
        uint sourceX, uint sourceY, uint width, uint height, ulong tag,
        uint waitForCompositionFrame, uint waitTimeoutMilliseconds,
        out uint compositionFrameObserved,
        out ulong presentId, out ulong retiringFenceValue);

    [LibraryImport(WindowsNativeV1.LibraryName,
        EntryPoint = "doroti_windows_vulkan_composition_retire_buffers_v1")]
    private static partial int UnbindCompositionBuffer(
        nint context, ulong tag, out ulong presentId);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static partial bool GetClientRect(nint window, out NativeRect rect);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial uint WaitForMultipleObjects(
        uint count, nint* handles,
        [MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)] bool waitAll, uint milliseconds);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static partial bool CloseHandle(nint handle);

    [LibraryImport("kernel32.dll", EntryPoint = "GetModuleHandleW", StringMarshalling = StringMarshalling.Utf16)]
    private static partial nint GetModuleHandle(string? moduleName);

    [LibraryImport("dcomp.dll")]
    private static partial uint DCompositionWaitForCompositorClock(
        uint count, nint handles, uint timeoutInMs);

    [DllImport("dwmapi.dll", ExactSpelling = true)]
    private static extern int DwmFlush();

    [LibraryImport("dwmapi.dll")]
    private static partial int DwmSetWindowAttribute(
        nint window, int attribute, ref int value, int valueSize);

}

internal sealed class SystemCompositionWorker : IDisposable
{
    private readonly Windows.System.DispatcherQueueController _dispatcher;
    private bool _disposed;

    internal SystemCompositionWorker()
    {
        _dispatcher = Windows.System.DispatcherQueueController.CreateOnDedicatedThread();
        Invoke(() =>
        {
            ThreadId = Environment.CurrentManagedThreadId;
            Compositor = new SystemComposition.Compositor();
        });
    }

    internal int ThreadId { get; private set; }
    internal SystemComposition.Compositor Compositor { get; private set; } = null!;

    internal T Invoke<T>(Func<T> callback)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (ThreadId != 0 && Environment.CurrentManagedThreadId == ThreadId) return callback();
        using var completed = new ManualResetEventSlim();
        Exception? failure = null;
        T? result = default;
        if (!_dispatcher.DispatcherQueue.TryEnqueue(() =>
            {
                try { result = callback(); }
                catch (Exception exception) { failure = exception; }
                finally { completed.Set(); }
            }))
            throw new InvalidOperationException("System Composition DispatcherQueue rejected work.");
        if (!completed.Wait(TimeSpan.FromSeconds(15)))
            throw new TimeoutException("System Composition DispatcherQueue work timed out.");
        if (failure is not null)
            throw new InvalidOperationException(
                $"System Composition DispatcherQueue work failed: {failure.GetType().Name}: {failure.Message}",
                failure);
        return result!;
    }

    internal void Invoke(Action callback) => Invoke(() =>
    {
        callback();
        return true;
    });

    public void Dispose()
    {
        if (_disposed) return;
        Invoke(Compositor.Dispose);
        _disposed = true;
        _dispatcher.ShutdownQueueAsync().AsTask().GetAwaiter().GetResult();
    }
}

internal sealed class VulkanAcrylicScene : IDisposable
{
    private readonly SystemComposition.ContainerVisual _root;
    private readonly SystemCompositionDesktop.DesktopWindowTarget _target;
    private readonly DesktopAcrylicController _backdrop;
    private readonly SystemBackdropConfiguration _configuration;
    private bool _disposed;

    internal VulkanAcrylicScene(
        SystemComposition.Compositor compositor,
        nint topLevelWindow,
        WindowBackdropOptions acrylicOptions,
        Brightness systemBrightness)
    {
        if (topLevelWindow == 0)
            throw new ArgumentOutOfRangeException(nameof(topLevelWindow));
        _root = compositor.CreateContainerVisual();
        _target = SystemDesktopCompositionInterop.CreateDesktopWindowTarget(
            compositor, topLevelWindow);
        _target.Root = _root;
        _configuration = new SystemBackdropConfiguration { IsInputActive = true };
        _backdrop = new DesktopAcrylicController();
        BackdropTargetAdded = _backdrop.SetTarget(
            Microsoft.UI.Win32Interop.GetWindowIdFromWindow(topLevelWindow), _target);
        DesktopWindowTargetConnected = _target.Root is not null;
        ApplyAcrylic(acrylicOptions, systemBrightness);
    }

    internal bool BackdropTargetAdded { get; }
    internal bool DesktopWindowTargetConnected { get; }
    internal string State => _backdrop.State.ToString();

    internal void ApplyAcrylic(
        WindowBackdropOptions options,
        Brightness systemBrightness)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _backdrop.ResetProperties();
        _backdrop.Kind = options.acrylicKind switch
        {
            Doroti.Ui.WindowAcrylicKind.@base => DesktopAcrylicKind.Base,
            Doroti.Ui.WindowAcrylicKind.thin => DesktopAcrylicKind.Thin,
            _ => DesktopAcrylicKind.Default,
        };
        if (options.tintColor is { } tint)
        {
            var value = tint.value;
            _backdrop.TintColor = Windows.UI.Color.FromArgb(
                (byte)(value >> 24), (byte)(value >> 16),
                (byte)(value >> 8), (byte)value);
        }
        if (options.tintOpacity is { } tintOpacity)
            _backdrop.TintOpacity = (float)tintOpacity;
        if (options.luminosityOpacity is { } luminosityOpacity)
            _backdrop.LuminosityOpacity = (float)luminosityOpacity;
        _configuration.Theme = options.theme switch
        {
            Doroti.Ui.WindowBackdropTheme.light => SystemBackdropTheme.Light,
            Doroti.Ui.WindowBackdropTheme.dark => SystemBackdropTheme.Dark,
            _ => systemBrightness == Brightness.dark
                ? SystemBackdropTheme.Dark : SystemBackdropTheme.Light,
        };
        _backdrop.SetSystemBackdropConfiguration(_configuration);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _backdrop.Dispose();
        _target.Root = null;
        _root.Dispose();
        _target.Dispose();
    }
}

internal static class SystemDesktopCompositionInterop
{
    private static readonly Guid CompositorDesktopInteropIid =
        new("29E691FA-4567-4DCA-B319-D0F207EB6807");

    internal static unsafe SystemCompositionDesktop.DesktopWindowTarget CreateDesktopWindowTarget(
        SystemComposition.Compositor compositor,
        nint window)
    {
        ArgumentNullException.ThrowIfNull(compositor);
        if (window == 0) throw new ArgumentOutOfRangeException(nameof(window));
        using var interop = ((WinRT.IWinRTObject)compositor).NativeObject.As(
            CompositorDesktopInteropIid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var create = (delegate* unmanaged[Stdcall]<nint, nint, int, nint*, int>)vtable[3];
        nint result = 0;
        var hresult = create(thisPointer, window, 0, &result);
        if (hresult < 0) Marshal.ThrowExceptionForHR(hresult);
        try
        {
            return WinRT.MarshalInterface<SystemCompositionDesktop.DesktopWindowTarget>.FromAbi(result);
        }
        finally
        {
            Marshal.Release(result);
        }
    }
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
    int SurfaceWidth,
    int SurfaceHeight,
    ulong RetainedSurfaceReuses,
    ulong SurfaceRecreates,
    int BackingCapacityWidth,
    int BackingCapacityHeight,
    ulong BackingAllocationBytes,
    ulong RetainedFrameAllocationBytes,
    bool RetainedFrameInitialized,
    ulong BackingAllocations,
    ulong BackingReuses,
    ulong DeferredCopySubmissions,
    ulong CopyFenceWaits,
    long LastCopyFenceWaitMicroseconds,
    long MaximumCopyFenceWaitMicroseconds,
    int Width,
    int Height,
    ulong SwapchainGeneration,
    ulong Acquired,
    ulong Presented,
    ulong SuccessfulPresents,
    int OutstandingAcquired,
    int OutstandingImageIndex,
    int OutstandingCopySubmission,
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
    long LastRecreateLatencyMicroseconds,
    long MaximumRecreateLatencyMicroseconds,
    long LastSwapchainCreateLatencyMicroseconds,
    long MaximumSwapchainCreateLatencyMicroseconds,
    long LastBackingWrapLatencyMicroseconds,
    long MaximumBackingWrapLatencyMicroseconds,
    long FirstPresentQpc,
    long LastTargetQpc,
    long LastPresentQpc,
    string RetirementMode,
    ulong QueueIdleRetirementWaits,
    ulong CompositionFrameWaits,
    ulong CompositionFrameObserved,
    ulong CompositionFrameWaitTimeouts,
    ulong FixedOriginPreGeometryAdmissions,
    ulong MovingOriginPreGeometryAdmissions,
    ulong MovingOriginPreGeometryDisplayWaits,
    ulong MovingOriginPrepared,
    ulong MovingOriginWindowPosCommitAttempt,
    ulong MovingOriginWindowPosCommitted,
    ulong MovingOriginWindowPosMismatch,
    ulong MovingOriginWindowPosCancelled,
    ulong MovingOriginWindowPosFailed,
    int MovingOriginReserved,
    ulong ClockWait,
    ulong ClockWaitObserved,
    ulong ClockWaitTimeout,
    ulong PostGeometryFallback,
    string CandidatePolicy,
    long LastCompositionFrameWaitMicroseconds,
    long MaximumCompositionFrameWaitMicroseconds,
    ulong ResizeClockWaits,
    ulong ResizeClockSignals,
    ulong ResizeClockFailures,
    uint LastResizeClockStatus,
    long MaximumResizeClockWaitMicroseconds,
    string[] RecentEvents);
