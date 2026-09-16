// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/carousel.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public class CarouselView : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Painting.EdgeInsets? padding { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual double? elevation { get; private set; }
    public virtual global::Doroti.Framework.Painting.ShapeBorder? shape { get; private set; }
    public virtual Clip? itemClipBehavior { get; private set; }
    public virtual global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor { get; private set; }
    public virtual double shrinkExtent { get; private set; } = default!;
    public virtual bool itemSnapping { get; private set; } = default!;
    public virtual CarouselController? controller { get; private set; }
    public virtual global::Doroti.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual bool consumeMaxWeight { get; private set; } = default!;
    public virtual global::System.Action<long>? onTap { get; private set; }
    public virtual bool enableSplash { get; private set; } = default!;
    public virtual double? itemExtent { get; private set; }
    public virtual List<long>? flexWeights { get; private set; }
    public virtual List<global::Doroti.Framework.Widgets.Widget> children { get; private set; } = default!;
    public virtual global::System.Action<long>? onIndexChanged { get; private set; }
    public virtual global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>? itemBuilder { get; private set; }
    public virtual long? itemCount { get; private set; }
    public virtual bool infinite { get; private set; } = default!;

    public CarouselView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? itemClipBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, bool itemSnapping = false, double shrinkExtent = 0.0, CarouselController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.horizontal, bool reverse = false, global::System.Action<long>? onTap = null, bool enableSplash = true, bool infinite = false, double itemExtent = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::System.Action<long>? onIndexChanged = null) : base(key: key)
    {
        this.padding = padding;
        this.backgroundColor = backgroundColor;
        this.elevation = elevation;
        this.shape = shape;
        this.itemClipBehavior = itemClipBehavior;
        this.overlayColor = overlayColor;
        this.itemSnapping = itemSnapping;
        this.shrinkExtent = shrinkExtent;
        this.controller = controller;
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.onTap = onTap;
        this.enableSplash = enableSplash;
        this.infinite = infinite;
        this.itemExtent = itemExtent;
        this.children = children;
        this.onIndexChanged = onIndexChanged;
        consumeMaxWeight = true;
        flexWeights = null;
        itemBuilder = null;
        itemCount = null;
    }

    public static CarouselView CreateWeighted(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? itemClipBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, bool itemSnapping = false, double shrinkExtent = 0.0, CarouselController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.horizontal, bool reverse = false, bool consumeMaxWeight = true, global::System.Action<long>? onTap = null, bool enableSplash = true, bool infinite = false, List<long> flexWeights = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::System.Action<long>? onIndexChanged = null)
    {
        var __instance = new CarouselView(key: key, padding: padding, backgroundColor: backgroundColor, elevation: elevation, shape: shape, itemClipBehavior: itemClipBehavior, overlayColor: overlayColor, itemSnapping: itemSnapping, shrinkExtent: shrinkExtent, controller: controller, scrollDirection: scrollDirection, reverse: reverse, onTap: onTap, enableSplash: enableSplash, infinite: infinite, itemExtent: default!, children: children, onIndexChanged: onIndexChanged);
        __instance.padding = padding;
        __instance.backgroundColor = backgroundColor;
        __instance.elevation = elevation;
        __instance.shape = shape;
        __instance.itemClipBehavior = itemClipBehavior;
        __instance.overlayColor = overlayColor;
        __instance.itemSnapping = itemSnapping;
        __instance.shrinkExtent = shrinkExtent;
        __instance.controller = controller;
        __instance.scrollDirection = scrollDirection;
        __instance.reverse = reverse;
        __instance.consumeMaxWeight = consumeMaxWeight;
        __instance.onTap = onTap;
        __instance.enableSplash = enableSplash;
        __instance.infinite = infinite;
        __instance.flexWeights = flexWeights;
        __instance.children = children;
        __instance.onIndexChanged = onIndexChanged;
        __instance.itemExtent = null;
        __instance.itemBuilder = null;
        __instance.itemCount = null;
        return __instance;
    }

    public static CarouselView CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? itemClipBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, bool itemSnapping = false, double shrinkExtent = 0.0, CarouselController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.horizontal, bool reverse = false, global::System.Action<long>? onTap = null, bool enableSplash = true, double itemExtent = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>? itemBuilder = default!, long? itemCount = null, global::System.Action<long>? onIndexChanged = null, bool infinite = false)
    {
        var __instance = new CarouselView(key: key, padding: padding, backgroundColor: backgroundColor, elevation: elevation, shape: shape, itemClipBehavior: itemClipBehavior, overlayColor: overlayColor, itemSnapping: itemSnapping, shrinkExtent: shrinkExtent, controller: controller, scrollDirection: scrollDirection, reverse: reverse, onTap: onTap, enableSplash: enableSplash, infinite: infinite, itemExtent: itemExtent, children: default!, onIndexChanged: onIndexChanged);
        __instance.padding = padding;
        __instance.backgroundColor = backgroundColor;
        __instance.elevation = elevation;
        __instance.shape = shape;
        __instance.itemClipBehavior = itemClipBehavior;
        __instance.overlayColor = overlayColor;
        __instance.itemSnapping = itemSnapping;
        __instance.shrinkExtent = shrinkExtent;
        __instance.controller = controller;
        __instance.scrollDirection = scrollDirection;
        __instance.reverse = reverse;
        __instance.onTap = onTap;
        __instance.enableSplash = enableSplash;
        __instance.itemExtent = itemExtent;
        __instance.itemBuilder = itemBuilder;
        __instance.itemCount = itemCount;
        __instance.onIndexChanged = onIndexChanged;
        __instance.infinite = infinite;
        __instance.consumeMaxWeight = true;
        __instance.flexWeights = null;
        __instance.children = new List<global::Doroti.Framework.Widgets.Widget>();
        return __instance;
    }

    public static CarouselView CreateWeightedBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? itemClipBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, bool itemSnapping = false, double shrinkExtent = 0.0, CarouselController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = Axis.horizontal, bool reverse = false, bool consumeMaxWeight = true, global::System.Action<long>? onTap = null, bool enableSplash = true, List<long> flexWeights = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>? itemBuilder = default!, long? itemCount = null, global::System.Action<long>? onIndexChanged = null, bool infinite = false)
    {
        var __instance = new CarouselView(key: key, padding: padding, backgroundColor: backgroundColor, elevation: elevation, shape: shape, itemClipBehavior: itemClipBehavior, overlayColor: overlayColor, itemSnapping: itemSnapping, shrinkExtent: shrinkExtent, controller: controller, scrollDirection: scrollDirection, reverse: reverse, onTap: onTap, enableSplash: enableSplash, infinite: infinite, itemExtent: default!, children: default!, onIndexChanged: onIndexChanged);
        __instance.padding = padding;
        __instance.backgroundColor = backgroundColor;
        __instance.elevation = elevation;
        __instance.shape = shape;
        __instance.itemClipBehavior = itemClipBehavior;
        __instance.overlayColor = overlayColor;
        __instance.itemSnapping = itemSnapping;
        __instance.shrinkExtent = shrinkExtent;
        __instance.controller = controller;
        __instance.scrollDirection = scrollDirection;
        __instance.reverse = reverse;
        __instance.consumeMaxWeight = consumeMaxWeight;
        __instance.onTap = onTap;
        __instance.enableSplash = enableSplash;
        __instance.flexWeights = flexWeights;
        __instance.itemBuilder = itemBuilder;
        __instance.itemCount = itemCount;
        __instance.onIndexChanged = onIndexChanged;
        __instance.infinite = infinite;
        __instance.itemExtent = null;
        __instance.children = new List<global::Doroti.Framework.Widgets.Widget>();
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CarouselViewState__carousel());
}

