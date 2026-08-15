// Doroti adaptation of Avalonia Win32Platform/WindowImpl/Dispatcher lifecycle flow.
// Source identity and selected closure are pinned in migration/avalonia-shell/port-selection.json.
using System.Collections.Concurrent;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Shell.Core;

namespace Doroti.Vendor.Avalonia.Win32;

public static class Win32ShellPlatformFactory
{
    public static IShellWindowingPlatform Create() => new Win32ShellPlatform();
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

internal sealed class Win32ShellWindow : IShellWindow, IShellInputService, IShellTextInputService,
    IShellClipboardService, IShellCursorService, IShellGraphicsService, IShellFocusService,
    IShellInputTestService, IShellAccessibilityService
{
    private readonly Win32ShellPlatform _platform;
    private readonly NativeWindowHost _native;
    private long _generation;
    private long _scaleGeneration;
    private long _surfaceGeneration;
    private ShellWindowMetrics _metrics;
    private readonly Win32AutomationRootProvider _automationProvider;
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
            OnNativePointer,
            OnNativeKey,
            input => Text?.Invoke(new((ShellTextEventKind)input.Kind, input.Text)));
        _metrics = Convert(_native.Metrics, ShellWindowState.Normal, ++_generation);
        _automationProvider = new(_native, title);
        _native.AutomationRequested = _automationProvider.HandleGetObject;
        Services.Add<IShellInputService>(this);
        Services.Add<IShellTextInputService>(this);
        Services.Add<IShellClipboardService>(this);
        Services.Add<IShellCursorService>(this);
        Services.Add<IShellGraphicsService>(this);
        Services.Add<IShellFocusService>(this);
        Services.Add<IShellInputTestService>(this);
        Services.Add<IShellAccessibilityService>(this);
    }

    public event Action<ShellWindowEvent>? WindowEvent;

    public event Action<RawPointerEvent>? Pointer;

    public event Action<RawKeyEvent>? Key;

    public event Action<ShellTextEvent>? Text;

    public InputCapabilities Capabilities { get; } = new(
        Mouse: true,
        Touch: VendorBoundary.TouchDigitizerPresent,
        Pen: VendorBoundary.PenDigitizerPresent,
        Wheel: true,
        PointerCapture: true,
        PhysicalKeys: true,
        TextInput: true);

    public string BackendIdentity => "skia-wgl-opengl-gpu";

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

    public void RequestFocus(bool focused) => _native.RequestFocus(focused);

    public void SetCaretRect(Rect logicalRect)
    {
        var caret = PixelExtentPolicy.ToPixelRect(logicalRect, Metrics.ScaleFactor);
        Win32ImeInterop.SetCandidatePosition(_native.Handle, caret.Left, caret.Bottom);
    }

    public ValueTask<ClipboardResult> GetTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = Win32ClipboardInterop.GetText(_native.Handle);
        return ValueTask.FromResult(new ClipboardResult(result.Success, result.Text, result.Diagnostic));
    }

    public ValueTask<ClipboardResult> SetTextAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(text);
        cancellationToken.ThrowIfCancellationRequested();
        var result = Win32ClipboardInterop.SetText(_native.Handle, text);
        return ValueTask.FromResult(new ClipboardResult(result.Success, result.Text, result.Diagnostic));
    }

    public void SetCursor(CursorKind cursor) => _native.SetCursor(cursor switch
    {
        CursorKind.Basic => NativeInterop.CursorArrow,
        CursorKind.Click => NativeInterop.CursorHand,
        CursorKind.Forbidden or CursorKind.NoDrop => NativeInterop.CursorNo,
        CursorKind.Wait => NativeInterop.CursorWait,
        CursorKind.Progress => NativeInterop.CursorAppStarting,
        CursorKind.Help => NativeInterop.CursorHelp,
        CursorKind.Text or CursorKind.VerticalText => NativeInterop.CursorIBeam,
        CursorKind.Cell or CursorKind.Precise or CursorKind.ZoomIn or CursorKind.ZoomOut => NativeInterop.CursorCross,
        CursorKind.Move or CursorKind.Grab or CursorKind.Grabbing or CursorKind.AllScroll => NativeInterop.CursorSizeAll,
        CursorKind.ResizeLeftRight or CursorKind.ResizeLeft or CursorKind.ResizeRight or CursorKind.ResizeColumn => NativeInterop.CursorSizeWestEast,
        CursorKind.ResizeUpDown or CursorKind.ResizeUp or CursorKind.ResizeDown or CursorKind.ResizeRow => NativeInterop.CursorSizeNorthSouth,
        CursorKind.ResizeUpLeftDownRight or CursorKind.ResizeUpLeft or CursorKind.ResizeDownRight => NativeInterop.CursorSizeNorthWestSouthEast,
        CursorKind.ResizeUpRightDownLeft or CursorKind.ResizeUpRight or CursorKind.ResizeDownLeft => NativeInterop.CursorSizeNorthEastSouthWest,
        CursorKind.Hidden => 0,
        _ => NativeInterop.CursorArrow,
    });

    public void Present(ReadOnlySpan<byte> pixels, int width, int height, int rowBytes) =>
        _native.Present(pixels, width, height, rowBytes);

    public IOpenGlWindowContext CreateOpenGlContext() => new Win32ShellOpenGlContext(new NativeOpenGlContext(_native.Handle));

    public void PostPointerMove(Offset value) => _native.PostPointerMove(value.X, value.Y);
    public void PostPointerLeave(Offset value) => _native.PostPointerLeave(value.X, value.Y);
    public void PostPointerDown(Offset value) => _native.PostPointerDown(value.X, value.Y);
    public void PostPointerUp(Offset value) => _native.PostPointerUp(value.X, value.Y);
    public void PostPointerTap(Offset value) => _native.PostPointerTap(value.X, value.Y);
    public void PostPointerDrag(Offset start, Offset end) => _native.PostPointerDrag(start.X, start.Y, end.X, end.Y);
    public void PostPointerWheel(Offset value, Offset delta) => _native.PostPointerWheel(value.X, value.Y, delta.X, delta.Y);
    public void PostPointerCaptureLoss(Offset value) => _native.PostPointerCaptureLoss(value.X, value.Y);
    public void PostKeyboardActivation(uint logicalKey) => _native.PostKeyboardActivation(logicalKey);
    public void PostTextInput(string text) => _native.PostTextInput(text);

    public void Update(SemanticsTreeSnapshot snapshot, Func<SemanticsActionRequest, bool> performAction) =>
        _automationProvider.Update(snapshot, performAction);

    public bool InvokeAction(int nodeId, SemanticsAction action, object? arguments = null) =>
        _automationProvider.Invoke(new(nodeId, action, arguments));

    public void Clear() => _automationProvider.Clear();

    internal void PumpPendingMessages() => _native.PumpPendingMessages();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        VerifyAccess();
        _disposed = true;
        _automationProvider.Clear();
        _native.AutomationRequested = null;
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

    private void OnNativePointer(NativePointerEvent native)
    {
        var phase = native.Phase switch
        {
            NativePointerPhase.Added => PointerPhase.Added,
            NativePointerPhase.Hover => PointerPhase.Hover,
            NativePointerPhase.Down => PointerPhase.Down,
            NativePointerPhase.Move => PointerPhase.Move,
            NativePointerPhase.Up => PointerPhase.Up,
            NativePointerPhase.Removed => PointerPhase.Removed,
            NativePointerPhase.Cancelled => PointerPhase.Cancelled,
            NativePointerPhase.Wheel => PointerPhase.Hover,
            _ => throw new InvalidOperationException($"Unknown pointer phase {native.Phase}."),
        };
        Pointer?.Invoke(new(
            new(native.WindowId), native.DeviceId, (PointerDeviceKind)native.DeviceKind, phase,
            new(native.LogicalX, native.LogicalY), native.Buttons,
            TimeSpan.FromMilliseconds(native.TimestampMilliseconds),
            PointerScrollNormalizer.Normalize(new(native.WheelDeltaX, native.WheelDeltaY), PlatformScrollConvention.WindowsWheel, VendorBoundary.ReadWheelScrollLines()),
            (InputModifiers)native.Modifiers));
    }

    private void OnNativeKey(NativeKeyEvent native) => Key?.Invoke(new(
        new(native.WindowId), native.ScanCode, native.VirtualKey, (KeyPhase)native.Phase,
        TimeSpan.FromMilliseconds(native.TimestampMilliseconds), (InputModifiers)native.Modifiers));

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

internal sealed class Win32ShellOpenGlContext(NativeOpenGlContext inner) : IOpenGlWindowContext
{
    public string Renderer => inner.Renderer;
    public string Version => inner.Version;
    public bool IsHardwareAccelerated => inner.IsHardwareAccelerated;
    public IDisposable MakeCurrent() => inner.MakeCurrent();
    public void Present() => inner.Present();
    public void Dispose() => inner.Dispose();
}
