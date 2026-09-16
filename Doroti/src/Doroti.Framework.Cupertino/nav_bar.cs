// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/nav_bar.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public enum NavigationBarBottomMode
{
    automatic,
    always
}

public static partial class Nav_barLibrary
{
    internal static double _kNavBarPersistentHeight = ConstantsLibrary.kMinInteractiveDimensionCupertino;
}

public static partial class Nav_barLibrary
{
    internal static double _kNavBarLargeTitleHeightExtension = 52.0;
}

public static partial class Nav_barLibrary
{
    internal static double _kNavBarShowLargeTitleThreshold = 10.0;
}

public static partial class Nav_barLibrary
{
    internal static double _kNavBarScrollUnderAnimationExtent = 10.0;
}

public static partial class Nav_barLibrary
{
    internal static double _kNavBarEdgePadding = 16.0;
}

public static partial class Nav_barLibrary
{
    internal static double _kNavBarBottomPadding = 8.0;
}

public static partial class Nav_barLibrary
{
    internal static double _kNavBarBackButtonTapWidth = 50.0;
}

public static partial class Nav_barLibrary
{
    internal static double _kMinScaleFactor = 0.9;
}

public static partial class Nav_barLibrary
{
    internal static double _kMaxScaleFactor = 1.235;
}

public static partial class Nav_barLibrary
{
    internal static double _kLargeTitleScaleDampingRatio = 3.0;
}

public static partial class Nav_barLibrary
{
    internal static double _kSearchFieldCancelButtonWidth = 67.0;
}

public static partial class Nav_barLibrary
{
    internal static double _kSearchFieldHeight = 36.0;
}

public static partial class Nav_barLibrary
{
    internal static Duration _kNavBarSearchDuration = Duration.Create(milliseconds: 300L);
}

public static partial class Nav_barLibrary
{
    internal static Curve _kNavBarSearchCurve = Curves.easeInOut;
}

public static partial class Nav_barLibrary
{
    internal static Duration _kNavBarTitleFadeDuration = Duration.Create(milliseconds: 150L);
}

public static partial class Nav_barLibrary
{
    internal static Color _kDefaultNavBarBorderColor = new Color(1291845632L);
}

public static partial class Nav_barLibrary
{
    internal static Border _kDefaultNavBarBorder = new Border(bottom: new BorderSide(color: _kDefaultNavBarBorderColor, width: 0.0));
}

public static partial class Nav_barLibrary
{
    internal static Border _kTransparentNavBarBorder = new Border(bottom: new BorderSide(color: new Color(0L), width: 0.0));
}

public static partial class Nav_barLibrary
{
    internal static Curve _kTopNavBarHeaderTransitionCurve = new Cubic(0.0, 0.45, 0.45, 0.98);
}

public static partial class Nav_barLibrary
{
    internal static Curve _kBottomNavBarHeaderTransitionCurve = new Cubic(0.05, 0.9, 0.9, 0.95);
}

public static partial class Nav_barLibrary
{
    internal static _HeroTag__nav_bar _defaultHeroTag = new _HeroTag__nav_bar(null);
}

internal class _HeroTag__nav_bar
{
    public virtual NavigatorState? navigator { get; private set; }

    internal _HeroTag__nav_bar(NavigatorState? navigator)
    {
        this.navigator = navigator;
    }

    public override string ToString() => $"Default Hero tag for Cupertino navigation bars with navigator {navigator}";
    public override bool Equals(object? other)
    {
        var __other = other as _HeroTag__nav_bar;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _HeroTag__nav_bar) && Equals(__other.navigator, navigator);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(Dart_coreLibrary.identityHashCode(navigator));
}

