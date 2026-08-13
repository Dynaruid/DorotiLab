using Doroti.Composition;
using Doroti.Graphics;

namespace Doroti.Rendering;

public sealed class PaintingContext
{
    private readonly PipelineOwner _owner;
    private readonly DisplayListBuilder _displayList;
    private readonly List<Layer> _children = [];

    internal PaintingContext(PipelineOwner owner, RenderObject node)
    {
        _owner = owner;
        var size = node is RenderBox box ? box.Size : new Size(1, 1);
        _displayList = new(new(0, 0, Math.Max(1, size.Width), Math.Max(1, size.Height)));
    }

    public void DrawColor(Color color) => _displayList.DrawColor(color);

    public void DrawRect(Rect rect, RasterPaint paint) => _displayList.DrawRect(rect, paint);

    public void DrawPath(PathGeometry path, RasterPaint paint) => _displayList.DrawPath(path, paint);

    public void DrawImage(ResourceId resource, Rect source, Rect destination, double opacity = 1) =>
        _displayList.DrawImage(resource, source, destination, opacity);

    public void DrawText(string text, Offset origin, double fontSize, RasterPaint paint, string? fontFamily = null) =>
        _displayList.DrawText(text, origin, fontSize, paint, fontFamily);

    public void PaintChild(RenderBox child, Offset offset)
    {
        ArgumentNullException.ThrowIfNull(child);
        if (!offset.IsFinite)
        {
            throw new ArgumentException("Paint offsets must be finite.", nameof(offset));
        }
        var layer = child.BuildPaintLayer(_owner);
        var data = child.ParentData as BoxParentData;
        var transform = Matrix.CreateTranslation(offset.X, offset.Y) * (data?.Transform ?? Matrix.Identity);
        _children.Add(transform == Matrix.Identity ? layer : new TransformLayer(transform, layer));
    }

    public void PushClipRect(Rect clip, Action<PaintingContext> painter)
    {
        ArgumentNullException.ThrowIfNull(painter);
        if (!clip.IsFinite)
        {
            throw new ArgumentException("Paint clips must be finite.", nameof(clip));
        }
        var nested = new PaintingContext(_owner, new PaintBoundsBox(clip.Size));
        painter(nested);
        _children.Add(new ClipRectLayer(clip, nested.BuildLayer()));
    }

    public void PushOpacity(double opacity, Action<PaintingContext> painter)
    {
        ArgumentNullException.ThrowIfNull(painter);
        if (!double.IsFinite(opacity) || opacity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity));
        }
        var nested = new PaintingContext(_owner, new PaintBoundsBox(_displayList.CullSize));
        painter(nested);
        _children.Add(new OpacityLayer(opacity, nested.BuildLayer()));
    }

    internal Layer BuildLayer()
    {
        var picture = new PictureLayer(Offset.Zero, _displayList.Build());
        return _children.Count == 0
            ? picture
            : new ContainerLayer([picture, .. _children]);
    }

    private sealed class PaintBoundsBox : RenderBox
    {
        internal PaintBoundsBox(Size size) => SetSize(size);

        protected override void PerformLayout()
        {
        }
    }
}
