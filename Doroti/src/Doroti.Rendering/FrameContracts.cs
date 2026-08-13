using Doroti.Composition;
using Doroti.Graphics;

namespace Doroti.Rendering;

/// <summary>Host-neutral view metrics carried with a committed Flutter scene.</summary>
public readonly record struct RenderViewConfiguration(
    Size LogicalSize,
    Size PixelSize,
    double DevicePixelRatio = 1,
    long SurfaceGeneration = 0)
{
    public RenderViewConfiguration(Size logicalSize, double devicePixelRatio = 1)
        : this(logicalSize, PixelExtentPolicy.ToPixelSize(logicalSize, devicePixelRatio), devicePixelRatio)
    {
    }

    public RenderViewConfiguration Validate()
    {
        if (!LogicalSize.IsFinite || LogicalSize.Width <= 0 || LogicalSize.Height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(LogicalSize), "Root logical size must be finite and positive.");
        }
        if (!double.IsFinite(DevicePixelRatio) || DevicePixelRatio <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(DevicePixelRatio));
        }
        if (!PixelSize.IsFinite || PixelSize.Width <= 0 || PixelSize.Height <= 0 ||
            PixelSize.Width != Math.Truncate(PixelSize.Width) || PixelSize.Height != Math.Truncate(PixelSize.Height))
        {
            throw new ArgumentOutOfRangeException(nameof(PixelSize), "Root physical size must contain positive whole pixels.");
        }
        return this;
    }
}

/// <summary>Immutable scene/frame DTO passed from Flutter hosting to the raster mailbox.</summary>
public sealed record RenderPipelineFrame(
    Layer RootLayer,
    LayerTreeSnapshot Snapshot,
    long Sequence,
    RenderViewConfiguration Configuration);
