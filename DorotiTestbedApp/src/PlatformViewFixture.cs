using Doroti.Framework.Painting;
using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

// Product acceptance fixture; runner capability/compositor registration is required.
internal sealed class PlatformViewFixture : StatefulWidget
{
    public override IState createState() => new FixtureState();
    private sealed class FixtureState : State<PlatformViewFixture>
    {
        private bool _mounted = true;
        private int _generation;
        public override Widget build(BuildContext context)
        {
            var owner = View.of(context);
            if (!owner.registeredCapabilityIds.Contains(DorotiCapabilityIds.PlatformViews))
                return new M.Scaffold(appBar: new M.AppBar(title: new Text("PlatformView")), body: new Center(
                    child: new Text($"{owner.targetIdentity}: platform.views is not registered.")));
            Widget Native(long id, string type) => new PlatformView(owner,
                new PlatformViewRequest(_generation * 10L + id, type, PlatformViewComposition.InterleavedComposition),
                key: new ValueKey<string>($"{_generation}/{id}"));
            return new M.Scaffold(appBar: new M.AppBar(title: new Text($"PlatformView — owner {owner.viewId}")),
                body: new Column(children: [
                    new Wrap(children: [
                        new M.TextButton(onPressed: () => setState(() => { _mounted = !_mounted; if (_mounted) _generation++; }),
                            child: new Text(_mounted ? "Dispose controls" : "Create controls")),
                        new M.TextButton(onPressed: () => M.DialogLibrary.showDialog<object>(context,
                            _ => new PointerInterceptor(new M.AlertDialog(title: new Text("Native overlay shield"),
                                content: new M.TextField()), debug: true)), child: new Text("Open modal")),
                    ]),
                    new SizedBox(height: 360, child: new Stack(children: [
                        new Positioned(left: 0, top: 0, right: 0, bottom: 0, child: new Container(color: new Color(0xffddeaff))),
                        .. (_mounted ? new Widget[] { new Positioned(left: 20, top: 20, width: 220, height: 80, child: Native(1, "doroti/native-button")) } : []),
                        new Positioned(left: 120, top: 60, width: 240, height: 80, child: new Container(color: new Color(0x9900aa55))),
                        .. (_mounted ? new Widget[] { new Positioned(left: 180, top: 100, width: 220, height: 100, child: Native(2, "doroti/native-editor")) } : []),
                        new Positioned(left: 50, top: 150, width: 160, height: 70, child: new PointerInterceptor(
                            new Container(color: new Color(0x99ff9933), child: new Center(child: new Text("Doroti foreground"))), debug: true)),
                    ])),
                    new Padding(padding: EdgeInsets.CreateAll(16), child: new M.TextField(decoration: new M.InputDecoration(labelText: "Doroti IME / native focus return"))),
                ]));
        }
    }
}
