using Doroti.Flutter.Runtime;

namespace Doroti.Flutter.Ui;

public enum BlendMode { clear, src, dst, srcOver, dstOver, srcIn, dstIn, srcOut, dstOut, srcATop, dstATop, xor, plus, modulate, screen, overlay, darken, lighten, colorDodge, colorBurn, hardLight, softLight, difference, exclusion, multiply, hue, saturation, color, luminosity }
public enum BlurStyle { normal, solid, outer, inner }
public enum BoxHeightStyle { tight, max, includeLineSpacingMiddle, includeLineSpacingTop, includeLineSpacingBottom, strut }
public enum BoxWidthStyle { tight, max }
public enum Clip { none, hardEdge, antiAlias, antiAliasWithSaveLayer }
public enum FilterQuality { none, low, medium, high }
public enum FontStyle { normal, italic }
public enum PlaceholderAlignment { baseline, aboveBaseline, belowBaseline, top, bottom, middle }
public enum TextBaseline { alphabetic, ideographic }
public enum TextLeadingDistribution { proportional, even }
public enum TileMode { clamp, repeated, mirror, decal }
public enum PathFillType { nonZero, evenOdd }
public enum PathOperation { difference, intersect, union, xor, reverseDifference }
public enum TextDecorationStyle { solid, doubleLine, dotted, dashed, wavy }

public readonly record struct Radius(double x, double y)
{
    public Radius(double radius) : this(radius, radius) { }
    public static Radius zero { get; } = new(0, 0);
    public static Radius circular(double radius) => new(radius, radius);
    public static Radius CreateCircular(double radius) => circular(radius);
    public static Radius elliptical(double x, double y) => new(x, y);
    public Radius clamp(Radius? minimum = null, Radius? maximum = null) => new(
        Math.Clamp(x, minimum?.x ?? double.NegativeInfinity, maximum?.x ?? double.PositiveInfinity),
        Math.Clamp(y, minimum?.y ?? double.NegativeInfinity, maximum?.y ?? double.PositiveInfinity));
    public static Radius operator +(Radius left, Radius right) => new(left.x + right.x, left.y + right.y);
    public static Radius operator -(Radius left, Radius right) => new(left.x - right.x, left.y - right.y);
    public static Radius operator -(Radius value) => new(-value.x, -value.y);
    public static Radius operator *(Radius value, double operand) => new(value.x * operand, value.y * operand);
    public static Radius operator /(Radius value, double operand) => new(value.x / operand, value.y / operand);
    public static Radius operator %(Radius value, double operand) => new(value.x % operand, value.y % operand);
    public Radius ___(double operand) => new(Math.Truncate(x / operand), Math.Truncate(y / operand));
}

