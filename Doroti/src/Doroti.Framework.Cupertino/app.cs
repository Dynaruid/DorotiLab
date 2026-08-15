// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/cupertino/app.dart
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

public class CupertinoApp : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.NavigatorState>? navigatorKey { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? home { get; private set; }
    public virtual CupertinoThemeData? theme { get; private set; }
    public virtual DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>? routes { get; private set; }
    public virtual string? initialRoute { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>? onGenerateRoute { get; private set; }
    public virtual global::System.Func<string, List<dynamic>>? onGenerateInitialRoutes { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>? onUnknownRoute { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>? onNavigationNotification { get; private set; }
    public virtual List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver>? navigatorObservers { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual dynamic routeInformationParser { get; private set; } = default!;
    public virtual dynamic routerDelegate { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Widgets.BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.RouterConfig<object>? routerConfig { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder { get; private set; }
    public virtual string? title { get; private set; }
    public virtual global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>? onGenerateTitle { get; private set; }
    public virtual Color? color { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual IEnumerable<dynamic>? localizationsDelegates { get; private set; }
    public virtual global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback { get; private set; }
    public virtual global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback { get; private set; }
    public virtual IEnumerable<Locale> supportedLocales { get; private set; } = default!;
    public virtual bool showPerformanceOverlay { get; private set; } = default!;
    public virtual bool checkerboardRasterCacheImages { get; private set; } = default!;
    public virtual bool checkerboardOffscreenLayers { get; private set; } = default!;
    public virtual bool showSemanticsDebugger { get; private set; } = default!;
    public virtual bool debugShowCheckedModeBanner { get; private set; } = default!;
    public virtual DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent>? shortcuts { get; private set; }
    public virtual DartMap<Type, dynamic>? actions { get; private set; }
    public virtual string? restorationScopeId { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.ScrollBehavior? scrollBehavior { get; private set; }
    public virtual bool useInheritedMediaQuery { get; private set; } = default!;

    public CupertinoApp(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.NavigatorState>? navigatorKey = null, global::Doroti.Generated.Framework.Widgets.Widget? home = null, CupertinoThemeData? theme = null, DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>> routes = default!, string? initialRoute = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>? onGenerateRoute = null, global::System.Func<string, List<dynamic>>? onGenerateInitialRoutes = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>? onUnknownRoute = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>? onNavigationNotification = null, List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver> navigatorObservers = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, string? title = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>? onGenerateTitle = null, Color? color = null, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool showPerformanceOverlay = false, bool checkerboardRasterCacheImages = false, bool checkerboardOffscreenLayers = false, bool showSemanticsDebugger = false, bool debugShowCheckedModeBanner = true, DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, global::Doroti.Generated.Framework.Widgets.ScrollBehavior? scrollBehavior = null, bool useInheritedMediaQuery = false) : base(key: key)
    {
        DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>> __routes = routes ?? new DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>();
        List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver> __navigatorObservers = navigatorObservers ?? new List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver>();
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
        this.routeInformationProvider = null;
        this.routeInformationParser = null;
        this.routerDelegate = null;
        this.backButtonDispatcher = null;
        this.routerConfig = null;
    }

    public static CupertinoApp CreateRouter(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.RouteInformationProvider? routeInformationProvider = null, dynamic routeInformationParser = null, dynamic routerDelegate = null, global::Doroti.Generated.Framework.Widgets.BackButtonDispatcher? backButtonDispatcher = null, global::Doroti.Generated.Framework.Widgets.RouterConfig<object>? routerConfig = null, CupertinoThemeData? theme = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, string? title = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>? onGenerateTitle = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>? onNavigationNotification = null, Color? color = null, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool showPerformanceOverlay = false, bool checkerboardRasterCacheImages = false, bool checkerboardOffscreenLayers = false, bool showSemanticsDebugger = false, bool debugShowCheckedModeBanner = true, DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, global::Doroti.Generated.Framework.Widgets.ScrollBehavior? scrollBehavior = null, bool useInheritedMediaQuery = false)
    {
        var __instance = new CupertinoApp(key: key, theme: theme, routes: new DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>(), onNavigationNotification: onNavigationNotification, navigatorObservers: new List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver>(), builder: builder, title: title, onGenerateTitle: onGenerateTitle, color: color, locale: locale, localizationsDelegates: localizationsDelegates, localeListResolutionCallback: localeListResolutionCallback, localeResolutionCallback: localeResolutionCallback, supportedLocales: supportedLocales, showPerformanceOverlay: showPerformanceOverlay, checkerboardRasterCacheImages: checkerboardRasterCacheImages, checkerboardOffscreenLayers: checkerboardOffscreenLayers, showSemanticsDebugger: showSemanticsDebugger, debugShowCheckedModeBanner: debugShowCheckedModeBanner, shortcuts: shortcuts, actions: actions, restorationScopeId: restorationScopeId, scrollBehavior: scrollBehavior, useInheritedMediaQuery: useInheritedMediaQuery);
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
    public static global::Doroti.Generated.Framework.Widgets.HeroController createCupertinoHeroController() => new global::Doroti.Generated.Framework.Widgets.HeroController();
}

public class CupertinoScrollBehavior : global::Doroti.Generated.Framework.Widgets.ScrollBehavior
{
    public CupertinoScrollBehavior()
    {
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildScrollbar(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Widgets.ScrollableDetails details)
    {
        switch (getPlatform(context))
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Widgets.ScrollableDetails)details).controller is not null));
                    return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new CupertinoScrollbar(controller: ((global::Doroti.Generated.Framework.Widgets.ScrollableDetails)details).controller, child: child));
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
                {
                    return child;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildOverscrollIndicator(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Widgets.ScrollableDetails details)
    {
        return child;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.ScrollPhysics getScrollPhysics(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        if ((object.Equals(getPlatform(context), global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS)))
        {
            return ((global::Doroti.Generated.Framework.Widgets.ScrollPhysics)(object?)new global::Doroti.Generated.Framework.Widgets.BouncingScrollPhysics(decelerationRate: global::Doroti.Generated.Framework.Widgets.ScrollDecelerationRate.fast));
        }
        return ((global::Doroti.Generated.Framework.Widgets.ScrollPhysics)(object?)new global::Doroti.Generated.Framework.Widgets.BouncingScrollPhysics());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Gestures.MultitouchDragStrategy getMultitouchDragStrategy(global::Doroti.Generated.Framework.Widgets.BuildContext context) => global::Doroti.Generated.Framework.Gestures.MultitouchDragStrategy.averageBoundaryPointers;
}

internal class _CupertinoAppState__app : global::Doroti.Generated.Framework.Widgets.State<CupertinoApp>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.HeroController _heroController { get; set; } = default!;

    internal virtual bool _usesRouter => DartRuntimePrimitives.ConvertValue<bool>(((((CupertinoApp)this.widget).routerDelegate is not null) || (((CupertinoApp)this.widget).routerConfig is not null)));
    public override void initState()
    {
        base.initState();
        _heroController = CupertinoApp.createCupertinoHeroController();
    }

    public override void dispose()
    {
        this._heroController.dispose();
        base.dispose();
    }

    internal virtual IEnumerable<object> _localizationsDelegates
    {
        get
        {
            return ((IEnumerable<object>)(object?)((Func<List<object>>)(() => { var __collection18903 = new List<object>(); var __collectionSpread18943 = ((CupertinoApp)this.widget).localizationsDelegates; if (__collectionSpread18943 is not null) { __collection18903.AddRange(__collectionSpread18943); } __collection18903.Add(DefaultCupertinoLocalizations.@delegate); return __collection18903; }))());
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _exitWidgetSelectionButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key, global::System.Action onPressed, string semanticsLabel)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _CupertinoInspectorButton__app(onPressed: () => onPressed(), semanticsLabel: semanticsLabel, icon: CupertinoIcons.xmark, buttonKey: key));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _moveExitWidgetSelectionButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, string semanticsLabel, bool usesDefaultAlignment = true)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_CupertinoInspectorButton__app.CreateIconOnly(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: (usesDefaultAlignment ? CupertinoIcons.arrow_right : CupertinoIcons.arrow_left)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _tapBehaviorButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, bool selectionOnTapEnabled, string semanticsLabel)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_CupertinoInspectorButton__app.CreateToggle(onPressed: () => onPressed(), semanticsLabel: semanticsLabel, icon: new global::Doroti.Generated.Framework.Widgets.IconData(128842L), toggledOn: selectionOnTapEnabled));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.WidgetsApp _buildWidgetApp(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CupertinoThemeData effectiveThemeData__20419 = CupertinoTheme.of(context);
        global::Doroti.Ui.Color color__20484 = ((global::Doroti.Ui.Color)(object?)CupertinoDynamicColor.resolve((((CupertinoApp)this.widget).color ?? effectiveThemeData__20419.primaryColor), context));
        if (this._usesRouter)
        {
            return global::Doroti.Generated.Framework.Widgets.WidgetsApp.CreateRouter(key: new global::Doroti.Generated.Framework.Widgets.GlobalObjectKey<IState>(this), routeInformationProvider: ((CupertinoApp)this.widget).routeInformationProvider, routeInformationParser: ((CupertinoApp)this.widget).routeInformationParser, routerDelegate: ((CupertinoApp)this.widget).routerDelegate, routerConfig: ((CupertinoApp)this.widget).routerConfig, backButtonDispatcher: ((CupertinoApp)this.widget).backButtonDispatcher, onNavigationNotification: (global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>?)((CupertinoApp)this.widget).onNavigationNotification, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>?)((CupertinoApp)this.widget).builder, title: ((CupertinoApp)this.widget).title, onGenerateTitle: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>?)((CupertinoApp)this.widget).onGenerateTitle, textStyle: effectiveThemeData__20419.textTheme.textStyle, color: color__20484, locale: ((CupertinoApp)this.widget).locale, localizationsDelegates: this._localizationsDelegates.Cast<dynamic>(), localeResolutionCallback: (global::System.Func<Locale?, IEnumerable<Locale>, Locale?>?)((CupertinoApp)this.widget).localeResolutionCallback, localeListResolutionCallback: (global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>?)((CupertinoApp)this.widget).localeListResolutionCallback, supportedLocales: ((CupertinoApp)this.widget).supportedLocales.Cast<Locale>(), showPerformanceOverlay: ((CupertinoApp)this.widget).showPerformanceOverlay, showSemanticsDebugger: ((CupertinoApp)this.widget).showSemanticsDebugger, debugShowCheckedModeBanner: ((CupertinoApp)this.widget).debugShowCheckedModeBanner, exitWidgetSelectionButtonBuilder: (ExitWidgetSelectionButtonBuilder)this._exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder: (MoveExitWidgetSelectionButtonBuilder)this._moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder: (TapBehaviorButtonBuilder)this._tapBehaviorButtonBuilder, shortcuts: ((CupertinoApp)this.widget).shortcuts, actions: ((CupertinoApp)this.widget).actions, restorationScopeId: ((CupertinoApp)this.widget).restorationScopeId);
        }
        return new global::Doroti.Generated.Framework.Widgets.WidgetsApp(key: new global::Doroti.Generated.Framework.Widgets.GlobalObjectKey<IState>(this), navigatorKey: ((CupertinoApp)this.widget).navigatorKey, navigatorObservers: ((CupertinoApp)this.widget).navigatorObservers!, pageRouteBuilder: ((PageRouteFactory)((settings, builder) => {
return new CupertinoPageRoute<object>(settings: settings, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)builder);
throw new InvalidOperationException("Dart closure completed without a value.");
})), home: ((CupertinoApp)this.widget).home, routes: ((CupertinoApp)this.widget).routes!, initialRoute: ((CupertinoApp)this.widget).initialRoute, onGenerateRoute: (global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>?)((CupertinoApp)this.widget).onGenerateRoute, onGenerateInitialRoutes: (global::System.Func<string, List<dynamic>>?)((CupertinoApp)this.widget).onGenerateInitialRoutes, onUnknownRoute: (global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>?)((CupertinoApp)this.widget).onUnknownRoute, onNavigationNotification: (global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>?)((CupertinoApp)this.widget).onNavigationNotification, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>?)((CupertinoApp)this.widget).builder, title: ((CupertinoApp)this.widget).title, onGenerateTitle: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>?)((CupertinoApp)this.widget).onGenerateTitle, textStyle: effectiveThemeData__20419.textTheme.textStyle, color: color__20484, locale: ((CupertinoApp)this.widget).locale, localizationsDelegates: this._localizationsDelegates.Cast<dynamic>(), localeResolutionCallback: (global::System.Func<Locale?, IEnumerable<Locale>, Locale?>?)((CupertinoApp)this.widget).localeResolutionCallback, localeListResolutionCallback: (global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>?)((CupertinoApp)this.widget).localeListResolutionCallback, supportedLocales: ((CupertinoApp)this.widget).supportedLocales.Cast<Locale>(), showPerformanceOverlay: ((CupertinoApp)this.widget).showPerformanceOverlay, showSemanticsDebugger: ((CupertinoApp)this.widget).showSemanticsDebugger, debugShowCheckedModeBanner: ((CupertinoApp)this.widget).debugShowCheckedModeBanner, exitWidgetSelectionButtonBuilder: (ExitWidgetSelectionButtonBuilder)this._exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder: (MoveExitWidgetSelectionButtonBuilder)this._moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder: (TapBehaviorButtonBuilder)this._tapBehaviorButtonBuilder, shortcuts: ((CupertinoApp)this.widget).shortcuts, actions: ((CupertinoApp)this.widget).actions, restorationScopeId: ((CupertinoApp)this.widget).restorationScopeId);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        CupertinoThemeData effectiveThemeData__23770 = ((((CupertinoApp)this.widget).theme ?? new CupertinoThemeData())).resolveFrom(context);
        global::Doroti.Ui.Brightness brightness__23963 = ((effectiveThemeData__23770.brightness ?? (Brightness)MediaQuery.platformBrightnessOf(context)));
        SystemChrome.setSystemUIOverlayStyle(((object.Equals(brightness__23963, Brightness.dark)) ? global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle.light : global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle.dark));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ScrollConfiguration(behavior: (((CupertinoApp)this.widget).scrollBehavior ?? new CupertinoScrollBehavior()), child: new CupertinoUserInterfaceLevel(data: CupertinoUserInterfaceLevelData.@base, child: new CupertinoTheme(data: effectiveThemeData__23770, child: new global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle(selectionColor: effectiveThemeData__23770.primaryColor.withOpacity(0.2), cursorColor: effectiveThemeData__23770.primaryColor, child: new global::Doroti.Generated.Framework.Widgets.HeroControllerScope(controller: this._heroController, child: new global::Doroti.Generated.Framework.Widgets.Builder(builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.WidgetsApp>)this._buildWidgetApp)))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _CupertinoInspectorButton__app : global::Doroti.Generated.Framework.Widgets.InspectorButton
{
    internal _CupertinoInspectorButton__app(global::System.Action onPressed, string semanticsLabel, global::Doroti.Generated.Framework.Widgets.IconData icon, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>? buttonKey = null) : base(onPressed, semanticsLabel, icon, buttonKey)
    {
    }

    internal static _CupertinoInspectorButton__app CreateToggle(global::System.Action onPressed, string semanticsLabel, global::Doroti.Generated.Framework.Widgets.IconData icon, bool toggledOn = true)
    {
        var __instance = new _CupertinoInspectorButton__app(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: icon);
        return __instance;
    }

    internal static _CupertinoInspectorButton__app CreateIconOnly(global::System.Action onPressed, string semanticsLabel, global::Doroti.Generated.Framework.Widgets.IconData icon)
    {
        var __instance = new _CupertinoInspectorButton__app(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: icon);
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        var buttonIcon__25484 = new global::Doroti.Generated.Framework.Widgets.Icon(this.icon, semanticLabel: this.semanticsLabel, size: this.iconSizeForVariant, color: foregroundColor(context));
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.Padding(key: this.buttonKey, padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.CreateAll((((ConstantsLibrary.kMinInteractiveDimensionCupertino - global::Doroti.Generated.Framework.Widgets.InspectorButton.buttonSize)) / 2L)), child: (((object.Equals(this.variant, global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.toggle)) && !DartRuntimePrimitives.RequireValue(this.toggledOn)) ? new CupertinoButton(minSize: global::Doroti.Generated.Framework.Widgets.InspectorButton.buttonSize, onPressed: this.onPressed, padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, child: buttonIcon__25484) : new CupertinoButton(minSize: global::Doroti.Generated.Framework.Widgets.InspectorButton.buttonSize, onPressed: this.onPressed, padding: global::Doroti.Generated.Framework.Painting.EdgeInsets.zero, color: backgroundColor(context), child: buttonIcon__25484))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color foregroundColor(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color primaryColor__26415 = ((global::Doroti.Ui.Color)(object?)CupertinoTheme.of(context).primaryColor);
        global::Doroti.Ui.Color secondaryColor__26487 = ((global::Doroti.Ui.Color)(object?)CupertinoTheme.of(context).primaryContrastingColor);
        switch (this.variant)
        {
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.filled:
                {
                    return secondaryColor__26487;
                }
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.iconOnly:
                {
                    return primaryColor__26415;
                }
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.toggle:
                {
                    return (!DartRuntimePrimitives.RequireValue(this.toggledOn) ? primaryColor__26415 : secondaryColor__26487);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color backgroundColor(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color primaryColor__26914 = ((global::Doroti.Ui.Color)(object?)CupertinoTheme.of(context).primaryColor);
        switch (this.variant)
        {
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.filled:
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.toggle:
                {
                    return primaryColor__26914;
                }
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.iconOnly:
                {
                    return new global::Doroti.Ui.Color(0L);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
