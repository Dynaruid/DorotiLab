// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/widget_inspector.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Widgets;

public delegate Widget ExitWidgetSelectionButtonBuilder(BuildContext context, GlobalKey<IState> key, global::System.Action onPressed, string semanticsLabel);

public delegate Widget MoveExitWidgetSelectionButtonBuilder(BuildContext context, global::System.Action onPressed, string semanticsLabel, bool usesDefaultAlignment = default!);

public delegate Widget TapBehaviorButtonBuilder(BuildContext context, global::System.Action onPressed, bool selectionOnTapEnabled, string semanticsLabel);

public delegate void RegisterServiceExtensionCallback(global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>> callback, string name);

internal class _ProxyLayer__widget_inspector : global::Doroti.Framework.Rendering.Layer
{
    internal virtual global::Doroti.Framework.Rendering.Layer _layer { get; private set; } = default!;

    internal _ProxyLayer__widget_inspector(global::Doroti.Framework.Rendering.Layer _layer)
    {
        this._layer = _layer;
    }

    public override void addToScene(SceneBuilder builder)
    {
        this._layer.addToScene(builder);
    }

    public override bool findAnnotations<S>(global::Doroti.Framework.Rendering.AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return this._layer.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MulticastCanvas__widget_inspector : Canvas
{
    internal virtual Canvas _main { get; private set; } = default!;
    internal virtual Canvas _screenshot { get; private set; } = default!;

    internal _MulticastCanvas__widget_inspector(Canvas main, Canvas screenshot)
    {
        this._main = main;
        this._screenshot = screenshot;
    }

    public virtual void clipPath(Path path, bool doAntiAlias = true)
    {
        this._main.clipPath(path, doAntiAlias: doAntiAlias);
        this._screenshot.clipPath(path, doAntiAlias: doAntiAlias);
    }

    public virtual void clipRRect(RRect rrect, bool doAntiAlias = true)
    {
        this._main.clipRRect(rrect, doAntiAlias: doAntiAlias);
        this._screenshot.clipRRect(rrect, doAntiAlias: doAntiAlias);
    }

    public virtual void clipRect(Rect rect, global::Doroti.Ui.ClipOp clipOp = default!, bool doAntiAlias = true)
    {
        this._main.clipRect(rect, clipOp: clipOp, doAntiAlias: doAntiAlias);
        this._screenshot.clipRect(rect, clipOp: clipOp, doAntiAlias: doAntiAlias);
    }

    public virtual void drawArc(Rect rect, double startAngle, double sweepAngle, bool useCenter, Paint paint)
    {
        this._main.drawArc(rect, startAngle, sweepAngle, useCenter, paint);
        this._screenshot.drawArc(rect, startAngle, sweepAngle, useCenter, paint);
    }

    public virtual void drawAtlas(global::Doroti.Ui.Image atlas, List<global::Doroti.Ui.RSTransform> transforms, List<Rect> rects, List<Color>? colors, BlendMode? blendMode, Rect? cullRect, Paint paint)
    {
        this._main.drawAtlas(atlas, transforms, rects, colors, blendMode, cullRect, paint);
        this._screenshot.drawAtlas(atlas, transforms, rects, colors, blendMode, cullRect, paint);
    }

    public virtual void drawCircle(Offset c, double radius, Paint paint)
    {
        this._main.drawCircle(c, radius, paint);
        this._screenshot.drawCircle(c, radius, paint);
    }

    public virtual void drawColor(Color color, BlendMode blendMode)
    {
        this._main.drawColor(color, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(blendMode)));
        this._screenshot.drawColor(color, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(blendMode)));
    }

    public virtual void drawDRRect(RRect outer, RRect inner, Paint paint)
    {
        this._main.drawDRRect(outer, inner, paint);
        this._screenshot.drawDRRect(outer, inner, paint);
    }

    public virtual void drawImage(global::Doroti.Ui.Image image, Offset p, Paint paint)
    {
        this._main.drawImage(image, p, paint);
        this._screenshot.drawImage(image, p, paint);
    }

    public virtual void drawImageNine(global::Doroti.Ui.Image image, Rect center, Rect dst, Paint paint)
    {
        this._main.drawImageNine(image, center, dst, paint);
        this._screenshot.drawImageNine(image, center, dst, paint);
    }

    public virtual void drawImageRect(global::Doroti.Ui.Image image, Rect src, Rect dst, Paint paint)
    {
        this._main.drawImageRect(image, src, dst, paint);
        this._screenshot.drawImageRect(image, src, dst, paint);
    }

    public virtual void drawLine(Offset p1, Offset p2, Paint paint)
    {
        this._main.drawLine(p1, p2, paint);
        this._screenshot.drawLine(p1, p2, paint);
    }

    public virtual void drawOval(Rect rect, Paint paint)
    {
        this._main.drawOval(rect, paint);
        this._screenshot.drawOval(rect, paint);
    }

    public virtual void drawPaint(Paint paint)
    {
        this._main.drawPaint(paint);
        this._screenshot.drawPaint(paint);
    }

    public virtual void drawParagraph(Paragraph paragraph, Offset offset)
    {
        this._main.drawParagraph(paragraph, offset);
        this._screenshot.drawParagraph(paragraph, offset);
    }

    public virtual void drawPath(Path path, Paint paint)
    {
        this._main.drawPath(path, paint);
        this._screenshot.drawPath(path, paint);
    }

    public virtual void drawPicture(Picture picture)
    {
        this._main.drawPicture(picture);
        this._screenshot.drawPicture(picture);
    }

    public virtual void drawPoints(global::Doroti.Ui.PointMode pointMode, List<Offset> points, Paint paint)
    {
        this._main.drawPoints(pointMode, points, paint);
        this._screenshot.drawPoints(pointMode, points, paint);
    }

    public virtual void drawRRect(RRect rrect, Paint paint)
    {
        this._main.drawRRect(rrect, paint);
        this._screenshot.drawRRect(rrect, paint);
    }

    public virtual void drawRawAtlas(global::Doroti.Ui.Image atlas, Float32List rstTransforms, Float32List rects, Int32List? colors, BlendMode? blendMode, Rect? cullRect, Paint paint)
    {
        this._main.drawRawAtlas(atlas, rstTransforms, rects, colors, blendMode, cullRect, paint);
        this._screenshot.drawRawAtlas(atlas, rstTransforms, rects, colors, blendMode, cullRect, paint);
    }

    public virtual void drawRawPoints(global::Doroti.Ui.PointMode pointMode, Float32List points, Paint paint)
    {
        this._main.drawRawPoints(pointMode, points, paint);
        this._screenshot.drawRawPoints(pointMode, points, paint);
    }

    public virtual void drawRect(Rect rect, Paint paint)
    {
        this._main.drawRect(rect, paint);
        this._screenshot.drawRect(rect, paint);
    }

    public virtual void drawShadow(Path path, Color color, double elevation, bool transparentOccluder)
    {
        this._main.drawShadow(path, color, elevation, transparentOccluder);
        this._screenshot.drawShadow(path, color, elevation, transparentOccluder);
    }