public sealed record RRect(Rect outerRect, Radius tlRadius, Radius trRadius, Radius brRadius, Radius blRadius)
{
    public RRect(Rect rect, Radius radius) : this(rect, radius, radius, radius, radius) { }
    public static RRect fromRectAndRadius(Rect rect, Radius radius) => new(rect, radius, radius, radius, radius);
    public static RRect fromLTRBAndRadius(double left, double top, double right, double bottom, Radius radius) => fromRectAndRadius(new(left, top, right, bottom), radius);
    public static RRect fromLTRBAndCorners(double left, double top, double right, double bottom, Radius? topLeft = null, Radius? topRight = null, Radius? bottomRight = null, Radius? bottomLeft = null) =>
        fromRectAndCorners(new(left, top, right, bottom), topLeft, topRight, bottomRight, bottomLeft);
    public static RRect fromRectXY(Rect rect, double radiusX, double radiusY) => fromRectAndRadius(rect, new(radiusX, radiusY));
    public static RRect fromLTRBXY(double left, double top, double right, double bottom, double radiusX, double radiusY) => fromRectXY(new(left, top, right, bottom), radiusX, radiusY);
    public static RRect fromLTRBR(double left, double top, double right, double bottom, Radius radius) => fromRectAndRadius(new(left, top, right, bottom), radius);
    public static RRect fromRectAndCorners(Rect rect, Radius? topLeft = null, Radius? topRight = null, Radius? bottomRight = null, Radius? bottomLeft = null) =>
        new(rect, topLeft ?? Radius.zero, topRight ?? Radius.zero, bottomRight ?? Radius.zero, bottomLeft ?? Radius.zero);
    public Rect outerRectValue => outerRect;
    public double left => outerRect.left;
    public double top => outerRect.top;
    public double right => outerRect.right;
    public double bottom => outerRect.bottom;
    public double width => outerRect.width;
    public double height => outerRect.height;
    public Offset center => outerRect.center;
    public double shortestSide => outerRect.shortestSide;
    public double tlRadiusX => tlRadius.x;
    public double tlRadiusY => tlRadius.y;
    public double trRadiusX => trRadius.x;
    public double trRadiusY => trRadius.y;
    public double brRadiusX => brRadius.x;
    public double brRadiusY => brRadius.y;
    public double blRadiusX => blRadius.x;
    public double blRadiusY => blRadius.y;
    public RRect inflate(double delta) => new(outerRect.inflate(delta), Inflate(tlRadius, delta), Inflate(trRadius, delta), Inflate(brRadius, delta), Inflate(blRadius, delta));
    public RRect deflate(double delta) => inflate(-delta);
    public RRect shift(Offset offset) => this with { outerRect = outerRect.shift(offset) };
    public RRect scaleRadii() => this;
    public bool contains(Offset point)
    {
        if (!outerRect.contains(point)) return false;
        return CornerContains(point, left, top, tlRadius, 1, 1) &&
            CornerContains(point, right, top, trRadius, -1, 1) &&
            CornerContains(point, right, bottom, brRadius, -1, -1) &&
            CornerContains(point, left, bottom, blRadius, 1, -1);
    }
    private static bool CornerContains(Offset point, double x, double y, Radius radius, double xDirection, double yDirection)
    {
        if (radius.x <= 0 || radius.y <= 0) return true;
        var centerX = x + (xDirection * radius.x);
        var centerY = y + (yDirection * radius.y);
        if ((point.dx - centerX) * xDirection >= 0 || (point.dy - centerY) * yDirection >= 0) return true;
        var normalizedX = (point.dx - centerX) / radius.x;
        var normalizedY = (point.dy - centerY) / radius.y;
        return (normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1;
    }
    private static Radius Inflate(Radius radius, double delta) => new(Math.Max(0, radius.x + delta), Math.Max(0, radius.y + delta));
}

public sealed record RSuperellipse(Rect outerRect, Radius tlRadius, Radius trRadius, Radius brRadius, Radius blRadius)
{
    public RSuperellipse(Rect rect, Radius radius) : this(rect, radius, radius, radius, radius) { }
    public static RSuperellipse fromRectAndCorners(Rect rect, Radius? topLeft = null, Radius? topRight = null, Radius? bottomRight = null, Radius? bottomLeft = null) =>
        new(rect, topLeft ?? Radius.zero, topRight ?? Radius.zero, bottomRight ?? Radius.zero, bottomLeft ?? Radius.zero);
    public static RSuperellipse fromRectAndRadius(Rect rect, Radius radius) => new(rect, radius, radius, radius, radius);
    public RSuperellipse inflate(double delta) => new(outerRect.inflate(delta), tlRadius, trRadius, brRadius, blRadius);
    public RSuperellipse deflate(double delta) => inflate(-delta);
    public RSuperellipse shift(Offset offset) => this with { outerRect = outerRect.shift(offset) };
    public bool contains(Offset point) => outerRect.contains(point);
    public double tlRadiusX => tlRadius.x;
    public double tlRadiusY => tlRadius.y;
    public double trRadiusX => trRadius.x;
    public double trRadiusY => trRadius.y;
    public double brRadiusX => brRadius.x;
    public double brRadiusY => brRadius.y;
    public double blRadiusX => blRadius.x;
    public double blRadiusY => blRadius.y;
}

public abstract class Shader;

public sealed class Gradient : Shader
{
    private Gradient(Offset? begin = null, Offset? end = null, Offset? center = null, double radius = 0, Offset? focal = null, double focalRadius = 0, double startAngle = 0, double endAngle = Math.PI * 2, TileMode tileMode = TileMode.clamp)
    { this.begin = begin; this.end = end; this.center = center; this.radius = radius; this.focal = focal; this.focalRadius = focalRadius; this.startAngle = startAngle; this.endAngle = endAngle; this.tileMode = tileMode; }
    public Gradient(Offset from, Offset to, IReadOnlyList<Color> colors, IReadOnlyList<double>? colorStops = null, TileMode tileMode = TileMode.clamp, IReadOnlyList<double>? matrix4 = null) : this(begin: from, end: to, tileMode: tileMode) { }
    public Gradient(Offset center, double radius, IReadOnlyList<Color> colors, IReadOnlyList<double>? colorStops = null, TileMode tileMode = TileMode.clamp, IReadOnlyList<double>? matrix4 = null, Offset? focal = null, double focalRadius = 0) : this(center: center, radius: radius, focal: focal, focalRadius: focalRadius, tileMode: tileMode) { }
    public Gradient(Offset center, IReadOnlyList<Color> colors, IReadOnlyList<double>? colorStops = null, TileMode tileMode = TileMode.clamp, double startAngle = 0, double endAngle = Math.PI * 2, IReadOnlyList<double>? matrix4 = null) : this(center: center, startAngle: startAngle, endAngle: endAngle, tileMode: tileMode) { }
    public Offset? begin { get; }
    public Offset? end { get; }
    public Offset? center { get; }
    public double radius { get; }
    public Offset? focal { get; }
    public double focalRadius { get; }
    public double startAngle { get; }
    public double endAngle { get; }
    public TileMode tileMode { get; }
    public static Gradient linear(Offset from, Offset to, IReadOnlyList<Color> colors, IReadOnlyList<double>? colorStops = null, TileMode tileMode = TileMode.clamp, IReadOnlyList<double>? matrix4 = null) => new(from, to, colors, colorStops, tileMode, matrix4);
    public static Gradient radial(Offset center, double radius, IReadOnlyList<Color> colors, IReadOnlyList<double>? colorStops = null, TileMode tileMode = TileMode.clamp, IReadOnlyList<double>? matrix4 = null, Offset? focal = null, double focalRadius = 0) => new(center, radius, colors, colorStops, tileMode, matrix4, focal, focalRadius);
    public static Gradient sweep(Offset center, IReadOnlyList<Color> colors, IReadOnlyList<double>? colorStops = null, TileMode tileMode = TileMode.clamp, double startAngle = 0, double endAngle = Math.PI * 2, IReadOnlyList<double>? matrix4 = null) => new(center, colors, colorStops, tileMode, startAngle, endAngle, matrix4);
}

public sealed class ImageShader(Image image, TileMode tmx, TileMode tmy, Matrix4 matrix4, FilterQuality? filterQuality = null) : Shader
{
    public Image image { get; } = image;
    public TileMode tmx { get; } = tmx;
    public TileMode tmy { get; } = tmy;
    public Matrix4 matrix4 { get; } = matrix4;
    public FilterQuality? filterQuality { get; } = filterQuality;
}

public sealed record ColorFilter
{
    public static ColorFilter mode(Color color, BlendMode blendMode) => new();
    public static ColorFilter matrix(IReadOnlyList<double> matrix) => new();
    public static ColorFilter linearToSrgbGamma() => new();
    public static ColorFilter srgbToLinearGamma() => new();
}

public sealed record ImageFilter
{
    public static bool isShaderFilterSupported => false;

