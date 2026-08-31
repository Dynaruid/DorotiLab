using System.Collections.ObjectModel;

namespace Doroti.Graphics.DisplayList;

public enum DisplayBlendMode : byte
{
    Clear,
    Source,
    Destination,
    SourceOver,
    DestinationOver,
    SourceIn,
    DestinationIn,
    SourceOut,
    DestinationOut,
    SourceAtop,
    DestinationAtop,
    Xor,
    Plus,
    Modulate,
    Screen,
    Overlay,
    Darken,
    Lighten,
    ColorDodge,
    ColorBurn,
    HardLight,
    SoftLight,
    Difference,
    Exclusion,
    Multiply,
    Hue,
    Saturation,
    Color,
    Luminosity,
}

public enum DisplayPaintStyle : byte
{
    Fill,
    Stroke,
}

public enum DisplayStrokeCap : byte
{
    Butt,
    Round,
    Square,
}

public enum DisplayStrokeJoin : byte
{
    Miter,
    Round,
    Bevel,
}

public enum DisplaySamplingQuality : byte
{
    None,
    Low,
    Medium,
    High,
}

public enum DisplayTileMode : byte
{
    Clamp,
    Repeat,
    Mirror,
    Decal,
}

public enum DisplayClipOperation : byte
{
    Difference,
    Intersect,
}

public enum DisplayPointMode : byte
{
    Points,
    Lines,
    Polygon,
}

public enum DisplayBlurStyle : byte
{
    Normal,
    Solid,
    Outer,
    Inner,
}

public enum DisplayPathFillType : byte
{
    NonZero,
    EvenOdd,
}

public enum DisplayPathVerb : byte
{
    MoveTo,
    LineTo,
    RelativeMoveTo,
    RelativeLineTo,
    QuadraticTo,
    ConicTo,
    CubicTo,
    AddRect,
    AddOval,
    AddArc,
    AddRoundedRect,
    AddSuperellipse,
    ArcToPoint,
    ArcTo,
    Close,
}

public enum DisplayTextDirection : byte
{
    LeftToRight,
    RightToLeft,
}

public enum DisplayTextAlign : byte
{
    Start,
    End,
    Left,
    Right,
    Center,
    Justify,
}

public enum DisplayFontSlant : byte
{
    Normal,
    Italic,
}

public enum DisplayTextBaseline : byte
{
    Alphabetic,
    Ideographic,
}

public enum DisplayTextDecorationStyle : byte
{
    Solid,
    Double,
    Dotted,
    Dashed,
    Wavy,
}

public readonly record struct DisplayPoint(float X, float Y);

public readonly record struct DisplayRect(float Left, float Top, float Right, float Bottom);

public readonly record struct DisplayRoundedRect(
    DisplayRect Bounds,
    float TopLeftX,
    float TopLeftY,
    float TopRightX,
    float TopRightY,
    float BottomRightX,
    float BottomRightY,
    float BottomLeftX,
    float BottomLeftY)
{
    public DisplayRoundedRect(DisplayRect bounds, float radiusX, float radiusY)
        : this(bounds, radiusX, radiusY, radiusX, radiusY, radiusX, radiusY, radiusX, radiusY)
    {
    }
}

public sealed class DisplayMatrix
{
    private readonly ReadOnlyCollection<float> _values;

    public DisplayMatrix(IEnumerable<float> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = new ReadOnlyCollection<float>(values.ToArray());
        if (_values.Count != 16)
        {
            throw new ArgumentException("A DisplayList transform must contain exactly 16 values.", nameof(values));
        }
    }

    public IReadOnlyList<float> Values => _values;

    public static DisplayMatrix Identity { get; } = new(
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1,
        ]);
}

public sealed class DisplayPath
{
    private readonly ReadOnlyCollection<DisplayPathVerb> _verbs;
    private readonly ReadOnlyCollection<float> _values;

    public DisplayPath(
        DisplayPathFillType fillType,
        IEnumerable<DisplayPathVerb> verbs,
        IEnumerable<float> values)
    {
        ArgumentNullException.ThrowIfNull(verbs);
        ArgumentNullException.ThrowIfNull(values);
        FillType = fillType;
        _verbs = new ReadOnlyCollection<DisplayPathVerb>(verbs.ToArray());
        _values = new ReadOnlyCollection<float>(values.ToArray());
    }

    public DisplayPathFillType FillType { get; }

    public IReadOnlyList<DisplayPathVerb> Verbs => _verbs;

    public IReadOnlyList<float> Values => _values;
}

public abstract record DisplayShader;

