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
        new("Doroti C# App", new Size(720, 640), new UiColor(0xfffffbfeL));

    private static Widget CreateRootWidget()
    {
        var theme = Material.ThemeData.Create(
            useMaterial3: true,
            colorSchemeSeed: new UiColor(0xff6750a4L),
            scaffoldBackgroundColor: new UiColor(0xfffffbfeL));
        return new Material.MaterialApp(
            title: "Doroti C# App",
            color: new UiColor(0xff6750a4L),
            locale: new Locale("en", "US"),
            debugShowCheckedModeBanner: false,
            home: new Material.Theme(data: theme, child: new CounterPage()));
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

    public override Widget build(BuildContext context) => new Material.Scaffold(
        appBar: new Material.AppBar(
            title: new Text("Doroti C# single-project app"),
            backgroundColor: new UiColor(0xffeaddffL),
            foregroundColor: new UiColor(0xff21005dL)),
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
