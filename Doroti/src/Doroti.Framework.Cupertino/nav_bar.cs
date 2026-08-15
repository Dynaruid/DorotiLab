// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/cupertino/nav_bar.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Cupertino;

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
    internal static global::Doroti.Generated.Framework.Animation.Curve _kNavBarSearchCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)global::Doroti.Generated.Framework.Animation.Curves.easeInOut);
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
    internal static global::Doroti.Generated.Framework.Painting.Border _kDefaultNavBarBorder = new global::Doroti.Generated.Framework.Painting.Border(bottom: new global::Doroti.Generated.Framework.Painting.BorderSide(color: Nav_barLibrary._kDefaultNavBarBorderColor, width: 0.0));
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.Border _kTransparentNavBarBorder = new global::Doroti.Generated.Framework.Painting.Border(bottom: new global::Doroti.Generated.Framework.Painting.BorderSide(color: new global::Doroti.Ui.Color(0L), width: 0.0));
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _kTopNavBarHeaderTransitionCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.0, 0.45, 0.45, 0.98));
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.Curve _kBottomNavBarHeaderTransitionCurve = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Cubic(0.05, 0.9, 0.9, 0.95));
}

public static partial class Nav_barLibrary
{
    internal static _HeroTag__nav_bar _defaultHeroTag = new _HeroTag__nav_bar(null);
}

internal class _HeroTag__nav_bar
{
    public virtual global::Doroti.Generated.Framework.Widgets.NavigatorState? navigator { get; private set; }

