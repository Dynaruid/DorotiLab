using System.Globalization;
using System.Text;
using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Platform;

namespace Doroti.Rendering;

public abstract class RenderProxyBox : RenderBox
{
    private RenderBox? _child;

    protected RenderProxyBox(RenderBox? child = null)
    {
        if (child is not null)
        {
            _child = child;
            AdoptChild(child);
        }
    }

    public RenderBox? Child
    {
        get => _child;
        set
        {
            if (ReferenceEquals(_child, value))
            {
                return;
            }
            if (_child is not null)
            {
                DropChild(_child);
            }
            _child = value;
            if (value is not null)
            {
                AdoptChild(value);
            }
        }
    }

    protected internal override void VisitChildren(Action<RenderObject> visitor)
    {
        if (_child is not null)
        {
            visitor(_child);
        }
    }

    protected override bool HitTestChildren(HitTestResult result, Offset position) =>
        _child is not null && HitTestChild(result, _child, position);

    protected internal override void Paint(PaintingContext context, Offset offset)
    {
        if (_child is not null)
        {
            var childOffset = ((BoxParentData)_child.ParentData!).Offset;
            context.PaintChild(_child, childOffset);
        }
    }
}

public sealed class RenderView : RenderProxyBox
{
    private RenderViewConfiguration _configuration;

    public RenderView(RenderViewConfiguration configuration, RenderBox? child = null)
        : base(child)
    {
        _configuration = configuration.Validate();
    }

    public override bool IsRepaintBoundary => true;

    public RenderViewConfiguration Configuration
    {
        get => _configuration;
        set
        {
            value.Validate();
            if (_configuration == value)
            {
                return;
            }
            _configuration = value;
            MarkNeedsLayout();
        }
    }

    protected override void PerformLayout()
    {
        SetSize(Constraints.Constrain(_configuration.LogicalSize));
        if (Child is not null)
        {
            Child.Layout(BoxConstraints.Tight(Size));
            ((BoxParentData)Child.ParentData!).Offset = Offset.Zero;
        }
    }

    protected internal override void Paint(PaintingContext context, Offset offset)
    {
        context.DrawRect(Rect.FromLeftTopWidthHeight(0, 0, Size.Width, Size.Height), new(Color.Transparent));
        base.Paint(context, offset);
    }
}

public sealed class RenderColoredBox : RenderProxyBox
{
    private Color _color;

    public RenderColoredBox(Color color, RenderBox? child = null)
        : base(child)
    {
        _color = color;
    }

    public Color Color
    {
        get => _color;
        set
        {
            if (_color == value)
            {
                return;
            }
            _color = value;
            MarkNeedsPaint();
        }
    }

    protected override void PerformLayout()
    {
        if (Child is null)
        {
            var width = Constraints.HasBoundedWidth ? Constraints.MaxWidth : Constraints.MinWidth;
            var height = Constraints.HasBoundedHeight ? Constraints.MaxHeight : Constraints.MinHeight;
            SetSize(Constraints.Constrain(new(width, height)));
            return;
        }
        Child.Layout(Constraints, parentUsesSize: true);
        SetSize(Constraints.Constrain(Child.Size));
        ((BoxParentData)Child.ParentData!).Offset = Offset.Zero;
    }

    protected override bool HitTestSelf(Offset position) => true;

    protected internal override void Paint(PaintingContext context, Offset offset)
    {
        context.DrawRect(Rect.FromLeftTopWidthHeight(0, 0, Size.Width, Size.Height), new(_color));
        base.Paint(context, offset);
    }
}

public sealed class RenderRepaintBoundary : RenderProxyBox
{
    public RenderRepaintBoundary(RenderBox? child = null)
        : base(child)
    {
    }

    public override bool IsRepaintBoundary => true;

    protected override void PerformLayout()
    {
        if (Child is null)
        {
            SetSize(Constraints.Constrain(Size.Zero));
            return;
        }
        Child.Layout(Constraints, parentUsesSize: true);
        SetSize(Constraints.Constrain(Child.Size));
        ((BoxParentData)Child.ParentData!).Offset = Offset.Zero;
    }
}

