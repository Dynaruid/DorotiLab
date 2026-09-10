using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;
using M = Doroti.Framework.Material;

// Opt-in diagnostics only: DOROTI_TESTBED_MODE=media-query (or ?testbed=media-query on Web).
internal sealed class MediaQueryFixture : StatefulWidget
{
    public override IState createState() => new FixtureState();
    private sealed class FixtureState : State<MediaQueryFixture>
    {
        private bool _safe = true, _resize = true, _maintain, _sliver;
        public override Widget build(BuildContext context)
        {
            var media = MediaQuery.of(context);
            var view = Doroti.Framework.Widgets.View.of(context);
            var raw = view.metrics;
            var report = $"view={view.viewId} metrics={raw.generation} surface={raw.surfaceGeneration}\n" +
                $"physical={raw.physicalSize.width}×{raw.physicalSize.height} DPR={raw.devicePixelRatio}\n" +
                $"logical={media.size.width}×{media.size.height}\n" +
                $"padding={media.padding}\nviewPadding={media.viewPadding}\nviewInsets={media.viewInsets}\n" +
                $"gestures={media.systemGestureInsets} features={media.displayFeatures.Count}\n" +
                $"text={media.textScaler.scale(14):F2}/{media.textScaler.scale(32):F2} theme={media.platformBrightness}\n" +
                $"24h={media.alwaysUse24HourFormat} contrast={media.highContrast} motion={media.reduceMotion}";
            Console.WriteLine("MQ-FIXTURE " + System.Text.Json.JsonSerializer.Serialize(new {
                viewId = view.viewId, physicalWidth = raw.physicalSize.width, physicalHeight = raw.physicalSize.height,
                raw.devicePixelRatio, metricsGeneration = raw.generation, raw.surfaceGeneration,
                raw.viewPadding, raw.viewInsets, raw.systemGestureInsets,
                logicalWidth = media.size.width, logicalHeight = media.size.height, padding = media.padding.ToString(),
                logicalViewInsets = media.viewInsets.ToString(), safe = _safe, resize = _resize, maintain = _maintain }));
            Widget Content(BuildContext ctx) => new Padding(padding: EdgeInsets.CreateAll(12), child: new Column(
                mainAxisSize: MainAxisSize.min, crossAxisAlignment: CrossAxisAlignment.stretch, spacing: 8, children:
                [
                    new Text(report),
                    new Text($"Consumed padding: {MediaQuery.paddingOf(ctx)}"),
                    new Wrap(children: [
                        new M.TextButton(onPressed: () => setState(() => _safe = !_safe), child: new Text($"SafeArea: {_safe}")),
                        new M.TextButton(onPressed: () => setState(() => _resize = !_resize), child: new Text($"Scaffold resize: {_resize}")),
                        new M.TextButton(onPressed: () => setState(() => _maintain = !_maintain), child: new Text($"Maintain bottom: {_maintain}")),
                        new M.TextButton(onPressed: () => setState(() => _sliver = !_sliver), child: new Text($"SliverSafeArea: {_sliver}")),
                        new M.TextButton(onPressed: () => M.DialogLibrary.showDialog<object>(ctx,
                            _ => new M.AlertDialog(title: new Text("Inset dialog"), content: new M.TextField())), child: new Text("Dialog")),
                        new M.TextButton(onPressed: () => M.Bottom_sheetLibrary.showModalBottomSheet<object>(ctx,
                            _ => new SafeArea(child: new Padding(padding: EdgeInsets.CreateAll(20), child: new M.TextField()))), child: new Text("Bottom sheet")),
                    ]),
                    new M.TextField(decoration: new M.InputDecoration(labelText: "Keyboard / caret / focus")),
                    new M.TextField(minLines: 2, maxLines: 4, decoration: new M.InputDecoration(labelText: "Multiline editor")),
                    new SizedBox(height: 180),
                    new M.TextField(decoration: new M.InputDecoration(labelText: "Bottom input")),
                ]));
            Widget body = _sliver
                ? new CustomScrollView(slivers: [new SliverSafeArea(sliver: new SliverToBoxAdapter(child: new Builder(builder: Content)))])
                : new SingleChildScrollView(child: new Builder(builder: Content));
            if (_safe && !_sliver) body = new SafeArea(maintainBottomViewPadding: _maintain, child: body);
            return new M.Scaffold(backgroundColor: new Color(0xffe6f3ff), resizeToAvoidBottomInset: _resize,
                appBar: new M.AppBar(title: new Text("MediaQuery / SafeArea")), body: body,
                bottomNavigationBar: new SafeArea(maintainBottomViewPadding: _maintain,
                    child: new SizedBox(height: 40, child: new Center(child: new Text("Bottom edge marker")))));
        }
    }
}
