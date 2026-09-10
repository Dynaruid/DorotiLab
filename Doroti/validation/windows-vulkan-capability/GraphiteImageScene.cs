using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

namespace Doroti.Validation.WindowsVulkanCapability;

// Uses the product decoder, scene replay and raster cache with the sample's
// embedded WebP. The Vulkan probe compares its output to the CPU reference.
internal sealed class GraphiteImageScene : ISkiaSceneRendererHost, IDisposable
{
    private readonly SkiaSceneRenderer _renderer;
    private readonly Doroti.Ui.Image _image;
    private readonly Picture _picture;
    private readonly Scene _scene;
    private long _frame;
    internal SKBitmap Reference { get; }
    internal int Promotions => (int)_renderer.Diagnostics.PictureRasterCacheMisses;
    public GraphiteImageScene(int size)
    {
        ResizeTarget = new(1, size, size, size, size, 1, 0);
        ViewEpoch = new(91, 1, 1, size, size, size, size, 1, 1, 0);
        _renderer = new(91, this, new Color(0xff000000), null, "image-regression", "image-regression", "image-regression");
        using var stream = typeof(GraphiteImageScene).Assembly.GetManifestResourceStream("sample.webp")!;
        using var bytes = new MemoryStream();
        stream.CopyTo(bytes);
        _image = _renderer.DecodeSizedAsync(bytes.ToArray(), static (_, _) => new TargetImageSize(32, 32), false,
            DartUiInvocation.Managed("graphite-image-regression")).GetAwaiter().GetResult();
        var recorder = new PictureRecorder();
        new Canvas(recorder).drawImageRect(_image, Rect.fromLTWH(0, 0, 32, 32), Rect.fromLTWH(0, 16, 32, 32),
            new Paint { filterQuality = FilterQuality.medium });
        _picture = recorder.endRecording();
        var builder = new SceneBuilder(91);
        builder.addPicture(Offset.zero, _picture, Rect.fromLTWH(0, 16, 32, 32), isComplexHint: true);
        _scene = builder.build();
        using var reference = _renderer.RasterizeAsync(_picture, size, size,
            DartUiInvocation.Managed("graphite-image-reference")).GetAwaiter().GetResult();
        var rgba = reference.toByteData().GetAwaiter().GetResult()!.asMemory().ToArray();
        Reference = new(new SKImageInfo(size, size, SKColorType.Rgba8888, SKAlphaType.Premul));
        System.Runtime.InteropServices.Marshal.Copy(rgba, 0, Reference.GetPixels(), rgba.Length);
    }

    internal SkiaPaintCompletion Draw(SKSurface surface)
    {
        _renderer.Submit(91, new(_scene, new(ViewEpoch, ++_frame, ViewEpoch.PhysicalWidth, ViewEpoch.PhysicalHeight)),
            DartUiInvocation.Managed("graphite-image-scene"));
        return _renderer.Paint(surface, ViewEpoch.PhysicalWidth, ViewEpoch.PhysicalHeight, ResizeTarget).Completion
            ?? throw new InvalidOperationException("Image scene did not paint.");
    }
    internal void Cancel(SkiaPaintCompletion completion) => _renderer.SupersedePaint(completion, "cancelled image regression");
    internal void Complete(SkiaPaintCompletion completion) => _renderer.CompletePaint(completion, DorotiFrameTerminal.submitted);
    public void Dispose() { _renderer.Dispose(); _scene.Dispose(); _picture.Dispose(); _image.Dispose(); Reference.Dispose(); }
    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiViewEpoch ViewEpoch { get; }
    public DorotiResizeEpoch ResizeTarget { get; }
    public PlatformConfiguration Configuration { get; } = new([new Locale("en", "US")], Brightness.light, false, false, HostOperatingSystem.windows);
    public event Action<int, SemanticsAction, object?>? SemanticsAction { add { } remove { } }
    public event Action<long, TimeSpan>? InputReceived { add { } remove { } }
    public event Action<PlatformConfiguration>? ConfigurationChanged { add { } remove { } }
    public void UpdateSemantics(SemanticsUpdate update) { }
    public void ClearSemantics() { }
    public void RequestInvalidate() { }
}
