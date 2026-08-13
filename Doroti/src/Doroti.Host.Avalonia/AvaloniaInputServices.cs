using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Input.TextInput;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Doroti.Graphics;
using Doroti.Platform;
using AvaloniaCursor = Avalonia.Input.Cursor;
using AvaloniaTextSelection = Avalonia.Input.TextInput.TextSelection;
using DorotiTextSelection = Doroti.Platform.TextSelection;

namespace Doroti.Host.Avalonia;

internal sealed class AvaloniaRawInputSource : IRawInputSource, IAvaloniaInputDiagnosticController, IDisposable
{
    private readonly AvaloniaDisplayListControl _control;
    private readonly Window _window;
    private readonly WindowId _windowId;
    private readonly Func<WindowMetrics> _metrics;
    private readonly AvaloniaHostDiagnostics _diagnostics;
    private readonly HashSet<IRawInputSink> _sinks = [];
    private readonly Dictionary<int, ActivePointer> _activePointers = [];
    private readonly HashSet<PhysicalKey> _pressedKeys = [];
    private bool _disposed;

    internal AvaloniaRawInputSource(
        AvaloniaDisplayListControl control,
        Window window,
        WindowId windowId,
        Func<WindowMetrics> metrics,
        AvaloniaHostDiagnostics diagnostics)
    {
        _control = control;
        _window = window;
        _windowId = windowId;
        _metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
        _diagnostics = diagnostics;
        control.PointerEntered += OnPointerEntered;
        control.PointerExited += OnPointerExited;
        control.PointerMoved += OnPointerMoved;
        control.PointerPressed += OnPointerPressed;
        control.PointerReleased += OnPointerReleased;
        control.PointerWheelChanged += OnPointerWheelChanged;
        control.PointerCaptureLost += OnPointerCaptureLost;
        control.KeyDown += OnKeyDown;
        control.KeyUp += OnKeyUp;
        control.GotFocus += OnGotFocus;
        control.LostFocus += OnLostFocus;
        window.Deactivated += OnWindowDeactivated;
    }

    public InputCapabilities Capabilities { get; } = new(
        Mouse: true,
        Touch: true,
        Pen: true,
        Wheel: true,
        PointerCapture: true,
        PhysicalKeys: true,
        TextInput: true);

