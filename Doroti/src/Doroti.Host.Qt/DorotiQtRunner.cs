using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Skia.Rendering;
using Doroti.Skia.RuntimeEffects;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Host.Qt;

/// <summary>
/// Runs the managed-owned Linux process against the versioned doroti.qt-host/v2 C ABI.
/// Native callbacks execute on Qt's GUI thread; host API calls are safe from any managed
/// thread because the native side queues all QObject access back to that thread.
/// </summary>
public static unsafe partial class DorotiQtRunner
{
    private const string NativeLibraryName = "doroti_qt_host";

    public static int Run(DorotiApplicationDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        if (!OperatingSystem.IsLinux())
        {
            throw new PlatformNotSupportedException(
                "Doroti.Host.Qt can only launch on Linux. Build/publish graph validation may run on another host.");
        }

        QtNativeV2.ValidateLayout();
        using var application = DorotiApplicationBoundary.Load(
            descriptor.ManifestAssembly,
            descriptor.ApplicationAssembly,
            descriptor.LaunchContext.RuntimeIdentifier);
        using var session = new DorotiHostSession(descriptor.EntrypointFactory());
        using var state = new QtManagedState(session, application, descriptor.ViewConfiguration);
        var stateHandle = GCHandle.Alloc(state);
        try
        {
            session.Start(deferFrameworkBootstrap: true);
            var titleBytes = Encoding.UTF8.GetBytes(descriptor.ViewConfiguration.title);
            fixed (byte* title = titleBytes)
            {
                var configuration = new QtNativeV2.Configuration(
                    new QtNativeV2.Utf8(title, checked((ulong)titleBytes.Length)),
                    checked((int)descriptor.ViewConfiguration.logicalSize.width),
                    checked((int)descriptor.ViewConfiguration.logicalSize.height),
                    (uint)(descriptor.ViewConfiguration.backdrop?.mode ?? WindowBackdropMode.system),
                    (uint)(descriptor.ViewConfiguration.backdrop?.fallback ?? WindowBackdropFallback.transparent));
                var callbacks = new QtNativeV2.Callbacks(GCHandle.ToIntPtr(stateHandle));
                var exitCode = NativeMethods.Run(in configuration, in callbacks);
                if (exitCode is >= 64 and <= 70)
                    return exitCode;
                state.ThrowIfFatal();
                state.ValidateTerminalCoverage();
                if (ShouldWriteSummary()) state.WriteDiagnostics();
                return exitCode;
            }
        }
        finally
        {
            stateHandle.Free();
        }
    }

    private static bool ShouldWriteSummary()
    {
#if DEBUG
        return true;
#else
        return string.Equals(
            Environment.GetEnvironmentVariable("DOROTI_QT_DIAGNOSTICS"),
            "1",
            StringComparison.Ordinal);
#endif
    }

    internal sealed class QtManagedState : IDisposable
    {
        private readonly object _gate = new();
        private readonly HashSet<ulong> _terminalTokens = [];
        private readonly Dictionary<ulong, SkiaPaintCompletion?> _paintCompletions = [];
        private readonly Dictionary<string, string> _nativeDiagnostics = new(StringComparer.Ordinal);
        private readonly DorotiApplicationBoundary _application;
        private readonly DorotiViewConfiguration _configuration;
        private Exception? _fatal;
        private QtNativeV2.HostApi _hostApi;
        private nint _viewHandle;
        private long _rasterized;
        private long _presented;
        private long _replayed;
        private long _superseded;
        private long _failed;
        internal ulong RendererContextIdentity { get; set; }
        private bool _disposed;

        private readonly GRGlGetProcedureAddressDelegate _glResolver;

        internal QtManagedState(DorotiHostSession session, DorotiApplicationBoundary application,
            DorotiViewConfiguration configuration)
        {
            Session = session;
            _application = application;
            _configuration = configuration;
            _glResolver = ResolveGlProcedure;
            Surface = new(_glResolver);
        }

        internal DorotiHostSession Session { get; }
        internal QtSkiaSurface Surface { get; }
        internal QtHostAdapter? Host { get; private set; }
        internal SkiaSceneRenderer? Renderer { get; private set; }
        internal DorotiView? View { get; private set; }

