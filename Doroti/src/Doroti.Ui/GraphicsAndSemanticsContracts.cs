using Doroti.Runtime;

namespace Doroti.Ui;

public class Color : IEquatable<Color>
{
    protected Color() : this(0U) { }
    public Color(uint value) => this.value = value;
    public Color(long value) : this(unchecked((uint)value)) { }
    public Color(double alpha, double red, double green, double blue, ColorSpace? colorSpace = null)
        : this((uint)((ClampChannel(alpha) << 24) | (ClampChannel(red) << 16) | (ClampChannel(green) << 8) | ClampChannel(blue))) => this.colorSpace = colorSpace ?? ColorSpace.sRGB;
    public uint value { get; }
    public ColorSpace colorSpace { get; } = ColorSpace.sRGB;
    public static Color fromARGB(long alpha, long red, long green, long blue) => new(
        ((uint)Math.Clamp(alpha, 0, 255) << 24) |
        ((uint)Math.Clamp(red, 0, 255) << 16) |
        ((uint)Math.Clamp(green, 0, 255) << 8) |
        (uint)Math.Clamp(blue, 0, 255));
    public static Color CreateFromARGB(long alpha, long red, long green, long blue) => fromARGB(alpha, red, green, blue);
    public static Color fromRGBO(long red, long green, long blue, double opacity) => fromARGB((long)Math.Round(Math.Clamp(opacity, 0, 1) * 255), red, green, blue);
    public static Color from(double alpha, double red, double green, double blue, ColorSpace colorSpace = ColorSpace.sRGB) => new(alpha, red, green, blue, colorSpace);
    public static Color? lerp(Color? a, Color? b, double t)
    {
        if (a is null && b is null) return null;
        var begin = a ?? new Color(0U);
        var end = b ?? new Color(0U);
        return fromARGB(
            (long)Math.Round(begin.alpha + ((end.alpha - begin.alpha) * t)),
            (long)Math.Round(begin.red + ((end.red - begin.red) * t)),
            (long)Math.Round(begin.green + ((end.green - begin.green) * t)),
            (long)Math.Round(begin.blue + ((end.blue - begin.blue) * t)));
    }
    public int alpha => (int)((value >> 24) & 0xff);
    public int red => (int)((value >> 16) & 0xff);
    public int green => (int)((value >> 8) & 0xff);
    public int blue => (int)(value & 0xff);
    public double a => alpha / 255d;
    public double r => red / 255d;
    public double g => green / 255d;
    public double b => blue / 255d;
    public double opacity => a;
    public long toARGB32() => value;
    public Color withAlpha(long alpha) => new((value & 0x00ffffffU) | ((uint)Math.Clamp(alpha, 0, 255) << 24));
    public Color withRed(long red) => fromARGB(alpha, red, green, blue);
    public Color withGreen(long green) => fromARGB(alpha, red, green, blue);
    public Color withBlue(long blue) => fromARGB(alpha, red, green, blue);
    public Color withOpacity(double opacity) => withAlpha((long)Math.Round(Math.Clamp(opacity, 0, 1) * 255));
    public Color withValues(double? alpha = null, double? red = null, double? green = null, double? blue = null, ColorSpace? colorSpace = null) =>
        new(alpha ?? a, red ?? r, green ?? g, blue ?? b, colorSpace ?? this.colorSpace);
    public virtual Color resolveFrom<TContext>(TContext context) => this;
    public double computeLuminance()
    {
        static double Linearize(int channel)
        {
            var component = channel / 255d;
            return component <= 0.03928 ? component / 12.92 : Math.Pow((component + 0.055) / 1.055, 2.4);
        }
        return (0.2126 * Linearize(red)) + (0.7152 * Linearize(green)) + (0.0722 * Linearize(blue));
    }
    public bool Equals(Color? other) => other is not null && value == other.value;
    public override bool Equals(object? obj) => obj is Color other && Equals(other);
    public override int GetHashCode() => value.GetHashCode();
    public static bool operator ==(Color? left, Color? right) => Equals(left, right);
    public static bool operator !=(Color? left, Color? right) => !Equals(left, right);
    private static int ClampChannel(double value) => (int)Math.Round(Math.Clamp(value, 0, 1) * 255);
}

public enum ColorSpace { sRGB, extendedSRGB }

public enum PaintingStyle
{
    fill,
    stroke,
}

public enum StrokeCap { butt, round, square }
public enum StrokeJoin { miter, round, bevel }

public sealed class Paint
{
    public Color color { get; set; } = new(0xFF000000);

    public PaintingStyle style { get; set; } = PaintingStyle.fill;

    public double strokeWidth { get; set; }
    public StrokeCap strokeCap { get; set; } = StrokeCap.butt;
    public StrokeJoin strokeJoin { get; set; } = StrokeJoin.miter;
    public bool isAntiAlias { get; set; } = true;
    public BlendMode blendMode { get; set; } = BlendMode.srcOver;
    public Shader? shader { get; set; }
    public ColorFilter? colorFilter { get; set; }
    public MaskFilter? maskFilter { get; set; }
    public FilterQuality filterQuality { get; set; } = FilterQuality.none;
    public bool invertColors { get; set; }
}

public sealed class Path
{
    private readonly List<PathCommand> _commands = [];

    public IReadOnlyList<PathCommand> Commands => _commands;

    public void moveTo(double x, double y) => _commands.Add(new("moveTo", [x, y]));

    public void lineTo(double x, double y) => _commands.Add(new("lineTo", [x, y]));

    public void close() => _commands.Add(new("close", []));
    public PathFillType fillType { get; set; }
    public void reset() => _commands.Clear();
    public void relativeMoveTo(double dx, double dy) => _commands.Add(new("relativeMoveTo", [dx, dy]));
    public void relativeLineTo(double dx, double dy) => _commands.Add(new("relativeLineTo", [dx, dy]));
    public void quadraticBezierTo(double x1, double y1, double x2, double y2) => _commands.Add(new("quadraticBezierTo", [x1, y1, x2, y2]));
    public void conicTo(double x1, double y1, double x2, double y2, double weight) => _commands.Add(new("conicTo", [x1, y1, x2, y2, weight]));
    public void cubicTo(double x1, double y1, double x2, double y2, double x3, double y3) => _commands.Add(new("cubicTo", [x1, y1, x2, y2, x3, y3]));
    public void addRect(Rect rect) => _commands.Add(new("addRect", [rect.left, rect.top, rect.right, rect.bottom]));
    public void addOval(Rect oval) => _commands.Add(new("addOval", [oval.left, oval.top, oval.right, oval.bottom]));
    public void addArc(Rect oval, double startAngle, double sweepAngle) => _commands.Add(new("addArc", [oval.left, oval.top, oval.right, oval.bottom, startAngle, sweepAngle]));
    public void addRRect(RRect rrect) => _commands.Add(new("addRRect", [
        rrect.left, rrect.top, rrect.right, rrect.bottom,
        rrect.tlRadiusX, rrect.tlRadiusY,
        rrect.trRadiusX, rrect.trRadiusY,
        rrect.brRadiusX, rrect.brRadiusY,
        rrect.blRadiusX, rrect.blRadiusY]));
    public void addRSuperellipse(RSuperellipse rse) => _commands.Add(new("addRSuperellipse", [
        rse.outerRect.left, rse.outerRect.top, rse.outerRect.right, rse.outerRect.bottom,
        rse.tlRadiusX, rse.tlRadiusY,
        rse.trRadiusX, rse.trRadiusY,
        rse.brRadiusX, rse.brRadiusY,
        rse.blRadiusX, rse.blRadiusY]));
    public void addPolygon(IReadOnlyList<Offset> points, bool close) { foreach (var point in points) lineTo(point.dx, point.dy); if (close) this.close(); }
    public void addPath(Path path, Offset offset, Matrix4? matrix4 = null) => _commands.AddRange(path.Commands);
    public Path shift(Offset offset)
    {
        var shifted = new Path { fillType = fillType };
        foreach (var command in _commands)
        {
            var values = command.Arguments.ToArray();
            switch (command.Operation)
            {
                case "moveTo":
                case "lineTo":
                case "quadraticBezierTo":
                case "conicTo":
                case "cubicTo":
                    for (var index = 0; index + 1 < values.Length; index += 2)
                    {
                        values[index] += offset.dx;
                        values[index + 1] += offset.dy;
                    }
                    break;
                case "addRect":
                case "addOval":
                case "addRRect":
                case "addRSuperellipse":
                    if (values.Length >= 4)
                    {
                        values[0] += offset.dx;
                        values[1] += offset.dy;
                        values[2] += offset.dx;
                        values[3] += offset.dy;
                    }
                    break;
            }
            shifted._commands.Add(new PathCommand(command.Operation, values));
        }
        return shifted;
    }
    public Rect getBounds()
    {
        var points = _commands
            .Where(command => command.Operation is "moveTo" or "lineTo" && command.Arguments.Count >= 2)
            .Select(command => new Offset(command.Arguments[0], command.Arguments[1]))
            .Concat(_commands
                .Where(command => command.Operation is "addRect" or "addOval" or "addRRect" or "addRSuperellipse" && command.Arguments.Count >= 4)
                .SelectMany(command => new[]
                {
                    new Offset(command.Arguments[0], command.Arguments[1]),
                    new Offset(command.Arguments[2], command.Arguments[3]),
                }))
            .ToArray();
        return points.Length == 0
            ? Rect.zero
            : Rect.fromLTRB(points.Min(point => point.dx), points.Min(point => point.dy), points.Max(point => point.dx), points.Max(point => point.dy));
    }
    public void arcToPoint(Offset arcEnd, Radius? radius = null, double rotation = 0, bool largeArc = false, bool clockwise = true) => _commands.Add(new("arcToPoint", [arcEnd.dx, arcEnd.dy, radius?.x ?? 0, radius?.y ?? 0, rotation, largeArc ? 1 : 0, clockwise ? 1 : 0]));
    public void arcTo(Rect oval, double startAngle, double sweepAngle, bool forceMoveTo) =>
        _commands.Add(new("arcTo", [oval.left, oval.top, oval.right, oval.bottom, startAngle, sweepAngle, forceMoveTo ? 1 : 0]));
    public Path transform(IReadOnlyList<double> matrix4) { var result = new Path { fillType = fillType }; result._commands.AddRange(_commands); return result; }
    public bool contains(Offset point)
    {
        var shapeMatches = _commands
            .Where(command => command.Arguments.Count >= 4)
            .Select(command => command.Operation switch
            {
                "addRect" => new Rect(command.Arguments[0], command.Arguments[1], command.Arguments[2], command.Arguments[3]).contains(point),
                "addOval" => OvalContains(command.Arguments, point),
                "addRRect" => RoundedRectContains(command.Arguments, point),
                "addRSuperellipse" => new Rect(command.Arguments[0], command.Arguments[1], command.Arguments[2], command.Arguments[3]).contains(point),
                _ => false,
            })
            .ToArray();
        var vertices = _commands
            .Where(command => command.Operation is "moveTo" or "lineTo" && command.Arguments.Count >= 2)
            .Select(command => new Offset(command.Arguments[0], command.Arguments[1]))
            .ToArray();
        var polygonMatch = false;
        if (vertices.Length >= 3)
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                var j = i == 0 ? vertices.Length - 1 : i - 1;
                var a = vertices[i];
                var b = vertices[j];
                if ((a.dy > point.dy) != (b.dy > point.dy) &&
                    point.dx < ((b.dx - a.dx) * (point.dy - a.dy) / (b.dy - a.dy)) + a.dx)
                {
                    polygonMatch = !polygonMatch;
                }
            }
        }
        var matchCount = shapeMatches.Count(match => match) + (polygonMatch ? 1 : 0);
        return fillType == PathFillType.evenOdd ? (matchCount & 1) != 0 : matchCount != 0;
    }

    private static bool OvalContains(IReadOnlyList<double> values, Offset point)
    {
        var centerX = (values[0] + values[2]) / 2;
        var centerY = (values[1] + values[3]) / 2;
        var radiusX = Math.Abs(values[2] - values[0]) / 2;
        var radiusY = Math.Abs(values[3] - values[1]) / 2;
        if (radiusX == 0 || radiusY == 0) return false;
        var x = (point.dx - centerX) / radiusX;
        var y = (point.dy - centerY) / radiusY;
        return (x * x) + (y * y) <= 1;
    }

    private static bool RoundedRectContains(IReadOnlyList<double> values, Offset point)
    {
        if (values.Count < 12) return false;
        return new RRect(
            new Rect(values[0], values[1], values[2], values[3]),
            new Radius(values[4], values[5]),
            new Radius(values[6], values[7]),
            new Radius(values[8], values[9]),
            new Radius(values[10], values[11])).contains(point);
    }
}

