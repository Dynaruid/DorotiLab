using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Shell.Core;

namespace Doroti.Vendor.Avalonia.Native;

public static class MacOsShellPlatformFactory
{
    public static IShellWindowingPlatform Create() => new MacOsShellPlatform();
}

internal sealed class MacOsShellPlatform : IShellWindowingPlatform, IShellDispatcher, IShellEventLoop, IDisposable
{
    private readonly int _threadId = Environment.CurrentManagedThreadId;
    private readonly ConcurrentQueue<Action> _posted = new();
    private readonly List<MacOsShellWindow> _windows = [];
    private bool _exitRequested;
    private bool _disposed;

    internal MacOsShellPlatform()
    {
        if (!OperatingSystem.IsMacOS() || RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
        {
            throw new PlatformNotSupportedException("Doroti.Vendor.Avalonia.Native requires macOS arm64.");
        }
        if (!CheckAccess())
        {
            throw new InvalidOperationException("The AppKit shell must be created on its owning main thread.");
        }
        NativeInterop.AppInit();
    }

    public IShellDispatcher Dispatcher => this;
    public IShellEventLoop EventLoop => this;
    public bool CheckAccess() => Environment.CurrentManagedThreadId == _threadId;

    public void VerifyAccess()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!CheckAccess()) throw new InvalidOperationException("AppKit shell access must occur on its owning thread.");
    }

    public void Post(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        _posted.Enqueue(callback);
        NativeInterop.AppWake();
    }

    public IShellWindow CreateWindow(string title, Size initialLogicalClientSize)
    {
        VerifyAccess();
        if (!initialLogicalClientSize.IsFinite || initialLogicalClientSize.IsEmpty) throw new ArgumentOutOfRangeException(nameof(initialLogicalClientSize));
        var window = new MacOsShellWindow(this, title, initialLogicalClientSize);
        _windows.Add(window);
        return window;
    }

    public bool PumpOnce(bool waitForMessage = false)
    {
        VerifyAccess();
        var didWork = false;
        var count = _posted.Count;
        for (var i = 0; i < count && _posted.TryDequeue(out var callback); i++) { callback(); didWork = true; }
        didWork |= NativeInterop.AppPump(waitForMessage ? 1 : 0) != 0;
        _windows.RemoveAll(window => window.IsClosed);
        return didWork;
    }

    public void Run(CancellationToken cancellationToken = default)
    {
        VerifyAccess();
        while (!_exitRequested && !cancellationToken.IsCancellationRequested && _windows.Count > 0) PumpOnce(true);
    }

    public void RequestExit() { _exitRequested = true; NativeInterop.AppWake(); }

    public void Dispose()
    {
        if (_disposed) return;
        VerifyAccess();
        foreach (var window in _windows.ToArray()) window.Dispose();
        _windows.Clear(); _disposed = true;
    }
}

