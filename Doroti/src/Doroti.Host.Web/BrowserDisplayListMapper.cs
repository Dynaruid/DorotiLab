using System.Buffers.Binary;
using Doroti.Graphics.DisplayList;
using Doroti.Ui;
using UiImage = Doroti.Ui.Image;

namespace Doroti.Host.Web;

internal interface IBrowserDisplayListResources
{
    DisplayResourceReference DefaultFont { get; }

    DisplayResourceDescriptor Describe(DisplayResourceReference reference);

    DisplayResourceReference ResolveImage(UiImage image);

    DisplayResourceReference ResolveRuntimeEffect(FragmentShaderState effect);
}

/// <summary>
/// Converts the immutable framework scene snapshot into renderer-neutral values.
/// No SkiaSharp or JavaScript/CanvasKit object crosses this boundary.
/// </summary>
internal static class BrowserDisplayListMapper
{
    internal static DisplayListDocument Create(
        Scene scene,
        DisplayListSceneMetadata metadata,
        uint backgroundColor,
        IBrowserDisplayListResources resources)
    {
        ArgumentNullException.ThrowIfNull(scene);
        ArgumentNullException.ThrowIfNull(resources);
        if (scene.viewId != metadata.ViewId)
            throw new InvalidDataException(
                $"Doroti scene view {scene.viewId} does not match DisplayList view {metadata.ViewId}.");
        var scopedResources = new ViewScopedResources(metadata.ViewId, resources);
        var commands = new List<DisplayListCommand>
        {
            new DisplayDrawColorCommand(backgroundColor, DisplayBlendMode.Source),
        };
        var referenced = new Dictionary<DisplayResourceReference, DisplayResourceDescriptor>();
        AppendScene(scene.Commands, commands, referenced, scopedResources);
        return new(metadata, referenced.Values, commands);
    }

