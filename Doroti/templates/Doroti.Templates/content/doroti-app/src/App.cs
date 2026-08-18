using Doroti.Framework;
using Doroti.Hosting;
using Doroti.Ui;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Widgets;
using Locale = Doroti.Ui.Locale;
using Material = Doroti.Framework.Material;
using Size = Doroti.Ui.Size;
using UiColor = Doroti.Ui.Color;

namespace DorotiTemplateApp;

public static class App
{
    public static Func<IDorotiViewEntrypoint> Definition =>
        () => new DorotiWidgetEntrypoint(CreateRootWidget);

    public static DorotiViewConfiguration ViewConfiguration { get; } =
        new("Doroti C# App", new Size(720, 640),
            new UiColor(0xfffffbfeL), new UiColor(0xff141218L));

    private static Widget CreateRootWidget()
    {
        return new Material.MaterialApp(
            title: "Doroti C# App",
            color: new UiColor(0xff6750a4L),
            theme: AppTheme.Light,
            darkTheme: AppTheme.Dark,
            themeMode: Material.ThemeMode.system,
            locale: new Locale("en", "US"),
            debugShowCheckedModeBanner: false,
            home: new CounterPage());
    }
}

public static class AppTheme
{
    private static readonly UiColor Seed = new(0xff6750a4L);

    public static Material.ThemeData Light { get; } = Create(Brightness.light);
    public static Material.ThemeData Dark { get; } = Create(Brightness.dark);

    private static Material.ThemeData Create(Brightness brightness)
    {
        var isDark = brightness == Brightness.dark;
        var palette = Material.ColorScheme.CreateFromSeed(
            seedColor: Seed,
            brightness: brightness,
            surface: new UiColor(isDark ? 0xff141218L : 0xfffffbfeL),
            surfaceContainer: new UiColor(isDark ? 0xff211f26L : 0xfff3edf7L),
            surfaceContainerHigh: new UiColor(isDark ? 0xff2b2930L : 0xffece6f0L),
            outline: new UiColor(isDark ? 0xff938f99L : 0xff79747eL));
        return Material.ThemeData.Create(
            useMaterial3: true,
            colorScheme: palette,
            scaffoldBackgroundColor: palette.surface);
    }
}

public sealed class CounterPage : StatefulWidget
{
    public override IState createState() => new CounterPageState();
}

public sealed class CounterPageState : State<CounterPage>
{
    private const string CounterShaderSource = """
        uniform float2 uSize;
        uniform float uPhase;

        half4 main(float2 position) {
            float2 uv = position / max(uSize, float2(1.0));
            return half4(uv.x, 0.35 + 0.35 * sin(uPhase + uv.x * 6.2831853), uv.y, 1.0);
        }
        """;

    private int _count;
    private readonly FragmentShader _counterShader =
        FragmentProgram.fromSource(CounterShaderSource, "doroti-template-counter").fragmentShader();

    public override Widget build(BuildContext context)
    {
        var palette = Material.Theme.of(context).colorScheme;
        return new Material.Scaffold(
            appBar: new Material.AppBar(
                title: new Text("Doroti C# single-project app"),
                backgroundColor: palette.primaryContainer,
                foregroundColor: palette.onPrimaryContainer),
            body: new Center(
                child: new Column(
                    mainAxisAlignment: Doroti.Framework.Rendering.MainAxisAlignment.center,
                    spacing: 16,
                    children:
                    [
                        new Text("Package asset: assets/doroti-mark.txt"),
                        new Text("Localized resource: locales/en-US.json"),
                        new ShaderMask(
                            shaderCallback: bounds =>
                            {
                                _counterShader.setFloat(0, bounds.width);
                                _counterShader.setFloat(1, bounds.height);
                                _counterShader.setFloat(2, _count * 0.4);
                                return _counterShader;
                            },
                            child: new Text($"Custom SkSL count: {_count}")),
                        new Material.ElevatedButton(
                            onPressed: () => setState(() => _count++),
                            child: new Text("Increment")),
                    ])));
    }
}
