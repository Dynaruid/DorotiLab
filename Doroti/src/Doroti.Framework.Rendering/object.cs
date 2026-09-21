// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/object.dart
using Doroti.Runtime;
using Doroti.Ui;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public interface IRenderLayoutCallback
{
    void layoutCallback();
}

public class ParentData
{
    public ParentData() { }

    public virtual void detach() { }

    public override string ToString() => "<none>";
}

public delegate void PaintingContextCallback(PaintingContext context, Offset offset);

internal delegate Rect _TransformRect__object(Matrix4 transform, Rect rect);

public class PaintingContext : ClipContext
{
    internal virtual ContainerLayer _containerLayer { get; private set; } = default!;
    public virtual Rect estimatedBounds { get; private set; } = default!;
    internal virtual PictureLayer? _currentLayer { get; set; } = default;
    internal virtual PictureRecorder? _recorder { get; set; } = default;
    internal virtual Canvas? _canvas { get; set; } = default;

    public PaintingContext(ContainerLayer _containerLayer, Rect estimatedBounds)
    {
        this._containerLayer = _containerLayer;
        this.estimatedBounds = estimatedBounds;
    }

    public static void repaintCompositedChild(
        RenderObject child,
        bool debugAlsoPaintedParent = false
    )
    {
        DartRuntimePrimitives.Assert(() => child._needsPaint);
        _repaintCompositedChild(child, debugAlsoPaintedParent: debugAlsoPaintedParent);
    }

