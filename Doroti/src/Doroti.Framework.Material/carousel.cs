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
        global::Doroti.Framework.Widgets.ScrollPosition position__22338 = this._controller.position;
        long currentLeadingIndex__22385 = (((_CarouselPosition__carousel?)(object?)position__22338)!).leadingItem;
        if ((currentLeadingIndex__22385 != this._lastReportedLeadingItem))
        {
            _lastReportedLeadingItem = currentLeadingIndex__22385;
            ((CarouselView)this.widget).onIndexChanged!(currentLeadingIndex__22385);
        }
    }

    internal virtual long _getInitialLeadingItem()
    {
        if ((((CarouselView)this.widget).flexWeights is not null))
        {
            long maxWeight__23108 = ((CarouselView)this.widget).flexWeights!.max();
            long firstMaxWeightIndex__23161 = ((long)((dynamic)((CarouselView)this.widget).flexWeights!).IndexOf(maxWeight__23108));
            return Math.Max((((CarouselController)this._controller).initialItem - firstMaxWeightIndex__23161), 0L);
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
        CarouselViewThemeData carouselTheme__23601 = CarouselViewTheme.of(this.context);
        ColorScheme colorScheme__23670 = ColorScheme.of(this.context);
        global::Doroti.Framework.Painting.EdgeInsets effectivePadding__23730 = ((((CarouselView)this.widget).padding ?? carouselTheme__23601.padding) ?? global::Doroti.Framework.Painting.EdgeInsets.CreateAll(4.0));
        global::Doroti.Ui.Color effectiveBackgroundColor__23843 = ((global::Doroti.Ui.Color)(object?)((((CarouselView)this.widget).backgroundColor ?? carouselTheme__23601.backgroundColor) ?? colorScheme__23670.surface));
        double effectiveElevation__23975 = ((((CarouselView)this.widget).elevation ?? carouselTheme__23601.elevation) ?? 0.0);
        global::Doroti.Framework.Painting.ShapeBorder effectiveShape__24070 = ((((CarouselView)this.widget).shape ?? carouselTheme__23601.shape) ?? new global::Doroti.Framework.Painting.RoundedRectangleBorder(borderRadius: global::Doroti.Framework.Painting.BorderRadius.CreateAll(global::Doroti.Ui.Radius.circular(28.0))));
        global::Doroti.Ui.Clip effectiveItemClipBehavior__24250 = ((((CarouselView)this.widget).itemClipBehavior ?? carouselTheme__23601.itemClipBehavior) ?? Clip.antiAlias);
        global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?> effectiveOverlayColor__24401 = ((global::Doroti.Framework.Widgets.WidgetStateProperty<global::Doroti.Ui.Color?>)(object?)(((((CarouselView)this.widget).overlayColor ?? carouselTheme__23601.overlayColor) ?? (global::Doroti.Framework.Widgets.WidgetStateProperty<Color?>)WidgetStateProperty.resolveWith(((global::System.Func<HashSet<global::Doroti.Framework.Widgets.WidgetState>, CarouselView>)((states) =>
        {
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.pressed))
            {
                return ((CarouselView)(object?)colorScheme__23670.onSurface.withOpacity(0.1));
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.hovered))
            {
                return ((CarouselView)(object?)colorScheme__23670.onSurface.withOpacity(0.08));
            }
            if (states.Contains(global::Doroti.Framework.Widgets.WidgetState.focused))
            {
                return ((CarouselView)(object?)colorScheme__23670.onSurface.withOpacity(0.1));
            }
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))));
        global::Doroti.Framework.Widgets.Widget contents__24985 = ((CarouselView)this.widget).children[(int)(index)];
        if (((CarouselView)this.widget).enableSplash)
        {
            contents__24985 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Stack(fit: global::Doroti.Framework.Rendering.StackFit.expand, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(contents__24985), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new Material(color: Colors.transparent, child: new InkWell(onTap: (() => { ((CarouselView)this.widget).onTap?.Invoke(index); }), overlayColor: effectiveOverlayColor__24401))) }));
        }
        else
        {
            if ((((CarouselView)this.widget).onTap is not null))
            {
                contents__24985 = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.GestureDetector(onTap: ((global::System.Action)(() => { ((CarouselView)this.widget).onTap!(index); })), child: contents__24985));
            }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: effectivePadding__23730, child: new Material(clipBehavior: effectiveItemClipBehavior__24250, color: effectiveBackgroundColor__23843, elevation: effectiveElevation__23975, shape: effectiveShape__24070, child: contents__24985)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildSliverCarousel(ThemeData theme)
    {
        long? childCount__25959 = (((CarouselView)this.widget).infinite ? null : ((((CarouselView)this.widget).itemBuilder is not null) ? ((CarouselView)this.widget).itemCount : checked((long)(((CarouselView)this.widget).children.Count))));
        global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?> effectiveBuilder__26135 = default!;
        if ((((CarouselView)this.widget).itemBuilder is not null))
        {
            if (((((CarouselView)this.widget).infinite && (((CarouselView)this.widget).itemCount is not null)) && (DartRuntimePrimitives.RequireValue(((CarouselView)this.widget).itemCount) > 0L)))
            {
                long itemCount__26291 = DartRuntimePrimitives.RequireValue(((CarouselView)this.widget).itemCount);
                effectiveBuilder__26135 = (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((context, index) =>
                {
                    return ((CarouselView)this.widget).itemBuilder!(context, (index % itemCount__26291));
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            }
            else
            {
                effectiveBuilder__26135 = (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((CarouselView)this.widget).itemBuilder!;
            }
        }
        else
        {
            effectiveBuilder__26135 = (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)((context, index) => _buildCarouselItem(index));
        }
        if ((this._itemExtent is not null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new _SliverFixedExtentCarousel__carousel(itemExtent: DartRuntimePrimitives.RequireValue(this._itemExtent), minExtent: ((CarouselView)this.widget).shrinkExtent, infinite: ((CarouselView)this.widget).infinite, @delegate: new global::Doroti.Framework.Widgets.SliverChildBuilderDelegate((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)effectiveBuilder__26135, childCount: childCount__25959)));
        }
        DartRuntimePrimitives.Assert(() => ((this._flexWeights is not null) && this._flexWeights!.All(((weight) => (weight > 0L)))), () => (object?)"flexWeights is null or it contains non-positive integers");
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new _SliverWeightedCarousel__carousel(consumeMaxWeight: this._consumeMaxWeight, shrinkExtent: ((CarouselView)this.widget).shrinkExtent, weights: this._flexWeights!, infinite: ((CarouselView)this.widget).infinite, @delegate: new global::Doroti.Framework.Widgets.SliverChildBuilderDelegate((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, long, global::Doroti.Framework.Widgets.Widget?>)effectiveBuilder__26135, childCount: childCount__25959)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__27443 = Theme.of(context);
        global::Doroti.Framework.Widgets.ScrollPhysics physics__27494 = (((CarouselView)this.widget).itemSnapping ? new CarouselScrollPhysics() : ScrollConfiguration.of(context).getScrollPhysics(context));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
        {
            double mainAxisExtent__27749 = (((CarouselView)this.widget).scrollDirection switch { global::Doroti.Framework.Painting.Axis.horizontal => ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth, global::Doroti.Framework.Painting.Axis.vertical => ((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight, _ when DartRuntimePrimitives.NonExhaustiveSwitchGuard => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            _itemExtent = ((((CarouselView)this.widget).itemExtent is null) ? null : Dart_uiLibrary.clampDouble(DartRuntimePrimitives.RequireValue(((CarouselView)this.widget).itemExtent), 0, mainAxisExtent__27749));
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.CustomScrollView(scrollDirection: ((CarouselView)this.widget).scrollDirection, reverse: ((CarouselView)this.widget).reverse, controller: this._controller, physics: physics__27494, clipBehavior: Clip.antiAlias, scrollCacheExtent: global::Doroti.Framework.Rendering.ScrollCacheExtent.CreateViewport(0.0), slivers: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_buildSliverCarousel(theme__27443)) }));
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
        var element__29647 = ((global::Doroti.Framework.Widgets.SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverFixedExtentCarousel__carousel(childManager: element__29647, minExtent: this.minExtent, maxExtent: this.itemExtent, infinite: this.infinite));
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
        long firstVisibleIndex__31222 = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.maxExtent)).floor();
        long offscreenItems__31377 = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.maxExtent)).floor();
        double offscreenExtent__31720 = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset - (offscreenItems__31377 * this.maxExtent));
        double effectiveMinExtent__32045 = Math.Max((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent % this.maxExtent), this.minExtent);
        if ((index == firstVisibleIndex__31222))
        {
            double effectiveExtent__32331 = (this.maxExtent - offscreenExtent__31720);
            return Math.Max(effectiveExtent__32331, effectiveMinExtent__32045);
        }
        double scrollOffsetForLastIndex__32462 = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset + ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent);
        if ((index == getMaxChildIndexForScrollOffset(scrollOffsetForLastIndex__32462, this.maxExtent)))
        {
            return Dart_uiLibrary.clampDouble((scrollOffsetForLastIndex__32462 - (this.maxExtent * index)), effectiveMinExtent__32045, this.maxExtent);
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
        long firstVisibleIndex__33199 = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.maxExtent)).floor();
        double effectiveMinExtent__33522 = Math.Max((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent % this.maxExtent), this.minExtent);
        if ((index == firstVisibleIndex__33199))
        {
            double firstVisibleItemExtent__33686 = _buildItemExtent(index, this.layoutDimensions);
            if ((firstVisibleItemExtent__33686 <= effectiveMinExtent__33522))
            {
                return (((this.maxExtent * index) - effectiveMinExtent__33522) + this.maxExtent);
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
        long firstVisibleIndex__34539 = ((scrollOffset / this.maxExtent)).floor();
        return Math.Max(firstVisibleIndex__34539, 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        if ((this.maxExtent > 0.0))
        {
            double actual__35031 = ((scrollOffset / this.maxExtent) - 1L);
            long round__35086 = actual__35031.round();
            if (((((actual__35031 * this.maxExtent) - (round__35086 * this.maxExtent))).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                return Math.Max(0L, round__35086);
            }
            return Math.Max(0L, actual__35031.ceil());
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
        var element__37363 = ((global::Doroti.Framework.Widgets.SliverMultiBoxAdaptorElement?)(object?)context)!;
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderSliverWeightedCarousel__carousel(childManager: element__37363, consumeMaxWeight: this.consumeMaxWeight, shrinkExtent: this.shrinkExtent, weights: this.weights, infinite: this.infinite));
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
        double extent__39795 = default!;
        if ((index == this._firstVisibleItemIndex))
        {
            extent__39795 = Math.Max(this._distanceToLeadingEdge, this.effectiveShrinkExtent);
        }
        else
        {
            if (((index > this._firstVisibleItemIndex) && (((index - this._firstVisibleItemIndex) + 1L) <= checked((long)(this.weights.Count)))))
            {
                DartRuntimePrimitives.Assert(() => ((index - this._firstVisibleItemIndex) < checked((long)(this.weights.Count))));
                long currIndexOnWeightList__40456 = (index - this._firstVisibleItemIndex);
                long currWeight__40528 = this.weights[(int)(currIndexOnWeightList__40456)];
                extent__39795 = (this.extentUnit * currWeight__40528);
                double progress__40650 = (this._firstVisibleItemOffscreenExtent / this.firstChildExtent);
                long prevWeight__40731 = this.weights[(int)((currIndexOnWeightList__40456 - 1L))];
                double finalIncrease__40799 = (((prevWeight__40731 - currWeight__40528)) / this.weights.max());
                extent__39795 = (extent__39795 + ((finalIncrease__40799 * progress__40650) * this.maxChildExtent));
            }
            else
            {
                if (((index > this._firstVisibleItemIndex) && (((index - this._firstVisibleItemIndex) + 1L) > checked((long)(this.weights.Count)))))
                {
                    double visibleItemsTotalExtent__41402 = this._distanceToLeadingEdge;
                    for (long i__41467 = (this._firstVisibleItemIndex + 1L); (i__41467 < index); i__41467++)
                    {
                        visibleItemsTotalExtent__41402 += _buildItemExtent(i__41467, currentLayoutDimensions);
                    }
                    extent__39795 = Math.Max((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).remainingPaintExtent - visibleItemsTotalExtent__41402), this.effectiveShrinkExtent);
                }
                else
                {
                    extent__39795 = Math.Max(this.minChildExtent, this.effectiveShrinkExtent);
                }
            }
        }
        return extent__39795;
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
            var smallerWeightCount__43049 = 0L;
            foreach (long weight__43092 in this.weights)
            {
                if ((weight__43092 == this.weights.max()))
                {
                    break;
                }
                smallerWeightCount__43049 += 1L;
            }
            long index__43216 = default!;
            double actual__43241 = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.firstChildExtent);
            long round__43309 = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.firstChildExtent)).round();
            if ((((actual__43241 - round__43309)).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                index__43216 = round__43309;
            }
            else
            {
                index__43216 = actual__43241.floor();
            }
            return (this.consumeMaxWeight ? (index__43216 - smallerWeightCount__43049) : index__43216);
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
            long index__44030 = default!;
            double actual__44054 = (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.firstChildExtent);
            long round__44122 = ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset / this.firstChildExtent)).round();
            if ((((actual__44054 - round__44122)).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
            {
                index__44030 = round__44122;
            }
            else
            {
                index__44030 = actual__44054.floor();
            }
            return (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset - (index__44030 * this.firstChildExtent));
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
        double visibleItemsTotalExtent__45372 = this._distanceToLeadingEdge;
        for (long i__45435 = (this._firstVisibleItemIndex + 1L); (i__45435 < index); i__45435++)
        {
            visibleItemsTotalExtent__45372 += _buildItemExtent(i__45435, this.layoutDimensions);
        }
        return (((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).scrollOffset + visibleItemsTotalExtent__45372);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMinChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        return Math.Max(this._firstVisibleItemIndex, 0L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override long getMaxChildIndexForScrollOffset(double scrollOffset, double itemExtent)
    {
        long? childCount__46235 = ((global::Doroti.Framework.Rendering.RenderSliverBoxChildManager)this.childManager).estimatedChildCount;
        if ((this.infinite && (childCount__46235 is null)))
        {
            double visibleItemsTotalExtent__46414 = this._distanceToLeadingEdge;
            long index__46474 = (this._firstVisibleItemIndex + 1L);
            double safeMinExtent__46701 = Math.Max(this.minChildExtent, 1.0);
            long estimatedUpperBound__46764 = (this._firstVisibleItemIndex + ((((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent / safeMinExtent__46701)).ceil());
            while (((visibleItemsTotalExtent__46414 < ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent) && (index__46474 < estimatedUpperBound__46764)))
            {
                visibleItemsTotalExtent__46414 += _buildItemExtent(index__46474, this.layoutDimensions);
                if ((visibleItemsTotalExtent__46414 >= ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent))
                {
                    return index__46474;
                }
                index__46474++;
            }
            return index__46474;
        }
        if ((childCount__46235 is not null))
        {
            long childCount__46235__value47249 = DartRuntimePrimitives.RequireValue(childCount__46235);
            double visibleItemsTotalExtent__47284 = this._distanceToLeadingEdge;
            for (long i__47349 = (this._firstVisibleItemIndex + 1L); (i__47349 < DartRuntimePrimitives.RequireValue(childCount__46235__value47249)); i__47349++)
            {
                visibleItemsTotalExtent__47284 += _buildItemExtent(i__47349, this.layoutDimensions);
                if ((visibleItemsTotalExtent__47284 >= ((global::Doroti.Framework.Rendering.SliverConstraints)this.constraints).viewportMainAxisExtent))
                {
                    return i__47349;
                }
            }
        }
        return (childCount__46235 ?? 0L);
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
        double extent__48086 = DartRuntimePrimitives.RequireValue(this.itemExtentBuilder!(index, this.layoutDimensions));
        return ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)this.constraints.asBoxConstraints(minExtent: extent__48086, maxExtent: extent__48086));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        DartRuntimePrimitives.Assert(() => ((((this.itemExtent is not null) && (this.itemExtentBuilder is null))) || (((this.itemExtent is null) && (this.itemExtentBuilder is not null)))));
        DartRuntimePrimitives.Assert(() => ((this.itemExtentBuilder is not null) || ((double.IsFinite(DartRuntimePrimitives.RequireValue(this.itemExtent)) && (DartRuntimePrimitives.RequireValue(this.itemExtent) >= 0L)))));
        global::Doroti.Framework.Rendering.SliverConstraints constraints__48887 = this.constraints;
        this.childManager.didStartLayout();
        this.childManager.setDidUnderflow(false);
        double scrollOffset__49013 = (((global::Doroti.Framework.Rendering.SliverConstraints)constraints__48887).scrollOffset + ((global::Doroti.Framework.Rendering.SliverConstraints)constraints__48887).cacheOrigin);
        DartRuntimePrimitives.Assert(() => (scrollOffset__49013 >= 0.0));
        double remainingExtent__49130 = ((global::Doroti.Framework.Rendering.SliverConstraints)constraints__48887).remainingCacheExtent;
        DartRuntimePrimitives.Assert(() => (remainingExtent__49130 >= 0.0));
        double targetEndScrollOffset__49235 = (scrollOffset__49013 + remainingExtent__49130);
        double deprecatedExtraItemExtent__49364 = -1;
        long firstIndex__49411 = getMinChildIndexForScrollOffset(scrollOffset__49013, deprecatedExtraItemExtent__49364);
        long? targetLastIndex__49513 = (double.IsFinite(targetEndScrollOffset__49235) ? getMaxChildIndexForScrollOffset(targetEndScrollOffset__49235, deprecatedExtraItemExtent__49364) : null);
        if ((this.firstChild is not null))
        {
            long leadingGarbage__49717 = calculateLeadingGarbage(firstIndex: firstIndex__49411);
            long trailingGarbage__49799 = ((targetLastIndex__49513 is not null) ? calculateTrailingGarbage(lastIndex: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetLastIndex__49513))) : 0L);
            collectGarbage(leadingGarbage__49717, trailingGarbage__49799);
        }
        else
        {
            collectGarbage(0L, 0L);
        }
        if ((this.firstChild is null))
        {
            double layoutOffset__50073 = indexToLayoutOffset(deprecatedExtraItemExtent__49364, firstIndex__49411);
            if (!addInitialChild(index: firstIndex__49411, layoutOffset: layoutOffset__50073))
            {
                double max__50331 = default!;
                if ((firstIndex__49411 <= 0L))
                {
                    max__50331 = 0.0;
                }
                else
                {
                    max__50331 = computeMaxScrollOffset(constraints__48887, deprecatedExtraItemExtent__49364);
                }
                geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: max__50331, maxPaintExtent: max__50331);
                this.childManager.didFinishLayout();
                return;
            }
        }
        global::Doroti.Framework.Rendering.RenderBox? trailingChildWithLayout__50656 = default!;
        for (long index__50695 = (indexOf(this.firstChild!) - 1L); (index__50695 >= firstIndex__49411); --index__50695)
        {
            global::Doroti.Framework.Rendering.RenderBox? child__50784 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)insertAndLayoutLeadingChild(_getChildConstraints(index__50695)));
            if ((child__50784 is null))
            {
                geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollOffsetCorrection: indexToLayoutOffset(deprecatedExtraItemExtent__49364, index__50695));
                return;
            }
            var childParentData__51255 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)child__50784.parentData!)!;
            childParentData__51255.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent__49364, index__50695);
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData)childParentData__51255).index == index__50695));
            trailingChildWithLayout__50656 ??= child__50784;
        }
        if ((trailingChildWithLayout__50656 is null))
        {
            this.firstChild!.layout(_getChildConstraints(indexOf(this.firstChild!)));
            var childParentData__51638 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)this.firstChild!.parentData!)!;
            childParentData__51638.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent__49364, firstIndex__49411);
            trailingChildWithLayout__50656 = this.firstChild;
        }
        double extraLayoutOffset__51937 = 0;
        if (this.consumeMaxWeight)
        {
            for (long i__52003 = (checked((long)(this.weights.Count)) - 1L); (i__52003 >= 0L); i__52003--)
            {
                if ((this.weights[(int)(i__52003)] == this.weights.max()))
                {
                    break;
                }
                extraLayoutOffset__51937 += (this.weights[(int)(i__52003)] * this.extentUnit);
            }
        }
        double estimatedMaxScrollOffset__52190 = double.PositiveInfinity;
        for (long index__52312 = (indexOf(trailingChildWithLayout__50656!) + 1L); ((targetLastIndex__49513 is null) || (index__52312 <= DartRuntimePrimitives.RequireValue(targetLastIndex__49513))); ++index__52312)
        {
            global::Doroti.Framework.Rendering.RenderBox? child__52457 = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)childAfter(trailingChildWithLayout__50656!));
            if (((child__52457 is null) || (indexOf(child__52457) != index__52312)))
            {
                child__52457 = insertAndLayoutChild(_getChildConstraints(index__52312), after: trailingChildWithLayout__50656);
                if ((child__52457 is null))
                {
                    estimatedMaxScrollOffset__52190 = (indexToLayoutOffset(deprecatedExtraItemExtent__49364, index__52312) + extraLayoutOffset__51937);
                    break;
                }
            }
            else
            {
                child__52457.layout(_getChildConstraints(index__52312));
            }
            trailingChildWithLayout__50656 = child__52457;
            var childParentData__53005 = ((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData?)(object?)child__52457.parentData!)!;
            DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData)childParentData__53005).index == index__52312));
            childParentData__53005.layoutOffset = indexToLayoutOffset(deprecatedExtraItemExtent__49364, DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Rendering.SliverMultiBoxAdaptorParentData)childParentData__53005).index));
        }
        long lastIndex__53278 = indexOf(this.lastChild!);
        double leadingScrollOffset__53328 = indexToLayoutOffset(deprecatedExtraItemExtent__49364, firstIndex__49411);
        double trailingScrollOffset__53421 = default!;
        if ((!this.infinite && ((lastIndex__53278 + 1L) == ((global::Doroti.Framework.Rendering.RenderSliverBoxChildManager)this.childManager).childCount)))
        {
            trailingScrollOffset__53421 = indexToLayoutOffset(deprecatedExtraItemExtent__49364, lastIndex__53278);
            trailingScrollOffset__53421 += Math.Max((this.weights.Last() * this.extentUnit), _buildItemExtent(lastIndex__53278, this.layoutDimensions));
            trailingScrollOffset__53421 += extraLayoutOffset__51937;
        }
        else
        {
            trailingScrollOffset__53421 = indexToLayoutOffset(deprecatedExtraItemExtent__49364, (lastIndex__53278 + 1L));
        }
        DartRuntimePrimitives.Assert(() => debugAssertChildListIsNonEmptyAndContiguous());
        DartRuntimePrimitives.Assert(() => (indexOf(this.firstChild!) == firstIndex__49411));
        DartRuntimePrimitives.Assert(() => ((targetLastIndex__49513 is null) || (lastIndex__53278 <= DartRuntimePrimitives.RequireValue(targetLastIndex__49513))));
        estimatedMaxScrollOffset__52190 = Math.Min(estimatedMaxScrollOffset__52190, estimateMaxScrollOffset(constraints__48887, firstIndex: firstIndex__49411, lastIndex: lastIndex__53278, leadingScrollOffset: leadingScrollOffset__53328, trailingScrollOffset: trailingScrollOffset__53421));
        double paintExtent__54398 = calculatePaintOffset(constraints__48887, from: (this.consumeMaxWeight ? 0 : leadingScrollOffset__53328), to: trailingScrollOffset__53421);
        double cacheExtent__54566 = calculateCacheOffset(constraints__48887, from: (this.consumeMaxWeight ? 0 : leadingScrollOffset__53328), to: trailingScrollOffset__53421);
        double targetEndScrollOffsetForPaint__54734 = (((global::Doroti.Framework.Rendering.SliverConstraints)constraints__48887).scrollOffset + ((global::Doroti.Framework.Rendering.SliverConstraints)constraints__48887).remainingPaintExtent);
        long? targetLastIndexForPaint__54850 = (double.IsFinite(targetEndScrollOffsetForPaint__54734) ? getMaxChildIndexForScrollOffset(targetEndScrollOffsetForPaint__54734, deprecatedExtraItemExtent__49364) : null);
        geometry = new global::Doroti.Framework.Rendering.SliverGeometry(scrollExtent: estimatedMaxScrollOffset__52190, paintExtent: paintExtent__54398, cacheExtent: cacheExtent__54566, maxPaintExtent: estimatedMaxScrollOffset__52190, hasVisualOverflow: ((((targetLastIndexForPaint__54850 is not null) && (lastIndex__53278 >= DartRuntimePrimitives.RequireValue(targetLastIndexForPaint__54850)))) || (((global::Doroti.Framework.Rendering.SliverConstraints)constraints__48887).scrollOffset > 0.0)));
        if ((estimatedMaxScrollOffset__52190 == trailingScrollOffset__53421))
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
        double fraction__56695 = default!;
        if ((((_CarouselPosition__carousel)position).itemExtent is not null))
        {
            fraction__56695 = (DartRuntimePrimitives.RequireValue(((_CarouselPosition__carousel)position).itemExtent) / position.viewportDimension);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (((_CarouselPosition__carousel)position).flexWeights is not null));
            fraction__56695 = (((_CarouselPosition__carousel)position).flexWeights!.First() / ((_CarouselPosition__carousel)position).flexWeights!.sum());
        }
        double itemWidth__56968 = (position.viewportDimension * fraction__56695);
        double actual__57037 = (Math.Max(0.0, position.pixels) / itemWidth__56968);
        double round__57107 = actual__57037.roundToDouble();
        double item__57150 = default!;
        if ((((actual__57037 - round__57107)).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            item__57150 = round__57107;
        }
        else
        {
            item__57150 = actual__57037;
        }
        if ((velocity < -((global::Doroti.Framework.Physics.Tolerance)tolerance).velocity))
        {
            item__57150 -= 0.5;
        }
        else
        {
            if ((velocity > ((global::Doroti.Framework.Physics.Tolerance)tolerance).velocity))
            {
                item__57150 += 0.5;
            }
        }
        return (item__57150.roundToDouble() * itemWidth__56968);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Physics.Simulation? createBallisticSimulation(global::Doroti.Framework.Widgets.ScrollMetrics position, double velocity)
    {
        DartRuntimePrimitives.Assert(() => (position is _CarouselPosition__carousel), () => (object?)"CarouselScrollPhysics can only be used with Scrollables that uses " + "the CarouselController");
        var metrics__57729 = ((_CarouselPosition__carousel?)(object?)position)!;
        if (((((velocity <= 0.0) && (metrics__57729.pixels <= metrics__57729.minScrollExtent))) || (((velocity >= 0.0) && (metrics__57729.pixels >= metrics__57729.maxScrollExtent)))))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)(object?)base.createBallisticSimulation(metrics__57729, velocity));
        }
        global::Doroti.Framework.Physics.Tolerance tolerance__58010 = ((global::Doroti.Framework.Physics.Tolerance)(object?)toleranceFor(metrics__57729));
        double target__58062 = _getTargetPixels(metrics__57729, tolerance__58010, velocity);
        if ((target__58062 != metrics__57729.pixels))
        {
            return ((global::Doroti.Framework.Physics.Simulation?)(object?)new global::Doroti.Framework.Physics.ScrollSpringSimulation(this.spring, metrics__57729.pixels, target__58062, velocity, tolerance: tolerance__58010));
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
                double leadingItem__61950 = updateLeadingItem(this.flexWeights, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(__value)));
                double newPixel__62022 = getPixelsFromItem(leadingItem__61950, this.flexWeights, this.itemExtent);
                forcePixels(newPixel__62022);
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
                double leadingItem__62414 = getItemFromPixels(DartRuntimePrimitives.RequireValue(this.pixels), DartRuntimePrimitives.RequireValue(this.viewportDimension));
                double newPixel__62493 = getPixelsFromItem(leadingItem__62414, this.flexWeights, __value);
                forcePixels(newPixel__62493);
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
            List<long>? oldWeights__62817 = this._flexWeights.ToList();
            if ((this.hasPixels && (oldWeights__62817 is not null)))
            {
                double leadingItem__62906 = updateLeadingItem(__value, DartRuntimePrimitives.RequireValue(this.consumeMaxWeight));
                double newPixel__62983 = getPixelsFromItem(leadingItem__62906, __value, this.itemExtent);
                forcePixels(newPixel__62983);
            }
            _flexWeights = __value;
        }
    }
    public virtual long leadingItem
    {
        get
        {
            long leadingItem__63423 = getItemFromPixels(DartRuntimePrimitives.RequireValue(this.pixels), DartRuntimePrimitives.RequireValue(this.viewportDimension)).toInt();
            if ((this.consumeMaxWeight && (this.flexWeights is not null)))
            {
                leadingItem__63423 = Math.Max((leadingItem__63423 - ((long)((dynamic)this.flexWeights!).IndexOf(this.flexWeights!.max()))), 0L);
            }
            if (((this.infinite && (this.itemCount is not null)) && (DartRuntimePrimitives.RequireValue(this.itemCount) > 0L)))
            {
                long itemCount__value64303 = DartRuntimePrimitives.RequireValue(itemCount);
                leadingItem__63423 = (leadingItem__63423 % DartRuntimePrimitives.RequireValue(this.itemCount));
            }
            return leadingItem__63423;
            return default!;
        }
    }
    public virtual double updateLeadingItem(List<long>? newFlexWeights, bool newConsumeMaxWeight)
    {
        double maxItem__64522 = default!;
        if ((this.hasPixels && (this.flexWeights is not null)))
        {
            double leadingItem__64594 = getItemFromPixels(DartRuntimePrimitives.RequireValue(this.pixels), DartRuntimePrimitives.RequireValue(this.viewportDimension));
            maxItem__64522 = (this.consumeMaxWeight ? leadingItem__64594 : (leadingItem__64594 + ((long)((dynamic)this.flexWeights!).IndexOf(this.flexWeights!.max()))));
        }
        else
        {
            if (!newConsumeMaxWeight)
            {
                return this._itemToShowOnStartup;
            }
            maxItem__64522 = this._itemToShowOnStartup;
        }
        if (((newFlexWeights is not null) && !newConsumeMaxWeight))
        {
            var smallerWeights__64981 = 0L;
            foreach (long weight__65022 in newFlexWeights)
            {
                if ((weight__65022 == newFlexWeights.max()))
                {
                    break;
                }
                smallerWeights__64981 += 1L;
            }
            return (maxItem__64522 - smallerWeights__64981);
        }
        return maxItem__64522;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getItemFromPixels(double pixels, double viewportDimension)
    {
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(viewportDimension) > 0.0));
        double fraction__65346 = default!;
        if ((this.itemExtent is not null))
        {
            double itemExtent__value65364 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction__65346 = (DartRuntimePrimitives.RequireValue(this.itemExtent) / DartRuntimePrimitives.RequireValue(viewportDimension));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (this.flexWeights is not null));
            fraction__65346 = (this.flexWeights!.First() / this.flexWeights!.sum());
        }
        double actual__65624 = (Math.Max(0.0, DartRuntimePrimitives.RequireValue(pixels)) / ((DartRuntimePrimitives.RequireValue(viewportDimension) * fraction__65346)));
        double round__65706 = actual__65624.roundToDouble();
        if ((((actual__65624 - round__65706)).abs() < global::Doroti.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return round__65706;
        }
        return actual__65624;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getPixelsFromItem(double item, List<long>? flexWeights, double? itemExtent)
    {
        double fraction__65945 = default!;
        if ((this.viewportDimension == 0.0))
        {
            return 0.0;
        }
        if ((itemExtent is not null))
        {
            double itemExtent__value66023 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction__65945 = (DartRuntimePrimitives.RequireValue(itemExtent__value66023) / this.viewportDimension);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (flexWeights is not null));
            fraction__65945 = (flexWeights!.First() / flexWeights.sum());
        }
        return ((item * this.viewportDimension) * fraction__65945);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool applyViewportDimension(double viewportDimension)
    {
        double? oldViewportDimensions__66405 = (this.hasViewportDimension ? this.viewportDimension : null);
        if ((DartRuntimePrimitives.RequireValue(viewportDimension) == oldViewportDimensions__66405))
        {
            return true;
        }
        bool result__66577 = base.applyViewportDimension(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(viewportDimension)));
        double? oldPixels__66653 = (this.hasPixels ? this.pixels : null);
        double item__66703 = default!;
        if ((oldPixels__66653 is null))
        {
            item__66703 = updateLeadingItem(this.flexWeights, DartRuntimePrimitives.RequireValue(this.consumeMaxWeight));
        }
        else
        {
            if ((oldViewportDimensions__66405 == 0.0))
            {
                item__66703 = DartRuntimePrimitives.RequireValue(this._cachedItem);
            }
            else
            {
                item__66703 = getItemFromPixels(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(oldPixels__66653)), (oldViewportDimensions__66405 ?? DartRuntimePrimitives.RequireValue(viewportDimension)));
            }
        }
        double newPixels__67080 = getPixelsFromItem(item__66703, this.flexWeights, this.itemExtent);
        _cachedItem = (((DartRuntimePrimitives.RequireValue(viewportDimension) == 0.0)) ? item__66703 : null);
        if ((newPixels__67080 != oldPixels__66653))
        {
            correctPixels(newPixels__67080);
            return false;
        }
        return result__66577;
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
        double fraction__67960 = default!;
        if ((this.itemExtent is not null))
        {
            double itemExtent__value67978 = DartRuntimePrimitives.RequireValue(itemExtent);
            fraction__67960 = (DartRuntimePrimitives.RequireValue(this.itemExtent) / this.viewportDimension);
        }
        else
        {
            if ((this.flexWeights is not null))
            {
                fraction__67960 = (this.flexWeights!.First() / this.flexWeights!.sum());
            }
            else
            {
                return 0.0;
            }
        }
        return ((DartRuntimePrimitives.RequireValue(this.itemCount) * this.viewportDimension) * fraction__67960);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        if ((this.infinite && this.hasPixels))
        {
            double cycleLength__68540 = _getCycleLengthInPixels();
            if (((cycleLength__68540 > 0L) && (this.pixels < cycleLength__68540)))
            {
                long cyclesToAdd__68819 = ((((cycleLength__68540 - this.pixels)) / cycleLength__68540)).ceil();
                correctPixels((this.pixels + (cyclesToAdd__68819 * cycleLength__68540)));
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
        bool hasFlexWeights__72618 = ((this._carouselState!._flexWeights is { } __items72635 ? System.Linq.Enumerable.Any(__items72635) : (bool?)null) ?? false);
        if ((this._carouselState!.widget.itemBuilder is not null))
        {
            long? itemCount__72757 = this._carouselState!.widget.itemCount;
            index = ((itemCount__72757 is not null) ? index.clamp(0L, (DartRuntimePrimitives.RequireValue(itemCount__72757) - 1L)) : 0L);
        }
        else
        {
            index = index.clamp(0L, (checked((long)(this._carouselState!.widget.children.Count)) - 1L));
        }
        await global::Doroti.Runtime.DartAsyncRuntime.wait<object?>(((Func<List<Future>>)(() => { var __collection72994 = new List<Future>(); foreach (var position__73045 in this.positions.cast<_CarouselPosition__carousel>()) { __collection72994.Add(position__73045.animateTo(_getTargetOffset(position__73045, index, hasFlexWeights__72618), duration: DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(duration)), curve: curve)); } return __collection72994; }))());
    }

    internal virtual double _getTargetOffset(_CarouselPosition__carousel position, long index, bool hasFlexWeights)
    {
        if (!hasFlexWeights)
        {
            double targetInFirstCycle__73395 = (index * DartRuntimePrimitives.RequireValue(this._carouselState!._itemExtent));
            if (!this._carouselState!.widget.infinite)
            {
                return targetInFirstCycle__73395;
            }
            return _adjustForInfiniteCycle(position, targetInFirstCycle__73395);
        }
        _CarouselViewState__carousel carouselState__73647 = this._carouselState!;
        List<long> weights__73700 = ((_CarouselViewState__carousel)carouselState__73647)._flexWeights!.ToList();
        long totalWeight__73753 = weights__73700.reduce(((a, b) => (a + b)));
        double dimension__73825 = position.viewportDimension;
        long maxWeightIndex__73880 = ((long)((dynamic)weights__73700).IndexOf(weights__73700.max()));
        long leadingIndex__73935 = (((_CarouselViewState__carousel)carouselState__73647)._consumeMaxWeight ? index : (index - maxWeightIndex__73880));
        if ((carouselState__73647.widget.itemBuilder is not null))
        {
            long? itemCount__74085 = carouselState__73647.widget.itemCount;
            leadingIndex__73935 = ((itemCount__74085 is not null) ? leadingIndex__73935.clamp(0L, (DartRuntimePrimitives.RequireValue(itemCount__74085) - 1L)) : 0L);
        }
        else
        {
            long itemCount__74241 = checked((long)(carouselState__73647.widget.children.Count));
            leadingIndex__73935 = leadingIndex__73935.clamp(0L, (itemCount__74241 - 1L));
        }
        double targetInFirstCycle__74374 = ((dimension__73825 * ((weights__73700.First() / totalWeight__73753))) * leadingIndex__73935);
        if (!carouselState__73647.widget.infinite)
        {
            return targetInFirstCycle__74374;
        }
        return _adjustForInfiniteCycle(position, targetInFirstCycle__74374);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _adjustForInfiniteCycle(_CarouselPosition__carousel position, double targetInFirstCycle)
    {
        double cycleLength__75054 = position._getCycleLengthInPixels();
        if ((cycleLength__75054 <= 0L))
        {
            return targetInFirstCycle;
        }
        double currentPixels__75188 = position.pixels;
        double currentCycleStart__75295 = (((currentPixels__75188 / cycleLength__75054)).floorToDouble() * cycleLength__75054);
        double sameCycleTarget__75460 = (currentCycleStart__75295 + targetInFirstCycle);
        if ((sameCycleTarget__75460 >= currentPixels__75188))
        {
            return sameCycleTarget__75460;
        }
        return (sameCycleTarget__75460 + cycleLength__75054);
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
        var carouselPosition__76641 = ((_CarouselPosition__carousel?)(object?)position)!;
        carouselPosition__76641.flexWeights = this._carouselState!._flexWeights;
        carouselPosition__76641.itemExtent = this._carouselState!._itemExtent;
        carouselPosition__76641.consumeMaxWeight = this._carouselState!._consumeMaxWeight;
        carouselPosition__76641.infinite = this._carouselState!.widget.infinite;
        carouselPosition__76641.itemCount = _getItemCount();
    }

}
