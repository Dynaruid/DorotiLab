// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: ../../../reference/flutter-master/packages/flutter/lib/src/widgets/widget_inspector.dart
using Doroti.Runtime;
using Doroti.Ui;

namespace Doroti.Framework.Widgets;

public delegate Widget ExitWidgetSelectionButtonBuilder(BuildContext context, GlobalKey<IState> key, Action onPressed, string semanticsLabel);

public delegate Widget MoveExitWidgetSelectionButtonBuilder(BuildContext context, Action onPressed, string semanticsLabel, bool usesDefaultAlignment = default!);

public delegate Widget TapBehaviorButtonBuilder(BuildContext context, Action onPressed, bool selectionOnTapEnabled, string semanticsLabel);

public delegate void RegisterServiceExtensionCallback(Func<DartMap<string, string>, Future<DartMap<string, object?>>> callback, string name);

internal class _ProxyLayer__widget_inspector : Layer
{
    internal virtual Layer _layer { get; private set; } = default!;

    internal _ProxyLayer__widget_inspector(Layer _layer)
    {
        this._layer = _layer;
    }

    public override void addToScene(SceneBuilder builder)
    {
        _layer.addToScene(builder);
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst)
    {
        return _layer.findAnnotations(result, localPosition, onlyFirst: onlyFirst);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

internal class _MulticastCanvas__widget_inspector : Canvas
{
    internal virtual Canvas _main { get; private set; } = default!;
    internal virtual Canvas _screenshot { get; private set; } = default!;

    internal _MulticastCanvas__widget_inspector(Canvas main, Canvas screenshot)
    {
        _main = main;
        _screenshot = screenshot;
    }

    public new virtual void clipPath(Path path, bool doAntiAlias = true)
    {
        _main.clipPath(path, doAntiAlias: doAntiAlias);
        _screenshot.clipPath(path, doAntiAlias: doAntiAlias);
    }

    public new virtual void clipRRect(RRect rrect, bool doAntiAlias = true)
    {
        _main.clipRRect(rrect, doAntiAlias: doAntiAlias);
        _screenshot.clipRRect(rrect, doAntiAlias: doAntiAlias);
    }

    public new virtual void clipRect(Rect rect, ClipOp clipOp = default!, bool doAntiAlias = true)
    {
        _main.clipRect(rect, clipOp: clipOp, doAntiAlias: doAntiAlias);
        _screenshot.clipRect(rect, clipOp: clipOp, doAntiAlias: doAntiAlias);
    }

    public new virtual void drawArc(Rect rect, double startAngle, double sweepAngle, bool useCenter, Paint paint)
    {
        _main.drawArc(rect, startAngle, sweepAngle, useCenter, paint);
        _screenshot.drawArc(rect, startAngle, sweepAngle, useCenter, paint);
    }

    public virtual void drawAtlas(Ui.Image atlas, List<RSTransform> transforms, List<Rect> rects, List<Color>? colors, BlendMode? blendMode, Rect? cullRect, Paint paint)
    {
        _main.drawAtlas(atlas, transforms, rects, colors, blendMode, cullRect, paint);
        _screenshot.drawAtlas(atlas, transforms, rects, colors, blendMode, cullRect, paint);
    }

    public new virtual void drawCircle(Offset c, double radius, Paint paint)
    {
        _main.drawCircle(c, radius, paint);
        _screenshot.drawCircle(c, radius, paint);
    }

    public new virtual void drawColor(Color color, BlendMode blendMode)
    {
        _main.drawColor(color, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(blendMode)));
        _screenshot.drawColor(color, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(blendMode)));
    }

    public new virtual void drawDRRect(RRect outer, RRect inner, Paint paint)
    {
        _main.drawDRRect(outer, inner, paint);
        _screenshot.drawDRRect(outer, inner, paint);
    }

    public new virtual void drawImage(Ui.Image image, Offset p, Paint paint)
    {
        _main.drawImage(image, p, paint);
        _screenshot.drawImage(image, p, paint);
    }

    public new virtual void drawImageNine(Ui.Image image, Rect center, Rect dst, Paint paint)
    {
        _main.drawImageNine(image, center, dst, paint);
        _screenshot.drawImageNine(image, center, dst, paint);
    }

    public new virtual void drawImageRect(Ui.Image image, Rect src, Rect dst, Paint paint)
    {
        _main.drawImageRect(image, src, dst, paint);
        _screenshot.drawImageRect(image, src, dst, paint);
    }

    public new virtual void drawLine(Offset p1, Offset p2, Paint paint)
    {
        _main.drawLine(p1, p2, paint);
        _screenshot.drawLine(p1, p2, paint);
    }

    public new virtual void drawOval(Rect rect, Paint paint)
    {
        _main.drawOval(rect, paint);
        _screenshot.drawOval(rect, paint);
    }

    public new virtual void drawPaint(Paint paint)
    {
        _main.drawPaint(paint);
        _screenshot.drawPaint(paint);
    }

    public new virtual void drawParagraph(Paragraph paragraph, Offset offset)
    {
        _main.drawParagraph(paragraph, offset);
        _screenshot.drawParagraph(paragraph, offset);
    }

    public new virtual void drawPath(Path path, Paint paint)
    {
        _main.drawPath(path, paint);
        _screenshot.drawPath(path, paint);
    }

    public new virtual void drawPicture(Picture picture)
    {
        _main.drawPicture(picture);
        _screenshot.drawPicture(picture);
    }

    public virtual void drawPoints(PointMode pointMode, List<Offset> points, Paint paint)
    {
        _main.drawPoints(pointMode, points, paint);
        _screenshot.drawPoints(pointMode, points, paint);
    }

    public new virtual void drawRRect(RRect rrect, Paint paint)
    {
        _main.drawRRect(rrect, paint);
        _screenshot.drawRRect(rrect, paint);
    }

    public new virtual void drawRawAtlas(Ui.Image atlas, Float32List rstTransforms, Float32List rects, Int32List? colors, BlendMode? blendMode, Rect? cullRect, Paint paint)
    {
        _main.drawRawAtlas(atlas, rstTransforms, rects, colors, blendMode, cullRect, paint);
        _screenshot.drawRawAtlas(atlas, rstTransforms, rects, colors, blendMode, cullRect, paint);
    }

    public new virtual void drawRawPoints(PointMode pointMode, Float32List points, Paint paint)
    {
        _main.drawRawPoints(pointMode, points, paint);
        _screenshot.drawRawPoints(pointMode, points, paint);
    }

    public new virtual void drawRect(Rect rect, Paint paint)
    {
        _main.drawRect(rect, paint);
        _screenshot.drawRect(rect, paint);
    }

    public new virtual void drawShadow(Path path, Color color, double elevation, bool transparentOccluder)
    {
        _main.drawShadow(path, color, elevation, transparentOccluder);
        _screenshot.drawShadow(path, color, elevation, transparentOccluder);
    }

