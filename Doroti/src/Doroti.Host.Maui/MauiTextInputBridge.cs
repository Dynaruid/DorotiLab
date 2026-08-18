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
    private bool _disposed;

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
        _updating = true;
        try
        {
            _active.Text = state.text;
            var length = state.text.Length;
            var start = Math.Clamp(Math.Min(state.selection.baseOffset, state.selection.extentOffset), 0, length);
            var end = Math.Clamp(Math.Max(state.selection.baseOffset, state.selection.extentOffset), start, length);
            _active.CursorPosition = start;
            _active.SelectionLength = end - start;
        }
        finally
        {
            _updating = false;
        }
    }

    internal void SetCaretRect(Doroti.Ui.Rect rect)
    {
        _active.TranslationX = Math.Max(0, rect.left);
        _active.TranslationY = Math.Max(0, rect.top);
        _active.WidthRequest = Math.Max(1, rect.width);
        _active.HeightRequest = Math.Max(1, rect.height);
    }

    internal void ClearClient()
    {
        if (_disposed) return;
        _hasClient = false;
        _active.Unfocus();
        _active.Text = string.Empty;
        _active.WidthRequest = 1;
        _active.HeightRequest = 1;
        DetachInputs();
    }

    internal void Suspend()
    {
        if (_disposed || !_attachOnDemand) return;
        _suspended = true;
        _active.Unfocus();
        DetachInputs();
    }

    internal void Resume()
    {
        if (_disposed || !_attachOnDemand) return;
        _suspended = false;
        if (_hasClient) AttachActiveInput(requestFocus: true);
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
        var start = _active.CursorPosition;
        EditingStateChanged?.Invoke(new(args.NewTextValue ?? string.Empty,
            new(start, start + _active.SelectionLength), null));
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
        _entry.Completed -= HandleCompleted;
        _editor.Completed -= HandleCompleted;
        _entry.Focused -= HandleFocused;
        _editor.Focused -= HandleFocused;
        _entry.Unfocused -= HandleUnfocused;
        _editor.Unfocused -= HandleUnfocused;
        DetachInputs();
    }
}
