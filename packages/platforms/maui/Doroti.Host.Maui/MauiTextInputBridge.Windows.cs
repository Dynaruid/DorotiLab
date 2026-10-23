#if WINDOWS
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml.Controls;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;

namespace Doroti.Host.Maui;

public sealed partial class MauiTextInputBridge
{
    private DorotiTextSelection? _windowsComposing;
    private bool _windowsCompositionActive;
    internal bool WindowsCompositionActive => _windowsCompositionActive;
    private readonly Dictionary<InputView, WindowsCompositionSubscription> _windowsInputs = [];

    private bool UsesWindowsNativeTextNotifications(InputView input) =>
        _windowsInputs.TryGetValue(input, out var subscription) && subscription.SupportsNativeSelection;

    private bool WindowsNativeTextChangePending(InputView input) =>
        _windowsInputs.TryGetValue(input, out var subscription) && subscription.SupportsNativeSelection && subscription.TextChangePending;

    private static bool TryReadWindowsSelection(InputView input, string text, out DorotiTextSelection selection)
    {
        // PasswordBox exposes no selection. Keep the existing fallback for that
        // endpoint and for detached handlers; do not copy TextBox.Text per edit.
        if (input.Handler?.PlatformView is TextBox native
            && native is not Microsoft.Maui.Platform.MauiPasswordTextBox { IsPassword: true }
            && string.Equals(input.Text, text, StringComparison.Ordinal))
        {
            var start = native.SelectionStart;
            var length = native.SelectionLength;
            if (start >= 0 && start <= text.Length && length >= 0 && length <= text.Length - start)
            {
                selection = new(start, start + length);
                return true;
            }
        }
        selection = default;
        return false;
    }

    private void HandleWindowsInputHandlerChanged(object? sender, EventArgs args)
    {
        if (sender is InputView input) AttachWindowsInput(input);
    }

    private void AttachWindowsInput(InputView input)
    {
        DetachWindowsInput(input);
        if (!_disposed && input.Handler?.PlatformView is TextBox text)
            _windowsInputs[input] = new(this, input, text);
    }

    private void DetachWindowsInput(InputView input)
    {
        if (_windowsInputs.Remove(input, out var subscription)) subscription.Dispose();
        if (ReferenceEquals(input, _active)) { _windowsComposing = null; _windowsCompositionActive = false; }
    }

    private sealed class WindowsCompositionSubscription : IDisposable
    {
        private readonly MauiTextInputBridge _owner;
        private readonly InputView _input;
        private readonly TextBox _text;
        private bool _disposed;
        internal bool TextChangePending { get; private set; }
        internal bool SupportsNativeSelection => _text is not Microsoft.Maui.Platform.MauiPasswordTextBox { IsPassword: true };

        internal WindowsCompositionSubscription(MauiTextInputBridge owner, InputView input, TextBox text)
        {
            _owner = owner; _input = input; _text = text;
            text.TextCompositionStarted += Started;
            text.TextCompositionChanged += Changed;
            text.TextCompositionEnded += Ended;
            text.TextChanging += NativeTextChanging;
            text.TextChanged += NativeTextChanged;
        }
        private void NativeTextChanging(TextBox sender, TextBoxTextChangingEventArgs args) => TextChangePending = true;
        private void NativeTextChanged(object sender, Microsoft.UI.Xaml.Controls.TextChangedEventArgs args)
        {
            TextChangePending = false;
            if (!SupportsNativeSelection || _disposed || _owner._disposed || !_owner._hasClient || _owner._updating
                || _owner._drainingInputMutations || _owner.HasPendingClientChange
                || !ReferenceEquals(_owner._active, _input) || !ReferenceEquals(_input.Handler?.PlatformView, _text)) return;
            // MAUI's handler subscribed before HandlerChanged attached us and
            // has already materialized this revision. Avoid another Text read.
            var text = _input.Text ?? string.Empty;
            if (!TryReadWindowsSelection(_input, text, out var selection)) return;
            _owner._publishingNativeTextChange = true;
            try { _owner.PublishNativeEditingState(_input, text, selection.baseOffset, selection.extentOffset); }
            finally { _owner._publishingNativeTextChange = false; }
        }
        private void Started(TextBox sender, TextCompositionStartedEventArgs args) => Update(args.StartIndex, args.Length, true);
        private void Changed(TextBox sender, TextCompositionChangedEventArgs args) => Update(args.StartIndex, args.Length, true);
        private void Ended(TextBox sender, TextCompositionEndedEventArgs args) => Update(-1, 0, false);
        private void Update(int start, int length, bool active)
        {
            if (_disposed || _owner._disposed || !_owner._hasClient || _owner._updating
                || !ReferenceEquals(_owner._active, _input) || !ReferenceEquals(_input.Handler?.PlatformView, _text)) return;
            _owner._windowsCompositionActive = active;
            _owner._windowsComposing = start >= 0 && length > 0 ? new(start, checked(start + length)) : null;
            _owner.QueueNativeEditingState(_input, _text.Text ?? "");
        }
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _text.TextCompositionStarted -= Started;
            _text.TextCompositionChanged -= Changed;
            _text.TextCompositionEnded -= Ended;
            _text.TextChanging -= NativeTextChanging;
            _text.TextChanged -= NativeTextChanged;
        }
    }
}
#endif
