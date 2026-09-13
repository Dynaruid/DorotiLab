using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Material = Doroti.Framework.Material;
using ListView = Doroti.Framework.Widgets.ListView;
using Locale = Doroti.Ui.Locale;
using Rect = Doroti.Ui.Rect;
using Semantics = Doroti.Framework.Widgets.Semantics;
using Size = Doroti.Ui.Size;
using UiColor = Doroti.Ui.Color;

internal sealed class MaterialDemoEntrypoint(DemoEntryMode entryMode, bool requireExternalUia) : IDorotiViewEntrypoint
{
    private WidgetsFlutterBinding? _binding;
    private Doroti.Framework.DorotiWidgetEntrypoint? _widgetEntrypoint;
    private DorotiView? _view;

    internal Material.Scaffold? RootScaffold { get; private set; }
    internal MaterialGalleryState? GalleryState { get; private set; }
    internal Widget RootApp => _rootApp ??= CreateRootApp();
    internal DemoEntryMode EntryMode { get; } = entryMode;
    internal bool RequireExternalUia { get; } = requireExternalUia;
#if DOROTI_LEGACY_DESKTOP
    internal DesktopFrameworkPixelReadback? InitialReadback { get; set; }
    internal DesktopFrameworkPixelReadback? ChangedReadback { get; set; }
    internal DesktopFrameworkPixelReadback? BackdropOnReadback { get; set; }
    internal DesktopFrameworkPixelReadback? BackdropOffReadback { get; set; }
#endif
    internal string? InitialStateSignature { get; set; }
    internal string? ChangedStateSignature { get; set; }
    internal long CadencePresented { get; set; }
    internal TimeSpan CadenceDuration { get; set; }
    internal int NativePointerInteractionCount { get; set; }
    internal IReadOnlyList<string> NativePointerHitTestTargets { get; set; } = [];
    internal Offset? NativeEffectTogglePoint { get; set; }
    internal Rect? NativeEffectPanelBounds { get; set; }
    internal IReadOnlyList<string> NativeEffectHitTestTargets { get; set; } = [];

    private Widget? _rootApp;

    internal FlutterErrorDetails? FirstFrameworkError { get; private set; }

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details =>
        {
            FirstFrameworkError ??= details;
            Console.Error.WriteLine(details.exceptionThrown);
        };
        _widgetEntrypoint = new Doroti.Framework.DorotiWidgetEntrypoint(() => RootApp, PrepareResourcesAsync);
        _widgetEntrypoint.Bootstrap(dispatcher);
        _binding = (WidgetsFlutterBinding)WidgetsFlutterBinding.ensureInitialized();
    }

    public void AttachView(DorotiView view)
    {
        if (_binding is null)
        {
            throw new InvalidOperationException("The Material framework binding was not bootstrapped.");
        }
        if (_view is not null)
        {
            throw new InvalidOperationException("DorotiTestbedApp owns exactly one Doroti view.");
        }

        _view = view;
        _widgetEntrypoint!.AttachView(view);
    }

    private static Task PrepareResourcesAsync() =>
        App.SampleEnabled ? PrepareSampleResourcesAsync() : Task.CompletedTask;

    internal static async Task PrepareSampleResourcesAsync()
    {
        using var stream = typeof(MaterialDemoEntrypoint).Assembly.GetManifestResourceStream("MaterialSample.icons.otf")
            ?? throw new InvalidOperationException("MaterialIcons resource is missing.");
        using var bytes = new MemoryStream(); stream.CopyTo(bytes);
        await Dart_uiLibrary.loadFontFromList(new Uint8List(bytes.ToArray()), fontFamily: "MaterialIcons");
        // Flutter Web registers its regular Roboto fallback; native hosts also use weight faces.
        foreach (var weight in (Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb ? new[] { "regular" } : new[] { "medium", "bold", "regular" }))
        {
            using var fontStream = typeof(MaterialDemoEntrypoint).Assembly.GetManifestResourceStream($"MaterialSample.Roboto-{weight}.ttf")
                ?? throw new InvalidOperationException($"Roboto {weight} resource is missing.");
            using var fontBytes = new MemoryStream(); fontStream.CopyTo(fontBytes);
            await Dart_uiLibrary.loadFontFromList(new Uint8List(fontBytes.ToArray()), fontFamily: "Roboto");
        }
    }

    public void DetachView(DorotiView view)
    {
        _widgetEntrypoint?.DetachView(view);
        if (ReferenceEquals(_view, view))
        {
            _view = null;
        }
    }

    internal void ExerciseAll() =>
        (GalleryState ?? throw new InvalidOperationException("The Material gallery State is not mounted.")).ExerciseAll();

    internal void RequestFrame()
    {
        if (GalleryState is { } galleryState)
        {
            galleryState.PulseFrame();
            return;
        }
        (_binding ?? throw new InvalidOperationException("The Material binding is not initialized.")).scheduleFrame();
    }

    internal IReadOnlyList<string> HitTestTargetsAt(double x, double y)
    {
        var binding = _binding ?? throw new InvalidOperationException("The Material binding is not initialized.");
        var result = new Doroti.Framework.Gestures.HitTestResult();
        binding.hitTestInView(
            result,
            new Offset(x, y),
            checked((long)(_view ?? throw new InvalidOperationException("The Doroti view is not attached.")).viewId));
        return result.path.Select(entry => entry.target.GetType().FullName ?? entry.target.GetType().Name).ToArray();
    }

    internal Rect BackdropPanelPhysicalBounds()
    {
        var view = _view ?? throw new InvalidOperationException("The Doroti view is not attached.");
        var logical = (GalleryState ?? throw new InvalidOperationException("The Material gallery State is not mounted."))
            .BackdropPanelBounds();
        var scale = view.devicePixelRatio;
        return new Rect(logical.left * scale, logical.top * scale, logical.right * scale, logical.bottom * scale);
    }

    public void Shutdown()
    {
        _widgetEntrypoint?.Shutdown();
        _widgetEntrypoint = null;
        _binding = null;
        _view = null;
        FlutterError.onError = null;
    }

    private Widget CreateRootApp()
    {
        if (Environment.GetEnvironmentVariable("DOROTI_TESTBED_MODE") == "platform-views")
            return new Material.MaterialApp(debugShowCheckedModeBanner: false, home: new PlatformViewFixture());
        if (Environment.GetEnvironmentVariable("DOROTI_TESTBED_MODE") == "media-query")
            return new Material.MaterialApp(debugShowCheckedModeBanner: false, home: new MediaQueryFixture());
        if (App.SampleEnabled) return new MaterialSample.SampleApp(App.SampleAcrylicAvailable);
        Widget Gallery() => Environment.GetEnvironmentVariable("DOROTI_RESIZE_FIXTURE") is "F0" or "F1" or "F2"
            ? new ResizeFixture(Environment.GetEnvironmentVariable("DOROTI_RESIZE_FIXTURE")!)
            : new MaterialGallery(
                state => GalleryState = state,
                scaffold => RootScaffold = scaffold);

        return EntryMode == DemoEntryMode.Builder
            ? new Material.MaterialApp(
                title: "Doroti Material Testbed",
                color: new UiColor(0xff6750a4L),
                themeFactory: () => DemoTheme.Create(
                    Brightness.light, App.AcrylicEnabled),
                darkThemeFactory: () => DemoTheme.Create(
                    Brightness.dark, App.AcrylicEnabled),
                themeMode: Material.ThemeMode.system,
                locale: new Locale("en", "US"),
                debugShowCheckedModeBanner: false,
                builder: (_, _) => new Overlay(initialEntries:
                [
                    new OverlayEntry(builder: _ => Gallery()),
                ]))
            : new Material.MaterialApp(
                title: "Doroti Material Testbed",
                color: new UiColor(0xff6750a4L),
                themeFactory: () => DemoTheme.Create(
                    Brightness.light, App.AcrylicEnabled),
                darkThemeFactory: () => DemoTheme.Create(
                    Brightness.dark, App.AcrylicEnabled),
                themeMode: Material.ThemeMode.system,
                locale: new Locale("en", "US"),
                debugShowCheckedModeBanner: false,
                home: Gallery());
    }
}

