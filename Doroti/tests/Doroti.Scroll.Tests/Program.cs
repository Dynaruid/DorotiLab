using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using Doroti.Framework.Gestures;
using Doroti.Framework.Widgets;
using Doroti.Hosting;
using Doroti.Host.Maui;
using Doroti.Runtime;
using Doroti.Ui;

if (args.Length == 1)
{
    ReplayTrackpadReport(args[0]);
    return;
}

VelocityTracker[] trackers =
[
    new(PointerDeviceKind.trackpad),
    new(PointerDeviceKind.touch),
    new IOSScrollViewFlingVelocityTracker(PointerDeviceKind.touch),
    new MacOSScrollViewFlingVelocityTracker(PointerDeviceKind.trackpad),
];
Check(AndroidPointerMapping.Timestamp(TimeSpan.FromMilliseconds(500), 1500, 1444)
    == TimeSpan.FromMilliseconds(444), "Android retains 56 ms of hardware input age");
Check(AndroidPointerMapping.Timestamp(TimeSpan.FromMilliseconds(10), 1500, 1400)
    == TimeSpan.Zero, "Android events cannot precede the framework clock origin");
Check(AndroidPointerMapping.Timestamp(TimeSpan.FromMilliseconds(500), 1500, 0)
    == TimeSpan.FromMilliseconds(500), "Android missing event time uses arrival time");
foreach (var tracker in trackers)
{
    for (var i = 0; i < 8; i++)
    {
        tracker.addPosition(Duration.Create(milliseconds: i * 8), new Offset(0, -i * 8));
    }
}
// One slow frame after the last hardware move must not turn a quick release
// into a hold. The input timestamps still say release occurred 8 ms later.
Thread.Sleep(65);
foreach (var tracker in trackers)
{
    var velocity = tracker.getVelocityEstimateAt(Duration.Create(milliseconds: 64))!;
    Check(Math.Abs(velocity.pixelsPerSecond.dy + 1000) < 0.01,
        $"delayed {tracker.GetType().Name}/{tracker.kind} release retains velocity");
    var simulation = new ClampingScrollSimulation(500, -velocity.pixelsPerSecond.dy);
    Check(simulation.x(0.1) > 550 && !simulation.isDone(0.1), "ballistic scroll continues after release");
    Check(tracker.getVelocityEstimateAt(Duration.Create(milliseconds: 120))!.pixelsPerSecond == Offset.zero,
        "real pause before release does not fling");
    Check(tracker.getVelocityEstimate()!.pixelsPerSecond == Offset.zero,
        "parameterless velocity still observes wall-clock inactivity");
}

var node = new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 100, 40), "item", null,
    SemanticsAction.tap, []);
var previous = new Dictionary<int, SemanticsNodeUpdate> { [1] = node };
IReadOnlyList<SemanticsNodeUpdate>[] snapshots =
[
    [node], [], [node, node with { id = 2 }], [node with { id = 2 }],
    [node with { rect = Rect.fromLTWH(0, 5, 100, 40) }],
    [node with { label = "changed" }], [node with { value = "on" }],
    [node with { actions = SemanticsAction.longPress }],
    [node with { flags = new SemanticsFlags(isEnabled: Tristate.isFalse) }],
    [node with { children = [2] }], [node with { textSelectionExtent = 2 }],
    [node with { scrollPosition = 10 }], [node with { identifier = "new" }],
    [node with { role = SemanticsRole.listItem }],
];
foreach (var snapshot in snapshots)
{
    Check(SemanticsUpdateDiffer.HasChanges(previous, snapshot)
        == SemanticsUpdateDiffer.Diff(previous, snapshot).HasChanges,
        "allocation-free snapshot comparison matches detailed delta");
}

if (OperatingSystem.IsWindows())
{
    NativeInertiaRegression.Run();
    NativeInputSurfaceRegression.Run();
}
ScrollContinuityRegression.Run();
ProjectionRegression.Run();
CaretGeometryRegression.Run();
Console.WriteLine("PASS: release velocity, ballistic continuation, semantic changes, native inertia lifecycle");

