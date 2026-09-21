using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

internal sealed class TextureSample : StatefulWidget
{
    public override IState createState() => new TextureSampleState();
}

internal sealed class TextureSampleState : State<TextureSample>
{
    private TextureEntry? _entry;
    private DorotiView? _owner;
    private Doroti.Runtime.Timer? _timer;
    private readonly byte[] _pixels = new byte[160 * 90 * 4];
    private int _frame;
    private bool _freeze;

    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        var owner = View.of(context);
        if (ReferenceEquals(owner, _owner))
            return;
        _timer?.cancel();
        _entry?.Dispose();
        _entry = null;
        _owner = owner;
        if (!owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.GraphicsTexture))
            return;
        _entry = TextureRegistry.ForView(owner).CreateTexture();
        PublishFrame();
        _timer = Doroti.Runtime.Timer.periodic(
            Duration.Create(milliseconds: 33),
            _ => PublishFrame()
        );
    }

    private void PublishFrame()
    {
        for (var y = 0; y < 90; y++)
        for (var x = 0; x < 160; x++)
        {
            var i = (y * 160 + x) * 4;
            _pixels[i] = (byte)((x + _frame * 3) % 256);
            _pixels[i + 1] = (byte)(y * 255 / 90);
            _pixels[i + 2] = (byte)((x + y + _frame) % 256);
            _pixels[i + 3] = 255;
        }
        _frame++;
        _entry?.PushFrame(_pixels, 160, 90);
    }

    public override Widget build(BuildContext context) =>
        new SingleChildScrollView(
            child: new Padding(
                padding: EdgeInsets.CreateAll(24),
                child: new Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children:
                    [
                        new Text("Texture", style: M.Theme.of(context).textTheme.headlineMedium),
                        new SizedBox(height: 12),
                        new Text(
                            "Live frames rendered inside the widget tree. Freeze holds the displayed frame while the producer keeps running."
                        ),
                        new SizedBox(height: 24),
                        _entry is { } entry
                            ? new AspectRatio(
                                aspectRatio: 16.0 / 9,
                                child: new ClipRRect(
                                    borderRadius: BorderRadius.CreateCircular(24),
                                    child: new Texture(textureId: entry.Id, freeze: _freeze)
                                )
                            )
                            : new Text("Textures are unavailable on this host."),
                        new SizedBox(height: 16),
                        new M.FilledButton(
                            onPressed: _entry is null
                                ? null
                                : () => setState(() => _freeze = !_freeze),
                            child: new Text(_freeze ? "Resume" : "Freeze")
                        ),
                    ]
                )
            )
        );

    public override void dispose()
    {
        _timer?.cancel();
        _entry?.Dispose();
        base.dispose();
    }
}
