// <doroti-reviewed-product-source milestone="G6-3" />
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
// Doroti typed semantic compiler 3.0.0; source: ../../../flutter-master/packages/flutter/lib/src/material/app.dart
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Material;

public static partial class AppLibrary
{
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _errorTextStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(color: new global::Doroti.Ui.Color(3506372608L), fontFamily: "monospace", fontSize: 48.0, fontWeight: FontWeight.w900, decoration: TextDecoration.underline, decorationColor: new global::Doroti.Ui.Color(4294967040L), decorationStyle: TextDecorationStyle.doubleLine, debugLabel: "fallback style; consider putting your text in a Material");
}

public enum ThemeMode
{
    system,
    light,
    dark
}

public static class ThemeModeMembers
{
    public static bool isSystem(this ThemeMode value) => (object.Equals(value, ThemeMode.system));
    public static bool isLight(this ThemeMode value) => (object.Equals(value, ThemeMode.light));
    public static bool isDark(this ThemeMode value) => (object.Equals(value, ThemeMode.dark));
}

public class MaterialApp : global::Doroti.Generated.Framework.Widgets.StatefulWidget
{
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.NavigatorState>? navigatorKey { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.GlobalKey<ScaffoldMessengerState>? scaffoldMessengerKey { get; private set; }
    public virtual global::Doroti.Generated.Framework.Widgets.Widget? home { get; private set; }
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
    public virtual ThemeData? theme { get; private set; }
    public virtual ThemeData? darkTheme { get; private set; }
    public virtual ThemeData? highContrastTheme { get; private set; }
    public virtual ThemeData? highContrastDarkTheme { get; private set; }
    public virtual ThemeMode? themeMode { get; private set; }
    public virtual Duration themeAnimationDuration { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.Curve themeAnimationCurve { get; private set; } = default!;
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
    public virtual bool debugShowMaterialGrid { get; private set; } = default!;
    public virtual bool useInheritedMediaQuery { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Animation.AnimationStyle? themeAnimationStyle { get; private set; }

    public MaterialApp(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.GlobalKey<global::Doroti.Generated.Framework.Widgets.NavigatorState>? navigatorKey = null, global::Doroti.Generated.Framework.Widgets.GlobalKey<ScaffoldMessengerState>? scaffoldMessengerKey = null, global::Doroti.Generated.Framework.Widgets.Widget? home = null, DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>> routes = default!, string? initialRoute = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>? onGenerateRoute = null, global::System.Func<string, List<dynamic>>? onGenerateInitialRoutes = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>? onUnknownRoute = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>? onNavigationNotification = null, List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver> navigatorObservers = default!, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, string? title = "", global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>? onGenerateTitle = null, Color? color = null, ThemeData? theme = null, ThemeData? darkTheme = null, ThemeData? highContrastTheme = null, ThemeData? highContrastDarkTheme = null, ThemeMode? themeMode = ThemeMode.system, Duration? themeAnimationDuration = null, global::Doroti.Generated.Framework.Animation.Curve themeAnimationCurve = default!, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool debugShowMaterialGrid = false, bool showPerformanceOverlay = false, bool checkerboardRasterCacheImages = false, bool checkerboardOffscreenLayers = false, bool showSemanticsDebugger = false, bool debugShowCheckedModeBanner = true, DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, global::Doroti.Generated.Framework.Widgets.ScrollBehavior? scrollBehavior = null, bool useInheritedMediaQuery = false, global::Doroti.Generated.Framework.Animation.AnimationStyle? themeAnimationStyle = null) : base(key: key)
    {
        DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>> __routes = routes ?? new DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>();
        List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver> __navigatorObservers = navigatorObservers ?? new List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver>();
        Duration __themeAnimationDuration = themeAnimationDuration ?? ThemeLibrary.kThemeAnimationDuration;
        global::Doroti.Generated.Framework.Animation.Curve __themeAnimationCurve = themeAnimationCurve ?? global::Doroti.Generated.Framework.Animation.Curves.linear;
        IEnumerable<Locale> __supportedLocales = supportedLocales ?? new List<Locale> { new Locale("en", "US") };
        this.navigatorKey = navigatorKey;
        this.scaffoldMessengerKey = scaffoldMessengerKey;
        this.home = home;
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
        this.theme = theme;
        this.darkTheme = darkTheme;
        this.highContrastTheme = highContrastTheme;
        this.highContrastDarkTheme = highContrastDarkTheme;
        this.themeMode = themeMode;
        this.themeAnimationDuration = __themeAnimationDuration;
        this.themeAnimationCurve = __themeAnimationCurve;
        this.locale = locale;
        this.localizationsDelegates = localizationsDelegates;
        this.localeListResolutionCallback = localeListResolutionCallback;
        this.localeResolutionCallback = localeResolutionCallback;
        this.supportedLocales = __supportedLocales;
        this.debugShowMaterialGrid = debugShowMaterialGrid;
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
        this.themeAnimationStyle = themeAnimationStyle;
        this.routeInformationProvider = null;
        this.routeInformationParser = null;
        this.routerDelegate = null;
        this.backButtonDispatcher = null;
        this.routerConfig = null;
    }

    public static MaterialApp CreateRouter(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::Doroti.Generated.Framework.Widgets.GlobalKey<ScaffoldMessengerState>? scaffoldMessengerKey = null, global::Doroti.Generated.Framework.Widgets.RouteInformationProvider? routeInformationProvider = null, dynamic routeInformationParser = null, dynamic routerDelegate = null, global::Doroti.Generated.Framework.Widgets.RouterConfig<object>? routerConfig = null, global::Doroti.Generated.Framework.Widgets.BackButtonDispatcher? backButtonDispatcher = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>? builder = null, string? title = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>? onGenerateTitle = null, global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>? onNavigationNotification = null, Color? color = null, ThemeData? theme = null, ThemeData? darkTheme = null, ThemeData? highContrastTheme = null, ThemeData? highContrastDarkTheme = null, ThemeMode? themeMode = ThemeMode.system, Duration? themeAnimationDuration = null, global::Doroti.Generated.Framework.Animation.Curve themeAnimationCurve = default!, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool debugShowMaterialGrid = false, bool showPerformanceOverlay = false, bool checkerboardRasterCacheImages = false, bool checkerboardOffscreenLayers = false, bool showSemanticsDebugger = false, bool debugShowCheckedModeBanner = true, DartMap<global::Doroti.Generated.Framework.Widgets.ShortcutActivator, global::Doroti.Generated.Framework.Widgets.Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, global::Doroti.Generated.Framework.Widgets.ScrollBehavior? scrollBehavior = null, bool useInheritedMediaQuery = false, global::Doroti.Generated.Framework.Animation.AnimationStyle? themeAnimationStyle = null)
    {
        var __instance = new MaterialApp(key: key, scaffoldMessengerKey: scaffoldMessengerKey, routes: new DartMap<string, global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>>(), onNavigationNotification: onNavigationNotification, navigatorObservers: new List<global::Doroti.Generated.Framework.Widgets.NavigatorObserver>(), builder: builder, title: title, onGenerateTitle: onGenerateTitle, color: color, theme: theme, darkTheme: darkTheme, highContrastTheme: highContrastTheme, highContrastDarkTheme: highContrastDarkTheme, themeMode: themeMode, themeAnimationDuration: themeAnimationDuration, themeAnimationCurve: themeAnimationCurve, locale: locale, localizationsDelegates: localizationsDelegates, localeListResolutionCallback: localeListResolutionCallback, localeResolutionCallback: localeResolutionCallback, supportedLocales: supportedLocales, debugShowMaterialGrid: debugShowMaterialGrid, showPerformanceOverlay: showPerformanceOverlay, checkerboardRasterCacheImages: checkerboardRasterCacheImages, checkerboardOffscreenLayers: checkerboardOffscreenLayers, showSemanticsDebugger: showSemanticsDebugger, debugShowCheckedModeBanner: debugShowCheckedModeBanner, shortcuts: shortcuts, actions: actions, restorationScopeId: restorationScopeId, scrollBehavior: scrollBehavior, useInheritedMediaQuery: useInheritedMediaQuery, themeAnimationStyle: themeAnimationStyle);
        Duration __themeAnimationDuration = themeAnimationDuration ?? ThemeLibrary.kThemeAnimationDuration;
        global::Doroti.Generated.Framework.Animation.Curve __themeAnimationCurve = themeAnimationCurve ?? global::Doroti.Generated.Framework.Animation.Curves.linear;
        IEnumerable<Locale> __supportedLocales = supportedLocales ?? new List<Locale> { new Locale("en", "US") };
        __instance.scaffoldMessengerKey = scaffoldMessengerKey;
        __instance.routeInformationProvider = routeInformationProvider;
        __instance.routeInformationParser = routeInformationParser;
        __instance.routerDelegate = routerDelegate;
        __instance.routerConfig = routerConfig;
        __instance.backButtonDispatcher = backButtonDispatcher;
        __instance.builder = builder;
        __instance.title = title;
        __instance.onGenerateTitle = onGenerateTitle;
        __instance.onNavigationNotification = onNavigationNotification;
        __instance.color = color;
        __instance.theme = theme;
        __instance.darkTheme = darkTheme;
        __instance.highContrastTheme = highContrastTheme;
        __instance.highContrastDarkTheme = highContrastDarkTheme;
        __instance.themeMode = themeMode;
        __instance.themeAnimationDuration = __themeAnimationDuration;
        __instance.themeAnimationCurve = __themeAnimationCurve;
        __instance.locale = locale;
        __instance.localizationsDelegates = localizationsDelegates;
        __instance.localeListResolutionCallback = localeListResolutionCallback;
        __instance.localeResolutionCallback = localeResolutionCallback;
        __instance.supportedLocales = __supportedLocales;
        __instance.debugShowMaterialGrid = debugShowMaterialGrid;
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
        __instance.themeAnimationStyle = themeAnimationStyle;
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

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _MaterialAppState__app());
    public static global::Doroti.Generated.Framework.Widgets.HeroController createMaterialHeroController()
    {
        return new global::Doroti.Generated.Framework.Widgets.HeroController(createRectTween: ((global::System.Func<Rect?, Rect?, global::Doroti.Generated.Framework.Animation.Tween<Rect?>>?)((begin, end) => {
return ((global::Doroti.Generated.Framework.Animation.Tween<Rect?>)(object?)new MaterialRectArcTween(begin: begin, end: end));
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class MaterialScrollBehavior : global::Doroti.Generated.Framework.Widgets.ScrollBehavior
{
    public MaterialScrollBehavior()
    {
    }

    public override global::Doroti.Generated.Framework.Foundation.TargetPlatform getPlatform(global::Doroti.Generated.Framework.Widgets.BuildContext context) => Theme.of(context).platform;
    public override global::Doroti.Generated.Framework.Widgets.Widget buildScrollbar(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Widgets.ScrollableDetails details)
    {
        switch (global::Doroti.Generated.Framework.Painting.Basic_typesLibrary.axisDirectionToAxis(((global::Doroti.Generated.Framework.Widgets.ScrollableDetails)details).direction))
        {
            case global::Doroti.Generated.Framework.Painting.Axis.horizontal:
                {
                    return child;
                }
            case global::Doroti.Generated.Framework.Painting.Axis.vertical:
                {
                    switch (getPlatform(context))
                    {
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
                        case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                            {
                                DartRuntimePrimitives.Assert(() => (((global::Doroti.Generated.Framework.Widgets.ScrollableDetails)details).controller is not null));
                                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new Scrollbar(controller: ((global::Doroti.Generated.Framework.Widgets.ScrollableDetails)details).controller, child: child));
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
                    break;
                }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget buildOverscrollIndicator(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget child, global::Doroti.Generated.Framework.Widgets.ScrollableDetails details)
    {
        global::Doroti.Generated.Framework.Widgets.AndroidOverscrollIndicator indicator__32935 = (Theme.of(context).useMaterial3 ? global::Doroti.Generated.Framework.Widgets.AndroidOverscrollIndicator.stretch : global::Doroti.Generated.Framework.Widgets.AndroidOverscrollIndicator.glow);
        switch (getPlatform(context))
        {
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.iOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.linux:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.macOS:
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.windows:
                {
                    return child;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.android:
                {
                    switch (indicator__32935)
                    {
                        case global::Doroti.Generated.Framework.Widgets.AndroidOverscrollIndicator.stretch:
                            {
                                return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.StretchingOverscrollIndicator(axisDirection: ((global::Doroti.Generated.Framework.Widgets.ScrollableDetails)details).direction, clipBehavior: (((global::Doroti.Generated.Framework.Widgets.ScrollableDetails)details).clipBehavior ?? Clip.hardEdge), child: child));
                            }
                        case global::Doroti.Generated.Framework.Widgets.AndroidOverscrollIndicator.glow:
                            {
                                break;
                            }
                        default:
                            throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                    }
                    break;
                }
            case global::Doroti.Generated.Framework.Foundation.TargetPlatform.fuchsia:
                {
                    break;
                }
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.GlowingOverscrollIndicator(axisDirection: ((global::Doroti.Generated.Framework.Widgets.ScrollableDetails)details).direction, color: Theme.of(context).colorScheme.secondary, child: child));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MaterialAppState__app : global::Doroti.Generated.Framework.Widgets.State<MaterialApp>
{
    internal virtual global::Doroti.Generated.Framework.Widgets.HeroController _heroController { get; set; } = default!;

    internal virtual bool _usesRouter => DartRuntimePrimitives.ConvertValue<bool>(((((MaterialApp)this.widget).routerDelegate is not null) || (((MaterialApp)this.widget).routerConfig is not null)));
    public override void initState()
    {
        base.initState();
        _heroController = MaterialApp.createMaterialHeroController();
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
            return ((IEnumerable<object>)(object?)((Func<List<object>>)(() => { var __collection34671 = new List<object>(); var __collectionSpread34711 = ((MaterialApp)this.widget).localizationsDelegates; if (__collectionSpread34711 is not null) { __collection34671.AddRange(__collectionSpread34711); } __collection34671.Add(DefaultMaterialLocalizations.@delegate); __collection34671.Add(DefaultCupertinoLocalizations.@delegate); return __collection34671; }))());
            return default!;
        }
    }
    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _exitWidgetSelectionButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState> key, global::System.Action onPressed, string semanticsLabel)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new _MaterialInspectorButton__app(onPressed: () => onPressed(), semanticsLabel: semanticsLabel, icon: Icons.close, isDarkTheme: _isDarkTheme(context), buttonKey: key));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _moveExitWidgetSelectionButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, string semanticsLabel, bool usesDefaultAlignment = true)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_MaterialInspectorButton__app.CreateIconOnly(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: (usesDefaultAlignment ? Icons.arrow_right : Icons.arrow_left), isDarkTheme: _isDarkTheme(context)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _tapBehaviorButtonBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::System.Action onPressed, bool selectionOnTapEnabled, string semanticsLabel)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_MaterialInspectorButton__app.CreateToggle(onPressed: () => onPressed(), semanticsLabel: semanticsLabel, icon: new global::Doroti.Generated.Framework.Widgets.IconData(128842L), isDarkTheme: _isDarkTheme(context), toggledOn: selectionOnTapEnabled));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isDarkTheme(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((object.Equals(((MaterialApp)this.widget).themeMode, ThemeMode.dark)) || ((object.Equals(((MaterialApp)this.widget).themeMode, ThemeMode.system)) && (object.Equals(MediaQuery.platformBrightnessOf(context), Brightness.dark))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ThemeData _themeBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData? theme__36531 = default!;
        ThemeMode mode__36631 = (((MaterialApp)this.widget).themeMode ?? ThemeMode.system);
        global::Doroti.Ui.Brightness platformBrightness__36697 = MediaQuery.platformBrightnessOf(context);
        bool useDarkTheme__36775 = ((object.Equals(mode__36631, ThemeMode.dark)) || (((object.Equals(mode__36631, ThemeMode.system)) && (object.Equals(platformBrightness__36697, Brightness.dark)))));
        bool highContrast__36919 = MediaQuery.highContrastOf(context);
        if (((useDarkTheme__36775 && highContrast__36919) && (((MaterialApp)this.widget).highContrastDarkTheme is not null)))
        {
            theme__36531 = ((MaterialApp)this.widget).highContrastDarkTheme;
        }
        else
        {
            if ((useDarkTheme__36775 && (((MaterialApp)this.widget).darkTheme is not null)))
            {
                theme__36531 = ((MaterialApp)this.widget).darkTheme;
            }
            else
            {
                if ((highContrast__36919 && (((MaterialApp)this.widget).highContrastTheme is not null)))
                {
                    theme__36531 = ((MaterialApp)this.widget).highContrastTheme;
                }
            }
        }
        theme__36531 ??= (((MaterialApp)this.widget).theme ?? ThemeData.Create());
        SystemChrome.setSystemUIOverlayStyle(((object.Equals(theme__36531.brightness, Brightness.dark)) ? global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle.light : global::Doroti.Generated.Framework.Services.SystemUiOverlayStyle.dark));
        return theme__36531;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _materialBuilder(global::Doroti.Generated.Framework.Widgets.BuildContext context, global::Doroti.Generated.Framework.Widgets.Widget? child)
    {
        ThemeData theme__37599 = ((ThemeData)(object?)_themeBuilder(context));
        global::Doroti.Ui.Color effectiveSelectionColor__37647 = ((global::Doroti.Ui.Color)(object?)(theme__37599.textSelectionTheme.selectionColor ?? theme__37599.colorScheme.primary.withOpacity(0.4)));
        global::Doroti.Ui.Color effectiveCursorColor__37785 = ((global::Doroti.Ui.Color)(object?)(theme__37599.textSelectionTheme.cursorColor ?? theme__37599.colorScheme.primary));
        global::Doroti.Generated.Framework.Widgets.Widget childWidget__37895 = (child ?? global::Doroti.Generated.Framework.Widgets.SizedBox.CreateShrink());
        if ((((MaterialApp)this.widget).builder is not null))
        {
            childWidget__37895 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.Builder(builder: ((global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget>)((context) => {
return ((MaterialApp)this.widget).builder!(context, child);
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        }
        childWidget__37895 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new ScaffoldMessenger(key: ((MaterialApp)this.widget).scaffoldMessengerKey, child: new global::Doroti.Generated.Framework.Widgets.DefaultSelectionStyle(selectionColor: effectiveSelectionColor__37647, cursorColor: effectiveCursorColor__37785, child: childWidget__37895)));
        if ((!object.Equals(((MaterialApp)this.widget).themeAnimationStyle, global::Doroti.Generated.Framework.Animation.AnimationStyle.noAnimation)))
        {
            childWidget__37895 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new AnimatedTheme(data: theme__37599, duration: (((MaterialApp)this.widget).themeAnimationStyle?.duration ?? ((MaterialApp)this.widget).themeAnimationDuration), curve: (((MaterialApp)this.widget).themeAnimationStyle?.curve ?? ((MaterialApp)this.widget).themeAnimationCurve), child: childWidget__37895));
        }
        else
        {
            childWidget__37895 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new Theme(data: theme__37599, child: childWidget__37895));
        }
        return childWidget__37895;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Widgets.Widget _buildWidgetApp(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color materialColor__39933 = ((global::Doroti.Ui.Color)(object?)((((MaterialApp)this.widget).color ?? ((MaterialApp)this.widget).theme?.primaryColor) ?? Colors.blue));
        if (this._usesRouter)
        {
            return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)global::Doroti.Generated.Framework.Widgets.WidgetsApp.CreateRouter(key: new global::Doroti.Generated.Framework.Widgets.GlobalObjectKey<IState>(this), routeInformationProvider: ((MaterialApp)this.widget).routeInformationProvider, routeInformationParser: ((MaterialApp)this.widget).routeInformationParser, routerDelegate: ((MaterialApp)this.widget).routerDelegate, routerConfig: ((MaterialApp)this.widget).routerConfig, backButtonDispatcher: ((MaterialApp)this.widget).backButtonDispatcher, onNavigationNotification: (global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>?)((MaterialApp)this.widget).onNavigationNotification, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._materialBuilder, title: ((MaterialApp)this.widget).title, onGenerateTitle: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>?)((MaterialApp)this.widget).onGenerateTitle, textStyle: AppLibrary._errorTextStyle, color: materialColor__39933, locale: ((MaterialApp)this.widget).locale, localizationsDelegates: this._localizationsDelegates.Cast<dynamic>(), localeResolutionCallback: (global::System.Func<Locale?, IEnumerable<Locale>, Locale?>?)((MaterialApp)this.widget).localeResolutionCallback, localeListResolutionCallback: (global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>?)((MaterialApp)this.widget).localeListResolutionCallback, supportedLocales: ((MaterialApp)this.widget).supportedLocales.Cast<Locale>(), showPerformanceOverlay: ((MaterialApp)this.widget).showPerformanceOverlay, showSemanticsDebugger: ((MaterialApp)this.widget).showSemanticsDebugger, debugShowCheckedModeBanner: ((MaterialApp)this.widget).debugShowCheckedModeBanner, exitWidgetSelectionButtonBuilder: (ExitWidgetSelectionButtonBuilder)this._exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder: (MoveExitWidgetSelectionButtonBuilder)this._moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder: (TapBehaviorButtonBuilder)this._tapBehaviorButtonBuilder, shortcuts: ((MaterialApp)this.widget).shortcuts, actions: ((MaterialApp)this.widget).actions, restorationScopeId: ((MaterialApp)this.widget).restorationScopeId));
        }
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.WidgetsApp(key: new global::Doroti.Generated.Framework.Widgets.GlobalObjectKey<IState>(this), navigatorKey: ((MaterialApp)this.widget).navigatorKey, navigatorObservers: ((MaterialApp)this.widget).navigatorObservers!, pageRouteBuilder: ((PageRouteFactory)((settings, builder) => {
return new MaterialPageRoute<object>(settings: settings, builder: builder);
throw new InvalidOperationException("Dart closure completed without a value.");
})), home: ((MaterialApp)this.widget).home, routes: ((MaterialApp)this.widget).routes!, initialRoute: ((MaterialApp)this.widget).initialRoute, onGenerateRoute: (global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>?)((MaterialApp)this.widget).onGenerateRoute, onGenerateInitialRoutes: (global::System.Func<string, List<dynamic>>?)((MaterialApp)this.widget).onGenerateInitialRoutes, onUnknownRoute: (global::System.Func<global::Doroti.Generated.Framework.Widgets.RouteSettings, dynamic>?)((MaterialApp)this.widget).onUnknownRoute, onNavigationNotification: (global::System.Func<global::Doroti.Generated.Framework.Widgets.NavigationNotification, bool>?)((MaterialApp)this.widget).onNavigationNotification, builder: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, global::Doroti.Generated.Framework.Widgets.Widget?, global::Doroti.Generated.Framework.Widgets.Widget>)this._materialBuilder, title: ((MaterialApp)this.widget).title, onGenerateTitle: (global::System.Func<global::Doroti.Generated.Framework.Widgets.BuildContext, string>?)((MaterialApp)this.widget).onGenerateTitle, textStyle: AppLibrary._errorTextStyle, color: materialColor__39933, locale: ((MaterialApp)this.widget).locale, localizationsDelegates: this._localizationsDelegates.Cast<dynamic>(), localeResolutionCallback: (global::System.Func<Locale?, IEnumerable<Locale>, Locale?>?)((MaterialApp)this.widget).localeResolutionCallback, localeListResolutionCallback: (global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>?)((MaterialApp)this.widget).localeListResolutionCallback, supportedLocales: ((MaterialApp)this.widget).supportedLocales.Cast<Locale>(), showPerformanceOverlay: ((MaterialApp)this.widget).showPerformanceOverlay, showSemanticsDebugger: ((MaterialApp)this.widget).showSemanticsDebugger, debugShowCheckedModeBanner: ((MaterialApp)this.widget).debugShowCheckedModeBanner, exitWidgetSelectionButtonBuilder: (ExitWidgetSelectionButtonBuilder)this._exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder: (MoveExitWidgetSelectionButtonBuilder)this._moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder: (TapBehaviorButtonBuilder)this._tapBehaviorButtonBuilder, shortcuts: ((MaterialApp)this.widget).shortcuts, actions: ((MaterialApp)this.widget).actions, restorationScopeId: ((MaterialApp)this.widget).restorationScopeId));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Generated.Framework.Widgets.Widget result__43132 = ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)_buildWidgetApp(context));
        DartRuntimePrimitives.Assert(() =>
            {
                if (((MaterialApp)this.widget).debugShowMaterialGrid)
                {
                    result__43132 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Widgets.Widget>(new global::Doroti.Generated.Framework.Widgets.GridPaper(color: new global::Doroti.Ui.Color(3774462944L), interval: 8.0, subdivisions: 1L, child: result__43132));
                }
                return true;
            });
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new global::Doroti.Generated.Framework.Widgets.ScrollConfiguration(behavior: (((MaterialApp)this.widget).scrollBehavior ?? new MaterialScrollBehavior()), child: new global::Doroti.Generated.Framework.Widgets.HeroControllerScope(controller: this._heroController, child: result__43132)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MaterialInspectorButton__app : global::Doroti.Generated.Framework.Widgets.InspectorButton
{
    public virtual bool isDarkTheme { get; private set; } = default!;
    internal static global::Doroti.Generated.Framework.Painting.EdgeInsets _buttonPadding = global::Doroti.Generated.Framework.Painting.EdgeInsets.zero;
    internal static global::Doroti.Generated.Framework.Rendering.BoxConstraints _buttonConstraints = global::Doroti.Generated.Framework.Rendering.BoxConstraints.CreateTightFor(width: global::Doroti.Generated.Framework.Widgets.InspectorButton.buttonSize, height: global::Doroti.Generated.Framework.Widgets.InspectorButton.buttonSize);

    internal _MaterialInspectorButton__app(global::System.Action onPressed, string semanticsLabel, global::Doroti.Generated.Framework.Widgets.IconData icon, bool isDarkTheme, global::Doroti.Generated.Framework.Widgets.GlobalKey<IState>? buttonKey = null) : base(onPressed, semanticsLabel, icon, buttonKey)
    {
        this.isDarkTheme = isDarkTheme;
    }

    internal static _MaterialInspectorButton__app CreateToggle(global::System.Action onPressed, string semanticsLabel, global::Doroti.Generated.Framework.Widgets.IconData icon, bool isDarkTheme, bool toggledOn = true)
    {
        var __instance = new _MaterialInspectorButton__app(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: icon, isDarkTheme: isDarkTheme);
        __instance.isDarkTheme = isDarkTheme;
        return __instance;
    }

    internal static _MaterialInspectorButton__app CreateIconOnly(global::System.Action onPressed, string semanticsLabel, global::Doroti.Generated.Framework.Widgets.IconData icon, bool isDarkTheme)
    {
        var __instance = new _MaterialInspectorButton__app(onPressed: onPressed, semanticsLabel: semanticsLabel, icon: icon, isDarkTheme: isDarkTheme);
        __instance.isDarkTheme = isDarkTheme;
        return __instance;
    }

    public override global::Doroti.Generated.Framework.Widgets.Widget build(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Widgets.Widget)(object?)new IconButton(key: this.buttonKey, onPressed: this.onPressed, iconSize: this.iconSizeForVariant, padding: _buttonPadding, constraints: _buttonConstraints, style: _selectionButtonsIconStyle(context), icon: new global::Doroti.Generated.Framework.Widgets.Icon(this.icon, semanticLabel: this.semanticsLabel)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ButtonStyle _selectionButtonsIconStyle(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color foreground__44958 = ((global::Doroti.Ui.Color)(object?)foregroundColor(context));
        global::Doroti.Ui.Color background__45013 = ((global::Doroti.Ui.Color)(object?)backgroundColor(context));
        return ((ButtonStyle)(object?)IconButton.styleFrom(foregroundColor: foreground__44958, backgroundColor: background__45013, side: _borderSide(color: foreground__44958), tapTargetSize: MaterialTapTargetSize.padded));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Generated.Framework.Painting.BorderSide? _borderSide(Color color)
    {
        switch (this.variant)
        {
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.filled:
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.iconOnly:
                {
                    return null;
                }
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.toggle:
                {
                    return ((this.toggledOn == false) ? new global::Doroti.Generated.Framework.Painting.BorderSide(color: color) : null);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color foregroundColor(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color primaryColor__45643 = ((global::Doroti.Ui.Color)(object?)_primaryColor(context));
        global::Doroti.Ui.Color secondaryColor__45698 = ((global::Doroti.Ui.Color)(object?)_secondaryColor(context));
        switch (this.variant)
        {
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.filled:
                {
                    return primaryColor__45643;
                }
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.iconOnly:
                {
                    return secondaryColor__45698;
                }
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.toggle:
                {
                    return (!DartRuntimePrimitives.RequireValue(this.toggledOn) ? secondaryColor__45698 : primaryColor__45643);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color backgroundColor(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        global::Doroti.Ui.Color secondaryColor__46099 = ((global::Doroti.Ui.Color)(object?)_secondaryColor(context));
        switch (this.variant)
        {
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.filled:
                {
                    return secondaryColor__46099;
                }
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.iconOnly:
                {
                    return Colors.transparent;
                }
            case global::Doroti.Generated.Framework.Widgets.InspectorButtonVariant.toggle:
                {
                    return (!DartRuntimePrimitives.RequireValue(this.toggledOn) ? Colors.transparent : secondaryColor__46099);
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _primaryColor(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__46502 = Theme.of(context);
        return ((global::Doroti.Ui.Color)(object?)(this.isDarkTheme ? theme__46502.colorScheme.onPrimaryContainer : theme__46502.colorScheme.primaryContainer));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual global::Doroti.Ui.Color _secondaryColor(global::Doroti.Generated.Framework.Widgets.BuildContext context)
    {
        ThemeData theme__46702 = Theme.of(context);
        return ((global::Doroti.Ui.Color)(object?)(this.isDarkTheme ? theme__46702.colorScheme.primaryContainer : theme__46702.colorScheme.onPrimaryContainer));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}
