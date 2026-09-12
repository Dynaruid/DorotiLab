using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using AppKit;
using CoreGraphics;
using Doroti.Host.Maui;
using Doroti.Skia.Rendering;
using Foundation;
using Metal;
using MetalKit;
using SkiaSharp;

NSApplication.Init();
NSApplication.SharedApplication.Delegate = new HostDelegate();
NSApplication.Main(args);

public sealed class HostDelegate : NSApplicationDelegate
{
    private DorotiMacOSMetalView _view = null!;
    private object _owner = null!;
    private NSWindow _window = null!;
    private NSTimer _timer = null!;
    private IMTLSharedEvent? _gate;
    private volatile int _blockerStatus;
    private static readonly double DelaySeconds =
        Environment.GetEnvironmentVariable("DOROTI_METAL_TEST_DELAY_SECONDS") == "7" ? 7 : 1;
    private string? _blockerError;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private int _phase;
    private int _ticks;
    private int _responsiveTicks;
    private int _painted;
    private double _delayStart;
    private double _disconnectMs;
    private readonly List<string> _errors = [];
    private const BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;

    public override void DidFinishLaunching(NSNotification notification)
    {
        _window = new NSWindow(new CGRect(100, 100, 320, 240), NSWindowStyle.Titled | NSWindowStyle.Resizable,
            NSBackingStore.Buffered, false);
        _view = new DorotiMacOSMetalView { Frame = new CGRect(0, 0, 320, 240) };
        _window.ContentView!.AddSubview(_view);
        _owner = Activator.CreateInstance(typeof(DorotiMacOSMetalSurface), Private, null, new object[] { 1UL }, null)!;
        var paintEvent = _owner.GetType().GetEvent("Paint", Private)!;
        paintEvent.GetAddMethod(true)!.Invoke(_owner, new object[] {
            Delegate.CreateDelegate(paintEvent.EventHandlerType!, this, GetType().GetMethod(nameof(Paint))!) });
        var failureEvent = _owner.GetType().GetEvent("PaintFailed", Private)!;
        // Paint failures are also observable in the native snapshot below.
        _ = failureEvent;
        Call("Connect", _owner);
        _window.MakeKeyAndOrderFront(null);
        NSApplication.SharedApplication.Activate();
        _timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromMilliseconds(10), _ => Tick());
    }

    public void Paint(object context)
    {
        var surface = (SKSurface)context.GetType().GetProperty("Surface", Private)!.GetValue(context)!;
        surface.Canvas.Clear(SKColors.Coral);
        _painted++;
    }

    private void Tick()
    {
        try
        {
            _ticks++;
            Assert(_blockerStatus != (int)MTLCommandBufferStatus.Error,
                $"The injected delay buffer failed: {_blockerError}");
            Assert(_clock.Elapsed.TotalSeconds < 30, "Probe deadline exceeded.");
            Assert(Field<long>("_commandBuffersErrored") == 0, "Metal command error.");
            if (_phase == 0)
            {
                if (Field<long>("_commandBuffersCompleted") < 3 || Field<int>("_inFlight") != 0)
                { _view.NeedsDisplay = true; return; }
                _gate = _view.Device!.CreateSharedEvent()!;
                using var blocker = Field<IMTLCommandQueue>("_commandQueue").CommandBuffer()!;
                blocker.EncodeWait(_gate, 1);
                blocker.AddCompletedHandler(command =>
                {
                    _blockerError = command.Error?.LocalizedDescription;
                    _blockerStatus = (int)command.Status;
                    Console.WriteLine($"delay-buffer status={command.Status} error={_blockerError}");
                });
                blocker.Commit();
                _phase = 1;
            }
            if (_phase == 1)
            {
                if (Field<int>("_inFlight") < 3) { _view.NeedsDisplay = true; return; }
                Assert(Field<SkiaGraphiteSession>("_graphite").OutstandingFrames == 3, "Frame accounting mismatch.");
                var painted = _painted;
                ((IMTKViewDelegate)_view).Draw(_view);
                Assert(painted == _painted, "Backpressure did not bound rendering.");
                var before = Stopwatch.GetTimestamp();
                Call("Disconnect");
                _disconnectMs = Stopwatch.GetElapsedTime(before).TotalMilliseconds;
                Assert(_disconnectMs < 100, "Disconnect waited on GPU.");
                Assert(!Field<bool>("_resourcesReleased"), "Premature resource release.");
                _view.RemoveFromSuperview();
                _view.Dispose();
                _delayStart = _clock.Elapsed.TotalSeconds;
                _phase = 2;
            }
            else if (_phase == 2)
            {
                _responsiveTicks++;
                Assert(!Field<bool>("_resourcesReleased"), "Delayed GPU resources released.");
                if (_clock.Elapsed.TotalSeconds - _delayStart < DelaySeconds) return;
                _gate!.SignaledValue = 1;
                _phase = 3;
            }
            else if (_phase == 3 && Field<bool>("_resourcesReleased"))
            {
                Assert(Field<int>("_inFlight") == 0, "Outstanding command buffers.");
                Assert(_responsiveTicks >= 20, "UI did not remain responsive.");
                Assert(Field<long>("_staleCompletions") >= 3, "Disconnected callbacks were not stale.");
                Finish(null);
            }
        }
        catch (Exception error) { Finish(error); }
    }
    private T Field<T>(string name) => (T)_view.GetType().GetField(name, Private)!.GetValue(_view)!;
    private void Call(string name, params object[] args) => _view.GetType().GetMethod(name, Private)!.Invoke(_view, args);
    private static void Assert(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    private void Finish(Exception? error)
    {
        _timer.Invalidate();
        var gateValueBeforeCleanup = _gate?.SignaledValue;
        if (_gate is not null) _gate.SignaledValue = 1;
        var report = new { status = error is null ? "PASS" : "FAIL", error = error?.ToString(),
            blockerStatus = _blockerStatus, blockerError = _blockerError, gateValueBeforeCleanup, requestedDelaySeconds = DelaySeconds,
            painted = _painted, completed = Field<long>("_commandBuffersCompleted"), inFlight = Field<int>("_inFlight"),
            released = Field<bool>("_resourcesReleased"), stale = Field<long>("_staleCompletions"),
            disconnectMs = _disconnectMs, responsiveTicks = _responsiveTicks, phase = _phase, ticks = _ticks };
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(json);
        var path = Environment.GetEnvironmentVariable("DOROTI_APPKIT_HOST_LIFECYCLE_EVIDENCE");
        if (path is not null) File.WriteAllText(path, json);
        Environment.Exit(error is null ? 0 : 1);
    }
}