public sealed class SystemColorPalette
{
    public SystemColorValue accentColor { get; init; } = new(new Color(0xff6750a4));
    public SystemColorValue accentColorText { get; init; } = new(new Color(0xffffffff));
    public SystemColorValue canvas { get; init; } = new(new Color(0xfffffbfe));
    public SystemColorValue canvasText { get; init; } = new(new Color(0xff1c1b1f));
    public SystemColorValue buttonFace { get; init; } = new(null);
    public SystemColorValue buttonBorder { get; init; } = new(null);
    public SystemColorValue buttonText { get; init; } = new(null);
    public SystemColorValue field { get; init; } = new(null);
    public SystemColorValue fieldText { get; init; } = new(null);
}

public sealed record SystemColorValue(Color? value);

public static class SystemColor
{
    public static bool platformProvidesSystemColors => false;
    public static SystemColorPalette light { get; } = new();
    public static SystemColorPalette dark { get; } = new()
    {
        accentColor = new(new Color(0xffd0bcff)),
        accentColorText = new(new Color(0xff381e72)),
        canvas = new(new Color(0xff1c1b1f)),
        canvasText = new(new Color(0xffe6e1e5)),
    };
}

public sealed record PathCommand(string Operation, IReadOnlyList<double> Arguments)
{
    internal object? HostPayload { get; init; }
}

public interface ISceneHostCapability
{
    void Submit(ulong viewId, DorotiSceneSubmission submission, DartUiInvocation invocation);
}

/// <summary>
/// Immutable hand-off from framework composition to a renderer. A null build
/// token means the scene was submitted outside a dispatcher frame and must not
/// be accepted as resize-exact by a resize-aware host.
/// </summary>
public sealed record DorotiSceneSubmission(
    Scene Scene,
    DorotiSceneBuildToken? BuildToken,
    DorotiFrameTransaction? FrameTransaction = null);

public sealed class Scene : IDisposable
{
    private int _disposed;

    public Scene(ulong viewId, IReadOnlyList<SceneCommand> commands)
        : this(viewId, commands?.ToArray() ?? throw new ArgumentNullException(nameof(commands)))
    {
    }

    private Scene(ulong viewId, SceneCommand[] ownedCommands)
    {
        this.viewId = viewId;
        Commands = ownedCommands;
    }

    internal static Scene FromOwnedCommands(ulong viewId, SceneCommand[] ownedCommands) =>
        new(viewId, ownedCommands ?? throw new ArgumentNullException(nameof(ownedCommands)));

    public ulong viewId { get; }

    public IReadOnlyList<SceneCommand> Commands { get; }

    public bool debugDisposed => Volatile.Read(ref _disposed) != 0;

    public Doroti.Runtime.Future<Image> toImage(long width, long height) =>
        Doroti.Runtime.Future<Image>.value(toImageSync(width, height));

    public Image toImageSync(long width, long height)
    {
        ObjectDisposedException.ThrowIf(debugDisposed, this);
        return new Image(viewId, checked((int)width), checked((int)height));
    }

    public void Dispose() => Interlocked.Exchange(ref _disposed, 1);
    public void dispose() => Dispose();
}

public sealed record SceneCommand(string Operation, object? Payload)
{
    internal object? HostPayload { get; init; }
}

internal sealed record ScenePicturePayload(
    long SnapshotIdentity,
    Offset Offset,
    IReadOnlyList<PathCommand> Commands,
    Rect? CanvasBounds,
    bool IsComplexHint,
    bool WillChangeHint);
internal sealed record SceneOffsetPayload(double Dx, double Dy);
internal sealed record SceneClipRectPayload(Rect Rect);
internal sealed record SceneClipRRectPayload(RRect RRect);
internal sealed record SceneClipRSuperellipsePayload(RSuperellipse RSuperellipse);
internal sealed record SceneClipPathPayload(Path Path);
internal sealed record SceneTransformPayload(IReadOnlyList<double> Matrix4);
internal sealed record SceneOpacityPayload(double Opacity, Offset Offset);
internal sealed record SceneColorFilterPayload(ColorFilterSnapshot Filter);
internal sealed record SceneImageFilterPayload(
    ImageFilterSnapshot Filter,
    Offset Offset,
    Rect? Bounds,
    object? CacheKey = null,
    long CacheGeneration = 0);
internal sealed record SceneShaderMaskPayload(ShaderSnapshot Shader, Rect MaskRect, BlendMode BlendMode);
internal sealed record SceneBackdropFilterPayload(ImageFilterSnapshot Filter, BlendMode BlendMode, object? BackdropId);
internal sealed record SceneRetainedPayload(IReadOnlyList<SceneCommand> Commands, ulong ViewId, long Generation);
internal sealed record CanvasSaveLayerPayload(Rect? Bounds, PaintSnapshot Paint);
internal sealed record CanvasPathPayload(Path Path, PaintSnapshot Paint);
internal sealed record CanvasRectPayload(Rect Rect, PaintSnapshot Paint);
internal sealed record CanvasRRectPayload(RRect RRect, PaintSnapshot Paint);
internal sealed record CanvasRSuperellipsePayload(RSuperellipse RSuperellipse, PaintSnapshot Paint);
internal sealed record CanvasDRRectPayload(RRect Outer, RRect Inner, PaintSnapshot Paint);
internal sealed record CanvasClipRRectPayload(RRect RRect);
internal sealed record CanvasClipRSuperellipsePayload(RSuperellipse RSuperellipse, bool DoAntiAlias);
internal sealed record CanvasClipPathPayload(Path Path);
internal sealed record CanvasImagePayload(Image Image, Rect Source, Rect Destination, PaintSnapshot Paint);
internal sealed record CanvasImageNinePayload(Image Image, Rect Center, Rect Destination, PaintSnapshot Paint);
internal sealed record CanvasParagraphPayload(Paragraph Paragraph, Offset Offset);
internal sealed record CanvasShadowPayload(Path Path, Color Color, double Elevation, bool TransparentOccluder);
internal sealed record CanvasCirclePayload(Offset Center, double Radius, PaintSnapshot Paint);
internal sealed record CanvasLinePayload(Offset Start, Offset End, PaintSnapshot Paint);
internal sealed record CanvasPointsPayload(PointMode PointMode, IReadOnlyList<Offset> Points, PaintSnapshot Paint);
internal sealed record CanvasOvalPayload(Rect Rect, PaintSnapshot Paint);
internal sealed record CanvasArcPayload(Rect Rect, double StartAngle, double SweepAngle, bool UseCenter, PaintSnapshot Paint);
internal sealed record CanvasColorPayload(Color Color, BlendMode BlendMode);
internal sealed record PaintSnapshot(
    Color Color,
    PaintingStyle Style,
    double StrokeWidth,
    StrokeCap StrokeCap,
    StrokeJoin StrokeJoin,
    bool IsAntiAlias,
    BlendMode BlendMode,
    ShaderSnapshot? Shader,
    ColorFilterSnapshot? ColorFilter,
    MaskFilter? MaskFilter,
    FilterQuality FilterQuality,
    bool InvertColors)
{
    internal static PaintSnapshot Capture(Paint paint)
    {
        ArgumentNullException.ThrowIfNull(paint);
        return new(
            paint.color,
            paint.style,
            paint.strokeWidth,
            paint.strokeCap,
            paint.strokeJoin,
            paint.isAntiAlias,
            paint.blendMode,
            paint.shader is null ? null : ShaderSnapshot.Capture(paint.shader),
            paint.colorFilter is null ? null : ColorFilterSnapshot.Capture(paint.colorFilter),
            paint.maskFilter,
            paint.filterQuality,
            paint.invertColors);
    }
}

internal abstract record ShaderSnapshot
{
    internal static ShaderSnapshot Capture(Shader shader) => shader switch
    {
        Gradient gradient => new GradientShaderSnapshot(
            gradient.begin, gradient.end, gradient.center, gradient.radius, gradient.focal, gradient.focalRadius,
            gradient.startAngle, gradient.endAngle, gradient.tileMode,
            Array.AsReadOnly(gradient.colors.ToArray()), Array.AsReadOnly(gradient.colorStops.ToArray()),
            gradient.matrix4 is null ? null : Array.AsReadOnly(gradient.matrix4.ToArray())),
        ImageShader image => new ImageShaderSnapshot(image.image, image.tmx, image.tmy,
            Array.AsReadOnly(image.matrix4.storage.ToArray()), image.filterQuality),
        FragmentShader fragment => new FragmentShaderSnapshot(fragment.CaptureState()),
        _ => new UnsupportedShaderSnapshot(shader.GetType().FullName ?? shader.GetType().Name),
    };
}

internal sealed record GradientShaderSnapshot(
    Offset? Begin, Offset? End, Offset? Center, double Radius, Offset? Focal, double FocalRadius,
    double StartAngle, double EndAngle, TileMode TileMode, IReadOnlyList<Color> Colors,
    IReadOnlyList<double> Stops, IReadOnlyList<double>? Matrix4) : ShaderSnapshot;
internal sealed record ImageShaderSnapshot(
    Image Image, TileMode TileModeX, TileMode TileModeY, IReadOnlyList<double> Matrix4,
    FilterQuality? FilterQuality) : ShaderSnapshot;
internal sealed record FragmentShaderSnapshot(FragmentShaderState State) : ShaderSnapshot;
internal sealed record UnsupportedShaderSnapshot(string Family) : ShaderSnapshot;

internal sealed record ColorFilterSnapshot(
    ColorFilterKind Kind,
    Color? Color,
    BlendMode BlendMode,
    IReadOnlyList<double>? Matrix)
{
    internal static ColorFilterSnapshot Capture(ColorFilter filter) => new(
        filter.kind,
        filter.color,
        filter.blendMode,
        filter.matrixValues is null ? null : Array.AsReadOnly(filter.matrixValues.ToArray()));
}

internal sealed record ImageFilterSnapshot(
    double SigmaX,
    double SigmaY,
    TileMode TileMode,
    Rect? Bounds,
    ImageFilterSnapshot? Outer,
    ImageFilterSnapshot? Inner,
    ColorFilterSnapshot? ColorFilter,
    IReadOnlyList<double>? Matrix4,
    FilterQuality FilterQuality,
    ShaderSnapshot? Shader)
{
    internal static ImageFilterSnapshot Capture(ImageFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        if (filter.shader is not null)
            return new(0, 0, TileMode.clamp, null, null, null, null, null, filter.filterQuality,
                ShaderSnapshot.Capture(filter.shader));
        return new(
            filter.sigmaX,
            filter.sigmaY,
            filter.tileMode,
            filter.bounds,
            filter.outer is null ? null : Capture(filter.outer),
            filter.inner is null ? null : Capture(filter.inner),
            filter.colorFilter is null ? null : ColorFilterSnapshot.Capture(filter.colorFilter),
            filter.matrix4 is null ? null : Array.AsReadOnly(filter.matrix4.ToArray()),
            filter.filterQuality,
            null);
    }
}
internal interface IDorotiImageHandle
{
    IDorotiImageHandle Clone();

    void Release();
}

public sealed record RetainedResourceDiagnostics(
    long EngineLayersCreated,
    long EngineLayersDisposed,
    long ActiveEngineLayers,
    long RetainedSnapshots,
    long RetainedReuses);

public class EngineLayer : IDisposable
{
    private static long _created;
    private static long _disposedCount;
    private static long _active;
    private static long _retainedSnapshots;
    private static long _retainedReuses;
    private int _disposed;