internal sealed class MacOsShellWindow : IShellWindow, IShellInputService, IShellTextInputService,
    IShellClipboardService, IShellCursorService, IShellGraphicsService, IShellFocusService,
    IShellInputTestService, IShellAccessibilityService
{
    private readonly MacOsShellPlatform _platform;
    private readonly NativeInterop.EventCallback _callback;
    private readonly GCHandle _self;
    private nint _native;
    private ShellWindowMetrics _metrics;
    private long _generation;
    private long _scaleGeneration;
    private long _surfaceGeneration;
    private Func<SemanticsActionRequest, bool>? _performAction;
    private int _accessibilityNodeId;
    private bool _disposed;

    internal MacOsShellWindow(MacOsShellPlatform platform, string title, Size size)
    {
        _platform = platform;
        _callback = OnNativeStatic;
        _self = GCHandle.Alloc(this);
        _native = NativeInterop.WindowCreate(title, size.Width, size.Height, _callback, GCHandle.ToIntPtr(_self));
        if (_native == 0) throw new InvalidOperationException("Avalonia.Native AppKit window creation failed.");
        Services = new();
        Services.Add<IShellDispatcher>(platform); Services.Add<IShellEventLoop>(platform);
        Services.Add<IShellInputService>(this); Services.Add<IShellTextInputService>(this);
        Services.Add<IShellClipboardService>(this); Services.Add<IShellCursorService>(this);
        Services.Add<IShellGraphicsService>(this); Services.Add<IShellFocusService>(this);
        Services.Add<IShellInputTestService>(this); Services.Add<IShellAccessibilityService>(this);
        RefreshMetrics(ShellWindowState.Normal);
    }

    public event Action<ShellWindowEvent>? WindowEvent;
    public event Action<RawPointerEvent>? Pointer;
    public event Action<RawKeyEvent>? Key;
    public event Action<ShellTextEvent>? Text;
    public ulong Id { get; private set; }
    public ShellNativeHandle NativeHandle => new(_native == 0 ? 0 : NativeInterop.WindowHandle(_native), "NSWindow");
    public ShellWindowMetrics Metrics => _metrics;
    public ShellPlatformServiceRegistry Services { get; }
    public IReadOnlyList<ShellScreen> Screens
    {
        get
        {
            return NativeInterop.ScreenPrimary(out var id, out var x, out var y, out var width, out var height, out var scale) != 0
                ? [new(id, Rect.FromLeftTopWidthHeight(x, y, width, height), scale)]
                : [];
        }
    }
    public bool IsClosed => _native == 0 || _metrics.State == ShellWindowState.Closed;
    public InputCapabilities Capabilities { get; } = new(true, false, false, true, true, true, true);
    public string BackendIdentity => "skia-nsopengl-opengl-gpu";

    public void Show() { Verify(); NativeInterop.WindowShow(_native); WindowEvent?.Invoke(new(ShellWindowEventKind.Opened, _metrics)); }
    public void Resize(Size size) { Verify(); if (!size.IsFinite || size.IsEmpty) throw new ArgumentOutOfRangeException(nameof(size)); NativeInterop.WindowResize(_native, size.Width, size.Height); }
    public void SetState(ShellWindowState state) { Verify(); if (state is not (ShellWindowState.Normal or ShellWindowState.Minimized)) throw new NotSupportedException(); NativeInterop.WindowMinimize(_native, state == ShellWindowState.Minimized ? 1 : 0); }
    public void MoveToScreen(ulong screenId) { Verify(); if (!Screens.Any(screen => screen.Id == screenId)) throw new ArgumentOutOfRangeException(nameof(screenId)); NativeInterop.WindowMoveToScreen(_native, screenId); }
    public void Close() { Verify(); NativeInterop.WindowClose(_native); }
    public void RequestFocus(bool focused) { Verify(); NativeInterop.WindowFocus(_native, focused ? 1 : 0); }
    public void SetCaretRect(Rect logicalRect) { if (!logicalRect.IsFinite) throw new ArgumentException("Caret rectangle must be finite.", nameof(logicalRect)); NativeInterop.TextCaret(_native, logicalRect.Left, logicalRect.Top, logicalRect.Width, logicalRect.Height); }

    public ValueTask<ClipboardResult> GetTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested(); Verify();
        var value = NativeInterop.ClipboardGet();
        if (value == 0) return ValueTask.FromResult(new ClipboardResult(true, null));
        try { return ValueTask.FromResult(new ClipboardResult(true, Marshal.PtrToStringUTF8(value))); }
        finally { NativeInterop.StringFree(value); }
    }

    public ValueTask<ClipboardResult> SetTextAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(text); cancellationToken.ThrowIfCancellationRequested(); Verify();
        return ValueTask.FromResult(NativeInterop.ClipboardSet(text) != 0
            ? new ClipboardResult(true, text) : new ClipboardResult(false, null, "NSPasteboard rejected UTF-8 text."));
    }

    public void SetCursor(CursorKind cursor)
    {
        Verify();
        NativeInterop.CursorSet(cursor switch
        {
            CursorKind.Click => 1,
            CursorKind.Text or CursorKind.VerticalText => 2,
            CursorKind.Cell or CursorKind.Precise => 3,
            CursorKind.Grab or CursorKind.Grabbing => 4,
            CursorKind.Forbidden or CursorKind.NoDrop => 5,
            _ => 0
        });
    }

    public void Present(ReadOnlySpan<byte> pixels, int width, int height, int rowBytes) =>
        throw new NotSupportedException("The osx-arm64 product shell forbids CPU full-frame presentation.");
    public IOpenGlWindowContext CreateOpenGlContext() { ObjectDisposedException.ThrowIf(_disposed, this); return new MacOsOpenGlContext(_native); }

    public void PostPointerMove(Offset p) => TestPointer(1, p);
    public void PostPointerLeave(Offset p) => TestPointer(5, p);
    public void PostPointerDown(Offset p) => TestPointer(2, p);
    public void PostPointerUp(Offset p) => TestPointer(4, p);
    public void PostPointerTap(Offset p) { TestPointer(2, p); TestPointer(4, p); }
    public void PostPointerDrag(Offset start, Offset end) { TestPointer(2, start); TestPointer(3, end); TestPointer(4, end); }
    public void PostPointerWheel(Offset p, Offset delta) { RequireFinite(p); RequireFinite(delta); NativeInterop.TestPointer(_native, 1, p.X, p.Y, -delta.X, -delta.Y); }
    public void PostPointerCaptureLoss(Offset p) => TestPointer(6, p);
    public void PostKeyboardActivation(uint logicalKey) { NativeInterop.TestKey(_native, 0, logicalKey); NativeInterop.TestKey(_native, 2, logicalKey); }
    public void PostTextInput(string text) { ArgumentNullException.ThrowIfNull(text); NativeInterop.TestText(_native, 0, text); }

    public void Update(SemanticsTreeSnapshot snapshot, Func<SemanticsActionRequest, bool> performAction)
    {
        ArgumentNullException.ThrowIfNull(snapshot); ArgumentNullException.ThrowIfNull(performAction); Verify();
        _performAction = performAction; _accessibilityNodeId = snapshot.Root.Id;
        NativeInterop.AccessibilitySet(_native, snapshot.Root.Id, snapshot.Root.Label ?? "Doroti",
            (snapshot.Root.Actions & (SemanticsAction.Tap | SemanticsAction.Toggle)) != 0 ? 1 : 0);
    }
    public bool InvokeAction(int nodeId, SemanticsAction action, object? arguments = null) => _performAction?.Invoke(new(nodeId, action, arguments)) is true;
    public void Clear() { _performAction = null; _accessibilityNodeId = 0; }

    public void Dispose()
    {
        if (_disposed) return; _platform.VerifyAccess(); _disposed = true; Clear();
        var native = Interlocked.Exchange(ref _native, 0); if (native != 0) NativeInterop.WindowDestroy(native);
        if (_self.IsAllocated) _self.Free();
    }

    private void TestPointer(int phase, Offset p) { RequireFinite(p); NativeInterop.TestPointer(_native, phase, p.X, p.Y, 0, 0); }
    private static void RequireFinite(Offset value) { if (!value.IsFinite) throw new ArgumentException("Input coordinates must be finite."); }
    private void Verify() { ObjectDisposedException.ThrowIf(_disposed, this); _platform.VerifyAccess(); }

    private static void OnNativeStatic(nint context, int kind, int phase, ulong windowId,
        double a, double b, double c, double d, ulong u0, ulong u1, nint text)
    {
        if (GCHandle.FromIntPtr(context).Target is MacOsShellWindow window) window.OnNative(kind, phase, windowId, a, b, c, d, u0, u1, text);
    }

    private void OnNative(int kind, int phase, ulong windowId, double a, double b, double c, double d, ulong u0, ulong u1, nint text)
    {
        if (Id == 0) Id = windowId;
        if (kind is >= 1 and <= 5)
        {
            var state = kind == 5 ? ShellWindowState.Closed : kind == 3 && phase != 0 ? ShellWindowState.Minimized : ShellWindowState.Normal;
            if (kind == 3) SetMetrics(a, b, c, d, u0 / 1000d, state);
            var eventKind = kind switch
            {
                1 => ShellWindowEventKind.Activated,
                2 => ShellWindowEventKind.Deactivated,
                3 => ShellWindowEventKind.MetricsChanged,
                4 => ShellWindowEventKind.CloseRequested,
                _ => ShellWindowEventKind.Closed
            };
            if (kind == 5) _metrics = _metrics with { State = ShellWindowState.Closed };
            WindowEvent?.Invoke(new(eventKind, _metrics)); return;
        }
        if (kind == 6)
        {
            var pointerPhase = phase switch
            {
                0 => PointerPhase.Added,
                1 => PointerPhase.Hover,
                2 => PointerPhase.Down,
                3 => PointerPhase.Move,
                4 => PointerPhase.Up,
                5 => PointerPhase.Removed,
                _ => PointerPhase.Cancelled
            };
            Pointer?.Invoke(new(new(windowId), 1, PointerDeviceKind.Mouse, pointerPhase, new(a, b), checked((uint)u0),
                TimeSpan.FromSeconds(Environment.TickCount64 / 1000d), new(-c, -d), (InputModifiers)u1)); return;
        }
        if (kind == 7)
        {
            Key?.Invoke(new(new(windowId), checked((uint)u0), checked((uint)u0), (KeyPhase)phase,
            TimeSpan.FromSeconds(Environment.TickCount64 / 1000d), (InputModifiers)u1)); return;
        }
        if (kind is >= 8 and <= 11) Text?.Invoke(new((ShellTextEventKind)(kind - 8), Marshal.PtrToStringUTF8(text) ?? string.Empty));
        if (kind == 12 && _accessibilityNodeId != 0) _ = InvokeAction(_accessibilityNodeId, SemanticsAction.Tap);
    }

    private void RefreshMetrics(ShellWindowState state)
    {
        NativeInterop.WindowMetrics(_native, out var width, out var height, out var pixelWidth, out var pixelHeight, out var scale);
        SetMetrics(width, height, pixelWidth, pixelHeight, scale, state);
    }

    private void SetMetrics(double width, double height, double pixelWidth, double pixelHeight, double scale, ShellWindowState state)
    {
        var logical = new Size(width, height); var physical = new Size(pixelWidth, pixelHeight);
        if (_scaleGeneration == 0 || _metrics.ScaleFactor != scale) _scaleGeneration++;
        if (_surfaceGeneration == 0 || _metrics.LogicalClientSize != logical || _metrics.PhysicalClientSize != physical || _metrics.ScaleFactor != scale) _surfaceGeneration++;
        _metrics = new(logical, physical, scale, ++_generation, _scaleGeneration, _surfaceGeneration, state);
    }
}

