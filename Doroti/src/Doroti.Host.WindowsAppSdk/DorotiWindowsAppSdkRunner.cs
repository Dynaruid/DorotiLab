using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

public static unsafe partial class DorotiWindowsAppSdkRunner
{
    internal static WindowsProductRunDiagnostics? LastRunDiagnostics { get; private set; }

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

        WindowsNativeV1.ValidateLayout();
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
        var handle = GCHandle.Alloc(state);
        try
        {
            session.Start(deferFrameworkBootstrap: true);
            var applicationId = Encoding.UTF8.GetBytes(application.Manifest.ApplicationId);
            var title = Encoding.UTF8.GetBytes(descriptor.ViewConfiguration.title);
            fixed (byte* applicationIdData = applicationId)
            fixed (byte* titleData = title)
            {
                var configuration = new WindowsNativeV1.Configuration
                {
                    AbiVersion = WindowsNativeV1.AbiVersion,
                    StructSize = checked((uint)sizeof(WindowsNativeV1.Configuration)),
                    ApplicationId = Utf8(applicationIdData, applicationId.Length),
                    Title = Utf8(titleData, title.Length),
                    InitialWidthPx = ToDimension(descriptor.ViewConfiguration.logicalSize.width),
                    InitialHeightPx = ToDimension(descriptor.ViewConfiguration.logicalSize.height),
                    NCmdShow = 1,
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
        private bool _visibleAfterExactPresent;
        private bool _disposed;

        internal WindowsManagedState(
            DorotiHostSession session,
            DorotiApplicationBoundary application,
            DorotiViewConfiguration configuration)
        {
            _session = session;
            _application = application;
            _configuration = configuration;
            Presenter = CreatePresenter(ShouldWriteDiagnostics());
        }

        internal WindowsManagedProductHost? Host { get; private set; }
        internal WindowsManagedHwndPresenterBase Presenter { get; }
        internal SkiaSceneRenderer? Renderer { get; private set; }
        internal DorotiView? View { get; private set; }

        internal void SetHost(in WindowsNativeV1.Host native)
        {
            var host = new WindowsManagedProductHost(in native,
                checked((int)_configuration.logicalSize.width),
                checked((int)_configuration.logicalSize.height));
            var presenterSlug = Presenter.BackendName.ToLowerInvariant().Replace('/', '-');
            var target = $"win-x64/windowsappsdk-2.4/cpp-child-hwnd/managed-{presenterSlug}-skia";
            var renderer = new SkiaSceneRenderer(
                1, host, _configuration.backgroundColor, _configuration.darkBackgroundColor,
                target, Presenter.RuntimeEffectsBackend,
                $"windowsappsdk-2.4-hwnd-{presenterSlug}-skia-managed");
            var messages = new WindowsAppSdkPlatformMessageCapability();
            var capabilities = new DorotiViewCapabilities(target)
                .Register<IViewHostCapability>(DorotiCapabilityIds.WindowLifecycle, host)
                .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
                .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
                .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
                .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, host)
                .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host)
                .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
                .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
                .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, renderer);
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

        internal void MarkNativeStopped() => Host?.MarkNativeStopped();

        internal uint Render(in WindowsNativeV1.FrameRequest request)
        {
            var host = Host ?? throw new InvalidOperationException("Render arrived before host-ready.");
            var renderer = Renderer ?? throw new InvalidOperationException("Renderer is unavailable.");
            host.BeginFrame(in request);
            var width = checked((int)request.WidthPx);
            var height = checked((int)request.HeightPx);
            var causalFrameId = checked((long)request.CausalFrameId);
            Presenter.EnsureTarget(host.ChildHwnd, width, height);
            Presenter.SealInitializationDebugBaseline();
            var result = Presenter.RenderAndPresent(
                surface => renderer.Paint(surface, width, height, host.ResizeTarget, causalFrameId),
                static value => value.ShouldPresent);
            Presenter.CaptureOperationalDebugMessages();
            if (Presenter.OperationalDebugErrorCount != 0)
                throw new InvalidOperationException(
                    $"Managed {Presenter.BackendName} presentation emitted " +
                    $"{Presenter.OperationalDebugErrorCount} operational GPU errors.");
            Interlocked.Increment(ref _renderCallbacks);
            if (!result.ShouldPresent) return (uint)WindowsNativeV1.FrameTerminalKind.Superseded;
            if (result.Completion is { } completion)
            {
                lock (_gate) _paintCompletions.Add(request.CausalFrameId, completion);
            }
            return (uint)WindowsNativeV1.FrameTerminalKind.Presented;
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
            if (rendered == 0 || rendered != terminals)
                throw new InvalidOperationException($"Product terminal coverage differs: render={rendered}, terminal={terminals}.");
            if (Host?.ResizeSnapshot is not { UnterminatedCount: 0, DuplicateTerminalCount: 0 })
                throw new InvalidOperationException("Product resize coordinator did not drain exactly once.");
            if (!_visibleAfterExactPresent)
                throw new InvalidOperationException("The product HWND was not visible after an exact managed present.");
            LastRunDiagnostics = CreateDiagnostics();
        }

        internal void WriteDiagnostics()
        {
            Console.Error.WriteLine($"doroti.windows.summary={JsonSerializer.Serialize(CreateDiagnostics())}");
        }

        private WindowsProductRunDiagnostics CreateDiagnostics()
        {
            var resize = Host?.ResizeSnapshot ?? throw new InvalidOperationException("Product host diagnostics are unavailable.");
            var renderer = Renderer?.Diagnostics ?? throw new InvalidOperationException("Product renderer diagnostics are unavailable.");
            return new(
                Presenter.BackendName, Presenter.DiagnosticCoverage, Presenter.AdapterDescription,
                _renderCallbacks, _presented, _superseded, _failed,
                _visibleAfterExactPresent,
                resize.AcceptedCount, resize.PresentedCount, resize.SupersededCount,
                resize.FailedCount, resize.UnterminatedCount, resize.DuplicateTerminalCount,
                Presenter.DeviceGeneration, Presenter.ResizeBuffersCount, Presenter.PresentCount,
                Presenter.GpuSubmitCount, Presenter.GpuCopyCount,
                Presenter.InitializationDebugErrorCount, Presenter.OperationalDebugErrorCount,
                renderer.Submitted, renderer.Presented, renderer.Replayed);
        }

        internal object DiagnosticDocument()
        {
            var diagnostics = CreateDiagnostics();
            return new
            {
                schemaVersion = "doroti.windows.hwnd-exact-cpp-product/v1",
                ownership = new { cpp = "HWND/task-pump", managed = $"{Presenter.BackendName}/Skia/surface/present", abiGpuPointerCount = 0 },
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
                    new WindowsManagedHwndPresenter(diagnosticsEnabled),
                var value => throw new InvalidOperationException(
                    $"Unsupported managed Windows presenter '{value}'. Expected D3D12 or AngleD3D11."),
            };
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
            (state.Host ?? throw new InvalidOperationException("Pointer arrived before host-ready."))
                .ApplyPointer(in *pointer);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnKey(nint context, WindowsNativeV1.Key* key) =>
        GuardVoid(context, state =>
        {
            if (key is null) throw new InvalidDataException("Native key supplied null.");
            (state.Host ?? throw new InvalidOperationException("Key arrived before host-ready."))
                .ApplyKey(in *key, Decode(key->Character));
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnFocus(nint context, ulong viewId, uint focused, long timestampQpc) =>
        GuardVoid(context, state =>
        {
            if (viewId != 1) throw new InvalidDataException("Native focus view id differs.");
            (state.Host ?? throw new InvalidOperationException("Focus arrived before host-ready."))
                .ApplyFocus(focused != 0, timestampQpc);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    private static void OnClipboard(nint context, ulong requestId, WindowsNativeV1.Utf8 text) =>
        GuardVoid(context, state =>
            (state.Host ?? throw new InvalidOperationException("Clipboard arrived before host-ready."))
                .CompleteClipboard(requestId, Decode(text)));

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
}

internal sealed record WindowsProductRunDiagnostics(
    string PresenterBackend,
    string PresenterDiagnosticCoverage,
    string AdapterDescription,
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
    long RendererReplayed);