    public virtual void drawVertices(global::Doroti.Ui.Vertices vertices, BlendMode blendMode, Paint paint)
    {
        this._main.drawVertices(vertices, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(blendMode)), paint);
        this._screenshot.drawVertices(vertices, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(blendMode)), paint);
    }

    public virtual long getSaveCount()
    {
        return this._main.getSaveCount();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void restore()
    {
        this._main.restore();
        this._screenshot.restore();
    }

    public virtual void rotate(double radians)
    {
        this._main.rotate(radians);
        this._screenshot.rotate(radians);
    }

    public virtual void save()
    {
        this._main.save();
        this._screenshot.save();
    }

    public virtual void saveLayer(Rect? bounds, Paint paint)
    {
        this._main.saveLayer(bounds, paint);
        this._screenshot.saveLayer(bounds, paint);
    }

    public virtual void scale(double sx, double? sy = null)
    {
        this._main.scale(sx, sy);
        this._screenshot.scale(sx, sy);
    }

    public virtual void skew(double sx, double sy)
    {
        this._main.skew(sx, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(sy)));
        this._screenshot.skew(sx, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(sy)));
    }

    public virtual void transform(Float64List matrix4)
    {
        this._main.transform(matrix4);
        this._screenshot.transform(matrix4);
    }

    public virtual void translate(double dx, double dy)
    {
        this._main.translate(dx, dy);
        this._screenshot.translate(dx, dy);
    }

    public virtual dynamic noSuchMethod(global::Doroti.Runtime.Invocation invocation)
    {
        base.noSuchMethod(invocation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static Rect _calculateSubtreeBoundsHelper(global::Doroti.Framework.Rendering.RenderObject @object, Matrix4 transform)
    {
        global::Doroti.Ui.Rect bounds = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform, ((global::Doroti.Framework.Rendering.RenderObject)@object).semanticBounds));
        ((dynamic)@object).visitChildren(((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)((child) =>
        {
            Matrix4 childTransform = transform.clone();
            ((dynamic)@object).applyPaintTransform(child, childTransform);
            global::Doroti.Ui.Rect childBounds = ((global::Doroti.Ui.Rect)(object?)Widget_inspectorLibrary._calculateSubtreeBoundsHelper(child, childTransform));
            global::Doroti.Ui.Rect? paintClip = ((global::Doroti.Ui.Rect?)(object?)((Rect?)((dynamic)@object).describeApproximatePaintClip(child)));
            if ((paintClip is not null))
            {
                Rect paintClip__9652__value9716 = DartRuntimePrimitives.RequireValue(paintClip);
                global::Doroti.Ui.Rect transformedPaintClip = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(transform, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(paintClip__9652__value9716))));
                childBounds = childBounds.intersect(transformedPaintClip);
            }
            if ((childBounds.isFinite && !childBounds.isEmpty))
            {
                bounds = (bounds.isEmpty ? childBounds : bounds.expandToInclude(childBounds));
            }
        })));
        return bounds;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static Rect _calculateSubtreeBounds(global::Doroti.Framework.Rendering.RenderObject @object)
    {
        return Widget_inspectorLibrary._calculateSubtreeBoundsHelper(@object, Matrix4.identity());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ScreenshotContainerLayer__widget_inspector : global::Doroti.Framework.Rendering.OffsetLayer
{
    public override void addToScene(SceneBuilder builder)
    {
        addChildrenToScene(builder);
    }

}

public class _ScreenshotData__widget_inspector
{
    public virtual global::Doroti.Framework.Rendering.RenderObject target { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.OffsetLayer containerLayer { get; private set; } = default!;
    public virtual bool foundTarget { get; set; } = false;
    public virtual bool includeInScreenshot { get; set; } = false;
    public virtual bool includeInRegularContext { get; set; } = true;

    internal _ScreenshotData__widget_inspector(global::Doroti.Framework.Rendering.RenderObject target)
    {
        this.target = target;
        this.containerLayer = new _ScreenshotContainerLayer__widget_inspector();
    }

    public virtual global::Doroti.Ui.Offset screenshotOffset
    {
        get
        {
            DartRuntimePrimitives.Assert(() => this.foundTarget);
            return ((global::Doroti.Framework.Rendering.OffsetLayer)this.containerLayer).offset;
            return default!;
        }
        set
        {
            var offset = value;
            this.containerLayer.offset = offset;
        }
    }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this.containerLayer.dispose();
    }

}

internal class _ScreenshotPaintingContext__widget_inspector : global::Doroti.Framework.Rendering.PaintingContext
{
    internal virtual _ScreenshotData__widget_inspector _data { get; private set; } = default!;
    internal virtual global::Doroti.Framework.Rendering.PictureLayer? _screenshotCurrentLayer { get; set; } = default;
    internal virtual PictureRecorder? _screenshotRecorder { get; set; } = default;
    internal virtual Canvas? _screenshotCanvas { get; set; } = default;
    internal virtual _MulticastCanvas__widget_inspector? _multicastCanvas { get; set; } = default;

    internal _ScreenshotPaintingContext__widget_inspector(global::Doroti.Framework.Rendering.ContainerLayer containerLayer, Rect estimatedBounds, _ScreenshotData__widget_inspector screenshotData) : base(containerLayer, estimatedBounds)
    {
        this._data = screenshotData;
    }

    public override Canvas canvas
    {
        get
        {
            if (((_ScreenshotData__widget_inspector)this._data).includeInScreenshot)
            {
                if ((this._screenshotCanvas is null))
                {
                    _startRecordingScreenshot();
                }
                DartRuntimePrimitives.Assert(() => (this._screenshotCanvas is not null));
                return (((_ScreenshotData__widget_inspector)this._data).includeInRegularContext ? this._multicastCanvas! : this._screenshotCanvas!);
            }
            else
            {
                DartRuntimePrimitives.Assert(() => ((_ScreenshotData__widget_inspector)this._data).includeInRegularContext);
                return base.canvas;
            }
            return default!;
        }
    }
    internal virtual bool _isScreenshotRecording
    {
        get
        {
            var hasScreenshotCanvas = (this._screenshotCanvas is not null);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (hasScreenshotCanvas)
                    {
                        DartRuntimePrimitives.Assert(() => (this._screenshotCurrentLayer is not null));
                        DartRuntimePrimitives.Assert(() => (this._screenshotRecorder is not null));
                        DartRuntimePrimitives.Assert(() => (this._screenshotCanvas is not null));
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => (this._screenshotCurrentLayer is null));
                        DartRuntimePrimitives.Assert(() => (this._screenshotRecorder is null));
                        DartRuntimePrimitives.Assert(() => (this._screenshotCanvas is null));
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return hasScreenshotCanvas;
            return default!;
        }
    }
    internal virtual void _startRecordingScreenshot()
    {
        DartRuntimePrimitives.Assert(() => ((_ScreenshotData__widget_inspector)this._data).includeInScreenshot);
        DartRuntimePrimitives.Assert(() => !this._isScreenshotRecording);
        _screenshotCurrentLayer = new global::Doroti.Framework.Rendering.PictureLayer(this.estimatedBounds);
        _screenshotRecorder = new global::Doroti.Ui.PictureRecorder();
        _screenshotCanvas = new global::Doroti.Ui.Canvas(this._screenshotRecorder!);
        ((_ScreenshotData__widget_inspector)this._data).containerLayer.append(this._screenshotCurrentLayer!);
        if (((_ScreenshotData__widget_inspector)this._data).includeInRegularContext)
        {
            _multicastCanvas = new _MulticastCanvas__widget_inspector(main: base.canvas, screenshot: this._screenshotCanvas!);
        }
        else
        {
            _multicastCanvas = null;
        }
    }

    public override void stopRecordingIfNeeded()
    {
        base.stopRecordingIfNeeded();
        _stopRecordingScreenshotIfNeeded();
    }

    internal virtual void _stopRecordingScreenshotIfNeeded()
    {
        if (!this._isScreenshotRecording)
        {
            return;
        }
        this._screenshotCurrentLayer!.picture = this._screenshotRecorder!.endRecording();
        _screenshotCurrentLayer = null;
        _screenshotRecorder = null;
        _multicastCanvas = null;
        _screenshotCanvas = null;
    }

    public override void appendLayer(global::Doroti.Framework.Rendering.Layer layer)
    {
        if (((_ScreenshotData__widget_inspector)this._data).includeInRegularContext)
        {
            base.appendLayer(layer);
            if (((_ScreenshotData__widget_inspector)this._data).includeInScreenshot)
            {
                DartRuntimePrimitives.Assert(() => !this._isScreenshotRecording);
                ((_ScreenshotData__widget_inspector)this._data).containerLayer.append(new _ProxyLayer__widget_inspector(layer));
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !this._isScreenshotRecording);
            DartRuntimePrimitives.Assert(() => ((_ScreenshotData__widget_inspector)this._data).includeInScreenshot);
            layer.remove();
            ((_ScreenshotData__widget_inspector)this._data).containerLayer.append(layer);
            return;
        }
    }

    public override global::Doroti.Framework.Rendering.PaintingContext createChildContext(global::Doroti.Framework.Rendering.ContainerLayer childLayer, Rect bounds)
    {
        if (((_ScreenshotData__widget_inspector)this._data).foundTarget)
        {
            return ((global::Doroti.Framework.Rendering.PaintingContext)(object?)base.createChildContext(childLayer, bounds));
        }
        else
        {
            return ((global::Doroti.Framework.Rendering.PaintingContext)(object?)new _ScreenshotPaintingContext__widget_inspector(containerLayer: childLayer, estimatedBounds: bounds, screenshotData: this._data));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintChild(global::Doroti.Framework.Rendering.RenderObject child, Offset offset)
    {
        bool isScreenshotTarget = DartRuntimePrimitives.Identical(child, ((_ScreenshotData__widget_inspector)this._data).target);
        if (isScreenshotTarget)
        {
            DartRuntimePrimitives.Assert(() => !((_ScreenshotData__widget_inspector)this._data).includeInScreenshot);
            DartRuntimePrimitives.Assert(() => !((_ScreenshotData__widget_inspector)this._data).foundTarget);
            this._data.foundTarget = true;
            this._data.screenshotOffset = offset;
            this._data.includeInScreenshot = true;
        }
        base.paintChild(child, offset);
        if (isScreenshotTarget)
        {
            _stopRecordingScreenshotIfNeeded();
            this._data.includeInScreenshot = false;
        }
    }

    public static async Future<global::Doroti.Ui.Image> toImage(global::Doroti.Framework.Rendering.RenderObject renderObject, Rect renderBounds, double pixelRatio = 1.0, bool debugPaint = false)
    {
        var repaintBoundary = renderObject;
        while (!((global::Doroti.Framework.Rendering.RenderObject)repaintBoundary).isRepaintBoundary)
        {
            repaintBoundary = ((global::Doroti.Framework.Rendering.RenderObject)repaintBoundary).parent!;
        }
        var data = new _ScreenshotData__widget_inspector(target: renderObject);
        var context = new _ScreenshotPaintingContext__widget_inspector(containerLayer: ((global::Doroti.Framework.Rendering.RenderObject)repaintBoundary).debugLayer!, estimatedBounds: ((global::Doroti.Framework.Rendering.RenderObject)repaintBoundary).paintBounds, screenshotData: data);
        if (DartRuntimePrimitives.Identical(renderObject, repaintBoundary))
        {
            ((_ScreenshotData__widget_inspector)data).containerLayer.append(new _ProxyLayer__widget_inspector(((global::Doroti.Framework.Rendering.RenderObject)repaintBoundary).debugLayer!));
            data.foundTarget = true;
            var offsetLayer = ((global::Doroti.Framework.Rendering.OffsetLayer?)(object?)((global::Doroti.Framework.Rendering.RenderObject)repaintBoundary).debugLayer!)!;
            data.screenshotOffset = ((global::Doroti.Framework.Rendering.OffsetLayer)offsetLayer).offset;
        }
        else
        {
            PaintingContext.debugInstrumentRepaintCompositedChild(repaintBoundary, customContext: context);
        }
        if ((debugPaint && !global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled))
        {
            data.includeInRegularContext = false;
            context.stopRecordingIfNeeded();
            DartRuntimePrimitives.Assert(() => ((_ScreenshotData__widget_inspector)data).foundTarget);
            data.includeInScreenshot = true;
            global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled = true;
            try
            {
                ((dynamic)renderObject).debugPaint(context, ((_ScreenshotData__widget_inspector)data).screenshotOffset);
            }
            finally
            {
                global::Doroti.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled = false;
                context.stopRecordingIfNeeded();
            }
        }
        ((global::Doroti.Framework.Rendering.RenderObject)repaintBoundary).debugLayer!.buildScene(new global::Doroti.Ui.SceneBuilder());
        global::Doroti.Ui.Image image = default!;
        try
        {
            image = await ((_ScreenshotData__widget_inspector)data).containerLayer.toImage(renderBounds, pixelRatio: pixelRatio);
        }
        finally
        {
            data.dispose();
        }
        return image;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _DiagnosticsPathNode__widget_inspector
{
    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode node { get; private set; } = default!;
    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> children { get; private set; } = default!;
    public virtual long? childIndex { get; private set; }

    internal _DiagnosticsPathNode__widget_inspector(global::Doroti.Framework.Foundation.DiagnosticsNode node, List<global::Doroti.Framework.Foundation.DiagnosticsNode> children, long? childIndex = null)
    {
        this.node = node;
        this.children = children;
        this.childIndex = childIndex;
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static List<_DiagnosticsPathNode__widget_inspector>? _followDiagnosticableChain(List<global::Doroti.Framework.Foundation.Diagnosticable> chain)
    {
        var path = new List<_DiagnosticsPathNode__widget_inspector>();
        if (!System.Linq.Enumerable.Any(chain))
        {
            return path;
        }
        global::Doroti.Framework.Foundation.DiagnosticsNode diagnostic = ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object?)((Diagnosticable)chain.First()).toDiagnosticsNode());
        for (var i = 1L; (i < checked((long)(chain.Count))); i += 1L)
        {
            global::Doroti.Framework.Foundation.Diagnosticable target = chain[(int)(i)];
            var foundMatch = false;
            List<global::Doroti.Framework.Foundation.DiagnosticsNode> childrenLocal = ((List<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)diagnostic.getChildren());
            for (var j = 0L; (j < checked((long)(childrenLocal.Count))); j += 1L)
            {
                global::Doroti.Framework.Foundation.DiagnosticsNode child = childrenLocal[(int)(j)];
                if ((object.Equals(((global::Doroti.Framework.Foundation.DiagnosticsNode)child).value, target)))
                {
                    foundMatch = true;
                    path.Add(new _DiagnosticsPathNode__widget_inspector(node: diagnostic, children: childrenLocal, childIndex: j));
                    diagnostic = child;
                    break;
                }
            }
            DartRuntimePrimitives.Assert(() => foundMatch);
        }
        path.Add(new _DiagnosticsPathNode__widget_inspector(node: diagnostic, children: diagnostic.getChildren().ToList()));
        return path;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate void InspectorSelectionChangedCallback();

public class InspectorReferenceData
{
    internal virtual WeakReference<object>? _ref { get; set; } = default;
    internal virtual object? _value { get; set; } = default;
    public virtual string id { get; private set; } = default!;
    public virtual long count { get; set; } = 1L;

    public InspectorReferenceData(object @object, string id)
    {
        this.id = id;
    }

    public virtual object? value => (DartCoreExtensions.weakTarget(this._ref) ?? this._value);
}

internal class _WidgetInspectorService__widget_inspector : WidgetInspectorService
{
    public virtual List<string?> _serializeRing { get; set; } = new List<string?>(System.Linq.Enumerable.Repeat<string?>(null, checked((int)20L)));
    public virtual long _serializeRingIndex { get; set; } = 0L;
    public virtual InspectorSelection selection { get; set; } = new InspectorSelection();
    public virtual global::System.Action? selectionChangedCallback { get; set; } = default;
    public virtual DartMap<string, HashSet<InspectorReferenceData>> _groups { get; set; } = new DartMap<string, HashSet<InspectorReferenceData>>();
    public virtual DartMap<string, InspectorReferenceData> _idToReferenceData { get; set; } = new DartMap<string, InspectorReferenceData>();
    public virtual WeakMap<object, string> _objectToId { get; set; } = new WeakMap<object, string>();
    public virtual long _nextId { get; set; } = 0L;
    public virtual List<string>? _pubRootDirectories { get; set; } = default;
    public virtual DartMap<string, bool> _isLocalCreationCache { get; set; } = new DartMap<string, bool>();
    public virtual bool _trackRebuildDirtyWidgets { get; set; } = false;
    public virtual bool _trackRepaintWidgets { get; set; } = false;
    public virtual long _errorsSinceReload { get; set; } = 0L;
    public virtual bool? _widgetCreationTracked { get; set; } = default;
    public virtual Duration _frameStart { get; set; } = default!;
    public virtual long _frameNumber { get; set; } = default!;
    public virtual _ElementLocationStatsTracker__widget_inspector _rebuildStats { get; set; } = new _ElementLocationStatsTracker__widget_inspector();
    public virtual _ElementLocationStatsTracker__widget_inspector _repaintStats { get; set; } = new _ElementLocationStatsTracker__widget_inspector();

    internal _WidgetInspectorService__widget_inspector()
    {
    }

    public virtual bool isSelectMode
    {
        set
        {
            var enabled = value;
            _changeWidgetSelectionMode(enabled);
        }
    }
    public virtual void registerServiceExtension(string name, global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>> callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerExtension(name: $"inspector.{name}", callback: callback);
    }

    public virtual void _registerSignalServiceExtension(string name, dynamic callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
        {
            return new DartMap<string, object> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback()) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual void _registerObjectGroupServiceExtension(string name, dynamic callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
        {
            return new DartMap<string, object> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(parameters.GetValueOrDefault("objectGroup")!)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual void _registerBoolServiceExtension(string name, global::System.Func<Future<bool>> getter, global::System.Func<bool, Future> setter, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
        {
            if (parameters.ContainsKey("enabled"))
            {
                var value = (parameters.GetValueOrDefault("enabled") == "true");
                await setter(DartRuntimePrimitives.RequireValue(value));
                _postExtensionStateChangedEvent(name, DartRuntimePrimitives.RequireValue(value));
            }
            return new DartMap<string, object> { ["enabled"] = (await getter() ? "true" : "false") };
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual void _postExtensionStateChangedEvent(string name, object? value)
    {
        postEvent("Flutter.ServiceExtensionStateChanged", new DartMap<string, object> { ["extension"] = $"ext.flutter.inspector.{name}", ["value"] = value }.cast<object, object>());
    }

    public virtual void _registerServiceExtensionWithArg(string name, dynamic callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
        {
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("objectGroup"));
            return new DartMap<string, object> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(parameters.GetValueOrDefault("arg"), parameters.GetValueOrDefault("objectGroup")!)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual void _registerServiceExtensionVarArgs(string name, global::System.Func<List<string>, object> callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
        {
            long index = default!;
            var args = new List<string>();
            DartRuntimePrimitives.Assert(() => ((index == checked((long)(parameters.Count))) || (((index == (checked((long)(parameters.Count)) - 1L)) && parameters.ContainsKey("isolateId")))));
            return new DartMap<string, object> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(args)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual Future forceRebuild()
    {
        WidgetsBinding binding = WidgetsBinding.instance;
        if ((((WidgetsBinding)binding).rootElement is not null))
        {
            ((WidgetsBinding)binding).buildOwner!.reassemble(((WidgetsBinding)binding).rootElement!);
            return binding.endOfFrame;
        }
        return Future.value();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reportStructuredError(global::Doroti.Framework.Foundation.FlutterErrorDetails details)
    {
        DartMap<string, object?> errorJson = _nodeToJson(((Diagnosticable)details).toDiagnosticsNode(), new InspectorSerializationDelegate(groupName: WidgetInspectorService._consoleObjectGroup, subtreeDepth: 5L, includeProperties: true, maxDescendantsTruncatableNode: 5L, service: this))!.cast<string, object?>();
        errorJson["errorsSinceReload"] = this._errorsSinceReload;
        if ((this._errorsSinceReload == 0L))
        {
            errorJson["renderedErrorText"] = new global::Doroti.Framework.Foundation.TextTreeRenderer(wrapWidthProperties: global::Doroti.Framework.Foundation.FlutterError.wrapWidth, maxDescendentsTruncatableNode: 5L).render(((Diagnosticable)details).toDiagnosticsNode(style: global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.error)).trimRight();
        }
        else
        {
            errorJson["renderedErrorText"] = $"Another exception was thrown: {(((global::Doroti.Framework.Foundation.FlutterErrorDetails)details).summary)}";
        }
        this._errorsSinceReload += 1L;
        postEvent("Flutter.Error", errorJson.cast<object, object>());
    }

    public virtual void _resetErrorCount()
    {
        this._errorsSinceReload = 0L;
    }

    public virtual bool isStructuredErrorsEnabled()
    {
        var enabled = false;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return enabled;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void initServiceExtensions(RegisterServiceExtensionCallback registerExtension)
    {
        global::Doroti.Framework.Foundation.FlutterExceptionHandler defaultExceptionHandler = ((global::Doroti.Framework.Foundation.FlutterExceptionHandler)(object?)global::Doroti.Framework.Foundation.FlutterError.presentError);
        if (isStructuredErrorsEnabled())
        {
            global::Doroti.Framework.Foundation.FlutterError.presentError = this._reportStructuredError;
        }
        DartRuntimePrimitives.Assert(() => !WidgetInspectorService._debugServiceExtensionsRegistered);
        DartRuntimePrimitives.Assert(() =>
            {
                WidgetInspectorService._debugServiceExtensionsRegistered = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPersistentFrameCallback((global::System.Action<Duration>)this._onFrameStart);
        _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.structuredErrors.ToString(), getter: ((global::System.Func<Future<bool>>)(async () => (object.Equals((global::Doroti.Framework.Foundation.FlutterExceptionHandler)global::Doroti.Framework.Foundation.FlutterError.presentError, (global::Doroti.Framework.Foundation.FlutterExceptionHandler)this._reportStructuredError)))), setter: ((global::System.Func<bool, Future>)((value) =>
        {
            global::Doroti.Framework.Foundation.FlutterError.presentError = ((value ? (global::Doroti.Framework.Foundation.FlutterExceptionHandler)this._reportStructuredError : defaultExceptionHandler));
            return Future.value();
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.show.ToString(), getter: ((global::System.Func<Future<bool>>)(async () => WidgetsBinding.instance.debugShowWidgetInspectorOverride)), setter: ((global::System.Func<bool, Future>)((value) =>
        {
            if ((WidgetsBinding.instance.debugShowWidgetInspectorOverride != value))
            {
                _changeWidgetSelectionMode(DartRuntimePrimitives.RequireValue(value), notifyStateChange: false);
            }
            return Future.value();
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        if (isWidgetCreationTracked())
        {
            _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.trackRebuildDirtyWidgets.ToString(), getter: ((global::System.Func<Future<bool>>)(async () => this._trackRebuildDirtyWidgets)), setter: ((global::System.Func<bool, Future>)(async (value) =>
            {
                if ((value == this._trackRebuildDirtyWidgets))
                {
                    return;
                }
                this._rebuildStats.resetCounts();
                this._trackRebuildDirtyWidgets = value;
                if (value)
                {
                    DartRuntimePrimitives.Assert(() => (global::Doroti.Framework.Widgets.DebugLibrary.debugOnRebuildDirtyWidget is null));
                    global::Doroti.Framework.Widgets.DebugLibrary.debugOnRebuildDirtyWidget = this._onRebuildWidget;
                    await forceRebuild();
                    return;
                }
                else
                {
                    global::Doroti.Framework.Widgets.DebugLibrary.debugOnRebuildDirtyWidget = null;
                    return;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
            _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.widgetLocationIdMap.ToString(), callback: ((global::System.Func<object>)(() =>
            {
                return Widget_inspectorLibrary._locationIdMapToJson();
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
            _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.trackRepaintWidgets.ToString(), getter: ((global::System.Func<Future<bool>>)(async () => this._trackRepaintWidgets)), setter: ((global::System.Func<bool, Future>)(async (value) =>
            {
                if ((value == this._trackRepaintWidgets))
                {
                    return;
                }
                this._repaintStats.resetCounts();
                this._trackRepaintWidgets = value;
                if (value)
                {
                    DartRuntimePrimitives.Assert(() => (global::Doroti.Framework.Rendering.DebugLibrary.debugOnProfilePaint is null));
                    global::Doroti.Framework.Rendering.DebugLibrary.debugOnProfilePaint = this._onPaint;
                    void markTreeNeedsPaint(global::Doroti.Framework.Rendering.RenderObject renderObject)
                    {
                        ((dynamic)renderObject).markNeedsPaint();
                        ((dynamic)renderObject).visitChildren((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)markTreeNeedsPaint);
                    }
                    global::Doroti.Framework.Rendering.RendererBinding.instance.renderViews.forEach((__arg0) => ((global::System.Action<global::Doroti.Framework.Rendering.RenderObject>)markTreeNeedsPaint)(DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.RenderObject>(__arg0)));
                }
                else
                {
                    global::Doroti.Framework.Rendering.DebugLibrary.debugOnProfilePaint = null;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        }
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.disposeAllGroups.ToString(), callback: ((global::System.Func<object>)(() =>
        {
            disposeAllGroups();
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerObjectGroupServiceExtension(name: WidgetInspectorServiceExtensions.disposeGroup.ToString(), callback: ((global::System.Func<string, object>)((name) =>
        {
            disposeGroup(name);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.isWidgetTreeReady.ToString(), callback: (global::System.Func<string?, bool>)this.isWidgetTreeReady, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.disposeId.ToString(), callback: ((global::System.Func<string?, string, object>)((objectId, objectGroup) =>
        {
            disposeId(objectId, objectGroup);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.setPubRootDirectories.ToString(), callback: ((global::System.Func<List<string>, object>)((args) =>
        {
            setPubRootDirectories(args);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.addPubRootDirectories.ToString(), callback: ((global::System.Func<List<string>, object>)((args) =>
        {
            addPubRootDirectories(args);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.removePubRootDirectories.ToString(), callback: ((global::System.Func<List<string>, object>)((args) =>
        {
            removePubRootDirectories(args);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getPubRootDirectories.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this.pubRootDirectories, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.setSelectionById.ToString(), callback: (global::System.Func<string?, string?, bool>)this.setSelectionById, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getParentChain.ToString(), callback: (global::System.Func<string?, string, List<object>>)this._getParentChain, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getProperties.ToString(), callback: (global::System.Func<string?, string, List<object>>)this._getProperties, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getChildren.ToString(), callback: (global::System.Func<string?, string, List<object>>)this._getChildren, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getChildrenSummaryTree.ToString(), callback: (global::System.Func<string?, string, List<object>>)this._getChildrenSummaryTree, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getChildrenDetailsSubtree.ToString(), callback: (global::System.Func<string?, string, List<object>>)this._getChildrenDetailsSubtree, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerObjectGroupServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidget.ToString(), callback: (global::System.Func<string, DartMap<string, object>?>)this._getRootWidget, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerObjectGroupServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetSummaryTree.ToString(), callback: (global::System.Func<string, global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>?, DartMap<string, object>?>)this._getRootWidgetSummaryTree, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetSummaryTreeWithPreviews.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this._getRootWidgetSummaryTreeWithPreviews, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetTree.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this._getRootWidgetTree, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getDetailsSubtree.ToString(), callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
        {
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("objectGroup"));
            string? subtreeDepth = parameters.GetValueOrDefault("subtreeDepth");
            return new DartMap<string, object> { ["result"] = _getDetailsSubtree(parameters.GetValueOrDefault("arg"), parameters.GetValueOrDefault("objectGroup"), ((subtreeDepth is not null) ? Dart_coreLibrary.parse(subtreeDepth) : 2L)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getSelectedWidget.ToString(), callback: (global::System.Func<string?, string, DartMap<string, object>?>)this._getSelectedWidget, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getSelectedSummaryWidget.ToString(), callback: (global::System.Func<string?, string, DartMap<string, object>?>)this._getSelectedSummaryWidget, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.isWidgetCreationTracked.ToString(), callback: (global::System.Func<bool>)this.isWidgetCreationTracked, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.screenshot.ToString(), callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) =>
        {
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("id"));
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("width"));
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("height"));
            global::Doroti.Ui.Image? image = await screenshot(toObject(parameters.GetValueOrDefault("id")), width: Dart_coreLibrary.parse(parameters.GetValueOrDefault("width")!), height: Dart_coreLibrary.parse(parameters.GetValueOrDefault("height")!), margin: (parameters.ContainsKey("margin") ? Dart_coreLibrary.parse(parameters.GetValueOrDefault("margin")!) : 0.0), maxPixelRatio: (parameters.ContainsKey("maxPixelRatio") ? Dart_coreLibrary.parse(parameters.GetValueOrDefault("maxPixelRatio")!) : 1.0), debugPaint: (parameters.GetValueOrDefault("debugPaint") == "true"));
            if ((image is null))
            {
                return new DartMap<string, object> { ["result"] = null };
            }
            ByteData? byteData = await image.toByteData(format: ImageByteFormat.png);
            image.dispose();
            return new DartMap<string, object> { ["result"] = global::Doroti.Runtime.Dart_convertLibrary.base64.encoder.convert(new Uint8List(byteData!.buffer)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        })), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getLayoutExplorerNode.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this._getLayoutExplorerNode, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.setFlexFit.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this._setFlexFit, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.setFlexFactor.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this._setFlexFactor, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.setFlexProperties.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this._setFlexProperties, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual void _clearStats()
    {
        this._rebuildStats.resetCounts();
        this._repaintStats.resetCounts();
    }

    public virtual void disposeAllGroups()
    {
        this._groups.Clear();
        this._idToReferenceData.Clear();
        this._objectToId.clear();
        this._nextId = 0L;
    }

    public virtual void resetAllState()
    {
        disposeAllGroups();
        this.selection.clear();
        resetPubRootDirectories();
    }

    public virtual void disposeGroup(string name)
    {
        HashSet<InspectorReferenceData>? references = this._groups.remove(name);
        if ((references is null))
        {
            return;
        }
        references.forEach((__arg0) => ((global::System.Action<InspectorReferenceData>)this._decrementReferenceCount)(__arg0));
    }

    public virtual void _decrementReferenceCount(InspectorReferenceData reference)
    {
        reference.count -= 1L;
        DartRuntimePrimitives.Assert(() => (((InspectorReferenceData)reference).count >= 0L));
        if ((((InspectorReferenceData)reference).count == 0L))
        {
            object? valueLocal = ((InspectorReferenceData)reference).value;
            if ((valueLocal is not null))
            {
                this._objectToId.remove(valueLocal);
            }
            this._idToReferenceData.remove(((InspectorReferenceData)reference).id);
        }
    }

    public virtual string? toId(object? @object, string groupName)
    {
        if ((@object is null))
        {
            return ((string)(object)null);
        }
        HashSet<InspectorReferenceData> @group = this._groups.putIfAbsent(groupName, (() => new HashSet<InspectorReferenceData>()));
        string? id = this._objectToId[@object];
        InspectorReferenceData referenceData = default!;
        if ((id is null))
        {
            id = $"inspector-{this._nextId}";
            this._nextId += 1L;
            this._objectToId[@object] = id;
            referenceData = new InspectorReferenceData(@object, id);
            this._idToReferenceData[id] = referenceData;
            @group.Add(referenceData);
        }
        else
        {
            referenceData = this._idToReferenceData.GetValueOrDefault(id)!;
            if (@group.Add(referenceData))
            {
                referenceData.count += 1L;
            }
        }
        return id;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isWidgetTreeReady(string? groupName = null)
    {
        return WidgetsBinding.instance.debugDidSendFirstFrameEvent;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object? toObject(string? id, string? groupName = null)
    {
        if ((id is null))
        {
            return null;
        }
        InspectorReferenceData? data = this._idToReferenceData.GetValueOrDefault(id);
        if ((data is null))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Id does not exist.") }));
        }
        return ((InspectorReferenceData)data).value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object? toObjectForSourceLocation(string id, string? groupName = null)
    {
        object? @object = toObject(id);
        if ((@object is Element))
        {
            Element @object__51282__as51313 = (Element)@object;
            return ((Element)((Element)@object__51282__as51313)).widget;
        }
        return @object;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void disposeId(string? id, string groupName)
    {
        if ((id is null))
        {
            return;
        }
        InspectorReferenceData? referenceData = this._idToReferenceData.GetValueOrDefault(id);
        if ((referenceData is null))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Id does not exist") }));
        }
        if ((this._groups.GetValueOrDefault(groupName)?.Remove(referenceData) != true))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary("Id is not in group") }));
        }
        _decrementReferenceCount(referenceData);
    }

    public virtual void setPubRootDirectories(List<string> pubRootDirectories)
    {
        addPubRootDirectories(pubRootDirectories);
    }

    public virtual void resetPubRootDirectories()
    {
        this._pubRootDirectories = new List<string>();
        this._isLocalCreationCache.clear();
    }

    public virtual void addPubRootDirectories(List<string> pubRootDirectories)
    {
        pubRootDirectories = pubRootDirectories.map<string, string>(((directory) => DartUri.parse(directory).path)).ToList();
        var directorySet = new HashSet<string>(pubRootDirectories);
        if ((this._pubRootDirectories is not null))
        {
            directorySet.UnionWith(this._pubRootDirectories!.Cast<string>());
        }
        this._pubRootDirectories = directorySet.ToList();
        this._isLocalCreationCache.clear();
    }

    public virtual void removePubRootDirectories(List<string> pubRootDirectories)
    {
        if ((this._pubRootDirectories is null))
        {
            return;
        }
        pubRootDirectories = pubRootDirectories.map<string, string>(((directory) => DartUri.parse(directory).path)).ToList();
        var directorySet = new HashSet<string>(this._pubRootDirectories!);
        directorySet.removeAll(pubRootDirectories);
        this._pubRootDirectories = directorySet.ToList();
        this._isLocalCreationCache.clear();
    }

    public virtual Future<DartMap<string, object>> pubRootDirectories(DartMap<string, string> parameters)
    {
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = (this._pubRootDirectories ?? new List<string>()) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool setSelectionById(string? id, string? groupName = null)
    {
        return setSelection(toObject(id), groupName);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool setSelection(object? @object, string? groupName = null)
    {
        switch (@object)
        {
            case Element __object55868 when ((!object.Equals(@object, ((InspectorSelection)this.selection).currentElement))):
                {
                    this.selection.clearCandidates();
                    this.selection.currentElement = (Element)@object;
                    _notifyToolsOfSelection(((InspectorSelection)this.selection).currentElement);
                    return true;
                }
            case global::Doroti.Framework.Rendering.RenderObject __object56090 when ((!object.Equals(@object, ((InspectorSelection)this.selection).current))):
                {
                    this.selection.clearCandidates();
                    this.selection.current = (global::Doroti.Framework.Rendering.RenderObject)@object;
                    _notifyToolsOfSelection(((InspectorSelection)this.selection).current);
                    return true;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _notifyToolsOfSelection(object? @object, bool restrictToProjectFiles = false)
    {
        inspect(@object);
        global::Doroti.Runtime.CreationLocation? location = ((global::Doroti.Runtime.CreationLocation?)(object?)_getSelectedWidgetLocation(restrictToSummaryTree: restrictToProjectFiles));
        if ((location is not null))
        {
            postEvent("navigate", new DartMap<string, object> { ["fileUri"] = ((object)((dynamic)location).file), ["line"] = ((object)((dynamic)location).line), ["column"] = ((object)((dynamic)location).column), ["source"] = "flutter.inspector" }.cast<object, object>(), stream: "ToolEvent");
        }
    }

    public virtual void _changeWidgetSelectionMode(bool enabled, bool notifyStateChange = true)
    {
        WidgetsBinding.instance.debugShowWidgetInspectorOverride = enabled;
        if (notifyStateChange)
        {
            _postExtensionStateChangedEvent(WidgetInspectorServiceExtensions.show.ToString(), enabled);
        }
        if (!enabled)
        {
            this.selection.currentElement = null;
        }
    }

    public virtual string? _devToolsInspectorUriForElement(Element element)
    {
        if (((global::Doroti.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress is not null) && (global::Doroti.Framework.Foundation.DebugLibrary.connectedVmServiceUri is not null)))
        {
            string? inspectorRef = ((string?)(object?)toId(element, WidgetInspectorService._consoleObjectGroup));
            if ((inspectorRef is not null))
            {
                return ((string?)(object?)devToolsInspectorUri(inspectorRef));
            }
        }
        return ((string)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string devToolsInspectorUri(string inspectorRef)
    {
        DartRuntimePrimitives.Assert(() => (global::Doroti.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress is not null));
        DartRuntimePrimitives.Assert(() => (global::Doroti.Framework.Foundation.DebugLibrary.connectedVmServiceUri is not null));
        DartUri uri = DartUri.parse(global::Doroti.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress!.ToString()).replace(queryParameters: new DartMap<string, string> { ["uri"] = global::Doroti.Framework.Foundation.DebugLibrary.connectedVmServiceUri.ToString(), ["inspectorRef"] = inspectorRef });
        var devToolsInspectorUriLocal = uri.ToString();
        long startQueryParamIndex = ((long)((dynamic)devToolsInspectorUriLocal).IndexOf("?"));
        DartRuntimePrimitives.Assert(() => (startQueryParamIndex != -1L));
        return $"{devToolsInspectorUriLocal.substring(0L, startQueryParamIndex)}" + "/#/inspector" + $"{devToolsInspectorUriLocal.substring(startQueryParamIndex)}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getParentChain(string id, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getParentChain(id, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getParentChain(string? id, string groupName)
    {
        object? value = toObject(id);
        List<_DiagnosticsPathNode__widget_inspector> path = (value switch { global::Doroti.Framework.Rendering.RenderObject __object60383 => _getRenderObjectParentChain(((global::Doroti.Framework.Rendering.RenderObject)__object60383), groupName)!, Element __object60455 => _getElementParentChain(((Element)__object60455), groupName), _ => throw DartRuntimePrimitives.AsException(new global::Doroti.Framework.Foundation.FlutterError(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorSummary($"Cannot get parent chain for node of type {DartRuntimePrimitives.RuntimeType(value)}") })) }).ToList();
        InspectorSerializationDelegate createDelegate()
        {
            return new InspectorSerializationDelegate(groupName: groupName, service: this);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return new List<object?>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<Element> _getRawElementParentChain(Element element, long? numLocalParents)
    {
        List<Element> elements = ((List<Element>)(object?)element.debugGetDiagnosticChain());
        if ((numLocalParents is not null))
        {
            for (var i = 0L; (i < checked((long)(elements.Count))); i += 1L)
            {
                if (_isValueCreatedByLocalProject(elements[(int)(i)]))
                {
                    numLocalParents = (DartRuntimePrimitives.RequireValue(numLocalParents) - 1L);
                    if ((numLocalParents <= 0L))
                    {
                        elements = elements.take((i + 1L)).ToList();
                        break;
                    }
                }
            }
        }
        return System.Linq.Enumerable.Reverse(elements).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_DiagnosticsPathNode__widget_inspector> _getElementParentChain(Element element, string groupName, long? numLocalParents = null)
    {
        return (Widget_inspectorLibrary._followDiagnosticableChain(_getRawElementParentChain(element, numLocalParents: numLocalParents).Cast<global::Doroti.Framework.Foundation.Diagnosticable>().ToList()) ?? new List<_DiagnosticsPathNode__widget_inspector>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_DiagnosticsPathNode__widget_inspector>? _getRenderObjectParentChain(global::Doroti.Framework.Rendering.RenderObject? renderObject, string groupName)
    {
        var chain = new List<global::Doroti.Framework.Rendering.RenderObject>();
        while ((renderObject is not null))
        {
            chain.Add(renderObject);
            renderObject = ((global::Doroti.Framework.Rendering.RenderObject)renderObject).parent;
        }
        return Widget_inspectorLibrary._followDiagnosticableChain(System.Linq.Enumerable.Reverse(chain).ToList().Cast<global::Doroti.Framework.Foundation.Diagnosticable>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _nodeToJson(global::Doroti.Framework.Foundation.DiagnosticsNode? node, InspectorSerializationDelegate @delegate, bool fullDetails = true)
    {
        if (fullDetails)
        {
            return ((DartMap<string, object>?)(object?)node?.toJsonMap(@delegate));
        }
        else
        {
            return ((DartMap<string, object>?)(object?)node?.toJsonMapIterative(@delegate));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isValueCreatedByLocalProject(object? value)
    {
        global::Doroti.Runtime.CreationLocation? creationLocation = ((global::Doroti.Runtime.CreationLocation?)(object?)Widget_inspectorLibrary._getCreationLocation(value));
        if ((creationLocation is null))
        {
            return false;
        }
        return _isLocalCreationLocation(((string)(object)((object)((dynamic)creationLocation).file)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isLocalCreationLocationImpl(string locationUri)
    {
        string @file = DartUri.parse(locationUri).path;
        if ((this._pubRootDirectories is null))
        {
            return !@file.contains("packages/flutter/");
        }
        foreach (string directory in this._pubRootDirectories!)
        {
            if (@file.startsWith(directory))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isLocalCreationLocation(string locationUri)
    {
        bool? cachedValue = DartCollectionRuntime.NullableMapValue<bool>(this._isLocalCreationCache, locationUri);
        if ((cachedValue is not null))
        {
            bool cachedValue__63933__value63991 = DartRuntimePrimitives.RequireValue(cachedValue);
            return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(cachedValue__63933__value63991));
        }
        bool result = _isLocalCreationLocationImpl(locationUri);
        this._isLocalCreationCache[locationUri] = result;
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string _safeJsonEncode(object? @object)
    {
        string jsonString = global::Doroti.Runtime.Dart_convertLibrary.json.encode(@object);
        this._serializeRing[(int)(this._serializeRingIndex)] = jsonString;
        this._serializeRingIndex = (((this._serializeRingIndex + 1L)) % checked((long)(this._serializeRing.Count)));
        return jsonString;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> _truncateNodes(IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, long maxDescendentsTruncatableNode)
    {
        if ((nodes.All(((node) => (((global::Doroti.Framework.Foundation.DiagnosticsNode)node).value is Element))) && isWidgetCreationTracked()))
        {
            List<global::Doroti.Framework.Foundation.DiagnosticsNode> localNodes = nodes.where(((node) => _isValueCreatedByLocalProject(((global::Doroti.Framework.Foundation.DiagnosticsNode)node).value))).ToList().ToList();
            if (System.Linq.Enumerable.Any(localNodes))
            {
                return localNodes;
            }
        }
        return nodes.take(maxDescendentsTruncatableNode).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DartMap<string, object>> _nodesToJson(List<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate, global::Doroti.Framework.Foundation.DiagnosticsNode? parent)
    {
        return ((List<DartMap<string, object>>)(object?)DiagnosticsNode.toJsonList(nodes, parent, @delegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getProperties(string diagnosticsNodeId, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getProperties(diagnosticsNodeId, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getProperties(string? diagnosticableId, string groupName)
    {
        global::Doroti.Framework.Foundation.DiagnosticsNode? node = ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        if ((node is null))
        {
            return new List<object>();
        }
        return ((List<object>)(object?)_nodesToJson(node.getProperties().ToList(), new InspectorSerializationDelegate(groupName: groupName, service: this), parent: node));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildren(string diagnosticsNodeId, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getChildren(diagnosticsNodeId, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildren(string? diagnosticsNodeId, string groupName)
    {
        var node = ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)toObject(diagnosticsNodeId))!;
        var @delegate = new InspectorSerializationDelegate(groupName: groupName, service: this);
        return ((List<object>)(object?)_nodesToJson(((node is null) ? new List<global::Doroti.Framework.Foundation.DiagnosticsNode>() : _getChildrenFiltered(node, @delegate)), @delegate, parent: node));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildrenSummaryTree(string diagnosticsNodeId, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getChildrenSummaryTree(diagnosticsNodeId, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode? _idToDiagnosticsNode(string? diagnosticableId)
    {
        object? @object = toObject(diagnosticableId);
        return ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)WidgetInspectorService.objectToDiagnosticsNode(@object));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildrenSummaryTree(string? diagnosticableId, string groupName)
    {
        global::Doroti.Framework.Foundation.DiagnosticsNode? node = ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        if ((node is null))
        {
            return new List<object>();
        }
        var @delegate = new InspectorSerializationDelegate(groupName: groupName, summaryTree: true, service: this);
        return ((List<object>)(object?)_nodesToJson(_getChildrenFiltered(node, @delegate), @delegate, parent: node));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildrenDetailsSubtree(string diagnosticableId, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getChildrenDetailsSubtree(diagnosticableId, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildrenDetailsSubtree(string? diagnosticableId, string groupName)
    {
        global::Doroti.Framework.Foundation.DiagnosticsNode? node = ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        var @delegate = new InspectorSerializationDelegate(groupName: groupName, includeProperties: true, service: this);
        return ((List<object>)(object?)_nodesToJson(((node is null) ? new List<global::Doroti.Framework.Foundation.DiagnosticsNode>() : _getChildrenFiltered(node, @delegate)), @delegate, parent: node));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _shouldShowInSummaryTree(global::Doroti.Framework.Foundation.DiagnosticsNode node)
    {
        if ((object.Equals(((global::Doroti.Framework.Foundation.DiagnosticsNode)node).level, global::Doroti.Framework.Foundation.DiagnosticLevel.error)))
        {
            return true;
        }
        object? valueLocal = ((global::Doroti.Framework.Foundation.DiagnosticsNode)node).value;
        if ((valueLocal is not global::Doroti.Framework.Foundation.Diagnosticable))
        {
            return true;
        }
        if (((((global::Doroti.Framework.Foundation.Diagnosticable)valueLocal) is not Element) || !isWidgetCreationTracked()))
        {
            return true;
        }
        return _isValueCreatedByLocalProject(((Element)valueLocal));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> _getChildrenFiltered(global::Doroti.Framework.Foundation.DiagnosticsNode node, InspectorSerializationDelegate @delegate)
    {
        return ((List<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)_filterChildren(node.getChildren().ToList(), @delegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> _filterChildren(List<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate)
    {
        var children = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        foreach (var child in nodes)
        {
            InspectorSerializationDelegate? updatedDelegate = ((InspectorSerializationDelegate?)(object?)_updateDelegateForWidgetInspectorEnabledState(@delegate: @delegate, node: child));
            bool inDisableWidgetInspectorScopeLocal = (((updatedDelegate?.inDisableWidgetInspectorScope ?? false)) || ((InspectorSerializationDelegate)@delegate).inDisableWidgetInspectorScope);
            if ((!inDisableWidgetInspectorScopeLocal && ((!((InspectorSerializationDelegate)@delegate).summaryTree || _shouldShowInSummaryTree(child)))))
            {
                children.Add(child);
            }
            else
            {
                children.AddRange(_getChildrenFiltered(child, (updatedDelegate ?? @delegate)));
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InspectorSerializationDelegate? _updateDelegateForWidgetInspectorEnabledState(InspectorSerializationDelegate @delegate, global::Doroti.Framework.Foundation.DiagnosticsNode node)
    {
        object? valueLocal = ((global::Doroti.Framework.Foundation.DiagnosticsNode)node).value;
        if ((!((InspectorSerializationDelegate)@delegate).inDisableWidgetInspectorScope && (valueLocal is _DisableWidgetInspectorScopeProxyElement__widget_inspector)))
        {
            _DisableWidgetInspectorScopeProxyElement__widget_inspector value__72458__as72537 = (_DisableWidgetInspectorScopeProxyElement__widget_inspector)valueLocal;
            return ((InspectorSerializationDelegate?)(object?)@delegate.copyWith(inDisableWidgetInspectorScope: true));
        }
        else
        {
            if ((((InspectorSerializationDelegate)@delegate).inDisableWidgetInspectorScope && (valueLocal is _EnableWidgetInspectorScopeProxyElement__widget_inspector)))
            {
                _EnableWidgetInspectorScopeProxyElement__widget_inspector value__72458__as72724 = (_EnableWidgetInspectorScopeProxyElement__widget_inspector)valueLocal;
                return ((InspectorSerializationDelegate?)(object?)@delegate.copyWith(inDisableWidgetInspectorScope: false));
            }
        }
        return ((InspectorSerializationDelegate)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getRootWidget(string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getRootWidget(groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getRootWidget(string groupName)
    {
        return ((DartMap<string, object>?)(object?)_nodeToJson(((Diagnosticable)WidgetsBinding.instance.rootElement).toDiagnosticsNode(), new InspectorSerializationDelegate(groupName: groupName, service: this)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getRootWidgetSummaryTree(string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getRootWidgetSummaryTree(groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getRootWidgetSummaryTree(string groupName, global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null)
    {
        return ((DartMap<string, object>?)(object?)_getRootWidgetTreeImpl(groupName: groupName, isSummaryTree: true, withPreviews: false, addAdditionalPropertiesCallback: (global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>?)addAdditionalPropertiesCallback));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _getRootWidgetSummaryTreeWithPreviews(DartMap<string, string> parameters)
    {
        string groupNameLocal = parameters.GetValueOrDefault("groupName")!;
        DartMap<string, object?>? result = ((DartMap<string, object?>?)(object?)_getRootWidgetTreeImpl(groupName: groupNameLocal, isSummaryTree: true, withPreviews: true));
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = result });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _getRootWidgetTree(DartMap<string, string> parameters)
    {
        string groupNameLocal = parameters.GetValueOrDefault("groupName")!;
        var isSummaryTreeLocal = (parameters.GetValueOrDefault("isSummaryTree") == "true");
        var withPreviewsLocal = (parameters.GetValueOrDefault("withPreviews") == "true");
        var fullDetailsLocal = (parameters.GetValueOrDefault("fullDetails") != "false");
        DartMap<string, object?>? result = ((DartMap<string, object?>?)(object?)_getRootWidgetTreeImpl(groupName: groupNameLocal, isSummaryTree: isSummaryTreeLocal, withPreviews: withPreviewsLocal, fullDetails: fullDetailsLocal));
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = result });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getRootWidgetTreeImpl(string groupName, bool isSummaryTree, bool withPreviews, bool fullDetails = true, global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null)
    {
        bool shouldAddAdditionalProperties = ((addAdditionalPropertiesCallback is not null) || withPreviews);
        DartMap<string, object>? combinedAddAdditionalPropertiesCallback(global::Doroti.Framework.Foundation.DiagnosticsNode node, InspectorSerializationDelegate @delegate)
        {
            DartMap<string, object> additionalPropertiesJson = ((addAdditionalPropertiesCallback is null ? new DartMap<string, object>() : addAdditionalPropertiesCallback.Invoke(node, @delegate)));
            if (!withPreviews)
            {
                return additionalPropertiesJson;
            }
            object? valueLocal = ((global::Doroti.Framework.Foundation.DiagnosticsNode)node).value;
            if ((valueLocal is Element))
            {
                Element value__76023__as76053 = (Element)valueLocal;
                global::Doroti.Framework.Rendering.RenderObject? renderObject = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)_renderObjectOrNull(((Element)value__76023__as76053)));
                if ((renderObject is global::Doroti.Framework.Rendering.RenderParagraph))
                {
                    global::Doroti.Framework.Rendering.RenderParagraph renderObject__76101__as76156 = (global::Doroti.Framework.Rendering.RenderParagraph)renderObject;
                    additionalPropertiesJson["textPreview"] = ((global::Doroti.Framework.Rendering.RenderParagraph)((global::Doroti.Framework.Rendering.RenderParagraph)renderObject__76101__as76156)).text.toPlainText();
                }
            }
            return additionalPropertiesJson;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return ((DartMap<string, object>?)(object?)_nodeToJson(((Diagnosticable)WidgetsBinding.instance.rootElement).toDiagnosticsNode(), new InspectorSerializationDelegate(groupName: groupName, subtreeDepth: 1000000L, summaryTree: isSummaryTree, service: this, addAdditionalPropertiesCallback: ((global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>)(shouldAddAdditionalProperties ? combinedAddAdditionalPropertiesCallback : null))), fullDetails: fullDetails));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getDetailsSubtree(string diagnosticableId, string groupName, long subtreeDepth = 2)
    {
        return ((string)(object?)_safeJsonEncode(_getDetailsSubtree(diagnosticableId, groupName, subtreeDepth)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getDetailsSubtree(string? diagnosticableId, string? groupName, long subtreeDepth)
    {
        global::Doroti.Framework.Foundation.DiagnosticsNode? root = ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        if ((root is null))
        {
            return ((DartMap<string, object>)(object)null);
        }
        return ((DartMap<string, object>?)(object?)_nodeToJson(root, new InspectorSerializationDelegate(groupName: groupName, subtreeDepth: subtreeDepth, includeProperties: true, service: this)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getSelectedWidget(string? previousSelectionId, string groupName)
    {
        if ((previousSelectionId is not null))
        {
            global::Doroti.Framework.Foundation.PrintLibrary.debugPrint("previousSelectionId is deprecated in API");
        }
        return ((string)(object?)_safeJsonEncode(_getSelectedWidget(((string)(object)null), groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<global::Doroti.Ui.Image?> screenshot(object? @object, double width, double height, double margin = 0.0, double maxPixelRatio = 1.0, bool debugPaint = false)
    {
        if (((@object is not Element) && (@object is not global::Doroti.Framework.Rendering.RenderObject)))
        {
            return ((global::Doroti.Ui.Image)(object)null);
        }
        global::Doroti.Framework.Rendering.RenderObject? renderObject = ((@object is Element) ? _renderObjectOrNull((Element)@object) : (((global::Doroti.Framework.Rendering.RenderObject?)(object?)@object)!));
        if (((renderObject is null) || !((global::Doroti.Framework.Rendering.RenderObject)renderObject).attached))
        {
            return ((global::Doroti.Ui.Image)(object)null);
        }
        if (((global::Doroti.Framework.Rendering.RenderObject)renderObject).debugNeedsLayout)
        {
            global::Doroti.Framework.Rendering.PipelineOwner ownerLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.PipelineOwner>(((global::Doroti.Framework.Rendering.RenderObject)renderObject).owner!);
            DartRuntimePrimitives.Assert(() => !((global::Doroti.Framework.Rendering.PipelineOwner)ownerLocal).debugDoingLayout);
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Framework.Rendering.PipelineOwner>)(() =>
{
    var __cascade = ownerLocal;
    __cascade.flushLayout();
    __cascade.flushCompositingBits();
    __cascade.flushPaint();
    return __cascade;
}))());
            if (((global::Doroti.Framework.Rendering.RenderObject)renderObject).debugNeedsLayout)
            {
                return ((global::Doroti.Ui.Image)(object)null);
            }
        }
        global::Doroti.Ui.Rect renderBounds = ((global::Doroti.Ui.Rect)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Ui.Rect>(Widget_inspectorLibrary._calculateSubtreeBounds(renderObject)));
        if ((margin != 0.0))
        {
            renderBounds = renderBounds.inflate(margin);
        }
        if (renderBounds.isEmpty)
        {
            return ((global::Doroti.Ui.Image)(object)null);
        }
        double pixelRatioLocal = Math.Min(maxPixelRatio, Math.Min((width / renderBounds.width), (height / renderBounds.height)));
        return await _ScreenshotPaintingContext__widget_inspector.toImage(renderObject, renderBounds, pixelRatio: pixelRatioLocal, debugPaint: debugPaint);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _getLayoutExplorerNode(DartMap<string, string> parameters)
    {
        string? diagnosticableId = parameters.GetValueOrDefault("id");
        long subtreeDepthLocal = Dart_coreLibrary.parse(parameters.GetValueOrDefault("subtreeDepth")!);
        string? groupNameLocal = parameters.GetValueOrDefault("groupName");
        DartMap<string, object>? result = new DartMap<string, object>().cast<string, object>();
        global::Doroti.Framework.Foundation.DiagnosticsNode? root = ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        if ((root is null))
        {
            return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = result });
        }
        result = _nodeToJson(root, new InspectorSerializationDelegate(groupName: groupNameLocal, summaryTree: true, subtreeDepth: subtreeDepthLocal, service: this, addAdditionalPropertiesCallback: ((global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>?)((node, @delegate) =>
        {
            object? valueLocal = ((global::Doroti.Framework.Foundation.DiagnosticsNode)node).value;
            global::Doroti.Framework.Rendering.RenderObject? renderObject = ((valueLocal is Element) ? _renderObjectOrNull(((Element)valueLocal)) : null);
            if ((renderObject is null))
            {
                return new DartMap<string, object>();
            }
            global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate renderObjectSerializationDelegate = ((global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate)(object?)@delegate.copyWith(subtreeDepth: 0L, includeProperties: true, expandPropertyValues: false));
            var additionalJson = new DartMap<string, object>();
            global::Doroti.Framework.Rendering.RenderObject? renderParent = ((global::Doroti.Framework.Rendering.RenderObject)renderObject).parent;
            if ((((renderParent is not null) && (((InspectorSerializationDelegate)@delegate).subtreeDepth > 0L)) && ((InspectorSerializationDelegate)@delegate).expandPropertyValues))
            {
                object? parentCreator = ((global::Doroti.Framework.Rendering.RenderObject)renderParent).debugCreator;
                if ((parentCreator is DebugCreator))
                {
                    DebugCreator parentCreator__82646__as82705 = (DebugCreator)parentCreator;
                    additionalJson["parentRenderElement"] = ((Diagnosticable)((DebugCreator)((DebugCreator)parentCreator__82646__as82705)).element).toDiagnosticsNode().toJsonMap(@delegate.copyWith(subtreeDepth: 0L, includeProperties: true));
                }
            }
            try
            {
                if (!((global::Doroti.Framework.Rendering.RenderObject)renderObject).debugNeedsLayout)
                {
                    global::Doroti.Framework.Rendering.Constraints constraintsLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.Constraints>(((global::Doroti.Framework.Rendering.RenderObject)renderObject).constraints);
                    var constraintsProperty = new DartMap<string, object> { ["type"] = DartRuntimePrimitives.RuntimeTypeName(constraintsLocal), ["description"] = constraintsLocal.ToString() };
                    if ((constraintsLocal is global::Doroti.Framework.Rendering.BoxConstraints))
                    {
                        global::Doroti.Framework.Rendering.BoxConstraints constraints__83404__as83654 = (global::Doroti.Framework.Rendering.BoxConstraints)constraintsLocal;
                        constraintsProperty.AddRange(new DartMap<string, object> { ["minWidth"] = ((global::Doroti.Framework.Rendering.BoxConstraints)((global::Doroti.Framework.Rendering.BoxConstraints)constraints__83404__as83654)).minWidth.ToString(), ["minHeight"] = ((global::Doroti.Framework.Rendering.BoxConstraints)((global::Doroti.Framework.Rendering.BoxConstraints)constraints__83404__as83654)).minHeight.ToString(), ["maxWidth"] = ((global::Doroti.Framework.Rendering.BoxConstraints)((global::Doroti.Framework.Rendering.BoxConstraints)constraints__83404__as83654)).maxWidth.ToString(), ["maxHeight"] = ((global::Doroti.Framework.Rendering.BoxConstraints)((global::Doroti.Framework.Rendering.BoxConstraints)constraints__83404__as83654)).maxHeight.ToString() });
                    }
                    additionalJson["constraints"] = constraintsProperty;
                }
            }
            catch (Exception e)
            {
            }
            try
            {
                if ((renderObject is global::Doroti.Framework.Rendering.RenderBox))
                {
                    global::Doroti.Framework.Rendering.RenderBox renderObject__81532__as84297 = (global::Doroti.Framework.Rendering.RenderBox)renderObject;
                    additionalJson["isBox"] = true;
                    additionalJson["size"] = new DartMap<string, object> { ["width"] = ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)renderObject__81532__as84297)).size.width.ToString(), ["height"] = ((global::Doroti.Framework.Rendering.RenderBox)((global::Doroti.Framework.Rendering.RenderBox)renderObject__81532__as84297)).size.height.ToString() };
                    global::Doroti.Framework.Rendering.ParentData? parentDataLocal = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Rendering.ParentData>(((global::Doroti.Framework.Rendering.RenderBox)renderObject__81532__as84297).parentData);
                    if ((parentDataLocal is global::Doroti.Framework.Rendering.FlexParentData))
                    {
                        global::Doroti.Framework.Rendering.FlexParentData parentData__84603__as84659 = (global::Doroti.Framework.Rendering.FlexParentData)parentDataLocal;
                        additionalJson["flexFactor"] = (((global::Doroti.Framework.Rendering.FlexParentData)((global::Doroti.Framework.Rendering.FlexParentData)parentData__84603__as84659)).flex ?? 0L);
                        additionalJson["flexFit"] = ((((global::Doroti.Framework.Rendering.FlexParentData)((global::Doroti.Framework.Rendering.FlexParentData)parentData__84603__as84659)).fit ?? global::Doroti.Framework.Rendering.FlexFit.tight)).ToString();
                    }
                    else
                    {
                        if ((parentDataLocal is global::Doroti.Framework.Rendering.BoxParentData))
                        {
                            global::Doroti.Framework.Rendering.BoxParentData parentData__84603__as84869 = (global::Doroti.Framework.Rendering.BoxParentData)parentDataLocal;
                            global::Doroti.Ui.Offset offsetLocal = ((global::Doroti.Ui.Offset)(object?)((global::Doroti.Framework.Rendering.BoxParentData)((global::Doroti.Framework.Rendering.BoxParentData)parentData__84603__as84869)).offset);
                            additionalJson["parentData"] = new DartMap<string, object> { ["offsetX"] = offsetLocal.dx.ToString(), ["offsetY"] = offsetLocal.dy.ToString() };
                        }
                    }
                }
                else
                {
                    if ((renderObject is global::Doroti.Framework.Rendering.RenderView))
                    {
                        global::Doroti.Framework.Rendering.RenderView renderObject__81532__as85182 = (global::Doroti.Framework.Rendering.RenderView)renderObject;
                        additionalJson["size"] = new DartMap<string, object> { ["width"] = ((global::Doroti.Framework.Rendering.RenderView)((global::Doroti.Framework.Rendering.RenderView)renderObject__81532__as85182)).size.width.ToString(), ["height"] = ((global::Doroti.Framework.Rendering.RenderView)((global::Doroti.Framework.Rendering.RenderView)renderObject__81532__as85182)).size.height.ToString() };
                    }
                }
            }
            catch (Exception eLocal)
            {
            }
            return additionalJson;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }))));
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = result });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _setFlexFit(DartMap<string, string> parameters)
    {
        string? id = parameters.GetValueOrDefault("id");
        string parameter = parameters.GetValueOrDefault("flexFit")!;
        global::Doroti.Framework.Rendering.FlexFit flexFit = _toEnumEntry<global::Doroti.Framework.Rendering.FlexFit>(System.Enum.GetValues<global::Doroti.Framework.Rendering.FlexFit>().ToList(), parameter);
        object? @object = toObject(id);
        var succeed = false;
        if (((@object is not null) && (@object is Element)))
        {
            Element @object__85909__as85983 = (Element)@object;
            global::Doroti.Framework.Rendering.RenderObject? render = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)_renderObjectOrNull(((Element)@object__85909__as85983)));
            global::Doroti.Framework.Rendering.ParentData? parentDataLocal = ((global::Doroti.Framework.Rendering.ParentData?)((dynamic)render)?.parentData);
            if ((parentDataLocal is global::Doroti.Framework.Rendering.FlexParentData))
            {
                global::Doroti.Framework.Rendering.FlexParentData parentData__86092__as86135 = (global::Doroti.Framework.Rendering.FlexParentData)parentDataLocal;
                parentData__86092__as86135.fit = flexFit;
                ((dynamic)render!).markNeedsLayout();
                succeed = true;
            }
        }
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = succeed });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _setFlexFactor(DartMap<string, string> parameters)
    {
        string? id = parameters.GetValueOrDefault("id");
        string flexFactor = parameters.GetValueOrDefault("flexFactor")!;
        long? factor = ((flexFactor == "null") ? null : Dart_coreLibrary.parse(flexFactor));
        dynamic @object = toObject(id);
        var succeed = false;
        if (((@object is not null) && (@object is Element)))
        {
            Element @object__86635__as86709 = (Element)@object;
            global::Doroti.Framework.Rendering.RenderObject? render = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)_renderObjectOrNull(((Element)@object__86635__as86709)));
            global::Doroti.Framework.Rendering.ParentData? parentDataLocal = ((global::Doroti.Framework.Rendering.ParentData?)((dynamic)render)?.parentData);
            if ((parentDataLocal is global::Doroti.Framework.Rendering.FlexParentData))
            {
                global::Doroti.Framework.Rendering.FlexParentData parentData__86818__as86861 = (global::Doroti.Framework.Rendering.FlexParentData)parentDataLocal;
                parentData__86818__as86861.flex = factor;
                ((dynamic)render!).markNeedsLayout();
                succeed = true;
            }
        }
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = succeed });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _setFlexProperties(DartMap<string, string> parameters)
    {
        string? id = parameters.GetValueOrDefault("id");
        global::Doroti.Framework.Rendering.MainAxisAlignment mainAxisAlignmentLocal = _toEnumEntry<global::Doroti.Framework.Rendering.MainAxisAlignment>(System.Enum.GetValues<global::Doroti.Framework.Rendering.MainAxisAlignment>().ToList(), parameters.GetValueOrDefault("mainAxisAlignment")!);
        global::Doroti.Framework.Rendering.CrossAxisAlignment crossAxisAlignmentLocal = _toEnumEntry<global::Doroti.Framework.Rendering.CrossAxisAlignment>(System.Enum.GetValues<global::Doroti.Framework.Rendering.CrossAxisAlignment>().ToList(), parameters.GetValueOrDefault("crossAxisAlignment")!);
        object? @object = toObject(id);
        var succeed = false;
        if (((@object is not null) && (@object is Element)))
        {
            Element @object__87556__as87630 = (Element)@object;
            global::Doroti.Framework.Rendering.RenderObject? render = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)_renderObjectOrNull(((Element)@object__87556__as87630)));
            if ((render is global::Doroti.Framework.Rendering.RenderFlex))
            {
                global::Doroti.Framework.Rendering.RenderFlex render__87677__as87725 = (global::Doroti.Framework.Rendering.RenderFlex)render;
                render__87677__as87725.mainAxisAlignment = mainAxisAlignmentLocal;
                render__87677__as87725.crossAxisAlignment = crossAxisAlignmentLocal;
                ((global::Doroti.Framework.Rendering.RenderFlex)render__87677__as87725).markNeedsLayout();
                ((global::Doroti.Framework.Rendering.RenderFlex)render__87677__as87725).markNeedsPaint();
                succeed = true;
            }
        }
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = succeed });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T _toEnumEntry<T>(List<T> enumEntries, string name)
    {
        foreach (var entry in enumEntries)
        {
            if ((entry.ToString() == name))
            {
                return entry;
            }
        }
        throw new Exception($"Enum value {name} not found");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getSelectedWidget(string? previousSelectionId, string groupName)
    {
        return ((DartMap<string, object>?)(object?)_nodeToJson(_getSelectedWidgetDiagnosticsNode(previousSelectionId), new InspectorSerializationDelegate(groupName: groupName, service: this)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode? _getSelectedWidgetDiagnosticsNode(string? previousSelectionId)
    {
        var previousSelection = ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)toObject(previousSelectionId))!;
        Element? current = ((InspectorSelection)this.selection).currentElement;
        return ((object.Equals(current, previousSelection?.value)) ? previousSelection : ((Diagnosticable)current).toDiagnosticsNode());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getSelectedSummaryWidget(string? previousSelectionId, string groupName)
    {
        if ((previousSelectionId is not null))
        {
            global::Doroti.Framework.Foundation.PrintLibrary.debugPrint("previousSelectionId is deprecated in API");
        }
        return ((string)(object?)_safeJsonEncode(_getSelectedSummaryWidget(((string)(object)null), groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Runtime.CreationLocation? _getSelectedWidgetLocation(bool restrictToSummaryTree = false)
    {
        global::Doroti.Framework.Foundation.DiagnosticsNode? selectedNode = (restrictToSummaryTree ? _getSelectedSummaryDiagnosticsNode(((string)(object)null)) : _getSelectedWidgetDiagnosticsNode(((string)(object)null)));
        return ((global::Doroti.Runtime.CreationLocation)(object)Widget_inspectorLibrary._getCreationLocation(selectedNode?.value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.DiagnosticsNode? _getSelectedSummaryDiagnosticsNode(string? previousSelectionId)
    {
        if (!isWidgetCreationTracked())
        {
            return ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)_getSelectedWidgetDiagnosticsNode(previousSelectionId));
        }
        var previousSelection = ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)toObject(previousSelectionId))!;
        Element? current = ((InspectorSelection)this.selection).currentElement;
        if (((current is not null) && !_isValueCreatedByLocalProject(current)))
        {
            Element? firstLocal = default!;
            foreach (Element candidate in current.debugGetDiagnosticChain())
            {
                if (_isValueCreatedByLocalProject(candidate))
                {
                    firstLocal = candidate;
                    break;
                }
            }
            current = firstLocal;
        }
        return ((object.Equals(current, previousSelection?.value)) ? previousSelection : ((Diagnosticable)current).toDiagnosticsNode());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getSelectedSummaryWidget(string? previousSelectionId, string groupName)
    {
        return ((DartMap<string, object>?)(object?)_nodeToJson(_getSelectedSummaryDiagnosticsNode(previousSelectionId), new InspectorSerializationDelegate(groupName: groupName, service: this)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isWidgetCreationTracked()
    {
        this._widgetCreationTracked ??= ((global::Doroti.Runtime.CreationLocation.of(new _WidgetForTypeTests__widget_inspector()) is not null));
        return DartRuntimePrimitives.RequireValue(this._widgetCreationTracked);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _onFrameStart(Duration timeStamp)
    {
        this._frameStart = timeStamp;
        this._frameNumber = PlatformDispatcher.instance.frameData.frameNumber;
        global::Doroti.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((global::System.Action<Duration>)this._onFrameEnd)(__arg0), debugLabel: "WidgetInspector.onFrameStart");
    }

    public virtual void _onFrameEnd(Duration timeStamp)
    {
        if (this._trackRebuildDirtyWidgets)
        {
            _postStatsEvent("Flutter.RebuiltWidgets", this._rebuildStats);
        }
        if (this._trackRepaintWidgets)
        {
            _postStatsEvent("Flutter.RepaintWidgets", this._repaintStats);
        }
    }

    public virtual void _postStatsEvent(string eventName, _ElementLocationStatsTracker__widget_inspector stats)
    {
        postEvent(eventName, stats.exportToJson(this._frameStart, frameNumber: this._frameNumber).cast<object, object>());
    }

    public virtual void postEvent(string eventKind, DartMap<object, object> eventData, string stream = "Extension")
    {
        Dart_developerLibrary.postEvent(eventKind, eventData, stream: stream);
    }

    public virtual void inspect(object? @object)
    {
        Dart_developerLibrary.inspect(@object);
    }

    public virtual void _onRebuildWidget(Element element, bool builtOnce)
    {
        this._rebuildStats.add(element);
    }

    public virtual void _onPaint(global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        try
        {
            Element? elementLocal = DartRuntimePrimitives.ConvertValue<Element>((((DebugCreator?)(object?)((global::Doroti.Framework.Rendering.RenderObject)renderObject).debugCreator)!)?.element);
            if ((elementLocal is not RenderObjectElement))
            {
                return;
            }
            this._repaintStats.add(((RenderObjectElement)elementLocal));
            ((RenderObjectElement)elementLocal).visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) =>
            {
                if ((ancestor is RenderObjectElement))
                {
                    return false;
                }
                this._repaintStats.add(ancestor);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            })));
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widget inspector library", context: new global::Doroti.Framework.Foundation.ErrorDescription("while tracking widget repaints")));
        }
    }

    public virtual void performReassemble()
    {
        _clearStats();
        _resetErrorCount();
    }

    public virtual global::Doroti.Framework.Rendering.RenderObject? _renderObjectOrNull(Element element) => (((Element)element).mounted ? ((Element)element).renderObject : null);
}

public interface WidgetInspectorService
{
    List<string?> _serializeRing { get; }
    long _serializeRingIndex { get; set; }
    public static WidgetInspectorService _instance = ((WidgetInspectorService)(object?)new _WidgetInspectorService__widget_inspector());
    internal static bool _debugServiceExtensionsRegistered = false;
    InspectorSelection selection { get; }
    global::System.Action? selectionChangedCallback { get; set; }
    DartMap<string, HashSet<InspectorReferenceData>> _groups { get; }
    DartMap<string, InspectorReferenceData> _idToReferenceData { get; }
    WeakMap<object, string> _objectToId { get; }
    long _nextId { get; set; }
    List<string>? _pubRootDirectories { get; set; }
    DartMap<string, bool> _isLocalCreationCache { get; }
    bool _trackRebuildDirtyWidgets { get; set; }
    bool _trackRepaintWidgets { get; set; }
    internal const string _consoleObjectGroup = "console-group";
    long _errorsSinceReload { get; set; }
    bool? _widgetCreationTracked { get; set; }
    Duration _frameStart { get; set; }
    long _frameNumber { get; set; }
    _ElementLocationStatsTracker__widget_inspector _rebuildStats { get; }
    _ElementLocationStatsTracker__widget_inspector _repaintStats { get; }

    public static WidgetInspectorService instance
    {
        get => _instance;
        set => _instance = value;
    }
    public bool isSelectMode { set; }
    public void registerServiceExtension(string name, global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>> callback, RegisterServiceExtensionCallback registerExtension);
    public void _registerSignalServiceExtension(string name, dynamic callback, RegisterServiceExtensionCallback registerExtension);
    public void _registerObjectGroupServiceExtension(string name, dynamic callback, RegisterServiceExtensionCallback registerExtension);
    public void _registerBoolServiceExtension(string name, global::System.Func<Future<bool>> getter, global::System.Func<bool, Future> setter, RegisterServiceExtensionCallback registerExtension);
    public void _postExtensionStateChangedEvent(string name, object? value);
    public void _registerServiceExtensionWithArg(string name, dynamic callback, RegisterServiceExtensionCallback registerExtension);
    public void _registerServiceExtensionVarArgs(string name, global::System.Func<List<string>, object> callback, RegisterServiceExtensionCallback registerExtension);
    public Future forceRebuild();
    public void _reportStructuredError(global::Doroti.Framework.Foundation.FlutterErrorDetails details);
    public void _resetErrorCount();
    public bool isStructuredErrorsEnabled();
    public void _clearStats();
    public void disposeAllGroups();
    public void resetAllState();
    public void disposeGroup(string name);
    public void _decrementReferenceCount(InspectorReferenceData reference);
    public string? toId(object? @object, string groupName);
    public bool isWidgetTreeReady(string? groupName = null);
    public object? toObject(string? id, string? groupName = null);
    public object? toObjectForSourceLocation(string id, string? groupName = null);
    public void disposeId(string? id, string groupName);
    public void setPubRootDirectories(List<string> pubRootDirectories);
    public void resetPubRootDirectories();
    public void addPubRootDirectories(List<string> pubRootDirectories);
    public void removePubRootDirectories(List<string> pubRootDirectories);
    public Future<DartMap<string, object>> pubRootDirectories(DartMap<string, string> parameters);
    public bool setSelectionById(string? id, string? groupName = null);
    public bool setSelection(object? @object, string? groupName = null);
    public void _notifyToolsOfSelection(object? @object, bool restrictToProjectFiles = false);
    public void _changeWidgetSelectionMode(bool enabled, bool notifyStateChange = true);
    public string? _devToolsInspectorUriForElement(Element element);
    public string devToolsInspectorUri(string inspectorRef);
    public string getParentChain(string id, string groupName);
    public List<object?> _getParentChain(string? id, string groupName);
    public List<Element> _getRawElementParentChain(Element element, long? numLocalParents);
    public List<_DiagnosticsPathNode__widget_inspector> _getElementParentChain(Element element, string groupName, long? numLocalParents = null);
    public List<_DiagnosticsPathNode__widget_inspector>? _getRenderObjectParentChain(global::Doroti.Framework.Rendering.RenderObject? renderObject, string groupName);
    public DartMap<string, object?>? _nodeToJson(global::Doroti.Framework.Foundation.DiagnosticsNode? node, InspectorSerializationDelegate @delegate, bool fullDetails = true);
    public bool _isValueCreatedByLocalProject(object? value);
    public bool _isLocalCreationLocationImpl(string locationUri);
    public bool _isLocalCreationLocation(string locationUri);
    public string _safeJsonEncode(object? @object);
    public List<global::Doroti.Framework.Foundation.DiagnosticsNode> _truncateNodes(IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, long maxDescendentsTruncatableNode);
    public List<DartMap<string, object?>> _nodesToJson(List<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate, global::Doroti.Framework.Foundation.DiagnosticsNode? parent);
    public string getProperties(string diagnosticsNodeId, string groupName);
    public List<object> _getProperties(string? diagnosticableId, string groupName);
    public string getChildren(string diagnosticsNodeId, string groupName);
    public List<object> _getChildren(string? diagnosticsNodeId, string groupName);
    public string getChildrenSummaryTree(string diagnosticsNodeId, string groupName);
    public global::Doroti.Framework.Foundation.DiagnosticsNode? _idToDiagnosticsNode(string? diagnosticableId);
    public static global::Doroti.Framework.Foundation.DiagnosticsNode? objectToDiagnosticsNode(object? @object)
    {
        if ((@object is global::Doroti.Framework.Foundation.Diagnosticable))
        {
            global::Doroti.Framework.Foundation.Diagnosticable @object__as68125 = (global::Doroti.Framework.Foundation.Diagnosticable)@object;
            return ((global::Doroti.Framework.Foundation.DiagnosticsNode?)(object?)((Diagnosticable)((global::Doroti.Framework.Foundation.Diagnosticable)@object__as68125)).toDiagnosticsNode());
        }
        return ((global::Doroti.Framework.Foundation.DiagnosticsNode)(object)null);
    }
    public List<object> _getChildrenSummaryTree(string? diagnosticableId, string groupName);
    public string getChildrenDetailsSubtree(string diagnosticableId, string groupName);
    public List<object> _getChildrenDetailsSubtree(string? diagnosticableId, string groupName);
    public bool _shouldShowInSummaryTree(global::Doroti.Framework.Foundation.DiagnosticsNode node);
    public List<global::Doroti.Framework.Foundation.DiagnosticsNode> _getChildrenFiltered(global::Doroti.Framework.Foundation.DiagnosticsNode node, InspectorSerializationDelegate @delegate);
    public List<global::Doroti.Framework.Foundation.DiagnosticsNode> _filterChildren(List<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate);
    public InspectorSerializationDelegate? _updateDelegateForWidgetInspectorEnabledState(InspectorSerializationDelegate @delegate, global::Doroti.Framework.Foundation.DiagnosticsNode node);
    public string getRootWidget(string groupName);
    public DartMap<string, object?>? _getRootWidget(string groupName);
    public string getRootWidgetSummaryTree(string groupName);
    public DartMap<string, object?>? _getRootWidgetSummaryTree(string groupName, global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null);
    public Future<DartMap<string, object?>> _getRootWidgetSummaryTreeWithPreviews(DartMap<string, string> parameters);
    public Future<DartMap<string, object?>> _getRootWidgetTree(DartMap<string, string> parameters);
    public DartMap<string, object?>? _getRootWidgetTreeImpl(string groupName, bool isSummaryTree, bool withPreviews, bool fullDetails = true, global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null);
    public string getDetailsSubtree(string diagnosticableId, string groupName, long subtreeDepth = 2);
    public DartMap<string, object?>? _getDetailsSubtree(string? diagnosticableId, string? groupName, long subtreeDepth);
    public string getSelectedWidget(string? previousSelectionId, string groupName);
    public Future<global::Doroti.Ui.Image?> screenshot(object? @object, double width, double height, double margin = 0.0, double maxPixelRatio = 1.0, bool debugPaint = false);
    public Future<DartMap<string, object?>> _getLayoutExplorerNode(DartMap<string, string> parameters);
    public Future<DartMap<string, object>> _setFlexFit(DartMap<string, string> parameters);
    public Future<DartMap<string, object>> _setFlexFactor(DartMap<string, string> parameters);
    public Future<DartMap<string, object>> _setFlexProperties(DartMap<string, string> parameters);
    public T _toEnumEntry<T>(List<T> enumEntries, string name);
    public DartMap<string, object?>? _getSelectedWidget(string? previousSelectionId, string groupName);
    public global::Doroti.Framework.Foundation.DiagnosticsNode? _getSelectedWidgetDiagnosticsNode(string? previousSelectionId);
    public string getSelectedSummaryWidget(string? previousSelectionId, string groupName);
    public global::Doroti.Runtime.CreationLocation? _getSelectedWidgetLocation(bool restrictToSummaryTree = false);
    public global::Doroti.Framework.Foundation.DiagnosticsNode? _getSelectedSummaryDiagnosticsNode(string? previousSelectionId);
    public DartMap<string, object?>? _getSelectedSummaryWidget(string? previousSelectionId, string groupName);
    public bool isWidgetCreationTracked();
    public void _onFrameStart(Duration timeStamp);
    public void _onFrameEnd(Duration timeStamp);
    public void _postStatsEvent(string eventName, _ElementLocationStatsTracker__widget_inspector stats);
    public void postEvent(string eventKind, DartMap<object, object> eventData, string stream = "Extension");
    public void inspect(object? @object);
    public void _onRebuildWidget(Element element, bool builtOnce);
    public void _onPaint(global::Doroti.Framework.Rendering.RenderObject renderObject);
    public global::Doroti.Framework.Rendering.RenderObject? _renderObjectOrNull(Element element);
}

public class _LocationCount__widget_inspector
{
    public virtual long id { get; private set; } = default!;
    public virtual bool local { get; private set; } = default!;
    public virtual object location { get; private set; } = default!;
    internal virtual long _count { get; set; } = 0L;

    internal _LocationCount__widget_inspector(object location, long id, bool local)
    {
        this.location = location;
        this.id = id;
        this.local = local;
    }

    public virtual long count => this._count;
    public virtual void reset()
    {
        _count = 0L;
    }

    public virtual void increment()
    {
        _count++;
    }

}

public class _ElementLocationStatsTracker__widget_inspector
{
    internal virtual List<_LocationCount__widget_inspector?> _stats { get; private set; } = new List<_LocationCount__widget_inspector?>();
    public virtual List<_LocationCount__widget_inspector> active { get; private set; } = new List<_LocationCount__widget_inspector>();
    public virtual List<_LocationCount__widget_inspector> newLocations { get; private set; } = new List<_LocationCount__widget_inspector>();

    public virtual void add(Element element)
    {
        object widgetLocal = ((Element)element).widget;
        global::Doroti.Runtime.CreationLocation? locationLocal = ((global::Doroti.Runtime.CreationLocation?)(object?)global::Doroti.Runtime.CreationLocation.of(widgetLocal));
        if ((locationLocal is null))
        {
            return;
        }
        long idLocal = Widget_inspectorLibrary._toLocationId(locationLocal);
        _LocationCount__widget_inspector entry = default!;
        if (((idLocal >= checked((long)(this._stats.Count))) || (this._stats[(int)(idLocal)] is null)))
        {
            while ((idLocal >= checked((long)(this._stats.Count))))
            {
                this._stats.Add(((_LocationCount__widget_inspector)(object)null));
            }
            entry = new _LocationCount__widget_inspector(location: locationLocal, id: idLocal, local: WidgetInspectorService.instance._isLocalCreationLocation(((string)(object)((object)((dynamic)locationLocal).file))));
            if (((_LocationCount__widget_inspector)entry).local)
            {
                this.newLocations.Add(entry);
            }
            this._stats[(int)(idLocal)] = entry;
        }
        else
        {
            entry = this._stats[(int)(idLocal)]!;
        }
        if (((_LocationCount__widget_inspector)entry).local)
        {
            if ((((_LocationCount__widget_inspector)entry).count == 0L))
            {
                this.active.Add(entry);
            }
            entry.increment();
        }
    }

    public virtual void resetCounts()
    {
        foreach (_LocationCount__widget_inspector entry in this.active)
        {
            entry.reset();
        }
        this.active.Clear();
    }

    public virtual DartMap<string, object> exportToJson(Duration startTime, long frameNumber)
    {
        var events = new List<long>(System.Linq.Enumerable.Repeat<long>(0L, checked((int)(checked((long)(this.active.Count)) * 2L))));
        var j = 0L;
        foreach (_LocationCount__widget_inspector stat in this.active)
        {
            events[(int)(j++)] = ((_LocationCount__widget_inspector)stat).id;
            events[(int)(j++)] = ((_LocationCount__widget_inspector)stat).count;
        }
        var json = new DartMap<string, object> { ["startTime"] = startTime.inMicroseconds, ["frameNumber"] = frameNumber, ["events"] = events };
        if (System.Linq.Enumerable.Any(this.newLocations))
        {
            var locationsJson = new DartMap<string, List<long>>();
            foreach (_LocationCount__widget_inspector entry in this.newLocations)
            {
                global::Doroti.Runtime.CreationLocation locationLocal = ((global::Doroti.Runtime.CreationLocation)(object?)((_LocationCount__widget_inspector)entry).location);
                List<long> jsonForFile = locationsJson.putIfAbsent(((string)(object)((object)((dynamic)locationLocal).file)), (() => new List<long>())).ToList();
                DartRuntimePrimitives.Ignore(((Func<List<long>>)(() =>
{
    var __cascade = jsonForFile;
    __cascade.Add(((_LocationCount__widget_inspector)entry).id);
    __cascade.Add(((long)(object)((object)((dynamic)locationLocal).line)));
    __cascade.Add(((long)(object)((object)((dynamic)locationLocal).column)));
    return __cascade;
}))());
            }
            json["newLocations"] = locationsJson;
        }
        if (System.Linq.Enumerable.Any(this.newLocations))
        {
            var fileLocationsMap = new DartMap<string, DartMap<string, List<object>>>();
            foreach (_LocationCount__widget_inspector entryLocal in this.newLocations)
            {
                global::Doroti.Runtime.CreationLocation locationAlternate = ((global::Doroti.Runtime.CreationLocation)(object?)((_LocationCount__widget_inspector)entryLocal).location);
                DartMap<string, List<object?>> locations = fileLocationsMap.putIfAbsent(((string)(object)((object)((dynamic)locationAlternate).file)), (() => new DartMap<string, List<object>> { ["ids"] = new List<long>().Cast<object>().ToList(), ["lines"] = new List<long>().Cast<object>().ToList(), ["columns"] = new List<long>().Cast<object>().ToList(), ["names"] = new List<string?>().Cast<object>().ToList() })).cast<string, List<object?>>();
                locations.GetValueOrDefault("ids")!.Add(((_LocationCount__widget_inspector)entryLocal).id);
                locations.GetValueOrDefault("lines")!.Add(((object)((dynamic)locationAlternate).line));
                locations.GetValueOrDefault("columns")!.Add(((object)((dynamic)locationAlternate).column));
                locations.GetValueOrDefault("names")!.Add(locationAlternate.ToString());
            }
            json["locations"] = fileLocationsMap;
        }
        resetCounts();
        this.newLocations.Clear();
        return json;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _WidgetForTypeTests__widget_inspector : Widget
{
    internal _WidgetForTypeTests__widget_inspector()
    {
    }

    public override Element createElement() => throw new NotImplementedException();
}

public class WidgetInspector : StatefulWidget
{
    public virtual Widget child { get; private set; } = default!;
    public virtual ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder { get; private set; }
    public virtual MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder { get; private set; }
    public virtual TapBehaviorButtonBuilder? tapBehaviorButtonBuilder { get; private set; }

    public WidgetInspector(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!, TapBehaviorButtonBuilder? tapBehaviorButtonBuilder = default!, ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder = default!, MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder = default!) : base(key: key)
    {
        this.child = child;
        this.tapBehaviorButtonBuilder = tapBehaviorButtonBuilder;
        this.exitWidgetSelectionButtonBuilder = exitWidgetSelectionButtonBuilder;
        this.moveExitWidgetSelectionButtonBuilder = moveExitWidgetSelectionButtonBuilder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WidgetInspectorState__widget_inspector());
}

internal class _WidgetInspectorState__widget_inspector : State<WidgetInspector>, WidgetsBindingObserver
{
    internal virtual Offset? _lastPointerLocation { get; set; } = default;
    public virtual InspectorSelection selection { get; set; } = default!;
    public virtual bool isSelectMode { get; set; } = default!;
    internal virtual GlobalKey<IState> _ignorePointerKey { get; private set; } = GlobalKey<IState>.Create();
    internal const double _edgeHitMargin = 2.0;

    internal _WidgetInspectorState__widget_inspector()
    {
    }

    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _selectionOnTapEnabled => WidgetsBinding.instance.debugWidgetInspectorSelectionOnTapEnabled;
    internal virtual bool _isSelectModeWithSelectionOnTapEnabled => DartRuntimePrimitives.ConvertValue<bool>((this.isSelectMode && ((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this._selectionOnTapEnabled).value));
    public override void initState()
    {
        base.initState();
        WidgetInspectorService.instance.selection.addListener(() => this._selectionInformationChanged());
        WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier.addListener(() => this._selectionInformationChanged());
        this._selectionOnTapEnabled.addListener(() => this._selectionInformationChanged());
        selection = WidgetInspectorService.instance.selection;
        isSelectMode = WidgetsBinding.instance.debugShowWidgetInspectorOverride;
    }

    public override void dispose()
    {
        WidgetInspectorService.instance.selection.removeListener(() => this._selectionInformationChanged());
        WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier.removeListener(() => this._selectionInformationChanged());
        this._selectionOnTapEnabled.removeListener(() => this._selectionInformationChanged());
        base.dispose();
    }

    internal virtual void _selectionInformationChanged() => setState(((global::System.Action)(() =>
    {
        selection = WidgetInspectorService.instance.selection;
        isSelectMode = WidgetsBinding.instance.debugShowWidgetInspectorOverride;
    })));
    internal virtual bool _hitTestHelper(List<global::Doroti.Framework.Rendering.RenderObject> hits, List<global::Doroti.Framework.Rendering.RenderObject> edgeHits, Offset position, global::Doroti.Framework.Rendering.RenderObject @object, Matrix4 transform)
    {
        var hit = false;
        Matrix4? inverse = Matrix4.tryInvert(transform);
        if ((inverse is null))
        {
            return false;
        }
        global::Doroti.Ui.Offset localPosition = ((global::Doroti.Ui.Offset)(object?)MatrixUtils.transformPoint(inverse, position));
        List<global::Doroti.Framework.Foundation.DiagnosticsNode> children = ((List<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)((List<global::Doroti.Framework.Foundation.DiagnosticsNode>)((dynamic)@object).debugDescribeChildren()));
        for (long i = (checked((long)(children.Count)) - 1L); (i >= 0L); i -= 1L)
        {
            global::Doroti.Framework.Foundation.DiagnosticsNode diagnostics = children[(int)(i)];
            if (((object.Equals(((global::Doroti.Framework.Foundation.DiagnosticsNode)diagnostics).style, global::Doroti.Framework.Foundation.DiagnosticsTreeStyle.offstage)) || (((global::Doroti.Framework.Foundation.DiagnosticsNode)diagnostics).value is not global::Doroti.Framework.Rendering.RenderObject)))
            {
                continue;
            }
            var child = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)((global::Doroti.Framework.Foundation.DiagnosticsNode)diagnostics).value!)!;
            global::Doroti.Ui.Rect? paintClip = ((global::Doroti.Ui.Rect?)(object?)((Rect?)((dynamic)@object).describeApproximatePaintClip(child)));
            if (((paintClip is not null) && !DartRuntimePrimitives.RequireValue(paintClip).contains(localPosition)))
            {
                Rect paintClip__106714__value106780 = DartRuntimePrimitives.RequireValue(paintClip);
                continue;
            }
            Matrix4 childTransform = transform.clone();
            ((dynamic)@object).applyPaintTransform(child, childTransform);
            if (_hitTestHelper(hits, edgeHits, position, child, childTransform))
            {
                hit = true;
            }
        }
        global::Doroti.Ui.Rect bounds = ((global::Doroti.Ui.Rect)(object?)((global::Doroti.Framework.Rendering.RenderObject)@object).semanticBounds);
        if (bounds.contains(localPosition))
        {
            hit = true;
            if (!bounds.deflate(_edgeHitMargin).contains(localPosition))
            {
                edgeHits.Add(@object);
            }
        }
        if (hit)
        {
            hits.Add(@object);
        }
        return hit;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Framework.Rendering.RenderObject> hitTest(Offset position, global::Doroti.Framework.Rendering.RenderObject root)
    {
        var regularHits = new List<global::Doroti.Framework.Rendering.RenderObject>();
        var edgeHits = new List<global::Doroti.Framework.Rendering.RenderObject>();
        _hitTestHelper(regularHits, edgeHits, position, root, ((Matrix4)((dynamic)root).getTransformTo(((global::Doroti.Framework.Rendering.RenderObject)(object)null))));
        double area(global::Doroti.Framework.Rendering.RenderObject @object)
        {
            global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.RenderObject)@object).semanticBounds.size);
            return (sizeLocal.width * sizeLocal.height);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        regularHits.sort(((a, b) => area(a).CompareTo(area(b))));
        var hits = new HashSet<global::Doroti.Framework.Rendering.RenderObject>();
        return hits.ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _inspectAt(Offset position)
    {
        if (!this._isSelectModeWithSelectionOnTapEnabled)
        {
            return;
        }
        var ignorePointer = ((global::Doroti.Framework.Rendering.RenderIgnorePointer?)(object?)((GlobalKey<IState>)this._ignorePointerKey).currentContext!.findRenderObject()!)!;
        global::Doroti.Framework.Rendering.RenderObject userRender = ((global::Doroti.Framework.Rendering.RenderObject)(object?)((global::Doroti.Framework.Rendering.RenderBox?)((dynamic)ignorePointer).child)!);
        List<global::Doroti.Framework.Rendering.RenderObject> selected = ((List<global::Doroti.Framework.Rendering.RenderObject>)(object?)hitTest(position, userRender));
        this.selection.candidates = Widget_inspectorLibrary._filterInspectorHitCandidatesToModalRouteScope(selected);
    }

    internal virtual void _handlePanDown(global::Doroti.Framework.Gestures.DragDownDetails @event)
    {
        _lastPointerLocation = ((global::Doroti.Framework.Gestures.DragDownDetails)@event).globalPosition;
        _inspectAt(((global::Doroti.Framework.Gestures.DragDownDetails)@event).globalPosition);
    }

    internal virtual void _handlePanUpdate(global::Doroti.Framework.Gestures.DragUpdateDetails @event)
    {
        _lastPointerLocation = ((global::Doroti.Framework.Gestures.DragUpdateDetails)@event).globalPosition;
        _inspectAt(((global::Doroti.Framework.Gestures.DragUpdateDetails)@event).globalPosition);
    }

    internal virtual void _handlePanEnd(global::Doroti.Framework.Gestures.DragEndDetails details)
    {
        global::Doroti.Ui.DorotiView view = ((global::Doroti.Ui.DorotiView)(object?)View.of(this.context));
        global::Doroti.Ui.Rect bounds = ((global::Doroti.Ui.Rect)(object?)((Offset.zero & ((view.physicalSize / view.devicePixelRatio)))).deflate(Widget_inspectorLibrary._kOffScreenMargin));
        if (!bounds.contains(DartRuntimePrimitives.RequireValue(this._lastPointerLocation)))
        {
            this.selection.clear();
        }
        else
        {
            WidgetInspectorService.instance._notifyToolsOfSelection(((InspectorSelection)this.selection).current, restrictToProjectFiles: true);
        }
    }

    internal virtual void _handleTap()
    {
        if (!this._isSelectModeWithSelectionOnTapEnabled)
        {
            return;
        }
        if ((this._lastPointerLocation is not null))
        {
            _inspectAt(DartRuntimePrimitives.RequireValue(this._lastPointerLocation));
            WidgetInspectorService.instance._notifyToolsOfSelection(((InspectorSelection)this.selection).current, restrictToProjectFiles: true);
        }
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Stack(children: new List<Widget> { new GestureDetector(onTap: () => this._handleTap(), onPanDown: (global::System.Action<global::Doroti.Framework.Gestures.DragDownDetails>)this._handlePanDown, onPanEnd: (global::System.Action<global::Doroti.Framework.Gestures.DragEndDetails>)this._handlePanEnd, onPanUpdate: (global::System.Action<global::Doroti.Framework.Gestures.DragUpdateDetails>)this._handlePanUpdate, behavior: global::Doroti.Framework.Rendering.HitTestBehavior.opaque, excludeFromSemantics: true, child: new IgnorePointer(ignoring: this._isSelectModeWithSelectionOnTapEnabled, key: this._ignorePointerKey, child: ((WidgetInspector)this.widget).child)), Positioned.CreateFill(child: new _InspectorOverlay__widget_inspector(selection: this.selection)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EnableWidgetInspectorScope : ProxyWidget
{
    public EnableWidgetInspectorScope(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override Element createElement() => DartRuntimePrimitives.ConvertValue<Element>(new _EnableWidgetInspectorScopeProxyElement__widget_inspector(this));
}

internal class _EnableWidgetInspectorScopeProxyElement__widget_inspector : ProxyElement
{
    internal _EnableWidgetInspectorScopeProxyElement__widget_inspector(ProxyWidget widget) : base(widget)
    {
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
    }

}

public class DisableWidgetInspectorScope : ProxyWidget
{
    public DisableWidgetInspectorScope(global::Doroti.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
    {
    }

    public override Element createElement() => DartRuntimePrimitives.ConvertValue<Element>(new _DisableWidgetInspectorScopeProxyElement__widget_inspector(this));
}

internal class _DisableWidgetInspectorScopeProxyElement__widget_inspector : ProxyElement
{
    internal _DisableWidgetInspectorScopeProxyElement__widget_inspector(ProxyWidget widget) : base(widget)
    {
    }

    public override void notifyClients(ProxyWidget oldWidget)
    {
    }

}

public enum InspectorButtonVariant
{
    filled,
    toggle,
    iconOnly
}

public abstract class InspectorButton : StatelessWidget
{
    public virtual global::System.Action onPressed { get; private set; } = default!;
    public virtual string semanticsLabel { get; private set; } = default!;
    public virtual IconData icon { get; private set; } = default!;
    public virtual GlobalKey<IState>? buttonKey { get; private set; }
    public virtual InspectorButtonVariant variant { get; private set; } = default!;
    public virtual bool? toggledOn { get; private set; }
    public const double buttonSize = 32.0;
    public const double buttonIconSize = 18.0;

    protected InspectorButton(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, GlobalKey<IState>? buttonKey = null, InspectorButtonVariant variant = default!, bool? toggledOn = null) : base(key: key)
    {
        this.onPressed = onPressed;
        this.semanticsLabel = semanticsLabel;
        this.icon = icon;
        this.buttonKey = buttonKey;
        this.variant = variant;
        this.toggledOn = toggledOn;
    }

    protected InspectorButton(global::System.Action onPressed, string semanticsLabel, IconData icon, GlobalKey<IState>? buttonKey = null)
        : this(null, onPressed, semanticsLabel, icon, buttonKey) { }

    protected static InspectorButton CreateFilled(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, GlobalKey<IState>? buttonKey = null)
    {
        throw new InvalidOperationException("Dart abstract constructors cannot be invoked directly.");
    }

    protected static InspectorButton CreateToggle(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, bool toggledOn = true)
    {
        throw new InvalidOperationException("Dart abstract constructors cannot be invoked directly.");
    }

    protected static InspectorButton CreateIconOnly(global::Doroti.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!)
    {
        throw new InvalidOperationException("Dart abstract constructors cannot be invoked directly.");
    }

    public virtual double iconSizeForVariant
    {
        get
        {
            switch (this.variant)
            {
                case InspectorButtonVariant.iconOnly:
                    {
                        return buttonSize;
                    }
                case InspectorButtonVariant.filled:
                case InspectorButtonVariant.toggle:
                    {
                        return buttonIconSize;
                    }
                default:
                    throw new InvalidOperationException("Non-exhaustive Dart switch value.");
            }
            return default!;
        }
    }
    public abstract global::Doroti.Ui.Color foregroundColor(BuildContext context);
    public abstract global::Doroti.Ui.Color backgroundColor(BuildContext context);
    public abstract override Widget build(BuildContext context);
}

public class InspectorSelection : ChangeNotifier
{
    internal virtual List<global::Doroti.Framework.Rendering.RenderObject> _candidates { get; set; } = new List<global::Doroti.Framework.Rendering.RenderObject>();
    internal virtual long _index { get; set; } = 0L;
    internal virtual global::Doroti.Framework.Rendering.RenderObject? _current { get; set; } = default;
    internal virtual Element? _currentElement { get; set; } = default;

    public InspectorSelection()
    {
    }

    public virtual List<global::Doroti.Framework.Rendering.RenderObject> candidates
    {
        get => this._candidates;
        set
        {
            var __value = value;
            _candidates = __value;
            _index = 0L;
            _computeCurrent();
        }
    }
    public virtual long index
    {
        get => this._index;
        set
        {
            var __value = value;
            _index = DartRuntimePrimitives.RequireValue(__value);
            _computeCurrent();
        }
    }
    public virtual void clear()
    {
        _candidates = new List<global::Doroti.Framework.Rendering.RenderObject>();
        _index = 0L;
        _computeCurrent();
    }

    public virtual void clearCandidates()
    {
        if (!System.Linq.Enumerable.Any(this._candidates))
        {
            return;
        }
        _candidates = new List<global::Doroti.Framework.Rendering.RenderObject>();
        _index = 0L;
    }

    public virtual global::Doroti.Framework.Rendering.RenderObject? current
    {
        get => (this.active ? this._current : null);
        set
        {
            var __value = value;
            if ((!object.Equals(this._current, __value)))
            {
                _current = __value;
                _currentElement = Widget_inspectorLibrary._elementForRenderObject(__value);
                notifyListeners();
            }
        }
    }
    public virtual Element? currentElement
    {
        get
        {
            return ((this._currentElement?.debugIsDefunct ?? true) ? null : this._currentElement);
            return default!;
        }
        set
        {
            var element = value;
            if ((element?.debugIsDefunct ?? false))
            {
                _currentElement = null;
                _current = null;
                notifyListeners();
                return;
            }
            if ((!object.Equals(this.currentElement, element)))
            {
                _currentElement = element;
                _current = element?.findRenderObject();
                notifyListeners();
            }
        }
    }
    internal virtual void _computeCurrent()
    {
        if ((this._index < checked((long)(this.candidates.Count))))
        {
            _current = this.candidates[(int)(this.index)];
            _currentElement = (((DebugCreator?)(object?)((dynamic)this._current)?.debugCreator)!)?.element;
            notifyListeners();
        }
        else
        {
            _current = null;
            _currentElement = null;
            notifyListeners();
        }
    }

    public virtual bool active => DartRuntimePrimitives.ConvertValue<bool>(((this._current is not null) && ((bool)((dynamic)this._current!).attached)));
}

internal class _InspectorOverlay__widget_inspector : LeafRenderObjectWidget
{
    public virtual InspectorSelection selection { get; private set; } = default!;

    internal _InspectorOverlay__widget_inspector(InspectorSelection selection)
    {
        this.selection = selection;
    }

    public override global::Doroti.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Framework.Rendering.RenderObject)(object?)new _RenderInspectorOverlay__widget_inspector(selection: this.selection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInspectorOverlay__widget_inspector)(object)renderObject;
        __renderObject.selection = this.selection;
    }

}

public class _RenderInspectorOverlay__widget_inspector : global::Doroti.Framework.Rendering.RenderBox
{
    internal virtual InspectorSelection _selection { get; set; } = default!;

    internal _RenderInspectorOverlay__widget_inspector(InspectorSelection selection)
    {
        this._selection = selection;
    }

    public virtual InspectorSelection selection
    {
        get => this._selection;
        set
        {
            var __value = value;
            if ((!object.Equals(__value, this._selection)))
            {
                _selection = __value;
            }
            markNeedsPaint();
        }
    }
    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override Size computeDryLayout(global::Doroti.Framework.Rendering.BoxConstraints constraints)
    {
        return constraints.constrain(Size.infinite);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => this.needsCompositing);
        context.addLayer(new _InspectorOverlayLayer__widget_inspector(overlayRect: global::Doroti.Ui.Rect.fromLTWH(offset.dx, offset.dy, this.size.width, this.size.height), selection: this.selection, rootRenderObject: (true ? this.parent! : null)));
    }

}

public class _TransformedRect__widget_inspector
{
    public virtual Rect rect { get; private set; } = default!;
    public virtual Matrix4 transform { get; private set; } = default!;

    internal _TransformedRect__widget_inspector(global::Doroti.Framework.Rendering.RenderObject @object, global::Doroti.Framework.Rendering.RenderObject? ancestor)
    {
        this.rect = ((global::Doroti.Framework.Rendering.RenderObject)@object).semanticBounds;
        this.transform = ((Matrix4)((dynamic)@object).getTransformTo(ancestor));
    }

    public override bool Equals(object? other)
    {
        var __other = other as _TransformedRect__widget_inspector;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((__other is _TransformedRect__widget_inspector) && (object.Equals(((_TransformedRect__widget_inspector)((_TransformedRect__widget_inspector)__other)).rect, this.rect))) && (object.Equals(((_TransformedRect__widget_inspector)((_TransformedRect__widget_inspector)__other)).transform, this.transform)));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.rect, this.transform));
}

internal class _InspectorOverlayRenderState__widget_inspector
{
    public virtual Rect overlayRect { get; private set; } = default!;
    public virtual _TransformedRect__widget_inspector selected { get; private set; } = default!;
    public virtual List<_TransformedRect__widget_inspector> candidates { get; private set; } = default!;
    public virtual string tooltip { get; private set; } = default!;
    public virtual TextDirection textDirection { get; private set; } = default!;

    internal _InspectorOverlayRenderState__widget_inspector(Rect overlayRect, _TransformedRect__widget_inspector selected, List<_TransformedRect__widget_inspector> candidates, string tooltip, TextDirection textDirection)
    {
        this.overlayRect = overlayRect;
        this.selected = selected;
        this.candidates = candidates;
        this.tooltip = tooltip;
        this.textDirection = textDirection;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _InspectorOverlayRenderState__widget_inspector;
        if (__other is null) return false;
        if ((!object.Equals(DartRuntimePrimitives.RuntimeType(__other), this.GetType())))
        {
            return false;
        }
        return (((((__other is _InspectorOverlayRenderState__widget_inspector) && (object.Equals(((_InspectorOverlayRenderState__widget_inspector)((_InspectorOverlayRenderState__widget_inspector)__other)).overlayRect, this.overlayRect))) && (object.Equals(((_InspectorOverlayRenderState__widget_inspector)((_InspectorOverlayRenderState__widget_inspector)__other)).selected, this.selected))) && global::Doroti.Framework.Foundation.CollectionsLibrary.listEquals<_TransformedRect__widget_inspector>(((_InspectorOverlayRenderState__widget_inspector)((_InspectorOverlayRenderState__widget_inspector)__other)).candidates, this.candidates)) && (((_InspectorOverlayRenderState__widget_inspector)((_InspectorOverlayRenderState__widget_inspector)__other)).tooltip == this.tooltip));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.overlayRect, this.selected, FoundationRuntimePorts.ObjectHashAll(this.candidates), this.tooltip));
}

public static partial class Widget_inspectorLibrary
{
    internal static long _kMaxTooltipLines = 5L;
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kTooltipBackgroundColor = global::Doroti.Ui.Color.fromARGB(230L, 60L, 60L, 60L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kHighlightedRenderObjectFillColor = global::Doroti.Ui.Color.fromARGB(128L, 128L, 128L, 255L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kHighlightedRenderObjectBorderColor = global::Doroti.Ui.Color.fromARGB(128L, 64L, 64L, 128L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Element? _elementForRenderObject(global::Doroti.Framework.Rendering.RenderObject? @object)
    {
        object? creator = ((dynamic)@object)?.debugCreator;
        if ((creator is DebugCreator))
        {
            DebugCreator creator__124233__as124271 = (DebugCreator)creator;
            return ((DebugCreator)((DebugCreator)creator__124233__as124271)).element;
        }
        return ((Element)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static dynamic _modalRouteForRenderObject(global::Doroti.Framework.Rendering.RenderObject? @object)
    {
        Element? element = Widget_inspectorLibrary._elementForRenderObject(@object);
        if ((element is null))
        {
            return null;
        }
        return ModalRoute<object>.of<object?>(element);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static double _inspectorHitArea(global::Doroti.Framework.Rendering.RenderObject @object)
    {
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Rendering.RenderObject)@object).semanticBounds.size);
        return (sizeLocal.width * sizeLocal.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static dynamic _inspectorScopeRouteForHits(List<global::Doroti.Framework.Rendering.RenderObject> hits)
    {
        foreach (var hit in hits)
        {
            dynamic route = Widget_inspectorLibrary._modalRouteForRenderObject(hit);
            if ((((bool?)((dynamic)route)?.isCurrent) ?? false))
            {
                return route;
            }
        }
        global::Doroti.Framework.Rendering.RenderObject? smallestHit = default!;
        double smallestArea = double.PositiveInfinity;
        foreach (var hitLocal in hits)
        {
            dynamic routeLocal = Widget_inspectorLibrary._modalRouteForRenderObject(hitLocal);
            if ((routeLocal is null))
            {
                continue;
            }
            double area = Widget_inspectorLibrary._inspectorHitArea(hitLocal);
            if ((area < smallestArea))
            {
                smallestArea = area;
                smallestHit = hitLocal;
            }
        }
        if ((smallestHit is not null))
        {
            return Widget_inspectorLibrary._modalRouteForRenderObject(smallestHit);
        }
        return Widget_inspectorLibrary._modalRouteForRenderObject(hits.First());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static List<global::Doroti.Framework.Rendering.RenderObject> _filterInspectorHitCandidatesToModalRouteScope(List<global::Doroti.Framework.Rendering.RenderObject> hits)
    {
        if (!System.Linq.Enumerable.Any(hits))
        {
            return hits;
        }
        List<global::Doroti.Framework.Rendering.RenderObject> onstageHits = hits.where(((hit) =>
        {
            dynamic route = Widget_inspectorLibrary._modalRouteForRenderObject(hit);
            return ((route is null) || !((bool)((dynamic)route).offstage));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList().ToList();
        if (!System.Linq.Enumerable.Any(onstageHits))
        {
            return onstageHits;
        }
        dynamic scopeRoute = Widget_inspectorLibrary._inspectorScopeRouteForHits(onstageHits);
        List<global::Doroti.Framework.Rendering.RenderObject> scopedHits = onstageHits.where(((hit) => DartRuntimePrimitives.Identical(Widget_inspectorLibrary._modalRouteForRenderObject(hit), scopeRoute))).ToList().ToList();
        scopedHits.sort(((a, b) => Widget_inspectorLibrary._inspectorHitArea(a).CompareTo(Widget_inspectorLibrary._inspectorHitArea(b))));
        return scopedHits;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _InspectorOverlayLayer__widget_inspector : global::Doroti.Framework.Rendering.Layer
{
    public virtual InspectorSelection selection { get; set; } = default!;
    public virtual Rect overlayRect { get; private set; } = default!;
    public virtual global::Doroti.Framework.Rendering.RenderObject? rootRenderObject { get; private set; }
    internal virtual _InspectorOverlayRenderState__widget_inspector? _lastState { get; set; } = default;
    internal virtual Picture? _picture { get; set; } = default;
    internal virtual global::Doroti.Framework.Painting.TextPainter? _textPainter { get; set; } = default;
    internal virtual double? _textPainterMaxWidth { get; set; } = default;

    internal _InspectorOverlayLayer__widget_inspector(Rect overlayRect, InspectorSelection selection, global::Doroti.Framework.Rendering.RenderObject? rootRenderObject)
    {
        this.overlayRect = overlayRect;
        this.selection = selection;
        this.rootRenderObject = rootRenderObject;
    }

    public override void dispose()
    {
        this._textPainter?.dispose();
        _textPainter = null;
        this._picture?.dispose();
        base.dispose();
    }

    public override void addToScene(SceneBuilder builder)
    {
        if (!((InspectorSelection)this.selection).active)
        {
            return;
        }
        global::Doroti.Framework.Rendering.RenderObject selectedLocal = ((InspectorSelection)this.selection).current!;
        if (!_isInInspectorRenderObjectTree(selectedLocal))
        {
            return;
        }
        var candidatesLocal = new List<_TransformedRect__widget_inspector>();
        foreach (global::Doroti.Framework.Rendering.RenderObject candidate in ((InspectorSelection)this.selection).candidates)
        {
            if (((((object.Equals(candidate, selectedLocal)) || !((global::Doroti.Framework.Rendering.RenderObject)candidate).attached) || !_isInInspectorRenderObjectTree(candidate)) || !DartRuntimePrimitives.Identical(Widget_inspectorLibrary._modalRouteForRenderObject(candidate), Widget_inspectorLibrary._modalRouteForRenderObject(selectedLocal))))
            {
                continue;
            }
            candidatesLocal.Add(new _TransformedRect__widget_inspector(candidate, this.rootRenderObject));
        }
        var selectedRect = new _TransformedRect__widget_inspector(selectedLocal, this.rootRenderObject);
        string widgetName = ((string)(object?)((Diagnosticable)((InspectorSelection)this.selection).currentElement!).toStringShort());
        string widthLocal = ((_TransformedRect__widget_inspector)selectedRect).rect.width.toStringAsFixed(1L);
        string heightLocal = ((_TransformedRect__widget_inspector)selectedRect).rect.height.toStringAsFixed(1L);
        var state = new _InspectorOverlayRenderState__widget_inspector(overlayRect: this.overlayRect, selected: selectedRect, tooltip: $"{widgetName} ({widthLocal} x {heightLocal})", textDirection: TextDirection.ltr, candidates: candidatesLocal);
        if ((!object.Equals(state, this._lastState)))
        {
            _lastState = state;
            this._picture?.dispose();
            _picture = _buildPicture(state);
        }
        builder.addPicture(Offset.zero, this._picture!);
    }

    internal virtual global::Doroti.Ui.Picture _buildPicture(_InspectorOverlayRenderState__widget_inspector state)
    {
        var recorder = new global::Doroti.Ui.PictureRecorder();
        var canvas = new global::Doroti.Ui.Canvas(recorder, ((_InspectorOverlayRenderState__widget_inspector)state).overlayRect);
        global::Doroti.Ui.Size sizeLocal = ((global::Doroti.Ui.Size)(object?)((_InspectorOverlayRenderState__widget_inspector)state).overlayRect.size);
        canvas.translate(((_InspectorOverlayRenderState__widget_inspector)state).overlayRect.left, ((_InspectorOverlayRenderState__widget_inspector)state).overlayRect.top);
        var fillPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = Widget_inspectorLibrary._kHighlightedRenderObjectFillColor;
    return __cascade;
}))();
        var borderPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = Widget_inspectorLibrary._kHighlightedRenderObjectBorderColor;
    return __cascade;
}))();
        global::Doroti.Ui.Rect selectedPaintRect = ((global::Doroti.Ui.Rect)(object?)((_InspectorOverlayRenderState__widget_inspector)state).selected.rect.deflate(0.5));
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = canvas;
    __cascade.save();
    __cascade.transform(((_InspectorOverlayRenderState__widget_inspector)state).selected.transform.storage);
    __cascade.drawRect(selectedPaintRect, fillPaint);
    __cascade.drawRect(selectedPaintRect, borderPaint);
    __cascade.restore();
    return __cascade;
}))());
        foreach (_TransformedRect__widget_inspector transformedRect in ((_InspectorOverlayRenderState__widget_inspector)state).candidates)
        {
            DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = canvas;
    __cascade.save();
    __cascade.transform(((_TransformedRect__widget_inspector)transformedRect).transform.storage);
    __cascade.drawRect(((_TransformedRect__widget_inspector)transformedRect).rect.deflate(0.5), borderPaint);
    __cascade.restore();
    return __cascade;
}))());
        }
        global::Doroti.Ui.Rect targetRect = ((global::Doroti.Ui.Rect)(object?)MatrixUtils.transformRect(((_InspectorOverlayRenderState__widget_inspector)state).selected.transform, ((_InspectorOverlayRenderState__widget_inspector)state).selected.rect));
        if (!targetRect.hasNaN)
        {
            var target = new global::Doroti.Ui.Offset(targetRect.left, ((Offset)((dynamic)targetRect).center).dy);
            var offsetFromWidget = 9.0;
            double verticalOffset = ((targetRect.height / 2L) + offsetFromWidget);
            _paintDescription(canvas, ((_InspectorOverlayRenderState__widget_inspector)state).tooltip, ((_InspectorOverlayRenderState__widget_inspector)state).textDirection, target, verticalOffset, sizeLocal, targetRect);
        }
        return ((global::Doroti.Ui.Picture)(object?)recorder.endRecording());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintDescription(Canvas canvas, string message, TextDirection textDirection, Offset target, double verticalOffset, Size size, Rect targetRect)
    {
        canvas.save();
        double maxWidthLocal = Math.Max((size.width - (2L * ((Widget_inspectorLibrary._kScreenEdgeMargin + Widget_inspectorLibrary._kTooltipPadding)))), 0);
        var textSpan = ((global::Doroti.Framework.Painting.TextSpan?)(object?)this._textPainter?.text)!;
        if ((((this._textPainter is null) || (textSpan!.text != message)) || (this._textPainterMaxWidth != maxWidthLocal)))
        {
            _textPainterMaxWidth = maxWidthLocal;
            this._textPainter?.dispose();
            _textPainter = ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter();
    __cascade.maxLines = Widget_inspectorLibrary._kMaxTooltipLines;
    __cascade.ellipsis = "...";
    __cascade.text = new global::Doroti.Framework.Painting.TextSpan(style: Widget_inspectorLibrary._messageStyle, text: message);
    __cascade.textDirection = textDirection;
    __cascade.layout(maxWidth: maxWidthLocal);
    return __cascade;
}))();
        }
        global::Doroti.Ui.Size tooltipSize = ((global::Doroti.Ui.Size)(object?)(this._textPainter!.size + new global::Doroti.Ui.Offset((Widget_inspectorLibrary._kTooltipPadding * 2L), (Widget_inspectorLibrary._kTooltipPadding * 2L))));
        global::Doroti.Ui.Offset tipOffset = ((global::Doroti.Ui.Offset)(object?)global::Doroti.Framework.Painting.GeometryLibrary.positionDependentBox(size: size, childSize: tooltipSize, target: target, verticalOffset: verticalOffset, preferBelow: false));
        var tooltipBackground = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = Widget_inspectorLibrary._kTooltipBackgroundColor;
    return __cascade;
}))();
        canvas.drawRect(global::Doroti.Ui.Rect.fromPoints(tipOffset, tipOffset.translate(tooltipSize.width, tooltipSize.height)), tooltipBackground);
        double wedgeY = tipOffset.dy;
        bool tooltipBelow = (tipOffset.dy > target.dy);
        if (!tooltipBelow)
        {
            wedgeY += tooltipSize.height;
        }
        double wedgeSize = (Widget_inspectorLibrary._kTooltipPadding * 2L);
        double wedgeX = (Math.Max(tipOffset.dx, target.dx) + (wedgeSize * 2L));
        wedgeX = Math.Min(wedgeX, ((tipOffset.dx + tooltipSize.width) - (wedgeSize * 2L)));
        var wedge = new List<global::Doroti.Ui.Offset> { new global::Doroti.Ui.Offset((wedgeX - wedgeSize), wedgeY), new global::Doroti.Ui.Offset((wedgeX + wedgeSize), wedgeY), new global::Doroti.Ui.Offset(wedgeX, (wedgeY + ((tooltipBelow ? -wedgeSize : wedgeSize)))) };
        canvas.drawPath(((Func<Path>)(() =>
{
    var __cascade = new global::Doroti.Ui.Path();
    __cascade.addPolygon(wedge, true);
    return __cascade;
}))(), tooltipBackground);
        this._textPainter!.paint(canvas, (tipOffset + new global::Doroti.Ui.Offset(Widget_inspectorLibrary._kTooltipPadding, Widget_inspectorLibrary._kTooltipPadding)));
        canvas.restore();
    }

    public override bool findAnnotations<S>(global::Doroti.Framework.Rendering.AnnotationResult<S> result, Offset localPosition, bool onlyFirst = default!)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isInInspectorRenderObjectTree(global::Doroti.Framework.Rendering.RenderObject child)
    {
        global::Doroti.Framework.Rendering.RenderObject? current = ((global::Doroti.Framework.Rendering.RenderObject)child).parent;
        while ((current is not null))
        {
            if (((current is global::Doroti.Framework.Rendering.RenderStack) && ((global::Doroti.Framework.Rendering.RenderStack)current).getChildrenAsList().any(((child) => (child is _RenderInspectorOverlay__widget_inspector)))))
            {
                global::Doroti.Framework.Rendering.RenderStack current__134258__as134376 = (global::Doroti.Framework.Rendering.RenderStack)current;
                return (object.Equals(this.rootRenderObject, ((global::Doroti.Framework.Rendering.RenderStack)current__134258__as134376)));
            }
            current = ((global::Doroti.Framework.Rendering.RenderObject)current).parent;
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static double _kScreenEdgeMargin = 10.0;
}

public static partial class Widget_inspectorLibrary
{
    internal static double _kTooltipPadding = 5.0;
}

public static partial class Widget_inspectorLibrary
{
    internal static double _kOffScreenMargin = 1.0;
}

public static partial class Widget_inspectorLibrary
{
    internal static global::Doroti.Framework.Painting.TextStyle _messageStyle = new global::Doroti.Framework.Painting.TextStyle(color: new global::Doroti.Ui.Color(4294967295L), fontSize: 10.0, height: 1.2);
}

internal class _WidgetInspectorButtonGroup__widget_inspector : StatefulWidget
{
    public virtual ExitWidgetSelectionButtonBuilder exitWidgetSelectionButtonBuilder { get; private set; } = default!;
    public virtual MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder { get; private set; }
    public virtual TapBehaviorButtonBuilder? tapBehaviorButtonBuilder { get; private set; }

    internal _WidgetInspectorButtonGroup__widget_inspector(ExitWidgetSelectionButtonBuilder exitWidgetSelectionButtonBuilder, MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder, TapBehaviorButtonBuilder? tapBehaviorButtonBuilder)
    {
        this.exitWidgetSelectionButtonBuilder = exitWidgetSelectionButtonBuilder;
        this.moveExitWidgetSelectionButtonBuilder = moveExitWidgetSelectionButtonBuilder;
        this.tapBehaviorButtonBuilder = tapBehaviorButtonBuilder;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WidgetInspectorButtonGroupState__widget_inspector());
}

internal class _WidgetInspectorButtonGroupState__widget_inspector : State<_WidgetInspectorButtonGroup__widget_inspector>
{
    internal const double _kExitWidgetSelectionButtonMargin = 10.0;
    internal const bool _defaultSelectionOnTapEnabled = true;
    internal virtual GlobalKey<IState> _exitWidgetSelectionButtonKey { get; private set; } = GlobalKey<IState>.Create(debugLabel: "Exit Widget Selection button");
    internal virtual string? _tooltipMessage { get; set; } = default;
    internal virtual bool _usesDefaultAlignment { get; set; } = true;

    internal virtual global::Doroti.Framework.Foundation.ValueNotifier<bool> _selectionOnTapEnabled => WidgetsBinding.instance.debugWidgetInspectorSelectionOnTapEnabled;
    internal virtual Widget? _moveExitWidgetSelectionButton
    {
        get
        {
            MoveExitWidgetSelectionButtonBuilder? buttonBuilder = ((_WidgetInspectorButtonGroup__widget_inspector)this.widget).moveExitWidgetSelectionButtonBuilder;
            if ((buttonBuilder is null))
            {
                return ((Widget)(object)null);
            }
            global::Doroti.Ui.TextDirection textDirection = Directionality.of(this.context);
            var buttonLabel = $"Move to the {((this._usesDefaultAlignment == ((object.Equals(textDirection, TextDirection.ltr)))) ? "right" : "left")}";
            return ((Widget?)(object?)new _WidgetInspectorButton__widget_inspector(button: buttonBuilder(this.context, onPressed: (() =>
            {
                _changeButtonGroupAlignment();
                _onTooltipHidden();
            }), semanticsLabel: buttonLabel, usesDefaultAlignment: this._usesDefaultAlignment), onTooltipVisible: ((global::System.Action)(() =>
            {
                _changeTooltipMessage(buttonLabel);
            })), onTooltipHidden: () => this._onTooltipHidden()));
            return default!;
        }
    }
    internal virtual Widget _exitWidgetSelectionButton
    {
        get
        {
            var buttonLabel = "Exit Select Widget mode";
            return ((Widget)(object?)new _WidgetInspectorButton__widget_inspector(button: this.widget.exitWidgetSelectionButtonBuilder(this.context, onPressed: this._exitWidgetSelectionMode, semanticsLabel: buttonLabel, key: this._exitWidgetSelectionButtonKey), onTooltipVisible: ((global::System.Action)(() =>
            {
                _changeTooltipMessage(buttonLabel);
            })), onTooltipHidden: () => this._onTooltipHidden()));
            return default!;
        }
    }
    internal virtual Widget? _tapBehaviorButton
    {
        get
        {
            TapBehaviorButtonBuilder? buttonBuilder = ((_WidgetInspectorButtonGroup__widget_inspector)this.widget).tapBehaviorButtonBuilder;
            if ((buttonBuilder is null))
            {
                return ((Widget)(object)null);
            }
            return ((Widget?)(object?)new _WidgetInspectorButton__widget_inspector(button: buttonBuilder(this.context, onPressed: () => this._changeSelectionOnTapMode(default), semanticsLabel: "Change widget selection mode for taps", selectionOnTapEnabled: ((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this._selectionOnTapEnabled).value), onTooltipVisible: () => this._changeSelectionOnTapTooltip(), onTooltipHidden: () => this._onTooltipHidden()));
            return default!;
        }
    }
    internal virtual bool _tooltipVisible => DartRuntimePrimitives.ConvertValue<bool>((this._tooltipMessage is not null));
    public override Widget build(BuildContext context)
    {
        double bottomPadding = Math.Max(_kExitWidgetSelectionButtonMargin, MediaQuery.viewPaddingOf(context).bottom);
        Widget selectionModeButtons = ((Widget)(object?)new Column(children: new List<Widget> { this._exitWidgetSelectionButton }));
        Widget buttonGroup = ((Widget)(object?)new Stack(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.topCenter, children: new List<Widget> { new CustomPaint(painter: new _ExitWidgetSelectionTooltipPainter__widget_inspector(tooltipMessage: this._tooltipMessage, buttonKey: this._exitWidgetSelectionButtonKey, usesDefaultAlignment: this._usesDefaultAlignment)), new Row(crossAxisAlignment: global::Doroti.Framework.Rendering.CrossAxisAlignment.end, mainAxisAlignment: global::Doroti.Framework.Rendering.MainAxisAlignment.center, children: new List<Widget>()) }));
        return ((Widget)(object?)Positioned.CreateDirectional(textDirection: Directionality.of(context), start: (this._usesDefaultAlignment ? _kExitWidgetSelectionButtonMargin : null), end: (this._usesDefaultAlignment ? null : _kExitWidgetSelectionButtonMargin), bottom: bottomPadding, child: buttonGroup));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _exitWidgetSelectionMode()
    {
        WidgetInspectorService.instance._changeWidgetSelectionMode(false);
        _changeSelectionOnTapMode(selectionOnTapEnabled: _defaultSelectionOnTapEnabled);
    }

    internal virtual void _changeSelectionOnTapMode(bool? selectionOnTapEnabled = null)
    {
        bool newValue = (selectionOnTapEnabled ?? !((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this._selectionOnTapEnabled).value);
        this._selectionOnTapEnabled.value = newValue;
        WidgetInspectorService.instance.selection.clear();
        if (this._tooltipVisible)
        {
            _changeSelectionOnTapTooltip();
        }
    }

    internal virtual void _changeSelectionOnTapTooltip()
    {
        _changeTooltipMessage((((global::Doroti.Framework.Foundation.ValueNotifier<bool>)this._selectionOnTapEnabled).value ? "Disable widget selection for taps" : "Enable widget selection for taps"));
    }

    internal virtual void _changeButtonGroupAlignment()
    {
        if (this.mounted)
        {
            setState(((global::System.Action)(() =>
            {
                _usesDefaultAlignment = !this._usesDefaultAlignment;
            })));
        }
    }

    internal virtual void _onTooltipHidden()
    {
        _changeTooltipMessage(((string)(object)null));
    }

    internal virtual void _changeTooltipMessage(string? message)
    {
        if (this.mounted)
        {
            setState(((global::System.Action)(() =>
            {
                _tooltipMessage = message;
            })));
        }
    }

}

internal class _WidgetInspectorButton__widget_inspector : StatefulWidget
{
    public virtual Widget button { get; private set; } = default!;
    public virtual global::System.Action onTooltipVisible { get; private set; } = default!;
    public virtual global::System.Action onTooltipHidden { get; private set; } = default!;
    internal static Duration _tooltipShownOnLongPressDuration = Duration.Create(milliseconds: 1500L);
    internal static Duration _tooltipDelayDuration = Duration.Create(milliseconds: 100L);

    internal _WidgetInspectorButton__widget_inspector(Widget button, global::System.Action onTooltipVisible, global::System.Action onTooltipHidden)
    {
        this.button = button;
        this.onTooltipVisible = onTooltipVisible;
        this.onTooltipHidden = onTooltipHidden;
    }

    public override IState createState() => DartRuntimePrimitives.ConvertValue<IState>(new _WidgetInspectorButtonState__widget_inspector());
}

internal class _WidgetInspectorButtonState__widget_inspector : State<_WidgetInspectorButton__widget_inspector>
{
    internal virtual Timer? _tooltipVisibleTimer { get; set; } = default;
    internal virtual Timer? _tooltipHiddenTimer { get; set; } = default;

    public override void dispose()
    {
        this._tooltipVisibleTimer?.cancel();
        _tooltipVisibleTimer = null;
        this._tooltipHiddenTimer?.cancel();
        _tooltipHiddenTimer = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return ((Widget)(object?)new Stack(alignment: global::Doroti.Framework.Painting.AlignmentDirectional.topCenter, children: new List<Widget> { new GestureDetector(onLongPress: ((global::System.Action)(() => {
_tooltipVisibleAfter(_WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
_tooltipHiddenAfter((_WidgetInspectorButton__widget_inspector._tooltipShownOnLongPressDuration + _WidgetInspectorButton__widget_inspector._tooltipDelayDuration));
})), child: new MouseRegion(onEnter: ((global::System.Action<global::Doroti.Framework.Gestures.PointerEnterEvent>)((_) => {
_tooltipVisibleAfter(_WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
})), onExit: ((global::System.Action<global::Doroti.Framework.Gestures.PointerExitEvent>)((_) => {
_tooltipHiddenAfter(_WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
})), child: ((_WidgetInspectorButton__widget_inspector)this.widget).button)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _tooltipVisibleAfter(Duration duration)
    {
        _tooltipVisibilityChangedAfter(duration, isVisible: true);
    }

    internal virtual void _tooltipHiddenAfter(Duration duration)
    {
        _tooltipVisibilityChangedAfter(duration, isVisible: false);
    }

    internal virtual void _tooltipVisibilityChangedAfter(Duration duration, bool isVisible)
    {
        Timer? timer = (isVisible ? this._tooltipVisibleTimer : this._tooltipHiddenTimer);
        if ((timer?.isActive ?? false))
        {
            timer!.cancel();
        }
        if (isVisible)
        {
            _tooltipVisibleTimer = new Timer(duration, (() =>
            {
                this.widget.onTooltipVisible();
            }));
        }
        else
        {
            _tooltipHiddenTimer = new Timer(duration, (() =>
            {
                this.widget.onTooltipHidden();
            }));
        }
    }

}

internal class _ExitWidgetSelectionTooltipPainter__widget_inspector : global::Doroti.Framework.Rendering.CustomPainter
{
    public virtual string? tooltipMessage { get; private set; }
    public virtual GlobalKey<IState> buttonKey { get; private set; } = default!;
    public virtual bool usesDefaultAlignment { get; private set; } = default!;

    internal _ExitWidgetSelectionTooltipPainter__widget_inspector(string? tooltipMessage, GlobalKey<IState> buttonKey, bool usesDefaultAlignment)
    {
        this.tooltipMessage = tooltipMessage;
        this.buttonKey = buttonKey;
        this.usesDefaultAlignment = usesDefaultAlignment;
    }

    public override void paint(Canvas canvas, Size size)
    {
        var isVisible = (this.tooltipMessage is not null);
        if (!isVisible)
        {
            return;
        }
        global::Doroti.Framework.Rendering.RenderObject? buttonRenderObject = ((global::Doroti.Framework.Rendering.RenderObject?)(object?)((GlobalKey<IState>)this.buttonKey).currentContext?.findRenderObject());
        if ((buttonRenderObject is null))
        {
            return;
        }
        var tooltipPadding = 4.0;
        var tooltipSpacing = 6.0;
        var tooltipTextPainter = ((Func<global::Doroti.Framework.Painting.TextPainter>)(() =>
{
    var __cascade = new global::Doroti.Framework.Painting.TextPainter();
    __cascade.maxLines = 1L;
    __cascade.ellipsis = "...";
    __cascade.text = new global::Doroti.Framework.Painting.TextSpan(text: this.tooltipMessage, style: Widget_inspectorLibrary._messageStyle);
    __cascade.textDirection = TextDirection.ltr;
    __cascade.layout();
    return __cascade;
}))();
        var tooltipPaint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = Widget_inspectorLibrary._kTooltipBackgroundColor;
    return __cascade;
}))();
        double buttonWidth = ((global::Doroti.Framework.Rendering.RenderObject)buttonRenderObject).paintBounds.width;
        global::Doroti.Ui.Size textSize = ((global::Doroti.Ui.Size)(object?)((global::Doroti.Framework.Painting.TextPainter)tooltipTextPainter).size);
        double textWidth = textSize.width;
        double textHeight = textSize.height;
        double tooltipWidth = (textWidth + ((tooltipPadding * 2L)));
        double tooltipHeight = (textHeight + ((tooltipPadding * 2L)));
        double tooltipXOffset = (this.usesDefaultAlignment ? (0L - buttonWidth) : (0L - ((tooltipWidth - buttonWidth))));
        double tooltipYOffset = ((0L - tooltipHeight) - tooltipSpacing);
        canvas.drawRect(global::Doroti.Ui.Rect.fromLTWH(tooltipXOffset, tooltipYOffset, tooltipWidth, tooltipHeight), tooltipPaint);
        tooltipTextPainter.paint(canvas, new global::Doroti.Ui.Offset((tooltipXOffset + tooltipPadding), (tooltipYOffset + tooltipPadding)));
    }

    public override bool shouldRepaint(global::Doroti.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_ExitWidgetSelectionTooltipPainter__widget_inspector)(object)oldDelegate;
        return (this.tooltipMessage != ((_ExitWidgetSelectionTooltipPainter__widget_inspector)__oldDelegate).tooltipMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static bool _isDebugCreator(global::Doroti.Framework.Foundation.DiagnosticsNode node) => (node is global::Doroti.Framework.Rendering.DiagnosticsDebugCreator);
}

public static partial class Widget_inspectorLibrary
{
    public static IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode> debugTransformDebugCreator(IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode> properties)
    {
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode)
        {
            return ((IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)new List<global::Doroti.Framework.Foundation.DiagnosticsNode>());
        }
        var pending = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        global::Doroti.Framework.Foundation.ErrorSummary? errorSummary = default!;
        foreach (var node in properties)
        {
            if ((node is global::Doroti.Framework.Foundation.ErrorSummary))
            {
                global::Doroti.Framework.Foundation.ErrorSummary node__145947__as145977 = (global::Doroti.Framework.Foundation.ErrorSummary)node;
                errorSummary = ((global::Doroti.Framework.Foundation.ErrorSummary)node__145947__as145977);
                break;
            }
        }
        var foundStackTrace = false;
        var result = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        foreach (var nodeLocal in properties)
        {
            if ((!foundStackTrace && (nodeLocal is global::Doroti.Framework.Foundation.DiagnosticsStackTrace)))
            {
                global::Doroti.Framework.Foundation.DiagnosticsStackTrace node__146133__as146183 = (global::Doroti.Framework.Foundation.DiagnosticsStackTrace)nodeLocal;
                foundStackTrace = true;
            }
            if (Widget_inspectorLibrary._isDebugCreator(nodeLocal))
            {
                result.AddRange(Widget_inspectorLibrary._parseDiagnosticsNode(nodeLocal, errorSummary).Cast<global::Doroti.Framework.Foundation.DiagnosticsNode>());
            }
            else
            {
                if (foundStackTrace)
                {
                    pending.Add(nodeLocal);
                }
                else
                {
                    result.Add(nodeLocal);
                }
            }
        }
        result.AddRange(pending.Cast<global::Doroti.Framework.Foundation.DiagnosticsNode>());
        return ((IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)result);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode> _parseDiagnosticsNode(global::Doroti.Framework.Foundation.DiagnosticsNode node, global::Doroti.Framework.Foundation.ErrorSummary? errorSummary)
    {
        DartRuntimePrimitives.Assert(() => Widget_inspectorLibrary._isDebugCreator(node));
        try
        {
            var debugCreator = ((DebugCreator?)(object?)((global::Doroti.Framework.Foundation.DiagnosticsNode)node).value!)!;
            Element elementLocal = ((DebugCreator)debugCreator).element;
            return Widget_inspectorLibrary._describeRelevantUserCode(elementLocal, errorSummary);
        }
        catch (Exception error)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            DartAsyncRuntime.scheduleMicrotask((() =>
            {
                FlutterError.reportError(new global::Doroti.Framework.Foundation.FlutterErrorDetails(exception: error, stack: stackLocal, library: "widget inspector", informationCollector: ((InformationCollector)(() => new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { global::Doroti.Framework.Foundation.DiagnosticsNode.CreateMessage("This exception was caught while trying to describe the user-relevant code of another error.") }))));
            }));
            return ((IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)new List<global::Doroti.Framework.Foundation.DiagnosticsNode>());
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode> _describeRelevantUserCode(Element element, global::Doroti.Framework.Foundation.ErrorSummary? errorSummary)
    {
        if (!WidgetInspectorService.instance.isWidgetCreationTracked())
        {
            return ((IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorDescription("Widget creation tracking is currently disabled. Enabling " + "it enables improved error messages. It can be enabled by passing " + "`--track-widget-creation` to `flutter run` or `flutter test`."), new global::Doroti.Framework.Foundation.ErrorSpacer() });
        }
        bool isOverflowError()
        {
            if (((errorSummary is not null) && !string.IsNullOrEmpty(errorSummary.value?.ToString())))
            {
                object summary = errorSummary.value;
                if (((summary is string) && ((string)summary).startsWith("A RenderFlex overflowed by")))
                {
                    string summary__148033__as148079 = (string)summary;
                    return true;
                }
            }
            return false;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var nodes = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();
        bool processElement(Element target)
        {
            if (Widget_inspectorLibrary.debugIsLocalCreationLocation(target))
            {
                global::Doroti.Framework.Foundation.DiagnosticsNode? devToolsDiagnostic = default!;
                if (isOverflowError())
                {
                    string? devToolsInspectorUri = ((string?)(object?)WidgetInspectorService.instance._devToolsInspectorUriForElement(target));
                    if ((devToolsInspectorUri is not null))
                    {
                        devToolsDiagnostic = DartRuntimePrimitives.ConvertValue<global::Doroti.Framework.Foundation.DiagnosticsNode>(new DevToolsDeepLinkProperty($"To inspect this widget in Flutter DevTools, visit: {devToolsInspectorUri}", devToolsInspectorUri));
                    }
                }
                nodes.AddRange(new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.DiagnosticsBlock(name: "The relevant error-causing widget was", children: new List<global::Doroti.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Framework.Foundation.ErrorDescription($"{((Diagnosticable)((Element)target).widget).toStringShort()} {Widget_inspectorLibrary._describeCreationLocation(target)}") }), new global::Doroti.Framework.Foundation.ErrorSpacer() }.Cast<global::Doroti.Framework.Foundation.DiagnosticsNode>());
                return false;
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if (processElement(element))
        {
            element.visitAncestorElements((global::System.Func<Element, bool>)processElement);
        }
        return ((IEnumerable<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)nodes);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DevToolsDeepLinkProperty : global::Doroti.Framework.Foundation.DiagnosticsProperty<string>
{
    public DevToolsDeepLinkProperty(string description, string url) : base("", url, description: description, level: global::Doroti.Framework.Foundation.DiagnosticLevel.info)
    {
    }

}

public static partial class Widget_inspectorLibrary
{
    public static bool debugIsLocalCreationLocation(object @object)
    {
        var isLocal = false;
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Runtime.CreationLocation? location = ((global::Doroti.Runtime.CreationLocation?)(object?)Widget_inspectorLibrary._getCreationLocation(@object));
                if ((location is not null))
                {
                    isLocal = WidgetInspectorService.instance._isLocalCreationLocation(((string)(object)((object)((dynamic)location).file)));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isLocal;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    public static bool debugIsWidgetLocalCreation(Widget widget)
    {
        global::Doroti.Runtime.CreationLocation? location = ((global::Doroti.Runtime.CreationLocation?)(object?)global::Doroti.Runtime.CreationLocation.of(widget));
        return ((location is not null) && WidgetInspectorService.instance._isLocalCreationLocation(((string)(object)((object)((dynamic)location).file))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static string? _describeCreationLocation(object @object)
    {
        global::Doroti.Runtime.CreationLocation? location = ((global::Doroti.Runtime.CreationLocation?)(object?)Widget_inspectorLibrary._getCreationLocation(@object));
        return ((string?)((dynamic)location)?.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static global::Doroti.Runtime.CreationLocation? _getCreationLocation(object? @object)
    {
        object? candidate = (((@object is Element) && !((Element)((Element)@object)).debugIsDefunct) ? ((Element)((Element)@object)).widget : @object);
        return ((candidate is null) ? null : global::Doroti.Runtime.CreationLocation.of(candidate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static DartMap<object, long> _locationToId = new DartMap<object, long>();
}

public static partial class Widget_inspectorLibrary
{
    internal static List<object> _locations = new List<global::Doroti.Runtime.CreationLocation>().Cast<object>().ToList();
}

public static partial class Widget_inspectorLibrary
{
    internal static long _toLocationId(object location)
    {
        long? id = DartCollectionRuntime.NullableMapValue<long>(Widget_inspectorLibrary._locationToId, location);
        if ((id is not null))
        {
            long id__152830__value152866 = DartRuntimePrimitives.RequireValue(id);
            return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id__152830__value152866));
        }
        id = checked((long)(Widget_inspectorLibrary._locations.Count));
        Widget_inspectorLibrary._locations.Add(location);
        Widget_inspectorLibrary._locationToId[location] = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id));
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static DartMap<string, object> _locationIdMapToJson()
    {
        var idsKey = "ids";
        var linesKey = "lines";
        var columnsKey = "columns";
        var namesKey = "names";
        var fileLocationsMap = new DartMap<string, DartMap<string, List<object>>>();
        foreach (var entry in Widget_inspectorLibrary._locationToId.entries)
        {
            global::Doroti.Runtime.CreationLocation location = ((global::Doroti.Runtime.CreationLocation)(object?)entry.key);
            DartMap<string, List<object?>> locations = fileLocationsMap.putIfAbsent(((string)(object)((object)((dynamic)location).file)), (() => new DartMap<string, List<object>> { [idsKey] = new List<long>().Cast<object>().ToList(), [linesKey] = new List<long>().Cast<object>().ToList(), [columnsKey] = new List<long>().Cast<object>().ToList(), [namesKey] = new List<string?>().Cast<object>().ToList() })).cast<string, List<object?>>();
            locations.GetValueOrDefault(idsKey)!.Add(entry.value);
            locations.GetValueOrDefault(linesKey)!.Add(((object)((dynamic)location).line));
            locations.GetValueOrDefault(columnsKey)!.Add(((object)((dynamic)location).column));
            locations.GetValueOrDefault(namesKey)!.Add(location.ToString());
        }
        return ((DartMap<string, object>)(object?)fileLocationsMap);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class InspectorSerializationDelegate : global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate
{
    public virtual WidgetInspectorService service { get; private set; } = default!;
    public virtual string? groupName { get; private set; }
    public virtual bool summaryTree { get; private set; } = default!;
    public virtual long maxDescendantsTruncatableNode { get; private set; } = default!;
    public virtual bool includeProperties { get; private set; } = default!;
    public virtual long subtreeDepth { get; private set; } = default!;
    public virtual bool expandPropertyValues { get; private set; } = default!;
    public virtual bool inDisableWidgetInspectorScope { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback { get; private set; }
    internal virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> _nodesCreatedByLocalProject { get; private set; } = new List<global::Doroti.Framework.Foundation.DiagnosticsNode>();

    public InspectorSerializationDelegate(string? groupName = null, bool summaryTree = false, long maxDescendantsTruncatableNode = -1, bool expandPropertyValues = true, long subtreeDepth = 1, bool includeProperties = false, WidgetInspectorService service = default!, global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null, bool inDisableWidgetInspectorScope = false)
    {
        this.groupName = groupName;
        this.summaryTree = summaryTree;
        this.maxDescendantsTruncatableNode = maxDescendantsTruncatableNode;
        this.expandPropertyValues = expandPropertyValues;
        this.subtreeDepth = subtreeDepth;
        this.includeProperties = includeProperties;
        this.service = service;
        this.addAdditionalPropertiesCallback = addAdditionalPropertiesCallback;
        this.inDisableWidgetInspectorScope = inDisableWidgetInspectorScope;
    }

    internal virtual bool _interactive => DartRuntimePrimitives.ConvertValue<bool>((this.groupName is not null));
    public virtual DartMap<string, object> additionalNodeProperties(global::Doroti.Framework.Foundation.DiagnosticsNode node, bool fullDetails = true)
    {
        var result = new DartMap<string, object>();
        object? valueLocal = ((global::Doroti.Framework.Foundation.DiagnosticsNode)node).value;
        if ((this.summaryTree && fullDetails))
        {
            result["summaryTree"] = true;
        }
        if (this._interactive)
        {
            result["valueId"] = this.service.toId(valueLocal, this.groupName!);
        }
        global::Doroti.Runtime.CreationLocation? creationLocation = ((global::Doroti.Runtime.CreationLocation?)(object?)Widget_inspectorLibrary._getCreationLocation(valueLocal));
        if ((creationLocation is not null))
        {
            if (fullDetails)
            {
                result["locationId"] = Widget_inspectorLibrary._toLocationId(creationLocation);
                result["creationLocation"] = ((object)((dynamic)creationLocation).toJsonMap());
            }
            if (this.service._isLocalCreationLocation(((string)(object)((object)((dynamic)creationLocation).file))))
            {
                this._nodesCreatedByLocalProject.Add(node);
                result["createdByLocalProject"] = true;
            }
        }
        if ((this.addAdditionalPropertiesCallback is not null))
        {
            result.AddRange((this.addAdditionalPropertiesCallback!(node, this) ?? new DartMap<string, object>()));
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate delegateForNode(global::Doroti.Framework.Foundation.DiagnosticsNode node)
    {
        return ((global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate)(object?)(((this.summaryTree || (this.subtreeDepth > 1L)) || this.service._shouldShowInSummaryTree(node)) ? copyWith(subtreeDepth: (this.subtreeDepth - 1L)) : this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> filterChildren(List<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, global::Doroti.Framework.Foundation.DiagnosticsNode owner)
    {
        return ((List<global::Doroti.Framework.Foundation.DiagnosticsNode>)(object?)this.service._filterChildren(nodes, this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> filterProperties(List<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, global::Doroti.Framework.Foundation.DiagnosticsNode owner)
    {
        bool createdByLocalProject = this._nodesCreatedByLocalProject.Contains(owner);
        return nodes.where(((node) =>
        {
            return !node.isFiltered((createdByLocalProject ? global::Doroti.Framework.Foundation.DiagnosticLevel.fine : global::Doroti.Framework.Foundation.DiagnosticLevel.info));
            throw new InvalidOperationException("Dart closure completed without a value.");
        })).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Framework.Foundation.DiagnosticsNode> truncateNodesList(List<global::Doroti.Framework.Foundation.DiagnosticsNode> nodes, global::Doroti.Framework.Foundation.DiagnosticsNode? owner)
    {
        if ((((this.maxDescendantsTruncatableNode >= 0L) && owner!.allowTruncate) && (checked((long)(nodes.Count)) > this.maxDescendantsTruncatableNode)))
        {
            nodes = this.service._truncateNodes(nodes.Cast<global::Doroti.Framework.Foundation.DiagnosticsNode>(), this.maxDescendantsTruncatableNode);
        }
        return nodes;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate copyWith(long? subtreeDepth = null, bool? includeProperties = null, bool? expandPropertyValues = null, bool? inDisableWidgetInspectorScope = null)
    {
        return ((global::Doroti.Framework.Foundation.DiagnosticsSerializationDelegate)(object?)new InspectorSerializationDelegate(groupName: this.groupName, summaryTree: this.summaryTree, maxDescendantsTruncatableNode: this.maxDescendantsTruncatableNode, expandPropertyValues: (expandPropertyValues ?? this.expandPropertyValues), subtreeDepth: (subtreeDepth ?? this.subtreeDepth), includeProperties: (includeProperties ?? this.includeProperties), service: this.service, addAdditionalPropertiesCallback: (global::System.Func<global::Doroti.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>?)this.addAdditionalPropertiesCallback, inDisableWidgetInspectorScope: (inDisableWidgetInspectorScope ?? this.inDisableWidgetInspectorScope)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Widget_inspectorLibrary
{
    public static object widgetFactory = new object();
}

public class WeakMap<K, V> where K : notnull
{
    internal virtual Expando<object> _objects { get; set; } = new Expando<object>();
    internal virtual DartMap<K, V> _primitives { get; private set; } = new DartMap<K, V>();

    internal virtual bool _isPrimitive(object? key)
    {
        return ((((key is null) || (key is string)) || (key is double)) || (key is bool));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public V? this[K key]
    {
        get
        {
            if (_isPrimitive(key))
            {
                return this._primitives.GetValueOrDefault(key);
            }
            else
            {
                return ((V?)(object?)this._objects[key!])!;
            }
            return default!;
        }
        set
        {
            if (_isPrimitive(key))
            {
                this._primitives[key] = value;
            }
            else
            {
                this._objects[key!] = value;
            }
        }
    }

    public virtual V? remove(K key)
    {
        if (_isPrimitive(key))
        {
            return ((V?)(object?)this._primitives.remove(key));
        }
        else
        {
            var result = ((V?)(object?)this._objects[key!])!;
            this._objects[key] = null;
            return result;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void clear()
    {
        _objects = new Expando<object>();
        this._primitives.Clear();
    }

}

public static partial class Widget_inspectorLibrary
{
    public static class developer
    {
        public static class CreationLocation
        {
            public static global::Doroti.Runtime.CreationLocation? of(object? value) => global::Doroti.Runtime.CreationLocation.of(value);
        }
    }
}
