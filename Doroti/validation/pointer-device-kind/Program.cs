using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Doroti.Framework.Gestures;
using Doroti.Framework.Widgets;
using Doroti.Host.Maui;
using Doroti.Host.Web;
using Doroti.Ui;
using PointerDownEvent = Doroti.Framework.Gestures.PointerDownEvent;
using PointerScrollEvent = Doroti.Framework.Gestures.PointerScrollEvent;

// Expectations come from the local Flutter engine and framework, not host enum casts.
var kinds = Enum.GetValues<PointerDeviceKind>();
Require(kinds.Select(kind => kind.ToString()).SequenceEqual(
    new[] { "touch", "mouse", "stylus", "invertedStylus", "trackpad", "unknown" }), "Flutter enum ABI");
var scrollBehavior = new ScrollBehavior();
Require(new PointerPanZoomStartEvent().kind == PointerDeviceKind.trackpad, "default pan-zoom start kind");
Require(new PointerPanZoomUpdateEvent() is { kind: PointerDeviceKind.trackpad, scale: 1 }, "default pan-zoom update kind/scale");
Require(new PointerPanZoomEndEvent().kind == PointerDeviceKind.trackpad, "default pan-zoom end kind");
Require(new PointerDownEvent() is { down: true, buttons: 1, pressure: 1 }, "default touch down initialization");
Require(new Doroti.Framework.Gestures.PointerMoveEvent() is { down: true, buttons: 1 }, "default touch move initialization");
Require(new PointerScaleEvent() is { kind: PointerDeviceKind.mouse, scale: 1 }, "default mouse scale initialization");
Require(new PointerScrollEvent().kind == PointerDeviceKind.mouse, "default mouse scroll kind");
foreach (var kind in kinds)
{
    Require(scrollBehavior.dragDevices.Contains(kind) == (kind != PointerDeviceKind.mouse),
        $"default dragDevices: {kind}");
    Require(EventsLibrary.computeHitSlop(kind, new DeviceGestureSettings(25)) ==
        (kind == PointerDeviceKind.mouse ? 1 : 25), $"hit slop: {kind}");
    Require(EventsLibrary.computePanSlop(kind, new DeviceGestureSettings(25)) ==
        (kind == PointerDeviceKind.mouse ? 2 : 50), $"pan slop: {kind}");
    Require(EventsLibrary.computeScaleSlop(kind) == (kind == PointerDeviceKind.mouse ? 1 : 18),
        $"scale slop: {kind}");
    var allowed = new ProbeRecognizer { supportedDevices = [kind] };
    foreach (var incoming in kinds.Where(value => value != PointerDeviceKind.trackpad))
    {
        var down = new PointerDownEvent(pointer: 10, kind: incoming, buttons: 1);
        Require(allowed.isPointerAllowed(down) == (incoming == kind), $"supportedDevices: {kind}/{incoming}");
    }
    Require(allowed.isPointerPanZoomAllowed(new PointerPanZoomStartEvent()) ==
        (kind == PointerDeviceKind.trackpad), $"pan-zoom device filter: {kind}");

    var changes = kind == PointerDeviceKind.trackpad
        ? new[] { PointerChange.add, PointerChange.panZoomStart, PointerChange.panZoomUpdate, PointerChange.panZoomEnd, PointerChange.remove }
        : new[] { PointerChange.add, PointerChange.hover, PointerChange.down, PointerChange.move, PointerChange.up, PointerChange.cancel, PointerChange.remove };
    foreach (var change in changes)
    {
        var datum = new PointerData(1, TimeSpan.FromMilliseconds(5), change, kind,
            7, 40, 60, 4, 6, 0, pointerIdentifier: 10, pressure: .4,
            panX: 20, panY: 30, panDeltaX: 2, panDeltaY: 4);
        var converted = PointerEventConverter.expand([datum], _ => 2).Single();
        Require(converted.kind == kind && converted.device == 7 && converted.position == new Offset(20, 30),
            $"packet device identity: {kind}/{change}");
        Require(converted.copyWith().kind == kind && converted.transformed(Matrix4.identity()).kind == kind,
            $"copied/transformed identity: {kind}/{change}");
        if (change is PointerChange.down or PointerChange.move)
            Require(converted.buttons == (kind == PointerDeviceKind.mouse ? 0 : 1), $"contact buttons: {kind}");
        if (converted is PointerDownEvent down)
        {
            allowed.addPointer(down);
            Require(allowed.getKindForPointer(10) == kind && allowed.Accepted == 1, $"recognizer remembers {kind}");
        }
        if (converted is PointerPanZoomUpdateEvent update)
            Require(update.pan == new Offset(10, 15) && update.panDelta == new Offset(1, 2), "trackpad logical deltas");
    }
    var wheel = PointerEventConverter.expand([
        new PointerData(1, TimeSpan.Zero, PointerChange.hover, kind, 7, 0, 0, 0, 0, 0,
            scrollDeltaY: 40, signalKind: PointerSignalKind.scroll)], _ => 2).Single();
    Require(wheel is PointerScrollEvent { scrollDelta.dy: 20 } && wheel.kind == kind, $"wheel identity: {kind}");
}
Console.WriteLine("PASS: six kinds, converter, contact buttons, recognizer filters, trackpad pan-zoom, scroll policy and slop");

