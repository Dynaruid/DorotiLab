// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/page_view.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public class PageController : ScrollController
{
    public virtual long initialPage { get; private set; } = default!;
    public virtual bool keepPage { get; private set; } = default!;
    public virtual double viewportFraction { get; private set; } = default!;

    public PageController(long initialPage = 0, bool keepPage = true, double viewportFraction = 1.0, global::System.Action<ScrollPosition>? onAttach = null, global::System.Action<ScrollPosition>? onDetach = null) : base(onAttach: onAttach, onDetach: onDetach)
    {
        this.initialPage = initialPage;
        this.keepPage = keepPage;
        this.viewportFraction = viewportFraction;
        System.Diagnostics.Debug.Assert((viewportFraction > 0.0));
    }

    public virtual double? page
    {
        get
        {
            DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.positions), () => (object?)"PageController.page cannot be accessed before a PageView is built with it.");
            DartRuntimePrimitives.Assert(() => (this.positions.Count() == 1L), () => (object?)"The page property cannot be read when multiple PageViews are attached to " + "the same PageController.");
            var position__6263 = ((_PagePosition__page_view?)(object?)this.position)!;
            return ((_PagePosition__page_view)position__6263).page;
            return default!;
        }
    }
    internal virtual bool _debugCheckPageControllerAttached()
    {
        DartRuntimePrimitives.Assert(() => System.Linq.Enumerable.Any(this.positions), () => (object?)"PageController is not attached to a PageView.");
        DartRuntimePrimitives.Assert(() => (this.positions.Count() == 1L), () => (object?)"Multiple PageViews are attached to " + "the same PageController.");
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future animateToPage(long page, Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckPageControllerAttached());
        var position__7018 = ((_PagePosition__page_view?)(object?)this.position)!;
        if ((((_PagePosition__page_view)position__7018)._cachedPage is not null))
        {
            position__7018._cachedPage = page.toDouble();
            return Future.value();
        }
        if (!position__7018.hasViewportDimension)
        {
            position__7018._pageToUseOnStartup = page.toDouble();
            return Future.value();
        }
        return ((Future)(object?)position__7018.animateTo(position__7018.getPixelsFromPage(page.toDouble()), duration: duration, curve: curve));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void jumpToPage(long page)
    {
        DartRuntimePrimitives.Assert(() => _debugCheckPageControllerAttached());
        var position__7779 = ((_PagePosition__page_view?)(object?)this.position)!;
        if ((((_PagePosition__page_view)position__7779)._cachedPage is not null))
        {
            position__7779._cachedPage = page.toDouble();
            return;
        }
        if (!position__7779.hasViewportDimension)
        {
            position__7779._pageToUseOnStartup = page.toDouble();
            return;
        }
        position__7779.jumpTo(position__7779.getPixelsFromPage(page.toDouble()));
    }

    public virtual Future nextPage(Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve)
    {
        return ((Future)(object?)animateToPage((DartRuntimePrimitives.RequireValue(this.page).round() + 1L), duration: duration, curve: curve));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future previousPage(Duration duration, global::Doroti.Generated.Framework.Animation.Curve curve)
    {
        return ((Future)(object?)animateToPage((DartRuntimePrimitives.RequireValue(this.page).round() - 1L), duration: duration, curve: curve));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ScrollPosition createScrollPosition(ScrollPhysics physics, ScrollContext context, ScrollPosition? oldPosition)
    {
        return ((ScrollPosition)(object?)new _PagePosition__page_view(physics: physics, context: context, initialPage: this.initialPage, keepPage: this.keepPage, viewportFraction: this.viewportFraction, oldPosition: oldPosition));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void attach(ScrollPosition position)
    {
        base.attach(position);
        var pagePosition__9362 = ((_PagePosition__page_view?)(object?)position)!;
        pagePosition__9362.viewportFraction = this.viewportFraction;
    }

}

public class PageMetrics : FixedScrollMetrics
{
    public virtual double viewportFraction { get; private set; } = default!;
    public PageMetrics() : base(default!, default!, default!, default!, default!, default!) { }


    public PageMetrics(double? minScrollExtent, double? maxScrollExtent, double? pixels, double? viewportDimension, global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection, double viewportFraction, double devicePixelRatio) : base(minScrollExtent: DartRuntimePrimitives.RequireValue(minScrollExtent), maxScrollExtent: DartRuntimePrimitives.RequireValue(maxScrollExtent), pixels: DartRuntimePrimitives.RequireValue(pixels), viewportDimension: DartRuntimePrimitives.RequireValue(viewportDimension), axisDirection: axisDirection, devicePixelRatio: devicePixelRatio)
    {
        this.viewportFraction = viewportFraction;
    }

    public virtual PageMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Generated.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new PageMetrics(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), viewportFraction: (viewportFraction ?? this.viewportFraction), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? page
    {
        get
        {
            return (Math.Max(0.0, Dart_uiLibrary.clampDouble(this.pixels, this.minScrollExtent, this.maxScrollExtent)) / Math.Max(1.0, (this.viewportDimension * this.viewportFraction)));
            return default!;
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
        this._viewportFraction = DartRuntimePrimitives.RequireValue(viewportFraction);
        this._pageToUseOnStartup = initialPage.toDouble();
        System.Diagnostics.Debug.Assert((DartRuntimePrimitives.RequireValue(viewportFraction) > 0.0));
    }

    public override Future ensureVisible(global::Doroti.Generated.Framework.Rendering.RenderObject @object, double alignment = 0.0, Duration duration = default, global::Doroti.Generated.Framework.Animation.Curve curve = default!, ScrollPositionAlignmentPolicy alignmentPolicy = ScrollPositionAlignmentPolicy.@explicit, global::Doroti.Generated.Framework.Rendering.RenderObject? targetRenderObject = null)
    {
        return ((Future)(object?)base.ensureVisible(@object, alignment: alignment, duration: duration, curve: curve, alignmentPolicy: alignmentPolicy));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double viewportFraction
    {
        get => this._viewportFraction;
        set
        {
            var __value = value;
            if ((this._viewportFraction == __value))
            {
                return;
            }
            double? oldPage__12904 = this.page;
            _viewportFraction = __value;
            if ((oldPage__12904 is not null))
            {
                double oldPage__12904__value12959 = DartRuntimePrimitives.RequireValue(oldPage__12904);
                forcePixels(getPixelsFromPage(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(oldPage__12904__value12959))));
            }
        }
    }
    internal virtual double _initialPageOffset => Math.Max(0, ((this.viewportDimension * ((this.viewportFraction - 1L))) / 2L));
    public virtual double getPageFromPixels(double pixels, double viewportDimension)
    {
        DartRuntimePrimitives.Assert(() => (DartRuntimePrimitives.RequireValue(viewportDimension) > 0.0));
        double actual__13586 = (Math.Max(0.0, (DartRuntimePrimitives.RequireValue(pixels) - this._initialPageOffset)) / ((DartRuntimePrimitives.RequireValue(viewportDimension) * this.viewportFraction)));
        double round__13705 = actual__13586.roundToDouble();
        if ((((actual__13586 - round__13705)).abs() < global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.precisionErrorTolerance))
        {
            return round__13705;
        }
        return actual__13586;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double getPixelsFromPage(double page)
    {
        return (((page * this.viewportDimension) * this.viewportFraction) + this._initialPageOffset);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual double? page
    {
        get
        {
            if (!this.hasPixels)
            {
                return null;
            }
            DartRuntimePrimitives.Assert(() => (this.hasContentDimensions || !this.haveDimensions), () => (object?)"Page value is only available after content dimensions are established.");
            return ((this.hasContentDimensions || this.haveDimensions) ? ((this._cachedPage ?? (double)getPageFromPixels(Dart_uiLibrary.clampDouble(this.pixels, this.minScrollExtent, this.maxScrollExtent), DartRuntimePrimitives.RequireValue(this.viewportDimension)))) : null);
            return default!;
        }
    }
    public override void saveScrollOffset()
    {
        PageStorage.maybeOf(((ScrollContext)this.context).storageContext)?.writeState(((ScrollContext)this.context).storageContext, ((this._cachedPage ?? (double)getPageFromPixels(DartRuntimePrimitives.RequireValue(this.pixels), DartRuntimePrimitives.RequireValue(this.viewportDimension)))));
    }

    public override void restoreScrollOffset()
    {
        if (!this.hasPixels)
        {
            var value__14735 = ((double?)PageStorage.maybeOf(((ScrollContext)this.context).storageContext)?.readState(((ScrollContext)this.context).storageContext));
            if ((value__14735 is not null))
            {
                double value__14735__value14854 = DartRuntimePrimitives.RequireValue(value__14735);
                _pageToUseOnStartup = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(value__14735__value14854));
            }
        }
    }

    public override void saveOffset()
    {
        this.context.saveOffset(((this._cachedPage ?? (double)getPageFromPixels(DartRuntimePrimitives.RequireValue(this.pixels), DartRuntimePrimitives.RequireValue(this.viewportDimension)))));
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
        double? oldViewportDimensions__15347 = (this.hasViewportDimension ? this.viewportDimension : null);
        if ((DartRuntimePrimitives.RequireValue(viewportDimension) == oldViewportDimensions__15347))
        {
            return true;
        }
        bool result__15519 = base.applyViewportDimension(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(viewportDimension)));
        double? oldPixels__15595 = (this.hasPixels ? this.pixels : null);
        double page__15645 = default!;
        if ((oldPixels__15595 is null))
        {
            page__15645 = this._pageToUseOnStartup;
        }
        else
        {
            if ((oldViewportDimensions__15347 == 0.0))
            {
                page__15645 = DartRuntimePrimitives.RequireValue(this._cachedPage);
            }
            else
            {
                page__15645 = getPageFromPixels(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(oldPixels__15595)), DartRuntimePrimitives.RequireValue(oldViewportDimensions__15347));
            }
        }
        double newPixels__15973 = getPixelsFromPage(DartRuntimePrimitives.RequireValue(page__15645));
        _cachedPage = (((DartRuntimePrimitives.RequireValue(viewportDimension) == 0.0)) ? page__15645 : null);
        if ((newPixels__15973 != oldPixels__15595))
        {
            correctPixels(newPixels__15973);
            return false;
        }
        return result__15519;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void absorb(ScrollPosition other)
    {
        base.absorb(other);
        DartRuntimePrimitives.Assert(() => (this._cachedPage is null));
        if ((other is not _PagePosition__page_view))
        {
            return;
        }
        if ((((_PagePosition__page_view)((_PagePosition__page_view)other))._cachedPage is not null))
        {
            _cachedPage = ((_PagePosition__page_view)((_PagePosition__page_view)other))._cachedPage;
        }
    }

    public override bool applyContentDimensions(double minScrollExtent, double maxScrollExtent)
    {
        double newMinScrollExtent__16660 = (DartRuntimePrimitives.RequireValue(minScrollExtent) + this._initialPageOffset);
        return base.applyContentDimensions(newMinScrollExtent__16660, Math.Max(newMinScrollExtent__16660, (DartRuntimePrimitives.RequireValue(maxScrollExtent) - this._initialPageOffset)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual PageMetrics copyWith(double? minScrollExtent = null, double? maxScrollExtent = null, double? pixels = null, double? viewportDimension = null, global::Doroti.Generated.Framework.Painting.AxisDirection? axisDirection = null, double? devicePixelRatio = null, long? itemIndex = null, double? minRange = null, double? maxRange = null, double? correctionOffset = null, double? viewportFraction = null)
    {
        return new PageMetrics(minScrollExtent: (minScrollExtent ?? ((this.hasContentDimensions ? this.minScrollExtent : null))), maxScrollExtent: (maxScrollExtent ?? ((this.hasContentDimensions ? this.maxScrollExtent : null))), pixels: (pixels ?? ((this.hasPixels ? this.pixels : null))), viewportDimension: (viewportDimension ?? ((this.hasViewportDimension ? this.viewportDimension : null))), axisDirection: (axisDirection ?? this.axisDirection), viewportFraction: (viewportFraction ?? this.viewportFraction), devicePixelRatio: (devicePixelRatio ?? this.devicePixelRatio));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ForceImplicitScrollPhysics__page_view : ScrollPhysics
{
    private bool __field_allowImplicitScrolling = default!;
    public override bool allowImplicitScrolling { get => __field_allowImplicitScrolling; }

    internal _ForceImplicitScrollPhysics__page_view(bool allowImplicitScrolling, ScrollPhysics? parent = null) : base(parent: parent)
    {
        this.__field_allowImplicitScrolling = allowImplicitScrolling;
    }

    public override _ForceImplicitScrollPhysics__page_view applyTo(ScrollPhysics? ancestor)
    {
        return new _ForceImplicitScrollPhysics__page_view(allowImplicitScrolling: this.allowImplicitScrolling, parent: buildParent(ancestor));
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
        if ((position is _PagePosition__page_view))
        {
            _PagePosition__page_view position__as18753 = (_PagePosition__page_view)position;
            return DartRuntimePrimitives.RequireValue(((_PagePosition__page_view)((_PagePosition__page_view)position__as18753)).page);
        }
        return (((ScrollMetrics)position).pixels / ((ScrollMetrics)position).viewportDimension);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getPixels(ScrollMetrics position, double page)
    {
        if ((position is _PagePosition__page_view))
        {
            _PagePosition__page_view position__as18946 = (_PagePosition__page_view)position;
            return ((_PagePosition__page_view)position__as18946).getPixelsFromPage(page);
        }
        return (page * ((ScrollMetrics)position).viewportDimension);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual double _getTargetPixels(ScrollMetrics position, global::Doroti.Generated.Framework.Physics.Tolerance tolerance, double velocity)
    {
        double page__19180 = _getPage(position);
        if ((velocity < -((global::Doroti.Generated.Framework.Physics.Tolerance)tolerance).velocity))
        {
            page__19180 -= 0.5;
        }
        else
        {
            if ((velocity > ((global::Doroti.Generated.Framework.Physics.Tolerance)tolerance).velocity))
            {
                page__19180 += 0.5;
            }
        }
        return _getPixels(position, page__19180.roundToDouble());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Physics.Simulation? createBallisticSimulation(ScrollMetrics position, double velocity)
    {
        if (((((velocity <= 0.0) && (((ScrollMetrics)position).pixels <= ((ScrollMetrics)position).minScrollExtent))) || (((velocity >= 0.0) && (((ScrollMetrics)position).pixels >= ((ScrollMetrics)position).maxScrollExtent)))))
        {
            return ((global::Doroti.Generated.Framework.Physics.Simulation?)(object?)base.createBallisticSimulation(position, velocity));
        }
        global::Doroti.Generated.Framework.Physics.Tolerance tolerance__19892 = ((global::Doroti.Generated.Framework.Physics.Tolerance)(object?)toleranceFor(position));
        double target__19945 = _getTargetPixels(position, tolerance__19892, velocity);
        if ((target__19945 != ((ScrollMetrics)position).pixels))
        {
            return ((global::Doroti.Generated.Framework.Physics.Simulation?)(object?)new global::Doroti.Generated.Framework.Physics.ScrollSpringSimulation(this.spring, ((ScrollMetrics)position).pixels, target__19945, velocity, tolerance: tolerance__19892));
        }
        return ((global::Doroti.Generated.Framework.Physics.Simulation)(object)null);
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
    public virtual global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent scrollCacheExtent { get; private set; } = default!;
    public virtual string? restorationId { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Axis scrollDirection { get; private set; } = default!;
    public virtual bool reverse { get; private set; } = default!;
    public virtual PageController? controller { get; private set; }
    public virtual ScrollPhysics? physics { get; private set; }
    public virtual bool pageSnapping { get; private set; } = default!;
    public virtual global::System.Action<long>? onPageChanged { get; private set; }
    public virtual SliverChildDelegate childrenDelegate { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior { get; private set; } = default!;
    public virtual Clip clipBehavior { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior { get; private set; } = default!;
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual bool padEnds { get; private set; } = default!;

    public PageView(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.horizontal, bool reverse = false, PageController? controller = null, ScrollPhysics? physics = null, bool pageSnapping = true, global::System.Action<long>? onPageChanged = null, List<Widget> children = default!, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, bool allowImplicitScrolling = false, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, ScrollBehavior? scrollBehavior = null, bool padEnds = true) : base(key: key)
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
        this.scrollCacheExtent = (scrollCacheExtent ?? global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreateViewport((allowImplicitScrolling ? 1.0 : 0.0)));
        this.childrenDelegate = new SliverChildListDelegate(children);
        System.Diagnostics.Debug.Assert(((scrollCacheExtent is null) || (((((global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent)scrollCacheExtent).value > 0.0)) == allowImplicitScrolling)));
    }

    public static PageView CreateBuilder(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.horizontal, bool reverse = false, PageController? controller = null, ScrollPhysics? physics = null, bool pageSnapping = true, global::System.Action<long>? onPageChanged = null, global::System.Func<BuildContext, long, Widget?> itemBuilder = default!, global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>? findChildIndexCallback = null, long? itemCount = null, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, bool allowImplicitScrolling = false, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, ScrollBehavior? scrollBehavior = null, bool padEnds = true)
    {
        var __instance = new PageView(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
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
        __instance.scrollCacheExtent = (scrollCacheExtent ?? global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreateViewport((allowImplicitScrolling ? 1.0 : 0.0)));
        __instance.childrenDelegate = new SliverChildBuilderDelegate((global::System.Func<BuildContext, long, Widget?>)itemBuilder, findChildIndexCallback: (global::System.Func<global::Doroti.Generated.Framework.Foundation.Key, long?>?)findChildIndexCallback, childCount: itemCount);
        return __instance;
    }

    public static PageView CreateCustom(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Painting.Axis scrollDirection = global::Doroti.Generated.Framework.Painting.Axis.horizontal, bool reverse = false, PageController? controller = null, ScrollPhysics? physics = null, bool pageSnapping = true, global::System.Action<long>? onPageChanged = null, SliverChildDelegate childrenDelegate = default!, global::Doroti.Generated.Framework.Gestures.DragStartBehavior dragStartBehavior = global::Doroti.Generated.Framework.Gestures.DragStartBehavior.start, bool allowImplicitScrolling = false, global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent? scrollCacheExtent = null, string? restorationId = null, Clip clipBehavior = Clip.hardEdge, global::Doroti.Generated.Framework.Rendering.HitTestBehavior hitTestBehavior = global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, ScrollBehavior? scrollBehavior = null, bool padEnds = true)
    {
        var __instance = new PageView(default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!, default!);
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
        __instance.scrollCacheExtent = (scrollCacheExtent ?? global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreateViewport((allowImplicitScrolling ? 1.0 : 0.0)));
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
        _lastReportedPage = ((PageController)this._controller).initialPage;
    }

    public override void dispose()
    {
        if ((((PageView)this.widget).controller is null))
        {
            this._controller.dispose();
        }
        base.dispose();
    }

    internal virtual void _initController()
    {
        _controller = (((PageView)this.widget).controller ?? new PageController());
    }

    public override void didUpdateWidget(PageView oldWidget)
    {
        if ((!object.Equals(((PageView)oldWidget).controller, ((PageView)this.widget).controller)))
        {
            if ((((PageView)oldWidget).controller is null))
            {
                this._controller.dispose();
            }
            _initController();
        }
        base.didUpdateWidget(oldWidget);
    }

    internal virtual global::Doroti.Generated.Framework.Painting.AxisDirection _getDirection(BuildContext context)
    {
        switch (((PageView)this.widget).scrollDirection)
        {
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugCheckHasDirectionality(context));
                    global::Doroti.Flutter.Ui.TextDirection textDirection__34725 = Directionality.of(context);
                    global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection__34797 = global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.textDirectionToAxisDirection(textDirection__34725);
                    return (((PageView)this.widget).reverse ? global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.flipAxisDirection(axisDirection__34797) : axisDirection__34797);
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    return (((PageView)this.widget).reverse ? global::Doroti.Generated.Framework.Painting.AxisDirection.up : global::Doroti.Generated.Framework.Painting.AxisDirection.down);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        global::Doroti.Generated.Framework.Painting.AxisDirection axisDirection__35123 = _getDirection(context);
        ScrollPhysics physics__35187 = ((ScrollPhysics)(object?)new _ForceImplicitScrollPhysics__page_view(allowImplicitScrolling: ((PageView)this.widget).allowImplicitScrolling).applyTo((((PageView)this.widget).pageSnapping ? Page_viewLibrary._kPagePhysics.applyTo(((((PageView)this.widget).physics ?? (ScrollPhysics)((PageView)this.widget).scrollBehavior?.getScrollPhysics(context)))) : ((((PageView)this.widget).physics ?? (ScrollPhysics)((PageView)this.widget).scrollBehavior?.getScrollPhysics(context))))));
        return ((Widget)(object?)new NotificationListener<ScrollNotification>(onNotification: ((global::System.Func<ScrollNotification, bool>?)((notification) => {
if ((((notification.depth == 0L) && (((PageView)this.widget).onPageChanged is not null)) && (notification is ScrollUpdateNotification)))
{
    var metrics__35832 = ((PageMetrics?)(object?)((ScrollUpdateNotification)notification).metrics)!;
    long currentPage__35899 = DartRuntimePrimitives.RequireValue(((PageMetrics)metrics__35832).page).round();
    if ((currentPage__35899 != this._lastReportedPage))
    {
        _lastReportedPage = currentPage__35899;
        ((PageView)this.widget).onPageChanged!(currentPage__35899);
    }
}
return false;
throw new InvalidOperationException("Dart closure completed without a value.");
})), child: new Scrollable(dragStartBehavior: ((PageView)this.widget).dragStartBehavior, axisDirection: axisDirection__35123, controller: this._controller, physics: physics__35187, restorationId: ((PageView)this.widget).restorationId, hitTestBehavior: ((PageView)this.widget).hitTestBehavior, scrollBehavior: ((((PageView)this.widget).scrollBehavior ?? (ScrollBehavior)ScrollConfiguration.of(context).copyWith(scrollbars: false))), viewportBuilder: ((global::System.Func<BuildContext, global::Doroti.Generated.Framework.Rendering.ViewportOffset, Widget>)((context, position) => {
return ((Widget)(object?)new Viewport(scrollCacheExtent: ((PageView)this.widget).scrollCacheExtent, axisDirection: axisDirection__35123, offset: position, clipBehavior: ((PageView)this.widget).clipBehavior, slivers: new List<Widget> { new SliverFillViewport(viewportFraction: ((PageController)this._controller).viewportFraction, @delegate: ((PageView)this.widget).childrenDelegate, padEnds: ((PageView)this.widget).padEnds, allowImplicitScrolling: ((PageView)this.widget).allowImplicitScrolling) }));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder description)
    {
        DiagnosticableDefaults.debugFillProperties(description);
        description.add(new global::Doroti.Generated.Framework.Foundation.EnumProperty<global::Doroti.Generated.Framework.Painting.Axis>("scrollDirection", ((PageView)this.widget).scrollDirection));
        description.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("reverse", value: ((PageView)this.widget).reverse, ifTrue: "reversed"));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<PageController>("controller", this._controller, showName: false));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<ScrollPhysics>("physics", ((PageView)this.widget).physics, showName: false));
        description.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("pageSnapping", value: ((PageView)this.widget).pageSnapping, ifFalse: "snapping disabled"));
        description.add(new global::Doroti.Generated.Framework.Foundation.FlagProperty("allowImplicitScrolling", value: ((PageView)this.widget).allowImplicitScrolling, ifTrue: "allow implicit scrolling"));
        description.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent>("scrollCacheExtent", ((PageView)this.widget).scrollCacheExtent, defaultValue: global::Doroti.Generated.Framework.Rendering.ScrollCacheExtent.CreateViewport((((PageView)this.widget).allowImplicitScrolling ? 1.0 : 0.0))));
    }

}
