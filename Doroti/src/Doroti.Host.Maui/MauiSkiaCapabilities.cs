using Doroti.Ui;
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
    private readonly object _gate = new();
    private readonly Dictionary<int, SemanticsNodeUpdate> _semantics = [];
    private Scene? _pendingScene;
    private Action? _invalidate;
    private long _submitted;
    private long _presented;
    private long _failed;
    private long _contextGeneration;
    private bool _semanticsEnabled;
    private bool _disposed;

    internal MauiSkiaCapabilities(ulong viewId, MauiHostAdapter host)
    {
        _viewId = viewId;
        _host = host;
    }

    private Action<SemanticsActionEvent>? _action;
    public event Action<SemanticsActionEvent>? Action { add => _action += value; remove => _action -= value; }

    internal MauiFrameDiagnostics Diagnostics
    {
        get
        {
            lock (_gate)
                return new(_submitted, _presented, _failed, _contextGeneration,
                    _host.Snapshot.SurfaceGeneration, _pendingScene is not null,
                    "skiasharp-maui-skglview-gpu");
        }
    }

    internal void AttachSurface(Action invalidate)
    {
        ArgumentNullException.ThrowIfNull(invalidate);
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_gate)
        {
            _invalidate = invalidate;
            _contextGeneration++;
            if (_pendingScene is not null) invalidate();
        }
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
            _pendingScene = scene;
            _submitted++;
            invalidate = _invalidate;
        }
        invalidate?.Invoke();
    }

    internal void Paint(SKSurface surface, int pixelWidth, int pixelHeight)
    {
        ArgumentNullException.ThrowIfNull(surface);
        ObjectDisposedException.ThrowIf(_disposed, this);
        Scene? scene;
        lock (_gate)
        {
            scene = _pendingScene;
            _pendingScene = null;
        }
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        if (scene is null) return;
        try
        {
            // RenderView's root transform has already converted logical coordinates
            // into physical pixels. Applying the browser DPR here would scale twice.
            DrawScene(canvas, scene.Commands);
            canvas.Flush();
            lock (_gate) _presented++;
        }
        catch
        {
            lock (_gate) _failed++;
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
        var nodes = _semantics.Values.OrderBy(node => node.indexInParent ?? int.MaxValue).ThenBy(node => node.id)
            .Select(node => new
            {
                node.id,
                node.label,
                node.value,
                role = node.role.ToString(),
                actions = node.actions.ToString(),
                rect = new[] { node.rect.left, node.rect.top, node.rect.right, node.rect.bottom },
            });
        _host.UpdateSemantics(System.Text.Json.JsonSerializer.Serialize(new { generation = update.generation, nodes }));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        lock (_gate)
        {
            _pendingScene = null;
            _invalidate = null;
        }
        _semantics.Clear();
    }

    private static void DrawScene(SKCanvas canvas, IReadOnlyList<SceneCommand> commands)
    {
        var restoreCounts = new Stack<int>();
        DrawCommands(commands);
        if (restoreCounts.Count != 0)
            throw new InvalidDataException($"Doroti MAUI scene has {restoreCounts.Count} unclosed scopes.");

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
                    case "imageFilter" when command.HostPayload is SceneImageFilterPayload image:
                        using (var paint = FilterPaint(image.Filter)) canvas.SaveLayer(paint);
                        restoreCounts.Push(1); canvas.Translate((float)image.Offset.dx, (float)image.Offset.dy); break;
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
                case "drawArc" when command.HostPayload is CanvasArcPayload draw: using (var paint = ToPaint(draw.Paint)) canvas.DrawArc(ToRect(draw.Rect), (float)(draw.StartAngle * 180 / Math.PI), (float)(draw.SweepAngle * 180 / Math.PI), draw.UseCenter, paint); break;
                case "drawColor" when command.HostPayload is CanvasColorPayload draw: canvas.DrawColor(ToColor(draw.Color), ToBlend(draw.BlendMode)); break;
                case "drawParagraph" when command.HostPayload is CanvasParagraphPayload draw:
                    using (var typeface = SKTypeface.FromFamilyName(draw.Paragraph.fontFamily))
                    using (var font = new SKFont(typeface, (float)draw.Paragraph.fontSize))
                    using (var paint = new SKPaint { Color = ToColor(draw.Paragraph.color), IsAntialias = true })
                        canvas.DrawText(draw.Paragraph.text, (float)draw.Offset.dx, (float)(draw.Offset.dy + draw.Paragraph.alphabeticBaseline), SKTextAlign.Left, font, paint);
                    break;
                case "drawImageRect" or "drawImage" when command.HostPayload is CanvasImagePayload draw && draw.Image.HostHandle is MauiImageHandle handle:
                    using (var paint = ToPaint(draw.Paint)) canvas.DrawImage(handle.Image, ToRect(draw.Source), ToRect(draw.Destination), paint); break;
                case "drawShadow" when command.HostPayload is CanvasShadowPayload draw:
                    DrawShadow(canvas, draw);
                    break;
                default: throw new NotSupportedException($"Doroti MAUI canvas operation '{command.Operation}' has no Skia GPU mapping.");
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
        if (value.Shader is GradientShaderSnapshot gradient && gradient.Begin is { } begin && gradient.End is { } end)
            paint.Shader = SKShader.CreateLinearGradient(new((float)begin.dx, (float)begin.dy), new((float)end.dx, (float)end.dy), gradient.Colors.Select(ToColor).ToArray(), gradient.Stops.Select(stop => (float)stop).ToArray(), SKShaderTileMode.Clamp);
        else if (value.Shader is not null)
            throw new NotSupportedException("The Doroti MAUI backend rejects an unsupported shader family.");
        return paint;
    }

    private static SKPaint FilterPaint(ImageFilterSnapshot filter) => new() { ImageFilter = ToImageFilter(filter) };

    private static SKImageFilter ToImageFilter(ImageFilterSnapshot filter)
    {
        if (filter.IsShader || filter.Outer is not null || filter.Inner is not null ||
            filter.ColorFilter is not null || filter.Matrix4 is not null)
        {
            throw new NotSupportedException("The Doroti MAUI backend currently supports blur image filters only.");
        }
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
        if (matrix.Count < 16) throw new InvalidDataException("A Doroti transform must contain 16 values.");
        var value = new SKMatrix
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
        canvas.Concat(value);
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
