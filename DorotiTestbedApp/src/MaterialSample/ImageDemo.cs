// Doroti adaptation of the local reference Image demo extension.
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;
using M = Doroti.Framework.Material;
using Image = Doroti.Framework.Widgets.Image;
using TextStyle = Doroti.Framework.Painting.TextStyle;

namespace MaterialSample;
internal sealed class SampleImageDemo : StatefulWidget
{
    public override IState createState() => new SampleImageDemoState();
}
internal sealed class SampleImageDemoState : State<SampleImageDemo>
{
    private const string Url = "https://plus.unsplash.com/premium_photo-1734210255965-0a721514a34e";
    private static readonly Lazy<Uint8List> LocalBytes = new(() =>
    {
        using var stream = typeof(SampleImageDemo).Assembly.GetManifestResourceStream("MaterialSample.image.webp")
            ?? throw new InvalidOperationException("Material sample image resource is missing.");
        using var bytes = new MemoryStream(); stream.CopyTo(bytes); return new Uint8List(bytes.ToArray());
    });
    private bool _url, _cover, _loading;
    private int _revision;
    private string? _error;
    private M.ColorScheme? _light, _dark;
    private object Provider() => _url ? new NetworkImageIo(Url) : new MemoryImage(LocalBytes.Value);
    public override void initState() { base.initState(); }
    public override void didChangeDependencies() { base.didChangeDependencies(); if (_revision == 0) Load(); }
    private async void Load()
    {
        var revision = ++_revision;
        setState(() => { _loading = true; _error = null; _light = null; _dark = null; });
        try
        {
            var provider = Provider();
            var light = await M.ColorScheme.fromImageProvider(provider);
            var dark = await M.ColorScheme.fromImageProvider(provider, brightness: Brightness.dark);
            if (mounted && revision == _revision) setState(() => { _light = light; _dark = dark; _loading = false; });
        }
        catch (Exception error) { if (mounted && revision == _revision) setState(() => { _error = error.Message; _loading = false; }); }
    }
    public override void dispose() { _revision++; base.dispose(); }
    public override Widget build(BuildContext context) => new Column(crossAxisAlignment: CrossAxisAlignment.stretch, spacing: 10, children:
    [
        new Wrap(spacing: 10, children:
        [
            new M.SegmentedButton<string>(segments: [new("Local", label: new Text("Local image")), new("URL", label: new Text("URL image"))], selected: [_url ? "URL" : "Local"],
                onSelectionChanged: values => { setState(() => _url = values.Contains("URL")); Load(); }),
            new M.SegmentedButton<string>(segments: [new("Contain", label: new Text("Contain")), new("Cover", label: new Text("Cover"))], selected: [_cover ? "Cover" : "Contain"],
                onSelectionChanged: values => setState(() => _cover = values.Contains("Cover"))),
        ]),
        new SizedBox(height: 240, child: new Image(image: Provider(), fit: _cover ? BoxFit.cover : BoxFit.contain,
            errorBuilder: (_, error, _) => new Center(child: new Text($"Image unavailable: {error}")))),
        _loading ? new M.LinearProgressIndicator() : SizedBox.CreateShrink(),
        _error is null ? SizedBox.CreateShrink() : new Text($"Image failed: {_error}"),
        new M.TextButton(child: new Text("Retry image demo"), onPressed: Load),
        Palette("Light palette", _light), Palette("Dark palette", _dark),
    ]);
    private static Widget Palette(string title, M.ColorScheme? scheme)
    {
        if (scheme is null) return SizedBox.CreateShrink();
        (string Label, Color Color, Color On)[] colors = [("Primary", scheme.primary, scheme.onPrimary), ("Secondary", scheme.secondary, scheme.onSecondary), ("Tertiary", scheme.tertiary, scheme.onTertiary)];
        return new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children: [new Text(title), .. colors.Select(role => (Widget)new Container(
            padding: EdgeInsets.CreateAll(12), color: role.Color, child: new Text($"{role.Label} RGB({role.Color.red}, {role.Color.green}, {role.Color.blue})", style: new TextStyle(color: role.On))))]);
    }
}
