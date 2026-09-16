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
    internal static global::Doroti.Framework.Animation.Curve _kNavBarSearchCurve = Curves.easeInOut;
}

public static partial class Nav_barLibrary
{
    internal static Duration _kNavBarTitleFadeDuration = Duration.Create(milliseconds: 150L);
}

public static partial class Nav_barLibrary
{
    internal static Color _kDefaultNavBarBorderColor = new global::Doroti.Ui.Color(1291845632L);
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Painting.Border _kDefaultNavBarBorder = new global::Doroti.Framework.Painting.Border(bottom: new global::Doroti.Framework.Painting.BorderSide(color: _kDefaultNavBarBorderColor, width: 0.0));
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Painting.Border _kTransparentNavBarBorder = new global::Doroti.Framework.Painting.Border(bottom: new global::Doroti.Framework.Painting.BorderSide(color: new global::Doroti.Ui.Color(0L), width: 0.0));
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kTopNavBarHeaderTransitionCurve = new global::Doroti.Framework.Animation.Cubic(0.0, 0.45, 0.45, 0.98);
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kBottomNavBarHeaderTransitionCurve = new global::Doroti.Framework.Animation.Cubic(0.05, 0.9, 0.9, 0.95);
}

public static partial class Nav_barLibrary
{
    internal static _HeroTag__nav_bar _defaultHeroTag = new _HeroTag__nav_bar(null);
}

internal class _HeroTag__nav_bar
{
    public virtual global::Doroti.Framework.Widgets.NavigatorState? navigator { get; private set; }

