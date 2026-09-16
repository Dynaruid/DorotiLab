// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/app.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Cupertino;

public class CupertinoApp : StatefulWidget
{
    public virtual GlobalKey<NavigatorState>? navigatorKey { get; private set; }
    public virtual Widget? home { get; private set; }
    public virtual CupertinoThemeData? theme { get; private set; }
    public virtual DartMap<string, Func<BuildContext, Widget>>? routes { get; private set; }
    public virtual string? initialRoute { get; private set; }
    public virtual Func<RouteSettings, dynamic>? onGenerateRoute { get; private set; }
    public virtual Func<string, List<dynamic>>? onGenerateInitialRoutes { get; private set; }
    public virtual Func<RouteSettings, dynamic>? onUnknownRoute { get; private set; }
    public virtual Func<NavigationNotification, bool>? onNavigationNotification { get; private set; }
    public virtual List<NavigatorObserver>? navigatorObservers { get; private set; }
    public virtual RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual object? routeInformationParser { get; private set; } = default!;
    public virtual IRouterDelegate? routerDelegate { get; private set; } = default!;
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual IRouterConfig? routerConfig { get; private set; }
    public virtual Func<BuildContext, Widget?, Widget>? builder { get; private set; }
    public virtual string? title { get; private set; }
    public virtual Func<BuildContext, string>? onGenerateTitle { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual IEnumerable<dynamic>? localizationsDelegates { get; private set; }
    public virtual Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback { get; private set; }
    public virtual Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback { get; private set; }
    public virtual IEnumerable<Locale> supportedLocales { get; private set; } = default!;
    public virtual bool showPerformanceOverlay { get; private set; } = default!;
    public virtual bool checkerboardRasterCacheImages { get; private set; } = default!;
    public virtual bool checkerboardOffscreenLayers { get; private set; } = default!;
    public virtual bool showSemanticsDebugger { get; private set; } = default!;
    public virtual bool debugShowCheckedModeBanner { get; private set; } = default!;
    public virtual DartMap<ShortcutActivator, Intent>? shortcuts { get; private set; }
    public virtual DartMap<Type, dynamic>? actions { get; private set; }
    public virtual string? restorationScopeId { get; private set; }
    public virtual ScrollBehavior? scrollBehavior { get; private set; }
    public virtual bool useInheritedMediaQuery { get; private set; } = default!;

    public CupertinoApp(Key? key = null, GlobalKey<NavigatorState>? navigatorKey = null, Widget? home = null, CupertinoThemeData? theme = null, DartMap<string, Func<BuildContext, Widget>> routes = default!, string? initialRoute = null, Func<RouteSettings, dynamic>? onGenerateRoute = null, Func<string, List<dynamic>>? onGenerateInitialRoutes = null, Func<RouteSettings, dynamic>? onUnknownRoute = null, Func<NavigationNotification, bool>? onNavigationNotification = null, List<NavigatorObserver> navigatorObservers = default!, Func<BuildContext, Widget?, Widget>? builder = null, string? title = null, Func<BuildContext, string>? onGenerateTitle = null, Color? color = null, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool showPerformanceOverlay = false, bool checkerboardRasterCacheImages = false, bool checkerboardOffscreenLayers = false, bool showSemanticsDebugger = false, bool debugShowCheckedModeBanner = true, DartMap<ShortcutActivator, Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, ScrollBehavior? scrollBehavior = null, bool useInheritedMediaQuery = false) : base(key: key)
    {
        DartMap<string, Func<BuildContext, Widget>> __routes = routes ?? new DartMap<string, Func<BuildContext, Widget>>();
        List<NavigatorObserver> __navigatorObservers = navigatorObservers ?? new List<NavigatorObserver>();
        IEnumerable<Locale> __supportedLocales = supportedLocales ?? new List<Locale> { new Locale("en", "US") };
        this.navigatorKey = navigatorKey;
        this.home = home;
        this.theme = theme;
        this.routes = __routes;
        this.initialRoute = initialRoute;
        this.onGenerateRoute = onGenerateRoute;
        this.onGenerateInitialRoutes = onGenerateInitialRoutes;
        this.onUnknownRoute = onUnknownRoute;
        this.onNavigationNotification = onNavigationNotification;
        this.navigatorObservers = __navigatorObservers;
        this.builder = builder;
        this.title = title;
        this.onGenerateTitle = onGenerateTitle;
        this.color = color;
        this.locale = locale;
        this.localizationsDelegates = localizationsDelegates;
        this.localeListResolutionCallback = localeListResolutionCallback;
        this.localeResolutionCallback = localeResolutionCallback;
        this.supportedLocales = __supportedLocales;
        this.showPerformanceOverlay = showPerformanceOverlay;
        this.checkerboardRasterCacheImages = checkerboardRasterCacheImages;
        this.checkerboardOffscreenLayers = checkerboardOffscreenLayers;
        this.showSemanticsDebugger = showSemanticsDebugger;
        this.debugShowCheckedModeBanner = debugShowCheckedModeBanner;
        this.shortcuts = shortcuts;
        this.actions = actions;
        this.restorationScopeId = restorationScopeId;
        this.scrollBehavior = scrollBehavior;
        this.useInheritedMediaQuery = useInheritedMediaQuery;
        routeInformationProvider = null;
        routeInformationParser = null;
        routerDelegate = null;
        backButtonDispatcher = null;
        routerConfig = null;
    }

    public static CupertinoApp CreateRouter(Key? key = null, RouteInformationProvider? routeInformationProvider = null, object? routeInformationParser = null, IRouterDelegate? routerDelegate = null, BackButtonDispatcher? backButtonDispatcher = null, IRouterConfig? routerConfig = null, CupertinoThemeData? theme = null, Func<BuildContext, Widget?, Widget>? builder = null, string? title = null, Func<BuildContext, string>? onGenerateTitle = null, Func<NavigationNotification, bool>? onNavigationNotification = null, Color? color = null, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool showPerformanceOverlay = false, bool checkerboardRasterCacheImages = false, bool checkerboardOffscreenLayers = false, bool showSemanticsDebugger = false, bool debugShowCheckedModeBanner = true, DartMap<ShortcutActivator, Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, ScrollBehavior? scrollBehavior = null, bool useInheritedMediaQuery = false)
    {
        var __instance = new CupertinoApp(key: key, theme: theme, routes: new DartMap<string, Func<BuildContext, Widget>>(), onNavigationNotification: onNavigationNotification, navigatorObservers: new List<NavigatorObserver>(), builder: builder, title: title, onGenerateTitle: onGenerateTitle, color: color, locale: locale, localizationsDelegates: localizationsDelegates, localeListResolutionCallback: localeListResolutionCallback, localeResolutionCallback: localeResolutionCallback, supportedLocales: supportedLocales, showPerformanceOverlay: showPerformanceOverlay, checkerboardRasterCacheImages: checkerboardRasterCacheImages, checkerboardOffscreenLayers: checkerboardOffscreenLayers, showSemanticsDebugger: showSemanticsDebugger, debugShowCheckedModeBanner: debugShowCheckedModeBanner, shortcuts: shortcuts, actions: actions, restorationScopeId: restorationScopeId, scrollBehavior: scrollBehavior, useInheritedMediaQuery: useInheritedMediaQuery);
        IEnumerable<Locale> __supportedLocales = supportedLocales ?? new List<Locale> { new Locale("en", "US") };
        __instance.routeInformationProvider = routeInformationProvider;
        __instance.routeInformationParser = routeInformationParser;
        __instance.routerDelegate = routerDelegate;
        __instance.backButtonDispatcher = backButtonDispatcher;
        __instance.routerConfig = routerConfig;
        __instance.theme = theme;
        __instance.builder = builder;
        __instance.title = title;
        __instance.onGenerateTitle = onGenerateTitle;
        __instance.onNavigationNotification = onNavigationNotification;
        __instance.color = color;
        __instance.locale = locale;
        __instance.localizationsDelegates = localizationsDelegates;
        __instance.localeListResolutionCallback = localeListResolutionCallback;
        __instance.localeResolutionCallback = localeResolutionCallback;
        __instance.supportedLocales = __supportedLocales;
        __instance.showPerformanceOverlay = showPerformanceOverlay;
        __instance.checkerboardRasterCacheImages = checkerboardRasterCacheImages;
        __instance.checkerboardOffscreenLayers = checkerboardOffscreenLayers;
        __instance.showSemanticsDebugger = showSemanticsDebugger;
        __instance.debugShowCheckedModeBanner = debugShowCheckedModeBanner;
        __instance.shortcuts = shortcuts;
        __instance.actions = actions;
        __instance.restorationScopeId = restorationScopeId;
        __instance.scrollBehavior = scrollBehavior;
        __instance.useInheritedMediaQuery = useInheritedMediaQuery;
        __instance.navigatorObservers = null;
        __instance.navigatorKey = null;
        __instance.onGenerateRoute = null;
        __instance.home = null;
        __instance.onGenerateInitialRoutes = null;
        __instance.onUnknownRoute = null;
        __instance.routes = null;
        __instance.initialRoute = null;
        return __instance;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _CupertinoAppState__app());
    public static HeroController createCupertinoHeroController() => new HeroController();
}

public class CupertinoScrollBehavior : ScrollBehavior
{
    public CupertinoScrollBehavior()
    {
    }

