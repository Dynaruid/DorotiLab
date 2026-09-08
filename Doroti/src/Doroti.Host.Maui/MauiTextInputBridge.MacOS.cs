#if MACOS
using AppKit;
using Foundation;
using Microsoft.Maui.Controls;

namespace Doroti.Host.Maui;

public sealed partial class MauiTextInputBridge
{
    private readonly Dictionary<InputView, NSObject> _macOSInputObservers = [];
    private long _macOSFocusRequest;

    private void HandleMacOSInputHandlerChanged(object? sender, EventArgs args)
    {
        if (sender is InputView input) AttachMacOSInput(input);
    }

    private void AttachMacOSInput(InputView input)
    {
        DetachMacOSInput(input);
        if (input.Handler?.PlatformView is not NSTextField field) return;
        // The preview EntryHandler forwards every EditingEnded as Completed,
        // including NSTextMovement.Other during layout. Subscribe to the native
        // notification so only an actual Return submits the framework client.
        _macOSInputObservers.Add(input, NSNotificationCenter.DefaultCenter.AddObserver(
            NSControl.TextDidEndEditingNotification, notification => HandleMacOSEditingEnded(input, notification), field));
    }

    private void DetachMacOSInput(InputView input)
    {
        if (_macOSInputObservers.Remove(input, out var observer)) observer.Dispose();
    }

    private void HandleMacOSEditingEnded(InputView input, NSNotification notification)
    {
        if (_disposed || !_hasClient || !ReferenceEquals(input, _active)) return;
        var movement = notification.UserInfo?["NSTextMovement"] as NSNumber;
        if (movement?.Int64Value == (long)NSTextMovement.Return)
            ActionPerformed?.Invoke(_configuration.inputAction);
        else
            ScheduleMacOSFocus(input);
    }

    private void ScheduleMacOSFocus(InputView input)
    {
        var request = ++_macOSFocusRequest;
        // SetClient/show can run inside a framework focus callback or native
        // layout. Let both finish before installing AppKit's field editor.
        NSApplication.SharedApplication.BeginInvokeOnMainThread(() =>
        {
            if (_disposed || _suspended || !_hasClient || request != _macOSFocusRequest ||
                !ReferenceEquals(input, _active) || input.Handler?.PlatformView is not NSView native) return;
            native.Superview?.LayoutSubtreeIfNeeded();
            if (_disposed || !_hasClient || !ReferenceEquals(input, _active)) return;
            var editor = NativeMacOSTextView(native);
            if (editor is not null && ReferenceEquals(native.Window?.FirstResponder, editor)) return;
            var focused = input.Focus();
            editor = NativeMacOSTextView(native);
            if (focused && editor is not null)
            {
                var start = Math.Clamp(input.CursorPosition, 0, (input.Text ?? string.Empty).Length);
                var length = Math.Clamp(input.SelectionLength, 0, (input.Text ?? string.Empty).Length - start);
                editor.SetSelectedRange(new NSRange(start, length));
            }
            if (Environment.GetEnvironmentVariable("DOROTI_TRACE_MAC_INPUT") == "1")
                Console.WriteLine($"[AppKit text focus] result={focused} native={native.GetType().Name} responder={native.Window?.FirstResponder?.GetType().Name}");
        });
    }

    private static NSTextView? NativeMacOSTextView(NSView native) => native switch
    {
        NSTextField field => field.CurrentEditor as NSTextView,
        NSTextView textView => textView,
        NSScrollView scrollView => scrollView.DocumentView as NSTextView,
        _ => null,
    };
}
#endif