    internal _HeroTag__nav_bar(global::Doroti.Framework.Widgets.NavigatorState? navigator)
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

public class _FixedSizeSlidingTransition__nav_bar : global::Doroti.Framework.Widgets.AnimatedWidget
{
    public virtual bool isLTR { get; private set; } = default!;
    public virtual double width { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<Offset> offsetAnimation { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _FixedSizeSlidingTransition__nav_bar(bool isLTR, global::Doroti.Framework.Animation.Animation<Offset> offsetAnimation, double width, double height, global::Doroti.Framework.Widgets.Widget child) : base(listenable: offsetAnimation)
    {
        this.isLTR = isLTR;
        this.offsetAnimation = offsetAnimation;
        this.width = width;
        this.height = height;
        this.child = child;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Positioned(top: offsetAnimation.value.dy, left: isLTR ? offsetAnimation.value.dx : null, right: isLTR ? null : offsetAnimation.value.dx, width: width, height: height, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _wrapWithBackground(global::Doroti.Framework.Painting.Border? border = null, Color backgroundColor = default!, Brightness? brightness = null, global::Doroti.Framework.Widgets.Widget child = default!, bool updateSystemUiOverlay = true, bool enableBackgroundFilterBlur = true)
    {
        var result = child;
        if (updateSystemUiOverlay)
        {
            bool isDark = backgroundColor.computeLuminance() < 0.179;
            global::Doroti.Ui.Brightness newBrightness = brightness ?? (isDark ? Brightness.dark : Brightness.light);
            global::Doroti.Framework.Services.SystemUiOverlayStyle overlayStyle = newBrightness switch { Brightness.dark => SystemUiOverlayStyle.light, Brightness.light => SystemUiOverlayStyle.dark, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") };
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnnotatedRegion<global::Doroti.Framework.Services.SystemUiOverlayStyle>(value: new global::Doroti.Framework.Services.SystemUiOverlayStyle(statusBarColor: overlayStyle.statusBarColor, statusBarBrightness: overlayStyle.statusBarBrightness, statusBarIconBrightness: overlayStyle.statusBarIconBrightness, systemStatusBarContrastEnforced: overlayStyle.systemStatusBarContrastEnforced), child: result));
        }
        var childWithBackground = new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: border, color: backgroundColor), child: result);
        return new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.BackdropFilter(enabled: (backgroundColor.alpha != 255L) && enableBackgroundFilterBlur, filter: new global::Doroti.Ui.ImageFilter(sigmaX: 10.0, sigmaY: 10.0), child: childWithBackground));
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
    internal static bool _isTransitionable(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.IModalRoute? route = ModalRoute<object>.untypedOf(context);
        return (route is IPageRoute) && !route.fullscreenDialog && !CupertinoSheetRoute<object>.hasParentSheet(context);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoNavigationBar : global::Doroti.Framework.Widgets.StatefulWidget, ObstructingPreferredSizeWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? largeTitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual bool automaticallyImplyMiddle { get; private set; } = default!;
    public virtual string? previousPageTitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? middle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.Border? border { get; private set; }
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom { get; private set; }

    public CupertinoNavigationBar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyMiddle = true, string? previousPageTitle = null, global::Doroti.Framework.Widgets.Widget? middle = null, global::Doroti.Framework.Widgets.Widget? trailing = null, global::Doroti.Framework.Painting.Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
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

    public static CupertinoNavigationBar CreateLarge(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? largeTitle = null, global::Doroti.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, string? previousPageTitle = null, global::Doroti.Framework.Widgets.Widget? trailing = null, global::Doroti.Framework.Painting.Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom = null)
    {
        var __instance = new CupertinoNavigationBar(key: key, leading: leading, automaticallyImplyLeading: automaticallyImplyLeading, previousPageTitle: previousPageTitle, trailing: trailing, border: border, backgroundColor: backgroundColor, automaticBackgroundVisibility: automaticBackgroundVisibility, enableBackgroundFilterBlur: enableBackgroundFilterBlur, brightness: brightness, padding: padding, transitionBetweenRoutes: transitionBetweenRoutes, heroTag: heroTag, bottom: bottom);
        global::Doroti.Framework.Painting.Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
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

    public virtual bool shouldFullyObstruct(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color backgroundColorLocal = CupertinoDynamicColor.maybeResolve(backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor;
        return backgroundColorLocal.alpha == 255L;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size preferredSize
    {
        get
        {
            double bottomHeight = bottom?.preferredSize.height ?? 0.0;
            double effectiveLargeHeight = (largeTitle is not null) ? Nav_barLibrary._kNavBarLargeTitleHeightExtension : 0.0;
            return new global::Doroti.Ui.Size(Nav_barLibrary._kNavBarPersistentHeight + bottomHeight + effectiveLargeHeight);
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoNavigationBarState__nav_bar());
}

internal class _CupertinoNavigationBarState__nav_bar : global::Doroti.Framework.Widgets.State<CupertinoNavigationBar>
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } = default;
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

    internal virtual void _handleScrollNotification(global::Doroti.Framework.Widgets.ScrollNotification notification)
    {
        if ((notification is global::Doroti.Framework.Widgets.ScrollUpdateNotification) && (((global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification).depth == 0L))
        {
            global::Doroti.Framework.Widgets.ScrollUpdateNotification notification__as27250 = (global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification;
            global::Doroti.Framework.Widgets.ScrollMetrics metricsLocal = notification__as27250.metrics;
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => (widget.middle is null) || (widget.largeTitle is null));
        global::Doroti.Ui.Color backgroundColorLocal = CupertinoDynamicColor.maybeResolve(widget.backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor;
        global::Doroti.Ui.Color? parentPageScaffoldBackgroundColor = CupertinoPageScaffoldBackgroundColor.maybeOf(context);
        global::Doroti.Framework.Painting.Border? initialBorder = (widget.automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? Nav_barLibrary._kTransparentNavBarBorder : widget.border;
        global::Doroti.Framework.Painting.Border? effectiveBorder = (widget.border is null) ? null : Border.lerp(initialBorder, widget.border, _scrollAnimationValue);
        global::Doroti.Ui.Color effectiveBackgroundColor = (widget.automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? (Dart_uiLibrary.Color.lerp(parentPageScaffoldBackgroundColor, backgroundColorLocal, _scrollAnimationValue) ?? backgroundColorLocal) : backgroundColorLocal;
        double bottomHeight = widget.bottom?.preferredSize.height ?? 0.0;
        double persistentHeight = Nav_barLibrary._kNavBarPersistentHeight + bottomHeight + MediaQuery.paddingOf(context).top;
        double largeHeight = persistentHeight + Nav_barLibrary._kNavBarLargeTitleHeightExtension;
        var componentsLocal = new _NavigationBarStaticComponents__nav_bar(keys: keys, route: ModalRoute<object>.untypedOf(context), userLeading: widget.leading, automaticallyImplyLeading: widget.automaticallyImplyLeading, automaticallyImplyTitle: widget.automaticallyImplyMiddle, previousPageTitle: widget.previousPageTitle, userMiddle: widget.middle, userTrailing: widget.trailing, padding: widget.padding, userLargeTitle: widget.largeTitle, userBottom: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(widget.bottom), large: widget.largeTitle is not null, staticBar: true, context: context);
        global::Doroti.Framework.Widgets.Widget navBar = new _PersistentNavigationBar__nav_bar(components: componentsLocal, padding: widget.padding, middleVisible: widget.largeTitle is null);
        if (widget.largeTitle is not null)
        {
            navBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: largeHeight), child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection31165 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(navBar)); __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Framework.Widgets.Semantics(header: true, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, maxLines: 1L, overflow: TextOverflow.ellipsis, child: new _LargeTitle__nav_bar(height: Nav_barLibrary._kNavBarLargeTitleHeightExtension, child: componentsLocal.largeTitle))))))); if (widget.bottom is not null) { __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: bottomHeight, child: componentsLocal.navBarBottom))); } return __collection31165; }))())));
        }
        else
        {
            navBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: persistentHeight), child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection32281 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection32281.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(navBar)); if (widget.bottom is not null) { __collection32281.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: bottomHeight, child: componentsLocal.navBarBottom))); } return __collection32281; }))())));
        }
        navBar = Nav_barLibrary._wrapWithBackground(border: effectiveBorder, backgroundColor: effectiveBackgroundColor, brightness: widget.brightness, enableBackgroundFilterBlur: widget.enableBackgroundFilterBlur, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.textStyle, child: navBar));
        if (!widget.transitionBetweenRoutes || !Nav_barLibrary._isTransitionable(context))
        {
            return navBar;
        }
        return new global::Doroti.Framework.Widgets.Builder(builder: (context) =>
        {
            return new global::Doroti.Framework.Widgets.Hero(tag: Equals(widget.heroTag, Nav_barLibrary._defaultHeroTag) ? new _HeroTag__nav_bar(Navigator.of(context)) : widget.heroTag, createRectTween: (global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.RectTween>)Nav_barLibrary._linearTranslateWithLargestRectSizeTween, placeholderBuilder: Nav_barLibrary._navBarHeroLaunchPadBuilder, flightShuttleBuilder: Nav_barLibrary._navBarHeroFlightShuttleBuilder, transitionOnUserGestures: true, child: new _TransitionableNavigationBar__nav_bar(componentsKeys: keys, backgroundColor: effectiveBackgroundColor, backButtonTextStyle: CupertinoTheme.of(context).textTheme.navActionTextStyle, titleTextStyle: CupertinoTheme.of(context).textTheme.navTitleTextStyle, largeTitleTextStyle: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, border: effectiveBorder, hasUserMiddle: widget.middle is not null, largeExpanded: widget.largeTitle is not null, searchable: false, automaticBackgroundVisibility: widget.automaticBackgroundVisibility, child: navBar));
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoSliverNavigationBar : global::Doroti.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Framework.Widgets.Widget? largeTitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual bool automaticallyImplyTitle { get; private set; } = default!;
    public virtual bool alwaysShowMiddle { get; private set; } = default!;
    public virtual string? previousPageTitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? middle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding { get; private set; }
    public virtual global::Doroti.Framework.Painting.Border? border { get; private set; }
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom { get; private set; }
    public virtual NavigationBarBottomMode? bottomMode { get; private set; }
    public virtual global::System.Action<bool>? onSearchableBottomTap { get; private set; }
    public virtual bool stretch { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? searchField { get; private set; }
    internal virtual bool _searchable { get; private set; } = default!;

    public CupertinoSliverNavigationBar(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget? largeTitle = null, global::Doroti.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, bool alwaysShowMiddle = true, string? previousPageTitle = null, global::Doroti.Framework.Widgets.Widget? middle = null, global::Doroti.Framework.Widgets.Widget? trailing = null, global::Doroti.Framework.Painting.Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, bool stretch = false, global::Doroti.Framework.Widgets.PreferredSizeWidget? bottom = null, NavigationBarBottomMode? bottomMode = null) : base(key: key)
    {
        global::Doroti.Framework.Painting.Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
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

    public static CupertinoSliverNavigationBar CreateSearch(global::Doroti.Framework.Foundation.Key? key = null, global::Doroti.Framework.Widgets.Widget searchField = default!, global::Doroti.Framework.Widgets.Widget? largeTitle = null, global::Doroti.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, bool alwaysShowMiddle = true, string? previousPageTitle = null, global::Doroti.Framework.Widgets.Widget? middle = null, global::Doroti.Framework.Widgets.Widget? trailing = null, global::Doroti.Framework.Painting.Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, bool stretch = false, NavigationBarBottomMode? bottomMode = NavigationBarBottomMode.automatic, global::System.Action<bool>? onSearchableBottomTap = null)
    {
        var __instance = new CupertinoSliverNavigationBar(key: key, largeTitle: largeTitle, leading: leading, automaticallyImplyLeading: automaticallyImplyLeading, automaticallyImplyTitle: automaticallyImplyTitle, alwaysShowMiddle: alwaysShowMiddle, previousPageTitle: previousPageTitle, middle: middle, trailing: trailing, border: border, backgroundColor: backgroundColor, automaticBackgroundVisibility: automaticBackgroundVisibility, enableBackgroundFilterBlur: enableBackgroundFilterBlur, brightness: brightness, padding: padding, transitionBetweenRoutes: transitionBetweenRoutes, heroTag: heroTag, stretch: stretch, bottomMode: bottomMode);
        global::Doroti.Framework.Painting.Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
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

internal class _CupertinoSliverNavigationBarState__nav_bar : global::Doroti.Framework.Widgets.State<CupertinoSliverNavigationBar>, global::Doroti.Framework.Widgets.TickerProviderStateMixin<CupertinoSliverNavigationBar>
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; set; } = default!;
    internal virtual global::Doroti.Framework.Widgets.ScrollableState? _scrollableState { get; set; } = default;
    public virtual global::Doroti.Framework.Widgets.Widget? effectiveMiddle { get; set; } = default;
    internal virtual global::Doroti.Framework.Animation.AnimationController _animationController { get; set; } = default!;
    internal virtual global::Doroti.Framework.Animation.CurvedAnimation _searchAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> persistentHeightAnimation { get; set; } = default!;
    public virtual global::Doroti.Framework.Animation.Animation<double> largeTitleHeightAnimation { get; set; } = default!;
    public virtual double scaledSearchFieldHeight { get; set; } = default!;
    public virtual double scaledLargeTitleHeight { get; set; } = default!;
    public virtual bool searchIsActive { get; set; } = false;
    public virtual bool isPortrait { get; set; } = true;
    public virtual HashSet<global::Doroti.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        keys = new _NavigationBarStaticComponentsKeys__nav_bar();
        _animationController = new global::Doroti.Framework.Animation.AnimationController(vsync: this, duration: Nav_barLibrary._kNavBarSearchDuration);
        _searchAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: _animationController, curve: Nav_barLibrary._kNavBarSearchCurve);
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
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
                    {
                        if (ticker.isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
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
        global::Doroti.Framework.Painting.TextScaler textScaler = MediaQuery.textScalerOf(context);
        scaledSearchFieldHeight = Nav_barLibrary._kSearchFieldHeight * Nav_barLibrary._dampScaleFactor(textScaler.scale(Nav_barLibrary._kSearchFieldHeight), Nav_barLibrary._kSearchFieldHeight, Nav_barLibrary._kMaxScaleFactor);
        scaledLargeTitleHeight = isPortrait ? (Nav_barLibrary._kNavBarLargeTitleHeightExtension * Nav_barLibrary._dampScaleFactor(textScaler.scale(Nav_barLibrary._kNavBarLargeTitleHeightExtension), Nav_barLibrary._kNavBarLargeTitleHeightExtension, Nav_barLibrary._kLargeTitleScaleDampingRatio)) : 0.0;
    }

    internal virtual void _setupSearchableAnimation()
    {
        var persistentHeightTween = new global::Doroti.Framework.Animation.Tween<double>(begin: Nav_barLibrary._kNavBarPersistentHeight, end: 0.0);
        persistentHeightAnimation = ((Func<global::Doroti.Framework.Animation.Animation<double>>)(() =>
{
    var __cascade = persistentHeightTween.animate(_animationController);
    __cascade.addStatusListener(_handleSearchFieldStatusChanged);
    return __cascade;
}))();
        var largeTitleHeightTween = new global::Doroti.Framework.Animation.Tween<double>(begin: scaledLargeTitleHeight, end: 0.0);
        largeTitleHeightAnimation = largeTitleHeightTween.animate(_animationController);
    }

    internal virtual void _handleScrollChange()
    {
        global::Doroti.Framework.Widgets.ScrollPosition? positionLocal = _scrollableState?.position;
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

    internal virtual void _handleSearchFieldStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
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

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var componentsLocal = new _NavigationBarStaticComponents__nav_bar(keys: keys, route: ModalRoute<object>.untypedOf(context), userLeading: (widget.leading is not null) ? new global::Doroti.Framework.Widgets.Visibility(visible: !searchIsActive, child: widget.leading!) : null, automaticallyImplyLeading: widget.automaticallyImplyLeading, automaticallyImplyTitle: widget.automaticallyImplyTitle, previousPageTitle: widget.previousPageTitle, userMiddle: _animationController.isAnimating ? new global::Doroti.Framework.Widgets.Text("") : effectiveMiddle, userTrailing: (widget.trailing is not null) ? new global::Doroti.Framework.Widgets.Visibility(visible: !searchIsActive, child: widget.trailing!) : null, userLargeTitle: widget.largeTitle, userBottom: (widget._searchable ? (searchIsActive ? new _ActiveSearchableBottom__nav_bar(animationController: _animationController, animation: persistentHeightAnimation, searchField: widget.searchField, searchFieldHeight: scaledSearchFieldHeight, onSearchFieldTap: () => _onSearchFieldTap()) : new _InactiveSearchableBottom__nav_bar(animationController: _animationController, animation: persistentHeightAnimation, searchField: widget.searchField, searchFieldHeight: scaledSearchFieldHeight, onSearchFieldTap: () => _onSearchFieldTap())) : (global::Doroti.Framework.Widgets.Widget?)widget.bottom) ?? SizedBox.CreateShrink(), padding: widget.padding, large: isPortrait, staticBar: false, context: context);
        return MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: _searchAnimation, builder: (context, child) =>
        {
            return new global::Doroti.Framework.Widgets.SliverPersistentHeader(pinned: true, @delegate: new _LargeTitleNavigationBarSliverDelegate__nav_bar(keys: keys, components: componentsLocal, userMiddle: effectiveMiddle, backgroundColor: CupertinoDynamicColor.maybeResolve(widget.backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor, automaticBackgroundVisibility: widget.automaticBackgroundVisibility, brightness: widget.brightness, border: widget.border, padding: widget.padding, actionsForegroundColor: CupertinoTheme.of(context).primaryColor, transitionBetweenRoutes: widget.transitionBetweenRoutes, heroTag: widget.heroTag, persistentHeight: persistentHeightAnimation.value + MediaQuery.paddingOf(context).top, largeTitleHeight: largeTitleHeightAnimation.value, alwaysShowMiddle: widget.alwaysShowMiddle && (effectiveMiddle is not null), stretchConfiguration: (widget.stretch && !searchIsActive) ? new global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration() : null, enableBackgroundFilterBlur: widget.enableBackgroundFilterBlur, bottomMode: searchIsActive ? NavigationBarBottomMode.always : (widget.bottomMode ?? NavigationBarBottomMode.automatic), bottomHeight: _bottomHeight, controller: _animationController, searchable: widget._searchable));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if (_tickerModeNotifier is null)
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => _tickerModeNotifier is not null);
        _tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = _tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
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

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in _tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = values.forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = TickerMode.getValuesNotifier(context);
        if (Equals(newNotifier, _tickerModeNotifier))
        {
            return;
        }
        _tickerModeNotifier?.removeListener(_updateTickers);
        newNotifier.addListener(_updateTickers);
        _tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", _tickers, description: (_tickers is not null) ? $"tracking {checked((long)_tickers!.Count)} ticker{((checked(_tickers!.Count) == 1L) ? "" : "s")}" : null, defaultValue: default));
    }

}

internal class _LargeTitleNavigationBarSliverDelegate__nav_bar : global::Doroti.Framework.Widgets.SliverPersistentHeaderDelegate
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; private set; } = default!;
    public virtual _NavigationBarStaticComponents__nav_bar components { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? userMiddle { get; private set; }
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual global::Doroti.Framework.Painting.Border? border { get; private set; }
    public virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding { get; private set; }
    public virtual Color actionsForegroundColor { get; private set; } = default!;
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual double persistentHeight { get; private set; } = default!;
    public virtual double largeTitleHeight { get; private set; } = default!;
    public virtual bool alwaysShowMiddle { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual NavigationBarBottomMode bottomMode { get; private set; } = default!;
    public virtual double bottomHeight { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.AnimationController controller { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    private global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? __field_stretchConfiguration = default;
    public override global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration => __field_stretchConfiguration;

    internal _LargeTitleNavigationBarSliverDelegate__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar keys, _NavigationBarStaticComponents__nav_bar components, global::Doroti.Framework.Widgets.Widget? userMiddle, Color backgroundColor, bool automaticBackgroundVisibility, Brightness? brightness, global::Doroti.Framework.Painting.Border? border, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding, Color actionsForegroundColor, bool transitionBetweenRoutes, object heroTag, double persistentHeight, double largeTitleHeight, bool alwaysShowMiddle, global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration, bool enableBackgroundFilterBlur, NavigationBarBottomMode bottomMode, double bottomHeight, global::Doroti.Framework.Animation.AnimationController controller, bool searchable)
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
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context, double shrinkOffset, bool overlapsContent)
    {
        double largeTitleThreshold = maxExtent - minExtent - Nav_barLibrary._kNavBarShowLargeTitleThreshold;
        bool showLargeTitle = shrinkOffset < largeTitleThreshold;
        double bottomShrinkFactor = Dart_uiLibrary.clampDouble(shrinkOffset / bottomHeight, 0, 1);
        double shrinkAnimationValue = Dart_uiLibrary.clampDouble((shrinkOffset - largeTitleThreshold - Nav_barLibrary._kNavBarScrollUnderAnimationExtent) / Nav_barLibrary._kNavBarScrollUnderAnimationExtent, 0, 1);
        var persistentNavigationBar = new _PersistentNavigationBar__nav_bar(components: components, padding: padding, middleVisible: alwaysShowMiddle ? null : !showLargeTitle);
        global::Doroti.Ui.Color? parentPageScaffoldBackgroundColor = CupertinoPageScaffoldBackgroundColor.maybeOf(context);
        global::Doroti.Framework.Painting.Border? initialBorder = (automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? Nav_barLibrary._kTransparentNavBarBorder : border;
        global::Doroti.Framework.Painting.Border? effectiveBorder = (border is null) ? null : Border.lerp(initialBorder, border, shrinkAnimationValue);
        global::Doroti.Ui.Color effectiveBackgroundColor = (automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? (Dart_uiLibrary.Color.lerp(parentPageScaffoldBackgroundColor, backgroundColor, shrinkAnimationValue) ?? backgroundColor) : backgroundColor;
        global::Doroti.Framework.Widgets.Widget navBar = Nav_barLibrary._wrapWithBackground(border: effectiveBorder, backgroundColor: effectiveBackgroundColor, brightness: brightness, enableBackgroundFilterBlur: enableBackgroundFilterBlur, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.textStyle, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection60282 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection60282.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection60368 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(top: persistentHeight, left: 0.0, right: 0.0, bottom: Equals(bottomMode, NavigationBarBottomMode.automatic) ? (bottomHeight * (1.0 - bottomShrinkFactor)) : 0.0, child: new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Framework.Widgets.SafeArea(top: false, bottom: false, child: new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: (showLargeTitle && !controller.isForwardOrCompleted) ? 1.0 : 0.0, duration: Nav_barLibrary._kNavBarTitleFadeDuration, child: new global::Doroti.Framework.Widgets.Semantics(header: true, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, maxLines: 1L, overflow: TextOverflow.ellipsis, child: new _LargeTitle__nav_bar(height: largeTitleHeight, child: components.largeTitle)))))))))); __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(left: 0.0, right: 0.0, top: 0.0, child: persistentNavigationBar))); if (Equals(bottomMode, NavigationBarBottomMode.automatic)) { __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(left: 0.0, right: 0.0, bottom: 0.0, child: new global::Doroti.Framework.Widgets.SizedBox(height: bottomHeight * (1.0 - bottomShrinkFactor), child: new global::Doroti.Framework.Widgets.ClipRect(child: components.navBarBottom))))); } return __collection60368; }))())))); if (Equals(bottomMode, NavigationBarBottomMode.always)) { __collection60282.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: bottomHeight, child: components.navBarBottom))); } return __collection60282; }))())));
        if (!transitionBetweenRoutes || !Nav_barLibrary._isTransitionable(context))
        {
            return navBar;
        }
        return new global::Doroti.Framework.Widgets.Hero(tag: Equals(heroTag, Nav_barLibrary._defaultHeroTag) ? new _HeroTag__nav_bar(Navigator.of(context)) : heroTag, createRectTween: (global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.RectTween>)Nav_barLibrary._linearTranslateWithLargestRectSizeTween, flightShuttleBuilder: Nav_barLibrary._navBarHeroFlightShuttleBuilder, placeholderBuilder: Nav_barLibrary._navBarHeroLaunchPadBuilder, transitionOnUserGestures: true, child: new _TransitionableNavigationBar__nav_bar(componentsKeys: keys, backgroundColor: effectiveBackgroundColor, backButtonTextStyle: CupertinoTheme.of(context).textTheme.navActionTextStyle, titleTextStyle: CupertinoTheme.of(context).textTheme.navTitleTextStyle, largeTitleTextStyle: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, border: effectiveBorder, hasUserMiddle: (userMiddle is not null) && (alwaysShowMiddle || !showLargeTitle), largeExpanded: showLargeTitle, searchable: searchable, automaticBackgroundVisibility: automaticBackgroundVisibility, child: navBar));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(global::Doroti.Framework.Widgets.SliverPersistentHeaderDelegate oldDelegate)
    {
        var __oldDelegate = (_LargeTitleNavigationBarSliverDelegate__nav_bar)oldDelegate;
        return (!Equals(components, __oldDelegate.components)) || (!Equals(userMiddle, __oldDelegate.userMiddle)) || (!Equals(backgroundColor, __oldDelegate.backgroundColor)) || (automaticBackgroundVisibility != __oldDelegate.automaticBackgroundVisibility) || (!Equals(border, __oldDelegate.border)) || (!Equals(padding, __oldDelegate.padding)) || (!Equals(actionsForegroundColor, __oldDelegate.actionsForegroundColor)) || (transitionBetweenRoutes != __oldDelegate.transitionBetweenRoutes) || (persistentHeight != __oldDelegate.persistentHeight) || (largeTitleHeight != __oldDelegate.largeTitleHeight) || (alwaysShowMiddle != __oldDelegate.alwaysShowMiddle) || (!Equals(heroTag, __oldDelegate.heroTag)) || (enableBackgroundFilterBlur != __oldDelegate.enableBackgroundFilterBlur) || (!Equals(bottomMode, __oldDelegate.bottomMode)) || (bottomHeight != __oldDelegate.bottomHeight) || (!Equals(controller, __oldDelegate.controller)) || (searchable != __oldDelegate.searchable);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _LargeTitle__nav_bar : global::Doroti.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual double height { get; private set; } = default!;

    internal _LargeTitle__nav_bar(global::Doroti.Framework.Widgets.Widget? child = null, double height = default!) : base(child: child)
    {
        this.height = height;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new _RenderLargeTitle__nav_bar(alignment: AlignmentDirectional.bottomStart.resolve(Directionality.of(context)), height: height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
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

public class _RenderLargeTitle__nav_bar : global::Doroti.Framework.Rendering.RenderShiftedBox
{
    internal virtual global::Doroti.Framework.Painting.Alignment _alignment { get; set; } = default!;
    internal virtual double _height { get; set; } = default!;
    internal virtual double _scale { get; set; } = 1.0;

    internal _RenderLargeTitle__nav_bar(global::Doroti.Framework.Painting.Alignment alignment, double height) : base(null)
    {
        _alignment = alignment;
        _height = height;
    }

    public virtual global::Doroti.Framework.Painting.Alignment alignment
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
    internal static double _computeTitleScale(Size childSize, global::Doroti.Framework.Rendering.BoxConstraints constraints, double height)
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
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)child!.parentData!)!;
        return childParentData.offset.dy + (DartRuntimePrimitives.RequireValue(distance) * _scale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return null;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = constraints.widthConstraints().loosen();
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if (result is null)
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = childLocal.getDryLayout(childConstraints);
        double scale = _computeTitleScale(childSize, constraints, height);
        global::Doroti.Ui.Size scaledChildSize = childSize * scale;
        return (DartRuntimePrimitives.RequireValue(result) * scale) + alignment.alongOffset(constraints.biggest - scaledChildSize).dy;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        size = constraints.biggest;
        if (childLocal is null)
        {
            return;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = constraints.widthConstraints().loosen();
        childLocal.layout(childConstraints, parentUsesSize: true);
        _scale = _computeTitleScale(childLocal.size, constraints, height);
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)childLocal.parentData!)!;
        childParentData.offset = alignment.alongOffset(size - childLocal.size * _scale);
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)child;
        DartRuntimePrimitives.Assert(() => Equals(__child, this.child));
        base.applyPaintTransform(__child, transform);
        transform.scaleByDouble(_scale, _scale, _scale, 1);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            layer = null;
        }
        else
        {
            var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)childLocal.parentData!)!;
            layer = context.pushTransform(needsCompositing, offset + childParentData.offset, Matrix4.diagonal3Values(_scale, _scale, 1.0), (context, offset) => { context.paintChild(childLocal, offset); }, oldLayer: ((global::Doroti.Framework.Rendering.TransformLayer?)layer)!);
        }
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = child;
        if (childLocal is null)
        {
            return false;
        }
        global::Doroti.Ui.Offset childOffset = ((BoxParentData?)childLocal.parentData!)!.offset;
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

