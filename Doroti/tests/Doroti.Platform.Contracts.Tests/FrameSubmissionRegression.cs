using Doroti.Skia.Rendering;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;

static class FrameSubmissionRegression
{
    private static readonly DorotiUiInvocation Invocation = new("frame-submission-contract");

    static Scene PictureScene(ulong viewId, IReadOnlyList<PathCommand> commands)
    {
        var builder = new SceneBuilder(viewId);
        builder.addPicture(Offset.zero, commands);
        return builder.build();
    }

    static Scene ImageScene(ulong viewId, Image image)
    {
        var commands = new List<PathCommand>();
        var canvas = new Canvas(commands);
        canvas.drawImage(image, Offset.zero, new Paint());
        return PictureScene(viewId, commands);
    }

    static void FrozenOwnership()
    {
        var releases = 0;
        using var image = new Image(1, 1, 1, () => releases++);
        using var scene = ImageScene(1, image);
        using var submission = new DorotiSceneSubmission(scene, null);
        var firstConsumer = new object();
        var lease = submission.Scene.RetainForConsumer(firstConsumer);
        Check.Throws<InvalidOperationException>(() => submission.Scene.RetainForConsumer(new object()));
        using (var crossView = ImageScene(2, image))
            Check.Throws<InvalidOperationException>(() => new DorotiSceneSubmission(crossView, null));
        image.Dispose(); scene.Dispose(); submission.Dispose();
        Check.True(releases == 0, "Producer disposal released a consumer's image storage.");
        lease.Dispose(); lease.Dispose();
        Check.True(releases == 1, "The final frozen resource lease did not release exactly once.");
        Check.Throws<ObjectDisposedException>(() => submission.Scene.Retain());
    }

