using Doroti.Skia.Rendering;
using Doroti.Skia.Vulkan;
using Doroti.Ui;
using Silk.NET.Vulkan;
using SkiaSharp;

const uint foreign = uint.MaxValue - 2;
var journal = new SubmissionJournal();
journal.Register(1, ImageLayout.General, foreign, 3);
journal.Allocate(10, 20);
Transition(ImageLayout.General, ImageLayout.ShaderReadOnlyOptimal, foreign, 3);
journal.Submit([10], Result.Success);
Check("external acquire records renderer ownership", journal.Images[1].Family == 3);
Transition(ImageLayout.ShaderReadOnlyOptimal, ImageLayout.General, 3, foreign);
journal.Submit([10], Result.ErrorOutOfDeviceMemory);
Check("failed release submission preserves ownership", journal.Images[1].Family == 3);
journal.Submit([10], Result.Success);
Check("successful release records foreign ownership", journal.Images[1].Family == foreign);
Transition(ImageLayout.General, ImageLayout.ShaderReadOnlyOptimal, foreign, 4);
Reject("other renderer queue rejected", () => journal.Submit([10], Result.Success));
Transition(ImageLayout.General, ImageLayout.ShaderReadOnlyOptimal, 3, foreign);
Reject("stale source ownership rejected", () => journal.Submit([10], Result.Success));
Transition(ImageLayout.General, ImageLayout.ShaderReadOnlyOptimal, foreign, Vk.QueueFamilyIgnored);
Reject("partial ownership descriptor rejected", () => journal.Submit([10], Result.Success));
journal.Register(2, ImageLayout.General, 3);
journal.Begin(10);
journal.Barrier(10, 2, ImageLayout.General, ImageLayout.General, 3, foreign);
journal.End(10);
Reject(
    "ordinary Graphite target still forbids foreign transfer",
    () => journal.Submit([10], Result.Success)
);
Check(
    "rejected submissions leave both images unchanged",
    journal.Images[1].Family == foreign && journal.Images[2].Family == 3
);
Console.WriteLine("PASS 8 imported image ownership contracts");

var host = new Host();
using var renderer = new SkiaSceneRenderer(
    1,
    host,
    null,
    null,
    "native-contract",
    "native-contract",
    "native-contract"
);
var source = new Source();
using var registration = renderer.RegisterExternalTexture(source);
registration.MarkFrameAvailable();
Check("native source wakes raster", host.Invalidations == 1);
using var surface = SKSurface.Create(new SKImageInfo(8, 8));
var builder = new SceneBuilder(1);
builder.addTexture(registration.Id, width: 8, height: 8, freeze: true);
using var scene = builder.build();
renderer.DrawPlatformRasterSegment(surface.Canvas, scene.Commands, 8, 8);
Check("texture resolves native adapter and forwards freeze", source.Draws == 1 && source.Frozen);
registration.Dispose();
registration.Dispose();
Check("native registration disposes source once", source.Disposals == 1);
var invalidations = host.Invalidations;
registration.MarkFrameAvailable();
renderer.DrawPlatformRasterSegment(surface.Canvas, scene.Commands, 8, 8);
Check(
    "stale native IDs and notifications are inert",
    source.Draws == 1 && host.Invalidations == invalidations
);
var secondSource = new Source();
using var second = renderer.RegisterExternalTexture(secondSource);
Check("native IDs are not reused", second.Id != registration.Id);
renderer.Dispose();
Check("renderer disposal closes native sources", secondSource.Disposals == 1);
Console.WriteLine("PASS 6 native registration contracts");

// Windows external queue family is distinct from Android/Linux foreign ownership.
const uint external = uint.MaxValue - 1;
journal.Register(3, ImageLayout.General, external, 3);
journal.Begin(10);
journal.Barrier(10, 3, ImageLayout.General, ImageLayout.ShaderReadOnlyOptimal, external, 3);
journal.End(10);
journal.Submit([10], Result.Success);
Check("Windows external family acquires local ownership", journal.Images[3].Family == 3);
journal.Begin(10);
journal.Barrier(10, 3, ImageLayout.ShaderReadOnlyOptimal, ImageLayout.General, 3, foreign);
journal.End(10);
Reject("import cannot switch external family domains", () => journal.Submit([10], Result.Success));
journal.Begin(10);
journal.Barrier(10, 3, ImageLayout.ShaderReadOnlyOptimal, ImageLayout.General, 3, external);
journal.End(10);
journal.Submit([10], Result.Success);
Check("Windows external family returned", journal.Images[3].Family == external);