internal class _PersistentNavigationBar__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual _NavigationBarStaticComponents__nav_bar components { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding { get; private set; }
    public virtual bool? middleVisible { get; private set; }

    internal _PersistentNavigationBar__nav_bar(_NavigationBarStaticComponents__nav_bar components, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding = null, bool? middleVisible = null)
    {
        this.components = components;
        this.padding = padding;
        this.middleVisible = middleVisible;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget? middleLocal = components.middle;
        if (middleLocal is not null)
        {
            middleLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navTitleTextStyle, child: new global::Doroti.Framework.Widgets.Semantics(header: true, child: middleLocal)));
            middleLocal = (middleVisible is null) ? middleLocal : new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: DartRuntimePrimitives.RequireValue(middleVisible) ? 1.0 : 0.0, duration: Nav_barLibrary._kNavBarTitleFadeDuration, child: middleLocal);
        }
        global::Doroti.Framework.Widgets.Widget? leadingLocal = components.leading;
        global::Doroti.Framework.Widgets.Widget? backChevronLocal = components.backChevron;
        global::Doroti.Framework.Widgets.Widget? backLabelLocal = components.backLabel;
        if ((leadingLocal is null) && (backChevronLocal is not null) && (backLabelLocal is not null) && !CupertinoSheetRoute<object>.hasParentSheet(context))
        {
            leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(CupertinoNavigationBarBackButton.Create_assemble(backChevronLocal, backLabelLocal));
        }
        else
        {
            leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(widthFactor: 1.0, child: leadingLocal));
        }
        global::Doroti.Framework.Widgets.Widget paddedToolbar = new global::Doroti.Framework.Widgets.NavigationToolbar(leading: leadingLocal, middle: middleLocal, trailing: components.trailing, middleSpacing: 6.0);
        if (padding is not null)
        {
            paddedToolbar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(top: padding!.top, bottom: padding!.bottom), child: paddedToolbar));
        }
        return new global::Doroti.Framework.Widgets.SizedBox(height: Nav_barLibrary._kNavBarPersistentHeight + MediaQuery.paddingOf(context).top, child: new global::Doroti.Framework.Widgets.SafeArea(top: !CupertinoSheetRoute<object>.hasParentSheet(context), bottom: false, child: paddedToolbar));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _NavigationBarStaticComponentsKeys__nav_bar
{
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> navBarBoxKey { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> leadingKey { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> backChevronKey { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> backLabelKey { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> middleKey { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> trailingKey { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> largeTitleKey { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.GlobalKey<IState> navBarBottomKey { get; private set; } = default!;

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
    public virtual global::Doroti.Framework.Widgets.KeyedSubtree? leading { get; private set; }
    public virtual global::Doroti.Framework.Widgets.KeyedSubtree? backChevron { get; private set; }
    public virtual global::Doroti.Framework.Widgets.KeyedSubtree? backLabel { get; private set; }
    public virtual global::Doroti.Framework.Widgets.KeyedSubtree? middle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.KeyedSubtree? trailing { get; private set; }
    public virtual global::Doroti.Framework.Widgets.KeyedSubtree? largeTitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.KeyedSubtree? navBarBottom { get; private set; }

    internal _NavigationBarStaticComponents__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar keys, global::Doroti.Framework.Widgets.IModalRoute? route, global::Doroti.Framework.Widgets.Widget? userLeading, bool automaticallyImplyLeading, bool automaticallyImplyTitle, string? previousPageTitle, global::Doroti.Framework.Widgets.Widget? userMiddle, global::Doroti.Framework.Widgets.Widget? userTrailing, global::Doroti.Framework.Widgets.Widget? userLargeTitle, global::Doroti.Framework.Widgets.Widget? userBottom, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding, bool large, bool staticBar, global::Doroti.Framework.Widgets.BuildContext context)
    {
        leading = createLeading(leadingKey: keys.leadingKey, userLeading: userLeading, route: route, automaticallyImplyLeading: automaticallyImplyLeading, padding: padding, context: context);
        backChevron = createBackChevron(backChevronKey: keys.backChevronKey, userLeading: userLeading, route: route, automaticallyImplyLeading: automaticallyImplyLeading, context: context);
        backLabel = createBackLabel(backLabelKey: keys.backLabelKey, userLeading: userLeading, route: route, previousPageTitle: previousPageTitle, automaticallyImplyLeading: automaticallyImplyLeading, context: context);
        middle = createMiddle(middleKey: keys.middleKey, userMiddle: userMiddle, userLargeTitle: userLargeTitle, route: route, automaticallyImplyTitle: automaticallyImplyTitle, large: large, staticBar: staticBar, context: context);
        trailing = createTrailing(trailingKey: keys.trailingKey, userTrailing: userTrailing, padding: padding, context: context);
        largeTitle = createLargeTitle(largeTitleKey: keys.largeTitleKey, userLargeTitle: userLargeTitle, route: route, automaticImplyTitle: automaticallyImplyTitle, large: large, context: context);
        navBarBottom = createNavBarBottom(navBarBottomKey: keys.navBarBottomKey, userBottom: userBottom, context: context);
    }

    internal static global::Doroti.Framework.Widgets.Widget? _derivedTitle(bool automaticallyImplyTitle, global::Doroti.Framework.Widgets.IModalRoute? currentRoute = null)
    {
        if (automaticallyImplyTitle && (currentRoute is ICupertinoRouteTitle) && (((ICupertinoRouteTitle)currentRoute).title is not null))
        {
            ICupertinoRouteTitle currentRoute__as76488 = (ICupertinoRouteTitle)currentRoute;
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.Text(currentRoute__as76488.title!);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createLeading(global::Doroti.Framework.Widgets.GlobalKey<IState> leadingKey, global::Doroti.Framework.Widgets.Widget? userLeading, global::Doroti.Framework.Widgets.IModalRoute? route, bool automaticallyImplyLeading, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding, global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget? leadingContent = default!;
        if (userLeading is not null)
        {
            leadingContent = userLeading;
        }
        else
        {
            if (automaticallyImplyLeading && (route is IPageRoute) && route.canPop && route.fullscreenDialog)
            {
                var route__as77104 = route;
                leadingContent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new CupertinoButton(padding: EdgeInsets.zero, onPressed: () =>
                {
                    DartRuntimePrimitives.Ignore(route__as77104.navigator!.maybePop<object>());
                }, child: new global::Doroti.Framework.Widgets.Text(CupertinoLocalizations.of(context).cancelButtonLabel)));
            }
        }
        if (leadingContent is null)
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: leadingKey, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: padding?.start ?? Nav_barLibrary._kNavBarEdgePadding), child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(size: 32.0), child: leadingContent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createBackChevron(global::Doroti.Framework.Widgets.GlobalKey<IState> backChevronKey, global::Doroti.Framework.Widgets.Widget? userLeading, global::Doroti.Framework.Widgets.IModalRoute? route, bool automaticallyImplyLeading, global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((userLeading is not null) || !automaticallyImplyLeading || (route is null) || !route.canPop || (route is IPageRoute) && route.fullscreenDialog)
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: backChevronKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: new _BackChevron__nav_bar()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createBackLabel(global::Doroti.Framework.Widgets.GlobalKey<IState> backLabelKey, global::Doroti.Framework.Widgets.Widget? userLeading, global::Doroti.Framework.Widgets.IModalRoute? route, bool automaticallyImplyLeading, string? previousPageTitle, global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((userLeading is not null) || !automaticallyImplyLeading || (route is null) || !route.canPop || (route is IPageRoute) && route.fullscreenDialog)
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: backLabelKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: new _BackLabel__nav_bar(specifiedPreviousTitle: previousPageTitle, route: route)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createMiddle(global::Doroti.Framework.Widgets.GlobalKey<IState> middleKey, global::Doroti.Framework.Widgets.Widget? userMiddle, global::Doroti.Framework.Widgets.Widget? userLargeTitle, bool large, bool staticBar, bool automaticallyImplyTitle, global::Doroti.Framework.Widgets.IModalRoute? route, global::Doroti.Framework.Widgets.BuildContext context)
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
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: middleKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: middleContent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createTrailing(global::Doroti.Framework.Widgets.GlobalKey<IState> trailingKey, global::Doroti.Framework.Widgets.Widget? userTrailing, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding, global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (userTrailing is null)
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: trailingKey, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(end: padding?.end ?? Nav_barLibrary._kNavBarEdgePadding), child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _clampedTextScaler(context)), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(size: 32.0), child: userTrailing))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createLargeTitle(global::Doroti.Framework.Widgets.GlobalKey<IState> largeTitleKey, global::Doroti.Framework.Widgets.Widget? userLargeTitle, bool large, bool automaticImplyTitle, global::Doroti.Framework.Widgets.IModalRoute? route, global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!large)
        {
            return null;
        }
        global::Doroti.Framework.Widgets.Widget? largeTitleContent = userLargeTitle ?? _derivedTitle(automaticallyImplyTitle: automaticImplyTitle, currentRoute: route);
        DartRuntimePrimitives.Assert(() => largeTitleContent is not null, () => (object?)"largeTitle was not provided and there was no title from the route.");
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: largeTitleKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: TextScaler.CreateLinear(Nav_barLibrary._dampScaleFactor(MediaQuery.textScalerOf(context).scale(Nav_barLibrary._kNavBarLargeTitleHeightExtension), Nav_barLibrary._kNavBarLargeTitleHeightExtension, Nav_barLibrary._kLargeTitleScaleDampingRatio))), child: largeTitleContent!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createNavBarBottom(global::Doroti.Framework.Widgets.GlobalKey<IState> navBarBottomKey, global::Doroti.Framework.Widgets.Widget? userBottom, global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: navBarBottomKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: MediaQuery.textScalerOf(context)), child: userBottom ?? SizedBox.CreateShrink()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Painting.TextScaler _clampedTextScaler(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return MediaQuery.textScalerOf(context).clamp(minScaleFactor: 1.0, maxScaleFactor: Nav_barLibrary._kMaxScaleFactor);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoNavigationBarBackButton : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual Color? color { get; private set; }
    public virtual string? previousPageTitle { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    internal virtual global::Doroti.Framework.Widgets.Widget? _backChevron { get; private set; }
    internal virtual global::Doroti.Framework.Widgets.Widget? _backLabel { get; private set; }

    public CupertinoNavigationBarBackButton(global::Doroti.Framework.Foundation.Key? key = null, Color? color = null, string? previousPageTitle = null, global::System.Action? onPressed = null) : base(key: key)
    {
        this.color = color;
        this.previousPageTitle = previousPageTitle;
        this.onPressed = onPressed;
        _backChevron = null;
        _backLabel = null;
    }

    public static CupertinoNavigationBarBackButton Create_assemble(global::Doroti.Framework.Widgets.Widget? _backChevron, global::Doroti.Framework.Widgets.Widget? _backLabel)
    {
        var __instance = new CupertinoNavigationBarBackButton();
        __instance._backChevron = _backChevron;
        __instance._backLabel = _backLabel;
        __instance.previousPageTitle = null;
        __instance.color = null;
        __instance.onPressed = null;
        return __instance;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.IModalRoute? currentRoute = ModalRoute<object>.untypedOf(context);
        if (onPressed is null)
        {
            DartRuntimePrimitives.Assert(() => (currentRoute?.canPop) ?? false, () => (object?)"CupertinoNavigationBarBackButton should only be used in routes that can be popped");
        }
        global::Doroti.Framework.Painting.TextStyle actionTextStyle = CupertinoTheme.of(context).textTheme.navActionTextStyle;
        if (color is not null)
        {
            actionTextStyle = actionTextStyle.copyWith(color: CupertinoDynamicColor.maybeResolve(color, context));
        }
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        return new CupertinoButton(padding: EdgeInsets.zero, child: new global::Doroti.Framework.Widgets.Semantics(container: true, excludeSemantics: true, label: localizations.backButtonLabel, button: true, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: actionTextStyle, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: Nav_barLibrary._kNavBarBackButtonTapWidth), child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 8.0))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(_backChevron ?? new _BackChevron__nav_bar()), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 6.0))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: _backLabel ?? new _BackLabel__nav_bar(specifiedPreviousTitle: previousPageTitle, route: currentRoute))) })))), onPressed: () =>
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

