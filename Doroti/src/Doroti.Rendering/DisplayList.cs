using Doroti.Composition;
using Doroti.Graphics;

namespace Doroti.Rendering;

public abstract record DisplayCommand;

public sealed record SaveCommand : DisplayCommand;

public sealed record RestoreCommand : DisplayCommand;

public sealed record TransformCommand(Matrix Transform) : DisplayCommand;

public sealed record ClipRectCommand(Rect Rect) : DisplayCommand;

public sealed record ClipPathCommand(PathGeometry Path) : DisplayCommand;

public sealed record DrawColorCommand(Color Color) : DisplayCommand;

public sealed record DrawRectCommand(Rect Rect, RasterPaint Paint) : DisplayCommand;

public sealed record DrawPathCommand(PathGeometry Path, RasterPaint Paint) : DisplayCommand;

public sealed record DrawImageCommand(ResourceId Resource, Rect Source, Rect Destination, double Opacity) : DisplayCommand;

public sealed record DrawTextCommand(string Text, Offset Origin, double FontSize, RasterPaint Paint, string? FontFamily) : DisplayCommand;

public sealed class DisplayList
{
    private readonly IReadOnlyList<DisplayCommand> _commands;
    private readonly IReadOnlyList<ResourceId> _resources;

    internal DisplayList(DisplayCommand[] commands, ResourceId[] resources, Rect bounds, int byteSize)
    {
        _commands = Array.AsReadOnly(commands);
        _resources = Array.AsReadOnly(resources);
        Bounds = bounds;
        ByteSize = byteSize;
    }

    public IReadOnlyList<DisplayCommand> Commands => _commands;

    public IReadOnlyList<ResourceId> Resources => _resources;

    public Rect Bounds { get; }

    public int ByteSize { get; }

    public void Execute(IRasterCanvas canvas, IReadOnlyDictionary<ResourceId, IResourceSnapshot> resources)
    {
        ArgumentNullException.ThrowIfNull(canvas);
        ArgumentNullException.ThrowIfNull(resources);
        var initialSaveCount = canvas.SaveCount;
        foreach (var command in _commands)
        {
            switch (command)
            {
                case SaveCommand:
                    canvas.Save();
                    break;
                case RestoreCommand:
                    canvas.Restore();
                    break;
                case TransformCommand transform:
                    canvas.Transform(transform.Transform);
                    break;
                case ClipRectCommand clip:
                    canvas.ClipRect(clip.Rect);
                    break;
                case ClipPathCommand clip:
                    canvas.ClipPath(clip.Path);
                    break;
                case DrawColorCommand color:
                    canvas.DrawColor(color.Color);
                    break;
                case DrawRectCommand rect:
                    canvas.DrawRect(rect.Rect, rect.Paint);
                    break;
                case DrawPathCommand path:
                    canvas.DrawPath(path.Path, path.Paint);
                    break;
                case DrawImageCommand image:
                    if (!resources.TryGetValue(image.Resource, out var snapshot) || snapshot is not ImageResourceSnapshot rasterImage)
                    {
                        throw new InvalidOperationException($"Image resource {image.Resource.Value} is missing or has the wrong type.");
                    }
                    canvas.DrawImage(rasterImage, image.Source, image.Destination, image.Opacity);
                    break;
                case DrawTextCommand text:
                    canvas.DrawText(text.Text, text.Origin, text.FontSize, text.Paint, text.FontFamily);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown display command {command.GetType().Name}.");
            }
        }
        if (canvas.SaveCount != initialSaveCount)
        {
            throw new InvalidOperationException("DisplayList execution changed the canvas save count.");
        }
    }
}

public sealed class DisplayListBuilder
{
    private readonly Rect _cullRect;
    private readonly List<DisplayCommand> _commands = [];
    private int _saveDepth;
    private bool _built;

    public DisplayListBuilder(Rect cullRect)
    {
        if (!cullRect.IsFinite || cullRect.IsEmpty)
        {
            throw new ArgumentOutOfRangeException(nameof(cullRect), "Cull rectangle must be finite and non-empty.");
        }
        _cullRect = cullRect;
    }

    public Size CullSize => _cullRect.Size;

    public void Save()
    {
        EnsureMutable();
        _saveDepth++;
        _commands.Add(new SaveCommand());
    }

    public void Restore()
    {
        EnsureMutable();
        if (_saveDepth == 0)
        {
            throw new InvalidOperationException("DisplayList restore is unbalanced.");
        }
        _saveDepth--;
        _commands.Add(new RestoreCommand());
    }

    public void Transform(Matrix transform)
    {
        EnsureMutable();
        if (!transform.IsFinite)
        {
            throw new ArgumentException("Transform must be finite.", nameof(transform));
        }
        _commands.Add(new TransformCommand(transform));
    }

    public void ClipRect(Rect rect)
    {
        EnsureFinite(rect, nameof(rect));
        _commands.Add(new ClipRectCommand(rect));
    }

