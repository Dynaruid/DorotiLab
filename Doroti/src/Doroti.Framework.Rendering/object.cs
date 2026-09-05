// <doroti-reviewed-framework-source />
// Flutter 56b8e1a8: packages/flutter/lib/src/rendering/object.dart
#nullable enable
#pragma warning disable CS0108, CS0114, CS0162, CS0168, CS0675, CS0693, CS4014, CS8321, CS8600, CS8601, CS8602, CS8603, CS8604, CS8605, CS8619, CS8620, CS8622, CS8625, CS8714
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Doroti.Runtime;
using Doroti.Ui;
using static Doroti.Runtime.FoundationRuntimePorts;
using Match = Doroti.Runtime.DartMatch;

namespace Doroti.Framework.Rendering;

public interface IRenderLayoutCallback
{
    void layoutCallback();
}

public class ParentData
{
    public ParentData() { }

    public virtual void detach()
    {
    }

    public override string ToString() => "<none>";
}

public delegate void PaintingContextCallback(PaintingContext context, Offset offset);

internal delegate Rect _TransformRect__object(Matrix4 transform, Rect rect);

public class PaintingContext : global::Doroti.Framework.Painting.ClipContext
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

    public static void repaintCompositedChild(RenderObject child, bool debugAlsoPaintedParent = false)
    {
        DartRuntimePrimitives.Assert(() => ((RenderObject)child)._needsPaint);
        _repaintCompositedChild(child, debugAlsoPaintedParent: debugAlsoPaintedParent);
    }

    internal static void _repaintCompositedChild(RenderObject child, bool debugAlsoPaintedParent = false, PaintingContext? childContext = null)
    {
        FrameworkWorkCounters.Add(FrameworkWork.RepaintBoundary);
        DartRuntimePrimitives.Assert(() => ((RenderObject)child).isRepaintBoundary);
        DartRuntimePrimitives.Assert(() =>
            {
                child.debugRegisterRepaintBoundaryPaint(includedParent: debugAlsoPaintedParent, includedChild: true);
                return true;
            });
        var childLayer = ((OffsetLayer?)(object?)((RenderObject)child)._layerHandle.layer)!;
        if ((childLayer is null))
        {
            DartRuntimePrimitives.Assert(() => debugAlsoPaintedParent);
            DartRuntimePrimitives.Assert(() => (((RenderObject)child)._layerHandle.layer is null));
            OffsetLayer layerLocal = child.updateCompositedLayer(oldLayer: null);
            ((RenderObject)child)._layerHandle.layer = childLayer = layerLocal;
        }
        else
        {
            DartRuntimePrimitives.Assert(() => (debugAlsoPaintedParent || childLayer.attached));
            global::Doroti.Ui.Offset? debugOldOffset = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    debugOldOffset = childLayer!.offset;
                    return true;
                });
            childLayer.removeAllChildren();
            OffsetLayer updatedLayer = child.updateCompositedLayer(oldLayer: childLayer);
            DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(updatedLayer, childLayer));
            DartRuntimePrimitives.Assert(() => (object.Equals(debugOldOffset, ((OffsetLayer)updatedLayer).offset)));
        }
        child._needsCompositedLayerUpdate = false;
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(childLayer, ((RenderObject)child)._layerHandle.layer));
        DartRuntimePrimitives.Assert(() => (((RenderObject)child)._layerHandle.layer is OffsetLayer));
        DartRuntimePrimitives.Assert(() =>
            {
                childLayer!.debugCreator = (((object?)((RenderObject)child).debugCreator ?? (object?)DartRuntimePrimitives.RuntimeType(child)));
                return true;
            });
        childContext ??= new PaintingContext(childLayer, ((RenderObject)child).paintBounds);
        child._paintWithContext(childContext, Offset.zero);
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(childLayer, ((RenderObject)child)._layerHandle.layer));
        childContext.stopRecordingIfNeeded();
    }

    public static void updateLayerProperties(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (((RenderObject)child).isRepaintBoundary && ((RenderObject)child)._wasRepaintBoundary));
        DartRuntimePrimitives.Assert(() => !((RenderObject)child)._needsPaint);
        DartRuntimePrimitives.Assert(() => (((RenderObject)child)._layerHandle.layer is not null));
        var childLayer = ((OffsetLayer?)(object?)((RenderObject)child)._layerHandle.layer!)!;
        global::Doroti.Ui.Offset? debugOldOffset = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                debugOldOffset = ((OffsetLayer)childLayer).offset;
                return true;
            });
        OffsetLayer updatedLayer = child.updateCompositedLayer(oldLayer: childLayer);
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(updatedLayer, childLayer));
        DartRuntimePrimitives.Assert(() => (object.Equals(debugOldOffset, ((OffsetLayer)updatedLayer).offset)));
        child._needsCompositedLayerUpdate = false;
    }

    public static void debugInstrumentRepaintCompositedChild(RenderObject child, bool debugAlsoPaintedParent = false, PaintingContext customContext = default!)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                _repaintCompositedChild(child, debugAlsoPaintedParent: debugAlsoPaintedParent, childContext: customContext);
                return true;
            });
    }

    public virtual void paintChild(RenderObject child, Offset offset)
    {
        DartRuntimePrimitives.Assert(() =>
            {
                global::Doroti.Framework.Rendering.DebugLibrary.debugOnProfilePaint?.Invoke(child);
                return true;
            });
        if (((RenderObject)child).isRepaintBoundary)
        {
            stopRecordingIfNeeded();
            _compositeChild(child, offset);
        }
        else
        {
            if (((RenderObject)child)._wasRepaintBoundary)
            {
                DartRuntimePrimitives.Assert(() => (((RenderObject)child)._layerHandle.layer is OffsetLayer));
                ((RenderObject)child)._layerHandle.layer = null;
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
        DartRuntimePrimitives.Assert(() => !this._isRecording);
        DartRuntimePrimitives.Assert(() => ((RenderObject)child).isRepaintBoundary);
        DartRuntimePrimitives.Assert(() => ((this._canvas is null) || (this._canvas!.getSaveCount() == 1L)));
        if ((((RenderObject)child)._needsPaint || !((RenderObject)child)._wasRepaintBoundary))
        {
            repaintCompositedChild(child, debugAlsoPaintedParent: true);
        }
        else
        {
            if (((RenderObject)child)._needsCompositedLayerUpdate)
            {
                updateLayerProperties(child);
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    child.debugRegisterRepaintBoundaryPaint();
                    ((RenderObject)child)._layerHandle.layer!.debugCreator = (((object?)((RenderObject)child).debugCreator ?? (object?)child));
                    return true;
                });
        }
        DartRuntimePrimitives.Assert(() => (((RenderObject)child)._layerHandle.layer is OffsetLayer));
        var childOffsetLayer = ((OffsetLayer?)(object?)((RenderObject)child)._layerHandle.layer!)!;
        childOffsetLayer.offset = offset;
        appendLayer(childOffsetLayer);
    }

    public virtual void appendLayer(Layer layer)
    {
        DartRuntimePrimitives.Assert(() => !this._isRecording);
        layer.remove();
        this._containerLayer.append(layer);
    }

    internal virtual bool _isRecording
    {
        get
        {
            var hasCanvas = (this._canvas is not null);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (hasCanvas)
                    {
                        DartRuntimePrimitives.Assert(() => (this._currentLayer is not null));
                        DartRuntimePrimitives.Assert(() => (this._recorder is not null));
                        DartRuntimePrimitives.Assert(() => (this._canvas is not null));
                    }
                    else
                    {
                        DartRuntimePrimitives.Assert(() => (this._currentLayer is null));
                        DartRuntimePrimitives.Assert(() => (this._recorder is null));
                        DartRuntimePrimitives.Assert(() => (this._canvas is null));
                    }
                    return true;
                });
            return hasCanvas;
            return default!;
        }
    }
    public virtual global::Doroti.Ui.PictureRecorder recorder
    {
        get
        {
            if ((this._recorder is null))
            {
                _startRecording();
            }
            DartRuntimePrimitives.Assert(() => (this._currentLayer is not null));
            return this._recorder!;
            return default!;
        }
    }
    public override Canvas canvas
    {
        get
        {
            if ((this._canvas is null))
            {
                _startRecording();
            }
            DartRuntimePrimitives.Assert(() => (this._currentLayer is not null));
            return this._canvas!;
            return default!;
        }
    }
    internal virtual void _startRecording()
    {
        DartRuntimePrimitives.Assert(() => !this._isRecording);
        _currentLayer = new PictureLayer(this.estimatedBounds);
        _recorder = RendererBinding.instance.createPictureRecorder();
        _canvas = RendererBinding.instance.createCanvas(this._recorder!);
        this._containerLayer.append(this._currentLayer!);
    }

    public virtual Action addCompositionCallback(Action<Layer> callback)
    {
        return this._containerLayer.addCompositionCallback((Action<Layer>)callback);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void stopRecordingIfNeeded()
    {
        if (!this._isRecording)
        {
            return;
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugRepaintRainbowEnabled)
                {
                    var paint = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 6.0;
    __cascade.color = global::Doroti.Framework.Rendering.DebugLibrary.debugCurrentRepaintColor.toColor();
    return __cascade;
}))();
                    this.canvas.drawRect(this.estimatedBounds.deflate(3.0), paint);
                }
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugPaintLayerBordersEnabled)
                {
                    var paintLocal = ((Func<Paint>)(() =>
{
    var __cascade = new global::Doroti.Ui.Paint();
    __cascade.style = PaintingStyle.stroke;
    __cascade.strokeWidth = 1.0;
    __cascade.color = new global::Doroti.Ui.Color(4294940672L);
    return __cascade;
}))();
                    this.canvas.drawRect(this.estimatedBounds, paintLocal);
                }
                return true;
            });
        this._currentLayer!.picture = this._recorder!.endRecording();
        FrameworkWorkCounters.Add(FrameworkWork.NewPicture);
        _currentLayer = null;
        _recorder = null;
        _canvas = null;
    }

    public virtual void setIsComplexHint()
    {
        if ((this._currentLayer is null))
        {
            _startRecording();
        }
        this._currentLayer!.isComplexHint = true;
    }

    public virtual void setWillChangeHint()
    {
        if ((this._currentLayer is null))
        {
            _startRecording();
        }
        this._currentLayer!.willChangeHint = true;
    }

    public virtual void addLayer(Layer layer)
    {
        stopRecordingIfNeeded();
        appendLayer(layer);
    }

    public virtual void pushLayer(ContainerLayer childLayer, Action<PaintingContext, Offset> painter, Offset offset, Rect? childPaintBounds = null)
    {
        if (((ContainerLayer)childLayer).hasChildren)
        {
            childLayer.removeAllChildren();
        }
        stopRecordingIfNeeded();
        appendLayer(childLayer);
        PaintingContext childContext = createChildContext(childLayer, (childPaintBounds ?? this.estimatedBounds));
        painter(childContext, offset);
        childContext.stopRecordingIfNeeded();
    }

    public virtual PaintingContext createChildContext(ContainerLayer childLayer, Rect bounds)
    {
        return new PaintingContext(childLayer, bounds);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ClipRectLayer? pushClipRect(bool needsCompositing, Offset offset, Rect clipRect, Action<PaintingContext, Offset> painter, Clip clipBehavior = Clip.hardEdge, ClipRectLayer? oldLayer = null)
    {
        if ((object.Equals(clipBehavior, Clip.none)))
        {
            painter(this, offset);
            return null;
        }
        global::Doroti.Ui.Rect offsetClipRect = clipRect.shift(offset);
        if (needsCompositing)
        {
            ClipRectLayer layer = (oldLayer ?? new ClipRectLayer());
            ((Func<ClipRectLayer>)(() =>
{
    var __cascade = layer;
    __cascade.clipRect = offsetClipRect;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))();
            pushLayer(layer, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: offsetClipRect);
            return layer;
        }
        else
        {
            clipRectAndPaint(offsetClipRect, clipBehavior, offsetClipRect, ((Action)(() => painter(this, offset))));
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ClipRRectLayer? pushClipRRect(bool needsCompositing, Offset offset, Rect bounds, RRect clipRRect, Action<PaintingContext, Offset> painter, Clip clipBehavior = Clip.antiAlias, ClipRRectLayer? oldLayer = null)
    {
        if ((object.Equals(clipBehavior, Clip.none)))
        {
            painter(this, offset);
            return null;
        }
        global::Doroti.Ui.Rect offsetBounds = bounds.shift(offset);
        global::Doroti.Ui.RRect offsetClipRRect = clipRRect.shift(offset);
        if (needsCompositing)
        {
            ClipRRectLayer layer = (oldLayer ?? new ClipRRectLayer());
            ((Func<ClipRRectLayer>)(() =>
{
    var __cascade = layer;
    __cascade.clipRRect = offsetClipRRect;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))();
            pushLayer(layer, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: offsetBounds);
            return layer;
        }
        else
        {
            clipRRectAndPaint(offsetClipRRect, clipBehavior, offsetBounds, ((Action)(() => painter(this, offset))));
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ClipRSuperellipseLayer? pushClipRSuperellipse(bool needsCompositing, Offset offset, Rect bounds, RSuperellipse clipRSuperellipse, Action<PaintingContext, Offset> painter, Clip clipBehavior = Clip.antiAlias, ClipRSuperellipseLayer? oldLayer = null)
    {
        if ((object.Equals(clipBehavior, Clip.none)))
        {
            painter(this, offset);
            return null;
        }
        global::Doroti.Ui.Rect offsetBounds = bounds.shift(offset);
        global::Doroti.Ui.RSuperellipse offsetShape = clipRSuperellipse.shift(offset);
        if (needsCompositing)
        {
            ClipRSuperellipseLayer layer = (oldLayer ?? new ClipRSuperellipseLayer());
            ((Func<ClipRSuperellipseLayer>)(() =>
{
    var __cascade = layer;
    __cascade.clipRSuperellipse = offsetShape;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))();
            pushLayer(layer, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: offsetBounds);
            return layer;
        }
        else
        {
            clipRSuperellipseAndPaint(offsetShape, clipBehavior, offsetBounds, ((Action)(() => painter(this, offset))));
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ClipPathLayer? pushClipPath(bool needsCompositing, Offset offset, Rect bounds, Path clipPath, Action<PaintingContext, Offset> painter, Clip clipBehavior = Clip.antiAlias, ClipPathLayer? oldLayer = null)
    {
        if ((object.Equals(clipBehavior, Clip.none)))
        {
            painter(this, offset);
            return null;
        }
        global::Doroti.Ui.Rect offsetBounds = bounds.shift(offset);
        global::Doroti.Ui.Path offsetClipPath = clipPath.shift(offset);
        if (needsCompositing)
        {
            ClipPathLayer layer = (oldLayer ?? new ClipPathLayer());
            ((Func<ClipPathLayer>)(() =>
{
    var __cascade = layer;
    __cascade.clipPath = offsetClipPath;
    __cascade.clipBehavior = clipBehavior;
    return __cascade;
}))();
            pushLayer(layer, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: offsetBounds);
            return layer;
        }
        else
        {
            clipPathAndPaint(offsetClipPath, clipBehavior, offsetBounds, ((Action)(() => painter(this, offset))));
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ColorFilterLayer pushColorFilter(Offset offset, ColorFilter colorFilter, Action<PaintingContext, Offset> painter, ColorFilterLayer? oldLayer = null)
    {
        ColorFilterLayer layer = (oldLayer ?? new ColorFilterLayer());
        layer.colorFilter = colorFilter;
        pushLayer(layer, (Action<PaintingContext, Offset>)painter, offset);
        return layer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual TransformLayer? pushTransform(bool needsCompositing, Offset offset, Matrix4 transform, Action<PaintingContext, Offset> painter, TransformLayer? oldLayer = null)
    {
        var effectiveTransform = ((Func<Matrix4>)(() =>
{
    var __cascade = Matrix4.translationValues(offset.dx, offset.dy, 0.0);
    __cascade.multiply(transform);
    __cascade.translateByDouble(-offset.dx, -offset.dy, 0, 1);
    return __cascade;
}))();
        if (needsCompositing)
        {
            TransformLayer layer = (oldLayer ?? new TransformLayer());
            layer.transform = effectiveTransform;
            pushLayer(layer, (Action<PaintingContext, Offset>)painter, offset, childPaintBounds: MatrixUtils.inverseTransformRect(effectiveTransform, this.estimatedBounds));
            return layer;
        }
        else
        {
            ((Func<Canvas>)(() =>
{
    var __cascade = this.canvas;
    __cascade.save();
    __cascade.transform(effectiveTransform.storage);
    return __cascade;
}))();
            painter(this, offset);
            this.canvas.restore();
            return null;
        }
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual OpacityLayer pushOpacity(Offset offset, long alpha, Action<PaintingContext, Offset> painter, OpacityLayer? oldLayer = null)
    {
        OpacityLayer layer = (oldLayer ?? new OpacityLayer());
        ((Func<OpacityLayer>)(() =>
{
    var __cascade = layer;
    __cascade.alpha = alpha;
    __cascade.offset = offset;
    return __cascade;
}))();
        pushLayer(layer, (Action<PaintingContext, Offset>)painter, Offset.zero);
        return layer;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public override string ToString() => $"{(global::Doroti.Framework.Foundation.objectRuntimeTypeFunctions.objectRuntimeType(this, "PaintingContext"))}#{GetHashCode()}(layer: {this._containerLayer}, canvas bounds: {this.estimatedBounds})";
}

public abstract class Constraints
{
    protected Constraints()
    {
    }

    public abstract bool isTight { get; }
    public abstract bool isNormalized { get; }
    public virtual bool debugAssertIsValid(bool isAppliedConstraint = false, InformationCollector? informationCollector = null)
    {
        DartRuntimePrimitives.Assert(() => this.isNormalized);
        return this.isNormalized;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public delegate void RenderObjectVisitor(RenderObject child);

public delegate void LayoutCallback<T>(T constraints) where T : Constraints;

internal class _LocalSemanticsHandle__object : global::Doroti.Framework.Semantics.SemanticsHandle
{
    internal virtual PipelineOwner _owner { get; private set; } = default!;
    public virtual Action? listener { get; private set; }

    internal _LocalSemanticsHandle__object(PipelineOwner owner, Action? listener)
    {
        this.listener = listener;
        this._owner = owner;
    }

    public override void dispose()
    {
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        if ((this.listener is not null))
        {
            ((PipelineOwner)this._owner).semanticsOwner!.removeListener(this.listener!);
        }
        this._owner._didDisposeSemanticsHandle();
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
    internal virtual List<RenderObject> _nodesNeedingLayout { get; set; } = new List<RenderObject>();
    private List<RenderObject> _nodesNeedingLayoutScratch = new List<RenderObject>();
    internal virtual bool _debugDoingLayout { get; set; } = false;
    internal virtual bool _debugDoingChildLayout { get; set; } = false;
    internal virtual bool _debugAllowMutationsToDirtySubtrees { get; set; } = false;
    internal virtual List<RenderObject> _nodesNeedingCompositingBitsUpdate { get; private set; } = new List<RenderObject>();
    internal virtual List<RenderObject> _nodesNeedingPaint { get; set; } = new List<RenderObject>();
    private List<RenderObject> _nodesNeedingPaintScratch = new List<RenderObject>();
    internal virtual bool _debugDoingPaint { get; set; } = false;
    internal virtual global::Doroti.Framework.Semantics.SemanticsOwner? _semanticsOwner { get; set; } = default;
    internal virtual long _outstandingSemanticsHandles { get; set; } = 0L;
    internal virtual bool _debugDoingSemantics { get; set; } = false;
    internal virtual HashSet<RenderObject> _nodesNeedingSemanticsUpdate { get; private set; } = new HashSet<RenderObject>();
    internal virtual HashSet<RenderObject> _nodesNeedingSemanticsGeometryUpdate { get; private set; } = new HashSet<RenderObject>();
    internal virtual HashSet<PipelineOwner> _children { get; private set; } = new HashSet<PipelineOwner>();
    internal virtual PipelineManifold? _manifold { get; set; } = default;
    internal virtual PipelineOwner? _debugParent { get; set; } = default;

    public PipelineOwner(Action? onNeedVisualUpdate = null, Action? onSemanticsOwnerCreated = null, Action<SemanticsUpdate>? onSemanticsUpdate = null, Action? onSemanticsOwnerDisposed = null)
    {
        this.onNeedVisualUpdate = onNeedVisualUpdate;
        this.onSemanticsOwnerCreated = onSemanticsOwnerCreated;
        this.onSemanticsUpdate = onSemanticsUpdate;
        this.onSemanticsOwnerDisposed = onSemanticsOwnerDisposed;
    }

    public virtual void requestVisualUpdate()
    {
        if ((this.onNeedVisualUpdate is not null))
        {
            this.onNeedVisualUpdate!();
        }
        else
        {
            this._manifold?.requestVisualUpdate();
        }
    }

    public virtual RenderObject? rootNode
    {
        get => this._rootNode;
        set
        {
            var __value = value;
            if ((object.Equals(this._rootNode, __value)))
            {
                return;
            }
            this._rootNode?.detach();
            _rootNode = __value;
            this._rootNode?.attach(this);
        }
    }
    public virtual IEnumerable<RenderObject> nodesNeedingLayout => this._nodesNeedingLayout;
    public virtual bool debugDoingLayout => this._debugDoingLayout;
    public virtual void flushLayout()
    {
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Rendering.DebugLibrary.debugEnhanceLayoutTimelineArguments)
                    {
                        debugTimelineArguments = new DartMap<string, string> { ["dirty count"] = $"{checked((long)(this._nodesNeedingLayout.Count))}", ["dirty list"] = $"{this._nodesNeedingLayout}" };
                    }
                    return true;
                });
            FlutterTimeline.startSync($"LAYOUT{this._debugRootSuffixForTimelineEventNames}", arguments: debugTimelineArguments);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingLayout = true;
                return true;
            });
        try
        {
            while ((checked((long)(this._nodesNeedingLayout.Count)) != 0))
            {
                DartRuntimePrimitives.Assert(() => !this._shouldMergeDirtyNodes);
                List<RenderObject> dirtyNodes = this._nodesNeedingLayout;
                _nodesNeedingLayout = this._nodesNeedingLayoutScratch;
                _nodesNeedingLayoutScratch = dirtyNodes;
                this._nodesNeedingLayout.Clear();
                dirtyNodes.sort(((a, b) => (((RenderObject)a).depth - ((RenderObject)b).depth)));
                for (var i = 0L; (i < checked((long)(dirtyNodes.Count))); i++)
                {
                    if (this._shouldMergeDirtyNodes)
                    {
                        _shouldMergeDirtyNodes = false;
                        if ((checked((long)(this._nodesNeedingLayout.Count)) != 0))
                        {
                            this._nodesNeedingLayout.AddRange(dirtyNodes.Skip(checked((int)i)).ToList());
                            break;
                        }
                    }
                    RenderObject node = dirtyNodes[(int)(i)];
                    if ((((RenderObject)node)._needsLayout && (object.Equals(((RenderObject)node).owner, this))))
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
            foreach (PipelineOwner child in this._children)
            {
                child.flushLayout();
            }
            DartRuntimePrimitives.Assert(() => (checked((long)(this._nodesNeedingLayout.Count)) == 0));
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
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    internal virtual void _enableMutationsToDirtySubtrees(Action callback)
    {
        DartRuntimePrimitives.Assert(() => this._debugDoingLayout);
        bool? oldState = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                oldState = this._debugAllowMutationsToDirtySubtrees;
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
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync($"UPDATING COMPOSITING BITS{this._debugRootSuffixForTimelineEventNames}");
        }
        this._nodesNeedingCompositingBitsUpdate.sort(((a, b) => (((RenderObject)a).depth - ((RenderObject)b).depth)));
        foreach (RenderObject node in this._nodesNeedingCompositingBitsUpdate)
        {
            if ((((RenderObject)node)._needsCompositingBitsUpdate && (object.Equals(((RenderObject)node).owner, this))))
            {
                node._updateCompositingBits();
            }
        }
        this._nodesNeedingCompositingBitsUpdate.Clear();
        foreach (PipelineOwner child in this._children)
        {
            child.flushCompositingBits();
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(this._nodesNeedingCompositingBitsUpdate.Count)) == 0));
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.finishSync();
        }
    }

    public virtual IEnumerable<RenderObject> nodesNeedingPaint => this._nodesNeedingPaint;
    public virtual bool debugDoingPaint => this._debugDoingPaint;
    public virtual void flushPaint()
    {
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Rendering.DebugLibrary.debugEnhancePaintTimelineArguments)
                    {
                        debugTimelineArguments = new DartMap<string, string> { ["dirty count"] = $"{checked((long)(this._nodesNeedingPaint.Count))}", ["dirty list"] = $"{this._nodesNeedingPaint}" };
                    }
                    return true;
                });
            FlutterTimeline.startSync($"PAINT{this._debugRootSuffixForTimelineEventNames}", arguments: debugTimelineArguments);
        }
        try
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingPaint = true;
                    return true;
                });
            List<RenderObject> dirtyNodes = this._nodesNeedingPaint;
            _nodesNeedingPaint = this._nodesNeedingPaintScratch;
            _nodesNeedingPaintScratch = dirtyNodes;
            this._nodesNeedingPaint.Clear();
            foreach (var node in ((Func<List<RenderObject>>)(() =>
{
    var __cascade = dirtyNodes;
    __cascade.sort(((a, b) => (((RenderObject)b).depth - ((RenderObject)a).depth)));
    return __cascade;
}))())
            {
                DartRuntimePrimitives.Assert(() => (((RenderObject)node)._layerHandle.layer is not null));
                if ((((((RenderObject)node)._needsPaint || ((RenderObject)node)._needsCompositedLayerUpdate)) && (object.Equals(((RenderObject)node).owner, this))))
                {
                    if (((RenderObject)node)._layerHandle.layer!.attached)
                    {
                        DartRuntimePrimitives.Assert(() => ((RenderObject)node).isRepaintBoundary);
                        if (((RenderObject)node)._needsPaint)
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
            foreach (PipelineOwner child in this._children)
            {
                child.flushPaint();
            }
            DartRuntimePrimitives.Assert(() => (checked((long)(this._nodesNeedingPaint.Count)) == 0));
        }
        finally
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingPaint = false;
                    return true;
                });
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
        }
    }

    public virtual global::Doroti.Framework.Semantics.SemanticsOwner? semanticsOwner => this._semanticsOwner;
    public virtual long debugOutstandingSemanticsHandles => this._outstandingSemanticsHandles;
    public virtual global::Doroti.Framework.Semantics.SemanticsHandle ensureSemantics(Action? listener = null)
    {
        _outstandingSemanticsHandles += 1L;
        _updateSemanticsOwner();
        return new _LocalSemanticsHandle__object(this, listener);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _updateSemanticsOwner()
    {
        if ((((this._manifold?.semanticsEnabled ?? false)) || (this._outstandingSemanticsHandles > 0L)))
        {
            if ((this._semanticsOwner is null))
            {
                DartRuntimePrimitives.Assert(() => (this.onSemanticsUpdate is not null));
                _semanticsOwner = new global::Doroti.Framework.Semantics.SemanticsOwner(onSemanticsUpdate: this.onSemanticsUpdate!);
                this.onSemanticsOwnerCreated?.Invoke();
            }
        }
        else
        {
            if ((this._semanticsOwner is not null))
            {
                this._semanticsOwner?.dispose();
                _semanticsOwner = null;
                this.onSemanticsOwnerDisposed?.Invoke();
            }
        }
    }

    internal virtual void _didDisposeSemanticsHandle()
    {
        DartRuntimePrimitives.Assert(() => (this._semanticsOwner is not null));
        _outstandingSemanticsHandles -= 1L;
        _updateSemanticsOwner();
    }

    public virtual void flushSemantics()
    {
        if ((this._semanticsOwner is null))
        {
            return;
        }
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            FlutterTimeline.startSync($"SEMANTICS{this._debugRootSuffixForTimelineEventNames}");
        }
        DartRuntimePrimitives.Assert(() => (this._semanticsOwner is not null));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingSemantics = true;
                return true;
            });
        try
        {
            List<RenderObject> nodesToProcess = ((Func<List<RenderObject>>)(() =>
{
    var __cascade = this._nodesNeedingSemanticsUpdate.where(((@object) => (!((RenderObject)@object)._needsLayout && (object.Equals(((RenderObject)@object).owner, this))))).ToList();
    __cascade.sort(((a, b) => (((RenderObject)a).depth - ((RenderObject)b).depth)));
    return __cascade;
}))();
            this._nodesNeedingSemanticsUpdate.Clear();
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.updateChildren");
            }
            RenderObject? rootNodeLocal = this.rootNode;
            foreach (var node in nodesToProcess)
            {
                if (((RenderObject)node)._semantics.parentDataDirty)
                {
                    continue;
                }
                ((RenderObject)node)._semantics.updateChildren();
            }
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    DartRuntimePrimitives.Assert(() => ((checked((long)(nodesToProcess.Count)) == 0) || (rootNodeLocal is not null)));
                    if ((rootNodeLocal is not null))
                    {
                        _RenderObjectSemantics__object.debugCheckForParentData(rootNodeLocal);
                    }
                    return true;
                });
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.ensureGeometry");
            }
            List<RenderObject> nodesToProcessGeometry = this._nodesNeedingSemanticsGeometryUpdate.where(((@object) => ((!((RenderObject)@object)._needsLayout && (object.Equals(((RenderObject)@object).owner, this))) && !((RenderObject)@object)._semantics.parentDataDirty))).ToList();
            this._nodesNeedingSemanticsGeometryUpdate.Clear();
            foreach (var nodeLocal in nodesToProcessGeometry)
            {
                if ((((RenderObject)nodeLocal)._semantics.shouldFormSemanticsNode && ((RenderObject)nodeLocal)._semantics.geometryDirty))
                {
                    continue;
                }
                if ((((RenderObject)nodeLocal)._semantics.shouldFormSemanticsNode && ((((RenderObject)nodeLocal)._isRelayoutBoundary ?? false))))
                {
                    ((RenderObject)nodeLocal)._semantics.geometry = null;
                    continue;
                }
                if (!((RenderObject)nodeLocal)._semantics.contributesToSemanticsTree)
                {
                    foreach (_RenderObjectSemantics__object child in ((RenderObject)nodeLocal)._semantics.mergeUp.OfType<_RenderObjectSemantics__object>())
                    {
                        if (((_RenderObjectSemantics__object)child).shouldFormSemanticsNode)
                        {
                            child.geometry = null;
                        }
                        else
                        {
                            foreach (_RenderObjectSemantics__object nodeInSubtree in ((_RenderObjectSemantics__object)child)._children)
                            {
                                DartRuntimePrimitives.Assert(() => ((_RenderObjectSemantics__object)nodeInSubtree).shouldFormSemanticsNode);
                                nodeInSubtree.geometry = null;
                            }
                        }
                    }
                    continue;
                }
                foreach (_RenderObjectSemantics__object childLocal in ((RenderObject)nodeLocal)._semantics._children)
                {
                    childLocal.geometry = null;
                }
            }
            var treeShapeToken = new object();
            var nodeToEnsureGeometry = new HashSet<_RenderObjectSemantics__object>();
            foreach (var nodeAlternate in nodesToProcessGeometry)
            {
                ((RenderObject)nodeAlternate)._semantics.computeAncestorInfo(treeShapeToken);
                if ((((RenderObject)nodeAlternate)._semantics.firstAncestorNodeWithCleanGeometry is not null))
                {
                    nodeToEnsureGeometry.Add(((RenderObject)nodeAlternate)._semantics.firstAncestorNodeWithCleanGeometry!);
                }
            }
            foreach (_RenderObjectSemantics__object nodeNested in ((Func<List<_RenderObjectSemantics__object>>)(() =>
{
    var __cascade = nodeToEnsureGeometry.ToList();
    __cascade.sort(((a, b) => (((_RenderObjectSemantics__object)a).renderObject.depth - ((_RenderObjectSemantics__object)b).renderObject.depth)));
    return __cascade;
}))())
            {
                nodeNested.ensureGeometry();
            }
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.startSync("Semantics.ensureSemanticsNode");
            }
            foreach (RenderObject nodeCurrent in System.Linq.Enumerable.Reverse(nodesToProcess))
            {
                ((RenderObject)nodeCurrent)._semantics.computeAncestorInfo(treeShapeToken);
                var targets = new List<_RenderObjectSemantics__object>();
                if (((RenderObject)nodeCurrent)._semantics.geometryDirty)
                {
                    if ((((RenderObject)nodeCurrent)._semantics.firstAncestorNodeWithCleanGeometry is not null))
                    {
                        targets.Add(((RenderObject)nodeCurrent)._semantics.firstAncestorNodeWithCleanGeometry!);
                    }
                }
                else
                {
                    if ((!((RenderObject)nodeCurrent)._semantics.geometry!.isVisible && !((RenderObject)nodeCurrent)._semantics.isRoot))
                    {
                        _RenderObjectSemantics__object? parentInSemanticsTreeLocal = ((RenderObject)nodeCurrent)._semantics.parentInSemanticsTree;
                        if ((parentInSemanticsTreeLocal is not null))
                        {
                            if (!((_RenderObjectSemantics__object)parentInSemanticsTreeLocal).geometryDirty)
                            {
                                targets.Add(parentInSemanticsTreeLocal);
                            }
                            else
                            {
                                _RenderObjectSemantics__object? firstAncestorNodeWithCleanGeometryLocal = ((_RenderObjectSemantics__object)parentInSemanticsTreeLocal).firstAncestorNodeWithCleanGeometry;
                                if ((firstAncestorNodeWithCleanGeometryLocal is not null))
                                {
                                    targets.Add(firstAncestorNodeWithCleanGeometryLocal);
                                }
                            }
                        }
                    }
                    targets.Add(((RenderObject)nodeCurrent)._semantics);
                }
                foreach (var target in targets)
                {
                    if (((_RenderObjectSemantics__object)target).parentDataDirty)
                    {
                        continue;
                    }
                    target.ensureSemanticsNode();
                }
            }
            DartRuntimePrimitives.Assert(() =>
                {
                    if ((rootNodeLocal is not null))
                    {
                        _RenderObjectSemantics__object.debugCheckForBuilds(((RenderObject)rootNodeLocal)._semantics);
                    }
                    return true;
                });
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                FlutterTimeline.finishSync();
            }
            this._semanticsOwner!.sendSemanticsUpdate();
            foreach (PipelineOwner childAlternate in this._children)
            {
                childAlternate.flushSemantics();
            }
            DartRuntimePrimitives.Assert(() => (checked((long)(this._nodesNeedingSemanticsUpdate.Count)) == 0));
            DartRuntimePrimitives.Assert(() => (checked((long)(this._nodesNeedingSemanticsGeometryUpdate.Count)) == 0));
        }
        finally
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingSemantics = false;
                    return true;
                });
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
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
        this._semanticsOwner is not null &&
        (this._nodesNeedingSemanticsUpdate.Count != 0 ||
         this._nodesNeedingSemanticsGeometryUpdate.Count != 0 ||
         this._children.Any(child => child.hasPendingSemanticsUpdate));

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        return new List<DiagnosticsNode>();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new DiagnosticsProperty<RenderObject>("rootNode", this.rootNode, defaultValue: null));
    }

    internal virtual bool _debugSetParent(PipelineOwner child, PipelineOwner? parent)
    {
        child._debugParent = parent;
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual string _debugRootSuffixForTimelineEventNames => ((this._debugParent is null) ? " (root)" : "");
    public virtual void attach(PipelineManifold manifold)
    {
        DartRuntimePrimitives.Assert(() => (this._manifold is null));
        _manifold = manifold;
        this._manifold!.addListener(this._updateSemanticsOwner);
        _updateSemanticsOwner();
        foreach (PipelineOwner child in this._children)
        {
            child.attach(manifold);
        }
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => (this._manifold is not null));
        this._manifold!.removeListener(this._updateSemanticsOwner);
        _manifold = null;
        foreach (PipelineOwner child in this._children)
        {
            child.detach();
        }
    }

    internal virtual bool _debugAllowChildListModifications => ((!this._debugDoingChildLayout && !this._debugDoingPaint) && !this._debugDoingSemantics);
    public virtual void adoptChild(PipelineOwner child)
    {
        DartRuntimePrimitives.Assert(() => (((PipelineOwner)child)._debugParent is null));
        DartRuntimePrimitives.Assert(() => !this._children.Contains(child));
        DartRuntimePrimitives.Assert(() => this._debugAllowChildListModifications);
        this._children.Add(child);
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            _debugSetParent(child, this);
        }
        if ((this._manifold is not null))
        {
            child.attach(this._manifold!);
        }
    }

    public virtual void dropChild(PipelineOwner child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((PipelineOwner)child)._debugParent, this)));
        DartRuntimePrimitives.Assert(() => this._children.Contains(child));
        DartRuntimePrimitives.Assert(() => this._debugAllowChildListModifications);
        this._children.Remove(child);
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            _debugSetParent(child, null);
        }
        if ((this._manifold is not null))
        {
            child.detach();
        }
    }

    public virtual void visitChildren(Action<PipelineOwner> visitor)
    {
        this._children.forEach(visitor);
    }

    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => (checked((long)(this._children.Count)) == 0));
        DartRuntimePrimitives.Assert(() => (this.rootNode is null));
        DartRuntimePrimitives.Assert(() => (this._manifold is null));
        DartRuntimePrimitives.Assert(() => (this._debugParent is null));
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._semanticsOwner?.dispose();
        _semanticsOwner = null;
        this._nodesNeedingLayout.Clear();
        this._nodesNeedingCompositingBitsUpdate.Clear();
        this._nodesNeedingPaint.Clear();
        this._nodesNeedingSemanticsUpdate.Clear();
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
    public virtual LayerHandle<ContainerLayer> _layerHandle { get; private set; } = new LayerHandle<ContainerLayer>();
    internal virtual bool _needsCompositingBitsUpdate { get; set; } = false;
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

    protected RenderObject()
    {
    }

    public virtual void reassemble()
    {
        markNeedsLayout();
        markNeedsCompositingBitsUpdate();
        markNeedsPaint();
        markNeedsSemanticsUpdate();
        visitChildren(((Action<RenderObject>)((child) =>
        {
            child.reassemble();
        })));
    }

    public virtual bool? debugDisposed
    {
        get
        {
            bool? disposed = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    disposed = this._debugDisposed;
                    return true;
                });
            return disposed;
            return default!;
        }
    }
    public virtual void dispose()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => global::Doroti.Framework.Foundation.DebugLibrary.debugMaybeDispatchDisposed(this));
        this._layerHandle.layer = null;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDisposed = true;
                return true;
            });
    }

    public virtual void setupParentData(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => this._debugCanPerformMutations);
        if ((((RenderObject)child).parentData is not ParentData))
        {
            child.parentData = new ParentData();
        }
    }

    public virtual long depth => this._depth;
    public virtual void redepthChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).owner, this.owner)));
        if ((((RenderObject)child)._depth <= this._depth))
        {
            child._depth = (this._depth + 1L);
            child.redepthChildren();
        }
    }

    public virtual void redepthChildren()
    {
    }

    public virtual RenderObject? parent => this._parent;
    public virtual void adoptChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (((RenderObject)child)._parent is null));
        DartRuntimePrimitives.Assert(() =>
            {
                var node = this;
                while ((((RenderObject)node).parent is not null))
                {
                    node = ((RenderObject)node).parent!;
                }
                DartRuntimePrimitives.Assert(() => (!object.Equals(node, child)));
                return true;
            });
        setupParentData(child);
        markNeedsLayout();
        markNeedsCompositingBitsUpdate();
        markNeedsSemanticsUpdate();
        child._parent = this;
        if (this.attached)
        {
            child.attach(this.owner!);
        }
        redepthChild(child);
    }

    public virtual void dropChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child)._parent, this)));
        DartRuntimePrimitives.Assert(() => (((RenderObject)child).attached == this.attached));
        DartRuntimePrimitives.Assert(() => (((RenderObject)child).parentData is not null));
        if (!((((RenderObject)child)._isRelayoutBoundary ?? true)))
        {
            child._isRelayoutBoundary = null;
        }
        ((RenderObject)child).parentData!.detach();
        child.parentData = null;
        child._parent = null;
        if (this.attached)
        {
            child.detach();
        }
        markNeedsLayout();
        markNeedsCompositingBitsUpdate();
        markNeedsSemanticsUpdate();
    }

    public virtual void visitChildren(Action<RenderObject> visitor)
    {
    }

    internal virtual void _reportException(string method, object exception, global::System.Diagnostics.StackTrace stack)
    {
        FlutterError.reportError(new FlutterErrorDetails(exception: exception, stack: stack, library: "rendering library", context: new ErrorDescription($"during {method}()"), informationCollector: (() => new List<DiagnosticsNode> { describeForError("The following RenderObject was being processed when the exception was fired"), describeForError("RenderObject", style: DiagnosticsTreeStyle.truncateChildren) })));
    }

    public virtual bool debugDoingThisResize => this._debugDoingThisResize;
    public virtual bool debugDoingThisLayout => this._debugDoingThisLayout;
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

    public virtual bool debugCanParentUseSize => DartRuntimePrimitives.RequireValue(this._debugCanParentUseSize);
    internal virtual (RenderObject, bool)? _debugClosestMutationRoot
    {
        get
        {
            return (this switch { RenderObject { _doingThisLayoutWithCallback: true } __object91304 => (this, true), RenderObject { owner: PipelineOwner { _debugAllowMutationsToDirtySubtrees: true } __object91571, _needsLayout: true } __object91542 => (this, true), RenderObject { _debugMutationsLocked: true } __object91746 => (this, false), RenderObject __object91812 => this.debugLayoutParent?._debugClosestMutationRoot, _ => throw new InvalidOperationException("Non-exhaustive Dart switch value.") });
            return default!;
        }
    }
    internal virtual bool _debugCanPerformMutations
    {
        get
        {
            bool isMutationAllowed = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (this._debugDisposed)
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A disposed RenderObject was mutated."), new DiagnosticsProperty<RenderObject>("The disposed RenderObject was", this, style: DiagnosticsTreeStyle.errorProperty) });
                    }
                    PipelineOwner? ownerLocal = this.owner;
                    if (((ownerLocal is null) || !((PipelineOwner)ownerLocal).debugDoingLayout))
                    {
                        isMutationAllowed = true;
                        return true;
                    }
                    RenderObject? activeLayoutRoot = default!;
                    (activeLayoutRoot, isMutationAllowed) = (this._debugClosestMutationRoot ?? (null, false));
                    if (isMutationAllowed)
                    {
                        return true;
                    }
                    RenderObject debugActiveLayoutLocal = RenderObject.debugActiveLayout!;
                    var culpritMethodName = (((RenderObject)debugActiveLayoutLocal).debugDoingThisLayout ? "performLayout" : "performResize");
                    var culpritFullMethodName = $"{DartRuntimePrimitives.RuntimeType(debugActiveLayoutLocal)}.{culpritMethodName}";
                    if ((activeLayoutRoot is null))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} was mutated in {culpritFullMethodName}."), new ErrorDescription("The RenderObject was mutated when none of its ancestors is actively performing layout."), new DiagnosticsProperty<RenderObject>("The RenderObject being mutated was", this, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<RenderObject>($"The RenderObject that was mutating the said {this.GetType()} was", debugActiveLayoutLocal, style: DiagnosticsTreeStyle.errorProperty) });
                    }
                    if ((object.Equals(activeLayoutRoot, this)))
                    {
                        throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary($"A {this.GetType()} was mutated in its own {culpritMethodName} implementation."), new ErrorDescription("A RenderObject must not re-dirty itself while still being laid out."), new DiagnosticsProperty<RenderObject>("The RenderObject being mutated was", this, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("Consider using the LayoutBuilder widget to dynamically change a subtree during layout.") });
                    }
                    var summary = new ErrorSummary($"A {this.GetType()} was mutated in {culpritFullMethodName}.");
                    var isMutatedByAncestor = (object.Equals(activeLayoutRoot, debugActiveLayoutLocal));
                    var description = (isMutatedByAncestor ? $"A RenderObject must not mutate its descendants in its {culpritMethodName} method." : "A RenderObject must not mutate another RenderObject from a different render subtree " + $"in its {culpritMethodName} method.");
                    throw new FlutterError(new List<DiagnosticsNode> { summary, new ErrorDescription(description), new DiagnosticsProperty<RenderObject>("The RenderObject being mutated was", this, style: DiagnosticsTreeStyle.errorProperty), new DiagnosticsProperty<RenderObject>($"The {(isMutatedByAncestor ? "ancestor " : "")}RenderObject that was mutating the said {this.GetType()} was", debugActiveLayoutLocal, style: DiagnosticsTreeStyle.errorProperty), new ErrorHint("Mutating the layout of another RenderObject may cause some RenderObjects in its subtree to be laid out more than once. " + "Consider using the LayoutBuilder widget to dynamically mutate a subtree during layout.") });
                });
            return isMutationAllowed;
            return default!;
        }
    }
    public virtual RenderObject? debugLayoutParent
    {
        get
        {
            RenderObject? layoutParent = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    layoutParent = this.parent;
                    return true;
                });
            return layoutParent;
            return default!;
        }
    }
    public virtual PipelineOwner? owner => this._owner;
    public virtual bool attached => (this.owner is not null);
    public virtual void attach(PipelineOwner owner)
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => (this._owner is null));
        _owner = owner;
        if ((this._needsLayout && (this._isRelayoutBoundary is not null)))
        {
            _needsLayout = false;
            markNeedsLayout();
        }
        if (this._needsCompositingBitsUpdate)
        {
            _needsCompositingBitsUpdate = false;
            markNeedsCompositingBitsUpdate();
        }
        if ((this._needsPaint && (((LayerHandle<ContainerLayer>)this._layerHandle).layer is not null)))
        {
            _needsPaint = false;
            markNeedsPaint();
        }
        if ((((_RenderObjectSemantics__object)this._semantics).configProvider.effective.isSemanticBoundary && ((((_RenderObjectSemantics__object)this._semantics).parentDataDirty || !((_RenderObjectSemantics__object)this._semantics).built))))
        {
            markNeedsSemanticsUpdate();
        }
    }

    public virtual void detach()
    {
        DartRuntimePrimitives.Assert(() => (this._owner is not null));
        _owner = null;
        DartRuntimePrimitives.Assert(() => ((this.parent is null) || (this.attached == this.parent!.attached)));
    }

    public virtual bool debugNeedsLayout
    {
        get
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode)
            {
                return false;
            }
            return this._needsLayout;
            return default!;
        }
    }
    public virtual bool debugDoingThisLayoutWithCallback => this._doingThisLayoutWithCallback;
    public virtual Constraints constraints
    {
        get
        {
            if ((this._constraints is null))
            {
                throw new InvalidOperationException("A RenderObject does not have any constraints before it has been laid out.");
            }
            return this._constraints!;
            return default!;
        }
    }
    public abstract void debugAssertDoesMeetConstraints();
    internal virtual bool _debugRelayoutBoundaryAlreadyMarkedNeedsLayout()
    {
        for (RenderObject? node = this; ((node is not null) && (((RenderObject)node)._isRelayoutBoundary is not null)); node = ((RenderObject)node).parent)
        {
            bool alreadyMarkedNeedsLayout = (((RenderObject)node)._needsLayout || ((RenderObject)node)._debugDoingThisLayout);
            if (!alreadyMarkedNeedsLayout)
            {
                return false;
            }
            if (DartRuntimePrimitives.RequireValue(((RenderObject)node)._isRelayoutBoundary))
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
        if (_needsLayout) FrameworkWorkCounters.Add(FrameworkWork.MarkLayoutAlreadyDirty);
        DartRuntimePrimitives.Assert(() => this._debugCanPerformMutations);
        if (this._needsLayout)
        {
            DartRuntimePrimitives.Assert(() => _debugRelayoutBoundaryAlreadyMarkedNeedsLayout());
            return;
        }
        _needsLayout = true;
        if (this.owner is PipelineOwner ownerLocal && (((this._isRelayoutBoundary ?? false))))
        {
            FrameworkWorkCounters.Add(FrameworkWork.LayoutBoundary);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Rendering.DebugLibrary.debugPrintMarkNeedsLayoutStacks)
                    {
                        global::Doroti.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"markNeedsLayout() called for {this}");
                    }
                    return true;
                });
            ((PipelineOwner)ownerLocal)._nodesNeedingLayout.Add(this);
            ownerLocal.requestVisualUpdate();
        }
        else
        {
            if ((this.parent is not null))
            {
                markParentNeedsLayout();
            }
        }
    }

    public virtual void markParentNeedsLayout()
    {
        DartRuntimePrimitives.Assert(() => this._debugCanPerformMutations);
        _needsLayout = true;
        DartRuntimePrimitives.Assert(() => (this.parent is not null));
        RenderObject parentLocal = this.parent!;
        if (!this._doingThisLayoutWithCallback)
        {
            parentLocal.markNeedsLayout();
            FrameworkWorkCounters.Add(FrameworkWork.LayoutParentPropagation);
        }
        else
        {
            DartRuntimePrimitives.Assert(() => ((RenderObject)parentLocal)._debugDoingThisLayout);
        }
        DartRuntimePrimitives.Assert(() => (object.Equals(parentLocal, this.parent)));
    }

    public virtual void markNeedsLayoutForSizedByParentChange()
    {
        markNeedsLayout();
        markParentNeedsLayout();
    }

    public virtual void scheduleInitialLayout()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => this.attached);
        DartRuntimePrimitives.Assert(() => (this.parent is null));
        DartRuntimePrimitives.Assert(() => !this.owner!._debugDoingLayout);
        DartRuntimePrimitives.Assert(() => (this._isRelayoutBoundary is null));
        _isRelayoutBoundary = true;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugCanParentUseSize = false;
                return true;
            });
        this.owner!._nodesNeedingLayout.Add(this);
    }

    internal virtual void _layoutWithoutResize()
    {
        DartRuntimePrimitives.Assert(() => this._needsLayout);
        DartRuntimePrimitives.Assert(() => (((this._isRelayoutBoundary ?? false)) || (this is RenderObjectWithLayoutCallbackMixin)));
        RenderObject? debugPreviousActiveLayout = default!;
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !this._doingThisLayoutWithCallback);
        DartRuntimePrimitives.Assert(() => (this._debugCanParentUseSize is not null));
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMutationsLocked = true;
                _debugDoingThisLayout = true;
                debugPreviousActiveLayout = _debugActiveLayout;
                _debugActiveLayout = this;
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugPrintLayouts)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Laying out (without resize) {this}");
                }
                return true;
            });
        try
        {
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
        FrameworkWorkCounters.Add(FrameworkWork.LayoutEntry);
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        if ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled))
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Rendering.DebugLibrary.debugEnhanceLayoutTimelineArguments)
                    {
                        debugTimelineArguments = toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                });
            FlutterTimeline.startSync($"{this.GetType()}", arguments: debugTimelineArguments);
        }
        DartRuntimePrimitives.Assert(() => constraints.debugAssertIsValid(isAppliedConstraint: true, informationCollector: ((InformationCollector)(() =>
        {
            List<string> stack = new global::System.Diagnostics.StackTrace(true).ToString().split("\n");
            long? targetFrame = default!;
            Pattern layoutFramePattern = new RegExp("^#[0-9]+ +Render(?:Object|Box).layout \\(");
            for (var i = 0L; (i < checked((long)(stack.Count))); i += 1L)
            {
                if ((layoutFramePattern.matchAsPrefix(stack[(int)(i)]) is not null))
                {
                    targetFrame = (i + 1L);
                }
                else
                {
                    if ((targetFrame is not null))
                    {
                        long targetFrame__112422__value112715 = DartRuntimePrimitives.RequireValue(targetFrame);
                        break;
                    }
                }
            }
            if (((targetFrame is not null) && (DartRuntimePrimitives.RequireValue(targetFrame) < checked((long)(stack.Count)))))
            {
                long targetFrame__112422__value112799 = DartRuntimePrimitives.RequireValue(targetFrame);
                Pattern targetFramePattern = new RegExp("^#[0-9]+ +(.+)$");
                Match? targetFrameMatch = targetFramePattern.matchAsPrefix(stack[(int)(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetFrame__112422__value112799)))]);
                string? problemFunction = ((((targetFrameMatch is not null) && (targetFrameMatch.groupCount > 0L))) ? targetFrameMatch.group(1L) : stack[(int)(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(targetFrame__112422__value112799)))].Trim());
                return new List<DiagnosticsNode> { new ErrorDescription($"These invalid constraints were provided to {this.GetType()}'s layout() " + "function by the following function, which probably computed the " + "invalid constraints in question:\n" + $"  {problemFunction}") };
            }
            return new List<DiagnosticsNode>();
            return default;
        }))));
        DartRuntimePrimitives.Assert(() => !this._debugDoingThisResize);
        DartRuntimePrimitives.Assert(() => !this._debugDoingThisLayout);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugCanParentUseSize = parentUsesSize;
                return true;
            });
        _isRelayoutBoundary = (((!parentUsesSize || this.sizedByParent) || ((Constraints)constraints).isTight) || (this.parent is null));
        if ((!this._needsLayout && (object.Equals(constraints, this._constraints))))
        {
            FrameworkWorkCounters.Add(FrameworkWork.LayoutFastPath);
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingThisResize = this.sizedByParent;
                    _debugDoingThisLayout = !this.sizedByParent;
                    RenderObject? debugPreviousActiveLayout = _debugActiveLayout;
                    _debugActiveLayout = this;
                    debugResetSize();
                    _debugActiveLayout = debugPreviousActiveLayout;
                    _debugDoingThisLayout = false;
                    _debugDoingThisResize = false;
                    return true;
                });
            if ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled))
            {
                FlutterTimeline.finishSync();
            }
            return;
        }
        if (FrameworkWorkCounters.Enabled && _needsLayout && object.Equals(constraints, _constraints))
            FrameworkWorkCounters.Add(FrameworkWork.LayoutDirtySameConstraints);
        _constraints = constraints;
        FrameworkWorkCounters.Add(FrameworkWork.LayoutWork);
        DartRuntimePrimitives.Assert(() => !this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => !this._doingThisLayoutWithCallback);
        DartRuntimePrimitives.Assert(() =>
            {
                _debugMutationsLocked = true;
                if (global::Doroti.Framework.Rendering.DebugLibrary.debugPrintLayouts)
                {
                    global::Doroti.Framework.Foundation.PrintLibrary.debugPrint($"Laying out ({(this.sizedByParent ? "with separate resize" : "with resize allowed")}) {this}");
                }
                return true;
            });
        if (this.sizedByParent)
        {
            DartRuntimePrimitives.Assert(() =>
                {
                    _debugDoingThisResize = true;
                    return true;
                });
            try
            {
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
        if ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Framework.Rendering.DebugLibrary.debugProfileLayoutsEnabled))
        {
            FlutterTimeline.finishSync();
        }
    }

    public virtual void debugResetSize()
    {
    }

    public virtual bool sizedByParent => false;
    public abstract void performResize();
    public abstract void performLayout();
    public virtual void invokeLayoutCallback<T>(Action<T> callback) where T : Constraints
    {
        DartRuntimePrimitives.Assert(() => this._debugMutationsLocked);
        DartRuntimePrimitives.Assert(() => this._debugDoingThisLayout);
        DartRuntimePrimitives.Assert(() => !this._doingThisLayoutWithCallback);
        _doingThisLayoutWithCallback = true;
        try
        {
            this.owner!._enableMutationsToDirtySubtrees(((Action)(() =>
            {
                callback(((T?)(object?)this.constraints)!);
            })));
        }
        finally
        {
            _doingThisLayoutWithCallback = false;
        }
    }

    public virtual void runLayoutCallback()
    {
        DartRuntimePrimitives.Assert(() => debugDoingThisLayout);
        invokeLayoutCallback<Constraints>((Constraints _) =>
            (this as IRenderLayoutCallback ?? throw new InvalidOperationException(
                $"{GetType().FullName} scheduled a layout callback without implementing {nameof(IRenderLayoutCallback)}."))
            .layoutCallback());
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

    public virtual bool debugDoingThisPaint => this._debugDoingThisPaint;
    public static RenderObject? debugActivePaint => _debugActivePaint;
    public virtual bool isRepaintBoundary => false;
    public virtual void debugRegisterRepaintBoundaryPaint(bool includedParent = true, bool includedChild = false)
    {
    }

    public virtual bool alwaysNeedsCompositing => false;
    public virtual OffsetLayer updateCompositedLayer(OffsetLayer? oldLayer)
    {
        DartRuntimePrimitives.Assert(() => this.isRepaintBoundary);
        return (oldLayer ?? new OffsetLayer());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual ContainerLayer? layer
    {
        get
        {
            DartRuntimePrimitives.Assert(() => ((!this.isRepaintBoundary || (((LayerHandle<ContainerLayer>)this._layerHandle).layer is null)) || (((LayerHandle<ContainerLayer>)this._layerHandle).layer is OffsetLayer)));
            return ((LayerHandle<ContainerLayer>)this._layerHandle).layer;
            return default!;
        }
        set
        {
            var newLayer = value;
            DartRuntimePrimitives.Assert(() => !this.isRepaintBoundary);
            this._layerHandle.layer = newLayer;
        }
    }
    public virtual ContainerLayer? debugLayer
    {
        get
        {
            ContainerLayer? result = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    result = ((LayerHandle<ContainerLayer>)this._layerHandle).layer;
                    return true;
                });
            return result;
            return default!;
        }
    }
    public virtual void markNeedsCompositingBitsUpdate()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        if (this._needsCompositingBitsUpdate)
        {
            return;
        }
        _needsCompositingBitsUpdate = true;
        RenderObject? parentLocal = this.parent;
        if ((parentLocal is not null))
        {
            if (((RenderObject)parentLocal)._needsCompositingBitsUpdate)
            {
                return;
            }
            if ((((!this._wasRepaintBoundary || !this.isRepaintBoundary)) && !((RenderObject)parentLocal).isRepaintBoundary))
            {
                parentLocal.markNeedsCompositingBitsUpdate();
                return;
            }
        }
        this.owner?._nodesNeedingCompositingBitsUpdate.Add(this);
    }

    public virtual bool needsCompositing
    {
        get
        {
            DartRuntimePrimitives.Assert(() => !this._needsCompositingBitsUpdate);
            return this._needsCompositing;
            return default!;
        }
    }
    internal virtual void _updateCompositingBits()
    {
        if (!this._needsCompositingBitsUpdate)
        {
            return;
        }
        bool oldNeedsCompositing = this._needsCompositing;
        _needsCompositing = false;
        visitChildren(((Action<RenderObject>)((child) =>
        {
            child._updateCompositingBits();
            if (((RenderObject)child).needsCompositing)
            {
                _needsCompositing = true;
            }
        })));
        if ((this.isRepaintBoundary || this.alwaysNeedsCompositing))
        {
            _needsCompositing = true;
        }
        if ((!this.isRepaintBoundary && this._wasRepaintBoundary))
        {
            _needsPaint = false;
            _needsCompositedLayerUpdate = false;
            this.owner?._nodesNeedingPaint.removeWhere(((t) => DartRuntimePrimitives.Identical(t, this)));
            _needsCompositingBitsUpdate = false;
            markNeedsPaint();
        }
        else
        {
            if ((oldNeedsCompositing != this._needsCompositing))
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
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode)
            {
                return false;
            }
            return this._needsPaint;
            return default!;
        }
    }
    public virtual bool debugNeedsCompositedLayerUpdate
    {
        get
        {
            if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kDebugMode)
            {
                return false;
            }
            return this._needsCompositedLayerUpdate;
            return default!;
        }
    }
    public virtual void markNeedsPaint()
    {
        FrameworkWorkCounters.Add(FrameworkWork.MarkPaint);
        if (_needsPaint) FrameworkWorkCounters.Add(FrameworkWork.MarkPaintAlreadyDirty);
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => ((this.owner is null) || !this.owner!.debugDoingPaint));
        if (this._needsPaint)
        {
            return;
        }
        _needsPaint = true;
        if ((this.isRepaintBoundary && this._wasRepaintBoundary))
        {
            FrameworkWorkCounters.Add(FrameworkWork.PaintBoundary);
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Rendering.DebugLibrary.debugPrintMarkNeedsPaintStacks)
                    {
                        global::Doroti.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"markNeedsPaint() called for {this}");
                    }
                    return true;
                });
            DartRuntimePrimitives.Assert(() => (((LayerHandle<ContainerLayer>)this._layerHandle).layer is OffsetLayer));
            if ((this.owner is not null))
            {
                this.owner!._nodesNeedingPaint.Add(this);
                this.owner!.requestVisualUpdate();
            }
        }
        else
        {
            if ((this.parent is not null))
            {
                this.parent!.markNeedsPaint();
                FrameworkWorkCounters.Add(FrameworkWork.PaintParentPropagation);
            }
            else
            {
                DartRuntimePrimitives.Assert(() =>
                    {
                        if (global::Doroti.Framework.Rendering.DebugLibrary.debugPrintMarkNeedsPaintStacks)
                        {
                            global::Doroti.Framework.Foundation.AssertionsLibrary.debugPrintStack(label: $"markNeedsPaint() called for {this} (root of render tree)");
                        }
                        return true;
                    });
                this.owner?.requestVisualUpdate();
            }
        }
    }

    public virtual void markNeedsCompositedLayerUpdate()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => ((this.owner is null) || !this.owner!.debugDoingPaint));
        if ((this._needsCompositedLayerUpdate || this._needsPaint))
        {
            return;
        }
        _needsCompositedLayerUpdate = true;
        if ((this.isRepaintBoundary && this._wasRepaintBoundary))
        {
            DartRuntimePrimitives.Assert(() => (((LayerHandle<ContainerLayer>)this._layerHandle).layer is not null));
            if ((this.owner is not null))
            {
                this.owner!._nodesNeedingPaint.Add(this);
                this.owner!.requestVisualUpdate();
            }
        }
        else
        {
            markNeedsPaint();
        }
    }

    internal virtual void _skippedPaintingOnLayer()
    {
        DartRuntimePrimitives.Assert(() => this.attached);
        DartRuntimePrimitives.Assert(() => this.isRepaintBoundary);
        DartRuntimePrimitives.Assert(() => (this._needsPaint || this._needsCompositedLayerUpdate));
        DartRuntimePrimitives.Assert(() => (((LayerHandle<ContainerLayer>)this._layerHandle).layer is not null));
        DartRuntimePrimitives.Assert(() => !((LayerHandle<ContainerLayer>)this._layerHandle).layer!.attached);
        RenderObject? node = this.parent;
        while ((node is not null))
        {
            if (((RenderObject)node).isRepaintBoundary)
            {
                if ((((RenderObject)node)._layerHandle.layer is null))
                {
                    break;
                }
                if (((RenderObject)node)._layerHandle.layer!.attached)
                {
                    break;
                }
                node._needsPaint = true;
            }
            node = ((RenderObject)node).parent;
        }
    }

    public virtual void scheduleInitialPaint(ContainerLayer rootLayer)
    {
        DartRuntimePrimitives.Assert(() => rootLayer.attached);
        DartRuntimePrimitives.Assert(() => this.attached);
        DartRuntimePrimitives.Assert(() => (this.parent is null));
        DartRuntimePrimitives.Assert(() => !this.owner!._debugDoingPaint);
        DartRuntimePrimitives.Assert(() => this.isRepaintBoundary);
        DartRuntimePrimitives.Assert(() => (((LayerHandle<ContainerLayer>)this._layerHandle).layer is null));
        this._layerHandle.layer = rootLayer;
        DartRuntimePrimitives.Assert(() => this._needsPaint);
        this.owner!._nodesNeedingPaint.Add(this);
    }

    public virtual void replaceRootLayer(OffsetLayer rootLayer)
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => rootLayer.attached);
        DartRuntimePrimitives.Assert(() => this.attached);
        DartRuntimePrimitives.Assert(() => (this.parent is null));
        DartRuntimePrimitives.Assert(() => !this.owner!._debugDoingPaint);
        DartRuntimePrimitives.Assert(() => this.isRepaintBoundary);
        DartRuntimePrimitives.Assert(() => (((LayerHandle<ContainerLayer>)this._layerHandle).layer is not null));
        ((LayerHandle<ContainerLayer>)this._layerHandle).layer!.detach();
        this._layerHandle.layer = rootLayer;
        markNeedsPaint();
    }

    internal virtual void _paintWithContext(PaintingContext context, Offset offset)
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() =>
            {
                if (this._debugDoingThisPaint)
                {
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Tried to paint a RenderObject reentrantly."), describeForError("The following RenderObject was already being painted when it was " + "painted again"), new ErrorDescription("Since this typically indicates an infinite recursion, it is " + "disallowed.") });
                }
                return true;
            });
        if (this._needsLayout)
        {
            return;
        }
        if ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled))
        {
            DartMap<string, string>? debugTimelineArguments = default!;
            DartRuntimePrimitives.Assert(() =>
                {
                    if (global::Doroti.Framework.Rendering.DebugLibrary.debugEnhancePaintTimelineArguments)
                    {
                        debugTimelineArguments = toDiagnosticsNode().toTimelineArguments();
                    }
                    return true;
                });
            FlutterTimeline.startSync($"{this.GetType()}", arguments: debugTimelineArguments);
        }
        DartRuntimePrimitives.Assert(() =>
            {
                if (this._needsCompositingBitsUpdate)
                {
                    RenderObject? parentLocal = this.parent;
                    if ((parentLocal is not null))
                    {
                        var visitedByParent = false;
                        parentLocal.visitChildren(((Action<RenderObject>)((child) =>
                        {
                            if ((object.Equals(child, this)))
                            {
                                visitedByParent = true;
                            }
                        })));
                        if (!visitedByParent)
                        {
                            throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("A RenderObject was not visited by the parent's visitChildren " + "during paint."), parentLocal.describeForError("The parent was"), describeForError("The child that was not visited was"), new ErrorDescription("A RenderObject with children must implement visitChildren and " + "call the visitor exactly once for each child; it also should not " + "paint children that were removed with dropChild."), new ErrorHint("This usually indicates an error in the Flutter framework itself.") });
                        }
                    }
                    throw new FlutterError(new List<DiagnosticsNode> { new ErrorSummary("Tried to paint a RenderObject before its compositing bits were " + "updated."), describeForError("The following RenderObject was marked as having dirty compositing " + "bits at the time that it was painted"), new ErrorDescription("A RenderObject that still has dirty compositing bits cannot be " + "painted because this indicates that the tree has not yet been " + "properly configured for creating the layer tree."), new ErrorHint("This usually indicates an error in the Flutter framework itself.") });
                }
                return true;
            });
        RenderObject? debugLastActivePaint = default!;
        DartRuntimePrimitives.Assert(() =>
            {
                _debugDoingThisPaint = true;
                debugLastActivePaint = _debugActivePaint;
                _debugActivePaint = this;
                DartRuntimePrimitives.Assert(() => (!this.isRepaintBoundary || (((LayerHandle<ContainerLayer>)this._layerHandle).layer is not null)));
                return true;
            });
        _needsPaint = false;
        _needsCompositedLayerUpdate = false;
        _wasRepaintBoundary = this.isRepaintBoundary;
        try
        {
            paint(context, offset);
            DartRuntimePrimitives.Assert(() => !this._needsLayout);
            DartRuntimePrimitives.Assert(() => !this._needsPaint);
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
        if ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && global::Doroti.Framework.Rendering.DebugLibrary.debugProfilePaintsEnabled))
        {
            FlutterTimeline.finishSync();
        }
    }

    public abstract global::Doroti.Ui.Rect paintBounds { get; }
    public virtual void debugPaint(PaintingContext context, Offset offset)
    {
    }

    public virtual void paint(PaintingContext context, Offset offset)
    {
    }

    public virtual void applyPaintTransform(RenderObject child, Matrix4 transform)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
    }

    public virtual bool paintsChild(RenderObject child)
    {
        DartRuntimePrimitives.Assert(() => (object.Equals(((RenderObject)child).parent, this)));
        return true;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual Matrix4 getTransformTo(RenderObject? target)
    {
        DartRuntimePrimitives.Assert(() => this.attached);
        List<RenderObject>? fromPath = default!;
        List<RenderObject>? toPath = default!;
        var @from = this;
        RenderObject to = (target ?? this.owner!.rootNode!);
        while (!DartRuntimePrimitives.Identical(@from, to))
        {
            long fromDepth = ((RenderObject)@from).depth;
            long toDepth = ((RenderObject)to).depth;
            if ((fromDepth >= toDepth))
            {
                RenderObject fromParent = (((RenderObject)@from).parent ?? throw new FlutterError($"{target} and {this} are not in the same render tree."));
                (fromPath ??= new List<RenderObject> { this }).Add(fromParent);
                @from = fromParent;
            }
            if ((fromDepth <= toDepth))
            {
                RenderObject toParent = (((RenderObject)to).parent ?? throw new FlutterError($"{target} and {this} are not in the same render tree."));
                DartRuntimePrimitives.Assert(() => (target is not null));
                (toPath ??= new List<RenderObject> { target! }).Add(toParent);
                to = toParent;
            }
        }
        Matrix4? fromTransform = default!;
        if ((fromPath is not null))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(fromPath.Count)) > 1L));
            fromTransform = Matrix4.identity();
            long lastIndex = ((target is null) ? (checked((long)(fromPath.Count)) - 2L) : (checked((long)(fromPath.Count)) - 1L));
            for (var index = lastIndex; (index > 0L); index -= 1L)
            {
                fromPath[(int)(index)].applyPaintTransform(fromPath[(int)((index - 1L))], fromTransform);
            }
        }
        if ((toPath is null))
        {
            return (fromTransform ?? Matrix4.identity());
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(toPath.Count)) > 1L));
        var toTransform = Matrix4.identity();
        for (long indexLocal = (checked((long)(toPath.Count)) - 1L); (indexLocal > 0L); indexLocal -= 1L)
        {
            toPath[(int)(indexLocal)].applyPaintTransform(toPath[(int)((indexLocal - 1L))], toTransform);
        }
        if ((toTransform.invert() == 0L))
        {
            return Matrix4.zero();
        }
        return ((((Func<Matrix4?>)(() =>
{
    var __cascade = fromTransform;
    __cascade.multiply(toTransform);
    return __cascade;
}))()) ?? toTransform);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual global::Doroti.Ui.Rect? describeApproximatePaintClip(RenderObject child) => null;
    public virtual global::Doroti.Ui.Rect? describeSemanticsClip(RenderObject? child) => null;
    public virtual void scheduleInitialSemantics()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => this.attached);
        DartRuntimePrimitives.Assert(() => (this.parent is null));
        DartRuntimePrimitives.Assert(() => !this.owner!._debugDoingSemantics);
        DartRuntimePrimitives.Assert(() => (((_RenderObjectSemantics__object)this._semantics).parentDataDirty || !((_RenderObjectSemantics__object)this._semantics).built));
        DartRuntimePrimitives.Assert(() => (this.owner!._semanticsOwner is not null));
        this.owner!._nodesNeedingSemanticsUpdate.Add(this);
        this.owner!._nodesNeedingSemanticsGeometryUpdate.Add(this);
        this.owner!.requestVisualUpdate();
    }

    public virtual void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config)
    {
    }

    public virtual void sendSemanticsEvent(global::Doroti.Framework.Semantics.SemanticsEvent semanticsEvent)
    {
        if ((this.owner!.semanticsOwner is null))
        {
            return;
        }
        global::Doroti.Framework.Semantics.SemanticsNode? node = ((_RenderObjectSemantics__object)this._semantics).cachedSemanticsNode;
        if (((node is not null) && !((global::Doroti.Framework.Semantics.SemanticsNode)node).isMergedIntoParent))
        {
            node.sendEvent(semanticsEvent);
        }
        else
        {
            if ((this.parent is not null))
            {
                this.parent!.sendSemanticsEvent(semanticsEvent);
            }
        }
    }

    public abstract global::Doroti.Ui.Rect semanticBounds { get; }
    public virtual bool debugNeedsSemanticsUpdate
    {
        get
        {
            if (global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
            {
                return false;
            }
            return ((_RenderObjectSemantics__object)this._semantics).parentDataDirty;
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Semantics.SemanticsNode? debugSemantics
    {
        get
        {
            if ((!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode && ((_RenderObjectSemantics__object)this._semantics).built))
            {
                return ((_RenderObjectSemantics__object)this._semantics).cachedSemanticsNode;
            }
            return null;
            return default!;
        }
    }
    public virtual void clearSemantics()
    {
        this._semantics.clear();
        visitChildren(((Action<RenderObject>)((child) =>
        {
            child.clearSemantics();
        })));
    }

    public virtual void markNeedsSemanticsUpdate()
    {
        DartRuntimePrimitives.Assert(() => !this._debugDisposed);
        DartRuntimePrimitives.Assert(() => (!this.attached || !this.owner!._debugDoingSemantics));
        if ((!this.attached || (this.owner!._semanticsOwner is null)))
        {
            return;
        }
        this._semantics.markNeedsUpdate();
    }

    public virtual void visitChildrenForSemantics(Action<RenderObject> visitor)
    {
        visitChildren((Action<RenderObject>)visitor);
    }

    public virtual void assembleSemanticsNode(global::Doroti.Framework.Semantics.SemanticsNode node, global::Doroti.Framework.Semantics.SemanticsConfiguration config, IEnumerable<global::Doroti.Framework.Semantics.SemanticsNode> children)
    {
        node.updateWith(config: config, childrenInInversePaintOrder: ((List<global::Doroti.Framework.Semantics.SemanticsNode>?)(object?)children)!);
    }

    public virtual void handleEvent(global::Doroti.Framework.Gestures.PointerEvent @event, HitTestEntry<HitTestTarget> entry)
    {
    }

    public virtual string toStringShort()
    {
        string header = global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this);
        if (!global::Doroti.Framework.Foundation.ConstantsLibrary.kReleaseMode)
        {
            if (this._debugDisposed)
            {
                header += " DISPOSED";
                return header;
            }
            var count = 0L;
            for (RenderObject? node = this; ((node is not null) && !((((RenderObject)node)._isRelayoutBoundary ?? false))); node = ((RenderObject)node).parent)
            {
                if ((((RenderObject)node)._isRelayoutBoundary is null))
                {
                    count = -1L;
                    break;
                }
                count += 1L;
            }
            if ((count > 0L))
            {
                header += $" relayoutBoundary=up{count}";
            }
            if (this._needsLayout)
            {
                header += " NEEDS-LAYOUT";
            }
            if (this._needsPaint)
            {
                header += " NEEDS-PAINT";
            }
            if (this._needsCompositingBitsUpdate)
            {
                header += " NEEDS-COMPOSITING-BITS-UPDATE";
            }
            if (!this.attached)
            {
                header += " DETACHED";
            }
        }
        return header;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string ToString(DiagnosticLevel minLevel = DiagnosticLevel.info) => toStringShort();
    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = "", DiagnosticLevel minLevel = DiagnosticLevel.debug, long wrapWidth = 65)
    {
        return _withDebugActiveLayoutCleared(((Func<string>)(() => base.toStringDeep(prefixLineOne: prefixLineOne, prefixOtherLines: prefixOtherLines, minLevel: minLevel, wrapWidth: wrapWidth))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual string toStringShallow(string joiner = ", ", DiagnosticLevel minLevel = DiagnosticLevel.debug)
    {
        return _withDebugActiveLayoutCleared(((Func<string>)(() => base.toStringShallow(joiner: joiner, minLevel: minLevel))));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new FlagProperty("needsCompositing", value: this._needsCompositing, ifTrue: "needs compositing"));
        properties.add(new DiagnosticsProperty<object?>("creator", this.debugCreator, defaultValue: null, level: DiagnosticLevel.debug));
        properties.add(new DiagnosticsProperty<ParentData>("parentData", this.parentData, tooltip: (((this._debugCanParentUseSize ?? false)) ? "can use size" : null), missingIfNull: true));
        properties.add(new DiagnosticsProperty<Constraints>("constraints", this._constraints, missingIfNull: true));
        properties.add(new DiagnosticsProperty<ContainerLayer>("layer", ((LayerHandle<ContainerLayer>)this._layerHandle).layer, defaultValue: null));
        properties.add(new DiagnosticsProperty<global::Doroti.Framework.Semantics.SemanticsNode>("semantics node", this.debugSemantics, defaultValue: null));
        properties.add(new FlagProperty("isBlockingSemanticsOfPreviouslyPaintedNodes", value: ((_RenderObjectSemantics__object)this._semantics).configProvider.effective.isBlockingSemanticsOfPreviouslyPaintedNodes, ifTrue: "blocks semantics of earlier render objects below the common boundary"));
        properties.add(new FlagProperty("isSemanticBoundary", value: ((_RenderObjectSemantics__object)this._semantics).configProvider.effective.isSemanticBoundary, ifTrue: "semantic boundary"));
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren() => new List<DiagnosticsNode>();
    public virtual void showOnScreen(RenderObject? descendant = null, Rect? rect = null, Duration duration = default, Curve curve = default!)
    {
        this.parent?.showOnScreen(descendant: (descendant ?? this), rect: rect, duration: duration, curve: curve);
    }

    public virtual DiagnosticsNode describeForError(string name, DiagnosticsTreeStyle style = DiagnosticsTreeStyle.shallow)
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

public interface RenderObjectWithChildMixin<ChildType> : IRenderObjectWithChild where ChildType : RenderObject
{
    ChildType? _child { get; set; }

    public bool debugValidateChild(RenderObject child);
    public ChildType? child { get; set; }
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
    public virtual void runLayoutCallback()
    {
        DartRuntimePrimitives.Assert(() => debugDoingThisLayout);
        invokeLayoutCallback<Constraints>((Constraints _) => layoutCallback());
        _needsRebuild = false;
    }

    public virtual void scheduleLayoutCallback()
    {
        if (this._needsRebuild)
        {
            DartRuntimePrimitives.Assert(() => debugNeedsLayout);
            return;
        }
        _needsRebuild = true;
        owner?._nodesNeedingLayout.Add(this);
        base.markNeedsLayout();
    }

}

public interface ContainerParentDataMixin<ChildType> where ChildType : RenderObject
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

public interface ContainerRenderObjectMixin<ChildType, ParentDataType> : IContainerRenderObject where ChildType : RenderObject where ParentDataType : ContainerParentDataMixin<ChildType>
{
    long _childCount { get; set; }
    ChildType? _firstChild { get; set; }
    ChildType? _lastChild { get; set; }

    public bool _debugUltimatePreviousSiblingOf(ChildType child, ChildType? equals = null);
    public bool _debugUltimateNextSiblingOf(ChildType child, ChildType? equals = null);
    public long childCount { get; }
    public bool debugValidateChild(RenderObject child);
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
    void IContainerRenderObject.insert(RenderObject child, RenderObject? after) => insert((ChildType)child, (ChildType?)after);
    void IContainerRenderObject.move(RenderObject child, RenderObject? after) => move((ChildType)child, (ChildType?)after);
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
    global::Doroti.Framework.Semantics.SemanticsProperties _properties { get; set; }
    bool _container { get; set; }
    bool _explicitChildNodes { get; set; }
    bool _excludeSemantics { get; set; }
    bool _blockUserActions { get; set; }
    Locale? _localeForSubtree { get; set; }
    global::Doroti.Framework.Semantics.AttributedString? _attributedLabel { get; set; }
    global::Doroti.Framework.Semantics.AttributedString? _attributedValue { get; set; }
    global::Doroti.Framework.Semantics.AttributedString? _attributedIncreasedValue { get; set; }
    global::Doroti.Framework.Semantics.AttributedString? _attributedDecreasedValue { get; set; }
    global::Doroti.Framework.Semantics.AttributedString? _attributedHint { get; set; }
    TextDirection? _textDirection { get; set; }

    public void initSemanticsAnnotations(global::Doroti.Framework.Semantics.SemanticsProperties properties, bool container, bool explicitChildNodes, bool excludeSemantics, bool blockUserActions, Locale? localeForSubtree, TextDirection? textDirection);
    public global::Doroti.Framework.Semantics.SemanticsProperties properties { get; set; }
    public bool container { get; set; }
    public bool explicitChildNodes { get; set; }
    public bool excludeSemantics { get; set; }
    public bool blockUserActions { get; set; }
    public global::Doroti.Ui.Locale? localeForSubtree { get; set; }
    public void _updateAttributedFields(global::Doroti.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedLabel(global::Doroti.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedValue(global::Doroti.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedIncreasedValue(global::Doroti.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedDecreasedValue(global::Doroti.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Framework.Semantics.AttributedString? _effectiveAttributedHint(global::Doroti.Framework.Semantics.SemanticsProperties value);
    public global::Doroti.Ui.TextDirection? textDirection { get; set; }
    public void visitChildrenForSemantics(Action<RenderObject> visitor);
    public void describeSemanticsConfiguration(global::Doroti.Framework.Semantics.SemanticsConfiguration config);
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
    public virtual global::Doroti.Framework.Semantics.AccessibilityFocusBlockType? accessibilityFocusBlockType { get; private set; }
    public virtual bool explicitChildNodes { get; private set; } = default!;
    public virtual HashSet<global::Doroti.Framework.Semantics.SemanticsTag>? tagsForChildren { get; private set; }
    public virtual Locale? localeForChildren { get; private set; }

    internal _SemanticsParentData__object(bool mergeIntoParent, bool blocksUserActions, bool explicitChildNodes, HashSet<global::Doroti.Framework.Semantics.SemanticsTag>? tagsForChildren, Locale? localeForChildren, global::Doroti.Framework.Semantics.AccessibilityFocusBlockType? accessibilityFocusBlockType)
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
        if (__other is null) return false;
        return (((((((__other is _SemanticsParentData__object) && (((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).mergeIntoParent == this.mergeIntoParent)) && (((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).blocksUserActions == this.blocksUserActions)) && (((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).explicitChildNodes == this.explicitChildNodes)) && (object.Equals(((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).localeForChildren, this.localeForChildren))) && (object.Equals(((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).accessibilityFocusBlockType, this.accessibilityFocusBlockType))) && global::Doroti.Framework.Foundation.CollectionsLibrary.setEquals<global::Doroti.Framework.Semantics.SemanticsTag>(((_SemanticsParentData__object)((_SemanticsParentData__object)__other)).tagsForChildren, this.tagsForChildren));
    }

    public override int GetHashCode()
    {
        return FoundationRuntimePorts.ObjectHash(this.mergeIntoParent, this.blocksUserActions, this.explicitChildNodes, this.localeForChildren, this.accessibilityFocusBlockType, Dart_coreLibrary.hashAllUnordered((this.tagsForChildren ?? new HashSet<global::Doroti.Framework.Semantics.SemanticsTag>())));
        return default!;
    }
}

public class _SemanticsConfigurationProvider__object
{
    internal virtual RenderObject _renderObject { get; private set; } = default!;
    internal virtual bool _isEffectiveConfigWritable { get; set; } = false;
    internal virtual global::Doroti.Framework.Semantics.SemanticsConfiguration? _originalConfiguration { get; set; } = default;
    internal virtual global::Doroti.Framework.Semantics.SemanticsConfiguration? _effectiveConfiguration { get; set; } = default;

    internal _SemanticsConfigurationProvider__object(RenderObject _renderObject)
    {
        this._renderObject = _renderObject;
    }

    public virtual bool wasSemanticsBoundary => (this._originalConfiguration?.isSemanticBoundary ?? false);
    public virtual global::Doroti.Framework.Semantics.SemanticsConfiguration effective
    {
        get
        {
            return (this._effectiveConfiguration ?? this.original);
            return default!;
        }
    }
    public virtual global::Doroti.Framework.Semantics.SemanticsConfiguration original
    {
        get
        {
            if ((this._originalConfiguration is null))
            {
                _effectiveConfiguration = _originalConfiguration = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
                this._renderObject.describeSemanticsConfiguration(this._originalConfiguration!);
                DartRuntimePrimitives.Assert(() => (!this._originalConfiguration!.explicitChildNodes || (this._originalConfiguration!.childConfigurationsDelegate is null)));
            }
            return this._originalConfiguration!;
            return default!;
        }
    }
    public virtual void updateConfig(Action<global::Doroti.Framework.Semantics.SemanticsConfiguration> callback)
    {
        if (!this._isEffectiveConfigWritable)
        {
            _effectiveConfiguration = this.original.copy();
            _isEffectiveConfigWritable = true;
        }
        callback(this._effectiveConfiguration!);
    }

    public virtual void absorbAll(IEnumerable<global::Doroti.Framework.Semantics.SemanticsConfiguration> configs)
    {
        updateConfig(((Action<global::Doroti.Framework.Semantics.SemanticsConfiguration>)((config) =>
        {
            configs.forEach(((global::Doroti.Framework.Semantics.SemanticsConfiguration)config).absorb);
        })));
    }

    public virtual void reset()
    {
        _effectiveConfiguration = this.original;
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

    public abstract global::Doroti.Framework.Semantics.SemanticsConfiguration? configToMergeUp { get; }
    public abstract _RenderObjectSemantics__object owner { get; }
    public abstract void markSiblingConfigurationConflict(bool conflict);
}

internal class _IncompleteSemanticsFragment__object : _SemanticsFragment__object
{
    private global::Doroti.Framework.Semantics.SemanticsConfiguration? __field_configToMergeUp = default!;
    public override global::Doroti.Framework.Semantics.SemanticsConfiguration? configToMergeUp { get => __field_configToMergeUp; }
    private _RenderObjectSemantics__object __field_owner = default!;
    public override _RenderObjectSemantics__object owner { get => __field_owner; }

    internal _IncompleteSemanticsFragment__object(global::Doroti.Framework.Semantics.SemanticsConfiguration configToMergeUp, _RenderObjectSemantics__object owner)
    {
        this.__field_configToMergeUp = configToMergeUp;
        this.__field_owner = owner;
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
    public virtual global::Doroti.Framework.Semantics.SemanticsNode? cachedSemanticsNode { get; set; } = default;
    public virtual List<global::Doroti.Framework.Semantics.SemanticsNode> semanticsNodes { get; private set; } = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
    public virtual List<_SemanticsFragment__object> mergeUp { get; private set; } = new List<_SemanticsFragment__object>();
    internal virtual List<_RenderObjectSemantics__object> _children { get; private set; } = new List<_RenderObjectSemantics__object>();
    public virtual List<List<_SemanticsFragment__object>> siblingMergeGroups { get; private set; } = new List<List<_SemanticsFragment__object>>();
    internal virtual DartMap<global::Doroti.Framework.Semantics.SemanticsNode, List<_SemanticsFragment__object>> _producedSiblingNodesAndOwners { get; private set; } = new DartMap<global::Doroti.Framework.Semantics.SemanticsNode, List<_SemanticsFragment__object>>();
    public virtual _SemanticsParentData__object? parentData { get; set; } = default;
    public virtual _SemanticsGeometry__object? geometry { get; set; } = default;
    public virtual _SemanticsConfigurationProvider__object configProvider { get; private set; } = default!;
    public virtual _RenderObjectSemantics__object? parentInSemanticsTree { get; set; } = default;
    internal virtual object _currentTreeShapeToken { get; set; } = new object();
    public virtual _RenderObjectSemantics__object? firstAncestorNodeWithCleanGeometry { get; set; } = default;

    internal _RenderObjectSemantics__object(RenderObject renderObject)
    {
        this.renderObject = renderObject;
        this.configProvider = new _SemanticsConfigurationProvider__object(renderObject);
    }

    public override _RenderObjectSemantics__object owner => this;
    public virtual _RenderObjectSemantics__object? parent => ((RenderObject)this.renderObject).parent?._semantics;
    public virtual bool parentDataDirty
    {
        get
        {
            if (this.isRoot)
            {
                return false;
            }
            return (this.parentData is null);
            return default!;
        }
    }
    public virtual bool geometryDirty
    {
        get
        {
            if (this.isRoot)
            {
                return false;
            }
            return (this.geometry is null);
            return default!;
        }
    }
    public virtual void computeAncestorInfo(object treeShapeToken)
    {
        if ((object.Equals(treeShapeToken, this._currentTreeShapeToken)))
        {
            return;
        }
        _currentTreeShapeToken = treeShapeToken;
        if (this.isRoot)
        {
            firstAncestorNodeWithCleanGeometry = this;
            return;
        }
        firstAncestorNodeWithCleanGeometry = null;
        if (this.parentDataDirty)
        {
            return;
        }
        _RenderObjectSemantics__object? next = default!;
        if (this.shouldFormSemanticsNode)
        {
            if (!this.geometryDirty)
            {
                firstAncestorNodeWithCleanGeometry = this;
            }
            next = this.parentInSemanticsTree;
        }
        else
        {
            next = this;
            while ((!next!.parentDataDirty && !((_RenderObjectSemantics__object)next).shouldFormSemanticsNode))
            {
                next = ((_RenderObjectSemantics__object)next).parent;
                DartRuntimePrimitives.Assert(() => (next is not null));
            }
        }
        if ((next is null))
        {
            return;
        }
        if ((this.firstAncestorNodeWithCleanGeometry is null))
        {
            next.computeAncestorInfo(treeShapeToken);
            firstAncestorNodeWithCleanGeometry = ((_RenderObjectSemantics__object)next).firstAncestorNodeWithCleanGeometry;
        }
    }

    public override global::Doroti.Framework.Semantics.SemanticsConfiguration? configToMergeUp => (this.shouldFormSemanticsNode ? null : ((_SemanticsConfigurationProvider__object)this.configProvider).effective);
    public virtual bool contributesToSemanticsTree
    {
        get
        {
            return (((((_SemanticsConfigurationProvider__object)this.configProvider).effective.hasBeenAnnotated || this._containsIncompleteFragment) || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary) || this.isRoot);
            return default!;
        }
    }
    public virtual bool isRoot => (this.parent is null);
    internal virtual bool _needsMergingSiblingNodesIntoSelf
    {
        get
        {
            return (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isMergingSemanticsOfDescendants && (checked((long)(this._producedSiblingNodesAndOwners.Count)) != 0));
            return default!;
        }
    }
    public virtual bool shouldFormSemanticsNode
    {
        get
        {
            if (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary)
            {
                return true;
            }
            if (this.isRoot)
            {
                return true;
            }
            if (!this.contributesToSemanticsTree)
            {
                return false;
            }
            DartRuntimePrimitives.Assert(() => (this.parentData is not null));
            return (this.parentData!.explicitChildNodes || this._hasSiblingConflict);
            return default!;
        }
    }
    public static void debugCheckForParentData(RenderObject root)
    {
        void debugCheckParentDataNotDirty(_RenderObjectSemantics__object semantics)
        {
            DartRuntimePrimitives.Assert(() => !((_RenderObjectSemantics__object)semantics).parentDataDirty);
            semantics._getNonBlockedChildren().forEach(debugCheckParentDataNotDirty);
        }
        debugCheckParentDataNotDirty(((RenderObject)root)._semantics);
    }

    public static void debugCheckForBuilds(_RenderObjectSemantics__object node)
    {
        DartRuntimePrimitives.Assert(() => ((_RenderObjectSemantics__object)node).built);
        ((_RenderObjectSemantics__object)node)._children.forEach(debugCheckForBuilds);
    }

    public virtual bool isBlockingPreviousSibling
    {
        get
        {
            if ((this._blocksPreviousSibling is not null))
            {
                return DartRuntimePrimitives.RequireValue(this._blocksPreviousSibling);
            }
            _blocksPreviousSibling = ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isBlockingSemanticsOfPreviouslyPaintedNodes;
            if (DartRuntimePrimitives.RequireValue(this._blocksPreviousSibling))
            {
                return true;
            }
            if (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary)
            {
                return false;
            }
            this.renderObject.visitChildrenForSemantics(((Action<RenderObject>)((child) =>
            {
                _RenderObjectSemantics__object childSemantics = ((RenderObject)child)._semantics;
                if (((_RenderObjectSemantics__object)childSemantics).isBlockingPreviousSibling)
                {
                    _blocksPreviousSibling = true;
                }
            })));
            return DartRuntimePrimitives.RequireValue(this._blocksPreviousSibling);
            return default!;
        }
    }
    public static bool shouldDrop(global::Doroti.Framework.Semantics.SemanticsNode node) => ((global::Doroti.Framework.Semantics.SemanticsNode)node).isInvisible;
    public virtual void markNeedsBuild()
    {
        built = false;
        if ((!this.parentDataDirty && !this.shouldFormSemanticsNode))
        {
            return;
        }
        foreach (List<_SemanticsFragment__object> @group in this.siblingMergeGroups)
        {
            foreach (_RenderObjectSemantics__object semantics in @group.OfType<_RenderObjectSemantics__object>())
            {
                if (((_RenderObjectSemantics__object)semantics).parentDataDirty)
                {
                    continue;
                }
                if (!((_RenderObjectSemantics__object)semantics).shouldFormSemanticsNode)
                {
                    semantics.markNeedsBuild();
                }
            }
        }
    }

    public virtual void updateChildren()
    {
        DartRuntimePrimitives.Assert(() => ((this.parentData is not null) || this.isRoot));
        this.configProvider.reset();
        HashSet<global::Doroti.Framework.Semantics.SemanticsTag>? tagsForChildrenLocal = _getTagsForChildren();
        bool explicitChildNodesForChildren = ((this.isRoot || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.explicitChildNodes) || ((!this.contributesToSemanticsTree && ((this.parentData?.explicitChildNodes ?? true)))));
        bool blocksUserAction = (((this.parentData?.blocksUserActions ?? false)) || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isBlockingUserActions);
        global::Doroti.Framework.Semantics.AccessibilityFocusBlockType accessibilityFocusBlockTypeLocal = default!;
        if ((object.Equals(this.parentData?.accessibilityFocusBlockType, global::Doroti.Framework.Semantics.AccessibilityFocusBlockType.blockSubtree)))
        {
            accessibilityFocusBlockTypeLocal = global::Doroti.Framework.Semantics.AccessibilityFocusBlockType.blockSubtree;
        }
        else
        {
            accessibilityFocusBlockTypeLocal = ((_SemanticsConfigurationProvider__object)this.configProvider).effective.accessibilityFocusBlockType;
        }
        global::Doroti.Ui.Locale? localeForChildrenLocal = (((_SemanticsConfigurationProvider__object)this.configProvider).effective.localeForSubtree ?? this.parentData?.localeForChildren);
        this.siblingMergeGroups.Clear();
        this.mergeUp.Clear();
        var childParentData = new _SemanticsParentData__object(mergeIntoParent: (((this.parentData?.mergeIntoParent ?? false)) || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isMergingSemanticsOfDescendants), blocksUserActions: blocksUserAction, accessibilityFocusBlockType: accessibilityFocusBlockTypeLocal, localeForChildren: localeForChildrenLocal, explicitChildNodes: explicitChildNodesForChildren, tagsForChildren: tagsForChildrenLocal);
        (List<_SemanticsFragment__object>, List<List<_SemanticsFragment__object>>) result = _collectChildMergeUpAndSiblingGroup(childParentData);
        this.mergeUp.AddRange(result.Item1);
        this.siblingMergeGroups.AddRange(result.Item2);
        HashSet<_RenderObjectSemantics__object> oldChildren = this._children.toSet();
        this._children.Clear();
        if (!this.contributesToSemanticsTree)
        {
            return;
        }
        _marksConflictsInMergeGroup(this.mergeUp, isMergeUp: true);
        this.siblingMergeGroups.forEach(__fragments => this._marksConflictsInMergeGroup(__fragments));
        IEnumerable<global::Doroti.Framework.Semantics.SemanticsConfiguration> mergeUpConfigs = this.mergeUp.map<_SemanticsFragment__object, global::Doroti.Framework.Semantics.SemanticsConfiguration?>(((fragment) => ((_SemanticsFragment__object)fragment).configToMergeUp)).OfType<global::Doroti.Framework.Semantics.SemanticsConfiguration>();
        this.configProvider.absorbAll(mergeUpConfigs);
        this.mergeUp.Clear();
        this.mergeUp.Add(this);
        foreach (_RenderObjectSemantics__object childSemantics in result.Item1.OfType<_RenderObjectSemantics__object>())
        {
            DartRuntimePrimitives.Assert(() => ((_RenderObjectSemantics__object)childSemantics).contributesToSemanticsTree);
            if (((_RenderObjectSemantics__object)childSemantics).shouldFormSemanticsNode)
            {
                foreach (_RenderObjectSemantics__object child in ((_RenderObjectSemantics__object)childSemantics)._children)
                {
                    child.parentInSemanticsTree = childSemantics;
                }
                if (((_RenderObjectSemantics__object)childSemantics).geometryDirty)
                {
                    ((RenderObject)this.renderObject).owner!._nodesNeedingSemanticsGeometryUpdate.Add(((_RenderObjectSemantics__object)childSemantics).renderObject);
                }
                this._children.Add(childSemantics);
            }
            else
            {
                this._children.AddRange(((_RenderObjectSemantics__object)childSemantics)._children);
                this.siblingMergeGroups.AddRange(((_RenderObjectSemantics__object)childSemantics).siblingMergeGroups);
            }
        }
        if ((this.isRoot || ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary))
        {
            foreach (_RenderObjectSemantics__object childLocal in this._children)
            {
                childLocal.parentInSemanticsTree = this;
            }
        }
        oldChildren.removeAll(this._children);
        foreach (var removedChild in oldChildren)
        {
            if ((object.Equals(((_RenderObjectSemantics__object)removedChild).parentInSemanticsTree, this)))
            {
                removedChild.parentInSemanticsTree = null;
            }
        }
        HashSet<global::Doroti.Framework.Semantics.SemanticsTag>? tags = this.parentData?.tagsForChildren;
        if ((tags is not null))
        {
            DartRuntimePrimitives.Assert(() => (checked((long)(tags.Count)) != 0));
            this.configProvider.updateConfig(((Action<global::Doroti.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                tags.forEach(((global::Doroti.Framework.Semantics.SemanticsConfiguration)config).addTagForChildren);
            })));
        }
        if ((!object.Equals(accessibilityFocusBlockTypeLocal, ((_SemanticsConfigurationProvider__object)this.configProvider).effective.accessibilityFocusBlockType)))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.accessibilityFocusBlockType = accessibilityFocusBlockTypeLocal;
            })));
        }
        if ((blocksUserAction != ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isBlockingUserActions))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.isBlockingUserActions = blocksUserAction;
            })));
        }
        if ((!object.Equals(localeForChildrenLocal, ((_SemanticsConfigurationProvider__object)this.configProvider).effective.locale)))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.locale = localeForChildrenLocal;
            })));
        }
        if ((!object.Equals(accessibilityFocusBlockTypeLocal, global::Doroti.Framework.Semantics.AccessibilityFocusBlockType.none)))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.isFocused = null;
            })));
        }
    }

    internal virtual List<_RenderObjectSemantics__object> _getNonBlockedChildren()
    {
        var result = new List<_RenderObjectSemantics__object>();
        this.renderObject.visitChildrenForSemantics(((Action<RenderObject>)((renderChild) =>
        {
            if (((RenderObject)renderChild)._semantics.isBlockingPreviousSibling)
            {
                result.Clear();
            }
            result.Add(((RenderObject)renderChild)._semantics);
        })));
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual HashSet<global::Doroti.Framework.Semantics.SemanticsTag>? _getTagsForChildren()
    {
        if (this.contributesToSemanticsTree)
        {
            return ((_SemanticsConfigurationProvider__object)this.configProvider).original.tagsForChildren?.toSet();
        }
        HashSet<global::Doroti.Framework.Semantics.SemanticsTag>? result = default!;
        if ((((_SemanticsConfigurationProvider__object)this.configProvider).original.tagsForChildren is not null))
        {
            result = ((_SemanticsConfigurationProvider__object)this.configProvider).original.tagsForChildren!.toSet();
        }
        if ((this.parentData?.tagsForChildren is not null))
        {
            if ((result is null))
            {
                result = this.parentData!.tagsForChildren;
            }
            else
            {
                result.UnionWith(this.parentData!.tagsForChildren!);
            }
        }
        return result;
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual (List<_SemanticsFragment__object>, List<List<_SemanticsFragment__object>>) _collectChildMergeUpAndSiblingGroup(_SemanticsParentData__object childParentData)
    {
        var mergeUpLocal = new List<_SemanticsFragment__object>();
        var siblingMergeGroupsLocal = new List<List<_SemanticsFragment__object>>();
        var childConfigurations = new List<global::Doroti.Framework.Semantics.SemanticsConfiguration>();
        Func<List<global::Doroti.Framework.Semantics.SemanticsConfiguration>, global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult>? childConfigurationsDelegateLocal = ((_SemanticsConfigurationProvider__object)this.configProvider).effective.childConfigurationsDelegate;
        var hasChildConfigurationsDelegate = (childConfigurationsDelegateLocal is not null);
        var configToFragment = new DartMap<global::Doroti.Framework.Semantics.SemanticsConfiguration, _SemanticsFragment__object>();
        bool needsToMakeIncompleteFragmentAssumption = (hasChildConfigurationsDelegate && ((_SemanticsParentData__object)childParentData).explicitChildNodes);
        _SemanticsParentData__object effectiveChildParentData = default!;
        if (needsToMakeIncompleteFragmentAssumption)
        {
            effectiveChildParentData = new _SemanticsParentData__object(mergeIntoParent: ((_SemanticsParentData__object)childParentData).mergeIntoParent, blocksUserActions: ((_SemanticsParentData__object)childParentData).blocksUserActions, accessibilityFocusBlockType: ((_SemanticsParentData__object)childParentData).accessibilityFocusBlockType, explicitChildNodes: false, tagsForChildren: ((_SemanticsParentData__object)childParentData).tagsForChildren, localeForChildren: ((_SemanticsParentData__object)childParentData).localeForChildren);
        }
        else
        {
            effectiveChildParentData = childParentData;
        }
        foreach (_RenderObjectSemantics__object childSemantics in _getNonBlockedChildren())
        {
            DartRuntimePrimitives.Assert(() => !((_RenderObjectSemantics__object)childSemantics).renderObject._needsLayout);
            childSemantics._didUpdateParentData(effectiveChildParentData);
            foreach (_SemanticsFragment__object fragment in ((_RenderObjectSemantics__object)childSemantics).mergeUp)
            {
                if ((hasChildConfigurationsDelegate && (((_SemanticsFragment__object)fragment).configToMergeUp is not null)))
                {
                    childConfigurations.Add(((_SemanticsFragment__object)fragment).configToMergeUp!);
                    configToFragment[((_SemanticsFragment__object)fragment).configToMergeUp!] = fragment;
                }
                else
                {
                    mergeUpLocal.Add(fragment);
                }
            }
            if (!((_RenderObjectSemantics__object)childSemantics).contributesToSemanticsTree)
            {
                siblingMergeGroupsLocal.AddRange(((_RenderObjectSemantics__object)childSemantics).siblingMergeGroups);
            }
        }
        _containsIncompleteFragment = false;
        DartRuntimePrimitives.Assert(() => ((childConfigurationsDelegateLocal is not null) || (checked((long)(configToFragment.Count)) == 0)));
        if (hasChildConfigurationsDelegate)
        {
            global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult result = childConfigurationsDelegateLocal(childConfigurations);
            mergeUpLocal.AddRange(((global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult)result).mergeUp.map<global::Doroti.Framework.Semantics.SemanticsConfiguration, _SemanticsFragment__object>(((config) =>
            {
                _SemanticsFragment__object? fragmentLocal = configToFragment.GetValueOrDefault(config);
                if ((fragmentLocal is not null))
                {
                    return fragmentLocal;
                }
                _containsIncompleteFragment = true;
                return new _IncompleteSemanticsFragment__object(config, this);
                return default;
            })));
            foreach (IEnumerable<global::Doroti.Framework.Semantics.SemanticsConfiguration> @group in ((global::Doroti.Framework.Semantics.ChildSemanticsConfigurationsResult)result).siblingMergeGroups)
            {
                siblingMergeGroupsLocal.Add(@group.map<global::Doroti.Framework.Semantics.SemanticsConfiguration, _SemanticsFragment__object>(((config) =>
                {
                    _SemanticsFragment__object? fragmentAlternate = configToFragment.GetValueOrDefault(config);
                    if ((fragmentAlternate is not null))
                    {
                        return fragmentAlternate;
                    }
                    _containsIncompleteFragment = true;
                    return new _IncompleteSemanticsFragment__object(config, this);
                    return default;
                })).ToList());
            }
        }
        if ((!this._containsIncompleteFragment && needsToMakeIncompleteFragmentAssumption))
        {
            mergeUpLocal.Clear();
            siblingMergeGroupsLocal.Clear();
            foreach (_RenderObjectSemantics__object childSemanticsLocal in _getNonBlockedChildren())
            {
                DartRuntimePrimitives.Assert(() => ((_SemanticsParentData__object)childParentData).explicitChildNodes);
                childSemanticsLocal._didUpdateParentData(childParentData);
                mergeUpLocal.AddRange(((_RenderObjectSemantics__object)childSemanticsLocal).mergeUp);
                if (!((_RenderObjectSemantics__object)childSemanticsLocal).contributesToSemanticsTree)
                {
                    siblingMergeGroupsLocal.AddRange(((_RenderObjectSemantics__object)childSemanticsLocal).siblingMergeGroups);
                }
            }
        }
        return (mergeUpLocal, siblingMergeGroupsLocal);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _didUpdateParentData(_SemanticsParentData__object newParentData)
    {
        if ((object.Equals(this.parentData, newParentData)))
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
        DartRuntimePrimitives.Assert(() => !this.geometryDirty);
        if (this.isRoot)
        {
            if ((!object.Equals(this.geometry?.rect, ((RenderObject)this.renderObject).semanticBounds)))
            {
                markNeedsBuild();
            }
            geometry = _SemanticsGeometry__object.CreateRoot(((RenderObject)this.renderObject).semanticBounds);
        }
        _updateChildGeometry(onlyDirtyChildren: true);
    }

    internal virtual void _updateChildGeometry(bool onlyDirtyChildren = false)
    {
        DartRuntimePrimitives.Assert(() => (this.geometry is not null));
        _SemanticsGeometry__object parentGeometry = this.geometry!;
        foreach (_RenderObjectSemantics__object childLocal in this._children)
        {
            if (childLocal.renderObject is RenderBox childBox && !childBox.hasSize)
            {
                continue;
            }
            if ((onlyDirtyChildren && !((_RenderObjectSemantics__object)childLocal).geometryDirty))
            {
                continue;
            }
            _SemanticsGeometry__object childGeometry = _SemanticsGeometry__object.computeChildGeometry(parentPaintClipRect: ((_SemanticsGeometry__object)parentGeometry).paintClipRect, parentSemanticsClipRect: ((_SemanticsGeometry__object)parentGeometry).semanticsClipRect, parentTransform: null, parent: this, child: childLocal);
            childLocal._updateGeometry(newGeometry: childGeometry);
        }
        foreach (_RenderObjectSemantics__object explicitSiblingChild in this.siblingMergeGroups.expand(((group) => group)).OfType<_RenderObjectSemantics__object>().expand(((siblingChild) => (((_RenderObjectSemantics__object)siblingChild).shouldFormSemanticsNode ? new List<_RenderObjectSemantics__object> { siblingChild } : ((_RenderObjectSemantics__object)siblingChild)._children))))
        {
            if (explicitSiblingChild.renderObject is RenderBox siblingBox && !siblingBox.hasSize)
            {
                continue;
            }
            if ((onlyDirtyChildren && !((_RenderObjectSemantics__object)explicitSiblingChild).geometryDirty))
            {
                continue;
            }
            _SemanticsGeometry__object childGeometryLocal = _SemanticsGeometry__object.computeChildGeometry(parentPaintClipRect: ((_SemanticsGeometry__object)parentGeometry).paintClipRect, parentSemanticsClipRect: ((_SemanticsGeometry__object)parentGeometry).semanticsClipRect, parentTransform: ((_SemanticsGeometry__object)parentGeometry).transform, parent: this, child: explicitSiblingChild);
            explicitSiblingChild._updateGeometry(newGeometry: childGeometryLocal);
        }
    }

    internal virtual void _updateGeometry(_SemanticsGeometry__object newGeometry)
    {
        _SemanticsGeometry__object? currentGeometry = this.geometry;
        geometry = newGeometry;
        if ((currentGeometry is not null))
        {
            bool isSemanticsHidden = (((_SemanticsConfigurationProvider__object)this.configProvider).original.isHidden || ((!((this.parentData?.mergeIntoParent ?? false)) && ((_SemanticsGeometry__object)newGeometry).hidden)));
            var sizeChanged = (!object.Equals(((_SemanticsGeometry__object)currentGeometry).rect.size, ((_SemanticsGeometry__object)newGeometry).rect.size));
            var visibilityChanged = (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isHidden != isSemanticsHidden);
            if ((!sizeChanged && !visibilityChanged))
            {
                return;
            }
        }
        markNeedsBuild();
        _updateChildGeometry();
    }

    public virtual void ensureSemanticsNode()
    {
        DartRuntimePrimitives.Assert(() => this.shouldFormSemanticsNode);
        if (!this.built)
        {
            _buildSemantics(usedSemanticsIds: new HashSet<long>());
        }
        else
        {
            DartRuntimePrimitives.Assert(() => this.built);
            _buildSemanticsSubtree(usedSemanticsIds: new HashSet<long>());
        }
    }

    internal virtual void _buildSemantics(HashSet<long> usedSemanticsIds)
    {
        DartRuntimePrimitives.Assert(() => this.shouldFormSemanticsNode);
        if ((this.cachedSemanticsNode is not null))
        {
            foreach (global::Doroti.Framework.Semantics.SemanticsNode node in this.semanticsNodes)
            {
                if ((!object.Equals(node, this.cachedSemanticsNode)))
                {
                    node.tags = null;
                }
            }
        }
        if (!this.built)
        {
            _produceSemanticsNode(usedSemanticsIds: usedSemanticsIds);
        }
        DartRuntimePrimitives.Assert(() => this.built);
        global::Doroti.Framework.Semantics.SemanticsNode producedNode = this.cachedSemanticsNode!;
        foreach (global::Doroti.Framework.Semantics.SemanticsNode nodeLocal in this.semanticsNodes)
        {
            if ((!object.Equals(nodeLocal, producedNode)))
            {
                if ((this.parentData?.tagsForChildren is not null))
                {
                    nodeLocal.tags ??= new HashSet<global::Doroti.Framework.Semantics.SemanticsTag>();
                    ((global::Doroti.Framework.Semantics.SemanticsNode)nodeLocal).tags!.UnionWith(this.parentData!.tagsForChildren!);
                }
                else
                {
                    if (((((long?)(((global::Doroti.Framework.Semantics.SemanticsNode)nodeLocal).tags?.Count)) is { } __count240857 ? __count240857 == 0 : (bool?)null) ?? false))
                    {
                        nodeLocal.tags = null;
                    }
                }
            }
        }
    }

    internal virtual void _buildSemanticsSubtree(HashSet<long> usedSemanticsIds)
    {
        var children = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
        foreach (_RenderObjectSemantics__object child in this._children)
        {
            if (child.geometry is null)
            {
                continue;
            }
            if (((_RenderObjectSemantics__object)child).parentDataDirty)
            {
                continue;
            }
            DartRuntimePrimitives.Assert(() => ((_RenderObjectSemantics__object)child).shouldFormSemanticsNode);
            if (((((_RenderObjectSemantics__object)child).cachedSemanticsNode is not null) && usedSemanticsIds.Contains(((_RenderObjectSemantics__object)child).cachedSemanticsNode!.id)))
            {
                child.markNeedsBuild();
                child.cachedSemanticsNode = null;
            }
            child._buildSemantics(usedSemanticsIds: usedSemanticsIds);
            children.AddRange(((_RenderObjectSemantics__object)child).semanticsNodes);
        }
        global::Doroti.Framework.Semantics.SemanticsNode node = this.cachedSemanticsNode!;
        children.removeWhere(shouldDrop);
        bool isSemanticsHidden = (((_SemanticsConfigurationProvider__object)this.configProvider).original.isHidden || ((!((this.parentData?.mergeIntoParent ?? false)) && this.geometry!.hidden)));
        if ((((_SemanticsConfigurationProvider__object)this.configProvider).effective.isHidden != isSemanticsHidden))
        {
            this.configProvider.updateConfig(((Action<global::Doroti.Framework.Semantics.SemanticsConfiguration>)((config) =>
            {
                config.isHidden = isSemanticsHidden;
            })));
        }
        if (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary)
        {
            if (this._needsMergingSiblingNodesIntoSelf)
            {
                var innerNode = new global::Doroti.Framework.Semantics.SemanticsNode(showOnScreen: () => this.renderObject.showOnScreen());
                this.renderObject.assembleSemanticsNode(innerNode, ((_SemanticsConfigurationProvider__object)this.configProvider).effective, children);
                var configLocal = ((Func<global::Doroti.Framework.Semantics.SemanticsConfiguration>)(() =>
{
    var __cascade = new global::Doroti.Framework.Semantics.SemanticsConfiguration();
    __cascade.isSemanticBoundary = true;
    __cascade.isMergingSemanticsOfDescendants = true;
    return __cascade;
}))();
                node.updateWith(config: configLocal, childrenInInversePaintOrder: new List<global::Doroti.Framework.Semantics.SemanticsNode> { innerNode });
            }
            else
            {
                this.renderObject.assembleSemanticsNode(node, ((_SemanticsConfigurationProvider__object)this.configProvider).effective, children);
            }
        }
        else
        {
            DartRuntimePrimitives.Assert(() => !((_SemanticsConfigurationProvider__object)this.configProvider).effective.isMergingSemanticsOfDescendants);
            node.updateWith(config: ((_SemanticsConfigurationProvider__object)this.configProvider).effective, childrenInInversePaintOrder: children);
        }
    }

    internal virtual void _produceSemanticsNode(HashSet<long> usedSemanticsIds)
    {
        DartRuntimePrimitives.Assert(() => !this.built);
        this.semanticsNodes.Clear();
        this._producedSiblingNodesAndOwners.Clear();
        global::Doroti.Framework.Semantics.SemanticsNode node = cachedSemanticsNode ??= _createSemanticsNode();
        ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = node;
    __cascade.isMergedIntoParent = ((this.parentData?.mergeIntoParent ?? false));
    __cascade.tags = this.parentData?.tagsForChildren;
    return __cascade;
}))();
        _updateSemanticsNodeGeometry();
        _mergeSiblingGroup(usedSemanticsIds);
        _buildSemanticsSubtree(usedSemanticsIds: usedSemanticsIds);
        this.semanticsNodes.Add(node);
        if (!this._needsMergingSiblingNodesIntoSelf)
        {
            this.semanticsNodes.AddRange(this._producedSiblingNodesAndOwners.Keys);
        }
        built = true;
    }

    internal virtual global::Doroti.Framework.Semantics.SemanticsNode _createSemanticsNode()
    {
        if (this.isRoot)
        {
            return global::Doroti.Framework.Semantics.SemanticsNode.CreateRoot(showOnScreen: () => ((_RenderObjectSemantics__object)this.owner).renderObject.showOnScreen(), owner: ((_RenderObjectSemantics__object)this.owner).renderObject.owner!.semanticsOwner!);
        }
        return new global::Doroti.Framework.Semantics.SemanticsNode(showOnScreen: () => ((_RenderObjectSemantics__object)this.owner).renderObject.showOnScreen());
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal virtual void _mergeSiblingGroup(HashSet<long> usedSemanticsIds)
    {
        foreach (List<_SemanticsFragment__object> @group in this.siblingMergeGroups)
        {
            global::Doroti.Framework.Semantics.SemanticsConfiguration? configuration = default!;
            global::Doroti.Framework.Semantics.SemanticsNode? node = default!;
            var explicitChildren = new List<_RenderObjectSemantics__object>();
            foreach (var fragmentLocal in @group)
            {
                if ((fragmentLocal is _RenderObjectSemantics__object))
                {
                    _RenderObjectSemantics__object fragment__244890__as244923 = (_RenderObjectSemantics__object)fragmentLocal;
                    if (((_RenderObjectSemantics__object)((_RenderObjectSemantics__object)fragment__244890__as244923)).shouldFormSemanticsNode)
                    {
                        explicitChildren.Add(((_RenderObjectSemantics__object)fragment__244890__as244923));
                        DartRuntimePrimitives.Assert(() => (((_RenderObjectSemantics__object)((_RenderObjectSemantics__object)fragment__244890__as244923)).configToMergeUp is null));
                        continue;
                    }
                    explicitChildren.AddRange(((_RenderObjectSemantics__object)((_RenderObjectSemantics__object)fragment__244890__as244923))._children);
                }
                if ((((_SemanticsFragment__object)fragmentLocal).configToMergeUp is not null))
                {
                    fragmentLocal.mergesToSibling = true;
                    node ??= ((_SemanticsFragment__object)fragmentLocal).owner.cachedSemanticsNode;
                    configuration ??= new global::Doroti.Framework.Semantics.SemanticsConfiguration();
                    configuration.absorb(((_SemanticsFragment__object)fragmentLocal).configToMergeUp!);
                }
            }
            var childrenNodes = new List<global::Doroti.Framework.Semantics.SemanticsNode>();
            foreach (var explicitChild in explicitChildren)
            {
                explicitChild._buildSemantics(usedSemanticsIds: usedSemanticsIds);
                childrenNodes.AddRange(((_RenderObjectSemantics__object)explicitChild).semanticsNodes);
            }
            if ((configuration is not null))
            {
                if (((node is null) || usedSemanticsIds.Contains(((global::Doroti.Framework.Semantics.SemanticsNode)node).id)))
                {
                    node = new global::Doroti.Framework.Semantics.SemanticsNode(showOnScreen: () => this.renderObject.showOnScreen());
                }
                usedSemanticsIds.Add(((global::Doroti.Framework.Semantics.SemanticsNode)node).id);
                foreach (var fragmentAlternate in @group)
                {
                    if ((((_SemanticsFragment__object)fragmentAlternate).configToMergeUp is not null))
                    {
                        ((_SemanticsFragment__object)fragmentAlternate).owner.built = true;
                        ((_SemanticsFragment__object)fragmentAlternate).owner.cachedSemanticsNode = node;
                    }
                }
                node.updateWith(config: configuration, childrenInInversePaintOrder: childrenNodes);
                this._producedSiblingNodesAndOwners[DartRuntimePrimitives.RequireReference(node)] = @group;
                HashSet<global::Doroti.Framework.Semantics.SemanticsTag> tagsLocal = @group.map<_SemanticsFragment__object, HashSet<global::Doroti.Framework.Semantics.SemanticsTag>?>(((fragment) => ((_SemanticsFragment__object)fragment).owner.parentData!.tagsForChildren)).OfType<HashSet<global::Doroti.Framework.Semantics.SemanticsTag>>().expand(((tagsLocal) => tagsLocal)).toSet();
                if ((checked((long)(tagsLocal.Count)) != 0))
                {
                    if ((((global::Doroti.Framework.Semantics.SemanticsNode)node).tags is null))
                    {
                        node.tags = tagsLocal;
                    }
                    else
                    {
                        ((global::Doroti.Framework.Semantics.SemanticsNode)node).tags!.UnionWith(tagsLocal);
                    }
                }
                node.isMergedIntoParent = (this.parentData?.mergeIntoParent ?? false);
            }
        }
        _updateSiblingNodesGeometries();
    }

    internal virtual void _updateSemanticsNodeGeometry()
    {
        global::Doroti.Framework.Semantics.SemanticsNode node = this.cachedSemanticsNode!;
        _SemanticsGeometry__object nodeGeometry = this.geometry!;
        ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = node;
    __cascade.rect = ((_SemanticsGeometry__object)nodeGeometry).rect;
    __cascade.transform = ((_SemanticsGeometry__object)nodeGeometry).transform;
    __cascade.parentSemanticsClipRect = ((_SemanticsGeometry__object)nodeGeometry).semanticsClipRect;
    __cascade.parentPaintClipRect = ((_SemanticsGeometry__object)nodeGeometry).paintClipRect;
    return __cascade;
}))();
    }

    internal virtual void _updateSiblingNodesGeometries()
    {
        _SemanticsGeometry__object mainGeometry = this.geometry!;
        foreach (MapEntry<global::Doroti.Framework.Semantics.SemanticsNode, List<_SemanticsFragment__object>> entry in this._producedSiblingNodesAndOwners.entries)
        {
            global::Doroti.Ui.Rect? rectLocal = default!;
            global::Doroti.Ui.Rect? semanticsClipRectLocal = default!;
            global::Doroti.Ui.Rect? paintClipRectLocal = default!;
            foreach (_SemanticsFragment__object fragment in entry.value)
            {
                if (((_SemanticsFragment__object)fragment).owner.shouldFormSemanticsNode)
                {
                    continue;
                }
                _SemanticsGeometry__object parentGeometry = _SemanticsGeometry__object.computeChildGeometry(parentTransform: ((_SemanticsGeometry__object)mainGeometry).transform, parentSemanticsClipRect: ((_SemanticsGeometry__object)mainGeometry).semanticsClipRect, parentPaintClipRect: ((_SemanticsGeometry__object)mainGeometry).paintClipRect, parent: this, child: ((_SemanticsFragment__object)fragment).owner);
                global::Doroti.Ui.Rect rectInFragmentOwnerCoordinates = (((_SemanticsGeometry__object)parentGeometry).semanticsClipRect?.intersect(((_SemanticsFragment__object)fragment).owner.renderObject.semanticBounds) ?? ((_SemanticsFragment__object)fragment).owner.renderObject.semanticBounds);
                global::Doroti.Ui.Rect rectInParentCoordinates = MatrixUtils.transformRect(((_SemanticsGeometry__object)parentGeometry).transform, rectInFragmentOwnerCoordinates);
                rectLocal = (rectLocal?.expandToInclude(rectInParentCoordinates) ?? rectInParentCoordinates);
                if ((((_SemanticsGeometry__object)parentGeometry).semanticsClipRect is not null))
                {
                    global::Doroti.Ui.Rect rectAlternate = MatrixUtils.transformRect(((_SemanticsGeometry__object)parentGeometry).transform, DartRuntimePrimitives.RequireValue(((_SemanticsGeometry__object)parentGeometry).semanticsClipRect));
                    semanticsClipRectLocal = (semanticsClipRectLocal?.intersect(rectAlternate) ?? rectAlternate);
                }
                if ((((_SemanticsGeometry__object)parentGeometry).paintClipRect is not null))
                {
                    global::Doroti.Ui.Rect rectNested = MatrixUtils.transformRect(((_SemanticsGeometry__object)parentGeometry).transform, DartRuntimePrimitives.RequireValue(((_SemanticsGeometry__object)parentGeometry).paintClipRect));
                    paintClipRectLocal = (paintClipRectLocal?.intersect(rectNested) ?? rectNested);
                }
            }
            global::Doroti.Framework.Semantics.SemanticsNode node = entry.key;
            ((Func<global::Doroti.Framework.Semantics.SemanticsNode>)(() =>
{
    var __cascade = node;
    __cascade.rect = DartRuntimePrimitives.RequireValue(rectLocal);
    __cascade.transform = null;
    __cascade.parentSemanticsClipRect = semanticsClipRectLocal;
    __cascade.parentPaintClipRect = paintClipRectLocal;
    return __cascade;
}))();
        }
    }

    public virtual void markNeedsUpdate()
    {
        ((RenderObject)this.renderObject).owner!._nodesNeedingSemanticsGeometryUpdate.Add(this.renderObject);
        global::Doroti.Framework.Semantics.SemanticsNode? producedSemanticsNode = this.cachedSemanticsNode;
        bool wasSemanticsBoundaryLocal = ((producedSemanticsNode is not null) && ((_SemanticsConfigurationProvider__object)this.configProvider).wasSemanticsBoundary);
        this.configProvider.clear();
        _containsIncompleteFragment = false;
        var mayProduceSiblingNodes = (((_SemanticsConfigurationProvider__object)this.configProvider).effective.childConfigurationsDelegate is not null);
        bool isEffectiveSemanticsBoundary = (((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary && wasSemanticsBoundaryLocal);
        RenderObject node = this.renderObject;
        while (((((RenderObject)node).parent is not null) && ((mayProduceSiblingNodes || !isEffectiveSemanticsBoundary))))
        {
            if ((((!object.Equals(node, this.renderObject)) && ((RenderObject)node)._semantics.parentDataDirty) && !mayProduceSiblingNodes))
            {
                break;
            }
            ((RenderObject)node)._semantics.parentData = null;
            ((RenderObject)node)._semantics._blocksPreviousSibling = null;
            if (isEffectiveSemanticsBoundary)
            {
                mayProduceSiblingNodes = false;
            }
            mayProduceSiblingNodes |= (((RenderObject)node)._semantics.configProvider.effective.childConfigurationsDelegate is not null);
            node = ((RenderObject)node).parent!;
            isEffectiveSemanticsBoundary = (((RenderObject)node)._semantics.configProvider.effective.isSemanticBoundary && ((RenderObject)node)._semantics.built);
        }
        if ((((!object.Equals(node, this.renderObject)) && (producedSemanticsNode is not null)) && ((RenderObject)node)._semantics.parentDataDirty))
        {
            ((RenderObject)this.renderObject).owner!._nodesNeedingSemanticsUpdate.Remove(this.renderObject);
        }
        if ((!((RenderObject)node)._semantics.parentDataDirty || ((RenderObject)node)._semantics.isRoot))
        {
            if ((((RenderObject)this.renderObject).owner is not null))
            {
                DartRuntimePrimitives.Assert(() => (((RenderObject)node)._semantics.configProvider.effective.isSemanticBoundary || (((RenderObject)node).parent is null)));
                if (((RenderObject)this.renderObject).owner!._nodesNeedingSemanticsUpdate.Add(node))
                {
                    ((RenderObject)this.renderObject).owner!.requestVisualUpdate();
                }
            }
        }
    }

    internal virtual void _marksConflictsInMergeGroup(List<_SemanticsFragment__object> mergeGroup, bool isMergeUp = false)
    {
        var hasSiblingConflict = new HashSet<_SemanticsFragment__object>();
        for (var i = 0L; (i < checked((long)(mergeGroup.Count))); i += 1L)
        {
            _SemanticsFragment__object fragment = mergeGroup[(int)(i)];
            fragment.markSiblingConfigurationConflict(false);
            if ((((_SemanticsFragment__object)fragment).configToMergeUp is null))
            {
                continue;
            }
            if ((isMergeUp && !((_SemanticsConfigurationProvider__object)this.configProvider).original.isCompatibleWith(((_SemanticsFragment__object)fragment).configToMergeUp)))
            {
                hasSiblingConflict.Add(fragment);
            }
            var siblingLength = i;
            for (var j = 0L; (j < siblingLength); j += 1L)
            {
                _SemanticsFragment__object siblingFragment = mergeGroup[(int)(j)];
                if (!((_SemanticsFragment__object)fragment).configToMergeUp!.isCompatibleWith(((_SemanticsFragment__object)siblingFragment).configToMergeUp))
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
        this.mergeUp.Clear();
        this.siblingMergeGroups.Clear();
        this._children.Clear();
        this.semanticsNodes.Clear();
        this.configProvider.clear();
    }

    public virtual List<DiagnosticsNode> debugDescribeChildren()
    {
        return this._children.map<_RenderObjectSemantics__object, DiagnosticsNode>(((child) => ((Diagnosticable)child).toDiagnosticsNode())).ToList();
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties)
    {
        DiagnosticableDefaults.debugFillProperties(properties);
        properties.add(new StringProperty("owner", global::Doroti.Framework.Foundation.DiagnosticsLibrary.describeIdentity(this.renderObject)));
        properties.add(new FlagProperty("noParentData", value: this.parentDataDirty, ifTrue: "NO PARENT DATA"));
        properties.add(new FlagProperty("geometry", value: this.geometryDirty, ifTrue: "NO GEOMETRY"));
        properties.add(new FlagProperty("semanticsBlock", value: ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isBlockingSemanticsOfPreviouslyPaintedNodes, ifTrue: "BLOCK PREVIOUS"));
        if ((!this.parentDataDirty && this.contributesToSemanticsTree))
        {
            string semanticsNodeStatus = default!;
            if (this.built)
            {
                semanticsNodeStatus = $"formed {this.cachedSemanticsNode?.id}";
            }
            else
            {
                if (this.shouldFormSemanticsNode)
                {
                    semanticsNodeStatus = "needs build";
                }
                else
                {
                    semanticsNodeStatus = "no semantics node";
                }
            }
            properties.add(new StringProperty("formedSemanticsNode", semanticsNodeStatus, quoted: false));
        }
        properties.add(new FlagProperty("isSemanticBoundary", value: ((_SemanticsConfigurationProvider__object)this.configProvider).effective.isSemanticBoundary, ifTrue: "semantic boundary"));
        properties.add(new FlagProperty("blocksSemantics", value: this.isBlockingPreviousSibling, ifTrue: "BLOCKS SEMANTICS"));
        if ((this.contributesToSemanticsTree && (checked((long)(this.siblingMergeGroups.Count)) != 0)))
        {
            properties.add(new StringProperty("Sibling group", this.siblingMergeGroups.ToString(), quoted: false));
        }
    }

    public virtual string toStringDeep(string prefixLineOne = "", string? prefixOtherLines = null, DiagnosticLevel minLevel = DiagnosticLevel.debug, long? wrapWidth = null) =>
        ((DiagnosticableTree)this).toStringDeep(prefixLineOne, prefixOtherLines, minLevel, wrapWidth);
}

public static partial class ObjectLibrary
{
    public static void debugDumpRenderObjectSemanticsTree()
    {
        if ((RendererBinding.instance.renderViews.Count() == 0))
        {
            global::Doroti.Framework.Foundation.PrintLibrary.debugPrint("No render tree root was added to the binding.");
            return;
        }
        global::Doroti.Framework.Foundation.PrintLibrary.debugPrint(string.Join("\n\n", new List<string>()));
    }
}

public static partial class ObjectLibrary
{
    internal static string _debugCollectRenderObjectSemanticsTrees(RenderObject root)
    {
        return ((RenderObject)root)._semantics.toStringDeep();
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

    internal _SemanticsGeometry__object(Rect? paintClipRect, Rect? semanticsClipRect, Matrix4 transform, Rect rect, bool hidden)
    {
        this.paintClipRect = paintClipRect;
        this.semanticsClipRect = semanticsClipRect;
        this.transform = transform;
        this.rect = rect;
        this.hidden = hidden;
    }

    internal static _SemanticsGeometry__object CreateRoot(Rect rect)
    {
        return new _SemanticsGeometry__object(paintClipRect: null, semanticsClipRect: null, transform: Matrix4.identity(), hidden: false, rect: DartRuntimePrimitives.RequireValue(rect));
    }

    public virtual bool isVisible => (!this.rect.isEmpty && !this.transform.isZero());
    public static _SemanticsGeometry__object computeChildGeometry(Matrix4? parentTransform, Rect? parentPaintClipRect, Rect? parentSemanticsClipRect, _RenderObjectSemantics__object parent, _RenderObjectSemantics__object child)
    {
        RenderObject childRenderObject = ((_RenderObjectSemantics__object)child).renderObject;
        RenderObject parentRenderObject = ((_RenderObjectSemantics__object)parent).renderObject;
        var childToCommonAncestor = new List<RenderObject> { childRenderObject };
        while ((((RenderObject)childRenderObject).depth > ((RenderObject)parentRenderObject).depth))
        {
            DartRuntimePrimitives.Assert(() => (((RenderObject)childRenderObject).parent is not null));
            childRenderObject = ((RenderObject)childRenderObject).parent!;
            childToCommonAncestor.Add(childRenderObject);
        }
        DartRuntimePrimitives.Assert(() => (checked((long)(childToCommonAncestor.Count)) >= 2L));
        DartRuntimePrimitives.Assert(() => DartRuntimePrimitives.Identical(childRenderObject, parentRenderObject));
        global::Doroti.Ui.Rect? paintClipRectLocal = default!;
        global::Doroti.Ui.Rect? semanticsClipRectLocal = default!;
        var transformLocal = Matrix4.identity();
        for (long i = (checked((long)(childToCommonAncestor.Count)) - 1L); (i > 0L); i -= 1L)
        {
            RenderObject nodeParent = childToCommonAncestor[(int)(i)];
            RenderObject node = childToCommonAncestor[(int)((i - 1L))];
            global::Doroti.Ui.Rect? localPaintClipInParent = _transformRect(nodeParent.describeApproximatePaintClip(node), transformLocal, (Func<Matrix4, Rect, Rect>)global::Doroti.Framework.Painting.MatrixUtils.transformRect);
            global::Doroti.Ui.Rect? localSemanticsClipInParent = _transformRect(nodeParent.describeSemanticsClip(node), transformLocal, (Func<Matrix4, Rect, Rect>)global::Doroti.Framework.Painting.MatrixUtils.transformRect);
            paintClipRectLocal = _intersectRects(paintClipRectLocal, localPaintClipInParent);
            semanticsClipRectLocal = (localSemanticsClipInParent ?? semanticsClipRectLocal?.intersect((localPaintClipInParent ?? DartRuntimePrimitives.RequireValue(semanticsClipRectLocal))));
            nodeParent.applyPaintTransform(node, transformLocal);
        }
        semanticsClipRectLocal = (semanticsClipRectLocal ?? _intersectRects(paintClipRectLocal, parentSemanticsClipRect));
        paintClipRectLocal = _intersectRects(paintClipRectLocal, parentPaintClipRect);
        if (((paintClipRectLocal is not null) || (semanticsClipRectLocal is not null)))
        {
            Matrix4 inverted = transformLocal.clone();
            var hasInverse = (inverted.invert() != 0.0);
            semanticsClipRectLocal = (hasInverse ? _transformRect(semanticsClipRectLocal, inverted, (Func<Matrix4, Rect, Rect>)global::Doroti.Framework.Painting.MatrixUtils.transformRect) : null);
            paintClipRectLocal = (hasInverse ? _transformRect(paintClipRectLocal, inverted, (Func<Matrix4, Rect, Rect>)global::Doroti.Framework.Painting.MatrixUtils.transformRect) : null);
        }
        if ((parentTransform is not null))
        {
            MatrixUtils.multiplyInPlace(parentTransform, transformLocal);
        }
        global::Doroti.Ui.Rect rectLocal = (semanticsClipRectLocal?.intersect(((_RenderObjectSemantics__object)child).renderObject.semanticBounds) ?? ((_RenderObjectSemantics__object)child).renderObject.semanticBounds);
        var isRectHidden = false;
        if ((paintClipRectLocal is not null))
        {
            Rect paintClipRect__259962__value262006 = DartRuntimePrimitives.RequireValue(paintClipRectLocal);
            global::Doroti.Ui.Rect paintRect = DartRuntimePrimitives.RequireValue(paintClipRect__259962__value262006).intersect(DartRuntimePrimitives.RequireValue(DartRuntimePrimitives.RequireValue(rectLocal)));
            isRectHidden = (paintRect.isEmpty && !DartRuntimePrimitives.RequireValue(rectLocal).isEmpty);
            if (!isRectHidden)
            {
                rectLocal = paintRect;
            }
        }
        return new _SemanticsGeometry__object(transform: transformLocal, paintClipRect: paintClipRectLocal, semanticsClipRect: semanticsClipRectLocal, rect: DartRuntimePrimitives.RequireValue(rectLocal), hidden: isRectHidden);
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect? _transformRect(Rect? rect, Matrix4 transform, Func<Matrix4, Rect, Rect> apply = default!)
    {
        if ((rect is null))
        {
            return null;
        }
        if ((DartRuntimePrimitives.RequireValue(rect).isEmpty || transform.isZero()))
        {
            return Rect.zero;
        }
        return apply(transform, DartRuntimePrimitives.RequireValue(rect));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

    internal static global::Doroti.Ui.Rect? _intersectRects(Rect? a, Rect? b)
    {
        if ((b is null))
        {
            return a;
        }
        return (a?.intersect(DartRuntimePrimitives.RequireValue(b)) ?? DartRuntimePrimitives.RequireValue(b));
        throw new InvalidOperationException("Dart control flow completed without a value.");
    }

}

public class DiagnosticsDebugCreator : DiagnosticsProperty<object>
{
    public DiagnosticsDebugCreator(object value) : base("debugCreator", value, level: DiagnosticLevel.hidden)
    {
    }

}
