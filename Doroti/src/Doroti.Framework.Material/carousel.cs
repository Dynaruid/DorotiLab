// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/carousel.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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

    public CarouselView(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? itemClipBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, bool itemSnapping = false, double shrinkExtent = 0.0, CarouselController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.horizontal, bool reverse = false, global::System.Action<long>? onTap = null, bool enableSplash = true, bool infinite = false, double itemExtent = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::System.Action<long>? onIndexChanged = null) : base(key: key)
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
        this.consumeMaxWeight = true;
        this.flexWeights = null;
        this.itemBuilder = null;
        this.itemCount = null;
    }

    public static CarouselView CreateWeighted(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? itemClipBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, bool itemSnapping = false, double shrinkExtent = 0.0, CarouselController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.horizontal, bool reverse = false, bool consumeMaxWeight = true, global::System.Action<long>? onTap = null, bool enableSplash = true, bool infinite = false, List<long> flexWeights = default!, List<global::Doroti.Framework.Widgets.Widget> children = default!, global::System.Action<long>? onIndexChanged = null)
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

    public static CarouselView CreateBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? itemClipBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, bool itemSnapping = false, double shrinkExtent = 0.0, CarouselController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.horizontal, bool reverse = false, global::System.Action<long>? onTap = null, bool enableSplash = true, double itemExtent = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>? itemBuilder = default!, long? itemCount = null, global::System.Action<long>? onIndexChanged = null, bool infinite = false)
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

    public static CarouselView CreateWeightedBuilder(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Painting.EdgeInsets? padding = null, Color? backgroundColor = null, double? elevation = null, global::Doroti.Framework.Painting.ShapeBorder? shape = null, Clip? itemClipBehavior = null, global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>? overlayColor = null, bool itemSnapping = false, double shrinkExtent = 0.0, CarouselController? controller = null, global::Doroti.Framework.Painting.Axis scrollDirection = global::Doroti.Framework.Painting.Axis.horizontal, bool reverse = false, bool consumeMaxWeight = true, global::System.Action<long>? onTap = null, bool enableSplash = true, List<long> flexWeights = default!, global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>? itemBuilder = default!, long? itemCount = null, global::System.Action<long>? onIndexChanged = null, bool infinite = false)
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

    internal virtual List<long>? _flexWeights => ((CarouselView)this.widget).flexWeights;
    internal virtual bool _consumeMaxWeight => ((CarouselView)this.widget).consumeMaxWeight;
    internal virtual CarouselController _controller => DartRuntimePrimitives.ConvertValue<CarouselController>((((CarouselView)this.widget).controller ?? this._internalController!));
    public override void initState()
    {
        base.initState();
        _itemExtent = ((CarouselView)this.widget).itemExtent;
        if ((((CarouselView)this.widget).controller is null))
        {
            _internalController = new CarouselController();
        }
        _lastReportedLeadingItem = _getInitialLeadingItem();
        this._controller._attach(this);
        this._controller.addListener(() => this._handleScroll());
    }

    public override void didUpdateWidget(CarouselView oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((CarouselView)this.widget).controller, ((CarouselView)oldWidget).controller)))
        {
            ((CarouselView)oldWidget).controller?._detach(this);
            if ((((CarouselView)this.widget).controller is not null))
            {
                this._internalController?._detach(this);
                _internalController = null;
                ((CarouselView)this.widget).controller?._attach(this);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => (this._internalController is null));
                _internalController = new CarouselController();
                this._controller._attach(this);
            }
        }
        if ((!object.Equals(((CarouselView)this.widget).flexWeights, ((CarouselView)oldWidget).flexWeights)))
        {
            (((_CarouselPosition__carousel?)(object?)this._controller.position)!).flexWeights = this._flexWeights;
        }
        if ((((CarouselView)this.widget).itemExtent != ((CarouselView)oldWidget).itemExtent))
        {
            _itemExtent = ((CarouselView)this.widget).itemExtent;
            (((_CarouselPosition__carousel?)(object?)this._controller.position)!).itemExtent = this._itemExtent;
        }
        if ((((CarouselView)this.widget).consumeMaxWeight != ((CarouselView)oldWidget).consumeMaxWeight))
        {
            (((_CarouselPosition__carousel?)(object?)this._controller.position)!).consumeMaxWeight = this._consumeMaxWeight;
        }
    }

    public override void dispose()
    {
        this._controller.removeListener(() => this._handleScroll());
        this._controller._detach(this);
        this._internalController?.dispose();
        base.dispose();
    }

    internal virtual void _handleScroll()
    {
        if ((((CarouselView)this.widget).onIndexChanged is null))
        {
            return;
        }
        global::Doroti.Framework.Widgets.ScrollPosition positionLocal = this._controller.position;
        long currentLeadingIndex = (((_CarouselPosition__carousel?)(object?)positionLocal)!).leadingItem;
        if ((currentLeadingIndex != this._lastReportedLeadingItem))
        {
            _lastReportedLeadingItem = currentLeadingIndex;
            ((CarouselView)this.widget).onIndexChanged!(currentLeadingIndex);
        }
    }

    internal virtual long _getInitialLeadingItem()
    {
        if ((((CarouselView)this.widget).flexWeights is not null))
        {
            long maxWeight = ((CarouselView)this.widget).flexWeights!.max();
            long firstMaxWeightIndex = ((long)((dynamic)((CarouselView)this.widget).flexWeights!).IndexOf(maxWeight));
            return Math.Max((((CarouselController)this._controller).initialItem - firstMaxWeightIndex), 0L);
        }
        return ((CarouselController)this._controller).initialItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildCarouselItem(long index)
    {
        if ((((CarouselView)this.widget).infinite && System.Linq.Enumerable.Any(((CarouselView)this.widget).children)))
        {
            index = (index % checked((long)(((CarouselView)this.widget).children.Count)));
        }
        CarouselViewThemeData carouselTheme = CarouselViewTheme.of(this.context);
        ColorScheme colorScheme = ColorScheme.of(this.context);
        global::Doroti.Framework.Painting.EdgeInsets effectivePadding = ((((CarouselView)this.widget).padding ?? carouselTheme.padding) ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0));
        global::Doroti.Ui.Color effectiveBackgroundColor = ((global::Doroti.Ui.Color)(object?)((((CarouselView)this.widget).backgroundColor ?? carouselTheme.backgroundColor) ?? colorScheme.surface));
        double effectiveElevation = ((((CarouselView)this.widget).elevation ?? carouselTheme.elevation) ?? 0.0);
        global::Doroti.Framework.Painting.ShapeBorder effectiveShape = ((((CarouselView)this.widget).shape ?? carouselTheme.shape) ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(28.0))));
        global::Doroti.Ui.Clip effectiveItemClipBehavior = ((((CarouselView)this.widget).itemClipBehavior ?? carouselTheme.itemClipBehavior) ?? Clip.antiAlias);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> effectiveOverlayColor = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)(((((CarouselView)this.widget).overlayColor ?? carouselTheme.overlayColor) ?? (global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, CarouselView>)((states) =>
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
            {
                return ((CarouselView)(object?)colorScheme.onSurface.withOpacity(0.1));
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
            {
                return ((CarouselView)(object?)colorScheme.onSurface.withOpacity(0.08));
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
            {
                return ((CarouselView)(object?)colorScheme.onSurface.withOpacity(0.1));
            }
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        global::Doroti.Framework.Widgets.Widget contents = ((CarouselView)this.widget).children[(int)(index)];
        if (((CarouselView)this.widget).enableSplash)
        {
            contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Stack(fit: global::Doroti.Framework.Rendering.StackFit.expand, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(contents), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(color: Colors.transparent, child: new InkWell(onTap: (() => { ((CarouselView)this.widget).onTap?.Invoke(index); }), overlayColor: effectiveOverlayColor))) }));
        }
        else
        {
            if ((((CarouselView)this.widget).onTap is not null))
            {
                contents = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.GestureDetector(onTap: ((global::System.Action)(() => { ((CarouselView)this.widget).onTap!(index); })), child: contents));
            }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: effectivePadding, child: new Material(clipBehavior: effectiveItemClipBehavior, color: effectiveBackgroundColor, elevation: effectiveElevation, shape: effectiveShape, child: contents)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSliverCarousel(ThemeData theme)
    {
        long? childCountLocal = (((CarouselView)this.widget).infinite ? null : ((((CarouselView)this.widget).itemBuilder is not null) ? ((CarouselView)this.widget).itemCount : checked((long)(((CarouselView)this.widget).children.Count))));
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?> effectiveBuilder = default!;
        if ((((CarouselView)this.widget).itemBuilder is not null))
        {
            if (((((CarouselView)this.widget).infinite && (((CarouselView)this.widget).itemCount is not null)) && (DartRuntimePrimitives.RequireValue(((CarouselView)this.widget).itemCount) > 0L)))
            {
                long itemCountLocal = DartRuntimePrimitives.RequireValue(((CarouselView)this.widget).itemCount);
                effectiveBuilder = (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((context, index) =>
                {
                    return ((CarouselView)this.widget).itemBuilder!(context, (index % itemCountLocal));
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            }
            else
            {
                effectiveBuilder = (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((CarouselView)this.widget).itemBuilder!;
            }
        }
        else
        {
            effectiveBuilder = (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((context, index) => _buildCarouselItem(index));
        }
        if ((this._itemExtent is not null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _SliverFixedExtentCarousel__carousel(itemExtent: DartRuntimePrimitives.RequireValue(this._itemExtent), minExtent: ((CarouselView)this.widget).shrinkExtent, infinite: ((CarouselView)this.widget).infinite, @delegate: new global::Doroti.Framework.Widgets.SliverChildBuilderDelegate((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)effectiveBuilder, childCount: childCountLocal)));
        }
        DartRuntimePrimitives.Assert(() => ((this._flexWeights is not null) && this._flexWeights!.All(((weight) => (weight > 0L)))), () => (object?)"flexWeights is null or it contains non-positive integers");
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _SliverWeightedCarousel__carousel(consumeMaxWeight: this._consumeMaxWeight, shrinkExtent: ((CarouselView)this.widget).shrinkExtent, weights: this._flexWeights!, infinite: ((CarouselView)this.widget).infinite, @delegate: new global::Doroti.Framework.Widgets.SliverChildBuilderDelegate((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)effectiveBuilder, childCount: childCountLocal)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        global::Doroti.Framework.Widgets.ScrollPhysics physicsLocal = (((CarouselView)this.widget).itemSnapping ? new CarouselScrollPhysics() : ScrollConfiguration.of(context).getScrollPhysics(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
        {
            double mainAxisExtent = (((CarouselView)this.widget).scrollDirection switch { global::Doroti.Framework.Painting.Axis.horizontal => ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, global::Doroti.Framework.Painting.Axis.vertical => ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            _itemExtent = ((((CarouselView)this.widget).itemExtent is null) ? null : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(((CarouselView)this.widget).itemExtent), 0, mainAxisExtent));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CustomScrollView(scrollDirection: ((CarouselView)this.widget).scrollDirection, reverse: ((CarouselView)this.widget).reverse, controller: this._controller, physics: physicsLocal, clipBehavior: Clip.antiAlias, scrollCacheExtent: global::Doroti.Framework.Rendering.ScrollCacheExtent.CreateViewport(0.0), slivers: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildSliverCarousel(theme)) }));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
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
        var element = ((global::Doroti.Framework.Widgets.SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverFixedExtentCarousel__carousel(childManager: element, minExtent: this.minExtent, maxExtent: this.itemExtent, infinite: this.infinite));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverFixedExtentCarousel__carousel)(object)renderObject;
        __renderObject.maxExtent = this.itemExtent;
        __renderObject.minExtent = this.minExtent;
        __renderObject.infinite = this.infinite;
    }

}

public class _RenderSliverFixedExtentCarousel__carousel : global::Doroti.Framework.Rendering.RenderSliverFixedExtentBoxAdaptor
{
    internal virtual double _maxExtent { get; set; } = default!;
    internal virtual double _minExtent { get; set; } = default!;
    internal virtual bool _infinite { get; set; } = default!;

    internal _RenderSliverFixedExtentCarousel__carousel(global::Doroti.Framework.Rendering.RenderSliverBoxChildManager childManager, double maxExtent, double minExtent, bool infinite) : base(childManager: childManager)
    {
        this._maxExtent = maxExtent;
        this._minExtent = minExtent;
        this._infinite = infinite;
    }

    public virtual double maxExtent
    {
        get => this._maxExtent;
        set
        {
            var __value = value;
            if ((this._maxExtent == __value))
            {
                return;
            }
            _maxExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual double minExtent
    {
        get => this._minExtent;
        set
        {
            var __value = value;
            if ((this._minExtent == __value))
            {
                return;
            }
            _minExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual bool infinite
    {
        get => this._infinite;
        set
        {
            var __value = value;
            if ((this._infinite == __value))
            {
                return;
            }
            _infinite = __value;
            markNeedsLayout();
        }
    }
    internal virtual double _buildItemExtent(long index, global::Doroti.Framework.Rendering.SliverLayoutDimensions currentLayoutDimensions)
    {
        if ((this.maxExtent == 0.0))
        {
            return this.maxExtent;
        }
        long firstVisibleIndex = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.maxExtent)).floor();
        long offscreenItems = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.maxExtent)).floor();
        double offscreenExtent = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset - (offscreenItems * this.maxExtent));
        double effectiveMinExtent = Math.Max((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent % this.maxExtent), this.minExtent);
        if ((index == firstVisibleIndex))
        {
            double effectiveExtent = (this.maxExtent - offscreenExtent);
            return Math.Max(effectiveExtent, effectiveMinExtent);
        }
        double scrollOffsetForLastIndex = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset + ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent);
        if ((index == getMaxChildIndexForScrollOffset(scrollOffsetForLastIndex, this.maxExtent)))
        {
            return Dart_uiLibrary.clampDouble((scrollOffsetForLastIndex - (this.maxExtent * index)), effectiveMinExtent, this.maxExtent);
        }
        return this.maxExtent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double indexToLayoutOffset(double itemExtent, long index)
    {
        if ((this.maxExtent == 0.0))
        {
            return this.maxExtent;
        }
        long firstVisibleIndex = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.maxExtent)).floor();
        double effectiveMinExtent = Math.Max((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent % this.maxExtent), this.minExtent);
        if ((index == firstVisibleIndex))
        {
            double firstVisibleItemExtent = _buildItemExtent(index, this.layoutDimensions);
            if ((firstVisibleItemExtent <= effectiveMinExtent))
            {
                return (((this.maxExtent * index) - effectiveMinExtent) + this.maxExtent);
            }
            return ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset;
        }
        return (this.maxExtent * index);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMinChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if ((this.maxExtent == 0.0))
        {
            return 0L;
        }
        long firstVisibleIndex = ((scrollOffset / this.maxExtent)).floor();
        return Math.Max(firstVisibleIndex, 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if ((this.maxExtent > 0.0))
        {
            double actual = ((scrollOffset / this.maxExtent) - 1L);
            long roundLocal = actual.round();
            if (((((actual * this.maxExtent) - (roundLocal * this.maxExtent))).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                return Math.Max(0L, roundLocal);
            }
            return Math.Max(0L, actual.ceil());
        }
        return 0L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? itemExtent => DartRuntimePrimitives.ConvertValue<double>(null);
    public new ItemExtentBuilder? itemExtentBuilder => (index, dimensions) => this._buildItemExtent(index, dimensions);
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
        var element = ((global::Doroti.Framework.Widgets.SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverWeightedCarousel__carousel(childManager: element, consumeMaxWeight: this.consumeMaxWeight, shrinkExtent: this.shrinkExtent, weights: this.weights, infinite: this.infinite));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderSliverWeightedCarousel__carousel)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderSliverWeightedCarousel__carousel>)(() =>
{
    var __cascade = __renderObject;
    __cascade.consumeMaxWeight = this.consumeMaxWeight;
    __cascade.shrinkExtent = this.shrinkExtent;
    __cascade.weights = this.weights;
    __cascade.infinite = this.infinite;
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
        this._consumeMaxWeight = consumeMaxWeight;
        this._shrinkExtent = shrinkExtent;
        this._weights = weights;
        this._infinite = infinite;
    }

    public virtual bool consumeMaxWeight
    {
        get => this._consumeMaxWeight;
        set
        {
            var __value = value;
            if ((this._consumeMaxWeight == __value))
            {
                return;
            }
            _consumeMaxWeight = __value;
            markNeedsLayout();
        }
    }
    public virtual double shrinkExtent
    {
        get => this._shrinkExtent;
        set
        {
            var __value = value;
            if ((this._shrinkExtent == __value))
            {
                return;
            }
            _shrinkExtent = __value;
            markNeedsLayout();
        }
    }
    public virtual List<long> weights
    {
        get => this._weights;
        set
        {
            var __value = value;
            if ((object.Equals(this._weights, __value)))
            {
                return;
            }
            _weights = __value;
            markNeedsLayout();
        }
    }
    public virtual bool infinite
    {
        get => this._infinite;
        set
        {
            var __value = value;
            if ((this._infinite == __value))
            {
                return;
            }
            _infinite = __value;
            markNeedsLayout();
        }
    }
    internal virtual double _buildItemExtent(long index, global::Doroti.Framework.Rendering.SliverLayoutDimensions currentLayoutDimensions)
    {
        if ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent == 0L))
        {
            return 0;
        }
        double extent = default!;
        if ((index == this._firstVisibleItemIndex))
        {
            extent = Math.Max(this._distanceToLeadingEdge, this.effectiveShrinkExtent);
        }
        else
        {
            if (((index > this._firstVisibleItemIndex) && (((index - this._firstVisibleItemIndex) + 1L) <= checked((long)(this.weights.Count)))))
            {
                DartRuntimePrimitives.Assert(() => ((index - this._firstVisibleItemIndex) < checked((long)(this.weights.Count))));
                long currIndexOnWeightList = (index - this._firstVisibleItemIndex);
                long currWeight = this.weights[(int)(currIndexOnWeightList)];
                extent = (this.extentUnit * currWeight);
                double progress = (this._firstVisibleItemOffscreenExtent / this.firstChildExtent);
                long prevWeight = this.weights[(int)((currIndexOnWeightList - 1L))];
                double finalIncrease = (((prevWeight - currWeight)) / this.weights.max());
                extent = (extent + ((finalIncrease * progress) * this.maxChildExtent));
            }
            else
            {
                if (((index > this._firstVisibleItemIndex) && (((index - this._firstVisibleItemIndex) + 1L) > checked((long)(this.weights.Count)))))
                {
                    double visibleItemsTotalExtent = this._distanceToLeadingEdge;
                    for (long i = (this._firstVisibleItemIndex + 1L); (i < index); i++)
                    {
                        visibleItemsTotalExtent += _buildItemExtent(i, currentLayoutDimensions);
                    }
                    extent = Math.Max((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent - visibleItemsTotalExtent), this.effectiveShrinkExtent);
                }
                else
                {
                    extent = Math.Max(this.minChildExtent, this.effectiveShrinkExtent);
                }
            }
        }
        return extent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double extentUnit => DartRuntimePrimitives.ConvertValue<double>((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent / (this.weights.reduce(((total, extent) => (total + extent))))));
    public virtual double firstChildExtent => DartRuntimePrimitives.ConvertValue<double>((this.weights.First() * this.extentUnit));
    public virtual double maxChildExtent => DartRuntimePrimitives.ConvertValue<double>((this.weights.max() * this.extentUnit));
    public virtual double minChildExtent => DartRuntimePrimitives.ConvertValue<double>((this.weights.min() * this.extentUnit));
    public virtual double effectiveShrinkExtent => Dart_uiLibrary.clampDouble(this.shrinkExtent, 0, this.minChildExtent);
    internal virtual long _firstVisibleItemIndex
    {
        get
        {
            if ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent == 0.0))
            {
                return 0L;
            }
            var smallerWeightCount = 0L;
            foreach (long weight in this.weights)
            {
                if ((weight == this.weights.max()))
                {
                    break;
                }
                smallerWeightCount += 1L;
            }
            long index = default!;
            double actual = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.firstChildExtent);
            long roundLocal = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.firstChildExtent)).round();
            if ((((actual - roundLocal)).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                index = roundLocal;
            }
            else
            {
                index = actual.floor();
            }
            return (this.consumeMaxWeight ? (index - smallerWeightCount) : index);
            return default!;
        }
    }
    internal virtual double _firstVisibleItemOffscreenExtent
    {
        get
        {
            if ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent == 0.0))
            {
                return 0;
            }
            long index = default!;
            double actual = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.firstChildExtent);
            long roundLocal = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.firstChildExtent)).round();
            if ((((actual - roundLocal)).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                index = roundLocal;
            }
            else
            {
                index = actual.floor();
            }
            return (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset - (index * this.firstChildExtent));
            return default!;
        }
    }
    internal virtual double _distanceToLeadingEdge => DartRuntimePrimitives.ConvertValue<double>((this.firstChildExtent - this._firstVisibleItemOffscreenExtent));
    public override double indexToLayoutOffset(double itemExtent, long index)
    {
        if ((index == this._firstVisibleItemIndex))
        {
            if ((this._distanceToLeadingEdge <= this.effectiveShrinkExtent))
            {
                return ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset - this.effectiveShrinkExtent) + this._distanceToLeadingEdge);
            }
            return ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset;
        }
        double visibleItemsTotalExtent = this._distanceToLeadingEdge;
        for (long i = (this._firstVisibleItemIndex + 1L); (i < index); i++)
        {
            visibleItemsTotalExtent += _buildItemExtent(i, this.layoutDimensions);
        }
        return (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset + visibleItemsTotalExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMinChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        return Math.Max(this._firstVisibleItemIndex, 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        long? childCount = ((global::Doroti.Framework.Rendering.RenderSliverBoxChildManager)this.childManager).estimatedChildCount;
        if ((this.infinite && (childCount is null)))
        {
            double visibleItemsTotalExtent = this._distanceToLeadingEdge;
            long index = (this._firstVisibleItemIndex + 1L);
            double safeMinExtent = Math.Max(this.minChildExtent, 1.0);
            long estimatedUpperBound = (this._firstVisibleItemIndex + ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent / safeMinExtent)).ceil());
            while (((visibleItemsTotalExtent < ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent) && (index < estimatedUpperBound)))
            {
                visibleItemsTotalExtent += _buildItemExtent(index, this.layoutDimensions);
                if ((visibleItemsTotalExtent >= ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent))
                {
                    return index;
                }
                index++;
            }
            return index;
        }
        if ((childCount is not null))
        {
            long childCount__46235__value47249 = DartRuntimePrimitives.RequireValue(childCount);
            double visibleItemsTotalExtentLocal = this._distanceToLeadingEdge;
            for (long i = (this._firstVisibleItemIndex + 1L); (i < DartRuntimePrimitives.RequireValue(childCount__46235__value47249)); i++)
            {
                visibleItemsTotalExtentLocal += _buildItemExtent(i, this.layoutDimensions);
                if ((visibleItemsTotalExtentLocal >= ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent))
                {
                    return i;
                }
            }
        }
        return (childCount ?? 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double computeMaxScrollOffset(global::Doroti.Framework.Rendering.SliverConstraints constraints, double itemExtent)
    {
        if (this.infinite)
        {
            return double.PositiveInfinity;
        }
        return (((global::Doroti.Framework.Rendering.RenderSliverBoxChildManager)this.childManager).childCount * this.maxChildExtent);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Rendering.BoxConstraints _getChildConstraints(long index)
    {
        double extent = DartRuntimePrimitives.RequireValue(this.itemExtentBuilder!(index, this.layoutDimensions));
        return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)this.constraints.asBoxConstraints(minExtent: extent, maxExtent: extent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => ((((this.itemExtent is not null) && (this.itemExtentBuilder is null))) || (((this.itemExtent is null) && (this.itemExtentBuilder is not null)))));
        DartRuntimePrimitives.Assert(() => ((this.itemExtentBuilder is not null) || ((double.IsFinite(DartRuntimePrimitives.RequireValue(this.itemExtent)) && (DartRuntimePrimitives.RequireValue(this.itemExtent) >= 0L)))));
        global::Doroti.Framework.Rendering.SliverConstraints constraintsLocal = this.constraints;
        this.childManager.didStartLayout();
        this.childManager.setDidUnderflow(false);
        double scrollOffsetLocal = (((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).scrollOffset + ((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).cacheOrigin);
        DartRuntimePrimitives.Assert(() => (scrollOffsetLocal >= 0.0));
        double remainingExtent = ((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => (remainingExtent >= 0.0));
        double targetEndScrollOffset = (scrollOffsetLocal + remainingExtent);
        double deprecatedExtraItemExtent = -1;
        long firstIndexLocal = getMinChildIndexForScrollOffset(scrollOffsetLocal, deprecatedExtraItemExtent);
        long? targetLastIndex = (double.IsFinite(targetEndScrollOffset) ? getMaxChildIndexForScrollOffset(targetEndScrollOffset, deprecatedExtraItemExtent) : null);
        if ((this.firstChild is not null))
        {
            long leadingGarbage = calculateLeadingGarbage(firstIndex: firstIndexLocal);
            long trailingGarbage = ((targetLastIndex is not null) ? calculateTrailingGarbage(lastIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetLastIndex))) : 0L);
            collectGarbage(leadingGarbage, trailingGarbage);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        if ((this.firstChild is null))
        {
            double layoutOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, firstIndexLocal);
            if (!addInitialChild(index: firstIndexLocal, layoutOffset: layoutOffsetLocal))
            {
                double maxLocal = default!;
                if ((firstIndexLocal <= 0L))
                {
                    maxLocal = 0.0;
                }
                else
                {
                    maxLocal = computeMaxScrollOffset(constraintsLocal, deprecatedExtraItemExtent);
                }
                geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: maxLocal, maxPaintExtent: maxLocal);
                this.childManager.didFinishLayout();
                return;
            }
        }
        global::Doroti.Framework.Rendering.RenderBox? trailingChildWithLayout = default!;
        for (long indexLocal = (indexOf(this.firstChild!) - 1L); (indexLocal >= firstIndexLocal); --indexLocal)
        {
            global::Doroti.Framework.Rendering.RenderBox? child = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)insertAndLayoutLeadingChild(_getChildConstraints(indexLocal)));
            if ((child is null))
            {
                geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollOffsetCorrection: indexToLayoutOffset(deprecatedExtraItemExtent, indexLocal));
                return;
            }
            var childParentData = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)child.parentData!)!;
            childParentData.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, indexLocal);
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData)childParentData).index == indexLocal));
            trailingChildWithLayout ??= child;
        }
        if ((trailingChildWithLayout is null))
        {
            this.firstChild!.layout(_getChildConstraints(indexOf(this.firstChild!)));
            var childParentDataLocal = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)this.firstChild!.parentData!)!;
            childParentDataLocal.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, firstIndexLocal);
            trailingChildWithLayout = this.firstChild;
        }
        double extraLayoutOffset = 0;
        if (this.consumeMaxWeight)
        {
            for (long i = (checked((long)(this.weights.Count)) - 1L); (i >= 0L); i--)
            {
                if ((this.weights[(int)(i)] == this.weights.max()))
                {
                    break;
                }
                extraLayoutOffset += (this.weights[(int)(i)] * this.extentUnit);
            }
        }
        double estimatedMaxScrollOffset = double.PositiveInfinity;
        for (long indexAlternate = (indexOf(trailingChildWithLayout!) + 1L); ((targetLastIndex is null) || (indexAlternate <= DartRuntimePrimitives.RequireValue(targetLastIndex))); ++indexAlternate)
        {
            global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childAfter(trailingChildWithLayout!));
            if (((childLocal is null) || (indexOf(childLocal) != indexAlternate)))
            {
                childLocal = insertAndLayoutChild(_getChildConstraints(indexAlternate), after: trailingChildWithLayout);
                if ((childLocal is null))
                {
                    estimatedMaxScrollOffset = (indexToLayoutOffset(deprecatedExtraItemExtent, indexAlternate) + extraLayoutOffset);
                    break;
                }
            }
            else
            {
                childLocal.layout(_getChildConstraints(indexAlternate));
            }
            trailingChildWithLayout = childLocal;
            var childParentDataAlternate = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)childLocal.parentData!)!;
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData)childParentDataAlternate).index == indexAlternate));
            childParentDataAlternate.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent, DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData)childParentDataAlternate).index));
        }
        long lastIndexLocal = indexOf(this.lastChild!);
        double leadingScrollOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, firstIndexLocal);
        double trailingScrollOffsetLocal = default!;
        if ((!this.infinite && ((lastIndexLocal + 1L) == ((global::Doroti.Framework.Rendering.RenderSliverBoxChildManager)this.childManager).childCount)))
        {
            trailingScrollOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, lastIndexLocal);
            trailingScrollOffsetLocal += Math.Max((this.weights.Last() * this.extentUnit), _buildItemExtent(lastIndexLocal, this.layoutDimensions));
            trailingScrollOffsetLocal += extraLayoutOffset;
        }
        else
        {
            trailingScrollOffsetLocal = indexToLayoutOffset(deprecatedExtraItemExtent, (lastIndexLocal + 1L));
        }
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => (indexOf(this.firstChild!) == firstIndexLocal));
        DartRuntimePrimitives.Assert(() => ((targetLastIndex is null) || (lastIndexLocal <= DartRuntimePrimitives.RequireValue(targetLastIndex))));
        estimatedMaxScrollOffset = Math.Min(estimatedMaxScrollOffset, estimateMaxScrollOffset(constraintsLocal, firstIndex: firstIndexLocal, lastIndex: lastIndexLocal, leadingScrollOffset: leadingScrollOffsetLocal, trailingScrollOffset: trailingScrollOffsetLocal));
        double paintExtentLocal = calculatePaintOffset(constraintsLocal, from: (this.consumeMaxWeight ? 0 : leadingScrollOffsetLocal), to: trailingScrollOffsetLocal);
        double cacheExtentLocal = calculateCacheOffset(constraintsLocal, from: (this.consumeMaxWeight ? 0 : leadingScrollOffsetLocal), to: trailingScrollOffsetLocal);
        double targetEndScrollOffsetForPaint = (((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).scrollOffset + ((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).remainingPaintExtent);
        long? targetLastIndexForPaint = (double.IsFinite(targetEndScrollOffsetForPaint) ? getMaxChildIndexForScrollOffset(targetEndScrollOffsetForPaint, deprecatedExtraItemExtent) : null);
        geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: estimatedMaxScrollOffset, paintExtent: paintExtentLocal, cacheExtent: cacheExtentLocal, maxPaintExtent: estimatedMaxScrollOffset, hasVisualOverflow: ((((targetLastIndexForPaint is not null) && (lastIndexLocal >= DartRuntimePrimitives.RequireValue(targetLastIndexForPaint)))) || (((global::Doroti.Framework.Rendering.SliverConstraints)constraintsLocal).scrollOffset > 0.0)));
        if ((estimatedMaxScrollOffset == trailingScrollOffsetLocal))
        {
            this.childManager.setDidUnderflow(true);
        }
        this.childManager.didFinishLayout();
    }

    public override double? itemExtent => DartRuntimePrimitives.ConvertValue<double>(null);
    public new ItemExtentBuilder? itemExtentBuilder => (index, dimensions) => this._buildItemExtent(index, dimensions);
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
        if ((((_CarouselPosition__carousel)position).itemExtent is not null))
        {
            fraction = (DartRuntimePrimitives.RequireValue(((_CarouselPosition__carousel)position).itemExtent) / position.viewportDimension);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (((_CarouselPosition__carousel)position).flexWeights is not null));
            fraction = (((_CarouselPosition__carousel)position).flexWeights!.First() / ((_CarouselPosition__carousel)position).flexWeights!.sum());
        }
        double itemWidth = (position.viewportDimension * fraction);
        double actual = (Math.Max(0.0, position.pixels) / itemWidth);
        double round = actual.roundToDouble();
        double item = default!;
        if ((((actual - round)).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            item = round;
        }
        else
        {
            item = actual;
        }
        if ((velocity < -((global::Doroti.Framework.Physics.Tolerance)tolerance).velocity))
        {
            item -= 0.5;
        }
        else
        {
            if ((velocity > ((global::Doroti.Framework.Physics.Tolerance)tolerance).velocity))
            {
                item += 0.5;
            }
        }
        return (item.roundToDouble() * itemWidth);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Physics.Simulation? createBallisticSimulation(global::Doroti.Framework.Widgets.ScrollMetrics position, double velocity)
    {
        DartRuntimePrimitives.Assert(() => (position is _CarouselPosition__carousel), () => (object?)"CarouselScrollPhysics can only be used with Scrollables that uses " + "the CarouselController");
        var metrics = ((_CarouselPosition__carousel?)(object?)position)!;
        if (((((velocity <= 0.0) && (metrics.pixels <= metrics.minScrollExtent))) || (((velocity >= 0.0) && (metrics.pixels >= metrics.maxScrollExtent)))))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)(object?)base.createBallisticSimulation(metrics, velocity));
        }
        global::Doroti.Framework.Physics.Tolerance toleranceLocal = ((global::Doroti.Framework.Physics.Tolerance)(object?)toleranceFor(metrics));
        double target = _getTargetPixels(metrics, toleranceLocal, velocity);
        if ((target != metrics.pixels))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)(object?)new global::Doroti.Framework.Physics.ScrollSpringSimulation(this.spring, metrics.pixels, target, velocity, tolerance: toleranceLocal));
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

    public virtual _CarouselMetrics__carousel copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, double? itemExtent = null, List<long>? flexWeights = null, bool? consumeMaxWeight = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new _CarouselMetrics__carousel(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), itemExtent: (itemExtent ?? this.itemExtent), flexWeights: (flexWeights ?? this.flexWeights), consumeMaxWeight: (consumeMaxWeight ?? this.consumeMaxWeight), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio));
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
        this._itemToShowOnStartup = initialItem.toDouble();
        this._consumeMaxWeight = DartRuntimePrimitives.RequireValue(consumeMaxWeight);
        this._infinite = infinite;
        this._itemCount = itemCount;
        System.Diagnostics.Debug.Assert((((flexWeights is not null) && (itemExtent is null)) || ((flexWeights is null) && (itemExtent is not null))));
    }

    public virtual long? itemCount
    {
        get => this._itemCount;
        set
        {
            var __value = value;
            if ((this._itemCount == __value))
            {
                return;
            }
            _itemCount = __value;
        }
    }
    public virtual bool infinite
    {
        get => this._infinite;
        set
        {
            var __value = value;
            if ((this._infinite == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            _infinite = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual bool consumeMaxWeight
    {
        get => this._consumeMaxWeight;
        set
        {
            var __value = value;
            if ((this._consumeMaxWeight == DartRuntimePrimitives.RequireValue(__value)))
            {
                return;
            }
            if ((this.hasPixels && (this.flexWeights is not null)))
            {
                double leadingItem = updateLeadingItem(this.flexWeights, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(__value)));
                double newPixel = getPixelsFromItem(leadingItem, this.flexWeights, this.itemExtent);
                forcePixels(newPixel);
            }
            _consumeMaxWeight = DartRuntimePrimitives.RequireValue(__value);
        }
    }
    public virtual double? itemExtent
    {
        get => this._itemExtent;
        set
        {
            var __value = value;
            if ((this._itemExtent == __value))
            {
                return;
            }
            if (((this.hasPixels && (this._itemExtent is not null)) && (this.viewportDimension != 0.0)))
            {
                double leadingItem = getItemFromPixels(DartRuntimePrimitives.RequireValue(this.pixels), DartRuntimePrimitives.RequireValue(this.viewportDimension));
                double newPixel = getPixelsFromItem(leadingItem, this.flexWeights, __value);
                forcePixels(newPixel);
            }
            _itemExtent = __value;
        }
    }
    public virtual List<long>? flexWeights
    {
        get => this._flexWeights;
        set
        {
            var __value = value;
            if ((object.Equals(this.flexWeights, __value)))
            {
                return;
            }
            List<long>? oldWeights = this._flexWeights.ToList();
            if ((this.hasPixels && (oldWeights is not null)))
            {
                double leadingItem = updateLeadingItem(__value, DartRuntimePrimitives.RequireValue(this.consumeMaxWeight));
                double newPixel = getPixelsFromItem(leadingItem, __value, this.itemExtent);
                forcePixels(newPixel);
            }
            _flexWeights = __value;
        }
    }
    public virtual long leadingItem
    {
        get
        {
            long leadingItem = getItemFromPixels(DartRuntimePrimitives.RequireValue(this.pixels), DartRuntimePrimitives.RequireValue(this.viewportDimension)).toInt();
            if ((this.consumeMaxWeight && (this.flexWeights is not null)))
            {
                leadingItem = Math.Max((leadingItem - ((long)((dynamic)this.flexWeights!).IndexOf(this.flexWeights!.max()))), 0L);
            }
            if (((this.infinite && (this.itemCount is not null)) && (DartRuntimePrimitives.RequireValue(this.itemCount) > 0L)))
            {
                long itemCount__value64303 = DartRuntimePrimitives.RequireValue(itemCount);
                leadingItem = (leadingItem % DartRuntimePrimitives.RequireValue(this.itemCount));
            }
            return leadingItem;
            return default!;
        }
    }
    public virtual double updateLeadingItem(List<long>? newFlexWeights, bool newConsumeMaxWeight)
    {
        double maxItem = default!;
        if ((this.hasPixels && (this.flexWeights is not null)))
        {
            double leadingItem = getItemFromPixels(DartRuntimePrimitives.RequireValue(this.pixels), DartRuntimePrimitives.RequireValue(this.viewportDimension));
            maxItem = (this.consumeMaxWeight ? leadingItem : (leadingItem + ((long)((dynamic)this.flexWeights!).IndexOf(this.flexWeights!.max()))));
        }
        else
        {
            if (!newConsumeMaxWeight)
            {
                return this._itemToShowOnStartup;
            }
            maxItem = this._itemToShowOnStartup;
        }
        if (((newFlexWeights is not null) && !newConsumeMaxWeight))
        {
            var smallerWeights = 0L;
            foreach (long weight in newFlexWeights)
            {
                if ((weight == newFlexWeights.max()))
                {
                    break;
                }
                smallerWeights += 1L;
            }
            return (maxItem - smallerWeights);
        }
        return maxItem;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getItemFromPixels(double pixels, double viewportDimension)
    {
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(viewportDimension) > 0.0));
        double fraction = default!;
        if ((this.itemExtent is not null))
        {
            double itemExtent__value65364 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction = (DartRuntimePrimitives.RequireValue(this.itemExtent) / DartRuntimePrimitives.RequireValue(viewportDimension));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this.flexWeights is not null));
            fraction = (this.flexWeights!.First() / this.flexWeights!.sum());
        }
        double actual = (Math.Max(0.0, DartRuntimePrimitives.RequireValue(pixels)) / ((DartRuntimePrimitives.RequireValue(viewportDimension) * fraction)));
        double round = actual.roundToDouble();
        if ((((actual - round)).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return round;
        }
        return actual;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getPixelsFromItem(double item, List<long>? flexWeights, double? itemExtent)
    {
        double fraction = default!;
        if ((this.viewportDimension == 0.0))
        {
            return 0.0;
        }
        if ((itemExtent is not null))
        {
            double itemExtent__value66023 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction = (DartRuntimePrimitives.RequireValue(itemExtent__value66023) / this.viewportDimension);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (flexWeights is not null));
            fraction = (flexWeights!.First() / flexWeights.sum());
        }
        return ((item * this.viewportDimension) * fraction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool applyViewportDimension(double viewportDimension)
    {
        double? oldViewportDimensions = (this.hasViewportDimension ? this.viewportDimension : null);
        if ((DartRuntimePrimitives.RequireValue(viewportDimension) == oldViewportDimensions))
        {
            return true;
        }
        bool result = base.applyViewportDimension(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(viewportDimension)));
        double? oldPixels = (this.hasPixels ? this.pixels : null);
        double item = default!;
        if ((oldPixels is null))
        {
            item = updateLeadingItem(this.flexWeights, DartRuntimePrimitives.RequireValue(this.consumeMaxWeight));
        }
        else
        {
            if ((oldViewportDimensions == 0.0))
            {
                item = DartRuntimePrimitives.RequireValue(this._cachedItem);
            }
            else
            {
                item = getItemFromPixels(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(oldPixels)), (oldViewportDimensions ?? DartRuntimePrimitives.RequireValue(viewportDimension)));
            }
        }
        double newPixels = getPixelsFromItem(item, this.flexWeights, this.itemExtent);
        _cachedItem = (((DartRuntimePrimitives.RequireValue(viewportDimension) == 0.0)) ? item : null);
        if ((newPixels != oldPixels))
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
        if ((other is not _CarouselPosition__carousel))
        {
            return;
        }
        _cachedItem = ((_CarouselPosition__carousel)((_CarouselPosition__carousel)other))._cachedItem;
        _itemExtent = ((_CarouselPosition__carousel)((_CarouselPosition__carousel)other))._itemExtent;
    }

    internal virtual double _getCycleLengthInPixels()
    {
        if (((((this.itemCount is null) || (DartRuntimePrimitives.RequireValue(this.itemCount) <= 0L)) || !this.hasViewportDimension) || (this.viewportDimension == 0L)))
        {
            return 0.0;
        }
        double fraction = default!;
        if ((this.itemExtent is not null))
        {
            double itemExtent__value67978 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction = (DartRuntimePrimitives.RequireValue(this.itemExtent) / this.viewportDimension);
        }
        else
        {
            if ((this.flexWeights is not null))
            {
                fraction = (this.flexWeights!.First() / this.flexWeights!.sum());
            }
            else
            {
                return 0.0;
            }
        }
        return ((DartRuntimePrimitives.RequireValue(this.itemCount) * this.viewportDimension) * fraction);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        if ((this.infinite && this.hasPixels))
        {
            double cycleLength = _getCycleLengthInPixels();
            if (((cycleLength > 0L) && (this.pixels < cycleLength)))
            {
                long cyclesToAdd = ((((cycleLength - this.pixels)) / cycleLength)).ceil();
                correctPixels((this.pixels + (cyclesToAdd * cycleLength)));
                return false;
            }
        }
        return base.applyContentDimensions((this.infinite ? 0.0 : DartRuntimePrimitives.RequireValue(minScrollExtent)), DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(maxScrollExtent)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _CarouselMetrics__carousel copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, double? itemExtent = null, List<long>? flexWeights = null, bool? consumeMaxWeight = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new _CarouselMetrics__carousel(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), itemExtent: (itemExtent ?? this.itemExtent), flexWeights: (flexWeights ?? this.flexWeights), consumeMaxWeight: (consumeMaxWeight ?? this.consumeMaxWeight), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio));
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
            DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.positions), () => (object?)"CarouselController.leadingItem cannot be accessed before a CarouselView is built with it.");
            DartRuntimePrimitives.Assert(() => (this.positions.Count() == 1L), () => (object?)"CarouselController.leadingItem cannot be read when multiple CarouselViews " + "are attached to the same controller.");
            return (((_CarouselPosition__carousel?)(object?)this.position)!).leadingItem;
            return default!;
        }
    }
    internal virtual void _attach(_CarouselViewState__carousel anchor)
    {
        _carouselState = anchor;
    }

    internal virtual void _detach(_CarouselViewState__carousel anchor)
    {
        if ((object.Equals(this._carouselState, anchor)))
        {
            _carouselState = null;
        }
    }

    public async virtual Future animateToItem(long index, Duration? duration = null, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        if ((!this.hasClients || (this._carouselState is null)))
        {
            return;
        }
        bool hasFlexWeights = ((this._carouselState!._flexWeights is { } __items72635 ? System.Linq.Enumerable.Any(__items72635) : (bool?)null) ?? false);
        if ((this._carouselState!.widget.itemBuilder is not null))
        {
            long? itemCountLocal = this._carouselState!.widget.itemCount;
            index = ((itemCountLocal is not null) ? index.clamp(0L, (DartRuntimePrimitives.RequireValue(itemCountLocal) - 1L)) : 0L);
        }
        else
        {
            index = index.clamp(0L, (checked((long)(this._carouselState!.widget.children.Count)) - 1L));
        }
        await global::Doroti.Runtime.DartAsyncRuntime.wait<object?>(((Func<List<Future>>)(() => { var __collection72994 = new List<Future>(); foreach (var position in this.positions.cast<_CarouselPosition__carousel>()) { __collection72994.Add(position.animateTo(_getTargetOffset(position, index, hasFlexWeights), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)), curve: curve)); } return __collection72994; }))());
    }

    internal virtual double _getTargetOffset(_CarouselPosition__carousel position, long index, bool hasFlexWeights)
    {
        if (!hasFlexWeights)
        {
            double targetInFirstCycle = (index * DartRuntimePrimitives.RequireValue(this._carouselState!._itemExtent));
            if (!this._carouselState!.widget.infinite)
            {
                return targetInFirstCycle;
            }
            return _adjustForInfiniteCycle(position, targetInFirstCycle);
        }
        _CarouselViewState__carousel carouselState = this._carouselState!;
        List<long> weights = ((_CarouselViewState__carousel)carouselState)._flexWeights!.ToList();
        long totalWeight = weights.reduce(((a, b) => (a + b)));
        double dimension = position.viewportDimension;
        long maxWeightIndex = ((long)((dynamic)weights).IndexOf(weights.max()));
        long leadingIndex = (((_CarouselViewState__carousel)carouselState)._consumeMaxWeight ? index : (index - maxWeightIndex));
        if ((carouselState.widget.itemBuilder is not null))
        {
            long? itemCountLocal = carouselState.widget.itemCount;
            leadingIndex = ((itemCountLocal is not null) ? leadingIndex.clamp(0L, (DartRuntimePrimitives.RequireValue(itemCountLocal) - 1L)) : 0L);
        }
        else
        {
            long itemCountAlternate = checked((long)(carouselState.widget.children.Count));
            leadingIndex = leadingIndex.clamp(0L, (itemCountAlternate - 1L));
        }
        double targetInFirstCycleLocal = ((dimension * ((weights.First() / totalWeight))) * leadingIndex);
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
        if ((cycleLength <= 0L))
        {
            return targetInFirstCycle;
        }
        double currentPixels = position.pixels;
        double currentCycleStart = (((currentPixels / cycleLength)).floorToDouble() * cycleLength);
        double sameCycleTarget = (currentCycleStart + targetInFirstCycle);
        if ((sameCycleTarget >= currentPixels))
        {
            return sameCycleTarget;
        }
        return (sameCycleTarget + cycleLength);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual long? _getItemCount()
    {
        if ((this._carouselState is null))
        {
            return null;
        }
        if ((this._carouselState!.widget.itemBuilder is not null))
        {
            return this._carouselState!.widget.itemCount;
        }
        return checked((long)(this._carouselState!.widget.children.Count));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.ScrollPosition createScrollPosition(global::Doroti.Framework.Widgets.ScrollPhysics physics, global::Doroti.Framework.Widgets.ScrollContext context, global::Doroti.Framework.Widgets.ScrollPosition? oldPosition)
    {
        DartRuntimePrimitives.Assert(() => (this._carouselState is not null));
        return ((global::Doroti.Framework.Widgets.ScrollPosition)(object?)new _CarouselPosition__carousel(physics: physics, context: context, initialItem: this.initialItem, itemExtent: this._carouselState!._itemExtent, consumeMaxWeight: this._carouselState!._consumeMaxWeight, flexWeights: this._carouselState!._flexWeights, infinite: this._carouselState!.widget.infinite, itemCount: _getItemCount(), oldPosition: oldPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(global::Doroti.Framework.Widgets.ScrollPosition position)
    {
        base.attach(position);
        var carouselPosition = ((_CarouselPosition__carousel?)(object?)position)!;
        carouselPosition.flexWeights = this._carouselState!._flexWeights;
        carouselPosition.itemExtent = this._carouselState!._itemExtent;
        carouselPosition.consumeMaxWeight = this._carouselState!._consumeMaxWeight;
        carouselPosition.infinite = this._carouselState!.widget.infinite;
        carouselPosition.itemCount = _getItemCount();
    }

}
