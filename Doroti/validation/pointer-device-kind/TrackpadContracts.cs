using Doroti.Host.Maui;
using Doroti.Framework.Gestures;
using Doroti.Ui;
using Doroti.Framework.Widgets;

internal static class TrackpadContracts
{
    internal static void Verify()
    {
        var packets = new List<MauiSurfacePointerData>();
        var stream = new MauiTrackpadGesture(77, packets.Add);
        var t = TimeSpan.Zero;
        stream.Begin(t, 100, 200);
        stream.Begin(t, 800, 900);
        stream.Update(t + TimeSpan.FromMilliseconds(10), 6, 12, 1.5, .2);
        stream.Update(t + TimeSpan.FromMilliseconds(20), 10, 18, 2, .4);
        stream.End(t + TimeSpan.FromMilliseconds(30));
        stream.End(t);
        Require(packets.Select(p => p.Change).SequenceEqual(new[] { PointerChange.add, PointerChange.panZoomStart,
            PointerChange.panZoomUpdate, PointerChange.panZoomUpdate, PointerChange.panZoomEnd }), "balanced phases");
        Require(packets.All(p => p.X == 100 && p.Y == 200 && p.Device == 77), "fixed origin/device during mixed pan and pinch");
        var update = packets[3];
        Require(update.PanDeltaX == 4 && update.PanDeltaY == 6 && update.Scale == 2 && update.Rotation == .4,
            "cumulative transforms produce incremental physical pan");
        var datum = new PointerData(1, update.Timestamp, update.Change, update.Kind, update.Device!.Value,
            update.X, update.Y, 0, 0, 0, pointerIdentifier: update.Pointer,
            panX: update.PanX, panY: update.PanY, panDeltaX: update.PanDeltaX, panDeltaY: update.PanDeltaY,
            scale: update.Scale, rotation: update.Rotation);
        var result = (PointerPanZoomUpdateEvent)PointerEventConverter.expand([datum], _ => 2).Single();
        Require(result.pan == new Offset(5, 9) && result.panDelta == new Offset(2, 3) && result.scale == 2 && result.rotation == .4,
            "DPR conversion preserves dimensionless scale/radians");
        var oldPointer = packets[1].Pointer;
        stream.CancelInertia(t);
        Require(packets[^1].SignalKind == PointerSignalKind.scrollInertiaCancel, "inertia cancellation");
        stream.Begin(t, 300, 400);
        Require(packets[^1].Pointer != oldPointer && packets[^1].Device == 77 && packets[^1].Scale == 1,
            "new gesture resets values and changes pointer identity");
        stream.Remove(t);
        Require(packets[^2].Change == PointerChange.panZoomEnd && packets[^1].Change == PointerChange.remove,
            "focus loss/detach terminates active gesture before removal");
        var count = packets.Count;
        stream.Remove(t);
        stream.Update(t, 1, 2);
        Require(packets.Count == count, "no duplicate terminal or updates after detach");
        Console.WriteLine("PASS: native trackpad lifecycle, mixed transforms, DPR, pointer identity, inertia cancel and detach");
        var androidPackets = new List<MauiSurfacePointerData>();
        var android = new AndroidTrackpadGesture(androidPackets.Add);
        Require(!android.Handle(1, PointerChange.down, PointerDeviceKind.mouse, true, 1, 10, 20, t), "real mouse is not a trackpad");
        Require(android.Handle(1, PointerChange.down, PointerDeviceKind.mouse, true, 0, 10, 20, t), "Android buttonless pan starts");
        android.Handle(1, PointerChange.move, PointerDeviceKind.mouse, true, 0, 20, 40, t);
        Require(androidPackets[^1] is { Kind: PointerDeviceKind.trackpad, PanX: 10, PanY: 20, PanDeltaY: 20 }, "Android source coordinates become pan");
        android.Cancel(t);
        Require(androidPackets[^2].Change == PointerChange.panZoomEnd && androidPackets[^1].Change == PointerChange.remove,
            "Android focus loss balances gesture");
        Console.WriteLine("PASS: Android buttonless SOURCE_MOUSE pan and normal mouse separation");
        VerifyRecognizer();
    }