    public void Attach(IRawInputSink sink)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(sink);
        AvaloniaWindowBackend.RequireUiThread();
        _sinks.Add(sink);
    }

    public void Detach(IRawInputSink sink)
    {
        ArgumentNullException.ThrowIfNull(sink);
        AvaloniaWindowBackend.RequireUiThread();
        _sinks.Remove(sink);
    }

    public void EmitPointer(
        string correlationId,
        ulong deviceId,
        PointerDeviceKind deviceKind,
        PointerPhase phase,
        Offset logicalPosition,
        uint buttons,
        Offset platformScrollDelta = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(correlationId);
        AvaloniaWindowBackend.RequireUiThread();
        if (!logicalPosition.IsFinite || !platformScrollDelta.IsFinite)
        {
            throw new ArgumentOutOfRangeException(nameof(logicalPosition), "Diagnostic pointer coordinates and deltas must be finite.");
        }
        var input = new RawPointerEvent(
            _windowId,
            deviceId,
            deviceKind,
            phase,
            logicalPosition,
            buttons,
            TimeSpan.FromMilliseconds(Environment.TickCount64),
            PointerScrollNormalizer.NormalizeAvalonia(platformScrollDelta, ReadWindowsWheelScrollLines()));
        _diagnostics.Record(
            "raw-platform-delta",
            _windowId,
            ReadMetrics(),
            $"correlation={correlationId};device={deviceId};kind={deviceKind};phase={phase};x={logicalPosition.X:0.###};y={logicalPosition.Y:0.###};buttons={buttons};wheel={platformScrollDelta.X:0.###},{platformScrollDelta.Y:0.###};source=diagnostic-replay");
        foreach (var sink in _sinks.ToArray())
        {
            sink.OnPointer(input);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        AvaloniaWindowBackend.RequireUiThread();
        _disposed = true;
        CancelActivePointers("dispose");
        _control.PointerEntered -= OnPointerEntered;
        _control.PointerExited -= OnPointerExited;
        _control.PointerMoved -= OnPointerMoved;
        _control.PointerPressed -= OnPointerPressed;
        _control.PointerReleased -= OnPointerReleased;
        _control.PointerWheelChanged -= OnPointerWheelChanged;
        _control.PointerCaptureLost -= OnPointerCaptureLost;
        _control.KeyDown -= OnKeyDown;
        _control.KeyUp -= OnKeyUp;
        _control.GotFocus -= OnGotFocus;
        _control.LostFocus -= OnLostFocus;
        _window.Deactivated -= OnWindowDeactivated;
        _sinks.Clear();
        _pressedKeys.Clear();
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (!_activePointers.ContainsKey(e.Pointer.Id))
        {
            EmitPointer(e, PointerPhase.Added);
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (!_activePointers.ContainsKey(e.Pointer.Id))
        {
            EmitPointer(e, PointerPhase.Removed);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e) =>
        EmitPointer(e, ReadButtons(e.Properties) == 0 ? PointerPhase.Hover : PointerPhase.Move);

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(_control);
        _activePointers[e.Pointer.Id] = new(
            e.Pointer,
            ReadDeviceKind(e.Pointer),
            new(point.Position.X, point.Position.Y));
        e.Pointer.Capture(_control);
        _control.Focus();
        EmitPointer(e, PointerPhase.Down);
        _diagnostics.Record("pointer-captured", _windowId, ReadMetrics(), $"device={e.Pointer.Id}");
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        EmitPointer(e, PointerPhase.Up);
        _activePointers.Remove(e.Pointer.Id);
        if (ReferenceEquals(e.Pointer.Captured, _control))
        {
            e.Pointer.Capture(null);
        }
        _diagnostics.Record("pointer-released", _windowId, ReadMetrics(), $"device={e.Pointer.Id}");
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e) =>
        EmitPointer(e, PointerPhase.Move, new(e.Delta.X, e.Delta.Y));

    private void OnPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (_activePointers.Remove(e.Pointer.Id, out var active))
        {
            EmitCancelled(active, "capture-lost");
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        var phase = _pressedKeys.Add(e.PhysicalKey) ? KeyPhase.Down : KeyPhase.Repeat;
        EmitKey(e, phase);
    }

    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        _pressedKeys.Remove(e.PhysicalKey);
        EmitKey(e, KeyPhase.Up);
    }

    private void OnGotFocus(object? sender, FocusChangedEventArgs e) => EmitFocus(true, "control");

    private void OnLostFocus(object? sender, FocusChangedEventArgs e)
    {
        EmitFocus(false, "control");
        CancelActivePointers("focus-lost");
        _pressedKeys.Clear();
    }

    private void OnWindowDeactivated(object? sender, EventArgs e)
    {
        EmitFocus(false, "window-deactivated");
        CancelActivePointers("window-deactivated");
        _pressedKeys.Clear();
    }

    private void EmitPointer(PointerEventArgs e, PointerPhase phase, Offset scrollDelta = default)
    {
        var point = e.GetCurrentPoint(_control);
        if (_activePointers.TryGetValue(e.Pointer.Id, out var active))
        {
            _activePointers[e.Pointer.Id] = active with { Position = new(point.Position.X, point.Position.Y) };
        }
        var input = new RawPointerEvent(
            _windowId,
            checked((ulong)e.Pointer.Id),
            ReadDeviceKind(e.Pointer),
            phase,
            new(point.Position.X, point.Position.Y),
            ReadButtons(point.Properties),
            Timestamp(e.Timestamp),
            PointerScrollNormalizer.NormalizeAvalonia(scrollDelta, ReadWindowsWheelScrollLines()),
            ReadModifiers(e.KeyModifiers));
        foreach (var sink in _sinks.ToArray())
        {
            sink.OnPointer(input);
        }
        _diagnostics.Record(
            "raw-pointer",
            _windowId,
            ReadMetrics(),
            $"device={input.DeviceId};kind={input.DeviceKind};phase={phase};x={input.Position.X:0.###};y={input.Position.Y:0.###};buttons={input.Buttons};wheel={scrollDelta.X:0.###},{scrollDelta.Y:0.###}");
    }

    private void EmitCancelled(ActivePointer active, string reason)
    {
        var input = new RawPointerEvent(
            _windowId,
            checked((ulong)active.Pointer.Id),
            active.DeviceKind,
            PointerPhase.Cancelled,
            active.Position,
            0,
            TimeSpan.FromMilliseconds(Environment.TickCount64));
        foreach (var sink in _sinks.ToArray())
        {
            sink.OnPointer(input);
        }
        _diagnostics.Record("raw-pointer", _windowId, ReadMetrics(), $"device={input.DeviceId};phase=Cancelled;reason={reason}");
    }

    private void EmitKey(KeyEventArgs e, KeyPhase phase)
    {
        var input = new RawKeyEvent(
            _windowId,
            checked((uint)e.PhysicalKey),
            checked((uint)e.Key),
            phase,
            TimeSpan.FromMilliseconds(Environment.TickCount64),
            ReadModifiers(e.KeyModifiers));
        foreach (var sink in _sinks.ToArray())
        {
            sink.OnKey(input);
        }
        _diagnostics.Record("raw-key", _windowId, ReadMetrics(), $"phase={phase};physical={input.PhysicalKey};logical={input.LogicalKey};modifiers={input.Modifiers}");
    }

    private void EmitFocus(bool focused, string source)
    {
        var input = new RawFocusEvent(_windowId, focused, TimeSpan.FromMilliseconds(Environment.TickCount64));
        foreach (var sink in _sinks.ToArray())
        {
            sink.OnFocus(input);
        }
        _diagnostics.Record(focused ? "focus-gained" : "focus-lost", _windowId, ReadMetrics(), source);
    }

    private void CancelActivePointers(string reason)
    {
        foreach (var (id, active) in _activePointers.ToArray())
        {
            var input = new RawPointerEvent(
                _windowId,
                checked((ulong)id),
                active.DeviceKind,
                PointerPhase.Cancelled,
                active.Position,
                0,
                TimeSpan.FromMilliseconds(Environment.TickCount64));
            foreach (var sink in _sinks.ToArray())
            {
                sink.OnPointer(input);
            }
            _diagnostics.Record("raw-pointer", _windowId, ReadMetrics(), $"device={id};phase=Cancelled;reason={reason}");
            if (ReferenceEquals(active.Pointer.Captured, _control))
            {
                active.Pointer.Capture(null);
            }
        }
        _activePointers.Clear();
    }

    private WindowMetrics ReadMetrics() => _metrics();

    private static TimeSpan Timestamp(ulong milliseconds) =>
        milliseconds <= long.MaxValue ? TimeSpan.FromMilliseconds((long)milliseconds) : TimeSpan.MaxValue;

    private static uint ReadWindowsWheelScrollLines()
    {
        const uint defaultLines = 3;
        const uint getWheelScrollLines = 0x0068;
        return OperatingSystem.IsWindows() &&
            SystemParametersInfo(getWheelScrollLines, 0, out var lines, 0) &&
            lines <= 100
                ? lines
                : defaultLines;
    }

    [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SystemParametersInfoW", SetLastError = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(uint action, uint parameter, out uint value, uint update);

    private static uint ReadButtons(PointerPointProperties properties)
    {
        uint buttons = 0;
        if (properties.IsLeftButtonPressed) buttons |= 1;
        if (properties.IsRightButtonPressed) buttons |= 2;
        if (properties.IsMiddleButtonPressed) buttons |= 4;
        if (properties.IsXButton1Pressed) buttons |= 8;
        if (properties.IsXButton2Pressed) buttons |= 16;
        if (properties.IsBarrelButtonPressed) buttons |= 32;
        return buttons;
    }

    private static PointerDeviceKind ReadDeviceKind(IPointer pointer) => pointer.Type switch
    {
        PointerType.Touch => PointerDeviceKind.Touch,
        PointerType.Pen => PointerDeviceKind.Pen,
        _ => PointerDeviceKind.Mouse,
    };

    private static InputModifiers ReadModifiers(KeyModifiers modifiers)
    {
        var result = InputModifiers.None;
        if (modifiers.HasFlag(KeyModifiers.Shift)) result |= InputModifiers.Shift;
        if (modifiers.HasFlag(KeyModifiers.Control)) result |= InputModifiers.Control;
        if (modifiers.HasFlag(KeyModifiers.Alt)) result |= InputModifiers.Alt;
        if (modifiers.HasFlag(KeyModifiers.Meta)) result |= InputModifiers.Meta;
        return result;
    }

    private sealed record ActivePointer(IPointer Pointer, PointerDeviceKind DeviceKind, Offset Position);
}

internal sealed class AvaloniaTextInputConnection : ITextInputConnection, ITextInputGeometry
{
    private readonly AvaloniaDisplayListControl _control;
    private readonly WindowId _windowId;
    private readonly Func<WindowMetrics> _metrics;
    private readonly AvaloniaHostDiagnostics _diagnostics;
    private readonly DorotiTextInputMethodClient _methodClient;
    private ITextInputClient? _client;
    private TextEditingState _state = new(string.Empty, new(0, 0), null);
    private TextEditingState? _compositionBase;
    private bool _disposed;

    internal AvaloniaTextInputConnection(
        AvaloniaDisplayListControl control,
        WindowId windowId,
        Func<WindowMetrics> metrics,
        AvaloniaHostDiagnostics diagnostics)
    {
        _control = control;
        _windowId = windowId;
        _metrics = metrics;
        _diagnostics = diagnostics;
        _methodClient = new(control, ApplySelection, ApplyPreedit);
        control.TextInputMethodClientRequested += OnTextInputMethodClientRequested;
        control.TextInput += OnTextInput;
    }

    public void SetClient(ITextInputClient client, TextEditingState initialState)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(client);
        AvaloniaWindowBackend.RequireUiThread();
        _client = client;
        _state = Normalize(initialState);
        _compositionBase = null;
        _methodClient.Update(_state);
        InputMethod.SetIsInputMethodEnabled(_control, true);
        _control.Focus();
        RequeryInputMethod();
        _diagnostics.Record("text-client-set", _windowId, _metrics(), Describe(_state));
    }

    public void UpdateState(TextEditingState state)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        AvaloniaWindowBackend.RequireUiThread();
        _state = Normalize(state);
        if (_state.ComposingRange is null)
        {
            _compositionBase = null;
        }
        _methodClient.Update(_state);
        _diagnostics.Record("text-state-updated", _windowId, _metrics(), Describe(_state));
    }

    public void SetCaretRect(Rect logicalRect)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        AvaloniaWindowBackend.RequireUiThread();
        _methodClient.SetCaretRect(logicalRect);
        _diagnostics.Record("ime-caret", _windowId, _metrics(), $"x={logicalRect.Left:0.###};y={logicalRect.Top:0.###};width={logicalRect.Width:0.###};height={logicalRect.Height:0.###};scale={_metrics().ScaleFactor:0.###}");
    }

    public void ClearClient()
    {
        if (_disposed)
        {
            return;
        }
        AvaloniaWindowBackend.RequireUiThread();
        _client = null;
        _compositionBase = null;
        _methodClient.Update(new(string.Empty, new(0, 0), null));
        InputMethod.SetIsInputMethodEnabled(_control, false);
        RequeryInputMethod();
        _diagnostics.Record("text-client-cleared", _windowId, _metrics());
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        AvaloniaWindowBackend.RequireUiThread();
        ClearClient();
        _disposed = true;
        _control.TextInputMethodClientRequested -= OnTextInputMethodClientRequested;
        _control.TextInput -= OnTextInput;
    }

    private void OnTextInputMethodClientRequested(object? sender, TextInputMethodClientRequestedEventArgs e)
    {
        if (_client is not null)
        {
            e.Client = _methodClient;
        }
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (_client is null || string.IsNullOrEmpty(e.Text))
        {
            return;
        }
        _state = TextEditingStateReducer.CommitText(_state, e.Text);
        _compositionBase = null;
        _methodClient.Update(_state);
        _client.UpdateEditingState(_state);
        _diagnostics.Record("text-committed", _windowId, _metrics(), $"text={e.Text};{Describe(_state)}");
        e.Handled = true;
    }

    private void ApplyPreedit(string? text, int? cursorPosition)
    {
        if (_client is null)
        {
            return;
        }
        if (string.IsNullOrEmpty(text))
        {
            if (_compositionBase is { } original && _state.ComposingRange is not null)
            {
                _state = TextEditingStateReducer.CancelComposition(original);
                _client.UpdateEditingState(_state);
                _methodClient.Update(_state);
                _diagnostics.Record("ime-cancelled", _windowId, _metrics(), Describe(_state));
            }
            _compositionBase = null;
            return;
        }
        if (_state.ComposingRange is null)
        {
            _compositionBase = _state;
            _state = TextEditingStateReducer.BeginComposition(_state);
        }
        _state = TextEditingStateReducer.UpdateComposition(_state, text);
        if (cursorPosition is { } cursor && _state.ComposingRange is { } range)
        {
            var start = Math.Min(range.BaseOffset, range.ExtentOffset);
            var caret = Math.Clamp(start + cursor, start, Math.Max(range.BaseOffset, range.ExtentOffset));
            _state = _state with { Selection = new(caret, caret) };
        }
        _methodClient.Update(_state);
        _client.UpdateEditingState(_state);
        _diagnostics.Record("ime-preedit", _windowId, _metrics(), $"text={text};{Describe(_state)}");
    }

    private void ApplySelection(AvaloniaTextSelection selection)
    {
        if (_client is null)
        {
            return;
        }
        _state = Normalize(_state with { Selection = new(selection.Start, selection.End) });
        _methodClient.Update(_state);
        _client.UpdateEditingState(_state);
        _diagnostics.Record("text-selection", _windowId, _metrics(), Describe(_state));
    }

    private void RequeryInputMethod() => _control.RaiseEvent(new TextInputMethodClientRequeryRequestedEventArgs
    {
        RoutedEvent = InputMethod.TextInputMethodClientRequeryRequestedEvent,
    });

    private static TextEditingState Normalize(TextEditingState state)
    {
        var text = state.Text ?? string.Empty;
        var selection = new DorotiTextSelection(
            Math.Clamp(state.Selection.BaseOffset, 0, text.Length),
            Math.Clamp(state.Selection.ExtentOffset, 0, text.Length));
        DorotiTextSelection? composing = state.ComposingRange is { } range
            ? new DorotiTextSelection(Math.Clamp(range.BaseOffset, 0, text.Length), Math.Clamp(range.ExtentOffset, 0, text.Length))
            : null;
        return new(text, selection, composing);
    }

    private static string Describe(TextEditingState state) =>
        $"length={state.Text.Length};selection={state.Selection.BaseOffset},{state.Selection.ExtentOffset};composing={(state.ComposingRange is { } range ? $"{range.BaseOffset},{range.ExtentOffset}" : "none")}";
}

