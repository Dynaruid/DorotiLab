using Doroti.Core;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Shell.Core;
using Doroti.Vendor.Avalonia.Base;

namespace Doroti.Host.Desktop;

/// <summary>Creates native desktop windows through an injected source-ported shell.</summary>
public sealed class DesktopWindowBackend : IWindowBackend, IDisposable
{
    private readonly List<DesktopWindow> _windows = [];
    private readonly NativeResourceTracker _resources = new();
    private readonly IShellWindowingPlatform _shell;
    private bool _disposed;

    public DesktopWindowBackend()
        : this(CreateCurrentPlatform())
    {
    }

    public DesktopWindowBackend(IShellWindowingPlatform shell) =>
        _shell = shell ?? throw new ArgumentNullException(nameof(shell));

    public void Post(Action callback) => _shell.Dispatcher.Post(callback);

    /// <summary>Returns the process-owned native window/context counts, including after the last window closes.</summary>
    public NativeResourceSnapshot CaptureResourceSnapshot()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        return _resources.Snapshot;
    }

    public IWindow CreateWindow(WindowConfiguration configuration, IWindowEventSink eventSink)
    {
        ArgumentNullException.ThrowIfNull(eventSink);
        if (!configuration.InitialSize.IsFinite || configuration.InitialSize.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(configuration), "The initial window size must be finite and positive.");
        }

        ObjectDisposedException.ThrowIf(_disposed, this);
        var window = new DesktopWindow(_shell, configuration, eventSink, RemoveWindow, _resources);
        _windows.Add(window);
        return window;
    }

    /// <summary>Dispatches currently queued native messages and rethrows callback failures on the UI thread.</summary>
    public void PumpPendingMessages() => _shell.EventLoop.PumpOnce();

    public void RunEventLoop(CancellationToken cancellationToken = default) => _shell.EventLoop.Run(cancellationToken);

    public void RequestExit() => _shell.EventLoop.RequestExit();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        foreach (var window in _windows.ToArray())
        {
            window.Dispose();
        }
        _windows.Clear();
        if (_shell is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void RemoveWindow(DesktopWindow window) => _windows.Remove(window);

    private static IShellWindowingPlatform CreateCurrentPlatform()
    {
        var (assemblyName, typeName) = OperatingSystem.IsWindows()
            ? ("Doroti.Vendor.Avalonia.Win32", "Doroti.Vendor.Avalonia.Win32.Win32ShellPlatformFactory")
            : OperatingSystem.IsMacOS()
                ? ("Doroti.Vendor.Avalonia.Native", "Doroti.Vendor.Avalonia.Native.MacOsShellPlatformFactory")
                : throw new PlatformNotSupportedException("Doroti desktop supports Windows and macOS source-ported shells.");
        var type = System.Reflection.Assembly.Load(assemblyName).GetType(typeName, throwOnError: true)!;
        return (IShellWindowingPlatform)(type.GetMethod("Create", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.Invoke(null, null)
            ?? throw new MissingMethodException(typeName, "Create"));
    }
}

internal sealed class DesktopWindow : IWindow, IRawInputSource, ITextInputConnection, ITextInputGeometry, ICursorController, IClipboard, IBgra8888FramebufferTarget, IOpenGlWindowTarget, IWindowPlacementController, IWindowFocusController, IWindowInputTestController, IWindowCoordinateDiagnostics, INativeResourceDiagnostics, INativeWindowHandleDiagnostics, IAccessibilityBridge, IAccessibilityDiagnostics
{
    private readonly IWindowEventSink _windowSink;
    private readonly Action<DesktopWindow> _onClosed;
    private readonly List<IRawInputSink> _rawInputSinks = [];
    private readonly IShellWindow _shellWindow;
    private readonly IShellInputService _input;
    private readonly IShellTextInputService _text;
    private readonly IShellClipboardService _clipboard;
    private readonly IShellCursorService _cursor;
    private readonly IShellGraphicsService _graphics;
    private readonly IShellFocusService _focus;
    private readonly IShellInputTestService _inputTest;
    private readonly IShellAccessibilityService _accessibility;
    private readonly NativeResourceTracker _resources;
    private readonly DesktopFrameDispatcher _frameDispatcher;
    private ITextInputClient? _textClient;
    private TextEditingState _editingState = new(string.Empty, new(0, 0), null);
    private TextEditingState _compositionBaseState = new(string.Empty, new(0, 0), null);
    private bool _compositionActive;
    private bool _compositionCommitted;
    private Rect _caretRect;
    private Offset? _lastPointer;
    private int _windowCounted;
    private bool _disposed;

    internal DesktopWindow(
        IShellWindowingPlatform shell,
        WindowConfiguration configuration,
        IWindowEventSink windowSink,
        Action<DesktopWindow> onClosed,
        NativeResourceTracker resources)
    {
        _windowSink = windowSink;
        _onClosed = onClosed;
        _resources = resources;
        _shellWindow = shell.CreateWindow(configuration.Title, configuration.InitialSize);
        _input = _shellWindow.Services.GetRequired<IShellInputService>();
        _text = _shellWindow.Services.GetRequired<IShellTextInputService>();
        _clipboard = _shellWindow.Services.GetRequired<IShellClipboardService>();
        _cursor = _shellWindow.Services.GetRequired<IShellCursorService>();
        _graphics = _shellWindow.Services.GetRequired<IShellGraphicsService>();
        _focus = _shellWindow.Services.GetRequired<IShellFocusService>();
        _inputTest = _shellWindow.Services.GetRequired<IShellInputTestService>();
        _accessibility = _shellWindow.Services.GetRequired<IShellAccessibilityService>();
        _frameDispatcher = new(shell.Dispatcher);
        _shellWindow.WindowEvent += OnWindowEvent;
        _input.Pointer += OnPointer;
        _input.Key += OnKey;
        _text.Text += OnText;
        _resources.WindowCreated();
        _windowCounted = 1;
    }

    public WindowId Id => new(_shellWindow.Id);

    public nint Handle => _shellWindow.NativeHandle.Value;

    public WindowMetrics Metrics => DesktopAdapterBoundary.Convert(_shellWindow.Metrics);

    public bool IsClosed => _shellWindow.Metrics.State == ShellWindowState.Closed;

    public IRawInputSource RawInput => this;

    public InputCapabilities Capabilities => _input.Capabilities;

    public ITextInputConnection TextInput => this;

    public ICursorController Cursor => this;

    public IReadOnlyList<DisplayInfo> Displays => _shellWindow.Screens.Select(DesktopAdapterBoundary.Convert).ToArray();

    public NativeResourceSnapshot Snapshot => _resources.Snapshot;

    public WindowCoordinateSnapshot Coordinates
    {
        get
        {
            var metrics = Metrics;
            var caretPixels = PixelExtentPolicy.ToPixelRect(_caretRect, metrics.ScaleFactor);
            return new(
                metrics.Generation,
                metrics.LogicalSize,
                metrics.PixelSize,
                _lastPointer,
                _lastPointer is { } pointer ? PixelExtentPolicy.ToPhysicalPoint(pointer, metrics.ScaleFactor) : null,
                _caretRect,
                new(caretPixels.Left, caretPixels.Top, caretPixels.Right, caretPixels.Bottom));
        }
    }

    public SemanticsTreeSnapshot? LastSnapshot { get; private set; }

    public void Show() => _shellWindow.Show();

    public void Resize(Size logicalSize)
    {
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(logicalSize), "The logical window size must be finite and positive.");
        }
        _shellWindow.Resize(logicalSize);
    }

    public void SetMinimized(bool minimized) => _shellWindow.SetState(minimized ? ShellWindowState.Minimized : ShellWindowState.Normal);

    public void MoveToDisplay(DisplayId display) => _shellWindow.MoveToScreen(display.Value);

    public void RequestFocus(bool focused) => _focus.RequestFocus(focused);

    public void PostPointerTap(Offset logicalPosition)
    {
        if (!logicalPosition.IsFinite)
        {
            throw new ArgumentException("Pointer validation position must be finite.", nameof(logicalPosition));
        }
        _inputTest.PostPointerTap(logicalPosition);
    }

    public void PostPointerMove(Offset logicalPosition)
    {
        RequireFinite(logicalPosition);
        _inputTest.PostPointerMove(logicalPosition);
    }

    public void PostPointerLeave(Offset logicalPosition)
    {
        RequireFinite(logicalPosition);
        _inputTest.PostPointerLeave(logicalPosition);
    }

    public void PostPointerDown(Offset logicalPosition)
    {
        RequireFinite(logicalPosition);
        _inputTest.PostPointerDown(logicalPosition);
    }

    public void PostPointerUp(Offset logicalPosition)
    {
        RequireFinite(logicalPosition);
        _inputTest.PostPointerUp(logicalPosition);
    }

    public void PostPointerDrag(Offset logicalStart, Offset logicalEnd)
    {
        if (!logicalStart.IsFinite || !logicalEnd.IsFinite)
        {
            throw new ArgumentException("Pointer validation positions must be finite.");
        }
        _inputTest.PostPointerDrag(logicalStart, logicalEnd);
    }

    public void PostPointerWheel(Offset logicalPosition, Offset wheelDelta)
    {
        if (!logicalPosition.IsFinite || !wheelDelta.IsFinite)
        {
            throw new ArgumentException("Pointer validation position and delta must be finite.");
        }
        _inputTest.PostPointerWheel(logicalPosition, wheelDelta);
    }

    public void PostPointerCaptureLoss(Offset logicalPosition)
    {
        if (!logicalPosition.IsFinite)
        {
            throw new ArgumentException("Pointer validation position must be finite.", nameof(logicalPosition));
        }
        _inputTest.PostPointerCaptureLoss(logicalPosition);
    }

    public void PostKeyboardActivation(uint logicalKey) => _inputTest.PostKeyboardActivation(logicalKey);

    public void PostTextInput(string text) => _inputTest.PostTextInput(text);

    public void Close() => _shellWindow.Close();

    public bool TryGetFeature<TFeature>(out TFeature? feature)
        where TFeature : class
    {
        feature = this as TFeature ?? _frameDispatcher as TFeature ?? _graphics as TFeature;
        return feature is not null;
    }

    public void Attach(IRawInputSink sink)
    {
        ArgumentNullException.ThrowIfNull(sink);
        if (!_rawInputSinks.Contains(sink))
        {
            _rawInputSinks.Add(sink);
        }
    }

    public void Detach(IRawInputSink sink) => _rawInputSinks.Remove(sink);

    public void SetClient(ITextInputClient client, TextEditingState initialState)
    {
        ArgumentNullException.ThrowIfNull(client);
        _textClient = client;
        _editingState = initialState;
        _compositionBaseState = initialState;
        _compositionActive = false;
        _compositionCommitted = false;
    }

    public void UpdateState(TextEditingState state) => _editingState = state;

    public void ClearClient()
    {
        _textClient = null;
        _compositionActive = false;
        _compositionCommitted = false;
    }

    public void SetCaretRect(Rect logicalRect)
    {
        if (!logicalRect.IsFinite)
        {
            throw new ArgumentException("Caret rectangle must be finite.", nameof(logicalRect));
        }
        _caretRect = logicalRect;
        _text.SetCaretRect(logicalRect);
    }

    public void Update(SemanticsTreeSnapshot snapshot, Func<SemanticsActionRequest, bool> performAction)
    {
        LastSnapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        _accessibility.Update(snapshot, performAction);
    }

    public bool InvokeAction(int nodeId, SemanticsAction action, object? arguments = null) =>
        _accessibility.InvokeAction(nodeId, action, arguments);

    public ValueTask<ClipboardResult> GetTextAsync(CancellationToken cancellationToken = default)
    {
        return _clipboard.GetTextAsync(cancellationToken);
    }

    public ValueTask<ClipboardResult> SetTextAsync(string text, CancellationToken cancellationToken = default)
    {
        return _clipboard.SetTextAsync(text, cancellationToken);
    }

    public void SetCursor(WindowId window, CursorKind cursor)
    {
        if (window != Id)
        {
            throw new ArgumentException("The cursor target does not belong to this window.", nameof(window));
        }
        _cursor.SetCursor(cursor);
    }

    private static void RequireFinite(Offset logicalPosition)
    {
        if (!logicalPosition.IsFinite)
        {
            throw new ArgumentException("Pointer validation position must be finite.", nameof(logicalPosition));
        }
    }

    public void Present(ReadOnlySpan<byte> pixels, int width, int height, int rowBytes) =>
        _graphics.Present(pixels, width, height, rowBytes);

    public IOpenGlWindowContext CreateContext() => new ResourceTrackingOpenGlContext(_graphics.CreateOpenGlContext(), _resources);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _shellWindow.WindowEvent -= OnWindowEvent;
        _input.Pointer -= OnPointer;
        _input.Key -= OnKey;
        _text.Text -= OnText;
        _accessibility.Clear();
        _frameDispatcher.Dispose();
        _shellWindow.Dispose();
        ReleaseWindowCount();
        _rawInputSinks.Clear();
        _textClient = null;
        LastSnapshot = null;
    }

    private void OnWindowEvent(ShellWindowEvent notification)
    {
        var metrics = DesktopAdapterBoundary.Convert(notification.Metrics);
        switch (notification.Kind)
        {
            case ShellWindowEventKind.Opened:
            case ShellWindowEventKind.MetricsChanged:
                _windowSink.OnMetricsChanged(Id, metrics);
                break;
            case ShellWindowEventKind.Activated:
            case ShellWindowEventKind.Deactivated:
                var focus = new RawFocusEvent(
                    Id,
                    notification.Kind == ShellWindowEventKind.Activated,
                    TimeSpan.FromMilliseconds(Environment.TickCount64));
                foreach (var sink in _rawInputSinks.ToArray())
                {
                    sink.OnFocus(focus);
                }
                break;
            case ShellWindowEventKind.CaptureCancelled:
                break;
            case ShellWindowEventKind.CloseRequested:
                _windowSink.OnCloseRequested(Id);
                break;
            case ShellWindowEventKind.Closed:
                ReleaseWindowCount();
                _windowSink.OnClosed(Id);
                _onClosed(this);
                break;
            default:
                throw new InvalidOperationException($"Unknown shell window notification: {notification.Kind}");
        }
    }

    private void ReleaseWindowCount()
    {
        if (Interlocked.Exchange(ref _windowCounted, 0) != 0)
        {
            _resources.WindowReleased();
        }
    }

    private void OnPointer(RawPointerEvent input)
    {
        if (input.Phase is not (PointerPhase.Removed or PointerPhase.Cancelled))
        {
            _lastPointer = input.Position;
        }
        foreach (var sink in _rawInputSinks.ToArray())
        {
            sink.OnPointer(input);
        }
    }

    private void OnKey(RawKeyEvent input)
    {
        foreach (var sink in _rawInputSinks.ToArray())
        {
            sink.OnKey(input);
        }
    }

    private void OnText(ShellTextEvent native)
    {
        if (_textClient is null)
        {
            return;
        }
        if (native.Kind is ShellTextEventKind.CompositionStarted)
        {
            _compositionBaseState = _editingState;
            _editingState = TextEditingStateReducer.BeginComposition(_editingState);
            _compositionActive = true;
            _compositionCommitted = false;
        }
        else if (native.Kind is ShellTextEventKind.Text && native.Text.Length > 0)
        {
            _editingState = TextEditingStateReducer.CommitText(_editingState, native.Text);
            _compositionCommitted = _compositionActive;
            _textClient.UpdateEditingState(_editingState);
        }
        else if (native.Kind is ShellTextEventKind.CompositionUpdated)
        {
            if (!_compositionActive)
            {
                _compositionBaseState = _editingState;
                _editingState = TextEditingStateReducer.BeginComposition(_editingState);
                _compositionActive = true;
            }
            _editingState = TextEditingStateReducer.UpdateComposition(_editingState, native.Text);
            _textClient.UpdateEditingState(_editingState);
        }
        else if (native.Kind is ShellTextEventKind.CompositionEnded)
        {
            _editingState = _compositionActive && !_compositionCommitted
                ? TextEditingStateReducer.CancelComposition(_compositionBaseState)
                : _editingState with { ComposingRange = null };
            _compositionActive = false;
            _compositionCommitted = false;
            _textClient.UpdateEditingState(_editingState);
        }
    }
}

