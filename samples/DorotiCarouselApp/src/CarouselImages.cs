using Doroti.Framework.Foundation;
using Doroti.Framework.Painting;
using Doroti.Framework.Scheduler;
using Doroti.Framework.Widgets;
using Doroti.Runtime;
using Doroti.Ui;

namespace DorotiCarouselApp;

// FittedBox changes the physical footprint without changing its child's layout size.
internal sealed class CarouselImageScale(double scale, Widget child) : InheritedWidget(child: child)
{
    internal double Scale { get; } = scale;
    internal static double Of(BuildContext context) =>
        context.dependOnInheritedWidgetOfExactType<CarouselImageScale>()?.Scale ?? 1;
    public override bool updateShouldNotify(InheritedWidget oldWidget) =>
        Scale != ((CarouselImageScale)oldWidget).Scale;
}

internal sealed record CarouselImageKey(MemoryImage Source, int Width, int Height, BoxFit Fit);

// Keep the original aspect ratio, including the pixels cropped by BoxFit.cover.
// Buckets and a fixed quality margin keep animation ticks out of the cache key.
internal sealed class CarouselImageProvider(CarouselImageKey key) : ImageProvider<CarouselImageKey>
{
    internal static IImageProvider For(BuildContext context, string path, double width, double height,
        BoxFit fit = BoxFit.cover)
    {
        var source = Ui.Image(path);
        if (!double.IsFinite(width) || !double.IsFinite(height) || width <= 0 || height <= 0) return source;
        var scale = (MediaQuery.maybeDevicePixelRatioOf(context) ?? 1) * CarouselImageScale.Of(context);
        return new CarouselImageProvider(new(source, Bucket(width * scale), Bucket(height * scale), fit));
    }

    internal static int Bucket(double physicalPixels) =>
        checked((int)Math.Max(64, Math.Ceiling(physicalPixels * 1.2 / 64) * 64));

    internal TargetImageSize TargetSize(long intrinsicWidth, long intrinsicHeight)
    {
        var x = (double)key.Width / intrinsicWidth;
        var y = (double)key.Height / intrinsicHeight;
        var scale = Math.Min(1, key.Fit == BoxFit.cover ? Math.Max(x, y) : Math.Min(x, y));
        return new TargetImageSize(
            Math.Max(1, (long)Math.Ceiling(intrinsicWidth * scale)),
            Math.Max(1, (long)Math.Ceiling(intrinsicHeight * scale)));
    }

    public override Future<CarouselImageKey> obtainKey(ImageConfiguration configuration) =>
        new SynchronousFuture<CarouselImageKey>(key);

    public override ImageStreamCompleter loadImage(CarouselImageKey cacheKey,
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> decode)
    {
        Func<ImmutableBuffer, Func<long, long, TargetImageSize>?, Future<Codec>> resize =
            (buffer, _) => decode(buffer, TargetSize);
        var completer = cacheKey.Source.loadImage(cacheKey.Source, resize);
        completer.addEphemeralErrorListener((_, _) => DartAsyncRuntime.scheduleMicrotask(() =>
            PaintingBinding.instance.imageCache.evict(cacheKey)));
        return completer;
    }

    public override bool Equals(object? other) => other is CarouselImageProvider provider && key == provider.Key;
    private CarouselImageKey Key => key;
    public override int GetHashCode() => key.GetHashCode();
}

// One decoded image at a time, between frames, using exactly the display provider.
internal sealed class CarouselImageWarmup(IReadOnlyList<string> paths, double size, Widget child) : StatefulWidget
{
    internal IReadOnlyList<string> Paths { get; } = paths;
    internal double Size { get; } = size;
    internal Widget Child { get; } = child;
    public override IState createState() => new CarouselImageWarmupState();
}

internal sealed class CarouselImageWarmupState : State<CarouselImageWarmup>
{
    private IImageProvider[] _providers = [];
    private CancellationTokenSource? _cancellation;

    public override void didChangeDependencies() { base.didChangeDependencies(); Schedule(); }
    public override void didUpdateWidget(CarouselImageWarmup oldWidget) { base.didUpdateWidget(oldWidget); Schedule(); }
    private void Schedule()
    {
        var providers = widget.Paths.Select(path => CarouselImageProvider.For(context, path, widget.Size, widget.Size)).ToArray();
        if (_providers.SequenceEqual(providers)) return;
        _providers = providers;
        _cancellation?.Cancel();
        _cancellation?.Dispose();
        _cancellation = new();
        var token = _cancellation.Token;
        SchedulerBinding.instance.addPostFrameCallback(time =>
        {
            if (mounted && !token.IsCancellationRequested) _ = Prime(providers, token);
        });
    }

    private async Task Prime(IImageProvider[] providers, CancellationToken token)
    {
        try
        {
            foreach (var provider in providers)
            {
                token.ThrowIfCancellationRequested();
                if (!mounted) return;
                await ImageLibrary.precacheImage(provider, context).asTask().WaitAsync(token);
                await SchedulerBinding.instance.endOfFrame.asTask().WaitAsync(token);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
    }

    public override Widget build(BuildContext context) => widget.Child;
    public override void dispose()
    {
        _cancellation?.Cancel();
        _cancellation?.Dispose();
        base.dispose();
    }
}
