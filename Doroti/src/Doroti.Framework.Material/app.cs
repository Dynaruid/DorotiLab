// <doroti-reviewed-product-source milestone="G6-3" />
// Doroti typed semantic compiler 3.0.0; source: ../../../reference/flutter-master/packages/flutter/lib/src/material/app.dart

using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Material;

public static partial class AppLibrary
{
    internal static TextStyle _errorTextStyle = new TextStyle(
        color: new Color(3506372608L),
        fontFamily: "monospace",
        fontSize: 48.0,
        fontWeight: FontWeight.w900,
        decoration: TextDecoration.underline,
        decorationColor: new Color(4294967040L),
        decorationStyle: TextDecorationStyle.doubleLine,
        debugLabel: "fallback style; consider putting your text in a Material"
    );
}

public enum ThemeMode
{
    system,
    light,
    dark,
}

public static class ThemeModeMembers
{
    public static bool isSystem(this ThemeMode value) => Equals(value, ThemeMode.system);

    public static bool isLight(this ThemeMode value) => Equals(value, ThemeMode.light);

    public static bool isDark(this ThemeMode value) => Equals(value, ThemeMode.dark);
}

public class MaterialApp : StatefulWidget
{
    public virtual GlobalKey<NavigatorState>? navigatorKey { get; private set; }
    public virtual GlobalKey<ScaffoldMessengerState>? scaffoldMessengerKey { get; private set; }
    public virtual Widget? home { get; private set; }
    public virtual DartMap<string, Func<BuildContext, Widget>>? routes { get; private set; }
    public virtual string? initialRoute { get; private set; }
    public virtual Func<RouteSettings, dynamic>? onGenerateRoute { get; private set; }
    public virtual Func<string, List<dynamic>>? onGenerateInitialRoutes { get; private set; }
    public virtual Func<RouteSettings, dynamic>? onUnknownRoute { get; private set; }
    public virtual Func<NavigationNotification, bool>? onNavigationNotification
    {
        get;
        private set;
    }
    public virtual List<NavigatorObserver>? navigatorObservers { get; private set; }
    public virtual RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual object? routeInformationParser { get; private set; } = default!;
    public virtual IRouterDelegate? routerDelegate { get; private set; } = default!;
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual IRouterConfig? routerConfig { get; private set; }
    public virtual Func<BuildContext, Widget?, Widget>? builder { get; private set; }
    public virtual string? title { get; private set; }
    public virtual Func<BuildContext, string>? onGenerateTitle { get; private set; }
    public virtual ThemeData? theme { get; private set; }
    public virtual ThemeData? darkTheme { get; private set; }
    public virtual ThemeData? highContrastTheme { get; private set; }
    public virtual ThemeData? highContrastDarkTheme { get; private set; }
    private readonly Lazy<ThemeData>? _themeFactory;
    private readonly Lazy<ThemeData>? _darkThemeFactory;
    private readonly Lazy<ThemeData>? _highContrastThemeFactory;
    private readonly Lazy<ThemeData>? _highContrastDarkThemeFactory;
    public virtual ThemeMode? themeMode { get; private set; }
    public virtual Duration themeAnimationDuration { get; private set; } = default!;
    public virtual Curve themeAnimationCurve { get; private set; } = default!;
    public virtual Color? color { get; private set; }
    public virtual Locale? locale { get; private set; }
    public virtual IEnumerable<dynamic>? localizationsDelegates { get; private set; }
    public virtual Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback
    {
        get;
        private set;
    }
    public virtual Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback
    {
        get;
        private set;
    }
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
    public virtual bool debugShowMaterialGrid { get; private set; } = default!;
    public virtual bool useInheritedMediaQuery { get; private set; } = default!;
    public virtual AnimationStyle? themeAnimationStyle { get; private set; }

