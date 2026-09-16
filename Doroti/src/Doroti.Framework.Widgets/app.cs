// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/app.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Locale? LocaleListResolutionCallback(List<Locale>? locales, IEnumerable<Locale> supportedLocales);

public delegate Locale? LocaleResolutionCallback(Locale? locale, IEnumerable<Locale> supportedLocales);

public static partial class AppLibrary
{
    public static Locale basicLocaleListResolution(List<Locale>? preferredLocales, IEnumerable<Locale> supportedLocales)
    {
        if ((preferredLocales is null) || !Enumerable.Any(preferredLocales))
        {
            return supportedLocales.First();
        }
        DartMap<string, global::Doroti.Ui.Locale> allSupportedLocales = new DartMap<string, global::Doroti.Ui.Locale>().cast<string, global::Doroti.Ui.Locale>();
        DartMap<string, global::Doroti.Ui.Locale> languageAndCountryLocales = new DartMap<string, global::Doroti.Ui.Locale>().cast<string, global::Doroti.Ui.Locale>();
        DartMap<string, global::Doroti.Ui.Locale> languageAndScriptLocales = new DartMap<string, global::Doroti.Ui.Locale>().cast<string, global::Doroti.Ui.Locale>();
        DartMap<string, global::Doroti.Ui.Locale> languageLocales = new DartMap<string, global::Doroti.Ui.Locale>().cast<string, global::Doroti.Ui.Locale>();
        DartMap<string?, global::Doroti.Ui.Locale> countryLocales = new DartMap<string?, global::Doroti.Ui.Locale>().cast<string?, global::Doroti.Ui.Locale>();
        foreach (var locale in supportedLocales)
        {
            allSupportedLocales.putIfAbsent($"{locale.languageCode}_{locale.scriptCode}_{locale.countryCode}", () => locale);
            languageAndScriptLocales.putIfAbsent($"{locale.languageCode}_{locale.scriptCode}", () => locale);
            languageAndCountryLocales.putIfAbsent($"{locale.languageCode}_{locale.countryCode}", () => locale);
            languageLocales.putIfAbsent(locale.languageCode, () => locale);
            countryLocales.putIfAbsent(locale.countryCode, () => locale);
        }
        global::Doroti.Ui.Locale? matchesLanguageCode = default!;
        global::Doroti.Ui.Locale? matchesCountryCode = default!;
        for (var localeIndex = 0L; localeIndex < checked(preferredLocales.Count); localeIndex += 1L)
        {
            global::Doroti.Ui.Locale userLocale = preferredLocales[(int)localeIndex];
            if (allSupportedLocales.ContainsKey($"{userLocale.languageCode}_{userLocale.scriptCode}_{userLocale.countryCode}"))
            {
                return userLocale;
            }
            if (userLocale.scriptCode is not null)
            {
                global::Doroti.Ui.Locale? match = DartCollectionRuntime.NullableMapValue<Locale>(languageAndScriptLocales, $"{userLocale.languageCode}_{userLocale.scriptCode}");
                if (match is not null)
                {
                    Locale match__8388__value8497 = DartRuntimePrimitives.RequireValue(match);
                    return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(match__8388__value8497));
                }
            }
            if (userLocale.countryCode is not null)
            {
                global::Doroti.Ui.Locale? matchLocal = DartCollectionRuntime.NullableMapValue<Locale>(languageAndCountryLocales, $"{userLocale.languageCode}_{userLocale.countryCode}");
                if (matchLocal is not null)
                {
                    Locale match__8652__value8763 = DartRuntimePrimitives.RequireValue(matchLocal);
                    return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(match__8652__value8763));
                }
            }
            if (matchesLanguageCode is not null)
            {
                Locale matchesLanguageCode__7850__value9013 = DartRuntimePrimitives.RequireValue(matchesLanguageCode);
                return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(matchesLanguageCode__7850__value9013));
            }
            global::Doroti.Ui.Locale? matchAlternate = DartCollectionRuntime.NullableMapValue<Locale>(languageLocales, userLocale.languageCode);
            if (matchAlternate is not null)
            {
                Locale match__9139__value9197 = DartRuntimePrimitives.RequireValue(matchAlternate);
                matchesLanguageCode = DartRuntimePrimitives.RequireValue(match__9139__value9197);
                if ((localeIndex == 0L) && !(((localeIndex + 1L) < checked(preferredLocales.Count)) && (preferredLocales[(int)(localeIndex + 1L)].languageCode == userLocale.languageCode)))
                {
                    return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(matchesLanguageCode));
                }
            }
            if ((matchesCountryCode is null) && (userLocale.countryCode is not null))
            {
                matchAlternate = DartCollectionRuntime.NullableMapValue<Locale>(countryLocales, userLocale.countryCode);
                if (matchAlternate is not null)
                {
                    Locale match__9139__value10144 = DartRuntimePrimitives.RequireValue(matchAlternate);
                    matchesCountryCode = DartRuntimePrimitives.RequireValue(match__9139__value10144);
                }
            }
        }
        global::Doroti.Ui.Locale resolvedLocale = (matchesLanguageCode ?? matchesCountryCode) ?? supportedLocales.First();
        return resolvedLocale;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate string GenerateAppTitle(BuildContext context);

