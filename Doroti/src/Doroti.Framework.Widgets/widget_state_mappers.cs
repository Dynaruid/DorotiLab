using Doroti.Framework.Painting;
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

internal interface IWidgetStateMapping<T>
{
    DartMap<WidgetStatesConstraint, T> Mapping { get; }
}

internal static class WidgetStateMapping
{
    internal static Exception Unresolved(string member) =>
        new FlutterError($"Resolve the WidgetStateProperty before accessing {member}.");

    internal static T ResolveRequired<T>(WidgetStateMapper<T> mapper, HashSet<WidgetState> states)
        where T : class =>
        mapper.resolve(states)
        ?? throw new DartArgumentError(
            states,
            nameof(states),
            "No widget-state map key matched the current states. Add WidgetState.any."
        );
}

internal sealed class _WidgetStateColorMapper__widget_state
    : WidgetStateColor,
        IWidgetStateMapping<Color>
{
    private readonly WidgetStateMapper<Color> mapper;
    DartMap<WidgetStatesConstraint, Color> IWidgetStateMapping<Color>.Mapping =>
        ((IWidgetStateMapping<Color>)mapper).Mapping;

    internal _WidgetStateColorMapper__widget_state(DartMap<WidgetStatesConstraint, Color> map)
        : base(0) => mapper = new(map);

    public override Color resolve(HashSet<WidgetState> states) =>
        WidgetStateMapping.ResolveRequired(mapper, states);

    public override bool Equals(object? other) => mapper.Equals(other);

    public override int GetHashCode() => mapper.GetHashCode();

    public override string ToString() => mapper.ToString();

    public override uint value => throw WidgetStateMapping.Unresolved(nameof(value));
    public override ColorSpace colorSpace =>
        throw WidgetStateMapping.Unresolved(nameof(colorSpace));

    public override Color resolveFrom<TContext>(TContext context) =>
        throw WidgetStateMapping.Unresolved(nameof(resolveFrom));

    public override Color withValues(
        double? alpha = null,
        double? red = null,
        double? green = null,
        double? blue = null,
        ColorSpace? colorSpace = null
    ) => throw WidgetStateMapping.Unresolved(nameof(withValues));

    public override bool Equals(Color? other) => mapper.Equals(other);
}

internal sealed class _WidgetMouseCursorMapper__widget_state
    : WidgetStateMouseCursor,
        IWidgetStateMapping<MouseCursor>
{
    private readonly WidgetStateMapper<MouseCursor> mapper;
    DartMap<WidgetStatesConstraint, MouseCursor> IWidgetStateMapping<MouseCursor>.Mapping =>
        ((IWidgetStateMapping<MouseCursor>)mapper).Mapping;

    internal _WidgetMouseCursorMapper__widget_state(
        DartMap<WidgetStatesConstraint, MouseCursor> map
    ) => mapper = new(map);

    public override MouseCursor resolve(HashSet<WidgetState> states) =>
        WidgetStateMapping.ResolveRequired(mapper, states);

    public override bool Equals(object? other) => mapper.Equals(other);

    public override int GetHashCode() => mapper.GetHashCode();

    public override string ToString() => mapper.ToString();

    public override string debugDescription =>
        throw WidgetStateMapping.Unresolved(nameof(debugDescription));

    public override MouseCursorSession createSession(long device) =>
        throw WidgetStateMapping.Unresolved(nameof(createSession));
}

internal sealed class _WidgetBorderSideMapper__widget_state
    : WidgetStateBorderSide,
        IWidgetStateMapping<BorderSide?>
{
    private readonly WidgetStateMapper<BorderSide?> mapper;
    DartMap<WidgetStatesConstraint, BorderSide?> IWidgetStateMapping<BorderSide?>.Mapping =>
        ((IWidgetStateMapping<BorderSide?>)mapper).Mapping;

    internal _WidgetBorderSideMapper__widget_state(
        DartMap<WidgetStatesConstraint, BorderSide?> map
    ) => mapper = new(map);

    public override BorderSide? resolve(HashSet<WidgetState> states) => mapper.resolve(states);

    public override bool Equals(object? other) => mapper.Equals(other);

    public override int GetHashCode() => mapper.GetHashCode();

    public override string ToString() => mapper.ToString();

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties) =>
        mapper.debugFillProperties(properties);

    public override Color color => throw WidgetStateMapping.Unresolved(nameof(color));
    public override double width => throw WidgetStateMapping.Unresolved(nameof(width));
    public override BorderStyle style => throw WidgetStateMapping.Unresolved(nameof(style));
    public override double strokeAlign => throw WidgetStateMapping.Unresolved(nameof(strokeAlign));

    public override BorderSide copyWith(
        Color? color = null,
        double? width = null,
        BorderStyle? style = null,
        double? strokeAlign = null
    ) => throw WidgetStateMapping.Unresolved(nameof(copyWith));

    public override BorderSide scale(double t) =>
        throw WidgetStateMapping.Unresolved(nameof(scale));

    public override Paint toPaint() => throw WidgetStateMapping.Unresolved(nameof(toPaint));

    public override double strokeInset => throw WidgetStateMapping.Unresolved(nameof(strokeInset));
    public override double strokeOutset =>
        throw WidgetStateMapping.Unresolved(nameof(strokeOutset));
    public override double strokeOffset =>
        throw WidgetStateMapping.Unresolved(nameof(strokeOffset));
}