public sealed class RenderPadding : RenderProxyBox
{
    private EdgeInsets _padding;

    public RenderPadding(EdgeInsets padding, RenderBox? child = null)
        : base(child)
    {
        if (!padding.IsFiniteAndNonNegative)
        {
            throw new ArgumentException("Padding must be finite and non-negative.", nameof(padding));
        }
        _padding = padding;
    }

    public EdgeInsets Padding
    {
        get => _padding;
        set
        {
            if (!value.IsFiniteAndNonNegative)
            {
                throw new ArgumentException("Padding must be finite and non-negative.", nameof(value));
            }
            if (_padding == value)
            {
                return;
            }
            _padding = value;
            MarkNeedsLayout();
        }
    }

    protected override void PerformLayout()
    {
        if (Child is null)
        {
            SetSize(Constraints.Constrain(new(_padding.Horizontal, _padding.Vertical)));
            return;
        }
        Child.Layout(Constraints.Deflate(_padding), parentUsesSize: true);
        SetSize(Constraints.Constrain(new(Child.Size.Width + _padding.Horizontal, Child.Size.Height + _padding.Vertical)));
        ((BoxParentData)Child.ParentData!).Offset = new(_padding.Left, _padding.Top);
    }
}

public readonly record struct Alignment(double X, double Y)
{
    public static Alignment Center { get; } = new(0, 0);

    public static Alignment TopLeft { get; } = new(-1, -1);

    public Offset AlongOffset(Size container, Size child) => new(
        ((container.Width - child.Width) / 2) * (X + 1),
        ((container.Height - child.Height) / 2) * (Y + 1));
}

public sealed class RenderPositionedBox : RenderProxyBox
{
    private Alignment _alignment;
    private double? _widthFactor;
    private double? _heightFactor;

    public RenderPositionedBox(
        Alignment? alignment = null,
        double? widthFactor = null,
        double? heightFactor = null,
        RenderBox? child = null)
        : base(child)
    {
        ValidateFactor(widthFactor, nameof(widthFactor));
        ValidateFactor(heightFactor, nameof(heightFactor));
        _alignment = alignment ?? Alignment.Center;
        _widthFactor = widthFactor;
        _heightFactor = heightFactor;
    }

    public Alignment Alignment
    {
        get => _alignment;
        set
        {
            if (_alignment != value)
            {
                _alignment = value;
                MarkNeedsLayout();
            }
        }
    }

    protected override void PerformLayout()
    {
        if (Child is null)
        {
            SetSize(Constraints.Constrain(Size.Zero));
            return;
        }
        Child.Layout(Constraints.Loosen(), parentUsesSize: true);
        var desired = new Size(
            _widthFactor is { } width ? Child.Size.Width * width : Constraints.HasBoundedWidth ? Constraints.MaxWidth : Child.Size.Width,
            _heightFactor is { } height ? Child.Size.Height * height : Constraints.HasBoundedHeight ? Constraints.MaxHeight : Child.Size.Height);
        SetSize(Constraints.Constrain(desired));
        ((BoxParentData)Child.ParentData!).Offset = _alignment.AlongOffset(Size, Child.Size);
    }

    private static void ValidateFactor(double? factor, string name)
    {
        if (factor is { } value && (!double.IsFinite(value) || value < 0))
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }
}

public interface IRenderObjectChildContainer
{
    IReadOnlyList<RenderObject> RenderChildren { get; }

    void SetChildren(IReadOnlyList<RenderObject> children);
}

