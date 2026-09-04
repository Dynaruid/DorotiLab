using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Content;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

internal sealed unsafe partial class WindowsManagedAcrylicCompositionPresenter :
    WindowsManagedHwndPresenterBase,
    IWindowsAcrylicPresenter
{
    internal const string RuntimeChannel = "doroti/windows/experimental-acrylic";
    internal const int LogicalEdgeBudget = 6;
    internal const int PhysicalEdgeBudget = 12;
    private const int GuardBandPixels = PhysicalEdgeBudget;
    private const int BufferCapacityQuantum = 256;
    private const int MaximumRetainedCapacityAreaRatio = 4;
    private const uint BufferAvailabilityWaitMilliseconds = 17;
    private const uint WaitObject0 = 0;

    private const string AngleLibrary = "av_libglesv2.dll";
    private const int EglFalse = 0;
    private const int EglNone = 0x3038;
    private const int EglExtensions = 0x3055;
    private const uint EglPlatformAngle = 0x3202;
    private const int EglPlatformAngleType = 0x3203;
    private const int EglPlatformAngleTypeD3D11 = 0x3208;
    private const int EglPlatformAngleDeviceType = 0x3209;
    private const int EglPlatformAngleDeviceTypeHardware = 0x320A;
    private const int EglSurfaceType = 0x3033;
    private const int EglPbufferBit = 0x0001;
    private const int EglRenderableType = 0x3040;
    private const int EglOpenGles2Bit = 0x0004;
    private const int EglRedSize = 0x3024;
    private const int EglGreenSize = 0x3023;
    private const int EglBlueSize = 0x3022;
    private const int EglAlphaSize = 0x3021;
    private const int EglDepthSize = 0x3025;
    private const int EglStencilSize = 0x3026;
    private const int EglContextClientVersion = 0x3098;
    private const int EglOpenGlesApi = 0x30A0;
    private const int EglDeviceExt = 0x322C;
    private const int EglD3D11DeviceAngle = 0x33A1;
    private const int EglD3DTextureAngle = 0x33A3;
    private const int EglWidth = 0x3057;
    private const int EglHeight = 0x3056;
    private const int EglTextureOffsetXAngle = 0x3490;
    private const int EglTextureOffsetYAngle = 0x3491;
    private const uint GlNoError = 0;
    private const uint GlRenderer = 0x1F01;
    private const uint GlSamples = 0x80A9;
    private const uint GlStencilBits = 0x0D57;
    private const uint GlRgba8 = 0x8058;

    private readonly bool _diagnosticsEnabled;
    private readonly object _optionGate = new();
    private readonly object _viewportGate = new();
    private readonly Slot[] _slots = [new(), new(), new()];
    private WindowBackdropOptions _options;
    private Brightness _systemBrightness;
    private nint _display;
    private nint _config;
    private nint _eglContext;
    private nint _d3d11Device;
    private nint _presentationContext;
    private nint _compositionSurfaceHandle;
    private GRGlInterface? _glInterface;
    private GRContext? _context;
    private DispatcherQueueController? _islandDispatcher;
    private AcrylicCompositionWorker? _composition;
    private ContentIsland? _island;
    private DesktopChildSiteBridge? _bridge;
    private AppWindow? _appWindow;
    private AcrylicScene? _scene;
    private PendingOption? _pendingOption;
    private bool _optionApplying;
    private bool _debugBaselineSealed;
    private bool _resetPending;
    private bool _disposed;
    private int _selectedSlot = -1;
    private long _viewportRevision;
    private int _viewportWidth;
    private int _viewportHeight;
    private int _presentedWidth;
    private int _presentedHeight;
    private int _presentedCapacityWidth;
    private int _presentedCapacityHeight;
    private float _placedOffsetX;
    private float _placedOffsetY;
    private int _placedWidth;
    private int _placedHeight;
    private nint _topLevelWindow;
    private ClientScreenBounds _lastClientBounds;
    private bool _hasClientBounds;
    private bool _anchorRight;
    private bool _anchorBottom;
    private ulong _presentTag;
    private ulong _availableReuseCount;
    private ulong _unavailableSkipCount;
    private ulong _maximumRegisteredSlots;
    private long _nextOptionRevision;
    private long _appliedOptionRevisions;
    private long _supersededOptionRevisions;
    private long _failedOptionRevisions;
    private bool _backdropTargetAdded;
    private bool _contentIslandConnected;
    private ProbeSnapshot _probe;

    internal WindowsManagedAcrylicCompositionPresenter(
        bool enableDiagnostics,
        WindowBackdropOptions options,
        Brightness systemBrightness)
    {
        _diagnosticsEnabled = enableDiagnostics;
        _options = ValidateOptions(options);
        _systemBrightness = systemBrightness;
        if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 26100))
            throw new PlatformNotSupportedException(
                "experimentalAcrylic requires Windows 11 24H2 build 26100 or newer.");
        try
        {
            InitializeDevice();
        }
        catch
        {
            ReleaseCompositionIsland();
            ReleaseDevice();
            throw;
        }
    }

    internal override string BackendName => "ANGLE-D3D11/Composition-Swapchain";
    internal override string RuntimeEffectsBackend => DorotiSkiaRuntimeEffects.WindowsAngleEglBackend;
    internal override bool UsesCompositionTopology => true;
    internal override string DiagnosticCoverage =>
        "experimental Acrylic, same-device ANGLE D3D11 texture import, premultiplied Composition Swapchain, " +
        "three-slot IPresentationBuffer availability, no CPU copy, nonblocking interactive resize, raster-thread exact DwmFlush, " +
        "single-owner atomic raster commits, transparent guard crop, unstretched 1:1 retained content, ResizeContentToParentWindow, " +
        "6 logical/12 physical active-edge budget";
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

    internal override void AttachWindow(nint topLevelWindow)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (topLevelWindow == 0) throw new ArgumentOutOfRangeException(nameof(topLevelWindow));
        if (_bridge is not null) throw new InvalidOperationException("The Acrylic presenter already owns a window.");
        if (Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FORCE_FALLBACK") == "1")
            throw new InvalidOperationException("Forced pre-show Acrylic fallback for validation.");
        // AttachWindow is invoked by host-ready on the native HWND/platform
        // thread and still precedes the first visible frame, so a failure
        // here remains a deterministic pre-show opaque fallback.
        InitializeCompositionIsland();
        if (_islandDispatcher is null || _composition is null || _island is null)
            throw new InvalidOperationException("The pre-show Acrylic ContentIsland topology is unavailable.");
        if (!_islandDispatcher.DispatcherQueue.HasThreadAccess)
            throw new InvalidOperationException("The Acrylic ContentIsland dispatcher does not own the HWND thread.");

        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(topLevelWindow);
        _appWindow = AppWindow.GetFromWindowId(windowId)
            ?? throw new InvalidOperationException("The Acrylic HWND has no AppWindow.");
        Trace("app-window-query-pass");
        _bridge = _composition.Invoke(() =>
        {
            var bridge = DesktopChildSiteBridge.Create(_composition.Compositor, windowId);
            // The Doroti raster target is already sized in physical pixels.
            // Keep the ContentIsland coordinate space 1:1 with that target;
            // framework layout continues to use the separately published DPI.
            bridge.OverrideScale = 1;
            // Let the site bridge be the single HWND geometry owner. Combining
            // this policy with native SetWindowPos calls creates two resize
            // timelines and exposes the parent on top/left growth.
            bridge.ResizePolicy = ContentSizePolicy.ResizeContentToParentWindow;
            bridge.Disable();
            bridge.Connect(_island);
            bridge.Show();
            return bridge;
        });
        Trace("site-bridge-create-pass");
        Trace("site-bridge-disabled-input-pass");
        Trace("site-bridge-connect-pass");
        var island = _island;
        _scene = _composition.Invoke(() => new AcrylicScene(
            _composition.Compositor, _compositionRoot!, island, _compositionSurfaceHandle,
            _options, _systemBrightness));
        Trace("acrylic-scene-create-pass");
        _contentIslandConnected = _island.IsConnected;
        _backdropTargetAdded = _scene.BackdropTargetAdded;
        if (!_contentIslandConnected || !_backdropTargetAdded)
            throw new InvalidOperationException("The Acrylic ContentIsland/backdrop target did not connect.");
        _topLevelWindow = topLevelWindow;
    }

    private ContainerVisual? _compositionRoot;

    private void InitializeCompositionIsland()
    {
        Trace("island-dispatcher-create-start");
        _islandDispatcher = DispatcherQueueController.CreateOnCurrentThread();
        Trace("island-dispatcher-create-pass");
        _composition = new AcrylicCompositionWorker(_islandDispatcher);
        Trace("composition-worker-create-pass");
        _compositionRoot = _composition.Invoke(() =>
        {
            var root = _composition.Compositor.CreateContainerVisual();
            return root;
        });
        Trace("composition-root-create-pass");
        _island = _composition.Invoke(() => ContentIsland.Create(_compositionRoot));
        Trace("content-island-create-pass");
        Trace("acrylic-support-query-start");
        if (!DesktopAcrylicController.IsSupported())
            throw new PlatformNotSupportedException("Desktop Acrylic is not supported by this Windows session.");
        Trace("acrylic-support-query-pass");
    }

    internal void ApplySystemBrightness(Brightness brightness)
    {
        _systemBrightness = brightness;
        if (_options.theme != WindowBackdropTheme.system || _scene is null || _composition is null) return;
        _composition.Invoke(() => _scene.Apply(_options, _systemBrightness));
    }

    internal override void ResizeViewport(
        int width, int height, double scale, uint sizingEdge, bool preGeometry)
    {
        _ = sizingEdge;
        _ = preGeometry;
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (!double.IsFinite(scale) || scale <= 0)
            throw new ArgumentOutOfRangeException(nameof(scale));
        if (_scene is null)
            throw new InvalidOperationException("The Acrylic scene is not attached.");
        lock (_viewportGate)
        {
            // Publish the shell extent before changing the visual tree. The
            // raster lane may finish a frame for a slightly older extent, but
            // it can commit that frame only within the explicit edge budget.
            _viewportRevision++;
            _viewportWidth = width;
            _viewportHeight = height;
            UpdateResizeAnchors();
            _scene.PrepareViewport(width, height, _anchorRight, _anchorBottom);
        }
    }

    internal ValueTask<ReadOnlyMemory<byte>?> HandleRuntimeMessageAsync(
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (data is null || data.Value.IsEmpty)
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(SerializeSnapshot());

        WindowBackdropOptions options;
        try
        {
            var request = JsonSerializer.Deserialize<RuntimeOptionRequest>(data.Value.Span,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                ?? throw new InvalidDataException("The Acrylic option request is empty.");
            options = ValidateOptions(request.ApplyTo(_options));
        }
        catch (Exception exception)
        {
            return ValueTask.FromResult<ReadOnlyMemory<byte>?>(Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(new { status = "failed", error = exception.Message })));
        }

        var revision = Interlocked.Increment(ref _nextOptionRevision);
        var completion = new TaskCompletionSource<ReadOnlyMemory<byte>?>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        PendingOption? start = null;
        lock (_optionGate)
        {
            var pending = new PendingOption(revision, options, completion);
            if (!_optionApplying)
            {
                _optionApplying = true;
                start = pending;
            }
            else
            {
                if (_pendingOption is { } superseded)
                {
                    _supersededOptionRevisions++;
                    superseded.Completion.TrySetResult(SerializeTerminal(
                        superseded.Revision, "superseded", superseded.Options));
                }
                _pendingOption = pending;
            }
        }
        if (start is not null)
            ThreadPool.QueueUserWorkItem(static value =>
            {
                var tuple = ((WindowsManagedAcrylicCompositionPresenter Owner, PendingOption Item))value!;
                tuple.Owner.ApplyOptionLoop(tuple.Item);
            }, (this, start));
        return new ValueTask<ReadOnlyMemory<byte>?>(completion.Task.WaitAsync(cancellationToken));
    }

    internal AcrylicPresenterSnapshot Snapshot() => new(
        "experimentalAcrylic", "experimentalAcrylic", null,
        _probe.PresentationSupported != 0,
        _probe.IndependentFlipSupported != 0,
        $"{_probe.AdapterLuidHigh}:{unchecked((uint)_probe.AdapterLuidLow)}",
        _probe.AdapterVendorId, _probe.AdapterDeviceId,
        _options.acrylicKind.ToString(), ResolveTheme(_options.theme, _systemBrightness).ToString(),
        _options.tintColor?.value, _options.tintOpacity, _options.luminosityOpacity,
        LogicalEdgeBudget, PhysicalEdgeBudget,
        _maximumRegisteredSlots, _availableReuseCount, _unavailableSkipCount,
        _nextOptionRevision, _appliedOptionRevisions,
        _supersededOptionRevisions, _failedOptionRevisions,
        _backdropTargetAdded,
        _contentIslandConnected);

    bool IWindowsAcrylicPresenter.AcrylicEnabled => true;
    void IWindowsAcrylicPresenter.ApplySystemBrightness(Brightness brightness) =>
        ApplySystemBrightness(brightness);
    ValueTask<ReadOnlyMemory<byte>?> IWindowsAcrylicPresenter.HandleRuntimeMessageAsync(
        ReadOnlyMemory<byte>? data,
        CancellationToken cancellationToken) =>
        HandleRuntimeMessageAsync(data, cancellationToken);
    AcrylicPresenterSnapshot IWindowsAcrylicPresenter.Snapshot() => Snapshot();

    internal override bool EnsureTarget(nint childWindow, int width, int height)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        LastPresentSucceeded = false;
        if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
        if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));
        if (_bridge is null || _scene is null || _composition is null)
            throw new InvalidOperationException("The Acrylic Composition topology is not attached.");
        if (_resetPending)
        {
            RecreateDevice();
            _resetPending = false;
        }

        if (!IsViewportWithinBudget(width, height))
        {
            // Keep the previous buffer aligned when this queued paint request
            // has already fallen outside the bounded-stale admission window.
            // Successful callbacks commit exactly once below.
            PlaceRetainedSurface();
            return false;
        }

        _selectedSlot = SelectAvailableSlot();
        if (_selectedSlot < 0 && WaitForAnyAvailableSlot())
            _selectedSlot = SelectAvailableSlot();
        if (_selectedSlot < 0)
        {
            _unavailableSkipCount++;
            PlaceRetainedSurface();
            return false;
        }
        var slot = _slots[_selectedSlot];
        var capacityWidth = checked(RoundBufferCapacity(width) + GuardBandPixels * 2);
        var capacityHeight = checked(RoundBufferCapacity(height) + GuardBandPixels * 2);
        if (CanReuseCapacity(slot, width, height, capacityWidth, capacityHeight))
        {
            _availableReuseCount++;
        }
        else
        {
            ReplaceSlot(_selectedSlot, capacityWidth, capacityHeight);
            if (Width != 0 && (Width != width || Height != height)) ResizeBuffersCount++;
        }
        slot.Width = width;
        slot.Height = height;
        if (!IsViewportWithinBudget(width, height))
        {
            _selectedSlot = -1;
            PlaceRetainedSurface();
            return false;
        }
        Width = width;
        Height = height;
        return true;
    }

    internal override T RenderAndPresent<T>(Func<SKSurface, T> paint, Predicate<T> shouldPresent)
    {
        ArgumentNullException.ThrowIfNull(paint);
        ArgumentNullException.ThrowIfNull(shouldPresent);
        ObjectDisposedException.ThrowIf(_disposed, this);
        LastPresentSucceeded = false;
        if (_selectedSlot is < 0 or > 2) throw new InvalidOperationException("No Acrylic slot was admitted.");
        var slot = _slots[_selectedSlot];
        if (!slot.Registered || slot.Texture == 0)
            throw new InvalidOperationException("The admitted Acrylic slot has no texture.");

        slot.ImportedSurface = slot.ImportedSurface != 0
            ? slot.ImportedSurface
            : CreateImportedSurface(slot.Texture, slot.CapacityWidth, slot.CapacityHeight);
        var madeCurrent = false;
        try
        {
            if (EglMakeCurrent(
                    _display, slot.ImportedSurface, slot.ImportedSurface, _eglContext) == EglFalse)
                ThrowEgl("eglMakeCurrent(Acrylic texture)");
            madeCurrent = true;
            EnsureSkiaContext();
            var context = _context!;
            // eglMakeCurrent changes what default framebuffer 0 denotes
            // outside Skia's state tracker. Invalidate cached GL state before
            // using the per-slot wrapper for the newly current EGL surface.
            context.ResetContext(GRGlBackendState.All);
            EnsureSlotRenderTarget(slot);
            var surface = slot.SkiaSurface!;
            var result = paint(surface);
            if (!IsViewportWithinBudget(slot.Width, slot.Height) || !shouldPresent(result))
                return RetainAndReturn(result, ref madeCurrent);
            surface.Canvas.Flush();
            context.Flush(surface);
            context.Submit(false);
            GlFlush();
            GpuSubmitCount++;
            ThrowIfGlErrors("Acrylic Skia submit");
            if (!IsViewportWithinBudget(slot.Width, slot.Height) || !shouldPresent(result))
                return RetainAndReturn(result, ref madeCurrent);
            UnbindImportedSurface(ref madeCurrent);
            var committed = false;
            lock (_viewportGate)
            {
                // Serialize the final bounded-stale check with one atomic
                // SetBuffer/SourceRect/transform Present. The transform pins
                // the inactive edges while leaving at most the declared
                // 12-physical-pixel discrepancy at the dragged edges.
                if (IsWithinEdgeBudget(
                        _viewportWidth, _viewportHeight, slot.Width, slot.Height) &&
                    TryGetSourceRect(
                        _viewportWidth, _viewportHeight,
                        slot.Width, slot.Height,
                        slot.CapacityWidth, slot.CapacityHeight,
                        _anchorRight, _anchorBottom,
                        out var sourceX, out var sourceY))
                {
                    var presentResult = PresentCropped(
                        _presentationContext, checked((uint)_selectedSlot),
                        checked((uint)sourceX), checked((uint)sourceY),
                        checked((uint)_viewportWidth), checked((uint)_viewportHeight),
                        ++_presentTag,
                        out _, out _);
                    if (presentResult < 0) Marshal.ThrowExceptionForHR(presentResult);
                    _presentedWidth = slot.Width;
                    _presentedHeight = slot.Height;
                    _presentedCapacityWidth = slot.CapacityWidth;
                    _presentedCapacityHeight = slot.CapacityHeight;
                    _placedOffsetX = sourceX;
                    _placedOffsetY = sourceY;
                    _placedWidth = _viewportWidth;
                    _placedHeight = _viewportHeight;
                    committed = true;
                }
            }
            if (!committed) return RetainAndReturn(result, ref madeCurrent);
            PresentCount++;
            LastPresentSucceeded = true;
            return result;
        }
        finally
        {
            if (madeCurrent) UnbindImportedSurface(ref madeCurrent);
            _selectedSlot = -1;
        }
    }

    internal override void SealInitializationDebugBaseline()
    {
        if (_debugBaselineSealed) return;
        _debugBaselineSealed = true;
    }

    internal override void CaptureOperationalDebugMessages()
    {
        // EGL/GLES errors are consumed at every operation boundary.
    }

    internal override void ResetDevice()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _resetPending = true;
    }

    internal override void ReleaseCompositionResources() => Dispose();

    public override void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Trace("dispose-start");
        PendingOption? pending;
        lock (_optionGate)
        {
            pending = _pendingOption;
            _pendingOption = null;
        }
        pending?.Completion.TrySetCanceled();
        ReleaseCompositionIsland();
        ReleaseDevice();
        Trace("dispose-device-pass");
    }

    private void ReleaseCompositionIsland()
    {
        if (_scene is not null && _composition is not null)
            _composition.Invoke(_scene.Dispose);
        Trace("dispose-scene-pass");
        _scene = null;
        if (_bridge is not null && _composition is not null)
            _composition.Invoke(_bridge.Dispose);
        Trace("dispose-bridge-pass");
        _bridge = null;
        if (_island is not null && _composition is not null)
            _composition.Invoke(_island.Dispose);
        Trace("dispose-island-pass");
        _island = null;
        _appWindow = null;
        if (_compositionRoot is not null && _composition is not null)
            _composition.Invoke(_compositionRoot.Dispose);
        _compositionRoot = null;
        _composition?.Dispose();
        Trace("dispose-composition-pass");
        _composition = null;
        _islandDispatcher?.ShutdownQueue();
        Trace("dispose-island-dispatcher-pass");
        _islandDispatcher = null;
    }

    private void InitializeDevice()
    {
        Trace("angle-display-start");
        var platformAttributes = new[]
        {
            EglPlatformAngleType, EglPlatformAngleTypeD3D11,
            EglPlatformAngleDeviceType, EglPlatformAngleDeviceTypeHardware,
            EglNone,
        };
        _display = EglGetPlatformDisplayExt(EglPlatformAngle, 0, platformAttributes);
        if (_display == 0 || EglInitialize(_display, out _, out _) == EglFalse)
            ThrowEgl("ANGLE D3D11 display initialization");
        Trace("angle-display-pass");
        if (EglBindApi(EglOpenGlesApi) == EglFalse) ThrowEgl("eglBindAPI");
        var extensions = string.Concat(
            Marshal.PtrToStringAnsi(EglQueryString(0, EglExtensions)), " ",
            Marshal.PtrToStringAnsi(EglQueryString(_display, EglExtensions)));
        foreach (var required in new[] { "EGL_ANGLE_d3d_texture_client_buffer", "EGL_EXT_device_query" })
            if (!extensions.Split(' ', StringSplitOptions.RemoveEmptyEntries).Contains(required, StringComparer.Ordinal))
                throw new PlatformNotSupportedException($"ANGLE is missing {required}.");
        var queryDisplayPointer = EglGetProcAddress("eglQueryDisplayAttribEXT");
        var queryDevicePointer = EglGetProcAddress("eglQueryDeviceAttribEXT");
        if (queryDisplayPointer == 0 || queryDevicePointer == 0)
            throw new PlatformNotSupportedException("ANGLE device-query entrypoints are unavailable.");
        var queryDisplay = Marshal.GetDelegateForFunctionPointer<QueryDisplayAttrib>(queryDisplayPointer);
        var queryDevice = Marshal.GetDelegateForFunctionPointer<QueryDeviceAttrib>(queryDevicePointer);
        if (queryDisplay(_display, EglDeviceExt, out var eglDevice) == EglFalse || eglDevice == 0)
            ThrowEgl("eglQueryDisplayAttribEXT(EGL_DEVICE_EXT)");
        if (queryDevice(eglDevice, EglD3D11DeviceAngle, out _d3d11Device) == EglFalse || _d3d11Device == 0)
            ThrowEgl("eglQueryDeviceAttribEXT(EGL_D3D11_DEVICE_ANGLE)");
        Trace("angle-device-query-pass");

        var configAttributes = new[]
        {
            EglSurfaceType, EglPbufferBit,
            EglRenderableType, EglOpenGles2Bit,
            EglRedSize, 8, EglGreenSize, 8, EglBlueSize, 8, EglAlphaSize, 8,
            EglDepthSize, 0, EglStencilSize, 8, EglNone,
        };
        if (EglChooseConfig(_display, configAttributes, out _config, 1, out var count) == EglFalse ||
            count != 1 || _config == 0)
            ThrowEgl("eglChooseConfig(Acrylic RGBA8/stencil8)");
        _eglContext = EglCreateContext(_display, _config, 0, [EglContextClientVersion, 2, EglNone]);
        if (_eglContext == 0) ThrowEgl("eglCreateContext(Acrylic)");
        Trace("egl-context-pass");

        _probe = new ProbeSnapshot { AbiVersion = 1, StructSize = checked((uint)sizeof(ProbeSnapshot)) };
        Trace("presentation-create-start");
        var create = Create(_d3d11Device, out _presentationContext,
            out _compositionSurfaceHandle, ref _probe);
        Trace($"presentation-create-result-{create}");
        if (create < 0) Marshal.ThrowExceptionForHR(create);
        if (create != 0 || _presentationContext == 0 || _compositionSurfaceHandle == 0 ||
            _probe.PresentationSupported == 0)
            throw new PlatformNotSupportedException("The ANGLE D3D11 device does not support Composition Swapchain presentation.");
        if (_probe.AdapterVendorId == 0x1414)
            throw new PlatformNotSupportedException("experimentalAcrylic does not allow a Microsoft software/WARP adapter.");
        DeviceGeneration++;
    }

    private void Trace(string value)
    {
        if (_diagnosticsEnabled && string.Equals(
                Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_TRACE"),
                "1", StringComparison.Ordinal))
            Console.Error.WriteLine($"doroti.windows.experimental-acrylic={value}");
    }

    private void RecreateDevice()
    {
        ReleaseDevice();
        InitializeDevice();
        if (_scene is not null && _composition is not null)
            _composition.Invoke(() => _scene.ReplaceSurface(_compositionSurfaceHandle));
    }

    private void ReleaseDevice()
    {
        foreach (var slot in _slots)
        {
            // Release Skia and EGL wrappers before dropping the texture COM
            // reference and before the Presentation manager releases buffers.
            ReleaseSlotRenderTarget(slot);
            slot.Release();
        }
        Trace("release-slots-pass");
        _selectedSlot = -1;
        Width = Height = 0;
        lock (_viewportGate)
        {
            _presentedWidth = _presentedHeight = 0;
            _presentedCapacityWidth = _presentedCapacityHeight = 0;
            _placedOffsetX = _placedOffsetY = 0;
            _placedWidth = _placedHeight = 0;
            if (_presentationContext != 0) Destroy(_presentationContext);
            _presentationContext = _compositionSurfaceHandle = 0;
        }
        _context?.AbandonContext(false);
        _context?.Dispose();
        Trace("release-skia-context-pass");
        _context = null;
        _glInterface?.Dispose();
        Trace("release-gl-interface-pass");
        _glInterface = null;
        if (_display != 0) EglMakeCurrent(_display, 0, 0, 0);
        Trace("release-egl-unbind-pass");
        // Presentation buffers must release while ANGLE still owns the D3D11
        // device. The validated P1-CS probe uses this same lifetime order.
        Trace("release-presentation-pass");
        if (_eglContext != 0) EglDestroyContext(_display, _eglContext);
        Trace("release-egl-context-pass");
        if (_display != 0) EglTerminate(_display);
        Trace("release-egl-display-pass");
        _eglContext = _display = _config = _d3d11Device = 0;
    }

    private int SelectAvailableSlot()
    {
        for (var index = 0; index < _slots.Length; index++)
            if (!_slots[index].Registered) return index;
        for (var index = 0; index < _slots.Length; index++)
        {
            var result = IsAvailable(_presentationContext, checked((uint)index), out var available);
            if (result < 0) Marshal.ThrowExceptionForHR(result);
            if (available != 0) return index;
        }
        return -1;
    }

    private bool WaitForAnyAvailableSlot()
    {
        // EnsureTarget is called by Doroti's raster worker. The platform-side
        // WM_SIZE path never waits; only this worker may briefly wait for a
        // compositor-owned Presentation buffer to retire.
        nint* handles = stackalloc nint[_slots.Length];
        uint count = 0;
        foreach (var slot in _slots)
        {
            if (!slot.Registered || slot.AvailableEvent == 0) continue;
            handles[count++] = unchecked((nint)slot.AvailableEvent);
        }
        if (count == 0) return false;
        var result = WaitForMultipleObjects(
            count, handles, false, BufferAvailabilityWaitMilliseconds);
        return result >= WaitObject0 && result < WaitObject0 + count;
    }

    internal bool CanPresentViewport(int width, int height) =>
        IsViewportWithinBudget(width, height);

    private bool IsViewportWithinBudget(int width, int height)
    {
        lock (_viewportGate)
        {
            return _viewportRevision != 0 &&
                IsWithinEdgeBudget(_viewportWidth, _viewportHeight, width, height);
        }
    }

    private static bool IsWithinEdgeBudget(
        int viewportWidth, int viewportHeight, int sourceWidth, int sourceHeight) =>
        Math.Abs((long)viewportWidth - sourceWidth) <= PhysicalEdgeBudget &&
        Math.Abs((long)viewportHeight - sourceHeight) <= PhysicalEdgeBudget;

    private void UpdateResizeAnchors()
    {
        if (_topLevelWindow == 0 || !TryGetClientScreenBounds(_topLevelWindow, out var current))
            return;
        if (_hasClientBounds)
        {
            var leftTravel = Math.Abs(current.Left - _lastClientBounds.Left);
            var rightTravel = Math.Abs(current.Right - _lastClientBounds.Right);
            if (leftTravel > rightTravel + 1) _anchorRight = true;
            else if (rightTravel > leftTravel + 1) _anchorRight = false;

            var topTravel = Math.Abs(current.Top - _lastClientBounds.Top);
            var bottomTravel = Math.Abs(current.Bottom - _lastClientBounds.Bottom);
            if (topTravel > bottomTravel + 1) _anchorBottom = true;
            else if (bottomTravel > topTravel + 1) _anchorBottom = false;
        }
        _lastClientBounds = current;
        _hasClientBounds = true;
    }

    private void PlaceRetainedSurface()
    {
        lock (_viewportGate)
        {
            if (_presentationContext == 0 || _presentedWidth <= 0 || _presentedHeight <= 0)
                return;
            if (!TryGetSourceRect(
                    _viewportWidth, _viewportHeight,
                    _presentedWidth, _presentedHeight,
                    _presentedCapacityWidth, _presentedCapacityHeight,
                    _anchorRight, _anchorBottom,
                    out var sourceX, out var sourceY))
                return;
            if (_placedOffsetX == sourceX && _placedOffsetY == sourceY &&
                _placedWidth == _viewportWidth && _placedHeight == _viewportHeight)
                return;
            var result = Crop(
                _presentationContext,
                checked((uint)sourceX), checked((uint)sourceY),
                checked((uint)_viewportWidth), checked((uint)_viewportHeight),
                ++_presentTag);
            if (result < 0) Marshal.ThrowExceptionForHR(result);
            _placedOffsetX = sourceX;
            _placedOffsetY = sourceY;
            _placedWidth = _viewportWidth;
            _placedHeight = _viewportHeight;
        }
    }

    private T RetainAndReturn<T>(T result, ref bool madeCurrent)
    {
        UnbindImportedSurface(ref madeCurrent);
        PlaceRetainedSurface();
        return result;
    }

    private static bool TryGetSourceRect(
        int viewportWidth, int viewportHeight,
        int sourceWidth, int sourceHeight,
        int capacityWidth, int capacityHeight,
        bool anchorRight, bool anchorBottom,
        out int sourceX, out int sourceY)
    {
        sourceX = checked(GuardBandPixels + (anchorRight ? sourceWidth - viewportWidth : 0));
        sourceY = checked(GuardBandPixels + (anchorBottom ? sourceHeight - viewportHeight : 0));
        return sourceX >= 0 && sourceY >= 0 &&
            viewportWidth <= capacityWidth - sourceX &&
            viewportHeight <= capacityHeight - sourceY;
    }

    private static bool TryGetClientScreenBounds(nint window, out ClientScreenBounds bounds)
    {
        bounds = default;
        if (!GetClientRect(window, out var client)) return false;
        var topLeft = new NativePoint { X = client.Left, Y = client.Top };
        var bottomRight = new NativePoint { X = client.Right, Y = client.Bottom };
        if (!ClientToScreen(window, ref topLeft) || !ClientToScreen(window, ref bottomRight))
            return false;
        bounds = new ClientScreenBounds(
            topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        return true;
    }

    private void ReplaceSlot(int index, int capacityWidth, int capacityHeight)
    {
        var slot = _slots[index];
        ReleaseSlotRenderTarget(slot);
        slot.ReleaseTextureReference();
        var buffer = new BufferSnapshot { AbiVersion = 1, StructSize = checked((uint)sizeof(BufferSnapshot)) };
        var result = ReplaceBuffer(
            _presentationContext, checked((uint)index),
            checked((uint)capacityWidth), checked((uint)capacityHeight),
            out var texture, out var availableEvent, ref buffer);
        if (result < 0) Marshal.ThrowExceptionForHR(result);
        if (texture == 0 || availableEvent == 0 || buffer.InitiallyAvailable == 0)
            throw new InvalidOperationException("A new Acrylic presentation buffer was not available.");
        slot.Texture = texture;
        slot.AvailableEvent = availableEvent;
        slot.CapacityWidth = capacityWidth;
        slot.CapacityHeight = capacityHeight;
        slot.Registered = true;
        _maximumRegisteredSlots = Math.Max(
            _maximumRegisteredSlots, checked((ulong)_slots.Count(static value => value.Registered)));
    }

    private static int RoundBufferCapacity(int value) =>
        checked(((value + BufferCapacityQuantum - 1) / BufferCapacityQuantum) * BufferCapacityQuantum);

    private static bool CanReuseCapacity(
        Slot slot,
        int width,
        int height,
        int requestedCapacityWidth,
        int requestedCapacityHeight)
    {
        var innerWidth = slot.CapacityWidth - GuardBandPixels * 2;
        var innerHeight = slot.CapacityHeight - GuardBandPixels * 2;
        if (!slot.Registered || innerWidth < width || innerHeight < height)
            return false;
        // A large startup surface can otherwise survive a later compact
        // window forever. Skia clears the complete backend target even though
        // Presentation samples only the actual source rect, so bound retained
        // over-allocation with enough hysteresis to avoid resize oscillation.
        var retainedArea = checked((long)innerWidth * innerHeight);
        var requestedArea = checked(
            (long)(requestedCapacityWidth - GuardBandPixels * 2) *
            (requestedCapacityHeight - GuardBandPixels * 2));
        return retainedArea <= requestedArea * MaximumRetainedCapacityAreaRatio;
    }

    private nint CreateImportedSurface(nint texture, int width, int height)
    {
        var innerWidth = checked(width - GuardBandPixels * 2);
        var innerHeight = checked(height - GuardBandPixels * 2);
        var attributes = new[]
        {
            EglWidth, innerWidth, EglHeight, innerHeight,
            EglTextureOffsetXAngle, GuardBandPixels,
            EglTextureOffsetYAngle, GuardBandPixels,
            EglNone,
        };
        var surface = EglCreatePbufferFromClientBuffer(
            _display, EglD3DTextureAngle, texture, _config, attributes);
        if (surface == 0) ThrowEgl("eglCreatePbufferFromClientBuffer(Acrylic texture)");
        return surface;
    }

    private void EnsureSlotRenderTarget(Slot slot)
    {
        if (slot.SkiaSurface is not null) return;
        if (slot.BackendTarget is not null)
            throw new InvalidOperationException("The Acrylic slot has a target without a Skia surface.");
        GlGetIntegerv(GlSamples, out var samples);
        GlGetIntegerv(GlStencilBits, out var stencilBits);
        ThrowIfGlErrors("Acrylic default framebuffer query");
        var target = new GRBackendRenderTarget(
            checked(slot.CapacityWidth - GuardBandPixels * 2),
            checked(slot.CapacityHeight - GuardBandPixels * 2),
            Math.Max(0, samples), Math.Max(0, stencilBits),
            new GRGlFramebufferInfo(0, GlRgba8));
        try
        {
            var surface = SKSurface.Create(
                // The imported D3D11 texture is consumed by Composition with
                // a top-left origin. Unlike an EGL window surface, treating
                // this FBO as bottom-left stores the complete scene inverted.
                _context!, target, GRSurfaceOrigin.TopLeft, SKColorType.Rgba8888)
                ?? throw new InvalidOperationException(
                    "Skia could not wrap the Acrylic texture framebuffer.");
            slot.BackendTarget = target;
            slot.SkiaSurface = surface;
        }
        catch
        {
            target.Dispose();
            throw;
        }
    }

    private void ReleaseSlotRenderTarget(Slot slot)
    {
        slot.SkiaSurface?.Dispose();
        slot.SkiaSurface = null;
        slot.BackendTarget?.Dispose();
        slot.BackendTarget = null;
        if (slot.ImportedSurface == 0) return;
        if (_display != 0) EglDestroySurface(_display, slot.ImportedSurface);
        slot.ImportedSurface = 0;
    }

    private void EnsureSkiaContext()
    {
        if (_context is not null) return;
        _glInterface = GRGlInterface.CreateGles(EglGetProcAddress)
            ?? throw new InvalidOperationException("Skia could not resolve the Acrylic ANGLE GLES interface.");
        _context = GRContext.CreateGl(_glInterface)
            ?? throw new InvalidOperationException("Skia could not create the Acrylic ANGLE context.");
        var renderer = GlGetString(GlRenderer);
        AdapterDescription = renderer == 0
            ? "ANGLE renderer unavailable"
            : Marshal.PtrToStringAnsi(renderer) ?? "ANGLE renderer unavailable";
        if (!AdapterDescription.Contains("ANGLE", StringComparison.OrdinalIgnoreCase) ||
            !(AdapterDescription.Contains("D3D11", StringComparison.OrdinalIgnoreCase) ||
              AdapterDescription.Contains("Direct3D11", StringComparison.OrdinalIgnoreCase)) ||
            AdapterDescription.Contains("WARP", StringComparison.OrdinalIgnoreCase) ||
            AdapterDescription.Contains("SwiftShader", StringComparison.OrdinalIgnoreCase))
            throw new PlatformNotSupportedException(
                $"ANGLE did not select a hardware D3D11 renderer: '{AdapterDescription}'.");
    }

    private void UnbindImportedSurface(ref bool madeCurrent)
    {
        if (!madeCurrent) return;
        if (EglMakeCurrent(_display, 0, 0, 0) == EglFalse)
            ThrowEgl("eglMakeCurrent(Acrylic unbind)");
        madeCurrent = false;
    }

    private void ApplyOptionLoop(PendingOption current)
    {
        while (true)
        {
            try
            {
                var composition = _composition ?? throw new ObjectDisposedException(GetType().Name);
                var scene = _scene ?? throw new InvalidOperationException("The Acrylic scene is unavailable.");
                composition.Invoke(() => scene.Apply(current.Options, _systemBrightness));
                _options = current.Options;
                Interlocked.Increment(ref _appliedOptionRevisions);
                current.Completion.TrySetResult(SerializeTerminal(current.Revision, "applied", current.Options));
            }
            catch (Exception exception)
            {
                Interlocked.Increment(ref _failedOptionRevisions);
                current.Completion.TrySetResult(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                    revision = current.Revision,
                    status = "failed",
                    error = exception.Message,
                })));
            }
            lock (_optionGate)
            {
                if (_pendingOption is null)
                {
                    _optionApplying = false;
                    return;
                }
                current = _pendingOption;
                _pendingOption = null;
            }
        }
    }

    private ReadOnlyMemory<byte> SerializeSnapshot() =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(Snapshot()));

    private static ReadOnlyMemory<byte> SerializeTerminal(
        long revision, string status, WindowBackdropOptions options) =>
        Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
        {
            revision,
            status,
            kind = options.acrylicKind.ToString(),
            theme = options.theme.ToString(),
            tintColor = options.tintColor?.value,
            options.tintOpacity,
            options.luminosityOpacity,
        }));

    private static WindowBackdropOptions ValidateOptions(WindowBackdropOptions options)
    {
        if (options.mode != WindowBackdropMode.experimentalAcrylic)
            throw new ArgumentException("A runtime Acrylic update cannot change the window topology.", nameof(options));
        if (options.tintOpacity is { } tint && (!double.IsFinite(tint) || tint is < 0 or > 1))
            throw new ArgumentOutOfRangeException(nameof(options.tintOpacity));
        if (options.luminosityOpacity is { } luminosity &&
            (!double.IsFinite(luminosity) || luminosity is < 0 or > 1))
            throw new ArgumentOutOfRangeException(nameof(options.luminosityOpacity));
        return options;
    }

    private void ThrowIfGlErrors(string operation)
    {
        var count = 0UL;
        for (var index = 0; index < 16; index++)
        {
            var error = GlGetError();
            if (error == GlNoError) break;
            count++;
            Console.Error.WriteLine($"GLES {operation} error=0x{error:x4}");
        }
        if (count == 0) return;
        if (_debugBaselineSealed)
        {
            OperationalDebugMessageCount += count;
            OperationalDebugErrorCount += count;
        }
        else
        {
            InitializationDebugMessageCount += count;
            InitializationDebugErrorCount += count;
        }
        throw new InvalidOperationException($"{operation} emitted one or more GLES errors.");
    }

    private void ThrowEgl(string operation)
    {
        var error = EglGetError();
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
        throw new InvalidOperationException($"{operation} failed with EGL error 0x{error:x4}.");
    }

    private static SystemBackdropTheme ResolveTheme(WindowBackdropTheme theme, Brightness brightness) => theme switch
    {
        WindowBackdropTheme.light => SystemBackdropTheme.Light,
        WindowBackdropTheme.dark => SystemBackdropTheme.Dark,
        _ => brightness == Brightness.dark ? SystemBackdropTheme.Dark : SystemBackdropTheme.Light,
    };

    private sealed class Slot
    {
        internal bool Registered;
        internal nint Texture;
        internal nint ImportedSurface;
        internal GRBackendRenderTarget? BackendTarget;
        internal SKSurface? SkiaSurface;
        internal ulong AvailableEvent;
        internal int Width;
        internal int Height;
        internal int CapacityWidth;
        internal int CapacityHeight;

        internal void ReleaseTextureReference()
        {
            if (Texture != 0) Marshal.Release(Texture);
            Texture = 0;
        }

        internal void Release()
        {
            ReleaseTextureReference();
            Registered = false;
            AvailableEvent = 0;
            Width = Height = CapacityWidth = CapacityHeight = 0;
        }
    }

    private readonly record struct ClientScreenBounds(
        int Left, int Top, int Right, int Bottom);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    private sealed record PendingOption(
        long Revision,
        WindowBackdropOptions Options,
        TaskCompletionSource<ReadOnlyMemory<byte>?> Completion);

    private sealed record RuntimeOptionRequest(
        string? Kind,
        string? Theme,
        uint? TintColor,
        double? TintOpacity,
        double? LuminosityOpacity)
    {
        internal WindowBackdropOptions ApplyTo(WindowBackdropOptions current) => current with
        {
            acrylicKind = Kind?.ToLowerInvariant() switch
            {
                null => current.acrylicKind,
                "default" => WindowAcrylicKind.@default,
                "base" => WindowAcrylicKind.@base,
                "thin" => WindowAcrylicKind.thin,
                _ => throw new InvalidDataException($"Unknown Acrylic kind '{Kind}'."),
            },
            theme = Theme?.ToLowerInvariant() switch
            {
                null => current.theme,
                "system" => WindowBackdropTheme.system,
                "light" => WindowBackdropTheme.light,
                "dark" => WindowBackdropTheme.dark,
                _ => throw new InvalidDataException($"Unknown Acrylic theme '{Theme}'."),
            },
            tintColor = TintColor is { } color ? new Color(color) : current.tintColor,
            tintOpacity = TintOpacity ?? current.tintOpacity,
            luminosityOpacity = LuminosityOpacity ?? current.luminosityOpacity,
        };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct ProbeSnapshot
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal int FactoryHresult;
        internal int ManagerHresult;
        internal int SurfaceHandleHresult;
        internal int PresentationSurfaceHresult;
        internal int RetiringFenceHresult;
        internal uint DeviceCreationFlags;
        internal uint PresentationSupported;
        internal uint IndependentFlipSupported;
        internal int AdapterLuidLow;
        internal int AdapterLuidHigh;
        internal uint AdapterVendorId;
        internal uint AdapterDeviceId;
        internal ulong RetiringFenceCompletedValue;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    private struct BufferSnapshot
    {
        internal uint AbiVersion;
        internal uint StructSize;
        internal int TextureHresult;
        internal int AddBufferHresult;
        internal int AvailableEventHresult;
        internal uint Width;
        internal uint Height;
        internal uint Format;
        internal uint BindFlags;
        internal uint MiscFlags;
        internal uint InitiallyAvailable;
    }

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate int QueryDisplayAttrib(nint display, int attribute, out nint value);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate int QueryDeviceAttrib(nint device, int attribute, out nint value);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_create_v1")]
    private static partial int Create(
        nint d3d11Device, out nint context, out nint compositionSurfaceHandle,
        ref ProbeSnapshot snapshot);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_destroy_v1")]
    private static partial void Destroy(nint context);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_replace_buffer_v1")]
    private static partial int ReplaceBuffer(
        nint context, uint slotIndex, uint width, uint height,
        out nint texture, out ulong availableEvent, ref BufferSnapshot snapshot);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_is_available_v1")]
    private static partial int IsAvailable(nint context, uint slotIndex, out uint available);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_present_v1")]
    private static partial int Present(
        nint context, uint slotIndex, uint width, uint height, ulong tag,
        out ulong presentId, out ulong retiringFenceValue);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_present_positioned_v1")]
    private static partial int PresentPositioned(
        nint context, uint slotIndex, uint width, uint height,
        float offsetX, float offsetY, ulong tag,
        out ulong presentId, out ulong retiringFenceValue);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_present_cropped_v1")]
    private static partial int PresentCropped(
        nint context, uint slotIndex,
        uint sourceX, uint sourceY, uint width, uint height, ulong tag,
        out ulong presentId, out ulong retiringFenceValue);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_crop_v1")]
    private static partial int Crop(
        nint context, uint sourceX, uint sourceY,
        uint width, uint height, ulong tag);

    [LibraryImport(WindowsNativeV1.LibraryName, EntryPoint = "doroti_windows_acrylic_place_v1")]
    private static partial int Place(
        nint context, float offsetX, float offsetY, ulong tag);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetClientRect(nint window, out NativeRect rect);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ClientToScreen(nint window, ref NativePoint point);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial uint WaitForMultipleObjects(
        uint count, nint* handles,
        [MarshalAs(UnmanagedType.Bool)] bool waitAll, uint milliseconds);

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetPlatformDisplayEXT", ExactSpelling = true)]
    private static extern nint EglGetPlatformDisplayExt(uint platform, nint nativeDisplay, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_Initialize", ExactSpelling = true)]
    private static extern int EglInitialize(nint display, out int major, out int minor);

    [DllImport(AngleLibrary, EntryPoint = "EGL_BindAPI", ExactSpelling = true)]
    private static extern int EglBindApi(int api);

    [DllImport(AngleLibrary, EntryPoint = "EGL_QueryString", ExactSpelling = true)]
    private static extern nint EglQueryString(nint display, int name);

    [DllImport(AngleLibrary, EntryPoint = "EGL_ChooseConfig", ExactSpelling = true)]
    private static extern int EglChooseConfig(
        nint display, int[] attributes, out nint config, int configSize, out int count);

    [DllImport(AngleLibrary, EntryPoint = "EGL_CreateContext", ExactSpelling = true)]
    private static extern nint EglCreateContext(nint display, nint config, nint sharedContext, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_CreatePbufferFromClientBuffer", ExactSpelling = true)]
    private static extern nint EglCreatePbufferFromClientBuffer(
        nint display, int bufferType, nint buffer, nint config, int[] attributes);

    [DllImport(AngleLibrary, EntryPoint = "EGL_MakeCurrent", ExactSpelling = true)]
    private static extern int EglMakeCurrent(nint display, nint drawSurface, nint readSurface, nint context);

    [DllImport(AngleLibrary, EntryPoint = "EGL_DestroySurface", ExactSpelling = true)]
    private static extern int EglDestroySurface(nint display, nint surface);

    [DllImport(AngleLibrary, EntryPoint = "EGL_DestroyContext", ExactSpelling = true)]
    private static extern int EglDestroyContext(nint display, nint context);

    [DllImport(AngleLibrary, EntryPoint = "EGL_Terminate", ExactSpelling = true)]
    private static extern int EglTerminate(nint display);

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetError", ExactSpelling = true)]
    private static extern int EglGetError();

    [DllImport(AngleLibrary, EntryPoint = "EGL_GetProcAddress", ExactSpelling = true, CharSet = CharSet.Ansi)]
    private static extern nint EglGetProcAddress(string name);

    [DllImport(AngleLibrary, EntryPoint = "glGetError", ExactSpelling = true)]
    private static extern uint GlGetError();

    [DllImport(AngleLibrary, EntryPoint = "glGetIntegerv", ExactSpelling = true)]
    private static extern void GlGetIntegerv(uint name, out int value);

    [DllImport(AngleLibrary, EntryPoint = "glGetString", ExactSpelling = true)]
    private static extern nint GlGetString(uint name);

    [DllImport(AngleLibrary, EntryPoint = "glFlush", ExactSpelling = true)]
    private static extern void GlFlush();
}

internal sealed record AcrylicPresenterSnapshot(
    string RequestedMode,
    string EffectiveMode,
    string? FallbackReason,
    bool PresentationSupported,
    bool IndependentFlipSupported,
    string AdapterLuid,
    uint AdapterVendorId,
    uint AdapterDeviceId,
    string AcrylicKind,
    string Theme,
    uint? TintColor,
    double? TintOpacity,
    double? LuminosityOpacity,
    int LogicalActiveEdgeBudget,
    int PhysicalActiveEdgeBudget,
    ulong MaximumRegisteredSlots,
    ulong AvailableReuseCount,
    ulong UnavailableSkipCount,
    long AcceptedOptionRevisions,
    long AppliedOptionRevisions,
    long SupersededOptionRevisions,
    long FailedOptionRevisions,
    bool BackdropTargetAdded,
    bool ContentIslandConnected,
    bool DesktopWindowTargetConnected = false,
    bool HostBackdropBrushEnabled = false,
    string BackdropTransport = "DesktopAcrylicController",
    string? SystemBackdropType = null,
    bool RedirectionBitmapAlphaEnabled = false,
    string? BackdropState = null);

internal sealed class AcrylicCompositionWorker : IDisposable
{
    private readonly DispatcherQueueController _dispatcher;
    private bool _disposed;

    internal AcrylicCompositionWorker(DispatcherQueueController dispatcher)
    {
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        ThreadId = Environment.CurrentManagedThreadId;
        Compositor = new Compositor();
    }

    internal int ThreadId { get; private set; }
    internal Compositor Compositor { get; private set; } = null!;

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
            throw new InvalidOperationException("Composition DispatcherQueue rejected work.");
        if (!completed.Wait(TimeSpan.FromSeconds(15)))
            throw new TimeoutException("Composition DispatcherQueue work timed out.");
        if (failure is not null)
            throw new InvalidOperationException(
                $"Composition DispatcherQueue work failed: {failure.GetType().Name}: {failure.Message}",
                failure);
        return result!;
    }

    internal void Invoke(Action callback) => Invoke(() =>
    {
        callback();
        return true;
    });

    internal void Post(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (ThreadId != 0 && Environment.CurrentManagedThreadId == ThreadId)
        {
            callback();
            return;
        }
        if (!_dispatcher.DispatcherQueue.TryEnqueue(() => callback()))
            throw new InvalidOperationException("Composition DispatcherQueue rejected work.");
    }

    public void Dispose()
    {
        if (_disposed) return;
        Invoke(Compositor.Dispose);
        _disposed = true;
    }
}

internal sealed class AcrylicScene : IDisposable
{
    private readonly Compositor _compositor;
    private readonly ContentIsland _island;
    private readonly ContainerVisual _root;
    private readonly SpriteVisual _content;
    private readonly DesktopAcrylicController _backdrop;
    private readonly SystemBackdropConfiguration _configuration;
    private ICompositionSurface? _surface;
    private CompositionSurfaceBrush? _brush;
    private bool _disposed;

    internal AcrylicScene(
        Compositor compositor,
        ContainerVisual root,
        ContentIsland island,
        nint surfaceHandle,
        WindowBackdropOptions options,
        Brightness systemBrightness)
    {
        _compositor = compositor;
        _island = island;
        _root = root;
        _content = compositor.CreateSpriteVisual();
        _root.Children.InsertAtTop(_content);
        _configuration = new SystemBackdropConfiguration { IsInputActive = true };
        _backdrop = new DesktopAcrylicController();
        BackdropTargetAdded = _backdrop.AddSystemBackdropTarget(island);
        ReplaceSurface(surfaceHandle);
        Apply(options, systemBrightness);
    }

    internal bool BackdropTargetAdded { get; }

    internal void ReplaceSurface(nint surfaceHandle)
    {
        _brush?.Dispose();
        if (_surface is IDisposable disposable) disposable.Dispose();
        _surface = CreateCompositionSurfaceForHandle(_compositor, surfaceHandle);
        _brush = _compositor.CreateSurfaceBrush(_surface);
        _brush.Stretch = CompositionStretch.None;
        _brush.HorizontalAlignmentRatio = 0;
        _brush.VerticalAlignmentRatio = 0;
        _brush.Offset = Vector2.Zero;
        _brush.Scale = Vector2.One;
        _brush.SnapToPixels = true;
        _content.Brush = _brush;
    }

    internal void Apply(WindowBackdropOptions options, Brightness systemBrightness)
    {
        _backdrop.ResetProperties();
        _backdrop.Kind = options.acrylicKind switch
        {
            WindowAcrylicKind.@base => DesktopAcrylicKind.Base,
            WindowAcrylicKind.thin => DesktopAcrylicKind.Thin,
            _ => DesktopAcrylicKind.Default,
        };
        if (options.tintColor is { } tint)
        {
            var value = tint.value;
            _backdrop.TintColor = Windows.UI.Color.FromArgb(
                (byte)(value >> 24), (byte)(value >> 16), (byte)(value >> 8), (byte)value);
        }
        if (options.tintOpacity is { } tintOpacity) _backdrop.TintOpacity = (float)tintOpacity;
        if (options.luminosityOpacity is { } luminosityOpacity)
            _backdrop.LuminosityOpacity = (float)luminosityOpacity;
        _configuration.Theme = options.theme switch
        {
            WindowBackdropTheme.light => SystemBackdropTheme.Light,
            WindowBackdropTheme.dark => SystemBackdropTheme.Dark,
            _ => systemBrightness == Brightness.dark
                ? SystemBackdropTheme.Dark : SystemBackdropTheme.Light,
        };
        _backdrop.SetSystemBackdropConfiguration(_configuration);
    }

    internal void PrepareViewport(
        float width, float height, bool anchorRight, bool anchorBottom)
    {
        if (_disposed) throw new ObjectDisposedException(GetType().Name);
        _root.Size = new Vector2(width, height);
        _content.Size = new Vector2(width, height);
        if (_brush is null) throw new ObjectDisposedException(GetType().Name);
        _brush.Stretch = CompositionStretch.None;
        // Stretch=None preserves native pixels. Align the retained surface to
        // the stationary edges while the shell changes the opposite edges;
        // any not-yet-rastered strip therefore reveals Acrylic instead of
        // moving/scaling the previous frame or exposing an opaque HWND fill.
        _brush.HorizontalAlignmentRatio = anchorRight ? 1 : 0;
        _brush.VerticalAlignmentRatio = anchorBottom ? 1 : 0;
        _brush.Offset = Vector2.Zero;
        _brush.Scale = Vector2.One;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _backdrop.RemoveSystemBackdropTarget(_island);
        _backdrop.Dispose();
        _content.Dispose();
        _brush?.Dispose();
        if (_surface is IDisposable disposable) disposable.Dispose();
        _surface = null;
    }

    internal static unsafe ICompositionSurface CreateCompositionSurfaceForHandle(
        Compositor compositor, nint surfaceHandle)
    {
        var iid = new Guid("FC084699-67D8-40E1-ADE7-08901D84FFDA");
        using var interop = ((WinRT.IWinRTObject)compositor).NativeObject.As(iid);
        var thisPointer = interop.ThisPtr;
        var vtable = *(nint**)thisPointer;
        var create = (delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)vtable[4];
        nint result = 0;
        var hresult = create(thisPointer, surfaceHandle, &result);
        if (hresult < 0) Marshal.ThrowExceptionForHR(hresult);
        try { return WinRT.MarshalInterface<ICompositionSurface>.FromAbi(result); }
        finally { Marshal.Release(result); }
    }

}