public sealed record DisplayLinearGradientShader : DisplayShader
{
    private readonly ReadOnlyCollection<uint> _colors;
    private readonly ReadOnlyCollection<float> _stops;

    public DisplayLinearGradientShader(
        DisplayPoint start,
        DisplayPoint end,
        IEnumerable<uint> colors,
        IEnumerable<float> stops,
        DisplayTileMode tileMode = DisplayTileMode.Clamp,
        DisplayMatrix? transform = null)
    {
        ArgumentNullException.ThrowIfNull(colors);
        ArgumentNullException.ThrowIfNull(stops);
        Start = start;
        End = end;
        _colors = new ReadOnlyCollection<uint>(colors.ToArray());
        _stops = new ReadOnlyCollection<float>(stops.ToArray());
        TileMode = tileMode;
        Transform = transform;
    }

    public DisplayPoint Start { get; }

    public DisplayPoint End { get; }

    public IReadOnlyList<uint> Colors => _colors;

    public IReadOnlyList<float> Stops => _stops;

    public DisplayTileMode TileMode { get; }

    public DisplayMatrix? Transform { get; }
}

public sealed record DisplayRadialGradientShader : DisplayShader
{
    private readonly ReadOnlyCollection<uint> _colors;
    private readonly ReadOnlyCollection<float> _stops;

    public DisplayRadialGradientShader(
        DisplayPoint center,
        float radius,
        IEnumerable<uint> colors,
        IEnumerable<float> stops,
        DisplayTileMode tileMode = DisplayTileMode.Clamp,
        DisplayPoint? focal = null,
        float focalRadius = 0,
        DisplayMatrix? transform = null)
    {
        ArgumentNullException.ThrowIfNull(colors);
        ArgumentNullException.ThrowIfNull(stops);
        Center = center;
        Radius = radius;
        _colors = new ReadOnlyCollection<uint>(colors.ToArray());
        _stops = new ReadOnlyCollection<float>(stops.ToArray());
        TileMode = tileMode;
        Focal = focal;
        FocalRadius = focalRadius;
        Transform = transform;
    }

    public DisplayPoint Center { get; }

    public float Radius { get; }

    public IReadOnlyList<uint> Colors => _colors;

    public IReadOnlyList<float> Stops => _stops;

    public DisplayTileMode TileMode { get; }

    public DisplayPoint? Focal { get; }

    public float FocalRadius { get; }

    public DisplayMatrix? Transform { get; }
}

public sealed record DisplaySweepGradientShader : DisplayShader
{
    private readonly ReadOnlyCollection<uint> _colors;
    private readonly ReadOnlyCollection<float> _stops;

    public DisplaySweepGradientShader(
        DisplayPoint center,
        float startAngle,
        float endAngle,
        IEnumerable<uint> colors,
        IEnumerable<float> stops,
        DisplayTileMode tileMode = DisplayTileMode.Clamp,
        DisplayMatrix? transform = null)
    {
        ArgumentNullException.ThrowIfNull(colors);
        ArgumentNullException.ThrowIfNull(stops);
        Center = center;
        StartAngle = startAngle;
        EndAngle = endAngle;
        _colors = new ReadOnlyCollection<uint>(colors.ToArray());
        _stops = new ReadOnlyCollection<float>(stops.ToArray());
        TileMode = tileMode;
        Transform = transform;
    }

    public DisplayPoint Center { get; }

    public float StartAngle { get; }

    public float EndAngle { get; }

    public IReadOnlyList<uint> Colors => _colors;

    public IReadOnlyList<float> Stops => _stops;

    public DisplayTileMode TileMode { get; }

    public DisplayMatrix? Transform { get; }
}

public sealed record DisplayImageShader(
    DisplayResourceReference Image,
    DisplayTileMode TileModeX,
    DisplayTileMode TileModeY,
    DisplaySamplingQuality Sampling,
    DisplayMatrix Transform) : DisplayShader;

public sealed record DisplayRuntimeEffectShader : DisplayShader
{
    private readonly ReadOnlyCollection<byte> _uniforms;
    private readonly ReadOnlyCollection<DisplayResourceReference> _children;

    public DisplayRuntimeEffectShader(
        DisplayResourceReference effect,
        IEnumerable<byte> uniforms,
        IEnumerable<DisplayResourceReference>? children = null)
    {
        ArgumentNullException.ThrowIfNull(uniforms);
        Effect = effect;
        _uniforms = new ReadOnlyCollection<byte>(uniforms.ToArray());
        _children = new ReadOnlyCollection<DisplayResourceReference>((children ?? []).ToArray());
    }

    public DisplayResourceReference Effect { get; }

