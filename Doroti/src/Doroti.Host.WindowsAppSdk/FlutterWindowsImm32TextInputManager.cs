using System.Runtime.InteropServices;
using Doroti.Ui;

namespace Doroti.Host.WindowsAppSdk;

/// <summary>
/// The sole child-HWND text-input owner for F7.  It uses the IMM32 message and
/// context APIs directly; no second text service or hidden native edit control
/// competes for composition, candidate, or caret ownership.
/// </summary>
internal sealed class FlutterWindowsImm32TextInputManager : IDisposable
{
    private const uint WmImeSetContext = 0x0281;
    private const uint WmImeNotify = 0x0282;
    private const uint WmImeStartComposition = 0x010d;
    private const uint WmImeEndComposition = 0x010e;
    private const uint WmImeComposition = 0x010f;
    private const int GcsCompStr = 0x0008;
    private const int GcsCursorPos = 0x0080;
    private const int GcsResultStr = 0x0800;
    private const uint ImnOpenCandidate = 0x0005;
    private const uint ImnChangeCandidate = 0x0003;
    private const uint NiCompositionStr = 0x0015;
    private const uint CpsCancel = 0x0004;
    private const uint CfsPoint = 0x0002;
    private const uint CfsCandidatePos = 0x0040;

    private readonly nint _childHwnd;
    private readonly Func<WindowsViewMetrics> _metricsProvider;
    private readonly Action<DorotiTextEditingState> _editingStateChanged;
    private readonly Action<DorotiTextInputAction> _actionPerformed;
    private readonly object _gate = new();
    private DorotiTextInputConfiguration? _configuration;
    private DorotiTextEditingState _editingState = new(
        string.Empty,
        new DorotiTextSelection(0, 0),
        null);
    // A preedit string is projected into DorotiTextEditingState while IMM32 is
    // composing. Retaining the pre-composition state lets an IME cancel return
    // the text model to its actual preedit boundary instead of accidentally
    // committing a discarded Korean candidate.
    private DorotiTextEditingState? _compositionBaseState;
    private Rect _caretLogicalRect = Rect.zero;
    private FlutterWindowsChildClientRect? _committedCaretClientRect;
    private FlutterWindowsScreenRect? _committedCaretScreenRect;
    private bool _isComposing;
    private bool _systemCaretCreated;
    private bool _disposed;
    private long _clientSetCount;
    private long _stateUpdateCount;
    private long _clearClientCount;
    private long _compositionStartCount;
    private long _compositionUpdateCount;
    private long _resultCommitCount;
    private long _compositionEndCount;
    private long _candidatePositionUpdateCount;
    private long _caretPositionUpdateCount;
    private long _keyboardTextCommitCount;
    private long _actionPerformedCount;
    private long _immContextAcquireCount;
    private long _immContextReleaseCount;

    internal FlutterWindowsImm32TextInputManager(
        nint childHwnd,
        Func<WindowsViewMetrics> metricsProvider,
        Action<DorotiTextEditingState> editingStateChanged,
        Action<DorotiTextInputAction> actionPerformed)
    {
        if (childHwnd == 0) throw new ArgumentOutOfRangeException(nameof(childHwnd));
        _childHwnd = childHwnd;
        _metricsProvider = metricsProvider ?? throw new ArgumentNullException(nameof(metricsProvider));
        _editingStateChanged = editingStateChanged ?? throw new ArgumentNullException(nameof(editingStateChanged));
        _actionPerformed = actionPerformed ?? throw new ArgumentNullException(nameof(actionPerformed));
    }

