using Doroti.Graphics;

namespace Doroti.Platform;

public enum CursorKind
{
    Basic,
    Click,
    Forbidden,
    Wait,
    Progress,
    ContextMenu,
    Help,
    Text,
    VerticalText,
    Cell,
    Precise,
    Move,
    Grab,
    Grabbing,
    NoDrop,
    Alias,
    Copy,
    Disappearing,
    AllScroll,
    ResizeLeftRight,
    ResizeUpDown,
    ResizeUpLeftDownRight,
    ResizeUpRightDownLeft,
    ResizeUp,
    ResizeDown,
    ResizeLeft,
    ResizeRight,
    ResizeUpLeft,
    ResizeUpRight,
    ResizeDownLeft,
    ResizeDownRight,
    ResizeColumn,
    ResizeRow,
    ZoomIn,
    ZoomOut,
    Hidden,
}

public interface ICursorController
{
    void SetCursor(WindowId window, CursorKind cursor);
}

public readonly record struct TextSelection(int BaseOffset, int ExtentOffset);

public readonly record struct TextEditingState(string Text, TextSelection Selection, TextSelection? ComposingRange);

public static class TextEditingStateReducer
{
    public static TextEditingState BeginComposition(TextEditingState state)
    {
        var normalized = Normalize(state);
        var start = Math.Min(normalized.Selection.BaseOffset, normalized.Selection.ExtentOffset);
        var end = Math.Max(normalized.Selection.BaseOffset, normalized.Selection.ExtentOffset);
        return normalized with { ComposingRange = new(start, end) };
    }

    public static TextEditingState UpdateComposition(TextEditingState state, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var normalized = Normalize(state);
        var range = normalized.ComposingRange ?? normalized.Selection;
        return Replace(normalized, range, text, composing: true);
    }

    public static TextEditingState CommitText(TextEditingState state, string text)
    {
        ArgumentNullException.ThrowIfNull(text);
        var normalized = Normalize(state);
        return Replace(normalized, normalized.ComposingRange ?? normalized.Selection, text, composing: false);
    }

    public static TextEditingState CancelComposition(TextEditingState original) => Normalize(original) with { ComposingRange = null };

    private static TextEditingState Replace(TextEditingState state, TextSelection range, string replacement, bool composing)
    {
        var start = Math.Min(range.BaseOffset, range.ExtentOffset);
        var end = Math.Max(range.BaseOffset, range.ExtentOffset);
        var nextText = state.Text.Remove(start, end - start).Insert(start, replacement);
        var caret = start + replacement.Length;
        return new(
            nextText,
            new(caret, caret),
            composing ? new TextSelection(start, caret) : null);
    }

    private static TextEditingState Normalize(TextEditingState state)
    {
        ArgumentNullException.ThrowIfNull(state.Text);
        var selection = new TextSelection(
            Math.Clamp(state.Selection.BaseOffset, 0, state.Text.Length),
            Math.Clamp(state.Selection.ExtentOffset, 0, state.Text.Length));
        var composing = state.ComposingRange is { } range
            ? new TextSelection(
                Math.Clamp(range.BaseOffset, 0, state.Text.Length),
                Math.Clamp(range.ExtentOffset, 0, state.Text.Length))
            : (TextSelection?)null;
        return new(state.Text, selection, composing);
    }
}

public interface ITextInputClient
{
    void UpdateEditingState(TextEditingState state);

    void PerformAction(TextInputAction action);
}

public interface ITextInputConnection : IDisposable
{
    void SetClient(ITextInputClient client, TextEditingState initialState);

    void UpdateState(TextEditingState state);

    void ClearClient();
}

public sealed class UnsupportedTextInputConnection(string diagnostic) : ITextInputConnection
{
    public void SetClient(ITextInputClient client, TextEditingState initialState) => throw new NotSupportedException(diagnostic);

    public void UpdateState(TextEditingState state) => throw new NotSupportedException(diagnostic);

    public void ClearClient()
    {
    }

    public void Dispose()
    {
    }
}

public sealed class UnsupportedCursorController(string diagnostic) : ICursorController
{
    public void SetCursor(WindowId window, CursorKind cursor) => throw new NotSupportedException(diagnostic);
}

public interface ITextInputGeometry
{
    void SetCaretRect(Rect logicalRect);
}

public enum TextInputAction
{
    None,
    Done,
    Next,
    Previous,
    NewLine,
}