    public IReadOnlyList<byte> Uniforms => _uniforms;

    public IReadOnlyList<DisplayResourceReference> Children => _children;
}

public abstract record DisplayColorFilter;

public sealed record DisplayBlendColorFilter(uint Color, DisplayBlendMode BlendMode) : DisplayColorFilter;

public sealed record DisplayMatrixColorFilter : DisplayColorFilter
{
    private readonly ReadOnlyCollection<float> _values;

    public DisplayMatrixColorFilter(IEnumerable<float> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        _values = new ReadOnlyCollection<float>(values.ToArray());
        if (_values.Count != 20)
        {
            throw new ArgumentException("A color matrix must contain exactly 20 values.", nameof(values));
        }
    }

    public IReadOnlyList<float> Values => _values;
}

public sealed record DisplayLinearToSrgbColorFilter : DisplayColorFilter;

public sealed record DisplaySrgbToLinearColorFilter : DisplayColorFilter;

public sealed record DisplayMaskFilter(DisplayBlurStyle Style, float Sigma);

public abstract record DisplayImageFilter;

public sealed record DisplayBlurImageFilter(
    float SigmaX,
    float SigmaY,
    DisplayTileMode TileMode,
    DisplayRect? Bounds = null) : DisplayImageFilter;

public sealed record DisplayColorImageFilter(DisplayColorFilter Filter) : DisplayImageFilter;

public sealed record DisplayMatrixImageFilter(
    DisplayMatrix Matrix,
    DisplaySamplingQuality Sampling) : DisplayImageFilter;

public sealed record DisplayRuntimeEffectImageFilter(
    DisplayRuntimeEffectShader Shader,
    DisplaySamplingQuality Sampling) : DisplayImageFilter;

public sealed record DisplayComposeImageFilter(
    DisplayImageFilter Outer,
    DisplayImageFilter Inner) : DisplayImageFilter;

public sealed record DisplayDropShadowImageFilter(
    float DeltaX,
    float DeltaY,
    float SigmaX,
    float SigmaY,
    uint Color,
    bool ShadowOnly) : DisplayImageFilter;

public sealed record DisplayPaint(
    uint Color,
    DisplayPaintStyle Style = DisplayPaintStyle.Fill,
    float StrokeWidth = 0,
    float StrokeMiterLimit = 4,
    DisplayStrokeCap StrokeCap = DisplayStrokeCap.Butt,
    DisplayStrokeJoin StrokeJoin = DisplayStrokeJoin.Miter,
    bool IsAntiAlias = true,
    DisplayBlendMode BlendMode = DisplayBlendMode.SourceOver,
    DisplaySamplingQuality Sampling = DisplaySamplingQuality.None,
    bool InvertColors = false,
    DisplayShader? Shader = null,
    DisplayColorFilter? ColorFilter = null,
    DisplayMaskFilter? MaskFilter = null,
    DisplayImageFilter? ImageFilter = null);

public sealed class DisplayParagraphRecipe
{
    private readonly ReadOnlyCollection<DisplayResourceReference> _fallbackFonts;
    private readonly ReadOnlyCollection<DisplayParagraphTextRun> _textRuns;

    public DisplayParagraphRecipe(
        string text,
        DisplayResourceReference font,
        string fontFamily,
        float fontSize,
        float heightMultiplier,
        uint color,
        int fontWeight,
        DisplayFontSlant fontSlant,
        DisplayTextDirection direction,
        DisplayTextAlign align,
        string locale,
        uint maxLines,
        string? ellipsis,
        float layoutWidth,
        float measuredWidth,
        float measuredHeight,
        ulong metricsHash,
        IEnumerable<DisplayResourceReference>? fallbackFonts = null,
        IEnumerable<DisplayParagraphTextRun>? textRuns = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(fontFamily);
        ArgumentNullException.ThrowIfNull(locale);
        Text = text;
        Font = font;
        FontFamily = fontFamily;
        FontSize = fontSize;
        HeightMultiplier = heightMultiplier;
        Color = color;
        FontWeight = fontWeight;
        FontSlant = fontSlant;
        Direction = direction;
        Align = align;
        Locale = locale;
        MaxLines = maxLines;
        Ellipsis = ellipsis;
        LayoutWidth = layoutWidth;
        MeasuredWidth = measuredWidth;
        MeasuredHeight = measuredHeight;
        MetricsHash = metricsHash;
        _fallbackFonts = new ReadOnlyCollection<DisplayResourceReference>((fallbackFonts ?? []).ToArray());
        _textRuns = new ReadOnlyCollection<DisplayParagraphTextRun>((textRuns ?? []).ToArray());
    }

