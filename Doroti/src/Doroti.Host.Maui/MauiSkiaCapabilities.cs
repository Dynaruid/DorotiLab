using Doroti.Ui;
using Doroti.Skia.RuntimeEffects;
using SkiaSharp;
using BlendMode = Doroti.Ui.BlendMode;
using Rect = Doroti.Ui.Rect;
using UiColor = Doroti.Ui.Color;
using UiImage = Doroti.Ui.Image;
using UiPath = Doroti.Ui.Path;

namespace Doroti.Host.Maui;

internal sealed class MauiSkiaCapabilities :
    ISceneHostCapability,
    IParagraphHostCapability,
    IImageHostCapability,
    ISemanticsHostCapability,
    IDisposable
{
    private readonly ulong _viewId;
    private readonly MauiHostAdapter _host;
    private readonly SKColor _backgroundColor;
    private readonly object _gate = new();
    private readonly object _paintGate = new();
    private readonly Dictionary<TextRenderKey, TextRenderResources> _textRenderResources = [];
    private readonly Dictionary<int, SemanticsNodeUpdate> _semantics = [];
    private SceneFrame? _pendingFrame;
    private SceneFrame? _presentedFrame;
    private Action? _invalidate;
    private long _submitted;
    private long _presented;
    private long _replayed;
    private long _failed;
    private long _contextGeneration;
    private long _shaderImageFiltersRendered;
    private bool _semanticsEnabled;
    private bool _disposed;

    internal MauiSkiaCapabilities(ulong viewId, MauiHostAdapter host, UiColor? backgroundColor)
    {
        _viewId = viewId;
        _host = host;
        backgroundColor ??= new UiColor(0xfffffbfeL);
        _backgroundColor = new SKColor(
            checked((byte)backgroundColor.red), checked((byte)backgroundColor.green),
            checked((byte)backgroundColor.blue), checked((byte)backgroundColor.alpha));
        _host.SemanticsAction += HandleSemanticsAction;
    }

    private Action<SemanticsActionEvent>? _action;
    public event Action<SemanticsActionEvent>? Action { add => _action += value; remove => _action -= value; }

    internal MauiFrameDiagnostics Diagnostics
    {
        get
        {
            lock (_gate)
                return new(_submitted, _presented, _replayed, _failed, _contextGeneration,
                    _host.Snapshot.SurfaceGeneration, _pendingFrame is not null,
                    Volatile.Read(ref _shaderImageFiltersRendered),
                    "skiasharp-maui-skglview-gpu");
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
        lock (_paintGate) PaintCore(surface, pixelWidth, pixelHeight);
    }

    private void PaintCore(SKSurface surface, int pixelWidth, int pixelHeight)
    {
        SceneFrame? frame;
        bool isNewFrame;
        lock (_gate)
        {
            frame = _pendingFrame;
            isNewFrame = frame is not null;
            if (isNewFrame) _pendingFrame = null;
            else frame = _presentedFrame;
        }
        var canvas = surface.Canvas;
        // The native GL surface exists before the first framework scene. Clear
        // every fresh back buffer to the app-owned opaque color so neither that
        // startup gap nor an uncovered scene region exposes Android's black
        // TextureView/window background.
        canvas.Clear(_backgroundColor);
        if (frame is null)
        {
            canvas.Flush();
            return;
        }
        try
        {
            // SKSwapChainPanel and TextureView can rotate to a fresh back buffer.
            // Replay the last successful framework scene when no replacement is pending.
            // RenderView's root transform has already converted logical coordinates
            // into physical pixels. Applying the browser DPR here would scale twice.
            DrawScene(canvas, frame.Commands, pixelWidth, pixelHeight);
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
        var handle = new MauiImageHandle(image);
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
        PruneUnreachableSemantics(_semantics);
        var nodes = _semantics.Values
            .OrderBy(node => node.indexInParent ?? int.MaxValue)
            .ThenBy(node => node.id)
            .ToArray();
        _host.UpdateSemantics(new SemanticsUpdate(update.generation, nodes));
    }

    private static void PruneUnreachableSemantics(Dictionary<int, SemanticsNodeUpdate> nodes)
    {
        const int rootNodeId = 0;
        if (!nodes.ContainsKey(rootNodeId)) return;
        var reachable = new HashSet<int>();
        var pending = new Stack<int>();
        pending.Push(rootNodeId);
        while (pending.TryPop(out var nodeId))
        {
            if (!reachable.Add(nodeId) || !nodes.TryGetValue(nodeId, out var node)) continue;
            foreach (var childId in node.children) pending.Push(childId);
        }
        foreach (var staleId in nodes.Keys.Where(id => !reachable.Contains(id)).ToArray())
            nodes.Remove(staleId);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _host.SemanticsAction -= HandleSemanticsAction;
        lock (_paintGate)
        {
            lock (_gate)
            {
                _pendingFrame = null;
                _presentedFrame = null;
                _invalidate = null;
            }
            foreach (var resources in _textRenderResources.Values) resources.Dispose();
            _textRenderResources.Clear();
        }
        _semantics.Clear();
    }

    private void HandleSemanticsAction(int nodeId, SemanticsAction action, object? arguments)
    {
        if (!_disposed) _action?.Invoke(new(_viewId, nodeId, action, arguments));
    }

    private sealed record SceneFrame(IReadOnlyList<SceneCommand> Commands);

    private void DrawScene(
        SKCanvas canvas,
        IReadOnlyList<SceneCommand> commands,
        int pixelWidth,
        int pixelHeight) =>
        DrawScene(canvas, commands, 0, commands.Count, pixelWidth, pixelHeight);

    private void DrawScene(
        SKCanvas canvas,
        IReadOnlyList<SceneCommand> commands,
        int start,
        int end,
        int pixelWidth,
        int pixelHeight)
    {
        var restoreCounts = new Stack<int>();
        DrawCommands(commands, start, end);
        if (restoreCounts.Count != 0)
            throw new InvalidDataException($"Doroti MAUI scene has {restoreCounts.Count} unclosed scopes.");

        void DrawCommands(IReadOnlyList<SceneCommand> source, int sourceStart = 0, int sourceEnd = -1)
        {
            if (sourceEnd < 0) sourceEnd = source.Count;
            for (var commandIndex = sourceStart; commandIndex < sourceEnd; commandIndex++)
            {
                var command = source[commandIndex];
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
                        canvas.Save();
                        restoreCounts.Push(1);
                        canvas.ClipRect(ToRect(clip.Rect), SKClipOperation.Intersect, true);
                        break;
                    case "clipRRect" when command.HostPayload is SceneClipRRectPayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipPath(ToPath(clip.RRect), SKClipOperation.Intersect, true); break;
                    case "clipRSuperellipse" when command.HostPayload is SceneClipRSuperellipsePayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipRect(ToRect(clip.RSuperellipse.outerRect), SKClipOperation.Intersect, true); break;
                    case "clipPath" when command.HostPayload is SceneClipPathPayload clip:
                        canvas.Save(); restoreCounts.Push(1); canvas.ClipPath(ToPath(clip.Path), SKClipOperation.Intersect, true); break;
                    case "transform" when command.HostPayload is SceneTransformPayload transform:
                        canvas.Save();
                        restoreCounts.Push(1);
                        Concat(canvas, transform.Matrix4);
                        break;
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
                    case "imageFilter" when command.HostPayload is SceneImageFilterPayload image &&
                                                  image.Filter.Shader is FragmentShaderSnapshot fragment:
                        {
                            var matchingPop = FindMatchingPop(source, commandIndex, sourceEnd);
                            var offset = new SKPoint((float)image.Offset.dx, (float)image.Offset.dy);
                            var bounds = image.Bounds is { } explicitBounds
                                ? ToRect(explicitBounds)
                                : new SKRect(
                                    canvas.LocalClipBounds.Left - offset.X,
                                    canvas.LocalClipBounds.Top - offset.Y,
                                    canvas.LocalClipBounds.Right - offset.X,
                                    canvas.LocalClipBounds.Bottom - offset.Y);
                            if (DorotiSkiaImageFilterRenderer.Draw(
                                canvas,
                                pixelWidth,
                                pixelHeight,
                                fragment,
                                bounds,
                                offset,
                                ToSamplingOptions(image.Filter.FilterQuality),
                                CreateImageShader,
                                (inputCanvas, inputWidth, inputHeight) =>
                                    DrawScene(inputCanvas, source, commandIndex + 1, matchingPop, inputWidth, inputHeight)))
                                Interlocked.Increment(ref _shaderImageFiltersRendered);
                            commandIndex = matchingPop;
                            break;
                        }
                    case "imageFilter" when command.HostPayload is SceneImageFilterPayload image:
                        canvas.Save();
                        var imageRestoreCount = 1;
                        if (image.Filter.Matrix4 is not null && image.Filter.Outer is null &&
                            image.Filter.Inner is null && image.Filter.ColorFilter is null && image.Filter.Shader is null)
                        {
                            // Doroti retains vector scene commands, so replay a pure matrix image filter as
                            // an equivalent scene transform. An unbounded GPU SaveLayer + matrix filter can
                            // produce an empty texture while Android is animating stretch overscroll.
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
                        throw new NotSupportedException($"Doroti MAUI scene operation '{command.Operation}' has no Skia GPU mapping.");
                }
            }
        }
    }

    private static int FindMatchingPop(IReadOnlyList<SceneCommand> source, int scopeStart, int end)
    {
        var depth = 1;
        for (var index = scopeStart + 1; index < end; index++)
        {
            if (IsSceneScopeStart(source[index].Operation))
            {
                depth++;
            }
            else if (source[index].Operation == "pop" && --depth == 0)
            {
                return index;
            }
        }
        throw new InvalidDataException(
            $"Doroti MAUI scene image-filter scope at command {scopeStart} has no matching pop.");
    }

    private static bool IsSceneScopeStart(string operation) => operation is
        "offset" or "clipRect" or "clipRRect" or "clipRSuperellipse" or "clipPath" or
        "transform" or "opacity" or "colorFilter" or "shaderMask" or "imageFilter" or
        "backdropFilter";

    private void DrawPicture(SKCanvas canvas, Picture picture)
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
                    var textResources = GetTextRenderResources(
                        draw.Paragraph.fontFamily,
                        (float)draw.Paragraph.fontSize,
                        ToColor(draw.Paragraph.color));
                    canvas.DrawText(draw.Paragraph.text, (float)draw.Offset.dx,
                        (float)(draw.Offset.dy + draw.Paragraph.alphabeticBaseline),
                        SKTextAlign.Left, textResources.Font, textResources.Paint);
                    break;
                case "drawImageRect" or "drawImage" when command.HostPayload is CanvasImagePayload draw && draw.Image.HostHandle is MauiImageHandle handle:
                    using (var paint = ToPaint(draw.Paint))
                        canvas.DrawImage(handle.Image, ToRect(draw.Source), ToRect(draw.Destination),
                            ToSamplingOptions(draw.Paint.FilterQuality), paint);
                    break;
                case "drawShadow" when command.HostPayload is CanvasShadowPayload draw:
                    DrawShadow(canvas, draw);
                    break;
                default: throw new NotSupportedException($"Doroti MAUI canvas operation '{command.Operation}' has no Skia GPU mapping.");
            }
        }
    }

    private TextRenderResources GetTextRenderResources(string? fontFamily, float fontSize, SKColor color)
    {
        var key = new TextRenderKey(fontFamily ?? string.Empty, fontSize, color);
        if (_textRenderResources.TryGetValue(key, out var resources)) return resources;
        resources = new TextRenderResources(fontFamily, fontSize, color);
        _textRenderResources.Add(key, resources);
        return resources;
    }

    private readonly record struct TextRenderKey(string FontFamily, float FontSize, SKColor Color);

    private sealed class TextRenderResources : IDisposable
    {
        private readonly SKTypeface _typeface;

        internal TextRenderResources(string? fontFamily, float fontSize, SKColor color)
        {
            _typeface = SKTypeface.FromFamilyName(fontFamily);
            Font = new SKFont(_typeface, fontSize);
            Paint = new SKPaint { Color = color, IsAntialias = true };
        }

        internal SKFont Font { get; }
        internal SKPaint Paint { get; }

        public void Dispose()
        {
            Paint.Dispose();
            Font.Dispose();
            _typeface.Dispose();
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
            throw new InvalidOperationException(
                "Shader image filters must be rendered through Doroti's GPU offscreen input path.");
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
            $"The Doroti MAUI backend rejects shader family '{unsupported.Family}'."),
        _ => throw new NotSupportedException($"The Doroti MAUI backend rejects shader snapshot '{value.GetType().Name}'."),
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
        if (value.Image.HostHandle is not MauiImageHandle handle)
            throw new InvalidDataException("Doroti MAUI image shader has no native image handle.");
        return handle.Image.ToShader(ToTileMode(value.TileModeX), ToTileMode(value.TileModeY),
            ToSamplingOptions(value.FilterQuality ?? FilterQuality.none), ToMatrix(value.Matrix4));
    }

    private static SKShader CreateImageShader(Doroti.Ui.Image image)
    {
        if (image.HostHandle is not MauiImageHandle handle)
            throw new InvalidDataException("Doroti MAUI fragment shader sampler has no native image handle.");
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
        using var builder = new SKPathBuilder
        {
            FillType = path.fillType == PathFillType.evenOdd ? SKPathFillType.EvenOdd : SKPathFillType.Winding,
        };
        foreach (var command in path.Commands)
        {
            var a = command.Arguments;
            switch (command.Operation)
            {
                case "moveTo": builder.MoveTo((float)a[0], (float)a[1]); break;
                case "lineTo": builder.LineTo((float)a[0], (float)a[1]); break;
                case "quadraticBezierTo": builder.QuadTo((float)a[0], (float)a[1], (float)a[2], (float)a[3]); break;
                case "cubicTo": builder.CubicTo((float)a[0], (float)a[1], (float)a[2], (float)a[3], (float)a[4], (float)a[5]); break;
                case "addRect": builder.AddRect(new((float)a[0], (float)a[1], (float)a[2], (float)a[3]), SKPathDirection.Clockwise); break;
                case "addOval": builder.AddOval(new((float)a[0], (float)a[1], (float)a[2], (float)a[3]), SKPathDirection.Clockwise); break;
                case "addRRect": builder.AddRoundRect(new((float)a[0], (float)a[1], (float)a[2], (float)a[3]), (float)a[4], (float)a[5], SKPathDirection.Clockwise); break;
                case "close": builder.Close(); break;
            }
        }
        return builder.Detach();
    }

    private static SKPath ToPath(RRect value)
    {
        using var builder = new SKPathBuilder();
        builder.AddRoundRect(ToRect(value.outerRect), (float)value.tlRadiusX, (float)value.tlRadiusY,
            SKPathDirection.Clockwise);
        return builder.Detach();
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

    internal sealed class MauiImageHandle : IDorotiImageHandle
    {
        private readonly SharedImage _shared;
        internal MauiImageHandle(SKImage image) => _shared = new(image);
        private MauiImageHandle(SharedImage shared) { _shared = shared; Interlocked.Increment(ref shared.References); }
        internal SKImage Image => _shared.Image;
        public IDorotiImageHandle Clone() => new MauiImageHandle(_shared);
        public void Release() { if (Interlocked.Decrement(ref _shared.References) == 0) _shared.Image.Dispose(); }
        private sealed class SharedImage(SKImage image) { internal readonly SKImage Image = image; internal int References = 1; }
    }
}