    public EngineLayer()
    {
        Interlocked.Increment(ref _created);
        Interlocked.Increment(ref _active);
    }

    internal string? Operation { get; set; }
    internal IReadOnlyList<SceneCommand>? RetainedCommands { get; set; }
    internal ulong OwnerViewId { get; set; }
    internal long Generation { get; set; }
    public long debugGeneration => Generation;
    public bool debugDisposed => Volatile.Read(ref _disposed) != 0;
    public static RetainedResourceDiagnostics debugResourceDiagnostics => new(
        Interlocked.Read(ref _created),
        Interlocked.Read(ref _disposedCount),
        Interlocked.Read(ref _active),
        Interlocked.Read(ref _retainedSnapshots),
        Interlocked.Read(ref _retainedReuses));

    internal static void RecordSnapshot() => Interlocked.Increment(ref _retainedSnapshots);
    internal static void RecordReuse() => Interlocked.Increment(ref _retainedReuses);

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        RetainedCommands = null;
        Interlocked.Increment(ref _disposedCount);
        Interlocked.Decrement(ref _active);
    }
    public void dispose() => Dispose();
}

public sealed class OffsetEngineLayer : EngineLayer;
public sealed class ClipRectEngineLayer : EngineLayer;
public sealed class ClipRRectEngineLayer : EngineLayer;
public sealed class ClipRSuperellipseEngineLayer : EngineLayer;
public sealed class ClipPathEngineLayer : EngineLayer;
public sealed class ColorFilterEngineLayer : EngineLayer;
public sealed class ImageFilterEngineLayer : EngineLayer;
public sealed class TransformEngineLayer : EngineLayer;
public sealed class OpacityEngineLayer : EngineLayer;
public sealed class ShaderMaskEngineLayer : EngineLayer;
public sealed class BackdropFilterEngineLayer : EngineLayer;

public sealed class SceneBuilder
{
    private static long _nextRetainedGeneration;
    private readonly ulong _viewId;
    private readonly List<SceneCommand> _commands = [];
    private readonly Stack<(EngineLayer Layer, int Start)> _scopes = [];

    public SceneBuilder(ulong viewId = 0) => _viewId = viewId;

    public void addPicture(Offset offset, IReadOnlyList<PathCommand> picture) =>
        _commands.Add(new("picture", new { offset, picture }));

    public void addPicture(Offset offset, Picture picture, bool isComplexHint = false, bool willChangeHint = false) =>
        AddPicture(offset, picture, null, isComplexHint, willChangeHint);

    public void addPicture(
        Offset offset,
        Picture picture,
        Rect canvasBounds,
        bool isComplexHint = false,
        bool willChangeHint = false) =>
        AddPicture(offset, picture, canvasBounds, isComplexHint, willChangeHint);

    private void AddPicture(
        Offset offset,
        Picture picture,
        Rect? canvasBounds,
        bool isComplexHint,
        bool willChangeHint)
    {
        ArgumentNullException.ThrowIfNull(picture);
        ObjectDisposedException.ThrowIf(picture.debugDisposed, picture);
        var commands = picture.Commands;
        _commands.Add(new SceneCommand("picture", new { offset, picture = commands, isComplexHint, willChangeHint })
        {
            // Picture is a Dart-side handle whose dispose may run as soon as a
            // replacement layer is built. The scene owns the immutable command
            // snapshot so raster/replay never observes that handle's lifetime.
            HostPayload = new ScenePicturePayload(picture.SnapshotIdentity, offset, commands, canvasBounds, isComplexHint, willChangeHint),
        });
    }

    public OffsetEngineLayer pushOffset(
        double dx,
        double dy,
        OffsetEngineLayer? oldLayer = null) =>
        Push(oldLayer, "offset", new { dx, dy }, new SceneOffsetPayload(dx, dy));
    public ClipRectEngineLayer pushClipRect(Rect rect, Clip clipBehavior = Clip.antiAlias, ClipRectEngineLayer? oldLayer = null) =>
        Push(oldLayer, "clipRect", new { rect, clipBehavior }, new SceneClipRectPayload(rect));
    public ClipRRectEngineLayer pushClipRRect(RRect rrect, Clip clipBehavior = Clip.antiAlias, ClipRRectEngineLayer? oldLayer = null) =>
        Push(oldLayer, "clipRRect", new { rrect, clipBehavior }, new SceneClipRRectPayload(rrect));
    public ClipRSuperellipseEngineLayer pushClipRSuperellipse(RSuperellipse rse, Clip clipBehavior = Clip.antiAlias, ClipRSuperellipseEngineLayer? oldLayer = null) =>
        Push(oldLayer, "clipRSuperellipse", new { rse, clipBehavior }, new SceneClipRSuperellipsePayload(rse));
    public ClipPathEngineLayer pushClipPath(Path path, Clip clipBehavior = Clip.antiAlias, ClipPathEngineLayer? oldLayer = null) =>
        Push(oldLayer, "clipPath", new { path, clipBehavior }, new SceneClipPathPayload(path));
    public ColorFilterEngineLayer pushColorFilter(ColorFilter filter, ColorFilterEngineLayer? oldLayer = null) =>
        Push(oldLayer, "colorFilter", filter, new SceneColorFilterPayload(ColorFilterSnapshot.Capture(filter)));
    public ImageFilterEngineLayer pushImageFilter(
        ImageFilter filter,
        Offset offset = default,
        ImageFilterEngineLayer? oldLayer = null,
        Rect? bounds = null,
        object? cacheKey = null,
        long cacheGeneration = 0) =>
        Push(oldLayer, "imageFilter", new { filter, offset, bounds },
            new SceneImageFilterPayload(
                ImageFilterSnapshot.Capture(filter), offset, bounds, cacheKey, cacheGeneration));
    public TransformEngineLayer pushTransform(IReadOnlyList<double> matrix4, TransformEngineLayer? oldLayer = null) =>
        Push(oldLayer, "transform", matrix4, new SceneTransformPayload(matrix4));
    public OpacityEngineLayer pushOpacity(long alpha, Offset offset = default, OpacityEngineLayer? oldLayer = null) =>
        Push(oldLayer, "opacity", new { alpha, offset }, new SceneOpacityPayload(Math.Clamp(alpha, 0, 255) / 255d, offset));
    public ShaderMaskEngineLayer pushShaderMask(Shader shader, Rect maskRect, BlendMode blendMode, ShaderMaskEngineLayer? oldLayer = null) =>
        Push(oldLayer, "shaderMask", new { shader, maskRect, blendMode }, new SceneShaderMaskPayload(ShaderSnapshot.Capture(shader), maskRect, blendMode));
    public BackdropFilterEngineLayer pushBackdropFilter(ImageFilter filter, BlendMode blendMode = BlendMode.srcOver, BackdropFilterEngineLayer? oldLayer = null, object? backdropId = null) =>
        Push(oldLayer, "backdropFilter", new { filter, blendMode, backdropId }, new SceneBackdropFilterPayload(ImageFilterSnapshot.Capture(filter), blendMode, backdropId));
    public void pop()
    {
        if (_scopes.Count == 0) throw new InvalidOperationException("SceneBuilder pop is unbalanced.");
        _commands.Add(new("pop", null));
        var (layer, start) = _scopes.Pop();
        var retainedCount = _commands.Count - start;
        var retainedCommands = new SceneCommand[retainedCount];
        _commands.CopyTo(start, retainedCommands, 0, retainedCount);
        layer.RetainedCommands = retainedCommands;
        layer.Generation = Interlocked.Increment(ref _nextRetainedGeneration);
        EngineLayer.RecordSnapshot();
    }
    public void addRetained(EngineLayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);
        ObjectDisposedException.ThrowIf(layer.debugDisposed, layer);
        if (layer.RetainedCommands is null || layer.OwnerViewId != _viewId)
            throw new InvalidOperationException("A retained layer must be complete and owned by the same Flutter view.");
        _commands.Add(new SceneCommand("retained", layer) { HostPayload = new SceneRetainedPayload(layer.RetainedCommands, layer.OwnerViewId, layer.Generation) });
        EngineLayer.RecordReuse();
    }
    public void addPerformanceOverlay(long enabledOptions, Rect bounds) => _commands.Add(new("performanceOverlay", new { enabledOptions, bounds }));
    public void addPlatformView(long viewId, Offset offset = default, double width = 0, double height = 0) =>
        _commands.Add(new("platformView", new { viewId, offset, width, height }));
    public void addTexture(long textureId, Offset offset = default, double width = 0, double height = 0, bool freeze = false, FilterQuality filterQuality = FilterQuality.low) =>
        _commands.Add(new("texture", new { textureId, offset, width, height, freeze, filterQuality }));

    private T Push<T>(T? oldLayer, string operation, object? payload, object? hostPayload = null) where T : EngineLayer, new()
    {
        var layer = oldLayer is { debugDisposed: false } && oldLayer.Operation == operation ? oldLayer : new T();
        layer.Operation = operation;
        layer.OwnerViewId = _viewId;
        var start = _commands.Count;
        _commands.Add(new SceneCommand(operation, payload) { HostPayload = hostPayload });
        _scopes.Push((layer, start));
        return layer;
    }

    public Scene build()
    {
        if (_scopes.Count != 0) throw new InvalidOperationException($"SceneBuilder has {_scopes.Count} unclosed effect scope(s).");
        return Scene.FromOwnedCommands(_viewId, _commands.ToArray());
    }
}

public class Canvas
{
    private readonly List<PathCommand> _commands;
    private int _saveCount = 1;

    protected Canvas() => _commands = [];
    public Canvas(List<PathCommand> commands) => _commands = commands ?? throw new ArgumentNullException(nameof(commands));

