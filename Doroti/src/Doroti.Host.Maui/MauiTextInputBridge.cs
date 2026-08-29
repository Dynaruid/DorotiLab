using Doroti.Ui;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

public sealed class MauiTextInputBridge : IDisposable
{
    private readonly Entry _entry;
    private readonly Editor _editor;
    private readonly Layout? _visualHost;
    private readonly bool _attachOnDemand;
    private InputView _active;
    private DorotiTextInputConfiguration _configuration;
    private bool _hasClient;
    private bool _suspended;
    private bool _updating;
    private bool _publishingNativeTextChange;
    private bool _disposed;
    private bool _editingStateDispatchPending;
    private string? _pendingNativeText;
    private InputView? _pendingNativeInput;
    private readonly object _caretGate = new();
    private Doroti.Ui.Rect _pendingCaretRect;
    private bool _caretDispatchPending;
    private InputView? _lastCaretInput;
    private Doroti.Ui.Rect _lastCaretRect;
    private bool _hasLastCaretRect;

    internal MauiTextInputBridge(
        Entry entry,
        Editor editor,
        Layout? visualHost = null,
        bool attachOnDemand = false)
    {
        _entry = entry;
        _editor = editor;
        _visualHost = visualHost;
        _attachOnDemand = attachOnDemand;
        if (_attachOnDemand && _visualHost is null)
            throw new ArgumentNullException(nameof(visualHost), "On-demand MAUI text input requires a visual host.");
        _active = entry;
        _entry.TextChanged += HandleTextChanged;
        _editor.TextChanged += HandleTextChanged;
        _entry.PropertyChanged += HandleInputPropertyChanged;
        _editor.PropertyChanged += HandleInputPropertyChanged;
        _entry.Completed += HandleCompleted;
        _editor.Completed += HandleCompleted;
        _entry.Focused += HandleFocused;
        _editor.Focused += HandleFocused;
        _entry.Unfocused += HandleUnfocused;
        _editor.Unfocused += HandleUnfocused;
    }

    internal event Action<DorotiTextEditingState>? EditingStateChanged;
    internal event Action<DorotiTextInputAction>? ActionPerformed;
    internal event Action<bool>? FocusChanged;
    internal IReadOnlyList<InputView> Inputs => [_entry, _editor];
    internal bool HasClient => _hasClient;

