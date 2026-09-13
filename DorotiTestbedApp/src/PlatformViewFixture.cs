using Doroti.Framework.Painting;
using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

// The opt-in native runner probe changes the same widget state as these visible controls.
public static class PlatformViewFixtureProbe
{
    public static System.Action<int>? SetStage { get; internal set; }
    public static int Stage { get; internal set; } = 5;
    public static int ForegroundClicks { get; internal set; }
}

// Shared sample page and product acceptance fixture; host registration is required.
internal sealed class PlatformViewFixture : StatefulWidget
{
    internal PlatformViewFixture(bool embedded = false) { Embedded = embedded; }
    internal bool Embedded { get; }
    // Navigation can remount while the previous controls are still disposing asynchronously.
    private static long _nextExampleId = 1_000_000;
    private static readonly string[] Scenarios = [
        "Native controls", "Partial cover", "Full cover", "Restore controls", "Foreground behind controls",
        "Interleaved layers", "Translucent cover", "Pointer pass-through", "Reverse order", "Move editor",
    ];
    public override IState createState() => new FixtureState();
    private sealed class FixtureState : State<PlatformViewFixture>
    {
        private bool _mounted = true;
        private int _generation;
        private int _stage = 5;
        private long _exampleId;
        public override void initState()
        {
            base.initState();
            if (widget.Embedded) _exampleId = Interlocked.Add(ref _nextExampleId, 2);
        }
        private long InstanceId(long id) => (widget.Embedded ? _exampleId : _generation * 10L) + id;
        private void ToggleControls() => setState(() =>
        {
            _mounted = !_mounted;
            if (_mounted)
            {
                _generation++;
                if (widget.Embedded) _exampleId = Interlocked.Add(ref _nextExampleId, 2);
            }
        });
        private Widget Unavailable(string reason)
        {
            Widget message = new Padding(padding: EdgeInsets.CreateAll(24), child: new Text(
                $"Platform views\n\nThis example needs live native buttons, text input and interleaved composition.\n\n{reason}"));
            return widget.Embedded ? new ListView(children: [message]) :
                new M.Scaffold(appBar: new M.AppBar(title: new Text("PlatformView")), body: new Center(child: message));
        }
        public override void dispose()
        {
            PlatformViewFixtureProbe.SetStage = null;
            base.dispose();
        }
        public override Widget build(BuildContext context)
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_EVIDENCE")))
                Environment.SetEnvironmentVariable("DOROTI_PLATFORM_VIEW_PROBE_STATE",
                    $"{_stage},{PlatformViewFixtureProbe.ForegroundClicks},{_generation},{_mounted}");
            var owner = View.of(context);
            if (!owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformViews))
                return Unavailable($"{owner.targetIdentity}: platform views are unavailable on this host.");
            if (widget.Embedded)
            {
                var host = owner.RequireCapability<IPlatformViewHostCapability>(DorotiCapabilityIds.PlatformViews,
                    DartUiInvocation.Managed("PlatformViewExample.support"));
                foreach (var type in new[] { "doroti/native-button", "doroti/native-editor" })
                {
                    var support = host.QuerySupport(new PlatformViewRequest(InstanceId(1), type,
                        PlatformViewComposition.InterleavedComposition));
                    if (!support.Supported) return Unavailable(support.Reason ?? $"{type} is unavailable on this host.");
                }
            }
            if (!widget.Embedded && Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_COMPOSITION") == "overlay")
                return BuildOverlay(owner);
            PlatformViewFixtureProbe.SetStage = stage => setState(() =>
            {
                _stage = stage;
                PlatformViewFixtureProbe.Stage = stage;
            });
            Widget Native(long id, string type) => new PlatformView(owner,
                new PlatformViewRequest(InstanceId(id), type, PlatformViewComposition.InterleavedComposition),
                key: new ValueKey<string>($"{_generation}/{id}"));
            List<Widget> content = [
                    .. (widget.Embedded ? new Widget[] {
                        new Padding(padding: EdgeInsets.CreateAll(16), child: new Text(
                            "Platform views\n\nTry a native button and editor with Doroti layers.\nChange the overlap, open a modal, or recreate the controls.")),
                        new Padding(padding: EdgeInsets.CreateSymmetric(horizontal: 16), child: new Text(
                            $"{Scenarios[_stage]} · Foreground taps: {PlatformViewFixtureProbe.ForegroundClicks}")),
                    } : []),
                    new Wrap(children: [
                        new M.TextButton(onPressed: () => PlatformViewFixtureProbe.SetStage?.Invoke((_stage + 1) % 10),
                            child: new Text($"Overlap case {_stage}: next")),
                        new M.TextButton(onPressed: ToggleControls,
                            child: new Text(_mounted ? "Dispose controls" : "Create controls")),
                        new M.TextButton(onPressed: () => M.DialogLibrary.showDialog<object>(context,
                            dialogContext => new PointerInterceptor(SizedBox.CreateExpand(child: new GestureDetector(
                                behavior: HitTestBehavior.opaque, onTap: () => Navigator.pop<object>(dialogContext),
                                child: new M.AlertDialog(title: new Text("Native overlay shield"),
                                    content: new M.TextField()))), debug: true)), child: new Text("Open modal")),
                    ]),
                    widget.Embedded
                        ? new SingleChildScrollView(scrollDirection: Axis.horizontal, child: new SizedBox(width: 440, height: 240,
                            child: new Stack(children: BuildScene(Native))))
                        : new SizedBox(height: 360, child: new Stack(children: BuildScene(Native))),
                    new Padding(padding: EdgeInsets.CreateAll(16), child: new M.TextField(decoration: new M.InputDecoration(labelText: "Doroti IME / native focus return"))),
                ];
            return widget.Embedded ? new ListView(children: content) :
                new M.Scaffold(appBar: new M.AppBar(title: new Text($"PlatformView — owner {owner.viewId}")),
                    body: new Column(children: content));
        }