internal sealed class DorotiTextInputMethodClient(
    AvaloniaDisplayListControl control,
    Action<AvaloniaTextSelection> selectionChanged,
    Action<string?, int?> preeditChanged) : TextInputMethodClient
{
    private TextEditingState _state = new(string.Empty, new(0, 0), null);
    private global::Avalonia.Rect _caretRect;
    private bool _updating;

    public override global::Avalonia.Visual TextViewVisual => control;

    public override bool SupportsPreedit => true;

    public override bool SupportsSurroundingText => true;

    public override string SurroundingText => _state.Text;

    public override global::Avalonia.Rect CursorRectangle => _caretRect;

    public override AvaloniaTextSelection Selection
    {
        get => new(_state.Selection.BaseOffset, _state.Selection.ExtentOffset);
        set
        {
            if (!_updating)
            {
                selectionChanged(value);
            }
        }
    }

    public override void SetPreeditText(string? preeditText, int? cursorPos) => preeditChanged(preeditText, cursorPos);

    internal void Update(TextEditingState state)
    {
        var oldText = _state.Text;
        var oldSelection = _state.Selection;
        _updating = true;
        _state = state;
        _updating = false;
        if (!string.Equals(oldText, state.Text, StringComparison.Ordinal))
        {
            RaiseSurroundingTextChanged();
        }
        if (oldSelection != state.Selection)
        {
            RaiseSelectionChanged();
        }
    }

    internal void SetCaretRect(Rect rect)
    {
        _caretRect = new(rect.Left, rect.Top, rect.Width, rect.Height);
        RaiseCursorRectangleChanged();
    }
}