    public ImageFilter(double sigmaX = 0, double sigmaY = 0, TileMode tileMode = TileMode.clamp, Rect? bounds = null)
    {
        this.sigmaX = sigmaX;
        this.sigmaY = sigmaY;
        this.tileMode = tileMode;
        this.bounds = bounds;
    }

    public ImageFilter(ImageFilter outer, ImageFilter inner)
    {
        this.outer = outer;
        this.inner = inner;
    }

    public ImageFilter(ColorFilter outer, ImageFilter inner)
    {
        colorFilter = outer;
        this.inner = inner;
    }

    public ImageFilter(ColorFilter filter)
    {
        colorFilter = filter;
    }

    public ImageFilter(IReadOnlyList<double> matrix4, FilterQuality filterQuality = FilterQuality.low)
    {
        this.matrix4 = matrix4;
        this.filterQuality = filterQuality;
    }

    public ImageFilter(FragmentShader shader)
    {
        this.shader = shader ?? throw new ArgumentNullException(nameof(shader));
    }

    public double sigmaX { get; }
    public double sigmaY { get; }
    public TileMode tileMode { get; }
    public Rect? bounds { get; }
    public ImageFilter? outer { get; }
    public ImageFilter? inner { get; }
    public ColorFilter? colorFilter { get; }
    public FragmentShader? shader { get; }
    public IReadOnlyList<double>? matrix4 { get; }
    public FilterQuality filterQuality { get; }
    public string debugShortDescription => outer is not null && inner is not null
        ? $"{inner.debugShortDescription} -> {outer.debugShortDescription}"
        : matrix4 is not null ? "matrix" : "blur";
}

public sealed record MaskFilter
{
    public static MaskFilter blur(BlurStyle style, double sigma) => new();
}

public class Shadow(Color color, Offset offset, double blurRadius)
{
    public Shadow(double blurRadius = 0) : this(new Color(0xFF000000L), Offset.zero, blurRadius) { }