/// <summary>
/// Coalesces engine frame requests onto the pinned Avalonia sleep-loop render timer and then
/// returns callbacks to the source-ported shell UI thread.
/// </summary>
internal sealed class DesktopFrameDispatcher : IFrameDispatcher, IDisposable
{
    private readonly object _gate = new();
    private readonly IShellDispatcher _dispatcher;
    private readonly SleepLoopRenderTimer _timer = new(60);
    private readonly Queue<Action<TimeSpan>> _callbacks = [];
    private bool _tickRequested;
    private bool _uiPostPending;
    private bool _timerActive;
    private bool _disposed;

    internal DesktopFrameDispatcher(IShellDispatcher dispatcher) =>
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _callbacks.Enqueue(callback);
            if (_tickRequested || _uiPostPending)
            {
                return;
            }
            _tickRequested = true;
            if (!_timerActive)
            {
                _timerActive = true;
                _timer.Tick = OnTick;
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _callbacks.Clear();
        }
        _timer.Dispose();
    }

    private void OnTick(TimeSpan timestamp)
    {
        lock (_gate)
        {
            if (_disposed || !_tickRequested)
            {
                return;
            }
            _tickRequested = false;
            _uiPostPending = true;
        }
        _dispatcher.Post(() => Drain(timestamp));
    }

    private void Drain(TimeSpan timestamp)
    {
        Action<TimeSpan>[] callbacks;
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }
            callbacks = _callbacks.ToArray();
            _callbacks.Clear();
            _uiPostPending = false;
        }
        foreach (var callback in callbacks)
        {
            callback(timestamp);
        }
        lock (_gate)
        {
            if (!_disposed && !_tickRequested && !_uiPostPending && _callbacks.Count == 0)
            {
                _timer.Tick = null;
                _timerActive = false;
            }
        }
    }
}

