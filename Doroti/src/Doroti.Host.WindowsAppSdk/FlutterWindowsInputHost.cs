using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// Child-HWND input capability owner for one Flutter-style view.  All content
/// pointer, keyboard, text, clipboard, focus, and client cursor work is routed
/// from <see cref="FlutterWindowsChildMessageRouter"/>; the standard top-level
/// WndProc is intentionally not a participant.
/// </summary>
internal sealed class FlutterWindowsInputHost :
    IInputHostCapability,
    IViewFocusRequestCapability,
    ITextInputHostCapability,
    IPlatformServicesHostCapability,
    IDisposable
{
    private const uint WmSetFocus = 0x0007;
    private const uint WmKillFocus = 0x0008;
    private const uint WmCancelMode = 0x001f;
    // Native spelling is retained because this child-only handler intentionally
    // owns WM_SETCURSOR only for HTCLIENT.
    private const uint WM_SETCURSOR = 0x0020;
    private const uint WmCaptureChanged = 0x0215;
    private const uint WmMouseMove = 0x0200;
    private const uint WmLeftButtonDown = 0x0201;
    private const uint WmLeftButtonUp = 0x0202;
    private const uint WmLeftButtonDoubleClick = 0x0203;
    private const uint WmRightButtonDown = 0x0204;
    private const uint WmRightButtonUp = 0x0205;
    private const uint WmRightButtonDoubleClick = 0x0206;
    private const uint WmMiddleButtonDown = 0x0207;
    private const uint WmMiddleButtonUp = 0x0208;
    private const uint WmMiddleButtonDoubleClick = 0x0209;
    private const uint WmMouseWheel = 0x020a;
    private const uint WmXButtonDown = 0x020b;
    private const uint WmXButtonUp = 0x020c;
    private const uint WmXButtonDoubleClick = 0x020d;
    private const uint WmMouseHorizontalWheel = 0x020e;
    private const uint WmMouseLeave = 0x02a3;
    private const uint WmTouch = 0x0240;
    private const uint WmPointerUpdate = 0x0245;
    private const uint WmPointerDown = 0x0246;
    private const uint WmPointerUp = 0x0247;
    private const uint WmPointerEnter = 0x0249;
    private const uint WmPointerLeave = 0x024a;
    private const uint WmPointerCaptureChanged = 0x024c;
    private const uint HtClient = 1;
    private const uint TrackMouseEventLeave = 0x00000002;
    private const int MouseWheelDelta = 120;
    private const uint MkLeftButton = 0x0001;
    private const uint MkRightButton = 0x0002;
    private const uint MkMiddleButton = 0x0010;
    private const uint MkXButton1 = 0x0020;
    private const uint MkXButton2 = 0x0040;
    private const uint XButton1 = 0x0001;
    private const uint PtPointer = 0x00000001;
    private const uint PtTouch = 0x00000002;
    private const uint PtPen = 0x00000003;
    private const uint TouchEventMove = 0x0001;
    private const uint TouchEventDown = 0x0002;
    private const uint TouchEventUp = 0x0004;

    private readonly FlutterWindowsHostWindow _host;
    private readonly nint _childHwnd;
    private readonly ulong _viewId;
    private readonly object _gate = new();
    private readonly Dictionary<ulong, PointerState> _pointerStates = [];
    private readonly FlutterWindowsImm32TextInputManager _textInput;
    private readonly FlutterWindowsKeyboardManager _keyboard;
    private readonly FlutterWindowsChildMessageRouter _router;
    private long _mouseButtons;
    private bool _mouseAdded;
    private bool _mouseCapture;
    private bool _releasingMouseCapture;
    private bool _trackingMouseLeave;
    private bool _touchRegistered;
    private bool _focused;
    private bool _hasLastMousePoint;
    private FlutterWindowsInputNative.NativePoint _lastMousePoint;
    private int _clientCursor = (int)DorotiMouseCursorKind.basic;
    private bool _disposed;
    private long _pointerAddCount;
    private long _pointerHoverCount;
    private long _pointerDownCount;
    private long _pointerMoveCount;
    private long _pointerUpCount;
    private long _pointerRemoveCount;
    private long _pointerCancelCount;
    private long _wheelCount;
    private long _mouseCaptureAcquireCount;
    private long _mouseCaptureReleaseCount;
    private long _pointerCaptureAcquireCount;
    private long _pointerCaptureReleaseCount;
    private long _focusGainedCount;
    private long _focusLostCount;
    private long _clientCursorHandledCount;
    private long _outsideBoundsUpCount;
    private long _clipboardReadCount;
    private long _clipboardWriteCount;
    private long _touchRegistrationCount;
    private long _touchUnregistrationCount;

    internal FlutterWindowsInputHost(
        FlutterWindowsHostWindow host,
        ulong viewId,
        Func<WindowsViewMetrics> metricsProvider)
    {
        _host = host ?? throw new ArgumentNullException(nameof(host));
        if (viewId == 0) throw new ArgumentOutOfRangeException(nameof(viewId));
        _childHwnd = host.ViewHwnd;
        if (_childHwnd == 0)
            throw new InvalidOperationException("The Flutter input host requires the live child HWND.");
        _viewId = viewId;
        ArgumentNullException.ThrowIfNull(metricsProvider);
        _textInput = new(
            _childHwnd,
            metricsProvider,
            state => EditingStateChanged?.Invoke(state),
            action => ActionPerformed?.Invoke(action));
        _keyboard = new(
            _viewId,
            key => KeyData?.Invoke(key),
            _textInput.TryCommitKeyboardText,
            _textInput.TryPerformTextAction);
        _touchRegistered = FlutterWindowsInputNative.RegisterTouchWindow(_childHwnd, 0);
        if (_touchRegistered) Interlocked.Increment(ref _touchRegistrationCount);
        _router = new FlutterWindowsChildMessageRouter(_host, HandleChildMessage);
    }

    public event Action<PointerDataPacket>? PointerData;

    public event Action<KeyData>? KeyData;

    public event Action<RawFocusData>? FocusData;

    public event Action<DorotiTextEditingState>? EditingStateChanged;

    public event Action<DorotiTextInputAction>? ActionPerformed;

    /// <summary>Actual host-attached route, also usable by the isolated fixture.</summary>
    internal FlutterWindowsChildMessageRouter Router => _router;

    internal FlutterWindowsKeyboardManagerSnapshot KeyboardSnapshot => _keyboard.Snapshot;

    internal FlutterWindowsImm32TextInputManagerSnapshot TextInputSnapshot => _textInput.Snapshot;

    internal FlutterWindowsInputHostSnapshot Snapshot
    {
        get
        {
            lock (_gate)
            {
                return new(
                    _viewId,
                    _childHwnd,
                    _focused,
                    _mouseCapture,
                    _pointerStates.Values.Any(static state => state.HasCapture),
                    (DorotiMouseCursorKind)Volatile.Read(ref _clientCursor),
                    Interlocked.Read(ref _pointerAddCount),
                    Interlocked.Read(ref _pointerHoverCount),
                    Interlocked.Read(ref _pointerDownCount),
                    Interlocked.Read(ref _pointerMoveCount),
                    Interlocked.Read(ref _pointerUpCount),
                    Interlocked.Read(ref _pointerRemoveCount),
                    Interlocked.Read(ref _pointerCancelCount),
                    Interlocked.Read(ref _wheelCount),
                    Interlocked.Read(ref _mouseCaptureAcquireCount),
                    Interlocked.Read(ref _mouseCaptureReleaseCount),
                    Interlocked.Read(ref _pointerCaptureAcquireCount),
                    Interlocked.Read(ref _pointerCaptureReleaseCount),
                    Interlocked.Read(ref _focusGainedCount),
                    Interlocked.Read(ref _focusLostCount),
                    Interlocked.Read(ref _clientCursorHandledCount),
                    TopLevelNonClientCursorHandledCount: 0,
                    Interlocked.Read(ref _outsideBoundsUpCount),
                    Interlocked.Read(ref _clipboardReadCount),
                    Interlocked.Read(ref _clipboardWriteCount),
                    _touchRegistered,
                    Interlocked.Read(ref _touchRegistrationCount),
                    Interlocked.Read(ref _touchUnregistrationCount),
                    _disposed);
            }
        }
    }

    public void RequestFocus(ViewFocusState state, ViewFocusDirection direction)
    {
        _ = direction;
        ThrowIfDisposed();
        if (state == ViewFocusState.focused)
        {
            _ = FlutterWindowsInputNative.SetFocus(_childHwnd);
            return;
        }
        if (FlutterWindowsInputNative.GetFocus() == _childHwnd)
            _ = FlutterWindowsInputNative.SetFocus(0);
    }

    public void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState)
    {
        ThrowIfDisposed();
        _textInput.SetClient(configuration, initialState);
        RequestFocus(ViewFocusState.focused, ViewFocusDirection.undefined);
    }

    public void UpdateState(DorotiTextEditingState state)
    {
        ThrowIfDisposed();
        _textInput.UpdateState(state);
    }

    public void SetCaretRect(Rect logicalRect)
    {
        ThrowIfDisposed();
        _textInput.SetCaretRect(logicalRect);
    }

    public void ClearClient()
    {
        if (_disposed) return;
        _textInput.ClearClient();
    }

    public ValueTask<string?> GetClipboardTextAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ThrowIfDisposed();
        var result = FlutterWindowsInputNative.GetClipboardUnicodeText(_childHwnd);
        Interlocked.Increment(ref _clipboardReadCount);
        return ValueTask.FromResult(result);
    }

    public ValueTask SetClipboardTextAsync(string text, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ArgumentNullException.ThrowIfNull(text);
        ThrowIfDisposed();
        FlutterWindowsInputNative.SetClipboardUnicodeText(_childHwnd, text);
        Interlocked.Increment(ref _clipboardWriteCount);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Stores the desired Doroti cursor.  Native cursor application occurs only
    /// for a child <c>WM_SETCURSOR</c> whose hit-test is <c>HTCLIENT</c>.
    /// </summary>
    public void SetCursor(DorotiMouseCursorKind cursor) =>
        Volatile.Write(ref _clientCursor, (int)cursor);

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            CancelAllPointersLocked();
            _router.Dispose();
            if (_touchRegistered)
            {
                _ = FlutterWindowsInputNative.UnregisterTouchWindow(_childHwnd);
                _touchRegistered = false;
                Interlocked.Increment(ref _touchUnregistrationCount);
            }
            _keyboard.Dispose();
            _textInput.Dispose();
            PointerData = null;
            KeyData = null;
            FocusData = null;
            EditingStateChanged = null;
            ActionPerformed = null;
            _disposed = true;
        }
    }

    private FlutterWindowsChildMessageResult HandleChildMessage(FlutterWindowsChildMessage message)
    {
        lock (_gate)
        {
            if (_disposed) return FlutterWindowsChildMessageResult.Unhandled;

            var imeResult = _textInput.HandleMessage(message);
            if (imeResult.Handled) return imeResult;

            if (IsKeyboardMessage(message.Message))
                return _keyboard.HandleMessage(message);

            return message.Message switch
            {
                WmSetFocus => HandleFocusChanged(message.Timestamp, focused: true),
                WmKillFocus => HandleFocusChanged(message.Timestamp, focused: false),
                WmCancelMode => HandleCancelMode(),
                WmCaptureChanged => HandleMouseCaptureChanged(message),
                WM_SETCURSOR => HandleSetCursor(message),
                WmMouseMove => HandleMouseMove(message),
                WmLeftButtonDown or WmLeftButtonDoubleClick => HandleMouseButtonDown(message, MouseButton.Left),
                WmRightButtonDown or WmRightButtonDoubleClick => HandleMouseButtonDown(message, MouseButton.Right),
                WmMiddleButtonDown or WmMiddleButtonDoubleClick => HandleMouseButtonDown(message, MouseButton.Middle),
                WmXButtonDown or WmXButtonDoubleClick => HandleMouseButtonDown(
                    message,
                    IsXButton1(message.WParam) ? MouseButton.X1 : MouseButton.X2),
                WmLeftButtonUp => HandleMouseButtonUp(message, MouseButton.Left),
                WmRightButtonUp => HandleMouseButtonUp(message, MouseButton.Right),
                WmMiddleButtonUp => HandleMouseButtonUp(message, MouseButton.Middle),
                WmXButtonUp => HandleMouseButtonUp(
                    message,
                    IsXButton1(message.WParam) ? MouseButton.X1 : MouseButton.X2),
                WmMouseWheel => HandleMouseWheel(message, horizontal: false),
                WmMouseHorizontalWheel => HandleMouseWheel(message, horizontal: true),
                WmMouseLeave => HandleMouseLeave(message.Timestamp),
                WmPointerEnter => HandlePointer(message, PointerTransition.Enter),
                WmPointerDown => HandlePointer(message, PointerTransition.Down),
                WmPointerUpdate => HandlePointer(message, PointerTransition.Update),
                WmPointerUp => HandlePointer(message, PointerTransition.Up),
                WmPointerLeave => HandlePointer(message, PointerTransition.Leave),
                WmPointerCaptureChanged => HandlePointer(message, PointerTransition.CaptureChanged),
                WmTouch => HandleTouch(message),
                _ => FlutterWindowsChildMessageResult.Unhandled,
            };
        }
    }

    private FlutterWindowsChildMessageResult HandleFocusChanged(TimeSpan timestamp, bool focused)
    {
        if (_focused == focused) return FlutterWindowsChildMessageResult.Unhandled;
        _focused = focused;
        if (focused)
        {
            Interlocked.Increment(ref _focusGainedCount);
            _textInput.OnFocusChanged(true);
        }
        else
        {
            Interlocked.Increment(ref _focusLostCount);
            _keyboard.ResetForFocusLoss();
            CancelAllPointersLocked();
            _textInput.OnFocusChanged(false);
        }
        FocusData?.Invoke(new(_viewId, focused, timestamp));
        return FlutterWindowsChildMessageResult.Unhandled;
    }

    private FlutterWindowsChildMessageResult HandleCancelMode()
    {
        CancelAllPointersLocked();
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private FlutterWindowsChildMessageResult HandleMouseCaptureChanged(FlutterWindowsChildMessage message)
    {
        if (!_releasingMouseCapture && message.LParam != _childHwnd)
            CancelAllPointersLocked();
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private FlutterWindowsChildMessageResult HandleSetCursor(FlutterWindowsChildMessage message)
    {
        var hitTest = unchecked((uint)(message.LParam.ToInt64() & 0xffff));
        if (hitTest != HtClient) return FlutterWindowsChildMessageResult.Unhandled;
        var cursor = (DorotiMouseCursorKind)Volatile.Read(ref _clientCursor);
        _ = FlutterWindowsInputNative.SetCursor(
            cursor == DorotiMouseCursorKind.none
                ? 0
                : FlutterWindowsInputNative.LoadCursorW(0, FlutterWindowsInputNative.CursorId(cursor)));
        Interlocked.Increment(ref _clientCursorHandledCount);
        return FlutterWindowsChildMessageResult.HandledResult(1);
    }

    private FlutterWindowsChildMessageResult HandleMouseButtonDown(
        FlutterWindowsChildMessage message,
        MouseButton button)
    {
        _ = FlutterWindowsInputNative.SetFocus(_childHwnd);
        var point = FlutterWindowsInputNative.PointFromLParam(message.LParam);
        TrackMouseLeaveLocked();
        EnsureMouseAddedLocked(point, message.Timestamp);
        var previousButtons = _mouseButtons;
        _mouseButtons = ButtonsFromMouseWParam(message.WParam) | ButtonMask(button);
        RaiseMouseLocked(
            previousButtons == 0 ? PointerChange.down : PointerChange.move,
            point,
            _mouseButtons,
            message.Timestamp);
        if (previousButtons == 0)
            AcquireMouseCaptureLocked();
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private FlutterWindowsChildMessageResult HandleMouseButtonUp(
        FlutterWindowsChildMessage message,
        MouseButton button)
    {
        var point = FlutterWindowsInputNative.PointFromLParam(message.LParam);
        var previousButtons = _mouseButtons;
        _mouseButtons = ButtonsFromMouseWParam(message.WParam) & ~ButtonMask(button);
        if (_mouseAdded)
        {
            RaiseMouseLocked(
                previousButtons != 0 && _mouseButtons == 0 ? PointerChange.up : PointerChange.move,
                point,
                _mouseButtons,
                message.Timestamp);
        }
        if (_mouseButtons == 0)
        {
            ReleaseMouseCaptureLocked();
            if (!IsClientPointLocked(point))
            {
                Interlocked.Increment(ref _outsideBoundsUpCount);
                RemoveMouseLocked(point, message.Timestamp);
            }
        }
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private FlutterWindowsChildMessageResult HandleMouseMove(FlutterWindowsChildMessage message)
    {
        var point = FlutterWindowsInputNative.PointFromLParam(message.LParam);
        TrackMouseLeaveLocked();
        var messageButtons = ButtonsFromMouseWParam(message.WParam);
        if (_mouseButtons != 0 || messageButtons != 0)
        {
            EnsureMouseAddedLocked(point, message.Timestamp);
            _mouseButtons = messageButtons;
            RaiseMouseLocked(PointerChange.move, point, _mouseButtons, message.Timestamp);
            return FlutterWindowsChildMessageResult.HandledResult();
        }

        EnsureMouseAddedLocked(point, message.Timestamp);
        RaiseMouseLocked(PointerChange.hover, point, 0, message.Timestamp);
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private FlutterWindowsChildMessageResult HandleMouseWheel(
        FlutterWindowsChildMessage message,
        bool horizontal)
    {
        var point = FlutterWindowsInputNative.PointFromLParam(message.LParam);
        _ = FlutterWindowsInputNative.ScreenToClient(_childHwnd, ref point);
        TrackMouseLeaveLocked();
        EnsureMouseAddedLocked(point, message.Timestamp);
        var delta = GetWheelDelta(message.WParam) / (double)MouseWheelDelta * 100.0;
        RaiseMouseLocked(
            _mouseButtons == 0 ? PointerChange.hover : PointerChange.move,
            point,
            _mouseButtons,
            message.Timestamp,
            horizontal ? delta : 0,
            horizontal ? 0 : -delta);
        Interlocked.Increment(ref _wheelCount);
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private FlutterWindowsChildMessageResult HandleMouseLeave(TimeSpan timestamp)
    {
        _trackingMouseLeave = false;
        if (_mouseButtons == 0 && _mouseAdded)
            RemoveMouseLocked(_lastMousePoint, timestamp);
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private FlutterWindowsChildMessageResult HandlePointer(
        FlutterWindowsChildMessage message,
        PointerTransition transition)
    {
        var pointerId = unchecked((uint)message.WParam & 0xffffu);
        if (pointerId == 0) pointerId = 1;
        var point = FlutterWindowsInputNative.PointFromLParam(message.LParam);
        _ = FlutterWindowsInputNative.ScreenToClient(_childHwnd, ref point);
        var kind = PointerKindFromMessage(pointerId);
        var state = GetOrCreatePointerStateLocked(pointerId, kind);
        switch (transition)
        {
            case PointerTransition.Enter:
                EnsurePointerAddedLocked(state, point, message.Timestamp);
                RaisePointerLocked(state, PointerChange.hover, point, 0, message.Timestamp);
                break;
            case PointerTransition.Down:
                _ = FlutterWindowsInputNative.SetFocus(_childHwnd);
                EnsurePointerAddedLocked(state, point, message.Timestamp);
                state.Buttons = 1;
                RaisePointerLocked(state, PointerChange.down, point, state.Buttons, message.Timestamp);
                if (FlutterWindowsInputNative.SetPointerCapture(_childHwnd, pointerId))
                {
                    state.HasCapture = true;
                    Interlocked.Increment(ref _pointerCaptureAcquireCount);
                }
                break;
            case PointerTransition.Update:
                EnsurePointerAddedLocked(state, point, message.Timestamp);
                RaisePointerLocked(
                    state,
                    state.Buttons == 0 ? PointerChange.hover : PointerChange.move,
                    point,
                    state.Buttons,
                    message.Timestamp);
                break;
            case PointerTransition.Up:
                EnsurePointerAddedLocked(state, point, message.Timestamp);
                RaisePointerLocked(state, PointerChange.up, point, 0, message.Timestamp);
                state.Buttons = 0;
                ReleasePointerCaptureLocked(state);
                RemovePointerLocked(state, point, message.Timestamp);
                break;
            case PointerTransition.Leave:
                if (state.Buttons == 0) RemovePointerLocked(state, point, message.Timestamp);
                break;
            case PointerTransition.CaptureChanged:
                CancelPointerLocked(state, point, message.Timestamp);
                break;
        }
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private FlutterWindowsChildMessageResult HandleTouch(FlutterWindowsChildMessage message)
    {
        var count = unchecked((uint)message.WParam & 0xffffu);
        if (count == 0 || message.LParam == 0) return FlutterWindowsChildMessageResult.Unhandled;
        var inputs = new FlutterWindowsInputNative.TouchInput[checked((int)count)];
        if (!FlutterWindowsInputNative.GetTouchInputInfo(
                message.LParam,
                count,
                inputs,
                Marshal.SizeOf<FlutterWindowsInputNative.TouchInput>()))
        {
            return FlutterWindowsChildMessageResult.Unhandled;
        }
        try
        {
            foreach (var input in inputs)
            {
                var pointerId = 0x1_0000UL | input.Id;
                var point = new FlutterWindowsInputNative.NativePoint(input.X / 100, input.Y / 100);
                _ = FlutterWindowsInputNative.ScreenToClient(_childHwnd, ref point);
                var state = GetOrCreatePointerStateLocked(pointerId, PointerDeviceKind.touch);
                if ((input.Flags & TouchEventDown) != 0)
                {
                    _ = FlutterWindowsInputNative.SetFocus(_childHwnd);
                    EnsurePointerAddedLocked(state, point, message.Timestamp);
                    state.Buttons = 1;
                    RaisePointerLocked(state, PointerChange.down, point, 1, message.Timestamp);
                }
                else if ((input.Flags & TouchEventUp) != 0)
                {
                    EnsurePointerAddedLocked(state, point, message.Timestamp);
                    RaisePointerLocked(state, PointerChange.up, point, 0, message.Timestamp);
                    state.Buttons = 0;
                    RemovePointerLocked(state, point, message.Timestamp);
                }
                else if ((input.Flags & TouchEventMove) != 0)
                {
                    EnsurePointerAddedLocked(state, point, message.Timestamp);
                    RaisePointerLocked(state, PointerChange.move, point, state.Buttons, message.Timestamp);
                }
            }
        }
        finally
        {
            _ = FlutterWindowsInputNative.CloseTouchInputHandle(message.LParam);
        }
        return FlutterWindowsChildMessageResult.HandledResult();
    }

    private void EnsureMouseAddedLocked(FlutterWindowsInputNative.NativePoint point, TimeSpan timestamp)
    {
        if (_mouseAdded) return;
        _mouseAdded = true;
        RaiseMouseLocked(PointerChange.add, point, 0, timestamp);
    }

    private void RemoveMouseLocked(FlutterWindowsInputNative.NativePoint point, TimeSpan timestamp)
    {
        if (!_mouseAdded) return;
        RaiseMouseLocked(PointerChange.remove, point, 0, timestamp);
        _mouseAdded = false;
        _hasLastMousePoint = false;
    }

    private void RaiseMouseLocked(
        PointerChange change,
        FlutterWindowsInputNative.NativePoint point,
        long buttons,
        TimeSpan timestamp,
        double scrollDeltaX = 0,
        double scrollDeltaY = 0)
    {
        var isSignal = scrollDeltaX != 0 || scrollDeltaY != 0;
        var resetDelta = isSignal || change is PointerChange.add or PointerChange.remove or PointerChange.cancel;
        var deltaX = resetDelta || !_hasLastMousePoint ? 0 : point.X - _lastMousePoint.X;
        var deltaY = resetDelta || !_hasLastMousePoint ? 0 : point.Y - _lastMousePoint.Y;
        DispatchPointerLocked(new(
            _viewId,
            timestamp,
            change,
            PointerDeviceKind.mouse,
            1,
            point.X,
            point.Y,
            deltaX,
            deltaY,
            buttons,
            scrollDeltaX: scrollDeltaX,
            scrollDeltaY: scrollDeltaY,
            signalKind: isSignal ? PointerSignalKind.scroll : PointerSignalKind.none,
            pointerIdentifier: change is PointerChange.down or PointerChange.move or PointerChange.up or PointerChange.cancel
                ? 1UL
                : 0UL));
        _lastMousePoint = point;
        _hasLastMousePoint = change is not (PointerChange.remove or PointerChange.cancel);
    }

    private PointerState GetOrCreatePointerStateLocked(ulong pointerId, PointerDeviceKind kind)
    {
        if (_pointerStates.TryGetValue(pointerId, out var state)) return state;
        state = new(pointerId, kind);
        _pointerStates.Add(pointerId, state);
        return state;
    }

    private void EnsurePointerAddedLocked(
        PointerState state,
        FlutterWindowsInputNative.NativePoint point,
        TimeSpan timestamp)
    {
        if (state.Added) return;
        state.Added = true;
        RaisePointerLocked(state, PointerChange.add, point, 0, timestamp);
    }

    private void RemovePointerLocked(
        PointerState state,
        FlutterWindowsInputNative.NativePoint point,
        TimeSpan timestamp)
    {
        if (!state.Added) return;
        RaisePointerLocked(state, PointerChange.remove, point, 0, timestamp);
        state.Added = false;
        state.HasLastPoint = false;
        _pointerStates.Remove(state.Id);
    }

    private void CancelPointerLocked(
        PointerState state,
        FlutterWindowsInputNative.NativePoint point,
        TimeSpan timestamp)
    {
        if (!state.Added) return;
        if (state.Buttons != 0)
            RaisePointerLocked(state, PointerChange.cancel, point, 0, timestamp);
        state.Buttons = 0;
        ReleasePointerCaptureLocked(state);
        RemovePointerLocked(state, point, timestamp);
    }

    private void RaisePointerLocked(
        PointerState state,
        PointerChange change,
        FlutterWindowsInputNative.NativePoint point,
        long buttons,
        TimeSpan timestamp)
    {
        var resetDelta = change is PointerChange.add or PointerChange.remove or PointerChange.cancel;
        var deltaX = resetDelta || !state.HasLastPoint ? 0 : point.X - state.LastPoint.X;
        var deltaY = resetDelta || !state.HasLastPoint ? 0 : point.Y - state.LastPoint.Y;
        DispatchPointerLocked(new(
            _viewId,
            timestamp,
            change,
            state.Kind,
            state.Id,
            point.X,
            point.Y,
            deltaX,
            deltaY,
            buttons,
            pointerIdentifier: state.Id));
        state.LastPoint = point;
        state.HasLastPoint = change is not (PointerChange.remove or PointerChange.cancel);
    }

    private void DispatchPointerLocked(PointerData pointer)
    {
        switch (pointer.change)
        {
            case PointerChange.add:
                Interlocked.Increment(ref _pointerAddCount);
                break;
            case PointerChange.hover:
                Interlocked.Increment(ref _pointerHoverCount);
                break;
            case PointerChange.down:
                Interlocked.Increment(ref _pointerDownCount);
                break;
            case PointerChange.move:
                Interlocked.Increment(ref _pointerMoveCount);
                break;
            case PointerChange.up:
                Interlocked.Increment(ref _pointerUpCount);
                break;
            case PointerChange.remove:
                Interlocked.Increment(ref _pointerRemoveCount);
                break;
            case PointerChange.cancel:
                Interlocked.Increment(ref _pointerCancelCount);
                break;
        }
        PointerData?.Invoke(new([pointer]));
    }

    private void AcquireMouseCaptureLocked()
    {
        _ = FlutterWindowsInputNative.SetCapture(_childHwnd);
        _mouseCapture = FlutterWindowsInputNative.GetCapture() == _childHwnd;
        if (_mouseCapture) Interlocked.Increment(ref _mouseCaptureAcquireCount);
    }

    private void ReleaseMouseCaptureLocked()
    {
        if (!_mouseCapture && FlutterWindowsInputNative.GetCapture() != _childHwnd) return;
        _releasingMouseCapture = true;
        try
        {
            _ = FlutterWindowsInputNative.ReleaseCapture();
        }
        finally
        {
            _releasingMouseCapture = false;
            _mouseCapture = false;
            Interlocked.Increment(ref _mouseCaptureReleaseCount);
        }
    }

    private void ReleasePointerCaptureLocked(PointerState state)
    {
        if (!state.HasCapture) return;
        _ = FlutterWindowsInputNative.ReleasePointerCapture(_childHwnd, checked((uint)state.Id));
        state.HasCapture = false;
        Interlocked.Increment(ref _pointerCaptureReleaseCount);
    }

    private void CancelAllPointersLocked()
    {
        var timestamp = DorotiFrameClock.Now;
        if (_mouseAdded)
        {
            if (_mouseButtons != 0)
                RaiseMouseLocked(PointerChange.cancel, _lastMousePoint, 0, timestamp);
            _mouseButtons = 0;
            ReleaseMouseCaptureLocked();
            RemoveMouseLocked(_lastMousePoint, timestamp);
        }
        foreach (var state in _pointerStates.Values.ToArray())
            CancelPointerLocked(state, state.HasLastPoint ? state.LastPoint : default, timestamp);
    }

    private void TrackMouseLeaveLocked()
    {
        if (_trackingMouseLeave) return;
        var tracking = new FlutterWindowsInputNative.NativeTrackMouseEvent(
            checked((uint)Marshal.SizeOf<FlutterWindowsInputNative.NativeTrackMouseEvent>()),
            TrackMouseEventLeave,
            _childHwnd,
            0);
        _trackingMouseLeave = FlutterWindowsInputNative.TrackMouseEvent(ref tracking);
    }

    private bool IsClientPointLocked(FlutterWindowsInputNative.NativePoint point)
    {
        if (!FlutterWindowsInputNative.GetClientRect(_childHwnd, out var rect)) return true;
        return point.X >= rect.Left && point.X < rect.Right && point.Y >= rect.Top && point.Y < rect.Bottom;
    }

    private PointerDeviceKind PointerKindFromMessage(uint pointerId)
    {
        if (!FlutterWindowsInputNative.GetPointerType(pointerId, out var type))
            return PointerDeviceKind.unknown;
        return type switch
        {
            PtTouch => PointerDeviceKind.touch,
            PtPen => PointerDeviceKind.stylus,
            PtPointer => PointerDeviceKind.mouse,
            _ => PointerDeviceKind.unknown,
        };
    }

    private static bool IsKeyboardMessage(uint message) => message is >= 0x0100 and <= 0x0109;

    private static long ButtonsFromMouseWParam(nuint wParam)
    {
        var native = unchecked((uint)wParam);
        long buttons = 0;
        if ((native & MkLeftButton) != 0) buttons |= ButtonMask(MouseButton.Left);
        if ((native & MkRightButton) != 0) buttons |= ButtonMask(MouseButton.Right);
        if ((native & MkMiddleButton) != 0) buttons |= ButtonMask(MouseButton.Middle);
        if ((native & MkXButton1) != 0) buttons |= ButtonMask(MouseButton.X1);
        if ((native & MkXButton2) != 0) buttons |= ButtonMask(MouseButton.X2);
        return buttons;
    }

    private static bool IsXButton1(nuint wParam) =>
        unchecked((uint)(wParam >> 16) & 0xffffu) == XButton1;

    private static long ButtonMask(MouseButton button) => button switch
    {
        MouseButton.Left => 1,
        MouseButton.Right => 2,
        MouseButton.Middle => 4,
        MouseButton.X1 => 8,
        MouseButton.X2 => 16,
        _ => 0,
    };

    private static int GetWheelDelta(nuint wParam) => unchecked((short)((ulong)wParam >> 16));

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);

    private sealed class PointerState(ulong id, PointerDeviceKind kind)
    {
        internal ulong Id { get; } = id;
        internal PointerDeviceKind Kind { get; } = kind;
        internal bool Added { get; set; }
        internal bool HasCapture { get; set; }
        internal bool HasLastPoint { get; set; }
        internal long Buttons { get; set; }
        internal FlutterWindowsInputNative.NativePoint LastPoint { get; set; }
    }

    private enum MouseButton
    {
        Left,
        Right,
        Middle,
        X1,
        X2,
    }

    private enum PointerTransition
    {
        Enter,
        Down,
        Update,
        Up,
        Leave,
        CaptureChanged,
    }
}

/// <summary>Child-only pointer, focus, cursor, capture, and clipboard evidence.</summary>
internal sealed record FlutterWindowsInputHostSnapshot(
    ulong ViewId,
    nint ChildHwnd,
    bool IsFocused,
    bool HasMouseCapture,
    bool HasPointerCapture,
    DorotiMouseCursorKind ClientCursor,
    long PointerAddCount,
    long PointerHoverCount,
    long PointerDownCount,
    long PointerMoveCount,
    long PointerUpCount,
    long PointerRemoveCount,
    long PointerCancelCount,
    long WheelCount,
    long MouseCaptureAcquireCount,
    long MouseCaptureReleaseCount,
    long PointerCaptureAcquireCount,
    long PointerCaptureReleaseCount,
    long FocusGainedCount,
    long FocusLostCount,
    long ClientCursorHandledCount,
    long TopLevelNonClientCursorHandledCount,
    long OutsideBoundsUpCount,
    long ClipboardReadCount,
    long ClipboardWriteCount,
    bool TouchRegistered,
    long TouchRegistrationCount,
    long TouchUnregistrationCount,
    bool IsDisposed);

/// <summary>
/// Narrow native interop for the F7 child input owner.  It deliberately has
/// no message retrieval or dispatch API: native dispatch remains owned by the
/// existing platform loop and child WndProc hook.
/// </summary>
internal static class FlutterWindowsInputNative
{
    private const uint CfUnicodeText = 13;
    private const uint GmemMoveable = 0x0002;

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativePoint
    {
        internal NativePoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        internal int X;
        internal int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeRect
    {
        internal NativeRect(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct NativeTrackMouseEvent
    {
        internal NativeTrackMouseEvent(uint size, uint flags, nint hwnd, uint hoverTime)
        {
            Size = size;
            Flags = flags;
            Hwnd = hwnd;
            HoverTime = hoverTime;
        }

        internal uint Size;
        internal uint Flags;
        internal nint Hwnd;
        internal uint HoverTime;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct TouchInput
    {
        internal int X;
        internal int Y;
        internal nint Source;
        internal uint Id;
        internal uint Flags;
        internal uint Mask;
        internal uint Time;
        internal nint ExtraInfo;
        internal uint ContactWidth;
        internal uint ContactHeight;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct CompositionForm(uint style, NativePoint currentPosition, NativeRect area)
    {
        internal readonly uint Style = style;
        internal readonly NativePoint CurrentPosition = currentPosition;
        internal readonly NativeRect Area = area;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal readonly struct CandidateForm(uint index, uint style, NativePoint currentPosition, NativeRect area)
    {
        internal readonly uint Index = index;
        internal readonly uint Style = style;
        internal readonly NativePoint CurrentPosition = currentPosition;
        internal readonly NativeRect Area = area;
    }

    internal static NativePoint PointFromLParam(nint lParam) => new(
        unchecked((short)(lParam.ToInt64() & 0xffff)),
        unchecked((short)((lParam.ToInt64() >> 16) & 0xffff)));

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

    internal static string? GetClipboardUnicodeText(nint owner)
    {
        if (!OpenClipboard(owner)) return null;
        try
        {
            var handle = GetClipboardData(CfUnicodeText);
            if (handle == 0) return null;
            var pointer = GlobalLock(handle);
            if (pointer == 0) return null;
            try { return Marshal.PtrToStringUni(pointer); }
            finally { _ = GlobalUnlock(handle); }
        }
        finally
        {
            _ = CloseClipboard();
        }
    }

    internal static void SetClipboardUnicodeText(nint owner, string text)
    {
        if (!OpenClipboard(owner))
            throw new Win32Exception(Marshal.GetLastWin32Error(), "OpenClipboard failed for the child input owner.");
        nint memory = 0;
        try
        {
            if (!EmptyClipboard())
                throw new Win32Exception(Marshal.GetLastWin32Error(), "EmptyClipboard failed for the child input owner.");
            var bytes = Encoding.Unicode.GetBytes(text + '\0');
            memory = GlobalAlloc(GmemMoveable, checked((nuint)bytes.Length));
            if (memory == 0) throw new OutOfMemoryException("GlobalAlloc failed for clipboard text.");
            var pointer = GlobalLock(memory);
            if (pointer == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GlobalLock failed for clipboard text.");
            try { Marshal.Copy(bytes, 0, pointer, bytes.Length); }
            finally { _ = GlobalUnlock(memory); }
            if (SetClipboardData(CfUnicodeText, memory) == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error(), "SetClipboardData failed for the child input owner.");
            memory = 0; // The clipboard owns it after SetClipboardData succeeds.
        }
        finally
        {
            if (memory != 0) _ = GlobalFree(memory);
            _ = CloseClipboard();
        }
    }

    [DllImport("user32.dll")]
    internal static extern nint SetFocus(nint hwnd);

    [DllImport("user32.dll")]
    internal static extern nint GetFocus();

    [DllImport("user32.dll")]
    internal static extern nint SetCapture(nint hwnd);

    [DllImport("user32.dll")]
    internal static extern nint GetCapture();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ReleaseCapture();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetClientRect(nint hwnd, out NativeRect rect);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ScreenToClient(nint hwnd, ref NativePoint point);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ClientToScreen(nint hwnd, ref NativePoint point);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool TrackMouseEvent(ref NativeTrackMouseEvent tracking);

    [DllImport("user32.dll")]
    internal static extern nint LoadCursorW(nint instance, int cursorName);

    [DllImport("user32.dll")]
    internal static extern nint SetCursor(nint cursor);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetPointerType(uint pointerId, out uint pointerType);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetPointerCapture(nint hwnd, uint pointerId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ReleasePointerCapture(nint hwnd, uint pointerId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetTouchInputInfo(
        nint touchInput,
        uint count,
        [In, Out] TouchInput[] inputs,
        int size);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CloseTouchInputHandle(nint touchInput);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterTouchWindow(nint hwnd, uint flags);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterTouchWindow(nint hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CreateCaret(nint hwnd, nint bitmap, int width, int height);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DestroyCaret();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShowCaret(nint hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool HideCaret(nint hwnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetCaretPos(int x, int y);

    [DllImport("imm32.dll")]
    internal static extern nint ImmGetContext(nint hwnd);

    [DllImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ImmReleaseContext(nint hwnd, nint context);

    [DllImport("imm32.dll", CharSet = CharSet.Unicode)]
    internal static extern int ImmGetCompositionStringW(nint context, int index, nint buffer, uint bufferLength);

    [DllImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ImmSetCompositionWindow(nint context, in CompositionForm form);

    [DllImport("imm32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ImmSetCandidateWindow(nint context, in CandidateForm form);

    [DllImport("imm32.dll", EntryPoint = "ImmNotifyIME")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ImmNotifyIME(nint context, uint action, uint index, uint value);

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

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalAlloc(uint flags, nuint bytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalLock(nint memory);

    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GlobalUnlock(nint memory);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GlobalFree(nint memory);
}
