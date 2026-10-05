using Doroti.Runtime;
using Doroti.Testing;
using Doroti.Framework.Widgets;
using Doroti.Framework.Scheduler;
using Doroti.Ui;
using Doroti.Host.Maui;
using Doroti.Framework.Foundation;
using Doroti.Framework.Rendering;
using Doroti.Hosting;

internal static class FullReviewRegression
{
    static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
    static void Terminal(Task task) => task.WaitAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
    static void Canceled(Task task)
    {
        try { Terminal(task); throw new Exception("Canceled Future completed successfully."); }
        catch (OperationCanceledException) { }
    }

    public static void Run()
    {
        var layers = EngineLayer.debugResourceDiagnostics.ActiveEngineLayers;
        Futures(); OwnerIsolation(); Frames(); ScheduledTasks(); Snapshots(); SnapshotWidgets(); Cleanup(); AppleState();
        Require(EngineLayer.debugResourceDiagnostics.ActiveEngineLayers == layers, "Snapshot/widget teardown retained an engine layer.");
        Console.WriteLine("PASS: full-review Future terminal/error ownership, mid-frame microtasks, CPU snapshot pixels/identity, cleanup and Apple state policy.");
    }

    static void Futures()
    {
        using var life = new CancellationTokenSource();
        var queue = new System.Collections.Concurrent.ConcurrentQueue<Action>();
        using var enqueued = new ManualResetEventSlim();
        using var owner = DorotiExecutionContext.EnterDispatcher(action => { queue.Enqueue(action); enqueued.Set(); return true; }, life.Token);
        var source = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
        var filtered = false; var handled = false;
        var future = Future<int>.fromTask(source.Task).catchError((Func<Exception, int>)(_ => { handled = true; return 42; }),
            _ => { filtered = true; return true; });
        source.SetException(new InvalidOperationException("background"));
        Require(enqueued.Wait(TimeSpan.FromSeconds(5)), "Error callback was never queued.");
        Require(!filtered && !handled, "Error filter/handler ran outside its captured owner queue.");
        while (queue.TryDequeue(out var callback)) callback();
        Terminal(future.asTask()); Require(future.asTask().Result == 42 && filtered && handled, "Recovery value lost.");
        var untypedHandled = false;
        var untyped = Future.error(new Exception("untyped")).catchError((Action<Exception>)(_ => untypedHandled = true), _ => true);
        Require(!untypedHandled, "Untyped error handler bypassed its queue.");
        while (queue.TryDequeue(out var callback)) callback();
        Terminal(untyped.asTask()); Require(untypedHandled, "Untyped handler missing.");
        var recoveredFuture = Future<int>.error(new Exception()).then<int>((Func<int, Future<int>>)(value => Future<int>.value(value)),
            (Func<Exception, Future<int>>)(_ => Future<int>.value(9)));
        while (queue.TryDequeue(out var callback)) callback(); Terminal(recoveredFuture.asTask());
        Require(recoveredFuture.asTask().Result == 9, "Future-valued then error recovery failed.");
        var filterFailure = new InvalidOperationException("filter failure");
        var filterThrows = Future<int>.error(new Exception()).catchError((Func<Exception,int>)(_ => 0), _ => throw filterFailure);
        while (queue.TryDequeue(out var callback)) callback();
        try { Terminal(filterThrows.asTask()); throw new Exception("Filter exception swallowed."); }
        catch (InvalidOperationException error) { Require(ReferenceEquals(error, filterFailure), "Filter exception replaced."); }
        var original = new InvalidOperationException("original");
        var rejectedFilter = Future<int>.error(original).catchError((Func<Exception, int>)(_ => 99), _ => false);
        while (queue.TryDequeue(out var callback)) callback();
        try { Terminal(rejectedFilter.asTask()); throw new Exception("False filter swallowed original."); }
        catch (InvalidOperationException error) { Require(ReferenceEquals(error, original), "Original error identity lost."); }
        var pending = new TaskCompletionSource<int>();
        var nested = new TaskCompletionSource<int>();
        var canceledNested = Future<int>.value(1).then<int>((Func<int,Future<int>>)(_ => Future<int>.fromTask(nested.Task)));
        while (queue.TryDequeue(out var callback)) callback();
        var canceledBeforeCompletion = Future<int>.fromTask(pending.Task).then((Func<int, int>)(value => value + 1));
        var canceledAfterEnqueue = Future<int>.value(1).then((Func<int, int>)(_ => throw new Exception("Canceled callback ran.")));
        life.Cancel();
        Canceled(canceledBeforeCompletion.asTask()); Canceled(canceledAfterEnqueue.asTask());
        Canceled(canceledNested.asTask());
        while (queue.TryDequeue(out var callback)) callback();
        pending.SetResult(1);
        using (DorotiExecutionContext.EnterDispatcher(_ => false))
            Canceled(Future<int>.value(1).then((Func<int, int>)(value => value)).asTask());
        using (DorotiExecutionContext.EnterDispatcher(_ => throw new InvalidOperationException("post failure")))
        {
            try { Terminal(Future<int>.value(1).then((Func<int, int>)(value => value)).asTask()); throw new Exception("Post failure lost."); }
            catch (InvalidOperationException error) { Require(error.Message == "post failure", "Post failure changed."); }
        }
    }

