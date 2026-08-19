using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Doroti.Hosting;
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
        using var state = new QtManagedState(session);
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
                    checked((int)descriptor.ViewConfiguration.logicalSize.height));
                var callbacks = new QtNativeV2.Callbacks(GCHandle.ToIntPtr(stateHandle));
                var exitCode = NativeMethods.Run(in configuration, in callbacks);
                state.ThrowIfFatal();
                if (exitCode is >= 64 and <= 70)
                    throw new InvalidOperationException($"doroti.qt-host/v2 exited with deterministic error code {exitCode}.");
                state.ValidateTerminalCoverage();
                return exitCode;
            }
        }
        finally
        {
            stateHandle.Free();
        }
    }

    internal sealed class QtManagedState : IDisposable
    {
        private readonly object _gate = new();
        private readonly HashSet<ulong> _terminalTokens = [];
        private Exception? _fatal;
        private QtNativeV2.HostApi _hostApi;
        private nint _viewHandle;
        private long _rasterized;
        private long _presented;
        private long _replayed;
        private long _superseded;
        private long _failed;
        private bool _disposed;

        private readonly GRGlGetProcedureAddressDelegate _glResolver;

        internal QtManagedState(DorotiHostSession session)
        {
            Session = session;
            _glResolver = ResolveGlProcedure;
            Renderer = new(_glResolver);
        }

        internal DorotiHostSession Session { get; }
        internal QtSkiaSurface Renderer { get; }

        internal void SetHost(nint viewHandle, in QtNativeV2.HostApi hostApi)
        {
            if (hostApi.AbiVersion != QtNativeV2.AbiVersion ||
                hostApi.StructSize < (uint)sizeof(QtNativeV2.HostApi) ||
                (hostApi.FeatureBits & QtNativeV2.RequiredFeatures) != QtNativeV2.RequiredFeatures ||
                hostApi.RequestFrame == null || hostApi.RequestClose == null ||
                hostApi.GetGlProcAddress == null)
            {
                throw new InvalidDataException("The native Qt host API does not satisfy doroti.qt-host/v2.");
            }
            lock (_gate)
            {
                _viewHandle = viewHandle;
                _hostApi = hostApi;
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

        internal void RecordRasterized() => Interlocked.Increment(ref _rasterized);

        internal void RecordTerminal(ulong token, QtNativeV2.TerminalState terminal)
        {
            lock (_gate)
            {
                if (!_terminalTokens.Add(token))
                    throw new InvalidDataException($"Qt frame token {token} received more than one terminal ACK.");
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
            Renderer.Dispose();
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
            state.Renderer.Render(in *surface);
            state.RecordRasterized();
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
            state.Renderer.Release(surfaceGeneration, contextIdentity);
        });

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnDiagnostic(nint context, QtNativeV2.Utf8 key, QtNativeV2.Utf8 value) =>
        GuardVoid(context, _ => Console.Error.WriteLine($"doroti.qt {Decode(key)}={Decode(value)}"));

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    internal static void OnFatal(nint context, int errorCode, QtNativeV2.Utf8 message) =>
        GuardVoid(context, state => state.CaptureFatal(
            new InvalidOperationException($"Qt native fatal {errorCode}: {Decode(message)}")));

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
