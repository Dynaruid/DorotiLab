#if WINDOWS
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml.Controls;
using TextBox = Microsoft.UI.Xaml.Controls.TextBox;

namespace Doroti.Host.Maui;

public sealed partial class MauiTextInputBridge
{
    private DorotiTextSelection? _windowsComposing;
    private readonly Dictionary<InputView, WindowsCompositionSubscription> _windowsInputs = [];

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
        if (ReferenceEquals(input, _active)) _windowsComposing = null;
    }

    private sealed class WindowsCompositionSubscription : IDisposable
    {
        private readonly MauiTextInputBridge _owner;
        private readonly InputView _input;
        private readonly TextBox _text;
        private bool _disposed;

        internal WindowsCompositionSubscription(MauiTextInputBridge owner, InputView input, TextBox text)
        {
            _owner = owner; _input = input; _text = text;
            text.TextCompositionStarted += Started;
            text.TextCompositionChanged += Changed;
            text.TextCompositionEnded += Ended;
        }
        private void Started(TextBox sender, TextCompositionStartedEventArgs args) => Update(args.StartIndex, args.Length);
        private void Changed(TextBox sender, TextCompositionChangedEventArgs args) => Update(args.StartIndex, args.Length);
        private void Ended(TextBox sender, TextCompositionEndedEventArgs args) => Update(-1, 0);
        private void Update(int start, int length)
        {
            if (_disposed || _owner._disposed || !_owner._hasClient || _owner._updating
                || !ReferenceEquals(_owner._active, _input) || !ReferenceEquals(_input.Handler?.PlatformView, _text)) return;
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
        }
    }
}
#endif
