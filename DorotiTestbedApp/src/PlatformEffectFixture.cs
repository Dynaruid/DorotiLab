using System.Text;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

internal sealed class PlatformEffectFixture : StatefulWidget
{
    public override IState createState() => new FixtureState();
    private sealed class FixtureState : State<PlatformEffectFixture>
    {
        private bool _blur = true, _block;
        private int _taps;
        private bool _mounted = true, _second, _moved;
        private int _generation;
        private static readonly byte[] Html = Encoding.UTF8.GetBytes("""
            <!doctype html><meta charset="utf-8"><style>
            body{margin:0;font:22px sans-serif;background:repeating-conic-gradient(#eee 0% 25%,#999 0% 50%) 0/40px 40px}
            input,button{font:22px sans-serif;margin:20px;padding:10px}#moving{background:#1877cc;color:white;width:150px;padding:20px;animation:move 3s infinite alternate}
            @keyframes move{to{transform:translateX(220px)}}p{margin:20px}</style>
            <input value="Native WebView2 text"><button style="position:absolute;left:360px;top:80px;z-index:2;margin:0;width:80px;height:55px" onclick="this.textContent=Number(this.textContent)+1;window.chrome?.webview?.postMessage(this.textContent)">0</button>
            <div id="moving">Live animation</div><p>Native backdrop source ABCDE 12345</p><div style="height:900px">Scroll native content</div>
            """);
        private static readonly byte[] SecondHtml = Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(Html)
            .Replace("#eee", "#ffeeaa").Replace("#999", "#dd9944").Replace("Native WebView2 text", "Second native WebView"));
        public override Widget build(BuildContext context)
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE")))
                Environment.SetEnvironmentVariable("DOROTI_PLATFORM_EFFECT_PROBE_STATE", $"{_blur},{_block},{_taps}");
            var owner = View.of(context);
            var host = owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews,
                DartUiInvocation.Managed("PlatformEffectFixture"));
            var request = new PlatformViewRequest(0, "doroti/webview", PlatformViewComposition.InterleavedComposition,
                CreationParameters: Html);
            var descriptor = new PlatformViewDescriptor("doroti/webview", Html, PlatformViewStrategyPolicy.RequireRequested);
            var support = host.QuerySupport(request);
            if (!support.Supported) return new M.Scaffold(body: new Center(child: new Text(support.Reason ?? "WebView attachment unavailable")));
            return new M.Scaffold(appBar: new M.AppBar(title: new Text("Native WebView + PlatformEffect")), body: new Column(children: [
                new Wrap(children: [
                    new M.TextButton(onPressed: () => setState(() => _blur = !_blur), child: new Text(_blur ? "Disable blur" : "Enable blur")),
                    new M.TextButton(onPressed: () => setState(() => _block = !_block), child: new Text(_block ? "Block input" : "Pass through input")),
                    new Text($"Foreground taps: {_taps}")]),
                new SizedBox(width: 620, height: 400, child: new Stack(children: [
                    .. _mounted ? new Widget[] { new Positioned(left: 0, top: 0, width: 620, height: 400,
                        child: new PlatformView(owner, descriptor, key: new Doroti.Framework.Foundation.ValueKey<int>(_generation))) } : [],
                    .. _mounted && _second ? new Widget[] {
                        new Positioned(left: 70, top: 45, width: 180, height: 160, child: new Container(color: new Color(0xff00aa55))),
                        new Positioned(left: 210, top: 100, width: 310, height: 210,
                            child: new PlatformView(owner, descriptor with { CreationParameters = SecondHtml },
                                key: new Doroti.Framework.Foundation.ValueKey<int>(-_generation - 1))) } : [],
                    new Positioned(left: _moved ? 180 : 130, top: _moved ? 100 : 65, width: 320, height: 210,
                        child: new PointerInterceptor(new PlatformEffect(
                            style: new(Strength: _blur ? .75 : 0, Tint: 0x33ffffff),
                            child: new Center(child: new Text("Sharp Doroti foreground"))), intercepting: _block)),
                    new Positioned(left: 190, top: 285, width: 240, height: 60,
                        child: new PointerInterceptor(new M.ElevatedButton(onPressed: () => setState(() => _taps++),
                            child: new Text("Doroti button"))))
                ])),
                new M.TextField(decoration: new M.InputDecoration(labelText: "Doroti focus return")),
                new Wrap(children: [
                    new M.TextButton(onPressed: () => setState(() => _second = !_second), child: new Text("Second WebView")),
                    new M.TextButton(onPressed: () => setState(() => _moved = !_moved), child: new Text("Move effect")),
                    new M.TextButton(onPressed: () => setState(() => { _mounted = !_mounted; if (_mounted) _generation++; }),
                        child: new Text(_mounted ? "Dispose WebViews" : "Create WebViews"))])
            ]));
        }
    }
}
