using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Runtime;
using Doroti.Ui;
using System.Diagnostics;

if (args.Contains("--native") && System.Runtime.CompilerServices.RuntimeFeature.IsDynamicCodeSupported)
    throw new InvalidOperationException("Expected a published NativeAOT executable.");

var provider = new ExternalProvider<ExternalKey>(new ExternalKey(7));
IImageProvider erased = provider;
var configuration = new ImageConfiguration(devicePixelRatio: 2);
var pending = erased.obtainKeyObject(configuration);
Require(pending is SynchronousFuture<object>, "synchronous key delivery through erased contract");
var key = await pending;
Require(Equals(key, new ExternalKey(7)), "consumer value-type key retained");
Require(ReferenceEquals(provider.Configuration, configuration), "configuration identity");
var imageStream = erased.resolve(configuration);
Require(ReferenceEquals(imageStream, provider.Stream), "consumer resolve override");
erased.resolveStreamForKeyObject(configuration, imageStream, key, (_, _) => throw new Exception("unexpected error"));
Require(provider.Resolved == 1, "typed resolution override");
Require(ReferenceEquals(erased.createStream(configuration), provider.Stream), "stream factory override");
Require(await erased.obtainCacheStatus(configuration) is null, "cache override");
Require(await erased.evict(configuration: configuration), "evict override");
try
{
    erased.loadImageObject("wrong-key-type", (_, _) => throw new Exception("must reject key first"));
    throw new Exception("accepted an incompatible key");
}
catch (InvalidCastException) { }
Require(ReferenceEquals(ResizeImage.resizeIfNeeded(null, null, erased), erased), "unresized provider identity");
foreach (var item in new (ResizeImagePolicy Policy, long? Width, long? Height, bool Upscale, long? ExpectedW, long? ExpectedH)[]
{
    (ResizeImagePolicy.exact, 20, 30, false, 20, 30),
    (ResizeImagePolicy.exact, 200, 300, false, 100, 50),
    (ResizeImagePolicy.exact, 200, null, true, 200, null),
    (ResizeImagePolicy.fit, 20, 20, false, 20, 10),
    (ResizeImagePolicy.fit, 200, 200, false, 100, 50),
    (ResizeImagePolicy.fit, 200, 200, true, 200, 100),
    (ResizeImagePolicy.fit, null, 100, true, 200, 100),
})
{
    var resized = new ResizeImage(erased, width: item.Width, height: item.Height, policy: item.Policy, allowUpscaling: item.Upscale);
    var resizePending = resized.obtainKey(configuration);
    Require(resizePending is SynchronousFuture<ResizeImageKey>, "resize preserves synchronous key");
    var resizeKey = await resizePending;
    var called = false;
    var completer = resized.loadImage(resizeKey, (Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>>)((_, size) =>
    {
        var actual = size!(100, 50);
        Require(actual.width == item.ExpectedW && actual.height == item.ExpectedH, $"resize dimensions: {item}");
        called = true;
        return Future<Codec>.value(null!);
    }));
    Require(called && ReferenceEquals(completer, provider.Completer), "typed loadImage dispatch and completer identity");
    Require(resizeKey.Equals(await new ResizeImage(erased, width: item.Width, height: item.Height, policy: item.Policy, allowUpscaling: item.Upscale).obtainKey(configuration)), "resize cache equality");
}
var bufferResize = new ResizeImage(erased, width: 17, height: 29, allowUpscaling: true);
var bufferCalled = false;
bufferResize.loadBuffer(await bufferResize.obtainKey(configuration), (Func<ImmutableBuffer, bool, long?, long?, Future<Codec>>)((_, upscale, height, width) =>
{
    Require(upscale && height == 29 && width == 17, "buffer callback height/width order and upscaling");
    bufferCalled = true;
    return Future<Codec>.value(null!);
}));
Require(bufferCalled, "buffer decoder called");
var referenceProvider = new ExternalProvider<string>("external-reference");
Require(Equals(await ((IImageProvider)referenceProvider).obtainKeyObject(configuration), "external-reference"), "reference-type key");
var queued = new List<Action>();
using (DartAsyncRuntime.enterMicrotaskScheduler(queued.Add))
{
    var ordinary = ((IImageProvider)new OrdinaryProvider()).obtainKeyObject(configuration);
    Require(queued.Count == 1 && !ordinary.asTask().IsCompleted, "ordinary Future key schedules its continuation");
    queued[0]();
    Require(Equals(await ordinary.asTask().WaitAsync(TimeSpan.FromSeconds(5)), "ordinary"), "scheduled key delivered");
}
var deferred = new DeferredProvider();
var deferredKey = new ResizeImage(deferred, width: 2).obtainKey(configuration);
Require(!deferredKey.asTask().IsCompleted, "deferred key stays pending");
deferred.Source.SetResult("ready");
await deferredKey.asTask().WaitAsync(TimeSpan.FromSeconds(5));
var failure = new InvalidOperationException("key-failure");
deferred = new DeferredProvider();
deferredKey = new ResizeImage(deferred, width: 2).obtainKey(configuration);
deferred.Source.SetException(failure);
try { await deferredKey.asTask().WaitAsync(TimeSpan.FromSeconds(5)); throw new Exception("swallowed key failure"); }
catch (InvalidOperationException error) when (ReferenceEquals(error, failure)) { }
Console.WriteLine("External image provider: value/reference keys, sync/deferred/error, cache, typed dispatch, resize dimensions and buffer order PASS");

static void Require(bool value, string message) { if (!value) throw new Exception(message); }
readonly record struct ExternalKey(int Value);
sealed class ExternalProvider<T>(T key) : ImageProvider<T>
{
    public ImageConfiguration? Configuration;
    public readonly ImageStream Stream = new();
    public readonly ImageStreamCompleter Completer = new ExternalCompleter();
    public int Resolved;
    public override Future<T> obtainKey(ImageConfiguration configuration) { Configuration = configuration; return new SynchronousFuture<T>(key); }
    public override ImageStream resolve(ImageConfiguration configuration) => Stream;
    public override ImageStream createStream(ImageConfiguration configuration) => Stream;
    public override void resolveStreamForKey(ImageConfiguration configuration, ImageStream stream, T actual, Action<object, StackTrace?> handleError)
    {
        if (!Equals(key, actual)) throw new Exception("key changed");
        Resolved++;
    }
    public override Future<ImageCacheStatus?> obtainCacheStatus(ImageConfiguration configuration, Action<object, StackTrace?>? handleError = null) => new SynchronousFuture<ImageCacheStatus?>(null);
    public override Future<bool> evict(ImageCache? cache = null, ImageConfiguration configuration = default!) => new SynchronousFuture<bool>(true);
    public override ImageStreamCompleter loadImage(T actual, Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        if (!Equals(key, actual)) throw new Exception("key changed");
        decode(null!, null);
        return Completer;
    }
    public override ImageStreamCompleter loadBuffer(T actual, Func<ImmutableBuffer, bool, long?, long?, Future<Codec>> decode)
    {
        if (!Equals(key, actual)) throw new Exception("key changed");
        decode(null!, false, null, null);
        return Completer;
    }
}
sealed class DeferredProvider : ImageProvider<string>
{
    public readonly TaskCompletionSource<string> Source = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public override Future<string> obtainKey(ImageConfiguration configuration) => Future<string>.fromTask(Source.Task);
}

sealed class ExternalCompleter : ImageStreamCompleter { }
sealed class OrdinaryProvider : ImageProvider<string>
{
    public override Future<string> obtainKey(ImageConfiguration configuration) => Future<string>.value("ordinary");
}