    public void ClipPath(PathGeometry path)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(path);
        _commands.Add(new ClipPathCommand(path));
    }

    public void DrawColor(Color color)
    {
        EnsureMutable();
        _commands.Add(new DrawColorCommand(color));
    }

    public void DrawRect(Rect rect, RasterPaint paint)
    {
        EnsureFinite(rect, nameof(rect));
        _commands.Add(new DrawRectCommand(rect, paint.Validate()));
    }

    public void DrawPath(PathGeometry path, RasterPaint paint)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(path);
        _commands.Add(new DrawPathCommand(path, paint.Validate()));
    }

    public void DrawImage(ResourceId resource, Rect source, Rect destination, double opacity = 1)
    {
        EnsureFinite(source, nameof(source));
        EnsureFinite(destination, nameof(destination));
        if (resource.Value == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(resource), "Resource identifier zero is reserved.");
        }
        ValidateOpacity(opacity);
        _commands.Add(new DrawImageCommand(resource, source, destination, opacity));
    }

    public void DrawText(string text, Offset origin, double fontSize, RasterPaint paint, string? fontFamily = null)
    {
        EnsureMutable();
        ArgumentNullException.ThrowIfNull(text);
        if (text.Length == 0)
        {
            return;
        }
        if (!origin.IsFinite || !double.IsFinite(fontSize) || fontSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fontSize), "Text geometry must be finite and positive.");
        }
        _commands.Add(new DrawTextCommand(text, origin, fontSize, paint.Validate(), fontFamily));
    }

    public DisplayList Build()
    {
        EnsureMutable();
        if (_saveDepth != 0)
        {
            throw new InvalidOperationException($"DisplayList has {_saveDepth} unmatched save command(s).");
        }
        _built = true;
        var resources = _commands.OfType<DrawImageCommand>()
            .Select(command => command.Resource)
            .Distinct()
            .ToArray();
        return new(_commands.ToArray(), resources, ComputeBounds(), EstimateByteSize());
    }

    private Rect ComputeBounds()
    {
        var matrix = Matrix.Identity;
        var clip = _cullRect;
        var stack = new Stack<(Matrix Transform, Rect Clip)>();
        var bounds = Rect.Zero;
        foreach (var command in _commands)
        {
            Rect? drawn = null;
            switch (command)
            {
                case SaveCommand:
                    stack.Push((matrix, clip));
                    break;
                case RestoreCommand:
                    (matrix, clip) = stack.Pop();
                    break;
                case TransformCommand transform:
                    matrix *= transform.Transform;
                    break;
                case ClipRectCommand clipCommand:
                    clip = clip.Intersect(matrix.TransformBounds(clipCommand.Rect));
                    break;
                case ClipPathCommand clipCommand:
                    clip = clip.Intersect(matrix.TransformBounds(clipCommand.Path.Bounds));
                    break;
                case DrawColorCommand:
                    drawn = clip;
                    break;
                case DrawRectCommand rect:
                    drawn = matrix.TransformBounds(rect.Rect).Intersect(clip);
                    break;
                case DrawPathCommand path:
                    drawn = matrix.TransformBounds(path.Path.Bounds).Intersect(clip);
                    break;
                case DrawImageCommand image:
                    drawn = matrix.TransformBounds(image.Destination).Intersect(clip);
                    break;
                case DrawTextCommand text:
                    var textRect = Rect.FromLeftTopWidthHeight(
                        text.Origin.X,
                        text.Origin.Y - text.FontSize,
                        text.Text.Length * text.FontSize * 0.6,
                        text.FontSize);
                    drawn = matrix.TransformBounds(textRect).Intersect(clip);
                    break;
            }
            if (drawn is { IsEmpty: false } value)
            {
                bounds = bounds.ExpandToInclude(value);
            }
        }
        return bounds;
    }

    private int EstimateByteSize() => _commands.Sum(command => command switch
    {
        SaveCommand or RestoreCommand => 1,
        TransformCommand => 1 + (16 * sizeof(double)),
        ClipRectCommand or DrawRectCommand => 1 + (4 * sizeof(double)) + sizeof(uint) + sizeof(double),
        ClipPathCommand path => 1 + (path.Path.Points.Count * 2 * sizeof(double)),
        DrawColorCommand => 1 + sizeof(uint),
        DrawPathCommand path => 1 + (path.Path.Points.Count * 2 * sizeof(double)) + sizeof(uint) + sizeof(double),
        DrawImageCommand => 1 + sizeof(ulong) + (8 * sizeof(double)) + sizeof(double),
        DrawTextCommand text => 1 + ((text.Text.Length + (text.FontFamily?.Length ?? 0)) * sizeof(char)) + (3 * sizeof(double)) + sizeof(uint),
        _ => 1,
    });

    private void EnsureFinite(Rect rect, string parameterName)
    {
        EnsureMutable();
        if (!rect.IsFinite)
        {
            throw new ArgumentException("Rectangle must be finite.", parameterName);
        }
    }

    private void EnsureMutable()
    {
        if (_built)
        {
            throw new InvalidOperationException("A built DisplayListBuilder is immutable.");
        }
    }

    private static void ValidateOpacity(double opacity)
    {
        if (!double.IsFinite(opacity) || opacity is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(opacity));
        }
    }
}