public class _FixedSizeSlidingTransition__nav_bar : AnimatedWidget
{
    public virtual bool isLTR { get; private set; } = default!;
    public virtual double width { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual Animation<Offset> offsetAnimation { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _FixedSizeSlidingTransition__nav_bar(bool isLTR, Animation<Offset> offsetAnimation, double width, double height, Widget child) : base(listenable: offsetAnimation)
    {
        this.isLTR = isLTR;
        this.offsetAnimation = offsetAnimation;
        this.width = width;
        this.height = height;
        this.child = child;
    }

    public override Widget build(BuildContext context)
    {
        return new Positioned(top: offsetAnimation.value.dy, left: isLTR ? offsetAnimation.value.dx : null, right: isLTR ? null : offsetAnimation.value.dx, width: width, height: height, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Nav_barLibrary
{
    internal static Widget _wrapWithBackground(Border? border = null, Color backgroundColor = default!, Brightness? brightness = null, Widget child = default!, bool updateSystemUiOverlay = true, bool enableBackgroundFilterBlur = true)
    {
        var result = child;
        if (updateSystemUiOverlay)
        {
            bool isDark = backgroundColor.computeLuminance() < 0.179;
            Brightness newBrightness = brightness ?? (isDark ? Brightness.dark : Brightness.light);
            SystemUiOverlayStyle overlayStyle = newBrightness switch { Brightness.dark => SystemUiOverlayStyle.light, Brightness.light => SystemUiOverlayStyle.dark, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            result = DartRuntimePrimitives.ConvertValue<Widget>(new AnnotatedRegion<SystemUiOverlayStyle>(value: new SystemUiOverlayStyle(statusBarColor: overlayStyle.statusBarColor, statusBarBrightness: overlayStyle.statusBarBrightness, statusBarIconBrightness: overlayStyle.statusBarIconBrightness, systemStatusBarContrastEnforced: overlayStyle.systemStatusBarContrastEnforced), child: result));
        }
        var childWithBackground = new DecoratedBox(decoration: new BoxDecoration(border: border, color: backgroundColor), child: result);
        return new ClipRect(child: new BackdropFilter(enabled: (backgroundColor.alpha != 255L) && enableBackgroundFilterBlur, filter: new ImageFilter(sigmaX: 10.0, sigmaY: 10.0), child: childWithBackground));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static double _dampScaleFactor(double scaledFontSize, double unscaledFontSize, double dampingRatio)
    {
        double scaleFactor = scaledFontSize / unscaledFontSize;
        return (scaleFactor < 1.0) ? Math.Max(_kMinScaleFactor, scaleFactor) : (1.0 + (scaleFactor - 1.0) / dampingRatio);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static bool _isTransitionable(BuildContext context)
    {
        IModalRoute? route = ModalRoute<object>.untypedOf(context);
        return (route is IPageRoute) && !route.fullscreenDialog && !CupertinoSheetRoute<object>.hasParentSheet(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoNavigationBar : StatefulWidget, ObstructingPreferredSizeWidget
{
    public virtual Widget? largeTitle { get; private set; }
    public virtual Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual bool automaticallyImplyMiddle { get; private set; } = default!;
    public virtual string? previousPageTitle { get; private set; }
    public virtual Widget? middle { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual EdgeInsetsDirectional? padding { get; private set; }
    public virtual Border? border { get; private set; }
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual PreferredSizeWidget? bottom { get; private set; }

    public CupertinoNavigationBar(Key? key = null, Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyMiddle = true, string? previousPageTitle = null, Widget? middle = null, Widget? trailing = null, Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, PreferredSizeWidget? bottom = null) : base(key: key)
    {
        Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
        object __heroTag = heroTag ?? Nav_barLibrary._defaultHeroTag;
        this.leading = leading;
        this.automaticallyImplyLeading = automaticallyImplyLeading;
        this.automaticallyImplyMiddle = automaticallyImplyMiddle;
        this.previousPageTitle = previousPageTitle;
        this.middle = middle;
        this.trailing = trailing;
        this.border = __border;
        this.backgroundColor = backgroundColor;
        this.automaticBackgroundVisibility = automaticBackgroundVisibility;
        this.enableBackgroundFilterBlur = enableBackgroundFilterBlur;
        this.brightness = brightness;
        this.padding = padding;
        this.transitionBetweenRoutes = transitionBetweenRoutes;
        this.heroTag = __heroTag;
        this.bottom = bottom;
        largeTitle = null;
        System.Diagnostics.Debug.Assert(!transitionBetweenRoutes || DartRuntimePrimitives.Identical(__heroTag, Nav_barLibrary._defaultHeroTag));
    }

    public static CupertinoNavigationBar CreateLarge(Key? key = null, Widget? largeTitle = null, Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, string? previousPageTitle = null, Widget? trailing = null, Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, PreferredSizeWidget? bottom = null)
    {
        var __instance = new CupertinoNavigationBar(key: key, leading: leading, automaticallyImplyLeading: automaticallyImplyLeading, previousPageTitle: previousPageTitle, trailing: trailing, border: border, backgroundColor: backgroundColor, automaticBackgroundVisibility: automaticBackgroundVisibility, enableBackgroundFilterBlur: enableBackgroundFilterBlur, brightness: brightness, padding: padding, transitionBetweenRoutes: transitionBetweenRoutes, heroTag: heroTag, bottom: bottom);
        Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
        object __heroTag = heroTag ?? Nav_barLibrary._defaultHeroTag;
        __instance.largeTitle = largeTitle;
        __instance.leading = leading;
        __instance.automaticallyImplyLeading = automaticallyImplyLeading;
        __instance.previousPageTitle = previousPageTitle;
        __instance.trailing = trailing;
        __instance.border = __border;
        __instance.backgroundColor = backgroundColor;
        __instance.automaticBackgroundVisibility = automaticBackgroundVisibility;
        __instance.enableBackgroundFilterBlur = enableBackgroundFilterBlur;
        __instance.brightness = brightness;
        __instance.padding = padding;
        __instance.transitionBetweenRoutes = transitionBetweenRoutes;
        __instance.heroTag = __heroTag;
        __instance.bottom = bottom;
        __instance.middle = null;
        __instance.automaticallyImplyMiddle = automaticallyImplyTitle;
        return __instance;
    }

    public virtual bool shouldFullyObstruct(BuildContext context)
    {
        Color backgroundColorLocal = CupertinoDynamicColor.maybeResolve(backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor;
        return backgroundColorLocal.alpha == 255L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size preferredSize
    {
        get
        {
            double bottomHeight = bottom?.preferredSize.height ?? 0.0;
            double effectiveLargeHeight = (largeTitle is not null) ? Nav_barLibrary._kNavBarLargeTitleHeightExtension : 0.0;
            return new Size(Nav_barLibrary._kNavBarPersistentHeight + bottomHeight + effectiveLargeHeight);
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoNavigationBarState__nav_bar());
}

internal class _CupertinoNavigationBarState__nav_bar : State<CupertinoNavigationBar>
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; set; } = default!;
    internal virtual ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } = default;
    internal virtual double _scrollAnimationValue { get; set; } = 0.0;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        _scrollNotificationObserver?.removeListener(_handleScrollNotification);
        _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(context);
        _scrollNotificationObserver?.addListener(_handleScrollNotification);
    }

    public override void dispose()
    {
        if (_scrollNotificationObserver is not null)
        {
            _scrollNotificationObserver!.removeListener(_handleScrollNotification);
            _scrollNotificationObserver = null;
        }
        base.dispose();
    }

    public override void initState()
    {
        base.initState();
        keys = new _NavigationBarStaticComponentsKeys__nav_bar();
    }

    internal virtual void _handleScrollNotification(ScrollNotification notification)
    {
        if ((notification is ScrollUpdateNotification) && (((ScrollUpdateNotification)notification).depth == 0L))
        {
            ScrollUpdateNotification notification__as27250 = (ScrollUpdateNotification)notification;
            ScrollMetrics metricsLocal = notification__as27250.metrics;
            double oldScrollAnimationValue = _scrollAnimationValue;
            var scrollExtent = 0.0;
            switch (metricsLocal.axisDirection)
            {
                case AxisDirection.up:
                    {
                        scrollExtent = metricsLocal.extentAfter;
                        break;
                    }
                case AxisDirection.down:
                    {
                        scrollExtent = metricsLocal.extentBefore;
                        break;
                    }
                case AxisDirection.right:
                case AxisDirection.left:
                    {
                        break;
                    }
            }
            if ((scrollExtent >= 0L) && (scrollExtent < Nav_barLibrary._kNavBarScrollUnderAnimationExtent))
            {
                setState(() =>
                {
                    _scrollAnimationValue = Dart_uiLibrary.clampDouble(scrollExtent / Nav_barLibrary._kNavBarScrollUnderAnimationExtent, 0, 1);
                });
            }
            else
            {
                if ((scrollExtent > Nav_barLibrary._kNavBarScrollUnderAnimationExtent) && (oldScrollAnimationValue != 1.0))
                {
                    setState(() =>
                    {
                        _scrollAnimationValue = 1.0;
                    });
                }
                else
                {
                    if ((scrollExtent <= 0L) && (oldScrollAnimationValue != 0.0))
                    {
                        setState(() =>
                        {
                            _scrollAnimationValue = 0.0;
                        });
                    }
                }
            }
        }
    }

    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (widget.middle is null) || (widget.largeTitle is null));
        Color backgroundColorLocal = CupertinoDynamicColor.maybeResolve(widget.backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor;
        Color? parentPageScaffoldBackgroundColor = CupertinoPageScaffoldBackgroundColor.maybeOf(context);
        Border? initialBorder = (widget.automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? Nav_barLibrary._kTransparentNavBarBorder : widget.border;
        Border? effectiveBorder = (widget.border is null) ? null : Border.lerp(initialBorder, widget.border, _scrollAnimationValue);
        Color effectiveBackgroundColor = (widget.automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? (Dart_uiLibrary.Color.lerp(parentPageScaffoldBackgroundColor, backgroundColorLocal, _scrollAnimationValue) ?? backgroundColorLocal) : backgroundColorLocal;
        double bottomHeight = widget.bottom?.preferredSize.height ?? 0.0;
        double persistentHeight = Nav_barLibrary._kNavBarPersistentHeight + bottomHeight + MediaQuery.paddingOf(context).top;
        double largeHeight = persistentHeight + Nav_barLibrary._kNavBarLargeTitleHeightExtension;
        var componentsLocal = new _NavigationBarStaticComponents__nav_bar(keys: keys, route: ModalRoute<object>.untypedOf(context), userLeading: widget.leading, automaticallyImplyLeading: widget.automaticallyImplyLeading, automaticallyImplyTitle: widget.automaticallyImplyMiddle, previousPageTitle: widget.previousPageTitle, userMiddle: widget.middle, userTrailing: widget.trailing, padding: widget.padding, userLargeTitle: widget.largeTitle, userBottom: DartRuntimePrimitives.ConvertValue<Widget>(widget.bottom), large: widget.largeTitle is not null, staticBar: true, context: context);
        Widget navBar = new _PersistentNavigationBar__nav_bar(components: componentsLocal, padding: widget.padding, middleVisible: widget.largeTitle is null);
        if (widget.largeTitle is not null)
        {
            navBar = DartRuntimePrimitives.ConvertValue<Widget>(new ConstrainedBox(constraints: new BoxConstraints(maxHeight: largeHeight), child: new Column(children: ((Func<List<Widget>>)(() => { var __collection31165 = new List<Widget>(); __collection31165.Add(DartRuntimePrimitives.ConvertValue<Widget>(navBar)); __collection31165.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new Widgets.Semantics(header: true, child: new DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, maxLines: 1L, overflow: TextOverflow.ellipsis, child: new _LargeTitle__nav_bar(height: Nav_barLibrary._kNavBarLargeTitleHeightExtension, child: componentsLocal.largeTitle))))))); if (widget.bottom is not null) { __collection31165.Add(DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(height: bottomHeight, child: componentsLocal.navBarBottom))); } return __collection31165; }))())));
        }
        else
        {
            navBar = DartRuntimePrimitives.ConvertValue<Widget>(new ConstrainedBox(constraints: new BoxConstraints(maxHeight: persistentHeight), child: new Column(children: ((Func<List<Widget>>)(() => { var __collection32281 = new List<Widget>(); __collection32281.Add(DartRuntimePrimitives.ConvertValue<Widget>(navBar)); if (widget.bottom is not null) { __collection32281.Add(DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(height: bottomHeight, child: componentsLocal.navBarBottom))); } return __collection32281; }))())));
        }
        navBar = Nav_barLibrary._wrapWithBackground(border: effectiveBorder, backgroundColor: effectiveBackgroundColor, brightness: widget.brightness, enableBackgroundFilterBlur: widget.enableBackgroundFilterBlur, child: new DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.textStyle, child: navBar));
        if (!widget.transitionBetweenRoutes || !Nav_barLibrary._isTransitionable(context))
        {
            return navBar;
        }
        return new Builder(builder: (context) =>
        {
            return new Hero(tag: Equals(widget.heroTag, Nav_barLibrary._defaultHeroTag) ? new _HeroTag__nav_bar(Navigator.of(context)) : widget.heroTag, createRectTween: (Func<Rect?, Rect?, RectTween>)Nav_barLibrary._linearTranslateWithLargestRectSizeTween, placeholderBuilder: Nav_barLibrary._navBarHeroLaunchPadBuilder, flightShuttleBuilder: Nav_barLibrary._navBarHeroFlightShuttleBuilder, transitionOnUserGestures: true, child: new _TransitionableNavigationBar__nav_bar(componentsKeys: keys, backgroundColor: effectiveBackgroundColor, backButtonTextStyle: CupertinoTheme.of(context).textTheme.navActionTextStyle, titleTextStyle: CupertinoTheme.of(context).textTheme.navTitleTextStyle, largeTitleTextStyle: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, border: effectiveBorder, hasUserMiddle: widget.middle is not null, largeExpanded: widget.largeTitle is not null, searchable: false, automaticBackgroundVisibility: widget.automaticBackgroundVisibility, child: navBar));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoSliverNavigationBar : StatefulWidget
{
    public virtual Widget? largeTitle { get; private set; }
    public virtual Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual bool automaticallyImplyTitle { get; private set; } = default!;
    public virtual bool alwaysShowMiddle { get; private set; } = default!;
    public virtual string? previousPageTitle { get; private set; }
    public virtual Widget? middle { get; private set; }
    public virtual Widget? trailing { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual EdgeInsetsDirectional? padding { get; private set; }
    public virtual Border? border { get; private set; }
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual PreferredSizeWidget? bottom { get; private set; }
    public virtual NavigationBarBottomMode? bottomMode { get; private set; }
    public virtual Action<bool>? onSearchableBottomTap { get; private set; }
    public virtual bool stretch { get; private set; } = default!;
    public virtual Widget? searchField { get; private set; }
    internal virtual bool _searchable { get; private set; } = default!;

    public CupertinoSliverNavigationBar(Key? key = null, Widget? largeTitle = null, Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, bool alwaysShowMiddle = true, string? previousPageTitle = null, Widget? middle = null, Widget? trailing = null, Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, bool stretch = false, PreferredSizeWidget? bottom = null, NavigationBarBottomMode? bottomMode = null) : base(key: key)
    {
        Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
        object __heroTag = heroTag ?? Nav_barLibrary._defaultHeroTag;
        this.largeTitle = largeTitle;
        this.leading = leading;
        this.automaticallyImplyLeading = automaticallyImplyLeading;
        this.automaticallyImplyTitle = automaticallyImplyTitle;
        this.alwaysShowMiddle = alwaysShowMiddle;
        this.previousPageTitle = previousPageTitle;
        this.middle = middle;
        this.trailing = trailing;
        this.border = __border;
        this.backgroundColor = backgroundColor;
        this.automaticBackgroundVisibility = automaticBackgroundVisibility;
        this.enableBackgroundFilterBlur = enableBackgroundFilterBlur;
        this.brightness = brightness;
        this.padding = padding;
        this.transitionBetweenRoutes = transitionBetweenRoutes;
        this.heroTag = __heroTag;
        this.stretch = stretch;
        this.bottom = bottom;
        this.bottomMode = bottomMode;
        onSearchableBottomTap = null;
        searchField = null;
        _searchable = false;
        System.Diagnostics.Debug.Assert(automaticallyImplyTitle || (largeTitle is not null));
        System.Diagnostics.Debug.Assert((bottomMode is null) || (bottom is not null));
    }

    public static CupertinoSliverNavigationBar CreateSearch(Key? key = null, Widget searchField = default!, Widget? largeTitle = null, Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, bool alwaysShowMiddle = true, string? previousPageTitle = null, Widget? middle = null, Widget? trailing = null, Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, bool stretch = false, NavigationBarBottomMode? bottomMode = NavigationBarBottomMode.automatic, Action<bool>? onSearchableBottomTap = null)
    {
        var __instance = new CupertinoSliverNavigationBar(key: key, largeTitle: largeTitle, leading: leading, automaticallyImplyLeading: automaticallyImplyLeading, automaticallyImplyTitle: automaticallyImplyTitle, alwaysShowMiddle: alwaysShowMiddle, previousPageTitle: previousPageTitle, middle: middle, trailing: trailing, border: border, backgroundColor: backgroundColor, automaticBackgroundVisibility: automaticBackgroundVisibility, enableBackgroundFilterBlur: enableBackgroundFilterBlur, brightness: brightness, padding: padding, transitionBetweenRoutes: transitionBetweenRoutes, heroTag: heroTag, stretch: stretch, bottomMode: bottomMode);
        Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
        object __heroTag = heroTag ?? Nav_barLibrary._defaultHeroTag;
        __instance.searchField = searchField;
        __instance.largeTitle = largeTitle;
        __instance.leading = leading;
        __instance.automaticallyImplyLeading = automaticallyImplyLeading;
        __instance.automaticallyImplyTitle = automaticallyImplyTitle;
        __instance.alwaysShowMiddle = alwaysShowMiddle;
        __instance.previousPageTitle = previousPageTitle;
        __instance.middle = middle;
        __instance.trailing = trailing;
        __instance.border = __border;
        __instance.backgroundColor = backgroundColor;
        __instance.automaticBackgroundVisibility = automaticBackgroundVisibility;
        __instance.enableBackgroundFilterBlur = enableBackgroundFilterBlur;
        __instance.brightness = brightness;
        __instance.padding = padding;
        __instance.transitionBetweenRoutes = transitionBetweenRoutes;
        __instance.heroTag = __heroTag;
        __instance.stretch = stretch;
        __instance.bottomMode = bottomMode;
        __instance.onSearchableBottomTap = onSearchableBottomTap;
        __instance.bottom = null;
        __instance._searchable = true;
        return __instance;
    }

    public virtual bool opaque => DartRuntimePrimitives.ConvertValue<bool>(backgroundColor?.alpha == 255L);
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoSliverNavigationBarState__nav_bar());
}

internal class _CupertinoSliverNavigationBarState__nav_bar : State<CupertinoSliverNavigationBar>, TickerProviderStateMixin<CupertinoSliverNavigationBar>
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; set; } = default!;
    internal virtual ScrollableState? _scrollableState { get; set; } = default;
    public virtual Widget? effectiveMiddle { get; set; } = default;
    internal virtual AnimationController _animationController { get; set; } = default!;
    internal virtual CurvedAnimation _searchAnimation { get; set; } = default!;
    public virtual Animation<double> persistentHeightAnimation { get; set; } = default!;
    public virtual Animation<double> largeTitleHeightAnimation { get; set; } = default!;
    public virtual double scaledSearchFieldHeight { get; set; } = default!;
    public virtual double scaledLargeTitleHeight { get; set; } = default!;
    public virtual bool searchIsActive { get; set; } = false;
    public virtual bool isPortrait { get; set; } = true;
    public virtual HashSet<Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        keys = new _NavigationBarStaticComponentsKeys__nav_bar();
        _animationController = new AnimationController(vsync: this, duration: Nav_barLibrary._kNavBarSearchDuration);
        _searchAnimation = new CurvedAnimation(parent: _animationController, curve: Nav_barLibrary._kNavBarSearchCurve);
    }

    public override void didUpdateWidget(CupertinoSliverNavigationBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if (!Equals(widget.middle, oldWidget.middle))
        {
            _updateEffectiveMiddle();
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        isPortrait = Equals(MediaQuery.orientationOf(context), Orientation.portrait);
        _updateEffectiveMiddle();
        _computeScaledHeights();
        _setupSearchableAnimation();
        _scrollableState?.position.isScrollingNotifier.removeListener(_handleScrollChange);
        _scrollableState = Scrollable.maybeOf(context);
        _scrollableState?.position.isScrollingNotifier.addListener(_handleScrollChange);
    }

    public override void dispose()
    {
        if (_scrollableState?.position is not null)
        {
            _scrollableState?.position.isScrollingNotifier.removeListener(_handleScrollChange);
        }
        _searchAnimation.dispose();
        _animationController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if (_tickers is not null)
                {
                    foreach (Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"{this} was disposed with an active Ticker."), new ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        _tickerModeNotifier?.removeListener(_updateTickers);
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual double _bottomHeight
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !widget._searchable || (widget.bottom is null));
            if (widget._searchable)
            {
                return scaledSearchFieldHeight + Nav_barLibrary._kNavBarBottomPadding;
            }
            else
            {
                if (widget.bottom is not null)
                {
                    return widget.bottom!.preferredSize.height;
                }
            }
            return 0.0;
        }
    }
    internal virtual void _updateEffectiveMiddle()
    {
        effectiveMiddle = widget.middle ?? (isPortrait ? null : widget.largeTitle);
    }

    internal virtual void _computeScaledHeights()
    {
        TextScaler textScaler = MediaQuery.textScalerOf(context);
        scaledSearchFieldHeight = Nav_barLibrary._kSearchFieldHeight * Nav_barLibrary._dampScaleFactor(textScaler.scale(Nav_barLibrary._kSearchFieldHeight), Nav_barLibrary._kSearchFieldHeight, Nav_barLibrary._kMaxScaleFactor);
        scaledLargeTitleHeight = isPortrait ? (Nav_barLibrary._kNavBarLargeTitleHeightExtension * Nav_barLibrary._dampScaleFactor(textScaler.scale(Nav_barLibrary._kNavBarLargeTitleHeightExtension), Nav_barLibrary._kNavBarLargeTitleHeightExtension, Nav_barLibrary._kLargeTitleScaleDampingRatio)) : 0.0;
    }

    internal virtual void _setupSearchableAnimation()
    {
        var persistentHeightTween = new Tween<double>(begin: Nav_barLibrary._kNavBarPersistentHeight, end: 0.0);
        persistentHeightAnimation = ((Func<Animation<double>>)(() =>
{
    var __cascade = persistentHeightTween.animate(_animationController);
    __cascade.addStatusListener(_handleSearchFieldStatusChanged);
    return __cascade;
}))();
        var largeTitleHeightTween = new Tween<double>(begin: scaledLargeTitleHeight, end: 0.0);
        largeTitleHeightAnimation = largeTitleHeightTween.animate(_animationController);
    }

    internal virtual void _handleScrollChange()
    {
        ScrollPosition? positionLocal = _scrollableState?.position;
        if ((positionLocal is null) || !positionLocal.hasPixels || (positionLocal.pixels <= 0.0))
        {
            return;
        }
        double? target = default!;
        double bottomScrollOffset = Equals(widget.bottomMode, NavigationBarBottomMode.always) ? 0.0 : _bottomHeight;
        bool canScrollBottom = (widget._searchable || (widget.bottom is not null)) && (bottomScrollOffset > 0.0);
        if (canScrollBottom && (positionLocal.pixels < bottomScrollOffset))
        {
            target = (positionLocal.pixels > (bottomScrollOffset / 2L)) ? bottomScrollOffset : 0.0;
        }
        else
        {
            if ((positionLocal.pixels > bottomScrollOffset) && (positionLocal.pixels < (bottomScrollOffset + scaledLargeTitleHeight)))
            {
                target = (positionLocal.pixels > (bottomScrollOffset + scaledLargeTitleHeight / 2L)) ? (bottomScrollOffset + scaledLargeTitleHeight) : bottomScrollOffset;
            }
        }
        if ((target is not null) && (target <= positionLocal.maxScrollExtent))
        {
            double target__50844__value51736 = DartRuntimePrimitives.RequireValue(target);
            DartRuntimePrimitives.Ignore(positionLocal.animateTo(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(target__50844__value51736)), duration: Duration.Create(milliseconds: 300L), curve: Curves.fastEaseInToSlowEaseOut));
        }
    }

    internal virtual void _handleSearchFieldStatusChanged(AnimationStatus status)
    {
        setState(() =>
        {
            switch (status)
            {
                case AnimationStatus.forward:
                    {
                        searchIsActive = true;
                        break;
                    }
                case AnimationStatus.reverse:
                    {
                        searchIsActive = false;
                        break;
                    }
                case AnimationStatus.completed:
                case AnimationStatus.dismissed:
                    break;
            }
        });
    }

    internal virtual void _onSearchFieldTap()
    {
        if (widget.onSearchableBottomTap is not null)
        {
            widget.onSearchableBottomTap!(!searchIsActive);
        }
        _animationController.toggle();
    }

    public override Widget build(BuildContext context)
    {
        var componentsLocal = new _NavigationBarStaticComponents__nav_bar(keys: keys, route: ModalRoute<object>.untypedOf(context), userLeading: (widget.leading is not null) ? new Visibility(visible: !searchIsActive, child: widget.leading!) : null, automaticallyImplyLeading: widget.automaticallyImplyLeading, automaticallyImplyTitle: widget.automaticallyImplyTitle, previousPageTitle: widget.previousPageTitle, userMiddle: _animationController.isAnimating ? new Text("") : effectiveMiddle, userTrailing: (widget.trailing is not null) ? new Visibility(visible: !searchIsActive, child: widget.trailing!) : null, userLargeTitle: widget.largeTitle, userBottom: (widget._searchable ? (searchIsActive ? new _ActiveSearchableBottom__nav_bar(animationController: _animationController, animation: persistentHeightAnimation, searchField: widget.searchField, searchFieldHeight: scaledSearchFieldHeight, onSearchFieldTap: () => _onSearchFieldTap()) : new _InactiveSearchableBottom__nav_bar(animationController: _animationController, animation: persistentHeightAnimation, searchField: widget.searchField, searchFieldHeight: scaledSearchFieldHeight, onSearchFieldTap: () => _onSearchFieldTap())) : (Widget?)widget.bottom) ?? SizedBox.CreateShrink(), padding: widget.padding, large: isPortrait, staticBar: false, context: context);
        return MediaQuery.withNoTextScaling(child: new AnimatedBuilder(animation: _searchAnimation, builder: (context, child) =>
        {
            return new SliverPersistentHeader(pinned: true, @delegate: new _LargeTitleNavigationBarSliverDelegate__nav_bar(keys: keys, components: componentsLocal, userMiddle: effectiveMiddle, backgroundColor: CupertinoDynamicColor.maybeResolve(widget.backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor, automaticBackgroundVisibility: widget.automaticBackgroundVisibility, brightness: widget.brightness, border: widget.border, padding: widget.padding, actionsForegroundColor: CupertinoTheme.of(context).primaryColor, transitionBetweenRoutes: widget.transitionBetweenRoutes, heroTag: widget.heroTag, persistentHeight: persistentHeightAnimation.value + MediaQuery.paddingOf(context).top, largeTitleHeight: largeTitleHeightAnimation.value, alwaysShowMiddle: widget.alwaysShowMiddle && (effectiveMiddle is not null), stretchConfiguration: (widget.stretch && !searchIsActive) ? new OverScrollHeaderStretchConfiguration() : null, enableBackgroundFilterBlur: widget.enableBackgroundFilterBlur, bottomMode: searchIsActive ? NavigationBarBottomMode.always : (widget.bottomMode ?? NavigationBarBottomMode.automatic), bottomHeight: _bottomHeight, controller: _animationController, searchable: widget._searchable));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Scheduler.Ticker createTicker(Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<_WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider(onTick, this, debugLabel: Foundation.ConstantsLibrary.kDebugMode ? $"created by {DiagnosticsLibrary.describeIdentity(this)}" : null);
    __cascade.muted = !values.enabled;
    __cascade.forceFrames = values.forceFrames;
    return __cascade;
}))();
        _tickers!.Add(result);
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(_WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => _tickers is not null);
        DartRuntimePrimitives.Assert(() => _tickers!.Contains(ticker));
        _tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if (_tickers is not null)
        {
            TickerModeData values = _tickerModeNotifier!.value;
            bool mutedLocal = !values.enabled;
            foreach (Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<HashSet<Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _LargeTitleNavigationBarSliverDelegate__nav_bar : SliverPersistentHeaderDelegate
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; private set; } = default!;
    public virtual _NavigationBarStaticComponents__nav_bar components { get; private set; } = default!;
    public virtual Widget? userMiddle { get; private set; }
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual Border? border { get; private set; }
    public virtual EdgeInsetsDirectional? padding { get; private set; }
    public virtual Color actionsForegroundColor { get; private set; } = default!;
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual double persistentHeight { get; private set; } = default!;
    public virtual double largeTitleHeight { get; private set; } = default!;
    public virtual bool alwaysShowMiddle { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual NavigationBarBottomMode bottomMode { get; private set; } = default!;
    public virtual double bottomHeight { get; private set; } = default!;
    public virtual AnimationController controller { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    private OverScrollHeaderStretchConfiguration? __field_stretchConfiguration = default;
    public override OverScrollHeaderStretchConfiguration? stretchConfiguration => __field_stretchConfiguration;

    internal _LargeTitleNavigationBarSliverDelegate__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar keys, _NavigationBarStaticComponents__nav_bar components, Widget? userMiddle, Color backgroundColor, bool automaticBackgroundVisibility, Brightness? brightness, Border? border, EdgeInsetsDirectional? padding, Color actionsForegroundColor, bool transitionBetweenRoutes, object heroTag, double persistentHeight, double largeTitleHeight, bool alwaysShowMiddle, OverScrollHeaderStretchConfiguration? stretchConfiguration, bool enableBackgroundFilterBlur, NavigationBarBottomMode bottomMode, double bottomHeight, AnimationController controller, bool searchable)
    {
        this.keys = keys;
        this.components = components;
        this.userMiddle = userMiddle;
        this.backgroundColor = backgroundColor;
        this.automaticBackgroundVisibility = automaticBackgroundVisibility;
        this.brightness = brightness;
        this.border = border;
        this.padding = padding;
        this.actionsForegroundColor = actionsForegroundColor;
        this.transitionBetweenRoutes = transitionBetweenRoutes;
        this.heroTag = heroTag;
        this.persistentHeight = persistentHeight;
        this.largeTitleHeight = largeTitleHeight;
        this.alwaysShowMiddle = alwaysShowMiddle;
        __field_stretchConfiguration = stretchConfiguration;
        this.enableBackgroundFilterBlur = enableBackgroundFilterBlur;
        this.bottomMode = bottomMode;
        this.bottomHeight = bottomHeight;
        this.controller = controller;
        this.searchable = searchable;
    }

    public override double minExtent => DartRuntimePrimitives.ConvertValue<double>(persistentHeight + (Equals(bottomMode, NavigationBarBottomMode.always) ? bottomHeight : 0.0));
    public override double maxExtent => DartRuntimePrimitives.ConvertValue<double>(persistentHeight + largeTitleHeight + bottomHeight);
    public override Widget build(BuildContext context, double shrinkOffset, bool overlapsContent)
    {
        double largeTitleThreshold = maxExtent - minExtent - Nav_barLibrary._kNavBarShowLargeTitleThreshold;
        bool showLargeTitle = shrinkOffset < largeTitleThreshold;
        double bottomShrinkFactor = Dart_uiLibrary.clampDouble(shrinkOffset / bottomHeight, 0, 1);
        double shrinkAnimationValue = Dart_uiLibrary.clampDouble((shrinkOffset - largeTitleThreshold - Nav_barLibrary._kNavBarScrollUnderAnimationExtent) / Nav_barLibrary._kNavBarScrollUnderAnimationExtent, 0, 1);
        var persistentNavigationBar = new _PersistentNavigationBar__nav_bar(components: components, padding: padding, middleVisible: alwaysShowMiddle ? null : !showLargeTitle);
        Color? parentPageScaffoldBackgroundColor = CupertinoPageScaffoldBackgroundColor.maybeOf(context);
        Border? initialBorder = (automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? Nav_barLibrary._kTransparentNavBarBorder : border;
        Border? effectiveBorder = (border is null) ? null : Border.lerp(initialBorder, border, shrinkAnimationValue);
        Color effectiveBackgroundColor = (automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? (Dart_uiLibrary.Color.lerp(parentPageScaffoldBackgroundColor, backgroundColor, shrinkAnimationValue) ?? backgroundColor) : backgroundColor;
        Widget navBar = Nav_barLibrary._wrapWithBackground(border: effectiveBorder, backgroundColor: effectiveBackgroundColor, brightness: brightness, enableBackgroundFilterBlur: enableBackgroundFilterBlur, child: new DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.textStyle, child: new Column(children: ((Func<List<Widget>>)(() => { var __collection60282 = new List<Widget>(); __collection60282.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new Stack(children: ((Func<List<Widget>>)(() => { var __collection60368 = new List<Widget>(); __collection60368.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Positioned(top: persistentHeight, left: 0.0, right: 0.0, bottom: Equals(bottomMode, NavigationBarBottomMode.automatic) ? (bottomHeight * (1.0 - bottomShrinkFactor)) : 0.0, child: new ClipRect(child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new SafeArea(top: false, bottom: false, child: new AnimatedOpacity(opacity: (showLargeTitle && !controller.isForwardOrCompleted) ? 1.0 : 0.0, duration: Nav_barLibrary._kNavBarTitleFadeDuration, child: new Widgets.Semantics(header: true, child: new DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, maxLines: 1L, overflow: TextOverflow.ellipsis, child: new _LargeTitle__nav_bar(height: largeTitleHeight, child: components.largeTitle)))))))))); __collection60368.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Positioned(left: 0.0, right: 0.0, top: 0.0, child: persistentNavigationBar))); if (Equals(bottomMode, NavigationBarBottomMode.automatic)) { __collection60368.Add(DartRuntimePrimitives.ConvertValue<Widget>(new Positioned(left: 0.0, right: 0.0, bottom: 0.0, child: new SizedBox(height: bottomHeight * (1.0 - bottomShrinkFactor), child: new ClipRect(child: components.navBarBottom))))); } return __collection60368; }))())))); if (Equals(bottomMode, NavigationBarBottomMode.always)) { __collection60282.Add(DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(height: bottomHeight, child: components.navBarBottom))); } return __collection60282; }))())));
        if (!transitionBetweenRoutes || !Nav_barLibrary._isTransitionable(context))
        {
            return navBar;
        }
        return new Hero(tag: Equals(heroTag, Nav_barLibrary._defaultHeroTag) ? new _HeroTag__nav_bar(Navigator.of(context)) : heroTag, createRectTween: (Func<Rect?, Rect?, RectTween>)Nav_barLibrary._linearTranslateWithLargestRectSizeTween, flightShuttleBuilder: Nav_barLibrary._navBarHeroFlightShuttleBuilder, placeholderBuilder: Nav_barLibrary._navBarHeroLaunchPadBuilder, transitionOnUserGestures: true, child: new _TransitionableNavigationBar__nav_bar(componentsKeys: keys, backgroundColor: effectiveBackgroundColor, backButtonTextStyle: CupertinoTheme.of(context).textTheme.navActionTextStyle, titleTextStyle: CupertinoTheme.of(context).textTheme.navTitleTextStyle, largeTitleTextStyle: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, border: effectiveBorder, hasUserMiddle: (userMiddle is not null) && (alwaysShowMiddle || !showLargeTitle), largeExpanded: showLargeTitle, searchable: searchable, automaticBackgroundVisibility: automaticBackgroundVisibility, child: navBar));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(SliverPersistentHeaderDelegate oldDelegate)
    {
        var __oldDelegate = (_LargeTitleNavigationBarSliverDelegate__nav_bar)oldDelegate;
        return (!Equals(components, __oldDelegate.components)) || (!Equals(userMiddle, __oldDelegate.userMiddle)) || (!Equals(backgroundColor, __oldDelegate.backgroundColor)) || (automaticBackgroundVisibility != __oldDelegate.automaticBackgroundVisibility) || (!Equals(border, __oldDelegate.border)) || (!Equals(padding, __oldDelegate.padding)) || (!Equals(actionsForegroundColor, __oldDelegate.actionsForegroundColor)) || (transitionBetweenRoutes != __oldDelegate.transitionBetweenRoutes) || (persistentHeight != __oldDelegate.persistentHeight) || (largeTitleHeight != __oldDelegate.largeTitleHeight) || (alwaysShowMiddle != __oldDelegate.alwaysShowMiddle) || (!Equals(heroTag, __oldDelegate.heroTag)) || (enableBackgroundFilterBlur != __oldDelegate.enableBackgroundFilterBlur) || (!Equals(bottomMode, __oldDelegate.bottomMode)) || (bottomHeight != __oldDelegate.bottomHeight) || (!Equals(controller, __oldDelegate.controller)) || (searchable != __oldDelegate.searchable);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _LargeTitle__nav_bar : SingleChildRenderObjectWidget
{
    public virtual double height { get; private set; } = default!;

    internal _LargeTitle__nav_bar(Widget? child = null, double height = default!) : base(child: child)
    {
        this.height = height;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderLargeTitle__nav_bar(alignment: AlignmentDirectional.bottomStart.resolve(Directionality.of(context)), height: height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderLargeTitle__nav_bar)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderLargeTitle__nav_bar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = AlignmentDirectional.bottomStart.resolve(Directionality.of(context));
    __cascade.height = height;
    return __cascade;
}))());
    }

}