internal sealed class _WidgetOutlinedBorderMapper__widget_state
    : WidgetStateOutlinedBorder,
        IWidgetStateMapping<OutlinedBorder?>
{
    private readonly WidgetStateMapper<OutlinedBorder?> mapper;
    DartMap<WidgetStatesConstraint, OutlinedBorder?> IWidgetStateMapping<OutlinedBorder?>.Mapping =>
        ((IWidgetStateMapping<OutlinedBorder?>)mapper).Mapping;

    internal _WidgetOutlinedBorderMapper__widget_state(
        DartMap<WidgetStatesConstraint, OutlinedBorder?> map
    ) => mapper = new(map);

    public override OutlinedBorder? resolve(HashSet<WidgetState> states) => mapper.resolve(states);

    public override bool Equals(object? other) => mapper.Equals(other);

    public override int GetHashCode() => mapper.GetHashCode();

    public override string ToString() => mapper.ToString();

    public override EdgeInsetsGeometry dimensions =>
        throw WidgetStateMapping.Unresolved(nameof(dimensions));

    public override ShapeBorder? add(ShapeBorder other, bool reversed = false) =>
        throw WidgetStateMapping.Unresolved(nameof(add));

    public override ShapeBorder op_Add(ShapeBorder other) =>
        throw WidgetStateMapping.Unresolved(nameof(op_Add));

    public override ShapeBorder scale(double t) =>
        throw WidgetStateMapping.Unresolved(nameof(scale));

    public override ShapeBorder? lerpFrom(ShapeBorder? a, double t) =>
        throw WidgetStateMapping.Unresolved(nameof(lerpFrom));

    public override ShapeBorder? lerpTo(ShapeBorder? b, double t) =>
        throw WidgetStateMapping.Unresolved(nameof(lerpTo));

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null) =>
        throw WidgetStateMapping.Unresolved(nameof(getOuterPath));

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null) =>
        throw WidgetStateMapping.Unresolved(nameof(getInnerPath));

    public override bool hitTest(Rect rect, Offset position, TextDirection? textDirection = null) =>
        throw WidgetStateMapping.Unresolved(nameof(hitTest));

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    ) => throw WidgetStateMapping.Unresolved(nameof(paintInterior));

    public override bool preferPaintInterior =>
        throw WidgetStateMapping.Unresolved(nameof(preferPaintInterior));

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    ) => throw WidgetStateMapping.Unresolved(nameof(paint));

    public override BorderSide side => throw WidgetStateMapping.Unresolved(nameof(side));

    public override OutlinedBorder copyWith(
        BorderSide? side = null,
        BorderRadiusGeometry? borderRadius = null,
        double? eccentricity = null,
        LinearBorderEdge? start = null,
        LinearBorderEdge? end = null,
        LinearBorderEdge? top = null,
        LinearBorderEdge? bottom = null,
        double? circularity = null,
        double? rectilinearity = null,
        double? points = null,
        double? innerRadiusRatio = null,
        double? pointRounding = null,
        double? valleyRounding = null,
        double? rotation = null,
        double? squash = null
    ) => throw WidgetStateMapping.Unresolved(nameof(copyWith));
}

