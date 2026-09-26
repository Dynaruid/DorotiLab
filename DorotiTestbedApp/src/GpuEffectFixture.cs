using Doroti.Framework.Widgets;
using Doroti.Framework.Rendering;
using Doroti.Ui;
using Material = Doroti.Framework.Material;

internal sealed class GpuEffectFixture(bool backdrop = false) : StatefulWidget
{
    internal bool Backdrop { get; } = backdrop;
    public override IState createState() => new FixtureState();

    private sealed class FixtureState : State<GpuEffectFixture>
    {
        private bool _enabled = true;
        public override Widget build(BuildContext context) => new Material.Scaffold(
            appBar: new Material.AppBar(title: new Text("WGSL GPU effect")),
            body: new Center(child: new Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                    new Text("Original: red / green / blue"),
                    Pattern(),
                    new SizedBox(height: 24),
                    new Text(_enabled ? "WGSL: blue / green / red" : "Effect disabled"),
                    widget.Backdrop
                        ? new SizedBox(width: 300, height: 100, child: new Stack(children: [Pattern(),
                            new Positioned(left: 0, top: 0, right: 0, bottom: 0,
                                child: new GpuBackdropEffect(Doroti.Generated.Effects.SwapChannels, enabled: _enabled,
                                    child: new SizedBox()))]))
                        : new GpuEffect(Doroti.Generated.Effects.SwapChannels, enabled: _enabled, child: Pattern()),
                    new SizedBox(height: 24),
                    new Material.ElevatedButton(onPressed: () => setState(() => _enabled = !_enabled),
                        child: new Text("Toggle effect")),
                    new Text("Text and clip after the external GPU pass"),
                ])));

        private static Widget Pattern() => new SizedBox(width: 300, height: 100,
            child: new Row(crossAxisAlignment: CrossAxisAlignment.stretch, children: [
                new Expanded(child: new ColoredBox(color: new Color(0xffff0000))),
                new Expanded(child: new ColoredBox(color: new Color(0xff00ff00))),
                new Expanded(child: new ColoredBox(color: new Color(0xff0000ff))),
            ]));
    }
}
