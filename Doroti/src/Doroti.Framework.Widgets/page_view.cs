// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/page_view.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public class PageController : ScrollController
{
    public virtual long initialPage { get; private set; } = default!;
    public virtual bool keepPage { get; private set; } = default!;
    public virtual double viewportFraction { get; private set; } = default!;

    public PageController(long initialPage = 0, bool keepPage = true, double viewportFraction = 1.0, Action<ScrollPosition>? onAttach = null, Action<ScrollPosition>? onDetach = null) : base(onAttach: onAttach, onDetach: onDetach)
    {
        this.initialPage = initialPage;
        this.keepPage = keepPage;
        this.viewportFraction = viewportFraction;
        System.Diagnostics.Debug.Assert(viewportFraction > 0.0);
    }

    public virtual double? page
    {
        get
        {
            DartRuntimePrimitives.Assert(() => Enumerable.Any(positions), () => (object?)"PageController.page cannot be accessed before a PageView is built with it.");
            DartRuntimePrimitives.Assert(() => positions.Count() == 1L, () => (object?)"The page property cannot be read when multiple PageViews are attached to " + "the same PageController.");
            var positionLocal = ((_PagePosition__page_view?)position)!;
            return positionLocal.page;
        }
    }
    internal virtual bool _debugCheckPageControllerAttached()
    {
        DartRuntimePrimitives.Assert(() => Enumerable.Any(positions), () => (object?)"PageController is not attached to a PageView.");
        DartRuntimePrimitives.Assert(() => positions.Count() == 1L, () => (object?)"Multiple PageViews are attached to " + "the same PageController.");
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future animateToPage(long page, Duration duration, Curve curve)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckPageControllerAttached());
        var positionLocal = ((_PagePosition__page_view?)position)!;
        if (positionLocal._cachedPage is not null)
        {
            positionLocal._cachedPage = page.toDouble();
            return Future.value();
        }
        if (!positionLocal.hasViewportDimension)
        {
            positionLocal._pageToUseOnStartup = page.toDouble();
            return Future.value();
        }
        return positionLocal.animateTo(positionLocal.getPixelsFromPage(page.toDouble()), duration: duration, curve: curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void jumpToPage(long page)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckPageControllerAttached());
        var positionLocal = ((_PagePosition__page_view?)position)!;
        if (positionLocal._cachedPage is not null)
        {
            positionLocal._cachedPage = page.toDouble();
            return;
        }
        if (!positionLocal.hasViewportDimension)
        {
            positionLocal._pageToUseOnStartup = page.toDouble();
            return;
        }
        positionLocal.jumpTo(positionLocal.getPixelsFromPage(page.toDouble()));
    }

    public virtual Future nextPage(Duration duration, Curve curve)
    {
        return animateToPage(DartRuntimePrimitives.RequireValue(page).round() + 1L, duration: duration, curve: curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future previousPage(Duration duration, Curve curve)
    {
        return animateToPage(DartRuntimePrimitives.RequireValue(page).round() - 1L, duration: duration, curve: curve);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ScrollPosition createScrollPosition(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition)
    {
        return new _PagePosition__page_view(physics: physics, context: context, initialPage: initialPage, keepPage: keepPage, viewportFraction: viewportFraction, oldPosition: oldPosition);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(ScrollPosition position)
    {
        base.attach(position);
        var pagePosition = ((_PagePosition__page_view?)position)!;
        pagePosition.viewportFraction = viewportFraction;
    }

}

public class PageMetrics : FixedScrollMetrics
{
    public virtual double viewportFraction { get; private set; } = default!;
    public PageMetrics() : base(default!, default!, default!, default!, default!, default!) { }


    public PageMetrics(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, AxisDirection axisDirection, double viewportFraction, double devicePixelRatio) : base(minScrollExtent: DartRuntimePrimitives.RequireValue(minScrollExtent), maxScrollExtent: DartRuntimePrimitives.RequireValue(maxScrollExtent), pixels: DartRuntimePrimitives.RequireValue(pixels), viewportDimension: DartRuntimePrimitives.RequireValue(viewportDimension), axisDirection: axisDirection, devicePixelRatio: devicePixelRatio)
    {
        this.viewportFraction = viewportFraction;
    }

    public override PageMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new PageMetrics(minScrollExtent: minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : null), maxScrollExtent: maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : null), pixels: pixels ?? (hasPixels ? this.pixels : null), viewportDimension: viewportDimension ?? (hasViewportDimension ? this.viewportDimension : null), axisDirection: axisDirection ?? this.axisDirection, viewportFraction: viewportFraction ?? this.viewportFraction, devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? page
    {
        get
        {
            return Math.Max(0.0, Dart_uiLibrary.clampDouble(pixels, minScrollExtent, maxScrollExtent)) / Math.Max(1.0, viewportDimension * viewportFraction);
        }
    }
}

internal class _PagePosition__page_view : ScrollPositionWithSingleContext
{
    public virtual long initialPage { get; private set; } = default!;
    internal virtual double _pageToUseOnStartup { get; set; } = default!;
    internal virtual double? _cachedPage { get; set; } = default;
    internal virtual double _viewportFraction { get; set; } = default!;

    internal _PagePosition__page_view(ScrollPhysics physics, ScrollContext context, long initialPage = 0, bool keepPage = true, double viewportFraction = 1.0, ScrollPosition? oldPosition = null) : base(physics: physics, context: context, oldPosition: oldPosition, initialPixels: null, keepScrollOffset: keepPage)
    {
        this.initialPage = initialPage;
        _viewportFraction = DartRuntimePrimitives.RequireValue(viewportFraction);
        _pageToUseOnStartup = initialPage.toDouble();
        System.Diagnostics.Debug.Assert(DartRuntimePrimitives.RequireValue(viewportFraction) > 0.0);
    }

    public override Future ensureVisible(RenderObject @object, double alignment = 0.0, Duration duration = default, Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, RenderObject? targetRenderObject = null)
    {
        return base.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double viewportFraction
    {
        get => _viewportFraction;
        set
        {
            var __value = value;
            if (_viewportFraction == __value)
            {
                return;
            }
            double? oldPage = page;
            _viewportFraction = __value;
            if (oldPage is not null)
            {
                double oldPage__12904__value12959 = DartRuntimePrimitives.RequireValue(oldPage);
                forcePixels(getPixelsFromPage(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(oldPage__12904__value12959))));
            }
        }
    }
    internal virtual double _initialPageOffset => Math.Max(0, viewportDimension * (viewportFraction - 1L) / 2L);
    public virtual double getPageFromPixels(double pixels, double viewportDimension)
    {
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.RequireValue(viewportDimension) > 0.0);
        double actual = Math.Max(0.0, DartRuntimePrimitives.RequireValue(pixels) - _initialPageOffset) / (DartRuntimePrimitives.RequireValue(viewportDimension) * viewportFraction);
        double round = actual.roundToDouble();
        if ((actual - round).abs() < Foundation.ConstantsLibrary.precisionErrorTolerance)
        {
            return round;
        }
        return actual;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getPixelsFromPage(double page)
    {
        return (page * viewportDimension * viewportFraction) + _initialPageOffset;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? page
    {
        get
        {
            if (!hasPixels)
            {
                return null;
            }
            DartRuntimePrimitives.Assert(() => hasContentDimensions || !haveDimensions, () => (object?)"Page value is only available after content dimensions are established.");
            return (hasContentDimensions || haveDimensions) ? (_cachedPage ?? (double)getPageFromPixels(Dart_uiLibrary.clampDouble(pixels, minScrollExtent, maxScrollExtent), DartRuntimePrimitives.RequireValue(viewportDimension))) : null;
        }
    }
    public override void saveScrollOffset()
    {
        PageStorage.maybeOf(context.storageContext)?.writeState(context.storageContext, _cachedPage ?? (double)getPageFromPixels(DartRuntimePrimitives.RequireValue(pixels), DartRuntimePrimitives.RequireValue(viewportDimension)));
    }

    public override void restoreScrollOffset()
    {
        if (!hasPixels)
        {
            var value = (double?)PageStorage.maybeOf(context.storageContext)?.readState(context.storageContext);
            if (value is not null)
            {
                double value__14735__value14854 = DartRuntimePrimitives.RequireValue(value);
                _pageToUseOnStartup = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value__14735__value14854));
            }
        }
    }

    public override void saveOffset()
    {
        context.saveOffset(_cachedPage ?? (double)getPageFromPixels(DartRuntimePrimitives.RequireValue(pixels), DartRuntimePrimitives.RequireValue(viewportDimension)));
    }

    public override void restoreOffset(double offset, bool initialRestore = false)
    {
        if (initialRestore)
        {
            _pageToUseOnStartup = offset;
        }
        else
        {
            jumpTo(getPixelsFromPage(offset));
        }
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
        double page = default!;
        if (oldPixels is null)
        {
            page = _pageToUseOnStartup;
        }
        else
        {
            if (oldViewportDimensions == 0.0)
            {
                page = DartRuntimePrimitives.RequireValue(_cachedPage);
            }
            else
            {
                page = getPageFromPixels(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(oldPixels)), DartRuntimePrimitives.RequireValue(oldViewportDimensions));
            }
        }
        double newPixels = getPixelsFromPage(DartRuntimePrimitives.RequireValue(page));
        _cachedPage = (DartRuntimePrimitives.RequireValue(viewportDimension) == 0.0) ? page : null;
        if (newPixels != oldPixels)
        {
            correctPixels(newPixels);
            return false;
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        DartRuntimePrimitives.Assert(() => _cachedPage is null);
        if (other is not _PagePosition__page_view)
        {
            return;
        }
        if (((_PagePosition__page_view)other)._cachedPage is not null)
        {
            _cachedPage = ((_PagePosition__page_view)other)._cachedPage;
        }
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        double newMinScrollExtent = DartRuntimePrimitives.RequireValue(minScrollExtent) + _initialPageOffset;
        return base.applyContentDimensions(newMinScrollExtent, Math.Max(newMinScrollExtent, DartRuntimePrimitives.RequireValue(maxScrollExtent) - _initialPageOffset));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override PageMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new PageMetrics(minScrollExtent: minScrollExtent ?? (hasContentDimensions ? this.minScrollExtent : null), maxScrollExtent: maxScrollExtent ?? (hasContentDimensions ? this.maxScrollExtent : null), pixels: pixels ?? (hasPixels ? this.pixels : null), viewportDimension: viewportDimension ?? (hasViewportDimension ? this.viewportDimension : null), axisDirection: axisDirection ?? this.axisDirection, viewportFraction: viewportFraction ?? this.viewportFraction, devicePixelRatio: devicePixelRatio ?? this.devicePixelRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ForceImplicitScrollPhysics__page_view : ScrollPhysics
{
    private bool __field_allowImplicitScrolling = default!;
    public override bool allowImplicitScrolling { get => __field_allowImplicitScrolling; }

    internal _ForceImplicitScrollPhysics__page_view(bool allowImplicitScrolling, ScrollPhysics? parent = null) : base(parent: parent)
    {
        __field_allowImplicitScrolling = allowImplicitScrolling;
    }

    public override _ForceImplicitScrollPhysics__page_view applyTo(ScrollPhysics? ancestor)
    {
        return new _ForceImplicitScrollPhysics__page_view(allowImplicitScrolling: allowImplicitScrolling, parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class PageScrollPhysics : ScrollPhysics
{
    public PageScrollPhysics(ScrollPhysics? parent = null) : base(parent: parent)
    {
    }

    public override PageScrollPhysics applyTo(ScrollPhysics? ancestor)
    {
        return new PageScrollPhysics(parent: buildParent(ancestor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getPage(ScrollMetrics position)
    {
        if (position is _PagePosition__page_view)
        {
            _PagePosition__page_view position__as18753 = (_PagePosition__page_view)position;
            return DartRuntimePrimitives.RequireValue(position__as18753.page);
        }
        return position.pixels / position.viewportDimension;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getPixels(ScrollMetrics position, double page)
    {
        if (position is _PagePosition__page_view)
        {
            _PagePosition__page_view position__as18946 = (_PagePosition__page_view)position;
            return position__as18946.getPixelsFromPage(page);
        }
        return page * position.viewportDimension;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getTargetPixels(ScrollMetrics position, Physics.Tolerance tolerance, double velocity)
    {
        double page = _getPage(position);
        if (velocity < -tolerance.velocity)
        {
            page -= 0.5;
        }
        else
        {
            if (velocity > tolerance.velocity)
            {
                page += 0.5;
            }
        }
        return _getPixels(position, page.roundToDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        if ((velocity <= 0.0) && (position.pixels <= position.minScrollExtent) || (velocity >= 0.0) && (position.pixels >= position.maxScrollExtent))
        {
            return base.createBallisticSimulation(position, velocity);
        }
        Physics.Tolerance toleranceLocal = toleranceFor(position);
        double target = _getTargetPixels(position, toleranceLocal, velocity);
        if (target != position.pixels)
        {
            return (Physics.Simulation?)new Physics.ScrollSpringSimulation(spring, position.pixels, target, velocity, tolerance: toleranceLocal);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool allowImplicitScrolling => false;
}

public static partial class Page_viewLibrary
{
    internal static PageScrollPhysics _kPagePhysics = new PageScrollPhysics();
}

public class PageView : StatefulWidget
{
    public virtual bool allowImplicitScrolling { get; private set; } = default!;
    public virtual ScrollCacheExtent scrollCacheExtent { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual PageController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual bool pageSnapping { get; private set; } = default!;
    public virtual Action<long>? onPageChanged { get; private set; }
    public virtual SliverChildDelegate childrenDelegate { get; private set; } = default!;
    public virtual DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual bool padEnds { get; private set; } = default!;

    public PageView(Key? key = null, Axis scrollDirection = Axis.horizontal, bool reverse = false, PageController? controller = null, ScrollPhysics? physics = null, bool pageSnapping = true, Action<long>? onPageChanged = null, List<Widget> children = default!, DragStartBehavior dragStartBehavior = DragStartBehavior.start, bool allowImplicitScrolling = false, ScrollCacheExtent? scrollCacheExtent = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, ScrollBehavior? scrollBehavior = null, bool padEnds = true) : base(key: key)
    {
        List<Widget> __children = children ?? new List<Widget>();
        this.scrollDirection = scrollDirection;
        this.reverse = reverse;
        this.controller = controller;
        this.physics = physics;
        this.pageSnapping = pageSnapping;
        this.onPageChanged = onPageChanged;
        this.dragStartBehavior = dragStartBehavior;
        this.allowImplicitScrolling = allowImplicitScrolling;
        this.restorationId = restorationId;
        this.clipBehavior = clipBehavior;
        this.hitTestBehavior = hitTestBehavior;
        this.scrollBehavior = scrollBehavior;
        this.padEnds = padEnds;
        this.scrollCacheExtent = scrollCacheExtent ?? ScrollCacheExtent.CreateViewport(allowImplicitScrolling ? 1.0 : 0.0);
        childrenDelegate = new SliverChildListDelegate(children ?? new List<Widget>());
        System.Diagnostics.Debug.Assert((scrollCacheExtent is null) || (scrollCacheExtent.value > 0.0 == allowImplicitScrolling));
    }

    public static PageView CreateBuilder(Key? key = null, Axis scrollDirection = Axis.horizontal, bool reverse = false, PageController? controller = null, ScrollPhysics? physics = null, bool pageSnapping = true, Action<long>? onPageChanged = null, Func<BuildContext, long, Widget?> itemBuilder = default!, Func<Key, long?>? findChildIndexCallback = null, long? itemCount = null, DragStartBehavior dragStartBehavior = DragStartBehavior.start, bool allowImplicitScrolling = false, ScrollCacheExtent? scrollCacheExtent = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, ScrollBehavior? scrollBehavior = null, bool padEnds = true)
    {
        var __instance = new PageView(key, scrollDirection, reverse, controller, physics, pageSnapping, onPageChanged, default!, dragStartBehavior, allowImplicitScrolling, scrollCacheExtent, restorationId, clipBehavior, hitTestBehavior, scrollBehavior, padEnds);
        __instance.scrollDirection = scrollDirection;
        __instance.reverse = reverse;
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.pageSnapping = pageSnapping;
        __instance.onPageChanged = onPageChanged;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.allowImplicitScrolling = allowImplicitScrolling;
        __instance.restorationId = restorationId;
        __instance.clipBehavior = clipBehavior;
        __instance.hitTestBehavior = hitTestBehavior;
        __instance.scrollBehavior = scrollBehavior;
        __instance.padEnds = padEnds;
        __instance.scrollCacheExtent = scrollCacheExtent ?? ScrollCacheExtent.CreateViewport(allowImplicitScrolling ? 1.0 : 0.0);
        __instance.childrenDelegate = new SliverChildBuilderDelegate(itemBuilder, findChildIndexCallback: findChildIndexCallback, childCount: itemCount);
        return __instance;
    }

    public static PageView CreateCustom(Key? key = null, Axis scrollDirection = Axis.horizontal, bool reverse = false, PageController? controller = null, ScrollPhysics? physics = null, bool pageSnapping = true, Action<long>? onPageChanged = null, SliverChildDelegate childrenDelegate = default!, DragStartBehavior dragStartBehavior = DragStartBehavior.start, bool allowImplicitScrolling = false, ScrollCacheExtent? scrollCacheExtent = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, HitTestBehavior hitTestBehavior = HitTestBehavior.opaque, ScrollBehavior? scrollBehavior = null, bool padEnds = true)
    {
        var __instance = new PageView(key, scrollDirection, reverse, controller, physics, pageSnapping, onPageChanged, default!, dragStartBehavior, allowImplicitScrolling, scrollCacheExtent, restorationId, clipBehavior, hitTestBehavior, scrollBehavior, padEnds);
        __instance.scrollDirection = scrollDirection;
        __instance.reverse = reverse;
        __instance.controller = controller;
        __instance.physics = physics;
        __instance.pageSnapping = pageSnapping;
        __instance.onPageChanged = onPageChanged;
        __instance.childrenDelegate = childrenDelegate;
        __instance.dragStartBehavior = dragStartBehavior;
        __instance.allowImplicitScrolling = allowImplicitScrolling;
        __instance.restorationId = restorationId;
        __instance.clipBehavior = clipBehavior;
        __instance.hitTestBehavior = hitTestBehavior;
        __instance.scrollBehavior = scrollBehavior;
        __instance.padEnds = padEnds;
        __instance.scrollCacheExtent = scrollCacheExtent ?? ScrollCacheExtent.CreateViewport(allowImplicitScrolling ? 1.0 : 0.0);
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _PageViewState__page_view());
}

internal class _PageViewState__page_view : State<PageView>
{
    internal virtual long _lastReportedPage { get; set; } = 0L;
    internal virtual PageController _controller { get; set; } = default!;

    public override void initState()
    {
        base.initState();
        _initController();
        _lastReportedPage = _controller.initialPage;
    }

    public override void dispose()
    {
        if (widget.controller is null)
        {
            _controller.dispose();
        }
        base.dispose();
    }

    internal virtual void _initController()
    {
        _controller = widget.controller ?? new PageController();
    }

    public override void didUpdateWidget(PageView oldWidget)
    {
        if (!Equals(oldWidget.controller, widget.controller))
        {
            if (oldWidget.controller is null)
            {
                _controller.dispose();
            }
            _initController();
        }
        base.didUpdateWidget(oldWidget);
    }

    internal virtual AxisDirection _getDirection(BuildContext context)
    {
        switch (widget.scrollDirection)
        {
            case Axis.horizontal:
                {
                    DartRuntimePrimitives.Assert(() => DebugLibrary.debugCheckHasDirectionality(context));
                    TextDirection textDirection = Directionality.of(context);
                    AxisDirection axisDirection = Basic_typesLibrary.textDirectionToAxisDirection(textDirection);
                    return widget.reverse ? Basic_typesLibrary.flipAxisDirection(axisDirection) : axisDirection;
                }
            case Axis.vertical:
                {
                    return widget.reverse ? AxisDirection.up : AxisDirection.down;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        AxisDirection axisDirectionLocal = _getDirection(context);
        ScrollPhysics physicsLocal = new _ForceImplicitScrollPhysics__page_view(allowImplicitScrolling: widget.allowImplicitScrolling).applyTo(widget.pageSnapping ? Page_viewLibrary._kPagePhysics.applyTo(widget.physics ?? (widget.scrollBehavior?.getScrollPhysics(context))) : (widget.physics ?? (widget.scrollBehavior?.getScrollPhysics(context))));
        return new NotificationListener<ScrollNotification>(onNotification: (notification) =>
        {
            if ((notification.depth == 0L) && (widget.onPageChanged is not null) && (notification is ScrollUpdateNotification))
            {
                var metricsLocal = ((PageMetrics?)((ScrollUpdateNotification)notification).metrics)!;
                long currentPage = DartRuntimePrimitives.RequireValue(metricsLocal.page).round();
                if (currentPage != _lastReportedPage)
                {
                    _lastReportedPage = currentPage;
                    widget.onPageChanged!(currentPage);
                }
            }
            return false;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: new Scrollable(dragStartBehavior: widget.dragStartBehavior, axisDirection: axisDirectionLocal, controller: _controller, physics: physicsLocal, restorationId: widget.restorationId, hitTestBehavior: widget.hitTestBehavior, scrollBehavior: widget.scrollBehavior ?? ScrollConfiguration.of(context).copyWith(scrollbars: false), viewportBuilder: (context, position) =>
        {
            return new Viewport(scrollCacheExtent: widget.scrollCacheExtent, axisDirection: axisDirectionLocal, offset: position, clipBehavior: widget.clipBehavior, slivers: new List<Widget> { new SliverFillViewport(viewportFraction: _controller.viewportFraction, @delegate: widget.childrenDelegate, padEnds: widget.padEnds, allowImplicitScrolling: widget.allowImplicitScrolling) });
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new EnumProperty<Axis>("scrollDirection", widget.scrollDirection));
        description.add(new FlagProperty("reverse", value: widget.reverse, ifTrue: "reversed"));
        description.add(new DiagnosticsProperty<PageController>("controller", _controller, showName: false));
        description.add(new DiagnosticsProperty<ScrollPhysics>("physics", widget.physics, showName: false));
        description.add(new FlagProperty("pageSnapping", value: widget.pageSnapping, ifFalse: "snapping disabled"));
        description.add(new FlagProperty("allowImplicitScrolling", value: widget.allowImplicitScrolling, ifTrue: "allow implicit scrolling"));
        description.add(new DiagnosticsProperty<ScrollCacheExtent>("scrollCacheExtent", widget.scrollCacheExtent, defaultValue: ScrollCacheExtent.CreateViewport(widget.allowImplicitScrolling ? 1.0 : 0.0)));
    }

}