internal sealed class _WidgetTextStyleMapper__widget_state
    : WidgetStateTextStyle,
        IWidgetStateMapping<TextStyle>
{
    private readonly WidgetStateMapper<TextStyle> mapper;
    DartMap<WidgetStatesConstraint, TextStyle> IWidgetStateMapping<TextStyle>.Mapping =>
        ((IWidgetStateMapping<TextStyle>)mapper).Mapping;

    internal _WidgetTextStyleMapper__widget_state(DartMap<WidgetStatesConstraint, TextStyle> map) =>
        mapper = new(map);

    public override TextStyle resolve(HashSet<WidgetState> states) =>
        WidgetStateMapping.ResolveRequired(mapper, states);

    public override bool Equals(object? other) => mapper.Equals(other);

    public override int GetHashCode() => mapper.GetHashCode();

    public override string ToString() => mapper.ToString();

    public override void debugFillProperties(
        DiagnosticPropertiesBuilder properties,
        string prefix = ""
    ) => mapper.debugFillProperties(properties);

    public override bool inherit => throw WidgetStateMapping.Unresolved(nameof(inherit));
    public override Color? color => throw WidgetStateMapping.Unresolved(nameof(color));
    public override Color? backgroundColor =>
        throw WidgetStateMapping.Unresolved(nameof(backgroundColor));
    public override string? fontFamily => throw WidgetStateMapping.Unresolved(nameof(fontFamily));
    public override double? fontSize => throw WidgetStateMapping.Unresolved(nameof(fontSize));
    public override FontWeight? fontWeight =>
        throw WidgetStateMapping.Unresolved(nameof(fontWeight));
    public override FontStyle? fontStyle => throw WidgetStateMapping.Unresolved(nameof(fontStyle));
    public override double? letterSpacing =>
        throw WidgetStateMapping.Unresolved(nameof(letterSpacing));
    public override double? wordSpacing => throw WidgetStateMapping.Unresolved(nameof(wordSpacing));
    public override TextBaseline? textBaseline =>
        throw WidgetStateMapping.Unresolved(nameof(textBaseline));
    public override double? height => throw WidgetStateMapping.Unresolved(nameof(height));
    public override TextLeadingDistribution? leadingDistribution =>
        throw WidgetStateMapping.Unresolved(nameof(leadingDistribution));
    public override Locale? locale => throw WidgetStateMapping.Unresolved(nameof(locale));
    public override Paint? foreground => throw WidgetStateMapping.Unresolved(nameof(foreground));
    public override Paint? background => throw WidgetStateMapping.Unresolved(nameof(background));
    public override TextDecoration? decoration =>
        throw WidgetStateMapping.Unresolved(nameof(decoration));
    public override Color? decorationColor =>
        throw WidgetStateMapping.Unresolved(nameof(decorationColor));
    public override TextDecorationStyle? decorationStyle =>
        throw WidgetStateMapping.Unresolved(nameof(decorationStyle));
    public override double? decorationThickness =>
        throw WidgetStateMapping.Unresolved(nameof(decorationThickness));
    public override string? debugLabel => throw WidgetStateMapping.Unresolved(nameof(debugLabel));
    public override List<Shadow>? shadows => throw WidgetStateMapping.Unresolved(nameof(shadows));
    public override List<FontFeature>? fontFeatures =>
        throw WidgetStateMapping.Unresolved(nameof(fontFeatures));
    public override List<FontVariation>? fontVariations =>
        throw WidgetStateMapping.Unresolved(nameof(fontVariations));
    public override TextOverflow? overflow => throw WidgetStateMapping.Unresolved(nameof(overflow));
    public override List<string>? fontFamilyFallback =>
        throw WidgetStateMapping.Unresolved(nameof(fontFamilyFallback));

    public override TextStyle copyWith(
        bool? inherit = null,
        Color? color = null,
        Color? backgroundColor = null,
        double? fontSize = null,
        FontWeight? fontWeight = null,
        FontStyle? fontStyle = null,
        double? letterSpacing = null,
        double? wordSpacing = null,
        TextBaseline? textBaseline = null,
        double? height = null,
        TextLeadingDistribution? leadingDistribution = null,
        Locale? locale = null,
        Paint? foreground = null,
        Paint? background = null,
        List<Shadow>? shadows = null,
        List<FontFeature>? fontFeatures = null,
        List<FontVariation>? fontVariations = null,
        TextDecoration? decoration = null,
        Color? decorationColor = null,
        TextDecorationStyle? decorationStyle = null,
        double? decorationThickness = null,
        string? debugLabel = null,
        string? fontFamily = null,
        List<string>? fontFamilyFallback = null,
        string? package = null,
        TextOverflow? overflow = null
    ) => throw WidgetStateMapping.Unresolved(nameof(copyWith));

    public override TextStyle apply(
        Color? color = null,
        Color? backgroundColor = null,
        TextDecoration? decoration = null,
        Color? decorationColor = null,
        TextDecorationStyle? decorationStyle = null,
        double decorationThicknessFactor = 1.0,
        double decorationThicknessDelta = 0.0,
        string? fontFamily = null,
        List<string>? fontFamilyFallback = null,
        double fontSizeFactor = 1.0,
        double fontSizeDelta = 0.0,
        long fontWeightDelta = 0,
        FontStyle? fontStyle = null,
        double letterSpacingFactor = 1.0,
        double letterSpacingDelta = 0.0,
        double wordSpacingFactor = 1.0,
        double wordSpacingDelta = 0.0,
        double heightFactor = 1.0,
        double heightDelta = 0.0,
        TextBaseline? textBaseline = null,
        TextLeadingDistribution? leadingDistribution = null,
        Locale? locale = null,
        List<Shadow>? shadows = null,
        List<FontFeature>? fontFeatures = null,
        List<FontVariation>? fontVariations = null,
        string? package = null,
        TextOverflow? overflow = null
    ) => throw WidgetStateMapping.Unresolved(nameof(apply));

    public override TextStyle merge(TextStyle? other) =>
        throw WidgetStateMapping.Unresolved(nameof(merge));

    public override Ui.TextStyle getTextStyle(
        double textScaleFactor = 1.0,
        TextScaler textScaler = default!
    ) => throw WidgetStateMapping.Unresolved(nameof(getTextStyle));

    public override ParagraphStyle getParagraphStyle(
        TextAlign? textAlign = null,
        TextDirection? textDirection = null,
        TextScaler textScaler = default!,
        string? ellipsis = null,
        long? maxLines = null,
        TextHeightBehavior? textHeightBehavior = null,
        Locale? locale = null,
        string? fontFamily = null,
        double? fontSize = null,
        FontWeight? fontWeight = null,
        FontStyle? fontStyle = null,
        double? height = null,
        Painting.StrutStyle? strutStyle = null
    ) => throw WidgetStateMapping.Unresolved(nameof(getParagraphStyle));

    public override RenderComparison compareTo(TextStyle other) =>
        throw WidgetStateMapping.Unresolved(nameof(compareTo));
}

