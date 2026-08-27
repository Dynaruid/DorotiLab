// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/nav_bar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

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
    internal static global::Doroti.Framework.Animation.Curve _kNavBarSearchCurve = ((global::Doroti.Framework.Animation.Curve)(object?)global::Doroti.Framework.Animation.Curves.easeInOut);
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
    internal static global::Doroti.Framework.Painting.Border _kDefaultNavBarBorder = new global::Doroti.Framework.Painting.Border(bottom: new global::Doroti.Framework.Painting.BorderSide(color: Nav_barLibrary._kDefaultNavBarBorderColor, width: 0.0));
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Painting.Border _kTransparentNavBarBorder = new global::Doroti.Framework.Painting.Border(bottom: new global::Doroti.Framework.Painting.BorderSide(color: new global::Doroti.Ui.Color(0L), width: 0.0));
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kTopNavBarHeaderTransitionCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.0, 0.45, 0.45, 0.98));
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Animation.Curve _kBottomNavBarHeaderTransitionCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Cubic(0.05, 0.9, 0.9, 0.95));
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

    public override string ToString() => $"Default Hero tag for Cupertino navigation bars with navigator {this.navigator}";
    public override bool Equals(object? other)
    {
        var __other = other as _HeroTag__nav_bar;
        if (__other is null) return false;
        if (DartRuntimePrimitives.Identical(this, __other))
        {
            return true;
        }
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return ((__other is _HeroTag__nav_bar) && (object.Equals(((_HeroTag__nav_bar)((_HeroTag__nav_bar)__other)).navigator, this.navigator)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(Dart_coreLibrary.identityHashCode(this.navigator));
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
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Positioned(top: ((global::Doroti.Framework.Animation.Animation<Offset>)this.offsetAnimation).value.dy, left: (this.isLTR ? ((global::Doroti.Framework.Animation.Animation<Offset>)this.offsetAnimation).value.dx : null), right: (this.isLTR ? null : ((global::Doroti.Framework.Animation.Animation<Offset>)this.offsetAnimation).value.dx), width: this.width, height: this.height, child: this.child));
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
            bool isDark = (backgroundColor.computeLuminance() < 0.179);
            global::Doroti.Ui.Brightness newBrightness = (brightness ?? ((isDark ? Brightness.dark : Brightness.light)));
            global::Doroti.Framework.Services.SystemUiOverlayStyle overlayStyle = (newBrightness switch { Brightness.dark => global::Doroti.Framework.Services.SystemUiOverlayStyle.light, Brightness.light => global::Doroti.Framework.Services.SystemUiOverlayStyle.dark, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            result = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnnotatedRegion<global::Doroti.Framework.Services.SystemUiOverlayStyle>(value: new global::Doroti.Framework.Services.SystemUiOverlayStyle(statusBarColor: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)overlayStyle).statusBarColor, statusBarBrightness: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)overlayStyle).statusBarBrightness, statusBarIconBrightness: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)overlayStyle).statusBarIconBrightness, systemStatusBarContrastEnforced: ((global::Doroti.Framework.Services.SystemUiOverlayStyle)overlayStyle).systemStatusBarContrastEnforced), child: result));
        }
        var childWithBackground = new global::Doroti.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Framework.Painting.BoxDecoration(border: border, color: backgroundColor), child: result);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.BackdropFilter(enabled: ((backgroundColor.alpha != 255L) && enableBackgroundFilterBlur), filter: new global::Doroti.Ui.ImageFilter(sigmaX: 10.0, sigmaY: 10.0), child: childWithBackground)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static double _dampScaleFactor(double scaledFontSize, double unscaledFontSize, double dampingRatio)
    {
        double scaleFactor = (scaledFontSize / unscaledFontSize);
        return ((scaleFactor < 1.0) ? Math.Max(Nav_barLibrary._kMinScaleFactor, scaleFactor) : (1.0 + ((((scaleFactor - 1.0)) / dampingRatio))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static bool _isTransitionable(global::Doroti.Framework.Widgets.BuildContext context)
    {
        dynamic route = global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(context);
        return (((route is PageRoute<object>) && !((bool)((dynamic)route).fullscreenDialog)) && !CupertinoSheetRoute<object>.hasParentSheet(context));
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
        this.largeTitle = null;
        System.Diagnostics.Debug.Assert((!transitionBetweenRoutes || DartRuntimePrimitives.Identical(__heroTag, Nav_barLibrary._defaultHeroTag)));
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
        global::Doroti.Ui.Color backgroundColorLocal = ((global::Doroti.Ui.Color)(object?)(CupertinoDynamicColor.maybeResolve(this.backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor));
        return (backgroundColorLocal.alpha == 255L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size preferredSize
    {
        get
        {
            double bottomHeight = (this.bottom?.preferredSize.height ?? 0.0);
            double effectiveLargeHeight = ((this.largeTitle is not null) ? Nav_barLibrary._kNavBarLargeTitleHeightExtension : 0.0);
            return new global::Doroti.Ui.Size(((Nav_barLibrary._kNavBarPersistentHeight + bottomHeight) + effectiveLargeHeight));
            return default!;
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
        this._scrollNotificationObserver?.removeListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
        _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(this.context);
        this._scrollNotificationObserver?.addListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
    }

    public override void dispose()
    {
        if ((this._scrollNotificationObserver is not null))
        {
            this._scrollNotificationObserver!.removeListener((global::System.Action<global::Doroti.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
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
        if (((notification is global::Doroti.Framework.Widgets.ScrollUpdateNotification) && (((global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification).depth == 0L)))
        {
            global::Doroti.Framework.Widgets.ScrollUpdateNotification notification__as27250 = (global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification;
            global::Doroti.Framework.Widgets.ScrollMetrics metricsLocal = ((global::Doroti.Framework.Widgets.ScrollUpdateNotification)notification__as27250).metrics;
            double oldScrollAnimationValue = this._scrollAnimationValue;
            var scrollExtent = 0.0;
            switch (((global::Doroti.Framework.Widgets.ScrollMetrics)metricsLocal).axisDirection)
            {
                case global::Doroti.Framework.Painting.AxisDirection.up:
                    {
                        scrollExtent = ((global::Doroti.Framework.Widgets.ScrollMetrics)metricsLocal).extentAfter;
                        break;
                    }
                case global::Doroti.Framework.Painting.AxisDirection.down:
                    {
                        scrollExtent = ((global::Doroti.Framework.Widgets.ScrollMetrics)metricsLocal).extentBefore;
                        break;
                    }
                case global::Doroti.Framework.Painting.AxisDirection.right:
                case global::Doroti.Framework.Painting.AxisDirection.left:
                    {
                        break;
                    }
            }
            if (((scrollExtent >= 0L) && (scrollExtent < Nav_barLibrary._kNavBarScrollUnderAnimationExtent)))
            {
                setState(((global::System.Action)(() =>
                {
                    _scrollAnimationValue = Dart_uiLibrary.clampDouble((scrollExtent / Nav_barLibrary._kNavBarScrollUnderAnimationExtent), 0, 1);
                })));
            }
            else
            {
                if (((scrollExtent > Nav_barLibrary._kNavBarScrollUnderAnimationExtent) && (oldScrollAnimationValue != 1.0)))
                {
                    setState(((global::System.Action)(() =>
                    {
                        _scrollAnimationValue = 1.0;
                    })));
                }
                else
                {
                    if (((scrollExtent <= 0L) && (oldScrollAnimationValue != 0.0)))
                    {
                        setState(((global::System.Action)(() =>
                        {
                            _scrollAnimationValue = 0.0;
                        })));
                    }
                }
            }
        }
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ((((CupertinoNavigationBar)this.widget).middle is null) || (((CupertinoNavigationBar)this.widget).largeTitle is null)));
        global::Doroti.Ui.Color backgroundColorLocal = ((global::Doroti.Ui.Color)(object?)(CupertinoDynamicColor.maybeResolve(((CupertinoNavigationBar)this.widget).backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor));
        global::Doroti.Ui.Color? parentPageScaffoldBackgroundColor = ((global::Doroti.Ui.Color?)(object?)CupertinoPageScaffoldBackgroundColor.maybeOf(context));
        global::Doroti.Framework.Painting.Border? initialBorder = ((((CupertinoNavigationBar)this.widget).automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? Nav_barLibrary._kTransparentNavBarBorder : ((CupertinoNavigationBar)this.widget).border);
        global::Doroti.Framework.Painting.Border? effectiveBorder = ((((CupertinoNavigationBar)this.widget).border is null) ? null : Border.lerp(initialBorder, ((CupertinoNavigationBar)this.widget).border, this._scrollAnimationValue));
        global::Doroti.Ui.Color effectiveBackgroundColor = ((global::Doroti.Ui.Color)(object?)((((CupertinoNavigationBar)this.widget).automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? (Dart_uiLibrary.Color.lerp(parentPageScaffoldBackgroundColor, backgroundColorLocal, this._scrollAnimationValue) ?? backgroundColorLocal) : backgroundColorLocal));
        double bottomHeight = (((CupertinoNavigationBar)this.widget).bottom?.preferredSize.height ?? 0.0);
        double persistentHeight = ((Nav_barLibrary._kNavBarPersistentHeight + bottomHeight) + MediaQuery.paddingOf(context).top);
        double largeHeight = (persistentHeight + Nav_barLibrary._kNavBarLargeTitleHeightExtension);
        var componentsLocal = new _NavigationBarStaticComponents__nav_bar(keys: this.keys, route: global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(context), userLeading: ((CupertinoNavigationBar)this.widget).leading, automaticallyImplyLeading: ((CupertinoNavigationBar)this.widget).automaticallyImplyLeading, automaticallyImplyTitle: ((CupertinoNavigationBar)this.widget).automaticallyImplyMiddle, previousPageTitle: ((CupertinoNavigationBar)this.widget).previousPageTitle, userMiddle: ((CupertinoNavigationBar)this.widget).middle, userTrailing: ((CupertinoNavigationBar)this.widget).trailing, padding: ((CupertinoNavigationBar)this.widget).padding, userLargeTitle: ((CupertinoNavigationBar)this.widget).largeTitle, userBottom: DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(((CupertinoNavigationBar)this.widget).bottom), large: (((CupertinoNavigationBar)this.widget).largeTitle is not null), staticBar: true, context: context);
        global::Doroti.Framework.Widgets.Widget navBar = ((global::Doroti.Framework.Widgets.Widget)(object?)new _PersistentNavigationBar__nav_bar(components: componentsLocal, padding: ((CupertinoNavigationBar)this.widget).padding, middleVisible: (((CupertinoNavigationBar)this.widget).largeTitle is null)));
        if ((((CupertinoNavigationBar)this.widget).largeTitle is not null))
        {
            navBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: largeHeight), child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection31165 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(navBar)); __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Framework.Widgets.Semantics(header: true, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: new _LargeTitle__nav_bar(height: Nav_barLibrary._kNavBarLargeTitleHeightExtension, child: ((_NavigationBarStaticComponents__nav_bar)componentsLocal).largeTitle))))))); if ((((CupertinoNavigationBar)this.widget).bottom is not null)) { __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: bottomHeight, child: ((_NavigationBarStaticComponents__nav_bar)componentsLocal).navBarBottom))); } return __collection31165; }))())));
        }
        else
        {
            navBar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(maxHeight: persistentHeight), child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection32281 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection32281.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(navBar)); if ((((CupertinoNavigationBar)this.widget).bottom is not null)) { __collection32281.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: bottomHeight, child: ((_NavigationBarStaticComponents__nav_bar)componentsLocal).navBarBottom))); } return __collection32281; }))())));
        }
        navBar = Nav_barLibrary._wrapWithBackground(border: effectiveBorder, backgroundColor: effectiveBackgroundColor, brightness: ((CupertinoNavigationBar)this.widget).brightness, enableBackgroundFilterBlur: ((CupertinoNavigationBar)this.widget).enableBackgroundFilterBlur, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.textStyle, child: navBar));
        if ((!((CupertinoNavigationBar)this.widget).transitionBetweenRoutes || !Nav_barLibrary._isTransitionable(context)))
        {
            return navBar;
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)((context) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Hero(tag: ((object.Equals(((CupertinoNavigationBar)this.widget).heroTag, Nav_barLibrary._defaultHeroTag)) ? new _HeroTag__nav_bar(Navigator.of(context)) : ((CupertinoNavigationBar)this.widget).heroTag), createRectTween: (global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.RectTween>)Nav_barLibrary._linearTranslateWithLargestRectSizeTween, placeholderBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, Size, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>)Nav_barLibrary._navBarHeroLaunchPadBuilder, flightShuttleBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.HeroFlightDirection, global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)Nav_barLibrary._navBarHeroFlightShuttleBuilder, transitionOnUserGestures: true, child: new _TransitionableNavigationBar__nav_bar(componentsKeys: this.keys, backgroundColor: effectiveBackgroundColor, backButtonTextStyle: CupertinoTheme.of(context).textTheme.navActionTextStyle, titleTextStyle: CupertinoTheme.of(context).textTheme.navTitleTextStyle, largeTitleTextStyle: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, border: effectiveBorder, hasUserMiddle: (((CupertinoNavigationBar)this.widget).middle is not null), largeExpanded: (((CupertinoNavigationBar)this.widget).largeTitle is not null), searchable: false, automaticBackgroundVisibility: ((CupertinoNavigationBar)this.widget).automaticBackgroundVisibility, child: navBar)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
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
        this.onSearchableBottomTap = null;
        this.searchField = null;
        this._searchable = false;
        System.Diagnostics.Debug.Assert((automaticallyImplyTitle || (largeTitle is not null)));
        System.Diagnostics.Debug.Assert(((bottomMode is null) || (bottom is not null)));
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

    public virtual bool opaque => DartRuntimePrimitives.ConvertValue<bool>((this.backgroundColor?.alpha == 255L));
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
        _searchAnimation = new global::Doroti.Framework.Animation.CurvedAnimation(parent: this._animationController, curve: Nav_barLibrary._kNavBarSearchCurve);
    }

    public override void didUpdateWidget(CupertinoSliverNavigationBar oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        if ((!object.Equals(((CupertinoSliverNavigationBar)this.widget).middle, ((CupertinoSliverNavigationBar)oldWidget).middle)))
        {
            _updateEffectiveMiddle();
        }
    }

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        isPortrait = (object.Equals(MediaQuery.orientationOf(this.context), global::Doroti.Framework.Widgets.Orientation.portrait));
        _updateEffectiveMiddle();
        _computeScaledHeights();
        _setupSearchableAnimation();
        this._scrollableState?.position.isScrollingNotifier.removeListener(() => this._handleScrollChange());
        _scrollableState = Scrollable.maybeOf(this.context);
        this._scrollableState?.position.isScrollingNotifier.addListener(() => this._handleScrollChange());
    }

    public override void dispose()
    {
        if ((this._scrollableState?.position is not null))
        {
            this._scrollableState?.position.isScrollingNotifier.removeListener(() => this._handleScrollChange());
        }
        this._searchAnimation.dispose();
        this._animationController.dispose();
        DartRuntimePrimitives.Assert(() =>
            {
                if ((this._tickers is not null))
                {
                    foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
                    {
                        if (((global::Doroti.Framework.Scheduler.Ticker)ticker).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker.describeForError("The offending ticker was") }));
                        }
                    }
                }
                return true;
            });
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        _tickerModeNotifier = null;
        base.dispose();
    }

    internal virtual double _bottomHeight
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (!((CupertinoSliverNavigationBar)this.widget)._searchable || (((CupertinoSliverNavigationBar)this.widget).bottom is null)));
            if (((CupertinoSliverNavigationBar)this.widget)._searchable)
            {
                return (this.scaledSearchFieldHeight + Nav_barLibrary._kNavBarBottomPadding);
            }
            else
            {
                if ((((CupertinoSliverNavigationBar)this.widget).bottom is not null))
                {
                    return ((CupertinoSliverNavigationBar)this.widget).bottom!.preferredSize.height;
                }
            }
            return 0.0;
            return default!;
        }
    }
    internal virtual void _updateEffectiveMiddle()
    {
        effectiveMiddle = (((CupertinoSliverNavigationBar)this.widget).middle ?? ((this.isPortrait ? null : ((CupertinoSliverNavigationBar)this.widget).largeTitle)));
    }

    internal virtual void _computeScaledHeights()
    {
        global::Doroti.Framework.Painting.TextScaler textScaler = ((global::Doroti.Framework.Painting.TextScaler)(object?)MediaQuery.textScalerOf(this.context));
        scaledSearchFieldHeight = (Nav_barLibrary._kSearchFieldHeight * Nav_barLibrary._dampScaleFactor(textScaler.scale(Nav_barLibrary._kSearchFieldHeight), Nav_barLibrary._kSearchFieldHeight, Nav_barLibrary._kMaxScaleFactor));
        scaledLargeTitleHeight = (this.isPortrait ? (Nav_barLibrary._kNavBarLargeTitleHeightExtension * Nav_barLibrary._dampScaleFactor(textScaler.scale(Nav_barLibrary._kNavBarLargeTitleHeightExtension), Nav_barLibrary._kNavBarLargeTitleHeightExtension, Nav_barLibrary._kLargeTitleScaleDampingRatio)) : 0.0);
    }

    internal virtual void _setupSearchableAnimation()
    {
        var persistentHeightTween = new global::Doroti.Framework.Animation.Tween<double>(begin: Nav_barLibrary._kNavBarPersistentHeight, end: 0.0);
        persistentHeightAnimation = ((Func<global::Doroti.Framework.Animation.Animation<double>>)(() =>
{
    var __cascade = persistentHeightTween.animate(this._animationController);
    __cascade.addStatusListener((AnimationStatusListener)this._handleSearchFieldStatusChanged);
    return __cascade;
}))();
        var largeTitleHeightTween = new global::Doroti.Framework.Animation.Tween<double>(begin: this.scaledLargeTitleHeight, end: 0.0);
        largeTitleHeightAnimation = largeTitleHeightTween.animate(this._animationController);
    }

    internal virtual void _handleScrollChange()
    {
        global::Doroti.Framework.Widgets.ScrollPosition? positionLocal = this._scrollableState?.position;
        if ((((positionLocal is null) || !((global::Doroti.Framework.Widgets.ScrollPosition)positionLocal).hasPixels) || (((global::Doroti.Framework.Widgets.ScrollPosition)positionLocal).pixels <= 0.0)))
        {
            return;
        }
        double? target = default!;
        double bottomScrollOffset = ((object.Equals(((CupertinoSliverNavigationBar)this.widget).bottomMode, NavigationBarBottomMode.always)) ? 0.0 : this._bottomHeight);
        bool canScrollBottom = (((((CupertinoSliverNavigationBar)this.widget)._searchable || (((CupertinoSliverNavigationBar)this.widget).bottom is not null))) && (bottomScrollOffset > 0.0));
        if ((canScrollBottom && (((global::Doroti.Framework.Widgets.ScrollPosition)positionLocal).pixels < bottomScrollOffset)))
        {
            target = ((((global::Doroti.Framework.Widgets.ScrollPosition)positionLocal).pixels > (bottomScrollOffset / 2L)) ? bottomScrollOffset : 0.0);
        }
        else
        {
            if (((((global::Doroti.Framework.Widgets.ScrollPosition)positionLocal).pixels > bottomScrollOffset) && (((global::Doroti.Framework.Widgets.ScrollPosition)positionLocal).pixels < (bottomScrollOffset + this.scaledLargeTitleHeight))))
            {
                target = ((((global::Doroti.Framework.Widgets.ScrollPosition)positionLocal).pixels > (bottomScrollOffset + ((this.scaledLargeTitleHeight / 2L)))) ? (bottomScrollOffset + this.scaledLargeTitleHeight) : bottomScrollOffset);
            }
        }
        if (((target is not null) && (target <= ((global::Doroti.Framework.Widgets.ScrollPosition)positionLocal).maxScrollExtent)))
        {
            double target__50844__value51736 = DartRuntimePrimitives.RequireValue(target);
            DartRuntimePrimitives.Ignore(positionLocal.animateTo(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(target__50844__value51736)), duration: Duration.Create(milliseconds: 300L), curve: global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut));
        }
    }

    internal virtual void _handleSearchFieldStatusChanged(global::Doroti.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() =>
        {
            switch (status)
            {
                case global::Doroti.Framework.Animation.AnimationStatus.forward:
                    {
                        searchIsActive = true;
                        break;
                    }
                case global::Doroti.Framework.Animation.AnimationStatus.reverse:
                    {
                        searchIsActive = false;
                        break;
                    }
                case global::Doroti.Framework.Animation.AnimationStatus.completed:
                case global::Doroti.Framework.Animation.AnimationStatus.dismissed:
                    break;
            }
        })));
    }

    internal virtual void _onSearchFieldTap()
    {
        if ((((CupertinoSliverNavigationBar)this.widget).onSearchableBottomTap is not null))
        {
            ((CupertinoSliverNavigationBar)this.widget).onSearchableBottomTap!(!this.searchIsActive);
        }
        this._animationController.toggle();
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var componentsLocal = new _NavigationBarStaticComponents__nav_bar(keys: this.keys, route: global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(context), userLeading: ((((CupertinoSliverNavigationBar)this.widget).leading is not null) ? new global::Doroti.Framework.Widgets.Visibility(visible: !this.searchIsActive, child: ((CupertinoSliverNavigationBar)this.widget).leading!) : null), automaticallyImplyLeading: ((CupertinoSliverNavigationBar)this.widget).automaticallyImplyLeading, automaticallyImplyTitle: ((CupertinoSliverNavigationBar)this.widget).automaticallyImplyTitle, previousPageTitle: ((CupertinoSliverNavigationBar)this.widget).previousPageTitle, userMiddle: (((global::Doroti.Framework.Animation.AnimationController)this._animationController).isAnimating ? new global::Doroti.Framework.Widgets.Text("") : this.effectiveMiddle), userTrailing: ((((CupertinoSliverNavigationBar)this.widget).trailing is not null) ? new global::Doroti.Framework.Widgets.Visibility(visible: !this.searchIsActive, child: ((CupertinoSliverNavigationBar)this.widget).trailing!) : null), userLargeTitle: ((CupertinoSliverNavigationBar)this.widget).largeTitle, userBottom: (((((CupertinoSliverNavigationBar)this.widget)._searchable ? (this.searchIsActive ? new _ActiveSearchableBottom__nav_bar(animationController: this._animationController, animation: this.persistentHeightAnimation, searchField: ((CupertinoSliverNavigationBar)this.widget).searchField, searchFieldHeight: this.scaledSearchFieldHeight, onSearchFieldTap: () => this._onSearchFieldTap()) : new _InactiveSearchableBottom__nav_bar(animationController: this._animationController, animation: this.persistentHeightAnimation, searchField: ((CupertinoSliverNavigationBar)this.widget).searchField, searchFieldHeight: this.scaledSearchFieldHeight, onSearchFieldTap: () => this._onSearchFieldTap())) : (global::Doroti.Framework.Widgets.Widget?)(object?)((CupertinoSliverNavigationBar)this.widget).bottom)) ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink()), padding: ((CupertinoSliverNavigationBar)this.widget).padding, large: this.isPortrait, staticBar: false, context: context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this._searchAnimation, builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SliverPersistentHeader(pinned: true, @delegate: new _LargeTitleNavigationBarSliverDelegate__nav_bar(keys: this.keys, components: componentsLocal, userMiddle: this.effectiveMiddle, backgroundColor: (CupertinoDynamicColor.maybeResolve(((CupertinoSliverNavigationBar)this.widget).backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor), automaticBackgroundVisibility: ((CupertinoSliverNavigationBar)this.widget).automaticBackgroundVisibility, brightness: ((CupertinoSliverNavigationBar)this.widget).brightness, border: ((CupertinoSliverNavigationBar)this.widget).border, padding: ((CupertinoSliverNavigationBar)this.widget).padding, actionsForegroundColor: CupertinoTheme.of(context).primaryColor, transitionBetweenRoutes: ((CupertinoSliverNavigationBar)this.widget).transitionBetweenRoutes, heroTag: ((CupertinoSliverNavigationBar)this.widget).heroTag, persistentHeight: (((global::Doroti.Framework.Animation.Animation<double>)this.persistentHeightAnimation).value + MediaQuery.paddingOf(context).top), largeTitleHeight: ((global::Doroti.Framework.Animation.Animation<double>)this.largeTitleHeightAnimation).value, alwaysShowMiddle: (((CupertinoSliverNavigationBar)this.widget).alwaysShowMiddle && (this.effectiveMiddle is not null)), stretchConfiguration: ((((CupertinoSliverNavigationBar)this.widget).stretch && !this.searchIsActive) ? new global::Doroti.Framework.Rendering.OverScrollHeaderStretchConfiguration() : null), enableBackgroundFilterBlur: ((CupertinoSliverNavigationBar)this.widget).enableBackgroundFilterBlur, bottomMode: (this.searchIsActive ? NavigationBarBottomMode.always : (((CupertinoSliverNavigationBar)this.widget).bottomMode ?? NavigationBarBottomMode.automatic)), bottomHeight: this._bottomHeight, controller: this._animationController, searchable: ((CupertinoSliverNavigationBar)this.widget)._searchable)));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Framework.Scheduler.Ticker>();
        TickerModeData values = this._tickerModeNotifier!.value;
        var result = ((Func<global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{
    var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
    __cascade.muted = !((TickerModeData)values).enabled;
    __cascade.forceFrames = ((TickerModeData)values).forceFrames;
    return __cascade;
}))();
        this._tickers!.Add(result);
        return ((global::Doroti.Framework.Scheduler.Ticker)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Framework.Widgets._WidgetTicker__ticker_provider ticker)
    {
        DartRuntimePrimitives.Assert(() => (this._tickers is not null));
        DartRuntimePrimitives.Assert(() => this._tickers!.Contains(ticker));
        this._tickers!.Remove(ticker);
    }

    public override void activate()
    {
        base.activate();
        _updateTickerModeNotifier();
        _updateTickers();
    }

    public virtual void _updateTickers()
    {
        if ((this._tickers is not null))
        {
            TickerModeData values = this._tickerModeNotifier!.value;
            bool mutedLocal = !((TickerModeData)values).enabled;
            foreach (global::Doroti.Framework.Scheduler.Ticker ticker in this._tickers!)
            {
                ticker.muted = mutedLocal;
                ticker.forceFrames = ((TickerModeData)values).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Framework.Foundation.ValueListenable<TickerModeData> newNotifier = ((global::Doroti.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier;
    }

    public override void debugFillProperties(global::Doroti.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
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
        this.__field_stretchConfiguration = stretchConfiguration;
        this.enableBackgroundFilterBlur = enableBackgroundFilterBlur;
        this.bottomMode = bottomMode;
        this.bottomHeight = bottomHeight;
        this.controller = controller;
        this.searchable = searchable;
    }

    public override double minExtent => DartRuntimePrimitives.ConvertValue<double>((this.persistentHeight + (((object.Equals(this.bottomMode, NavigationBarBottomMode.always)) ? this.bottomHeight : 0.0))));
    public override double maxExtent => DartRuntimePrimitives.ConvertValue<double>(((this.persistentHeight + this.largeTitleHeight) + this.bottomHeight));
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context, double shrinkOffset, bool overlapsContent)
    {
        double largeTitleThreshold = ((this.maxExtent - this.minExtent) - Nav_barLibrary._kNavBarShowLargeTitleThreshold);
        bool showLargeTitle = (shrinkOffset < largeTitleThreshold);
        double bottomShrinkFactor = Dart_uiLibrary.clampDouble((shrinkOffset / this.bottomHeight), 0, 1);
        double shrinkAnimationValue = Dart_uiLibrary.clampDouble(((((shrinkOffset - largeTitleThreshold) - Nav_barLibrary._kNavBarScrollUnderAnimationExtent)) / Nav_barLibrary._kNavBarScrollUnderAnimationExtent), 0, 1);
        var persistentNavigationBar = new _PersistentNavigationBar__nav_bar(components: this.components, padding: this.padding, middleVisible: (this.alwaysShowMiddle ? null : !showLargeTitle));
        global::Doroti.Ui.Color? parentPageScaffoldBackgroundColor = ((global::Doroti.Ui.Color?)(object?)CupertinoPageScaffoldBackgroundColor.maybeOf(context));
        global::Doroti.Framework.Painting.Border? initialBorder = ((this.automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? Nav_barLibrary._kTransparentNavBarBorder : this.border);
        global::Doroti.Framework.Painting.Border? effectiveBorder = ((this.border is null) ? null : Border.lerp(initialBorder, this.border, shrinkAnimationValue));
        global::Doroti.Ui.Color effectiveBackgroundColor = ((global::Doroti.Ui.Color)(object?)((this.automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor is not null)) ? (Dart_uiLibrary.Color.lerp(parentPageScaffoldBackgroundColor, this.backgroundColor, shrinkAnimationValue) ?? this.backgroundColor) : this.backgroundColor));
        global::Doroti.Framework.Widgets.Widget navBar = Nav_barLibrary._wrapWithBackground(border: effectiveBorder, backgroundColor: effectiveBackgroundColor, brightness: this.brightness, enableBackgroundFilterBlur: this.enableBackgroundFilterBlur, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.textStyle, child: new global::Doroti.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection60282 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection60282.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection60368 = new List<global::Doroti.Framework.Widgets.Widget>(); __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(top: this.persistentHeight, left: 0.0, right: 0.0, bottom: ((object.Equals(this.bottomMode, NavigationBarBottomMode.automatic)) ? (this.bottomHeight * ((1.0 - bottomShrinkFactor))) : 0.0), child: new global::Doroti.Framework.Widgets.ClipRect(child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Framework.Widgets.SafeArea(top: false, bottom: false, child: new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: ((showLargeTitle && !this.controller.isForwardOrCompleted) ? 1.0 : 0.0), duration: Nav_barLibrary._kNavBarTitleFadeDuration, child: new global::Doroti.Framework.Widgets.Semantics(header: true, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: new _LargeTitle__nav_bar(height: this.largeTitleHeight, child: ((_NavigationBarStaticComponents__nav_bar)this.components).largeTitle)))))))))); __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(left: 0.0, right: 0.0, top: 0.0, child: persistentNavigationBar))); if ((object.Equals(this.bottomMode, NavigationBarBottomMode.automatic))) { __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Positioned(left: 0.0, right: 0.0, bottom: 0.0, child: new global::Doroti.Framework.Widgets.SizedBox(height: (this.bottomHeight * ((1.0 - bottomShrinkFactor))), child: new global::Doroti.Framework.Widgets.ClipRect(child: ((_NavigationBarStaticComponents__nav_bar)this.components).navBarBottom))))); } return __collection60368; }))())))); if ((object.Equals(this.bottomMode, NavigationBarBottomMode.always))) { __collection60282.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(height: this.bottomHeight, child: ((_NavigationBarStaticComponents__nav_bar)this.components).navBarBottom))); } return __collection60282; }))())));
        if ((!this.transitionBetweenRoutes || !Nav_barLibrary._isTransitionable(context)))
        {
            return navBar;
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Hero(tag: ((object.Equals(this.heroTag, Nav_barLibrary._defaultHeroTag)) ? new _HeroTag__nav_bar(Navigator.of(context)) : this.heroTag), createRectTween: (global::System.Func<Rect?, Rect?, global::Doroti.Framework.Animation.RectTween>)Nav_barLibrary._linearTranslateWithLargestRectSizeTween, flightShuttleBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Animation.Animation<double>, global::Doroti.Framework.Widgets.HeroFlightDirection, global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget>)Nav_barLibrary._navBarHeroFlightShuttleBuilder, placeholderBuilder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, Size, global::Doroti.Framework.Widgets.Widget, global::Doroti.Framework.Widgets.Widget>)Nav_barLibrary._navBarHeroLaunchPadBuilder, transitionOnUserGestures: true, child: new _TransitionableNavigationBar__nav_bar(componentsKeys: this.keys, backgroundColor: effectiveBackgroundColor, backButtonTextStyle: CupertinoTheme.of(context).textTheme.navActionTextStyle, titleTextStyle: CupertinoTheme.of(context).textTheme.navTitleTextStyle, largeTitleTextStyle: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, border: effectiveBorder, hasUserMiddle: ((this.userMiddle is not null) && ((this.alwaysShowMiddle || !showLargeTitle))), largeExpanded: showLargeTitle, searchable: this.searchable, automaticBackgroundVisibility: this.automaticBackgroundVisibility, child: navBar)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(global::Doroti.Framework.Widgets.SliverPersistentHeaderDelegate oldDelegate)
    {
        var __oldDelegate = (_LargeTitleNavigationBarSliverDelegate__nav_bar)(object)oldDelegate;
        return (((((((((((((((((!object.Equals(this.components, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).components)) || (!object.Equals(this.userMiddle, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).userMiddle))) || (!object.Equals(this.backgroundColor, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).backgroundColor))) || (this.automaticBackgroundVisibility != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).automaticBackgroundVisibility)) || (!object.Equals(this.border, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).border))) || (!object.Equals(this.padding, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).padding))) || (!object.Equals(this.actionsForegroundColor, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).actionsForegroundColor))) || (this.transitionBetweenRoutes != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).transitionBetweenRoutes)) || (this.persistentHeight != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).persistentHeight)) || (this.largeTitleHeight != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).largeTitleHeight)) || (this.alwaysShowMiddle != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).alwaysShowMiddle)) || (!object.Equals(this.heroTag, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).heroTag))) || (this.enableBackgroundFilterBlur != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).enableBackgroundFilterBlur)) || (!object.Equals(this.bottomMode, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).bottomMode))) || (this.bottomHeight != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).bottomHeight)) || (!object.Equals(this.controller, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).controller))) || (this.searchable != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).searchable));
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
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderLargeTitle__nav_bar(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.bottomStart.resolve(Directionality.of(context)), height: this.height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Framework.Widgets.BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderLargeTitle__nav_bar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderLargeTitle__nav_bar>)(() =>
{
    var __cascade = __renderObject;
    __cascade.alignment = global::Doroti.Framework.Painting.AlignmentDirectional.bottomStart.resolve(Directionality.of(context));
    __cascade.height = this.height;
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
        this._alignment = alignment;
        this._height = height;
    }

    public virtual global::Doroti.Framework.Painting.Alignment alignment
    {
        get => this._alignment;
        set
        {
            var __value = value;
            if ((object.Equals(this._alignment, __value)))
            {
                return;
            }
            _alignment = __value;
            markNeedsLayout();
        }
    }
    public virtual double height
    {
        get => this._height;
        set
        {
            var __value = value;
            if ((this._height == __value))
            {
                return;
            }
            _height = __value;
            markNeedsLayout();
        }
    }
    internal static double _computeTitleScale(Size childSize, global::Doroti.Framework.Rendering.BoxConstraints constraints, double height)
    {
        double maxHeightLocal = (height - Nav_barLibrary._kNavBarBottomPadding);
        double scale = (1.0 + ((0.03 * ((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxHeight - maxHeightLocal))) / maxHeightLocal));
        double maxScale = ((childSize.width != 0.0) ? Dart_uiLibrary.clampDouble((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth / childSize.width), 1.0, 1.1) : 1.1);
        return Dart_uiLibrary.clampDouble(scale, 1.0, maxScale);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        double? distance = this.child?.getDistanceToActualBaseline(baseline);
        if ((distance is null))
        {
            return null;
        }
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
        return (((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset.dy + (DartRuntimePrimitives.RequireValue(distance) * this._scale));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return null;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)constraints.widthConstraints().loosen());
        double? result = childLocal.getDryBaseline(childConstraints, baseline);
        if ((result is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize = ((global::Doroti.Ui.Size)(object?)childLocal.getDryLayout(childConstraints));
        double scale = _RenderLargeTitle__nav_bar._computeTitleScale(childSize, constraints, this.height);
        global::Doroti.Ui.Size scaledChildSize = ((global::Doroti.Ui.Size)(object?)(childSize * scale));
        return ((DartRuntimePrimitives.RequireValue(result) * scale) + this.alignment.alongOffset((((global::Doroti.Framework.Rendering.BoxConstraints)constraints).biggest - scaledChildSize)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        size = ((global::Doroti.Framework.Rendering.BoxConstraints)this.constraints).biggest;
        if ((childLocal is null))
        {
            return;
        }
        global::Doroti.Framework.Rendering.BoxConstraints childConstraints = ((global::Doroti.Framework.Rendering.BoxConstraints)(object?)this.constraints.widthConstraints().loosen());
        childLocal.layout(childConstraints, parentUsesSize: true);
        _scale = _RenderLargeTitle__nav_bar._computeTitleScale(((global::Doroti.Framework.Rendering.RenderBox)childLocal).size, this.constraints, this.height);
        var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!;
        childParentData.offset = this.alignment.alongOffset((this.size - ((((global::Doroti.Framework.Rendering.RenderBox)childLocal).size * this._scale))));
    }

    public override void applyPaintTransform(global::Doroti.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Framework.Rendering.RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child, ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child))));
        base.applyPaintTransform(__child, transform);
        transform.scaleByDouble(this._scale, this._scale, this._scale, 1);
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            layer = null;
        }
        else
        {
            var childParentData = ((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!;
            layer = context.pushTransform(this.needsCompositing, (offset + ((global::Doroti.Framework.Rendering.BoxParentData)childParentData).offset), Matrix4.diagonal3Values(this._scale, this._scale, 1.0), ((global::System.Action<global::Doroti.Framework.Rendering.PaintingContext, Offset>)((context, offset) => { context.paintChild(childLocal, offset); })), oldLayer: ((global::Doroti.Framework.Rendering.TransformLayer?)(object?)this.layer)!);
        }
    }

    public override bool hitTestChildren(global::Doroti.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Framework.Rendering.RenderBox? childLocal = ((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((childLocal is null))
        {
            return false;
        }
        global::Doroti.Ui.Offset childOffset = ((global::Doroti.Ui.Offset)(object?)(((global::Doroti.Framework.Rendering.BoxParentData?)(object?)childLocal.parentData!)!).offset);
        var transformLocal = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.scaleByDouble((1.0 / this._scale), (1.0 / this._scale), 1.0, 1);
    __cascade.translateByDouble(-childOffset.dx, -childOffset.dy, 0, 1);
    return __cascade;
}))();
        return result.addWithRawTransform(transform: transformLocal, position: position, hitTest: ((global::System.Func<global::Doroti.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) =>
        {
            return childLocal.hitTest(result, position: transformed);
            throw new InvalidOperationException("Dart closure completed without a value.");
        })));
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
        global::Doroti.Framework.Widgets.Widget? middleLocal = ((global::Doroti.Framework.Widgets.Widget?)(object?)((_NavigationBarStaticComponents__nav_bar)this.components).middle);
        if ((middleLocal is not null))
        {
            middleLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navTitleTextStyle, child: new global::Doroti.Framework.Widgets.Semantics(header: true, child: middleLocal)));
            middleLocal = ((this.middleVisible is null) ? middleLocal : new global::Doroti.Framework.Widgets.AnimatedOpacity(opacity: (DartRuntimePrimitives.RequireValue(this.middleVisible) ? 1.0 : 0.0), duration: Nav_barLibrary._kNavBarTitleFadeDuration, child: middleLocal));
        }
        global::Doroti.Framework.Widgets.Widget? leadingLocal = ((global::Doroti.Framework.Widgets.Widget?)(object?)((_NavigationBarStaticComponents__nav_bar)this.components).leading);
        global::Doroti.Framework.Widgets.Widget? backChevronLocal = ((global::Doroti.Framework.Widgets.Widget?)(object?)((_NavigationBarStaticComponents__nav_bar)this.components).backChevron);
        global::Doroti.Framework.Widgets.Widget? backLabelLocal = ((global::Doroti.Framework.Widgets.Widget?)(object?)((_NavigationBarStaticComponents__nav_bar)this.components).backLabel);
        if (((((leadingLocal is null) && (backChevronLocal is not null)) && (backLabelLocal is not null)) && !CupertinoSheetRoute<object>.hasParentSheet(context)))
        {
            leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(CupertinoNavigationBarBackButton.Create_assemble(backChevronLocal, backLabelLocal));
        }
        else
        {
            leadingLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Align(widthFactor: 1.0, child: leadingLocal));
        }
        global::Doroti.Framework.Widgets.Widget paddedToolbar = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.NavigationToolbar(leading: leadingLocal, middle: middleLocal, trailing: ((_NavigationBarStaticComponents__nav_bar)this.components).trailing, middleSpacing: 6.0));
        if ((this.padding is not null))
        {
            paddedToolbar = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(top: this.padding!.top, bottom: this.padding!.bottom), child: paddedToolbar));
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(height: (Nav_barLibrary._kNavBarPersistentHeight + MediaQuery.paddingOf(context).top), child: new global::Doroti.Framework.Widgets.SafeArea(top: !CupertinoSheetRoute<object>.hasParentSheet(context), bottom: false, child: paddedToolbar)));
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
        this.navBarBoxKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Navigation bar render box");
        this.leadingKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Leading");
        this.backChevronKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Back chevron");
        this.backLabelKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Back label");
        this.middleKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Middle");
        this.trailingKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Trailing");
        this.largeTitleKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Large title");
        this.navBarBottomKey = global::Doroti.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Navigation bar bottom");
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

    internal _NavigationBarStaticComponents__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar keys, dynamic route, global::Doroti.Framework.Widgets.Widget? userLeading, bool automaticallyImplyLeading, bool automaticallyImplyTitle, string? previousPageTitle, global::Doroti.Framework.Widgets.Widget? userMiddle, global::Doroti.Framework.Widgets.Widget? userTrailing, global::Doroti.Framework.Widgets.Widget? userLargeTitle, global::Doroti.Framework.Widgets.Widget? userBottom, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding, bool large, bool staticBar, global::Doroti.Framework.Widgets.BuildContext context)
    {
        this.leading = _NavigationBarStaticComponents__nav_bar.createLeading(leadingKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).leadingKey, userLeading: userLeading, route: route, automaticallyImplyLeading: automaticallyImplyLeading, padding: padding, context: context);
        this.backChevron = _NavigationBarStaticComponents__nav_bar.createBackChevron(backChevronKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).backChevronKey, userLeading: userLeading, route: route, automaticallyImplyLeading: automaticallyImplyLeading, context: context);
        this.backLabel = _NavigationBarStaticComponents__nav_bar.createBackLabel(backLabelKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).backLabelKey, userLeading: userLeading, route: route, previousPageTitle: previousPageTitle, automaticallyImplyLeading: automaticallyImplyLeading, context: context);
        this.middle = _NavigationBarStaticComponents__nav_bar.createMiddle(middleKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).middleKey, userMiddle: userMiddle, userLargeTitle: userLargeTitle, route: route, automaticallyImplyTitle: automaticallyImplyTitle, large: large, staticBar: staticBar, context: context);
        this.trailing = _NavigationBarStaticComponents__nav_bar.createTrailing(trailingKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).trailingKey, userTrailing: userTrailing, padding: padding, context: context);
        this.largeTitle = _NavigationBarStaticComponents__nav_bar.createLargeTitle(largeTitleKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).largeTitleKey, userLargeTitle: userLargeTitle, route: route, automaticImplyTitle: automaticallyImplyTitle, large: large, context: context);
        this.navBarBottom = _NavigationBarStaticComponents__nav_bar.createNavBarBottom(navBarBottomKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).navBarBottomKey, userBottom: userBottom, context: context);
    }

    internal static global::Doroti.Framework.Widgets.Widget? _derivedTitle(bool automaticallyImplyTitle, dynamic currentRoute = null)
    {
        if (((automaticallyImplyTitle && (currentRoute is CupertinoRouteTransitionMixin<object>)) && (((CupertinoRouteTransitionMixin<object>)currentRoute).title is not null)))
        {
            CupertinoRouteTransitionMixin<object> currentRoute__as76488 = (CupertinoRouteTransitionMixin<object>)currentRoute;
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.Text(((CupertinoRouteTransitionMixin<object>)currentRoute__as76488).title!));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createLeading(global::Doroti.Framework.Widgets.GlobalKey<IState> leadingKey, global::Doroti.Framework.Widgets.Widget? userLeading, dynamic route, bool automaticallyImplyLeading, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding, global::Doroti.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Framework.Widgets.Widget? leadingContent = default!;
        if ((userLeading is not null))
        {
            leadingContent = userLeading;
        }
        else
        {
            if ((((automaticallyImplyLeading && (route is PageRoute<object>)) && ((bool)((dynamic)route).canPop)) && ((bool)((dynamic)route).fullscreenDialog)))
            {
                dynamic route__as77104 = (dynamic)route;
                leadingContent = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new CupertinoButton(padding: global::Doroti.Framework.Painting.EdgeInsets.zero, onPressed: (() =>
                {
                    DartRuntimePrimitives.Ignore(((global::Doroti.Framework.Widgets.NavigatorState?)((dynamic)route__as77104).navigator)!.maybePop<object>());
                }), child: new global::Doroti.Framework.Widgets.Text(CupertinoLocalizations.of(context).cancelButtonLabel)));
            }
        }
        if ((leadingContent is null))
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: leadingKey, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (padding?.start ?? Nav_barLibrary._kNavBarEdgePadding)), child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(size: 32.0), child: leadingContent))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createBackChevron(global::Doroti.Framework.Widgets.GlobalKey<IState> backChevronKey, global::Doroti.Framework.Widgets.Widget? userLeading, dynamic route, bool automaticallyImplyLeading, global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((((((userLeading is not null) || !automaticallyImplyLeading) || (route is null)) || !((bool)((dynamic)route).canPop)) || (((route is PageRoute<object>) && ((bool)((dynamic)route).fullscreenDialog)))))
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: backChevronKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: new _BackChevron__nav_bar()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createBackLabel(global::Doroti.Framework.Widgets.GlobalKey<IState> backLabelKey, global::Doroti.Framework.Widgets.Widget? userLeading, dynamic route, bool automaticallyImplyLeading, string? previousPageTitle, global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((((((userLeading is not null) || !automaticallyImplyLeading) || (route is null)) || !((bool)((dynamic)route).canPop)) || (((route is PageRoute<object>) && ((bool)((dynamic)route).fullscreenDialog)))))
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: backLabelKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: new _BackLabel__nav_bar(specifiedPreviousTitle: previousPageTitle, route: route)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createMiddle(global::Doroti.Framework.Widgets.GlobalKey<IState> middleKey, global::Doroti.Framework.Widgets.Widget? userMiddle, global::Doroti.Framework.Widgets.Widget? userLargeTitle, bool large, bool staticBar, bool automaticallyImplyTitle, dynamic route, global::Doroti.Framework.Widgets.BuildContext context)
    {
        var middleContent = userMiddle;
        if ((large && staticBar))
        {
            return null;
        }
        if (large)
        {
            middleContent ??= userLargeTitle;
        }
        middleContent ??= _NavigationBarStaticComponents__nav_bar._derivedTitle(automaticallyImplyTitle: automaticallyImplyTitle, currentRoute: route);
        if ((middleContent is null))
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: middleKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: middleContent));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createTrailing(global::Doroti.Framework.Widgets.GlobalKey<IState> trailingKey, global::Doroti.Framework.Widgets.Widget? userTrailing, global::Doroti.Framework.Painting.EdgeInsetsDirectional? padding, global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((userTrailing is null))
        {
            return null;
        }
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: trailingKey, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: (padding?.end ?? Nav_barLibrary._kNavBarEdgePadding)), child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: IconTheme.merge(data: new global::Doroti.Framework.Widgets.IconThemeData(size: 32.0), child: userTrailing))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createLargeTitle(global::Doroti.Framework.Widgets.GlobalKey<IState> largeTitleKey, global::Doroti.Framework.Widgets.Widget? userLargeTitle, bool large, bool automaticImplyTitle, dynamic route, global::Doroti.Framework.Widgets.BuildContext context)
    {
        if (!large)
        {
            return null;
        }
        global::Doroti.Framework.Widgets.Widget? largeTitleContent = ((userLargeTitle ?? (global::Doroti.Framework.Widgets.Widget)_NavigationBarStaticComponents__nav_bar._derivedTitle(automaticallyImplyTitle: automaticImplyTitle, currentRoute: route)));
        DartRuntimePrimitives.Assert(() => (largeTitleContent is not null), () => (object?)"largeTitle was not provided and there was no title from the route.");
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: largeTitleKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: global::Doroti.Framework.Painting.TextScaler.CreateLinear(Nav_barLibrary._dampScaleFactor(MediaQuery.textScalerOf(context).scale(Nav_barLibrary._kNavBarLargeTitleHeightExtension), Nav_barLibrary._kNavBarLargeTitleHeightExtension, Nav_barLibrary._kLargeTitleScaleDampingRatio))), child: largeTitleContent!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Framework.Widgets.KeyedSubtree? createNavBarBottom(global::Doroti.Framework.Widgets.GlobalKey<IState> navBarBottomKey, global::Doroti.Framework.Widgets.Widget? userBottom, global::Doroti.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Framework.Widgets.KeyedSubtree(key: navBarBottomKey, child: new global::Doroti.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: MediaQuery.textScalerOf(context)), child: (userBottom ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Framework.Painting.TextScaler _clampedTextScaler(global::Doroti.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Framework.Painting.TextScaler)(object?)MediaQuery.textScalerOf(context).clamp(minScaleFactor: 1.0, maxScaleFactor: Nav_barLibrary._kMaxScaleFactor));
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
        this._backChevron = null;
        this._backLabel = null;
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
        dynamic currentRoute = global::Doroti.Framework.Widgets.ModalRoute<object>.of<object>(context);
        if ((this.onPressed is null))
        {
            DartRuntimePrimitives.Assert(() => (((bool?)((dynamic)currentRoute)?.canPop) ?? false), () => (object?)"CupertinoNavigationBarBackButton should only be used in routes that can be popped");
        }
        global::Doroti.Framework.Painting.TextStyle actionTextStyle = CupertinoTheme.of(context).textTheme.navActionTextStyle;
        if ((this.color is not null))
        {
            actionTextStyle = actionTextStyle.copyWith(color: CupertinoDynamicColor.maybeResolve(this.color, context));
        }
        CupertinoLocalizations localizations = CupertinoLocalizations.of(context);
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new CupertinoButton(padding: global::Doroti.Framework.Painting.EdgeInsets.zero, child: new global::Doroti.Framework.Widgets.Semantics(container: true, excludeSemantics: true, label: localizations.backButtonLabel, button: true, child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: actionTextStyle, child: new global::Doroti.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Framework.Rendering.BoxConstraints(minWidth: Nav_barLibrary._kNavBarBackButtonTapWidth), child: new global::Doroti.Framework.Widgets.Row(mainAxisSize: global::Doroti.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 8.0))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>((this._backChevron ?? new _BackChevron__nav_bar())), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 6.0))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Flexible(child: (this._backLabel ?? new _BackLabel__nav_bar(specifiedPreviousTitle: this.previousPageTitle, route: currentRoute)))) })))), onPressed: (() =>
        {
            if ((this.onPressed is not null))
            {
                this.onPressed!();
            }
            else
            {
                DartRuntimePrimitives.Ignore(Navigator.maybePop<object>(context));
            }
        })));
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
        global::Doroti.Framework.Widgets.Widget iconWidget = ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 6, end: 2), child: global::Doroti.Framework.Widgets.Text.CreateRich(new global::Doroti.Framework.Painting.TextSpan(text: char.ConvertFromUtf32(checked((int)CupertinoIcons.back.codePoint)), style: new global::Doroti.Framework.Painting.TextStyle(inherit: false, color: ((global::Doroti.Framework.Painting.TextStyle)textStyle).color, fontSize: 30.0, fontFamily: CupertinoIcons.back.fontFamily, package: CupertinoIcons.back.fontPackage)))));
        switch (textDirection)
        {
            case TextDirection.rtl:
                {
                    iconWidget = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Transform(transform: ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.identity();
    __cascade.scaleByDouble(-1.0, 1.0, 1.0, 1);
    return __cascade;
}))(), alignment: global::Doroti.Framework.Painting.Alignment.center, transformHitTests: false, child: iconWidget));
                    break;
                }
            case TextDirection.ltr:
                {
                    break;
                }
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.KeyedSubtree(key: StandardComponentTypeMembers.key(global::Doroti.Framework.Widgets.StandardComponentType.backButton), child: iconWidget));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BackLabel__nav_bar : global::Doroti.Framework.Widgets.StatelessWidget
{
    public virtual string? specifiedPreviousTitle { get; private set; }
    public virtual dynamic route { get; private set; } = default!;

    internal _BackLabel__nav_bar(string? specifiedPreviousTitle, dynamic route)
    {
        this.specifiedPreviousTitle = specifiedPreviousTitle;
        this.route = route;
    }

    internal virtual global::Doroti.Framework.Widgets.Widget _buildPreviousTitleWidget(global::Doroti.Framework.Widgets.BuildContext context, string? previousTitle, global::Doroti.Framework.Widgets.Widget? child)
    {
        if ((previousTitle is null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
        }
        var textWidget = new global::Doroti.Framework.Widgets.Text(previousTitle, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis);
        if ((previousTitle.Length > 12L))
        {
            textWidget = new global::Doroti.Framework.Widgets.Text(CupertinoLocalizations.of(context).backButtonLabel);
        }
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, widthFactor: 1.0, child: textWidget));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        if ((this.specifiedPreviousTitle is not null))
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)_buildPreviousTitleWidget(context, this.specifiedPreviousTitle, null));
        }
        else
        {
            if (((this.route is CupertinoRouteTransitionMixin<object>) && !((bool)((dynamic)this.route!).isFirst)))
            {
                CupertinoRouteTransitionMixin<object> route__as89428 = (CupertinoRouteTransitionMixin<object>)route;
                var cupertinoRoute = ((CupertinoRouteTransitionMixin<object>?)(object?)this.route!)!;
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.ValueListenableBuilder<string?>(valueListenable: ((CupertinoRouteTransitionMixin<object>)cupertinoRoute).previousTitle, builder: (global::System.Func<global::Doroti.Framework.Widgets.BuildContext, string?, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)this._buildPreviousTitleWidget));
            }
            else
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)global::Doroti.Framework.Widgets.SizedBox.CreateShrink());
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
        return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.Alignment.centerLeft, child: new global::Doroti.Framework.Widgets.Opacity(opacity: this.opacity, child: new CupertinoButton(padding: global::Doroti.Framework.Painting.EdgeInsets.zero, onPressed: this.onPressed, child: new global::Doroti.Framework.Widgets.Text(localizations.cancelButtonLabel, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.clip))))));
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
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this.animation, child: new global::Doroti.Framework.Widgets.GestureDetector(onTap: () => this.onSearchFieldTap(), child: new global::Doroti.Framework.Widgets.AbsorbPointer(child: new global::Doroti.Framework.Widgets.FocusableActionDetector(descendantsAreFocusable: false, child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, end: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Framework.Widgets.SizedBox(height: this.searchFieldHeight, child: this.searchField))))), builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) =>
        {
            return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Rendering.BoxConstraints, global::Doroti.Framework.Widgets.Widget>)((context, constraints) =>
            {
                return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Row(children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: (((global::Doroti.Framework.Rendering.BoxConstraints)constraints).maxWidth - ((Nav_barLibrary._kSearchFieldCancelButtonWidth * ((global::Doroti.Framework.Animation.AnimationController)this.animationController).value))), child: child)), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.SizedBox(width: (((global::Doroti.Framework.Animation.AnimationController)this.animationController).value * Nav_barLibrary._kSearchFieldCancelButtonWidth), child: new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsets.CreateOnly(bottom: Nav_barLibrary._kNavBarBottomPadding), child: new _CancelButton__nav_bar(opacity: 0.4, onPressed: ((global::System.Action)(() => {
})))))) }));
                throw new InvalidOperationException("Dart closure completed without a value.");
            }))));
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
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
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Padding(padding: global::Doroti.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Framework.Widgets.Row(spacing: 12.0, children: new List<global::Doroti.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.Expanded(child: new global::Doroti.Framework.Widgets.SizedBox(height: this.searchFieldHeight, child: (this.searchField ?? global::Doroti.Framework.Widgets.SizedBox.CreateShrink())))), DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.AnimatedBuilder(animation: this.animation, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0).animate(this.animationController), child: new _CancelButton__nav_bar(onPressed: () => this.onSearchFieldTap())), builder: ((global::System.Func<global::Doroti.Framework.Widgets.BuildContext, global::Doroti.Framework.Widgets.Widget?, global::Doroti.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.SizedBox(width: (((global::Doroti.Framework.Animation.AnimationController)this.animationController).value * Nav_barLibrary._kSearchFieldCancelButtonWidth), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})))) })));
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

    internal _TransitionableNavigationBar__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar componentsKeys, Color? backgroundColor, global::Doroti.Framework.Painting.TextStyle backButtonTextStyle, global::Doroti.Framework.Painting.TextStyle titleTextStyle, global::Doroti.Framework.Painting.TextStyle? largeTitleTextStyle, global::Doroti.Framework.Painting.Border? border, bool hasUserMiddle, bool largeExpanded, bool searchable, bool automaticBackgroundVisibility, global::Doroti.Framework.Widgets.Widget child) : base(key: ((_NavigationBarStaticComponentsKeys__nav_bar)componentsKeys).navBarBoxKey)
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
        System.Diagnostics.Debug.Assert((!largeExpanded || (largeTitleTextStyle is not null)));
    }

    public virtual global::Doroti.Framework.Rendering.RenderBox renderBox
    {
        get
        {
            var box = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.componentsKeys).navBarBoxKey.currentContext!.findRenderObject()!)!;
            DartRuntimePrimitives.Assert(() => box.attached, () => (object?)"_TransitionableNavigationBar.renderBox should be called when building " + "hero flight shuttles when the from and the to nav bar boxes are already " + "laid out and painted.");
            return box;
            return default!;
        }
    }
    public virtual bool userGestureInProgress
    {
        get
        {
            return Navigator.of(((_NavigationBarStaticComponentsKeys__nav_bar)this.componentsKeys).navBarBoxKey.currentContext!).userGestureInProgress;
            return default!;
        }
    }
    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var inHero = false;
                context.visitAncestorElements(((global::System.Func<global::Doroti.Framework.Widgets.Element, bool>)((ancestor) =>
                {
                    if ((ancestor is global::Doroti.Framework.Widgets.ComponentElement))
                    {
                        DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Widgets.ComponentElement)ancestor).widget), typeof(_NavigationBarTransition__nav_bar))), () => (object?)"_TransitionableNavigationBar should never re-appear inside " + "_NavigationBarTransition. Keyed _TransitionableNavigationBar should " + "only serve as anchor points in routes rather than appearing inside " + "Hero flights themselves.");
                        if ((object.Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Framework.Widgets.ComponentElement)ancestor).widget), typeof(global::Doroti.Framework.Widgets.Hero))))
                        {
                            inHero = true;
                        }
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                })));
                DartRuntimePrimitives.Assert(() => inHero, () => (object?)"_TransitionableNavigationBar should only be added as the immediate " + "child of Hero widgets.");
                return true;
            });
        return this.child;
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
        this.heightTween = new global::Doroti.Framework.Animation.Tween<double>(begin: ((_TransitionableNavigationBar__nav_bar)bottomNavBar).renderBox.size.height, end: ((_TransitionableNavigationBar__nav_bar)topNavBar).renderBox.size.height);
    }

    public override global::Doroti.Framework.Widgets.Widget build(global::Doroti.Framework.Widgets.BuildContext context)
    {
        var componentsTransition = new _NavigationBarComponentsTransition__nav_bar(animation: this.animation, bottomNavBar: this.bottomNavBar, topNavBar: this.topNavBar, directionality: Directionality.of(context));
        var childrenLocal = ((Func<List<global::Doroti.Framework.Widgets.Widget>>)(() => { var __collection98801 = new List<global::Doroti.Framework.Widgets.Widget>(); var __collectionElement98817 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).bottomNavBarBackground; if (__collectionElement98817 is { } __nonNullCollectionElement98817) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement98817)); } var __collectionElement98869 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).bottomBackChevron; if (__collectionElement98869 is { } __nonNullCollectionElement98869) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement98869)); } var __collectionElement98916 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).bottomBackLabel; if (__collectionElement98916 is { } __nonNullCollectionElement98916) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement98916)); } var __collectionElement98961 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).bottomLeading; if (__collectionElement98961 is { } __nonNullCollectionElement98961) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement98961)); } var __collectionElement99004 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).bottomMiddle; if (__collectionElement99004 is { } __nonNullCollectionElement99004) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99004)); } var __collectionElement99046 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).bottomLargeTitle; if (__collectionElement99046 is { } __nonNullCollectionElement99046) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99046)); } var __collectionElement99092 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).bottomTrailing; if (__collectionElement99092 is { } __nonNullCollectionElement99092) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99092)); } var __collectionElement99136 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).bottomNavBarBottom; if (__collectionElement99136 is { } __nonNullCollectionElement99136) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99136)); } var __collectionElement99246 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).topNavBarBackground; if (__collectionElement99246 is { } __nonNullCollectionElement99246) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99246)); } var __collectionElement99295 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).topLeading; if (__collectionElement99295 is { } __nonNullCollectionElement99295) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99295)); } var __collectionElement99335 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).topBackChevron; if (__collectionElement99335 is { } __nonNullCollectionElement99335) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99335)); } var __collectionElement99379 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).topBackLabel; if (__collectionElement99379 is { } __nonNullCollectionElement99379) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99379)); } var __collectionElement99421 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).topMiddle; if (__collectionElement99421 is { } __nonNullCollectionElement99421) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99421)); } var __collectionElement99460 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).topLargeTitle; if (__collectionElement99460 is { } __nonNullCollectionElement99460) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99460)); } var __collectionElement99503 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).topTrailing; if (__collectionElement99503 is { } __nonNullCollectionElement99503) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99503)); } var __collectionElement99544 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition).topNavBarBottom; if (__collectionElement99544 is { } __nonNullCollectionElement99544) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(__nonNullCollectionElement99544)); } return __collection98801; }))();
        return ((global::Doroti.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: new global::Doroti.Framework.Widgets.SizedBox(height: (Math.Max(DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Tween<double>)this.heightTween).begin), DartRuntimePrimitives.RequireValue(((global::Doroti.Framework.Animation.Tween<double>)this.heightTween).end)) + MediaQuery.paddingOf(context).top), width: double.PositiveInfinity, child: new global::Doroti.Framework.Widgets.Stack(children: childrenLocal))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarComponentsTransition__nav_bar
{
    public static global::Doroti.Framework.Animation.Animatable<double> fadeOut = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0));
    public static global::Doroti.Framework.Animation.Animatable<double> fadeIn = ((global::Doroti.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0));
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
        this.bottomComponents = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).componentsKeys;
        this.topComponents = ((_TransitionableNavigationBar__nav_bar)topNavBar).componentsKeys;
        this.bottomNavBarBox = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).renderBox;
        this.topNavBarBox = ((_TransitionableNavigationBar__nav_bar)topNavBar).renderBox;
        this.bottomBackButtonTextStyle = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).backButtonTextStyle;
        this.topBackButtonTextStyle = ((_TransitionableNavigationBar__nav_bar)topNavBar).backButtonTextStyle;
        this.bottomTitleTextStyle = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).titleTextStyle;
        this.topTitleTextStyle = ((_TransitionableNavigationBar__nav_bar)topNavBar).titleTextStyle;
        this.bottomLargeTitleTextStyle = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).largeTitleTextStyle;
        this.topLargeTitleTextStyle = ((_TransitionableNavigationBar__nav_bar)topNavBar).largeTitleTextStyle;
        this.bottomHasUserMiddle = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).hasUserMiddle;
        this.topHasUserMiddle = ((_TransitionableNavigationBar__nav_bar)topNavBar).hasUserMiddle;
        this.bottomLargeExpanded = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).largeExpanded;
        this.topLargeExpanded = ((_TransitionableNavigationBar__nav_bar)topNavBar).largeExpanded;
        this.bottomBackgroundColor = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).backgroundColor;
        this.topBackgroundColor = ((_TransitionableNavigationBar__nav_bar)topNavBar).backgroundColor;
        this.bottomBorder = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).border;
        this.topBorder = ((_TransitionableNavigationBar__nav_bar)topNavBar).border;
        this.bottomAutomaticBackgroundVisibility = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).automaticBackgroundVisibility;
        this.userGestureInProgress = (((_TransitionableNavigationBar__nav_bar)topNavBar).userGestureInProgress || ((_TransitionableNavigationBar__nav_bar)bottomNavBar).userGestureInProgress);
        this.searchable = (((_TransitionableNavigationBar__nav_bar)topNavBar).searchable && ((_TransitionableNavigationBar__nav_bar)bottomNavBar).searchable);
        this.transitionBox = ((_TransitionableNavigationBar__nav_bar)bottomNavBar).renderBox.paintBounds.expandToInclude(((_TransitionableNavigationBar__nav_bar)topNavBar).renderBox.paintBounds);
        this.forwardDirection = ((object.Equals(directionality, TextDirection.ltr)) ? 1.0 : -1.0);
    }

    public virtual global::Doroti.Framework.Rendering.RelativeRect positionInTransitionBox(global::Doroti.Framework.Widgets.GlobalKey<IState> key, global::Doroti.Framework.Rendering.RenderBox from)
    {
        var componentBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)key).currentContext!.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => componentBox.attached);
        return global::Doroti.Framework.Rendering.RelativeRect.CreateFromRect((((Offset)((dynamic)componentBox).localToGlobal(Offset.zero, ancestor: from)) & ((global::Doroti.Framework.Rendering.RenderBox)componentBox).size), this.transitionBox);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _FixedSizeSlidingTransition__nav_bar slideFromLeadingEdge(global::Doroti.Framework.Widgets.GlobalKey<IState> fromKey, global::Doroti.Framework.Rendering.RenderBox fromNavBarBox, global::Doroti.Framework.Widgets.GlobalKey<IState> toKey, global::Doroti.Framework.Rendering.RenderBox toNavBarBox, global::Doroti.Framework.Animation.Curve curve = default!, global::Doroti.Framework.Widgets.Widget child = default!)
    {
        var fromBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)fromKey).currentContext!.findRenderObject()!)!;
        var toBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Framework.Widgets.GlobalKey<IState>)toKey).currentContext!.findRenderObject()!)!;
        bool isLTRLocal = (this.forwardDirection > 0L);
        var fromAnchorLocal = new global::Doroti.Ui.Offset((isLTRLocal ? 0 : ((global::Doroti.Framework.Rendering.RenderBox)fromBox).size.width), (((global::Doroti.Framework.Rendering.RenderBox)fromBox).size.height / 2L));
        var toAnchorLocal = new global::Doroti.Ui.Offset((isLTRLocal ? 0 : ((global::Doroti.Framework.Rendering.RenderBox)toBox).size.width), (((global::Doroti.Framework.Rendering.RenderBox)toBox).size.height / 2L));
        global::Doroti.Ui.Offset fromAnchorInFromBox = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)fromBox).localToGlobal(fromAnchorLocal, ancestor: fromNavBarBox)));
        global::Doroti.Ui.Offset toAnchorInToBox = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)toBox).localToGlobal(toAnchorLocal, ancestor: toNavBarBox)));
        global::Doroti.Ui.Offset translation = ((global::Doroti.Ui.Offset)(object?)(isLTRLocal ? (toAnchorInToBox - fromAnchorInFromBox) : (new global::Doroti.Ui.Offset((((global::Doroti.Framework.Rendering.RenderBox)toNavBarBox).size.width - toAnchorInToBox.dx), toAnchorInToBox.dy) - new global::Doroti.Ui.Offset((((global::Doroti.Framework.Rendering.RenderBox)fromNavBarBox).size.width - fromAnchorInFromBox.dx), fromAnchorInFromBox.dy))));
        global::Doroti.Framework.Rendering.RelativeRect fromBoxMargin = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(fromKey, from: fromNavBarBox));
        var fromOriginInTransitionBox = new global::Doroti.Ui.Offset((isLTRLocal ? ((global::Doroti.Framework.Rendering.RelativeRect)fromBoxMargin).left : ((global::Doroti.Framework.Rendering.RelativeRect)fromBoxMargin).right), ((global::Doroti.Framework.Rendering.RelativeRect)fromBoxMargin).top);
        var anchorMovementInTransitionBox = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: fromOriginInTransitionBox, end: (fromOriginInTransitionBox + translation));
        return new _FixedSizeSlidingTransition__nav_bar(isLTR: isLTRLocal, offsetAnimation: this.animation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: curve)).drive(anchorMovementInTransitionBox), width: ((global::Doroti.Framework.Rendering.RenderBox)fromNavBarBox).size.width, height: ((global::Doroti.Framework.Rendering.RenderBox)fromBox).size.height, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> fadeInFrom(double t, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        return ((global::Doroti.Framework.Animation.Animation<double>)(object?)this.animation.drive(fadeIn.chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(t, 1.0, curve: curve)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> fadeOutBy(double t, global::Doroti.Framework.Animation.Curve curve = default!)
    {
        return ((global::Doroti.Framework.Animation.Animation<double>)(object?)this.animation.drive(fadeOut.chain(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval(0.0, t, curve: curve)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Animation.Animation<double> routeAnimation
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.animation is global::Doroti.Framework.Animation.CurvedAnimation));
            return (((global::Doroti.Framework.Animation.CurvedAnimation?)(object?)this.animation)!).parent;
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomNavBarBackground
    {
        get
        {
            if (((this.bottomBackgroundColor is null) || ((this.bottomLargeExpanded && this.bottomAutomaticBackgroundVisibility))))
            {
                return null;
            }
            global::Doroti.Framework.Animation.Curve animationCurve = ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward)) ? global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut : global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut.flipped);
            global::Doroti.Framework.Animation.Animation<double> pageTransitionAnimation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)this.routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: (this.userGestureInProgress ? global::Doroti.Framework.Animation.Curves.linear : animationCurve))));
            global::Doroti.Framework.Rendering.RelativeRect fromLocal = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).navBarBoxKey, from: this.bottomNavBarBox));
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(end: fromLocal.shift(new global::Doroti.Ui.Offset((this.forwardDirection * -((global::Doroti.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.width), 0.0)), begin: fromLocal);
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: pageTransitionAnimation.drive(positionTween), child: Nav_barLibrary._wrapWithBackground(updateSystemUiOverlay: false, backgroundColor: this.bottomBackgroundColor!, border: this.topBorder, child: new global::Doroti.Framework.Widgets.SizedBox(height: ((global::Doroti.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.height, width: double.PositiveInfinity))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomLeading
    {
        get
        {
            var bottomLeading = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).leadingKey.currentWidget)!;
            if ((bottomLeading is null))
            {
                return null;
            }
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)global::Doroti.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).leadingKey, from: this.bottomNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.4), child: ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomLeading).child)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomBackChevron
    {
        get
        {
            var bottomBackChevron = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backChevronKey.currentWidget)!;
            if ((bottomBackChevron is null))
            {
                return null;
            }
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)global::Doroti.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backChevronKey, from: this.bottomNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: this.bottomBackButtonTextStyle, child: ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomBackChevron).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomBackLabel
    {
        get
        {
            var bottomBackLabel = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backLabelKey.currentWidget)!;
            if ((bottomBackLabel is null))
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect fromLocal = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backLabelKey, from: this.bottomNavBarBox));
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new global::Doroti.Ui.Offset((this.forwardDirection * ((-((global::Doroti.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.width / 2.0))), 0.0)));
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: this.animation.drive(positionTween), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.2), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: this.bottomBackButtonTextStyle, child: ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomBackLabel).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomMiddle
    {
        get
        {
            var bottomMiddle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey.currentWidget)!;
            var topBackLabel = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey.currentWidget)!;
            var topLeading = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).leadingKey.currentWidget)!;
            if ((!this.bottomHasUserMiddle && this.bottomLargeExpanded))
            {
                return null;
            }
            if (((bottomMiddle is not null) && (topBackLabel is not null)))
            {
                return ((global::Doroti.Framework.Widgets.Widget?)(object?)slideFromLeadingEdge(fromKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey, fromNavBarBox: this.bottomNavBarBox, toKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey, toNavBarBox: this.topNavBarBox, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy((this.bottomHasUserMiddle ? 0.4 : 0.7)), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.DefaultTextStyleTransition(style: this.animation.drive(new global::Doroti.Framework.Widgets.TextStyleTween(begin: this.bottomTitleTextStyle, end: this.topBackButtonTextStyle)), child: ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomMiddle).child)))));
            }
            if (((bottomMiddle is not null) && (topLeading is not null)))
            {
                return ((global::Doroti.Framework.Widgets.Widget?)(object?)global::Doroti.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey, from: this.bottomNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy((this.bottomHasUserMiddle ? 0.4 : 0.7)), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: this.bottomTitleTextStyle, child: ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomMiddle).child))));
            }
            return null;
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomLargeTitle
    {
        get
        {
            var bottomLargeTitle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey.currentWidget)!;
            var topBackLabel = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey.currentWidget)!;
            if (((bottomLargeTitle is null) || !this.bottomLargeExpanded))
            {
                return null;
            }
            if ((topBackLabel is not null))
            {
                return ((global::Doroti.Framework.Widgets.Widget?)(object?)slideFromLeadingEdge(fromKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey, fromNavBarBox: this.bottomNavBarBox, toKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey, toNavBarBox: this.topNavBarBox, curve: new global::Doroti.Framework.Animation.Interval(0.0, ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward)) ? 0.7 : 1.0)), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: new global::Doroti.Framework.Widgets.Align(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Framework.Widgets.DefaultTextStyleTransition(style: this.animation.drive(new global::Doroti.Framework.Widgets.TextStyleTween(begin: this.bottomLargeTitleTextStyle, end: this.topBackButtonTextStyle)), maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomLargeTitle).child)))));
            }
            global::Doroti.Framework.Rendering.RelativeRect fromLocal = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey, from: this.bottomNavBarBox));
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new global::Doroti.Ui.Offset(((this.forwardDirection * ((global::Doroti.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.width) / 4.0), 0.0)));
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: this.animation.drive(positionTween), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.4), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: this.bottomLargeTitleTextStyle!, child: ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomLargeTitle).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomTrailing
    {
        get
        {
            var bottomTrailing = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).trailingKey.currentWidget)!;
            if ((bottomTrailing is null))
            {
                return null;
            }
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)global::Doroti.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).trailingKey, from: this.bottomNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomTrailing).child)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? bottomNavBarBottom
    {
        get
        {
            var bottomNavBarBottom = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).navBarBottomKey.currentWidget)!;
            if ((bottomNavBarBottom is null))
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect fromLocal = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).navBarBottomKey, from: this.bottomNavBarBox));
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: fromLocal, end: fromLocal.shift(new global::Doroti.Ui.Offset((this.forwardDirection * -((global::Doroti.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.width), 0.0)));
            global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.KeyedSubtree)bottomNavBarBottom).child;
            global::Doroti.Framework.Animation.Curve animationCurve = ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward)) ? Nav_barLibrary._kBottomNavBarHeaderTransitionCurve : ((global::Doroti.Framework.Animation.Curve)Nav_barLibrary._kBottomNavBarHeaderTransitionCurve).flipped);
            if (!this.searchable)
            {
                childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.8, curve: animationCurve), child: childLocal));
            }
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: (this.userGestureInProgress ? this.routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.linear)).drive(positionTween) : this.animation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: animationCurve)).drive(positionTween)), child: new global::Doroti.Framework.Widgets.ClipRect(child: childLocal)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topNavBarBackground
    {
        get
        {
            if ((this.topBackgroundColor is null))
            {
                return null;
            }
            global::Doroti.Framework.Animation.Curve animationCurve = ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward)) ? global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut : global::Doroti.Framework.Animation.Curves.fastEaseInToSlowEaseOut.flipped);
            global::Doroti.Framework.Animation.Animation<double> pageTransitionAnimation = ((global::Doroti.Framework.Animation.Animation<double>)(object?)this.routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: (this.userGestureInProgress ? global::Doroti.Framework.Animation.Curves.linear : animationCurve))));
            global::Doroti.Framework.Rendering.RelativeRect to = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).navBarBoxKey, from: this.topNavBarBox));
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: to.shift(new global::Doroti.Ui.Offset((this.forwardDirection * ((global::Doroti.Framework.Rendering.RenderBox)this.topNavBarBox).size.width), 0.0)), end: to);
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: pageTransitionAnimation.drive(positionTween), child: Nav_barLibrary._wrapWithBackground(updateSystemUiOverlay: false, backgroundColor: this.topBackgroundColor!, border: this.topBorder, child: new global::Doroti.Framework.Widgets.SizedBox(height: ((global::Doroti.Framework.Rendering.RenderBox)this.topNavBarBox).size.height, width: double.PositiveInfinity))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topLeading
    {
        get
        {
            var topLeading = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).leadingKey.currentWidget)!;
            if ((topLeading is null))
            {
                return null;
            }
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)global::Doroti.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).leadingKey, from: this.topNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.6), child: ((global::Doroti.Framework.Widgets.KeyedSubtree)topLeading).child)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topBackChevron
    {
        get
        {
            var topBackChevron = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backChevronKey.currentWidget)!;
            var bottomBackChevron = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backChevronKey.currentWidget)!;
            if ((topBackChevron is null))
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect to = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backChevronKey, from: this.topNavBarBox));
            var fromLocal = to;
            global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.KeyedSubtree)topBackChevron).child;
            global::Doroti.Framework.Animation.Curve forwardScaleCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.0, 0.2));
            global::Doroti.Framework.Animation.Curve backwardScaleCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.8, 1.0));
            global::Doroti.Framework.Animation.Curve forwardPositionCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.0, 0.5));
            global::Doroti.Framework.Animation.Curve backwardPositionCurve = ((global::Doroti.Framework.Animation.Curve)(object?)new global::Doroti.Framework.Animation.Interval(0.5, 1.0));
            global::Doroti.Framework.Animation.Curve effectiveScaleCurve = default!;
            global::Doroti.Framework.Animation.Curve effectivePositionCurve = default!;
            if ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward)))
            {
                effectiveScaleCurve = forwardScaleCurve;
                effectivePositionCurve = forwardPositionCurve;
            }
            else
            {
                effectiveScaleCurve = backwardScaleCurve;
                effectivePositionCurve = backwardPositionCurve;
            }
            if ((bottomBackChevron is null))
            {
                var topBackChevronBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backChevronKey.currentContext!.findRenderObject()!)!;
                fromLocal = to.shift(new global::Doroti.Ui.Offset(((this.forwardDirection * ((global::Doroti.Framework.Rendering.RenderBox)topBackChevronBox).size.width) * 2.0), 0.0));
                childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.ScaleTransition(scale: this.routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: effectiveScaleCurve)), child: childLocal));
            }
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: fromLocal, end: to);
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: this.routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: effectivePositionCurve)).drive(positionTween), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: this.routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: new global::Doroti.Framework.Animation.Interval((((bottomBackChevron is null) && (!object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward))) ? 0.9 : 0.4), 1.0))), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: this.topBackButtonTextStyle, child: childLocal))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topBackLabel
    {
        get
        {
            var bottomMiddle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey.currentWidget)!;
            var bottomLargeTitle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey.currentWidget)!;
            var topBackLabel = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey.currentWidget)!;
            if ((topBackLabel is null))
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RenderAnimatedOpacity? topBackLabelOpacity = ((global::Doroti.Framework.Rendering.RenderAnimatedOpacity?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey.currentContext?.findAncestorRenderObjectOfType<global::Doroti.Framework.Rendering.RenderAnimatedOpacity>());
            global::Doroti.Framework.Animation.Animation<double>? midClickOpacity = default!;
            if (((topBackLabelOpacity is not null) && (topBackLabelOpacity.opacity.value < 1.0)))
            {
                midClickOpacity = this.animation.drive(new global::Doroti.Framework.Animation.Tween<double>(begin: 0.0, end: topBackLabelOpacity.opacity.value));
            }
            if (((bottomLargeTitle is not null) && this.bottomLargeExpanded))
            {
                return ((global::Doroti.Framework.Widgets.Widget?)(object?)slideFromLeadingEdge(fromKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey, fromNavBarBox: this.bottomNavBarBox, toKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey, toNavBarBox: this.topNavBarBox, curve: new global::Doroti.Framework.Animation.Interval(0.0, ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward)) ? 0.7 : 1.0)), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: ((midClickOpacity ?? (global::Doroti.Framework.Animation.Animation<double>)fadeInFrom(0.4))), child: new global::Doroti.Framework.Widgets.DefaultTextStyleTransition(style: this.animation.drive(new global::Doroti.Framework.Widgets.TextStyleTween(begin: this.bottomLargeTitleTextStyle, end: this.topBackButtonTextStyle)), maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: ((global::Doroti.Framework.Widgets.KeyedSubtree)topBackLabel).child))));
            }
            if ((bottomMiddle is not null))
            {
                return ((global::Doroti.Framework.Widgets.Widget?)(object?)slideFromLeadingEdge(fromKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey, fromNavBarBox: this.bottomNavBarBox, toKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey, toNavBarBox: this.topNavBarBox, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: ((midClickOpacity ?? (global::Doroti.Framework.Animation.Animation<double>)fadeInFrom(0.3))), child: new global::Doroti.Framework.Widgets.DefaultTextStyleTransition(style: this.animation.drive(new global::Doroti.Framework.Widgets.TextStyleTween(begin: this.bottomTitleTextStyle, end: this.topBackButtonTextStyle)), child: ((global::Doroti.Framework.Widgets.KeyedSubtree)topBackLabel).child))));
            }
            return null;
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topMiddle
    {
        get
        {
            var topMiddle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).middleKey.currentWidget)!;
            if ((topMiddle is null))
            {
                return null;
            }
            if ((!this.topHasUserMiddle && this.topLargeExpanded))
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect to = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).middleKey, from: this.topNavBarBox));
            var toBox = ((global::Doroti.Framework.Rendering.RenderBox?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).middleKey.currentContext!.findRenderObject()!)!;
            bool isLTRLocal = (this.forwardDirection > 0L);
            var toAnchorInTransitionBox = new global::Doroti.Ui.Offset((isLTRLocal ? ((global::Doroti.Framework.Rendering.RelativeRect)to).left : ((global::Doroti.Framework.Rendering.RelativeRect)to).right), ((global::Doroti.Framework.Rendering.RelativeRect)to).top);
            var anchorMovementInTransitionBox = new global::Doroti.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset((((global::Doroti.Framework.Rendering.RenderBox)this.topNavBarBox).size.width - (((global::Doroti.Framework.Rendering.RenderBox)toBox).size.width / 2L)), ((global::Doroti.Framework.Rendering.RelativeRect)to).top), end: toAnchorInTransitionBox);
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new _FixedSizeSlidingTransition__nav_bar(isLTR: isLTRLocal, offsetAnimation: this.animation.drive(anchorMovementInTransitionBox), width: ((global::Doroti.Framework.Rendering.RenderBox)toBox).size.width, height: ((global::Doroti.Framework.Rendering.RenderBox)toBox).size.height, child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.25), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: this.topTitleTextStyle, child: ((global::Doroti.Framework.Widgets.KeyedSubtree)topMiddle).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topTrailing
    {
        get
        {
            var topTrailing = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).trailingKey.currentWidget)!;
            if ((topTrailing is null))
            {
                return null;
            }
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)global::Doroti.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).trailingKey, from: this.topNavBarBox), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.4), child: ((global::Doroti.Framework.Widgets.KeyedSubtree)topTrailing).child)));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topLargeTitle
    {
        get
        {
            var topLargeTitle = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).largeTitleKey.currentWidget)!;
            if (((topLargeTitle is null) || !this.topLargeExpanded))
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect to = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).largeTitleKey, from: this.topNavBarBox));
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: to.shift(new global::Doroti.Ui.Offset((this.forwardDirection * ((global::Doroti.Framework.Rendering.RenderBox)this.topNavBarBox).size.width), 0.0)), end: to);
            global::Doroti.Framework.Animation.Curve animationCurve = ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward)) ? Nav_barLibrary._kTopNavBarHeaderTransitionCurve : ((global::Doroti.Framework.Animation.Curve)Nav_barLibrary._kTopNavBarHeaderTransitionCurve).flipped);
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: (this.userGestureInProgress ? this.routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.linear)).drive(positionTween) : this.animation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: animationCurve)).drive(positionTween)), child: new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.0, curve: animationCurve), child: new global::Doroti.Framework.Widgets.DefaultTextStyle(style: this.topLargeTitleTextStyle!, maxLines: 1L, overflow: global::Doroti.Framework.Painting.TextOverflow.ellipsis, child: ((global::Doroti.Framework.Widgets.KeyedSubtree)topLargeTitle).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Widgets.Widget? topNavBarBottom
    {
        get
        {
            var topNavBarBottom = ((global::Doroti.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).navBarBottomKey.currentWidget)!;
            if ((topNavBarBottom is null))
            {
                return null;
            }
            global::Doroti.Framework.Rendering.RelativeRect to = ((global::Doroti.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).navBarBottomKey, from: this.topNavBarBox));
            var positionTween = new global::Doroti.Framework.Widgets.RelativeRectTween(begin: to.shift(new global::Doroti.Ui.Offset((this.forwardDirection * ((global::Doroti.Framework.Rendering.RenderBox)this.topNavBarBox).size.width), 0.0)), end: to);
            global::Doroti.Framework.Widgets.Widget childLocal = ((global::Doroti.Framework.Widgets.KeyedSubtree)topNavBarBottom).child;
            global::Doroti.Framework.Animation.Curve animationCurve = ((object.Equals(((global::Doroti.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Framework.Animation.AnimationStatus.forward)) ? Nav_barLibrary._kTopNavBarHeaderTransitionCurve : ((global::Doroti.Framework.Animation.Curve)Nav_barLibrary._kTopNavBarHeaderTransitionCurve).flipped);
            if (!this.searchable)
            {
                childLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Widgets.Widget>(new global::Doroti.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.0, curve: animationCurve), child: childLocal));
            }
            return ((global::Doroti.Framework.Widgets.Widget?)(object?)new global::Doroti.Framework.Widgets.PositionedTransition(rect: (this.userGestureInProgress ? this.routeAnimation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: global::Doroti.Framework.Animation.Curves.linear)).drive(positionTween) : this.animation.drive(new global::Doroti.Framework.Animation.CurveTween(curve: animationCurve)).drive(positionTween)), child: new global::Doroti.Framework.Widgets.ClipRect(child: childLocal)));
            return default!;
        }
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Animation.RectTween _linearTranslateWithLargestRectSizeTween(Rect? begin, Rect? end)
    {
        var largestSize = new global::Doroti.Ui.Size(Math.Max(DartRuntimePrimitives.RequireValue(begin).size.width, DartRuntimePrimitives.RequireValue(end).size.width), Math.Max(DartRuntimePrimitives.RequireValue(begin).size.height, DartRuntimePrimitives.RequireValue(end).size.height));
        return new global::Doroti.Framework.Animation.RectTween(begin: (DartRuntimePrimitives.RequireValue(begin).topLeft & largestSize), end: (DartRuntimePrimitives.RequireValue(end).topLeft & largestSize));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _navBarHeroLaunchPadBuilder(global::Doroti.Framework.Widgets.BuildContext context, Size heroSize, global::Doroti.Framework.Widgets.Widget child)
    {
        DartRuntimePrimitives.Assert(() => (child is _TransitionableNavigationBar__nav_bar));
        return ((global::Doroti.Framework.Widgets.Widget)(object?)new global::Doroti.Framework.Widgets.Visibility(maintainSize: true, maintainAnimation: true, maintainState: true, visible: false, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Framework.Widgets.Widget _navBarHeroFlightShuttleBuilder(global::Doroti.Framework.Widgets.BuildContext flightContext, global::Doroti.Framework.Animation.Animation<double> animation, global::Doroti.Framework.Widgets.HeroFlightDirection flightDirection, global::Doroti.Framework.Widgets.BuildContext fromHeroContext, global::Doroti.Framework.Widgets.BuildContext toHeroContext)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Widgets.BuildContext)fromHeroContext).widget is global::Doroti.Framework.Widgets.Hero));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Widgets.BuildContext)toHeroContext).widget is global::Doroti.Framework.Widgets.Hero));
        var fromHeroWidget = ((global::Doroti.Framework.Widgets.Hero?)(object?)((global::Doroti.Framework.Widgets.BuildContext)fromHeroContext).widget)!;
        var toHeroWidget = ((global::Doroti.Framework.Widgets.Hero?)(object?)((global::Doroti.Framework.Widgets.BuildContext)toHeroContext).widget)!;
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Widgets.Hero)fromHeroWidget).child is _TransitionableNavigationBar__nav_bar));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Framework.Widgets.Hero)toHeroWidget).child is _TransitionableNavigationBar__nav_bar));
        var fromNavBar = ((_TransitionableNavigationBar__nav_bar?)(object?)((global::Doroti.Framework.Widgets.Hero)fromHeroWidget).child)!;
        var toNavBar = ((_TransitionableNavigationBar__nav_bar?)(object?)((global::Doroti.Framework.Widgets.Hero)toHeroWidget).child)!;
        DartRuntimePrimitives.Assert(() => (((_TransitionableNavigationBar__nav_bar)fromNavBar).componentsKeys.navBarBoxKey.currentContext!.owner is not null), () => (object?)"The from nav bar to Hero must have been mounted in the previous frame");
        DartRuntimePrimitives.Assert(() => (((_TransitionableNavigationBar__nav_bar)toNavBar).componentsKeys.navBarBoxKey.currentContext!.owner is not null), () => (object?)"The to nav bar to Hero must have been mounted in the previous frame");
        switch (flightDirection)
        {
            case global::Doroti.Framework.Widgets.HeroFlightDirection.push:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new _NavigationBarTransition__nav_bar(animation: animation, bottomNavBar: fromNavBar, topNavBar: toNavBar));
                }
            case global::Doroti.Framework.Widgets.HeroFlightDirection.pop:
                {
                    return ((global::Doroti.Framework.Widgets.Widget)(object?)new _NavigationBarTransition__nav_bar(animation: animation, bottomNavBar: toNavBar, topNavBar: fromNavBar));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