    public void drawPath(Path path, Paint paint)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(paint);
        _commands.Add(new PathCommand("drawPath", [path.Commands.Count, paint.color.value, paint.strokeWidth])
        {
            HostPayload = new CanvasPathPayload(path, PaintSnapshot.Capture(paint)),
        });
    }

    public Canvas(PictureRecorder recorder, Rect? cullRect = null) : this(recorder.commands) { }
    public virtual object? noSuchMethod(Invocation invocation) =>
        throw new MissingMethodException($"Canvas does not implement the requested Dart invocation: {invocation}.");
    public void save() { _saveCount++; _commands.Add(new("save", [])); }
    public void restore() { if (_saveCount > 1) _saveCount--; _commands.Add(new("restore", [])); }
    public void saveLayer(Rect? bounds, Paint paint)
    {
        _saveCount++;
        _commands.Add(new PathCommand("saveLayer", []) { HostPayload = new CanvasSaveLayerPayload(bounds, PaintSnapshot.Capture(paint)) });
    }
    public long getSaveCount() => _saveCount;
    public void translate(double dx, double dy) => _commands.Add(new("translate", [dx, dy]));
    public void scale(double sx, double? sy = null) => _commands.Add(new("scale", [sx, sy ?? sx]));
    public void rotate(double radians) => _commands.Add(new("rotate", [radians]));
    public void skew(double sx, double sy) => _commands.Add(new("skew", [sx, sy]));
    public void transform(IReadOnlyList<double> matrix4) => _commands.Add(new("transform", matrix4));
    public void clipRect(Rect rect, Clip clipBehavior = Clip.antiAlias) => _commands.Add(new("clipRect", [rect.left, rect.top, rect.right, rect.bottom]));
    public void clipRect(Rect rect, ClipOp clipOp, bool doAntiAlias = true) =>
        _commands.Add(new("clipRect", [rect.left, rect.top, rect.right, rect.bottom, (double)clipOp, doAntiAlias ? 1 : 0]));
    public void clipRect(Rect rect, bool doAntiAlias) => _commands.Add(new("clipRect", [rect.left, rect.top, rect.right, rect.bottom, doAntiAlias ? 1 : 0]));
    public void clipRRect(RRect rrect, bool doAntiAlias = true) => _commands.Add(new PathCommand("clipRRect", [rrect.left, rrect.top, rrect.right, rrect.bottom])
    {
        HostPayload = new CanvasClipRRectPayload(rrect),
    });
    public void clipRSuperellipse(RSuperellipse rse, bool doAntiAlias = true) => _commands.Add(new PathCommand("clipRSuperellipse", [rse.outerRect.left, rse.outerRect.top, rse.outerRect.right, rse.outerRect.bottom])
    {
        HostPayload = new CanvasClipRSuperellipsePayload(rse, doAntiAlias),
    });
    public void clipPath(Path path, bool doAntiAlias = true) => _commands.Add(new PathCommand("clipPath", [path.Commands.Count])
    {
        HostPayload = new CanvasClipPathPayload(path),
    });
    public void drawRect(Rect rect, Paint paint) => _commands.Add(new PathCommand("drawRect", [rect.left, rect.top, rect.right, rect.bottom, paint.color.value])
    {
        HostPayload = new CanvasRectPayload(rect, PaintSnapshot.Capture(paint)),
    });
    public void drawRRect(RRect rrect, Paint paint) => _commands.Add(new PathCommand("drawRRect", [rrect.left, rrect.top, rrect.right, rrect.bottom])
    {
        HostPayload = new CanvasRRectPayload(rrect, PaintSnapshot.Capture(paint)),
    });
    public void drawRSuperellipse(RSuperellipse rse, Paint paint) => _commands.Add(new PathCommand("drawRSuperellipse", [rse.outerRect.left, rse.outerRect.top, rse.outerRect.right, rse.outerRect.bottom])
    {
        HostPayload = new CanvasRSuperellipsePayload(rse, PaintSnapshot.Capture(paint)),
    });
    public void drawDRRect(RRect outer, RRect inner, Paint paint) =>
        _commands.Add(new PathCommand("drawDRRect", [outer.left, outer.top, outer.right, outer.bottom, inner.left, inner.top, inner.right, inner.bottom])
        {
            HostPayload = new CanvasDRRectPayload(outer, inner, PaintSnapshot.Capture(paint)),
        });
    public void drawCircle(Offset center, double radius, Paint paint) => _commands.Add(new PathCommand("drawCircle", [center.dx, center.dy, radius])
    {
        HostPayload = new CanvasCirclePayload(center, radius, PaintSnapshot.Capture(paint)),
    });
    public void drawArc(Rect rect, double startAngle, double sweepAngle, bool useCenter, Paint paint) =>
        _commands.Add(new PathCommand("drawArc", [rect.left, rect.top, rect.right, rect.bottom, startAngle, sweepAngle, useCenter ? 1 : 0])
        {
            HostPayload = new CanvasArcPayload(rect, startAngle, sweepAngle, useCenter, PaintSnapshot.Capture(paint)),
        });
    public void drawLine(Offset point1, Offset point2, Paint paint) => _commands.Add(new PathCommand("drawLine", [point1.dx, point1.dy, point2.dx, point2.dy])
    {
        HostPayload = new CanvasLinePayload(point1, point2, PaintSnapshot.Capture(paint)),
    });
    public void drawPaint(Paint paint) => _commands.Add(new PathCommand("drawPaint", [paint.color.value])
    {
        HostPayload = PaintSnapshot.Capture(paint),
    });
    public void drawShadow(Path path, Color color, double elevation, bool transparentOccluder) =>
        _commands.Add(new PathCommand("drawShadow", [path.Commands.Count, color.value, elevation, transparentOccluder ? 1 : 0])
        {
            HostPayload = new CanvasShadowPayload(path, color, elevation, transparentOccluder),
        });
    public void drawColor(Color color, BlendMode blendMode) =>
        _commands.Add(new PathCommand("drawColor", [color.value, (double)blendMode]) { HostPayload = new CanvasColorPayload(color, blendMode) });
    public void drawImage(Image image, Offset offset, Paint paint) =>
        _commands.Add(new PathCommand("drawImage", [image.width, image.height, offset.dx, offset.dy])
        {
            HostPayload = new CanvasImagePayload(image, Rect.fromLTWH(0, 0, image.width, image.height),
                Rect.fromLTWH(offset.dx, offset.dy, image.width, image.height), PaintSnapshot.Capture(paint)),
        });
    public void drawPicture(Picture picture) => _commands.AddRange(picture.Commands);
    public void drawPoints(PointMode pointMode, IReadOnlyList<Offset> points, Paint paint)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(paint);
        var capturedPoints = Array.AsReadOnly(points.ToArray());
        _commands.Add(new PathCommand("drawPoints", [(double)pointMode, capturedPoints.Count, paint.color.value])
        {
            HostPayload = new CanvasPointsPayload(pointMode, capturedPoints, PaintSnapshot.Capture(paint)),
        });
    }
    public void drawRawPoints(PointMode pointMode, Float32List points, Paint paint)
    {
        ArgumentNullException.ThrowIfNull(points);
        ArgumentNullException.ThrowIfNull(paint);
        if ((points.Count & 1) != 0)
            throw new ArgumentException("Raw point coordinates must contain x/y pairs.", nameof(points));
        var capturedPoints = Array.AsReadOnly(Enumerable.Range(0, points.Count / 2)
            .Select(index => new Offset(points[index * 2], points[(index * 2) + 1]))
            .ToArray());
        _commands.Add(new PathCommand("drawRawPoints", [(double)pointMode, capturedPoints.Count, paint.color.value])
        {
            HostPayload = new CanvasPointsPayload(pointMode, capturedPoints, PaintSnapshot.Capture(paint)),
        });
    }
    public void drawVertices(Vertices vertices, BlendMode blendMode, Paint paint) =>
        _commands.Add(new("drawVertices", [(double)blendMode, paint.color.value]));
    public void drawAtlas(Image atlas, IReadOnlyList<RSTransform> transforms, IReadOnlyList<Rect> rects,
        IReadOnlyList<Color>? colors, BlendMode? blendMode, Rect? cullRect, Paint paint) =>
        _commands.Add(new("drawAtlas", [atlas.width, atlas.height, transforms.Count, rects.Count, colors?.Count ?? 0]));
    public void drawRawAtlas(Image atlas, Float32List rstTransforms, Float32List rects, Int32List? colors,
        BlendMode? blendMode, Rect? cullRect, Paint paint) =>
        _commands.Add(new("drawRawAtlas", [atlas.width, atlas.height, rstTransforms.Count, rects.Count, colors?.Count ?? 0]));
    public void drawOval(Rect rect, Paint paint) => _commands.Add(new PathCommand("drawOval", [rect.left, rect.top, rect.right, rect.bottom])
    {
        HostPayload = new CanvasOvalPayload(rect, PaintSnapshot.Capture(paint)),
    });
    public void drawImageRect(Image image, Rect src, Rect dst, Paint paint) => _commands.Add(new PathCommand("drawImageRect", [image.width, image.height, src.left, src.top, src.right, src.bottom, dst.left, dst.top, dst.right, dst.bottom])
    {
        HostPayload = new CanvasImagePayload(image, src, dst, PaintSnapshot.Capture(paint)),
    });
    public void drawImageNine(Image image, Rect center, Rect dst, Paint paint) =>
        _commands.Add(new PathCommand(
            "drawImageNine",
            [image.width, image.height, center.left, center.top, center.right, center.bottom,
                dst.left, dst.top, dst.right, dst.bottom])
        {
            HostPayload = new CanvasImageNinePayload(
                image, center, dst, PaintSnapshot.Capture(paint)),
        });
    public void drawParagraph(Paragraph paragraph, Offset offset) => _commands.Add(new PathCommand("drawParagraph", [offset.dx, offset.dy, paragraph.width, paragraph.height])
    {
        HostPayload = new CanvasParagraphPayload(paragraph, offset),
    });
}

public interface IParagraphHostCapability
{
    Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation);
}

public sealed record ParagraphRequest(
    string Text,
    double Width,
    string? FontFamily,
    double FontSize,
    long? MaxLines = null,
    Color? Color = null,
    double? Height = null,
    TextAlign? TextAlign = null,
    TextDirection? TextDirection = null,
    Locale? Locale = null,
    string? Ellipsis = null,
    IReadOnlyList<ParagraphTextRun>? TextRuns = null);

internal sealed record ParagraphHostLineSnapshot(
    int Start,
    int End,
    bool HardBreak,
    double Ascent,
    double Descent,
    double Height,
    double Width,
    double Left,
    double Baseline);

internal sealed record ParagraphHostGraphemeSnapshot(
    int Start,
    int End,
    double Left,
    double Top,
    double Right,
    double Bottom,
    double StrutTop,
    double StrutBottom,
    TextDirection Direction);

internal sealed record ParagraphHostLayoutSnapshot(
    double Width,
    double Height,
    double AlphabeticBaseline,
    double IdeographicBaseline,
    double MinIntrinsicWidth,
    double MaxIntrinsicWidth,
    double LongestLine,
    bool DidExceedMaxLines,
    ulong MetricsHash,
    IReadOnlyList<double> CodeUnitAdvances,
    IReadOnlyList<ParagraphHostLineSnapshot> Lines,
    IReadOnlyList<ParagraphHostGraphemeSnapshot> Graphemes);