public class _RenderLargeTitle__nav_bar : RenderShiftedBox
{
    internal virtual Alignment _alignment { get; set; } = default!;
    internal virtual double _height { get; set; } = default!;
    internal virtual double _scale { get; set; } = 1.0;

    internal _RenderLargeTitle__nav_bar(Alignment alignment, double height) : base(null)
    {
        _alignment = alignment;
        _height = height;
    }

    public virtual Alignment alignment
    {
        get => _alignment;
        set
        {
            var __value = value;
            if (Equals(_alignment, __value))
            {
                return;
            }
            _alignment = __value;
            markNeedsLayout();
        }
    }
    public virtual double height
    {
        get => _height;
        set
        {
            var __value = value;
            if (_height == __value)
            {
                return;
            }
            _height = __value;
            markNeedsLayout();
        }
    }
    internal static double _computeTitleScale(Size childSize, BoxConstraints constraints, double height)
    {
        double maxHeightLocal = height - Nav_barLibrary._kNavBarBottomPadding;
        double scale = 1.0 + (0.03 * (constraints.maxHeight - maxHeightLocal) / maxHeightLocal);
        double maxScale = (childSize.width != 0.0) ? Dart_uiLibrary.clampDouble(constraints.maxWidth / childSize.width, 1.0, 1.1) : 1.1;
        return Dart_uiLibrary.clampDouble(scale, 1.0, maxScale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        double? distance = child?.getDistanceToActualBaseline(baseline);
        if (distance is null)
        {
            return null;
        }
        var childParentData = ((BoxParentData?)child!.parentData!)!;
        return childParentData.offset.dy + (DartRuntimePrimitives.RequireValue(distance) * _scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(BoxConstraints constraints, TextBaseline baseline)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        BoxConstraints childConstraints = constraints.widthConstraints().loosen();
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        Size childSize = childLocal.getDryLayout(childConstraints);
        double scale = _computeTitleScale(childSize, constraints, height);
        Size scaledChildSize = childSize * scale;
        return (DartRuntimePrimitives.RequireValue(result) * scale) + alignment.alongOffset(constraints.biggest - scaledChildSize).dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        RenderBox? childLocal = child;
        size = constraints.biggest;
        if (childLocal is null)
        {
            return;
        }
        BoxConstraints childConstraints = constraints.widthConstraints().loosen();
        childLocal.layout(childConstraints, parentUsesSize: true);
        _scale = _computeTitleScale(childLocal.size, constraints, height);
        var childParentData = ((BoxParentData?)childLocal.parentData!)!;
        childParentData.offset = alignment.alongOffset(size - childLocal.size * _scale);
    }

    public override void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        var __child = (RenderBox)child;
        DartRuntimePrimitives.Assert(() => Equals(__child, this.child));
        base.applyPaintTransform(__child, transform);
        transform.scaleByDouble(_scale, _scale, _scale, 1);
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            layer = null;
        }
        else
        {
            var childParentData = ((BoxParentData?)childLocal.parentData!)!;
            layer = context.pushTransform(needsCompositing, offset + childParentData.offset, Matrix4.diagonal3Values(_scale, _scale, 1.0), (context, offset) => { context.paintChild(childLocal, offset); }, oldLayer: ((TransformLayer?)layer)!);
        }
    }

