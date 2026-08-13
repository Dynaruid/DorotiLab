using Doroti.Composition;
using Doroti.Core;
using Doroti.Graphics;
using Doroti.Platform;
using Doroti.Rendering;

namespace Doroti.Widgets;

public sealed class RawImage(ResourceId resource, Size imageSize, double opacity = 1, Key? key = null) : LeafRenderObjectWidget(key)
{
    public ResourceId Resource { get; } = resource.Value == 0 ? throw new ArgumentOutOfRangeException(nameof(resource)) : resource;

    public Size ImageSize { get; } = !imageSize.IsFinite || imageSize.IsEmpty ? throw new ArgumentOutOfRangeException(nameof(imageSize)) : imageSize;

    public double Opacity { get; } = !double.IsFinite(opacity) || opacity is < 0 or > 1 ? throw new ArgumentOutOfRangeException(nameof(opacity)) : opacity;

    public override RenderObject CreateRenderObject(BuildContext context) => new RenderRawImage(Resource, ImageSize, Opacity);

    public override void UpdateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var image = (RenderRawImage)renderObject;
        image.Resource = Resource;
        image.ImageSize = ImageSize;
        image.Opacity = Opacity;
    }
}

public sealed class RenderRawImage : RenderBox
{
    private ResourceId _resource;
    private Size _imageSize;
    private double _opacity;

    public RenderRawImage(ResourceId resource, Size imageSize, double opacity)
    {
        _resource = resource;
        _imageSize = imageSize;
        _opacity = opacity;
    }

    public ResourceId Resource
    {
        get => _resource;
        set
        {
            if (_resource != value)
            {
                _resource = value;
                MarkNeedsPaint();
            }
        }
    }

    public Size ImageSize
    {
        get => _imageSize;
        set
        {
            if (_imageSize != value)
            {
                _imageSize = value;
                MarkNeedsLayout();
            }
        }
    }

    public double Opacity
    {
        get => _opacity;
        set
        {
            if (_opacity != value)
            {
                _opacity = value;
                MarkNeedsPaint();
            }
        }
    }

    public override void DescribeSemanticsConfiguration(SemanticsConfiguration configuration)
    {
        configuration.Role = SemanticsRole.Image;
        configuration.State = SemanticsState.Enabled | SemanticsState.ReadOnly;
    }

    protected override void PerformLayout() => SetSize(Constraints.Constrain(_imageSize));

    protected override void Paint(PaintingContext context, Offset offset) => context.DrawImage(
        _resource,
        Rect.FromLeftTopWidthHeight(0, 0, _imageSize.Width, _imageSize.Height),
        Rect.FromLeftTopWidthHeight(0, 0, Size.Width, Size.Height),
        _opacity);
}

public sealed class AsyncImage : StatefulWidget
{
    public AsyncImage(
        IImageProvider provider,
        ImageCache cache,
        IUiDispatcher dispatcher,
        Widget? loading = null,
        Func<Exception, Widget>? errorBuilder = null,
        Key? key = null)
        : base(key)
    {
        Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        Cache = cache ?? throw new ArgumentNullException(nameof(cache));
        Dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        Loading = loading;
        ErrorBuilder = errorBuilder;
    }

    public IImageProvider Provider { get; }

    public ImageCache Cache { get; }

    public IUiDispatcher Dispatcher { get; }

    public Widget? Loading { get; }

    public Func<Exception, Widget>? ErrorBuilder { get; }

    public override State CreateState() => new AsyncImageState();
}

public sealed class AsyncImageState : State<AsyncImage>
{
    private CancellationTokenSource? _loadCancellation;
    private ImageCache.ImageLease? _lease;
    private Exception? _error;
    private long _generation;

    protected internal override void InitState() => StartLoad();

    protected internal override void DidUpdateWidget(AsyncImage oldWidget)
    {
        if (!ReferenceEquals(oldWidget.Provider, Widget.Provider) || !ReferenceEquals(oldWidget.Cache, Widget.Cache))
        {
            StartLoad();
        }
    }

    protected internal override void Dispose()
    {
        _generation++;
        _loadCancellation?.Cancel();
        _loadCancellation?.Dispose();
        _lease?.Dispose();
    }

    public override Widget? Build(BuildContext context)
    {
        if (_lease is { } lease)
        {
            return new RawImage(lease.Resource, lease.Size);
        }
        if (_error is not null)
        {
            return Widget.ErrorBuilder?.Invoke(_error);
        }
        return Widget.Loading;
    }

    private void StartLoad()
    {
        var generation = ++_generation;
        _loadCancellation?.Cancel();
        _loadCancellation?.Dispose();
        _loadCancellation = new();
        _lease?.Dispose();
        _lease = null;
        _error = null;
        _ = ResolveAsync(generation, _loadCancellation.Token);
    }

    private async Task ResolveAsync(long generation, CancellationToken cancellationToken)
    {
        ImageCache.ImageLease? lease = null;
        Exception? error = null;
        try
        {
            lease = await Widget.Cache.ResolveAsync(Widget.Provider, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        catch (Exception exception)
        {
            error = exception;
        }

        Widget.Dispatcher.Post(() =>
        {
            if (!Mounted || generation != _generation)
            {
                lease?.Dispose();
                return;
            }
            SetState(() =>
            {
                _lease?.Dispose();
                _lease = lease;
                _error = error;
            });
        });
    }
}
