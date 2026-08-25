using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Owns the Windows App SDK prerequisites for the Flutter-style host path.
/// This type deliberately does not create, show, resize, or destroy a window;
/// F2 owns the standard top-level/child HWND tree.
/// </summary>
internal sealed partial class FlutterWindowsAppSdkBootstrap : IDisposable
{
    internal const string ExpectedWindowsAppSdkVersion = "2.4.0";

    private const uint CoinitApartmentThreaded = 0x2;
    private const uint RoInitSingleThreaded = 0;
    private const int ErrorAccessDenied = 5;
    private static readonly nint PerMonitorAwareV2 = new(-4);
    private static readonly ConcurrentDictionary<uint, FlutterWindowsAppSdkBootstrap>
        ActiveBootstrapsByNativeThread = new();

    private readonly object _stateGate = new();
    private FlutterWindowsAppSdkBootstrapPhase _phase = FlutterWindowsAppSdkBootstrapPhase.Created;
    private int _platformManagedThreadId;
    private uint _platformNativeThreadId;
    private bool _registeredActiveBootstrap;
    private bool _processDpiAwarenessRequested;
    private bool _threadDpiAwarenessSet;
    private nint _previousThreadDpiAwarenessContext;
    private bool _comInitialized;
    private bool _oleInitialized;
    private bool _winRtInitialized;
    private DispatcherQueueController? _dispatcherQueueController;
    private FlutterWindowsAppSdkPlatformTaskRunner? _platformTaskRunner;
    private nint _rawWindow;
    private AppWindow? _appWindow;
    private int _rawWindowAssociationCount;

    /// <summary>
    /// A cross-thread-safe immutable view of the bootstrap lifecycle. The live
    /// Windows App SDK objects themselves remain platform-thread-affine.
    /// </summary>
    internal FlutterWindowsAppSdkBootstrapSnapshot Snapshot
    {
        get
        {
            lock (_stateGate)
            {
                var appWindowAssembly = typeof(AppWindow).Assembly;
                return new FlutterWindowsAppSdkBootstrapSnapshot(
                    _phase,
                    _platformManagedThreadId,
                    _platformNativeThreadId,
                    _processDpiAwarenessRequested,
                    _threadDpiAwarenessSet,
                    _comInitialized,
                    _oleInitialized,
                    _winRtInitialized,
                    _dispatcherQueueController is not null,
                    _platformTaskRunner is not null,
                    _rawWindow,
                    _rawWindow != 0,
                    _appWindow is not null,
                    _rawWindowAssociationCount,
                    appWindowAssembly.GetName().Version?.ToString() ?? "unknown",
                    appWindowAssembly.Location);
            }
        }
    }

    /// <summary>
    /// The platform-thread task runner created after the DispatcherQueue. It
    /// never runs a nested Win32 message loop.
    /// </summary>
    internal FlutterWindowsAppSdkPlatformTaskRunner PlatformTaskRunner
    {
        get
        {
            EnsureInitializedOnPlatformThread();
            return _platformTaskRunner ?? throw new InvalidOperationException(
                "The platform task runner was not created.");
        }
    }

    /// <summary>
    /// Initializes the one allowed DispatcherQueue for this platform thread.
    /// Call this on an STA thread before F2 creates any HWND or AppWindow.
    /// The setup below invokes SetThreadDpiAwarenessContext before the queue is
    /// created, so the effective per-monitor-v2 context cannot lag Windows App SDK.
    /// </summary>
    internal void InitializeOnCurrentThread()
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException(
                "The Flutter Windows App SDK bootstrap can only run on Windows.");
        if (Thread.CurrentThread.GetApartmentState() != ApartmentState.STA)
            throw new InvalidOperationException(
                "The Flutter Windows App SDK platform thread must be STA before bootstrap.");

        var managedThreadId = Environment.CurrentManagedThreadId;
        var nativeThreadId = GetCurrentThreadId();
        lock (_stateGate)
        {
            switch (_phase)
            {
                case FlutterWindowsAppSdkBootstrapPhase.Ready:
                case FlutterWindowsAppSdkBootstrapPhase.RawWindowAssociated:
                    EnsureCurrentThreadLocked(managedThreadId, nativeThreadId);
                    return;
                case FlutterWindowsAppSdkBootstrapPhase.Created:
                    _platformManagedThreadId = managedThreadId;
                    _platformNativeThreadId = nativeThreadId;
                    _phase = FlutterWindowsAppSdkBootstrapPhase.Initializing;
                    break;
                default:
                    throw new InvalidOperationException(
                        $"Bootstrap cannot be initialized from {_phase}.");
            }
        }