    public Color color { get; } = color;
    public Offset offset { get; } = offset;
    public double blurRadius { get; } = blurRadius;
    public double blurSigma => blurRadius * 0.57735 + 0.5;
    public Paint toPaint() => new() { color = color };
}

public sealed class Picture : IDisposable
{
    private int _disposed;
    public Picture(IReadOnlyList<PathCommand>? commands = null) => Commands = commands ?? [];
    public IReadOnlyList<PathCommand> Commands { get; }
    public bool debugDisposed => Volatile.Read(ref _disposed) != 0;
    public void Dispose() => Interlocked.Exchange(ref _disposed, 1);
    public void dispose() => Dispose();
    public Future<Image> toImage(long width, long height) => Future<Image>.value(new Image(0, checked((int)width), checked((int)height)));
}

public sealed class PictureRecorder
{
    private readonly List<PathCommand> _commands = [];
    public bool isRecording { get; private set; } = true;
    internal List<PathCommand> commands => _commands;
    public Picture endRecording() { isRecording = false; return new(_commands.ToArray()); }
}

public sealed record TargetImageSize(long? width, long? height);
public delegate TargetImageSize TargetImageSizeCallback(long intrinsicWidth, long intrinsicHeight);

public sealed record FrameInfo(Image image, global::Doroti.Flutter.Runtime.Duration duration);

public sealed class Codec : IDisposable
{
    private readonly IReadOnlyList<FrameInfo> _frames;
    private int _next;
    public Codec(IReadOnlyList<FrameInfo> frames, long repetitionCount = 0) { _frames = frames; this.repetitionCount = repetitionCount; }
    public long frameCount => _frames.Count;
    public long repetitionCount { get; }
    public Future<FrameInfo> getNextFrame()
    {
        if (_frames.Count == 0) return Future<FrameInfo>.error(new InvalidOperationException("codec has no frames"));
        var frame = _frames[_next++ % _frames.Count];
        return Future<FrameInfo>.value(frame);
    }
    public void Dispose() { }
    public void dispose() => Dispose();
}

public sealed record TextBox(double left, double top, double right, double bottom, TextDirection direction)
{
    public Rect toRect() => new(left, top, right, bottom);
    public double start => direction == TextDirection.ltr ? left : right;
    public double end => direction == TextDirection.ltr ? right : left;
}

public sealed record TextHeightBehavior(
    bool applyHeightToFirstAscent = true,
    bool applyHeightToLastDescent = true,
    TextLeadingDistribution? leadingDistribution = TextLeadingDistribution.proportional);

public sealed record FontFeature(string feature, long value = 1)
{
    public static FontFeature enable(string feature) => new(feature);
    public static FontFeature disable(string feature) => new(feature, 0);
}

public sealed record FontVariation(string axis, double value);

public sealed record TextDecoration(long mask)
{
    public static TextDecoration none { get; } = new(0);
    public static TextDecoration underline { get; } = new(1);
    public static TextDecoration overline { get; } = new(2);
    public static TextDecoration lineThrough { get; } = new(4);
    public static TextDecoration combine(IReadOnlyList<TextDecoration> decorations) => new(decorations.Aggregate(0L, (mask, item) => mask | item.mask));
    public bool contains(TextDecoration other) => (mask & other.mask) == other.mask;
}

public sealed class TextStyle
{
    public TextStyle(Color? color = null, TextDecoration? decoration = null, Color? decorationColor = null, TextDecorationStyle? decorationStyle = null,
        double? decorationThickness = null, FontWeight? fontWeight = null, FontStyle? fontStyle = null, TextBaseline? textBaseline = null,
        string? fontFamily = null, IReadOnlyList<string>? fontFamilyFallback = null, double? fontSize = null, double? letterSpacing = null,
        double? wordSpacing = null, double? height = null, TextLeadingDistribution? leadingDistribution = null, Locale? locale = null,
        Paint? foreground = null, Paint? background = null, IReadOnlyList<Shadow>? shadows = null, IReadOnlyList<FontFeature>? fontFeatures = null,
        IReadOnlyList<FontVariation>? fontVariations = null)
    {
        this.color = color;
        this.foreground = foreground;
        this.fontFamily = fontFamily;
        this.fontSize = fontSize;
    }

