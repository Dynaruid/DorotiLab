using Doroti.Composition;
using Doroti.Graphics;
using Doroti.Rendering;

namespace Doroti.Widgets;

/// <summary>
/// A vertically scrollable linear list for a finite, explicitly-created set of children.
/// Mouse-wheel input changes the paint offset while layout keeps the full content extent.
/// </summary>
public sealed class ListView : MultiChildRenderObjectWidget
{
    public ListView(
        IEnumerable<Widget> children,
        EdgeInsets? padding = null,
        double itemSpacing = 0,
        bool showScrollbar = true,
        Key? key = null)
        : base(children, key)
    {
        ValidateNonNegativeFinite(itemSpacing, nameof(itemSpacing));
        var resolvedPadding = padding ?? EdgeInsets.Zero;
        if (!resolvedPadding.IsFiniteAndNonNegative)
        {
            throw new ArgumentException("List padding must be finite and non-negative.", nameof(padding));
        }

        Padding = resolvedPadding;
        ItemSpacing = itemSpacing;
        ShowScrollbar = showScrollbar;
    }

    public EdgeInsets Padding { get; }

    public double ItemSpacing { get; }

    public bool ShowScrollbar { get; }

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderListView(
        Padding,
        ItemSpacing,
        ShowScrollbar);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var list = (RenderListView)renderObject;
        list.Padding = Padding;
        list.ItemSpacing = ItemSpacing;
        list.ShowScrollbar = ShowScrollbar;
    }

    private static void ValidateNonNegativeFinite(double value, string name)
    {
        if (!double.IsFinite(value) || value < 0)
        {
            throw new ArgumentOutOfRangeException(name, "The value must be finite and non-negative.");
        }
    }

    private static void ValidatePositiveFinite(double value, string name)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            throw new ArgumentOutOfRangeException(name, "The value must be finite and positive.");
        }
    }
}

/// <summary>Render object used by <see cref="ListView"/>.</summary>
public sealed class RenderListView : ContainerRenderBox<BoxParentData>, IPointerEventTarget, IPointerSignalTarget, IFocusableKeyboardTarget
{
    private static readonly Color ScrollbarTrackColor = Color.FromArgb(55, 255, 255, 255);
    private static readonly Color ScrollbarThumbColor = Color.FromArgb(185, 115, 174, 255);
    private EdgeInsets _padding;
    private double _itemSpacing;
    private bool _showScrollbar;
    private double _scrollOffset;
    private double _maxScrollExtent;
    private double _contentExtent;

    public RenderListView(
        EdgeInsets? padding = null,
        double itemSpacing = 0,
        bool showScrollbar = true)
    {
        _padding = padding ?? EdgeInsets.Zero;
        if (!_padding.IsFiniteAndNonNegative)
        {
            throw new ArgumentException("List padding must be finite and non-negative.", nameof(padding));
        }
        ValidateNonNegativeFinite(itemSpacing, nameof(itemSpacing));
        _itemSpacing = itemSpacing;
        _showScrollbar = showScrollbar;
    }

    public EdgeInsets Padding
    {
        get => _padding;
        set
        {
            if (!value.IsFiniteAndNonNegative)
            {
                throw new ArgumentException("List padding must be finite and non-negative.", nameof(value));
            }
            if (_padding != value)
            {
                _padding = value;
                MarkNeedsLayout();
            }
        }
    }

    public double ItemSpacing
    {
        get => _itemSpacing;
        set
        {
            ValidateNonNegativeFinite(value, nameof(value));
            if (_itemSpacing != value)
            {
                _itemSpacing = value;
                MarkNeedsLayout();
            }
        }
    }

    public bool ShowScrollbar
    {
        get => _showScrollbar;
        set
        {
            if (_showScrollbar != value)
            {
                _showScrollbar = value;
                MarkNeedsPaint();
            }
        }
    }

    public double ScrollOffset => _scrollOffset;

    public double MaxScrollExtent => _maxScrollExtent;

    public double ContentExtent => _contentExtent;

    public void ScrollBy(double delta)
    {
        if (!double.IsFinite(delta))
        {
            throw new ArgumentOutOfRangeException(nameof(delta), "Scroll delta must be finite.");
        }
        ScrollTo(_scrollOffset + delta);
    }

    public void ScrollTo(double offset)
    {
        if (!double.IsFinite(offset))
        {
            throw new ArgumentOutOfRangeException(nameof(offset), "Scroll offset must be finite.");
        }
        var next = Math.Clamp(offset, 0, _maxScrollExtent);
        if (Math.Abs(next - _scrollOffset) < 0.001)
        {
            return;
        }
        _scrollOffset = next;
        ApplyChildOffsets();
        MarkNeedsPaint();
    }

    public void HandlePointerEvent(PointerEvent input)
    {
    }