    static void Frames()
    {
        using var tester = new WidgetTester();
        tester.pumpWidget(new SizedBox());
        var binding = SchedulerBinding.instance;
        var trace = new List<string>();
        binding.scheduleFrameCallback(_ => {
            trace.Add("A");
            DartAsyncRuntime.scheduleMicrotask(() => {
                Require(binding.schedulerPhase == SchedulerPhase.midFrameMicrotasks, "Microtask phase disagrees with delivery.");
                trace.Add("M");
                DartAsyncRuntime.scheduleMicrotask(() => trace.Add("M2"));
            });
        });
        binding.addPersistentFrameCallback(_ => { if (trace.Count != 0) trace.Add("B"); });
        binding.addPostFrameCallback(_ => trace.Add("C"));
        tester.pump(TimeSpan.FromMilliseconds(16));
        Require(string.Join(" ", trace) == "A M M2 B C", "Frame order: " + string.Join(" ", trace));
        trace.Clear();
        binding.scheduleFrameCallback(_ => { trace.Add("A"); DartAsyncRuntime.scheduleMicrotask(() => trace.Add("M")); });
        binding.addPostFrameCallback(_ => trace.Add("C"));
        binding.scheduleWarmUpFrame();
        Require(string.Join(" ", trace) == "A M B C", "Warm-up did not drain before draw.");
        var dispatcher = PlatformDispatcher.instance;
        Action<DorotiView, TimeSpan> failedBegin = (_, _) => throw new InvalidOperationException("begin observer");
        dispatcher.beginFrame += failedBegin;
        try { tester.pump(); throw new Exception("Frame begin failure was not reported."); }
        catch (AggregateException) { }
        finally { dispatcher.beginFrame -= failedBegin; }
        Require(binding.schedulerPhase == SchedulerPhase.idle, "Begin failure left the scheduler phase active.");
        binding.scheduleFrameCallback(_ => DartAsyncRuntime.scheduleMicrotask(() => throw new InvalidOperationException("microtask")));
        try { tester.pump(); throw new Exception("Frame microtask failure was not reported."); }
        catch (AggregateException) { }
        Require(binding.schedulerPhase == SchedulerPhase.idle, "Microtask failure left the scheduler phase active.");
        tester.pump();
    }

    static void OwnerIsolation()
    {
        var ownerTag = new AsyncLocal<string?>();
        var first = new Queue<Action>(); var second = new Queue<Action>();
        Future<int> a, b;
        ownerTag.Value = "A";
        using (DorotiExecutionContext.EnterDispatcher(action => { first.Enqueue(action); return true; }))
            a = Future<int>.error(new Exception()).onError((Func<Exception,int>)(_ => { Require(ownerTag.Value == "A", "A callback escaped its owner context."); return 1; }));
        ownerTag.Value = "B";
        using (DorotiExecutionContext.EnterDispatcher(action => { second.Enqueue(action); return true; }))
            b = Future<int>.error(new Exception()).catchError((Func<Exception,int>)(_ => { Require(ownerTag.Value == "B", "B callback escaped its owner context."); return 2; }));
        ownerTag.Value = "foreign pump";
        second.Dequeue()(); Require(!a.asTask().IsCompleted, "Pumping B completed A.");
        first.Dequeue()(); Terminal(a.asTask()); Terminal(b.asTask());
        Require(a.asTask().Result == 1 && b.asTask().Result == 2, "Owner queue values mixed.");
    }

    static void ScheduledTasks()
    {
        using var tester = new WidgetTester();
        tester.pumpWidget(new SizedBox());
        var binding = SchedulerBinding.instance;
        var error = new InvalidOperationException("scheduled error");
        var failed = binding.scheduleTask<int>(() => throw error, Priority.animation);
        var next = binding.scheduleTask<int>(() => 7, Priority.animation);
        var reported = 0; var oldError = FlutterError.onError;
        FlutterError.onError = details => { Require(ReferenceEquals(details.exception, error), "Scheduler reported the wrong error."); reported++; };
        try { binding.handleEventLoopCallback(); }
        finally { FlutterError.onError = oldError; }
        Require(reported == 1, "Scheduler did not report exactly once.");
        try { Terminal(failed.asTask()); throw new Exception("Throwing scheduled task did not completeError."); }
        catch (InvalidOperationException caught) { Require(ReferenceEquals(caught, error), "Scheduler lost original failure."); }
        binding.handleEventLoopCallback(); Terminal(next.asTask()); Require(next.asTask().Result == 7, "Scheduler stopped after a synchronous failure.");
    }