    internal void SetClient(DorotiTextInputConfiguration configuration, DorotiTextEditingState state)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _configuration = configuration;
        _hasClient = true;
        var next = configuration.inputType == DorotiTextInputType.multiline ? (InputView)_editor : _entry;
        if (!ReferenceEquals(_active, next))
        {
            _active.Unfocus();
            _active = next;
        }
        Configure(_active, configuration);
        UpdateState(state);
        AttachActiveInput(requestFocus: true);
    }

    internal void UpdateState(DorotiTextEditingState state)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var length = state.text.Length;
        var start = Math.Clamp(Math.Min(state.selection.baseOffset, state.selection.extentOffset), 0, length);
        var end = Math.Clamp(Math.Max(state.selection.baseOffset, state.selection.extentOffset), start, length);

        // Do not echo a native edit back into the same control. Reassigning an
        // accepted text/selection can replace or end the platform IME's active
        // composing/marked range (for example Gboard's Editable span).
        if (string.Equals(_active.Text ?? string.Empty, state.text, StringComparison.Ordinal) &&
            (_publishingNativeTextChange ||
             (_active.CursorPosition == start && _active.SelectionLength == end - start)))
        {
            return;
        }

        // A framework update supersedes any native TextChanged callback that
        // was queued before the framework accepted the previous editing state.
        _pendingNativeText = null;
        _pendingNativeInput = null;
        _updating = true;
        try
        {
            if (!string.Equals(_active.Text ?? string.Empty, state.text, StringComparison.Ordinal))
                _active.Text = state.text;
            if (_active.CursorPosition != start) _active.CursorPosition = start;
            if (_active.SelectionLength != end - start) _active.SelectionLength = end - start;
        }
        finally
        {
            _updating = false;
        }
    }

    internal void SetCaretRect(Doroti.Ui.Rect rect)
    {
        lock (_caretGate)
        {
            _pendingCaretRect = rect;
            if (_caretDispatchPending) return;
            _caretDispatchPending = true;
        }

        // The caret rectangle is published while Doroti is painting, which can
        // run on the GPU/render thread. MAUI visual properties must only mutate
        // the native view hierarchy from the platform UI thread. Coalesce paint
        // updates so scrolling cannot queue an unbounded number of UI mutations.
        var dispatcher = _active.Dispatcher;
        if (dispatcher.IsDispatchRequired)
        {
            if (!dispatcher.Dispatch(ApplyPendingCaretRect))
            {
                lock (_caretGate) _caretDispatchPending = false;
            }
        }
        else ApplyPendingCaretRect();
    }

    private void ApplyPendingCaretRect()
    {
        Doroti.Ui.Rect rect;
        lock (_caretGate)
        {
            rect = _pendingCaretRect;
            _caretDispatchPending = false;
        }

        if (_disposed) return;
        var active = _active;
        if (_hasLastCaretRect && ReferenceEquals(_lastCaretInput, active) && _lastCaretRect == rect) return;

        var x = Math.Max(0, rect.left);
        var y = Math.Max(0, rect.top);
        var width = Math.Max(1, rect.width);
        var height = Math.Max(1, rect.height);
        if (active.TranslationX != x) active.TranslationX = x;
        if (active.TranslationY != y) active.TranslationY = y;
        if (active.WidthRequest != width) active.WidthRequest = width;
        if (active.HeightRequest != height) active.HeightRequest = height;

        _lastCaretInput = active;
        _lastCaretRect = rect;
        _hasLastCaretRect = true;
    }

    internal void ClearClient()
    {
        if (_disposed) return;
        _hasClient = false;
        _pendingNativeText = null;
        _pendingNativeInput = null;
        DeactivateActiveInput(clearFocus: true);
        _active.Text = string.Empty;
        _active.WidthRequest = 1;
        _active.HeightRequest = 1;
        DetachInputs();
    }

    internal void Suspend()
    {
        if (_disposed || !_attachOnDemand) return;
        _suspended = true;
        DeactivateActiveInput(clearFocus: true);
        DetachInputs();
    }

    internal void Resume()
    {
        if (_disposed || !_attachOnDemand) return;
        _suspended = false;
        if (_hasClient) AttachActiveInput(requestFocus: true);
    }

    internal void ShowTextInput()
    {
        if (_disposed || _suspended || !_hasClient) return;
        AttachActiveInput(requestFocus: false);
        DispatchActiveInputMutation(ActivateNativeTextInput);
    }

    internal void HideTextInput()
    {
        if (_disposed) return;
        DeactivateActiveInput(clearFocus: false);
    }

    private void AttachActiveInput(bool requestFocus)
    {
        if (!_attachOnDemand)
        {
            if (requestFocus) _active.Dispatcher.Dispatch(() => _active.Focus());
            return;
        }
        if (_visualHost is null || _suspended || !_hasClient) return;
        var expected = _active;
        DispatchVisualMutation(() =>
        {
            if (_disposed || _suspended || !_hasClient || !ReferenceEquals(expected, _active)) return;
            foreach (var input in Inputs)
            {
                if (!ReferenceEquals(input, expected) && ReferenceEquals(input.Parent, _visualHost))
                    _visualHost.Children.Remove(input);
            }
            if (expected.Parent is null) _visualHost.Children.Add(expected);
            if (requestFocus) expected.Focus();
        });
    }

    private void DetachInputs()
    {
        if (!_attachOnDemand || _visualHost is null) return;
        DispatchVisualMutation(() =>
        {
            foreach (var input in Inputs)
            {
                if (ReferenceEquals(input.Parent, _visualHost)) _visualHost.Children.Remove(input);
            }
        });
    }

    private void DispatchVisualMutation(Action mutation)
    {
        if (_visualHost is null) return;
        if (_visualHost.Dispatcher.IsDispatchRequired) _visualHost.Dispatcher.Dispatch(mutation);
        else mutation();
    }

    private void DispatchActiveInputMutation(Action<InputView> mutation)
    {
        var expected = _active;
        void Apply()
        {
            if (_disposed || !ReferenceEquals(expected, _active)) return;
            mutation(expected);
        }

        if (expected.Dispatcher.IsDispatchRequired) expected.Dispatcher.Dispatch(Apply);
        else Apply();
    }

    private void DeactivateActiveInput(bool clearFocus)
    {
        DispatchActiveInputMutation(input =>
        {
            HideNativeTextInput(input);
            if (clearFocus) input.Unfocus();
        });
    }

    private static void ActivateNativeTextInput(InputView input)
    {
        input.Focus();
#if ANDROID
        if (input.Handler?.PlatformView is Android.Views.View nativeView)
        {
            nativeView.RequestFocus();
            nativeView.Post(() =>
            {
                var inputMethodManager = nativeView.Context?.GetSystemService(
                    Android.Content.Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
                inputMethodManager?.ShowSoftInput(
                    nativeView, Android.Views.InputMethods.ShowFlags.Implicit);
            });
        }
#endif
    }

    private static void HideNativeTextInput(InputView input)
    {
#if ANDROID
        if (input.Handler?.PlatformView is Android.Views.View nativeView)
        {
            var inputMethodManager = nativeView.Context?.GetSystemService(
                Android.Content.Context.InputMethodService) as Android.Views.InputMethods.InputMethodManager;
            inputMethodManager?.HideSoftInputFromWindow(
                nativeView.WindowToken, Android.Views.InputMethods.HideSoftInputFlags.None);
        }
#endif
    }

    private static void Configure(InputView input, DorotiTextInputConfiguration configuration)
    {
        input.IsReadOnly = configuration.readOnly;
        input.IsTextPredictionEnabled = configuration.enableSuggestions;
        input.IsSpellCheckEnabled = configuration.autocorrect;
        var keyboardFlags = configuration.textCapitalization switch
        {
            DorotiTextCapitalization.words => KeyboardFlags.CapitalizeWord,
            DorotiTextCapitalization.sentences => KeyboardFlags.CapitalizeSentence,
            DorotiTextCapitalization.characters => KeyboardFlags.CapitalizeCharacter,
            _ => KeyboardFlags.None,
        };
        if (configuration.autocorrect) keyboardFlags |= KeyboardFlags.Spellcheck;
        if (configuration.enableSuggestions) keyboardFlags |= KeyboardFlags.Suggestions;
        input.Keyboard = configuration.inputType switch
        {
            DorotiTextInputType.number => Keyboard.Numeric,
            DorotiTextInputType.phone => Keyboard.Telephone,
            DorotiTextInputType.emailAddress => Keyboard.Email,
            DorotiTextInputType.url => Keyboard.Url,
            DorotiTextInputType.webSearch or DorotiTextInputType.twitter => Keyboard.Chat,
            _ => Keyboard.Create(keyboardFlags),
        };
        if (input is Entry entry)
        {
            entry.IsPassword = configuration.obscureText;
            entry.ReturnType = configuration.inputAction switch
            {
                DorotiTextInputAction.done => ReturnType.Done,
                DorotiTextInputAction.go => ReturnType.Go,
                DorotiTextInputAction.search => ReturnType.Search,
                DorotiTextInputAction.send => ReturnType.Send,
                DorotiTextInputAction.next => ReturnType.Next,
                _ => ReturnType.Default,
            };
        }
    }

    private void HandleTextChanged(object? sender, TextChangedEventArgs args)
    {
        if (_updating || !ReferenceEquals(sender, _active)) return;
        var text = args.NewTextValue ?? string.Empty;
        var selection = ResolveSelectionAfterTextChange(args.OldTextValue ?? string.Empty, text);
        _publishingNativeTextChange = true;
        try
        {
            // Native key-repeat can keep the platform input callback busy long
            // enough to starve every queued UI callback (Android Gboard does
            // this for a long-pressed Backspace). Text edits collapse selection
            // at the end of their changed range, which can be derived before
            // MAUI publishes its later CursorPosition notification. Publish the
            // complete state from this callback so every repeated edit reaches
            // the framework without waiting for another UI turn.
            PublishNativeEditingState(_active, text, selection, selection);
        }
        finally
        {
            _publishingNativeTextChange = false;
        }
    }

    private void HandleInputPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
    {
        if (_updating || !ReferenceEquals(sender, _active) ||
            args.PropertyName is not (nameof(InputView.CursorPosition) or nameof(InputView.SelectionLength))) return;
        QueueNativeEditingState(_active, _active.Text ?? string.Empty);
    }

    private void QueueNativeEditingState(InputView input, string text)
    {
        _pendingNativeText = text;
        _pendingNativeInput = input;
        if (_editingStateDispatchPending) return;
        _editingStateDispatchPending = true;

        // Selection-only notifications can arrive before the platform has
        // finished normalizing both ends. Coalesce those notifications by one
        // UI turn; text mutations take the synchronous path above and cannot be
        // starved by continuous native key repeat.
        if (!input.Dispatcher.DispatchDelayed(TimeSpan.Zero, PublishPendingEditingState))
        {
            _editingStateDispatchPending = false;
            PublishPendingEditingState();
        }
    }

    private void PublishPendingEditingState()
    {
        _editingStateDispatchPending = false;
        var input = _pendingNativeInput;
        var text = _pendingNativeText;
        _pendingNativeInput = null;
        _pendingNativeText = null;
        if (_disposed || _updating || !_hasClient || text is null ||
            input is null || !ReferenceEquals(input, _active)) return;

        // UIKit and Android can both normalize selection after their text
        // callback. Clamp the finalized native selection to the exact value
        // being sent so the framework never observes mismatched revisions.
        var start = Math.Clamp(input.CursorPosition, 0, text.Length);
        var end = Math.Clamp(start + Math.Max(0, input.SelectionLength), start, text.Length);
        PublishNativeEditingState(input, text, start, end);
    }

    private void PublishNativeEditingState(InputView input, string text, int start, int end)
    {
        // A synchronous text change supersedes any older queued snapshot. The
        // already-posted callback is intentionally left registered; it will
        // observe these nulls and become a harmless no-op.
        _pendingNativeInput = null;
        _pendingNativeText = null;
        if (_disposed || _updating || !_hasClient || !ReferenceEquals(input, _active)) return;
        EditingStateChanged?.Invoke(new DorotiTextEditingState(
            text,
            new(start, end),
            ReadNativeComposingRange(input, text.Length)));
    }

    internal static int ResolveSelectionAfterTextChange(string oldText, string newText)
    {
        var prefixLength = 0;
        var sharedLength = Math.Min(oldText.Length, newText.Length);
        while (prefixLength < sharedLength && oldText[prefixLength] == newText[prefixLength])
            prefixLength++;

        var suffixLength = 0;
        while (suffixLength < oldText.Length - prefixLength &&
               suffixLength < newText.Length - prefixLength &&
               oldText[oldText.Length - 1 - suffixLength] == newText[newText.Length - 1 - suffixLength])
        {
            suffixLength++;
        }

        return newText.Length - suffixLength;
    }

    private static DorotiTextSelection? ReadNativeComposingRange(InputView input, int textLength)
    {
#if ANDROID
        if (input.Handler?.PlatformView is Android.Widget.EditText nativeView &&
            nativeView.EditableText is Android.Text.ISpannable editable)
        {
            var start = Android.Views.InputMethods.BaseInputConnection.GetComposingSpanStart(editable);
            var end = Android.Views.InputMethods.BaseInputConnection.GetComposingSpanEnd(editable);
            if (start >= 0 && end > start && end <= textLength) return new(start, end);
        }
#elif IOS || MACCATALYST
        if (input.Handler?.PlatformView is UIKit.IUITextInput nativeInput &&
            nativeInput.MarkedTextRange is { } markedRange)
        {
            var beginning = nativeInput.BeginningOfDocument;
            var start = checked((int)nativeInput.GetOffsetFromPosition(beginning, markedRange.Start));
            var end = checked((int)nativeInput.GetOffsetFromPosition(beginning, markedRange.End));
            if (start >= 0 && end > start && end <= textLength) return new(start, end);
        }
#elif MACOS
        AppKit.NSTextView? nativeTextView = input.Handler?.PlatformView switch
        {
            AppKit.NSTextView textView => textView,
            AppKit.NSTextField textField => textField.CurrentEditor as AppKit.NSTextView,
            AppKit.NSScrollView scrollView => scrollView.DocumentView as AppKit.NSTextView,
            _ => null,
        };
        if (nativeTextView is { HasMarkedText: true })
        {
            var range = nativeTextView.MarkedRange;
            var start = checked((int)range.Location);
            var end = checked(start + (int)range.Length);
            if (start >= 0 && end > start && end <= textLength) return new(start, end);
        }
#endif
        return null;
    }

    private void HandleCompleted(object? sender, EventArgs args)
    {
        _ = args;
        if (ReferenceEquals(sender, _active)) ActionPerformed?.Invoke(_configuration.inputAction);
    }

    private void HandleFocused(object? sender, FocusEventArgs args)
    {
        if (ReferenceEquals(sender, _active)) FocusChanged?.Invoke(true);
    }

    private void HandleUnfocused(object? sender, FocusEventArgs args)
    {
        if (ReferenceEquals(sender, _active)) FocusChanged?.Invoke(false);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _entry.TextChanged -= HandleTextChanged;
        _editor.TextChanged -= HandleTextChanged;
        _entry.PropertyChanged -= HandleInputPropertyChanged;
        _editor.PropertyChanged -= HandleInputPropertyChanged;
        _entry.Completed -= HandleCompleted;
        _editor.Completed -= HandleCompleted;
        _entry.Focused -= HandleFocused;
        _editor.Focused -= HandleFocused;
        _entry.Unfocused -= HandleUnfocused;
        _editor.Unfocused -= HandleUnfocused;
        DetachInputs();
    }
}
