// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../flutter-master/packages/flutter/lib/src/widgets/widget_inspector.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0659, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8609, CS8613, CS8619, CS8620, CS8622, CS8625, CS8629, CS8714, CS8765, CS8767, CS8981
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Flutter.Runtime;
using Doroti.Flutter.Ui;
using static Doroti.Flutter.Runtime.FoundationRuntimePorts;
using Match = Doroti.Flutter.Runtime.DartMatch;

namespace Doroti.Generated.Framework.Widgets;

public delegate Widget ExitWidgetSelectionButtonBuilder(BuildContext context, GlobalKey<IState> key, global::System.Action onPressed, string semanticsLabel);

public delegate Widget MoveExitWidgetSelectionButtonBuilder(BuildContext context, global::System.Action onPressed, string semanticsLabel, bool usesDefaultAlignment = default!);

public delegate Widget TapBehaviorButtonBuilder(BuildContext context, global::System.Action onPressed, bool selectionOnTapEnabled, string semanticsLabel);

public delegate void RegisterServiceExtensionCallback(global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>> callback, string name);

internal class _ProxyLayer__widget_inspector : global::Doroti.Generated.Framework.Rendering.Layer
{
    internal virtual global::Doroti.Generated.Framework.Rendering.Layer _layer { get; private set; } = default!;

    internal _ProxyLayer__widget_inspector(global::Doroti.Generated.Framework.Rendering.Layer _layer)
    {
        this._layer = _layer;
    }

    public override void addToScene(SceneBuilder builder)
    {
        this._layer.addToScene(builder);
    }

