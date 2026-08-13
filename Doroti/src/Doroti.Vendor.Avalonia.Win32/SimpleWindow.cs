// Adapted from A0-pinned Avalonia WindowImpl; see migration/avalonia-shell/a1-source-port-provenance.json.
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Doroti.Graphics;

namespace Doroti.Vendor.Avalonia.Win32;

internal sealed class NativeWindowHost : IDisposable
{
    private static readonly ConcurrentDictionary<nint, NativeWindowHost> Instances = new();
    private static readonly NativeInterop.WindowProcedure RootWindowProcedure = WindowProcedure;
    private static readonly bool DpiAwarenessConfigured = ConfigureDpiAwareness();
    private static readonly ushort ClassAtom = RegisterWindowClass();
    private static long s_nextWindowId;

    private readonly Action<NativeWindowNotification> _windowSink;
    private readonly Action<NativePointerEvent> _pointerSink;
    private readonly Action<NativeKeyEvent> _keySink;
    private readonly Action<NativeTextEvent> _textSink;
    private ExceptionDispatchInfo? _pendingException;
    private nint _cursor;
    private bool _mouseInside;
    private int _validationMouseMoves;
    private char? _pendingHighSurrogate;
    private NativePointerEvent _lastPointer;
    private bool _isMinimized;
    private bool _closeRequested;