    public MaterialApp(
        Key? key = null,
        GlobalKey<NavigatorState>? navigatorKey = null,
        GlobalKey<ScaffoldMessengerState>? scaffoldMessengerKey = null,
        Widget? home = null,
        DartMap<string, Func<BuildContext, Widget>> routes = default!,
        string? initialRoute = null,
        Func<RouteSettings, dynamic>? onGenerateRoute = null,
        Func<string, List<dynamic>>? onGenerateInitialRoutes = null,
        Func<RouteSettings, dynamic>? onUnknownRoute = null,
        Func<NavigationNotification, bool>? onNavigationNotification = null,
        List<NavigatorObserver> navigatorObservers = default!,
        Func<BuildContext, Widget?, Widget>? builder = null,
        string? title = "",
        Func<BuildContext, string>? onGenerateTitle = null,
        Color? color = null,
        ThemeData? theme = null,
        ThemeData? darkTheme = null,
        ThemeData? highContrastTheme = null,
        ThemeData? highContrastDarkTheme = null,
        ThemeMode? themeMode = ThemeMode.system,
        Duration? themeAnimationDuration = null,
        Curve themeAnimationCurve = default!,
        Locale? locale = null,
        IEnumerable<dynamic>? localizationsDelegates = null,
        Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null,
        Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null,
        IEnumerable<Locale> supportedLocales = default!,
        bool debugShowMaterialGrid = false,
        bool showPerformanceOverlay = false,
        bool checkerboardRasterCacheImages = false,
        bool checkerboardOffscreenLayers = false,
        bool showSemanticsDebugger = false,
        bool debugShowCheckedModeBanner = true,
        DartMap<ShortcutActivator, Intent>? shortcuts = null,
        DartMap<Type, dynamic>? actions = null,
        string? restorationScopeId = null,
        ScrollBehavior? scrollBehavior = null,
        bool useInheritedMediaQuery = false,
        AnimationStyle? themeAnimationStyle = null,
        Func<ThemeData>? themeFactory = null,
        Func<ThemeData>? darkThemeFactory = null,
        Func<ThemeData>? highContrastThemeFactory = null,
        Func<ThemeData>? highContrastDarkThemeFactory = null
    )
        : base(key: key)
    {
        if (theme is not null && themeFactory is not null)
        {
            throw new ArgumentException("Specify either theme or themeFactory, not both.");
        }

        if (darkTheme is not null && darkThemeFactory is not null)
        {
            throw new ArgumentException("Specify either darkTheme or darkThemeFactory, not both.");
        }

        if (highContrastTheme is not null && highContrastThemeFactory is not null)
        {
            throw new ArgumentException(
                "Specify either highContrastTheme or highContrastThemeFactory, not both."
            );
        }

        if (highContrastDarkTheme is not null && highContrastDarkThemeFactory is not null)
        {
            throw new ArgumentException(
                "Specify either highContrastDarkTheme or highContrastDarkThemeFactory, not both."
            );
        }

        DartMap<string, Func<BuildContext, Widget>> __routes =
            routes ?? new DartMap<string, Func<BuildContext, Widget>>();
        List<NavigatorObserver> __navigatorObservers =
            navigatorObservers ?? new List<NavigatorObserver>();
        Duration __themeAnimationDuration =
            themeAnimationDuration ?? ThemeLibrary.kThemeAnimationDuration;
        Curve __themeAnimationCurve = themeAnimationCurve ?? Curves.linear;
        IEnumerable<Locale> __supportedLocales =
            supportedLocales ?? new List<Locale> { new Locale("en", "US") };
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
        _themeFactory = themeFactory is null ? null : new(themeFactory);
        _darkThemeFactory = darkThemeFactory is null ? null : new(darkThemeFactory);
        _highContrastThemeFactory = highContrastThemeFactory is null
            ? null
            : new(highContrastThemeFactory);
        _highContrastDarkThemeFactory = highContrastDarkThemeFactory is null
            ? null
            : new(highContrastDarkThemeFactory);
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
        routeInformationProvider = null;
        routeInformationParser = null;
        routerDelegate = null;
        backButtonDispatcher = null;
        routerConfig = null;
    }

    public ThemeData? resolveTheme() => theme ?? _themeFactory?.Value;

    public ThemeData? resolveDarkTheme() => darkTheme ?? _darkThemeFactory?.Value;

    public ThemeData? resolveHighContrastTheme() =>
        highContrastTheme ?? _highContrastThemeFactory?.Value;

    public ThemeData? resolveHighContrastDarkTheme() =>
        highContrastDarkTheme ?? _highContrastDarkThemeFactory?.Value;

