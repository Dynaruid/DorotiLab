using Doroti.Core;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;

namespace Doroti.Widgets;

public sealed class TextEditingController : Doroti.Generated.Framework.Foundation.ValueNotifier<TextEditingState>
{
    public TextEditingController(string text = "")
        : base(new(text ?? throw new ArgumentNullException(nameof(text)), new(text.Length, text.Length), null))
    {
    }

    public string Text
    {
        get => Value.Text;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            Value = new(value, new(value.Length, value.Length), null);
        }
    }

    public TextSelection Selection
    {
        get => Value.Selection;
        set => Value = Value with { Selection = NormalizeSelection(value, Value.Text.Length) };
    }

    private static TextSelection NormalizeSelection(TextSelection selection, int length) => new(
        Math.Clamp(selection.BaseOffset, 0, length),
        Math.Clamp(selection.ExtentOffset, 0, length));
}

public sealed class EditableText : LeafRenderObjectWidget
{
    public EditableText(
        TextEditingController controller,
        ITextInputConnection connection,
        FocusManager focusManager,
        FocusNode focusNode,
        double fontSize = 20,
        Color? color = null,
        Color? cursorColor = null,
        int maxLines = 1,
        TextInputAction textInputAction = TextInputAction.Done,
        Action<string>? onChanged = null,
        Action<TextInputAction>? onSubmitted = null,
        Key? key = null)
        : base(key)
    {
        Controller = controller ?? throw new ArgumentNullException(nameof(controller));
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        FocusManager = focusManager ?? throw new ArgumentNullException(nameof(focusManager));
        FocusNode = focusNode ?? throw new ArgumentNullException(nameof(focusNode));
        if (!double.IsFinite(fontSize) || fontSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fontSize));
        }
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxLines);
        FontSize = fontSize;
        Color = color ?? Color.FromArgb(255, 20, 24, 32);
        CursorColor = cursorColor ?? Color.FromArgb(255, 45, 115, 235);
        MaxLines = maxLines;
        TextInputAction = textInputAction;
        OnChanged = onChanged;
        OnSubmitted = onSubmitted;
    }

    public TextEditingController Controller { get; }

    public ITextInputConnection Connection { get; }

    public FocusManager FocusManager { get; }

    public FocusNode FocusNode { get; }

    public double FontSize { get; }

    public Color Color { get; }

    public Color CursorColor { get; }

    public int MaxLines { get; }

    public TextInputAction TextInputAction { get; }

    public Action<string>? OnChanged { get; }

    public Action<TextInputAction>? OnSubmitted { get; }

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderEditableText(this);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject) =>
        ((RenderEditableText)renderObject).Update(this);
}

public sealed class RenderEditableText : RenderBox, ITextInputClient, IPointerEventTarget, IFocusableKeyboardTarget, IDisposable
{
    private readonly UnicodeParagraphLayout _paragraphLayout = new();
    private EditableText _configuration;
    private ImmutableParagraphSnapshot? _paragraph;
    private bool _disposed;

    public RenderEditableText(EditableText configuration)
    {
        _configuration = configuration;
        _configuration.Controller.AddListener(HandleControllerChanged);
        _configuration.FocusNode.AddListener(HandleFocusChanged);
        _configuration.FocusNode.Attach(_configuration.FocusManager);
    }

    public TextEditingState EditingState => _configuration.Controller.Value;

    public bool RequestFocus()
    {
        var focused = _configuration.FocusManager.RequestFocus(_configuration.FocusNode);
        if (focused)
        {
            _configuration.Connection.SetClient(this, EditingState);
            UpdateCaretGeometry();
        }
        return focused;
    }