internal sealed class ResourceTrackingOpenGlContext : IOpenGlWindowContext
{
    private readonly IOpenGlWindowContext _context;
    private readonly NativeResourceTracker _resources;
    private int _counted = 1;

    internal ResourceTrackingOpenGlContext(IOpenGlWindowContext context, NativeResourceTracker resources)
    {
        _context = context;
        _resources = resources;
        _resources.OpenGlContextCreated();
    }

    public string Renderer => _context.Renderer;

    public string Version => _context.Version;

    public bool IsHardwareAccelerated => _context.IsHardwareAccelerated;

    public IDisposable MakeCurrent() => _context.MakeCurrent();

    public void Present() => _context.Present();

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _counted, 0) == 0)
        {
            return;
        }
        _context.Dispose();
        _resources.OpenGlContextReleased();
    }
}

internal sealed class NativeResourceTracker : INativeResourceDiagnostics
{
    private long _windowsCreated;
    private long _windowsReleased;
    private long _openGlContextsCreated;
    private long _openGlContextsReleased;

    public NativeResourceSnapshot Snapshot
    {
        get
        {
            var windowsCreated = Interlocked.Read(ref _windowsCreated);
            var windowsReleased = Interlocked.Read(ref _windowsReleased);
            var contextsCreated = Interlocked.Read(ref _openGlContextsCreated);
            var contextsReleased = Interlocked.Read(ref _openGlContextsReleased);
            return new(
                checked((int)(windowsCreated - windowsReleased)),
                windowsCreated,
                windowsReleased,
                checked((int)(contextsCreated - contextsReleased)),
                contextsCreated,
                contextsReleased);
        }
    }

    internal void WindowCreated() => Interlocked.Increment(ref _windowsCreated);

    internal void WindowReleased() => Interlocked.Increment(ref _windowsReleased);

    internal void OpenGlContextCreated() => Interlocked.Increment(ref _openGlContextsCreated);

    internal void OpenGlContextReleased() => Interlocked.Increment(ref _openGlContextsReleased);
}
