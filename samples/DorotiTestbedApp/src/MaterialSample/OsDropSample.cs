using System.Text;
using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

public sealed class OsDropSample : StatefulWidget
{
    public override IState createState() => new OsDropState();
}

internal sealed class OsDropState : State<OsDropSample>
{
    private IOsDropRegistration? _registration;
    private readonly CancellationTokenSource _lifetime = new();
    private string _phase = "Drop files, text or a URI anywhere in this view.";
    private string _result = "Copy only. Dropped content is never executed.";
    private int _drops;
    private IOsDragSourceHostCapability? _source;
    private async void Send()
    {
        if (_source is null) return;
        try
        {
            var result = await _source.StartDragAsync(new(Text: "Hello from Doroti", Uris: [new Uri("https://example.com")]), cancellationToken: _lifetime.Token);
            if (mounted) setState(() => _result = result.Canceled ? "Drag canceled" : $"Sent: {result.Action}");
        }
        catch (OperationCanceledException) { }
        catch (Exception error) { if (mounted) setState(() => _result = error.Message); }
    }
    public override void initState()
    {
        base.initState();
        var view = PlatformDispatcher.instance.implicitView!;
        if (view.registeredCapabilityIds.Contains(DorotiCapabilityIds.OsDragSource))
            _source = view.RequireCapability<IOsDragSourceHostCapability>(DorotiCapabilityIds.OsDragSource, DorotiUiInvocation.Managed("OS drag source sample"));
        if (!OsDragDrop.support(view).CanReceive) { _phase = "OS drop reception is unsupported on this host."; return; }
        _registration = OsDragDrop.register(view, new(OsDropAction.Copy,
            [OsDropFormats.Files, OsDropFormats.Text, OsDropFormats.UriList]), Receive);
    }
    private async void Receive(OsDropEvent item)
    {
        if (!mounted) { item.Data?.Dispose(); return; }
        setState(() => _phase = $"{item.Phase} · {item.Action} · ({item.Offer.Position.dx:F1}, {item.Offer.Position.dy:F1}) DIP");
        if (item.Phase == OsDropPhase.Error) { setState(() => _result = $"{item.Failure}: {item.Message}"); return; }
        if (item.Data is not { } data) return;
        using (data)
        using (var cancellation = CancellationTokenSource.CreateLinkedTokenSource(_lifetime.Token, data.Lifetime))
        {
            try
            {
                var lines = new List<string>();
                foreach (var file in data.Files)
                {
                    var bytes = new byte[32];
                    var count = await file.ReadAsync(0, bytes, cancellation.Token);
                    lines.Add($"{file.Name}: {file.Length:N0} bytes · {Convert.ToHexString(bytes.AsSpan(0, count))}");
                }
                if (data.Text is { } text) lines.Add($"Text: {text}");
                lines.AddRange(data.Uris.Select(uri => $"URI: {uri}"));
                if (mounted) setState(() => { _drops++; _result = string.Join("\n", lines); });
            }
            catch (OperationCanceledException) { }
            catch (Exception error) { if (mounted) setState(() => _result = error.Message); }
        }
    }
    public override Widget build(BuildContext context) => new M.Scaffold(body: new SafeArea(child: new Column(children:
    [
        .. _source is not null ? new Widget[] { new GestureDetector(onPanStart: _ => Send(), child: new SizedBox(height: 64,
            child: new Center(child: new Text("Drag this text and link to another app")))) } : [],
        new Text("OS Drag & Drop"), new Text(_phase), new Text($"Completed drops: {_drops}"),
        new Expanded(child: new SingleChildScrollView(child: new Text(_result))),
    ])));
    public override void dispose()
    {
        _lifetime.Cancel(); _registration?.Dispose(); _lifetime.Dispose(); base.dispose();
    }
}
