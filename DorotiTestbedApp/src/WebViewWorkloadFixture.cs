using Doroti.Framework.Services;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

internal sealed class WebViewWorkloadFixture : StatefulWidget
{
    public override IState createState() => new WorkloadState();
    private sealed class WorkloadState : State<WebViewWorkloadFixture>
    {
        private readonly List<WebViewController> _controllers = [];
        private readonly string _mode = Environment.GetEnvironmentVariable("DOROTI_WEBVIEW_WORKLOAD") ?? "idle";
        private bool _initialized;
        public override void didChangeDependencies()
        {
            base.didChangeDependencies();
            if (_initialized) return;
            _initialized = true;
            var count = int.Parse(Environment.GetEnvironmentVariable("DOROTI_WEBVIEW_COUNT") ?? "1");
            if (count is not (0 or 1 or 4)) throw new ArgumentOutOfRangeException(nameof(count));
            var html = "<!doctype html><meta charset=utf-8><style>body{margin:0;background:repeating-linear-gradient(45deg,#ffd080 0 20px,#4060c0 20px 40px);font:20px sans-serif}#box{width:60px;height:60px;background:#40e0b0}" +
                (_mode == "animation" ? "#box{animation:move 1s infinite alternate}@keyframes move{to{transform:translateX(160px)}}" : "") +
                "</style><input value='Native workload'><div id=box></div><div style='height:1200px'>Live WebView</div>" +
                (_mode == "scroll" ? "<script>setInterval(()=>scrollTo(0,(Date.now()/4)%900),16)</script>" : "");
            for (var i = 0; i < count; i++) _controllers.Add(new(View.of(context), new(Html: html)));
        }
        public override Widget build(BuildContext context) => new M.Scaffold(
            appBar: new M.AppBar(title: new Text($"WebView workload: {_controllers.Count} / {_mode}")),
            body: new Stack(children:
            [
                new Positioned(left: 0, top: 0, right: 0, height: 6,
                    child: _mode == "idle" ? new SizedBox() : new M.LinearProgressIndicator()),
                .. _controllers.Select((controller, index) => (Widget)new Positioned(
                    left: 20 + index % 2 * 320, top: 20 + index / 2 * 240, width: 300, height: 220,
                    child: new WebViewWidget(controller, key: new Doroti.Framework.Foundation.ValueKey<int>(index)))),
                .. _controllers.Count > 0 ? new Widget[] {
                    new Positioned(left: 80, top: 90, width: 220, height: 120,
                        child: new PointerInterceptor(new PlatformEffect(new(Strength: .375),
                            new Center(child: new Text("Sharp foreground"))), intercepting: _mode == "modal")) } : []
            ]));
        public override void dispose()
        {
            foreach (var controller in _controllers) _ = controller.DisposeAsync();
            _controllers.Clear();
            base.dispose();
        }
    }
}