public abstract class ContainerRenderBox<TParentData> : RenderBox, IRenderObjectChildContainer
    where TParentData : BoxParentData, new()
{
    private readonly List<RenderBox> _children = [];

    public IReadOnlyList<RenderBox> Children => _children;

    IReadOnlyList<RenderObject> IRenderObjectChildContainer.RenderChildren => _children;

    public void Add(RenderBox child) => Insert(child, _children.Count);

    public void Insert(RenderBox child, int index)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (index < 0 || index > _children.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        AdoptChild(child);
        _children.Insert(index, child);
    }

    public bool Remove(RenderBox child)
    {
        var index = _children.IndexOf(child);
        if (index < 0)
        {
            return false;
        }
        DropChild(child);
        _children.RemoveAt(index);
        return true;
    }

    public void Move(RenderBox child, int index)
    {
        ArgumentNullException.ThrowIfNull(child);
        var oldIndex = _children.IndexOf(child);
        if (oldIndex < 0)
        {
            throw new InvalidOperationException("The RenderBox is not a child of this container.");
        }
        if (index < 0 || index >= _children.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (oldIndex == index)
        {
            return;
        }
        _children.RemoveAt(oldIndex);
        _children.Insert(index, child);
        MarkNeedsLayout();
        MarkNeedsPaint();
    }

    void IRenderObjectChildContainer.SetChildren(IReadOnlyList<RenderObject> children)
    {
        ArgumentNullException.ThrowIfNull(children);
        var desired = children.Select(item => item as RenderBox
            ?? throw new InvalidOperationException($"{DebugName} accepts only RenderBox children.")).ToArray();
        if (desired.Distinct(ReferenceEqualityComparer.Instance).Count() != desired.Length)
        {
            throw new InvalidOperationException("A RenderObject child cannot appear more than once.");
        }
        foreach (var obsolete in _children.Where(item => !desired.Contains(item, ReferenceEqualityComparer.Instance)).ToArray())
        {
            Remove(obsolete);
        }
        for (var index = 0; index < desired.Length; index++)
        {
            var child = desired[index];
            var currentIndex = _children.IndexOf(child);
            if (currentIndex < 0)
            {
                Insert(child, index);
            }
            else if (currentIndex != index)
            {
                Move(child, index);
            }
        }
    }

    protected internal override void SetupParentData(RenderObject child)
    {
        child.ParentData ??= new TParentData();
        if (child.ParentData is not TParentData)
        {
            throw new InvalidOperationException($"{child.DebugName} requires {typeof(TParentData).Name}.");
        }
    }

    protected internal override void VisitChildren(Action<RenderObject> visitor)
    {
        foreach (var child in _children)
        {
            visitor(child);
        }
    }

    protected override bool HitTestChildren(HitTestResult result, Offset position)
    {
        for (var index = _children.Count - 1; index >= 0; index--)
        {
            if (HitTestChild(result, _children[index], position))
            {
                return true;
            }
        }
        return false;
    }

    protected internal override void Paint(PaintingContext context, Offset offset)
    {
        foreach (var child in _children)
        {
            context.PaintChild(child, ((BoxParentData)child.ParentData!).Offset);
        }
    }
}

public enum Axis
{
    Horizontal,
    Vertical,
}

public enum FlexFit
{
    Tight,
    Loose,
}

public enum MainAxisAlignment
{
    Start,
    Center,
    End,
    SpaceBetween,
}

public enum CrossAxisAlignment
{
    Start,
    Center,
    End,
    Stretch,
}

public sealed class FlexParentData : BoxParentData
{
    private int _flex;

    public int Flex
    {
        get => _flex;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);
            _flex = value;
        }
    }

    public FlexFit Fit { get; set; } = FlexFit.Loose;
}

public sealed class RenderFlex : ContainerRenderBox<FlexParentData>
{
    private Axis _direction;
    private MainAxisAlignment _mainAxisAlignment;
    private CrossAxisAlignment _crossAxisAlignment;

    public RenderFlex(
        Axis direction = Axis.Horizontal,
        MainAxisAlignment mainAxisAlignment = MainAxisAlignment.Start,
        CrossAxisAlignment crossAxisAlignment = CrossAxisAlignment.Center)
    {
        _direction = direction;
        _mainAxisAlignment = mainAxisAlignment;
        _crossAxisAlignment = crossAxisAlignment;
    }

    public Axis Direction
    {
        get => _direction;
        set
        {
            if (_direction != value)
            {
                _direction = value;
                MarkNeedsLayout();
            }
        }
    }