        if (!ActiveBootstrapsByNativeThread.TryAdd(nativeThreadId, this))
        {
            lock (_stateGate) _phase = FlutterWindowsAppSdkBootstrapPhase.Faulted;
            throw new InvalidOperationException(
                "A Flutter Windows App SDK bootstrap already owns this platform thread.");
        }
        _registeredActiveBootstrap = true;

        try
        {
            InitializePerMonitorV2DpiAwareness();
            InitializeComOleAndWinRt();

            // This is intentionally the only DispatcherQueueController created by
            // the Flutter-style path for its platform thread.
            _dispatcherQueueController = DispatcherQueueController.CreateOnCurrentThread();
            _platformTaskRunner = new FlutterWindowsAppSdkPlatformTaskRunner(
                _dispatcherQueueController.DispatcherQueue,
                _platformManagedThreadId,
                _platformNativeThreadId);

            lock (_stateGate)
            {
                _phase = FlutterWindowsAppSdkBootstrapPhase.Ready;
            }
        }
        catch
        {
            ShutdownPartiallyInitializedOnCurrentThread();
            lock (_stateGate) _phase = FlutterWindowsAppSdkBootstrapPhase.Faulted;
            UnregisterActiveBootstrap();
            throw;
        }
    }

    /// <summary>
    /// Associates an F2-owned raw top-level HWND with the already-created
    /// DispatcherQueue. This method does not create, show, size, or close the
    /// HWND, so render-size and first-frame ownership stay out of F1.
    /// </summary>
    internal AppWindow AssociateRawWindow(nint hwnd)
    {
        if (hwnd == 0) throw new ArgumentOutOfRangeException(nameof(hwnd));
        EnsureInitializedOnPlatformThread();

        lock (_stateGate)
        {
            if (_rawWindow != 0)
            {
                if (_rawWindow != hwnd)
                    throw new InvalidOperationException(
                        "This bootstrap is already associated with a different raw HWND.");
                return _appWindow ?? throw new InvalidOperationException(
                    "The raw HWND association has no AppWindow.");
            }
        }

        var dispatcher = _dispatcherQueueController ?? throw new InvalidOperationException(
            "The platform DispatcherQueue was not created.");
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.AssociateWithDispatcherQueue(dispatcher.DispatcherQueue);

        lock (_stateGate)
        {
            if (_phase != FlutterWindowsAppSdkBootstrapPhase.Ready)
                throw new InvalidOperationException(
                    $"Raw HWND association is not valid while bootstrap is {_phase}.");
            _rawWindow = hwnd;
            _appWindow = appWindow;
            _rawWindowAssociationCount++;
            _phase = FlutterWindowsAppSdkBootstrapPhase.RawWindowAssociated;
        }
        return appWindow;
    }

    /// <summary>
    /// Records that the F2 owner has closed its AppWindow/raw HWND pair. It
    /// must be called on the platform thread before queue shutdown; this type
    /// never destroys a caller-owned HWND itself.
    /// </summary>
    internal void ReleaseRawWindowAssociation(nint hwnd)
    {
        if (hwnd == 0) throw new ArgumentOutOfRangeException(nameof(hwnd));
        EnsureInitializedOnPlatformThread();

        lock (_stateGate)
        {
            if (_phase != FlutterWindowsAppSdkBootstrapPhase.RawWindowAssociated ||
                _rawWindow != hwnd)
            {
                throw new InvalidOperationException(
                    "The supplied raw HWND is not the active bootstrap association.");
            }

            _appWindow = null;
            _rawWindow = 0;
            _phase = FlutterWindowsAppSdkBootstrapPhase.Ready;
        }
    }

    /// <summary>
    /// Stops the platform task runner, shuts down its DispatcherQueue, then
    /// unwinds WinRT, OLE, COM, and the per-thread DPI override in reverse
    /// creation order. Calling it from another thread is an ownership error.
    /// </summary>
    internal void DisposeOnCurrentThread()
    {
        FlutterWindowsAppSdkBootstrapPhase phase;
        lock (_stateGate) phase = _phase;

        if (phase == FlutterWindowsAppSdkBootstrapPhase.Created)
        {
            lock (_stateGate) _phase = FlutterWindowsAppSdkBootstrapPhase.Disposed;
            return;
        }

        EnsurePlatformThread();
        lock (_stateGate)
        {
            if (_phase == FlutterWindowsAppSdkBootstrapPhase.Disposed) return;
            if (_phase == FlutterWindowsAppSdkBootstrapPhase.RawWindowAssociated)
            {
                throw new InvalidOperationException(
                    "Close and release the F2 raw HWND/AppWindow association before queue shutdown.");
            }
            if (_phase is not (FlutterWindowsAppSdkBootstrapPhase.Ready or
                FlutterWindowsAppSdkBootstrapPhase.Faulted))
            {
                throw new InvalidOperationException(
                    $"Bootstrap cannot be shut down from {_phase}.");
            }
            _phase = FlutterWindowsAppSdkBootstrapPhase.ShuttingDown;
        }

        Exception? failure = null;
        try
        {
            _platformTaskRunner?.StopAcceptingWorkOnPlatformThread();
            _dispatcherQueueController?.ShutdownQueue();
        }
        catch (Exception exception)
        {
            failure = exception;
        }
        finally
        {
            _platformTaskRunner = null;
            _dispatcherQueueController = null;
            UninitializeComOleAndWinRt();
            RestoreThreadDpiAwareness();
            UnregisterActiveBootstrap();
            lock (_stateGate) _phase = FlutterWindowsAppSdkBootstrapPhase.Disposed;
        }

        if (failure is not null) throw new InvalidOperationException(
            "DispatcherQueue shutdown failed after platform work was stopped.", failure);
    }

    public void Dispose() => DisposeOnCurrentThread();

    private void InitializePerMonitorV2DpiAwareness()
    {
        if (SetProcessDpiAwarenessContext(PerMonitorAwareV2))
        {
            _processDpiAwarenessRequested = true;
        }
        else
        {
            var error = Marshal.GetLastWin32Error();
            if (error != ErrorAccessDenied)
                throw new Win32Exception(error,
                    "SetProcessDpiAwarenessContext(PER_MONITOR_AWARE_V2) failed.");

            // A manifest or an earlier bootstrap may have set the process mode.
            // The thread override and verification below establish the effective
            // context for this platform thread without weakening the F1 contract.
            _processDpiAwarenessRequested = true;
        }

        var previousContext = SetThreadDpiAwarenessContext(PerMonitorAwareV2);
        if (previousContext == 0)
        {
            var error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error,
                "SetThreadDpiAwarenessContext(PER_MONITOR_AWARE_V2) failed.");
        }

        _previousThreadDpiAwarenessContext = previousContext;
        _threadDpiAwarenessSet = true;
        if (!AreDpiAwarenessContextsEqual(
            GetThreadDpiAwarenessContext(), PerMonitorAwareV2))
        {
            throw new InvalidOperationException(
                "The platform thread did not enter PER_MONITOR_AWARE_V2 DPI awareness.");
        }
    }

    private void InitializeComOleAndWinRt()
    {
        var result = CoInitializeEx(0, CoinitApartmentThreaded);
        ThrowIfFailed(result, "CoInitializeEx(COINIT_APARTMENTTHREADED)");
        _comInitialized = true;

        result = OleInitialize(0);
        ThrowIfFailed(result, "OleInitialize");
        _oleInitialized = true;

        result = RoInitialize(RoInitSingleThreaded);
        ThrowIfFailed(result, "RoInitialize(RO_INIT_SINGLETHREADED)");
        _winRtInitialized = true;
    }

    private void ShutdownPartiallyInitializedOnCurrentThread()
    {
        try
        {
            _platformTaskRunner?.StopAcceptingWorkOnPlatformThread();
            _dispatcherQueueController?.ShutdownQueue();
        }
        catch
        {
            // Preserve the original initialization failure; the state snapshot
            // still shows which prerequisites were successfully acquired.
        }
        finally
        {
            _platformTaskRunner = null;
            _dispatcherQueueController = null;
            UninitializeComOleAndWinRt();
            RestoreThreadDpiAwareness();
        }
    }

    private void UninitializeComOleAndWinRt()
    {
        if (_winRtInitialized)
        {
            RoUninitialize();
            _winRtInitialized = false;
        }
        if (_oleInitialized)
        {
            OleUninitialize();
            _oleInitialized = false;
        }
        if (_comInitialized)
        {
            CoUninitialize();
            _comInitialized = false;
        }
    }

    private void RestoreThreadDpiAwareness()
    {
        if (!_threadDpiAwarenessSet) return;
        _ = SetThreadDpiAwarenessContext(_previousThreadDpiAwarenessContext);
        _threadDpiAwarenessSet = false;
        _previousThreadDpiAwarenessContext = 0;
    }

    private void EnsureInitializedOnPlatformThread()
    {
        EnsurePlatformThread();
        lock (_stateGate)
        {
            if (_phase is not (FlutterWindowsAppSdkBootstrapPhase.Ready or
                FlutterWindowsAppSdkBootstrapPhase.RawWindowAssociated))
            {
                throw new InvalidOperationException(
                    $"Bootstrap is not ready while in {_phase}.");
            }
        }
    }

    private void EnsurePlatformThread()
    {
        var managedThreadId = Environment.CurrentManagedThreadId;
        var nativeThreadId = GetCurrentThreadId();
        lock (_stateGate) EnsureCurrentThreadLocked(managedThreadId, nativeThreadId);
    }

    private void EnsureCurrentThreadLocked(int managedThreadId, uint nativeThreadId)
    {
        if (_platformManagedThreadId != managedThreadId ||
            _platformNativeThreadId != nativeThreadId)
        {
            throw new InvalidOperationException(
                "Flutter Windows App SDK bootstrap objects may only be used by their platform thread.");
        }
    }

    private void UnregisterActiveBootstrap()
    {
        if (!_registeredActiveBootstrap) return;
        if (ActiveBootstrapsByNativeThread.TryGetValue(
            _platformNativeThreadId, out var active) && ReferenceEquals(active, this))
        {
            _ = ActiveBootstrapsByNativeThread.TryRemove(_platformNativeThreadId, out _);
        }
        _registeredActiveBootstrap = false;
    }

    private static void ThrowIfFailed(int result, string operation)
    {
        if (result >= 0) return;
        throw new COMException($"{operation} failed with HRESULT 0x{result:X8}.", result);
    }

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetProcessDpiAwarenessContext(nint value);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetThreadDpiAwarenessContext(nint value);

    [DllImport("user32.dll")]
    private static extern nint GetThreadDpiAwarenessContext();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AreDpiAwarenessContextsEqual(nint first, nint second);

    [DllImport("ole32.dll")]
    private static extern int CoInitializeEx(nint reserved, uint coInit);

    [DllImport("ole32.dll")]
    private static extern int OleInitialize(nint reserved);

    [DllImport("ole32.dll")]
    private static extern void OleUninitialize();

    [DllImport("ole32.dll")]
    private static extern void CoUninitialize();

    [DllImport("combase.dll")]
    private static extern int RoInitialize(uint initType);

    [DllImport("combase.dll")]
    private static extern void RoUninitialize();

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    internal static uint GetCurrentThreadIdForTaskRunner() => GetCurrentThreadId();
}