    public override Widget buildScrollbar(BuildContext context, Widget child, ScrollableDetails details)
    {
        switch (getPlatform(context))
        {
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
                {
                    DartRuntimePrimitives.Assert(() => details.controller is not null);
                    return new CupertinoScrollbar(controller: details.controller, child: child);
                }
            case TargetPlatform.android:
            case TargetPlatform.fuchsia:
            case TargetPlatform.iOS:
                {
                    return child;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildOverscrollIndicator(BuildContext context, Widget child, ScrollableDetails details)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override ScrollPhysics getScrollPhysics(BuildContext context)
    {
        if (Equals(getPlatform(context), TargetPlatform.macOS))
        {
            return new BouncingScrollPhysics(decelerationRate: ScrollDecelerationRate.fast);
        }
        return new BouncingScrollPhysics();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Gestures.MultitouchDragStrategy getMultitouchDragStrategy(BuildContext context) => Gestures.MultitouchDragStrategy.averageBoundaryPointers;
}

internal class _CupertinoAppState__app : State<CupertinoApp>
{
    internal virtual HeroController _heroController { get; set; } = default!;

    internal virtual bool _usesRouter => DartRuntimePrimitives.ConvertValue<bool>((widget.routerDelegate is not null) || (widget.routerConfig is not null));
    public override void initState()
    {
        base.initState();
        _heroController = CupertinoApp.createCupertinoHeroController();
    }

    public override void dispose()
    {
        _heroController.dispose();
        base.dispose();
    }

    internal virtual IEnumerable<object> _localizationsDelegates
    {
        get
        {
            return ((Func<List<object>>)(() => { var __collection18903 = new List<object>(); var __collectionSpread18943 = widget.localizationsDelegates; if (__collectionSpread18943 is not null) { __collection18903.AddRange(__collectionSpread18943); } __collection18903.Add(DefaultCupertinoLocalizations.@delegate); return __collection18903; }))();
        }
    }
    internal virtual Widget _exitWidgetSelectionButtonBuilder(BuildContext context, GlobalKey<IState> key, Action onPressed, string semanticsLabel)
    {
        return new _CupertinoInspectorButton__app(onPressed: () => onPressed(), semanticsLabel: semanticsLabel, icon: CupertinoIcons.xmark, buttonKey: key);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _moveExitWidgetSelectionButtonBuilder(BuildContext context, Action onPressed, string semanticsLabel, bool usesDefaultAlignment = true)
    {
        return _CupertinoInspectorButton__app.CreateIconOnly(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: usesDefaultAlignment ? CupertinoIcons.arrow_right : CupertinoIcons.arrow_left);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _tapBehaviorButtonBuilder(BuildContext context, Action onPressed, bool selectionOnTapEnabled, string semanticsLabel)
    {
        return _CupertinoInspectorButton__app.CreateToggle(onPressed: () => onPressed(), semanticsLabel: semanticsLabel, icon: new IconData(128842L), toggledOn: selectionOnTapEnabled);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual WidgetsApp _buildWidgetApp(BuildContext context)
    {
        CupertinoThemeData effectiveThemeData = CupertinoTheme.of(context);
        Color colorLocal = CupertinoDynamicColor.resolve(widget.color ?? effectiveThemeData.primaryColor, context);
        if (_usesRouter)
        {
            return WidgetsApp.CreateRouter(key: new GlobalObjectKey<IState>(this), routeInformationProvider: widget.routeInformationProvider, routeInformationParser: widget.routeInformationParser, routerDelegate: widget.routerDelegate, routerConfig: widget.routerConfig, backButtonDispatcher: widget.backButtonDispatcher, onNavigationNotification: widget.onNavigationNotification, builder: widget.builder, title: widget.title, onGenerateTitle: widget.onGenerateTitle, textStyle: effectiveThemeData.textTheme.textStyle, color: colorLocal, locale: widget.locale, localizationsDelegates: _localizationsDelegates.Cast<dynamic>(), localeResolutionCallback: widget.localeResolutionCallback, localeListResolutionCallback: widget.localeListResolutionCallback, supportedLocales: widget.supportedLocales.Cast<Locale>(), showPerformanceOverlay: widget.showPerformanceOverlay, showSemanticsDebugger: widget.showSemanticsDebugger, debugShowCheckedModeBanner: widget.debugShowCheckedModeBanner, exitWidgetSelectionButtonBuilder: _exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder: _moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder: _tapBehaviorButtonBuilder, shortcuts: widget.shortcuts, actions: widget.actions, restorationScopeId: widget.restorationScopeId);
        }
        return new WidgetsApp(key: new GlobalObjectKey<IState>(this), navigatorKey: widget.navigatorKey, navigatorObservers: widget.navigatorObservers!, pageRouteBuilder: (settings, builder) =>
        {
            return new CupertinoPageRoute<object>(settings: settings, builder: builder);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, home: widget.home, routes: widget.routes!, initialRoute: widget.initialRoute, onGenerateRoute: widget.onGenerateRoute, onGenerateInitialRoutes: widget.onGenerateInitialRoutes, onUnknownRoute: widget.onUnknownRoute, onNavigationNotification: widget.onNavigationNotification, builder: widget.builder, title: widget.title, onGenerateTitle: widget.onGenerateTitle, textStyle: effectiveThemeData.textTheme.textStyle, color: colorLocal, locale: widget.locale, localizationsDelegates: _localizationsDelegates.Cast<dynamic>(), localeResolutionCallback: widget.localeResolutionCallback, localeListResolutionCallback: widget.localeListResolutionCallback, supportedLocales: widget.supportedLocales.Cast<Locale>(), showPerformanceOverlay: widget.showPerformanceOverlay, showSemanticsDebugger: widget.showSemanticsDebugger, debugShowCheckedModeBanner: widget.debugShowCheckedModeBanner, exitWidgetSelectionButtonBuilder: _exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder: _moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder: _tapBehaviorButtonBuilder, shortcuts: widget.shortcuts, actions: widget.actions, restorationScopeId: widget.restorationScopeId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        CupertinoThemeData effectiveThemeData = (widget.theme ?? new CupertinoThemeData()).resolveFrom(context);
        Brightness brightnessLocal = effectiveThemeData.brightness ?? MediaQuery.platformBrightnessOf(context);
        SystemChrome.setSystemUIOverlayStyle(Equals(brightnessLocal, Brightness.dark) ? SystemUiOverlayStyle.light : SystemUiOverlayStyle.dark);
        return new ScrollConfiguration(behavior: widget.scrollBehavior ?? new CupertinoScrollBehavior(), child: new CupertinoUserInterfaceLevel(data: CupertinoUserInterfaceLevelData.@base, child: new CupertinoTheme(data: effectiveThemeData, child: new DefaultSelectionStyle(selectionColor: effectiveThemeData.primaryColor.withOpacity(0.2), cursorColor: effectiveThemeData.primaryColor, child: new HeroControllerScope(controller: _heroController, child: new Builder(builder: (Func<BuildContext, WidgetsApp>)_buildWidgetApp))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoInspectorButton__app : InspectorButton
{
    internal _CupertinoInspectorButton__app(Action onPressed, string semanticsLabel, IconData icon, GlobalKey<IState>? buttonKey = null) : base(onPressed, semanticsLabel, icon, buttonKey)
    {
    }

    internal static _CupertinoInspectorButton__app CreateToggle(Action onPressed, string semanticsLabel, IconData icon, bool toggledOn = true)
    {
        var __instance = new _CupertinoInspectorButton__app(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: icon);
        return __instance;
    }

    internal static _CupertinoInspectorButton__app CreateIconOnly(Action onPressed, string semanticsLabel, IconData icon)
    {
        var __instance = new _CupertinoInspectorButton__app(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: icon);
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        var buttonIcon = new Icon(icon, semanticLabel: semanticsLabel, size: iconSizeForVariant, color: foregroundColor(context));
        return new Padding(key: buttonKey, padding: EdgeInsets.CreateAll((ConstantsLibrary.kMinInteractiveDimensionCupertino - buttonSize) / 2L), child: (Equals(variant, InspectorButtonVariant.toggle) && !DartRuntimePrimitives.RequireValue(toggledOn)) ? new CupertinoButton(minSize: buttonSize, onPressed: onPressed, padding: EdgeInsets.zero, child: buttonIcon) : new CupertinoButton(minSize: buttonSize, onPressed: onPressed, padding: EdgeInsets.zero, color: backgroundColor(context), child: buttonIcon));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color foregroundColor(BuildContext context)
    {
        Color primaryColorLocal = CupertinoTheme.of(context).primaryColor;
        Color secondaryColor = CupertinoTheme.of(context).primaryContrastingColor;
        switch (variant)
        {
            case InspectorButtonVariant.filled:
                {
                    return secondaryColor;
                }
            case InspectorButtonVariant.iconOnly:
                {
                    return primaryColorLocal;
                }
            case InspectorButtonVariant.toggle:
                {
                    return !DartRuntimePrimitives.RequireValue(toggledOn) ? primaryColorLocal : secondaryColor;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color backgroundColor(BuildContext context)
    {
        Color primaryColorLocal = CupertinoTheme.of(context).primaryColor;
        switch (variant)
        {
            case InspectorButtonVariant.filled:
            case InspectorButtonVariant.toggle:
                {
                    return primaryColorLocal;
                }
            case InspectorButtonVariant.iconOnly:
                {
                    return new Color(0L);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