    internal _HeroTag__nav_bar(global::Doroti.Generated.Framework.Widgets.NavigatorState? navigator)
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

public class _FixedSizeSlidingTransition__nav_bar : global::Doroti.Generated.Framework.Widgets.AnimatedWidget
{
    public virtual bool isLTR { get; private set; } = default!;
    public virtual double width { get; private set; } = default!;
    public virtual double height { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<Offset> offsetAnimation { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _FixedSizeSlidingTransition__nav_bar(bool isLTR, global::Doroti.Generated.Framework.Animation.Animation<Offset> offsetAnimation, double width, double height, global::Doroti.Generated.Framework.Widgets.Widget child) : base(listenable: offsetAnimation)
    {
        this.isLTR = isLTR;
        this.offsetAnimation = offsetAnimation;
        this.width = width;
        this.height = height;
        this.child = child;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Positioned(top: ((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this.offsetAnimation).value.dy, left: (this.isLTR ? ((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this.offsetAnimation).value.dx : null), right: (this.isLTR ? null : ((global::Doroti.Generated.Framework.Animation.Animation<Offset>)this.offsetAnimation).value.dx), width: this.width, height: this.height, child: this.child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Generated.Framework.Widgets.Widget _wrapWithBackground(global::Doroti.Generated.Framework.Painting.Border? border = null, Color backgroundColor = default!, Brightness? brightness = null, global::Doroti.Generated.Framework.Widgets.Widget child = default!, bool updateSystemUiOverlay = true, bool enableBackgroundFilterBlur = true)
    {
        var result__7546 = child;
        if (updateSystemUiOverlay)
        {
            bool isDark__7608 = (backgroundColor.computeLuminance() < 0.179);
            global::Doroti.Ui.Brightness newBrightness__7682 = (brightness ?? ((isDark__7608 ? Brightness.dark : Brightness.light)));
            global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle overlayStyle__7790 = (newBrightness__7682 switch { Brightness.dark => global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle.light, Brightness.light => global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle.dark, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            result__7546 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnnotatedRegion<global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle>(value: new global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle(statusBarColor: ((global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle)overlayStyle__7790).statusBarColor, statusBarBrightness: ((global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle)overlayStyle__7790).statusBarBrightness, statusBarIconBrightness: ((global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle)overlayStyle__7790).statusBarIconBrightness, systemStatusBarContrastEnforced: ((global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle)overlayStyle__7790).systemStatusBarContrastEnforced), child: result__7546));
        }
        var childWithBackground__8776 = new global::Doroti.Generated.Framework.Widgets.DecoratedBox(decoration: new global::Doroti.Generated.Framework.Painting.BoxDecoration(border: border, color: backgroundColor), child: result__7546);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ClipRect(child: new global::Doroti.Generated.Framework.Widgets.BackdropFilter(enabled: ((backgroundColor.alpha != 255L) && enableBackgroundFilterBlur), filter: new global::Doroti.Ui.ImageFilter(sigmaX: 10.0, sigmaY: 10.0), child: childWithBackground__8776)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static double _dampScaleFactor(double scaledFontSize, double unscaledFontSize, double dampingRatio)
    {
        double scaleFactor__9249 = (scaledFontSize / unscaledFontSize);
        return ((scaleFactor__9249 < 1.0) ? Math.Max(Nav_barLibrary._kMinScaleFactor, scaleFactor__9249) : (1.0 + ((((scaleFactor__9249 - 1.0)) / dampingRatio))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static bool _isTransitionable(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        dynamic route__9579 = global::Doroti.Generated.Framework.Widgets.ModalRoute<object>.of<object>(context);
        return (((route__9579 is PageRoute<object>) && !((bool)((dynamic)route__9579).fullscreenDialog)) && !CupertinoSheetRoute<object>.hasParentSheet(context));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class CupertinoNavigationBar : global::Doroti.Generated.Framework.Widgets.StatefulWidget, ObstructingPreferredSizeWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? largeTitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual bool automaticallyImplyMiddle { get; private set; } = default!;
    public virtual string? previousPageTitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? middle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Border? border { get; private set; }
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget? bottom { get; private set; }

    public CupertinoNavigationBar(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyMiddle = true, string? previousPageTitle = null, global::Doroti.Generated.Framework.Widgets.Widget? middle = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, global::Doroti.Generated.Framework.Painting.Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget? bottom = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
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

    public static CupertinoNavigationBar CreateLarge(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? largeTitle = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, string? previousPageTitle = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, global::Doroti.Generated.Framework.Painting.Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget? bottom = null)
    {
        var __instance = new CupertinoNavigationBar(key: key, leading: leading, automaticallyImplyLeading: automaticallyImplyLeading, previousPageTitle: previousPageTitle, trailing: trailing, border: border, backgroundColor: backgroundColor, automaticBackgroundVisibility: automaticBackgroundVisibility, enableBackgroundFilterBlur: enableBackgroundFilterBlur, brightness: brightness, padding: padding, transitionBetweenRoutes: transitionBetweenRoutes, heroTag: heroTag, bottom: bottom);
        global::Doroti.Generated.Framework.Painting.Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
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

    public virtual bool shouldFullyObstruct(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color backgroundColor__25496 = ((global::Doroti.Ui.Color)(object?)(CupertinoDynamicColor.maybeResolve(this.backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor));
        return (backgroundColor__25496.alpha == 255L);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Size preferredSize
    {
        get
        {
            double bottomHeight__25749 = (this.bottom?.preferredSize.height ?? 0.0);
            double effectiveLargeHeight__25819 = ((this.largeTitle is not null) ? Nav_barLibrary._kNavBarLargeTitleHeightExtension : 0.0);
            return new global::Doroti.Ui.Size(((Nav_barLibrary._kNavBarPersistentHeight + bottomHeight__25749) + effectiveLargeHeight__25819));
            return default!;
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoNavigationBarState__nav_bar());
}

internal class _CupertinoNavigationBarState__nav_bar : global::Doroti.Generated.Framework.Widgets.State<CupertinoNavigationBar>
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.ScrollNotificationObserverState? _scrollNotificationObserver { get; set; } = default;
    internal virtual double _scrollAnimationValue { get; set; } = 0.0;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        this._scrollNotificationObserver?.removeListener((global::System.Action<global::Doroti.Generated.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
        _scrollNotificationObserver = ScrollNotificationObserver.maybeOf(this.context);
        this._scrollNotificationObserver?.addListener((global::System.Action<global::Doroti.Generated.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
    }

    public override void dispose()
    {
        if ((this._scrollNotificationObserver is not null))
        {
            this._scrollNotificationObserver!.removeListener((global::System.Action<global::Doroti.Generated.Framework.Widgets.ScrollNotification>)this._handleScrollNotification);
            _scrollNotificationObserver = null;
        }
        base.dispose();
    }

    public override void initState()
    {
        base.initState();
        keys = new _NavigationBarStaticComponentsKeys__nav_bar();
    }

    internal virtual void _handleScrollNotification(global::Doroti.Generated.Framework.Widgets.ScrollNotification notification)
    {
        if (((notification is global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification) && (((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification).depth == 0L)))
        {
            global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification notification__as27250 = (global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification;
            global::Doroti.Generated.Framework.Widgets.ScrollMetrics metrics__27347 = ((global::Doroti.Generated.Framework.Widgets.ScrollUpdateNotification)notification__as27250).metrics;
            double oldScrollAnimationValue__27398 = this._scrollAnimationValue;
            var scrollExtent__27457 = 0.0;
            switch (((global::Doroti.Generated.Framework.Widgets.ScrollMetrics)metrics__27347).axisDirection)
            {
                case global::Doroti.Generated.Framework.Painting.AxisDirection.up:
                    {
                        scrollExtent__27457 = ((global::Doroti.Generated.Framework.Widgets.ScrollMetrics)metrics__27347).extentAfter;
                        break;
                    }
                case global::Doroti.Generated.Framework.Painting.AxisDirection.down:
                    {
                        scrollExtent__27457 = ((global::Doroti.Generated.Framework.Widgets.ScrollMetrics)metrics__27347).extentBefore;
                        break;
                    }
                case global::Doroti.Generated.Framework.Painting.AxisDirection.right:
                case global::Doroti.Generated.Framework.Painting.AxisDirection.left:
                    {
                        break;
                    }
            }
            if (((scrollExtent__27457 >= 0L) && (scrollExtent__27457 < Nav_barLibrary._kNavBarScrollUnderAnimationExtent)))
            {
                setState(((global::System.Action)(() => {
_scrollAnimationValue = Dart_uiLibrary.clampDouble((scrollExtent__27457 / Nav_barLibrary._kNavBarScrollUnderAnimationExtent), 0, 1);
})));
            }
            else
            {
                if (((scrollExtent__27457 > Nav_barLibrary._kNavBarScrollUnderAnimationExtent) && (oldScrollAnimationValue__27398 != 1.0)))
                {
                    setState(((global::System.Action)(() => {
_scrollAnimationValue = 1.0;
})));
                }
                else
                {
                    if (((scrollExtent__27457 <= 0L) && (oldScrollAnimationValue__27398 != 0.0)))
                    {
                        setState(((global::System.Action)(() => {
_scrollAnimationValue = 0.0;
})));
                    }
                }
            }
        }
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() => ((((CupertinoNavigationBar)this.widget).middle is null) || (((CupertinoNavigationBar)this.widget).largeTitle is null)));
        global::Doroti.Ui.Color backgroundColor__28961 = ((global::Doroti.Ui.Color)(object?)(CupertinoDynamicColor.maybeResolve(((CupertinoNavigationBar)this.widget).backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor));
        global::Doroti.Ui.Color? parentPageScaffoldBackgroundColor__29131 = ((global::Doroti.Ui.Color?)(object?)CupertinoPageScaffoldBackgroundColor.maybeOf(context));
        global::Doroti.Generated.Framework.Painting.Border? initialBorder__29254 = ((((CupertinoNavigationBar)this.widget).automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor__29131 is not null)) ? Nav_barLibrary._kTransparentNavBarBorder : ((CupertinoNavigationBar)this.widget).border);
        global::Doroti.Generated.Framework.Painting.Border? effectiveBorder__29439 = ((((CupertinoNavigationBar)this.widget).border is null) ? null : Border.lerp(initialBorder__29254, ((CupertinoNavigationBar)this.widget).border, this._scrollAnimationValue));
        global::Doroti.Ui.Color effectiveBackgroundColor__29587 = ((global::Doroti.Ui.Color)(object?)((((CupertinoNavigationBar)this.widget).automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor__29131 is not null)) ? (Dart_uiLibrary.Color.lerp(parentPageScaffoldBackgroundColor__29131, backgroundColor__28961, this._scrollAnimationValue) ?? backgroundColor__28961) : backgroundColor__28961));
        double bottomHeight__29878 = (((CupertinoNavigationBar)this.widget).bottom?.preferredSize.height ?? 0.0);
        double persistentHeight__29954 = ((Nav_barLibrary._kNavBarPersistentHeight + bottomHeight__29878) + MediaQuery.paddingOf(context).top);
        double largeHeight__30075 = (persistentHeight__29954 + Nav_barLibrary._kNavBarLargeTitleHeightExtension);
        var components__30154 = new _NavigationBarStaticComponents__nav_bar(keys: this.keys, route: global::Doroti.Generated.Framework.Widgets.ModalRoute<object>.of<object>(context), userLeading: ((CupertinoNavigationBar)this.widget).leading, automaticallyImplyLeading: ((CupertinoNavigationBar)this.widget).automaticallyImplyLeading, automaticallyImplyTitle: ((CupertinoNavigationBar)this.widget).automaticallyImplyMiddle, previousPageTitle: ((CupertinoNavigationBar)this.widget).previousPageTitle, userMiddle: ((CupertinoNavigationBar)this.widget).middle, userTrailing: ((CupertinoNavigationBar)this.widget).trailing, padding: ((CupertinoNavigationBar)this.widget).padding, userLargeTitle: ((CupertinoNavigationBar)this.widget).largeTitle, userBottom: DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(((CupertinoNavigationBar)this.widget).bottom), large: (((CupertinoNavigationBar)this.widget).largeTitle is not null), staticBar: true, context: context);
        global::Doroti.Generated.Framework.Widgets.Widget navBar__30818 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _PersistentNavigationBar__nav_bar(components: components__30154, padding: ((CupertinoNavigationBar)this.widget).padding, middleVisible: (((CupertinoNavigationBar)this.widget).largeTitle is null)));
        if ((((CupertinoNavigationBar)this.widget).largeTitle is not null))
        {
            navBar__30818 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxHeight: largeHeight__30075), child: new global::Doroti.Generated.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection31165 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(navBar__30818)); __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Generated.Framework.Widgets.Semantics(header: true, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: new _LargeTitle__nav_bar(height: Nav_barLibrary._kNavBarLargeTitleHeightExtension, child: ((_NavigationBarStaticComponents__nav_bar)components__30154).largeTitle))))))); if ((((CupertinoNavigationBar)this.widget).bottom is not null)) { __collection31165.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: bottomHeight__29878, child: ((_NavigationBarStaticComponents__nav_bar)components__30154).navBarBottom))); } return __collection31165; }))())));
        }
        else
        {
            navBar__30818 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(maxHeight: persistentHeight__29954), child: new global::Doroti.Generated.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection32281 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection32281.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(navBar__30818)); if ((((CupertinoNavigationBar)this.widget).bottom is not null)) { __collection32281.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: bottomHeight__29878, child: ((_NavigationBarStaticComponents__nav_bar)components__30154).navBarBottom))); } return __collection32281; }))())));
        }
        navBar__30818 = Nav_barLibrary._wrapWithBackground(border: effectiveBorder__29439, backgroundColor: effectiveBackgroundColor__29587, brightness: ((CupertinoNavigationBar)this.widget).brightness, enableBackgroundFilterBlur: ((CupertinoNavigationBar)this.widget).enableBackgroundFilterBlur, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.textStyle, child: navBar__30818));
        if ((!((CupertinoNavigationBar)this.widget).transitionBetweenRoutes || !Nav_barLibrary._isTransitionable(context)))
        {
            return navBar__30818;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((context) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Hero(tag: ((object.Equals(((CupertinoNavigationBar)this.widget).heroTag, Nav_barLibrary._defaultHeroTag)) ? new _HeroTag__nav_bar(Navigator.of(context)) : ((CupertinoNavigationBar)this.widget).heroTag), createRectTween: (global::System.Func<Rect?, Rect?, global::Doroti.Generated.Framework.Animation.RectTween>)Nav_barLibrary._linearTranslateWithLargestRectSizeTween, placeholderBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, Size, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>)Nav_barLibrary._navBarHeroLaunchPadBuilder, flightShuttleBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.HeroFlightDirection, global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)Nav_barLibrary._navBarHeroFlightShuttleBuilder, transitionOnUserGestures: true, child: new _TransitionableNavigationBar__nav_bar(componentsKeys: this.keys, backgroundColor: effectiveBackgroundColor__29587, backButtonTextStyle: CupertinoTheme.of(context).textTheme.navActionTextStyle, titleTextStyle: CupertinoTheme.of(context).textTheme.navTitleTextStyle, largeTitleTextStyle: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, border: effectiveBorder__29439, hasUserMiddle: (((CupertinoNavigationBar)this.widget).middle is not null), largeExpanded: (((CupertinoNavigationBar)this.widget).largeTitle is not null), searchable: false, automaticBackgroundVisibility: ((CupertinoNavigationBar)this.widget).automaticBackgroundVisibility, child: navBar__30818)));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoSliverNavigationBar : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? largeTitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? leading { get; private set; }
    public virtual bool automaticallyImplyLeading { get; private set; } = default!;
    public virtual bool automaticallyImplyTitle { get; private set; } = default!;
    public virtual bool alwaysShowMiddle { get; private set; } = default!;
    public virtual string? previousPageTitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? middle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? trailing { get; private set; }
    public virtual Color? backgroundColor { get; private set; }
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Border? border { get; private set; }
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget? bottom { get; private set; }
    public virtual NavigationBarBottomMode? bottomMode { get; private set; }
    public virtual global::System.Action<bool>? onSearchableBottomTap { get; private set; }
    public virtual bool stretch { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? searchField { get; private set; }
    internal virtual bool _searchable { get; private set; } = default!;

    public CupertinoSliverNavigationBar(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget? largeTitle = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, bool alwaysShowMiddle = true, string? previousPageTitle = null, global::Doroti.Generated.Framework.Widgets.Widget? middle = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, global::Doroti.Generated.Framework.Painting.Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, bool stretch = false, global::Doroti.Generated.Framework.Widgets.PreferredSizeWidget? bottom = null, NavigationBarBottomMode? bottomMode = null) : base(key: key)
    {
        global::Doroti.Generated.Framework.Painting.Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
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

    public static CupertinoSliverNavigationBar CreateSearch(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.Widget searchField = default!, global::Doroti.Generated.Framework.Widgets.Widget? largeTitle = null, global::Doroti.Generated.Framework.Widgets.Widget? leading = null, bool automaticallyImplyLeading = true, bool automaticallyImplyTitle = true, bool alwaysShowMiddle = true, string? previousPageTitle = null, global::Doroti.Generated.Framework.Widgets.Widget? middle = null, global::Doroti.Generated.Framework.Widgets.Widget? trailing = null, global::Doroti.Generated.Framework.Painting.Border? border = default!, Color? backgroundColor = null, bool automaticBackgroundVisibility = true, bool enableBackgroundFilterBlur = true, Brightness? brightness = null, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding = null, bool transitionBetweenRoutes = true, object heroTag = default!, bool stretch = false, NavigationBarBottomMode? bottomMode = NavigationBarBottomMode.automatic, global::System.Action<bool>? onSearchableBottomTap = null)
    {
        var __instance = new CupertinoSliverNavigationBar(key: key, largeTitle: largeTitle, leading: leading, automaticallyImplyLeading: automaticallyImplyLeading, automaticallyImplyTitle: automaticallyImplyTitle, alwaysShowMiddle: alwaysShowMiddle, previousPageTitle: previousPageTitle, middle: middle, trailing: trailing, border: border, backgroundColor: backgroundColor, automaticBackgroundVisibility: automaticBackgroundVisibility, enableBackgroundFilterBlur: enableBackgroundFilterBlur, brightness: brightness, padding: padding, transitionBetweenRoutes: transitionBetweenRoutes, heroTag: heroTag, stretch: stretch, bottomMode: bottomMode);
        global::Doroti.Generated.Framework.Painting.Border? __border = border ?? Nav_barLibrary._kDefaultNavBarBorder;
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

internal class _CupertinoSliverNavigationBarState__nav_bar : global::Doroti.Generated.Framework.Widgets.State<CupertinoSliverNavigationBar>, global::Doroti.Generated.Framework.Widgets.TickerProviderStateMixin<CupertinoSliverNavigationBar>
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Widgets.ScrollableState? _scrollableState { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? effectiveMiddle { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Animation.AnimationController _animationController { get; set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Animation.CurvedAnimation _searchAnimation { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> persistentHeightAnimation { get; set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> largeTitleHeightAnimation { get; set; } = default!;
    public virtual double scaledSearchFieldHeight { get; set; } = default!;
    public virtual double scaledLargeTitleHeight { get; set; } = default!;
    public virtual bool searchIsActive { get; set; } = false;
    public virtual bool isPortrait { get; set; } = true;
    public virtual HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>? _tickers { get; set; } = default;
    public virtual global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>? _tickerModeNotifier { get; set; } = default;

    public override void initState()
    {
        base.initState();
        keys = new _NavigationBarStaticComponentsKeys__nav_bar();
        _animationController = new global::Doroti.Generated.Framework.Animation.AnimationController(vsync: this, duration: Nav_barLibrary._kNavBarSearchDuration);
        _searchAnimation = new global::Doroti.Generated.Framework.Animation.CurvedAnimation(parent: this._animationController, curve: Nav_barLibrary._kNavBarSearchCurve);
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
        isPortrait = (object.Equals(MediaQuery.orientationOf(this.context), global::Doroti.Generated.Framework.Widgets.Orientation.portrait));
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
                    foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18989 in this._tickers!)
                    {
                        if (((global::Doroti.Generated.Framework.Scheduler.Ticker)ticker__18989).isActive)
                        {
                            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"{this} was disposed with an active Ticker."), new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{this.GetType()} created a Ticker via its TickerProviderStateMixin, but at the time " + "dispose() was called on the mixin, that Ticker was still active. All Tickers must " + "be disposed before calling super.dispose()."), new global::Doroti.Generated.Framework.Foundation.ErrorHint("Tickers used by AnimationControllers " + "should be disposed by calling dispose() on the AnimationController itself. " + "Otherwise, the ticker will leak."), ticker__18989.describeForError("The offending ticker was") }));
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
        global::Doroti.Generated.Framework.Painting.TextScaler textScaler__49612 = ((global::Doroti.Generated.Framework.Painting.TextScaler)(object?)MediaQuery.textScalerOf(this.context));
        scaledSearchFieldHeight = (Nav_barLibrary._kSearchFieldHeight * Nav_barLibrary._dampScaleFactor(textScaler__49612.scale(Nav_barLibrary._kSearchFieldHeight), Nav_barLibrary._kSearchFieldHeight, Nav_barLibrary._kMaxScaleFactor));
        scaledLargeTitleHeight = (this.isPortrait ? (Nav_barLibrary._kNavBarLargeTitleHeightExtension * Nav_barLibrary._dampScaleFactor(textScaler__49612.scale(Nav_barLibrary._kNavBarLargeTitleHeightExtension), Nav_barLibrary._kNavBarLargeTitleHeightExtension, Nav_barLibrary._kLargeTitleScaleDampingRatio)) : 0.0);
    }

    internal virtual void _setupSearchableAnimation()
    {
        var persistentHeightTween__50232 = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: Nav_barLibrary._kNavBarPersistentHeight, end: 0.0);
        persistentHeightAnimation = ((Func<global::Doroti.Generated.Framework.Animation.Animation<double>>)(() =>
{            var __cascade = persistentHeightTween__50232.animate(this._animationController);
            __cascade.addStatusListener((AnimationStatusListener)this._handleSearchFieldStatusChanged);
            return __cascade;        }))();
        var largeTitleHeightTween__50468 = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: this.scaledLargeTitleHeight, end: 0.0);
        largeTitleHeightAnimation = largeTitleHeightTween__50468.animate(this._animationController);
    }

    internal virtual void _handleScrollChange()
    {
        global::Doroti.Generated.Framework.Widgets.ScrollPosition? position__50695 = this._scrollableState?.position;
        if ((((position__50695 is null) || !((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__50695).hasPixels) || (((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__50695).pixels <= 0.0)))
        {
            return;
        }
        double? target__50844 = default!;
        double bottomScrollOffset__50869 = ((object.Equals(((CupertinoSliverNavigationBar)this.widget).bottomMode, NavigationBarBottomMode.always)) ? 0.0 : this._bottomHeight);
        bool canScrollBottom__50996 = (((((CupertinoSliverNavigationBar)this.widget)._searchable || (((CupertinoSliverNavigationBar)this.widget).bottom is not null))) && (bottomScrollOffset__50869 > 0.0));
        if ((canScrollBottom__50996 && (((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__50695).pixels < bottomScrollOffset__50869)))
        {
            target__50844 = ((((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__50695).pixels > (bottomScrollOffset__50869 / 2L)) ? bottomScrollOffset__50869 : 0.0);
        }
        else
        {
            if (((((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__50695).pixels > bottomScrollOffset__50869) && (((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__50695).pixels < (bottomScrollOffset__50869 + this.scaledLargeTitleHeight))))
            {
                target__50844 = ((((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__50695).pixels > (bottomScrollOffset__50869 + ((this.scaledLargeTitleHeight / 2L)))) ? (bottomScrollOffset__50869 + this.scaledLargeTitleHeight) : bottomScrollOffset__50869);
            }
        }
        if (((target__50844 is not null) && (target__50844 <= ((global::Doroti.Generated.Framework.Widgets.ScrollPosition)position__50695).maxScrollExtent)))
        {
            double target__50844__value51736 = DartRuntimePrimitives.RequireValue(target__50844);
            DartRuntimePrimitives.Ignore(position__50695.animateTo(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(target__50844__value51736)), duration: Duration.Create(milliseconds: 300L), curve: global::Doroti.Generated.Framework.Animation.Curves.fastEaseInToSlowEaseOut));
        }
    }

    internal virtual void _handleSearchFieldStatusChanged(global::Doroti.Generated.Framework.Animation.AnimationStatus status)
    {
        setState(((global::System.Action)(() => {
switch (status)
{
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.forward:
        {
            searchIsActive = true;
            break;
        }
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.reverse:
        {
            searchIsActive = false;
            break;
        }
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.completed:
    case global::Doroti.Generated.Framework.Animation.AnimationStatus.dismissed:
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

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var components__52880 = new _NavigationBarStaticComponents__nav_bar(keys: this.keys, route: global::Doroti.Generated.Framework.Widgets.ModalRoute<object>.of<object>(context), userLeading: ((((CupertinoSliverNavigationBar)this.widget).leading is not null) ? new global::Doroti.Generated.Framework.Widgets.Visibility(visible: !this.searchIsActive, child: ((CupertinoSliverNavigationBar)this.widget).leading!) : null), automaticallyImplyLeading: ((CupertinoSliverNavigationBar)this.widget).automaticallyImplyLeading, automaticallyImplyTitle: ((CupertinoSliverNavigationBar)this.widget).automaticallyImplyTitle, previousPageTitle: ((CupertinoSliverNavigationBar)this.widget).previousPageTitle, userMiddle: (((global::Doroti.Generated.Framework.Animation.AnimationController)this._animationController).isAnimating ? new global::Doroti.Generated.Framework.Widgets.Text("") : this.effectiveMiddle), userTrailing: ((((CupertinoSliverNavigationBar)this.widget).trailing is not null) ? new global::Doroti.Generated.Framework.Widgets.Visibility(visible: !this.searchIsActive, child: ((CupertinoSliverNavigationBar)this.widget).trailing!) : null), userLargeTitle: ((CupertinoSliverNavigationBar)this.widget).largeTitle, userBottom: (((((CupertinoSliverNavigationBar)this.widget)._searchable ? (this.searchIsActive ? new _ActiveSearchableBottom__nav_bar(animationController: this._animationController, animation: this.persistentHeightAnimation, searchField: ((CupertinoSliverNavigationBar)this.widget).searchField, searchFieldHeight: this.scaledSearchFieldHeight, onSearchFieldTap: () => this._onSearchFieldTap()) : new _InactiveSearchableBottom__nav_bar(animationController: this._animationController, animation: this.persistentHeightAnimation, searchField: ((CupertinoSliverNavigationBar)this.widget).searchField, searchFieldHeight: this.scaledSearchFieldHeight, onSearchFieldTap: () => this._onSearchFieldTap())) : (global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((CupertinoSliverNavigationBar)this.widget).bottom)) ?? global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink()), padding: ((CupertinoSliverNavigationBar)this.widget).padding, large: this.isPortrait, staticBar: false, context: context);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this._searchAnimation, builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SliverPersistentHeader(pinned: true, @delegate: new _LargeTitleNavigationBarSliverDelegate__nav_bar(keys: this.keys, components: components__52880, userMiddle: this.effectiveMiddle, backgroundColor: (CupertinoDynamicColor.maybeResolve(((CupertinoSliverNavigationBar)this.widget).backgroundColor, context) ?? CupertinoTheme.of(context).barBackgroundColor), automaticBackgroundVisibility: ((CupertinoSliverNavigationBar)this.widget).automaticBackgroundVisibility, brightness: ((CupertinoSliverNavigationBar)this.widget).brightness, border: ((CupertinoSliverNavigationBar)this.widget).border, padding: ((CupertinoSliverNavigationBar)this.widget).padding, actionsForegroundColor: CupertinoTheme.of(context).primaryColor, transitionBetweenRoutes: ((CupertinoSliverNavigationBar)this.widget).transitionBetweenRoutes, heroTag: ((CupertinoSliverNavigationBar)this.widget).heroTag, persistentHeight: (((global::Doroti.Generated.Framework.Animation.Animation<double>)this.persistentHeightAnimation).value + MediaQuery.paddingOf(context).top), largeTitleHeight: ((global::Doroti.Generated.Framework.Animation.Animation<double>)this.largeTitleHeightAnimation).value, alwaysShowMiddle: (((CupertinoSliverNavigationBar)this.widget).alwaysShowMiddle && (this.effectiveMiddle is not null)), stretchConfiguration: ((((CupertinoSliverNavigationBar)this.widget).stretch && !this.searchIsActive) ? new global::Doroti.Generated.Framework.Rendering.OverScrollHeaderStretchConfiguration() : null), enableBackgroundFilterBlur: ((CupertinoSliverNavigationBar)this.widget).enableBackgroundFilterBlur, bottomMode: (this.searchIsActive ? NavigationBarBottomMode.always : (((CupertinoSliverNavigationBar)this.widget).bottomMode ?? NavigationBarBottomMode.automatic)), bottomHeight: this._bottomHeight, controller: this._animationController, searchable: ((CupertinoSliverNavigationBar)this.widget)._searchable)));
throw new InvalidOperationException("Dart closure completed without a value.");
})))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Scheduler.Ticker createTicker(global::System.Action<Duration> onTick)
    {
        if ((this._tickerModeNotifier is null))
        {
            _updateTickerModeNotifier();
        }
        DartRuntimePrimitives.Assert(() => (this._tickerModeNotifier is not null));
        this._tickers ??= new HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>();
        TickerModeData values__17506 = this._tickerModeNotifier!.value;
        var result__17553 = ((Func<global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider>)(() =>
{            var __cascade = new _WidgetTicker__ticker_provider((global::System.Action<Duration>)onTick, this, debugLabel: (global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode ? $"created by {(global::Doroti.Generated.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this))}" : null));
            __cascade.muted = !((TickerModeData)values__17506).enabled;
            __cascade.forceFrames = ((TickerModeData)values__17506).forceFrames;
            return __cascade;        }))();
        this._tickers!.Add(result__17553);
        return ((global::Doroti.Generated.Framework.Scheduler.Ticker)(object?)result__17553);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _removeTicker(global::Doroti.Generated.Framework.Widgets._WidgetTicker__ticker_provider ticker)
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
            TickerModeData values__18318 = this._tickerModeNotifier!.value;
            bool muted__18372 = !((TickerModeData)values__18318).enabled;
            foreach (global::Doroti.Generated.Framework.Scheduler.Ticker ticker__18421 in this._tickers!)
            {
                ticker__18421.muted = muted__18372;
                ticker__18421.forceFrames = ((TickerModeData)values__18318).forceFrames;
            }
        }
    }

    public virtual void _updateTickerModeNotifier()
    {
        global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData> newNotifier__18621 = ((global::Doroti.Generated.Framework.Foundation.ValueListenable<TickerModeData>)(object?)TickerMode.getValuesNotifier(this.context));
        if ((object.Equals(newNotifier__18621, this._tickerModeNotifier)))
        {
            return;
        }
        this._tickerModeNotifier?.removeListener(() => this._updateTickers());
        newNotifier__18621.addListener(() => this._updateTickers());
        this._tickerModeNotifier = newNotifier__18621;
    }

    public override void debugFillProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<HashSet<global::Doroti.Generated.Framework.Scheduler.Ticker>>("tickers", this._tickers, description: ((this._tickers is not null) ? $"tracking {checked((long)(this._tickers!.Count))} ticker{((checked((long)(this._tickers!.Count)) == 1L) ? "" : "s")}" : null), defaultValue: default));
    }

}

internal class _LargeTitleNavigationBarSliverDelegate__nav_bar : global::Doroti.Generated.Framework.Widgets.SliverPersistentHeaderDelegate
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar keys { get; private set; } = default!;
    public virtual _NavigationBarStaticComponents__nav_bar components { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? userMiddle { get; private set; }
    public virtual Color backgroundColor { get; private set; } = default!;
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual Brightness? brightness { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Border? border { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding { get; private set; }
    public virtual Color actionsForegroundColor { get; private set; } = default!;
    public virtual bool transitionBetweenRoutes { get; private set; } = default!;
    public virtual object heroTag { get; private set; } = default!;
    public virtual double persistentHeight { get; private set; } = default!;
    public virtual double largeTitleHeight { get; private set; } = default!;
    public virtual bool alwaysShowMiddle { get; private set; } = default!;
    public virtual bool enableBackgroundFilterBlur { get; private set; } = default!;
    public virtual NavigationBarBottomMode bottomMode { get; private set; } = default!;
    public virtual double bottomHeight { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController controller { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    private global::Doroti.Generated.Framework.Rendering.OverScrollHeaderStretchConfiguration? __field_stretchConfiguration = default;
    public override global::Doroti.Generated.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration => __field_stretchConfiguration;

    internal _LargeTitleNavigationBarSliverDelegate__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar keys, _NavigationBarStaticComponents__nav_bar components, global::Doroti.Generated.Framework.Widgets.Widget? userMiddle, Color backgroundColor, bool automaticBackgroundVisibility, Brightness? brightness, global::Doroti.Generated.Framework.Painting.Border? border, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding, Color actionsForegroundColor, bool transitionBetweenRoutes, object heroTag, double persistentHeight, double largeTitleHeight, bool alwaysShowMiddle, global::Doroti.Generated.Framework.Rendering.OverScrollHeaderStretchConfiguration? stretchConfiguration, bool enableBackgroundFilterBlur, NavigationBarBottomMode bottomMode, double bottomHeight, global::Doroti.Generated.Framework.Animation.AnimationController controller, bool searchable)
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
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context, double shrinkOffset, bool overlapsContent)
    {
        double largeTitleThreshold__58417 = ((this.maxExtent - this.minExtent) - Nav_barLibrary._kNavBarShowLargeTitleThreshold);
        bool showLargeTitle__58511 = (shrinkOffset < largeTitleThreshold__58417);
        double bottomShrinkFactor__58634 = Dart_uiLibrary.clampDouble((shrinkOffset / this.bottomHeight), 0, 1);
        double shrinkAnimationValue__58721 = Dart_uiLibrary.clampDouble(((((shrinkOffset - largeTitleThreshold__58417) - Nav_barLibrary._kNavBarScrollUnderAnimationExtent)) / Nav_barLibrary._kNavBarScrollUnderAnimationExtent), 0, 1);
        var persistentNavigationBar__58921 = new _PersistentNavigationBar__nav_bar(components: this.components, padding: this.padding, middleVisible: (this.alwaysShowMiddle ? null : !showLargeTitle__58511));
        global::Doroti.Ui.Color? parentPageScaffoldBackgroundColor__59233 = ((global::Doroti.Ui.Color?)(object?)CupertinoPageScaffoldBackgroundColor.maybeOf(context));
        global::Doroti.Generated.Framework.Painting.Border? initialBorder__59356 = ((this.automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor__59233 is not null)) ? Nav_barLibrary._kTransparentNavBarBorder : this.border);
        global::Doroti.Generated.Framework.Painting.Border? effectiveBorder__59527 = ((this.border is null) ? null : Border.lerp(initialBorder__59356, this.border, shrinkAnimationValue__58721));
        global::Doroti.Ui.Color effectiveBackgroundColor__59660 = ((global::Doroti.Ui.Color)(object?)((this.automaticBackgroundVisibility && (parentPageScaffoldBackgroundColor__59233 is not null)) ? (Dart_uiLibrary.Color.lerp(parentPageScaffoldBackgroundColor__59233, this.backgroundColor, shrinkAnimationValue__58721) ?? this.backgroundColor) : this.backgroundColor));
        global::Doroti.Generated.Framework.Widgets.Widget navBar__59943 = Nav_barLibrary._wrapWithBackground(border: effectiveBorder__59527, backgroundColor: effectiveBackgroundColor__59660, brightness: this.brightness, enableBackgroundFilterBlur: this.enableBackgroundFilterBlur, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.textStyle, child: new global::Doroti.Generated.Framework.Widgets.Column(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection60282 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection60282.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.Stack(children: ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection60368 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(top: this.persistentHeight, left: 0.0, right: 0.0, bottom: ((object.Equals(this.bottomMode, NavigationBarBottomMode.automatic)) ? (this.bottomHeight * ((1.0 - bottomShrinkFactor__58634))) : 0.0), child: new global::Doroti.Generated.Framework.Widgets.ClipRect(child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Generated.Framework.Widgets.SafeArea(top: false, bottom: false, child: new global::Doroti.Generated.Framework.Widgets.AnimatedOpacity(opacity: ((showLargeTitle__58511 && !this.controller.isForwardOrCompleted) ? 1.0 : 0.0), duration: Nav_barLibrary._kNavBarTitleFadeDuration, child: new global::Doroti.Generated.Framework.Widgets.Semantics(header: true, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: new _LargeTitle__nav_bar(height: this.largeTitleHeight, child: ((_NavigationBarStaticComponents__nav_bar)this.components).largeTitle)))))))))); __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(left: 0.0, right: 0.0, top: 0.0, child: persistentNavigationBar__58921))); if ((object.Equals(this.bottomMode, NavigationBarBottomMode.automatic))) { __collection60368.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Positioned(left: 0.0, right: 0.0, bottom: 0.0, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (this.bottomHeight * ((1.0 - bottomShrinkFactor__58634))), child: new global::Doroti.Generated.Framework.Widgets.ClipRect(child: ((_NavigationBarStaticComponents__nav_bar)this.components).navBarBottom))))); } return __collection60368; }))())))); if ((object.Equals(this.bottomMode, NavigationBarBottomMode.always))) { __collection60282.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(height: this.bottomHeight, child: ((_NavigationBarStaticComponents__nav_bar)this.components).navBarBottom))); } return __collection60282; }))())));
        if ((!this.transitionBetweenRoutes || !Nav_barLibrary._isTransitionable(context)))
        {
            return navBar__59943;
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Hero(tag: ((object.Equals(this.heroTag, Nav_barLibrary._defaultHeroTag)) ? new _HeroTag__nav_bar(Navigator.of(context)) : this.heroTag), createRectTween: (global::System.Func<Rect?, Rect?, global::Doroti.Generated.Framework.Animation.RectTween>)Nav_barLibrary._linearTranslateWithLargestRectSizeTween, flightShuttleBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Animation.Animation<double>, global::Doroti.Generated.Framework.Widgets.HeroFlightDirection, global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)Nav_barLibrary._navBarHeroFlightShuttleBuilder, placeholderBuilder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, Size, global::Doroti.Generated.Framework.Widgets.Widget, global::Doroti.Generated.Framework.Widgets.Widget>)Nav_barLibrary._navBarHeroLaunchPadBuilder, transitionOnUserGestures: true, child: new _TransitionableNavigationBar__nav_bar(componentsKeys: this.keys, backgroundColor: effectiveBackgroundColor__59660, backButtonTextStyle: CupertinoTheme.of(context).textTheme.navActionTextStyle, titleTextStyle: CupertinoTheme.of(context).textTheme.navTitleTextStyle, largeTitleTextStyle: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle, border: effectiveBorder__59527, hasUserMiddle: ((this.userMiddle is not null) && ((this.alwaysShowMiddle || !showLargeTitle__58511))), largeExpanded: showLargeTitle__58511, searchable: this.searchable, automaticBackgroundVisibility: this.automaticBackgroundVisibility, child: navBar__59943)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override bool shouldRebuild(global::Doroti.Generated.Framework.Widgets.SliverPersistentHeaderDelegate oldDelegate)
    {
        var __oldDelegate = (_LargeTitleNavigationBarSliverDelegate__nav_bar)(object)oldDelegate;
        return (((((((((((((((((!object.Equals(this.components, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).components)) || (!object.Equals(this.userMiddle, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).userMiddle))) || (!object.Equals(this.backgroundColor, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).backgroundColor))) || (this.automaticBackgroundVisibility != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).automaticBackgroundVisibility)) || (!object.Equals(this.border, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).border))) || (!object.Equals(this.padding, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).padding))) || (!object.Equals(this.actionsForegroundColor, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).actionsForegroundColor))) || (this.transitionBetweenRoutes != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).transitionBetweenRoutes)) || (this.persistentHeight != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).persistentHeight)) || (this.largeTitleHeight != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).largeTitleHeight)) || (this.alwaysShowMiddle != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).alwaysShowMiddle)) || (!object.Equals(this.heroTag, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).heroTag))) || (this.enableBackgroundFilterBlur != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).enableBackgroundFilterBlur)) || (!object.Equals(this.bottomMode, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).bottomMode))) || (this.bottomHeight != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).bottomHeight)) || (!object.Equals(this.controller, ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).controller))) || (this.searchable != ((_LargeTitleNavigationBarSliverDelegate__nav_bar)__oldDelegate).searchable));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _LargeTitle__nav_bar : global::Doroti.Generated.Framework.Widgets.SingleChildRenderObjectWidget
{
    public virtual double height { get; private set; } = default!;

    internal _LargeTitle__nav_bar(global::Doroti.Generated.Framework.Widgets.Widget? child = null, double height = default!) : base(child: child)
    {
        this.height = height;
    }

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderLargeTitle__nav_bar(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.bottomStart.resolve(Directionality.of(context)), height: this.height));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderLargeTitle__nav_bar)(object)renderObject;
        DartRuntimePrimitives.Ignore(((Func<_RenderLargeTitle__nav_bar>)(() =>
{            var __cascade = __renderObject;
            __cascade.alignment = global::Doroti.Generated.Framework.Painting.AlignmentDirectional.bottomStart.resolve(Directionality.of(context));
            __cascade.height = this.height;
            return __cascade;        }))());
    }

}