    public MainAxisAlignment MainAxisAlignment
    {
        get => _mainAxisAlignment;
        set
        {
            if (_mainAxisAlignment != value)
            {
                _mainAxisAlignment = value;
                MarkNeedsLayout();
            }
        }
    }

    public CrossAxisAlignment CrossAxisAlignment
    {
        get => _crossAxisAlignment;
        set
        {
            if (_crossAxisAlignment != value)
            {
                _crossAxisAlignment = value;
                MarkNeedsLayout();
            }
        }
    }

    public void Add(RenderBox child, int flex, FlexFit fit = FlexFit.Loose)
    {
        Add(child);
        var data = (FlexParentData)child.ParentData!;
        data.Flex = flex;
        data.Fit = fit;
        MarkNeedsLayout();
    }

    protected override void PerformLayout()
    {
        var horizontal = Direction is Axis.Horizontal;
        var maxMain = horizontal ? Constraints.MaxWidth : Constraints.MaxHeight;
        var maxCross = horizontal ? Constraints.MaxHeight : Constraints.MaxWidth;
        var boundedMain = double.IsFinite(maxMain);
        var usedMain = 0d;
        var maxChildCross = 0d;
        var flexTotal = Children.Sum(child => ((FlexParentData)child.ParentData!).Flex);

        foreach (var child in Children.Where(item => ((FlexParentData)item.ParentData!).Flex == 0))
        {
            child.Layout(CreateChildConstraints(horizontal, 0, double.PositiveInfinity, maxCross, stretch: CrossAxisAlignment is CrossAxisAlignment.Stretch), parentUsesSize: true);
            usedMain += MainSize(child.Size, horizontal);
            maxChildCross = Math.Max(maxChildCross, CrossSize(child.Size, horizontal));
        }

        if (!boundedMain && flexTotal > 0)
        {
            throw new InvalidOperationException("RenderFlex cannot lay out flex children along an unbounded main axis.");
        }
        var free = boundedMain ? Math.Max(0, maxMain - usedMain) : 0;
        foreach (var child in Children.Where(item => ((FlexParentData)item.ParentData!).Flex > 0))
        {
            var data = (FlexParentData)child.ParentData!;
            var extent = flexTotal == 0 ? 0 : free * data.Flex / flexTotal;
            var minimum = data.Fit is FlexFit.Tight ? extent : 0;
            child.Layout(CreateChildConstraints(horizontal, minimum, extent, maxCross, stretch: CrossAxisAlignment is CrossAxisAlignment.Stretch), parentUsesSize: true);
            usedMain += MainSize(child.Size, horizontal);
            maxChildCross = Math.Max(maxChildCross, CrossSize(child.Size, horizontal));
        }

        var desired = horizontal ? new Size(usedMain, maxChildCross) : new Size(maxChildCross, usedMain);
        SetSize(Constraints.Constrain(desired));
        var actualMain = MainSize(Size, horizontal);
        var extra = Math.Max(0, actualMain - usedMain);
        var gap = MainAxisAlignment is MainAxisAlignment.SpaceBetween && Children.Count > 1 ? extra / (Children.Count - 1) : 0;
        var cursor = MainAxisAlignment switch
        {
            MainAxisAlignment.Center => extra / 2,
            MainAxisAlignment.End => extra,
            _ => 0,
        };
        var actualCross = CrossSize(Size, horizontal);
        foreach (var child in Children)
        {
            var childCross = CrossSize(child.Size, horizontal);
            var cross = CrossAxisAlignment switch
            {
                CrossAxisAlignment.Center => (actualCross - childCross) / 2,
                CrossAxisAlignment.End => actualCross - childCross,
                _ => 0,
            };
            ((FlexParentData)child.ParentData!).Offset = horizontal ? new(cursor, cross) : new(cross, cursor);
            cursor += MainSize(child.Size, horizontal) + gap;
        }
    }

    private static BoxConstraints CreateChildConstraints(bool horizontal, double minMain, double maxMain, double maxCross, bool stretch)
    {
        var minCross = stretch && double.IsFinite(maxCross) ? maxCross : 0;
        return horizontal
            ? new(minMain, maxMain, minCross, maxCross)
            : new(minCross, maxCross, minMain, maxMain);
    }