        internal void SetHost(nint viewHandle, in QtNativeV2.HostApi hostApi)
        {
            if (hostApi.AbiVersion != QtNativeV2.AbiVersion ||
                hostApi.StructSize < (uint)sizeof(QtNativeV2.HostApi) ||
                (hostApi.FeatureBits & QtNativeV2.RequiredFeatures) != QtNativeV2.RequiredFeatures ||
                hostApi.RequestFrame == null || hostApi.RequestClose == null ||
                hostApi.GetGlProcAddress == null || hostApi.Resize == null ||
                hostApi.SetClipboardText == null || hostApi.RequestClipboardText == null ||
                hostApi.SetCursor == null || hostApi.SetTextClient == null ||
                hostApi.UpdateTextState == null || hostApi.SetCaretRect == null ||
                hostApi.ClearTextClient == null || hostApi.UpdateSemantics == null ||
                hostApi.ClearSemantics == null)
            {
                throw new InvalidDataException("The native Qt host API does not satisfy doroti.qt-host/v2.");
            }
            lock (_gate)
            {
                _viewHandle = viewHandle;
                _hostApi = hostApi;
            }
            var host = new QtHostAdapter(viewHandle, hostApi,
                checked((int)_configuration.logicalSize.width),
                checked((int)_configuration.logicalSize.height));
            var renderer = new SkiaSceneRenderer(1, host,
                _configuration.backgroundColor, _configuration.darkBackgroundColor,
                "linux-x64/qt6-opengl/skia-gl", DorotiSkiaRuntimeEffects.QtGpuBackend,
                DorotiSkiaRuntimeEffects.QtGpuBackend);
            var messages = new QtPlatformMessageCapability();
            var capabilities = new DorotiViewCapabilities("linux-x64/qt6-opengl/skia-gl")
                .Register<IViewHostCapability>(DorotiCapabilityIds.WindowLifecycle, host)
                .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
                .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
                .Register<IInputHostCapability>(DorotiCapabilityIds.InputEvents, host)
                .Register<ITextInputHostCapability>(DorotiCapabilityIds.TextInput, host)
                .Register<IPlatformServicesHostCapability>(DorotiCapabilityIds.PlatformServices, host)
                .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, host)
                .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer)
                .Register<IParagraphHostCapability>(DorotiCapabilityIds.GraphicsText, renderer)
                .Register<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, renderer)
                .Register<ISemanticsHostCapability>(DorotiCapabilityIds.AccessibilitySemantics, renderer);
            _application.Configure(capabilities, messages);
            DorotiView? view = null;
            try
            {
                using var dispatcherScope = Session.dispatcher.EnterScope();
                view = Session.dispatcher.RegisterView(1, capabilities);
                renderer.AttachFrameworkTrace(view.FrameTrace);
                Session.AttachView(view);
                Host = host;
                Renderer = renderer;
                View = view;
                renderer.AttachSurface(host.RequestInvalidate);
                host.Show();
            }
            catch
            {
                if (view is null) capabilities.Dispose();
                else view.Dispose();
                throw;
            }
        }

        private nint ResolveGlProcedure(string name)
        {
            var length = Encoding.UTF8.GetByteCount(name);
            Span<byte> bytes = length <= 512 ? stackalloc byte[length] : new byte[length];
            Encoding.UTF8.GetBytes(name.AsSpan(), bytes);
            fixed (byte* data = bytes)
            {
                QtNativeV2.HostApi hostApi;
                nint viewHandle;
                lock (_gate)
                {
                    hostApi = _hostApi;
                    viewHandle = _viewHandle;
                }
                return hostApi.GetGlProcAddress(viewHandle,
                    new QtNativeV2.Utf8(data, checked((ulong)bytes.Length)));
            }
        }

        internal void RecordRasterized(ulong token, SkiaPaintCompletion? completion)
        {
            lock (_gate) _paintCompletions[token] = completion;
            Interlocked.Increment(ref _rasterized);
        }

        internal void RecordTerminal(ulong token, QtNativeV2.TerminalState terminal)
        {
            lock (_gate)
            {
                if (!_terminalTokens.Add(token))
                    throw new InvalidDataException($"Qt frame token {token} received more than one terminal ACK.");
                if (_paintCompletions.Remove(token, out var completion) && completion is { } painted)
                {
                    Renderer?.CompletePaint(painted);
                    terminal = painted.IsNewFrame
                        ? QtNativeV2.TerminalState.Presented
                        : QtNativeV2.TerminalState.Replayed;
                }
                switch (terminal)
                {
                    case QtNativeV2.TerminalState.Presented: _presented++; break;
                    case QtNativeV2.TerminalState.Replayed: _replayed++; break;
                    case QtNativeV2.TerminalState.Superseded: _superseded++; break;
                    case QtNativeV2.TerminalState.Failed: _failed++; break;
                    default: throw new InvalidDataException($"Unknown Qt terminal frame state {(uint)terminal}.");
                }
            }
        }

        internal void CaptureFatal(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);
            lock (_gate) _fatal ??= exception;
            Console.Error.WriteLine($"doroti.qt managed.fatal={exception}");
            RequestClose();
        }

        internal void RecordDiagnostic(string key, string value)
        {
            if (key == "qpa") Surface.SetQpaPlatform(value);
            lock (_gate) _nativeDiagnostics[key] = value;
            Console.Error.WriteLine($"doroti.qt {key}={value}");
        }

        internal void WriteDiagnostics()
        {
            object snapshot;
            lock (_gate)
            {
                var renderer = Renderer?.Diagnostics;
                snapshot = new
                {
                    schemaVersion = "doroti.linux-qt-diagnostics/v1",
                    native = _nativeDiagnostics,
                    metrics = Host is null ? null : new
                    {
                        width = Host.Metrics.physicalSize.width,
                        height = Host.Metrics.physicalSize.height,
                        dpr = Host.Metrics.devicePixelRatio,
                        Host.Metrics.generation,
                        Host.Metrics.surfaceGeneration,
                        lifecycle = Host.Metrics.lifecycleState.ToString(),
                    },
                    frames = new
                    {
                        rasterized = _rasterized, presented = _presented, replayed = _replayed,
                        superseded = _superseded, failed = _failed,
                        rendererSubmitted = renderer?.Submitted,
                        rendererPending = renderer?.PendingScene,
                    },
                    inputCount = Host?.InputSequence ?? 0,
                    semanticsNodes = _nativeDiagnostics.GetValueOrDefault("semantics.nodes", "0"),
                    renderer = renderer?.Backend,
                    softwareFallback = false,
                    fullFrameCpuCopies = 0,
                    synchronousGuiWaits = 0,
                };
            }
            Console.Error.WriteLine($"doroti.qt.summary={JsonSerializer.Serialize(snapshot)}");
        }

        internal void RequestClose()
        {
            QtNativeV2.HostApi hostApi;
            nint viewHandle;
            lock (_gate)
            {
                hostApi = _hostApi;
                viewHandle = _viewHandle;
            }
            if (viewHandle != 0 && hostApi.RequestClose != null) hostApi.RequestClose(viewHandle);
        }

        internal void ThrowIfFatal()
        {
            Exception? fatal;
            lock (_gate) fatal = _fatal;
            if (fatal is not null) throw new InvalidOperationException("The Qt managed callback entered a fatal state.", fatal);
        }

        internal void ValidateTerminalCoverage()
        {
            lock (_gate)
            {
                if (_failed != 0)
                    throw new InvalidOperationException($"Qt reported {_failed} failed frame terminal ACKs.");
                if (_rasterized != _presented + _replayed)
                    throw new InvalidOperationException(
                        $"Qt frame ACK mismatch: rasterized={_rasterized}, presented={_presented}, replayed={_replayed}, superseded={_superseded}.");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (View is { } view)
            {
                Session.DetachView(view);
                view.Dispose();
                View = null;
            }
            Surface.Dispose();
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static int OnViewCreated(nint context, nint viewHandle, QtNativeV2.HostApi* hostApi) =>
        Guard(context, state =>
        {
            if (hostApi == null) throw new InvalidDataException("Qt supplied a null host API table.");
            state.SetHost(viewHandle, in *hostApi);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static int OnRender(nint context, nint viewHandle, QtNativeV2.Surface* surface, ulong frameToken) =>
        Guard(context, state =>
        {
            _ = (viewHandle, frameToken);
            if (surface == null) throw new InvalidDataException("Qt supplied a null surface descriptor.");
            if (state.Host is null || state.Renderer is null)
                throw new InvalidOperationException("Qt render arrived before the Doroti view was attached.");
            if (state.RendererContextIdentity == 0)
                state.RendererContextIdentity = surface->ContextIdentity;
            else if (state.RendererContextIdentity != surface->ContextIdentity)
            {
                state.Renderer.AttachSurface(state.Host.RequestInvalidate);
                state.RendererContextIdentity = surface->ContextIdentity;
            }
            state.Host.BeginFrame(in *surface);
            SkiaPaintCompletion? completion = null;
            state.Surface.Render(in *surface, skiaSurface =>
                completion = state.Renderer.Paint(skiaSurface, surface->PixelWidth, surface->PixelHeight));
            state.RecordRasterized(frameToken, completion);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnFrameTerminal(
        nint context,
        nint viewHandle,
        ulong frameToken,
        uint terminalState,
        ulong surfaceGeneration,
        long timestampMicroseconds) =>
        GuardVoid(context, state =>
        {
            _ = (viewHandle, surfaceGeneration, timestampMicroseconds);
            state.RecordTerminal(frameToken, (QtNativeV2.TerminalState)terminalState);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnSurfaceDestroying(
        nint context,
        nint viewHandle,
        ulong surfaceGeneration,
        ulong contextIdentity) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            state.Surface.Release(surfaceGeneration, contextIdentity);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnDiagnostic(nint context, QtNativeV2.Utf8 key, QtNativeV2.Utf8 value) =>
        GuardVoid(context, state => state.RecordDiagnostic(Decode(key), Decode(value)));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnFatal(nint context, int errorCode, QtNativeV2.Utf8 message) =>
        GuardVoid(context, state => state.CaptureFatal(
            new InvalidOperationException($"Qt native fatal {errorCode}: {Decode(message)}")));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnMetricsChanged(nint context, nint viewHandle, QtNativeV2.Metrics* metrics) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            if (metrics == null || metrics->AbiVersion != QtNativeV2.AbiVersion ||
                metrics->StructSize < sizeof(QtNativeV2.Metrics))
                throw new InvalidDataException("Qt supplied an invalid metrics descriptor.");
            state.Host?.ApplyMetrics(in *metrics);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnLifecycleChanged(nint context, nint viewHandle, uint lifecycle, long timestamp) =>
        GuardVoid(context, state =>
        {
            _ = (viewHandle, timestamp);
            state.Host?.ApplyLifecycle(lifecycle);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnCloseRequested(nint context, nint viewHandle) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            state.Host?.RaiseCloseRequested();
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnClosed(nint context, nint viewHandle) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            state.Host?.RaiseClosed();
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnPointer(nint context, nint viewHandle, QtNativeV2.Pointer* pointer) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            if (pointer == null || pointer->AbiVersion != QtNativeV2.AbiVersion ||
                pointer->StructSize < sizeof(QtNativeV2.Pointer)) return;
            state.Host?.ApplyPointer(in *pointer);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnKey(nint context, nint viewHandle, QtNativeV2.Key* key) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            if (key == null || key->AbiVersion != QtNativeV2.AbiVersion ||
                key->StructSize < sizeof(QtNativeV2.Key)) return;
            state.Host?.ApplyKey(in *key, Decode(key->Character));
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnFocus(nint context, nint viewHandle, uint focused, long timestamp) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            state.Host?.ApplyFocus(focused != 0, timestamp);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnTextEditing(nint context, nint viewHandle, QtNativeV2.TextState* editing) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            if (editing == null || editing->AbiVersion != QtNativeV2.AbiVersion ||
                editing->StructSize < sizeof(QtNativeV2.TextState)) return;
            state.Host?.ApplyTextEditing(Decode(editing->Text), editing->SelectionBase,
                editing->SelectionExtent, editing->ComposingBase, editing->ComposingExtent);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnTextAction(nint context, nint viewHandle, uint action) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            state.Host?.ApplyTextAction(action);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnClipboardText(nint context, nint viewHandle, ulong requestId, QtNativeV2.Utf8 text) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            state.Host?.CompleteClipboard(requestId, Decode(text));
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnConfigurationChanged(nint context, nint viewHandle,
        QtNativeV2.Utf8 languages, uint brightness, uint alwaysUse24HourFormat) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            state.Host?.ApplyConfiguration(Decode(languages), brightness, alwaysUse24HourFormat != 0);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnSemanticsAction(nint context, nint viewHandle,
        long nodeId, long action, QtNativeV2.Utf8 argumentsJson) =>
        GuardVoid(context, state =>
        {
            _ = viewHandle;
            state.Host?.ApplySemanticsAction(nodeId, action, Decode(argumentsJson));
        });

    private static int Guard(nint context, Action<QtManagedState> callback)
    {
        try
        {
            callback(GetState(context));
            return (int)QtNativeV2.Result.Ok;
        }
        catch (Exception exception)
        {
            TryCaptureFatal(context, exception);
            return (int)QtNativeV2.Result.ManagedFatal;
        }
    }

    private static void GuardVoid(nint context, Action<QtManagedState> callback)
    {
        try
        {
            callback(GetState(context));
        }
        catch (Exception exception)
        {
            TryCaptureFatal(context, exception);
        }
    }

    private static void TryCaptureFatal(nint context, Exception exception)
    {
        try { GetState(context).CaptureFatal(exception); }
        catch { /* Never allow a managed exception to cross the C ABI boundary. */ }
    }

    private static QtManagedState GetState(nint context) =>
        (QtManagedState)(GCHandle.FromIntPtr(context).Target
            ?? throw new InvalidOperationException("The Qt managed callback context is no longer available."));

    private static string Decode(QtNativeV2.Utf8 value)
    {
        if (value.Data == null || value.Length == 0) return string.Empty;
        if (value.Length > int.MaxValue) throw new InvalidDataException("Qt UTF-8 payload exceeds the managed size limit.");
        return Encoding.UTF8.GetString(new ReadOnlySpan<byte>(value.Data, checked((int)value.Length)));
    }

    private static partial class NativeMethods
    {
        [LibraryImport(NativeLibraryName, EntryPoint = "doroti_qt_run_v2")]
        internal static partial int Run(
            in QtNativeV2.Configuration configuration,
            in QtNativeV2.Callbacks callbacks);
    }
}