internal sealed class _WidgetStateOutlinedBorder__widget_state : WidgetStateOutlinedBorder
{
    private readonly Func<HashSet<WidgetState>, OutlinedBorder?> resolver;
    private readonly RoundedRectangleBorder shape = new();

    internal _WidgetStateOutlinedBorder__widget_state(
        Func<HashSet<WidgetState>, OutlinedBorder?> resolver
    ) => this.resolver = resolver;

    public override OutlinedBorder? resolve(HashSet<WidgetState> states) => resolver(states);

    public override EdgeInsetsGeometry dimensions => shape.dimensions;

    public override Path getOuterPath(Rect rect, TextDirection? textDirection = null) =>
        shape.getOuterPath(rect, textDirection);

    public override Path getInnerPath(Rect rect, TextDirection? textDirection = null) =>
        shape.getInnerPath(rect, textDirection);

    public override ShapeBorder scale(double t) => shape.scale(t);

    public override void paint(
        Canvas canvas,
        Rect rect,
        TextDirection? textDirection = null,
        BoxShape shape = BoxShape.rectangle,
        BorderRadius? borderRadius = null
    ) => this.shape.paint(canvas, rect, textDirection, shape, borderRadius);

    public override void paintInterior(
        Canvas canvas,
        Rect rect,
        Paint paint,
        TextDirection? textDirection = null
    ) => shape.paintInterior(canvas, rect, paint, textDirection);

    public override bool preferPaintInterior => shape.preferPaintInterior;

    public override OutlinedBorder copyWith(
        BorderSide? side = null,
        BorderRadiusGeometry? borderRadius = null,
        double? eccentricity = null,
        LinearBorderEdge? start = null,
        LinearBorderEdge? end = null,
        LinearBorderEdge? top = null,
        LinearBorderEdge? bottom = null,
        double? circularity = null,
        double? rectilinearity = null,
        double? points = null,
        double? innerRadiusRatio = null,
        double? pointRounding = null,
        double? valleyRounding = null,
        double? rotation = null,
        double? squash = null
    ) =>
        shape.copyWith(
            side,
            borderRadius,
            eccentricity,
            start,
            end,
            top,
            bottom,
            circularity,
            rectilinearity,
            points,
            innerRadiusRatio,
            pointRounding,
            valleyRounding,
            rotation,
            squash
        );
}
