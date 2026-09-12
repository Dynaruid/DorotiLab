using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using CoreAnimation;
using CoreGraphics;
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
    private bool _activationChecked = false;
    private bool _resizeChecked;
    private int _resizePasses;
    private int _transactionPaints;
    private int _centerSamples;
    private double _centerError;
    private double _topLeftCenterError;
    private double _markerSizeError;
    private double _resizeMarkerSizeError;
    private bool _injectDeadline;
    private int _deadlineFaults;
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
            if (_view!.PresentsWithTransaction)
            {
                _transactionPaints++;
                Assert(paint.Surface.Canvas.DeviceClipBounds.Width == Math.Round(_view.Bounds.Width * _view.ContentScaleFactor),
                    "Transaction paint used the previous orientation's drawable width.");
                Assert(paint.Surface.Canvas.DeviceClipBounds.Height == Math.Round(_view.Bounds.Height * _view.ContentScaleFactor),
                    "Transaction paint used the previous orientation's drawable height.");
            }
            paint.Surface.Canvas.Clear(SKColors.CornflowerBlue);
            paint.Completion = new(_painted);
            paint.SkipPresent = _cancel;
        };
        _owner.GraphitePresentCompleted += (_, stale) =>
        {
            Assert(NSThread.IsMain, "Completion left the UIKit owner thread.");
            if (stale) _stale++; else _completed++;
        };
        _owner.GraphiteFailed += (_, error) =>
        {
            if (_injectDeadline && error is TimeoutException) _deadlineFaults++;
            else _failures.Add(error.ToString());
        };
        _owner.GpuResourcesReleasing += () =>
        {
            Assert(NSThread.IsMain, "GPU retirement left the UIKit owner thread.");
            _released++;
        };
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
#if IOS && !MACCATALYST
                if (!_activationChecked)
                {
                    var painted = _painted;
                    NSNotificationCenter.DefaultCenter.PostNotificationName(UIApplication.WillResignActiveNotification, null);
                    _view!.Draw(_view);
                    Assert(_painted == painted, "Inactive application admitted a GPU frame.");
                    NSNotificationCenter.DefaultCenter.PostNotificationName(UIApplication.DidBecomeActiveNotification, null);
                    _view.Draw(_view);
                    Assert(_painted > painted, "Activation did not reopen frame admission.");
                    _activationChecked = true;
                    return;
                }
#endif
                if (!_resizeChecked)
                {
                    var original = _view!.Frame;
                    var painted = _painted;
                    var transactionPaints = _transactionPaints;
                    // Swap both axes in each direction, as device rotation does.
                    _view.Frame = new CoreGraphics.CGRect(original.X, original.Y, original.Height, original.Width);
                    _view.SetNeedsLayout();
                    _view.LayoutIfNeeded();
                    Assert(_view.DrawableSize.Width == Math.Round(_view.Bounds.Width * _view.ContentScaleFactor), "Resize did not update drawable width.");
                    Assert(_view.DrawableSize.Height == Math.Round(_view.Bounds.Height * _view.ContentScaleFactor), "Resize did not update drawable height.");
                    Assert(_painted > painted, "Resize deferred its paint beyond the layout transaction.");
#if IOS && !MACCATALYST
                    Assert(_transactionPaints > transactionPaints, "Resize paint did not join UIKit's transaction.");
                    if (_centerSamples == 0) CheckRotationContentCenter();
                    using var referenceView = new UIView { ContentMode = UIViewContentMode.Center };
                    Assert(_view.Layer.ContentsGravity == referenceView.Layer.ContentsGravity,
                        "Layout did not retain unscaled, centered content alignment.");
#endif
                    Assert(!_view.PresentsWithTransaction, "Resize left ordinary frames in transaction presentation mode.");
                    _resizeChecked = ++_resizePasses == 2;
                    return;
                }
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
                // Invoke the real deadline callback while a real GPU queue is
                // pending. This checks fault/hold semantics without claiming
                // that the driver sustained a five-second GPU stall.
                _injectDeadline = true;
                Field<NSTimer>("_retirementTimer").Fire();
                _injectDeadline = false;
                Assert(_deadlineFaults == 1, "Retirement deadline was not reported.");
                Assert(_released == 0 && session.OutstandingFrames == 3 && !session.IsDeviceLost,
                    "Deadline released pending resources or became device loss.");
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

