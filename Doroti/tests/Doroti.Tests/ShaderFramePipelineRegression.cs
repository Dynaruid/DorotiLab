using System.Reflection;
using Doroti.Host.Maui;
using Doroti.Framework.Widgets;
using Doroti.Skia.Rendering;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;

internal static class ShaderFramePipelineRegression
{
    public static void Run()
    {
        foreach (var (serial, pipeline, presentation, expectedPipeline, expectedAsync) in new[]
        {
            ((string?)null, (string?)null, (string?)null, true, true),
            ("1", null, null, false, false),
            (null, "0", null, false, false),
            (null, null, "transaction", false, false),
            ("1", "1", null, false, false),
            (null, "1", "transaction", false, false),
            ("1", "0", "async", false, true),
            ("0", "1", "async", true, true),
        })
        {
            var options = IosFrameLoopOptions.Resolve(serial, pipeline, presentation);
            if (options.Pipeline != expectedPipeline || options.Asynchronous != expectedAsync)
                throw new Exception("iOS default/serial/async policy selection lost its override contract.");
        }
        using var tester = new WidgetTester(new Size(80, 60));
        var host = (ISkiaSceneRendererHost)typeof(WidgetTester)
            .GetField("_host", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(tester)!;
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "pipeline-test", "cpu", "cpu", false);
        long frameworkFrame = 0;
        void Submit(IReadOnlyList<SceneCommand> commands)
        {
            using var scene = new Scene(1, commands);
            renderer.Submit(1, new(scene, new(host.ViewEpoch, ++frameworkFrame, 80, 60)),
                DorotiUiInvocation.Managed("ShaderFramePipelineRegression"));
        }
        // Run the production host ordering with no scene submitted beforehand.
        var callbacks = new MauiFrameCallbackQueue();
        var callbackCount = 0;
        void Prepare()
        {
            callbacks.Take()?.Invoke(TimeSpan.Zero);
        }
        callbacks.TrySchedule(_ =>
        {
            callbackCount++;
            Submit([new("pushOffset", new object[] { 0d, 0d }), new("pop", null)]);
            callbacks.TrySchedule(__ => callbackCount++);
        });
        if (callbacks.TrySchedule(_ => throw new Exception("Duplicate callback")))
            throw new Exception("Framework requests did not coalesce.");
        if (renderer.CanRecordShaderSceneAhead) throw new Exception("Expected an initially empty pending scene.");
        var admission = IosFrameAdmissionPolicy.PrepareAndDecide(true, true, Prepare,
            () => renderer.ShaderSceneAdmission, () => 1, false, false, false);
        if (!admission.Admitted || !admission.FreshOnly || admission.Transaction || callbackCount != 1 || !callbacks.HasPending)
            throw new Exception("Callback-created scene did not enter the host pipeline or re-request was consumed twice.");
        var full = IosFrameAdmissionPolicy.PrepareAndDecide(true, true, Prepare,
            () => renderer.ShaderSceneAdmission, () => 2, false, false, false);
        if (full.Admitted || callbackCount != 2) throw new Exception("Two-frame cap blocked framework preparation or admitted a third frame.");
        foreach (var native in new[] { (true, false), (false, true) })
        {
            var fallback = IosFrameAdmissionPolicy.PrepareAndDecide(true, true, Prepare,
                () => SkiaShaderSceneAdmission.eligible, () => 1, native.Item1, native.Item2, false);
            if (fallback.Admitted || fallback.FreshOnly || !fallback.Transaction)
                throw new Exception("Native transition did not drain shader work first.");
        }
        foreach (var reason in new[] { SkiaShaderSceneAdmission.noNewScene, SkiaShaderSceneAdmission.nativeScene, SkiaShaderSceneAdmission.viewportMismatch })
        {
            var fallback = IosFrameAdmissionPolicy.PrepareAndDecide(true, true, Prepare, () => reason, () => 1, false, false, false);
            if (fallback.Admitted || fallback.FreshOnly) throw new Exception("Replay/native/stale scene recorded ahead.");
        }
        var resize = IosFrameAdmissionPolicy.PrepareAndDecide(true, true,
            () => throw new Exception("Rotation preparation bypassed its serial gate."),
            () => SkiaShaderSceneAdmission.eligible, () => 1, false, false, true);
        if (resize.Admitted || !resize.Transaction) throw new Exception("Resize did not keep serial transaction semantics.");
        var failedSubmission = new IosFrameLifetime();
        if (failedSubmission.CanRetire(false, false)) throw new Exception("Failed submission allowed early resource retirement.");
        failedSubmission.MarkTerminalCommitted();
        if (failedSubmission.CanRetire(false, false)) throw new Exception("Terminal commit/scheduling was mistaken for completion.");
        failedSubmission.MarkRetired(true, false);
        if (failedSubmission.CanRetire(true, false)) throw new Exception("Duplicate completion retired resources twice.");
        var lostDevice = new IosFrameLifetime();
        if (!lostDevice.CanRetire(false, true)) throw new Exception("Confirmed context loss did not allow retirement.");
        lostDevice.MarkRetired(false, true);
        var disposedCallbacks = new MauiFrameCallbackQueue();
        disposedCallbacks.TrySchedule(_ => throw new Exception("Disposed callback executed."));
        disposedCallbacks.Clear();
        if (disposedCallbacks.Take() is not null) throw new Exception("Shutdown retained a framework callback.");
        callbacks.TrySchedule(_ => Submit([new("inputShield", null)]));
        var callbackNative = IosFrameAdmissionPolicy.PrepareAndDecide(true, true, Prepare,
            () => renderer.ShaderSceneAdmission, () => 1, false, false, false);
        if (callbackNative.Admitted || callbackNative.FreshOnly || !renderer.Diagnostics.PendingScene)
            throw new Exception("Callback-created native scene was consumed before shader frames drained.");
        Submit([new("pushOffset", new object[] { 0d, 0d }), new("pop", null)]);
        ((IViewHostCapability)host).Resize(new Size(81, 60));
        if (renderer.ShaderSceneAdmission != SkiaShaderSceneAdmission.viewportMismatch)
            throw new Exception("A viewport generation change retained shader-ahead eligibility.");
        ((IViewHostCapability)host).Resize(new Size(80, 60));
        var slots = 2;
        var afterCompletion = IosFrameAdmissionPolicy.PrepareAndDecide(true, true, () => slots--,
            () => SkiaShaderSceneAdmission.eligible, () => slots, false, false, false);
        if (!afterCompletion.Admitted) throw new Exception("Admission used a slot count captured before framework preparation.");
        callbacks.TrySchedule(_ => throw new Exception("Raster wake ran a framework re-request in the same pulse."));
        var rasterWake = IosFrameAdmissionPolicy.PrepareAndDecide(true, true,
            () => callbacks.Take()?.Invoke(TimeSpan.Zero), () => SkiaShaderSceneAdmission.eligible,
            () => 1, false, false, false, prepareFramework: false);
        if (!rasterWake.Admitted || !rasterWake.FreshOnly || !callbacks.HasPending)
            throw new Exception("Prepared-scene wake failed admission or consumed the next pulse callback.");
        callbacks.Clear();
        var variable = new SceneCommand("backdropFilter", null)
        {
            HostPayload = new SceneBackdropFilterPayload(ImageFilterSnapshot.Capture(
                ImageFilter.variableBlur(new(0, 0), new(0, 40), resolutionScale: .25)), BlendMode.srcOver, null),
        };
        Submit([variable, new("pop", null)]);
        if (!renderer.CanRecordShaderSceneAhead) throw new Exception("A new shader scene was not available for pipelining.");
        // Nested native/shield content must stay pending for the serial path.
        var retained = new SceneCommand("retained", null) { HostPayload = new SceneRetainedPayload(
            [new("inputShield", null)], 1, 0) };
        Submit([variable, new("pop", null), retained]);
        if (renderer.CanRecordShaderSceneAhead) throw new Exception("Native composition was admitted ahead of GPU completion.");
        using var surface = SKSurface.Create(new SKImageInfo(80, 60));
        var rejected = renderer.PaintNewShaderScene(surface, 80, 60, host.ResizeTarget);
        if (rejected.Disposition != SkiaPaintDisposition.superseded || rejected.Completion is not null
            || !renderer.Diagnostics.PendingScene)
            throw new Exception("Fresh-only admission consumed a scene that needed native composition.");
        SceneCommand Picture(SKColor color, long identity)
        {
            var commands = new List<PathCommand>();
            var canvas = new Doroti.Ui.Canvas(commands);
            canvas.drawRect(new Rect(0, 0, 80, 60), new Doroti.Ui.Paint { color = new Color(color.Red == 255 ? 0xffff0000 : 0xff00ff00) });
            return new("picture", null) { HostPayload = new ScenePicturePayload(identity, new(0, 0), commands, null, false, true) };
        }
        Submit([Picture(SKColors.Red, 6001)]);
        var older = renderer.Paint(surface, 80, 60, host.ResizeTarget).Completion ?? throw new Exception("Older paint missing.");
        Submit([Picture(SKColors.Lime, 6002)]);
        var newer = renderer.Paint(surface, 80, 60, host.ResizeTarget).Completion ?? throw new Exception("Newer paint missing.");
        renderer.CompletePaint(newer);
        renderer.CompletePaint(older);
        if (renderer.CanRecordShaderSceneAhead) throw new Exception("A replay was reported as a fresh shader scene.");
        var noFresh = renderer.PaintNewShaderScene(surface, 80, 60, host.ResizeTarget);
        if (noFresh.Completion is not null || noFresh.Disposition != SkiaPaintDisposition.superseded)
            throw new Exception("Fresh-only recording allowed a retained replay.");
        var replay = renderer.Paint(surface, 80, 60, host.ResizeTarget);
        using var image = surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(image);
        if (bitmap.GetPixel(10, 10) != SKColors.Lime || replay.Disposition != SkiaPaintDisposition.replay)
            throw new Exception("An older GPU completion replaced the newest replay source.");
        Console.WriteLine("PASS: fresh shader admission; native/replay deferral; out-of-order completion keeps the newest scene (CPU lifecycle contract).");
    }
}