    private static void AppendScene(
        IReadOnlyList<SceneCommand> source,
        List<DisplayListCommand> destination,
        Dictionary<DisplayResourceReference, DisplayResourceDescriptor> referenced,
        IBrowserDisplayListResources resources)
    {
        foreach (var command in source)
        {
            switch (command.Operation)
            {
                case "picture" when command.HostPayload is ScenePicturePayload picture:
                    destination.Add(new DisplaySaveCommand());
                    destination.Add(new DisplayTransformCommand(Translation(picture.Offset.dx, picture.Offset.dy)));
                    AppendPicture(picture.Commands, destination, referenced, resources);
                    destination.Add(new DisplayRestoreCommand());
                    break;
                case "offset" when command.HostPayload is SceneOffsetPayload offset:
                    destination.Add(new DisplaySaveCommand());
                    destination.Add(new DisplayTransformCommand(Translation(offset.Dx, offset.Dy)));
                    break;
                case "clipRect" when command.HostPayload is SceneClipRectPayload clip:
                    destination.Add(new DisplaySaveCommand());
                    destination.Add(new DisplayClipRectCommand(ToRect(clip.Rect)));
                    break;
                case "clipRRect" when command.HostPayload is SceneClipRRectPayload clip:
                    destination.Add(new DisplaySaveCommand());
                    destination.Add(new DisplayClipRoundedRectCommand(ToRoundedRect(clip.RRect)));
                    break;
                case "clipRSuperellipse" when command.HostPayload is SceneClipRSuperellipsePayload clip:
                    destination.Add(new DisplaySaveCommand());
                    // Match the current shared Skia renderer until it grows a true
                    // superellipse primitive: scene superellipses clip their outer rect.
                    destination.Add(new DisplayClipRectCommand(ToRect(clip.RSuperellipse.outerRect)));
                    break;
                case "clipPath" when command.HostPayload is SceneClipPathPayload clip:
                    destination.Add(new DisplaySaveCommand());
                    destination.Add(new DisplayClipPathCommand(ToPath(clip.Path)));
                    break;
                case "transform" when command.HostPayload is SceneTransformPayload transform:
                    destination.Add(new DisplaySaveCommand());
                    destination.Add(new DisplayTransformCommand(ToMatrix(transform.Matrix4)));
                    break;
                case "opacity" when command.HostPayload is SceneOpacityPayload opacity:
                    destination.Add(new DisplayPushOpacityCommand(
                        checked((float)opacity.Opacity), ToPoint(opacity.Offset)));
                    break;
                case "colorFilter" when command.HostPayload is SceneColorFilterPayload filter:
                    destination.Add(new DisplayPushColorFilterCommand(
                        ToColorFilter(filter.Filter), default));
                    break;
                case "imageFilter" when command.HostPayload is SceneImageFilterPayload filter:
                    destination.Add(new DisplayPushImageFilterCommand(
                        ToImageFilter(filter.Filter, referenced, resources), ToPoint(filter.Offset),
                        filter.Bounds is { } bounds ? ToRect(bounds) : null));
                    break;
                case "backdropFilter" when command.HostPayload is SceneBackdropFilterPayload filter:
                    destination.Add(new DisplayPushBackdropFilterCommand(
                        ToImageFilter(filter.Filter, referenced, resources),
                        ToBlendMode(filter.BlendMode),
                        StableValueIdentity(filter.BackdropId),
                        default));
                    break;
                case "shaderMask" when command.HostPayload is SceneShaderMaskPayload mask:
                    destination.Add(new DisplayPushShaderMaskCommand(
                        ToShader(mask.Shader, referenced, resources),
                        ToRect(mask.MaskRect),
                        ToBlendMode(mask.BlendMode)));
                    break;
                case "retained" when command.HostPayload is SceneRetainedPayload retained:
                    // Retained CLR array identity is deliberately not a wire key.  Inline its
                    // immutable value snapshot until the producer assigns a stable resource ID.
                    AppendScene(retained.Commands, destination, referenced, resources);
                    break;
                case "pop":
                    destination.Add(new DisplayRestoreCommand());
                    break;
                default:
                    throw new NotSupportedException(
                        $"Doroti scene operation '{command.Operation}' has no DisplayList mapping.");
            }
        }
    }

