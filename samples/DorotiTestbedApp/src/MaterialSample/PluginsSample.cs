using Doroti.Framework.Widgets;
using Doroti.Plugins;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

public sealed class PluginsSample : StatefulWidget
{
    public override IState createState() => new PluginsState();
}

internal sealed class PluginsState : State<PluginsSample>
{
    private readonly CancellationTokenSource _lifetime = new();
    private NativeFeatures? _features;
    private bool _busy;
    private string _status = "Choose files to read their first bytes, or open a URL.";
    private async void Run(bool pick)
    {
        if (_busy) return;
        setState(() => _busy = true);
        try
        {
            _features ??= NativeFeatures.ForView(PlatformDispatcher.instance.implicitView!);
            if (pick)
            {
                await using var selection = await _features.PickFilesAsync(new(AllowMultiple: true), _lifetime.Token);
                var lines = new List<string> { selection.Status.ToString() };
                foreach (var file in selection.Files)
                {
                    var bytes = new byte[16];
                    var count = await file.ReadAsync(0, bytes, _lifetime.Token);
                    lines.Add($"{file.Name}: {file.Length:N0} bytes; {Convert.ToHexString(bytes.AsSpan(0, count))}");
                }
                if (mounted) setState(() => _status = string.Join("\n", lines));
            }
            else
            {
                var result = await _features.LaunchUrlAsync("https://example.com", _lifetime.Token);
                if (mounted) setState(() => _status = $"URL: {result.Status} {result.Message}");
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception error) { if (mounted) setState(() => _status = error.Message); }
        finally { if (mounted) setState(() => _busy = false); }
    }
    public override Widget build(BuildContext context) => new M.Scaffold(body: new Column(children:
    [
        new Text("Native plugins: FilePicker / URL launcher"),
        new M.TextButton(onPressed: _busy ? null : () => Run(true), child: new Text("Choose files")),
        new M.TextButton(onPressed: _busy ? null : () => Run(false), child: new Text("Open example.com")),
        new Text(_status),
    ]));
    public override void dispose() { _lifetime.Cancel(); _lifetime.Dispose(); base.dispose(); }
}