    public static MaterialApp CreateRouter(
        Key? key = null,
        GlobalKey<ScaffoldMessengerState>? scaffoldMessengerKey = null,
        RouteInformationProvider? routeInformationProvider = null,
        object? routeInformationParser = null,
        IRouterDelegate? routerDelegate = null,
        IRouterConfig? routerConfig = null,
        BackButtonDispatcher? backButtonDispatcher = null,
        Func<BuildContext, Widget?, Widget>? builder = null,
        string? title = null,
        Func<BuildContext, string>? onGenerateTitle = null,
        Func<NavigationNotification, bool>? onNavigationNotification = null,
        Color? color = null,
        ThemeData? theme = null,
        ThemeData? darkTheme = null,
        ThemeData? highContrastTheme = null,
        ThemeData? highContrastDarkTheme = null,
        ThemeMode? themeMode = ThemeMode.system,
        Duration? themeAnimationDuration = null,
        Curve themeAnimationCurve = default!,
        Locale? locale = null,
        IEnumerable<dynamic>? localizationsDelegates = null,
        Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null,
        Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null,
        IEnumerable<Locale> supportedLocales = default!,
        bool debugShowMaterialGrid = false,
        bool showPerformanceOverlay = false,
        bool checkerboardRasterCacheImages = false,
        bool checkerboardOffscreenLayers = false,
        bool showSemanticsDebugger = false,
        bool debugShowCheckedModeBanner = true,
        DartMap<ShortcutActivator, Intent>? shortcuts = null,
        DartMap<Type, dynamic>? actions = null,
        string? restorationScopeId = null,
        ScrollBehavior? scrollBehavior = null,
        bool useInheritedMediaQuery = false,
        AnimationStyle? themeAnimationStyle = null,
        Func<ThemeData>? themeFactory = null,
        Func<ThemeData>? darkThemeFactory = null,
        Func<ThemeData>? highContrastThemeFactory = null,
        Func<ThemeData>? highContrastDarkThemeFactory = null
    )
    {
        var __instance = new MaterialApp(
            key: key,
            scaffoldMessengerKey: scaffoldMessengerKey,
            routes: new DartMap<string, Func<BuildContext, Widget>>(),
            onNavigationNotification: onNavigationNotification,
            navigatorObservers: new List<NavigatorObserver>(),
            builder: builder,
            title: title,
            onGenerateTitle: onGenerateTitle,
            color: color,
            theme: theme,
            darkTheme: darkTheme,
            highContrastTheme: highContrastTheme,
            highContrastDarkTheme: highContrastDarkTheme,
            themeMode: themeMode,
            themeAnimationDuration: themeAnimationDuration,
            themeAnimationCurve: themeAnimationCurve,
            locale: locale,
            localizationsDelegates: localizationsDelegates,
            localeListResolutionCallback: localeListResolutionCallback,
            localeResolutionCallback: localeResolutionCallback,
            supportedLocales: supportedLocales,
            debugShowMaterialGrid: debugShowMaterialGrid,
            showPerformanceOverlay: showPerformanceOverlay,
            checkerboardRasterCacheImages: checkerboardRasterCacheImages,
            checkerboardOffscreenLayers: checkerboardOffscreenLayers,
            showSemanticsDebugger: showSemanticsDebugger,
            debugShowCheckedModeBanner: debugShowCheckedModeBanner,
            shortcuts: shortcuts,
            actions: actions,
            restorationScopeId: restorationScopeId,
            scrollBehavior: scrollBehavior,
            useInheritedMediaQuery: useInheritedMediaQuery,
            themeAnimationStyle: themeAnimationStyle,
            themeFactory: themeFactory,
            darkThemeFactory: darkThemeFactory,
            highContrastThemeFactory: highContrastThemeFactory,
            highContrastDarkThemeFactory: highContrastDarkThemeFactory
        );
        Duration __themeAnimationDuration =
            themeAnimationDuration ?? ThemeLibrary.kThemeAnimationDuration;
        Curve __themeAnimationCurve = themeAnimationCurve ?? Curves.linear;
        IEnumerable<Locale> __supportedLocales =
            supportedLocales ?? new List<Locale> { new Locale("en", "US") };
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

    public override IState createState() =>
        DartRuntimePrimitives.ConvertValue<IState>(new _MaterialAppState__app());

    public static HeroController createMaterialHeroController()
    {
        return new HeroController(
            createRectTween: (begin, end) =>
            {
                return new MaterialRectArcTween(begin: begin, end: end);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class MaterialScrollBehavior : ScrollBehavior
{
    public MaterialScrollBehavior() { }

    public override TargetPlatform getPlatform(BuildContext context) => Theme.of(context).platform;

    public override Widget buildScrollbar(
        BuildContext context,
        Widget child,
        ScrollableDetails details
    )
    {
        switch (Basic_typesLibrary.axisDirectionToAxis(details.direction))
        {
            case Axis.horizontal:
            {
                return child;
            }
            case Axis.vertical:
            {
                switch (getPlatform(context))
                {
                    case TargetPlatform.linux:
                    case TargetPlatform.macOS:
                    case TargetPlatform.windows:
                    {
                        DartRuntimePrimitives.Assert(() => details.controller is not null);
                        return new Scrollbar(controller: details.controller, child: child);
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
            }
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget buildOverscrollIndicator(
        BuildContext context,
        Widget child,
        ScrollableDetails details
    )
    {
        AndroidOverscrollIndicator indicator = AndroidOverscrollIndicator.stretch;
        switch (getPlatform(context))
        {
            case TargetPlatform.iOS:
            case TargetPlatform.linux:
            case TargetPlatform.macOS:
            case TargetPlatform.windows:
            {
                return child;
            }
            case TargetPlatform.android:
            {
                switch (indicator)
                {
                    case AndroidOverscrollIndicator.stretch:
                    {
                        return new StretchingOverscrollIndicator(
                            axisDirection: details.direction,
                            clipBehavior: details.clipBehavior ?? Clip.hardEdge,
                            child: child
                        );
                    }
                    case AndroidOverscrollIndicator.glow:
                    {
                        break;
                    }
                    default:
                        throw new InvalidOperationException("Non-exhaustive Dart switch value.");
                }
                break;
            }
            case TargetPlatform.fuchsia:
            {
                break;
            }
        }
        return new GlowingOverscrollIndicator(
            axisDirection: details.direction,
            color: Theme.of(context).colorScheme.secondary,
            child: child
        );
    }
}

internal class _MaterialAppState__app : State<MaterialApp>
{
    internal virtual HeroController _heroController { get; set; } = default!;

    internal virtual bool _usesRouter =>
        DartRuntimePrimitives.ConvertValue<bool>(
            (widget.routerDelegate is not null) || (widget.routerConfig is not null)
        );

    public override void initState()
    {
        base.initState();
        _heroController = MaterialApp.createMaterialHeroController();
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
            return (
                (Func<List<object>>)(
                    () =>
                    {
                        var __collection34671 = new List<object>();
                        var __collectionSpread34711 = widget.localizationsDelegates;
                        if (__collectionSpread34711 is not null)
                        {
                            __collection34671.AddRange(__collectionSpread34711);
                        }
                        __collection34671.Add(DefaultMaterialLocalizations.@delegate);
                        __collection34671.Add(DefaultCupertinoLocalizations.@delegate);
                        return __collection34671;
                    }
                )
            )();
        }
    }

    internal virtual Widget _exitWidgetSelectionButtonBuilder(
        BuildContext context,
        GlobalKey<IState> key,
        Action onPressed,
        string semanticsLabel
    )
    {
        return new _MaterialInspectorButton__app(
            onPressed: () => onPressed(),
            semanticsLabel: semanticsLabel,
            icon: Icons.close,
            isDarkTheme: _isDarkTheme(context),
            buttonKey: key
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _moveExitWidgetSelectionButtonBuilder(
        BuildContext context,
        Action onPressed,
        string semanticsLabel,
        bool usesDefaultAlignment = true
    )
    {
        return _MaterialInspectorButton__app.CreateIconOnly(
            onPressed: onPressed,
            semanticsLabel: semanticsLabel,
            icon: usesDefaultAlignment ? Icons.arrow_right : Icons.arrow_left,
            isDarkTheme: _isDarkTheme(context)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _tapBehaviorButtonBuilder(
        BuildContext context,
        Action onPressed,
        bool selectionOnTapEnabled,
        string semanticsLabel
    )
    {
        return _MaterialInspectorButton__app.CreateToggle(
            onPressed: () => onPressed(),
            semanticsLabel: semanticsLabel,
            icon: new IconData(128842L),
            isDarkTheme: _isDarkTheme(context),
            toggledOn: selectionOnTapEnabled
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isDarkTheme(BuildContext context)
    {
        return Equals(widget.themeMode, ThemeMode.dark)
            || (
                Equals(widget.themeMode, ThemeMode.system)
                && Equals(MediaQuery.platformBrightnessOf(context), Brightness.dark)
            );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ThemeData _themeBuilder(BuildContext context)
    {
        var app = widget;
        ThemeData? themeLocal = null;
        ThemeMode mode = app.themeMode ?? ThemeMode.system;
        Brightness platformBrightness = MediaQuery.platformBrightnessOf(context);
        bool useDarkTheme =
            Equals(mode, ThemeMode.dark)
            || (Equals(mode, ThemeMode.system) && Equals(platformBrightness, Brightness.dark));
        bool highContrast = MediaQuery.highContrastOf(context);
        if (useDarkTheme && highContrast)
        {
            themeLocal = app.resolveHighContrastDarkTheme();
        }
        if (themeLocal is null && useDarkTheme)
        {
            themeLocal = app.resolveDarkTheme();
        }
        if (themeLocal is null && highContrast)
        {
            themeLocal = app.resolveHighContrastTheme();
        }
        themeLocal ??= (app.resolveTheme() ?? ThemeData.Create());
        SystemChrome.setSystemUIOverlayStyle(
            Equals(themeLocal.brightness, Brightness.dark)
                ? SystemUiOverlayStyle.light
                : SystemUiOverlayStyle.dark
        );
        return themeLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _materialBuilder(BuildContext context, Widget? child)
    {
        ThemeData theme = _themeBuilder(context);
        Color effectiveSelectionColor =
            theme.textSelectionTheme.selectionColor ?? theme.colorScheme.primary.withOpacity(0.4);
        Color effectiveCursorColor =
            theme.textSelectionTheme.cursorColor ?? theme.colorScheme.primary;
        Widget childWidget = child ?? SizedBox.CreateShrink();
        if (widget.builder is not null)
        {
            childWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Builder(
                    builder: (context) =>
                    {
                        return widget.builder!(context, child);
                        throw new InvalidOperationException(
                            "Dart closure completed without a value."
                        );
                    }
                )
            );
        }
        childWidget = DartRuntimePrimitives.ConvertValue<Widget>(
            new ScaffoldMessenger(
                key: widget.scaffoldMessengerKey,
                child: new DefaultSelectionStyle(
                    selectionColor: effectiveSelectionColor,
                    cursorColor: effectiveCursorColor,
                    child: childWidget
                )
            )
        );
        var view = View.maybeOf(context);
        if (view?.registeredCapabilityIds.Contains(DorotiCapabilityIds.WindowTitlebar) == true)
        {
            var titlebar = view.RequireCapability<IWindowTitlebarHostCapability>(
                DorotiCapabilityIds.WindowTitlebar,
                DartUiInvocation.Managed("MaterialApp.windowTitlebar")
            );
            var themedChild = childWidget;
            // Read below AnimatedTheme so the caption follows the same color transition as the body.
            childWidget = new Builder(builder: themeContext =>
            {
                var currentTheme = Theme.of(themeContext);
                titlebar.SetTheme(
                    new WindowTitlebarTheme(
                        currentTheme.scaffoldBackgroundColor,
                        currentTheme.brightness
                    )
                );
                return themedChild;
            });
        }
        if (!Equals(widget.themeAnimationStyle, AnimationStyle.noAnimation))
        {
            childWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new AnimatedTheme(
                    data: theme,
                    duration: widget.themeAnimationStyle?.duration ?? widget.themeAnimationDuration,
                    curve: widget.themeAnimationStyle?.curve ?? widget.themeAnimationCurve,
                    child: childWidget
                )
            );
        }
        else
        {
            childWidget = DartRuntimePrimitives.ConvertValue<Widget>(
                new Theme(data: theme, child: childWidget)
            );
        }
        return childWidget;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Widget _buildWidgetApp(BuildContext context)
    {
        Color materialColor = (widget.color ?? widget.theme?.primaryColor) ?? Colors.blue;
        if (_usesRouter)
        {
            return WidgetsApp.CreateRouter(
                key: new GlobalObjectKey<IState>(this),
                routeInformationProvider: widget.routeInformationProvider,
                routeInformationParser: widget.routeInformationParser,
                routerDelegate: widget.routerDelegate,
                routerConfig: widget.routerConfig,
                backButtonDispatcher: widget.backButtonDispatcher,
                onNavigationNotification: widget.onNavigationNotification,
                builder: _materialBuilder,
                title: widget.title,
                onGenerateTitle: widget.onGenerateTitle,
                textStyle: AppLibrary._errorTextStyle,
                color: materialColor,
                locale: widget.locale,
                localizationsDelegates: _localizationsDelegates.Cast<dynamic>(),
                localeResolutionCallback: widget.localeResolutionCallback,
                localeListResolutionCallback: widget.localeListResolutionCallback,
                supportedLocales: widget.supportedLocales.Cast<Locale>(),
                showPerformanceOverlay: widget.showPerformanceOverlay,
                showSemanticsDebugger: widget.showSemanticsDebugger,
                debugShowCheckedModeBanner: widget.debugShowCheckedModeBanner,
                exitWidgetSelectionButtonBuilder: _exitWidgetSelectionButtonBuilder,
                moveExitWidgetSelectionButtonBuilder: _moveExitWidgetSelectionButtonBuilder,
                tapBehaviorButtonBuilder: _tapBehaviorButtonBuilder,
                shortcuts: widget.shortcuts,
                actions: widget.actions,
                restorationScopeId: widget.restorationScopeId
            );
        }
        return new WidgetsApp(
            key: new GlobalObjectKey<IState>(this),
            navigatorKey: widget.navigatorKey,
            navigatorObservers: widget.navigatorObservers!,
            pageRouteBuilder: (settings, builder) =>
            {
                return new MaterialPageRoute<object>(settings: settings, builder: builder);
                throw new InvalidOperationException("Dart closure completed without a value.");
            },
            home: widget.home,
            routes: widget.routes!,
            initialRoute: widget.initialRoute,
            onGenerateRoute: widget.onGenerateRoute,
            onGenerateInitialRoutes: widget.onGenerateInitialRoutes,
            onUnknownRoute: widget.onUnknownRoute,
            onNavigationNotification: widget.onNavigationNotification,
            builder: _materialBuilder,
            title: widget.title,
            onGenerateTitle: widget.onGenerateTitle,
            textStyle: AppLibrary._errorTextStyle,
            color: materialColor,
            locale: widget.locale,
            localizationsDelegates: _localizationsDelegates.Cast<dynamic>(),
            localeResolutionCallback: widget.localeResolutionCallback,
            localeListResolutionCallback: widget.localeListResolutionCallback,
            supportedLocales: widget.supportedLocales.Cast<Locale>(),
            showPerformanceOverlay: widget.showPerformanceOverlay,
            showSemanticsDebugger: widget.showSemanticsDebugger,
            debugShowCheckedModeBanner: widget.debugShowCheckedModeBanner,
            exitWidgetSelectionButtonBuilder: _exitWidgetSelectionButtonBuilder,
            moveExitWidgetSelectionButtonBuilder: _moveExitWidgetSelectionButtonBuilder,
            tapBehaviorButtonBuilder: _tapBehaviorButtonBuilder,
            shortcuts: widget.shortcuts,
            actions: widget.actions,
            restorationScopeId: widget.restorationScopeId
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Widget build(BuildContext context)
    {
        Widget result = _buildWidgetApp(context);
        DartRuntimePrimitives.Assert(() =>
        {
            if (widget.debugShowMaterialGrid)
            {
                result = DartRuntimePrimitives.ConvertValue<Widget>(
                    new GridPaper(
                        color: new Color(3774462944L),
                        interval: 8.0,
                        subdivisions: 1L,
                        child: result
                    )
                );
            }
            return true;
        });
        return new ScrollConfiguration(
            behavior: widget.scrollBehavior ?? new MaterialScrollBehavior(),
            child: new HeroControllerScope(controller: _heroController, child: result)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _MaterialInspectorButton__app : InspectorButton
{
    public virtual bool isDarkTheme { get; private set; } = default!;
    internal static EdgeInsets _buttonPadding = EdgeInsets.zero;
    internal static BoxConstraints _buttonConstraints = BoxConstraints.CreateTightFor(
        width: buttonSize,
        height: buttonSize
    );

    internal _MaterialInspectorButton__app(
        Action onPressed,
        string semanticsLabel,
        IconData icon,
        bool isDarkTheme,
        GlobalKey<IState>? buttonKey = null
    )
        : base(onPressed, semanticsLabel, icon, buttonKey)
    {
        this.isDarkTheme = isDarkTheme;
    }

    internal static _MaterialInspectorButton__app CreateToggle(
        Action onPressed,
        string semanticsLabel,
        IconData icon,
        bool isDarkTheme,
        bool toggledOn = true
    )
    {
        var __instance = new _MaterialInspectorButton__app(
            onPressed: onPressed,
            semanticsLabel: semanticsLabel,
            icon: icon,
            isDarkTheme: isDarkTheme
        );
        __instance.isDarkTheme = isDarkTheme;
        return __instance;
    }

    internal static _MaterialInspectorButton__app CreateIconOnly(
        Action onPressed,
        string semanticsLabel,
        IconData icon,
        bool isDarkTheme
    )
    {
        var __instance = new _MaterialInspectorButton__app(
            onPressed: onPressed,
            semanticsLabel: semanticsLabel,
            icon: icon,
            isDarkTheme: isDarkTheme
        );
        __instance.isDarkTheme = isDarkTheme;
        return __instance;
    }

    public override Widget build(BuildContext context)
    {
        return new IconButton(
            key: buttonKey,
            onPressed: onPressed,
            iconSize: iconSizeForVariant,
            padding: _buttonPadding,
            constraints: _buttonConstraints,
            style: _selectionButtonsIconStyle(context),
            icon: new Icon(icon, semanticLabel: semanticsLabel)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual ButtonStyle _selectionButtonsIconStyle(BuildContext context)
    {
        Color foreground = foregroundColor(context);
        Color background = backgroundColor(context);
        return IconButton.styleFrom(
            foregroundColor: foreground,
            backgroundColor: background,
            side: _borderSide(color: foreground),
            tapTargetSize: MaterialTapTargetSize.padded
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual BorderSide? _borderSide(Color color)
    {
        switch (variant)
        {
            case InspectorButtonVariant.filled:
            case InspectorButtonVariant.iconOnly:
            {
                return null;
            }
            case InspectorButtonVariant.toggle:
            {
                return (toggledOn == false) ? new BorderSide(color: color) : null;
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color foregroundColor(BuildContext context)
    {
        Color primaryColor = _primaryColor(context);
        Color secondaryColor = _secondaryColor(context);
        switch (variant)
        {
            case InspectorButtonVariant.filled:
            {
                return primaryColor;
            }
            case InspectorButtonVariant.iconOnly:
            {
                return secondaryColor;
            }
            case InspectorButtonVariant.toggle:
            {
                return !DartRuntimePrimitives.RequireValue(toggledOn)
                    ? secondaryColor
                    : primaryColor;
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override Color backgroundColor(BuildContext context)
    {
        Color secondaryColor = _secondaryColor(context);
        switch (variant)
        {
            case InspectorButtonVariant.filled:
            {
                return secondaryColor;
            }
            case InspectorButtonVariant.iconOnly:
            {
                return Colors.transparent;
            }
            case InspectorButtonVariant.toggle:
            {
                return !DartRuntimePrimitives.RequireValue(toggledOn)
                    ? Colors.transparent
                    : secondaryColor;
            }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Color _primaryColor(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        return isDarkTheme
            ? theme.colorScheme.onPrimaryContainer
            : theme.colorScheme.primaryContainer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual Color _secondaryColor(BuildContext context)
    {
        ThemeData theme = Theme.of(context);
        return isDarkTheme
            ? theme.colorScheme.primaryContainer
            : theme.colorScheme.onPrimaryContainer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}