    public Color? color { get; }
    public Paint? foreground { get; }
    public string? fontFamily { get; }
    public double? fontSize { get; }
}

public sealed class ParagraphStyle
{
    public ParagraphStyle(TextAlign? textAlign = null, TextDirection? textDirection = null, long? maxLines = null, string? fontFamily = null,
        double? fontSize = null, double? height = null, FontWeight? fontWeight = null, FontStyle? fontStyle = null,
        StrutStyle? strutStyle = null, string? ellipsis = null, Locale? locale = null, TextHeightBehavior? textHeightBehavior = null)
    {
        this.fontFamily = fontFamily;
        this.fontSize = fontSize;
        this.height = height;
        this.maxLines = maxLines;
    }

    public string? fontFamily { get; }
    public double? fontSize { get; }
    public double? height { get; }
    public long? maxLines { get; }
}

public sealed class StrutStyle
{
    public StrutStyle(string? fontFamily = null, IReadOnlyList<string>? fontFamilyFallback = null, double? fontSize = null, double? height = null,
        TextLeadingDistribution? leadingDistribution = null, double? leading = null, FontWeight? fontWeight = null, FontStyle? fontStyle = null,
        bool? forceStrutHeight = false)
    { }
}

public readonly record struct ParagraphConstraints(double width);

public sealed class ParagraphBuilder
{
    private readonly ParagraphStyle _style;
    private readonly List<string> _text = [];
    private readonly Stack<TextStyle> _styles = new();
    private double _fontSize;
    private string? _fontFamily;
    private Color? _color;
    public ParagraphBuilder(ParagraphStyle style) => _style = style;
    public long placeholderCount => _text.LongCount(value => value == "\uFFFC");
    public void pushStyle(TextStyle style)
    {
        _styles.Push(style);
        _fontSize = Math.Max(_fontSize, style.fontSize ?? 0);
        _fontFamily = style.fontFamily ?? _fontFamily;
        _color = style.foreground?.color ?? style.color ?? _color;
    }
    public void pop() { if (_styles.Count > 0) _styles.Pop(); }
    public void addText(string text) => _text.Add(text);
    public void addPlaceholder(double width, double height, PlaceholderAlignment alignment, double scale = 1, double? baselineOffset = null, TextBaseline? baseline = null) => _text.Add("\uFFFC");
    public Paragraph build()
    {
        var fontSize = _fontSize > 0 ? _fontSize : _style.fontSize ?? 14;
        return new(string.Concat(_text), 0, 0, fontSize, _style.maxLines,
            _fontFamily ?? _style.fontFamily, _color ?? new Color(0xFF000000));
    }
}

public sealed record LineMetrics(bool hardBreak, double ascent, double descent, double unscaledAscent, double height, double width, double left, double baseline, long lineNumber);
public sealed record GlyphInfo(Rect graphemeClusterLayoutBounds, TextRange graphemeClusterCodeUnitRange, TextDirection writingDirection);

public abstract record StringAttribute(TextRange range)
{
    public StringAttribute copy(TextRange? range = null) => this switch
    {
        LocaleStringAttribute locale => new LocaleStringAttribute(range ?? this.range, locale.locale),
        SpellOutStringAttribute => new SpellOutStringAttribute(range ?? this.range),
        _ => throw new InvalidOperationException($"Unsupported string attribute type {GetType().Name}."),
    };
}
public sealed record LocaleStringAttribute(TextRange range, Locale locale) : StringAttribute(range);
public sealed record SpellOutStringAttribute(TextRange range) : StringAttribute(range);