    public void Update(EditableText configuration)
    {
        if (!ReferenceEquals(_configuration.Controller, configuration.Controller))
        {
            _configuration.Controller.RemoveListener(HandleControllerChanged);
            configuration.Controller.AddListener(HandleControllerChanged);
        }
        if (!ReferenceEquals(_configuration.FocusNode, configuration.FocusNode) || !ReferenceEquals(_configuration.FocusManager, configuration.FocusManager))
        {
            _configuration.FocusNode.RemoveListener(HandleFocusChanged);
            _configuration.FocusNode.Detach(_configuration.FocusManager);
            configuration.FocusNode.Attach(configuration.FocusManager);
            configuration.FocusNode.AddListener(HandleFocusChanged);
        }
        _configuration = configuration;
        MarkNeedsLayout();
    }

    public void UpdateEditingState(TextEditingState state)
    {
        if (_disposed)
        {
            return;
        }
        var normalized = Normalize(state);
        _configuration.Controller.Value = normalized;
        _configuration.OnChanged?.Invoke(normalized.Text);
        _configuration.Connection.UpdateState(normalized);
    }

    public void PerformAction(TextInputAction action) => _configuration.OnSubmitted?.Invoke(action);

    public void HandlePointerEvent(PointerEvent input)
    {
        if (input.Phase is not PointerEventPhase.Down)
        {
            return;
        }
        RequestFocus();
        if (_paragraph is not null)
        {
            var position = _paragraph.GetPositionForOffset(input.LocalPosition);
            var state = EditingState with { Selection = new(position, position), ComposingRange = null };
            _configuration.Controller.Value = state;
            _configuration.Connection.UpdateState(state);
        }
    }