    /// <summary>Exactly one IMM32 client may be active for this child HWND.</summary>
    internal void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState initialState)
    {
        lock (_gate)
        {
            ThrowIfDisposed();
            _configuration = configuration;
            _editingState = NormalizeState(initialState);
            _isComposing = false;
            _compositionBaseState = null;
            Interlocked.Increment(ref _clientSetCount);
            UpdateImePlacementLocked();
        }
    }

    internal void UpdateState(DorotiTextEditingState state)
    {
        lock (_gate)
        {
            ThrowIfDisposed();
            if (_configuration is null) return;
            _editingState = NormalizeState(state);
            _compositionBaseState = null;
            Interlocked.Increment(ref _stateUpdateCount);
            UpdateImePlacementLocked();
        }
    }

    /// <summary>
    /// Stores Flutter logical caret geometry, converts it with the immutable F3
    /// metrics source, and records the resulting committed child screen rect.
    /// IMM32 itself correctly receives the equivalent child-client coordinates.
    /// </summary>
    internal void SetCaretRect(Rect logicalRect)
    {
        if (!logicalRect.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(logicalRect));
        lock (_gate)
        {
            ThrowIfDisposed();
            _caretLogicalRect = logicalRect;
            UpdateImePlacementLocked();
        }
    }

    internal void ClearClient()
    {
        lock (_gate)
        {
            if (_disposed || _configuration is null) return;
            CancelCompositionLocked(publish: false);
            _configuration = null;
            _isComposing = false;
            DestroySystemCaretLocked();
            Interlocked.Increment(ref _clearClientCount);
        }
    }

    internal void OnFocusChanged(bool focused)
    {
        lock (_gate)
        {
            if (_disposed || _configuration is null) return;
            if (focused)
            {
                UpdateImePlacementLocked();
                return;
            }

            CancelCompositionLocked(publish: true);
            DestroySystemCaretLocked();
        }
    }

    internal FlutterWindowsChildMessageResult HandleMessage(FlutterWindowsChildMessage message)
    {
        lock (_gate)
        {
            if (_disposed || _configuration is null || _configuration.Value.readOnly)
                return FlutterWindowsChildMessageResult.Unhandled;

            switch (message.Message)
            {
                case WmImeSetContext:
                    UpdateImePlacementLocked();
                    return FlutterWindowsChildMessageResult.Unhandled;
                case WmImeStartComposition:
                    if (!_isComposing) _compositionBaseState = _editingState;
                    _isComposing = true;
                    Interlocked.Increment(ref _compositionStartCount);
                    UpdateImePlacementLocked();
                    return FlutterWindowsChildMessageResult.HandledResult();
                case WmImeComposition:
                    HandleCompositionLocked(message.LParam.ToInt64());
                    return FlutterWindowsChildMessageResult.HandledResult();
                case WmImeEndComposition:
                    CancelCompositionLocked(publish: true);
                    Interlocked.Increment(ref _compositionEndCount);
                    return FlutterWindowsChildMessageResult.HandledResult();
                case WmImeNotify when message.WParam is ImnOpenCandidate or ImnChangeCandidate:
                    UpdateImePlacementLocked();
                    return FlutterWindowsChildMessageResult.HandledResult();
                default:
                    return FlutterWindowsChildMessageResult.Unhandled;
            }
        }
    }

    /// <summary>
    /// Called only by <see cref="FlutterWindowsKeyboardManager"/> after it has
    /// serialized surrogate pairs.  During an active IMM32 composition, the
    /// IME result message remains the only commit authority.
    /// </summary>
    internal bool TryCommitKeyboardText(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        lock (_gate)
        {
            if (_disposed || _configuration is not { } configuration || configuration.readOnly || _isComposing)
                return false;
            var next = ReplaceSelectionOrComposingLocked(text, composingRange: null, text.Length);
            PublishEditingStateLocked(next);
            Interlocked.Increment(ref _keyboardTextCommitCount);
            return true;
        }
    }

    internal bool TryPerformTextAction()
    {
        lock (_gate)
        {
            if (_disposed || _configuration is not { } configuration || configuration.readOnly)
                return false;
            var action = configuration.inputAction;
            if (action is DorotiTextInputAction.none or DorotiTextInputAction.newline)
                return false;
            _actionPerformed(action);
            Interlocked.Increment(ref _actionPerformedCount);
            return true;
        }
    }

    internal FlutterWindowsImm32TextInputManagerSnapshot Snapshot
    {
        get
        {
            lock (_gate)
            {
                return new(
                    _childHwnd,
                    true,
                    _configuration is not null,
                    _isComposing,
                    _systemCaretCreated,
                    _editingState,
                    _caretLogicalRect,
                    _committedCaretClientRect,
                    _committedCaretScreenRect,
                    Interlocked.Read(ref _clientSetCount),
                    Interlocked.Read(ref _stateUpdateCount),
                    Interlocked.Read(ref _clearClientCount),
                    Interlocked.Read(ref _compositionStartCount),
                    Interlocked.Read(ref _compositionUpdateCount),
                    Interlocked.Read(ref _resultCommitCount),
                    Interlocked.Read(ref _compositionEndCount),
                    Interlocked.Read(ref _candidatePositionUpdateCount),
                    Interlocked.Read(ref _caretPositionUpdateCount),
                    Interlocked.Read(ref _keyboardTextCommitCount),
                    Interlocked.Read(ref _actionPerformedCount),
                    Interlocked.Read(ref _immContextAcquireCount),
                    Interlocked.Read(ref _immContextReleaseCount),
                    _disposed);
            }
        }
    }

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed) return;
            CancelCompositionLocked(publish: false);
            DestroySystemCaretLocked();
            _configuration = null;
            _disposed = true;
        }
    }

    private void HandleCompositionLocked(long flags)
    {
        if ((flags & GcsResultStr) != 0)
        {
            var result = GetCompositionStringLocked(GcsResultStr);
            if (!string.IsNullOrEmpty(result))
            {
                _isComposing = false;
                var committedState = ReplaceSelectionOrComposingLocked(result, composingRange: null, result.Length);
                PublishEditingStateLocked(committedState);
                _compositionBaseState = null;
                Interlocked.Increment(ref _resultCommitCount);
            }
        }

        if ((flags & GcsCompStr) == 0) return;
        var composing = GetCompositionStringLocked(GcsCompStr);
        if (composing is null) return;
        var cursor = GetCompositionCursorPositionLocked();
        if (!_isComposing) _compositionBaseState = _editingState;
        _isComposing = true;
        var nextComposingRange = new DorotiTextSelection(0, composing.Length);
        var next = ReplaceSelectionOrComposingLocked(
            composing,
            nextComposingRange,
            Math.Clamp(cursor, 0, composing.Length));
        PublishEditingStateLocked(next);
        Interlocked.Increment(ref _compositionUpdateCount);
        UpdateImePlacementLocked();
    }

    private DorotiTextEditingState ReplaceSelectionOrComposingLocked(
        string replacement,
        DorotiTextSelection? composingRange,
        int relativeSelectionOffset)
    {
        var source = _editingState.text ?? string.Empty;
        var replacementRange = _editingState.composingRange ?? _editingState.selection;
        var start = Math.Clamp(Math.Min(replacementRange.baseOffset, replacementRange.extentOffset), 0, source.Length);
        var end = Math.Clamp(Math.Max(replacementRange.baseOffset, replacementRange.extentOffset), start, source.Length);
        var text = string.Concat(source[..start], replacement, source[end..]);
        var selection = checked(start + Math.Clamp(relativeSelectionOffset, 0, replacement.Length));
        DorotiTextSelection? absoluteComposingRange = composingRange is null
            ? null
            : new DorotiTextSelection(start, checked(start + replacement.Length));
        return new(text, new(selection, selection), absoluteComposingRange);
    }

    private void PublishEditingStateLocked(DorotiTextEditingState state)
    {
        _editingState = NormalizeState(state);
        _editingStateChanged(_editingState);
    }

    private void UpdateImePlacementLocked()
    {
        if (_configuration is null || _childHwnd == 0) return;
        WindowsViewMetrics metrics;
        try
        {
            metrics = _metricsProvider();
        }
        catch (Exception)
        {
            // Geometry is an input aid, never a reason to fail an IMM32 text transaction.
            return;
        }
        if (!metrics.HasDrawableSize || metrics.DevicePixelRatio <= 0) return;

        var clientRect = LogicalToChildClientRect(_caretLogicalRect, metrics.DevicePixelRatio);
        _committedCaretClientRect = clientRect;
        var screenTopLeft = new FlutterWindowsInputNative.NativePoint(clientRect.Left, clientRect.Top);
        var screenBottomRight = new FlutterWindowsInputNative.NativePoint(clientRect.Right, clientRect.Bottom);
        if (FlutterWindowsInputNative.ClientToScreen(_childHwnd, ref screenTopLeft) &&
            FlutterWindowsInputNative.ClientToScreen(_childHwnd, ref screenBottomRight))
        {
            _committedCaretScreenRect = new(
                screenTopLeft.X,
                screenTopLeft.Y,
                screenBottomRight.X,
                screenBottomRight.Y);
        }

        EnsureSystemCaretLocked(clientRect);
        var context = AcquireImmContextLocked();
        if (context == 0) return;
        try
        {
            var composition = new FlutterWindowsInputNative.CompositionForm(
                CfsPoint,
                new(clientRect.Left, clientRect.Top),
                new(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom));
            _ = FlutterWindowsInputNative.ImmSetCompositionWindow(context, in composition);
            var candidate = new FlutterWindowsInputNative.CandidateForm(
                0,
                CfsCandidatePos,
                new(clientRect.Left, clientRect.Bottom),
                new(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom));
            _ = FlutterWindowsInputNative.ImmSetCandidateWindow(context, in candidate);
            Interlocked.Increment(ref _candidatePositionUpdateCount);
        }
        finally
        {
            ReleaseImmContextLocked(context);
        }
    }

    private void EnsureSystemCaretLocked(FlutterWindowsChildClientRect clientRect)
    {
        var width = Math.Max(1, clientRect.Right - clientRect.Left);
        var height = Math.Max(1, clientRect.Bottom - clientRect.Top);
        if (!_systemCaretCreated)
        {
            _systemCaretCreated = FlutterWindowsInputNative.CreateCaret(_childHwnd, 0, width, height);
            if (_systemCaretCreated) _ = FlutterWindowsInputNative.ShowCaret(_childHwnd);
        }
        if (_systemCaretCreated)
        {
            _ = FlutterWindowsInputNative.SetCaretPos(clientRect.Left, clientRect.Top);
            Interlocked.Increment(ref _caretPositionUpdateCount);
        }
    }

    private void DestroySystemCaretLocked()
    {
        if (!_systemCaretCreated) return;
        _ = FlutterWindowsInputNative.HideCaret(_childHwnd);
        _ = FlutterWindowsInputNative.DestroyCaret();
        _systemCaretCreated = false;
    }

    private void CancelCompositionLocked(bool publish)
    {
        var context = AcquireImmContextLocked();
        if (context != 0)
        {
            try
            {
                _ = FlutterWindowsInputNative.ImmNotifyIME(context, NiCompositionStr, 0, CpsCancel);
            }
            finally
            {
                ReleaseImmContextLocked(context);
            }
        }

        var reset = _compositionBaseState is { } baseState
            ? baseState with { composingRange = null }
            : _editingState with { composingRange = null };
        var changed = !Equals(reset, _editingState);
        _compositionBaseState = null;
        _isComposing = false;
        if (publish && changed) PublishEditingStateLocked(reset);
        else _editingState = NormalizeState(reset);
    }

    private string? GetCompositionStringLocked(int index)
    {
        var context = AcquireImmContextLocked();
        if (context == 0) return null;
        try
        {
            var byteCount = FlutterWindowsInputNative.ImmGetCompositionStringW(context, index, 0, 0);
            if (byteCount < 0) return null;
            if (byteCount == 0) return string.Empty;
            var buffer = Marshal.AllocHGlobal(byteCount);
            try
            {
                return FlutterWindowsInputNative.ImmGetCompositionStringW(
                    context,
                    index,
                    buffer,
                    checked((uint)byteCount)) < 0
                    ? null
                    : Marshal.PtrToStringUni(buffer, byteCount / sizeof(char));
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }
        finally
        {
            ReleaseImmContextLocked(context);
        }
    }

    private int GetCompositionCursorPositionLocked()
    {
        var context = AcquireImmContextLocked();
        if (context == 0) return 0;
        try
        {
            var cursor = FlutterWindowsInputNative.ImmGetCompositionStringW(context, GcsCursorPos, 0, 0);
            return Math.Max(0, cursor);
        }
        finally
        {
            ReleaseImmContextLocked(context);
        }
    }

    private nint AcquireImmContextLocked()
    {
        var context = FlutterWindowsInputNative.ImmGetContext(_childHwnd);
        if (context != 0) Interlocked.Increment(ref _immContextAcquireCount);
        return context;
    }

    private void ReleaseImmContextLocked(nint context)
    {
        if (context == 0) return;
        if (FlutterWindowsInputNative.ImmReleaseContext(_childHwnd, context))
            Interlocked.Increment(ref _immContextReleaseCount);
    }

    private static FlutterWindowsChildClientRect LogicalToChildClientRect(Rect logicalRect, double dpr)
    {
        static int ToPhysical(double value, double scale) => checked((int)Math.Round(
            value * scale,
            MidpointRounding.AwayFromZero));
        var left = ToPhysical(logicalRect.left, dpr);
        var top = ToPhysical(logicalRect.top, dpr);
        var right = ToPhysical(logicalRect.right, dpr);
        var bottom = ToPhysical(logicalRect.bottom, dpr);
        if (right <= left) right = checked(left + 1);
        if (bottom <= top) bottom = checked(top + 1);
        return new(left, top, right, bottom);
    }

    private static DorotiTextEditingState NormalizeState(DorotiTextEditingState state)
    {
        var text = state.text ?? string.Empty;
        var selection = ClampSelection(state.selection, text.Length);
        DorotiTextSelection? composing = state.composingRange is { } range
            ? ClampSelection(range, text.Length)
            : null;
        return new(text, selection, composing);
    }

    private static DorotiTextSelection ClampSelection(DorotiTextSelection selection, int textLength) => new(
        Math.Clamp(selection.baseOffset, 0, textLength),
        Math.Clamp(selection.extentOffset, 0, textLength));

    private void ThrowIfDisposed() => ObjectDisposedException.ThrowIf(_disposed, this);
}

