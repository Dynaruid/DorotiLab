using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;
using Rect = Doroti.Ui.Rect;
using Size = Doroti.Ui.Size;

namespace Doroti.Host.WindowsAppSdk;

internal sealed class WindowsAppSdkHostAdapter :
    IViewHostCapability,
    IFrameHostCapability,
    IInputHostCapability,
    IViewFocusRequestCapability,
    ITextInputHostCapability,
    IPlatformEnvironmentHostCapability,
    IPlatformServicesHostCapability,
    ISkiaSceneRendererHost
{
    private const uint WmClose = 0x0010;
    private const uint WmDestroy = 0x0002;
    private const uint WmCancelMode = 0x001F;
    private const uint WmSetCursor = 0x0020;
    private const uint WmEraseBackground = 0x0014;
    private const uint WmSetFocus = 0x0007;
    private const uint WmKillFocus = 0x0008;
    private const uint WmKeyDown = 0x0100;
    private const uint WmKeyUp = 0x0101;
    private const uint WmChar = 0x0102;
    private const uint WmSysKeyDown = 0x0104;
    private const uint WmSysKeyUp = 0x0105;
    private const uint WmImeStartComposition = 0x010D;
    private const uint WmImeEndComposition = 0x010E;
    private const uint WmImeComposition = 0x010F;
    private const uint WmMouseMove = 0x0200;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmLeftButtonUp = 0x0202;
    private const uint WmLeftButtonDoubleClick = 0x0203;
    private const uint WmRightButtonDown = 0x0204;
    private const uint WmRightButtonUp = 0x0205;
    private const uint WmMiddleButtonDown = 0x0207;
    private const uint WmMiddleButtonUp = 0x0208;
    private const uint WmMouseWheel = 0x020A;
    private const uint WmMouseHorizontalWheel = 0x020E;
    private const uint WmCaptureChanged = 0x0215;
    private const uint WmMouseLeave = 0x02A3;
    private const uint WmAppFrame = 0x8001;
    private const uint WmAppCommitted = 0x8002;
    private const uint WsPopup = 0x80000000;
    private const uint WsExAppWindow = 0x00040000;
    private const uint WsExNoRedirectionBitmap = 0x00200000;
    private const int SwShow = 5;
    private const int SwMinimize = 6;
    private const int GcsCompStr = 0x0008;
    private const int GcsResultStr = 0x0800;
    private const uint CfUnicodeText = 13;
    private const uint GmemMoveable = 0x0002;
    private const int MinimumWidth = 320;
    private const int MinimumContentHeight = 240;
    private const int ResizeBorderLogical = 8;
    private const int TitleBarLogicalHeight = 36;
    private const int CaptionButtonLogicalWidth = 46;
    private const int MouseWheelDelta = 120;
    private const uint TrackMouseEventLeave = 0x00000002;
    private const uint ClassDoubleClicks = 0x0008;
    private const string WindowClassName = "Doroti.WindowsAppSdk.RawArmN";
    private static readonly ConcurrentDictionary<nint, WindowsAppSdkHostAdapter> Instances = new();
    private static readonly NativeMethods.WindowProcedure WindowProcedure = StaticWindowProcedure;
    private static readonly object ClassGate = new();
    private static ushort _windowClass;

    private readonly ulong _viewId;
    private readonly DorotiViewConfiguration _configuration;
    private readonly object _stateGate = new();
    private readonly object _smokeGate = new();
    private readonly AutoResetEvent _renderSignal = new(false);
    private readonly AutoResetEvent _frameSignal = new(false);
    private readonly ManualResetEventSlim _firstCommit = new(false);
    private readonly Thread _renderThread;
    private readonly Thread _frameThread;
    private readonly NativeMethods.NativeRect _workArea;
    private readonly nint _hwnd;
    private readonly WindowsAppSdkIslandBridge _islandBridge;
    private readonly ArmNDualFrontPresenter _presenter;
    private SkiaSceneRenderer? _renderer;
    private Action<TimeSpan>? _pendingFrame;
    private TimeSpan _pendingFrameTimestamp;
    private GeometrySnapshot _geometry;
    private GeometrySnapshot? _pendingRender;
    private NativeMethods.NativePoint _captureOrigin;
    private GeometrySnapshot _captureGeometry;
    private ResizeEdges _resizeEdges;
    private CaptureKind _captureKind;
    private ChromeHit _hoverChrome;
    private ChromeHit _pressedChrome;
    private GeometrySnapshot? _restoreGeometry;
    private NativeMethods.NativePoint _lastPointerPoint;
    private DorotiTextInputConfiguration? _textConfiguration;
    private DorotiTextEditingState _editingState = new(
        string.Empty, new DorotiTextSelection(0, 0), null);
    private Rect _caretRect = Rect.zero;
    private long _lastCommittedEpoch;
    private long _inputSequence;
    private long _frameRequests;
    private long _frameRequestsCoalesced;
    private long _vsyncCallbacks;
    private long _pointerAdds;
    private long _pointerRemoves;
    private long _wheelSignals;
    private long _captionMoves;
    private long _edgeResizes;
    private long _buttons;
    private double _scrollOffsetMultiplier;
    private DorotiMouseCursorKind _clientCursor = DorotiMouseCursorKind.basic;
    private bool _pointerAdded;
    private bool _trackingMouseLeave;
    private bool _hasLastPointerPoint;
    private bool _releasingCapture;
    private bool _isMaximized;
    private bool _imeComposing;
    private bool _shown;
    private bool _closing;
    private bool _disposed;
    private Action<int, SemanticsAction, object?>? _semanticsAction;

    internal WindowsAppSdkHostAdapter(
        ulong viewId,
        DorotiViewConfiguration configuration)
    {
        _viewId = viewId;
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        EnsureWindowClass();
        _workArea = NativeMethods.GetPrimaryWorkArea();
        var initialDpi = NativeMethods.GetDpiForSystem();
        var scale = Math.Max(1.0, initialDpi / 96.0);
        var width = Math.Clamp(
            checked((int)Math.Round(configuration.logicalSize.width * scale)),
            MinimumWidth,
            _workArea.Width);
        var titleBarHeight = TitleBarHeight(scale);
        var height = Math.Clamp(
            checked((int)Math.Round(configuration.logicalSize.height * scale)) + titleBarHeight,
            MinimumContentHeight + titleBarHeight,
            _workArea.Height);
        var offsetX = Math.Max(0, (_workArea.Width - width) / 2);
        var offsetY = Math.Max(0, (_workArea.Height - height) / 2);
        _geometry = new(1, offsetX, offsetY, width, height, scale);
        _scrollOffsetMultiplier = NativeMethods.GetScrollOffsetMultiplier();
        _hwnd = NativeMethods.CreateWindowExW(
            WsExAppWindow | WsExNoRedirectionBitmap,
            WindowClassName,
            configuration.title,
            WsPopup,
            _workArea.Left,
            _workArea.Top,
            _workArea.Width,
            _workArea.Height,
            0,
            0,
            NativeMethods.GetModuleHandleW(null),
            0);
        if (_hwnd == 0)
            throw new InvalidOperationException(
                $"CreateWindowExW failed: {Marshal.GetLastWin32Error()}");
        Instances[_hwnd] = this;
        ApplyWindowRegion(_geometry);

        _islandBridge = new(_hwnd, _workArea.Width, _workArea.Height);
        _presenter = new(_hwnd, _workArea.Width, _workArea.Height);
        _presenter.Committed += HandlePresenterCommitted;
        _presenter.StageGeometry(
            _geometry.Epoch,
            _geometry.OffsetX,
            _geometry.OffsetY,
            _geometry.Width,
            _geometry.Height);
        _renderThread = new(RenderLoop)
        {
            IsBackground = true,
            Name = "Doroti WindowsAppSdk Arm N raster",
        };
        _renderThread.SetApartmentState(ApartmentState.MTA);
        _renderThread.Start();
        _frameThread = new(FrameLoop)
        {
            IsBackground = true,
            Name = "Doroti WindowsAppSdk DWM vsync",
        };
        _frameThread.SetApartmentState(ApartmentState.MTA);
        _frameThread.Start();
    }

    internal nint Hwnd => _hwnd;

    internal bool IslandConnected => _islandBridge.IsConnected;

    public ViewMetrics Metrics
    {
        get
        {
            var geometry = SnapshotGeometry();
            return ToMetrics(geometry);
        }
    }

    public DorotiViewEpoch ViewEpoch
    {
        get
        {
            var value = SnapshotGeometry();
            var contentHeight = ContentHeight(value);
            return new(
                1,
                value.Epoch,
                value.Epoch,
                value.Width / value.Scale,
                contentHeight / value.Scale,
                value.Width,
                contentHeight,
                value.Scale,
                value.Scale,
                DorotiFrameClock.Now.Ticks / 10);
        }
    }

    public long InputSequence => Volatile.Read(ref _inputSequence);

    public long SurfaceGeneration => 1;

    public DorotiResizeEpoch ResizeTarget
    {
        get
        {
            var value = SnapshotGeometry();
            var contentHeight = ContentHeight(value);
            return new(
                value.Epoch,
                value.Width / value.Scale,
                contentHeight / value.Scale,
                value.Width,
                contentHeight,
                value.Scale,
                DorotiFrameClock.Now.Ticks / 10);
        }
    }

    public PlatformConfiguration Configuration => new(
        [ToLocale(CultureInfo.CurrentUICulture)],
        Brightness.light,
        false,
        false,
        HostOperatingSystem.windows);

    public event Action<ViewMetrics>? MetricsChanged;
    public event Action<AppLifecycleState>? LifecycleChanged;
    public event Action? CloseRequested;
    public event Action? Closed;
    public event Action<PointerDataPacket>? PointerData;
    public event Action<KeyData>? KeyData;
    public event Action<RawFocusData>? FocusData;
    public event Action<DorotiTextEditingState>? EditingStateChanged;
    public event Action<DorotiTextInputAction>? ActionPerformed;
    public event Action<PlatformConfiguration>? ConfigurationChanged;
    public event Action<int, SemanticsAction, object?>? SemanticsAction
    {
        add => _semanticsAction += value;
        remove => _semanticsAction -= value;
    }
    public event Action<long, TimeSpan>? InputReceived;

    internal void AttachRenderer(SkiaSceneRenderer renderer)
    {
        ArgumentNullException.ThrowIfNull(renderer);
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (Interlocked.CompareExchange(ref _renderer, renderer, null) is not null)
            throw new InvalidOperationException("The Windows App SDK renderer is already attached.");
        renderer.AttachSurface(RequestInvalidate);
        RequestInvalidate();
    }

    internal int RunMessageLoop()
    {
        NativeMethods.NativeMessage message;
        while (NativeMethods.GetMessageW(out message, 0, 0, 0) > 0)
        {
            NativeMethods.TranslateMessage(in message);
            NativeMethods.DispatchMessageW(in message);
        }
        return checked((int)message.WParam);
    }

    internal void WriteDiagnostics(SkiaFrameDiagnostics diagnostics)
    {
        var geometry = SnapshotGeometry();
        Console.Error.WriteLine(
            $"doroti.windowsappsdk.summary=" +
            $"islandWasConnected={_islandBridge.WasConnected};islandConnected={_islandBridge.IsConnected};" +
            $"presenter=arm-n-dual-front;adapter={_presenter.AdapterDescription};" +
            $"geometry={geometry.OffsetX},{geometry.OffsetY},{geometry.Width}x{geometry.Height};" +
            $"geometryEpoch={geometry.Epoch};committedEpoch={Volatile.Read(ref _lastCommittedEpoch)};" +
            $"pointerAdds={Volatile.Read(ref _pointerAdds)};pointerRemoves={Volatile.Read(ref _pointerRemoves)};" +
            $"wheelSignals={Volatile.Read(ref _wheelSignals)};captionMoves={Volatile.Read(ref _captionMoves)};" +
            $"edgeResizes={Volatile.Read(ref _edgeResizes)};" +
            $"frameRequests={Volatile.Read(ref _frameRequests)};" +
            $"frameCoalesced={Volatile.Read(ref _frameRequestsCoalesced)};" +
            $"vsyncCallbacks={Volatile.Read(ref _vsyncCallbacks)};" +
            $"submitted={diagnostics.Submitted};presented={diagnostics.Presented};" +
            $"replayed={diagnostics.Replayed};failed={diagnostics.Failed};" +
            $"superseded={diagnostics.Superseded};dropped={diagnostics.Dropped}");
    }

    internal void ApplyLeftResizeSmokeStep(int step)
    {
        if (_disposed) return;
        lock (_smokeGate)
        {
            if (_disposed) return;
            var current = SnapshotGeometry();
            var cycle = Math.Abs(step % 48);
            var delta = (cycle <= 24 ? cycle : 48 - cycle) * 6;
            delta = Math.Min(delta, current.OffsetX + current.Width - MinimumWidth);
            var right = current.OffsetX + current.Width;
            var baselineLeft = Math.Max(0, right -
                Math.Clamp(
                    checked((int)Math.Round(_configuration.logicalSize.width * current.Scale)),
                    MinimumWidth,
                    _workArea.Width));
            var left = Math.Clamp(baselineLeft + delta, 0, right - MinimumWidth);
            UpdateGeometry(new(
                checked(current.Epoch + 1),
                left,
                current.OffsetY,
                right - left,
                current.Height,
                current.Scale));
        }
    }

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_shown) return;
        RequestInvalidate();
        PumpUntilFirstCommit();
        _shown = true;
        NativeMethods.ShowWindow(_hwnd, SwShow);
        NativeMethods.UpdateWindow(_hwnd);
        LifecycleChanged?.Invoke(AppLifecycleState.resumed);
        ConfigurationChanged?.Invoke(Configuration);
        RequestInvalidate();
    }

    private void PumpUntilFirstCommit()
    {
        var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(5);
        while (!_firstCommit.IsSet)
        {
            while (NativeMethods.PeekMessageW(out var message, 0, 0, 0, 1))
            {
                if (message.Message == 0x0012)
                    throw new InvalidOperationException(
                        "The window thread quit before the first exact Arm N frame committed.");
                NativeMethods.TranslateMessage(in message);
                NativeMethods.DispatchMessageW(in message);
            }
            if (DateTime.UtcNow >= deadline)
                throw new TimeoutException(
                    "The first exact Arm N frame was not committed before the raw HWND show gate.");
            Thread.Sleep(1);
        }
    }

    public void Resize(Size logicalSize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(logicalSize);
        if (!logicalSize.IsFinite || logicalSize.IsEmpty)
            throw new ArgumentOutOfRangeException(nameof(logicalSize));
        var current = SnapshotGeometry();
        var width = Math.Clamp(
            checked((int)Math.Round(logicalSize.width * current.Scale)),
            MinimumWidth,
            _workArea.Width);
        var titleBarHeight = TitleBarHeight(current.Scale);
        var height = Math.Clamp(
            checked((int)Math.Round(logicalSize.height * current.Scale)) + titleBarHeight,
            MinimumContentHeight + titleBarHeight,
            _workArea.Height);
        UpdateGeometry(current with
        {
            Epoch = checked(current.Epoch + 1),
            Width = width,
            Height = height,
            OffsetX = Math.Clamp(current.OffsetX, 0, _workArea.Width - width),
            OffsetY = Math.Clamp(current.OffsetY, 0, _workArea.Height - height),
        });
    }

    public void Close()
    {
        if (!_disposed) NativeMethods.PostMessageW(_hwnd, WmClose, 0, 0);
    }

    public void ScheduleFrame(Action<TimeSpan> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ObjectDisposedException.ThrowIf(_disposed, this);
        Interlocked.Increment(ref _frameRequests);
        lock (_stateGate)
        {
            if (_pendingFrame is not null)
            {
                Interlocked.Increment(ref _frameRequestsCoalesced);
                return;
            }
            _pendingFrame = callback;
        }
        _frameSignal.Set();
    }

    public void RequestInvalidate()
    {
        if (_disposed) return;
        lock (_stateGate) _pendingRender = _geometry;
        _renderSignal.Set();
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _ = direction;
        if (state == ViewFocusState.focused) NativeMethods.SetFocus(_hwnd);
        else NativeMethods.SetFocus(0);
    }

    public void SetClient(
        DorotiTextInputConfiguration configuration,
        DorotiTextEditingState initialState)
    {
        _textConfiguration = configuration;
        _editingState = initialState;
        NativeMethods.SetFocus(_hwnd);
        UpdateImeCaret();
    }

    public void UpdateState(DorotiTextEditingState state) => _editingState = state;

    public void SetCaretRect(Rect logicalRect)
    {
        _caretRect = logicalRect;
        UpdateImeCaret();
    }

    public void ClearClient()
    {
        _textConfiguration = null;
        _imeComposing = false;
    }

    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.FromResult(NativeMethods.GetClipboardUnicodeText(_hwnd));
    }

    public ValueTask SetClipboardTextAsync(
        string text,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        NativeMethods.SetClipboardUnicodeText(_hwnd, text);
        return ValueTask.CompletedTask;
    }

    public void SetCursor(DorotiMouseCursorKind cursor)
    {
        _clientCursor = cursor;
        if (TryGetCursorClientPoint(out var point) &&
            HitChrome(point, SnapshotGeometry()) == ChromeHit.Content)
            SetNativeCursor(cursor);
    }

    public void UpdateSemantics(SemanticsUpdate update)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _ = update;
    }

    public void ClearSemantics() { }

    internal void DispatchSemanticsAction(int nodeId, SemanticsAction action, object? arguments) =>
        _semanticsAction?.Invoke(nodeId, action, arguments);

    private void FrameLoop()
    {
        try
        {
            while (true)
            {
                _frameSignal.WaitOne();
                if (_disposed) return;
                var result = NativeMethods.DwmFlush();
                if (result < 0) Marshal.ThrowExceptionForHR(result);
                lock (_stateGate)
                {
                    if (_pendingFrame is null) continue;
                    _pendingFrameTimestamp = DorotiFrameClock.Now;
                }
                NativeMethods.PostMessageW(_hwnd, WmAppFrame, 0, 0);
            }
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"doroti.windowsappsdk vsync.fatal={exception}");
            NativeMethods.PostMessageW(_hwnd, WmClose, 0, 0);
        }
    }

    private void RenderLoop()
    {
        try
        {
            while (true)
            {
                _renderSignal.WaitOne();
                if (_disposed) return;
                GeometrySnapshot? request;
                lock (_stateGate)
                {
                    request = _pendingRender;
                    _pendingRender = null;
                }
                if (request is not { } geometry || _renderer is not { } renderer) continue;
                SkiaPaintResult paintResult = default;
                try
                {
                    var presented = _presenter.RenderAndPresent(
                        geometry.Epoch,
                        geometry.OffsetX,
                        geometry.OffsetY,
                        geometry.Width,
                        geometry.Height,
                        surface =>
                        {
                            var canvas = surface.Canvas;
                            var titleBarHeight = TitleBarHeight(geometry.Scale);
                            var contentHeight = ContentHeight(geometry);
                            canvas.Save();
                            canvas.ClipRect(
                                new SKRect(0, titleBarHeight, geometry.Width, geometry.Height),
                                SKClipOperation.Intersect,
                                false);
                            canvas.Translate(0, titleBarHeight);
                            try
                            {
                                paintResult = renderer.Paint(
                                    surface,
                                    geometry.Width,
                                    contentHeight,
                                    ToResizeEpoch(geometry));
                            }
                            finally
                            {
                                canvas.Restore();
                            }
                            if (paintResult.ShouldPresent)
                                DrawWindowChrome(canvas, geometry);
                            return paintResult.ShouldPresent;
                        });
                    if (paintResult.Completion is { } completion)
                    {
                        if (presented) renderer.CompletePaint(completion);
                        else renderer.SupersedePaint(
                            completion,
                            "a newer Arm N geometry epoch replaced the prepared front");
                    }
                }
                catch (Exception exception)
                {
                    if (paintResult.Completion is { } completion)
                        renderer.FailPaint(completion, exception.Message);
                    Console.Error.WriteLine($"doroti.windowsappsdk raster.fail={exception}");
                    NativeMethods.PostMessageW(_hwnd, WmClose, 0, 0);
                }
            }
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"doroti.windowsappsdk raster.fatal={exception}");
            NativeMethods.PostMessageW(_hwnd, WmClose, 0, 0);
        }
    }

    private nint WindowProcedureCore(uint message, nuint wParam, nint lParam)
    {
        switch (message)
        {
            case WmEraseBackground:
                return 1;
            case WmAppFrame:
                Action<TimeSpan>? callback;
                TimeSpan timestamp;
                lock (_stateGate)
                {
                    callback = _pendingFrame;
                    _pendingFrame = null;
                    timestamp = _pendingFrameTimestamp;
                }
                if (callback is not null)
                {
                    Interlocked.Increment(ref _vsyncCallbacks);
                    callback(timestamp);
                }
                return 0;
            case WmAppCommitted:
                var committedEpoch = checked((long)wParam);
                Volatile.Write(ref _lastCommittedEpoch, committedEpoch);
                var current = SnapshotGeometry();
                if (_captureKind == CaptureKind.None && current.Epoch == committedEpoch)
                    ApplyWindowRegion(current);
                return 0;
            case WmSetFocus:
                FocusData?.Invoke(new(_viewId, true, DorotiFrameClock.Now));
                return 0;
            case WmKillFocus:
                FocusData?.Invoke(new(_viewId, false, DorotiFrameClock.Now));
                return 0;
            case WmLeftButtonDown:
                return HandleButtonDown(MouseButton.Left, wParam, lParam);
            case WmRightButtonDown:
                return HandleButtonDown(MouseButton.Right, wParam, lParam);
            case WmMiddleButtonDown:
                return HandleButtonDown(MouseButton.Middle, wParam, lParam);
            case WmLeftButtonDoubleClick:
                return HandleLeftButtonDoubleClick(lParam);
            case WmMouseMove:
                return HandleMouseMove(wParam, lParam);
            case WmLeftButtonUp:
                return HandleButtonUp(MouseButton.Left, wParam, lParam);
            case WmRightButtonUp:
                return HandleButtonUp(MouseButton.Right, wParam, lParam);
            case WmMiddleButtonUp:
                return HandleButtonUp(MouseButton.Middle, wParam, lParam);
            case WmMouseWheel:
                HandleMouseWheel(wParam, lParam, horizontal: false);
                return 0;
            case WmMouseHorizontalWheel:
                HandleMouseWheel(wParam, lParam, horizontal: true);
                return 0;
            case WmMouseLeave:
                HandleMouseLeave();
                return 0;
            case WmSetCursor:
                return HandleSetCursor();
            case WmCancelMode:
                CancelCapture();
                return 0;
            case WmCaptureChanged:
                if (!_releasingCapture && lParam != _hwnd) CancelCapture();
                return 0;
            case WmKeyDown:
                RaiseKey(KeyEventType.down, wParam, lParam);
                return 0;
            case WmSysKeyDown:
                RaiseKey(KeyEventType.down, wParam, lParam);
                if (wParam == 0x73) NativeMethods.PostMessageW(_hwnd, WmClose, 0, 0);
                return 0;
            case WmKeyUp:
            case WmSysKeyUp:
                RaiseKey(KeyEventType.up, wParam, lParam);
                return 0;
            case WmChar:
                HandleCharacter(checked((char)wParam));
                return 0;
            case WmImeStartComposition:
                _imeComposing = true;
                UpdateImeCaret();
                return 0;
            case WmImeComposition:
                HandleImeComposition(lParam);
                return 0;
            case WmImeEndComposition:
                _imeComposing = false;
                return 0;
            case WmClose:
                if (!_closing)
                {
                    _closing = true;
                    CloseRequested?.Invoke();
                    NativeMethods.DestroyWindow(_hwnd);
                }
                return 0;
            case WmDestroy:
                LifecycleChanged?.Invoke(AppLifecycleState.detached);
                Closed?.Invoke();
                NativeMethods.PostQuitMessage(0);
                return 0;
            default:
                return NativeMethods.DefWindowProcW(_hwnd, message, wParam, lParam);
        }
    }

    private nint HandleButtonDown(MouseButton button, nuint wParam, nint lParam)
    {
        NativeMethods.SetFocus(_hwnd);
        var point = NativeMethods.PointFromLParam(lParam);
        var geometry = SnapshotGeometry();
        TrackMouseLeave();
        if (button == MouseButton.Left)
        {
            _resizeEdges = HitResizeEdges(point, geometry);
            if (_resizeEdges != ResizeEdges.None)
            {
                BeginNativeCapture(CaptureKind.Resize, geometry);
                return 0;
            }
            var chrome = HitChrome(point, geometry);
            if (chrome == ChromeHit.Caption)
            {
                BeginNativeCapture(CaptureKind.Move, geometry);
                return 0;
            }
            if (chrome is ChromeHit.Minimize or ChromeHit.Maximize or ChromeHit.Close)
            {
                _pressedChrome = chrome;
                SetHoverChrome(chrome);
                BeginNativeCapture(CaptureKind.CaptionButton, geometry);
                return 0;
            }
            if (chrome != ChromeHit.Content) return 0;
        }
        else if (HitChrome(point, geometry) != ChromeHit.Content)
        {
            return 0;
        }

        EnsurePointerAdded(point);
        var previousButtons = _buttons;
        _buttons = ButtonsFromWParam(wParam) | ButtonMask(button);
        RaisePointer(
            previousButtons == 0 ? PointerChange.down : PointerChange.move,
            point,
            _buttons);
        if (previousButtons == 0) NativeMethods.SetCapture(_hwnd);
        return 0;
    }

    private nint HandleMouseMove(nuint wParam, nint lParam)
    {
        var point = NativeMethods.PointFromLParam(lParam);
        TrackMouseLeave();
        if (_captureKind == CaptureKind.Resize)
        {
            NativeMethods.GetCursorPos(out var cursor);
            ResizeFromCapture(cursor.X - _captureOrigin.X, cursor.Y - _captureOrigin.Y);
            return 0;
        }
        if (_captureKind == CaptureKind.Move)
        {
            NativeMethods.GetCursorPos(out var cursor);
            MoveFromCapture(cursor.X - _captureOrigin.X, cursor.Y - _captureOrigin.Y);
            return 0;
        }
        if (_captureKind == CaptureKind.CaptionButton)
        {
            SetHoverChrome(HitChrome(point, SnapshotGeometry()));
            return 0;
        }

        var messageButtons = ButtonsFromWParam(wParam);
        if (_buttons != 0 || messageButtons != 0)
        {
            EnsurePointerAdded(point);
            _buttons = messageButtons;
            RaisePointer(PointerChange.move, point, _buttons);
            return 0;
        }

        var geometry = SnapshotGeometry();
        if (HitResizeEdges(point, geometry) != ResizeEdges.None)
        {
            SetHoverChrome(ChromeHit.None);
            RemovePointerIfIdle(point);
            return 0;
        }
        var chrome = HitChrome(point, geometry);
        SetHoverChrome(chrome);
        if (chrome != ChromeHit.Content)
        {
            RemovePointerIfIdle(point);
            return 0;
        }
        EnsurePointerAdded(point);
        RaisePointer(PointerChange.hover, point, 0);
        return 0;
    }

    private nint HandleButtonUp(MouseButton button, nuint wParam, nint lParam)
    {
        var point = NativeMethods.PointFromLParam(lParam);
        if (_captureKind != CaptureKind.None)
        {
            if (button != MouseButton.Left) return 0;
            var action = _captureKind == CaptureKind.CaptionButton &&
                         HitChrome(point, SnapshotGeometry()) == _pressedChrome
                ? _pressedChrome
                : ChromeHit.None;
            EndNativeCapture();
            _pressedChrome = ChromeHit.None;
            SetHoverChrome(HitChrome(point, SnapshotGeometry()));
            PerformChromeAction(action);
            return 0;
        }

        var previousButtons = _buttons;
        _buttons = ButtonsFromWParam(wParam) & ~ButtonMask(button);
        if (_pointerAdded)
        {
            RaisePointer(
                previousButtons != 0 && _buttons == 0 ? PointerChange.up : PointerChange.move,
                point,
                _buttons);
        }
        if (_buttons == 0 && NativeMethods.GetCapture() == _hwnd)
        {
            ReleaseMouseCapture();
            if (!IsContentPoint(point, SnapshotGeometry())) RemovePointer(point);
        }
        return 0;
    }

    private nint HandleLeftButtonDoubleClick(nint lParam)
    {
        var point = NativeMethods.PointFromLParam(lParam);
        var geometry = SnapshotGeometry();
        if (HitResizeEdges(point, geometry) == ResizeEdges.None &&
            HitChrome(point, geometry) == ChromeHit.Caption)
        {
            if (_captureKind != CaptureKind.None) EndNativeCapture();
            ToggleMaximize();
            return 0;
        }
        return HandleButtonDown(MouseButton.Left, 1, lParam);
    }

    private void HandleMouseWheel(nuint wParam, nint lParam, bool horizontal)
    {
        var point = NativeMethods.PointFromLParam(lParam);
        NativeMethods.ScreenToClient(_hwnd, ref point);
        if (!IsContentPoint(point, SnapshotGeometry())) return;
        TrackMouseLeave();
        EnsurePointerAdded(point);
        var wheelTicks = GetWheelDelta(wParam) / (double)MouseWheelDelta;
        var delta = wheelTicks * _scrollOffsetMultiplier;
        Interlocked.Increment(ref _wheelSignals);
        RaisePointer(
            _buttons == 0 ? PointerChange.hover : PointerChange.move,
            point,
            _buttons,
            horizontal ? delta : 0,
            horizontal ? 0 : -delta);
    }

    private void HandleMouseLeave()
    {
        _trackingMouseLeave = false;
        SetHoverChrome(ChromeHit.None);
        if (_buttons == 0 && _captureKind == CaptureKind.None &&
            TryGetCursorClientPoint(out var point))
            RemovePointer(point);
    }

    private nint HandleSetCursor()
    {
        if (!TryGetCursorClientPoint(out var point)) return 0;
        var geometry = SnapshotGeometry();
        var edges = _captureKind == CaptureKind.Resize
            ? _resizeEdges
            : HitResizeEdges(point, geometry);
        if (edges != ResizeEdges.None)
        {
            SetNativeCursor(CursorForEdges(edges));
            return 1;
        }
        SetNativeCursor(HitChrome(point, geometry) == ChromeHit.Content
            ? _clientCursor
            : DorotiMouseCursorKind.basic);
        return 1;
    }

    private void BeginNativeCapture(CaptureKind kind, GeometrySnapshot geometry)
    {
        if (_pointerAdded && _buttons == 0)
            RemovePointer(_hasLastPointerPoint
                ? _lastPointerPoint
                : new() { X = geometry.OffsetX, Y = geometry.OffsetY });
        _captureKind = kind;
        _captureGeometry = geometry;
        NativeMethods.GetCursorPos(out _captureOrigin);
        NativeMethods.SetWindowRgn(_hwnd, 0, true);
        NativeMethods.SetCapture(_hwnd);
    }

    private void EndNativeCapture()
    {
        _captureKind = CaptureKind.None;
        _resizeEdges = ResizeEdges.None;
        ReleaseMouseCapture();
        var current = SnapshotGeometry();
        if (Volatile.Read(ref _lastCommittedEpoch) == current.Epoch)
            ApplyWindowRegion(current);
        else
            RequestInvalidate();
    }

    private void CancelCapture()
    {
        if (_captureKind != CaptureKind.None)
        {
            _pressedChrome = ChromeHit.None;
            EndNativeCapture();
            SetHoverChrome(ChromeHit.None);
            return;
        }
        if (_pointerAdded && _buttons != 0)
            RaisePointer(PointerChange.cancel, _lastPointerPoint, 0);
        _buttons = 0;
        _pointerAdded = false;
        _hasLastPointerPoint = false;
    }

    private void ReleaseMouseCapture()
    {
        if (NativeMethods.GetCapture() != _hwnd) return;
        _releasingCapture = true;
        try { NativeMethods.ReleaseCapture(); }
        finally { _releasingCapture = false; }
    }

    private void TrackMouseLeave()
    {
        if (_trackingMouseLeave) return;
        var tracking = new NativeMethods.NativeTrackMouseEvent
        {
            Size = checked((uint)Marshal.SizeOf<NativeMethods.NativeTrackMouseEvent>()),
            Flags = TrackMouseEventLeave,
            WindowHandle = _hwnd,
        };
        _trackingMouseLeave = NativeMethods.TrackMouseEvent(ref tracking);
    }

    private void SetHoverChrome(ChromeHit value)
    {
        if (_hoverChrome == value) return;
        _hoverChrome = value;
        RequestInvalidate();
    }

    private void PerformChromeAction(ChromeHit action)
    {
        switch (action)
        {
            case ChromeHit.Minimize:
                NativeMethods.ShowWindow(_hwnd, SwMinimize);
                break;
            case ChromeHit.Maximize:
                ToggleMaximize();
                break;
            case ChromeHit.Close:
                NativeMethods.PostMessageW(_hwnd, WmClose, 0, 0);
                break;
        }
    }

    private void ToggleMaximize()
    {
        var current = SnapshotGeometry();
        GeometrySnapshot next;
        if (_isMaximized)
        {
            var restore = _restoreGeometry ?? current with
            {
                Width = Math.Min(960, _workArea.Width),
                Height = Math.Min(720, _workArea.Height),
            };
            next = restore with { Epoch = checked(current.Epoch + 1) };
            _isMaximized = false;
        }
        else
        {
            _restoreGeometry = current;
            next = current with
            {
                Epoch = checked(current.Epoch + 1),
                OffsetX = 0,
                OffsetY = 0,
                Width = _workArea.Width,
                Height = _workArea.Height,
            };
            _isMaximized = true;
        }
        UpdateGeometry(next);
    }

    private void MoveFromCapture(int deltaX, int deltaY)
    {
        if (_isMaximized) return;
        var current = SnapshotGeometry();
        var next = current with
        {
            Epoch = checked(current.Epoch + 1),
            OffsetX = Math.Clamp(_captureGeometry.OffsetX + deltaX, 0,
                _workArea.Width - _captureGeometry.Width),
            OffsetY = Math.Clamp(_captureGeometry.OffsetY + deltaY, 0,
                _workArea.Height - _captureGeometry.Height),
        };
        if (next.OffsetX == current.OffsetX && next.OffsetY == current.OffsetY) return;
        Interlocked.Increment(ref _captionMoves);
        UpdateGeometry(next);
    }

    private void ResizeFromCapture(int deltaX, int deltaY)
    {
        var left = _captureGeometry.OffsetX;
        var top = _captureGeometry.OffsetY;
        var right = left + _captureGeometry.Width;
        var bottom = top + _captureGeometry.Height;
        if ((_resizeEdges & ResizeEdges.Left) != 0)
            left = Math.Clamp(left + deltaX, 0, right - MinimumWidth);
        if ((_resizeEdges & ResizeEdges.Right) != 0)
            right = Math.Clamp(right + deltaX, left + MinimumWidth, _workArea.Width);
        if ((_resizeEdges & ResizeEdges.Top) != 0)
            top = Math.Clamp(top + deltaY, 0,
                bottom - MinimumContentHeight - TitleBarHeight(_captureGeometry.Scale));
        if ((_resizeEdges & ResizeEdges.Bottom) != 0)
            bottom = Math.Clamp(bottom + deltaY,
                top + MinimumContentHeight + TitleBarHeight(_captureGeometry.Scale),
                _workArea.Height);
        var current = SnapshotGeometry();
        var next = new GeometrySnapshot(
            checked(current.Epoch + 1),
            left,
            top,
            right - left,
            bottom - top,
            current.Scale);
        if (next.OffsetX == current.OffsetX && next.OffsetY == current.OffsetY &&
            next.Width == current.Width && next.Height == current.Height) return;
        _isMaximized = false;
        Interlocked.Increment(ref _edgeResizes);
        UpdateGeometry(next);
    }

    private void UpdateGeometry(GeometrySnapshot next)
    {
        lock (_stateGate)
        {
            _geometry = next;
            _pendingRender = next;
        }
        _presenter.StageGeometry(
            next.Epoch, next.OffsetX, next.OffsetY, next.Width, next.Height);
        MetricsChanged?.Invoke(ToMetrics(next));
        _renderSignal.Set();
    }

    private void HandlePresenterCommitted(long epoch, int x, int y, int width, int height)
    {
        _ = (x, y, width, height);
        _firstCommit.Set();
        NativeMethods.PostMessageW(_hwnd, WmAppCommitted, checked((nuint)epoch), 0);
    }

    private void RaisePointer(
        PointerChange change,
        NativeMethods.NativePoint point,
        long buttons,
        double scrollDeltaX = 0,
        double scrollDeltaY = 0)
    {
        var geometry = SnapshotGeometry();
        var x = point.X - geometry.OffsetX;
        var y = point.Y - geometry.OffsetY - TitleBarHeight(geometry.Scale);
        var isSignal = scrollDeltaX != 0 || scrollDeltaY != 0;
        var resetDelta = isSignal ||
            change is PointerChange.add or PointerChange.remove or PointerChange.cancel;
        var deltaX = resetDelta || !_hasLastPointerPoint
            ? 0
            : point.X - _lastPointerPoint.X;
        var deltaY = resetDelta || !_hasLastPointerPoint
            ? 0
            : point.Y - _lastPointerPoint.Y;
        var sequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = DorotiFrameClock.Now;
        PointerData?.Invoke(new([
            new(
                _viewId,
                timestamp,
                change,
                PointerDeviceKind.mouse,
                1,
                x,
                y,
                deltaX,
                deltaY,
                buttons,
                scrollDeltaX: scrollDeltaX,
                scrollDeltaY: scrollDeltaY,
                signalKind: isSignal ? PointerSignalKind.scroll : PointerSignalKind.none,
                pointerIdentifier: change is PointerChange.down or PointerChange.move or
                    PointerChange.up or PointerChange.cancel ? 1UL : 0UL)
        ]));
        InputReceived?.Invoke(sequence, timestamp);
        _lastPointerPoint = point;
        _hasLastPointerPoint = change is not (PointerChange.remove or PointerChange.cancel);
    }

    private void EnsurePointerAdded(NativeMethods.NativePoint point)
    {
        if (_pointerAdded) return;
        _pointerAdded = true;
        Interlocked.Increment(ref _pointerAdds);
        RaisePointer(PointerChange.add, point, 0);
    }

    private void RemovePointerIfIdle(NativeMethods.NativePoint point)
    {
        if (_buttons == 0) RemovePointer(point);
    }

    private void RemovePointer(NativeMethods.NativePoint point)
    {
        if (!_pointerAdded) return;
        RaisePointer(PointerChange.remove, point, 0);
        _pointerAdded = false;
        _hasLastPointerPoint = false;
        Interlocked.Increment(ref _pointerRemoves);
    }

    private static int TitleBarHeight(double scale) =>
        Math.Max(TitleBarLogicalHeight, checked((int)Math.Round(TitleBarLogicalHeight * scale)));

    private static int CaptionButtonWidth(double scale) =>
        Math.Max(CaptionButtonLogicalWidth,
            checked((int)Math.Round(CaptionButtonLogicalWidth * scale)));

    private static int ResizeBorderWidth(double scale) =>
        Math.Max(ResizeBorderLogical,
            checked((int)Math.Round(ResizeBorderLogical * scale)));

    private static int ContentHeight(GeometrySnapshot geometry) =>
        Math.Max(1, geometry.Height - TitleBarHeight(geometry.Scale));

    private static ChromeHit HitChrome(
        NativeMethods.NativePoint point,
        GeometrySnapshot geometry)
    {
        var x = point.X - geometry.OffsetX;
        var y = point.Y - geometry.OffsetY;
        if (x < 0 || y < 0 || x >= geometry.Width || y >= geometry.Height)
            return ChromeHit.None;
        if (y >= TitleBarHeight(geometry.Scale)) return ChromeHit.Content;
        var buttonWidth = CaptionButtonWidth(geometry.Scale);
        var buttonStart = geometry.Width - buttonWidth * 3;
        if (x < buttonStart) return ChromeHit.Caption;
        if (x < buttonStart + buttonWidth) return ChromeHit.Minimize;
        if (x < buttonStart + buttonWidth * 2) return ChromeHit.Maximize;
        return ChromeHit.Close;
    }

    private static bool IsContentPoint(
        NativeMethods.NativePoint point,
        GeometrySnapshot geometry) =>
        HitResizeEdges(point, geometry) == ResizeEdges.None &&
        HitChrome(point, geometry) == ChromeHit.Content;

    private bool TryGetCursorClientPoint(out NativeMethods.NativePoint point)
    {
        if (!NativeMethods.GetCursorPos(out point)) return false;
        return NativeMethods.ScreenToClient(_hwnd, ref point);
    }

    private static long ButtonMask(MouseButton button) => button switch
    {
        MouseButton.Left => 1,
        MouseButton.Right => 2,
        MouseButton.Middle => 4,
        _ => 0,
    };

    private static long ButtonsFromWParam(nuint wParam)
    {
        var keys = unchecked((ushort)wParam);
        return ((keys & 0x0001) != 0 ? 1 : 0) |
               ((keys & 0x0002) != 0 ? 2 : 0) |
               ((keys & 0x0010) != 0 ? 4 : 0);
    }

    private static int GetWheelDelta(nuint wParam) =>
        unchecked((short)((ulong)wParam >> 16));

    private static DorotiMouseCursorKind CursorForEdges(ResizeEdges edges) => edges switch
    {
        ResizeEdges.Left or ResizeEdges.Right => DorotiMouseCursorKind.resizeLeftRight,
        ResizeEdges.Top or ResizeEdges.Bottom => DorotiMouseCursorKind.resizeUpDown,
        ResizeEdges.Left | ResizeEdges.Top or
        ResizeEdges.Right | ResizeEdges.Bottom => DorotiMouseCursorKind.resizeUpLeftDownRight,
        _ => DorotiMouseCursorKind.resizeUpRightDownLeft,
    };

    private static void SetNativeCursor(DorotiMouseCursorKind cursor) =>
        NativeMethods.SetCursor(NativeMethods.LoadCursorW(0, NativeMethods.CursorId(cursor)));

    private void DrawWindowChrome(SKCanvas canvas, GeometrySnapshot geometry)
    {
        var height = TitleBarHeight(geometry.Scale);
        var buttonWidth = CaptionButtonWidth(geometry.Scale);
        var hover = _hoverChrome;
        var pressed = _pressedChrome;
        var scale = (float)geometry.Scale;
        using var background = new SKPaint
        {
            Color = new SKColor(0xF3, 0xF3, 0xF3),
            Style = SKPaintStyle.Fill,
        };
        using var border = new SKPaint
        {
            Color = new SKColor(0xB8, 0xB8, 0xB8),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = Math.Max(1, scale),
            IsAntialias = false,
        };
        using var glyph = new SKPaint
        {
            Color = new SKColor(0x20, 0x20, 0x20),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = Math.Max(1, scale),
            IsAntialias = true,
        };
        canvas.DrawRect(0, 0, geometry.Width, height, background);

        DrawCaptionButton(ChromeHit.Minimize, geometry.Width - buttonWidth * 3);
        DrawCaptionButton(ChromeHit.Maximize, geometry.Width - buttonWidth * 2);
        DrawCaptionButton(ChromeHit.Close, geometry.Width - buttonWidth);

        using var typeface = SKTypeface.FromFamilyName("Segoe UI");
        using var font = new SKFont(typeface, 12 * scale);
        using var text = new SKPaint
        {
            Color = new SKColor(0x20, 0x20, 0x20),
            IsAntialias = true,
        };
        var title = string.IsNullOrWhiteSpace(_configuration.title)
            ? "Doroti"
            : _configuration.title;
        var baseline = height / 2f + font.Size * 0.36f;
        canvas.DrawText(title, 12 * scale, baseline, SKTextAlign.Left, font, text);
        canvas.DrawLine(0, height - border.StrokeWidth / 2,
            geometry.Width, height - border.StrokeWidth / 2, border);
        canvas.DrawRect(border.StrokeWidth / 2, border.StrokeWidth / 2,
            geometry.Width - border.StrokeWidth, geometry.Height - border.StrokeWidth, border);

        void DrawCaptionButton(ChromeHit hit, int left)
        {
            if (hover == hit || pressed == hit)
            {
                using var fill = new SKPaint
                {
                    Color = hit == ChromeHit.Close
                        ? new SKColor(0xC4, 0x2B, 0x1C)
                        : pressed == hit
                            ? new SKColor(0xD6, 0xD6, 0xD6)
                            : new SKColor(0xE5, 0xE5, 0xE5),
                    Style = SKPaintStyle.Fill,
                };
                canvas.DrawRect(left, 0, buttonWidth, height, fill);
            }
            var color = hit == ChromeHit.Close && (hover == hit || pressed == hit)
                ? SKColors.White
                : new SKColor(0x20, 0x20, 0x20);
            glyph.Color = color;
            var centerX = left + buttonWidth / 2f;
            var centerY = height / 2f;
            var half = 5 * scale;
            switch (hit)
            {
                case ChromeHit.Minimize:
                    canvas.DrawLine(centerX - half, centerY + 3 * scale,
                        centerX + half, centerY + 3 * scale, glyph);
                    break;
                case ChromeHit.Maximize when _isMaximized:
                    canvas.DrawRect(centerX - half + 2 * scale, centerY - half,
                        half * 2, half * 2, glyph);
                    canvas.DrawRect(centerX - half, centerY - half + 2 * scale,
                        half * 2, half * 2, glyph);
                    break;
                case ChromeHit.Maximize:
                    canvas.DrawRect(centerX - half, centerY - half,
                        half * 2, half * 2, glyph);
                    break;
                case ChromeHit.Close:
                    canvas.DrawLine(centerX - half, centerY - half,
                        centerX + half, centerY + half, glyph);
                    canvas.DrawLine(centerX + half, centerY - half,
                        centerX - half, centerY + half, glyph);
                    break;
            }
        }
    }

    private void RaiseKey(KeyEventType type, nuint wParam, nint lParam)
    {
        var sequence = Interlocked.Increment(ref _inputSequence);
        var timestamp = DorotiFrameClock.Now;
        var scanCode = (long)((lParam.ToInt64() >> 16) & 0x1ff);
        KeyData?.Invoke(new(
            _viewId,
            timestamp,
            type,
            scanCode,
            checked((long)wParam),
            false));
        InputReceived?.Invoke(sequence, timestamp);
    }

    private void HandleCharacter(char character)
    {
        if (_textConfiguration is not { } configuration || _imeComposing) return;
        if (character is '\r' or '\n')
        {
            if (configuration.inputType == DorotiTextInputType.multiline)
                PublishText(_editingState.text + "\n", composing: false);
            else
                ActionPerformed?.Invoke(configuration.inputAction);
            return;
        }
        if (character == '\b')
        {
            var text = _editingState.text;
            if (text.Length > 0) PublishText(text[..^1], composing: false);
            return;
        }
        if (!char.IsControl(character))
            PublishText(_editingState.text + character, composing: false);
    }

    private void HandleImeComposition(nint lParam)
    {
        var flags = checked((int)lParam);
        if ((flags & GcsResultStr) != 0)
        {
            var result = NativeMethods.GetImmCompositionString(_hwnd, GcsResultStr);
            if (result is not null) PublishText(result, composing: false);
            _imeComposing = false;
        }
        else if ((flags & GcsCompStr) != 0)
        {
            var composition = NativeMethods.GetImmCompositionString(_hwnd, GcsCompStr);
            if (composition is not null) PublishText(composition, composing: true);
        }
    }

    private void PublishText(string text, bool composing)
    {
        var selection = new DorotiTextSelection(text.Length, text.Length);
        _editingState = new(
            text,
            selection,
            composing ? new DorotiTextSelection(0, text.Length) : null);
        EditingStateChanged?.Invoke(_editingState);
    }

    private void UpdateImeCaret()
    {
        if (_hwnd == 0 || _textConfiguration is null) return;
        var geometry = SnapshotGeometry();
        var x = geometry.OffsetX + checked((int)Math.Round(_caretRect.left * geometry.Scale));
        var y = geometry.OffsetY + TitleBarHeight(geometry.Scale) +
                checked((int)Math.Round(_caretRect.bottom * geometry.Scale));
        NativeMethods.SetImePosition(_hwnd, x, y);
    }

    private void ApplyWindowRegion(GeometrySnapshot geometry)
    {
        var region = NativeMethods.CreateRectRgn(
            geometry.OffsetX,
            geometry.OffsetY,
            geometry.OffsetX + geometry.Width,
            geometry.OffsetY + geometry.Height);
        if (region == 0) throw new InvalidOperationException("CreateRectRgn failed.");
        if (NativeMethods.SetWindowRgn(_hwnd, region, true) == 0)
            NativeMethods.DeleteObject(region);
    }

    private GeometrySnapshot SnapshotGeometry()
    {
        lock (_stateGate) return _geometry;
    }

    private static ViewMetrics ToMetrics(GeometrySnapshot value) => new(
        new Size(value.Width, ContentHeight(value)),
        value.Scale,
        ViewPadding.zero,
        ViewPadding.zero,
        ViewPadding.zero,
        AppLifecycleState.resumed,
        value.Epoch,
        1);

    private static DorotiResizeEpoch ToResizeEpoch(GeometrySnapshot value) => new(
        value.Epoch,
        value.Width / value.Scale,
        ContentHeight(value) / value.Scale,
        value.Width,
        ContentHeight(value),
        value.Scale,
        DorotiFrameClock.Now.Ticks / 10);

    private static Locale ToLocale(CultureInfo culture)
    {
        try
        {
            var parts = culture.Name.Split('-', StringSplitOptions.RemoveEmptyEntries);
            return new(parts.ElementAtOrDefault(0) ?? "en", parts.ElementAtOrDefault(1));
        }
        catch (CultureNotFoundException)
        {
            return new("en", "US");
        }
    }

    private static ResizeEdges HitResizeEdges(
        NativeMethods.NativePoint point,
        GeometrySnapshot geometry)
    {
        if (point.X < geometry.OffsetX ||
            point.X >= geometry.OffsetX + geometry.Width ||
            point.Y < geometry.OffsetY ||
            point.Y >= geometry.OffsetY + geometry.Height)
            return ResizeEdges.None;
        var resizeBorder = ResizeBorderWidth(geometry.Scale);
        var result = ResizeEdges.None;
        if (Math.Abs(point.X - geometry.OffsetX) <= resizeBorder) result |= ResizeEdges.Left;
        if (Math.Abs(point.X - (geometry.OffsetX + geometry.Width - 1)) <= resizeBorder)
            result |= ResizeEdges.Right;
        if (Math.Abs(point.Y - geometry.OffsetY) <= resizeBorder) result |= ResizeEdges.Top;
        if (Math.Abs(point.Y - (geometry.OffsetY + geometry.Height - 1)) <= resizeBorder)
            result |= ResizeEdges.Bottom;
        return result;
    }

    private static void EnsureWindowClass()
    {
        if (Volatile.Read(ref _windowClass) != 0) return;
        lock (ClassGate)
        {
            if (_windowClass != 0) return;
            var windowClass = new NativeMethods.WindowClassEx
            {
                Size = checked((uint)Marshal.SizeOf<NativeMethods.WindowClassEx>()),
                Style = ClassDoubleClicks,
                WindowProcedure = Marshal.GetFunctionPointerForDelegate(WindowProcedure),
                Instance = NativeMethods.GetModuleHandleW(null),
                Cursor = NativeMethods.LoadCursorW(0, 32512),
                ClassName = WindowClassName,
            };
            _windowClass = NativeMethods.RegisterClassExW(in windowClass);
            if (_windowClass == 0)
                throw new InvalidOperationException(
                    $"RegisterClassExW failed: {Marshal.GetLastWin32Error()}");
        }
    }

    private static nint StaticWindowProcedure(nint hwnd, uint message, nuint wParam, nint lParam) =>
        Instances.TryGetValue(hwnd, out var instance)
            ? instance.WindowProcedureCore(message, wParam, lParam)
            : NativeMethods.DefWindowProcW(hwnd, message, wParam, lParam);

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _renderSignal.Set();
        _frameSignal.Set();
        if (Thread.CurrentThread != _renderThread && !_renderThread.Join(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("Windows App SDK raster thread did not stop.");
        if (Thread.CurrentThread != _frameThread && !_frameThread.Join(TimeSpan.FromSeconds(5)))
            throw new TimeoutException("Windows App SDK frame thread did not stop.");
        _presenter.Committed -= HandlePresenterCommitted;
        _presenter.Dispose();
        _islandBridge.Dispose();
        Instances.TryRemove(_hwnd, out _);
        if (NativeMethods.IsWindow(_hwnd)) NativeMethods.DestroyWindow(_hwnd);
        _firstCommit.Dispose();
        _renderSignal.Dispose();
        _frameSignal.Dispose();
    }

    [Flags]
    private enum ResizeEdges
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8,
    }

    private enum CaptureKind
    {
        None,
        Move,
        Resize,
        CaptionButton,
    }

    private enum ChromeHit
    {
        None,
        Content,
        Caption,
        Minimize,
        Maximize,
        Close,
    }

    private enum MouseButton
    {
        Left,
        Right,
        Middle,
    }

    private readonly record struct GeometrySnapshot(
        long Epoch,
        int OffsetX,
        int OffsetY,
        int Width,
        int Height,
        double Scale);

    private static partial class NativeMethods
    {
        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        internal delegate nint WindowProcedure(nint hwnd, uint message, nuint wParam, nint lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WindowClassEx
        {
            internal uint Size;
            internal uint Style;
            internal nint WindowProcedure;
            internal int ClassExtra;
            internal int WindowExtra;
            internal nint Instance;
            internal nint Icon;
            internal nint Cursor;
            internal nint Background;
            internal string? MenuName;
            internal string ClassName;
            internal nint SmallIcon;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativePoint
        {
            internal int X;
            internal int Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeRect
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
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

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeMessage
        {
            internal nint Hwnd;
            internal uint Message;
            internal nuint WParam;
            internal nint LParam;
            internal uint Time;
            internal NativePoint Point;
            internal uint Private;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct NativeTrackMouseEvent
        {
            internal uint Size;
            internal uint Flags;
            internal nint WindowHandle;
            internal uint HoverTime;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CompositionForm
        {
            internal uint Style;
            internal NativePoint CurrentPosition;
            internal NativeRect Area;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CandidateForm
        {
            internal uint Index;
            internal uint Style;
            internal NativePoint CurrentPosition;
            internal NativeRect Area;
        }

        internal static NativePoint PointFromLParam(nint value) => new()
        {
            X = unchecked((short)(value.ToInt64() & 0xffff)),
            Y = unchecked((short)((value.ToInt64() >> 16) & 0xffff)),
        };

        internal static NativeRect GetPrimaryWorkArea()
        {
            var monitor = MonitorFromPoint(default, 1);
            var info = new MonitorInfo { Size = checked((uint)Marshal.SizeOf<MonitorInfo>()) };
            if (monitor == 0 || !GetMonitorInfoW(monitor, ref info))
                throw new InvalidOperationException("The primary monitor work area is unavailable.");
            return info.Work;
        }

        internal static double GetScrollOffsetMultiplier()
        {
            const uint defaultLines = 3;
            const uint pageScroll = uint.MaxValue;
            var lines = defaultLines;
            if (!SystemParametersInfoW(0x0068, 0, ref lines, 0) ||
                lines == 0 || lines == pageScroll)
                lines = defaultLines;
            return lines * 100.0 / 3.0;
        }

        internal static int CursorId(DorotiMouseCursorKind cursor) => cursor switch
        {
            DorotiMouseCursorKind.text or DorotiMouseCursorKind.verticalText => 32513,
            DorotiMouseCursorKind.wait => 32514,
            DorotiMouseCursorKind.precise => 32515,
            DorotiMouseCursorKind.resizeUpLeftDownRight => 32642,
            DorotiMouseCursorKind.resizeUpRightDownLeft => 32643,
            DorotiMouseCursorKind.resizeLeftRight => 32644,
            DorotiMouseCursorKind.resizeUpDown => 32645,
            DorotiMouseCursorKind.move or DorotiMouseCursorKind.allScroll => 32646,
            DorotiMouseCursorKind.forbidden or DorotiMouseCursorKind.noDrop => 32648,
            DorotiMouseCursorKind.click => 32649,
            DorotiMouseCursorKind.help => 32651,
            _ => 32512,
        };

        internal static string? GetImmCompositionString(nint hwnd, int index)
        {
            var context = ImmGetContext(hwnd);
            if (context == 0) return null;
            try
            {
                var byteCount = ImmGetCompositionStringW(context, index, 0, 0);
                if (byteCount < 0) return null;
                if (byteCount == 0) return string.Empty;
                var buffer = Marshal.AllocHGlobal(byteCount);
                try
                {
                    if (ImmGetCompositionStringW(
                            context, index, buffer, checked((uint)byteCount)) < 0)
                        return null;
                    return Marshal.PtrToStringUni(buffer, byteCount / 2);
                }
                finally
                {
                    Marshal.FreeHGlobal(buffer);
                }
            }
            finally
            {
                ImmReleaseContext(hwnd, context);
            }
        }

        internal static void SetImePosition(nint hwnd, int x, int y)
        {
            var context = ImmGetContext(hwnd);
            if (context == 0) return;
            try
            {
                var composition = new CompositionForm
                {
                    Style = 0x0002,
                    CurrentPosition = new() { X = x, Y = y },
                };
                ImmSetCompositionWindow(context, in composition);
                var candidate = new CandidateForm
                {
                    Style = 0x0040,
                    CurrentPosition = new() { X = x, Y = y },
                };
                ImmSetCandidateWindow(context, in candidate);
            }
            finally
            {
                ImmReleaseContext(hwnd, context);
            }
        }

        internal static string? GetClipboardUnicodeText(nint hwnd)
        {
            if (!OpenClipboard(hwnd)) return null;
            try
            {
                var handle = GetClipboardData(CfUnicodeText);
                if (handle == 0) return null;
                var pointer = GlobalLock(handle);
                if (pointer == 0) return null;
                try { return Marshal.PtrToStringUni(pointer); }
                finally { GlobalUnlock(handle); }
            }
            finally
            {
                CloseClipboard();
            }
        }

        internal static void SetClipboardUnicodeText(nint hwnd, string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            if (!OpenClipboard(hwnd))
                throw new InvalidOperationException("OpenClipboard failed.");
            nint memory = 0;
            try
            {
                if (!EmptyClipboard()) throw new InvalidOperationException("EmptyClipboard failed.");
                var bytes = Encoding.Unicode.GetBytes(text + '\0');
                memory = GlobalAlloc(GmemMoveable, checked((nuint)bytes.Length));
                if (memory == 0) throw new OutOfMemoryException();
                var pointer = GlobalLock(memory);
                if (pointer == 0) throw new InvalidOperationException("GlobalLock failed.");
                try { Marshal.Copy(bytes, 0, pointer, bytes.Length); }
                finally { GlobalUnlock(memory); }
                if (SetClipboardData(CfUnicodeText, memory) == 0)
                    throw new InvalidOperationException("SetClipboardData failed.");
                memory = 0;
            }
            finally
            {
                if (memory != 0) GlobalFree(memory);
                CloseClipboard();
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern ushort RegisterClassExW(in WindowClassEx windowClass);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern nint CreateWindowExW(
            uint extendedStyle, string className, string windowName, uint style,
            int x, int y, int width, int height, nint parent, nint menu,
            nint instance, nint parameter);

        [DllImport("user32.dll")]
        internal static extern nint DefWindowProcW(nint hwnd, uint message, nuint wParam, nint lParam);

        [DllImport("user32.dll")]
        internal static extern int GetMessageW(out NativeMessage message, nint hwnd, uint minimum, uint maximum);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TranslateMessage(in NativeMessage message);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekMessageW(
            out NativeMessage message,
            nint window,
            uint minimum,
            uint maximum,
            uint removeMessage);

        [DllImport("user32.dll")]
        internal static extern nint DispatchMessageW(in NativeMessage message);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PostMessageW(nint hwnd, uint message, nuint wParam, nint lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(nint hwnd, int command);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateWindow(nint hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyWindow(nint hwnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindow(nint hwnd);

        [DllImport("user32.dll")]
        internal static extern void PostQuitMessage(int exitCode);

        [DllImport("user32.dll")]
        internal static extern nint SetFocus(nint hwnd);

        [DllImport("user32.dll")]
        internal static extern nint SetCapture(nint hwnd);

        [DllImport("user32.dll")]
        internal static extern nint GetCapture();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(out NativePoint point);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ScreenToClient(nint hwnd, ref NativePoint point);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TrackMouseEvent(ref NativeTrackMouseEvent tracking);

        [DllImport("user32.dll")]
        internal static extern nint LoadCursorW(nint instance, int cursorName);

        [DllImport("user32.dll")]
        internal static extern nint SetCursor(nint cursor);

        [DllImport("user32.dll")]
        internal static extern uint GetDpiForSystem();

        [DllImport("dwmapi.dll")]
        internal static extern int DwmFlush();

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfoW(
            uint action, uint parameter, ref uint value, uint flags);

        [DllImport("user32.dll")]
        internal static extern int SetWindowRgn(nint hwnd, nint region, [MarshalAs(UnmanagedType.Bool)] bool redraw);

        [DllImport("gdi32.dll")]
        internal static extern nint CreateRectRgn(int left, int top, int right, int bottom);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(nint value);

        [DllImport("user32.dll")]
        private static extern nint MonitorFromPoint(NativePoint point, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfoW(nint monitor, ref MonitorInfo info);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern nint GetModuleHandleW(string? moduleName);

        [DllImport("imm32.dll")]
        private static extern nint ImmGetContext(nint hwnd);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ImmReleaseContext(nint hwnd, nint context);

        [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
        private static extern int ImmGetCompositionStringW(
            nint context, int index, nint buffer, uint bufferLength);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ImmSetCompositionWindow(nint context, in CompositionForm form);

        [DllImport("imm32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ImmSetCandidateWindow(nint context, in CandidateForm form);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenClipboard(nint owner);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EmptyClipboard();

        [DllImport("user32.dll")]
        private static extern nint GetClipboardData(uint format);

        [DllImport("user32.dll")]
        private static extern nint SetClipboardData(uint format, nint memory);

        [DllImport("kernel32.dll")]
        private static extern nint GlobalAlloc(uint flags, nuint bytes);

        [DllImport("kernel32.dll")]
        private static extern nint GlobalLock(nint memory);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalUnlock(nint memory);

        [DllImport("kernel32.dll")]
        private static extern nint GlobalFree(nint memory);
    }
}