public sealed class Paragraph : IDisposable
{
    private int _disposed;
    private double _naturalWidth;
    private double _lineHeight;
    private readonly long? _maxLines;
    private readonly int[] _graphemeStarts;
    private double[] _codeUnitAdvances;
    private List<ParagraphLine> _lines = [];
    private ParagraphHostGraphemeSnapshot[]? _hostGraphemes;
    private double? _hostMinIntrinsicWidth;
    private double? _hostMaxIntrinsicWidth;
    private double? _hostLongestLine;
    private double? _hostAlphabeticBaseline;
    private double? _hostIdeographicBaseline;
    public Paragraph(
        string text,
        double width,
        double height,
        double fontSize = 14,
        long? maxLines = null,
        string? fontFamily = null,
        Color? color = null,
        IReadOnlyList<double>? codeUnitAdvances = null,
        IReadOnlyList<ParagraphTextRun>? textRuns = null)
    {
        this.text = text;
        this.fontSize = Math.Max(1, fontSize);
        this.fontFamily = fontFamily;
        this.color = color ?? new Color(0xFF000000);
        TextRuns = (textRuns ?? []).ToArray();
        _graphemeStarts = System.Globalization.StringInfo.ParseCombiningCharacters(text);
        _lineHeight = height > 0 ? height : this.fontSize * 1.2;
        _codeUnitAdvances = codeUnitAdvances is { Count: var count } && count == text.Length
            ? codeUnitAdvances.Select(value => Math.Max(0, value)).ToArray()
            : CreateFallbackAdvances(text, this.fontSize);
        _naturalWidth = ComputeNaturalWidth(text, _codeUnitAdvances);
        _maxLines = maxLines;
        this.width = width;
        this.height = height;
        CanvasKitMetricsHash = ComputeMetricsHash(text, width, height, this.fontSize, _codeUnitAdvances);
    }
    public string text { get; }
    public double fontSize { get; }
    public string? fontFamily { get; }
    public Color color { get; }
    public double width { get; private set; }
    public double height { get; private set; }
    public double minIntrinsicWidth => _hostMinIntrinsicWidth ?? ComputeMinIntrinsicWidth();
    public double maxIntrinsicWidth => _hostMaxIntrinsicWidth ?? _naturalWidth;
    public double longestLine => _hostLongestLine ?? Math.Min(_naturalWidth, width);
    public double alphabeticBaseline => _hostAlphabeticBaseline ?? _lineHeight * 0.8;
    public double ideographicBaseline => _hostIdeographicBaseline ?? _lineHeight;
    public bool didExceedMaxLines { get; private set; }
    public int numberOfLines { get; private set; }
    public bool debugDisposed => Volatile.Read(ref _disposed) != 0;
    internal ulong CanvasKitMetricsHash { get; private set; }
    internal float CanvasKitHeightMultiplier { get; init; } = 1;
    internal string CanvasKitLocale { get; init; } = string.Empty;
    internal TextDirection CanvasKitTextDirection { get; init; } = TextDirection.ltr;
    internal TextAlign CanvasKitTextAlign { get; init; } = TextAlign.start;
    internal string? CanvasKitEllipsis { get; init; }
    internal IReadOnlyList<ParagraphTextRun> TextRuns { get; }
    internal Func<double, ParagraphHostLayoutSnapshot>? CanvasKitRelayout { get; init; }
    internal uint CanvasKitMaxLines => _maxLines is null
        ? 0
        : checked((uint)Math.Clamp(_maxLines.Value, 0L, (long)uint.MaxValue));
    public void layout(ParagraphConstraints constraints)
    {
        var availableWidth = constraints.width;
        if (double.IsNaN(availableWidth) || availableWidth < 0)
            throw new ArgumentOutOfRangeException(nameof(constraints),
                "Paragraph width must be nonnegative or positive infinity.");
        if (CanvasKitRelayout is { } relayout)
        {
            ApplyHostLayout(relayout(availableWidth));
            return;
        }

        _hostMinIntrinsicWidth = null;
        _hostMaxIntrinsicWidth = null;
        _hostLongestLine = null;
        _hostAlphabeticBaseline = null;
        _hostIdeographicBaseline = null;
        _hostGraphemes = null;
        width = double.IsFinite(availableWidth) ? Math.Max(0, availableWidth) : _naturalWidth;
        _lines = BuildLines(width);
        didExceedMaxLines = _maxLines.HasValue && _lines.Count > _maxLines.Value;
        if (_maxLines.HasValue && _lines.Count > _maxLines.Value)
        {
            _lines.RemoveRange(checked((int)_maxLines.Value), _lines.Count - checked((int)_maxLines.Value));
        }
        numberOfLines = _lines.Count;
        height = numberOfLines * _lineHeight;
    }
    public List<TextBox> getBoxesForRange(long start, long end, BoxHeightStyle boxHeightStyle = BoxHeightStyle.tight, BoxWidthStyle boxWidthStyle = BoxWidthStyle.tight)
    {
        var clampedStart = Math.Clamp(checked((int)start), 0, text.Length);
        var clampedEnd = Math.Clamp(checked((int)end), clampedStart, text.Length);
        if (clampedStart == clampedEnd)
        {
            return [];
        }

        if (_hostGraphemes is { } graphemes)
        {
            var useStrutHeight = boxHeightStyle == BoxHeightStyle.strut;
            return graphemes
                .Where(value => value.Start < clampedEnd && value.End > clampedStart)
                .Select(value => new TextBox(
                    value.Left,
                    useStrutHeight ? value.StrutTop : value.Top,
                    value.Right,
                    useStrutHeight ? value.StrutBottom : value.Bottom,
                    value.Direction))
                .ToList();
        }

        var boxes = new List<TextBox>();
        for (var line = 0; line < _lines.Count; line++)
        {
            var paragraphLine = _lines[line];
            var boxStart = Math.Max(clampedStart, paragraphLine.Start);
            var boxEnd = Math.Min(clampedEnd, paragraphLine.End);
            if (boxStart >= boxEnd) continue;
            boxes.Add(new TextBox(
                paragraphLine.Left + AdvanceBetween(paragraphLine.Start, boxStart),
                paragraphLine.Top,
                paragraphLine.Left + AdvanceBetween(paragraphLine.Start, boxEnd),
                paragraphLine.Top + paragraphLine.Height,
                CanvasKitTextDirection));
        }

        return boxes;
    }
    public List<TextBox> getBoxesForPlaceholders() => [];
    public TextPosition getPositionForOffset(Offset offset)
    {
        if (_hostGraphemes is { Length: > 0 } graphemes)
        {
            var glyph = ClosestGrapheme(graphemes, offset);
            var afterMidpoint = offset.dx >= (glyph.Left + glyph.Right) / 2;
            return new TextPosition(glyph.Direction == TextDirection.rtl
                ? afterMidpoint ? glyph.Start : glyph.End
                : afterMidpoint ? glyph.End : glyph.Start);
        }
        if (_lines.Count == 0) return new TextPosition(0);
        var lineIndex = _lines.FindIndex(value => offset.dy < value.Top + value.Height);
        var line = _lines[lineIndex < 0 ? ^1 : lineIndex];
        var x = Math.Max(0, offset.dx - line.Left);
        var currentX = 0.0;
        for (var index = line.Start; index < line.End; index = NextClusterEnd(index))
        {
            var clusterEnd = NextClusterEnd(index);
            var advance = AdvanceBetween(index, clusterEnd);
            if (x < currentX + advance / 2) return new TextPosition(index);
            currentX += advance;
        }
        return new TextPosition(line.End);
    }
    public TextRange getWordBoundary(TextPosition position)
    {
        var offset = Math.Clamp(checked((int)position.offset), 0, text.Length);
        var start = offset;
        var end = offset;
        while (start > 0 && !char.IsWhiteSpace(text[start - 1])) start--;
        while (end < text.Length && !char.IsWhiteSpace(text[end])) end++;
        return new TextRange(start, end);
    }
    public TextRange getLineBoundary(TextPosition position)
    {
        var offset = Math.Clamp(checked((int)position.offset), 0, text.Length);
        var line = FindLine(offset);
        return line is null ? TextRange.empty : new TextRange(line.Value.Start, line.Value.End);
    }
    public List<LineMetrics> computeLineMetrics()
    {
        var metrics = new List<LineMetrics>(numberOfLines);
        for (var line = 0; line < numberOfLines; line++)
        {
            var paragraphLine = _lines[line];
            metrics.Add(new LineMetrics(
                hardBreak: paragraphLine.HardBreak,
                ascent: paragraphLine.Ascent,
                descent: paragraphLine.Descent,
                unscaledAscent: paragraphLine.Ascent,
                height: paragraphLine.Height,
                width: paragraphLine.Width,
                left: paragraphLine.Left,
                baseline: paragraphLine.Baseline,
                lineNumber: line));
        }
        return metrics;
    }
    public LineMetrics? getLineMetricsAt(long lineNumber) =>
        lineNumber >= 0 && lineNumber < numberOfLines ? computeLineMetrics()[checked((int)lineNumber)] : null;
    public GlyphInfo? getGlyphInfoAt(long codeUnitOffset)
    {
        if (codeUnitOffset < 0 || codeUnitOffset >= text.Length)
        {
            return null;
        }
        var offset = checked((int)codeUnitOffset);
        if (_hostGraphemes is { } graphemes)
        {
            var glyph = graphemes.FirstOrDefault(value =>
                offset >= value.Start && offset < value.End);
            return glyph is null
                ? null
                : ToGlyphInfo(glyph);
        }
        var clusterStart = ClusterStart(offset);
        var clusterEnd = NextClusterEnd(clusterStart);
        var lineIndex = FindLineIndex(clusterStart);
        if (lineIndex < 0) return null;
        var line = _lines[lineIndex];
        var left = line.Left + AdvanceBetween(line.Start, clusterStart);
        return new GlyphInfo(
            Rect.fromLTWH(left, line.Top, AdvanceBetween(clusterStart, clusterEnd), line.Height),
            new TextRange(clusterStart, clusterEnd),
            CanvasKitTextDirection);
    }
    public GlyphInfo? getClosestGlyphInfoForOffset(Offset offset)
    {
        if (_hostGraphemes is { Length: > 0 } graphemes)
            return ToGlyphInfo(ClosestGrapheme(graphemes, offset));
        if (text.Length == 0)
        {
            return null;
        }
        var position = getPositionForOffset(offset);
        return getGlyphInfoAt(Math.Min(position.offset, text.Length - 1));
    }
    public void Dispose() => Interlocked.Exchange(ref _disposed, 1);
    public void dispose() => Dispose();

    internal void ApplyHostLayout(ParagraphHostLayoutSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ObjectDisposedException.ThrowIf(debugDisposed, this);
        if (!IsNonnegativeFinite(snapshot.Width) || !IsNonnegativeFinite(snapshot.Height) ||
            !IsNonnegativeFinite(snapshot.AlphabeticBaseline) ||
            !IsNonnegativeFinite(snapshot.IdeographicBaseline) ||
            !IsNonnegativeFinite(snapshot.MinIntrinsicWidth) ||
            !IsNonnegativeFinite(snapshot.MaxIntrinsicWidth) ||
            !IsNonnegativeFinite(snapshot.LongestLine))
            throw new InvalidDataException("The host returned non-finite or negative paragraph geometry.");
        if (snapshot.CodeUnitAdvances.Count != text.Length ||
            snapshot.CodeUnitAdvances.Any(value => !IsNonnegativeFinite(value)))
            throw new InvalidDataException("The host paragraph advance table does not match the UTF-16 text.");

        var lines = new List<ParagraphLine>(snapshot.Lines.Count);
        foreach (var line in snapshot.Lines)
        {
            if (line.Start < 0 || line.End < line.Start || line.End > text.Length ||
                !IsNonnegativeFinite(line.Ascent) || !IsNonnegativeFinite(line.Descent) ||
                !IsNonnegativeFinite(line.Height) || !IsNonnegativeFinite(line.Width) ||
                !double.IsFinite(line.Left) || !IsNonnegativeFinite(line.Baseline))
                throw new InvalidDataException("The host returned an invalid paragraph line snapshot.");
            lines.Add(new(
                line.Start, line.End, line.Width, line.HardBreak,
                line.Ascent, line.Descent, line.Height, line.Left, line.Baseline));
        }

        var graphemes = new ParagraphHostGraphemeSnapshot[snapshot.Graphemes.Count];
        var previousEnd = 0;
        for (var index = 0; index < graphemes.Length; index++)
        {
            var grapheme = snapshot.Graphemes[index];
            if (grapheme.Start < previousEnd || grapheme.End <= grapheme.Start ||
                grapheme.End > text.Length || !double.IsFinite(grapheme.Left) ||
                !double.IsFinite(grapheme.Top) || !double.IsFinite(grapheme.Right) ||
                !double.IsFinite(grapheme.Bottom) || grapheme.Right < grapheme.Left ||
                grapheme.Bottom < grapheme.Top || !double.IsFinite(grapheme.StrutTop) ||
                !double.IsFinite(grapheme.StrutBottom) ||
                grapheme.StrutBottom < grapheme.StrutTop)
                throw new InvalidDataException("The host returned an invalid paragraph grapheme snapshot.");
            graphemes[index] = grapheme;
            previousEnd = grapheme.End;
        }

        width = snapshot.Width;
        height = snapshot.Height;
        _naturalWidth = snapshot.MaxIntrinsicWidth;
        if (lines.Count != 0) _lineHeight = lines[0].Height;
        _codeUnitAdvances = snapshot.CodeUnitAdvances.ToArray();
        _lines = lines;
        _hostGraphemes = graphemes;
        _hostMinIntrinsicWidth = snapshot.MinIntrinsicWidth;
        _hostMaxIntrinsicWidth = snapshot.MaxIntrinsicWidth;
        _hostLongestLine = snapshot.LongestLine;
        _hostAlphabeticBaseline = snapshot.AlphabeticBaseline;
        _hostIdeographicBaseline = snapshot.IdeographicBaseline;
        didExceedMaxLines = snapshot.DidExceedMaxLines;
        numberOfLines = lines.Count;
        CanvasKitMetricsHash = snapshot.MetricsHash;
    }

    private static bool IsNonnegativeFinite(double value) => double.IsFinite(value) && value >= 0;

    private static ParagraphHostGraphemeSnapshot ClosestGrapheme(
        IReadOnlyList<ParagraphHostGraphemeSnapshot> graphemes,
        Offset offset)
    {
        var closest = graphemes[0];
        var closestDistance = SquaredDistance(closest, offset);
        for (var index = 1; index < graphemes.Count; index++)
        {
            var distance = SquaredDistance(graphemes[index], offset);
            if (distance >= closestDistance) continue;
            closest = graphemes[index];
            closestDistance = distance;
        }
        return closest;
    }

    private static double SquaredDistance(ParagraphHostGraphemeSnapshot grapheme, Offset offset)
    {
        var dx = offset.dx < grapheme.Left
            ? grapheme.Left - offset.dx
            : offset.dx > grapheme.Right ? offset.dx - grapheme.Right : 0;
        var dy = offset.dy < grapheme.Top
            ? grapheme.Top - offset.dy
            : offset.dy > grapheme.Bottom ? offset.dy - grapheme.Bottom : 0;
        return (dx * dx) + (dy * dy);
    }