public class _RenderLargeTitle__nav_bar : global::Doroti.Generated.Framework.Rendering.RenderShiftedBox
{
    internal virtual global::Doroti.Generated.Framework.Painting.Alignment _alignment { get; set; } = default!;
    internal virtual double _height { get; set; } = default!;
    internal virtual double _scale { get; set; } = 1.0;

    internal _RenderLargeTitle__nav_bar(global::Doroti.Generated.Framework.Painting.Alignment alignment, double height) : base(null)
    {
        this._alignment = alignment;
        this._height = height;
    }

    public virtual global::Doroti.Generated.Framework.Painting.Alignment alignment
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
    internal static double _computeTitleScale(Size childSize, global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, double height)
    {
        double maxHeight__66796 = (height - Nav_barLibrary._kNavBarBottomPadding);
        double scale__66857 = (1.0 + ((0.03 * ((((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxHeight - maxHeight__66796))) / maxHeight__66796));
        double maxScale__66944 = ((childSize.width != 0.0) ? Dart_uiLibrary.clampDouble((((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth / childSize.width), 1.0, 1.1) : 1.1);
        return Dart_uiLibrary.clampDouble(scale__66857, 1.0, maxScale__66944);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDistanceToActualBaseline(TextBaseline baseline)
    {
        double? distance__67213 = this.child?.getDistanceToActualBaseline(baseline);
        if ((distance__67213 is null))
        {
            return null;
        }
        var childParentData__67333 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)this.child!.parentData!)!;
        return (((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__67333).offset.dy + (DartRuntimePrimitives.RequireValue(distance__67213) * this._scale));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override double? computeDryBaseline(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints, TextBaseline baseline)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__67576 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__67576 is null))
        {
            return null;
        }
        global::Doroti.Generated.Framework.Rendering.BoxConstraints childConstraints__67671 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)constraints.widthConstraints().loosen());
        double? result__67749 = child__67576.getDryBaseline(childConstraints__67671, baseline);
        if ((result__67749 is null))
        {
            return null;
        }
        global::Doroti.Ui.Size childSize__67874 = ((global::Doroti.Ui.Size)(object?)child__67576.getDryLayout(childConstraints__67671));
        double scale__67941 = _RenderLargeTitle__nav_bar._computeTitleScale(childSize__67874, constraints, this.height);
        global::Doroti.Ui.Size scaledChildSize__68016 = ((global::Doroti.Ui.Size)(object?)(childSize__67874 * scale__67941));
        return ((DartRuntimePrimitives.RequireValue(result__67749) * scale__67941) + this.alignment.alongOffset((((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).biggest - scaledChildSize__68016)).dy);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void performLayout()
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__68227 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        size = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)this.constraints).biggest;
        if ((child__68227 is null))
        {
            return;
        }
        global::Doroti.Generated.Framework.Rendering.BoxConstraints childConstraints__68351 = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)(object?)this.constraints.widthConstraints().loosen());
        child__68227.layout(childConstraints__68351, parentUsesSize: true);
        _scale = _RenderLargeTitle__nav_bar._computeTitleScale(((global::Doroti.Generated.Framework.Rendering.RenderBox)child__68227).size, this.constraints, this.height);
        var childParentData__68545 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child__68227.parentData!)!;
        childParentData__68545.offset = this.alignment.alongOffset((this.size - ((((global::Doroti.Generated.Framework.Rendering.RenderBox)child__68227).size * this._scale))));
    }

    public override void applyPaintTransform(global::Doroti.Generated.Framework.Rendering.RenderObject child, Matrix4 transform)
    {
        var __child = (global::Doroti.Generated.Framework.Rendering.RenderBox)(object)child;
        DartRuntimePrimitives.Assert(() => (object.Equals(__child, ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child))));
        base.applyPaintTransform(__child, transform);
        transform.scaleByDouble(this._scale, this._scale, this._scale, 1);
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__69006 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__69006 is null))
        {
            layer = null;
        }
        else
        {
            var childParentData__69097 = ((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child__69006.parentData!)!;
            layer = context.pushTransform(this.needsCompositing, (offset + ((global::Doroti.Generated.Framework.Rendering.BoxParentData)childParentData__69097).offset), Matrix4.diagonal3Values(this._scale, this._scale, 1.0), ((global::System.Action<global::Doroti.Generated.Framework.Rendering.PaintingContext, Offset>)((context, offset) => { context.paintChild(child__69006, offset); })), oldLayer: ((global::Doroti.Generated.Framework.Rendering.TransformLayer?)(object?)this.layer)!);
        }
    }

    public override bool hitTestChildren(global::Doroti.Generated.Framework.Rendering.BoxHitTestResult result, Offset position)
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox? child__69572 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)this).child);
        if ((child__69572 is null))
        {
            return false;
        }
        global::Doroti.Ui.Offset childOffset__69662 = ((global::Doroti.Ui.Offset)(object?)(((global::Doroti.Generated.Framework.Rendering.BoxParentData?)(object?)child__69572.parentData!)!).offset);
        var transform__69732 = ((Func<Matrix4>)(() =>
{            var __cascade = Matrix4.identity();
            __cascade.scaleByDouble((1.0 / this._scale), (1.0 / this._scale), 1.0, 1);
            __cascade.translateByDouble(-childOffset__69662.dx, -childOffset__69662.dy, 0, 1);
            return __cascade;        }))();
        return result.addWithRawTransform(transform: transform__69732, position: position, hitTest: ((global::System.Func<global::Doroti.Generated.Framework.Rendering.BoxHitTestResult, Offset, bool>)((result, transformed) => {
return child__69572.hitTest(result, position: transformed);
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _PersistentNavigationBar__nav_bar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual _NavigationBarStaticComponents__nav_bar components { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding { get; private set; }
    public virtual bool? middleVisible { get; private set; }

    internal _PersistentNavigationBar__nav_bar(_NavigationBarStaticComponents__nav_bar components, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding = null, bool? middleVisible = null)
    {
        this.components = components;
        this.padding = padding;
        this.middleVisible = middleVisible;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget? middle__70847 = ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((_NavigationBarStaticComponents__nav_bar)this.components).middle);
        if ((middle__70847 is not null))
        {
            middle__70847 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: CupertinoTheme.of(context).textTheme.navTitleTextStyle, child: new global::Doroti.Generated.Framework.Widgets.Semantics(header: true, child: middle__70847)));
            middle__70847 = ((this.middleVisible is null) ? middle__70847 : new global::Doroti.Generated.Framework.Widgets.AnimatedOpacity(opacity: (DartRuntimePrimitives.RequireValue(this.middleVisible) ? 1.0 : 0.0), duration: Nav_barLibrary._kNavBarTitleFadeDuration, child: middle__70847));
        }
        global::Doroti.Generated.Framework.Widgets.Widget? leading__71448 = ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((_NavigationBarStaticComponents__nav_bar)this.components).leading);
        global::Doroti.Generated.Framework.Widgets.Widget? backChevron__71496 = ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((_NavigationBarStaticComponents__nav_bar)this.components).backChevron);
        global::Doroti.Generated.Framework.Widgets.Widget? backLabel__71552 = ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)((_NavigationBarStaticComponents__nav_bar)this.components).backLabel);
        if (((((leading__71448 is null) && (backChevron__71496 is not null)) && (backLabel__71552 is not null)) && !CupertinoSheetRoute<object>.hasParentSheet(context)))
        {
            leading__71448 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(CupertinoNavigationBarBackButton.Create_assemble(backChevron__71496, backLabel__71552));
        }
        else
        {
            leading__71448 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Align(widthFactor: 1.0, child: leading__71448));
        }
        global::Doroti.Generated.Framework.Widgets.Widget paddedToolbar__71902 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.NavigationToolbar(leading: leading__71448, middle: middle__70847, trailing: ((_NavigationBarStaticComponents__nav_bar)this.components).trailing, middleSpacing: 6.0));
        if ((this.padding is not null))
        {
            paddedToolbar__71902 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(top: this.padding!.top, bottom: this.padding!.bottom), child: paddedToolbar__71902));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (Nav_barLibrary._kNavBarPersistentHeight + MediaQuery.paddingOf(context).top), child: new global::Doroti.Generated.Framework.Widgets.SafeArea(top: !CupertinoSheetRoute<object>.hasParentSheet(context), bottom: false, child: paddedToolbar__71902)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _NavigationBarStaticComponentsKeys__nav_bar
{
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> navBarBoxKey { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> leadingKey { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> backChevronKey { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> backLabelKey { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> middleKey { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> trailingKey { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> largeTitleKey { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> navBarBottomKey { get; private set; } = default!;

    internal _NavigationBarStaticComponentsKeys__nav_bar()
    {
        this.navBarBoxKey = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Navigation bar render box");
        this.leadingKey = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Leading");
        this.backChevronKey = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Back chevron");
        this.backLabelKey = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Back label");
        this.middleKey = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Middle");
        this.trailingKey = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Trailing");
        this.largeTitleKey = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Large title");
        this.navBarBottomKey = global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>.Create(debugLabel: "Navigation bar bottom");
    }

}

public class _NavigationBarStaticComponents__nav_bar
{
    public virtual global::Doroti.Generated.Framework.Widgets.KeyedSubtree? leading { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.KeyedSubtree? backChevron { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.KeyedSubtree? backLabel { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.KeyedSubtree? middle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.KeyedSubtree? trailing { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.KeyedSubtree? largeTitle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.KeyedSubtree? navBarBottom { get; private set; }

    internal _NavigationBarStaticComponents__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar keys, dynamic route, global::Doroti.Generated.Framework.Widgets.Widget? userLeading, bool automaticallyImplyLeading, bool automaticallyImplyTitle, string? previousPageTitle, global::Doroti.Generated.Framework.Widgets.Widget? userMiddle, global::Doroti.Generated.Framework.Widgets.Widget? userTrailing, global::Doroti.Generated.Framework.Widgets.Widget? userLargeTitle, global::Doroti.Generated.Framework.Widgets.Widget? userBottom, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding, bool large, bool staticBar, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        this.leading = _NavigationBarStaticComponents__nav_bar.createLeading(leadingKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).leadingKey, userLeading: userLeading, route: route, automaticallyImplyLeading: automaticallyImplyLeading, padding: padding, context: context);
        this.backChevron = _NavigationBarStaticComponents__nav_bar.createBackChevron(backChevronKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).backChevronKey, userLeading: userLeading, route: route, automaticallyImplyLeading: automaticallyImplyLeading, context: context);
        this.backLabel = _NavigationBarStaticComponents__nav_bar.createBackLabel(backLabelKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).backLabelKey, userLeading: userLeading, route: route, previousPageTitle: previousPageTitle, automaticallyImplyLeading: automaticallyImplyLeading, context: context);
        this.middle = _NavigationBarStaticComponents__nav_bar.createMiddle(middleKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).middleKey, userMiddle: userMiddle, userLargeTitle: userLargeTitle, route: route, automaticallyImplyTitle: automaticallyImplyTitle, large: large, staticBar: staticBar, context: context);
        this.trailing = _NavigationBarStaticComponents__nav_bar.createTrailing(trailingKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).trailingKey, userTrailing: userTrailing, padding: padding, context: context);
        this.largeTitle = _NavigationBarStaticComponents__nav_bar.createLargeTitle(largeTitleKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).largeTitleKey, userLargeTitle: userLargeTitle, route: route, automaticImplyTitle: automaticallyImplyTitle, large: large, context: context);
        this.navBarBottom = _NavigationBarStaticComponents__nav_bar.createNavBarBottom(navBarBottomKey: ((_NavigationBarStaticComponentsKeys__nav_bar)keys).navBarBottomKey, userBottom: userBottom, context: context);
    }

    internal static global::Doroti.Generated.Framework.Widgets.Widget? _derivedTitle(bool automaticallyImplyTitle, dynamic currentRoute = null)
    {
        if (((automaticallyImplyTitle && (currentRoute is CupertinoRouteTransitionMixin<object>)) && (((CupertinoRouteTransitionMixin<object>)currentRoute).title is not null)))
        {
            CupertinoRouteTransitionMixin<object> currentRoute__as76488 = (CupertinoRouteTransitionMixin<object>)currentRoute;
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.Text(((CupertinoRouteTransitionMixin<object>)currentRoute__as76488).title!));
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.KeyedSubtree? createLeading(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> leadingKey, global::Doroti.Generated.Framework.Widgets.Widget? userLeading, dynamic route, bool automaticallyImplyLeading, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget? leadingContent__76968 = default!;
        if ((userLeading is not null))
        {
            leadingContent__76968 = userLeading;
        }
        else
        {
            if ((((automaticallyImplyLeading && (route is PageRoute<object>)) && ((bool)((dynamic)route).canPop)) && ((bool)((dynamic)route).fullscreenDialog)))
            {
                dynamic route__as77104 = (dynamic)route;
                leadingContent__76968 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new CupertinoButton(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, onPressed: (() => {
DartRuntimePrimitives.Ignore(((global::Doroti.Generated.Framework.Widgets.NavigatorState?)((dynamic)route__as77104).navigator)!.maybePop<object>());
}), child: new global::Doroti.Generated.Framework.Widgets.Text(CupertinoLocalizations.of(context).cancelButtonLabel)));
            }
        }
        if ((leadingContent__76968 is null))
        {
            return null;
        }
        return new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: leadingKey, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: (padding?.start ?? Nav_barLibrary._kNavBarEdgePadding)), child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(size: 32.0), child: leadingContent__76968))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.KeyedSubtree? createBackChevron(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> backChevronKey, global::Doroti.Generated.Framework.Widgets.Widget? userLeading, dynamic route, bool automaticallyImplyLeading, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if ((((((userLeading is not null) || !automaticallyImplyLeading) || (route is null)) || !((bool)((dynamic)route).canPop)) || (((route is PageRoute<object>) && ((bool)((dynamic)route).fullscreenDialog)))))
        {
            return null;
        }
        return new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: backChevronKey, child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: new _BackChevron__nav_bar()));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.KeyedSubtree? createBackLabel(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> backLabelKey, global::Doroti.Generated.Framework.Widgets.Widget? userLeading, dynamic route, bool automaticallyImplyLeading, string? previousPageTitle, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if ((((((userLeading is not null) || !automaticallyImplyLeading) || (route is null)) || !((bool)((dynamic)route).canPop)) || (((route is PageRoute<object>) && ((bool)((dynamic)route).fullscreenDialog)))))
        {
            return null;
        }
        return new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: backLabelKey, child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: new _BackLabel__nav_bar(specifiedPreviousTitle: previousPageTitle, route: route)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.KeyedSubtree? createMiddle(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> middleKey, global::Doroti.Generated.Framework.Widgets.Widget? userMiddle, global::Doroti.Generated.Framework.Widgets.Widget? userLargeTitle, bool large, bool staticBar, bool automaticallyImplyTitle, dynamic route, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var middleContent__79954 = userMiddle;
        if ((large && staticBar))
        {
            return null;
        }
        if (large)
        {
            middleContent__79954 ??= userLargeTitle;
        }
        middleContent__79954 ??= _NavigationBarStaticComponents__nav_bar._derivedTitle(automaticallyImplyTitle: automaticallyImplyTitle, currentRoute: route);
        if ((middleContent__79954 is null))
        {
            return null;
        }
        return new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: middleKey, child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: middleContent__79954));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.KeyedSubtree? createTrailing(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> trailingKey, global::Doroti.Generated.Framework.Widgets.Widget? userTrailing, global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional? padding, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if ((userTrailing is null))
        {
            return null;
        }
        return new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: trailingKey, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(end: (padding?.end ?? Nav_barLibrary._kNavBarEdgePadding)), child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: _NavigationBarStaticComponents__nav_bar._clampedTextScaler(context)), child: IconTheme.merge(data: new global::Doroti.Generated.Framework.Widgets.IconThemeData(size: 32.0), child: userTrailing))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.KeyedSubtree? createLargeTitle(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> largeTitleKey, global::Doroti.Generated.Framework.Widgets.Widget? userLargeTitle, bool large, bool automaticImplyTitle, dynamic route, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if (!large)
        {
            return null;
        }
        global::Doroti.Generated.Framework.Widgets.Widget? largeTitleContent__81806 = ((userLargeTitle ?? (global::Doroti.Generated.Framework.Widgets.Widget)_NavigationBarStaticComponents__nav_bar._derivedTitle(automaticallyImplyTitle: automaticImplyTitle, currentRoute: route)));
        DartRuntimePrimitives.Assert(() => (largeTitleContent__81806 is not null), () => (object?)"largeTitle was not provided and there was no title from the route.");
        return new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: largeTitleKey, child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: global::Doroti.Generated.Framework.Painting.TextScaler.CreateLinear(Nav_barLibrary._dampScaleFactor(MediaQuery.textScalerOf(context).scale(Nav_barLibrary._kNavBarLargeTitleHeightExtension), Nav_barLibrary._kNavBarLargeTitleHeightExtension, Nav_barLibrary._kLargeTitleScaleDampingRatio))), child: largeTitleContent__81806!));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public static global::Doroti.Generated.Framework.Widgets.KeyedSubtree? createNavBarBottom(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> navBarBottomKey, global::Doroti.Generated.Framework.Widgets.Widget? userBottom, global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: navBarBottomKey, child: new global::Doroti.Generated.Framework.Widgets.MediaQuery(data: MediaQuery.of(context).copyWith(textScaler: MediaQuery.textScalerOf(context)), child: (userBottom ?? global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink())));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Generated.Framework.Painting.TextScaler _clampedTextScaler(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Painting.TextScaler)(object?)MediaQuery.textScalerOf(context).clamp(minScaleFactor: 1.0, maxScaleFactor: Nav_barLibrary._kMaxScaleFactor));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class CupertinoNavigationBarBackButton : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual Color? color { get; private set; }
    public virtual string? previousPageTitle { get; private set; }
    public virtual global::System.Action? onPressed { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Widgets.Widget? _backChevron { get; private set; }
    internal virtual global::Doroti.Generated.Framework.Widgets.Widget? _backLabel { get; private set; }

    public CupertinoNavigationBarBackButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, Color? color = null, string? previousPageTitle = null, global::System.Action? onPressed = null) : base(key: key)
    {
        this.color = color;
        this.previousPageTitle = previousPageTitle;
        this.onPressed = onPressed;
        this._backChevron = null;
        this._backLabel = null;
    }

    public static CupertinoNavigationBarBackButton Create_assemble(global::Doroti.Generated.Framework.Widgets.Widget? _backChevron, global::Doroti.Generated.Framework.Widgets.Widget? _backLabel)
    {
        var __instance = new CupertinoNavigationBarBackButton();
        __instance._backChevron = _backChevron;
        __instance._backLabel = _backLabel;
        __instance.previousPageTitle = null;
        __instance.color = null;
        __instance.onPressed = null;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        dynamic currentRoute__85369 = global::Doroti.Generated.Framework.Widgets.ModalRoute<object>.of<object>(context);
        if ((this.onPressed is null))
        {
            DartRuntimePrimitives.Assert(() => (((bool?)((dynamic)currentRoute__85369)?.canPop) ?? false), () => (object?)"CupertinoNavigationBarBackButton should only be used in routes that can be popped");
        }
        global::Doroti.Generated.Framework.Painting.TextStyle actionTextStyle__85613 = CupertinoTheme.of(context).textTheme.navActionTextStyle;
        if ((this.color is not null))
        {
            actionTextStyle__85613 = actionTextStyle__85613.copyWith(color: CupertinoDynamicColor.maybeResolve(this.color, context));
        }
        CupertinoLocalizations localizations__85879 = CupertinoLocalizations.of(context);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoButton(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, child: new global::Doroti.Generated.Framework.Widgets.Semantics(container: true, excludeSemantics: true, label: localizations__85879.backButtonLabel, button: true, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: actionTextStyle__85613, child: new global::Doroti.Generated.Framework.Widgets.ConstrainedBox(constraints: new global::Doroti.Generated.Framework.Rendering.BoxConstraints(minWidth: Nav_barLibrary._kNavBarBackButtonTapWidth), child: new global::Doroti.Generated.Framework.Widgets.Row(mainAxisSize: global::Doroti.Generated.Framework.Rendering.MainAxisSize.min, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 8.0))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>((this._backChevron ?? new _BackChevron__nav_bar())), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 6.0))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Flexible(child: (this._backLabel ?? new _BackLabel__nav_bar(specifiedPreviousTitle: this.previousPageTitle, route: currentRoute__85369)))) })))), onPressed: (() => {
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

internal class _BackChevron__nav_bar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    internal _BackChevron__nav_bar()
    {
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.TextDirection textDirection__87225 = Directionality.of(context);
        global::Doroti.Generated.Framework.Painting.TextStyle textStyle__87289 = DefaultTextStyle.of(context).style;
        global::Doroti.Generated.Framework.Widgets.Widget iconWidget__87456 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: 6, end: 2), child: global::Doroti.Generated.Framework.Widgets.Text.CreateRich(new global::Doroti.Generated.Framework.Painting.TextSpan(text: char.ConvertFromUtf32(checked((int)CupertinoIcons.back.codePoint)), style: new global::Doroti.Generated.Framework.Painting.TextStyle(inherit: false, color: ((global::Doroti.Generated.Framework.Painting.TextStyle)textStyle__87289).color, fontSize: 30.0, fontFamily: CupertinoIcons.back.fontFamily, package: CupertinoIcons.back.fontPackage)))));
        switch (textDirection__87225)
        {
            case TextDirection.rtl:
                {
                    iconWidget__87456 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Transform(transform: ((Func<Matrix4>)(() =>
{            var __cascade = Matrix4.identity();
            __cascade.scaleByDouble(-1.0, 1.0, 1.0, 1);
            return __cascade;        }))(), alignment: global::Doroti.Generated.Framework.Painting.Alignment.center, transformHitTests: false, child: iconWidget__87456));
                    break;
                }
            case TextDirection.ltr:
                {
                    break;
                }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.KeyedSubtree(key: StandardComponentTypeMembers.key(global::Doroti.Generated.Framework.Widgets.StandardComponentType.backButton), child: iconWidget__87456));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _BackLabel__nav_bar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual string? specifiedPreviousTitle { get; private set; }
    public virtual dynamic route { get; private set; } = default!;

    internal _BackLabel__nav_bar(string? specifiedPreviousTitle, dynamic route)
    {
        this.specifiedPreviousTitle = specifiedPreviousTitle;
        this.route = route;
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildPreviousTitleWidget(global::Doroti.Generated.Framework.Widgets.BuildContext context, string? previousTitle, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        if ((previousTitle is null))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
        }
        var textWidget__88934 = new global::Doroti.Generated.Framework.Widgets.Text(previousTitle, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis);
        if ((previousTitle.Length > 12L))
        {
            textWidget__88934 = new global::Doroti.Generated.Framework.Widgets.Text(CupertinoLocalizations.of(context).backButtonLabel);
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, widthFactor: 1.0, child: textWidget__88934));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if ((this.specifiedPreviousTitle is not null))
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_buildPreviousTitleWidget(context, this.specifiedPreviousTitle, null));
        }
        else
        {
            if (((this.route is CupertinoRouteTransitionMixin<object>) && !((bool)((dynamic)this.route!).isFirst)))
            {
                CupertinoRouteTransitionMixin<object> route__as89428 = (CupertinoRouteTransitionMixin<object>)route;
                var cupertinoRoute__89510 = ((CupertinoRouteTransitionMixin<object>?)(object?)this.route!)!;
                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ValueListenableBuilder<string?>(valueListenable: ((CupertinoRouteTransitionMixin<object>)cupertinoRoute__89510).previousTitle, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string?, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._buildPreviousTitleWidget));
            }
            else
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CancelButton__nav_bar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::System.Action? onPressed { get; private set; }
    public virtual double opacity { get; private set; } = default!;

    internal _CancelButton__nav_bar(double opacity = 1.0, global::System.Action? onPressed = default!)
    {
        this.opacity = opacity;
        this.onPressed = onPressed;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CupertinoLocalizations localizations__90329 = CupertinoLocalizations.of(context);
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.Alignment.centerLeft, child: new global::Doroti.Generated.Framework.Widgets.Opacity(opacity: this.opacity, child: new CupertinoButton(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, onPressed: this.onPressed, child: new global::Doroti.Generated.Framework.Widgets.Text(localizations__90329.cancelButtonLabel, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.clip))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _InactiveSearchableBottom__nav_bar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController animationController { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? searchField { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual double searchFieldHeight { get; private set; } = default!;
    public virtual global::System.Action? onSearchFieldTap { get; private set; }

    internal _InactiveSearchableBottom__nav_bar(global::Doroti.Generated.Framework.Animation.AnimationController animationController, global::Doroti.Generated.Framework.Widgets.Widget? searchField, global::Doroti.Generated.Framework.Animation.Animation<double> animation, double searchFieldHeight, global::System.Action? onSearchFieldTap)
    {
        this.animationController = animationController;
        this.searchField = searchField;
        this.animation = animation;
        this.searchFieldHeight = searchFieldHeight;
        this.onSearchFieldTap = onSearchFieldTap;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this.animation, child: new global::Doroti.Generated.Framework.Widgets.GestureDetector(onTap: () => this.onSearchFieldTap(), child: new global::Doroti.Generated.Framework.Widgets.AbsorbPointer(child: new global::Doroti.Generated.Framework.Widgets.FocusableActionDetector(descendantsAreFocusable: false, child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, end: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: this.searchFieldHeight, child: this.searchField))))), builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.LayoutBuilder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Rendering.BoxConstraints, global::Doroti.Generated.Framework.Widgets.Widget>)((context, constraints) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Row(children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: (((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints).maxWidth - ((Nav_barLibrary._kSearchFieldCancelButtonWidth * ((global::Doroti.Generated.Framework.Animation.AnimationController)this.animationController).value))), child: child)), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.SizedBox(width: (((global::Doroti.Generated.Framework.Animation.AnimationController)this.animationController).value * Nav_barLibrary._kSearchFieldCancelButtonWidth), child: new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateOnly(bottom: Nav_barLibrary._kNavBarBottomPadding), child: new _CancelButton__nav_bar(opacity: 0.4, onPressed: ((global::System.Action)(() => {
})))))) }));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _ActiveSearchableBottom__nav_bar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.AnimationController animationController { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? searchField { get; private set; }
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual double searchFieldHeight { get; private set; } = default!;
    public virtual global::System.Action? onSearchFieldTap { get; private set; }

    internal _ActiveSearchableBottom__nav_bar(global::Doroti.Generated.Framework.Animation.AnimationController animationController, global::Doroti.Generated.Framework.Widgets.Widget? searchField, global::Doroti.Generated.Framework.Animation.Animation<double> animation, double searchFieldHeight, global::System.Action? onSearchFieldTap)
    {
        this.animationController = animationController;
        this.searchField = searchField;
        this.animation = animation;
        this.searchFieldHeight = searchFieldHeight;
        this.onSearchFieldTap = onSearchFieldTap;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(padding: global::Doroti.Generated.Framework.Painting.EdgeInsetsDirectional.CreateOnly(start: Nav_barLibrary._kNavBarEdgePadding, bottom: Nav_barLibrary._kNavBarBottomPadding), child: new global::Doroti.Generated.Framework.Widgets.Row(spacing: 12.0, children: new List<global::Doroti.Generated.Framework.Widgets.Widget> { DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Expanded(child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: this.searchFieldHeight, child: (this.searchField ?? global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink())))), DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.AnimatedBuilder(animation: this.animation, child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0).animate(this.animationController), child: new _CancelButton__nav_bar(onPressed: () => this.onSearchFieldTap())), builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)((context, child) => {
return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.SizedBox(width: (((global::Doroti.Generated.Framework.Animation.AnimationController)this.animationController).value * Nav_barLibrary._kSearchFieldCancelButtonWidth), child: child));
throw new InvalidOperationException("Dart closure completed without a value.");
})))) })));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _TransitionableNavigationBar__nav_bar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual _NavigationBarStaticComponentsKeys__nav_bar componentsKeys { get; private set; } = default!;
    public virtual Color? backgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle backButtonTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle titleTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? largeTitleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Border? border { get; private set; }
    public virtual bool hasUserMiddle { get; private set; } = default!;
    public virtual bool largeExpanded { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    public virtual bool automaticBackgroundVisibility { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.Widget child { get; private set; } = default!;

    internal _TransitionableNavigationBar__nav_bar(_NavigationBarStaticComponentsKeys__nav_bar componentsKeys, Color? backgroundColor, global::Doroti.Generated.Framework.Painting.TextStyle backButtonTextStyle, global::Doroti.Generated.Framework.Painting.TextStyle titleTextStyle, global::Doroti.Generated.Framework.Painting.TextStyle? largeTitleTextStyle, global::Doroti.Generated.Framework.Painting.Border? border, bool hasUserMiddle, bool largeExpanded, bool searchable, bool automaticBackgroundVisibility, global::Doroti.Generated.Framework.Widgets.Widget child) : base(key: ((_NavigationBarStaticComponentsKeys__nav_bar)componentsKeys).navBarBoxKey)
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

    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox renderBox
    {
        get
        {
            var box__95905 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.componentsKeys).navBarBoxKey.currentContext!.findRenderObject()!)!;
            DartRuntimePrimitives.Assert(() => box__95905.attached, () => (object?)"_TransitionableNavigationBar.renderBox should be called when building " + "hero flight shuttles when the from and the to nav bar boxes are already " + "laid out and painted.");
            return box__95905;
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
    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                var inHero__96449 = false;
                context.visitAncestorElements(((global::System.Func<global::Doroti.Generated.Framework.Widgets.Element, bool>)((ancestor) => {
if ((ancestor is global::Doroti.Generated.Framework.Widgets.ComponentElement))
{
    DartRuntimePrimitives.Assert(() => (!object.Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Generated.Framework.Widgets.ComponentElement)ancestor).widget), typeof(_NavigationBarTransition__nav_bar))), () => (object?)"_TransitionableNavigationBar should never re-appear inside " + "_NavigationBarTransition. Keyed _TransitionableNavigationBar should " + "only serve as anchor points in routes rather than appearing inside " + "Hero flights themselves.");
    if ((object.Equals(DartRuntimePrimitives.RuntimeType(((global::Doroti.Generated.Framework.Widgets.ComponentElement)ancestor).widget), typeof(global::Doroti.Generated.Framework.Widgets.Hero))))
    {
        inHero__96449 = true;
    }
}
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
                DartRuntimePrimitives.Assert(() => inHero__96449, () => (object?)"_TransitionableNavigationBar should only be added as the immediate " + "child of Hero widgets.");
                return true;
            });
        return this.child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarTransition__nav_bar : global::Doroti.Generated.Framework.Widgets.StatelessWidget
{
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual _TransitionableNavigationBar__nav_bar topNavBar { get; private set; } = default!;
    public virtual _TransitionableNavigationBar__nav_bar bottomNavBar { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Tween<double> heightTween { get; private set; } = default!;

    internal _NavigationBarTransition__nav_bar(global::Doroti.Generated.Framework.Animation.Animation<double> animation, _TransitionableNavigationBar__nav_bar topNavBar, _TransitionableNavigationBar__nav_bar bottomNavBar)
    {
        this.animation = animation;
        this.topNavBar = topNavBar;
        this.bottomNavBar = bottomNavBar;
        this.heightTween = new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: ((_TransitionableNavigationBar__nav_bar)bottomNavBar).renderBox.size.height, end: ((_TransitionableNavigationBar__nav_bar)topNavBar).renderBox.size.height);
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var componentsTransition__98573 = new _NavigationBarComponentsTransition__nav_bar(animation: this.animation, bottomNavBar: this.bottomNavBar, topNavBar: this.topNavBar, directionality: Directionality.of(context));
        var children__98790 = ((Func<List<global::Doroti.Generated.Framework.Widgets.Widget>>)(() => { var __collection98801 = new List<global::Doroti.Generated.Framework.Widgets.Widget>(); var __collectionElement98817 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).bottomNavBarBackground; if (__collectionElement98817 is { } __nonNullCollectionElement98817) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement98817)); } var __collectionElement98869 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).bottomBackChevron; if (__collectionElement98869 is { } __nonNullCollectionElement98869) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement98869)); } var __collectionElement98916 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).bottomBackLabel; if (__collectionElement98916 is { } __nonNullCollectionElement98916) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement98916)); } var __collectionElement98961 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).bottomLeading; if (__collectionElement98961 is { } __nonNullCollectionElement98961) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement98961)); } var __collectionElement99004 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).bottomMiddle; if (__collectionElement99004 is { } __nonNullCollectionElement99004) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99004)); } var __collectionElement99046 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).bottomLargeTitle; if (__collectionElement99046 is { } __nonNullCollectionElement99046) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99046)); } var __collectionElement99092 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).bottomTrailing; if (__collectionElement99092 is { } __nonNullCollectionElement99092) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99092)); } var __collectionElement99136 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).bottomNavBarBottom; if (__collectionElement99136 is { } __nonNullCollectionElement99136) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99136)); } var __collectionElement99246 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).topNavBarBackground; if (__collectionElement99246 is { } __nonNullCollectionElement99246) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99246)); } var __collectionElement99295 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).topLeading; if (__collectionElement99295 is { } __nonNullCollectionElement99295) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99295)); } var __collectionElement99335 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).topBackChevron; if (__collectionElement99335 is { } __nonNullCollectionElement99335) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99335)); } var __collectionElement99379 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).topBackLabel; if (__collectionElement99379 is { } __nonNullCollectionElement99379) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99379)); } var __collectionElement99421 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).topMiddle; if (__collectionElement99421 is { } __nonNullCollectionElement99421) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99421)); } var __collectionElement99460 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).topLargeTitle; if (__collectionElement99460 is { } __nonNullCollectionElement99460) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99460)); } var __collectionElement99503 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).topTrailing; if (__collectionElement99503 is { } __nonNullCollectionElement99503) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99503)); } var __collectionElement99544 = ((_NavigationBarComponentsTransition__nav_bar)componentsTransition__98573).topNavBarBottom; if (__collectionElement99544 is { } __nonNullCollectionElement99544) { __collection98801.Add(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(__nonNullCollectionElement99544)); } return __collection98801; }))();
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)MediaQuery.withNoTextScaling(child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: (Math.Max(DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Animation.Tween<double>)this.heightTween).begin), DartRuntimePrimitives.RequireValue(((global::Doroti.Generated.Framework.Animation.Tween<double>)this.heightTween).end)) + MediaQuery.paddingOf(context).top), width: double.PositiveInfinity, child: new global::Doroti.Generated.Framework.Widgets.Stack(children: children__98790))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _NavigationBarComponentsTransition__nav_bar
{
    public static global::Doroti.Generated.Framework.Animation.Animatable<double> fadeOut = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 1.0, end: 0.0));
    public static global::Doroti.Generated.Framework.Animation.Animatable<double> fadeIn = ((global::Doroti.Generated.Framework.Animation.Animatable<double>)(object?)new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: 1.0));
    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> animation { get; private set; } = default!;
    public virtual _NavigationBarStaticComponentsKeys__nav_bar bottomComponents { get; private set; } = default!;
    public virtual _NavigationBarStaticComponentsKeys__nav_bar topComponents { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox bottomNavBarBox { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.RenderBox topNavBarBox { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle bottomBackButtonTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle topBackButtonTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle bottomTitleTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle topTitleTextStyle { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? bottomLargeTitleTextStyle { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.TextStyle? topLargeTitleTextStyle { get; private set; }
    public virtual bool bottomHasUserMiddle { get; private set; } = default!;
    public virtual bool topHasUserMiddle { get; private set; } = default!;
    public virtual bool bottomLargeExpanded { get; private set; } = default!;
    public virtual bool topLargeExpanded { get; private set; } = default!;
    public virtual bool userGestureInProgress { get; private set; } = default!;
    public virtual bool searchable { get; private set; } = default!;
    public virtual bool bottomAutomaticBackgroundVisibility { get; private set; } = default!;
    public virtual Color? bottomBackgroundColor { get; private set; }
    public virtual Color? topBackgroundColor { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Border? bottomBorder { get; private set; }
    public virtual global::Doroti.Generated.Framework.Painting.Border? topBorder { get; private set; }
    public virtual Rect transitionBox { get; private set; } = default!;
    public virtual double forwardDirection { get; private set; } = default!;

    internal _NavigationBarComponentsTransition__nav_bar(global::Doroti.Generated.Framework.Animation.Animation<double> animation, _TransitionableNavigationBar__nav_bar bottomNavBar, _TransitionableNavigationBar__nav_bar topNavBar, TextDirection directionality)
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

    public virtual global::Doroti.Generated.Framework.Rendering.RelativeRect positionInTransitionBox(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key, global::Doroti.Generated.Framework.Rendering.RenderBox from)
    {
        var componentBox__104559 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)key).currentContext!.findRenderObject()!)!;
        DartRuntimePrimitives.Assert(() => componentBox__104559.attached);
        return global::Doroti.Generated.Framework.Rendering.RelativeRect.CreateFromRect((((Offset)((dynamic)componentBox__104559).localToGlobal(Offset.zero, ancestor: from)) & ((global::Doroti.Generated.Framework.Rendering.RenderBox)componentBox__104559).size), this.transitionBox);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual _FixedSizeSlidingTransition__nav_bar slideFromLeadingEdge(global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> fromKey, global::Doroti.Generated.Framework.Rendering.RenderBox fromNavBarBox, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> toKey, global::Doroti.Generated.Framework.Rendering.RenderBox toNavBarBox, global::Doroti.Generated.Framework.Animation.Curve curve = default!, global::Doroti.Generated.Framework.Widgets.Widget child = default!)
    {
        var fromBox__105805 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)fromKey).currentContext!.findRenderObject()!)!;
        var toBox__105883 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>)toKey).currentContext!.findRenderObject()!)!;
        bool isLTR__105963 = (this.forwardDirection > 0L);
        var fromAnchorLocal__106157 = new global::Doroti.Ui.Offset((isLTR__105963 ? 0 : ((global::Doroti.Generated.Framework.Rendering.RenderBox)fromBox__105805).size.width), (((global::Doroti.Generated.Framework.Rendering.RenderBox)fromBox__105805).size.height / 2L));
        var toAnchorLocal__106250 = new global::Doroti.Ui.Offset((isLTR__105963 ? 0 : ((global::Doroti.Generated.Framework.Rendering.RenderBox)toBox__105883).size.width), (((global::Doroti.Generated.Framework.Rendering.RenderBox)toBox__105883).size.height / 2L));
        global::Doroti.Ui.Offset fromAnchorInFromBox__106344 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)fromBox__105805).localToGlobal(fromAnchorLocal__106157, ancestor: fromNavBarBox)));
        global::Doroti.Ui.Offset toAnchorInToBox__106467 = ((global::Doroti.Ui.Offset)(object?)((Offset)((dynamic)toBox__105883).localToGlobal(toAnchorLocal__106250, ancestor: toNavBarBox)));
        global::Doroti.Ui.Offset translation__107001 = ((global::Doroti.Ui.Offset)(object?)(isLTR__105963 ? (toAnchorInToBox__106467 - fromAnchorInFromBox__106344) : (new global::Doroti.Ui.Offset((((global::Doroti.Generated.Framework.Rendering.RenderBox)toNavBarBox).size.width - toAnchorInToBox__106467.dx), toAnchorInToBox__106467.dy) - new global::Doroti.Ui.Offset((((global::Doroti.Generated.Framework.Rendering.RenderBox)fromNavBarBox).size.width - fromAnchorInFromBox__106344.dx), fromAnchorInFromBox__106344.dy))));
        global::Doroti.Generated.Framework.Rendering.RelativeRect fromBoxMargin__107274 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(fromKey, from: fromNavBarBox));
        var fromOriginInTransitionBox__107355 = new global::Doroti.Ui.Offset((isLTR__105963 ? ((global::Doroti.Generated.Framework.Rendering.RelativeRect)fromBoxMargin__107274).left : ((global::Doroti.Generated.Framework.Rendering.RelativeRect)fromBoxMargin__107274).right), ((global::Doroti.Generated.Framework.Rendering.RelativeRect)fromBoxMargin__107274).top);
        var anchorMovementInTransitionBox__107490 = new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: fromOriginInTransitionBox__107355, end: (fromOriginInTransitionBox__107355 + translation__107001));
        return new _FixedSizeSlidingTransition__nav_bar(isLTR: isLTR__105963, offsetAnimation: this.animation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: curve)).drive(anchorMovementInTransitionBox__107490), width: ((global::Doroti.Generated.Framework.Rendering.RenderBox)fromNavBarBox).size.width, height: ((global::Doroti.Generated.Framework.Rendering.RenderBox)fromBox__105805).size.height, child: child);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> fadeInFrom(double t, global::Doroti.Generated.Framework.Animation.Curve curve = default!)
    {
        return ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)this.animation.drive(fadeIn.chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(t, 1.0, curve: curve)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> fadeOutBy(double t, global::Doroti.Generated.Framework.Animation.Curve curve = default!)
    {
        return ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)this.animation.drive(fadeOut.chain(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, t, curve: curve)))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Animation.Animation<double> routeAnimation
    {
        get
        {
            DartRuntimePrimitives.Assert(() => (this.animation is global::Doroti.Generated.Framework.Animation.CurvedAnimation));
            return (((global::Doroti.Generated.Framework.Animation.CurvedAnimation?)(object?)this.animation)!).parent;
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? bottomNavBarBackground
    {
        get
        {
            if (((this.bottomBackgroundColor is null) || ((this.bottomLargeExpanded && this.bottomAutomaticBackgroundVisibility))))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Animation.Curve animationCurve__108721 = ((object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)) ? global::Doroti.Generated.Framework.Animation.Curves.fastEaseInToSlowEaseOut : global::Doroti.Generated.Framework.Animation.Curves.fastEaseInToSlowEaseOut.flipped);
            global::Doroti.Generated.Framework.Animation.Animation<double> pageTransitionAnimation__108902 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)this.routeAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: (this.userGestureInProgress ? global::Doroti.Generated.Framework.Animation.Curves.linear : animationCurve__108721))));
            global::Doroti.Generated.Framework.Rendering.RelativeRect from__109062 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).navBarBoxKey, from: this.bottomNavBarBox));
            var positionTween__109178 = new global::Doroti.Generated.Framework.Widgets.RelativeRectTween(end: from__109062.shift(new global::Doroti.Ui.Offset((this.forwardDirection * -((global::Doroti.Generated.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.width), 0.0)), begin: from__109062);
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.PositionedTransition(rect: pageTransitionAnimation__108902.drive(positionTween__109178), child: Nav_barLibrary._wrapWithBackground(updateSystemUiOverlay: false, backgroundColor: this.bottomBackgroundColor!, border: this.topBorder, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: ((global::Doroti.Generated.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.height, width: double.PositiveInfinity))));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? bottomLeading
    {
        get
        {
            var bottomLeading__109774 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).leadingKey.currentWidget)!;
            if ((bottomLeading__109774 is null))
            {
                return null;
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).leadingKey, from: this.bottomNavBarBox), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.4), child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomLeading__109774).child)));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? bottomBackChevron
    {
        get
        {
            var bottomBackChevron__110177 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backChevronKey.currentWidget)!;
            if ((bottomBackChevron__110177 is null))
            {
                return null;
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backChevronKey, from: this.bottomNavBarBox), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this.bottomBackButtonTextStyle, child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomBackChevron__110177).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? bottomBackLabel
    {
        get
        {
            var bottomBackLabel__110682 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backLabelKey.currentWidget)!;
            if ((bottomBackLabel__110682 is null))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Rendering.RelativeRect from__110847 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backLabelKey, from: this.bottomNavBarBox));
            var positionTween__111049 = new global::Doroti.Generated.Framework.Widgets.RelativeRectTween(begin: from__110847, end: from__110847.shift(new global::Doroti.Ui.Offset((this.forwardDirection * ((-((global::Doroti.Generated.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.width / 2.0))), 0.0)));
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.PositionedTransition(rect: this.animation.drive(positionTween__111049), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.2), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this.bottomBackButtonTextStyle, child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomBackLabel__110682).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? bottomMiddle
    {
        get
        {
            var bottomMiddle__111499 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey.currentWidget)!;
            var topBackLabel__111583 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey.currentWidget)!;
            var topLeading__111667 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).leadingKey.currentWidget)!;
            if ((!this.bottomHasUserMiddle && this.bottomLargeExpanded))
            {
                return null;
            }
            if (((bottomMiddle__111499 is not null) && (topBackLabel__111583 is not null)))
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)slideFromLeadingEdge(fromKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey, fromNavBarBox: this.bottomNavBarBox, toKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey, toNavBarBox: this.topNavBarBox, child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy((this.bottomHasUserMiddle ? 0.4 : 0.7)), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyleTransition(style: this.animation.drive(new global::Doroti.Generated.Framework.Widgets.TextStyleTween(begin: this.bottomTitleTextStyle, end: this.topBackButtonTextStyle)), child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomMiddle__111499).child)))));
            }
            if (((bottomMiddle__111499 is not null) && (topLeading__111667 is not null)))
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey, from: this.bottomNavBarBox), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy((this.bottomHasUserMiddle ? 0.4 : 0.7)), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this.bottomTitleTextStyle, child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomMiddle__111499).child))));
            }
            return null;
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? bottomLargeTitle
    {
        get
        {
            var bottomLargeTitle__113694 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey.currentWidget)!;
            var topBackLabel__113786 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey.currentWidget)!;
            if (((bottomLargeTitle__113694 is null) || !this.bottomLargeExpanded))
            {
                return null;
            }
            if ((topBackLabel__113786 is not null))
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)slideFromLeadingEdge(fromKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey, fromNavBarBox: this.bottomNavBarBox, toKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey, toNavBarBox: this.topNavBarBox, curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, ((object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)) ? 0.7 : 1.0)), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: new global::Doroti.Generated.Framework.Widgets.Align(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.centerStart, child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyleTransition(style: this.animation.drive(new global::Doroti.Generated.Framework.Widgets.TextStyleTween(begin: this.bottomLargeTitleTextStyle, end: this.topBackButtonTextStyle)), maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomLargeTitle__113694).child)))));
            }
            global::Doroti.Generated.Framework.Rendering.RelativeRect from__115140 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey, from: this.bottomNavBarBox));
            var positionTween__115257 = new global::Doroti.Generated.Framework.Widgets.RelativeRectTween(begin: from__115140, end: from__115140.shift(new global::Doroti.Ui.Offset(((this.forwardDirection * ((global::Doroti.Generated.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.width) / 4.0), 0.0)));
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.PositionedTransition(rect: this.animation.drive(positionTween__115257), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.4), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this.bottomLargeTitleTextStyle!, child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomLargeTitle__113694).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? bottomTrailing
    {
        get
        {
            var bottomTrailing__115889 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).trailingKey.currentWidget)!;
            if ((bottomTrailing__115889 is null))
            {
                return null;
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).trailingKey, from: this.bottomNavBarBox), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.6), child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomTrailing__115889).child)));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? bottomNavBarBottom
    {
        get
        {
            var bottomNavBarBottom__116298 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).navBarBottomKey.currentWidget)!;
            if ((bottomNavBarBottom__116298 is null))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Rendering.RelativeRect from__116472 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).navBarBottomKey, from: this.bottomNavBarBox));
            var positionTween__116643 = new global::Doroti.Generated.Framework.Widgets.RelativeRectTween(begin: from__116472, end: from__116472.shift(new global::Doroti.Ui.Offset((this.forwardDirection * -((global::Doroti.Generated.Framework.Rendering.RenderBox)this.bottomNavBarBox).size.width), 0.0)));
            global::Doroti.Generated.Framework.Widgets.Widget child__116800 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)bottomNavBarBottom__116298).child;
            global::Doroti.Generated.Framework.Animation.Curve animationCurve__116850 = ((object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)) ? Nav_barLibrary._kBottomNavBarHeaderTransitionCurve : ((global::Doroti.Generated.Framework.Animation.Curve)Nav_barLibrary._kBottomNavBarHeaderTransitionCurve).flipped);
            if (!this.searchable)
            {
                child__116800 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeOutBy(0.8, curve: animationCurve__116850), child: child__116800));
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.PositionedTransition(rect: (this.userGestureInProgress ? this.routeAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.linear)).drive(positionTween__116643) : this.animation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: animationCurve__116850)).drive(positionTween__116643)), child: new global::Doroti.Generated.Framework.Widgets.ClipRect(child: child__116800)));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? topNavBarBackground
    {
        get
        {
            if ((this.topBackgroundColor is null))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Animation.Curve animationCurve__117794 = ((object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)) ? global::Doroti.Generated.Framework.Animation.Curves.fastEaseInToSlowEaseOut : global::Doroti.Generated.Framework.Animation.Curves.fastEaseInToSlowEaseOut.flipped);
            global::Doroti.Generated.Framework.Animation.Animation<double> pageTransitionAnimation__117975 = ((global::Doroti.Generated.Framework.Animation.Animation<double>)(object?)this.routeAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: (this.userGestureInProgress ? global::Doroti.Generated.Framework.Animation.Curves.linear : animationCurve__117794))));
            global::Doroti.Generated.Framework.Rendering.RelativeRect to__118135 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).navBarBoxKey, from: this.topNavBarBox));
            var positionTween__118224 = new global::Doroti.Generated.Framework.Widgets.RelativeRectTween(begin: to__118135.shift(new global::Doroti.Ui.Offset((this.forwardDirection * ((global::Doroti.Generated.Framework.Rendering.RenderBox)this.topNavBarBox).size.width), 0.0)), end: to__118135);
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.PositionedTransition(rect: pageTransitionAnimation__117975.drive(positionTween__118224), child: Nav_barLibrary._wrapWithBackground(updateSystemUiOverlay: false, backgroundColor: this.topBackgroundColor!, border: this.topBorder, child: new global::Doroti.Generated.Framework.Widgets.SizedBox(height: ((global::Doroti.Generated.Framework.Rendering.RenderBox)this.topNavBarBox).size.height, width: double.PositiveInfinity))));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? topLeading
    {
        get
        {
            var topLeading__118803 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).leadingKey.currentWidget)!;
            if ((topLeading__118803 is null))
            {
                return null;
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).leadingKey, from: this.topNavBarBox), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.6), child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)topLeading__118803).child)));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? topBackChevron
    {
        get
        {
            var topBackChevron__119186 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backChevronKey.currentWidget)!;
            var bottomBackChevron__119274 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).backChevronKey.currentWidget)!;
            if ((topBackChevron__119186 is null))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Rendering.RelativeRect to__119442 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backChevronKey, from: this.topNavBarBox));
            var from__119549 = to__119442;
            global::Doroti.Generated.Framework.Widgets.Widget child__119572 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)topBackChevron__119186).child;
            global::Doroti.Generated.Framework.Animation.Curve forwardScaleCurve__119688 = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0.0, 0.2));
            global::Doroti.Generated.Framework.Animation.Curve backwardScaleCurve__119744 = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0.8, 1.0));
            global::Doroti.Generated.Framework.Animation.Curve forwardPositionCurve__119801 = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0.0, 0.5));
            global::Doroti.Generated.Framework.Animation.Curve backwardPositionCurve__119860 = ((global::Doroti.Generated.Framework.Animation.Curve)(object?)new global::Doroti.Generated.Framework.Animation.Interval(0.5, 1.0));
            global::Doroti.Generated.Framework.Animation.Curve effectiveScaleCurve__119920 = default!;
            global::Doroti.Generated.Framework.Animation.Curve effectivePositionCurve__119957 = default!;
            if ((object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)))
            {
                effectiveScaleCurve__119920 = forwardScaleCurve__119688;
                effectivePositionCurve__119957 = forwardPositionCurve__119801;
            }
            else
            {
                effectiveScaleCurve__119920 = backwardScaleCurve__119744;
                effectivePositionCurve__119957 = backwardPositionCurve__119860;
            }
            if ((bottomBackChevron__119274 is null))
            {
                var topBackChevronBox__120411 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backChevronKey.currentContext!.findRenderObject()!)!;
                from__119549 = to__119442.shift(new global::Doroti.Ui.Offset(((this.forwardDirection * ((global::Doroti.Generated.Framework.Rendering.RenderBox)topBackChevronBox__120411).size.width) * 2.0), 0.0));
                child__119572 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.ScaleTransition(scale: this.routeAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: effectiveScaleCurve__119920)), child: child__119572));
            }
            var positionTween__120767 = new global::Doroti.Generated.Framework.Widgets.RelativeRectTween(begin: from__119549, end: to__119442);
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.PositionedTransition(rect: this.routeAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: effectivePositionCurve__119957)).drive(positionTween__120767), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: this.routeAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: new global::Doroti.Generated.Framework.Animation.Interval((((bottomBackChevron__119274 is null) && (!object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward))) ? 0.9 : 0.4), 1.0))), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this.topBackButtonTextStyle, child: child__119572))));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? topBackLabel
    {
        get
        {
            var bottomMiddle__121453 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey.currentWidget)!;
            var bottomLargeTitle__121537 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey.currentWidget)!;
            var topBackLabel__121629 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey.currentWidget)!;
            if ((topBackLabel__121629 is null))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Rendering.RenderAnimatedOpacity? topBackLabelOpacity__121795 = ((global::Doroti.Generated.Framework.Rendering.RenderAnimatedOpacity?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey.currentContext?.findAncestorRenderObjectOfType<global::Doroti.Generated.Framework.Rendering.RenderAnimatedOpacity>());
            global::Doroti.Generated.Framework.Animation.Animation<double>? midClickOpacity__121950 = default!;
            if (((topBackLabelOpacity__121795 is not null) && (topBackLabelOpacity__121795.opacity.value < 1.0)))
            {
                midClickOpacity__121950 = this.animation.drive(new global::Doroti.Generated.Framework.Animation.Tween<double>(begin: 0.0, end: topBackLabelOpacity__121795.opacity.value));
            }
            if (((bottomLargeTitle__121537 is not null) && this.bottomLargeExpanded))
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)slideFromLeadingEdge(fromKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).largeTitleKey, fromNavBarBox: this.bottomNavBarBox, toKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey, toNavBarBox: this.topNavBarBox, curve: new global::Doroti.Generated.Framework.Animation.Interval(0.0, ((object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)) ? 0.7 : 1.0)), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: ((midClickOpacity__121950 ?? (global::Doroti.Generated.Framework.Animation.Animation<double>)fadeInFrom(0.4))), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyleTransition(style: this.animation.drive(new global::Doroti.Generated.Framework.Widgets.TextStyleTween(begin: this.bottomLargeTitleTextStyle, end: this.topBackButtonTextStyle)), maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)topBackLabel__121629).child))));
            }
            if ((bottomMiddle__121453 is not null))
            {
                return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)slideFromLeadingEdge(fromKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.bottomComponents).middleKey, fromNavBarBox: this.bottomNavBarBox, toKey: ((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).backLabelKey, toNavBarBox: this.topNavBarBox, child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: ((midClickOpacity__121950 ?? (global::Doroti.Generated.Framework.Animation.Animation<double>)fadeInFrom(0.3))), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyleTransition(style: this.animation.drive(new global::Doroti.Generated.Framework.Widgets.TextStyleTween(begin: this.bottomTitleTextStyle, end: this.topBackButtonTextStyle)), child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)topBackLabel__121629).child))));
            }
            return null;
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? topMiddle
    {
        get
        {
            var topMiddle__124031 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).middleKey.currentWidget)!;
            if ((topMiddle__124031 is null))
            {
                return null;
            }
            if ((!this.topHasUserMiddle && this.topLargeExpanded))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Rendering.RelativeRect to__124410 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).middleKey, from: this.topNavBarBox));
            var toBox__124495 = ((global::Doroti.Generated.Framework.Rendering.RenderBox?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).middleKey.currentContext!.findRenderObject()!)!;
            bool isLTR__124593 = (this.forwardDirection > 0L);
            var toAnchorInTransitionBox__124740 = new global::Doroti.Ui.Offset((isLTR__124593 ? ((global::Doroti.Generated.Framework.Rendering.RelativeRect)to__124410).left : ((global::Doroti.Generated.Framework.Rendering.RelativeRect)to__124410).right), ((global::Doroti.Generated.Framework.Rendering.RelativeRect)to__124410).top);
            var anchorMovementInTransitionBox__124875 = new global::Doroti.Generated.Framework.Animation.Tween<global::Doroti.Ui.Offset>(begin: new global::Doroti.Ui.Offset((((global::Doroti.Generated.Framework.Rendering.RenderBox)this.topNavBarBox).size.width - (((global::Doroti.Generated.Framework.Rendering.RenderBox)toBox__124495).size.width / 2L)), ((global::Doroti.Generated.Framework.Rendering.RelativeRect)to__124410).top), end: toAnchorInTransitionBox__124740);
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new _FixedSizeSlidingTransition__nav_bar(isLTR: isLTR__124593, offsetAnimation: this.animation.drive(anchorMovementInTransitionBox__124875), width: ((global::Doroti.Generated.Framework.Rendering.RenderBox)toBox__124495).size.width, height: ((global::Doroti.Generated.Framework.Rendering.RenderBox)toBox__124495).size.height, child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.25), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this.topTitleTextStyle, child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)topMiddle__124031).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? topTrailing
    {
        get
        {
            var topTrailing__125597 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).trailingKey.currentWidget)!;
            if ((topTrailing__125597 is null))
            {
                return null;
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)global::Doroti.Generated.Framework.Widgets.Positioned.CreateFromRelativeRect(rect: positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).trailingKey, from: this.topNavBarBox), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.4), child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)topTrailing__125597).child)));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? topLargeTitle
    {
        get
        {
            var topLargeTitle__125984 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).largeTitleKey.currentWidget)!;
            if (((topLargeTitle__125984 is null) || !this.topLargeExpanded))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Rendering.RelativeRect to__126164 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).largeTitleKey, from: this.topNavBarBox));
            var positionTween__126327 = new global::Doroti.Generated.Framework.Widgets.RelativeRectTween(begin: to__126164.shift(new global::Doroti.Ui.Offset((this.forwardDirection * ((global::Doroti.Generated.Framework.Rendering.RenderBox)this.topNavBarBox).size.width), 0.0)), end: to__126164);
            global::Doroti.Generated.Framework.Animation.Curve animationCurve__126481 = ((object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)) ? Nav_barLibrary._kTopNavBarHeaderTransitionCurve : ((global::Doroti.Generated.Framework.Animation.Curve)Nav_barLibrary._kTopNavBarHeaderTransitionCurve).flipped);
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.PositionedTransition(rect: (this.userGestureInProgress ? this.routeAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.linear)).drive(positionTween__126327) : this.animation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: animationCurve__126481)).drive(positionTween__126327)), child: new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.0, curve: animationCurve__126481), child: new global::Doroti.Generated.Framework.Widgets.DefaultTextStyle(style: this.topLargeTitleTextStyle!, maxLines: 1L, overflow: global::Doroti.Generated.Framework.Painting.TextOverflow.ellipsis, child: ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)topLargeTitle__125984).child))));
            return default!;
        }
    }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? topNavBarBottom
    {
        get
        {
            var topNavBarBottom__127312 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree?)(object?)((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).navBarBottomKey.currentWidget)!;
            if ((topNavBarBottom__127312 is null))
            {
                return null;
            }
            global::Doroti.Generated.Framework.Rendering.RelativeRect to__127477 = ((global::Doroti.Generated.Framework.Rendering.RelativeRect)(object?)positionInTransitionBox(((_NavigationBarStaticComponentsKeys__nav_bar)this.topComponents).navBarBottomKey, from: this.topNavBarBox));
            var positionTween__127641 = new global::Doroti.Generated.Framework.Widgets.RelativeRectTween(begin: to__127477.shift(new global::Doroti.Ui.Offset((this.forwardDirection * ((global::Doroti.Generated.Framework.Rendering.RenderBox)this.topNavBarBox).size.width), 0.0)), end: to__127477);
            global::Doroti.Generated.Framework.Widgets.Widget child__127790 = ((global::Doroti.Generated.Framework.Widgets.KeyedSubtree)topNavBarBottom__127312).child;
            global::Doroti.Generated.Framework.Animation.Curve animationCurve__127838 = ((object.Equals(((global::Doroti.Generated.Framework.Animation.Animation<double>)this.animation).status, global::Doroti.Generated.Framework.Animation.AnimationStatus.forward)) ? Nav_barLibrary._kTopNavBarHeaderTransitionCurve : ((global::Doroti.Generated.Framework.Animation.Curve)Nav_barLibrary._kTopNavBarHeaderTransitionCurve).flipped);
            if (!this.searchable)
            {
                child__127790 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.FadeTransition(opacity: fadeInFrom(0.0, curve: animationCurve__127838), child: child__127790));
            }
            return ((global::Doroti.Generated.Framework.Widgets.Widget?)(object?)new global::Doroti.Generated.Framework.Widgets.PositionedTransition(rect: (this.userGestureInProgress ? this.routeAnimation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: global::Doroti.Generated.Framework.Animation.Curves.linear)).drive(positionTween__127641) : this.animation.drive(new global::Doroti.Generated.Framework.Animation.CurveTween(curve: animationCurve__127838)).drive(positionTween__127641)), child: new global::Doroti.Generated.Framework.Widgets.ClipRect(child: child__127790)));
            return default!;
        }
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Generated.Framework.Animation.RectTween _linearTranslateWithLargestRectSizeTween(Rect? begin, Rect? end)
    {
        var largestSize__128895 = new global::Doroti.Ui.Size(Math.Max(DartRuntimePrimitives.RequireValue(begin).size.width, DartRuntimePrimitives.RequireValue(end).size.width), Math.Max(DartRuntimePrimitives.RequireValue(begin).size.height, DartRuntimePrimitives.RequireValue(end).size.height));
        return new global::Doroti.Generated.Framework.Animation.RectTween(begin: (DartRuntimePrimitives.RequireValue(begin).topLeft & largestSize__128895), end: (DartRuntimePrimitives.RequireValue(end).topLeft & largestSize__128895));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Generated.Framework.Widgets.Widget _navBarHeroLaunchPadBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, Size heroSize, global::Doroti.Generated.Framework.Widgets.Widget child)
    {
        DartRuntimePrimitives.Assert(() => (child is _TransitionableNavigationBar__nav_bar));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Visibility(maintainSize: true, maintainAnimation: true, maintainState: true, visible: false, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Nav_barLibrary
{
    internal static global::Doroti.Generated.Framework.Widgets.Widget _navBarHeroFlightShuttleBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext flightContext, global::Doroti.Generated.Framework.Animation.Animation<double> animation, global::Doroti.Generated.Framework.Widgets.HeroFlightDirection flightDirection, global::Doroti.Generated.Framework.Widgets.BuildContext fromHeroContext, global::Doroti.Generated.Framework.Widgets.BuildContext toHeroContext)
    {
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Widgets.BuildContext)fromHeroContext).widget is global::Doroti.Generated.Framework.Widgets.Hero));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Widgets.BuildContext)toHeroContext).widget is global::Doroti.Generated.Framework.Widgets.Hero));
        var fromHeroWidget__130352 = ((global::Doroti.Generated.Framework.Widgets.Hero?)(object?)((global::Doroti.Generated.Framework.Widgets.BuildContext)fromHeroContext).widget)!;
        var toHeroWidget__130409 = ((global::Doroti.Generated.Framework.Widgets.Hero?)(object?)((global::Doroti.Generated.Framework.Widgets.BuildContext)toHeroContext).widget)!;
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Widgets.Hero)fromHeroWidget__130352).child is _TransitionableNavigationBar__nav_bar));
        DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Widgets.Hero)toHeroWidget__130409).child is _TransitionableNavigationBar__nav_bar));
        var fromNavBar__130590 = ((_TransitionableNavigationBar__nav_bar?)(object?)((global::Doroti.Generated.Framework.Widgets.Hero)fromHeroWidget__130352).child)!;
        var toNavBar__130665 = ((_TransitionableNavigationBar__nav_bar?)(object?)((global::Doroti.Generated.Framework.Widgets.Hero)toHeroWidget__130409).child)!;
        DartRuntimePrimitives.Assert(() => (((_TransitionableNavigationBar__nav_bar)fromNavBar__130590).componentsKeys.navBarBoxKey.currentContext!.owner is not null), () => (object?)"The from nav bar to Hero must have been mounted in the previous frame");
        DartRuntimePrimitives.Assert(() => (((_TransitionableNavigationBar__nav_bar)toNavBar__130665).componentsKeys.navBarBoxKey.currentContext!.owner is not null), () => (object?)"The to nav bar to Hero must have been mounted in the previous frame");
        switch (flightDirection)
        {
            case global::Doroti.Generated.Framework.Widgets.HeroFlightDirection.push:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _NavigationBarTransition__nav_bar(animation: animation, bottomNavBar: fromNavBar__130590, topNavBar: toNavBar__130665));
                }
            case global::Doroti.Generated.Framework.Widgets.HeroFlightDirection.pop:
                {
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _NavigationBarTransition__nav_bar(animation: animation, bottomNavBar: toNavBar__130665, topNavBar: fromNavBar__130590));
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