    internal static void _repaintCompositedChild(
        RenderObject child,
        bool debugAlsoPaintedParent = false,
        PaintingContext? childContext = null
    )
    {
        FrameworkWorkCounters.Add(FrameworkWork.RepaintBoundary);
        DartRuntimePrimitives.Assert(() => child.isRepaintBoundary);
        DartRuntimePrimitives.Assert(() =>
        {
            child.debugRegisterRepaintBoundaryPaint(
                includedParent: debugAlsoPaintedParent,
                includedChild: true
            );
            return true;
        });
        var childLayer = ((OffsetLayer?)child._layerHandle.layer)!;
        if (childLayer is null)
        {
            DartRuntimePrimitives.Assert(() => debugAlsoPaintedParent);
            DartRuntimePrimitives.Assert(() => child._layerHandle.layer is null);
            OffsetLayer layerLocal = child.updateCompositedLayer(oldLayer: null);
            child._layerHandle.layer = childLayer = layerLocal;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => debugAlsoPaintedParent || childLayer.attached);
            Offset? debugOldOffset = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                debugOldOffset = childLayer!.offset;
                return true;
            });
            childLayer.removeAllChildren();
            OffsetLayer updatedLayer = child.updateCompositedLayer(oldLayer: childLayer);
            DartRuntimePrimitives.Assert(() =>
                DartRuntimePrimitives.Identical(updatedLayer, childLayer)
            );
            DartRuntimePrimitives.Assert(() => Equals(debugOldOffset, updatedLayer.offset));
        }
        child._needsCompositedLayerUpdate = false;
        DartRuntimePrimitives.Assert(() =>
            DartRuntimePrimitives.Identical(childLayer, child._layerHandle.layer)
        );
        DartRuntimePrimitives.Assert(() => child._layerHandle.layer is OffsetLayer);
        DartRuntimePrimitives.Assert(() =>
        {
            childLayer!.debugCreator =
                child.debugCreator ?? (object?)DartRuntimePrimitives.RuntimeType(child);
            return true;
        });
        childContext ??= new PaintingContext(childLayer, child.paintBounds);
        child._paintWithContext(childContext, Offset.zero);
        DartRuntimePrimitives.Assert(() =>
            DartRuntimePrimitives.Identical(childLayer, child._layerHandle.layer)
        );
        childContext.stopRecordingIfNeeded();
    }

    public static void updateLayerProperties(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => child.isRepaintBoundary && child._wasRepaintBoundary);
        DartRuntimePrimitives.Assert(() => !child._needsPaint);
        DartRuntimePrimitives.Assert(() => child._layerHandle.layer is not null);
        var childLayer = ((OffsetLayer?)child._layerHandle.layer!)!;
        Offset? debugOldOffset = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            debugOldOffset = childLayer.offset;
            return true;
        });
        OffsetLayer updatedLayer = child.updateCompositedLayer(oldLayer: childLayer);
        DartRuntimePrimitives.Assert(() =>
            DartRuntimePrimitives.Identical(updatedLayer, childLayer)
        );
        DartRuntimePrimitives.Assert(() => Equals(debugOldOffset, updatedLayer.offset));
        child._needsCompositedLayerUpdate = false;
    }

    public static void debugInstrumentRepaintCompositedChild(
        RenderObject child,
        bool debugAlsoPaintedParent = false,
        PaintingContext customContext = default!
    )
    {
        DartRuntimePrimitives.Assert(() =>
        {
            _repaintCompositedChild(
                child,
                debugAlsoPaintedParent: debugAlsoPaintedParent,
                childContext: customContext
            );
            return true;
        });
    }

    public virtual void paintChild(RenderObject child, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
        {
            DebugLibrary.debugOnProfilePaint?.Invoke(child);
            return true;
        });
        if (child.isRepaintBoundary)
        {
            stopRecordingIfNeeded();
            _compositeChild(child, offset);
        }
        else
        {
            if (child._wasRepaintBoundary)
            {
                DartRuntimePrimitives.Assert(() => child._layerHandle.layer is OffsetLayer);
                child._layerHandle.layer = null;
                child._paintWithContext(this, offset);
            }
            else
            {
                child._paintWithContext(this, offset);
            }
        }
    }

    internal virtual void _compositeChild(RenderObject child, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => !_isRecording);
        DartRuntimePrimitives.Assert(() => child.isRepaintBoundary);
        DartRuntimePrimitives.Assert(() => (_canvas is null) || (_canvas!.getSaveCount() == 1L));
        if (child._needsPaint || !child._wasRepaintBoundary)
        {
            repaintCompositedChild(child, debugAlsoPaintedParent: true);
        }
        else
        {
            if (child._needsCompositedLayerUpdate)
            {
                updateLayerProperties(child);
            }
            DartRuntimePrimitives.Assert(() =>
            {
                child.debugRegisterRepaintBoundaryPaint();
                child._layerHandle.layer!.debugCreator = child.debugCreator ?? (object?)child;
                return true;
            });
        }
        DartRuntimePrimitives.Assert(() => child._layerHandle.layer is OffsetLayer);
        var childOffsetLayer = ((OffsetLayer?)child._layerHandle.layer!)!;
        childOffsetLayer.offset = offset;
        appendLayer(childOffsetLayer);
    }

    public virtual void appendLayer(Layer layer)
    {
        DartRuntimePrimitives.Assert(() => !_isRecording);
        layer.remove();
        _containerLayer.append(layer);
    }

    internal virtual bool _isRecording
    {
        get
        {
            var hasCanvas = _canvas is not null;
            DartRuntimePrimitives.Assert(() =>
            {
                if (hasCanvas)
                {
                    DartRuntimePrimitives.Assert(() => _currentLayer is not null);
                    DartRuntimePrimitives.Assert(() => _recorder is not null);
                    DartRuntimePrimitives.Assert(() => _canvas is not null);
                }
                else
                {
                    DartRuntimePrimitives.Assert(() => _currentLayer is null);
                    DartRuntimePrimitives.Assert(() => _recorder is null);
                    DartRuntimePrimitives.Assert(() => _canvas is null);
                }
                return true;
            });
            return hasCanvas;
        }
    }
    public virtual PictureRecorder recorder
    {
        get
        {
            if (_recorder is null)
            {
                _startRecording();
            }
            DartRuntimePrimitives.Assert(() => _currentLayer is not null);
            return _recorder!;
        }
    }
    public override Canvas canvas
    {
        get
        {
            if (_canvas is null)
            {
                _startRecording();
            }
            DartRuntimePrimitives.Assert(() => _currentLayer is not null);
            return _canvas!;
        }
    }

    internal virtual void _startRecording()
    {
        DartRuntimePrimitives.Assert(() => !_isRecording);
        _currentLayer = new PictureLayer(estimatedBounds);
        _recorder = RendererBinding.instance.createPictureRecorder();
        _canvas = RendererBinding.instance.createCanvas(_recorder!);
        _containerLayer.append(_currentLayer!);
    }

    public virtual Action addCompositionCallback(Action<Layer> callback)
    {
        return _containerLayer.addCompositionCallback(callback);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void stopRecordingIfNeeded()
    {
        if (!_isRecording)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (DebugLibrary.debugRepaintRainbowEnabled)
            {
                var paint = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.style = PaintingStyle.stroke;
                            __cascade.strokeWidth = 6.0;
                            __cascade.color = DebugLibrary.debugCurrentRepaintColor.toColor();
                            return __cascade;
                        }
                    )
                )();
                canvas.drawRect(estimatedBounds.deflate(3.0), paint);
            }
            if (DebugLibrary.debugPaintLayerBordersEnabled)
            {
                var paintLocal = (
                    (Func<Paint>)(
                        () =>
                        {
                            var __cascade = new Paint();
                            __cascade.style = PaintingStyle.stroke;
                            __cascade.strokeWidth = 1.0;
                            __cascade.color = new Color(4294940672L);
                            return __cascade;
                        }
                    )
                )();
                canvas.drawRect(estimatedBounds, paintLocal);
            }
            return true;
        });
        _currentLayer!.picture = _recorder!.endRecording();
        FrameworkWorkCounters.Add(FrameworkWork.NewPicture);
        _currentLayer = null;
        _recorder = null;
        _canvas = null;
    }

    public virtual void setIsComplexHint()
    {
        if (_currentLayer is null)
        {
            _startRecording();
        }
        _currentLayer!.isComplexHint = true;
    }

    public virtual void setWillChangeHint()
    {
        if (_currentLayer is null)
        {
            _startRecording();
        }
        _currentLayer!.willChangeHint = true;
    }

    public virtual void addLayer(Layer layer)
    {
        stopRecordingIfNeeded();
        appendLayer(layer);
    }

    public virtual void pushLayer(
        ContainerLayer childLayer,
        Action<PaintingContext, Offset> painter,
        Offset offset,
        Rect? childPaintBounds = null
    )
    {
        if (childLayer.hasChildren)
        {
            childLayer.removeAllChildren();
        }
        stopRecordingIfNeeded();
        appendLayer(childLayer);
        PaintingContext childContext = createChildContext(
            childLayer,
            childPaintBounds ?? estimatedBounds
        );
        painter(childContext, offset);
        childContext.stopRecordingIfNeeded();
    }

    public virtual PaintingContext createChildContext(ContainerLayer childLayer, Rect bounds)
    {
        return new PaintingContext(childLayer, bounds);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ClipRectLayer? pushClipRect(
        bool needsCompositing,
        Offset offset,
        Rect clipRect,
        Action<PaintingContext, Offset> painter,
        Clip clipBehavior = Clip.hardEdge,
        ClipRectLayer? oldLayer = null
    )
    {
        if (Equals(clipBehavior, Clip.none))
        {
            painter(this, offset);
            return null;
        }
        Rect offsetClipRect = clipRect.shift(offset);
        if (needsCompositing)
        {
            ClipRectLayer layer = oldLayer ?? new ClipRectLayer();
            (
                (Func<ClipRectLayer>)(
                    () =>
                    {
                        var __cascade = layer;
                        __cascade.clipRect = offsetClipRect;
                        __cascade.clipBehavior = clipBehavior;
                        return __cascade;
                    }
                )
            )();
            pushLayer(layer, painter, offset, childPaintBounds: offsetClipRect);
            return layer;
        }
        else
        {
            clipRectAndPaint(
                offsetClipRect,
                clipBehavior,
                offsetClipRect,
                () => painter(this, offset)
            );
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ClipRRectLayer? pushClipRRect(
        bool needsCompositing,
        Offset offset,
        Rect bounds,
        RRect clipRRect,
        Action<PaintingContext, Offset> painter,
        Clip clipBehavior = Clip.antiAlias,
        ClipRRectLayer? oldLayer = null
    )
    {
        if (Equals(clipBehavior, Clip.none))
        {
            painter(this, offset);
            return null;
        }
        Rect offsetBounds = bounds.shift(offset);
        RRect offsetClipRRect = clipRRect.shift(offset);
        if (needsCompositing)
        {
            ClipRRectLayer layer = oldLayer ?? new ClipRRectLayer();
            (
                (Func<ClipRRectLayer>)(
                    () =>
                    {
                        var __cascade = layer;
                        __cascade.clipRRect = offsetClipRRect;
                        __cascade.clipBehavior = clipBehavior;
                        return __cascade;
                    }
                )
            )();
            pushLayer(layer, painter, offset, childPaintBounds: offsetBounds);
            return layer;
        }
        else
        {
            clipRRectAndPaint(
                offsetClipRRect,
                clipBehavior,
                offsetBounds,
                () => painter(this, offset)
            );
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ClipRSuperellipseLayer? pushClipRSuperellipse(
        bool needsCompositing,
        Offset offset,
        Rect bounds,
        RSuperellipse clipRSuperellipse,
        Action<PaintingContext, Offset> painter,
        Clip clipBehavior = Clip.antiAlias,
        ClipRSuperellipseLayer? oldLayer = null
    )
    {
        if (Equals(clipBehavior, Clip.none))
        {
            painter(this, offset);
            return null;
        }
        Rect offsetBounds = bounds.shift(offset);
        RSuperellipse offsetShape = clipRSuperellipse.shift(offset);
        if (needsCompositing)
        {
            ClipRSuperellipseLayer layer = oldLayer ?? new ClipRSuperellipseLayer();
            (
                (Func<ClipRSuperellipseLayer>)(
                    () =>
                    {
                        var __cascade = layer;
                        __cascade.clipRSuperellipse = offsetShape;
                        __cascade.clipBehavior = clipBehavior;
                        return __cascade;
                    }
                )
            )();
            pushLayer(layer, painter, offset, childPaintBounds: offsetBounds);
            return layer;
        }
        else
        {
            clipRSuperellipseAndPaint(
                offsetShape,
                clipBehavior,
                offsetBounds,
                () => painter(this, offset)
            );
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ClipPathLayer? pushClipPath(
        bool needsCompositing,
        Offset offset,
        Rect bounds,
        Path clipPath,
        Action<PaintingContext, Offset> painter,
        Clip clipBehavior = Clip.antiAlias,
        ClipPathLayer? oldLayer = null
    )
    {
        if (Equals(clipBehavior, Clip.none))
        {
            painter(this, offset);
            return null;
        }
        Rect offsetBounds = bounds.shift(offset);
        Path offsetClipPath = clipPath.shift(offset);
        if (needsCompositing)
        {
            ClipPathLayer layer = oldLayer ?? new ClipPathLayer();
            (
                (Func<ClipPathLayer>)(
                    () =>
                    {
                        var __cascade = layer;
                        __cascade.clipPath = offsetClipPath;
                        __cascade.clipBehavior = clipBehavior;
                        return __cascade;
                    }
                )
            )();
            pushLayer(layer, painter, offset, childPaintBounds: offsetBounds);
            return layer;
        }
        else
        {
            clipPathAndPaint(
                offsetClipPath,
                clipBehavior,
                offsetBounds,
                () => painter(this, offset)
            );
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ColorFilterLayer pushColorFilter(
        Offset offset,
        ColorFilter colorFilter,
        Action<PaintingContext, Offset> painter,
        ColorFilterLayer? oldLayer = null
    )
    {
        ColorFilterLayer layer = oldLayer ?? new ColorFilterLayer();
        layer.colorFilter = colorFilter;
        pushLayer(layer, painter, offset);
        return layer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TransformLayer? pushTransform(
        bool needsCompositing,
        Offset offset,
        Matrix4 transform,
        Action<PaintingContext, Offset> painter,
        TransformLayer? oldLayer = null
    )
    {
        var effectiveTransform = (
            (Func<Matrix4>)(
                () =>
                {
                    var __cascade = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
                    __cascade.multiply(transform);
                    __cascade.translateByDouble(-offset.dx, -offset.dy, 0, 1);
                    return __cascade;
                }
            )
        )();
        if (needsCompositing)
        {
            TransformLayer layer = oldLayer ?? new TransformLayer();
            layer.transform = effectiveTransform;
            pushLayer(
                layer,
                painter,
                offset,
                childPaintBounds: MatrixUtils.inverseTransformRect(
                    effectiveTransform,
                    estimatedBounds
                )
            );
            return layer;
        }
        else
        {
            (
                (Func<Canvas>)(
                    () =>
                    {
                        var __cascade = canvas;
                        __cascade.save();
                        __cascade.transform(effectiveTransform.storage);
                        return __cascade;
                    }
                )
            )();
            painter(this, offset);
            canvas.restore();
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual OpacityLayer pushOpacity(
        Offset offset,
        long alpha,
        Action<PaintingContext, Offset> painter,
        OpacityLayer? oldLayer = null
    )
    {
        OpacityLayer layer = oldLayer ?? new OpacityLayer();
        (
            (Func<OpacityLayer>)(
                () =>
                {
                    var __cascade = layer;
                    __cascade.alpha = alpha;
                    __cascade.offset = offset;
                    return __cascade;
                }
            )
        )();
        pushLayer(layer, painter, Offset.zero);
        return layer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() =>
        $"{objectRuntimeTypeFunctions.objectRuntimeType(this, "PaintingContext")}#{GetHashCode()}(layer: {_containerLayer}, canvas bounds: {estimatedBounds})";
}

public abstract class Constraints
{
    protected Constraints() { }

    public abstract bool isTight { get; }
    public abstract bool isNormalized { get; }

    public virtual bool debugAssertIsValid(
        bool isAppliedConstraint = false,
        InformationCollector? informationCollector = null
    )
    {
        DartRuntimePrimitives.Assert(() => isNormalized);
        return isNormalized;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public delegate void RenderObjectVisitor(RenderObject child);

public delegate void LayoutCallback<T>(T constraints)
    where T : Constraints;

internal class _LocalSemanticsHandle__object : SemanticsHandle
{
    internal virtual PipelineOwner _owner { get; private set; } = default!;
    public virtual Action? listener { get; private set; }

    internal _LocalSemanticsHandle__object(PipelineOwner owner, Action? listener)
    {
        this.listener = listener;
        _owner = owner;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        if (listener is not null)
        {
            _owner.semanticsOwner!.removeListener(listener!);
        }
        _owner._didDisposeSemanticsHandle();
    }
}

public class PipelineOwner : DiagnosticableTreeMixin
{
    public virtual Action? onNeedVisualUpdate { get; private set; }
    public virtual Action? onSemanticsOwnerCreated { get; private set; }
    public virtual Action<SemanticsUpdate>? onSemanticsUpdate { get; private set; }
    public virtual Action? onSemanticsOwnerDisposed { get; private set; }
    internal virtual RenderObject? _rootNode { get; set; } = default;
    internal virtual bool _shouldMergeDirtyNodes { get; set; } = false;
    internal virtual List<RenderObject> _nodesNeedingLayout { get; set; } =
        new List<RenderObject>();
    private List<RenderObject> _nodesNeedingLayoutScratch = new List<RenderObject>();
    internal virtual bool _debugDoingLayout { get; set; } = false;
    internal virtual bool _debugDoingChildLayout { get; set; } = false;
    internal virtual bool _debugAllowMutationsToDirtySubtrees { get; set; } = false;
    internal virtual List<RenderObject> _nodesNeedingCompositingBitsUpdate { get; private set; } =
        new List<RenderObject>();
    internal virtual List<RenderObject> _nodesNeedingPaint { get; set; } = new List<RenderObject>();
    private List<RenderObject> _nodesNeedingPaintScratch = new List<RenderObject>();
    internal virtual bool _debugDoingPaint { get; set; } = false;
    internal virtual SemanticsOwner? _semanticsOwner { get; set; } = default;
    internal virtual long _outstandingSemanticsHandles { get; set; } = 0L;
    internal virtual bool _debugDoingSemantics { get; set; } = false;
    internal virtual HashSet<RenderObject> _nodesNeedingSemanticsUpdate { get; private set; } =
        new HashSet<RenderObject>();
    internal virtual HashSet<RenderObject> _nodesNeedingSemanticsGeometryUpdate
    {
        get;
        private set;
    } = new HashSet<RenderObject>();
    internal virtual HashSet<PipelineOwner> _children { get; private set; } =
        new HashSet<PipelineOwner>();
    internal virtual PipelineManifold? _manifold { get; set; } = default;
    internal virtual PipelineOwner? _debugParent { get; set; } = default;

    public PipelineOwner(
        Action? onNeedVisualUpdate = null,
        Action? onSemanticsOwnerCreated = null,
        Action<SemanticsUpdate>? onSemanticsUpdate = null,
        Action? onSemanticsOwnerDisposed = null
    )
    {
        this.onNeedVisualUpdate = onNeedVisualUpdate;
        this.onSemanticsOwnerCreated = onSemanticsOwnerCreated;
        this.onSemanticsUpdate = onSemanticsUpdate;
        this.onSemanticsOwnerDisposed = onSemanticsOwnerDisposed;
    }

    public virtual void requestVisualUpdate()
    {
        if (onNeedVisualUpdate is not null)
        {
            onNeedVisualUpdate!();
        }
        else
        {
            _manifold?.requestVisualUpdate();
        }
    }

    public virtual RenderObject? rootNode
    {
        get => _rootNode;
        set
        {
            var __value = value;
            if (Equals(_rootNode, __value))
            {
                return;
            }
            _rootNode?.detach();
            _rootNode = __value;
            _rootNode?.attach(this);
        }
    }
    public virtual IEnumerable<RenderObject> nodesNeedingLayout => _nodesNeedingLayout;
    public virtual bool debugDoingLayout => _debugDoingLayout;

    public virtual void flushLayout()
    {
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugEnhanceLayoutTimelineArguments)
                {
                    debugTimelineArguments = new DartMap<string, string>
                    {
                        ["dirty count"] = $"{checked((long)_nodesNeedingLayout.Count)}",
                        ["dirty list"] = $"{_nodesNeedingLayout}",
                    };
                }
                return true;
            });
            FlutterTimeline.startSync(
                $"LAYOUT{_debugRootSuffixForTimelineEventNames}",
                arguments: debugTimelineArguments
            );
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDoingLayout = true;
            return true;
        });
        try
        {
            while (checked((long)_nodesNeedingLayout.Count) != 0)
            {
                DartRuntimePrimitives.Assert(() => !_shouldMergeDirtyNodes);
                List<RenderObject> dirtyNodes = _nodesNeedingLayout;
                _nodesNeedingLayout = _nodesNeedingLayoutScratch;
                _nodesNeedingLayoutScratch = dirtyNodes;
                _nodesNeedingLayout.Clear();
                dirtyNodes.sort((a, b) => a.depth - b.depth);
                for (var i = 0L; i < checked(dirtyNodes.Count); i++)
                {
                    if (_shouldMergeDirtyNodes)
                    {
                        _shouldMergeDirtyNodes = false;
                        if (checked((long)_nodesNeedingLayout.Count) != 0)
                        {
                            _nodesNeedingLayout.AddRange(dirtyNodes.Skip(checked((int)i)).ToList());
                            break;
                        }
                    }
                    RenderObject node = dirtyNodes[(int)i];
                    if (node._needsLayout && Equals(node.owner, this))
                    {
                        node._layoutWithoutResize();
                    }
                }
                dirtyNodes.Clear();
                _shouldMergeDirtyNodes = false;
            }
            DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingChildLayout = true;
                return true;
            });
            foreach (PipelineOwner child in _children)
            {
                child.flushLayout();
            }
            DartRuntimePrimitives.Assert(() => checked((long)_nodesNeedingLayout.Count) == 0);
        }
        finally
        {
            _shouldMergeDirtyNodes = false;
            DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingLayout = false;
                _debugDoingChildLayout = false;
                return true;
            });
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    internal virtual void _enableMutationsToDirtySubtrees(Action callback)
    {
        DartRuntimePrimitives.Assert(() => _debugDoingLayout);
        bool? oldState = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            oldState = _debugAllowMutationsToDirtySubtrees;
            _debugAllowMutationsToDirtySubtrees = true;
            return true;
        });
        try
        {
            callback();
        }
        finally
        {
            _shouldMergeDirtyNodes = true;
            DartRuntimePrimitives.Assert(() =>
            {
                _debugAllowMutationsToDirtySubtrees = DartRuntimePrimitives.RequireValue(oldState);
                return true;
            });
        }
    }

    public virtual void flushCompositingBits()
    {
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync(
                $"UPDATING COMPOSITING BITS{_debugRootSuffixForTimelineEventNames}"
            );
        }
        _nodesNeedingCompositingBitsUpdate.sort((a, b) => a.depth - b.depth);
        foreach (RenderObject node in _nodesNeedingCompositingBitsUpdate)
        {
            if (node._needsCompositingBitsUpdate && Equals(node.owner, this))
            {
                node._updateCompositingBits();
            }
        }
        _nodesNeedingCompositingBitsUpdate.Clear();
        foreach (PipelineOwner child in _children)
        {
            child.flushCompositingBits();
        }
        DartRuntimePrimitives.Assert(() =>
            checked((long)_nodesNeedingCompositingBitsUpdate.Count) == 0
        );
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.finishSync();
        }
    }

    public virtual IEnumerable<RenderObject> nodesNeedingPaint => _nodesNeedingPaint;
    public virtual bool debugDoingPaint => _debugDoingPaint;

    public virtual void flushPaint()
    {
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugEnhancePaintTimelineArguments)
                {
                    debugTimelineArguments = new DartMap<string, string>
                    {
                        ["dirty count"] = $"{checked((long)_nodesNeedingPaint.Count)}",
                        ["dirty list"] = $"{_nodesNeedingPaint}",
                    };
                }
                return true;
            });
            FlutterTimeline.startSync(
                $"PAINT{_debugRootSuffixForTimelineEventNames}",
                arguments: debugTimelineArguments
            );
        }
        try
        {
            DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingPaint = true;
                return true;
            });
            List<RenderObject> dirtyNodes = _nodesNeedingPaint;
            _nodesNeedingPaint = _nodesNeedingPaintScratch;
            _nodesNeedingPaintScratch = dirtyNodes;
            _nodesNeedingPaint.Clear();
            foreach (
                var node in (
                    (Func<List<RenderObject>>)(
                        () =>
                        {
                            var __cascade = dirtyNodes;
                            __cascade.sort((a, b) => b.depth - a.depth);
                            return __cascade;
                        }
                    )
                )()
            )
            {
                DartRuntimePrimitives.Assert(() => node._layerHandle.layer is not null);
                if (
                    (node._needsPaint || node._needsCompositedLayerUpdate)
                    && Equals(node.owner, this)
                )
                {
                    if (node._layerHandle.layer!.attached)
                    {
                        DartRuntimePrimitives.Assert(() => node.isRepaintBoundary);
                        if (node._needsPaint)
                        {
                            PaintingContext.repaintCompositedChild(node);
                        }
                        else
                        {
                            PaintingContext.updateLayerProperties(node);
                        }
                    }
                    else
                    {
                        node._skippedPaintingOnLayer();
                    }
                }
            }
            dirtyNodes.Clear();
            foreach (PipelineOwner child in _children)
            {
                child.flushPaint();
            }
            DartRuntimePrimitives.Assert(() => checked((long)_nodesNeedingPaint.Count) == 0);
        }
        finally
        {
            DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingPaint = false;
                return true;
            });
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    public virtual SemanticsOwner? semanticsOwner => _semanticsOwner;
    public virtual long debugOutstandingSemanticsHandles => _outstandingSemanticsHandles;

    public virtual SemanticsHandle ensureSemantics(Action? listener = null)
    {
        _outstandingSemanticsHandles += 1L;
        _updateSemanticsOwner();
        return new _LocalSemanticsHandle__object(this, listener);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateSemanticsOwner()
    {
        if ((_manifold?.semanticsEnabled ?? false) || (_outstandingSemanticsHandles > 0L))
        {
            if (_semanticsOwner is null)
            {
                DartRuntimePrimitives.Assert(() => onSemanticsUpdate is not null);
                _semanticsOwner = new SemanticsOwner(onSemanticsUpdate: onSemanticsUpdate!);
                onSemanticsOwnerCreated?.Invoke();
            }
        }
        else
        {
            if (_semanticsOwner is not null)
            {
                _semanticsOwner?.dispose();
                _semanticsOwner = null;
                onSemanticsOwnerDisposed?.Invoke();
            }
        }
    }

    internal virtual void _didDisposeSemanticsHandle()
    {
        DartRuntimePrimitives.Assert(() => _semanticsOwner is not null);
        _outstandingSemanticsHandles -= 1L;
        _updateSemanticsOwner();
    }

    public virtual void flushSemantics()
    {
        using var allocationProfile = FrameworkWorkProfile.AllocationEnabled
            ? FrameworkWorkProfile.Begin(GetType(), 15)
            : default;
        if (_semanticsOwner is null)
        {
            return;
        }
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync($"SEMANTICS{_debugRootSuffixForTimelineEventNames}");
        }
        DartRuntimePrimitives.Assert(() => _semanticsOwner is not null);
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDoingSemantics = true;
            return true;
        });
        try
        {
            List<RenderObject> nodesToProcess = (
                (Func<List<RenderObject>>)(
                    () =>
                    {
                        var __cascade = _nodesNeedingSemanticsUpdate
                            .where(
                                (@object) => !@object._needsLayout && Equals(@object.owner, this)
                            )
                            .ToList();
                        __cascade.sort((a, b) => a.depth - b.depth);
                        return __cascade;
                    }
                )
            )();
            _nodesNeedingSemanticsUpdate.Clear();
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.updateChildren");
            }
            RenderObject? rootNodeLocal = rootNode;
            foreach (var node in nodesToProcess)
            {
                if (node._semantics.parentDataDirty)
                {
                    continue;
                }
                node._semantics.updateChildren();
            }
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            DartRuntimePrimitives.Assert(() =>
            {
                DartRuntimePrimitives.Assert(() =>
                    (checked((long)nodesToProcess.Count) == 0) || (rootNodeLocal is not null)
                );
                if (rootNodeLocal is not null)
                {
                    _RenderObjectSemantics__object.debugCheckForParentData(rootNodeLocal);
                }
                return true;
            });
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.ensureGeometry");
            }
            List<RenderObject> nodesToProcessGeometry = _nodesNeedingSemanticsGeometryUpdate
                .where(
                    (@object) =>
                        !@object._needsLayout
                        && Equals(@object.owner, this)
                        && !@object._semantics.parentDataDirty
                )
                .ToList();
            _nodesNeedingSemanticsGeometryUpdate.Clear();
            foreach (var nodeLocal in nodesToProcessGeometry)
            {
                if (
                    nodeLocal._semantics.shouldFormSemanticsNode
                    && nodeLocal._semantics.geometryDirty
                )
                {
                    continue;
                }
                if (
                    nodeLocal._semantics.shouldFormSemanticsNode
                    && (nodeLocal._isRelayoutBoundary ?? false)
                )
                {
                    nodeLocal._semantics.geometry = null;
                    continue;
                }
                if (!nodeLocal._semantics.contributesToSemanticsTree)
                {
                    foreach (
                        _RenderObjectSemantics__object child in nodeLocal._semantics.mergeUp.OfType<_RenderObjectSemantics__object>()
                    )
                    {
                        if (child.shouldFormSemanticsNode)
                        {
                            child.geometry = null;
                        }
                        else
                        {
                            foreach (
                                _RenderObjectSemantics__object nodeInSubtree in child._children
                            )
                            {
                                DartRuntimePrimitives.Assert(() =>
                                    nodeInSubtree.shouldFormSemanticsNode
                                );
                                nodeInSubtree.geometry = null;
                            }
                        }
                    }
                    continue;
                }
                foreach (
                    _RenderObjectSemantics__object childLocal in nodeLocal._semantics._children
                )
                {
                    childLocal.geometry = null;
                }
            }
            var treeShapeToken = new object();
            var nodeToEnsureGeometry = new HashSet<_RenderObjectSemantics__object>();
            foreach (var nodeAlternate in nodesToProcessGeometry)
            {
                nodeAlternate._semantics.computeAncestorInfo(treeShapeToken);
                if (nodeAlternate._semantics.firstAncestorNodeWithCleanGeometry is not null)
                {
                    nodeToEnsureGeometry.Add(
                        nodeAlternate._semantics.firstAncestorNodeWithCleanGeometry!
                    );
                }
            }
            foreach (
                _RenderObjectSemantics__object nodeNested in (
                    (Func<List<_RenderObjectSemantics__object>>)(
                        () =>
                        {
                            var __cascade = nodeToEnsureGeometry.ToList();
                            __cascade.sort((a, b) => a.renderObject.depth - b.renderObject.depth);
                            return __cascade;
                        }
                    )
                )()
            )
            {
                nodeNested.ensureGeometry();
            }
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.ensureSemanticsNode");
            }
            foreach (RenderObject nodeCurrent in Enumerable.Reverse(nodesToProcess))
            {
                nodeCurrent._semantics.computeAncestorInfo(treeShapeToken);
                var targets = new List<_RenderObjectSemantics__object>();
                if (nodeCurrent._semantics.geometryDirty)
                {
                    if (nodeCurrent._semantics.firstAncestorNodeWithCleanGeometry is not null)
                    {
                        targets.Add(nodeCurrent._semantics.firstAncestorNodeWithCleanGeometry!);
                    }
                }
                else
                {
                    if (
                        !nodeCurrent._semantics.geometry!.isVisible
                        && !nodeCurrent._semantics.isRoot
                    )
                    {
                        _RenderObjectSemantics__object? parentInSemanticsTreeLocal = nodeCurrent
                            ._semantics
                            .parentInSemanticsTree;
                        if (parentInSemanticsTreeLocal is not null)
                        {
                            if (!parentInSemanticsTreeLocal.geometryDirty)
                            {
                                targets.Add(parentInSemanticsTreeLocal);
                            }
                            else
                            {
                                _RenderObjectSemantics__object? firstAncestorNodeWithCleanGeometryLocal =
                                    parentInSemanticsTreeLocal.firstAncestorNodeWithCleanGeometry;
                                if (firstAncestorNodeWithCleanGeometryLocal is not null)
                                {
                                    targets.Add(firstAncestorNodeWithCleanGeometryLocal);
                                }
                            }
                        }
                    }
                    targets.Add(nodeCurrent._semantics);
                }
                foreach (var target in targets)
                {
                    if (target.parentDataDirty)
                    {
                        continue;
                    }
                    target.ensureSemanticsNode();
                }
            }
            DartRuntimePrimitives.Assert(() =>
            {
                if (rootNodeLocal is not null)
                {
                    _RenderObjectSemantics__object.debugCheckForBuilds(rootNodeLocal._semantics);
                }
                return true;
            });
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            _semanticsOwner!.sendSemanticsUpdate();
            foreach (PipelineOwner childAlternate in _children)
            {
                childAlternate.flushSemantics();
            }
            DartRuntimePrimitives.Assert(() =>
                checked((long)_nodesNeedingSemanticsUpdate.Count) == 0
            );
            DartRuntimePrimitives.Assert(() =>
                checked((long)_nodesNeedingSemanticsGeometryUpdate.Count) == 0
            );
        }
        finally
        {
            DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingSemantics = false;
                return true;
            });
            if (!Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    /// <summary>
    /// Reports retained semantics work without consuming it. Hosts can use this to
    /// coalesce geometry churn while a scroll is active and still force a final flush.
    /// </summary>
    public bool hasPendingSemanticsUpdate =>
        _semanticsOwner is not null
        && (
            _nodesNeedingSemanticsUpdate.Count != 0
            || _nodesNeedingSemanticsGeometryUpdate.Count != 0
            || _children.Any(child => child.hasPendingSemanticsUpdate)
        );

    public override List<DiagnosticsNode> debugDescribeChildren()
    {
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new DiagnosticsProperty<RenderObject>("rootNode", rootNode, defaultValue: null)
        );
    }

    internal virtual bool _debugSetParent(PipelineOwner child, PipelineOwner? parent)
    {
        child._debugParent = parent;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string _debugRootSuffixForTimelineEventNames =>
        (_debugParent is null) ? " (root)" : "";

    public virtual void attach(PipelineManifold manifold)
    {
        DartRuntimePrimitives.Assert(() => _manifold is null);
        _manifold = manifold;
        _manifold!.addListener(_updateSemanticsOwner);
        _updateSemanticsOwner();
        foreach (PipelineOwner child in _children)
        {
            child.attach(manifold);
        }
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => _manifold is not null);
        _manifold!.removeListener(_updateSemanticsOwner);
        _manifold = null;
        foreach (PipelineOwner child in _children)
        {
            child.detach();
        }
    }

    internal virtual bool _debugAllowChildListModifications =>
        !_debugDoingChildLayout && !_debugDoingPaint && !_debugDoingSemantics;

    public virtual void adoptChild(PipelineOwner child)
    {
        DartRuntimePrimitives.Assert(() => child._debugParent is null);
        DartRuntimePrimitives.Assert(() => !_children.Contains(child));
        DartRuntimePrimitives.Assert(() => _debugAllowChildListModifications);
        _children.Add(child);
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            _debugSetParent(child, this);
        }
        if (_manifold is not null)
        {
            child.attach(_manifold!);
        }
    }

    public virtual void dropChild(PipelineOwner child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child._debugParent, this));
        DartRuntimePrimitives.Assert(() => _children.Contains(child));
        DartRuntimePrimitives.Assert(() => _debugAllowChildListModifications);
        _children.Remove(child);
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            _debugSetParent(child, null);
        }
        if (_manifold is not null)
        {
            child.detach();
        }
    }

    public virtual void visitChildren(Action<PipelineOwner> visitor)
    {
        _children.forEach(visitor);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => checked((long)_children.Count) == 0);
        DartRuntimePrimitives.Assert(() => rootNode is null);
        DartRuntimePrimitives.Assert(() => _manifold is null);
        DartRuntimePrimitives.Assert(() => _debugParent is null);
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        _semanticsOwner?.dispose();
        _semanticsOwner = null;
        _nodesNeedingLayout.Clear();
        _nodesNeedingCompositingBitsUpdate.Clear();
        _nodesNeedingPaint.Clear();
        _nodesNeedingSemanticsUpdate.Clear();
    }
}

