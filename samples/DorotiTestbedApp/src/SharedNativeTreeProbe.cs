using Doroti.Framework;
using Doroti.Framework.Widgets;
using Doroti.Ui;

// Shared state sits above every View branch. The native probe reads the framework's own build evidence.
internal sealed class SharedNativeTreeProbe(DorotiApplicationViews views) : StatefulWidget
{
    public DorotiApplicationViews Views { get; } = views;
    public override IState createState() => new RootState();
    private sealed class Revision(int value, Widget child) : InheritedWidget(child: child)
    {
        public int Value { get; } = value;
        public override bool updateShouldNotify(InheritedWidget previous) => Value != ((Revision)previous).Value;
    }
    private sealed class RootState : State<SharedNativeTreeProbe>
    {
        private System.Threading.Timer? _timer;
        private int _revision;
        public override void initState()
        {
            base.initState();
            _timer = new System.Threading.Timer(_ => { _ = AdvanceAsync(); }, null, 100, 100);
        }
        private async Task AdvanceAsync()
        {
            var owner = widget.Views.Views.FirstOrDefault();
            if (owner is null) return;
            try { await owner.DispatchPlatformEventAsync(() => { if (mounted) setState(() => _revision++); }); }
            catch (OperationCanceledException) { }
            catch (ObjectDisposedException) { }
        }
        public override Widget build(BuildContext context) => new Revision(_revision,
            new DorotiApplicationViewCollection(widget.Views, (_, view) => new Leaf()));
        public override void dispose() { _timer?.Dispose(); _timer = null; base.dispose(); }
    }
    private sealed class Leaf : StatelessWidget
    {
        public override Widget build(BuildContext context)
        {
            var view = View.of(context);
            var revision = context.dependOnInheritedWidgetOfExactType<Revision>()!.Value;
            Console.WriteLine($"doroti.g1.build app={view.SceneOwner.ApplicationId} view={view.viewId} revision={revision}");
            return new Directionality(textDirection: TextDirection.ltr, child: new ColoredBox(color: new Color(revision % 2 == 0 ? 0xff0055aau : 0xff22aa55u),
                child: new Text($"Shared revision {revision} / view {view.viewId}", style: new Doroti.Framework.Painting.TextStyle(color: new Color(0xffffffff), fontSize: 26))));
        }
    }
}
