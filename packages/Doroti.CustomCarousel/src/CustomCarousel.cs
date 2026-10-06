// Adapted from flutter_custom_carousel, Copyright (c) 2024, gskinner.com, inc.
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;

namespace Doroti.CustomCarousel;

public delegate Widget EffectsBuilder(int index, double scrollRatio, Widget child);
public enum DepthOrder { forward, reverse, selectedInFront }

/// <summary>Scroll input selects items; effectsBuilder independently controls their presentation.</summary>
public sealed class CustomCarousel : StatefulWidget
{
    public EffectsBuilder effectsBuilder { get; }
    public IReadOnlyList<Widget> children { get; }
    public Axis scrollDirection { get; }
    public bool reverse { get; }
    public CustomCarouselScrollController? controller { get; }
    public ScrollPhysics? physics { get; }
    public ScrollBehavior? scrollBehavior { get; }
    public Alignment? alignment { get; }
    public bool loop { get; }
    public bool tapToSelect { get; }
    public double scrollSpeed { get; }
    public int? itemCountBefore { get; }
    public int? itemCountAfter { get; }
    public DepthOrder depthOrder { get; }
    public Action<int>? onSelectedItemChanged { get; }
    public Action<int?>? onSettledItemChanged { get; }
    public bool addRepaintBoundaries { get; }
    public bool addSemanticIndexes { get; }
    public bool excludeFromSemantics { get; }
    public string? restorationId { get; }
    public bool verticalWheelScroll { get; }

    public CustomCarousel(EffectsBuilder effectsBuilder, IReadOnlyList<Widget> children, Key? key = null,
        Axis scrollDirection = Axis.vertical, bool reverse = false, CustomCarouselScrollController? controller = null,
        ScrollPhysics? physics = null, ScrollBehavior? scrollBehavior = null, Alignment? alignment = null,
        bool loop = false, bool tapToSelect = true, double scrollSpeed = 1,
        int? itemCountBefore = null, int? itemCountAfter = null, DepthOrder depthOrder = DepthOrder.forward,
        Action<int>? onSelectedItemChanged = null, Action<int?>? onSettledItemChanged = null,
        bool addRepaintBoundaries = true, bool addSemanticIndexes = true, bool excludeFromSemantics = false,
        string? restorationId = null, bool verticalWheelScroll = true) : base(key: key)
    {
        ArgumentNullException.ThrowIfNull(effectsBuilder);
        ArgumentNullException.ThrowIfNull(children);
        if (children.Count == 0) throw new ArgumentException("Provide at least one carousel item.", nameof(children));
        if (itemCountBefore is < 0 || itemCountAfter is < 0 ||
            (long)(itemCountBefore ?? 0) + (itemCountAfter ?? 0) >= children.Count)
            throw new ArgumentOutOfRangeException(nameof(itemCountBefore), "Visible neighbors must total less than children.Count.");
        if (!double.IsFinite(scrollSpeed) || scrollSpeed <= 0) throw new ArgumentOutOfRangeException(nameof(scrollSpeed));
        this.effectsBuilder = effectsBuilder;
        this.children = children.ToArray();
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.controller = controller;
        this.physics = physics;
        this.scrollBehavior = scrollBehavior;
        this.alignment = alignment;
        this.loop = loop;
        this.tapToSelect = tapToSelect;
        this.scrollSpeed = scrollSpeed;
        this.itemCountBefore = itemCountBefore;
        this.itemCountAfter = itemCountAfter;
        this.depthOrder = depthOrder;
        this.onSelectedItemChanged = onSelectedItemChanged;
        this.onSettledItemChanged = onSettledItemChanged;
        this.addRepaintBoundaries = addRepaintBoundaries;
        this.addSemanticIndexes = addSemanticIndexes;
        this.excludeFromSemantics = excludeFromSemantics;
        this.restorationId = restorationId;
        this.verticalWheelScroll = verticalWheelScroll;
    }

    public override IState createState() => new CustomCarouselState();
}

internal sealed class CustomCarouselState : State<CustomCarousel>
{
    private CustomCarouselScrollController? _localController;
    private CustomCarouselScrollController scrollController => widget.controller ??
        (_localController ??= new CustomCarouselScrollController());
    private int _lastReportedItem;
    private bool _settled = true;

    public override void initState()
    {
        base.initState();
        _lastReportedItem = widget.loop
            ? CustomCarouselScrollPosition.Modulo(scrollController.initialItem, widget.children.Count)
            : Math.Clamp(scrollController.initialItem, 0, widget.children.Count - 1);
    }

    public override void didUpdateWidget(CustomCarousel oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (oldWidget.controller != widget.controller)
        {
            _localController?.dispose();
            _localController = null;
        }
    }

    public override void dispose()
    {
        _localController?.dispose();
        base.dispose();
    }