var releases = 0;
var consumer = new object();
var owned = new NativeTextureFrame(new TestBuffer(), () => Interlocked.Increment(ref releases));
var retained = owned.RetainForConsumer(consumer);
owned.Dispose();
owned.Dispose();
Check("native lease keeps producer allocation alive", releases == 0);
var parallel = retained.RetainForConsumer(consumer);
try
{
    using var wrong = parallel.RetainForConsumer(new object());
    throw new Exception("cross-view accepted");
}
catch (InvalidOperationException)
{
    Check("native frame rejects simultaneous cross-view ownership", true);
}
Parallel.Invoke(retained.Dispose, parallel.Dispose);
Check("concurrent final release occurs exactly once", releases == 1);
try
{
    using var late = retained.Retain();
    throw new Exception("disposed retain accepted");
}
catch (ObjectDisposedException)
{
    Check("disposed native handle cannot resurrect storage", true);
}
using var nativeRenderer = new SkiaSceneRenderer(
    2,
    new Host(),
    null,
    null,
    "native",
    "native",
    "native"
);
nativeRenderer.EnableNativeTextures(NativeTexturePlatform.Windows);
using var nativeEntry = nativeRenderer.Textures.CreateNativeTexture();
var dropped = 0;
var newest = 0;
using (var first = new NativeTextureFrame(new TestBuffer(), () => dropped++))
    nativeEntry.PushFrame(first);
Check("entry retains pending buffer", dropped == 0);
var revision = nativeRenderer.TextureRevision;
using (var latest = new NativeTextureFrame(new TestBuffer(), () => newest++))
    nativeEntry.PushFrame(latest);
Check("latest frame replaces and releases pending buffer", dropped == 1 && newest == 0);
Check(
    "producer advances raster revision without widget rebuild",
    nativeRenderer.TextureRevision > revision
);
nativeEntry.Dispose();
Check("entry disposal returns pending producer allocation", newest == 1);
Console.WriteLine("PASS 11 cross-platform native ownership contracts");

void Transition(ImageLayout before, ImageLayout after, uint source, uint destination)
{
    journal.Begin(10);
    journal.Barrier(10, 1, before, after, source, destination);
    journal.End(10);
}
static void Check(string name, bool value)
{
    if (!value)
        throw new InvalidOperationException(name);
    Console.WriteLine("PASS " + name);
}
static void Reject(string name, Action action)
{
    try
    {
        action();
    }
    catch (NotSupportedException)
    {
        Check(name, true);
        return;
    }
    throw new InvalidOperationException(name);
}

sealed class Source : ISkiaExternalTextureSource
{
    public int Draws,
        Disposals;
    public bool Frozen;

    public void Draw(SKCanvas canvas, SKRect destination, SKSamplingOptions sampling, bool freeze)
    {
        Draws++;
        Frozen = freeze;
        using var paint = new SKPaint { Color = SKColors.Red };
        canvas.DrawRect(destination, paint);
    }

    public void Dispose() => Disposals++;
}

sealed class Host : ISkiaSceneRendererHost
{
    public int Invalidations;
    public long InputSequence => 0;
    public long SurfaceGeneration => 1;
    public DorotiViewEpoch ViewEpoch => new(1, 1, 1, 8, 8, 8, 8, 1, 1, 1);
    public DorotiResizeEpoch ResizeTarget => new(1, 8, 8, 8, 8, 1, 1);
    public PlatformConfiguration Configuration => new([], Brightness.light, false, false);
    public event Action<int, SemanticsAction, object?>? SemanticsAction
    {
        add { }
        remove { }
    }
    public event Action<long, TimeSpan>? InputReceived
    {
        add { }
        remove { }
    }
    public event Action<PlatformConfiguration>? ConfigurationChanged
    {
        add { }
        remove { }
    }

    public void UpdateSemantics(SemanticsUpdate update) { }

    public void ClearSemantics() { }

    public void RequestInvalidate() => Invalidations++;
}

sealed record TestBuffer() : NativeTextureBuffer(8, 8, NativeTextureFormat.Bgra8888)
{
    public override NativeTexturePlatform Platform => NativeTexturePlatform.Windows;
}
