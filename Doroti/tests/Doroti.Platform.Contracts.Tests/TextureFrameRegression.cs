using Doroti.Host.WindowsAppSdk;
using Doroti.Skia.Rendering;
using Doroti.Testing;
using Doroti.Ui;
using SkiaSharp;

static class TextureFrameRegression
{
    private static readonly DorotiUiInvocation Invocation = new("texture-frame-regression");

    public static void Run()
    {
        ProducerOnlyFrames();
        InitiallyEmptyTexture();
        FrozenAndLiveInstances();
        ExternalFrameSnapshots();
        InputAfterPresentation();
        Console.WriteLine("PASS: producer-only CPU/external texture replay after input, stale new-scene rejection, latest-frame coalescing, retained layers/filters, freeze/resume, unregister and immutable raster leases (CPU; native presentation/GPU fences are separate).");
    }

    private static void Submit(SkiaSceneRenderer renderer, TestHost host, long id, long number, bool freeze = false)
    {
        // Exercise a texture below both a retained subtree and an image filter.
        var subtree = new SceneBuilder(1);
        var layer = subtree.pushOpacity(255);
        subtree.pushImageFilter(new ImageFilter(sigmaX: 0, sigmaY: 0));
        subtree.addTexture(id, width: 24, height: 24, freeze: freeze);
        subtree.pop();
        subtree.pop();
        using var child = subtree.build();
        var builder = new SceneBuilder(1);
        builder.addRetained(layer);
        using var scene = builder.build();
        using var submission = new DorotiSceneSubmission(scene, new(host.ViewEpoch, number, 24, 24));
        renderer.Submit(1, submission, Invocation);
    }

    private static SkiaPaintCompletion Paint(SkiaSceneRenderer renderer, SKSurface surface, SKColor expected)
    {
        var result = renderer.Paint(surface, 24, 24) ?? throw new Exception("Texture paint was not admitted.");
        renderer.CompletePaint(result);
        using var image = surface.Snapshot();
        using var pixels = SKBitmap.FromImage(image);
        Check.True(pixels.GetPixel(12, 12) == expected,
            $"Expected texture {expected}, got {pixels.GetPixel(12, 12)} (new scene: {result.IsNewFrame}).");
        return result;
    }