var android = new[] { PointerDeviceKind.unknown, PointerDeviceKind.touch, PointerDeviceKind.stylus,
    PointerDeviceKind.mouse, PointerDeviceKind.invertedStylus, PointerDeviceKind.unknown };
for (var tool = 0; tool < android.Length; tool++)
    Require(AndroidPointerMapping.Kind(tool) == android[tool], $"Android tool {tool}");
Require(AndroidPointerMapping.Kind(-1) == PointerDeviceKind.unknown, "Android invalid tool");
Require(AndroidPointerMapping.Buttons(PointerDeviceKind.mouse, 0x7f) == 31, "Android combined mouse buttons");
Require(AndroidPointerMapping.Buttons(PointerDeviceKind.stylus, 32 | 64) == 6, "Android stylus buttons");
Require(AndroidPointerMapping.Buttons(PointerDeviceKind.invertedStylus, 32) == 0, "Flutter Android eraser buttons");
foreach (var lowBits in new uint[] { 0, 1, 0x7f })
{
    Require(WindowsPointerMapping.Kind(unchecked((nint)(0xff515700u | lowBits))) == PointerDeviceKind.stylus,
        "Windows promoted pen signature");
    Require(WindowsPointerMapping.Kind(unchecked((nint)(0xff515780u | lowBits))) == PointerDeviceKind.touch,
        "Windows promoted touch signature");
}
Require(WindowsPointerMapping.Kind(0) == PointerDeviceKind.mouse, "Windows real mouse");
Require(WindowsPointerMapping.Kind(unchecked((nint)0xab515780u)) == PointerDeviceKind.mouse, "Windows unrelated extra info");
Console.WriteLine("PASS: Android tool/button mapping and Windows promoted mouse-message signatures");

VerifyBrowserIngress();
TrackpadContracts.Verify();
if (OperatingSystem.IsWindows()) WindowsTrackpadContracts.Verify();

#pragma warning disable CA1416 // Reflection fixture exercises packet conversion only, without browser APIs.
static void VerifyBrowserIngress()
{
    // Execute the production packet ingress without bootstrapping a GPU/JS runtime.
    const BindingFlags instance = BindingFlags.Instance | BindingFlags.NonPublic;
    const BindingFlags statics = BindingFlags.Static | BindingFlags.NonPublic;
    var type = typeof(BrowserHostAdapter);
    var host = (BrowserHostAdapter)RuntimeHelpers.GetUninitializedObject(type);
    var snapshot = JsonSerializer.Deserialize<BrowserHostSnapshot>("""
        {"canvasId":"fixture","logicalWidth":100,"logicalHeight":100,"devicePixelRatio":2}
        """, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    type.GetField("_snapshot", instance)!.SetValue(host, snapshot);
    type.GetField("_viewId", instance)!.SetValue(host, 1UL);
    type.GetField("_pointerPositions", instance)!.SetValue(host, new Dictionary<ulong, (double X, double Y)>());
    var registry = (Dictionary<int, WeakReference<BrowserHostAdapter>>)type.GetField("Registry", statics)!.GetValue(null)!;
    const int hostId = -101;
    registry.Add(hostId, new(host));
    PointerDataPacket? packet = null;
    type.GetEvent("PointerData")!.AddEventHandler(host, (System.Action<PointerDataPacket>)(value => packet = value));
    try
    {
        var dispatch = type.GetMethod("DispatchPointerBatch", statics)!;
        long sequence = 0;
        foreach (var (wire, expected) in new[] { (0, PointerDeviceKind.mouse), (1, PointerDeviceKind.touch),
            (2, PointerDeviceKind.stylus), (4, PointerDeviceKind.unknown), (99, PointerDeviceKind.unknown) })
        {
            foreach (var phase in new[] { 5, 1, 0, 2, 4, 6 })
            {
                dispatch.Invoke(null, [hostId, phase, wire, 7, phase is 1 or 0 ? 1 : 0, 0,
                    ++sequence, new double[] { 12, 18, .4, 0, 0, 0, sequence * 10 }]);
                var datum = packet!.data.Single();
                Require(datum.kind == expected && datum.physicalX == 24 && datum.physicalY == 36,
                    $"browser ingress wire={wire} phase={phase}");
                Require(PointerEventConverter.expand(packet.data, _ => 2).Single().kind == expected,
                    $"browser to framework wire={wire}");
            }
        }
        var wheel = type.GetMethod("DispatchWheel", statics)!;
        wheel.Invoke(null, [hostId, 12d, 18d, 0d, -20d, 1000d, 3, ++sequence, 3, Math.Exp(.1)]);
        var scale = PointerEventConverter.expand(packet!.data, _ => 2).Single();
        Require(scale is PointerScaleEvent scaled && scaled.kind == PointerDeviceKind.trackpad &&
            Math.Abs(scaled.scale - Math.Exp(.1)) < 1e-9, "Web pinch reaches framework as scale, not scroll");
    }
    finally { registry.Remove(hostId); }
    Console.WriteLine("PASS: production browser packet ingress preserves mouse/touch/pen/unknown through framework conversion");
}
#pragma warning restore CA1416

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

file sealed class ProbeRecognizer : GestureRecognizer
{
    public int Accepted { get; private set; }
    public override string debugDescription => "pointer-device-contract";
    public override void addAllowedPointer(PointerDownEvent value) => Accepted++;
}