    private static void AppendPicture(
        IReadOnlyList<PathCommand> source,
        List<DisplayListCommand> destination,
        Dictionary<DisplayResourceReference, DisplayResourceDescriptor> referenced,
        IBrowserDisplayListResources resources)
    {
        foreach (var command in source)
        {
            switch (command.Operation)
            {
                case "save": destination.Add(new DisplaySaveCommand()); break;
                case "restore": destination.Add(new DisplayRestoreCommand()); break;
                case "saveLayer" when command.HostPayload is CanvasSaveLayerPayload layer:
                    destination.Add(new DisplaySaveLayerCommand(
                        layer.Bounds is { } bounds ? ToRect(bounds) : null,
                        ToPaint(layer.Paint, referenced, resources)));
                    break;
                case "translate":
                    destination.Add(new DisplayTransformCommand(Translation(
                        Value(command, 0), Value(command, 1))));
                    break;
                case "scale":
                    destination.Add(new DisplayTransformCommand(Scale(
                        Value(command, 0), Value(command, 1))));
                    break;
                case "rotate":
                    destination.Add(new DisplayTransformCommand(Rotation(Value(command, 0))));
                    break;
                case "skew":
                    destination.Add(new DisplayTransformCommand(Skew(
                        Value(command, 0), Value(command, 1))));
                    break;
                case "transform":
                    destination.Add(new DisplayTransformCommand(ToMatrix(command.Arguments)));
                    break;
                case "clipRect":
                    destination.Add(new DisplayClipRectCommand(
                        new(Value(command, 0), Value(command, 1), Value(command, 2), Value(command, 3)),
                        command.Arguments.Count > 4 && command.Arguments.Count > 5
                            ? (DisplayClipOperation)checked((byte)Value(command, 4))
                            : DisplayClipOperation.Intersect,
                        command.Arguments.Count <= 4 || command.Arguments.Count == 5
                            ? command.Arguments.Count < 5 || Value(command, 4) != 0
                            : Value(command, 5) != 0));
                    break;
                case "clipRRect" when command.HostPayload is CanvasClipRRectPayload clip:
                    destination.Add(new DisplayClipRoundedRectCommand(ToRoundedRect(clip.RRect)));
                    break;
                case "clipRSuperellipse" when command.HostPayload is CanvasClipRSuperellipsePayload clip:
                    destination.Add(new DisplayClipRectCommand(
                        ToRect(clip.RSuperellipse.outerRect), IsAntiAlias: clip.DoAntiAlias));
                    break;
                case "clipPath" when command.HostPayload is CanvasClipPathPayload clip:
                    destination.Add(new DisplayClipPathCommand(ToPath(clip.Path)));
                    break;
                case "drawRect" when command.HostPayload is CanvasRectPayload draw:
                    destination.Add(new DisplayDrawRectCommand(
                        ToRect(draw.Rect), ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawRRect" when command.HostPayload is CanvasRRectPayload draw:
                    destination.Add(new DisplayDrawRoundedRectCommand(
                        ToRoundedRect(draw.RRect), ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawRSuperellipse" when command.HostPayload is CanvasRSuperellipsePayload draw:
                    destination.Add(new DisplayDrawRectCommand(
                        ToRect(draw.RSuperellipse.outerRect), ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawDRRect" when command.HostPayload is CanvasDRRectPayload draw:
                    destination.Add(new DisplayDrawDoubleRoundedRectCommand(
                        ToRoundedRect(draw.Outer), ToRoundedRect(draw.Inner),
                        ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawPath" when command.HostPayload is CanvasPathPayload draw:
                    destination.Add(new DisplayDrawPathCommand(
                        ToPath(draw.Path), ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawPaint" when command.HostPayload is PaintSnapshot draw:
                    destination.Add(new DisplayDrawPaintCommand(ToPaint(draw, referenced, resources)));
                    break;
                case "drawCircle" when command.HostPayload is CanvasCirclePayload draw:
                    destination.Add(new DisplayDrawCircleCommand(
                        ToPoint(draw.Center), checked((float)draw.Radius),
                        ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawOval" when command.HostPayload is CanvasOvalPayload draw:
                    destination.Add(new DisplayDrawOvalCommand(
                        ToRect(draw.Rect), ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawLine" when command.HostPayload is CanvasLinePayload draw:
                    destination.Add(new DisplayDrawLineCommand(
                        ToPoint(draw.Start), ToPoint(draw.End),
                        ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawPoints" or "drawRawPoints" when command.HostPayload is CanvasPointsPayload draw:
                    destination.Add(new DisplayDrawPointsCommand(
                        (DisplayPointMode)draw.PointMode,
                        draw.Points.Select(ToPoint),
                        ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawArc" when command.HostPayload is CanvasArcPayload draw:
                    destination.Add(new DisplayDrawArcCommand(
                        ToRect(draw.Rect), checked((float)draw.StartAngle), checked((float)draw.SweepAngle),
                        draw.UseCenter, ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawColor" when command.HostPayload is CanvasColorPayload draw:
                    destination.Add(new DisplayDrawColorCommand(
                        draw.Color.value, ToBlendMode(draw.BlendMode)));
                    break;
                case "drawParagraph" when command.HostPayload is CanvasParagraphPayload draw:
                    var font = resources.DefaultFont;
                    AddReference(font, referenced, resources);
                    destination.Add(new DisplayDrawParagraphCommand(
                        new DisplayParagraphRecipe(
                            draw.Paragraph.text,
                            font,
                            draw.Paragraph.fontFamily ?? "DorotiFallback",
                            checked((float)draw.Paragraph.fontSize),
                            draw.Paragraph.CanvasKitHeightMultiplier,
                            draw.Paragraph.color.value,
                            400,
                            DisplayFontSlant.Normal,
                            draw.Paragraph.CanvasKitTextDirection == TextDirection.rtl
                                ? DisplayTextDirection.RightToLeft
                                : DisplayTextDirection.LeftToRight,
                            draw.Paragraph.CanvasKitTextAlign switch
                            {
                                TextAlign.end => DisplayTextAlign.End,
                                TextAlign.left => DisplayTextAlign.Left,
                                TextAlign.right => DisplayTextAlign.Right,
                                TextAlign.center => DisplayTextAlign.Center,
                                TextAlign.justify => DisplayTextAlign.Justify,
                                _ => DisplayTextAlign.Start,
                            },
                            draw.Paragraph.CanvasKitLocale,
                            draw.Paragraph.CanvasKitMaxLines,
                            draw.Paragraph.CanvasKitEllipsis,
                            checked((float)draw.Paragraph.width),
                            checked((float)draw.Paragraph.longestLine),
                            checked((float)draw.Paragraph.height),
                            draw.Paragraph.CanvasKitMetricsHash,
                            [font],
                            draw.Paragraph.TextRuns.Select(ToParagraphTextRun)),
                        ToPoint(draw.Offset)));
                    break;
                case "drawImage" when command.HostPayload is CanvasImagePayload draw:
                    var image = resources.ResolveImage(draw.Image);
                    AddReference(image, referenced, resources);
                    destination.Add(new DisplayDrawImageCommand(
                        image,
                        new(checked((float)draw.Destination.left), checked((float)draw.Destination.top)),
                        ToSampling(draw.Paint.FilterQuality),
                        ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawImageRect" when command.HostPayload is CanvasImagePayload draw:
                    var rectImage = resources.ResolveImage(draw.Image);
                    AddReference(rectImage, referenced, resources);
                    destination.Add(new DisplayDrawImageRectCommand(
                        rectImage, ToRect(draw.Source), ToRect(draw.Destination),
                        ToSampling(draw.Paint.FilterQuality),
                        ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawImageNine" when command.HostPayload is CanvasImageNinePayload draw:
                    var nineImage = resources.ResolveImage(draw.Image);
                    AddReference(nineImage, referenced, resources);
                    destination.Add(new DisplayDrawNinePatchCommand(
                        nineImage, ToRect(draw.Center), ToRect(draw.Destination),
                        ToSampling(draw.Paint.FilterQuality),
                        ToPaint(draw.Paint, referenced, resources)));
                    break;
                case "drawShadow" when command.HostPayload is CanvasShadowPayload draw:
                    destination.Add(new DisplayDrawShadowCommand(
                        ToPath(draw.Path), draw.Color.value, checked((float)draw.Elevation),
                        draw.TransparentOccluder));
                    break;
                default:
                    throw new NotSupportedException(
                        $"Doroti canvas operation '{command.Operation}' has no DisplayList mapping.");
            }
        }
    }

    private static DisplayPaint ToPaint(
        PaintSnapshot paint,
        Dictionary<DisplayResourceReference, DisplayResourceDescriptor> referenced,
        IBrowserDisplayListResources resources) => new(
        paint.Color.value,
        (DisplayPaintStyle)paint.Style,
        checked((float)paint.StrokeWidth),
        StrokeCap: (DisplayStrokeCap)paint.StrokeCap,
        StrokeJoin: (DisplayStrokeJoin)paint.StrokeJoin,
        IsAntiAlias: paint.IsAntiAlias,
        BlendMode: ToBlendMode(paint.BlendMode),
        Sampling: ToSampling(paint.FilterQuality),
        InvertColors: paint.InvertColors,
        Shader: paint.Shader is null ? null : ToShader(paint.Shader, referenced, resources),
        ColorFilter: paint.ColorFilter is null ? null : ToColorFilter(paint.ColorFilter),
        MaskFilter: paint.MaskFilter is null ? null : new(
            (DisplayBlurStyle)paint.MaskFilter.style, checked((float)paint.MaskFilter.sigma)));

    private static DisplayShader ToShader(
        ShaderSnapshot shader,
        Dictionary<DisplayResourceReference, DisplayResourceDescriptor> referenced,
        IBrowserDisplayListResources resources) => shader switch
    {
        GradientShaderSnapshot gradient when gradient.Begin is { } start && gradient.End is { } end =>
            new DisplayLinearGradientShader(
                ToPoint(start), ToPoint(end), gradient.Colors.Select(color => color.value),
                gradient.Stops.Select(checkedValue => checked((float)checkedValue)),
                ToTileMode(gradient.TileMode),
                gradient.Matrix4 is null ? null : ToMatrix(gradient.Matrix4)),
        GradientShaderSnapshot gradient when gradient.Center is { } center &&
                                                   (gradient.Radius != 0 || gradient.Focal is not null) =>
            new DisplayRadialGradientShader(
                ToPoint(center), checked((float)gradient.Radius),
                gradient.Colors.Select(color => color.value),
                gradient.Stops.Select(checkedValue => checked((float)checkedValue)),
                ToTileMode(gradient.TileMode),
                gradient.Focal is { } focal ? ToPoint(focal) : null,
                checked((float)gradient.FocalRadius),
                gradient.Matrix4 is null ? null : ToMatrix(gradient.Matrix4)),
        GradientShaderSnapshot gradient when gradient.Center is { } center =>
            new DisplaySweepGradientShader(
                ToPoint(center), checked((float)gradient.StartAngle), checked((float)gradient.EndAngle),
                gradient.Colors.Select(color => color.value),
                gradient.Stops.Select(checkedValue => checked((float)checkedValue)),
                ToTileMode(gradient.TileMode),
                gradient.Matrix4 is null ? null : ToMatrix(gradient.Matrix4)),
        ImageShaderSnapshot image => ImageShader(image, referenced, resources),
        FragmentShaderSnapshot effect => RuntimeEffectShader(effect.State, referenced, resources),
        UnsupportedShaderSnapshot unsupported => throw new NotSupportedException(
            $"Shader family '{unsupported.Family}' has no DisplayList mapping."),
        _ => throw new NotSupportedException(
            $"Shader family '{shader.GetType().Name}' has no DisplayList mapping."),
    };

    private static DisplayImageShader ImageShader(
        ImageShaderSnapshot image,
        Dictionary<DisplayResourceReference, DisplayResourceDescriptor> referenced,
        IBrowserDisplayListResources resources)
    {
        var reference = resources.ResolveImage(image.Image);
        AddReference(reference, referenced, resources);
        return new(
            reference, ToTileMode(image.TileModeX), ToTileMode(image.TileModeY),
            ToSampling(image.FilterQuality ?? FilterQuality.none), ToMatrix(image.Matrix4));
    }

    private static DisplayRuntimeEffectShader RuntimeEffectShader(
        FragmentShaderState effect,
        Dictionary<DisplayResourceReference, DisplayResourceDescriptor> referenced,
        IBrowserDisplayListResources resources,
        int firstSamplerIndex = 0)
    {
        var reference = resources.ResolveRuntimeEffect(effect);
        AddReference(reference, referenced, resources);
        var samplers = effect.Samplers
            .Where(item => item.Key >= firstSamplerIndex)
            .OrderBy(item => item.Key)
            .ToArray();
        for (var index = 0; index < samplers.Length; index++)
        {
            var expected = checked((long)firstSamplerIndex + index);
            if (samplers[index].Key != expected)
                throw new InvalidDataException(
                    $"Runtime-effect sampler indices must be dense from {firstSamplerIndex}; " +
                    $"expected {expected}, received {samplers[index].Key}.");
        }
        var children = samplers.Select(item =>
        {
            var child = resources.ResolveImage(item.Value);
            AddReference(child, referenced, resources);
            return child;
        }).ToArray();
        var uniforms = new byte[checked(effect.Floats.Count * sizeof(float))];
        for (var index = 0; index < effect.Floats.Count; index++)
            BinaryPrimitives.WriteSingleLittleEndian(
                uniforms.AsSpan(index * sizeof(float), sizeof(float)),
                checked((float)effect.Floats[index]));
        return new(reference, uniforms, children);
    }

    private static DisplayColorFilter ToColorFilter(ColorFilterSnapshot filter) => filter.Kind switch
    {
        ColorFilterKind.mode when filter.Color is { } color =>
            new DisplayBlendColorFilter(color.value, ToBlendMode(filter.BlendMode)),
        ColorFilterKind.matrix when filter.Matrix is { } matrix =>
            new DisplayMatrixColorFilter(matrix.Select(value => checked((float)value))),
        ColorFilterKind.linearToSrgbGamma => new DisplayLinearToSrgbColorFilter(),
        ColorFilterKind.srgbToLinearGamma => new DisplaySrgbToLinearColorFilter(),
        _ => throw new InvalidDataException($"Invalid color filter '{filter.Kind}'."),
    };

    private static DisplayImageFilter ToImageFilter(
        ImageFilterSnapshot filter,
        Dictionary<DisplayResourceReference, DisplayResourceDescriptor> referenced,
        IBrowserDisplayListResources resources)
    {
        if (filter.Outer is { } composedOuter && filter.Inner is { } composedInner)
            return new DisplayComposeImageFilter(
                ToImageFilter(composedOuter, referenced, resources),
                ToImageFilter(composedInner, referenced, resources));

        DisplayImageFilter current;
        if (filter.Shader is FragmentShaderSnapshot effect)
            current = new DisplayRuntimeEffectImageFilter(
                RuntimeEffectShader(effect.State, referenced, resources, firstSamplerIndex: 1),
                ToSampling(filter.FilterQuality));
        else if (filter.Matrix4 is { } matrix)
            current = new DisplayMatrixImageFilter(ToMatrix(matrix), ToSampling(filter.FilterQuality));
        else if (filter.ColorFilter is { } color)
            current = new DisplayColorImageFilter(ToColorFilter(color));
        else
            current = new DisplayBlurImageFilter(
                checked((float)filter.SigmaX), checked((float)filter.SigmaY),
                ToTileMode(filter.TileMode), filter.Bounds is { } bounds ? ToRect(bounds) : null);

        if (filter.Inner is { } inner)
            current = new DisplayComposeImageFilter(current, ToImageFilter(inner, referenced, resources));
        if (filter.Outer is { } outer)
            current = new DisplayComposeImageFilter(ToImageFilter(outer, referenced, resources), current);
        return current;
    }

    private static DisplayPath ToPath(Doroti.Ui.Path path)
    {
        var verbs = new List<DisplayPathVerb>(path.Commands.Count);
        var values = new List<float>();
        foreach (var command in path.Commands)
        {
            verbs.Add(command.Operation switch
            {
                "moveTo" => DisplayPathVerb.MoveTo,
                "lineTo" => DisplayPathVerb.LineTo,
                "relativeMoveTo" => DisplayPathVerb.RelativeMoveTo,
                "relativeLineTo" => DisplayPathVerb.RelativeLineTo,
                "quadraticBezierTo" => DisplayPathVerb.QuadraticTo,
                "conicTo" => DisplayPathVerb.ConicTo,
                "cubicTo" => DisplayPathVerb.CubicTo,
                "addRect" => DisplayPathVerb.AddRect,
                "addOval" => DisplayPathVerb.AddOval,
                "addArc" => DisplayPathVerb.AddArc,
                "addRRect" => DisplayPathVerb.AddRoundedRect,
                "addRSuperellipse" => DisplayPathVerb.AddSuperellipse,
                "arcToPoint" => DisplayPathVerb.ArcToPoint,
                "arcTo" => DisplayPathVerb.ArcTo,
                "close" => DisplayPathVerb.Close,
                _ => throw new NotSupportedException(
                    $"Doroti path operation '{command.Operation}' has no DisplayList mapping."),
            });
            values.AddRange(command.Arguments.Select(value => checked((float)value)));
        }
        return new((DisplayPathFillType)path.fillType, verbs, values);
    }

    private static void AddReference(
        DisplayResourceReference reference,
        Dictionary<DisplayResourceReference, DisplayResourceDescriptor> referenced,
        IBrowserDisplayListResources resources) =>
        referenced.TryAdd(reference, resources.Describe(reference));

    private static DisplayMatrix Translation(double x, double y) => new(
        [
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            checked((float)x), checked((float)y), 0, 1,
        ]);

    private static DisplayMatrix Scale(double x, double y) => new(
        [
            checked((float)x), 0, 0, 0,
            0, checked((float)y), 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1,
        ]);

    private static DisplayMatrix Rotation(double radians)
    {
        var cosine = checked((float)Math.Cos(radians));
        var sine = checked((float)Math.Sin(radians));
        return new(
            [
                cosine, sine, 0, 0,
                -sine, cosine, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1,
            ]);
    }

    private static DisplayMatrix Skew(double x, double y) => new(
        [
            1, checked((float)y), 0, 0,
            checked((float)x), 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1,
        ]);

    private static DisplayMatrix ToMatrix(IReadOnlyList<double> values)
    {
        if (values.Count != 16) throw new InvalidDataException("Doroti matrix must contain 16 values.");
        return new(values.Select(value => checked((float)value)));
    }

    private static DisplayPoint ToPoint(Offset value) =>
        new(checked((float)value.dx), checked((float)value.dy));

    private static DisplayRect ToRect(Rect value) =>
        new(checked((float)value.left), checked((float)value.top),
            checked((float)value.right), checked((float)value.bottom));

    private static DisplayRoundedRect ToRoundedRect(RRect value) => new(
        ToRect(value.outerRect),
        checked((float)value.tlRadiusX), checked((float)value.tlRadiusY),
        checked((float)value.trRadiusX), checked((float)value.trRadiusY),
        checked((float)value.brRadiusX), checked((float)value.brRadiusY),
        checked((float)value.blRadiusX), checked((float)value.blRadiusY));

    private static DisplayRoundedRect ToRoundedRect(RSuperellipse value) => new(
        ToRect(value.outerRect),
        checked((float)value.tlRadiusX), checked((float)value.tlRadiusY),
        checked((float)value.trRadiusX), checked((float)value.trRadiusY),
        checked((float)value.brRadiusX), checked((float)value.brRadiusY),
        checked((float)value.blRadiusX), checked((float)value.blRadiusY));

    private static DisplayBlendMode ToBlendMode(BlendMode value) => (DisplayBlendMode)value;

    private static DisplaySamplingQuality ToSampling(FilterQuality value) =>
        (DisplaySamplingQuality)value;

    private static DisplayTileMode ToTileMode(TileMode value) => (DisplayTileMode)value;

    private static DisplayParagraphTextRun ToParagraphTextRun(ParagraphTextRun run)
    {
        ArgumentNullException.ThrowIfNull(run);
        ArgumentNullException.ThrowIfNull(run.Style);
        var style = run.Style;
        var foreground = style.foreground?.color ?? style.color ?? new Color(0xff000000L);
        return new DisplayParagraphTextRun(
            run.Text,
            style.fontFamily ?? "DorotiFallback",
            style.locale?.toLanguageTag() ?? string.Empty,
            checked((float)(style.fontSize ?? 14)),
            checked((float)(style.height ?? 1)),
            foreground.value,
            style.fontWeight?.value ?? 400,
            style.fontStyle == FontStyle.italic ? DisplayFontSlant.Italic : DisplayFontSlant.Normal,
            checked((uint)(style.decoration?.mask ?? 0)),
            style.background?.color.value,
            style.decorationColor?.value,
            style.decorationStyle switch
            {
                TextDecorationStyle.solid => DisplayTextDecorationStyle.Solid,
                TextDecorationStyle.doubleLine => DisplayTextDecorationStyle.Double,
                TextDecorationStyle.dotted => DisplayTextDecorationStyle.Dotted,
                TextDecorationStyle.dashed => DisplayTextDecorationStyle.Dashed,
                TextDecorationStyle.wavy => DisplayTextDecorationStyle.Wavy,
                _ => null,
            },
            style.decorationThickness is null ? null : checked((float)style.decorationThickness.Value),
            style.textBaseline switch
            {
                TextBaseline.alphabetic => DisplayTextBaseline.Alphabetic,
                TextBaseline.ideographic => DisplayTextBaseline.Ideographic,
                _ => null,
            },
            style.letterSpacing is null ? null : checked((float)style.letterSpacing.Value),
            style.wordSpacing is null ? null : checked((float)style.wordSpacing.Value),
            style.leadingDistribution switch
            {
                TextLeadingDistribution.proportional => false,
                TextLeadingDistribution.even => true,
                _ => null,
            },
            style.fontFamilyFallback,
            style.shadows?.Select(shadow => new DisplayTextShadow(
                shadow.color.value,
                checked((float)shadow.offset.dx),
                checked((float)shadow.offset.dy),
                checked((float)shadow.blurRadius))),
            style.fontFeatures?.Select(feature => new DisplayFontFeature(
                feature.feature, checked((int)feature.value))),
            style.fontVariations?.Select(variation => new DisplayFontVariation(
                variation.axis, checked((float)variation.value))));
    }

    private static float Value(PathCommand command, int index)
    {
        if ((uint)index >= (uint)command.Arguments.Count)
            throw new InvalidDataException(
                $"Doroti command '{command.Operation}' is missing argument {index}.");
        return checked((float)command.Arguments[index]);
    }

    private static ulong StableValueIdentity(object? value) => value switch
    {
        null => 0,
        byte number => number,
        ushort number => number,
        uint number => number,
        ulong number => number,
        sbyte number => unchecked((ulong)number),
        short number => unchecked((ulong)number),
        int number => unchecked((ulong)number),
        long number => unchecked((ulong)number),
        string text => StableStringHash(text),
        _ => throw new NotSupportedException(
            $"Backdrop identity '{value.GetType().FullName}' is not a renderer-neutral value."),
    };

    private static ulong StableStringHash(string value)
    {
        const ulong offset = 14695981039346656037;
        const ulong prime = 1099511628211;
        var result = offset;
        foreach (var character in value)
        {
            result ^= character;
            result *= prime;
        }
        return result;
    }

    private sealed class ViewScopedResources(
        ulong viewId,
        IBrowserDisplayListResources inner) : IBrowserDisplayListResources
    {
        public DisplayResourceReference DefaultFont => inner.DefaultFont;

        public DisplayResourceDescriptor Describe(DisplayResourceReference reference) =>
            inner.Describe(reference);

        public DisplayResourceReference ResolveImage(UiImage image)
        {
            ArgumentNullException.ThrowIfNull(image);
            if (image.viewId != viewId)
                throw new InvalidOperationException(
                    $"CanvasKit DisplayList view {viewId} cannot reference image view {image.viewId}.");
            return inner.ResolveImage(image);
        }

        public DisplayResourceReference ResolveRuntimeEffect(FragmentShaderState effect) =>
            inner.ResolveRuntimeEffect(effect);
    }
}