internal class _BackChevron__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    internal _BackChevron__nav_bar()
    {
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirection = Directionality.of(context);
        global::Doroti.Framework.Painting.TextStyle textStyle = DefaultTextStyle.of(context).style;
        global::Doroti.Framework.Widgets.Widget iconWidget = new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: 6, end: 2), child: Text.CreateRich(new global::Doroti.Framework.Painting.TextSpan(text: char.ConvertFromUtf32(checked((int)CupertinoIcons.back.codePoint)), style: new global::Doroti.Framework.Painting.TextStyle(inherit: false, color: textStyle.color, fontSize: 30.0, fontFamily: CupertinoIcons.back.fontFamily, package: CupertinoIcons.back.fontPackage))));
        switch (textDirection)
        {
            case TextDirection.rtl:
                {
                    iconWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Transform(transform: ((Func<Matrix4>)(() =>
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
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: StandardComponentTypeMembers.key(StandardComponentType.backButton), child: iconWidget);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BackLabel__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual string? specifiedPreviousTitle { get; private set; }
    public virtual global::Doroti.Framework.Widgets.IModalRoute? route { get; private set; } = default!;

    internal _BackLabel__nav_bar(string? specifiedPreviousTitle, global::Doroti.Framework.Widgets.IModalRoute? route)
    {
        this.specifiedPreviousTitle = specifiedPreviousTitle;
        this.route = route;
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildPreviousTitleWidget(global::Doroti.Framework.Widgets.BuildContext context, string? previousTitle, global::Doroti.Framework.Widgets.Widget? child)
    {
        if (previousTitle is null)
        {
            return SizedBox.CreateShrink();
        }
        var textWidget = new global::Doroti.Framework.Widgets.Text(previousTitle, maxLines: 1L, overflow: TextOverflow.ellipsis);
        if (previousTitle.Length > 12L)
        {
            textWidget = new global::Doroti.Framework.Widgets.Text(CupertinoLocalizations.of(context).backButtonLabel);
        }
        return new global::Doroti.Framework.Widgets.Align(alignment: AlignmentDirectional.centerStart, widthFactor: 1.0, child: textWidget);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
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
                return new global::Doroti.Framework.Widgets.ValueListenableBuilder<string?>(valueListenable: cupertinoRoute.previousTitle, builder: _buildPreviousTitleWidget);
            }
            else
            {
                return SizedBox.CreateShrink();
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CancelButton__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual double opacity { get; private set; } = default!;

    internal _CancelButton__nav_bar(double opacity = 1.0, global::System.Action? onPressed = default!)
    {
        this.opacity = opacity;
        this.onPressed = onPressed;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        return MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.Align(alignment: Alignment.centerLeft, child: new global::Doroti.Framework.Widgets.Opacity(opacity: opacity, child: new CupertinoButton(padding: EdgeInsets.zero, onPressed: onPressed, child: new global::Doroti.Framework.Widgets.Text(localizations.cancelButtonLabel, maxLines: 1L, overflow: TextOverflow.clip)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InactiveSearchableBottom__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.AnimationController animationController { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? searchField { get; private set; }
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual double searchFieldHeight { get; private set; } = default!;
    public virtual global::System.Action? onSearchFieldTap { get; private set; }

    internal _InactiveSearchableBottom__nav_bar(global::Doroti.Framework.Animation.AnimationController animationController, global::Doroti.Framework.Widgets.Widget? searchField, global::Doroti.Framework.Animation.Animation<double> animation, double searchFieldHeight, global::System.Action? onSearchFieldTap)
    {
        this.animationController = animationController;
        this.searchField = searchField;
        this.animation = animation;
        this.searchFieldHeight = searchFieldHeight;
        this.onSearchFieldTap = onSearchFieldTap;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: animation, child: new global::Doroti.Framework.Widgets.GestureDetector(onTap: onSearchFieldTap, child: new global::Doroti.Framework.Widgets.AbsorbPointer(child: new global::Doroti.Framework.Widgets.FocusableActionDetector(descendantsAreFocusable: false, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, end: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Framework.Widgets.SizedBox(height: searchFieldHeight, child: searchField))))), builder: (context, child) =>
        {
            return new global::Doroti.Framework.Widgets.LayoutBuilder(builder: (context, constraints) =>
            {
                return new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: constraints.maxWidth - Nav_barLibrary._kSearchFieldCancelButtonWidth * animationController.value, child: child)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: animationController.value * Nav_barLibrary._kSearchFieldCancelButtonWidth, child: new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsets.CreateOnly(bottom: Nav_barLibrary._kNavBarBottomPadding), child: new _CancelButton__nav_bar(opacity: 0.4, onPressed: () => {
})))) });
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
            throw new InvalidOperationException("Dart closure completed without a value.");
        });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActiveSearchableBottom__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.AnimationController animationController { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget? searchField { get; private set; }
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual double searchFieldHeight { get; private set; } = default!;
    public virtual global::System.Action? onSearchFieldTap { get; private set; }

    internal _ActiveSearchableBottom__nav_bar(global::Doroti.Framework.Animation.AnimationController animationController, global::Doroti.Framework.Widgets.Widget? searchField, global::Doroti.Framework.Animation.Animation<double> animation, double searchFieldHeight, global::System.Action? onSearchFieldTap)
    {
        this.animationController = animationController;
        this.searchField = searchField;
        this.animation = animation;
        this.searchFieldHeight = searchFieldHeight;
        this.onSearchFieldTap = onSearchFieldTap;
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.Padding(padding: EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Framework.Widgets.Row(spacing: 12.0, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.SizedBox(height: searchFieldHeight, child: searchField ?? SizedBox.CreateShrink()))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: animation, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0).animate(animationController), child: new _CancelButton__nav_bar(onPressed: onSearchFieldTap)), builder: (context, child) => {
return new global::Doroti.Framework.Widgets.SizedBox(width: animationController.value * Nav_barLibrary._kSearchFieldCancelButtonWidth, child: child);
throw new InvalidOperationException("Dart closure completed without a value.");
})) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _TransitionableNavigationBar__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar componentsKeys { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle backButtonTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle titleTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? largeTitleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.Border? border { get; private set; }
    public virtual bool hasUserMiddle { get; private set; } = default!;
    public virtual bool largeExpanded { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _TransitionableNavigationBar__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar componentsKeys, Color? backgroundColor, global::Doroti.Framework.Painting.TextStyle backButtonTextStyle, global::Doroti.Framework.Painting.TextStyle titleTextStyle, global::Doroti.Framework.Painting.TextStyle? largeTitleTextStyle, global::Doroti.Framework.Painting.Border? border, bool hasUserMiddle, bool largeExpanded, bool searchable, bool automaticBackgroundVisibility, global::Doroti.Framework.Widgets.Widget child) : base(key: componentsKeys.navBarBoxKey)
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

    public virtual global::Doroti.Framework.Rendering.RenderBox renderBox
    {
        get
        {
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)componentsKeys.navBarBoxKey.currentContext!.findRenderObject()!)!;
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
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var inHero = false;
                context.visitAncestorElements((ancestor) =>
                {
                    if (ancestor is global::Doroti.Framework.Widgets.ComponentElement)
                    {
                        DartRuntimePrimitives.Assert(() => !Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Widgets.ComponentElement)ancestor).widget), typeof(_NavigationBarTransition__nav_bar)), () => (object?)"_TransitionableNavigationBar should never re-appear inside " + "_NavigationBarTransition. Keyed _TransitionableNavigationBar should " + "only serve as anchor points in routes rather than appearing inside " + "Hero flights themselves.");
                        if (Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Widgets.ComponentElement)ancestor).widget), typeof(global::Doroti.Framework.Widgets.Hero)))
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

internal class _NavigationBarTransition__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual _TransitionableNavigationBar__nav_bar topNavBar { get; private set; } = default!;
    public virtual _TransitionableNavigationBar__nav_bar bottomNavBar { get; private set; } = default!;
    public virtual global::Doroti.Framework.Animation.Tween<double> heightTween { get; private set; } = default!;

    internal _NavigationBarTransition__nav_bar(global::Doroti.Framework.Animation.Animation<double> animation, _TransitionableNavigationBar__nav_bar topNavBar, _TransitionableNavigationBar__nav_bar bottomNavBar)
    {
        this.animation = animation;
        this.topNavBar = topNavBar;
        this.bottomNavBar = bottomNavBar;
        heightTween = new global::Doroti.Framework.Animation.Tween<double>(begin: bottomNavBar.renderBox.size.height, end: topNavBar.renderBox.size.height);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var componentsTransition = new _NavigationBarComponentsTransition__nav_bar(animation: animation, bottomNavBar: bottomNavBar, topNavBar: topNavBar, directionality: Directionality.of(context));
        var childrenLocal = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection98801 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement98817 = componentsTransition.bottomNavBarBackground; if (__collectionElement98817 is { } __nonNullCollectionElement98817) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement98817)); } var __collectionElement98869 = componentsTransition.bottomBackChevron; if (__collectionElement98869 is { } __nonNullCollectionElement98869) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement98869)); } var __collectionElement98916 = componentsTransition.bottomBackLabel; if (__collectionElement98916 is { } __nonNullCollectionElement98916) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement98916)); } var __collectionElement98961 = componentsTransition.bottomLeading; if (__collectionElement98961 is { } __nonNullCollectionElement98961) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement98961)); } var __collectionElement99004 = componentsTransition.bottomMiddle; if (__collectionElement99004 is { } __nonNullCollectionElement99004) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99004)); } var __collectionElement99046 = componentsTransition.bottomLargeTitle; if (__collectionElement99046 is { } __nonNullCollectionElement99046) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99046)); } var __collectionElement99092 = componentsTransition.bottomTrailing; if (__collectionElement99092 is { } __nonNullCollectionElement99092) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99092)); } var __collectionElement99136 = componentsTransition.bottomNavBarBottom; if (__collectionElement99136 is { } __nonNullCollectionElement99136) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99136)); } var __collectionElement99246 = componentsTransition.topNavBarBackground; if (__collectionElement99246 is { } __nonNullCollectionElement99246) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99246)); } var __collectionElement99295 = componentsTransition.topLeading; if (__collectionElement99295 is { } __nonNullCollectionElement99295) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99295)); } var __collectionElement99335 = componentsTransition.topBackChevron; if (__collectionElement99335 is { } __nonNullCollectionElement99335) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99335)); } var __collectionElement99379 = componentsTransition.topBackLabel; if (__collectionElement99379 is { } __nonNullCollectionElement99379) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99379)); } var __collectionElement99421 = componentsTransition.topMiddle; if (__collectionElement99421 is { } __nonNullCollectionElement99421) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99421)); } var __collectionElement99460 = componentsTransition.topLargeTitle; if (__collectionElement99460 is { } __nonNullCollectionElement99460) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99460)); } var __collectionElement99503 = componentsTransition.topTrailing; if (__collectionElement99503 is { } __nonNullCollectionElement99503) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99503)); } var __collectionElement99544 = componentsTransition.topNavBarBottom; if (__collectionElement99544 is { } __nonNullCollectionElement99544) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99544)); } return __collection98801; }))();
        return MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.SizedBox(height: Math.Max(DartRuntimePrimitives.RequireValue(heightTween.begin), DartRuntimePrimitives.RequireValue(heightTween.end)) + MediaQuery.paddingOf(context).top, width: double.PositiveInfinity, child: new global::Doroti.Framework.Widgets.Stack(children: childrenLocal)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarComponentsTransition__nav_bar
{
    public static global::Doroti.Framework.Animation.Animatable<double> fadeOut = new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0);
    public static global::Doroti.Framework.Animation.Animatable<double> fadeIn = new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0);
    public virtual global::Doroti.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual _NavigationBarStaticComponentsKeys__nav_bar bottomComponents { get; private set; } = default!;
    public virtual _NavigationBarStaticComponentsKeys__nav_bar topComponents { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.RenderBox bottomNavBarBox { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.RenderBox topNavBarBox { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle bottomBackButtonTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle topBackButtonTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle bottomTitleTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle topTitleTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Framework.Painting.TextStyle? bottomLargeTitleTextStyle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? topLargeTitleTextStyle { get; private set; }
    public virtual bool bottomHasUserMiddle { get; private set; } = default!;
    public virtual bool topHasUserMiddle { get; private set; } = default!;
    public virtual bool bottomLargeExpanded { get; private set; } = default!;
    public virtual bool topLargeExpanded { get; private set; } = default!;
    public virtual bool userGestureInProgress { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    public virtual bool bottomAutomaticBackgroundVisibility { get; private set; } = default!;
    public virtual Color? bottomBackgroundColor { get; private set; }
    public virtual Color? topBackgroundColor { get; private set; }
    public virtual global::Doroti.Framework.Painting.Border? bottomBorder { get; private set; }
    public virtual global::Doroti.Framework.Painting.Border? topBorder { get; private set; }
    public virtual Rect transitionBox { get; private set; } = default!;
    public virtual double forwardDirection { get; private set; } = default!;

    internal _NavigationBarComponentsTransition__nav_bar(global::Doroti.Framework.Animation.Animation<double> animation, _TransitionableNavigationBar__nav_bar bottomNavBar, _TransitionableNavigationBar__nav_bar topNavBar, TextDirection directionality)
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

    public virtual global::Doroti.Framework.Rendering.RelativeRect positionInTransitionBox(global::Doroti.Framework.Widgets.GlobalKey<IState> key, global::Doroti.Framework.Rendering.RenderBox from)
    {
        var componentBox = ((global::Doroti.Framework.Rendering.RenderBox?)key.currentContext!.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => componentBox.attached);
        return RelativeRect.CreateFromRect(componentBox.localToGlobal(Offset.zero, ancestor: from) & componentBox.size, transitionBox);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _FixedSizeSlidingTransition__nav_bar slideFromLeadingEdge(global::Doroti.Framework.Widgets.GlobalKey<IState> fromKey, global::Doroti.Framework.Rendering.RenderBox fromNavBarBox, global::Doroti.Framework.Widgets.GlobalKey<IState> toKey, global::Doroti.Framework.Rendering.RenderBox toNavBarBox, global::Doroti.Framework.Animation.Curve curve = default!, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        curve ??= new global::Doroti.Framework.Animation.Interval(0.0, 1.0);
        var fromBox = ((global::Doroti.Framework.Rendering.RenderBox?)fromKey.currentContext!.findRenderObject()!)!;
        var toBox = ((global::Doroti.Framework.Rendering.RenderBox?)toKey.currentContext!.findRenderObject()!)!;
        bool isLTRLocal = forwardDirection > 0L;
        var fromAnchorLocal = new global::Doroti.Ui.Offset(isLTRLocal ? 0 : fromBox.size.width, fromBox.size.height / 2L);
        var toAnchorLocal = new global::Doroti.Ui.Offset(isLTRLocal ? 0 : toBox.size.width, toBox.size.height / 2L);
        global::Doroti.Ui.Offset fromAnchorInFromBox = fromBox.localToGlobal(fromAnchorLocal, ancestor: fromNavBarBox);
        global::Doroti.Ui.Offset toAnchorInToBox = toBox.localToGlobal(toAnchorLocal, ancestor: toNavBarBox);
        global::Doroti.Ui.Offset translation = isLTRLocal ? (toAnchorInToBox - fromAnchorInFromBox) : (new global::Doroti.Ui.Offset(toNavBarBox.size.width - toAnchorInToBox.dx, toAnchorInToBox.dy) - new global::Doroti.Ui.Offset(fromNavBarBox.size.width - fromAnchorInFromBox.dx, fromAnchorInFromBox.dy));
        global::Doroti.Framework.Rendering.RelativeRect fromBoxMargin = positionInTransitionBox(fromKey, from: fromNavBarBox);
        var fromOriginInTransitionBox = new global::Doroti.Ui.Offset(isLTRLocal ? fromBoxMargin.left : fromBoxMargin.right, fromBoxMargin.top);
        var anchorMovementInTransitionBox = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: fromOriginInTransitionBox, end: fromOriginInTransitionBox + translation);
        return new _FixedSizeSlidingTransition__nav_bar(isLTR: isLTRLocal, offsetAnimation: animation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: curve)).drive(anchorMovementInTransitionBox), width: fromNavBarBox.size.width, height: fromBox.size.height, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> fadeInFrom(double t, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        curve ??= Curves.easeIn;
        return animation.drive(fadeIn.chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(t, 1.0, curve: curve))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> fadeOutBy(double t, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        curve ??= Curves.easeOut;
        return animation.drive(fadeOut.chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, t, curve: curve))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> routeAnimation
    {
        get
        {
            DartRuntimePrimitives.Assert(() => animation is global::Doroti.Framework.Animation.CurvedAnimation);
            return ((global::Doroti.Framework.Animation.CurvedAnimation?)animation)!.parent;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomNavBarBackground
    {
        get
        {
            if ((bottomBackgroundColor is null) || bottomLargeExpanded && bottomAutomaticBackgroundVisibility)
            {
                return null;
            }
            global::Doroti.Framework.Animation.Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Curves.fastEaseInToSlowEaseOut : Curves.fastEaseInToSlowEaseOut.flipped;
            global::Doroti.Framework.Animation.Animation<double> pageTransitionAnimation = routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: userGestureInProgress ? Curves.linear : animationCurve));
            global::Doroti.Framework.Rendering.RelativeRect fromLocal = positionInTransitionBox(bottomComponents.navBarBoxKey, from: bottomNavBarBox);
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(end: fromLocal.shift(new global::Doroti.Ui.Offset(forwardDirection * -bottomNavBarBox.size.width, 0.0)), begin: fromLocal);
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: pageTransitionAnimation.drive(positionTween), child: Nav_barLibrary._wrapWithBackground(updateSystemUiOverlay: false, backgroundColor: bottomBackgroundColor!, border: topBorder, child: new global::Doroti.Framework.Widgets.SizedBox(height: bottomNavBarBox.size.height, width: double.PositiveInfinity)));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomLeading
    {
        get
        {
            var bottomLeading = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.leadingKey.currentWidget)!;
            if (bottomLeading is null)
            {
                return null;
            }
            return (global::Doroti.Framework.Widgets.Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(bottomComponents.leadingKey, from: bottomNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.4), child: bottomLeading.child));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomBackChevron
    {
        get
        {
            var bottomBackChevron = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.backChevronKey.currentWidget)!;
            if (bottomBackChevron is null)
            {
                return null;
            }
            return (global::Doroti.Framework.Widgets.Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(bottomComponents.backChevronKey, from: bottomNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: bottomBackButtonTextStyle, child: bottomBackChevron.child)));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomBackLabel
    {
        get
        {
            var bottomBackLabel = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.backLabelKey.currentWidget)!;
            if (bottomBackLabel is null)
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect fromLocal = positionInTransitionBox(bottomComponents.backLabelKey, from: bottomNavBarBox);
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new global::Doroti.Ui.Offset(forwardDirection * (-bottomNavBarBox.size.width / 2.0), 0.0)));
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: animation.drive(positionTween), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.2), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: bottomBackButtonTextStyle, child: bottomBackLabel.child)));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomMiddle
    {
        get
        {
            var bottomMiddle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.middleKey.currentWidget)!;
            var topBackLabel = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.backLabelKey.currentWidget)!;
            var topLeading = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.leadingKey.currentWidget)!;
            if (!bottomHasUserMiddle && bottomLargeExpanded)
            {
                return null;
            }
            if ((bottomMiddle is not null) && (topBackLabel is not null))
            {
                return (global::Doroti.Framework.Widgets.Widget?)slideFromLeadingEdge(fromKey: bottomComponents.middleKey, fromNavBarBox: bottomNavBarBox, toKey: topComponents.backLabelKey, toNavBarBox: topNavBarBox, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(bottomHasUserMiddle ? 0.4 : 0.7), child: new global::Doroti.Framework.Widgets.Align(alignment: AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.DefaultTextStyleTransition(style: animation.drive(new global::Doroti.Framework.Widgets.TextStyleTween(begin: bottomTitleTextStyle, end: topBackButtonTextStyle)), child: bottomMiddle.child))));
            }
            if ((bottomMiddle is not null) && (topLeading is not null))
            {
                return (global::Doroti.Framework.Widgets.Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(bottomComponents.middleKey, from: bottomNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(bottomHasUserMiddle ? 0.4 : 0.7), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: bottomTitleTextStyle, child: bottomMiddle.child)));
            }
            return null;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomLargeTitle
    {
        get
        {
            var bottomLargeTitle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.largeTitleKey.currentWidget)!;
            var topBackLabel = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.backLabelKey.currentWidget)!;
            if ((bottomLargeTitle is null) || !bottomLargeExpanded)
            {
                return null;
            }
            if (topBackLabel is not null)
            {
                return (global::Doroti.Framework.Widgets.Widget?)slideFromLeadingEdge(fromKey: bottomComponents.largeTitleKey, fromNavBarBox: bottomNavBarBox, toKey: topComponents.backLabelKey, toNavBarBox: topNavBarBox, curve: new global::Doroti.Framework.Animation.Interval(0.0, Equals(animation.status, AnimationStatus.forward) ? 0.7 : 1.0), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: new global::Doroti.Framework.Widgets.Align(alignment: AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.DefaultTextStyleTransition(style: animation.drive(new global::Doroti.Framework.Widgets.TextStyleTween(begin: bottomLargeTitleTextStyle, end: topBackButtonTextStyle)), maxLines: 1L, overflow: TextOverflow.ellipsis, child: bottomLargeTitle.child))));
            }
            global::Doroti.Framework.Rendering.RelativeRect fromLocal = positionInTransitionBox(bottomComponents.largeTitleKey, from: bottomNavBarBox);
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new global::Doroti.Ui.Offset(forwardDirection * bottomNavBarBox.size.width / 4.0, 0.0)));
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: animation.drive(positionTween), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.4), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: bottomLargeTitleTextStyle!, child: bottomLargeTitle.child)));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomTrailing
    {
        get
        {
            var bottomTrailing = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.trailingKey.currentWidget)!;
            if (bottomTrailing is null)
            {
                return null;
            }
            return (global::Doroti.Framework.Widgets.Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(bottomComponents.trailingKey, from: bottomNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: bottomTrailing.child));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomNavBarBottom
    {
        get
        {
            var bottomNavBarBottom = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.navBarBottomKey.currentWidget)!;
            if (bottomNavBarBottom is null)
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect fromLocal = positionInTransitionBox(bottomComponents.navBarBottomKey, from: bottomNavBarBox);
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new global::Doroti.Ui.Offset(forwardDirection * -bottomNavBarBox.size.width, 0.0)));
            global::Doroti.Framework.Widgets.Widget childLocal = bottomNavBarBottom.child;
            global::Doroti.Framework.Animation.Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Nav_barLibrary._kBottomNavBarHeaderTransitionCurve : Nav_barLibrary._kBottomNavBarHeaderTransitionCurve.flipped;
            if (!searchable)
            {
                childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.8, curve: animationCurve), child: childLocal));
            }
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: userGestureInProgress ? routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: Curves.linear)).drive(positionTween) : animation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: animationCurve)).drive(positionTween), child: new global::Doroti.Framework.Widgets.ClipRect(child: childLocal));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topNavBarBackground
    {
        get
        {
            if (topBackgroundColor is null)
            {
                return null;
            }
            global::Doroti.Framework.Animation.Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Curves.fastEaseInToSlowEaseOut : Curves.fastEaseInToSlowEaseOut.flipped;
            global::Doroti.Framework.Animation.Animation<double> pageTransitionAnimation = routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: userGestureInProgress ? Curves.linear : animationCurve));
            global::Doroti.Framework.Rendering.RelativeRect to = positionInTransitionBox(topComponents.navBarBoxKey, from: topNavBarBox);
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: to.shift(new global::Doroti.Ui.Offset(forwardDirection * topNavBarBox.size.width, 0.0)), end: to);
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: pageTransitionAnimation.drive(positionTween), child: Nav_barLibrary._wrapWithBackground(updateSystemUiOverlay: false, backgroundColor: topBackgroundColor!, border: topBorder, child: new global::Doroti.Framework.Widgets.SizedBox(height: topNavBarBox.size.height, width: double.PositiveInfinity)));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topLeading
    {
        get
        {
            var topLeading = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.leadingKey.currentWidget)!;
            if (topLeading is null)
            {
                return null;
            }
            return (global::Doroti.Framework.Widgets.Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(topComponents.leadingKey, from: topNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.6), child: topLeading.child));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topBackChevron
    {
        get
        {
            var topBackChevron = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.backChevronKey.currentWidget)!;
            var bottomBackChevron = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.backChevronKey.currentWidget)!;
            if (topBackChevron is null)
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect to = positionInTransitionBox(topComponents.backChevronKey, from: topNavBarBox);
            var fromLocal = to;
            global::Doroti.Framework.Widgets.Widget childLocal = topBackChevron.child;
            global::Doroti.Framework.Animation.Curve forwardScaleCurve = new global::Doroti.Framework.Animation.Interval(0.0, 0.2);
            global::Doroti.Framework.Animation.Curve backwardScaleCurve = new global::Doroti.Framework.Animation.Interval(0.8, 1.0);
            global::Doroti.Framework.Animation.Curve forwardPositionCurve = new global::Doroti.Framework.Animation.Interval(0.0, 0.5);
            global::Doroti.Framework.Animation.Curve backwardPositionCurve = new global::Doroti.Framework.Animation.Interval(0.5, 1.0);
            global::Doroti.Framework.Animation.Curve effectiveScaleCurve = default!;
            global::Doroti.Framework.Animation.Curve effectivePositionCurve = default!;
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
                var topBackChevronBox = ((global::Doroti.Framework.Rendering.RenderBox?)topComponents.backChevronKey.currentContext!.findRenderObject()!)!;
                fromLocal = to.shift(new global::Doroti.Ui.Offset(forwardDirection * topBackChevronBox.size.width * 2.0, 0.0));
                childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ScaleTransition(scale: routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: effectiveScaleCurve)), child: childLocal));
            }
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: fromLocal, end: to);
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: effectivePositionCurve)).drive(positionTween), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(((bottomBackChevron is null) && (!Equals(animation.status, AnimationStatus.forward))) ? 0.9 : 0.4, 1.0))), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: topBackButtonTextStyle, child: childLocal)));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topBackLabel
    {
        get
        {
            var bottomMiddle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.middleKey.currentWidget)!;
            var bottomLargeTitle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)bottomComponents.largeTitleKey.currentWidget)!;
            var topBackLabel = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.backLabelKey.currentWidget)!;
            if (topBackLabel is null)
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RenderAnimatedOpacity? topBackLabelOpacity = topComponents.backLabelKey.currentContext?.findAncestorRenderObjectOfType<global::Doroti.Framework.Rendering.RenderAnimatedOpacity>();
            global::Doroti.Framework.Animation.Animation<double>? midClickOpacity = default!;
            if ((topBackLabelOpacity is not null) && (topBackLabelOpacity.opacity.value < 1.0))
            {
                midClickOpacity = animation.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: topBackLabelOpacity.opacity.value));
            }
            if ((bottomLargeTitle is not null) && bottomLargeExpanded)
            {
                return (global::Doroti.Framework.Widgets.Widget?)slideFromLeadingEdge(fromKey: bottomComponents.largeTitleKey, fromNavBarBox: bottomNavBarBox, toKey: topComponents.backLabelKey, toNavBarBox: topNavBarBox, curve: new global::Doroti.Framework.Animation.Interval(0.0, Equals(animation.status, AnimationStatus.forward) ? 0.7 : 1.0), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: midClickOpacity ?? fadeInFrom(0.4), child: new global::Doroti.Framework.Widgets.DefaultTextStyleTransition(style: animation.drive(new global::Doroti.Framework.Widgets.TextStyleTween(begin: bottomLargeTitleTextStyle, end: topBackButtonTextStyle)), maxLines: 1L, overflow: TextOverflow.ellipsis, child: topBackLabel.child)));
            }
            if (bottomMiddle is not null)
            {
                return (global::Doroti.Framework.Widgets.Widget?)slideFromLeadingEdge(fromKey: bottomComponents.middleKey, fromNavBarBox: bottomNavBarBox, toKey: topComponents.backLabelKey, toNavBarBox: topNavBarBox, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: midClickOpacity ?? fadeInFrom(0.3), child: new global::Doroti.Framework.Widgets.DefaultTextStyleTransition(style: animation.drive(new global::Doroti.Framework.Widgets.TextStyleTween(begin: bottomTitleTextStyle, end: topBackButtonTextStyle)), child: topBackLabel.child)));
            }
            return null;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topMiddle
    {
        get
        {
            var topMiddle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.middleKey.currentWidget)!;
            if (topMiddle is null)
            {
                return null;
            }
            if (!topHasUserMiddle && topLargeExpanded)
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect to = positionInTransitionBox(topComponents.middleKey, from: topNavBarBox);
            var toBox = ((global::Doroti.Framework.Rendering.RenderBox?)topComponents.middleKey.currentContext!.findRenderObject()!)!;
            bool isLTRLocal = forwardDirection > 0L;
            var toAnchorInTransitionBox = new global::Doroti.Ui.Offset(isLTRLocal ? to.left : to.right, to.top);
            var anchorMovementInTransitionBox = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset(topNavBarBox.size.width - (toBox.size.width / 2L), to.top), end: toAnchorInTransitionBox);
            return (global::Doroti.Framework.Widgets.Widget?)new _FixedSizeSlidingTransition__nav_bar(isLTR: isLTRLocal, offsetAnimation: animation.drive(anchorMovementInTransitionBox), width: toBox.size.width, height: toBox.size.height, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.25), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: topTitleTextStyle, child: topMiddle.child)));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topTrailing
    {
        get
        {
            var topTrailing = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.trailingKey.currentWidget)!;
            if (topTrailing is null)
            {
                return null;
            }
            return (global::Doroti.Framework.Widgets.Widget?)Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(topComponents.trailingKey, from: topNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.4), child: topTrailing.child));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topLargeTitle
    {
        get
        {
            var topLargeTitle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.largeTitleKey.currentWidget)!;
            if ((topLargeTitle is null) || !topLargeExpanded)
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect to = positionInTransitionBox(topComponents.largeTitleKey, from: topNavBarBox);
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: to.shift(new global::Doroti.Ui.Offset(forwardDirection * topNavBarBox.size.width, 0.0)), end: to);
            global::Doroti.Framework.Animation.Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Nav_barLibrary._kTopNavBarHeaderTransitionCurve : Nav_barLibrary._kTopNavBarHeaderTransitionCurve.flipped;
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: userGestureInProgress ? routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: Curves.linear)).drive(positionTween) : animation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: animationCurve)).drive(positionTween), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.0, curve: animationCurve), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: topLargeTitleTextStyle!, maxLines: 1L, overflow: TextOverflow.ellipsis, child: topLargeTitle.child)));
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topNavBarBottom
    {
        get
        {
            var topNavBarBottom = ((global::Doroti.Framework.Widgets.KeyedSubtree?)topComponents.navBarBottomKey.currentWidget)!;
            if (topNavBarBottom is null)
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect to = positionInTransitionBox(topComponents.navBarBottomKey, from: topNavBarBox);
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: to.shift(new global::Doroti.Ui.Offset(forwardDirection * topNavBarBox.size.width, 0.0)), end: to);
            global::Doroti.Framework.Widgets.Widget childLocal = topNavBarBottom.child;
            global::Doroti.Framework.Animation.Curve animationCurve = Equals(animation.status, AnimationStatus.forward) ? Nav_barLibrary._kTopNavBarHeaderTransitionCurve : Nav_barLibrary._kTopNavBarHeaderTransitionCurve.flipped;
            if (!searchable)
            {
                childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.0, curve: animationCurve), child: childLocal));
            }
            return (global::Doroti.Framework.Widgets.Widget?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: userGestureInProgress ? routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: Curves.linear)).drive(positionTween) : animation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: animationCurve)).drive(positionTween), child: new global::Doroti.Framework.Widgets.ClipRect(child: childLocal));
        }
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Animation.RectTween _linearTranslateWithLargestRectSizeTween(Rect? begin, Rect? end)
    {
        var largestSize = new global::Doroti.Ui.Size(Math.Max(DartRuntimePrimitives.RequireValue(begin).size.width, DartRuntimePrimitives.RequireValue(end).size.width), Math.Max(DartRuntimePrimitives.RequireValue(begin).size.height, DartRuntimePrimitives.RequireValue(end).size.height));
        return new global::Doroti.Framework.Animation.RectTween(begin: DartRuntimePrimitives.RequireValue(begin).topLeft & largestSize, end: DartRuntimePrimitives.RequireValue(end).topLeft & largestSize);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _navBarHeroLaunchPadBuilder(global::Doroti.Framework.Widgets.BuildContext context, Size heroSize, global::Doroti.Framework.Widgets.Widget child)
    {
        DartRuntimePrimitives.Assert(() => child is _TransitionableNavigationBar__nav_bar);
        return new global::Doroti.Framework.Widgets.Visibility(maintainSize: true, maintainAnimation: true, maintainState: true, visible: false, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _navBarHeroFlightShuttleBuilder(global::Doroti.Framework.Widgets.BuildContext flightContext, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Widgets.HeroFlightDirection flightDirection, global::Doroti.Framework.Widgets.BuildContext fromHeroContext, global::Doroti.Framework.Widgets.BuildContext toHeroContext)
    {
        DartRuntimePrimitives.Assert(() => fromHeroContext.widget is global::Doroti.Framework.Widgets.Hero);
        DartRuntimePrimitives.Assert(() => toHeroContext.widget is global::Doroti.Framework.Widgets.Hero);
        var fromHeroWidget = ((global::Doroti.Framework.Widgets.Hero?)fromHeroContext.widget)!;
        var toHeroWidget = ((global::Doroti.Framework.Widgets.Hero?)toHeroContext.widget)!;
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
