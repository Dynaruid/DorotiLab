using Doroti.Skia.Rendering;
using Doroti.Ui;
using SkiaSharp;

var host = new Host();
using var renderer = Renderer(host);
using var surface = SKSurface.Create(new SKImageInfo(8, 8));
using var entry = renderer.Textures.CreateTexture();
using var scene = Scene(entry.Id);
Check("empty registration is blank", Pixel(scene).Alpha == 0);
var red = new byte[] { 255, 0, 0, 255 };
entry.PushFrame(red, 1, 1);
Array.Clear(red);
Check("frame publication requests raster", host.Invalidations == 1);
Check("producer buffer is copied and scaled", Pixel(scene) == SKColors.Red);
entry.PushFrame([0, 0, 255, 255], 1, 1);
using var frozen = Scene(entry.Id, freeze: true);
Check("freeze holds previously acquired frame", Pixel(frozen) == SKColors.Red);
entry.PushFrame([0, 255, 0, 255], 1, 1);
Check("frozen producer can replace pending frames", Pixel(frozen) == SKColors.Red);
Check("resume takes newest frame", Pixel(scene) == SKColors.Lime);
Check(
    "immutable scene is never a reusable texture snapshot",
    !SkiaPlatformRasterContent.Equivalent(scene.Commands, scene.Commands)
);
Check(
    "platform raster splitting retains texture",
    SkiaPlatformRasterContent.Split(scene.Commands, 8, 8).Count == 1
);

var effects = new SceneBuilder(1);
effects.pushOffset(2, 1);
effects.pushClipRect(Rect.fromLTWH(0, 0, 2, 3), Clip.hardEdge);
effects.pushOpacity(128);
effects.addTexture(entry.Id, width: 8, height: 8, filterQuality: FilterQuality.none);
effects.pop();
effects.pop();
effects.pop();
using var effectScene = effects.build();
Check(
    "texture follows transform clip and opacity",
    Pixel(effectScene, 2, 1).Alpha is >= 127 and <= 129 && Pixel(effectScene, 4, 1).Alpha == 0
);

var retainedBuilder = new SceneBuilder(1);
var layer = retainedBuilder.pushOffset(0, 0);
retainedBuilder.addTexture(entry.Id, width: 8, height: 8);
retainedBuilder.pop();
using var original = retainedBuilder.build();
var nextBuilder = new SceneBuilder(1);
nextBuilder.addRetained(layer);
using var retained = nextBuilder.build();
entry.PushFrame([255, 0, 0, 255], 1, 1);
Check("retained layers resolve current texture", Pixel(retained) == SKColors.Red);

entry.PushFrame([255, 0, 0, 255, 9, 9, 9, 9, 0, 0, 255, 255], 1, 2, 8);
Check(
    "padded row stride and frame resize",
    Pixel(scene, 0, 7) == SKColors.Blue && Pixel(scene, 0, 0) == SKColors.Red
);
Throws<ArgumentException>("short buffers rejected", () => entry.PushFrame([0], 2, 2));
Throws<ArgumentOutOfRangeException>(
    "invalid stride rejected",
    () => entry.PushFrame([0, 0, 0, 0], 1, 1, 2)
);
Throws<ArgumentOutOfRangeException>("invalid dimensions rejected", () => entry.PushFrame([], 0, 1));
Throws<ArgumentOutOfRangeException>(
    "nonfinite scene geometry rejected",
    () => new SceneBuilder(1).addTexture(entry.Id, width: double.NaN)
);

renderer.AttachSurface(host.RequestInvalidate);
renderer.Submit(
    1,
    new(scene, new(host.ViewEpoch, 1, 8, 8)),
    DorotiUiInvocation.Managed("texture-test")
);
var first = renderer.Paint(surface, 8, 8);
Check("texture scene accepted by production Paint", first is { IsNewFrame: true });
renderer.CompletePaint(first!.Value);
entry.PushFrame([0, 255, 0, 255], 1, 1);
var replay = renderer.Paint(surface, 8, 8);
Check(
    "new frame replays without scene submission",
    replay is { IsNewFrame: false }
        && ReadPixel(0, 0) == SKColors.Lime
        && renderer.Diagnostics.SceneAccepted == 1
);
renderer.CompletePaint(replay!.Value);
renderer.InvalidateGpuContextResources();
Check("CPU source survives renderer context reset", Pixel(scene) == SKColors.Lime);

