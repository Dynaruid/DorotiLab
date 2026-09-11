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

// Product acceptance fixture; runner capability/compositor registration is required.
internal sealed class PlatformViewFixture : StatefulWidget
{
    public override IState createState() => new FixtureState();
    private sealed class FixtureState : State<PlatformViewFixture>
    {
        private bool _mounted = true;
        private int _generation;
        private int _stage = 5;
        public override void dispose()
        {
            PlatformViewFixtureProbe.SetStage = null;
            base.dispose();
        }
        public override Widget build(BuildContext context)
        {
            var owner = View.of(context);
            if (!owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformViews))
                return new M.Scaffold(appBar: new M.AppBar(title: new Text("PlatformView")), body: new Center(
                    child: new Text($"{owner.targetIdentity}: platform.views is not registered.")));
            if (Environment.GetEnvironmentVariable("DOROTI_PLATFORM_VIEW_COMPOSITION") == "overlay")
                return BuildOverlay(owner);
            PlatformViewFixtureProbe.SetStage = stage => setState(() =>
            {
                _stage = stage;
                PlatformViewFixtureProbe.Stage = stage;
            });
            Widget Native(long id, string type) => new PlatformView(owner,
                new PlatformViewRequest(_generation * 10L + id, type, PlatformViewComposition.InterleavedComposition),
                key: new ValueKey<string>($"{_generation}/{id}"));
            return new M.Scaffold(appBar: new M.AppBar(title: new Text($"PlatformView — owner {owner.viewId}")),
                body: new Column(children: [
                    new Wrap(children: [
                        new M.TextButton(onPressed: () => PlatformViewFixtureProbe.SetStage?.Invoke((_stage + 1) % 10),
                            child: new Text($"Overlap case {_stage}: next")),
                        new M.TextButton(onPressed: () => setState(() => { _mounted = !_mounted; if (_mounted) _generation++; }),
                            child: new Text(_mounted ? "Dispose controls" : "Create controls")),
                        new M.TextButton(onPressed: () => M.DialogLibrary.showDialog<object>(context,
                            dialogContext => new PointerInterceptor(SizedBox.CreateExpand(child: new GestureDetector(
                                behavior: HitTestBehavior.opaque, onTap: () => Navigator.pop<object>(dialogContext),
                                child: new M.AlertDialog(title: new Text("Native overlay shield"),
                                    content: new M.TextField()))), debug: true)), child: new Text("Open modal")),
                    ]),
                    new SizedBox(height: 360, child: new Stack(children: BuildScene(Native))),
                    new Padding(padding: EdgeInsets.CreateAll(16), child: new M.TextField(decoration: new M.InputDecoration(labelText: "Doroti IME / native focus return"))),
                ]));
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
            return new M.Scaffold(appBar: new M.AppBar(title: new Text($"NativeOverlay — owner {owner.viewId}")),
                body: new Column(children: [
                    new Padding(padding: EdgeInsets.CreateAll(16), child: new Text("Basic native placement. Foreground menus and modals require InterleavedComposition.")),
                    new M.TextButton(onPressed: () => setState(() => { _mounted = !_mounted; if (_mounted) _generation++; }),
                        child: new Text(_mounted ? "Dispose controls" : "Create controls")),
                    new SizedBox(height: 24),
                    .. (_mounted ? new Widget[] {
                        Native(1, "doroti/native-button"), new SizedBox(height: 24), Native(2, "doroti/native-editor")
                    } : []),
                    new Padding(padding: EdgeInsets.CreateAll(24), child: new M.TextField(
                        decoration: new M.InputDecoration(labelText: "Doroti IME / native focus return"))),
                ]));
        }
    }
}