    private static GlyphInfo ToGlyphInfo(ParagraphHostGraphemeSnapshot grapheme) => new(
        Rect.fromLTRB(grapheme.Left, grapheme.Top, grapheme.Right, grapheme.Bottom),
        new TextRange(grapheme.Start, grapheme.End),
        grapheme.Direction);

    private static double[] CreateFallbackAdvances(string text, double fontSize)
    {
        var advances = new double[text.Length];
        for (var index = 0; index < text.Length; index++)
        {
            if (char.IsLowSurrogate(text[index])) continue;
            var isWide = text[index] >= 0x2E80 || char.IsHighSurrogate(text[index]);
            advances[index] = fontSize * (isWide ? 1.0 : 0.55);
        }
        return advances;
    }

    private static double ComputeNaturalWidth(string text, IReadOnlyList<double> advances)
    {
        var longest = 0.0;
        var current = 0.0;
        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] == '\n')
            {
                longest = Math.Max(longest, current);
                current = 0;
            }
            else current += advances[index];
        }
        return Math.Max(longest, current);
    }

    private static ulong ComputeMetricsHash(
        string text,
        double width,
        double height,
        double fontSize,
        IReadOnlyList<double> advances)
    {
        const ulong offset = 14695981039346656037;
        const ulong prime = 1099511628211;
        var result = offset;
        static void Add(ref ulong hash, ulong value)
        {
            const ulong localPrime = 1099511628211;
            for (var index = 0; index < sizeof(ulong); index++)
            {
                hash ^= (byte)(value >> (index * 8));
                hash *= localPrime;
            }
        }
        foreach (var character in text)
        {
            result ^= character;
            result *= prime;
        }
        Add(ref result, unchecked((ulong)BitConverter.DoubleToInt64Bits(width)));
        Add(ref result, unchecked((ulong)BitConverter.DoubleToInt64Bits(height)));
        Add(ref result, unchecked((ulong)BitConverter.DoubleToInt64Bits(fontSize)));
        foreach (var advance in advances)
            Add(ref result, unchecked((ulong)BitConverter.DoubleToInt64Bits(advance)));
        return result;
    }

    private double ComputeMinIntrinsicWidth()
    {
        var longest = 0.0;
        var current = 0.0;
        for (var index = 0; index < text.Length; index++)
        {
            if (char.IsWhiteSpace(text[index]))
            {
                longest = Math.Max(longest, current);
                current = 0;
            }
            else current += _codeUnitAdvances[index];
        }
        return Math.Max(longest, current);
    }

    private List<ParagraphLine> BuildLines(double availableWidth)
    {
        if (text.Length == 0) return [];
        var lines = new List<ParagraphLine>();
        var lineStart = 0;
        var lineWidth = 0.0;
        var finiteWidth = double.IsFinite(availableWidth);
        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] == '\n')
            {
                lines.Add(CreateFallbackLine(lineStart, index, lineWidth, true, lines.Count));
                lineStart = index + 1;
                lineWidth = 0;
                continue;
            }

            var advance = _codeUnitAdvances[index];
            if (finiteWidth && index > lineStart && lineWidth + advance > availableWidth)
            {
                lines.Add(CreateFallbackLine(lineStart, index, lineWidth, false, lines.Count));
                lineStart = index;
                lineWidth = 0;
            }
            lineWidth += advance;
        }
        lines.Add(CreateFallbackLine(lineStart, text.Length, lineWidth, false, lines.Count));
        return lines;
    }

    private ParagraphLine CreateFallbackLine(
        int start, int end, double lineWidth, bool hardBreak, int lineNumber)
    {
        var ascent = _lineHeight * 0.8;
        return new(
            start, end, lineWidth, hardBreak,
            ascent, _lineHeight - ascent, _lineHeight, 0,
            (lineNumber * _lineHeight) + ascent);
    }

    private double AdvanceBetween(int start, int end)
    {
        var result = 0.0;
        for (var index = start; index < end; index++) result += _codeUnitAdvances[index];
        return result;
    }

    private int ClusterStart(int offset)
    {
        if (_graphemeStarts.Length == 0) return 0;
        var index = Array.BinarySearch(_graphemeStarts, offset);
        if (index >= 0) return _graphemeStarts[index];
        index = ~index;
        return _graphemeStarts[Math.Max(0, index - 1)];
    }

    private int NextClusterEnd(int start)
    {
        if (_graphemeStarts.Length == 0) return text.Length;
        var index = Array.BinarySearch(_graphemeStarts, start);
        index = index >= 0 ? index + 1 : ~index;
        return index < _graphemeStarts.Length ? _graphemeStarts[index] : text.Length;
    }

    private int FindLineIndex(int offset)
    {
        for (var index = 0; index < _lines.Count; index++)
        {
            var line = _lines[index];
            if (offset >= line.Start && offset < line.End) return index;
        }
        return -1;
    }

    private ParagraphLine? FindLine(int offset)
    {
        foreach (var line in _lines)
        {
            if (offset >= line.Start && offset <= line.End) return line;
        }
        return _lines.Count == 0 ? null : _lines[^1];
    }

    private readonly record struct ParagraphLine(
        int Start,
        int End,
        double Width,
        bool HardBreak,
        double Ascent,
        double Descent,
        double Height,
        double Left,
        double Baseline)
    {
        internal double Top => Baseline - Ascent;
    }
}

public interface IImageHostCapability
{
    ValueTask<Image> DecodeAsync(ReadOnlyMemory<byte> bytes, DartUiInvocation invocation, CancellationToken cancellationToken = default);
}

public sealed class Image : IDisposable
{
    private readonly Action? _release;
    private int _disposed;

    internal object? HostHandle { get; init; }

    public Image(ulong viewId, int width, int height, Action? release = null)
    {
        this.viewId = viewId;
        this.width = width;
        this.height = height;
        _release = release;
    }

    public ulong viewId { get; }

    public int width { get; }

    public int height { get; }

    public bool debugDisposed => Volatile.Read(ref _disposed) != 0;
    public Image clone()
    {
        if (HostHandle is IDorotiImageHandle handle)
        {
            var clone = handle.Clone();
            return new(viewId, width, height, clone.Release) { HostHandle = clone };
        }
        return new(viewId, width, height, _release) { HostHandle = HostHandle };
    }
    public bool isCloneOf(Image other) => ReferenceEquals(this, other) || (viewId == other.viewId && width == other.width && height == other.height);
    public static IReadOnlyList<string> debugGetOpenHandleStackTraces() => [];
    public Future<ByteData?> toByteData(ImageByteFormat format = ImageByteFormat.rawRgba)
    {
        throw new DorotiCapabilityException(
            DorotiCapabilityIds.GraphicsImage,
            viewId,
            DartUiInvocation.Managed("dart:ui#Image.toByteData"),
            $"image encoding for {format} is not registered by the active host");
    }
    public void dispose() => Dispose();

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0)
        {
            _release?.Invoke();
        }
    }
}

public enum ImageByteFormat { rawRgba, rawStraightRgba, rawUnmodified, png }

[Flags]
public enum SemanticsAction : long
{
    none = 0,
    tap = 1L << 0,
    longPress = 1L << 1,
    scrollLeft = 1L << 2,
    scrollRight = 1L << 3,
    scrollUp = 1L << 4,
    scrollDown = 1L << 5,
    increase = 1L << 6,
    decrease = 1L << 7,
    showOnScreen = 1L << 8,
    moveCursorForwardByCharacter = 1L << 9,
    moveCursorBackwardByCharacter = 1L << 10,
    setSelection = 1L << 11,
    copy = 1L << 12,
    cut = 1L << 13,
    paste = 1L << 14,
    didGainAccessibilityFocus = 1L << 15,
    didLoseAccessibilityFocus = 1L << 16,
    customAction = 1L << 17,
    dismiss = 1L << 18,
    moveCursorForwardByWord = 1L << 19,
    moveCursorBackwardByWord = 1L << 20,
    setText = 1L << 21,
    focus = 1L << 22,
    scrollToOffset = 1L << 23,
    expand = 1L << 24,
    collapse = 1L << 25,
}

public enum SemanticsRole
{
    none,
    tab,
    tabBar,
    tabPanel,
    dialog,
    alertDialog,
    table,
    cell,
    row,
    columnHeader,
    dragHandle,
    spinButton,
    comboBox,
    menuBar,
    menu,
    menuItem,
    menuItemCheckbox,
    menuItemRadio,
    list,
    listItem,
    form,
    tooltip,
    loadingSpinner,
    progressBar,
    hotKey,
    radioGroup,
    status,
    alert,
    complementary,
    contentInfo,
    main,
    navigation,
    region,
}

public enum SemanticsInputType { none, text, url, phone, search, email }
public enum SemanticsValidationResult { none, valid, invalid }
public enum SemanticsHitTestBehavior { defer, opaque, transparent }
public enum CheckedState { none, isTrue, isFalse, mixed }
public enum Tristate { none, isTrue, isFalse }

public static class TristateExtensions
{
    public static bool? toBoolOrNull(this Tristate value) => value switch
    {
        Tristate.isTrue => true,
        Tristate.isFalse => false,
        _ => null,
    };

    public static bool hasConflict(this Tristate value, Tristate other) =>
        value != Tristate.none && other != Tristate.none;

    public static Tristate merge(this Tristate value, Tristate other) =>
        value == Tristate.isTrue || other == Tristate.isTrue
            ? Tristate.isTrue
            : value == Tristate.isFalse || other == Tristate.isFalse
                ? Tristate.isFalse
                : Tristate.none;
}

[Flags]
public enum SemanticsFlag : long
{
    none = 0,
    hasCheckedState = 1L << 0,
    isChecked = 1L << 1,
    isCheckStateMixed = 1L << 2,
    hasSelectedState = 1L << 3,
    isSelected = 1L << 4,
    isButton = 1L << 5,
    isTextField = 1L << 6,
    isFocused = 1L << 7,
    hasEnabledState = 1L << 8,
    isEnabled = 1L << 9,
    isInMutuallyExclusiveGroup = 1L << 10,
    isHeader = 1L << 11,
    isObscured = 1L << 12,
    scopesRoute = 1L << 13,
    namesRoute = 1L << 14,
    isHidden = 1L << 15,
    isImage = 1L << 16,
    isLiveRegion = 1L << 17,
    hasToggledState = 1L << 18,
    isToggled = 1L << 19,
    hasImplicitScrolling = 1L << 20,
    isMultiline = 1L << 21,
    isReadOnly = 1L << 22,
    isFocusable = 1L << 23,
    isLink = 1L << 24,
    isSlider = 1L << 25,
    isKeyboardKey = 1L << 26,
    hasExpandedState = 1L << 27,
    isExpanded = 1L << 28,
    hasRequiredState = 1L << 29,
    isRequired = 1L << 30,
}