    private static double MainSize(Size size, bool horizontal) => horizontal ? size.Width : size.Height;

    private static double CrossSize(Size size, bool horizontal) => horizontal ? size.Height : size.Width;
}

public sealed class StackParentData : BoxParentData
{
    public double? Left { get; set; }

    public double? Top { get; set; }

    public double? Right { get; set; }

    public double? Bottom { get; set; }

    public double? Width { get; set; }

    public double? Height { get; set; }

    public bool IsPositioned => Left is not null || Top is not null || Right is not null || Bottom is not null || Width is not null || Height is not null;
}

public sealed class RenderStack : ContainerRenderBox<StackParentData>
{
    private Alignment _alignment;
    private bool _clip;

    public RenderStack(Alignment? alignment = null, bool clip = true)
    {
        _alignment = alignment ?? Alignment.TopLeft;
        _clip = clip;
    }

    public Alignment Alignment
    {
        get => _alignment;
        set
        {
            if (_alignment != value)
            {
                _alignment = value;
                MarkNeedsLayout();
            }
        }
    }

    public bool Clip
    {
        get => _clip;
        set
        {
            if (_clip != value)
            {
                _clip = value;
                MarkNeedsPaint();
            }
        }
    }

    public void AddPositioned(
        RenderBox child,
        double? left = null,
        double? top = null,
        double? right = null,
        double? bottom = null,
        double? width = null,
        double? height = null)
    {
        Add(child);
        var data = (StackParentData)child.ParentData!;
        data.Left = left;
        data.Top = top;
        data.Right = right;
        data.Bottom = bottom;
        data.Width = width;
        data.Height = height;
        MarkNeedsLayout();
    }

    protected override void PerformLayout()
    {
        var width = Constraints.HasBoundedWidth ? Constraints.MaxWidth : Constraints.MinWidth;
        var height = Constraints.HasBoundedHeight ? Constraints.MaxHeight : Constraints.MinHeight;
        foreach (var child in Children.Where(item => !((StackParentData)item.ParentData!).IsPositioned))
        {
            child.Layout(Constraints.Loosen(), parentUsesSize: true);
            width = Math.Max(width, child.Size.Width);
            height = Math.Max(height, child.Size.Height);
        }
        SetSize(Constraints.Constrain(new(width, height)));

        foreach (var child in Children)
        {
            var data = (StackParentData)child.ParentData!;
            if (data.IsPositioned)
            {
                var childWidth = data.Width ?? (data.Left is not null && data.Right is not null ? Math.Max(0, Size.Width - data.Left.Value - data.Right.Value) : (double?)null);
                var childHeight = data.Height ?? (data.Top is not null && data.Bottom is not null ? Math.Max(0, Size.Height - data.Top.Value - data.Bottom.Value) : (double?)null);
                child.Layout(BoxConstraints.TightFor(childWidth, childHeight).Enforce(Constraints.Loosen()), parentUsesSize: true);
                data.Offset = new(
                    data.Left ?? (data.Right is { } right ? Size.Width - right - child.Size.Width : Alignment.AlongOffset(Size, child.Size).X),
                    data.Top ?? (data.Bottom is { } bottom ? Size.Height - bottom - child.Size.Height : Alignment.AlongOffset(Size, child.Size).Y));
            }
            else
            {
                data.Offset = Alignment.AlongOffset(Size, child.Size);
            }
        }
    }

    protected internal override void Paint(PaintingContext context, Offset offset)
    {
        if (Clip)
        {
            context.PushClipRect(Rect.FromLeftTopWidthHeight(0, 0, Size.Width, Size.Height), nested => base.Paint(nested, offset));
            return;
        }
        base.Paint(context, offset);
    }
}

public sealed record ParagraphLayoutRequest(string Text, double FontSize, double MaxWidth, int MaxLines, Color Color);

public sealed record ParagraphLineMetric(int Start, int Length, double Baseline, double Width, double Height);

