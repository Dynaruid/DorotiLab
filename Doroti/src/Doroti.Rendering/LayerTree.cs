using Doroti.Composition;
using Doroti.Graphics;

namespace Doroti.Rendering;

public abstract class Layer : IRenderNode
{
    public abstract Rect Bounds { get; }

    internal abstract LayerSnapshot Snapshot(Matrix parentTransform, Rect? parentClip);
}

public sealed class PictureLayer : Layer
{
    public PictureLayer(Offset offset, DisplayList picture)
    {
        if (!offset.IsFinite)
        {
            throw new ArgumentException("Picture offset must be finite.", nameof(offset));
        }
        Offset = offset;
        Picture = picture ?? throw new ArgumentNullException(nameof(picture));
        Bounds = new(
            picture.Bounds.Left + offset.X,
            picture.Bounds.Top + offset.Y,
            picture.Bounds.Right + offset.X,
            picture.Bounds.Bottom + offset.Y);
    }

    public Offset Offset { get; }

    public DisplayList Picture { get; }

    public override Rect Bounds { get; }

    internal override LayerSnapshot Snapshot(Matrix parentTransform, Rect? parentClip)
    {
        var local = Matrix.CreateTranslation(Offset.X, Offset.Y);
        var bounds = parentTransform.TransformBounds(Bounds);
        if (parentClip is { } clip)
        {
            bounds = bounds.Intersect(clip);
        }
        return new PictureLayerSnapshot(bounds, local, Picture);
    }
}

public sealed class TransformLayer : Layer
{
    public TransformLayer(Matrix transform, Layer child)
    {
        if (!transform.IsFinite)
        {
            throw new ArgumentException("Layer transform must be finite.", nameof(transform));
        }
        Transform = transform;
        Child = child ?? throw new ArgumentNullException(nameof(child));
        Bounds = transform.TransformBounds(child.Bounds);
    }

    public Matrix Transform { get; }

    public Layer Child { get; }

    public override Rect Bounds { get; }

    internal override LayerSnapshot Snapshot(Matrix parentTransform, Rect? parentClip)
    {
        var combined = parentTransform * Transform;
        return new TransformLayerSnapshot(
            combined.TransformBounds(Child.Bounds).Intersect(parentClip ?? combined.TransformBounds(Child.Bounds)),
            Transform,
            Child.Snapshot(combined, parentClip));
    }
}

public sealed class ClipRectLayer : Layer
{
    public ClipRectLayer(Rect clipRect, Layer child)
    {
        if (!clipRect.IsFinite)
        {
            throw new ArgumentException("Layer clip must be finite.", nameof(clipRect));
        }
        ClipRect = clipRect;
        Child = child ?? throw new ArgumentNullException(nameof(child));
        Bounds = child.Bounds.Intersect(clipRect);
    }

    public Rect ClipRect { get; }

    public Layer Child { get; }

    public override Rect Bounds { get; }

    internal override LayerSnapshot Snapshot(Matrix parentTransform, Rect? parentClip)
    {
        var transformedClip = parentTransform.TransformBounds(ClipRect);
        var effectiveClip = parentClip is { } clip ? transformedClip.Intersect(clip) : transformedClip;
        return new ClipRectLayerSnapshot(
            parentTransform.TransformBounds(Child.Bounds).Intersect(effectiveClip),
            ClipRect,
            Child.Snapshot(parentTransform, effectiveClip));
    }
}

public sealed class OpacityLayer : Layer
{
    public OpacityLayer(double opacity, Layer child)
    {
        if (!double.IsFinite(opacity) || opacity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity));
        }
        Opacity = opacity;
        Child = child ?? throw new ArgumentNullException(nameof(child));
    }

    public double Opacity { get; }

    public Layer Child { get; }

    public override Rect Bounds => Child.Bounds;

    internal override LayerSnapshot Snapshot(Matrix parentTransform, Rect? parentClip) => new OpacityLayerSnapshot(
        parentTransform.TransformBounds(Child.Bounds).Intersect(parentClip ?? parentTransform.TransformBounds(Child.Bounds)),
        Opacity,
        Child.Snapshot(parentTransform, parentClip));
}

public sealed class ContainerLayer : Layer
{
    private readonly IReadOnlyList<Layer> _children;

    public ContainerLayer(IEnumerable<Layer> children)
    {
        ArgumentNullException.ThrowIfNull(children);
        var copy = children.ToArray();
        if (copy.Any(child => child is null))
        {
            throw new ArgumentException("Container children cannot contain null.", nameof(children));
        }
        _children = Array.AsReadOnly(copy);
        Bounds = copy.Aggregate(Rect.Zero, (bounds, child) => bounds.ExpandToInclude(child.Bounds));
    }