public sealed record SemanticsFlags(
    CheckedState isChecked = CheckedState.none,
    Tristate isSelected = Tristate.none,
    Tristate isEnabled = Tristate.none,
    Tristate isToggled = Tristate.none,
    Tristate isExpanded = Tristate.none,
    Tristate isRequired = Tristate.none,
    Tristate isFocused = Tristate.none,
    bool isButton = false,
    bool isTextField = false,
    bool isInMutuallyExclusiveGroup = false,
    bool isHeader = false,
    bool isObscured = false,
    bool scopesRoute = false,
    bool namesRoute = false,
    bool isHidden = false,
    bool isImage = false,
    bool isLiveRegion = false,
    bool hasImplicitScrolling = false,
    bool isMultiline = false,
    bool isReadOnly = false,
    bool isLink = false,
    bool isSlider = false,
    bool isKeyboardKey = false,
    bool isAccessibilityFocusBlocked = false)
{
    public static SemanticsFlags none { get; } = new();

    public SemanticsFlags copyWith(
        CheckedState? isChecked = null,
        Tristate? isSelected = null,
        Tristate? isEnabled = null,
        Tristate? isToggled = null,
        Tristate? isExpanded = null,
        Tristate? isRequired = null,
        Tristate? isFocused = null,
        bool? isButton = null,
        bool? isTextField = null,
        bool? isInMutuallyExclusiveGroup = null,
        bool? isHeader = null,
        bool? isObscured = null,
        bool? scopesRoute = null,
        bool? namesRoute = null,
        bool? isHidden = null,
        bool? isImage = null,
        bool? isLiveRegion = null,
        bool? hasImplicitScrolling = null,
        bool? isMultiline = null,
        bool? isReadOnly = null,
        bool? isLink = null,
        bool? isSlider = null,
        bool? isKeyboardKey = null,
        bool? isAccessibilityFocusBlocked = null) => new(
            isChecked ?? this.isChecked,
            isSelected ?? this.isSelected,
            isEnabled ?? this.isEnabled,
            isToggled ?? this.isToggled,
            isExpanded ?? this.isExpanded,
            isRequired ?? this.isRequired,
            isFocused ?? this.isFocused,
            isButton ?? this.isButton,
            isTextField ?? this.isTextField,
            isInMutuallyExclusiveGroup ?? this.isInMutuallyExclusiveGroup,
            isHeader ?? this.isHeader,
            isObscured ?? this.isObscured,
            scopesRoute ?? this.scopesRoute,
            namesRoute ?? this.namesRoute,
            isHidden ?? this.isHidden,
            isImage ?? this.isImage,
            isLiveRegion ?? this.isLiveRegion,
            hasImplicitScrolling ?? this.hasImplicitScrolling,
            isMultiline ?? this.isMultiline,
            isReadOnly ?? this.isReadOnly,
            isLink ?? this.isLink,
            isSlider ?? this.isSlider,
            isKeyboardKey ?? this.isKeyboardKey,
            isAccessibilityFocusBlocked ?? this.isAccessibilityFocusBlocked);

    public SemanticsFlags merge(SemanticsFlags other) => new(
        MergeChecked(isChecked, other.isChecked),
        isSelected.merge(other.isSelected),
        isEnabled.merge(other.isEnabled),
        isToggled.merge(other.isToggled),
        isExpanded.merge(other.isExpanded),
        isRequired.merge(other.isRequired),
        isFocused.merge(other.isFocused),
        isButton || other.isButton,
        isTextField || other.isTextField,
        isInMutuallyExclusiveGroup || other.isInMutuallyExclusiveGroup,
        isHeader || other.isHeader,
        isObscured || other.isObscured,
        scopesRoute || other.scopesRoute,
        namesRoute || other.namesRoute,
        isHidden || other.isHidden,
        isImage || other.isImage,
        isLiveRegion || other.isLiveRegion,
        hasImplicitScrolling || other.hasImplicitScrolling,
        isMultiline || other.isMultiline,
        isReadOnly || other.isReadOnly,
        isLink || other.isLink,
        isSlider || other.isSlider,
        isKeyboardKey || other.isKeyboardKey,
        isAccessibilityFocusBlocked || other.isAccessibilityFocusBlocked);

    public bool hasConflictingFlags(SemanticsFlags other) =>
        Conflicts(isChecked, other.isChecked) ||
        isSelected.hasConflict(other.isSelected) ||
        isEnabled.hasConflict(other.isEnabled) ||
        isToggled.hasConflict(other.isToggled) ||
        isExpanded.hasConflict(other.isExpanded) ||
        isRequired.hasConflict(other.isRequired) ||
        isFocused.hasConflict(other.isFocused);

    public List<string> toStrings()
    {
        var result = new List<string>();
        if (isChecked != CheckedState.none) result.Add(nameof(isChecked));
        if (isSelected != Tristate.none) result.Add(nameof(isSelected));
        if (isEnabled != Tristate.none) result.Add(nameof(isEnabled));
        if (isToggled != Tristate.none) result.Add(nameof(isToggled));
        if (isExpanded != Tristate.none) result.Add(nameof(isExpanded));
        if (isRequired != Tristate.none) result.Add(nameof(isRequired));
        if (isFocused != Tristate.none) result.Add(nameof(isFocused));
        if (isButton) result.Add(nameof(isButton));
        if (isTextField) result.Add(nameof(isTextField));
        if (isInMutuallyExclusiveGroup) result.Add(nameof(isInMutuallyExclusiveGroup));
        if (isHeader) result.Add(nameof(isHeader));
        if (isObscured) result.Add(nameof(isObscured));
        if (scopesRoute) result.Add(nameof(scopesRoute));
        if (namesRoute) result.Add(nameof(namesRoute));
        if (isHidden) result.Add(nameof(isHidden));
        if (isImage) result.Add(nameof(isImage));
        if (isLiveRegion) result.Add(nameof(isLiveRegion));
        if (hasImplicitScrolling) result.Add(nameof(hasImplicitScrolling));
        if (isMultiline) result.Add(nameof(isMultiline));
        if (isReadOnly) result.Add(nameof(isReadOnly));
        if (isLink) result.Add(nameof(isLink));
        if (isSlider) result.Add(nameof(isSlider));
        if (isKeyboardKey) result.Add(nameof(isKeyboardKey));
        if (isAccessibilityFocusBlocked) result.Add(nameof(isAccessibilityFocusBlocked));
        return result;
    }

    private static bool Conflicts(CheckedState left, CheckedState right) =>
        left != CheckedState.none && right != CheckedState.none;

    private static CheckedState MergeChecked(CheckedState left, CheckedState right) =>
        left != CheckedState.none ? left : right;
}

public sealed record SemanticsNodeUpdate(
    int id,
    Rect rect,
    string? label,
    string? value,
    SemanticsAction actions,
    IReadOnlyList<int> children,
    SemanticsFlags? flags = null,
    SemanticsRole role = SemanticsRole.none,
    int? traversalParent = null,
    int? indexInParent = null,
    long textSelectionBase = -1,
    long textSelectionExtent = -1,
    double? scrollPosition = null,
    double? scrollExtentMax = null,
    double? scrollExtentMin = null,
    string? identifier = null,
    string? hint = null,
    string? tooltip = null,
    string? increasedValue = null,
    string? decreasedValue = null,
    long? headingLevel = null,
    string? linkUrl = null,
    SemanticsValidationResult validationResult = SemanticsValidationResult.none,
    SemanticsHitTestBehavior hitTestBehavior = SemanticsHitTestBehavior.defer,
    SemanticsInputType inputType = SemanticsInputType.none,
    string? minValue = null,
    string? maxValue = null,
    long? maxValueLength = null,
    long? currentValueLength = null,
    long? scrollChildCount = null,
    long? scrollIndex = null,
    IReadOnlyList<string>? controlsNodes = null,
    Locale? locale = null);

public enum SemanticsUpdateUrgency
{
    /// <summary>Let the host classify the node delta. Geometry-only scroll work may be coalesced.</summary>
    automatic,
    /// <summary>Do not defer this update; it changes an assistive-technology interaction boundary.</summary>
    immediate,
    /// <summary>Flush the latest scroll geometry when a scroll activity reaches rest.</summary>
    scrollEnd,
}

public sealed record SemanticsUpdate(
    long generation,
    IReadOnlyList<SemanticsNodeUpdate> nodes,
    SemanticsUpdateUrgency urgency = SemanticsUpdateUrgency.automatic);

/// <summary>
/// Projects the framework's parent-relative semantics rectangles into logical view
/// coordinates. Native hosts keep a flat accessibility surface, while DOM hosts can
/// subtract the projected parent origin when reconstructing the semantic hierarchy.
/// </summary>
public static class SemanticsGeometryProjection
{
    public static IReadOnlyList<SemanticsNodeUpdate> ToViewCoordinates(
        IEnumerable<SemanticsNodeUpdate> source)
    {
        ArgumentNullException.ThrowIfNull(source);
        var input = source.ToArray();
        var nodes = input.ToDictionary(node => node.id);
        var childIds = nodes.Values.SelectMany(node => node.children).ToHashSet();
        var projected = new Dictionary<int, SemanticsNodeUpdate>();

        foreach (var root in nodes.Values.Where(node => !childIds.Contains(node.id)).OrderBy(node => node.id))
            Project(root.id, 0, 0, nodes, projected, []);
        foreach (var orphan in nodes.Values.Where(node => !projected.ContainsKey(node.id)).OrderBy(node => node.id))
            Project(orphan.id, 0, 0, nodes, projected, []);

        return input.Select(node => projected[node.id]).ToArray();
    }

    private static void Project(
        int id,
        double parentLeft,
        double parentTop,
        IReadOnlyDictionary<int, SemanticsNodeUpdate> nodes,
        IDictionary<int, SemanticsNodeUpdate> projected,
        HashSet<int> activePath)
    {
        if (projected.ContainsKey(id) || !nodes.TryGetValue(id, out var node) || !activePath.Add(id)) return;
        var rect = new Rect(
            node.rect.left + parentLeft,
            node.rect.top + parentTop,
            node.rect.right + parentLeft,
            node.rect.bottom + parentTop);
        projected[id] = node with { rect = rect };
        foreach (var childId in node.children)
            Project(childId, rect.left, rect.top, nodes, projected, activePath);
        activePath.Remove(id);
    }
}

[Flags]
public enum SemanticsNodeProperty
{
    none = 0,
    bounds = 1 << 0,
    label = 1 << 1,
    value = 1 << 2,
    actions = 1 << 3,
    flags = 1 << 4,
    role = 1 << 5,
    children = 1 << 6,
    traversal = 1 << 7,
    selection = 1 << 8,
    /// <summary>
    /// The current logical scroll offset and its framework-provided extents.
    /// This remains distinct from node bounds: native accessibility providers
    /// use it to expose an accurate Scroll pattern without inventing a second
    /// scroll authority.
    /// </summary>
    scroll = 1 << 9,
    /// <summary>Accessible name supplements, role metadata, ranges, and relationships.</summary>
    metadata = 1 << 10,
}

public sealed record SemanticsNodeDelta(
    int id,
    SemanticsNodeProperty changedProperties,
    int previousContentHash,
    int contentHash)
{
    public bool IsGeometryOnly =>
        (changedProperties & ~(SemanticsNodeProperty.bounds | SemanticsNodeProperty.scroll)) ==
        SemanticsNodeProperty.none;
}

public sealed record SemanticsUpdateDelta(
    IReadOnlyList<SemanticsNodeDelta> changedNodes,
    IReadOnlyList<int> removedNodeIds)
{
    public bool HasChanges => changedNodes.Count != 0 || removedNodeIds.Count != 0;
    public bool HasTopologyChange => removedNodeIds.Count != 0 || changedNodes.Any(delta =>
        delta.previousContentHash == 0 ||
        delta.changedProperties.HasFlag(SemanticsNodeProperty.children) ||
        delta.changedProperties.HasFlag(SemanticsNodeProperty.traversal));
    public bool IsGeometryOnly => changedNodes.Count != 0 && removedNodeIds.Count == 0 && changedNodes.All(delta => delta.IsGeometryOnly);
    // A virtualized scroll routinely inserts and removes semantics nodes. Treating that
    // topology churn as urgent bypasses the host's accessibility-rate limiter and makes
    // native layout work compete with every visual frame. Existing interactive content
    // mutations remain urgent; topology is flushed by the next bounded apply or by an
    // explicit immediate/scrollEnd update.
    public bool RequiresImmediateFlush => changedNodes.Any(delta =>
        delta.previousContentHash != 0 &&
        (delta.changedProperties & (SemanticsNodeProperty.label |
                                    SemanticsNodeProperty.value |
                                    SemanticsNodeProperty.actions |
                                    SemanticsNodeProperty.flags |
                                    SemanticsNodeProperty.role |
                                    SemanticsNodeProperty.selection |
                                    SemanticsNodeProperty.metadata)) != 0);
}