internal class _CarouselViewState__carousel : global::Doroti.Framework.Widgets.State<CarouselView>
{
    internal virtual double? _itemExtent { get; set; } = default;
    internal virtual CarouselController? _internalController { get; set; } = default;
    internal virtual long _lastReportedLeadingItem { get; set; } = default!;

    internal virtual List<long>? _flexWeights => widget.flexWeights;
    internal virtual bool _consumeMaxWeight => widget.consumeMaxWeight;
    internal virtual CarouselController _controller => DartRuntimePrimitives.ConvertValue<CarouselController>(widget.controller ?? _internalController!);
    public override void initState()
    {
        base.initState();
        _itemExtent = widget.itemExtent;
        if (widget.controller is null)
        {
            _internalController = new CarouselController();
        }
        _lastReportedLeadingItem = _getInitialLeadingItem();
        _controller._attach(this);
        _controller.addListener(_handleScroll);
    }

    public override void didUpdateWidget(CarouselView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.controller, oldWidget.controller))
        {
            oldWidget.controller?._detach(this);
            if (widget.controller is not null)
            {
                _internalController?._detach(this);
                _internalController = null;
                widget.controller?._attach(this);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => _internalController is null);
                _internalController = new CarouselController();
                _controller._attach(this);
            }
        }
        if (!Equals(widget.flexWeights, oldWidget.flexWeights))
        {
            ((_CarouselPosition__carousel?)_controller.position)!.flexWeights = _flexWeights;
        }
        if (widget.itemExtent != oldWidget.itemExtent)
        {
            _itemExtent = widget.itemExtent;
            ((_CarouselPosition__carousel?)_controller.position)!.itemExtent = _itemExtent;
        }
        if (widget.consumeMaxWeight != oldWidget.consumeMaxWeight)
        {
            ((_CarouselPosition__carousel?)_controller.position)!.consumeMaxWeight = _consumeMaxWeight;
        }
    }

    public override void dispose()
    {
        _controller.removeListener(_handleScroll);
        _controller._detach(this);
        _internalController?.dispose();
        base.dispose();
    }

    internal virtual void _handleScroll()
    {
        if (widget.onIndexChanged is null)
        {
            return;
        }
        global::Doroti.Framework.Widgets.ScrollPosition positionLocal = _controller.position;
        long currentLeadingIndex = ((_CarouselPosition__carousel?)positionLocal)!.leadingItem;
        if (currentLeadingIndex != _lastReportedLeadingItem)
        {
            _lastReportedLeadingItem = currentLeadingIndex;
            widget.onIndexChanged!(currentLeadingIndex);
        }
    }

    internal virtual long _getInitialLeadingItem()
    {
        if (widget.flexWeights is not null)
        {
            long maxWeight = widget.flexWeights!.max();
            long firstMaxWeightIndex = widget.flexWeights!.IndexOf(maxWeight);
            return Math.Max(_controller.initialItem - firstMaxWeightIndex, 0L);
        }
        return _controller.initialItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCarouselItem(long index)
    {
        if (widget.infinite && Enumerable.Any(widget.children))
        {
            index = index % checked(widget.children.Count);
        }
        CarouselViewThemeData carouselTheme = CarouselViewTheme.of(context);
        ColorScheme colorScheme = ColorScheme.of(context);
        global::Doroti.Framework.Painting.EdgeInsets effectivePadding = (widget.padding ?? carouselTheme.padding) ?? EdgeInsets.CreateAll(4.0);
        global::Doroti.Ui.Color effectiveBackgroundColor = (widget.backgroundColor ?? carouselTheme.backgroundColor) ?? colorScheme.surface;
        double effectiveElevation = (widget.elevation ?? carouselTheme.elevation) ?? 0.0;
        global::Doroti.Framework.Painting.ShapeBorder effectiveShape = (widget.shape ?? carouselTheme.shape) ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: BorderRadius.CreateAll(Radius.circular(28.0)));
        global::Doroti.Ui.Clip effectiveItemClipBehavior = (widget.itemClipBehavior ?? carouselTheme.itemClipBehavior) ?? Clip.antiAlias;
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> effectiveOverlayColor = (widget.overlayColor ?? carouselTheme.overlayColor) ?? WidgetStateProperty.resolveWith((states) =>
        {
            if (states.Contains(WidgetState.pressed))
            {
                return (Color?)colorScheme.onSurface.withOpacity(0.1);
            }
            if (states.Contains(WidgetState.hovered))
            {
                return (Color?)colorScheme.onSurface.withOpacity(0.08);
            }
            if (states.Contains(WidgetState.focused))
            {
                return (Color?)colorScheme.onSurface.withOpacity(0.1);
            }
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        global::Doroti.Framework.Widgets.Widget contents = widget.children[(int)index];
        if (widget.enableSplash)
        {
            contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Stack(fit: StackFit.expand, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(contents), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(color: Colors.transparent, child: new InkWell(onTap: () => { widget.onTap?.Invoke(index); }, overlayColor: effectiveOverlayColor))) }));
        }
        else
        {
            if (widget.onTap is not null)
            {
                contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.GestureDetector(onTap: () => { widget.onTap!(index); }, child: contents));
            }
        }
        return new global::Doroti.Framework.Widgets.Padding(padding: effectivePadding, child: new Material(clipBehavior: effectiveItemClipBehavior, color: effectiveBackgroundColor, elevation: effectiveElevation, shape: effectiveShape, child: contents));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSliverCarousel(ThemeData theme)
    {
        long? childCountLocal = widget.infinite ? null : ((widget.itemBuilder is not null) ? widget.itemCount : checked(widget.children.Count));
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?> effectiveBuilder = default!;
        if (widget.itemBuilder is not null)
        {
            if (widget.infinite && (widget.itemCount is not null) && (DartRuntimePrimitives.RequireValue(widget.itemCount) > 0L))
            {
                long itemCountLocal = DartRuntimePrimitives.RequireValue(widget.itemCount);
                effectiveBuilder = (context, index) =>
                {
                    return widget.itemBuilder!(context, index % itemCountLocal);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                };
            }
            else
            {
                effectiveBuilder = widget.itemBuilder!;
            }
        }
        else
        {
            effectiveBuilder = (context, index) => _buildCarouselItem(index);
        }
        if (_itemExtent is not null)
        {
            return new _SliverFixedExtentCarousel__carousel(itemExtent: DartRuntimePrimitives.RequireValue(_itemExtent), minExtent: widget.shrinkExtent, infinite: widget.infinite, @delegate: new global::Doroti.Framework.Widgets.SliverChildBuilderDelegate(effectiveBuilder, childCount: childCountLocal));
        }
        DartRuntimePrimitives.Assert(() => (_flexWeights is not null) && _flexWeights!.All((weight) => weight > 0L), () => (object?)"flexWeights is null or it contains non-positive integers");
        return new _SliverWeightedCarousel__carousel(consumeMaxWeight: _consumeMaxWeight, shrinkExtent: widget.shrinkExtent, weights: _flexWeights!, infinite: widget.infinite, @delegate: new global::Doroti.Framework.Widgets.SliverChildBuilderDelegate(effectiveBuilder, childCount: childCountLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        global::Doroti.Framework.Widgets.ScrollPhysics physicsLocal = widget.itemSnapping ? new CarouselScrollPhysics() : ScrollConfiguration.of(context).getScrollPhysics(context);
        return new global::Doroti.Framework.Widgets.LayoutBuilder(builder: (context, constraints) =>
        {
            double mainAxisExtent = widget.scrollDirection switch { Axis.horizontal => constraints.maxWidth, Axis.vertical => constraints.maxHeight, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            _itemExtent = (widget.itemExtent is null) ? null : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(widget.itemExtent), 0, mainAxisExtent);
            return new global::Doroti.Framework.Widgets.CustomScrollView(scrollDirection: widget.scrollDirection, reverse: widget.reverse, controller: _controller, physics: physicsLocal, clipBehavior: Clip.antiAlias, scrollCacheExtent: ScrollCacheExtent.CreateViewport(0.0), slivers: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildSliverCarousel(theme)) });
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _SliverFixedExtentCarousel__carousel : global::Doroti.Framework.Widgets.SliverMultiBoxAdaptorWidget
{
    public virtual double itemExtent { get; private set; } = default!;
    public virtual double minExtent { get; private set; } = default!;
    public virtual bool infinite { get; private set; } = default!;

    internal _SliverFixedExtentCarousel__carousel(global::Doroti.Framework.Widgets.SliverChildDelegate @delegate, double minExtent, double itemExtent, bool infinite) : base(@delegate: @delegate)
    {
        this.minExtent = minExtent;
        this.itemExtent = itemExtent;
        this.infinite = infinite;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var element = ((global::Doroti.Framework.Widgets.SliverMultiBoxAdaptorElement?)context)!;
        return new _RenderSliverFixedExtentCarousel__carousel(childManager: element, minExtent: minExtent, maxExtent: itemExtent, infinite: infinite);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFixedExtentCarousel__carousel)renderObject;
        __renderObject.maxExtent = itemExtent;
        __renderObject.minExtent = minExtent;
        __renderObject.infinite = infinite;
    }

}

public class _RenderSliverFixedExtentCarousel__carousel : global::Doroti.Framework.Rendering.RenderSliverFixedExtentBoxAdaptor
{
    internal virtual double _maxExtent { get; set; } = default!;
    internal virtual double _minExtent { get; set; } = default!;
    internal virtual bool _infinite { get; set; } = default!;

    internal _RenderSliverFixedExtentCarousel__carousel(global::Doroti.Framework.Rendering.RenderSliverBoxChildManager childManager, double maxExtent, double minExtent, bool infinite) : base(childManager: childManager)
    {
        _maxExtent = maxExtent;
        _minExtent = minExtent;
        _infinite = infinite;
    }

    public virtual double maxExtent
    {
        get => _maxExtent;
        set
        {
            var __value = value;
            if (_maxExtent == __value)
            {
                return;
            }
            _maxExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual double minExtent
    {
        get => _minExtent;
        set
        {
            var __value = value;
            if (_minExtent == __value)
            {
                return;
            }
            _minExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual bool infinite
    {
        get => _infinite;
        set
        {
            var __value = value;
            if (_infinite == __value)
            {
                return;
            }
            _infinite = __value;
            markNeedsLayout();
        }
    }
    internal virtual double _buildItemExtent(long index, global::Doroti.Framework.Rendering.SliverLayoutDimensions currentLayoutDimensions)
    {
        if (maxExtent == 0.0)
        {
            return maxExtent;
        }
        long firstVisibleIndex = (constraints.scrollOffset / maxExtent).floor();
        long offscreenItems = (constraints.scrollOffset / maxExtent).floor();
        double offscreenExtent = constraints.scrollOffset - (offscreenItems * maxExtent);
        double effectiveMinExtent = Math.Max(constraints.remainingPaintExtent % maxExtent, minExtent);
        if (index == firstVisibleIndex)
        {
            double effectiveExtent = maxExtent - offscreenExtent;
            return Math.Max(effectiveExtent, effectiveMinExtent);
        }
        double scrollOffsetForLastIndex = constraints.scrollOffset + constraints.remainingPaintExtent;
        if (index == getMaxChildIndexForScrollOffset(scrollOffsetForLastIndex, maxExtent))
        {
            return Dart_uiLibrary.clampDouble(scrollOffsetForLastIndex - (maxExtent * index), effectiveMinExtent, maxExtent);
        }
        return maxExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double indexToLayoutOffset(double itemExtent, long index)
    {
        if (maxExtent == 0.0)
        {
            return maxExtent;
        }
        long firstVisibleIndex = (constraints.scrollOffset / maxExtent).floor();
        double effectiveMinExtent = Math.Max(constraints.remainingPaintExtent % maxExtent, minExtent);
        if (index == firstVisibleIndex)
        {
            double firstVisibleItemExtent = _buildItemExtent(index, layoutDimensions);
            if (firstVisibleItemExtent <= effectiveMinExtent)
            {
                return (maxExtent * index) - effectiveMinExtent + maxExtent;
            }
            return constraints.scrollOffset;
        }
        return maxExtent * index;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMinChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if (maxExtent == 0.0)
        {
            return 0L;
        }
        long firstVisibleIndex = (scrollOffset / maxExtent).floor();
        return Math.Max(firstVisibleIndex, 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if (maxExtent > 0.0)
        {
            double actual = (scrollOffset / maxExtent) - 1L;
            long roundLocal = actual.round();
            if (((actual * maxExtent) - (roundLocal * maxExtent)).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance)
            {
                return Math.Max(0L, roundLocal);
            }
            return Math.Max(0L, actual.ceil());
        }
        return 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? itemExtent => null;
    public override ItemExtentBuilder? itemExtentBuilder => (index, dimensions) => _buildItemExtent(index, dimensions);
}

internal class _SliverWeightedCarousel__carousel : global::Doroti.Framework.Widgets.SliverMultiBoxAdaptorWidget
{
    public virtual bool consumeMaxWeight { get; private set; } = default!;
    public virtual double shrinkExtent { get; private set; } = default!;
    public virtual List<long> weights { get; private set; } = default!;
    public virtual bool infinite { get; private set; } = default!;

    internal _SliverWeightedCarousel__carousel(global::Doroti.Framework.Widgets.SliverChildDelegate @delegate, bool consumeMaxWeight, double shrinkExtent, List<long> weights, bool infinite) : base(@delegate: @delegate)
    {
        this.consumeMaxWeight = consumeMaxWeight;
        this.shrinkExtent = shrinkExtent;
        this.weights = weights;
        this.infinite = infinite;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var element = ((global::Doroti.Framework.Widgets.SliverMultiBoxAdaptorElement?)context)!;
        return new _RenderSliverWeightedCarousel__carousel(childManager: element, consumeMaxWeight: consumeMaxWeight, shrinkExtent: shrinkExtent, weights: weights, infinite: infinite);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverWeightedCarousel__carousel)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSliverWeightedCarousel__carousel>)(() =>
{
    var __cascade = __renderObject;
    __cascade.consumeMaxWeight = consumeMaxWeight;
    __cascade.shrinkExtent = shrinkExtent;
    __cascade.weights = weights;
    __cascade.infinite = infinite;
    return __cascade;
}))());
    }

}

public class _RenderSliverWeightedCarousel__carousel : global::Doroti.Framework.Rendering.RenderSliverFixedExtentBoxAdaptor
{
    internal virtual bool _consumeMaxWeight { get; set; } = default!;
    internal virtual double _shrinkExtent { get; set; } = default!;
    internal virtual List<long> _weights { get; set; } = default!;
    internal virtual bool _infinite { get; set; } = default!;

    internal _RenderSliverWeightedCarousel__carousel(global::Doroti.Framework.Rendering.RenderSliverBoxChildManager childManager, bool consumeMaxWeight, double shrinkExtent, List<long> weights, bool infinite) : base(childManager: childManager)
    {
        _consumeMaxWeight = consumeMaxWeight;
        _shrinkExtent = shrinkExtent;
        _weights = weights;
        _infinite = infinite;
    }

    public virtual bool consumeMaxWeight
    {
        get => _consumeMaxWeight;
        set
        {
            var __value = value;
            if (_consumeMaxWeight == __value)
            {
                return;
            }
            _consumeMaxWeight = __value;
            markNeedsLayout();
        }
    }
    public virtual double shrinkExtent
    {
        get => _shrinkExtent;
        set
        {
            var __value = value;
            if (_shrinkExtent == __value)
            {
                return;
            }
            _shrinkExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual List<long> weights
    {
        get => _weights;
        set
        {
            var __value = value;
            if (Equals(_weights, __value))
            {
                return;
            }
            _weights = __value;
            markNeedsLayout();
        }
    }
    public virtual bool infinite
    {
        get => _infinite;
        set
        {
            var __value = value;
            if (_infinite == __value)
            {
                return;
            }
            _infinite = __value;
            markNeedsLayout();
        }
    }
    internal virtual double _buildItemExtent(long index, global::Doroti.Framework.Rendering.SliverLayoutDimensions currentLayoutDimensions)
    {
        if (constraints.viewportMainAxisExtent == 0L)
        {
            return 0;
        }
        double extent = default!;
        if (index == _firstVisibleItemIndex)
        {
            extent = Math.Max(_distanceToLeadingEdge, effectiveShrinkExtent);
        }
        else
        {
            if ((index > _firstVisibleItemIndex) && ((index - _firstVisibleItemIndex + 1L) <= checked(weights.Count)))
            {
                DartRuntimePrimitives.Assert(() => (index - _firstVisibleItemIndex) < checked(weights.Count));
                long currIndexOnWeightList = index - _firstVisibleItemIndex;
                long currWeight = weights[(int)currIndexOnWeightList];
                extent = extentUnit * currWeight;
                double progress = _firstVisibleItemOffscreenExtent / firstChildExtent;
                long prevWeight = weights[(int)(currIndexOnWeightList - 1L)];
                double finalIncrease = (prevWeight - currWeight) / weights.max();
                extent = extent + (finalIncrease * progress * maxChildExtent);
            }
            else
            {
                if ((index > _firstVisibleItemIndex) && ((index - _firstVisibleItemIndex + 1L) > checked(weights.Count)))
                {
                    double visibleItemsTotalExtent = _distanceToLeadingEdge;
                    for (long i = _firstVisibleItemIndex + 1L; i < index; i++)
                    {
                        visibleItemsTotalExtent += _buildItemExtent(i, currentLayoutDimensions);
                    }
                    extent = Math.Max(constraints.remainingPaintExtent - visibleItemsTotalExtent, effectiveShrinkExtent);
                }
                else
                {
                    extent = Math.Max(minChildExtent, effectiveShrinkExtent);
                }
            }
        }
        return extent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double extentUnit => DartRuntimePrimitives.ConvertValue<double>(constraints.viewportMainAxisExtent / weights.reduce((total, extent) => total + extent));
    public virtual double firstChildExtent => DartRuntimePrimitives.ConvertValue<double>(weights.First() * extentUnit);
    public virtual double maxChildExtent => DartRuntimePrimitives.ConvertValue<double>(weights.max() * extentUnit);
    public virtual double minChildExtent => DartRuntimePrimitives.ConvertValue<double>(weights.min() * extentUnit);
    public virtual double effectiveShrinkExtent => Dart_uiLibrary.clampDouble(shrinkExtent, 0, minChildExtent);
    internal virtual long _firstVisibleItemIndex
    {
        get
        {
            if (constraints.viewportMainAxisExtent == 0.0)
            {
                return 0L;
            }
            var smallerWeightCount = 0L;
            foreach (long weight in weights)
            {
                if (weight == weights.max())
                {
                    break;
                }
                smallerWeightCount += 1L;
            }
            long index = default!;
            double actual = constraints.scrollOffset / firstChildExtent;
            long roundLocal = (constraints.scrollOffset / firstChildExtent).round();
            if ((actual - roundLocal).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance)
            {
                index = roundLocal;
            }
            else
            {
                index = actual.floor();
            }
            return consumeMaxWeight ? (index - smallerWeightCount) : index;
        }
    }
    internal virtual double _firstVisibleItemOffscreenExtent
    {
        get
        {
            if (constraints.viewportMainAxisExtent == 0.0)
            {
                return 0;
            }
            long index = default!;
            double actual = constraints.scrollOffset / firstChildExtent;
            long roundLocal = (constraints.scrollOffset / firstChildExtent).round();
            if ((actual - roundLocal).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance)
            {
                index = roundLocal;
            }
            else
            {
                index = actual.floor();
            }
            return constraints.scrollOffset - (index * firstChildExtent);
        }
    }
    internal virtual double _distanceToLeadingEdge => DartRuntimePrimitives.ConvertValue<double>(firstChildExtent - _firstVisibleItemOffscreenExtent);
    public override double indexToLayoutOffset(double itemExtent, long index)
    {
        if (index == _firstVisibleItemIndex)
        {
            if (_distanceToLeadingEdge <= effectiveShrinkExtent)
            {
                return constraints.scrollOffset - effectiveShrinkExtent + _distanceToLeadingEdge;
            }
            return constraints.scrollOffset;
        }
        double visibleItemsTotalExtent = _distanceToLeadingEdge;
        for (long i = _firstVisibleItemIndex + 1L; i < index; i++)
        {
            visibleItemsTotalExtent += _buildItemExtent(i, layoutDimensions);
        }
        return constraints.scrollOffset + visibleItemsTotalExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMinChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        return Math.Max(_firstVisibleItemIndex, 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        long? childCount = childManager.estimatedChildCount;
        if (infinite && (childCount is null))
        {
            double visibleItemsTotalExtent = _distanceToLeadingEdge;
            long index = _firstVisibleItemIndex + 1L;
            double safeMinExtent = Math.Max(minChildExtent, 1.0);
            long estimatedUpperBound = _firstVisibleItemIndex + (constraints.viewportMainAxisExtent / safeMinExtent).ceil();
            while ((visibleItemsTotalExtent < constraints.viewportMainAxisExtent) && (index < estimatedUpperBound))
            {
                visibleItemsTotalExtent += _buildItemExtent(index, layoutDimensions);
                if (visibleItemsTotalExtent >= constraints.viewportMainAxisExtent)
                {
                    return index;
                }
                index++;
            }
            return index;
        }
        if (childCount is not null)
        {
            long childCount__46235__value47249 = DartRuntimePrimitives.RequireValue(childCount);
            double visibleItemsTotalExtentLocal = _distanceToLeadingEdge;
            for (long i = _firstVisibleItemIndex + 1L; i < DartRuntimePrimitives.RequireValue(childCount__46235__value47249); i++)
            {
                visibleItemsTotalExtentLocal += _buildItemExtent(i, layoutDimensions);
                if (visibleItemsTotalExtentLocal >= constraints.viewportMainAxisExtent)
                {
                    return i;
                }
            }
        }
        return childCount ?? 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxScrollOffset(global::Doroti.Framework.Rendering.SliverConstraints constraints, double itemExtent)
    {
        if (infinite)
        {
            return double.PositiveInfinity;
        }
        return childManager.childCount * maxChildExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    // Dart library-private member: distinct from the same name in the base library.
    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _getChildConstraints(long index)
    {
        double extent = DartRuntimePrimitives.RequireValue(itemExtentBuilder!(index, layoutDimensions));
        return constraints.asBoxConstraints(minExtent: extent, maxExtent: extent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => (itemExtent is not null) && (itemExtentBuilder is null) || (itemExtent is null) && (itemExtentBuilder is not null));
        DartRuntimePrimitives.Assert(() => (itemExtentBuilder is not null) || double.IsFinite(DartRuntimePrimitives.RequireValue(itemExtent)) && (DartRuntimePrimitives.RequireValue(itemExtent) >= 0L));
        global::Doroti.Framework.Rendering.SliverConstraints constraintsLocal = constraints;
        childManager.didStartLayout();
        childManager.setDidUnderflow(false);
        double scrollOffsetLocal = constraintsLocal.scrollOffset + constraintsLocal.cacheOrigin;
        DartRuntimePrimitives.Assert(() => scrollOffsetLocal >= 0.0);
        double remainingExtent = constraintsLocal.remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => remainingExtent >= 0.0);
        double targetEndScrollOffset = scrollOffsetLocal + remainingExtent;
        double deprecatedExtraItemExtent = -1;
        long firstIndexLocal = getMinChildIndexForScrollOffset(scrollOffsetLocal, deprecatedExtraItemExtent);
        long? targetLastIndex = double.IsFinite(targetEndScrollOffset) ? getMaxChildIndexForScrollOffset(targetEndScrollOffset, deprecatedExtraItemExtent) : null;
        if (firstChild is not null)
        {
            long leadingGarbage = calculateLeadingGarbage(firstIndex: firstIndexLocal);
            long trailingGarbage = (targetLastIndex is not null) ? calculateTrailingGarbage(lastIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetLastIndex))) : 0L;
            collectGarbage(leadingGarbage, trailingGarbage);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        if (firstChild is null)
        {
            double layoutOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, firstIndexLocal);
            if (!addInitialChild(index: firstIndexLocal, layoutOffset: layoutOffsetLocal))
            {
                double maxLocal = default!;
                if (firstIndexLocal <= 0L)
                {
                    maxLocal = 0.0;
                }
                else
                {
                    maxLocal = computeMaxScrollOffset(constraintsLocal, deprecatedExtraItemExtent);
                }
                geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: maxLocal, maxPaintExtent: maxLocal);
                childManager.didFinishLayout();
                return;
            }
        }
        global::Doroti.Framework.Rendering.RenderBox? trailingChildWithLayout = default!;
        for (long indexLocal = indexOf(firstChild!) - 1L; indexLocal >= firstIndexLocal; --indexLocal)
        {
            global::Doroti.Framework.Rendering.RenderBox? child = insertAndLayoutLeadingChild(_getChildConstraints(indexLocal));
            if (child is null)
            {
                geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollOffsetCorrection: indexToLayoutOffset(deprecatedExtraItemExtent, indexLocal));
                return;
            }
            var childParentData = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)child.parentData!)!;
            childParentData.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, indexLocal);
            DartRuntimePrimitives.Assert(() => childParentData.index == indexLocal);
            trailingChildWithLayout ??= child;
        }
        if (trailingChildWithLayout is null)
        {
            firstChild!.layout(_getChildConstraints(indexOf(firstChild!)));
            var childParentDataLocal = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)firstChild!.parentData!)!;
            childParentDataLocal.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, firstIndexLocal);
            trailingChildWithLayout = firstChild;
        }
        double extraLayoutOffset = 0;
        if (consumeMaxWeight)
        {
            for (long i = checked(weights.Count) - 1L; i >= 0L; i--)
            {
                if (weights[(int)i] == weights.max())
                {
                    break;
                }
                extraLayoutOffset += weights[(int)i] * extentUnit;
            }
        }
        double estimatedMaxScrollOffset = double.PositiveInfinity;
        for (long indexAlternate = indexOf(trailingChildWithLayout!) + 1L; (targetLastIndex is null) || (indexAlternate <= DartRuntimePrimitives.RequireValue(targetLastIndex)); ++indexAlternate)
        {
            global::Doroti.Framework.Rendering.RenderBox? childLocal = childAfter(trailingChildWithLayout!);
            if ((childLocal is null) || (indexOf(childLocal) != indexAlternate))
            {
                childLocal = insertAndLayoutChild(_getChildConstraints(indexAlternate), after: trailingChildWithLayout);
                if (childLocal is null)
                {
                    estimatedMaxScrollOffset = indexToLayoutOffset(deprecatedExtraItemExtent, indexAlternate) + extraLayoutOffset;
                    break;
                }
            }
            else
            {
                childLocal.layout(_getChildConstraints(indexAlternate));
            }
            trailingChildWithLayout = childLocal;
            var childParentDataAlternate = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)childLocal.parentData!)!;
            DartRuntimePrimitives.Assert(() => childParentDataAlternate.index == indexAlternate);
            childParentDataAlternate.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, DartRuntimePrimitives.RequireValue(childParentDataAlternate.index));
        }
        long lastIndexLocal = indexOf(lastChild!);
        double leadingScrollOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, firstIndexLocal);
        double trailingScrollOffsetLocal = default!;
        if (!infinite && ((lastIndexLocal + 1L) == childManager.childCount))
        {
            trailingScrollOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, lastIndexLocal);
            trailingScrollOffsetLocal += Math.Max(weights.Last() * extentUnit, _buildItemExtent(lastIndexLocal, layoutDimensions));
            trailingScrollOffsetLocal += extraLayoutOffset;
        }
        else
        {
            trailingScrollOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, lastIndexLocal + 1L);
        }
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => indexOf(firstChild!) == firstIndexLocal);
        DartRuntimePrimitives.Assert(() => (targetLastIndex is null) || (lastIndexLocal <= DartRuntimePrimitives.RequireValue(targetLastIndex)));
        estimatedMaxScrollOffset = Math.Min(estimatedMaxScrollOffset, estimateMaxScrollOffset(constraintsLocal, firstIndex: firstIndexLocal, lastIndex: lastIndexLocal, leadingScrollOffset: leadingScrollOffsetLocal, trailingScrollOffset: trailingScrollOffsetLocal));
        double paintExtentLocal = calculatePaintOffset(constraintsLocal, from: consumeMaxWeight ? 0 : leadingScrollOffsetLocal, to: trailingScrollOffsetLocal);
        double cacheExtentLocal = calculateCacheOffset(constraintsLocal, from: consumeMaxWeight ? 0 : leadingScrollOffsetLocal, to: trailingScrollOffsetLocal);
        double targetEndScrollOffsetForPaint = constraintsLocal.scrollOffset + constraintsLocal.remainingPaintExtent;
        long? targetLastIndexForPaint = double.IsFinite(targetEndScrollOffsetForPaint) ? getMaxChildIndexForScrollOffset(targetEndScrollOffsetForPaint, deprecatedExtraItemExtent) : null;
        geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: estimatedMaxScrollOffset, paintExtent: paintExtentLocal, cacheExtent: cacheExtentLocal, maxPaintExtent: estimatedMaxScrollOffset, hasVisualOverflow: (targetLastIndexForPaint is not null) && (lastIndexLocal >= DartRuntimePrimitives.RequireValue(targetLastIndexForPaint)) || (constraintsLocal.scrollOffset > 0.0));
        if (estimatedMaxScrollOffset == trailingScrollOffsetLocal)
        {
            childManager.setDidUnderflow(true);
        }
        childManager.didFinishLayout();
    }

    public override double? itemExtent => null;
    public override ItemExtentBuilder? itemExtentBuilder => (index, dimensions) => _buildItemExtent(index, dimensions);
}

