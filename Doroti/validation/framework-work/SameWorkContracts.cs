using System.Text.Json;
using System.Text.Json.Serialization;
using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Rendering;
using Doroti.Framework.Widgets;
using Doroti.Ui;

internal static class SameWorkContracts
{
    private static readonly List<string> Calls = [];
    private static int _tick;
    internal static void Verify(string output)
    {
        Calls.Clear();
        using var dispatcher = new PlatformDispatcher();
        using var dispatcherScope = dispatcher.EnterScope();
        using var host = new FixtureHost();
        using var view = dispatcher.RegisterView(1, new DorotiViewCapabilities("same-work")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
            .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, host)
            .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, host));
        var binding = new WidgetsFlutterBinding(dispatcher);
        FrameworkWorkTrace.Start();
        var owner = binding.buildOwner!;
        var container = new RenderPositionedBox(alignment: Alignment.topLeft, textDirection: TextDirection.ltr);
        var key = new GlobalKey<ProbeState>();
        RenderObjectToWidgetElement<RenderBox>? root = null;
        var states = new List<IState?>();
        for (_tick = 0; _tick < 5; _tick++)
        {
            FrameworkWorkTrace.SetTick(_tick);
            var right = _tick == 2;
            Widget Child(bool side) => _tick < 4 && side == right ? new Probe(key) : new SizedBox(width: 10, height: 10);
            var content = new ScopeWidget(1, new Row(textDirection: TextDirection.ltr, children: [
                new ScopeWidget(10, Child(false)), new ScopeWidget(20, Child(true))]));
            root = new RenderObjectToWidgetAdapter<RenderBox>(container: container, child: content).attachToRenderTree(owner, root);
            owner.buildScope(root);
            container.layout(BoxConstraints.CreateTight(new Size(200, 100)));
            owner.finalizeTree();
            states.Add(key.currentState);
        }
        if (states.Take(4).Any(state => state is null || !ReferenceEquals(state, states[0])) || states[4] is not null)
            throw new Exception("GlobalKey State lifetime changed");
        var trace = FrameworkWorkTrace.Stop();
        if (!trace.Valid) throw new Exception("Contract trace overflow");
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(output))!);
        File.WriteAllText(output, JsonSerializer.Serialize(new { calls = Calls, trace }, new JsonSerializerOptions {
            WriteIndented = true, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals }));

        var node = new object();
        FrameworkWorkTrace.Start(1);
        FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.Build, node);
        FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.Build, node);
        var overflow = FrameworkWorkTrace.Stop();
        if (overflow.Valid || overflow.Dropped != 1) throw new Exception("Overflow was accepted");
        Console.WriteLine($"PASS deterministic same-work fixture: {trace.Events.Length} events, {trace.Nodes.Length} nodes; overflow rejected");
    }
    private sealed class FixtureHost : IViewHostCapability, IFrameHostCapability, IPlatformMessageHostCapability
    {
        public ViewMetrics Metrics { get; } = new(new Size(200, 100), 1, default, default, default, AppLifecycleState.resumed, 1, 1);
        public DorotiViewEpoch ViewEpoch { get; } = new(1, 1, 1, 200, 100, 200, 100, 1, 1, 0);
        public event System.Action<ViewMetrics>? MetricsChanged { add { } remove { } }
        public event System.Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
        public event Action? CloseRequested { add { } remove { } }
        public event Action? Closed { add { } remove { } }
        public void Show() { }
        public void Resize(Size size) { }
        public void Close() { }
        public void Dispose() { }
        public void ScheduleFrame(System.Action<TimeSpan> callback) { }
        public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
        public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
    }
    private sealed class ScopeWidget(int value, Widget child) : InheritedWidget(child: child)
    {
        internal int Value => value;
        public override bool updateShouldNotify(InheritedWidget oldWidget) => ((ScopeWidget)oldWidget).Value != value;
    }
    private sealed class Probe(GlobalKey<ProbeState> key) : StatefulWidget(key: key)
    { public override IState createState() => new ProbeState(); }
    private sealed class ProbeState : State<Probe>
    {
        private void Log(string name) => Calls.Add($"{_tick}:probe:{name}");
        public override void initState()
        {
            base.initState();
            FrameworkWorkTrace.Register(context, 1);
            FrameworkWorkTrace.Register(this, 2);
            Log("init");
        }
        public override void didChangeDependencies() { base.didChangeDependencies(); Log("dependencies"); }
        public override void didUpdateWidget(Probe oldWidget) { base.didUpdateWidget(oldWidget); Log("update"); }
        public override void activate() { base.activate(); Log("activate"); }
        public override void deactivate() { Log("deactivate"); base.deactivate(); }
        public override void dispose() { Log("dispose"); base.dispose(); }
        public override Widget build(BuildContext context)
        {
            var value = context.dependOnInheritedWidgetOfExactType<ScopeWidget>()!.Value;
            Log($"build:{value}");
            return new SizedBox(width: value, height: 10);
        }
    }
}



