using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.WindowsAppSdk;

public static unsafe partial class DorotiWindowsAppSdkRunner
{
    internal static WindowsProductRunDiagnostics? LastRunDiagnostics { get; private set; }
    internal static NativeHostProvenance? LastNativeProvenance { get; private set; }

    public static int Run(DorotiApplicationDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("Doroti.Host.WindowsAppSdk can only launch on Windows.");
        var adapter = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_ADAPTER");
        if (!string.IsNullOrWhiteSpace(adapter) &&
            !adapter.Equals("HwndExactCpp", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException(
                $"Unsupported Windows App SDK adapter '{adapter}'. Expected HwndExactCpp.");

        LastNativeProvenance = WindowsNativeV1.ConfigureAppDirectoryLoading();
        WindowsNativeV1.ValidateLayout();
        WindowsNativeV1.EnsureSelfContainedWindowsAppRuntime();
        var initializeResult = RoInitialize(0);
        if (initializeResult < 0) Marshal.ThrowExceptionForHR(initializeResult);
        try
        {
            return RunCore(descriptor);
        }
        finally
        {
            RoUninitialize();
        }
    }

    private static int RunCore(DorotiApplicationDescriptor descriptor)
    {
        LastRunDiagnostics = null;
        using var application = DorotiApplicationBoundary.Load(
            descriptor.ManifestAssembly,
            descriptor.ApplicationAssembly,
            descriptor.LaunchContext.RuntimeIdentifier,
            descriptor.NativePluginHandlers);
        using var session = new DorotiHostSession(descriptor.EntrypointFactory());
        using var state = new WindowsManagedState(session, application, descriptor.ViewConfiguration);
        // Experimental ContentIsland activation must occur on the HWND thread
        // during host-ready. Its process-wide DLL search restriction is
        // applied there immediately after attach and still before first show.
        // Opaque does not need that delayed WinRT activation.
        if (state.NativeRequiredFeatures == 0)
            WindowsNativeV1.RestrictProcessDllSearch();
        var handle = GCHandle.Alloc(state);
        try
        {
            session.Start(deferFrameworkBootstrap: true);
            var applicationId = Encoding.UTF8.GetBytes(application.Manifest.ApplicationId);
            var title = Encoding.UTF8.GetBytes(descriptor.ViewConfiguration.title);
            fixed (byte* applicationIdData = applicationId)
            fixed (byte* titleData = title)
            {
                var initialScale = GetDpiForSystem() / 96d;
                var configuration = new WindowsNativeV1.Configuration
                {
                    AbiVersion = WindowsNativeV1.AbiVersion,
                    StructSize = checked((uint)sizeof(WindowsNativeV1.Configuration)),
                    ApplicationId = Utf8(applicationIdData, applicationId.Length),
                    Title = Utf8(titleData, title.Length),
                    InitialWidthPx = ToDimension(descriptor.ViewConfiguration.logicalSize.width * initialScale),
                    InitialHeightPx = ToDimension(descriptor.ViewConfiguration.logicalSize.height * initialScale),
                    NCmdShow = 1,
                    RequiredFeatures = state.NativeRequiredFeatures,
                };
                var callbacks = new WindowsNativeV1.Callbacks
                {
                    AbiVersion = WindowsNativeV1.AbiVersion,
                    StructSize = checked((uint)sizeof(WindowsNativeV1.Callbacks)),
                    CallbackContext = GCHandle.ToIntPtr(handle),
                    HostReady = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.Host*, void>)&OnHostReady,
                    Metrics = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.Metrics*, void>)&OnMetrics,
                    Render = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.FrameRequest*, uint>)&OnRender,
                    FrameTerminal = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.FrameTerminal*, void>)&OnFrameTerminal,
                    Log = (nint)(delegate* unmanaged[Cdecl]<nint, uint, WindowsNativeV1.Utf8, void>)&OnLog,
                    Pointer = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.Pointer*, void>)&OnPointer,
                    Key = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.Key*, void>)&OnKey,
                    Focus = (nint)(delegate* unmanaged[Cdecl]<nint, ulong, uint, long, void>)&OnFocus,
                    Clipboard = (nint)(delegate* unmanaged[Cdecl]<nint, ulong, WindowsNativeV1.Utf8, void>)&OnClipboard,
                    TextEditing = (nint)(delegate* unmanaged[Cdecl]<nint, WindowsNativeV1.TextState*, void>)&OnTextEditing,
                    TextAction = (nint)(delegate* unmanaged[Cdecl]<nint, uint, void>)&OnTextAction,
                    SemanticsAction = (nint)(delegate* unmanaged[Cdecl]<nint, long, long, WindowsNativeV1.Utf8, void>)&OnSemanticsAction,
                    Lifecycle = (nint)(delegate* unmanaged[Cdecl]<nint, ulong, uint, long, void>)&OnLifecycle,
                    PlatformBrightness = (nint)(delegate* unmanaged[Cdecl]<nint, ulong, uint, void>)&OnPlatformBrightness,
                    PlatformResourcesShutdown = (nint)(delegate* unmanaged[Cdecl]<nint, void>)&OnPlatformResourcesShutdown,
                    CompositionResize = (nint)(delegate* unmanaged[Cdecl]<nint, uint, uint, double, void>)&OnCompositionResize,
                };
                var status = WindowsNativeV1.Run(in configuration, in callbacks);
                state.MarkNativeStopped();
                state.ThrowIfFatal();
                if (status != WindowsNativeV1.Status.Ok)
                    throw new InvalidOperationException($"Native HwndExactCpp product host failed: {status}.");
                state.ValidateTerminalCoverage();
                if (ShouldWriteDiagnostics()) state.WriteDiagnostics();
                return 0;
            }
        }
        finally
        {
            if (handle.IsAllocated) handle.Free();
        }
    }

    private sealed class WindowsManagedState : IDisposable
    {
        private readonly object _gate = new();
        private readonly DorotiHostSession _session;
        private readonly DorotiApplicationBoundary _application;
        private readonly DorotiViewConfiguration _configuration;
        private readonly Dictionary<ulong, SkiaPaintCompletion> _paintCompletions = [];
        private Exception? _fatal;
        private DorotiViewCapabilities? _capabilities;
        private long _renderCallbacks;
        private long _presented;
        private long _superseded;
        private long _failed;
        private long _staleInputPresentPrevented;
        private uint _platformThreadId;
        private uint _rasterThreadId;
        private uint _inputThreadId;
        private double _presenterScale;
        private ulong _presenterResizeGeneration;
        private ulong _lastPresentedResizeGeneration;
        private bool _visibleAfterExactPresent;
        private bool _readyFileWritten;
        private bool _deviceResetInjected;
        private bool _disposed;
        private AcrylicPresenterSnapshot? _releasedAcrylicSnapshot;
        private string? _releasedAdapterDescription;
        private Task? _optionSmoke;

        internal WindowsManagedState(
            DorotiHostSession session,
            DorotiApplicationBoundary application,
            DorotiViewConfiguration configuration)
        {
            _session = session;
            _application = application;
            _configuration = configuration;
            RequestedMode = configuration.backdrop?.mode == WindowBackdropMode.experimentalAcrylic
                ? "experimentalAcrylic" : "opaque";
            if (RequestedMode == "experimentalAcrylic")
            {
                try
                {
                    if (ShouldWriteDiagnostics())
                        Console.Error.WriteLine("doroti.windows.experimental-acrylic=pre-window-probe-start");
                    Presenter = new WindowsManagedAcrylicCompositionPresenter(
                        ShouldWriteDiagnostics(),
                        configuration.backdrop!,
                        Brightness.light);
                    EffectiveMode = "experimentalAcrylic";
                    NativeRequiredFeatures = WindowsNativeV1.ExperimentalAcrylicFeature;
                    if (ShouldWriteDiagnostics())
                        Console.Error.WriteLine("doroti.windows.experimental-acrylic=pre-window-probe-pass");
                }
                catch (Exception exception)
                {
                    FallbackReason = $"pre-window:{exception.GetType().Name}:{exception.Message}";
                    Presenter = CreatePresenter(ShouldWriteDiagnostics());
                    EffectiveMode = "opaque";
                }
            }
            else
            {
                Presenter = CreatePresenter(ShouldWriteDiagnostics());
                EffectiveMode = "opaque";
            }
        }

        internal WindowsManagedProductHost? Host { get; private set; }
        internal WindowsManagedHwndPresenterBase Presenter { get; private set; }
        internal SkiaSceneRenderer? Renderer { get; private set; }
        internal DorotiView? View { get; private set; }
        internal string RequestedMode { get; }
        internal string EffectiveMode { get; private set; }
        internal string? FallbackReason { get; private set; }
        internal ulong NativeRequiredFeatures { get; }

        internal void SetHost(in WindowsNativeV1.Host native)
        {
            RecordThread(ref _platformThreadId, "platform");
            var effectiveNative = native;
            if (Presenter is WindowsManagedAcrylicCompositionPresenter acrylic)
            {
                try
                {
                    if (ShouldWriteDiagnostics())
                        Console.Error.WriteLine("doroti.windows.experimental-acrylic=content-island-attach-start");
                    acrylic.ApplySystemBrightness((Brightness)native.InitialPlatformBrightness);
                    acrylic.AttachWindow(native.TopLevelHwnd);
                    if (string.Equals(
                            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_OPTION_SMOKE"),
                            "1", StringComparison.Ordinal))
                        _optionSmoke = Task.Run(() => RunAcrylicOptionSmoke(acrylic));
                    if (ShouldWriteDiagnostics())
                        Console.Error.WriteLine("doroti.windows.experimental-acrylic=content-island-attach-pass");
                }
                catch (Exception exception)
                {
                    acrylic.Dispose();
                    var fallback = (delegate* unmanaged[Cdecl]<nint, uint>)native.RequestOpaqueFallback;
                    var fallbackStatus = fallback(native.HostContext);
                    if (fallbackStatus != 0)
                        throw new InvalidOperationException(
                            $"experimentalAcrylic initialization failed and opaque fallback returned {fallbackStatus}.",
                            exception);
                    FallbackReason = $"pre-show:{exception.GetType().Name}:{exception.Message}";
                    EffectiveMode = "opaque";
                    Presenter = CreatePresenter(ShouldWriteDiagnostics());
                    effectiveNative.ChildHwnd = native.OpaqueChildHwnd;
                }
            }
            WindowsNativeV1.RestrictProcessDllSearch();
            var host = new WindowsManagedProductHost(in effectiveNative,
                checked((int)_configuration.logicalSize.width),
                checked((int)_configuration.logicalSize.height));
            var presenterSlug = Presenter.BackendName.ToLowerInvariant().Replace('/', '-');
            var topology = EffectiveMode == "experimentalAcrylic" ? "content-island" : "cpp-child-hwnd";
            var target = $"win-x64/windowsappsdk-2.4/{topology}/managed-{presenterSlug}-skia";
            var renderer = new SkiaSceneRenderer(
                1, host, _configuration.backgroundColor, _configuration.darkBackgroundColor,
                target, Presenter.RuntimeEffectsBackend,
                $"windowsappsdk-2.4-hwnd-{presenterSlug}-skia-managed");
            var messages = new WindowsAppSdkPlatformMessageCapability();
            if (Presenter is WindowsManagedAcrylicCompositionPresenter activeAcrylic)
                messages.SetMessageHandler(
                    WindowsManagedAcrylicCompositionPresenter.RuntimeChannel,
                    activeAcrylic.HandleRuntimeMessageAsync);
            var capabilities = new DorotiViewCapabilities(target)
                .Register<IViewHostCapability>(DorotiCapabilityIds.WindowLifecycle, host)
                .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
                .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
                .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
                .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
                .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, host)
                .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host)
                .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
                .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
                .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, renderer);
            capabilities.Register<ISemanticsHostCapability>(DorotiCapabilityIds.AccessibilitySemantics, renderer);
            _application.Configure(capabilities, messages);
            DorotiView? view = null;
            try
            {
                using var scope = _session.dispatcher.EnterScope();
                view = _session.dispatcher.RegisterView(1, capabilities);
                renderer.AttachFrameworkTrace(_session.dispatcher.frameTrace);
                Host = host;
                Renderer = renderer;
                _session.AttachView(view);
                renderer.AttachSurface(host.RequestInvalidate);
                View = view;
                _capabilities = capabilities;
                host.Show();
            }
            catch
            {
                Host = null;
                Renderer = null;
                if (view is null) capabilities.Dispose();
                else view.Dispose();
                renderer.Dispose();
                host.Dispose();
                throw;
            }
        }

        internal void ApplyMetrics(in WindowsNativeV1.Metrics metrics) =>
            (Host ?? throw new InvalidOperationException("Metrics arrived before host-ready."))
                .ApplyMetrics(in metrics);

        internal void ResizeComposition(uint width, uint height, double scale)
        {
            if (Presenter is not WindowsManagedAcrylicCompositionPresenter acrylic)
                throw new InvalidOperationException(
                    "A Composition viewport resize arrived without an Acrylic presenter.");
            acrylic.ResizeViewport(
                checked((int)width), checked((int)height), scale);
        }

        private static void RunAcrylicOptionSmoke(WindowsManagedAcrylicCompositionPresenter acrylic)
        {
            const int requestCount = 500;
            var requests = new Task<ReadOnlyMemory<byte>?>[requestCount];
            for (var index = 0; index < requestCount; index++)
            {
                var kind = (index % 3) switch { 0 => "default", 1 => "base", _ => "thin" };
                var theme = index % 2 == 0 ? "light" : "dark";
                var payload = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                    kind,
                    theme,
                    tintColor = 0xff204060u + (uint)(index & 0x1f),
                    tintOpacity = (index % 11) / 10d,
                    luminosityOpacity = (index % 6) / 5d,
                }));
                requests[index] = acrylic.HandleRuntimeMessageAsync(payload, CancellationToken.None).AsTask();
            }
            Task.WaitAll(requests);
            var terminals = requests.Select(task => task.Result)
                .Select(result => result is { } value
                    ? JsonDocument.Parse(value)
                    : throw new InvalidDataException("Acrylic option smoke returned no terminal."))
                .ToArray();
            try
            {
                if (terminals.Any(terminal =>
                        !terminal.RootElement.TryGetProperty("status", out var status) ||
                        status.GetString() is not ("applied" or "superseded")))
                    throw new InvalidDataException("Acrylic option smoke returned a failed or malformed terminal.");
                if (terminals[^1].RootElement.GetProperty("status").GetString() != "applied")
                    throw new InvalidDataException("Acrylic option smoke did not apply the last request.");
                var snapshot = acrylic.Snapshot();
                if (snapshot.AcceptedOptionRevisions != requestCount || snapshot.FailedOptionRevisions != 0 ||
                    snapshot.AppliedOptionRevisions + snapshot.SupersededOptionRevisions != requestCount ||
                    snapshot.AcrylicKind != "base" || snapshot.Theme != "Dark")
                    throw new InvalidDataException(
                        $"Acrylic option smoke counters or last-request-wins state differ: {snapshot}.");
            }
            finally
            {
                foreach (var terminal in terminals) terminal.Dispose();
            }
        }

        internal void MarkNativeStopped() => Host?.MarkNativeStopped();

        internal void ReleasePlatformResources()
        {
            if (Presenter is WindowsManagedAcrylicCompositionPresenter acrylic)
            {
                if (_optionSmoke is { } smoke && !smoke.Wait(TimeSpan.FromSeconds(5)))
                    throw new TimeoutException("Acrylic option smoke did not drain before platform shutdown.");
                _releasedAcrylicSnapshot = acrylic.Snapshot();
                _releasedAdapterDescription = acrylic.AdapterDescription;
                acrylic.Dispose();
            }
        }

        internal uint Render(in WindowsNativeV1.FrameRequest request)
        {
            RecordThread(ref _rasterThreadId, "raster");
            var host = Host ?? throw new InvalidOperationException("Render arrived before host-ready.");
            var renderer = Renderer ?? throw new InvalidOperationException("Renderer is unavailable.");
            var dispatchedFrameworkFrame = host.BeginFrame(in request);
            var width = checked((int)request.WidthPx);
            var height = checked((int)request.HeightPx);
            var causalFrameId = checked((long)request.CausalFrameId);
            var resizeGeneration = request.Generation;
            if (!dispatchedFrameworkFrame &&
                _lastPresentedResizeGeneration == resizeGeneration &&
                Presenter.Width == width && Presenter.Height == height)
            {
                // The exact surface for this generation is already visible
                // and no framework callback produced newer scene work. Treat
                // the wakeup as satisfied without replaying the retained scene
                // through the GPU and presentation queue a second time.
                Interlocked.Increment(ref _renderCallbacks);
                return (uint)WindowsNativeV1.FrameTerminalKind.Presented;
            }
            if (!_deviceResetInjected && Interlocked.Read(ref _renderCallbacks) >= 1 &&
                Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_C8_SMOKE") == "1")
            {
                renderer.InvalidateGpuContextResources();
                Presenter.ResetDevice();
                _deviceResetInjected = true;
            }
            var scale = host.ResizeTarget.DeviceScaleX;
            var dpiContextChanged = _presenterScale > 0 && _presenterScale != scale;
            if (dpiContextChanged)
            {
                // A cross-DPI transition invalidates ANGLE, Skia, and pooled
                // GPU state together. The move-end generation below performs
                // one final surface-only refresh after shell geometry settles.
                renderer.InvalidateGpuContextResources();
                if (Presenter is WindowsManagedAngleEglPresenter)
                    Presenter.ResetDevice();
            }
            var stableMoveRefresh = _presenterResizeGeneration > 0 &&
                _presenterResizeGeneration != resizeGeneration &&
                _presenterScale == scale && Presenter.Width == width && Presenter.Height == height;
            if (stableMoveRefresh && Presenter is WindowsManagedAngleEglPresenter movePresenter)
            {
                renderer.InvalidateWindowSurfaceResources();
                movePresenter.ResetWindowSurfaceAfterInteractiveMove();
            }
            var windowSurfaceChanged = Presenter.Width != width || Presenter.Height != height;
            if (windowSurfaceChanged && !stableMoveRefresh &&
                Presenter is not WindowsManagedAcrylicCompositionPresenter)
                renderer.InvalidateWindowSurfaceResources();
            if (!Presenter.EnsureTarget(host.ChildHwnd, width, height))
            {
                Interlocked.Increment(ref _renderCallbacks);
                return (uint)WindowsNativeV1.FrameTerminalKind.Superseded;
            }
            _presenterScale = scale;
            _presenterResizeGeneration = resizeGeneration;
            Presenter.SealInitializationDebugBaseline();
            var presented = false;
            var staleInputPrevented = false;
            var result = Presenter.RenderAndPresent(
                surface =>
                {
                    var paintResult = renderer.Paint(
                        surface, width, height, host.ResizeTarget, causalFrameId);
                    if (ShouldDrawValidationFrameMarker())
                        DrawValidationFrameMarker(
                            surface.Canvas, width, height, scale, checked((long)resizeGeneration));
                    return paintResult;
                },
                value =>
                {
                    if (!value.ShouldPresent || value.Completion is not { } candidate)
                        return presented = false;
                    if (!host.IsInputSequenceCurrent(candidate.InputSequence))
                    {
                        staleInputPrevented = true;
                        return presented = false;
                    }
                    return presented = host.IsLatestResizeGeneration(resizeGeneration);
                });
            presented &= Presenter.LastPresentSucceeded;
            if (staleInputPrevented) Interlocked.Increment(ref _staleInputPresentPrevented);
            if (!presented && result.Completion is { IsNewFrame: true } stale)
            {
                var reason = host.IsInputSequenceCurrent(stale.InputSequence)
                    ? "native presentation was superseded before swap"
                    : $"input sequence {stale.InputSequence} is older than current {host.InputSequence}";
                renderer.SupersedePaint(stale, reason);
            }
            Presenter.CaptureOperationalDebugMessages();
            if (Presenter.OperationalDebugErrorCount != 0)
                throw new InvalidOperationException(
                    $"Managed {Presenter.BackendName} presentation emitted " +
                    $"{Presenter.OperationalDebugErrorCount} operational GPU errors.");
            Interlocked.Increment(ref _renderCallbacks);
            if (presented)
                _lastPresentedResizeGeneration = resizeGeneration;
            if (presented && result.Completion is { } completion)
            {
                lock (_gate) _paintCompletions.Add(request.CausalFrameId, completion);
            }
            return presented
                ? (uint)WindowsNativeV1.FrameTerminalKind.Presented
                : (uint)WindowsNativeV1.FrameTerminalKind.Superseded;
        }

        internal void CompleteTerminal(in WindowsNativeV1.FrameTerminal terminal)
        {
            var host = Host ?? throw new InvalidOperationException("Terminal arrived before host-ready.");
            host.CompleteTerminal(in terminal);
            SkiaPaintCompletion? completion = null;
            lock (_gate)
            {
                if (_paintCompletions.Remove(terminal.CausalFrameId, out var value)) completion = value;
            }
            switch ((WindowsNativeV1.FrameTerminalKind)terminal.TerminalKind)
            {
                case WindowsNativeV1.FrameTerminalKind.Presented:
                    if (completion is { } painted) Renderer?.CompletePaint(painted);
                    _visibleAfterExactPresent |= IsWindowVisible(host.TopLevelHwnd);
                    if (_visibleAfterExactPresent) WriteReadyFile(host.TopLevelHwnd);
                    Interlocked.Increment(ref _presented);
                    break;
                case WindowsNativeV1.FrameTerminalKind.Superseded:
                    Interlocked.Increment(ref _superseded);
                    break;
                case WindowsNativeV1.FrameTerminalKind.Failed:
                    Interlocked.Increment(ref _failed);
                    break;
                default:
                    throw new InvalidDataException($"Unknown native terminal {terminal.TerminalKind}.");
            }
        }

        private static bool ShouldDrawValidationFrameMarker() =>
            string.Equals(
                Environment.GetEnvironmentVariable(
                    "DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FRAME_MARKER"),
                "1", StringComparison.Ordinal);

        private static void DrawValidationFrameMarker(
            SKCanvas canvas, int width, int height, double scale, long resizeGeneration)
        {
            if (resizeGeneration <= 0 || width <= 0 || height <= 0) return;
            var markerScale = Math.Max(1d, scale);
            // Keep the diagnostic stripe below half of the minimum client
            // width. The visual oracle samples the app bar by row; the old
            // 7-DIP cells covered most of a compact window and split that
            // otherwise continuous fill into two false segments.
            var bitSize = Math.Max(4, checked((int)Math.Round(4 * markerScale)));
            var bitGap = Math.Max(1, checked((int)Math.Round(markerScale)));
            const int preambleBitCount = 4;
            const int preamble = 0b1101;
            const int generationBitCount = 12;
            const int checksumBitCount = 8;
            const int bitCount = preambleBitCount + generationBitCount + checksumBitCount;
            var stripWidth = bitCount * bitSize + (bitCount - 1) * bitGap;
            var horizontalMargin = Math.Max(4, checked((int)Math.Round(4 * markerScale)));
            var verticalMargin = Math.Max(1, checked((int)Math.Round(5 * markerScale)));
            var corner = Environment.GetEnvironmentVariable(
                "DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_FRAME_MARKER_CORNER") ?? "TopRight";
            var left = corner.EndsWith("Left", StringComparison.OrdinalIgnoreCase);
            var bottom = corner.StartsWith("Bottom", StringComparison.OrdinalIgnoreCase);
            var startX = left ? horizontalMargin : width - stripWidth - horizontalMargin;
            var startY = bottom ? height - bitSize - verticalMargin : verticalMargin;
            if (startX < 0 || startY + bitSize > height) return;
            var binary = checked((int)(resizeGeneration & 0xFFF));
            var gray = binary ^ (binary >> 1);
            var checksum = ((gray * 0x9E37) ^ (gray >> 4) ^ 0xA5) & 0xFF;
            var payload = preamble |
                (gray << preambleBitCount) |
                (checksum << (preambleBitCount + generationBitCount));
            using var paint = new SKPaint { IsAntialias = false };
            for (var bit = 0; bit < bitCount; bit++)
            {
                paint.Color = (payload & (1 << bit)) != 0 ? SKColors.White : SKColors.Black;
                canvas.DrawRect(
                    startX + bit * (bitSize + bitGap), startY,
                    bitSize, bitSize, paint);
            }
        }

        internal void ApplyPointer(in WindowsNativeV1.Pointer pointer)
        {
            RecordThread(ref _inputThreadId, "input");
            (Host ?? throw new InvalidOperationException("Pointer arrived before host-ready."))
                .ApplyPointer(in pointer);
        }

        internal void ApplyKey(in WindowsNativeV1.Key key, string character)
        {
            RecordThread(ref _inputThreadId, "input");
            (Host ?? throw new InvalidOperationException("Key arrived before host-ready."))
                .ApplyKey(in key, character);
        }

        internal void ApplyFocus(bool focused, long timestampQpc)
        {
            RecordThread(ref _inputThreadId, "input");
            (Host ?? throw new InvalidOperationException("Focus arrived before host-ready."))
                .ApplyFocus(focused, timestampQpc);
        }

        internal void ApplyPlatformBrightness(uint brightness)
        {
            (Host ?? throw new InvalidOperationException("Platform brightness arrived before host-ready."))
                .ApplyPlatformBrightness(brightness);
            if (Presenter is WindowsManagedAcrylicCompositionPresenter acrylic)
                acrylic.ApplySystemBrightness((Brightness)brightness);
        }

        internal void CaptureFatal(Exception exception)
        {
            lock (_gate) _fatal ??= exception;
            try { Host?.Close(); } catch { }
        }

        internal void ThrowIfFatal()
        {
            lock (_gate)
            {
                if (_fatal is { } fatal)
                    throw new InvalidOperationException("The managed product callback entered a fatal state.", fatal);
            }
        }

        internal void ValidateTerminalCoverage()
        {
            var rendered = Interlocked.Read(ref _renderCallbacks);
            var terminals = Interlocked.Read(ref _presented) + Interlocked.Read(ref _superseded) + Interlocked.Read(ref _failed);
            LastRunDiagnostics = CreateDiagnostics();
            if (rendered == 0 || terminals < rendered)
                throw new InvalidOperationException($"Product terminal coverage differs: render={rendered}, terminal={terminals}.");
            var resize = Host?.ResizeSnapshot;
            if (resize is null || resize.UnterminatedCount != 0 || resize.DuplicateTerminalCount != 0)
                throw new InvalidOperationException(
                    $"Product resize coordinator did not drain exactly once: " +
                    $"accepted={resize?.AcceptedCount}, presented={resize?.PresentedCount}, " +
                    $"superseded={resize?.SupersededCount}, failed={resize?.FailedCount}, " +
                    $"unterminated={resize?.UnterminatedCount}, duplicate={resize?.DuplicateTerminalCount}.");
            if (!_visibleAfterExactPresent)
                throw new InvalidOperationException(
                    $"The product HWND was not visible after an exact managed present: " +
                    $"presented={_presented}, superseded={_superseded}, failed={_failed}.");
        }

        internal void WriteDiagnostics()
        {
            var summary = CreateDiagnostics();
            Console.Error.WriteLine($"doroti.windows.summary={JsonSerializer.Serialize(summary)}");
            var reportPath = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_REPORT");
            if (!string.IsNullOrWhiteSpace(reportPath))
            {
                var fullPath = System.IO.Path.GetFullPath(reportPath);
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath)!);
                File.WriteAllText(fullPath, JsonSerializer.Serialize(DiagnosticDocument(), new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));
            }
        }

        private void WriteReadyFile(nint topLevelHwnd)
        {
            if (_readyFileWritten) return;
            var readyPath = Environment.GetEnvironmentVariable("DOROTI_WINDOWS_EXPERIMENTAL_ACRYLIC_READY_FILE");
            if (string.IsNullOrWhiteSpace(readyPath)) return;
            var fullPath = System.IO.Path.GetFullPath(readyPath);
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, JsonSerializer.Serialize(new
            {
                schemaVersion = "doroti.windows.experimental-acrylic-ready/v1",
                hwnd = topLevelHwnd.ToInt64(),
                processId = Environment.ProcessId,
                title = _configuration.title,
                requestedMode = RequestedMode,
                effectiveMode = EffectiveMode,
            }, new JsonSerializerOptions { WriteIndented = true }));
            _readyFileWritten = true;
        }

        private WindowsProductRunDiagnostics CreateDiagnostics()
        {
            var resize = Host?.ResizeSnapshot ?? throw new InvalidOperationException("Product host diagnostics are unavailable.");
            var renderer = Renderer?.Diagnostics ?? throw new InvalidOperationException("Product renderer diagnostics are unavailable.");
            return new(
                Presenter.BackendName, Presenter.DiagnosticCoverage,
                _releasedAdapterDescription ?? Presenter.AdapterDescription,
                RequestedMode, EffectiveMode, FallbackReason,
                _releasedAcrylicSnapshot ?? (Presenter as WindowsManagedAcrylicCompositionPresenter)?.Snapshot(),
                _platformThreadId, _rasterThreadId, _inputThreadId,
                _renderCallbacks, _presented, _superseded, _failed,
                _visibleAfterExactPresent,
                resize.AcceptedCount, resize.PresentedCount, resize.SupersededCount,
                resize.FailedCount, resize.UnterminatedCount, resize.DuplicateTerminalCount,
                Presenter.DeviceGeneration, Presenter.ResizeBuffersCount, Presenter.PresentCount,
                Presenter.GpuSubmitCount, Presenter.GpuCopyCount,
                Presenter.InitializationDebugErrorCount, Presenter.OperationalDebugErrorCount,
                renderer.Submitted, renderer.Presented, renderer.Replayed,
                _staleInputPresentPrevented,
                LastNativeProvenance ?? throw new InvalidOperationException("Native provenance is unavailable."));
        }

        private static void RecordThread(ref uint owner, string role)
        {
            var current = GetCurrentThreadId();
            var existing = Volatile.Read(ref owner);
            if (existing == 0)
            {
                existing = Interlocked.CompareExchange(ref owner, current, 0);
                if (existing == 0) return;
            }
            if (existing != current)
                throw new InvalidOperationException(
                    $"The managed {role} callback moved from thread {existing} to {current}.");
        }

        internal object DiagnosticDocument()
        {
            var diagnostics = CreateDiagnostics();
            return new
            {
                schemaVersion = EffectiveMode == "experimentalAcrylic"
                    ? "doroti.windows.experimental-acrylic-product/v1"
                    : "doroti.windows.hwnd-exact-cpp-product/v1",
                ownership = new
                {
                    cpp = "HWND/task-pump",
                    managed = $"{Presenter.BackendName}/Skia/surface/present",
                    visibleOwner = EffectiveMode == "experimentalAcrylic" ? "ContentIsland" : "child HWND",
                    abiGpuPointerCount = 0,
                },
                mode = new { requested = RequestedMode, effective = EffectiveMode, fallbackReason = FallbackReason },
                acrylic = diagnostics.Acrylic,
                frames = diagnostics,
                resize = Host?.ResizeSnapshot,
                renderer = Renderer?.Diagnostics,
            };
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (View is { } view)
            {
                _session.DetachView(view);
                view.Dispose();
                View = null;
            }
            Renderer?.Dispose();
            Renderer = null;
            Presenter.Dispose();
            Host?.Dispose();
            Host = null;
            _capabilities?.Dispose();
            _capabilities = null;
        }

        private static WindowsManagedHwndPresenterBase CreatePresenter(bool diagnosticsEnabled) =>
            Environment.GetEnvironmentVariable("DOROTI_WINDOWS_PRESENTER")?.Trim() switch
            {
                null or "" => new WindowsManagedAngleEglPresenter(diagnosticsEnabled),
                var value when value.Equals("AngleD3D11", StringComparison.OrdinalIgnoreCase) =>
                    new WindowsManagedAngleEglPresenter(diagnosticsEnabled),
                var value when value.Equals("D3D12", StringComparison.OrdinalIgnoreCase) =>
                    CreateDiagnosticPresenter(diagnosticsEnabled),
                var value => throw new InvalidOperationException(
                    $"Unsupported managed Windows presenter '{value}'. Expected AngleD3D11 or D3D12."),
            };

        private static WindowsManagedHwndPresenterBase CreateDiagnosticPresenter(bool diagnosticsEnabled)
        {
            const string assemblyName = "Doroti.Host.WindowsAppSdk.Diagnostics";
            const string typeName = "Doroti.Host.WindowsAppSdk.WindowsManagedHwndPresenter";
            try
            {
                var assembly = Assembly.Load(assemblyName);
                var type = assembly.GetType(typeName, throwOnError: true)!;
                return (WindowsManagedHwndPresenterBase)(Activator.CreateInstance(
                    type,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    args: [diagnosticsEnabled],
                    culture: null) ?? throw new InvalidOperationException($"{typeName} did not produce an instance."));
            }
            catch (Exception exception) when (exception is FileNotFoundException or FileLoadException or TypeLoadException)
            {
                throw new InvalidOperationException(
                    "D3D12 is a separate diagnostic artifact. Deploy Doroti.Host.WindowsAppSdk.Diagnostics explicitly before selecting DOROTI_WINDOWS_PRESENTER=D3D12.",
                    exception);
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnHostReady(nint context, WindowsNativeV1.Host* host) =>
        GuardVoid(context, state =>
        {
            if (host is null) throw new InvalidDataException("Native host-ready supplied null.");
            state.SetHost(in *host);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnMetrics(nint context, WindowsNativeV1.Metrics* metrics) =>
        GuardVoid(context, state =>
        {
            if (metrics is null) throw new InvalidDataException("Native metrics supplied null.");
            state.ApplyMetrics(in *metrics);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnCompositionResize(
        nint context, uint width, uint height, double scale) =>
        GuardVoid(context, state => state.ResizeComposition(width, height, scale));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static uint OnRender(nint context, WindowsNativeV1.FrameRequest* request)
    {
        try
        {
            if (request is null) throw new InvalidDataException("Native render supplied null.");
            return GetState(context).Render(in *request);
        }
        catch (Exception exception)
        {
            TryCaptureFatal(context, exception);
            return (uint)WindowsNativeV1.FrameTerminalKind.Failed;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnFrameTerminal(nint context, WindowsNativeV1.FrameTerminal* terminal) =>
        GuardVoid(context, state =>
        {
            if (terminal is null) throw new InvalidDataException("Native terminal supplied null.");
            state.CompleteTerminal(in *terminal);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnLog(nint context, uint level, WindowsNativeV1.Utf8 message) =>
        GuardVoid(context, _ => Console.Error.WriteLine($"doroti.windows.native[{level}]={Decode(message)}"));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnPointer(nint context, WindowsNativeV1.Pointer* pointer) =>
        GuardVoid(context, state =>
        {
            if (pointer is null) throw new InvalidDataException("Native pointer supplied null.");
            state.ApplyPointer(in *pointer);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnKey(nint context, WindowsNativeV1.Key* key) =>
        GuardVoid(context, state =>
        {
            if (key is null) throw new InvalidDataException("Native key supplied null.");
            state.ApplyKey(in *key, Decode(key->Character));
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnFocus(nint context, ulong viewId, uint focused, long timestampQpc) =>
        GuardVoid(context, state =>
        {
            if (viewId != 1) throw new InvalidDataException("Native focus view id differs.");
            state.ApplyFocus(focused != 0, timestampQpc);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnClipboard(nint context, ulong requestId, WindowsNativeV1.Utf8 text) =>
        GuardVoid(context, state =>
            (state.Host ?? throw new InvalidOperationException("Clipboard arrived before host-ready."))
                .CompleteClipboard(requestId, Decode(text)));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnTextEditing(nint context, WindowsNativeV1.TextState* state) =>
        GuardVoid(context, managed =>
        {
            if (state is null || state->AbiVersion != WindowsNativeV1.AbiVersion ||
                state->StructSize < sizeof(WindowsNativeV1.TextState))
                throw new InvalidDataException("Native text editing supplied an invalid state.");
            (managed.Host ?? throw new InvalidOperationException("Text editing arrived before host-ready."))
                .ApplyTextEditing(Decode(state->Text), state->SelectionBase, state->SelectionExtent,
                    state->ComposingBase, state->ComposingExtent);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnTextAction(nint context, uint action) =>
        GuardVoid(context, managed =>
            (managed.Host ?? throw new InvalidOperationException("Text action arrived before host-ready."))
                .ApplyTextAction(action));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnSemanticsAction(nint context, long nodeId, long action,
        WindowsNativeV1.Utf8 arguments) =>
        GuardVoid(context, managed =>
        {
            (managed.Host ?? throw new InvalidOperationException("Semantics action arrived before host-ready."))
                .ApplySemanticsAction(nodeId, action, Decode(arguments));
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnLifecycle(nint context, ulong viewId, uint state, long timestampQpc) =>
        GuardVoid(context, managed =>
        {
            _ = timestampQpc;
            if (viewId != 1) throw new InvalidDataException("Native lifecycle view id differs.");
            (managed.Host ?? throw new InvalidOperationException("Lifecycle arrived before host-ready."))
                .ApplyLifecycle(state);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnPlatformBrightness(nint context, ulong viewId, uint brightness) =>
        GuardVoid(context, managed =>
        {
            if (viewId != 1) throw new InvalidDataException("Native platform-brightness view id differs.");
            managed.ApplyPlatformBrightness(brightness);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnPlatformResourcesShutdown(nint context) =>
        GuardVoid(context, static managed => managed.ReleasePlatformResources());

    private static void GuardVoid(nint context, Action<WindowsManagedState> callback)
    {
        try { callback(GetState(context)); }
        catch (Exception exception) { TryCaptureFatal(context, exception); }
    }

    private static void TryCaptureFatal(nint context, Exception exception)
    {
        try { GetState(context).CaptureFatal(exception); }
        catch { }
    }

    private static WindowsManagedState GetState(nint context) =>
        (WindowsManagedState)(GCHandle.FromIntPtr(context).Target ??
            throw new InvalidOperationException("The Windows managed callback context is unavailable."));

    private static WindowsNativeV1.Utf8 Utf8(byte* data, int length) => new()
    {
        AbiVersion = WindowsNativeV1.AbiVersion,
        StructSize = checked((uint)sizeof(WindowsNativeV1.Utf8)),
        Data = (nint)data,
        ByteLength = checked((ulong)length),
    };

    private static string Decode(WindowsNativeV1.Utf8 value)
    {
        if (value.Data == 0 || value.ByteLength == 0) return string.Empty;
        if (value.ByteLength > int.MaxValue) throw new InvalidDataException("Native UTF-8 payload is too large.");
        return Encoding.UTF8.GetString(new ReadOnlySpan<byte>((void*)value.Data, checked((int)value.ByteLength)));
    }

    private static uint ToDimension(double value)
    {
        if (!double.IsFinite(value) || value <= 0 || value > uint.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(value));
        return checked((uint)Math.Round(value));
    }

    private static bool ShouldWriteDiagnostics() =>
        string.Equals(Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_DIAGNOSTICS"), "1", StringComparison.Ordinal) ||
        !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_SMOKE_MS"));

    [LibraryImport("combase.dll")]
    private static partial int RoInitialize(uint initializationType);

    [LibraryImport("combase.dll")]
    private static partial void RoUninitialize();

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool IsWindowVisible(nint window);

    [LibraryImport("user32.dll")]
    private static partial uint GetDpiForSystem();

    [LibraryImport("kernel32.dll")]
    private static partial uint GetCurrentThreadId();
}

internal sealed record WindowsProductRunDiagnostics(
    string PresenterBackend,
    string PresenterDiagnosticCoverage,
    string AdapterDescription,
    string RequestedMode,
    string EffectiveMode,
    string? FallbackReason,
    AcrylicPresenterSnapshot? Acrylic,
    uint PlatformThreadId,
    uint RasterThreadId,
    uint InputThreadId,
    long RenderCallbacks,
    long PresentedTerminals,
    long SupersededTerminals,
    long FailedTerminals,
    bool VisibleAfterExactPresent,
    long AcceptedResizeGenerations,
    long PresentedResizeGenerations,
    long SupersededResizeGenerations,
    long FailedResizeGenerations,
    int UnterminatedResizeGenerations,
    long DuplicateResizeTerminals,
    ulong DeviceGenerations,
    ulong ResizeBuffers,
    ulong Presents,
    ulong GpuSubmits,
    ulong GpuCopies,
    ulong InitializationDebugErrors,
    ulong OperationalDebugErrors,
    long RendererSubmitted,
    long RendererPresented,
    long RendererReplayed,
    long StaleInputPresentsPrevented,
    NativeHostProvenance NativeProvenance);
