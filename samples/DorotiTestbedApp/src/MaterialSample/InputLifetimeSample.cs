using System.Text;
using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

namespace MaterialSample;

/// <summary>Shared IME, focus and native-view lifetime scene. Launch with DOROTI_SAMPLE=input.</summary>
internal sealed class InputLifetimeSample : StatefulWidget
{
    public override IState createState() => new InputLifetimeState();
}

internal sealed class InputLifetimeState : State<InputLifetimeSample>
{
    private readonly TextEditingController _first = new();
    private readonly TextEditingController _last = new();
    private int _generation;
    private bool _visible = true;
    private bool _showWeb;
    private bool _probeStarted;
    private int _created;
    private string _status = "Native views preparing";
    private readonly PlatformViewDescriptor _editor = new("doroti/native-editor", Encoding.UTF8.GetBytes("{\"text\":\"Native editor\"}"));
    private readonly PlatformViewDescriptor _web = new("doroti/webview", Encoding.UTF8.GetBytes("{\"html\":\"<meta charset='utf-8'><label>Web input <input aria-label='Web input'></label>\"}"));

    public override void initState()
    {
        base.initState();
        _first.addListener(Changed);
    }
    private void Changed() { if (mounted) setState(() => { }); }
    private void Report(string message)
    {
        if (mounted) setState(() => _status = message);
    }
    private void Created(PlatformViewHandle handle)
    {
        _created++;
        Report(_showWeb ? "WebView ready" : "Native editor ready");
    }
    public override void didChangeDependencies()
    {
        base.didChangeDependencies();
        if (!_probeStarted && Environment.GetEnvironmentVariable("DOROTI_INPUT_PROBE") is { Length: > 0 } path)
        {
            _probeStarted = true;
            _ = ProbeAsync(View.of(context), path);
        }
    }
    private async Task ProbeAsync(DorotiView owner, string path)
    {
        try
        {
            async Task WaitForCreated(int expected)
            {
                var deadline = DateTime.UtcNow.AddSeconds(15);
                while (Volatile.Read(ref _created) < expected)
                {
                    if (DateTime.UtcNow >= deadline) throw new TimeoutException("Native view creation did not complete.");
                    await Task.Delay(50);
                }
            }
            await WaitForCreated(1);
            owner.DispatchPlatformEvent(() => setState(() => _generation++));
            await WaitForCreated(2);
            // The host currently uses distinct composition topologies for HWND and WebView.
            owner.DispatchPlatformEvent(() => setState(() => _visible = false));
            await Task.Delay(250);
            owner.DispatchPlatformEvent(() => setState(() => { _showWeb = true; _visible = true; _generation++; }));
            await WaitForCreated(3);
            owner.DispatchPlatformEvent(() => setState(() => _generation++));
            await WaitForCreated(4);
            owner.DispatchPlatformEvent(() => setState(() => _visible = false));
            await Task.Delay(250);
            File.WriteAllText(path, "{\"created\":4,\"editorRecreated\":true,\"webViewRecreated\":true,\"physicalIme\":\"notVerified\"}");
        }
        catch (Exception error) { File.WriteAllText(path + ".error", error.ToString()); }
    }
    public override Widget build(BuildContext context)
    {
        var owner = View.of(context);
        var value = _first.value;
        return new M.Scaffold(
            appBar: new M.AppBar(title: new Text("Input and native view lifetime")),
            body: new SingleChildScrollView(child: new Padding(padding: Doroti.Framework.Painting.EdgeInsets.CreateAll(16), child:
                new Column(crossAxisAlignment: CrossAxisAlignment.stretch, children:
                [
                    new Text("Compose Korean, move with Tab / Shift+Tab, then recreate native views."),
                    new M.TextField(controller: _first, maxLines: 3,
                        decoration: new M.InputDecoration(labelText: "Framework input before native views")),
                    new Text($"Selection {value.selection.start}:{value.selection.end} · composing {value.composing.start}:{value.composing.end}"),
                    new Wrap(children:
                    [
                        new M.TextButton(onPressed: () => setState(() => _visible = !_visible), child: new Text(_visible ? "Remove native views" : "Create native views")),
                        new M.TextButton(onPressed: () => setState(() => _generation++), child: new Text("Recreate native views")),
                        new M.TextButton(onPressed: () => setState(() => { _showWeb = !_showWeb; _generation++; }), child: new Text(_showWeb ? "Use native editor" : "Use WebView")),
                    ]),
                    new Text(_status),
                    .. _visible && owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformViews) ? new Widget[]
                    {
                        new SizedBox(height: _showWeb ? 140 : 80, child: new PlatformView(owner, _showWeb ? _web : _editor,
                            key: new ValueKey<string>($"native-{_generation}"), onCreated: Created, onError: e => Report(e.Message))),
                    } : [],
                    new M.TextField(controller: _last,
                        decoration: new M.InputDecoration(labelText: "Framework input after native views")),
                ]))));
    }
    public override void dispose()
    {
        _first.removeListener(Changed);
        _first.dispose(); _last.dispose();
        base.dispose();
    }
}