    public void RegisterPointerSignal(PointerScrollEvent input, PointerSignalResolver resolver)
    {
        var target = Math.Clamp(_scrollOffset + input.ScrollDelta.Y, 0, _maxScrollExtent);
        if (Math.Abs(target - _scrollOffset) >= 0.01)
        {
            resolver.Register(input, resolved => ScrollBy(resolved.ScrollDelta.Y));
        }
    }

    public bool RequestFocus() => true;

    public bool HandleKeyboardEvent(KeyboardEvent input)
    {
        if (input.Phase is not (KeyboardEventPhase.Down or KeyboardEventPhase.Repeat))
        {
            return false;
        }
        switch (input.LogicalKey)
        {
            case 0x21:
                ScrollBy(-Size.Height);
                return true;
            case 0x22:
                ScrollBy(Size.Height);
                return true;
            case 0x23:
                ScrollTo(_maxScrollExtent);
                return true;
            case 0x24:
                ScrollTo(0);
                return true;
            default:
                return false;
        }
    }

    protected override void PerformLayout()
    {
        if (!Constraints.HasBoundedHeight)
        {
            throw new InvalidOperationException("ListView requires a bounded height.");
        }

        var boundedWidth = Constraints.HasBoundedWidth;
        var innerWidth = boundedWidth ? Math.Max(0, Constraints.MaxWidth - _padding.Horizontal) : double.PositiveInfinity;
        var childConstraints = boundedWidth
            ? BoxConstraints.TightFor(width: innerWidth)
            : new BoxConstraints();
        var contentHeight = _padding.Top + _padding.Bottom;
        var maxChildWidth = 0d;
        for (var index = 0; index < Children.Count; index++)
        {
            var child = Children[index];
            child.Layout(childConstraints, parentUsesSize: true);
            if (index > 0)
            {
                contentHeight += _itemSpacing;
            }
            contentHeight += child.Size.Height;
            maxChildWidth = Math.Max(maxChildWidth, child.Size.Width);
        }

        _contentExtent = contentHeight;
        var desiredWidth = boundedWidth ? Constraints.MaxWidth : maxChildWidth + _padding.Horizontal;
        SetSize(Constraints.Constrain(new(desiredWidth, Constraints.MaxHeight)));
        _maxScrollExtent = Math.Max(0, _contentExtent - Size.Height);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, _maxScrollExtent);
        ApplyChildOffsets();
    }

    protected override bool HitTestSelf(Offset position) => true;

    protected override void Paint(PaintingContext context, Offset offset)
    {
        var viewport = Rect.FromLeftTopWidthHeight(0, 0, Size.Width, Size.Height);
        context.PushClipRect(viewport, nested =>
        {
            foreach (var child in Children)
            {
                var childOffset = ((BoxParentData)child.ParentData!).Offset;
                if (childOffset.Y < Size.Height && childOffset.Y + child.Size.Height > 0)
                {
                    nested.PaintChild(child, childOffset);
                }
            }
        });

        if (!_showScrollbar || _maxScrollExtent <= 0 || Size.Height <= 0)
        {
            return;
        }

        const double width = 6;
        const double margin = 5;
        var trackHeight = Math.Max(0, Size.Height - (margin * 2));
        if (trackHeight <= 0 || Size.Width <= width + margin)
        {
            return;
        }
        var thumbHeight = Math.Clamp(trackHeight * Size.Height / _contentExtent, 28, trackHeight);
        var thumbTravel = trackHeight - thumbHeight;
        var thumbTop = margin + ((_scrollOffset / _maxScrollExtent) * thumbTravel);
        var left = Size.Width - margin - width;
        context.PushClipRect(viewport, nested =>
        {
            nested.DrawRect(Rect.FromLeftTopWidthHeight(left, margin, width, trackHeight), new RasterPaint(ScrollbarTrackColor));
            nested.DrawRect(Rect.FromLeftTopWidthHeight(left, thumbTop, width, thumbHeight), new RasterPaint(ScrollbarThumbColor));
        });
    }

    private void ApplyChildOffsets()
    {
        var cursor = _padding.Top - _scrollOffset;
        for (var index = 0; index < Children.Count; index++)
        {
            var child = Children[index];
            ((BoxParentData)child.ParentData!).Offset = new(_padding.Left, cursor);
            cursor += child.Size.Height;
            if (index < Children.Count - 1)
            {
                cursor += _itemSpacing;
            }
        }
    }

    private static void ValidateNonNegativeFinite(double value, string name)
    {
        if (!double.IsFinite(value) || value < 0)
        {
            throw new ArgumentOutOfRangeException(name, "The value must be finite and non-negative.");
        }
    }

    private static void ValidatePositiveFinite(double value, string name)
    {
        if (!double.IsFinite(value) || value <= 0)
        {
            throw new ArgumentOutOfRangeException(name, "The value must be finite and positive.");
        }
    }
}