public sealed class ImmutableParagraphSnapshot
{
    private readonly IReadOnlyList<ParagraphLineMetric> _lines;

    public ImmutableParagraphSnapshot(
        string text,
        double fontSize,
        Color color,
        Size size,
        IEnumerable<ParagraphLineMetric> lines)
    {
        Text = text ?? throw new ArgumentNullException(nameof(text));
        if (!double.IsFinite(fontSize) || fontSize <= 0 || !size.IsFinite)
        {
            throw new ArgumentOutOfRangeException(nameof(fontSize));
        }
        FontSize = fontSize;
        Color = color;
        Size = size;
        _lines = Array.AsReadOnly(lines.ToArray());
        if (_lines.Count == 0)
        {
            throw new ArgumentException("A paragraph snapshot needs at least one line.", nameof(lines));
        }
    }

    public string Text { get; }

    public double FontSize { get; }

    public Color Color { get; }

    public Size Size { get; }

    public IReadOnlyList<ParagraphLineMetric> Lines => _lines;

    public int GetPositionForOffset(Offset offset)
    {
        var lineHeight = _lines[0].Height;
        var lineIndex = Math.Clamp((int)Math.Floor(offset.Y / lineHeight), 0, _lines.Count - 1);
        var line = _lines[lineIndex];
        var characterWidth = FontSize * 0.6;
        return line.Start + Math.Clamp((int)Math.Round(offset.X / characterWidth), 0, line.Length);
    }

    public void Paint(PaintingContext context)
    {
        foreach (var line in _lines)
        {
            var text = Text.Substring(line.Start, line.Length);
            if (text.Length > 0)
            {
                context.DrawText(text, new(0, line.Baseline), FontSize, new(Color));
            }
        }
    }
}

public interface IParagraphLayout
{
    ImmutableParagraphSnapshot Layout(ParagraphLayoutRequest request);
}

public sealed class MonospaceParagraphLayout : IParagraphLayout
{
    public ImmutableParagraphSnapshot Layout(ParagraphLayoutRequest request)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Text);
        if (!double.IsFinite(request.FontSize) || request.FontSize <= 0 || request.MaxLines <= 0 || double.IsNaN(request.MaxWidth))
        {
            throw new ArgumentOutOfRangeException(nameof(request));
        }
        var characterWidth = request.FontSize * 0.6;
        var lineHeight = request.FontSize * 1.2;
        var maximumCharacters = double.IsFinite(request.MaxWidth)
            ? Math.Max(1, (int)Math.Floor(request.MaxWidth / characterWidth))
            : request.Text.Length;
        var metrics = new List<ParagraphLineMetric>();
        var start = 0;
        while (start < request.Text.Length && metrics.Count < request.MaxLines)
        {
            var length = Math.Min(maximumCharacters, request.Text.Length - start);
            metrics.Add(new(start, length, (metrics.Count * lineHeight) + request.FontSize, length * characterWidth, lineHeight));
            start += length;
        }
        var width = metrics.Max(item => item.Width);
        var size = new Size(width, metrics.Count * lineHeight);
        return new(request.Text, request.FontSize, request.Color, size, metrics);
    }
}

