using System.Text;
using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Material;

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
        Report(_showWeb ? "WebView ready" : "Native editor ready");
    }
    public override Widget build(BuildContext context)
    {
        var owner = View.of(context);
        var value = _first.value;
        return new M.Scaffold(
            appBar: new PreferredSize(preferredSize: Size.fromHeight(56),
                child: new ClipRect(child: new M.AppBar(title: new Text("Input and native view lifetime")))),
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
                    new ClipRect(child: new M.TextField(controller: _last,
                        decoration: new M.InputDecoration(labelText: "Framework input after native views"))),
                ]))));
    }
    public override void dispose()
    {
        _first.removeListener(Changed);
        _first.dispose(); _last.dispose();
        base.dispose();
    }
}