static void Check(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static void ReplayTrackpadReport(string path)
{
    using var document = JsonDocument.Parse(File.ReadAllText(path));
    var ingress = document.RootElement.GetProperty("inputIngress");
    var ratio = ingress.GetProperty("devicePixelRatio").GetDouble();
    var packets = ingress.GetProperty("trackpadPackets").Deserialize<PointerData[]>(
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    VelocityTracker? tracker = null;
    foreach (var packet in packets)
    {
        var time = Duration.Create(microseconds: packet.timeStamp.Ticks / 10);
        if (packet.change == PointerChange.panZoomStart)
        {
            tracker = new(PointerDeviceKind.trackpad);
            tracker.addPosition(time, Offset.zero);
        }
        else if (packet.change == PointerChange.panZoomUpdate && tracker is not null)
        {
            tracker.addPosition(time, new Offset(packet.panX / ratio, packet.panY / ratio));
        }
        else if (packet.change == PointerChange.panZoomEnd && tracker is not null)
        {
            var estimate = tracker.getVelocityEstimateAt(time)!;
            var physics = new ClampingScrollPhysics();
            var recognizer = new VerticalDragGestureRecognizer
            {
                minFlingDistance = physics.minFlingDistance,
                minFlingVelocity = physics.minFlingVelocity,
            };
            var velocity = recognizer.isFlingGesture(estimate, PointerDeviceKind.trackpad)
                ? Math.Clamp(-estimate.pixelsPerSecond.dy, -physics.maxFlingVelocity, physics.maxFlingVelocity)
                : 0;
            var simulation = velocity == 0 ? null : new ClampingScrollSimulation(0, velocity);
            var duration = 0.0;
            if (simulation is not null)
            {
                var lower = 0.0;
                var upper = 10.0;
                for (var i = 0; i < 20; i++)
                {
                    var middle = (lower + upper) / 2;
                    if (simulation.isDone(middle)) upper = middle;
                    else lower = middle;
                }
                duration = upper;
            }
            Console.WriteLine(JsonSerializer.Serialize(new {
                releaseMilliseconds = packet.timeStamp.TotalMilliseconds,
                velocity, sampleMilliseconds = estimate.duration.inMilliseconds,
                sampleOffset = estimate.offset.dy,
                expectedDurationMilliseconds = Math.Round(duration * 1000),
                expectedDistance = simulation?.x(duration) ?? 0,
            }));
            tracker = null;
        }
    }
}

internal static unsafe class NativeInertiaRegression
{
    internal static void Run()
    {
        var packets = new List<PointerDataPacket>();
        // Exercise the production status callback without a physical device or
        // constructing a DirectManipulation manager. Only ZoomToRect is used.
        var owner = (WindowsPrecisionTrackpad)RuntimeHelpers.GetUninitializedObject(typeof(WindowsPrecisionTrackpad));
        Set(owner, "_dispatch", (Action<PointerDataPacket>)packets.Add);
        Set(owner, "_running", true);
        Set(owner, "_last", new PointerData(1, TimeSpan.Zero, PointerChange.panZoomUpdate,
            PointerDeviceKind.trackpad, 1, 0, 0, 0, 0, 0, panY: -56, panDeltaY: -8));
        var table = (nint*)NativeMemory.AllocZeroed(14, (nuint)sizeof(nint));
        var viewport = (nint*)NativeMemory.AllocZeroed(1, (nuint)sizeof(nint));
        table[13] = (nint)(delegate* unmanaged[Stdcall]<nint, float, float, float, float, int, int>)&Reset;
        *viewport = (nint)table;
        try
        {
            Status(owner, (nint)viewport, 4, 3);
            if (packets.Count != 1 || packets[0].data[0].change != PointerChange.panZoomEnd)
                throw new InvalidOperationException("native inertia must end contact exactly once");
            // A coarse native tick may leave a large final delta at READY.
            Set(owner, "_lastDeltaY", 2.0);
            Status(owner, (nint)viewport, 5, 4);
            if (packets.Count != 1)
                throw new InvalidOperationException("natural native completion canceled framework inertia");
            // Recontact must stop momentum even if the last native delta was zero.
            Set(owner, "_lastDeltaY", 0.0);
            Set(owner, "_running", true);
            Status(owner, (nint)viewport, 3, 4);
            if (packets.Count != 2 || packets[^1].data[0].signalKind != PointerSignalKind.scrollInertiaCancel)
                throw new InvalidOperationException("recontact did not cancel framework inertia");
        }
        finally
        {
            NativeMemory.Free(viewport);
            NativeMemory.Free(table);
        }
    }

    private static void Set(object owner, string name, object value) =>
        typeof(WindowsPrecisionTrackpad).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(owner, value);

    private static void Status(object owner, nint viewport, uint current, uint previous) =>
        typeof(WindowsPrecisionTrackpad).GetMethod("Status", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(owner, [viewport, current, previous]);

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static int Reset(nint viewport, float left, float top, float right, float bottom, int animate) => 0;
}

internal static class NativeInputSurfaceRegression
{
    internal static void Run()
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            nint parent = 0, child = 0;
            try
            {
                parent = CreateWindowExW(0, "STATIC", "input owner", 0x80000000,
                    0, 0, 200, 200, 0, 0, 0, 0);
                child = CreateWindowExW(0, "STATIC", "raster input surface", 0x40000000,
                    0, 0, 200, 200, parent, 0, 0, 0);
                if (parent == 0 || child == 0) throw new InvalidOperationException("hidden test HWND creation failed");
                var owner = new WindowsPrecisionTrackpad(parent, 1, _ => { }, child, child, parent, 0);
                try
                {
                    using var diagnostics = JsonDocument.Parse(JsonSerializer.Serialize(owner.CaptureDiagnostics()));
                    var windows = diagnostics.RootElement.GetProperty("activeWindows").EnumerateArray()
                        .Select(value => value.GetInt64()).ToArray();
                    if (windows.Length != 2 || !windows.Contains(parent.ToInt64()) || !windows.Contains(child.ToInt64()))
                        throw new InvalidOperationException("one DM manager must register both distinct input surfaces");
                    var id = (nuint)typeof(WindowsPrecisionTrackpad).GetField("_subclassId", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(owner)!;
                    var updates = typeof(WindowsPrecisionTrackpad).GetField("_updatesCalled", BindingFlags.Instance | BindingFlags.NonPublic)!;
                    var before = (long)updates.GetValue(owner)!;
                    SendMessageW(parent, 0x0113, id, 0);
                    SendMessageW(child, 0x0113, id, 0);
                    if ((long)updates.GetValue(owner)! != before + 2)
                        throw new InvalidOperationException("both input surfaces must deliver real window messages to the manager");
                    owner.Dispose();
                    var disposedUpdates = (long)updates.GetValue(owner)!;
                    SendMessageW(parent, 0x0113, id, 0);
                    SendMessageW(child, 0x0113, id, 0);
                    if ((long)updates.GetValue(owner)! != disposedUpdates)
                        throw new InvalidOperationException("disposing the manager left an input subclass attached");
                }
                finally { owner.Dispose(); }
            }
            catch (Exception error) { failure = error; }
            finally
            {
                if (child != 0) DestroyWindow(child);
                if (parent != 0) DestroyWindow(parent);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null) throw new InvalidOperationException("native input surface regression", failure);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern nint CreateWindowExW(uint extendedStyle, string className, string name,
        uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint parameter);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(nint window);
    [DllImport("user32.dll")]
    private static extern nint SendMessageW(nint window, uint message, nuint wparam, nint lparam);
}
