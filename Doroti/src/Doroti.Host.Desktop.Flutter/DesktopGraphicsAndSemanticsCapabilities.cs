using Doroti.Backends.Skia;
using Doroti.Composition;
using Doroti.Engine;
using Doroti.Flutter.Ui;
using Doroti.Host.Desktop;
using Doroti.Platform;
using Doroti.Rendering;
using GraphicsColor = Doroti.Graphics.Color;
using GraphicsMatrix = Doroti.Graphics.Matrix;
using GraphicsOffset = Doroti.Graphics.Offset;
using GraphicsPath = Doroti.Graphics.PathGeometry;
using GraphicsRect = Doroti.Graphics.Rect;
using PlatformSemanticsAction = Doroti.Platform.SemanticsAction;
using PlatformSemanticsRole = Doroti.Platform.SemanticsRole;
using PlatformSemanticsState = Doroti.Platform.SemanticsState;
using UiImage = Doroti.Flutter.Ui.Image;
using UiPath = Doroti.Flutter.Ui.Path;
using UiSemanticsAction = Doroti.Flutter.Ui.SemanticsAction;
using UiSemanticsRole = Doroti.Flutter.Ui.SemanticsRole;

namespace Doroti.Host.Desktop.Flutter;

public sealed record DesktopFlutterFrameDiagnostics(
    long Submitted,
    long Presented,
    long Superseded,
    long Stale,
    long Failed,
    long Cancelled,
    int QueueDepth,
    int QueueHighWatermark,
    int ActiveContexts,
    int ActiveFrames,
    ulong SurfaceGeneration,
    int RecoveryCount,
    bool SoftwareFallbackUsed,
    string BackendIdentity);

public sealed record DesktopFlutterRetainedDiagnostics(
    long Hits,
    long Misses,
    long SurfaceInvalidations,
    int Entries,
    long SurfaceGeneration);

