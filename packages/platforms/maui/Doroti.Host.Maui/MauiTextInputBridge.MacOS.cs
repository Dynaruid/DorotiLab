#if MACOS
using AppKit;
using Foundation;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

public sealed partial class MauiTextInputBridge
{
    private readonly Dictionary<InputView, NSObject> _macOSInputObservers = [];
    private readonly Dictionary<InputView, MacOSTextSubscription> _macOSTextInputs = [];
    internal int MacOSNativeTextReadCount => _macOSTextInputs.Values.Sum(input => input.NativeTextReads);
    private long _macOSFocusRequest;
    private bool _macOSNativeFocus;

    internal void YieldMacOSNativeFocus()
    {
        _macOSNativeFocus = true;
        ++_macOSFocusRequest;
        // Preserve the framework client, but stop its hidden field editor from reclaiming focus.
        DeactivateActiveInput(clearFocus: true);
        DetachInputs();
    }

    private void HandleMacOSInputHandlerChanged(object? sender, EventArgs args)
    {
        if (sender is InputView input)
        {
            AttachMacOSInput(input);
        }
    }

    private void AttachMacOSInput(InputView input)
    {
        if (_macOSTextInputs.TryGetValue(input, out var current) && current.Matches(input.Handler?.PlatformView))
        {
            current.EnsureEditor();
            return;
        }
        DetachMacOSInput(input);
        if (input.Handler?.PlatformView is NSView native)
            _macOSTextInputs[input] = new(this, input, native);
        if (input.Handler?.PlatformView is not NSTextField field)
        {
            return;
        }
        // The preview EntryHandler forwards every EditingEnded as Completed,
        // including NSTextMovement.Other during layout. Subscribe to the native
        // notification so only an actual Return submits the framework client.
        _macOSInputObservers.Add(
            input,
            NSNotificationCenter.DefaultCenter.AddObserver(
                NSControl.TextDidEndEditingNotification,
                notification => HandleMacOSEditingEnded(input, notification),
                field
            )
        );
    }

    private void DetachMacOSInput(InputView input)
    {
        if (_macOSTextInputs.Remove(input, out var text)) text.Dispose();
        if (_macOSInputObservers.Remove(input, out var observer))
        {
            observer.Dispose();
        }
    }

    private string ReadMacOSText(InputView input) => _macOSTextInputs.TryGetValue(input, out var subscription)
        ? subscription.ReadText() : input.Text ?? string.Empty;

    private void RecordMacOSText(InputView input, string text)
    {
        if (_macOSTextInputs.TryGetValue(input, out var subscription)) subscription.RecordText(text);
    }

    private sealed class MacOSTextSubscription : IDisposable
    {
        private readonly MauiTextInputBridge _owner;
        private readonly InputView _input;
        private readonly NSView _native;
        private readonly NSObject? _begin;
        private readonly List<NSObject> _observers = [];
        private NSTextView? _editor;
        private string _text;
        private bool _dirty;
        private bool _disposed;
        internal int NativeTextReads { get; private set; }

        internal MacOSTextSubscription(MauiTextInputBridge owner, InputView input, NSView native)
        {
            _owner = owner; _input = input; _native = native;
            _text = input.Text ?? string.Empty;
            if (native is NSTextField field)
                _begin = NSNotificationCenter.DefaultCenter.AddObserver(NSControl.TextDidBeginEditingNotification,
                    _ => AttachEditor(), field);
            AttachEditor();
        }

        private void AttachEditor()
        {
            var editor = NativeMacOSTextView(_native);
            if (_disposed || ReferenceEquals(editor, _editor)) return;
            foreach (var observer in _observers) observer.Dispose();
            _observers.Clear();
            _editor = editor;
            _text = _input.Text ?? string.Empty;
            _dirty = false;
            if (editor is null) return;
            _observers.Add(NSNotificationCenter.DefaultCenter.AddObserver(NSTextView.DidChangeSelectionNotification,
                _ => Changed(), editor));
            _observers.Add(NSNotificationCenter.DefaultCenter.AddObserver(NSText.DidChangeNotification,
                _ => Changed(), editor));
            if (editor.TextStorage is { } storage)
                _observers.Add(NSTextStorage.Notifications.ObserveDidProcessEditing(storage,
                    (_, _) =>
                    {
                        if ((storage.EditedMask & NSTextStorageEditActions.Characters) != 0) _dirty = true;
                        Changed();
                    }));
        }

