using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

public static class NativeTextureFixtureProbe
{
    public static Func<SurfaceTextureEntry, IDisposable>? StartProducer { get; set; }
    public static Func<NativeTextureEntry, IDisposable>? StartNativeProducer { get; set; }
    public static int Width { get; set; } = 320;
    public static int Height { get; set; } = 180;
}

internal sealed class NativeTextureFixture : StatefulWidget
{
    public override IState createState() => new NativeTextureState();
}

internal sealed class NativeTextureState : State<NativeTextureFixture>
{
    private SurfaceTextureEntry? _texture;
    private NativeTextureEntry? _nativeTexture;
    private IDisposable? _producer;
    private bool _started,
        _freeze;
    private string? _error;
    private int _generation;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (!_started)
        {
            _started = true;
            _ = CreateAsync();
        }
    }

    private async Task CreateAsync()
    {
        var generation = ++_generation;
        _producer?.Dispose();
        _producer = null;
        _texture?.Dispose();
        _texture = null;
        _nativeTexture?.Dispose();
        _nativeTexture = null;
        try
        {
            if (NativeTextureFixtureProbe.StartNativeProducer is { } nativeProducer)
            {
                _nativeTexture = TextureRegistry.ForView(View.of(context)).CreateNativeTexture();
                _producer = nativeProducer(_nativeTexture);
                setState(() => _error = null);
                return;
            }
            var texture = await TextureRegistry
                .ForView(View.of(context))
                .CreateSurfaceTextureAsync(
                    NativeTextureFixtureProbe.Width,
                    NativeTextureFixtureProbe.Height
                );
            if (!mounted || generation != _generation)
            {
                texture.Dispose();
                return;
            }
            _texture = texture;
            _producer = (
                NativeTextureFixtureProbe.StartProducer
                ?? throw new PlatformNotSupportedException(
                    "Launch the Android native texture fixture to attach a producer."
                )
            )(texture);
            setState(() => _error = null);
        }
        catch (Exception e)
        {
            if (mounted)
                setState(() => _error = e.Message);
        }
    }

    public override Widget build(BuildContext context) =>
        new M.Scaffold(
            appBar: new M.AppBar(title: new Text("Native GPU Texture")),
            body: new Padding(
                padding: EdgeInsets.CreateAll(20),
                child: new Column(
                    children:
                    [
                        new Text("Native camera / video frames"),
                        new SizedBox(height: 20),
                        new SizedBox(
                            width: 320,
                            height: 320.0
                                * NativeTextureFixtureProbe.Height
                                / NativeTextureFixtureProbe.Width,
                            child: new ClipRRect(
                                borderRadius: BorderRadius.CreateCircular(20),
                                child: (_nativeTexture?.Id ?? _texture?.Id) is { } textureId
                                    ? new Texture(textureId: textureId, freeze: _freeze)
                                    : new Text(_error ?? "Preparing producer…")
                            )
                        ),
                        new SizedBox(height: 20),
                        new M.FilledButton(
                            onPressed: () => setState(() => _freeze = !_freeze),
                            child: new Text(_freeze ? "Resume" : "Freeze")
                        ),
                        new M.FilledButton(
                            onPressed: () =>
                            {
                                _ = CreateAsync();
                            },
                            child: new Text("Recreate")
                        ),
                        new Text(_error ?? "Native producer owns frame updates."),
                    ]
                )
            )
        );

    public override void dispose()
    {
        _generation++;
        _producer?.Dispose();
        _texture?.Dispose();
        _nativeTexture?.Dispose();
        base.dispose();
    }
}