internal enum DemoEntryMode { Builder, Home }

internal static class App
{
    internal static Func<IDorotiViewEntrypoint> Definition =>
        () => new MaterialDemoEntrypoint(DemoEntryMode.Home, requireExternalUia: false);

    internal static bool ExperimentalAcrylicEnabled => string.Equals(
        Environment.GetEnvironmentVariable("DOROTI_DEMO_EXPERIMENTAL_ACRYLIC"),
        "1", StringComparison.Ordinal);

    internal static bool SampleEnabled =>
        Environment.GetEnvironmentVariable("DOROTI_RESIZE_FIXTURE") is not ("F0" or "F1" or "F2") &&
        string.Equals(Environment.GetEnvironmentVariable("DOROTI_TESTBED_MODE"), "sample", StringComparison.OrdinalIgnoreCase);

    internal static bool SampleAcrylicAvailable =>
        OperatingSystem.IsWindowsVersionAtLeast(10, 0, 26100) || OperatingSystem.IsLinux() ||
        OperatingSystem.IsMacOS();

    internal static bool MacOSLiquidGlassRequested => OperatingSystem.IsMacOS() &&
        string.Equals(Environment.GetEnvironmentVariable("DOROTI_MACOS_BACKDROP"), "liquidGlass", StringComparison.OrdinalIgnoreCase);

    internal static string WindowEffectLabel => OperatingSystem.IsMacOS()
        ? MacOSLiquidGlassRequested && OperatingSystem.IsMacOSVersionAtLeast(26) ? "Liquid Glass" : "Window blur"
        : "Acrylic window";

    internal static bool AcrylicEnabled => SampleEnabled
        ? SampleAcrylicAvailable
        : OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS() || ExperimentalAcrylicEnabled;

    internal static DorotiViewConfiguration ViewConfiguration { get; } =
        new("Doroti Material Testbed", SampleEnabled ? new Size(1280, 900) : new Size(720, 640),
            // Prepare the native backdrop at startup. The sample toggles its
            // Scaffold between opaque and translucent without recreating the window.
            // A transparent renderer base avoids applying the surface tint twice.
            AcrylicEnabled
                ? new UiColor(0x00000000L) : new UiColor(SampleEnabled ? 0xfffffbfeL : 0xccfffbfeL),
            AcrylicEnabled
                ? new UiColor(0x00000000L) : new UiColor(SampleEnabled ? 0xff141218L : 0xcc141218L),
            MacOSLiquidGlassRequested ? new WindowBackdropOptions(WindowBackdropMode.liquidGlass) :
            SampleEnabled && !SampleAcrylicAvailable ? new WindowBackdropOptions(WindowBackdropMode.solid) : ExperimentalAcrylicEnabled
                ? new WindowBackdropOptions(
                    WindowBackdropMode.experimentalAcrylic,
                    WindowBackdropFallback.transparent,
                    WindowAcrylicKind.@default,
                    WindowBackdropTheme.system)
                : new WindowBackdropOptions(
                    WindowBackdropMode.acrylic,
                    WindowBackdropFallback.transparent),
            terminateAfterLastWindowClosed: true);
}