    internal NativeWindowHost(
        string title,
        double logicalWidth,
        double logicalHeight,
        Action<NativeWindowNotification> windowSink,
        Action<NativePointerEvent> pointerSink,
        Action<NativeKeyEvent> keySink,
        Action<NativeTextEvent> textSink)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("The Doroti Win32 backend requires Windows.");
        }

        _ = DpiAwarenessConfigured;

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(logicalWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(logicalHeight);
        _windowSink = windowSink;
        _pointerSink = pointerSink;
        _keySink = keySink;
        _textSink = textSink;
        WindowId = unchecked((ulong)Interlocked.Increment(ref s_nextWindowId));
        _cursor = NativeInterop.LoadCursor(0, NativeInterop.CursorArrow);

        var root = GCHandle.Alloc(this);
        try
        {
            Handle = NativeInterop.CreateWindow(
                0,
                ClassAtom,
                title,
                (uint)NativeInterop.WindowStyles.OverlappedWindow,
                NativeInterop.CreateUseDefault,
                NativeInterop.CreateUseDefault,
                checked((int)Math.Ceiling(logicalWidth)),
                checked((int)Math.Ceiling(logicalHeight)),
                0,
                0,
                0,
                GCHandle.ToIntPtr(root));
            if (Handle == 0)
            {
                throw new Win32Exception(Marshal.GetLastPInvokeError(), "CreateWindowExW failed.");
            }
        }
        finally
        {
            root.Free();
        }

        Resize(logicalWidth, logicalHeight);
        NotifyMetrics();
        ThrowPendingException();
    }

    internal ulong WindowId { get; }

    internal nint Handle { get; private set; }

    internal NativeWindowEvent Metrics => ReadMetrics();

    internal IReadOnlyList<NativeDisplayInfo> Displays => ReadDisplays();

    internal Func<nint, nuint, nint, nint>? AutomationRequested { get; set; }

    internal NativeInterop.NativeRect WindowBounds
    {
        get
        {
            return Handle != 0 && NativeInterop.GetWindowRect(Handle, out var bounds)
                ? bounds
                : default;
        }
    }

    internal NativeInterop.NativeRect LogicalClientRectToScreen(double left, double top, double right, double bottom)
    {
        if (Handle == 0)
        {
            return default;
        }

        var origin = new NativeInterop.NativePoint();
        if (!NativeInterop.ClientToScreen(Handle, ref origin))
        {
            return default;
        }

        var pixelBounds = PixelExtentPolicy.ToPixelRect(new(left, top, right, bottom), GetScaleFactor());
        return new NativeInterop.NativeRect
        {
            Left = origin.X + pixelBounds.Left,
            Top = origin.Y + pixelBounds.Top,
            Right = origin.X + pixelBounds.Right,
            Bottom = origin.Y + pixelBounds.Bottom,
        };
    }

    internal void Show()
    {
        ThrowIfClosed();
        _ = NativeInterop.ShowWindow(Handle, NativeInterop.ShowWindowCommand.ShowNormal);
        _ = NativeInterop.UpdateWindow(Handle);
        ThrowPendingException();
    }

    internal void RequestFocus(bool focused)
    {
        ThrowIfClosed();
        _ = NativeInterop.SetFocus(focused ? Handle : 0);
        ThrowPendingException();
    }

    internal void Resize(double logicalWidth, double logicalHeight)
    {
        ThrowIfClosed();
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(logicalWidth);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(logicalHeight);
        _ = NativeInterop.GetWindowRect(Handle, out var window);
        _ = NativeInterop.GetClientRect(Handle, out var client);
        var scale = GetScaleFactor();
        var clientExtent = PixelExtentPolicy.ToPixelSize(new(logicalWidth, logicalHeight), scale);
        var width = checked((int)clientExtent.Width) + Math.Max(0, window.Width - client.Width);
        var height = checked((int)clientExtent.Height) + Math.Max(0, window.Height - client.Height);
        if (!NativeInterop.SetWindowPos(
                Handle,
                0,
                window.Left,
                window.Top,
                width,
                height,
                NativeInterop.SetWindowNoActivate | NativeInterop.SetWindowNoZOrder))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "SetWindowPos failed while resizing the window.");
        }
        ThrowPendingException();
    }

    internal void SetMinimized(bool minimized)
    {
        ThrowIfClosed();
        _isMinimized = minimized;
        _ = NativeInterop.ShowWindow(
            Handle,
            minimized ? NativeInterop.ShowWindowCommand.ShowMinimized : NativeInterop.ShowWindowCommand.Restore);
        ThrowPendingException();
    }

    internal void MoveToDisplay(ulong displayId)
    {
        ThrowIfClosed();
        var display = ReadDisplays().SingleOrDefault(item => item.Id == displayId);
        if (display.Id == 0)
        {
            throw new ArgumentException($"Display {displayId} is not connected.", nameof(displayId));
        }
        if (!NativeInterop.GetWindowRect(Handle, out var window))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "GetWindowRect failed while moving the window.");
        }
        var x = display.WorkArea.Left + Math.Max(0, (display.WorkArea.Width - window.Width) / 2);
        var y = display.WorkArea.Top + Math.Max(0, (display.WorkArea.Height - window.Height) / 2);
        if (!NativeInterop.SetWindowPos(
                Handle,
                0,
                x,
                y,
                window.Width,
                window.Height,
                NativeInterop.SetWindowNoActivate | NativeInterop.SetWindowNoZOrder))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "SetWindowPos failed while moving the window to another display.");
        }
        ThrowPendingException();
    }

    internal void SetCursor(int cursorIdentifier)
    {
        ThrowIfClosed();
        _cursor = cursorIdentifier == 0 ? 0 : NativeInterop.LoadCursor(0, cursorIdentifier);
        _ = NativeInterop.SetCursor(_cursor);
    }

    internal void PostPointerTap(double logicalX, double logicalY)
    {
        ThrowIfClosed();
        var scale = GetScaleFactor();
        var x = checked((short)Math.Round(logicalX * scale));
        var y = checked((short)Math.Round(logicalY * scale));
        var packed = unchecked((uint)(ushort)x) | (unchecked((uint)(ushort)y) << 16);
        PostValidationMouseMove(0, packed);
        PostMessage(NativeInterop.WindowMessage.LeftButtonDown, 1, packed);
        PostMessage(NativeInterop.WindowMessage.LeftButtonUp, 0, packed);
    }

    internal void PostPointerMove(double logicalX, double logicalY)
    {
        ThrowIfClosed();
        PostValidationMouseMove(0, PackLogicalPoint(logicalX, logicalY));
    }

    internal void PostPointerLeave(double logicalX, double logicalY)
    {
        ThrowIfClosed();
        PostValidationMouseMove(0, PackLogicalPoint(logicalX, logicalY));
        PostMessage(NativeInterop.WindowMessage.MouseLeave, 0, 0);
    }

    internal void PostPointerDown(double logicalX, double logicalY)
    {
        ThrowIfClosed();
        var point = PackLogicalPoint(logicalX, logicalY);
        PostValidationMouseMove(0, point);
        PostMessage(NativeInterop.WindowMessage.LeftButtonDown, 1, point);
    }

    internal void PostPointerUp(double logicalX, double logicalY)
    {
        ThrowIfClosed();
        PostMessage(NativeInterop.WindowMessage.LeftButtonUp, 0, PackLogicalPoint(logicalX, logicalY));
    }

    internal void PostPointerDrag(double logicalStartX, double logicalStartY, double logicalEndX, double logicalEndY)
    {
        ThrowIfClosed();
        var start = PackLogicalPoint(logicalStartX, logicalStartY);
        var end = PackLogicalPoint(logicalEndX, logicalEndY);
        PostValidationMouseMove(0, start);
        PostMessage(NativeInterop.WindowMessage.LeftButtonDown, 1, start);
        PostValidationMouseMove(1, end);
        PostMessage(NativeInterop.WindowMessage.LeftButtonUp, 0, end);
    }

    internal void PostPointerWheel(double logicalX, double logicalY, double wheelDeltaX, double wheelDeltaY)
    {
        ThrowIfClosed();
        if (wheelDeltaX != 0)
        {
            PostMessage(NativeInterop.WindowMessage.MouseHorizontalWheel, PackWheelDelta(wheelDeltaX), PackWheelPoint(logicalX, logicalY));
        }
        if (wheelDeltaY != 0)
        {
            PostMessage(NativeInterop.WindowMessage.MouseWheel, PackWheelDelta(wheelDeltaY), PackWheelPoint(logicalX, logicalY));
        }
    }

    internal void PostPointerCaptureLoss(double logicalX, double logicalY)
    {
        ThrowIfClosed();
        var packed = PackLogicalPoint(logicalX, logicalY);
        PostValidationMouseMove(0, packed);
        PostMessage(NativeInterop.WindowMessage.LeftButtonDown, 1, packed);
        PostMessage(NativeInterop.WindowMessage.CancelMode, 0, 0);
    }

    internal void PostKeyboardActivation(uint logicalKey)
    {
        ThrowIfClosed();
        var (scanCode, extended) = logicalKey switch
        {
            0x09 => (0x0Fu, false),
            0x0D => (0x1Cu, false),
            0x20 => (0x39u, false),
            0x22 => (0x51u, true),
            _ => (0u, false),
        };
        if (scanCode == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(logicalKey), "Only Tab, Enter, Space, and PageDown are supported by the target input validator.");
        }
        var down = 1u | (scanCode << 16) | (extended ? 1u << 24 : 0u);
        var up = down | (1u << 30) | (1u << 31);
        PostMessage(NativeInterop.WindowMessage.KeyDown, logicalKey, down);
        PostMessage(NativeInterop.WindowMessage.KeyUp, logicalKey, up);
    }

    internal void PostTextInput(string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        foreach (var codeUnit in text)
        {
            PostMessage(NativeInterop.WindowMessage.Character, codeUnit, 0);
        }
    }

    internal void Present(ReadOnlySpan<byte> pixels, int width, int height, int rowBytes)
    {
        ThrowIfClosed();
        FramebufferPresenter.Present(Handle, pixels, width, height, rowBytes);
    }

    internal void PumpPendingMessages()
    {
        while (NativeInterop.PeekMessage(out var message, 0, 0, 0, NativeInterop.PeekRemove))
        {
            _ = NativeInterop.TranslateMessage(in message);
            _ = NativeInterop.DispatchMessage(in message);
            ThrowPendingException();
        }
    }

    internal void Close()
    {
        if (Handle == 0)
        {
            return;
        }
        if (!_closeRequested)
        {
            _closeRequested = true;
            _windowSink(new(NativeWindowNotificationKind.CloseRequested, ReadMetrics()));
        }
        if (!NativeInterop.DestroyWindow(Handle))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "DestroyWindow failed.");
        }
        ThrowPendingException();
    }

    public void Dispose() => Close();

    private static ushort RegisterWindowClass()
    {
        if (!OperatingSystem.IsWindows())
        {
            return 0;
        }

        var windowClass = new NativeInterop.WindowClass
        {
            Size = checked((uint)Marshal.SizeOf<NativeInterop.WindowClass>()),
            Style = NativeInterop.ClassDoubleClicks | NativeInterop.ClassOwnDc,
            WindowProcedure = RootWindowProcedure,
            Cursor = NativeInterop.LoadCursor(0, NativeInterop.CursorArrow),
            BackgroundBrush = NativeInterop.ColorWindow + 1,
            ClassName = $"Doroti.SourcePortWindow.{Environment.ProcessId}",
        };
        var atom = NativeInterop.RegisterClass(ref windowClass);
        return atom != 0
            ? atom
            : throw new Win32Exception(Marshal.GetLastPInvokeError(), "RegisterClassExW failed.");
    }

    private static bool ConfigureDpiAwareness()
    {
        if (!OperatingSystem.IsWindows())
        {
            return false;
        }

        // DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2. Access denied means the host selected a context first.
        return NativeInterop.SetProcessDpiAwarenessContext(-4) || Marshal.GetLastPInvokeError() == 5;
    }

    private static nint WindowProcedure(nint window, uint messageId, nuint wParam, nint lParam)
    {
        NativeWindowHost? host;
        if (messageId == 0x0081)
        {
            var root = Marshal.ReadIntPtr(lParam);
            host = GCHandle.FromIntPtr(root).Target as NativeWindowHost;
            if (host is not null)
            {
                Instances[window] = host;
            }
        }
        else
        {
            _ = Instances.TryGetValue(window, out host);
        }

        if (host is null)
        {
            return NativeInterop.DefaultWindowProcedure(window, messageId, wParam, lParam);
        }

        try
        {
            return host.ProcessMessage(window, (NativeInterop.WindowMessage)messageId, wParam, lParam);
        }
        catch (Exception exception)
        {
            host._pendingException ??= ExceptionDispatchInfo.Capture(exception);
            return NativeInterop.DefaultWindowProcedure(window, messageId, wParam, lParam);
        }
    }

    private nint ProcessMessage(nint window, NativeInterop.WindowMessage message, nuint wParam, nint lParam)
    {
        if (message is NativeInterop.WindowMessage.PointerCaptureChanged)
        {
            CancelPointerCapture();
            _windowSink(new(NativeWindowNotificationKind.CaptureCancelled, ReadMetrics()));
            return 0;
        }
        if (WindowEventTranslator.TryTranslatePointerMessage(window, WindowId, message, wParam, GetScaleFactor(), out var pointerMessage))
        {
            if (pointerMessage.Phase is NativePointerPhase.Down)
            {
                _ = NativeInterop.SetCapture(window);
            }
            _lastPointer = pointerMessage;
            _pointerSink(pointerMessage);
            if (pointerMessage.Phase is NativePointerPhase.Up or NativePointerPhase.Cancelled && NativeInterop.GetCapture() == window)
            {
                _ = NativeInterop.ReleaseCapture();
            }
            return 0;
        }
        if (WindowEventTranslator.TryTranslatePointer(WindowId, message, wParam, lParam, GetScaleFactor(), out var pointer))
        {
            if (message is NativeInterop.WindowMessage.MouseWheel or NativeInterop.WindowMessage.MouseHorizontalWheel)
            {
                var point = new NativeInterop.NativePoint
                {
                    X = checked((int)Math.Round(pointer.LogicalX * GetScaleFactor())),
                    Y = checked((int)Math.Round(pointer.LogicalY * GetScaleFactor())),
                };
                if (NativeInterop.ScreenToClient(window, ref point))
                {
                    var scale = GetScaleFactor();
                    pointer = pointer with { LogicalX = point.X / scale, LogicalY = point.Y / scale };
                }
            }
            var validationMouseMove = message is NativeInterop.WindowMessage.MouseMove &&
                Interlocked.CompareExchange(ref _validationMouseMoves, 0, 0) > 0;
            if (validationMouseMove)
            {
                Interlocked.Decrement(ref _validationMouseMoves);
            }
            if (!_mouseInside && message is NativeInterop.WindowMessage.MouseMove)
            {
                _mouseInside = true;
                if (!validationMouseMove)
                {
                    var request = NativeInterop.TrackMouseEventRequest.Leave(window);
                    _ = NativeInterop.TrackMouseEvent(ref request);
                }
                _pointerSink(pointer with { Phase = NativePointerPhase.Added });
            }
            if (pointer.Phase is NativePointerPhase.Down)
            {
                _ = NativeInterop.SetCapture(window);
            }
            _lastPointer = pointer;
            var capturedLeave = pointer.Phase is NativePointerPhase.Removed && NativeInterop.GetCapture() == window;
            if (!capturedLeave)
            {
                _pointerSink(pointer);
            }
            if (pointer.Phase is NativePointerPhase.Up && pointer.Buttons == 0 && NativeInterop.GetCapture() == window)
            {
                _ = NativeInterop.ReleaseCapture();
            }
            if (pointer.Phase is NativePointerPhase.Removed)
            {
                _mouseInside = false;
            }
            return 0;
        }
        if (WindowEventTranslator.TryTranslateKey(WindowId, message, wParam, lParam, out var key))
        {
            _keySink(key);
            return 0;
        }

        switch (message)
        {
            case NativeInterop.WindowMessage.GetObject when
                lParam == NativeInterop.UiaRootObjectId && AutomationRequested is { } automationRequested:
                return automationRequested(window, wParam, lParam);
            case NativeInterop.WindowMessage.Activate:
                _windowSink(new(
                    (wParam.ToUInt64() & 0xffff) == 0
                        ? NativeWindowNotificationKind.Deactivated
                        : NativeWindowNotificationKind.Activated,
                    ReadMetrics()));
                return NativeInterop.DefaultWindowProcedure(window, (uint)message, wParam, lParam);
            case NativeInterop.WindowMessage.SetFocus:
                _windowSink(new(NativeWindowNotificationKind.Activated, ReadMetrics()));
                return NativeInterop.DefaultWindowProcedure(window, (uint)message, wParam, lParam);
            case NativeInterop.WindowMessage.CaptureChanged:
            case NativeInterop.WindowMessage.CancelMode:
                CancelPointerCapture();
                _windowSink(new(NativeWindowNotificationKind.CaptureCancelled, ReadMetrics()));
                return 0;
            case NativeInterop.WindowMessage.KillFocus:
                CancelPointerCapture();
                _windowSink(new(NativeWindowNotificationKind.Deactivated, ReadMetrics()));
                return NativeInterop.DefaultWindowProcedure(window, (uint)message, wParam, lParam);
            case NativeInterop.WindowMessage.Close:
                CancelPointerCapture();
                if (!_closeRequested)
                {
                    _closeRequested = true;
                    _windowSink(new(NativeWindowNotificationKind.CloseRequested, ReadMetrics()));
                }
                if (Handle != 0 && !NativeInterop.DestroyWindow(Handle))
                {
                    throw new Win32Exception(Marshal.GetLastPInvokeError(), "DestroyWindow failed while accepting WM_CLOSE.");
                }
                return 0;
            case NativeInterop.WindowMessage.Destroy:
                var metrics = ReadMetrics();
                Instances.TryRemove(window, out _);
                Handle = 0;
                _windowSink(new(NativeWindowNotificationKind.Closed, metrics));
                return 0;
            case NativeInterop.WindowMessage.Size:
                _isMinimized = wParam.ToUInt64() == 1;
                NotifyMetrics();
                return 0;
            case NativeInterop.WindowMessage.DpiChanged:
                var suggested = Marshal.PtrToStructure<NativeInterop.NativeRect>(lParam);
                _ = NativeInterop.SetWindowPos(
                    window,
                    0,
                    suggested.Left,
                    suggested.Top,
                    suggested.Width,
                    suggested.Height,
                    NativeInterop.SetWindowNoActivate | NativeInterop.SetWindowNoZOrder);
                NotifyMetrics();
                return 0;
            case NativeInterop.WindowMessage.SetCursor:
                if (unchecked((ushort)lParam.ToInt64()) == NativeInterop.HitTestClient)
                {
                    _ = NativeInterop.SetCursor(_cursor);
                    return 1;
                }
                return NativeInterop.DefaultWindowProcedure(window, (uint)message, wParam, lParam);
            case NativeInterop.WindowMessage.Character:
                EmitCharacter(unchecked((char)wParam.ToUInt64()));
                return 0;
            case NativeInterop.WindowMessage.ImeStartComposition:
                _textSink(new(NativeTextEventKind.CompositionStarted, string.Empty));
                return 0;
            case NativeInterop.WindowMessage.ImeComposition:
                var composition = ReadImeString(window, lParam, NativeInterop.GcsResultString);
                var kind = NativeTextEventKind.Text;
                if (composition is null)
                {
                    composition = ReadImeString(window, lParam, NativeInterop.GcsCompositionString);
                    kind = NativeTextEventKind.CompositionUpdated;
                }
                if (composition is not null)
                {
                    _textSink(new(kind, composition));
                }
                return 0;
            case NativeInterop.WindowMessage.ImeEndComposition:
                _textSink(new(NativeTextEventKind.CompositionEnded, string.Empty));
                return 0;
            default:
                return NativeInterop.DefaultWindowProcedure(window, (uint)message, wParam, lParam);
        }
    }

    private void EmitCharacter(char codeUnit)
    {
        if (char.IsHighSurrogate(codeUnit))
        {
            if (_pendingHighSurrogate is not null)
            {
                _textSink(new(NativeTextEventKind.Text, "\uFFFD"));
            }
            _pendingHighSurrogate = codeUnit;
            return;
        }
        if (char.IsLowSurrogate(codeUnit))
        {
            var text = _pendingHighSurrogate is { } high
                ? new string([high, codeUnit])
                : "\uFFFD";
            _pendingHighSurrogate = null;
            _textSink(new(NativeTextEventKind.Text, text));
            return;
        }
        if (_pendingHighSurrogate is not null)
        {
            _pendingHighSurrogate = null;
            _textSink(new(NativeTextEventKind.Text, "\uFFFD"));
        }
        _textSink(new(NativeTextEventKind.Text, codeUnit.ToString()));
    }

    private static string? ReadImeString(nint window, nint lParam, uint kind)
    {
        if ((unchecked((uint)lParam.ToInt64()) & kind) == 0)
        {
            return null;
        }
        var context = NativeInterop.ImmGetContext(window);
        if (context == 0)
        {
            return null;
        }
        try
        {
            var byteCount = NativeInterop.ImmGetCompositionString(context, kind, 0, 0);
            if (byteCount <= 0)
            {
                return string.Empty;
            }
            var buffer = Marshal.AllocHGlobal(byteCount);
            try
            {
                var actual = NativeInterop.ImmGetCompositionString(context, kind, buffer, checked((uint)byteCount));
                return actual > 0 ? Marshal.PtrToStringUni(buffer, actual / sizeof(char)) : string.Empty;
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        finally
        {
            _ = NativeInterop.ImmReleaseContext(window, context);
        }
    }

    private NativeWindowEvent ReadMetrics()
    {
        var handle = Handle;
        if (handle == 0 || !NativeInterop.GetClientRect(handle, out var client))
        {
            return new(WindowId, 0, 0, 0, 0, 1, true);
        }
        var scale = GetScaleFactor();
        var minimized = _isMinimized || client.Width == 0 || client.Height == 0;
        return new(WindowId, client.Width / scale, client.Height / scale, client.Width, client.Height, scale, minimized);
    }

    private static IReadOnlyList<NativeDisplayInfo> ReadDisplays()
    {
        var displays = new List<NativeDisplayInfo>();
        var callbackError = 0;
        NativeInterop.MonitorEnumerationProcedure callback = (
            nint monitor,
            nint deviceContext,
            ref NativeInterop.NativeRect rectangle,
            nint data) =>
        {
            _ = deviceContext;
            _ = rectangle;
            _ = data;
            var info = NativeInterop.MonitorInfo.Create();
            if (!NativeInterop.GetMonitorInfo(monitor, ref info))
            {
                callbackError = Marshal.GetLastPInvokeError();
                return false;
            }
            displays.Add(new(unchecked((ulong)monitor.ToInt64()), info.WorkArea));
            return true;
        };
        if (!NativeInterop.EnumDisplayMonitors(0, 0, callback, 0))
        {
            throw new Win32Exception(
                callbackError == 0 ? Marshal.GetLastPInvokeError() : callbackError,
                "EnumDisplayMonitors failed.");
        }
        return displays;
    }

    private double GetScaleFactor()
    {
        var dpi = Handle == 0 ? 96u : NativeInterop.GetDpiForWindow(Handle);
        return dpi == 0 ? 1 : dpi / 96d;
    }

    private void NotifyMetrics() => _windowSink(new(NativeWindowNotificationKind.MetricsChanged, ReadMetrics()));

    private void PostMessage(NativeInterop.WindowMessage message, nuint wParam, uint lParam)
    {
        if (!NativeInterop.PostMessage(Handle, (uint)message, wParam, unchecked((nint)(int)lParam)))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), $"PostMessageW failed for {message}.");
        }
    }

    private void PostValidationMouseMove(nuint wParam, uint lParam)
    {
        Interlocked.Increment(ref _validationMouseMoves);
        try
        {
            PostMessage(NativeInterop.WindowMessage.MouseMove, wParam, lParam);
        }
        catch
        {
            Interlocked.Decrement(ref _validationMouseMoves);
            throw;
        }
    }

    private uint PackLogicalPoint(double logicalX, double logicalY)
    {
        var scale = GetScaleFactor();
        var x = checked((short)Math.Round(logicalX * scale));
        var y = checked((short)Math.Round(logicalY * scale));
        return unchecked((uint)(ushort)x) | (unchecked((uint)(ushort)y) << 16);
    }

    private static nuint PackWheelDelta(double value)
    {
        var wheelDelta = checked((short)Math.Round(value * 120));
        return unchecked((nuint)(uint)(unchecked((ushort)wheelDelta) << 16));
    }

    private uint PackWheelPoint(double logicalX, double logicalY)
    {
        var scale = GetScaleFactor();
        var point = new NativeInterop.NativePoint
        {
            X = checked((int)Math.Round(logicalX * scale)),
            Y = checked((int)Math.Round(logicalY * scale)),
        };
        if (!NativeInterop.ClientToScreen(Handle, ref point))
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "ClientToScreen failed for pointer validation input.");
        }
        var x = checked((short)point.X);
        var y = checked((short)point.Y);
        return unchecked((uint)(ushort)x) | (unchecked((uint)(ushort)y) << 16);
    }

    private void CancelPointerCapture()
    {
        if (_lastPointer.WindowId != 0 && (_lastPointer.Buttons != 0 || NativeInterop.GetCapture() == Handle))
        {
            _lastPointer = _lastPointer with { Phase = NativePointerPhase.Cancelled, Buttons = 0 };
            _pointerSink(_lastPointer);
        }
        if (Handle != 0 && NativeInterop.GetCapture() == Handle)
        {
            _ = NativeInterop.ReleaseCapture();
        }
    }

    private void ThrowIfClosed() => ObjectDisposedException.ThrowIf(Handle == 0, this);

    private void ThrowPendingException()
    {
        var pending = Interlocked.Exchange(ref _pendingException, null);
        pending?.Throw();
    }
}

internal readonly record struct NativeDisplayInfo(ulong Id, NativeInterop.NativeRect WorkArea);
