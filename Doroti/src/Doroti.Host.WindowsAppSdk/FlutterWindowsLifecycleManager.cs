using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// F8 owner for DPI/display/fullscreen and process-visible lifecycle messages.
/// It never derives render extent from a monitor rectangle: F3 child metrics
/// remain the only surface authority after every transition.
/// </summary>
internal sealed class FlutterWindowsLifecycleManager : IDisposable
{
    private const uint WmSize = 0x0005;
    private const uint WmDisplayChange = 0x007e;
    private const uint WmDpiChanged = 0x02e0;
    private const uint WmPowerBroadcast = 0x0218;
    private const uint WmWtsSessionChange = 0x02b1;
    private const nuint SizeMinimized = 1;
    private const nuint PbtApmSuspend = 4;
    private const nuint PbtApmResumeAutomatic = 18;
    private const nuint WtsSessionLock = 7;
    private const nuint WtsSessionUnlock = 8;
    private const nuint WtsConsoleDisconnect = 2;
    private const nuint WtsConsoleConnect = 1;
    private const int GwlStyle = -16;
    private const uint WsOverlappedWindow = 0x00cf0000;
    private const uint WsPopup = 0x80000000;
    private const uint SwpFrameChanged = 0x0020;
    private const uint SwpNoActivate = 0x0010;
    private const uint MonitorDefaultToNearest = 2;

    private readonly FlutterWindowsHostWindow _host;
    private readonly IFlutterWindowsFrameScheduler? _scheduler;
    private readonly Action _requestLatestFrame;
    private readonly Action _requestGraphicsRecovery;
    private readonly Action _terminalizePending;
    private readonly object _gate = new();
    private NativeRect _restoreRect;
    private nint _restoreMonitor;
    private uint _restoreDpi;
    private uint _restoreStyle;
    private bool _fullscreen;
    private bool _suspended;
    private bool _shutdown;
    private bool _disposed;
    private long _dpiChangedCount;
    private long _displayChangedCount;
    private long _minimizeCount;
    private long _restoreCount;
    private long _suspendCount;
    private long _resumeCount;
    private long _sessionDisconnectCount;
    private long _sessionReconnectCount;
    private long _fullscreenEnterCount;
    private long _fullscreenExitCount;
    private long _workAreaClampCount;
    private long _graphicsRecoveryRequestCount;
    private long _pendingTerminalizationCount;

