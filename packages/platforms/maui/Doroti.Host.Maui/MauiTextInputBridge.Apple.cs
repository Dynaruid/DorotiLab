#if IOS || MACCATALYST || MACOS
using Doroti.Ui;
using Microsoft.Maui.Controls;
#if MACOS
using AppKit;
#else
using UIKit;
#endif

namespace Doroti.Host.Maui;

public sealed partial class MauiTextInputBridge
{
    private long _appleNativeRevision;
    private long _appleClientRevision;
    private long _applePublishedClientRevision;
    private long _appleStateRequestRevision;
    private DorotiTextEditingState? _applePublishedState;

    private void UpdateAppleState(DorotiTextEditingState state)
    {
        long nativeRevision, clientRevision, requestRevision;
        DorotiTextEditingState? expected;
        lock (_inputMutationGate)
        {
            nativeRevision = _appleNativeRevision;
            requestRevision = ++_appleStateRequestRevision;
            expected = _publishingNativeTextChange ? _applePublishedState : null;
            clientRevision = expected.HasValue ? _applePublishedClientRevision : _appleClientRevision;
        }
        // Accepted native echoes must not write a stale MAUI cursor back after
        // this callback, or collapse the platform's marked range. A formatter's changed
        // text/selection still follows the ordered, deferred mutation path.
        if (expected == state) return;
        DispatchInputMutation(() =>
        {
            lock (_inputMutationGate)
            {
                if (nativeRevision != _appleNativeRevision || clientRevision != _appleClientRevision
                    || requestRevision != _appleStateRequestRevision) return;
            }
            // Selection/marked-text changes can precede the coalesced native
            // notification. Check those endpoints too before applying a reply.
            if (expected is { } snapshot && (_active is null
                || !TryReadAppleSelection(_active, snapshot.text, out var selection)
                || selection != snapshot.selection
                || ReadNativeComposingRange(_active, snapshot.text.Length) != snapshot.composingRange)) return;
            UpdateStateCore(state);
        }, defer: _publishingNativeTextChange);
    }

    private DorotiTextEditingState ReadAppleState(InputView input)
    {
#if MACOS
        var text = ReadMacOSText(input);
#else
        var text = input.Text ?? string.Empty;
#endif
        if (!TryReadAppleSelection(input, text, out var selection))
        {
            var start = Math.Clamp(input.CursorPosition, 0, text.Length);
            selection = new(start, Math.Clamp(start + Math.Max(0, input.SelectionLength), start, text.Length));
        }
        return new(text, selection, ReadNativeComposingRange(input, text.Length));
    }

    private void ApplyAppleNativeState(InputView input, DorotiTextEditingState state, int start, int end, bool textChanged)
    {
#if MACOS
        if (input.Handler?.PlatformView is NSView view && NativeMacOSTextView(view) is { } editor)
        {
            if (state.composingRange is null && editor.HasMarkedText) editor.UnmarkText();
            // NSTextField.StringValue can change without replacing its active
            // field editor's buffer. Apply intentional framework text there too.
            if (textChanged && (view is NSTextField || !string.Equals(input.Text, state.text, StringComparison.Ordinal)))
                editor.Value = state.text;
            var range = editor.SelectedRange;
            if (range.Location != start || range.Length != end - start)
                editor.SetSelectedRange(new Foundation.NSRange(start, end - start));
            RecordMacOSText(input, state.text);
        }
#else
        if (input.Handler?.PlatformView is IUITextInput native)
        {
            if (state.composingRange is null && native.MarkedTextRange is not null) native.UnmarkText();
            if (TryReadAppleSelection(input, state.text, out var selection) && selection != new DorotiTextSelection(start, end))
                native.SelectedTextRange = native.GetTextRange(native.GetPosition(native.BeginningOfDocument, start)!,
                    native.GetPosition(native.BeginningOfDocument, end)!);
        }
#endif
    }

    private bool TryReadAppleSelection(InputView input, string text, out DorotiTextSelection selection)
    {
        selection = default;
#if MACOS
        if (!string.Equals(ReadMacOSText(input), text, StringComparison.Ordinal)) return false;
        if (input.Handler?.PlatformView is not NSView view || NativeMacOSTextView(view) is not { } native
            || native.TextStorage?.Length != text.Length) return false;
        var range = native.SelectedRange;
        // NSNotFound and invalid/overflowing ranges must not be clamped into a
        // plausible caret. Storage length/range queries do not copy text.
        if (range.Location < 0 || range.Location > text.Length
            || range.Length < 0 || range.Length > text.Length - range.Location) return false;
        selection = new((int)range.Location, (int)(range.Location + range.Length));
#else
        if (!string.Equals(input.Text ?? string.Empty, text, StringComparison.Ordinal)) return false;
        if (input.Handler?.PlatformView is not IUITextInput native || native.SelectedTextRange is not { } range)
            return false;
        var beginning = native.BeginningOfDocument;
        if (native.GetOffsetFromPosition(beginning, native.EndOfDocument) != text.Length) return false;
        var start = native.GetOffsetFromPosition(beginning, range.Start);
        var end = native.GetOffsetFromPosition(beginning, range.End);
        if (start < 0 || end < start || end > text.Length) return false;
        selection = new((int)start, (int)end);
#endif
        return true;
    }
}
#endif