    public new virtual void drawVertices(Vertices vertices, BlendMode blendMode, Paint paint)
    {
        _main.drawVertices(vertices, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(blendMode)), paint);
        _screenshot.drawVertices(vertices, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(blendMode)), paint);
    }

    public new virtual long getSaveCount()
    {
        return _main.getSaveCount();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public new virtual void restore()
    {
        _main.restore();
        _screenshot.restore();
    }

    public new virtual void rotate(double radians)
    {
        _main.rotate(radians);
        _screenshot.rotate(radians);
    }

    public new virtual void save()
    {
        _main.save();
        _screenshot.save();
    }

    public new virtual void saveLayer(Rect? bounds, Paint paint)
    {
        _main.saveLayer(bounds, paint);
        _screenshot.saveLayer(bounds, paint);
    }

    public new virtual void scale(double sx, double? sy = null)
    {
        _main.scale(sx, sy);
        _screenshot.scale(sx, sy);
    }

    public new virtual void skew(double sx, double sy)
    {
        _main.skew(sx, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(sy)));
        _screenshot.skew(sx, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(sy)));
    }

    public virtual void transform(Float64List matrix4)
    {
        _main.transform(matrix4);
        _screenshot.transform(matrix4);
    }

    public new virtual void translate(double dx, double dy)
    {
        _main.translate(dx, dy);
        _screenshot.translate(dx, dy);
    }

    public override dynamic noSuchMethod(Invocation invocation)
    {
        base.noSuchMethod(invocation);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static Rect _calculateSubtreeBoundsHelper(RenderObject @object, Matrix4 transform)
    {
        Rect bounds = MatrixUtils.transformRect(transform, @object.semanticBounds);
        @object.visitChildren((child) =>
        {
            Matrix4 childTransform = transform.clone();
            @object.applyPaintTransform(child, childTransform);
            Rect childBounds = _calculateSubtreeBoundsHelper(child, childTransform);
            Rect? paintClip = @object.describeApproximatePaintClip(child);
            if (paintClip is not null)
            {
                Rect paintClip__9652__value9716 = DartRuntimePrimitives.RequireValue(paintClip);
                Rect transformedPaintClip = MatrixUtils.transformRect(transform, DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(paintClip__9652__value9716)));
                childBounds = childBounds.intersect(transformedPaintClip);
            }
            if (childBounds.isFinite && !childBounds.isEmpty)
            {
                bounds = bounds.isEmpty ? childBounds : bounds.expandToInclude(childBounds);
            }
        });
        return bounds;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static Rect _calculateSubtreeBounds(RenderObject @object)
    {
        return _calculateSubtreeBoundsHelper(@object, Matrix4.identity());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _ScreenshotContainerLayer__widget_inspector : OffsetLayer
{
    public override void addToScene(SceneBuilder builder)
    {
        addChildrenToScene(builder);
    }

}

public class _ScreenshotData__widget_inspector
{
    public virtual RenderObject target { get; private set; } = default!;
    public virtual OffsetLayer containerLayer { get; private set; } = default!;
    public virtual bool foundTarget { get; set; } = false;
    public virtual bool includeInScreenshot { get; set; } = false;
    public virtual bool includeInRegularContext { get; set; } = true;

    internal _ScreenshotData__widget_inspector(RenderObject target)
    {
        this.target = target;
        containerLayer = new _ScreenshotContainerLayer__widget_inspector();
    }

    public virtual Offset screenshotOffset
    {
        get
        {
            DartRuntimePrimitives.Assert(() => foundTarget);
            return containerLayer.offset;
        }
        set
        {
            var offset = value;
            containerLayer.offset = offset;
        }
    }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        containerLayer.dispose();
    }

}

internal class _ScreenshotPaintingContext__widget_inspector : PaintingContext
{
    internal virtual _ScreenshotData__widget_inspector _data { get; private set; } = default!;
    internal virtual PictureLayer? _screenshotCurrentLayer { get; set; } = default;
    internal virtual PictureRecorder? _screenshotRecorder { get; set; } = default;
    internal virtual Canvas? _screenshotCanvas { get; set; } = default;
    internal virtual _MulticastCanvas__widget_inspector? _multicastCanvas { get; set; } = default;

    internal _ScreenshotPaintingContext__widget_inspector(ContainerLayer containerLayer, Rect estimatedBounds, _ScreenshotData__widget_inspector screenshotData) : base(containerLayer, estimatedBounds)
    {
        _data = screenshotData;
    }

    public override Canvas canvas
    {
        get
        {
            if (_data.includeInScreenshot)
            {
                if (_screenshotCanvas is null)
                {
                    _startRecordingScreenshot();
                }
                DartRuntimePrimitives.Assert(() => _screenshotCanvas is not null);
                return _data.includeInRegularContext ? _multicastCanvas! : _screenshotCanvas!;
            }
            else
            {
                DartRuntimePrimitives.Assert(() => _data.includeInRegularContext);
                return base.canvas;
            }
        }
    }
    internal virtual bool _isScreenshotRecording
    {
        get
        {
            var hasScreenshotCanvas = _screenshotCanvas is not null;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (hasScreenshotCanvas)
                    {
                        DartRuntimePrimitives.Assert(() => _screenshotCurrentLayer is not null);
                        DartRuntimePrimitives.Assert(() => _screenshotRecorder is not null);
                        DartRuntimePrimitives.Assert(() => _screenshotCanvas is not null);
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => _screenshotCurrentLayer is null);
                        DartRuntimePrimitives.Assert(() => _screenshotRecorder is null);
                        DartRuntimePrimitives.Assert(() => _screenshotCanvas is null);
                    }
                    return true;
                    throw new InvalidOperationException("Dart closure completed without a value.");
                });
            return hasScreenshotCanvas;
        }
    }
    internal virtual void _startRecordingScreenshot()
    {
        DartRuntimePrimitives.Assert(() => _data.includeInScreenshot);
        DartRuntimePrimitives.Assert(() => !_isScreenshotRecording);
        _screenshotCurrentLayer = new PictureLayer(estimatedBounds);
        _screenshotRecorder = new PictureRecorder();
        _screenshotCanvas = new Canvas(_screenshotRecorder!);
        _data.containerLayer.append(_screenshotCurrentLayer!);
        if (_data.includeInRegularContext)
        {
            _multicastCanvas = new _MulticastCanvas__widget_inspector(main: base.canvas, screenshot: _screenshotCanvas!);
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
        if (!_isScreenshotRecording)
        {
            return;
        }
        _screenshotCurrentLayer!.picture = _screenshotRecorder!.endRecording();
        _screenshotCurrentLayer = null;
        _screenshotRecorder = null;
        _multicastCanvas = null;
        _screenshotCanvas = null;
    }

    public override void appendLayer(Layer layer)
    {
        if (_data.includeInRegularContext)
        {
            base.appendLayer(layer);
            if (_data.includeInScreenshot)
            {
                DartRuntimePrimitives.Assert(() => !_isScreenshotRecording);
                _data.containerLayer.append(new _ProxyLayer__widget_inspector(layer));
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !_isScreenshotRecording);
            DartRuntimePrimitives.Assert(() => _data.includeInScreenshot);
            layer.remove();
            _data.containerLayer.append(layer);
            return;
        }
    }

    public override PaintingContext createChildContext(ContainerLayer childLayer, Rect bounds)
    {
        if (_data.foundTarget)
        {
            return base.createChildContext(childLayer, bounds);
        }
        else
        {
            return new _ScreenshotPaintingContext__widget_inspector(containerLayer: childLayer, estimatedBounds: bounds, screenshotData: _data);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paintChild(RenderObject child, Offset offset)
    {
        bool isScreenshotTarget = DartRuntimePrimitives.Identical(child, _data.target);
        if (isScreenshotTarget)
        {
            DartRuntimePrimitives.Assert(() => !_data.includeInScreenshot);
            DartRuntimePrimitives.Assert(() => !_data.foundTarget);
            _data.foundTarget = true;
            _data.screenshotOffset = offset;
            _data.includeInScreenshot = true;
        }
        base.paintChild(child, offset);
        if (isScreenshotTarget)
        {
            _stopRecordingScreenshotIfNeeded();
            _data.includeInScreenshot = false;
        }
    }

    public static async Future<Ui.Image> toImage(RenderObject renderObject, Rect renderBounds, double pixelRatio = 1.0, bool debugPaint = false)
    {
        var repaintBoundary = renderObject;
        while (!repaintBoundary.isRepaintBoundary)
        {
            repaintBoundary = repaintBoundary.parent!;
        }
        var data = new _ScreenshotData__widget_inspector(target: renderObject);
        var context = new _ScreenshotPaintingContext__widget_inspector(containerLayer: repaintBoundary.debugLayer!, estimatedBounds: repaintBoundary.paintBounds, screenshotData: data);
        if (DartRuntimePrimitives.Identical(renderObject, repaintBoundary))
        {
            data.containerLayer.append(new _ProxyLayer__widget_inspector(repaintBoundary.debugLayer!));
            data.foundTarget = true;
            var offsetLayer = ((OffsetLayer?)repaintBoundary.debugLayer!)!;
            data.screenshotOffset = offsetLayer.offset;
        }
        else
        {
            debugInstrumentRepaintCompositedChild(repaintBoundary, customContext: context);
        }
        if (debugPaint && !Rendering.DebugLibrary.debugPaintSizeEnabled)
        {
            data.includeInRegularContext = false;
            context.stopRecordingIfNeeded();
            DartRuntimePrimitives.Assert(() => data.foundTarget);
            data.includeInScreenshot = true;
            Rendering.DebugLibrary.debugPaintSizeEnabled = true;
            try
            {
                renderObject.debugPaint(context, data.screenshotOffset);
            }
            finally
            {
                Rendering.DebugLibrary.debugPaintSizeEnabled = false;
                context.stopRecordingIfNeeded();
            }
        }
        repaintBoundary.debugLayer!.buildScene(new SceneBuilder());
        Ui.Image image = default!;
        try
        {
            image = await data.containerLayer.toImage(renderBounds, pixelRatio: pixelRatio);
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
    public virtual DiagnosticsNode node { get; private set; } = default!;
    public virtual List<DiagnosticsNode> children { get; private set; } = default!;
    public virtual long? childIndex { get; private set; }

    internal _DiagnosticsPathNode__widget_inspector(DiagnosticsNode node, List<DiagnosticsNode> children, long? childIndex = null)
    {
        this.node = node;
        this.children = children;
        this.childIndex = childIndex;
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static List<_DiagnosticsPathNode__widget_inspector>? _followDiagnosticableChain(List<Diagnosticable> chain)
    {
        var path = new List<_DiagnosticsPathNode__widget_inspector>();
        if (!Enumerable.Any(chain))
        {
            return path;
        }
        DiagnosticsNode diagnostic = chain.First().toDiagnosticsNode();
        for (var i = 1L; i < checked(chain.Count); i += 1L)
        {
            Diagnosticable target = chain[(int)i];
            var foundMatch = false;
            List<DiagnosticsNode> childrenLocal = (List<DiagnosticsNode>)diagnostic.getChildren();
            for (var j = 0L; j < checked(childrenLocal.Count); j += 1L)
            {
                DiagnosticsNode child = childrenLocal[(int)j];
                if (Equals(child.value, target))
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
        // Boxed value types have no stable reference identity in the CLR.
        if (@object is string || @object.GetType().IsValueType)
            _value = @object;
        else
            _ref = new WeakReference<object>(@object);
    }

    public virtual object? value => DartCoreExtensions.weakTarget(_ref) ?? _value;
}

internal class _WidgetInspectorService__widget_inspector : WidgetInspectorService
{
    public virtual List<string?> _serializeRing { get; set; } = new List<string?>(Enumerable.Repeat<string?>(null, checked((int)20L)));
    public virtual long _serializeRingIndex { get; set; } = 0L;
    public virtual InspectorSelection selection { get; set; } = new InspectorSelection();
    public virtual Action? selectionChangedCallback { get; set; } = default;
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
    public virtual void registerServiceExtension(string name, Func<DartMap<string, string>, Future<DartMap<string, object?>>> callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerExtension(name: $"inspector.{name}", callback: callback);
    }

    public virtual void _registerSignalServiceExtension<TResult>(string name, Func<TResult> callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: async (parameters) =>
        {
            return new DartMap<string, object?> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback()) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
    }

    public virtual void _registerObjectGroupServiceExtension<TResult>(string name, Func<string, TResult> callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: async (parameters) =>
        {
            return new DartMap<string, object?> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(parameters.GetValueOrDefault("objectGroup")!)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
    }

    public virtual void _registerBoolServiceExtension(string name, Func<Future<bool>> getter, Func<bool, Future> setter, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: async (parameters) =>
        {
            if (parameters.ContainsKey("enabled"))
            {
                var value = parameters.GetValueOrDefault("enabled") == "true";
                await setter(DartRuntimePrimitives.RequireValue(value));
                _postExtensionStateChangedEvent(name, DartRuntimePrimitives.RequireValue(value));
            }
            return new DartMap<string, object?> { ["enabled"] = await getter() ? "true" : "false" };
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
    }

    public virtual void _postExtensionStateChangedEvent(string name, object? value)
    {
        postEvent("Flutter.ServiceExtensionStateChanged", new DartMap<string, object?> { ["extension"] = $"ext.flutter.inspector.{name}", ["value"] = value }.cast<object, object>());
    }

    public virtual void _registerServiceExtensionWithArg<TResult>(string name, Func<string?, string, TResult> callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: async (parameters) =>
        {
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("objectGroup"));
            return new DartMap<string, object?> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(parameters.GetValueOrDefault("arg"), parameters.GetValueOrDefault("objectGroup")!)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
    }

    public virtual void _registerServiceExtensionVarArgs(string name, Func<List<string>, object?> callback, RegisterServiceExtensionCallback registerExtension)
    {
        registerServiceExtension(name: name, callback: async (parameters) =>
        {
            long index = default!;
            var args = new List<string>();
            DartRuntimePrimitives.Assert(() => (index == checked(parameters.Count)) || (index == (checked(parameters.Count) - 1L)) && parameters.ContainsKey("isolateId"));
            return new DartMap<string, object?> { ["result"] = await DartAsyncRuntime.AwaitFutureOrValue<object>(callback(args)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
    }

    public virtual Future forceRebuild()
    {
        WidgetsBinding binding = WidgetsBinding.instance;
        if (binding.rootElement is not null)
        {
            binding.buildOwner!.reassemble(binding.rootElement!);
            return binding.endOfFrame;
        }
        return Future.value();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _reportStructuredError(FlutterErrorDetails details)
    {
        DartMap<string, object?> errorJson = _nodeToJson(((Diagnosticable)details).toDiagnosticsNode(), new InspectorSerializationDelegate(groupName: WidgetInspectorService._consoleObjectGroup, subtreeDepth: 5L, includeProperties: true, maxDescendantsTruncatableNode: 5L, service: this))!.cast<string, object?>();
        errorJson["errorsSinceReload"] = _errorsSinceReload;
        if (_errorsSinceReload == 0L)
        {
            errorJson["renderedErrorText"] = new TextTreeRenderer(wrapWidthProperties: FlutterError.wrapWidth, maxDescendentsTruncatableNode: 5L).render(((Diagnosticable)details).toDiagnosticsNode(style: DiagnosticsTreeStyle.error)).trimRight();
        }
        else
        {
            errorJson["renderedErrorText"] = $"Another exception was thrown: {details.summary}";
        }
        _errorsSinceReload += 1L;
        postEvent("Flutter.Error", errorJson.cast<object, object>());
    }

    public virtual void _resetErrorCount()
    {
        _errorsSinceReload = 0L;
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
        FlutterExceptionHandler defaultExceptionHandler = FlutterError.presentError;
        if (isStructuredErrorsEnabled())
        {
            FlutterError.presentError = _reportStructuredError;
        }
        DartRuntimePrimitives.Assert(() => !WidgetInspectorService._debugServiceExtensionsRegistered);
        DartRuntimePrimitives.Assert(() =>
            {
                WidgetInspectorService._debugServiceExtensionsRegistered = true;
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        Scheduler.SchedulerBinding.instance.addPersistentFrameCallback(_onFrameStart);
        _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.structuredErrors.ToString(), getter: async () => Equals(FlutterError.presentError, (FlutterExceptionHandler)_reportStructuredError), setter: (value) =>
        {
            FlutterError.presentError = value ? _reportStructuredError : defaultExceptionHandler;
            return Future.value();
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
        _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.show.ToString(), getter: async () => WidgetsBinding.instance.debugShowWidgetInspectorOverride, setter: (value) =>
        {
            if (WidgetsBinding.instance.debugShowWidgetInspectorOverride != value)
            {
                _changeWidgetSelectionMode(DartRuntimePrimitives.RequireValue(value), notifyStateChange: false);
            }
            return Future.value();
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
        if (isWidgetCreationTracked())
        {
            _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.trackRebuildDirtyWidgets.ToString(), getter: async () => _trackRebuildDirtyWidgets, setter: async (value) =>
            {
                if (value == _trackRebuildDirtyWidgets)
                {
                    return;
                }
                _rebuildStats.resetCounts();
                _trackRebuildDirtyWidgets = value;
                if (value)
                {
                    DartRuntimePrimitives.Assert(() => DebugLibrary.debugOnRebuildDirtyWidget is null);
                    DebugLibrary.debugOnRebuildDirtyWidget = _onRebuildWidget;
                    await forceRebuild();
                    return;
                }
                else
                {
                    DebugLibrary.debugOnRebuildDirtyWidget = null;
                    return;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, registerExtension: registerExtension);
            _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.widgetLocationIdMap.ToString(), callback: (Func<object?>)(() =>
            {
                return Widget_inspectorLibrary._locationIdMapToJson();
                throw new InvalidOperationException("Dart closure completed without a value.");
            }), registerExtension: registerExtension);
            _registerBoolServiceExtension(name: WidgetInspectorServiceExtensions.trackRepaintWidgets.ToString(), getter: async () => _trackRepaintWidgets, setter: async (value) =>
            {
                if (value == _trackRepaintWidgets)
                {
                    return;
                }
                _repaintStats.resetCounts();
                _trackRepaintWidgets = value;
                if (value)
                {
                    DartRuntimePrimitives.Assert(() => Rendering.DebugLibrary.debugOnProfilePaint is null);
                    Rendering.DebugLibrary.debugOnProfilePaint = _onPaint;
                    void markTreeNeedsPaint(RenderObject renderObject)
                    {
                        renderObject.markNeedsPaint();
                        renderObject.visitChildren(markTreeNeedsPaint);
                    }
                    RendererBinding.instance.renderViews.forEach((__arg0) => ((Action<RenderObject>)markTreeNeedsPaint)(DartRuntimePrimitives.ConvertValue<RenderObject>(__arg0)));
                }
                else
                {
                    Rendering.DebugLibrary.debugOnProfilePaint = null;
                }
                throw new InvalidOperationException("Dart closure completed without a value.");
            }, registerExtension: registerExtension);
        }
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.disposeAllGroups.ToString(), callback: (Func<object?>)(() =>
        {
            disposeAllGroups();
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), registerExtension: registerExtension);
        _registerObjectGroupServiceExtension(name: WidgetInspectorServiceExtensions.disposeGroup.ToString(), callback: (Func<string, object?>)((name) =>
        {
            disposeGroup(name);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), registerExtension: registerExtension);
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.isWidgetTreeReady.ToString(), callback: () => isWidgetTreeReady(null), registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.disposeId.ToString(), callback: (Func<string?, string, object?>)((objectId, objectGroup) =>
        {
            disposeId(objectId, objectGroup);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }), registerExtension: registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.setPubRootDirectories.ToString(), callback: (args) =>
        {
            setPubRootDirectories(args);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.addPubRootDirectories.ToString(), callback: (args) =>
        {
            addPubRootDirectories(args);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
        _registerServiceExtensionVarArgs(name: WidgetInspectorServiceExtensions.removePubRootDirectories.ToString(), callback: (args) =>
        {
            removePubRootDirectories(args);
            return null;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getPubRootDirectories.ToString(), callback: pubRootDirectories, registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.setSelectionById.ToString(), callback: setSelectionById, registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getParentChain.ToString(), callback: _getParentChain, registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getProperties.ToString(), callback: _getProperties, registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getChildren.ToString(), callback: _getChildren, registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getChildrenSummaryTree.ToString(), callback: _getChildrenSummaryTree, registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getChildrenDetailsSubtree.ToString(), callback: _getChildrenDetailsSubtree, registerExtension: registerExtension);
        _registerObjectGroupServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidget.ToString(), callback: _getRootWidget, registerExtension: registerExtension);
        _registerObjectGroupServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetSummaryTree.ToString(), callback: (string group) => _getRootWidgetSummaryTree(group, null), registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetSummaryTreeWithPreviews.ToString(), callback: _getRootWidgetSummaryTreeWithPreviews, registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getRootWidgetTree.ToString(), callback: _getRootWidgetTree, registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getDetailsSubtree.ToString(), callback: async (parameters) =>
        {
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("objectGroup"));
            string? subtreeDepth = parameters.GetValueOrDefault("subtreeDepth");
            return new DartMap<string, object?> { ["result"] = _getDetailsSubtree(parameters.GetValueOrDefault("arg"), parameters.GetValueOrDefault("objectGroup"), (subtreeDepth is not null) ? long.Parse(subtreeDepth, System.Globalization.CultureInfo.InvariantCulture) : 2L) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getSelectedWidget.ToString(), callback: _getSelectedWidget, registerExtension: registerExtension);
        _registerServiceExtensionWithArg(name: WidgetInspectorServiceExtensions.getSelectedSummaryWidget.ToString(), callback: _getSelectedSummaryWidget, registerExtension: registerExtension);
        _registerSignalServiceExtension(name: WidgetInspectorServiceExtensions.isWidgetCreationTracked.ToString(), callback: isWidgetCreationTracked, registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.screenshot.ToString(), callback: async (parameters) =>
        {
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("id"));
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("width"));
            DartRuntimePrimitives.Assert(() => parameters.ContainsKey("height"));
            Ui.Image? image = await screenshot(toObject(parameters.GetValueOrDefault("id")), width: double.Parse(parameters.GetValueOrDefault("width")!, System.Globalization.CultureInfo.InvariantCulture), height: double.Parse(parameters.GetValueOrDefault("height")!, System.Globalization.CultureInfo.InvariantCulture), margin: parameters.ContainsKey("margin") ? double.Parse(parameters.GetValueOrDefault("margin")!, System.Globalization.CultureInfo.InvariantCulture) : 0.0, maxPixelRatio: parameters.ContainsKey("maxPixelRatio") ? double.Parse(parameters.GetValueOrDefault("maxPixelRatio")!, System.Globalization.CultureInfo.InvariantCulture) : 1.0, debugPaint: parameters.GetValueOrDefault("debugPaint") == "true");
            if (image is null)
            {
                return new DartMap<string, object?> { ["result"] = null };
            }
            ByteData? byteData = await image.toByteData(format: ImageByteFormat.png);
            image.dispose();
            return new DartMap<string, object?> { ["result"] = Dart_convertLibrary.base64.encoder.convert(new Uint8List(byteData!.buffer)) };
            throw new InvalidOperationException("Dart closure completed without a value.");
        }, registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.getLayoutExplorerNode.ToString(), callback: _getLayoutExplorerNode, registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.setFlexFit.ToString(), callback: _setFlexFit, registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.setFlexFactor.ToString(), callback: _setFlexFactor, registerExtension: registerExtension);
        registerServiceExtension(name: WidgetInspectorServiceExtensions.setFlexProperties.ToString(), callback: _setFlexProperties, registerExtension: registerExtension);
    }

    public virtual void _clearStats()
    {
        _rebuildStats.resetCounts();
        _repaintStats.resetCounts();
    }

    public virtual void disposeAllGroups()
    {
        _groups.Clear();
        _idToReferenceData.Clear();
        _objectToId.clear();
        _nextId = 0L;
    }

    public virtual void resetAllState()
    {
        disposeAllGroups();
        selection.clear();
        resetPubRootDirectories();
    }

    public virtual void disposeGroup(string name)
    {
        HashSet<InspectorReferenceData>? references = _groups.remove(name);
        if (references is null)
        {
            return;
        }
        references.forEach((__arg0) => ((Action<InspectorReferenceData>)_decrementReferenceCount)(__arg0));
    }

    public virtual void _decrementReferenceCount(InspectorReferenceData reference)
    {
        reference.count -= 1L;
        DartRuntimePrimitives.Assert(() => reference.count >= 0L);
        if (reference.count == 0L)
        {
            object? valueLocal = reference.value;
            if (valueLocal is not null)
            {
                _objectToId.remove(valueLocal);
            }
            _idToReferenceData.remove(reference.id);
        }
    }

    public virtual string? toId(object? @object, string groupName)
    {
        if (@object is null)
        {
            return null;
        }
        HashSet<InspectorReferenceData> @group = _groups.putIfAbsent(groupName, () => new HashSet<InspectorReferenceData>());
        string? id = _objectToId[@object];
        InspectorReferenceData referenceData = default!;
        if (id is null)
        {
            id = $"inspector-{_nextId}";
            _nextId += 1L;
            _objectToId[@object] = id;
            referenceData = new InspectorReferenceData(@object, id);
            _idToReferenceData[id] = referenceData;
            @group.Add(referenceData);
        }
        else
        {
            referenceData = _idToReferenceData.GetValueOrDefault(id)!;
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
        if (id is null)
        {
            return null;
        }
        InspectorReferenceData? data = _idToReferenceData.GetValueOrDefault(id);
        if (data is null)
        {
            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Id does not exist.") }));
        }
        return data.value;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual object? toObjectForSourceLocation(string id, string? groupName = null)
    {
        object? @object = toObject(id);
        if (@object is Element)
        {
            Element @object__51282__as51313 = (Element)@object;
            return @object__51282__as51313.widget;
        }
        return @object;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void disposeId(string? id, string groupName)
    {
        if (id is null)
        {
            return;
        }
        InspectorReferenceData? referenceData = _idToReferenceData.GetValueOrDefault(id);
        if (referenceData is null)
        {
            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Id does not exist") }));
        }
        if (_groups.GetValueOrDefault(groupName)?.Remove(referenceData) != true)
        {
            throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Id is not in group") }));
        }
        _decrementReferenceCount(referenceData);
    }

    public virtual void setPubRootDirectories(List<string> pubRootDirectories)
    {
        addPubRootDirectories(pubRootDirectories);
    }

    public virtual void resetPubRootDirectories()
    {
        _pubRootDirectories = new List<string>();
        _isLocalCreationCache.clear();
    }

    public virtual void addPubRootDirectories(List<string> pubRootDirectories)
    {
        pubRootDirectories = pubRootDirectories.map((directory) => DartUri.parse(directory).path).ToList();
        var directorySet = new HashSet<string>(pubRootDirectories);
        if (_pubRootDirectories is not null)
        {
            directorySet.UnionWith(_pubRootDirectories!.Cast<string>());
        }
        _pubRootDirectories = directorySet.ToList();
        _isLocalCreationCache.clear();
    }

    public virtual void removePubRootDirectories(List<string> pubRootDirectories)
    {
        if (_pubRootDirectories is null)
        {
            return;
        }
        pubRootDirectories = pubRootDirectories.map((directory) => DartUri.parse(directory).path).ToList();
        var directorySet = new HashSet<string>(_pubRootDirectories!);
        directorySet.removeAll(pubRootDirectories);
        _pubRootDirectories = directorySet.ToList();
        _isLocalCreationCache.clear();
    }

    public virtual Future<DartMap<string, object?>> pubRootDirectories(DartMap<string, string> parameters)
    {
        return Future<DartMap<string, object?>>.value(new DartMap<string, object?> { ["result"] = _pubRootDirectories ?? new List<string>() });
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
            case Element __object55868 when !Equals(@object, selection.currentElement):
                {
                    selection.clearCandidates();
                    selection.currentElement = (Element)@object;
                    _notifyToolsOfSelection(selection.currentElement);
                    return true;
                }
            case RenderObject __object56090 when !Equals(@object, selection.current):
                {
                    selection.clearCandidates();
                    selection.current = (RenderObject)@object;
                    _notifyToolsOfSelection(selection.current);
                    return true;
                }
            default:
                throw new InvalidOperationException("Non-exhaustive Dart switch value.");
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _notifyToolsOfSelection(object? @object, bool restrictToProjectFiles = false)
    {
        inspect(@object);
        CreationLocation? location = _getSelectedWidgetLocation(restrictToSummaryTree: restrictToProjectFiles);
        if (location is not null)
        {
            postEvent("navigate", new DartMap<string, object?> { ["fileUri"] = location.file, ["line"] = location.line, ["column"] = location.column, ["source"] = "flutter.inspector" }.cast<object, object>(), stream: "ToolEvent");
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
            selection.currentElement = null;
        }
    }

    public virtual string? _devToolsInspectorUriForElement(Element element)
    {
        if ((Foundation.DebugLibrary.activeDevToolsServerAddress is not null) && (Foundation.DebugLibrary.connectedVmServiceUri is not null))
        {
            string? inspectorRef = toId(element, WidgetInspectorService._consoleObjectGroup);
            if (inspectorRef is not null)
            {
                return (string?)devToolsInspectorUri(inspectorRef);
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string devToolsInspectorUri(string inspectorRef)
    {
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.activeDevToolsServerAddress is not null);
        DartRuntimePrimitives.Assert(() => Foundation.DebugLibrary.connectedVmServiceUri is not null);
        DartUri uri = DartUri.parse(Foundation.DebugLibrary.activeDevToolsServerAddress!.ToString()).replace(queryParameters: new DartMap<string, string> { ["uri"] = DartRuntimePrimitives.RequireReference(Foundation.DebugLibrary.connectedVmServiceUri).ToString(), ["inspectorRef"] = inspectorRef });
        var devToolsInspectorUriLocal = uri.ToString();
        long startQueryParamIndex = devToolsInspectorUriLocal.IndexOf("?");
        DartRuntimePrimitives.Assert(() => startQueryParamIndex != -1L);
        return $"{devToolsInspectorUriLocal.substring(0L, startQueryParamIndex)}" + "/#/inspector" + $"{devToolsInspectorUriLocal.substring(startQueryParamIndex)}";
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getParentChain(string id, string groupName)
    {
        return _safeJsonEncode(_getParentChain(id, groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object?> _getParentChain(string? id, string groupName)
    {
        object? value = toObject(id);
        List<_DiagnosticsPathNode__widget_inspector> path = (value switch { RenderObject __object60383 => _getRenderObjectParentChain(__object60383, groupName)!, Element __object60455 => _getElementParentChain(__object60455, groupName), _ => throw DartRuntimePrimitives.AsException(new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"Cannot get parent chain for node of type {DartRuntimePrimitives.RuntimeType(value)}") })) }).ToList();
        InspectorSerializationDelegate createDelegate()
        {
            return new InspectorSerializationDelegate(groupName: groupName, service: this);
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return path.Select(pathNode =>
        {
            var serializationDelegate = createDelegate();
            return (object?)new DartMap<string, object?>
            {
                ["node"] = _nodeToJson(pathNode.node, serializationDelegate),
                ["children"] = _nodesToJson(pathNode.children, serializationDelegate, parent: pathNode.node),
                ["childIndex"] = pathNode.childIndex,
            };
        }).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<Element> _getRawElementParentChain(Element element, long? numLocalParents)
    {
        List<Element> elements = element.debugGetDiagnosticChain();
        if (numLocalParents is not null)
        {
            for (var i = 0L; i < checked(elements.Count); i += 1L)
            {
                if (_isValueCreatedByLocalProject(elements[(int)i]))
                {
                    numLocalParents = DartRuntimePrimitives.RequireValue(numLocalParents) - 1L;
                    if (numLocalParents <= 0L)
                    {
                        elements = elements.take(i + 1L).ToList();
                        break;
                    }
                }
            }
        }
        return Enumerable.Reverse(elements).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_DiagnosticsPathNode__widget_inspector> _getElementParentChain(Element element, string groupName, long? numLocalParents = null)
    {
        return Widget_inspectorLibrary._followDiagnosticableChain(_getRawElementParentChain(element, numLocalParents: numLocalParents).Cast<Diagnosticable>().ToList()) ?? new List<_DiagnosticsPathNode__widget_inspector>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<_DiagnosticsPathNode__widget_inspector>? _getRenderObjectParentChain(RenderObject? renderObject, string groupName)
    {
        var chain = new List<RenderObject>();
        while (renderObject is not null)
        {
            chain.Add(renderObject);
            renderObject = renderObject.parent;
        }
        return Widget_inspectorLibrary._followDiagnosticableChain(Enumerable.Reverse(chain).ToList().Cast<Diagnosticable>().ToList());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object?>? _nodeToJson(DiagnosticsNode? node, InspectorSerializationDelegate @delegate, bool fullDetails = true)
    {
        if (fullDetails)
        {
            return node?.toJsonMap(@delegate);
        }
        else
        {
            return node?.toJsonMapIterative(@delegate);
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isValueCreatedByLocalProject(object? value)
    {
        CreationLocation? creationLocation = Widget_inspectorLibrary._getCreationLocation(value);
        if (creationLocation is null)
        {
            return false;
        }
        return _isLocalCreationLocation(creationLocation.file);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _isLocalCreationLocationImpl(string locationUri)
    {
        string @file = DartUri.parse(locationUri).path;
        if (_pubRootDirectories is null)
        {
            return !@file.contains("packages/flutter/");
        }
        foreach (string directory in _pubRootDirectories!)
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
        bool? cachedValue = DartCollectionRuntime.NullableMapValue<bool>(_isLocalCreationCache, locationUri);
        if (cachedValue is not null)
        {
            bool cachedValue__63933__value63991 = DartRuntimePrimitives.RequireValue(cachedValue);
            return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(cachedValue__63933__value63991));
        }
        bool result = _isLocalCreationLocationImpl(locationUri);
        _isLocalCreationCache[locationUri] = result;
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string _safeJsonEncode(object? @object)
    {
        string jsonString = Dart_convertLibrary.json.encode(@object);
        _serializeRing[(int)_serializeRingIndex] = jsonString;
        _serializeRingIndex = (_serializeRingIndex + 1L) % checked(_serializeRing.Count);
        return jsonString;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DiagnosticsNode> _truncateNodes(IEnumerable<DiagnosticsNode> nodes, long maxDescendentsTruncatableNode)
    {
        if (nodes.All((node) => node.value is Element) && isWidgetCreationTracked())
        {
            List<DiagnosticsNode> localNodes = nodes.where((node) => _isValueCreatedByLocalProject(node.value)).ToList().ToList();
            if (Enumerable.Any(localNodes))
            {
                return localNodes;
            }
        }
        return nodes.take(maxDescendentsTruncatableNode).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DartMap<string, object?>> _nodesToJson(List<DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate, DiagnosticsNode? parent)
    {
        return DiagnosticsNode.toJsonList(nodes, parent, @delegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getProperties(string diagnosticsNodeId, string groupName)
    {
        return _safeJsonEncode(_getProperties(diagnosticsNodeId, groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getProperties(string? diagnosticableId, string groupName)
    {
        DiagnosticsNode? node = _idToDiagnosticsNode(diagnosticableId);
        if (node is null)
        {
            return new List<object>();
        }
        return _nodesToJson(node.getProperties().ToList(), new InspectorSerializationDelegate(groupName: groupName, service: this), parent: node).Cast<object>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildren(string diagnosticsNodeId, string groupName)
    {
        return _safeJsonEncode(_getChildren(diagnosticsNodeId, groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildren(string? diagnosticsNodeId, string groupName)
    {
        var node = ((DiagnosticsNode?)toObject(diagnosticsNodeId))!;
        var @delegate = new InspectorSerializationDelegate(groupName: groupName, service: this);
        return _nodesToJson((node is null) ? new List<DiagnosticsNode>() : _getChildrenFiltered(node, @delegate), @delegate, parent: node).Cast<object>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildrenSummaryTree(string diagnosticsNodeId, string groupName)
    {
        return _safeJsonEncode(_getChildrenSummaryTree(diagnosticsNodeId, groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode? _idToDiagnosticsNode(string? diagnosticableId)
    {
        object? @object = toObject(diagnosticableId);
        return WidgetInspectorService.objectToDiagnosticsNode(@object);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildrenSummaryTree(string? diagnosticableId, string groupName)
    {
        DiagnosticsNode? node = _idToDiagnosticsNode(diagnosticableId);
        if (node is null)
        {
            return new List<object>();
        }
        var @delegate = new InspectorSerializationDelegate(groupName: groupName, summaryTree: true, service: this);
        return _nodesToJson(_getChildrenFiltered(node, @delegate), @delegate, parent: node).Cast<object>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getChildrenDetailsSubtree(string diagnosticableId, string groupName)
    {
        return _safeJsonEncode(_getChildrenDetailsSubtree(diagnosticableId, groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<object> _getChildrenDetailsSubtree(string? diagnosticableId, string groupName)
    {
        DiagnosticsNode? node = _idToDiagnosticsNode(diagnosticableId);
        var @delegate = new InspectorSerializationDelegate(groupName: groupName, includeProperties: true, service: this);
        return _nodesToJson((node is null) ? new List<DiagnosticsNode>() : _getChildrenFiltered(node, @delegate), @delegate, parent: node).Cast<object>().ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool _shouldShowInSummaryTree(DiagnosticsNode node)
    {
        if (Equals(node.level, DiagnosticLevel.error))
        {
            return true;
        }
        object? valueLocal = node.value;
        if (valueLocal is not Diagnosticable)
        {
            return true;
        }
        if ((((Diagnosticable)valueLocal) is not Element) || !isWidgetCreationTracked())
        {
            return true;
        }
        return _isValueCreatedByLocalProject((Element)valueLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DiagnosticsNode> _getChildrenFiltered(DiagnosticsNode node, InspectorSerializationDelegate @delegate)
    {
        return _filterChildren(node.getChildren().ToList(), @delegate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DiagnosticsNode> _filterChildren(List<DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate)
    {
        var children = new List<DiagnosticsNode>();
        foreach (var child in nodes)
        {
            InspectorSerializationDelegate? updatedDelegate = _updateDelegateForWidgetInspectorEnabledState(@delegate: @delegate, node: child);
            bool inDisableWidgetInspectorScopeLocal = (updatedDelegate?.inDisableWidgetInspectorScope ?? false) || @delegate.inDisableWidgetInspectorScope;
            if (!inDisableWidgetInspectorScopeLocal && (!@delegate.summaryTree || _shouldShowInSummaryTree(child)))
            {
                children.Add(child);
            }
            else
            {
                children.AddRange(_getChildrenFiltered(child, updatedDelegate ?? @delegate));
            }
        }
        return children;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual InspectorSerializationDelegate? _updateDelegateForWidgetInspectorEnabledState(InspectorSerializationDelegate @delegate, DiagnosticsNode node)
    {
        object? valueLocal = node.value;
        if (!@delegate.inDisableWidgetInspectorScope && (valueLocal is _DisableWidgetInspectorScopeProxyElement__widget_inspector))
        {
            _DisableWidgetInspectorScopeProxyElement__widget_inspector value__72458__as72537 = (_DisableWidgetInspectorScopeProxyElement__widget_inspector)valueLocal;
            return (InspectorSerializationDelegate?)@delegate.copyWith(inDisableWidgetInspectorScope: true);
        }
        else
        {
            if (@delegate.inDisableWidgetInspectorScope && (valueLocal is _EnableWidgetInspectorScopeProxyElement__widget_inspector))
            {
                _EnableWidgetInspectorScopeProxyElement__widget_inspector value__72458__as72724 = (_EnableWidgetInspectorScopeProxyElement__widget_inspector)valueLocal;
                return (InspectorSerializationDelegate?)@delegate.copyWith(inDisableWidgetInspectorScope: false);
            }
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getRootWidget(string groupName)
    {
        return _safeJsonEncode(_getRootWidget(groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object?>? _getRootWidget(string groupName)
    {
        return _nodeToJson(WidgetsBinding.instance.rootElement?.toDiagnosticsNode(), new InspectorSerializationDelegate(groupName: groupName, service: this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getRootWidgetSummaryTree(string groupName)
    {
        return _safeJsonEncode(_getRootWidgetSummaryTree(groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object?>? _getRootWidgetSummaryTree(string groupName, Func<DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object?>?>? addAdditionalPropertiesCallback = null)
    {
        return _getRootWidgetTreeImpl(groupName: groupName, isSummaryTree: true, withPreviews: false, addAdditionalPropertiesCallback: addAdditionalPropertiesCallback);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object?>> _getRootWidgetSummaryTreeWithPreviews(DartMap<string, string> parameters)
    {
        string groupNameLocal = parameters.GetValueOrDefault("groupName")!;
        DartMap<string, object?>? result = _getRootWidgetTreeImpl(groupName: groupNameLocal, isSummaryTree: true, withPreviews: true);
        return Future<DartMap<string, object?>>.value(new DartMap<string, object?> { ["result"] = result });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object?>> _getRootWidgetTree(DartMap<string, string> parameters)
    {
        string groupNameLocal = parameters.GetValueOrDefault("groupName")!;
        var isSummaryTreeLocal = parameters.GetValueOrDefault("isSummaryTree") == "true";
        var withPreviewsLocal = parameters.GetValueOrDefault("withPreviews") == "true";
        var fullDetailsLocal = parameters.GetValueOrDefault("fullDetails") != "false";
        DartMap<string, object?>? result = _getRootWidgetTreeImpl(groupName: groupNameLocal, isSummaryTree: isSummaryTreeLocal, withPreviews: withPreviewsLocal, fullDetails: fullDetailsLocal);
        return Future<DartMap<string, object?>>.value(new DartMap<string, object?> { ["result"] = result });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object?>? _getRootWidgetTreeImpl(string groupName, bool isSummaryTree, bool withPreviews, bool fullDetails = true, Func<DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object?>?>? addAdditionalPropertiesCallback = null)
    {
        bool shouldAddAdditionalProperties = (addAdditionalPropertiesCallback is not null) || withPreviews;
        DartMap<string, object?>? combinedAddAdditionalPropertiesCallback(DiagnosticsNode node, InspectorSerializationDelegate @delegate)
        {
            DartMap<string, object?> additionalPropertiesJson = addAdditionalPropertiesCallback?.Invoke(node, @delegate) ?? new DartMap<string, object?>();
            if (!withPreviews)
            {
                return additionalPropertiesJson;
            }
            object? valueLocal = node.value;
            if (valueLocal is Element)
            {
                Element value__76023__as76053 = (Element)valueLocal;
                RenderObject? renderObject = _renderObjectOrNull(value__76023__as76053);
                if (renderObject is RenderParagraph)
                {
                    RenderParagraph renderObject__76101__as76156 = (RenderParagraph)renderObject;
                    additionalPropertiesJson["textPreview"] = renderObject__76101__as76156.text.toPlainText();
                }
            }
            return additionalPropertiesJson;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        return _nodeToJson(WidgetsBinding.instance.rootElement?.toDiagnosticsNode(), new InspectorSerializationDelegate(groupName: groupName, subtreeDepth: 1000000L, summaryTree: isSummaryTree, service: this, addAdditionalPropertiesCallback: shouldAddAdditionalProperties ? combinedAddAdditionalPropertiesCallback : null), fullDetails: fullDetails);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getDetailsSubtree(string diagnosticableId, string groupName, long subtreeDepth = 2)
    {
        return _safeJsonEncode(_getDetailsSubtree(diagnosticableId, groupName, subtreeDepth));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object?>? _getDetailsSubtree(string? diagnosticableId, string? groupName, long subtreeDepth)
    {
        DiagnosticsNode? root = _idToDiagnosticsNode(diagnosticableId);
        if (root is null)
        {
            return null;
        }
        return _nodeToJson(root, new InspectorSerializationDelegate(groupName: groupName, subtreeDepth: subtreeDepth, includeProperties: true, service: this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getSelectedWidget(string? previousSelectionId, string groupName)
    {
        if (previousSelectionId is not null)
        {
            PrintLibrary.debugPrint("previousSelectionId is deprecated in API");
        }
        return _safeJsonEncode(_getSelectedWidget(null, groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public async virtual Future<Ui.Image?> screenshot(object? @object, double width, double height, double margin = 0.0, double maxPixelRatio = 1.0, bool debugPaint = false)
    {
        if ((@object is not Element) && (@object is not RenderObject))
        {
            return null;
        }
        RenderObject? renderObject = (@object is Element) ? _renderObjectOrNull((Element)@object) : ((RenderObject?)@object)!;
        if ((renderObject is null) || !renderObject.attached)
        {
            return null;
        }
        if (renderObject.debugNeedsLayout)
        {
            PipelineOwner ownerLocal = DartRuntimePrimitives.ConvertValue<PipelineOwner>(renderObject.owner!);
            DartRuntimePrimitives.Assert(() => !ownerLocal.debugDoingLayout);
            DartRuntimePrimitives.Ignore(((Func<PipelineOwner>)(() =>
{
    var __cascade = ownerLocal;
    __cascade.flushLayout();
    __cascade.flushCompositingBits();
    __cascade.flushPaint();
    return __cascade;
}))());
            if (renderObject.debugNeedsLayout)
            {
                return null;
            }
        }
        Rect renderBounds = DartRuntimePrimitives.ConvertValue<Rect>(Widget_inspectorLibrary._calculateSubtreeBounds(renderObject));
        if (margin != 0.0)
        {
            renderBounds = renderBounds.inflate(margin);
        }
        if (renderBounds.isEmpty)
        {
            return null;
        }
        double pixelRatioLocal = Math.Min(maxPixelRatio, Math.Min(width / renderBounds.width, height / renderBounds.height));
        return await _ScreenshotPaintingContext__widget_inspector.toImage(renderObject, renderBounds, pixelRatio: pixelRatioLocal, debugPaint: debugPaint);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object?>> _getLayoutExplorerNode(DartMap<string, string> parameters)
    {
        string? diagnosticableId = parameters.GetValueOrDefault("id");
        long subtreeDepthLocal = long.Parse(parameters.GetValueOrDefault("subtreeDepth")!, System.Globalization.CultureInfo.InvariantCulture);
        string? groupNameLocal = parameters.GetValueOrDefault("groupName");
        DartMap<string, object?>? result = new DartMap<string, object?>();
        DiagnosticsNode? root = _idToDiagnosticsNode(diagnosticableId);
        if (root is null)
        {
            return Future<DartMap<string, object?>>.value(new DartMap<string, object?> { ["result"] = result });
        }
        result = _nodeToJson(root, new InspectorSerializationDelegate(groupName: groupNameLocal, summaryTree: true, subtreeDepth: subtreeDepthLocal, service: this, addAdditionalPropertiesCallback: (node, @delegate) =>
        {
            object? valueLocal = node.value;
            RenderObject? renderObject = (valueLocal is Element) ? _renderObjectOrNull((Element)valueLocal) : null;
            if (renderObject is null)
            {
                return new DartMap<string, object?>();
            }
            DiagnosticsSerializationDelegate renderObjectSerializationDelegate = @delegate.copyWith(subtreeDepth: 0L, includeProperties: true, expandPropertyValues: false);
            var additionalJson = new DartMap<string, object?>();
            RenderObject? renderParent = renderObject.parent;
            if ((renderParent is not null) && (@delegate.subtreeDepth > 0L) && @delegate.expandPropertyValues)
            {
                object? parentCreator = renderParent.debugCreator;
                if (parentCreator is DebugCreator)
                {
                    DebugCreator parentCreator__82646__as82705 = (DebugCreator)parentCreator;
                    additionalJson["parentRenderElement"] = ((Diagnosticable)parentCreator__82646__as82705.element).toDiagnosticsNode().toJsonMap(@delegate.copyWith(subtreeDepth: 0L, includeProperties: true));
                }
            }
            try
            {
                if (!renderObject.debugNeedsLayout)
                {
                    Constraints constraintsLocal = DartRuntimePrimitives.ConvertValue<Constraints>(renderObject.constraints);
                    var constraintsProperty = new DartMap<string, object?> { ["type"] = DartRuntimePrimitives.RuntimeTypeName(constraintsLocal), ["description"] = constraintsLocal.ToString() };
                    if (constraintsLocal is BoxConstraints)
                    {
                        BoxConstraints constraints__83404__as83654 = (BoxConstraints)constraintsLocal;
                        constraintsProperty.AddRange(new DartMap<string, object?> { ["minWidth"] = constraints__83404__as83654.minWidth.ToString(), ["minHeight"] = constraints__83404__as83654.minHeight.ToString(), ["maxWidth"] = constraints__83404__as83654.maxWidth.ToString(), ["maxHeight"] = constraints__83404__as83654.maxHeight.ToString() });
                    }
                    additionalJson["constraints"] = constraintsProperty;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                if (renderObject is RenderBox)
                {
                    RenderBox renderObject__81532__as84297 = (RenderBox)renderObject;
                    additionalJson["isBox"] = true;
                    additionalJson["size"] = new DartMap<string, object?> { ["width"] = renderObject__81532__as84297.size.width.ToString(), ["height"] = renderObject__81532__as84297.size.height.ToString() };
                    ParentData? parentDataLocal = DartRuntimePrimitives.ConvertValue<ParentData>(renderObject__81532__as84297.parentData);
                    if (parentDataLocal is FlexParentData)
                    {
                        FlexParentData parentData__84603__as84659 = (FlexParentData)parentDataLocal;
                        additionalJson["flexFactor"] = parentData__84603__as84659.flex ?? 0L;
                        additionalJson["flexFit"] = (parentData__84603__as84659.fit ?? FlexFit.tight).ToString();
                    }
                    else
                    {
                        if (parentDataLocal is BoxParentData)
                        {
                            BoxParentData parentData__84603__as84869 = (BoxParentData)parentDataLocal;
                            Offset offsetLocal = parentData__84603__as84869.offset;
                            additionalJson["parentData"] = new DartMap<string, object?> { ["offsetX"] = offsetLocal.dx.ToString(), ["offsetY"] = offsetLocal.dy.ToString() };
                        }
                    }
                }
                else
                {
                    if (renderObject is RenderView)
                    {
                        RenderView renderObject__81532__as85182 = (RenderView)renderObject;
                        additionalJson["size"] = new DartMap<string, object?> { ["width"] = renderObject__81532__as85182.size.width.ToString(), ["height"] = renderObject__81532__as85182.size.height.ToString() };
                    }
                }
            }
            catch (Exception)
            {
            }
            return additionalJson;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }));
        return Future<DartMap<string, object?>>.value(new DartMap<string, object?> { ["result"] = result });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object?>> _setFlexFit(DartMap<string, string> parameters)
    {
        string? id = parameters.GetValueOrDefault("id");
        string parameter = parameters.GetValueOrDefault("flexFit")!;
        FlexFit flexFit = _toEnumEntry(Enum.GetValues<FlexFit>().ToList(), parameter);
        object? @object = toObject(id);
        var succeed = false;
        if ((@object is not null) && (@object is Element))
        {
            Element @object__85909__as85983 = (Element)@object;
            RenderObject? render = _renderObjectOrNull(@object__85909__as85983);
            ParentData? parentDataLocal = render?.parentData;
            if (parentDataLocal is FlexParentData)
            {
                FlexParentData parentData__86092__as86135 = (FlexParentData)parentDataLocal;
                parentData__86092__as86135.fit = flexFit;
                render!.markNeedsLayout();
                succeed = true;
            }
        }
        return Future<DartMap<string, object?>>.value(new DartMap<string, object?> { ["result"] = succeed });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object?>> _setFlexFactor(DartMap<string, string> parameters)
    {
        string? id = parameters.GetValueOrDefault("id");
        string flexFactor = parameters.GetValueOrDefault("flexFactor")!;
        long? factor = (flexFactor == "null") ? null : long.Parse(flexFactor, System.Globalization.CultureInfo.InvariantCulture);
        object? @object = toObject(id);
        var succeed = false;
        if ((@object is not null) && (@object is Element))
        {
            Element @object__86635__as86709 = (Element)@object;
            RenderObject? render = _renderObjectOrNull(@object__86635__as86709);
            ParentData? parentDataLocal = render?.parentData;
            if (parentDataLocal is FlexParentData)
            {
                FlexParentData parentData__86818__as86861 = (FlexParentData)parentDataLocal;
                parentData__86818__as86861.flex = factor;
                render!.markNeedsLayout();
                succeed = true;
            }
        }
        return Future<DartMap<string, object?>>.value(new DartMap<string, object?> { ["result"] = succeed });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Future<DartMap<string, object?>> _setFlexProperties(DartMap<string, string> parameters)
    {
        string? id = parameters.GetValueOrDefault("id");
        MainAxisAlignment mainAxisAlignmentLocal = _toEnumEntry(Enum.GetValues<MainAxisAlignment>().ToList(), parameters.GetValueOrDefault("mainAxisAlignment")!);
        CrossAxisAlignment crossAxisAlignmentLocal = _toEnumEntry(Enum.GetValues<CrossAxisAlignment>().ToList(), parameters.GetValueOrDefault("crossAxisAlignment")!);
        object? @object = toObject(id);
        var succeed = false;
        if ((@object is not null) && (@object is Element))
        {
            Element @object__87556__as87630 = (Element)@object;
            RenderObject? render = _renderObjectOrNull(@object__87556__as87630);
            if (render is RenderFlex)
            {
                RenderFlex render__87677__as87725 = (RenderFlex)render;
                render__87677__as87725.mainAxisAlignment = mainAxisAlignmentLocal;
                render__87677__as87725.crossAxisAlignment = crossAxisAlignmentLocal;
                render__87677__as87725.markNeedsLayout();
                render__87677__as87725.markNeedsPaint();
                succeed = true;
            }
        }
        return Future<DartMap<string, object?>>.value(new DartMap<string, object?> { ["result"] = succeed });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual T _toEnumEntry<T>(List<T> enumEntries, string name)
    {
        foreach (var entry in enumEntries)
        {
            if (entry?.ToString() == name)
            {
                return entry;
            }
        }
        throw new Exception($"Enum value {name} not found");
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object?>? _getSelectedWidget(string? previousSelectionId, string groupName)
    {
        return _nodeToJson(_getSelectedWidgetDiagnosticsNode(previousSelectionId), new InspectorSerializationDelegate(groupName: groupName, service: this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode? _getSelectedWidgetDiagnosticsNode(string? previousSelectionId)
    {
        var previousSelection = ((DiagnosticsNode?)toObject(previousSelectionId))!;
        Element? current = selection.currentElement;
        return Equals(current, previousSelection?.value) ? previousSelection : current?.toDiagnosticsNode();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string getSelectedSummaryWidget(string? previousSelectionId, string groupName)
    {
        if (previousSelectionId is not null)
        {
            PrintLibrary.debugPrint("previousSelectionId is deprecated in API");
        }
        return _safeJsonEncode(_getSelectedSummaryWidget(null, groupName));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual CreationLocation? _getSelectedWidgetLocation(bool restrictToSummaryTree = false)
    {
        DiagnosticsNode? selectedNode = restrictToSummaryTree ? _getSelectedSummaryDiagnosticsNode(null) : _getSelectedWidgetDiagnosticsNode(null);
        return Widget_inspectorLibrary._getCreationLocation(selectedNode?.value);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsNode? _getSelectedSummaryDiagnosticsNode(string? previousSelectionId)
    {
        if (!isWidgetCreationTracked())
        {
            return _getSelectedWidgetDiagnosticsNode(previousSelectionId);
        }
        var previousSelection = ((DiagnosticsNode?)toObject(previousSelectionId))!;
        Element? current = selection.currentElement;
        if ((current is not null) && !_isValueCreatedByLocalProject(current))
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
        return Equals(current, previousSelection?.value) ? previousSelection : current?.toDiagnosticsNode();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DartMap<string, object?>? _getSelectedSummaryWidget(string? previousSelectionId, string groupName)
    {
        return _nodeToJson(_getSelectedSummaryDiagnosticsNode(previousSelectionId), new InspectorSerializationDelegate(groupName: groupName, service: this));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool isWidgetCreationTracked()
    {
        _widgetCreationTracked ??= (CreationLocation.of(new _WidgetForTypeTests__widget_inspector()) is not null);
        return DartRuntimePrimitives.RequireValue(_widgetCreationTracked);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void _onFrameStart(Duration timeStamp)
    {
        _frameStart = timeStamp;
        _frameNumber = PlatformDispatcher.instance.frameData.frameNumber;
        Scheduler.SchedulerBinding.instance.addPostFrameCallback((__arg0) => ((Action<Duration>)_onFrameEnd)(__arg0), debugLabel: "WidgetInspector.onFrameStart");
    }

    public virtual void _onFrameEnd(Duration timeStamp)
    {
        if (_trackRebuildDirtyWidgets)
        {
            _postStatsEvent("Flutter.RebuiltWidgets", _rebuildStats);
        }
        if (_trackRepaintWidgets)
        {
            _postStatsEvent("Flutter.RepaintWidgets", _repaintStats);
        }
    }

    public virtual void _postStatsEvent(string eventName, _ElementLocationStatsTracker__widget_inspector stats)
    {
        postEvent(eventName, stats.exportToJson(_frameStart, frameNumber: _frameNumber).cast<object, object>());
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
        _rebuildStats.add(element);
    }

    public virtual void _onPaint(RenderObject renderObject)
    {
        try
        {
            Element? elementLocal = DartRuntimePrimitives.ConvertValue<Element>(((DebugCreator?)renderObject.debugCreator)!?.element);
            if (elementLocal is not RenderObjectElement)
            {
                return;
            }
            _repaintStats.add((RenderObjectElement)elementLocal);
            ((RenderObjectElement)elementLocal).visitAncestorElements((ancestor) =>
            {
                if (ancestor is RenderObjectElement)
                {
                    return false;
                }
                _repaintStats.add(ancestor);
                return true;
                throw new InvalidOperationException("Dart closure completed without a value.");
            });
        }
        catch (Exception exceptionLocal)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            FlutterError.reportError(new FlutterErrorDetails(exception: exceptionLocal, stack: stackLocal, library: "widget inspector library", context: new ErrorDescription("while tracking widget repaints")));
        }
    }

    public virtual void performReassemble()
    {
        _clearStats();
        _resetErrorCount();
    }

    public virtual RenderObject? _renderObjectOrNull(Element element) => element.mounted ? element.renderObject : null;
}

public interface WidgetInspectorService
{
    List<string?> _serializeRing { get; }
    long _serializeRingIndex { get; set; }
    public static WidgetInspectorService _instance = new _WidgetInspectorService__widget_inspector();
    internal static bool _debugServiceExtensionsRegistered = false;
    InspectorSelection selection { get; }
    Action? selectionChangedCallback { get; set; }
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
    public void registerServiceExtension(string name, Func<DartMap<string, string>, Future<DartMap<string, object?>>> callback, RegisterServiceExtensionCallback registerExtension);
    public void _registerSignalServiceExtension<TResult>(string name, Func<TResult> callback, RegisterServiceExtensionCallback registerExtension);
    public void _registerObjectGroupServiceExtension<TResult>(string name, Func<string, TResult> callback, RegisterServiceExtensionCallback registerExtension);
    public void _registerBoolServiceExtension(string name, Func<Future<bool>> getter, Func<bool, Future> setter, RegisterServiceExtensionCallback registerExtension);
    public void _postExtensionStateChangedEvent(string name, object? value);
    public void _registerServiceExtensionWithArg<TResult>(string name, Func<string?, string, TResult> callback, RegisterServiceExtensionCallback registerExtension);
    public void _registerServiceExtensionVarArgs(string name, Func<List<string>, object?> callback, RegisterServiceExtensionCallback registerExtension);
    public Future forceRebuild();
    public void _reportStructuredError(FlutterErrorDetails details);
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
    public Future<DartMap<string, object?>> pubRootDirectories(DartMap<string, string> parameters);
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
    public List<_DiagnosticsPathNode__widget_inspector>? _getRenderObjectParentChain(RenderObject? renderObject, string groupName);
    public DartMap<string, object?>? _nodeToJson(DiagnosticsNode? node, InspectorSerializationDelegate @delegate, bool fullDetails = true);
    public bool _isValueCreatedByLocalProject(object? value);
    public bool _isLocalCreationLocationImpl(string locationUri);
    public bool _isLocalCreationLocation(string locationUri);
    public string _safeJsonEncode(object? @object);
    public List<DiagnosticsNode> _truncateNodes(IEnumerable<DiagnosticsNode> nodes, long maxDescendentsTruncatableNode);
    public List<DartMap<string, object?>> _nodesToJson(List<DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate, DiagnosticsNode? parent);
    public string getProperties(string diagnosticsNodeId, string groupName);
    public List<object> _getProperties(string? diagnosticableId, string groupName);
    public string getChildren(string diagnosticsNodeId, string groupName);
    public List<object> _getChildren(string? diagnosticsNodeId, string groupName);
    public string getChildrenSummaryTree(string diagnosticsNodeId, string groupName);
    public DiagnosticsNode? _idToDiagnosticsNode(string? diagnosticableId);
    public static DiagnosticsNode? objectToDiagnosticsNode(object? @object)
    {
        if (@object is Diagnosticable)
        {
            Diagnosticable @object__as68125 = (Diagnosticable)@object;
            return (DiagnosticsNode?)@object__as68125.toDiagnosticsNode();
        }
        return null;
    }
    public List<object> _getChildrenSummaryTree(string? diagnosticableId, string groupName);
    public string getChildrenDetailsSubtree(string diagnosticableId, string groupName);
    public List<object> _getChildrenDetailsSubtree(string? diagnosticableId, string groupName);
    public bool _shouldShowInSummaryTree(DiagnosticsNode node);
    public List<DiagnosticsNode> _getChildrenFiltered(DiagnosticsNode node, InspectorSerializationDelegate @delegate);
    public List<DiagnosticsNode> _filterChildren(List<DiagnosticsNode> nodes, InspectorSerializationDelegate @delegate);
    public InspectorSerializationDelegate? _updateDelegateForWidgetInspectorEnabledState(InspectorSerializationDelegate @delegate, DiagnosticsNode node);
    public string getRootWidget(string groupName);
    public DartMap<string, object?>? _getRootWidget(string groupName);
    public string getRootWidgetSummaryTree(string groupName);
    public DartMap<string, object?>? _getRootWidgetSummaryTree(string groupName, Func<DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object?>?>? addAdditionalPropertiesCallback = null);
    public Future<DartMap<string, object?>> _getRootWidgetSummaryTreeWithPreviews(DartMap<string, string> parameters);
    public Future<DartMap<string, object?>> _getRootWidgetTree(DartMap<string, string> parameters);
    public DartMap<string, object?>? _getRootWidgetTreeImpl(string groupName, bool isSummaryTree, bool withPreviews, bool fullDetails = true, Func<DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object?>?>? addAdditionalPropertiesCallback = null);
    public string getDetailsSubtree(string diagnosticableId, string groupName, long subtreeDepth = 2);
    public DartMap<string, object?>? _getDetailsSubtree(string? diagnosticableId, string? groupName, long subtreeDepth);
    public string getSelectedWidget(string? previousSelectionId, string groupName);
    public Future<Ui.Image?> screenshot(object? @object, double width, double height, double margin = 0.0, double maxPixelRatio = 1.0, bool debugPaint = false);
    public Future<DartMap<string, object?>> _getLayoutExplorerNode(DartMap<string, string> parameters);
    public Future<DartMap<string, object?>> _setFlexFit(DartMap<string, string> parameters);
    public Future<DartMap<string, object?>> _setFlexFactor(DartMap<string, string> parameters);
    public Future<DartMap<string, object?>> _setFlexProperties(DartMap<string, string> parameters);
    public T _toEnumEntry<T>(List<T> enumEntries, string name);
    public DartMap<string, object?>? _getSelectedWidget(string? previousSelectionId, string groupName);
    public DiagnosticsNode? _getSelectedWidgetDiagnosticsNode(string? previousSelectionId);
    public string getSelectedSummaryWidget(string? previousSelectionId, string groupName);
    public CreationLocation? _getSelectedWidgetLocation(bool restrictToSummaryTree = false);
    public DiagnosticsNode? _getSelectedSummaryDiagnosticsNode(string? previousSelectionId);
    public DartMap<string, object?>? _getSelectedSummaryWidget(string? previousSelectionId, string groupName);
    public bool isWidgetCreationTracked();
    public void _onFrameStart(Duration timeStamp);
    public void _onFrameEnd(Duration timeStamp);
    public void _postStatsEvent(string eventName, _ElementLocationStatsTracker__widget_inspector stats);
    public void postEvent(string eventKind, DartMap<object, object> eventData, string stream = "Extension");
    public void inspect(object? @object);
    public void _onRebuildWidget(Element element, bool builtOnce);
    public void _onPaint(RenderObject renderObject);
    public RenderObject? _renderObjectOrNull(Element element);
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

    public virtual long count => _count;
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
        object widgetLocal = element.widget;
        CreationLocation? locationLocal = CreationLocation.of(widgetLocal);
        if (locationLocal is null)
        {
            return;
        }
        long idLocal = Widget_inspectorLibrary._toLocationId(locationLocal);
        _LocationCount__widget_inspector entry = default!;
        if ((idLocal >= checked(_stats.Count)) || (_stats[(int)idLocal] is null))
        {
            while (idLocal >= checked(_stats.Count))
            {
                _stats.Add(null);
            }
            entry = new _LocationCount__widget_inspector(location: locationLocal, id: idLocal, local: WidgetInspectorService.instance._isLocalCreationLocation(locationLocal.file));
            if (entry.local)
            {
                newLocations.Add(entry);
            }
            _stats[(int)idLocal] = entry;
        }
        else
        {
            entry = _stats[(int)idLocal]!;
        }
        if (entry.local)
        {
            if (entry.count == 0L)
            {
                active.Add(entry);
            }
            entry.increment();
        }
    }

    public virtual void resetCounts()
    {
        foreach (_LocationCount__widget_inspector entry in active)
        {
            entry.reset();
        }
        active.Clear();
    }

    public virtual DartMap<string, object?> exportToJson(Duration startTime, long frameNumber)
    {
        var events = new List<long>(Enumerable.Repeat(0L, checked((int)(checked(active.Count) * 2L))));
        var j = 0L;
        foreach (_LocationCount__widget_inspector stat in active)
        {
            events[(int)j++] = stat.id;
            events[(int)j++] = stat.count;
        }
        var json = new DartMap<string, object?> { ["startTime"] = startTime.inMicroseconds, ["frameNumber"] = frameNumber, ["events"] = events };
        if (Enumerable.Any(newLocations))
        {
            var locationsJson = new DartMap<string, List<long>>();
            foreach (_LocationCount__widget_inspector entry in newLocations)
            {
                CreationLocation locationLocal = (CreationLocation)entry.location;
                List<long> jsonForFile = locationsJson.putIfAbsent(locationLocal.file, () => new List<long>()).ToList();
                DartRuntimePrimitives.Ignore(((Func<List<long>>)(() =>
{
    var __cascade = jsonForFile;
    __cascade.Add(entry.id);
    __cascade.Add((long)(object)locationLocal.line);
    __cascade.Add((long)(object)locationLocal.column);
    return __cascade;
}))());
            }
            json["newLocations"] = locationsJson;
        }
        if (Enumerable.Any(newLocations))
        {
            var fileLocationsMap = new DartMap<string, DartMap<string, List<object>>>();
            foreach (_LocationCount__widget_inspector entryLocal in newLocations)
            {
                CreationLocation locationAlternate = (CreationLocation)entryLocal.location;
                DartMap<string, List<object?>> locations = fileLocationsMap.putIfAbsent(locationAlternate.file, () => new DartMap<string, List<object>> { ["ids"] = new List<long>().Cast<object>().ToList(), ["lines"] = new List<long>().Cast<object>().ToList(), ["columns"] = new List<long>().Cast<object>().ToList(), ["names"] = new List<string?>().Cast<object>().ToList() }).cast<string, List<object?>>();
                locations.GetValueOrDefault("ids")!.Add(entryLocal.id);
                locations.GetValueOrDefault("lines")!.Add(locationAlternate.line);
                locations.GetValueOrDefault("columns")!.Add(locationAlternate.column);
                locations.GetValueOrDefault("names")!.Add(locationAlternate.ToString());
            }
            json["locations"] = fileLocationsMap;
        }
        resetCounts();
        newLocations.Clear();
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

    public WidgetInspector(Key? key = null, Widget child = default!, TapBehaviorButtonBuilder? tapBehaviorButtonBuilder = default!, ExitWidgetSelectionButtonBuilder? exitWidgetSelectionButtonBuilder = default!, MoveExitWidgetSelectionButtonBuilder? moveExitWidgetSelectionButtonBuilder = default!) : base(key: key)
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

    internal virtual ValueNotifier<bool> _selectionOnTapEnabled => WidgetsBinding.instance.debugWidgetInspectorSelectionOnTapEnabled;
    internal virtual bool _isSelectModeWithSelectionOnTapEnabled => DartRuntimePrimitives.ConvertValue<bool>(isSelectMode && _selectionOnTapEnabled.value);
    public override void initState()
    {
        base.initState();
        WidgetInspectorService.instance.selection.addListener(_selectionInformationChanged);
        WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier.addListener(_selectionInformationChanged);
        _selectionOnTapEnabled.addListener(_selectionInformationChanged);
        selection = WidgetInspectorService.instance.selection;
        isSelectMode = WidgetsBinding.instance.debugShowWidgetInspectorOverride;
    }

    public override void dispose()
    {
        WidgetInspectorService.instance.selection.removeListener(_selectionInformationChanged);
        WidgetsBinding.instance.debugShowWidgetInspectorOverrideNotifier.removeListener(_selectionInformationChanged);
        _selectionOnTapEnabled.removeListener(_selectionInformationChanged);
        base.dispose();
    }

    internal virtual void _selectionInformationChanged() => setState(() =>
    {
        selection = WidgetInspectorService.instance.selection;
        isSelectMode = WidgetsBinding.instance.debugShowWidgetInspectorOverride;
    });
    internal virtual bool _hitTestHelper(List<RenderObject> hits, List<RenderObject> edgeHits, Offset position, RenderObject @object, Matrix4 transform)
    {
        var hit = false;
        Matrix4? inverse = Matrix4.tryInvert(transform);
        if (inverse is null)
        {
            return false;
        }
        Offset localPosition = MatrixUtils.transformPoint(inverse, position);
        List<DiagnosticsNode> children = @object.debugDescribeChildren();
        for (long i = checked(children.Count) - 1L; i >= 0L; i -= 1L)
        {
            DiagnosticsNode diagnostics = children[(int)i];
            if (Equals(diagnostics.style, DiagnosticsTreeStyle.offstage) || (diagnostics.value is not RenderObject))
            {
                continue;
            }
            var child = ((RenderObject?)diagnostics.value!)!;
            Rect? paintClip = @object.describeApproximatePaintClip(child);
            if ((paintClip is not null) && !DartRuntimePrimitives.RequireValue(paintClip).contains(localPosition))
            {
                Rect paintClip__106714__value106780 = DartRuntimePrimitives.RequireValue(paintClip);
                continue;
            }
            Matrix4 childTransform = transform.clone();
            @object.applyPaintTransform(child, childTransform);
            if (_hitTestHelper(hits, edgeHits, position, child, childTransform))
            {
                hit = true;
            }
        }
        Rect bounds = @object.semanticBounds;
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

    public virtual List<RenderObject> hitTest(Offset position, RenderObject root)
    {
        var regularHits = new List<RenderObject>();
        var edgeHits = new List<RenderObject>();
        _hitTestHelper(regularHits, edgeHits, position, root, root.getTransformTo(null));
        double area(RenderObject @object)
        {
            Size sizeLocal = @object.semanticBounds.size;
            return sizeLocal.width * sizeLocal.height;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        regularHits.sort((a, b) => area(a).CompareTo(area(b)));
        var hits = new HashSet<RenderObject>();
        return hits.ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _inspectAt(Offset position)
    {
        if (!_isSelectModeWithSelectionOnTapEnabled)
        {
            return;
        }
        var ignorePointer = ((RenderIgnorePointer?)_ignorePointerKey.currentContext!.findRenderObject()!)!;
        RenderObject userRender = ignorePointer.child!;
        List<RenderObject> selected = hitTest(position, userRender);
        selection.candidates = Widget_inspectorLibrary._filterInspectorHitCandidatesToModalRouteScope(selected);
    }

    internal virtual void _handlePanDown(DragDownDetails @event)
    {
        _lastPointerLocation = @event.globalPosition;
        _inspectAt(@event.globalPosition);
    }

    internal virtual void _handlePanUpdate(DragUpdateDetails @event)
    {
        _lastPointerLocation = @event.globalPosition;
        _inspectAt(@event.globalPosition);
    }

    internal virtual void _handlePanEnd(DragEndDetails details)
    {
        DorotiView view = View.of(context);
        Rect bounds = (Offset.zero & view.physicalSize / view.devicePixelRatio).deflate(Widget_inspectorLibrary._kOffScreenMargin);
        if (!bounds.contains(DartRuntimePrimitives.RequireValue(_lastPointerLocation)))
        {
            selection.clear();
        }
        else
        {
            WidgetInspectorService.instance._notifyToolsOfSelection(selection.current, restrictToProjectFiles: true);
        }
    }

    internal virtual void _handleTap()
    {
        if (!_isSelectModeWithSelectionOnTapEnabled)
        {
            return;
        }
        if (_lastPointerLocation is not null)
        {
            _inspectAt(DartRuntimePrimitives.RequireValue(_lastPointerLocation));
            WidgetInspectorService.instance._notifyToolsOfSelection(selection.current, restrictToProjectFiles: true);
        }
    }

    public override Widget build(BuildContext context)
    {
        return new Stack(children: new List<Widget> { new GestureDetector(onTap: () => _handleTap(), onPanDown: _handlePanDown, onPanEnd: _handlePanEnd, onPanUpdate: _handlePanUpdate, behavior: HitTestBehavior.opaque, excludeFromSemantics: true, child: new IgnorePointer(ignoring: _isSelectModeWithSelectionOnTapEnabled, key: _ignorePointerKey, child: widget.child)), Positioned.CreateFill(child: new _InspectorOverlay__widget_inspector(selection: selection)) });
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class EnableWidgetInspectorScope : ProxyWidget
{
    public EnableWidgetInspectorScope(Key? key = null, Widget child = default!) : base(key: key, child: child)
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
    public DisableWidgetInspectorScope(Key? key = null, Widget child = default!) : base(key: key, child: child)
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
    public virtual Action onPressed { get; private set; } = default!;
    public virtual string semanticsLabel { get; private set; } = default!;
    public virtual IconData icon { get; private set; } = default!;
    public virtual GlobalKey<IState>? buttonKey { get; private set; }
    public virtual InspectorButtonVariant variant { get; private set; } = default!;
    public virtual bool? toggledOn { get; private set; }
    public const double buttonSize = 32.0;
    public const double buttonIconSize = 18.0;

    protected InspectorButton(Key? key = null, Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, GlobalKey<IState>? buttonKey = null, InspectorButtonVariant variant = default!, bool? toggledOn = null) : base(key: key)
    {
        this.onPressed = onPressed;
        this.semanticsLabel = semanticsLabel;
        this.icon = icon;
        this.buttonKey = buttonKey;
        this.variant = variant;
        this.toggledOn = toggledOn;
    }

    protected InspectorButton(Action onPressed, string semanticsLabel, IconData icon, GlobalKey<IState>? buttonKey = null)
        : this(null, onPressed, semanticsLabel, icon, buttonKey) { }

    protected static InspectorButton CreateFilled(Key? key = null, Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, GlobalKey<IState>? buttonKey = null)
    {
        throw new InvalidOperationException("Dart abstract constructors cannot be invoked directly.");
    }

    protected static InspectorButton CreateToggle(Key? key = null, Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!, bool toggledOn = true)
    {
        throw new InvalidOperationException("Dart abstract constructors cannot be invoked directly.");
    }

    protected static InspectorButton CreateIconOnly(Key? key = null, Action onPressed = default!, string semanticsLabel = default!, IconData icon = default!)
    {
        throw new InvalidOperationException("Dart abstract constructors cannot be invoked directly.");
    }

    public virtual double iconSizeForVariant
    {
        get
        {
            switch (variant)
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
        }
    }
    public abstract Color foregroundColor(BuildContext context);
    public abstract Color backgroundColor(BuildContext context);
    public abstract override Widget build(BuildContext context);
}

public class InspectorSelection : ChangeNotifier
{
    internal virtual List<RenderObject> _candidates { get; set; } = new List<RenderObject>();
    internal virtual long _index { get; set; } = 0L;
    internal virtual RenderObject? _current { get; set; } = default;
    internal virtual Element? _currentElement { get; set; } = default;

    public InspectorSelection()
    {
    }

    public virtual List<RenderObject> candidates
    {
        get => _candidates;
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
        get => _index;
        set
        {
            var __value = value;
            _index = DartRuntimePrimitives.RequireValue(__value);
            _computeCurrent();
        }
    }
    public virtual void clear()
    {
        _candidates = new List<RenderObject>();
        _index = 0L;
        _computeCurrent();
    }

    public virtual void clearCandidates()
    {
        if (!Enumerable.Any(_candidates))
        {
            return;
        }
        _candidates = new List<RenderObject>();
        _index = 0L;
    }

    public virtual RenderObject? current
    {
        get => active ? _current : null;
        set
        {
            var __value = value;
            if (!Equals(_current, __value))
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
            return (_currentElement?.debugIsDefunct ?? true) ? null : _currentElement;
        }
        set
        {
            var element = value;
            if (element?.debugIsDefunct ?? false)
            {
                _currentElement = null;
                _current = null;
                notifyListeners();
                return;
            }
            if (!Equals(currentElement, element))
            {
                _currentElement = element;
                _current = element?.findRenderObject();
                notifyListeners();
            }
        }
    }
    internal virtual void _computeCurrent()
    {
        if (_index < checked(candidates.Count))
        {
            _current = candidates[(int)index];
            _currentElement = ((DebugCreator?)_current?.debugCreator)!?.element;
            notifyListeners();
        }
        else
        {
            _current = null;
            _currentElement = null;
            notifyListeners();
        }
    }

    public virtual bool active => DartRuntimePrimitives.ConvertValue<bool>((_current is not null) && _current!.attached);
}

internal class _InspectorOverlay__widget_inspector : LeafRenderObjectWidget
{
    public virtual InspectorSelection selection { get; private set; } = default!;

    internal _InspectorOverlay__widget_inspector(InspectorSelection selection)
    {
        this.selection = selection;
    }

    public override RenderObject createRenderObject(BuildContext context)
    {
        return new _RenderInspectorOverlay__widget_inspector(selection: selection);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void updateRenderObject(BuildContext context, RenderObject renderObject)
    {
        var __renderObject = (_RenderInspectorOverlay__widget_inspector)renderObject;
        __renderObject.selection = selection;
    }

}

public class _RenderInspectorOverlay__widget_inspector : RenderBox
{
    internal virtual InspectorSelection _selection { get; set; } = default!;

    internal _RenderInspectorOverlay__widget_inspector(InspectorSelection selection)
    {
        _selection = selection;
    }

    public virtual InspectorSelection selection
    {
        get => _selection;
        set
        {
            var __value = value;
            if (!Equals(__value, _selection))
            {
                _selection = __value;
            }
            markNeedsPaint();
        }
    }
    public override bool sizedByParent => true;
    public override bool alwaysNeedsCompositing => true;
    public override Size computeDryLayout(BoxConstraints constraints)
    {
        return constraints.constrain(Size.infinite);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void paint(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => needsCompositing);
        context.addLayer(new _InspectorOverlayLayer__widget_inspector(overlayRect: Rect.fromLTWH(offset.dx, offset.dy, size.width, size.height), selection: selection, rootRenderObject: true ? parent! : null));
    }

}

public class _TransformedRect__widget_inspector
{
    public virtual Rect rect { get; private set; } = default!;
    public virtual Matrix4 transform { get; private set; } = default!;

    internal _TransformedRect__widget_inspector(RenderObject @object, RenderObject? ancestor)
    {
        rect = @object.semanticBounds;
        transform = @object.getTransformTo(ancestor);
    }

    public override bool Equals(object? other)
    {
        var __other = other as _TransformedRect__widget_inspector;
        if (__other is null) return false;
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _TransformedRect__widget_inspector) && Equals(__other.rect, rect) && Equals(__other.transform, transform);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(rect, transform));
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
        if (!Equals(DartRuntimePrimitives.RuntimeType(__other), GetType()))
        {
            return false;
        }
        return (__other is _InspectorOverlayRenderState__widget_inspector) && Equals(__other.overlayRect, overlayRect) && Equals(__other.selected, selected) && CollectionsLibrary.listEquals(__other.candidates, candidates) && (__other.tooltip == tooltip);
    }

    public override int GetHashCode() => DartRuntimePrimitives.ConvertValue<int>(FoundationRuntimePorts.ObjectHash(overlayRect, selected, FoundationRuntimePorts.ObjectHashAll(candidates), tooltip));
}

public static partial class Widget_inspectorLibrary
{
    internal static long _kMaxTooltipLines = 5L;
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kTooltipBackgroundColor = Color.fromARGB(230L, 60L, 60L, 60L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kHighlightedRenderObjectFillColor = Color.fromARGB(128L, 128L, 128L, 255L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Color _kHighlightedRenderObjectBorderColor = Color.fromARGB(128L, 64L, 64L, 128L);
}

public static partial class Widget_inspectorLibrary
{
    internal static Element? _elementForRenderObject(RenderObject? @object)
    {
        object? creator = @object?.debugCreator;
        if (creator is DebugCreator)
        {
            DebugCreator creator__124233__as124271 = (DebugCreator)creator;
            return creator__124233__as124271.element;
        }
        return null;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static IModalRoute? _modalRouteForRenderObject(RenderObject? @object)
    {
        Element? element = _elementForRenderObject(@object);
        if (element is null)
        {
            return null;
        }
        return ModalRoute<object>.untypedOf(element);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static double _inspectorHitArea(RenderObject @object)
    {
        Size sizeLocal = @object.semanticBounds.size;
        return sizeLocal.width * sizeLocal.height;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static IModalRoute? _inspectorScopeRouteForHits(List<RenderObject> hits)
    {
        foreach (var hit in hits)
        {
            IModalRoute? route = _modalRouteForRenderObject(hit);
            if (route?.isCurrent ?? false)
            {
                return route;
            }
        }
        RenderObject? smallestHit = default!;
        double smallestArea = double.PositiveInfinity;
        foreach (var hitLocal in hits)
        {
            IModalRoute? routeLocal = _modalRouteForRenderObject(hitLocal);
            if (routeLocal is null)
            {
                continue;
            }
            double area = _inspectorHitArea(hitLocal);
            if (area < smallestArea)
            {
                smallestArea = area;
                smallestHit = hitLocal;
            }
        }
        if (smallestHit is not null)
        {
            return _modalRouteForRenderObject(smallestHit);
        }
        return _modalRouteForRenderObject(hits.First());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static List<RenderObject> _filterInspectorHitCandidatesToModalRouteScope(List<RenderObject> hits)
    {
        if (!Enumerable.Any(hits))
        {
            return hits;
        }
        List<RenderObject> onstageHits = hits.where((hit) =>
        {
            IModalRoute? route = _modalRouteForRenderObject(hit);
            return (route is null) || !route.offstage;
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList().ToList();
        if (!Enumerable.Any(onstageHits))
        {
            return onstageHits;
        }
        IModalRoute? scopeRoute = _inspectorScopeRouteForHits(onstageHits);
        List<RenderObject> scopedHits = onstageHits.where((hit) => DartRuntimePrimitives.Identical(_modalRouteForRenderObject(hit), scopeRoute)).ToList().ToList();
        scopedHits.sort((a, b) => _inspectorHitArea(a).CompareTo(_inspectorHitArea(b)));
        return scopedHits;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

internal class _InspectorOverlayLayer__widget_inspector : Layer
{
    public virtual InspectorSelection selection { get; set; } = default!;
    public virtual Rect overlayRect { get; private set; } = default!;
    public virtual RenderObject? rootRenderObject { get; private set; }
    internal virtual _InspectorOverlayRenderState__widget_inspector? _lastState { get; set; } = default;
    internal virtual Picture? _picture { get; set; } = default;
    internal virtual TextPainter? _textPainter { get; set; } = default;
    internal virtual double? _textPainterMaxWidth { get; set; } = default;

    internal _InspectorOverlayLayer__widget_inspector(Rect overlayRect, InspectorSelection selection, RenderObject? rootRenderObject)
    {
        this.overlayRect = overlayRect;
        this.selection = selection;
        this.rootRenderObject = rootRenderObject;
    }

    public override void dispose()
    {
        _textPainter?.dispose();
        _textPainter = null;
        _picture?.dispose();
        base.dispose();
    }

    public override void addToScene(SceneBuilder builder)
    {
        if (!selection.active)
        {
            return;
        }
        RenderObject selectedLocal = selection.current!;
        if (!_isInInspectorRenderObjectTree(selectedLocal))
        {
            return;
        }
        var candidatesLocal = new List<_TransformedRect__widget_inspector>();
        foreach (RenderObject candidate in selection.candidates)
        {
            if (Equals(candidate, selectedLocal) || !candidate.attached || !_isInInspectorRenderObjectTree(candidate) || !DartRuntimePrimitives.Identical(Widget_inspectorLibrary._modalRouteForRenderObject(candidate), Widget_inspectorLibrary._modalRouteForRenderObject(selectedLocal)))
            {
                continue;
            }
            candidatesLocal.Add(new _TransformedRect__widget_inspector(candidate, rootRenderObject));
        }
        var selectedRect = new _TransformedRect__widget_inspector(selectedLocal, rootRenderObject);
        string widgetName = ((Diagnosticable)selection.currentElement!).toStringShort();
        string widthLocal = selectedRect.rect.width.toStringAsFixed(1L);
        string heightLocal = selectedRect.rect.height.toStringAsFixed(1L);
        var state = new _InspectorOverlayRenderState__widget_inspector(overlayRect: overlayRect, selected: selectedRect, tooltip: $"{widgetName} ({widthLocal} x {heightLocal})", textDirection: TextDirection.ltr, candidates: candidatesLocal);
        if (!Equals(state, _lastState))
        {
            _lastState = state;
            _picture?.dispose();
            _picture = _buildPicture(state);
        }
        builder.addPicture(Offset.zero, _picture!);
    }

    internal virtual Picture _buildPicture(_InspectorOverlayRenderState__widget_inspector state)
    {
        var recorder = new PictureRecorder();
        var canvas = new Canvas(recorder, state.overlayRect);
        Size sizeLocal = state.overlayRect.size;
        canvas.translate(state.overlayRect.left, state.overlayRect.top);
        var fillPaint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = Widget_inspectorLibrary._kHighlightedRenderObjectFillColor;
    return __cascade;
}))();
        var borderPaint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = Widget_inspectorLibrary._kHighlightedRenderObjectBorderColor;
    return __cascade;
}))();
        Rect selectedPaintRect = state.selected.rect.deflate(0.5);
        DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = canvas;
    __cascade.save();
    __cascade.transform(state.selected.transform.storage);
    __cascade.drawRect(selectedPaintRect, fillPaint);
    __cascade.drawRect(selectedPaintRect, borderPaint);
    __cascade.restore();
    return __cascade;
}))());
        foreach (_TransformedRect__widget_inspector transformedRect in state.candidates)
        {
            DartRuntimePrimitives.Ignore(((Func<Canvas>)(() =>
{
    var __cascade = canvas;
    __cascade.save();
    __cascade.transform(transformedRect.transform.storage);
    __cascade.drawRect(transformedRect.rect.deflate(0.5), borderPaint);
    __cascade.restore();
    return __cascade;
}))());
        }
        Rect targetRect = MatrixUtils.transformRect(state.selected.transform, state.selected.rect);
        if (!targetRect.hasNaN)
        {
            var target = new Offset(targetRect.left, targetRect.center.dy);
            var offsetFromWidget = 9.0;
            double verticalOffset = (targetRect.height / 2L) + offsetFromWidget;
            _paintDescription(canvas, state.tooltip, state.textDirection, target, verticalOffset, sizeLocal, targetRect);
        }
        return recorder.endRecording();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _paintDescription(Canvas canvas, string message, TextDirection textDirection, Offset target, double verticalOffset, Size size, Rect targetRect)
    {
        canvas.save();
        double maxWidthLocal = Math.Max(size.width - (2L * (Widget_inspectorLibrary._kScreenEdgeMargin + Widget_inspectorLibrary._kTooltipPadding)), 0);
        var textSpan = ((TextSpan?)_textPainter?.text)!;
        if ((_textPainter is null) || (textSpan!.text != message) || (_textPainterMaxWidth != maxWidthLocal))
        {
            _textPainterMaxWidth = maxWidthLocal;
            _textPainter?.dispose();
            _textPainter = ((Func<TextPainter>)(() =>
{
    var __cascade = new TextPainter();
    __cascade.maxLines = Widget_inspectorLibrary._kMaxTooltipLines;
    __cascade.ellipsis = "...";
    __cascade.text = new TextSpan(style: Widget_inspectorLibrary._messageStyle, text: message);
    __cascade.textDirection = textDirection;
    __cascade.layout(maxWidth: maxWidthLocal);
    return __cascade;
}))();
        }
        Size tooltipSize = _textPainter!.size + new Offset(Widget_inspectorLibrary._kTooltipPadding * 2L, Widget_inspectorLibrary._kTooltipPadding * 2L);
        Offset tipOffset = GeometryLibrary.positionDependentBox(size: size, childSize: tooltipSize, target: target, verticalOffset: verticalOffset, preferBelow: false);
        var tooltipBackground = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = Widget_inspectorLibrary._kTooltipBackgroundColor;
    return __cascade;
}))();
        canvas.drawRect(Rect.fromPoints(tipOffset, tipOffset.translate(tooltipSize.width, tooltipSize.height)), tooltipBackground);
        double wedgeY = tipOffset.dy;
        bool tooltipBelow = tipOffset.dy > target.dy;
        if (!tooltipBelow)
        {
            wedgeY += tooltipSize.height;
        }
        double wedgeSize = Widget_inspectorLibrary._kTooltipPadding * 2L;
        double wedgeX = Math.Max(tipOffset.dx, target.dx) + (wedgeSize * 2L);
        wedgeX = Math.Min(wedgeX, tipOffset.dx + tooltipSize.width - (wedgeSize * 2L));
        var wedge = new List<Offset> { new Offset(wedgeX - wedgeSize, wedgeY), new Offset(wedgeX + wedgeSize, wedgeY), new Offset(wedgeX, wedgeY + (tooltipBelow ? -wedgeSize : wedgeSize)) };
        canvas.drawPath(((Func<Path>)(() =>
{
    var __cascade = new Path();
    __cascade.addPolygon(wedge, true);
    return __cascade;
}))(), tooltipBackground);
        _textPainter!.paint(canvas, tipOffset + new Offset(Widget_inspectorLibrary._kTooltipPadding, Widget_inspectorLibrary._kTooltipPadding));
        canvas.restore();
    }

    public override bool findAnnotations<S>(AnnotationResult<S> result, Offset localPosition, bool onlyFirst = default!)
    {
        return false;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual bool _isInInspectorRenderObjectTree(RenderObject child)
    {
        RenderObject? current = child.parent;
        while (current is not null)
        {
            if ((current is RenderStack) && ((RenderStack)current).getChildrenAsList().any((child) => child is _RenderInspectorOverlay__widget_inspector))
            {
                RenderStack current__134258__as134376 = (RenderStack)current;
                return Equals(rootRenderObject, current__134258__as134376);
            }
            current = current.parent;
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
    internal static TextStyle _messageStyle = new TextStyle(color: new Color(4294967295L), fontSize: 10.0, height: 1.2);
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

    internal virtual ValueNotifier<bool> _selectionOnTapEnabled => WidgetsBinding.instance.debugWidgetInspectorSelectionOnTapEnabled;
    internal virtual Widget? _moveExitWidgetSelectionButton
    {
        get
        {
            MoveExitWidgetSelectionButtonBuilder? buttonBuilder = widget.moveExitWidgetSelectionButtonBuilder;
            if (buttonBuilder is null)
            {
                return null;
            }
            TextDirection textDirection = Directionality.of(context);
            var buttonLabel = $"Move to the {((_usesDefaultAlignment == Equals(textDirection, TextDirection.ltr)) ? "right" : "left")}";
            return (Widget?)new _WidgetInspectorButton__widget_inspector(button: buttonBuilder(context, onPressed: () =>
            {
                _changeButtonGroupAlignment();
                _onTooltipHidden();
            }, semanticsLabel: buttonLabel, usesDefaultAlignment: _usesDefaultAlignment), onTooltipVisible: () =>
            {
                _changeTooltipMessage(buttonLabel);
            }, onTooltipHidden: () => _onTooltipHidden());
        }
    }
    internal virtual Widget _exitWidgetSelectionButton
    {
        get
        {
            var buttonLabel = "Exit Select Widget mode";
            return new _WidgetInspectorButton__widget_inspector(button: widget.exitWidgetSelectionButtonBuilder(context, onPressed: _exitWidgetSelectionMode, semanticsLabel: buttonLabel, key: _exitWidgetSelectionButtonKey), onTooltipVisible: () =>
            {
                _changeTooltipMessage(buttonLabel);
            }, onTooltipHidden: () => _onTooltipHidden());
        }
    }
    internal virtual Widget? _tapBehaviorButton
    {
        get
        {
            TapBehaviorButtonBuilder? buttonBuilder = widget.tapBehaviorButtonBuilder;
            if (buttonBuilder is null)
            {
                return null;
            }
            return (Widget?)new _WidgetInspectorButton__widget_inspector(button: buttonBuilder(context, onPressed: () => _changeSelectionOnTapMode(default), semanticsLabel: "Change widget selection mode for taps", selectionOnTapEnabled: _selectionOnTapEnabled.value), onTooltipVisible: () => _changeSelectionOnTapTooltip(), onTooltipHidden: () => _onTooltipHidden());
        }
    }
    internal virtual bool _tooltipVisible => DartRuntimePrimitives.ConvertValue<bool>(_tooltipMessage is not null);
    public override Widget build(BuildContext context)
    {
        double bottomPadding = Math.Max(_kExitWidgetSelectionButtonMargin, MediaQuery.viewPaddingOf(context).bottom);
        Widget selectionModeButtons = new Column(children: new List<Widget> { _exitWidgetSelectionButton });
        Widget buttonGroup = new Stack(alignment: AlignmentDirectional.topCenter, children: new List<Widget> { new CustomPaint(painter: new _ExitWidgetSelectionTooltipPainter__widget_inspector(tooltipMessage: _tooltipMessage, buttonKey: _exitWidgetSelectionButtonKey, usesDefaultAlignment: _usesDefaultAlignment)), new Row(crossAxisAlignment: CrossAxisAlignment.end, mainAxisAlignment: MainAxisAlignment.center, children: new List<Widget>()) });
        return Positioned.CreateDirectional(textDirection: Directionality.of(context), start: _usesDefaultAlignment ? _kExitWidgetSelectionButtonMargin : null, end: _usesDefaultAlignment ? null : _kExitWidgetSelectionButtonMargin, bottom: bottomPadding, child: buttonGroup);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _exitWidgetSelectionMode()
    {
        WidgetInspectorService.instance._changeWidgetSelectionMode(false);
        _changeSelectionOnTapMode(selectionOnTapEnabled: _defaultSelectionOnTapEnabled);
    }

    internal virtual void _changeSelectionOnTapMode(bool? selectionOnTapEnabled = null)
    {
        bool newValue = selectionOnTapEnabled ?? !_selectionOnTapEnabled.value;
        _selectionOnTapEnabled.value = newValue;
        WidgetInspectorService.instance.selection.clear();
        if (_tooltipVisible)
        {
            _changeSelectionOnTapTooltip();
        }
    }

    internal virtual void _changeSelectionOnTapTooltip()
    {
        _changeTooltipMessage(_selectionOnTapEnabled.value ? "Disable widget selection for taps" : "Enable widget selection for taps");
    }

    internal virtual void _changeButtonGroupAlignment()
    {
        if (mounted)
        {
            setState(() =>
            {
                _usesDefaultAlignment = !_usesDefaultAlignment;
            });
        }
    }

    internal virtual void _onTooltipHidden()
    {
        _changeTooltipMessage(null);
    }

    internal virtual void _changeTooltipMessage(string? message)
    {
        if (mounted)
        {
            setState(() =>
            {
                _tooltipMessage = message;
            });
        }
    }

}

internal class _WidgetInspectorButton__widget_inspector : StatefulWidget
{
    public virtual Widget button { get; private set; } = default!;
    public virtual Action onTooltipVisible { get; private set; } = default!;
    public virtual Action onTooltipHidden { get; private set; } = default!;
    internal static Duration _tooltipShownOnLongPressDuration = Duration.Create(milliseconds: 1500L);
    internal static Duration _tooltipDelayDuration = Duration.Create(milliseconds: 100L);

    internal _WidgetInspectorButton__widget_inspector(Widget button, Action onTooltipVisible, Action onTooltipHidden)
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
        _tooltipVisibleTimer?.cancel();
        _tooltipVisibleTimer = null;
        _tooltipHiddenTimer?.cancel();
        _tooltipHiddenTimer = null;
        base.dispose();
    }

    public override Widget build(BuildContext context)
    {
        return new Stack(alignment: AlignmentDirectional.topCenter, children: new List<Widget> { new GestureDetector(onLongPress: () => {
_tooltipVisibleAfter(_WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
_tooltipHiddenAfter(_WidgetInspectorButton__widget_inspector._tooltipShownOnLongPressDuration + _WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
}, child: new MouseRegion(onEnter: (_) => {
_tooltipVisibleAfter(_WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
}, onExit: (_) => {
_tooltipHiddenAfter(_WidgetInspectorButton__widget_inspector._tooltipDelayDuration);
}, child: widget.button)) });
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
        Timer? timer = isVisible ? _tooltipVisibleTimer : _tooltipHiddenTimer;
        if (timer?.isActive ?? false)
        {
            timer!.cancel();
        }
        if (isVisible)
        {
            _tooltipVisibleTimer = new Timer(duration, () =>
            {
                widget.onTooltipVisible();
            });
        }
        else
        {
            _tooltipHiddenTimer = new Timer(duration, () =>
            {
                widget.onTooltipHidden();
            });
        }
    }

}

internal class _ExitWidgetSelectionTooltipPainter__widget_inspector : CustomPainter
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
        var isVisible = tooltipMessage is not null;
        if (!isVisible)
        {
            return;
        }
        RenderObject? buttonRenderObject = buttonKey.currentContext?.findRenderObject();
        if (buttonRenderObject is null)
        {
            return;
        }
        var tooltipPadding = 4.0;
        var tooltipSpacing = 6.0;
        var tooltipTextPainter = ((Func<TextPainter>)(() =>
{
    var __cascade = new TextPainter();
    __cascade.maxLines = 1L;
    __cascade.ellipsis = "...";
    __cascade.text = new TextSpan(text: tooltipMessage, style: Widget_inspectorLibrary._messageStyle);
    __cascade.textDirection = TextDirection.ltr;
    __cascade.layout();
    return __cascade;
}))();
        var tooltipPaint = ((Func<Paint>)(() =>
{
    var __cascade = new Paint();
    __cascade.style = PaintingStyle.fill;
    __cascade.color = Widget_inspectorLibrary._kTooltipBackgroundColor;
    return __cascade;
}))();
        double buttonWidth = buttonRenderObject.paintBounds.width;
        Size textSize = tooltipTextPainter.size;
        double textWidth = textSize.width;
        double textHeight = textSize.height;
        double tooltipWidth = textWidth + tooltipPadding * 2L;
        double tooltipHeight = textHeight + tooltipPadding * 2L;
        double tooltipXOffset = usesDefaultAlignment ? (0L - buttonWidth) : (0L - (tooltipWidth - buttonWidth));
        double tooltipYOffset = 0L - tooltipHeight - tooltipSpacing;
        canvas.drawRect(Rect.fromLTWH(tooltipXOffset, tooltipYOffset, tooltipWidth, tooltipHeight), tooltipPaint);
        tooltipTextPainter.paint(canvas, new Offset(tooltipXOffset + tooltipPadding, tooltipYOffset + tooltipPadding));
    }

    public override bool shouldRepaint(CustomPainter oldDelegate)
    {
        var __oldDelegate = (_ExitWidgetSelectionTooltipPainter__widget_inspector)oldDelegate;
        return tooltipMessage != __oldDelegate.tooltipMessage;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public static partial class Widget_inspectorLibrary
{
    internal static bool _isDebugCreator(DiagnosticsNode node) => node is DiagnosticsDebugCreator;
}

public static partial class Widget_inspectorLibrary
{
    public static IEnumerable<DiagnosticsNode> debugTransformDebugCreator(IEnumerable<DiagnosticsNode> properties)
    {
        if (!Foundation.ConstantsLibrary.kDebugMode)
        {
            return new List<DiagnosticsNode>();
        }
        var pending = new List<DiagnosticsNode>();
        ErrorSummary? errorSummary = default!;
        foreach (var node in properties)
        {
            if (node is ErrorSummary)
            {
                ErrorSummary node__145947__as145977 = (ErrorSummary)node;
                errorSummary = node__145947__as145977;
                break;
            }
        }
        var foundStackTrace = false;
        var result = new List<DiagnosticsNode>();
        foreach (var nodeLocal in properties)
        {
            if (!foundStackTrace && (nodeLocal is DiagnosticsStackTrace))
            {
                DiagnosticsStackTrace node__146133__as146183 = (DiagnosticsStackTrace)nodeLocal;
                foundStackTrace = true;
            }
            if (_isDebugCreator(nodeLocal))
            {
                result.AddRange(_parseDiagnosticsNode(nodeLocal, errorSummary).Cast<DiagnosticsNode>());
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
        result.AddRange(pending.Cast<DiagnosticsNode>());
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static IEnumerable<DiagnosticsNode> _parseDiagnosticsNode(DiagnosticsNode node, ErrorSummary? errorSummary)
    {
        DartRuntimePrimitives.Assert(() => _isDebugCreator(node));
        try
        {
            var debugCreator = ((DebugCreator?)node.value!)!;
            Element elementLocal = debugCreator.element;
            return _describeRelevantUserCode(elementLocal, errorSummary);
        }
        catch (Exception error)
        {
            var stackLocal = new System.Diagnostics.StackTrace();
            DartAsyncRuntime.scheduleMicrotask(() =>
            {
                FlutterError.reportError(new FlutterErrorDetails(exception: error, stack: stackLocal, library: "widget inspector", informationCollector: () => new List<DiagnosticsNode> { DiagnosticsNode.CreateMessage("This exception was caught while trying to describe the user-relevant code of another error.") }));
            });
            return new List<DiagnosticsNode>();
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static IEnumerable<DiagnosticsNode> _describeRelevantUserCode(Element element, ErrorSummary? errorSummary)
    {
        if (!WidgetInspectorService.instance.isWidgetCreationTracked())
        {
            return new List<DiagnosticsNode> { new ErrorDescription("Widget creation tracking is currently disabled. Enabling " + "it enables improved error messages. It can be enabled by passing " + "`--track-widget-creation` to `flutter run` or `flutter test`."), new ErrorSpacer() };
        }
        bool isOverflowError()
        {
            if ((errorSummary is not null) && !string.IsNullOrEmpty(errorSummary.value?.ToString()))
            {
                object summary = errorSummary.value;
                if ((summary is string) && ((string)summary).startsWith("A RenderFlex overflowed by"))
                {
                    string summary__148033__as148079 = (string)summary;
                    return true;
                }
            }
            return false;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        var nodes = new List<DiagnosticsNode>();
        bool processElement(Element target)
        {
            if (debugIsLocalCreationLocation(target))
            {
                DiagnosticsNode? devToolsDiagnostic = default!;
                if (isOverflowError())
                {
                    string? devToolsInspectorUri = WidgetInspectorService.instance._devToolsInspectorUriForElement(target);
                    if (devToolsInspectorUri is not null)
                    {
                        devToolsDiagnostic = DartRuntimePrimitives.ConvertValue<DiagnosticsNode>(new DevToolsDeepLinkProperty($"To inspect this widget in Flutter DevTools, visit: {devToolsInspectorUri}", devToolsInspectorUri));
                    }
                }
                nodes.AddRange(new List<DiagnosticsNode> { new DiagnosticsBlock(name: "The relevant error-causing widget was", children: new List<DiagnosticsNode> { new ErrorDescription($"{((Diagnosticable)target.widget).toStringShort()} {_describeCreationLocation(target)}") }), new ErrorSpacer() }.Cast<DiagnosticsNode>());
                return false;
            }
            return true;
            throw new InvalidOperationException("Dart control flow completed without a value.");
        }
        if (processElement(element))
        {
            element.visitAncestorElements(processElement);
        }
        return nodes;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DevToolsDeepLinkProperty : DiagnosticsProperty<string>
{
    public DevToolsDeepLinkProperty(string description, string url) : base("", url, description: description, level: DiagnosticLevel.info)
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
                CreationLocation? location = _getCreationLocation(@object);
                if (location is not null)
                {
                    isLocal = WidgetInspectorService.instance._isLocalCreationLocation(location.file);
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
        CreationLocation? location = CreationLocation.of(widget);
        return (location is not null) && WidgetInspectorService.instance._isLocalCreationLocation(location.file);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static string? _describeCreationLocation(object @object)
    {
        CreationLocation? location = _getCreationLocation(@object);
        return location?.ToString();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static CreationLocation? _getCreationLocation(object? @object)
    {
        object? candidate = ((@object is Element) && !((Element)@object).debugIsDefunct) ? ((Element)@object).widget : @object;
        return (candidate is null) ? null : CreationLocation.of(candidate);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static DartMap<CreationLocation, long> _locationToId = new();
}

public static partial class Widget_inspectorLibrary
{
    internal static List<CreationLocation> _locations = new();
}

public static partial class Widget_inspectorLibrary
{
    internal static long _toLocationId(CreationLocation location)
    {
        long? id = DartCollectionRuntime.NullableMapValue<long>(_locationToId, location);
        if (id is not null)
        {
            long id__152830__value152866 = DartRuntimePrimitives.RequireValue(id);
            return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id__152830__value152866));
        }
        id = checked(_locations.Count);
        _locations.Add(location);
        _locationToId[location] = DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id));
        return DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(id));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public static partial class Widget_inspectorLibrary
{
    internal static DartMap<string, object?> _locationIdMapToJson()
    {
        var idsKey = "ids";
        var linesKey = "lines";
        var columnsKey = "columns";
        var namesKey = "names";
        var fileLocationsMap = new DartMap<string, DartMap<string, List<object>>>();
        foreach (var entry in _locationToId.entries)
        {
            CreationLocation location = entry.key;
            DartMap<string, List<object?>> locations = fileLocationsMap.putIfAbsent(location.file, () => new DartMap<string, List<object>> { [idsKey] = new List<long>().Cast<object>().ToList(), [linesKey] = new List<long>().Cast<object>().ToList(), [columnsKey] = new List<long>().Cast<object>().ToList(), [namesKey] = new List<string?>().Cast<object>().ToList() }).cast<string, List<object?>>();
            locations.GetValueOrDefault(idsKey)!.Add(entry.value);
            locations.GetValueOrDefault(linesKey)!.Add(location.line);
            locations.GetValueOrDefault(columnsKey)!.Add(location.column);
            locations.GetValueOrDefault(namesKey)!.Add(location.name);
        }
        return fileLocationsMap.cast<string, object?>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class InspectorSerializationDelegate : DiagnosticsSerializationDelegate
{
    public virtual WidgetInspectorService service { get; private set; } = default!;
    public virtual string? groupName { get; private set; }
    public virtual bool summaryTree { get; private set; } = default!;
    public virtual long maxDescendantsTruncatableNode { get; private set; } = default!;
    public new virtual bool includeProperties { get; private set; } = default!;
    public new virtual long subtreeDepth { get; private set; } = default!;
    public virtual bool expandPropertyValues { get; private set; } = default!;
    public virtual bool inDisableWidgetInspectorScope { get; private set; } = default!;
    public virtual Func<DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object?>?>? addAdditionalPropertiesCallback { get; private set; }
    internal virtual List<DiagnosticsNode> _nodesCreatedByLocalProject { get; private set; } = new List<DiagnosticsNode>();

    public InspectorSerializationDelegate(string? groupName = null, bool summaryTree = false, long maxDescendantsTruncatableNode = -1, bool expandPropertyValues = true, long subtreeDepth = 1, bool includeProperties = false, WidgetInspectorService service = default!, Func<DiagnosticsNode, InspectorSerializationDelegate, DartMap<string, object?>?>? addAdditionalPropertiesCallback = null, bool inDisableWidgetInspectorScope = false)
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

    internal virtual bool _interactive => DartRuntimePrimitives.ConvertValue<bool>(groupName is not null);
    public virtual DartMap<string, object?> additionalNodeProperties(DiagnosticsNode node, bool fullDetails = true)
    {
        var result = new DartMap<string, object?>();
        object? valueLocal = node.value;
        if (summaryTree && fullDetails)
        {
            result["summaryTree"] = true;
        }
        if (_interactive)
        {
            result["valueId"] = service.toId(valueLocal, groupName!);
        }
        CreationLocation? creationLocation = Widget_inspectorLibrary._getCreationLocation(valueLocal);
        if (creationLocation is not null)
        {
            if (fullDetails)
            {
                result["locationId"] = Widget_inspectorLibrary._toLocationId(creationLocation);
                result["creationLocation"] = creationLocation.toJsonMap();
            }
            if (service._isLocalCreationLocation(creationLocation.file))
            {
                _nodesCreatedByLocalProject.Add(node);
                result["createdByLocalProject"] = true;
            }
        }
        if (addAdditionalPropertiesCallback is not null)
        {
            result.AddRange(addAdditionalPropertiesCallback!(node, this) ?? new DartMap<string, object?>());
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override DiagnosticsSerializationDelegate delegateForNode(DiagnosticsNode node)
    {
        return (summaryTree || (subtreeDepth > 1L) || service._shouldShowInSummaryTree(node)) ? copyWith(subtreeDepth: subtreeDepth - 1L) : this;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DiagnosticsNode> filterChildren(List<DiagnosticsNode> nodes, DiagnosticsNode owner)
    {
        return service._filterChildren(nodes, this);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual List<DiagnosticsNode> filterProperties(List<DiagnosticsNode> nodes, DiagnosticsNode owner)
    {
        bool createdByLocalProject = _nodesCreatedByLocalProject.Contains(owner);
        return nodes.where((node) =>
        {
            return !node.isFiltered(createdByLocalProject ? DiagnosticLevel.fine : DiagnosticLevel.info);
            throw new InvalidOperationException("Dart closure completed without a value.");
        }).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override List<DiagnosticsNode> truncateNodesList(List<DiagnosticsNode> nodes, DiagnosticsNode? owner)
    {
        if ((maxDescendantsTruncatableNode >= 0L) && owner!.allowTruncate && (checked(nodes.Count) > maxDescendantsTruncatableNode))
        {
            nodes = service._truncateNodes(nodes.Cast<DiagnosticsNode>(), maxDescendantsTruncatableNode);
        }
        return nodes;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual DiagnosticsSerializationDelegate copyWith(long? subtreeDepth = null, bool? includeProperties = null, bool? expandPropertyValues = null, bool? inDisableWidgetInspectorScope = null)
    {
        return new InspectorSerializationDelegate(groupName: groupName, summaryTree: summaryTree, maxDescendantsTruncatableNode: maxDescendantsTruncatableNode, expandPropertyValues: expandPropertyValues ?? this.expandPropertyValues, subtreeDepth: subtreeDepth ?? this.subtreeDepth, includeProperties: includeProperties ?? this.includeProperties, service: service, addAdditionalPropertiesCallback: addAdditionalPropertiesCallback, inDisableWidgetInspectorScope: inDisableWidgetInspectorScope ?? this.inDisableWidgetInspectorScope);
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
    internal virtual DartMap<K, V?> _primitives { get; private set; } = new DartMap<K, V?>();

    internal virtual bool _isPrimitive(object? key)
    {
        return (key is null) || (key is string) || (key is double) || (key is bool);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public V? this[K key]
    {
        get
        {
            if (_isPrimitive(key))
            {
                return _primitives.GetValueOrDefault(key);
            }
            else
            {
                return ((V?)_objects[key!])!;
            }
        }
        set
        {
            if (_isPrimitive(key))
            {
                _primitives[key] = value;
            }
            else
            {
                _objects[key!] = value;
            }
        }
    }

    public virtual V? remove(K key)
    {
        if (_isPrimitive(key))
        {
            return _primitives.remove(key);
        }
        else
        {
            var result = ((V?)_objects[key!])!;
            _objects[key] = null;
            return result;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void clear()
    {
        _objects = new Expando<object>();
        _primitives.Clear();
    }

}

public static partial class Widget_inspectorLibrary
{
    public static class @developer
    {
        public static class CreationLocation
        {
            public static Runtime.CreationLocation? of(object? value) => Runtime.CreationLocation.of(value);
        }
    }
}
