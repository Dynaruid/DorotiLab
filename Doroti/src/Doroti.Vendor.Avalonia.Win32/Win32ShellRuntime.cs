// Doroti adaptation of Avalonia Win32Platform/WindowImpl/Dispatcher lifecycle flow.
// Source identity and selected closure are pinned in migration/avalonia-shell/port-selection.json.
using System.Collections.Concurrent;
using Doroti.Graphics;
using Doroti.Shell.Core;

namespace Doroti.Vendor.Avalonia.Win32;

internal static class Win32ShellBootstrap
{
    internal static Win32ShellPlatform Create() => new();
}

internal sealed class Win32ShellPlatform : IShellWindowingPlatform, IShellDispatcher, IShellEventLoop, IDisposable
{
    private readonly int _threadId = Environment.CurrentManagedThreadId;
    private readonly ConcurrentQueue<Action> _posted = new();
    private readonly AutoResetEvent _wake = new(false);
    private readonly List<Win32ShellWindow> _windows = [];
    private bool _exitRequested;
    private bool _disposed;

    public IShellDispatcher Dispatcher => this;

    public IShellEventLoop EventLoop => this;

    public bool CheckAccess() => Environment.CurrentManagedThreadId == _threadId;

    public void VerifyAccess()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CheckAccess())
        {
            throw new InvalidOperationException("The source-ported Win32 shell must be accessed from its owning UI thread.");
        }
    }

    public void Post(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        _posted.Enqueue(callback);
        _wake.Set();
    }

    public IShellWindow CreateWindow(string title, Size initialLogicalClientSize)
    {
        VerifyAccess();
        if (!initialLogicalClientSize.IsFinite || initialLogicalClientSize.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(initialLogicalClientSize));
        }

        var window = new Win32ShellWindow(this, title, initialLogicalClientSize);
        _windows.Add(window);
        return window;
    }

    public bool PumpOnce(bool waitForMessage = false)
    {
        VerifyAccess();
        if (waitForMessage && _posted.IsEmpty)
        {
            _wake.WaitOne(10);
        }

        var didWork = false;
        // Process the callbacks that were queued when this pump began. A callback may
        // schedule the next animation/frame callback; draining those recursively can
        // starve the native message loop and prevent diagnostics or shutdown from running.
        var postedAtEntry = _posted.Count;
        for (var index = 0; index < postedAtEntry && _posted.TryDequeue(out var callback); index++)
        {
            callback();
            didWork = true;
        }

        foreach (var window in _windows.ToArray())
        {
            if (!window.IsClosed)
            {
                window.PumpPendingMessages();
                didWork = true;
            }
        }

        _windows.RemoveAll(window => window.IsClosed);
        return didWork;
    }

    public void Run(CancellationToken cancellationToken = default)
    {
        VerifyAccess();
        while (!_exitRequested && !cancellationToken.IsCancellationRequested && _windows.Count > 0)
        {
            PumpOnce(waitForMessage: true);
        }
    }

    public void RequestExit()
    {
        _exitRequested = true;
        _wake.Set();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        VerifyAccess();
        foreach (var window in _windows.ToArray())
        {
            window.Dispose();
        }
        _windows.Clear();
        _disposed = true;
        _wake.Dispose();
    }
}

internal sealed class Win32ShellWindow : IShellWindow
{
    private readonly Win32ShellPlatform _platform;
    private readonly NativeWindowHost _native;
    private long _generation;
    private long _scaleGeneration;
    private long _surfaceGeneration;
    private ShellWindowMetrics _metrics;
    private bool _disposed;

    internal Win32ShellWindow(Win32ShellPlatform platform, string title, Size initialLogicalClientSize)
    {
        _platform = platform;
        Services = new();
        Services.Add<IShellDispatcher>(platform);
        Services.Add<IShellEventLoop>(platform);
        _native = new(
            title,
            initialLogicalClientSize.Width,
            initialLogicalClientSize.Height,
            OnNativeWindow,
            input => Pointer?.Invoke(input),
            input => Key?.Invoke(input),
            input => Text?.Invoke(input));
        _metrics = Convert(_native.Metrics, ShellWindowState.Normal, ++_generation);
    }