    static void Snapshots()
    {
        using var tester = new WidgetTester(new Size(8, 8));
        var recorder = new PictureRecorder(); var canvas = new Canvas(recorder);
        canvas.save(); canvas.translate(1, 1); canvas.clipRect(Rect.fromLTWH(0, 0, 2, 2));
        canvas.drawRect(Rect.fromLTWH(0, 0, 4, 4), new Paint { color = new Color(0xffff0000) }); canvas.restore();
        using var picture = recorder.endRecording();
        var builder = new SceneBuilder(tester.View.viewId); builder.addPicture(Offset.zero, picture);
        using var scene = builder.build();
        using var image = scene.toImageSync(4, 4);
        using var other = scene.toImageSync(4, 4);
        using var clone = image.clone(); using var chained = clone.clone();
        Require(!image.isCloneOf(other) && image.isCloneOf(clone) && chained.isCloneOf(image), "Image storage identity is incorrect.");
        var bytes = image.toByteData().asTask(); Terminal(bytes);
        var data = bytes.Result!.buffer.asUint8List().ToArray();
        Require(data[0] == 0 && data[3] == 0 && data[(1 * 4 + 1) * 4] == 255 && data[(1 * 4 + 1) * 4 + 3] == 255,
            "Scene snapshot clip/translate/color pixels failed.");
        var png = clone.toByteData(ImageByteFormat.png).asTask(); Terminal(png);
        Require(png.Result!.lengthInBytes > 8, "Snapshot PNG missing.");
        var decodedTask = tester.View.RequireCapability<IImageHostCapability>(DorotiCapabilityIds.GraphicsImage, new("snapshot round trip"))
            .DecodeAsync(png.Result.buffer.asUint8List().Select(value => (byte)value).ToArray(), new("snapshot round trip")).AsTask();
        Terminal(decodedTask); using var decoded = decodedTask.Result;
        var repaintRecorder = new PictureRecorder(); var repaintCanvas = new Canvas(repaintRecorder);
        repaintCanvas.drawImage(decoded, Offset.zero, new Paint()); using var repaintPicture = repaintRecorder.endRecording();
        var repaintBuilder = new SceneBuilder(tester.View.viewId); repaintBuilder.addPicture(Offset.zero, repaintPicture);
        using var repaintScene = repaintBuilder.build(); using var repainted = repaintScene.toImageSync(4, 4);
        var repaintBytes = repainted.toByteData().asTask(); Terminal(repaintBytes);
        Require(repaintBytes.Result!.buffer.asUint8List().ToArray().SequenceEqual(data), "PNG decode/image repaint differs from snapshot pixels.");
        var asyncImage = scene.toImage(4,4).asTask(); Terminal(asyncImage); asyncImage.Result.Dispose();
        image.Dispose();
        Terminal(chained.toByteData().asTask());
        var released = 0;
        var empty = new Doroti.Ui.Image(1, 1, 1, () => released++); var emptyClone = empty.clone();
        empty.Dispose(); Require(released == 0, "Fallback image released while clone is live.");
        emptyClone.Dispose(); emptyClone.Dispose(); Require(released == 1, "Image resource released twice.");
        using var native = new Scene(tester.View.viewId, [new("platformView", null)]);
        try { native.toImageSync(1, 1); throw new Exception("Unsupported native snapshot admitted."); }
        catch (DorotiCapabilityException) { }
    }