    public IReadOnlyList<Layer> Children => _children;

    public override Rect Bounds { get; }

    internal override LayerSnapshot Snapshot(Matrix parentTransform, Rect? parentClip)
    {
        var children = _children.Select(child => child.Snapshot(parentTransform, parentClip)).ToArray();
        var bounds = children.Aggregate(Rect.Zero, (current, child) => current.ExpandToInclude(child.Bounds));
        return new ContainerLayerSnapshot(bounds, children);
    }
}

public sealed class LayerTreeSnapshot
{
    private readonly LayerSnapshot _root;
    private readonly IReadOnlyList<ResourceId> _resources;

    private LayerTreeSnapshot(LayerSnapshot root)
    {
        _root = root;
        Bounds = root.Bounds;
        var resources = new List<ResourceId>();
        root.CollectResources(resources);
        _resources = Array.AsReadOnly(resources.Distinct().ToArray());
        DisplayListBytes = root.DisplayListBytes;
    }

    public Rect Bounds { get; }

    public IReadOnlyList<ResourceId> Resources => _resources;

    public int DisplayListBytes { get; }

    public static LayerTreeSnapshot Create(Layer root)
    {
        ArgumentNullException.ThrowIfNull(root);
        return new(root.Snapshot(Matrix.Identity, null));
    }

    public void Rasterize(IRasterCanvas canvas, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources) =>
        _root.Paint(canvas, resources);
}

internal abstract class LayerSnapshot(Rect bounds)
{
    internal Rect Bounds { get; } = bounds;

    internal abstract int DisplayListBytes { get; }

    internal abstract void Paint(IRasterCanvas canvas, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources);

    internal abstract void CollectResources(ICollection<ResourceId> resources);
}

internal sealed class PictureLayerSnapshot(Rect bounds, Matrix transform, DisplayList picture) : LayerSnapshot(bounds)
{
    internal override int DisplayListBytes => picture.ByteSize;

    internal override void Paint(IRasterCanvas canvas, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources)
    {
        canvas.Save();
        canvas.Transform(transform);
        picture.Execute(canvas, resources);
        canvas.Restore();
    }

    internal override void CollectResources(ICollection<ResourceId> resources)
    {
        foreach (var resource in picture.Resources)
        {
            resources.Add(resource);
        }
    }
}

internal abstract class SingleChildLayerSnapshot(Rect bounds, LayerSnapshot child) : LayerSnapshot(bounds)
{
    protected LayerSnapshot Child { get; } = child;

    internal override int DisplayListBytes => Child.DisplayListBytes;

    internal override void CollectResources(ICollection<ResourceId> resources) => Child.CollectResources(resources);
}

internal sealed class TransformLayerSnapshot(Rect bounds, Matrix transform, LayerSnapshot child) : SingleChildLayerSnapshot(bounds, child)
{
    internal override void Paint(IRasterCanvas canvas, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources)
    {
        canvas.Save();
        canvas.Transform(transform);
        Child.Paint(canvas, resources);
        canvas.Restore();
    }
}

internal sealed class ClipRectLayerSnapshot(Rect bounds, Rect clip, LayerSnapshot child) : SingleChildLayerSnapshot(bounds, child)
{
    internal override void Paint(IRasterCanvas canvas, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources)
    {
        canvas.Save();
        canvas.ClipRect(clip);
        Child.Paint(canvas, resources);
        canvas.Restore();
    }
}

internal sealed class OpacityLayerSnapshot(Rect bounds, double opacity, LayerSnapshot child) : SingleChildLayerSnapshot(bounds, child)
{
    internal override void Paint(IRasterCanvas canvas, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources)
    {
        canvas.Save();
        canvas.MultiplyOpacity(opacity);
        Child.Paint(canvas, resources);
        canvas.Restore();
    }
}

internal sealed class ContainerLayerSnapshot(Rect bounds, IReadOnlyList<LayerSnapshot> children) : LayerSnapshot(bounds)
{
    internal override int DisplayListBytes => children.Sum(child => child.DisplayListBytes);

    internal override void Paint(IRasterCanvas canvas, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources)
    {
        foreach (var child in children)
        {
            child.Paint(canvas, resources);
        }
    }

    internal override void CollectResources(ICollection<ResourceId> resources)
    {
        foreach (var child in children)
        {
            child.CollectResources(resources);
        }
    }
}