/// <summary>Physical caret bounds in child-client pixels, used for IMM32 calls.</summary>
internal readonly record struct FlutterWindowsChildClientRect(int Left, int Top, int Right, int Bottom)
{
    internal int Width => Right - Left;
    internal int Height => Bottom - Top;
}

/// <summary>Committed caret bounds in screen pixels for input/DPI evidence.</summary>
internal readonly record struct FlutterWindowsScreenRect(int Left, int Top, int Right, int Bottom)
{
    internal int Width => Right - Left;
    internal int Height => Bottom - Top;
}

/// <summary>IMM32 text, composition, candidate, and caret ownership evidence.</summary>
internal sealed record FlutterWindowsImm32TextInputManagerSnapshot(
    nint ChildHwnd,
    bool IsImm32Owner,
    bool HasClient,
    bool IsComposing,
    bool HasSystemCaret,
    DorotiTextEditingState EditingState,
    Rect CaretLogicalRect,
    FlutterWindowsChildClientRect? CommittedCaretClientRect,
    FlutterWindowsScreenRect? CommittedCaretScreenRect,
    long ClientSetCount,
    long StateUpdateCount,
    long ClearClientCount,
    long CompositionStartCount,
    long CompositionUpdateCount,
    long ResultCommitCount,
    long CompositionEndCount,
    long CandidatePositionUpdateCount,
    long CaretPositionUpdateCount,
    long KeyboardTextCommitCount,
    long ActionPerformedCount,
    long ImmContextAcquireCount,
    long ImmContextReleaseCount,
    bool IsDisposed);