public delegate void PipelineOwnerVisitor(PipelineOwner child);

public interface PipelineManifold : Listenable
{
    public bool semanticsEnabled { get; }
    public void requestVisualUpdate();
}

public abstract class RenderObject : DiagnosticableTreeMixin, HitTestTarget
{
    internal virtual bool _debugDisposed { get; set; } = false;
    public virtual ParentData? parentData { get; set; } = default;
    internal virtual long _depth { get; set; } = 0L;
    internal virtual RenderObject? _parent { get; set; } = default;
    public virtual object? debugCreator { get; set; } = default;
    internal virtual bool _debugDoingThisResize { get; set; } = false;
    internal virtual bool _debugDoingThisLayout { get; set; } = false;
    internal static RenderObject? _debugActiveLayout = default;
    internal virtual bool? _debugCanParentUseSize { get; set; } = default;
    internal virtual bool _debugMutationsLocked { get; set; } = false;
    internal virtual PipelineOwner? _owner { get; set; } = default;
    internal virtual bool _needsLayout { get; set; } = true;
    internal virtual bool _needsLayoutCallbackRebuild { get; set; } = true;
    internal virtual bool? _isRelayoutBoundary { get; set; } = default;
    internal virtual bool _doingThisLayoutWithCallback { get; set; } = false;
    internal virtual Constraints? _constraints { get; set; } = default;
    public static bool debugCheckingIntrinsics = false;
    internal virtual bool _debugDoingThisPaint { get; set; } = false;
    internal static RenderObject? _debugActivePaint = default;
    internal virtual bool _wasRepaintBoundary { get; set; } = default!;
    public virtual LayerHandle<ContainerLayer> _layerHandle { get; private set; } =
        new LayerHandle<ContainerLayer>();

    // Dart initializes compositing from virtual repaint-boundary getters in
    // RenderObject's constructor. Derived constructor state may not yet be
    // available there in C#: compute initial bits on attach instead.
    internal virtual bool _needsCompositingBitsUpdate { get; set; } = true;
    internal virtual bool _needsCompositing { get; set; } = default!;
    internal virtual bool _needsPaint { get; set; } = true;
    internal virtual bool _needsCompositedLayerUpdate { get; set; } = false;
    private bool __late__semantics_initialized;
    private _RenderObjectSemantics__object __late__semantics = default!;
    internal virtual _RenderObjectSemantics__object _semantics
    {
        get
        {
            if (!__late__semantics_initialized)
            {
                __late__semantics = new _RenderObjectSemantics__object(this);
                __late__semantics_initialized = true;
            }
            return __late__semantics;
        }
    }

    protected RenderObject() { }

    public virtual void reassemble()
    {
        markNeedsLayout();
        markNeedsCompositingBitsUpdate();
        markNeedsPaint();
        markNeedsSemanticsUpdate();
        visitChildren(
            (child) =>
            {
                child.reassemble();
            }
        );
    }

