using Doroti.Framework;
using Doroti.Framework.Foundation;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using DorotiDemoApp.Components;
using M = Doroti.Material;

namespace DorotiDemoApp;

internal static class App
{
    internal static Func<IDorotiViewEntrypoint> Definition =>
        () => new DorotiWidgetEntrypoint(() => new DemoApplication(), PrepareResourcesAsync);

    internal static DorotiViewConfiguration ViewConfiguration { get; } =
        new(
            "Doroti Demo",
            new Size(1280, 900),
            new Color(0xfffffbfe),
            new Color(0xff141218),
            terminateAfterLastWindowClosed: true
        );

    private static async Task PrepareResourcesAsync()
    {
        foreach (
            var weight in Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb
                ? new[] { "regular" }
                : new[] { "medium", "bold", "regular" }
        )
        {
            using var stream =
                typeof(App).Assembly.GetManifestResourceStream(
                    $"MaterialSample.Roboto-{weight}.ttf"
                ) ?? throw new InvalidOperationException($"Roboto {weight} resource is missing.");
            using var bytes = new MemoryStream();
            stream.CopyTo(bytes);
            await DorotiUiLibrary.loadFontFromList(
                new Uint8List(bytes.ToArray()),
                fontFamily: "Roboto"
            );
        }
    }
}

internal sealed class DemoApplication : StatefulWidget
{
    public override IState createState() => new DemoApplicationState();
}

internal sealed class DemoApplicationState : State<DemoApplication>
{
    private M.ThemeMode _mode = M.ThemeMode.system;
    private M.ThemeData? _light;
    private M.ThemeData? _dark;

    private void ToggleBrightness()
    {
        var dark =
            _mode == M.ThemeMode.dark
            || (
                _mode == M.ThemeMode.system
                && View.of(context).platformDispatcher.platformBrightness == Brightness.dark
            );
        setState(() => _mode = dark ? M.ThemeMode.light : M.ThemeMode.dark);
    }

    public override Widget build(BuildContext context) =>
        new M.MaterialApp(
            title: "Doroti Demo",
            initialRoute: "/",
            debugShowCheckedModeBanner: false,
            locale: new Locale("en", "US"),
            themeFactory: () =>
                _light ??= M.ThemeData.Create(
                    fontFamilyFallback: ["Roboto"],
                    colorSchemeSeed: new Color(0xff6750a4)
                ),
            darkThemeFactory: () =>
                _dark ??= M.ThemeData.Create(
                    fontFamilyFallback: ["Roboto"],
                    colorSchemeSeed: new Color(0xff6750a4),
                    brightness: Brightness.dark
                ),
            themeMode: _mode,
            builder: (themeContext, child) =>
                new AnnotatedRegion<SystemUiOverlayStyle>(
                    value: SystemOverlayStyle(M.Theme.of(themeContext)),
                    child: child ?? SizedBox.CreateShrink()
                ),
            home: new ComponentsHome(ToggleBrightness)
        );

    internal static SystemUiOverlayStyle SystemOverlayStyle(M.ThemeData theme) =>
        new(
            statusBarColor: theme.appBarTheme.backgroundColor ?? theme.colorScheme.surface,
            statusBarBrightness: theme.brightness,
            statusBarIconBrightness: theme.brightness == Brightness.dark
                ? Brightness.light
                : Brightness.dark,
            systemNavigationBarColor: theme.navigationBarTheme.backgroundColor
                ?? theme.colorScheme.surfaceContainer,
            systemNavigationBarIconBrightness: theme.brightness == Brightness.dark
                ? Brightness.light
                : Brightness.dark,
            systemStatusBarContrastEnforced: false,
            systemNavigationBarContrastEnforced: false
        );
}

internal sealed class ComponentsHome(Action toggleBrightness) : StatefulWidget
{
    internal Action ToggleBrightness => toggleBrightness;

    public override IState createState() => new ComponentsHomeState();
}

internal sealed class ComponentsHomeState : State<ComponentsHome>
{
    private readonly GlobalKey<M.ScaffoldState> _scaffold = new();
    private readonly GlobalKey<ComponentsState> _components = new();

    public override Widget build(BuildContext context)
    {
        var theme = M.Theme.of(context);
        var dark = theme.brightness == Brightness.dark;
        return new M.Scaffold(
            key: _scaffold,
            appBar: new M.AppBar(
                title: new Text("Components"),
                systemOverlayStyle: DemoApplicationState.SystemOverlayStyle(theme),
                notificationPredicate: notification =>
                    Scroll_notificationLibrary.defaultScrollNotificationPredicate(notification)
                    && _components.currentState?.OwnsScrollNotification(notification) == true,
                actions:
                [
                    new M.IconButton(
                        icon: new Icon(
                            dark ? M.Icons.light_mode_outlined : M.Icons.dark_mode_outlined
                        ),
                        tooltip: dark ? "Switch to light mode" : "Switch to dark mode",
                        onPressed: widget.ToggleBrightness
                    ),
                ]
            ),
            endDrawer: new GalleryDrawer(),
            body: new SafeArea(
                top: false,
                child: new LayoutBuilder(
                    builder: (_, constraints) =>
                        new ComponentsScreen(
                            twoColumns: constraints.maxWidth > 1000,
                            scaffold: _scaffold,
                            key: _components
                        )
                )
            )
        );
    }
}
