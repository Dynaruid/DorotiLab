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
    private bool _probeStarted;
    private int _created;
    private readonly bool _mixed = Environment.GetEnvironmentVariable("DOROTI_MIXED_SCENE_PROBE") == "1";
    private string _status = "Native views preparing";
    private readonly List<PlatformViewHandle> _createdWeb = [];
    private readonly PlatformViewDescriptor _editor = new("doroti/native-editor", Encoding.UTF8.GetBytes("{\"text\":\"Native editor\"}"));
    private readonly PlatformViewDescriptor _web = new("doroti/webview", Encoding.UTF8.GetBytes("{\"html\":\"<meta charset='utf-8'><label>Web input <input aria-label='Web input'></label>\"}"));

    public override void initState()
    {
        base.initState();
        if (Environment.GetEnvironmentVariable("DOROTI_WINDOWS_APPSDK_TEXT_SMOKE") == "1" ||
            Environment.GetEnvironmentVariable("DOROTI_QT_TEXT_GEOMETRY_SMOKE") == "1")
            _first.text = "한글 😀 e\u0301 עברית line wrap text";
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
    private void WebCreated(PlatformViewHandle handle) { _createdWeb.Add(handle); Created(handle); }
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
            var perScene = _mixed ? 2 : 1;
            await WaitForCreated(perScene);
            async Task VerifyMixedWeb()
            {
                if (!_mixed) return;
                var host = owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews,
                    DorotiUiInvocation.Managed("mixed native probe"));
                var web = (IWebViewHostCapability)host;
                var deadline = DateTime.UtcNow.AddSeconds(15);
                while (true)
                {
                    var state = await web.ExecuteWebViewAsync(_createdWeb[^1], new(WebViewOperation.State));
                    if (!state.IsLoading && state.DocumentGeneration > 0) break;
                    if (DateTime.UtcNow > deadline) throw new TimeoutException("Mixed WebView did not finish initial HTML.");
                    await Task.Delay(50);
                }
                var result = await web.ExecuteWebViewAsync(_createdWeb[^1], new(WebViewOperation.EvaluateJavaScript,
                    "document.querySelector('input')?.setAttribute('value','mixed 한글');document.querySelector('input')?.getAttribute('value')"));
                if (result.Json?.Contains("mixed") != true) throw new Exception("Mixed WebView JavaScript did not reach its native control.");
            }
            await VerifyMixedWeb();
            owner.DispatchPlatformEvent(() => setState(() => _generation++));
            await WaitForCreated(perScene * 2);
            // The host currently uses distinct composition topologies for HWND and WebView.
            owner.DispatchPlatformEvent(() => setState(() => _visible = false));
            await Task.Delay(250);
            owner.DispatchPlatformEvent(() => setState(() => { _showWeb = true; _visible = true; _generation++; }));
            await WaitForCreated(perScene * 3);
            owner.DispatchPlatformEvent(() => setState(() => _generation++));
            await WaitForCreated(perScene * 4);
            await VerifyMixedWeb();
            owner.DispatchPlatformEvent(() => setState(() => _visible = false));
            await Task.Delay(250);
            File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(
                new InputLifetimeProbeResult(perScene * 4, true, true, _mixed, "notVerified"),
                TestbedProbeJsonContext.Default.InputLifetimeProbeResult));
        }
        catch (Exception error) { File.WriteAllText(path + ".error", error.ToString()); }
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
                            key: new ValueKey<string>($"native-{_generation}"), onCreated: _showWeb ? WebCreated : Created, onError: e => Report(e.Message))),
                    } : [],
                    .. _visible && _mixed && owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformViews)
                        ? new Widget[] { new SizedBox(height: _showWeb ? 80 : 140,
                            child: new PlatformView(owner, _showWeb ? _editor : _web, key: new ValueKey<string>($"mixed-{_generation}"),
                                onCreated: _showWeb ? Created : WebCreated, onError: e => Report(e.Message))) } : [],
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
