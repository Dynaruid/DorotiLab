using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Doroti.Host.Maui;
using Doroti.Skia.Rendering;
using Foundation;
using Metal;
using SkiaSharp;
using UIKit;

UIApplication.Main(args, null, typeof(LifecycleDelegate));

[Register("LifecycleDelegate")]
public sealed class LifecycleDelegate : UIApplicationDelegate
{
    private UIWindow? _window;
    private DorotiUIKitGraphiteView? _view;
    private DorotiGraphiteView? _owner;
    private NSTimer? _timer;
    private IMTLSharedEvent? _gate;
    private volatile int _blockerStatus;
    private static readonly double DelaySeconds =
        Environment.GetEnvironmentVariable("DOROTI_METAL_TEST_DELAY_SECONDS") == "7" ? 7 : 1;
    private string? _blockerError;
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private double _delayStart;
    private int _phase;
    private int _painted;
    private int _completed;
    private int _stale;
    private int _staleBeforeCancellation;
    private int _released;
    private int _ticks;
    private int _waitingTicks;
    private bool _cancel;
    private double _disconnectMs;
    private int _delayedFrames;
    private readonly List<string> _failures = [];
    public override UIWindow? Window { get => _window; set => _window = value; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary? launchOptions)
    {
#pragma warning disable CA1422 // This isolated probe deliberately uses the pre-scene app lifecycle.
        _window = new UIWindow(UIScreen.MainScreen.Bounds);
#pragma warning restore CA1422
        var controller = new UIViewController();
        _view = new DorotiUIKitGraphiteView { Frame = _window.Bounds, AutoresizingMask = UIViewAutoresizing.FlexibleDimensions };
        controller.View!.AddSubview(_view);
        _window.RootViewController = controller;
        _owner = new DorotiGraphiteView();
        _owner.GraphitePaint += paint =>
        {
            _painted++;
            paint.Surface.Canvas.Clear(SKColors.CornflowerBlue);
            paint.Completion = new(_painted);
            paint.SkipPresent = _cancel;
        };
        _owner.GraphitePresentCompleted += (_, stale) => { if (stale) _stale++; else _completed++; };
        _owner.GraphiteFailed += (_, error) => _failures.Add(error.ToString());
        _owner.GpuResourcesReleasing += () => _released++;
        _view.Connect(_owner);
        _window.MakeKeyAndVisible();
        _timer = NSTimer.CreateRepeatingScheduledTimer(TimeSpan.FromMilliseconds(10), _ => Tick());
        return true;
    }

    private void Tick()
    {
        try
        {
            _ticks++;
            Assert(_blockerStatus != (int)MTLCommandBufferStatus.Error,
                $"The injected delay buffer failed: {_blockerError}");
            Assert(_clock.Elapsed.TotalSeconds < 30, "Lifecycle test exceeded 30-second internal deadline.");
            Assert(_failures.Count == 0, string.Join("\n", _failures));
            if (_phase == 0)
            {
                if (_completed < 3) { DrawFrame(); return; }
                if (Field<SkiaGraphiteSession>("_session").OutstandingFrames != 0) return;
                _cancel = true;
                _staleBeforeCancellation = _stale;
                _phase = 1;
                DrawFrame();
            }
            else if (_phase == 1)
            {
                // Wait for an actual canceled paint before proceeding.
                if (_stale == _staleBeforeCancellation) { DrawFrame(); return; }
                Assert(Field<SkiaGraphiteSession>("_session").OutstandingFrames == 0, "Cancelled frame leaked.");
                _cancel = false;
                _gate = _view!.Device!.CreateSharedEvent()!;
                using var blocker = Field<IMTLCommandQueue>("_queue").CommandBuffer()!;
                blocker.EncodeWait(_gate, 1);
                blocker.AddCompletedHandler(command =>
                {
                    _blockerError = command.Error?.LocalizedDescription;
                    _blockerStatus = (int)command.Status;
                    Console.WriteLine($"delay-buffer status={command.Status} error={_blockerError}");
                });
                blocker.Commit();
                _phase = 2;
                DrawFrame();
            }
            else if (_phase == 2)
            {
                var session = Field<SkiaGraphiteSession>("_session");
                if (session.OutstandingFrames < 3) { DrawFrame(); return; }
                _delayedFrames = session.OutstandingFrames;
                Assert(_delayedFrames == 3, "Expected three pending drawable frames.");
                var painted = _painted;
                // At capacity the delegate must return before borrowing a
                // fourth drawable, even when called directly by the host.
                _view!.Draw(_view);
                Assert(_painted == painted, "Frame backpressure did not bound admission.");
                var before = Stopwatch.GetTimestamp();
                _view.Disconnect();
                _disconnectMs = Stopwatch.GetElapsedTime(before).TotalMilliseconds;
                Assert(_disconnectMs < 100, "Disconnect blocked on the GPU.");
                Assert(_released == 0, "Resources released before GPU completion.");
                using var replacement = new DorotiUIKitGraphiteView();
                var rejected = false;
                try { replacement.Connect(new DorotiGraphiteView()); }
                catch (InvalidOperationException) { rejected = true; }
                Assert(rejected, "New generation admitted during retirement.");
                _view.RemoveFromSuperview();
                _view.Dispose();
                _delayStart = _clock.Elapsed.TotalSeconds;
                _phase = 3;
            }
            else if (_phase == 3)
            {
                _waitingTicks++;
                Assert(_released == 0, "GPU resources retired while queue was delayed.");
                if (_clock.Elapsed.TotalSeconds - _delayStart < DelaySeconds) return;
                _gate!.SignaledValue = 1;
                _phase = 4;
            }
            else if (_phase == 4 && _released == 1)
            {
                Assert(_waitingTicks >= 20, "UI run loop did not remain responsive.");
                Assert(_stale >= 4, "Cancelled and disconnected completions were not stale.");
                Assert(Field<bool>("_resourcesReleased"), "Queue/session not released.");
                using var fresh = new DorotiUIKitGraphiteView();
                fresh.Connect(new DorotiGraphiteView());
                fresh.Disconnect();
                Finish(null);
            }
        }
        catch (Exception error) { Finish(error); }
    }

    private void DrawFrame()
    {
        // Exercise the same invalidation used by the production host.
        _view!.SetNeedsDisplay();
        _view.Draw();
    }

    private T Field<T>(string name) => (T)typeof(DorotiUIKitGraphiteView)
        .GetField(name, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(_view)!;
    private static void Assert(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    private void Finish(Exception? error)
    {
        _timer?.Invalidate();
        var gateValueBeforeCleanup = _gate?.SignaledValue;
        if (_gate is not null) _gate.SignaledValue = 1;
        var report = new { status = error is null ? "PASS" : "FAIL", error = error?.ToString(),
            blockerStatus = _blockerStatus, blockerError = _blockerError, gateValueBeforeCleanup, requestedDelaySeconds = DelaySeconds,
            painted = _painted, completed = _completed, stale = _stale, released = _released, ticks = _ticks,
            phase = _phase,
            delayedFrames = _delayedFrames, disconnectMs = _disconnectMs, responsiveTicks = _waitingTicks,
            runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
            physicalInput = "notVerified", permanentStall = "notVerified", deviceLoss = "notVerified" };
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(json);
        var path = Environment.GetEnvironmentVariable("DOROTI_UIKIT_LIFECYCLE_EVIDENCE");
        if (path is not null) File.WriteAllText(path, json);
        Environment.Exit(error is null ? 0 : 1);
    }
}