    public override bool findAnnotations<S>(global::Doroti.Generated.Framework.Rendering.AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
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

    public virtual void clipRect(Rect rect, global::Doroti.Flutter.Ui.ClipOp clipOp = default!, bool doAntiAlias = true)
    {
        this._main.clipRect(rect, clipOp: clipOp, doAntiAlias: doAntiAlias);
        this._screenshot.clipRect(rect, clipOp: clipOp, doAntiAlias: doAntiAlias);
    }

    public virtual void drawArc(Rect rect, double startAngle, double sweepAngle, bool useCenter, Paint paint)
    {
        this._main.drawArc(rect, startAngle, sweepAngle, useCenter, paint);
        this._screenshot.drawArc(rect, startAngle, sweepAngle, useCenter, paint);
    }

    public virtual void drawAtlas(global::Doroti.Flutter.Ui.Image atlas, List<global::Doroti.Flutter.Ui.RSTransform> transforms, List<Rect> rects, List<Color>? colors, BlendMode? blendMode, Rect? cullRect, Paint paint)
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

    public virtual void drawImage(global::Doroti.Flutter.Ui.Image image, Offset p, Paint paint)
    {
        this._main.drawImage(image, p, paint);
        this._screenshot.drawImage(image, p, paint);
    }

    public virtual void drawImageNine(global::Doroti.Flutter.Ui.Image image, Rect center, Rect dst, Paint paint)
    {
        this._main.drawImageNine(image, center, dst, paint);
        this._screenshot.drawImageNine(image, center, dst, paint);
    }

    public virtual void drawImageRect(global::Doroti.Flutter.Ui.Image image, Rect src, Rect dst, Paint paint)
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

    public virtual void drawPoints(global::Doroti.Flutter.Ui.PointMode pointMode, List<Offset> points, Paint paint)
    {
        this._main.drawPoints(pointMode, points, paint);
        this._screenshot.drawPoints(pointMode, points, paint);
    }

    public virtual void drawRRect(RRect rrect, Paint paint)
    {
        this._main.drawRRect(rrect, paint);
        this._screenshot.drawRRect(rrect, paint);
    }

    public virtual void drawRawAtlas(global::Doroti.Flutter.Ui.Image atlas, Float32List rstTransforms, Float32List rects, Int32List? colors, BlendMode? blendMode, Rect? cullRect, Paint paint)
    {
        this._main.drawRawAtlas(atlas, rstTransforms, rects, colors, blendMode, cullRect, paint);
        this._screenshot.drawRawAtlas(atlas, rstTransforms, rects, colors, blendMode, cullRect, paint);
    }

    public virtual void drawRawPoints(global::Doroti.Flutter.Ui.PointMode pointMode, Float32List points, Paint paint)
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

    public virtual void drawVertices(global::Doroti.Flutter.Ui.Vertices vertices, BlendMode blendMode, Paint paint)
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

    public virtual dynamic noSuchMethod(global::Doroti.Flutter.Runtime.Invocation invocation)
    {
        base.noSuchMethod(invocation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static Rect _calculateSubtreeBoundsHelper(global::Doroti.Generated.Framework.Rendering.RenderObject @object, Matrix4 transform)
    {
        global::Doroti.Flutter.Ui.Rect bounds__9333 = ((global::Doroti.Flutter.Ui.Rect)(object?)MatrixUtils.transformRect(transform, ((global::Doroti.Generated.Framework.Rendering.RenderObject)@object).semanticBounds));
        ((dynamic)@object).visitChildren(((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)((child) => {
Matrix4 childTransform__9468 = transform.clone();
((dynamic)@object).applyPaintTransform(child, childTransform__9468);
global::Doroti.Flutter.Ui.Rect childBounds__9568 = ((global::Doroti.Flutter.Ui.Rect)(object?)Widget_inspectorLibrary._calculateSubtreeBoundsHelper(child, childTransform__9468));
global::Doroti.Flutter.Ui.Rect? paintClip__9652 = ((global::Doroti.Flutter.Ui.Rect?)(object?)((Rect?)((dynamic)@object).describeApproximatePaintClip(child)));
if ((paintClip__9652 is not null))
{
    Rect paintClip__9652__value9716 = DartRuntimePrimitives.RequireValue(paintClip__9652);
    global::Doroti.Flutter.Ui.Rect transformedPaintClip__9754 = ((global::Doroti.Flutter.Ui.Rect)(object?)MatrixUtils.transformRect(transform, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(paintClip__9652__value9716))));
    childBounds__9568 = childBounds__9568.intersect(transformedPaintClip__9754);
}
if ((childBounds__9568.isFinite && !childBounds__9568.isEmpty))
{
    bounds__9333 = (bounds__9333.isEmpty ? childBounds__9568 : bounds__9333.expandToInclude(childBounds__9568));
}
})));
        return bounds__9333;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static Rect _calculateSubtreeBounds(global::Doroti.Generated.Framework.Rendering.RenderObject @object)
    {
        return Widget_inspectorLibrary._calculateSubtreeBoundsHelper(@object, Matrix4.identity());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ScreenshotContainerLayer__widget_inspector : global::Doroti.Generated.Framework.Rendering.OffsetLayer
{
    public override void addToScene(SceneBuilder builder)
    {
        addChildrenToScene(builder);
    }

}

public class _ScreenshotData__widget_inspector
{
    public virtual global::Doroti.Generated.Framework.Rendering.RenderObject target { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.OffsetLayer containerLayer { get; private set; } = default!;
    public virtual bool foundTarget { get; set; } = false;
    public virtual bool includeInScreenshot { get; set; } = false;
    public virtual bool includeInRegularContext { get; set; } = true;

    internal _ScreenshotData__widget_inspector(global::Doroti.Generated.Framework.Rendering.RenderObject target)
    {
        this.target = target;
        this.containerLayer = new _ScreenshotContainerLayer__widget_inspector();
    }

    public virtual global::Doroti.Flutter.Ui.Offset screenshotOffset
    {
        get{
            DartRuntimePrimitives.Assert(() => this.foundTarget);
            return ((global::Doroti.Generated.Framework.Rendering.OffsetLayer)this.containerLayer).offset;
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
        DartRuntimePrimitives.Assert(() => global::Doroti.Generated.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this.containerLayer.dispose();
    }

}

internal class _ScreenshotPaintingContext__widget_inspector : global::Doroti.Generated.Framework.Rendering.PaintingContext
{
    internal virtual _ScreenshotData__widget_inspector _data { get; private set; } = default!;
    internal virtual global::Doroti.Generated.Framework.Rendering.PictureLayer? _screenshotCurrentLayer { get; set; } = default;
    internal virtual PictureRecorder? _screenshotRecorder { get; set; } = default;
    internal virtual Canvas? _screenshotCanvas { get; set; } = default;
    internal virtual _MulticastCanvas__widget_inspector? _multicastCanvas { get; set; } = default;

    internal _ScreenshotPaintingContext__widget_inspector(global::Doroti.Generated.Framework.Rendering.ContainerLayer containerLayer, Rect estimatedBounds, _ScreenshotData__widget_inspector screenshotData) : base(containerLayer, estimatedBounds)
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
            var hasScreenshotCanvas__13680 = (this._screenshotCanvas is not null);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (hasScreenshotCanvas__13680)
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
            return hasScreenshotCanvas__13680;
            return default!;
        }
    }
    internal virtual void _startRecordingScreenshot()
    {
        DartRuntimePrimitives.Assert(() => ((_ScreenshotData__widget_inspector)this._data).includeInScreenshot);
        DartRuntimePrimitives.Assert(() => !this._isScreenshotRecording);
        _screenshotCurrentLayer = new global::Doroti.Generated.Framework.Rendering.PictureLayer(this.estimatedBounds);
        _screenshotRecorder = new global::Doroti.Flutter.Ui.PictureRecorder();
        _screenshotCanvas = new global::Doroti.Flutter.Ui.Canvas(this._screenshotRecorder!);
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

    public override void appendLayer(global::Doroti.Generated.Framework.Rendering.Layer layer)
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

    public override global::Doroti.Generated.Framework.Rendering.PaintingContext createChildContext(global::Doroti.Generated.Framework.Rendering.ContainerLayer childLayer, Rect bounds)
    {
        if (((_ScreenshotData__widget_inspector)this._data).foundTarget)
        {
            return ((global::Doroti.Generated.Framework.Rendering.PaintingContext)(object?)base.createChildContext(childLayer, bounds));
        }
        else
        {
            return ((global::Doroti.Generated.Framework.Rendering.PaintingContext)(object?)new _ScreenshotPaintingContext__widget_inspector(containerLayer: childLayer, estimatedBounds: bounds, screenshotData: this._data));
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintChild(global::Doroti.Generated.Framework.Rendering.RenderObject child, Offset offset)
    {
        bool isScreenshotTarget__16348 = DartRuntimePrimitives.Identical(child, ((_ScreenshotData__widget_inspector)this._data).target);
        if (isScreenshotTarget__16348)
        {
            DartRuntimePrimitives.Assert(() => !((_ScreenshotData__widget_inspector)this._data).includeInScreenshot);
            DartRuntimePrimitives.Assert(() => !((_ScreenshotData__widget_inspector)this._data).foundTarget);
            this._data.foundTarget = true;
            this._data.screenshotOffset = offset;
            this._data.includeInScreenshot = true;
        }
        base.paintChild(child, offset);
        if (isScreenshotTarget__16348)
        {
            _stopRecordingScreenshotIfNeeded();
            this._data.includeInScreenshot = false;
        }
    }

    public static async Future<global::Doroti.Flutter.Ui.Image> toImage(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject, Rect renderBounds, double pixelRatio = 1.0, bool debugPaint = false)
    {
        var repaintBoundary__18429 = renderObject;
        while (!((global::Doroti.Generated.Framework.Rendering.RenderObject)repaintBoundary__18429).isRepaintBoundary)
        {
            repaintBoundary__18429 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)repaintBoundary__18429).parent!;
        }
        var data__18575 = new _ScreenshotData__widget_inspector(target: renderObject);
        var context__18631 = new _ScreenshotPaintingContext__widget_inspector(containerLayer: ((global::Doroti.Generated.Framework.Rendering.RenderObject)repaintBoundary__18429).debugLayer!, estimatedBounds: ((global::Doroti.Generated.Framework.Rendering.RenderObject)repaintBoundary__18429).paintBounds, screenshotData: data__18575);
        if (DartRuntimePrimitives.Identical(renderObject, repaintBoundary__18429))
        {
            ((_ScreenshotData__widget_inspector)data__18575).containerLayer.append(new _ProxyLayer__widget_inspector(((global::Doroti.Generated.Framework.Rendering.RenderObject)repaintBoundary__18429).debugLayer!));
            data__18575.foundTarget = true;
            var offsetLayer__19195 = ((global::Doroti.Generated.Framework.Rendering.OffsetLayer?)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)repaintBoundary__18429).debugLayer!)!;
            data__18575.screenshotOffset = ((global::Doroti.Generated.Framework.Rendering.OffsetLayer)offsetLayer__19195).offset;
        }
        else
        {
            PaintingContext.debugInstrumentRepaintCompositedChild(repaintBoundary__18429, customContext: context__18631);
        }
        if ((debugPaint && !global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled))
        {
            data__18575.includeInRegularContext = false;
            context__18631.stopRecordingIfNeeded();
            DartRuntimePrimitives.Assert(() => ((_ScreenshotData__widget_inspector)data__18575).foundTarget);
            data__18575.includeInScreenshot = true;
            global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled = true;
            try
            {
                ((dynamic)renderObject).debugPaint(context__18631, ((_ScreenshotData__widget_inspector)data__18575).screenshotOffset);
            }
            finally
            {
                global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugPaintSizeEnabled = false;
                context__18631.stopRecordingIfNeeded();
            }
        }
        ((global::Doroti.Generated.Framework.Rendering.RenderObject)repaintBoundary__18429).debugLayer!.buildScene(new global::Doroti.Flutter.Ui.SceneBuilder());
        global::Doroti.Flutter.Ui.Image image__20637 = default!;
        try
        {
            image__20637 = await ((_ScreenshotData__widget_inspector)data__18575).containerLayer.toImage(renderBounds, pixelRatio: pixelRatio);
        }
        finally
        {
            data__18575.dispose();
        }
        return image__20637;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class _DiagnosticsPathNode__widget_inspector
{
    public virtual global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node { get; private set; } = default!;
    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> children { get; private set; } = default!;
    public virtual long? childIndex { get; private set; }

    internal _DiagnosticsPathNode__widget_inspector(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node, List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> children, long? childIndex = null)
    {
        this.node = node;
        this.children = children;
        this.childIndex = childIndex;
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static List<_DiagnosticsPathNode__widget_inspector>? _followDiagnosticableChain(List<global::Doroti.Generated.Framework.Foundation.Diagnosticable> chain)
    {
        var path__21974 = new List<_DiagnosticsPathNode__widget_inspector>();
        if (!System.Linq.Enumerable.Any(chain))
        {
            return path__21974;
        }
        global::Doroti.Generated.Framework.Foundation.DiagnosticsNode diagnostic__22069 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)(object?)((Diagnosticable)chain.First()).toDiagnosticsNode());
        for (var i__22126 = 1L; (i__22126 < checked((long)(chain.Count))); i__22126 += 1L)
        {
            global::Doroti.Generated.Framework.Foundation.Diagnosticable target__22186 = chain[(int)(i__22126)];
            var foundMatch__22213 = false;
            List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> children__22265 = ((List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)diagnostic__22069.getChildren());
            for (var j__22315 = 0L; (j__22315 < checked((long)(children__22265.Count))); j__22315 += 1L)
            {
                global::Doroti.Generated.Framework.Foundation.DiagnosticsNode child__22381 = children__22265[(int)(j__22315)];
                if ((object.Equals(((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)child__22381).value, target__22186)))
                {
                    foundMatch__22213 = true;
                    path__21974.Add(new _DiagnosticsPathNode__widget_inspector(node: diagnostic__22069, children: children__22265, childIndex: j__22315));
                    diagnostic__22069 = child__22381;
                    break;
                }
            }
            DartRuntimePrimitives.Assert(() => foundMatch__22213);
        }
        path__21974.Add(new _DiagnosticsPathNode__widget_inspector(node: diagnostic__22069, children: diagnostic__22069.getChildren().ToList()));
        return path__21974;
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
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) => {
return new DartMap<string, object> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback()) };
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual void _registerObjectGroupServiceExtension(string name, dynamic callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) => {
return new DartMap<string, object> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(parameters.GetValueOrDefault("objectGroup")!)) };
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual void _registerBoolServiceExtension(string name, global::System.Func<Future<bool>> getter, global::System.Func<bool, Future> setter, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) => {
if (parameters.ContainsKey("enabled"))
{
    var value__31029 = (parameters.GetValueOrDefault("enabled") == "true");
    await setter(DartRuntimePrimitives.RequireValue(value__31029));
    _postExtensionStateChangedEvent(name, DartRuntimePrimitives.RequireValue(value__31029));
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
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) => {
DartRuntimePrimitives.Assert(() => parameters.ContainsKey("objectGroup"));
return new DartMap<string, object> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(parameters.GetValueOrDefault("arg"), parameters.GetValueOrDefault("objectGroup")!)) };
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual void _registerServiceExtensionVarArgs(string name, global::System.Func<List<string>, object> callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) => {
long index__33399 = default!;
var args__33420 = new List<string>();
DartRuntimePrimitives.Assert(() => ((index__33399 == checked((long)(parameters.Count))) || (((index__33399 == (checked((long)(parameters.Count)) - 1L)) && parameters.ContainsKey("isolateId")))));
return new DartMap<string, object> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(args__33420)) };
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
    }

    public virtual Future forceRebuild()
    {
        WidgetsBinding binding__34321 = WidgetsBinding.instance;
        if ((((WidgetsBinding)binding__34321).rootElement is not null))
        {
            ((WidgetsBinding)binding__34321).buildOwner!.reassemble(((WidgetsBinding)binding__34321).rootElement!);
            return binding__34321.endOfFrame;
        }
        return Future.value();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reportStructuredError(global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails details)
    {
        DartMap<string, object?> errorJson__34717 = _nodeToJson(((Diagnosticable)details).toDiagnosticsNode(), new InspectorSerializationDelegate(groupName: WidgetInspectorService._consoleObjectGroup, subtreeDepth: 5L, includeProperties: true, maxDescendantsTruncatableNode: 5L, service: this))!.cast<string, object?>();
        errorJson__34717["errorsSinceReload"] = this._errorsSinceReload;
        if ((this._errorsSinceReload == 0L))
        {
            errorJson__34717["renderedErrorText"] = new global::Doroti.Generated.Framework.Foundation.TextTreeRenderer(wrapWidthProperties: global::Doroti.Generated.Framework.Foundation.FlutterError.wrapWidth, maxDescendentsTruncatableNode: 5L).render(((Diagnosticable)details).toDiagnosticsNode(style: global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.error)).trimRight();
        }
        else
        {
            errorJson__34717["renderedErrorText"] = $"Another exception was thrown: {(((global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails)details).summary)}";
        }
        this._errorsSinceReload += 1L;
        postEvent("Flutter.Error", errorJson__34717.cast<object, object>());
    }

    public virtual void _resetErrorCount()
    {
        this._errorsSinceReload = 0L;
    }

    public virtual bool isStructuredErrorsEnabled()
    {
        var enabled__36155 = false;
        DartRuntimePrimitives.Assert(() =>
            {
                enabled__36155 = false;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return enabled__36155;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void initServiceExtensions(RegisterServiceExtensionCallback registerExtension)
    {
        global::Doroti.Generated.Framework.Foundation.FlutterExceptionHandler defaultExceptionHandler__36848 = ((global::Doroti.Generated.Framework.Foundation.FlutterExceptionHandler)(object?)global::Doroti.Generated.Framework.Foundation.FlutterError.presentError);
        if (isStructuredErrorsEnabled())
        {
            global::Doroti.Generated.Framework.Foundation.FlutterError.presentError = this._reportStructuredError;
        }
        DartRuntimePrimitives.Assert(() => !WidgetInspectorService._debugServiceExtensionsRegistered);
        DartRuntimePrimitives.Assert(() =>
            {
                WidgetInspectorService._debugServiceExtensionsRegistered = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPersistentFrameCallback((global::System.Action<Duration>)this._onFrameStart);
        _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.structuredErrors.ToString(), getter: ((global::System.Func<Future<bool>>)(async () => (object.Equals((global::Doroti.Generated.Framework.Foundation.FlutterExceptionHandler)global::Doroti.Generated.Framework.Foundation.FlutterError.presentError, (global::Doroti.Generated.Framework.Foundation.FlutterExceptionHandler)this._reportStructuredError)))), setter: ((global::System.Func<bool, Future>)((value) => {
global::Doroti.Generated.Framework.Foundation.FlutterError.presentError = ((value ? (global::Doroti.Generated.Framework.Foundation.FlutterExceptionHandler)this._reportStructuredError : defaultExceptionHandler__36848));
return Future.value();
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.show.ToString(), getter: ((global::System.Func<Future<bool>>)(async () => WidgetsBinding.instance.debugShowWidgetInspectorOverride)), setter: ((global::System.Func<bool, Future>)((value) => {
if ((WidgetsBinding.instance.debugShowWidgetInspectorOverride != value))
{
    _changeWidgetSelectionMode(DartRuntimePrimitives.RequireValue(value), notifyStateChange: false);
}
return Future.value();
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        if (isWidgetCreationTracked())
        {
            _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.trackRebuildDirtyWidgets.ToString(), getter: ((global::System.Func<Future<bool>>)(async () => this._trackRebuildDirtyWidgets)), setter: ((global::System.Func<bool, Future>)(async (value) => {
if ((value == this._trackRebuildDirtyWidgets))
{
    return;
}
this._rebuildStats.resetCounts();
this._trackRebuildDirtyWidgets = value;
if (value)
{
    DartRuntimePrimitives.Assert(() => (global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugOnRebuildDirtyWidget is null));
    global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugOnRebuildDirtyWidget = this._onRebuildWidget;
    await forceRebuild();
    return;
}
else
{
    global::Doroti.Generated.Framework.Widgets.DebugLibrary.debugOnRebuildDirtyWidget = null;
    return;
}
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
            _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.widgetLocationIdMap.ToString(), callback: ((global::System.Func<object>)(() => {
return Widget_inspectorLibrary._locationIdMapToJson();
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
            _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.trackRepaintWidgets.ToString(), getter: ((global::System.Func<Future<bool>>)(async () => this._trackRepaintWidgets)), setter: ((global::System.Func<bool, Future>)(async (value) => {
if ((value == this._trackRepaintWidgets))
{
    return;
}
this._repaintStats.resetCounts();
this._trackRepaintWidgets = value;
if (value)
{
    DartRuntimePrimitives.Assert(() => (global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugOnProfilePaint is null));
    global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugOnProfilePaint = this._onPaint;
    void markTreeNeedsPaint(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        ((dynamic)renderObject).markNeedsPaint();
        ((dynamic)renderObject).visitChildren((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)markTreeNeedsPaint);
    }
    global::Doroti.Generated.Framework.Rendering.RendererBinding.instance.renderViews.forEach((__arg0) => ((global::System.Action<global::Doroti.Generated.Framework.Rendering.RenderObject>)markTreeNeedsPaint)(DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.RenderObject>(__arg0)));
}
else
{
    global::Doroti.Generated.Framework.Rendering.DebugLibrary.debugOnProfilePaint = null;
}
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        }
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.disposeAllGroups.ToString(), callback: ((global::System.Func<object>)(() => {
disposeAllGroups();
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerObjectGroupServiceExtension(name: WidgetInspectorServiceExtensions.disposeGroup.ToString(), callback: ((global::System.Func<string, object>)((name) => {
disposeGroup(name);
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.isWidgetTreeReady.ToString(), callback: (global::System.Func<string?, bool>)this.isWidgetTreeReady, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.disposeId.ToString(), callback: ((global::System.Func<string?, string, object>)((objectId, objectGroup) => {
disposeId(objectId, objectGroup);
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.setPubRootDirectories.ToString(), callback: ((global::System.Func<List<string>, object>)((args) => {
setPubRootDirectories(args);
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.addPubRootDirectories.ToString(), callback: ((global::System.Func<List<string>, object>)((args) => {
addPubRootDirectories(args);
return null;
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.removePubRootDirectories.ToString(), callback: ((global::System.Func<List<string>, object>)((args) => {
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
        _registerObjectGroupServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetSummaryTree.ToString(), callback: (global::System.Func<string, global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>?, DartMap<string, object>?>)this._getRootWidgetSummaryTree, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetSummaryTreeWithPreviews.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this._getRootWidgetSummaryTreeWithPreviews, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetTree.ToString(), callback: (global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)this._getRootWidgetTree, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getDetailsSubtree.ToString(), callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) => {
DartRuntimePrimitives.Assert(() => parameters.ContainsKey("objectGroup"));
string? subtreeDepth__44497 = parameters.GetValueOrDefault("subtreeDepth");
return new DartMap<string, object> { ["result"] = _getDetailsSubtree(parameters.GetValueOrDefault("arg"), parameters.GetValueOrDefault("objectGroup"), ((subtreeDepth__44497 is not null) ? Dart_coreLibrary.parse(subtreeDepth__44497) : 2L)) };
throw new InvalidOperationException("Dart closure completed without a value.");
})), registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getSelectedWidget.ToString(), callback: (global::System.Func<string?, string, DartMap<string, object>?>)this._getSelectedWidget, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getSelectedSummaryWidget.ToString(), callback: (global::System.Func<string?, string, DartMap<string, object>?>)this._getSelectedSummaryWidget, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.isWidgetCreationTracked.ToString(), callback: (global::System.Func<bool>)this.isWidgetCreationTracked, registerExtension: (RegisterServiceExtensionCallback)registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.screenshot.ToString(), callback: ((global::System.Func<DartMap<string, string>, Future<DartMap<string, object>>>)(async (parameters) => {
DartRuntimePrimitives.Assert(() => parameters.ContainsKey("id"));
DartRuntimePrimitives.Assert(() => parameters.ContainsKey("width"));
DartRuntimePrimitives.Assert(() => parameters.ContainsKey("height"));
global::Doroti.Flutter.Ui.Image? image__45758 = await screenshot(toObject(parameters.GetValueOrDefault("id")), width: Dart_coreLibrary.parse(parameters.GetValueOrDefault("width")!), height: Dart_coreLibrary.parse(parameters.GetValueOrDefault("height")!), margin: (parameters.ContainsKey("margin") ? Dart_coreLibrary.parse(parameters.GetValueOrDefault("margin")!) : 0.0), maxPixelRatio: (parameters.ContainsKey("maxPixelRatio") ? Dart_coreLibrary.parse(parameters.GetValueOrDefault("maxPixelRatio")!) : 1.0), debugPaint: (parameters.GetValueOrDefault("debugPaint") == "true"));
if ((image__45758 is null))
{
    return new DartMap<string, object> { ["result"] = null };
}
ByteData? byteData__46355 = await image__45758.toByteData(format: ImageByteFormat.png);
image__45758.dispose();
return new DartMap<string, object> { ["result"] = global::Doroti.Flutter.Runtime.Dart_convertLibrary.base64.encoder.convert(new Uint8List(byteData__46355!.buffer)) };
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
        HashSet<InspectorReferenceData>? references__48345 = this._groups.remove(name);
        if ((references__48345 is null))
        {
            return;
        }
        references__48345.forEach((__arg0) => ((global::System.Action<InspectorReferenceData>)this._decrementReferenceCount)(__arg0));
    }

    public virtual void _decrementReferenceCount(InspectorReferenceData reference)
    {
        reference.count -= 1L;
        DartRuntimePrimitives.Assert(() => (((InspectorReferenceData)reference).count >= 0L));
        if ((((InspectorReferenceData)reference).count == 0L))
        {
            object? value__48665 = ((InspectorReferenceData)reference).value;
            if ((value__48665 is not null))
            {
                this._objectToId.remove(value__48665);
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
        HashSet<InspectorReferenceData> group__49095 = this._groups.putIfAbsent(groupName, (() => new HashSet<InspectorReferenceData>()));
        string? id__49212 = this._objectToId[@object];
        InspectorReferenceData referenceData__49265 = default!;
        if ((id__49212 is null))
        {
            id__49212 = $"inspector-{this._nextId}";
            this._nextId += 1L;
            this._objectToId[@object] = id__49212;
            referenceData__49265 = new InspectorReferenceData(@object, id__49212);
            this._idToReferenceData[id__49212] = referenceData__49265;
            group__49095.Add(referenceData__49265);
        }
        else
        {
            referenceData__49265 = this._idToReferenceData.GetValueOrDefault(id__49212)!;
            if (group__49095.Add(referenceData__49265))
            {
                referenceData__49265.count += 1L;
            }
        }
        return id__49212;
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
        InspectorReferenceData? data__50504 = this._idToReferenceData.GetValueOrDefault(id);
        if ((data__50504 is null))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Id does not exist.") }));
        }
        return ((InspectorReferenceData)data__50504).value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object? toObjectForSourceLocation(string id, string? groupName = null)
    {
        object? @object__51282 = toObject(id);
        if ((@object__51282 is Element))
        {
            Element @object__51282__as51313 = (Element)@object__51282;
            return ((Element)((Element)@object__51282__as51313)).widget;
        }
        return @object__51282;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void disposeId(string? id, string groupName)
    {
        if ((id is null))
        {
            return;
        }
        InspectorReferenceData? referenceData__51731 = this._idToReferenceData.GetValueOrDefault(id);
        if ((referenceData__51731 is null))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Id does not exist") }));
        }
        if ((this._groups.GetValueOrDefault(groupName)?.Remove(referenceData__51731) != true))
        {
            throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary("Id is not in group") }));
        }
        _decrementReferenceCount(referenceData__51731);
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
        var directorySet__53593 = new HashSet<string>(pubRootDirectories);
        if ((this._pubRootDirectories is not null))
        {
            directorySet__53593.UnionWith(this._pubRootDirectories!.Cast<string>());
        }
        this._pubRootDirectories = directorySet__53593.ToList();
        this._isLocalCreationCache.clear();
    }

    public virtual void removePubRootDirectories(List<string> pubRootDirectories)
    {
        if ((this._pubRootDirectories is null))
        {
            return;
        }
        pubRootDirectories = pubRootDirectories.map<string, string>(((directory) => DartUri.parse(directory).path)).ToList();
        var directorySet__54396 = new HashSet<string>(this._pubRootDirectories!);
        directorySet__54396.removeAll(pubRootDirectories);
        this._pubRootDirectories = directorySet__54396.ToList();
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
            case global::Doroti.Generated.Framework.Rendering.RenderObject __object56090 when ((!object.Equals(@object, ((InspectorSelection)this.selection).current))):
                {
                    this.selection.clearCandidates();
                    this.selection.current = (global::Doroti.Generated.Framework.Rendering.RenderObject)@object;
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
        global::Doroti.Flutter.Runtime.CreationLocation? location__57195 = ((global::Doroti.Flutter.Runtime.CreationLocation?)(object?)_getSelectedWidgetLocation(restrictToSummaryTree: restrictToProjectFiles));
        if ((location__57195 is not null))
        {
            postEvent("navigate", new DartMap<string, object> { ["fileUri"] = ((object)((dynamic)location__57195).file), ["line"] = ((object)((dynamic)location__57195).line), ["column"] = ((object)((dynamic)location__57195).column), ["source"] = "flutter.inspector" }.cast<object, object>(), stream: "ToolEvent");
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
        if (((global::Doroti.Generated.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress is not null) && (global::Doroti.Generated.Framework.Foundation.DebugLibrary.connectedVmServiceUri is not null)))
        {
            string? inspectorRef__58355 = ((string?)(object?)toId(element, WidgetInspectorService._consoleObjectGroup));
            if ((inspectorRef__58355 is not null))
            {
                return ((string?)(object?)devToolsInspectorUri(inspectorRef__58355));
            }
        }
        return ((string)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string devToolsInspectorUri(string inspectorRef)
    {
        DartRuntimePrimitives.Assert(() => (global::Doroti.Generated.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress is not null));
        DartRuntimePrimitives.Assert(() => (global::Doroti.Generated.Framework.Foundation.DebugLibrary.connectedVmServiceUri is not null));
        DartUri uri__58816 = DartUri.parse(global::Doroti.Generated.Framework.Foundation.DebugLibrary.activeDevToolsServerAddress!.ToString()).replace(queryParameters: new DartMap<string, string> { ["uri"] = global::Doroti.Generated.Framework.Foundation.DebugLibrary.connectedVmServiceUri.ToString(), ["inspectorRef"] = inspectorRef });
        var devToolsInspectorUri__59352 = uri__58816.ToString();
        long startQueryParamIndex__59405 = ((long)((dynamic)devToolsInspectorUri__59352).IndexOf("?"));
        DartRuntimePrimitives.Assert(() => (startQueryParamIndex__59405 != -1L));
        return $"{devToolsInspectorUri__59352.substring(0L, startQueryParamIndex__59405)}" + "/#/inspector" + $"{devToolsInspectorUri__59352.substring(startQueryParamIndex__59405)}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getParentChain(string id, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getParentChain(id, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getParentChain(string? id, string groupName)
    {
        object? value__60294 = toObject(id);
        List<_DiagnosticsPathNode__widget_inspector> path__60353 = (value__60294 switch { global::Doroti.Generated.Framework.Rendering.RenderObject __object60383 => _getRenderObjectParentChain(((global::Doroti.Generated.Framework.Rendering.RenderObject)__object60383), groupName)!, Element __object60455 => _getElementParentChain(((Element)__object60455), groupName), _ => throw DartRuntimePrimitives.AsException(new global::Doroti.Generated.Framework.Foundation.FlutterError(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorSummary($"Cannot get parent chain for node of type {DartRuntimePrimitives.RuntimeType(value__60294)}") })) }).ToList();
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
        List<Element> elements__61327 = ((List<Element>)(object?)element.debugGetDiagnosticChain());
        if ((numLocalParents is not null))
        {
            for (var i__61423 = 0L; (i__61423 < checked((long)(elements__61327.Count))); i__61423 += 1L)
            {
                if (_isValueCreatedByLocalProject(elements__61327[(int)(i__61423)]))
                {
                    numLocalParents = (DartRuntimePrimitives.RequireValue(numLocalParents) - 1L);
                    if ((numLocalParents <= 0L))
                    {
                        elements__61327 = elements__61327.take((i__61423 + 1L)).ToList();
                        break;
                    }
                }
            }
        }
        return System.Linq.Enumerable.Reverse(elements__61327).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_DiagnosticsPathNode__widget_inspector> _getElementParentChain(Element element, string groupName, long? numLocalParents = null)
    {
        return (Widget_inspectorLibrary._followDiagnosticableChain(_getRawElementParentChain(element, numLocalParents: numLocalParents).Cast<global::Doroti.Generated.Framework.Foundation.Diagnosticable>().ToList()) ?? new List<_DiagnosticsPathNode__widget_inspector>());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_DiagnosticsPathNode__widget_inspector>? _getRenderObjectParentChain(global::Doroti.Generated.Framework.Rendering.RenderObject? renderObject, string groupName)
    {
        var chain__62197 = new List<global::Doroti.Generated.Framework.Rendering.RenderObject>();
        while ((renderObject is not null))
        {
            chain__62197.Add(renderObject);
            renderObject = ((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).parent;
        }
        return Widget_inspectorLibrary._followDiagnosticableChain(System.Linq.Enumerable.Reverse(chain__62197).ToList().Cast<global::Doroti.Generated.Framework.Foundation.Diagnosticable>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _nodeToJson(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? node, InspectorSerializationDelegate @delegate, bool fullDetails = true)
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
        global::Doroti.Flutter.Runtime.CreationLocation? creationLocation__63067 = ((global::Doroti.Flutter.Runtime.CreationLocation?)(object?)Widget_inspectorLibrary._getCreationLocation(value));
        if ((creationLocation__63067 is null))
        {
            return false;
        }
        return _isLocalCreationLocation(((string)(object)((object)((dynamic)creationLocation__63067).file)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isLocalCreationLocationImpl(string locationUri)
    {
        string file__63317 = DartUri.parse(locationUri).path;
        if ((this._pubRootDirectories is null))
        {
            return !file__63317.contains("packages/flutter/");
        }
        foreach (string directory__63670 in this._pubRootDirectories!)
        {
            if (file__63317.startsWith(directory__63670))
            {
                return true;
            }
        }
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isLocalCreationLocation(string locationUri)
    {
        bool? cachedValue__63933 = DartCollectionRuntime.NullableMapValue<bool>(this._isLocalCreationCache, locationUri);
        if ((cachedValue__63933 is not null))
        {
            bool cachedValue__63933__value63991 = DartRuntimePrimitives.RequireValue(cachedValue__63933);
            return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(cachedValue__63933__value63991));
        }
        bool result__64061 = _isLocalCreationLocationImpl(locationUri);
        this._isLocalCreationCache[locationUri] = result__64061;
        return result__64061;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string _safeJsonEncode(object? @object)
    {
        string jsonString__64687 = global::Doroti.Flutter.Runtime.Dart_convertLibrary.json.encode(@object);
        this._serializeRing[(int)(this._serializeRingIndex)] = jsonString__64687;
        this._serializeRingIndex = (((this._serializeRingIndex + 1L)) % checked((long)(this._serializeRing.Count)));
        return jsonString__64687;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _truncateNodes(IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, long maxDescendentsTruncatableNode)
    {
        if ((nodes.All(((node) => (((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).value is Element))) && isWidgetCreationTracked()))
        {
            List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> localNodes__65137 = nodes.where(((node) => _isValueCreatedByLocalProject(((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).value))).ToList().ToList();
            if (System.Linq.Enumerable.Any(localNodes__65137))
            {
                return localNodes__65137;
            }
        }
        return nodes.take(maxDescendentsTruncatableNode).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DartMap<string, object>> _nodesToJson(List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? parent)
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
        global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? node__66037 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        if ((node__66037 is null))
        {
            return new List<object>();
        }
        return ((List<object>)(object?)_nodesToJson(node__66037.getProperties().ToList(), new InspectorSerializationDelegate(groupName: groupName, service: this), parent: node__66037));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildren(string diagnosticsNodeId, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getChildren(diagnosticsNodeId, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildren(string? diagnosticsNodeId, string groupName)
    {
        var node__66661 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)toObject(diagnosticsNodeId))!;
        var @delegate__66727 = new InspectorSerializationDelegate(groupName: groupName, service: this);
        return ((List<object>)(object?)_nodesToJson(((node__66661 is null) ? new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>() : _getChildrenFiltered(node__66661, @delegate__66727)), @delegate__66727, parent: node__66661));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildrenSummaryTree(string diagnosticsNodeId, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getChildrenSummaryTree(diagnosticsNodeId, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? _idToDiagnosticsNode(string? diagnosticableId)
    {
        object? @object__67881 = toObject(diagnosticableId);
        return ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)WidgetInspectorService.objectToDiagnosticsNode(@object__67881));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildrenSummaryTree(string? diagnosticableId, string groupName)
    {
        global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? node__68334 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        if ((node__68334 is null))
        {
            return new List<object>();
        }
        var @delegate__68447 = new InspectorSerializationDelegate(groupName: groupName, summaryTree: true, service: this);
        return ((List<object>)(object?)_nodesToJson(_getChildrenFiltered(node__68334, @delegate__68447), @delegate__68447, parent: node__68334));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildrenDetailsSubtree(string diagnosticableId, string groupName)
    {
        return ((string)(object?)_safeJsonEncode(_getChildrenDetailsSubtree(diagnosticableId, groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildrenDetailsSubtree(string? diagnosticableId, string groupName)
    {
        global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? node__69276 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        var @delegate__69419 = new InspectorSerializationDelegate(groupName: groupName, includeProperties: true, service: this);
        return ((List<object>)(object?)_nodesToJson(((node__69276 is null) ? new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>() : _getChildrenFiltered(node__69276, @delegate__69419)), @delegate__69419, parent: node__69276));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _shouldShowInSummaryTree(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node)
    {
        if ((object.Equals(((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).level, global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.error)))
        {
            return true;
        }
        object? value__69855 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).value;
        if ((value__69855 is not global::Doroti.Generated.Framework.Foundation.Diagnosticable))
        {
            return true;
        }
        if (((((global::Doroti.Generated.Framework.Foundation.Diagnosticable)value__69855) is not Element) || !isWidgetCreationTracked()))
        {
            return true;
        }
        return _isValueCreatedByLocalProject(((Element)value__69855));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _getChildrenFiltered(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node, InspectorSerializationDelegate @delegate)
    {
        return ((List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)_filterChildren(node.getChildren().ToList(), @delegate));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _filterChildren(List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate)
    {
        var children__70492 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        foreach (var child__70540 in nodes)
        {
            InspectorSerializationDelegate? updatedDelegate__70741 = ((InspectorSerializationDelegate?)(object?)_updateDelegateForWidgetInspectorEnabledState(@delegate: @delegate, node: child__70540));
            bool inDisableWidgetInspectorScope__71306 = (((updatedDelegate__70741?.inDisableWidgetInspectorScope ?? false)) || ((InspectorSerializationDelegate)@delegate).inDisableWidgetInspectorScope);
            if ((!inDisableWidgetInspectorScope__71306 && ((!((InspectorSerializationDelegate)@delegate).summaryTree || _shouldShowInSummaryTree(child__70540)))))
            {
                children__70492.Add(child__70540);
            }
            else
            {
                children__70492.AddRange(_getChildrenFiltered(child__70540, (updatedDelegate__70741 ?? @delegate)));
            }
        }
        return children__70492;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InspectorSerializationDelegate? _updateDelegateForWidgetInspectorEnabledState(InspectorSerializationDelegate @delegate, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node)
    {
        object? value__72458 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).value;
        if ((!((InspectorSerializationDelegate)@delegate).inDisableWidgetInspectorScope && (value__72458 is _DisableWidgetInspectorScopeProxyElement__widget_inspector)))
        {
            _DisableWidgetInspectorScopeProxyElement__widget_inspector value__72458__as72537 = (_DisableWidgetInspectorScopeProxyElement__widget_inspector)value__72458;
            return ((InspectorSerializationDelegate?)(object?)@delegate.copyWith(inDisableWidgetInspectorScope: true));
        }
        else
        {
            if ((((InspectorSerializationDelegate)@delegate).inDisableWidgetInspectorScope && (value__72458 is _EnableWidgetInspectorScopeProxyElement__widget_inspector)))
            {
                _EnableWidgetInspectorScopeProxyElement__widget_inspector value__72458__as72724 = (_EnableWidgetInspectorScopeProxyElement__widget_inspector)value__72458;
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

    public virtual DartMap<string, object>? _getRootWidgetSummaryTree(string groupName, global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null)
    {
        return ((DartMap<string, object>?)(object?)_getRootWidgetTreeImpl(groupName: groupName, isSummaryTree: true, withPreviews: false, addAdditionalPropertiesCallback: (global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>?)addAdditionalPropertiesCallback));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _getRootWidgetSummaryTreeWithPreviews(DartMap<string, string> parameters)
    {
        string groupName__74112 = parameters.GetValueOrDefault("groupName")!;
        DartMap<string, object?>? result__74182 = ((DartMap<string, object?>?)(object?)_getRootWidgetTreeImpl(groupName: groupName__74112, isSummaryTree: true, withPreviews: true));
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = result__74182 });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _getRootWidgetTree(DartMap<string, string> parameters)
    {
        string groupName__74493 = parameters.GetValueOrDefault("groupName")!;
        var isSummaryTree__74541 = (parameters.GetValueOrDefault("isSummaryTree") == "true");
        var withPreviews__74606 = (parameters.GetValueOrDefault("withPreviews") == "true");
        var fullDetails__74741 = (parameters.GetValueOrDefault("fullDetails") != "false");
        DartMap<string, object?>? result__74826 = ((DartMap<string, object?>?)(object?)_getRootWidgetTreeImpl(groupName: groupName__74493, isSummaryTree: isSummaryTree__74541, withPreviews: withPreviews__74606, fullDetails: fullDetails__74741));
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = result__74826 });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getRootWidgetTreeImpl(string groupName, bool isSummaryTree, bool withPreviews, bool fullDetails = true, global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null)
    {
        bool shouldAddAdditionalProperties__75403 = ((addAdditionalPropertiesCallback is not null) || withPreviews);
        DartMap<string, object>? combinedAddAdditionalPropertiesCallback(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node, InspectorSerializationDelegate @delegate)
        {
            DartMap<string, object> additionalPropertiesJson__75813 = ((addAdditionalPropertiesCallback is null ? new DartMap<string, object>() : addAdditionalPropertiesCallback.Invoke(node, @delegate)));
            if (!withPreviews)
            {
                return additionalPropertiesJson__75813;
            }
            object? value__76023 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).value;
            if ((value__76023 is Element))
            {
                Element value__76023__as76053 = (Element)value__76023;
                global::Doroti.Generated.Framework.Rendering.RenderObject? renderObject__76101 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)_renderObjectOrNull(((Element)value__76023__as76053)));
                if ((renderObject__76101 is global::Doroti.Generated.Framework.Rendering.RenderParagraph))
                {
                    global::Doroti.Generated.Framework.Rendering.RenderParagraph renderObject__76101__as76156 = (global::Doroti.Generated.Framework.Rendering.RenderParagraph)renderObject__76101;
                    additionalPropertiesJson__75813["textPreview"] = ((global::Doroti.Generated.Framework.Rendering.RenderParagraph)((global::Doroti.Generated.Framework.Rendering.RenderParagraph)renderObject__76101__as76156)).text.toPlainText();
                }
            }
            return additionalPropertiesJson__75813;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return ((DartMap<string, object>?)(object?)_nodeToJson(((Diagnosticable)WidgetsBinding.instance.rootElement).toDiagnosticsNode(), new InspectorSerializationDelegate(groupName: groupName, subtreeDepth: 1000000L, summaryTree: isSummaryTree, service: this, addAdditionalPropertiesCallback: ((global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>)(shouldAddAdditionalProperties__75403 ? combinedAddAdditionalPropertiesCallback : null))), fullDetails: fullDetails));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getDetailsSubtree(string diagnosticableId, string groupName, long subtreeDepth = 2)
    {
        return ((string)(object?)_safeJsonEncode(_getDetailsSubtree(diagnosticableId, groupName, subtreeDepth)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getDetailsSubtree(string? diagnosticableId, string? groupName, long subtreeDepth)
    {
        global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? root__77643 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId));
        if ((root__77643 is null))
        {
            return ((DartMap<string, object>)(object)null);
        }
        return ((DartMap<string, object>?)(object?)_nodeToJson(root__77643, new InspectorSerializationDelegate(groupName: groupName, subtreeDepth: subtreeDepth, includeProperties: true, service: this)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getSelectedWidget(string? previousSelectionId, string groupName)
    {
        if ((previousSelectionId is not null))
        {
            global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint("previousSelectionId is deprecated in API");
        }
        return ((string)(object?)_safeJsonEncode(_getSelectedWidget(((string)(object)null), groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<global::Doroti.Flutter.Ui.Image?> screenshot(object? @object, double width, double height, double margin = 0.0, double maxPixelRatio = 1.0, bool debugPaint = false)
    {
        if (((@object is not Element) && (@object is not global::Doroti.Generated.Framework.Rendering.RenderObject)))
        {
            return ((global::Doroti.Flutter.Ui.Image)(object)null);
        }
        global::Doroti.Generated.Framework.Rendering.RenderObject? renderObject__79378 = ((@object is Element) ? _renderObjectOrNull((Element)@object) : (((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)@object)!));
        if (((renderObject__79378 is null) || !((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject__79378).attached))
        {
            return ((global::Doroti.Flutter.Ui.Image)(object)null);
        }
        if (((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject__79378).debugNeedsLayout)
        {
            global::Doroti.Generated.Framework.Rendering.PipelineOwner owner__79637 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.PipelineOwner>(((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject__79378).owner!);
            DartRuntimePrimitives.Assert(() => !((global::Doroti.Generated.Framework.Rendering.PipelineOwner)owner__79637).debugDoingLayout);
            DartRuntimePrimitives.Ignore(((Func<global::Doroti.Generated.Framework.Rendering.PipelineOwner>)(() =>
{            var __cascade = owner__79637;
            __cascade.flushLayout();
            __cascade.flushCompositingBits();
            __cascade.flushPaint();
            return __cascade;        }))());
            if (((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject__79378).debugNeedsLayout)
            {
                return ((global::Doroti.Flutter.Ui.Image)(object)null);
            }
        }
        global::Doroti.Flutter.Ui.Rect renderBounds__80150 = ((global::Doroti.Flutter.Ui.Rect)(object?)DartRuntimePrimitives.ConvertValue<global::Doroti.Flutter.Ui.Rect>(Widget_inspectorLibrary._calculateSubtreeBounds(renderObject__79378)));
        if ((margin != 0.0))
        {
            renderBounds__80150 = renderBounds__80150.inflate(margin);
        }
        if (renderBounds__80150.isEmpty)
        {
            return ((global::Doroti.Flutter.Ui.Image)(object)null);
        }
        double pixelRatio__80361 = Math.Min(maxPixelRatio, Math.Min((width / renderBounds__80150.width), (height / renderBounds__80150.height)));
        return await _ScreenshotPaintingContext__widget_inspector.toImage(renderObject__79378, renderBounds__80150, pixelRatio: pixelRatio__80361, debugPaint: debugPaint);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _getLayoutExplorerNode(DartMap<string, string> parameters)
    {
        string? diagnosticableId__80752 = parameters.GetValueOrDefault("id");
        long subtreeDepth__80803 = Dart_coreLibrary.parse(parameters.GetValueOrDefault("subtreeDepth")!);
        string? groupName__80876 = parameters.GetValueOrDefault("groupName");
        DartMap<string, object>? result__80939 = new DartMap<string, object>().cast<string, object>();
        global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? root__80996 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)_idToDiagnosticsNode(diagnosticableId__80752));
        if ((root__80996 is null))
        {
            return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = result__80939 });
        }
        result__80939 = _nodeToJson(root__80996, new InspectorSerializationDelegate(groupName: groupName__80876, summaryTree: true, subtreeDepth: subtreeDepth__80803, service: this, addAdditionalPropertiesCallback: ((global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>?)((node, @delegate) => {
object? value__81482 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).value;
global::Doroti.Generated.Framework.Rendering.RenderObject? renderObject__81532 = ((value__81482 is Element) ? _renderObjectOrNull(((Element)value__81482)) : null);
if ((renderObject__81532 is null))
{
    return new DartMap<string, object>();
}
global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate renderObjectSerializationDelegate__81746 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate)(object?)@delegate.copyWith(subtreeDepth: 0L, includeProperties: true, expandPropertyValues: false));
var additionalJson__81903 = new DartMap<string, object>();
global::Doroti.Generated.Framework.Rendering.RenderObject? renderParent__82484 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject__81532).parent;
if ((((renderParent__82484 is not null) && (((InspectorSerializationDelegate)@delegate).subtreeDepth > 0L)) && ((InspectorSerializationDelegate)@delegate).expandPropertyValues))
{
    object? parentCreator__82646 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)renderParent__82484).debugCreator;
    if ((parentCreator__82646 is DebugCreator))
    {
        DebugCreator parentCreator__82646__as82705 = (DebugCreator)parentCreator__82646;
        additionalJson__81903["parentRenderElement"] = ((Diagnosticable)((DebugCreator)((DebugCreator)parentCreator__82646__as82705)).element).toDiagnosticsNode().toJsonMap(@delegate.copyWith(subtreeDepth: 0L, includeProperties: true));
    }
}
try
{
    if (!((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject__81532).debugNeedsLayout)
    {
        global::Doroti.Generated.Framework.Rendering.Constraints constraints__83404 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.Constraints>(((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject__81532).constraints);
        var constraintsProperty__83464 = new DartMap<string, object> { ["type"] = DartRuntimePrimitives.RuntimeTypeName(constraints__83404), ["description"] = constraints__83404.ToString() };
        if ((constraints__83404 is global::Doroti.Generated.Framework.Rendering.BoxConstraints))
        {
            global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints__83404__as83654 = (global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__83404;
            constraintsProperty__83464.AddRange(new DartMap<string, object> { ["minWidth"] = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__83404__as83654)).minWidth.ToString(), ["minHeight"] = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__83404__as83654)).minHeight.ToString(), ["maxWidth"] = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__83404__as83654)).maxWidth.ToString(), ["maxHeight"] = ((global::Doroti.Generated.Framework.Rendering.BoxConstraints)((global::Doroti.Generated.Framework.Rendering.BoxConstraints)constraints__83404__as83654)).maxHeight.ToString() });
        }
        additionalJson__81903["constraints"] = constraintsProperty__83464;
    }
}
catch (Exception e__84140)
{
}
try
{
    if ((renderObject__81532 is global::Doroti.Generated.Framework.Rendering.RenderBox))
    {
        global::Doroti.Generated.Framework.Rendering.RenderBox renderObject__81532__as84297 = (global::Doroti.Generated.Framework.Rendering.RenderBox)renderObject__81532;
        additionalJson__81903["isBox"] = true;
        additionalJson__81903["size"] = new DartMap<string, object> { ["width"] = ((global::Doroti.Generated.Framework.Rendering.RenderBox)((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObject__81532__as84297)).size.width.ToString(), ["height"] = ((global::Doroti.Generated.Framework.Rendering.RenderBox)((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObject__81532__as84297)).size.height.ToString() };
        global::Doroti.Generated.Framework.Rendering.ParentData? parentData__84603 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Rendering.ParentData>(((global::Doroti.Generated.Framework.Rendering.RenderBox)renderObject__81532__as84297).parentData);
        if ((parentData__84603 is global::Doroti.Generated.Framework.Rendering.FlexParentData))
        {
            global::Doroti.Generated.Framework.Rendering.FlexParentData parentData__84603__as84659 = (global::Doroti.Generated.Framework.Rendering.FlexParentData)parentData__84603;
            additionalJson__81903["flexFactor"] = (((global::Doroti.Generated.Framework.Rendering.FlexParentData)((global::Doroti.Generated.Framework.Rendering.FlexParentData)parentData__84603__as84659)).flex ?? 0L);
            additionalJson__81903["flexFit"] = ((((global::Doroti.Generated.Framework.Rendering.FlexParentData)((global::Doroti.Generated.Framework.Rendering.FlexParentData)parentData__84603__as84659)).fit ?? global::Doroti.Generated.Framework.Rendering.FlexFit.tight)).ToString();
        }
        else
        {
            if ((parentData__84603 is global::Doroti.Generated.Framework.Rendering.BoxParentData))
            {
                global::Doroti.Generated.Framework.Rendering.BoxParentData parentData__84603__as84869 = (global::Doroti.Generated.Framework.Rendering.BoxParentData)parentData__84603;
                global::Doroti.Flutter.Ui.Offset offset__84929 = ((global::Doroti.Flutter.Ui.Offset)(object?)((global::Doroti.Generated.Framework.Rendering.BoxParentData)((global::Doroti.Generated.Framework.Rendering.BoxParentData)parentData__84603__as84869)).offset);
                additionalJson__81903["parentData"] = new DartMap<string, object> { ["offsetX"] = offset__84929.dx.ToString(), ["offsetY"] = offset__84929.dy.ToString() };
            }
        }
    }
    else
    {
        if ((renderObject__81532 is global::Doroti.Generated.Framework.Rendering.RenderView))
        {
            global::Doroti.Generated.Framework.Rendering.RenderView renderObject__81532__as85182 = (global::Doroti.Generated.Framework.Rendering.RenderView)renderObject__81532;
            additionalJson__81903["size"] = new DartMap<string, object> { ["width"] = ((global::Doroti.Generated.Framework.Rendering.RenderView)((global::Doroti.Generated.Framework.Rendering.RenderView)renderObject__81532__as85182)).size.width.ToString(), ["height"] = ((global::Doroti.Generated.Framework.Rendering.RenderView)((global::Doroti.Generated.Framework.Rendering.RenderView)renderObject__81532__as85182)).size.height.ToString() };
        }
    }
}
catch (Exception e__85443)
{
}
return additionalJson__81903;
throw new InvalidOperationException("Dart closure completed without a value.");
}))));
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = result__80939 });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _setFlexFit(DartMap<string, string> parameters)
    {
        string? id__85737 = parameters.GetValueOrDefault("id");
        string parameter__85777 = parameters.GetValueOrDefault("flexFit")!;
        global::Doroti.Generated.Framework.Rendering.FlexFit flexFit__85831 = _toEnumEntry<global::Doroti.Generated.Framework.Rendering.FlexFit>(System.Enum.GetValues<global::Doroti.Generated.Framework.Rendering.FlexFit>().ToList(), parameter__85777);
        object? @object__85909 = toObject(id__85737);
        var succeed__85940 = false;
        if (((@object__85909 is not null) && (@object__85909 is Element)))
        {
            Element @object__85909__as85983 = (Element)@object__85909;
            global::Doroti.Generated.Framework.Rendering.RenderObject? render__86030 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)_renderObjectOrNull(((Element)@object__85909__as85983)));
            global::Doroti.Generated.Framework.Rendering.ParentData? parentData__86092 = ((global::Doroti.Generated.Framework.Rendering.ParentData?)((dynamic)render__86030)?.parentData);
            if ((parentData__86092 is global::Doroti.Generated.Framework.Rendering.FlexParentData))
            {
                global::Doroti.Generated.Framework.Rendering.FlexParentData parentData__86092__as86135 = (global::Doroti.Generated.Framework.Rendering.FlexParentData)parentData__86092;
                parentData__86092__as86135.fit = flexFit__85831;
                ((dynamic)render__86030!).markNeedsLayout();
                succeed__85940 = true;
            }
        }
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = succeed__85940 });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _setFlexFactor(DartMap<string, string> parameters)
    {
        string? id__86460 = parameters.GetValueOrDefault("id");
        string flexFactor__86500 = parameters.GetValueOrDefault("flexFactor")!;
        long? factor__86555 = ((flexFactor__86500 == "null") ? null : Dart_coreLibrary.parse(flexFactor__86500));
        dynamic @object__86635 = toObject(id__86460);
        var succeed__86666 = false;
        if (((@object__86635 is not null) && (@object__86635 is Element)))
        {
            Element @object__86635__as86709 = (Element)@object__86635;
            global::Doroti.Generated.Framework.Rendering.RenderObject? render__86756 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)_renderObjectOrNull(((Element)@object__86635__as86709)));
            global::Doroti.Generated.Framework.Rendering.ParentData? parentData__86818 = ((global::Doroti.Generated.Framework.Rendering.ParentData?)((dynamic)render__86756)?.parentData);
            if ((parentData__86818 is global::Doroti.Generated.Framework.Rendering.FlexParentData))
            {
                global::Doroti.Generated.Framework.Rendering.FlexParentData parentData__86818__as86861 = (global::Doroti.Generated.Framework.Rendering.FlexParentData)parentData__86818;
                parentData__86818__as86861.flex = factor__86555;
                ((dynamic)render__86756!).markNeedsLayout();
                succeed__86666 = true;
            }
        }
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = succeed__86666 });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object>> _setFlexProperties(DartMap<string, string> parameters)
    {
        string? id__87190 = parameters.GetValueOrDefault("id");
        global::Doroti.Generated.Framework.Rendering.MainAxisAlignment mainAxisAlignment__87241 = _toEnumEntry<global::Doroti.Generated.Framework.Rendering.MainAxisAlignment>(System.Enum.GetValues<global::Doroti.Generated.Framework.Rendering.MainAxisAlignment>().ToList(), parameters.GetValueOrDefault("mainAxisAlignment")!);
        global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment crossAxisAlignment__87402 = _toEnumEntry<global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment>(System.Enum.GetValues<global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment>().ToList(), parameters.GetValueOrDefault("crossAxisAlignment")!);
        object? @object__87556 = toObject(id__87190);
        var succeed__87587 = false;
        if (((@object__87556 is not null) && (@object__87556 is Element)))
        {
            Element @object__87556__as87630 = (Element)@object__87556;
            global::Doroti.Generated.Framework.Rendering.RenderObject? render__87677 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)_renderObjectOrNull(((Element)@object__87556__as87630)));
            if ((render__87677 is global::Doroti.Generated.Framework.Rendering.RenderFlex))
            {
                global::Doroti.Generated.Framework.Rendering.RenderFlex render__87677__as87725 = (global::Doroti.Generated.Framework.Rendering.RenderFlex)render__87677;
                render__87677__as87725.mainAxisAlignment = mainAxisAlignment__87241;
                render__87677__as87725.crossAxisAlignment = crossAxisAlignment__87402;
                ((global::Doroti.Generated.Framework.Rendering.RenderFlex)render__87677__as87725).markNeedsLayout();
                ((global::Doroti.Generated.Framework.Rendering.RenderFlex)render__87677__as87725).markNeedsPaint();
                succeed__87587 = true;
            }
        }
        return Future<DartMap<string, object>>.value(new DartMap<string, object> { ["result"] = succeed__87587 });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T _toEnumEntry<T>(List<T> enumEntries, string name)
    {
        foreach (var entry__88123 in enumEntries)
        {
            if ((entry__88123.ToString() == name))
            {
                return entry__88123;
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

    public virtual global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? _getSelectedWidgetDiagnosticsNode(string? previousSelectionId)
    {
        var previousSelection__88636 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)toObject(previousSelectionId))!;
        Element? current__88726 = ((InspectorSelection)this.selection).currentElement;
        return ((object.Equals(current__88726, previousSelection__88636?.value)) ? previousSelection__88636 : ((Diagnosticable)current__88726).toDiagnosticsNode());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getSelectedSummaryWidget(string? previousSelectionId, string groupName)
    {
        if ((previousSelectionId is not null))
        {
            global::Doroti.Generated.Framework.Foundation.PrintLibrary.debugPrint("previousSelectionId is deprecated in API");
        }
        return ((string)(object?)_safeJsonEncode(_getSelectedSummaryWidget(((string)(object)null), groupName)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Flutter.Runtime.CreationLocation? _getSelectedWidgetLocation(bool restrictToSummaryTree = false)
    {
        global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? selectedNode__89851 = (restrictToSummaryTree ? _getSelectedSummaryDiagnosticsNode(((string)(object)null)) : _getSelectedWidgetDiagnosticsNode(((string)(object)null)));
        return ((global::Doroti.Flutter.Runtime.CreationLocation)(object)Widget_inspectorLibrary._getCreationLocation(selectedNode__89851?.value));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? _getSelectedSummaryDiagnosticsNode(string? previousSelectionId)
    {
        if (!isWidgetCreationTracked())
        {
            return ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)_getSelectedWidgetDiagnosticsNode(previousSelectionId));
        }
        var previousSelection__90258 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)toObject(previousSelectionId))!;
        Element? current__90342 = ((InspectorSelection)this.selection).currentElement;
        if (((current__90342 is not null) && !_isValueCreatedByLocalProject(current__90342)))
        {
            Element? firstLocal__90463 = default!;
            foreach (Element candidate__90500 in current__90342.debugGetDiagnosticChain())
            {
                if (_isValueCreatedByLocalProject(candidate__90500))
                {
                    firstLocal__90463 = candidate__90500;
                    break;
                }
            }
            current__90342 = firstLocal__90463;
        }
        return ((object.Equals(current__90342, previousSelection__90258?.value)) ? previousSelection__90258 : ((Diagnosticable)current__90342).toDiagnosticsNode());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object>? _getSelectedSummaryWidget(string? previousSelectionId, string groupName)
    {
        return ((DartMap<string, object>?)(object?)_nodeToJson(_getSelectedSummaryDiagnosticsNode(previousSelectionId), new InspectorSerializationDelegate(groupName: groupName, service: this)));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isWidgetCreationTracked()
    {
        this._widgetCreationTracked ??= ((global::Doroti.Flutter.Runtime.CreationLocation.of(new _WidgetForTypeTests__widget_inspector()) is not null));
        return DartRuntimePrimitives.RequireValue(this._widgetCreationTracked);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _onFrameStart(Duration timeStamp)
    {
        this._frameStart = timeStamp;
        this._frameNumber = PlatformDispatcher.instance.frameData.frameNumber;
        global::Doroti.Generated.Framework.Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((global::System.Action<Duration>)this._onFrameEnd)(__arg0), debugLabel: "WidgetInspector.onFrameStart");
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

    public virtual void _onPaint(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        try
        {
            Element? element__93316 = DartRuntimePrimitives.ConvertValue<Element>((((DebugCreator?)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)renderObject).debugCreator)!)?.element);
            if ((element__93316 is not RenderObjectElement))
            {
                return;
            }
            this._repaintStats.add(((RenderObjectElement)element__93316));
            ((RenderObjectElement)element__93316).visitAncestorElements(((global::System.Func<Element, bool>)((ancestor) => {
if ((ancestor is RenderObjectElement))
{
    return false;
}
this._repaintStats.add(ancestor);
return true;
throw new InvalidOperationException("Dart closure completed without a value.");
})));
        }
        catch (Exception exception__94210)
        {
            var stack__94221 = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: exception__94210, stack: stack__94221, library: "widget inspector library", context: new global::Doroti.Generated.Framework.Foundation.ErrorDescription("while tracking widget repaints")));
        }
    }

    public virtual void performReassemble()
    {
        _clearStats();
        _resetErrorCount();
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RenderObject? _renderObjectOrNull(Element element) => (((Element)element).mounted ? ((Element)element).renderObject : null);
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
    public void _reportStructuredError(global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails details);
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
    public List<_DiagnosticsPathNode__widget_inspector>? _getRenderObjectParentChain(global::Doroti.Generated.Framework.Rendering.RenderObject? renderObject, string groupName);
    public DartMap<string, object?>? _nodeToJson(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? node, InspectorSerializationDelegate @delegate, bool fullDetails = true);
    public bool _isValueCreatedByLocalProject(object? value);
    public bool _isLocalCreationLocationImpl(string locationUri);
    public bool _isLocalCreationLocation(string locationUri);
    public string _safeJsonEncode(object? @object);
    public List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _truncateNodes(IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, long maxDescendentsTruncatableNode);
    public List<DartMap<string, object?>> _nodesToJson(List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? parent);
    public string getProperties(string diagnosticsNodeId, string groupName);
    public List<object> _getProperties(string? diagnosticableId, string groupName);
    public string getChildren(string diagnosticsNodeId, string groupName);
    public List<object> _getChildren(string? diagnosticsNodeId, string groupName);
    public string getChildrenSummaryTree(string diagnosticsNodeId, string groupName);
    public global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? _idToDiagnosticsNode(string? diagnosticableId);
    public static global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? objectToDiagnosticsNode(object? @object)
    {
        if ((@object is global::Doroti.Generated.Framework.Foundation.Diagnosticable))
        {
            global::Doroti.Generated.Framework.Foundation.Diagnosticable @object__as68125 = (global::Doroti.Generated.Framework.Foundation.Diagnosticable)@object;
            return ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode?)(object?)((Diagnosticable)((global::Doroti.Generated.Framework.Foundation.Diagnosticable)@object__as68125)).toDiagnosticsNode());
        }
        return ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)(object)null);
    }
    public List<object> _getChildrenSummaryTree(string? diagnosticableId, string groupName);
    public string getChildrenDetailsSubtree(string diagnosticableId, string groupName);
    public List<object> _getChildrenDetailsSubtree(string? diagnosticableId, string groupName);
    public bool _shouldShowInSummaryTree(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node);
    public List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _getChildrenFiltered(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node, InspectorSerializationDelegate @delegate);
    public List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _filterChildren(List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate);
    public InspectorSerializationDelegate? _updateDelegateForWidgetInspectorEnabledState(InspectorSerializationDelegate @delegate, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node);
    public string getRootWidget(string groupName);
    public DartMap<string, object?>? _getRootWidget(string groupName);
    public string getRootWidgetSummaryTree(string groupName);
    public DartMap<string, object?>? _getRootWidgetSummaryTree(string groupName, global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null);
    public Future<DartMap<string, object?>> _getRootWidgetSummaryTreeWithPreviews(DartMap<string, string> parameters);
    public Future<DartMap<string, object?>> _getRootWidgetTree(DartMap<string, string> parameters);
    public DartMap<string, object?>? _getRootWidgetTreeImpl(string groupName, bool isSummaryTree, bool withPreviews, bool fullDetails = true, global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null);
    public string getDetailsSubtree(string diagnosticableId, string groupName, long subtreeDepth = 2);
    public DartMap<string, object?>? _getDetailsSubtree(string? diagnosticableId, string? groupName, long subtreeDepth);
    public string getSelectedWidget(string? previousSelectionId, string groupName);
    public Future<global::Doroti.Flutter.Ui.Image?> screenshot(object? @object, double width, double height, double margin = 0.0, double maxPixelRatio = 1.0, bool debugPaint = false);
    public Future<DartMap<string, object?>> _getLayoutExplorerNode(DartMap<string, string> parameters);
    public Future<DartMap<string, object>> _setFlexFit(DartMap<string, string> parameters);
    public Future<DartMap<string, object>> _setFlexFactor(DartMap<string, string> parameters);
    public Future<DartMap<string, object>> _setFlexProperties(DartMap<string, string> parameters);
    public T _toEnumEntry<T>(List<T> enumEntries, string name);
    public DartMap<string, object?>? _getSelectedWidget(string? previousSelectionId, string groupName);
    public global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? _getSelectedWidgetDiagnosticsNode(string? previousSelectionId);
    public string getSelectedSummaryWidget(string? previousSelectionId, string groupName);
    public global::Doroti.Flutter.Runtime.CreationLocation? _getSelectedWidgetLocation(bool restrictToSummaryTree = false);
    public global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? _getSelectedSummaryDiagnosticsNode(string? previousSelectionId);
    public DartMap<string, object?>? _getSelectedSummaryWidget(string? previousSelectionId, string groupName);
    public bool isWidgetCreationTracked();
    public void _onFrameStart(Duration timeStamp);
    public void _onFrameEnd(Duration timeStamp);
    public void _postStatsEvent(string eventName, _ElementLocationStatsTracker__widget_inspector stats);
    public void postEvent(string eventKind, DartMap<object, object> eventData, string stream = "Extension");
    public void inspect(object? @object);
    public void _onRebuildWidget(Element element, bool builtOnce);
    public void _onPaint(global::Doroti.Generated.Framework.Rendering.RenderObject renderObject);
    public global::Doroti.Generated.Framework.Rendering.RenderObject? _renderObjectOrNull(Element element);
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
        object widget__97346 = ((Element)element).widget;
        global::Doroti.Flutter.Runtime.CreationLocation? location__97409 = ((global::Doroti.Flutter.Runtime.CreationLocation?)(object?)global::Doroti.Flutter.Runtime.CreationLocation.of(widget__97346));
        if ((location__97409 is null))
        {
            return;
        }
        long id__97521 = Widget_inspectorLibrary._toLocationId(location__97409);
        _LocationCount__widget_inspector entry__97571 = default!;
        if (((id__97521 >= checked((long)(this._stats.Count))) || (this._stats[(int)(id__97521)] is null)))
        {
            while ((id__97521 >= checked((long)(this._stats.Count))))
            {
                this._stats.Add(((_LocationCount__widget_inspector)(object)null));
            }
            entry__97571 = new _LocationCount__widget_inspector(location: location__97409, id: id__97521, local: WidgetInspectorService.instance._isLocalCreationLocation(((string)(object)((object)((dynamic)location__97409).file))));
            if (((_LocationCount__widget_inspector)entry__97571).local)
            {
                this.newLocations.Add(entry__97571);
            }
            this._stats[(int)(id__97521)] = entry__97571;
        }
        else
        {
            entry__97571 = this._stats[(int)(id__97521)]!;
        }
        if (((_LocationCount__widget_inspector)entry__97571).local)
        {
            if ((((_LocationCount__widget_inspector)entry__97571).count == 0L))
            {
                this.active.Add(entry__97571);
            }
            entry__97571.increment();
        }
    }

    public virtual void resetCounts()
    {
        foreach (_LocationCount__widget_inspector entry__99103 in this.active)
        {
            entry__99103.reset();
        }
        this.active.Clear();
    }

    public virtual DartMap<string, object> exportToJson(Duration startTime, long frameNumber)
    {
        var events__99379 = new List<long>(System.Linq.Enumerable.Repeat<long>(0L, checked((int)(checked((long)(this.active.Count)) * 2L))));
        var j__99436 = 0L;
        foreach (_LocationCount__widget_inspector stat__99473 in this.active)
        {
            events__99379[(int)(j__99436++)] = ((_LocationCount__widget_inspector)stat__99473).id;
            events__99379[(int)(j__99436++)] = ((_LocationCount__widget_inspector)stat__99473).count;
        }
        var json__99569 = new DartMap<string, object> { ["startTime"] = startTime.inMicroseconds, ["frameNumber"] = frameNumber, ["events"] = events__99379 };
        if (System.Linq.Enumerable.Any(this.newLocations))
        {
            var locationsJson__99865 = new DartMap<string, List<long>>();
            foreach (_LocationCount__widget_inspector entry__99936 in this.newLocations)
            {
                global::Doroti.Flutter.Runtime.CreationLocation location__100002 = ((global::Doroti.Flutter.Runtime.CreationLocation)(object?)((_LocationCount__widget_inspector)entry__99936).location);
                List<long> jsonForFile__100053 = locationsJson__99865.putIfAbsent(((string)(object)((object)((dynamic)location__100002).file)), (() => new List<long>())).ToList();
                DartRuntimePrimitives.Ignore(((Func<List<long>>)(() =>
{            var __cascade = jsonForFile__100053;
            __cascade.Add(((_LocationCount__widget_inspector)entry__99936).id);
            __cascade.Add(((long)(object)((object)((dynamic)location__100002).line)));
            __cascade.Add(((long)(object)((object)((dynamic)location__100002).column)));
            return __cascade;        }))());
            }
            json__99569["newLocations"] = locationsJson__99865;
        }
        if (System.Linq.Enumerable.Any(this.newLocations))
        {
            var fileLocationsMap__100414 = new DartMap<string, DartMap<string, List<object>>>();
            foreach (_LocationCount__widget_inspector entry__100505 in this.newLocations)
            {
                global::Doroti.Flutter.Runtime.CreationLocation location__100571 = ((global::Doroti.Flutter.Runtime.CreationLocation)(object?)((_LocationCount__widget_inspector)entry__100505).location);
                DartMap<string, List<object?>> locations__100639 = fileLocationsMap__100414.putIfAbsent(((string)(object)((object)((dynamic)location__100571).file)), (() => new DartMap<string, List<object>> { ["ids"] = new List<long>().Cast<object>().ToList(), ["lines"] = new List<long>().Cast<object>().ToList(), ["columns"] = new List<long>().Cast<object>().ToList(), ["names"] = new List<string?>().Cast<object>().ToList() })).cast<string, List<object?>>();
                locations__100639.GetValueOrDefault("ids")!.Add(((_LocationCount__widget_inspector)entry__100505).id);
                locations__100639.GetValueOrDefault("lines")!.Add(((object)((dynamic)location__100571).line));
                locations__100639.GetValueOrDefault("columns")!.Add(((object)((dynamic)location__100571).column));
                locations__100639.GetValueOrDefault("names")!.Add(location__100571.ToString());
            }
            json__99569["locations"] = fileLocationsMap__100414;
        }
        resetCounts();
        this.newLocations.Clear();
        return json__99569;
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

    public WidgetInspector(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!, TapBehaviorButtonBuilder? tapBehaviorButtonBuilder = default!, ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder = default!, MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder = default!) : base(key: key)
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

    internal virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool> _selectionOnTapEnabled => WidgetsBinding.instance.debugWidgetInspectorSelectionOnTapEnabled;
    internal virtual bool _isSelectModeWithSelectionOnTapEnabled => DartRuntimePrimitives.ConvertValue<bool>((this.isSelectMode && ((global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool>)this._selectionOnTapEnabled).value));
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

    internal virtual void _selectionInformationChanged() => setState(((global::System.Action)(() => {
selection = WidgetInspectorService.instance.selection;
isSelectMode = WidgetsBinding.instance.debugShowWidgetInspectorOverride;
})));
    internal virtual bool _hitTestHelper(List<global::Doroti.Generated.Framework.Rendering.RenderObject> hits, List<global::Doroti.Generated.Framework.Rendering.RenderObject> edgeHits, Offset position, global::Doroti.Generated.Framework.Rendering.RenderObject @object, Matrix4 transform)
    {
        var hit__105994 = false;
        Matrix4? inverse__106026 = Matrix4.tryInvert(transform);
        if ((inverse__106026 is null))
        {
            return false;
        }
        global::Doroti.Flutter.Ui.Offset localPosition__106252 = ((global::Doroti.Flutter.Ui.Offset)(object?)MatrixUtils.transformPoint(inverse__106026, position));
        List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> children__106348 = ((List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)((List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)((dynamic)@object).debugDescribeChildren()));
        for (long i__106404 = (checked((long)(children__106348.Count)) - 1L); (i__106404 >= 0L); i__106404 -= 1L)
        {
            global::Doroti.Generated.Framework.Foundation.DiagnosticsNode diagnostics__106475 = children__106348[(int)(i__106404)];
            if (((object.Equals(((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)diagnostics__106475).style, global::Doroti.Generated.Framework.Foundation.DiagnosticsTreeStyle.offstage)) || (((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)diagnostics__106475).value is not global::Doroti.Generated.Framework.Rendering.RenderObject)))
            {
                continue;
            }
            var child__106652 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)diagnostics__106475).value!)!;
            global::Doroti.Flutter.Ui.Rect? paintClip__106714 = ((global::Doroti.Flutter.Ui.Rect?)(object?)((Rect?)((dynamic)@object).describeApproximatePaintClip(child__106652)));
            if (((paintClip__106714 is not null) && !DartRuntimePrimitives.RequireValue(paintClip__106714).contains(localPosition__106252)))
            {
                Rect paintClip__106714__value106780 = DartRuntimePrimitives.RequireValue(paintClip__106714);
                continue;
            }
            Matrix4 childTransform__106886 = transform.clone();
            ((dynamic)@object).applyPaintTransform(child__106652, childTransform__106886);
            if (_hitTestHelper(hits, edgeHits, position, child__106652, childTransform__106886))
            {
                hit__105994 = true;
            }
        }
        global::Doroti.Flutter.Ui.Rect bounds__107106 = ((global::Doroti.Flutter.Ui.Rect)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)@object).semanticBounds);
        if (bounds__107106.contains(localPosition__106252))
        {
            hit__105994 = true;
            if (!bounds__107106.deflate(_edgeHitMargin).contains(localPosition__106252))
            {
                edgeHits.Add(@object);
            }
        }
        if (hit__105994)
        {
            hits.Add(@object);
        }
        return hit__105994;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Rendering.RenderObject> hitTest(Offset position, global::Doroti.Generated.Framework.Rendering.RenderObject root)
    {
        var regularHits__107997 = new List<global::Doroti.Generated.Framework.Rendering.RenderObject>();
        var edgeHits__108039 = new List<global::Doroti.Generated.Framework.Rendering.RenderObject>();
        _hitTestHelper(regularHits__107997, edgeHits__108039, position, root, ((Matrix4)((dynamic)root).getTransformTo(((global::Doroti.Generated.Framework.Rendering.RenderObject)(object)null))));
        double area(global::Doroti.Generated.Framework.Rendering.RenderObject @object)
        {
            global::Doroti.Flutter.Ui.Size size__108261 = ((global::Doroti.Flutter.Ui.Size)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)@object).semanticBounds.size);
            return (size__108261.width * size__108261.height);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        regularHits__107997.sort(((a, b) => area(a).CompareTo(area(b))));
        var hits__108438 = new HashSet<global::Doroti.Generated.Framework.Rendering.RenderObject>();
        return hits__108438.ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _inspectAt(Offset position)
    {
        if (!this._isSelectModeWithSelectionOnTapEnabled)
        {
            return;
        }
        var ignorePointer__108640 = ((global::Doroti.Generated.Framework.Rendering.RenderIgnorePointer?)(object?)((GlobalKey<IState>)this._ignorePointerKey).currentContext!.findRenderObject()!)!;
        global::Doroti.Generated.Framework.Rendering.RenderObject userRender__108765 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)((global::Doroti.Generated.Framework.Rendering.RenderBox?)((dynamic)ignorePointer__108640).child)!);
        List<global::Doroti.Generated.Framework.Rendering.RenderObject> selected__108829 = ((List<global::Doroti.Generated.Framework.Rendering.RenderObject>)(object?)hitTest(position, userRender__108765));
        this.selection.candidates = Widget_inspectorLibrary._filterInspectorHitCandidatesToModalRouteScope(selected__108829);
    }

    internal virtual void _handlePanDown(global::Doroti.Generated.Framework.Gestures.DragDownDetails @event)
    {
        _lastPointerLocation = ((global::Doroti.Generated.Framework.Gestures.DragDownDetails)@event).globalPosition;
        _inspectAt(((global::Doroti.Generated.Framework.Gestures.DragDownDetails)@event).globalPosition);
    }

    internal virtual void _handlePanUpdate(global::Doroti.Generated.Framework.Gestures.DragUpdateDetails @event)
    {
        _lastPointerLocation = ((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)@event).globalPosition;
        _inspectAt(((global::Doroti.Generated.Framework.Gestures.DragUpdateDetails)@event).globalPosition);
    }

    internal virtual void _handlePanEnd(global::Doroti.Generated.Framework.Gestures.DragEndDetails details)
    {
        global::Doroti.Flutter.Ui.FlutterView view__109703 = ((global::Doroti.Flutter.Ui.FlutterView)(object?)View.of(this.context));
        global::Doroti.Flutter.Ui.Rect bounds__109743 = ((global::Doroti.Flutter.Ui.Rect)(object?)((Offset.zero & ((view__109703.physicalSize / view__109703.devicePixelRatio)))).deflate(Widget_inspectorLibrary._kOffScreenMargin));
        if (!bounds__109743.contains(DartRuntimePrimitives.RequireValue(this._lastPointerLocation)))
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
        return ((Widget)(object?)new Stack(children: new List<Widget> { new GestureDetector(onTap: () => this._handleTap(), onPanDown: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragDownDetails>)this._handlePanDown, onPanEnd: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragEndDetails>)this._handlePanEnd, onPanUpdate: (global::System.Action<global::Doroti.Generated.Framework.Gestures.DragUpdateDetails>)this._handlePanUpdate, behavior: global::Doroti.Generated.Framework.Rendering.HitTestBehavior.opaque, excludeFromSemantics: true, child: new IgnorePointer(ignoring: this._isSelectModeWithSelectionOnTapEnabled, key: this._ignorePointerKey, child: ((WidgetInspector)this.widget).child)), Positioned.CreateFill(child: new _InspectorOverlay__widget_inspector(selection: this.selection)) }));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EnableWidgetInspectorScope : ProxyWidget
{
    public EnableWidgetInspectorScope(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
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
    public DisableWidgetInspectorScope(global::Doroti.Generated.Framework.Foundation.Key? key = null, Widget child = default!) : base(key: key, child: child)
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

    protected InspectorButton(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, GlobalKey<IState>? buttonKey = null, InspectorButtonVariant variant = default!, bool? toggledOn = null) : base(key: key)
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

    protected static InspectorButton CreateFilled(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, GlobalKey<IState>? buttonKey = null)
    {
        throw new InvalidOperationException("Dart abstract constructors cannot be invoked directly.");
    }

    protected static InspectorButton CreateToggle(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, bool toggledOn = true)
    {
        throw new InvalidOperationException("Dart abstract constructors cannot be invoked directly.");
    }

    protected static InspectorButton CreateIconOnly(global::Doroti.Generated.Framework.Foundation.Key? key = null, global::System.Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!)
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
    public abstract global::Doroti.Flutter.Ui.Color foregroundColor(BuildContext context);
    public abstract global::Doroti.Flutter.Ui.Color backgroundColor(BuildContext context);
    public abstract override Widget build(BuildContext context);
}

public class InspectorSelection : ChangeNotifier
{
    internal virtual List<global::Doroti.Generated.Framework.Rendering.RenderObject> _candidates { get; set; } = new List<global::Doroti.Generated.Framework.Rendering.RenderObject>();
    internal virtual long _index { get; set; } = 0L;
    internal virtual global::Doroti.Generated.Framework.Rendering.RenderObject? _current { get; set; } = default;
    internal virtual Element? _currentElement { get; set; } = default;

    public InspectorSelection()
    {
    }

    public virtual List<global::Doroti.Generated.Framework.Rendering.RenderObject> candidates
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
        _candidates = new List<global::Doroti.Generated.Framework.Rendering.RenderObject>();
        _index = 0L;
        _computeCurrent();
    }

    public virtual void clearCandidates()
    {
        if (!System.Linq.Enumerable.Any(this._candidates))
        {
            return;
        }
        _candidates = new List<global::Doroti.Generated.Framework.Rendering.RenderObject>();
        _index = 0L;
    }

    public virtual global::Doroti.Generated.Framework.Rendering.RenderObject? current
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
        get{
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

    public override global::Doroti.Generated.Framework.Rendering.RenderObject createRenderObject(BuildContext context)
    {
        return ((global::Doroti.Generated.Framework.Rendering.RenderObject)(object?)new _RenderInspectorOverlay__widget_inspector(selection: this.selection));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, global::Doroti.Generated.Framework.Rendering.RenderObject renderObject)
    {
        var __renderObject = (_RenderInspectorOverlay__widget_inspector)(object)renderObject;
        __renderObject.selection = this.selection;
    }

}

public class _RenderInspectorOverlay__widget_inspector : global::Doroti.Generated.Framework.Rendering.RenderBox
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
    public override Size computeDryLayout(global::Doroti.Generated.Framework.Rendering.BoxConstraints constraints)
    {
        return constraints.constrain(Size.infinite);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(global::Doroti.Generated.Framework.Rendering.PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => this.needsCompositing);
        context.addLayer(new _InspectorOverlayLayer__widget_inspector(overlayRect: global::Doroti.Flutter.Ui.Rect.fromLTWH(offset.dx, offset.dy, this.size.width, this.size.height), selection: this.selection, rootRenderObject: (true ? this.parent! : null)));
    }

}

public class _TransformedRect__widget_inspector
{
    public virtual Rect rect { get; private set; } = default!;
    public virtual Matrix4 transform { get; private set; } = default!;

    internal _TransformedRect__widget_inspector(global::Doroti.Generated.Framework.Rendering.RenderObject @object, global::Doroti.Generated.Framework.Rendering.RenderObject? ancestor)
    {
        this.rect = ((global::Doroti.Generated.Framework.Rendering.RenderObject)@object).semanticBounds;
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
        return (((((__other is _InspectorOverlayRenderState__widget_inspector) && (object.Equals(((_InspectorOverlayRenderState__widget_inspector)((_InspectorOverlayRenderState__widget_inspector)__other)).overlayRect, this.overlayRect))) && (object.Equals(((_InspectorOverlayRenderState__widget_inspector)((_InspectorOverlayRenderState__widget_inspector)__other)).selected, this.selected))) && global::Doroti.Generated.Framework.Foundation.CollectionsLibrary.listEquals<_TransformedRect__widget_inspector>(((_InspectorOverlayRenderState__widget_inspector)((_InspectorOverlayRenderState__widget_inspector)__other)).candidates, this.candidates)) && (((_InspectorOverlayRenderState__widget_inspector)((_InspectorOverlayRenderState__widget_inspector)__other)).tooltip == this.tooltip));
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(this.overlayRect, this.selected, FoundationRuntimePorts.ObjectHashAll(this.candidates), this.tooltip));
}

public static partial class Widget_inspectorLibrary
{
    internal static long _kMaxTooltipLines = 5L;
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kTooltipBackgroundColor = global::Doroti.Flutter.Ui.Color.fromARGB(230L, 60L, 60L, 60L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kHighlightedRenderObjectFillColor = global::Doroti.Flutter.Ui.Color.fromARGB(128L, 128L, 128L, 255L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kHighlightedRenderObjectBorderColor = global::Doroti.Flutter.Ui.Color.fromARGB(128L, 64L, 64L, 128L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Element? _elementForRenderObject(global::Doroti.Generated.Framework.Rendering.RenderObject? @object)
    {
        object? creator__124233 = ((dynamic)@object)?.debugCreator;
        if ((creator__124233 is DebugCreator))
        {
            DebugCreator creator__124233__as124271 = (DebugCreator)creator__124233;
            return ((DebugCreator)((DebugCreator)creator__124233__as124271)).element;
        }
        return ((Element)(object)null);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static dynamic _modalRouteForRenderObject(global::Doroti.Generated.Framework.Rendering.RenderObject? @object)
    {
        Element? element__124437 = Widget_inspectorLibrary._elementForRenderObject(@object);
        if ((element__124437 is null))
        {
            return null;
        }
        return ModalRoute<object>.of<object?>(element__124437);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static double _inspectorHitArea(global::Doroti.Generated.Framework.Rendering.RenderObject @object)
    {
        global::Doroti.Flutter.Ui.Size size__124632 = ((global::Doroti.Flutter.Ui.Size)(object?)((global::Doroti.Generated.Framework.Rendering.RenderObject)@object).semanticBounds.size);
        return (size__124632.width * size__124632.height);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static dynamic _inspectorScopeRouteForHits(List<global::Doroti.Generated.Framework.Rendering.RenderObject> hits)
    {
        foreach (var hit__124794 in hits)
        {
            dynamic route__124840 = Widget_inspectorLibrary._modalRouteForRenderObject(hit__124794);
            if ((((bool?)((dynamic)route__124840)?.isCurrent) ?? false))
            {
                return route__124840;
            }
        }
        global::Doroti.Generated.Framework.Rendering.RenderObject? smallestHit__125107 = default!;
        double smallestArea__125129 = double.PositiveInfinity;
        foreach (var hit__125174 in hits)
        {
            dynamic route__125220 = Widget_inspectorLibrary._modalRouteForRenderObject(hit__125174);
            if ((route__125220 is null))
            {
                continue;
            }
            double area__125325 = Widget_inspectorLibrary._inspectorHitArea(hit__125174);
            if ((area__125325 < smallestArea__125129))
            {
                smallestArea__125129 = area__125325;
                smallestHit__125107 = hit__125174;
            }
        }
        if ((smallestHit__125107 is not null))
        {
            return Widget_inspectorLibrary._modalRouteForRenderObject(smallestHit__125107);
        }
        return Widget_inspectorLibrary._modalRouteForRenderObject(hits.First());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static List<global::Doroti.Generated.Framework.Rendering.RenderObject> _filterInspectorHitCandidatesToModalRouteScope(List<global::Doroti.Generated.Framework.Rendering.RenderObject> hits)
    {
        if (!System.Linq.Enumerable.Any(hits))
        {
            return hits;
        }
        List<global::Doroti.Generated.Framework.Rendering.RenderObject> onstageHits__125809 = hits.where(((hit) => {
dynamic route__125897 = Widget_inspectorLibrary._modalRouteForRenderObject(hit);
return ((route__125897 is null) || !((bool)((dynamic)route__125897).offstage));
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList().ToList();
        if (!System.Linq.Enumerable.Any(onstageHits__125809))
        {
            return onstageHits__125809;
        }
        dynamic scopeRoute__126100 = Widget_inspectorLibrary._inspectorScopeRouteForHits(onstageHits__125809);
        List<global::Doroti.Generated.Framework.Rendering.RenderObject> scopedHits__126182 = onstageHits__125809.where(((hit) => DartRuntimePrimitives.Identical(Widget_inspectorLibrary._modalRouteForRenderObject(hit), scopeRoute__126100))).ToList().ToList();
        scopedHits__126182.sort(((a, b) => Widget_inspectorLibrary._inspectorHitArea(a).CompareTo(Widget_inspectorLibrary._inspectorHitArea(b))));
        return scopedHits__126182;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _InspectorOverlayLayer__widget_inspector : global::Doroti.Generated.Framework.Rendering.Layer
{
    public virtual InspectorSelection selection { get; set; } = default!;
    public virtual Rect overlayRect { get; private set; } = default!;
    public virtual global::Doroti.Generated.Framework.Rendering.RenderObject? rootRenderObject { get; private set; }
    internal virtual _InspectorOverlayRenderState__widget_inspector? _lastState { get; set; } = default;
    internal virtual Picture? _picture { get; set; } = default;
    internal virtual global::Doroti.Generated.Framework.Painting.TextPainter? _textPainter { get; set; } = default;
    internal virtual double? _textPainterMaxWidth { get; set; } = default;

    internal _InspectorOverlayLayer__widget_inspector(Rect overlayRect, InspectorSelection selection, global::Doroti.Generated.Framework.Rendering.RenderObject? rootRenderObject)
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
        global::Doroti.Generated.Framework.Rendering.RenderObject selected__128250 = ((InspectorSelection)this.selection).current!;
        if (!_isInInspectorRenderObjectTree(selected__128250))
        {
            return;
        }
        var candidates__128366 = new List<_TransformedRect__widget_inspector>();
        foreach (global::Doroti.Generated.Framework.Rendering.RenderObject candidate__128429 in ((InspectorSelection)this.selection).candidates)
        {
            if (((((object.Equals(candidate__128429, selected__128250)) || !((global::Doroti.Generated.Framework.Rendering.RenderObject)candidate__128429).attached) || !_isInInspectorRenderObjectTree(candidate__128429)) || !DartRuntimePrimitives.Identical(Widget_inspectorLibrary._modalRouteForRenderObject(candidate__128429), Widget_inspectorLibrary._modalRouteForRenderObject(selected__128250))))
            {
                continue;
            }
            candidates__128366.Add(new _TransformedRect__widget_inspector(candidate__128429, this.rootRenderObject));
        }
        var selectedRect__128839 = new _TransformedRect__widget_inspector(selected__128250, this.rootRenderObject);
        string widgetName__128917 = ((string)(object?)((Diagnosticable)((InspectorSelection)this.selection).currentElement!).toStringShort());
        string width__128990 = ((_TransformedRect__widget_inspector)selectedRect__128839).rect.width.toStringAsFixed(1L);
        string height__129059 = ((_TransformedRect__widget_inspector)selectedRect__128839).rect.height.toStringAsFixed(1L);
        var state__129124 = new _InspectorOverlayRenderState__widget_inspector(overlayRect: this.overlayRect, selected: selectedRect__128839, tooltip: $"{widgetName__128917} ({width__128990} x {height__129059})", textDirection: TextDirection.ltr, candidates: candidates__128366);
        if ((!object.Equals(state__129124, this._lastState)))
        {
            _lastState = state__129124;
            this._picture?.dispose();
            _picture = _buildPicture(state__129124);
        }
        builder.addPicture(Offset.zero, this._picture!);
    }

    internal virtual global::Doroti.Flutter.Ui.Picture _buildPicture(_InspectorOverlayRenderState__widget_inspector state)
    {
        var recorder__129608 = new global::Doroti.Flutter.Ui.PictureRecorder();
        var canvas__129651 = new global::Doroti.Flutter.Ui.Canvas(recorder__129608, ((_InspectorOverlayRenderState__widget_inspector)state).overlayRect);
        global::Doroti.Flutter.Ui.Size size__129712 = ((global::Doroti.Flutter.Ui.Size)(object?)((_InspectorOverlayRenderState__widget_inspector)state).overlayRect.size);
        canvas__129651.translate(((_InspectorOverlayRenderState__widget_inspector)state).overlayRect.left, ((_InspectorOverlayRenderState__widget_inspector)state).overlayRect.top);
        var fillPaint__129929 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.style = PaintingStyle.fill;
            __cascade.color = Widget_inspectorLibrary._kHighlightedRenderObjectFillColor;
            return __cascade;        }))();
        var borderPaint__130047 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.style = PaintingStyle.stroke;
            __cascade.strokeWidth = 1.0;
            __cascade.color = Widget_inspectorLibrary._kHighlightedRenderObjectBorderColor;
            return __cascade;        }))();
        global::Doroti.Flutter.Ui.Rect selectedPaintRect__130246 = ((global::Doroti.Flutter.Ui.Rect)(object?)((_InspectorOverlayRenderState__widget_inspector)state).selected.rect.deflate(0.5));
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = canvas__129651;
            __cascade.save();
            __cascade.transform(((_InspectorOverlayRenderState__widget_inspector)state).selected.transform.storage);
            __cascade.drawRect(selectedPaintRect__130246, fillPaint__129929);
            __cascade.drawRect(selectedPaintRect__130246, borderPaint__130047);
            __cascade.restore();
            return __cascade;        }))());
        foreach (_TransformedRect__widget_inspector transformedRect__130742 in ((_InspectorOverlayRenderState__widget_inspector)state).candidates)
        {
            DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{            var __cascade = canvas__129651;
            __cascade.save();
            __cascade.transform(((_TransformedRect__widget_inspector)transformedRect__130742).transform.storage);
            __cascade.drawRect(((_TransformedRect__widget_inspector)transformedRect__130742).rect.deflate(0.5), borderPaint__130047);
            __cascade.restore();
            return __cascade;        }))());
        }
        global::Doroti.Flutter.Ui.Rect targetRect__130976 = ((global::Doroti.Flutter.Ui.Rect)(object?)MatrixUtils.transformRect(((_InspectorOverlayRenderState__widget_inspector)state).selected.transform, ((_InspectorOverlayRenderState__widget_inspector)state).selected.rect));
        if (!targetRect__130976.hasNaN)
        {
            var target__131124 = new global::Doroti.Flutter.Ui.Offset(targetRect__130976.left, ((Offset)((dynamic)targetRect__130976).center).dy);
            var offsetFromWidget__131192 = 9.0;
            double verticalOffset__131235 = ((targetRect__130976.height / 2L) + offsetFromWidget__131192);
            _paintDescription(canvas__129651, ((_InspectorOverlayRenderState__widget_inspector)state).tooltip, ((_InspectorOverlayRenderState__widget_inspector)state).textDirection, target__131124, verticalOffset__131235, size__129712, targetRect__130976);
        }
        return ((global::Doroti.Flutter.Ui.Picture)(object?)recorder__129608.endRecording());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintDescription(Canvas canvas, string message, TextDirection textDirection, Offset target, double verticalOffset, Size size, Rect targetRect)
    {
        canvas.save();
        double maxWidth__131840 = Math.Max((size.width - (2L * ((Widget_inspectorLibrary._kScreenEdgeMargin + Widget_inspectorLibrary._kTooltipPadding)))), 0);
        var textSpan__131932 = ((global::Doroti.Generated.Framework.Painting.TextSpan?)(object?)this._textPainter?.text)!;
        if ((((this._textPainter is null) || (textSpan__131932!.text != message)) || (this._textPainterMaxWidth != maxWidth__131840)))
        {
            _textPainterMaxWidth = maxWidth__131840;
            this._textPainter?.dispose();
            _textPainter = ((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Painting.TextPainter();
            __cascade.maxLines = Widget_inspectorLibrary._kMaxTooltipLines;
            __cascade.ellipsis = "...";
            __cascade.text = new global::Doroti.Generated.Framework.Painting.TextSpan(style: Widget_inspectorLibrary._messageStyle, text: message);
            __cascade.textDirection = textDirection;
            __cascade.layout(maxWidth: maxWidth__131840);
            return __cascade;        }))();
        }
        global::Doroti.Flutter.Ui.Size tooltipSize__132407 = ((global::Doroti.Flutter.Ui.Size)(object?)(this._textPainter!.size + new global::Doroti.Flutter.Ui.Offset((Widget_inspectorLibrary._kTooltipPadding * 2L), (Widget_inspectorLibrary._kTooltipPadding * 2L))));
        global::Doroti.Flutter.Ui.Offset tipOffset__132525 = ((global::Doroti.Flutter.Ui.Offset)(object?)global::Doroti.Generated.Framework.Painting.GeometryLibrary.positionDependentBox(size: size, childSize: tooltipSize__132407, target: target, verticalOffset: verticalOffset, preferBelow: false));
        var tooltipBackground__132711 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.style = PaintingStyle.fill;
            __cascade.color = Widget_inspectorLibrary._kTooltipBackgroundColor;
            return __cascade;        }))();
        canvas.drawRect(global::Doroti.Flutter.Ui.Rect.fromPoints(tipOffset__132525, tipOffset__132525.translate(tooltipSize__132407.width, tooltipSize__132407.height)), tooltipBackground__132711);
        double wedgeY__132975 = tipOffset__132525.dy;
        bool tooltipBelow__133013 = (tipOffset__132525.dy > target.dy);
        if (!tooltipBelow__133013)
        {
            wedgeY__132975 += tooltipSize__132407.height;
        }
        double wedgeSize__133139 = (Widget_inspectorLibrary._kTooltipPadding * 2L);
        double wedgeX__133184 = (Math.Max(tipOffset__132525.dx, target.dx) + (wedgeSize__133139 * 2L));
        wedgeX__133184 = Math.Min(wedgeX__133184, ((tipOffset__132525.dx + tooltipSize__132407.width) - (wedgeSize__133139 * 2L)));
        var wedge__133335 = new List<global::Doroti.Flutter.Ui.Offset> { new global::Doroti.Flutter.Ui.Offset((wedgeX__133184 - wedgeSize__133139), wedgeY__132975), new global::Doroti.Flutter.Ui.Offset((wedgeX__133184 + wedgeSize__133139), wedgeY__132975), new global::Doroti.Flutter.Ui.Offset(wedgeX__133184, (wedgeY__132975 + ((tooltipBelow__133013 ? -wedgeSize__133139 : wedgeSize__133139)))) };
        canvas.drawPath(((Func<Path>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Path();
            __cascade.addPolygon(wedge__133335, true);
            return __cascade;        }))(), tooltipBackground__132711);
        this._textPainter!.paint(canvas, (tipOffset__132525 + new global::Doroti.Flutter.Ui.Offset(Widget_inspectorLibrary._kTooltipPadding, Widget_inspectorLibrary._kTooltipPadding)));
        canvas.restore();
    }

    public override bool findAnnotations<S>(global::Doroti.Generated.Framework.Rendering.AnnotationResult<S> result, Offset localPosition, bool onlyFirst = default!)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isInInspectorRenderObjectTree(global::Doroti.Generated.Framework.Rendering.RenderObject child)
    {
        global::Doroti.Generated.Framework.Rendering.RenderObject? current__134258 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)child).parent;
        while ((current__134258 is not null))
        {
            if (((current__134258 is global::Doroti.Generated.Framework.Rendering.RenderStack) && ((global::Doroti.Generated.Framework.Rendering.RenderStack)current__134258).getChildrenAsList().any(((child) => (child is _RenderInspectorOverlay__widget_inspector)))))
            {
                global::Doroti.Generated.Framework.Rendering.RenderStack current__134258__as134376 = (global::Doroti.Generated.Framework.Rendering.RenderStack)current__134258;
                return (object.Equals(this.rootRenderObject, ((global::Doroti.Generated.Framework.Rendering.RenderStack)current__134258__as134376)));
            }
            current__134258 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)current__134258).parent;
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
    internal static global::Doroti.Generated.Framework.Painting.TextStyle _messageStyle = new global::Doroti.Generated.Framework.Painting.TextStyle(color: new global::Doroti.Flutter.Ui.Color(4294967295L), fontSize: 10.0, height: 1.2);
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

    internal virtual global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool> _selectionOnTapEnabled => WidgetsBinding.instance.debugWidgetInspectorSelectionOnTapEnabled;
    internal virtual Widget? _moveExitWidgetSelectionButton
    {
        get
        {
            MoveExitWidgetSelectionButtonBuilder? buttonBuilder__136339 = ((_WidgetInspectorButtonGroup__widget_inspector)this.widget).moveExitWidgetSelectionButtonBuilder;
            if ((buttonBuilder__136339 is null))
            {
                return ((Widget)(object)null);
            }
            global::Doroti.Flutter.Ui.TextDirection textDirection__136491 = Directionality.of(this.context);
            var buttonLabel__136546 = $"Move to the {((this._usesDefaultAlignment == ((object.Equals(textDirection__136491, TextDirection.ltr)))) ? "right" : "left")}";
            return ((Widget?)(object?)new _WidgetInspectorButton__widget_inspector(button: buttonBuilder__136339(this.context, onPressed: (() => {
_changeButtonGroupAlignment();
_onTooltipHidden();
}), semanticsLabel: buttonLabel__136546, usesDefaultAlignment: this._usesDefaultAlignment), onTooltipVisible: ((global::System.Action)(() => {
_changeTooltipMessage(buttonLabel__136546);
})), onTooltipHidden: () => this._onTooltipHidden()));
            return default!;
        }
    }
    internal virtual Widget _exitWidgetSelectionButton
    {
        get
        {
            var buttonLabel__137141 = "Exit Select Widget mode";
            return ((Widget)(object?)new _WidgetInspectorButton__widget_inspector(button: this.widget.exitWidgetSelectionButtonBuilder(this.context, onPressed: this._exitWidgetSelectionMode, semanticsLabel: buttonLabel__137141, key: this._exitWidgetSelectionButtonKey), onTooltipVisible: ((global::System.Action)(() => {
_changeTooltipMessage(buttonLabel__137141);
})), onTooltipHidden: () => this._onTooltipHidden()));
            return default!;
        }
    }
    internal virtual Widget? _tapBehaviorButton
    {
        get
        {
            TapBehaviorButtonBuilder? buttonBuilder__137630 = ((_WidgetInspectorButtonGroup__widget_inspector)this.widget).tapBehaviorButtonBuilder;
            if ((buttonBuilder__137630 is null))
            {
                return ((Widget)(object)null);
            }
            return ((Widget?)(object?)new _WidgetInspectorButton__widget_inspector(button: buttonBuilder__137630(this.context, onPressed: () => this._changeSelectionOnTapMode(default), semanticsLabel: "Change widget selection mode for taps", selectionOnTapEnabled: ((global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool>)this._selectionOnTapEnabled).value), onTooltipVisible: () => this._changeSelectionOnTapTooltip(), onTooltipHidden: () => this._onTooltipHidden()));
            return default!;
        }
    }
    internal virtual bool _tooltipVisible => DartRuntimePrimitives.ConvertValue<bool>((this._tooltipMessage is not null));
    public override Widget build(BuildContext context)
    {
        double bottomPadding__138231 = Math.Max(_kExitWidgetSelectionButtonMargin, MediaQuery.viewPaddingOf(context).bottom);
        Widget selectionModeButtons__138371 = ((Widget)(object?)new Column(children: new List<Widget> { this._exitWidgetSelectionButton }));
        Widget buttonGroup__138502 = ((Widget)(object?)new Stack(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topCenter, children: new List<Widget> { new CustomPaint(painter: new _ExitWidgetSelectionTooltipPainter__widget_inspector(tooltipMessage: this._tooltipMessage, buttonKey: this._exitWidgetSelectionButtonKey, usesDefaultAlignment: this._usesDefaultAlignment)), new Row(crossAxisAlignment: global::Doroti.Generated.Framework.Rendering.CrossAxisAlignment.end, mainAxisAlignment: global::Doroti.Generated.Framework.Rendering.MainAxisAlignment.center, children: new List<Widget>()) }));
        return ((Widget)(object?)Positioned.CreateDirectional(textDirection: Directionality.of(context), start: (this._usesDefaultAlignment ? _kExitWidgetSelectionButtonMargin : null), end: (this._usesDefaultAlignment ? null : _kExitWidgetSelectionButtonMargin), bottom: bottomPadding__138231, child: buttonGroup__138502));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _exitWidgetSelectionMode()
    {
        WidgetInspectorService.instance._changeWidgetSelectionMode(false);
        _changeSelectionOnTapMode(selectionOnTapEnabled: _defaultSelectionOnTapEnabled);
    }

    internal virtual void _changeSelectionOnTapMode(bool? selectionOnTapEnabled = null)
    {
        bool newValue__139859 = (selectionOnTapEnabled ?? !((global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool>)this._selectionOnTapEnabled).value);
        this._selectionOnTapEnabled.value = newValue__139859;
        WidgetInspectorService.instance.selection.clear();
        if (this._tooltipVisible)
        {
            _changeSelectionOnTapTooltip();
        }
    }

    internal virtual void _changeSelectionOnTapTooltip()
    {
        _changeTooltipMessage((((global::Doroti.Generated.Framework.Foundation.ValueNotifier<bool>)this._selectionOnTapEnabled).value ? "Disable widget selection for taps" : "Enable widget selection for taps"));
    }

    internal virtual void _changeButtonGroupAlignment()
    {
        if (this.mounted)
        {
            setState(((global::System.Action)(() => {
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
            setState(((global::System.Action)(() => {
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
        return ((Widget)(object?)new Stack(alignment: global::Doroti.Generated.Framework.Painting.AlignmentDirectional.topCenter, children: new List<Widget> { new GestureDetector(onLongPress: ((global::System.Action)(() => {
_tooltipVisibleAfter(_WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
_tooltipHiddenAfter((_WidgetInspectorButton__widget_inspector._tooltipShownOnLongPressDuration + _WidgetInspectorButton__widget_inspector._tooltipDelayDuration));
})), child: new MouseRegion(onEnter: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerEnterEvent>)((_) => {
_tooltipVisibleAfter(_WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
})), onExit: ((global::System.Action<global::Doroti.Generated.Framework.Gestures.PointerExitEvent>)((_) => {
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
        Timer? timer__142733 = (isVisible ? this._tooltipVisibleTimer : this._tooltipHiddenTimer);
        if ((timer__142733?.isActive ?? false))
        {
            timer__142733!.cancel();
        }
        if (isVisible)
        {
            _tooltipVisibleTimer = new Timer(duration, (() => {
this.widget.onTooltipVisible();
}));
        }
        else
        {
            _tooltipHiddenTimer = new Timer(duration, (() => {
this.widget.onTooltipHidden();
}));
        }
    }

}

internal class _ExitWidgetSelectionTooltipPainter__widget_inspector : global::Doroti.Generated.Framework.Rendering.CustomPainter
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
        var isVisible__143531 = (this.tooltipMessage is not null);
        if (!isVisible__143531)
        {
            return;
        }
        global::Doroti.Generated.Framework.Rendering.RenderObject? buttonRenderObject__143714 = ((global::Doroti.Generated.Framework.Rendering.RenderObject?)(object?)((GlobalKey<IState>)this.buttonKey).currentContext?.findRenderObject());
        if ((buttonRenderObject__143714 is null))
        {
            return;
        }
        var tooltipPadding__143884 = 4.0;
        var tooltipSpacing__143916 = 6.0;
        var tooltipTextPainter__143949 = ((Func<global::Doroti.Generated.Framework.Painting.TextPainter>)(() =>
{            var __cascade = new global::Doroti.Generated.Framework.Painting.TextPainter();
            __cascade.maxLines = 1L;
            __cascade.ellipsis = "...";
            __cascade.text = new global::Doroti.Generated.Framework.Painting.TextSpan(text: this.tooltipMessage, style: Widget_inspectorLibrary._messageStyle);
            __cascade.textDirection = TextDirection.ltr;
            __cascade.layout();
            return __cascade;        }))();
        var tooltipPaint__144169 = ((Func<Paint>)(() =>
{            var __cascade = new global::Doroti.Flutter.Ui.Paint();
            __cascade.style = PaintingStyle.fill;
            __cascade.color = Widget_inspectorLibrary._kTooltipBackgroundColor;
            return __cascade;        }))();
        double buttonWidth__144322 = ((global::Doroti.Generated.Framework.Rendering.RenderObject)buttonRenderObject__143714).paintBounds.width;
        global::Doroti.Flutter.Ui.Size textSize__144389 = ((global::Doroti.Flutter.Ui.Size)(object?)((global::Doroti.Generated.Framework.Painting.TextPainter)tooltipTextPainter__143949).size);
        double textWidth__144442 = textSize__144389.width;
        double textHeight__144487 = textSize__144389.height;
        double tooltipWidth__144534 = (textWidth__144442 + ((tooltipPadding__143884 * 2L)));
        double tooltipHeight__144600 = (textHeight__144487 + ((tooltipPadding__143884 * 2L)));
        double tooltipXOffset__144669 = (this.usesDefaultAlignment ? (0L - buttonWidth__144322) : (0L - ((tooltipWidth__144534 - buttonWidth__144322))));
        double tooltipYOffset__144794 = ((0L - tooltipHeight__144600) - tooltipSpacing__143916);
        canvas.drawRect(global::Doroti.Flutter.Ui.Rect.fromLTWH(tooltipXOffset__144669, tooltipYOffset__144794, tooltipWidth__144534, tooltipHeight__144600), tooltipPaint__144169);
        tooltipTextPainter__143949.paint(canvas, new global::Doroti.Flutter.Ui.Offset((tooltipXOffset__144669 + tooltipPadding__143884), (tooltipYOffset__144794 + tooltipPadding__143884)));
    }

    public override bool shouldRepaint(global::Doroti.Generated.Framework.Rendering.CustomPainter oldDelegate)
    {
        var __oldDelegate = (_ExitWidgetSelectionTooltipPainter__widget_inspector)(object)oldDelegate;
        return (this.tooltipMessage != ((_ExitWidgetSelectionTooltipPainter__widget_inspector)__oldDelegate).tooltipMessage);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static bool _isDebugCreator(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node) => (node is global::Doroti.Generated.Framework.Rendering.DiagnosticsDebugCreator);
}

public static partial class Widget_inspectorLibrary
{
    public static IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> debugTransformDebugCreator(IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> properties)
    {
        if (!global::Doroti.Generated.Framework.Foundation.ConstantsLibrary.kDebugMode)
        {
            return ((IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
        }
        var pending__145873 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        global::Doroti.Generated.Framework.Foundation.ErrorSummary? errorSummary__145920 = default!;
        foreach (var node__145947 in properties)
        {
            if ((node__145947 is global::Doroti.Generated.Framework.Foundation.ErrorSummary))
            {
                global::Doroti.Generated.Framework.Foundation.ErrorSummary node__145947__as145977 = (global::Doroti.Generated.Framework.Foundation.ErrorSummary)node__145947;
                errorSummary__145920 = ((global::Doroti.Generated.Framework.Foundation.ErrorSummary)node__145947__as145977);
                break;
            }
        }
        var foundStackTrace__146057 = false;
        var result__146090 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        foreach (var node__146133 in properties)
        {
            if ((!foundStackTrace__146057 && (node__146133 is global::Doroti.Generated.Framework.Foundation.DiagnosticsStackTrace)))
            {
                global::Doroti.Generated.Framework.Foundation.DiagnosticsStackTrace node__146133__as146183 = (global::Doroti.Generated.Framework.Foundation.DiagnosticsStackTrace)node__146133;
                foundStackTrace__146057 = true;
            }
            if (Widget_inspectorLibrary._isDebugCreator(node__146133))
            {
                result__146090.AddRange(Widget_inspectorLibrary._parseDiagnosticsNode(node__146133, errorSummary__145920).Cast<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
            }
            else
            {
                if (foundStackTrace__146057)
                {
                    pending__145873.Add(node__146133);
                }
                else
                {
                    result__146090.Add(node__146133);
                }
            }
        }
        result__146090.AddRange(pending__145873.Cast<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
        return ((IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)result__146090);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _parseDiagnosticsNode(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node, global::Doroti.Generated.Framework.Foundation.ErrorSummary? errorSummary)
    {
        DartRuntimePrimitives.Assert(() => Widget_inspectorLibrary._isDebugCreator(node));
        try
        {
            var debugCreator__146783 = ((DebugCreator?)(object?)((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).value!)!;
            Element element__146845 = ((DebugCreator)debugCreator__146783).element;
            return Widget_inspectorLibrary._describeRelevantUserCode(element__146845, errorSummary);
        }
        catch (Exception error__146949)
        {
            var stack__146956 = new System.Diagnostics.StackTrace();
            DartAsyncRuntime.scheduleMicrotask((() => {
FlutterError.reportError(new global::Doroti.Generated.Framework.Foundation.FlutterErrorDetails(exception: error__146949, stack: stack__146956, library: "widget inspector", informationCollector: ((InformationCollector)(() => new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { global::Doroti.Generated.Framework.Foundation.DiagnosticsNode.CreateMessage("This exception was caught while trying to describe the user-relevant code of another error.") }))));
}));
            return ((IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _describeRelevantUserCode(Element element, global::Doroti.Generated.Framework.Foundation.ErrorSummary? errorSummary)
    {
        if (!WidgetInspectorService.instance.isWidgetCreationTracked())
        {
            return ((IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorDescription("Widget creation tracking is currently disabled. Enabling " + "it enables improved error messages. It can be enabled by passing " + "`--track-widget-creation` to `flutter run` or `flutter test`."), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer() });
        }
        bool isOverflowError()
        {
            if (((errorSummary is not null) && !string.IsNullOrEmpty(errorSummary.value?.ToString())))
            {
                object summary__148033 = errorSummary.value;
                if (((summary__148033 is string) && ((string)summary__148033).startsWith("A RenderFlex overflowed by")))
                {
                    string summary__148033__as148079 = (string)summary__148033;
                    return true;
                }
            }
            return false;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var nodes__148218 = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();
        bool processElement(Element target)
        {
            if (Widget_inspectorLibrary.debugIsLocalCreationLocation(target))
            {
                global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? devToolsDiagnostic__148465 = default!;
                if (isOverflowError())
                {
                    string? devToolsInspectorUri__148769 = ((string?)(object?)WidgetInspectorService.instance._devToolsInspectorUriForElement(target));
                    if ((devToolsInspectorUri__148769 is not null))
                    {
                        devToolsDiagnostic__148465 = DartRuntimePrimitives.ConvertValue<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(new DevToolsDeepLinkProperty($"To inspect this widget in Flutter DevTools, visit: {devToolsInspectorUri__148769}", devToolsInspectorUri__148769));
                    }
                }
                nodes__148218.AddRange(new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.DiagnosticsBlock(name: "The relevant error-causing widget was", children: new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> { new global::Doroti.Generated.Framework.Foundation.ErrorDescription($"{((Diagnosticable)((Element)target).widget).toStringShort()} {Widget_inspectorLibrary._describeCreationLocation(target)}") }), new global::Doroti.Generated.Framework.Foundation.ErrorSpacer() }.Cast<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>());
                return false;
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if (processElement(element))
        {
            element.visitAncestorElements((global::System.Func<Element, bool>)processElement);
        }
        return ((IEnumerable<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)nodes__148218);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DevToolsDeepLinkProperty : global::Doroti.Generated.Framework.Foundation.DiagnosticsProperty<string>
{
    public DevToolsDeepLinkProperty(string description, string url) : base("", url, description: description, level: global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.info)
    {
    }

}

public static partial class Widget_inspectorLibrary
{
    public static bool debugIsLocalCreationLocation(object @object)
    {
        var isLocal__150761 = false;
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Flutter.Runtime.CreationLocation? location__150830 = ((global::Doroti.Flutter.Runtime.CreationLocation?)(object?)Widget_inspectorLibrary._getCreationLocation(@object));
                if ((location__150830 is not null))
                {
                    isLocal__150761 = WidgetInspectorService.instance._isLocalCreationLocation(((string)(object)((object)((dynamic)location__150830).file)));
                }
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        return isLocal__150761;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    public static bool debugIsWidgetLocalCreation(Widget widget)
    {
        global::Doroti.Flutter.Runtime.CreationLocation? location__151319 = ((global::Doroti.Flutter.Runtime.CreationLocation?)(object?)global::Doroti.Flutter.Runtime.CreationLocation.of(widget));
        return ((location__151319 is not null) && WidgetInspectorService.instance._isLocalCreationLocation(((string)(object)((object)((dynamic)location__151319).file))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static string? _describeCreationLocation(object @object)
    {
        global::Doroti.Flutter.Runtime.CreationLocation? location__151859 = ((global::Doroti.Flutter.Runtime.CreationLocation?)(object?)Widget_inspectorLibrary._getCreationLocation(@object));
        return ((string?)((dynamic)location__151859)?.ToString());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static global::Doroti.Flutter.Runtime.CreationLocation? _getCreationLocation(object? @object)
    {
        object? candidate__152248 = (((@object is Element) && !((Element)((Element)@object)).debugIsDefunct) ? ((Element)((Element)@object)).widget : @object);
        return ((candidate__152248 is null) ? null : global::Doroti.Flutter.Runtime.CreationLocation.of(candidate__152248));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static DartMap<object, long> _locationToId = new DartMap<object, long>();
}

public static partial class Widget_inspectorLibrary
{
    internal static List<object> _locations = new List<global::Doroti.Flutter.Runtime.CreationLocation>().Cast<object>().ToList();
}

public static partial class Widget_inspectorLibrary
{
    internal static long _toLocationId(object location)
    {
        long? id__152830 = DartCollectionRuntime.NullableMapValue<long>(Widget_inspectorLibrary._locationToId, location);
        if ((id__152830 is not null))
        {
            long id__152830__value152866 = DartRuntimePrimitives.RequireValue(id__152830);
            return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id__152830__value152866));
        }
        id__152830 = checked((long)(Widget_inspectorLibrary._locations.Count));
        Widget_inspectorLibrary._locations.Add(location);
        Widget_inspectorLibrary._locationToId[location] = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id__152830));
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id__152830));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static DartMap<string, object> _locationIdMapToJson()
    {
        var idsKey__153055 = "ids";
        var linesKey__153079 = "lines";
        var columnsKey__153107 = "columns";
        var namesKey__153139 = "names";
        var fileLocationsMap__153168 = new DartMap<string, DartMap<string, List<object>>>();
        foreach (var entry__153282 in Widget_inspectorLibrary._locationToId.entries)
        {
            global::Doroti.Flutter.Runtime.CreationLocation location__153353 = ((global::Doroti.Flutter.Runtime.CreationLocation)(object?)entry__153282.key);
            DartMap<string, List<object?>> locations__153412 = fileLocationsMap__153168.putIfAbsent(((string)(object)((object)((dynamic)location__153353).file)), (() => new DartMap<string, List<object>> { [idsKey__153055] = new List<long>().Cast<object>().ToList(), [linesKey__153079] = new List<long>().Cast<object>().ToList(), [columnsKey__153107] = new List<long>().Cast<object>().ToList(), [namesKey__153139] = new List<string?>().Cast<object>().ToList() })).cast<string, List<object?>>();
            locations__153412.GetValueOrDefault(idsKey__153055)!.Add(entry__153282.value);
            locations__153412.GetValueOrDefault(linesKey__153079)!.Add(((object)((dynamic)location__153353).line));
            locations__153412.GetValueOrDefault(columnsKey__153107)!.Add(((object)((dynamic)location__153353).column));
            locations__153412.GetValueOrDefault(namesKey__153139)!.Add(location__153353.ToString());
        }
        return ((DartMap<string, object>)(object?)fileLocationsMap__153168);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class InspectorSerializationDelegate : global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate
{
    public virtual WidgetInspectorService service { get; private set; } = default!;
    public virtual string? groupName { get; private set; }
    public virtual bool summaryTree { get; private set; } = default!;
    public virtual long maxDescendantsTruncatableNode { get; private set; } = default!;
    public virtual bool includeProperties { get; private set; } = default!;
    public virtual long subtreeDepth { get; private set; } = default!;
    public virtual bool expandPropertyValues { get; private set; } = default!;
    public virtual bool inDisableWidgetInspectorScope { get; private set; } = default!;
    public virtual global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback { get; private set; }
    internal virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> _nodesCreatedByLocalProject { get; private set; } = new List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>();

    public InspectorSerializationDelegate(string? groupName = null, bool summaryTree = false, long maxDescendantsTruncatableNode = -1, bool expandPropertyValues = true, long subtreeDepth = 1, bool includeProperties = false, WidgetInspectorService service = default!, global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>? addAdditionalPropertiesCallback = null, bool inDisableWidgetInspectorScope = false)
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
    public virtual DartMap<string, object> additionalNodeProperties(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node, bool fullDetails = true)
    {
        var result__156129 = new DartMap<string, object>();
        object? value__156177 = ((global::Doroti.Generated.Framework.Foundation.DiagnosticsNode)node).value;
        if ((this.summaryTree && fullDetails))
        {
            result__156129["summaryTree"] = true;
        }
        if (this._interactive)
        {
            result__156129["valueId"] = this.service.toId(value__156177, this.groupName!);
        }
        global::Doroti.Flutter.Runtime.CreationLocation? creationLocation__156404 = ((global::Doroti.Flutter.Runtime.CreationLocation?)(object?)Widget_inspectorLibrary._getCreationLocation(value__156177));
        if ((creationLocation__156404 is not null))
        {
            if (fullDetails)
            {
                result__156129["locationId"] = Widget_inspectorLibrary._toLocationId(creationLocation__156404);
                result__156129["creationLocation"] = ((object)((dynamic)creationLocation__156404).toJsonMap());
            }
            if (this.service._isLocalCreationLocation(((string)(object)((object)((dynamic)creationLocation__156404).file))))
            {
                this._nodesCreatedByLocalProject.Add(node);
                result__156129["createdByLocalProject"] = true;
            }
        }
        if ((this.addAdditionalPropertiesCallback is not null))
        {
            result__156129.AddRange((this.addAdditionalPropertiesCallback!(node, this) ?? new DartMap<string, object>()));
        }
        return result__156129;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate delegateForNode(global::Doroti.Generated.Framework.Foundation.DiagnosticsNode node)
    {
        return ((global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate)(object?)(((this.summaryTree || (this.subtreeDepth > 1L)) || this.service._shouldShowInSummaryTree(node)) ? copyWith(subtreeDepth: (this.subtreeDepth - 1L)) : this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> filterChildren(List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode owner)
    {
        return ((List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>)(object?)this.service._filterChildren(nodes, this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> filterProperties(List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode owner)
    {
        bool createdByLocalProject__157887 = this._nodesCreatedByLocalProject.Contains(owner);
        return nodes.where(((node) => {
return !node.isFiltered((createdByLocalProject__157887 ? global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.fine : global::Doroti.Generated.Framework.Foundation.DiagnosticLevel.info));
throw new InvalidOperationException("Dart closure completed without a value.");
})).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> truncateNodesList(List<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode> nodes, global::Doroti.Generated.Framework.Foundation.DiagnosticsNode? owner)
    {
        if ((((this.maxDescendantsTruncatableNode >= 0L) && owner!.allowTruncate) && (checked((long)(nodes.Count)) > this.maxDescendantsTruncatableNode)))
        {
            nodes = this.service._truncateNodes(nodes.Cast<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode>(), this.maxDescendantsTruncatableNode);
        }
        return nodes;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate copyWith(long? subtreeDepth = null, bool? includeProperties = null, bool? expandPropertyValues = null, bool? inDisableWidgetInspectorScope = null)
    {
        return ((global::Doroti.Generated.Framework.Foundation.DiagnosticsSerializationDelegate)(object?)new InspectorSerializationDelegate(groupName: this.groupName, summaryTree: this.summaryTree, maxDescendantsTruncatableNode: this.maxDescendantsTruncatableNode, expandPropertyValues: (expandPropertyValues ?? this.expandPropertyValues), subtreeDepth: (subtreeDepth ?? this.subtreeDepth), includeProperties: (includeProperties ?? this.includeProperties), service: this.service, addAdditionalPropertiesCallback: (global::System.Func<global::Doroti.Generated.Framework.Foundation.DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object>?>?)this.addAdditionalPropertiesCallback, inDisableWidgetInspectorScope: (inDisableWidgetInspectorScope ?? this.inDisableWidgetInspectorScope)));
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
            var result__162375 = ((V?)(object?)this._objects[key!])!;
            this._objects[key] = null;
            return result__162375;
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
            public static global::Doroti.Flutter.Runtime.CreationLocation? of(object? value) => global::Doroti.Flutter.Runtime.CreationLocation.of(value);
        }
    }
}