internal sealed class MacOsOpenGlContext : IOpenGlWindowContext
{
    private nint _context;
    private readonly int _ownerThread = Environment.CurrentManagedThreadId;
    internal MacOsOpenGlContext(nint host)
    {
        _context = NativeInterop.GlCreate(host); if (_context == 0) throw new InvalidOperationException("NSOpenGLContext hardware creation failed.");
        using (MakeCurrent()) { Renderer = Read(NativeInterop.GlRenderer(_context)); Version = Read(NativeInterop.GlVersion(_context)); }
        IsHardwareAccelerated = !Renderer.Contains("Software", StringComparison.OrdinalIgnoreCase);
    }
    public string Renderer { get; }
    public string Version { get; }
    public bool IsHardwareAccelerated { get; }
    public IDisposable MakeCurrent() { Verify(); return new Restore(NativeInterop.GlMakeCurrent(_context)); }
    public void Present() { Verify(); NativeInterop.GlPresent(_context); }
    public void Dispose() { if (_context == 0) return; Verify(); NativeInterop.GlDestroy(_context); _context = 0; }
    private void Verify() { ObjectDisposedException.ThrowIf(_context == 0, this); if (Environment.CurrentManagedThreadId != _ownerThread) throw new InvalidOperationException("NSOpenGLContext is raster-thread affine."); }
    private static string Read(nint value) => Marshal.PtrToStringUTF8(value) ?? "unknown";
    private sealed class Restore(nint previous) : IDisposable { private nint _previous = previous; public void Dispose() { var value = Interlocked.Exchange(ref _previous, 0); NativeInterop.GlRestore(value); } }
}
