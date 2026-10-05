using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using System.Text.Json;
using Doroti.Hosting;
using Doroti.Ui;

namespace Doroti.Host.Web;

[SupportedOSPlatform("browser")]
internal static partial class BrowserInterop
{
    [JSImport("openBrowserDrop", Module)] internal static partial void OpenBrowserDrop(int id, string canvasId, string options);
    [JSImport("closeBrowserDrop", Module)] internal static partial void CloseBrowserDrop(int id);
    [JSExport] internal static void DispatchBrowserDrop(int id, string json) => BrowserOsDrop.Dispatch(id, json);
}

[SupportedOSPlatform("browser")]
internal sealed class BrowserOsDrop : IOsDragDropHostCapability, IDisposable
{
    private static readonly Dictionary<int, BrowserOsDrop> Owners = [];
    private readonly int _id;
    private readonly string _canvas;
    private readonly OsDropReceiver _receiver;
    public BrowserOsDrop(int id, string canvas, Action<Action> dispatch)
    {
        _id = id; _canvas = canvas;
        _receiver = new(new(true, false, OsDropAction.Copy, [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]), dispatch);
        Owners.Add(id, this);
    }
    public OsDropSupport Support => _receiver.Support;
    public IOsDropRegistration Register(OsDropOptions options, Action<OsDropEvent> onEvent)
    {
        var inner = _receiver.Register(options, onEvent);
        try { Update(options); return new Registration(this, inner); }
        catch { inner.Dispose(); throw; }
    }
    private void Update(OsDropOptions options)
    {
        // Use Utf8JsonWriter so Release trimming never relies on reflection metadata.
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            writer.WriteStartObject(); writer.WriteStartArray("formats");
            if ((options.Actions & OsDropAction.Copy) != 0) foreach (var format in options.Formats) writer.WriteStringValue(format);
            writer.WriteEndArray();
            if (options.Bounds is { } b) { writer.WriteStartArray("bounds"); foreach (var value in new[] { b.left, b.top, b.right, b.bottom }) writer.WriteNumberValue(value); writer.WriteEndArray(); }
            writer.WriteEndObject();
        }
        BrowserInterop.OpenBrowserDrop(_id, _canvas, System.Text.Encoding.UTF8.GetString(stream.ToArray()));
    }
    private sealed class Registration(BrowserOsDrop owner, IOsDropRegistration inner) : IOsDropRegistration
    {
        private bool _disposed;
        public void Update(OsDropOptions options) { ObjectDisposedException.ThrowIf(_disposed, this); inner.Update(options); owner.Update(options); }
        public void Dispose() { if (_disposed) return; _disposed = true; BrowserInterop.CloseBrowserDrop(owner._id); inner.Dispose(); }
    }
    public static void Dispatch(int id, string json)
    {
        if (!Owners.TryGetValue(id, out var owner)) return;
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var offer = new OsDropOffer(new(root.GetProperty("x").GetDouble(), root.GetProperty("y").GetDouble()),
            root.GetProperty("formats").EnumerateArray().Select(v => v.GetString()!).ToArray(),
            root.GetProperty("copy").GetBoolean() ? OsDropAction.Copy : OsDropAction.None);
        var phase = Enum.Parse<OsDropPhase>(root.GetProperty("phase").GetString()!);
        if (phase != OsDropPhase.Drop) { owner._receiver.Hover(phase, offer); return; }
        var files = root.GetProperty("files").EnumerateArray().Select(v => (IPickedFile)new BrowserPickedFile(id,
            v.GetProperty("token").GetString()!, v.GetProperty("name").GetString()!, v.GetProperty("length").GetInt64())).ToArray();
        var action = owner._receiver.Drop(offer, formats =>
        {
            var uris = (root.GetProperty("uris").GetString() ?? "").Split('\n').Select(s => s.Trim())
                .Where(s => !s.StartsWith('#') && Uri.TryCreate(s, UriKind.Absolute, out _)).Select(s => new Uri(s)).ToArray();
            var selected = formats.Contains(OsDropFormats.Files) ? files : [];
            foreach (var file in files.Except(selected)) file.Dispose();
            return new(selected, formats.Contains(OsDropFormats.Text) ? root.GetProperty("text").GetString() : null,
                formats.Contains(OsDropFormats.UriList) ? uris : []);
        });
        if (action == OsDropAction.None) foreach (var file in files) file.Dispose();
    }
    public void Dispose() { if (!Owners.Remove(_id)) return; BrowserInterop.CloseBrowserDrop(_id); _receiver.Dispose(); }
}
