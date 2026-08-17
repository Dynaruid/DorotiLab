using Doroti.Ui;
using Doroti.Skia.RuntimeEffects;
using SkiaSharp;
using System.Text.Json;
using UiColor = Doroti.Ui.Color;
using UiImage = Doroti.Ui.Image;
using UiPath = Doroti.Ui.Path;

namespace Doroti.Host.Web;

[System.Runtime.Versioning.SupportedOSPlatform("browser")]
internal sealed class BrowserSkiaCapabilities :
    ISceneHostCapability,
    IParagraphHostCapability,
    IImageHostCapability,
    ISemanticsHostCapability,
    IDisposable
{
    private readonly ulong _viewId;
    private readonly BrowserHostAdapter _host;
    private readonly object _gate = new();
    private readonly Dictionary<int, SemanticsNodeUpdate> _semantics = [];
    private SceneFrame? _pendingFrame;
    private SceneFrame? _presentedFrame;
    private Action? _invalidate;
    private long _submitted;
    private long _presented;
    private long _replayed;
    private long _failed;
    private long _contextGeneration;
    private bool _semanticsEnabled;
    private bool _disposed;

    internal BrowserSkiaCapabilities(ulong viewId, BrowserHostAdapter host)
    {
        _viewId = viewId;
        _host = host;
        _host.SemanticsAction += HandleSemanticsAction;
    }

    private Action<SemanticsActionEvent>? _action;
    public event Action<SemanticsActionEvent>? Action { add => _action += value; remove => _action -= value; }

    internal BrowserFrameDiagnostics Diagnostics
    {
        get
        {
            lock (_gate)
                return new(_submitted, _presented, _replayed, _failed, _contextGeneration,
                    _host.Snapshot.SurfaceGeneration, _pendingFrame is not null,
                    "skiasharp-skglview-webgl2-gpu");
        }
    }

    internal void AttachSurface(Action invalidate)
    {
        ArgumentNullException.ThrowIfNull(invalidate);
        ObjectDisposedException.ThrowIf(_disposed, this);
        bool hasFrame;
        lock (_gate)
        {
            _invalidate = invalidate;
            _contextGeneration++;
            hasFrame = _pendingFrame is not null || _presentedFrame is not null;
        }
        if (hasFrame) invalidate();
    }

    public void Submit(ulong viewId, Scene scene, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(scene);
        if (viewId != _viewId || scene.viewId != _viewId)
            throw new DorotiCapabilityException(DorotiCapabilityIds.GraphicsScene, viewId, invocation,
                "scene/view ownership mismatch", "browser-wasm/document-canvas-webgl2");
        Action? invalidate;
        lock (_gate)
        {
            _pendingFrame = new(scene.Commands);
            _submitted++;
            invalidate = _invalidate;
        }
        invalidate?.Invoke();
    }

    internal void Paint(SKSurface surface, int pixelWidth, int pixelHeight)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ObjectDisposedException.ThrowIf(_disposed, this);
        SceneFrame? frame;
        bool isNewFrame;
        lock (_gate)
        {
            frame = _pendingFrame;
            isNewFrame = frame is not null;
            if (isNewFrame) _pendingFrame = null;
            else frame = _presentedFrame;
        }
        if (frame is null) return;
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        try
        {
            // RenderView's root transform has already converted logical coordinates
            // into physical pixels. Applying the browser DPR here would scale twice.
            DrawScene(canvas, frame.Commands);
            canvas.Flush();
            lock (_gate)
            {
                if (isNewFrame)
                {
                    _presentedFrame = frame;
                    _presented++;
                }
                else
                {
                    _replayed++;
                }
            }
        }
        catch
        {
            lock (_gate)
            {
                if (isNewFrame && _pendingFrame is null) _pendingFrame = frame;
                _failed++;
            }
            throw;
        }
    }

    public Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        using var typeface = SKTypeface.FromFamilyName(request.FontFamily);
        using var font = new SKFont(typeface, (float)request.FontSize);
        var width = Math.Min(request.Width, font.MeasureText(request.Text));
        return new Paragraph(request.Text, width, request.FontSize * 1.2, request.FontSize, fontFamily: request.FontFamily);
    }

    public ValueTask<UiImage> DecodeAsync(
        ReadOnlyMemory<byte> bytes,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        cancellationToken.ThrowIfCancellationRequested();
        var data = SKData.CreateCopy(bytes.Span);
        var image = SKImage.FromEncodedData(data);
        data.Dispose();
        if (image is null) throw new InvalidDataException("SkiaSharp could not decode the browser image resource.");
        var handle = new BrowserImageHandle(image);
        return ValueTask.FromResult(new UiImage(_viewId, image.Width, image.Height, handle.Release) { HostHandle = handle });
    }

    public void SetEnabled(bool enabled, DartUiInvocation invocation)
    {
        _semanticsEnabled = enabled;
        if (!enabled) _semantics.Clear();
    }

    public void Update(SemanticsUpdate update, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (!_semanticsEnabled) return;
        foreach (var node in update.nodes) _semantics[node.id] = node;
        var nodes = _semantics.Values.OrderBy(node => node.indexInParent ?? int.MaxValue).ThenBy(node => node.id)
            .Select(node => new
            {
                node.id,
                node.label,
                node.value,
                role = node.role.ToString(),
                actions = (long)node.actions,
                children = node.children,
                flags = node.flags is null ? null : new
                {
                    @checked = node.flags.isChecked.ToString(),
                    selected = node.flags.isSelected.toBoolOrNull(),
                    enabled = node.flags.isEnabled.toBoolOrNull(),
                    toggled = node.flags.isToggled.toBoolOrNull(),
                    expanded = node.flags.isExpanded.toBoolOrNull(),
                    required = node.flags.isRequired.toBoolOrNull(),
                    focused = node.flags.isFocused.toBoolOrNull(),
                    button = node.flags.isButton,
                    textField = node.flags.isTextField,
                    header = node.flags.isHeader,
                    hidden = node.flags.isHidden,
                    image = node.flags.isImage,
                    liveRegion = node.flags.isLiveRegion,
                    multiline = node.flags.isMultiline,
                    readOnly = node.flags.isReadOnly,
                    link = node.flags.isLink,
                    slider = node.flags.isSlider,
                },
                node.textSelectionBase,
                node.textSelectionExtent,
                rect = new[] { node.rect.left, node.rect.top, node.rect.right, node.rect.bottom },
            });
        _host.UpdateSemantics(System.Text.Json.JsonSerializer.Serialize(new { generation = update.generation, nodes }));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _host.SemanticsAction -= HandleSemanticsAction;
        lock (_gate)
        {
            _pendingFrame = null;
            _presentedFrame = null;
            _invalidate = null;
        }
        _semantics.Clear();
    }

    private void HandleSemanticsAction(long nodeId, long action, string argumentsJson)
    {
        if (_disposed || nodeId is < int.MinValue or > int.MaxValue) return;
        _action?.Invoke(new(_viewId, checked((int)nodeId), (SemanticsAction)action, ParseArguments(argumentsJson)));
    }

    private static object? ParseArguments(string json)
    {
        if (string.IsNullOrWhiteSpace(json) || json == "null") return null;
        using var document = JsonDocument.Parse(json);
        return ConvertElement(document.RootElement);
    }

    private static object? ConvertElement(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => element.GetString(),
        JsonValueKind.Number when element.TryGetInt64(out var integer) => integer,
        JsonValueKind.Number => element.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Array => element.EnumerateArray().Select(ConvertElement).ToArray(),
        JsonValueKind.Object => element.EnumerateObject().ToDictionary(
            property => property.Name, property => ConvertElement(property.Value), StringComparer.Ordinal),
        _ => null,
    };

    private sealed record SceneFrame(IReadOnlyList<SceneCommand> Commands);

    private static void DrawScene(SKCanvas canvas, IReadOnlyList<SceneCommand> commands)
    {
        var restoreCounts = new Stack<int>();
        DrawCommands(commands);
        if (restoreCounts.Count != 0)
            throw new InvalidDataException($"Doroti browser scene has {restoreCounts.Count} unclosed scopes.");

        void DrawCommands(IReadOnlyList<SceneCommand> source)
        {
            foreach (var command in source)
            {
                switch (command.Operation)
                {
                    case "picture" when command.HostPayload is ScenePicturePayload picture:
                        canvas.Save();
                        canvas.Translate((float)picture.Offset.dx, (float)picture.Offset.dy);
                        DrawPicture(canvas, picture.Picture);
                        canvas.Restore();
                        break;
                    case "offset" when command.HostPayload is SceneOffsetPayload offset:
                        canvas.Save(); restoreCounts.Push(1); canvas.Translate((float)offset.Dx, (float)offset.Dy); break;
                    case "clipRect" when command.HostPayload is SceneClipRectPayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipRect(ToRect(clip.Rect), SKClipOperation.Intersect, true); break;
                    case "clipRRect" when command.HostPayload is SceneClipRRectPayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipPath(ToPath(clip.RRect), SKClipOperation.Intersect, true); break;
                    case "clipRSuperellipse" when command.HostPayload is SceneClipRSuperellipsePayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipRect(ToRect(clip.RSuperellipse.outerRect), SKClipOperation.Intersect, true); break;
                    case "clipPath" when command.HostPayload is SceneClipPathPayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipPath(ToPath(clip.Path), SKClipOperation.Intersect, true); break;
                    case "transform" when command.HostPayload is SceneTransformPayload transform:
                        canvas.Save(); restoreCounts.Push(1); Concat(canvas, transform.Matrix4); break;
                    case "opacity" when command.HostPayload is SceneOpacityPayload opacity:
                        using (var paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)Math.Clamp(Math.Round(opacity.Opacity * 255), 0, 255)) })
                            canvas.SaveLayer(paint);
                        restoreCounts.Push(1); canvas.Translate((float)opacity.Offset.dx, (float)opacity.Offset.dy); break;
                    case "colorFilter" when command.HostPayload is SceneColorFilterPayload:
                        canvas.SaveLayer(); restoreCounts.Push(1); break;
                    case "shaderMask" when command.HostPayload is SceneShaderMaskPayload mask:
                        using (var shader = ToShader(mask.Shader))
                        using (var paint = new SKPaint { Shader = shader, BlendMode = ToBlend(mask.BlendMode) })
                            canvas.SaveLayer(ToRect(mask.MaskRect), paint);
                        restoreCounts.Push(1); break;
                    case "imageFilter" when command.HostPayload is SceneImageFilterPayload image:
                        canvas.Save();
                        var imageRestoreCount = 1;
                        if (image.Filter.Matrix4 is not null && image.Filter.Outer is null &&
                            image.Filter.Inner is null && image.Filter.ColorFilter is null && image.Filter.Shader is null)
                        {
                            // The browser host also replays retained vector commands. Keep matrix-only image
                            // filters on that path instead of allocating an unbounded intermediate texture.
                            Concat(canvas, image.Filter.Matrix4);
                        }
                        else
                        {
                            using var paint = FilterPaint(image.Filter);
                            canvas.SaveLayer(paint);
                            imageRestoreCount++;
                        }
                        canvas.Translate((float)image.Offset.dx, (float)image.Offset.dy);
                        restoreCounts.Push(imageRestoreCount);
                        break;
                    case "backdropFilter" when command.HostPayload is SceneBackdropFilterPayload backdrop:
                        using (var filter = ToImageFilter(backdrop.Filter))
                        using (var paint = new SKPaint { BlendMode = ToBlend(backdrop.BlendMode) })
                        {
                            var restoreCount = 1;
                            if (backdrop.Filter.Bounds is { } clipBounds)
                            {
                                canvas.Save();
                                canvas.ClipRect(ToRect(clipBounds), SKClipOperation.Intersect, true);
                                restoreCount++;
                            }
                            var layer = new SKCanvasSaveLayerRec
                            {
                                Backdrop = filter,
                                Bounds = backdrop.Filter.Bounds is { } bounds ? ToRect(bounds) : null,
                                Paint = paint,
                            };
                            canvas.SaveLayer(layer);
                            restoreCounts.Push(restoreCount);
                        }
                        break;
                    case "retained" when command.HostPayload is SceneRetainedPayload retained:
                        DrawCommands(retained.Commands); break;
                    case "pop" when restoreCounts.Count > 0:
                        for (var count = restoreCounts.Pop(); count > 0; count--) canvas.Restore();
                        break;
                    default:
                        throw new NotSupportedException($"Doroti browser scene operation '{command.Operation}' has no Skia GPU mapping.");
                }
            }
        }
    }

    private static void DrawPicture(SKCanvas canvas, Picture picture)
    {
        ObjectDisposedException.ThrowIf(picture.debugDisposed, picture);
        foreach (var command in picture.Commands)
        {
            switch (command.Operation)
            {
                case "save": canvas.Save(); break;
                case "saveLayer" when command.HostPayload is CanvasSaveLayerPayload layer:
                    using (var paint = ToPaint(layer.Paint))
                    {
                        if (layer.Bounds is { } bounds) canvas.SaveLayer(ToRect(bounds), paint);
                        else canvas.SaveLayer(paint);
                    }
                    break;
                case "restore": canvas.Restore(); break;
                case "translate": canvas.Translate((float)command.Arguments[0], (float)command.Arguments[1]); break;
                case "scale": canvas.Scale((float)command.Arguments[0], (float)command.Arguments[1]); break;
                case "rotate": canvas.RotateRadians((float)command.Arguments[0]); break;
                case "transform": Concat(canvas, command.Arguments); break;
                case "clipRect": canvas.ClipRect(new((float)command.Arguments[0], (float)command.Arguments[1], (float)command.Arguments[2], (float)command.Arguments[3]), SKClipOperation.Intersect, true); break;
                case "clipRRect" when command.HostPayload is CanvasClipRRectPayload clip: canvas.ClipPath(ToPath(clip.RRect), SKClipOperation.Intersect, true); break;
                case "clipRSuperellipse" when command.HostPayload is CanvasClipRSuperellipsePayload clip: canvas.ClipRect(ToRect(clip.RSuperellipse.outerRect), SKClipOperation.Intersect, clip.DoAntiAlias); break;
                case "clipPath" when command.HostPayload is CanvasClipPathPayload clip: canvas.ClipPath(ToPath(clip.Path), SKClipOperation.Intersect, true); break;
                case "drawRect" when command.HostPayload is CanvasRectPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawRect(ToRect(draw.Rect), paint); break;
                case "drawRRect" when command.HostPayload is CanvasRRectPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawPath(ToPath(draw.RRect), paint); break;
                case "drawRSuperellipse" when command.HostPayload is CanvasRSuperellipsePayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawRect(ToRect(draw.RSuperellipse.outerRect), paint); break;
                case "drawPath" when command.HostPayload is CanvasPathPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawPath(ToPath(draw.Path), paint); break;
                case "drawPaint" when command.HostPayload is PaintSnapshot draw: using (var paint = ToPaint(draw)) canvas.DrawPaint(paint); break;
                case "drawCircle" when command.HostPayload is CanvasCirclePayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawCircle((float)draw.Center.dx, (float)draw.Center.dy, (float)draw.Radius, paint); break;
                case "drawOval" when command.HostPayload is CanvasOvalPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawOval(ToRect(draw.Rect), paint); break;
                case "drawLine" when command.HostPayload is CanvasLinePayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawLine((float)draw.Start.dx, (float)draw.Start.dy, (float)draw.End.dx, (float)draw.End.dy, paint); break;
                case "drawPoints" or "drawRawPoints" when command.HostPayload is CanvasPointsPayload draw:
                    using (var paint = ToPaint(draw.Paint))
                        canvas.DrawPoints(ToPointMode(draw.PointMode), draw.Points.Select(ToPoint).ToArray(), paint);
                    break;
                case "drawArc" when command.HostPayload is CanvasArcPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawArc(ToRect(draw.Rect), (float)(draw.StartAngle * 180 / Math.PI), (float)(draw.SweepAngle * 180 / Math.PI), draw.UseCenter, paint); break;
                case "drawColor" when command.HostPayload is CanvasColorPayload draw: canvas.DrawColor(ToColor(draw.Color), ToBlend(draw.BlendMode)); break;
                case "drawParagraph" when command.HostPayload is CanvasParagraphPayload draw:
                    using (var typeface = SKTypeface.FromFamilyName(draw.Paragraph.fontFamily))
                    using (var font = new SKFont(typeface, (float)draw.Paragraph.fontSize))
                    using (var paint = new SKPaint { Color = ToColor(draw.Paragraph.color), IsAntialias = true })
                        canvas.DrawText(draw.Paragraph.text, (float)draw.Offset.dx, (float)(draw.Offset.dy + draw.Paragraph.alphabeticBaseline), SKTextAlign.Left, font, paint);
                    break;
                case "drawImageRect" or "drawImage" when command.HostPayload is CanvasImagePayload draw && draw.Image.HostHandle is BrowserImageHandle handle:
                    using (var paint = ToPaint(draw.Paint)) canvas.DrawImage(handle.Image, ToRect(draw.Source), ToRect(draw.Destination), paint); break;
                case "drawShadow" when command.HostPayload is CanvasShadowPayload draw:
                    DrawShadow(canvas, draw);
                    break;
                default: throw new NotSupportedException($"Doroti browser canvas operation '{command.Operation}' has no Skia GPU mapping.");
            }
        }
    }

    private static SKPaint ToPaint(PaintSnapshot value)
    {
        var paint = new SKPaint
        {
            Color = ToColor(value.Color),
            Style = value.Style == PaintingStyle.stroke ? SKPaintStyle.Stroke : SKPaintStyle.Fill,
            StrokeWidth = (float)value.StrokeWidth,
            IsAntialias = value.IsAntiAlias,
            BlendMode = ToBlend(value.BlendMode),
            StrokeCap = value.StrokeCap switch { StrokeCap.round => SKStrokeCap.Round, StrokeCap.square => SKStrokeCap.Square, _ => SKStrokeCap.Butt },
            StrokeJoin = value.StrokeJoin switch { StrokeJoin.round => SKStrokeJoin.Round, StrokeJoin.bevel => SKStrokeJoin.Bevel, _ => SKStrokeJoin.Miter },
        };
        if (value.Shader is not null) paint.Shader = ToShader(value.Shader);
        return paint;
    }

    private static SKPaint FilterPaint(ImageFilterSnapshot filter) => new() { ImageFilter = ToImageFilter(filter) };

    private static SKImageFilter ToImageFilter(ImageFilterSnapshot filter)
    {
        if (filter.Shader is not null)
        {
            throw new NotSupportedException(
                "Doroti Web does not advertise ImageFilter.shader because SkiaSharp cannot bind the filtered child as its implicit texture input.");
        }
        if (filter.Outer is not null && filter.Inner is not null)
        {
            using var outer = ToImageFilter(filter.Outer);
            using var inner = ToImageFilter(filter.Inner);
            return SKImageFilter.CreateCompose(outer, inner);
        }
        if (filter.ColorFilter is not null)
        {
            using var color = ToColorFilter(filter.ColorFilter);
            if (filter.Inner is null) return SKImageFilter.CreateColorFilter(color);
            using var inner = ToImageFilter(filter.Inner);
            return SKImageFilter.CreateColorFilter(color, inner);
        }
        if (filter.Matrix4 is not null)
            return SKImageFilter.CreateMatrix(ToMatrix(filter.Matrix4), ToSamplingOptions(filter.FilterQuality), null);
        return SKImageFilter.CreateBlur(
            (float)filter.SigmaX,
            (float)filter.SigmaY,
            filter.TileMode switch
            {
                TileMode.repeated => SKShaderTileMode.Repeat,
                TileMode.mirror => SKShaderTileMode.Mirror,
                TileMode.decal => SKShaderTileMode.Decal,
                _ => SKShaderTileMode.Clamp,
            });
    }

    private static SKShader ToShader(ShaderSnapshot value) => value switch
    {
        GradientShaderSnapshot gradient => ToGradientShader(gradient),
        ImageShaderSnapshot image => ToImageShader(image),
        FragmentShaderSnapshot fragment => DorotiSkiaRuntimeEffects.CreateShader(fragment, CreateImageShader),
        UnsupportedShaderSnapshot unsupported => throw new NotSupportedException(
            $"The Doroti browser backend rejects shader family '{unsupported.Family}'."),
        _ => throw new NotSupportedException($"The Doroti browser backend rejects shader snapshot '{value.GetType().Name}'."),
    };

    private static SKShader ToGradientShader(GradientShaderSnapshot value)
    {
        var colors = value.Colors.Select(ToColor).ToArray();
        var stops = value.Stops.Select(stop => (float)stop).ToArray();
        var tile = ToTileMode(value.TileMode);
        var matrix = value.Matrix4 is null ? SKMatrix.Identity : ToMatrix(value.Matrix4);
        if (value.Begin is { } begin && value.End is { } end)
            return SKShader.CreateLinearGradient(new((float)begin.dx, (float)begin.dy),
                new((float)end.dx, (float)end.dy), colors, stops, tile, matrix);
        if (value.Center is { } center && value.Radius > 0)
            return SKShader.CreateRadialGradient(new((float)center.dx, (float)center.dy),
                (float)value.Radius, colors, stops, tile, matrix);
        if (value.Center is { } sweepCenter)
            return SKShader.CreateSweepGradient(new((float)sweepCenter.dx, (float)sweepCenter.dy),
                colors, stops, tile, (float)(value.StartAngle * 180 / Math.PI),
                (float)(value.EndAngle * 180 / Math.PI), matrix);
        throw new InvalidDataException("Doroti gradient shader has no supported geometry.");
    }

    private static SKShader ToImageShader(ImageShaderSnapshot value)
    {
        if (value.Image.HostHandle is not BrowserImageHandle handle)
            throw new InvalidDataException("Doroti browser image shader has no native image handle.");
        return handle.Image.ToShader(ToTileMode(value.TileModeX), ToTileMode(value.TileModeY),
            ToSamplingOptions(value.FilterQuality ?? FilterQuality.none), ToMatrix(value.Matrix4));
    }

    private static SKShader CreateImageShader(Doroti.Ui.Image image)
    {
        if (image.HostHandle is not BrowserImageHandle handle)
            throw new InvalidDataException("Doroti browser fragment shader sampler has no native image handle.");
        return handle.Image.ToShader(SKShaderTileMode.Clamp, SKShaderTileMode.Clamp, SKSamplingOptions.Default);
    }

    private static SKColorFilter ToColorFilter(ColorFilterSnapshot value) => value.Kind switch
    {
        ColorFilterKind.mode => SKColorFilter.CreateBlendMode(
            value.Color is null ? throw new InvalidDataException("Mode color filter has no color.") : ToColor(value.Color),
            ToBlend(value.BlendMode)),
        ColorFilterKind.matrix => SKColorFilter.CreateColorMatrix(
            value.Matrix?.Select(item => (float)item).ToArray()
            ?? throw new InvalidDataException("Matrix color filter has no matrix.")),
        ColorFilterKind.linearToSrgbGamma => SKColorFilter.CreateLinearToSrgbGamma(),
        ColorFilterKind.srgbToLinearGamma => SKColorFilter.CreateSrgbToLinearGamma(),
        _ => throw new NotSupportedException($"Unsupported Doroti color filter '{value.Kind}'."),
    };

    private static SKShaderTileMode ToTileMode(TileMode value) => value switch
    {
        TileMode.repeated => SKShaderTileMode.Repeat,
        TileMode.mirror => SKShaderTileMode.Mirror,
        TileMode.decal => SKShaderTileMode.Decal,
        _ => SKShaderTileMode.Clamp,
    };

    private static SKPointMode ToPointMode(PointMode value) => value switch
    {
        PointMode.lines => SKPointMode.Lines,
        PointMode.polygon => SKPointMode.Polygon,
        _ => SKPointMode.Points,
    };

    private static SKPoint ToPoint(Offset value) => new((float)value.dx, (float)value.dy);

    private static SKSamplingOptions ToSamplingOptions(FilterQuality value) => value switch
    {
        FilterQuality.low => new SKSamplingOptions(SKFilterMode.Linear),
        FilterQuality.medium => new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.Linear),
        FilterQuality.high => new SKSamplingOptions(SKCubicResampler.Mitchell),
        _ => new SKSamplingOptions(SKFilterMode.Nearest),
    };

    private static void DrawShadow(SKCanvas canvas, CanvasShadowPayload shadow)
    {
        using var path = ToPath(shadow.Path);
        var elevation = Math.Max(0, shadow.Elevation);
        DrawPass(elevation * .2, .18, .24, Math.Max(.75, elevation * .45));
        DrawPass(elevation * .55, .24, .32, Math.Max(1, elevation * .8));

        void DrawPass(double offsetY, double transparentOpacity, double opaqueOpacity, double sigma)
        {
            var opacity = shadow.TransparentOccluder ? transparentOpacity : opaqueOpacity;
            using var paint = new SKPaint
            {
                Color = ToColor(shadow.Color).WithAlpha((byte)Math.Clamp(Math.Round(shadow.Color.alpha * opacity), 0, 255)),
                ImageFilter = SKImageFilter.CreateBlur((float)sigma, (float)sigma),
                IsAntialias = true,
            };
            canvas.Save();
            canvas.Translate(0, (float)offsetY);
            canvas.DrawPath(path, paint);
            canvas.Restore();
        }
    }

    private static SKPath ToPath(UiPath path)
    {
        var result = new SKPath { FillType = path.fillType == PathFillType.evenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding };
        foreach (var command in path.Commands)
        {
            var a = command.Arguments;
            switch (command.Operation)
            {
                case "moveTo": result.MoveTo((float)a[0], (float)a[1]); break;
                case "lineTo": result.LineTo((float)a[0], (float)a[1]); break;
                case "quadraticBezierTo": result.QuadTo((float)a[0], (float)a[1], (float)a[2], (float)a[3]); break;
                case "cubicTo": result.CubicTo((float)a[0], (float)a[1], (float)a[2], (float)a[3], (float)a[4], (float)a[5]); break;
                case "addRect": result.AddRect(new((float)a[0], (float)a[1], (float)a[2], (float)a[3])); break;
                case "addOval": result.AddOval(new((float)a[0], (float)a[1], (float)a[2], (float)a[3])); break;
                case "addRRect": result.AddRoundRect(new((float)a[0], (float)a[1], (float)a[2], (float)a[3]), (float)a[4], (float)a[5]); break;
                case "close": result.Close(); break;
            }
        }
        return result;
    }

    private static SKPath ToPath(RRect value)
    {
        var path = new SKPath();
        path.AddRoundRect(ToRect(value.outerRect), (float)value.tlRadiusX, (float)value.tlRadiusY);
        return path;
    }

    private static SKRect ToRect(Rect value) => new((float)value.left, (float)value.top, (float)value.right, (float)value.bottom);
    private static SKColor ToColor(UiColor value) => new((byte)value.red, (byte)value.green, (byte)value.blue, (byte)value.alpha);
    private static SKBlendMode ToBlend(BlendMode value) => Enum.TryParse<SKBlendMode>(value.ToString(), true, out var result) ? result : SKBlendMode.SrcOver;

    private static void Concat(SKCanvas canvas, IReadOnlyList<double> matrix)
    {
        canvas.Concat(ToMatrix(matrix));
    }

    private static SKMatrix ToMatrix(IReadOnlyList<double> matrix)
    {
        if (matrix.Count < 16) throw new InvalidDataException("A Doroti transform must contain 16 values.");
        return new SKMatrix
        {
            ScaleX = (float)matrix[0],
            SkewX = (float)matrix[4],
            TransX = (float)matrix[12],
            SkewY = (float)matrix[1],
            ScaleY = (float)matrix[5],
            TransY = (float)matrix[13],
            Persp0 = (float)matrix[3],
            Persp1 = (float)matrix[7],
            Persp2 = (float)matrix[15],
        };
    }

    internal sealed class BrowserImageHandle : IDorotiImageHandle
    {
        private readonly SharedImage _shared;
        internal BrowserImageHandle(SKImage image) => _shared = new(image);
        private BrowserImageHandle(SharedImage shared) { _shared = shared; Interlocked.Increment(ref shared.References); }
        internal SKImage Image => _shared.Image;
        public IDorotiImageHandle Clone() => new BrowserImageHandle(_shared);
        public void Release() { if (Interlocked.Decrement(ref _shared.References) == 0) _shared.Image.Dispose(); }
        private sealed class SharedImage(SKImage image) { internal readonly SKImage Image = image; internal int References = 1; }
    }
}