    public string Text { get; }

    public DisplayResourceReference Font { get; }

    public string FontFamily { get; }

    public float FontSize { get; }

    public float HeightMultiplier { get; }

    public uint Color { get; }

    public int FontWeight { get; }

    public DisplayFontSlant FontSlant { get; }

    public DisplayTextDirection Direction { get; }

    public DisplayTextAlign Align { get; }

    public string Locale { get; }

    public uint MaxLines { get; }

    public string? Ellipsis { get; }

    public float LayoutWidth { get; }

    public float MeasuredWidth { get; }

    public float MeasuredHeight { get; }

    public ulong MetricsHash { get; }

    public IReadOnlyList<DisplayResourceReference> FallbackFonts => _fallbackFonts;

    public IReadOnlyList<DisplayParagraphTextRun> TextRuns => _textRuns;
}

public sealed record DisplayTextShadow(uint Color, float DeltaX, float DeltaY, float BlurRadius);

public sealed record DisplayFontFeature(string Name, int Value);

public sealed record DisplayFontVariation(string Axis, float Value);

public sealed class DisplayParagraphTextRun
{
    private readonly ReadOnlyCollection<string> _fontFamilyFallback;
    private readonly ReadOnlyCollection<DisplayTextShadow> _shadows;
    private readonly ReadOnlyCollection<DisplayFontFeature> _fontFeatures;
    private readonly ReadOnlyCollection<DisplayFontVariation> _fontVariations;

    public DisplayParagraphTextRun(
        string text,
        string fontFamily,
        string locale,
        float fontSize,
        float heightMultiplier,
        uint color,
        int fontWeight,
        DisplayFontSlant fontSlant,
        uint decoration = 0,
        uint? backgroundColor = null,
        uint? decorationColor = null,
        DisplayTextDecorationStyle? decorationStyle = null,
        float? decorationThickness = null,
        DisplayTextBaseline? textBaseline = null,
        float? letterSpacing = null,
        float? wordSpacing = null,
        bool? halfLeading = null,
        IEnumerable<string>? fontFamilyFallback = null,
        IEnumerable<DisplayTextShadow>? shadows = null,
        IEnumerable<DisplayFontFeature>? fontFeatures = null,
        IEnumerable<DisplayFontVariation>? fontVariations = null)
    {
        ArgumentNullException.ThrowIfNull(text);
        ArgumentNullException.ThrowIfNull(fontFamily);
        ArgumentNullException.ThrowIfNull(locale);
        Text = text;
        FontFamily = fontFamily;
        Locale = locale;
        FontSize = fontSize;
        HeightMultiplier = heightMultiplier;
        Color = color;
        FontWeight = fontWeight;
        FontSlant = fontSlant;
        Decoration = decoration;
        BackgroundColor = backgroundColor;
        DecorationColor = decorationColor;
        DecorationStyle = decorationStyle;
        DecorationThickness = decorationThickness;
        TextBaseline = textBaseline;
        LetterSpacing = letterSpacing;
        WordSpacing = wordSpacing;
        HalfLeading = halfLeading;
        _fontFamilyFallback = new ReadOnlyCollection<string>((fontFamilyFallback ?? []).ToArray());
        _shadows = new ReadOnlyCollection<DisplayTextShadow>((shadows ?? []).ToArray());
        _fontFeatures = new ReadOnlyCollection<DisplayFontFeature>((fontFeatures ?? []).ToArray());
        _fontVariations = new ReadOnlyCollection<DisplayFontVariation>((fontVariations ?? []).ToArray());
    }

    public string Text { get; }
    public string FontFamily { get; }
    public string Locale { get; }
    public float FontSize { get; }
    public float HeightMultiplier { get; }
    public uint Color { get; }
    public int FontWeight { get; }
    public DisplayFontSlant FontSlant { get; }
    public uint Decoration { get; }
    public uint? BackgroundColor { get; }
    public uint? DecorationColor { get; }
    public DisplayTextDecorationStyle? DecorationStyle { get; }
    public float? DecorationThickness { get; }
    public DisplayTextBaseline? TextBaseline { get; }
    public float? LetterSpacing { get; }
    public float? WordSpacing { get; }
    public bool? HalfLeading { get; }
    public IReadOnlyList<string> FontFamilyFallback => _fontFamilyFallback;
    public IReadOnlyList<DisplayTextShadow> Shadows => _shadows;
    public IReadOnlyList<DisplayFontFeature> FontFeatures => _fontFeatures;
    public IReadOnlyList<DisplayFontVariation> FontVariations => _fontVariations;
}
