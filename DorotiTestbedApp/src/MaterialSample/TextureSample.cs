using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

internal sealed class TextureSample : StatefulWidget
{
    public override IState createState() => new TextureSampleState();
}

public static class TextureSampleProbe
{
    public static SynchronizationContext? OwnerContext { get; internal set; }
    public static DorotiView? Owner { get; internal set; }
    public static System.Action<long, bool, int>? SetTexture { get; internal set; }
    public static int Builds { get; internal set; }
    public static Func<string, Task<string>>? SelectSource { get; set; }
}

internal sealed partial class TextureSampleState : State<TextureSample>
{
    private TextureEntry? _entry;
    private DorotiView? _owner;
    private Doroti.Runtime.Timer? _timer;
    private readonly byte[] _pixels = new byte[160 * 90 * 4];
    private int _frame;
    private bool _freeze;
    private long? _externalId;
    private string? _error;
    private string _source = "canvas";
    private int _effect;
    private bool _busy;
    private WebViewController? _web;

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
        TextureSampleProbe.Owner = owner;
        TextureSampleProbe.OwnerContext = SynchronizationContext.Current;
        TextureSampleProbe.SetTexture = (id, freeze, effect) =>
            setState(() =>
            {
                _timer?.cancel();
                _entry?.Dispose();
                _entry = null;
                _externalId = id > 0 ? id : null;
                _freeze = freeze;
                _effect = effect;
                if (effect == 3 && _web is null)
                    _web = new WebViewController(
                        owner,
                        new WebViewOptions(
                            Html: "<body style='margin:0;background:#123456'><input value='live iframe' style='width:90px'>",
                            Profile: WebViewProfile.BrowserDefault
                        )
                    );
                if (effect != 3 && _web is { } web)
                {
                    _web = null;
                    _ = web.DisposeAsync();
                }
            });
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

    public override Widget build(BuildContext context)
    {
        TextureSampleProbe.Builds++;
        var id = _externalId ?? _entry?.Id;
        Widget preview = id is { } textureId
            ? new ClipRRect(
                borderRadius: BorderRadius.CreateCircular(24),
                child: new Opacity(
                    opacity: _effect == 1 ? 0.5 : 1,
                    child: Transform.CreateRotate(
                        angle: _effect == 2 ? 0.15 : 0,
                        child: new RepaintBoundary(
                            child: new Texture(textureId: textureId, freeze: _freeze)
                        )
                    )
                )
            )
            : new Text("Textures are unavailable on this host.");
        if (_web is { } webView && id is { } overlayId)
            preview = new Stack(
                children:
                [
                    preview,
                    new Positioned(
                        left: 90,
                        top: 30,
                        width: 140,
                        height: 110,
                        child: new WebViewWidget(webView)
                    ),
                    new Positioned(
                        left: 160,
                        top: 80,
                        width: 80,
                        height: 45,
                        child: new Texture(textureId: overlayId, freeze: _freeze)
                    ),
                ]
            );
        return new SingleChildScrollView(
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
                        Environment.GetEnvironmentVariable("DOROTI_TESTBED_MODE") == "texture-web"
                            ? new SizedBox(width: 320, height: 180, child: preview)
                            : new AspectRatio(aspectRatio: 16.0 / 9, child: preview),
                        new SizedBox(height: 16),
                        new M.FilledButton(
                            onPressed: id is null ? null : () => setState(() => _freeze = !_freeze),
                            child: new Text(_freeze ? "Resume" : "Freeze")
                        ),
                        .. (
                            OperatingSystem.IsBrowser()
                                ? new Widget[]
                                {
                                    new Wrap(
                                        spacing: 8,
                                        children:
                                        [
                                            SourceButton("Canvas", "canvas"),
                                            SourceButton("Video", "video"),
                                            SourceButton("Camera", "camera"),
                                            SourceButton("ImageBitmap", "bitmap"),
                                            SourceButton("VideoFrame", "frame"),
                                            SourceButton("WebCodecs", "codec"),
                                            SourceButton("Recreate", "recreate"),
                                            SourceButton("Stop", "stop"),
                                            SourceButton("Resize", "resize"),
                                            SourceButton("Update", "update"),
                                            SourceButton("Pause / Play", "pause"),
                                            SourceButton("Seek", "seek"),
                                        ]
                                    ),
                                }
                                : Array.Empty<Widget>()
                        ),
                        new Text(_error ?? ""),
                    ]
                )
            )
        );
    }

    private Widget SourceButton(string label, string operation) =>
        new M.FilledButton(
            onPressed: _busy
                ? null
                : () =>
                {
                    _ = SelectSource(operation);
                },
            child: new Text(label)
        );

    private async Task SelectSource(string operation)
    {
        setState(() => _busy = true);
        try
        {
            _timer?.cancel();
            _entry?.Dispose();
            _entry = null;
            if (operation is "canvas" or "video" or "camera" or "bitmap" or "frame" or "codec")
                _source = operation;
            var id = await (
                TextureSampleProbe.SelectSource
                ?? throw new InvalidOperationException("Browser texture sources are not ready.")
            )(operation == "recreate" ? _source : operation);
            if (mounted)
                setState(() =>
                {
                    _externalId = long.TryParse(id, out var parsed) && parsed > 0 ? parsed : null;
                    _error = null;
                });
        }
        catch (Exception error)
        {
            if (mounted)
                setState(() => _error = error.Message);
        }
        finally
        {
            if (mounted)
                setState(() => _busy = false);
        }
    }

    public override void dispose()
    {
        _timer?.cancel();
        _entry?.Dispose();
        if (_web is { } web)
            _ = web.DisposeAsync();
        if (TextureSampleProbe.SelectSource is { } stop)
            _ = stop("stop");
        TextureSampleProbe.SetTexture = null;
        TextureSampleProbe.Owner = null;
        TextureSampleProbe.OwnerContext = null;
        base.dispose();
    }
}