/// <summary>
/// Host-neutral semantics delta classifier. It keeps the framework's full node snapshots
/// cheap for hosts that need native accessibility overlays, while making the set of native
/// property writes and the 15-fps eligibility explicit.
/// </summary>
public static class SemanticsUpdateDiffer
{
    public static SemanticsUpdateDelta Diff(
        IReadOnlyDictionary<int, SemanticsNodeUpdate> previous,
        IReadOnlyList<SemanticsNodeUpdate> current)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        var changed = new List<SemanticsNodeDelta>();
        var currentIds = new HashSet<int>();
        foreach (var node in current)
        {
            currentIds.Add(node.id);
            if (!previous.TryGetValue(node.id, out var oldNode))
            {
                changed.Add(new(node.id, AllProperties, 0, ContentHash(node)));
                continue;
            }

            var properties = ChangedProperties(oldNode, node);
            if (properties != SemanticsNodeProperty.none)
                changed.Add(new(node.id, properties, ContentHash(oldNode), ContentHash(node)));
        }

        return new(changed, previous.Keys.Where(id => !currentIds.Contains(id)).OrderBy(id => id).ToArray());
    }

    public static int ContentHash(SemanticsNodeUpdate node)
    {
        var hash = new HashCode();
        hash.Add(node.label, StringComparer.Ordinal);
        hash.Add(node.value, StringComparer.Ordinal);
        hash.Add(node.actions);
        hash.Add(node.flags);
        hash.Add(node.role);
        hash.Add(node.traversalParent);
        hash.Add(node.indexInParent);
        hash.Add(node.textSelectionBase);
        hash.Add(node.textSelectionExtent);
        hash.Add(node.scrollPosition);
        hash.Add(node.scrollExtentMax);
        hash.Add(node.scrollExtentMin);
        hash.Add(node.identifier, StringComparer.Ordinal);
        hash.Add(node.hint, StringComparer.Ordinal);
        hash.Add(node.tooltip, StringComparer.Ordinal);
        hash.Add(node.increasedValue, StringComparer.Ordinal);
        hash.Add(node.decreasedValue, StringComparer.Ordinal);
        hash.Add(node.headingLevel);
        hash.Add(node.linkUrl, StringComparer.Ordinal);
        hash.Add(node.validationResult);
        hash.Add(node.hitTestBehavior);
        hash.Add(node.inputType);
        hash.Add(node.minValue, StringComparer.Ordinal);
        hash.Add(node.maxValue, StringComparer.Ordinal);
        hash.Add(node.maxValueLength);
        hash.Add(node.currentValueLength);
        hash.Add(node.scrollChildCount);
        hash.Add(node.scrollIndex);
        if (node.controlsNodes is not null)
            foreach (var controlledNode in node.controlsNodes) hash.Add(controlledNode, StringComparer.Ordinal);
        hash.Add(node.locale);
        foreach (var child in node.children) hash.Add(child);
        return hash.ToHashCode();
    }

    private const SemanticsNodeProperty AllProperties =
        SemanticsNodeProperty.bounds |
        SemanticsNodeProperty.label |
        SemanticsNodeProperty.value |
        SemanticsNodeProperty.actions |
        SemanticsNodeProperty.flags |
        SemanticsNodeProperty.role |
        SemanticsNodeProperty.children |
        SemanticsNodeProperty.traversal |
        SemanticsNodeProperty.selection |
        SemanticsNodeProperty.scroll |
        SemanticsNodeProperty.metadata;

    private static SemanticsNodeProperty ChangedProperties(SemanticsNodeUpdate previous, SemanticsNodeUpdate current)
    {
        var result = SemanticsNodeProperty.none;
        if (previous.rect != current.rect) result |= SemanticsNodeProperty.bounds;
        if (!string.Equals(previous.label, current.label, StringComparison.Ordinal)) result |= SemanticsNodeProperty.label;
        if (!string.Equals(previous.value, current.value, StringComparison.Ordinal)) result |= SemanticsNodeProperty.value;
        if (previous.actions != current.actions) result |= SemanticsNodeProperty.actions;
        if (previous.flags != current.flags) result |= SemanticsNodeProperty.flags;
        if (previous.role != current.role) result |= SemanticsNodeProperty.role;
        if (!previous.children.SequenceEqual(current.children)) result |= SemanticsNodeProperty.children;
        if (previous.traversalParent != current.traversalParent || previous.indexInParent != current.indexInParent)
            result |= SemanticsNodeProperty.traversal;
        if (previous.textSelectionBase != current.textSelectionBase || previous.textSelectionExtent != current.textSelectionExtent)
            result |= SemanticsNodeProperty.selection;
        if (previous.scrollPosition != current.scrollPosition ||
            previous.scrollExtentMax != current.scrollExtentMax ||
            previous.scrollExtentMin != current.scrollExtentMin)
        {
            result |= SemanticsNodeProperty.scroll;
        }
        if (!string.Equals(previous.identifier, current.identifier, StringComparison.Ordinal) ||
            !string.Equals(previous.hint, current.hint, StringComparison.Ordinal) ||
            !string.Equals(previous.tooltip, current.tooltip, StringComparison.Ordinal) ||
            !string.Equals(previous.increasedValue, current.increasedValue, StringComparison.Ordinal) ||
            !string.Equals(previous.decreasedValue, current.decreasedValue, StringComparison.Ordinal) ||
            previous.headingLevel != current.headingLevel ||
            !string.Equals(previous.linkUrl, current.linkUrl, StringComparison.Ordinal) ||
            previous.validationResult != current.validationResult ||
            previous.hitTestBehavior != current.hitTestBehavior ||
            previous.inputType != current.inputType ||
            !string.Equals(previous.minValue, current.minValue, StringComparison.Ordinal) ||
            !string.Equals(previous.maxValue, current.maxValue, StringComparison.Ordinal) ||
            previous.maxValueLength != current.maxValueLength ||
            previous.currentValueLength != current.currentValueLength ||
            previous.scrollChildCount != current.scrollChildCount ||
            previous.scrollIndex != current.scrollIndex ||
            !SequenceEqual(previous.controlsNodes, current.controlsNodes) ||
            previous.locale != current.locale)
        {
            result |= SemanticsNodeProperty.metadata;
        }
        return result;
    }

    private static bool SequenceEqual(IReadOnlyList<string>? left, IReadOnlyList<string>? right) =>
        ReferenceEquals(left, right) ||
        (left is not null && right is not null && left.SequenceEqual(right, StringComparer.Ordinal));
}

public sealed class SemanticsUpdateBuilder
{
    private readonly Dictionary<int, SemanticsNodeUpdate> _nodes = [];
    private long _generation;

    public void updateNode(SemanticsNodeUpdate node)
    {
        ArgumentNullException.ThrowIfNull(node);
        _nodes[node.id] = node;
    }

    public void updateNode(
        long id,
        SemanticsFlags flags,
        long actions,
        Rect rect,
        string identifier,
        string label,
        object labelAttributes,
        string value,
        object valueAttributes,
        string increasedValue,
        object increasedValueAttributes,
        string decreasedValue,
        object decreasedValueAttributes,
        string hint,
        object hintAttributes,
        string tooltip,
        TextDirection? textDirection,
        long textSelectionBase,
        long textSelectionExtent,
        long platformViewId,
        long maxValueLength,
        long currentValueLength,
        long scrollChildren,
        long scrollIndex,
        double scrollPosition,
        double scrollExtentMax,
        double scrollExtentMin,
        object transform,
        long traversalParent,
        object hitTestTransform,
        object childrenInTraversalOrder,
        object childrenInHitTestOrder,
        object additionalActions,
        long headingLevel,
        string linkUrl,
        SemanticsRole role,
        List<string>? controlsNodes,
        SemanticsValidationResult validationResult,
        SemanticsHitTestBehavior hitTestBehavior,
        SemanticsInputType inputType,
        Locale? locale,
        string minValue,
        string maxValue)
    {
        var actionFlags = (SemanticsAction)actions;
        var traversalChildren = ToNodeIds(childrenInTraversalOrder);
        var hitTestChildren = ToNodeIds(childrenInHitTestOrder);
        updateNode(new SemanticsNodeUpdate(
            checked((int)id),
            TransformRect(rect, transform),
            label,
            value,
            actionFlags,
            traversalChildren.Count > 0 ? traversalChildren : hitTestChildren,
            flags,
            role,
            traversalParent >= 0 ? checked((int)traversalParent) : null,
            null,
            textSelectionBase,
            textSelectionExtent,
            NormalizeOptionalDouble(scrollPosition),
            NormalizeOptionalDouble(scrollExtentMax),
            NormalizeOptionalDouble(scrollExtentMin),
            NullIfEmpty(identifier),
            NullIfEmpty(hint),
            NullIfEmpty(tooltip),
            NullIfEmpty(increasedValue),
            NullIfEmpty(decreasedValue),
            headingLevel > 0 ? headingLevel : null,
            NullIfEmpty(linkUrl),
            validationResult,
            hitTestBehavior,
            inputType,
            NullIfEmpty(minValue),
            NullIfEmpty(maxValue),
            NormalizeOptionalLong(maxValueLength),
            NormalizeOptionalLong(currentValueLength),
            NormalizeOptionalLong(scrollChildren),
            NormalizeOptionalLong(scrollIndex),
            controlsNodes?.ToArray(),
            locale));
    }

    private static Rect TransformRect(Rect rect, object transform)
    {
        if (transform is not IEnumerable<double> values) return rect;
        var storage = values.ToArray();
        if (storage.Length != 16) return rect;
        var matrix = new Matrix4(storage);
        var corners = new[]
        {
            matrix.perspectiveTransform(new Vector3(rect.left, rect.top, 0)),
            matrix.perspectiveTransform(new Vector3(rect.right, rect.top, 0)),
            matrix.perspectiveTransform(new Vector3(rect.left, rect.bottom, 0)),
            matrix.perspectiveTransform(new Vector3(rect.right, rect.bottom, 0)),
        };
        return new Rect(
            corners.Min(point => point.x),
            corners.Min(point => point.y),
            corners.Max(point => point.x),
            corners.Max(point => point.y));
    }

    // The generated framework ABI represents an absent scroll metric as NaN.
    // Host-neutral semantics use null instead, so consumers cannot mistake an
    // absent axis for a usable UIA Scroll range.
    private static double? NormalizeOptionalDouble(double value) =>
        double.IsFinite(value) ? value : null;

    private static long? NormalizeOptionalLong(long value) => value >= 0 ? value : null;

    private static string? NullIfEmpty(string? value) => string.IsNullOrEmpty(value) ? null : value;

    public void updateCustomAction(long id, string? label = null, string? hint = null, long overrideId = -1)
    {
        // Custom actions are transported alongside the next node batch by the
        // framework layer. The reduced host contract currently consumes only
        // node updates, so retaining the call is intentionally side-effect free.
    }

    public SemanticsUpdate build(long generation) => new(
        generation,
        _nodes.Values.OrderBy(node => node.id).ToArray());

    public SemanticsUpdate build() => build(Interlocked.Increment(ref _generation));

    private static IReadOnlyList<int> ToNodeIds(object value) => value switch
    {
        IEnumerable<int> integers => integers.ToArray(),
        IEnumerable<long> longs => longs.Select(checkedValue => checked((int)checkedValue)).ToArray(),
        System.Collections.IEnumerable items => items.Cast<object>().Select(item => checked(Convert.ToInt32(item, System.Globalization.CultureInfo.InvariantCulture))).ToArray(),
        _ => [],
    };
}

public readonly record struct SemanticsActionEvent(ulong viewId, int nodeId, SemanticsAction action, object? arguments = null)
{
    public SemanticsAction type => action;

    public SemanticsActionEvent copyWith(object? arguments = null) =>
        this with { arguments = arguments };
}

public interface ISemanticsHostCapability
{
    event Action<SemanticsActionEvent>? Action;

    /// <summary>
    /// Whether this host benefits from coalescing geometry-only semantics work
    /// while viewport metrics are changing continuously. Native hosts remain
    /// immediate unless they opt in explicitly.
    /// </summary>
    bool CoalesceGeometryDuringActiveMetrics => false;

    void SetEnabled(bool enabled, DartUiInvocation invocation);

    void Update(SemanticsUpdate update, DartUiInvocation invocation);
}