        private List<Widget> BuildScene(Func<long, string, Widget> native)
        {
            var children = new List<Widget> {
                new Positioned(left: 0, top: 0, right: 0, bottom: 0, child: new Container(color: new Color(0xffddeaff)))
            };
            Widget Button() => new Positioned(key: new ValueKey<string>("native-button-slot"), left: 20, top: 20,
                width: 220, height: 100, child: native(1, "doroti/native-button"));
            Widget Editor() => new Positioned(key: new ValueKey<string>("native-editor-slot"), left: _stage == 9 ? 210 : 180,
                top: 80, width: 220, height: 100, child: native(2, "doroti/native-editor"));
            Widget Foreground() => new Positioned(key: new ValueKey<string>("foreground-slot"),
                left: _stage == 2 ? 0 : 230, top: _stage == 2 ? 0 : 100,
                width: _stage == 2 ? 440 : 100, height: _stage == 2 ? 220 : 80,
                child: new PointerInterceptor(new GestureDetector(onTap: () =>
                    setState(() => PlatformViewFixtureProbe.ForegroundClicks++),
                    child: new Container(color: new Color(_stage >= 5 ? 0x99ff3300u : 0xffff3300u))),
                    intercepting: _stage != 7));
            if (_stage is 4 or 8) children.Add(Foreground());
            if (_mounted) children.Add(Button());
            if (_stage == 5) children.Add(new Positioned(left: 120, top: 60, width: 240, height: 80,
                child: new Container(color: new Color(0xff00aa55))));
            if (_mounted) children.Add(Editor());
            if (_stage is 1 or 2 or 5 or 6 or 7 or 9) children.Add(Foreground());
            return children;
        }

        private Widget BuildOverlay(DorotiView owner)
        {
            Widget Native(long id, string type) => new SizedBox(width: 240, height: 64,
                child: new PlatformView(owner, new PlatformViewRequest(_generation * 10L + id, type),
                    key: new ValueKey<string>($"overlay/{_generation}/{id}")));
            return new M.Scaffold(
                body: new Column(children: [
                    new Padding(padding: EdgeInsets.CreateAll(16), child: new Text($"NativeOverlay — owner {owner.viewId}")),
                    new Padding(padding: EdgeInsets.CreateAll(16), child: new Text("Basic native placement. Foreground menus and modals require InterleavedComposition.")),
                    new M.TextButton(onPressed: () => setState(() => { _mounted = !_mounted; if (_mounted) _generation++; }),
                        child: new Text(_mounted ? "Dispose controls" : "Create controls")),
                    new SizedBox(height: 24),
                    new Padding(padding: EdgeInsets.CreateAll(24), child: new M.TextField(
                        decoration: new M.InputDecoration(labelText: "Doroti IME / native focus return"))),
                    .. (_mounted ? new Widget[] {
                        Native(1, "doroti/native-button"), new SizedBox(height: 24), Native(2, "doroti/native-editor")
                    } : []),
                ]));
        }
    }
}