    private static void VerifyRecognizer()
    {
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        var host = new TrackpadHost();
        using var view = dispatcher.RegisterView(1, host.Capabilities());
        using var binding = new WidgetsFlutterBinding(dispatcher);
        ScaleStartDetails? startDetails = null;
        ScaleUpdateDetails? updateDetails = null;
        var ends = 0;
        var recognizer = new ScaleGestureRecognizer
        {
            onStart = value => startDetails = value,
            onUpdate = value => updateDetails = value,
            onEnd = _ => ends++,
        };
        var start = new PointerPanZoomStartEvent(viewId: 1, pointer: 900, device: 77, position: new Offset(50, 60));
        recognizer.addPointerPanZoom(start);
        binding.pointerRouter.route(start);
        binding.gestureArena.close(900);
        binding.pointerRouter.route(new PointerPanZoomUpdateEvent(viewId: 1, pointer: 900, device: 77,
            position: new Offset(50, 60), pan: new Offset(10, 20), panDelta: new Offset(10, 20), scale: 1.5, rotation: .2));
        binding.pointerRouter.route(new PointerPanZoomUpdateEvent(viewId: 1, pointer: 900, device: 77,
            position: new Offset(50, 60), pan: new Offset(20, 30), panDelta: new Offset(10, 10), scale: 2, rotation: .4));
        Require(startDetails?.kind == PointerDeviceKind.trackpad && updateDetails is { scale: > 1, rotation: > .1 },
            "trackpad must reach real ScaleGestureRecognizer callbacks");
        binding.pointerRouter.route(new PointerPanZoomEndEvent(viewId: 1, pointer: 900, device: 77, position: new Offset(50, 60)));
        Require(ends == 1, "real scale recognizer ends exactly once");
        recognizer.dispose();
        Console.WriteLine("PASS: real framework scale recognizer receives trackpad pan/scale/rotation and one end callback");
        DragUpdateDetails? dragUpdate = null;
        var dragEnds = 0;
        var drag = new VerticalDragGestureRecognizer { onUpdate = value => dragUpdate = value, onEnd = _ => dragEnds++ };
        var panStart = new PointerPanZoomStartEvent(viewId: 1, pointer: 901, device: 77);
        drag.addPointerPanZoom(panStart);
        binding.pointerRouter.route(panStart);
        binding.gestureArena.close(901);
        binding.pointerRouter.route(new PointerPanZoomUpdateEvent(viewId: 1, pointer: 901, device: 77,
            pan: new Offset(0, 40), panDelta: new Offset(0, 40)));
        binding.pointerRouter.route(new PointerPanZoomUpdateEvent(viewId: 1, pointer: 901, device: 77,
            pan: new Offset(0, 50), panDelta: new Offset(0, 10)));
        binding.pointerRouter.route(new PointerPanZoomEndEvent(viewId: 1, pointer: 901, device: 77));
        Require(dragUpdate is { kind: PointerDeviceKind.trackpad, delta.dy: 10 } && dragEnds == 1,
            "trackpad pan must reach the scroll drag recognizer and end once");
        drag.dispose();
        Console.WriteLine("PASS: real framework vertical drag recognizer receives trackpad pan and end");
    }

    private static void Require(bool value, string message)
    { if (!value) throw new InvalidOperationException(message); }
}

file sealed class TrackpadHost : IViewHostCapability, IFrameHostCapability, IPlatformEnvironmentHostCapability, IPlatformMessageHostCapability
{
    public ViewMetrics Metrics { get; } = new(new Size(800, 600), 1, default, default, default, AppLifecycleState.resumed, 1, 1);
    public DorotiViewEpoch ViewEpoch => new(1, 1, 1, 800, 600, 800, 600, 1, 1, 0);
    public PlatformConfiguration Configuration { get; } = new([new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows);
    public event System.Action<ViewMetrics>? MetricsChanged { add { } remove { } }
    public event System.Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
    public event System.Action<AppLifecycleState>? LifecycleChanged { add { } remove { } }
    public event System.Action? CloseRequested { add { } remove { } }
    public event System.Action? Closed { add { } remove { } }
    internal DorotiViewCapabilities Capabilities() => new DorotiViewCapabilities("trackpad-contract")
        .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, this)
        .Register<IFrameHostCapability>(DorotiCapabilityIds.ViewFrameDispatch, this)
        .Register<IPlatformEnvironmentHostCapability>(DorotiCapabilityIds.PlatformEnvironment, this)
        .Register<IPlatformMessageHostCapability>(DorotiCapabilityIds.PlatformMessaging, this);
    public void Show() { } public void Resize(Size size) { } public void Close() { } public void Dispose() { }
    public void ScheduleFrame(System.Action<TimeSpan> callback) { }
    public ValueTask<ReadOnlyMemory<byte>?> SendAsync(string channel, ReadOnlyMemory<byte>? data, CancellationToken cancellationToken = default) => ValueTask.FromResult<ReadOnlyMemory<byte>?>(null);
    public void SetMessageHandler(string channel, PlatformMessageHandler? handler) { }
}