internal enum FlutterWindowsAppSdkBootstrapPhase
{
    Created,
    Initializing,
    Ready,
    RawWindowAssociated,
    ShuttingDown,
    Faulted,
    Disposed,
}

/// <summary>
/// Immutable lifecycle evidence for the F1 bootstrap. It intentionally exposes
/// only values, never thread-affine Windows App SDK objects.
/// </summary>
internal sealed record FlutterWindowsAppSdkBootstrapSnapshot(
    FlutterWindowsAppSdkBootstrapPhase Phase,
    int PlatformManagedThreadId,
    uint PlatformNativeThreadId,
    bool ProcessPerMonitorV2Requested,
    bool ThreadPerMonitorV2Active,
    bool ComInitialized,
    bool OleInitialized,
    bool WinRtInitialized,
    bool DispatcherQueueCreated,
    bool PlatformTaskRunnerCreated,
    nint RawWindow,
    bool RawWindowAssociated,
    bool AppWindowAssociated,
    int RawWindowAssociationCount,
    string WindowsAppSdkAssemblyVersion,
    string WindowsAppSdkAssemblyPath);

/// <summary>
/// Minimal platform-thread task runner facade. It is not a window tree, a
/// renderer, or a nested message pump; F2+ own those contracts.
/// </summary>
internal sealed class FlutterWindowsAppSdkPlatformTaskRunner
{
    private readonly DispatcherQueue _dispatcherQueue;
    private readonly int _platformManagedThreadId;
    private readonly uint _platformNativeThreadId;
    private int _acceptingWork = 1;

    internal FlutterWindowsAppSdkPlatformTaskRunner(
        DispatcherQueue dispatcherQueue,
        int platformManagedThreadId,
        uint platformNativeThreadId)
    {
        _dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
        _platformManagedThreadId = platformManagedThreadId;
        _platformNativeThreadId = platformNativeThreadId;
    }

    internal bool TryEnqueue(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (Volatile.Read(ref _acceptingWork) == 0) return false;
        return _dispatcherQueue.TryEnqueue(() =>
        {
            if (Volatile.Read(ref _acceptingWork) != 0) callback();
        });
    }

    internal void StopAcceptingWorkOnPlatformThread()
    {
        if (Environment.CurrentManagedThreadId != _platformManagedThreadId ||
            FlutterWindowsAppSdkBootstrap.GetCurrentThreadIdForTaskRunner() != _platformNativeThreadId)
        {
            throw new InvalidOperationException(
                "The platform task runner must be stopped by its platform thread.");
        }
        Interlocked.Exchange(ref _acceptingWork, 0);
    }
}