    public override bool hitTestChildren(BoxHitTestResult result, Offset position)
    {
        RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return false;
        }
        Offset childOffset = ((BoxParentData?)childLocal.parentData!)!.offset;
        var transformLocal = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.scaleByDouble(1.0 / _scale, 1.0 / _scale, 1.0, 1);
    __cascade.translateByDouble(-childOffset.dx, -childOffset.dy, 0, 1);
    return __cascade;
}))();
        return result.addWithRawTransform(transform: transformLocal, position: position, hitTest: (result, transformed) =>
        {
            return childLocal.hitTest(result, position: transformed);
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PersistentNavigationBar__nav_bar : StatelessWidget
{
    public virtual _NavigationBarStaticComponents__nav_bar components { get; private set; } = default!;
    public virtual EdgeInsetsDirectional? padding { get; private set; }
    public virtual bool? middleVisible { get; private set; }

    internal _PersistentNavigationBar__nav_bar(_NavigationBarStaticComponents__nav_bar components, EdgeInsetsDirectional? padding = null, bool? middleVisible = null)
    {
        this.components = components;
        this.padding = padding;
        this.middleVisible = middleVisible;
    }

    public override Widget build(BuildContext context)
    {
        Widget? middleLocal = components.middle;
        if (middleLocal is not null)
        {
            middleLocal = DartRuntimePrimitives.ConvertValue<Widget>(new DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navTitleTextStyle, child: new Widgets.Semantics(header: true, child: middleLocal)));
            middleLocal = (middleVisible is null) ? middleLocal : new AnimatedOpacity(opacity: DartRuntimePrimitives.RequireValue(middleVisible) ? 1.0 : 0.0, duration: Nav_barLibrary._kNavBarTitleFadeDuration, child: middleLocal);
        }
        Widget? leadingLocal = components.leading;
        Widget? backChevronLocal = components.backChevron;
        Widget? backLabelLocal = components.backLabel;
        if ((leadingLocal is null) && (backChevronLocal is not null) && (backLabelLocal is not null) && !CupertinoSheetRoute<object>.hasParentSheet(context))
        {
            leadingLocal = DartRuntimePrimitives.ConvertValue<Widget>(CupertinoNavigationBarBackButton.Create_assemble(backChevronLocal, backLabelLocal));
        }
        else
        {
            leadingLocal = DartRuntimePrimitives.ConvertValue<Widget>(new Align(widthFactor: 1.0, child: leadingLocal));
        }
        Widget paddedToolbar = new NavigationToolbar(leading: leadingLocal, middle: middleLocal, trailing: components.trailing, middleSpacing: 6.0);
        if (padding is not null)
        {
            paddedToolbar = DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsets.CreateOnly(top: padding!.top, bottom: padding!.bottom), child: paddedToolbar));
        }
        return new SizedBox(height: Nav_barLibrary._kNavBarPersistentHeight + MediaQuery.paddingOf(context).top, child: new SafeArea(top: !CupertinoSheetRoute<object>.hasParentSheet(context), bottom: false, child: paddedToolbar));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _NavigationBarStaticComponentsKeys__nav_bar
{
    public virtual GlobalKey<IState> navBarBoxKey { get; private set; } = default!;
    public virtual GlobalKey<IState> leadingKey { get; private set; } = default!;
    public virtual GlobalKey<IState> backChevronKey { get; private set; } = default!;
    public virtual GlobalKey<IState> backLabelKey { get; private set; } = default!;
    public virtual GlobalKey<IState> middleKey { get; private set; } = default!;
    public virtual GlobalKey<IState> trailingKey { get; private set; } = default!;
    public virtual GlobalKey<IState> largeTitleKey { get; private set; } = default!;
    public virtual GlobalKey<IState> navBarBottomKey { get; private set; } = default!;

    internal _NavigationBarStaticComponentsKeys__nav_bar()
    {
        navBarBoxKey = GlobalKey<IState>.Create(debugLabel: "Navigation bar render box");
        leadingKey = GlobalKey<IState>.Create(debugLabel: "Leading");
        backChevronKey = GlobalKey<IState>.Create(debugLabel: "Back chevron");
        backLabelKey = GlobalKey<IState>.Create(debugLabel: "Back label");
        middleKey = GlobalKey<IState>.Create(debugLabel: "Middle");
        trailingKey = GlobalKey<IState>.Create(debugLabel: "Trailing");
        largeTitleKey = GlobalKey<IState>.Create(debugLabel: "Large title");
        navBarBottomKey = GlobalKey<IState>.Create(debugLabel: "Navigation bar bottom");
    }

}

public class _NavigationBarStaticComponents__nav_bar
{
    public virtual KeyedSubtree? leading { get; private set; }
    public virtual KeyedSubtree? backChevron { get; private set; }
    public virtual KeyedSubtree? backLabel { get; private set; }
    public virtual KeyedSubtree? middle { get; private set; }
    public virtual KeyedSubtree? trailing { get; private set; }
    public virtual KeyedSubtree? largeTitle { get; private set; }
    public virtual KeyedSubtree? navBarBottom { get; private set; }

    internal _NavigationBarStaticComponents__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar keys, IModalRoute? route, Widget? userLeading, bool automaticallyImplyLeading, bool automaticallyImplyTitle, string? previousPageTitle, Widget? userMiddle, Widget? userTrailing, Widget? userLargeTitle, Widget? userBottom, EdgeInsetsDirectional? padding, bool large, bool staticBar, BuildContext context)
    {
        leading = createLeading(leadingKey: keys.leadingKey, userLeading: userLeading, route: route, automaticallyImplyLeading: automaticallyImplyLeading, padding: padding, context: context);
        backChevron = createBackChevron(backChevronKey: keys.backChevronKey, userLeading: userLeading, route: route, automaticallyImplyLeading: automaticallyImplyLeading, context: context);
        backLabel = createBackLabel(backLabelKey: keys.backLabelKey, userLeading: userLeading, route: route, previousPageTitle: previousPageTitle, automaticallyImplyLeading: automaticallyImplyLeading, context: context);
        middle = createMiddle(middleKey: keys.middleKey, userMiddle: userMiddle, userLargeTitle: userLargeTitle, route: route, automaticallyImplyTitle: automaticallyImplyTitle, large: large, staticBar: staticBar, context: context);
        trailing = createTrailing(trailingKey: keys.trailingKey, userTrailing: userTrailing, padding: padding, context: context);
        largeTitle = createLargeTitle(largeTitleKey: keys.largeTitleKey, userLargeTitle: userLargeTitle, route: route, automaticImplyTitle: automaticallyImplyTitle, large: large, context: context);
        navBarBottom = createNavBarBottom(navBarBottomKey: keys.navBarBottomKey, userBottom: userBottom, context: context);
    }

    internal static Widget? _derivedTitle(bool automaticallyImplyTitle, IModalRoute? currentRoute = null)
    {
        if (automaticallyImplyTitle && (currentRoute is ICupertinoRouteTitle) && (((ICupertinoRouteTitle)currentRoute).title is not null))
        {
            ICupertinoRouteTitle currentRoute__as76488 = (ICupertinoRouteTitle)currentRoute;
            return (Widget?)new Text(currentRoute__as76488.title!);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static KeyedSubtree? createLeading(GlobalKey<IState> leadingKey, Widget? userLeading, IModalRoute? route, bool automaticallyImplyLeading, EdgeInsetsDirectional? padding, BuildContext context)
    {
        Widget? leadingContent = default!;
        if (userLeading is not null)
        {
            leadingContent = userLeading;
        }
        else
        {
            if (automaticallyImplyLeading && (route is IPageRoute) && route.canPop && route.fullscreenDialog)
            {
                var route__as77104 = route;
                leadingContent = DartRuntimePrimitives.ConvertValue<Widget>(new CupertinoButton(padding: EdgeInsets.zero, onPressed: () =>
                {
                    DartRuntimePrimitives.Ignore(route__as77104.navigator!.maybePop<object>());
                }, child: new Text(CupertinoLocalizations.of(context).cancelButtonLabel)));
            }
        }
        if (leadingContent is null)
        {
            return null;
        }
        return new KeyedSubtree(key: leadingKey, child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: padding?.start ?? Nav_barLibrary._kNavBarEdgePadding), child: new MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: IconTheme.merge(data: new IconThemeData(size: 32.0), child: leadingContent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static KeyedSubtree? createBackChevron(GlobalKey<IState> backChevronKey, Widget? userLeading, IModalRoute? route, bool automaticallyImplyLeading, BuildContext context)
    {
        if ((userLeading is not null) || !automaticallyImplyLeading || (route is null) || !route.canPop || (route is IPageRoute) && route.fullscreenDialog)
        {
            return null;
        }
        return new KeyedSubtree(key: backChevronKey, child: new MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: new _BackChevron__nav_bar()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static KeyedSubtree? createBackLabel(GlobalKey<IState> backLabelKey, Widget? userLeading, IModalRoute? route, bool automaticallyImplyLeading, string? previousPageTitle, BuildContext context)
    {
        if ((userLeading is not null) || !automaticallyImplyLeading || (route is null) || !route.canPop || (route is IPageRoute) && route.fullscreenDialog)
        {
            return null;
        }
        return new KeyedSubtree(key: backLabelKey, child: new MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: new _BackLabel__nav_bar(specifiedPreviousTitle: previousPageTitle, route: route)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static KeyedSubtree? createMiddle(GlobalKey<IState> middleKey, Widget? userMiddle, Widget? userLargeTitle, bool large, bool staticBar, bool automaticallyImplyTitle, IModalRoute? route, BuildContext context)
    {
        var middleContent = userMiddle;
        if (large && staticBar)
        {
            return null;
        }
        if (large)
        {
            middleContent ??= userLargeTitle;
        }
        middleContent ??= _derivedTitle(automaticallyImplyTitle: automaticallyImplyTitle, currentRoute: route);
        if (middleContent is null)
        {
            return null;
        }
        return new KeyedSubtree(key: middleKey, child: new MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: middleContent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static KeyedSubtree? createTrailing(GlobalKey<IState> trailingKey, Widget? userTrailing, EdgeInsetsDirectional? padding, BuildContext context)
    {
        if (userTrailing is null)
        {
            return null;
        }
        return new KeyedSubtree(key: trailingKey, child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(end: padding?.end ?? Nav_barLibrary._kNavBarEdgePadding), child: new MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: IconTheme.merge(data: new IconThemeData(size: 32.0), child: userTrailing))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static KeyedSubtree? createLargeTitle(GlobalKey<IState> largeTitleKey, Widget? userLargeTitle, bool large, bool automaticImplyTitle, IModalRoute? route, BuildContext context)
    {
        if (!large)
        {
            return null;
        }
        Widget? largeTitleContent = userLargeTitle ?? _derivedTitle(automaticallyImplyTitle: automaticImplyTitle, currentRoute: route);
        DartRuntimePrimitives.Assert(() => largeTitleContent is not null, () => (object?)"largeTitle was not provided and there was no title from the route.");
        return new KeyedSubtree(key: largeTitleKey, child: new MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: TextScaler.CreateLinear(Nav_barLibrary._dampScaleFactor(MediaQuery.textScalerOf(context).scale(Nav_barLibrary._kNavBarLargeTitleHeightExtension), Nav_barLibrary._kNavBarLargeTitleHeightExtension, Nav_barLibrary._kLargeTitleScaleDampingRatio))), child: largeTitleContent!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static KeyedSubtree? createNavBarBottom(GlobalKey<IState> navBarBottomKey, Widget? userBottom, BuildContext context)
    {
        return new KeyedSubtree(key: navBarBottomKey, child: new MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: MediaQuery.textScalerOf(context)), child: userBottom ?? SizedBox.CreateShrink()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static TextScaler _clampedTextScaler(BuildContext context)
    {
        return MediaQuery.textScalerOf(context).clamp(minScaleFactor: 1.0, maxScaleFactor: Nav_barLibrary._kMaxScaleFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoNavigationBarBackButton : StatelessWidget
{
    public virtual Color? color { get; private set; }
    public virtual string? previousPageTitle { get; private set; }
    public virtual Action? onPressed { get; private set; }
    internal virtual Widget? _backChevron { get; private set; }
    internal virtual Widget? _backLabel { get; private set; }

    public CupertinoNavigationBarBackButton(Key? key = null, Color? color = null, string? previousPageTitle = null, Action? onPressed = null) : base(key: key)
    {
        this.color = color;
        this.previousPageTitle = previousPageTitle;
        this.onPressed = onPressed;
        _backChevron = null;
        _backLabel = null;
    }

    public static CupertinoNavigationBarBackButton Create_assemble(Widget? _backChevron, Widget? _backLabel)
    {
        var __instance = new CupertinoNavigationBarBackButton();
        __instance._backChevron = _backChevron;
        __instance._backLabel = _backLabel;
        __instance.previousPageTitle = null;
        __instance.color = null;
        __instance.onPressed = null;
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        IModalRoute? currentRoute = ModalRoute<object>.untypedOf(context);
        if (onPressed is null)
        {
            DartRuntimePrimitives.Assert(() => (currentRoute?.canPop) ?? false, () => (object?)"CupertinoNavigationBarBackButton should only be used in routes that can be popped");
        }
        TextStyle actionTextStyle = CupertinoTheme.of(context).textTheme.navActionTextStyle;
        if (color is not null)
        {
            actionTextStyle = actionTextStyle.copyWith(color: CupertinoDynamicColor.maybeResolve(color, context));
        }
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        return new CupertinoButton(padding: EdgeInsets.zero, child: new Widgets.Semantics(container: true, excludeSemantics: true, label: localizations.backButtonLabel, button: true, child: new DefaultTextStyle(style: actionTextStyle, child: new ConstrainedBox(constraints: new BoxConstraints(minWidth: Nav_barLibrary._kNavBarBackButtonTapWidth), child: new Row(mainAxisSize: MainAxisSize.min, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 8.0))), DartRuntimePrimitives.ConvertValue<Widget>(_backChevron ?? new _BackChevron__nav_bar()), DartRuntimePrimitives.ConvertValue<Widget>(new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 6.0))), DartRuntimePrimitives.ConvertValue<Widget>(new Flexible(child: _backLabel ?? new _BackLabel__nav_bar(specifiedPreviousTitle: previousPageTitle, route: currentRoute))) })))), onPressed: () =>
        {
            if (onPressed is not null)
            {
                onPressed!();
            }
            else
            {
                DartRuntimePrimitives.Ignore(Navigator.maybePop<object>(context));
            }
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BackChevron__nav_bar : StatelessWidget
{
    internal _BackChevron__nav_bar()
    {
    }

    public override Widget build(BuildContext context)
    {
        TextDirection textDirection = Directionality.of(context);
        TextStyle textStyle = DefaultTextStyle.of(context).style;
        Widget iconWidget = new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 6, end: 2), child: Text.CreateRich(new TextSpan(text: char.ConvertFromUtf32(checked((int)CupertinoIcons.back.codePoint)), style: new TextStyle(inherit: false, color: textStyle.color, fontSize: 30.0, fontFamily: CupertinoIcons.back.fontFamily, package: CupertinoIcons.back.fontPackage))));
        switch (textDirection)
        {
            case TextDirection.rtl:
                {
                    iconWidget = DartRuntimePrimitives.ConvertValue<Widget>(new Transform(transform: ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.scaleByDouble(-1.0, 1.0, 1.0, 1);
    return __cascade;
}))(), alignment: Alignment.center, transformHitTests: false, child: iconWidget));
                    break;
                }
            case TextDirection.ltr:
                {
                    break;
                }
        }
        return new KeyedSubtree(key: StandardComponentTypeMembers.key(StandardComponentType.backButton), child: iconWidget);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BackLabel__nav_bar : StatelessWidget
{
    public virtual string? specifiedPreviousTitle { get; private set; }
    public virtual IModalRoute? route { get; private set; } = default!;

    internal _BackLabel__nav_bar(string? specifiedPreviousTitle, IModalRoute? route)
    {
        this.specifiedPreviousTitle = specifiedPreviousTitle;
        this.route = route;
    }

    internal virtual Widget _buildPreviousTitleWidget(BuildContext context, string? previousTitle, Widget? child)
    {
        if (previousTitle is null)
        {
            return SizedBox.CreateShrink();
        }
        var textWidget = new Text(previousTitle, maxLines: 1L, overflow: TextOverflow.ellipsis);
        if (previousTitle.Length > 12L)
        {
            textWidget = new Text(CupertinoLocalizations.of(context).backButtonLabel);
        }
        return new Align(alignment: AlignmentDirectional.centerStart, widthFactor: 1.0, child: textWidget);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        if (specifiedPreviousTitle is not null)
        {
            return _buildPreviousTitleWidget(context, specifiedPreviousTitle, null);
        }
        else
        {
            if ((route is ICupertinoRouteTitle) && !route!.isFirst)
            {
                ICupertinoRouteTitle route__as89428 = (ICupertinoRouteTitle)route;
                var cupertinoRoute = ((ICupertinoRouteTitle?)route!)!;
                return new ValueListenableBuilder<string?>(valueListenable: cupertinoRoute.previousTitle, builder: _buildPreviousTitleWidget);
            }
            else
            {
                return SizedBox.CreateShrink();
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CancelButton__nav_bar : StatelessWidget
{
    public virtual Action? onPressed { get; private set; }
    public virtual double opacity { get; private set; } = default!;

    internal _CancelButton__nav_bar(double opacity = 1.0, Action? onPressed = default!)
    {
        this.opacity = opacity;
        this.onPressed = onPressed;
    }

    public override Widget build(BuildContext context)
    {
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        return MediaQuery.withNoTextScaling(child: new Align(alignment: Alignment.centerLeft, child: new Opacity(opacity: opacity, child: new CupertinoButton(padding: EdgeInsets.zero, onPressed: onPressed, child: new Text(localizations.cancelButtonLabel, maxLines: 1L, overflow: TextOverflow.clip)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InactiveSearchableBottom__nav_bar : StatelessWidget
{
    public virtual AnimationController animationController { get; private set; } = default!;
    public virtual Widget? searchField { get; private set; }
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual double searchFieldHeight { get; private set; } = default!;
    public virtual Action? onSearchFieldTap { get; private set; }

    internal _InactiveSearchableBottom__nav_bar(AnimationController animationController, Widget? searchField, Animation<double> animation, double searchFieldHeight, Action? onSearchFieldTap)
    {
        this.animationController = animationController;
        this.searchField = searchField;
        this.animation = animation;
        this.searchFieldHeight = searchFieldHeight;
        this.onSearchFieldTap = onSearchFieldTap;
    }

    public override Widget build(BuildContext context)
    {
        return new AnimatedBuilder(animation: animation, child: new GestureDetector(onTap: onSearchFieldTap, child: new AbsorbPointer(child: new FocusableActionDetector(descendantsAreFocusable: false, child: new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, end: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new SizedBox(height: searchFieldHeight, child: searchField))))), builder: (context, child) =>
        {
            return new LayoutBuilder(builder: (context, constraints) =>
            {
                return new Row(children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: constraints.maxWidth - Nav_barLibrary._kSearchFieldCancelButtonWidth * animationController.value, child: child)), DartRuntimePrimitives.ConvertValue<Widget>(new SizedBox(width: animationController.value * Nav_barLibrary._kSearchFieldCancelButtonWidth, child: new Padding(padding: EdgeInsets.CreateOnly(bottom: Nav_barLibrary._kNavBarBottomPadding), child: new _CancelButton__nav_bar(opacity: 0.4, onPressed: () => {
})))) });
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActiveSearchableBottom__nav_bar : StatelessWidget
{
    public virtual AnimationController animationController { get; private set; } = default!;
    public virtual Widget? searchField { get; private set; }
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual double searchFieldHeight { get; private set; } = default!;
    public virtual Action? onSearchFieldTap { get; private set; }

    internal _ActiveSearchableBottom__nav_bar(AnimationController animationController, Widget? searchField, Animation<double> animation, double searchFieldHeight, Action? onSearchFieldTap)
    {
        this.animationController = animationController;
        this.searchField = searchField;
        this.animation = animation;
        this.searchFieldHeight = searchFieldHeight;
        this.onSearchFieldTap = onSearchFieldTap;
    }

    public override Widget build(BuildContext context)
    {
        return new Padding(padding: EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new Row(spacing: 12.0, children: new List<Widget> { DartRuntimePrimitives.ConvertValue<Widget>(new Expanded(child: new SizedBox(height: searchFieldHeight, child: searchField ?? SizedBox.CreateShrink()))), DartRuntimePrimitives.ConvertValue<Widget>(new AnimatedBuilder(animation: animation, child: new FadeTransition(opacity: new Tween<double>(begin: 0.0, end: 1.0).animate(animationController), child: new _CancelButton__nav_bar(onPressed: onSearchFieldTap)), builder: (context, child) => {
return new SizedBox(width: animationController.value * Nav_barLibrary._kSearchFieldCancelButtonWidth, child: child);
throw new InvalidOperationException("Dart closure completed without a value.");
})) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _TransitionableNavigationBar__nav_bar : StatelessWidget
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar componentsKeys { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual TextStyle backButtonTextStyle { get; private set; } = default!;
    public virtual TextStyle titleTextStyle { get; private set; } = default!;
    public virtual TextStyle? largeTitleTextStyle { get; private set; }
    public virtual Border? border { get; private set; }
    public virtual bool hasUserMiddle { get; private set; } = default!;
    public virtual bool largeExpanded { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual Widget child { get; private set; } = default!;

    internal _TransitionableNavigationBar__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar componentsKeys, Color? backgroundColor, TextStyle backButtonTextStyle, TextStyle titleTextStyle, TextStyle? largeTitleTextStyle, Border? border, bool hasUserMiddle, bool largeExpanded, bool searchable, bool automaticBackgroundVisibility, Widget child) : base(key: componentsKeys.navBarBoxKey)
    {
        this.componentsKeys = componentsKeys;
        this.backgroundColor = backgroundColor;
        this.backButtonTextStyle = backButtonTextStyle;
        this.titleTextStyle = titleTextStyle;
        this.largeTitleTextStyle = largeTitleTextStyle;
        this.border = border;
        this.hasUserMiddle = hasUserMiddle;
        this.largeExpanded = largeExpanded;
        this.searchable = searchable;
        this.automaticBackgroundVisibility = automaticBackgroundVisibility;
        this.child = child;
        System.Diagnostics.Debug.Assert(!largeExpanded || (largeTitleTextStyle is not null));
    }

    public virtual RenderBox renderBox
    {
        get
        {
            var box = ((RenderBox?)componentsKeys.navBarBoxKey.currentContext!.findRenderObject()!)!;
            DartRuntimePrimitives.Assert(() => box.attached, () => (object?)"_TransitionableNavigationBar.renderBox should be called when building " + "hero flight shuttles when the from and the to nav bar boxes are already " + "laid out and painted.");
            return box;
        }
    }
    public virtual bool userGestureInProgress
    {
        get
        {
            return Navigator.of(componentsKeys.navBarBoxKey.currentContext!).userGestureInProgress;
        }
    }
    public override Widget build(BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var inHero = false;
                context.visitAncestorElements((ancestor) =>
                {
                    if (ancestor is ComponentElement)
                    {
                        DartRuntimePrimitives.Assert(() => !Equals(DartRuntimePrimitives.RuntimeType(((ComponentElement)ancestor).widget), typeof(_NavigationBarTransition__nav_bar)), () => (object?)"_TransitionableNavigationBar should never re-appear inside " + "_NavigationBarTransition. Keyed _TransitionableNavigationBar should " + "only serve as anchor points in routes rather than appearing inside " + "Hero flights themselves.");
                        if (Equals(DartRuntimePrimitives.RuntimeType(((ComponentElement)ancestor).widget), typeof(Hero)))
                        {
                            inHero = true;
                        }
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
                DartRuntimePrimitives.Assert(() => inHero, () => (object?)"_TransitionableNavigationBar should only be added as the immediate " + "child of Hero widgets.");
                return true;
            });
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarTransition__nav_bar : StatelessWidget
{
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual _TransitionableNavigationBar__nav_bar topNavBar { get; private set; } = default!;
    public virtual _TransitionableNavigationBar__nav_bar bottomNavBar { get; private set; } = default!;
    public virtual Tween<double> heightTween { get; private set; } = default!;

    internal _NavigationBarTransition__nav_bar(Animation<double> animation, _TransitionableNavigationBar__nav_bar topNavBar, _TransitionableNavigationBar__nav_bar bottomNavBar)
    {
        this.animation = animation;
        this.topNavBar = topNavBar;
        this.bottomNavBar = bottomNavBar;
        heightTween = new Tween<double>(begin: bottomNavBar.renderBox.size.height, end: topNavBar.renderBox.size.height);
    }

    public override Widget build(BuildContext context)
    {
        var componentsTransition = new _NavigationBarComponentsTransition__nav_bar(animation: animation, bottomNavBar: bottomNavBar, topNavBar: topNavBar, directionality: Directionality.of(context));
        var childrenLocal = ((Func<List<Widget>>)(() => { var __collection98801 = new List<Widget>(); var __collectionElement98817 = componentsTransition.bottomNavBarBackground; if (__collectionElement98817 is { } __nonNullCollectionElement98817) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement98817)); } var __collectionElement98869 = componentsTransition.bottomBackChevron; if (__collectionElement98869 is { } __nonNullCollectionElement98869) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement98869)); } var __collectionElement98916 = componentsTransition.bottomBackLabel; if (__collectionElement98916 is { } __nonNullCollectionElement98916) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement98916)); } var __collectionElement98961 = componentsTransition.bottomLeading; if (__collectionElement98961 is { } __nonNullCollectionElement98961) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement98961)); } var __collectionElement99004 = componentsTransition.bottomMiddle; if (__collectionElement99004 is { } __nonNullCollectionElement99004) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99004)); } var __collectionElement99046 = componentsTransition.bottomLargeTitle; if (__collectionElement99046 is { } __nonNullCollectionElement99046) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99046)); } var __collectionElement99092 = componentsTransition.bottomTrailing; if (__collectionElement99092 is { } __nonNullCollectionElement99092) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99092)); } var __collectionElement99136 = componentsTransition.bottomNavBarBottom; if (__collectionElement99136 is { } __nonNullCollectionElement99136) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99136)); } var __collectionElement99246 = componentsTransition.topNavBarBackground; if (__collectionElement99246 is { } __nonNullCollectionElement99246) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99246)); } var __collectionElement99295 = componentsTransition.topLeading; if (__collectionElement99295 is { } __nonNullCollectionElement99295) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99295)); } var __collectionElement99335 = componentsTransition.topBackChevron; if (__collectionElement99335 is { } __nonNullCollectionElement99335) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99335)); } var __collectionElement99379 = componentsTransition.topBackLabel; if (__collectionElement99379 is { } __nonNullCollectionElement99379) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99379)); } var __collectionElement99421 = componentsTransition.topMiddle; if (__collectionElement99421 is { } __nonNullCollectionElement99421) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99421)); } var __collectionElement99460 = componentsTransition.topLargeTitle; if (__collectionElement99460 is { } __nonNullCollectionElement99460) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99460)); } var __collectionElement99503 = componentsTransition.topTrailing; if (__collectionElement99503 is { } __nonNullCollectionElement99503) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99503)); } var __collectionElement99544 = componentsTransition.topNavBarBottom; if (__collectionElement99544 is { } __nonNullCollectionElement99544) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<Widget>(__nonNullCollectionElement99544)); } return __collection98801; }))();
        return MediaQuery.withNoTextScaling(child: new SizedBox(height: Math.Max(DartRuntimePrimitives.RequireValue(heightTween.begin), DartRuntimePrimitives.RequireValue(heightTween.end)) + MediaQuery.paddingOf(context).top, width: double.PositiveInfinity, child: new Stack(children: childrenLocal)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarComponentsTransition__nav_bar
{
    public static Animatable<double> fadeOut = new Tween<double>(begin: 1.0, end: 0.0);
    public static Animatable<double> fadeIn = new Tween<double>(begin: 0.0, end: 1.0);
    public virtual Animation<double> animation { get; private set; } = default!;
    public virtual _NavigationBarStaticComponentsKeys__nav_bar bottomComponents { get; private set; } = default!;
    public virtual _NavigationBarStaticComponentsKeys__nav_bar topComponents { get; private set; } = default!;
    public virtual RenderBox bottomNavBarBox { get; private set; } = default!;
    public virtual RenderBox topNavBarBox { get; private set; } = default!;
    public virtual TextStyle bottomBackButtonTextStyle { get; private set; } = default!;
    public virtual TextStyle topBackButtonTextStyle { get; private set; } = default!;
    public virtual TextStyle bottomTitleTextStyle { get; private set; } = default!;
    public virtual TextStyle topTitleTextStyle { get; private set; } = default!;
    public virtual TextStyle? bottomLargeTitleTextStyle { get; private set; }
    public virtual TextStyle? topLargeTitleTextStyle { get; private set; }
    public virtual bool bottomHasUserMiddle { get; private set; } = default!;
    public virtual bool topHasUserMiddle { get; private set; } = default!;
    public virtual bool bottomLargeExpanded { get; private set; } = default!;
    public virtual bool topLargeExpanded { get; private set; } = default!;
    public virtual bool userGestureInProgress { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    public virtual bool bottomAutomaticBackgroundVisibility { get; private set; } = default!;
    public virtual Color? bottomBackgroundColor { get; private set; }
    public virtual Color? topBackgroundColor { get; private set; }
    public virtual Border? bottomBorder { get; private set; }
    public virtual Border? topBorder { get; private set; }
    public virtual Rect transitionBox { get; private set; } = default!;
    public virtual double forwardDirection { get; private set; } = default!;

    internal _NavigationBarComponentsTransition__nav_bar(Animation<double> animation, _TransitionableNavigationBar__nav_bar bottomNavBar, _TransitionableNavigationBar__nav_bar topNavBar, TextDirection directionality)
    {
        this.animation = animation;
        bottomComponents = bottomNavBar.componentsKeys;
        topComponents = topNavBar.componentsKeys;
        bottomNavBarBox = bottomNavBar.renderBox;
        topNavBarBox = topNavBar.renderBox;
        bottomBackButtonTextStyle = bottomNavBar.backButtonTextStyle;
        topBackButtonTextStyle = topNavBar.backButtonTextStyle;
        bottomTitleTextStyle = bottomNavBar.titleTextStyle;
        topTitleTextStyle = topNavBar.titleTextStyle;
        bottomLargeTitleTextStyle = bottomNavBar.largeTitleTextStyle;
        topLargeTitleTextStyle = topNavBar.largeTitleTextStyle;
        bottomHasUserMiddle = bottomNavBar.hasUserMiddle;
        topHasUserMiddle = topNavBar.hasUserMiddle;
        bottomLargeExpanded = bottomNavBar.largeExpanded;
        topLargeExpanded = topNavBar.largeExpanded;
        bottomBackgroundColor = bottomNavBar.backgroundColor;
        topBackgroundColor = topNavBar.backgroundColor;
        bottomBorder = bottomNavBar.border;
        topBorder = topNavBar.border;
        bottomAutomaticBackgroundVisibility = bottomNavBar.automaticBackgroundVisibility;
        userGestureInProgress = topNavBar.userGestureInProgress || bottomNavBar.userGestureInProgress;
        searchable = topNavBar.searchable && bottomNavBar.searchable;
        transitionBox = bottomNavBar.renderBox.paintBounds.expandToInclude(topNavBar.renderBox.paintBounds);
        forwardDirection = Equals(directionality, TextDirection.ltr) ? 1.0 : -1.0;
    }

    public virtual RelativeRect positionInTransitionBox(GlobalKey<IState> key, RenderBox from)
    {
        var componentBox = ((RenderBox?)key.currentContext!.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => componentBox.attached);
        return RelativeRect.CreateFromRect(componentBox.localToGlobal(Offset.zero, ancestor: from) & componentBox.size, transitionBox);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _FixedSizeSlidingTransition__nav_bar slideFromLeadingEdge(GlobalKey<IState> fromKey, RenderBox fromNavBarBox, GlobalKey<IState> toKey, RenderBox toNavBarBox, Curve curve = default!, Widget child = default!)
    {
        curve ??= new Interval(0.0, 1.0);
        var fromBox = ((RenderBox?)fromKey.currentContext!.findRenderObject()!)!;
        var toBox = ((RenderBox?)toKey.currentContext!.findRenderObject()!)!;
        bool isLTRLocal = forwardDirection > 0L;
        var fromAnchorLocal = new Offset(isLTRLocal ? 0 : fromBox.size.width, fromBox.size.height / 2L);
        var toAnchorLocal = new Offset(isLTRLocal ? 0 : toBox.size.width, toBox.size.height / 2L);
        Offset fromAnchorInFromBox = fromBox.localToGlobal(fromAnchorLocal, ancestor: fromNavBarBox);
        Offset toAnchorInToBox = toBox.localToGlobal(toAnchorLocal, ancestor: toNavBarBox);
        Offset translation = isLTRLocal ? (toAnchorInToBox - fromAnchorInFromBox) : (new Offset(toNavBarBox.size.width - toAnchorInToBox.dx, toAnchorInToBox.dy) - new Offset(fromNavBarBox.size.width - fromAnchorInFromBox.dx, fromAnchorInFromBox.dy));
        RelativeRect fromBoxMargin = positionInTransitionBox(fromKey, from: fromNavBarBox);
        var fromOriginInTransitionBox = new Offset(isLTRLocal ? fromBoxMargin.left : fromBoxMargin.right, fromBoxMargin.top);
        var anchorMovementInTransitionBox = new Tween<Offset>(begin: fromOriginInTransitionBox, end: fromOriginInTransitionBox + translation);
        return new _FixedSizeSlidingTransition__nav_bar(isLTR: isLTRLocal, offsetAnimation: animation.drive(new CurveTween(curve: curve)).drive(anchorMovementInTransitionBox), width: fromNavBarBox.size.width, height: fromBox.size.height, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animation<double> fadeInFrom(double t, Curve curve = default!)
    {
        curve ??= Curves.easeIn;
        return animation.drive(fadeIn.chain(new CurveTween(curve: new Interval(t, 1.0, curve: curve))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animation<double> fadeOutBy(double t, Curve curve = default!)
    {
        curve ??= Curves.easeOut;
        return animation.drive(fadeOut.chain(new CurveTween(curve: new Interval(0.0, t, curve: curve))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Animation<double> routeAnimation
    {
        get
        {
            DartRuntimePrimitives.Assert(() => animation is CurvedAnimation);
            return ((CurvedAnimation?)animation)!.parent;
        }
    }
    public virtual Widget? bottomNavBarBackground
    {
        get
        {
            if ((bottomBackgroundColor is null) || bottomLargeExpanded && bottomAutomaticBackgroundVisibility)
            {
                return null;
            }
            Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Curves.fastEaseInToSlowEaseOut : Curves.fastEaseInToSlowEaseOut.flipped;
            Animation<double> pageTransitionAnimation = routeAnimation.drive(new CurveTween(curve: userGestureInProgress ? Curves.linear : animationCurve));
            RelativeRect fromLocal = positionInTransitionBox(bottomComponents.navBarBoxKey, from: bottomNavBarBox);
            var positionTween = new RelativeRectTween(end: fromLocal.shift(new Offset(forwardDirection * -bottomNavBarBox.size.width, 0.0)), begin: fromLocal);
            return (Widget?)new PositionedTransition(rect: pageTransitionAnimation.drive(positionTween), child: Nav_barLibrary._wrapWithBackground(updateSystemUiOverlay: false, backgroundColor: bottomBackgroundColor!, border: topBorder, child: new SizedBox(height: bottomNavBarBox.size.height, width: double.PositiveInfinity)));
        }
    }
    public virtual Widget? bottomLeading
    {
        get
        {
            var bottomLeading = ((KeyedSubtree?)bottomComponents.leadingKey.currentWidget)!;
            if (bottomLeading is null)
            {
                return null;
            }
            return (Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(bottomComponents.leadingKey, from: bottomNavBarBox), child: new FadeTransition(opacity: fadeOutBy(0.4), child: bottomLeading.child));
        }
    }
    public virtual Widget? bottomBackChevron
    {
        get
        {
            var bottomBackChevron = ((KeyedSubtree?)bottomComponents.backChevronKey.currentWidget)!;
            if (bottomBackChevron is null)
            {
                return null;
            }
            return (Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(bottomComponents.backChevronKey, from: bottomNavBarBox), child: new FadeTransition(opacity: fadeOutBy(0.6), child: new DefaultTextStyle(style: bottomBackButtonTextStyle, child: bottomBackChevron.child)));
        }
    }
    public virtual Widget? bottomBackLabel
    {
        get
        {
            var bottomBackLabel = ((KeyedSubtree?)bottomComponents.backLabelKey.currentWidget)!;
            if (bottomBackLabel is null)
            {
                return null;
            }
            RelativeRect fromLocal = positionInTransitionBox(bottomComponents.backLabelKey, from: bottomNavBarBox);
            var positionTween = new RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new Offset(forwardDirection * (-bottomNavBarBox.size.width / 2.0), 0.0)));
            return (Widget?)new PositionedTransition(rect: animation.drive(positionTween), child: new FadeTransition(opacity: fadeOutBy(0.2), child: new DefaultTextStyle(style: bottomBackButtonTextStyle, child: bottomBackLabel.child)));
        }
    }
    public virtual Widget? bottomMiddle
    {
        get
        {
            var bottomMiddle = ((KeyedSubtree?)bottomComponents.middleKey.currentWidget)!;
            var topBackLabel = ((KeyedSubtree?)topComponents.backLabelKey.currentWidget)!;
            var topLeading = ((KeyedSubtree?)topComponents.leadingKey.currentWidget)!;
            if (!bottomHasUserMiddle && bottomLargeExpanded)
            {
                return null;
            }
            if ((bottomMiddle is not null) && (topBackLabel is not null))
            {
                return (Widget?)slideFromLeadingEdge(fromKey: bottomComponents.middleKey, fromNavBarBox: bottomNavBarBox, toKey: topComponents.backLabelKey, toNavBarBox: topNavBarBox, child: new FadeTransition(opacity: fadeOutBy(bottomHasUserMiddle ? 0.4 : 0.7), child: new Align(alignment: AlignmentDirectional.centerStart, child: new DefaultTextStyleTransition(style: animation.drive(new TextStyleTween(begin: bottomTitleTextStyle, end: topBackButtonTextStyle)), child: bottomMiddle.child))));
            }
            if ((bottomMiddle is not null) && (topLeading is not null))
            {
                return (Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(bottomComponents.middleKey, from: bottomNavBarBox), child: new FadeTransition(opacity: fadeOutBy(bottomHasUserMiddle ? 0.4 : 0.7), child: new DefaultTextStyle(style: bottomTitleTextStyle, child: bottomMiddle.child)));
            }
            return null;
        }
    }
    public virtual Widget? bottomLargeTitle
    {
        get
        {
            var bottomLargeTitle = ((KeyedSubtree?)bottomComponents.largeTitleKey.currentWidget)!;
            var topBackLabel = ((KeyedSubtree?)topComponents.backLabelKey.currentWidget)!;
            if ((bottomLargeTitle is null) || !bottomLargeExpanded)
            {
                return null;
            }
            if (topBackLabel is not null)
            {
                return (Widget?)slideFromLeadingEdge(fromKey: bottomComponents.largeTitleKey, fromNavBarBox: bottomNavBarBox, toKey: topComponents.backLabelKey, toNavBarBox: topNavBarBox, curve: new Interval(0.0, Equals(animation.status, AnimationStatus.forward) ? 0.7 : 1.0), child: new FadeTransition(opacity: fadeOutBy(0.6), child: new Align(alignment: AlignmentDirectional.centerStart, child: new DefaultTextStyleTransition(style: animation.drive(new TextStyleTween(begin: bottomLargeTitleTextStyle, end: topBackButtonTextStyle)), maxLines: 1L, overflow: TextOverflow.ellipsis, child: bottomLargeTitle.child))));
            }
            RelativeRect fromLocal = positionInTransitionBox(bottomComponents.largeTitleKey, from: bottomNavBarBox);
            var positionTween = new RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new Offset(forwardDirection * bottomNavBarBox.size.width / 4.0, 0.0)));
            return (Widget?)new PositionedTransition(rect: animation.drive(positionTween), child: new FadeTransition(opacity: fadeOutBy(0.4), child: new DefaultTextStyle(style: bottomLargeTitleTextStyle!, child: bottomLargeTitle.child)));
        }
    }
    public virtual Widget? bottomTrailing
    {
        get
        {
            var bottomTrailing = ((KeyedSubtree?)bottomComponents.trailingKey.currentWidget)!;
            if (bottomTrailing is null)
            {
                return null;
            }
            return (Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(bottomComponents.trailingKey, from: bottomNavBarBox), child: new FadeTransition(opacity: fadeOutBy(0.6), child: bottomTrailing.child));
        }
    }
    public virtual Widget? bottomNavBarBottom
    {
        get
        {
            var bottomNavBarBottom = ((KeyedSubtree?)bottomComponents.navBarBottomKey.currentWidget)!;
            if (bottomNavBarBottom is null)
            {
                return null;
            }
            RelativeRect fromLocal = positionInTransitionBox(bottomComponents.navBarBottomKey, from: bottomNavBarBox);
            var positionTween = new RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new Offset(forwardDirection * -bottomNavBarBox.size.width, 0.0)));
            Widget childLocal = bottomNavBarBottom.child;
            Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Nav_barLibrary._kBottomNavBarHeaderTransitionCurve : Nav_barLibrary._kBottomNavBarHeaderTransitionCurve.flipped;
            if (!searchable)
            {
                childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new FadeTransition(opacity: fadeOutBy(0.8, curve: animationCurve), child: childLocal));
            }
            return (Widget?)new PositionedTransition(rect: userGestureInProgress ? routeAnimation.drive(new CurveTween(curve: Curves.linear)).drive(positionTween) : animation.drive(new CurveTween(curve: animationCurve)).drive(positionTween), child: new ClipRect(child: childLocal));
        }
    }
    public virtual Widget? topNavBarBackground
    {
        get
        {
            if (topBackgroundColor is null)
            {
                return null;
            }
            Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Curves.fastEaseInToSlowEaseOut : Curves.fastEaseInToSlowEaseOut.flipped;
            Animation<double> pageTransitionAnimation = routeAnimation.drive(new CurveTween(curve: userGestureInProgress ? Curves.linear : animationCurve));
            RelativeRect to = positionInTransitionBox(topComponents.navBarBoxKey, from: topNavBarBox);
            var positionTween = new RelativeRectTween(begin: to.shift(new Offset(forwardDirection * topNavBarBox.size.width, 0.0)), end: to);
            return (Widget?)new PositionedTransition(rect: pageTransitionAnimation.drive(positionTween), child: Nav_barLibrary._wrapWithBackground(updateSystemUiOverlay: false, backgroundColor: topBackgroundColor!, border: topBorder, child: new SizedBox(height: topNavBarBox.size.height, width: double.PositiveInfinity)));
        }
    }
    public virtual Widget? topLeading
    {
        get
        {
            var topLeading = ((KeyedSubtree?)topComponents.leadingKey.currentWidget)!;
            if (topLeading is null)
            {
                return null;
            }
            return (Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(topComponents.leadingKey, from: topNavBarBox), child: new FadeTransition(opacity: fadeInFrom(0.6), child: topLeading.child));
        }
    }
    public virtual Widget? topBackChevron
    {
        get
        {
            var topBackChevron = ((KeyedSubtree?)topComponents.backChevronKey.currentWidget)!;
            var bottomBackChevron = ((KeyedSubtree?)bottomComponents.backChevronKey.currentWidget)!;
            if (topBackChevron is null)
            {
                return null;
            }
            RelativeRect to = positionInTransitionBox(topComponents.backChevronKey, from: topNavBarBox);
            var fromLocal = to;
            Widget childLocal = topBackChevron.child;
            Curve forwardScaleCurve = new Interval(0.0, 0.2);
            Curve backwardScaleCurve = new Interval(0.8, 1.0);
            Curve forwardPositionCurve = new Interval(0.0, 0.5);
            Curve backwardPositionCurve = new Interval(0.5, 1.0);
            Curve effectiveScaleCurve = default!;
            Curve effectivePositionCurve = default!;
            if (Equals(animation.status, AnimationStatus.forward))
            {
                effectiveScaleCurve = forwardScaleCurve;
                effectivePositionCurve = forwardPositionCurve;
            }
            else
            {
                effectiveScaleCurve = backwardScaleCurve;
                effectivePositionCurve = backwardPositionCurve;
            }
            if (bottomBackChevron is null)
            {
                var topBackChevronBox = ((RenderBox?)topComponents.backChevronKey.currentContext!.findRenderObject()!)!;
                fromLocal = to.shift(new Offset(forwardDirection * topBackChevronBox.size.width * 2.0, 0.0));
                childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new ScaleTransition(scale: routeAnimation.drive(new CurveTween(curve: effectiveScaleCurve)), child: childLocal));
            }
            var positionTween = new RelativeRectTween(begin: fromLocal, end: to);
            return (Widget?)new PositionedTransition(rect: routeAnimation.drive(new CurveTween(curve: effectivePositionCurve)).drive(positionTween), child: new FadeTransition(opacity: routeAnimation.drive(new CurveTween(curve: new Interval(((bottomBackChevron is null) && (!Equals(animation.status, AnimationStatus.forward))) ? 0.9 : 0.4, 1.0))), child: new DefaultTextStyle(style: topBackButtonTextStyle, child: childLocal)));
        }
    }
    public virtual Widget? topBackLabel
    {
        get
        {
            var bottomMiddle = ((KeyedSubtree?)bottomComponents.middleKey.currentWidget)!;
            var bottomLargeTitle = ((KeyedSubtree?)bottomComponents.largeTitleKey.currentWidget)!;
            var topBackLabel = ((KeyedSubtree?)topComponents.backLabelKey.currentWidget)!;
            if (topBackLabel is null)
            {
                return null;
            }
            RenderAnimatedOpacity? topBackLabelOpacity = topComponents.backLabelKey.currentContext?.findAncestorRenderObjectOfType<RenderAnimatedOpacity>();
            Animation<double>? midClickOpacity = default!;
            if ((topBackLabelOpacity is not null) && (topBackLabelOpacity.opacity.value < 1.0))
            {
                midClickOpacity = animation.drive(new Tween<double>(begin: 0.0, end: topBackLabelOpacity.opacity.value));
            }
            if ((bottomLargeTitle is not null) && bottomLargeExpanded)
            {
                return (Widget?)slideFromLeadingEdge(fromKey: bottomComponents.largeTitleKey, fromNavBarBox: bottomNavBarBox, toKey: topComponents.backLabelKey, toNavBarBox: topNavBarBox, curve: new Interval(0.0, Equals(animation.status, AnimationStatus.forward) ? 0.7 : 1.0), child: new FadeTransition(opacity: midClickOpacity ?? fadeInFrom(0.4), child: new DefaultTextStyleTransition(style: animation.drive(new TextStyleTween(begin: bottomLargeTitleTextStyle, end: topBackButtonTextStyle)), maxLines: 1L, overflow: TextOverflow.ellipsis, child: topBackLabel.child)));
            }
            if (bottomMiddle is not null)
            {
                return (Widget?)slideFromLeadingEdge(fromKey: bottomComponents.middleKey, fromNavBarBox: bottomNavBarBox, toKey: topComponents.backLabelKey, toNavBarBox: topNavBarBox, child: new FadeTransition(opacity: midClickOpacity ?? fadeInFrom(0.3), child: new DefaultTextStyleTransition(style: animation.drive(new TextStyleTween(begin: bottomTitleTextStyle, end: topBackButtonTextStyle)), child: topBackLabel.child)));
            }
            return null;
        }
    }
    public virtual Widget? topMiddle
    {
        get
        {
            var topMiddle = ((KeyedSubtree?)topComponents.middleKey.currentWidget)!;
            if (topMiddle is null)
            {
                return null;
            }
            if (!topHasUserMiddle && topLargeExpanded)
            {
                return null;
            }
            RelativeRect to = positionInTransitionBox(topComponents.middleKey, from: topNavBarBox);
            var toBox = ((RenderBox?)topComponents.middleKey.currentContext!.findRenderObject()!)!;
            bool isLTRLocal = forwardDirection > 0L;
            var toAnchorInTransitionBox = new Offset(isLTRLocal ? to.left : to.right, to.top);
            var anchorMovementInTransitionBox = new Tween<Offset>(begin: new Offset(topNavBarBox.size.width - (toBox.size.width / 2L), to.top), end: toAnchorInTransitionBox);
            return (Widget?)new _FixedSizeSlidingTransition__nav_bar(isLTR: isLTRLocal, offsetAnimation: animation.drive(anchorMovementInTransitionBox), width: toBox.size.width, height: toBox.size.height, child: new FadeTransition(opacity: fadeInFrom(0.25), child: new DefaultTextStyle(style: topTitleTextStyle, child: topMiddle.child)));
        }
    }
    public virtual Widget? topTrailing
    {
        get
        {
            var topTrailing = ((KeyedSubtree?)topComponents.trailingKey.currentWidget)!;
            if (topTrailing is null)
            {
                return null;
            }
            return (Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(topComponents.trailingKey, from: topNavBarBox), child: new FadeTransition(opacity: fadeInFrom(0.4), child: topTrailing.child));
        }
    }
    public virtual Widget? topLargeTitle
    {
        get
        {
            var topLargeTitle = ((KeyedSubtree?)topComponents.largeTitleKey.currentWidget)!;
            if ((topLargeTitle is null) || !topLargeExpanded)
            {
                return null;
            }
            RelativeRect to = positionInTransitionBox(topComponents.largeTitleKey, from: topNavBarBox);
            var positionTween = new RelativeRectTween(begin: to.shift(new Offset(forwardDirection * topNavBarBox.size.width, 0.0)), end: to);
            Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Nav_barLibrary._kTopNavBarHeaderTransitionCurve : Nav_barLibrary._kTopNavBarHeaderTransitionCurve.flipped;
            return (Widget?)new PositionedTransition(rect: userGestureInProgress ? routeAnimation.drive(new CurveTween(curve: Curves.linear)).drive(positionTween) : animation.drive(new CurveTween(curve: animationCurve)).drive(positionTween), child: new FadeTransition(opacity: fadeInFrom(0.0, curve: animationCurve), child: new DefaultTextStyle(style: topLargeTitleTextStyle!, maxLines: 1L, overflow: TextOverflow.ellipsis, child: topLargeTitle.child)));
        }
    }
    public virtual Widget? topNavBarBottom
    {
        get
        {
            var topNavBarBottom = ((KeyedSubtree?)topComponents.navBarBottomKey.currentWidget)!;
            if (topNavBarBottom is null)
            {
                return null;
            }
            RelativeRect to = positionInTransitionBox(topComponents.navBarBottomKey, from: topNavBarBox);
            var positionTween = new RelativeRectTween(begin: to.shift(new Offset(forwardDirection * topNavBarBox.size.width, 0.0)), end: to);
            Widget childLocal = topNavBarBottom.child;
            Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Nav_barLibrary._kTopNavBarHeaderTransitionCurve : Nav_barLibrary._kTopNavBarHeaderTransitionCurve.flipped;
            if (!searchable)
            {
                childLocal = DartRuntimePrimitives.ConvertValue<Widget>(new FadeTransition(opacity: fadeInFrom(0.0, curve: animationCurve), child: childLocal));
            }
            return (Widget?)new PositionedTransition(rect: userGestureInProgress ? routeAnimation.drive(new CurveTween(curve: Curves.linear)).drive(positionTween) : animation.drive(new CurveTween(curve: animationCurve)).drive(positionTween), child: new ClipRect(child: childLocal));
        }
    }
}

public static partial class Nav_barLibrary
{
    internal static RectTween _linearTranslateWithLargestRectSizeTween(Rect? begin, Rect? end)
    {
        var largestSize = new Size(Math.Max(DartRuntimePrimitives.RequireValue(begin).size.width, DartRuntimePrimitives.RequireValue(end).size.width), Math.Max(DartRuntimePrimitives.RequireValue(begin).size.height, DartRuntimePrimitives.RequireValue(end).size.height));
        return new RectTween(begin: DartRuntimePrimitives.RequireValue(begin).topLeft & largestSize, end: DartRuntimePrimitives.RequireValue(end).topLeft & largestSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static Widget _navBarHeroLaunchPadBuilder(BuildContext context, Size heroSize, Widget child)
    {
        DartRuntimePrimitives.Assert(() => child is _TransitionableNavigationBar__nav_bar);
        return new Visibility(maintainSize: true, maintainAnimation: true, maintainState: true, visible: false, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static Widget _navBarHeroFlightShuttleBuilder(BuildContext flightContext, Animation<double> animation, HeroFlightDirection flightDirection, BuildContext fromHeroContext, BuildContext toHeroContext)
    {
        DartRuntimePrimitives.Assert(() => fromHeroContext.widget is Hero);
        DartRuntimePrimitives.Assert(() => toHeroContext.widget is Hero);
        var fromHeroWidget = ((Hero?)fromHeroContext.widget)!;
        var toHeroWidget = ((Hero?)toHeroContext.widget)!;
        DartRuntimePrimitives.Assert(() => fromHeroWidget.child is _TransitionableNavigationBar__nav_bar);
        DartRuntimePrimitives.Assert(() => toHeroWidget.child is _TransitionableNavigationBar__nav_bar);
        var fromNavBar = ((_TransitionableNavigationBar__nav_bar?)fromHeroWidget.child)!;
        var toNavBar = ((_TransitionableNavigationBar__nav_bar?)toHeroWidget.child)!;
        DartRuntimePrimitives.Assert(() => fromNavBar.componentsKeys.navBarBoxKey.currentContext!.owner is not null, () => (object?)"The from nav bar to Hero must have been mounted in the previous frame");
        DartRuntimePrimitives.Assert(() => toNavBar.componentsKeys.navBarBoxKey.currentContext!.owner is not null, () => (object?)"The to nav bar to Hero must have been mounted in the previous frame");
        switch (flightDirection)
        {
            case HeroFlightDirection.push:
                {
                    return new _NavigationBarTransition__nav_bar(animation: animation, bottomNavBar: fromNavBar, topNavBar: toNavBar);
                }
            case HeroFlightDirection.pop:
                {
                    return new _NavigationBarTransition__nav_bar(animation: animation, bottomNavBar: toNavBar, topNavBar: fromNavBar);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