internal sealed class DesktopGraphicsAndSemanticsCapabilities :
    ISceneHostCapability,
    IParagraphHostCapability,
    IImageHostCapability,
    ISemanticsHostCapability,
    IDisposable
{
    private readonly ulong _viewId;
    private readonly IWindow _window;
    private readonly DesktopGpuFrameSink _sink;
    private readonly IAccessibilityBridge _accessibility;
    private readonly object _gate = new();
    private readonly HashSet<Task<FrameAckResult>> _pending = [];
    private readonly Dictionary<int, SemanticsNodeUpdate> _semanticsNodes = [];
    private readonly Dictionary<(long Generation, long SurfaceGeneration, double Width, double Height), DisplayList> _retained = [];
    private int? _semanticsRootId;
    private long _nextFrame;
    private long _submitted;
    private long _presented;
    private long _superseded;
    private long _stale;
    private long _failed;
    private long _cancelled;
    private bool _semanticsEnabled;
    private long _retainedHits;
    private long _retainedMisses;
    private long _retainedSurfaceInvalidations;
    private long _retainedSurfaceGeneration;
    private bool _disposed;

    internal DesktopGraphicsAndSemanticsCapabilities(ulong viewId, IWindow window)
    {
        _viewId = viewId;
        _window = window;
        _sink = new(window);
        if (!window.TryGetFeature<IAccessibilityBridge>(out var accessibility) || accessibility is null)
        {
            _sink.Dispose();
            throw new NotSupportedException("The native window does not expose an accessibility bridge.");
        }
        _accessibility = accessibility;
    }

    public event Action<SemanticsActionEvent>? Action;

    internal nint NativeWindowHandle => _window.TryGetFeature<INativeWindowHandleDiagnostics>(out var native) && native is not null
        ? native.Handle
        : throw new NotSupportedException("The active window does not expose a native handle diagnostic.");

    internal DesktopFlutterFrameDiagnostics Diagnostics
    {
        get
        {
            var resources = _sink.Resources;
            lock (_gate)
            {
                return new(
                    _submitted,
                    _presented,
                    _superseded,
                    _stale,
                    _failed,
                    _cancelled,
                    _sink.QueueDepth,
                    _sink.QueueHighWatermark,
                    resources.ActiveContexts,
                    resources.ActiveFrames,
                    _sink.SurfaceGeneration.Value,
                    _sink.RecoveryCount,
                    _sink.SoftwareFallbackUsed,
                    _sink.BackendIdentity);
            }
        }
    }

    internal void FailNextFrameForValidation() => _sink.FailNextFrameForValidation();

    internal DesktopFlutterRetainedDiagnostics RetainedDiagnostics
    {
        get
        {
            lock (_gate)
                return new(_retainedHits, _retainedMisses, _retainedSurfaceInvalidations, _retained.Count, _retainedSurfaceGeneration);
        }
    }

    internal async Task<DesktopFlutterPixelReadback> CaptureNextFrameAsync()
    {
        var readback = await _sink.CaptureNextFrameAsync().ConfigureAwait(false);
        return new(
            readback.FrameId.Value,
            checked((int)readback.PixelSize.Width),
            checked((int)readback.PixelSize.Height),
            readback.RowBytes,
            readback.Bgra8888Pixels);
    }

    public void Submit(ulong viewId, Scene scene, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (viewId != _viewId || scene.viewId != _viewId)
        {
            throw new FlutterCapabilityException(FlutterCapabilityIds.GraphicsScene, viewId, invocation, "scene/view ownership mismatch");
        }

        var metrics = _window.Metrics;
        var root = TranslateScene(scene, metrics);
        var sequence = Interlocked.Increment(ref _nextFrame);
        var snapshot = LayerTreeSnapshot.Create(root);
        var frame = new RenderPipelineFrame(
            root,
            snapshot,
            sequence,
            new(metrics.LogicalSize, metrics.PixelSize, metrics.ScaleFactor, metrics.SurfaceGeneration));
        var task = _sink.PresentAsync(new((ulong)sequence), frame).AsTask();
        lock (_gate)
        {
            _submitted++;
            _pending.Add(task);
        }
        _ = task.ContinueWith(
            completed => ObserveAck(completed),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }

    public Paragraph Layout(ParagraphRequest request, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(request);
        var metrics = SkiaTextMeasurer.Measure(request.Text, request.FontFamily, request.FontSize, request.Width);
        return new(request.Text, metrics.Width, metrics.Height);
    }

    public async ValueTask<UiImage> DecodeAsync(
        ReadOnlyMemory<byte> bytes,
        DartUiInvocation invocation,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var key = $"flutter-view-{_viewId}:{System.Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(bytes.Span))}";
        var lease = await _sink.Images.ResolveAsync(
            new EncodedImageProvider(key, bytes, new SkiaImageDecoder()),
            cancellationToken).ConfigureAwait(false);
        var handle = DesktopImageHandle.Create(lease);
        return new UiImage(_viewId, checked((int)lease.Size.Width), checked((int)lease.Size.Height), handle.Release)
        {
            HostHandle = handle,
        };
    }

    public void SetEnabled(bool enabled, DartUiInvocation invocation) => _semanticsEnabled = enabled;

    public void Update(SemanticsUpdate update, DartUiInvocation invocation)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(update);
        if (!_semanticsEnabled || update.nodes.Count == 0)
        {
            return;
        }

        foreach (var node in update.nodes) _semanticsNodes[node.id] = node;
        if (_semanticsRootId is null)
        {
            var childIds = _semanticsNodes.Values.SelectMany(node => node.children).ToHashSet();
            _semanticsRootId = _semanticsNodes.Values.Where(node => !childIds.Contains(node.id))
                .OrderBy(node => node.indexInParent ?? int.MaxValue)
                .ThenBy(node => node.id)
                .Select(node => (int?)node.id)
                .FirstOrDefault() ?? throw new InvalidDataException("The Flutter semantics update has no root node.");
        }
        var root = _semanticsNodes[_semanticsRootId.Value];
        _accessibility.Update(
            new(update.generation, ConvertNode(root, _semanticsNodes, new HashSet<int>())),
            request => DispatchAction(request));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Task<FrameAckResult>[] pending;
        lock (_gate) pending = _pending.ToArray();
        if (pending.Length > 0 && !Task.WaitAll(pending, TimeSpan.FromSeconds(15)))
        {
            throw new TimeoutException("Flutter scene terminal ACKs did not drain within 15 seconds.");
        }
        _sink.Dispose();
        lock (_gate) _retained.Clear();
    }

    private Layer TranslateScene(Scene scene, WindowMetrics metrics)
    {
        var logicalSize = metrics.LogicalSize;
        lock (_gate)
        {
            if (_retainedSurfaceGeneration != 0 && _retainedSurfaceGeneration != metrics.SurfaceGeneration)
            {
                _retained.Clear();
                _retainedSurfaceInvalidations++;
            }
            _retainedSurfaceGeneration = metrics.SurfaceGeneration;
        }
        var builder = new DisplayListBuilder(GraphicsRect.FromLeftTopWidthHeight(0, 0, logicalSize.Width, logicalSize.Height));
        var saveDepth = 0;
        TranslateCommands(scene.Commands, builder, ref saveDepth);
        if (saveDepth != 0)
            throw new InvalidDataException($"Flutter scene ended with {saveDepth} unclosed effect scope(s).");
        return new PictureLayer(GraphicsOffset.Zero, builder.Build());

        void TranslateCommands(IReadOnlyList<SceneCommand> commands, DisplayListBuilder target, ref int depth)
        {
            foreach (var command in commands)
            {
                switch (command.Operation)
                {
                    case "picture" when command.HostPayload is ScenePicturePayload picture:
                        target.Save();
                        target.Transform(GraphicsMatrix.CreateTranslation(picture.Offset.dx, picture.Offset.dy));
                        TranslatePicture(picture.Picture, target);
                        target.Restore();
                        break;
                    case "offset" when command.HostPayload is SceneOffsetPayload offset:
                        target.Save();
                        depth++;
                        target.Transform(GraphicsMatrix.CreateTranslation(offset.Dx, offset.Dy));
                        break;
                    case "clipRect" when command.HostPayload is SceneClipRectPayload clip:
                        target.Save();
                        depth++;
                        target.ClipRect(Convert(clip.Rect));
                        break;
                    case "clipRRect" when command.HostPayload is SceneClipRRectPayload clip:
                        target.Save();
                        depth++;
                        target.ClipPath(Convert(clip.RRect));
                        break;
                    case "clipRSuperellipse" when command.HostPayload is SceneClipRSuperellipsePayload clip:
                        target.Save();
                        depth++;
                        target.ClipPath(Convert(clip.RSuperellipse));
                        break;
                    case "clipPath" when command.HostPayload is SceneClipPathPayload clip:
                        target.Save();
                        depth++;
                        target.ClipPath(Convert(clip.Path));
                        break;
                    case "transform" when command.HostPayload is SceneTransformPayload transform:
                        target.Save();
                        depth++;
                        target.Transform(Convert(transform.Matrix4));
                        break;
                    case "opacity" when command.HostPayload is SceneOpacityPayload opacity:
                        target.SaveLayer(new RasterLayerOptions(Opacity: opacity.Opacity));
                        depth++;
                        target.Transform(GraphicsMatrix.CreateTranslation(opacity.Offset.dx, opacity.Offset.dy));
                        break;
                    case "colorFilter" when command.HostPayload is SceneColorFilterPayload color:
                        target.SaveLayer(new RasterLayerOptions(ColorFilter: Convert(color.Filter)));
                        depth++;
                        break;
                    case "imageFilter" when command.HostPayload is SceneImageFilterPayload image:
                        target.SaveLayer(new RasterLayerOptions(ImageFilter: Convert(image.Filter)));
                        depth++;
                        target.Transform(GraphicsMatrix.CreateTranslation(image.Offset.dx, image.Offset.dy));
                        break;
                    case "backdropFilter" when command.HostPayload is SceneBackdropFilterPayload backdrop:
                        target.SaveLayer(new RasterLayerOptions(
                            Bounds: backdrop.Filter.Bounds is { } backdropBounds ? Convert(backdropBounds) : null,
                            BlendMode: Convert(backdrop.BlendMode),
                            BackdropFilter: Convert(backdrop.Filter)));
                        depth++;
                        break;
                    case "retained" when command.HostPayload is SceneRetainedPayload retained:
                        if (retained.ViewId != scene.viewId)
                            throw new InvalidOperationException("Retained scene subtree belongs to a different Flutter view.");
                        target.Replay(GetRetainedDisplayList(retained));
                        break;
                    case "pop" when depth > 0:
                        target.Restore();
                        depth--;
                        break;
                    case "shaderMask":
                        throw new NotSupportedException("Flutter scene operation 'shaderMask' reached a backend without shader-mask raster capability.");
                    default:
                        throw new NotSupportedException($"Flutter scene operation '{command.Operation}' has no strict-GPU mapping; silent downgrade is forbidden.");
                }
            }

        }

        DisplayList GetRetainedDisplayList(SceneRetainedPayload retained)
        {
            var key = (retained.Generation, metrics.SurfaceGeneration, logicalSize.Width, logicalSize.Height);
            lock (_gate)
            {
                if (_retained.TryGetValue(key, out var cached))
                {
                    _retainedHits++;
                    return cached;
                }
                _retainedMisses++;
            }

            var retainedBuilder = new DisplayListBuilder(
                GraphicsRect.FromLeftTopWidthHeight(0, 0, logicalSize.Width, logicalSize.Height));
            var retainedDepth = 0;
            TranslateCommands(retained.Commands, retainedBuilder, ref retainedDepth);
            if (retainedDepth != 0)
                throw new InvalidDataException($"Retained Flutter scene ended with {retainedDepth} unclosed effect scope(s).");
            var translated = retainedBuilder.Build();
            lock (_gate)
            {
                _retained[key] = translated;
                while (_retained.Count > 16) _retained.Remove(_retained.Keys.First());
            }
            return translated;
        }
    }

    private static void TranslatePicture(Picture picture, DisplayListBuilder builder)
    {
        ObjectDisposedException.ThrowIf(picture.debugDisposed, picture);
        foreach (var command in picture.Commands)
        {
            switch (command.Operation)
            {
                case "save": builder.Save(); break;
                case "saveLayer" when command.HostPayload is CanvasSaveLayerPayload layer:
                    builder.SaveLayer(new RasterLayerOptions(
                        Bounds: layer.Bounds is null ? null : Convert(layer.Bounds.Value),
                        Opacity: layer.Paint.Color.alpha / 255d,
                        BlendMode: Convert(layer.Paint.BlendMode),
                        ColorFilter: Convert(layer.Paint.ColorFilter)));
                    break;
                case "restore": builder.Restore(); break;
                case "translate": builder.Transform(GraphicsMatrix.CreateTranslation(command.Arguments[0], command.Arguments[1])); break;
                case "scale": builder.Transform(GraphicsMatrix.CreateScale(command.Arguments[0], command.Arguments[1])); break;
                case "rotate":
                    var cosine = Math.Cos(command.Arguments[0]);
                    var sine = Math.Sin(command.Arguments[0]);
                    builder.Transform(GraphicsMatrix.Identity with { M11 = cosine, M12 = -sine, M21 = sine, M22 = cosine });
                    break;
                case "transform": builder.Transform(Convert(command.Arguments)); break;
                case "clipRect": builder.ClipRect(new(command.Arguments[0], command.Arguments[1], command.Arguments[2], command.Arguments[3])); break;
                case "clipRRect" when command.HostPayload is CanvasClipRRectPayload roundedClip:
                    builder.ClipPath(Convert(roundedClip.RRect));
                    break;
                case "clipRSuperellipse" when command.HostPayload is CanvasClipRSuperellipsePayload superellipseClip:
                    builder.ClipPath(Convert(superellipseClip.RSuperellipse));
                    break;
                case "clipPath" when command.HostPayload is CanvasClipPathPayload pathClip:
                    builder.ClipPath(Convert(pathClip.Path));
                    break;
                case "drawRect" when command.HostPayload is CanvasRectPayload rect:
                    builder.DrawRect(Convert(rect.Rect), Convert(rect.Paint));
                    break;
                case "drawRRect" when command.HostPayload is CanvasRRectPayload rounded:
                    builder.DrawPath(Convert(rounded.RRect), Convert(rounded.Paint));
                    break;
                case "drawRSuperellipse" when command.HostPayload is CanvasRSuperellipsePayload superellipse:
                    builder.DrawPath(Convert(superellipse.RSuperellipse), Convert(superellipse.Paint));
                    break;
                case "drawDRRect" when command.HostPayload is CanvasDRRectPayload doubleRounded:
                    DrawDoubleRoundedRect(builder, doubleRounded);
                    break;
                case "drawPath" when command.HostPayload is CanvasPathPayload path:
                    builder.DrawPath(Convert(path.Path), Convert(path.Paint));
                    break;
                case "drawPaint" when command.HostPayload is PaintSnapshot paint:
                    builder.DrawRect(GraphicsRect.FromLeftTopWidthHeight(0, 0, builder.CullSize.Width, builder.CullSize.Height), Convert(paint));
                    break;
                case "drawImageRect" when command.HostPayload is CanvasImagePayload image && image.Image.HostHandle is DesktopImageHandle handle:
                    builder.DrawImage(handle.Resource, Convert(image.Source), Convert(image.Destination), Alpha(image.Paint));
                    break;
                case "drawParagraph" when command.HostPayload is CanvasParagraphPayload paragraph:
                    builder.DrawText(
                        paragraph.Paragraph.text,
                        new(paragraph.Offset.dx, paragraph.Offset.dy + paragraph.Paragraph.alphabeticBaseline),
                        paragraph.Paragraph.fontSize,
                        new(new GraphicsColor(checked((uint)paragraph.Paragraph.color.value))),
                        paragraph.Paragraph.fontFamily);
                    break;
                case "drawCircle" when command.HostPayload is CanvasCirclePayload circle:
                    builder.DrawPath(Ellipse(circle.Center.dx - circle.Radius, circle.Center.dy - circle.Radius, circle.Center.dx + circle.Radius, circle.Center.dy + circle.Radius), Convert(circle.Paint));
                    break;
                case "drawOval" when command.HostPayload is CanvasOvalPayload oval:
                    builder.DrawPath(Ellipse(oval.Rect.left, oval.Rect.top, oval.Rect.right, oval.Rect.bottom), Convert(oval.Paint));
                    break;
                case "drawLine" when command.HostPayload is CanvasLinePayload line:
                    builder.DrawPath(new GraphicsPath([new(line.Start.dx, line.Start.dy), new(line.End.dx, line.End.dy)], false), Convert(line.Paint));
                    break;
                case "drawArc" when command.HostPayload is CanvasArcPayload arc:
                    builder.DrawPath(Arc(arc), Convert(arc.Paint));
                    break;
                case "drawColor" when command.HostPayload is CanvasColorPayload color:
                    builder.SaveLayer(new RasterLayerOptions(BlendMode: Convert(color.BlendMode)));
                    builder.DrawColor(Convert(color.Color));
                    builder.Restore();
                    break;
                case "drawImage" when command.HostPayload is CanvasImagePayload image && image.Image.HostHandle is DesktopImageHandle handle:
                    builder.DrawImage(handle.Resource, Convert(image.Source), Convert(image.Destination), Alpha(image.Paint));
                    break;
                case "drawShadow" when command.HostPayload is CanvasShadowPayload shadow:
                    // Flutter elevation contains a diffuse ambient shadow and a lower
                    // spot shadow. Keep both passes in the backend-neutral display list
                    // so strict-GPU presentation has the same soft depth cue.
                    var elevation = Math.Max(0, shadow.Elevation);
                    builder.Save();
                    builder.Transform(GraphicsMatrix.CreateTranslation(0, elevation * 0.2));
                    builder.DrawPath(Convert(shadow.Path), new RasterPaint(
                        Convert(shadow.Color), shadow.TransparentOccluder ? 0.18 : 0.24, Math.Max(0.75, elevation * 0.45)));
                    builder.Restore();
                    builder.Save();
                    builder.Transform(GraphicsMatrix.CreateTranslation(0, elevation * 0.55));
                    builder.DrawPath(Convert(shadow.Path), new RasterPaint(
                        Convert(shadow.Color), shadow.TransparentOccluder ? 0.24 : 0.32, Math.Max(1.0, elevation * 0.8)));
                    builder.Restore();
                    break;
                default:
                    throw new NotSupportedException($"Flutter canvas operation '{command.Operation}' has no strict-GPU mapping.");
            }
        }
    }

    private void ObserveAck(Task<FrameAckResult> task)
    {
        lock (_gate)
        {
            _pending.Remove(task);
            if (task.IsFaulted || task.IsCanceled)
            {
                _failed++;
                return;
            }
            switch (task.Result.Status)
            {
                case FrameAckStatus.Presented: _presented++; break;
                case FrameAckStatus.Superseded: _superseded++; break;
                case FrameAckStatus.Stale: _stale++; break;
                case FrameAckStatus.Failed: _failed++; break;
                case FrameAckStatus.Cancelled: _cancelled++; break;
            }
        }
    }

    private bool DispatchAction(SemanticsActionRequest request)
    {
        var action = request.Action switch
        {
            PlatformSemanticsAction.Tap or PlatformSemanticsAction.Toggle => UiSemanticsAction.tap,
            PlatformSemanticsAction.Focus => UiSemanticsAction.focus,
            PlatformSemanticsAction.SetText => UiSemanticsAction.setText,
            PlatformSemanticsAction.SetSelection => UiSemanticsAction.setSelection,
            PlatformSemanticsAction.ScrollUp => UiSemanticsAction.scrollUp,
            PlatformSemanticsAction.ScrollDown => UiSemanticsAction.scrollDown,
            PlatformSemanticsAction.Dismiss => UiSemanticsAction.dismiss,
            _ => UiSemanticsAction.none,
        };
        if (action == UiSemanticsAction.none) return false;
        Action?.Invoke(new(_viewId, request.NodeId, action, request.Arguments));
        return true;
    }

    private static SemanticsNodeSnapshot ConvertNode(
        SemanticsNodeUpdate node,
        IReadOnlyDictionary<int, SemanticsNodeUpdate> nodes,
        HashSet<int> ancestry)
    {
        if (!ancestry.Add(node.id)) throw new InvalidDataException($"Semantics traversal contains a cycle at node {node.id}.");
        var flags = node.flags ?? SemanticsFlags.none;
        var children = node.children.Select(id => nodes.TryGetValue(id, out var child)
                ? ConvertNode(child, nodes, ancestry)
                : throw new InvalidDataException($"Semantics node {node.id} references missing child {id}."))
            .ToArray();
        ancestry.Remove(node.id);
        return new(
            node.id,
            Role(node.role, flags),
            node.label,
            node.value,
            State(flags),
            Actions(node.actions, flags),
            Convert(node.rect),
            children,
            node.indexInParent);
    }

    private static PlatformSemanticsRole Role(UiSemanticsRole role, SemanticsFlags flags) =>
        flags.isToggled != Tristate.none || flags.isChecked != CheckedState.none ? PlatformSemanticsRole.CheckBox :
        flags.isButton ? PlatformSemanticsRole.Button :
        flags.isTextField ? PlatformSemanticsRole.TextField :
        flags.isImage ? PlatformSemanticsRole.Image :
        role is UiSemanticsRole.list ? PlatformSemanticsRole.List :
        role is UiSemanticsRole.listItem ? PlatformSemanticsRole.ListItem :
        role is UiSemanticsRole.dialog or UiSemanticsRole.alertDialog ? PlatformSemanticsRole.Dialog :
        flags.isSlider ? PlatformSemanticsRole.Slider : PlatformSemanticsRole.Generic;

    private static PlatformSemanticsState State(SemanticsFlags flags)
    {
        var state = PlatformSemanticsState.None;
        if (flags.isEnabled != Tristate.isFalse) state |= PlatformSemanticsState.Enabled;
        if (flags.isFocused == Tristate.isTrue) state |= PlatformSemanticsState.Focused;
        if (flags.isSelected == Tristate.isTrue) state |= PlatformSemanticsState.Selected;
        if (flags.isReadOnly) state |= PlatformSemanticsState.ReadOnly;
        if (flags.isMultiline) state |= PlatformSemanticsState.Multiline;
        if (flags.isHidden) state |= PlatformSemanticsState.Hidden;
        if (flags.isChecked == CheckedState.isTrue) state |= PlatformSemanticsState.Checked;
        if (flags.isChecked == CheckedState.mixed) state |= PlatformSemanticsState.Mixed;
        if (flags.isToggled == Tristate.isTrue) state |= PlatformSemanticsState.Toggled;
        return state;
    }

    private static PlatformSemanticsAction Actions(UiSemanticsAction actions, SemanticsFlags flags)
    {
        var mapped = PlatformSemanticsAction.None;
        if (actions.HasFlag(UiSemanticsAction.tap)) mapped |= PlatformSemanticsAction.Tap;
        if (actions.HasFlag(UiSemanticsAction.focus)) mapped |= PlatformSemanticsAction.Focus;
        if (actions.HasFlag(UiSemanticsAction.setText)) mapped |= PlatformSemanticsAction.SetText;
        if (actions.HasFlag(UiSemanticsAction.setSelection)) mapped |= PlatformSemanticsAction.SetSelection;
        if (actions.HasFlag(UiSemanticsAction.scrollUp)) mapped |= PlatformSemanticsAction.ScrollUp;
        if (actions.HasFlag(UiSemanticsAction.scrollDown)) mapped |= PlatformSemanticsAction.ScrollDown;
        if (actions.HasFlag(UiSemanticsAction.dismiss)) mapped |= PlatformSemanticsAction.Dismiss;
        if ((flags.isToggled != Tristate.none || flags.isChecked != CheckedState.none) && actions.HasFlag(UiSemanticsAction.tap))
            mapped |= PlatformSemanticsAction.Toggle;
        return mapped;
    }

    private static GraphicsRect Convert(Rect value) => new(value.left, value.top, value.right, value.bottom);
    private static GraphicsColor Convert(Color value) => new(value.value);
    private static RasterPaint Convert(PaintSnapshot value)
    {
        if (value.Shader is not null)
            throw new NotSupportedException($"Flutter paint shader '{value.Shader.GetType().Name}' reached a backend without shader raster capability.");
        if (value.InvertColors)
            throw new NotSupportedException("Flutter Paint.invertColors reached a backend without invert-color capability.");
        return new(
        Convert(value.Color),
        1,
        BlurSigma: value.MaskFilter?.sigma ?? 0,
        Style: value.Style == PaintingStyle.stroke ? RasterPaintStyle.Stroke : RasterPaintStyle.Fill,
        StrokeWidth: value.StrokeWidth,
        BlendMode: Convert(value.BlendMode),
        IsAntiAlias: value.IsAntiAlias,
        ColorFilter: Convert(value.ColorFilter));
    }

    private static double Alpha(PaintSnapshot value) => value.Color.alpha / 255d;

    private static RasterBlendMode Convert(BlendMode value) => value switch
    {
        BlendMode.clear => RasterBlendMode.Clear,
        BlendMode.src => RasterBlendMode.Source,
        BlendMode.dst => RasterBlendMode.Destination,
        BlendMode.srcOver => RasterBlendMode.SourceOver,
        BlendMode.dstOver => RasterBlendMode.DestinationOver,
        BlendMode.srcIn => RasterBlendMode.SourceIn,
        BlendMode.dstIn => RasterBlendMode.DestinationIn,
        BlendMode.srcOut => RasterBlendMode.SourceOut,
        BlendMode.dstOut => RasterBlendMode.DestinationOut,
        BlendMode.srcATop => RasterBlendMode.SourceAtop,
        BlendMode.dstATop => RasterBlendMode.DestinationAtop,
        BlendMode.xor => RasterBlendMode.Xor,
        BlendMode.plus => RasterBlendMode.Plus,
        BlendMode.modulate => RasterBlendMode.Modulate,
        BlendMode.screen => RasterBlendMode.Screen,
        BlendMode.overlay => RasterBlendMode.Overlay,
        BlendMode.darken => RasterBlendMode.Darken,
        BlendMode.lighten => RasterBlendMode.Lighten,
        BlendMode.colorDodge => RasterBlendMode.ColorDodge,
        BlendMode.colorBurn => RasterBlendMode.ColorBurn,
        BlendMode.hardLight => RasterBlendMode.HardLight,
        BlendMode.softLight => RasterBlendMode.SoftLight,
        BlendMode.difference => RasterBlendMode.Difference,
        BlendMode.exclusion => RasterBlendMode.Exclusion,
        BlendMode.multiply => RasterBlendMode.Multiply,
        BlendMode.hue => RasterBlendMode.Hue,
        BlendMode.saturation => RasterBlendMode.Saturation,
        BlendMode.color => RasterBlendMode.Color,
        BlendMode.luminosity => RasterBlendMode.Luminosity,
        _ => throw new NotSupportedException($"Unsupported Flutter blend mode '{value}'."),
    };

    private static RasterTileMode Convert(TileMode value) => value switch
    {
        TileMode.clamp => RasterTileMode.Clamp,
        TileMode.repeated => RasterTileMode.Repeat,
        TileMode.mirror => RasterTileMode.Mirror,
        TileMode.decal => RasterTileMode.Decal,
        _ => throw new NotSupportedException($"Unsupported Flutter tile mode '{value}'."),
    };

    private static RasterColorFilter? Convert(ColorFilterSnapshot? value)
    {
        if (value is null) return null;
        return value.Kind switch
        {
            ColorFilterKind.mode => new RasterColorFilter(RasterColorFilterKind.Mode,
                value.Color is null ? throw new InvalidDataException("Mode color filter has no color.") : Convert(value.Color),
                Convert(value.BlendMode)).Validate(),
            ColorFilterKind.matrix => new RasterColorFilter(RasterColorFilterKind.Matrix,
                Matrix: value.Matrix is null ? null : Array.AsReadOnly(value.Matrix.ToArray())).Validate(),
            ColorFilterKind.linearToSrgbGamma => new RasterColorFilter(RasterColorFilterKind.LinearToSrgbGamma).Validate(),
            ColorFilterKind.srgbToLinearGamma => new RasterColorFilter(RasterColorFilterKind.SrgbToLinearGamma).Validate(),
            _ => throw new NotSupportedException($"Unsupported Flutter color filter '{value.Kind}'."),
        };
    }

    private static RasterImageFilter Convert(ImageFilterSnapshot value)
    {
        if (value.IsShader)
            throw new NotSupportedException("Fragment shader image filters are explicitly unsupported by the current backend.");
        if (value.Outer is not null && value.Inner is not null)
            return new RasterImageFilter(RasterImageFilterKind.Compose,
                Outer: Convert(value.Outer), Inner: Convert(value.Inner)).Validate();
        if (value.ColorFilter is not null && value.Inner is not null)
            return new RasterImageFilter(RasterImageFilterKind.Compose,
                Outer: new RasterImageFilter(RasterImageFilterKind.ColorFilter, ColorFilter: Convert(value.ColorFilter)),
                Inner: Convert(value.Inner)).Validate();
        if (value.ColorFilter is not null)
            return new RasterImageFilter(RasterImageFilterKind.ColorFilter, ColorFilter: Convert(value.ColorFilter)).Validate();
        if (value.Matrix4 is not null)
            return new RasterImageFilter(RasterImageFilterKind.Matrix,
                Matrix: Array.AsReadOnly(value.Matrix4.ToArray())).Validate();
        return new RasterImageFilter(RasterImageFilterKind.Blur, value.SigmaX, value.SigmaY, Convert(value.TileMode)).Validate();
    }

    private static GraphicsPath Convert(UiPath value)
    {
        var points = new List<GraphicsOffset>();
        foreach (var command in value.Commands)
        {
            var args = command.Arguments;
            switch (command.Operation)
            {
                case "moveTo" or "lineTo" when args.Count >= 2:
                    points.Add(new(args[0], args[1]));
                    break;
                case "quadraticBezierTo" or "conicTo" when args.Count >= 4:
                    points.Add(new(args[2], args[3]));
                    break;
                case "cubicTo" when args.Count >= 6:
                    points.Add(new(args[4], args[5]));
                    break;
                case "addRect" when args.Count >= 4:
                    points.Add(new(args[0], args[1]));
                    points.Add(new(args[2], args[1]));
                    points.Add(new(args[2], args[3]));
                    points.Add(new(args[0], args[3]));
                    break;
                case "addRRect" or "addRSuperellipse" when args.Count >= 12:
                    AddRoundedRect(args);
                    break;
                case "addRRect" or "addRSuperellipse" when args.Count >= 4:
                    points.Add(new(args[0], args[1]));
                    points.Add(new(args[2], args[1]));
                    points.Add(new(args[2], args[3]));
                    points.Add(new(args[0], args[3]));
                    break;
                case "addOval" when args.Count >= 4:
                    AddOval(args[0], args[1], args[2], args[3]);
                    break;
                case "addArc" or "arcTo" when args.Count >= 6:
                    AddArc(args[0], args[1], args[2], args[3], args[4], args[5]);
                    break;
                case "arcToPoint" when args.Count >= 2:
                    points.Add(new(args[0], args[1]));
                    break;
            }
        }
        if (points.Count < 2) throw new NotSupportedException($"Strict-GPU path mapping requires at least two path vertices; operations={string.Join(',', value.Commands.Select(item => item.Operation))}.");
        const double finitePathExtent = 1_000_000;
        var finitePoints = points.Select(point => new GraphicsOffset(
            Normalize(point.X),
            Normalize(point.Y))).ToList();
        return new(finitePoints, value.Commands.Any(item => item.Operation == "close"),
            value.fillType == PathFillType.evenOdd ? Doroti.Graphics.PathFillRule.EvenOdd : Doroti.Graphics.PathFillRule.NonZero);

        static double Normalize(double coordinate) => double.IsFinite(coordinate)
            ? Math.Clamp(coordinate, -finitePathExtent, finitePathExtent)
            : 0;

        void AddOval(double left, double top, double right, double bottom)
        {
            var centerX = (left + right) / 2;
            var centerY = (top + bottom) / 2;
            var radiusX = Math.Abs(right - left) / 2;
            var radiusY = Math.Abs(bottom - top) / 2;
            for (var index = 0; index < 24; index++)
            {
                var angle = Math.PI * 2 * index / 24;
                points.Add(new(centerX + Math.Cos(angle) * radiusX, centerY + Math.Sin(angle) * radiusY));
            }
        }

        void AddArc(double left, double top, double right, double bottom, double start, double sweep)
        {
            var centerX = (left + right) / 2;
            var centerY = (top + bottom) / 2;
            var radiusX = Math.Abs(right - left) / 2;
            var radiusY = Math.Abs(bottom - top) / 2;
            for (var index = 0; index <= 16; index++)
            {
                var angle = start + sweep * index / 16;
                points.Add(new(centerX + Math.Cos(angle) * radiusX, centerY + Math.Sin(angle) * radiusY));
            }
        }

        void AddRoundedRect(IReadOnlyList<double> args)
        {
            const int steps = 6;
            var left = args[0];
            var top = args[1];
            var right = args[2];
            var bottom = args[3];
            var width = Math.Max(0, right - left);
            var height = Math.Max(0, bottom - top);
            var radii = new[]
            {
                Math.Max(0, args[4]), Math.Max(0, args[5]),
                Math.Max(0, args[6]), Math.Max(0, args[7]),
                Math.Max(0, args[8]), Math.Max(0, args[9]),
                Math.Max(0, args[10]), Math.Max(0, args[11]),
            };
            var scale = Math.Min(1d, new[]
            {
                Ratio(width, radii[0] + radii[2]),
                Ratio(width, radii[6] + radii[4]),
                Ratio(height, radii[1] + radii[7]),
                Ratio(height, radii[3] + radii[5]),
            }.Min());
            for (var index = 0; index < radii.Length; index++) radii[index] *= scale;

            AddCorner(right - radii[2], top + radii[3], radii[2], radii[3], -Math.PI / 2, 0);
            AddCorner(right - radii[4], bottom - radii[5], radii[4], radii[5], 0, Math.PI / 2);
            AddCorner(left + radii[6], bottom - radii[7], radii[6], radii[7], Math.PI / 2, Math.PI);
            AddCorner(left + radii[0], top + radii[1], radii[0], radii[1], Math.PI, Math.PI * 1.5);

            static double Ratio(double available, double requested) => requested > 0 ? available / requested : 1d;

            void AddCorner(double centerX, double centerY, double radiusX, double radiusY, double start, double end)
            {
                if (radiusX == 0 || radiusY == 0)
                {
                    points.Add(new(centerX + Math.Cos(end) * radiusX, centerY + Math.Sin(end) * radiusY));
                    return;
                }
                for (var index = 0; index <= steps; index++)
                {
                    var angle = start + ((end - start) * index / steps);
                    points.Add(new(centerX + Math.Cos(angle) * radiusX, centerY + Math.Sin(angle) * radiusY));
                }
            }
        }
    }

    private static GraphicsPath Convert(RRect value)
    {
        const int steps = 6;
        var points = new List<GraphicsOffset>(steps * 4);
        AddCorner(value.right - value.trRadiusX, value.top + value.trRadiusY, value.trRadiusX, value.trRadiusY, -Math.PI / 2, 0);
        AddCorner(value.right - value.brRadiusX, value.bottom - value.brRadiusY, value.brRadiusX, value.brRadiusY, 0, Math.PI / 2);
        AddCorner(value.left + value.blRadiusX, value.bottom - value.blRadiusY, value.blRadiusX, value.blRadiusY, Math.PI / 2, Math.PI);
        AddCorner(value.left + value.tlRadiusX, value.top + value.tlRadiusY, value.tlRadiusX, value.tlRadiusY, Math.PI, Math.PI * 1.5);
        return new GraphicsPath(points, true, Doroti.Graphics.PathFillRule.NonZero);

        void AddCorner(double centerX, double centerY, double radiusX, double radiusY, double start, double end)
        {
            for (var index = 0; index <= steps; index++)
            {
                var angle = start + ((end - start) * index / steps);
                points.Add(new GraphicsOffset(centerX + Math.Cos(angle) * radiusX, centerY + Math.Sin(angle) * radiusY));
            }
        }
    }

    private static GraphicsPath Convert(RSuperellipse value)
    {
        // Flutter's rounded superellipse uses a continuous high-order corner.
        // Sample each corner independently so it never degrades to a bounds rectangle.
        const int steps = 12;
        const double exponent = 4;
        var points = new List<GraphicsOffset>(steps * 4);
        // Rect.largest is a valid Flutter painting primitive. Sampling its
        // enormous coordinates directly can overflow the corner arithmetic,
        // so cap only non-finite/overflowing geometry to a viewport-dominating
        // finite extent. Normal finite superellipses remain unchanged.
        const double finiteExtent = 1_000_000;
        var left = Finite(value.outerRect.left, -finiteExtent);
        var top = Finite(value.outerRect.top, -finiteExtent);
        var right = Finite(value.outerRect.right, finiteExtent);
        var bottom = Finite(value.outerRect.bottom, finiteExtent);
        var maxRadiusX = Math.Max(0, (right - left) / 2);
        var maxRadiusY = Math.Max(0, (bottom - top) / 2);
        var trX = Radius(value.trRadiusX, maxRadiusX);
        var trY = Radius(value.trRadiusY, maxRadiusY);
        var brX = Radius(value.brRadiusX, maxRadiusX);
        var brY = Radius(value.brRadiusY, maxRadiusY);
        var blX = Radius(value.blRadiusX, maxRadiusX);
        var blY = Radius(value.blRadiusY, maxRadiusY);
        var tlX = Radius(value.tlRadiusX, maxRadiusX);
        var tlY = Radius(value.tlRadiusY, maxRadiusY);
        AddCorner(right - trX, top + trY, trX, trY, -Math.PI / 2, 0);
        AddCorner(right - brX, bottom - brY, brX, brY, 0, Math.PI / 2);
        AddCorner(left + blX, bottom - blY, blX, blY, Math.PI / 2, Math.PI);
        AddCorner(left + tlX, top + tlY, tlX, tlY, Math.PI, Math.PI * 1.5);
        return new GraphicsPath(points, true, Doroti.Graphics.PathFillRule.NonZero);

        static double Finite(double candidate, double fallback) =>
            double.IsFinite(candidate) && Math.Abs(candidate) <= finiteExtent ? candidate : fallback;

        static double Radius(double candidate, double maximum) =>
            double.IsFinite(candidate) ? Math.Clamp(Math.Abs(candidate), 0, maximum) : 0;

        void AddCorner(double centerX, double centerY, double radiusX, double radiusY, double start, double end)
        {
            for (var index = 0; index <= steps; index++)
            {
                var angle = start + ((end - start) * index / steps);
                var cosine = Math.Cos(angle);
                var sine = Math.Sin(angle);
                var x = Math.Sign(cosine) * Math.Pow(Math.Abs(cosine), 2 / exponent);
                var y = Math.Sign(sine) * Math.Pow(Math.Abs(sine), 2 / exponent);
                points.Add(new(centerX + (x * radiusX), centerY + (y * radiusY)));
            }
        }
    }

    private static void DrawDoubleRoundedRect(DisplayListBuilder builder, CanvasDRRectPayload value)
    {
        // The backend-neutral display list does not yet carry multi-contour
        // paths. Preserve the exact double-rect occupied area as four GPU
        // quads; rounded corners continue through the existing RRect path for
        // non-border fills, and this mapping never silently drops a border.
        var outer = value.Outer.outerRect;
        var inner = value.Inner.outerRect;
        var paint = Convert(value.Paint);
        Draw(outer.left, outer.top, outer.right, Math.Clamp(inner.top, outer.top, outer.bottom));
        Draw(outer.left, Math.Clamp(inner.bottom, outer.top, outer.bottom), outer.right, outer.bottom);
        Draw(outer.left, Math.Clamp(inner.top, outer.top, outer.bottom), Math.Clamp(inner.left, outer.left, outer.right), Math.Clamp(inner.bottom, outer.top, outer.bottom));
        Draw(Math.Clamp(inner.right, outer.left, outer.right), Math.Clamp(inner.top, outer.top, outer.bottom), outer.right, Math.Clamp(inner.bottom, outer.top, outer.bottom));

        void Draw(double left, double top, double right, double bottom)
        {
            if (right > left && bottom > top) builder.DrawRect(new(left, top, right, bottom), paint);
        }
    }

    private static GraphicsPath Ellipse(double left, double top, double right, double bottom)
    {
        const int steps = 24;
        var centerX = (left + right) / 2;
        var centerY = (top + bottom) / 2;
        var radiusX = Math.Abs(right - left) / 2;
        var radiusY = Math.Abs(bottom - top) / 2;
        return new GraphicsPath(Enumerable.Range(0, steps).Select(index =>
        {
            var angle = Math.PI * 2 * index / steps;
            return new GraphicsOffset(centerX + Math.Cos(angle) * radiusX, centerY + Math.Sin(angle) * radiusY);
        }).ToArray(), true);
    }

    private static GraphicsPath Arc(CanvasArcPayload arc)
    {
        const int steps = 16;
        var centerX = (arc.Rect.left + arc.Rect.right) / 2;
        var centerY = (arc.Rect.top + arc.Rect.bottom) / 2;
        var radiusX = Math.Abs(arc.Rect.right - arc.Rect.left) / 2;
        var radiusY = Math.Abs(arc.Rect.bottom - arc.Rect.top) / 2;
        var points = new List<GraphicsOffset>();
        if (arc.UseCenter) points.Add(new(centerX, centerY));
        for (var index = 0; index <= steps; index++)
        {
            var angle = arc.StartAngle + arc.SweepAngle * index / steps;
            points.Add(new(centerX + Math.Cos(angle) * radiusX, centerY + Math.Sin(angle) * radiusY));
        }
        return new(points, arc.UseCenter);
    }

    private static GraphicsMatrix Convert(IReadOnlyList<double> values)
    {
        if (values.Count != 16) throw new NotSupportedException("Strict-GPU transform mapping requires a 4x4 matrix.");
        return new(values[0], values[4], values[8], values[12], values[1], values[5], values[9], values[13], values[2], values[6], values[10], values[14], values[3], values[7], values[11], values[15]);
    }

    private sealed class DesktopImageHandle : IFlutterImageHandle
    {
        private readonly SharedImageLease _shared;
        private int _released;

        private DesktopImageHandle(SharedImageLease shared) => _shared = shared;

        internal ResourceId Resource => _shared.Resource;

        internal static DesktopImageHandle Create(ImageCache.ImageLease lease) => new(new(lease));

        public IFlutterImageHandle Clone()
        {
            _shared.AddRef();
            return new DesktopImageHandle(_shared);
        }

        public void Release()
        {
            if (Interlocked.Exchange(ref _released, 1) == 0) _shared.Release();
        }
    }

    private sealed class SharedImageLease(ImageCache.ImageLease lease)
    {
        private int _references = 1;
        internal ResourceId Resource => lease.Resource;
        internal void AddRef() => Interlocked.Increment(ref _references);
        internal void Release()
        {
            if (Interlocked.Decrement(ref _references) == 0) lease.Dispose();
        }
    }
}