public sealed class UnicodeParagraphLayout : IParagraphLayout
{
    public ImmutableParagraphSnapshot Layout(ParagraphLayoutRequest request)
    {
        ArgumentNullException.ThrowIfNull(request.Text);
        if (!double.IsFinite(request.FontSize) || request.FontSize <= 0 || request.MaxLines <= 0 || double.IsNaN(request.MaxWidth))
        {
            throw new ArgumentOutOfRangeException(nameof(request));
        }

        var elements = EnumerateTextElements(request.Text).ToArray();
        var lineHeight = request.FontSize * 1.25;
        var maxWidth = double.IsFinite(request.MaxWidth) ? Math.Max(request.FontSize * 0.25, request.MaxWidth) : double.PositiveInfinity;
        var lines = new List<ParagraphLineMetric>();
        var lineStart = 0;
        var lineLength = 0;
        var lineWidth = 0d;
        var maximumLineWidth = 0d;

        foreach (var element in elements)
        {
            var width = MeasureElement(element.Text, request.FontSize);
            var explicitBreak = element.Text is "\n" or "\r\n";
            if (explicitBreak || lineLength > 0 && lineWidth + width > maxWidth)
            {
                lines.Add(new(lineStart, lineLength, (lines.Count * lineHeight) + request.FontSize, lineWidth, lineHeight));
                maximumLineWidth = Math.Max(maximumLineWidth, lineWidth);
                if (lines.Count >= request.MaxLines)
                {
                    break;
                }
                lineStart = explicitBreak ? element.Index + element.Text.Length : element.Index;
                lineLength = 0;
                lineWidth = 0;
                if (explicitBreak)
                {
                    continue;
                }
            }
            lineLength += element.Text.Length;
            lineWidth += width;
        }

        if (lines.Count < request.MaxLines && (lineLength > 0 || lines.Count == 0))
        {
            lines.Add(new(lineStart, lineLength, (lines.Count * lineHeight) + request.FontSize, lineWidth, lineHeight));
            maximumLineWidth = Math.Max(maximumLineWidth, lineWidth);
        }
        var widthResult = double.IsFinite(request.MaxWidth) ? Math.Min(maximumLineWidth, request.MaxWidth) : maximumLineWidth;
        return new(request.Text, request.FontSize, request.Color, new(widthResult, lines.Count * lineHeight), lines);
    }

    private static IEnumerable<(int Index, string Text)> EnumerateTextElements(string text)
    {
        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            yield return (enumerator.ElementIndex, enumerator.GetTextElement());
        }
    }

    private static double MeasureElement(string element, double fontSize)
    {
        if (element is "\n" or "\r\n")
        {
            return 0;
        }
        var rune = element.EnumerateRunes().FirstOrDefault();
        if (Rune.IsWhiteSpace(rune))
        {
            return fontSize * 0.35;
        }
        var category = Rune.GetUnicodeCategory(rune);
        return category is UnicodeCategory.OtherLetter or UnicodeCategory.OtherSymbol || rune.Value >= 0x1100
            ? fontSize
            : fontSize * 0.58;
    }
}

public sealed class RenderParagraph : RenderBox
{
    private readonly IParagraphLayout _paragraphLayout;
    private string _text;
    private double _fontSize;
    private Color _color;
    private ImmutableParagraphSnapshot? _snapshot;

    public RenderParagraph(string text, IParagraphLayout paragraphLayout, double fontSize = 14, Color? color = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        _text = text;
        _paragraphLayout = paragraphLayout ?? throw new ArgumentNullException(nameof(paragraphLayout));
        _fontSize = fontSize;
        _color = color ?? Color.FromArgb(255, 0, 0, 0);
    }

    public ImmutableParagraphSnapshot Snapshot => _snapshot
        ?? throw new InvalidOperationException("Paragraph has no immutable snapshot before layout.");

    public string Text
    {
        get => _text;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (_text != value)
            {
                _text = value;
                MarkNeedsLayout();
            }
        }
    }

    public double FontSize
    {
        get => _fontSize;
        set
        {
            if (!double.IsFinite(value) || value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            if (_fontSize != value)
            {
                _fontSize = value;
                MarkNeedsLayout();
            }
        }
    }

    public Color Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                MarkNeedsPaint();
            }
        }
    }

    public int GetPositionForOffset(Offset offset) => Snapshot.GetPositionForOffset(offset);

    public override void DescribeSemanticsConfiguration(SemanticsConfiguration configuration)
    {
        configuration.Role = SemanticsRole.Text;
        configuration.Value = _text;
        configuration.State = SemanticsState.Enabled | SemanticsState.ReadOnly;
    }

    protected override void PerformLayout()
    {
        _snapshot = _paragraphLayout.Layout(new(_text, _fontSize, Constraints.MaxWidth, int.MaxValue, _color));
        SetSize(Constraints.Constrain(_snapshot.Size));
    }

    protected override bool HitTestSelf(Offset position) => true;

    protected internal override void Paint(PaintingContext context, Offset offset) => Snapshot.Paint(context);
}