    internal FlutterWindowsLifecycleManager(
        FlutterWindowsHostWindow host,
        Action requestLatestFrame,
        Action requestGraphicsRecovery,
        Action terminalizePending,
        IFlutterWindowsFrameScheduler? scheduler = null)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        _requestLatestFrame = requestLatestFrame ?? throw new ArgumentNullException(nameof(requestLatestFrame));
        _requestGraphicsRecovery = requestGraphicsRecovery ?? throw new ArgumentNullException(nameof(requestGraphicsRecovery));
        _terminalizePending = terminalizePending ?? throw new ArgumentNullException(nameof(terminalizePending));
        _scheduler = scheduler;
        _host.TopLevelMessageReceived += HandleTopLevelMessage;
    }

    internal bool IsFullscreen { get { lock (_gate) return _fullscreen; } }

    internal void SetFullscreen(bool fullscreen)
    {
        lock (_gate)
        {
            ThrowIfDisposed();
            if (_fullscreen == fullscreen) return;
            if (fullscreen) EnterFullscreenLocked();
            else ExitFullscreenLocked();
        }
    }

    internal void EnsureVisibleInCurrentWorkArea()
    {
        lock (_gate)
        {
            ThrowIfDisposed();
            if (!NativeMethods.GetWindowRect(_host.TopLevelHwnd, out var rect))
                throw NativeFailure("GetWindowRect for work-area clamp failed.");
            var monitor = NativeMethods.MonitorFromRect(in rect, MonitorDefaultToNearest);
            var info = GetMonitorInfo(monitor);
            var clamped = ClampToWorkArea(rect, info.Work);
            if (clamped == rect) return;
            SetTopLevelRect(clamped, frameChanged: false);
            _workAreaClampCount++;
        }
    }

    internal void BeginShutdown()
    {
        lock (_gate)
        {
            if (_disposed || _shutdown) return;
            _shutdown = true;
            _scheduler?.SetSuspended(true);
            _terminalizePending();
            _pendingTerminalizationCount++;
        }
    }

    internal FlutterWindowsLifecycleSnapshot Snapshot
    {
        get
        {
            lock (_gate)
            {
                return new(
                    _fullscreen,
                    _suspended,
                    _shutdown,
                    _restoreMonitor,
                    _restoreDpi,
                    _dpiChangedCount,
                    _displayChangedCount,
                    _minimizeCount,
                    _restoreCount,
                    _suspendCount,
                    _resumeCount,
                    _sessionDisconnectCount,
                    _sessionReconnectCount,
                    _fullscreenEnterCount,
                    _fullscreenExitCount,
                    _workAreaClampCount,
                    _graphicsRecoveryRequestCount,
                    _pendingTerminalizationCount,
                    _disposed);
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            BeginShutdown();
            _host.TopLevelMessageReceived -= HandleTopLevelMessage;
            _disposed = true;
        }
    }

    private FlutterWindowsChildMessageResult HandleTopLevelMessage(FlutterWindowsTopLevelMessage message)
    {
        lock (_gate)
        {
            if (_disposed) return FlutterWindowsChildMessageResult.Unhandled;
            switch (message.Message)
            {
                case WmDpiChanged:
                    _dpiChangedCount++;
                    RequestLatestAndRecoveryLocked();
                    break;
                case WmDisplayChange:
                    _displayChangedCount++;
                    EnsureVisibleInCurrentWorkArea();
                    RequestLatestAndRecoveryLocked();
                    break;
                case WmSize:
                    var minimized = message.WParam == SizeMinimized;
                    _scheduler?.SetMinimized(minimized);
                    if (minimized) _minimizeCount++;
                    else
                    {
                        _restoreCount++;
                        _requestLatestFrame();
                    }
                    break;
                case WmPowerBroadcast when message.WParam == PbtApmSuspend:
                    SuspendLocked(sessionDisconnect: false);
                    return FlutterWindowsChildMessageResult.HandledResult(1);
                case WmPowerBroadcast when message.WParam == PbtApmResumeAutomatic:
                    ResumeLocked(sessionReconnect: false);
                    return FlutterWindowsChildMessageResult.HandledResult(1);
                case WmWtsSessionChange when message.WParam is WtsSessionLock or WtsConsoleDisconnect:
                    SuspendLocked(sessionDisconnect: true);
                    break;
                case WmWtsSessionChange when message.WParam is WtsSessionUnlock or WtsConsoleConnect:
                    ResumeLocked(sessionReconnect: true);
                    break;
            }
            return FlutterWindowsChildMessageResult.Unhandled;
        }
    }

    private void EnterFullscreenLocked()
    {
        if (!NativeMethods.GetWindowRect(_host.TopLevelHwnd, out _restoreRect))
            throw NativeFailure("GetWindowRect before fullscreen failed.");
        _restoreMonitor = NativeMethods.MonitorFromWindow(_host.TopLevelHwnd, MonitorDefaultToNearest);
        _restoreDpi = NativeMethods.GetDpiForWindow(_host.TopLevelHwnd);
        _restoreStyle = unchecked((uint)NativeMethods.GetWindowLongPtrW(_host.TopLevelHwnd, GwlStyle).ToInt64());
        var monitorInfo = GetMonitorInfo(_restoreMonitor);
        _ = NativeMethods.SetWindowLongPtrW(_host.TopLevelHwnd, GwlStyle,
            unchecked((nint)((_restoreStyle & ~WsOverlappedWindow) | WsPopup)));
        SetTopLevelRect(monitorInfo.Monitor, frameChanged: true);
        _fullscreen = true;
        _fullscreenEnterCount++;
        _requestLatestFrame();
    }

    private void ExitFullscreenLocked()
    {
        _ = NativeMethods.SetWindowLongPtrW(_host.TopLevelHwnd, GwlStyle, unchecked((nint)_restoreStyle));
        var target = _restoreRect;
        var monitor = NativeMethods.MonitorFromRect(in target, MonitorDefaultToNearest);
        target = ClampToWorkArea(target, GetMonitorInfo(monitor).Work);
        SetTopLevelRect(target, frameChanged: true);
        _fullscreen = false;
        _fullscreenExitCount++;
        _requestLatestFrame();
    }

    private void SuspendLocked(bool sessionDisconnect)
    {
        if (!_suspended)
        {
            _suspended = true;
            _scheduler?.SetSuspended(true);
            _suspendCount++;
        }
        if (sessionDisconnect) _sessionDisconnectCount++;
    }

    private void ResumeLocked(bool sessionReconnect)
    {
        if (_suspended)
        {
            _suspended = false;
            _scheduler?.SetSuspended(false);
            _resumeCount++;
        }
        if (sessionReconnect) _sessionReconnectCount++;
        RequestLatestAndRecoveryLocked();
    }

    private void RequestLatestAndRecoveryLocked()
    {
        _requestLatestFrame();
        _requestGraphicsRecovery();
        _graphicsRecoveryRequestCount++;
    }

    private void SetTopLevelRect(NativeRect rect, bool frameChanged)
    {
        if (!NativeMethods.SetWindowPos(
                _host.TopLevelHwnd,
                0,
                rect.Left,
                rect.Top,
                rect.Width,
                rect.Height,
                SwpNoActivate | (frameChanged ? SwpFrameChanged : 0)))
            throw NativeFailure("SetWindowPos for lifecycle transition failed.");
    }

    private static MonitorInfo GetMonitorInfo(nint monitor)
    {
        if (monitor == 0) throw new InvalidOperationException("No nearest monitor was available.");
        var info = new MonitorInfo { Size = checked((uint)Marshal.SizeOf<MonitorInfo>()) };
        if (!NativeMethods.GetMonitorInfoW(monitor, ref info))
            throw NativeFailure("GetMonitorInfoW failed.");
        return info;
    }

    private static NativeRect ClampToWorkArea(NativeRect rect, NativeRect work)
    {
        var width = Math.Min(rect.Width, work.Width);
        var height = Math.Min(rect.Height, work.Height);
        var left = Math.Clamp(rect.Left, work.Left, work.Right - width);
        var top = Math.Clamp(rect.Top, work.Top, work.Bottom - height);
        return new(left, top, left + width, top + height);
    }

    private static Win32Exception NativeFailure(string message) =>
        new(Marshal.GetLastWin32Error(), message);

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct NativeRect(int Left, int Top, int Right, int Bottom)
    {
        internal int Width => Right - Left;
        internal int Height => Bottom - Top;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MonitorInfo
    {
        internal uint Size;
        internal NativeRect Monitor;
        internal NativeRect Work;
        internal uint Flags;
    }

    private static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(nint hwnd, out NativeRect rect);
        [DllImport("user32.dll")]
        internal static extern nint MonitorFromWindow(nint hwnd, uint flags);
        [DllImport("user32.dll")]
        internal static extern nint MonitorFromRect(in NativeRect rect, uint flags);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetMonitorInfoW(nint monitor, ref MonitorInfo info);
        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
        internal static extern nint GetWindowLongPtrW(nint hwnd, int index);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        internal static extern nint SetWindowLongPtrW(nint hwnd, int index, nint value);
        [DllImport("user32.dll")]
        internal static extern uint GetDpiForWindow(nint hwnd);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(
            nint hwnd, nint insertAfter, int x, int y, int width, int height, uint flags);
    }
}

internal sealed record FlutterWindowsLifecycleSnapshot(
    bool Fullscreen,
    bool Suspended,
    bool Shutdown,
    nint RestoreMonitor,
    uint RestoreDpi,
    long DpiChangedCount,
    long DisplayChangedCount,
    long MinimizeCount,
    long RestoreCount,
    long SuspendCount,
    long ResumeCount,
    long SessionDisconnectCount,
    long SessionReconnectCount,
    long FullscreenEnterCount,
    long FullscreenExitCount,
    long WorkAreaClampCount,
    long GraphicsRecoveryRequestCount,
    long PendingTerminalizationCount,
    bool Disposed);