#if IOS && !MACCATALYST
    private void CheckRotationContentCenter()
    {
        // Core Animation composites a known center marker using the production
        // view's actual gravity/scale. Probe old and new images against endpoint
        // and intermediate bounds. This is a bitmap compositing regression, not
        // a capture of the system's physical rotation animation or Metal output.
        using var sourceFormat = new UIGraphicsImageRendererFormat { Scale = _view!.ContentScaleFactor };
        using var outputFormat = new UIGraphicsImageRendererFormat { Scale = 1 };
        using var referenceView = new UIView { ContentMode = UIViewContentMode.Center };
        foreach (var sourceSize in new[] { new CGSize(120, 240), new CGSize(240, 120) })
        {
            using var sourceRenderer = new UIGraphicsImageRenderer(sourceSize, sourceFormat);
            using var source = sourceRenderer.CreateImage(context =>
            {
                context.CGContext.SetFillColor(1, 0, 0, 1);
                context.CGContext.FillRect(new CGRect(sourceSize.Width / 2 - 10, sourceSize.Height / 2 - 10, 20, 20));
            });
            foreach (var size in new[] { new CGSize(120, 240), new CGSize(180, 180), new CGSize(240, 120) })
            {
                (double CenterError, double SizeError) MeasureMarker(string gravity)
                {
                    using var layer = new CALayer
                    {
                        Bounds = new CGRect(CGPoint.Empty, size), Contents = source.CGImage,
                        ContentsScale = _view.ContentScaleFactor, ContentsGravity = gravity,
                        MasksToBounds = true,
                    };
                    using var renderer = new UIGraphicsImageRenderer(size, outputFormat);
                    using var rendered = renderer.CreateImage(context => layer.RenderInContext(context.CGContext));
                    using var png = rendered.AsPNG()!;
                    using var pixels = SKBitmap.Decode(png.ToArray());
                    double xSum = 0, ySum = 0;
                    var count = 0;
                    var left = pixels.Width;
                    var top = pixels.Height;
                    var right = -1;
                    var bottom = -1;
                    for (var y = 0; y < pixels.Height; y++)
                    for (var x = 0; x < pixels.Width; x++)
                    {
                        var color = pixels.GetPixel(x, y);
                        if (color.Red < 200 || color.Green > 40 || color.Blue > 40 || color.Alpha < 200) continue;
                        xSum += x + 0.5; ySum += y + 0.5; count++;
                        left = Math.Min(left, x); right = Math.Max(right, x);
                        top = Math.Min(top, y); bottom = Math.Max(bottom, y);
                    }
                    Assert(count > 0, "Center marker disappeared during bounds interpolation.");
                    return (Math.Max(Math.Abs(xSum / count - (double)size.Width / 2),
                        Math.Abs(ySum / count - (double)size.Height / 2)),
                        Math.Max(Math.Abs(right - left + 1 - 20), Math.Abs(bottom - top + 1 - 20)));
                }
                var actual = MeasureMarker(_view.Layer.ContentsGravity);
                _centerError = Math.Max(_centerError, actual.CenterError);
                _markerSizeError = Math.Max(_markerSizeError, actual.SizeError);
                var reference = MeasureMarker(referenceView.Layer.ContentsGravity);
                Assert(reference.CenterError <= 1 && reference.SizeError <= 1, "Centered UIView changed the marker geometry.");
                _topLeftCenterError = Math.Max(_topLeftCenterError, MeasureMarker(CALayer.GravityTopLeft).CenterError);
                _resizeMarkerSizeError = Math.Max(_resizeMarkerSizeError, MeasureMarker(CALayer.GravityResize).SizeError);
                _centerSamples++;
            }
        }
        Assert(_centerError <= 1, $"Rotation content drifted {_centerError} pixels from the layer center.");
        Assert(_markerSizeError <= 1, $"Rotation stretched the marker by {_markerSizeError} pixels.");
        Assert(_topLeftCenterError > 10, "Negative control did not reproduce the old top-left content drift.");
        Assert(_resizeMarkerSizeError > 10, "Negative control did not reproduce scale-to-fill stretching.");
    }
#endif

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
            activationNotificationContract = _activationChecked, resizeContract = _resizeChecked,
            resizePasses = _resizePasses, transactionPaints = _transactionPaints,
            centerSamples = _centerSamples, centerErrorPixels = _centerError, topLeftCenterErrorPixels = _topLeftCenterError,
            markerSizeErrorPixels = _markerSizeError, resizeMarkerSizeErrorPixels = _resizeMarkerSizeError,
            injectedRetirementDeadlineFaults = _deadlineFaults, actualFiveSecondStall = "notVerified",
            runtime = System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier,
            physicalInput = "notVerified", permanentStall = "notVerified", deviceLoss = "notVerified" };
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(json);
        var path = Environment.GetEnvironmentVariable("DOROTI_UIKIT_LIFECYCLE_EVIDENCE");
        if (path == "1") path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "uikit-metal-lifecycle.json");
        if (path is not null) File.WriteAllText(path, json);
        Environment.Exit(error is null ? 0 : 1);
    }
}
