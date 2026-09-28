using Doroti.Framework.Foundation;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Runtime;
using Doroti.Ui;
using Size = Doroti.Ui.Size;
using UiColor = Doroti.Ui.Color;

internal sealed class MaterialDemoEntrypoint : IDorotiViewEntrypoint
{
    private WidgetsFlutterBinding? _binding;
    private Doroti.Framework.DorotiWidgetEntrypoint? _widgetEntrypoint;
    private DorotiView? _view;

    private Widget RootApp => _rootApp ??= CreateRootApp();

    private Widget? _rootApp;

    public void Bootstrap(PlatformDispatcher dispatcher)
    {
        FlutterError.onError = details =>
        {
            Console.Error.WriteLine(details.exceptionThrown);
        };
        _widgetEntrypoint = new Doroti.Framework.DorotiWidgetEntrypoint(
            () => RootApp,
            PrepareSampleResourcesAsync
        );
        _widgetEntrypoint.Bootstrap(dispatcher);
        _binding = (WidgetsFlutterBinding)WidgetsFlutterBinding.ensureInitialized();
    }

    public void AttachView(DorotiView view)
    {
        if (_binding is null)
        {
            throw new InvalidOperationException(
                "The Material framework binding was not bootstrapped."
            );
        }
        if (_view is not null)
        {
            throw new InvalidOperationException("DorotiTestbedApp owns exactly one Doroti view.");
        }

        _view = view;
        _widgetEntrypoint!.AttachView(view);
    }

    private static async Task PrepareSampleResourcesAsync()
    {
        using var stream =
            typeof(MaterialDemoEntrypoint).Assembly.GetManifestResourceStream(
                "MaterialSample.icons.otf"
            ) ?? throw new InvalidOperationException("MaterialIcons resource is missing.");
        using var bytes = new MemoryStream();
        stream.CopyTo(bytes);
        await DorotiUiLibrary.loadFontFromList(
            new Uint8List(bytes.ToArray()),
            fontFamily: "MaterialIcons"
        );
        // Flutter Web registers its regular Roboto fallback; native hosts also use weight faces.
        foreach (
            var weight in Doroti.Framework.Foundation.ConstantsLibrary.kIsWeb
                ? new[] { "regular" }
                : new[] { "medium", "bold", "regular" }
        )
        {
            using var fontStream =
                typeof(MaterialDemoEntrypoint).Assembly.GetManifestResourceStream(
                    $"MaterialSample.Roboto-{weight}.ttf"
                ) ?? throw new InvalidOperationException($"Roboto {weight} resource is missing.");
            using var fontBytes = new MemoryStream();
            fontStream.CopyTo(fontBytes);
            await DorotiUiLibrary.loadFontFromList(
                new Uint8List(fontBytes.ToArray()),
                fontFamily: "Roboto"
            );
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

    public void Shutdown()
    {
        _widgetEntrypoint?.Shutdown();
        _widgetEntrypoint = null;
        _binding = null;
        _view = null;
        FlutterError.onError = null;
    }

    private static Widget CreateRootApp() =>
        Environment.GetEnvironmentVariable("DOROTI_SAMPLE") switch
        {
            "input" => new Doroti.Framework.Material.MaterialApp(home: new MaterialSample.InputLifetimeSample()),
            "reload" => new Doroti.Framework.Material.MaterialApp(home: new MaterialSample.HotReloadSample()),
            _ => new MaterialSample.SampleApp(App.SampleAcrylicAvailable),
        };
}

internal static class App
{
    internal static Func<IDorotiViewEntrypoint> Definition => () => new MaterialDemoEntrypoint();

    internal static bool ExperimentalAcrylicEnabled =>
        string.Equals(
            Environment.GetEnvironmentVariable("DOROTI_DEMO_EXPERIMENTAL_ACRYLIC"),
            "1",
            StringComparison.Ordinal
        );

    internal static bool SampleAcrylicAvailable =>
        OperatingSystem.IsWindowsVersionAtLeast(10, 0, 26100)
        || OperatingSystem.IsLinux()
        || OperatingSystem.IsMacOS();

    internal static bool MacOSLiquidGlassRequested =>
        OperatingSystem.IsMacOS()
        && string.Equals(
            Environment.GetEnvironmentVariable("DOROTI_MACOS_BACKDROP"),
            "liquidGlass",
            StringComparison.OrdinalIgnoreCase
        );

    internal static WindowTitlebarStyle TitlebarStyle =>
        string.Equals(
            Environment.GetEnvironmentVariable("DOROTI_TITLEBAR")
                ?? Environment.GetEnvironmentVariable("DOROTI_MACOS_TITLEBAR"),
            "solid",
            StringComparison.OrdinalIgnoreCase
        )
            ? WindowTitlebarStyle.solid
            : WindowTitlebarStyle.unified;

    internal static string WindowEffectLabel =>
        OperatingSystem.IsMacOS()
            ? MacOSLiquidGlassRequested && OperatingSystem.IsMacOSVersionAtLeast(26)
                ? "Liquid Glass"
                : "Window blur"
            : "Acrylic window";

    internal static bool AcrylicEnabled => SampleAcrylicAvailable;

    internal static DorotiViewConfiguration ViewConfiguration { get; } =
        new(
            "Doroti Material Testbed",
            new Size(1280, 900),
            // Prepare the native backdrop at startup. The sample toggles its
            // Scaffold between opaque and translucent without recreating the window.
            // A transparent renderer base avoids applying the surface tint twice.
            AcrylicEnabled ? new UiColor(0x00000000L) : new UiColor(0xfffffbfeL),
            AcrylicEnabled ? new UiColor(0x00000000L) : new UiColor(0xff141218L),
            terminateAfterLastWindowClosed: true,
            appearance: new WindowAppearanceOptions(
                backdrop: !SampleAcrylicAvailable
                    ? new(WindowBackdropMode.solid)
                    : new(
                        ExperimentalAcrylicEnabled
                            ? WindowBackdropMode.experimentalAcrylic
                            : WindowBackdropMode.acrylic,
                        acrylicKind: OperatingSystem.IsWindows()
                            ? WindowAcrylicKind.thin
                            : WindowAcrylicKind.@default
                    ),
                titlebarStyle: TitlebarStyle,
                macOSBackdrop: new(
                    MacOSLiquidGlassRequested
                        ? WindowBackdropMode.liquidGlass
                        : WindowBackdropMode.acrylic
                )
            )
        );
}