    private static void ProducerOnlyFrames()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null, "texture-replay", "cpu", "cpu", false);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        using var entry = renderer.Textures.CreateTexture();
        entry.PushFrame([255, 0, 0, 255], 1, 1);
        Submit(renderer, host, entry.Id, 1);
        entry.PushFrame([0, 0, 255, 255], 1, 1);
        Check.True(Paint(renderer, surface, SKColors.Red).IsNewFrame,
            "A queued scene lost its original texture snapshot.");
        var accepted = renderer.Diagnostics.SceneAccepted;
        var revision = renderer.TextureRevision;
        Check.True(!Paint(renderer, surface, SKColors.Blue).IsNewFrame,
            "A producer-only update required a replacement scene.");
        entry.PushFrame([0, 255, 0, 255], 1, 1);
        Task.Run(() => entry.PushFrame([255, 255, 0, 255], 1, 1)).GetAwaiter().GetResult();
        Paint(renderer, surface, SKColors.Yellow);
        Check.True(renderer.Diagnostics.SceneAccepted == accepted && renderer.TextureRevision == revision + 2,
            "Texture replay rebuilt a scene or lost producer revisions.");

        Submit(renderer, host, entry.Id, 2, freeze: true);
        Paint(renderer, surface, SKColors.Yellow);
        entry.PushFrame([0, 255, 0, 255], 1, 1);
        Paint(renderer, surface, SKColors.Yellow);
        entry.PushFrame([0, 0, 255, 255], 1, 1);
        Paint(renderer, surface, SKColors.Yellow);
        Submit(renderer, host, entry.Id, 3);
        Paint(renderer, surface, SKColors.Blue);
        entry.PushFrame([255, 0, 0, 255], 1, 1);
        Paint(renderer, surface, SKColors.Red);
        entry.Dispose();
        Paint(renderer, surface, SKColors.White);
    }

    private static void InitiallyEmptyTexture()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null, "empty-texture-replay", "cpu", "cpu", false);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        using var entry = renderer.Textures.CreateTexture();
        Submit(renderer, host, entry.Id, 1);
        Paint(renderer, surface, SKColors.White);
        entry.PushFrame([0, 0, 255, 255], 1, 1);
        Paint(renderer, surface, SKColors.Blue);
        Check.True(renderer.Diagnostics.SceneAccepted == 1, "The first producer frame submitted another scene.");
    }

    private static void ExternalFrameSnapshots()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null, "external-texture-replay", "cpu", "cpu", false);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        var source = new ExternalSource();
        using var entry = renderer.RegisterExternalTexture(source);
        Submit(renderer, host, entry.Id, 1);
        Paint(renderer, surface, SKColors.Red);
        source.Color = SKColors.Blue;
        entry.MarkFrameAvailable();
        source.OnDraw = () =>
        {
            source.Color = SKColors.Green;
            entry.MarkFrameAvailable();
        };
        Paint(renderer, surface, SKColors.Blue);
        Check.True(source.Captured == 2 && source.Released == 1,
            "External replay leaked its temporary frame or re-captured during drawing.");
        source.OnDraw = null;
        Paint(renderer, surface, SKColors.Green);
        Check.True(source.Captured == 3 && source.Released == 2 && renderer.Diagnostics.SceneAccepted == 1,
            "External producer-only replay did not use immutable per-raster frames.");
        entry.Dispose();
        Paint(renderer, surface, SKColors.White);
        renderer.Dispose();
        Check.True(source.Disposed && source.Released == source.Captured,
            "Unregister/renderer shutdown leaked an external frame snapshot.");
    }

    private static void FrozenAndLiveInstances()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null, "mixed-texture-freeze", "cpu", "cpu", false);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        using var entry = renderer.Textures.CreateTexture();
        entry.PushFrame([255, 0, 0, 255], 1, 1);
        var builder = new SceneBuilder(1);
        builder.addTexture(entry.Id, width: 12, height: 24, freeze: true);
        builder.addTexture(entry.Id, offset: new Offset(12, 0), width: 12, height: 24);
        using var scene = builder.build();
        using var submission = new DorotiSceneSubmission(scene, new(host.ViewEpoch, 1, 24, 24));
        renderer.Submit(1, submission, Invocation);
        renderer.CompletePaint(renderer.Paint(surface, 24, 24)!.Value);
        entry.PushFrame([0, 0, 255, 255], 1, 1);
        var replay = renderer.Paint(surface, 24, 24)!.Value;
        renderer.CompletePaint(replay);
        using var image = surface.Snapshot();
        using var pixels = SKBitmap.FromImage(image);
        Check.True(!replay.IsNewFrame && pixels.GetPixel(6, 12) == SKColors.Red
            && pixels.GetPixel(18, 12) == SKColors.Blue && renderer.Diagnostics.SceneAccepted == 1,
            "Refreshing a live instance changed a frozen instance of the same texture.");
    }

    private static void InputAfterPresentation()
    {
        using var host = new TestHost(new Size(24, 24), 1);
        using var renderer = new SkiaSceneRenderer(1, host, new Color(0xffffffff), null, "texture-after-input", "cpu", "cpu", false);
        using var surface = SKSurface.Create(new SKImageInfo(24, 24))!;
        using var entry = renderer.Textures.CreateTexture();
        var input = new WindowsFrameInputState();
        entry.PushFrame([255, 0, 0, 255], 1, 1);
        Submit(renderer, host, entry.Id, 1);
        Paint(renderer, surface, SKColors.Red);
        // Focus/hover/click processing can consume input without repainting any
        // widget. The displayed scene still carries its original input stamp.
        input.Dispatch(input.Receive());
        entry.PushFrame([0, 0, 255, 255], 1, 1);
        var replay = renderer.Paint(surface, 24, 24)!.Value;
        Check.True(!replay.IsNewFrame && input.CanPresent(replay),
            "Input without a replacement scene permanently blocked texture replay.");
        renderer.CompletePaint(replay);
        Check.True(!input.CanPresent(replay with { IsNewFrame = true }),
            "Allowing texture replay also admitted a stale replacement scene.");
        var current = replay with { IsNewFrame = true, InputSequence = input.Dispatched };
        Check.True(input.CanPresent(current), "A scene built after dispatched input was rejected.");
        input.Receive(); // Queued input belongs to the next framework frame.
        Check.True(input.CanPresent(current) && !input.IsLatestReceived(current.InputSequence),
            "Queued input starved presentation or weakened prepared-buffer admission.");
    }

    private sealed class ExternalSource : ISkiaExternalTextureSource
    {
        internal SKColor Color = SKColors.Red;
        internal int Captured, Released;
        internal bool Disposed;
        internal Action? OnDraw;
        public SkiaExternalTextureFrame CaptureFrame(bool freeze)
        {
            ObjectDisposedException.ThrowIf(Disposed, this);
            Captured++;
            return new Frame(this, Color);
        }
        public void Draw(SKCanvas canvas, SKRect destination, SKSamplingOptions sampling, bool freeze) =>
            throw new Exception("A mutable source crossed the immutable raster boundary.");
        public void Dispose() => Disposed = true;

        private sealed class Frame(ExternalSource source, SKColor color) : SkiaExternalTextureFrame
        {
            private bool _disposed;
            public override void Draw(SKCanvas canvas, SKRect destination, SKSamplingOptions sampling)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                source.OnDraw?.Invoke();
                using var paint = new SKPaint { Color = color };
                canvas.DrawRect(destination, paint);
            }
            public override void Dispose()
            {
                Check.True(!_disposed, "An external frame snapshot was released twice.");
                _disposed = true;
                source.Released++;
            }
        }
    }
}
