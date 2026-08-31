using System.Collections.ObjectModel;

namespace Doroti.Graphics.DisplayList;

public abstract record DisplayListCommand
{
    public abstract DisplayListOpcode Opcode { get; }
}

public sealed record DisplaySaveCommand : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.Save;
}

public sealed record DisplayRestoreCommand : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.Restore;
}

public sealed record DisplaySaveLayerCommand(DisplayRect? Bounds, DisplayPaint? Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.SaveLayer;
}

public sealed record DisplayTransformCommand(DisplayMatrix Matrix) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.Transform;
}

public sealed record DisplayClipRectCommand(
    DisplayRect Rect,
    DisplayClipOperation Operation = DisplayClipOperation.Intersect,
    bool IsAntiAlias = true) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.ClipRect;
}

public sealed record DisplayClipRoundedRectCommand(
    DisplayRoundedRect RoundedRect,
    DisplayClipOperation Operation = DisplayClipOperation.Intersect,
    bool IsAntiAlias = true) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.ClipRoundedRect;
}

public sealed record DisplayClipPathCommand(
    DisplayPath Path,
    DisplayClipOperation Operation = DisplayClipOperation.Intersect,
    bool IsAntiAlias = true) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.ClipPath;
}

public sealed record DisplayDrawColorCommand(uint Color, DisplayBlendMode BlendMode) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawColor;
}

public sealed record DisplayDrawPaintCommand(DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawPaint;
}

public sealed record DisplayDrawLineCommand(
    DisplayPoint Start,
    DisplayPoint End,
    DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawLine;
}

public sealed record DisplayDrawPointsCommand : DisplayListCommand
{
    private readonly ReadOnlyCollection<DisplayPoint> _points;

    public DisplayDrawPointsCommand(
        DisplayPointMode mode,
        IEnumerable<DisplayPoint> points,
        DisplayPaint paint)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(paint);
        Mode = mode;
        _points = new ReadOnlyCollection<DisplayPoint>(points.ToArray());
        Paint = paint;
    }

    public DisplayPointMode Mode { get; }

    public IReadOnlyList<DisplayPoint> Points => _points;

    public DisplayPaint Paint { get; }

    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawPoints;
}

public sealed record DisplayDrawRectCommand(DisplayRect Rect, DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawRect;
}

public sealed record DisplayDrawRoundedRectCommand(
    DisplayRoundedRect RoundedRect,
    DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawRoundedRect;
}

public sealed record DisplayDrawDoubleRoundedRectCommand(
    DisplayRoundedRect Outer,
    DisplayRoundedRect Inner,
    DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawDoubleRoundedRect;
}

public sealed record DisplayDrawCircleCommand(
    DisplayPoint Center,
    float Radius,
    DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawCircle;
}

public sealed record DisplayDrawOvalCommand(DisplayRect Bounds, DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawOval;
}

public sealed record DisplayDrawArcCommand(
    DisplayRect Bounds,
    float StartAngle,
    float SweepAngle,
    bool UseCenter,
    DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawArc;
}

public sealed record DisplayDrawPathCommand(DisplayPath Path, DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawPath;
}

public sealed record DisplayDrawShadowCommand(
    DisplayPath Path,
    uint Color,
    float Elevation,
    bool TransparentOccluder) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawShadow;
}

public sealed record DisplayDrawImageCommand(
    DisplayResourceReference Image,
    DisplayPoint Offset,
    DisplaySamplingQuality Sampling,
    DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawImage;
}

public sealed record DisplayDrawImageRectCommand(
    DisplayResourceReference Image,
    DisplayRect Source,
    DisplayRect Destination,
    DisplaySamplingQuality Sampling,
    DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawImageRect;
}

public sealed record DisplayDrawNinePatchCommand(
    DisplayResourceReference Image,
    DisplayRect Center,
    DisplayRect Destination,
    DisplaySamplingQuality Sampling,
    DisplayPaint Paint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawNinePatch;
}

public sealed record DisplayDrawParagraphCommand(
    DisplayParagraphRecipe Paragraph,
    DisplayPoint Offset) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawParagraph;
}

public sealed record DisplayPushOpacityCommand(float Opacity, DisplayPoint Offset) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.PushOpacity;
}

public sealed record DisplayPushColorFilterCommand(
    DisplayColorFilter Filter,
    DisplayPoint Offset) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.PushColorFilter;
}

public sealed record DisplayPushImageFilterCommand(
    DisplayImageFilter Filter,
    DisplayPoint Offset,
    DisplayRect? Bounds = null) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.PushImageFilter;
}

public sealed record DisplayPushBackdropFilterCommand(
    DisplayImageFilter Filter,
    DisplayBlendMode BlendMode,
    ulong BackdropId,
    DisplayPoint Offset) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.PushBackdropFilter;
}

public sealed record DisplayPushShaderMaskCommand(
    DisplayShader Shader,
    DisplayRect MaskRect,
    DisplayBlendMode BlendMode) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.PushShaderMask;
}

[Flags]
public enum DisplayRetainedSceneCacheHint : byte
{
    None = 0,
    IsComplex = 1 << 0,
    WillChange = 1 << 1,
}

public sealed record DisplayDrawRetainedSceneCommand(
    DisplayResourceReference Scene,
    DisplayPoint Offset,
    DisplayRetainedSceneCacheHint CacheHint) : DisplayListCommand
{
    public override DisplayListOpcode Opcode => DisplayListOpcode.DrawRetainedScene;
}