public delegate Route<object> PageRouteFactory(RouteSettings settings, global::System.Func<BuildContext, Widget> builder);

public delegate List<dynamic> InitialRouteListFactory(string initialRoute);

public class WidgetsApp : StatefulWidget
{
    public virtual GlobalKey<NavigatorState>? navigatorKey { get; private set; }
    public virtual global::System.Func<RouteSettings, dynamic?>? onGenerateRoute { get; private set; }
    public virtual global::System.Func<string, List<dynamic>>? onGenerateInitialRoutes { get; private set; }
    public virtual PageRouteFactory? pageRouteBuilder { get; private set; }
    public virtual object? routeInformationParser { get; private set; } = default!;
    public virtual global::Doroti.Framework.Widgets.IRouterDelegate? routerDelegate { get; private set; } = default!;
    public virtual BackButtonDispatcher? backButtonDispatcher { get; private set; }
    public virtual RouteInformationProvider? routeInformationProvider { get; private set; }
    public virtual IRouterConfig? routerConfig { get; private set; }
    public virtual Widget? home { get; private set; }
    public virtual DartMap<string, global::System.Func<BuildContext, Widget>>? routes { get; private set; }
    public virtual global::System.Func<RouteSettings, dynamic?>? onUnknownRoute { get; private set; }
    public virtual global::System.Func<NavigationNotification, bool>? onNavigationNotification { get; private set; }
    public virtual string? initialRoute { get; private set; }
    public virtual List<NavigatorObserver>? navigatorObservers { get; private set; }
    public virtual global::System.Func<BuildContext, Widget?, Widget>? builder { get; private set; }
    public virtual string? title { get; private set; }
    public virtual global::System.Func<BuildContext, string>? onGenerateTitle { get; private set; }
    public virtual global::Doroti.Framework.Painting.TextStyle? textStyle { get; private set; }
    public virtual Color color { get; private set; } = default!;
    public virtual Locale? locale { get; private set; }
    public virtual IEnumerable<dynamic>? localizationsDelegates { get; private set; }
    public virtual global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback { get; private set; }
    public virtual global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback { get; private set; }
    public virtual IEnumerable<Locale> supportedLocales { get; private set; } = default!;
    public virtual bool showPerformanceOverlay { get; private set; } = default!;
    public virtual bool showSemanticsDebugger { get; private set; } = default!;
    public virtual bool debugShowWidgetInspector { get; private set; } = default!;
    public virtual ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder { get; private set; }
    public virtual MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder { get; private set; }
    public virtual TapBehaviorButtonBuilder? tapBehaviorButtonBuilder { get; private set; }
    public virtual bool debugShowCheckedModeBanner { get; private set; } = default!;
    public virtual DartMap<ShortcutActivator, Intent>? shortcuts { get; private set; }
    public virtual DartMap<Type, dynamic>? actions { get; private set; }
    public virtual string? restorationScopeId { get; private set; }
    public virtual bool useInheritedMediaQuery { get; private set; } = default!;
    public static bool showPerformanceOverlayOverride = false;
    public static bool debugAllowBannerOverride = true;
    internal static DartMap<ShortcutActivator, Intent> _defaultShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.enter)] = new ActivateIntent(), [new SingleActivator(LogicalKeyboardKey.numpadEnter)] = new ActivateIntent(), [new SingleActivator(LogicalKeyboardKey.space)] = new ActivateIntent(), [new SingleActivator(LogicalKeyboardKey.gameButtonA)] = new ActivateIntent(), [new SingleActivator(LogicalKeyboardKey.select)] = new ActivateIntent(), [new SingleActivator(LogicalKeyboardKey.escape)] = new DismissIntent(), [new SingleActivator(LogicalKeyboardKey.tab)] = new NextFocusIntent(), [new SingleActivator(LogicalKeyboardKey.tab, shift: true)] = new PreviousFocusIntent(), [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = new DirectionalFocusIntent(TraversalDirection.left), [new SingleActivator(LogicalKeyboardKey.arrowRight)] = new DirectionalFocusIntent(TraversalDirection.right), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new DirectionalFocusIntent(TraversalDirection.down), [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new DirectionalFocusIntent(TraversalDirection.up), [new SingleActivator(LogicalKeyboardKey.arrowUp, control: true)] = new ScrollIntent(direction: AxisDirection.up), [new SingleActivator(LogicalKeyboardKey.arrowDown, control: true)] = new ScrollIntent(direction: AxisDirection.down), [new SingleActivator(LogicalKeyboardKey.arrowLeft, control: true)] = new ScrollIntent(direction: AxisDirection.left), [new SingleActivator(LogicalKeyboardKey.arrowRight, control: true)] = new ScrollIntent(direction: AxisDirection.right), [new SingleActivator(LogicalKeyboardKey.pageUp)] = new ScrollIntent(direction: AxisDirection.up, type: ScrollIncrementType.page), [new SingleActivator(LogicalKeyboardKey.pageDown)] = new ScrollIntent(direction: AxisDirection.down, type: ScrollIncrementType.page) };
    internal static DartMap<ShortcutActivator, Intent> _defaultWebShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.space)] = new PrioritizedIntents(orderedIntents: new List<Intent> { new ActivateIntent(), new ScrollIntent(direction: AxisDirection.down, type: ScrollIncrementType.page) }), [new SingleActivator(LogicalKeyboardKey.enter)] = new ButtonActivateIntent(), [new SingleActivator(LogicalKeyboardKey.numpadEnter)] = new ButtonActivateIntent(), [new SingleActivator(LogicalKeyboardKey.escape)] = new DismissIntent(), [new SingleActivator(LogicalKeyboardKey.tab)] = new NextFocusIntent(), [new SingleActivator(LogicalKeyboardKey.tab, shift: true)] = new PreviousFocusIntent(), [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new ScrollIntent(direction: AxisDirection.up), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new ScrollIntent(direction: AxisDirection.down), [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = new ScrollIntent(direction: AxisDirection.left), [new SingleActivator(LogicalKeyboardKey.arrowRight)] = new ScrollIntent(direction: AxisDirection.right), [new SingleActivator(LogicalKeyboardKey.pageUp)] = new ScrollIntent(direction: AxisDirection.up, type: ScrollIncrementType.page), [new SingleActivator(LogicalKeyboardKey.pageDown)] = new ScrollIntent(direction: AxisDirection.down, type: ScrollIncrementType.page) };
    internal static DartMap<ShortcutActivator, Intent> _defaultAppleOsShortcuts = new DartMap<ShortcutActivator, Intent> { [new SingleActivator(LogicalKeyboardKey.enter)] = new ActivateIntent(), [new SingleActivator(LogicalKeyboardKey.numpadEnter)] = new ActivateIntent(), [new SingleActivator(LogicalKeyboardKey.space)] = new ActivateIntent(), [new SingleActivator(LogicalKeyboardKey.escape)] = new DismissIntent(), [new SingleActivator(LogicalKeyboardKey.tab)] = new NextFocusIntent(), [new SingleActivator(LogicalKeyboardKey.tab, shift: true)] = new PreviousFocusIntent(), [new SingleActivator(LogicalKeyboardKey.arrowLeft)] = new DirectionalFocusIntent(TraversalDirection.left), [new SingleActivator(LogicalKeyboardKey.arrowRight)] = new DirectionalFocusIntent(TraversalDirection.right), [new SingleActivator(LogicalKeyboardKey.arrowDown)] = new DirectionalFocusIntent(TraversalDirection.down), [new SingleActivator(LogicalKeyboardKey.arrowUp)] = new DirectionalFocusIntent(TraversalDirection.up), [new SingleActivator(LogicalKeyboardKey.arrowUp, meta: true)] = new ScrollIntent(direction: AxisDirection.up), [new SingleActivator(LogicalKeyboardKey.arrowDown, meta: true)] = new ScrollIntent(direction: AxisDirection.down), [new SingleActivator(LogicalKeyboardKey.arrowLeft, meta: true)] = new ScrollIntent(direction: AxisDirection.left), [new SingleActivator(LogicalKeyboardKey.arrowRight, meta: true)] = new ScrollIntent(direction: AxisDirection.right), [new SingleActivator(LogicalKeyboardKey.pageUp)] = new ScrollIntent(direction: AxisDirection.up, type: ScrollIncrementType.page), [new SingleActivator(LogicalKeyboardKey.pageDown)] = new ScrollIntent(direction: AxisDirection.down, type: ScrollIncrementType.page) };
    public static DartMap<Type, dynamic> defaultActions = new DartMap<Type, dynamic> { [typeof(DoNothingIntent)] = new DoNothingAction(), [typeof(DoNothingAndStopPropagationIntent)] = new DoNothingAction(consumesKey: false), [typeof(RequestFocusIntent)] = new RequestFocusAction(), [typeof(NextFocusIntent)] = new NextFocusAction(), [typeof(PreviousFocusIntent)] = new PreviousFocusAction(), [typeof(DirectionalFocusIntent)] = new DirectionalFocusAction(), [typeof(ScrollIntent)] = new ScrollAction(), [typeof(PrioritizedIntents)] = new PrioritizedAction(), [typeof(VoidCallbackIntent)] = new VoidCallbackAction() };

    internal static DartMap<Type, dynamic> defaultActionsForContext(BuildContext context)
    {
        var actions = new DartMap<Type, dynamic>();
        actions.AddRange(defaultActions);
        actions[typeof(ScrollIntent)] = Action<ScrollIntent>.CreateOverridable(context: context, defaultAction: new ScrollAction());
        return actions;
    }


    public WidgetsApp(global::Doroti.Framework.Foundation.Key? key = null, GlobalKey<NavigatorState>? navigatorKey = null, global::System.Func<RouteSettings, dynamic?>? onGenerateRoute = null, global::System.Func<string, List<dynamic>>? onGenerateInitialRoutes = null, global::System.Func<RouteSettings, dynamic?>? onUnknownRoute = null, global::System.Func<NavigationNotification, bool>? onNavigationNotification = null, List<NavigatorObserver> navigatorObservers = default!, string? initialRoute = null, PageRouteFactory? pageRouteBuilder = null, Widget? home = null, DartMap<string, global::System.Func<BuildContext, Widget>> routes = default!, global::System.Func<BuildContext, Widget?, Widget>? builder = null, string? title = null, global::System.Func<BuildContext, string>? onGenerateTitle = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, Color color = default!, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool showPerformanceOverlay = false, bool showSemanticsDebugger = false, bool debugShowWidgetInspector = false, bool debugShowCheckedModeBanner = true, ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder = null, MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder = null, TapBehaviorButtonBuilder? tapBehaviorButtonBuilder = null, DartMap<ShortcutActivator, Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, bool useInheritedMediaQuery = false) : base(key: key)
    {
        List<NavigatorObserver> __navigatorObservers = navigatorObservers ?? new List<NavigatorObserver>();
        DartMap<string, global::System.Func<BuildContext, Widget>> __routes = routes ?? new DartMap<string, global::System.Func<BuildContext, Widget>>();
        IEnumerable<Locale> __supportedLocales = supportedLocales ?? new List<Locale> { new Locale("en", "US") };
        this.navigatorKey = navigatorKey;
        this.onGenerateRoute = onGenerateRoute;
        this.onGenerateInitialRoutes = onGenerateInitialRoutes;
        this.onUnknownRoute = onUnknownRoute;
        this.onNavigationNotification = onNavigationNotification;
        this.navigatorObservers = __navigatorObservers;
        this.initialRoute = initialRoute;
        this.pageRouteBuilder = pageRouteBuilder;
        this.home = home;
        this.routes = __routes;
        this.builder = builder;
        this.title = title;
        this.onGenerateTitle = onGenerateTitle;
        this.textStyle = textStyle;
        this.color = color;
        this.locale = locale;
        this.localizationsDelegates = localizationsDelegates;
        this.localeListResolutionCallback = localeListResolutionCallback;
        this.localeResolutionCallback = localeResolutionCallback;
        this.supportedLocales = __supportedLocales;
        this.showPerformanceOverlay = showPerformanceOverlay;
        this.showSemanticsDebugger = showSemanticsDebugger;
        this.debugShowWidgetInspector = debugShowWidgetInspector;
        this.debugShowCheckedModeBanner = debugShowCheckedModeBanner;
        this.exitWidgetSelectionButtonBuilder = exitWidgetSelectionButtonBuilder;
        this.moveExitWidgetSelectionButtonBuilder = moveExitWidgetSelectionButtonBuilder;
        this.tapBehaviorButtonBuilder = tapBehaviorButtonBuilder;
        this.shortcuts = shortcuts;
        this.actions = actions;
        this.restorationScopeId = restorationScopeId;
        this.useInheritedMediaQuery = useInheritedMediaQuery;
        routeInformationProvider = null;
        routeInformationParser = null;
        routerDelegate = null;
        backButtonDispatcher = null;
        routerConfig = null;
        System.Diagnostics.Debug.Assert((home is null) || (onGenerateInitialRoutes is null));
        System.Diagnostics.Debug.Assert((home is null) || !__routes.ContainsKey(Navigator.defaultRouteName));
        System.Diagnostics.Debug.Assert((builder is not null) || (home is not null) || __routes.ContainsKey(Navigator.defaultRouteName) || (onGenerateRoute is not null) || (onUnknownRoute is not null));
        System.Diagnostics.Debug.Assert((home is not null) || Enumerable.Any(__routes) || (onGenerateRoute is not null) || (onUnknownRoute is not null) || (builder is not null) && (navigatorKey is null) && (initialRoute is null) && !Enumerable.Any(__navigatorObservers));
        System.Diagnostics.Debug.Assert((builder is not null) || (onGenerateRoute is not null) || (pageRouteBuilder is not null));
        System.Diagnostics.Debug.Assert(Enumerable.Any(__supportedLocales));
    }

    public static WidgetsApp CreateRouter(global::Doroti.Framework.Foundation.Key? key = null, RouteInformationProvider? routeInformationProvider = null, object? routeInformationParser = null, global::Doroti.Framework.Widgets.IRouterDelegate? routerDelegate = null, IRouterConfig? routerConfig = null, BackButtonDispatcher? backButtonDispatcher = null, global::System.Func<BuildContext, Widget?, Widget>? builder = null, string? title = null, global::System.Func<BuildContext, string>? onGenerateTitle = null, global::System.Func<NavigationNotification, bool>? onNavigationNotification = null, global::Doroti.Framework.Painting.TextStyle? textStyle = null, Color color = default!, Locale? locale = null, IEnumerable<dynamic>? localizationsDelegates = null, global::System.Func<List<Locale>?, IEnumerable<Locale>, Locale?>? localeListResolutionCallback = null, global::System.Func<Locale?, IEnumerable<Locale>, Locale?>? localeResolutionCallback = null, IEnumerable<Locale> supportedLocales = default!, bool showPerformanceOverlay = false, bool showSemanticsDebugger = false, bool debugShowWidgetInspector = false, bool debugShowCheckedModeBanner = true, ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder = null, MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder = null, TapBehaviorButtonBuilder? tapBehaviorButtonBuilder = null, DartMap<ShortcutActivator, Intent>? shortcuts = null, DartMap<Type, dynamic>? actions = null, string? restorationScopeId = null, bool useInheritedMediaQuery = false)
    {
        var __instance = new WidgetsApp(key, default!, default!, default!, default!, onNavigationNotification, default!, default!, default!, default!, default!, builder, title, onGenerateTitle, textStyle, color, locale, localizationsDelegates, localeListResolutionCallback, localeResolutionCallback, supportedLocales, showPerformanceOverlay, showSemanticsDebugger, debugShowWidgetInspector, debugShowCheckedModeBanner, exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder, shortcuts, actions, restorationScopeId, useInheritedMediaQuery);
        IEnumerable<Locale> __supportedLocales = supportedLocales ?? new List<Locale> { new Locale("en", "US") };
        __instance.routeInformationProvider = routeInformationProvider;
        __instance.routeInformationParser = routeInformationParser;
        __instance.routerDelegate = routerDelegate;
        __instance.routerConfig = routerConfig;
        __instance.backButtonDispatcher = backButtonDispatcher;
        __instance.builder = builder;
        __instance.title = title;
        __instance.onGenerateTitle = onGenerateTitle;
        __instance.onNavigationNotification = onNavigationNotification;
        __instance.textStyle = textStyle;
        __instance.color = color;
        __instance.locale = locale;
        __instance.localizationsDelegates = localizationsDelegates;
        __instance.localeListResolutionCallback = localeListResolutionCallback;
        __instance.localeResolutionCallback = localeResolutionCallback;
        __instance.supportedLocales = __supportedLocales;
        __instance.showPerformanceOverlay = showPerformanceOverlay;
        __instance.showSemanticsDebugger = showSemanticsDebugger;
        __instance.debugShowWidgetInspector = debugShowWidgetInspector;
        __instance.debugShowCheckedModeBanner = debugShowCheckedModeBanner;
        __instance.exitWidgetSelectionButtonBuilder = exitWidgetSelectionButtonBuilder;
        __instance.moveExitWidgetSelectionButtonBuilder = moveExitWidgetSelectionButtonBuilder;
        __instance.tapBehaviorButtonBuilder = tapBehaviorButtonBuilder;
        __instance.shortcuts = shortcuts;
        __instance.actions = actions;
        __instance.restorationScopeId = restorationScopeId;
        __instance.useInheritedMediaQuery = useInheritedMediaQuery;
        __instance.navigatorObservers = null;
        __instance.navigatorKey = null;
        __instance.onGenerateRoute = null;
        __instance.pageRouteBuilder = null;
        __instance.home = null;
        __instance.onGenerateInitialRoutes = null;
        __instance.onUnknownRoute = null;
        __instance.routes = null;
        __instance.initialRoute = null;
        return __instance;
    }

    public static bool debugShowWidgetInspectorOverride
    {
        get
        {
            return WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier.value;
        }
        set
        {
            var __value = value;
            WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier.value = __value;
        }
    }
    public static DartMap<ShortcutActivator, Intent> defaultShortcuts
    {
        get
        {
            if (Foundation.ConstantsLibrary.kIsWeb)
            {
                return _defaultWebShortcuts;
            }
            switch (PlatformLibrary.defaultTargetPlatform)
            {
                case TargetPlatform.android:
                case TargetPlatform.fuchsia:
                case TargetPlatform.linux:
                case TargetPlatform.windows:
                    {
                        return _defaultShortcuts;
                    }
                case TargetPlatform.iOS:
                case TargetPlatform.macOS:
                    {
                        return _defaultAppleOsShortcuts;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
        }
    }
    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WidgetsAppState__app());
}

internal class _WidgetsAppState__app : State<WidgetsApp>, WidgetsBindingObserver
{
    internal virtual AppLifecycleState? _appLifecycleState { get; set; } = default;
    internal virtual PlatformRouteInformationProvider? _defaultRouteInformationProvider { get; set; } = default;
    internal virtual RootBackButtonDispatcher? _defaultBackButtonDispatcher { get; set; } = default;
    internal virtual GlobalKey<NavigatorState>? _navigator { get; set; } = default;
    private bool __late__localizationsResolver_initialized;
    private LocalizationsResolver __late__localizationsResolver = default!;
    internal virtual LocalizationsResolver _localizationsResolver
    {
        get
        {
            if (!__late__localizationsResolver_initialized)
            {
                __late__localizationsResolver = new LocalizationsResolver(locale: widget.locale, localeListResolutionCallback: widget.localeListResolutionCallback, localeResolutionCallback: widget.localeResolutionCallback, localizationsDelegates: widget.localizationsDelegates, supportedLocales: widget.supportedLocales.Cast<Locale>());
                __late__localizationsResolver_initialized = true;
            }
            return __late__localizationsResolver;
        }
    }

    internal virtual string _initialRouteName => (WidgetsBinding.instance.platformDispatcher.defaultRouteName != Navigator.defaultRouteName) ? WidgetsBinding.instance.platformDispatcher.defaultRouteName : (widget.initialRoute ?? WidgetsBinding.instance.platformDispatcher.defaultRouteName);
    internal virtual bool _defaultOnNavigationNotification(NavigationNotification notification)
    {
        switch (_appLifecycleState)
        {
            case null:
            case var __constant63443 when Equals(__constant63443, AppLifecycleState.detached):
                {
                    return true;
                }
            case var __constant63566 when Equals(__constant63566, AppLifecycleState.inactive):
            case var __constant63605 when Equals(__constant63605, AppLifecycleState.resumed):
            case var __constant63643 when Equals(__constant63643, AppLifecycleState.hidden):
            case var __constant63680 when Equals(__constant63680, AppLifecycleState.paused):
                {
                    DartRuntimePrimitives.Ignore(SystemNavigator.setFrameworkHandlesBack(notification.canHandlePop));
                    return true;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void didChangeAppLifecycleState(AppLifecycleState state)
    {
        _appLifecycleState = state;
        base.didChangeAppLifecycleState(state);
    }

    public override void initState()
    {
        base.initState();
        _updateRouting();
        WidgetsBinding.instance.addObserver(this);
        _appLifecycleState = WidgetsBinding.instance.lifecycleState;
    }

    public override void didUpdateWidget(WidgetsApp oldWidget)
    {
        base.didUpdateWidget(oldWidget);
        _updateRouting(oldWidget: oldWidget);
        _updateLocalizations(oldWidget: oldWidget);
    }

    public override void dispose()
    {
        WidgetsBinding.instance.removeObserver(this);
        _defaultRouteInformationProvider?.dispose();
        _localizationsResolver.dispose();
        base.dispose();
    }

    internal virtual void _clearRouterResource()
    {
        _defaultRouteInformationProvider?.dispose();
        _defaultRouteInformationProvider = null;
        _defaultBackButtonDispatcher = null;
    }

    internal virtual void _clearNavigatorResource()
    {
        _navigator = null;
    }

    internal virtual void _updateRouting(WidgetsApp? oldWidget = null)
    {
        if (_usesRouterWithDelegates)
        {
            DartRuntimePrimitives.Assert(() => !_usesNavigator && !_usesRouterWithConfig);
            _clearNavigatorResource();
            if ((widget.routeInformationProvider is null) && (widget.routeInformationParser is not null))
            {
                _defaultRouteInformationProvider ??= new PlatformRouteInformationProvider(initialRouteInformation: new RouteInformation(uri: DartUri.parse(_initialRouteName)));
            }
            else
            {
                _defaultRouteInformationProvider?.dispose();
                _defaultRouteInformationProvider = null;
            }
            if (widget.backButtonDispatcher is null)
            {
                _defaultBackButtonDispatcher ??= new RootBackButtonDispatcher();
            }
        }
        else
        {
            if (_usesNavigator)
            {
                DartRuntimePrimitives.Assert(() => !_usesRouterWithDelegates && !_usesRouterWithConfig);
                _clearRouterResource();
                if ((_navigator is null) || (!Equals(widget.navigatorKey, oldWidget!.navigatorKey)))
                {
                    _navigator = widget.navigatorKey ?? new GlobalObjectKey<NavigatorState>(this);
                }
                DartRuntimePrimitives.Assert(() => _navigator is not null);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => (widget.builder is not null) || _usesRouterWithConfig);
                DartRuntimePrimitives.Assert(() => !_usesRouterWithDelegates && !_usesNavigator);
                _clearRouterResource();
                _clearNavigatorResource();
            }
        }
        DartRuntimePrimitives.Assert(() => _usesNavigator == _navigator is not null);
    }

    internal virtual bool _usesRouterWithDelegates => DartRuntimePrimitives.ConvertValue<bool>(widget.routerDelegate is not null);
    internal virtual bool _usesRouterWithConfig => DartRuntimePrimitives.ConvertValue<bool>(widget.routerConfig is not null);
    internal virtual bool _usesNavigator => DartRuntimePrimitives.ConvertValue<bool>((((WidgetsApp)widget).home is not null) || ((((WidgetsApp)widget).routes is { } __items66337 ? System.Linq.Enumerable.Any(__items66337) : (bool?)null) ?? false) || (((WidgetsApp)widget).onGenerateRoute is not null) || (((WidgetsApp)widget).onUnknownRoute is not null));
    internal virtual RouteInformationProvider? _effectiveRouteInformationProvider => DartRuntimePrimitives.ConvertValue<RouteInformationProvider>(widget.routeInformationProvider ?? _defaultRouteInformationProvider);
    internal virtual BackButtonDispatcher _effectiveBackButtonDispatcher => DartRuntimePrimitives.ConvertValue<BackButtonDispatcher>(widget.backButtonDispatcher ?? _defaultBackButtonDispatcher!);
    internal virtual dynamic? _onGenerateRoute(RouteSettings settings)
    {
        string? nameLocal = settings.name;
        global::System.Func<BuildContext, Widget>? pageContentBuilder = ((nameLocal == Navigator.defaultRouteName) && (widget.home is not null)) ? ((context) => widget.home!) : widget.routes!.GetValueOrDefault(DartRuntimePrimitives.RequireReference(nameLocal));
        if (pageContentBuilder is not null)
        {
            DartRuntimePrimitives.Assert(() => widget.pageRouteBuilder is not null, () => (object?)"The default onGenerateRoute handler for WidgetsApp must have a " + "pageRouteBuilder set if the home or routes properties are set.");
            dynamic route = widget.pageRouteBuilder!(settings, pageContentBuilder);
            return route;
        }
        if (widget.onGenerateRoute is not null)
        {
            return widget.onGenerateRoute!(settings);
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual dynamic _onUnknownRoute(RouteSettings settings)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                if (widget.onUnknownRoute is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create($"Could not find a generator for route {settings} in the {GetType()}.\n" + "Make sure your root app widget has provided a way to generate \n" + "this route.\n" + "Generators for routes are searched for in the following order:\n" + " 1. For the \"/\" route, the \"home\" property, if non-null, is used.\n" + " 2. Otherwise, the \"routes\" table is used, if it has an entry for " + "the route.\n" + " 3. Otherwise, onGenerateRoute is called. It should return a " + "non-null value for any valid route not handled by \"home\" and \"routes\".\n" + " 4. Finally if all else fails onUnknownRoute is called.\n" + "Unfortunately, onUnknownRoute was not set."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        dynamic result = widget.onUnknownRoute!(settings) ?? throw new InvalidOperationException("The onUnknownRoute callback must return a route.");
        DartRuntimePrimitives.Assert(() =>
            {
                if (result is null)
                {
                    throw DartRuntimePrimitives.AsException(FlutterError.Create("The onUnknownRoute callback returned null.\n" + $"When the {GetType()} requested the route {settings} from its " + "onUnknownRoute callback, the callback returned null. Such callbacks " + "must never return null."));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return result!;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<bool> didPopRoute()
    {
        DartRuntimePrimitives.Assert(() => mounted);
        if (_usesRouterWithDelegates)
        {
            return false;
        }
        NavigatorState? navigator = _navigator?.currentState;
        if (navigator is null)
        {
            return false;
        }
        return await navigator.maybePop<object>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<bool> didPushRouteInformation(RouteInformation routeInformation)
    {
        DartRuntimePrimitives.Assert(() => mounted);
        if (_usesRouterWithDelegates)
        {
            return false;
        }
        NavigatorState? navigator = _navigator?.currentState;
        if (navigator is null)
        {
            return false;
        }
        DartUri uriLocal = routeInformation.uri;
        DartRuntimePrimitives.Ignore(navigator.pushNamed<object>(Dart_coreLibrary.decodeComponent(new DartUri(path: (uriLocal.path.Length == 0) ? "/" : uriLocal.path, queryParameters: !Enumerable.Any(uriLocal.queryParametersAll) ? null : uriLocal.queryParametersAll, fragment: (uriLocal.fragment.Length == 0) ? null : uriLocal.fragment).ToString())));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _shouldUpdateLocalizations(WidgetsApp oldWidget)
    {
        return (!Equals(widget.locale, oldWidget.locale)) || (!Equals(widget.localeListResolutionCallback, oldWidget.localeListResolutionCallback)) || (!Equals(widget.localeResolutionCallback, oldWidget.localeResolutionCallback)) || (!Equals(widget.supportedLocales, oldWidget.supportedLocales)) || (!Equals(widget.localizationsDelegates, oldWidget.localizationsDelegates));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateLocalizations(WidgetsApp oldWidget)
    {
        if (_shouldUpdateLocalizations(oldWidget))
        {
            _localizationsResolver.update(locale: widget.locale, localeListResolutionCallback: widget.localeListResolutionCallback, localeResolutionCallback: widget.localeResolutionCallback, localizationsDelegates: widget.localizationsDelegates, supportedLocales: widget.supportedLocales.Cast<Locale>());
        }
    }

    public override Widget build(BuildContext context)
    {
        Widget? routing = default!;
        if (_usesRouterWithDelegates)
        {
            routing = DartRuntimePrimitives.ConvertValue<Widget>(widget.routerDelegate!.createRouterWidget(_effectiveRouteInformationProvider, widget.routeInformationParser, _effectiveBackButtonDispatcher, "router"));
        }
        else
        {
            if (_usesNavigator)
            {
                DartRuntimePrimitives.Assert(() => _navigator is not null);
                routing = DartRuntimePrimitives.ConvertValue<Widget>(new FocusScope(debugLabel: "Navigator Scope", autofocus: true, child: new Navigator(clipBehavior: Clip.none, restorationScopeId: "nav", key: _navigator, initialRoute: _initialRouteName, onGenerateRoute: _onGenerateRoute, onGenerateInitialRoutes: (widget.onGenerateInitialRoutes is null) ? Navigator.defaultGenerateInitialRoutes : ((navigator, initialRouteName) =>
                {
                    return widget.onGenerateInitialRoutes!(initialRouteName);
                    throw new InvalidOperationException("Dart closure completed without a value.");
                }), onUnknownRoute: _onUnknownRoute, observers: widget.navigatorObservers!, routeTraversalEdgeBehavior: Foundation.ConstantsLibrary.kIsWeb ? TraversalEdgeBehavior.leaveDorotiView : TraversalEdgeBehavior.parentScope, reportsRouteUpdateToEngine: true)));
            }
            else
            {
                if (_usesRouterWithConfig)
                {
                    routing = DartRuntimePrimitives.ConvertValue<Widget>(widget.routerConfig!.createRouterWidget(restorationScopeId: "router"));
                }
            }
        }
        Widget result = default!;
        if (widget.builder is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new Builder(builder: (context) =>
            {
                return widget.builder!(context, routing);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
        else
        {
            DartRuntimePrimitives.Assert(() => routing is not null);
            result = routing!;
        }
        if (widget.textStyle is not null)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new DefaultTextStyle(style: widget.textStyle!, child: result));
        }
        if (widget.showPerformanceOverlay || WidgetsApp.showPerformanceOverlayOverride)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new Stack(children: new List<Widget> { result, new Positioned(top: 0.0, left: 0.0, right: 0.0, child: PerformanceOverlay.CreateAllEnabled()) }));
        }
        if (widget.showSemanticsDebugger)
        {
            result = DartRuntimePrimitives.ConvertValue<Widget>(new SemanticsDebugger(child: result));
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (!WidgetsBinding.instance.debugExcludeRootWidgetInspector)
                {
                    result = DartRuntimePrimitives.ConvertValue<Widget>(new ValueListenableBuilder<bool>(valueListenable: WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier, builder: (context, debugShowWidgetInspectorOverride, child) =>
                    {
                        if (widget.debugShowWidgetInspector || debugShowWidgetInspectorOverride)
                        {
                            return new WidgetInspector(exitWidgetSelectionButtonBuilder: widget.exitWidgetSelectionButtonBuilder, moveExitWidgetSelectionButtonBuilder: widget.moveExitWidgetSelectionButtonBuilder, tapBehaviorButtonBuilder: widget.tapBehaviorButtonBuilder, child: child!);
                        }
                        return child!;
                        throw new InvalidOperationException("Dart closure completed without a value.");
                    }, child: result));
                }
                if (widget.debugShowCheckedModeBanner && WidgetsApp.debugAllowBannerOverride)
                {
                    result = DartRuntimePrimitives.ConvertValue<Widget>(new CheckedModeBanner(child: result));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        result = DartRuntimePrimitives.ConvertValue<Widget>(new Focus(canRequestFocus: false, onKeyEvent: (node, @event) =>
        {
            if ((@event is not KeyDownEvent) && (@event is not KeyRepeatEvent) || (!Equals(@event.logicalKey, LogicalKeyboardKey.escape)))
            {
                return KeyEventResult.ignored;
            }
            return RawTooltip.dismissAllToolTips() ? KeyEventResult.handled : KeyEventResult.ignored;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, child: result));
        Widget? titleLocal = default!;
        if (widget.onGenerateTitle is not null)
        {
            titleLocal = DartRuntimePrimitives.ConvertValue<Widget>(new Builder(builder: (context) =>
            {
                string titleAlternate = widget.onGenerateTitle!(context);
                return new Title(title: titleAlternate, color: widget.color.withOpacity(1.0), child: result);
                throw new InvalidOperationException("Dart closure completed without a value.");
            }));
        }
        else
        {
            if ((widget.title is null) && Foundation.ConstantsLibrary.kIsWeb)
            {
                titleLocal = null;
            }
            else
            {
                titleLocal = DartRuntimePrimitives.ConvertValue<Widget>(new Title(title: widget.title ?? "", color: widget.color.withOpacity(1.0), child: result));
            }
        }
        return new RootRestorationScope(restorationId: widget.restorationScopeId, child: new SharedAppData(child: new NotificationListener<NavigationNotification>(onNotification: widget.onNavigationNotification ?? _defaultOnNavigationNotification, child: new Shortcuts(debugLabel: "<Default WidgetsApp Shortcuts>", shortcuts: widget.shortcuts ?? WidgetsApp.defaultShortcuts, child: new DefaultTextEditingShortcuts(child: new Actions(actions: widget.actions ?? WidgetsApp.defaultActionsForContext(context), child: new FocusTraversalGroup(policy: new ReadingOrderTraversalPolicy(), child: new TapRegionSurface(child: new ShortcutRegistrar(child: new ListenableBuilder(listenable: _localizationsResolver, builder: (context, _) =>
        {
            return new Localizations(isApplicationLevel: true, locale: _localizationsResolver.locale, delegates: _localizationsResolver.localizationsDelegates.ToList(), child: titleLocal ?? result);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))))))))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