    static void Cleanup()
    {
        var disposed = 0;
        var shared = new CleanupCapability(() => disposed++);
        var throwing = new CleanupCapability(() => throw new InvalidOperationException("cleanup"));
        var registry = new DorotiViewCapabilities("regression").Register<CleanupCapability>("a", throwing)
            .Register<CleanupCapability>("b", shared).Register<CleanupCapability>("c", shared);
        try { registry.Dispose(); throw new Exception("Cleanup failure not reported."); }
        catch (AggregateException) { }
        registry.Dispose(); Require(disposed == 1, "Distinct remaining capabilities were not disposed exactly once.");
        var entry = new FailingEntrypoint();
        var session = new DorotiHostSession(entry); session.Start();
        var first = new FailingHost(true); var second = new FailingHost(false);
        var firstView = session.dispatcher.RegisterView(1, new DorotiViewCapabilities("test")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, first));
        var secondView = session.dispatcher.RegisterView(2, new DorotiViewCapabilities("test")
            .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, second));
        session.AttachView(firstView); session.AttachView(secondView);
        var pending = new TaskCompletionSource<int>(); Task? waiting = null;
        firstView.DispatchPlatformEvent(() => waiting = Future<int>.fromTask(pending.Task).then((Func<int,int>)(value => value)).asTask());
        try { session.Shutdown(); throw new Exception("Host cleanup failures were not reported."); }
        catch (AggregateException) { }
        Canceled(waiting!); session.Dispose();
        Require(entry.Detached == 2 && entry.ShutDown == 1 && first.ClosedCount == 1 && second.ClosedCount == 1
            && first.Disposed == 1 && second.Disposed == 1, "One failing detach/Close skipped remaining owners.");
    }

    static void SnapshotWidgets()
    {
        using var tester = new WidgetTester(new Size(8, 8), 2);
        tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr, child: new RepaintBoundary(child: new ColoredBox(color: new Color(0xff00ff00)))));
        var boundary = (RenderRepaintBoundary)tester.byType<RepaintBoundary>().Single(element => ((RepaintBoundary)element.widget).child is ColoredBox).findRenderObject()!;
        using var image = boundary.toImageSync(2);
        Require(image.width == 16 && image.height == 16, "RepaintBoundary DPR not rasterized.");
        var bytes = image.toByteData().asTask(); Terminal(bytes);
        Require(bytes.Result!.buffer.asUint8List().ToArray()[1] == 255, "OffsetLayer consumer produced no green pixels.");
        var controller = new SnapshotController(true);
        tester.pumpWidget(new Directionality(textDirection: TextDirection.ltr, child: new SnapshotWidget(controller: controller, autoresize: true,
            child: new ColoredBox(color: new Color(0xffff0000)))));
        Require(tester.pixel(1, 1).Red == 255, "SnapshotWidget image repaint failed.");
        tester.setViewport(new Size(10, 6), 3); controller.clear(); tester.pump();
        Require(tester.pixel(1, 1).Red == 255, "SnapshotWidget DPR/resize invalidation failed.");
        tester.pumpWidget(new SizedBox()); controller.dispose();
    }

    static void AppleState()
    {
        var node = new SemanticsNodeUpdate(1, Rect.fromLTWH(0, 0, 1, 1), "check", null, SemanticsAction.tap, []);
        foreach (var (state, number, text) in new[] { (CheckedState.isFalse, 0, "Off"), (CheckedState.isTrue, 1, "On"), (CheckedState.mixed, 2, "Mixed"), (CheckedState.isFalse, 0, "Off") })
        {
            var update = node with { flags = new(isChecked: state) };
            Require(AppleSemanticsState.NumericValue(update) == number && AppleSemanticsState.TextValue(update, key => key) == text,
                "Null-valued Checkbox state lost in Apple projection.");
        }
        Require(AppleSemanticsState.TextValue(node with { value = "value", flags = new(isToggled: Tristate.isTrue) }, key => key) == "value, On", "Toggle state/text composition lost.");
        Require(AppleSemanticsState.TextValue(node with { flags = new(isObscured: true, isChecked: CheckedState.isTrue) }, key => key) is null, "Obscured value leaked.");
    }
    sealed class CleanupCapability(Action action) : IDisposable { public void Dispose() => action(); }
    sealed class FailingEntrypoint : IDorotiViewEntrypoint
    {
        public int Detached, ShutDown;
        public void Bootstrap(PlatformDispatcher dispatcher) { }
        public void AttachView(DorotiView view) { }
        public void DetachView(DorotiView view) { Detached++; if (Detached == 1) throw new InvalidOperationException("detach"); }
        public void Shutdown() { ShutDown++; }
    }
#pragma warning disable CS0067
    sealed class FailingHost(bool fail) : IViewHostCapability
    {
        public int ClosedCount, Disposed;
        public ViewMetrics Metrics => new(new Size(10, 10), 1, ViewPadding.zero, ViewPadding.zero, ViewPadding.zero, AppLifecycleState.resumed, 1, 1);
        public DorotiViewEpoch ViewEpoch => default!;
        public event Action<ViewMetrics>? MetricsChanged;
        public event Action<AppLifecycleState>? LifecycleChanged;
        public event Action? CloseRequested;
        public event Action? Closed;
        public void Show() { }
        public void Resize(Size size) { }
        public void Close() { ClosedCount++; if (fail) throw new InvalidOperationException("close"); }
        public void Dispose() { Disposed++; }
    }
#pragma warning restore CS0067
}