    public virtual bool? debugDisposed
    {
        get
        {
            bool? disposed = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                disposed = _debugDisposed;
                return true;
            });
            return disposed;
        }
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() =>
            Foundation.DebugLibrary.debugMaybeDispatchDisposed(this)
        );
        _layerHandle.layer = null;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDisposed = true;
            return true;
        });
    }

    public virtual void setupParentData(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => _debugCanPerformMutations);
        if (child.parentData is not ParentData)
        {
            child.parentData = new ParentData();
        }
    }

    public virtual long depth => _depth;

    public virtual void redepthChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.owner, owner));
        if (child._depth <= _depth)
        {
            child._depth = _depth + 1L;
            child.redepthChildren();
        }
    }

    public virtual void redepthChildren() { }

    public virtual RenderObject? parent => _parent;

    public virtual void adoptChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => child._parent is null);
        DartRuntimePrimitives.Assert(() =>
        {
            var node = this;
            while (node.parent is not null)
            {
                node = node.parent!;
            }
            DartRuntimePrimitives.Assert(() => !Equals(node, child));
            return true;
        });
        setupParentData(child);
        markNeedsLayout();
        markNeedsCompositingBitsUpdate();
        markNeedsSemanticsUpdate();
        child._parent = this;
        if (attached)
        {
            child.attach(owner!);
        }
        redepthChild(child);
    }

    public virtual void dropChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child._parent, this));
        DartRuntimePrimitives.Assert(() => child.attached == attached);
        DartRuntimePrimitives.Assert(() => child.parentData is not null);
        if (!(child._isRelayoutBoundary ?? true))
        {
            child._isRelayoutBoundary = null;
        }
        child.parentData!.detach();
        child.parentData = null;
        child._parent = null;
        if (attached)
        {
            child.detach();
        }
        markNeedsLayout();
        markNeedsCompositingBitsUpdate();
        markNeedsSemanticsUpdate();
    }

    public virtual void visitChildren(Action<RenderObject> visitor) { }

    internal virtual void _reportException(
        string method,
        object exception,
        System.Diagnostics.StackTrace stack
    )
    {
        FlutterError.reportError(
            new FlutterErrorDetails(
                exception: exception,
                stack: stack,
                library: "rendering library",
                context: new ErrorDescription($"during {method}()"),
                informationCollector: () =>
                    new List<DiagnosticsNode>
                    {
                        describeForError(
                            "The following RenderObject was being processed when the exception was fired"
                        ),
                        describeForError(
                            "RenderObject",
                            style: DiagnosticsTreeStyle.truncateChildren
                        ),
                    }
            )
        );
    }

    public virtual bool debugDoingThisResize => _debugDoingThisResize;
    public virtual bool debugDoingThisLayout => _debugDoingThisLayout;
    public static RenderObject? debugActiveLayout => _debugActiveLayout;

    internal static T _withDebugActiveLayoutCleared<T>(Func<T> inner)
    {
        RenderObject? debugPreviousActiveLayout = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            debugPreviousActiveLayout = _debugActiveLayout;
            _debugActiveLayout = null;
            return true;
        });
        T result = inner();
        DartRuntimePrimitives.Assert(() =>
        {
            _debugActiveLayout = debugPreviousActiveLayout;
            return true;
        });
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual bool debugCanParentUseSize =>
        DartRuntimePrimitives.RequireValue(_debugCanParentUseSize);
    internal virtual (RenderObject, bool)? _debugClosestMutationRoot
    {
        get
        {
            return this switch
            {
                RenderObject { _doingThisLayoutWithCallback: true } __object91304 => (this, true),
                RenderObject
                {
                    owner: PipelineOwner
                    {
                        _debugAllowMutationsToDirtySubtrees: true
                    } __object91571,
                    _needsLayout: true
                } __object91542 => (this, true),
                RenderObject { _debugMutationsLocked: true } __object91746 => (this, false),
                RenderObject __object91812 => debugLayoutParent?._debugClosestMutationRoot,
                _ => throw new InvalidOperationException("Non-exhaustive Dart switch value."),
            };
        }
    }
    internal virtual bool _debugCanPerformMutations
    {
        get
        {
            bool isMutationAllowed = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                if (_debugDisposed)
                {
                    throw new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary("A disposed RenderObject was mutated."),
                            new DiagnosticsProperty<RenderObject>(
                                "The disposed RenderObject was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        }
                    );
                }
                PipelineOwner? ownerLocal = owner;
                if ((ownerLocal is null) || !ownerLocal.debugDoingLayout)
                {
                    isMutationAllowed = true;
                    return true;
                }
                RenderObject? activeLayoutRoot = default!;
                var mutationRoot = _debugClosestMutationRoot;
                activeLayoutRoot = mutationRoot?.Item1;
                isMutationAllowed = mutationRoot?.Item2 ?? false;
                if (isMutationAllowed)
                {
                    return true;
                }
                RenderObject debugActiveLayoutLocal = debugActiveLayout!;
                var culpritMethodName = debugActiveLayoutLocal.debugDoingThisLayout
                    ? "performLayout"
                    : "performResize";
                var culpritFullMethodName =
                    $"{DartRuntimePrimitives.RuntimeType(debugActiveLayoutLocal)}.{culpritMethodName}";
                if (activeLayoutRoot is null)
                {
                    throw new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                $"A {GetType()} was mutated in {culpritFullMethodName}."
                            ),
                            new ErrorDescription(
                                "The RenderObject was mutated when none of its ancestors is actively performing layout."
                            ),
                            new DiagnosticsProperty<RenderObject>(
                                "The RenderObject being mutated was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                            new DiagnosticsProperty<RenderObject>(
                                $"The RenderObject that was mutating the said {GetType()} was",
                                debugActiveLayoutLocal,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                        }
                    );
                }
                if (Equals(activeLayoutRoot, this))
                {
                    throw new FlutterError(
                        new List<DiagnosticsNode>
                        {
                            new ErrorSummary(
                                $"A {GetType()} was mutated in its own {culpritMethodName} implementation."
                            ),
                            new ErrorDescription(
                                "A RenderObject must not re-dirty itself while still being laid out."
                            ),
                            new DiagnosticsProperty<RenderObject>(
                                "The RenderObject being mutated was",
                                this,
                                style: DiagnosticsTreeStyle.errorProperty
                            ),
                            new ErrorHint(
                                "Consider using the LayoutBuilder widget to dynamically change a subtree during layout."
                            ),
                        }
                    );
                }
                var summary = new ErrorSummary(
                    $"A {GetType()} was mutated in {culpritFullMethodName}."
                );
                var isMutatedByAncestor = Equals(activeLayoutRoot, debugActiveLayoutLocal);
                var description = isMutatedByAncestor
                    ? $"A RenderObject must not mutate its descendants in its {culpritMethodName} method."
                    : "A RenderObject must not mutate another RenderObject from a different render subtree "
                        + $"in its {culpritMethodName} method.";
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        summary,
                        new ErrorDescription(description),
                        new DiagnosticsProperty<RenderObject>(
                            "The RenderObject being mutated was",
                            this,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                        new DiagnosticsProperty<RenderObject>(
                            $"The {(isMutatedByAncestor ? "ancestor " : "")}RenderObject that was mutating the said {GetType()} was",
                            debugActiveLayoutLocal,
                            style: DiagnosticsTreeStyle.errorProperty
                        ),
                        new ErrorHint(
                            "Mutating the layout of another RenderObject may cause some RenderObjects in its subtree to be laid out more than once. "
                                + "Consider using the LayoutBuilder widget to dynamically mutate a subtree during layout."
                        ),
                    }
                );
            });
            return isMutationAllowed;
        }
    }
    public virtual RenderObject? debugLayoutParent
    {
        get
        {
            RenderObject? layoutParent = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                layoutParent = parent;
                return true;
            });
            return layoutParent;
        }
    }
    public virtual PipelineOwner? owner => _owner;
    public virtual bool attached => owner is not null;

    public virtual void attach(PipelineOwner owner)
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() => _owner is null);
        _owner = owner;
        if (_needsLayout && (_isRelayoutBoundary is not null))
        {
            _needsLayout = false;
            markNeedsLayout();
        }
        if (_needsCompositingBitsUpdate)
        {
            _needsCompositingBitsUpdate = false;
            markNeedsCompositingBitsUpdate();
        }
        if (_needsPaint && (_layerHandle.layer is not null))
        {
            _needsPaint = false;
            markNeedsPaint();
        }
        if (
            _semantics.configProvider.effective.isSemanticBoundary
            && (_semantics.parentDataDirty || !_semantics.built)
        )
        {
            markNeedsSemanticsUpdate();
        }
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => _owner is not null);
        _owner = null;
        DartRuntimePrimitives.Assert(() => (parent is null) || (attached == parent!.attached));
    }

    public virtual bool debugNeedsLayout
    {
        get
        {
            if (!Foundation.ConstantsLibrary.kDebugMode)
            {
                return false;
            }
            return _needsLayout;
        }
    }
    public virtual bool debugDoingThisLayoutWithCallback => _doingThisLayoutWithCallback;
    public virtual Constraints constraints
    {
        get
        {
            if (_constraints is null)
            {
                throw new InvalidOperationException(
                    "A RenderObject does not have any constraints before it has been laid out."
                );
            }
            return _constraints!;
        }
    }
    public abstract void debugAssertDoesMeetConstraints();

    internal virtual bool _debugRelayoutBoundaryAlreadyMarkedNeedsLayout()
    {
        for (
            RenderObject? node = this;
            (node is not null) && (node._isRelayoutBoundary is not null);
            node = node.parent
        )
        {
            bool alreadyMarkedNeedsLayout = node._needsLayout || node._debugDoingThisLayout;
            if (!alreadyMarkedNeedsLayout)
            {
                return false;
            }
            if (DartRuntimePrimitives.RequireValue(node._isRelayoutBoundary))
            {
                return true;
            }
        }
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void markNeedsLayout()
    {
        FrameworkWorkCounters.Add(FrameworkWork.MarkLayout);
        if (FrameworkWorkTrace.Enabled)
        {
            FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.MarkLayout, this);
        }

        if (_needsLayout)
        {
            FrameworkWorkCounters.Add(FrameworkWork.MarkLayoutAlreadyDirty);
        }

        DartRuntimePrimitives.Assert(() => _debugCanPerformMutations);
        if (_needsLayout)
        {
            DartRuntimePrimitives.Assert(() => _debugRelayoutBoundaryAlreadyMarkedNeedsLayout());
            return;
        }
        _needsLayout = true;
        if (owner is PipelineOwner ownerLocal && (_isRelayoutBoundary ?? false))
        {
            FrameworkWorkCounters.Add(FrameworkWork.LayoutBoundary);
            DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintMarkNeedsLayoutStacks)
                {
                    AssertionsLibrary.debugPrintStack(
                        label: $"markNeedsLayout() called for {this}"
                    );
                }
                return true;
            });
            ownerLocal._nodesNeedingLayout.Add(this);
            ownerLocal.requestVisualUpdate();
        }
        else
        {
            if (parent is not null)
            {
                markParentNeedsLayout();
            }
        }
    }

    public virtual void markParentNeedsLayout()
    {
        DartRuntimePrimitives.Assert(() => _debugCanPerformMutations);
        _needsLayout = true;
        DartRuntimePrimitives.Assert(() => parent is not null);
        RenderObject parentLocal = parent!;
        if (!_doingThisLayoutWithCallback)
        {
            parentLocal.markNeedsLayout();
            FrameworkWorkCounters.Add(FrameworkWork.LayoutParentPropagation);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => parentLocal._debugDoingThisLayout);
        }
        DartRuntimePrimitives.Assert(() => Equals(parentLocal, parent));
    }

    public virtual void markNeedsLayoutForSizedByParentChange()
    {
        markNeedsLayout();
        markParentNeedsLayout();
    }

    public virtual void scheduleInitialLayout()
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() => attached);
        DartRuntimePrimitives.Assert(() => parent is null);
        DartRuntimePrimitives.Assert(() => !owner!._debugDoingLayout);
        DartRuntimePrimitives.Assert(() => _isRelayoutBoundary is null);
        _isRelayoutBoundary = true;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugCanParentUseSize = false;
            return true;
        });
        owner!._nodesNeedingLayout.Add(this);
    }

    internal virtual void _layoutWithoutResize()
    {
        using var layoutProfile = FrameworkWorkProfile.LayoutEnabled
            ? FrameworkWorkProfile.Begin(GetType(), 7)
            : default;
        DartRuntimePrimitives.Assert(() => _needsLayout);
        DartRuntimePrimitives.Assert(() =>
            (_isRelayoutBoundary ?? false) || (this is RenderObjectWithLayoutCallbackMixin)
        );
        RenderObject? debugPreviousActiveLayout = default!;
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !_doingThisLayoutWithCallback);
        DartRuntimePrimitives.Assert(() => _debugCanParentUseSize is not null);
        DartRuntimePrimitives.Assert(() =>
        {
            _debugMutationsLocked = true;
            _debugDoingThisLayout = true;
            debugPreviousActiveLayout = _debugActiveLayout;
            _debugActiveLayout = this;
            if (DebugLibrary.debugPrintLayouts)
            {
                PrintLibrary.debugPrint($"Laying out (without resize) {this}");
            }
            return true;
        });
        try
        {
            if (FrameworkWorkTrace.Enabled)
            {
                FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.PerformLayout, this);
            }

            performLayout();
            markNeedsSemanticsUpdate();
        }
        catch (Exception e)
        {
            var stack = new System.Diagnostics.StackTrace();
            _reportException("performLayout", e, stack);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugActiveLayout = debugPreviousActiveLayout;
            _debugDoingThisLayout = false;
            _debugMutationsLocked = false;
            return true;
        });
        _needsLayout = false;
        markNeedsPaint();
    }

    public virtual void layout(Constraints constraints, bool parentUsesSize = false)
    {
        using var layoutProfile = FrameworkWorkProfile.LayoutEnabled
            ? FrameworkWorkProfile.Begin(GetType(), 7)
            : default;
        FrameworkWorkCounters.Add(FrameworkWork.LayoutEntry);
        if (FrameworkWorkTrace.Enabled)
        {
            var box = constraints as BoxConstraints;
            FrameworkWorkTrace.Record(
                FrameworkWorkTrace.Kind.Layout,
                this,
                owner,
                (parentUsesSize ? 1 : 0) | (_needsLayout ? 2 : 0) | (box is null ? 4 : 0),
                box?.minWidth ?? 0,
                box?.maxWidth ?? 0,
                box?.minHeight ?? 0,
                box?.maxHeight ?? 0
            );
        }
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        if (!Foundation.ConstantsLibrary.kReleaseMode && DebugLibrary.debugProfileLayoutsEnabled)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugEnhanceLayoutTimelineArguments)
                {
                    debugTimelineArguments = toDiagnosticsNode().toTimelineArguments();
                }
                return true;
            });
            FlutterTimeline.startSync($"{GetType()}", arguments: debugTimelineArguments);
        }
        DartRuntimePrimitives.Assert(() =>
            constraints.debugAssertIsValid(
                isAppliedConstraint: true,
                informationCollector: () =>
                {
                    List<string> stack = new System.Diagnostics.StackTrace(true)
                        .ToString()
                        .split("\n");
                    long? targetFrame = default!;
                    Pattern layoutFramePattern = new RegExp(
                        "^#[0-9]+ +Render(?:Object|Box).layout \\("
                    );
                    for (var i = 0L; i < checked(stack.Count); i += 1L)
                    {
                        if (layoutFramePattern.matchAsPrefix(stack[(int)i]) is not null)
                        {
                            targetFrame = i + 1L;
                        }
                        else
                        {
                            if (targetFrame is not null)
                            {
                                long targetFrame__112422__value112715 =
                                    DartRuntimePrimitives.RequireValue(targetFrame);
                                break;
                            }
                        }
                    }
                    if (
                        (targetFrame is not null)
                        && (DartRuntimePrimitives.RequireValue(targetFrame) < checked(stack.Count))
                    )
                    {
                        long targetFrame__112422__value112799 = DartRuntimePrimitives.RequireValue(
                            targetFrame
                        );
                        Pattern targetFramePattern = new RegExp("^#[0-9]+ +(.+)$");
                        Match? targetFrameMatch = targetFramePattern.matchAsPrefix(
                            stack[
                                (int)
                                    DartRuntimePrimitives.RequireValue(
                                        DartRuntimePrimitives.RequireValue(
                                            targetFrame__112422__value112799
                                        )
                                    )
                            ]
                        );
                        string? problemFunction =
                            ((targetFrameMatch is not null) && (targetFrameMatch.groupCount > 0L))
                                ? targetFrameMatch.group(1L)
                                : stack[
                                    (int)
                                        DartRuntimePrimitives.RequireValue(
                                            DartRuntimePrimitives.RequireValue(
                                                targetFrame__112422__value112799
                                            )
                                        )
                                ]
                                    .Trim();
                        return new List<DiagnosticsNode>
                        {
                            new ErrorDescription(
                                $"These invalid constraints were provided to {GetType()}'s layout() "
                                    + "function by the following function, which probably computed the "
                                    + "invalid constraints in question:\n"
                                    + $"  {problemFunction}"
                            ),
                        };
                    }
                    return new List<DiagnosticsNode>();
                }
            )
        );
        DartRuntimePrimitives.Assert(() => !_debugDoingThisResize);
        DartRuntimePrimitives.Assert(() => !_debugDoingThisLayout);
        DartRuntimePrimitives.Assert(() =>
        {
            _debugCanParentUseSize = parentUsesSize;
            return true;
        });
        _isRelayoutBoundary =
            !parentUsesSize || sizedByParent || constraints.isTight || (parent is null);
        if (!_needsLayout && Equals(constraints, _constraints))
        {
            FrameworkWorkCounters.Add(FrameworkWork.LayoutFastPath);
            if (FrameworkWorkTrace.Enabled)
            {
                FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.LayoutFastReturn, this);
            }

            DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingThisResize = sizedByParent;
                _debugDoingThisLayout = !sizedByParent;
                RenderObject? debugPreviousActiveLayout = _debugActiveLayout;
                _debugActiveLayout = this;
                debugResetSize();
                _debugActiveLayout = debugPreviousActiveLayout;
                _debugDoingThisLayout = false;
                _debugDoingThisResize = false;
                return true;
            });
            if (
                !Foundation.ConstantsLibrary.kReleaseMode && DebugLibrary.debugProfileLayoutsEnabled
            )
            {
                FlutterTimeline.finishSync();
            }
            return;
        }
        if (FrameworkWorkCounters.Enabled && _needsLayout && Equals(constraints, _constraints))
        {
            FrameworkWorkCounters.Add(FrameworkWork.LayoutDirtySameConstraints);
        }

        _constraints = constraints;
        FrameworkWorkCounters.Add(FrameworkWork.LayoutWork);
        DartRuntimePrimitives.Assert(() => !_debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !_doingThisLayoutWithCallback);
        DartRuntimePrimitives.Assert(() =>
        {
            _debugMutationsLocked = true;
            if (DebugLibrary.debugPrintLayouts)
            {
                PrintLibrary.debugPrint(
                    $"Laying out ({(sizedByParent ? "with separate resize" : "with resize allowed")}) {this}"
                );
            }
            return true;
        });
        if (sizedByParent)
        {
            DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingThisResize = true;
                return true;
            });
            try
            {
                if (FrameworkWorkTrace.Enabled)
                {
                    FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.PerformResize, this);
                }

                performResize();
                DartRuntimePrimitives.Assert(() =>
                {
                    debugAssertDoesMeetConstraints();
                    return true;
                });
            }
            catch (Exception e)
            {
                var stackLocal = new System.Diagnostics.StackTrace();
                _reportException("performResize", e, stackLocal);
            }
            DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingThisResize = false;
                return true;
            });
        }
        RenderObject? debugPreviousActiveLayoutLocal = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDoingThisLayout = true;
            debugPreviousActiveLayoutLocal = _debugActiveLayout;
            _debugActiveLayout = this;
            return true;
        });
        try
        {
            if (FrameworkWorkTrace.Enabled)
            {
                FrameworkWorkTrace.Record(FrameworkWorkTrace.Kind.PerformLayout, this);
            }

            performLayout();
            markNeedsSemanticsUpdate();
            DartRuntimePrimitives.Assert(() =>
            {
                debugAssertDoesMeetConstraints();
                return true;
            });
        }
        catch (Exception eLocal)
        {
            var stackAlternate = new System.Diagnostics.StackTrace();
            _reportException("performLayout", eLocal, stackAlternate);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            _debugActiveLayout = debugPreviousActiveLayoutLocal;
            _debugDoingThisLayout = false;
            _debugMutationsLocked = false;
            return true;
        });
        _needsLayout = false;
        markNeedsPaint();
        if (!Foundation.ConstantsLibrary.kReleaseMode && DebugLibrary.debugProfileLayoutsEnabled)
        {
            FlutterTimeline.finishSync();
        }
    }

    public virtual void debugResetSize() { }

    public virtual bool sizedByParent => false;
    public abstract void performResize();
    public abstract void performLayout();

    public virtual void invokeLayoutCallback<T>(Action<T> callback)
        where T : Constraints
    {
        DartRuntimePrimitives.Assert(() => _debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => _debugDoingThisLayout);
        DartRuntimePrimitives.Assert(() => !_doingThisLayoutWithCallback);
        _doingThisLayoutWithCallback = true;
        try
        {
            owner!._enableMutationsToDirtySubtrees(() =>
            {
                callback(((T?)constraints)!);
            });
        }
        finally
        {
            _doingThisLayoutWithCallback = false;
        }
    }

    public virtual void runLayoutCallback()
    {
        DartRuntimePrimitives.Assert(() => debugDoingThisLayout);
        invokeLayoutCallback(
            (Constraints _) =>
                (
                    this as IRenderLayoutCallback
                    ?? throw new InvalidOperationException(
                        $"{GetType().FullName} scheduled a layout callback without implementing {nameof(IRenderLayoutCallback)}."
                    )
                ).layoutCallback()
        );
        _needsLayoutCallbackRebuild = false;
    }

    public virtual void scheduleLayoutCallback()
    {
        if (_needsLayoutCallbackRebuild)
        {
            DartRuntimePrimitives.Assert(() => debugNeedsLayout);
            return;
        }
        _needsLayoutCallbackRebuild = true;
        owner?._nodesNeedingLayout.Add(this);
        markNeedsLayout();
    }

    public virtual bool debugDoingThisPaint => _debugDoingThisPaint;
    public static RenderObject? debugActivePaint => _debugActivePaint;
    public virtual bool isRepaintBoundary => false;

    public virtual void debugRegisterRepaintBoundaryPaint(
        bool includedParent = true,
        bool includedChild = false
    ) { }

    public virtual bool alwaysNeedsCompositing => false;

    public virtual OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
    {
        DartRuntimePrimitives.Assert(() => isRepaintBoundary);
        return oldLayer ?? new OffsetLayer();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ContainerLayer? layer
    {
        get
        {
            DartRuntimePrimitives.Assert(() =>
                !isRepaintBoundary
                || (_layerHandle.layer is null)
                || (_layerHandle.layer is OffsetLayer)
            );
            return _layerHandle.layer;
        }
        set
        {
            var newLayer = value;
            DartRuntimePrimitives.Assert(() => !isRepaintBoundary);
            _layerHandle.layer = newLayer;
        }
    }
    public virtual ContainerLayer? debugLayer
    {
        get
        {
            ContainerLayer? result = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                result = _layerHandle.layer;
                return true;
            });
            return result;
        }
    }

    public virtual void markNeedsCompositingBitsUpdate()
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        if (_needsCompositingBitsUpdate)
        {
            return;
        }
        _needsCompositingBitsUpdate = true;
        RenderObject? parentLocal = parent;
        if (parentLocal is not null)
        {
            if (parentLocal._needsCompositingBitsUpdate)
            {
                return;
            }
            if ((!_wasRepaintBoundary || !isRepaintBoundary) && !parentLocal.isRepaintBoundary)
            {
                parentLocal.markNeedsCompositingBitsUpdate();
                return;
            }
        }
        owner?._nodesNeedingCompositingBitsUpdate.Add(this);
    }

    public virtual bool needsCompositing
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !_needsCompositingBitsUpdate);
            return _needsCompositing;
        }
    }

    internal virtual void _updateCompositingBits()
    {
        if (!_needsCompositingBitsUpdate)
        {
            return;
        }
        bool oldNeedsCompositing = _needsCompositing;
        _needsCompositing = false;
        visitChildren(
            (child) =>
            {
                child._updateCompositingBits();
                if (child.needsCompositing)
                {
                    _needsCompositing = true;
                }
            }
        );
        if (isRepaintBoundary || alwaysNeedsCompositing)
        {
            _needsCompositing = true;
        }
        if (!isRepaintBoundary && _wasRepaintBoundary)
        {
            _needsPaint = false;
            _needsCompositedLayerUpdate = false;
            owner?._nodesNeedingPaint.removeWhere((t) => DartRuntimePrimitives.Identical(t, this));
            _needsCompositingBitsUpdate = false;
            markNeedsPaint();
        }
        else
        {
            if (oldNeedsCompositing != _needsCompositing)
            {
                _needsCompositingBitsUpdate = false;
                markNeedsPaint();
            }
            else
            {
                _needsCompositingBitsUpdate = false;
            }
        }
    }

    public virtual bool debugNeedsPaint
    {
        get
        {
            if (!Foundation.ConstantsLibrary.kDebugMode)
            {
                return false;
            }
            return _needsPaint;
        }
    }
    public virtual bool debugNeedsCompositedLayerUpdate
    {
        get
        {
            if (!Foundation.ConstantsLibrary.kDebugMode)
            {
                return false;
            }
            return _needsCompositedLayerUpdate;
        }
    }

    public virtual void markNeedsPaint()
    {
        FrameworkWorkCounters.Add(FrameworkWork.MarkPaint);
        if (_needsPaint)
        {
            FrameworkWorkCounters.Add(FrameworkWork.MarkPaintAlreadyDirty);
        }

        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() => (owner is null) || !owner!.debugDoingPaint);
        if (_needsPaint)
        {
            return;
        }
        _needsPaint = true;
        if (isRepaintBoundary && _wasRepaintBoundary)
        {
            FrameworkWorkCounters.Add(FrameworkWork.PaintBoundary);
            DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugPrintMarkNeedsPaintStacks)
                {
                    AssertionsLibrary.debugPrintStack(label: $"markNeedsPaint() called for {this}");
                }
                return true;
            });
            DartRuntimePrimitives.Assert(() => _layerHandle.layer is OffsetLayer);
            if (owner is not null)
            {
                owner!._nodesNeedingPaint.Add(this);
                owner!.requestVisualUpdate();
            }
        }
        else
        {
            if (parent is not null)
            {
                parent!.markNeedsPaint();
                FrameworkWorkCounters.Add(FrameworkWork.PaintParentPropagation);
            }
            else
            {
                DartRuntimePrimitives.Assert(() =>
                {
                    if (DebugLibrary.debugPrintMarkNeedsPaintStacks)
                    {
                        AssertionsLibrary.debugPrintStack(
                            label: $"markNeedsPaint() called for {this} (root of render tree)"
                        );
                    }
                    return true;
                });
                owner?.requestVisualUpdate();
            }
        }
    }

    public virtual void markNeedsCompositedLayerUpdate()
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() => (owner is null) || !owner!.debugDoingPaint);
        if (_needsCompositedLayerUpdate || _needsPaint)
        {
            return;
        }
        _needsCompositedLayerUpdate = true;
        if (isRepaintBoundary && _wasRepaintBoundary)
        {
            DartRuntimePrimitives.Assert(() => _layerHandle.layer is not null);
            if (owner is not null)
            {
                owner!._nodesNeedingPaint.Add(this);
                owner!.requestVisualUpdate();
            }
        }
        else
        {
            markNeedsPaint();
        }
    }

    internal virtual void _skippedPaintingOnLayer()
    {
        DartRuntimePrimitives.Assert(() => attached);
        DartRuntimePrimitives.Assert(() => isRepaintBoundary);
        DartRuntimePrimitives.Assert(() => _needsPaint || _needsCompositedLayerUpdate);
        DartRuntimePrimitives.Assert(() => _layerHandle.layer is not null);
        DartRuntimePrimitives.Assert(() => !_layerHandle.layer!.attached);
        RenderObject? node = parent;
        while (node is not null)
        {
            if (node.isRepaintBoundary)
            {
                if (node._layerHandle.layer is null)
                {
                    break;
                }
                if (node._layerHandle.layer!.attached)
                {
                    break;
                }
                node._needsPaint = true;
            }
            node = node.parent;
        }
    }

    public virtual void scheduleInitialPaint(ContainerLayer rootLayer)
    {
        DartRuntimePrimitives.Assert(() => rootLayer.attached);
        DartRuntimePrimitives.Assert(() => attached);
        DartRuntimePrimitives.Assert(() => parent is null);
        DartRuntimePrimitives.Assert(() => !owner!._debugDoingPaint);
        DartRuntimePrimitives.Assert(() => isRepaintBoundary);
        DartRuntimePrimitives.Assert(() => _layerHandle.layer is null);
        _layerHandle.layer = rootLayer;
        DartRuntimePrimitives.Assert(() => _needsPaint);
        owner!._nodesNeedingPaint.Add(this);
    }

    public virtual void replaceRootLayer(OffsetLayer rootLayer)
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() => rootLayer.attached);
        DartRuntimePrimitives.Assert(() => attached);
        DartRuntimePrimitives.Assert(() => parent is null);
        DartRuntimePrimitives.Assert(() => !owner!._debugDoingPaint);
        DartRuntimePrimitives.Assert(() => isRepaintBoundary);
        DartRuntimePrimitives.Assert(() => _layerHandle.layer is not null);
        _layerHandle.layer!.detach();
        _layerHandle.layer = rootLayer;
        markNeedsPaint();
    }

    internal virtual void _paintWithContext(PaintingContext context, Offset offset)
    {
        using var allocationProfile = FrameworkWorkProfile.AllocationEnabled
            ? FrameworkWorkProfile.Begin(GetType(), 12)
            : default;
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() =>
        {
            if (_debugDoingThisPaint)
            {
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary("Tried to paint a RenderObject reentrantly."),
                        describeForError(
                            "The following RenderObject was already being painted when it was "
                                + "painted again"
                        ),
                        new ErrorDescription(
                            "Since this typically indicates an infinite recursion, it is "
                                + "disallowed."
                        ),
                    }
                );
            }
            return true;
        });
        if (_needsLayout)
        {
            return;
        }
        if (!Foundation.ConstantsLibrary.kReleaseMode && DebugLibrary.debugProfilePaintsEnabled)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
            {
                if (DebugLibrary.debugEnhancePaintTimelineArguments)
                {
                    debugTimelineArguments = toDiagnosticsNode().toTimelineArguments();
                }
                return true;
            });
            FlutterTimeline.startSync($"{GetType()}", arguments: debugTimelineArguments);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            if (_needsCompositingBitsUpdate)
            {
                RenderObject? parentLocal = parent;
                if (parentLocal is not null)
                {
                    var visitedByParent = false;
                    parentLocal.visitChildren(
                        (child) =>
                        {
                            if (Equals(child, this))
                            {
                                visitedByParent = true;
                            }
                        }
                    );
                    if (!visitedByParent)
                    {
                        throw new FlutterError(
                            new List<DiagnosticsNode>
                            {
                                new ErrorSummary(
                                    "A RenderObject was not visited by the parent's visitChildren "
                                        + "during paint."
                                ),
                                parentLocal.describeForError("The parent was"),
                                describeForError("The child that was not visited was"),
                                new ErrorDescription(
                                    "A RenderObject with children must implement visitChildren and "
                                        + "call the visitor exactly once for each child; it also should not "
                                        + "paint children that were removed with dropChild."
                                ),
                                new ErrorHint(
                                    "This usually indicates an error in the Flutter framework itself."
                                ),
                            }
                        );
                    }
                }
                throw new FlutterError(
                    new List<DiagnosticsNode>
                    {
                        new ErrorSummary(
                            "Tried to paint a RenderObject before its compositing bits were "
                                + "updated."
                        ),
                        describeForError(
                            "The following RenderObject was marked as having dirty compositing "
                                + "bits at the time that it was painted"
                        ),
                        new ErrorDescription(
                            "A RenderObject that still has dirty compositing bits cannot be "
                                + "painted because this indicates that the tree has not yet been "
                                + "properly configured for creating the layer tree."
                        ),
                        new ErrorHint(
                            "This usually indicates an error in the Flutter framework itself."
                        ),
                    }
                );
            }
            return true;
        });
        RenderObject? debugLastActivePaint = default!;
        DartRuntimePrimitives.Assert(() =>
        {
            _debugDoingThisPaint = true;
            debugLastActivePaint = _debugActivePaint;
            _debugActivePaint = this;
            DartRuntimePrimitives.Assert(() =>
                !isRepaintBoundary || (_layerHandle.layer is not null)
            );
            return true;
        });
        _needsPaint = false;
        _needsCompositedLayerUpdate = false;
        _wasRepaintBoundary = isRepaintBoundary;
        try
        {
            paint(context, offset);
            DartRuntimePrimitives.Assert(() => !_needsLayout);
            DartRuntimePrimitives.Assert(() => !_needsPaint);
        }
        catch (Exception e)
        {
            var stack = new System.Diagnostics.StackTrace();
            _reportException("paint", e, stack);
        }
        DartRuntimePrimitives.Assert(() =>
        {
            debugPaint(context, offset);
            _debugActivePaint = debugLastActivePaint;
            _debugDoingThisPaint = false;
            return true;
        });
        if (!Foundation.ConstantsLibrary.kReleaseMode && DebugLibrary.debugProfilePaintsEnabled)
        {
            FlutterTimeline.finishSync();
        }
    }

    public abstract Rect paintBounds { get; }

    public virtual void debugPaint(PaintingContext context, Offset offset) { }

    public virtual void paint(PaintingContext context, Offset offset) { }

    public virtual void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
    }

    public virtual bool paintsChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => Equals(child.parent, this));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Matrix4 getTransformTo(RenderObject? target)
    {
        DartRuntimePrimitives.Assert(() => attached);
        List<RenderObject>? fromPath = default!;
        List<RenderObject>? toPath = default!;
        var @from = this;
        RenderObject to = target ?? owner!.rootNode!;
        while (!DartRuntimePrimitives.Identical(@from, to))
        {
            long fromDepth = @from.depth;
            long toDepth = to.depth;
            if (fromDepth >= toDepth)
            {
                RenderObject fromParent =
                    @from.parent
                    ?? throw new FlutterError(
                        $"{target} and {this} are not in the same render tree."
                    );
                (fromPath ??= new List<RenderObject> { this }).Add(fromParent);
                @from = fromParent;
            }
            if (fromDepth <= toDepth)
            {
                RenderObject toParent =
                    to.parent
                    ?? throw new FlutterError(
                        $"{target} and {this} are not in the same render tree."
                    );
                DartRuntimePrimitives.Assert(() => target is not null);
                (toPath ??= new List<RenderObject> { target! }).Add(toParent);
                to = toParent;
            }
        }
        Matrix4? fromTransform = default!;
        if (fromPath is not null)
        {
            DartRuntimePrimitives.Assert(() => checked(fromPath.Count) > 1L);
            fromTransform = Matrix4.identity();
            long lastIndex =
                (target is null) ? (checked(fromPath.Count) - 2L) : (checked(fromPath.Count) - 1L);
            for (var index = lastIndex; index > 0L; index -= 1L)
            {
                fromPath[(int)index]
                    .applyPaintTransform(fromPath[(int)(index - 1L)], fromTransform);
            }
        }
        if (toPath is null)
        {
            return fromTransform ?? Matrix4.identity();
        }
        DartRuntimePrimitives.Assert(() => checked(toPath.Count) > 1L);
        var toTransform = Matrix4.identity();
        for (long indexLocal = checked(toPath.Count) - 1L; indexLocal > 0L; indexLocal -= 1L)
        {
            toPath[(int)indexLocal]
                .applyPaintTransform(toPath[(int)(indexLocal - 1L)], toTransform);
        }
        if (toTransform.invert() == 0L)
        {
            return Matrix4.zero();
        }
        return (
                (Func<Matrix4?>)(
                    () =>
                    {
                        var __cascade = fromTransform;
                        __cascade.multiply(toTransform);
                        return __cascade;
                    }
                )
            )() ?? toTransform;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Rect? describeApproximatePaintClip(RenderObject child) => null;

    public virtual Rect? describeSemanticsClip(RenderObject? child) => null;

    public virtual void scheduleInitialSemantics()
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() => attached);
        DartRuntimePrimitives.Assert(() => parent is null);
        DartRuntimePrimitives.Assert(() => !owner!._debugDoingSemantics);
        DartRuntimePrimitives.Assert(() => _semantics.parentDataDirty || !_semantics.built);
        DartRuntimePrimitives.Assert(() => owner!._semanticsOwner is not null);
        owner!._nodesNeedingSemanticsUpdate.Add(this);
        owner!._nodesNeedingSemanticsGeometryUpdate.Add(this);
        owner!.requestVisualUpdate();
    }

    public virtual void describeSemanticsConfiguration(SemanticsConfiguration config) { }

    public virtual void sendSemanticsEvent(SemanticsEvent semanticsEvent)
    {
        if (owner!.semanticsOwner is null)
        {
            return;
        }
        SemanticsNode? node = _semantics.cachedSemanticsNode;
        if ((node is not null) && !node.isMergedIntoParent)
        {
            node.sendEvent(semanticsEvent);
        }
        else
        {
            if (parent is not null)
            {
                parent!.sendSemanticsEvent(semanticsEvent);
            }
        }
    }

    public abstract Rect semanticBounds { get; }
    public virtual bool debugNeedsSemanticsUpdate
    {
        get
        {
            if (Foundation.ConstantsLibrary.kReleaseMode)
            {
                return false;
            }
            return _semantics.parentDataDirty;
        }
    }
    public virtual SemanticsNode? debugSemantics
    {
        get
        {
            if (!Foundation.ConstantsLibrary.kReleaseMode && _semantics.built)
            {
                return _semantics.cachedSemanticsNode;
            }
            return null;
        }
    }

    public virtual void clearSemantics()
    {
        _semantics.clear();
        visitChildren(
            (child) =>
            {
                child.clearSemantics();
            }
        );
    }

    public virtual void markNeedsSemanticsUpdate()
    {
        DartRuntimePrimitives.Assert(() => !_debugDisposed);
        DartRuntimePrimitives.Assert(() => !attached || !owner!._debugDoingSemantics);
        if (!attached || (owner!._semanticsOwner is null))
        {
            return;
        }
        _semantics.markNeedsUpdate();
    }

    public virtual void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        visitChildren(visitor);
    }

    public virtual void assembleSemanticsNode(
        SemanticsNode node,
        SemanticsConfiguration config,
        IEnumerable<SemanticsNode> children
    )
    {
        node.updateWith(
            config: config,
            childrenInInversePaintOrder: ((List<SemanticsNode>?)children)!
        );
    }

    public virtual void handleEvent(PointerEvent @event, HitTestEntry<HitTestTarget> entry) { }

    public override string toStringShort()
    {
        string header = DiagnosticsLibrary.describeIdentity(this);
        if (!Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (_debugDisposed)
            {
                header += " DISPOSED";
                return header;
            }
            var count = 0L;
            for (
                RenderObject? node = this;
                (node is not null) && !(node._isRelayoutBoundary ?? false);
                node = node.parent
            )
            {
                if (node._isRelayoutBoundary is null)
                {
                    count = -1L;
                    break;
                }
                count += 1L;
            }
            if (count > 0L)
            {
                header += $" relayoutBoundary=up{count}";
            }
            if (_needsLayout)
            {
                header += " NEEDS-LAYOUT";
            }
            if (_needsPaint)
            {
                header += " NEEDS-PAINT";
            }
            if (_needsCompositingBitsUpdate)
            {
                header += " NEEDS-COMPOSITING-BITS-UPDATE";
            }
            if (!attached)
            {
                header += " DETACHED";
            }
        }
        return header;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => ToString(DiagnosticLevel.info);

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info) =>
        toStringShort();

    public override string toStringDeep(
        string prefixLineOne = "",
        string? prefixOtherLines = "",
        DiagnosticLevel minLevel = DiagnosticLevel.debug,
        long? wrapWidth = 65
    )
    {
        return _withDebugActiveLayoutCleared(() =>
            base.toStringDeep(
                prefixLineOne: prefixLineOne,
                prefixOtherLines: prefixOtherLines,
                minLevel: minLevel,
                wrapWidth: wrapWidth
            )
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string toStringShallow(
        string joiner = ", ",
        DiagnosticLevel minLevel = DiagnosticLevel.debug
    )
    {
        return _withDebugActiveLayoutCleared(() =>
            base.toStringShallow(joiner: joiner, minLevel: minLevel)
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new FlagProperty(
                "needsCompositing",
                value: _needsCompositing,
                ifTrue: "needs compositing"
            )
        );
        properties.add(
            new DiagnosticsProperty<object?>(
                "creator",
                debugCreator,
                defaultValue: null,
                level: DiagnosticLevel.debug
            )
        );
        properties.add(
            new DiagnosticsProperty<ParentData>(
                "parentData",
                parentData,
                tooltip: (_debugCanParentUseSize ?? false) ? "can use size" : null,
                missingIfNull: true
            )
        );
        properties.add(
            new DiagnosticsProperty<Constraints>("constraints", _constraints, missingIfNull: true)
        );
        properties.add(
            new DiagnosticsProperty<ContainerLayer>("layer", _layerHandle.layer, defaultValue: null)
        );
        properties.add(
            new DiagnosticsProperty<SemanticsNode>(
                "semantics node",
                debugSemantics,
                defaultValue: null
            )
        );
        properties.add(
            new FlagProperty(
                "isBlockingSemanticsOfPreviouslyPaintedNodes",
                value: _semantics
                    .configProvider
                    .effective
                    .isBlockingSemanticsOfPreviouslyPaintedNodes,
                ifTrue: "blocks semantics of earlier render objects below the common boundary"
            )
        );
        properties.add(
            new FlagProperty(
                "isSemanticBoundary",
                value: _semantics.configProvider.effective.isSemanticBoundary,
                ifTrue: "semantic boundary"
            )
        );
    }

    public override List<DiagnosticsNode> debugDescribeChildren() => new List<DiagnosticsNode>();

    public virtual void showOnScreen(
        RenderObject? descendant = null,
        Rect? rect = null,
        Duration duration = default,
        Curve curve = default!
    )
    {
        parent?.showOnScreen(
            descendant: descendant ?? this,
            rect: rect,
            duration: duration,
            curve: curve
        );
    }

    public virtual DiagnosticsNode describeForError(
        string name,
        DiagnosticsTreeStyle style = DiagnosticsTreeStyle.shallow
    )
    {
        return toDiagnosticsNode(name: name, style: style);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public interface IRenderObjectWithChild
{
    bool debugValidateChild(RenderObject child);
    RenderObject? child { get; set; }
}

public interface RenderObjectWithChildMixin<ChildType> : IRenderObjectWithChild
    where ChildType : RenderObject
{
    ChildType? _child { get; set; }

    public new bool debugValidateChild(RenderObject child);
    public new ChildType? child { get; set; }
    public void attach(PipelineOwner owner);
    public void detach();
    public void redepthChildren();
    public void visitChildren(Action<RenderObject> visitor);
    public List<DiagnosticsNode> debugDescribeChildren();

    bool IRenderObjectWithChild.debugValidateChild(RenderObject child) => debugValidateChild(child);
    RenderObject? IRenderObjectWithChild.child
    {
        get => child;
        set => child = value is null ? null : (ChildType)value;
    }
}

public abstract class RenderObjectWithLayoutCallbackMixin : RenderObject
{
    internal virtual bool _needsRebuild { get; set; } = true;

    public abstract void layoutCallback();

    public override void runLayoutCallback()
    {
        DartRuntimePrimitives.Assert(() => debugDoingThisLayout);
        invokeLayoutCallback((Constraints _) => layoutCallback());
        _needsRebuild = false;
    }

    public override void scheduleLayoutCallback()
    {
        if (_needsRebuild)
        {
            DartRuntimePrimitives.Assert(() => debugNeedsLayout);
            return;
        }
        _needsRebuild = true;
        owner?._nodesNeedingLayout.Add(this);
        base.markNeedsLayout();
    }
}

public interface ContainerParentDataMixin<ChildType>
    where ChildType : RenderObject
{
    ChildType? previousSibling { get; set; }
    ChildType? nextSibling { get; set; }

    public void detach();
}

public interface IContainerRenderObject
{
    bool debugValidateChild(RenderObject child);
    void insert(RenderObject child, RenderObject? after = null);
    void move(RenderObject child, RenderObject? after = null);
    void remove(RenderObject child);
}

public interface ContainerRenderObjectMixin<ChildType, ParentDataType> : IContainerRenderObject
    where ChildType : RenderObject
    where ParentDataType : ContainerParentDataMixin<ChildType>
{
    long _childCount { get; set; }
    ChildType? _firstChild { get; set; }
    ChildType? _lastChild { get; set; }

    public bool _debugUltimatePreviousSiblingOf(ChildType child, ChildType? equals = null);
    public bool _debugUltimateNextSiblingOf(ChildType child, ChildType? equals = null);
    public long childCount { get; }
    public new bool debugValidateChild(RenderObject child);
    public void _insertIntoChildList(ChildType child, ChildType? after = null);
    public void insert(ChildType child, ChildType? after = null);
    public void add(ChildType child);
    public void addAll(List<ChildType>? children);
    public void _removeFromChildList(ChildType child);
    public void remove(ChildType child);
    public void removeAll();
    public void move(ChildType child, ChildType? after = null);
    public void attach(PipelineOwner owner);
    public void detach();
    public void redepthChildren();
    public void visitChildren(Action<RenderObject> visitor);
    public ChildType? firstChild { get; }
    public ChildType? lastChild { get; }
    public ChildType? childBefore(ChildType child);
    public ChildType? childAfter(ChildType child);
    public List<DiagnosticsNode> debugDescribeChildren();

    bool IContainerRenderObject.debugValidateChild(RenderObject child) => debugValidateChild(child);
    void IContainerRenderObject.insert(RenderObject child, RenderObject? after) =>
        insert((ChildType)child, (ChildType?)after);
    void IContainerRenderObject.move(RenderObject child, RenderObject? after) =>
        move((ChildType)child, (ChildType?)after);
    void IContainerRenderObject.remove(RenderObject child) => remove((ChildType)child);
}

public interface RelayoutWhenSystemFontsChangeMixin
{
    bool _hasPendingSystemFontsDidChangeCallBack { get; set; }

    public void systemFontsDidChange();
    public void _scheduleSystemFontsUpdate();
    public void attach(PipelineOwner owner);
    public void detach();
}

public interface SemanticsAnnotationsMixin
{
    SemanticsProperties _properties { get; set; }
    bool _container { get; set; }
    bool _explicitChildNodes { get; set; }
    bool _excludeSemantics { get; set; }
    bool _blockUserActions { get; set; }
    Locale? _localeForSubtree { get; set; }
    AttributedString? _attributedLabel { get; set; }
    AttributedString? _attributedValue { get; set; }
    AttributedString? _attributedIncreasedValue { get; set; }
    AttributedString? _attributedDecreasedValue { get; set; }
    AttributedString? _attributedHint { get; set; }
    TextDirection? _textDirection { get; set; }

    public void initSemanticsAnnotations(
        SemanticsProperties properties,
        bool container,
        bool explicitChildNodes,
        bool excludeSemantics,
        bool blockUserActions,
        Locale? localeForSubtree,
        TextDirection? textDirection
    );
    public SemanticsProperties properties { get; set; }
    public bool container { get; set; }
    public bool explicitChildNodes { get; set; }
    public bool excludeSemantics { get; set; }
    public bool blockUserActions { get; set; }
    public Locale? localeForSubtree { get; set; }
    public void _updateAttributedFields(SemanticsProperties value);
    public AttributedString? _effectiveAttributedLabel(SemanticsProperties value);
    public AttributedString? _effectiveAttributedValue(SemanticsProperties value);
    public AttributedString? _effectiveAttributedIncreasedValue(SemanticsProperties value);
    public AttributedString? _effectiveAttributedDecreasedValue(SemanticsProperties value);
    public AttributedString? _effectiveAttributedHint(SemanticsProperties value);
    public TextDirection? textDirection { get; set; }
    public void visitChildrenForSemantics(Action<RenderObject> visitor);
    public void describeSemanticsConfiguration(SemanticsConfiguration config);
    public void _performTap();
    public void _performLongPress();
    public void _performDismiss();
    public void _performScrollLeft();
    public void _performScrollRight();
    public void _performScrollUp();
    public void _performScrollDown();
    public void _performIncrease();
    public void _performDecrease();
    public void _performCopy();
    public void _performCut();
    public void _performPaste();
    public void _performMoveCursorForwardByCharacter(bool extendSelection);
    public void _performMoveCursorBackwardByCharacter(bool extendSelection);
    public void _performMoveCursorForwardByWord(bool extendSelection);
    public void _performMoveCursorBackwardByWord(bool extendSelection);
    public void _performSetSelection(TextSelection selection);
    public void _performSetText(string text);
    public void _performDidGainAccessibilityFocus();
    public void _performDidLoseAccessibilityFocus();
    public void _performFocus();
    public void _performExpand();
    public void _performCollapse();
}

public class _SemanticsParentData__object
{
    public virtual bool mergeIntoParent { get; private set; } = default!;
    public virtual bool blocksUserActions { get; private set; } = default!;
    public virtual AccessibilityFocusBlockType? accessibilityFocusBlockType { get; private set; }
    public virtual bool explicitChildNodes { get; private set; } = default!;
    public virtual HashSet<SemanticsTag>? tagsForChildren { get; private set; }
    public virtual Locale? localeForChildren { get; private set; }

    internal _SemanticsParentData__object(
        bool mergeIntoParent,
        bool blocksUserActions,
        bool explicitChildNodes,
        HashSet<SemanticsTag>? tagsForChildren,
        Locale? localeForChildren,
        AccessibilityFocusBlockType? accessibilityFocusBlockType
    )
    {
        this.mergeIntoParent = mergeIntoParent;
        this.blocksUserActions = blocksUserActions;
        this.explicitChildNodes = explicitChildNodes;
        this.tagsForChildren = tagsForChildren;
        this.localeForChildren = localeForChildren;
        this.accessibilityFocusBlockType = accessibilityFocusBlockType;
    }

    public override bool Equals(object? other)
    {
        var __other = other as _SemanticsParentData__object;
        if (__other is null)
        {
            return false;
        }

        return (__other is _SemanticsParentData__object)
            && (__other.mergeIntoParent == mergeIntoParent)
            && (__other.blocksUserActions == blocksUserActions)
            && (__other.explicitChildNodes == explicitChildNodes)
            && Equals(__other.localeForChildren, localeForChildren)
            && Equals(__other.accessibilityFocusBlockType, accessibilityFocusBlockType)
            && CollectionsLibrary.setEquals(__other.tagsForChildren, tagsForChildren);
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(
            mergeIntoParent,
            blocksUserActions,
            explicitChildNodes,
            localeForChildren,
            accessibilityFocusBlockType,
            Dart_coreLibrary.hashAllUnordered(tagsForChildren ?? new HashSet<SemanticsTag>())
        );
    }
}

public class _SemanticsConfigurationProvider__object
{
    internal virtual RenderObject _renderObject { get; private set; } = default!;
    internal virtual bool _isEffectiveConfigWritable { get; set; } = false;
    internal virtual SemanticsConfiguration? _originalConfiguration { get; set; } = default;
    internal virtual SemanticsConfiguration? _effectiveConfiguration { get; set; } = default;

    internal _SemanticsConfigurationProvider__object(RenderObject _renderObject)
    {
        this._renderObject = _renderObject;
    }

    public virtual bool wasSemanticsBoundary => _originalConfiguration?.isSemanticBoundary ?? false;
    public virtual SemanticsConfiguration effective
    {
        get { return _effectiveConfiguration ?? original; }
    }
    public virtual SemanticsConfiguration original
    {
        get
        {
            if (_originalConfiguration is null)
            {
                _effectiveConfiguration = _originalConfiguration = new SemanticsConfiguration();
                _renderObject.describeSemanticsConfiguration(_originalConfiguration!);
                DartRuntimePrimitives.Assert(() =>
                    !_originalConfiguration!.explicitChildNodes
                    || (_originalConfiguration!.childConfigurationsDelegate is null)
                );
            }
            return _originalConfiguration!;
        }
    }

    public virtual void updateConfig(Action<SemanticsConfiguration> callback)
    {
        if (!_isEffectiveConfigWritable)
        {
            _effectiveConfiguration = original.copy();
            _isEffectiveConfigWritable = true;
        }
        callback(_effectiveConfiguration!);
    }

    public virtual void absorbAll(IEnumerable<SemanticsConfiguration> configs)
    {
        updateConfig(
            (config) =>
            {
                configs.forEach(config.absorb);
            }
        );
    }

    public virtual void reset()
    {
        _effectiveConfiguration = original;
        _isEffectiveConfigWritable = false;
    }

    public virtual void clear()
    {
        _isEffectiveConfigWritable = false;
        _effectiveConfiguration = null;
        _originalConfiguration = null;
    }
}

public abstract class _SemanticsFragment__object
{
    public virtual bool mergesToSibling { get; set; } = false;

    public abstract SemanticsConfiguration? configToMergeUp { get; }
    public abstract _RenderObjectSemantics__object owner { get; }
    public abstract void markSiblingConfigurationConflict(bool conflict);
}

internal class _IncompleteSemanticsFragment__object : _SemanticsFragment__object
{
    private SemanticsConfiguration? __field_configToMergeUp = default!;
    public override SemanticsConfiguration? configToMergeUp
    {
        get => __field_configToMergeUp;
    }
    private _RenderObjectSemantics__object __field_owner = default!;
    public override _RenderObjectSemantics__object owner
    {
        get => __field_owner;
    }

    internal _IncompleteSemanticsFragment__object(
        SemanticsConfiguration configToMergeUp,
        _RenderObjectSemantics__object owner
    )
    {
        __field_configToMergeUp = configToMergeUp;
        __field_owner = owner;
    }

    public override void markSiblingConfigurationConflict(bool conflict)
    {
        DartRuntimePrimitives.Assert(() => !conflict);
    }
}

internal delegate void _MergeUpAndSiblingMergeGroups__object();

public class _RenderObjectSemantics__object : _SemanticsFragment__object, DiagnosticableTree
{
    public virtual RenderObject renderObject { get; private set; } = default!;
    internal virtual bool _hasSiblingConflict { get; set; } = false;
    internal virtual bool? _blocksPreviousSibling { get; set; } = default;
    internal virtual bool _containsIncompleteFragment { get; set; } = false;
    public virtual bool built { get; set; } = false;
    public virtual SemanticsNode? cachedSemanticsNode { get; set; } = default;
    public virtual List<SemanticsNode> semanticsNodes { get; private set; } =
        new List<SemanticsNode>();
    public virtual List<_SemanticsFragment__object> mergeUp { get; private set; } =
        new List<_SemanticsFragment__object>();
    internal virtual List<_RenderObjectSemantics__object> _children { get; private set; } =
        new List<_RenderObjectSemantics__object>();
    public virtual List<List<_SemanticsFragment__object>> siblingMergeGroups { get; private set; } =
        new List<List<_SemanticsFragment__object>>();
    internal virtual DartMap<
        SemanticsNode,
        List<_SemanticsFragment__object>
    > _producedSiblingNodesAndOwners { get; private set; } =
        new DartMap<SemanticsNode, List<_SemanticsFragment__object>>();
    public virtual _SemanticsParentData__object? parentData { get; set; } = default;
    public virtual _SemanticsGeometry__object? geometry { get; set; } = default;
    public virtual _SemanticsConfigurationProvider__object configProvider { get; private set; } =
        default!;
    public virtual _RenderObjectSemantics__object? parentInSemanticsTree { get; set; } = default;
    internal virtual object _currentTreeShapeToken { get; set; } = new object();
    public virtual _RenderObjectSemantics__object? firstAncestorNodeWithCleanGeometry { get; set; } =
        default;

    internal _RenderObjectSemantics__object(RenderObject renderObject)
    {
        this.renderObject = renderObject;
        configProvider = new _SemanticsConfigurationProvider__object(renderObject);
    }

    public override _RenderObjectSemantics__object owner => this;
    public virtual _RenderObjectSemantics__object? parent => renderObject.parent?._semantics;
    public virtual bool parentDataDirty
    {
        get
        {
            if (isRoot)
            {
                return false;
            }
            return parentData is null;
        }
    }
    public virtual bool geometryDirty
    {
        get
        {
            if (isRoot)
            {
                return false;
            }
            return geometry is null;
        }
    }

    public virtual void computeAncestorInfo(object treeShapeToken)
    {
        if (Equals(treeShapeToken, _currentTreeShapeToken))
        {
            return;
        }
        _currentTreeShapeToken = treeShapeToken;
        if (isRoot)
        {
            firstAncestorNodeWithCleanGeometry = this;
            return;
        }
        firstAncestorNodeWithCleanGeometry = null;
        if (parentDataDirty)
        {
            return;
        }
        _RenderObjectSemantics__object? next = default!;
        if (shouldFormSemanticsNode)
        {
            if (!geometryDirty)
            {
                firstAncestorNodeWithCleanGeometry = this;
            }
            next = parentInSemanticsTree;
        }
        else
        {
            next = this;
            while (!next!.parentDataDirty && !next.shouldFormSemanticsNode)
            {
                next = next.parent;
                DartRuntimePrimitives.Assert(() => next is not null);
            }
        }
        if (next is null)
        {
            return;
        }
        if (firstAncestorNodeWithCleanGeometry is null)
        {
            next.computeAncestorInfo(treeShapeToken);
            firstAncestorNodeWithCleanGeometry = next.firstAncestorNodeWithCleanGeometry;
        }
    }

    public override SemanticsConfiguration? configToMergeUp =>
        shouldFormSemanticsNode ? null : configProvider.effective;
    public virtual bool contributesToSemanticsTree
    {
        get
        {
            return configProvider.effective.hasBeenAnnotated
                || _containsIncompleteFragment
                || configProvider.effective.isSemanticBoundary
                || isRoot;
        }
    }
    public virtual bool isRoot => parent is null;
    internal virtual bool _needsMergingSiblingNodesIntoSelf
    {
        get
        {
            return configProvider.effective.isMergingSemanticsOfDescendants
                && (checked((long)_producedSiblingNodesAndOwners.Count) != 0);
        }
    }
    public virtual bool shouldFormSemanticsNode
    {
        get
        {
            if (configProvider.effective.isSemanticBoundary)
            {
                return true;
            }
            if (isRoot)
            {
                return true;
            }
            if (!contributesToSemanticsTree)
            {
                return false;
            }
            DartRuntimePrimitives.Assert(() => parentData is not null);
            return parentData!.explicitChildNodes || _hasSiblingConflict;
        }
    }

    public static void debugCheckForParentData(RenderObject root)
    {
        void debugCheckParentDataNotDirty(_RenderObjectSemantics__object semantics)
        {
            DartRuntimePrimitives.Assert(() => !semantics.parentDataDirty);
            semantics._getNonBlockedChildren().forEach(debugCheckParentDataNotDirty);
        }
        debugCheckParentDataNotDirty(root._semantics);
    }

    public static void debugCheckForBuilds(_RenderObjectSemantics__object node)
    {
        DartRuntimePrimitives.Assert(() => node.built);
        node._children.forEach(debugCheckForBuilds);
    }

    public virtual bool isBlockingPreviousSibling
    {
        get
        {
            if (_blocksPreviousSibling is not null)
            {
                return DartRuntimePrimitives.RequireValue(_blocksPreviousSibling);
            }
            _blocksPreviousSibling = configProvider
                .effective
                .isBlockingSemanticsOfPreviouslyPaintedNodes;
            if (DartRuntimePrimitives.RequireValue(_blocksPreviousSibling))
            {
                return true;
            }
            if (configProvider.effective.isSemanticBoundary)
            {
                return false;
            }
            renderObject.visitChildrenForSemantics(
                (child) =>
                {
                    _RenderObjectSemantics__object childSemantics = child._semantics;
                    if (childSemantics.isBlockingPreviousSibling)
                    {
                        _blocksPreviousSibling = true;
                    }
                }
            );
            return DartRuntimePrimitives.RequireValue(_blocksPreviousSibling);
        }
    }

    public static bool shouldDrop(SemanticsNode node) => node.isInvisible;

    public virtual void markNeedsBuild()
    {
        built = false;
        if (!parentDataDirty && !shouldFormSemanticsNode)
        {
            return;
        }
        foreach (List<_SemanticsFragment__object> @group in siblingMergeGroups)
        {
            foreach (
                _RenderObjectSemantics__object semantics in @group.OfType<_RenderObjectSemantics__object>()
            )
            {
                if (semantics.parentDataDirty)
                {
                    continue;
                }
                if (!semantics.shouldFormSemanticsNode)
                {
                    semantics.markNeedsBuild();
                }
            }
        }
    }

    public virtual void updateChildren()
    {
        DartRuntimePrimitives.Assert(() => (parentData is not null) || isRoot);
        configProvider.reset();
        HashSet<SemanticsTag>? tagsForChildrenLocal = _getTagsForChildren();
        bool explicitChildNodesForChildren =
            isRoot
            || configProvider.effective.explicitChildNodes
            || (!contributesToSemanticsTree && (parentData?.explicitChildNodes ?? true));
        bool blocksUserAction =
            (parentData?.blocksUserActions ?? false)
            || configProvider.effective.isBlockingUserActions;
        AccessibilityFocusBlockType accessibilityFocusBlockTypeLocal = default!;
        if (
            Equals(
                parentData?.accessibilityFocusBlockType,
                AccessibilityFocusBlockType.blockSubtree
            )
        )
        {
            accessibilityFocusBlockTypeLocal = AccessibilityFocusBlockType.blockSubtree;
        }
        else
        {
            accessibilityFocusBlockTypeLocal = configProvider.effective.accessibilityFocusBlockType;
        }
        Locale? localeForChildrenLocal =
            configProvider.effective.localeForSubtree ?? parentData?.localeForChildren;
        siblingMergeGroups.Clear();
        mergeUp.Clear();
        var childParentData = new _SemanticsParentData__object(
            mergeIntoParent: (parentData?.mergeIntoParent ?? false)
                || configProvider.effective.isMergingSemanticsOfDescendants,
            blocksUserActions: blocksUserAction,
            accessibilityFocusBlockType: accessibilityFocusBlockTypeLocal,
            localeForChildren: localeForChildrenLocal,
            explicitChildNodes: explicitChildNodesForChildren,
            tagsForChildren: tagsForChildrenLocal
        );
        (List<_SemanticsFragment__object>, List<List<_SemanticsFragment__object>>) result =
            _collectChildMergeUpAndSiblingGroup(childParentData);
        mergeUp.AddRange(result.Item1);
        siblingMergeGroups.AddRange(result.Item2);
        HashSet<_RenderObjectSemantics__object> oldChildren = _children.toSet();
        _children.Clear();
        if (!contributesToSemanticsTree)
        {
            return;
        }
        _marksConflictsInMergeGroup(mergeUp, isMergeUp: true);
        siblingMergeGroups.forEach(__fragments => _marksConflictsInMergeGroup(__fragments));
        IEnumerable<SemanticsConfiguration> mergeUpConfigs = mergeUp
            .map((fragment) => fragment.configToMergeUp)
            .OfType<SemanticsConfiguration>();
        configProvider.absorbAll(mergeUpConfigs);
        mergeUp.Clear();
        mergeUp.Add(this);
        foreach (
            _RenderObjectSemantics__object childSemantics in result.Item1.OfType<_RenderObjectSemantics__object>()
        )
        {
            DartRuntimePrimitives.Assert(() => childSemantics.contributesToSemanticsTree);
            if (childSemantics.shouldFormSemanticsNode)
            {
                foreach (_RenderObjectSemantics__object child in childSemantics._children)
                {
                    child.parentInSemanticsTree = childSemantics;
                }
                if (childSemantics.geometryDirty)
                {
                    renderObject.owner!._nodesNeedingSemanticsGeometryUpdate.Add(
                        childSemantics.renderObject
                    );
                }
                _children.Add(childSemantics);
            }
            else
            {
                _children.AddRange(childSemantics._children);
                siblingMergeGroups.AddRange(childSemantics.siblingMergeGroups);
            }
        }
        if (isRoot || configProvider.effective.isSemanticBoundary)
        {
            foreach (_RenderObjectSemantics__object childLocal in _children)
            {
                childLocal.parentInSemanticsTree = this;
            }
        }
        oldChildren.removeAll(_children);
        foreach (var removedChild in oldChildren)
        {
            if (Equals(removedChild.parentInSemanticsTree, this))
            {
                removedChild.parentInSemanticsTree = null;
            }
        }
        HashSet<SemanticsTag>? tags = parentData?.tagsForChildren;
        if (tags is not null)
        {
            DartRuntimePrimitives.Assert(() => checked((long)tags.Count) != 0);
            configProvider.updateConfig(
                (config) =>
                {
                    tags.forEach(config.addTagForChildren);
                }
            );
        }
        if (
            !Equals(
                accessibilityFocusBlockTypeLocal,
                configProvider.effective.accessibilityFocusBlockType
            )
        )
        {
            configProvider.updateConfig(
                (config) =>
                {
                    config.accessibilityFocusBlockType = accessibilityFocusBlockTypeLocal;
                }
            );
        }
        if (blocksUserAction != configProvider.effective.isBlockingUserActions)
        {
            configProvider.updateConfig(
                (config) =>
                {
                    config.isBlockingUserActions = blocksUserAction;
                }
            );
        }
        if (!Equals(localeForChildrenLocal, configProvider.effective.locale))
        {
            configProvider.updateConfig(
                (config) =>
                {
                    config.locale = localeForChildrenLocal;
                }
            );
        }
        if (!Equals(accessibilityFocusBlockTypeLocal, AccessibilityFocusBlockType.none))
        {
            configProvider.updateConfig(
                (config) =>
                {
                    config.isFocused = null;
                }
            );
        }
    }

    internal virtual List<_RenderObjectSemantics__object> _getNonBlockedChildren()
    {
        var result = new List<_RenderObjectSemantics__object>();
        renderObject.visitChildrenForSemantics(
            (renderChild) =>
            {
                if (renderChild._semantics.isBlockingPreviousSibling)
                {
                    result.Clear();
                }
                result.Add(renderChild._semantics);
            }
        );
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<SemanticsTag>? _getTagsForChildren()
    {
        if (contributesToSemanticsTree)
        {
            return configProvider.original.tagsForChildren?.toSet();
        }
        HashSet<SemanticsTag>? result = default!;
        if (configProvider.original.tagsForChildren is not null)
        {
            result = configProvider.original.tagsForChildren!.toSet();
        }
        if (parentData?.tagsForChildren is not null)
        {
            if (result is null)
            {
                result = parentData!.tagsForChildren;
            }
            else
            {
                result.UnionWith(parentData!.tagsForChildren!);
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (
        List<_SemanticsFragment__object>,
        List<List<_SemanticsFragment__object>>
    ) _collectChildMergeUpAndSiblingGroup(_SemanticsParentData__object childParentData)
    {
        var mergeUpLocal = new List<_SemanticsFragment__object>();
        var siblingMergeGroupsLocal = new List<List<_SemanticsFragment__object>>();
        var childConfigurations = new List<SemanticsConfiguration>();
        Func<
            List<SemanticsConfiguration>,
            ChildSemanticsConfigurationsResult
        >? childConfigurationsDelegateLocal = configProvider.effective.childConfigurationsDelegate;
        var hasChildConfigurationsDelegate = childConfigurationsDelegateLocal is not null;
        var configToFragment = new DartMap<SemanticsConfiguration, _SemanticsFragment__object>();
        bool needsToMakeIncompleteFragmentAssumption =
            hasChildConfigurationsDelegate && childParentData.explicitChildNodes;
        _SemanticsParentData__object effectiveChildParentData = default!;
        if (needsToMakeIncompleteFragmentAssumption)
        {
            effectiveChildParentData = new _SemanticsParentData__object(
                mergeIntoParent: childParentData.mergeIntoParent,
                blocksUserActions: childParentData.blocksUserActions,
                accessibilityFocusBlockType: childParentData.accessibilityFocusBlockType,
                explicitChildNodes: false,
                tagsForChildren: childParentData.tagsForChildren,
                localeForChildren: childParentData.localeForChildren
            );
        }
        else
        {
            effectiveChildParentData = childParentData;
        }
        foreach (_RenderObjectSemantics__object childSemantics in _getNonBlockedChildren())
        {
            DartRuntimePrimitives.Assert(() => !childSemantics.renderObject._needsLayout);
            childSemantics._didUpdateParentData(effectiveChildParentData);
            foreach (_SemanticsFragment__object fragment in childSemantics.mergeUp)
            {
                if (hasChildConfigurationsDelegate && (fragment.configToMergeUp is not null))
                {
                    childConfigurations.Add(fragment.configToMergeUp!);
                    configToFragment[fragment.configToMergeUp!] = fragment;
                }
                else
                {
                    mergeUpLocal.Add(fragment);
                }
            }
            if (!childSemantics.contributesToSemanticsTree)
            {
                siblingMergeGroupsLocal.AddRange(childSemantics.siblingMergeGroups);
            }
        }
        _containsIncompleteFragment = false;
        DartRuntimePrimitives.Assert(() =>
            (childConfigurationsDelegateLocal is not null)
            || (checked((long)configToFragment.Count) == 0)
        );
        if (childConfigurationsDelegateLocal is not null)
        {
            ChildSemanticsConfigurationsResult result = childConfigurationsDelegateLocal(
                childConfigurations
            );
            mergeUpLocal.AddRange(
                result.mergeUp.map(
                    (config) =>
                    {
                        _SemanticsFragment__object? fragmentLocal =
                            configToFragment.GetValueOrDefault(config);
                        if (fragmentLocal is not null)
                        {
                            return fragmentLocal;
                        }
                        _containsIncompleteFragment = true;
                        return new _IncompleteSemanticsFragment__object(config, this);
                    }
                )
            );
            foreach (IEnumerable<SemanticsConfiguration> @group in result.siblingMergeGroups)
            {
                siblingMergeGroupsLocal.Add(
                    @group
                        .map(
                            (config) =>
                            {
                                _SemanticsFragment__object? fragmentAlternate =
                                    configToFragment.GetValueOrDefault(config);
                                if (fragmentAlternate is not null)
                                {
                                    return fragmentAlternate;
                                }
                                _containsIncompleteFragment = true;
                                return new _IncompleteSemanticsFragment__object(config, this);
                            }
                        )
                        .ToList()
                );
            }
        }
        if (!_containsIncompleteFragment && needsToMakeIncompleteFragmentAssumption)
        {
            mergeUpLocal.Clear();
            siblingMergeGroupsLocal.Clear();
            foreach (_RenderObjectSemantics__object childSemanticsLocal in _getNonBlockedChildren())
            {
                DartRuntimePrimitives.Assert(() => childParentData.explicitChildNodes);
                childSemanticsLocal._didUpdateParentData(childParentData);
                mergeUpLocal.AddRange(childSemanticsLocal.mergeUp);
                if (!childSemanticsLocal.contributesToSemanticsTree)
                {
                    siblingMergeGroupsLocal.AddRange(childSemanticsLocal.siblingMergeGroups);
                }
            }
        }
        return (mergeUpLocal, siblingMergeGroupsLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _didUpdateParentData(_SemanticsParentData__object newParentData)
    {
        if (Equals(parentData, newParentData))
        {
            return;
        }
        markNeedsBuild();
        parentData = newParentData;
        updateChildren();
    }

    public override void markSiblingConfigurationConflict(bool conflict)
    {
        _hasSiblingConflict = conflict;
    }

    public virtual void ensureGeometry()
    {
        DartRuntimePrimitives.Assert(() => !geometryDirty);
        if (isRoot)
        {
            if (!Equals(geometry?.rect, renderObject.semanticBounds))
            {
                markNeedsBuild();
            }
            geometry = _SemanticsGeometry__object.CreateRoot(renderObject.semanticBounds);
        }
        _updateChildGeometry(onlyDirtyChildren: true);
    }

    internal virtual void _updateChildGeometry(bool onlyDirtyChildren = false)
    {
        DartRuntimePrimitives.Assert(() => geometry is not null);
        _SemanticsGeometry__object parentGeometry = geometry!;
        foreach (_RenderObjectSemantics__object childLocal in _children)
        {
            if (childLocal.renderObject is RenderBox childBox && !childBox.hasSize)
            {
                continue;
            }
            if (onlyDirtyChildren && !childLocal.geometryDirty)
            {
                continue;
            }
            _SemanticsGeometry__object childGeometry =
                _SemanticsGeometry__object.computeChildGeometry(
                    parentPaintClipRect: parentGeometry.paintClipRect,
                    parentSemanticsClipRect: parentGeometry.semanticsClipRect,
                    parentTransform: null,
                    parent: this,
                    child: childLocal
                );
            childLocal._updateGeometry(newGeometry: childGeometry);
        }
        foreach (
            _RenderObjectSemantics__object explicitSiblingChild in siblingMergeGroups
                .expand((group) => group)
                .OfType<_RenderObjectSemantics__object>()
                .expand(
                    (siblingChild) =>
                        siblingChild.shouldFormSemanticsNode
                            ? new List<_RenderObjectSemantics__object> { siblingChild }
                            : siblingChild._children
                )
        )
        {
            if (explicitSiblingChild.renderObject is RenderBox siblingBox && !siblingBox.hasSize)
            {
                continue;
            }
            if (onlyDirtyChildren && !explicitSiblingChild.geometryDirty)
            {
                continue;
            }
            _SemanticsGeometry__object childGeometryLocal =
                _SemanticsGeometry__object.computeChildGeometry(
                    parentPaintClipRect: parentGeometry.paintClipRect,
                    parentSemanticsClipRect: parentGeometry.semanticsClipRect,
                    parentTransform: parentGeometry.transform,
                    parent: this,
                    child: explicitSiblingChild
                );
            explicitSiblingChild._updateGeometry(newGeometry: childGeometryLocal);
        }
    }

    internal virtual void _updateGeometry(_SemanticsGeometry__object newGeometry)
    {
        _SemanticsGeometry__object? currentGeometry = geometry;
        geometry = newGeometry;
        if (currentGeometry is not null)
        {
            bool isSemanticsHidden =
                configProvider.original.isHidden
                || (!(parentData?.mergeIntoParent ?? false) && newGeometry.hidden);
            var sizeChanged = !Equals(currentGeometry.rect.size, newGeometry.rect.size);
            var visibilityChanged = configProvider.effective.isHidden != isSemanticsHidden;
            if (!sizeChanged && !visibilityChanged)
            {
                return;
            }
        }
        markNeedsBuild();
        _updateChildGeometry();
    }

    public virtual void ensureSemanticsNode()
    {
        DartRuntimePrimitives.Assert(() => shouldFormSemanticsNode);
        if (!built)
        {
            _buildSemantics(usedSemanticsIds: new HashSet<long>());
        }
        else
        {
            DartRuntimePrimitives.Assert(() => built);
            _buildSemanticsSubtree(usedSemanticsIds: new HashSet<long>());
        }
    }

    internal virtual void _buildSemantics(HashSet<long> usedSemanticsIds)
    {
        DartRuntimePrimitives.Assert(() => shouldFormSemanticsNode);
        if (cachedSemanticsNode is not null)
        {
            foreach (SemanticsNode node in semanticsNodes)
            {
                if (!Equals(node, cachedSemanticsNode))
                {
                    node.tags = null;
                }
            }
        }
        if (!built)
        {
            _produceSemanticsNode(usedSemanticsIds: usedSemanticsIds);
        }
        DartRuntimePrimitives.Assert(() => built);
        SemanticsNode producedNode = cachedSemanticsNode!;
        foreach (SemanticsNode nodeLocal in semanticsNodes)
        {
            if (!Equals(nodeLocal, producedNode))
            {
                if (parentData?.tagsForChildren is not null)
                {
                    nodeLocal.tags ??= new HashSet<SemanticsTag>();
                    nodeLocal.tags!.UnionWith(parentData!.tagsForChildren!);
                }
                else
                {
                    if (
                        (
                            ((long?)(nodeLocal.tags?.Count)) is { } __count240857
                                ? __count240857 == 0
                                : (bool?)null
                        ) ?? false
                    )
                    {
                        nodeLocal.tags = null;
                    }
                }
            }
        }
    }

    internal virtual void _buildSemanticsSubtree(HashSet<long> usedSemanticsIds)
    {
        var children = new List<SemanticsNode>();
        foreach (_RenderObjectSemantics__object child in _children)
        {
            if (child.geometry is null)
            {
                continue;
            }
            if (child.parentDataDirty)
            {
                continue;
            }
            DartRuntimePrimitives.Assert(() => child.shouldFormSemanticsNode);
            if (
                (child.cachedSemanticsNode is not null)
                && usedSemanticsIds.Contains(child.cachedSemanticsNode!.id)
            )
            {
                child.markNeedsBuild();
                child.cachedSemanticsNode = null;
            }
            child._buildSemantics(usedSemanticsIds: usedSemanticsIds);
            children.AddRange(child.semanticsNodes);
        }
        SemanticsNode node = cachedSemanticsNode!;
        children.removeWhere(shouldDrop);
        bool isSemanticsHidden =
            configProvider.original.isHidden
            || (!(parentData?.mergeIntoParent ?? false) && geometry!.hidden);
        if (configProvider.effective.isHidden != isSemanticsHidden)
        {
            configProvider.updateConfig(
                (config) =>
                {
                    config.isHidden = isSemanticsHidden;
                }
            );
        }
        if (configProvider.effective.isSemanticBoundary)
        {
            if (_needsMergingSiblingNodesIntoSelf)
            {
                var innerNode = new SemanticsNode(showOnScreen: () => renderObject.showOnScreen());
                renderObject.assembleSemanticsNode(innerNode, configProvider.effective, children);
                var configLocal = (
                    (Func<SemanticsConfiguration>)(
                        () =>
                        {
                            var __cascade = new SemanticsConfiguration();
                            __cascade.isSemanticBoundary = true;
                            __cascade.isMergingSemanticsOfDescendants = true;
                            return __cascade;
                        }
                    )
                )();
                node.updateWith(
                    config: configLocal,
                    childrenInInversePaintOrder: new List<SemanticsNode> { innerNode }
                );
            }
            else
            {
                renderObject.assembleSemanticsNode(node, configProvider.effective, children);
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() =>
                !configProvider.effective.isMergingSemanticsOfDescendants
            );
            node.updateWith(
                config: configProvider.effective,
                childrenInInversePaintOrder: children
            );
        }
    }

    internal virtual void _produceSemanticsNode(HashSet<long> usedSemanticsIds)
    {
        DartRuntimePrimitives.Assert(() => !built);
        semanticsNodes.Clear();
        _producedSiblingNodesAndOwners.Clear();
        SemanticsNode node = cachedSemanticsNode ??= _createSemanticsNode();
        (
            (Func<SemanticsNode>)(
                () =>
                {
                    var __cascade = node;
                    __cascade.isMergedIntoParent = parentData?.mergeIntoParent ?? false;
                    __cascade.tags = parentData?.tagsForChildren;
                    return __cascade;
                }
            )
        )();
        _updateSemanticsNodeGeometry();
        _mergeSiblingGroup(usedSemanticsIds);
        _buildSemanticsSubtree(usedSemanticsIds: usedSemanticsIds);
        semanticsNodes.Add(node);
        if (!_needsMergingSiblingNodesIntoSelf)
        {
            semanticsNodes.AddRange(_producedSiblingNodesAndOwners.Keys);
        }
        built = true;
    }

    internal virtual SemanticsNode _createSemanticsNode()
    {
        if (isRoot)
        {
            return SemanticsNode.CreateRoot(
                showOnScreen: () => owner.renderObject.showOnScreen(),
                owner: owner.renderObject.owner!.semanticsOwner!
            );
        }
        return new SemanticsNode(showOnScreen: () => owner.renderObject.showOnScreen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _mergeSiblingGroup(HashSet<long> usedSemanticsIds)
    {
        foreach (List<_SemanticsFragment__object> @group in siblingMergeGroups)
        {
            SemanticsConfiguration? configuration = default!;
            SemanticsNode? node = default!;
            var explicitChildren = new List<_RenderObjectSemantics__object>();
            foreach (var fragmentLocal in @group)
            {
                if (fragmentLocal is _RenderObjectSemantics__object)
                {
                    _RenderObjectSemantics__object fragment__244890__as244923 =
                        (_RenderObjectSemantics__object)fragmentLocal;
                    if (fragment__244890__as244923.shouldFormSemanticsNode)
                    {
                        explicitChildren.Add(fragment__244890__as244923);
                        DartRuntimePrimitives.Assert(() =>
                            fragment__244890__as244923.configToMergeUp is null
                        );
                        continue;
                    }
                    explicitChildren.AddRange(fragment__244890__as244923._children);
                }
                if (fragmentLocal.configToMergeUp is not null)
                {
                    fragmentLocal.mergesToSibling = true;
                    node ??= fragmentLocal.owner.cachedSemanticsNode;
                    configuration ??= new SemanticsConfiguration();
                    configuration.absorb(fragmentLocal.configToMergeUp!);
                }
            }
            var childrenNodes = new List<SemanticsNode>();
            foreach (var explicitChild in explicitChildren)
            {
                explicitChild._buildSemantics(usedSemanticsIds: usedSemanticsIds);
                childrenNodes.AddRange(explicitChild.semanticsNodes);
            }
            if (configuration is not null)
            {
                if ((node is null) || usedSemanticsIds.Contains(node.id))
                {
                    node = new SemanticsNode(showOnScreen: () => renderObject.showOnScreen());
                }
                usedSemanticsIds.Add(node.id);
                foreach (var fragmentAlternate in @group)
                {
                    if (fragmentAlternate.configToMergeUp is not null)
                    {
                        fragmentAlternate.owner.built = true;
                        fragmentAlternate.owner.cachedSemanticsNode = node;
                    }
                }
                node.updateWith(config: configuration, childrenInInversePaintOrder: childrenNodes);
                _producedSiblingNodesAndOwners[DartRuntimePrimitives.RequireReference(node)] =
                    @group;
                HashSet<SemanticsTag> tagsLocal = @group
                    .map((fragment) => fragment.owner.parentData!.tagsForChildren)
                    .OfType<HashSet<SemanticsTag>>()
                    .expand((tagsLocal) => tagsLocal)
                    .toSet();
                if (checked((long)tagsLocal.Count) != 0)
                {
                    if (node.tags is null)
                    {
                        node.tags = tagsLocal;
                    }
                    else
                    {
                        node.tags!.UnionWith(tagsLocal);
                    }
                }
                node.isMergedIntoParent = parentData?.mergeIntoParent ?? false;
            }
        }
        _updateSiblingNodesGeometries();
    }

    internal virtual void _updateSemanticsNodeGeometry()
    {
        SemanticsNode node = cachedSemanticsNode!;
        _SemanticsGeometry__object nodeGeometry = geometry!;
        (
            (Func<SemanticsNode>)(
                () =>
                {
                    var __cascade = node;
                    __cascade.rect = nodeGeometry.rect;
                    __cascade.transform = nodeGeometry.transform;
                    __cascade.parentSemanticsClipRect = nodeGeometry.semanticsClipRect;
                    __cascade.parentPaintClipRect = nodeGeometry.paintClipRect;
                    return __cascade;
                }
            )
        )();
    }

    internal virtual void _updateSiblingNodesGeometries()
    {
        _SemanticsGeometry__object mainGeometry = geometry!;
        foreach (
            MapEntry<
                SemanticsNode,
                List<_SemanticsFragment__object>
            > entry in _producedSiblingNodesAndOwners.entries
        )
        {
            Rect? rectLocal = default!;
            Rect? semanticsClipRectLocal = default!;
            Rect? paintClipRectLocal = default!;
            foreach (_SemanticsFragment__object fragment in entry.value)
            {
                if (fragment.owner.shouldFormSemanticsNode)
                {
                    continue;
                }
                _SemanticsGeometry__object parentGeometry =
                    _SemanticsGeometry__object.computeChildGeometry(
                        parentTransform: mainGeometry.transform,
                        parentSemanticsClipRect: mainGeometry.semanticsClipRect,
                        parentPaintClipRect: mainGeometry.paintClipRect,
                        parent: this,
                        child: fragment.owner
                    );
                Rect rectInFragmentOwnerCoordinates =
                    parentGeometry.semanticsClipRect?.intersect(
                        fragment.owner.renderObject.semanticBounds
                    ) ?? fragment.owner.renderObject.semanticBounds;
                Rect rectInParentCoordinates = MatrixUtils.transformRect(
                    parentGeometry.transform,
                    rectInFragmentOwnerCoordinates
                );
                rectLocal =
                    rectLocal?.expandToInclude(rectInParentCoordinates) ?? rectInParentCoordinates;
                if (parentGeometry.semanticsClipRect is not null)
                {
                    Rect rectAlternate = MatrixUtils.transformRect(
                        parentGeometry.transform,
                        DartRuntimePrimitives.RequireValue(parentGeometry.semanticsClipRect)
                    );
                    semanticsClipRectLocal =
                        semanticsClipRectLocal?.intersect(rectAlternate) ?? rectAlternate;
                }
                if (parentGeometry.paintClipRect is not null)
                {
                    Rect rectNested = MatrixUtils.transformRect(
                        parentGeometry.transform,
                        DartRuntimePrimitives.RequireValue(parentGeometry.paintClipRect)
                    );
                    paintClipRectLocal = paintClipRectLocal?.intersect(rectNested) ?? rectNested;
                }
            }
            SemanticsNode node = entry.key;
            (
                (Func<SemanticsNode>)(
                    () =>
                    {
                        var __cascade = node;
                        __cascade.rect = DartRuntimePrimitives.RequireValue(rectLocal);
                        __cascade.transform = null;
                        __cascade.parentSemanticsClipRect = semanticsClipRectLocal;
                        __cascade.parentPaintClipRect = paintClipRectLocal;
                        return __cascade;
                    }
                )
            )();
        }
    }

    public virtual void markNeedsUpdate()
    {
        using var allocationProfile = FrameworkWorkProfile.AllocationEnabled
            ? FrameworkWorkProfile.Begin(renderObject.GetType(), 13)
            : default;
        renderObject.owner!._nodesNeedingSemanticsGeometryUpdate.Add(renderObject);
        SemanticsNode? producedSemanticsNode = cachedSemanticsNode;
        bool wasSemanticsBoundaryLocal =
            (producedSemanticsNode is not null) && configProvider.wasSemanticsBoundary;
        configProvider.clear();
        _containsIncompleteFragment = false;
        var mayProduceSiblingNodes =
            configProvider.effective.childConfigurationsDelegate is not null;
        bool isEffectiveSemanticsBoundary =
            configProvider.effective.isSemanticBoundary && wasSemanticsBoundaryLocal;
        RenderObject node = renderObject;
        while (
            (node.parent is not null) && (mayProduceSiblingNodes || !isEffectiveSemanticsBoundary)
        )
        {
            if (
                (!Equals(node, renderObject))
                && node._semantics.parentDataDirty
                && !mayProduceSiblingNodes
            )
            {
                break;
            }
            node._semantics.parentData = null;
            node._semantics._blocksPreviousSibling = null;
            if (isEffectiveSemanticsBoundary)
            {
                mayProduceSiblingNodes = false;
            }
            mayProduceSiblingNodes |=
                node._semantics.configProvider.effective.childConfigurationsDelegate is not null;
            node = node.parent!;
            isEffectiveSemanticsBoundary =
                node._semantics.configProvider.effective.isSemanticBoundary
                && node._semantics.built;
        }
        if (
            (!Equals(node, renderObject))
            && (producedSemanticsNode is not null)
            && node._semantics.parentDataDirty
        )
        {
            renderObject.owner!._nodesNeedingSemanticsUpdate.Remove(renderObject);
        }
        if (!node._semantics.parentDataDirty || node._semantics.isRoot)
        {
            if (renderObject.owner is not null)
            {
                DartRuntimePrimitives.Assert(() =>
                    node._semantics.configProvider.effective.isSemanticBoundary
                    || (node.parent is null)
                );
                if (renderObject.owner!._nodesNeedingSemanticsUpdate.Add(node))
                {
                    renderObject.owner!.requestVisualUpdate();
                }
            }
        }
    }

    internal virtual void _marksConflictsInMergeGroup(
        List<_SemanticsFragment__object> mergeGroup,
        bool isMergeUp = false
    )
    {
        var hasSiblingConflict = new HashSet<_SemanticsFragment__object>();
        for (var i = 0L; i < checked(mergeGroup.Count); i += 1L)
        {
            _SemanticsFragment__object fragment = mergeGroup[(int)i];
            fragment.markSiblingConfigurationConflict(false);
            if (fragment.configToMergeUp is null)
            {
                continue;
            }
            if (isMergeUp && !configProvider.original.isCompatibleWith(fragment.configToMergeUp))
            {
                hasSiblingConflict.Add(fragment);
            }
            var siblingLength = i;
            for (var j = 0L; j < siblingLength; j += 1L)
            {
                _SemanticsFragment__object siblingFragment = mergeGroup[(int)j];
                if (!fragment.configToMergeUp!.isCompatibleWith(siblingFragment.configToMergeUp))
                {
                    hasSiblingConflict.Add(fragment);
                    hasSiblingConflict.Add(siblingFragment);
                }
            }
        }
        foreach (var fragmentLocal in hasSiblingConflict)
        {
            fragmentLocal.markSiblingConfigurationConflict(true);
        }
    }

    public virtual void clear()
    {
        built = false;
        cachedSemanticsNode = null;
        parentData = null;
        geometry = null;
        _blocksPreviousSibling = null;
        _containsIncompleteFragment = false;
        mergeUp.Clear();
        siblingMergeGroups.Clear();
        _children.Clear();
        semanticsNodes.Clear();
        configProvider.clear();
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        return _children.map((child) => ((Diagnosticable)child).toDiagnosticsNode()).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(
            new StringProperty("owner", DiagnosticsLibrary.describeIdentity(renderObject))
        );
        properties.add(
            new FlagProperty("noParentData", value: parentDataDirty, ifTrue: "NO PARENT DATA")
        );
        properties.add(new FlagProperty("geometry", value: geometryDirty, ifTrue: "NO GEOMETRY"));
        properties.add(
            new FlagProperty(
                "semanticsBlock",
                value: configProvider.effective.isBlockingSemanticsOfPreviouslyPaintedNodes,
                ifTrue: "BLOCK PREVIOUS"
            )
        );
        if (!parentDataDirty && contributesToSemanticsTree)
        {
            string semanticsNodeStatus = default!;
            if (built)
            {
                semanticsNodeStatus = $"formed {cachedSemanticsNode?.id}";
            }
            else
            {
                if (shouldFormSemanticsNode)
                {
                    semanticsNodeStatus = "needs build";
                }
                else
                {
                    semanticsNodeStatus = "no semantics node";
                }
            }
            properties.add(
                new StringProperty("formedSemanticsNode", semanticsNodeStatus, quoted: false)
            );
        }
        properties.add(
            new FlagProperty(
                "isSemanticBoundary",
                value: configProvider.effective.isSemanticBoundary,
                ifTrue: "semantic boundary"
            )
        );
        properties.add(
            new FlagProperty(
                "blocksSemantics",
                value: isBlockingPreviousSibling,
                ifTrue: "BLOCKS SEMANTICS"
            )
        );
        if (contributesToSemanticsTree && (checked((long)siblingMergeGroups.Count) != 0))
        {
            properties.add(
                new StringProperty("Sibling group", siblingMergeGroups.ToString(), quoted: false)
            );
        }
    }

    public virtual string toStringDeep(
        string prefixLineOne = "",
        string? prefixOtherLines = null,
        DiagnosticLevel minLevel = DiagnosticLevel.debug,
        long? wrapWidth = null
    ) =>
        ((DiagnosticableTree)this).toStringDeep(
            prefixLineOne,
            prefixOtherLines,
            minLevel,
            wrapWidth
        );
}

public static partial class ObjectLibrary
{
    public static void debugDumpRenderObjectSemanticsTree()
    {
        if (RendererBinding.instance.renderViews.Count() == 0)
        {
            PrintLibrary.debugPrint("No render tree root was added to the binding.");
            return;
        }
        PrintLibrary.debugPrint(string.Join("\n\n", new List<string>()));
    }
}

public static partial class ObjectLibrary
{
    internal static string _debugCollectRenderObjectSemanticsTrees(RenderObject root)
    {
        return root._semantics.toStringDeep();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class _SemanticsGeometry__object
{
    public virtual Matrix4 transform { get; private set; } = default!;
    public virtual Rect? semanticsClipRect { get; private set; }
    public virtual Rect? paintClipRect { get; private set; }
    public virtual Rect rect { get; private set; } = default!;
    public virtual bool hidden { get; private set; } = default!;

    internal _SemanticsGeometry__object(
        Rect? paintClipRect,
        Rect? semanticsClipRect,
        Matrix4 transform,
        Rect rect,
        bool hidden
    )
    {
        this.paintClipRect = paintClipRect;
        this.semanticsClipRect = semanticsClipRect;
        this.transform = transform;
        this.rect = rect;
        this.hidden = hidden;
    }

    internal static _SemanticsGeometry__object CreateRoot(Rect rect)
    {
        return new _SemanticsGeometry__object(
            paintClipRect: null,
            semanticsClipRect: null,
            transform: Matrix4.identity(),
            hidden: false,
            rect: DartRuntimePrimitives.RequireValue(rect)
        );
    }

    public virtual bool isVisible => !rect.isEmpty && !transform.isZero();

    public static _SemanticsGeometry__object computeChildGeometry(
        Matrix4? parentTransform,
        Rect? parentPaintClipRect,
        Rect? parentSemanticsClipRect,
        _RenderObjectSemantics__object parent,
        _RenderObjectSemantics__object child
    )
    {
        RenderObject childRenderObject = child.renderObject;
        RenderObject parentRenderObject = parent.renderObject;
        var childToCommonAncestor = new List<RenderObject> { childRenderObject };
        while (childRenderObject.depth > parentRenderObject.depth)
        {
            DartRuntimePrimitives.Assert(() => childRenderObject.parent is not null);
            childRenderObject = childRenderObject.parent!;
            childToCommonAncestor.Add(childRenderObject);
        }
        DartRuntimePrimitives.Assert(() => checked(childToCommonAncestor.Count) >= 2L);
        DartRuntimePrimitives.Assert(() =>
            DartRuntimePrimitives.Identical(childRenderObject, parentRenderObject)
        );
        Rect? paintClipRectLocal = default!;
        Rect? semanticsClipRectLocal = default!;
        var transformLocal = Matrix4.identity();
        for (long i = checked(childToCommonAncestor.Count) - 1L; i > 0L; i -= 1L)
        {
            RenderObject nodeParent = childToCommonAncestor[(int)i];
            RenderObject node = childToCommonAncestor[(int)(i - 1L)];
            Rect? localPaintClipInParent = _transformRect(
                nodeParent.describeApproximatePaintClip(node),
                transformLocal,
                MatrixUtils.transformRect
            );
            Rect? localSemanticsClipInParent = _transformRect(
                nodeParent.describeSemanticsClip(node),
                transformLocal,
                MatrixUtils.transformRect
            );
            paintClipRectLocal = _intersectRects(paintClipRectLocal, localPaintClipInParent);
            semanticsClipRectLocal =
                localSemanticsClipInParent
                ?? semanticsClipRectLocal?.intersect(
                    localPaintClipInParent
                        ?? DartRuntimePrimitives.RequireValue(semanticsClipRectLocal)
                );
            nodeParent.applyPaintTransform(node, transformLocal);
        }
        semanticsClipRectLocal =
            semanticsClipRectLocal ?? _intersectRects(paintClipRectLocal, parentSemanticsClipRect);
        paintClipRectLocal = _intersectRects(paintClipRectLocal, parentPaintClipRect);
        if ((paintClipRectLocal is not null) || (semanticsClipRectLocal is not null))
        {
            Matrix4 inverted = transformLocal.clone();
            var hasInverse = inverted.invert() != 0.0;
            semanticsClipRectLocal = hasInverse
                ? _transformRect(semanticsClipRectLocal, inverted, MatrixUtils.transformRect)
                : null;
            paintClipRectLocal = hasInverse
                ? _transformRect(paintClipRectLocal, inverted, MatrixUtils.transformRect)
                : null;
        }
        if (parentTransform is not null)
        {
            MatrixUtils.multiplyInPlace(parentTransform, transformLocal);
        }
        Rect rectLocal =
            semanticsClipRectLocal?.intersect(child.renderObject.semanticBounds)
            ?? child.renderObject.semanticBounds;
        var isRectHidden = false;
        if (paintClipRectLocal is not null)
        {
            Rect paintClipRect__259962__value262006 = DartRuntimePrimitives.RequireValue(
                paintClipRectLocal
            );
            Rect paintRect = DartRuntimePrimitives
                .RequireValue(paintClipRect__259962__value262006)
                .intersect(
                    DartRuntimePrimitives.RequireValue(
                        DartRuntimePrimitives.RequireValue(rectLocal)
                    )
                );
            isRectHidden =
                paintRect.isEmpty && !DartRuntimePrimitives.RequireValue(rectLocal).isEmpty;
            if (!isRectHidden)
            {
                rectLocal = paintRect;
            }
        }
        return new _SemanticsGeometry__object(
            transform: transformLocal,
            paintClipRect: paintClipRectLocal,
            semanticsClipRect: semanticsClipRectLocal,
            rect: DartRuntimePrimitives.RequireValue(rectLocal),
            hidden: isRectHidden
        );
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Rect? _transformRect(
        Rect? rect,
        Matrix4 transform,
        Func<Matrix4, Rect, Rect> apply = default!
    )
    {
        if (rect is null)
        {
            return null;
        }
        if (DartRuntimePrimitives.RequireValue(rect).isEmpty || transform.isZero())
        {
            return Rect.zero;
        }
        return apply(transform, DartRuntimePrimitives.RequireValue(rect));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static Rect? _intersectRects(Rect? a, Rect? b)
    {
        if (b is null)
        {
            return a;
        }
        return a?.intersect(DartRuntimePrimitives.RequireValue(b))
            ?? DartRuntimePrimitives.RequireValue(b);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }
}

public class DiagnosticsDebugCreator : DiagnosticsProperty<object>
{
    public DiagnosticsDebugCreator(object value)
        : base("debugCreator", value, level: DiagnosticLevel.hidden) { }
}
