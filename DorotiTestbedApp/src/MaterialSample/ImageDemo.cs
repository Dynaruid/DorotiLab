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
    private int _revision, _reload;
    private string? _error;
    private M.ColorScheme? _light, _dark;
    private object Provider() => _url ? new NetworkImageIo(Url) : new MemoryImage(LocalBytes.Value);
    private void SelectSource(bool url)
    {
        if (_url == url) return;
        setState(() => { _url = url; _revision++; _light = _dark = null; _loading = false; _error = null; });
    }
    private async void RetryImage()
    {
        var revision = _revision;
        await ((dynamic)Provider()).evict();
        if (mounted && revision == _revision) setState(() => _reload++);
    }
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
    public override Widget build(BuildContext context) => new M.Card(child: new Padding(padding: EdgeInsets.CreateAll(16),
        child: new Column(crossAxisAlignment: CrossAxisAlignment.start, children:
    [
        new Text("Image demo", style: M.Theme.of(context).textTheme.titleLarge), new SizedBox(height: 12),
        new M.SegmentedButton<string>(segments: [new("Local", label: new Text("Local asset")), new("URL", label: new Text("Image URL"))], selected: [_url ? "URL" : "Local"],
            onSelectionChanged: values => SelectSource(values.Contains("URL"))),
        new SizedBox(height: 12), new Text(_url ? "Unsplash · remote image" : "Mae Mu · Unsplash"), new SizedBox(height: 8),
        new ClipRRect(borderRadius: BorderRadius.CreateCircular(12), child: new ColoredBox(color: M.Theme.of(context).colorScheme.surfaceContainerHighest,
            child: new SizedBox(width: double.PositiveInfinity, height: 240, child: new Image(key: new Doroti.Framework.Foundation.ValueKey<int>(_reload),
                image: Provider(), fit: _cover ? BoxFit.cover : BoxFit.contain,
                semanticLabel: _url ? "Photo loaded from the Unsplash image URL" : "Local Unsplash photo by Mae Mu",
                frameBuilder: (_, child, frame, synchronous) => frame is not null || synchronous ? child : new Center(child: new Text("Loading image…")),
                errorBuilder: (_, _, _) => new Center(child: new Column(mainAxisSize: MainAxisSize.min, children:
                    [new Text("Image could not be loaded."), new M.TextButton(child: new Text("Retry image"), onPressed: RetryImage)])))))),
        new SizedBox(height: 12),
        new Wrap(spacing: 8, runSpacing: 8, crossAxisAlignment: WrapCrossAlignment.center, children:
        [
            new M.ChoiceChip(label: new Text("Contain"), selected: !_cover, onSelected: _ => setState(() => _cover = false)),
            new M.ChoiceChip(label: new Text("Cover"), selected: _cover, onSelected: _ => setState(() => _cover = true)),
            M.FilledButton.CreateTonal(child: new Text(_loading ? "Extracting…" : "Extract colors"), onPressed: _loading ? null : Load),
        ]),
        .. _error is null ? Array.Empty<Widget>() : [new SizedBox(height: 12), new Text("Could not extract colors. Check the image and try again.")],
        .. _light is null || _dark is null ? Array.Empty<Widget>() : [new SizedBox(height: 16), Palette(context, "Light palette", _light), new SizedBox(height: 12), Palette(context, "Dark palette", _dark)],
    ])));
    private static Widget Palette(BuildContext context, string title, M.ColorScheme scheme)
    {
        (string Label, Color Color, Color On)[] colors = [("Primary", scheme.primary, scheme.onPrimary), ("Secondary", scheme.secondary, scheme.onSecondary), ("Tertiary", scheme.tertiary, scheme.onTertiary)];
        return new Column(crossAxisAlignment: CrossAxisAlignment.start, children: [new Text(title, style: M.Theme.of(context).textTheme.titleSmall), new SizedBox(height: 8),
            new Wrap(spacing: 8, runSpacing: 8, children: colors.Select(role => (Widget)new Container(width: 108,
                padding: EdgeInsets.CreateAll(10), decoration: new BoxDecoration(color: role.Color, borderRadius: BorderRadius.CreateCircular(8)),
                child: new Text($"{role.Label}\n#{role.Color.red:X2}{role.Color.green:X2}{role.Color.blue:X2}", style: new TextStyle(color: role.On)))).ToList())]);
    }
}