    private bool handleNotification(ScrollNotification notification)
    {
        if (notification.depth != 0 || notification.metrics is not FixedExtentMetrics metrics) return false;
        if (notification is ScrollStartNotification && _settled)
        {
            _settled = false;
            widget.onSettledItemChanged?.Invoke(null);
        }
        if (notification is ScrollUpdateNotification && metrics.itemIndex != _lastReportedItem)
        {
            _lastReportedItem = checked((int)metrics.itemIndex);
            widget.onSelectedItemChanged?.Invoke(_lastReportedItem);
        }
        if (notification is ScrollEndNotification && !_settled)
        {
            _settled = true;
            widget.onSettledItemChanged?.Invoke(checked((int)metrics.itemIndex));
        }
        return false;
    }

    public override Widget build(BuildContext context) => new LayoutBuilder(builder: (_, constraints) =>
    {
        var extent = widget.scrollDirection == Axis.horizontal ? constraints.maxWidth : constraints.maxHeight;
        if (!double.IsFinite(extent) || extent <= 0)
            throw new InvalidOperationException("CustomCarousel needs a finite, positive extent along its scroll axis.");
        var itemExtent = extent / widget.scrollSpeed;
        var direction = widget.scrollDirection == Axis.vertical
            ? (widget.reverse ? AxisDirection.up : AxisDirection.down)
            : ((Directionality.of(context) == TextDirection.rtl) ^ widget.reverse ? AxisDirection.left : AxisDirection.right);
        return new NotificationListener<ScrollNotification>(onNotification: handleNotification,
            child: new CustomCarouselScrollable(itemExtent, widget.children.Count, controller: scrollController,
                physics: widget.physics ?? new CustomCarouselScrollPhysics(), loop: widget.loop,
                axisDirection: direction, scrollBehavior: widget.scrollBehavior ?? ScrollConfiguration.of(context).copyWith(scrollbars: false),
                restorationId: widget.restorationId, excludeFromSemantics: widget.excludeFromSemantics,
                verticalWheelScroll: widget.verticalWheelScroll,
                viewportBuilder: (_, offset) => new LayoutBuilder(builder: (_, _) =>
                {
                    offset.applyViewportDimension(itemExtent);
                    var range = itemExtent * (widget.children.Count - 1);
                    var looping = widget.loop && widget.children.Count > 1;
                    offset.applyContentDimensions(looping ? double.NegativeInfinity : 0,
                        looping ? double.PositiveInfinity : range);
                    return new AnimatedBuilder(animation: scrollController, builder: (_, _) => buildContent(itemExtent));
                })));
    });

    private Widget buildContent(double itemExtent)
    {
        var remaining = widget.children.Count - 1;
        var before = widget.itemCountBefore ?? Math.Min(10, Math.Min(remaining / 2, remaining - (widget.itemCountAfter ?? 0)));
        var after = widget.itemCountAfter ?? Math.Min(10, Math.Min(remaining / 2, remaining - (widget.itemCountBefore ?? 0)));
        var position = scrollController.hasClients
            ? scrollController.position.normalizePixels(scrollController.position.pixels) / itemExtent
            : scrollController.initialItem;
        var index = checked((int)Math.Round(position, MidpointRounding.AwayFromZero));
        var delta = index - position;
        var begin = widget.loop ? index - before : Math.Max(0, index - before);
        var end = widget.loop ? index + after : Math.Min(remaining, index + after);
        var display = new List<Widget>();
        void Add(int virtualIndex, double ratio, bool below)
        {
            var i = widget.loop ? CustomCarouselScrollPosition.Modulo(virtualIndex, widget.children.Count) : virtualIndex;
            if (i < 0 || i >= widget.children.Count) return;
            Widget child = widget.children[i];
            if (widget.addRepaintBoundaries) child = new RepaintBoundary(child: child);
            if (widget.tapToSelect)
            {
                var selected = i == scrollController.selectedItem;
                // Keep the same ancestors as selection changes. Inserting or
                // removing these wrappers remounts the image and card state.
                child = new MouseRegion(cursor: selected ? MouseCursor.defer : SystemMouseCursors.click,
                    child: new GestureDetector(onTap: selected ? null : () => { _ = scrollController.animateToItem(i); },
                        child: new AbsorbPointer(absorbing: !selected, child: child)));
            }
            if (widget.addSemanticIndexes) child = new IndexedSemantics(index: i, child: child);
            child = widget.effectsBuilder(i, ratio, child);
            if (widget.alignment is not null) child = new Align(alignment: widget.alignment, child: child);
            child = new KeyedSubtree(key: new ValueKey<int>(i), child: child);
            if (below) display.Insert(0, child); else display.Add(child);
        }
        Add(index, delta / (delta > 0 ? after + 0.5 : before + 0.5), false);
        for (var distance = 1; distance <= Math.Max(index - begin, end - index); distance++)
        {
            if (index - distance >= begin) Add(index - distance, (delta - distance) / (before + 0.5), widget.depthOrder != DepthOrder.reverse);
            if (index + distance <= end) Add(index + distance, (delta + distance) / (after + 0.5), widget.depthOrder != DepthOrder.forward);
        }
        return new Stack(fit: StackFit.expand, children: display);
    }
}