    public event Action<ShellWindowEvent>? WindowEvent;

    internal event Action<NativePointerEvent>? Pointer;

    internal event Action<NativeKeyEvent>? Key;

    internal event Action<NativeTextEvent>? Text;

    public ulong Id => _native.WindowId;

    public ShellNativeHandle NativeHandle => new(_native.Handle, "HWND");

    public ShellWindowMetrics Metrics => _metrics;

    public ShellPlatformServiceRegistry Services { get; }

    public IReadOnlyList<ShellScreen> Screens => _native.Displays.Select(display => new ShellScreen(
        display.Id,
        new(display.WorkArea.Left, display.WorkArea.Top, display.WorkArea.Right, display.WorkArea.Bottom),
        _metrics.ScaleFactor)).ToArray();

    internal NativeWindowHost NativeHost => _native;

    internal bool IsClosed => _native.Handle == 0;

    public void Show()
    {
        VerifyAccess();
        _native.Show();
        WindowEvent?.Invoke(new(ShellWindowEventKind.Opened, _metrics));
    }

    public void Resize(Size logicalClientSize)
    {
        VerifyAccess();
        _native.Resize(logicalClientSize.Width, logicalClientSize.Height);
    }

    public void SetState(ShellWindowState state)
    {
        VerifyAccess();
        if (state is not (ShellWindowState.Normal or ShellWindowState.Minimized))
        {
            throw new NotSupportedException($"The A1 Win32 slice cannot request {state} explicitly.");
        }
        _native.SetMinimized(state == ShellWindowState.Minimized);
    }

    public void MoveToScreen(ulong screenId)
    {
        VerifyAccess();
        _native.MoveToDisplay(screenId);
    }

    public void Close()
    {
        VerifyAccess();
        _native.Close();
    }

    internal void PumpPendingMessages() => _native.PumpPendingMessages();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        VerifyAccess();
        _disposed = true;
        _native.Dispose();
    }

    private void OnNativeWindow(NativeWindowNotification notification)
    {
        var kind = notification.Kind switch
        {
            NativeWindowNotificationKind.Activated => ShellWindowEventKind.Activated,
            NativeWindowNotificationKind.Deactivated => ShellWindowEventKind.Deactivated,
            NativeWindowNotificationKind.MetricsChanged => ShellWindowEventKind.MetricsChanged,
            NativeWindowNotificationKind.CaptureCancelled => ShellWindowEventKind.CaptureCancelled,
            NativeWindowNotificationKind.CloseRequested => ShellWindowEventKind.CloseRequested,
            NativeWindowNotificationKind.Closed => ShellWindowEventKind.Closed,
            _ => throw new InvalidOperationException($"Unknown native notification {notification.Kind}."),
        };
        var state = notification.Kind == NativeWindowNotificationKind.Closed
            ? ShellWindowState.Closed
            : notification.Metrics.IsMinimized ? ShellWindowState.Minimized : ShellWindowState.Normal;
        _metrics = Convert(notification.Metrics, state, ++_generation);
        WindowEvent?.Invoke(new(kind, _metrics));
    }

    private ShellWindowMetrics Convert(NativeWindowEvent value, ShellWindowState state, long generation)
    {
        var logicalSize = new Size(value.LogicalWidth, value.LogicalHeight);
        var physicalSize = new Size(value.PhysicalWidth, value.PhysicalHeight);
        if (_scaleGeneration == 0 || _metrics.ScaleFactor != value.ScaleFactor)
        {
            _scaleGeneration++;
        }
        if (_surfaceGeneration == 0 ||
            _metrics.LogicalClientSize != logicalSize ||
            _metrics.PhysicalClientSize != physicalSize ||
            _metrics.ScaleFactor != value.ScaleFactor)
        {
            _surfaceGeneration++;
        }
        return new(
            logicalSize,
            physicalSize,
            value.ScaleFactor,
            generation,
            _scaleGeneration,
            _surfaceGeneration,
            state);
    }

    private void VerifyAccess()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _platform.VerifyAccess();
    }
}