        private void Changed()
        {
            if (_disposed || _owner._disposed || _owner._updating || _owner._drainingInputMutations
                || _owner.HasPendingClientChange || !_owner._hasClient || !ReferenceEquals(_owner._active, _input)
                || !ReferenceEquals(_input.Handler?.PlatformView, _native)
                || !ReferenceEquals(NativeMacOSTextView(_native), _editor)) return;
            // Marked text need not update NSTextField.StringValue/MAUI Text.
            // Let storage and selection normalization finish; ordinary MAUI
            // text changes still publish synchronously and supersede this job.
            _owner.QueueNativeEditingState(_input, _input.Text ?? string.Empty);
        }

        internal string ReadText()
        {
            if (!ReferenceEquals(NativeMacOSTextView(_native), _editor)) AttachEditor();
            if (_editor is null) return _input.Text ?? string.Empty;
            if (_dirty)
            {
                _text = _editor.Value;
                NativeTextReads++;
                _dirty = false;
            }
            return _text;
        }

        internal void RecordText(string text) { _text = text; _dirty = false; }
        internal void EnsureEditor() => AttachEditor();
        internal bool Matches(object? native) => ReferenceEquals(native, _native);

        public void Dispose()
        {
            _disposed = true;
            _begin?.Dispose();
            foreach (var observer in _observers) observer.Dispose();
            _observers.Clear();
            _editor = null;
        }
    }

    private void HandleMacOSEditingEnded(InputView input, NSNotification notification)
    {
        if (_disposed || !_hasClient || !ReferenceEquals(input, _active))
        {
            return;
        }

        var movement = notification.UserInfo?["NSTextMovement"] as NSNumber;
        if (movement?.Int64Value == (long)NSTextMovement.Return)
        {
            ActionPerformed?.Invoke(_configuration.inputAction);
        }
        else
        {
            ScheduleMacOSFocus(input);
        }
    }

    private void ScheduleMacOSFocus(InputView input)
    {
        var request = ++_macOSFocusRequest;
        // SetClient/show can run inside a framework focus callback or native
        // layout. Let both finish before installing AppKit's field editor.
        NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
        {
            if (
                _disposed
                || _suspended
                || _macOSNativeFocus
                || !_hasClient
                || request != _macOSFocusRequest
                || !ReferenceEquals(input, _active)
                || input.Handler?.PlatformView is not NSView native
            )
            {
                return;
            }

            native.Superview?.LayoutSubtreeIfNeeded();
            if (_disposed || !_hasClient || !ReferenceEquals(input, _active))
            {
                return;
            }

            var editor = NativeMacOSTextView(native);
            if (editor is not null && ReferenceEquals(native.Window?.FirstResponder, editor))
            {
                return;
            }

            var start = Math.Clamp(
                input.CursorPosition,
                0,
                (input.Text ?? string.Empty).Length
            );
            var length = Math.Clamp(
                input.SelectionLength,
                0,
                (input.Text ?? string.Empty).Length - start
            );
            var focused = input.Focus();
            editor = NativeMacOSTextView(native);
            if (_macOSTextInputs.TryGetValue(input, out var subscription)) subscription.EnsureEditor();
            if (focused && editor is not null)
            {
                editor.SetSelectedRange(new NSRange(start, length));
            }
            if (Environment.GetEnvironmentVariable("DOROTI_TRACE_MAC_INPUT") == "1")
            {
                Console.WriteLine(
                    $"[AppKit text focus] result={focused} native={native.GetType().Name} responder={native.Window?.FirstResponder?.GetType().Name}"
                );
            }
        });
    }

    private static NSTextView? NativeMacOSTextView(NSView native) =>
        native switch
        {
            NSTextField field => field.CurrentEditor as NSTextView,
            NSTextView textView => textView,
            NSScrollView scrollView => scrollView.DocumentView as NSTextView,
            _ => null,
        };
}
#endif