public class CarouselScrollPhysics : global::Doroti.Framework.Widgets.ScrollPhysics
{
    public CarouselScrollPhysics(global::Doroti.Framework.Widgets.ScrollPhysics? parent = null) : base(parent: parent)
    {
    }

    public override CarouselScrollPhysics applyTo(global::Doroti.Framework.Widgets.ScrollPhysics? ancestor)
    {
        return new CarouselScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getTargetPixels(_CarouselPosition__carousel position, global::Doroti.Framework.Physics.Tolerance tolerance, double velocity)
    {
        double fraction = default!;
        if (position.itemExtent is not null)
        {
            fraction = DartRuntimePrimitives.RequireValue(position.itemExtent) / position.viewportDimension;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => position.flexWeights is not null);
            fraction = position.flexWeights!.First() / position.flexWeights!.sum();
        }
        double itemWidth = position.viewportDimension * fraction;
        double actual = Math.Max(0.0, position.pixels) / itemWidth;
        double round = actual.roundToDouble();
        double item = default!;
        if ((actual - round).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance)
        {
            item = round;
        }
        else
        {
            item = actual;
        }
        if (velocity < -tolerance.velocity)
        {
            item -= 0.5;
        }
        else
        {
            if (velocity > tolerance.velocity)
            {
                item += 0.5;
            }
        }
        return item.roundToDouble() * itemWidth;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Physics.Simulation? createBallisticSimulation(global::Doroti.Framework.Widgets.ScrollMetrics position, double velocity)
    {
        DartRuntimePrimitives.Assert(() => position is _CarouselPosition__carousel, () => (object?)"CarouselScrollPhysics can only be used with Scrollables that uses " + "the CarouselController");
        var metrics = ((_CarouselPosition__carousel?)position)!;
        if ((velocity <= 0.0) && (metrics.pixels <= metrics.minScrollExtent) || (velocity >= 0.0) && (metrics.pixels >= metrics.maxScrollExtent))
        {
            return base.createBallisticSimulation(metrics, velocity);
        }
        global::Doroti.Framework.Physics.Tolerance toleranceLocal = toleranceFor(metrics);
        double target = _getTargetPixels(metrics, toleranceLocal, velocity);
        if (target != metrics.pixels)
        {
            return (global::Doroti.Framework.Physics.Simulation?)new global::Doroti.Framework.Physics.ScrollSpringSimulation(spring, metrics.pixels, target, velocity, tolerance: toleranceLocal);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool allowImplicitScrolling => true;
}

public class _CarouselMetrics__carousel : global::Doroti.Framework.Widgets.FixedScrollMetrics
{
    public virtual double? itemExtent { get; private set; }
    public virtual List<long>? flexWeights { get; private set; }
    public virtual bool? consumeMaxWeight { get; private set; }
    public _CarouselMetrics__carousel() : base(default!, default!, default!, default!, default!, default!) { }


    internal _CarouselMetrics__carousel(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, global::Doroti.Framework.Painting.AxisDirection axisDirection, double? itemExtent = null, List<long>? flexWeights = null, bool? consumeMaxWeight = null, double devicePixelRatio = default!) : base(minScrollExtent: minScrollExtent, maxScrollExtent: maxScrollExtent, pixels: pixels, viewportDimension: viewportDimension, axisDirection: axisDirection, devicePixelRatio: devicePixelRatio)
    {
        this.itemExtent = itemExtent;
        this.flexWeights = flexWeights;
        this.consumeMaxWeight = consumeMaxWeight;
    }

    public override _CarouselMetrics__carousel copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null) =>
        copyWithCarousel(minScrollExtent: minScrollExtent, maxScrollExtent: maxScrollExtent, pixels: pixels, viewportDimension: viewportDimension, axisDirection: axisDirection, devicePixelRatio: devicePixelRatio);

    public virtual _CarouselMetrics__carousel copyWithCarousel(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, double? itemExtent = null, List<long>? flexWeights = null, bool? consumeMaxWeight = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new _CarouselMetrics__carousel(minScrollExtent: minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : null), maxScrollExtent: maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : null), pixels: pixels ?? (hasPixels ? this.pixels : null), viewportDimension: viewportDimension ?? (hasViewportDimension ? this.viewportDimension : null), axisDirection: axisDirection ?? this.axisDirection, itemExtent: itemExtent ?? this.itemExtent, flexWeights: flexWeights ?? this.flexWeights, consumeMaxWeight: consumeMaxWeight ?? this.consumeMaxWeight, devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CarouselPosition__carousel : global::Doroti.Framework.Widgets.ScrollPositionWithSingleContext
{
    public virtual long initialItem { get; set; } = default!;
    internal virtual double _itemToShowOnStartup { get; private set; } = default!;
    internal virtual long? _itemCount { get; set; } = default;
    internal virtual bool _infinite { get; set; } = default!;
    internal virtual double? _cachedItem { get; set; } = default;
    internal virtual bool _consumeMaxWeight { get; set; } = default!;
    internal virtual double? _itemExtent { get; set; } = default;
    internal virtual List<long>? _flexWeights { get; set; } = default;

    internal _CarouselPosition__carousel(global::Doroti.Framework.Widgets.ScrollPhysics physics, global::Doroti.Framework.Widgets.ScrollContext context, long initialItem = 0, double? itemExtent = null, List<long>? flexWeights = null, bool consumeMaxWeight = true, bool infinite = false, long? itemCount = null, global::Doroti.Framework.Widgets.ScrollPosition? oldPosition = null) : base(physics: physics, context: context, oldPosition: oldPosition, initialPixels: null)
    {
        this.initialItem = initialItem;
        _itemToShowOnStartup = initialItem.toDouble();
        _consumeMaxWeight = DartRuntimePrimitives.RequireValue(consumeMaxWeight);
        _infinite = infinite;
        _itemCount = itemCount;
        System.Diagnostics.Debug.Assert(((flexWeights is not null) && (itemExtent is null)) || ((flexWeights is null) && (itemExtent is not null)));
    }

    public virtual long? itemCount
    {
        get => _itemCount;
        set
        {
            var __value = value;
            if (_itemCount == __value)
            {
                return;
            }
            _itemCount = __value;
        }
    }
    public virtual bool infinite
    {
        get => _infinite;
        set
        {
            var __value = value;
            if (_infinite == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            _infinite = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual bool consumeMaxWeight
    {
        get => _consumeMaxWeight;
        set
        {
            var __value = value;
            if (_consumeMaxWeight == DartRuntimePrimitives.RequireValue(__value))
            {
                return;
            }
            if (hasPixels && (flexWeights is not null))
            {
                double leadingItem = updateLeadingItem(flexWeights, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(__value)));
                double newPixel = getPixelsFromItem(leadingItem, flexWeights, itemExtent);
                forcePixels(newPixel);
            }
            _consumeMaxWeight = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual double? itemExtent
    {
        get => _itemExtent;
        set
        {
            var __value = value;
            if (_itemExtent == __value)
            {
                return;
            }
            if (hasPixels && (_itemExtent is not null) && (viewportDimension != 0.0))
            {
                double leadingItem = getItemFromPixels(DartRuntimePrimitives.RequireValue(pixels), DartRuntimePrimitives.RequireValue(viewportDimension));
                double newPixel = getPixelsFromItem(leadingItem, flexWeights, __value);
                forcePixels(newPixel);
            }
            _itemExtent = __value;
        }
    }
    public virtual List<long>? flexWeights
    {
        get => _flexWeights;
        set
        {
            var __value = value;
            if (Equals(flexWeights, __value))
            {
                return;
            }
            List<long>? oldWeights = _flexWeights?.ToList();
            if (hasPixels && (oldWeights is not null))
            {
                double leadingItem = updateLeadingItem(__value, DartRuntimePrimitives.RequireValue(consumeMaxWeight));
                double newPixel = getPixelsFromItem(leadingItem, __value, itemExtent);
                forcePixels(newPixel);
            }
            _flexWeights = __value;
        }
    }
    public virtual long leadingItem
    {
        get
        {
            long leadingItem = getItemFromPixels(DartRuntimePrimitives.RequireValue(pixels), DartRuntimePrimitives.RequireValue(viewportDimension)).toInt();
            if (consumeMaxWeight && (flexWeights is not null))
            {
                leadingItem = Math.Max(leadingItem - flexWeights!.IndexOf(flexWeights!.max()), 0L);
            }
            if (infinite && (itemCount is not null) && (DartRuntimePrimitives.RequireValue(itemCount) > 0L))
            {
                long itemCount__value64303 = DartRuntimePrimitives.RequireValue(itemCount);
                leadingItem = leadingItem % DartRuntimePrimitives.RequireValue(itemCount);
            }
            return leadingItem;
        }
    }
    public virtual double updateLeadingItem(List<long>? newFlexWeights, bool newConsumeMaxWeight)
    {
        double maxItem = default!;
        if (hasPixels && (flexWeights is not null))
        {
            double leadingItem = getItemFromPixels(DartRuntimePrimitives.RequireValue(pixels), DartRuntimePrimitives.RequireValue(viewportDimension));
            maxItem = consumeMaxWeight ? leadingItem : (leadingItem + flexWeights!.IndexOf(flexWeights!.max()));
        }
        else
        {
            if (!newConsumeMaxWeight)
            {
                return _itemToShowOnStartup;
            }
            maxItem = _itemToShowOnStartup;
        }
        if ((newFlexWeights is not null) && !newConsumeMaxWeight)
        {
            var smallerWeights = 0L;
            foreach (long weight in newFlexWeights)
            {
                if (weight == newFlexWeights.max())
                {
                    break;
                }
                smallerWeights += 1L;
            }
            return maxItem - smallerWeights;
        }
        return maxItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getItemFromPixels(double pixels, double viewportDimension)
    {
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(viewportDimension) > 0.0);
        double fraction = default!;
        if (itemExtent is not null)
        {
            double itemExtent__value65364 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction = DartRuntimePrimitives.RequireValue(itemExtent) / DartRuntimePrimitives.RequireValue(viewportDimension);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => flexWeights is not null);
            fraction = flexWeights!.First() / flexWeights!.sum();
        }
        double actual = Math.Max(0.0, DartRuntimePrimitives.RequireValue(pixels)) / (DartRuntimePrimitives.RequireValue(viewportDimension) * fraction);
        double round = actual.roundToDouble();
        if ((actual - round).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance)
        {
            return round;
        }
        return actual;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getPixelsFromItem(double item, List<long>? flexWeights, double? itemExtent)
    {
        double fraction = default!;
        if (viewportDimension == 0.0)
        {
            return 0.0;
        }
        if (itemExtent is not null)
        {
            double itemExtent__value66023 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction = DartRuntimePrimitives.RequireValue(itemExtent__value66023) / viewportDimension;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => flexWeights is not null);
            var weights = flexWeights ?? throw new InvalidOperationException("A weighted carousel requires flex weights.");
            fraction = weights.First() / weights.sum();
        }
        return item * viewportDimension * fraction;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool applyViewportDimension(double viewportDimension)
    {
        double? oldViewportDimensions = hasViewportDimension ? this.viewportDimension : null;
        if (DartRuntimePrimitives.RequireValue(viewportDimension) == oldViewportDimensions)
        {
            return true;
        }
        bool result = base.applyViewportDimension(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(viewportDimension)));
        double? oldPixels = hasPixels ? pixels : null;
        double item = default!;
        if (oldPixels is null)
        {
            item = updateLeadingItem(flexWeights, DartRuntimePrimitives.RequireValue(consumeMaxWeight));
        }
        else
        {
            if (oldViewportDimensions == 0.0)
            {
                item = DartRuntimePrimitives.RequireValue(_cachedItem);
            }
            else
            {
                item = getItemFromPixels(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(oldPixels)), oldViewportDimensions ?? DartRuntimePrimitives.RequireValue(viewportDimension));
            }
        }
        double newPixels = getPixelsFromItem(item, flexWeights, itemExtent);
        _cachedItem = (DartRuntimePrimitives.RequireValue(viewportDimension) == 0.0) ? item : null;
        if (newPixels != oldPixels)
        {
            correctPixels(newPixels);
            return false;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void absorb(global::Doroti.Framework.Widgets.ScrollPosition other)
    {
        base.absorb(other);
        if (other is not _CarouselPosition__carousel)
        {
            return;
        }
        _cachedItem = ((_CarouselPosition__carousel)other)._cachedItem;
        _itemExtent = ((_CarouselPosition__carousel)other)._itemExtent;
    }

    internal virtual double _getCycleLengthInPixels()
    {
        if ((itemCount is null) || (DartRuntimePrimitives.RequireValue(itemCount) <= 0L) || !hasViewportDimension || (viewportDimension == 0L))
        {
            return 0.0;
        }
        double fraction = default!;
        if (itemExtent is not null)
        {
            double itemExtent__value67978 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction = DartRuntimePrimitives.RequireValue(itemExtent) / viewportDimension;
        }
        else
        {
            if (flexWeights is not null)
            {
                fraction = flexWeights!.First() / flexWeights!.sum();
            }
            else
            {
                return 0.0;
            }
        }
        return DartRuntimePrimitives.RequireValue(itemCount) * viewportDimension * fraction;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        if (infinite && hasPixels)
        {
            double cycleLength = _getCycleLengthInPixels();
            if ((cycleLength > 0L) && (pixels < cycleLength))
            {
                long cyclesToAdd = ((cycleLength - pixels) / cycleLength).ceil();
                correctPixels(pixels + (cyclesToAdd * cycleLength));
                return false;
            }
        }
        return base.applyContentDimensions(infinite ? 0.0 : DartRuntimePrimitives.RequireValue(minScrollExtent), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(maxScrollExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override _CarouselMetrics__carousel copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null) =>
        copyWithCarousel(minScrollExtent: minScrollExtent, maxScrollExtent: maxScrollExtent, pixels: pixels, viewportDimension: viewportDimension, axisDirection: axisDirection, devicePixelRatio: devicePixelRatio);

    public virtual _CarouselMetrics__carousel copyWithCarousel(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, double? itemExtent = null, List<long>? flexWeights = null, bool? consumeMaxWeight = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new _CarouselMetrics__carousel(minScrollExtent: minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : null), maxScrollExtent: maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : null), pixels: pixels ?? (hasPixels ? this.pixels : null), viewportDimension: viewportDimension ?? (hasViewportDimension ? this.viewportDimension : null), axisDirection: axisDirection ?? this.axisDirection, itemExtent: itemExtent ?? this.itemExtent, flexWeights: flexWeights ?? this.flexWeights, consumeMaxWeight: consumeMaxWeight ?? this.consumeMaxWeight, devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CarouselController : global::Doroti.Framework.Widgets.ScrollController
{
    public virtual long initialItem { get; private set; } = default!;
    internal virtual _CarouselViewState__carousel? _carouselState { get; set; } = default;

    public CarouselController(long initialItem = 0)
    {
        this.initialItem = initialItem;
    }

    public virtual long leadingItem
    {
        get
        {
            DartRuntimePrimitives.Assert(() => Enumerable.Any(positions), () => (object?)"CarouselController.leadingItem cannot be accessed before a CarouselView is built with it.");
            DartRuntimePrimitives.Assert(() => positions.Count() == 1L, () => (object?)"CarouselController.leadingItem cannot be read when multiple CarouselViews " + "are attached to the same controller.");
            return ((_CarouselPosition__carousel?)position)!.leadingItem;
        }
    }
    internal virtual void _attach(_CarouselViewState__carousel anchor)
    {
        _carouselState = anchor;
    }

    internal virtual void _detach(_CarouselViewState__carousel anchor)
    {
        if (Equals(_carouselState, anchor))
        {
            _carouselState = null;
        }
    }

    public async virtual Future animateToItem(long index, Duration? duration = null, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        if (!hasClients || (_carouselState is null))
        {
            return;
        }
        bool hasFlexWeights = (_carouselState!._flexWeights is { } __items72635 ? System.Linq.Enumerable.Any(__items72635) : (bool?)null) ?? false;
        if (_carouselState!.widget.itemBuilder is not null)
        {
            long? itemCountLocal = _carouselState!.widget.itemCount;
            index = (itemCountLocal is not null) ? index.clamp(0L, DartRuntimePrimitives.RequireValue(itemCountLocal) - 1L) : 0L;
        }
        else
        {
            index = index.clamp(0L, checked(_carouselState!.widget.children.Count) - 1L);
        }
        await DartAsyncRuntime.wait<object?>(((Func<List<Future>>)(() => { var __collection72994 = new List<Future>(); foreach (var position in positions.cast<_CarouselPosition__carousel>()) { __collection72994.Add(position.animateTo(_getTargetOffset(position, index, hasFlexWeights), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)), curve: curve)); } return __collection72994; }))());
    }

    internal virtual double _getTargetOffset(_CarouselPosition__carousel position, long index, bool hasFlexWeights)
    {
        if (!hasFlexWeights)
        {
            double targetInFirstCycle = index * DartRuntimePrimitives.RequireValue(_carouselState!._itemExtent);
            if (!_carouselState!.widget.infinite)
            {
                return targetInFirstCycle;
            }
            return _adjustForInfiniteCycle(position, targetInFirstCycle);
        }
        _CarouselViewState__carousel carouselState = _carouselState!;
        List<long> weights = carouselState._flexWeights!.ToList();
        long totalWeight = weights.reduce((a, b) => a + b);
        double dimension = position.viewportDimension;
        long maxWeightIndex = weights.IndexOf(weights.max());
        long leadingIndex = carouselState._consumeMaxWeight ? index : (index - maxWeightIndex);
        if (carouselState.widget.itemBuilder is not null)
        {
            long? itemCountLocal = carouselState.widget.itemCount;
            leadingIndex = (itemCountLocal is not null) ? leadingIndex.clamp(0L, DartRuntimePrimitives.RequireValue(itemCountLocal) - 1L) : 0L;
        }
        else
        {
            long itemCountAlternate = checked(carouselState.widget.children.Count);
            leadingIndex = leadingIndex.clamp(0L, itemCountAlternate - 1L);
        }
        double targetInFirstCycleLocal = dimension * (weights.First() / totalWeight) * leadingIndex;
        if (!carouselState.widget.infinite)
        {
            return targetInFirstCycleLocal;
        }
        return _adjustForInfiniteCycle(position, targetInFirstCycleLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _adjustForInfiniteCycle(_CarouselPosition__carousel position, double targetInFirstCycle)
    {
        double cycleLength = position._getCycleLengthInPixels();
        if (cycleLength <= 0L)
        {
            return targetInFirstCycle;
        }
        double currentPixels = position.pixels;
        double currentCycleStart = (currentPixels / cycleLength).floorToDouble() * cycleLength;
        double sameCycleTarget = currentCycleStart + targetInFirstCycle;
        if (sameCycleTarget >= currentPixels)
        {
            return sameCycleTarget;
        }
        return sameCycleTarget + cycleLength;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long? _getItemCount()
    {
        if (_carouselState is null)
        {
            return null;
        }
        if (_carouselState!.widget.itemBuilder is not null)
        {
            return _carouselState!.widget.itemCount;
        }
        return checked(_carouselState!.widget.children.Count);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.ScrollPosition createScrollPosition(global::Doroti.Framework.Widgets.ScrollPhysics physics, global::Doroti.Framework.Widgets.ScrollContext context, global::Doroti.Framework.Widgets.ScrollPosition? oldPosition)
    {
        DartRuntimePrimitives.Assert(() => _carouselState is not null);
        return new _CarouselPosition__carousel(physics: physics, context: context, initialItem: initialItem, itemExtent: _carouselState!._itemExtent, consumeMaxWeight: _carouselState!._consumeMaxWeight, flexWeights: _carouselState!._flexWeights, infinite: _carouselState!.widget.infinite, itemCount: _getItemCount(), oldPosition: oldPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Framework.Widgets.ScrollPosition position)
    {
        base.attach(position);
        var carouselPosition = ((_CarouselPosition__carousel?)position)!;
        carouselPosition.flexWeights = _carouselState!._flexWeights;
        carouselPosition.itemExtent = _carouselState!._itemExtent;
        carouselPosition.consumeMaxWeight = _carouselState!._consumeMaxWeight;
        carouselPosition.infinite = _carouselState!.widget.infinite;
        carouselPosition.itemCount = _getItemCount();
    }

}