    static void RendererQueueAndCompletion()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "frame-contract", "cpu", "cpu", false);
        // This lifecycle probe has no backend image handle. Use the public
        // native-scene painter seam; pixel rendering is checked separately below.
        renderer.PlatformScenePainter = (canvas, commands, descriptor, width, height) => canvas.Clear(SKColors.Red);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        long frame = 0;
        var released = new int[6];
        void SubmitPinned(int index)
        {
            using var image = new Image(1, 1, 1, () => released[index]++);
            using var scene = ImageScene(1, image);
            using var submission = new DorotiSceneSubmission(scene, new(host.ViewEpoch, ++frame, 24, 24));
            renderer.Submit(1, submission, Invocation);
        }
        // Replacing queued snapshots must retire the replaced producer leases.
        SubmitPinned(0); SubmitPinned(1);
        Check.True(released[0] == 1 && released[1] == 0, "Latest-pending replacement leaked or released the admitted scene.");
        var first = renderer.Paint(surface, 24, 24)!.Value;
        Check.True(released[1] == 0, "Raster start prematurely released frame resources.");
        SubmitPinned(2);
        var second = renderer.Paint(surface, 24, 24)!.Value;
        SubmitPinned(3);
        Check.True(renderer.Paint(surface, 24, 24) is null && released[3] == 0,
            "A third unfinished consumer was admitted or its pending lease was released.");
        SubmitPinned(4);
        Check.True(released[3] == 1 && released[1] == 0 && released[2] == 0,
            "Pending replacement released in-flight consumers.");
        renderer.CompletePaint(second);
        renderer.CompletePaint(first); // Late completion cannot replace the newer replay.
        Check.True(released[1] == 1 && released[2] == 0, "Out-of-order completion damaged the replay lease.");
        var latest = renderer.Paint(surface, 24, 24)!.Value;
        renderer.CompletePaint(latest);
        Check.True(released[2] == 1 && released[4] == 0, "Replay replacement did not release its old source.");
        SubmitPinned(5);
        var failed = renderer.Paint(surface, 24, 24)!.Value;
        renderer.FailOutstandingGpuPaints("injected device loss after owned consumer retirement");
        Check.True(released[5] == 1, "Failed outstanding raster kept its producer lease.");
        renderer.FailPaint(failed, "late device-loss callback");
        renderer.Dispose();
        Check.True(released.All(count => count == 1), "Renderer close did not release every admitted/rejected source exactly once.");
    }

    static void MutableCommandsAndOwner()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "frame-freeze", "cpu", "cpu", false);
        var translation = new double[] { 0, 0 };
        var commands = new List<PathCommand> { new("translate", translation) };
        var canvas = new Canvas(commands);
        canvas.drawRect(Rect.fromLTWH(0, 0, 12, 12), new Paint { color = new Color(0xffff0000) });
        using var scene = PictureScene(1, commands);
        using var submission = new DorotiSceneSubmission(scene, new(host.ViewEpoch, 1, 24, 24));
        translation[0] = 100; commands.Clear(); scene.Dispose();
        renderer.Submit(1, submission, Invocation);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        var completion = renderer.Paint(surface, 24, 24)!.Value;
        renderer.CompletePaint(completion);
        using var pixels = surface.Snapshot();
        using var bitmap = SKBitmap.FromImage(pixels);
        Check.True(bitmap.GetPixel(6, 6).Red == 255, "Mutable producer command arguments crossed the freeze boundary.");
        using var liveScene = PictureScene(1, []);
        Check.Throws<InvalidOperationException>(() => Task.Run(() => new DorotiSceneSubmission(liveScene, null)).GetAwaiter().GetResult());
    }

    public static void Run()
    {
        FrozenOwnership();
        RendererQueueAndCompletion();
        MutableCommandsAndOwner();
        ReplayCompletionOwnership();
        TextureSnapshots();
        ViewGenerations();
        Console.WriteLine("PASS: frozen command/image storage, consumer ownership, bounded pending/raster admission, late completion, renderer close and simulated device-loss cleanup (CPU; native GPU fence acceptance remains separate).");
    }

    static void ReplayCompletionOwnership()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "replay-completion", "cpu", "cpu", false);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        var released = new int[2];
        void Submit(int index)
        {
            using var image = new Image(1, 1, 1, () => released[index]++);
            using var scene = ImageScene(1, image);
            using var submission = new DorotiSceneSubmission(scene, new(host.ViewEpoch, index + 1, 24, 24));
            renderer.Submit(1, submission, Invocation);
        }
        renderer.PlatformScenePainter = (canvas, commands, descriptor, width, height) => canvas.Clear(SKColors.Red);
        Submit(0);
        renderer.CompletePaint(renderer.Paint(surface, 24, 24)!.Value);
        Submit(1);
        var pending = renderer.Paint(surface, 24, 24)!.Value;
        renderer.PlatformScenePainter = (canvas, commands, descriptor, width, height) =>
        {
            // The GPU/owner completion queue can replace the replay source while
            // this raster owner is still using its image and command storage.
            Task.Run(() => renderer.CompletePaint(pending)).GetAwaiter().GetResult();
            Check.True(released[0] == 0, "A completion released resources still used by the active replay raster.");
            canvas.Clear(SKColors.Blue);
        };
        var replay = renderer.Paint(surface, 24, 24)!.Value;
        Check.True(!replay.IsNewFrame && released[0] == 1, "The replay raster did not release its old source on completion.");
        renderer.CompletePaint(replay);
        renderer.Dispose();
        Check.True(released.All(count => count == 1), "Replay completion leaked or duplicated an image release.");
    }

    static void ViewGenerations()
    {
        using var dispatcher = new PlatformDispatcher();
        using var scope = dispatcher.EnterScope();
        DorotiView Register(PlatformDispatcher owner, out TestHost host, out SkiaSceneRenderer renderer)
        {
            host = new(new Size(24, 24), 1);
            renderer = new(1, host, null, null, "generation-contract", "cpu", "cpu", false);
            return owner.RegisterView(1, new DorotiViewCapabilities("generation-contract")
                .Register<IViewHostCapability>(DorotiCapabilityIds.ViewLifecycleMetrics, host)
                .Register<ISceneHostCapability>(DorotiCapabilityIds.GraphicsScene, renderer));
        }
        using var first = Register(dispatcher, out var firstHost, out _);
        using var source = PictureScene(1, []);
        using var old = new DorotiSceneSubmission(source, new(firstHost.ViewEpoch, 1, 24, 24, first.SceneOwner));
        first.Dispose();
        using var replacement = Register(dispatcher, out var replacementHost, out var replacementRenderer);
        Check.True(replacement.SceneOwner.Generation > first.SceneOwner.Generation,
            "Reusing a view ID reused the prior incarnation.");
        Check.Throws<DorotiCapabilityException>(() => replacementRenderer.Submit(1, old, Invocation));
        Check.Throws<InvalidOperationException>(() => new DorotiSceneSubmission(source, old.BuildToken));
        using (replacement.EnterInvocationScope())
        using (var currentSource = PictureScene(1, []))
        using (var current = new DorotiSceneSubmission(currentSource, new(replacementHost.ViewEpoch, 1, 24, 24, replacement.SceneOwner)))
            replacementRenderer.Submit(1, current, Invocation);
        using var restarted = new PlatformDispatcher();
        using var restartedScope = restarted.EnterScope();
        using var restartedView = Register(restarted, out _, out var restartedRenderer);
        Check.True(restartedView.SceneOwner.ApplicationId != first.SceneOwner.ApplicationId,
            "A new application reused the old application identity.");
        Check.Throws<DorotiCapabilityException>(() => restartedRenderer.Submit(1, old, Invocation));
    }

    sealed record TestBuffer() : NativeTextureBuffer(1, 1, NativeTextureFormat.Rgba8888)
    {
        public override NativeTexturePlatform Platform => NativeTexturePlatform.Linux;
    }

    static void TextureSnapshots()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, null, null, "texture-freeze", "cpu", "cpu", false);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        using var entry = renderer.Textures.CreateTexture();
        entry.PushFrame([255, 0, 0, 255], 1, 1);
        var builder = new SceneBuilder(1);
        builder.addTexture(entry.Id, width: 24, height: 24);
        using var scene = builder.build();
        using var submission = new DorotiSceneSubmission(scene, new(host.ViewEpoch, 1, 24, 24));
        renderer.Submit(1, submission, Invocation);
        entry.PushFrame([0, 0, 255, 255], 1, 1); entry.Dispose();
        var completion = renderer.Paint(surface, 24, 24)!.Value;
        renderer.CompletePaint(completion);
        using (var image = surface.Snapshot())
        using (var pixels = SKBitmap.FromImage(image))
            Check.True(pixels.GetPixel(12, 12).Red == 255 && pixels.GetPixel(12, 12).Blue == 0,
                "A queued texture observed newer producer pixels or registration disposal.");

        renderer.EnableNativeTextures(NativeTexturePlatform.Linux);
        using var native = renderer.Textures.CreateNativeTexture();
        var releases = 0;
        using var producer = new NativeTextureFrame(new TestBuffer(), () => releases++);
        native.PushFrame(producer);
        using var otherHost = new TestHost(new Size(24, 24), 1, viewId: 2);
        using var other = new SkiaSceneRenderer(2, otherHost, null, null, "other-texture-owner", "cpu", "cpu", false);
        other.EnableNativeTextures(NativeTexturePlatform.Linux);
        using var otherEntry = other.Textures.CreateNativeTexture();
        Check.Throws<InvalidOperationException>(() => otherEntry.PushFrame(producer));
        var nativeBuilder = new SceneBuilder(1);
        nativeBuilder.addTexture(native.Id, width: 24, height: 24);
        using var nativeScene = nativeBuilder.build();
        using var nativeSubmission = new DorotiSceneSubmission(nativeScene, new(host.ViewEpoch, 2, 24, 24));
        renderer.Submit(1, nativeSubmission, Invocation);
        producer.Dispose(); native.Dispose(); nativeSubmission.Dispose();
        Check.True(releases == 0, "Removing native registration released a queued native allocation.");
        renderer.Dispose();
        Check.True(releases == 1, "Close did not release the queued native allocation exactly once.");
    }
}
