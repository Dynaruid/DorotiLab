#if IOS || MACCATALYST
using CoreGraphics;
using Doroti.Ui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace Doroti.Host.Maui;

internal sealed class DorotiUIKitEntry : Entry;

internal sealed class DorotiUIKitEditor : Editor;

internal sealed class DorotiUIKitEntryHandler : EntryHandler
{
    protected override MauiTextField CreatePlatformView() => new DorotiUIKitTextField();
}

internal sealed class DorotiUIKitEditorHandler : EditorHandler
{
    protected override MauiTextView CreatePlatformView() => new DorotiUIKitTextView();
}

internal interface IDorotiUIKitTextInput
{
    CGRect CaretRect { get; set; }
    Action<DorotiFloatingCursorEvent>? FloatingCursorChanged { get; set; }
    void ResetFloatingCursor();
}

internal sealed class UIKitFloatingCursor
{
    private CGPoint _origin;
    private bool _active;
    public Action<DorotiFloatingCursorEvent>? Changed { get; set; }

    public void Begin(CGPoint point)
    {
        _origin = point;
        _active = true;
        Changed?.Invoke(new(DorotiFloatingCursorPhase.start, Offset.zero));
    }

    public void Update(CGPoint point)
    {
        if (_active)
        {
            Changed?.Invoke(
                new(
                    DorotiFloatingCursorPhase.update,
                    new Offset(point.X - _origin.X, point.Y - _origin.Y)
                )
            );
        }
    }

    public void End()
    {
        if (!_active)
        {
            return;
        }

        _active = false;
        Changed?.Invoke(new(DorotiFloatingCursorPhase.end, Offset.zero));
    }

    public void Reset() => _active = false;
}

internal sealed class DorotiUIKitTextField : MauiTextField, IDorotiUIKitTextInput
{
    public DorotiUIKitTextField() => SmartInsertDeleteType = UITextSmartInsertDeleteType.No;

    private readonly UIKitFloatingCursor _cursor = new();
    public CGRect CaretRect { get; set; }
    public Action<DorotiFloatingCursorEvent>? FloatingCursorChanged
    {
        get => _cursor.Changed;
        set => _cursor.Changed = value;
    }

    public void ResetFloatingCursor() => _cursor.Reset();

    // Selection offsets still belong to UITextInput for native menu commands.
    // Only Doroti paints range highlights/handles; exporting UIKit's native
    // layout rectangles can produce a second set in its selection overlay.
    public override UITextSelectionRect[] GetSelectionRects(UITextRange? range) => [];

    public override CGRect GetCaretRectForPosition(UITextPosition? position) => CaretRect;

    public override void BeginFloatingCursor(CGPoint point) => _cursor.Begin(point);

    public override void UpdateFloatingCursor(CGPoint point) => _cursor.Update(point);

    public override void EndFloatingCursor() => _cursor.End();
}

internal sealed class DorotiUIKitTextView : MauiTextView, IDorotiUIKitTextInput
{
    public DorotiUIKitTextView() => SmartInsertDeleteType = UITextSmartInsertDeleteType.No;
    internal Func<bool>? PerformInputAction { get; set; }

    public override void InsertText(string text)
    {
        if (text == "\n" && MarkedTextRange is null && PerformInputAction?.Invoke() == true)
        {
            return;
        }
        base.InsertText(text);
    }

    private readonly UIKitFloatingCursor _cursor = new();
    public CGRect CaretRect { get; set; }
    public Action<DorotiFloatingCursorEvent>? FloatingCursorChanged
    {
        get => _cursor.Changed;
        set => _cursor.Changed = value;
    }

    public void ResetFloatingCursor() => _cursor.Reset();

    // Selection offsets still belong to UITextInput for native menu commands.
    // Only Doroti paints range highlights/handles; exporting UIKit's native
    // layout rectangles can produce a second set in its selection overlay.
    public override UITextSelectionRect[] GetSelectionRects(UITextRange? range) => [];

    public override CGRect GetCaretRectForPosition(UITextPosition? position) => CaretRect;

    public override void BeginFloatingCursor(CGPoint point) => _cursor.Begin(point);

    public override void UpdateFloatingCursor(CGPoint point) => _cursor.Update(point);

    public override void EndFloatingCursor() => _cursor.End();
}

public sealed partial class MauiTextInputBridge
{
    private void HandleUIKitInputHandlerChanged(object? sender, EventArgs args)
    {
        if (sender is InputView input)
        {
            AttachUIKitInput(input);
        }
    }

    private void AttachUIKitInput(InputView input)
    {
        if (_hasClient && ReferenceEquals(input, _active))
        {
            ConfigureUIKitInputTraits(input, _configuration);
        }
        if (input.Handler?.PlatformView is DorotiUIKitTextView editor)
        {
            editor.PerformInputAction = () =>
            {
                if (_disposed || _updating || _drainingInputMutations || HasPendingClientChange
                    || !_hasClient || !ReferenceEquals(input, _active)
                    || !ReferenceEquals(input.Handler?.PlatformView, editor)
                    || _configuration.inputAction is DorotiTextInputAction.none
                        or DorotiTextInputAction.unspecified or DorotiTextInputAction.newline)
                    return false;
                ActionPerformed?.Invoke(_configuration.inputAction);
                return true;
            };
        }
        if (input.Handler?.PlatformView is not IDorotiUIKitTextInput native)
        {
            return;
        }

        native.FloatingCursorChanged = point =>
        {
            if (
                _disposed
                || !_hasClient
                || HasPendingClientChange
                || !ReferenceEquals(input, _active)
                || !ReferenceEquals(input.Handler?.PlatformView, native)
            )
            {
                return;
            }

            FloatingCursorChanged?.Invoke(point);
        };
    }

    private static void ConfigureUIKitInputTraits(InputView input, DorotiTextInputConfiguration configuration)
    {
        var key = configuration.inputAction switch
        {
            DorotiTextInputAction.done => UIReturnKeyType.Done,
            DorotiTextInputAction.go => UIReturnKeyType.Go,
            DorotiTextInputAction.search => UIReturnKeyType.Search,
            DorotiTextInputAction.send => UIReturnKeyType.Send,
            DorotiTextInputAction.next => UIReturnKeyType.Next,
            DorotiTextInputAction.continueAction => UIReturnKeyType.Continue,
            DorotiTextInputAction.join => UIReturnKeyType.Join,
            DorotiTextInputAction.route => UIReturnKeyType.Route,
            DorotiTextInputAction.emergencyCall => UIReturnKeyType.EmergencyCall,
            _ => UIReturnKeyType.Default,
        };
        if (input.Handler?.PlatformView is UITextField field)
        {
            var changed = field.SecureTextEntry != configuration.obscureText || field.ReturnKeyType != key;
            if (field.SecureTextEntry != configuration.obscureText) field.SecureTextEntry = configuration.obscureText;
            field.ReturnKeyType = key;
            if (changed && field.IsFirstResponder) field.ReloadInputViews();
        }
        else if (input.Handler?.PlatformView is UITextView view && view.ReturnKeyType != key)
        {
            view.ReturnKeyType = key;
            if (view.IsFirstResponder) view.ReloadInputViews();
        }
    }

    private void ResetUIKitInput()
    {
#if IOS
        ResetSystemMagnifier();
#endif
        if (_active?.Handler?.PlatformView is IDorotiUIKitTextInput native)
        {
            native.ResetFloatingCursor();
        }

        HideSystemContextMenu();
    }
}
#endif