    public bool HandleKeyboardEvent(KeyboardEvent input)
    {
        if (!_configuration.FocusNode.HasFocus || input.Phase is not KeyboardEventPhase.Down)
        {
            return false;
        }
        var state = EditingState;
        var extent = Math.Clamp(state.Selection.ExtentOffset, 0, state.Text.Length);
        switch (input.LogicalKey)
        {
            case 0x08:
                if (extent > 0)
                {
                    var previous = PreviousTextElement(state.Text, extent);
                    ApplyLocalEdit(state.Text.Remove(previous, extent - previous), previous);
                }
                return true;
            case 0x25:
                ApplySelection(state, PreviousTextElement(state.Text, extent));
                return true;
            case 0x27:
                ApplySelection(state, NextTextElement(state.Text, extent));
                return true;
            case 0x0D:
                if (_configuration.MaxLines > 1 && _configuration.TextInputAction is TextInputAction.NewLine)
                {
                    ApplyLocalEdit(state.Text.Insert(extent, "\n"), extent + 1);
                }
                else
                {
                    PerformAction(_configuration.TextInputAction);
                }
                return true;
            default:
                return false;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        _disposed = true;
        _configuration.Controller.RemoveListener(HandleControllerChanged);
        _configuration.FocusNode.RemoveListener(HandleFocusChanged);
        _configuration.FocusNode.Detach(_configuration.FocusManager);
        _configuration.Connection.ClearClient();
    }

    public override void DescribeSemanticsConfiguration(SemanticsConfiguration configuration)
    {
        configuration.IsSemanticBoundary = true;
        configuration.Role = SemanticsRole.TextField;
        configuration.Value = EditingState.Text;
        configuration.State = SemanticsState.Enabled |
            (_configuration.FocusNode.HasFocus ? SemanticsState.Focused : SemanticsState.None) |
            (_configuration.MaxLines > 1 ? SemanticsState.Multiline : SemanticsState.None);
        configuration.On(SemanticsAction.Focus, () => RequestFocus());
    }

    protected override void PerformLayout()
    {
        var maxWidth = Constraints.HasBoundedWidth ? Constraints.MaxWidth : double.PositiveInfinity;
        _paragraph = _paragraphLayout.Layout(new(
            EditingState.Text,
            _configuration.FontSize,
            maxWidth,
            _configuration.MaxLines,
            _configuration.Color));
        var minimumHeight = _configuration.FontSize * 1.5;
        SetSize(Constraints.Constrain(new(_paragraph.Size.Width, Math.Max(minimumHeight, _paragraph.Size.Height))));
        UpdateCaretGeometry();
    }

    protected override bool HitTestSelf(Offset position) => true;

    protected override void Paint(PaintingContext context, Offset offset)
    {
        _paragraph?.Paint(context);
        var selection = EditingState.Selection;
        if (_configuration.FocusNode.HasFocus)
        {
            var caret = CaretRect(selection.ExtentOffset);
            context.DrawRect(caret, new(_configuration.CursorColor));
        }
        if (EditingState.ComposingRange is { } composing && composing.BaseOffset != composing.ExtentOffset)
        {
            var start = CaretRect(composing.BaseOffset);
            var end = CaretRect(composing.ExtentOffset);
            context.DrawRect(new(start.Left, start.Bottom - 1, Math.Max(start.Right + 1, end.Left), start.Bottom), new(_configuration.CursorColor));
        }
    }

    private void HandleControllerChanged()
    {
        MarkNeedsLayout();
        MarkNeedsPaint();
    }

    private void HandleFocusChanged()
    {
        if (_configuration.FocusNode.HasFocus)
        {
            _configuration.Connection.SetClient(this, EditingState);
            UpdateCaretGeometry();
        }
        else
        {
            _configuration.Connection.ClearClient();
        }
        MarkNeedsPaint();
    }

    private void ApplyLocalEdit(string text, int caret)
    {
        var next = new TextEditingState(text, new(caret, caret), null);
        _configuration.Controller.Value = next;
        _configuration.Connection.UpdateState(next);
        _configuration.OnChanged?.Invoke(text);
    }

    private void ApplySelection(TextEditingState state, int caret)
    {
        var next = state with { Selection = new(caret, caret), ComposingRange = null };
        _configuration.Controller.Value = next;
        _configuration.Connection.UpdateState(next);
    }

    private Rect CaretRect(int position)
    {
        if (_paragraph is null)
        {
            return Rect.FromLeftTopWidthHeight(0, 0, 1, _configuration.FontSize * 1.25);
        }
        var line = _paragraph.Lines.FirstOrDefault(item => position >= item.Start && position <= item.Start + item.Length)
            ?? _paragraph.Lines[^1];
        var prefix = EditingState.Text.Substring(line.Start, Math.Clamp(position - line.Start, 0, line.Length));
        var prefixLayout = _paragraphLayout.Layout(new(prefix, _configuration.FontSize, double.PositiveInfinity, 1, _configuration.Color));
        var top = line.Baseline - _configuration.FontSize;
        return Rect.FromLeftTopWidthHeight(prefixLayout.Size.Width, top, 1.5, line.Height);
    }

    private void UpdateCaretGeometry()
    {
        if (_configuration.Connection is ITextInputGeometry geometry && _configuration.FocusNode.HasFocus)
        {
            var local = CaretRect(EditingState.Selection.ExtentOffset);
            var global = LocalToGlobal(new(local.Left, local.Top));
            geometry.SetCaretRect(Rect.FromLeftTopWidthHeight(global.X, global.Y, local.Width, local.Height));
        }
    }

    private static TextEditingState Normalize(TextEditingState state)
    {
        ArgumentNullException.ThrowIfNull(state.Text);
        var selection = new TextSelection(
            Math.Clamp(state.Selection.BaseOffset, 0, state.Text.Length),
            Math.Clamp(state.Selection.ExtentOffset, 0, state.Text.Length));
        TextSelection? composing = state.ComposingRange is { } range
            ? new TextSelection(Math.Clamp(range.BaseOffset, 0, state.Text.Length), Math.Clamp(range.ExtentOffset, 0, state.Text.Length))
            : null;
        return new(state.Text, selection, composing);
    }

    private static int PreviousTextElement(string text, int offset)
    {
        if (offset <= 0)
        {
            return 0;
        }
        var starts = System.Globalization.StringInfo.ParseCombiningCharacters(text);
        return starts.LastOrDefault(value => value < offset);
    }

    private static int NextTextElement(string text, int offset)
    {
        var starts = System.Globalization.StringInfo.ParseCombiningCharacters(text);
        return starts.FirstOrDefault(value => value > offset, text.Length);
    }
}