using var otherRenderer = Renderer(new Host());
using var other = otherRenderer.Textures.CreateTexture();
Check("IDs do not alias across views", other.Id != entry.Id);
surface.Canvas.Clear(SKColors.Transparent);
otherRenderer.DrawPlatformRasterSegment(surface.Canvas, scene.Commands, 8, 8);
Check("foreign texture ID draws blank", ReadPixel(0, 0).Alpha == 0);
using (var initial = renderer.Textures.CreateTexture())
{
    initial.PushFrame([0, 0, 255, 255], 1, 1);
    using var initialFrozen = Scene(initial.Id, freeze: true);
    Check(
        "initially frozen texture acquires its first frame",
        Pixel(initialFrozen) == SKColors.Blue
    );
}
using (var concurrent = renderer.Textures.CreateTexture())
{
    using var concurrentScene = Scene(concurrent.Id);
    var producer = Task.Run(() =>
    {
        for (var i = 0; i < 16; i++)
        {
            try
            {
                concurrent.PushFrame([255, 0, 0, 255], 1, 1);
            }
            catch (ObjectDisposedException)
            {
                return;
            }
        }
    });
    for (var i = 0; i < 16; i++)
    {
        Pixel(concurrentScene);
        if (i == 7)
            concurrent.Dispose();
    }
    await producer;
    Check("producer render and unregister can overlap", Pixel(concurrentScene).Alpha == 0);
}
entry.Dispose();
entry.Dispose();
Check("unregistered retained texture becomes blank", Pixel(scene).Alpha == 0);
Throws<ObjectDisposedException>(
    "late producer rejected",
    () => entry.PushFrame([0, 0, 0, 255], 1, 1)
);
using var orphan = renderer.Textures.CreateTexture();
renderer.Dispose();
Throws<ObjectDisposedException>(
    "renderer shutdown closes entries",
    () => orphan.PushFrame([0, 0, 0, 255], 1, 1)
);
Throws<ObjectDisposedException>(
    "renderer shutdown closes registry",
    () => renderer.Textures.CreateTexture()
);
var ownerContext = new QueuedContext();
var previousContext = SynchronizationContext.Current;
var queuedHost = new Host();
SkiaSceneRenderer queuedRenderer;
try
{
    SynchronizationContext.SetSynchronizationContext(ownerContext);
    queuedRenderer = Renderer(queuedHost);
}
finally
{
    SynchronizationContext.SetSynchronizationContext(previousContext);
}
using (queuedRenderer)
using (var queuedEntry = queuedRenderer.Textures.CreateTexture())
{
    await Task.Run(() =>
    {
        queuedEntry.PushFrame([255, 0, 0, 255], 1, 1);
        queuedEntry.PushFrame([0, 255, 0, 255], 1, 1);
    });
    Check(
        "producer notifications coalesce onto owner context",
        ownerContext.Count == 1 && queuedHost.Invalidations == 0
    );
    ownerContext.Drain();
    Check("owner context delivers native invalidation", queuedHost.Invalidations == 1);
    await Task.Run(() => queuedEntry.PushFrame([0, 0, 255, 255], 1, 1));
    queuedRenderer.Dispose();
    ownerContext.Drain();
    Check("queued notifications are inert after renderer disposal", queuedHost.Invalidations == 1);
}
Console.WriteLine(
    "PASS texture validation (CPU Skia pixels and lifecycle; native GPU presentation not exercised)"
);

SKColor Pixel(Scene target, int x = 0, int y = 0)
{
    surface.Canvas.Clear(SKColors.Transparent);
    renderer.DrawPlatformRasterSegment(surface.Canvas, target.Commands, 8, 8);
    return ReadPixel(x, y);
}
SKColor ReadPixel(int x, int y)
{
    using var image = surface.Snapshot();
    using var bitmap = SKBitmap.FromImage(image);
    return bitmap.GetPixel(x, y);
}
static Scene Scene(long id, bool freeze = false)
{
    var builder = new SceneBuilder(1);
    builder.addTexture(id, width: 8, height: 8, freeze: freeze, filterQuality: FilterQuality.none);
    return builder.build();
}
static SkiaSceneRenderer Renderer(Host host) =>
    new(
        1,
        host,
        null,
        null,
        "texture-test",
        "texture-test",
        "texture-test",
        enablePictureRasterCache: false
    );
static void Check(string name, bool value)
{
    if (!value)
        throw new InvalidOperationException(name);
    Console.WriteLine("PASS " + name);
}
static void Throws<T>(string name, Action action)
    where T : Exception
{
    try
    {
        action();
    }
    catch (T)
    {
        Check(name, true);
        return;
    }
    throw new InvalidOperationException(name);
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

    public void RequestInvalidate() => Interlocked.Increment(ref Invalidations);
}

sealed class QueuedContext : SynchronizationContext
{
    private readonly System.Collections.Concurrent.ConcurrentQueue<(
        SendOrPostCallback Callback,
        object? State
    )> _queue = new();
    public int Count => _queue.Count;

    public override void Post(SendOrPostCallback d, object? state) => _queue.Enqueue((d, state));

    public void Drain()
    {
        while (_queue.TryDequeue(out var item))
            item.Callback(item.State);
    }
}