internal sealed class AvaloniaClipboard(Window window) : Doroti.Platform.IClipboard
{
    public async ValueTask<ClipboardResult> GetTextAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (Dispatcher.UIThread.CheckAccess())
            {
                return await GetTextCoreAsync(cancellationToken);
            }
            return await Dispatcher.UIThread.InvokeAsync(() => GetTextCoreAsync(cancellationToken));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return ClipboardResult.Failure(exception.Message);
        }
    }

    public async ValueTask<ClipboardResult> SetTextAsync(string text, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(text);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (Dispatcher.UIThread.CheckAccess())
            {
                return await SetTextCoreAsync(text, cancellationToken);
            }
            return await Dispatcher.UIThread.InvokeAsync(() => SetTextCoreAsync(text, cancellationToken));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            return ClipboardResult.Failure(exception.Message);
        }
    }

    private async Task<ClipboardResult> GetTextCoreAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var clipboard = TopLevel.GetTopLevel(window)?.Clipboard;
        return clipboard is null
            ? ClipboardResult.Failure("Avalonia clipboard is unavailable for this window.")
            : ClipboardResult.FromText(await clipboard.TryGetTextAsync());
    }

    private async Task<ClipboardResult> SetTextCoreAsync(string text, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var clipboard = TopLevel.GetTopLevel(window)?.Clipboard;
        if (clipboard is null)
        {
            return ClipboardResult.Failure("Avalonia clipboard is unavailable for this window.");
        }
        await clipboard.SetTextAsync(text);
        return new(true);
    }
}

internal sealed class AvaloniaCursorController(AvaloniaDisplayListControl control, WindowId windowId) : ICursorController, IDisposable
{
    private AvaloniaCursor? _cursor;

    public void SetCursor(WindowId window, CursorKind cursor)
    {
        AvaloniaWindowBackend.RequireUiThread();
        if (window != windowId)
        {
            throw new ArgumentOutOfRangeException(nameof(window), $"Cursor controller belongs to window {windowId.Value}.");
        }
        _cursor?.Dispose();
        _cursor = cursor is CursorKind.Hidden
            ? new(StandardCursorType.None)
            : new(cursor switch
            {
                CursorKind.Click => StandardCursorType.Hand,
                CursorKind.Text => StandardCursorType.Ibeam,
                CursorKind.Precise => StandardCursorType.Cross,
                CursorKind.Forbidden => StandardCursorType.No,
                _ => StandardCursorType.Arrow,
            });
        control.Cursor = _cursor;
    }

    public void Dispose()
    {
        _cursor?.Dispose();
        _cursor = null;
        control.Cursor = null;
    }
}
